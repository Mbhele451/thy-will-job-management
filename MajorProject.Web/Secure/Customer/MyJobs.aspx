<%@ Page Title="My Jobs"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="MyJobs.aspx.cs"
    Inherits="MajorProject.Web.Secure.Customer.MyJobs" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
    <p><a href="<%= ResolveUrl("~/Secure/Customer/CancellationFees.aspx") %>">View booking changes and fees</a></p>

    <div class="dashboard-page"  data-record-tabs="secure-customer-myjobs" data-tab-labels="list=My jobs;details=Job details" data-selected-id="lblSelectedJobID">
<asp:Panel ID="pnlMessage"
            runat="server"
            Visible="false"
            CssClass="dashboard-alert">

            <i class="bi bi-info-circle"></i>

            <asp:Label ID="lblMessage"
                runat="server" />

        </asp:Panel>


        <!-- =========================================================
             PAGE HEADER
             ========================================================= -->

        <div class="dashboard-page-header">

            <div>
                <div class="dashboard-eyebrow">
                    CUSTOMER PORTAL
                </div>

                <h1 class="dashboard-page-title">
                    My Jobs
                </h1>

                <p class="dashboard-page-subtitle">
                    View your service requests, appointments and job progress.
                </p>
            </div>

            <div class="dashboard-page-actions">

                <a href="<%= ResolveUrl("~/Secure/Customer/BookJob.aspx") %>"
                   class="dashboard-btn dashboard-btn-primary">
                    <i class="bi bi-plus-lg"></i>
                    Book a Job
                </a>

            </div>

        </div>


        <!-- =========================================================
             STATUS FILTER
             ========================================================= -->

        <section class="dashboard-panel"  data-tab-pane="list">

            <div class="dashboard-panel-header">

                <div>
                    <h2 class="dashboard-panel-title">
                        Your Jobs
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Filter your jobs by their current status.
                    </p>
                </div>

            </div>

            <div class="dashboard-action-group">

                <asp:Button ID="btnAll"
                    runat="server"
                    Text="All"
                    CssClass="dashboard-btn dashboard-btn-secondary"
                    CommandArgument="All"
                    OnClick="StatusFilter_Click" />

                <asp:Button ID="btnPending"
                    runat="server"
                    Text="Pending"
                    CssClass="dashboard-btn dashboard-btn-secondary"
                    CommandArgument="Pending"
                    OnClick="StatusFilter_Click" />

                <asp:Button ID="btnInProgress"
                    runat="server"
                    Text="In Progress"
                    CssClass="dashboard-btn dashboard-btn-secondary"
                    CommandArgument="In Progress"
                    OnClick="StatusFilter_Click" />

                <asp:Button ID="btnCompleted"
                    runat="server"
                    Text="Completed"
                    CssClass="dashboard-btn dashboard-btn-secondary"
                    CommandArgument="Completed"
                    OnClick="StatusFilter_Click" />

                <asp:Button ID="btnCancelled"
                    runat="server"
                    Text="Cancelled"
                    CssClass="dashboard-btn dashboard-btn-secondary"
                    CommandArgument="Cancelled"
                    OnClick="StatusFilter_Click" />

            </div>

        </section>


        <!-- =========================================================
             MESSAGE
             ========================================================= -->

        


        <!-- =========================================================
             JOB LIST
             ========================================================= -->

        <section class="dashboard-panel dashboard-panel-large"  data-tab-pane="list">

            <div class="dashboard-panel-header">

                <div>
                    <h2 class="dashboard-panel-title">
                        Service History
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Your recent and previous service requests.
                    </p>
                </div>

                <div>
                    <asp:Label ID="lblJobCount"
                        runat="server"
                        CssClass="dashboard-role-badge" />
                </div>

            </div>


            <asp:Panel ID="pnlJobs"
                runat="server"
                CssClass="dashboard-table-wrapper">

                <asp:GridView ID="gvJobs"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="dashboard-table"
                    GridLines="None"
                    AllowPaging="true"
                    PageSize="10"
                    OnPageIndexChanging="gvJobs_PageIndexChanging"
                    OnRowCommand="gvJobs_RowCommand"
                    EmptyDataText="You do not have any jobs yet.">

                    <Columns>

                        <asp:BoundField
                            DataField="JobID"
                            HeaderText="Job #" />

                        <asp:BoundField
                            DataField="JobTypeName"
                            HeaderText="Service" />

                        <asp:TemplateField HeaderText="Status">

                            <ItemTemplate>

                                <span class='<%# GetStatusClass(Eval("JobStatus")) %>'>
                                    <%# Eval("JobStatus") %>
                                </span>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Service Date">

                            <ItemTemplate>

                                <%# FormatDate(Eval("PromiseDate")) %>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Requested">

                            <ItemTemplate>

                                <%# FormatDate(Eval("CreatedAt")) %>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="">

                            <ItemTemplate>

                                <asp:LinkButton
                                    ID="btnViewJob"
                                    runat="server"
                                    CommandName="ViewJob"
                                    CommandArgument='<%# Eval("JobID") %>'
                                    CssClass="dashboard-table-action" data-record-select="details">

                                    <i class="bi bi-eye"></i>
                                    View

                                </asp:LinkButton>
                                <asp:HyperLink runat="server" Text="Request rescheduling" NavigateUrl='<%# "CancellationFees.aspx?job=" + Eval("JobID") %>'
                                    Visible='<%# Convert.ToString(Eval("JobStatus")) == "Pending" %>' />
                                <asp:HyperLink runat="server" Text="Request cancellation" NavigateUrl='<%# "CancellationFees.aspx?view=cancel&job=" + Eval("JobID") %>'
                                    Visible='<%# Convert.ToString(Eval("JobStatus")) == "Pending" || Convert.ToString(Eval("JobStatus")) == "In Progress" %>' />
                                <asp:HyperLink runat="server" Text="View fee invoices" NavigateUrl="CancellationFees.aspx?view=fees"
                                    Visible='<%# Convert.ToString(Eval("JobStatus")) == "Cancelled" %>' />

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                    <PagerStyle CssClass="dashboard-pagination" />

                </asp:GridView>

            </asp:Panel>


            <!-- =====================================================
                 EMPTY STATE
                 ===================================================== -->

            <asp:Panel ID="pnlEmpty"
                runat="server"
                Visible="false"
                CssClass="dashboard-empty-state">

                <div class="dashboard-feature-icon">
                    <i class="bi bi-briefcase"></i>
                </div>

                <h3 class="dashboard-feature-title">
                    No jobs found
                </h3>

                <p class="dashboard-feature-text">
                    You do not have any service requests matching this filter.
                </p>

                <a href="<%= ResolveUrl("~/Secure/Customer/BookJob.aspx") %>"
                   class="dashboard-btn dashboard-btn-primary">

                    <i class="bi bi-calendar-plus"></i>
                    Book a Job

                </a>

            </asp:Panel>

        </section>


        <!-- =========================================================
             SELECTED JOB
             ========================================================= -->

        <asp:Panel ID="pnlSelectedJob"
            runat="server"
            Visible="false"
            CssClass="dashboard-panel dashboard-panel-large"  data-tab-pane="details" data-ready-id="lblSelectedJobID" data-ready-kind="positive">

            <div class="dashboard-panel-header">

                <div>

                    <div class="dashboard-eyebrow">
                        JOB DETAILS
                    </div>

                    <h2 class="dashboard-panel-title">

                        Job #
                        <asp:Label ID="lblSelectedJobID"
                            runat="server" />

                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Details for your selected service request.
                    </p>

                </div>

            </div>


            <div class="dashboard-stats">

                <div class="dashboard-stat-card">

                    <div class="dashboard-stat-icon">
                        <i class="bi bi-tools"></i>
                    </div>

                    <div class="dashboard-stat-content">

                        <div class="dashboard-stat-label">
                            Service
                        </div>

                        <div class="dashboard-stat-value dashboard-stat-value-small">

                            <asp:Label ID="lblSelectedService"
                                runat="server" />

                        </div>

                    </div>

                </div>


                <div class="dashboard-stat-card">

                    <div class="dashboard-stat-icon">
                        <i class="bi bi-activity"></i>
                    </div>

                    <div class="dashboard-stat-content">

                        <div class="dashboard-stat-label">
                            Status
                        </div>

                        <div class="dashboard-stat-value dashboard-stat-value-small">

                            <asp:Label ID="lblSelectedStatus"
                                runat="server" />

                        </div>

                    </div>

                </div>


                <div class="dashboard-stat-card">

                    <div class="dashboard-stat-icon">
                        <i class="bi bi-calendar-event"></i>
                    </div>

                    <div class="dashboard-stat-content">

                        <div class="dashboard-stat-label">
                            Service Date
                        </div>

                        <div class="dashboard-stat-value dashboard-stat-value-small">

                            <asp:Label ID="lblSelectedDate"
                                runat="server" />

                        </div>

                    </div>

                </div>

            </div>


