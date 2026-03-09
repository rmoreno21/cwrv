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
    public partial class DescargaSolicitudes : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DescargaSolicitudes));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                    }
                    //<SOLINI25621> Se comenta para que puede ser accedido desde solicitudes individual
                    //if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DescargaSolicitudes))
                    //{

                        string numeroLote;
                        //<SOLINI25621>
                        string numeroOperacion;
                        numeroOperacion = (string)Session["numeroOperacion"];

                        if (numeroOperacion == "") {
                            numeroOperacion = "-1";
                        }

                        //<SOLFIN25621>
                        numeroLote = (string)Session["numeroLote"];

                        ReportViewer visorReporte = new ReportViewer();
                        visorReporte.ProcessingMode = ProcessingMode.Remote;
                        visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                        visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteDescargaSolicitudes"];

                        ReportParameter p1 = new ReportParameter("wl_num_lote_cotizacion", numeroLote);
                        ReportParameter p2 = new ReportParameter("wl_num_operacion", numeroOperacion);

                        log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                ConfigurationManager.AppSettings["DominioReportingServices"],
                                ConfigurationManager.AppSettings["RutaReporteDescargaSolicitudes"]));
                        log.Debug(String.Format("Parámetros del reporte: wl_num_lote_cotizacion[{0}].", numeroLote));
                        //<SOLINI25621>
                        log.Debug(String.Format("Parámetros del reporte: wl_num_operacion[{0}].", numeroOperacion));
                        //<SOLFIN25621>
                        visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1 , p2});
                        log.Debug(String.Format("Reporte para lote de solicitudes N° [{0}] y Operacion N°[{1}] procesado.", numeroLote,numeroOperacion));

                        string mimeType, encoding, extension;
                        string[] streamids;
                        Warning[] warnings;

                        string format = "PDF";
                        log.Debug(String.Format("Se va a exportar a formato PDF el reporte para el lote de solicitudes N° [{0}] ú Operación N°[{1}].", numeroLote,numeroOperacion));
                        byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                        HttpContext.Current.Session["ArchivoPDF"] = bytes;
                        log.Debug(String.Format("Reporte para lote [{0}] exportado y almacenado en sesión de usuario.", numeroLote));

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
                            Detalle = String.Format("Lote de Solicitudes {0} exportado a formato PDF", numeroLote)
                        });

                        Response.ClearHeaders();
                        Response.Clear();
                        Response.AddHeader("Content-Type", "application/pdf");
                        Response.AddHeader("Content-Length", bytes.Length.ToString());
                        Response.AddHeader("Content-Disposition", "inline; filename=DescargaSolicitudes.pdf");

                        Response.BinaryWrite(bytes);
                        Response.Flush();
                        Response.End();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    //}
                    //else
                    //{
                    //    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                    //        Enums.OpcionesSistema.DescargaSolicitudes.StringValue()));
                    //    Response.Redirect("~/Error/Permisos.aspx");
                    //}
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