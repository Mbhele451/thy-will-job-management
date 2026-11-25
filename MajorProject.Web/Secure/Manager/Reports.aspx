<%@ Page
    Title="Reports"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Reports.aspx.cs"
    Inherits="MajorProject.Web.Manager.Reports" %>

<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>

    <section class="dashboard-page-header reveal">

        <div>
            <div class="dashboard-eyebrow">
                MANAGEMENT
            </div>

            <h1 class="dashboard-page-title">
                Reports
            </h1>

            <p class="dashboard-page-subtitle">
                View operational and financial reports for THY WILL Electrical & Construction.
            </p>
        </div>

    </section>


    <!-- REPORT SUMMARY -->
    <section class="dashboard-panel reports-intro-panel reveal">

        <div class="dashboard-panel-header">

            <div>
                <h2 class="dashboard-panel-title">
                    Business Intelligence
                </h2>

                <p class="dashboard-panel-subtitle">
                    Use the interactive report below to analyse jobs, revenue,
                    materials, technicians and business performance.
                </p>
            </div>

            <div class="reports-status">
                <span class="reports-status-dot"></span>
                Live Report
            </div>

        </div>

    </section>


    <!-- POWER BI REPORT -->
    <section class="dashboard-panel reports-viewer-panel mt-4 reveal">

        <div class="dashboard-panel-header">

            <div>
                <h2 class="dashboard-panel-title">
                    Management Report
                </h2>

                <p class="dashboard-panel-subtitle">
                    Interactive Power BI dashboard.
                </p>
            </div>

        </div>


        <div class="reports-viewer">

            <iframe
                id="powerBiReport"
                runat="server"
                class="powerbi-frame"
                title="THY WILL Management Report"
                frameborder="0"
                allowfullscreen="true">
            </iframe>

        </div>

    </section>


</div>
</asp:Content>