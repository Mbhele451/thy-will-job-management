<%@ Page Title="Book a service" Language="C#" MasterPageFile="~/MasterPages/Dashboard.Master" AutoEventWireup="true" Async="true" CodeBehind="BookJob.aspx.cs" Inherits="MajorProject.Web.Secure.Customer.BookJob" %>
<asp:Content ID="MainContent" ContentPlaceHolderID="MainContent" runat="server">
<asp:UpdatePanel ID="upBooking" runat="server" UpdateMode="Conditional"><ContentTemplate>
<div class="tw-page-stage" data-page-stage>
    <div class="booking-page booking-cart-page">
        <header class="dashboard-page-header"><div><p class="dashboard-eyebrow">CUSTOMER WORKSPACE / BOOKING</p><h1 class="booking-title">One booking. All the work you need.</h1><p class="booking-subtitle">Choose your services, review your cart, then select a date for the booking.</p></div></header>
        <asp:Panel ID="pnlMessage" runat="server" Visible="false" CssClass="booking-message" role="alert" TabIndex="-1"><asp:Label ID="lblMessage" runat="server" /></asp:Panel>
        <asp:ValidationSummary ID="vsBooking" runat="server" ValidationGroup="Booking" CssClass="booking-validation-summary" HeaderText="Review your booking:" DisplayMode="BulletList" />
        <div class="booking-layout">
            <div class="booking-fields">
                <section class="booking-section">
                    <div class="booking-section-heading"><span class="booking-section-number">01</span><div><h2>Select your services</h2><p>Select several services, then add the selection to your cart.</p></div></div>
                    <asp:PlaceHolder ID="phServiceTypeCards" runat="server" />
                    <asp:CustomValidator ID="cvServiceType" runat="server" ValidationGroup="CartSelection" CssClass="booking-field-error" ErrorMessage="Select at least one service to add." OnServerValidate="cvServiceType_ServerValidate" EnableClientScript="false" Display="Dynamic" />
                    <asp:Button ID="btnAddSelected" runat="server" Text="Add selected services" CssClass="dashboard-btn dashboard-btn-primary booking-add-selected" ValidationGroup="CartSelection" OnClick="btnAddSelected_Click" />
                    <p class="booking-field-hint">Each service starts with a quantity of one. Adjust quantities in your cart. Adding an existing service increases its quantity.</p>
                </section>
                <section class="booking-section">
                    <div class="booking-section-heading"><span class="booking-section-number">02</span><div><h2>Describe the work</h2><p>Share the details for this booking.</p></div></div>
                    <div class="booking-field"><label for="<%= txtDescription.ClientID %>" class="booking-label">Work description <span class="booking-required">*</span></label>
                        <asp:TextBox ID="txtDescription" runat="server" CssClass="booking-input booking-description" TextMode="MultiLine" Rows="4" MaxLength="230" placeholder="Describe the work, where it is needed and any access details." />
                        <p class="booking-field-hint">Up to 230 characters for the whole booking.</p>
                        <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" ValidationGroup="Booking" CssClass="booking-field-error" ErrorMessage="Describe the work for your booking." Display="Dynamic" />
                    </div>
                </section>
                <section class="booking-section">
                    <div class="booking-section-heading"><span class="booking-section-number">03</span><div><h2>Location &amp; date</h2><p>All services in this booking share one location and preferred date.</p></div></div>
                    <div class="address-card"><div class="address-card-main"><asp:Literal ID="litAddress" runat="server" /></div><div class="address-card-meta"><asp:Literal ID="litAreaCityProvince" runat="server" /></div></div>
                    <asp:TextBox ID="txtAddress" runat="server" style="display:none" ReadOnly="true" />
                    <asp:TextBox ID="txtArea" runat="server" style="display:none" ReadOnly="true" />
                    <asp:TextBox ID="txtCity" runat="server" style="display:none" ReadOnly="true" />
                    <asp:TextBox ID="txtProvince" runat="server" style="display:none" ReadOnly="true" />
                    <p class="address-note">Need another address? <a href="<%= ResolveUrl("~/Secure/Customer/Profile.aspx") %>">Update your profile</a> before submitting.</p>
                    <div class="booking-field"><label for="<%= txtPromiseDate.ClientID %>" class="booking-label">Preferred date <span class="booking-required">*</span></label>
                        <asp:TextBox ID="txtPromiseDate" runat="server" CssClass="booking-input booking-date-input" TextMode="Date" />
                        <p class="booking-field-hint">Today or a future date, subject to technician availability.</p>
                        <asp:RequiredFieldValidator ID="rfvPromiseDate" runat="server" ControlToValidate="txtPromiseDate" ValidationGroup="Booking" CssClass="booking-field-error" ErrorMessage="Select a preferred service date." Display="Dynamic" />
                    </div>
                </section>
            </div>
            <asp:Panel ID="pnlCart" runat="server" CssClass="booking-cart" TabIndex="-1" role="region" aria-label="Your booking cart">
                <div class="booking-cart-heading"><p class="dashboard-eyebrow">REVIEW YOUR BOOKING</p><h2>Your cart</h2><p role="status" aria-live="polite"><asp:Label ID="lblCartCount" runat="server" Text="0" /> services <span aria-hidden="true">/</span> <asp:Label ID="lblCartUnits" runat="server" Text="0" /> units</p></div>
                <asp:GridView ID="gvBookingCart" runat="server" AutoGenerateColumns="false" DataKeyNames="JobTypeID" ShowHeader="false" GridLines="None" CssClass="booking-cart-list" OnRowCommand="gvBookingCart_RowCommand">
                    <Columns><asp:TemplateField><ItemTemplate>
                        <div class="booking-cart-line"><h3><asp:Literal ID="litCartServiceName" runat="server" Mode="Encode" Text='<%# Eval("ServiceName") %>' /></h3><div class="booking-cart-line-controls">
                            <label class="booking-cart-quantity-label">Quantity <asp:TextBox ID="txtCartQuantity" runat="server" TextMode="Number" min="1" max="1000" step="1" CssClass="booking-cart-quantity" aria-label='<%# "Quantity for " + Eval("ServiceName") %>' Text='<%# Eval("Quantity") %>' /></label>
                            <asp:LinkButton ID="btnUpdateCartLine" runat="server" CommandName="SetQuantity" CommandArgument='<%# Eval("JobTypeID") %>' CausesValidation="false" aria-label='<%# "Update quantity for " + Eval("ServiceName") %>' CssClass="cart-line-action">Update</asp:LinkButton>
                            <asp:LinkButton ID="btnRemoveCartLine" runat="server" CommandName="RemoveLine" CommandArgument='<%# Eval("JobTypeID") %>' CausesValidation="false" aria-label='<%# "Remove " + Eval("ServiceName") %>' CssClass="cart-line-action cart-line-remove">Remove</asp:LinkButton>
                        </div></div>
                    </ItemTemplate></asp:TemplateField></Columns>
                    <EmptyDataTemplate><div class="booking-cart-empty"><strong>Your cart is empty.</strong><p>Select services on the left and choose Add selected services.</p></div></EmptyDataTemplate>
                </asp:GridView>
                <asp:CustomValidator ID="cvCart" runat="server" ValidationGroup="Booking" CssClass="booking-field-error" ErrorMessage="Add at least one service to your cart." OnServerValidate="cvCart_ServerValidate" EnableClientScript="false" Display="Dynamic" />
                <p class="booking-cart-note">One booking reference and invoice cover the selected services. Final charges and availability are confirmed when the booking is saved.</p>
                <asp:Button ID="btnSubmit" runat="server" Text="Submit booking" CssClass="booking-submit" ValidationGroup="Booking" OnClick="btnSubmit_Click" data-booking-submit="true" />
                <asp:HyperLink ID="lnkCancel" runat="server" NavigateUrl="~/Secure/Customer/Dashboard.aspx" CssClass="booking-cancel">Back to dashboard</asp:HyperLink>
            </asp:Panel>
        </div>
    </div>
</div>
</ContentTemplate></asp:UpdatePanel>
</asp:Content>
