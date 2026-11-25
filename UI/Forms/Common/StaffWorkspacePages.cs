using System.Threading.Tasks;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Major_Project.Core;
using MajorProject.Business.Helpers;
using MajorProject.Business.Services;

namespace Major_Project.UI.Forms.Common
{
    internal static class StaffWorkspacePages
    {
        internal static void AddProfile(WorkspaceTabs tabs, bool technician = false)
        {
            var page = tabs.TabPages["ProfileWorkspacePage"];
            var view = page.Controls.OfType<StaffProfileView>().Single();
            var details = view.Details;
            view.SetMyAvailabilityButton.Visible = technician;
            var staff = new StaffService(WorkflowUi.ConnectionString);
            DataRow CurrentStaff()
            {
                var result = staff.GetStaffByID(UserSession.StaffID);
                if (result.Rows.Count != 1)
                    throw new InvalidOperationException("Your profile is unavailable.");
                return result.Rows[0];
            }

            WorkflowUi.BindButton(view.EditMyProfileButton, async () => await WorkflowUi.RunAsync(page, async () =>
            {
                var row = CurrentStaff();
                using (var dialog = new WorkflowPanel("My staff profile", new ProfileEditor()))
                {
                    dialog.Note($"{row["Username"]} • {UserSession.RoleName}");
                    var name = dialog.Input("Full name", 100, Convert.ToString(row["FullName"]));
                    var phone = dialog.Input("Phone", 10, Convert.ToString(row["PhoneNumber"]));
                    var email = dialog.Input("Email", 100, Convert.ToString(row["EmailAddress"]));
                    dialog.Submit("Save", () => staff.UpdateStaffDetails(UserSession.StaffID, Convert.ToInt32(row["RoleID"]), name.Text.Trim(), phone.Text.Trim(), email.Text.Trim(), Convert.ToDecimal(row["Salary"])));
                    if (await dialog.OpenAsync(page))
                    {
                        details.ShowProfile(CurrentStaff());
                        WorkflowUi.Notify(page, "Profile updated.");
                    }
                }
            }), true);
            WorkflowUi.BindButton(view.ChangePasswordButton, async () => await WorkflowUi.RunAsync(page, async () =>
            {
                int staffId = UserSession.StaffID;
                using (var authentication = new AuthenticationService(WorkflowUi.ConnectionString))
                using (var dialog = new WorkflowPanel("Change staff password", new PasswordEditor()))
                {
                    if (!await authentication.GenerateAndSendOtpAsync(Convert.ToString(CurrentStaff()["EmailAddress"])))
                        throw new InvalidOperationException("A verification code could not be sent to your registered email.");
                    dialog.Note("Check your registered email for the verification code. The code is valid for five minutes.");
                    var code = dialog.Input("Verification code", 6);
                    var next = dialog.Input("New password", 128, password: true);
                    var confirm = dialog.Input("Confirm new password", 128, password: true);
                    dialog.Submit("Save Password", () =>
                    {
                        if (next.Text != confirm.Text || !ValidationHelper.IsValidPassword(next.Text))
                            throw new ArgumentException("Enter and confirm a password that meets the password requirements.");
                        if (staffId != UserSession.StaffID || !authentication.ValidateOtp(staffId, code.Text.Trim()))
                            throw new ArgumentException("The verification code is invalid or expired.");
                        authentication.ResetPassword(staffId, next.Text);
                    });
                    if (await dialog.OpenAsync(page))
                        WorkflowUi.Notify(page, "Password updated.");
                }
            }));
            if (technician)
                WorkflowUi.BindButton(view.SetMyAvailabilityButton, async () => await WorkflowUi.RunAsync(page, async () =>
                {
                    var row = CurrentStaff();
                    using (var dialog = new WorkflowPanel("My availability", new AvailabilityEditor()))
                    {
                        var choice = dialog.Choice("Availability", "Available", "Unavailable");
                        choice.SelectedIndex = Convert.ToBoolean(row["IsAvailable"]) ? 0 : 1;
                        dialog.Submit("Save", () => staff.ChangeAvailability(UserSession.StaffID, choice.SelectedIndex == 0));
                        if (await dialog.OpenAsync(page))
                        {
                            details.ShowProfile(CurrentStaff());
                            WorkflowUi.Notify(page, "Availability updated.");
                        }
                    }
                }));
            RefreshOnEntry(tabs, page, () => true, () => details.ShowProfile(CurrentStaff()));
        }

        internal static void AddInventory(WorkspaceTabs tabs)
        {
            var page = tabs.TabPages["StockWorkspacePage"];
            var view = page.Controls.OfType<StockDirectoryView>().Single();
            var grid = view.Records;
            DesktopTheme.Apply(grid);
            var search = view.Query;
            var inventory = new InventoryService(WorkflowUi.ConnectionString);
            void Refresh() => grid.DataSource = string.IsNullOrWhiteSpace(search.Text) ? inventory.GetAllInventoryItems() : inventory.SearchInventoryItems(search.Text.Trim());
            WorkflowUi.BindButton(view.SearchButton, () => WorkflowUi.Run(page, Refresh), true);
            RefreshOnEntry(tabs, page, () => true, Refresh);
        }

