<%@ Page Title="Create Profile"
    Language="C#"
    MasterPageFile="~/MasterPages/Auth.Master"
    AutoEventWireup="true"
    CodeBehind="Register.aspx.cs"
    Inherits="MajorProject.Web.Account.Register" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>

<section class="tw-registration-section">

    <div class="tw-blueprint-grid" aria-hidden="true"></div>

    <div class="tw-container">

        <div class="row justify-content-center">

            <div class="col-12">

                <!-- =====================================================
                     REGISTRATION FORM
                     ===================================================== -->

                <asp:Panel
                    ID="pnlForm"
                    runat="server"
                    CssClass="tw-glass-card tw-form-card">

                    <div class="text-center mb-4">

                        <div class="tw-brand-icon mx-auto mb-3">
                            <i class="bi bi-person-plus-fill"></i>
                        </div>

                        <h2 class="tw-text-primary fw-bold mb-1">
                            Create Customer Profile
                        </h2>

                        <p class="tw-text-muted">
                            Register your details to request electrical
                            &amp; construction services seamlessly.
                        </p>

                    </div>


                    <!-- =================================================
                         VALIDATION / ERROR MESSAGE
                         ================================================= -->

                    <asp:Label
                        ID="lblMessage"
                        runat="server"
                        CssClass="tw-booking-message text-danger d-block mb-3"
                        Visible="false"
                        role="alert" />


                    <div class="row g-3">


                        <!-- =============================================
                             FULL NAME
                             ============================================= -->

                        <div class="col-md-6">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtFullName.ClientID %>">

                                Full Name *

                            </label>

                            <asp:TextBox
                                ID="txtFullName"
                                runat="server"
                                CssClass="form-control tw-glass-input"
                                placeholder="Enter your full name"
                                autocomplete="name" />

                        </div>


                        <!-- =============================================
                             PHONE NUMBER
                             ============================================= -->

                        <div class="col-md-6">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtPhone.ClientID %>">

                                Phone Number *

                            </label>

                            <asp:TextBox
                                ID="txtPhone"
                                runat="server"
                                CssClass="form-control tw-glass-input"
                                placeholder="e.g. 083 123 4567"
                                autocomplete="tel"
                                TextMode="Phone" />

                            <small class="tw-text-muted d-block mt-2">
                                A phone number may be shared by more than
                                one customer account.
                            </small>

                        </div>


                        <!-- =============================================
                             EMAIL ADDRESS
                             ============================================= -->

                        <div class="col-md-6">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtEmail.ClientID %>">

                                Email Address *

                            </label>

                            <asp:TextBox
                                ID="txtEmail"
                                runat="server"
                                TextMode="Email"
                                CssClass="form-control tw-glass-input"
                                placeholder="name@example.com"
                                autocomplete="email" />

                            <small class="tw-text-muted d-block mt-2">
                                This email address will be used to sign in.
                            </small>

                        </div>


                        <!-- =============================================
                             CUSTOMER TYPE
                             ============================================= -->

                        <div class="col-md-6">

                            <label
                                class="form-label tw-form-label"
                                for="<%= ddlCustomerType.ClientID %>">

                                Customer Type *

                            </label>

                            <asp:DropDownList
                                ID="ddlCustomerType"
                                runat="server"
                                CssClass="form-select tw-glass-input"
                                onchange="updateIdentityField();">

                                <asp:ListItem
                                    Text="Residential"
                                    Value="Residential" />

                                <asp:ListItem
                                    Text="Business"
                                    Value="Business" />

                            </asp:DropDownList>

                        </div>


