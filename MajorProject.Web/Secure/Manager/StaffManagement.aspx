
<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="StaffManagement.aspx.cs"
    Inherits="MajorProject.Web.Manager.StaffManagement"
    MasterPageFile="~/MasterPages/Dashboard.Master" %>

<asp:Content ID="MainContent"
             ContentPlaceHolderID="MainContent"
             runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-manager-staffmanagement">


    

    <div class="dashboard-page staff-management-page"  data-record-tabs="secure-manager-staffmanagement" data-tab-labels="list=Staff;details=Staff details;add=Add staff" data-selected-id="lblStaffID">
<asp:Label ID="lblMessage"
                   runat="server"
                   CssClass="dashboard-info-banner staff-message"
                   EnableViewState="false">
        </asp:Label>


        <!-- PAGE HEADER -->

        <div class="dashboard-page-header">

            <div>
                <div class="dashboard-eyebrow">
                    <i class="bi bi-person-badge-fill"></i>
                    STAFF MANAGEMENT
                </div>

                <h1 class="dashboard-page-title">
                    Staff Management
                </h1>

                <p class="dashboard-page-subtitle">
                    Manage employees, roles, employment status and availability.
                </p>
            </div>

            <div class="dashboard-page-actions">
                <button type="button"
                        class="dashboard-btn dashboard-btn-primary"
                        data-tab-open="add">
                    <i class="bi bi-person-plus-fill"></i>
                    Add Staff
                </button>
            </div>

        </div>


        <!-- MESSAGE -->

        


        <!-- STAFF STATISTICS -->

        <div class="dashboard-stats staff-management-stats">

            <div class="dashboard-stat-card">
                <div class="dashboard-stat-icon">
                    <i class="bi bi-people-fill"></i>
                </div>

                <div class="dashboard-stat-content">
                    <span class="dashboard-stat-label">
                        Total Staff
                    </span>

                    <asp:Label ID="lblTotalStaff"
                               runat="server"
                               CssClass="dashboard-stat-value"
                               Text="0">
                    </asp:Label>

                    <span class="dashboard-stat-meta">
                        All staff members
                    </span>
                </div>
            </div>

            <div class="dashboard-stat-card dashboard-stat-card-primary">
                <div class="dashboard-stat-icon">
                    <i class="bi bi-shield-check"></i>
                </div>

                <div class="dashboard-stat-content">
                    <span class="dashboard-stat-label">
                        Managers
                    </span>

                    <asp:Label ID="lblManagers"
                               runat="server"
                               CssClass="dashboard-stat-value"
                               Text="0">
                    </asp:Label>

                    <span class="dashboard-stat-meta">
                        Management staff
                    </span>
                </div>
            </div>

            <div class="dashboard-stat-card dashboard-stat-card-info">
                <div class="dashboard-stat-icon">
                    <i class="bi bi-person-gear"></i>
                </div>

                <div class="dashboard-stat-content">
                    <span class="dashboard-stat-label">
                        Administrators
                    </span>

                    <asp:Label ID="lblAdministrators"
                               runat="server"
                               CssClass="dashboard-stat-value"
                               Text="0">
                    </asp:Label>

                    <span class="dashboard-stat-meta">
                        Administrative staff
                    </span>
                </div>
            </div>

            <div class="dashboard-stat-card dashboard-stat-card-success">
                <div class="dashboard-stat-icon">
                    <i class="bi bi-tools"></i>
                </div>

                <div class="dashboard-stat-content">
                    <span class="dashboard-stat-label">
                        Technicians
                    </span>

                    <asp:Label ID="lblTechnicians"
                               runat="server"
                               CssClass="dashboard-stat-value"
                               Text="0">
                    </asp:Label>

                    <span class="dashboard-stat-meta">
                        Technical staff
                    </span>
                </div>
            </div>

        </div>


        <!-- STAFF DIRECTORY -->

        <div class="dashboard-panel staff-management-panel"  data-tab-pane="list">

            <div class="dashboard-panel-header">
                <div>
                    <h2 class="dashboard-panel-title">
                        <i class="bi bi-people-fill"></i>
                        Staff Directory
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Search and select a staff member to manage their account.
                    </p>
                </div>
            </div>

            <div class="staff-directory-toolbar">

                <div class="staff-directory-search">
                    <label class="dashboard-stat-label"
                           for="<%= txtSearch.ClientID %>">
                        Search staff
                    </label>

                    <div class="dashboard-search">
                        <i class="bi bi-search"></i>

                        <asp:TextBox ID="txtSearch"
                                     runat="server"
                                     CssClass="dashboard-form-control"
                                     placeholder="Name, username, email or phone">
                        </asp:TextBox>
                    </div>
                </div>

                <div class="staff-directory-filter">
                    <label class="dashboard-stat-label"
                           for="<%= ddlRoleFilter.ClientID %>">
                        Role
                    </label>

                    <asp:DropDownList ID="ddlRoleFilter"
                                      runat="server"
                                      CssClass="form-select">
                    </asp:DropDownList>
                </div>

                <div class="staff-directory-filter">
                    <label class="dashboard-stat-label"
                           for="<%= ddlStatusFilter.ClientID %>">
                        Employment status
                    </label>

                    <asp:DropDownList ID="ddlStatusFilter"
                                      runat="server"
                                      CssClass="form-select">

                        <asp:ListItem Text="All" Value="All"></asp:ListItem>
                        <asp:ListItem Text="Active" Value="Active"></asp:ListItem>
                        <asp:ListItem Text="Inactive" Value="Inactive"></asp:ListItem>

                    </asp:DropDownList>
                </div>

                <div class="staff-directory-actions">
                    <asp:Button ID="btnSearch"
                                runat="server"
                                Text="Search"
                                CssClass="dashboard-btn"
                                OnClick="btnSearch_Click" />

                    <asp:Button ID="btnClear"
                                runat="server"
                                Text="Clear"
                                CssClass="dashboard-btn dashboard-btn-secondary"
                                OnClick="btnClear_Click" />
                </div>

            </div>

            <div class="dashboard-table-wrapper staff-table-wrapper">

                <asp:GridView ID="dgvStaff"
                              runat="server"
                              AutoGenerateColumns="False"
                              DataKeyNames="StaffID"
                              CssClass="dashboard-table staff-table"
                              GridLines="None"
                              AllowPaging="false"
                              OnSelectedIndexChanged="dgvStaff_SelectedIndexChanged">

                    <Columns>

                        <asp:BoundField
                            DataField="FullName"
                            HeaderText="Name" />

                        <asp:BoundField
                            DataField="Username"
                            HeaderText="Username" />

                        <asp:BoundField
                            DataField="RoleName"
                            HeaderText="Role" />

                        <asp:BoundField
                            DataField="PhoneNumber"
                            HeaderText="Phone" />

                        <asp:BoundField
                            DataField="EmploymentStatus"
                            HeaderText="Status" />

                        <asp:TemplateField HeaderText="Availability">
                            <ItemTemplate>
                                <span class='<%# Convert.ToBoolean(Eval("IsAvailable")) ? "staff-availability staff-availability-available" : "staff-availability staff-availability-unavailable" %>'>
                                    <%# Convert.ToBoolean(Eval("IsAvailable")) ? "Available" : "Unavailable" %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:CommandField
                            ShowSelectButton="True"
                            SelectText="View"
                            HeaderText="Action"
                            ButtonType="Link"
                            ControlStyle-CssClass="dashboard-table-action" />

                    </Columns>

                    <EmptyDataTemplate>
                        <div class="staff-empty-state">
                            <i class="bi bi-people"></i>

                            <div class="staff-empty-state-title">
                                No staff members found
                            </div>

                            <p class="staff-empty-state-text">
                                Try changing your search or filter.
                            </p>
                        </div>
                    </EmptyDataTemplate>

                </asp:GridView>

            </div>

        </div>


        <!-- SELECTED STAFF -->

        <asp:Panel ID="pnlSelectedStaff"
                   runat="server"
                   CssClass="dashboard-panel"
                   Visible="false"  data-tab-pane="details" data-ready-id="lblStaffID" data-ready-kind="positive">

            <div class="dashboard-panel-header">
                <div>
                    <div class="dashboard-eyebrow">
                        SELECTED STAFF
                    </div>

                    <h2 class="dashboard-panel-title">
                        <asp:Label ID="lblSelectedStaffName"
                                   runat="server"
                                   Text="—">
                        </asp:Label>
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Staff account details and management actions.
                    </p>
                </div>

                <div>
                    <asp:Label ID="lblSelectedStaffRole"
                               runat="server"
                               CssClass="dashboard-role-badge"
                               Text="—">
                    </asp:Label>
                </div>
            </div>


            <!-- BASIC DETAILS -->

            <div class="staff-selected-details">

                <div class="staff-detail">
                    <span class="staff-detail-label">Staff ID</span>
                    <span class="staff-detail-value">
                        <asp:Label ID="lblStaffID" runat="server" Text="—"></asp:Label>
                    </span>
                </div>

                <div class="staff-detail">
                    <span class="staff-detail-label">Username</span>
                    <span class="staff-detail-value">
                        <asp:Label ID="lblUsername" runat="server" Text="—"></asp:Label>
                    </span>
                </div>

                <div class="staff-detail">
                    <span class="staff-detail-label">Email</span>
                    <span class="staff-detail-value">
                        <asp:Label ID="lblEmail" runat="server" Text="—"></asp:Label>
                    </span>
                </div>

                <div class="staff-detail">
                    <span class="staff-detail-label">Phone</span>
                    <span class="staff-detail-value">
                        <asp:Label ID="lblPhone" runat="server" Text="—"></asp:Label>
                    </span>
                </div>

                <div class="staff-detail">
                    <span class="staff-detail-label">Hire Date</span>
                    <span class="staff-detail-value">
                        <asp:Label ID="lblHireDate" runat="server" Text="—"></asp:Label>
                    </span>
                </div>

                <div class="staff-detail">
                    <span class="staff-detail-label">Salary</span>
                    <span class="staff-detail-value">
                        R
                        <asp:Label ID="lblSalary" runat="server" Text="0.00"></asp:Label>
                    </span>
                </div>

                <div class="staff-detail">
                    <span class="staff-detail-label">Employment Status</span>
                    <span class="staff-detail-value">
                        <asp:Label ID="lblEmploymentStatus" runat="server" Text="—"></asp:Label>
                    </span>
                </div>

                <div class="staff-detail">
                    <span class="staff-detail-label">Availability</span>
                    <span class="staff-detail-value">
                        <asp:Label ID="lblAvailability" runat="server" Text="—"></asp:Label>
                    </span>
                </div>

            </div>


            <!-- EDIT STAFF DETAILS -->

            <div class="dashboard-panel staff-management-panel">

                <div class="dashboard-panel-header">
                    <div>
                        <h3 class="dashboard-panel-title">
                            <i class="bi bi-pencil-square"></i>
                            Edit Staff Details
                        </h3>

                        <p class="dashboard-panel-subtitle">
                            Update the selected employee's information.
                        </p>
                    </div>
                </div>

                <div class="staff-form-grid">

                    <div class="staff-form-group">
                        <label class="staff-form-label"
                               for="<%= txtEditFullName.ClientID %>">
                            Full Name
                        </label>

                        <asp:TextBox ID="txtEditFullName"
                                     runat="server"
                                     CssClass="dashboard-form-control">
                        </asp:TextBox>
                    </div>

                    <div class="staff-form-group">
                        <label class="staff-form-label"
                               for="<%= ddlEditRole.ClientID %>">
                            Role
                        </label>

                        <asp:DropDownList ID="ddlEditRole"
                                          runat="server"
                                          CssClass="form-select">
                        </asp:DropDownList>
                    </div>

                    <div class="staff-form-group">
                        <label class="staff-form-label"
                               for="<%= txtEditPhone.ClientID %>">
                            Phone Number
                        </label>

                        <asp:TextBox ID="txtEditPhone"
                                     runat="server"
                                     CssClass="dashboard-form-control"
                                     MaxLength="10">
                        </asp:TextBox>
                    </div>

                    <div class="staff-form-group">
                        <label class="staff-form-label"
                               for="<%= txtEditEmail.ClientID %>">
                            Email Address
                        </label>

                        <asp:TextBox ID="txtEditEmail"
                                     runat="server"
                                     CssClass="dashboard-form-control"
                                     TextMode="Email">
                        </asp:TextBox>
                    </div>

                    <div class="staff-form-group">
                        <label class="staff-form-label"
                               for="<%= txtEditSalary.ClientID %>">
                            Salary
                        </label>

                        <asp:TextBox ID="txtEditSalary"
                                     runat="server"
                                     CssClass="dashboard-form-control">
                        </asp:TextBox>
                    </div>

                </div>

                <div class="staff-form-actions">
                    <asp:Button ID="btnSaveDetails"
                                runat="server"
                                Text="Save Details"
                                CssClass="dashboard-btn dashboard-btn-success"
                                OnClick="btnSaveDetails_Click"
                                OnClientClick="return validateEditStaffDetails();" />
                </div>

            </div>


            <!-- PASSWORD MANAGEMENT -->

            <div class="dashboard-panel staff-management-panel">

                <div class="dashboard-panel-header">
                    <div>
                        <h3 class="dashboard-panel-title">
                            <i class="bi bi-key-fill"></i>
                            Password Management
                        </h3>

                        <p class="dashboard-panel-subtitle">
                            Set a new password for the selected staff account.
                        </p>
                    </div>
                </div>

                <div class="staff-password-form">

                    <div class="staff-password-field">
                        <label class="staff-form-label"
                               for="<%= txtResetPassword.ClientID %>">
                            New Password
                        </label>

                        <div class="staff-password-input">
                            <asp:TextBox
                                ID="txtResetPassword"
                                runat="server"
                                CssClass="dashboard-form-control"
                                TextMode="Password"
                                MaxLength="100">
                            </asp:TextBox>

                            <button type="button"
                                    class="staff-password-toggle"
                                    aria-label="Show password"
                                    aria-pressed="false"
                                    title="Show password"
                                    onclick="toggleStaffPassword('<%= txtResetPassword.ClientID %>', this);">
                                <i class="bi bi-eye"></i>
                            </button>
                        </div>
                    </div>

                    <div class="staff-password-field">
                        <label class="staff-form-label"
                               for="<%= txtConfirmResetPassword.ClientID %>">
                            Confirm Password
                        </label>

                        <div class="staff-password-input">
                            <asp:TextBox
                                ID="txtConfirmResetPassword"
                                runat="server"
                                CssClass="dashboard-form-control"
                                TextMode="Password"
                                MaxLength="100">
                            </asp:TextBox>

                            <button type="button"
                                    class="staff-password-toggle"
                                    aria-label="Show password"
                                    aria-pressed="false"
                                    title="Show password"
                                    onclick="toggleStaffPassword('<%= txtConfirmResetPassword.ClientID %>', this);">
                                <i class="bi bi-eye"></i>
                            </button>
                        </div>
                    </div>

                </div>

                <div class="staff-form-help">
                    Password must contain at least 8 characters, including uppercase, lowercase, a number and a special character.
                </div>

                <div class="staff-form-actions">
                    <asp:Button
                        ID="btnResetPassword"
                        runat="server"
                        Text="Reset Password"
                        CssClass="dashboard-btn dashboard-btn-warning"
                        OnClick="btnResetPassword_Click"
                        OnClientClick="return validateStaffPasswordReset();" />
                </div>

            </div>


            <!-- ACCOUNT MANAGEMENT -->

            <div class="dashboard-panel staff-management-panel">

                <div class="dashboard-panel-header">
                    <div>
                        <h3 class="dashboard-panel-title">
                            <i class="bi bi-person-gear"></i>
                            Account Management
                        </h3>

                        <p class="dashboard-panel-subtitle">
                            Manage employment status, technician availability and account archiving.
                        </p>
                    </div>
                </div>

                <div class="staff-form-actions">

                    <asp:Button ID="btnActivate"
                                runat="server"
                                Text="Activate"
                                CssClass="dashboard-btn dashboard-btn-success"
                                OnClick="btnActivate_Click" />

                    <asp:Button ID="btnDeactivate"
                                runat="server"
                                Text="Deactivate"
                                CssClass="dashboard-btn dashboard-btn-warning"
                                OnClick="btnDeactivate_Click"
                                OnClientClick="return confirm('Deactivate this staff account?');" />

                    <asp:Button ID="btnAvailable"
                                runat="server"
                                Text="Set Available"
                                CssClass="dashboard-btn"
                                OnClick="btnAvailable_Click" />

                    <asp:Button ID="btnUnavailable"
                                runat="server"
                                Text="Set Unavailable"
                                CssClass="dashboard-btn dashboard-btn-secondary"
                                OnClick="btnUnavailable_Click" />

                    <asp:Button ID="btnArchive"
                                runat="server"
                                Text="Archive Staff"
                                CssClass="dashboard-btn dashboard-btn-warning"
                                OnClick="btnArchive_Click"
                                OnClientClick="return confirm('Archive this staff member? They will be marked inactive and unavailable.');" />

                </div>

            </div>


            <!-- SELF PROTECTION -->

            <asp:Panel ID="pnlSelfWarning"
                       runat="server"
                       CssClass="dashboard-info-banner dashboard-info-success"
                       Visible="false">

                <div class="dashboard-info-icon">
                    <i class="bi bi-shield-lock-fill"></i>
                </div>

                <div>
                    <strong>Your Manager account is protected.</strong>

                    <div>
                        You cannot deactivate, archive, or change the role of your own account from Staff Management.
                    </div>
                </div>

            </asp:Panel>

        </asp:Panel>


        <!-- ADD STAFF -->

        <asp:Panel ID="pnlAddStaff"
                   runat="server"
                   CssClass="dashboard-panel"  data-tab-pane="add">

            <div class="dashboard-panel-header">
                <div>
                    <div class="dashboard-eyebrow">
                        NEW STAFF MEMBER
                    </div>

                    <h2 class="dashboard-panel-title">
                        <i class="bi bi-person-plus-fill"></i>
                        Add Staff
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Create a new employee account.
                    </p>
                </div>
            </div>

            <div class="staff-form-grid">

                <div class="staff-form-group">
                    <label class="staff-form-label"
                           for="<%= txtNewFullName.ClientID %>">
                        Full Name
                    </label>

                    <asp:TextBox ID="txtNewFullName"
                                 runat="server"
                                 CssClass="dashboard-form-control">
                    </asp:TextBox>
                </div>

                <div class="staff-form-group">
                    <label class="staff-form-label"
                           for="<%= ddlNewRole.ClientID %>">
                        Role
                    </label>

                    <asp:DropDownList ID="ddlNewRole"
                                      runat="server"
                                      CssClass="form-select">
                    </asp:DropDownList>
                </div>

                <div class="staff-form-group">
                    <label class="staff-form-label"
                           for="<%= txtNewPhone.ClientID %>">
                        Phone Number
                    </label>

                    <asp:TextBox ID="txtNewPhone"
                                 runat="server"
                                 CssClass="dashboard-form-control"
                                 MaxLength="10">
                    </asp:TextBox>
                </div>

                <div class="staff-form-group">
                    <label class="staff-form-label"
                           for="<%= txtNewEmail.ClientID %>">
                        Email Address
                    </label>

                    <asp:TextBox ID="txtNewEmail"
                                 runat="server"
                                 CssClass="dashboard-form-control"
                                 TextMode="Email">
                    </asp:TextBox>
                </div>

                <div class="staff-form-group">
                    <label class="staff-form-label"
                           for="<%= txtNewSalary.ClientID %>">
                        Salary
                    </label>

                    <asp:TextBox ID="txtNewSalary"
                                 runat="server"
                                 CssClass="dashboard-form-control">
                    </asp:TextBox>
                </div>

                <div class="staff-form-group">
                    <label class="staff-form-label"
                           for="<%= txtNewUsername.ClientID %>">
                        Username
                    </label>

                    <asp:TextBox ID="txtNewUsername"
                                 runat="server"
                                 CssClass="dashboard-form-control"
                                 MaxLength="50">
                    </asp:TextBox>
                </div>

                <div class="staff-form-group">
                    <label class="staff-form-label"
                           for="<%= txtNewPassword.ClientID %>">
                        Initial Password
                    </label>

                    <div class="staff-password-input">
                        <asp:TextBox
                            ID="txtNewPassword"
                            runat="server"
                            CssClass="dashboard-form-control"
                            TextMode="Password"
                            MaxLength="100">
                        </asp:TextBox>

                        <button type="button"
                                class="staff-password-toggle"
                                aria-label="Show password"
                                aria-pressed="false"
                                title="Show password"
                                onclick="toggleStaffPassword('<%= txtNewPassword.ClientID %>', this);">
                            <i class="bi bi-eye"></i>
                        </button>
                    </div>

                    <span class="staff-form-help">
                        Password must contain at least 8 characters, including uppercase, lowercase, a number and a special character.
                    </span>
                </div>

                <div class="staff-form-group">
                    <label class="staff-form-label"
                           for="<%= txtConfirmNewPassword.ClientID %>">
                        Confirm Password
                    </label>

                    <div class="staff-password-input">
                        <asp:TextBox
                            ID="txtConfirmNewPassword"
                            runat="server"
                            CssClass="dashboard-form-control"
                            TextMode="Password"
                            MaxLength="100">
                        </asp:TextBox>

                        <button type="button"
                                class="staff-password-toggle"
                                aria-label="Show password"
                                aria-pressed="false"
                                title="Show password"
                                onclick="toggleStaffPassword('<%= txtConfirmNewPassword.ClientID %>', this);">
                            <i class="bi bi-eye"></i>
                        </button>
                    </div>

                    <span class="staff-form-help">
                        Re-enter the initial password to confirm it.
                    </span>
                </div>

            </div>

            <div class="staff-form-actions">

                <asp:Button ID="btnAddStaff"
                            runat="server"
                            Text="Create Staff Account"
                            CssClass="dashboard-btn dashboard-btn-success"
                            OnClick="btnAddStaff_Click"
                            OnClientClick="return validateNewStaff();" />

                <asp:Button ID="btnClearNewStaff"
                            runat="server"
                            Text="Clear"
                            CssClass="dashboard-btn dashboard-btn-secondary"
                            OnClick="btnClearNewStaff_Click"
                            CausesValidation="false" />

            </div>

        </asp:Panel>

    </div>


    <!-- CLIENT-SIDE VALIDATION -->

    <script type="text/javascript">
        (function () {
            "use strict";

            /*
             * Map server-generated ASP.NET IDs to their client-side IDs.
             */
            var fields = {
                editName: "<%= txtEditFullName.ClientID %>",
                editPhone: "<%= txtEditPhone.ClientID %>",
                editEmail: "<%= txtEditEmail.ClientID %>",
                editSalary: "<%= txtEditSalary.ClientID %>",
                editRole: "<%= ddlEditRole.ClientID %>",

                newName: "<%= txtNewFullName.ClientID %>",
                newRole: "<%= ddlNewRole.ClientID %>",
                newPhone: "<%= txtNewPhone.ClientID %>",
                newEmail: "<%= txtNewEmail.ClientID %>",
                newSalary: "<%= txtNewSalary.ClientID %>",
                newUsername: "<%= txtNewUsername.ClientID %>",
                newPassword: "<%= txtNewPassword.ClientID %>",
                confirmNewPassword: "<%= txtConfirmNewPassword.ClientID %>",

                resetPassword: "<%= txtResetPassword.ClientID %>",
                confirmResetPassword: "<%= txtConfirmResetPassword.ClientID %>"
            };

            function getField(key) {
                return document.getElementById(fields[key]);
            }

            function getErrorElement(input) {
                if (!input) {
                    return null;
                }

                var errorId = input.id + "_clientError";
                var error = document.getElementById(errorId);

                if (error) {
                    return error;
                }

                error = document.createElement("span");
                error.id = errorId;
                error.className = "staff-client-error";
                error.setAttribute("aria-live", "polite");
                error.hidden = true;

                input.insertAdjacentElement("afterend", error);

                return error;
            }

            function showError(input, message) {
                if (!input) {
                    return false;
                }

                var error = getErrorElement(input);

                input.classList.remove("staff-client-valid");

                if (message) {
                    input.classList.add("staff-client-invalid");
                    input.setAttribute("aria-invalid", "true");

                    error.textContent = message;
                    error.hidden = false;

                    return false;
                }

                input.classList.remove("staff-client-invalid");
                input.setAttribute("aria-invalid", "false");

                error.textContent = "";
                error.hidden = true;

                if (input.value.trim() !== "") {
                    input.classList.add("staff-client-valid");
                }

                return true;
            }

            function valueOf(key) {
                var input = getField(key);

                return input ? input.value.trim() : "";
            }

            function passwordOf(key) {
                var input = getField(key);

                return input ? input.value : "";
            }

            function validateRequired(key, label) {
                var input = getField(key);

                if (!input) {
                    return true;
                }

                if (input.value.trim() === "") {
                    return showError(input, label + " is required.");
                }

                return showError(input, "");
            }

            function validateRole(key, label) {
                var input = getField(key);

                if (!input) {
                    return true;
                }

                if (!input.value) {
                    return showError(input, "Select a " + label + ".");
                }

                return showError(input, "");
            }

            function validatePhone(key) {
                var input = getField(key);

                if (!input) {
                    return true;
                }

                var phone = input.value.trim();

                if (phone === "") {
                    return showError(input, "Phone number is required.");
                }

                if (!/^[0-9]{10}$/.test(phone)) {
                    return showError(input, "Enter a valid 10-digit phone number.");
                }

                return showError(input, "");
            }

            function validateEmail(key) {
                var input = getField(key);

                if (!input) {
                    return true;
                }

                var email = input.value.trim();

                if (email === "") {
                    return showError(input, "Email address is required.");
                }

                /*
                 * Client-side format check. The C# MailAddress
                 * validation remains authoritative on the server.
                 */
                var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

                if (!emailPattern.test(email)) {
                    return showError(input, "Enter a valid email address.");
                }

                return showError(input, "");
            }

            function validateSalary(key) {
                var input = getField(key);

                if (!input) {
                    return true;
                }

                var salary = input.value.trim();

                if (salary === "") {
                    return showError(input, "Salary is required.");
                }

                /*
                 * Accept ordinary decimal numbers, including values
                 * such as 1000, 1000.00 and 1,000.00.
                 */
                var normalized = salary.replace(/,/g, "");

                if (!/^\d+(\.\d+)?$/.test(normalized)) {
                    return showError(input, "Enter a valid salary.");
                }

                var numericSalary = Number(normalized);

                if (!isFinite(numericSalary) || numericSalary <= 0) {
                    return showError(input, "Salary must be greater than zero.");
                }

                return showError(input, "");
            }

            function validatePasswordComplexity(key, label) {
                var input = getField(key);

                if (!input) {
                    return true;
                }

                var password = input.value;

                if (password.length === 0) {
                    return showError(input, label + " is required.");
                }

                if (password.length < 8) {
                    return showError(input, "Password must contain at least 8 characters.");
                }

                var hasUpper = /[A-Z]/.test(password);
                var hasLower = /[a-z]/.test(password);
                var hasDigit = /[0-9]/.test(password);
                var hasSpecial = /[^A-Za-z0-9]/.test(password);

                if (!hasUpper || !hasLower || !hasDigit || !hasSpecial) {
                    return showError(
                        input,
                        "Password must include uppercase, lowercase, a number and a special character."
                    );
                }

                return showError(input, "");
            }

            function validatePasswordMatch(passwordKey, confirmKey, label) {
                var passwordInput = getField(passwordKey);
                var confirmInput = getField(confirmKey);

                if (!passwordInput || !confirmInput) {
                    return true;
                }

                if (confirmInput.value.length === 0) {
                    return showError(confirmInput, "Confirm the " + label + ".");
                }

                if (passwordInput.value !== confirmInput.value) {
                    return showError(confirmInput, "The passwords do not match.");
                }

                return showError(confirmInput, "");
            }

            function validateEditName() {
                return validateRequired("editName", "Full name");
            }

            function validateNewName() {
                return validateRequired("newName", "Full name");
            }

            function validateEditDetails() {
                var valid = true;

                if (!validateEditName()) {
                    valid = false;
                }

                if (!validateRole("editRole", "role")) {
                    valid = false;
                }

                if (!validatePhone("editPhone")) {
                    valid = false;
                }

                if (!validateEmail("editEmail")) {
                    valid = false;
                }

                if (!validateSalary("editSalary")) {
                    valid = false;
                }

                if (!valid) {
                    focusFirstInvalid([
                        "editName",
                        "editRole",
                        "editPhone",
                        "editEmail",
                        "editSalary"
                    ]);
                }

                return valid;
            }

            function validateNewStaff() {
                var valid = true;

                if (!validateNewName()) {
                    valid = false;
                }

                if (!validateRole("newRole", "role")) {
                    valid = false;
                }

                if (!validatePhone("newPhone")) {
                    valid = false;
                }

                if (!validateEmail("newEmail")) {
                    valid = false;
                }

                if (!validateSalary("newSalary")) {
                    valid = false;
                }

                if (!validateRequired("newUsername", "Username")) {
                    valid = false;
                }

                if (!validatePasswordComplexity("newPassword", "Initial password")) {
                    valid = false;
                }

                if (!validatePasswordMatch(
                    "newPassword",
                    "confirmNewPassword",
                    "initial password"
                )) {
                    valid = false;
                }

                if (!valid) {
                    focusFirstInvalid([
                        "newName",
                        "newRole",
                        "newPhone",
                        "newEmail",
                        "newSalary",
                        "newUsername",
                        "newPassword",
                        "confirmNewPassword"
                    ]);
                }

                return valid;
            }

            function validateResetPassword() {
                var valid = true;

                if (!validatePasswordComplexity("resetPassword", "New password")) {
                    valid = false;
                }

                if (!validatePasswordMatch(
                    "resetPassword",
                    "confirmResetPassword",
                    "new password"
                )) {
                    valid = false;
                }

                if (!valid) {
                    focusFirstInvalid([
                        "resetPassword",
                        "confirmResetPassword"
                    ]);

                    return false;
                }

                return window.confirm(
                    "Reset the password for this staff account?"
                );
            }

            function focusFirstInvalid(keys) {
                for (var i = 0; i < keys.length; i++) {
                    var input = getField(keys[i]);

                    if (input &&
                        input.getAttribute("aria-invalid") === "true") {
                        input.focus();
                        return;
                    }
                }
            }

            /*
             * Attach blur validation and live correction feedback.
             * Password confirmation is rechecked whenever either
             * password field changes.
             */
            function attachValidation(key, validator, relatedKeys) {
                var input = getField(key);

                if (!input) {
                    return;
                }

                input.addEventListener("blur", function () {
                    validator();

                    if (relatedKeys) {
                        relatedKeys();
                    }
                });

                input.addEventListener("input", function () {
                    /*
                     * Show feedback immediately once the user has
                     * started correcting a field.
                     */
                    if (input.getAttribute("aria-invalid") === "true" ||
                        input.classList.contains("staff-client-valid")) {
                        validator();
                    }

                    if (relatedKeys) {
                        relatedKeys();
                    }
                });

                if (input.tagName === "SELECT") {
                    input.addEventListener("change", validator);
                }
            }

            function validateNewPasswordAndMatch() {
                validatePasswordComplexity("newPassword", "Initial password");

                var confirmation = getField("confirmNewPassword");

                if (confirmation &&
                    (confirmation.value.length > 0 ||
                        confirmation.getAttribute("aria-invalid") === "true")) {
                    validatePasswordMatch(
                        "newPassword",
                        "confirmNewPassword",
                        "initial password"
                    );
                }
            }

            function validateNewConfirmation() {
                validatePasswordMatch(
                    "newPassword",
                    "confirmNewPassword",
                    "initial password"
                );
            }

            function validateResetPasswordAndMatch() {
                validatePasswordComplexity("resetPassword", "New password");

                var confirmation = getField("confirmResetPassword");

                if (confirmation &&
                    (confirmation.value.length > 0 ||
                        confirmation.getAttribute("aria-invalid") === "true")) {
                    validatePasswordMatch(
                        "resetPassword",
                        "confirmResetPassword",
                        "new password"
                    );
                }
            }

            function validateResetConfirmation() {
                validatePasswordMatch(
                    "resetPassword",
                    "confirmResetPassword",
                    "new password"
                );
            }

            function initializeValidation() {
                attachValidation("editName", validateEditName);
                attachValidation("editPhone", function () {
                    validatePhone("editPhone");
                });
                attachValidation("editEmail", function () {
                    validateEmail("editEmail");
                });
                attachValidation("editSalary", function () {
                    validateSalary("editSalary");
                });
                attachValidation("editRole", function () {
                    validateRole("editRole", "role");
                });

                attachValidation("newName", validateNewName);
                attachValidation("newRole", function () {
                    validateRole("newRole", "role");
                });
                attachValidation("newPhone", function () {
                    validatePhone("newPhone");
                });
                attachValidation("newEmail", function () {
                    validateEmail("newEmail");
                });
                attachValidation("newSalary", function () {
                    validateSalary("newSalary");
                });
                attachValidation("newUsername", function () {
                    validateRequired("newUsername", "Username");
                });

                attachValidation(
                    "newPassword",
                    validateNewPasswordAndMatch
                );

                attachValidation(
                    "confirmNewPassword",
                    validateNewConfirmation
                );

                attachValidation(
                    "resetPassword",
                    validateResetPasswordAndMatch
                );

                attachValidation(
                    "confirmResetPassword",
                    validateResetConfirmation
                );
            }

            /*
             * Expose the functions used by ASP.NET OnClientClick.
             */
            window.validateEditStaffDetails = validateEditDetails;
            window.validateNewStaff = validateNewStaff;
            window.validateStaffPasswordReset = validateResetPassword;

            /*
             * Preserve password visibility toggle behaviour.
             */
            window.toggleStaffPassword = function (inputId, button) {
                var input = document.getElementById(inputId);

                if (!input || !button) {
                    return;
                }

                var icon = button.querySelector("i");

                if (input.type === "password") {
                    input.type = "text";

                    button.setAttribute("aria-label", "Hide password");
                    button.setAttribute("aria-pressed", "true");
                    button.setAttribute("title", "Hide password");

                    if (icon) {
                        icon.className = "bi bi-eye-slash";
                    }
                } else {
                    input.type = "password";

                    button.setAttribute("aria-label", "Show password");
                    button.setAttribute("aria-pressed", "false");
                    button.setAttribute("title", "Show password");

                    if (icon) {
                        icon.className = "bi bi-eye";
                    }
                }
            };

            if (document.readyState === "loading") {
                document.addEventListener(
                    "DOMContentLoaded",
                    initializeValidation
                );
            } else {
                initializeValidation();
            }

        })();
    </script>


</div>

</div>
</asp:Content>
