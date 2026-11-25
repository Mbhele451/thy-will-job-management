using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Web.UI;
using MajorProject.Business.Services.Payments;
using MajorProject.Web.Helpers;

namespace MajorProject.Web.Secure.Staff
{
    public partial class CashPayment : CashPaymentBasePage
    {
        private string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
            }
        }

        private PaymentService PaymentService
        {
            get
            {
                return new PaymentService(ConnectionString);
            }
        }

        private decimal AmountDue
        {
            get
            {
                return ViewState["AmountDue"] == null ? 0m : (decimal)ViewState["AmountDue"];
            }

            set
            {
                ViewState["AmountDue"] = value;
            }
        }

        private decimal TotalReceived
        {
            get
            {
                return ViewState["TotalReceived"] == null ? 0m : (decimal)ViewState["TotalReceived"];
            }

            set
            {
                ViewState["TotalReceived"] = value;
            }
        }

        private int JobID
        {
            get
            {
                return ViewState["JobID"] == null ? 0 : (int)ViewState["JobID"];
            }

            set
            {
                ViewState["JobID"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadPaymentDetails();
            }
        }

        private void LoadPaymentDetails()
        {
            int jobID;
            if (!int.TryParse(Request.QueryString["jobID"], out jobID) || jobID <= 0)
            {
                ShowMessage("Invalid job number.", "error");
                pnlPayment.Visible = false;
                return;
            }

            try
            {
                DataTable details = PaymentService.GetStaffCashPaymentDetails(jobID);
                if (details == null || details.Rows.Count == 0)
                {
                    ShowMessage("No payment record was found for this job.", "error");
                    pnlPayment.Visible = false;
                    return;
                }

                DataRow row = details.Rows[0];
                string jobStatus = Convert.ToString(row["JobStatus"]);
                string paymentStatus = Convert.ToString(row["PaymentStatus"]);
                if (!string.Equals(jobStatus, "Completed", StringComparison.OrdinalIgnoreCase))
                {
                    ShowMessage("Cash can only be collected for completed jobs.", "error");
                    pnlPayment.Visible = false;
                    return;
                }

                if (!string.Equals(paymentStatus, "Pending", StringComparison.OrdinalIgnoreCase))
                {
                    ShowMessage("This invoice is not pending and cannot receive another payment.", "error");
                    pnlPayment.Visible = false;
                    return;
                }

                JobID = jobID;
                AmountDue = Convert.ToDecimal(row["AmountDue"]);
                lblJobID.Text = jobID.ToString();
                lblCustomerID.Text = Convert.ToString(row["CustomerID"]);
                lblAmountDue.Text = AmountDue.ToString("N2");
                pnlInitialCash.Visible = true;
                pnlAdditionalCash.Visible = false;
                pnlFinalResult.Visible = false;
            }
            catch (Exception ex)
            {
                ShowMessage(HandleUnexpectedError("Loading staff cash payment details", ex), "error");
                pnlPayment.Visible = false;
            }
        }

        protected void btnSubmitInitialCash_Click(object sender, EventArgs e)
        {
            decimal initialCash;
            if (!TryReadCash(txtInitialCash.Text, out initialCash))
            {
                ShowMessage("Enter a valid cash amount greater than zero, with no more than two decimal places.", "error");
                return;
            }

            TotalReceived = initialCash;
            if (TotalReceived >= AmountDue)
            {
                ProcessFinalCashPayment();
                return;
            }

            ShowAdditionalCashPanel();
        }

        protected void btnSubmitAdditionalCash_Click(object sender, EventArgs e)
        {
            if (TotalReceived <= 0 || AmountDue <= 0)
            {
                ShowMessage("The cash collection session is invalid. Please reload the page.", "error");
                return;
            }

            string additionalText = (txtAdditionalCash.Text ?? "").Trim();
            // Blank means staff is confirming that no more cash
            // will be received. Record one failed collection.
            if (string.IsNullOrWhiteSpace(additionalText))
            {
                ProcessFinalCashPayment();
                return;
            }

            decimal additionalCash;
            if (!TryReadCash(additionalText, out additionalCash))
            {
                ShowMessage("Enter a valid additional cash amount greater than zero, with no more than two decimal places.", "error");
                return;
            }

            TotalReceived += additionalCash;
            if (TotalReceived >= AmountDue)
            {
                ProcessFinalCashPayment();
                return;
            }

            // Still insufficient: keep the accumulated amount
            // in ViewState and allow another additional entry.
            ShowAdditionalCashPanel();
        }

        private void ShowAdditionalCashPanel()
        {
            decimal outstanding = AmountDue - TotalReceived;
            pnlInitialCash.Visible = false;
            pnlAdditionalCash.Visible = true;
            pnlFinalResult.Visible = false;
            lblTotalReceived.Text = TotalReceived.ToString("N2");
            lblOutstanding.Text = outstanding.ToString("N2");
            txtAdditionalCash.Text = "";
            ShowMessage("The cash received is not enough. Enter additional cash, or leave the additional cash field blank to reject the collection.", "info");
        }

        private void ProcessFinalCashPayment()
        {
            try
            {
                int staffID = WebSession.StaffID;
                if (staffID <= 0)
                {
                    ShowMessage("Your staff session has expired. Please log in again.", "error");
                    return;
                }

                DataTable result = PaymentService.ProcessStaffCashPayment(JobID, TotalReceived, staffID);
                if (result == null || result.Rows.Count == 0)
                {
                    ShowMessage("The payment could not be processed. Please try again.", "error");
                    return;
                }

                DataRow row = result.Rows[0];
                bool successful = Convert.ToBoolean(row["IsSuccessful"]);
                string message = Convert.ToString(row["Message"]);
                if (successful)
                {
                    decimal changeDue = Convert.ToDecimal(row["ChangeDue"]);
                    pnlInitialCash.Visible = false;
                    pnlAdditionalCash.Visible = false;
                    pnlFinalResult.Visible = true;
                    lblFinalReceived.Text = TotalReceived.ToString("N2");
                    lblChangeDue.Text = changeDue.ToString("N2");
                    txtInitialCash.Enabled = false;
                    txtAdditionalCash.Enabled = false;
                    btnSubmitInitialCash.Enabled = false;
                    btnSubmitAdditionalCash.Enabled = false;
                    ShowMessage("Cash payment successfully collected. The invoice is now paid.", "success");
                }
                else
                {
                    pnlInitialCash.Visible = false;
                    pnlAdditionalCash.Visible = false;
                    ShowMessage(string.IsNullOrWhiteSpace(message) ? "Cash collection was unsuccessful." : message, "error");
                }
            }
            catch (Exception ex)
            {
                ShowMessage(HandleUnexpectedError("Processing staff cash payment", ex), "error");
            }
        }

        private bool TryReadCash(string input, out decimal amount)
        {
            amount = 0m;
            if (string.IsNullOrWhiteSpace(input))
                return false;
            input = input.Trim();
            // Accept South African decimal format: 11962,30
            // Also accept international decimal format: 11962.30
            if (input.Contains(",") && input.Contains("."))
            {
                // Example: 11.962,30
                return decimal.TryParse(input, NumberStyles.Number, CultureInfo.GetCultureInfo("af-ZA"), out amount) && amount > 0;
            }

            if (input.Contains(","))
            {
                return decimal.TryParse(input, NumberStyles.Number, CultureInfo.GetCultureInfo("af-ZA"), out amount) && amount > 0;
            }

            return decimal.TryParse(input, NumberStyles.Number, CultureInfo.InvariantCulture, out amount) && amount > 0;
        }

        private void ShowMessage(string message, string messageType)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "message " + messageType;
            lblMessage.Visible = true;
        }
    }
}
