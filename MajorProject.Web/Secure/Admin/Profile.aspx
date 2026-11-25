
<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="Profile.aspx.cs"
    Inherits="MajorProject.Web.Secure.Admin.Profile"
    MasterPageFile="~/MasterPages/Dashboard.Master" %>

<asp:Content ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-admin-profile">


    

    <div class="dashboard-page admin-profile-page">

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


        <!-- SERVER MESSAGE -->

        <asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="dashboard-message"
            EnableViewState="false"
            role="status"
            aria-live="polite">
        </asp:Label>


        <!-- PROFILE OVERVIEW AND ACCESS -->

        <div class="dashboard-workspace admin-profile-layout">

            <!-- PROFILE CARD -->

            <section class="dashboard-panel admin-profile-card">

                <div class="dashboard-panel-header">
                    <div>
                        <div class="dashboard-eyebrow">ACCOUNT OVERVIEW</div>

                        <h2 class="dashboard-panel-title">Your Profile</h2>

                        <p class="dashboard-panel-subtitle">
                            Your current administrator account information.
                        </p>
                    </div>
                </div>

                <div class="admin-profile-identity">

                    <div class="admin-profile-avatar">
                        <asp:Label ID="lblInitials" runat="server"></asp:Label>
                    </div>

                    <div class="admin-profile-identity-text">

                        <h3>
                            <asp:Label ID="lblFullName" runat="server"></asp:Label>
                        </h3>

                        <div class="admin-profile-role">
                            <i class="bi bi-shield-fill-check"></i>
                            <asp:Label ID="lblRole" runat="server"></asp:Label>
                        </div>

                        <div class="admin-profile-status">
                            <span class="admin-profile-status-dot"></span>
                            <asp:Label ID="lblAccountStatus" runat="server"></asp:Label>
                        </div>

                    </div>

                </div>

                <div class="admin-profile-details">

                    <div class="admin-profile-detail">
                        <span class="admin-profile-detail-label">Staff ID</span>
                        <span class="admin-profile-detail-value">
                            #<asp:Label ID="lblStaffID" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="admin-profile-detail">
                        <span class="admin-profile-detail-label">Username</span>
                        <span class="admin-profile-detail-value">
                            <asp:Label ID="lblUsername" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="admin-profile-detail">
                        <span class="admin-profile-detail-label">Email Address</span>
                        <span class="admin-profile-detail-value">
                            <asp:Label ID="lblEmail" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="admin-profile-detail">
                        <span class="admin-profile-detail-label">Phone Number</span>
                        <span class="admin-profile-detail-value">
                            <asp:Label ID="lblPhone" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="admin-profile-detail">
                        <span class="admin-profile-detail-label">Hire Date</span>
                        <span class="admin-profile-detail-value">
                            <asp:Label ID="lblHireDate" runat="server"></asp:Label>
                        </span>
                    </div>

                    <div class="admin-profile-detail">
                        <span class="admin-profile-detail-label">Availability</span>
                        <span class="admin-profile-detail-value">
                            <asp:Label ID="lblAvailability" runat="server"></asp:Label>
                        </span>
                    </div>

                </div>

            </section>


            <!-- ADMIN ACCESS -->

            <section class="dashboard-panel admin-access-card">

                <div class="dashboard-panel-header">
                    <div>
                        <div class="dashboard-eyebrow">SYSTEM ACCESS</div>

                        <h2 class="dashboard-panel-title">
                            Administrator Access
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            Access available to your administrator account.
                        </p>
                    </div>
                </div>

                <div class="admin-access-list">

                    <div class="admin-access-item">
                        <div class="admin-access-icon">
                            <i class="bi bi-people-fill"></i>
                        </div>

                        <div>
                            <strong>Customer Management</strong>
                            <span>View and manage customer information.</span>
                        </div>

                        <i class="bi bi-check-circle-fill admin-access-check"></i>
                    </div>

                    <div class="admin-access-item">
                        <div class="admin-access-icon">
                            <i class="bi bi-briefcase-fill"></i>
                        </div>

                        <div>
                            <strong>Job Management</strong>
                            <span>Access service jobs and job-related information.</span>
                        </div>

                        <i class="bi bi-check-circle-fill admin-access-check"></i>
                    </div>

                    <div class="admin-access-item">
                        <div class="admin-access-icon">
                            <i class="bi bi-box-seam-fill"></i>
                        </div>

                        <div>
                            <strong>Inventory</strong>
                            <span>View inventory records and stock levels. Inventory changes require Manager access.</span>
                        </div>

                        <i class="bi bi-check-circle-fill admin-access-check"></i>
                    </div>

                    <div class="admin-access-item admin-access-restricted">
                        <div class="admin-access-icon"><i class="bi bi-lock-fill" aria-hidden="true"></i></div>
                        <div><strong>Staff management restricted</strong><span>Staff management requires Manager access. Your Administrator account has no staff-management access.</span></div>
                        <i class="bi bi-lock admin-access-check" aria-hidden="true"></i>
                    </div>

                </div>

            </section>

        </div>


        <!-- EDIT PROFILE -->

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

            <div class="admin-profile-form">

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
                        MaxLength="100">
                    </asp:TextBox>

                    <div id="adminFullNameError"
                         class="profile-field-error"
                         aria-live="polite"></div>

                </div>

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

                    <div id="adminPhoneError"
                         class="profile-field-error"
                         aria-live="polite"></div>

                </div>

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

                    <div id="adminEmailError"
                         class="profile-field-error"
                         aria-live="polite"></div>

                </div>

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


        <!-- CHANGE PASSWORD -->

        <section class="dashboard-panel"><h2 class="dashboard-panel-title">Account security</h2>
                    <p>Request a one-time code at your account email address to choose a new password.</p>
                    <a runat="server" href="~/Account/forgotPassword.aspx" class="dashboard-btn">Reset password by email</a></section>


        <!-- ACCOUNT PROTECTION -->

        <section class="dashboard-panel">

            <div class="dashboard-panel-header">
                <div>
                    <div class="dashboard-eyebrow">ACCOUNT SECURITY</div>

                    <h2 class="dashboard-panel-title">
                        Account Protection
                    </h2>
                </div>
            </div>

            <div class="dashboard-info-banner">

                <div class="dashboard-info-icon">
                    <i class="bi bi-shield-lock-fill"></i>
                </div>

                <div>
                    <strong>Administrator account</strong>

                    <p>
                        Your administrator account is protected by the THY WILL
                        authentication system. Username, role and employment status
                        are controlled by the system's staff administration functions.
                    </p>
                </div>

            </div>

        </section>

    </div>


    <!-- CLIENT-SIDE VALIDATION -->

    


</div>

</div>
</asp:Content>
