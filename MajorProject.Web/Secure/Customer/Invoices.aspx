
<%@ Page Title="Invoices"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Invoices.aspx.cs"
    Inherits="MajorProject.Web.Secure.Customer.Invoices" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-customer-invoices">





<div class="dashboard-page invoice-page">

    <!-- PAGE HEADER -->

    <div class="dashboard-page-header">
        <div>
            <div class="dashboard-eyebrow">CUSTOMER PORTAL</div>

            <h1 class="dashboard-page-title">Invoices</h1>

            <p class="dashboard-page-subtitle">
                Manage your service invoices, payments and receipts.
                <a href="CancellationFees.aspx?view=fees">View and pay booking change invoices</a>
            </p>
        </div>
    </div>


    <!-- SUMMARY CARDS -->

    <section class="dashboard-stats invoice-summary">

        <div class="dashboard-stat-card">
            <div class="dashboard-stat-icon">
                <i class="bi bi-receipt"></i>
            </div>

            <div class="dashboard-stat-content">
                <div class="dashboard-stat-label">Invoices</div>

                <div class="dashboard-stat-value">
                    <asp:Label ID="lblInvoiceCount"
                        runat="server"
                        Text="0" />
                </div>

                <div class="dashboard-stat-meta">
                    Generated invoices
                </div>
            </div>
        </div>


        <div class="dashboard-stat-card">
            <div class="dashboard-stat-icon">
                <i class="bi bi-cash-stack"></i>
            </div>

            <div class="dashboard-stat-content">
                <div class="dashboard-stat-label">Total Billed</div>

                <div class="dashboard-stat-value">
                    <asp:Label ID="lblTotalBilled"
                        runat="server"
                        Text="R 0.00" />
                </div>

                <div class="dashboard-stat-meta">
                    Total invoice value
                </div>
            </div>
        </div>


        <div class="dashboard-stat-card">
            <div class="dashboard-stat-icon">
                <i class="bi bi-wallet2"></i>
            </div>

            <div class="dashboard-stat-content">
                <div class="dashboard-stat-label">Outstanding</div>

                <div class="dashboard-stat-value">
                    <asp:Label ID="lblOutstandingBalance"
                        runat="server"
                        Text="R 0.00" />
                </div>

                <div class="dashboard-stat-meta">
                    Amount awaiting payment
                </div>
            </div>
        </div>

    </section>


    <!-- MESSAGE -->

    <asp:Panel ID="pnlMessage"
        runat="server"
        Visible="false"
        CssClass="dashboard-alert">

        <i class="bi bi-info-circle"></i>

        <asp:Label ID="lblMessage" runat="server" />

    </asp:Panel>


    <!-- MAIN INVOICE PANEL -->

    <asp:Panel ID="feeSection" runat="server" CssClass="dashboard-panel" Visible="false">
        <h2 class="dashboard-panel-title">Booking change invoices</h2>
        <div class="dashboard-table-wrapper">
            <asp:GridView ID="feeInvoices" runat="server" CssClass="dashboard-table" AutoGenerateColumns="false">
                <Columns>
                    <asp:BoundField DataField="JobID" HeaderText="Job" />
                    <asp:BoundField DataField="FeeReason" HeaderText="Reason for charge" />
                    <asp:BoundField DataField="Fee" HeaderText="Total (R)" DataFormatString="{0:N2}" />
                    <asp:TemplateField HeaderText="Payment"><ItemTemplate>
                        <asp:HyperLink runat="server" Text="Pay invoice (demo)" Visible='<%# (string)Eval("FeePaymentStatus") == "Unpaid" %>'
                            NavigateUrl='<%# "CancellationFees.aspx?view=fees#fee-" + Eval("RequestID") %>' />
                        <asp:Literal runat="server" Text="Paid (demo)" Visible='<%# (string)Eval("FeePaymentStatus") == "DemoPaid" %>' />
                    </ItemTemplate></asp:TemplateField>
                    <asp:TemplateField HeaderText="Invoice"><ItemTemplate>
                        <asp:HyperLink runat="server" Text="View PDF" NavigateUrl='<%# "InvoicePdf.aspx?fee=" + Eval("RequestID") %>' />
                    </ItemTemplate></asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </asp:Panel>

    <section class="dashboard-panel dashboard-panel-large" data-ui-tabs="invoice-views" data-prefer-tab="selected">

        <!-- MAIN TABS -->

        <div class="invoice-tabs" role="tablist">

            <button type="button"
                id="tabHistory" data-ui-tab="history"
                class="invoice-tab active"
                role="tab"
                aria-controls="invoiceHistoryTab"
                aria-selected="true"
                onclick="showInvoiceTab('history')">

                <i class="bi bi-list-ul"></i>
                Invoice History

            </button>


            <button type="button"
                id="tabSelected" data-ui-tab="selected"
                class="invoice-tab"
                role="tab"
                aria-controls="<%= pnlSelectedInvoice.ClientID %>"
                aria-selected="false"
                onclick="showInvoiceTab('selected')">

                <i class="bi bi-file-earmark-text"></i>
                Selected Invoice

            </button>

        </div>


        <!-- TAB 1: INVOICE HISTORY -->

        <div id="invoiceHistoryTab" data-ui-pane="history"
            class="invoice-tab-content active"
            role="tabpanel"
            aria-labelledby="tabHistory">

            <div class="dashboard-panel-header">
                <div>
                    <h2 class="dashboard-panel-title">
                        Invoice History
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Select an invoice to view its details or make a payment.
                    </p>
                </div>
            </div>


            <!-- INVOICE GRID -->

            <asp:Panel ID="pnlInvoices"
                runat="server"
                CssClass="dashboard-table-wrapper invoice-table-wrapper">

                <asp:GridView ID="gvInvoices"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="dashboard-table invoice-table"
                    GridLines="None"
                    AllowPaging="true"
                    PageSize="10"
                    EmptyDataText="No invoices are available."
                    OnPageIndexChanging="gvInvoices_PageIndexChanging"
                    OnRowCommand="gvInvoices_RowCommand">

                    <Columns>

                        <asp:TemplateField HeaderText="Invoice">
                            <ItemTemplate>
                                <strong>
                                    <%# GetInvoiceNumber(Eval("JobID")) %>
                                </strong>
                            </ItemTemplate>
                        </asp:TemplateField>


                        <asp:BoundField
                            DataField="JobTypeName"
                            HeaderText="Service" />


                        <asp:TemplateField HeaderText="Service Date">
                            <ItemTemplate>
                                <%# FormatDate(Eval("PromiseDate")) %>
                            </ItemTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField HeaderText="Total">
                            <HeaderStyle HorizontalAlign="Right" />
                            <ItemStyle CssClass="invoice-amount" />

                            <ItemTemplate>
                                <%# FormatCurrency(Eval("GrandTotal")) %>
                            </ItemTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField HeaderText="Payment Status">
                            <ItemTemplate>
                                <span class='<%#
                                    GetPaymentStatusClass(
                                        Eval("PaymentStatus"))
                                %>'>
                                    <%# GetDisplayPaymentStatus(
                                        Eval("PaymentStatus"))
                                    %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>


                        <asp:TemplateField HeaderText="Action">
                            <ItemTemplate>

                                <div class="invoice-actions">

                                    <asp:HyperLink
                                        ID="btnRowPayInvoice"
                                        runat="server"
                                        Visible='<%# CanPayInvoice(
                                            Eval("PaymentStatus"),
                                            Eval("JobStatus"),
                                            Eval("InvoiceFilePath"),
                                            Eval("GrandTotal"))
                                        %>'
                                        NavigateUrl='<%# ResolveUrl(
                                            "~/Secure/Customer/PayInvoice.aspx?jobID="
                                        ) + Eval("JobID") %>'
                                        CssClass="dashboard-btn dashboard-btn-primary">

                                        <i class="bi bi-credit-card"></i>
                                        Pay

                                    </asp:HyperLink>


                                    <asp:LinkButton
                                        ID="btnViewInvoice"
                                        runat="server"
                                        CommandName="ViewInvoice"
                                        CommandArgument='<%# Eval("JobID") %>'
                                        CssClass="dashboard-table-action invoice-view-action invoice-row-view"
                                        ToolTip="View invoice"
                                        aria-label="View invoice details">

                                        <i class="bi bi-chevron-right"></i>

                                    </asp:LinkButton>

                                </div>

                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>

                    <PagerStyle CssClass="dashboard-pagination" />

                </asp:GridView>

            </asp:Panel>


            <!-- EMPTY STATE -->

            <asp:Panel ID="pnlEmpty"
                runat="server"
                Visible="false"
                CssClass="dashboard-empty-state">

                <div class="dashboard-feature-icon">
                    <i class="bi bi-receipt"></i>
                </div>

                <h3 class="dashboard-feature-title">
                    No service invoices yet
                </h3>

                <p class="dashboard-feature-text">
                    Generated invoices for your completed services will appear here.
                </p>

                <a href="<%= ResolveUrl("~/Secure/Customer/MyJobs.aspx") %>"
                    class="dashboard-btn dashboard-btn-secondary">

                    <i class="bi bi-briefcase"></i>
                    View My Jobs

                </a>

            </asp:Panel>

        </div>


        <!-- TAB 2: SELECTED INVOICE -->

        <asp:Panel ID="pnlSelectedInvoice" data-ui-pane="selected"
            runat="server"
            Visible="false"
            CssClass="invoice-tab-content"
            role="tabpanel"
            aria-labelledby="tabSelected">

            <a href="javascript:void(0)"
                onclick="showInvoiceTab('history')"
                class="invoice-back">

                <i class="bi bi-arrow-left"></i>
                Back to invoice history

            </a>


            <!-- SELECTED INVOICE HEADER -->

            <div class="dashboard-panel-header">

                <div>
                    <div class="dashboard-eyebrow">
                        INVOICE DETAILS
                    </div>

                    <h2 class="dashboard-panel-title">
                        <asp:Label ID="lblSelectedInvoiceNumber"
                            runat="server" />
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Review your invoice, payment status and receipt.
                    </p>
                </div>


                <div class="invoice-actions">

                    <asp:LinkButton
                        ID="btnPayInvoice"
                        runat="server"
                        Visible="false"
                        CssClass="dashboard-btn dashboard-btn-primary"
                        OnClick="btnPayInvoice_Click">

                        <i class="bi bi-credit-card"></i>
                        Pay Invoice

                    </asp:LinkButton>


                    <asp:LinkButton
                        ID="btnDownloadInvoice"
                        runat="server"
                        CssClass="dashboard-btn dashboard-btn-secondary"
                        OnClick="btnDownloadInvoice_Click">

                        <i class="bi bi-download"></i>
                        Download PDF

                    </asp:LinkButton>

                </div>

            </div>


            <!-- SELECTED INVOICE SUMMARY -->

            <div class="invoice-detail-summary">

                <div class="invoice-detail-card">
                    <div class="invoice-detail-label">Service</div>

                    <asp:Label ID="lblSelectedService"
                        runat="server"
                        CssClass="invoice-detail-value" />
                </div>


                <div class="invoice-detail-card">
                    <div class="invoice-detail-label">Service Date</div>

                    <asp:Label ID="lblSelectedDate"
                        runat="server"
                        CssClass="invoice-detail-value" />
                </div>


                <div class="invoice-detail-card">
                    <div class="invoice-detail-label">Total</div>

                    <asp:Label ID="lblSelectedTotal"
                        runat="server"
                        CssClass="invoice-detail-value" />
                </div>

            </div>


            <!-- PAYMENT INFORMATION -->

            <div class="invoice-information">

                <i class="bi bi-wallet2"></i>

                <div class="invoice-information-content">

                    <h3 class="invoice-detail-label">
                        Payment Status
                    </h3>

                    <asp:Label ID="lblSelectedPaymentStatus"
                        runat="server"
                        CssClass="invoice-detail-value" />

                    <asp:Label ID="lblPaymentReceipt"
                        runat="server"
                        Visible="false"
                        CssClass="dashboard-feature-text" />

                </div>

            </div>


