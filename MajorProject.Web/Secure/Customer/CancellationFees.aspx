<%@ Page Language="C#" Inherits="MajorProject.Web.Helpers.CustomerBasePage" MasterPageFile="~/MasterPages/Dashboard.Master" Title="Booking changes and fees" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="MajorProject.Web.Helpers" %>
<%@ Import Namespace="MajorProject.Business.Services" %>
<script runat="server">
    string Connection { get { return System.Configuration.ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString; } }
    JobChangeService Changes { get { return new JobChangeService(Connection); } }
    bool Cancel { get { return Request.QueryString["view"] == "cancel"; } }

    protected void Page_Load(object sender, EventArgs e)
    {
        string view = Request.QueryString["view"];
        historyPane.Visible = view == "history";
        feesPane.Visible = view == "fees";
        requestPane.Visible = !historyPane.Visible && !feesPane.Visible;
        (historyPane.Visible ? historyLink : feesPane.Visible ? feesLink : Cancel ? cancelLink : dateLink).Attributes["aria-current"] = "page";
        dateField.Visible = !Cancel;
        submit.Text = Cancel ? "Request cancellation" : "Request rescheduling";
        if (!IsPostBack)
            Run(BindView);
    }

    void BindView()
    {
        if (feesPane.Visible)
        {
            fees.DataSource = Changes.CustomerFees(WebSession.CustomerID);
            fees.DataBind();
        }
        else if (historyPane.Visible)
        {
            history.DataSource = Changes.CustomerRequests(WebSession.CustomerID);
            history.DataBind();
        }
        else
        {
            var jobs = new DataView(new JobService(Connection).GetJobsByCustomerID(WebSession.CustomerID));
            jobs.RowFilter = Cancel ? "JobStatus IN ('Pending','In Progress')" : "JobStatus='Pending'";
            var underReview = new System.Collections.Generic.HashSet<int>();
            foreach (var pending in Changes.CustomerRequests(WebSession.CustomerID).Select("RequestStatus='Pending'"))
                underReview.Add(Convert.ToInt32(pending["JobID"]));
            booking.Items.Clear();
            booking.Items.Add(new ListItem("Select a booking", ""));
            foreach (DataRowView row in jobs)
            {
                string id = Convert.ToString(row["JobID"]);
                string date = row["PromiseDate"] == DBNull.Value ? "Date unconfirmed" : Convert.ToDateTime(row["PromiseDate"]).ToString("dd MMM yyyy");
                if (!underReview.Contains(Convert.ToInt32(row["JobID"])) && booking.Items.FindByValue(id) == null)
                    booking.Items.Add(new ListItem("Job #" + id + " · " + date + " · " + row["JobStatus"], id));
            }
            var selected = booking.Items.FindByValue(Request.QueryString["job"] ?? "");
            if (selected != null)
                booking.SelectedValue = selected.Value;
            submit.Enabled = booking.Items.Count > 1;
            if (!submit.Enabled)
                status.Text = "No bookings are eligible for this change. Check Your requests for changes already under review.";
        }
    }

    void Run(Action action)
    {
        status.CssClass = "change-feedback";
        try { action(); }
        catch (Exception ex)
        {
            MajorProject.Business.Helpers.Logger.Error("Customer booking change", ex);
            var sql = ex as System.Data.SqlClient.SqlException;
            status.CssClass += " error";
            status.Text = Server.HtmlEncode(ex is ArgumentException || ex is InvalidOperationException || (sql != null && sql.Number >= 50000) ? ex.Message : "We could not confirm this action. Refresh your requests or fees before trying again.");
        }
        if (IsPostBack && status.Text.Length > 0)
            status.Focus();
    }

    protected void SendRequest(object sender, EventArgs e)
    {
        Run(() =>
        {
            int job;
            if (!int.TryParse(booking.SelectedValue, out job))
                throw new ArgumentException("Select a booking first.");
            if (Cancel)
                Changes.RequestCustomerCancellation(job, WebSession.CustomerID, reason.Text);
            else
            {
                DateTime date;
                if (!DateTime.TryParseExact(newDate.Text, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out date))
                    throw new ArgumentException("Choose the new date you want to request.");
                Changes.RequestCustomerReschedule(job, WebSession.CustomerID, date, reason.Text);
            }
            status.Text = "Your request for job #" + job + " was submitted. You can follow the decision under Your requests.";
            reason.Text = "";
            newDate.Text = "";
            booking.Items.Remove(booking.Items.FindByValue(job.ToString()));
            booking.SelectedIndex = 0;
            submit.Enabled = booking.Items.Count > 1;
        });
    }

    protected void Pay(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "PayDemo") return;
        Run(() =>
        {
            Changes.PayDemoFee(WebSession.CustomerID, Convert.ToInt32(e.CommandArgument));
            status.Text = "Fee invoice paid in the demo. No money was transferred.";
            BindView();
        });
    }
