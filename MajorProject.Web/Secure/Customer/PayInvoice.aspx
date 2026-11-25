<%@ Page Title="Pay Invoice"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="PayInvoice.aspx.cs"
    Inherits="MajorProject.Web.Secure.Customer.PayInvoice" %>

<asp:Content ID="PaymentHead" ClientIDMode="Static" ContentPlaceHolderID="HeadContent" runat="server">
    <link href="<%= ResolveUrl("~/Content/css/payments.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content1" ClientIDMode="Static"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-customer-payinvoice">




<script src="<%= ResolveUrl("~/Content/js/payments.js") %>"></script>

<div class="dashboard-page payment-page">

    <div class="payment-page-header">

        <asp:Panel ID="pnlUnpaidHeader" ClientIDMode="Static" runat="server">
            <div class="payment-eyebrow">CUSTOMER ACCOUNT</div>
            <h1>Complete Your Payment</h1>
            <p>Review your invoice and choose a payment method. Online checkout is simulated; no money is transferred. Use test details only.</p>
        </asp:Panel>

        <asp:Panel ID="pnlPaidHeader" ClientIDMode="Static" runat="server" Visible="false">
            <div class="payment-eyebrow">PAYMENT COMPLETED</div>
            <h1>Payment Confirmation</h1>
            <p>Your payment record and receipt are available below.</p>
        </asp:Panel>

        <div class="payment-security-note">
            <i class="bi bi-shield-check"></i>
            Simulated online checkout
        </div>

    </div>

    <asp:Panel ID="pnlMessage" ClientIDMode="Static" runat="server" Visible="false">
        <asp:Label ID="lblMessage" ClientIDMode="Static"
            runat="server"
            CssClass="payment-error-message" />
    </asp:Panel>

    <asp:Panel ID="pnlInvoice" ClientIDMode="Static" runat="server">

        <!-- UNPAID INVOICE OVERVIEW -->

        <asp:Panel ID="pnlUnpaidOverview" ClientIDMode="Static" runat="server">

            <div class="payment-overview">

                <section class="payment-invoice-card">

                    <div class="payment-invoice-heading">
                        <div>
                            <div class="payment-eyebrow">INVOICE SUMMARY</div>
                            <h2>Invoice details</h2>
                            <p>Review your invoice reference and payment status.</p>
                        </div>

                        <div class="payment-invoice-icon">
                            <i class="bi bi-receipt"></i>
                        </div>
                    </div>

                    <hr class="payment-invoice-divider" />

                    <div class="payment-invoice-details">

                        <div>
                            <span class="payment-detail-label">Invoice Reference</span>
                            <span class="payment-detail-value">
                                <asp:Label ID="lblInvoiceReference" ClientIDMode="Static" runat="server" />
                            </span>
                        </div>

                        <div>
                            <span class="payment-detail-label">Payment Status</span>
                            <span class="payment-status-value">
                                <asp:Label ID="lblPaymentStatus" ClientIDMode="Static"
                                    runat="server"
                                    Text="Unpaid" />
                            </span>
                        </div>

                    </div>

                </section>

                <section class="payment-amount-card">

                    <div>
                        <div class="payment-amount-label">Total Amount Due</div>

                        <asp:Label ID="lblAmountDue" ClientIDMode="Static"
                            runat="server"
                            CssClass="payment-amount" />

                        <div class="payment-amount-caption">
                            Outstanding invoice amount
                        </div>
                    </div>

                    <div class="payment-amount-footer">
                        <i class="bi bi-shield-check"></i>
                        Amount retrieved from your invoice
                    </div>

                </section>

            </div>

        </asp:Panel>

        <!-- PAID INVOICE OVERVIEW -->

        <asp:Panel ID="pnlPaidOverview" ClientIDMode="Static"
            runat="server"
            Visible="false">

            <div class="payment-paid-overview">

                <div>
                    <span class="payment-detail-label">Invoice Reference</span>

                    <span class="payment-detail-value">
                        <asp:Label ID="lblPaidInvoiceReference" ClientIDMode="Static" runat="server" />
                    </span>

                    <div style="margin-top: 10px;">
                        <span class="payment-status-paid">Paid</span>
                    </div>
                </div>

                <div class="payment-paid-amount">
                    <div class="payment-amount-label">Total Amount Paid</div>

                    <asp:Label ID="lblPaidInvoiceAmount" ClientIDMode="Static"
                        runat="server"
                        CssClass="payment-amount" />
                </div>

            </div>

        </asp:Panel>

        <!-- PAYMENT METHODS -->

        <asp:Panel ID="pnlPaymentMethods" ClientIDMode="Static" runat="server">

            <section class="payment-method-section">

                <div class="payment-section-heading">
                    <h2>Choose Payment Method</h2>
                    <p>Select a payment option to continue.</p>
                </div>

                <div class="payment-method-grid">

                    <button type="button"
                        class="payment-method-card"
                        onclick="openPaymentModal('cardModal')"
                        aria-label="Choose card payment">

                        <span class="payment-method-icon">
                            <i class="bi bi-credit-card-2-front"></i>
                        </span>

                        <span class="payment-method-title">Card Payment</span>

                        <span class="payment-method-description">
                            Use a saved card or enter new card details.
                        </span>

                        <span class="payment-method-arrow">
                            <i class="bi bi-arrow-right"></i>
                        </span>

                    </button>

                    <button type="button"
                        class="payment-method-card"
                        onclick="openPaymentModal('eftModal')"
                        aria-label="Choose EFT payment">

                        <span class="payment-method-icon">
                            <i class="bi bi-bank"></i>
                        </span>

                        <span class="payment-method-title">EFT Payment</span>

                        <span class="payment-method-description">
                            Select a bank and provide the payer's name.
                        </span>

                        <span class="payment-method-arrow">
                            <i class="bi bi-arrow-right"></i>
                        </span>

                    </button>

                    <button type="button"
                        class="payment-method-card"
                        onclick="openPaymentModal('walletModal')"
                        aria-label="Choose mobile wallet payment">

                        <span class="payment-method-icon">
                            <i class="bi bi-phone"></i>
                        </span>

                        <span class="payment-method-title">Mobile Wallet</span>

                        <span class="payment-method-description">
                            Select a wallet provider and enter a mobile number.
                        </span>

                        <span class="payment-method-arrow">
                            <i class="bi bi-arrow-right"></i>
                        </span>

                    </button>

                </div>

            </section>

        </asp:Panel>

        <!-- PAYMENT SUCCESS -->

        <asp:Panel ID="pnlSuccess" ClientIDMode="Static"
            runat="server"
            Visible="false"
            CssClass="payment-success-panel">

            <div class="payment-success-heading">

                <div class="payment-success-icon">
                    <i class="bi bi-check-lg"></i>
                </div>

                <div>
                    <div class="payment-eyebrow">PAYMENT CONFIRMED</div>

                    <h2>Payment Successful</h2>

                    <p>
                        <asp:Label ID="lblReceiptMessage" ClientIDMode="Static" runat="server" />
                    </p>
                </div>

            </div>

            <div class="payment-success-detail">

                <div class="reference-detail">
                    <strong>Payment Reference</strong>

                    <div class="payment-reference-row">

                        <span class="payment-reference-text">
                            <asp:Label ID="lblPaymentReference" ClientIDMode="Static" runat="server" />
                        </span>

                        <button type="button"
                            id="copyPaymentReferenceButton"
                            class="payment-copy-button"
                            onclick="copyPaymentReference()"
                            aria-label="Copy payment reference"
                            title="Copy payment reference">

                            <i class="bi bi-copy"></i>

                        </button>

                    </div>
                </div>

                <div>
                    <strong>Payment Method</strong>
                    <p>
                        <asp:Label ID="lblSuccessfulMethod" ClientIDMode="Static" runat="server" />
                    </p>
                </div>

            </div>

            <div class="payment-receipt-footer">

                <a href="<%= ResolveUrl("~/Secure/Customer/Invoices.aspx") %>"
                    class="payment-back-link">

                    <i class="bi bi-arrow-left"></i>
                    Return to My Invoices

                </a>

                <div class="payment-receipt-actions">

                    <asp:HyperLink ID="lnkDownloadReceipt" ClientIDMode="Static"
                        runat="server"
                        CssClass="dashboard-btn dashboard-btn-secondary payment-download-button"
                        Text="Download Receipt"
                        Visible="false" />

                    <asp:HyperLink ID="lnkViewReceipt" ClientIDMode="Static"
                        runat="server"
                        CssClass="dashboard-btn dashboard-btn-primary"
                        Text="View Payment Receipt"
                        Target="_blank"
                        Visible="false" />

                </div>

            </div>

        </asp:Panel>

    </asp:Panel>

