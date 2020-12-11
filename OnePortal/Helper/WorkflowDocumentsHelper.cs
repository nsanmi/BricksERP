using HRM.DAL.Util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using WorkFlow.DAL;
using WorkFlow.DAL.Data;
using WorkFlow.DAL.Service;

namespace OnePortal.Helper
{
    public class WorkflowDocumentsHelper
    {
        TransactionTokenService _transactionTokenService = new TransactionTokenService();
        /*
         * Document Fonts
         */
        private readonly Font titleFont = FontFactory.GetFont(FontFactory.COURIER, 18, Font.BOLD);
        private readonly Font subTitleFont = FontFactory.GetFont(FontFactory.TIMES, 14, Font.BOLD);
        private readonly Font numberFont = FontFactory.GetFont(FontFactory.SYMBOL, 12, Font.BOLD);
        private readonly Font endingMessageFont = FontFactory.GetFont("Arial", 10, Font.ITALIC);
        private readonly Font bodyFont = FontFactory.GetFont("Arial", 11, Font.NORMAL);
        private readonly Font boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
        private readonly Font MessageFont = FontFactory.GetFont("Arial", 10, Font.NORMAL);

        //create the purchase request form
        public void createPRF(string filepath, string grant, XElement form, string header, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Request form");
            document.AddSubject("Purchase Reqeust form");
            document.AddTitle("Purchase Request form");

            document.Open();

            //add watermark to the document

            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 2f, 5f };
            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(6);
            table.WidthPercentage = 100;
            widths = new float[] { 2f, 2f, 6f, 3f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers
            cell = new PdfPCell(new Phrase(new Chunk("QTY", boldFont)));
            cell.Rowspan = 2;

            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Description", boldFont));
            table.AddCell(cell);

            cell.Rowspan = 1;
            cell.Colspan = 2;
            cell.Phrase = new Phrase(new Chunk("Estimated Cost", boldFont));
            table.AddCell(cell);


            cell.Rowspan = 2;
            cell.Colspan = 1;
            cell.Phrase = new Phrase(new Chunk("Grant/Class", boldFont));
            table.AddCell(cell);

            cell.Rowspan = 1;
            cell.Colspan = 1;
            cell.Phrase = new Phrase(new Chunk("Unit Cost", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total", boldFont));
            //cell.Rowspan = 2;
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            foreach (var element in form.Element("items").Elements("item"))
            {
                total += Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("est_unit_price") != null ? element.Element("est_unit_price").Value : element.Element("unit_cost").Value) * Convert.ToDouble(element.Element("unit").Value);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("itemname").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", element.Element("est_unit_price") != null ? element.Element("est_unit_price").Value : element.Element("unit_cost").Value));
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("est_unit_price") != null ? element.Element("est_unit_price").Value : element.Element("unit_cost").Value) * Convert.ToDouble(element.Element("unit").Value)));
                table.AddCell(cell);

