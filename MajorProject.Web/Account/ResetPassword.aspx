<%@ Page Title="Reset Password"
    Language="C#"
    MasterPageFile="~/MasterPages/Auth.Master"
    AutoEventWireup="true"
    CodeBehind="ResetPassword.aspx.cs"
    Inherits="MajorProject.Web.Account.ResetPassword" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>

    <section class="tw-services-section tw-auth-reset-section">

        <div class="tw-blueprint-grid" aria-hidden="true"></div>

        <div class="tw-container">

            <div class="row justify-content-center">

                <div class="col-12 col-md-8 col-lg-5">

                    <div class="tw-glass-card tw-form-card tw-reset-card">

                        <!-- HEADER -->

                        <div class="text-center mb-4">

                            <div class="tw-brand-icon mx-auto mb-3">
                                <i class="bi bi-shield-lock"></i>
                            </div>

                            <h2 class="tw-text-primary fw-bold mb-2">
                                Reset Password
                            </h2>

                            <p class="tw-text-muted mb-0">
                                Enter the 6-digit verification code
                                sent to your registered contact.
                            </p>

                        </div>


                        <!-- MESSAGE -->

                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            CssClass="tw-booking-message d-block mb-3"
                            Visible="false"
                            role="alert" />


                        <!-- VERIFICATION CODE -->

                        <div class="mb-4">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtOtp.ClientID %>">

                                Verification Code

                            </label>

                            <asp:TextBox
                                ID="txtOtp"
                                runat="server"
                                CssClass="form-control tw-glass-input tw-otp-input"
                                MaxLength="6"
                                placeholder="000000"
                                autocomplete="one-time-code"
                                inputmode="numeric" />

                            <div class="tw-field-hint">
                                Enter the 6-digit code you received.
                            </div>

                        </div>


                        <!-- NEW PASSWORD -->

                        <div class="mb-3">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtNewPassword.ClientID %>">

                                New Password

                            </label>

                            <div class="tw-password-wrapper">

                                <asp:TextBox
                                    ID="txtNewPassword"
                                    runat="server"
                                    TextMode="Password"
                                    CssClass="form-control tw-glass-input"
                                    placeholder="Enter your new password"
                                    autocomplete="new-password" />

                            </div>

                        </div>


                        <!-- CONFIRM PASSWORD -->

                        <div class="mb-4">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtConfirmPassword.ClientID %>">

                                Confirm New Password

                            </label>

                            <asp:TextBox
                                ID="txtConfirmPassword"
                                runat="server"
                                TextMode="Password"
                                CssClass="form-control tw-glass-input"
                                placeholder="Re-enter your new password"
                                autocomplete="new-password" />

                        </div>


                        <!-- PASSWORD REQUIREMENTS -->

                        <div class="tw-password-requirements mb-4">

                            <div class="tw-requirement-title">
                                <i class="bi bi-info-circle me-2"></i>
                                Password requirements
                            </div>

                            <ul>
                                <li>At least 8 characters</li>
                                <li>At least one uppercase letter</li>
                                <li>At least one lowercase letter</li>
                                <li>At least one number</li>
                                <li>At least one special character</li>
                            </ul>

                        </div>


                        <!-- SUBMIT -->

                        <asp:Button
                            ID="btnReset"
                            runat="server"
                            Text="Update Password"
                            CssClass="tw-btn-primary-gradient w-100 py-3"
                            OnClick="btnReset_Click" />


                        <!-- DIVIDER -->

                        <div class="tw-step-divider"></div>


                        <!-- BACK -->

                        <div class="text-center">

                            <a
                                href="~/Account/forgotPassword.aspx"
                                runat="server"
                                class="tw-card-link p-0">

                                <i class="bi bi-arrow-left me-1"></i>
                                Request a new verification code

                            </a>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </section>


</div>
</asp:Content>