        internal static void AddCustomerDirectory(WorkspaceTabs tabs, Action reloadBookingCustomers)
        {
            var page = tabs.TabPages["CustomersWorkspacePage"];
            var view = page.Controls.OfType<CustomerDirectoryView>().Single();
            var grid = view.Records;
            DesktopTheme.Apply(grid);
            var search = view.Query;
            var customers = new CustomerService(WorkflowUi.ConnectionString);
            void Refresh()
            {
                var rows = string.IsNullOrWhiteSpace(search.Text) ? customers.GetAllCustomers() : customers.SearchCustomersByName(search.Text.Trim());
                string[] visible =
                {
                    "CustomerID",
                    "FullName",
                    "EmailAddress",
                    "PhoneNumber",
                    "Area",
                    "CustomerType",
                    "AccountStatus"
                };
                grid.DataSource = rows.DefaultView.ToTable(false, visible.Where(rows.Columns.Contains).ToArray());
            }

            int SelectedId() => Convert.ToInt32(WorkflowUi.Selected(grid)["CustomerID"]);
            WorkflowUi.BindButton(view.SearchButton, () => WorkflowUi.Run(page, Refresh), true);
            WorkflowUi.BindButton(view.EditCustomerButton, async () => await WorkflowUi.RunAsync(page, async () =>
            {
                int customerId = SelectedId();
                var records = customers.GetCustomerByID(customerId);
                if (records.Rows.Count != 1)
                    throw new InvalidOperationException("The selected customer is unavailable.");
                var row = records.Rows[0];
                using (var dialog = new WorkflowPanel("Edit customer contact details", new CustomerContactEditor()))
                {
                    dialog.Note($"{row["FullName"]} • {row["CustomerType"]}");
                    foreach (string key in new[]
                    {
                        "SouthAfricanIDNumber",
                        "CompanyRegistrationNumber",
                        "PassportNumber",
                        "PassportCountry"
                    }

                    )
                        if (records.Columns.Contains(key) && !string.IsNullOrWhiteSpace(Convert.ToString(row[key])))
                            dialog.Note(key + ": " + Convert.ToString(row[key]));
                    var phone = dialog.Input("Phone", 10, Convert.ToString(row["PhoneNumber"]));
                    var email = dialog.Input("Email", 100, Convert.ToString(row["EmailAddress"]));
                    var address = dialog.Input("Full address", 255, Convert.ToString(row["FullAddress"]));
                    var area = dialog.Input("Service area", 100, Convert.ToString(row["Area"]));
                    dialog.Submit("Save", () => customers.UpdateCustomer(customerId, phone.Text, email.Text, address.Text, area.Text));
                    if (await dialog.OpenAsync(page))
                    {
                        Refresh();
                        reloadBookingCustomers();
                    }
                }
            }));
            WorkflowUi.BindButton(view.ActivateDeactivateButton, async () => await WorkflowUi.RunAsync(page, async () =>
            {
                int id = SelectedId();
                var rows = customers.GetCustomerByID(id);
                if (rows.Rows.Count != 1)
                    throw new InvalidOperationException("The selected customer is unavailable.");
                bool activate = Convert.ToString(rows.Rows[0]["AccountStatus"]) != "Active";
                if (!await WorkflowUi.ConfirmAsync(page, (activate ? "Activate" : "Deactivate") + $" customer #{id}?") || !UserSession.Check())
                    return;
                if (activate)
                    customers.ActivateCustomer(id);
                else
                    customers.DeactivateCustomer(id);
                Refresh();
                reloadBookingCustomers();
            }));
            RefreshOnEntry(tabs, page, () => grid.DataSource == null, Refresh);
        }

