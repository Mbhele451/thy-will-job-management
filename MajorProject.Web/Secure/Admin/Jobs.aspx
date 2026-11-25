<%@ Page
    Title="Job Management"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Jobs.aspx.cs"
    Inherits="MajorProject.Web.Secure.Admin.Jobs" %>

<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>

    <asp:UpdatePanel
        ID="updJobs"
        runat="server">

        <ContentTemplate>
<div class="tw-record-workspace" data-record-tabs="secure-admin-jobs" data-tab-labels="list=Jobs;details=Job details" data-selected-id="lblJobID">

<asp:Label
                        ID="lblMessage"
                        runat="server"
                        EnableViewState="false"
                        CssClass="d-block mb-3" />


            <!-- PAGE HEADER -->
            <section class="dashboard-page-header reveal">

                <div>
                    <div class="dashboard-eyebrow">
                        ADMINISTRATION
                    </div>

                    <h1 class="dashboard-page-title">
                        Job Management
                    </h1>

                    <p class="dashboard-page-subtitle">
                        Search, reschedule and manage customer service jobs.
                    </p>
                </div>

            </section>


            <!-- SEARCH / FILTER -->
            <section class="dashboard-panel reveal tw-filter-panel"  data-tab-pane="list">

                <div class="dashboard-panel-header">

                    <div>
                        <h2 class="dashboard-panel-title">
                            Find Jobs
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            Search by job number, customer or technician.
                        </p>
                    </div>

                </div>


                <div class="p-4">

                    <div class="row g-3 align-items-end">

                        <div class="col-lg-6">

                            <label
                                for="<%= txtSearch.ClientID %>"
                                class="form-label">
                                Search
                            </label>

                            <asp:TextBox
                                ID="txtSearch"
                                runat="server"
                                CssClass="form-control"
                                placeholder="Job ID, customer or technician" />

                        </div>


                        <div class="col-lg-3">

                            <label
                                for="<%= ddlStatus.ClientID %>"
                                class="form-label">
                                Status
                            </label>

                            <asp:DropDownList
                                ID="ddlStatus"
                                runat="server"
                                CssClass="form-select">

                                <asp:ListItem
                                    Text="All Jobs"
                                    Value="All" />

                                <asp:ListItem
                                    Text="Pending"
                                    Value="Pending" />

                                <asp:ListItem
                                    Text="In Progress"
                                    Value="In Progress" />

                                <asp:ListItem
                                    Text="Completed"
                                    Value="Completed" />

                                <asp:ListItem
                                    Text="Cancelled"
                                    Value="Cancelled" />

                            </asp:DropDownList>

                        </div>


                        <div class="col-lg-3">

                            <div class="d-flex gap-2">

                                <asp:Button
                                    ID="btnSearch"
                                    runat="server"
                                    Text="Search"
                                    CssClass="dashboard-btn"
                                    OnClick="btnSearch_Click" />

                                <asp:Button
                                    ID="btnClear"
                                    runat="server"
                                    Text="Clear"
                                    CssClass="dashboard-btn dashboard-btn-secondary"
                                    CausesValidation="false"
                                    OnClick="btnClear_Click" />

                            </div>

                        </div>

                    </div>

                </div>

            </section>


            <!-- JOB LIST -->
            <section class="dashboard-panel mt-4 reveal"  data-tab-pane="list">

                <div class="dashboard-panel-header">

                    <div>
                        <h2 class="dashboard-panel-title">
                            Jobs
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            Select a job to manage its schedule or cancellation.
                        </p>
                    </div>

                </div>


                <div class="dashboard-table-wrapper jobs-table-wrapper">

                    <asp:GridView
                        ID="dgvJobs"
                        runat="server"
                        CssClass="dashboard-table"
                        AutoGenerateColumns="False"
                        DataKeyNames="JobID"
                        EmptyDataText="No jobs found."
                        OnSelectedIndexChanged="dgvJobs_SelectedIndexChanged">

                        <Columns>

                            <asp:CommandField
                                ShowSelectButton="True"
                                SelectText="View"
                                ButtonType="Button"
                                ControlStyle-CssClass="dashboard-btn dashboard-btn-sm" />

                            <asp:BoundField
                                DataField="JobID"
                                HeaderText="Job #" />

                            <asp:BoundField
                                DataField="CustomerName"
                                HeaderText="Customer" />

                            <asp:BoundField
                                DataField="JobTypeName"
                                HeaderText="Service" />

                            <asp:BoundField
                                DataField="TechnicianName"
                                HeaderText="Technician" />

                            <asp:BoundField
                                DataField="PromiseDate"
                                HeaderText="Promise Date"
                                DataFormatString="{0:dd MMM yyyy}" />

                            <asp:BoundField
                                DataField="JobStatus"
                                HeaderText="Status" />

                            <asp:BoundField
                                DataField="GrandTotal"
                                HeaderText="Total"
                                DataFormatString="R {0:N2}" />

                        </Columns>

                    </asp:GridView>

                </div>

            </section>


            <!-- SELECTED JOB -->
            <section class="dashboard-panel mt-4 reveal"  data-tab-pane="details" data-ready-id="lblJobID" data-ready-kind="positive">

                <div class="dashboard-panel-header">

                    <div>
                        <h2 class="dashboard-panel-title">
                            Selected Job
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            Review the selected job before making a change.
                        </p>
                    </div>

                </div>


                <div class="p-4">

                    


                    <div class="row g-3">

                        <div class="col-lg-6">

                            <div class="dashboard-panel h-100">

                                <div class="p-4">

                                    <h3 class="dashboard-panel-title">
                                        Job Details
                                    </h3>

                                    <div class="mt-3">

                                        <p>
                                            <strong>Job:</strong>
                                            <asp:Label
                                                ID="lblJobID"
                                                runat="server"
                                                Text="—" />
                                        </p>

                                        <p>
                                            <strong>Customer:</strong>
                                            <asp:Label
                                                ID="lblCustomer"
                                                runat="server"
                                                Text="—" />
                                        </p>

                                        <p>
                                            <strong>Service:</strong>
                                            <asp:Label
                                                ID="lblService"
                                                runat="server"
                                                Text="—" />
                                        </p>

                                        <p>
                                            <strong>Technician:</strong>
                                            <asp:Label
                                                ID="lblTechnician"
                                                runat="server"
                                                Text="—" />
                                        </p>

                                        <p>
                                            <strong>Promise Date:</strong>
                                            <asp:Label
                                                ID="lblPromiseDate"
                                                runat="server"
                                                Text="—" />
                                        </p>

                                        <p>
                                            <strong>Status:</strong>
                                            <asp:Label
                                                ID="lblStatus"
                                                runat="server"
                                                Text="—" />
                                        </p>

                                    </div>

                                </div>

                            </div>

                        </div>


                        <div class="col-lg-6">

                            <div class="dashboard-panel h-100">

                                <div class="p-4">

                                    <h3 class="dashboard-panel-title">
                                        Financial Summary
                                    </h3>

                                    <div class="mt-3">

                                        <p>
                                            <strong>Labour:</strong>
                                            R
                                            <asp:Label
                                                ID="lblLabour"
                                                runat="server"
                                                Text="0.00" />
                                        </p>

                                        <p>
                                            <strong>Materials:</strong>
                                            R
                                            <asp:Label
                                                ID="lblMaterials"
                                                runat="server"
                                                Text="0.00" />
                                        </p>

                                        <p>
                                            <strong>Transport:</strong>
                                            R
                                            <asp:Label
                                                ID="lblTransport"
                                                runat="server"
                                                Text="0.00" />
                                        </p>

                                        <p>
                                            <strong>VAT:</strong>
                                            R
                                            <asp:Label
                                                ID="lblTax"
                                                runat="server"
                                                Text="0.00" />
                                        </p>

                                        <hr />

                                        <p class="fs-5">
                                            <strong>Total:</strong>
                                            R
                                            <asp:Label
                                                ID="lblTotal"
                                                runat="server"
                                                Text="0.00" />
                                        </p>

                                    </div>

                                </div>

                            </div>

                        </div>

                    </div>

                    <!-- CUSTOMER DOCUMENTS -->
                    <div class="dashboard-panel mt-4">

                        <div class="p-4">

                            <h3 class="dashboard-panel-title">
                                Customer Documents
                            </h3>

                            <p class="dashboard-panel-subtitle">
                                View the invoice and payment receipt associated
                                with this job.
                            </p>

                            <div class="mt-3">

                                <p>
                                    <strong>Payment status:</strong>
                                    <asp:Label
                                        ID="lblDocumentPaymentStatus"
                                        runat="server"
                                        Text="—" />
                                </p>

                                <div class="d-flex gap-2 flex-wrap">

                                    <asp:Button
                                        ID="btnViewAdminInvoice"
                                        runat="server"
                                        Text="View Invoice PDF"
                                        CssClass="dashboard-btn dashboard-btn-secondary"
                                        Visible="false"
                                        CausesValidation="false"
                                        OnClick="btnViewAdminInvoice_Click" />

                                    <asp:Button
                                        ID="btnViewAdminReceipt"
                                        runat="server"
                                        Text="View Payment Receipt"
                                        CssClass="dashboard-btn"
                                        Visible="false"
                                        CausesValidation="false"
                                        OnClick="btnViewAdminReceipt_Click" />

                                    <asp:Button
                                        ID="btnCollectCash"
                                        runat="server"
                                        Text="Collect Cash Payment"
                                        CssClass="btn btn-success"
                                        Visible="false"
                                        CausesValidation="false"
                                        OnClick="btnCollectCash_Click" />

                                </div>

                            </div>

                        </div>

                    </div>

                    

                    <!-- RESCHEDULE -->
                    <div class="dashboard-panel mt-4">

                        <div class="p-4">

                            <h3 class="dashboard-panel-title">
                                Reschedule Job
                            </h3>

                            <p class="dashboard-panel-subtitle">
                                Choose a new promise date and technician.
                                The system will reject technician conflicts.
                            </p>


                            <div class="row g-3 mt-2">

                                <div class="col-md-5">

                                    <label
                                        for="<%= txtNewPromiseDate.ClientID %>"
                                        class="form-label">
                                        New Promise Date
                                    </label>

                                    <asp:TextBox
                                        ID="txtNewPromiseDate"
                                        runat="server"
                                        TextMode="Date"
                                        CssClass="form-control"
                                        AutoPostBack="true"
                                        OnTextChanged="txtNewPromiseDate_TextChanged" />

                                </div>


                                <div class="col-md-7">

                                    <label
                                        for="<%= ddlTechnician.ClientID %>"
                                        class="form-label">
                                        Technician
                                    </label>

                                    <asp:DropDownList
                                        ID="ddlTechnician"
                                        runat="server"
                                        CssClass="form-select">
                                    </asp:DropDownList>

                                </div>

                            </div>


                            <div class="d-flex gap-2 mt-3">

                                <asp:Button
                                    ID="btnReschedule"
                                    runat="server"
                                    Text="Reschedule Job"
                                    CssClass="dashboard-btn"
                                    OnClick="btnReschedule_Click" />

                            </div>

                        </div>

                    </div>


                    <!-- CANCELLATION -->
                    <div class="dashboard-panel mt-4">

                        <div class="p-4">

                            <h3 class="dashboard-panel-title">
                                Cancel Job
                            </h3>

                            <p class="dashboard-panel-subtitle">
                                Record the customer's request. A manager reviews it and unused materials return to stock on approval.
                            </p>


                            <asp:Panel
                                ID="pnlLateCancellation"
                                runat="server"
                                CssClass="alert alert-warning mt-3"
                                Visible="false">

                                If approved, this request carries a fee of 5% of the original total. Materials already used can also be charged.

                            </asp:Panel>


                            <asp:Button
                                ID="btnCancelJob"
                                runat="server"
                                Text="Request cancellation"
                                CssClass="dashboard-btn dashboard-btn-danger mt-3"
                                OnClick="btnCancelJob_Click" />

                        </div>

                    </div>

                </div>

            </section>

        
</div>
</ContentTemplate>
                
        <Triggers>

    <asp:PostBackTrigger
        ControlID="btnViewAdminInvoice" />

    <asp:PostBackTrigger
        ControlID="btnViewAdminReceipt" />

    <asp:PostBackTrigger
        ControlID="btnCollectCash" />

</Triggers>

    </asp:UpdatePanel>
    


</div>
</asp:Content>
