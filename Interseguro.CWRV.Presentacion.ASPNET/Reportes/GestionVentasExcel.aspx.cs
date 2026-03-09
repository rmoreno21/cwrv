using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
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
    public partial class GestionVentasExcel : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GestionVentasExcel));
        private static IServicioCWRV servicioCotizador;
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    byte[] excelBytes = (byte[])Session["ExcelGestionVentasBytes"];
                    if (excelBytes == null || excelBytes.Length == 0)
                    {
                        log.Error("No se encontró el archivo Excel en la sesión o está vacío");
                    }
                    string nombreArchivo = String.Format("GestionVentas-{0}.xls", DateTime.Now.ToString("dd-MM-yyyy"));
                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", nombreArchivo));
                    Response.Clear();
                    Response.BinaryWrite(excelBytes);
                    Response.End();
                    Session.Remove("ExcelGestionVentasBytes");
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