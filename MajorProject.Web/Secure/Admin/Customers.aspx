<%@ Page
    Title="Customer Management"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Customers.aspx.cs"
    Inherits="MajorProject.Web.Admin.Customers" %>

<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>

    <div class="dashboard-page admin-customers"  data-record-tabs="secure-admin-customers" data-tab-labels="list=Customers;details=Customer profile" data-selected-id="hfSelectedCustomerID">
<asp:Label
                    ID="lblMessage"
                    runat="server"
                    CssClass="dashboard-alert"
                    Visible="false">
                </asp:Label>


        <!-- =========================================================
             PAGE HEADER
             ========================================================= -->

        <section class="dashboard-page-header">

            <div>
                <span class="dashboard-eyebrow">
                    CUSTOMER MANAGEMENT
                </span>

                <h1 class="dashboard-page-title">
                    Customers
                </h1>

                <p class="dashboard-page-description">
                    Register new customers and update existing customer profiles.
                </p>
            </div>

        </section>


        <!-- =========================================================
             CUSTOMER DIRECTORY
             ========================================================= -->

        <section class="dashboard-panel admin-customers-directory"  data-tab-pane="list">

            <div class="dashboard-panel-header">

                <div>
                    <h2 class="dashboard-panel-title">
                        Customer Directory
                    </h2>

                    <p class="dashboard-panel-description">
                        Search and select an existing customer to update their profile.
                    </p>
                </div>

            </div>


            <div class="dashboard-panel-body">

                <!-- SEARCH + SORT -->

                <div class="admin-customers-toolbar">

                    <div class="dashboard-form-group">

                        <label for="<%= txtSearch.ClientID %>">
                            Search Customers
                        </label>

                        <asp:TextBox
                            ID="txtSearch"
                            runat="server"
                            CssClass="dashboard-input"
                            placeholder="Search by customer name..."
                            AutoPostBack="true"
                            OnTextChanged="txtSearch_TextChanged">
                        </asp:TextBox>

                    </div>


                    <div class="dashboard-form-group">

                        <label for="<%= ddlSort.ClientID %>">
                            Sort By
                        </label>

                        <asp:DropDownList
                            ID="ddlSort"
                            runat="server"
                            CssClass="dashboard-input"
                            AutoPostBack="true"
                            OnSelectedIndexChanged="ddlSort_SelectedIndexChanged">

                            <asp:ListItem
                                Text="Name: A to Z"
                                Value="NameAsc" />

                            <asp:ListItem
                                Text="Name: Z to A"
                                Value="NameDesc" />

                            <asp:ListItem
                                Text="Recently Registered"
                                Value="DateDesc" />

                            <asp:ListItem
                                Text="Oldest Registered"
                                Value="DateAsc" />

                            <asp:ListItem
                                Text="Customer Type"
                                Value="CustomerType" />

                            <asp:ListItem
                                Text="Service Area"
                                Value="Area" />

                        </asp:DropDownList>

                    </div>


                    <div class="admin-customers-toolbar-action">

                        <asp:Button
                            ID="btnClearSearch"
                            runat="server"
                            Text="Clear"
                            CssClass="dashboard-btn dashboard-btn-secondary"
                            OnClick="btnClearSearch_Click" />

                    </div>

                </div>


                <!-- CUSTOMER TABLE -->

                <div class="admin-customers-table-wrapper">

                    <asp:GridView
                        ID="gvCustomers"
                        runat="server"
                        CssClass="dashboard-table"
                        AutoGenerateColumns="false"
                        GridLines="None"
                        AllowPaging="false"
                        DataKeyNames="CustomerID"
                        EmptyDataText="No customers found."
                        OnRowCommand="gvCustomers_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="CustomerID"
                                HeaderText="ID" />

                            <asp:BoundField
                                DataField="FullName"
                                HeaderText="Full Name" />

                            <asp:BoundField
                                DataField="EmailAddress"
                                HeaderText="Email Address" />

                            <asp:BoundField
                                DataField="PhoneNumber"
                                HeaderText="Phone Number" />

                            <asp:BoundField
                                DataField="CustomerType"
                                HeaderText="Type" />

                            <asp:BoundField
                                DataField="Area"
                                HeaderText="Service Area" />

                            <asp:BoundField
                                DataField="DateRegistered"
                                HeaderText="Registered"
                                DataFormatString="{0:dd MMM yyyy}" />

                            <asp:TemplateField
                                HeaderText="Action">

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="btnSelectCustomer"
                                        runat="server"
                                        CssClass="dashboard-btn dashboard-btn-small dashboard-btn-secondary"
                                        CommandName="SelectCustomer" CausesValidation="false" data-record-select="details"
                                        CommandArgument='<%# Eval("CustomerID") %>'>

                                        Load Profile

                                    </asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </section>


        <!-- =========================================================
             CUSTOMER PROFILE
             ========================================================= -->

        <section class="dashboard-panel admin-customer-profile"  data-tab-pane="details">

            <div class="dashboard-panel-header">

                <div>

                    <h2 class="dashboard-panel-title">
                        Customer Profile
                    </h2>

                    <p class="dashboard-panel-description">
                        Register a new customer or update an existing customer.
                    </p>

                </div>

            </div>


            <div class="dashboard-panel-body">

                <asp:HiddenField
                    ID="hfSelectedCustomerID"
                    runat="server" />


                <!-- CUSTOMER FORM -->

                <div class="admin-customer-form-grid">

                    <!-- FULL NAME -->

                    <div class="dashboard-form-group">

                        <label for="<%= txtCustName.ClientID %>">
                            Full Name
                        </label>

                        <asp:TextBox
                            ID="txtCustName"
                            runat="server"
                            CssClass="dashboard-input"
                            placeholder="Enter full name">
                        </asp:TextBox>

                    </div>


                    <!-- EMAIL -->

                    <div class="dashboard-form-group">

                        <label for="<%= txtEmail.ClientID %>">
                            Email Address
                        </label>

                        <asp:TextBox
                            ID="txtEmail"
                            runat="server"
                            CssClass="dashboard-input"
                            TextMode="Email"
                            placeholder="customer@example.com">
                        </asp:TextBox>

                    </div>


                    <!-- PHONE -->

                    <div class="dashboard-form-group">

                        <label for="<%= txtPhone.ClientID %>">
                            Phone Number
                        </label>

                        <asp:TextBox
                            ID="txtPhone"
                            runat="server"
                            CssClass="dashboard-input"
                            TextMode="Phone"
                            placeholder="e.g. 0821234567">
                        </asp:TextBox>

                    </div>


                    <!-- CUSTOMER TYPE -->

                    <div class="dashboard-form-group">

                        <label for="<%= ddlCustomerType.ClientID %>">
                            Customer Type
                        </label>

                        <asp:DropDownList
                            ID="ddlCustomerType"
                            runat="server"
                            CssClass="dashboard-input">

                            <asp:ListItem
                                Text="Select type"
                                Value="" />

                            <asp:ListItem
                                Text="Residential"
                                Value="Residential" />

                            <asp:ListItem
                                Text="Business"
                                Value="Business" />

                        </asp:DropDownList>

                    </div>


                    <!-- SOUTH AFRICAN ID -->

                    <div class="col-12 tw-identity-choice" data-identity-choice>
