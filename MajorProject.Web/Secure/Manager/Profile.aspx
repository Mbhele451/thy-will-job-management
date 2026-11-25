
<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs"
    Inherits="MajorProject.Web.Secure.Manager.Profile"
    MasterPageFile="~/MasterPages/Dashboard.Master" %>

<asp:Content ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-manager-profile">


    <div class="dashboard-page manager-profile-page">

        <!-- PAGE HEADER -->
        <div class="dashboard-page-header">
            <div>
                <div class="dashboard-eyebrow">ACCOUNT</div>
                <h1 class="dashboard-page-title">My Profile</h1>
                <p class="dashboard-page-subtitle">
                    Manage your personal information, account security and system access.
                </p>
            </div>

            <div class="dashboard-page-actions">
                <asp:Button
                    ID="btnLogout"
                    runat="server"
                    Text="Sign Out"
                    CssClass="dashboard-btn dashboard-btn-secondary"
                    OnClick="btnLogout_Click"
                    CausesValidation="false" />
            </div>
        </div>

        <!-- MESSAGE -->
        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="dashboard-message"
            EnableViewState="false">
        </asp:Label>

        <!-- PROFILE OVERVIEW -->
        <div class="dashboard-workspace manager-profile-layout">

            <!-- PROFILE CARD -->
            <section class="dashboard-panel manager-profile-card">

                <div class="dashboard-panel-header">
                    <div>
                        <div class="dashboard-eyebrow">ACCOUNT OVERVIEW</div>
                        <h2 class="dashboard-panel-title">Your Profile</h2>
                        <p class="dashboard-panel-subtitle">
                            Your current staff account information.
                        </p>
                    </div>
                </div>

                <div class="manager-profile-identity">
                    <div class="manager-profile-avatar">
                        <asp:Label ID="lblInitials" runat="server"></asp:Label>
                    </div>

                    <div class="manager-profile-identity-text">
                        <h3>
                            <asp:Label ID="lblFullName" runat="server"></asp:Label>
                        </h3>

                        <div class="manager-profile-role">
                            <i class="bi bi-shield-check"></i>
                            <asp:Label ID="lblRole" runat="server"></asp:Label>
                        </div>

                        <div class="manager-profile-status">
                            <span class="manager-profile-status-dot"></span>
                            <asp:Label ID="lblAccountStatus" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="manager-profile-details">

                    <div class="manager-profile-detail">
                        <span class="manager-profile-detail-label">Staff ID</span>
                        <span class="manager-profile-detail-value">
                            #<asp:Label ID="lblStaffID" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="manager-profile-detail">
                        <span class="manager-profile-detail-label">Username</span>
                        <span class="manager-profile-detail-value">
                            <asp:Label ID="lblUsername" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="manager-profile-detail">
                        <span class="manager-profile-detail-label">Email Address</span>
                        <span class="manager-profile-detail-value">
                            <asp:Label ID="lblEmail" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="manager-profile-detail">
                        <span class="manager-profile-detail-label">Phone Number</span>
                        <span class="manager-profile-detail-value">
                            <asp:Label ID="lblPhone" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="manager-profile-detail">
                        <span class="manager-profile-detail-label">Hire Date</span>
                        <span class="manager-profile-detail-value">
                            <asp:Label ID="lblHireDate" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="manager-profile-detail">
                        <span class="manager-profile-detail-label">Availability</span>
                        <span class="manager-profile-detail-value">
                            <asp:Label ID="lblAvailability" runat="server"></asp:Label>
                        </span>
                    </div>

                </div>
            </section>

            <!-- SYSTEM ACCESS -->
            <section class="dashboard-panel manager-access-card">

                <div class="dashboard-panel-header">
                    <div>
                        <div class="dashboard-eyebrow">SYSTEM ACCESS</div>
                        <h2 class="dashboard-panel-title">Manager Access</h2>
                        <p class="dashboard-panel-subtitle">
                            Your account permissions within THY WILL.
                        </p>
                    </div>
                </div>

                <div class="manager-access-list">

                    <div class="manager-access-item">
                        <div class="manager-access-icon">
                            <i class="bi bi-people-fill"></i>
                        </div>
                        <div>
                            <strong>Staff Management</strong>
                            <span>Manage staff accounts, roles and availability.</span>
                        </div>
                        <i class="bi bi-check-circle-fill manager-access-check"></i>
                    </div>

                    <div class="manager-access-item">
                        <div class="manager-access-icon">
                            <i class="bi bi-briefcase-fill"></i>
                        </div>
                        <div>
                            <strong>Job Management</strong>
                            <span>View, assign and manage service jobs.</span>
                        </div>
                        <i class="bi bi-check-circle-fill manager-access-check"></i>
                    </div>

                    <div class="manager-access-item">
                        <div class="manager-access-icon">
                            <i class="bi bi-box-seam-fill"></i>
                        </div>
                        <div>
                            <strong>Inventory</strong>
                            <span>Monitor and manage system inventory.</span>
                        </div>
                        <i class="bi bi-check-circle-fill manager-access-check"></i>
                    </div>

                    <div class="manager-access-item">
                        <div class="manager-access-icon">
                            <i class="bi bi-bar-chart-fill"></i>
                        </div>
                        <div>
                            <strong>Reports &amp; Finances</strong>
                            <span>Access manager-level operational and financial information.</span>
                        </div>
                        <i class="bi bi-check-circle-fill manager-access-check"></i>
                    </div>

                </div>
            </section>
        </div>

        <!-- EDIT PERSONAL INFORMATION -->
        <section class="dashboard-panel">

            <div class="dashboard-panel-header">
                <div>
                    <div class="dashboard-eyebrow">PERSONAL INFORMATION</div>
                    <h2 class="dashboard-panel-title">Edit Profile</h2>
                    <p class="dashboard-panel-subtitle">
                        Update the personal information associated with your account.
                    </p>
                </div>
            </div>

            <div class="manager-profile-form">

                <!-- FULL NAME: READ ONLY -->
                <div class="dashboard-form-group">
                    <label
                        for="<%= txtFullName.ClientID %>"
                        class="dashboard-form-label">
                        Full Name
                    </label>

                    <asp:TextBox
                        ID="txtFullName"
                        runat="server"
                        CssClass="dashboard-form-control"
                        ReadOnly="true"
                        MaxLength="100">
                    </asp:TextBox>

                    <small class="dashboard-form-help">
                        Your name cannot be changed from this page.
                    </small>
                </div>

                <!-- PHONE -->
                <div class="dashboard-form-group">
                    <label
                        for="<%= txtPhone.ClientID %>"
                        class="dashboard-form-label">
                        Phone Number
                    </label>

                    <asp:TextBox
                        ID="txtPhone"
                        runat="server"
                        CssClass="dashboard-form-control"
                        MaxLength="10"
                        inputmode="numeric"
                        autocomplete="tel">
                    </asp:TextBox>

                    <span
                        id="phoneError"
                        class="client-field-error"
                        aria-live="polite">
                    </span>
                </div>

                <!-- EMAIL -->
                <div class="dashboard-form-group">
                    <label
                        for="<%= txtEmail.ClientID %>"
                        class="dashboard-form-label">
                        Email Address
                    </label>

                    <asp:TextBox
                        ID="txtEmail"
                        runat="server"
                        CssClass="dashboard-form-control"
                        TextMode="Email"
                        MaxLength="100"
                        autocomplete="email">
                    </asp:TextBox>

                    <span
                        id="emailError"
                        class="client-field-error"
                        aria-live="polite">
                    </span>
                </div>

                <!-- USERNAME: READ ONLY -->
                <div class="dashboard-form-group">
                    <label
                        for="<%= txtUsername.ClientID %>"
                        class="dashboard-form-label">
                        Username
                    </label>

                    <asp:TextBox
                        ID="txtUsername"
                        runat="server"
                        CssClass="dashboard-form-control"
                        ReadOnly="true">
                    </asp:TextBox>

                    <small class="dashboard-form-help">
                        Your username cannot be changed from this page.
                    </small>
                </div>

            </div>

            <div class="dashboard-action-group">
                <asp:Button
                    ID="btnSaveProfile"
                    data-profile-save="true"
                    runat="server"
                    Text="Save Changes"
                    CssClass="dashboard-btn"
                    OnClick="btnSaveProfile_Click"
                    CausesValidation="false" />
            </div>

        </section>

        <!-- SECURITY -->
        <section class="dashboard-panel"><h2 class="dashboard-panel-title">Account security</h2>
                    <p>Request a one-time code at your account email address to choose a new password.</p>
                    <a runat="server" href="~/Account/forgotPassword.aspx" class="dashboard-btn">Reset password by email</a></section>

        <!-- ACCOUNT INFORMATION -->
        <section class="dashboard-panel manager-account-information">

            <div class="dashboard-panel-header">
                <div>
                    <div class="dashboard-eyebrow">ACCOUNT INFORMATION</div>
                    <h2 class="dashboard-panel-title">Account Status</h2>
                </div>
            </div>

            <div class="dashboard-info-banner">
                <div class="dashboard-info-icon">
                    <i class="bi bi-shield-lock-fill"></i>
                </div>

                <div>
                    <strong>Manager account</strong>
                    <p>
                        Your account is protected by the THY WILL authentication system.
                        Role and employment-status changes are managed through Staff Management.
                    </p>
                </div>
            </div>

        </section>

    </div>

    <!-- CLIENT-SIDE VALIDATION -->
    

    


</div>

</div>
</asp:Content>