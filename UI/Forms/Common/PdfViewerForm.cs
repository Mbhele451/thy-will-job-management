using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Major_Project.UI.Forms.Common
{
    public partial class PdfViewerForm : Form
    {
        private byte[] _document;
        private string _fileName;
        private string _previewFolder;
        public PdfViewerForm()
        {
            InitializeComponent();
        }

        internal PdfViewerForm(byte[] document, string fileName) : this()
        {
            if (!IsPdf(document))
                throw new InvalidOperationException("The PDF is unavailable. Generate the invoice first.");
            _document = document;
            _fileName = Path.GetFileName(fileName);
            Text = "THY WILL | " + _fileName;
        }

        internal static bool IsPdf(byte[] document) => document != null && document.Length >= 5 && Encoding.ASCII.GetString(document, 0, 5) == "%PDF-";
        private async void PdfViewerForm_Shown(object sender, EventArgs e)
        {
            if (_document == null)
                return; // Designer preview has no document or browser initialization.
            try
            {
                _previewFolder = Path.Combine(Path.GetTempPath(), "ThyWillPdf", Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(_previewFolder);
                string path = Path.Combine(_previewFolder, "document.pdf");
                File.WriteAllBytes(path, _document);
                string documentUri = new Uri(path).AbsoluteUri;
                await pdfBrowser.EnsureCoreWebView2Async(null);
                if (IsDisposed)
                    return;
                pdfBrowser.CoreWebView2.Settings.AreDevToolsEnabled = false;
                pdfBrowser.CoreWebView2.Settings.IsWebMessageEnabled = false;
                pdfBrowser.CoreWebView2.NewWindowRequested += (browser, args) => args.Handled = true;
                pdfBrowser.CoreWebView2.NavigationStarting += (browser, args) =>
                {
                    if (args.Uri != "about:blank" && args.Uri.Split('#')[0] != documentUri)
                        args.Cancel = true;
                };
                pdfBrowser.CoreWebView2.NavigationCompleted += (browser, args) => lblStatus.Text = args.IsSuccess ? _fileName + " — use the PDF toolbar to zoom, search or print." : "The PDF could not be displayed. You can still save a copy.";
                pdfBrowser.CoreWebView2.Navigate(documentUri);
            }
            catch (Exception ex)
            {
                MajorProject.Business.Helpers.Logger.Error("Opening desktop PDF preview.", ex);
                lblStatus.Text = "PDF preview is unavailable. Check the Microsoft Edge WebView2 Runtime, or use Save a copy.";
            }
        }

        private void SaveCopy_Click(object sender, EventArgs e)
        {
            if (_document == null)
                return;
            savePdfDialog.FileName = _fileName;
            if (savePdfDialog.ShowDialog(this) != DialogResult.OK)
                return;
            try
            {
                File.WriteAllBytes(savePdfDialog.FileName, _document);
                lblStatus.Text = "PDF copy saved successfully.";
            }
            catch (Exception ex)
            {
                MajorProject.Business.Helpers.Logger.Error("Saving PDF preview copy.", ex);
                lblStatus.Text = "The PDF could not be saved. Choose a writable location and try again.";
            }
        }

        private void CloseViewer_Click(object sender, EventArgs e) => Close();
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            pdfBrowser.Dispose();
            if (_previewFolder != null)
            {
                try
                {
                    File.Delete(Path.Combine(_previewFolder, "document.pdf"));
                    Directory.Delete(_previewFolder);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            base.OnFormClosed(e);
        }
    }
}
