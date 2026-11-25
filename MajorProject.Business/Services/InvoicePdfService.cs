using iTextSharp.text;
using iTextSharp.text.pdf;
using MajorProject.Business.Helpers;
using MajorProject.Business.Models.DTOs;
using System;
using System.IO;

namespace MajorProject.Business.Services
{
    public class InvoicePdfService
    {
        internal static byte[] GenerateFeeInvoicePdf(System.Data.DataRow invoice)
        {
            using (var stream = new MemoryStream())
            using (var document = new Document(PageSize.A4, 48, 48, 54, 48))
            {
                PdfWriter.GetInstance(document, stream);
                document.Open();
                document.Add(new Paragraph("THY WILL Electrical and Construction (Pty) Ltd"));
                document.Add(new Paragraph("Booking change invoice", FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20)));
                document.Add(new Paragraph("Invoice FEE-" + invoice["RequestID"] + "   |   Job #" + invoice["JobID"]));
                document.Add(new Paragraph("Customer: " + invoice["CustomerName"]));
                document.Add(new Paragraph("Approved: " + Convert.ToDateTime(invoice["DecidedAtUtc"]).AddHours(2).ToString("dd MMM yyyy")));
                document.Add(new Paragraph(Convert.ToString(invoice["FeeReason"])));
                var table = new PdfPTable(2) { WidthPercentage = 100, SpacingBefore = 20 };
                var normal = FontFactory.GetFont(FontFactory.HELVETICA, 11);
                if (Convert.ToString(invoice["RequestType"]) == "Cancellation")
                {
                    AddTotalsRow(table, "Cancellation charge", Convert.ToDecimal(invoice["CancellationFee"]), normal);
                    AddTotalsRow(table, "Materials consumed", Convert.ToDecimal(invoice["MaterialCharge"]), normal);
                }
                else
                    AddTotalsRow(table, "Late rescheduling", Convert.ToDecimal(invoice["ReschedulingFee"]), normal);
                AddTotalsRow(table, "Invoice total", Convert.ToDecimal(invoice["Fee"]), FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12));
                document.Add(table);
                document.Add(new Paragraph(Convert.ToString(invoice["FeePaymentStatus"]) == "DemoPaid" ? "Settled by demo payment." : "View this invoice in Booking changes & fees to make a demo payment."));
                document.Close();
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Generates a PDF invoice byte array from the given invoice data.
        /// No file I/O and no UI interactions are performed here.
        /// </summary>
        public byte[] GenerateInvoicePdf(InvoiceDto invoice)
        {
            if (invoice == null)
            {
                throw new ArgumentNullException(nameof(invoice));
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Document doc = new Document(PageSize.A4, 40, 40, 50, 40);
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    // ========================================================
                    // FONTS
                    // ========================================================
                    Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                    Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                    Font tableHeaderFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
                    // ========================================================
                    // COMPANY HEADER
                    // ========================================================
                    Paragraph companyName = new Paragraph("THY WILL Electrical and Construction (Pty) Ltd", headerFont);
                    companyName.Alignment = Element.ALIGN_LEFT;
                    doc.Add(companyName);
                    doc.Add(new Paragraph("VAT No: 4123456789 | Reg No: 2020/123456/07", normalFont));
                    doc.Add(new Paragraph("Durban, 4004, KwaZulu-Natal, South Africa", normalFont));
                    doc.Add(new Paragraph("Tel: 083 758 5301 | Email: accounts@thywill.co.za", normalFont));
                    doc.Add(new Paragraph(" "));
                    // ========================================================
                    // INVOICE TITLE
                    // ========================================================
                    Paragraph title = new Paragraph("TAX INVOICE", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    doc.Add(title);
                    doc.Add(new Paragraph(" "));
                    // ========================================================
                    // INVOICE META
                    // ========================================================
                    PdfPTable metaTable = new PdfPTable(2);
                    metaTable.WidthPercentage = 100;
                    metaTable.SetWidths(new float[] { 1f, 1f });
                    metaTable.AddCell(NoBorderCell($"Invoice No: INV-{invoice.JobId:D5}", boldFont));
                    metaTable.AddCell(NoBorderCell($"Date: {invoice.InvoiceDate:dd MMM yyyy}", normalFont));
                    metaTable.AddCell(NoBorderCell($"Job ID: {invoice.JobId}", normalFont));
                    metaTable.AddCell(NoBorderCell($"Customer: {invoice.CustomerName}", normalFont));
                    if (!string.IsNullOrWhiteSpace(invoice.CustomerEmail))
                    {
                        metaTable.AddCell(NoBorderCell($"Email: {invoice.CustomerEmail}", normalFont));
                    }

                    doc.Add(metaTable);
                    doc.Add(new Paragraph(" "));
                    // ========================================================
                    // SERVICE DETAILS
                    //
                    // We intentionally show only service name and quantity.
                    // Historical financial totals come from the committed
                    // Jobs record and are shown in the totals section below.
                    // ========================================================
                    PdfPTable table = new PdfPTable(2);
                    table.WidthPercentage = 100;
                    table.SetWidths(new float[] { 4f, 1f });
                    AddHeaderCell(table, "Service", tableHeaderFont);
                    AddHeaderCell(table, "Qty", tableHeaderFont);
                    if (invoice.Items != null && invoice.Items.Count > 0)
                    {
                        foreach (InvoiceItemDto item in invoice.Items)
                        {
                            table.AddCell(BodyCell(item.ServiceName, normalFont));
                            table.AddCell(BodyCell(item.Quantity.ToString(), normalFont, Element.ALIGN_CENTER));
                        }
                    }
                    else
                    {
                        PdfPCell emptyCell = new PdfPCell(new Phrase("No service details available.", normalFont));
                        emptyCell.Colspan = 2;
                        emptyCell.Padding = 5;
                        table.AddCell(emptyCell);
                    }

                    doc.Add(table);
                    doc.Add(new Paragraph(" "));
                    // ========================================================
                    // TOTALS
                    //
                    // These values come from the committed Jobs record.
                    // They are not recalculated here.
                    // ========================================================
                    PdfPTable totalsTable = new PdfPTable(2);
                    totalsTable.WidthPercentage = 50;
                    totalsTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                    totalsTable.SetWidths(new float[] { 1f, 1f });
                    AddTotalsRow(totalsTable, "Labour:", invoice.LabourCost, normalFont);
                    AddTotalsRow(totalsTable, "Transport:", invoice.TransportCost, normalFont);
                    AddTotalsRow(totalsTable, "Materials:", invoice.MaterialCost, normalFont);
                    AddTotalsRow(totalsTable, "Tax:", invoice.TaxAmount, normalFont);
                    AddTotalsRow(totalsTable, "Grand Total:", invoice.GrandTotal, boldFont);
                    doc.Add(totalsTable);
                    doc.Add(new Paragraph(" "));
                    // ========================================================
                    // PAYMENT STATUS
                    // ========================================================
                    Paragraph status = new Paragraph("Status: Pending Payment", normalFont);
                    status.Alignment = Element.ALIGN_CENTER;
                    doc.Add(status);
                    // ========================================================
                    // FOOTER
                    // ========================================================
                    Paragraph footer = new Paragraph("Thank you for your business. " + "For queries, contact accounts@thywill.co.za", normalFont);
                    footer.Alignment = Element.ALIGN_CENTER;
                    doc.Add(footer);
                    doc.Close();
                    Logger.Info($"PDF generated successfully for Job {invoice.JobId}");
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to generate PDF for Job {invoice.JobId}", ex);
                throw;
            }
        }

        // ================================================================
        // PRIVATE HELPERS
        // ================================================================
        private static PdfPCell NoBorderCell(string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.Border = Rectangle.NO_BORDER;
            cell.PaddingBottom = 4;
            return cell;
        }

        private static void AddHeaderCell(PdfPTable table, string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text, font));
            cell.BackgroundColor = new BaseColor(40, 60, 110);
            cell.Padding = 6;
            table.AddCell(cell);
        }

        private static PdfPCell BodyCell(string text, Font font, int alignment = Element.ALIGN_LEFT)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? string.Empty, font));
            cell.Padding = 5;
            cell.HorizontalAlignment = alignment;
            return cell;
        }

        private static void AddTotalsRow(PdfPTable table, string label, decimal value, Font font)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, font));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            labelCell.PaddingRight = 10;
            PdfPCell valueCell = new PdfPCell(new Phrase("R " + value.ToString("N2"), font));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }
    }
}
