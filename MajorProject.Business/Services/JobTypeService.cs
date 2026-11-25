using System;
using System.Data;
using MajorProject.Data.Data;

namespace MajorProject.Business.Services
{
    public class JobTypeService
    {
        private readonly JobTypeDataAccess _dataAccess;
        public JobTypeService(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be empty.", nameof(connectionString));
            }

            _dataAccess = new JobTypeDataAccess(connectionString);
        }

        /// <summary>
        /// Gets all services.
        /// </summary>
        public DataTable GetAllJobTypes()
        {
            return _dataAccess.GetAllJobTypes();
        }

        /// <summary>
        /// Gets a service together with its current
        /// material cost.
        /// </summary>
        public DataTable GetJobCostsAndMaterials(int jobTypeID)
        {
            ValidateJobTypeID(jobTypeID);
            return _dataAccess.GetJobCostsAndMaterials(jobTypeID);
        }

        /// <summary>
        /// Gets a single service by ID.
        /// </summary>
        public DataTable GetJobTypeByID(int jobTypeID)
        {
            ValidateJobTypeID(jobTypeID);
            return _dataAccess.GetJobTypeByID(jobTypeID);
        }

        /// <summary>
        /// Gets all materials configured for a service.
        /// </summary>
        public DataTable GetMaterialsByJobType(int jobTypeID)
        {
            ValidateJobTypeID(jobTypeID);
            return _dataAccess.GetMaterialsByJobType(jobTypeID);
        }

        /// <summary>
        /// Gets inventory items that can be assigned
        /// to services.
        /// </summary>
        public DataTable GetAvailableInventoryItems()
        {
            return _dataAccess.GetAvailableInventoryItems();
        }

        /// <summary>
        /// Updates an existing service.
        /// </summary>
        public void UpdateJobType(int jobTypeID, string jobTypeName, decimal labourCost, decimal transportCost, decimal taxPercentage)
        {
            ValidateJobTypeID(jobTypeID);
            ValidateJobTypeName(jobTypeName);
            ValidateCosts(labourCost, transportCost, taxPercentage);
            _dataAccess.UpdateJobType(jobTypeID, jobTypeName.Trim(), labourCost, transportCost, taxPercentage);
        }

        /// <summary>
        /// Creates a new service.
        /// </summary>
        public int CreateJobType(string jobTypeName, decimal labourCost, decimal transportCost, decimal taxPercentage)
        {
            ValidateJobTypeName(jobTypeName);
            ValidateCosts(labourCost, transportCost, taxPercentage);
            return _dataAccess.CreateJobType(jobTypeName.Trim(), labourCost, transportCost, taxPercentage);
        }

        /// <summary>
        /// Adds an inventory item to a service.
        /// </summary>
        public void AddMaterialToJobType(int jobTypeID, int itemID, int quantityRequired)
        {
            ValidateJobTypeID(jobTypeID);
            ValidateItemID(itemID);
            ValidateQuantity(quantityRequired);
            _dataAccess.AddMaterialToJobType(jobTypeID, itemID, quantityRequired);
        }

        /// <summary>
        /// Updates the required quantity of a service material.
        /// </summary>
        public void UpdateMaterialQuantity(int jobTypeMaterialID, int quantityRequired)
        {
            if (jobTypeMaterialID <= 0)
            {
                throw new ArgumentException("Invalid service material ID.", nameof(jobTypeMaterialID));
            }

            ValidateQuantity(quantityRequired);
            _dataAccess.UpdateMaterialQuantity(jobTypeMaterialID, quantityRequired);
        }

        /// <summary>
        /// Removes a material from a service.
        /// </summary>
        public void RemoveMaterialFromJobType(int jobTypeMaterialID)
        {
            if (jobTypeMaterialID <= 0)
            {
                throw new ArgumentException("Invalid service material ID.", nameof(jobTypeMaterialID));
            }

            _dataAccess.RemoveMaterialFromJobType(jobTypeMaterialID);
        }

        /// <summary>
        /// Gets jobs together with their service type.
        /// </summary>
        public DataTable GetJobDetailsWithType()
        {
            return _dataAccess.GetJobDetailsWithType();
        }

        private void ValidateJobTypeID(int jobTypeID)
        {
            if (jobTypeID <= 0)
            {
                throw new ArgumentException("Invalid service ID.", nameof(jobTypeID));
            }
        }

        private void ValidateItemID(int itemID)
        {
            if (itemID <= 0)
            {
                throw new ArgumentException("Invalid inventory item ID.", nameof(itemID));
            }
        }

        private void ValidateQuantity(int quantityRequired)
        {
            if (quantityRequired <= 0)
            {
                throw new ArgumentException("Material quantity must be greater than zero.", nameof(quantityRequired));
            }
        }

        private void ValidateJobTypeName(string jobTypeName)
        {
            if (string.IsNullOrWhiteSpace(jobTypeName))
            {
                throw new ArgumentException("Service name cannot be empty.", nameof(jobTypeName));
            }

            if (jobTypeName.Trim().Length > 100)
            {
                throw new ArgumentException("Service name cannot exceed 100 characters.", nameof(jobTypeName));
            }
        }

        private void ValidateCosts(decimal labourCost, decimal transportCost, decimal taxPercentage)
        {
            if (labourCost < 0)
            {
                throw new ArgumentException("Labour cost cannot be negative.", nameof(labourCost));
            }

            if (transportCost < 0)
            {
                throw new ArgumentException("Transport cost cannot be negative.", nameof(transportCost));
            }

            if (taxPercentage < 0 || taxPercentage > 100)
            {
                throw new ArgumentException("Tax percentage must be between 0 and 100.", nameof(taxPercentage));
            }
        }
    }
}
