using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using InfojectiveReports;

namespace HayleyReports
{
    /// <summary>
    /// Summary description for Report
    /// </summary>
    public class Report : IHttpHandler
    {

        private HttpContext _context;

        public void ProcessRequest(HttpContext context)
        {
            //context.Response.ContentType = "text/plain";
            //context.Response.Write("Hello World");
            _context = context;
            string string1 = _context.Request.QueryString["file"];
            string path = HttpContext.Current.Server.MapPath("~/DesktopModules/HayleyReports/Reports/");
            path += string1;
            
            //Dictionary<string, string> myDic = new Dictionary<string, string>();
            List<IParameter> _parameters = new List<IParameter>();
            //_parameters = ReportParameters.GetParameters(path);

            string[] sqlParams = Array.FindAll(_context.Request.QueryString.AllKeys, x => x.Contains("@"));

            foreach (string param in sqlParams)
            {
                IParameter newParam = new InfojectiveReports.Parameter();
                newParam.Name = param;
                newParam.Value = _context.Request.QueryString[param];
                _parameters.Add(newParam);
                //myDic.Add(param, _context.Request.QueryString[param]);
            }
            
            //myDic.Add("@id", "1161");
            
            IReportController reportController = new ReportController();
            IReport myReport = new ReportPreProcessor().PreProcess(path,_parameters);
            SendOutPDF(reportController.CreatePDF(myReport,_parameters), "PDF Title");
            
        }

        protected void SendOutPDF(System.IO.MemoryStream PDFData, string title)
        {
            // Clear response content & headers

            _context.Response.Clear();
            _context.Response.ClearContent();
            _context.Response.ClearHeaders();
            _context.Response.ContentType = "application/pdf";
            _context.Response.Charset = string.Empty;
            _context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);

            _context.Response.AddHeader("Content-Disposition",
               "attachment; filename=" + title.Replace(" ", "").Replace(":", "-") + ".pdf");


            _context.Response.OutputStream.Write(PDFData.GetBuffer(), 0, PDFData.GetBuffer().Length);
            _context.Response.OutputStream.Flush();
            _context.Response.OutputStream.Close();

            _context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}