<div class="col-12 tw-identity-choice" data-identity-choice>
<label for="<%= ddlNationality.ClientID %>">Identity document for an individual</label>
<asp:DropDownList ID="ddlNationality" runat="server" CssClass="dashboard-input form-select tw-glass-input" data-nationality="true"><asp:ListItem Value="SA">South African, 13-digit ID</asp:ListItem><asp:ListItem Value="Passport">Non-South African, passport</asp:ListItem></asp:DropDownList>
<div data-passport-fields="true" class="tw-passport-fields">
<div class="dashboard-form-group"><label for="<%= txtPassportNumber.ClientID %>">Passport number</label><asp:TextBox ID="txtPassportNumber" runat="server" CssClass="dashboard-input form-control tw-glass-input" MaxLength="20" autocomplete="off" />
</div><div class="dashboard-form-group"><label for="<%= txtPassportCountry.ClientID %>">Issuing country</label><asp:TextBox ID="txtPassportCountry" runat="server" CssClass="dashboard-input form-control tw-glass-input" MaxLength="50" autocomplete="country-name" /></div>
</div></div>
                        <!-- =============================================
                             RESIDENTIAL IDENTITY
                             ============================================= -->

                        <div
                            id="residentialIdentity"
                            class="col-12">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtSouthAfricanID.ClientID %>">

                                South African ID Number *

                            </label>

                            <asp:TextBox
                                ID="txtSouthAfricanID"
                                runat="server"
                                CssClass="form-control tw-glass-input"
                                MaxLength="13"
                                placeholder="Enter your 13-digit South African ID number"
                                autocomplete="off"
                                inputmode="numeric" />

                            <small class="tw-text-muted d-block mt-2">
                                Your South African ID number is required for
                                residential customer registration.
                            </small>

                        </div>


                        <!-- =============================================
                             BUSINESS IDENTITY
                             ============================================= -->

                        <div
                            id="businessIdentity"
                            class="col-12"
                            style="display: none;">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtCompanyRegistration.ClientID %>">

                                Company Registration Number *

                            </label>

                            <asp:TextBox
                                ID="txtCompanyRegistration"
                                runat="server"
                                CssClass="form-control tw-glass-input"
                                MaxLength="20"
                                placeholder="e.g. 2008/069148/23"
                                autocomplete="off" />

                            <small class="tw-text-muted d-block mt-2">
                                Enter your company's registration number.
                            </small>

                        </div>


                        <!-- =============================================
                             PASSWORD
                             ============================================= -->

                        <div class="col-md-6">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtPassword.ClientID %>">

                                Password *

                            </label>

                            <div class="position-relative">

                                <asp:TextBox
                                    ID="txtPassword"
                                    runat="server"
                                    TextMode="Password"
                                    CssClass="form-control tw-glass-input pe-5"
                                    placeholder="Create a secure password"
                                    autocomplete="new-password" />

                                <button
                                    type="button"
                                    class="btn position-absolute top-50 end-0 translate-middle-y border-0 bg-transparent tw-text-primary pe-3"
                                    style="z-index: 5;"
                                    onclick="togglePassword('<%= txtPassword.ClientID %>', this)">

                                    <i
                                        class="bi bi-eye"
                                        aria-hidden="true"></i>

                                </button>

                            </div>

                            <small class="tw-text-muted d-block mt-2">
                                Must contain at least 8 characters,
                                uppercase, lowercase, a number and
                                a special character.
                            </small>

                        </div>


                        <!-- =============================================
                             CONFIRM PASSWORD
                             ============================================= -->

                        <div class="col-md-6">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtConfirmPassword.ClientID %>">

                                Confirm Password *

                            </label>

                            <div class="position-relative">

                                <asp:TextBox
                                    ID="txtConfirmPassword"
                                    runat="server"
                                    TextMode="Password"
                                    CssClass="form-control tw-glass-input pe-5"
                                    placeholder="Confirm your password"
                                    autocomplete="new-password" />

                                <button
                                    type="button"
                                    class="btn position-absolute top-50 end-0 translate-middle-y border-0 bg-transparent tw-text-primary pe-3"
                                    style="z-index: 5;"
                                    onclick="togglePassword('<%= txtConfirmPassword.ClientID %>', this)">

                                    <i
                                        class="bi bi-eye"
                                        aria-hidden="true"></i>

                                </button>

                            </div>

                        </div>


                        <!-- =============================================
                             FULL ADDRESS
                             ============================================= -->

                        <div class="col-12">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtAddress.ClientID %>">

                                Full Address *

                            </label>

                            <asp:TextBox
                                ID="txtAddress"
                                runat="server"
                                TextMode="MultiLine"
                                Rows="3"
                                CssClass="form-control tw-glass-input"
                                placeholder="Enter your full street address"
                                autocomplete="street-address" />

                        </div>


                        <!-- =============================================
                             SERVICE AREA
                             ============================================= -->

                        <div class="col-md-4">

                            <label
                                class="form-label tw-form-label"
                                for="<%= ddlServiceArea.ClientID %>">

                                Service Area *

                            </label>

                            <asp:DropDownList
                                ID="ddlServiceArea"
                                runat="server"
                                CssClass="form-select tw-glass-input">

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


                        <!-- =============================================
                             CITY
                             ============================================= -->

                        <div class="col-md-4">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtCity.ClientID %>">

                                City *

                            </label>

                            <asp:TextBox
                                ID="txtCity"
                                runat="server"
                                CssClass="form-control tw-glass-input"
                                Text="Durban"
                                ReadOnly="true" />

                        </div>


                        <!-- =============================================
                             PROVINCE
                             ============================================= -->

                        <div class="col-md-4">

                            <label
                                class="form-label tw-form-label"
                                for="<%= txtProvince.ClientID %>">

                                Province *

                            </label>

                            <asp:TextBox
                                ID="txtProvince"
                                runat="server"
                                CssClass="form-control tw-glass-input"
                                Text="KwaZulu-Natal"
                                ReadOnly="true" />

                        </div>

                    </div>


                    <!-- =================================================
                         CREATE PROFILE
                         ================================================= -->

                    <asp:Button
                        ID="btnRegister"
                        runat="server"
                        Text="Create Profile"
                        CssClass="tw-btn-primary-gradient w-100 py-3 mt-4"
                        OnClick="btnRegister_Click" />


                    <div class="tw-step-divider"></div>


                    <p class="text-center tw-text-muted mb-0">

                        Already registered or staff?

                        <a
                            href="~/Account/Login.aspx"
                            runat="server"
                            class="tw-card-link p-0">

                            Sign In Here

                        </a>

                    </p>

                </asp:Panel>


                <!-- =====================================================
                     SUCCESS PANEL
                     ===================================================== -->

                <asp:Panel
                    ID="pnlSuccess"
                    runat="server"
                    Visible="false"
                    CssClass="tw-glass-card tw-form-card text-center py-5">

                    <div class="tw-brand-icon mx-auto mb-3">

                        <i class="bi bi-check-circle-fill text-success"></i>

                    </div>

                    <h3 class="tw-text-primary fw-bold mb-2">
                        Profile Created Successfully!
                    </h3>

                    <p class="tw-text-muted mb-4">
                        Your account is ready. You can now request an
                        electrical or construction service directly.
                    </p>

                    <a
                        href="~/Default.aspx#request-service"
                        runat="server"
                        class="tw-btn-primary-gradient">

                        Request a Service

                    </a>

                </asp:Panel>

            </div>

        </div>

    </div>

