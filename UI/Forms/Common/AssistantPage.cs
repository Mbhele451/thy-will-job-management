using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Major_Project.Core;
using MajorProject.Business.Models;
using MajorProject.Business.Services;

namespace Major_Project.UI.Forms.Common
{
    internal static class AssistantPage
    {
        internal static void Add(WorkspaceTabs tabs)
        {
            var page = tabs.TabPages["AssistantWorkspacePage"];
            var view = page.Controls.OfType<AssistantView>().Single();
            var transcript = view.Transcript;
            var question = view.Query;
            DesktopTheme.Apply(transcript);
            transcript.Text = "Ask about your jobs or profile, or type “help with” followed by a workspace tab name." + Environment.NewLine + Environment.NewLine;
            TabPage destination = null;
            Button open = null;
            var assistant = new ChatbotService(WorkflowUi.ConnectionString);
            Button ask = null;
            ask = WorkflowUi.BindButton(view.AskButton, async () => await WorkflowUi.RunAsync(page, async () =>
            {
                string message = question.Text.Trim();
                if (message.Length == 0)
                {
                    question.Focus();
                    WorkflowUi.Notify(page, "Enter a question first.");
                    return;
                }

                var topic = tabs.TabPages.Cast<TabPage>().FirstOrDefault(tab => PageHelp(tab.Text) != null && (message.IndexOf(tab.Text, StringComparison.OrdinalIgnoreCase) >= 0 || (tab.Text == "Inventory" && Regex.IsMatch(message, @"\b(stock|restock|reorder)\b", RegexOptions.IgnoreCase)) || (tab.Text == "Staff Management" && Regex.IsMatch(message, @"\b(staff|availability|employment)\b", RegexOptions.IgnoreCase)) || (tab.Text == "Job Management" && Regex.IsMatch(message, @"\bjobs?\b", RegexOptions.IgnoreCase))));
                if (topic != null && Regex.IsMatch(message, @"\b(help|how|where)\b", RegexOptions.IgnoreCase))
                {
                    transcript.AppendText("You: " + message + Environment.NewLine + PageHelp(topic.Text) + Environment.NewLine + Environment.NewLine);
                    destination = topic;
                    open.Text = "Open " + topic.Text;
                    open.Visible = true;
                    question.Clear();
                    return;
                }

                var staffId = UserSession.StaffID;
                var role = UserSession.RoleName;
                ask.Enabled = question.Enabled = false;
                ask.Text = "Looking up…";
                destination = null;
                open.Hide();
                ChatResponse response;
                try
                {
                    response = await System.Threading.Tasks.Task.Run(() =>
                    {
                        if (role.Equals("Manager", StringComparison.OrdinalIgnoreCase))
                            return assistant.ProcessManagerMessage(message, staffId);
                        if (role.Equals("Technician", StringComparison.OrdinalIgnoreCase))
                            return assistant.ProcessTechnicianMessage(message, staffId);
                        return assistant.ProcessAdministratorMessage(message, staffId);
                    });
                    if (page.IsDisposed || !UserSession.Check(false))
                        return;
                }
                finally
                {
                    if (!page.IsDisposed)
                    {
                        ask.Enabled = question.Enabled = true;
                        ask.Text = "Ask";
                    }
                }

                string plain = Regex.Replace(response.Message ?? "", @"<br\s*/?>", Environment.NewLine, RegexOptions.IgnoreCase);
                plain = WebUtility.HtmlDecode(Regex.Replace(plain, "<[^>]+>", ""));
                transcript.AppendText("You: " + message + Environment.NewLine + Environment.NewLine + "THY WILL: " + plain + Environment.NewLine + Environment.NewLine);
                question.Clear();
                destination = ResolveTab(tabs, response.ActionUrl);
                open.Visible = destination != null;
                open.Text = destination == null ? "Open" : "Open " + destination.Text;
            }), true);
            open = WorkflowUi.BindButton(view.OpenButton, () =>
            {
                if (destination != null && UserSession.Check())
                    tabs.SelectedTab = destination;
            });
            open.Hide();
            question.KeyDown += (sender, args) =>
            {
                if (args.KeyCode != Keys.Enter)
                    return;
                args.SuppressKeyPress = true;
                ask.PerformClick();
            };
        }

        internal static string PageHelp(string page)
        {
            switch (page)
            {
                case "Inventory":
                    return UserSession.HasRole("Manager") ? "Select a row in Stock and details. Use Update stock to restock or record waste. The reorder alert opens items at or below their reorder level." : "Search for the item name and check its available quantity and reorder level.";
                case "Staff Management":
                    return "Select a person in the directory, then open Edit / add staff to change their details, employment status, availability or password.";
                case "Audit":
                    return "Select a UTC date range and refresh to review recorded actions. Export CSV uses the same dates.";
                case "Accounts":
                    return "Search by account details, select a row and choose the account action. Check the account type before changing credentials or blocking access.";
                case "Sessions":
                    return "Refresh the session list and select the session you need to inspect. Check its owner and expiry before ending it.";
                case "Settings":
                    return "Select a setting, choose Edit Setting and review the current value before saving.";
                case "Jobs":
                case "Job Management":
                case "My Jobs":
                    return UserSession.HasRole("Technician") ? "Select an assigned job to see its details. Start a pending job, then mark it complete when the work is finished." : "Use the status filter or search to find a job. Select its row to see available actions.";
                case "My Profile":
                    return "Open My Profile to see your details. Choose Edit My Profile for contact changes or Change Password for sign-in credentials.";
                default:
                    return null;
            }
        }

        private static TabPage ResolveTab(WorkspaceTabs tabs, string url)
        {
            string page = (url ?? "").Split('?')[0].Split('/').Last();
            string label;
            switch (page)
            {
                case "WorkOrders.aspx":
                    label = "Add New Job";
                    break;
                case "BookJob.aspx":
                    label = "Book a Job";
                    break;
                case "MyJobs.aspx":
                    label = "My Jobs";
                    break;
                case "Jobs.aspx":
                    label = "Job Management";
                    break;
                case "Invoices.aspx":
                    label = "Invoices";
                    break;
                case "Customers.aspx":
                    label = "Customers";
                    break;
                case "Inventory.aspx":
                    label = "Inventory";
                    break;
                case "Profile.aspx":
                    return tabs.TabPages.Cast<TabPage>().FirstOrDefault(tab => tab.Text == "My Profile" || tab.Text == "Profile");
                case "StaffManagement.aspx":
                    label = "Staff Management";
                    break;
                case "Services.aspx":
                    label = "Services";
                    break;
                default:
                    return null;
            }

            return tabs.TabPages.Cast<TabPage>().FirstOrDefault(tab => tab.Text == label);
        }
    }
}
