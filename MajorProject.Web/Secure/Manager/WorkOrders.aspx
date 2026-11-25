
<%@ Page
    Title="Create Job"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    Async="true"
    CodeBehind="WorkOrders.aspx.cs"
    Inherits="MajorProject.Web.Manager.WorkOrders" %>

<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-manager-workorders">


    

    <div class="dashboard-page admin-workorder">

        <!-- PAGE HEADER -->

        <div class="dashboard-page-header">
            <div>
                <div class="dashboard-eyebrow">
                    <i class="bi bi-briefcase"></i>
                    MANAGER
                </div>

                <h1 class="dashboard-page-title">
                    Create Job
                </h1>

                <p class="dashboard-page-subtitle">
                    Select a customer, add one or more services, assign a technician,
                    and save the job.
                </p>
            </div>
        </div>

        <!-- MESSAGES -->

        <asp:Panel
            ID="pnlSuccess"
            runat="server"
            Visible="false"
            CssClass="alert alert-success workorder-message"
            role="alert">

            <i class="bi bi-check-circle me-2"></i>

            <asp:Literal ID="litSuccess" runat="server" />

        </asp:Panel>

        <asp:Panel
            ID="pnlError"
            runat="server"
            Visible="false"
            CssClass="alert alert-danger workorder-message"
            role="alert">

            <i class="bi bi-exclamation-triangle me-2"></i>

            <asp:Literal ID="litError" runat="server" />

        </asp:Panel>

        <!-- 1. CUSTOMER SELECTION -->

        <section class="dashboard-panel workorder-customer-panel">

            <div class="dashboard-panel-header">
                <div>
                    <h2 class="dashboard-panel-title">
                        <i class="bi bi-person-check"></i>
                        1. Select Customer
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Only active customer accounts are available for new jobs.
                    </p>
                </div>
            </div>

            <div class="workorder-panel-body">

                <!-- CUSTOMER SEARCH -->

                <div class="workorder-customer-toolbar">

                    <div class="workorder-search">

                        <label
                            for="<%= txtCustomerSearch.ClientID %>"
                            class="workorder-search-label">
                            Search Customer
                        </label>

                        <asp:TextBox
                            ID="txtCustomerSearch"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Search by customer name..." />

                    </div>

                    <div class="workorder-search-actions">

                        <asp:Button
                            ID="btnSearchCustomers"
                            runat="server"
                            Text="Search"
                            CssClass="dashboard-btn dashboard-btn-primary"
                            CausesValidation="false"
                            OnClick="btnSearchCustomers_Click" />

                        <asp:Button
                            ID="btnClearCustomerSearch"
                            runat="server"
                            Text="Clear"
                            CssClass="dashboard-btn dashboard-btn-secondary"
                            CausesValidation="false"
                            OnClick="btnClearCustomerSearch_Click" />

                    </div>

                </div>

                <!-- CUSTOMER TABLE -->

                <div class="workorder-customer-table-wrapper">

                    <asp:GridView
                        ID="gvCustomers"
                        runat="server"
                        AutoGenerateColumns="false"
                        AllowSorting="true"
                        DataKeyNames="CustomerID"
                        CssClass="dashboard-table workorder-customer-table"
                        GridLines="None"
                        EmptyDataText="No active customers were found."
                        OnRowCommand="gvCustomers_RowCommand"
                        OnSorting="gvCustomers_Sorting">

                        <Columns>

                            <asp:BoundField
                                DataField="FullName"
                                HeaderText="Customer"
                                SortExpression="FullName" />

                            <asp:BoundField
                                DataField="PhoneNumber"
                                HeaderText="Phone"
                                SortExpression="PhoneNumber" />

                            <asp:BoundField
                                DataField="EmailAddress"
                                HeaderText="Email"
                                SortExpression="EmailAddress" />

                            <asp:BoundField
                                DataField="Area"
                                HeaderText="Area"
                                SortExpression="Area" />

                            <asp:BoundField
                                DataField="CustomerType"
                                HeaderText="Type"
                                SortExpression="CustomerType" />

                            <asp:TemplateField HeaderText="Action">

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="btnSelectCustomer"
                                        runat="server"
                                        CommandName="SelectCustomer"
                                        CommandArgument='<%# Eval("CustomerID") %>'
                                        CausesValidation="false"
                                        CssClass="dashboard-table-action dashboard-table-action-primary">

                                        <i class="bi bi-check2-circle me-1"></i>
                                        Select

                                    </asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>

                </div>

                <!-- SELECTED CUSTOMER -->

                <asp:Panel
                    ID="pnlSelectedCustomer"
                    runat="server"
                    Visible="false"
                    CssClass="workorder-selected-customer">

                    <div class="workorder-selected-customer-name">

                        <i class="bi bi-person-fill"></i>

                        <asp:Label
                            ID="lblSelectedCustomerName"
                            runat="server" />

                    </div>

                    <div class="workorder-selected-customer-meta">

                        <asp:Label
                            ID="lblSelectedCustomerDetails"
                            runat="server" />

                    </div>

                </asp:Panel>

            </div>

        </section>

        <!-- 2. ADD SERVICE -->

        <section class="dashboard-panel workorder-details-panel">

            <div class="dashboard-panel-header">

                <div>
                    <h2 class="dashboard-panel-title">
                        <i class="bi bi-tools"></i>
                        2. Add Service
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Configure the service and job assignment.
                    </p>
                </div>

            </div>

            <div class="workorder-panel-body">

                <div class="workorder-form-grid">

                    <!-- SERVICE TYPE -->

                    <div class="workorder-form-group">

                        <label
                            for="<%= ddlServiceType.ClientID %>"
                            class="workorder-form-label workorder-required">
                            Service Type
                        </label>

                        <asp:DropDownList
                            ID="ddlServiceType"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem
                                Text="-- Select service type --"
                                Value="" />

                        </asp:DropDownList>

                    </div>

                    <!-- QUANTITY -->

                    <div class="workorder-form-group">

                        <label
                            for="<%= txtQuantity.ClientID %>"
                            class="workorder-form-label workorder-required">
                            Quantity
                        </label>

                        <asp:TextBox
                            ID="txtQuantity"
                            runat="server"
                            Text="1"
                            TextMode="Number"
                            min="1"
                            max="1000"
                            step="1"
                            CssClass="form-control quantity-input" />

                        <div class="workorder-form-help">
                            Enter a whole number from 1 to 1000.
                        </div>

                    </div>

                    <!-- PROMISE DATE -->

                    <div class="workorder-form-group">

                        <label
                            for="<%= txtPromiseDate.ClientID %>"
                            class="workorder-form-label workorder-required">
                            Promise Date
                        </label>

                        <asp:TextBox
                            ID="txtPromiseDate"
                            runat="server"
                            TextMode="Date"
                            CssClass="form-control"
                            AutoPostBack="true"
                            OnTextChanged="txtPromiseDate_TextChanged" />

                    </div>

                    <!-- TECHNICIAN -->

                    <div class="workorder-form-group workorder-form-group-wide">

                        <label
                            for="<%= ddlTechnician.ClientID %>"
                            class="workorder-form-label workorder-required">
                            Technician
                        </label>

                        <asp:DropDownList
                            ID="ddlTechnician"
                            runat="server"
                            CssClass="form-select">

                            <asp:ListItem
                                Text="-- Select technician --"
                                Value="" />

                        </asp:DropDownList>

                        <div class="workorder-form-help">
                            <i class="bi bi-info-circle me-1"></i>
                            Technicians shown here are available for the selected
                            promise date.
                        </div>

                    </div>

                    <!-- JOB NOTE -->

                    <div class="workorder-form-group workorder-form-group-wide">

                        <label
                            for="<%= txtJobNote.ClientID %>"
                            class="workorder-form-label">
                            Job Note
                        </label>

                        <asp:TextBox
                            ID="txtJobNote"
                            runat="server"
                            TextMode="MultiLine"
                            Rows="4"
                            MaxLength="255"
                            CssClass="form-control"
                            placeholder="Enter any notes relevant to this job..." />

                        <div class="workorder-form-help">
                            Maximum 255 characters.
                        </div>

                    </div>

                </div>

                <!-- ADD SERVICE -->

                <div class="workorder-add-service">

                    <asp:Button
                        ID="btnAddService"
                        runat="server"
                        Text="Add Service"
                        CssClass="dashboard-btn dashboard-btn-primary"
                        OnClientClick="return validateServiceEntryClient();"
                        OnClick="btnAddService_Click" />

                </div>

            </div>

        </section>

        <!-- 3. JOB SERVICES -->

        <section class="dashboard-panel workorder-cart-panel">

            <div class="dashboard-panel-header">

                <div>
                    <h2 class="dashboard-panel-title">
                        <i class="bi bi-cart3"></i>
                        3. Job Services
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Review the services before saving the job.
                    </p>
                </div>

            </div>

            <div class="workorder-panel-body">

                <!-- CART -->

                <div class="workorder-cart-table-wrapper">

                    <asp:GridView
                        ID="gvCart"
                        runat="server"
                        AutoGenerateColumns="false"
                        DataKeyNames="CartLineID"
                        CssClass="dashboard-table workorder-cart-table"
                        GridLines="None"
                        EmptyDataText="No services have been added yet."
                        OnRowCommand="gvCart_RowCommand">

                        <Columns>

                            <asp:BoundField
                                DataField="JobTypeName"
                                HeaderText="Service" />

                            <asp:BoundField
                                DataField="Quantity"
                                HeaderText="Quantity" />

                            <asp:TemplateField HeaderText="Action">

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="btnRemoveService"
                                        runat="server"
                                        CommandName="RemoveService"
                                        CommandArgument='<%# Eval("CartLineID") %>'
                                        CssClass="dashboard-table-action dashboard-table-action-danger"
                                        CausesValidation="false">

                                        <i class="bi bi-trash me-1"></i>
                                        Remove

                                    </asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>

                </div>

                <!-- JOB ACTIONS -->

                <div class="workorder-actions">

                    <asp:Button
                        ID="btnCancelJob"
                        runat="server"
                        Text="Cancel Job"
                        CssClass="dashboard-btn dashboard-btn-secondary"
                        CausesValidation="false"
                        OnClick="btnCancelJob_Click" />

                    <asp:Button
                        ID="btnSaveJob"
                        runat="server"
                        Text="Save Job"
                        CssClass="dashboard-btn dashboard-btn-success"
                        OnClientClick="return validateJobClient();"
                        OnClick="btnSaveJob_Click" />

                </div>

            </div>

        </section>

    </div>

    <script>
        (function () {
            "use strict";

            var maxQuantity = 1000;
            var maxNoteLength = 255;

            var ids = {
                serviceType: "<%= ddlServiceType.ClientID %>",
                quantity: "<%= txtQuantity.ClientID %>",
                promiseDate: "<%= txtPromiseDate.ClientID %>",
                technician: "<%= ddlTechnician.ClientID %>",
                jobNote: "<%= txtJobNote.ClientID %>"
            };

            var touched = {};

            function getElement(id) {
                return document.getElementById(id);
            }

            function getErrorElement(field) {
                var group = field.closest(".workorder-form-group");

                if (!group) {
                    return null;
                }

                var error = group.querySelector(".workorder-field-error");

                if (!error) {
                    error = document.createElement("small");
                    error.className = "workorder-field-error";
                    error.setAttribute("role", "alert");
                    error.setAttribute("aria-live", "polite");
                    group.appendChild(error);
                }

                return error;
            }

            function setFieldState(field, message) {
                if (!field) {
                    return false;
                }

                var error = getErrorElement(field);

                field.classList.remove("workorder-field-invalid");
                field.classList.remove("workorder-field-valid");

                if (message) {
                    field.classList.add("workorder-field-invalid");
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
                    field.classList.add("workorder-field-valid");
                }

                return true;
            }

            function validateServiceType() {
                var field = getElement(ids.serviceType);

                if (!field) {
                    return true;
                }

                if (!field.value) {
                    return setFieldState(field, "Please select a service type.");
                }

                return setFieldState(field, "");
            }

            function validateQuantity() {
                var field = getElement(ids.quantity);

                if (!field) {
                    return true;
                }

                var value = String(field.value || "").trim();

                if (value === "") {
                    return setFieldState(field, "Please enter a quantity.");
                }

                if (!/^\d+$/.test(value)) {
                    return setFieldState(field, "Quantity must be a whole number.");
                }

                var quantity = Number(value);

                if (!Number.isSafeInteger(quantity) || quantity <= 0) {
                    return setFieldState(field, "Quantity must be greater than zero.");
                }

                if (quantity > maxQuantity) {
                    return setFieldState(
                        field,
                        "Quantity cannot exceed " + maxQuantity + " per service line."
                    );
                }

                return setFieldState(field, "");
            }

            function validatePromiseDate() {
                var field = getElement(ids.promiseDate);

                if (!field) {
                    return true;
                }

                var value = String(field.value || "").trim();

                if (!value) {
                    return setFieldState(field, "Please select a promise date.");
                }

                var match = /^(\d{4})-(\d{2})-(\d{2})$/.exec(value);

                if (!match) {
                    return setFieldState(field, "Please enter a valid promise date.");
                }

                var year = Number(match[1]);
                var month = Number(match[2]);
                var day = Number(match[3]);
                var date = new Date(Date.UTC(year, month - 1, day));

                if (
                    date.getUTCFullYear() !== year ||
                    date.getUTCMonth() !== month - 1 ||
                    date.getUTCDate() !== day
                ) {
                    return setFieldState(field, "Please enter a valid calendar date.");
                }

                var today = new Date();
                var todayKey =
                    today.getFullYear() + "-" +
                    String(today.getMonth() + 1).padStart(2, "0") + "-" +
                    String(today.getDate()).padStart(2, "0");

                if (value < todayKey) {
                    return setFieldState(field, "Promise date cannot be earlier than today.");
                }

                return setFieldState(field, "");
            }

            function validateTechnician() {
                var field = getElement(ids.technician);

                if (!field) {
                    return true;
                }

                if (!field.value) {
                    return setFieldState(field, "Please assign a technician.");
                }

                return setFieldState(field, "");
            }

            function validateJobNote() {
                var field = getElement(ids.jobNote);

                if (!field) {
                    return true;
                }

                var value = field.value || "";

                if (value.trim().length > maxNoteLength) {
                    return setFieldState(
                        field,
                        "The job note cannot exceed " + maxNoteLength + " characters."
                    );
                }

                return setFieldState(field, "");
            }

            function validateField(key) {
                switch (key) {
                    case "serviceType":
                        return validateServiceType();

                    case "quantity":
                        return validateQuantity();

                    case "promiseDate":
                        return validatePromiseDate();

                    case "technician":
                        return validateTechnician();

                    case "jobNote":
                        return validateJobNote();

                    default:
                        return true;
                }
            }

            function validateFields(keys) {
                var valid = true;
                var firstInvalid = null;

                keys.forEach(function (key) {
                    touched[key] = true;

                    if (!validateField(key)) {
                        valid = false;

                        if (!firstInvalid) {
                            firstInvalid = getElement(ids[key]);
                        }
                    }
                });

                if (firstInvalid) {
                    firstInvalid.focus();
                }

                return valid;
            }

            window.validateServiceEntryClient = function () {
                return validateFields([
                    "serviceType",
                    "quantity",
                    "promiseDate",
                    "technician"
                ]);
            };

            window.validateJobClient = function () {
                var valid = validateFields([
                    "promiseDate",
                    "technician",
                    "jobNote"
                ]);

                /*
                 * Service type and quantity are validated when adding each
                 * service. The server validates that a customer is selected
                 * and that the job cart contains valid service lines.
                 */
                return valid;
            };

            function initialiseValidation() {
                Object.keys(ids).forEach(function (key) {
                    var field = getElement(ids[key]);

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

                    /*
                     * Promise-date changes trigger an ASP.NET postback.
                     * Blur validation runs before the user changes focus;
                     * server-side date validation remains authoritative.
                     */
                });
            }

            if (document.readyState === "loading") {
                document.addEventListener("DOMContentLoaded", initialiseValidation);
            } else {
                initialiseValidation();
            }
        })();
    </script>


</div>

</div>
</asp:Content>
