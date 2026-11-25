using System;
using Major_Project.UI.Forms.Common;

namespace Major_Project.Utilities
{
    /// <summary>
    /// Provides UI utilities and common dialog operations.
    /// </summary>
    public static class UIHelper
    {
        /// <summary>
        /// Shows a success message.
        /// </summary>
        public static void ShowSuccess(string message, string title = "Success")
        {
            WorkflowUi.Notify(null, message);
        }

        /// <summary>
        /// Shows an error message.
        /// </summary>
        public static void ShowError(string message, string title = "Error")
        {
            WorkflowUi.ShowError(null, message);
        }

        /// <summary>
        /// Shows a warning message.
        /// </summary>
        public static void ShowWarning(string message, string title = "Warning")
        {
            WorkflowUi.ShowError(null, message);
        }

        /// <summary>
        /// Shows an information message.
        /// </summary>
        public static void ShowInfo(string message, string title = "Information")
        {
            WorkflowUi.Notify(null, message);
        }
    }
}
