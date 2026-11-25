using Major_Project.Core;
using Major_Project.Utilities;
using MajorProject.Business.Services; // ← ADD THIS for EmailQueueService
using MajorProject.Business.Helpers; // Already there for Logger
using System;
using System.Windows.Forms;

namespace Major_Project
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Global exception handlers
            Application.ThreadException += (sender, e) =>
            {
                Logger.Error("Unhandled UI exception", e.Exception);
                UIHelper.ShowError("An unexpected error occurred. Please restart the application.");
            };
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                Logger.Error("Unhandled domain exception", e.ExceptionObject as Exception);
            // Can't show UI here if the app is about to crash, but we log it.
            };
            // ── INITIALIZE EMAIL QUEUE SERVICE ──────────────────────────
            try
            {
                EmailQueueService.Initialize(Properties.Settings.Default.MailjetApiKey, Properties.Settings.Default.MailjetApiSecret, Properties.Settings.Default.MailjetFromEmail, Properties.Settings.Default.MailjetFromName, maxRetries: 3, retryDelaySeconds: 30, batchSize: 10, bookingConnectionString: Properties.Settings.Default.GroupWst34ConnectionString);
                Logger.Info("EmailQueueService initialized successfully.");
            }
            catch (Exception ex)
            {
                // Log the error but don't crash the app – email is optional
                Logger.Error("Failed to initialize EmailQueueService. OTP emails will not work.", ex);
            // Optionally show a warning to the user (if you want)
            // UIHelper.ShowWarning("Email service is unavailable. OTP emails will not be sent.");
            }

            Application.Run(new LoginContext());
        }
    }
}
