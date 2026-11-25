using System.Threading.Tasks;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Major_Project.Core;
using MajorProject.Business.Services;
using MajorProject.Business.Services.Payments;

namespace Major_Project.UI.Forms.Common
{
    internal static class StaffJobActions
    {
        internal static void Bind(Control surface, DataGridView jobsGrid, Action refresh, Button cancel, Button cash, Button invoicePdf, Button receiptPdf, Button viewInvoice, Button viewReceipt)
        {
            cancel.AccessibleDescription = "Request cancellation of the selected job for manager review.";
            cash.AccessibleDescription = "Record cash received for the selected completed job.";
            invoicePdf.AccessibleDescription = "Save the selected job's invoice PDF to a file.";
            receiptPdf.AccessibleDescription = "Save the receipt for the selected paid job.";
            viewInvoice.AccessibleDescription = "Open the selected job's invoice PDF in the viewer.";
            viewReceipt.AccessibleDescription = "Open the selected job's payment receipt.";
            var jobs = new JobService(WorkflowUi.ConnectionString);
            var payments = new PaymentService(WorkflowUi.ConnectionString);
            var invoices = new InvoiceService(WorkflowUi.ConnectionString);
            cancel.Click += async (sender, args) => await WorkflowUi.RunAsync(surface, async () =>
            {
                int jobId = JobId(jobsGrid);
                if (!UserSession.Check())
                    return;
                if (!UserSession.HasRole("Manager") && !UserSession.HasRole("Admin") && !UserSession.HasRole("Administrator"))
                    throw new InvalidOperationException("An administrator or manager must record the in-person request.");
                await JobChangeWorkspace.RequestCancellation(surface, jobId);
                refresh();
            });
            cash.Click += async (sender, args) => await WorkflowUi.RunAsync(surface, async () =>
            {
                try
                {
                    int jobId = JobId(jobsGrid);
                    var details = payments.GetStaffCashPaymentDetails(jobId);
                    if (details.Rows.Count == 0 || Convert.ToString(details.Rows[0]["JobStatus"]) != "Completed" || !IsUnpaid(Convert.ToString(details.Rows[0]["PaymentStatus"])))
                        throw new InvalidOperationException("Cash collection requires a completed job with an unpaid invoice.");
                    await CollectCash(surface, jobId, Convert.ToDecimal(details.Rows[0]["AmountDue"]), payments);
                }
                finally
                {
                    // Also refresh after cancel/rejection or an uncertain service result.
                    // Payment may have changed in another session while the form was open.
                    if (!surface.IsDisposed)
                        refresh();
                }
            });
            invoicePdf.Click += async (sender, args) => await WorkflowUi.RunAsync(surface, async () =>
            {
                int jobId = JobId(jobsGrid);
                await SavePdf(surface, invoices.GetInvoicePDFFromDatabase(jobId), $"INV-{jobId:D5}.pdf");
            });
            receiptPdf.Click += async (sender, args) => await WorkflowUi.RunAsync(surface, async () =>
            {
                int jobId = JobId(jobsGrid);
                await SaveReceipt(surface, invoices, payments, GetCustomerId(jobs, jobId), jobId);
            });
            viewInvoice.Click += async (sender, args) => await WorkflowUi.RunAsync(surface, () =>
            {
                int jobId = JobId(jobsGrid);
                return ViewPdf(surface, invoices.GetInvoicePDFFromDatabase(jobId), $"Invoice-{jobId:D5}.pdf");
            });
            viewReceipt.Click += async (sender, args) => await WorkflowUi.RunAsync(surface, () =>
            {
                int jobId = JobId(jobsGrid);
                return ViewPdf(surface, ReceiptPdf(invoices, payments, GetCustomerId(jobs, jobId), jobId), $"Receipt-{jobId:D5}.pdf");
            });
            var paymentStates = new System.Collections.Generic.Dictionary<int, string>();
            void UpdateActions()
            {
                var row = jobsGrid.CurrentRow?.DataBoundItem as DataRowView;
                string status = row != null && row.Row.Table.Columns.Contains("JobStatus") ? Convert.ToString(row["JobStatus"]) : "";
                string payment = row != null && row.Row.Table.Columns.Contains("PaymentStatus") ? Convert.ToString(row["PaymentStatus"]) : "";
                if (row != null && (status == "Completed" || status == "Archived") && payment.Length == 0)
                {
                    int id = Convert.ToInt32(row["JobID"]);
                    if (!paymentStates.TryGetValue(id, out payment))
                    {
                        payment = "";
                        try
                        {
                            var details = payments.GetStaffCashPaymentDetails(id);
                            if (details.Rows.Count == 1)
                                payment = Convert.ToString(details.Rows[0]["PaymentStatus"]);
                            paymentStates[id] = payment;
                        }
                        catch (Exception ex)
                        {
                            MajorProject.Business.Helpers.Logger.Error("Checking invoice action eligibility.", ex);
                        }
                    }
                }

                bool hasInvoice = row != null && row.Row.Table.Columns.Contains("HasInvoicePdf") && row["HasInvoicePdf"] != DBNull.Value && Convert.ToBoolean(row["HasInvoicePdf"]);
                ApplyActionState(status, payment, hasInvoice, cancel, cash, invoicePdf, receiptPdf, viewInvoice, viewReceipt);
            }

            jobsGrid.SelectionChanged += (sender, args) => UpdateActions();
            jobsGrid.DataBindingComplete += (sender, args) =>
            {
                paymentStates.Clear();
                UpdateActions();
            };
            UpdateActions();
        }