</div>

<!-- CARD PAYMENT MODAL -->
<div id="cardModal" class="payment-modal-overlay" aria-hidden="true">
 <div class="payment-modal-content checkout-card-dialog" role="dialog" aria-modal="true" aria-labelledby="cardCheckoutTitle" tabindex="-1">
  <button type="button" class="payment-modal-close" onclick="closePaymentModal('cardModal')" aria-label="Close card payment">×</button>
  <div class="payment-modal-header"><div class="payment-eyebrow">CARD CHECKOUT</div><h2 id="cardCheckoutTitle">A simple, secure next step.</h2><p>Review your card details and confirm your invoice payment.</p></div>
  <div class="card-tabs" role="tablist" aria-label="Card payment method">
   <button type="button" id="savedCardTab" class="card-tab active" role="tab" aria-selected="true" aria-controls="savedCardPanel" onclick="selectCardTab('saved')">Saved card</button>
   <button type="button" id="newCardTab" class="card-tab" role="tab" aria-selected="false" aria-controls="newCardPanel" tabindex="-1" onclick="selectCardTab('new')">New card</button>
  </div>
  <div class="checkout-error" data-payment-error role="alert" tabindex="-1" hidden></div>
  <div id="savedCardPanel" class="card-tab-panel active" role="tabpanel" aria-labelledby="savedCardTab">
   <asp:Panel ID="pnlSavedCardSection" ClientIDMode="Static" runat="server">
    <div class="checkout-grid"><aside class="checkout-card-aside">
 <div class="checkout-aside-heading"><span class="payment-eyebrow">YOUR PAYMENT CARD</span><span class="checkout-card-badge">Saved card</span></div>
 <div class="simulated-card" aria-label="Masked card preview">
  <div class="simulated-card-top"><span id="savedCardBrand" class="simulated-card-brand">CARD</span><i class="bi bi-wifi" aria-hidden="true"></i></div>
  <div class="simulated-card-chip"><i class="bi bi-cpu" aria-hidden="true"></i></div>
  <div id="savedCardNumber" class="simulated-card-number">•••• •••• •••• ••••</div>
  <div class="simulated-card-bottom"><div class="simulated-card-holder"><span class="simulated-card-label">CARDHOLDER</span><span  class="simulated-card-holder-value">REGISTERED CARD</span></div><div class="checkout-preview-expiry"><span class="simulated-card-label">EXPIRES</span><span id="savedCardExpiry" class="simulated-card-expiry">--/--</span></div></div>
 </div>
 <div class="checkout-trust"><i class="bi bi-shield-check" aria-hidden="true"></i><div><strong>Your security code stays private</strong><p>CVV is never saved. Saved cards keep only the card name, expiry and last four digits.</p></div></div>
 <p class="checkout-demo">Demo checkout. No bank charge is made.</p>