</script>
<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/css/jobchanges.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
<div class="change-page">
    <h1>Booking changes &amp; fees</h1>
    <nav class="change-nav" aria-label="Booking changes">
        <asp:HyperLink ID="dateLink" runat="server" NavigateUrl="?view=reschedule" Text="Reschedule" />
        <asp:HyperLink ID="cancelLink" runat="server" NavigateUrl="?view=cancel" Text="Cancel a booking" />
        <asp:HyperLink ID="historyLink" runat="server" NavigateUrl="?view=history" Text="Your requests" />
        <asp:HyperLink ID="feesLink" runat="server" NavigateUrl="?view=fees" Text="Approved fees" />
    </nav>
    <asp:Label ID="status" runat="server" CssClass="change-feedback" role="status" aria-live="polite" tabindex="-1" EnableViewState="false" />
    <asp:Panel ID="requestPane" CssClass="change-card" runat="server">
        <h2><%= Cancel ? "Request cancellation" : "Request a new date" %></h2>
        <p><%= Cancel ? "A manager reviews your request. Cancellation on the promise date, the day before, or after work starts carries a fee of 5% of the original job total. Materials already used are invoiced separately on the same fee invoice." : "An administrator or manager reviews your request. A change requested on the promise date or the day before carries a fee of 5% of the transport cost on approval." %></p>
        <div class="change-fields">
            <div class="change-field"><asp:Label AssociatedControlID="booking" runat="server" Text="Booking" /><asp:DropDownList ID="booking" runat="server" /></div>
            <div class="change-field" id="dateField" runat="server"><asp:Label AssociatedControlID="newDate" runat="server" Text="Requested date" /><asp:TextBox ID="newDate" runat="server" TextMode="Date" /></div>
            <div class="change-field full"><asp:Label AssociatedControlID="reason" runat="server" Text="Reason (10–1,000 characters)" /><asp:TextBox ID="reason" runat="server" TextMode="MultiLine" MaxLength="1000" rows="3" /></div>
        </div>
        <div class="change-actions"><asp:Button ID="submit" runat="server" CssClass="change-action" OnClick="SendRequest" /></div>
    </asp:Panel>
    <asp:Panel ID="historyPane" CssClass="change-card" runat="server">
        <h2>Your requests</h2>
        <div class="change-table"><asp:GridView ID="history" runat="server" AutoGenerateColumns="false" EmptyDataText="You have no booking change requests."><Columns>
            <asp:BoundField DataField="JobID" HeaderText="Job" /><asp:BoundField DataField="RequestType" HeaderText="Change" />
            <asp:TemplateField HeaderText="Status"><ItemTemplate><%#: Convert.ToString(Eval("RequestStatus")) == "Pending"
                ? (Convert.ToBoolean(Eval("AwaitingMaterials")) ? "Awaiting technician materials report"
                    : Convert.ToString(Eval("RequestType")) == "Cancellation" ? "Awaiting manager review" : "Awaiting rescheduling decision")
                : Convert.ToString(Eval("RequestStatus")) %></ItemTemplate></asp:TemplateField>
            <asp:BoundField DataField="RequestedDate" HeaderText="Requested date" DataFormatString="{0:dd MMM yyyy}" NullDisplayText="—" />
            <asp:BoundField DataField="FeeIfApproved" HeaderText="Charge if approved (R)" DataFormatString="{0:N2}" />
            <asp:BoundField DataField="DecisionNote" HeaderText="Decision note" />
        </Columns></asp:GridView></div>
    </asp:Panel>
    <asp:Panel ID="feesPane" CssClass="change-card" runat="server">
        <h2>Approved fees</h2>
        <p>Each invoice below explains the approved charge for that booking. Late rescheduling is based on transport cost. A late or in-progress cancellation is based on the original job total and may also include materials used.</p>
        <div class="change-table"><asp:GridView ID="fees" runat="server" AutoGenerateColumns="false" OnRowCommand="Pay" EmptyDataText="You have no approved fee invoices."><Columns>
            <asp:BoundField DataField="JobID" HeaderText="Job" /><asp:BoundField DataField="FeeReason" HeaderText="Why you were charged" />
            <asp:BoundField DataField="MaterialCharge" HeaderText="Materials included (R)" DataFormatString="{0:N2}" />
            <asp:BoundField DataField="Fee" HeaderText="Invoice total (R)" DataFormatString="{0:N2}" />
            <asp:TemplateField HeaderText="Payment status"><ItemTemplate><%#: (string)Eval("FeePaymentStatus") == "DemoPaid" ? "Paid (demo)" : "Unpaid" %></ItemTemplate></asp:TemplateField>
            <asp:TemplateField HeaderText="Invoice"><ItemTemplate><asp:HyperLink runat="server" CssClass="change-link" Text="View PDF" NavigateUrl='<%# "InvoicePdf.aspx?fee=" + Eval("RequestID") %>' /></ItemTemplate></asp:TemplateField>
            <asp:TemplateField HeaderText="Payment"><ItemTemplate><span id="fee-<%# Eval("RequestID") %>"><asp:LinkButton runat="server" CssClass="change-link" Text="Pay invoice (demo)" CommandName="PayDemo" CommandArgument='<%# Eval("RequestID") %>' Visible='<%# Convert.ToString(Eval("FeePaymentStatus")) == "Unpaid" %>' OnClientClick="return confirm('Record a simulated payment for this invoice?');" /></span></ItemTemplate></asp:TemplateField>
        </Columns></asp:GridView></div>
        <p>Payments on this page are simulated. No money is transferred.</p>
    </asp:Panel>
</div>
</asp:Content>
