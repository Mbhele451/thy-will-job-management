<%@ Page Language="C#" Inherits="MajorProject.Web.Helpers.BasePage" MasterPageFile="~/MasterPages/Dashboard.Master" Title="Job changes and allocations" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="MajorProject.Web.Helpers" %>
<%@ Import Namespace="MajorProject.Business.Services" %>
<script runat="server">
    string CS { get { return System.Configuration.ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString; } }
    bool Manager { get { return WebSession.HasRole("Manager"); } }
    bool Reviewer { get { return Manager || WebSession.HasRole("Administrator"); } }
    JobChangeService Changes { get { return new JobChangeService(CS); } }
    int Actor { get { return WebSession.StaffID; } }
    string Screen
    {
        get
        {
            var view = Request.QueryString["view"];
            if (!Reviewer) return view == "materials" ? "materials" : "allocations";
            if (view == "handover" && Manager) return "handover";
            if (view == "cancel" || Request.QueryString["job"] != null) return "cancel";
            return "requests";
        }
    }

    protected override void OnInit(EventArgs e)
    {
        AccessControl.RequireLogin();
        if (!Reviewer && !WebSession.HasRole("Technician"))
        {
            Response.Redirect(ResolveUrl("~/Account/AccessDenied.aspx"), true);
            return;
        }
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        requestsLink.Visible = cancelLink.Visible = Reviewer;
        handoverLink.Visible = Manager;
        allocationsLink.Visible = materialsLink.Visible = !Reviewer;
        requestsPane.Visible = Screen == "requests";
        cancelPane.Visible = Screen == "cancel";
        handoverPane.Visible = Screen == "handover";
        materialsPane.Visible = Screen == "materials";
        allocationsPane.Visible = Screen == "allocations";
        foreach (var link in new HyperLink[] {requestsLink, cancelLink, handoverLink, allocationsLink, materialsLink})
            if (link.NavigateUrl == "?view=" + Screen)
                link.Attributes["aria-current"] = "page";
        if (!IsPostBack)
            Safe(BindScreen);
    }

    void Safe(Action action)
    {
        try { action(); }
        catch (Exception ex)
        {
            MajorProject.Business.Helpers.Logger.Error("Job change workflow", ex);
            var sql = ex as System.Data.SqlClient.SqlException;
            message.CssClass = "change-feedback error";
            message.Text = Server.HtmlEncode(ex is ArgumentException || ex is InvalidOperationException || ex is UnauthorizedAccessException || (sql != null && sql.Number >= 50000) ? ex.Message : "We could not confirm this action. Refresh these records before trying again.");
        }
    }

    void BindScreen()
    {
        review.Visible = false;
        materialsEditor.Visible = false;
        ViewState.Remove("ReviewID");
        ViewState.Remove("ScheduleJob");
        ViewState.Remove("ReportID");
        decisionNote.Text = "";
        if (Screen == "requests")
        {
            requests.SelectedIndex = -1;
            requests.DataSource = Changes.Requests(Actor, true);
            requests.DataBind();
        }
        else if (Screen == "handover")
        {
            queue.SelectedIndex = -1;
            queue.DataSource = Changes.ReassignmentQueue(Actor, allOpen.Checked);
            queue.DataBind();
        }
        else if (Screen == "cancel")
        {
            var jobs = new DataView(new JobService(CS).GetAllJobs());
            jobs.RowFilter = "JobStatus IN ('Pending','In Progress')";
            job.Items.Clear();
            job.Items.Add(new ListItem("Select an open job", ""));
            foreach (DataRowView row in jobs)
            {
                string id = Convert.ToString(row["JobID"]);
                if (job.Items.FindByValue(id) == null)
                    job.Items.Add(new ListItem("Job #" + id + " · " + row["JobStatus"], id));
            }
            var selected = job.Items.FindByValue(Request.QueryString["job"] ?? "");
            if (selected != null) job.SelectedValue = selected.Value;
        }
        else if (Screen == "materials")
        {
            reports.SelectedIndex = -1;
            reports.DataSource = Changes.TechnicianMaterialReports(Actor);
            reports.DataBind();
        }
        else
        {
            notices.DataSource = Changes.Notifications(Actor);
            notices.DataBind();
        }
    }

    protected void Refresh(object sender, EventArgs e)
    {
        Safe(() => { BindScreen(); message.Text = "Records refreshed."; });
    }

    protected void SubmitCancellation(object sender, EventArgs e)
    {
        Safe(() =>
        {
            int id;
            if (!Reviewer || !int.TryParse(job.SelectedValue, out id))
                throw new ArgumentException("Select an open job.");
            Changes.RequestStaffCancellation(id, Actor, reason.Text);
            reason.Text = "";
            job.SelectedIndex = 0;
            message.Text = "Cancellation recorded for job #" + id + ". A manager will review the request.";
        });
    }

    void LoadTechnicians()
    {
        technician.Items.Clear();
        DateTime date;
        if (ViewState["ScheduleJob"] != null && DateTime.TryParse(approvedDate.Text, out date))
        {
            technician.DataSource = new StaffService(CS).GetAvailableTechniciansForJob(date, (int)ViewState["ScheduleJob"]);
            technician.DataTextField = "FullName";
            technician.DataValueField = "StaffID";
            technician.DataBind();
        }
        technician.Items.Insert(0, new ListItem("Select a technician", ""));
    }

    void SetSchedule(int id, DateTime date)
    {
        ViewState["ScheduleJob"] = id;
        schedule.Visible = true;
        approvedDate.Text = (date.Date < DateTime.Today ? DateTime.Today : date.Date).ToString("yyyy-MM-dd");
        LoadTechnicians();
    }

    protected void DateChanged(object sender, EventArgs e) { Safe(LoadTechnicians); }

    protected void SelectHandover(object sender, EventArgs e)
    {
        Safe(() =>
        {
            if (!Manager || Screen != "handover") throw new UnauthorizedAccessException();
            var keys = queue.SelectedDataKey.Values;
            review.Visible = true;
            reviewText.Text = "Handover for job #" + Convert.ToInt32(keys["JobID"]);
            notesLabel.Text = "Handover reason";
            decisionNote.Text = "";
            summary.Visible = decisionGroup.Visible = saveDecision.Visible = false;
            reassignButton.Visible = true;
            SetSchedule(Convert.ToInt32(keys["JobID"]), keys["PromiseDate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(keys["PromiseDate"]));
            decisionNote.Focus();
        });
    }

    protected void SelectRequest(object sender, EventArgs e)
    {
        Safe(() =>
        {
            if (!Reviewer || Screen != "requests") throw new UnauthorizedAccessException();
            int id = Convert.ToInt32(requests.SelectedDataKey.Value);
            DataRow[] rows = Changes.Requests(Actor, true).Select("RequestID=" + id);
            if (rows.Length != 1) throw new InvalidOperationException("The request has changed. Refresh the list.");
            var row = rows[0];
            bool cancel = Convert.ToString(row["RequestType"]) == "Cancellation";
            bool pending = Convert.ToString(row["RequestStatus"]) == "Pending";
            bool awaiting = cancel && Convert.ToBoolean(row["AwaitingMaterials"]);
            decision.Items.FindByValue("Approve").Enabled = !awaiting;
            decision.SelectedValue = "Reject";
            ViewState["ReviewID"] = id;
            ViewState.Remove("ScheduleJob");
            reassignButton.Visible = schedule.Visible = false;
            review.Visible = true;
            summary.Visible = cancel && Manager;
            reviewMaterials.DataSource = summary.Visible ? Changes.RequestMaterials(Actor, id) : null;
            reviewMaterials.DataBind();
            bool authorised = !cancel || Manager;
            decisionGroup.Visible = saveDecision.Visible = notesField.Visible = pending && authorised;
            notesLabel.Text = "Decision note";
            decisionNote.Text = "";
            reviewText.Text = Server.HtmlEncode("Job #" + row["JobID"] + " · " + row["RequestType"] + " · " + row["RequestStatus"] + ". " + row["Reason"] +
                " Base fee if approved: R " + Convert.ToDecimal(row[cancel ? "CancellationFee" : "ReschedulingFee"]).ToString("N2") + "." +
                (awaiting ? " Approval is available after the technician submits materials used." :
                    cancel && (pending || Convert.ToString(row["RequestStatus"]) == "Approved")
                        ? " Materials used: R " + Convert.ToDecimal(row["MaterialCharge"]).ToString("N2") +
                          ". Cancellation invoice total: R " + (Convert.ToDecimal(row["CancellationFee"]) + Convert.ToDecimal(row["MaterialCharge"])).ToString("N2") + "." : "") +
                (pending && !authorised ? " A manager must review this cancellation." : "") +
                (pending ? "" : " Decision: " + row["DecisionNote"]));
            if (!cancel && pending) SetSchedule(Convert.ToInt32(row["JobID"]), Convert.ToDateTime(row["RequestedDate"]));
            review.Attributes["tabindex"] = "-1";
            review.Focus();
        });
    }

    protected void Decide(object sender, EventArgs e)
    {
        Safe(() =>
        {
            if (!Reviewer || Screen != "requests" || ViewState["ReviewID"] == null) throw new ArgumentException("Select a pending request.");
            bool approve = decision.SelectedValue == "Approve";
            int staff;
            int.TryParse(technician.SelectedValue, out staff);
            DateTime date;
            if (!DateTime.TryParse(approvedDate.Text, out date) && approve && ViewState["ScheduleJob"] != null)
                throw new ArgumentException("Choose the new promise date.");
            Changes.Decide((int)ViewState["ReviewID"], Actor, approve, decisionNote.Text, staff, date);
            BindScreen();
            message.Text = approve ? "Request approved." : "Request rejected.";
        });
    }

    protected void Reassign(object sender, EventArgs e)
    {
        Safe(() =>
        {
            if (!Manager || Screen != "handover" || queue.SelectedDataKey == null) throw new ArgumentException("Select a job for handover.");
            int staff;
            DateTime date;
            if (!DateTime.TryParse(approvedDate.Text, out date) || !int.TryParse(technician.SelectedValue, out staff))
                throw new ArgumentException("Choose a promise date and replacement technician.");
            var keys = queue.SelectedDataKey.Values;
            Changes.Reassign(Convert.ToInt32(keys["JobID"]), Actor, staff, date, decisionNote.Text, keys["PreviousStaffID"] == DBNull.Value ? 0 : Convert.ToInt32(keys["PreviousStaffID"]));
            BindScreen();
            message.Text = "Job reassigned and the replacement technician notified.";
        });
    }

    protected void SelectReport(object sender, EventArgs e)
    {
        Safe(() =>
        {
            if (Reviewer || Screen != "materials") throw new UnauthorizedAccessException();
            var keys = reports.SelectedDataKey.Values;
            ViewState["ReportID"] = Convert.ToInt32(keys["RequestID"]);
            int id = Convert.ToInt32(keys["JobID"]);
            materials.DataSource = Changes.Materials(id, Actor, false);
            materials.DataBind();
            materialsEditor.Visible = true;
            materialTitle.Text = "Materials used on job #" + id;
            materialsEditor.Attributes["tabindex"] = "-1";
            materialsEditor.Focus();
        });
    }

    protected void SubmitReport(object sender, EventArgs e)
    {
        Safe(() =>
        {
            if (Reviewer || Screen != "materials" || ViewState["ReportID"] == null) throw new ArgumentException("Select a material report first.");
            var used = new DataTable();
            used.Columns.Add("ItemID", typeof(int));
            used.Columns.Add("UsedQuantity", typeof(int));
            foreach (GridViewRow row in materials.Rows)
            {
                int amount;
                if (!int.TryParse(((TextBox)row.FindControl("used")).Text, out amount) || amount < 0)
                    throw new ArgumentException("Enter the quantity used for each item, including zero.");
                used.Rows.Add(Convert.ToInt32(materials.DataKeys[row.RowIndex].Value), amount);
            }
            Changes.SubmitMaterials((int)ViewState["ReportID"], Actor, used);
            BindScreen();
            message.Text = "Materials report submitted for manager review.";
        });
    }

    protected void NoticeCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName != "ReadNotice") return;
        Safe(() =>
        {
            if (Reviewer || Screen != "allocations") throw new UnauthorizedAccessException();
            Changes.ReadNotification(Actor, Convert.ToInt64(e.CommandArgument));
            BindScreen();
            message.Text = "Notification marked as read.";
        });
    }
