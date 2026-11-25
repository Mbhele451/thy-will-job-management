using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MajorProject.Business.Models
{
    /// <summary>A customer-owned draft. Prices are resolved by the existing job transaction.</summary>
    [Serializable]
    public sealed class BookingCart
    {
        public const int MaxServiceLines = 50;
        public const int MaxDescriptionLength = 230;
        public const int MaxQuantityPerLine = 1000;
        private List<BookingCartLine> _lines = new List<BookingCartLine>();
        public BookingCart(int customerID)
        {
            if (customerID <= 0)
                throw new ArgumentException("A valid customer is required.", nameof(customerID));
            CustomerID = customerID;
        }

        public int CustomerID { get; private set; }

        public IList<BookingCartLine> Lines
        {
            get
            {
                return _lines.AsReadOnly();
            }
        }

        public int ServiceCount
        {
            get
            {
                return _lines.Count;
            }
        }

        public int TotalQuantity
        {
            get
            {
                return _lines.Sum(line => line.Quantity);
            }
        }

        public void AddService(int jobTypeID, string serviceName, int quantity)
        {
            var candidate = new BookingCartLine(jobTypeID, serviceName, quantity);
            BookingCartLine existing = _lines.FirstOrDefault(line => line.JobTypeID == jobTypeID);
            if (existing != null)
            {
                long combined = (long)existing.Quantity + quantity;
                if (combined > MaxQuantityPerLine)
                    throw new ArgumentException("The quantity for one service cannot exceed 1000.");
                existing.SetQuantity((int)combined);
                return;
            }

            if (_lines.Count >= MaxServiceLines)
                throw new ArgumentException("A booking cannot contain more than 50 different services.");
            _lines.Add(candidate);
        }

        /// <summary>Validate the entire selection before changing the saved draft.</summary>
        public void AddSelection(IEnumerable<BookingCartLine> selection)
        {
            if (selection == null)
                throw new ArgumentNullException(nameof(selection));
            var staged = new BookingCart(CustomerID);
            foreach (BookingCartLine line in _lines)
                staged.AddService(line.JobTypeID, line.ServiceName, line.Quantity);
            bool any = false;
            foreach (BookingCartLine line in selection)
            {
                if (line == null)
                    throw new ArgumentException("The service selection is invalid.");
                staged.AddService(line.JobTypeID, line.ServiceName, line.Quantity);
                any = true;
            }

            if (!any)
                throw new ArgumentException("Select at least one service to add.");
            _lines = staged._lines;
        }

        public void SetQuantity(int jobTypeID, int quantity)
        {
            BookingCartLine line = _lines.FirstOrDefault(item => item.JobTypeID == jobTypeID);
            if (line == null)
                throw new ArgumentException("This service is no longer in the cart.");
            line.SetQuantity(quantity);
        }

        public bool RemoveService(int jobTypeID)
        {
            return _lines.RemoveAll(line => line.JobTypeID == jobTypeID) > 0;
        }

        public static string NormalizeDescription(string value)
        {
            string description = (value ?? string.Empty).Trim();
            if (description.Length == 0 || description.Length > MaxDescriptionLength)
                throw new ArgumentException("Describe the work using 1 to 230 characters.");
            return description;
        }

        public void Clear()
        {
            _lines.Clear();
        }

        public DataTable ToDataTable()
        {
            if (_lines.Count == 0)
                throw new ArgumentException("Add at least one service before submitting your booking.");
            var table = new DataTable();
            table.Columns.Add("JobTypeID", typeof(int));
            table.Columns.Add("Quantity", typeof(int));
            foreach (BookingCartLine line in _lines)
                table.Rows.Add(line.JobTypeID, line.Quantity);
            return table;
        }
    }

    [Serializable]
    public sealed class BookingCartLine
    {
        public BookingCartLine(int jobTypeID, string serviceName, int quantity)
        {
            if (jobTypeID <= 0)
                throw new ArgumentException("A valid service is required.", nameof(jobTypeID));
            if (string.IsNullOrWhiteSpace(serviceName) || serviceName.Trim().Length > 100)
                throw new ArgumentException("The service name is invalid.", nameof(serviceName));
            JobTypeID = jobTypeID;
            ServiceName = serviceName.Trim();
            SetQuantity(quantity);
        }

        public int JobTypeID { get; private set; }
        public string ServiceName { get; private set; }
        public int Quantity { get; private set; }

        internal void SetQuantity(int quantity)
        {
            if (quantity < 1 || quantity > BookingCart.MaxQuantityPerLine)
                throw new ArgumentException("Enter a whole-number quantity from 1 to 1000.", nameof(quantity));
            Quantity = quantity;
        }
    }
}
