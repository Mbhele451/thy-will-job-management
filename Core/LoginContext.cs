using System;
using System.Windows.Forms;
using Major_Project.UI.Forms.Common;

namespace Major_Project.Core
{
    public class LoginContext : ApplicationContext
    {
        private LoginForm _loginForm;
        private MDIForm _mdiForm;
        public LoginContext()
        {
            UserSession.ClearSession();
            ShowLogin();
        }

        private void ShowLogin()
        {
            if (_mdiForm != null)
            {
                _mdiForm.FormClosed -= MDIForm_FormClosed;
                _mdiForm.Dispose();
                _mdiForm = null;
            }

            _loginForm = new LoginForm();
            _loginForm.FormClosed += LoginForm_FormClosed;
            MainForm = _loginForm;
            _loginForm.Show();
            System.Diagnostics.Debug.WriteLine("LoginContext: LoginForm shown");
        }

        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("LoginContext: LoginForm closed");
            if (_loginForm != null)
            {
                _loginForm.FormClosed -= LoginForm_FormClosed;
                _loginForm = null;
            }

            if (UserSession.IsLoggedIn)
            {
                ShowMDI();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("LoginContext: Not logged in, exiting thread");
                ExitThread();
            }
        }

        private void ShowMDI()
        {
            _mdiForm = new MDIForm();
            _mdiForm.FormClosed += MDIForm_FormClosed;
            MainForm = _mdiForm;
            _mdiForm.Show();
            System.Diagnostics.Debug.WriteLine("LoginContext: MDIForm shown");
        }

        private void MDIForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("LoginContext: MDIForm closed");
            bool ended = _mdiForm?.SessionEnded == true;
            if (_mdiForm != null)
            {
                _mdiForm.FormClosed -= MDIForm_FormClosed;
                _mdiForm = null;
            }

            UserSession.ClearSession();
            ShowLogin(); // ✅ This should reopen the login form
            if (ended)
                WorkflowUi.Notify(_loginForm, "Your session ended. Sign in again to continue.");
        }
    }
}