</aside>
     <section class="checkout-form"><div class="saved-card-heading"><h3>Choose your card</h3><span>Confirm the card and enter its security code.</span></div>
      <asp:DropDownList ID="ddlSavedCards" ClientIDMode="Static" runat="server" CssClass="saved-card-native-select" aria-label="Saved card selection" />
      <div id="savedCardList" class="saved-card-list"></div><div class="payment-form-group checkout-cvv-field"><label for="txtSavedCardCvv">Security code <span class="checkout-optional">(CVV)</span></label><div class="checkout-cvv-control"><asp:TextBox ID="txtSavedCardCvv" ClientIDMode="Static" runat="server" CssClass="payment-form-control" TextMode="Password" MaxLength="4" EnableViewState="false" data-visibility-ready="true" autocomplete="off" inputmode="numeric" placeholder="3 or 4 digits" aria-describedby="txtSavedCardCvvHint" /><button type="button" class="checkout-cvv-toggle" onclick="togglePaymentCvv('txtSavedCardCvv', this)" aria-controls="txtSavedCardCvv" aria-pressed="false" aria-label="Show security code">Show</button></div><div id="txtSavedCardCvvHint" class="payment-form-hint">On the back of your card, or the front for a four-digit code.</div></div>
      <div class="payment-checkout-summary"><span>Invoice total</span><strong><asp:Label ID="lblSavedCardCheckoutAmount" ClientIDMode="Static" runat="server" /></strong></div>
      <asp:Button ID="btnPaySavedCard" ClientIDMode="Static" runat="server" Text="Confirm payment" CssClass="dashboard-btn dashboard-btn-primary" OnClientClick="return validatePaymentCheckout('saved');" OnClick="btnPaySavedCard_Click" />
     </section></div>
   </asp:Panel>
  </div>
  <div id="newCardPanel" class="card-tab-panel" role="tabpanel" aria-labelledby="newCardTab" hidden>
   <div class="checkout-grid"><aside class="checkout-card-aside">
 <div class="checkout-aside-heading"><span class="payment-eyebrow">YOUR PAYMENT CARD</span><span class="checkout-card-badge">Card preview</span></div>
 <div class="simulated-card" aria-label="Masked card preview">
  <div class="simulated-card-top"><span id="newCardBrandPreview" class="simulated-card-brand">CARD</span><i class="bi bi-wifi" aria-hidden="true"></i></div>
  <div class="simulated-card-chip"><i class="bi bi-cpu" aria-hidden="true"></i></div>
  <div id="newCardNumberPreview" class="simulated-card-number">•••• •••• •••• ••••</div>
  <div class="simulated-card-bottom"><div class="simulated-card-holder"><span class="simulated-card-label">CARDHOLDER</span><span id="newCardHolderPreview" class="simulated-card-holder-value">CARDHOLDER NAME</span></div><div class="checkout-preview-expiry"><span class="simulated-card-label">EXPIRES</span><span id="newCardExpiryPreview" class="simulated-card-expiry">--/--</span></div></div>
 </div>
 <div class="checkout-trust"><i class="bi bi-shield-check" aria-hidden="true"></i><div><strong>Your security code stays private</strong><p>CVV is never saved. Saved cards keep only the card name, expiry and last four digits.</p></div></div>
 <p class="checkout-demo">Demo checkout. No bank charge is made.</p>
