using System.Threading.Tasks;
using System;
using System.Windows.Forms;
using Major_Project.Core;
using MajorProject.Business.Services;

namespace Major_Project.UI.Forms.Common
{
    internal static class CustomerRegistration
    {
        internal static async Task<bool> ShowAsync(Control owner)
        {
            var service = new CustomerService(WorkflowUi.ConnectionString);
            if (!UserSession.Check() || !UserSession.HasRole("SysAdmin"))
                return false;
            using (var dialog = new WorkflowPanel("Create customer account"))
            {
                var view = new RegistrationView();
                dialog.UseContent().Controls.Add(view);
                var name = view.FullName;
                var phone = view.Phone;
                var email = view.Email;
                var address = view.Address;
                var area = view.Area;
                area.Items.AddRange(new object[] { "Umhlanga", "Bearea", "Morningside", "KwaMashu", "Durban North", "PineTown" });
                area.SelectedIndex = 0;
                var type = view.IdentityType;
                type.Items.AddRange(new object[] { "South African ID", "Passport", "Company registration" });
                type.SelectedIndex = 0;
                var document = view.Document;
                var country = view.Country;
                name.MaxLength = email.MaxLength = 100;
                phone.MaxLength = 10;
                address.MaxLength = 255;
                document.MaxLength = 20;
                country.MaxLength = 50;
                country.Enabled = false;
                type.SelectedIndexChanged += (sender, args) =>
                {
                    country.Enabled = type.SelectedIndex == 1;
                    document.Clear();
                    country.Clear();
                };
                var password = view.Password;
                password.MaxLength = 128;
                password.UseSystemPasswordChar = true;
                var confirm = view.ConfirmPassword;
                confirm.MaxLength = 128;
                confirm.UseSystemPasswordChar = true;
                dialog.BindSubmit(view.SubmitButton, () =>
                {
                    if (password.Text != confirm.Text)
                        throw new ArgumentException("The password confirmation does not match.");
                    bool company = type.SelectedIndex == 2;
                    bool passport = type.SelectedIndex == 1;
                    string customerType = company ? "Business" : "Residential";
                    string id = document.Text.Trim();
                    string sa = !company && !passport ? id : "";
                    string companyNumber = company ? id : "";
                    string passportNumber = passport ? id : "";
                    string passportCountry = passport ? country.Text.Trim() : "";
                    service.CreateCustomerBySysAdmin(UserSession.StaffID, UserSession.ClientIp, name.Text, phone.Text, email.Text, password.Text, address.Text, area.Text, "Durban", "KwaZulu-Natal", customerType, sa, companyNumber, passportNumber, passportCountry);
                }, authenticated: true);
                return await dialog.OpenAsync(owner);
            }
        }
    }
}
