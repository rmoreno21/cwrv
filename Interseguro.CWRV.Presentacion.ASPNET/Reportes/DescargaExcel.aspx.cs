using log4net;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class DescargaExcel : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DescargaExcel));
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    HSSFWorkbook wb;
                    wb = (HSSFWorkbook)Session["ArchivoExcel"];

                    string Archivo = (string)Session["NombreArchivoExcel"];

                    using (var exportData = new MemoryStream())
                    {
                        wb.Write(exportData);
                        //string nombreArchivo = String.Format("PensionProyectada-{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
                        string nombreArchivo = String.Format( Archivo + "-{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
                        //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.ContentType = "application/vnd.ms-excel";
                        Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", nombreArchivo));
                        Response.Clear();
                        Response.BinaryWrite(exportData.GetBuffer());
                        Response.End();
                    }
                }
                catch (ThreadAbortException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}: {1}]", ex.Source, ex.Message), ex);
                }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}: {1}]", ex.Source, ex.Message), ex);
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]", ex.Source, ex.Message, ex.StackTrace), ex);
                }
            }
        }
    }
}