<section class="booking-saved-services"><h3 class="dashboard-panel-title">Services in this booking</h3><asp:GridView ID="gvInvoiceServices" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="dashboard-table booking-service-breakdown" EmptyDataText="Service details are not available."><Columns><asp:BoundField DataField="JobTypeName" HeaderText="Service" HtmlEncode="true" /><asp:BoundField DataField="Quantity" HeaderText="Quantity" /></Columns></asp:GridView></section>
            <!-- INNER DOCUMENT TABS -->

            <div class="invoice-section-spacing" data-ui-tabs="invoice-documents">

                <div class="invoice-tabs" role="tablist">

                    <button type="button"
                        id="tabBreakdown" data-ui-tab="breakdown"
                        class="invoice-tab active"
                        role="tab"
                        aria-controls="invoiceBreakdownTab"
                        aria-selected="true"
                        onclick="showInvoiceDocumentTab('breakdown')">

                        <i class="bi bi-calculator"></i>
                        Financial Breakdown

                    </button>


                    <button type="button"
                        id="tabPdf" data-ui-tab="pdf"
                        class="invoice-tab"
                        role="tab"
                        aria-controls="invoicePdfTab"
                        aria-selected="false"
                        onclick="showInvoiceDocumentTab('pdf')">

                        <i class="bi bi-file-pdf"></i>
                        PDF Document

                    </button>


                    <!-- PAYMENT RECEIPT TAB -->

                    <button type="button"
                        id="tabReceipt" data-ui-tab="receipt"
                        runat="server"
                        visible="false"
                        class="invoice-tab"
                        role="tab"
                        aria-controls="<%= pnlReceiptTabContent.ClientID %>"
                        aria-selected="false"
                        onclick="showInvoiceDocumentTab('receipt')">

                        <i class="bi bi-receipt-cutoff"></i>
                        Payment Receipt

                    </button>

                </div>


                <!-- FINANCIAL BREAKDOWN TAB -->

                <div id="invoiceBreakdownTab" data-ui-pane="breakdown"
                    class="invoice-tab-content active"
                    role="tabpanel"
                    aria-labelledby="tabBreakdown">

                    <div class="dashboard-panel-header">
                        <div>
                            <h3 class="dashboard-panel-title">
                                Financial Breakdown
                            </h3>

                            <p class="dashboard-panel-subtitle">
                                Detailed cost components for your service.
                            </p>
                        </div>
                    </div>


                    <div class="dashboard-table-wrapper invoice-breakdown">

                        <table class="dashboard-table">
                            <tbody>

                                <tr>
                                    <td>Labour</td>

                                    <td class="invoice-amount">
                                        <asp:Label ID="lblLabourCost"
                                            runat="server" />
                                    </td>
                                </tr>


                                <tr>
                                    <td>Transport</td>

                                    <td class="invoice-amount">
                                        <asp:Label ID="lblTransportCost"
                                            runat="server" />
                                    </td>
                                </tr>


                                <tr>
                                    <td>Materials</td>

                                    <td class="invoice-amount">
                                        <asp:Label ID="lblMaterialCost"
                                            runat="server" />
                                    </td>
                                </tr>


                                <tr>
                                    <td>Tax</td>

                                    <td class="invoice-amount">
                                        <asp:Label ID="lblTaxAmount"
                                            runat="server" />
                                    </td>
                                </tr>


                                <tr class="invoice-grand-total">
                                    <th>GRAND TOTAL</th>

                                    <th>
                                        <asp:Label ID="lblGrandTotal"
                                            runat="server" />
                                    </th>
                                </tr>

                            </tbody>
                        </table>

                    </div>


                    <!-- EMAIL INFORMATION -->

                    <div class="invoice-information">

                        <i class="bi bi-envelope-check"></i>

                        <div class="invoice-information-content">

                            <h3 class="invoice-detail-label">
                                Invoice Email
                            </h3>

                            <p class="dashboard-feature-text">
                                <asp:Label ID="lblEmailStatus"
                                    runat="server" />
                            </p>

                        </div>

                    </div>

                </div>


                <!-- PDF DOCUMENT TAB -->

                <div id="invoicePdfTab" data-ui-pane="pdf"
                    class="invoice-tab-content"
                    role="tabpanel"
                    aria-labelledby="tabPdf">

                    <div class="dashboard-panel-header">
                        <div>
                            <h3 class="dashboard-panel-title">
                                Invoice Document
                            </h3>

                            <p class="dashboard-panel-subtitle">
                                Preview your generated invoice.
                            </p>
                        </div>
                    </div>


                    <asp:Panel ID="pnlPdfPreview"
                        runat="server">

                        <iframe
                            id="invoicePdfFrame"
                            runat="server"
                            title="Invoice PDF preview"
                            class="invoice-pdf-frame">
                        </iframe>

                    </asp:Panel>


                    <asp:Panel ID="pnlPdfUnavailable"
                        runat="server"
                        Visible="false"
                        CssClass="dashboard-empty-state">

                        <div class="dashboard-feature-icon">
                            <i class="bi bi-file-earmark-x"></i>
                        </div>

                        <h3 class="dashboard-feature-title">
                            PDF unavailable
                        </h3>

                        <p class="dashboard-feature-text">
                            Your invoice details are available, but the PDF could not be loaded.
                        </p>

                    </asp:Panel>

                </div>


                <!-- PAYMENT RECEIPT TAB -->

                <asp:Panel ID="pnlReceiptTabContent" data-ui-pane="receipt"
                    runat="server"
                    Visible="false"
                    CssClass="invoice-tab-content"
                    role="tabpanel"
                    aria-labelledby="tabReceipt">

                    <div class="dashboard-panel-header">
                        <div>
                            <h3 class="dashboard-panel-title">
                                Payment Receipt
                            </h3>

                            <p class="dashboard-panel-subtitle">
                                View your confirmation of successful payment.
                            </p>
                        </div>

                        <asp:HyperLink
                            ID="lnkDownloadReceipt"
                            runat="server"
                            CssClass="dashboard-btn dashboard-btn-secondary">

                            <i class="bi bi-download"></i>
                            Download Receipt

                        </asp:HyperLink>
                    </div>


                    <asp:Panel ID="pnlReceiptPreview"
                        runat="server">

                        <iframe
                            id="receiptPdfFrame"
                            runat="server"
                            title="Payment receipt PDF preview"
                            class="invoice-pdf-frame">
                        </iframe>

                    </asp:Panel>

                </asp:Panel>

            </div>

        </asp:Panel>

    </section>

</div>


<!-- TAB JAVASCRIPT -->

<script type="text/javascript">
function showInvoiceTab(key) { if (window.ThyWillUI) window.ThyWillUI.activateTab("invoice-views", key); }
function showInvoiceDocumentTab(key) { if (window.ThyWillUI) window.ThyWillUI.activateTab("invoice-documents", key); }
</script>


</div>

</div>
</asp:Content>
