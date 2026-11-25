
<%@ Page
    Title="Customer Management"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Customers.aspx.cs"
    Inherits="MajorProject.Web.Manager.Customers" %>

<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-manager-customers">


    

    <div class="dashboard-page admin-customers"  data-record-tabs="secure-manager-customers" data-tab-labels="list=Customers;details=Customer profile" data-selected-id="hfSelectedCustomerID">
<asp:Label
                    ID="lblMessage"
                    runat="server"
                    CssClass="dashboard-alert"
                    Visible="false">
                </asp:Label>


        <!-- PAGE HEADER -->

        <section class="dashboard-page-header">

            <div>

                <span class="dashboard-eyebrow">
                    CUSTOMER MANAGEMENT
                </span>

                <h1 class="dashboard-page-title">
                    Customers
                </h1>

                <p class="dashboard-page-description">
                    Register, update and manage customer accounts.
                </p>

            </div>

        </section>

        <!-- CUSTOMER DIRECTORY -->

        <section class="dashboard-panel admin-customers-directory"  data-tab-pane="list">

            <div class="dashboard-panel-header">

                <div>

                    <h2 class="dashboard-panel-title">
                        Customer Directory
                    </h2>

                    <p class="dashboard-panel-description">
                        Search and select an existing customer to manage their profile.
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

                            <asp:ListItem
                                Text="Account Status"
                                Value="AccountStatus" />

                        </asp:DropDownList>

                    </div>

                    <div class="admin-customers-toolbar-action">

                        <asp:Button
                            ID="btnClearSearch"
                            runat="server"
                            Text="Clear"
                            CssClass="dashboard-btn dashboard-btn-secondary"
                            CausesValidation="false"
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
                                DataField="AccountStatus"
                                HeaderText="Status" />

                            <asp:BoundField
                                DataField="DateRegistered"
                                HeaderText="Registered"
                                DataFormatString="{0:dd MMM yyyy}" />

                            <asp:TemplateField HeaderText="Action">

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

        <!-- CUSTOMER PROFILE -->

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
                            MaxLength="200"
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
                            MaxLength="254"
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
                            MaxLength="10"
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

                    <!-- ACCOUNT STATUS -->

                    <div class="dashboard-form-group">

                        <label for="<%= ddlAccountStatus.ClientID %>">
                            Account Status
                        </label>

                        <asp:DropDownList
                            ID="ddlAccountStatus"
                            runat="server"
                            CssClass="dashboard-input"
                            Enabled="false">

                            <asp:ListItem
                                Text="Active"
                                Value="Active" />

                            <asp:ListItem
                                Text="Deactivated"
                                Value="Deactivated" />

                        </asp:DropDownList>

                    </div>

                    <!-- FULL ADDRESS -->

                    <div class="dashboard-form-group admin-customer-address">

                        <label for="<%= txtAddress.ClientID %>">
                            Full Address
                        </label>

                        <asp:TextBox
                            ID="txtAddress"
                            runat="server"
                            TextMode="MultiLine"
                            Rows="3"
                            MaxLength="500"
                            CssClass="dashboard-input dashboard-textarea"
                            placeholder="Enter physical address">
                        </asp:TextBox>

                        <small class="customer-character-count"
                            id="addressCharacterCount">
                            0 / 255 characters
                        </small>

                    </div>

                </div>

                <!-- PASSWORD INFORMATION -->

                <div class="admin-customer-password-info">

                    <div class="admin-customer-password-icon">
                        <i class="bi bi-envelope"></i>
                    </div>

                    <div>

                        <strong>Customer password</strong>

                        <p>
                            The customer receives their username and password setup instructions by email.
                        </p>

                    </div>

                </div>

                <!-- ACCOUNT STATUS INFORMATION -->

                <div class="admin-customer-password-info">

                    <div class="admin-customer-password-icon">
                        <i class="bi bi-person-lock"></i>
                    </div>

                    <div>

                        <strong>Account status</strong>

                        <p>
                            Deactivating a customer changes their account status
                            to Deactivated. The customer record is retained and
                            is not physically deleted.
                        </p>

                    </div>

                </div>

                <!-- ACTIONS -->

                <div class="admin-customer-actions">

                    <asp:Button
                        ID="btnSaveProfile"
                        runat="server"
                        Text="Register Customer"
                        CssClass="dashboard-btn dashboard-btn-primary"
                        OnClientClick="return validateCustomerFormClient('register');"
                        OnClick="btnSaveProfile_Click" />

                    <asp:Button
                        ID="btnUpdateProfile"
                        runat="server"
                        Text="Update Customer"
                        CssClass="dashboard-btn dashboard-btn-secondary"
                        Enabled="false"
                        OnClientClick="return validateCustomerFormClient('update');"
                        OnClick="btnUpdateProfile_Click" />

                    <asp:Button
                        ID="btnDeactivateCustomer"
                        runat="server"
                        Text="Deactivate Customer"
                        CssClass="dashboard-btn dashboard-btn-danger"
                        Enabled="false"
                        CausesValidation="false"
                        OnClick="btnDeactivateCustomer_Click"
                        OnClientClick="return confirm('Are you sure you want to deactivate this customer account? The customer record will be retained.');" />

                    <asp:Button
                        ID="btnActivateCustomer"
                        runat="server"
                        Text="Activate Customer"
                        CssClass="dashboard-btn dashboard-btn-primary"
                        Enabled="false"
                        CausesValidation="false"
                        OnClick="btnActivateCustomer_Click"
                        OnClientClick="return confirm('Are you sure you want to activate this customer?');" />

                    <asp:Button
                        ID="btnClearForm"
                        runat="server"
                        Text="Clear Form"
                        CssClass="dashboard-btn dashboard-btn-secondary"
                        CausesValidation="false"
                        OnClick="btnClearForm_Click" />

                </div>

                <!-- MESSAGE -->

                

            </div>

        </section>

    </div>

    <!-- CLIENT-SIDE VALIDATION -->

    <script type="text/javascript">
        (function () {
            "use strict";

            var ids = {
                search: "<%= txtSearch.ClientID %>",
                name: "<%= txtCustName.ClientID %>",
                email: "<%= txtEmail.ClientID %>",
                phone: "<%= txtPhone.ClientID %>",
                customerType: "<%= ddlCustomerType.ClientID %>",
                nationality:"<%= ddlNationality.ClientID %>",passport:"<%= txtPassportNumber.ClientID %>",country:"<%= txtPassportCountry.ClientID %>",
                saId: "<%= txtSouthAfricanID.ClientID %>",
                companyRegistration: "<%= txtCompanyRegistration.ClientID %>",
                serviceArea: "<%= ddlServiceArea.ClientID %>",
                address: "<%= txtAddress.ClientID %>",
                accountStatus: "<%= ddlAccountStatus.ClientID %>",
                selectedCustomer: "<%= hfSelectedCustomerID.ClientID %>",
                residentialGroup: "<%= residentialIdentityGroup.ClientID %>",
                businessGroup: "<%= businessIdentityGroup.ClientID %>"
            };

            var touched = {};
            var validationMode = "register";
            var maxAddressLength = 255;
            var maxCompanyRegistrationLength = 20;

            function getField(key) {
                return document.getElementById(ids[key]);
            }

            function getErrorElement(field) {
                var group = field.closest(".dashboard-form-group");

                if (!group) {
                    return null;
                }

                var error = group.querySelector(".customer-field-error");

                if (!error) {
                    error = document.createElement("small");
                    error.className = "customer-field-error";
                    error.setAttribute("role", "alert");
                    error.setAttribute("aria-live", "polite");
                    group.appendChild(error);
                }

                return error;
            }

            function setFieldState(field, message) {
                if (!field) {
                    return !message;
                }

                var error = getErrorElement(field);

                field.classList.remove("customer-field-invalid");
                field.classList.remove("customer-field-valid");

                if (message) {
                    field.classList.add("customer-field-invalid");
                    field.setAttribute("aria-invalid", "true");

                    if (error) {
                        error.textContent = message;
                    }

                    return false;
                }

                field.removeAttribute("aria-invalid");

                if (error) {
                    error.textContent = "";
                }

                if (String(field.value || "").trim() !== "") {
                    field.classList.add("customer-field-valid");
                }

                return true;
            }

            function isValidName(value) {
                /*
                 * Mirrors the common intent of ValidationHelper.IsValidName.
                 * The server-side helper remains authoritative.
                 */
                return /^[\p{L}\p{M}]+(?:[ '\u2019-][\p{L}\p{M}]+)*$/u.test(value);
            }

            function validateName() {
                var field = getField("name");

                if (!field || field.readOnly || validationMode === "update") {
                    return true;
                }

                var value = field.value.trim();

                if (!value) {
                    return setFieldState(field, "Customer full name is required.");
                }

                if (!isValidName(value)) {
                    return setFieldState(field, "Please enter a valid customer name.");
                }

                return setFieldState(field, "");
            }

            function validateEmail() {
                var field = getField("email");

                if (!field) {
                    return true;
                }

                var value = field.value.trim();

                if (!value) {
                    return setFieldState(field, "Customer email address is required.");
                }

                var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

                if (!emailPattern.test(value)) {
                    return setFieldState(field, "Please enter a valid email address.");
                }

                return setFieldState(field, "");
            }

            function validatePhone() {
                var field = getField("phone");

                if (!field) {
                    return true;
                }

                var value = field.value.trim();

                if (!value) {
                    return setFieldState(field, "Phone number is required.");
                }

                if (!/^0\d{9}$/.test(value)) {
                    return setFieldState(
                        field,
                        "Enter a valid 10-digit South African number starting with 0."
                    );
                }

                return setFieldState(field, "");
            }

            function validateCustomerType() {
                var field = getField("customerType");

                if (!field || field.disabled || validationMode === "update") {
                    return true;
                }

                if (field.value !== "Residential" && field.value !== "Business") {
                    return setFieldState(field, "Please select a valid customer type.");
                }

                return setFieldState(field, "");
            }

            function validatePassport(key) {
                var field=getField(key),type=getField("customerType"),nation=getField("nationality");
                if(!field||field.readOnly||validationMode==="update"||!type||type.value!=="Residential"||!nation||nation.value!=="Passport")return true;
                var value=field.value.trim(),max=key==="passport"?20:50;
                return setFieldState(field,!value||value.length>max?"Enter "+(key==="passport"?"the passport number":"the issuing country")+", up to "+max+" characters.":"");
            }
            function validateSouthAfricanID() {
                if(getField("nationality")&&getField("nationality").value==="Passport")return true;
                var field = getField("saId");
                var type = getField("customerType");

                if (!field || !type || type.value !== "Residential") {
                    return true;
                }

                if (field.readOnly || validationMode === "update") {
                    return true;
                }

                var value = field.value.trim();

                if (!value) {
                    return setFieldState(
                        field,
                        "South African ID number is required for residential customers."
                    );
                }

                if (!/^\d{13}$/.test(value)) {
                    return setFieldState(
                        field,
                        "South African ID number must contain exactly 13 digits."
                    );
                }

                return setFieldState(field, "");
            }

            function validateCompanyRegistration() {
                var field = getField("companyRegistration");
                var type = getField("customerType");

                if (!field || !type || type.value !== "Business") {
                    return true;
                }

                if (field.readOnly || validationMode === "update") {
                    return true;
                }

                var value = field.value.trim();

                if (!value) {
                    return setFieldState(
                        field,
                        "Company registration number is required for business customers."
                    );
                }

                if (value.length > maxCompanyRegistrationLength) {
                    return setFieldState(
                        field,
                        "Company registration number cannot exceed 50 characters."
                    );
                }

                if (!/^\d{4}\/\d{6}\/\d{2}$/.test(value)) {
                    return setFieldState(
                        field,
                        "Use the format YYYY/NNNNNN/NN, for example 2008/069148/23."
                    );
                }

                return setFieldState(field, "");
            }

            function validateServiceArea() {
                var field = getField("serviceArea");

                if (!field) {
                    return true;
                }

                if (!field.value.trim()) {
                    return setFieldState(field, "Please select a service area.");
                }

                return setFieldState(field, "");
            }

            function validateAddress() {
                var field = getField("address");

                if (!field) {
                    return true;
                }

                var value = field.value.trim();

                if (!value) {
                    return setFieldState(field, "Full address is required.");
                }

                if (value.length > maxAddressLength) {
                    return setFieldState(
                        field,
                        "Full address cannot exceed 255 characters."
                    );
                }

                return setFieldState(field, "");
            }

            function validateField(key) {
                switch (key) {
                    case "name":
                        return validateName();

                    case "email":
                        return validateEmail();

                    case "phone":
                        return validatePhone();

                    case "customerType":
                        return validateCustomerType();

                    case "passport":case "country":return validatePassport(key);
                    case "saId":
                        return validateSouthAfricanID();

                    case "companyRegistration":
                        return validateCompanyRegistration();

                    case "serviceArea":
                        return validateServiceArea();

                    case "address":
                        return validateAddress();

                    default:
                        return true;
                }
            }

            function fieldsForMode(mode) {
                if (mode === "update") {
                    return ["email", "phone", "address", "serviceArea"];
                }

                var fields = [
                    "name",
                    "email",
                    "phone",
                    "customerType",
                    "serviceArea",
                    "address"
                ];

                var type = getField("customerType");

                if (type && type.value === "Residential") {
                    if(getField("nationality")&&getField("nationality").value==="Passport")fields.push("passport","country");else fields.push("saId");
                } else if (type && type.value === "Business") {
                    fields.push("companyRegistration");
                }

                return fields;
            }

            function validateForm(mode) {
                validationMode = mode;

                var selectedCustomer = getField("selectedCustomer");

                if (mode === "update" &&
                    (!selectedCustomer || !selectedCustomer.value)) {

                    window.alert("Please select a customer first.");
                    return false;
                }

                var fields = fieldsForMode(mode);
                var valid = true;
                var firstInvalid = null;

                fields.forEach(function (key) {
                    touched[key] = true;

                    if (!validateField(key)) {
                        valid = false;

                        if (!firstInvalid) {
                            firstInvalid = getField(key);
                        }
                    }
                });

                if (firstInvalid) {
                    firstInvalid.focus();
                }

                return valid;
            }

            window.validateCustomerFormClient = function (mode) {
                return validateForm(mode);
            };

            function updateIdentityFields() {
                var type = getField("customerType");
                var residential = getField("residentialGroup");
                var business = getField("businessGroup");

                if (!type || !residential || !business) {
                    return;
                }

                var isBusiness = type.value === "Business";

                residential.style.display = isBusiness ? "none" : "";
                business.style.display = isBusiness ? "" : "none";

                if (isBusiness) {
                    setFieldState(getField("saId"), "");
                } else {
                    setFieldState(getField("companyRegistration"), "");
                }

                if (touched.customerType) {
                    validateCustomerType();
                }

                if (touched.saId) {
                    validateSouthAfricanID();
                }

                if (touched.companyRegistration) {
                    validateCompanyRegistration();
                }
            }

            function updateAddressCounter() {
                var field = getField("address");
                var counter = document.getElementById("addressCharacterCount");

                if (!field || !counter) {
                    return;
                }

                var length = field.value.length;

                counter.textContent = length + " / " + maxAddressLength + " characters";
                counter.classList.toggle("is-limit", length > maxAddressLength);
            }

            function attachValidation() {
                var validationKeys = ["passport","country",
                    "name",
                    "email",
                    "phone",
                    "customerType",
                    "saId",
                    "companyRegistration",
                    "serviceArea",
                    "address"
                ];

                validationKeys.forEach(function (key) {
                    var field = getField(key);

                    if (!field) {
                        return;
                    }

                    field.addEventListener("blur", function () {
                        touched[key] = true;
                        validateField(key);
                    });

                    var eventName =
                        field.tagName === "SELECT" ? "change" : "input";

                    field.addEventListener(eventName, function () {
                        if (touched[key]) {
                            validateField(key);
                        }
                    });
                });

                var customerType = getField("customerType");

                if (customerType) {
                    customerType.addEventListener("change", updateIdentityFields);
                }

                var address = getField("address");

                if (address) {
                    address.addEventListener("input", updateAddressCounter);
                }

                updateIdentityFields();
                updateAddressCounter();

                /*
                 * Do not run profile validation automatically on page load.
                 * This allows a blank form to open without showing errors.
                 */
            }

            if (document.readyState === "loading") {
                document.addEventListener("DOMContentLoaded", attachValidation);
            } else {
                attachValidation();
            }
        })();
    </script>


</div>

</div>
</asp:Content>
