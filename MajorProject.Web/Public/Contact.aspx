<%@ Page
    Title="Contact Us"
    Language="C#"
    MasterPageFile="~/MasterPages/Public.Master"
    AutoEventWireup="true"
    Async="true"
    CodeBehind="Contact.aspx.cs"
    Inherits="MajorProject.Web.Contact" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
<section class="pr-page-hero hp-wrap"><div class="pr-heading" data-reveal><p class="hp-kicker">Contact THY WILL</p><h1>A conversation first.<br /><span>The right next step.</span></h1><p class="pr-lead">Tell us about your property, the electrical work you need and your location.</p></div><figure class="pr-hero-picture" data-reveal><img src="<%= ResolveUrl("~/Content/images/editorial/diagnostics.jpg") %>" alt="Electrical services and installation" loading="eager" width="1200" height="900" /></figure></section><section class="hp-section hp-wrap pr-contact-grid"><aside data-reveal><p class="hp-kicker">Talk to our team</p><h2>Let’s get<br /><span>the details right.</span></h2><dl class="pr-contact-list"><div><dt>Telephone</dt><dd><a href="tel:+27837585301">083 758 5301</a></dd></div><div><dt>Email</dt><dd><a href="mailto:contact@example.invalid">contact@example.invalid</a></dd></div><div><dt>Address</dt><dd>267 Kenyon Howden Road<br />Woodlands, Durban, 4004<br />KwaZulu-Natal, South Africa</dd></div></dl><p class="pr-help">Include your location, the type of property and a short description of the work required. For urgent enquiries, phone the team directly.</p></aside><div class="pr-contact-form" id="contact-form" data-reveal><p class="hp-kicker">Send an enquiry</p><h2>What do you have<br /><span>in mind?</span></h2><!-- Server-side message -->
                    <asp:Label ID="lblContactMessage"
                               runat="server"
                               CssClass="tw-booking-message mb-3"
                               Visible="false"
                               role="status" />


                    <!-- Validation Summary -->
                    <asp:ValidationSummary ID="vsContact"
                                           runat="server"
                                           ValidationGroup="Contact"
                                           CssClass="tw-booking-message mb-3"
                                           DisplayMode="BulletList"
                                           HeaderText="Please correct the following:"
                                           Visible="false" />


                    <!-- Form -->
                    <div class="row g-3">

                        <!-- Full Name -->
                        <div class="col-md-6">

                            <label class="form-label tw-form-label"
                                   for="<%= txtContactName.ClientID %>">
                                Your Name *
                            </label>

                            <asp:TextBox ID="txtContactName"
                                         runat="server"
                                         CssClass="form-control tw-glass-input"
                                         MaxLength="100"
                                         placeholder="e.g. Lerato Mbhele" />

                            <asp:RequiredFieldValidator
                                ID="rfvContactName"
                                runat="server"
                                ControlToValidate="txtContactName"
                                ValidationGroup="Contact"
                                ErrorMessage="Please enter your name."
                                CssClass="text-danger small mt-1"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>

                            <small class="tw-text-muted d-block mt-1">
                                Enter your full name.
                            </small>

                        </div>


                        <!-- Phone -->
                        <div class="col-md-6">

                            <label class="form-label tw-form-label"
                                   for="<%= txtContactPhone.ClientID %>">
                                Phone Number *
                            </label>

                            <asp:TextBox ID="txtContactPhone"
                                         runat="server"
                                         CssClass="form-control tw-glass-input"
                                         MaxLength="20"
                                         placeholder="083 123 4567" />

                            <asp:RequiredFieldValidator
                                ID="rfvContactPhone"
                                runat="server"
                                ControlToValidate="txtContactPhone"
                                ValidationGroup="Contact"
                                ErrorMessage="Please enter your phone number."
                                CssClass="text-danger small mt-1"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>

                            <small class="tw-text-muted d-block mt-1">
                                Include a number we can reach you on.
                            </small>

                        </div>


                        <!-- Email -->
                        <div class="col-12">

                            <label class="form-label tw-form-label"
                                   for="<%= txtContactEmail.ClientID %>">
                                Email Address *
                            </label>

                            <asp:TextBox ID="txtContactEmail"
                                         runat="server"
                                         TextMode="Email"
                                         CssClass="form-control tw-glass-input"
                                         MaxLength="200"
                                         placeholder="name@example.com" />

                            <asp:RequiredFieldValidator
                                ID="rfvContactEmail"
                                runat="server"
                                ControlToValidate="txtContactEmail"
                                ValidationGroup="Contact"
                                ErrorMessage="Please enter your email address."
                                CssClass="text-danger small mt-1"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>

                            <asp:RegularExpressionValidator
                                ID="revContactEmail"
                                runat="server"
                                ControlToValidate="txtContactEmail"
                                ValidationGroup="Contact"
                                ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$"
                                ErrorMessage="Please enter a valid email address."
                                CssClass="text-danger small mt-1"
                                Display="Dynamic">
                            </asp:RegularExpressionValidator>

                        </div>


                        <!-- Service -->
                        <div class="col-12">

                            <label class="form-label tw-form-label"
                                   for="<%= ddlSubject.ClientID %>">
                                What Can We Help You With? *
                            </label>

                            <asp:DropDownList ID="ddlSubject"
                                              runat="server"
                                              CssClass="form-select tw-glass-input">

                                <asp:ListItem Text="Select an enquiry type"
                                              Value=""
                                              Selected="True" />

                                <asp:ListItem Text="General Inquiry"
                                              Value="General Inquiry" />

                                <asp:ListItem Text="Electrical Installation Quote"
                                              Value="Electrical Installation Quote" />

                                <asp:ListItem Text="Electrical Maintenance &amp; Repairs"
                                              Value="Electrical Maintenance and Repairs" />

                                <asp:ListItem Text="DB Upgrade / High-Load Circuit"
                                              Value="DB Upgrade and High-Load Circuit" />

                                <asp:ListItem Text="Geyser / EV Charger Installation"
                                              Value="Geyser and EV Charger Installation" />

                                <asp:ListItem Text="Electrical Compliance / CoC"
                                              Value="Electrical Compliance and CoC" />

                                <asp:ListItem Text="Solar &amp; Backup Power"
                                              Value="Solar and Backup Power" />

                                <asp:ListItem Text="Emergency Fault Finding"
                                              Value="Emergency Fault Finding" />

                            </asp:DropDownList>

                            <asp:RequiredFieldValidator
                                ID="rfvSubject"
                                runat="server"
                                ControlToValidate="ddlSubject"
                                InitialValue=""
                                ValidationGroup="Contact"
                                ErrorMessage="Please select an enquiry type."
                                CssClass="text-danger small mt-1"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>

                            <small class="tw-text-muted d-block mt-1">
                                Select the option closest to what you need.
                            </small>

                        </div>


                        <!-- Message -->
                        <div class="col-12">

                            <label class="form-label tw-form-label"
                                   for="<%= txtMessage.ClientID %>">
                                Tell Us More *
                            </label>

                            <asp:TextBox ID="txtMessage"
                                         runat="server"
                                         TextMode="MultiLine"
                                         Rows="6"
                                         MaxLength="1000"
                                         CssClass="form-control tw-glass-input"
                                         placeholder="Please describe the electrical work you need, the problem you are experiencing, your location, or any other useful information." />

                            <asp:RequiredFieldValidator
                                ID="rfvMessage"
                                runat="server"
                                ControlToValidate="txtMessage"
                                ValidationGroup="Contact"
                                ErrorMessage="Please enter some details about your enquiry."
                                CssClass="text-danger small mt-1"
                                Display="Dynamic">
                            </asp:RequiredFieldValidator>

                            <small class="tw-text-muted d-block mt-1">
                                The more information you provide, the easier it
                                is for us to understand your request.
                            </small>

                        </div>

                    </div>


                    <!-- Submit -->
                    <asp:Button ID="btnSendMessage"
                                runat="server"
                                Text="Send My Enquiry"
                                CssClass="tw-btn-primary-gradient w-100 py-3 mt-4"
                                ValidationGroup="Contact"
                                CausesValidation="true"
                                OnClick="btnSendMessage_Click" /></div></section>
</asp:Content>
