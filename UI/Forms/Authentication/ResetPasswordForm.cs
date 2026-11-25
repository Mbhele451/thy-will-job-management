using MajorProject.Business.Services;
using Major_Project.Utilities;
using System;
using System.Drawing;
using System.Windows.Forms;
using MajorProject.Business.Helpers;

namespace Major_Project.UI.Forms.Common
{
    public partial class ResetPasswordForm : Form
    {
        private readonly HelpManager _helpManager;
        private readonly AuthenticationService _authService;
        private int _pendingAccountId;
        internal bool ResetSucceeded { get; private set; }

        private Label[] _checklistLabels;
        private Label[] _checklistTexts;
        private ErrorProvider _errorProvider;
        public ResetPasswordForm()
        {
            InitializeComponent();
            _helpManager = new HelpManager();
            _authService = new AuthenticationService(Properties.Settings.Default.GroupWst34ConnectionString);
        }

        private void ResetPasswordForm_Load(object sender, EventArgs e)
        {
            RegisterHelpTooltips();
            textBox2.Enabled = false;
            textBox3.Enabled = true;
            textBox4.Enabled = true;
            button2.Enabled = false;
            // ── Password checklist setup ──
            _errorProvider = new ErrorProvider();
            InitializePasswordChecklist();
            // Hook events for real-time validation
            textBox3.TextChanged += textBox3_TextChanged;
            textBox3.Leave += textBox3_Leave;
            textBox4.TextChanged += textBox4_TextChanged;
            textBox4.Leave += textBox4_Leave;
        }

        private void button3_Click(object sender, EventArgs e) => this.Close();
        private async void button1_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            if (!ValidationHelper.IsValidEmail(email))
            {
                UIHelper.ShowError("Please enter a valid email address.");
                return;
            }

            button1.Enabled = false;
            try
            {
                var accounts = _authService.GetStaffByEmail(email);
                bool sent = accounts.Rows.Count == 1 && await _authService.GenerateAndSendOtpAsync(email);
                if (IsDisposed)
                    return;
                if (!sent)
                {
                    UIHelper.ShowError("A verification code could not be sent. Check your email address and try again.");
                    return;
                }

                _pendingAccountId = Convert.ToInt32(accounts.Rows[0]["StaffID"]);
                textBox1.ReadOnly = true;
                textBox2.Clear();
                textBox2.Enabled = true;
                textBox2.Focus();
                button2.Enabled = true;
                UIHelper.ShowSuccess("Check your email for the verification code.");
            }
            catch (Exception ex)
            {
                Logger.Error("Unable to request a desktop password reset code.", ex);
                if (!IsDisposed)
                    UIHelper.ShowError("A verification code could not be sent. Please try again.");
            }
            finally
            {
                if (!IsDisposed)
                    button1.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_pendingAccountId <= 0)
            {
                UIHelper.ShowError("Request a verification code first.");
                return;
            }

            string otp = textBox2.Text.Trim();
            string password = textBox3.Text;
            if (string.IsNullOrWhiteSpace(otp) || !ValidationHelper.IsValidPassword(password) || password != textBox4.Text)
            {
                UIHelper.ShowError("Enter the verification code and confirm a password that meets the requirements.");
                return;
            }

            try
            {
                bool valid = _authService.ValidateOtp(_pendingAccountId, otp);
                if (!valid)
                {
                    UIHelper.ShowError("The verification code is invalid or expired.");
                    return;
                }

                _authService.ResetPassword(_pendingAccountId, password);
                ResetSucceeded = true;
                Close();
            }
            catch (Exception ex)
            {
                Logger.Error("Desktop password reset failed.", ex);
                UIHelper.ShowError("The password could not be reset. Please try again.");
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            _helpManager.DisplayHelpToolTip(btnHelp, textBox1);
        }

        private void RegisterHelpTooltips()
        {
            _helpManager.RegisterControl(textBox1, "Email Field", "Enter your registered email address.");
            _helpManager.RegisterControl(textBox2, "OTP Field", "Enter the 6-digit one-time code.");
            _helpManager.RegisterControl(textBox3, "New Password", "Enter your new password.");
            _helpManager.RegisterControl(textBox4, "Confirm Password", "Re-type your new password.");
            _helpManager.RegisterControl(button1, "Send OTP", "Request a new one-time code.");
            _helpManager.RegisterControl(button2, "Reset Password", "Confirm password update.");
            _helpManager.RegisterControl(button3, "Cancel", "Close and return to Login.");
            _helpManager.RegisterContainer(this);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _helpManager?.Dispose();
            _errorProvider?.Dispose();
            _authService.Dispose();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void InitializePasswordChecklist()
        {
            _checklistLabels = new[]
            {
                checklistStatus1,
                checklistStatus2,
                checklistStatus3,
                checklistStatus4,
                checklistStatus5
            };
            _checklistTexts = new[]
            {
                label5,
                label6,
                label7,
                label8,
                label9
            };
            UpdatePasswordChecklist(string.Empty);
        }

        private void UpdatePasswordChecklist(string password)
        {
            if (panelPasswordChecklist == null)
                return;
            var result = ValidationHelper.GetPasswordStrength(password);
            bool hasAnyInput = !string.IsNullOrEmpty(password);
            panelPasswordChecklist.Visible = hasAnyInput;
            if (!hasAnyInput)
                return;
            bool[] checks =
            {
                result.HasMinLength,
                result.HasLowercase,
                result.HasUppercase,
                result.HasDigit,
                result.HasSpecial
            };
            for (int i = 0; i < _checklistLabels.Length; i++)
            {
                bool isMet = checks[i];
                _checklistLabels[i].Text = isMet ? "✔" : "✘";
                _checklistLabels[i].ForeColor = isMet ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                _checklistTexts[i].ForeColor = isMet ? System.Drawing.Color.DarkGreen : System.Drawing.Color.Gray;
            }

            // Update the confirm password field if it has content
            if (!string.IsNullOrEmpty(textBox4.Text))
            {
                ValidateConfirmPassword();
            }
        }

        private void ValidateConfirmPassword()
        {
            if (string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
                return;
            bool passwordsMatch = textBox3.Text == textBox4.Text;
            _errorProvider?.SetError(textBox4, passwordsMatch ? "" : "Passwords do not match.");
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            UpdatePasswordChecklist(textBox3.Text);
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            // Validate password when focus leaves
            string password = textBox3.Text;
            if (string.IsNullOrEmpty(password))
            {
                _errorProvider.SetError(textBox3, "Password is required.");
                return;
            }

            if (!ValidationHelper.IsValidPassword(password))
            {
                _errorProvider.SetError(textBox3, "Password does not meet all requirements. See checklist.");
            // Don't force focus back, but show error
            }
            else
            {
                _errorProvider.SetError(textBox3, "");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            ValidateConfirmPassword();
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            ValidateConfirmPassword();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label5_Click(object sender, EventArgs e)
        {
        }

        private void label5_Click_1(object sender, EventArgs e)
        {
        }

        private void label7_Click(object sender, EventArgs e)
        {
        }

        private void label9_Click(object sender, EventArgs e)
        {
        }
    }
}
