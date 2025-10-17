using System.Collections.Generic;
using System.IO;
using ip = iTextSharp.text.pdf;
using it = iTextSharp.text;

namespace CSF.CITASWEB.WS
{
    public class DocumentPDF
    {
		public static byte[] MergeFiles(List<byte[]> sourceFiles)
		{
			byte[] buffer;
			it.Document document = new it.Document();
			using (MemoryStream ms = new MemoryStream())
			{
				ip.PdfCopy copy = new ip.PdfCopy(document, ms);
				document.Open();
				int documentPageCounter = 0;
				for (int fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
				{
					ip.PdfReader reader = new ip.PdfReader(sourceFiles[fileCounter]);
					int numberOfPages = reader.NumberOfPages;
					for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
					{
						documentPageCounter++;
						ip.PdfImportedPage importedPage = copy.GetImportedPage(reader, currentPageIndex);
						ip.PdfCopy.PageStamp pageStamp = copy.CreatePageStamp(importedPage);
						pageStamp.AlterContents();
						copy.AddPage(importedPage);
					}
					copy.FreeReader(reader);
					reader.Close();
				}
				document.Close();
				buffer = ms.GetBuffer();
			}
			return buffer;
		}
	}
}