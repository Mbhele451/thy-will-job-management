<%@ Page Title="Access Denied" Language="C#" MasterPageFile="~/MasterPages/Auth.Master" AutoEventWireup="true" CodeBehind="AccessDenied.aspx.cs" Inherits="MajorProject.Web.Account.AccessDenied" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
<div class="tw-page-stage" data-page-stage>

    <section class="tw-services-section"
             style="min-height: 85vh; display: flex; align-items: center;">

        <div class="tw-blueprint-grid" aria-hidden="true"></div>

        <div class="tw-container">

            <div class="row justify-content-center text-center">

                <div class="col-lg-6 col-md-8">

                    <div class="tw-glass-card tw-form-card p-5">

                        <div class="tw-brand-icon mx-auto mb-4"
                             style="border-color: var(--color-border);">

                            <i class="bi bi-shield-x text-danger fs-3"></i>

                        </div>

                        <h2 class="tw-text-primary fw-bold mb-3">
                            Access Denied
                        </h2>

                        <p class="tw-text-muted mb-4">
                            You do not have the required permissions to access this page.
                            If you believe this is an error, please contact your administrator.
                        </p>

                        <div class="d-flex justify-content-center gap-3">

                            <asp:HyperLink 
                                ID="lnkHome"
                                runat="server"
                                NavigateUrl="~/Default.aspx"
                                CssClass="tw-btn-glass">
                                Return Home
                            </asp:HyperLink>

                            <asp:HyperLink 
                                ID="lnkLogin"
                                runat="server"
                                NavigateUrl="~/Account/Login.aspx"
                                CssClass="tw-btn-primary">
                                Sign In
                            </asp:HyperLink>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </section>


</div>
</asp:Content>