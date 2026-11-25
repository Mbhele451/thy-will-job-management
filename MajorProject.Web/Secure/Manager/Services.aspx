<%@ Page
    Title="Service Management"
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Services.aspx.cs"
    Inherits="MajorProject.Web.Secure.Manager.Services" %>

<asp:Content
    ID="MainContent"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-manager-services">


    

    <asp:UpdatePanel
        ID="updServices"
        runat="server">

        <ContentTemplate>
<div class="tw-record-workspace" data-record-tabs="secure-manager-services" data-tab-labels="list=Services;details=Service details" data-selected-id="pnlServiceEditor">

<asp:Label
                ID="lblMessage"
                runat="server"
                EnableViewState="false"
                CssClass="d-block mb-3" />


            <!-- PAGE HEADER -->
            <section class="dashboard-page-header reveal">

                <div>
                    <div class="dashboard-eyebrow">
                        MANAGER
                    </div>

                    <h1 class="dashboard-page-title">
                        Service Management
                    </h1>

                    <p class="dashboard-page-subtitle">
                        Manage services, pricing and the inventory materials required to perform each service.
                    </p>
                </div>

            </section>


            <!-- MESSAGE -->
            


            <!-- SERVICE LIST -->
            <section class="dashboard-panel reveal"  data-tab-pane="list">

                <div class="dashboard-panel-header">

                    <div>
                        <h2 class="dashboard-panel-title">
                            Services
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            Select a service to view or update its definition.
                        </p>
                    </div>

                </div>


                <div class="dashboard-table-wrapper">

                    <asp:GridView
                        ID="gvServices"
                        runat="server"
                        CssClass="dashboard-table"
                        AutoGenerateColumns="False"
                        DataKeyNames="JobTypeID"
                        EmptyDataText="No services found."
                        OnSelectedIndexChanged="gvServices_SelectedIndexChanged">

                        <Columns>

                            <asp:CommandField
                                ShowSelectButton="True"
                                SelectText="Manage"
                                ButtonType="Button"
                                ControlStyle-CssClass="dashboard-btn dashboard-btn-sm" />

                            <asp:BoundField
                                DataField="JobTypeName"
                                HeaderText="Service" />

                            <asp:BoundField
                                DataField="BaseLabourCost"
                                HeaderText="Labour"
                                DataFormatString="R {0:N2}" />

                            <asp:BoundField
                                DataField="BaseTransportCost"
                                HeaderText="Transport"
                                DataFormatString="R {0:N2}" />

                            <asp:BoundField
                                DataField="TaxPercentage"
                                HeaderText="VAT"
                                DataFormatString="{0:N2}%" />

                        </Columns>

                    </asp:GridView>

                </div>

            </section>


            <!-- SELECTED SERVICE -->
            <asp:Panel
                ID="pnlServiceEditor"
                runat="server"
                Visible="false"  data-tab-pane="details" data-ready-id="pnlServiceEditor" data-ready-kind="exists">

                <!-- SERVICE DETAILS -->
                <section class="dashboard-panel mt-4 reveal">

                    <div class="dashboard-panel-header">

                        <div>
                            <h2 class="dashboard-panel-title">
                                Selected Service
                            </h2>

                            <p class="dashboard-panel-subtitle">
                                Update the service name and pricing used when customers book this service.
                            </p>
                        </div>

                    </div>


                    <div class="p-4">

                        <asp:HiddenField
                            ID="hfJobTypeID"
                            runat="server" />


                        <div class="row g-3">

                            <div class="col-lg-6">

                                <label
                                    for="<%= txtServiceName.ClientID %>"
                                    class="form-label">
                                    Service Name
                                </label>

                                <asp:TextBox
                                    ID="txtServiceName"
                                    runat="server"
                                    CssClass="form-control"
                                    MaxLength="100" />

                            </div>


                            <div class="col-lg-2">

                                <label
                                    for="<%= txtLabourCost.ClientID %>"
                                    class="form-label">
                                    Labour Cost
                                </label>

                                <asp:TextBox
                                    ID="txtLabourCost"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="Number"
                                    step="0.01" />

                            </div>


                            <div class="col-lg-2">

                                <label
                                    for="<%= txtTransportCost.ClientID %>"
                                    class="form-label">
                                    Transport Cost
                                </label>

                                <asp:TextBox
                                    ID="txtTransportCost"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="Number"
                                    step="0.01" />

                            </div>


                            <div class="col-lg-2">

                                <label
                                    for="<%= txtTaxPercentage.ClientID %>"
                                    class="form-label">
                                    VAT %
                                </label>

                                <asp:TextBox
                                    ID="txtTaxPercentage"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="Number"
                                    step="0.01" />

                            </div>

                        </div>


                        <div class="dashboard-action-group mt-4">

                            <asp:Button
                                ID="btnSaveService"
                                runat="server"
                                Text="Save Service"
                                CssClass="dashboard-btn"
                                OnClick="btnSaveService_Click"
                                OnClientClick="return validateServiceDetails();" />

                        </div>

                    </div>

                </section>


                <!-- COST SUMMARY -->
                <section class="dashboard-panel mt-4 reveal">

                    <div class="dashboard-panel-header">

                        <div>
                            <h2 class="dashboard-panel-title">
                                Cost Summary
                            </h2>

                            <p class="dashboard-panel-subtitle">
                                Current service costs based on its configured materials and pricing.
                            </p>
                        </div>

                    </div>


                    <div class="row g-3 p-4">

                        <div class="col-lg-4">

                            <div class="dashboard-stat-card">

                                <div class="dashboard-stat-icon dashboard-stat-icon-primary">
                                    <i class="bi bi-person-workspace"></i>
                                </div>

                                <div class="dashboard-stat-content">

                                    <span class="dashboard-stat-label">
                                        Labour
                                    </span>

                                    <span class="dashboard-stat-value">
                                        R <asp:Label
                                            ID="lblLabourCost"
                                            runat="server"
                                            Text="0.00" />
                                    </span>

                                </div>

                            </div>

                        </div>


                        <div class="col-lg-4">

                            <div class="dashboard-stat-card">

                                <div class="dashboard-stat-icon dashboard-stat-icon-info">
                                    <i class="bi bi-box-seam"></i>
                                </div>

                                <div class="dashboard-stat-content">

                                    <span class="dashboard-stat-label">
                                        Materials
                                    </span>

                                    <span class="dashboard-stat-value">
                                        R <asp:Label
                                            ID="lblMaterialCost"
                                            runat="server"
                                            Text="0.00" />
                                    </span>

                                </div>

                            </div>

                        </div>


                        <div class="col-lg-4">

                            <div class="dashboard-stat-card">

                                <div class="dashboard-stat-icon dashboard-stat-icon-success">
                                    <i class="bi bi-truck"></i>
                                </div>

                                <div class="dashboard-stat-content">

                                    <span class="dashboard-stat-label">
                                        Transport
                                    </span>

                                    <span class="dashboard-stat-value">
                                        R <asp:Label
                                            ID="lblTransportCost"
                                            runat="server"
                                            Text="0.00" />
                                    </span>

                                </div>

                            </div>

                        </div>

                    </div>

                </section>


                <!-- MATERIAL REQUIREMENTS -->
                <section class="dashboard-panel mt-4 reveal">

                    <div class="dashboard-panel-header">

                        <div>
                            <h2 class="dashboard-panel-title">
                                Material Requirements
                            </h2>

                            <p class="dashboard-panel-subtitle">
                                Define the inventory items and quantities required to perform this service.
                            </p>
                        </div>

                    </div>


                    <div class="dashboard-table-wrapper">

                        <asp:GridView
                            ID="gvMaterials"
                            runat="server"
                            CssClass="dashboard-table"
                            AutoGenerateColumns="False"
                            DataKeyNames="JobTypeMaterialID"
                            EmptyDataText="No materials have been configured for this service."
                            OnRowCommand="gvMaterials_RowCommand">

                            <Columns>

                                <asp:BoundField
                                    DataField="ItemName"
                                    HeaderText="Material" />

                                <asp:BoundField
                                    DataField="Category"
                                    HeaderText="Category" />

                                <asp:BoundField
                                    DataField="QuantityRequired"
                                    HeaderText="Required" />

                                <asp:BoundField
                                    DataField="UnitOfMeasure"
                                    HeaderText="Unit" />

                                <asp:BoundField
                                    DataField="UnitPrice"
                                    HeaderText="Unit Price"
                                    DataFormatString="R {0:N2}" />

                                <asp:BoundField
                                    DataField="MaterialCost"
                                    HeaderText="Material Cost"
                                    DataFormatString="R {0:N2}" />


                                <asp:TemplateField
                                    HeaderText="Quantity">

                                    <ItemTemplate>

                                        <asp:TextBox
                                            ID="txtQuantity"
                                            runat="server"
                                            CssClass="form-control"
                                            Text='<%# Eval("QuantityRequired") %>'
                                            TextMode="Number"
                                            min="1" />

                                    </ItemTemplate>

                                </asp:TemplateField>


                                <asp:TemplateField
                                    HeaderText="Actions">

                                    <ItemTemplate>

                                        <div class="dashboard-action-group">

                                            <asp:Button
                                                ID="btnUpdateQuantity"
                                                runat="server"
                                                Text="Update"
                                                CssClass="dashboard-btn dashboard-btn-sm"
                                                CommandName="UpdateQuantity"
                                                CommandArgument='<%# Eval("JobTypeMaterialID") %>'
                                                OnClientClick="return validateMaterialQuantity(this);" />

                                            <asp:Button
                                                ID="btnRemoveMaterial"
                                                runat="server"
                                                Text="Remove"
                                                CssClass="dashboard-btn dashboard-btn-sm dashboard-btn-danger"
                                                CommandName="RemoveMaterial"
                                                CommandArgument='<%# Eval("JobTypeMaterialID") %>'
                                                CausesValidation="false"
                                                OnClientClick="return confirm('Remove this material from the service?');" />

                                        </div>

                                    </ItemTemplate>

                                </asp:TemplateField>

                            </Columns>

                        </asp:GridView>

                    </div>

                </section>


                <!-- ADD MATERIAL -->
                <section class="dashboard-panel mt-4 reveal">

                    <div class="dashboard-panel-header">

                        <div>
                            <h2 class="dashboard-panel-title">
                                Add Material
                            </h2>

                            <p class="dashboard-panel-subtitle">
                                Add an existing inventory item to this service definition.
                            </p>
                        </div>

                    </div>


                    <div class="p-4">

                        <div class="row g-3 align-items-end">

                            <div class="col-lg-8">

                                <label
                                    for="<%= ddlInventoryItems.ClientID %>"
                                    class="form-label">
                                    Inventory Item
                                </label>

                                <asp:DropDownList
                                    ID="ddlInventoryItems"
                                    runat="server"
                                    CssClass="form-select"
                                    AppendDataBoundItems="true">

                                    <asp:ListItem
                                        Text="Select an inventory item"
                                        Value="" />

                                </asp:DropDownList>

                            </div>


                            <div class="col-lg-2">

                                <label
                                    for="<%= txtMaterialQuantity.ClientID %>"
                                    class="form-label">
                                    Quantity
                                </label>

                                <asp:TextBox
                                    ID="txtMaterialQuantity"
                                    runat="server"
                                    CssClass="form-control"
                                    TextMode="Number"
                                    Text="1"
                                    min="1" />

                            </div>


                            <div class="col-lg-2">

                                <asp:Button
                                    ID="btnAddMaterial"
                                    runat="server"
                                    Text="Add Material"
                                    CssClass="dashboard-btn w-100"
                                    OnClick="btnAddMaterial_Click"
                                    OnClientClick="return validateAddMaterial();" />

                            </div>

                        </div>

                    </div>

                </section>

            </asp:Panel>


            <!-- NO SERVICE SELECTED -->
            <asp:Panel
                ID="pnlNoService"
                runat="server"
                CssClass="dashboard-panel mt-4 reveal">

                <div class="p-4 text-center">

                    <div class="dashboard-feature-icon mx-auto mb-3">
                        <i class="bi bi-tools"></i>
                    </div>

                    <h2 class="dashboard-panel-title">
                        No Service Selected
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Select a service above to manage its pricing and material requirements.
                    </p>

                </div>

            </asp:Panel>

        
