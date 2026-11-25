using System;
using System.Collections.Generic;

namespace MajorProject.Business.Models.DTOs
{
    public class InvoiceDto
    {
        public int JobId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal LabourCost { get; set; }
        public decimal TransportCost { get; set; }
        public decimal MaterialCost { get; set; }
        // This is the actual tax amount calculated when the job was created.
        public decimal TaxAmount { get; set; }
        // This is the actual grand total saved against the job.
        public decimal GrandTotal { get; set; }
        public decimal Subtotal => LabourCost + TransportCost + MaterialCost;
        public List<InvoiceItemDto> Items { get; set; } = new List<InvoiceItemDto>();
    }
}// This class represents an invoice, including details such as the job ID, customer information, invoice date, costs, tax amount, and grand total. It also includes a list of invoice items represented by the InvoiceItemDto class. The Subtotal property calculates the total cost before tax by summing the labour, transport, and material costs.
// The class is part of the MajorProject.Business.Models.DTOs namespace, indicating that it is a Data Transfer Object (DTO) used for transferring invoice data between different layers of the application.
// The class is designed to be simple and focused on representing the data structure of an invoice, making it suitable for use in various parts of the application, such as generating invoices or displaying invoice details.
// The class is also designed to be easily serializable, which is important for scenarios where invoice data needs to be sent over a network or stored in a database.
// The class does not include any methods for data manipulation or business logic, as it is intended to be a simple data container. Any business logic related to invoices should be handled in the service layer or other appropriate parts of the application.
// The class is also designed to be easily extensible, allowing for additional properties or methods to be added in the future if needed, without affecting existing functionality.
// Overall, the InvoiceDto class is a key part of the invoice management functionality in the MajorProject application, providing a clear and concise representation of an invoice and its associated items.
//determine what information the PDF actually needs
//potentially add tax/service-cost fields where appropriate