        internal static void AddServices(WorkspaceTabs tabs, Action reloadBookingServices)
        {
            var page = tabs.TabPages["ServicesWorkspacePage"];
            var view = page.Controls.OfType<ServiceDirectoryView>().Single();
            var grid = view.Records;
            DesktopTheme.Apply(grid);
            var services = new JobTypeService(WorkflowUi.ConnectionString);
            void Refresh() => grid.DataSource = services.GetAllJobTypes();
            WorkflowUi.BindButton(view.RefreshButton, () => WorkflowUi.Run(page, Refresh));
            WorkflowUi.BindButton(view.EditPricesButton, async () => await WorkflowUi.RunAsync(page, async () =>
            {
                var row = WorkflowUi.Selected(grid);
                using (var dialog = new WorkflowPanel("Edit service pricing", new ServicePriceEditor()))
                {
                    var name = dialog.Input("Service name", 100, Convert.ToString(row["JobTypeName"]));
                    var labour = MoneyField(dialog, "Labour cost", Convert.ToDecimal(row["BaseLabourCost"]));
                    var transport = MoneyField(dialog, "Transport cost", Convert.ToDecimal(row["BaseTransportCost"]));
                    var tax = MoneyField(dialog, "VAT percentage", Convert.ToDecimal(row["TaxPercentage"]), 100);
                    dialog.Submit("Save", () => services.UpdateJobType(Convert.ToInt32(row["JobTypeID"]), name.Text, labour.Value, transport.Value, tax.Value));
                    if (await dialog.OpenAsync(page))
                    {
                        Refresh();
                        reloadBookingServices();
                    }
                }
            }), true);
            WorkflowUi.BindButton(view.ManageMaterialsButton, async () => await WorkflowUi.RunAsync(page, async () =>
            {
                var row = WorkflowUi.Selected(grid);
                await ManageMaterials(page, services, Convert.ToInt32(row["JobTypeID"]), Convert.ToString(row["JobTypeName"]));
            }));
            RefreshOnEntry(tabs, page, () => grid.DataSource == null, Refresh);
        }

        private static async Task ManageMaterials(Control owner, JobTypeService service, int serviceId, string name)
        {
            using (var panel = new WorkflowPanel("Service materials • " + name))
            {
                var window = new ServiceMaterialsView { Dock = DockStyle.Fill };
                panel.UseContent().Controls.Add(window);
                var grid = window.Records;
                DesktopTheme.Apply(grid);
                void Refresh() => grid.DataSource = service.GetMaterialsByJobType(serviceId);
                WorkflowUi.BindButton(window.AddMaterialButton, async () => await WorkflowUi.RunAsync(window, async () =>
                {
                    using (var dialog = new WorkflowPanel("Add service material", new MaterialEditor()))
                    {
                        var list = dialog.Field<ComboBox>("Inventory item", control =>
                        {
                            control.DropDownStyle = ComboBoxStyle.DropDownList;
                            control.DisplayMember = "ItemName";
                            control.ValueMember = "ItemID";
                            control.DataSource = service.GetAvailableInventoryItems();
                        });
                        var quantity = dialog.Field<NumericUpDown>("Required quantity", control =>
                        {
                            control.Minimum = 1;
                            control.Maximum = int.MaxValue;
                        });
                        dialog.Submit("Add", () =>
                        {
                            if (list.SelectedItem == null)
                                throw new ArgumentException("Choose an item.");
                            service.AddMaterialToJobType(serviceId, Convert.ToInt32(list.SelectedValue), (int)quantity.Value);
                        });
                        if (await dialog.OpenAsync(window))
                            Refresh();
                    }
                }));
                WorkflowUi.BindButton(window.UpdateQuantityButton, async () => await WorkflowUi.RunAsync(window, async () =>
                {
                    var row = WorkflowUi.Selected(grid);
                    using (var dialog = new WorkflowPanel("Update required quantity", new MaterialQuantityEditor()))
                    {
                        var quantity = dialog.Field<NumericUpDown>("Required quantity", control =>
                        {
                            control.Minimum = 1;
                            control.Maximum = int.MaxValue;
                            control.Value = Convert.ToInt32(row["QuantityRequired"]);
                        });
                        dialog.Submit("Save", () => service.UpdateMaterialQuantity(Convert.ToInt32(row["JobTypeMaterialID"]), (int)quantity.Value));
                        if (await dialog.OpenAsync(window))
                            Refresh();
                    }
                }));
                WorkflowUi.BindButton(window.RemoveMaterialButton, async () => await WorkflowUi.RunAsync(window, async () =>
                {
                    int id = Convert.ToInt32(WorkflowUi.Selected(grid)["JobTypeMaterialID"]);
                    if (await WorkflowUi.ConfirmAsync(window, "Remove the selected material from this service?") && UserSession.Check())
                    {
                        service.RemoveMaterialFromJobType(id);
                        Refresh();
                    }
                }));
                Refresh();
                await panel.OpenAsync(owner);
            }
        }

        private static NumericUpDown MoneyField(WorkflowPanel dialog, string label, decimal value, decimal maximum = 99999999.99m)
        {
            return dialog.Field<NumericUpDown>(label, control =>
            {
                control.DecimalPlaces = 2;
                control.Maximum = maximum;
                control.Value = value;
            });
        }

        private static void RefreshOnEntry(WorkspaceTabs tabs, TabPage page, Func<bool> uninitialized, Action refresh)
        {
            tabs.SelectedIndexChanged += (sender, args) =>
            {
                if (tabs.SelectedTab == page && uninitialized())
                    WorkflowUi.Run(page, refresh);
            };
        }
    }
}