</div>
</ContentTemplate>

    </asp:UpdatePanel>


    <!-- REAL-TIME VALIDATION -->
    <script type="text/javascript">
        (function () {
            "use strict";

            var ids = {
                serviceName: "<%= txtServiceName.ClientID %>",
                labourCost: "<%= txtLabourCost.ClientID %>",
                transportCost: "<%= txtTransportCost.ClientID %>",
                taxPercentage: "<%= txtTaxPercentage.ClientID %>",
                inventoryItem: "<%= ddlInventoryItems.ClientID %>",
                materialQuantity: "<%= txtMaterialQuantity.ClientID %>"
            };

            function getField(id) {
                return document.getElementById(id);
            }

            function getErrorElement(input) {
                if (!input) {
                    return null;
                }

                var errorId = input.id + "_validationError";
                var error = document.getElementById(errorId);

                if (error) {
                    return error;
                }

                error = document.createElement("span");
                error.id = errorId;
                error.className = "service-client-error";
                error.setAttribute("aria-live", "polite");
                error.hidden = true;

                input.insertAdjacentElement("afterend", error);

                return error;
            }

            function setError(input, message) {
                if (!input) {
                    return false;
                }

                var error = getErrorElement(input);

                if (message) {
                    input.classList.add("service-client-invalid");
                    input.classList.remove("service-client-valid");
                    input.setAttribute("aria-invalid", "true");

                    error.textContent = message;
                    error.hidden = false;

                    return false;
                }

                input.classList.remove("service-client-invalid");
                input.setAttribute("aria-invalid", "false");

                error.textContent = "";
                error.hidden = true;

                if (input.value.trim() !== "") {
                    input.classList.add("service-client-valid");
                } else {
                    input.classList.remove("service-client-valid");
                }

                return true;
            }

            function validateRequired(input, label) {
                if (!input) {
                    return true;
                }

                if (input.value.trim() === "") {
                    return setError(input, label + " is required.");
                }

                return setError(input, "");
            }

            function validateDecimal(input, label) {
                if (!input) {
                    return true;
                }

                var value = input.value.trim();

                if (value === "") {
                    return setError(input, label + " is required.");
                }

                /*
                 * The code-behind uses decimal.TryParse.
                 * Accept a signed decimal value here so the browser
                 * checks the numeric format without inventing a
                 * positivity rule not shown in the supplied handler.
                 */
                if (!/^[+-]?(?:\d+(?:\.\d*)?|\.\d+)$/.test(value)) {
                    return setError(input, "Enter a valid " + label.toLowerCase() + ".");
                }

                var number = Number(value);

                if (!isFinite(number)) {
                    return setError(input, "Enter a valid " + label.toLowerCase() + ".");
                }

                return setError(input, "");
            }

            function validateServiceName() {
                return validateRequired(
                    getField(ids.serviceName),
                    "Service name"
                );
            }

            function validateLabourCost() {
                return validateDecimal(
                    getField(ids.labourCost),
                    "Labour cost"
                );
            }

            function validateTransportCost() {
                return validateDecimal(
                    getField(ids.transportCost),
                    "Transport cost"
                );
            }

            function validateTaxPercentage() {
                return validateDecimal(
                    getField(ids.taxPercentage),
                    "VAT percentage"
                );
            }

            function validateInventoryItem() {
                var input = getField(ids.inventoryItem);

                if (!input) {
                    return true;
                }

                if (input.value === "") {
                    return setError(input, "Please select an inventory item.");
                }

                return setError(input, "");
            }

            function validateQuantity(input) {
                if (!input) {
                    return true;
                }

                var value = input.value.trim();

                if (value === "") {
                    return setError(input, "Quantity is required.");
                }

                /*
                 * int.TryParse accepts whole-number values.
                 * A positive quantity is required for a material.
                 */
                if (!/^\d+$/.test(value)) {
                    return setError(input, "Enter a whole-number quantity.");
                }

                var quantity = Number(value);

                if (!isFinite(quantity) ||
                    quantity < 1 ||
                    quantity > 2147483647) {
                    return setError(input, "Quantity must be a positive whole number.");
                }

                return setError(input, "");
            }

            function validateServiceDetails() {
                var valid = true;

                if (!validateServiceName()) {
                    valid = false;
                }

                if (!validateLabourCost()) {
                    valid = false;
                }

                if (!validateTransportCost()) {
                    valid = false;
                }

                if (!validateTaxPercentage()) {
                    valid = false;
                }

                if (!valid) {
                    focusFirstInvalid([
                        ids.serviceName,
                        ids.labourCost,
                        ids.transportCost,
                        ids.taxPercentage
                    ]);
                }

                return valid;
            }

            function validateAddMaterial() {
                var valid = true;

                if (!validateInventoryItem()) {
                    valid = false;
                }

                if (!validateQuantity(getField(ids.materialQuantity))) {
                    valid = false;
                }

                if (!valid) {
                    focusFirstInvalid([
                        ids.inventoryItem,
                        ids.materialQuantity
                    ]);
                }

                return valid;
            }

            function validateMaterialQuantity(button) {
                if (!button) {
                    return true;
                }

                var row = button.closest("tr");

                if (!row) {
                    return true;
                }

                var input = row.querySelector("input[id*='txtQuantity']");

                if (!input) {
                    return true;
                }

                var valid = validateQuantity(input);

                if (!valid) {
                    input.focus();
                }

                return valid;
            }

            function focusFirstInvalid(fieldIds) {
                for (var i = 0; i < fieldIds.length; i++) {
                    var input = getField(fieldIds[i]);

                    if (input &&
                        input.getAttribute("aria-invalid") === "true") {
                        input.focus();
                        return;
                    }
                }
            }

            function attachBlurAndInput(input, validator) {
                if (!input || input.getAttribute("data-validation-attached") === "true") {
                    return;
                }

                input.setAttribute("data-validation-attached", "true");

                input.addEventListener("blur", function () {
                    validator();
                });

                input.addEventListener("input", function () {
                    /*
                     * Do not show an error before the user interacts
                     * with the field. Once an error is displayed,
                     * update it immediately as the user corrects it.
                     */
                    if (input.getAttribute("aria-invalid") === "true" ||
                        input.classList.contains("service-client-valid")) {
                        validator();
                    }
                });

                if (input.tagName === "SELECT") {
                    input.addEventListener("change", validator);
                }
            }

            function initializeValidation() {
                attachBlurAndInput(
                    getField(ids.serviceName),
                    validateServiceName
                );

                attachBlurAndInput(
                    getField(ids.labourCost),
                    validateLabourCost
                );

                attachBlurAndInput(
                    getField(ids.transportCost),
                    validateTransportCost
                );

                attachBlurAndInput(
                    getField(ids.taxPercentage),
                    validateTaxPercentage
                );

                attachBlurAndInput(
                    getField(ids.inventoryItem),
                    validateInventoryItem
                );

                attachBlurAndInput(
                    getField(ids.materialQuantity),
                    function () {
                        validateQuantity(getField(ids.materialQuantity));
                    }
                );

                /*
                 * GridView quantity TextBoxes are created dynamically,
                 * so locate and bind each current row after every render.
                 */
                var grid = document.getElementById(
                    "<%= gvMaterials.ClientID %>"
                );

                if (grid) {
                    var quantityInputs = grid.querySelectorAll(
                        "input[id*='txtQuantity']"
                    );

                    for (var i = 0; i < quantityInputs.length; i++) {
                        (function (input) {
                            attachBlurAndInput(input, function () {
                                validateQuantity(input);
                            });
                        })(quantityInputs[i]);
                    }
                }
            }

            /*
             * Functions referenced by ASP.NET OnClientClick.
             */
            window.validateServiceDetails = validateServiceDetails;
            window.validateAddMaterial = validateAddMaterial;
            window.validateMaterialQuantity = validateMaterialQuantity;

            /*
             * Initial page load.
             */
            if (document.readyState === "loading") {
                document.addEventListener(
                    "DOMContentLoaded",
                    initializeValidation
                );
            } else {
                initializeValidation();
            }

            /*
             * Reattach listeners after UpdatePanel partial postbacks.
             * The data-validation-attached attribute prevents duplicate
             * listeners on controls that remain in the DOM.
             */
            if (window.Sys &&
                Sys.WebForms &&
                Sys.WebForms.PageRequestManager) {

                var pageRequestManager =
                    Sys.WebForms.PageRequestManager.getInstance();

                pageRequestManager.add_endRequest(function () {
                    initializeValidation();
                });
            }

        })();
    </script>


</div>

</div>
</asp:Content>