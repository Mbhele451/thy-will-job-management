using MajorProject.Business.Services;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.UI;

namespace MajorProject.Web
{
    public partial class Contact : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblContactMessage.Visible = false;
                vsContact.Visible = false;
            }
        }

        protected async void btnSendMessage_Click(object sender, EventArgs e)
        {
            Page.Validate("Contact");
            if (!Page.IsValid)
            {
                vsContact.Visible = true;
                lblContactMessage.Visible = false;
                return;
            }

            vsContact.Visible = false;
            string apiKey = ConfigurationManager.AppSettings["MailjetApiKey"];
            string apiSecret = ConfigurationManager.AppSettings["MailjetApiSecret"];
            string fromEmail = ConfigurationManager.AppSettings["MailjetFromEmail"];
            string fromName = ConfigurationManager.AppSettings["MailjetFromName"];
            string notificationEmail = ConfigurationManager.AppSettings["BookingNotifyEmail"];
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret) || string.IsNullOrWhiteSpace(fromEmail) || string.IsNullOrWhiteSpace(fromName) || string.IsNullOrWhiteSpace(notificationEmail))
            {
                ShowMessage("The support email service is currently unavailable. Please try again later.", false);
                return;
            }

            string fullName = txtContactName.Text.Trim();
            string phone = txtContactPhone.Text.Trim();
            string email = txtContactEmail.Text.Trim();
            string subject = ddlSubject.SelectedValue;
            string message = txtMessage.Text.Trim();
            string emailSubject = "THY WILL Contact Enquiry - " + subject;
            string htmlBody = BuildContactEmailBody(fullName, phone, email, subject, message);
            try
            {
                MailjetEmailService mailjet = new MailjetEmailService(apiKey, apiSecret, fromEmail, fromName);
                bool emailSent = await mailjet.SendEmailAsync(notificationEmail, "Thy Will Support", emailSubject, htmlBody);
                if (emailSent)
                {
                    ShowMessage("Thank you, " + Server.HtmlEncode(fullName) + ". Your enquiry has been sent to Thy Will Support. We will review your message and get back to you.", true);
                    ClearForm();
                }
                else
                {
                    ShowMessage("We could not send your enquiry right now. Please try again later or contact Thy Will Support by phone.", false);
                }
            }
            catch (Exception)
            {
                ShowMessage("We could not send your enquiry right now. Please try again later or contact Thy Will Support by phone.", false);
            }
        }

        private string BuildContactEmailBody(string fullName, string phone, string email, string subject, string message)
        {
            string safeName = Server.HtmlEncode(fullName);
            string safePhone = Server.HtmlEncode(phone);
            string safeEmail = Server.HtmlEncode(email);
            string safeSubject = Server.HtmlEncode(subject);
            string safeMessage = Server.HtmlEncode(message);
            safeMessage = safeMessage.Replace(Environment.NewLine, "<br />");
            safeMessage = safeMessage.Replace("\n", "<br />");
            string htmlBody = "<html>" + "<body style=\"font-family: Arial, sans-serif; color: #222;\">" + "<h2>New THY WILL Electrical Contact Enquiry</h2>" + "<p>A new enquiry has been submitted through the THY WILL Electrical website.</p>" + "<hr />" + "<p><strong>Customer Name:</strong><br />" + safeName + "</p>" + "<p><strong>Phone Number:</strong><br />" + safePhone + "</p>" + "<p><strong>Email Address:</strong><br />" + safeEmail + "</p>" + "<p><strong>Enquiry Type:</strong><br />" + safeSubject + "</p>" + "<p><strong>Message:</strong><br />" + safeMessage + "</p>" + "<hr />" + "<p style=\"font-size: 12px; color: #666;\">" + "This enquiry was submitted through the THY WILL Electrical public contact form." + "</p>" + "</body>" + "</html>";
            return htmlBody;
        }

        private void ShowMessage(string message, bool success)
        {
            lblContactMessage.Visible = true;
            if (success)
            {
                lblContactMessage.CssClass = "tw-booking-message d-block mb-3 alert alert-success";
            }
            else
            {
                lblContactMessage.CssClass = "tw-booking-message d-block mb-3 alert alert-warning";
            }

            lblContactMessage.Text = message;
        }

        private void ClearForm()
        {
            txtContactName.Text = string.Empty;
            txtContactPhone.Text = string.Empty;
            txtContactEmail.Text = string.Empty;
            if (ddlSubject.Items.Count > 0)
            {
                ddlSubject.SelectedIndex = 0;
            }

            txtMessage.Text = string.Empty;
        }
    }
}