<label for="<%= ddlNationality.ClientID %>">Identity document for an individual</label>
<asp:DropDownList ID="ddlNationality" runat="server" CssClass="dashboard-input form-select tw-glass-input" data-nationality="true"><asp:ListItem Value="SA">South African, 13-digit ID</asp:ListItem><asp:ListItem Value="Passport">Non-South African, passport</asp:ListItem></asp:DropDownList>
<div data-passport-fields="true" class="tw-passport-fields">
<div class="dashboard-form-group"><label for="<%= txtPassportNumber.ClientID %>">Passport number</label><asp:TextBox ID="txtPassportNumber" runat="server" CssClass="dashboard-input form-control tw-glass-input" MaxLength="20" autocomplete="off" />
</div><div class="dashboard-form-group"><label for="<%= txtPassportCountry.ClientID %>">Issuing country</label><asp:TextBox ID="txtPassportCountry" runat="server" CssClass="dashboard-input form-control tw-glass-input" MaxLength="50" autocomplete="country-name" /></div>
</div></div>
<div
                        id="residentialIdentityGroup"
                        runat="server"
                        class="dashboard-form-group">

                        <label for="<%= txtSouthAfricanID.ClientID %>">
                            South African ID Number
                        </label>

                        <asp:TextBox
                            ID="txtSouthAfricanID"
                            runat="server"
                            CssClass="dashboard-input"
                            MaxLength="13"
                            placeholder="13-digit SA ID number">
                        </asp:TextBox>

                    </div>


                    <!-- COMPANY REGISTRATION -->

                    <div
                        id="businessIdentityGroup"
                        runat="server"
                        class="dashboard-form-group">

                        <label for="<%= txtCompanyRegistration.ClientID %>">
                            Company Registration Number
                        </label>

                        <asp:TextBox
                            ID="txtCompanyRegistration"
                            runat="server"
                            CssClass="dashboard-input"
                            MaxLength="50"
                            placeholder="e.g. 2008/069148/23">
                        </asp:TextBox>

                    </div>


                    <!-- SERVICE AREA -->

                    <div class="dashboard-form-group">

                        <label for="<%= ddlServiceArea.ClientID %>">
                            Service Area
                        </label>

                        <asp:DropDownList
                            ID="ddlServiceArea"
                            runat="server"
                            CssClass="dashboard-input">

                            <asp:ListItem
                                Text="Select area"
                                Value="" />

                            <asp:ListItem
                                Text="Umhlanga"
                                Value="Umhlanga" />

                            <asp:ListItem
                                Text="Bearea"
                                Value="Bearea" />

                            <asp:ListItem
                                Text="Morningside"
                                Value="Morningside" />

                            <asp:ListItem
                                Text="KwaMashu"
                                Value="KwaMashu" />

                            <asp:ListItem
                                Text="Durban North"
                                Value="Durban North" />

                            <asp:ListItem
                                Text="PineTown"
                                Value="PineTown" />

                        </asp:DropDownList>

                    </div>


                    <!-- ADDRESS -->

                    <div class="dashboard-form-group admin-customer-address">

                        <label for="<%= txtAddress.ClientID %>">
                            Full Address
                        </label>

                        <asp:TextBox
                            ID="txtAddress"
                            runat="server"
                            TextMode="MultiLine"
                            Rows="3"
                            CssClass="dashboard-input dashboard-textarea"
                            placeholder="Enter physical address">
                        </asp:TextBox>

                    </div>

                </div>


                <!-- PASSWORD INFORMATION -->

                <div class="admin-customer-password-info">

                    <div class="admin-customer-password-icon">
                        <i class="bi bi-envelope"></i>
                    </div>

                    <div>

                        <strong>
                            Customer password
                        </strong>

                        <p>
                            The customer receives their username and password setup instructions by email.
                        </p>

                    </div>

                </div>


                <!-- CENTERED ACTIONS -->

                <div class="admin-customer-actions">

                    <asp:Button
                        ID="btnSaveProfile"
                        runat="server"
                        Text="Register Customer"
                        CssClass="dashboard-btn dashboard-btn-primary"
                        OnClick="btnSaveProfile_Click" />

                    <asp:Button
                        ID="btnUpdateProfile"
                        runat="server"
                        Text="Update Customer"
                        CssClass="dashboard-btn dashboard-btn-secondary"
                        Enabled="false"
                        OnClick="btnUpdateProfile_Click" />

                    <asp:Button
                        ID="btnClearForm"
                        runat="server"
                        Text="Clear Form"
                        CssClass="dashboard-btn dashboard-btn-secondary"
                        OnClick="btnClearForm_Click" />

                </div>


                <!-- MESSAGE -->

                

            </div>

        </section>

    </div>


    <!-- =============================================================
         CUSTOMER TYPE UI
         ============================================================= -->


