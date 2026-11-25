<%@ Page Title="Forgot Password"
    Language="C#"
    MasterPageFile="~/MasterPages/Auth.Master"
    AutoEventWireup="true"
    Async="true"
    CodeBehind="ForgotPassword.aspx.cs"
    Inherits="MajorProject.Web.Account.ForgotPassword" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>

    <section class="tw-services-section tw-auth-forgot-section">

        <div class="tw-blueprint-grid" aria-hidden="true"></div>

        <div class="tw-container">

            <div class="row justify-content-center">

                <div class="col-12 col-md-8 col-lg-5">

                    <div class="tw-glass-card tw-form-card tw-forgot-card">

                        <!-- ============================================= -->
                        <!-- HEADER                                        -->
                        <!-- ============================================= -->

                        <div class="text-center mb-4">

                            <div class="tw-brand-icon mx-auto mb-3">
                                <i class="bi bi-shield-lock"></i>
                            </div>

                            <h2 class="tw-text-primary fw-bold mb-2">
                                Forgot Password?
                            </h2>

                            <p class="tw-text-muted mb-0">
                                Enter the email address associated with
                                your account and we'll send you a
                                verification code.
                            </p>

                        </div>


                        <!-- ============================================= -->
                        <!-- MESSAGE                                        -->
                        <!-- ============================================= -->

                        <asp:Label
                            ID="lblMessage"
                            runat="server"
                            CssClass="tw-booking-message d-block mb-3"
                            Visible="false"
                            role="alert">
                        </asp:Label>


                        <!-- ============================================= -->
                        <!-- EMAIL ADDRESS                                  -->
                        <!-- ============================================= -->

                        <div class="mb-4">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtEmail.ClientID %>">
                                Email Address
                            </label>

                            <div class="position-relative">

                                <span
                                    class="position-absolute top-50 translate-middle-y ms-3 text-muted"
                                    aria-hidden="true">
                                    <i class="bi bi-envelope"></i>
                                </span>

                                <asp:TextBox
                                    ID="txtEmail"
                                    runat="server"
                                    CssClass="form-control tw-glass-input ps-5"
                                    TextMode="Email"
                                    placeholder="Enter your email address"
                                    autocomplete="email"
                                    MaxLength="100">
                                </asp:TextBox>

                            </div>

                            <div class="tw-field-hint mt-2">

                                <i class="bi bi-info-circle me-1"></i>

                                Use the email address registered to your
                                account.

                            </div>

                        </div>


                        <!-- ============================================= -->
                        <!-- SEND VERIFICATION CODE                        -->
                        <!-- ============================================= -->

                        <asp:Button
                            ID="btnSendOtp"
                            runat="server"
                            Text="Send Verification Code"
                            CssClass="tw-btn-primary-gradient w-100 py-3"
                            OnClick="btnSendOtp_Click" />


                        <!-- ============================================= -->
                        <!-- SECURITY NOTE                                 -->
                        <!-- ============================================= -->

                        <div class="tw-password-requirements mt-4 mb-4">

                            <div class="tw-requirement-title">
                                <i class="bi bi-shield-check me-2"></i>
                                Secure password recovery
                            </div>

                            <div class="tw-field-hint mt-2">
                                If an account matches the email address
                                provided, a verification code will be
                                sent to its registered email address.
                            </div>

                        </div>


                        <!-- ============================================= -->
                        <!-- DIVIDER                                        -->
                        <!-- ============================================= -->

                        <div class="tw-step-divider"></div>


                        <!-- ============================================= -->
                        <!-- BACK TO LOGIN                                  -->
                        <!-- ============================================= -->

                        <div class="text-center">

                            <span class="tw-text-muted">
                                Remember your password?
                            </span>

                            <a
                                href="~/Account/Login.aspx"
                                runat="server"
                                class="tw-card-link ms-1">

                                Sign in

                                <i class="bi bi-arrow-right ms-1"></i>

                            </a>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </section>


</div>
</asp:Content>