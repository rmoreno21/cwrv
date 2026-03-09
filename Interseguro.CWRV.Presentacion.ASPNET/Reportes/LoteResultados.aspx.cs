using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class LoteResultados : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportesRentaParticular));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                int lote = Convert.ToInt32(Request.QueryString["l"]);
                string usuario = Session["Usuario"].ToString();

                if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DescargaResultados))
                {
                    string nombreArchivo = string.Format("Resultados_{0}.pdf", lote);

                    log.Debug(string.Format("Se va a generar el reporte de Descarga de Resultados MELER para el Lote [{0}] Usuario [{1}]", lote, usuario));

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    byte[] archivo = servicioCotizador.ObtenerReporteCotizacionesGanadasPDF(lote);

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
                    log.Warn(string.Format("El usuario [{0}] intentó acceder a una opción con la que no cuenta con privilegios [{1}]", usuario, Enums.OpcionesSistema.DescargaResultados));
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