</script>
<asp:Content ContentPlaceHolderID="HeadContent" runat="server">
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/css/jobchanges.css") %>" />
</asp:Content>
<asp:Content ContentPlaceHolderID="MainContent" runat="server">
    <div class="change-page">
    <h1><%= Reviewer ? "Job changes" : "Allocations and materials" %></h1>
    <nav class="change-nav" aria-label="Job workspace">
        <asp:HyperLink ID="requestsLink" runat="server" NavigateUrl="?view=requests" Text="Request reviews" />
        <asp:HyperLink ID="cancelLink" runat="server" NavigateUrl="?view=cancel" Text="In-person cancellation" />
        <asp:HyperLink ID="handoverLink" runat="server" NavigateUrl="?view=handover" Text="Reassignment queue" />
        <asp:HyperLink ID="allocationsLink" runat="server" NavigateUrl="?view=allocations" Text="Job notifications" />
        <asp:HyperLink ID="materialsLink" runat="server" NavigateUrl="?view=materials" Text="Materials to report" />
    </nav>
    <asp:Label ID="message" runat="server" CssClass="change-feedback" role="status" aria-live="polite" EnableViewState="false" />
    <asp:Panel runat="server" ID="requestsPane" CssClass="change-card">
        <h2>Request reviews</h2>
        <div class="change-table"><asp:GridView ID="requests" runat="server" AutoGenerateColumns="false" DataKeyNames="RequestID" OnSelectedIndexChanged="SelectRequest" EmptyDataText="No requests to display."><Columns>
            <asp:CommandField ShowSelectButton="true" SelectText="Open" /><asp:BoundField DataField="JobID" HeaderText="Job" />
            <asp:BoundField DataField="RequestType" HeaderText="Change" /><asp:BoundField DataField="RequestedByName" HeaderText="Requested by" />
            <asp:BoundField DataField="RequestedDate" HeaderText="Requested date" DataFormatString="{0:dd MMM yyyy}" NullDisplayText="—" />
            <asp:BoundField DataField="RequestStatus" HeaderText="Decision status" />
            <asp:TemplateField HeaderText="Materials"><ItemTemplate><%# Convert.ToBoolean(Eval("AwaitingMaterials")) ? "Awaiting technician" : "—" %></ItemTemplate></asp:TemplateField>
        </Columns></asp:GridView></div>
    </asp:Panel>
    <asp:Panel runat="server" ID="cancelPane" CssClass="change-card">
        <h2>Record a customer's cancellation request</h2><p>A manager reviews the request. For a job already in progress, the assigned technician reports materials used before approval.</p>
        <div class="change-fields">
            <div class="change-field"><asp:Label AssociatedControlID="job" runat="server" Text="Open job" /><asp:DropDownList ID="job" runat="server" /></div>
            <div class="change-field full"><asp:Label AssociatedControlID="reason" runat="server" Text="Customer's reason (10–1,000 characters)" /><asp:TextBox ID="reason" runat="server" TextMode="MultiLine" MaxLength="1000" /></div>
        </div>
        <div class="change-actions"><asp:Button runat="server" CssClass="change-action" Text="Submit cancellation request" OnClick="SubmitCancellation" /></div>
    </asp:Panel>
    <asp:Panel ID="handoverPane" runat="server" CssClass="change-card">
        <h2>Reassignment queue</h2><asp:CheckBox ID="allOpen" runat="server" Text=" Include all open jobs" AutoPostBack="true" OnCheckedChanged="Refresh" />
        <div class="change-table"><asp:GridView ID="queue" runat="server" AutoGenerateColumns="false" DataKeyNames="JobID,PreviousStaffID,PromiseDate" OnSelectedIndexChanged="SelectHandover" EmptyDataText="No jobs need reassignment."><Columns>
            <asp:CommandField ShowSelectButton="true" SelectText="Open" /><asp:BoundField DataField="JobID" HeaderText="Job" />
            <asp:BoundField DataField="PreviousTechnician" HeaderText="Technician" NullDisplayText="Unassigned" /><asp:BoundField DataField="JobStatus" HeaderText="Job status" />
            <asp:BoundField DataField="PromiseDate" HeaderText="Promise date" DataFormatString="{0:dd MMM yyyy}" /><asp:BoundField DataField="Reason" HeaderText="Assignment" />
        </Columns></asp:GridView></div>
    </asp:Panel>
    <asp:Panel ID="review" runat="server" Visible="false" CssClass="change-card">
        <h2>Selected job</h2><p><asp:Label ID="reviewText" runat="server" /></p>
        <asp:Panel ID="summary" runat="server" CssClass="change-table">
            <asp:GridView ID="reviewMaterials" runat="server" AutoGenerateColumns="false" EmptyDataText="No allocated materials."><Columns>
                <asp:BoundField DataField="ItemName" HeaderText="Material" /><asp:BoundField DataField="AllocatedQuantity" HeaderText="Allocated" /><asp:BoundField DataField="UsedQuantity" HeaderText="Used" /><asp:BoundField DataField="ReturnQuantity" HeaderText="To return" />
            </Columns></asp:GridView>
        </asp:Panel>
        <div id="schedule" runat="server" class="change-fields">
            <div class="change-field"><asp:Label AssociatedControlID="approvedDate" runat="server" Text="Promise date" /><asp:TextBox ID="approvedDate" runat="server" TextMode="Date" AutoPostBack="true" OnTextChanged="DateChanged" /></div>
            <div class="change-field"><asp:Label AssociatedControlID="technician" runat="server" Text="Available technician" /><asp:DropDownList ID="technician" runat="server" /></div>
        </div>
        <div class="change-fields">
            <div id="decisionGroup" runat="server" class="change-field"><asp:Label AssociatedControlID="decision" runat="server" Text="Decision" /><asp:DropDownList ID="decision" runat="server"><asp:ListItem>Reject</asp:ListItem><asp:ListItem>Approve</asp:ListItem></asp:DropDownList></div>
            <div id="notesField" runat="server" class="change-field full"><asp:Label ID="notesLabel" AssociatedControlID="decisionNote" runat="server" Text="Decision note" /><asp:TextBox ID="decisionNote" runat="server" TextMode="MultiLine" MaxLength="1000" /></div>
        </div>
        <div class="change-actions"><asp:Button ID="saveDecision" runat="server" CssClass="change-action" Text="Save decision" OnClick="Decide" /><asp:Button ID="reassignButton" runat="server" CssClass="change-action" Text="Reassign job" OnClick="Reassign" /></div>
    </asp:Panel>
    <asp:Panel ID="allocationsPane" runat="server" CssClass="change-card">
        <h2>Job notifications</h2><p>Check new allocations and cancellation notices. Open Materials to report when a job needs your usage report.</p>
        <div class="change-table"><asp:GridView ID="notices" runat="server" AutoGenerateColumns="false" OnRowCommand="NoticeCommand" EmptyDataText="You have no unread job notifications."><Columns>
            <asp:BoundField DataField="Message" HeaderText="Notification" /><asp:BoundField DataField="HandoverNote" HeaderText="Handover notes" />
            <asp:BoundField DataField="CreatedAtUtc" HeaderText="Received (UTC)" DataFormatString="{0:dd MMM yyyy HH:mm}" />
            <asp:TemplateField><ItemTemplate><asp:LinkButton runat="server" Text="Mark as read" CommandName="ReadNotice" CommandArgument='<%# Eval("NotificationID") %>' /></ItemTemplate></asp:TemplateField>
        </Columns></asp:GridView></div>
    </asp:Panel>
    <asp:Panel ID="materialsPane" runat="server" CssClass="change-card">
        <h2>Materials to report</h2>
        <div class="change-table"><asp:GridView ID="reports" runat="server" AutoGenerateColumns="false" DataKeyNames="RequestID,JobID" OnSelectedIndexChanged="SelectReport" EmptyDataText="No jobs are awaiting your materials report."><Columns>
            <asp:CommandField ShowSelectButton="true" SelectText="Report usage" /><asp:BoundField DataField="JobID" HeaderText="Job" /><asp:BoundField DataField="PromiseDate" HeaderText="Promise date" DataFormatString="{0:dd MMM yyyy}" />
        </Columns></asp:GridView></div>
        <asp:Panel ID="materialsEditor" runat="server" Visible="false">
            <h2><asp:Literal ID="materialTitle" runat="server" /></h2><p>Enter the quantity actually used for each item, including zero. Unused quantities return to stock on approval.</p>
            <div class="change-table"><asp:GridView ID="materials" runat="server" AutoGenerateColumns="false" DataKeyNames="ItemID" EmptyDataText="No items were allocated. Submit to confirm zero use."><Columns>
                <asp:BoundField DataField="ItemName" HeaderText="Item" /><asp:BoundField DataField="AllocatedQuantity" HeaderText="Allocated" />
                <asp:TemplateField HeaderText="Used"><ItemTemplate><asp:TextBox ID="used" runat="server" TextMode="Number" min="0" step="1" max='<%# Eval("AllocatedQuantity") %>' aria-label='<%# "Used quantity for " + Eval("ItemName") %>' /></ItemTemplate></asp:TemplateField>
            </Columns></asp:GridView></div>
            <asp:Button CssClass="change-action" runat="server" Text="Submit materials report" OnClick="SubmitReport" />
        </asp:Panel>
    </asp:Panel>
    <asp:Button runat="server" Text="Refresh records" OnClick="Refresh" CssClass="dashboard-btn dashboard-btn-secondary" />
</div>
</asp:Content>