</aside>
    <section class="checkout-form">
     <div class="payment-form-group"><label for="txtCardholderName">Name on card</label><asp:TextBox ID="txtCardholderName" ClientIDMode="Static" runat="server" CssClass="payment-form-control" MaxLength="100" autocomplete="cc-name" placeholder="Name as shown on your card" /></div>
     <div class="payment-form-group"><label for="txtCardNumber">Card number</label><asp:TextBox ID="txtCardNumber" ClientIDMode="Static" runat="server" CssClass="payment-form-control" MaxLength="25" EnableViewState="false" autocomplete="off" inputmode="numeric" placeholder="0000 0000 0000 0000" /></div>
     <div class="payment-form-row"><div class="payment-form-group"><label for="ddlExpiryMonth">Expiry month</label><asp:DropDownList ID="ddlExpiryMonth" ClientIDMode="Static" runat="server" CssClass="payment-form-control" autocomplete="cc-exp-month"><asp:ListItem Value="">Month</asp:ListItem><asp:ListItem Value="1">01</asp:ListItem><asp:ListItem Value="2">02</asp:ListItem><asp:ListItem Value="3">03</asp:ListItem><asp:ListItem Value="4">04</asp:ListItem><asp:ListItem Value="5">05</asp:ListItem><asp:ListItem Value="6">06</asp:ListItem><asp:ListItem Value="7">07</asp:ListItem><asp:ListItem Value="8">08</asp:ListItem><asp:ListItem Value="9">09</asp:ListItem><asp:ListItem Value="10">10</asp:ListItem><asp:ListItem Value="11">11</asp:ListItem><asp:ListItem Value="12">12</asp:ListItem></asp:DropDownList></div><div class="payment-form-group"><label for="txtExpiryYear">Expiry year</label><asp:TextBox ID="txtExpiryYear" ClientIDMode="Static" runat="server" CssClass="payment-form-control" MaxLength="4" inputmode="numeric" autocomplete="cc-exp-year" placeholder="YYYY" /></div></div>
     <div class="payment-form-group checkout-cvv-field"><label for="txtNewCardCvv">Security code <span class="checkout-optional">(CVV)</span></label><div class="checkout-cvv-control"><asp:TextBox ID="txtNewCardCvv" ClientIDMode="Static" runat="server" CssClass="payment-form-control" TextMode="Password" MaxLength="4" EnableViewState="false" data-visibility-ready="true" autocomplete="off" inputmode="numeric" placeholder="3 or 4 digits" aria-describedby="txtNewCardCvvHint" /><button type="button" class="checkout-cvv-toggle" onclick="togglePaymentCvv('txtNewCardCvv', this)" aria-controls="txtNewCardCvv" aria-pressed="false" aria-label="Show security code">Show</button></div><div id="txtNewCardCvvHint" class="payment-form-hint">On the back of your card, or the front for a four-digit code.</div></div>
     <div class="payment-checkout-summary"><span>Invoice total</span><strong><asp:Label ID="lblNewCardCheckoutAmount" ClientIDMode="Static" runat="server" /></strong></div>
     <asp:Button ID="btnAddCardAndPay" ClientIDMode="Static" runat="server" Text="Save card &amp; confirm payment" CssClass="dashboard-btn dashboard-btn-primary" OnClientClick="return validatePaymentCheckout('new');" OnClick="btnAddCardAndPay_Click" />
    </section></div>
  </div>
 </div>
