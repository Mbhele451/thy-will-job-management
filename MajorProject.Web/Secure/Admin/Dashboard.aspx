<%@ Page
    Title="Admin Dashboard"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="MajorProject.Web.Admin.Admin" %>

<asp:Content
    ID="HeadContent"
    ContentPlaceHolderID="HeadContent"
    runat="server">
</asp:Content>

<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>

    <div class="dashboard-page">

        <!-- =====================================================
             PAGE HEADER
             ===================================================== -->
        <section class="dashboard-page-header">

            <div>

                <div class="dashboard-eyebrow">
                    <i class="bi bi-shield-lock-fill"></i>
                    ADMINISTRATOR
                </div>

                <h1 class="dashboard-page-title">
                    Admin Dashboard
                </h1>

                <p class="dashboard-page-subtitle">
                    Welcome back,
                    <asp:Label
                        ID="lblAdminGreeting"
                        runat="server"
                        Text="Admin" />.
                    Here's an overview of today's operations.
                </p>

            </div>

        </section>


        <!-- =====================================================
             STATISTICS
             ===================================================== -->
        <section class="dashboard-stats">

            <!-- =================================================
                 TOTAL JOBS
                 Clicking this resets the Jobs GridView to all jobs.
                 ================================================= -->
            <asp:LinkButton
                ID="btnTotalStat"
                runat="server"
                CssClass="dashboard-stat-card"
                OnClick="btnTotalStat_Click"
                CausesValidation="false">

                <div class="dashboard-stat-icon">
                    <i class="bi bi-briefcase"></i>
                </div>

                <div class="dashboard-stat-content">

                    <span class="dashboard-stat-label">
                        Total Jobs
                    </span>

                    <strong class="dashboard-stat-value">
                        <asp:Label
                            ID="lblTotalCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <span class="dashboard-stat-meta">
                        All jobs
                    </span>

                </div>

            </asp:LinkButton>


            <!-- =================================================
                 PENDING
                 ================================================= -->
            <asp:LinkButton
                ID="btnPendingJobs"
                runat="server"
                CssClass="dashboard-stat-card"
                OnClick="btnJobStatus_Click"
                CommandArgument="Pending"
                CausesValidation="false">

                <div class="dashboard-stat-top">
                    <span class="dashboard-stat-label">Pending Jobs</span>

                    <span class="dashboard-stat-icon">
                        <i class="bi bi-clock-history"></i>
                    </span>
                </div>

                <span
                    id="pendingJobsValue"
                    runat="server"
                    class="dashboard-stat-value">
                    0
                </span>

                <span class="dashboard-stat-meta">
                    Awaiting work
                </span>

            </asp:LinkButton>


            <!-- =================================================
                 IN PROGRESS
                 ================================================= -->
           <asp:LinkButton
                ID="btnInProgressJobs"
                runat="server"
                CssClass="dashboard-stat-card"
                OnClick="btnJobStatus_Click"
                CommandArgument="In Progress"
                CausesValidation="false">

                <div class="dashboard-stat-top">
                    <span class="dashboard-stat-label">In Progress</span>

                    <span class="dashboard-stat-icon">
                        <i class="bi bi-tools"></i>
                    </span>
                </div>

                <span
                    id="inProgressJobsValue"
                    runat="server"
                    class="dashboard-stat-value">
                    0
                </span>

                <span class="dashboard-stat-meta">
                    Currently active
                </span>

            </asp:LinkButton>


            <!-- =================================================
                 COMPLETED
                 ================================================= -->
            <asp:LinkButton
                ID="btnCompletedJobs"
                runat="server"
                CssClass="dashboard-stat-card"
                OnClick="btnJobStatus_Click"
                CommandArgument="Completed"
                CausesValidation="false">

                <div class="dashboard-stat-top">
                    <span class="dashboard-stat-label">Completed Jobs</span>

                    <span class="dashboard-stat-icon">
                        <i class="bi bi-check-circle"></i>
                    </span>
                </div>

                <span
                    id="completedJobsValue"
                    runat="server"
                    class="dashboard-stat-value">
                    0
                </span>

                <span class="dashboard-stat-meta">
                    Finished jobs
                </span>

            </asp:LinkButton>

        </section>


        <!-- =====================================================
             JOBS
             ===================================================== -->
        <section class="dashboard-panel">

            <!-- =================================================
                 JOB PANEL HEADER
                 ================================================= -->
            <div class="dashboard-panel-header">

                <div>

                    <div class="dashboard-panel-heading">
                        <span class="dashboard-panel-title">
                            Job Management
                        </span>
                    </div>

                    <p class="dashboard-panel-description">
                        Monitor and review all jobs across the organisation.
                    </p>

                </div>

                <!-- =================================================
                     ALL JOBS RESET BUTTON
                     ================================================= -->
                <div class="dashboard-panel-actions">

                    <asp:LinkButton
                        ID="btnAllJobs"
                        runat="server"
                        CssClass="dashboard-panel-action"
                        OnClick="btnAllJobs_Click"
                        CausesValidation="false">

                        <i class="bi bi-grid-3x3-gap"></i>
                        All Jobs

                    </asp:LinkButton>

                </div>

            </div>


            <!-- =================================================
                 ACTIVE FILTER
                 ================================================= -->
            <div class="dashboard-table-toolbar">

                <div class="dashboard-table-filter">

                    <span class="dashboard-table-filter-label">
                        Showing:
                    </span>

                    <asp:Label
                        ID="lblJobFilter"
                        runat="server"
                        CssClass="dashboard-status-badge">
                        All Jobs
                    </asp:Label>

                </div>

            </div>


            <!-- =================================================
                 JOB TABLE
                 ================================================= -->
            <div class="dashboard-table-wrapper dashboard-jobs-table-wrapper">

                <asp:GridView
                    ID="dgvJobs"
                    runat="server"
                    CssClass="dashboard-table"
                    AutoGenerateColumns="False"
                    GridLines="None"
                    EmptyDataText="No jobs are currently available."
                    AllowPaging="False">

                    <Columns>

                        <asp:BoundField
                            DataField="JobID"
                            HeaderText="Job ID" />

                        <asp:BoundField
                            DataField="CustomerID"
                            HeaderText="Customer ID" />

                        <asp:BoundField
                            DataField="StaffID"
                            HeaderText="Technician ID" />

                        <asp:BoundField
                            DataField="PromiseDate"
                            HeaderText="Promise Date"
                            DataFormatString="{0:dd MMM yyyy}" />

                        <asp:BoundField
                            DataField="JobStatus"
                            HeaderText="Status" />

                        <asp:BoundField
                            DataField="TotalMaterialCost"
                            HeaderText="Materials"
                            DataFormatString="{0:C2}" />

                        <asp:BoundField
                            DataField="LabourCost"
                            HeaderText="Labour"
                            DataFormatString="{0:C2}" />

                        <asp:BoundField
                            DataField="TransportCost"
                            HeaderText="Transport"
                            DataFormatString="{0:C2}" />

                        <asp:BoundField
                            DataField="TaxAmount"
                            HeaderText="Tax"
                            DataFormatString="{0:C2}" />

                        <asp:BoundField
                            DataField="GrandTotal"
                            HeaderText="Grand Total"
                            DataFormatString="{0:C2}" />

                        <asp:BoundField
                            DataField="Notes"
                            HeaderText="Notes" />

                        <asp:BoundField
                            DataField="CreatedAt"
                            HeaderText="Created"
                            DataFormatString="{0:dd MMM yyyy HH:mm}" />

                        <asp:BoundField
                            DataField="UpdatedAt"
                            HeaderText="Updated"
                            DataFormatString="{0:dd MMM yyyy HH:mm}" />

                    </Columns>

                </asp:GridView>

            </div>

        </section>


        <!-- =====================================================
             INVENTORY
             ===================================================== -->
        <section class="dashboard-panel dashboard-inventory-panel">

            <div class="dashboard-panel-header">

                <div>

                    <h2 class="dashboard-panel-title">
                        <i class="bi bi-box-seam-fill"></i>
                        Inventory
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Read-only view of current inventory levels.
                    </p>

                </div>

                <a
                    href="<%= ResolveUrl("~/Secure/Admin/Inventory.aspx") %>"
                    class="dashboard-panel-action">

                    Open Inventory
                    <i class="bi bi-arrow-right"></i>

                </a>

            </div>


            <div class="dashboard-table-wrapper">

                <asp:GridView
                    ID="dgvInventory"
                    runat="server"
                    CssClass="dashboard-table"
                    AutoGenerateColumns="False"
                    GridLines="None"
                    EmptyDataText="No inventory items are currently available.">

                    <Columns>

                        <asp:BoundField
                            DataField="ItemID"
                            HeaderText="Item ID" />

                        <asp:BoundField
                            DataField="ItemName"
                            HeaderText="Item" />

                        <asp:BoundField
                            DataField="Category"
                            HeaderText="Category" />

                        <asp:BoundField
                            DataField="QuantityInStock"
                            HeaderText="In Stock" />

                        <asp:BoundField
                            DataField="UnitPrice"
                            HeaderText="Unit Price"
                            DataFormatString="{0:C2}" />

                        <asp:BoundField
                            DataField="ReorderLevel"
                            HeaderText="Reorder Level" />

                        <asp:BoundField
                            DataField="SupplierName"
                            HeaderText="Supplier" />

                        <asp:BoundField
                            DataField="UpdatedAt"
                            HeaderText="Updated"
                            DataFormatString="{0:dd MMM yyyy HH:mm}" />

                    </Columns>

                </asp:GridView>

            </div>

        </section>


        <!-- =====================================================
             INFORMATION
             ===================================================== -->
        <section class="dashboard-info-banner">

            <div class="dashboard-info-icon">
                <i class="bi bi-info-circle-fill"></i>
            </div>

            <div>

                <strong>
                    Administrator Access
                </strong>

                <p>
                    Administrators can manage customers and jobs.
                    Inventory is displayed here for reference and remains
                    read-only on the dashboard.
                </p>

            </div>

        </section>

    </div>


</div>
</asp:Content>