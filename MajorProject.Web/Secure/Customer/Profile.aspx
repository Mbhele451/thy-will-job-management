
<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs"
    Inherits="MajorProject.Web.Secure.Customer.Profile"
    MasterPageFile="~/MasterPages/Dashboard.Master" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-customer-profile">


    

    <div class="dashboard-page">

        <!-- PAGE HEADER -->

        <div class="dashboard-page-header">

            <div>
                <div class="dashboard-eyebrow">
                    CUSTOMER ACCOUNT
                </div>

                <h1 class="dashboard-page-title">
                    My Profile
                </h1>

                <p class="dashboard-page-subtitle">
                    Manage your contact details and service information.
                </p>
            </div>

            <div class="dashboard-page-actions">

                <asp:HyperLink
                    ID="lnkBackToDashboard"
                    runat="server"
                    NavigateUrl="~/Secure/Customer/Dashboard.aspx"
                    CssClass="btn dashboard-btn-secondary">

                    <i class="bi bi-arrow-left me-2"></i>
                    Back to Dashboard

                </asp:HyperLink>

            </div>

        </div>


        <!-- SUCCESS MESSAGE -->

        <asp:Panel
            ID="pnlSuccess"
            runat="server"
            Visible="false"
            CssClass="dashboard-alert dashboard-alert-success mb-4">

            <i class="bi bi-check-circle me-2"></i>

            <asp:Label
                ID="lblSuccess"
                runat="server">
            </asp:Label>

        </asp:Panel>


        <!-- SERVER-SIDE ERROR MESSAGE -->

        <asp:Panel
            ID="pnlError"
            runat="server"
            Visible="false"
            CssClass="dashboard-alert dashboard-alert-danger mb-4">

            <i class="bi bi-exclamation-triangle me-2"></i>

            <asp:Label
                ID="lblError"
                runat="server">
            </asp:Label>

        </asp:Panel>


        <!-- PROFILE CONTENT -->

        <div class="dashboard-grid-2">

            <!-- PERSONAL INFORMATION -->

            <div class="dashboard-panel">

                <div class="dashboard-panel-header">
                    <div>
                        <div class="dashboard-eyebrow">
                            PERSONAL INFORMATION
                        </div>

                        <h2 class="dashboard-panel-title">
                            Contact Details
                        </h2>
                    </div>
                </div>

                <div class="dashboard-form">

                    <!-- FULL NAME -->

                    <div class="dashboard-form-group">

                        <label
                            for="<%= txtFullName.ClientID %>"
                            class="dashboard-form-label">
                            Full Name
                        </label>

                        <asp:TextBox
                            ID="txtFullName"
                            runat="server"
                            CssClass="form-control"
                            ReadOnly="true">
                        </asp:TextBox>

                        <div class="dashboard-form-help">
                            Your registered name cannot be changed from this page.
                        </div>

                    </div>


                    <!-- PHONE NUMBER -->

                    <div class="dashboard-form-group">

                        <label
                            for="<%= txtPhoneNumber.ClientID %>"
                            class="dashboard-form-label">
                            Phone Number
                        </label>

                        <asp:TextBox
                            ID="txtPhoneNumber"
                            runat="server"
                            CssClass="form-control"
                            MaxLength="10"
                            placeholder="e.g. 0821234567">
                        </asp:TextBox>

                        <div
                            id="profilePhoneError"
                            class="profile-field-error"
                            aria-live="polite">
                        </div>

                    </div>


                    <!-- EMAIL ADDRESS -->

                    <div class="dashboard-form-group">

                        <label
                            for="<%= txtEmailAddress.ClientID %>"
                            class="dashboard-form-label">
                            Email Address
                        </label>

                        <asp:TextBox
                            ID="txtEmailAddress"
                            runat="server"
                            CssClass="form-control"
                            TextMode="Email"
                            MaxLength="100"
                            placeholder="your@email.com">
                        </asp:TextBox>

                        <div
                            id="profileEmailError"
                            class="profile-field-error"
                            aria-live="polite">
                        </div>

                        <div class="dashboard-form-help">
                            This email address is also used as your customer login.
                        </div>

                    </div>


                    <!-- PHYSICAL ADDRESS -->

                    <div class="dashboard-form-group">

                        <label
                            for="<%= txtAddress.ClientID %>"
                            class="dashboard-form-label">
                            Physical Address
                        </label>

                        <asp:TextBox
                            ID="txtAddress"
                            runat="server"
                            CssClass="form-control"
                            TextMode="MultiLine"
                            Rows="4"
                            MaxLength="255"
                            placeholder="Enter your physical service address">
                        </asp:TextBox>

                        <div
                            id="profileAddressError"
                            class="profile-field-error"
                            aria-live="polite">
                        </div>

                    </div>


                    <!-- SERVICE AREA -->

                    <div class="dashboard-form-group">

                        <label
                            for="<%= ddlArea.ClientID %>"
                            class="dashboard-form-label">
                            Service Area
                        </label>

                        <asp:DropDownList
                            ID="ddlArea"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                        <div
                            id="profileAreaError"
                            class="profile-field-error"
                            aria-live="polite">
                        </div>

                        <div class="dashboard-form-help">
                            Select the area where you normally require THY WILL services.
                        </div>

                    </div>


                    <!-- SAVE PROFILE -->

                    <div class="dashboard-form-actions">

                        <asp:Button
                            ID="btnSaveProfile"
                    data-profile-save="true"
                            runat="server"
                            Text="Save Changes"
                            CssClass="btn dashboard-btn-primary"
                            OnClick="btnSaveProfile_Click" />

                    </div>

                </div>

            </div>


            <!-- ACCOUNT INFORMATION -->

            <div>

                <div class="dashboard-panel mb-4">

                    <div class="dashboard-panel-header">
                        <div>
                            <div class="dashboard-eyebrow">
                                ACCOUNT
                            </div>

                            <h2 class="dashboard-panel-title">
                                Account Information
                            </h2>
                        </div>
                    </div>

                    <div class="dashboard-detail-list">

                        <div class="dashboard-detail-row">
                            <span class="dashboard-detail-label">
                                Customer Type
                            </span>

                            <span class="dashboard-detail-value">
                                <asp:Label
                                    ID="lblCustomerType"
                                    runat="server"
                                    Text="-">
                                </asp:Label>
                            </span>
                        </div>

                        <div class="dashboard-detail-row">
                            <span class="dashboard-detail-label">
                                Account Status
                            </span>

                            <span class="dashboard-detail-value">
                                <asp:Label
                                    ID="lblAccountStatus"
                                    runat="server"
                                    Text="-">
                                </asp:Label>
                            </span>
                        </div>

                        <div class="dashboard-detail-row">
                            <span class="dashboard-detail-label">
                                Member Since
                            </span>

                            <span class="dashboard-detail-value">
                                <asp:Label
                                    ID="lblDateRegistered"
                                    runat="server"
                                    Text="-">
                                </asp:Label>
                            </span>
                        </div>

                        <div class="dashboard-detail-row">
                            <span class="dashboard-detail-label">
                                City
                            </span>

                            <span class="dashboard-detail-value">
                                <asp:Label
                                    ID="lblCity"
                                    runat="server"
                                    Text="Durban">
                                </asp:Label>
                            </span>
                        </div>

                        <div class="dashboard-detail-row">
                            <span class="dashboard-detail-label">
                                Province
                            </span>

                            <span class="dashboard-detail-value">
                                <asp:Label
                                    ID="lblProvince"
                                    runat="server"
                                    Text="KwaZulu-Natal">
                                </asp:Label>
                            </span>
                        </div>

                    </div>

                </div>


                <!-- ACCOUNT SECURITY -->

                <div class="dashboard-panel">

                    <div class="dashboard-panel-header">
                        <div>
                            <div class="dashboard-eyebrow">
                                SECURITY
                            </div>

                            <h2 class="dashboard-panel-title">
                                Account Security
                            </h2>
                        </div>
                    </div>

                    <div class="dashboard-security-card mb-4">

                        <div class="dashboard-security-icon">
                            <i class="bi bi-shield-lock"></i>
                        </div>

                        <div class="dashboard-security-content">

                            <strong>Password</strong>

                            <span>
                                Use your registered email address to verify the change.
                            </span>

                        </div>

                    </div>


<p>Request a one-time code at your account email address to choose a new password.</p>
                    <a runat="server" href="~/Account/forgotPassword.aspx" class="dashboard-btn">Reset password by email</a>

                </div>

            </div>

        </div>

    </div>


    <!-- CLIENT-SIDE VALIDATION -->

    


</div>

</div>
</asp:Content>
