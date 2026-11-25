using iTextSharp.text;
using iTextSharp.text.pdf;
using MajorProject.Business.Helpers;
using MajorProject.Business.Models.DTOs;
using MajorProject.Business.Models.Payments;
using System;
using System.Globalization;
using System.IO;

namespace MajorProject.Business.Services.Payments
{
    public class PaymentReceiptPdfService
    {
        public byte[] GeneratePaymentReceiptPdf(InvoiceDto invoice, PaymentResult payment)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));
            if (payment.PaymentStatus != "Paid" || payment.AttemptStatus != "Successful")
            {
                throw new InvalidOperationException("A receipt can only be generated for a successful payment.");
            }

            if (payment.AmountDue <= 0 || invoice.GrandTotal != payment.AmountDue)
            {
                throw new InvalidOperationException("The payment amount does not match the invoice total.");
            }

            if (string.IsNullOrWhiteSpace(payment.ProviderReference))
            {
                throw new InvalidOperationException("The payment reference is missing.");
            }

            if (!payment.PaidAt.HasValue)
            {
                throw new InvalidOperationException("The payment date is missing.");
            }

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    Document doc = new Document(PageSize.A4, 40, 40, 50, 40);
                    PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    // ====================================================
                    // FONTS
                    // ====================================================
                    Font titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.DARK_GRAY);
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                    Font normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                    Font successFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 13, new BaseColor(30, 130, 70));
                    Font noticeFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, new BaseColor(180, 90, 20));
                    // ====================================================
                    // COMPANY HEADER
                    // ====================================================
                    doc.Add(new Paragraph("THY WILL Electrical and Construction (Pty) Ltd", headerFont));
                    doc.Add(new Paragraph("VAT No: 4123456789 | Reg No: 2020/123456/07", normalFont));
                    doc.Add(new Paragraph("Durban, 4004, KwaZulu-Natal, South Africa", normalFont));
                    doc.Add(new Paragraph("Tel: 083 758 5301 | Email: accounts@thywill.co.za", normalFont));
                    doc.Add(new Paragraph(" "));
                    // ====================================================
                    // RECEIPT TITLE
                    // ====================================================
                    Paragraph title = new Paragraph("PAYMENT RECEIPT", titleFont);
                    title.Alignment = Element.ALIGN_CENTER;
                    doc.Add(title);
                    ;
                    doc.Add(new Paragraph(" "));
                    // ====================================================
                    // RECEIPT INFORMATION
                    // ====================================================
                    PdfPTable receiptTable = new PdfPTable(2);
                    receiptTable.WidthPercentage = 100;
                    receiptTable.SetWidths(new float[] { 1f, 1f });
                    receiptTable.AddCell(NoBorderCell("Receipt No: REC-" + payment.PaymentID.ToString("D8"), boldFont));
                    receiptTable.AddCell(NoBorderCell("Payment Date: " + payment.PaidAt.Value.ToString("dd MMM yyyy HH:mm"), normalFont));
                    receiptTable.AddCell(NoBorderCell("Invoice No: INV-" + invoice.JobId.ToString("D5"), normalFont));
                    receiptTable.AddCell(NoBorderCell("Job ID: " + invoice.JobId, normalFont));
                    receiptTable.AddCell(NoBorderCell("Customer: " + (invoice.CustomerName ?? "Customer"), normalFont));
                    receiptTable.AddCell(NoBorderCell("Email: " + (invoice.CustomerEmail ?? "Not provided"), normalFont));
                    doc.Add(receiptTable);
                    doc.Add(new Paragraph(" "));
                    // ====================================================
                    // PAYMENT DETAILS
                    // ====================================================
                    Paragraph paymentHeading = new Paragraph("PAYMENT DETAILS", headerFont);
                    doc.Add(paymentHeading);
                    PdfPTable paymentTable = new PdfPTable(2);
                    paymentTable.WidthPercentage = 100;
                    paymentTable.SetWidths(new float[] { 1f, 2f });
                    AddDetailRow(paymentTable, "Payment Method", payment.PaymentMethod, normalFont, boldFont);
                    AddDetailRow(paymentTable, "Payment Reference", payment.ProviderReference, normalFont, boldFont);
                    AddDetailRow(paymentTable, "Attempt Number", payment.AttemptNumber.ToString(), normalFont, boldFont);
                    AddDetailRow(paymentTable, "Payment Status", "SUCCESSFUL", normalFont, boldFont);
                    // ====================================================
                    // CASH PAYMENT STAFF ATTRIBUTION
                    // ====================================================
                    if (string.Equals(payment.PaymentMethod, "Cash", StringComparison.OrdinalIgnoreCase))
                    {
                        AddDetailRow(paymentTable, "Received By", payment.StaffFullName ?? "Staff details unavailable", normalFont, boldFont);
                        AddDetailRow(paymentTable, "Staff ID", payment.StaffID.HasValue ? payment.StaffID.Value.ToString() : "Unavailable", normalFont, boldFont);
                    }

                    // ====================================================
                    // CARD DETAILS
                    // ====================================================
                    if (!string.IsNullOrWhiteSpace(payment.CardBrand) && !string.IsNullOrWhiteSpace(payment.LastFourDigits))
                    {
                        AddDetailRow(paymentTable, "Card", payment.CardBrand + " ending in " + payment.LastFourDigits, normalFont, boldFont);
                    }

                    doc.Add(paymentTable);
                    doc.Add(new Paragraph(" "));
                    // ====================================================
                    // FINANCIAL BREAKDOWN
                    // ====================================================
                    Paragraph financialHeading = new Paragraph("PAYMENT BREAKDOWN", headerFont);
                    doc.Add(financialHeading);
                    PdfPTable totalsTable = new PdfPTable(2);
                    totalsTable.WidthPercentage = 55;
                    totalsTable.HorizontalAlignment = Element.ALIGN_RIGHT;
                    totalsTable.SetWidths(new float[] { 1f, 1f });
                    AddTotalsRow(totalsTable, "Labour:", invoice.LabourCost, normalFont);
                    AddTotalsRow(totalsTable, "Transport:", invoice.TransportCost, normalFont);
                    AddTotalsRow(totalsTable, "Materials:", invoice.MaterialCost, normalFont);
                    AddTotalsRow(totalsTable, "Tax:", invoice.TaxAmount, normalFont);
                    AddTotalsRow(totalsTable, "Amount Paid:", payment.AmountDue, boldFont);
                    doc.Add(totalsTable);
                    doc.Add(new Paragraph(" "));
                    // ====================================================
                    // SUCCESS MESSAGE
                    // ====================================================
                    Paragraph success = new Paragraph("PAYMENT SUCCESSFUL", successFont);
                    success.Alignment = Element.ALIGN_CENTER;
                    doc.Add(success);
                    Paragraph footer = new Paragraph("Thank you for choosing THY WILL Electrical " + "and Construction.\n" + "For payment queries, contact accounts@thywill.co.za.\n\n" + "This receipt confirms a simulated payment only. " + "No real money was transferred.", normalFont);
                    footer.Alignment = Element.ALIGN_CENTER;
                    doc.Add(footer);
                    doc.Close();
                    Logger.Info("Payment receipt PDF generated for Payment " + payment.PaymentID);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to generate payment receipt for Payment " + payment.PaymentID, ex);
                throw;
            }
        }

        // ================================================================
        // PRIVATE HELPERS
        // ================================================================
        private static PdfPCell NoBorderCell(string text, Font font)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text ?? string.Empty, font));
            cell.Border = Rectangle.NO_BORDER;
            cell.PaddingBottom = 6;
            return cell;
        }

        private static void AddDetailRow(PdfPTable table, string label, string value, Font normalFont, Font boldFont)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, boldFont));
            labelCell.Padding = 6;
            PdfPCell valueCell = new PdfPCell(new Phrase(value ?? string.Empty, normalFont));
            valueCell.Padding = 6;
            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        private static void AddTotalsRow(PdfPTable table, string label, decimal value, Font font)
        {
            PdfPCell labelCell = new PdfPCell(new Phrase(label, font));
            labelCell.Border = Rectangle.NO_BORDER;
            labelCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            labelCell.Padding = 5;
            PdfPCell valueCell = new PdfPCell(new Phrase(FormatCurrency(value), font));
            valueCell.Border = Rectangle.NO_BORDER;
            valueCell.HorizontalAlignment = Element.ALIGN_RIGHT;
            valueCell.Padding = 5;
            table.AddCell(labelCell);
            table.AddCell(valueCell);
        }

        private static string FormatCurrency(decimal amount)
        {
            return "R " + amount.ToString("N2", CultureInfo.GetCultureInfo("en-ZA"));
        }
    }
}
