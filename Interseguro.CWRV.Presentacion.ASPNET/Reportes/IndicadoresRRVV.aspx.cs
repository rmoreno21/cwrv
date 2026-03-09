using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using Interseguro.CWRV.Infraestructura.General;
using System.Threading;
using System.ServiceModel;
using System.Configuration;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Dominio.Entidades;
using System.Web.Services;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class IndicadoresRRVV : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IndicadoresRRVV));
        private static IServicioCWRV servicioCotizador;
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteIndicadoresRRVV))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));

                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ReporteIndicadoresRRVV.StringValue()));
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
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }

        //[WebMethod]
        //public static Respuesta GenerarReporteIndicadoresRRVV(string tokenUsuario, string cuspp, int num_maximo, string[] solicitudes, string[] cotizaciones)
        //{

        //}

        protected void BtnGenerarExcel_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteIndicadoresRRVV))
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<ReporteIndicadoresRRVV> lstReporteIndicadoresRRVV = servicioCotizador.ListarReporteIndicadoresRRVV();

                        List<object> lstReporteObject = new List<object>();
                        foreach (var item in lstReporteIndicadoresRRVV)
                        {
                            lstReporteObject.Add(item);
                        }

                        //Generar archivo Excel
                        string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                        string urlServicio = ConfigurationManager.AppSettings["url_reporte_generar_excel"].ToString();
                        string urlRutaReportePlantilla = ConfigurationManager.AppSettings["ruta_Reporte_Plantilla_indicadoresRRVV"].ToString();
                        string urlRutaReporteGenerado = ConfigurationManager.AppSettings["ruta_Reporte_Generado_indicadoresRRVV"].ToString();
                        string usuario = HttpContext.Current.Session["Usuario"].ToString();
                        string token_generado = string.Empty;

                        log.Info("Consumiendo servicio token: " + urlToken);
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";

                        log.Info("Pasando el json al servicio");
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string json = "{\"usuario\": \"" + usuario + "\"}";
                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        log.Info("Leyendo el servicio");
                        var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            var jsonResult = streamReader.ReadToEnd();
                            JObject jObject = JObject.Parse(jsonResult);
                            token_generado = (string)jObject["accessToken"];
                        }

                        if (token_generado != "")
                        {



                            ////var client = new WebClient();
                            ////var method = "POST"; // If your endpoint expects a GET then do it.
                            ////var parameters = new System.Collections.Specialized.NameValueCollection();

                            ////parameters.Add("rutaPlantilla", urlRutaReporte);
                            ////parameters.Add("celdaEscritura", "A2");
                            ////parameters.Add("nombreArchivo", "Indicadores de RRVV");
                            ////parameters.Add("listaDatos", "");

                            ////client.Headers.Add("Content-Type", "application/json");

                            /////* Always returns a byte[] array data as a response. */
                            ////var response_data = client.UploadValues(urlServicio, method, parameters);

                            ////// Parse the returned data (if any) if needed.
                            ////var responseString = UnicodeEncoding.UTF8.GetString(response_data);



                            log.Info("Consumiendo servicio: " + urlServicio);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServicio);
                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "POST";
                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                            log.Info("Pasando el json al servicio");
                            ReporteIndicadores reporteIndicadores = new ReporteIndicadores()
                            {
                                rutaPlantilla = urlRutaReportePlantilla,
                                rutaGenerar = urlRutaReporteGenerado,
                                celdaEscritura = "A2",
                                listaDatos = lstReporteObject
                            };
                            string json = JsonConvert.SerializeObject(reporteIndicadores);

                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            {
                                streamWriter.Write(json);
                                streamWriter.Flush();
                                streamWriter.Close();
                            }

                            log.Info("Leyendo el servicio");
                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {

                                var jsonResult = streamReader.ReadToEnd();

                                //if (jsonResult.Length > 0)
                                //{
                                //byte[] resultado = streamReader.CurrentEncoding.GetBytes(jsonResult);

                                byte[] array = File.ReadAllBytes(urlRutaReporteGenerado);

                                //eliminar archivo
                                File.Delete(urlRutaReporteGenerado);

                                Response.Clear();
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("Content-disposition", "filename=Indicadores Comercial.xlsx");
                                //Response.OutputStream.Write(array, 0, array.Length);
                                //Response.OutputStream.Flush();
                                //Response.OutputStream.Close();
                                Response.BinaryWrite(array);
                                Response.Flush();
                                Response.End();
                                HttpContext.Current.ApplicationInstance.CompleteRequest();

                                //Response.Close();

                                //}
                            }

                        }

                        respuesta.Estado = Constante.COD_OK;
                    }

                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                }
                catch (ThreadAbortException) { }
                catch (FaultException ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "No se ha podido completar el proceso debido al siguiente error:" + ex.Message });
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

            }
        }
    }
}