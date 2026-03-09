using Interseguro.CWRV.Infraestructura.General;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Dominio.Entidades;
using System.Globalization;
using System.Web.Services;
using System.Configuration;
using System.Data;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
{

    public partial class ListadoArchivoDrive : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ListadoCierrePlus));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ListadoCotizacionesEvaluacion))
                    {
                        if (!IsPostBack)
                        {

                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));

                            string usuario = (string)HttpContext.Current.Session["Usuario"].ToString();

                            HNroSolicitud.Value = HttpContext.Current.Session["solicitudEvaluacion"].ToString();
                            Hcuspp.Value = HttpContext.Current.Session["cuspp_rpp"].ToString();
                            Hnombre.Value = HttpContext.Current.Session["nombre_rpp"].ToString();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(Session["solicitudEvaluacion"].ToString());

                            HEstadoPlaft.Value = solicitud.CodigoEstadoPlaft.ToString();
                            HEstado.Value = solicitud.CodigoEstado.ToString();

                            HDescEstPlaft.Value = solicitud.EstadoSolicitudPlaft.ToString();
                            HDescEstOpe.Value = solicitud.EstadoSolicitud.ToString();

                            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.AprobarFlujoSolicitud))
                            {
                                HAprobarFlujoSolicitud.Value = "1";
                            }
                            else { HAprobarFlujoSolicitud.Value = "0"; }

                            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ObservarFlujoSolicitud))
                            {
                                HObservarFlujoSolicitud.Value = "1";
                            }
                            else { HObservarFlujoSolicitud.Value = "0"; }

                            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.RechazarFlujoSolicitud))
                            {
                                HRechazarFlujoSolicitud.Value = "1";
                            }
                            else { HRechazarFlujoSolicitud.Value = "0"; }

                            //<INI.GTI_26697>
                            DataTable tabla = new DataTable("TabDocumentos");
                            tabla.Columns.Add(new DataColumn("Id", typeof(string)));
                            tabla.Columns.Add(new DataColumn("Opcion", typeof(string)));
                            tabla.Columns.Add(new DataColumn("FechaSolicitud", typeof(string)));
                            tabla.Columns.Add(new DataColumn("Documento", typeof(string)));

                            DataRow row = tabla.NewRow();
                            row["Id"] = solicitud.Id;
                            row["Opcion"] = 1;
                            row["FechaSolicitud"] = Convert.ToDateTime(Session["fecCotizacion"], new CultureInfo("es-PE"));
                            row["Documento"] = "DETALLE DE COTIZACIÓN";
                            tabla.Rows.Add(row);

                            //indicador de rescate 
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            int ind_rescate = servicioCotizador.ObtenerIndicadorRescateIFP(solicitud.Id, usuario);

                            if (ind_rescate > 0)
                            {
                                row = tabla.NewRow();
                                row["Id"] = solicitud.Id;
                                row["Opcion"] = 6;
                                row["FechaSolicitud"] = Convert.ToDateTime(Session["fecCotizacion"], new CultureInfo("es-PE"));
                                row["Documento"] = "DETALLE DE RESCATE";
                                tabla.Rows.Add(row);
                            }

                            row = tabla.NewRow();
                            row["Id"] = solicitud.Id;
                            row["Opcion"] = 2;
                            row["FechaSolicitud"] = Convert.ToDateTime(Session["fecCotizacion"], new CultureInfo("es-PE"));
                            if (solicitud.TipoCotizacion.Id.ToString() == "IFP")
                                row["Documento"] = "SOLICITUD IFP";
                            else
                                row["Documento"] = "SOLICITUD RPP";

                            tabla.Rows.Add(row);

                            row = tabla.NewRow();
                            row["Id"] = solicitud.Id;
                            row["Opcion"] = 3;
                            row["FechaSolicitud"] = Convert.ToDateTime(Session["fecCotizacion"], new CultureInfo("es-PE"));
                            row["Documento"] = "FORMATO DE ORIGEN DE FONDOS";
                            tabla.Rows.Add(row);

                            if (solicitud.Beneficiarios.Where(x => x.Parentesco.Id == "80").FirstOrDefault().ind_PEP)
                            {
                                row = tabla.NewRow();
                                row["Id"] = solicitud.Id;
                                row["Opcion"] = 4;
                                row["FechaSolicitud"] = Convert.ToDateTime(Session["fecCotizacion"], new CultureInfo("es-PE"));
                                row["Documento"] = "FORMATO PEP";
                                tabla.Rows.Add(row);
                            }

                            row = tabla.NewRow();
                            row["Id"] = solicitud.Id;
                            row["Opcion"] = 5;
                            row["FechaSolicitud"] = Convert.ToDateTime(Session["fecCotizacion"], new CultureInfo("es-PE"));
                            row["Documento"] = "FORMATO DE CONSTANCIA DE ABONO";
                            tabla.Rows.Add(row);

                            TabDocumentos.DataSource = tabla;
                            TabDocumentos.DataBind();
                            //<FIN.GTI_26697>
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ListadoCotizacionesEvaluacion.StringValue()));
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

        [WebMethod]
        public static Respuesta flujoSolicitud(string tokenUsuario, string numSolicitud, string observacion, string codEstadoRPP)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio ListadoArchivoDrive.flujoSolicitud WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        string usuario = (string)HttpContext.Current.Session["Usuario"];
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        string url = string.Empty;
                        respuesta = servicioCotizador.ActualizarSolicitudOperaciones(numSolicitud, Convert.ToInt32(codEstadoRPP), observacion, usuario);
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    log.Debug("Fin ListadoArchivoDrive.flujoSolicitud WebMethod");
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    log.Debug("Fin ListadoArchivoDrive.flujoSolicitud WebMethod");
                    return respuesta;
                }
            }


        }

        //<INI.GTI_26697>
        [WebMethod]
        public static Respuesta DescargarFormato(string numSolicitud, int opcionPDF, string fecCotizacion)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                log.Debug("Inicio ListadoArchivoDrive.ObtenerPDF");

                if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ListadoCotizacionesEvaluacion))
                {
                    string usuario = (string)HttpContext.Current.Session["Usuario"].ToString();

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    //SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(numSolicitud, Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE")));
                    SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(numSolicitud);

                    log.Info("Accediendo a las Key necesarias");

                    string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                    string urlFormato = "";
                    string nombreFormato = "";

                    switch (opcionPDF)
                    {
                        case 1:
                            //Falta implementar en api
                            urlFormato = ConfigurationManager.AppSettings["url_formato_detalle_cotizacion"].ToString();
                            nombreFormato = string.Format("DetalleCotizacion_{0}.pdf", numSolicitud);

                            respuesta = ArmarServicio(urlToken, urlFormato, numSolicitud, nombreFormato);
                            break;
                        case 2:
                            nombreFormato = string.Format("Solicitud_{0}.pdf", numSolicitud);
                            if (solicitud.TipoCotizacion.Id.ToString() == "IFP")
                            {
                            
                                urlFormato = ConfigurationManager.AppSettings["url_formato_solicitud_ifp"].ToString();

                                respuesta = ArmarServicio(urlToken, urlFormato, numSolicitud, nombreFormato);
                            }
                            else
                            {
                                urlFormato = ConfigurationManager.AppSettings["url_formato_solicitud_rpp"].ToString();

                                respuesta = ArmarServicio(urlToken, urlFormato, numSolicitud, nombreFormato);
                            }
                            break;
                        case 3:
                            urlFormato = ConfigurationManager.AppSettings["url_formato_origen_fondo"].ToString();
                            nombreFormato = string.Format("OrigenFondos_{0}.pdf", numSolicitud);

                            respuesta = ArmarServicio(urlToken, urlFormato, numSolicitud, nombreFormato);
                            break;
                        case 4:
                            urlFormato = ConfigurationManager.AppSettings["url_formato_pep"].ToString();
                            nombreFormato = string.Format("PEP_{0}.pdf", numSolicitud);

                            respuesta = ArmarServicio(urlToken, urlFormato, numSolicitud, nombreFormato);
                            break;
                        case 5:
                            urlFormato = ConfigurationManager.AppSettings["url_formato_constancia_abono"].ToString();
                            nombreFormato = string.Format("ConstanciaAbono_{0}.pdf", numSolicitud);

                            respuesta = ArmarServicio(urlToken, urlFormato, numSolicitud, nombreFormato);
                            break;
                        case 6:
                            urlFormato = ConfigurationManager.AppSettings["url_formato_detalle_rescate"].ToString();
                            nombreFormato = string.Format("DetalleRescate_{0}.pdf", numSolicitud);

                            respuesta = ArmarServicio(urlToken, urlFormato, numSolicitud, nombreFormato);
                            break;
                    }
                    
                }
                else
                {
                    log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                    respuesta.Estado = Constante.COD_TOKEN;
                }

                log.Debug("Fin ListadoArchivoDrive.ObtenerPDF");
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }

            return respuesta;
        }

        private static Respuesta ArmarServicio(string urlToken, string urlFormato, string numSolicitud, string nombreFormato)
        {
            Respuesta respuesta = new Respuesta();
            string usuario = (string)HttpContext.Current.Session["Usuario"].ToString();
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
                urlFormato = string.Format(urlFormato, numSolicitud, usuario);

                log.Info("Consumiendo servicio solicitud: " + urlFormato);

                byte[] formatoArray = ConsumirServicio(urlFormato, usuario, token_generado);

                File.WriteAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\Plantilla\\IFP\\" + nombreFormato, formatoArray);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = nombreFormato;
            }
            else
            {
                log.Debug("No se genero el archivo");
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso</strong></div>";
            }

            return respuesta;
        }

        private static byte[] ConsumirServicio(string url, string usuario, string token_generado)
        {
            log.Info("Leyendo el servicio: " + url);
            WebClient myWebClient = new WebClient();
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token_generado));
            myWebClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            byte[] formatoByteArray = myWebClient.DownloadData(url);
            myWebClient.Dispose();
            return formatoByteArray;
        }

        //<FIN.GTI_26697>

    }

}