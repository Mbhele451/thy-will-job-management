using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Major_Project.Core;
using MajorProject.Business.Helpers;
using MajorProject.Business.Services;

namespace Major_Project.UI.Forms.Common
{
    public partial class LoginForm : Form
    {
        private readonly AuthenticationService _authService;
        // Native Win32 API call to safely set watermark cue banners without overriding WndProc delegates
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);
        private const int EM_SETCUEBANNER = 0x1501;
        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthenticationService(Properties.Settings.Default.GroupWst34ConnectionString);
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Set native watermark place-holders safely
            SetWatermark(textBox1, "Enter username or email");
            SetWatermark(textBox2, "Enter password");
            // Enable password masking by default
            textBox2.UseSystemPasswordChar = true;
        }

        private void SetWatermark(TextBox textBox, string watermarkText)
        {
            if (textBox.IsHandleCreated)
            {
                SendMessage(textBox.Handle, EM_SETCUEBANNER, (IntPtr)1, watermarkText);
            }
            else
            {
                textBox.HandleCreated += (s, e) =>
                {
                    SendMessage(textBox.Handle, EM_SETCUEBANNER, (IntPtr)1, watermarkText);
                };
            }
        }

        private bool signingIn;
        private async void button1_Click(object sender, EventArgs e)
        {
            if (signingIn)
                return;
            string username = textBox1.Text.Trim();
            string password = textBox2.Text;
            if (username.Length == 0 || string.IsNullOrWhiteSpace(password))
            {
                statusLabel.Text = "Enter your " + (username.Length == 0 ? "username or email." : "password.");
                (username.Length == 0 ? textBox1 : textBox2).Focus();
                return;
            }

            signingIn = true;
            button1.Enabled = linkLabel1.Enabled = false;
            textBox1.ReadOnly = textBox2.ReadOnly = true;
            statusLabel.Text = "Signing in…";
            bool success = false;
            try
            {
                string connection = Properties.Settings.Default.GroupWst34ConnectionString;
                var attempt = System.Threading.Tasks.Task.Run(() =>
                {
                    int id;
                    string name, role;
                    if (_authService.AuthenticateUser(username, password, out id, out name, out role))
                        UserSession.Begin(connection, true, id, name, role);
                    else
                        return false;
                    return true;
                });
                if (await System.Threading.Tasks.Task.WhenAny(attempt, System.Threading.Tasks.Task.Delay(2500)) != attempt)
                    statusLabel.Text = "The connection is taking longer than usual. Still signing in…";
                success = await attempt;
                if (!success)
                {
                    statusLabel.Text = "Sign-in details were not accepted. Check your username and password.";
                    textBox2.Clear();
                    textBox2.Focus();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Desktop sign-in failed.", ex);
                var sql = ex.GetBaseException() as System.Data.SqlClient.SqlException ?? ex as System.Data.SqlClient.SqlException;
                statusLabel.Text = ex is TimeoutException || sql?.Number == -2 ? "The connection timed out. Check your network and try again." : "Sign-in is unavailable. Check your connection and try again.";
            }
            finally
            {
                signingIn = false;
                button1.Enabled = linkLabel1.Enabled = true;
                textBox1.ReadOnly = textBox2.ReadOnly = false;
            }

            if (success)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (signingIn && e.CloseReason != CloseReason.WindowsShutDown)
            {
                e.Cancel = true;
                statusLabel.Text = "Please wait while the sign-in request finishes.";
            }

            base.OnFormClosing(e);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() =>
            {
                Application.Exit();
            }));
        }

        private async void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            using (var panel = new WorkflowPanel("Account recovery"))
            using (var recovery = new ResetPasswordForm())
            {
                var content = panel.UseContent();
                recovery.TopLevel = false;
                recovery.FormBorderStyle = FormBorderStyle.None;
                recovery.WindowState = FormWindowState.Normal;
                recovery.Dock = DockStyle.Fill;
                recovery.FormClosed += (s, args) => panel.Finish(false);
                content.Controls.Add(recovery);
                var opened = panel.OpenAsync(this);
                recovery.Show();
                await opened;
                if (recovery.ResetSucceeded && !IsDisposed)
                    WorkflowUi.Notify(this, "Password reset successfully. Sign in with your new password.");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle password masking safely
            textBox2.UseSystemPasswordChar = !checkBox1.Checked;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _authService.Dispose();
            base.OnFormClosed(e);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

    }
}
