
<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs"
    Inherits="MajorProject.Web.Secure.Technician.Profile"
    MasterPageFile="~/MasterPages/Dashboard.Master" %>

<asp:Content ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-technician-profile">


    

    <div class="dashboard-page technician-profile-page">

        <!-- PAGE HEADER -->

        <div class="dashboard-page-header">

            <div>
                <div class="dashboard-eyebrow">
                    TECHNICIAN ACCOUNT
                </div>

                <h1 class="dashboard-page-title">
                    My Profile
                </h1>

                <p class="dashboard-page-subtitle">
                    Manage your account, availability and field-work information.
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

        <!-- FIELD STATUS -->

        <section class="technician-field-status">

            <div class="technician-field-status-icon">
                <i class="bi bi-broadcast-pin"></i>
            </div>

            <div class="technician-field-status-content">
                <span class="technician-field-status-label">
                    FIELD AVAILABILITY
                </span>

                <strong>
                    <asp:Label
                        ID="lblFieldStatus"
                        runat="server">
                    </asp:Label>
                </strong>

                <p>
                    Your availability determines whether the manager can assign
                    you new jobs.
                </p>
            </div>

            <div class="technician-field-status-action">

                <asp:Button
                    ID="btnSetAvailable"
                    runat="server"
                    Text="I'm Available"
                    CssClass="dashboard-btn dashboard-btn-success"
                    OnClick="btnSetAvailable_Click"
                    CausesValidation="false" />

                <asp:Button
                    ID="btnSetUnavailable"
                    runat="server"
                    Text="I'm Unavailable"
                    CssClass="dashboard-btn dashboard-btn-warning"
                    OnClick="btnSetUnavailable_Click"
                    CausesValidation="false" />

            </div>

        </section>

        <!-- PROFILE AND FIELD TOOLS -->

        <div class="dashboard-workspace technician-profile-layout">

            <section class="dashboard-panel technician-profile-card">

                <div class="dashboard-panel-header">
                    <div>
                        <div class="dashboard-eyebrow">
                            TECHNICIAN PROFILE
                        </div>

                        <h2 class="dashboard-panel-title">
                            Your Information
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            Information used by THY WILL when managing your field work.
                        </p>
                    </div>
                </div>

                <!-- IDENTITY -->

                <div class="technician-profile-identity">

                    <div class="technician-profile-avatar">
                        <asp:Label
                            ID="lblInitials"
                            runat="server">
                        </asp:Label>
                    </div>

                    <div class="technician-profile-identity-text">

                        <h3>
                            <asp:Label
                                ID="lblFullName"
                                runat="server">
                            </asp:Label>
                        </h3>

                        <div class="technician-profile-role">
                            <i class="bi bi-tools"></i>

                            <asp:Label
                                ID="lblRole"
                                runat="server">
                            </asp:Label>
                        </div>

                        <div class="technician-profile-status">
                            <span class="technician-profile-status-dot"></span>

                            <asp:Label
                                ID="lblAccountStatus"
                                runat="server">
                            </asp:Label>
                        </div>

                    </div>
                </div>

                <!-- DETAILS -->

                <div class="technician-profile-details">

                    <div class="technician-profile-detail">
                        <span class="technician-profile-detail-label">
                            Technician ID
                        </span>

                        <span class="technician-profile-detail-value">
                            #<asp:Label
                                ID="lblStaffID"
                                runat="server">
                            </asp:Label>
                        </span>
                    </div>

                    <div class="technician-profile-detail">
                        <span class="technician-profile-detail-label">
                            Username
                        </span>

                        <span class="technician-profile-detail-value">
                            <asp:Label
                                ID="lblUsername"
                                runat="server">
                            </asp:Label>
                        </span>
                    </div>

                    <div class="technician-profile-detail">
                        <span class="technician-profile-detail-label">
                            Phone
                        </span>

                        <span class="technician-profile-detail-value">
                            <asp:Label
                                ID="lblPhone"
                                runat="server">
                            </asp:Label>
                        </span>
                    </div>

                    <div class="technician-profile-detail">
                        <span class="technician-profile-detail-label">
                            Email
                        </span>

                        <span class="technician-profile-detail-value">
                            <asp:Label
                                ID="lblEmail"
                                runat="server">
                            </asp:Label>
                        </span>
                    </div>

                    <div class="technician-profile-detail">
                        <span class="technician-profile-detail-label">
                            Hire Date
                        </span>

                        <span class="technician-profile-detail-value">
                            <asp:Label
                                ID="lblHireDate"
                                runat="server">
                            </asp:Label>
                        </span>
                    </div>

                    <div class="technician-profile-detail">
                        <span class="technician-profile-detail-label">
                            Availability
                        </span>

                        <span class="technician-profile-detail-value">
                            <asp:Label
                                ID="lblAvailability"
                                runat="server">
                            </asp:Label>
                        </span>
                    </div>

                </div>

            </section>

            <!-- FIELD TOOLS -->

            <section class="dashboard-panel technician-field-tools">

                <div class="dashboard-panel-header">
                    <div>
                        <div class="dashboard-eyebrow">
                            FIELD TOOLS
                        </div>

                        <h2 class="dashboard-panel-title">
                            Quick Access
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            Useful tools while you're on site.
                        </p>
                    </div>
                </div>

                <div class="technician-profile-actions">

                    <a
                        href="<%= ResolveUrl("~/Secure/Technician/Dashboard.aspx") %>"
                        class="technician-profile-action">

                        <span class="technician-profile-action-icon">
                            <i class="bi bi-briefcase-fill"></i>
                        </span>

                        <span class="technician-profile-action-content">
                            <strong>My Jobs</strong>
                            <small>View assigned service jobs.</small>
                        </span>

                        <i class="bi bi-chevron-right"></i>
                    </a>

                    <a
                        href="<%= ResolveUrl("~/Secure/Technician/Inventory.aspx") %>"
                        class="technician-profile-action">

                        <span class="technician-profile-action-icon">
                            <i class="bi bi-box-seam-fill"></i>
                        </span>

                        <span class="technician-profile-action-content">
                            <strong>Inventory</strong>
                            <small>Check equipment and stock.</small>
                        </span>

                        <i class="bi bi-chevron-right"></i>
                    </a>

                    <div class="technician-profile-field-note">
                        <i class="bi bi-info-circle-fill"></i>

                        <span>
                            Keep your availability accurate so managers know
                            when you can receive new assignments.
                        </span>
                    </div>

                </div>

            </section>

        </div>

        <!-- EDIT CONTACT INFORMATION -->

        <section class="dashboard-panel">

            <div class="dashboard-panel-header">
                <div>
                    <div class="dashboard-eyebrow">
                        CONTACT INFORMATION
                    </div>

                    <h2 class="dashboard-panel-title">
                        Update Your Details
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Keep your contact information up to date for customers
                        and the THY WILL team.
                    </p>
                </div>
            </div>

            <div class="technician-profile-form">

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
                        MaxLength="100"
                        ReadOnly="true">
                    </asp:TextBox>

                    <small class="dashboard-form-help">
                        Your full name is managed by the system.
                        Contact your administrator if it needs to be changed.
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
                        MaxLength="10">
                    </asp:TextBox>

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
                        MaxLength="100">
                    </asp:TextBox>

                </div>

                <!-- USERNAME -->

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
                        Your username is managed by the system.
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

        <!-- CHANGE PASSWORD -->

        <section class="dashboard-panel"><h2 class="dashboard-panel-title">Account security</h2>
                    <p>Request a one-time code at your account email address to choose a new password.</p>
                    <a runat="server" href="~/Account/forgotPassword.aspx" class="dashboard-btn">Reset password by email</a></section>

        <!-- SECURITY NOTICE -->

        <section class="dashboard-panel">

            <div class="dashboard-info-banner">

                <div class="dashboard-info-icon">
                    <i class="bi bi-shield-lock-fill"></i>
                </div>

                <div>
                    <strong>Protect your technician account</strong>

                    <p>
                        Never share your password with another person.
                        If you believe your account has been compromised,
                        change your password and contact your manager.
                    </p>
                </div>

            </div>

        </section>

    </div>

    <!-- =============================================================
         JAVASCRIPT VALIDATION
         ============================================================= -->

    


</div>

</div>
</asp:Content>
