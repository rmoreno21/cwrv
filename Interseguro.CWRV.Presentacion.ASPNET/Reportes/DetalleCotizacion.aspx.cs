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
    public partial class DetalleCotizacion : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DetalleCotizacion));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                    }
                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudExportarPDF))
                    {
                        if (Utilitarios.EsRolVerAgentesCesados((string)Session["RolAzman"]) || ((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["AgenteReporte"]))
                        {
                            string idSolicitud, fecCotizacion;
                            bool consentimiento;

                            idSolicitud = (string)Session["idSolicitud"];
                            fecCotizacion = (string)Session["fecCotizacion"];
                            consentimiento = true;
                            string num_lote = "1";
                            string grupo1 = "GRUPO1";
                            string grupo2 = "GRUPO2";
                            string id_benefi = "1";

                            ReportViewer visorReporte = new ReportViewer();
                            visorReporte.ProcessingMode = ProcessingMode.Remote;
                            visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                            if (consentimiento)
                            {
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteDetalleCotizacion"];
                            }
                            else
                            {
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteDetalleCotizacionProtegido"];
                            }

                            ReportParameter p1 = new ReportParameter("wl_solicitud", idSolicitud);
                            ReportParameter p2 = new ReportParameter("wl_fec_cotizacion", fecCotizacion);
                            ReportParameter p3 = new ReportParameter("wl_num_lote", num_lote);
                            ReportParameter p4 = new ReportParameter("wl_grupo", grupo1);
                            ReportParameter p5 = new ReportParameter("wl_grupo2", grupo2);
                            ReportParameter p6 = new ReportParameter("wl_id_benefi", id_benefi);

                            log.Info(string.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteDetalleCotizacion"]));
                            log.Debug(string.Format("Parámetros del reporte: wl_solicitud[{0}] wl_fec_cotizacion[{1}] wl_num_lote[{2}] wl_grupo1[{3}] wl_grupo2[{4}] wl_id_benefi[{5}].",
                                idSolicitud, fecCotizacion, num_lote, grupo1, grupo2, id_benefi));
                            visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6 });
                            log.Debug(string.Format("Reporte para solicitud [{0}] procesado.", idSolicitud));

                            string mimeType, encoding, extension;
                            string[] streamids;
                            Warning[] warnings;

                            string format = "PDF";
                            log.Debug(string.Format("Se va a exportar a formato PDF el reporte para solicitud [{0}].", idSolicitud));
                            byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                            HttpContext.Current.Session["ArchivoPDF"] = bytes;
                            log.Debug(string.Format("Reporte para solicitud [{0}] exportado y almacenado en sesión de usuario.", idSolicitud));

                            string nombreTerminal = string.Empty;
                            try
                            {
                                nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                            }
                            catch (Exception)
                            {
                                log.Warn(string.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
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
                                Detalle = string.Format("Solicitud {0} exportada a formato PDF", idSolicitud)
                            });

                            Response.ClearHeaders();
                            Response.Clear();
                            Response.AddHeader("Content-Type", "application/pdf");
                            Response.AddHeader("Content-Length", bytes.Length.ToString());
                            Response.AddHeader("Content-Disposition", "inline; filename=DetalleCotizacion.pdf");
                            //Response.AddHeader("Content-Disposition", "attachment; filename=DetalleCotizacion.pdf");

                            Response.BinaryWrite(bytes);
                            Response.Flush();
                            Response.End();
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a la información de una solicitud a la que no tiene privilegios [{0}].",
                                (string)Session["idSolicitud"]));
                            Response.Redirect("~/Error/Permisos.aspx");
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.SolicitudExportarPDF.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                }
            }
        }
    }
}