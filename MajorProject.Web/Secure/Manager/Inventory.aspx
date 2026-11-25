<%@ Page
    Language="C#"
    MasterPageFile="~/MasterPages/Dashboard.Master"
    AutoEventWireup="true"
    CodeBehind="Inventory.aspx.cs"
    Inherits="MajorProject.Web.Secure.Manager.Inventory"
    Title="Inventory" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">
<div class="tw-page-stage" data-page-stage>
<div class="tw-page-secure-manager-inventory">


    

    <div class="dashboard-page manager-inventory"  data-record-tabs="secure-manager-inventory" data-tab-labels="list=Inventory;details=Item details;add=Add item" data-selected-id="hfSelectedItemID">
<asp:Label
            ID="lblMessage"
            runat="server"
            CssClass="dashboard-alert"
            Visible="false" />


        <div class="dashboard-page-header">

            <div>

                <span class="dashboard-eyebrow">
                    INVENTORY MANAGEMENT
                </span>

                <h1 class="dashboard-page-title">
                    Inventory
                </h1>

                <p class="dashboard-page-subtitle">
                    Manage materials, stock levels and inventory items.
                </p>

            </div>

        </div>


        <section class="dashboard-panel manager-inventory-toolbar"  data-tab-pane="list">

            <div class="dashboard-panel-header">

                <div>

                    <h2 class="dashboard-panel-title">
                        Inventory Items
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Search and sort available materials.
                    </p>

                </div>

            </div>

            <div class="dashboard-panel-body">

                <div class="manager-inventory-filters">

                    <div class="dashboard-form-group">

                        <label for="<%= txtSearch.ClientID %>">
                            Search Item
                        </label>

                        <asp:TextBox
                            ID="txtSearch"
                            runat="server"
                            CssClass="dashboard-input"
                            placeholder="Search by item name..."
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
                                Value="ItemName"
                                Text="Item Name" />

                            <asp:ListItem
                                Value="QuantityAsc"
                                Text="Quantity: Low to High" />

                            <asp:ListItem
                                Value="QuantityDesc"
                                Text="Quantity: High to Low" />

                            <asp:ListItem
                                Value="Category"
                                Text="Category" />

                            <asp:ListItem
                                Value="PriceAsc"
                                Text="Price: Low to High" />

                            <asp:ListItem
                                Value="PriceDesc"
                                Text="Price: High to Low" />

                            <asp:ListItem
                                Value="UpdatedDesc"
                                Text="Recently Updated" />

                        </asp:DropDownList>

                    </div>


                    <div class="manager-inventory-filter-button">

                        <asp:Button
                            ID="btnClearSearch"
                            runat="server"
                            Text="Clear"
                            CssClass="dashboard-btn dashboard-btn-secondary"
                            OnClick="btnClearSearch_Click" />

                    </div>

                </div>

            </div>

        </section>


        <section class="dashboard-panel manager-inventory-table-panel"  data-tab-pane="list">

            <div class="dashboard-panel-header">

                <div>

                    <h2 class="dashboard-panel-title">
                        Stock
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Select an item to view its details.
                    </p>

                </div>

            </div>


            <div class="dashboard-table-wrapper manager-inventory-table-wrapper">

                <asp:GridView
                    ID="gvInventory"
                    runat="server"
                    AutoGenerateColumns="false"
                    CssClass="dashboard-table"
                    GridLines="None"
                    AllowPaging="false"
                    OnRowCommand="gvInventory_RowCommand"
                    EmptyDataText="No inventory items found.">

                    <Columns>

                        <asp:BoundField
                            DataField="ItemID"
                            HeaderText="ID" />

                        <asp:BoundField
                            DataField="ItemName"
                            HeaderText="Item Name" />

                        <asp:BoundField
                            DataField="Category"
                            HeaderText="Category" Visible="false" />

                        <asp:BoundField
                            DataField="QuantityInStock"
                            HeaderText="Quantity" />

                        <asp:BoundField
                            DataField="UnitOfMeasure"
                            HeaderText="Unit" />

                        <asp:BoundField
                            DataField="UnitPrice"
                            HeaderText="Unit Price"
                            DataFormatString="R {0:N2}" />

                        <asp:BoundField
                            DataField="ReorderLevel"
                            HeaderText="Reorder Level" />

                        <asp:BoundField
                            DataField="UpdatedAt"
                            HeaderText="Last Updated"
                            DataFormatString="{0:dd MMM yyyy HH:mm}" />

                        <asp:TemplateField
                            HeaderText="Action">

                            <ItemTemplate>

                                <asp:LinkButton
                                    ID="btnView"
                                    runat="server"
                                    CssClass="dashboard-btn dashboard-btn-small dashboard-btn-secondary"
                                    CommandName="ViewItem"
                                    CommandArgument='<%# Eval("ItemID") %>' data-record-select="details">
                                    View
                                </asp:LinkButton>

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>

        </section>


        <div class="manager-inventory-workspace">


            <section class="dashboard-panel manager-inventory-details-panel"  data-tab-pane="details" data-ready-id="hfSelectedItemID" data-ready-kind="positive">

                <div class="dashboard-panel-header">

                    <div>

                        <h2 class="dashboard-panel-title">
                            Selected Item
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            View the selected material before making changes.
                        </p>

                    </div>

                </div>


                <div class="dashboard-panel-body">

                    <asp:HiddenField
                        ID="hfSelectedItemID"
                        runat="server" />


                    <div class="manager-inventory-details-grid">

                        <div class="dashboard-form-group">

                            <label>
                                Item ID
                            </label>

                            <asp:TextBox
                                ID="txtSelectedItemID"
                                runat="server"
                                CssClass="dashboard-input"
                                ReadOnly="true" />

                        </div>


                        <div class="dashboard-form-group">

                            <label>
                                Item Name
                            </label>

                            <asp:TextBox
                                ID="txtSelectedItemName"
                                runat="server"
                                CssClass="dashboard-input"
                                ReadOnly="true" />

                        </div>


                        <div class="dashboard-form-group">

                            <label>
                                Category
                            </label>

                            <asp:TextBox
                                ID="txtSelectedCategory"
                                runat="server"
                                CssClass="dashboard-input"
                                ReadOnly="true" />

                        </div>


                        <div class="dashboard-form-group">

                            <label>
                                Current Quantity
                            </label>

                            <asp:TextBox
                                ID="txtSelectedQuantity"
                                runat="server"
                                CssClass="dashboard-input"
                                ReadOnly="true" />

                        </div>


                        <div class="dashboard-form-group">

                            <label>
                                Unit
                            </label>

                            <asp:TextBox
                                ID="txtSelectedUnitOfMeasure"
                                runat="server"
                                CssClass="dashboard-input"
                                ReadOnly="true" />

                        </div>


                        <div class="dashboard-form-group">

                            <label>
                                Unit Price
                            </label>

                            <asp:TextBox
                                ID="txtSelectedUnitPrice"
                                runat="server"
                                CssClass="dashboard-input"
                                ReadOnly="true" />

                        </div>


                        <div class="dashboard-form-group">

                            <label>
                                Reorder Level
                            </label>

                            <asp:TextBox
                                ID="txtSelectedReorderLevel"
                                runat="server"
                                CssClass="dashboard-input"
                                ReadOnly="true" />

                        </div>


                        <div class="dashboard-form-group">

                            <label>
                                Supplier
                            </label>

                            <asp:TextBox
                                ID="txtSelectedSupplier"
                                runat="server"
                                CssClass="dashboard-input"
                                ReadOnly="true" />

                        </div>


                        <div class="dashboard-form-group">

                            <label>
                                Last Updated
                            </label>

                            <asp:TextBox
                                ID="txtSelectedUpdatedAt"
                                runat="server"
                                CssClass="dashboard-input"
                                ReadOnly="true" />

                        </div>

                    </div>


                    <div class="manager-inventory-danger-action">

                        <asp:Button
                            ID="btnRemoveItem"
                            runat="server"
                            Text="Remove Item"
                            CssClass="dashboard-btn dashboard-btn-danger"
                            OnClick="btnRemoveItem_Click"
                            OnClientClick="return confirm('Are you sure you want to permanently delete this inventory item? This action cannot be undone.');" />

                    </div>

                </div>

            </section>


            <section class="dashboard-panel manager-inventory-stock-panel"  data-tab-pane="details" data-ready-id="hfSelectedItemID" data-ready-kind="positive">

                <div class="dashboard-panel-header">

                    <div>

                        <h2 class="dashboard-panel-title">
                            Update Material Quantity
                        </h2>

                        <p class="dashboard-panel-subtitle">
                            Restock materials or record wasted stock.
                        </p>

                    </div>

                </div>


                <div class="dashboard-panel-body">

                    <div class="manager-stock-operation">

                        <div class="dashboard-form-group">

                            <label>
                                Operation
                            </label>

                            <asp:RadioButtonList
                                ID="rblStockAction"
                                runat="server"
                                CssClass="manager-stock-options"
                                RepeatDirection="Horizontal"
                                RepeatLayout="Flow">

                                <asp:ListItem
                                    Text="Restock"
                                    Value="Restock"
                                    Selected="True" />

                                <asp:ListItem
                                    Text="Waste"
                                    Value="Waste" />

                            </asp:RadioButtonList>

                            <span
                                id="stockOperationError"
                                class="inventory-client-error"
                                aria-live="polite"
                                hidden>
                            </span>

                        </div>


                        <div class="dashboard-form-group">

                            <label for="<%= txtStockQuantity.ClientID %>">
                                Quantity
                            </label>

                            <asp:TextBox
                                ID="txtStockQuantity"
                                runat="server"
                                CssClass="dashboard-input"
                                TextMode="Number"
                                min="1"
                                placeholder="Restock up to 20,000 units" />

                        </div>


                        <div class="manager-stock-update-button">

                            <asp:Button
                                ID="btnUpdateQuantity"
                                runat="server"
                                Text="Update Quantity"
                                CssClass="dashboard-btn dashboard-btn-primary"
                                OnClick="btnUpdateQuantity_Click"
                                OnClientClick="return validateStockAdjustment();" />

                        </div>

                    </div>

                </div>

            </section>

        </div>


        <section class="dashboard-panel manager-inventory-add"  data-tab-pane="add">

            <div class="dashboard-panel-header">

                <div>

                    <h2 class="dashboard-panel-title">
                        Add New Item
                    </h2>

                    <p class="dashboard-panel-subtitle">
                        Add a new material to the inventory.
                    </p>

                </div>

            </div>


            <div class="dashboard-panel-body">

                <div class="manager-inventory-add-grid">

                    <div class="dashboard-form-group">

                        <label for="<%= txtNewItemName.ClientID %>">
                            Item Name
                        </label>

                        <asp:TextBox
                            ID="txtNewItemName"
                            runat="server"
                            CssClass="dashboard-input"
                            placeholder="e.g. Copper Cable" />

                    </div>


                    <div class="dashboard-form-group">

                        <label for="<%= txtNewCategory.ClientID %>">
                            Category
                        </label>

                        <asp:TextBox
                            ID="txtNewCategory"
                            runat="server"
                            CssClass="dashboard-input"
                            placeholder="e.g. Electrical" />

                    </div>


                    <div class="dashboard-form-group">

                        <label for="<%= txtNewQuantity.ClientID %>">
                            Initial Quantity
                        </label>

                        <asp:TextBox
                            ID="txtNewQuantity"
                            runat="server"
                            CssClass="dashboard-input"
                            TextMode="Number"
                            min="0"
                            placeholder="0" />

                    </div>


                    <div class="dashboard-form-group">

                        <label for="<%= ddlNewUnitOfMeasure.ClientID %>">
                            Unit
                        </label>

                        <asp:DropDownList
                            ID="ddlNewUnitOfMeasure"
                            runat="server"
                            CssClass="dashboard-input">

                            <asp:ListItem
                                Text="Each"
                                Value="Each" />

                            <asp:ListItem
                                Text="Metre"
                                Value="Metre" />

                            <asp:ListItem
                                Text="Length"
                                Value="Length" />

                            <asp:ListItem
                                Text="Roll"
                                Value="Roll" />

                        </asp:DropDownList>

                    </div>


                    <div class="dashboard-form-group">

                        <label for="<%= txtNewUnitPrice.ClientID %>">
                            Unit Price
                        </label>

                        <asp:TextBox
                            ID="txtNewUnitPrice"
                            runat="server"
                            CssClass="dashboard-input"
                            TextMode="Number"
                            min="0"
                            step="0.01"
                            placeholder="0.00" />

                    </div>


                    <div class="dashboard-form-group">

                        <label for="<%= txtNewReorderLevel.ClientID %>">
                            Reorder Level
                        </label>

                        <asp:TextBox
                            ID="txtNewReorderLevel"
                            runat="server"
                            CssClass="dashboard-input"
                            TextMode="Number"
                            min="0"
                            placeholder="5" />

                    </div>


                    <div class="dashboard-form-group">

                        <label for="<%= txtNewSupplier.ClientID %>">
                            Supplier Name
                        </label>

                        <asp:TextBox
                            ID="txtNewSupplier"
                            runat="server"
                            CssClass="dashboard-input"
                            placeholder="Supplier name" />

                    </div>

                </div>


                <div class="manager-inventory-add-actions">

                    <asp:Button
                        ID="btnAddItem"
                        runat="server"
                        Text="Add New Item"
                        CssClass="dashboard-btn dashboard-btn-primary"
                        OnClick="btnAddItem_Click"
                        OnClientClick="return validateNewInventoryItem();" />

                </div>

            </div>

        </section>


        

    </div>


    <!-- INVENTORY REAL-TIME VALIDATION -->
    <script type="text/javascript">
        (function () {
            "use strict";

            var ids = {
                stockQuantity: "<%= txtStockQuantity.ClientID %>",
                stockAction: "<%= rblStockAction.UniqueID %>",

                newItemName: "<%= txtNewItemName.ClientID %>",
                newCategory: "<%= txtNewCategory.ClientID %>",
                newQuantity: "<%= txtNewQuantity.ClientID %>",
                newUnitPrice: "<%= txtNewUnitPrice.ClientID %>",
                newReorderLevel: "<%= txtNewReorderLevel.ClientID %>",
                newSupplier: "<%= txtNewSupplier.ClientID %>",
                newUnitOfMeasure: "<%= ddlNewUnitOfMeasure.ClientID %>"
            };

            function getField(id) {
                return document.getElementById(id);
            }

            function getErrorElement(input) {
                if (!input) {
                    return null;
                }

                var errorId = input.id + "_clientError";
                var error = document.getElementById(errorId);

                if (error) {
                    return error;
                }

                error = document.createElement("span");
                error.id = errorId;
                error.className = "inventory-client-error";
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
                    input.classList.add("inventory-client-invalid");
                    input.classList.remove("inventory-client-valid");
                    input.setAttribute("aria-invalid", "true");

                    error.textContent = message;
                    error.hidden = false;

                    return false;
                }

                input.classList.remove("inventory-client-invalid");
                input.setAttribute("aria-invalid", "false");

                error.textContent = "";
                error.hidden = true;

                if (input.value.trim() !== "") {
                    input.classList.add("inventory-client-valid");
                } else {
                    input.classList.remove("inventory-client-valid");
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

            function validateWholeNumber(input, label, allowZero) {
                if (!input) {
                    return true;
                }

                var value = input.value.trim();

                if (value === "") {
                    return setError(input, label + " is required.");
                }

                if (!/^\d+$/.test(value)) {
                    return setError(input, "Enter a whole-number " + label.toLowerCase() + ".");
                }

                var number = Number(value);

                if (!isFinite(number) ||
                    number > 2147483647 ||
                    (allowZero ? number < 0 : number <= 0)) {

                    if (allowZero) {
                        return setError(input, label + " must be zero or greater.");
                    }

                    return setError(input, label + " must be greater than zero.");
                }

                return setError(input, "");
            }

            function validateUnitPrice() {
                var input = getField(ids.newUnitPrice);

                if (!input) {
                    return true;
                }

                var value = input.value.trim();

                if (value === "") {
                    return setError(input, "Unit price is required.");
                }

                if (!/^\d+(?:\.\d+)?$/.test(value)) {
                    return setError(input, "Enter a valid unit price.");
                }

                var price = Number(value);

                if (!isFinite(price) || price <= 0) {
                    return setError(input, "Unit price must be greater than zero.");
                }

                return setError(input, "");
            }

            function validateStockAdjustment() {
                var valid = true;

                var quantityInput = getField(ids.stockQuantity);

                if (!validateWholeNumber(
                    quantityInput,
                    "Quantity",
                    false
                )) {
                    valid = false;
                }

                if (!valid) {
                    focusFirstInvalid([ids.stockQuantity]);
                }

                return valid;
            }

            function validateNewInventoryItem() {
                var valid = true;

                var nameInput = getField(ids.newItemName);
                var categoryInput = getField(ids.newCategory);
                var quantityInput = getField(ids.newQuantity);
                var priceInput = getField(ids.newUnitPrice);
                var reorderInput = getField(ids.newReorderLevel);
                var unitInput = getField(ids.newUnitOfMeasure);

                if (!validateRequired(nameInput, "Item name")) {
                    valid = false;
                }

                if (!validateRequired(categoryInput, "Category")) {
                    valid = false;
                }

                if (!validateWholeNumber(
                    quantityInput,
                    "Initial quantity",
                    true
                )) {
                    valid = false;
                }

                if (!validateUnitPrice()) {
                    valid = false;
                }

                if (!validateWholeNumber(
                    reorderInput,
                    "Reorder level",
                    true
                )) {
                    valid = false;
                }

                if (unitInput && !unitInput.value.trim()) {
                    setError(unitInput, "Select a unit of measure.");
                    valid = false;
                } else if (unitInput) {
                    setError(unitInput, "");
                }

                if (!valid) {
                    focusFirstInvalid([
                        ids.newItemName,
                        ids.newCategory,
                        ids.newQuantity,
                        ids.newUnitPrice,
                        ids.newReorderLevel,
                        ids.newUnitOfMeasure
                    ]);
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

            function attachValidation(input, validator) {
                if (!input ||
                    input.getAttribute("data-inventory-validation-attached") === "true") {
                    return;
                }

                input.setAttribute(
                    "data-inventory-validation-attached",
                    "true"
                );

                input.addEventListener("blur", function () {
                    validator();
                });

                input.addEventListener("input", function () {
                    if (input.getAttribute("aria-invalid") === "true" ||
                        input.classList.contains("inventory-client-valid")) {
                        validator();
                    }
                });

                if (input.tagName === "SELECT") {
                    input.addEventListener("change", validator);
                }
            }

            function initializeValidation() {
                var stockGrid = document.getElementById("<%= gvInventory.ClientID %>");
                if (stockGrid) stockGrid.ondblclick = function (event) {
                    var row = event.target.closest("tr");
                    var select = row && row.querySelector("[data-record-select]");
                    if (select && !event.target.closest("a,button,input")) select.click();
                };
                attachValidation(
                    getField(ids.stockQuantity),
                    function () {
                        validateWholeNumber(
                            getField(ids.stockQuantity),
                            "Quantity",
                            false
                        );
                    }
                );

                attachValidation(
                    getField(ids.newItemName),
                    function () {
                        validateRequired(
                            getField(ids.newItemName),
                            "Item name"
                        );
                    }
                );

                attachValidation(
                    getField(ids.newCategory),
                    function () {
                        validateRequired(
                            getField(ids.newCategory),
                            "Category"
                        );
                    }
                );

                attachValidation(
                    getField(ids.newQuantity),
                    function () {
                        validateWholeNumber(
                            getField(ids.newQuantity),
                            "Initial quantity",
                            true
                        );
                    }
                );

                attachValidation(
                    getField(ids.newUnitPrice),
                    validateUnitPrice
                );

                attachValidation(
                    getField(ids.newReorderLevel),
                    function () {
                        validateWholeNumber(
                            getField(ids.newReorderLevel),
                            "Reorder level",
                            true
                        );
                    }
                );

                attachValidation(
                    getField(ids.newSupplier),
                    function () {
                        /*
                         * Supplier is optional in the supplied
                         * code-behind, so no required rule is imposed.
                         * Clear an existing error if the field is edited.
                         */
                        var input = getField(ids.newSupplier);

                        if (input &&
                            input.getAttribute("aria-invalid") === "true") {
                            setError(input, "");
                        }
                    }
                );

                attachValidation(
                    getField(ids.newUnitOfMeasure),
                    function () {
                        var input = getField(ids.newUnitOfMeasure);

                        if (!input) {
                            return;
                        }

                        if (!input.value.trim()) {
                            setError(input, "Select a unit of measure.");
                        } else {
                            setError(input, "");
                        }
                    }
                );

                /*
                 * The stock operation is an ASP.NET RadioButtonList.
                 * Check that one of the expected values is selected
                 * before applying a stock adjustment.
                 */
                var operationInputs = document.querySelectorAll(
                    "input[name='" + ids.stockAction + "']"
                );

                for (var i = 0; i < operationInputs.length; i++) {
                    operationInputs[i].addEventListener("change", function () {
                        var error = document.getElementById("stockOperationError");

                        if (error) {
                            error.textContent = "";
                            error.hidden = true;
                        }
                    });
                }
            }

            /*
             * Public functions referenced by OnClientClick.
             */
            window.validateStockAdjustment = function () {
                var valid = validateStockAdjustment();

                var selectedOperation = document.querySelector(
                    "input[name='" + ids.stockAction + "']:checked"
                );

                if (selectedOperation && selectedOperation.value === "Restock" &&
                    Number(getField(ids.stockQuantity).value) > <%= MajorProject.Business.Services.InventoryService.MaximumRestockQuantity %>) {
                    setError(getField(ids.stockQuantity), "Enter a restock quantity from 1 to 20,000.");
                    valid = false;
                }
                var error = document.getElementById("stockOperationError");

                if (!selectedOperation ||
                    (selectedOperation.value !== "Restock" &&
                     selectedOperation.value !== "Waste")) {

                    if (error) {
                        error.textContent = "Select either Restock or Waste.";
                        error.hidden = false;
                    }

                    valid = false;
                } else if (error) {
                    error.textContent = "";
                    error.hidden = true;
                }

                return valid;
            };

            window.validateNewInventoryItem = validateNewInventoryItem;

            /*
             * Initialise on first load.
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
             * UpdatePanel support: reattach validation after
             * asynchronous postbacks.
             */
            if (window.Sys &&
                Sys.WebForms &&
                Sys.WebForms.PageRequestManager) {

                Sys.WebForms.PageRequestManager
                    .getInstance()
                    .add_endRequest(function () {
                        initializeValidation();
                    });
            }

        })();
    </script>


</div>

</div>
</asp:Content>