<script type="text/javascript">
    (function () {
        "use strict";

        function initialiseCustomerForm() {
            var fields = {
                fullName: document.getElementById("<%= txtCustName.ClientID %>"),
            email: document.getElementById("<%= txtEmail.ClientID %>"),
            phone: document.getElementById("<%= txtPhone.ClientID %>"),
            customerType: document.getElementById("<%= ddlCustomerType.ClientID %>"),
            passport: document.getElementById("<%= txtPassportNumber.ClientID %>"),
            country: document.getElementById("<%= txtPassportCountry.ClientID %>"),
            nationality: document.getElementById("<%= ddlNationality.ClientID %>"),
            saId: document.getElementById("<%= txtSouthAfricanID.ClientID %>"),
            companyRegistration: document.getElementById("<%= txtCompanyRegistration.ClientID %>"),
            serviceArea: document.getElementById("<%= ddlServiceArea.ClientID %>"),
            address: document.getElementById("<%= txtAddress.ClientID %>")
        };

        var residentialGroup =
            document.getElementById("<%= residentialIdentityGroup.ClientID %>");

        var businessGroup =
            document.getElementById("<%= businessIdentityGroup.ClientID %>");

        var saveButton =
            document.getElementById("<%= btnSaveProfile.ClientID %>");

        var updateButton =
            document.getElementById("<%= btnUpdateProfile.ClientID %>");

            var customerForm =
                (saveButton && saveButton.form) ||
                (updateButton && updateButton.form);

            if (!customerForm) {
                return;
            }

            // ------------------------------------------------------------
            // ERROR STYLING
            // ------------------------------------------------------------

            var style = document.createElement("style");

            style.textContent = `
            .customer-field-error {
                display: block;
                margin-top: 6px;
                color: var(--color-danger);
                font-size: 0.82rem;
                line-height: 1.4;
            }

            .dashboard-input.customer-input-invalid {
                border-color: var(--color-danger) !important;
                box-shadow: 0 0 0 2px rgba(220, 98, 108, 0.12);
            }

            .dashboard-input.customer-input-valid {
                border-color: var(--color-success);
            }
        `;

            document.head.appendChild(style);

            // ------------------------------------------------------------
            // CREATE ERROR ELEMENTS
            // ------------------------------------------------------------

            var errorElements = {};

            Object.keys(fields).forEach(function (key) {
                var field = fields[key];

                if (!field) {
                    return;
                }

                var error = document.createElement("div");

                error.className = "customer-field-error";
                error.id = field.id + "_clientError";
                error.setAttribute("aria-live", "polite");
                error.style.display = "none";

                field.insertAdjacentElement("afterend", error);

                field.setAttribute("aria-describedby", error.id);

                errorElements[key] = error;
            });

            // ------------------------------------------------------------
            // CUSTOMER TYPE FIELD VISIBILITY
            // ------------------------------------------------------------

            function updateIdentityFields() {
                if (!fields.customerType ||
                    !residentialGroup ||
                    !businessGroup) {
                    return;
                }

                var isBusiness =
                    fields.customerType.value === "Business";

                residentialGroup.style.display =
                    isBusiness ? "none" : "";

                businessGroup.style.display =
                    isBusiness ? "" : "none";

                if(window.ThyWillUI)window.ThyWillUI.initializeIdentity();

                // The hidden identity field is not relevant to this type.
                if (isBusiness) {
                    fields.saId.value = "";
                    clearError("saId");
                } else {
                    fields.companyRegistration.value = "";
                    clearError("companyRegistration");
                }
            }

            // ------------------------------------------------------------
            // FORM MODE
            // ------------------------------------------------------------

            function isRegistrationMode() {
                return !!saveButton &&
                    !saveButton.disabled;
            }

            // ------------------------------------------------------------
            // ERROR DISPLAY HELPERS
            // ------------------------------------------------------------

            function showError(key, message) {
                var field = fields[key];
                var error = errorElements[key];

                if (!field || !error) {
                    return;
                }

                error.textContent = message;
                error.style.display = "block";

                field.classList.add("customer-input-invalid");
                field.classList.remove("customer-input-valid");
                field.setAttribute("aria-invalid", "true");
            }

            function clearError(key) {
                var field = fields[key];
                var error = errorElements[key];

                if (!field || !error) {
                    return;
                }

                error.textContent = "";
                error.style.display = "none";

                field.classList.remove("customer-input-invalid");
                field.classList.remove("customer-input-valid");
                field.removeAttribute("aria-invalid");
            }

            function markValid(key) {
                var field = fields[key];

                clearError(key);

                if (field) {
                    field.classList.add("customer-input-valid");
                }
            }

            function isVisible(field) {
                return !!field &&
                    field.getClientRects().length > 0;
            }

            // ------------------------------------------------------------
            // VALIDATION RULES
            // These mirror the rules visible in the supplied code-behind.
            // ------------------------------------------------------------

            function validateField(key) {
                var field = fields[key];

                if (!field) {
                    return true;
                }

                // Do not validate hidden identity fields.
                if ((key === "saId" ||
                    key === "companyRegistration") &&
                    !isVisible(field)) {
                    clearError(key);
                    return true;
                }

                // Customer type and identity fields are only validated
                // when registering a new customer.
                if (!isRegistrationMode() &&
                    (key === "fullName" ||
                        key === "customerType" ||
                        key === "saId" ||
                        key === "companyRegistration")) {
                    clearError(key);
                    return true;
                }

                var value = field.value.trim();

                switch (key) {
                    case "fullName":
                        if (!value) {
                            showError(
                                key,
                                "Customer full name is required."
                            );
                            return false;
                        }

                        // The exact ValidationHelper.IsValidName rules
                        // were not included in the supplied code.
                        // Do not invent a different name-format rule here.
                        break;

                    case "email":
                        if (!value) {
                            showError(
                                key,
                                "Customer email address is required."
                            );
                            return false;
                        }

                        if (value.length > 100 ||
                            !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
                            showError(
                                key,
                                "Please enter a valid email address."
                            );
                            return false;
                        }
                        break;

                    case "phone":
                        if (!value) {
                            showError(
                                key,
                                "Phone number is required."
                            );
                            return false;
                        }

                        if (!/^0\d{9}$/.test(value)) {
                            showError(
                                key,
                                "Enter a valid 10-digit South African phone number starting with 0."
                            );
                            return false;
                        }
                        break;

                    case "address":
                        if (!value) {
                            showError(
                                key,
                                "Full address is required."
                            );
                            return false;
                        }

                        if (value.length > 255) {
                            showError(
                                key,
                                "Full address cannot exceed 255 characters."
                            );
                            return false;
                        }
                        break;

                    case "serviceArea":
                        if (!value) {
                            showError(
                                key,
                                "Please select a service area."
                            );
                            return false;
                        }
                        break;

                    case "customerType":
                        if (!value) {
                            showError(
                                key,
                                "Please select a customer type."
                            );
                            return false;
                        }

                        if (value !== "Residential" &&
                            value !== "Business") {
                            showError(
                                key,
                                "Please select a valid customer type."
                            );
                            return false;
                        }
                        break;

                    case "passport":
                        if(!value||value.length>20){showError(key,"Enter a passport number of up to 20 characters.");return false;}break;
                    case "country":
                        if(!value||value.length>50){showError(key,"Enter the issuing country, up to 50 characters.");return false;}break;
                    case "saId":
                        if (!value) {
                            showError(
                                key,
                                "South African ID number is required for residential customers."
                            );
                            return false;
                        }

                        if (!/^\d{13}$/.test(value)) {
                            showError(
                                key,
                                "South African ID number must contain exactly 13 digits."
                            );
                            return false;
                        }
                        break;

                    case "companyRegistration":
                        if (!value) {
                            showError(
                                key,
                                "Company registration number is required for business customers."
                            );
                            return false;
                        }

                        if (value.length > 50) {
                            showError(
                                key,
                                "Company registration number cannot exceed 50 characters."
                            );
                            return false;
                        }

                        if (!/^\d{4}\/\d{6}\/\d{2}$/.test(value)) {
                            showError(
                                key,
                                "Use the format YYYY/NNNNNN/NN."
                            );
                            return false;
                        }
                        break;
                }

                markValid(key);
                return true;
            }

            // ------------------------------------------------------------
            // VALIDATE THE RELEVANT FIELDS FOR EACH OPERATION
            // ------------------------------------------------------------

            function validateForm() {
                var keys;

                if (isRegistrationMode()) {
                    keys = [
                        "fullName",
                        "email",
                        "phone",
                        "customerType",
                        "serviceArea",
                        "address"
                    ];

                    if (fields.customerType.value === "Residential") {
                        if(fields.nationality.value==="Passport")keys.push("passport","country");else keys.push("saId");
                    } else if (fields.customerType.value === "Business") {
                        keys.push("companyRegistration");
                    }
                } else {
                    // Matches ValidateCustomerUpdateForm().
                    keys = [
                        "email",
                        "phone",
                        "serviceArea",
                        "address"
                    ];
                }

                var firstInvalidField = null;
                var isValid = true;

                keys.forEach(function (key) {
                    if (!validateField(key)) {
                        isValid = false;

                        if (!firstInvalidField) {
                            firstInvalidField = fields[key];
                        }
                    }
                });

                if (!isValid && firstInvalidField) {
                    firstInvalidField.focus();
                }

                return isValid;
            }

            // ------------------------------------------------------------
            // BLUR / CHANGE / INPUT EVENTS
            // ------------------------------------------------------------

            Object.keys(fields).forEach(function (key) {
                var field = fields[key];

                if (!field) {
                    return;
                }

                field.addEventListener("blur", function () {
                    validateField(key);
                });

                if (field.tagName === "SELECT") {
                    field.addEventListener("change", function () {
                        if (key === "customerType") {
                            updateIdentityFields();
                        }

                        validateField(key);
                    });
                } else {
                    field.addEventListener("input", function () {
                        // Remove a displayed error as soon as the user
                        // corrects the field, then validate the new value.
                        if (errorElements[key] &&
                            errorElements[key].style.display !== "none") {
                            validateField(key);
                        }
                    });
                }
            });

            // ------------------------------------------------------------
            // BLOCK INVALID REGISTRATION / UPDATE SUBMISSIONS
            // ------------------------------------------------------------

            function attachSubmitValidation(button) {
                if (!button) {
                    return;
                }

                button.addEventListener("click", function (event) {
                    if (!validateForm()) {
                        event.preventDefault();
                        event.stopImmediatePropagation();
                    }
                }, true);
            }

            attachSubmitValidation(saveButton);
            attachSubmitValidation(updateButton);

            // ------------------------------------------------------------
            // INITIAL STATE
            // ------------------------------------------------------------

            updateIdentityFields();

            // The customer-type switching above is preserved.
            // Do not show validation errors before the user interacts.
        }

        if (document.readyState === "loading") {
            document.addEventListener(
                "DOMContentLoaded",
                initialiseCustomerForm
            );
        } else {
            initialiseCustomerForm();
        }
    })();
</script>

</div>
</asp:Content>
