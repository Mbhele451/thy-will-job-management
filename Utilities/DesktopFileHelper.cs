using System;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace Major_Project.Utilities
{
    public static class DesktopFileHelper
    {
        /// <summary>
        /// Saves the PDF bytes to the Reports folder and opens it.
        /// </summary>
        public static void SaveAndOpenPdf(byte[] pdfData, int jobId)
        {
            try
            {
                string reportFolder = Path.Combine(Application.StartupPath, "Reports");
                if (!Directory.Exists(reportFolder))
                    Directory.CreateDirectory(reportFolder);
                string pdfPath = Path.Combine(reportFolder, $"Invoice_{jobId}.pdf");
                File.WriteAllBytes(pdfPath, pdfData);
                Process.Start(pdfPath);
            }
            catch (Exception ex)
            {
                UIHelper.ShowError($"Could not save or open invoice: {ex.Message}");
            }
        }
    }
}
