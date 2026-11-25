<%@ Page Title="Technician Dashboard"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="MajorProject.Web.Technician.Dashboard" %>

<asp:Content ID="Content1"
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
                    <i class="bi bi-tools"></i>
                    TECHNICIAN OPERATIONS
                </div>

                <h1 class="dashboard-page-title">
                    Technician Dashboard
                </h1>

                <p class="dashboard-page-subtitle">
                    <asp:Label
                        ID="lblAdminGreeting"
                        runat="server"
                        Text="Welcome, Technician" />
                </p>

            </div>


        </section>


        <!-- =====================================================
             QUICK STATISTICS
             Entire cards are clickable.

             Total Jobs:
             - Shows all jobs assigned to the technician.
             - Resets the GridView filter.

             Pending / In Progress / Completed:
             - Filter the technician's assigned jobs.
             ===================================================== -->
        <section class="dashboard-stats">

            <!-- =================================================
                 TOTAL JOBS
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
                        All assigned jobs
                    </span>

                </div>

            </asp:LinkButton>


            <!-- =================================================
                 PENDING
                 ================================================= -->
            <asp:LinkButton
                ID="btnPendingStat"
                runat="server"
                CssClass="dashboard-stat-card dashboard-stat-warning"
                OnClick="btnStat_Click"
                CommandArgument="Pending"
                CausesValidation="false">

                <div class="dashboard-stat-icon">
                    <i class="bi bi-hourglass-split"></i>
                </div>

                <div class="dashboard-stat-content">

                    <span class="dashboard-stat-label">
                        Pending Jobs
                    </span>

                    <strong class="dashboard-stat-value">
                        <asp:Label
                            ID="lblPendingCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <span class="dashboard-stat-meta">
                        Waiting to start
                    </span>

                </div>

            </asp:LinkButton>


            <!-- =================================================
                 IN PROGRESS
                 ================================================= -->
            <asp:LinkButton
                ID="btnInProgressStat"
                runat="server"
                CssClass="dashboard-stat-card dashboard-stat-info"
                OnClick="btnStat_Click"
                CommandArgument="In Progress"
                CausesValidation="false">

                <div class="dashboard-stat-icon">
                    <i class="bi bi-gear-wide-connected"></i>
                </div>

                <div class="dashboard-stat-content">

                    <span class="dashboard-stat-label">
                        In Progress
                    </span>

                    <strong class="dashboard-stat-value">
                        <asp:Label
                            ID="lblInProgressCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <span class="dashboard-stat-meta">
                        Currently working
                    </span>

                </div>

            </asp:LinkButton>


            <!-- =================================================
                 COMPLETED
                 ================================================= -->
            <asp:LinkButton
                ID="btnCompletedStat"
                runat="server"
                CssClass="dashboard-stat-card dashboard-stat-success"
                OnClick="btnStat_Click"
                CommandArgument="Completed"
                CausesValidation="false">

                <div class="dashboard-stat-icon">
                    <i class="bi bi-check2-circle"></i>
                </div>

                <div class="dashboard-stat-content">

                    <span class="dashboard-stat-label">
                        Completed Jobs
                    </span>

                    <strong class="dashboard-stat-value">
                        <asp:Label
                            ID="lblCompletedCount"
                            runat="server"
                            Text="0" />
                    </strong>

                    <span class="dashboard-stat-meta">
                        Successfully completed
                    </span>

                </div>

            </asp:LinkButton>

        </section>


        <!-- =====================================================
             MY ASSIGNED JOBS
             ===================================================== -->
        <section class="dashboard-panel dashboard-panel-large">

            <div class="dashboard-panel-header">

                <div>

                    <div class="dashboard-panel-title">
                        <i class="bi bi-briefcase"></i>
                        My Assigned Jobs
                    </div>

                    <div class="dashboard-panel-subtitle">
                        Jobs assigned to you
                    </div>

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
            <div class="dashboard-table-wrapper">

                <asp:GridView
                    ID="dgvJobs"
                    runat="server"
                    CssClass="dashboard-table"
                    AutoGenerateColumns="False"
                    DataKeyNames="JobID"
                    EmptyDataText="No jobs match the selected filter.">

                    <Columns>

                        <asp:BoundField
                            DataField="JobID"
                            HeaderText="Job ID" />

                        <asp:BoundField
                            DataField="CustomerID"
                            HeaderText="Customer" />

                        <asp:BoundField
                            DataField="JobStatus"
                            HeaderText="Status" />

                        <asp:BoundField
                            DataField="PromiseDate"
                            HeaderText="Promise Date"
                            DataFormatString="{0:dd MMM yyyy}" />

                    </Columns>

                </asp:GridView>

            </div>

        </section>


        <!-- =====================================================
             INVENTORY
             ===================================================== -->
        <section class="dashboard-panel">

            <div class="dashboard-panel-header">

                <div>

                    <div class="dashboard-panel-title">
                        <i class="bi bi-box-seam"></i>
                        Inventory
                    </div>

                    <div class="dashboard-panel-subtitle">
                        Read-only view of available inventory
                    </div>

                </div>

            </div>


            <div class="dashboard-table-wrapper">

                <asp:GridView
                    ID="dgvInventory"
                    runat="server"
                    CssClass="dashboard-table"
                    AutoGenerateColumns="True"
                    EmptyDataText="No inventory items are currently available."
                    AllowPaging="False">

                </asp:GridView>

            </div>

        </section>


        <!-- =====================================================
             INFORMATION
             ===================================================== -->
        <section class="dashboard-info-banner">

            <div class="dashboard-info-icon">
                <i class="bi bi-info-circle"></i>
            </div>

            <div>

                <strong>
                    Technician Workflow
                </strong>

                <p>
                    Use the statistics above to view your total assigned
                    workload or filter your jobs by status. Select My Jobs
                    from the navigation bar to open the full job workspace
                    where individual jobs can be reviewed and worked on.
                </p>

            </div>

        </section>

    </div>


</div>
</asp:Content>