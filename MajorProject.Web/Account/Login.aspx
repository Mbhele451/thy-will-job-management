<%@ Page Title="Login"
Language="C#"
MasterPageFile="~/MasterPages/Auth.Master"
AutoEventWireup="true"
CodeBehind="Login.aspx.cs"
Inherits="MajorProject.Web.Account.Login" %>

<asp:Content
ID="Content1"
ContentPlaceHolderID="MainContent"
runat="server">
<div class="tw-page-stage" data-page-stage>

<section class="tw-auth-section">

    <!-- Ambient background grid -->
    <div
        class="tw-blueprint-grid"
        aria-hidden="true">
    </div>

    <div class="tw-container">

        <div class="tw-auth-wrap">

            <!-- =================================================
                 LOGIN CARD
                 ================================================= -->

            <div class="tw-glass-card tw-auth-card">

                <!-- Card glow -->
                <div
                    class="tw-auth-card-glow"
                    aria-hidden="true">
                </div>

                <!-- =================================================
                     HEADER
                     ================================================= -->

                <div class="tw-auth-head">

                    <div
                        class="tw-auth-icon"
                        aria-hidden="true">

                        <i class="bi bi-lightning-charge-fill"></i>

                    </div>

                    <div class="tw-auth-heading">

                        <span class="tw-auth-eyebrow">
                            SECURE ACCESS
                        </span>

                        <h1>
                            Sign In
                        </h1>

                        <p>
                            Sign in to access your THY WILL account.
                        </p>

                    </div>

                </div>

                <!-- =================================================
                     RESET SUCCESS MESSAGE
                     ================================================= -->

                <asp:Label
                    ID="lblResetSuccess"
                    runat="server"
                    Text="Your password was reset successfully. Please log in with your new password."
                    CssClass="tw-auth-alert tw-auth-alert-success"
                    Visible="false"
                    role="status">
                </asp:Label>

                <!-- =================================================
                     USERNAME / EMAIL
                     ================================================= -->

                <div class="tw-auth-field">

                    <label
                        class="tw-form-label"
                        for="<%= txtUsername.ClientID %>">

                        <span>
                            Username or Email
                        </span>

                    </label>

                    <div class="tw-input-wrap">

                        <i
                            class="bi bi-person"
                            aria-hidden="true">
                        </i>

                        <asp:TextBox
                            ID="txtUsername"
                            runat="server"
                            CssClass="form-control tw-glass-input"
                            autocomplete="username"
                            placeholder="Enter your username or email">
                        </asp:TextBox>

                    </div>

                </div>

                <!-- =================================================
                     PASSWORD
                     ================================================= -->

                <div class="tw-auth-field">

                    <label
                        class="tw-form-label"
                        for="<%= txtPassword.ClientID %>">

                        <span>
                            Password
                        </span>

                    </label>

                    <div class="tw-input-wrap position-relative">

                        <i
                            class="bi bi-lock"
                            aria-hidden="true">
                        </i>

                        <asp:TextBox
                            ID="txtPassword"
                            runat="server"
                            TextMode="Password"
                            CssClass="form-control tw-glass-input pe-5"
                            autocomplete="current-password"
                            placeholder="Enter your password">
                        </asp:TextBox>

                        <button
                            type="button"
                            class="btn position-absolute top-50 end-0 translate-middle-y border-0 bg-transparent tw-text-primary pe-3"
                            style="z-index: 5;"
                            aria-label="Show password"
                            aria-pressed="false"
                            title="Show password"
                            onclick="togglePassword('<%= txtPassword.ClientID %>', this)">

                            <i
                                class="bi bi-eye"
                                aria-hidden="true">
                            </i>

                        </button>

                    </div>

                </div>

                <!-- =================================================
                     ERROR
                     ================================================= -->

                <asp:Label
                    ID="lblError"
                    runat="server"
                    CssClass="tw-auth-alert tw-auth-alert-error"
                    Visible="false"
                    role="alert">
                </asp:Label>

                <!-- =================================================
                     LOGIN BUTTON
                     ================================================= -->

                <asp:Button
                    ID="btnLogin"
                    runat="server"
                    Text="Sign In"
                    CssClass="tw-btn-primary-gradient tw-auth-submit"
                    OnClick="btnLogin_Click" />

                <!-- =================================================
                     FORGOT PASSWORD
                     ================================================= -->

                <div class="tw-auth-forgot">

                    <span>
                        Forgot your password?
                    </span>

                    <asp:HyperLink
                        ID="lnkForgotPassword"
                        runat="server"
                        NavigateUrl="~/Account/ForgotPassword.aspx">

                        Reset it

                    </asp:HyperLink>

                </div>

                <!-- Bottom glass line -->

                <div
                    class="tw-auth-card-line"
                    aria-hidden="true">
                </div>

            </div>

            <!-- =================================================
                 REGISTER / SERVICE
                 ================================================= -->

            <div class="tw-auth-secondary">

                <span>
                    New customer?
                </span>

                <a
                    href="~/Account/Register.aspx"
                    runat="server">

                    Create a profile

                </a>

                <span class="tw-auth-divider">
                    or
                </span>

                <a
                    href="~/Default.aspx#request-service"
                    runat="server">

                    Request a service

                </a>

            </div>

        </div>

    </div>

</section>

<script type="text/javascript">
    function togglePassword(fieldId, btn) {
        var input = document.getElementById(fieldId);

        if (!input || !btn) {
            return;
        }

        var icon = btn.querySelector("i");
        var isPassword = input.type === "password";

        if (isPassword) {
            input.type = "text";

            btn.setAttribute("aria-label", "Hide password");
            btn.setAttribute("aria-pressed", "true");
            btn.setAttribute("title", "Hide password");

            if (icon) {
                icon.classList.remove("bi-eye");
                icon.classList.add("bi-eye-slash");
            }
        }
        else {
            input.type = "password";

            btn.setAttribute("aria-label", "Show password");
            btn.setAttribute("aria-pressed", "false");
            btn.setAttribute("title", "Show password");

            if (icon) {
                icon.classList.remove("bi-eye-slash");
                icon.classList.add("bi-eye");
            }
        }
    }
</script>


</div>
</asp:Content>
