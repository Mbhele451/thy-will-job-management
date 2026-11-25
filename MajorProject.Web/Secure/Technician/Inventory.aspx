<%@ Page
Title="Inventory"
Language="C#"
MasterPageFile="~/MasterPages/Dashboard.Master"
AutoEventWireup="true"
CodeBehind="Inventory.aspx.cs"
Inherits="MajorProject.Web.Secure.Technician.Inventory" %>

<asp:Content
ID="MainContent"
ContentPlaceHolderID="MainContent"
runat="server">
<div class="tw-page-stage" data-page-stage>

<section class="dashboard-page-header reveal">

    <div>

        <h1 class="dashboard-page-title">
            Inventory
        </h1>

        <p class="dashboard-page-subtitle">
            View current stock levels and inventory information.
        </p>

    </div>

    <span class="dashboard-status dashboard-status-info">

        <i class="bi bi-eye"></i>

        Read Only

    </span>

</section>


<div class="dashboard-inventory-layout reveal"  data-record-tabs="secure-technician-inventory" data-tab-labels="list=Inventory;details=Item details" data-selected-id="txtStockDetails">

    <section class="dashboard-panel dashboard-inventory-table-panel"  data-tab-pane="list">

        <div class="dashboard-panel-header">

            <div>

                <h2 class="dashboard-panel-title">
                    Inventory Items
                </h2>

                <p class="dashboard-panel-subtitle">
                    Inventory modifications are restricted to Managers.
                </p>

            </div>

            <div class="dashboard-panel-actions">

                <div class="dashboard-search">

                    <i class="bi bi-search"></i>

                    <asp:TextBox
                        ID="txtInventorySearch"
                        runat="server"
                        CssClass="form-control"
                        Placeholder="Search inventory..."
                        AutoPostBack="true"
                        OnTextChanged="txtInventorySearch_TextChanged" />

                </div>

                <div class="dashboard-search">

                    <i class="bi bi-funnel"></i>

                    <asp:DropDownList
                        ID="ddlInventorySort"
                        runat="server"
                        CssClass="form-select"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ddlInventorySort_SelectedIndexChanged">

                        <asp:ListItem
                            Text="Sort by Item Name"
                            Value="ItemName" />

                        <asp:ListItem
                            Text="Quantity: Low to High"
                            Value="QuantityAsc" />

                        <asp:ListItem
                            Text="Quantity: High to Low"
                            Value="QuantityDesc" />

                        <asp:ListItem
                            Text="Category"
                            Value="Category" />

                        <asp:ListItem
                            Text="Unit Price: Low to High"
                            Value="PriceAsc" />

                        <asp:ListItem
                            Text="Unit Price: High to Low"
                            Value="PriceDesc" />

                        <asp:ListItem
                            Text="Recently Updated"
                            Value="UpdatedDesc" />

                    </asp:DropDownList>

                </div>

            </div>

        </div>


        <div class="dashboard-table-wrapper dashboard-inventory-table-wrapper">

            <asp:GridView
                ID="dgvInventory"
                runat="server"
                CssClass="dashboard-table"
                AutoGenerateColumns="False"
                DataKeyNames="ItemID"
                GridLines="None"
                EmptyDataText="No inventory items found."
                AllowPaging="False"
                AutoGenerateSelectButton="False"
                OnSelectedIndexChanged="dgvInventory_SelectedIndexChanged">

                <Columns>

                    <asp:BoundField
                        DataField="ItemID"
                        HeaderText="Item ID" />

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

                    <asp:CommandField
                        ShowSelectButton="true"
                        SelectText="View" />

                </Columns>

            </asp:GridView>

        </div>

    </section>


    <section class="dashboard-panel dashboard-inventory-details-panel"  data-tab-pane="details" data-ready-id="txtStockDetails" data-ready-kind="nonempty">

        <div class="dashboard-panel-header">

            <div>

                <h2 class="dashboard-panel-title">
                    Stock Details
                </h2>

                <p class="dashboard-panel-subtitle">
                    Select an item to view its details.
                </p>

            </div>

        </div>

        <div class="dashboard-inventory-details-body">

            <asp:TextBox
                ID="txtStockDetails"
                runat="server"
                TextMode="MultiLine"
                Rows="12"
                ReadOnly="true"
                CssClass="form-control"
                Placeholder="Select an inventory item from the table to view its details." />

        </div>

    </section>

</div>


</div>
</asp:Content>