<section class="booking-saved-services"><h3 class="dashboard-panel-title">Services in this booking</h3><asp:GridView ID="gvJobServices" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="dashboard-table booking-service-breakdown" EmptyDataText="Service details are not available."><Columns><asp:BoundField DataField="JobTypeName" HeaderText="Service" HtmlEncode="true" /><asp:BoundField DataField="Quantity" HeaderText="Quantity" /></Columns></asp:GridView></section>
            <div class="dashboard-feature-content">

                <div class="dashboard-feature-icon">
                    <i class="bi bi-geo-alt"></i>
                </div>

                <div>

                    <h3 class="dashboard-feature-title">
                        Service Location
                    </h3>

                    <p class="dashboard-feature-text">

                        <asp:Label ID="lblSelectedAddress"
                            runat="server" />

                    </p>

                </div>

            </div>


            <div class="dashboard-feature-content">

                <div class="dashboard-feature-icon">
                    <i class="bi bi-chat-left-text"></i>
                </div>

                <div>

                    <h3 class="dashboard-feature-title">
                        Service Description
                    </h3>

                    <p class="dashboard-feature-text">

                        <asp:Label ID="lblSelectedDescription"
                            runat="server" />

                    </p>

                </div>

            </div>

        </asp:Panel>

    </div>


</div>
</asp:Content>
