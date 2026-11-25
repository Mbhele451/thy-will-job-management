using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;

namespace Major_Project.UI.Forms.Reports
{
    public partial class ReportsForm : Form
    {
        private readonly string reportUrl = Environment.GetEnvironmentVariable("THYWILL_POWERBI_URL") ?? "about:blank";
        public ReportsForm()
        {
            InitializeComponent();
        }

        private async void Reports_Load(object sender, EventArgs e)
        {
            try
            {
                await webViewReport.EnsureCoreWebView2Async(null);
                webViewReport.CoreWebView2.Navigate(reportUrl);
            }
            catch (Exception ex)
            {
                Common.WorkflowUi.ShowError(this, $"Error loading report: {ex.Message}");
            }
        }
    }
}
