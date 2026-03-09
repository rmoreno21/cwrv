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
    public partial class DetalleAfiliado : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DetalleAfiliado));
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
                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudExportarPDF))
                    {
                            string wl_num_cuispp;
                            string wl_usuario;



                            wl_num_cuispp = HttpContext.Current.Session["CUSPP_IMPRIMIR"].ToString();
                            wl_usuario = HttpContext.Current.Session["Usuario"].ToString();//(string)Session["wl_xml_calculo"];
                         

                            ReportViewer visorReporte = new ReportViewer();
                            visorReporte.ProcessingMode = ProcessingMode.Remote;
                            visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);

                            visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteDetalleAfiliado"];



                            ReportParameter p1 = new ReportParameter("wl_num_cuispp", wl_num_cuispp);
                            ReportParameter p2 = new ReportParameter("wl_usuario", wl_usuario);


                            log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReportCalculoCapital"]));
                            log.Debug(String.Format("Parámetros del reporte: wl_num_cuispp[{0}] wl_usuario[{1}].",
                                wl_num_cuispp, wl_usuario));
                            visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2 });
                            log.Debug(String.Format("Reporte de afiliado CUSPP [{0}] , Usuario [{1}] procesado.", wl_num_cuispp, wl_usuario));

                            string mimeType, encoding, extension;
                            string[] streamids;
                            Warning[] warnings;

                            string format = "PDF";
                            log.Debug(String.Format("Se va a exportar a formato PDF el reporte de afiliado CUSPP [{0}] Usuario [{1}].", wl_num_cuispp,wl_usuario));
                            byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                            HttpContext.Current.Session["ArchivoPDF"] = bytes;
                            log.Debug(String.Format("Reporte de afiliado [{0}]  [{1}] exportado y almacenado en sesión de usuario.", wl_num_cuispp,wl_usuario));

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
                                IdTipoEvento = Enums.EventoLog.ExportarPDFAfiliado.StringValue(),
                                Detalle = String.Format("Reporte afiliado CUSPP {0} usuario {1} exportada a formato PDF", wl_num_cuispp, wl_usuario)
                            });

                            Response.ClearHeaders();
                            Response.Clear();
                            Response.AddHeader("Content-Type", "application/pdf");
                            Response.AddHeader("Content-Length", bytes.Length.ToString());
                            Response.AddHeader("Content-Disposition", "inline; filename=DetalleAfiliado.pdf");
                            //Response.AddHeader("Content-Disposition", "attachment; filename=DetalleCotizacion.pdf");

                            Response.BinaryWrite(bytes);
                            Response.Flush();
                            Response.End();
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                       
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.SolicitudExportarPDF.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));

                        if (ex.InnerException.InnerException != null)
                        {
                            log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.InnerException.Source, ex.InnerException.InnerException.Message, ex.InnerException.InnerException.StackTrace));
                        }
                    }
                }
            }
        }
    }
}