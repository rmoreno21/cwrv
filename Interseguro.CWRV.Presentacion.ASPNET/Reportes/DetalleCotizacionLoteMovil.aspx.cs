using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Reporting.WebForms;
using System.Collections.Specialized;
using System.Configuration;
using log4net;
using System.ServiceModel;
using System.Reflection;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using System.Threading;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class DetalleCotizacionLoteMovil : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DetalleCotizacionLoteMovil));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                    }
                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CotizarLote))
                    {
                        string idSolicitud, fecCotizacion;

                        idSolicitud = (string)Session["idSolicitud"];
                        fecCotizacion = (string)Session["fecCotizacion"];
                        Session["idSolicitud"] = null;
                        Session["fecCotizacion"] = null;
                        string num_lote = "1";
                        string grupo1 = "GRUPO1";
                        string grupo2 = "GRUPO2";
                        string id_benefi = "1";

                        ReportViewer visorReporte = new ReportViewer();
                        visorReporte.ProcessingMode = ProcessingMode.Remote;
                        visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                        visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteDetalleCotizacionLote"];

                        ReportParameter p1 = new ReportParameter("wl_solicitud", idSolicitud);
                        ReportParameter p2 = new ReportParameter("wl_fec_cotizacion", fecCotizacion);
                        ReportParameter p3 = new ReportParameter("wl_num_lote", num_lote);
                        ReportParameter p4 = new ReportParameter("wl_grupo", grupo1);
                        ReportParameter p5 = new ReportParameter("wl_grupo2", grupo2);
                        ReportParameter p6 = new ReportParameter("wl_id_benefi", id_benefi);

                        log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                ConfigurationManager.AppSettings["DominioReportingServices"],
                                ConfigurationManager.AppSettings["RutaReporteDetalleCotizacionLote"]));
                        log.Debug(String.Format("Parámetros del reporte: wl_solicitud[{0}] wl_fec_cotizacion[{1}] wl_num_lote[{2}] wl_grupo1[{3}] wl_grupo2[{4}] wl_id_benefi[{5}].",
                            idSolicitud, fecCotizacion, num_lote, grupo1, grupo2, id_benefi));
                        visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6 });
                        log.Debug(String.Format("Reporte para solicitudes [{0}] procesado.", idSolicitud));

                        string mimeType, encoding, extension;
                        string[] streamids;
                        Warning[] warnings;

                        string format = "PDF";
                        log.Debug(String.Format("Se va a exportar a formato PDF el reporte para solicitudes [{0}].", idSolicitud));
                        byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                        HttpContext.Current.Session["ArchivoPDF"] = bytes;
                        log.Debug(String.Format("Reporte para solicitud [{0}] exportado y almacenado en sesión de usuario.", idSolicitud));

                        string nombreTerminal = String.Empty;
                        try
                        {
                            nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                        }
                        catch (Exception)
                        {
                            log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                HttpContext.Current.Request.ServerVariables["remote_addr"]));
                        }

                        nombreTerminal += HttpContext.Current.Request.UserAgent;

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        servicioCotizador.RegistrarLog(new LogBD
                        {
                            IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                            NombreTerminal = nombreTerminal,
                            IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                            NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                            IdTipoEvento = Enums.EventoLog.ExportarPDFSolicitud.StringValue(),
                            Detalle = String.Format("Solicitudes {0} exportadas a formato PDF", idSolicitud)
                        });

                        Response.Clear();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        string filename = "DetalleCotizacionLote.pdf";
                        Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                        Response.Charset = "";
                        Response.ContentType = "application/octet-stream";

                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.End();
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.CotizarLote.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                }
            }
        }
    }
}