</div>

<!-- EFT PAYMENT MODAL -->

<div id="eftModal" class="payment-modal-overlay" aria-hidden="true">

    <div class="payment-modal-content" role="dialog" aria-modal="true" aria-label="EFT payment" tabindex="-1"><div class="checkout-error" data-payment-error role="alert" tabindex="-1" hidden></div>

        <button type="button"
            class="payment-modal-close"
            onclick="closePaymentModal('eftModal')"
            aria-label="Close EFT payment">
            ×
        </button>

        <div class="payment-modal-header">

            <div class="payment-eyebrow">EFT CHECKOUT</div>

            <h2>
                <i class="bi bi-bank"></i>
                EFT Payment
            </h2>

            <p>
                Choose a bank and enter the payer's name.
            </p>

        </div>

        <div class="payment-form-group">

            <label for="<%= ddlEFTBank.ClientID %>">
                Select Bank
            </label>

            <asp:DropDownList ID="ddlEFTBank" ClientIDMode="Static"
                runat="server"
                CssClass="payment-form-control">

                <asp:ListItem Value="">Select bank</asp:ListItem>
                <asp:ListItem Value="ABSA">ABSA</asp:ListItem>
                <asp:ListItem Value="FNB">First National Bank</asp:ListItem>
                <asp:ListItem Value="NEDBANK">Nedbank</asp:ListItem>
                <asp:ListItem Value="STANDARD">Standard Bank</asp:ListItem>
                <asp:ListItem Value="CAPITEC">Capitec</asp:ListItem>
                <asp:ListItem Value="OTHER">Other</asp:ListItem>

            </asp:DropDownList>

        </div>

        <div class="payment-form-group">

            <label for="<%= txtEFTPayerName.ClientID %>">
                Account Holder / Payer Name
            </label>

            <asp:TextBox ID="txtEFTPayerName" ClientIDMode="Static"
                runat="server"
                CssClass="payment-form-control"
                MaxLength="100" />

        </div>

        <div class="payment-checkout-summary">
            <span>Amount to authorize</span>
            <span>
                <asp:Label ID="lblEFTCheckoutAmount" ClientIDMode="Static" runat="server" />
            </span>
        </div>

        <asp:Button ID="btnPayEFT" ClientIDMode="Static"
            runat="server"
            Text="Continue with EFT"
            CssClass="dashboard-btn dashboard-btn-primary"
            OnClientClick="return validatePaymentCheckout('eft');" OnClick="btnPayEFT_Click" />

    </div>