                cell.Phrase = new Phrase(grant);
                table.AddCell(cell);

            }

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL : ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:N}", total));
            table.AddCell(cell);

            cell.Phrase = new Phrase("");
            cell.Rowspan = 3;
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.Rowspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;

            cell.Phrase = new Phrase(new Chunk("EXCHANGE RATE: US$1 = ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(form.Element("conversion_rate").Value);
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL US$ = ", boldFont));
            table.AddCell(cell);

            double total_usd = total / Convert.ToDouble(form.Element("conversion_rate").Value);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(total_usd.ToString());
            table.AddCell(cell);

            document.Add(table);
            document.Add(new Paragraph(""));

            table = new PdfPTable(1);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            cell = new PdfPCell(new Phrase(new Chunk("Relevance to Grant/Class noted above (To be completed by requestor.):", boldFont)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("This expense is in support of the " + grant + " grant(s) for the purpose of : " + form.Element("justification").Value));
            table.AddCell(cell);

            document.Add(table);

            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Requestor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            try
            {
                //get the signature of the procurement lead
                signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "6");
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(signature.Element("name").Value));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Procurement Lead Signature"));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Name and Title"));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date"));
                table.AddCell(cell);
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }

            //get the signature of the program lead
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "3");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Program Director Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            try
            {
                //get the signature of the finance
                signature = form.Element("signatures").Elements("signature")
                    .FirstOrDefault(e => e.Element("step").Value.ToString() == "7");
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                if (token != null)
                {
                    cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                }
                else
                {
                    cell.Phrase = new Phrase(new Chunk("", sign_font));
                }

                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(signature.Element("name").Value));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Finance Signature"));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Name and Title"));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date"));
                table.AddCell(cell);
            }
            catch (Exception e)
            {
                Utils.LogError(e);
            }

            //add the signature table to the document
            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        /*
         * Create by Johnbosco
         * created from the inherited createPRF
         */
        public void NewCreatePRF(string filepath, string grant, XElement form, string header, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Request form");
            document.AddSubject("Purchase Request form");
            document.AddTitle("Purchase Request form");
            document.Open();

            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 5f;

            document.Add(jpg);

            /*
             * Add date and PRF number
             */
            var side = new PdfDiv();
            side.Width = 210;
            side.Float = PdfDiv.FloatType.RIGHT;

            var topParagraph = new Paragraph
            {
                Alignment = Element.ALIGN_LEFT
            };
            var topPhrase = new Phrase();
            topPhrase.Add(new Chunk("Date: ", bodyFont));
            topPhrase.Add(new Chunk(DateTime.Parse(definition.Element("created_at").Value).ToString("d"), boldFont));
            topPhrase.Add(new Chunk("       ", bodyFont));
            topPhrase.Add(new Chunk("Number: ", bodyFont));
            topPhrase.Add(new Chunk(definition.Element("process_number").Value, subTitleFont));
            topParagraph.Add(topPhrase);
            side.Content.Add(topParagraph);

            document.Add(side);

            document.Add(new LineSeparator());
            /*
             * Add the PO details
             */
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;

            var cell = new PdfPCell(PrintTextElement(bodyFont, boldFont, "Requesting Office", form.Element("requesting_office").Value, ":"));
            cell.BorderWidth = 0f;
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Deliver To", form.Element("delivery_to").Value, ":"));
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Requestor Name", form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1").Element("name").Value, ":"));
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Address", form.Element("delivery_address").Value, ":"));
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Phone", form.Element("phone").Value, ":"));
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "City/Country", form.Element("country").Value, ":"));
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Need-By Date", form.Element("needed_by").Value, ":"));
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Attention", form.Element("attention").Value, ":"));
            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(6)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };
            table.WidthPercentage = 100;
            var widths = new float[] { 1f, 2.5f, 6f, 2f, 2f, 2.5f };
            table.SetWidths(widths);
            //State Creating table headers
            cell = new PdfPCell(new Phrase(new Chunk("QTY", boldFont)));
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            cell.Rowspan = 2;
            cell.VerticalAlignment = Element.ALIGN_CENTER;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Padding = 5f;

            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Description", boldFont));
            table.AddCell(cell);

            cell.Rowspan = 1;
            cell.Colspan = 2;
            cell.Phrase = new Phrase(new Chunk("Estimated Cost", boldFont));
            table.AddCell(cell);

            cell.Rowspan = 2;
            cell.Colspan = 1;
            cell.Phrase = new Phrase(new Chunk("Grant/Class", boldFont));
            table.AddCell(cell);

            cell.Rowspan = 1;
            cell.Colspan = 1;
            cell.Phrase = new Phrase(new Chunk("Unit Cost", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total", boldFont));
            //cell.Rowspan = 2;
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            cell.Padding = 5f;
            foreach (var element in form.Element("items").Elements("item"))
            {
                total += Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("est_unit_cost").Value);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("description").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", element.Element("est_unit_cost").Value));
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("est_unit_cost").Value)));
                table.AddCell(cell);

                cell.Phrase = new Phrase(grant);
                table.AddCell(cell);

            }

            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL : ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:N}", total));
            table.AddCell(cell);

            cell.Phrase = new Phrase("");
            cell.Rowspan = 3;
            table.AddCell(cell);


            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 4;
            cell.Rowspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;

            cell.Phrase = new Phrase(new Chunk("Exchange rate: US$1 = ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(form.Element("conversion_rate").Value);
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);

            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL = ", boldFont));
            table.AddCell(cell);

            double total_usd = total / Convert.ToDouble(form.Element("conversion_rate").Value);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(total_usd.ToString("C"));
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);

            document.Add(table);
            document.Add(new Paragraph(""));

            table = new PdfPTable(1);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            cell = new PdfPCell(new Phrase(new Chunk("Relevance to Grant/Class noted above (To be completed by requestor)", boldFont)));
            cell.Padding = 5f;
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("This expense is in support of the " + grant + " grant(s) for the purpose of " + form.Element("justification").Value));
            cell.Padding = 5f;
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(form.Element("justification").Value));
            //table.AddCell(cell);

            document.Add(table);

            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            cell = new PdfPCell();
            cell.Colspan = 1;
            cell.Padding = 5f;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("created_at").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Requestor Signature", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name and Title", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Date", bodyFont);
            table.AddCell(cell);

            try
            {
                //get the signature of the program lead
                signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "3");
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("created_at").Value, boldFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Program Director Signature", bodyFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Name and Title", bodyFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Date", bodyFont);
                table.AddCell(cell);

                //get the signature of the procurement lead
                signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "4");
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("created_at").Value, boldFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Procurement Lead Signature", bodyFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Name and Title", bodyFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Date", bodyFont);
                table.AddCell(cell);
            }
            catch (Exception ex)
            {
                Utils.LogError(ex);
            }

            //get the signature of the finance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "5");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("created_at").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("DFA Signature", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name and Title", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Date", bodyFont);
            table.AddCell(cell);

            //get the signature of the country director
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "6");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("created_at").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Country Director", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name and Title", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Date", bodyFont);
            table.AddCell(cell);

            //add the signature table to the document
            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            //write.Flush();
            writer.Close();
            // Always close open filehandles explicity
            //fs.Flush();
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");
            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        //create the purchase request form
        public void createPRFPaymentSchedule(string filepath, string grant, XElement form, string header, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Payment Schedule");
            document.AddSubject("Payment Schedule");
            document.AddTitle("Payment Schedule");


            document.Open();

            //add watermark to the document



            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);


            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 2f, 5f };
            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(6);
            table.WidthPercentage = 100;
            widths = new float[] { 2f, 2f, 6f, 3f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers
            cell = new PdfPCell(new Phrase(new Chunk("QTY", boldFont)));
            //cell.Rowspan = 2;

            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Description", boldFont));
            table.AddCell(cell);

            //cell.Rowspan = 1;
            //cell.Colspan = 2;
            //cell.Phrase = new Phrase(new Chunk("Estimated Cost", boldFont));
            //table.AddCell(cell);


            //cell.Rowspan = 2;
            //cell.Colspan = 1;


            //cell.Rowspan = 1;
            //cell.Colspan = 1;
            cell.Phrase = new Phrase(new Chunk("Unit Cost", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total", boldFont));
            //cell.Rowspan = 2;
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Grant/Class", boldFont));
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            foreach (var element in form.Element("items").Elements("item"))
            {
                total += Convert.ToDouble(element.Element("amount").Value) * Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("unit").Value);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("itemname").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("amount").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", (Convert.ToDouble(element.Element("amount").Value) * Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("unit").Value)).ToString()));
                table.AddCell(cell);

                cell.Phrase = new Phrase(grant);
                table.AddCell(cell);

            }

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL : ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:N}", total));
            table.AddCell(cell);

            cell.Phrase = new Phrase("");
            //cell.Rowspan = 3;
            table.AddCell(cell);


            //cell = new PdfPCell();
            //cell.Colspan = 4;
            //cell.Rowspan = 1;
            //cell.HorizontalAlignment = Element.ALIGN_RIGHT;

            //cell.Phrase = new Phrase(new Chunk("EXCHANGE RATE: US$1 = ", boldFont));
            //table.AddCell(cell);

            //cell.Colspan = 1;
            //cell.Phrase = new Phrase(form.Element("conversion_rate").Value);
            //table.AddCell(cell);

            ////cell.Phrase = new Phrase("");
            ////table.AddCell(cell);


            //cell = new PdfPCell();
            //cell.Colspan = 4;
            //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell.Phrase = new Phrase(new Chunk("TOTAL US$ = ", boldFont));
            //table.AddCell(cell);

            //double total_usd = total / Convert.ToDouble(form.Element("conversion_rate").Value);

            //cell.Colspan = 1;
            //cell.Phrase = new Phrase(total_usd.ToString());
            //table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);

            document.Add(table);
            document.Add(new Paragraph(""));

            table = new PdfPTable(1);
            table.WidthPercentage = 100;
            table.SpacingBefore = 20f;

            cell = new PdfPCell(new Phrase(new Chunk("Relevance to Grant/Class noted above (To be completed by requestor.):", boldFont)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("This expense is in support of the " + grant + " grant(s) for the purpose of : " + form.Element("justification").Value));
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(form.Element("justification").Value));
            //table.AddCell(cell);

            document.Add(table);

            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };


            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Requestor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);


            //get the signature of compliance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "2");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Compliance Review"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);


            //get the signature of the program lead
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "3");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Program Director Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the finance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "4");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Director of Finance and Admin Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);



            //get the signature of the finance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "5");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Country Director's Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //add the signature table to the document
            document.Add(table);


            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        public void createReimbursement(string filepath, string grant, XElement form, string header, admin_hrm_employee employee, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Request form");
            document.AddSubject("Purchase Reqeust form");
            document.AddTitle("Purchase Request form");


            document.Open();

            //add watermark to the document



            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            //PdfPTable table = new PdfPTable(2);
            //table.WidthPercentage = 100;
            //float[] widths = new float[] { 2f, 5f };
            //PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            //table.AddCell(cell);

            /* document.Add(table)*/
            ;

            PdfPTable table = new PdfPTable(6);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 2f, 2f, 6f, 3f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers
            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Staff Name", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname))));
            cell.Colspan = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            cell.Colspan = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            cell.Colspan = 5;
            table.AddCell(cell);



            cell = new PdfPCell(new Phrase(new Chunk("Purpose:", boldFont)));
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(form.Element("justification").Value));
            cell.Colspan = 5;
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase(new Chunk("QTY", boldFont)));
            //cell.Rowspan = 2;
            cell.Colspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Description", boldFont));
            table.AddCell(cell);

            //cell.Rowspan = 1;
            //cell.Colspan = 2;
            //cell.Phrase = new Phrase(new Chunk("Estimated Cost", boldFont));
            //table.AddCell(cell);


            //cell.Rowspan = 2;
            //cell.Colspan = 1;


            //cell.Rowspan = 1;
            //cell.Colspan = 1;
            cell.Phrase = new Phrase(new Chunk("Unit Cost", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total", boldFont));
            //cell.Rowspan = 2;
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Grant/Class", boldFont));
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            foreach (var element in form.Element("items").Elements("item"))
            {
                total += Convert.ToDouble(element.Element("amount").Value) * Convert.ToDouble(element.Element("quantity").Value);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("itemname").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("amount").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase((Convert.ToDouble(element.Element("amount").Value) * Convert.ToDouble(element.Element("quantity").Value)).ToString());
                table.AddCell(cell);

                cell.Phrase = new Phrase(grant);
                table.AddCell(cell);

            }

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL : ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(total.ToString());
            table.AddCell(cell);

            cell.Phrase = new Phrase("");
            //cell.Rowspan = 3;
            table.AddCell(cell);


            //cell = new PdfPCell();
            //cell.Colspan = 4;
            //cell.Rowspan = 1;
            //cell.HorizontalAlignment = Element.ALIGN_RIGHT;

            //cell.Phrase = new Phrase(new Chunk("EXCHANGE RATE: US$1 = ", boldFont));
            //table.AddCell(cell);

            //cell.Colspan = 1;
            //cell.Phrase = new Phrase(form.Element("conversion_rate").Value);
            //table.AddCell(cell);

            ////cell.Phrase = new Phrase("");
            ////table.AddCell(cell);


            //cell = new PdfPCell();
            //cell.Colspan = 4;
            //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            //cell.Phrase = new Phrase(new Chunk("TOTAL US$ = ", boldFont));
            //table.AddCell(cell);

            //double total_usd = total / Convert.ToDouble(form.Element("conversion_rate").Value);

            //cell.Colspan = 1;
            //cell.Phrase = new Phrase(total_usd.ToString());
            //table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);

            document.Add(table);
            //document.Add(new Paragraph(""));

            //table = new PdfPTable(1);
            //table.WidthPercentage = 100;
            //table.SpacingBefore = 20f;


            ////cell = new PdfPCell(new Phrase(form.Element("justification").Value));
            ////table.AddCell(cell);

            //document.Add(table);

            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };


            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Requestor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);


            //get the signature of compliance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "2");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Compliance Review"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);


            //get the signature of the program lead
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "3");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Program Director Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the finance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "4");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Director of Finance and Admin Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);



            //get the signature of the finance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "5");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Country Director's Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //add the signature table to the document
            document.Add(table);


            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        //create the purchase request form
        public void createRFQ(string filepath, string grant, XElement form, string header, string company_name, string address, string emp_name, string emp_designation, string date, string email_info, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Request form");
            document.AddSubject("Purchase Reqeust form");
            document.AddTitle("Purchase Request form");

            document.Open();

            //add watermark to the document

            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            PdfPTable table = new PdfPTable(4);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 3f, 3f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers
            PdfPCell cell = new PdfPCell(new Phrase(new Chunk(company_name)));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk(address, boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("REQUEST FOR QUOTE", boldFont));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 4;
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Phrase = new Phrase(new Chunk("The Maryland Global Initiative Cooperation of Nigeria, a Non–profit, Non-Governmental Organization funded by the US Government through the Centre for Disease Control (CDC). University of Maryland (UMB) Program in Nigeria operationalized through Maryland Global Initiatives Corporation (MGIC) Nigeria, which promotes best practices in health care delivery and research using local and internationally-adapted models to strengthen health systems."));
            table.AddCell(cell);

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.Phrase = new Phrase(new Chunk("UNIT", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("QTY", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("ITEM", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("DESCRIPTION", boldFont));
            table.AddCell(cell);

            foreach (var item in form.Element("items").Elements("item"))
            {
                cell.Phrase = new Phrase(new Chunk(item.Element("unit").Value));
                table.AddCell(cell);

                cell.Phrase = new Phrase(new Chunk(item.Element("quantity").Value));
                table.AddCell(cell);

                cell.Phrase = new Phrase("");
                table.AddCell(cell);

                cell.Phrase = new Phrase(new Chunk(item.Element("description").Value));
                table.AddCell(cell);
            }

            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.Colspan = 4;
            cell.Phrase = new Phrase(new Chunk(" Should your organization be successful in the bid selection process, note that (as a statutory requirement) our organization shall withhold & remit to the tax authority the appropriate tax rate under which your professional service falls and a Performance Bond (PB) will be requested from you if your transaction falls within that threshold. We are VAT exempt as an organization (NGO)."));
            table.AddCell(cell);

            if (email_info != "")
            {
                cell.Phrase = new Phrase(new Chunk(email_info));
                table.AddCell(cell);
            }

            cell.Phrase = new Phrase(new Chunk(string.Format("All quotes must be submitted on or before the close of business on {0} in hard copies and sealed envelope to the Procurement Officer MGIC, Plot 784, by Gilmor Engineering, Off Life Camp, Gwarinpa—Expressway, Jahi District, Abuja or through our email: procurement@mgic-nigeria.org", date)));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk(emp_name, boldFont));
            table.AddCell(cell);

            //cell.Rowspan = 1;
            //cell.Colspan = 1;
            cell.Phrase = new Phrase(new Chunk(emp_designation));
            table.AddCell(cell);

            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        /*
         * Created By Johnbosco
         * created out of the createRFQ
         */
        public void createRFQTest(string filepath, string grant, XElement form, string header, string company_name, bpm_vendor vendor, string emp_name, string emp_designation, string date, string email_info, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Request form");
            document.AddSubject("Purchase Reqeust form");
            document.AddTitle("Purchase Request form");

            document.Open();

            /*
             * Add Water mark to the document
             * Watermark begin
             */
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);
            /*
             * Watermark end...
             */
            /*
             * Create a container for the headers
             * Container begin
             */
            //start of first table
            PdfPTable table = new PdfPTable(4)
            {
                SpacingBefore = 20f,
                SpacingAfter = 20f
            };
            table.WidthPercentage = 100;
            float[] widths = new float[] { 1f, 3f, 1f, 3f };
            table.SetWidths(widths);
            PdfPCell cell = new PdfPCell(new Phrase("FROM", bodyFont));
            cell.Padding = 5f;
            table.AddCell(cell);
            cell.Phrase = new Phrase("MGIC-NIGERIA", boldFont);
            table.AddCell(cell);
            cell = new PdfPCell(new Phrase("TO", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(company_name, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Address", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk(form.Element("delivery_address").Value, boldFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("Address", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk(vendor.address, boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("City", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase("Abuja", boldFont);
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("City", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase("", boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Country", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk(form.Element("country").Value, boldFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("Country", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Phone", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase("", boldFont);
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("Phone", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(vendor.phone, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Email", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase("", boldFont);
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("Email", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(vendor.email, boldFont);
            table.AddCell(cell);
            //add the container to the document
            document.Add(table);
            /*
             * Container End...
             */
            /*
             * Add a paragraph
             * Paragraph begin
             */
            var paragraphDiv = new PdfDiv();
            paragraphDiv.TextAlignment = Element.ALIGN_CENTER;
            paragraphDiv.AddElement(new Phrase("The Maryland Global Initiatives Corporation of Nigeria is a Non –profit, Non- Governmental Organization funded by the US " +
                                               "Government through the Centre for Disease Control (CDC). University of Maryland (UMB) Program in Nigeria is operationalized " +
                                               "through Maryland Global Initiatives Corporation (MGIC) Nigeria, which promotes best practices in health care delivery and " +
                                               "research using local and internationally-adapted models to strengthen health systems", bodyFont));
            document.Add(paragraphDiv);

            paragraphDiv = new PdfDiv();
            paragraphDiv.TextAlignment = Element.ALIGN_CENTER;
            new Phrase(new Chunk("We hereby request you to submit price quotation(s) for the supply of the item(s) listed on the attached Bidding Form", boldFont));
            document.Add(paragraphDiv);

            table = new PdfPTable(4)
            {
                SpacingAfter = 20f,
                SpacingBefore = 20f
            };
            table.WidthPercentage = 100;
            widths = new float[] { 3f, 3f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers
            cell.Phrase = new Phrase("REQUEST FOR QUOTE DETAILS", boldFont);
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 4;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("RFQ#", bodyFont)));
            cell.Padding = 5f;
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk(definition.Element("process_number").Value.Replace("PR", "RFQ"), boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Currency of Bid (3-letter code)", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("NGN", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("RFQ Issuing Date", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk(DateTime.Now.ToString("d"), boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Bid Validity Period (days)", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("RFQ Closing Date", bodyFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk(date, boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("RFQ Issuing Date", bodyFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk(date, boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Required Delivery Date", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("RFQ Closing Time", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("5:00pm", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Required Delivery Destination", bodyFont));
            table.AddCell(cell);
            cell.Phrase = new Phrase(new Chunk("Mgic Store", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Questions to the RFQ", bodyFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("procurement@mgic-nigeria.org", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Required Delivery Terms", bodyFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("", boldFont));
            table.AddCell(cell);

            document.Add(table);

            //add item tables
            table = new PdfPTable(4);
            table.WidthPercentage = 100;
            widths = new float[] { 1f, 2f, 3f, 1f };
            table.SetWidths(widths);

            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 1;
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            cell.Phrase = new Phrase(new Chunk("#", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("UNIT", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("DESCRIPTION", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("QTY", boldFont));
            table.AddCell(cell);
            var count = 0;
            foreach (var item in form.Element("items").Elements("item"))
            {
                count++;
                cell = new PdfPCell();
                cell.Phrase = new Phrase(new Chunk(count.ToString()));
                table.AddCell(cell);

                cell.Phrase = new Phrase(new Chunk(item.Element("unit").Value));
                table.AddCell(cell);

                cell.Phrase = new Phrase(new Chunk(item.Element("description").Value));
                table.AddCell(cell);

                cell.Phrase = new Phrase(new Chunk(item.Element("quantity").Value));
                table.AddCell(cell);
            }

            //cell.HorizontalAlignment = Element.ALIGN_LEFT;
            //cell.Colspan = 4;
            //cell.Phrase = new Phrase(new Chunk(" Should your organization be successful in the bid selection process, note that (as a statutory requirement) our organization shall withhold & remit to the tax authority the appropriate tax rate under which your professional service falls and a Performance Bond (PB) will be requested from you if your transaction falls within that threshold. We are VAT exempt as an organization (NGO)."));
            //table.AddCell(cell);

            //if (email_info != "")
            //{
            //    cell.Phrase = new Phrase(new Chunk(email_info));
            //    table.AddCell(cell);
            //}

            //cell.Phrase = new Phrase(new Chunk(string.Format("All quotes must be submitted on or before the close of business on {0} in hard copies and sealed envelope to the Procurement Officer MGIC, Plot 784, by Gilmor Engineering, Off Life Camp, Gwarinpa—Expressway, Jahi District, Abuja or through our email: procurement@mgic-nigeria.org", date)));
            //table.AddCell(cell);
            document.Add(table);

            //add the instructions page
            document.NewPage();


            PdfDiv titleDiv = new PdfDiv();
            titleDiv.Content.Add(new Chunk("RFQ INSTRUCTIONS", titleFont));
            titleDiv.TextAlignment = Element.ALIGN_CENTER;
            document.Add(titleDiv);

            Paragraph bodyParagraph = new Paragraph();
            bodyParagraph.Add(new Phrase("You must submit one original of the RFQ Bid Form in a sealed envelope, clearly marked with the RFQ " +
                "number and the Bidders name. The bid can be delivered directly to the tender box, mailed or delivered by courier services, " +
                "or alternatively send by email to the following dedicated, secure & controlled email address: procurement@mgic-nigeria.org. " +
                "THE RFQ BID FORM CANNOT BE EMAILED TO ANY OTHER MGIC EMAIL ADDRESS", bodyFont));
            bodyParagraph.SpacingAfter = 20f;
            bodyParagraph.SpacingBefore = 20f;
            document.Add(bodyParagraph);


            bodyParagraph = new Paragraph();
            bodyParagraph.Add(new Phrase("The sealed envelope must be deposited into the MGIC Tender Box at the address stated on page one before " +
                                         "the RFQ Closing Date and Time. It is the Bidders responsibility to ensure that the sealed envelope is " +
                                         "deposited into the Tender Box.", bodyFont));
            document.Add(bodyParagraph);

            table = new PdfPTable(2);
            table.WidthPercentage = 100;
            widths = new float[] { 3f, 3f };
            table.SetWidths(widths);

            cell.Phrase = new Phrase(new Chunk(emp_name, boldFont));
            cell.BorderWidth = 0f;
            table.AddCell(cell);

            //cell.Rowspan = 1;
            //cell.Colspan = 1;
            cell.Phrase = new Phrase(new Chunk(emp_designation));
            table.AddCell(cell);

            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            //write.Flush();
            writer.Close();
            // Always close open filehandles explicity
            //fs.Flush();
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        public void createPurchaseOrders_Old(string filepath, string grant, XElement form, string header, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Order");
            document.AddSubject("Purchase Order");
            document.AddTitle("Purchase Order");
            document.Open();

            //add watermark to the document

            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            /*
             * Edited by Johnbosco
             * Add table with To, Ship To etc
             */
            PdfPTable table = new PdfPTable(3);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 2f, 2f, 2f };
            //add who the po is to


            table = new PdfPTable(2);
            table.WidthPercentage = 100;
            widths = new float[] { 2f, 5f };
            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(6);
            table.WidthPercentage = 100;
            widths = new float[] { 1f, 2f, 3f, 8f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers
            //PdfPCell cell = new PdfPCell(new Phrase(new Chunk("QTY", boldFont)));
            //cell.Rowspan = 2;

            //cell.HorizontalAlignment = Element.ALIGN_CENTER;

            //table.AddCell(cell);

            cell = new PdfPCell();

            cell.Phrase = new Phrase(new Chunk("ITEM No", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Order QTY", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit", boldFont));
            table.AddCell(cell);



            cell.Phrase = new Phrase(new Chunk("Description", boldFont));
            table.AddCell(cell);


            cell.Phrase = new Phrase(new Chunk("Unit Price", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total", boldFont));
            //cell.Rowspan = 2;
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            foreach (var element in form.Element("items").Elements("item"))
            {
                total += Convert.ToDouble(element.Element("unit_price").Value) * Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("unit").Value);

                cell.Phrase = new Phrase("");
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("description").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit_price").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", Convert.ToDouble(element.Element("unit_price").Value) * Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("unit").Value)));
                table.AddCell(cell);

            }

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL : ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:N}", total));
            table.AddCell(cell);

            cell.Phrase = new Phrase("");
            cell.Rowspan = 3;
            table.AddCell(cell);


            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.Rowspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;

            cell.Phrase = new Phrase(new Chunk("EXCHANGE RATE: US$1 = ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(form.Element("conversion_rate").Value);
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL US$ = ", boldFont));
            table.AddCell(cell);

            double total_usd = total / Convert.ToDouble(form.Element("conversion_rate").Value);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(total_usd.ToString());
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);

            document.Add(table);
            document.Add(new Paragraph(""));



            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase("File");
            //cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Requestor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            try
            {
                //get the signature of the procurement lead
                signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "13");
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                //cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                cell.Phrase = new Phrase("File");
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(signature.Element("name").Value));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Program Signature"));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Name and Title"));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date"));
                table.AddCell(cell);
            }
            catch (Exception)
            {

            }


            //get the signature of the program lead
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "14");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            //cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            cell.Phrase = new Phrase("File");
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Finance Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the finance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "15");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            //cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            cell.Phrase = new Phrase("File");
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Director of Finance and Administration Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //add the signature table to the document
            document.Add(table);
            document.Add(new Paragraph(""));

            table = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            cell = new PdfPCell(new Phrase(new Chunk("Bank transfers abd checks must be made out to the vendor or vending company ONLY.", boldFont)));
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(""));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(""));
            table.AddCell(cell);

            document.Add(table);


            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };


            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor

            cell.Phrase = new Phrase("");
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(""));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(""));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Vendor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        /*
         * Edited By Johnbosco
         * created out of the createPurchaseOrders_Old
         */
        public void createPurchaseOrder(string filepath, string grant, XElement form, string header, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Order");
            document.AddSubject("Purchase Order");
            document.AddTitle("Purchase Order");
            document.Open();

            //add watermark to the document

            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            /*
             * Edited by Johnbosco
             * Add table with To, Ship To etc
             */
            PdfPTable table = new PdfPTable(3);
            table.WidthPercentage = 100;
            float[] widths = { 3f, 3f, 4f };
            table.SetWidths(widths);
            //add who the po is to
            PdfPCell cell;
            foreach (var element in definition.Element("vendors").Elements("vendor"))
            {
                if (element.Attribute("selected") != null && int.Parse(element.Attribute("selected").Value) == 1)
                {
                    var vendorName = element.Element("name").Value;
                    cell = new PdfPCell(new Phrase(new Chunk("TO: " + vendorName, boldFont)));
                    table.AddCell(cell);
                }
            }
            cell = new PdfPCell(new Phrase("SHIP TO:"));
            table.AddCell(cell);

            //Create the inner table for requestor anem and other information
            PdfPTable innerTable = new PdfPTable(2);
            innerTable.WidthPercentage = 100;
            float[] innerWidths = { 5f, 5f };
            innerTable.SetWidths(innerWidths);


            PdfPCell innerCell = new PdfPCell(new Phrase(new Chunk("MGIC", boldFont)));
            innerCell.Colspan = 2;
            innerTable.AddCell(innerCell);

            innerCell = new PdfPCell(new Phrase("Purchase Req No:"));
            innerTable.AddCell(innerCell);
            innerCell = new PdfPCell(new Phrase(""));
            innerTable.AddCell(innerCell);
            innerCell = new PdfPCell(new Phrase("Requestor Name:"));
            innerTable.AddCell(innerCell);
            foreach (var element in definition.Element("steps").Elements("step"))
            {
                if (int.Parse(element.Element("code").Value) == 1)
                {
                    innerCell = new PdfPCell(new Phrase(element.Element("created_by_name").Value));
                    innerTable.AddCell(innerCell);
                }
            }
            innerCell = new PdfPCell(new Phrase("Requestor Phone No:"));
            innerTable.AddCell(innerCell);
            innerCell = new PdfPCell(new Phrase(""));
            innerTable.AddCell(innerCell);

            innerCell = new PdfPCell(new Phrase("Quotation Number:"));
            innerTable.AddCell(innerCell);
            innerCell = new PdfPCell(new Phrase(""));
            innerTable.AddCell(innerCell);

            innerCell = new PdfPCell(new Phrase("Grant/Class:"));
            innerTable.AddCell(innerCell);
            innerCell = new PdfPCell(new Phrase(grant));
            innerTable.AddCell(innerCell);

            cell.AddElement(innerTable);
            table.AddCell(cell);

            document.Add(table);

            document.Add(new Paragraph("\n"));

            table = new PdfPTable(6);
            table.WidthPercentage = 100;
            widths = new float[] { 2f, 2f, 2f, 8f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers
            //PdfPCell cell = new PdfPCell(new Phrase(new Chunk("QTY", boldFont)));
            //cell.Rowspan = 2;

            //cell.HorizontalAlignment = Element.ALIGN_CENTER;

            //table.AddCell(cell);

            cell = new PdfPCell();

            cell.Phrase = new Phrase(new Chunk("Item No", boldFont));
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Order Qty", boldFont));
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit", boldFont));
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Description", boldFont));
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit Price", boldFont));
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total Price", boldFont));
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            //cell.Rowspan = 2;
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            var count = 0;
            foreach (var element in form.Element("items").Elements("item"))
            {
                count++;
                total += Convert.ToDouble(element.Element("unit_price") != null ? element.Element("unit_price").Value : element.Element("unit_cost").Value) * Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("unit").Value);

                cell.Phrase = new Phrase(count);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("itemname").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit_price") != null ? element.Element("unit_price").Value : element.Element("unit_cost").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", Convert.ToDouble(element.Element("unit_price") != null ? element.Element("unit_price").Value : element.Element("unit_cost").Value) * Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("unit").Value)));
                table.AddCell(cell);

            }

            cell = new PdfPCell();
            cell.Colspan = 5;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL : ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:N}", total));
            table.AddCell(cell);

            cell.Phrase = new Phrase("");
            cell.Rowspan = 3;
            table.AddCell(cell);


            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.Rowspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;

            cell.Phrase = new Phrase(new Chunk("EXCHANGE RATE: US$1 = ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(form.Element("conversion_rate").Value);
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL US$ = ", boldFont));
            table.AddCell(cell);

            double total_usd = total / Convert.ToDouble(form.Element("conversion_rate").Value);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(total_usd.ToString());
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);

            document.Add(table);
            document.Add(new Paragraph(""));


            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            //cell.Phrase = new Phrase("File");
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(DateTime.Parse(signature.Element("created_at").Value).ToString("d")));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Requestor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            try
            {
                //get the signature of the procurement lead
                signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "13");
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                //cell.Phrase = new Phrase("File");
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(""));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(""));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Procurement Lead Signature"));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Name and Title"));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase("Date"));
                table.AddCell(cell);
            }
            catch (Exception)
            {

            }

            //get the signature of the program lead
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "14");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            //cell.Phrase = new Phrase("File");
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Program Lead Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the finance lead
            /*
             * Some signatures might be empty when this doc is generated so check for that #when updating the workflow
             */
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "15");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            //cell.Phrase = new Phrase("File");
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Finance Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the country director
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "16");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            //cell.Phrase = new Phrase("File");
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Country Director Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //add the signature table to the document
            document.Add(table);
            document.Add(new Paragraph(""));

            table = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            cell = new PdfPCell(new Phrase(new Chunk("Bank transfers and checks must be made out to the vendor or vending company ONLY.", boldFont)));
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Account Name:"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Term of Payment:"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Account No:"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Bank Name:"));
            table.AddCell(cell);

            document.Add(table);


            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };


            cell = new PdfPCell(new Phrase("\n"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("\n"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("\n"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Vendor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            document.Add(table);

            //document.NewPage();
            //document.Add(new Paragraph("This is the I Agree Page"));

            // Close the document
            document.Close();
            // Close the writer instance
            //write.Flush();
            writer.Close();
            // Always close open filehandles explicity
            ////fs.Flush();
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        /*
         * Created By Johnbosco
         * created out of the createPurchaseOrder
         */
        public void NewCreatePurchaseOrder(string filepath, string grant, XElement form, string header, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 20, 20, 15, 15);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Order");
            document.AddSubject("Purchase Order");
            document.AddTitle("Purchase Order");
            document.Open();
            var vendorName = "";

            //add the title
            var titleText = new Paragraph("Purchase Order", titleFont);
            titleText.Alignment = Element.ALIGN_CENTER;
            document.Add(titleText);

            //add watermark to the document
            //add the document header
            var imageDiv = new PdfDiv();
            imageDiv.Width = 240f;
            imageDiv.Float = PdfDiv.FloatType.LEFT;
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(220f, 110f);
            imageDiv.Content.Add(jpg);
            //jpg.SpacingAfter = 10f;

            document.Add(imageDiv);

            /*
             * Add the PO number, Order date and Promised delivery date
             */
            var side = new PdfDiv();
            side.Width = 210;
            side.Float = PdfDiv.FloatType.RIGHT;

            Paragraph topParagraph = new Paragraph
            {
                Alignment = Element.ALIGN_LEFT
            };
            //var vendorDeliveryDate = "";
            //foreach (var element in definition.Element("vendors").Elements("vendor"))
            //{
            //    if (element.Attribute("selected") != null && int.Parse(element.Attribute("selected").Value) == 1)
            //    {
            //        var vendorName = element.Element("name").Value;

            //        //cell = new PdfPCell(new Phrase(new Chunk("TO:  \n" + vendorName, boldFont)));
            //        cell = new PdfPCell(PrintTextElement(bodyFont, boldFont, "TO", vendorName, ":\n"));
            //        table.AddCell(cell);
            //    }
            //}

            string process_number = definition.Element("process_number").Value;
            var PoNumber = "PO-" + process_number.Substring(2);
            topParagraph.SpacingAfter = 10f;
            topParagraph.Add(new Paragraph("\n"));
            topParagraph.Add(new Paragraph(PrintTextElement(bodyFont, boldFont, "PO Number", PoNumber, ":")));
            topParagraph.Add(PrintTextElement(bodyFont, boldFont, "Order Date", DateTime.Parse(definition.Element("created_at").Value).ToString("d"), ":"));
            topParagraph.Add(new Paragraph("Promised Delivery Date:  "));
            side.Content.Add(topParagraph);

            document.Add(side);

            /*
             * Add table with To, Ship To etc
             */
            PdfPTable table = new PdfPTable(3);
            table.WidthPercentage = 100;
            float[] widths = { 3f, 3f, 5f };
            table.SetWidths(widths);
            //add who the po is to
            PdfPCell cell;
            foreach (var element in definition.Element("vendors").Elements("vendor"))
            {
                if (element.Attribute("selected") != null && int.Parse(element.Attribute("selected").Value) == 1)
                {
                    vendorName = element.Element("name").Value;

                    //cell = new PdfPCell(new Phrase(new Chunk("TO:  \n" + vendorName, boldFont)));
                    cell = new PdfPCell(PrintTextElement(bodyFont, boldFont, "TO", vendorName, ":\n"));
                    table.AddCell(cell);
                }
            }
            cell = new PdfPCell(PrintTextElement(bodyFont, boldFont, "SHIP TO", "MGIC Store", ":\n"));
            table.AddCell(cell);

            // build request details
            var requestDiv = new PdfDiv();
            requestDiv.Content.Add(new Paragraph(PrintTextElement(bodyFont, boldFont, "MGIC", "", ":")));
            requestDiv.Content.Add(new Paragraph(PrintTextElement(bodyFont, boldFont, "Purchase Req No", definition.Element("process_number").Value, ":")));
            var reqName = "";
            foreach (var element in definition.Element("steps").Elements("step"))
            {
                if (int.Parse(element.Element("code").Value) == 1)
                {
                    reqName = element.Element("created_by_name").Value;
                }
            }
            requestDiv.Content.Add(new Paragraph(PrintTextElement(bodyFont, boldFont, "Requestor Name", reqName, ":")));
            requestDiv.Content.Add(new Paragraph(PrintTextElement(bodyFont, boldFont, "Requestor Phone No", "", ":")));
            requestDiv.Content.Add(new Paragraph(PrintTextElement(bodyFont, boldFont, "Quotation Number", "", ":")));
            requestDiv.Content.Add(new Paragraph(PrintTextElement(bodyFont, boldFont, "Grant/Class", grant, ":")));

            cell.AddElement(requestDiv);
            table.AddCell(cell);

            document.Add(table);

            document.Add(new Paragraph("\n"));

            table = new PdfPTable(6);
            table.WidthPercentage = 100;
            widths = new float[] { 1f, 1.5f, 4.5f, 7f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers

            cell = new PdfPCell();
            cell.Padding = 10f;

            cell.Phrase = new Phrase(new Chunk("#", boldFont));
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Qty", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Description", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit Price", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total Price", boldFont));
            //cell.Rowspan = 2;
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            cell.Padding = 5f;
            var count = 0;
            foreach (var element in form.Element("items").Elements("item"))
            {
                count++;
                total += Convert.ToDouble(element.Element("vendors_unit_price").Value) * Convert.ToDouble(element.Element("quantity").Value);

                cell.Phrase = new Phrase(count.ToString("0"));
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("description").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("vendors_unit_price").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(
                    $"{Convert.ToDouble(element.Element("vendors_unit_price").Value) * Convert.ToDouble(element.Element("quantity").Value):N}");
                table.AddCell(cell);
            }

            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 5;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("Total Cost ", boldFont));
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:N}", total));
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //cell.Rowspan = 3;
            //table.AddCell(cell);

            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 5;
            cell.Rowspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("Exchange rate: US$1", boldFont));
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 1;
            cell.Phrase = new Phrase(form.Element("conversion_rate").Value);
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);

            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 5;
            cell.Rowspan = 1;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL ", boldFont));
            table.AddCell(cell);

            double total_usd = total / Convert.ToDouble(form.Element("conversion_rate").Value);

            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 1;
            cell.Phrase = new Phrase(total_usd.ToString("C"));
            table.AddCell(cell);

            document.Add(table);
            document.Add(new Paragraph(""));

            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 15f
            };
            widths = new float[] { 2f, 2f, 1f };
            table.SetWidths(widths);

            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            cell = new PdfPCell();
            cell.Colspan = 1;
            cell.Padding = 5f;

            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            //cell.Phrase = new Phrase("File");
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            cell.BorderWidthBottom = 0f;
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("created_at").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Requestor Signature", bodyFont);
            cell.Padding = 1f;
            cell.BorderWidthBottom = .7f;
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name and Title", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Date", bodyFont);
            table.AddCell(cell);

            try
            {
                //get the signature of the procurement lead
                signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "13");
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                cell.Padding = 5f;
                cell.BorderWidthBottom = 0f;
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("created_at").Value, boldFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Procurement Lead Signature", bodyFont);
                cell.Padding = 1f;
                cell.BorderWidthBottom = .7f;
                table.AddCell(cell);

                cell.Phrase = new Phrase("Name and Title", bodyFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Date", bodyFont);
                table.AddCell(cell);

                //get the signature of the compliance person
                signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "14");
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                cell.Padding = 5f;
                cell.BorderWidthBottom = 0f;
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("created_at").Value, boldFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Compliance Signature", bodyFont);
                cell.Padding = 1f;
                cell.BorderWidthBottom = .7f;
                table.AddCell(cell);

                cell.Phrase = new Phrase("Name and Title", bodyFont);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Date", bodyFont);
                table.AddCell(cell);
            }
            catch (Exception ex)
            {
                Utils.LogError(ex);
            }

            //get the signature of the program lead
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "15");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            cell.Padding = 5f;
            cell.BorderWidthBottom = 0f;
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("created_at").Value.ToString(), boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Program Lead Signature", bodyFont);
            cell.Padding = 1f;
            cell.BorderWidthBottom = .7f;
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name and Title", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Date", bodyFont);
            table.AddCell(cell);

            //get the signature of the finance lead
            /*
             * Some signatures might be empty when this doc is generated so check for that #when updating the workflow
             */
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "16");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            cell.Padding = 5f;
            cell.BorderWidthBottom = 0f;
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("created_at").Value.ToString(), boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("DFA Signature", bodyFont);
            cell.Padding = 1f;
            cell.BorderWidthBottom = .7f;
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name and Title", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Date", bodyFont);
            table.AddCell(cell);

            //get the signature of the country director
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "17");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            cell.Padding = 5f;
            cell.BorderWidthBottom = 0f;
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("name").Value, boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("created_at").Value.ToString(), boldFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Country Director Signature", bodyFont);
            cell.Padding = 1f;
            cell.BorderWidthBottom = .7f;
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name and Title", bodyFont);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Date", bodyFont);
            table.AddCell(cell);

            //add the signature table to the document
            document.Add(table);
            document.Add(new Paragraph(""));

            table = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingBefore = 10f
            };

            cell = new PdfPCell(new Phrase(new Chunk("Bank transfers and checks must be made out to the vendor or vending company ONLY.", boldFont)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Colspan = 2;
            cell.Padding = 3f;
            table.AddCell(cell);

            cell.Phrase = PrintTextElement(bodyFont, boldFont, "Account Name", form.Element("account_name").Value, ":");
            //cell = new PdfPCell(new Phrase("Account Name: "));
            cell.Colspan = 1;
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("Term of Payment:"));
            cell.Phrase = PrintTextElement(bodyFont, boldFont, "Term of Payment", form.Element("term_of_payment").Value, ":");
            cell.Colspan = 1;
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("Account No:"));
            cell.Phrase = PrintTextElement(bodyFont, boldFont, "Account No", form.Element("account_number").Value, ":");
            cell.Colspan = 1;
            table.AddCell(cell);

            //cell = new PdfPCell(new Phrase("Bank Name:"));
            cell.Phrase = PrintTextElement(bodyFont, boldFont, "Bank Name", form.Element("bank_name").Value, ":");
            cell.Colspan = 1;
            table.AddCell(cell);

            document.Add(table);


            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };
            widths = new float[] { 2f, 2f, 1f };
            table.SetWidths(widths);

            cell = new PdfPCell(new Phrase("\n"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(vendorName));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("\n"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Vendor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            document.Add(table);

            document.NewPage();
            table = new PdfPTable(2)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };
            cell = new PdfPCell(new Phrase(new Chunk("MGIC Purchase Order Standard Terms and Conditions", boldFont)))
            {
                HorizontalAlignment = Element.ALIGN_CENTER,
                Colspan = 2
            };
            table.AddCell(cell);
            document.Add(table);


            document.Add(new Paragraph("I. CONTRACT", boldFont));

            document.Add(new Paragraph(" This purchase order (PO), when properly completed and signed by authorized MGIC representatives, is the only form which will be recognized by MGIC and will constitute the fixed-price contract. No terms stated by the Supplier in accepting or acknowledging this order shall be binding on MGIC unless accepted in writing by MGIC. The purchase order may not be assigned or delegated, in whole or in part, by the Supplier without the written consent of MGIC; absence of such written consent, any assignment is void.")
            {
                Alignment = Element.ALIGN_JUSTIFIED
            });

            document.Add(new Paragraph("II. CONDUCT", boldFont));

            document.Add(new Paragraph(" Vendor and its employees shall maintain and comply with a written code of conduct that prohibits giving anything of value, directly or indirectly, to any person or entity, including government officials or MGIC staff, in the form of a bribe or kickback; establishes appropriate limitations on transactions with relatives of Vendor employees or businesses or ventures related to Vendor or its employees; and otherwise properly governs the performance of its employees engaged in soliciting, awarding or administering contracts, and receiving gifts. Vendor shall inform MGIC in writing of any violations relating to its obligations hereunder. Vendor certifies that it has not knowingly provided and will not knowingly provide, in violation of applicable laws and moral codes, material support or resources to any individual or organization that advocates, plans, sponsors, engages in, or has engaged in an act of terrorism. Vendor shall comply with industry best practices to avoid exploitation of child labor and shall not discriminate on the basis of race, ethnicity, religion, national origin, gender, age, sexual orientation, marital status, citizenship status, disability, or military status. During the term hereof and for three years afterwards, except to perform the terms of this Agreement, Vendor shall not disclose information regarding MGIC to any third-party or make use of such information for its own purposes without MGIC’s prior written consent. The Vendor shall not use the MGIC name or trademarks in publicity or publicly disclose information relating to the Order without MGIC's prior written consent.")
            {
                Alignment = Element.ALIGN_JUSTIFIED
            });

            document.Add(new Paragraph("III. ENTIRE AGREEMENT", boldFont));


            document.Add(new Paragraph(" Vendor and its employees shall maintain and comply with a written code of conduct that prohibits giving anything of value, directly or indirectly, to any person or entity, including government officials or MGIC staff, in the form of a bribe or kickback; establishes appropriate limitations on transactions with relatives of Vendor employees or businesses or ventures related to Vendor or its employees; and otherwise properly governs the performance of its employees engaged in soliciting, awarding or administering contracts, and receiving gifts. Vendor shall inform MGIC in writing of any violations relating to its obligations hereunder. Vendor certifies that it has not knowingly provided and will not knowingly provide, in violation of applicable laws and moral codes, material support or resources to any individual or organization that advocates, plans, sponsors, engages in, or has engaged in an act of terrorism. Vendor shall comply with industry best practices to avoid exploitation of child labor and shall not discriminate on the basis of race, ethnicity, religion, national origin, gender, age, sexual orientation, marital status, citizenship status, disability, or military status. During the term hereof and for three years afterwards, except to perform the terms of this Agreement, Vendor shall not disclose information regarding MGIC to any third-party or make use of such information for its own purposes without MGIC’s prior written consent. The Vendor shall not use the MGIC name or trademarks in publicity or publicly disclose information relating to the Order without MGIC's prior written consent.")
            {
                Alignment = Element.ALIGN_JUSTIFIED
            });

            document.Add(new Paragraph("IV. PAYMENT", boldFont));
            var paragraphIV = new Paragraph(" Payment shall be made—via cheque, wire transfer, or cash—in the currency identified on the purchase order cover page for items that have been delivered to the delivery destination(s) set forth in the purchase order and that have been inspected and accepted by MGIC Inspection and Acceptance Committee. The Supplier shall submit an original invoice (or electronic invoice, if authorized) that includes, at a minimum: (a) name and address of the Supplier; (b) invoice date and number; (c) PO number; (d) description, quantity, unit of measure, unit price and extended price of the items delivered and tax PIN. Payment shall only be made on full PO performance unless expressly stated.");
            paragraphIV.Alignment = Element.ALIGN_JUSTIFIED;
            document.Add(paragraphIV);
            document.Add(new Paragraph());

            document.Add(new Paragraph("V. General Instructions.", boldFont));

            document.Add(new Paragraph("    1. Please retain this first original and return the second original duly signed and dated as evidence of your acceptance of this order."));
            document.Add(new Paragraph("    2. Please refer to this Purchase Order number in all correspondence concerning the order."));
            document.Add(new Paragraph("Invoice Address.The Invoice is to be delivered/sent to:", boldFont));
            document.Add(new Paragraph("            Procurement Department", boldFont));
            document.Add(new Paragraph("            Maryland Global Initiative Corporation (MGIC Nigeria)", boldFont));
            document.Add(new Paragraph("            No 2 Sirakoro street, off Blantyre street Wuse II, Abuja", boldFont));

            document.Add(new Paragraph("VI. SHIPPING; PACKING; DELIVERY", boldFont));
            var paragraphVI = new Paragraph(" Unless otherwise stated in this Order, all Goods shall be shipped freight prepaid D.D.U Destination, and MGIC will pay no charge for packing, boxing or cartage. Vendor is responsible for loss of or damage to any Goods/Services before receipt by MGIC at Destination. Each package of Goods will contain documentation showing shippers name, contents of package, and the Order Number.A copy of the bill of lading, invoice, customs and import / export notices, or similar documentation shall be sent at time of shipment to MGIC at the address stated in this Order as well as to the Destination, if different.Time is of the essence and delivery of Goods / Services shall be strictly in accordance with this Order.Delays in shipment or otherwise shall be reported immediately to MGIC, and the Order payment shall be subject to a late arrival penalty if specified on the Order.Partial deliveries/ performance may not be accepted; contact MGIC in advance if a partial delivery/ performance is requested.")
            {
                Alignment = Element.ALIGN_JUSTIFIED
            };
            document.Add(paragraphVI);


            document.Add(new Paragraph("ELIGIBILITY OF COMMODITIES AND SUPPLIERS", boldFont));
            document.Add(new Paragraph(" The Supplier shall adhere to the following in carrying out this purchase order:"));
            document.Add(new Paragraph("    a. All commodities must be new and unused unless otherwise authorized in writing by MGIC.", endingMessageFont));
            document.Add(new Paragraph("    b. All electrical commodities must operate on the voltage and frequency identified on the purchase order cover page and its attachments.", MessageFont));
            document.Add(new Paragraph("    c. No commodities or services shall be eligible for payment under this order if provided by a vendor included on any list of suspended, debarred, or ineligible under Government Of Nigeria and under the United States Watchlist", MessageFont));
            document.Add(new Paragraph("    d. No payment shall be made to Vendor(s) that support/ promote activities that are prohibited and restricted by the United Sates Government under various legislations.", MessageFont));
            document.Add(new Paragraph("    e. All materials, equipment, supplies or services shall conform to federal and State laws and regulations and to the specifications contained in the solicitation or request for quote/proposal", MessageFont));
            document.Add(new Paragraph("    f. If any Goods/Services are defective in material or workmanship or otherwise do not conform to this Order, MGIC may: (a) require Vendor to repair or replace at Vendor’s cost any such nonconforming Goods/Services; (b) require Vendor to refund the price of any such Goods/Services; or (c) elect to retain and correct any such Goods/Services with an appropriate price reduction to offset MGIC’s costs of making correction(s). Nothing, including any final inspection, shall relieve Vendor from its responsibility to correct or replace Goods/Services defective as a result of fraud, recklessness or latent defects.", MessageFont));
            document.Add(new Paragraph("VII. INSPECTION AND ACCEPTANCE", boldFont));

            document.Add(new Paragraph(" The Supplier shall only tender for acceptance those items that conform to the requirements of this PO. MGIC reserves the right to inspect or test any supplies or services that have been tendered for acceptance. MGIC may require repair or replacement of nonconforming commodities or re-performance of nonconforming services at no increase in purchase order price. If repair/replacement or re-performance will not correct the defects or is not possible, MGIC may seek an equitable price reduction or adequate consideration for acceptance of nonconforming commodities or services. MGIC must exercise its post-acceptance rights within a reasonable time after the defect was discovered or should have been discovered."));
            document.Add(new Paragraph("XI. WARRANTY", boldFont));

            document.Add(new Paragraph(" The Supplier warrants and implies that the items delivered hereunder are merchantable and fit for use for the particular purpose described in this PO. All equipment supplied under this order must be covered by the manufacturer's standard international warranty which shall, at a minimum, protect MGIC from any loss due to defective workmanship, material, and parts, for 12 (twelve) months (unless otherwise stated on the purchase order document) after the equipment is delivered to and accepted by MGIC. In the event that the warranty is breached, MGIC may require, and the Supplier is bound, to remedy all defects and faults, including both workmanship and materials within a reasonable time of notification. The Supplier shall be responsible for all necessary domestic transportation charges required to ship the defective commodities to the Supplier and replacement commodities to MGIC. In the event of the Supplier's refusal, failure, or inability to remedy such discrepancies within a reasonable time of notification, MGIC may remedy such defects on its own and claim the reasonable cost of such remedial action from the Supplier."));

            document.Add(new Paragraph("XII. GOVERNING LAW AND RESOLUTION OF DISPUTES", boldFont));
            var paragraphA = new Paragraph("    (A) Governing Law. This purchase order, including any disputes related thereto, shall be                                      governed by the Laws of Nigeria");
            paragraphA.Alignment = Element.ALIGN_JUSTIFIED;
            document.Add(paragraphA);

            var paragraphB = new Paragraph("    (B) Disputes between the Parties; The following procedures shall govern the resolution of any controversy, dispute or claim between or among the “Parties,” arising out of the interpretation, performance, breach or alleged breach of this purchase order (“Dispute”).");
            paragraphB.Alignment = Element.ALIGN_JUSTIFIED;
            document.Add(paragraphB);


            var paragraphB1 = new Paragraph("            (1) Negotiation- the Parties shall promptly attempt to resolve any Dispute by negotiation in the normal course of business. If, after good faith efforts, the Dispute is not resolved, either Party may request in writing that the Dispute be resolved through mediation, arbitration and litigation in that order.")
            {
                Alignment = Element.ALIGN_JUSTIFIED
            };
            document.Add(paragraphB1);

            document.Add(new Paragraph("            (2) Obligation to perform work- Supplier shall diligently proceed with the                                                    performance of work pending final resolution of any Dispute."));
            document.Add(new Paragraph("XIII. INDEMNITY AND SUPPLIER WAIVER OF BENEFITS.", boldFont));
            document.Add(new Paragraph(" The Supplier agrees to indemnify and save harmless MGIC and its officers, employees, and agents from and against any and all claims and liability, loss, expenses, suits, damages, judgments, demands, and costs (including reasonable legal and professional fees and expenses) arising out of the Supplier’s provision of goods or services under this purchase order."));
            document.Add(new Paragraph("XIV. EXCUSABLE DELAYS", boldFont));
            document.Add(new Paragraph(" The Supplier shall be liable for default unless nonperformance is caused by an occurrence beyond the reasonable control of the Supplier and without its fault or negligence such as, acts of God or the public enemy, acts of the Government in either its sovereign or contractual capacity, fires, floods, epidemics, quarantine restrictions, strikes, unusually severe weather, and delays of common carriers. The Supplier shall notify MGIC in writing as soon as it is reasonably possible after the commencement of any excusable delay, setting forth the full particulars in connection therewith, shall remedy such occurrence with all reasonable dispatch, and shall promptly give written notice to MGIC of the cessation of such occurrence."));

            document.Add(new Paragraph("XV. CHANGES", boldFont));
            document.Add(new Paragraph(" MGIC may at any time, by written order, and without notice to the sureties, make changes within the general scope of this purchase order. If any such changes cause an increase or decrease in the cost, or the time required for the performance, of any part of the work under this purchase order, an equitable adjustment shall be made in the purchase order price or delivery schedule, or both, and the purchase order shall be modified in writing accordingly. Any claim by the Supplier for adjustment under this purchase order must be asserted within thirty (30) days from the date of receipt by the Supplier of the modification or change."));
            document.Add(new Paragraph("XVI. TERMINATION FOR CONVENIENCE", boldFont));
            var paragraphXVI = new Paragraph(" MGIC reserves the right to terminate this purchase order, or any part hereof, for its sole convenience. In the event of such termination, the Supplier shall immediately stop all work hereunder and shall immediately cause any and all of its suppliers and subcontractors to cease work. Subject to the terms of this PO, the Supplier shall be paid a percentage of the PO price reflecting the percentage of the work performed prior to the notice of termination, plus reasonable charges the Supplier can demonstrate to the satisfaction of MGIC using its standard record keeping system, have resulted from the termination. The Supplier shall not be paid for any work performed or costs incurred which reasonably could have been avoided after the notice of termination is given.");
            paragraphXVI.Alignment = Element.ALIGN_JUSTIFIED;
            document.Add(paragraphXVI);


            document.Add(new Paragraph("XVII. TERMINATION FOR CAUSE", boldFont));

            document.Add(new Paragraph(
                " MGIC may terminate this purchase order, or any part hereof, for cause in the event of any default by the Supplier, or if the Supplier fails to comply with any PO terms and conditions, or fails to provide MGIC, upon request, with adequate assurances of future performance. In the event of termination for cause, MGIC shall not be liable to the Supplier for any amount for supplies or services not accepted, and the Supplier shall be liable to MGIC for any and all rights and remedies provided by law. If it is determined that MGIC improperly terminated this PO for default, such termination shall be deemed a termination for convenience.")
            {
                Alignment = Element.ALIGN_JUSTIFIED
            });

            document.Add(new Paragraph("XVIII. TITLE", boldFont));
            document.Add(new Paragraph(" Unless specified elsewhere in this contract, title to items rendered under this contract shall pass to MGIC upon acceptance."));
            document.Add(new Paragraph("XIX. RISK OF LOSS.", boldFont));

            document.Add(new Paragraph(
                " Unless the purchase order specifically provides otherwise, risk of loss or damage to the items provided under this purchase order shall remain with the Supplier until, and shall pass to MGIC upon delivery, inspection and acceptance of the items to MGIC or MGIC’ authorized agent at the delivery location.")
            { Alignment = Element.ALIGN_JUSTIFIED });
            document.Add(new Paragraph("\n"));

            document.Add(new Paragraph("Vendor Signature                                        Name & Title:                                       Date:"));



            var paragraph = new Paragraph();

            // Close the document
            document.Close();
            // Close the writer instance
            //write.Flush();
            writer.Close();
            // Always close open filehandles explicity
            //fs.Flush();
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        public void createVPRF(string filepath, string grant, XElement form, string header, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Vendor Payment Request Form");
            document.AddSubject("Vendor Payment Request Form");
            document.AddTitle("Vendor Payment Request Form");


            document.Open();

            //add watermark to the document

            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 2f, 5f };
            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(5);
            table.WidthPercentage = 100;
            widths = new float[] { 2f, 2f, 6f, 3f, 3f };
            table.SetWidths(widths);

            cell = new PdfPCell(new Phrase(new Chunk("Date Received by:" + " ______________ ")));
            cell.Colspan = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Requesting Unit:")));
            cell.Colspan = 5;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Deliver to : " + "_______________" + "  Grant: " + grant.ToUpper() + "   Location:" + " ____________")));
            cell.Colspan = 5;
            table.AddCell(cell);

            //State Creating table headers
            cell = new PdfPCell(new Phrase(new Chunk("Item Description", boldFont)));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Quantity", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Date Needed", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Anticipated Price Per Unit", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total Anticipated Price", boldFont));
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            foreach (var element in form.Element("items").Elements("item"))
            {
                total += Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("unit_price") != null ? element.Element("unit_price").Value : element.Element("unit_cost").Value) * Convert.ToDouble(element.Element("unit").Value);

                cell.Phrase = new Phrase(element.Element("description").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase("");
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", element.Element("unit_price") != null ? element.Element("unit_price").Value : element.Element("unit_cost").Value));
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("unit_price") != null ? element.Element("unit_price").Value : element.Element("unit_cost").Value) * Convert.ToDouble(element.Element("unit").Value)));
                table.AddCell(cell);
            }

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL : ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:N}", total));
            table.AddCell(cell);

            document.Add(table);
            document.Add(new Paragraph(""));

            table = new PdfPTable(1)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            cell = new PdfPCell(new Phrase("Amount in Words:"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Suggested sources:"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Reason for purchase:" + " __________ " /*form.Element("reason_for_purchase").Value*/));
            table.AddCell(cell);

            document.Add(table);

            if (form.Element("payments") != null && form.Element("payments").Elements("payment").Any())
            {
                var payment = form.Element("payments").Elements("payment").LastOrDefault();

                //add amount being paid
                table = new PdfPTable(2)
                {
                    WidthPercentage = 100
                };

                cell = new PdfPCell(new Phrase("Amount being paid : "));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(string.Format("{0:N}", payment.Element("vprf_amount").Value)));
                table.AddCell(cell);
            }

            //add destination information
            document.Add(table);

            table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            cell = new PdfPCell(new Phrase("Final Delivery Location:" + " ________"/*form.Element("final_delivery_location").Value*/));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name of Consignee:"));
            table.AddCell(cell);

            document.Add(table);

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);


            table = new PdfPTable(4)
            {
                WidthPercentage = 100,
            };

            cell = new PdfPCell(new Phrase("Requesting official:"));
            cell.Rowspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(4)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "20");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));

            cell = new PdfPCell(new Phrase("COO/Office Manager Approval:"));
            cell.Rowspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            //write.Flush();
            // Always close open filehandles explicity
            //fs.Flush();
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        /*
         * Created by Johnbosco
         * Created out of the CreateVPRF
         */
        public void NewCreateVPRF(string filepath, string grant, XElement form, string header, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Vendor Payment Request Form");
            document.AddSubject("Vendor Payment Request Form");
            document.AddTitle("Vendor Payment Request Form");

            document.Open();
            //add watermark to the document
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            var requestDiv = new PdfDiv()
            {
                TextAlignment = Element.ALIGN_RIGHT
            };
            requestDiv.Content.Add(new Paragraph(PrintTextElement(bodyFont, boldFont, "PRF Number", definition.Element("process_number").Value, ":")));
            //PdfPTable table = new PdfPTable(2);
            //table.WidthPercentage = 100;
            //float[] widths = new float[] { 2f, 5f };
            //PdfPCell cell = new PdfPCell(new Phrase(new Chunk("PRF Number", boldFont)));
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            //table.AddCell(cell);

            //document.Add(table);
            document.Add(requestDiv);

            PdfPTable table = new PdfPTable(5);
            //table = new PdfPTable(5);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 6f, 2f, 2f, 3f, 3f };
            table.SetWidths(widths);

            PdfPCell cell = new PdfPCell(PrintTextElement(bodyFont, boldFont, "Date Received by", form.Element("date_received_by").Value, ":"));
            cell.Colspan = 5;
            cell.Padding = 5f;
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Requesting Unit: " + form.Element("requesting_office").Value));
            cell.Colspan = 5;
            table.AddCell(cell);

            //adjust the colspan here also set the padding and cells
            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Deliver to", form.Element("final_delivery_location").Value, ":"));
            cell.Colspan = 1;
            cell.BorderWidth = 0f;
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Grant", grant.ToUpper(), ":"));
            cell.Colspan = 2;
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Location", form.Element("final_delivery_location").Value, ":"));
            cell.Colspan = 2;
            table.AddCell(cell);

            //State Creating table headers
            cell = new PdfPCell(new Phrase(new Chunk("Item Description", bodyFont)));
            cell.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Quantity", bodyFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Date Needed", bodyFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Anticipated Price Per Unit", bodyFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total Anticipated Cost", bodyFont));
            table.AddCell(cell);

            //cell.Phrase = new Phrase("");
            //table.AddCell(cell);
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            cell.Padding = 6f;
            foreach (var element in form.Element("items").Elements("item"))
            {
                total += Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("vendors_unit_price").Value);

                cell.Phrase = new Phrase(element.Element("description").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase("");
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", element.Element("vendors_unit_price").Value));
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("vendors_unit_price").Value)));
                table.AddCell(cell);
            }

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.Padding = 10f;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL : ", bodyFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:N}", total), boldFont);
            table.AddCell(cell);

            document.Add(table);


            // Creating Payment  headers
            PdfPTable tablePayment = new PdfPTable(3);
            tablePayment.WidthPercentage = 100;
            float[] T = new float[] { 9f, 3f, 3f };
            tablePayment.SetWidths(T);
            var cellPayment = new PdfPCell(new Phrase(new Chunk("Annual Payment tracker", bodyFont)));
            cellPayment.BackgroundColor = new CMYKColor(0, 0, 0, 20);
            cellPayment.HorizontalAlignment = Element.ALIGN_CENTER;
            tablePayment.AddCell(cellPayment);

            cellPayment.Phrase = new Phrase(new Chunk("Amount", bodyFont));
            tablePayment.AddCell(cellPayment);

            cellPayment.Phrase = new Phrase(new Chunk("Payment Date", bodyFont));
            tablePayment.AddCell(cellPayment);

            //End table headers


            cellPayment = new PdfPCell();
            cellPayment.Padding = 6f;
            foreach (var element in form.Element("payments").Elements("payment"))
            {
                cellPayment.Phrase = element.Element("new_description") != null ? new Phrase(element.Element("new_description").Value) : new Phrase("");
                //cell.Phrase = new Phrase(element.Element("new_description").Value);
                tablePayment.AddCell(cellPayment);

                cellPayment.Phrase = new Phrase(element.Element("vprf_amount").Value);

                tablePayment.AddCell(cellPayment);
                cellPayment.Phrase = new Phrase(element.Element("p_date").Value);
                tablePayment.AddCell(cellPayment);
            }


            tablePayment.AddCell(cellPayment);
            document.Add(tablePayment);




            table = new PdfPTable(1)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            cell = new PdfPCell(new Phrase("Amount in Words:"));
            cell.Padding = 5f;
            table.AddCell(cell);

            cell.Phrase = new Phrase("Suggested sources:");
            table.AddCell(cell);

            cell.Phrase = new Phrase(PrintTextElement(bodyFont, boldFont, "Reason for purchase", form.Element("reason_for_purchase").Value, ":"));
            table.AddCell(cell);

            document.Add(table);

            if (form.Element("payments") != null && form.Element("payments").Elements("payment").Any())
            {
                var payment = form.Element("payments").Elements("payment").FirstOrDefault();

                //add amount being paid
                table = new PdfPTable(2)
                {
                    WidthPercentage = 100
                };

                cell = new PdfPCell(new Phrase("Amount being paid : ", bodyFont));
                table.AddCell(cell);

                cell = new PdfPCell(new Phrase(string.Format("{0:N}", payment.Element("vprf_amount").Value), boldFont));
                table.AddCell(cell);
            }

            //add destination information
            document.Add(table);

            table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            cell = new PdfPCell(new Phrase(PrintTextElement(bodyFont, boldFont, "Final Delivery Location", form.Element("final_delivery_location").Value, ":")));
            cell.Padding = 5f;
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name of Consignee:");
            table.AddCell(cell);

            document.Add(table);

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Colspan = 1;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            table = new PdfPTable(4)
            {
                WidthPercentage = 100,
            };

            cell.Phrase = new Phrase("Requesting official:");
            cell.Rowspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell();
            cell.Padding = 5f;
            cell.Phrase = new Phrase(signature.Element("name").Value);
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("created_at").Value);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name");
            table.AddCell(cell);

            cell.Phrase = new Phrase("Signature");
            table.AddCell(cell);

            cell.Phrase = new Phrase("Date");
            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(4)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            signature = form.Element("signatures").Elements("signature").LastOrDefault(e => e.Element("step").Value.ToString() == "21");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));

            cell = new PdfPCell(new Phrase("COO/Office Manager Approval:"));
            cell.Padding = 8f;
            cell.Rowspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            cell.Padding = 8f;
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell.Phrase = new Phrase(signature.Element("created_at").Value);
            table.AddCell(cell);

            cell.Phrase = new Phrase("Name");
            table.AddCell(cell);

            cell.Phrase = new Phrase("Signature");
            table.AddCell(cell);

            cell.Phrase = new Phrase("Date");
            table.AddCell(cell);

            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            //write.Flush();
            writer.Close();
            // Always close open filehandles explicity
            //fs.Flush();
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        public void createTravelAdvance(string filepath, string grant, XElement form, string header, admin_hrm_employee employee, DateTime created_at, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Request form");
            document.AddSubject("Purchase Reqeust form");
            document.AddTitle("Purchase Request form");

            document.Open();

            //add watermark to the document
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 2f, 5f });

            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Full Name", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname))));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Grant/Class", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(grant)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Purpose of Travel", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(form.Element("purpose_of_travel").Value)));
            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(5);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 6f, 5f, 3f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers
            cell = new PdfPCell(new Phrase(new Chunk("Description of Expenses", boldFont)));

            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Dates", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Daily Rate", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("No of Days", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Amount", boldFont));
            table.AddCell(cell);

            double total = 0;
            cell = new PdfPCell();
            foreach (var element in form.Element("expenses").Elements("expense"))
            {
                try
                {
                    total += (Convert.ToDouble(element.Element("no_of_days").Value) * Convert.ToDouble(element.Element("rate").Value));

                    cell.Phrase = new Phrase((XMLHelper.GetValue(element.Element("name"))));
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(XMLHelper.GetValue(element.Element("start_date")) + " - " + XMLHelper.GetValue(element.Element("end_date")));
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.Element("rate").Value);
                    table.AddCell(cell);

                    cell.Phrase = new Phrase(element.Element("no_of_days").Value);
                    table.AddCell(cell);
                    var val = Convert.ToDouble(element.Element("no_of_days").Value);
                    cell.Phrase = new Phrase(String.Format("{0:N}", (val == 0 ? 1 : val * Convert.ToDouble(element.Element("rate").Value)).ToString()));
                    table.AddCell(cell);
                }
                catch (Exception e)
                {
                    Utils.LogError(e);
                }
            }

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL Advanced: ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(String.Format("{0:N}", total));
            table.AddCell(cell);

            document.Add(table);

            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            var token = new bpm_transaction_token();
            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor
            if (signature != null)
            {
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                table.AddCell(cell);
            }
            else
            {
                //var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                cell.Phrase = new Phrase(new Chunk("", sign_font));
                table.AddCell(cell);
            }
            cell = new PdfPCell(new Phrase(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(created_at.ToShortDateString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Prepared by"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the grants and compliance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "4");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Reviewed by"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the supervisor
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "2");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Approved By"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);


            //get the signature of the country director
            if (form.Element("signatures").Elements("signature")
                    .FirstOrDefault(e => e.Element("step").Value.ToString() == "7") != null)
            {
                signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "7");
                token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
                cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("name").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(signature.Element("created_at").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase("Country Director");
                table.AddCell(cell);

                cell.Phrase = new Phrase("Name and Title");
                table.AddCell(cell);

                cell.Phrase = new Phrase("Date");
                table.AddCell(cell);
            }

            //add the signature table to the document
            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            //write.Flush();
            writer.Close();
            // Always close open filehandles explicity
            //fs.Flush();
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        public void createTravelAuthorization(string filepath, string grant, XElement form, string header, admin_hrm_employee employee, DateTime created_at, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Request form");
            document.AddSubject("Purchase Reqeust form");
            document.AddTitle("Purchase Request form");


            document.Open();

            //add watermark to the document



            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            //PdfPTable table = new PdfPTable(2);
            //table.WidthPercentage = 100;
            //float[] widths = new float[] { 2f, 5f };
            //PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            //table.AddCell(cell);

            //cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            //table.AddCell(cell);

            //document.Add(table);



            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 2f, 5f });

            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Staff Name", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname))));
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase(new Chunk("Class of Travel", boldFont)));
            table.AddCell(cell);

            var type_id = Convert.ToInt32(form.Element("travel_class").Value);
            var travel_type = Util.GetTravelTypes().FirstOrDefault(e => e.id == type_id).name;


            cell = new PdfPCell(new Phrase(new Chunk(travel_type)));
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase(new Chunk("Dates of Travel:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(form.Element("start_date").Value + " - " + form.Element("end_date").Value)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Departure:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(form.Element("departure").Value)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Arrival:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(form.Element("destination").Value)));
            table.AddCell(cell);




            cell = new PdfPCell(new Phrase(new Chunk("Grant/Class", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(grant)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Purpose of travel:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(form.Element("purpose_of_travel").Value)));
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase(new Chunk("Approved By: (Signature indicates approval of program relevance as stated above.)")));
            cell.Colspan = 2;
            table.AddCell(cell);


            //get the signature of the grants and compliance
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "2");
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));


            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);


            cell = new PdfPCell(new Phrase(new Chunk("Name:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Signature:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Title:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("")));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Date:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);



            document.Add(table);






            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        //create the purchase request form
        public void createPV(string filepath, string grant, XElement form, string header, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Request form");
            document.AddSubject("Purchase Reqeust form");
            document.AddTitle("Purchase Request form");


            document.Open();

            //add watermark to the document



            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            var pv_summary = new PVHelper().GetWorkflowPVSummary(Guid.Parse(form.Element("workflow_id").Value));

            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 2f, 5f };
            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Pay To", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(pv_summary.Recipient));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Pay By", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(form.Element("pay_by").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Bank Account ", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(form.Element("pay_from").Value));
            table.AddCell(cell);


            cell = new PdfPCell(new Phrase(new Chunk("Purpose", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(pv_summary.Purpose));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Conversion Rate", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(pv_summary.Purpose));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Bank name", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(pv_summary.VendorBank));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Account name", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(pv_summary.AccountName));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Account Number", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(pv_summary.VendorAccountNumber));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Amount in  Naira", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(pv_summary.Total.ToString()));
            table.AddCell(cell);

            var amount_usd = pv_summary.Total / pv_summary.ConversionRate;

            cell = new PdfPCell(new Phrase(new Chunk("Amount in USD", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(Math.Round(Convert.ToDouble(amount_usd), 2).ToString()));
            table.AddCell(cell);



            cell = new PdfPCell(new Phrase(new Chunk("Withholding Tax", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(form.Element("withholding_tax").Value));
            table.AddCell(cell);



            var net_amount = pv_summary.Total - (Convert.ToDecimal(form.Element("withholding_tax").Value) * pv_summary.Total);
            cell = new PdfPCell(new Phrase(new Chunk("Net Amount", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(form.Element("withholding_tax").Value));
            table.AddCell(cell);


            document.Add(table);


            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };


            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Requestor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);


            //get the signature of compliance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "2");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Compliance Review"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);


            //get the signature of the program lead
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "3");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Program Director Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the finance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "4");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Director of Finance and Admin Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);



            //get the signature of the finance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "5");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Country Director's Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //add the signature table to the document
            document.Add(table);


            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        private static void PdfStampInExistingFile(string watermarkImagePath, string sourceFilePath)
        {
            byte[] bytes = File.ReadAllBytes(sourceFilePath);
            var img = iTextSharp.text.Image.GetInstance(watermarkImagePath);
            img.SetAbsolutePosition(150, 300);
            img.ScaleToFit(300, 300);
            img.Alignment = Image.ALIGN_MIDDLE;


            PdfContentByte waterMark;

            using (MemoryStream stream = new MemoryStream())
            {
                PdfReader reader = new PdfReader(bytes);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        waterMark = stamper.GetUnderContent(i);
                        waterMark.AddImage(img);
                    }
                }
                bytes = stream.ToArray();
            }
            File.WriteAllBytes(sourceFilePath, bytes);
        }

        public static void CreateWaterMarkInPDF(string path, string watermark_path)
        {
            try
            {
                Image pageIn = Image.GetInstance(HttpContext.Current.Server.MapPath(watermark_path));


                pageIn.Alignment = iTextSharp.text.Image.ALIGN_MIDDLE;
                pageIn.ScaleToFit(300, 300);
                pageIn.Alignment = iTextSharp.text.Image.UNDERLYING;
                pageIn.SetAbsolutePosition(150, 300);


                PdfReader pdfReader = new PdfReader(path);
                string FileLocation = path;
                PdfStamper stamp = new PdfStamper(pdfReader, new FileStream(FileLocation.Replace(".pdf", "[temp][file].pdf"), FileMode.Create));

                PdfContentByte waterPDF;
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    waterPDF = stamp.GetOverContent(page);
                    waterPDF.AddImage(pageIn);
                }
                stamp.FormFlattening = true;
                stamp.Close();
                pdfReader.Close();
                // now delete the original file and rename the temp file to the original file 
                File.Delete(FileLocation);
                File.Move(FileLocation.Replace(".pdf", "[temp][file].pdf"), FileLocation);
            }
            catch (Exception ee)
            {
                throw new Exception("Testing");
            }
        }

        /*
         * Added By Johnbosco
         * For better formatting of Question and Answers
         * Takes in the question font, answer font, question, answer and a separartion character
         */
        private static Paragraph PrintTextElement(Font questionFont, Font answerFont, string question, string answer, string separator)
        {
            var p = new Paragraph();
            p.Add(new Phrase(question + separator + "  ", questionFont));
            p.Add(new Phrase(answer, answerFont));
            return p;
        }

        //create the reimbursment form
        //string filepath, XElement form, string header, XElement definition
        public void createRF(string filepath, string grant, XElement form, string header, admin_hrm_employee employee, DateTime created_at, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Purchase Request form");
            document.AddSubject("Purchase Reqeust form");
            document.AddTitle("Purchase Request form");

            document.Open();

            //add watermark to the document

            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 2f, 5f };
            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Document Number", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(definition.Element("process_number").Value));
            table.AddCell(cell);

            document.Add(table);

            table = new PdfPTable(5);
            table.WidthPercentage = 100;
            widths = new float[] { 2f, 2f, 6f, 3f, 3f };
            table.SetWidths(widths);
            //State Creating table headers
            cell = new PdfPCell(new Phrase(new Chunk("QTY", boldFont)));
            cell.Rowspan = 2;

            cell.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Description", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Unit Cost", boldFont));
            table.AddCell(cell);

            cell.Phrase = new Phrase(new Chunk("Total", boldFont));
            table.AddCell(cell);

            /*cell.Phrase = new Phrase("");
            cell.Rowspan = 2;
            table.AddCell(cell);*/
            //End table headers
            double total = 0;
            cell = new PdfPCell();
            foreach (var element in form.Element("expenses").Elements("expense"))
            {
                total += Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("est_unit_cost").Value);

                cell.Phrase = new Phrase(element.Element("quantity").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("unit").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(element.Element("description").Value);
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", element.Element("est_unit_cost").Value));
                table.AddCell(cell);

                cell.Phrase = new Phrase(string.Format("{0:N}", Convert.ToDouble(element.Element("quantity").Value) * Convert.ToDouble(element.Element("est_unit_cost").Value)));
                table.AddCell(cell);
            }

            cell = new PdfPCell();
            cell.Colspan = 4;
            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell.Phrase = new Phrase(new Chunk("TOTAL : ", boldFont));
            table.AddCell(cell);

            cell.Colspan = 1;
            cell.Phrase = new Phrase(string.Format("{0:N}", total));
            table.AddCell(cell);

            document.Add(table);
            document.Add(new Paragraph(""));

            document.Add(new Paragraph(""));

            table = new PdfPTable(3)
            {
                WidthPercentage = 100,
                SpacingBefore = 20f
            };

            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);
            //add the signature section

            //get the signature of the request
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "1");
            cell = new PdfPCell();
            cell.Colspan = 1;
            //get the signature of the requestor
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Requestor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the supervisor
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "2");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Supervisor Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the supervisor
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "4");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Compliance Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //get the signature of the finance
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "6");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            if (token != null)
            {
                cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            }
            else
            {
                cell.Phrase = new Phrase(new Chunk("", sign_font));
            }
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Program Director Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);


            //get the signature of the supervisor
            signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "7");
            token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));
            cell.Phrase = new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Country Director Signature"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Name and Title"));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Date"));
            table.AddCell(cell);

            //add the signature table to the document
            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        public void createRAuthorization(string filepath, string grant, XElement form, string header, admin_hrm_employee employee, DateTime created_at, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Reimbursement form");
            document.AddSubject("Reimbursement form");
            document.AddTitle("Reimbursement form");

            document.Open();

            //add watermark to the document
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            /*
            * Add date and PRF number
            */
            var side = new PdfDiv();
            side.Width = 210;
            side.Float = PdfDiv.FloatType.RIGHT;

            var topParagraph = new Paragraph
            {
                Alignment = Element.ALIGN_LEFT
            };
            var topPhrase = new Phrase();
            topPhrase.Add(new Chunk("Date: ", bodyFont));
            topPhrase.Add(new Chunk(DateTime.Parse(definition.Element("created_at").Value).ToString("d"), boldFont));
            topPhrase.Add(new Chunk("       ", bodyFont));
            topPhrase.Add(new Chunk("Number: ", bodyFont));
            topPhrase.Add(new Chunk(definition.Element("process_number").Value, subTitleFont));
            topParagraph.Add(topPhrase);
            side.Content.Add(topParagraph);

            document.Add(side);

            document.Add(new LineSeparator());

            /*
            * Add date and PRF number
            */
            side = new PdfDiv();
            side.Width = 210;
            //side.Float = PdfDiv.FloatType.RIGHT;

            topParagraph = new Paragraph
            {
                Alignment = Element.ALIGN_LEFT
            };
            topPhrase = new Phrase();
            topPhrase.Add(new Chunk("Staff Name: ", bodyFont));
            topPhrase.Add(new Chunk(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname), boldFont));
            topPhrase.Add(new Chunk("       ", bodyFont));
            topPhrase.Add(new Chunk("Grant/Class: ", bodyFont));
            topPhrase.Add(new Chunk(grant, subTitleFont));
            topParagraph.Add(topPhrase);
            side.Content.Add(topParagraph);
            side.PaddingBottom = 40f;
            document.Add(side);

            document.Add(new LineSeparator());

            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 2f, 5f });
            table.PaddingTop = 10f;

            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Purpose of Reimbursement", boldFont)));
            cell.PaddingTop = 30f;
            cell.PaddingBottom = 30f;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(form.Element("purpose_of_reimbursement").Value)));
            cell.PaddingTop = 30f;
            cell.PaddingBottom = 30f;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Approved By: (Signature indicates approval of program relevance as stated above.)")));
            cell.Colspan = 2;
            cell.Border = 0;
            table.AddCell(cell);

            //get the signature of the grants and compliance
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "2");
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));

            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);

            cell = new PdfPCell(new Phrase(new Chunk("Name:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Signature:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Title:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("")));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Date:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

        public void createAAuthorization(string filepath, string grant, XElement form, string header, admin_hrm_employee employee, DateTime created_at, XElement definition)
        {
            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }
            FileStream fs = new FileStream(filepath, FileMode.OpenOrCreate);

            // Create an instance of the document class which represents the PDF document itself.
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            // Create an instance to the PDF file by creating an instance of the PDF 
            // Writer class using the document and the filestrem in the constructor.
            PdfWriter writer = PdfWriter.GetInstance(document, fs);
            // Add meta information to the document
            document.AddAuthor("MGIC Workspace");
            document.AddCreator("Workspace");
            document.AddKeywords("Reimbursement form");
            document.AddSubject("Reimbursement form");
            document.AddTitle("Reimbursement form");

            document.Open();

            //add watermark to the document
            var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
            //add the document header
            Image jpg = Image.GetInstance(header);
            jpg.Alignment = Element.ALIGN_LEFT;
            jpg.ScaleToFit(480f, 120f);
            jpg.SpacingAfter = 10f;

            document.Add(jpg);

            /*
            * Add date and PRF number
            */
            var side = new PdfDiv();
            side.Width = 210;
            side.Float = PdfDiv.FloatType.RIGHT;

            var topParagraph = new Paragraph
            {
                Alignment = Element.ALIGN_LEFT
            };
            var topPhrase = new Phrase();
            topPhrase.Add(new Chunk("Date: ", bodyFont));
            topPhrase.Add(new Chunk(DateTime.Parse(definition.Element("created_at").Value).ToString("d"), boldFont));
            topPhrase.Add(new Chunk("       ", bodyFont));
            topPhrase.Add(new Chunk("Number: ", bodyFont));
            topPhrase.Add(new Chunk(definition.Element("process_number").Value, subTitleFont));
            topParagraph.Add(topPhrase);
            side.Content.Add(topParagraph);

            document.Add(side);

            document.Add(new LineSeparator());

            /*
            * Add date and PRF number
            */
            side = new PdfDiv();
            side.Width = 210;
            //side.Float = PdfDiv.FloatType.RIGHT;

            topParagraph = new Paragraph
            {
                Alignment = Element.ALIGN_LEFT
            };
            topPhrase = new Phrase();
            topPhrase.Add(new Chunk("Staff Name: ", bodyFont));
            topPhrase.Add(new Chunk(string.Format("{0} {1}", employee.emp_lastname, employee.emp_firstname), boldFont));
            topPhrase.Add(new Chunk("       ", bodyFont));
            topPhrase.Add(new Chunk("Grant/Class: ", bodyFont));
            topPhrase.Add(new Chunk(grant, subTitleFont));
            topParagraph.Add(topPhrase);
            side.Content.Add(topParagraph);
            side.PaddingBottom = 40f;
            document.Add(side);

            document.Add(new LineSeparator());

            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 2f, 5f });
            table.PaddingTop = 10f;

            PdfPCell cell = new PdfPCell(new Phrase(new Chunk("Purpose of Advance", boldFont)));
            cell.PaddingTop = 30f;
            cell.PaddingBottom = 30f;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(form.Element("purpose_of_advance").Value)));
            cell.PaddingTop = 30f;
            cell.PaddingBottom = 30f;
            cell.Border = 0;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Approved By: (Signature indicates approval of program relevance as stated above.)")));
            cell.Colspan = 2;
            cell.Border = 0;
            table.AddCell(cell);

            //get the signature of the grants and compliance
            var signature = form.Element("signatures").Elements("signature").FirstOrDefault(e => e.Element("step").Value.ToString() == "2");
            var token = _transactionTokenService.GetToken(Convert.ToInt32(signature.Element("signature_id").Value));

            var sign_font = FontFactory.GetFont(FontFactory.COURIER, 12);

            cell = new PdfPCell(new Phrase(new Chunk("Name:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("name").Value));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Signature:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk(Util.Decrypt(token.t_code).ToUpper(), sign_font)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Title:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("")));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(new Chunk("Date:", boldFont)));
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase(signature.Element("created_at").Value.ToString()));
            table.AddCell(cell);

            document.Add(table);

            // Close the document
            document.Close();
            // Close the writer instance
            writer.Close();
            // Always close open filehandles explicity
            fs.Close();

            // CreateWaterMarkInPDF(filepath, "~/Documents/Images/watermark.png");

            PdfStampInExistingFile(HttpContext.Current.Server.MapPath("~/Documents/Images/watermark.png"), filepath);
        }

    }
}