using DelightFoods_Live.Models.DTO;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Fonts;
using PdfSharpCore.Pdf;
using System.Text;
using System;
using System.IO;


namespace DelightFoods_Live.Utilites
{
	public class PdfGenerator
	{

		public string GenerateHtmlContent(SaleOrderDTO model)
		{
			// Replace placeholders in the HTML file with actual data
			string htmlContent = System.IO.File.ReadAllText("Views\\PDFTemplates\\SaleOrderPDF.html");
			htmlContent = htmlContent.Replace("[TotalPrice]", model.TotalPrice.ToString());
			htmlContent = htmlContent.Replace("[Status]", model.Status);
			htmlContent = htmlContent.Replace("[CreatedOnUTC]", model.CreatedOnUTC.ToString());

			// Generate rows for product details
			string productRows = "";
			foreach (var item in model.saleOrderProductMappings)
			{
				productRows += "<tr>";
				productRows += $"<td>{item.ProductName}</td>";
				productRows += $"<td>{item.Price}</td>";
				productRows += $"<td>{item.Quantity}</td>";
				productRows += "</tr>";
			}

			htmlContent = htmlContent.Replace("[ProductRows]", productRows);

			return htmlContent;
		}

		public string GeneratePlainTextContent(SaleOrderDTO model)
		{
			// Initialize a StringBuilder to construct the plain text content
			StringBuilder plainTextContent = new StringBuilder();

			// Append the sale order details
			plainTextContent.AppendLine("SaleOrderModel");
			plainTextContent.AppendLine("---------------------");
			plainTextContent.AppendLine($"Customer Name: {model.CustomerName}");
			plainTextContent.AppendLine($"Status: {model.Status}");
			plainTextContent.AppendLine($"Total Price: {model.TotalPrice}");
			plainTextContent.AppendLine($"Created On UTC: {model.CreatedOnUTC.Date}");

			// Append the product details
			plainTextContent.AppendLine();
			plainTextContent.AppendLine("Product Details");
			plainTextContent.AppendLine("---------------------");

			foreach (var item in model.saleOrderProductMappings)
			{
				plainTextContent.AppendLine($"Product Name: {item.ProductName}");
				plainTextContent.AppendLine($"Price: {item.Price}");
				plainTextContent.AppendLine($"Quantity: {item.Quantity}");
				plainTextContent.AppendLine();
			}

			// Return the plain text content
			return plainTextContent.ToString();
		}



		public void ConvertHtmlToPdf(string htmlFilePath, string pdfOutputPath)
		{
			string htmlContent = File.ReadAllText(htmlFilePath);

			// Create a new PDF document
			PdfDocument document = new PdfDocument();

			// Add a new page to the document
			PdfPage page = document.AddPage();

			// Create a graphics object from the PDF page
			XGraphics gfx = XGraphics.FromPdfPage(page);

			// Create a font for rendering text
			XFont font = new XFont("Arial", 12);
			// Create a rectangle for the PDF page
			XRect layoutRectangle = new XRect(10, 10, page.Width - 20, page.Height - 20);

			// Create a text formatter
			XTextFormatter tf = new XTextFormatter(gfx);

			// Render the HTML content onto the PDF page
			tf.DrawString(htmlContent, font, XBrushes.Black, layoutRectangle, XStringFormats.TopLeft);

			// Save the PDF document to the output path
			document.Save(pdfOutputPath);
		}


  //      public void GeneratePdf(string htmlContent, string pdfFilePath, SaleOrderDTO model)
  //      {
  //          // Replace dynamic data fields in the HTML content with actual values
  //          htmlContent = htmlContent.Replace("@Model.TotalPrice", model.TotalPrice.ToString())
  //                                   .Replace("@Model.Status", model.Status)
  //                                   .Replace("@Model.CreatedOnUTC", model.CreatedOnUTC.ToString());

  //          // Add dynamic data fields for the table
  //          var tableRows = new StringBuilder();
  //          foreach (var item in model.saleOrderProductMappings)
  //          {
  //              tableRows.Append($"<tr><td>{item.ProductName}</td><td>{item.Price}</td><td>{item.Quantity}</td></tr>");
  //          }
  //          htmlContent = htmlContent.Replace("@Model.TableRows", tableRows.ToString());

		//	// Create a new PDF document
		//	using (var document = new PdfDocument())
		//	{
		//		// Add a page to the document
		//		var page = document.AddPage();
		//		var gfx = XGraphics.FromPdfPage(page);

		//		// Create a PDFSharp formatter
		//		var renderer = new HtmlRenderer.HtmlRenderer();
		//		var converter = new HtmlRenderer.PdfSharp.PdfGenerator();

		//		// Render HTML content
		//		converter.RenderHtmlAsPdf(htmlContent, document);

		//		// Save the document
		//		document.Save(pdfFilePath);
		//	}
		//}
    }
}
