
<%@ Page Title="Logout"
    Language="C#"
    MasterPageFile="~/MasterPages/Auth.Master"
    AutoEventWireup="true"
    CodeBehind="Logout.aspx.cs"
    Inherits="MajorProject.Web.Account.Logout" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>

    <section class="tw-services-section"
        style="min-height: 85vh; display: flex; align-items: center;">

        <div class="tw-blueprint-grid" aria-hidden="true"></div>

        <div class="tw-container">

            <div class="row justify-content-center text-center">

                <div class="col-lg-6 col-md-8">

                    <div class="tw-glass-card tw-form-card p-5">

                        <!-- Logout Icon -->
                        <div class="tw-brand-icon mx-auto mb-4">
                            <i class="bi bi-box-arrow-right text-warning fs-3"></i>
                        </div>

                        <!-- Heading -->
                        <h2 class="tw-text-primary fw-bold mb-3">
                            You Have Been Logged Out
                        </h2>

                        <!-- Message -->
                        <p class="tw-text-muted mb-4">
                            Thank you for using THY WILL Electrical & Construction.
                            Your session has been closed securely.
                        </p>

                        <!-- Navigation Buttons -->
                        <div class="d-flex justify-content-center gap-3 flex-wrap">

                            <!-- Return Home -->
                            <asp:HyperLink
                                ID="lnkReturnHome"
                                runat="server"
                                NavigateUrl="~/Default.aspx"
                                CssClass="tw-btn-glass">
                                <i class="bi bi-house me-2"></i>
                                Return to Home
                            </asp:HyperLink>

                            <!-- Sign In -->
                            <asp:HyperLink
                                ID="lnkSignIn"
                                runat="server"
                                NavigateUrl="~/Account/Login.aspx"
                                CssClass="tw-btn-primary">
                                <i class="bi bi-box-arrow-in-right me-2"></i>
                                Sign In Again
                            </asp:HyperLink>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </section>


</div>
</asp:Content>