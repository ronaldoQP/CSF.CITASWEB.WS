using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using io = System.IO;

namespace CSF.CITASWEB.WS
{
    public class HtmlToPdf
    {
		public static byte[] ObtenerBuffer(string contenidoHTML, HttpServerUtility oServer)
		{
			byte[] buffer = null;
			try
			{
				var htmlToPdf = new NReco.PdfGenerator.HtmlToPdfConverter();
				htmlToPdf.Size = NReco.PdfGenerator.PageSize.A4;
				htmlToPdf.Zoom = 1;
				string html = "<html><head><style>html, body{background-color:white !important;} *{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;}*:before,*:after{-webkit-box-sizing:border-box;-moz-box-sizing:border-box;box-sizing:border-box;}";
				html += io.File.ReadAllText(oServer.MapPath("~/Resources/Styles/01_Base/03_Align.css"));
				html += io.File.ReadAllText(oServer.MapPath("~/Resources/Styles/01_Base/05_Columns.css"));
				html += io.File.ReadAllText(oServer.MapPath("~/Resources/Styles/01_Base/06_Forms.css"));
				html += io.File.ReadAllText(oServer.MapPath("~/Resources/Styles/01_Base/08_Panel.css"));
				html += io.File.ReadAllText(oServer.MapPath("~/Resources/Styles/02_Directives/Table.css"));
				html += " div.form-group {margin-bottom: 10px !important;}</style></head>";
				html += "<body><div id='divContenedor' class='panel-body form-horizontal' style='position:relative;font-family:Arial !important;font-size:16px;'>";
				html += HttpUtility.HtmlEncode(contenidoHTML).Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&nbsp;", " ").Replace("&#39;", "\"");
				html += "</div></body></html>";
				html = html.Replace("&amp;nbsp;", "<span>&nbsp;</span>");
				buffer = htmlToPdf.GeneratePdf(html);
			}
			catch (Exception e)
			{

			}
			return buffer;
		}
	}
}