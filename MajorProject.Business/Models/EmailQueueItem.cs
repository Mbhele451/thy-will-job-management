using System;

namespace MajorProject.Business.Models
{
    public class EmailQueueItem
    {
        public int Id { get; set; }
        public string ToEmail { get; set; }
        public string ToName { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public int RetryCount { get; set; }
        public int MaxRetries { get; set; } = 3;
        public DateTime CreatedAt { get; set; }
        public DateTime? LastAttemptAt { get; set; }
        public DateTime? NextRetryAt { get; set; }
        public bool IsSent { get; set; }
        public string LastError { get; set; }

        public bool CanRetry()
        {
            return !IsSent && RetryCount < MaxRetries && (NextRetryAt == null || NextRetryAt <= DateTime.Now);
        }
    }
}