</section>

<script type="text/javascript">

    // ============================================================
    // FIELD REFERENCES
    // ============================================================

    const registrationFields = {
        fullName: document.getElementById('<%= txtFullName.ClientID %>'),
        phone: document.getElementById('<%= txtPhone.ClientID %>'),
        email: document.getElementById('<%= txtEmail.ClientID %>'),
        customerType: document.getElementById('<%= ddlCustomerType.ClientID %>'),
        passport: document.getElementById('<%= txtPassportNumber.ClientID %>'),
        country: document.getElementById('<%= txtPassportCountry.ClientID %>'),
        nationality: document.getElementById('<%= ddlNationality.ClientID %>'),
        southAfricanID: document.getElementById('<%= txtSouthAfricanID.ClientID %>'),
        companyRegistration: document.getElementById('<%= txtCompanyRegistration.ClientID %>'),
        password: document.getElementById('<%= txtPassword.ClientID %>'),
        confirmPassword: document.getElementById('<%= txtConfirmPassword.ClientID %>'),
        address: document.getElementById('<%= txtAddress.ClientID %>')
    };

    const fieldErrors = {};

    // ============================================================
    // CREATE AN ERROR MESSAGE BELOW EACH FIELD
    // ============================================================

    function createFieldError(field, errorKey) {
        const error = document.createElement('small');

        error.id = 'validation-' + errorKey;
        error.className = 'registration-field-error text-danger';
        error.hidden = true;
        error.style.display = 'none';
        error.setAttribute('aria-live', 'polite');

        const anchor = field.closest('.position-relative') || field;
        anchor.insertAdjacentElement('afterend', error);

        fieldErrors[errorKey] = error;

        field.setAttribute('aria-describedby', [field.getAttribute('aria-describedby'), error.id].filter(Boolean).join(' '));
    }

    createFieldError(registrationFields.fullName, 'fullName');
    createFieldError(registrationFields.phone, 'phone');
    createFieldError(registrationFields.email, 'email');
    createFieldError(registrationFields.passport, 'passport');
    createFieldError(registrationFields.country, 'country');
    createFieldError(registrationFields.southAfricanID, 'southAfricanID');
    createFieldError(registrationFields.companyRegistration, 'companyRegistration');
    createFieldError(registrationFields.password, 'password');
    createFieldError(registrationFields.confirmPassword, 'confirmPassword');
    createFieldError(registrationFields.address, 'address');

    // ============================================================
    // SHOW OR CLEAR VALIDATION ERRORS
    // ============================================================

    function setFieldError(field, errorKey, message) {
        const error = fieldErrors[errorKey];

        if (message) {
            error.textContent = message;
            error.hidden = false;
            error.style.display = 'block';

            field.classList.add('is-invalid');
            field.setAttribute('aria-invalid', 'true');
        } else {
            error.textContent = '';
            error.hidden = true;
            error.style.display = 'none';

            field.classList.remove('is-invalid');
            field.removeAttribute('aria-invalid');
        }

        return !message;
    }

    // ============================================================
    // VALIDATE INDIVIDUAL FIELDS
    // ============================================================

    function validateField(fieldName) {
        const fields = registrationFields;
        const field = fields[fieldName];

        if (!field) {
            return true;
        }

        const value = field.value.trim();
        let error = '';

        switch (fieldName) {

            case 'fullName':
                if (!value) {
                    error = 'Please enter your full name.';
                } else if (value.length < 2 || value.length > 100) {
                    error = 'Full name must be between 2 and 100 characters.';
                } else if (!/^[\p{L}\p{M}]+(?:[ '’.-][\p{L}\p{M}]+)*$/u.test(value)) {
                    error = 'Use letters, spaces and normal name punctuation.';
                }
                break;

            case 'phone': {
                if (!value) {
                    error = 'Please enter your phone number.';
                } else {
                    const digits = value.replace(/\D/g, '');

                    if (digits.length !== 10) {
                        error = 'Enter a valid South African 10-digit phone number.';
                    }
                }
                break;
            }

            case 'email':
                if (!value) {
                    error = 'Please enter your email address.';
                } else if (value.length > 100) {
                    error = 'Email address must not exceed 100 characters.';
                } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value)) {
                    error = 'Please enter a valid email address.';
                }
                break;

            case 'password':
                if (!field.value) {
                    error = 'Please create a password.';
                } else if (
                    field.value.length < 8 ||
                    !/[A-Z]/.test(field.value) ||
                    !/[a-z]/.test(field.value) ||
                    !/[0-9]/.test(field.value) ||
                    !/[^A-Za-z0-9]/.test(field.value)
                ) {
                    error = 'Use at least 8 characters, including uppercase, lowercase, a number and a special character.';
                }

                // Recheck confirmation if it has already been entered.
                if (fields.confirmPassword.value) {
                    validateField('confirmPassword');
                }
                break;

            case 'confirmPassword':
                if (!field.value) {
                    error = 'Please confirm your password.';
                } else if (field.value !== fields.password.value) {
                    error = 'Passwords do not match.';
                }
                break;

            case 'address':
                if (!value) {
                    error = 'Please enter your full address.';
                } else if (value.length < 5) {
                    error = 'Please enter a more complete address.';
                } else if (value.length > 255) {
                    error = 'Address must not exceed 255 characters.';
                }
                break;

            case 'passport':
                if(fields.customerType.value==='Residential'&&fields.nationality.value==='Passport'&&(!value||value.length>20))error='Enter a passport number of up to 20 characters.';break;
            case 'country':
                if(fields.customerType.value==='Residential'&&fields.nationality.value==='Passport'&&(!value||value.length>50))error='Enter the issuing country, up to 50 characters.';break;
            case 'southAfricanID':
                if(fields.nationality.value==='Passport')break;
                if (fields.customerType.value !== 'Residential') {
                    break;
                }

                if (!value) {
                    error = 'Please enter your South African ID number.';
                } else if (!/^\d{13}$/.test(value)) {
                    error = 'South African ID number must contain exactly 13 digits.';
                }
                break;

            case 'companyRegistration':
                if (fields.customerType.value !== 'Business') {
                    break;
                }

                if (!value) {
                    error = 'Please enter your company registration number.';
                } else if (!/^\d{4}\/\d{6}\/\d{2}$/.test(value)) {
                    error = 'Use the format YYYY/NNNNNN/TT, for example 2008/069148/23.';
                }
                break;
        }

        return setFieldError(field, fieldName, error);
    }

    // ============================================================
    // VALIDATE WHEN THE USER LEAVES A FIELD (BLUR)
    // ============================================================

    Object.keys(fieldErrors).forEach(function (fieldName) {
        const field = registrationFields[fieldName];

        field.addEventListener('blur', function () {
            validateField(fieldName);
        });

        // Once an error has appeared, update it as the user types.
        field.addEventListener('input', function () {
            if (field.classList.contains('is-invalid')) {
                validateField(fieldName);
            }
        });
    });

    // Check password confirmation when either password changes.
    registrationFields.password.addEventListener('input', function () {
        if (
            registrationFields.confirmPassword.value &&
            registrationFields.confirmPassword.dataset.touched === 'true'
        ) {
            validateField('confirmPassword');
        }
    });

    registrationFields.confirmPassword.addEventListener('blur', function () {
        registrationFields.confirmPassword.dataset.touched = 'true';
    });

    // ============================================================
    // SHOW THE CORRECT CUSTOMER IDENTITY FIELD
    // ============================================================

    function updateIdentityField() {
        const fields = registrationFields;

        const residentialIdentity =
            document.getElementById('residentialIdentity');

        const businessIdentity =
            document.getElementById('businessIdentity');

        if (fields.customerType.value === 'Business') {
            residentialIdentity.style.display = 'none';
            businessIdentity.style.display = 'block';

            fields.southAfricanID.value = '';
            setFieldError(fields.southAfricanID, 'southAfricanID', '');
        } else {
            residentialIdentity.style.display = 'block';
            businessIdentity.style.display = 'none';

            fields.companyRegistration.value = '';
            setFieldError(fields.companyRegistration, 'companyRegistration', '');
        }
    }

    registrationFields.customerType.addEventListener(
        'change',
        updateIdentityField
    );

    // ============================================================
    // VALIDATE BEFORE SUBMITTING
    // ============================================================

    document.querySelector('form').addEventListener('submit', function (event) {
        const fieldsToValidate = [
            'fullName',
            'phone',
            'email',
            'password',
            'confirmPassword',
            'address'
        ];

        if (registrationFields.customerType.value === 'Residential') {
            if(registrationFields.nationality.value==='Passport')fieldsToValidate.push('passport','country');else fieldsToValidate.push('southAfricanID');
        } else {
            fieldsToValidate.push('companyRegistration');
        }

        let firstInvalidField = null;

        fieldsToValidate.forEach(function (fieldName) {
            if (!validateField(fieldName) && !firstInvalidField) {
                firstInvalidField = registrationFields[fieldName];
            }
        });

        if (firstInvalidField) {
            event.preventDefault();
            firstInvalidField.focus();
        }
    });

    // ============================================================
    // PASSWORD VISIBILITY
    // ============================================================

    function togglePassword(fieldId, btn) {
        const input = document.getElementById(fieldId);
        const icon = btn.querySelector('i');

        if (input.type === 'password') {
            input.type = 'text';
            icon.classList.remove('bi-eye');
            icon.classList.add('bi-eye-slash');
        } else {
            input.type = 'password';
            icon.classList.remove('bi-eye-slash');
            icon.classList.add('bi-eye');
        }
    }

    // ============================================================
    // INITIALISE
    // ============================================================

    updateIdentityField();
    if(window.ThyWillUI)window.ThyWillUI.initializeIdentity();

</script>


</div>
</asp:Content>