        private static bool IsUnpaid(string status) => string.Equals(status, "Pending", StringComparison.OrdinalIgnoreCase) || string.Equals(status, "Unpaid", StringComparison.OrdinalIgnoreCase);
        internal static void ApplyActionState(string status, string payment, bool hasInvoice, Button cancel, Button cash, Button invoicePdf, Button receiptPdf, Button viewInvoice, Button viewReceipt)
        {
            cancel.Enabled = status == "Pending" || status == "In Progress";
            cash.Enabled = status == "Completed" && IsUnpaid(payment);
            invoicePdf.Enabled = hasInvoice;
            receiptPdf.Enabled = (status == "Completed" || status == "Archived") && string.Equals(payment, "Paid", StringComparison.OrdinalIgnoreCase);
            viewInvoice.Enabled = invoicePdf.Enabled;
            viewReceipt.Enabled = receiptPdf.Enabled;
        }

        private static int JobId(DataGridView grid) => Convert.ToInt32(WorkflowUi.Selected(grid)["JobID"]);
        private static int GetCustomerId(JobService jobs, int jobId)
        {
            var detail = jobs.GetJobManagementDetails(jobId);
            if (detail.Rows.Count != 1)
                throw new InvalidOperationException("The job is unavailable.");
            return Convert.ToInt32(detail.Rows[0]["CustomerID"]);
        }

        private static Task<bool> CollectCash(Control owner, int jobId, decimal amountDue, PaymentService payments)
        {
            return CollectCashAsync(owner, jobId, amountDue, () => UserSession.Check(), received => payments.ProcessStaffCashPayment(jobId, received, UserSession.StaffID));
        }

        internal static async Task<bool> CollectCashAsync(Control owner, int jobId, decimal amountDue, Func<bool> sessionValid, Func<decimal, DataTable> recordPayment)
        {
            owner = WorkflowUi.Surface(owner);
            if (amountDue <= 0)
                throw new InvalidOperationException("The invoice amount is unavailable.");
            if (!sessionValid())
                return false;
            decimal total = 0;
            while (true)
            {
                using (var dialog = new WorkflowPanel($"Cash collection • Job #{jobId}"))
                {
                    var view = new CashCollectionView();
                    dialog.UseContent().Controls.Add(view);
                    view.AmountDue.Text = $"R {amountDue:N2}";
                    view.Received.Text = $"R {total:N2}";
                    view.JobNumber.Text = jobId.ToString();
                    var amount = view.Amount;
                    amount.MaxLength = 16;
                    if (total > 0)
                        view.description.Text = "Enter additional cash, or leave the amount blank to reject collection.";
                    decimal nextTotal = total;
                    dialog.BindSubmit(view.SubmitButton, () =>
                    {
                        if (!sessionValid())
                            throw new InvalidOperationException("Your session ended. Sign in again before collecting cash.");
                        if (total != 0 && string.IsNullOrWhiteSpace(amount.Text))
                            return;
                        if (!TryCash(amount.Text, out decimal received) || total + received > 99999999.99m)
                            throw new ArgumentException("Enter a positive cash amount with at most two decimal places.");
                        nextTotal += received;
                    }, authenticated: false);
                    if (!await dialog.OpenAsync(owner) || !sessionValid())
                        return false;
                    if (nextTotal > total && nextTotal < amountDue)
                    {
                        total = nextTotal;
                        continue;
                    }

                    var result = recordPayment(nextTotal);
                    if (result.Rows.Count != 1)
                        throw new InvalidOperationException("No result was returned. Refresh the invoice before trying again.");
                    DataRow row = result.Rows[0];
                    if (!Convert.ToBoolean(row["IsSuccessful"]))
                    {
                        WorkflowUi.ShowError(owner, Convert.ToString(row["Message"]));
                        return false;
                    }

                    WorkflowUi.Notify(owner, $"Cash payment recorded. Change due: R {Convert.ToDecimal(row["ChangeDue"]):N2}");
                    return true;
                }
            }
        }

        internal static bool TryCash(string text, out decimal value)
        {
            return (decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out value) || decimal.TryParse(text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value)) && value > 0 && value <= 99999999.99m && decimal.Round(value, 2) == value;
        }

        internal static async Task SaveReceipt(Control owner, InvoiceService invoices, PaymentService payments, int customerId, int jobId)
        {
            await SavePdf(owner, ReceiptPdf(invoices, payments, customerId, jobId), $"Receipt-{jobId:D5}.pdf");
        }

        private static byte[] ReceiptPdf(InvoiceService invoices, PaymentService payments, int customerId, int jobId)
        {
            var payment = payments.GetCustomerInvoicePaymentReceiptResult(jobId, customerId);
            var invoice = invoices.GetCustomerInvoiceDto(customerId, jobId);
            if (payment == null || invoice == null)
                throw new InvalidOperationException("A paid invoice and successful payment are required for a receipt.");
            return new PaymentReceiptPdfService().GeneratePaymentReceiptPdf(invoice, payment);
        }

        internal static Task ViewPdf(Control owner, byte[] pdf, string name)
        {
            using (var viewer = new PdfViewerForm(pdf, name))
                viewer.ShowDialog(owner.FindForm());
            return Task.CompletedTask;
        }

        internal static Task SavePdf(Control owner, byte[] pdf, string name)
        {
            if (pdf == null || pdf.Length < 5 || Encoding.ASCII.GetString(pdf, 0, 5) != "%PDF-")
                throw new InvalidOperationException("The PDF is unavailable.");
            return WorkflowUi.SaveAsync(owner, name, stream => stream.Write(pdf, 0, pdf.Length));
        }
    }
}
