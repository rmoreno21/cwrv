using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Web;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class RecalculoCotizaciones : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportesRentaParticular));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string solicitud = Request.QueryString["s"];
                int correlativo = Convert.ToInt32(Request.QueryString["c"]);
                string usuario = Session["Usuario"].ToString();

                if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.RecalculoCotizaciones))
                {
                    string nombreArchivo = string.Format("Recalculo_{0}_{1}.pdf", solicitud, correlativo);

                    log.Debug(string.Format("Se va a generar el reporte de Recálculo de Cotizaciones para la Solicitud [{0}] Correlativo [{1}] Usuario[{2}]", solicitud, correlativo, usuario));

                    List<ReporteRecalculoCotizacion> reportes = new List<ReporteRecalculoCotizacion>();
                    ReporteRecalculoCotizacion reporte = new ReporteRecalculoCotizacion
                    {
                        num_solicitud = solicitud,
                        reporteRecalculoCotizacionDetalle = new ReporteRecalculoCotizacionDetalle
                        {
                            num_correlativo = correlativo
                        }
                    };
                    reportes.Add(reporte);

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    byte[] archivo = servicioCotizador.ObtenerReporteRecalculoPDF(reportes);

                    Response.ClearHeaders();
                    Response.Clear();
                    Response.AddHeader("Content-Type", "application/pdf");
                    Response.AddHeader("Content-Length", archivo.Length.ToString());
                    Response.AddHeader("Content-Disposition", string.Format("inline; filename={0}", nombreArchivo));

                    Response.BinaryWrite(archivo);
                    Response.Flush();
                    Response.End();
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    log.Warn(string.Format("El usuario [{0}] intentó acceder a una opción con la que no cuenta con privilegios [{1}]", usuario, Enums.OpcionesSistema.RecalculoCotizaciones));
                    Response.Redirect("~/Error/Permisos.aspx");
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                log.Error("Se ha producido un error al exportar el reporte de Recálculo de Cotizaciones", ex);
                Response.Redirect("~/Error/500.aspx");
            }
        }
    }
}