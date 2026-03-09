using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.UI;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class ReportesRentaParticular : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportesRentaParticular));
        private static IServicioCWRV servicioCotizador;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string cuspp = Request.QueryString["cuspp"];
                string solicitud = Request.QueryString["solicitud"];
                string formato = Request.QueryString["formato"];
                string formatoSolicitud = Request.QueryString["fs"];
                string version = Request.QueryString["v"];

                log.Info(string.Format("Inicio ReportesRentaParticular: solicitud[{0}], formato[{1}]", solicitud, formato));

                string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();

                string usuario = HttpContext.Current.Session["Usuario"].ToString();
                string token_generado = string.Empty;

                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                if (!string.IsNullOrEmpty(cuspp) && cuspp != "undefined")
                {
                    log.Debug(string.Format("Se va a actualizar la dirección. CUSPP[{0}] Solicitud[{1}] Usuario[{2}]", cuspp, solicitud, usuario));
                    Respuesta direccion = servicioCotizador.ActualizarDireccionSolicitud(cuspp, solicitud, usuario);
                }

                log.Info(string.Format("Se va a obtener el token. url[{0}]", urlToken));
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    dynamic parametros = new JObject();
                    parametros.usuario = usuario;
                    string json = parametros.ToString();

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var jsonResult = streamReader.ReadToEnd();
                    JObject jObject = JObject.Parse(jsonResult);
                    token_generado = (string)jObject["accessToken"];
                    log.Debug(string.Format("Token [{0}]", token_generado));
                }

                if (token_generado != "")
                {
                    string urlFormato = string.Empty;
                    string nombreArchivo = string.Empty;
                    if (string.IsNullOrEmpty(formatoSolicitud))
                    {
                        switch (formato)
                        {
                            case "1":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_solicitud_rpp"].ToString();
                                nombreArchivo = string.Format("Solicitud_{0}.pdf", solicitud);
                                break;
                            case "2":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_solicitud_ifp"].ToString();
                                nombreArchivo = string.Format("Solicitud_{0}.pdf", solicitud);
                                break;
                            case "3":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_origen_fondo"].ToString();
                                nombreArchivo = string.Format("OrigenFondos_{0}.pdf", solicitud);
                                break;
                            case "4":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_pep"].ToString();
                                nombreArchivo = string.Format("PEP_{0}.pdf", solicitud);
                                break;
                            case "5":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_constancia_abono"].ToString();
                                nombreArchivo = string.Format("ConstanciaAbono_{0}.pdf", solicitud);
                                break;
                            case "6":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_detalle_cotizacion"].ToString();
                                nombreArchivo = string.Format("DetalleCotizacion_{0}.pdf", solicitud);
                                break;
                            case "7":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_detalle_rescate"].ToString();
                                nombreArchivo = string.Format("DetalleRescate_{0}.pdf", solicitud);
                                break;
                            case "8":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_detalle_cotizacion_nuevo"].ToString();
                                nombreArchivo = string.Format("DetalleCotizacion_{0}.pdf", solicitud);
                                break;
                        }
                        urlFormato = string.Format(urlFormato, solicitud, usuario);
                    }
                    else
                    {
                        switch (formato)
                        {
                            case "1":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_solicitud_rpp_historico"].ToString();
                                nombreArchivo = string.Format("Solicitud_{0}_v{1}.pdf", solicitud, version);
                                break;
                            case "2":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_solicitud_ifp_historico"].ToString();
                                nombreArchivo = string.Format("Solicitud_{0}_v{1}.pdf", solicitud, version);
                                break;
                            case "3":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_origen_fondo_historico"].ToString();
                                nombreArchivo = string.Format("OrigenFondos_{0}_v{1}.pdf", solicitud, version);
                                break;
                            case "4":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_pep_historico"].ToString();
                                nombreArchivo = string.Format("PEP_{0}_v{1}.pdf", solicitud, version);
                                break;
                            case "5":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_constancia_abono_historico"].ToString();
                                nombreArchivo = string.Format("ConstanciaAbono_{0}_v{1}.pdf", solicitud, version);
                                break;
                            case "6":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_detalle_cotizacion"].ToString();
                                nombreArchivo = string.Format("DetalleCotizacion_{0}_v{1}.pdf", solicitud, version);
                                break;
                            case "7":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_detalle_rescate_historico"].ToString();
                                nombreArchivo = string.Format("DetalleRescate_{0}_v{1}.pdf", solicitud, version);
                                break;
                            case "8":
                                urlFormato = ConfigurationManager.AppSettings["url_formato_detalle_cotizacion_nuevo"].ToString();
                                nombreArchivo = string.Format("DetalleCotizacion_{0}_v{1}.pdf", solicitud, version);
                                break;
                        }
                        urlFormato = string.Format(urlFormato, solicitud, formatoSolicitud, usuario);
                    }

                    byte[] formatoBinario = Utilitarios.ConsumirServicio(urlFormato, usuario, token_generado);

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(formatoBinario);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", nombreArchivo));
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.End();
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                log.Error("Se ha producido un error en ReportesRentaParticular", ex);
            }
        }
    }
}