</div>

<!-- MOBILE WALLET MODAL -->

<div id="walletModal" class="payment-modal-overlay" aria-hidden="true">

    <div class="payment-modal-content" role="dialog" aria-modal="true" aria-label="Mobile wallet payment" tabindex="-1"><div class="checkout-error" data-payment-error role="alert" tabindex="-1" hidden></div>

        <button type="button"
            class="payment-modal-close"
            onclick="closePaymentModal('walletModal')"
            aria-label="Close wallet payment">
            ×
        </button>

        <div class="payment-modal-header">

            <div class="payment-eyebrow">WALLET CHECKOUT</div>

            <h2>
                <i class="bi bi-phone"></i>
                Mobile Wallet
            </h2>

            <p>
                Choose a wallet provider and enter a mobile number.
            </p>

        </div>

        <div class="payment-form-group">

            <label for="<%= ddlWalletProvider.ClientID %>">
                Wallet Provider
            </label>

            <asp:DropDownList ID="ddlWalletProvider" ClientIDMode="Static"
                runat="server"
                CssClass="payment-form-control">

                <asp:ListItem Value="">Select provider</asp:ListItem>
                <asp:ListItem Value="MobileWallet">Mobile Wallet</asp:ListItem>
                <asp:ListItem Value="DemoWallet">Demo Wallet</asp:ListItem>

            </asp:DropDownList>

        </div>

        <div class="payment-form-group">

            <label for="<%= txtWalletMobile.ClientID %>">
                Mobile Number
            </label>

            <asp:TextBox ID="txtWalletMobile" ClientIDMode="Static"
                runat="server"
                CssClass="payment-form-control"
                MaxLength="10"
                inputmode="numeric"
                placeholder="0821234567" />

        </div>

        <div class="payment-checkout-summary">
            <span>Amount to authorize</span>
            <span>
                <asp:Label ID="lblWalletCheckoutAmount" ClientIDMode="Static" runat="server" />
            </span>
        </div>

        <asp:Button ID="btnPayWallet" ClientIDMode="Static"
            runat="server"
            Text="Continue with Wallet"
            CssClass="dashboard-btn dashboard-btn-primary"
            OnClientClick="return validatePaymentCheckout('wallet');" OnClick="btnPayWallet_Click" />

    </div>

</div>


</div>

</div>
</asp:Content>
