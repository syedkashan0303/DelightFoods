using DelightFoods_Live.Models.DTO;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Fonts;
using PdfSharp.Pdf;


namespace DelightFoods_Live.Utilites
{
	public class PdfGenerator
	{

		public string GenerateHtmlContent(SaleOrderDTO model)
		{
			// Replace placeholders in the HTML file with actual data
			string htmlContent = System.IO.File.ReadAllText("Views\\PDFTemplates\\SaleOrderPDF.cshtml");
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

			// Create a rectangle for the PDF page
			XRect layoutRectangle = new XRect(10, 10, page.Width - 20, page.Height - 20);

			// Create a text formatter
			XTextFormatter tf = new XTextFormatter(gfx);

			// Render the HTML content onto the PDF page
			tf.DrawString(htmlContent,new XFont("Arial", 10), XBrushes.Black, layoutRectangle, XStringFormats.TopLeft);

			// Save the PDF document to the output path
			document.Save(pdfOutputPath);
		}

	}

	public class ArialFontResolver : IFontResolver
	{
		public byte[] GetFont(string faceName)
		{
			if (faceName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
			{
				string fontFilePath = "path/to/arial.ttf"; // Replace with the actual path to Arial font file
				if (File.Exists(fontFilePath))
				{
					return File.ReadAllBytes(fontFilePath);
				}
				else
				{
					throw new FileNotFoundException($"Arial font file not found at: {fontFilePath}");
				}
			}
			return null;
		}

		public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
		{
			if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
			{
				return new FontResolverInfo("Arial");
			}
			return null;
		}
	}

}
