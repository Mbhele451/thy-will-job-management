<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Jobs.aspx.cs"
    Inherits="MajorProject.Web.Secure.Technician.Jobs"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    Title="My Jobs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="tw-page-stage" data-page-stage>

    <div class="dashboard-page dashboard-technician-jobs"  data-record-tabs="secure-technician-jobs" data-tab-labels="list=Assigned jobs;details=Job details" data-selected-id="pnlJobDetails">
<asp:Panel
                    ID="pnlJobMessage"
                    runat="server"
                    CssClass="dashboard-job-message"
                    Visible="false">

                    <asp:Label
                        ID="lblJobMessage"
                        runat="server">
                    </asp:Label>

                </asp:Panel>


        <!-- =========================================================
             PAGE HEADER
             ========================================================= -->
        <div class="dashboard-page-header">
            <div>
                <span class="dashboard-page-eyebrow">TECHNICIAN OPERATIONS</span>

                <h1 class="dashboard-page-title">
                    My Jobs
                </h1>

                <p class="dashboard-page-description">
                    View and manage jobs assigned to you.
                </p>
            </div>
        </div>


        <!-- =========================================================
             JOB LIST
             ========================================================= -->
        <section class="dashboard-panel"  data-tab-pane="list">

            <div class="dashboard-panel-header">

                <div>
                    <h2 class="dashboard-panel-title">
                        Assigned Jobs
                    </h2>

                    <p class="dashboard-panel-description">
                        Search or filter your assigned jobs to view customer and job information.
                    </p>
                </div>

            </div>


            <div class="dashboard-panel-body">

                <!-- =================================================
                     SEARCH / FILTERS
                     ================================================= -->
                <div class="dashboard-jobs-toolbar">

                    <div class="dashboard-jobs-filter dashboard-jobs-search">

                        <label for="<%= txtJobSearch.ClientID %>"
                               class="dashboard-jobs-filter-label">
                            Search
                        </label>

                        <asp:TextBox
                            ID="txtJobSearch"
                            runat="server"
                            CssClass="dashboard-form-control"
                            placeholder="Search Job ID or customer name..."
                            AutoPostBack="true"
                            OnTextChanged="txtJobSearch_TextChanged">
                        </asp:TextBox>

                    </div>


                    <div class="dashboard-jobs-filter">

                        <label for="<%= ddlAreaFilter.ClientID %>"
                               class="dashboard-jobs-filter-label">
                            Area
                        </label>

                        <asp:DropDownList
                            ID="ddlAreaFilter"
                            runat="server"
                            CssClass="dashboard-form-control"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlAreaFilter_SelectedIndexChanged">
                        </asp:DropDownList>

                    </div>


                    <div class="dashboard-jobs-filter">

                        <label for="<%= ddlStatusFilter.ClientID %>"
                               class="dashboard-jobs-filter-label">
                            Status
                        </label>

                        <asp:DropDownList
                            ID="ddlStatusFilter"
                            runat="server"
                            CssClass="dashboard-form-control"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">

                            <asp:ListItem Text="All Statuses" Value="All"></asp:ListItem>
                            <asp:ListItem Text="Pending" Value="Pending"></asp:ListItem>
                            <asp:ListItem Text="In Progress" Value="In Progress"></asp:ListItem>
                            <asp:ListItem Text="Completed" Value="Completed"></asp:ListItem>
                            <asp:ListItem Text="Cancelled" Value="Cancelled"></asp:ListItem>

                        </asp:DropDownList>

                    </div>

                </div>


                <!-- =================================================
                     MESSAGE
                     ================================================= -->
                


                <!-- =================================================
                     JOB TABLE
                     ================================================= -->
                <div class="dashboard-table-wrapper dashboard-jobs-table-wrapper">

                    <asp:GridView
                        ID="dgvJobs"
                        runat="server"
                        AutoGenerateColumns="false"
                        DataKeyNames="JobID"
                        CssClass="dashboard-table"
                        GridLines="None"
                        AllowPaging="false"
                        OnSelectedIndexChanged="dgvJobs_SelectedIndexChanged">

                        <Columns>

                            <asp:BoundField
                                DataField="JobID"
                                HeaderText="Job ID" />

                            <asp:BoundField
                                DataField="CustomerName"
                                HeaderText="Customer" />

                            <asp:BoundField
                                DataField="Area"
                                HeaderText="Area" />

                            <asp:BoundField
                                DataField="PromiseDate"
                                HeaderText="Promise Date"
                                DataFormatString="{0:dd MMM yyyy}" />

                            <asp:BoundField
                                DataField="JobStatus"
                                HeaderText="Status" />

                            <asp:TemplateField
                                HeaderText="View">

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="btnViewJob"
                                        runat="server"
                                        CommandName="Select"
                                        CssClass="dashboard-job-view-link" data-record-select="details">
                                        View
                                    </asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                        <HeaderStyle CssClass="dashboard-table-header" />
                        <SelectedRowStyle CssClass="dashboard-table-row-selected" />

                        <EmptyDataTemplate>

                            <div class="dashboard-empty-state">
                                <div class="dashboard-empty-state-title">
                                    No jobs found
                                </div>

                                <div class="dashboard-empty-state-text">
                                    There are no jobs matching your current search or filters.
                                </div>
                            </div>

                        </EmptyDataTemplate>

                    </asp:GridView>

                </div>

            </div>

        </section>


        <!-- =========================================================
             SELECTED JOB DETAILS
             ========================================================= -->
        <section class="dashboard-panel dashboard-job-details-panel"  data-tab-pane="details" data-ready-id="pnlJobDetails" data-ready-kind="exists">

            <div class="dashboard-panel-header">

                <div>
                    <h2 class="dashboard-panel-title">
                        Job Details
                    </h2>

                    <p class="dashboard-panel-description">
                        Select a job above to view the customer and job information.
                    </p>
                </div>

                <asp:Label
                    ID="lblSelectedJobStatus"
                    runat="server"
                    CssClass="dashboard-job-status">
                </asp:Label>

            </div>


            <div class="dashboard-panel-body">

                <asp:Panel
                    ID="pnlJobDetails"
                    runat="server"
                    Visible="false">

                    <!-- =============================================
                     CUSTOMER DETAILS
                     ============================================= -->
                <div class="dashboard-job-details-section">

                    <div class="dashboard-job-details-section-header">
                        <h3>Customer Information</h3>
                    </div>


                    <div class="dashboard-job-details-grid">

                        <!-- CUSTOMER NAME -->
                        <div class="dashboard-job-detail-item">

                            <span class="dashboard-job-detail-label">
                                Full Name
                            </span>

                            <asp:Label
                                ID="lblCustomerFullName"
                                runat="server"
                                CssClass="dashboard-job-detail-value">
                            </asp:Label>

                        </div>


                        <!-- PHONE NUMBER -->
                        <div class="dashboard-job-detail-item">

                            <span class="dashboard-job-detail-label">
                                Phone Number
                            </span>

                            <asp:HyperLink
                                ID="btnCustomerPhone"
                                runat="server"
                                CssClass="dashboard-job-detail-value dashboard-job-phone-link">
                            </asp:HyperLink>

                        </div>


                        <!-- ADDRESS -->
                        <div class="dashboard-job-detail-item dashboard-job-detail-wide">

                            <span class="dashboard-job-detail-label">
                                Full Address
                            </span>

                            <asp:HyperLink
                                ID="btnCustomerAddress"
                                runat="server"
                                CssClass="dashboard-job-detail-value dashboard-job-address-link"
                                Target="_blank">
                            </asp:HyperLink>

                        </div>


                        <!-- AREA -->
                        <div class="dashboard-job-detail-item">

                            <span class="dashboard-job-detail-label">
                                Area
                            </span>

                            <asp:Label
                                ID="lblCustomerArea"
                                runat="server"
                                CssClass="dashboard-job-detail-value">
                            </asp:Label>

                        </div>

                    </div>


                    <!-- =============================================
                         CUSTOMER ACTIONS
                         ============================================= -->
                    <div class="dashboard-job-customer-actions">

                        <asp:HyperLink
                            ID="btnCallCustomer"
                            runat="server"
                            CssClass="dashboard-job-customer-action dashboard-job-customer-action-call">
                            <i class="bi bi-telephone-fill"></i>
                            <span>Call Customer</span>
                        </asp:HyperLink>


                        <asp:HyperLink
                            ID="btnNavigateCustomer"
                            runat="server"
                            CssClass="dashboard-job-customer-action dashboard-job-customer-action-navigate"
                            Target="_blank">
                            <i class="bi bi-geo-alt-fill"></i>
                            <span>Navigate</span>
                        </asp:HyperLink>

                    </div>


                    <!-- =============================================
                         MAP
                         ============================================= -->
                    <div class="dashboard-job-map-container">

                        <asp:HyperLink
                            ID="btnCustomerMap"
                            runat="server"
                            CssClass="dashboard-job-map-button"
                            Target="_blank"
                            Text="View Customer Map">
                        </asp:HyperLink>

                    </div>

                </div>


                    <!-- =============================================
                         JOB DETAILS
                         ============================================= -->
                    <div class="dashboard-job-details-section">

                        <div class="dashboard-job-details-section-header">
                            <h3>Job Information</h3>
                        </div>


                        <div class="dashboard-job-details-grid">

                            <div class="dashboard-job-detail-item">

                                <span class="dashboard-job-detail-label">
                                    Job ID
                                </span>

                                <asp:Label
                                    ID="lblJobID"
                                    runat="server"
                                    CssClass="dashboard-job-detail-value">
                                </asp:Label>

                            </div>


                            <div class="dashboard-job-detail-item">

                                <span class="dashboard-job-detail-label">
                                    Job Status
                                </span>

                                <asp:Label
                                    ID="lblJobStatus"
                                    runat="server"
                                    CssClass="dashboard-job-detail-value">
                                </asp:Label>

                            </div>


                            <div class="dashboard-job-detail-item">

                                <span class="dashboard-job-detail-label">
                                    Created At
                                </span>

                                <asp:Label
                                    ID="lblJobCreatedAt"
                                    runat="server"
                                    CssClass="dashboard-job-detail-value">
                                </asp:Label>

                            </div>


                            <div class="dashboard-job-detail-item">

                                <span class="dashboard-job-detail-label">
                                    Promise Date
                                </span>

                                <asp:Label
                                    ID="lblJobPromiseDate"
                                    runat="server"
                                    CssClass="dashboard-job-detail-value">
                                </asp:Label>

                            </div>


                            <div class="dashboard-job-detail-item dashboard-job-detail-wide">

                                <span class="dashboard-job-detail-label">
                                    Notes
                                </span>

                                <asp:Label
                                    ID="lblJobNotes"
                                    runat="server"
                                    CssClass="dashboard-job-detail-value dashboard-job-notes">
                                </asp:Label>

                            </div>

                        </div>

                    </div>


                    <!-- =============================================
                         JOB ACTIONS
                         ============================================= -->
                    <div class="dashboard-job-actions">

                        <asp:LinkButton
                            ID="btnStartJob"
                            runat="server"
                            CssClass="dashboard-job-action dashboard-job-action-start"
                            Text="Start Job"
                            OnClick="btnStartJob_Click"
                            OnClientClick="return confirm('Are you sure you want to start this job?');">
                        </asp:LinkButton>


                        <asp:LinkButton
                            ID="btnFinishJob"
                            runat="server"
                            CssClass="dashboard-job-action dashboard-job-action-finish"
                            Text="Finish Job"
                            OnClick="btnFinishJob_Click"
                            OnClientClick="return confirm('Are you sure you want to mark this job as completed?');">
                        </asp:LinkButton>


                        <asp:LinkButton
                            ID="btnCancelJob"
                            runat="server"
                            CssClass="dashboard-job-action dashboard-job-action-cancel"
                            Text="Materials to report"
                            OnClick="btnCancelJob_Click"
                            >
                        </asp:LinkButton>

                    </div>

                </asp:Panel>


                <!-- =============================================
                     NO SELECTED JOB
                     ============================================= -->
                <asp:Panel
                    ID="pnlNoJobSelected"
                    runat="server"
                    CssClass="dashboard-empty-state">

                    <div class="dashboard-empty-state-title">
                        No Job Selected
                    </div>

                    <div class="dashboard-empty-state-text">
                        Select a job from the table above to view its details.
                    </div>

                </asp:Panel>

            </div>

        </section>

    </div>


</div>
</asp:Content>