<%@ Page
    Title="Collect Cash Payment"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="CashPayment.aspx.cs"
    Inherits="MajorProject.Web.Secure.Staff.CashPayment" %>

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
<div class="tw-page-secure-staff-cashpayment">


    <div class="dashboard-page">

        <section class="dashboard-page-header">

            <div>

                <div class="dashboard-eyebrow">
                    PAYMENT MANAGEMENT
                </div>

                <h1 class="dashboard-page-title">
                    Collect Cash Payment
                </h1>

                <p class="dashboard-page-subtitle">
                    Record an in-person cash payment for a completed customer job.
                </p>

            </div>

        </section>


        <div class="cash-payment-container">

            <section class="dashboard-panel">

                <div class="dashboard-panel-header">

                    <div>

                        <h2 class="dashboard-panel-title">
                            Cash Collection
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            Enter the cash received from the customer.
                        </p>

                    </div>

                </div>


                <div class="p-4">

                    <asp:Label
                        ID="lblMessage"
                        runat="server"
                        Visible="false"
                        CssClass="cash-payment-message" />


                    <asp:Panel
                        ID="pnlPayment"
                        runat="server">

                        <!-- PAYMENT DETAILS -->

                        <div class="cash-payment-summary">

                            <div class="cash-payment-hero-label">
                                Amount Due
                            </div>

                            <div class="cash-payment-amount">

                                R<asp:Label
                                    ID="lblAmountDue"
                                    runat="server" />

                            </div>


                            <div class="cash-payment-meta">

                                <div class="cash-payment-meta-item">

                                    <div class="cash-payment-meta-label">
                                        Job Number
                                    </div>

                                    <div class="cash-payment-meta-value">
                                        <asp:Label
                                            ID="lblJobID"
                                            runat="server" />
                                    </div>

                                </div>


                                <div class="cash-payment-meta-item">

                                    <div class="cash-payment-meta-label">
                                        Customer ID
                                    </div>

                                    <div class="cash-payment-meta-value">
                                        <asp:Label
                                            ID="lblCustomerID"
                                            runat="server" />
                                    </div>

                                </div>

                            </div>

                        </div>


                        <!-- INITIAL CASH -->

                        <asp:Panel
                            ID="pnlInitialCash"
                            runat="server">

                            <div class="cash-payment-field">

                                <label for="<%= txtInitialCash.ClientID %>">
                                    Initial Cash Received (R)
                                </label>

                                <asp:TextBox
                                    ID="txtInitialCash"
                                    runat="server"
                                    TextMode="Number"
                                    step="0.01"
                                    min="0.01"
                                    CssClass="cash-payment-input"
                                    placeholder="Enter cash amount" />

                                <p class="cash-payment-hint">
                                    Enter the full amount the customer handed over.
                                </p>

                            </div>


                            <div class="cash-payment-actions">

                                <asp:Button
                                    ID="btnSubmitInitialCash"
                                    runat="server"
                                    Text="Submit Cash"
                                    CssClass="dashboard-btn"
                                    OnClick="btnSubmitInitialCash_Click" />

                            </div>

                        </asp:Panel>


                        <!-- ADDITIONAL CASH -->

                        <asp:Panel
                            ID="pnlAdditionalCash"
                            runat="server"
                            Visible="false">

                            <div class="cash-payment-result">

                                <div class="cash-payment-hero-label">
                                    Outstanding Balance
                                </div>

                                <div class="cash-payment-amount cash-payment-amount-danger">

                                    R<asp:Label
                                        ID="lblOutstanding"
                                        runat="server" />

                                </div>


                                <div class="cash-payment-meta">

                                    <div class="cash-payment-meta-item">

                                        <div class="cash-payment-meta-label">
                                            Total Cash Received
                                        </div>

                                        <div class="cash-payment-meta-value">
                                            R<asp:Label
                                                ID="lblTotalReceived"
                                                runat="server" />
                                        </div>

                                    </div>

                                </div>

                            </div>


                            <div class="cash-payment-field">

                                <label for="<%= txtAdditionalCash.ClientID %>">
                                    Additional Cash Received (R)
                                </label>

                                <asp:TextBox
                                    ID="txtAdditionalCash"
                                    runat="server"
                                    TextMode="Number"
                                    step="0.01"
                                    min="0.01"
                                    CssClass="cash-payment-input"
                                    placeholder="Enter additional cash" />

                                <p class="cash-payment-hint">
                                    Leave this blank and submit to reject the payment if the customer has no more cash.
                                </p>

                            </div>


                            <div class="cash-payment-actions">

                                <asp:Button
                                    ID="btnSubmitAdditionalCash"
                                    runat="server"
                                    Text="Add Cash / Confirm Collection"
                                    CssClass="dashboard-btn"
                                    OnClick="btnSubmitAdditionalCash_Click" />

                            </div>

                        </asp:Panel>


                        <!-- FINAL RESULT -->

                        <asp:Panel
                            ID="pnlFinalResult"
                            runat="server"
                            Visible="false">

                            <div class="cash-payment-result">

                                <h3 class="dashboard-panel-title">
                                    Cash Collection Result
                                </h3>

                                <div class="cash-payment-hero-label">
                                    Change Due
                                </div>

                                <div class="cash-payment-amount">

                                    R<asp:Label
                                        ID="lblChangeDue"
                                        runat="server" />

                                </div>


                                <div class="cash-payment-meta">

                                    <div class="cash-payment-meta-item">

                                        <div class="cash-payment-meta-label">
                                            Total Cash Received
                                        </div>

                                        <div class="cash-payment-meta-value">
                                            R<asp:Label
                                                ID="lblFinalReceived"
                                                runat="server" />
                                        </div>

                                    </div>

                                </div>

                            </div>

                        </asp:Panel>

                    </asp:Panel>

                </div>

            </section>

        </div>

    </div>


</div>

</div>
</asp:Content>
