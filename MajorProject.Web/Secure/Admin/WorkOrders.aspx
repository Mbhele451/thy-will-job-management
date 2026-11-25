
<%@ Page
    Title="Create Job"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    Async="true"
    CodeBehind="WorkOrders.aspx.cs"
    Inherits="MajorProject.Web.Admin.WorkOrders" %>

<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-admin-workorders">


    

    <div class="dashboard-page admin-workorder">

        <!-- PAGE HEADER -->

        <div class="dashboard-page-header">
            <div>
                <div class="dashboard-eyebrow">
                    <i class="bi bi-briefcase"></i>
                    ADMINISTRATION
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

            <asp:Literal
                ID="litSuccess"
                runat="server" />

        </asp:Panel>

        <asp:Panel
            ID="pnlError"
            runat="server"
            Visible="false"
            CssClass="alert alert-danger workorder-message"
            role="alert">

            <i class="bi bi-exclamation-triangle me-2"></i>

            <asp:Literal
                ID="litError"
                runat="server" />

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
                            step="1"
                            CssClass="form-control quantity-input" />

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
                        OnClick="btnAddService_Click"
                        OnClientClick="return validateAddService();" />

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
                        OnClick="btnCancelJob_Click"
                        OnClientClick="return confirm('Cancel this job creation process?');" />

                    <asp:Button
                        ID="btnSaveJob"
                        runat="server"
                        Text="Save Job"
                        CssClass="dashboard-btn dashboard-btn-success"
                        OnClick="btnSaveJob_Click"
                        OnClientClick="return validateSaveJob();" />

                </div>

            </div>
        </section>

    </div>


    <!-- CLIENT-SIDE VALIDATION -->

    <script type="text/javascript">
        (function () {
            "use strict";

            var ids = {
                serviceType: "<%= ddlServiceType.ClientID %>",
                quantity: "<%= txtQuantity.ClientID %>",
                promiseDate: "<%= txtPromiseDate.ClientID %>",
                technician: "<%= ddlTechnician.ClientID %>",
                jobNote: "<%= txtJobNote.ClientID %>",
                customerPanel: "<%= pnlSelectedCustomer.ClientID %>",
                customerName: "<%= lblSelectedCustomerName.ClientID %>",
                cart: "<%= gvCart.ClientID %>"
            };

            function getElement(key) {
                return document.getElementById(ids[key]);
            }

            function getErrorElement(input) {
                if (!input) {
                    return null;
                }

                var errorId = input.id + "_clientError";
                var error = document.getElementById(errorId);

                if (!error) {
                    error = document.createElement("span");
                    error.id = errorId;
                    error.className = "workorder-client-error";
                    error.setAttribute("aria-live", "polite");
                    error.hidden = true;

                    input.insertAdjacentElement("afterend", error);
                }

                return error;
            }

            function setError(input, message) {
                if (!input) {
                    return !message;
                }

                var error = getErrorElement(input);

                input.classList.remove("workorder-client-valid");

                if (message) {
                    input.classList.add("workorder-client-invalid");
                    input.setAttribute("aria-invalid", "true");

                    error.textContent = message;
                    error.hidden = false;

                    return false;
                }

                input.classList.remove("workorder-client-invalid");
                input.setAttribute("aria-invalid", "false");

                error.textContent = "";
                error.hidden = true;

                if (input.value.trim() !== "") {
                    input.classList.add("workorder-client-valid");
                }

                return true;
            }

            function validateServiceType() {
                var input = getElement("serviceType");

                if (!input) {
                    return true;
                }

                if (!input.value) {
                    return setError(input, "Please select a service type.");
                }

                return setError(input, "");
            }

            function validateQuantity() {
                var input = getElement("quantity");

                if (!input) {
                    return true;
                }

                var value = input.value.trim();

                if (value === "") {
                    return setError(input, "Quantity is required.");
                }

                if (!/^[0-9]+$/.test(value)) {
                    return setError(input, "Quantity must be a whole number.");
                }

                var quantity = Number(value);

                if (!Number.isSafeInteger(quantity) || quantity < 1) {
                    return setError(input, "Quantity must be at least 1.");
                }

                return setError(input, "");
            }

            function getLocalToday() {
                var today = new Date();

                var year = today.getFullYear();
                var month = String(today.getMonth() + 1).padStart(2, "0");
                var day = String(today.getDate()).padStart(2, "0");

                return year + "-" + month + "-" + day;
            }

            function validatePromiseDate() {
                var input = getElement("promiseDate");

                if (!input) {
                    return true;
                }

                var value = input.value;

                if (!value) {
                    return setError(input, "Promise date is required.");
                }

                if (!/^\d{4}-\d{2}-\d{2}$/.test(value)) {
                    return setError(input, "Please enter a valid promise date.");
                }

                var selectedDate = new Date(value + "T00:00:00");

                if (isNaN(selectedDate.getTime())) {
                    return setError(input, "Please enter a valid promise date.");
                }

                if (value < getLocalToday()) {
                    return setError(input, "Promise date cannot be in the past.");
                }

                return setError(input, "");
            }

            function validateTechnician() {
                var input = getElement("technician");

                if (!input) {
                    return true;
                }

                if (!input.value) {
                    return setError(input, "Please select an available technician.");
                }

                return setError(input, "");
            }

            function validateJobNote() {
                var input = getElement("jobNote");

                if (!input) {
                    return true;
                }

                if (input.value.length > 255) {
                    return setError(input, "Job note cannot exceed 255 characters.");
                }

                return setError(input, "");
            }

            function customerIsSelected() {
                var panel = getElement("customerPanel");
                var name = getElement("customerName");

                if (!panel || !name) {
                    return false;
                }

                return panel.style.display !== "none" &&
                    name.textContent.trim() !== "";
            }

            function getCartRowCount() {
                var cart = getElement("cart");

                if (!cart) {
                    return 0;
                }

                var rows = cart.querySelectorAll("tr");
                var count = 0;

                for (var i = 0; i < rows.length; i++) {
                    /*
                     * GridView rows containing data cells have a TD.
                     * Header and empty-data rows do not count.
                     */
                    if (rows[i].querySelector("td") &&
                        !rows[i].querySelector("td[colspan]")) {
                        count++;
                    }
                }

                return count;
            }

            function focusFirstInvalid(inputs) {
                for (var i = 0; i < inputs.length; i++) {
                    if (inputs[i] &&
                        inputs[i].getAttribute("aria-invalid") === "true") {
                        inputs[i].focus();
                        return;
                    }
                }
            }

            function validateServiceFields() {
                var valid = true;

                if (!validateServiceType()) {
                    valid = false;
                }

                if (!validateQuantity()) {
                    valid = false;
                }

                if (!validatePromiseDate()) {
                    valid = false;
                }

                if (!validateTechnician()) {
                    valid = false;
                }

                if (!validateJobNote()) {
                    valid = false;
                }

                if (!valid) {
                    focusFirstInvalid([
                        getElement("serviceType"),
                        getElement("quantity"),
                        getElement("promiseDate"),
                        getElement("technician"),
                        getElement("jobNote")
                    ]);
                }

                return valid;
            }

            function validateAddService() {
                var valid = validateServiceFields();

                if (!customerIsSelected()) {
                    window.alert("Please select a customer before adding a service.");
                    return false;
                }

                return valid;
            }

            function validateSaveJob() {
                if (!customerIsSelected()) {
                    window.alert("Please select a customer before saving the job.");
                    return false;
                }

                if (getCartRowCount() < 1) {
                    window.alert("Please add at least one service before saving the job.");
                    return false;
                }

                return true;
            }

            function attachValidation(key, validator) {
                var input = getElement(key);

                if (!input) {
                    return;
                }

                input.addEventListener("blur", validator);

                input.addEventListener("input", function () {
                    if (input.getAttribute("aria-invalid") === "true" ||
                        input.classList.contains("workorder-client-valid")) {
                        validator();
                    }
                });

                if (input.tagName === "SELECT") {
                    input.addEventListener("change", validator);
                }
            }

            function initializeValidation() {
                attachValidation("serviceType", validateServiceType);
                attachValidation("quantity", validateQuantity);
                attachValidation("promiseDate", validatePromiseDate);
                attachValidation("technician", validateTechnician);
                attachValidation("jobNote", validateJobNote);

                /*
                 * Set the native minimum date as an additional safeguard.
                 * The JavaScript validation above still handles feedback.
                 */
                var promiseDate = getElement("promiseDate");

                if (promiseDate) {
                    promiseDate.min = getLocalToday();
                }
            }

            window.validateAddService = validateAddService;
            window.validateSaveJob = validateSaveJob;

            if (document.readyState === "loading") {
                document.addEventListener("DOMContentLoaded", initializeValidation);
            } else {
                initializeValidation();
            }

        })();
    </script>


</div>

</div>
</asp:Content>