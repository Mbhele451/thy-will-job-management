
<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="Dashboard.aspx.cs"
    Inherits="MajorProject.Web.Secure.Customer.Dashboard"
    MasterPageFile="~/MasterPages/Dashboard.Master" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-customer-dashboard">


    


    <div class="dashboard-page">

        <!-- PAGE HEADER -->

        <div class="dashboard-page-header">

            <div>
                <div class="dashboard-eyebrow">
                    CUSTOMER PORTAL
                </div>

                <h1 class="dashboard-page-title">
                    Welcome,
                    <asp:Label ID="lblCustomerName"
                        runat="server"
                        Text="Customer">
                    </asp:Label>
                </h1>

                <p class="dashboard-page-subtitle">
                    Here's an overview of your services and account.
                </p>
            </div>

            <div class="dashboard-page-actions">

                <asp:HyperLink ID="lnkBookJob"
                    runat="server"
                    NavigateUrl="~/Secure/Customer/BookJob.aspx"
                    CssClass="btn dashboard-btn-primary">
                    Book a Service
                </asp:HyperLink>

            </div>

        </div>


        <!-- DASHBOARD ALERT -->

        <asp:Panel ID="pnlError"
            runat="server"
            Visible="false"
            CssClass="dashboard-alert dashboard-alert-danger">

            <asp:Label ID="lblError"
                runat="server">
            </asp:Label>

        </asp:Panel>


        <!-- STATISTICS -->

        <div class="dashboard-stats">

            <!-- ACTIVE JOBS -->

            <div class="dashboard-stat-card">
                <div class="dashboard-stat-content">

                    <span class="dashboard-stat-label">
                        Active Jobs
                    </span>

                    <asp:Label ID="lblActiveJobsCount"
                        runat="server"
                        CssClass="dashboard-stat-value"
                        Text="0">
                    </asp:Label>

                    <span class="dashboard-stat-meta">
                        Pending or in progress
                    </span>

                </div>
            </div>


            <!-- COMPLETED JOBS -->

            <div class="dashboard-stat-card">
                <div class="dashboard-stat-content">

                    <span class="dashboard-stat-label">
                        Completed Jobs
                    </span>

                    <asp:Label ID="lblCompletedJobsCount"
                        runat="server"
                        CssClass="dashboard-stat-value"
                        Text="0">
                    </asp:Label>

                    <span class="dashboard-stat-meta">
                        Completed services
                    </span>

                </div>
            </div>


            <!-- INVOICES -->

            <div class="dashboard-stat-card">
                <div class="dashboard-stat-content">

                    <span class="dashboard-stat-label">
                        Invoices
                    </span>

                    <asp:Label ID="lblInvoiceCount"
                        runat="server"
                        CssClass="dashboard-stat-value"
                        Text="0">
                    </asp:Label>

                    <span class="dashboard-stat-meta">
                        Recorded invoices
                    </span>

                </div>
            </div>


            <!-- NEXT SERVICE REPLACES TOTAL BILLED -->

            <asp:Panel ID="pnlNextService"
                runat="server"
                CssClass="dashboard-stat-card dashboard-next-service-stat">

                <div class="dashboard-stat-content">

                    <span class="dashboard-stat-label">
                        NEXT SERVICE
                    </span>

                    <asp:Label ID="lblNextServiceType"
                        runat="server"
                        CssClass="dashboard-next-service-type"
                        Text="Service">
                    </asp:Label>

                    <span class="dashboard-next-job">
                        Job #
                        <asp:Label ID="lblNextServiceJobID"
                            runat="server"
                            Text="0">
                        </asp:Label>
                    </span>

                    <div class="dashboard-next-meta">

                        <span>
                            Area:
                            <asp:Label ID="lblNextServiceArea"
                                runat="server"
                                Text="Not available">
                            </asp:Label>
                        </span>

                        <span>
                            Date:
                            <asp:Label ID="lblNextServiceDate"
                                runat="server"
                                Text="Not scheduled">
                            </asp:Label>
                        </span>

                    </div>

                    <span class="dashboard-next-status">
                        <asp:Label ID="lblNextServiceStatus"
                            runat="server"
                            Text="Pending">
                        </asp:Label>
                    </span>

                    <asp:HyperLink ID="lnkNextService"
                        runat="server"
                        NavigateUrl="~/Secure/Customer/MyJobs.aspx"
                        CssClass="dashboard-next-link">
                        View My Jobs
                    </asp:HyperLink>

                </div>

            </asp:Panel>


            <!-- NO UPCOMING SERVICE -->

            <asp:Panel ID="pnlNoNextService"
                runat="server"
                Visible="false"
                CssClass="dashboard-stat-card dashboard-next-service-stat">

                <div class="dashboard-stat-content">

                    <span class="dashboard-stat-label">
                        NEXT SERVICE
                    </span>

                    <strong class="dashboard-next-service-type">
                        No upcoming services
                    </strong>

                    <span class="dashboard-no-service">
                        You have no upcoming services. Check My Jobs for earlier bookings.
                    </span>

                    <asp:HyperLink ID="lnkBookJobEmpty"
                        runat="server"
                        NavigateUrl="~/Secure/Customer/BookJob.aspx"
                        CssClass="dashboard-next-link">
                        Book a Service
                    </asp:HyperLink>

                </div>

            </asp:Panel>

        </div>


        <!-- HIDDEN LEGACY CONTROL
             Retained so existing Dashboard.aspx.cs references compile. -->

        <asp:Label ID="lblTotalBilled"
            runat="server"
            Text="R 0.00"
            style="display: none;">
        </asp:Label>


        <!-- RECENT JOBS AND ACCOUNT ACTIVITY -->

        <div class="dashboard-grid-2">

            <!-- RECENT JOBS -->

            <div class="dashboard-panel">

                <div class="dashboard-panel-header">

                    <div>
                        <div class="dashboard-eyebrow">
                            SERVICES
                        </div>

                        <h2 class="dashboard-panel-title">
                            Recent Jobs
                        </h2>
                    </div>

                    <asp:HyperLink ID="lnkViewAllJobs"
                        runat="server"
                        NavigateUrl="~/Secure/Customer/MyJobs.aspx"
                        CssClass="dashboard-panel-link">
                        View all
                    </asp:HyperLink>

                </div>


                <div class="dashboard-table-wrapper">

                    <asp:GridView ID="gvRecentJobs"
                        runat="server"
                        AutoGenerateColumns="false"
                        DataKeyNames="JobID"
                        CssClass="dashboard-table"
                        GridLines="None"
                        EmptyDataText="No jobs have been recorded yet."
                        OnRowCommand="gvRecentJobs_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="JobID"
                                HeaderText="Job" />

                            <asp:BoundField
                                DataField="JobTypeName"
                                HeaderText="Service" />

                            <asp:BoundField
                                DataField="JobStatus"
                                HeaderText="Status" />

                            <asp:BoundField
                                DataField="PromiseDate"
                                HeaderText="Date"
                                DataFormatString="{0:dd MMM yyyy}" />

                            <asp:TemplateField HeaderText="">

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="btnViewJob"
                                        runat="server"
                                        CommandName="ViewJob"
                                        CommandArgument='<%# Eval("JobID") %>'
                                        CssClass="dashboard-table-action"
                                        ToolTip="View this job">
                                        View
                                    </asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>

                </div>

            </div>


            <!-- ACCOUNT ACTIVITY -->

            <div class="dashboard-panel">

                <div class="dashboard-panel-header">

                    <div>
                        <div class="dashboard-eyebrow">
                            ACTIVITY
                        </div>

                        <h2 class="dashboard-panel-title">
                            Account Activity
                        </h2>
                    </div>

                    <asp:HyperLink ID="lnkViewInvoices"
                        runat="server"
                        NavigateUrl="~/Secure/Customer/Invoices.aspx"
                        CssClass="dashboard-panel-link">
                        Invoices
                    </asp:HyperLink>

                </div>


                <asp:Repeater ID="rptActivity"
                    runat="server">

                    <HeaderTemplate>
                        <div class="dashboard-activity-list">
                    </HeaderTemplate>

                    <ItemTemplate>

                        <div class="dashboard-activity-item">

                            <div class="dashboard-activity-content">

                                <strong>
                                    <%# Eval("Title") %>
                                </strong>

                                <span>
                                    <%# Eval("Description") %>
                                </span>

                                <small>
                                    <%# Eval("DateText") %>
                                </small>

                            </div>

                        </div>

                    </ItemTemplate>

                    <FooterTemplate>
                        </div>
                    </FooterTemplate>

                </asp:Repeater>


                <asp:Panel ID="pnlNoActivity"
                    runat="server"
                    Visible="false"
                    CssClass="dashboard-empty-small">

                    <span>
                        No recent account activity.
                    </span>

                </asp:Panel>

            </div>

        </div>

    </div>


</div>

</div>
</asp:Content>
