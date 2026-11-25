using MajorProject.Business.Helpers; // Logger moved here
using MajorProject.Business.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MajorProject.Business.Services
{
    public class EmailQueueService : IDisposable
    {
        private static EmailQueueService _instance;
        private static readonly object _lock = new object ();
        public static EmailQueueService Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("EmailQueueService has not been initialized. Call EmailQueueService.Initialize(...) first.");
                return _instance;
            }
        }

        private readonly ConcurrentQueue<EmailQueueItem> _queue = new ConcurrentQueue<EmailQueueItem>();
        private readonly List<EmailQueueItem> _pendingItems = new List<EmailQueueItem>();
        private readonly MailjetEmailService _emailService;
        private readonly Timer _timer;
        private readonly int _maxRetries;
        private readonly int _retryDelaySeconds;
        private readonly int _batchSize;
        private int _isProcessing;
        private readonly MajorProject.Data.Data.JobChangeDataAccess _bookingEmails;
        private bool _disposed;
        /// <summary>
        /// Initializes the EmailQueueService singleton with Mailjet configuration.
        /// Must be called once before using Instance.
        /// </summary>
        public static void Initialize(string apiKey, string apiSecret, string fromEmail, string fromName, int maxRetries = 3, int retryDelaySeconds = 30, int batchSize = 10, string bookingConnectionString = null)
        {
            lock (_lock)
            {
                if (_instance != null)
                {
                    Logger.Warning("EmailQueueService already initialized. Ignoring duplicate call.");
                    return;
                }

                var emailService = new MailjetEmailService(apiKey, apiSecret, fromEmail, fromName);
                _instance = new EmailQueueService(emailService, maxRetries, retryDelaySeconds, batchSize, bookingConnectionString);
                Logger.Info("EmailQueueService initialized successfully.");
            }
        }

        // Private constructor – creates the timer and starts processing.
        private EmailQueueService(MailjetEmailService emailService, int maxRetries, int retryDelaySeconds, int batchSize, string bookingConnectionString)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _maxRetries = maxRetries;
            _retryDelaySeconds = retryDelaySeconds;
            _batchSize = batchSize;
            if (!string.IsNullOrWhiteSpace(bookingConnectionString))
                _bookingEmails = new MajorProject.Data.Data.JobChangeDataAccess(bookingConnectionString);
            // Start timer to process queue every 30 seconds
            _timer = new Timer(ProcessQueue, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
            Logger.Info($"EmailQueueService created: MaxRetries={maxRetries}, RetryDelay={retryDelaySeconds}s, BatchSize={batchSize}");
        }

        /// <summary>
        /// Adds an email to the queue for sending.
        /// </summary>
        public void Enqueue(string toEmail, string toName, string subject, string htmlBody)
        {
            var item = new EmailQueueItem
            {
                ToEmail = toEmail,
                ToName = toName,
                Subject = subject,
                HtmlBody = htmlBody,
                RetryCount = 0,
                MaxRetries = _maxRetries,
                CreatedAt = DateTime.Now,
                IsSent = false
            };
            _queue.Enqueue(item);
            Logger.Info($"Email queued for {toEmail}. Queue size: {_queue.Count}");
        }

        /// <summary>
        /// Enqueues an OTP email for password reset.
        /// </summary>
        public void EnqueueOtpEmail(string toEmail, string fullName, string otp)
        {
            string subject = "Password Reset OTP";
            string htmlBody = $@"
                <html>
                <body>
                    <h2>Password Reset Request</h2>
                    <p>Hi {fullName},</p>
                    <p>Your OTP is: <strong>{otp}</strong></p>
                    <p>This code expires in 5 minutes.</p>
                    <p>If you did not request this, please ignore this email.</p>
                    <br/>
                    <p>Regards,<br/>Support Team</p>
                </body>
                </html>
            ";
            Enqueue(toEmail, fullName, subject, htmlBody);
        }

        public static void QueueAccountWelcome(string email, string username)
        {
            string login = System.Net.WebUtility.HtmlEncode(username);
            string body = $"<p>Your THY WILL account is ready.</p><p>Username: <strong>{login}</strong></p>"
                + "<p>Before your first sign-in, open the THY WILL website and select Forgot Password on the sign-in page. "
                + "Enter the email address that received this message. Use the code in the separate password reset email "
                + "within five minutes, enter and confirm your new password, then select Update Password. "
                + "Return to sign-in and use the username above with your new password.</p>";
            try
            {
                Instance.Enqueue(email, username, "Your THY WILL account", body);
            }
            catch (Exception ex)
            {
                Logger.Error("Account created but welcome email could not be queued.", ex);
            }
        }

        /// <summary>
        /// Processes the email queue - sends emails and handles retries.
        /// </summary>
        private async void ProcessQueue(object state)
        {
            if (Interlocked.Exchange(ref _isProcessing, 1) != 0)
                return;
            try
            {
                // 1. Move items from pending back to queue if they are ready for retry
                MovePendingToQueue();
                // 2. Process items from the queue
                await ProcessBatchAsync();
                if (_bookingEmails != null)
                {
                    await SendBookingEmailsAsync();
                    await SendFeeEmailsAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Queue processing error: {ex.Message}", ex);
            }
            finally
            {
                Interlocked.Exchange(ref _isProcessing, 0);
            }
        }

        private async Task SendBookingEmailsAsync()
        {
            for (int count = 0; count < _batchSize; count++)
            {
                Guid lease = Guid.NewGuid();
                var messages = _bookingEmails.ClaimBookingEmail(lease);
                if (messages.Rows.Count == 0)
                    return;
                var row = messages.Rows[0];
                string subject = (Convert.ToBoolean(row["IsNewBooking"]) ? "Booking confirmed" : "Booking rescheduled") + " — Job #" + row["JobID"];
                string date = Convert.ToDateTime(row["PromiseDate"]).ToString("dd MMMM yyyy");
                string body = $"<p>Your service booking for job #{row["JobID"]} is confirmed for <strong>{date}</strong>.</p>" + "<p>You can view your updated booking and any approved change fees in your customer account.</p>";
                string recipient = Convert.ToString(row["Recipient"]);
                if (await _emailService.SendEmailAsync(recipient, recipient, subject, body))
                    _bookingEmails.MarkBookingEmailSent(Convert.ToInt64(row["EmailID"]), lease);
            }
        }

        private async Task SendFeeEmailsAsync()
        {
            for (int count = 0; count < _batchSize; count++)
            {
                Guid lease = Guid.NewGuid();
                var claims = _bookingEmails.ClaimFeeEmail(lease);
                if (claims.Rows.Count == 0)
                    return;
                var claim = claims.Rows[0];
                int request = Convert.ToInt32(claim["RequestID"]);
                string recipient = Convert.ToString(claim["Recipient"]);
                var details = _bookingEmails.FeeEmailDetails(request, lease);
                if (details.Rows.Count == 0)
                    continue;
                var row = details.Rows[0];
                string subject = row["RequestType"] + " approved for job #" + row["JobID"];
                const string intro = "<p>Your booking change has been approved.</p>";
                bool sent = Convert.ToDecimal(row["Fee"]) == 0
                    ? await _emailService.SendEmailAsync(recipient, recipient, subject, intro + "<p>No fee is due.</p>")
                    : await _emailService.SendEmailWithAttachmentAsync(recipient, recipient, subject,
                        intro + "<p>Your fee invoice is attached. You can view it and make a demo payment in Booking changes &amp; fees.</p>",
                        InvoicePdfService.GenerateFeeInvoicePdf(row), "FEE-" + request + ".pdf");
                if (sent)
                    _bookingEmails.MarkFeeEmailSent(request, lease);
            }
        }

        private void MovePendingToQueue()
        {
            var readyItems = _pendingItems.Where(x => x.CanRetry()).ToList();
            foreach (var item in readyItems)
            {
                _pendingItems.Remove(item);
                _queue.Enqueue(item);
                Logger.Info($"Retrying email for {item.ToEmail} (Attempt {item.RetryCount + 1}/{item.MaxRetries})");
            }
        }

        private async Task ProcessBatchAsync()
        {
            var batch = new List<EmailQueueItem>();
            // Dequeue up to batchSize items
            while (batch.Count < _batchSize && _queue.TryDequeue(out var item))
            {
                batch.Add(item);
            }

            if (!batch.Any())
                return;
            Logger.Info($"Processing {batch.Count} emails from queue");
            var tasks = batch.Select(item => SendEmailWithRetryAsync(item));
            await Task.WhenAll(tasks);
        }

        private async Task SendEmailWithRetryAsync(EmailQueueItem item)
        {
            try
            {
                item.LastAttemptAt = DateTime.Now;
                bool success = await _emailService.SendEmailAsync(item.ToEmail, item.ToName, item.Subject, item.HtmlBody);
                if (success)
                {
                    item.IsSent = true;
                    Logger.Info($"Email sent successfully to {item.ToEmail}");
                }
                else
                {
                    item.RetryCount++;
                    item.LastError = "Email service returned false";
                    HandleFailedItem(item);
                }
            }
            catch (Exception ex)
            {
                item.RetryCount++;
                item.LastError = ex.Message;
                Logger.Error($"Email send error for {item.ToEmail}: {ex.Message}", ex);
                HandleFailedItem(item);
            }
        }

        private void HandleFailedItem(EmailQueueItem item)
        {
            if (item.RetryCount < item.MaxRetries)
            {
                // Calculate exponential backoff delay
                int delaySeconds = _retryDelaySeconds * (int)Math.Pow(2, item.RetryCount - 1);
                item.NextRetryAt = DateTime.Now.AddSeconds(delaySeconds);
                _pendingItems.Add(item);
                Logger.Warning($"Email to {item.ToEmail} failed (Attempt {item.RetryCount}/{item.MaxRetries}). Next retry at {item.NextRetryAt.Value:HH:mm:ss}");
            }
            else
            {
                item.IsSent = false;
                Logger.Error($"Email to {item.ToEmail} failed after {item.MaxRetries} attempts. Last error: {item.LastError}");
            // Optionally: Send alert to admin or save to failed queue for manual review
            // You can add a callback here or write to a separate log file
            }
        }

        /// <summary>
        /// Gets the current queue status.
        /// </summary>
        public QueueStatus GetStatus()
        {
            return new QueueStatus
            {
                QueueCount = _queue.Count,
                PendingCount = _pendingItems.Count,
                TotalFailed = _pendingItems.Count(x => x.RetryCount >= x.MaxRetries),
                Items = _pendingItems.ToList()
            };
        }

        /// <summary>
        /// Clears all pending items (use with caution).
        /// </summary>
        public void ClearPendingItems()
        {
            _pendingItems.Clear();
            Logger.Info("All pending items cleared");
        }

        /// <summary>
        /// Force immediate processing of the queue.
        /// </summary>
        public async Task ForceProcessAsync()
        {
            if (Volatile.Read(ref _isProcessing) != 0)
                return;
            // Cancel any pending timer callback and process immediately
            await Task.Run(() => ProcessQueue(null));
        }

#region IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _timer?.Dispose();
                    // Clear the queue by dequeuing all items
                    while (_queue.TryDequeue(out _))
                    {
                    }

                    _pendingItems.Clear();
                }

                _disposed = true;
            }
        }
#endregion
    }

    public class QueueStatus
    {
        public int QueueCount { get; set; }
        public int PendingCount { get; set; }
        public int TotalFailed { get; set; }
        public List<EmailQueueItem> Items { get; set; }
    }
}
