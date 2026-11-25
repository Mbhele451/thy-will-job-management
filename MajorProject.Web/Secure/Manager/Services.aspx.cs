using System;
using System.Configuration;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using MajorProject.Business.Services;
using MajorProject.Web.Helpers;

namespace MajorProject.Web.Secure.Manager
{
    public partial class Services : Page
    {
        private JobTypeService _jobTypeService;
        protected void Page_Init(object sender, EventArgs e)
        {
            AccessControl.RequireRole("Manager");
            string connectionString = ConfigurationManager.ConnectionStrings["GroupWst34ConnectionString"].ConnectionString;
            _jobTypeService = new JobTypeService(connectionString);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadServices();
                ShowNoServiceSelected();
            }
        }

        private void LoadServices()
        {
            try
            {
                DataTable services = _jobTypeService.GetAllJobTypes();
                gvServices.DataSource = services;
                gvServices.DataBind();
            }
            catch (Exception)
            {
                ShowMessage("Unable to load services.", true);
            }
        }

        protected void gvServices_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (gvServices.SelectedDataKey == null)
            {
                ShowMessage("Please select a valid service.", true);
                return;
            }

            int jobTypeID = Convert.ToInt32(gvServices.SelectedDataKey.Value);
            LoadSelectedService(jobTypeID);
        }

        private void LoadSelectedService(int jobTypeID)
        {
            try
            {
                DataTable service = _jobTypeService.GetJobTypeByID(jobTypeID);
                if (service == null || service.Rows.Count == 0)
                {
                    ShowMessage("The selected service could not be found.", true);
                    ShowNoServiceSelected();
                    return;
                }

                DataRow row = service.Rows[0];
                hfJobTypeID.Value = jobTypeID.ToString();
                txtServiceName.Text = Convert.ToString(row["JobTypeName"]);
                txtLabourCost.Text = Convert.ToDecimal(row["BaseLabourCost"]).ToString("0.00");
                txtTransportCost.Text = Convert.ToDecimal(row["BaseTransportCost"]).ToString("0.00");
                txtTaxPercentage.Text = Convert.ToDecimal(row["TaxPercentage"]).ToString("0.00");
                pnlNoService.Visible = false;
                pnlServiceEditor.Visible = true;
                LoadMaterials(jobTypeID);
                LoadCostSummary(jobTypeID);
                LoadInventoryItems(jobTypeID);
            }
            catch (Exception)
            {
                ShowMessage("Unable to load the selected service.", true);
                ShowNoServiceSelected();
            }
        }

        private void LoadMaterials(int jobTypeID)
        {
            DataTable materials = _jobTypeService.GetMaterialsByJobType(jobTypeID);
            gvMaterials.DataSource = materials;
            gvMaterials.DataBind();
        }

        private void LoadCostSummary(int jobTypeID)
        {
            DataTable costs = _jobTypeService.GetJobCostsAndMaterials(jobTypeID);
            if (costs == null || costs.Rows.Count == 0)
            {
                lblLabourCost.Text = "0.00";
                lblMaterialCost.Text = "0.00";
                lblTransportCost.Text = "0.00";
                return;
            }

            DataRow row = costs.Rows[0];
            decimal labourCost = Convert.ToDecimal(row["BaseLabourCost"]);
            decimal materialCost = Convert.ToDecimal(row["MaterialCost"]);
            decimal transportCost = Convert.ToDecimal(row["BaseTransportCost"]);
            lblLabourCost.Text = labourCost.ToString("N2");
            lblMaterialCost.Text = materialCost.ToString("N2");
            lblTransportCost.Text = transportCost.ToString("N2");
        }

        private void LoadInventoryItems(int jobTypeID)
        {
            DataTable inventoryItems = _jobTypeService.GetAvailableInventoryItems();
            DataTable currentMaterials = _jobTypeService.GetMaterialsByJobType(jobTypeID);
            ddlInventoryItems.Items.Clear();
            ddlInventoryItems.Items.Add(new ListItem("Select an inventory item", ""));
            foreach (DataRow inventoryRow in inventoryItems.Rows)
            {
                int itemID = Convert.ToInt32(inventoryRow["ItemID"]);
                bool alreadyAssigned = false;
                foreach (DataRow materialRow in currentMaterials.Rows)
                {
                    int currentItemID = Convert.ToInt32(materialRow["ItemID"]);
                    if (itemID == currentItemID)
                    {
                        alreadyAssigned = true;
                        break;
                    }
                }

                if (alreadyAssigned)
                {
                    continue;
                }

                string itemName = Convert.ToString(inventoryRow["ItemName"]);
                string unitOfMeasure = Convert.ToString(inventoryRow["UnitOfMeasure"]);
                string displayName = itemName;
                if (!string.IsNullOrWhiteSpace(unitOfMeasure))
                {
                    displayName += " (" + unitOfMeasure + ")";
                }

                ddlInventoryItems.Items.Add(new ListItem(displayName, itemID.ToString()));
            }
        }

        protected void btnSaveService_Click(object sender, EventArgs e)
        {
            try
            {
                int jobTypeID;
                if (!int.TryParse(hfJobTypeID.Value, out jobTypeID) || jobTypeID <= 0)
                {
                    ShowMessage("Please select a service first.", true);
                    return;
                }

                string serviceName = txtServiceName.Text.Trim();
                decimal labourCost;
                decimal transportCost;
                decimal taxPercentage;
                if (!decimal.TryParse(txtLabourCost.Text.Trim(), out labourCost))
                {
                    ShowMessage("Please enter a valid labour cost.", true);
                    return;
                }

                if (!decimal.TryParse(txtTransportCost.Text.Trim(), out transportCost))
                {
                    ShowMessage("Please enter a valid transport cost.", true);
                    return;
                }

                if (!decimal.TryParse(txtTaxPercentage.Text.Trim(), out taxPercentage))
                {
                    ShowMessage("Please enter a valid VAT percentage.", true);
                    return;
                }

                _jobTypeService.UpdateJobType(jobTypeID, serviceName, labourCost, transportCost, taxPercentage);
                LoadServices();
                LoadSelectedService(jobTypeID);
                ShowMessage("Service details were updated successfully.", false);
            }
            catch (ArgumentException ex)
            {
                ShowMessage(ex.Message, true);
            }
            catch (Exception)
            {
                ShowMessage("Unable to update the service.", true);
            }
        }

        protected void gvMaterials_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int jobTypeMaterialID;
                if (!int.TryParse(Convert.ToString(e.CommandArgument), out jobTypeMaterialID) || jobTypeMaterialID <= 0)
                {
                    ShowMessage("Invalid service material.", true);
                    return;
                }

                int jobTypeID;
                if (!int.TryParse(hfJobTypeID.Value, out jobTypeID) || jobTypeID <= 0)
                {
                    ShowMessage("Please select a service first.", true);
                    return;
                }

                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                if (e.CommandName == "UpdateQuantity")
                {
                    TextBox txtQuantity = row.FindControl("txtQuantity") as TextBox;
                    if (txtQuantity == null)
                    {
                        ShowMessage("Unable to read the material quantity.", true);
                        return;
                    }

                    int quantityRequired;
                    if (!int.TryParse(txtQuantity.Text.Trim(), out quantityRequired))
                    {
                        ShowMessage("Please enter a valid material quantity.", true);
                        return;
                    }

                    _jobTypeService.UpdateMaterialQuantity(jobTypeMaterialID, quantityRequired);
                    ShowMessage("Material quantity was updated successfully.", false);
                }
                else if (e.CommandName == "RemoveMaterial")
                {
                    _jobTypeService.RemoveMaterialFromJobType(jobTypeMaterialID);
                    ShowMessage("Material was removed from the service.", false);
                }

                LoadSelectedService(jobTypeID);
            }
            catch (ArgumentException ex)
            {
                ShowMessage(ex.Message, true);
            }
            catch (Exception)
            {
                ShowMessage("Unable to update the service material.", true);
            }
        }

        protected void btnAddMaterial_Click(object sender, EventArgs e)
        {
            try
            {
                int jobTypeID;
                if (!int.TryParse(hfJobTypeID.Value, out jobTypeID) || jobTypeID <= 0)
                {
                    ShowMessage("Please select a service first.", true);
                    return;
                }

                int itemID;
                if (!int.TryParse(ddlInventoryItems.SelectedValue, out itemID) || itemID <= 0)
                {
                    ShowMessage("Please select an inventory item.", true);
                    return;
                }

                int quantityRequired;
                if (!int.TryParse(txtMaterialQuantity.Text.Trim(), out quantityRequired))
                {
                    ShowMessage("Please enter a valid material quantity.", true);
                    return;
                }

                _jobTypeService.AddMaterialToJobType(jobTypeID, itemID, quantityRequired);
                txtMaterialQuantity.Text = "1";
                LoadSelectedService(jobTypeID);
                ShowMessage("Material was added to the service successfully.", false);
            }
            catch (ArgumentException ex)
            {
                ShowMessage(ex.Message, true);
            }
            catch (Exception)
            {
                ShowMessage("Unable to add the material to the service.", true);
            }
        }

        private void ShowNoServiceSelected()
        {
            pnlServiceEditor.Visible = false;
            pnlNoService.Visible = true;
            hfJobTypeID.Value = string.Empty;
            txtServiceName.Text = string.Empty;
            txtLabourCost.Text = string.Empty;
            txtTransportCost.Text = string.Empty;
            txtTaxPercentage.Text = string.Empty;
            lblLabourCost.Text = "0.00";
            lblMaterialCost.Text = "0.00";
            lblTransportCost.Text = "0.00";
            gvMaterials.DataSource = null;
            gvMaterials.DataBind();
            ddlInventoryItems.Items.Clear();
            ddlInventoryItems.Items.Add(new ListItem("Select an inventory item", ""));
            txtMaterialQuantity.Text = "1";
        }

        private void ShowMessage(string message, bool isError)
        {
            lblMessage.Text = message;
            if (isError)
            {
                lblMessage.CssClass = "alert alert-danger d-block mb-3";
            }
            else
            {
                lblMessage.CssClass = "alert alert-success d-block mb-3";
            }
        }

        private void ClearMessage()
        {
            lblMessage.Text = string.Empty;
            lblMessage.CssClass = "d-block mb-3";
        }
    }
}
