using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Comun
{
    public partial class PlantillaCorreoElectronico : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PlantillaCorreoElectronico));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string tipoProceso = Request.QueryString["tp"].ToString();
                string tipoDocumento;
                string numeroDocumento;
                string nombreCliente;
                string cuspp;
                string solicitud;
                int item;
                int version;
                string nombreAgente;
                string asunto;
                string html;
                TextInfo ti = new CultureInfo("es-PE", false).TextInfo;


                switch (tipoProceso)
                {
                    case "1":
                    case "2":
                        // Consentimiento RV
                        NombreProceso.Text = tipoProceso == "1" ? "Consentimiento de asesoría RV" : "Consentimiento de asesoría RP";

                        tipoDocumento = Request.QueryString["td"].ToString();
                        numeroDocumento = Request.QueryString["nd"].ToString();

                        if (!string.IsNullOrEmpty(tipoDocumento) && !string.IsNullOrEmpty(numeroDocumento))
                        {
                            // Obtener el consentimiento
                            ConsentimientoCliente consentimiento = null;

                            string urlConsultaConsentimientoCliente = ConfigurationManager.AppSettings["url_consulta_consentimiento_cliente"];
                            urlConsultaConsentimientoCliente = string.Format(urlConsultaConsentimientoCliente, tipoProceso, tipoDocumento, numeroDocumento, Session["Usuario"]);

                            log.Debug($"Consumiendo endpoint urlConsultaConsentimientoCliente | [GET] {urlConsultaConsentimientoCliente}");
                            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlConsultaConsentimientoCliente);

                            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                            }

                            if (consentimiento != null)
                            {
                                string urlAppConsentimiento = ConfigurationManager.AppSettings["url_app_consentimiento"];
                                urlAppConsentimiento = string.Format(urlAppConsentimiento, consentimiento.gls_token);

                                nombreCliente = consentimiento.gls_nombres;
                                nombreAgente = consentimiento.gls_nombres_agente;

                                string keyCodProcesoSME = tipoProceso == "1" ? "SMEConsentimientoRV" : "SMEConsentimientoRP";
                                string codProcesoSME = ConfigurationManager.AppSettings[keyCodProcesoSME];

                                // Obtener asunto y plantilla
                                dynamic configuracionSME = Utilitarios.ObtenerConfiguracionSME(codProcesoSME, DateTime.Now);
                                asunto = configuracionSME.gls_configuracion;
                                html = configuracionSME.arc_documento_correo;

                                // Combinar plantilla
                                html = html
                                        .Replace("#!Id_link!#", urlAppConsentimiento)
                                        .Replace("#!Id_nombres!#", ti.ToTitleCase(nombreCliente.ToLower()))
                                        .Replace("#!Id_agente!#", ti.ToTitleCase(nombreAgente.ToLower()));

                                // Renderizar asunto y plantilla
                                Asunto.Text = asunto;
                                PlantillaHTML.Text = html;
                            }
                            else
                            {
                                // 404
                                Response.Redirect("~/Error/404.aspx");
                            }
                        }
                        else
                        {
                            // 404
                            Response.Redirect("~/Error/404.aspx");
                        }
                        break;
                    case "3":
                        // Firma Digital RV (VCTP)
                        NombreProceso.Text = "Fomato VCTP";

                        solicitud = Request.QueryString["s"].ToString();
                        item = Convert.ToInt32(Request.QueryString["i"]);
                        DateTime fechaCotizacion = Convert.ToDateTime(Request.QueryString["fc"]);

                        if (!string.IsNullOrEmpty(solicitud) && item != 0)
                        {
                            // Obtener datos del beneficiario
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Solicitud solicitudRV = servicioCotizador.ObtenerDatosSolicitud(solicitud, fechaCotizacion);
                            GrupoFamiliar beneficiario = solicitudRV.Beneficiarios.Find(b => b.Id == item);
                            Agente agente = servicioCotizador.ObtenerAgente(Convert.ToInt32(solicitudRV.Agente.Id), Session["Usuario"].ToString());

                            // Obtener el objeto de Firma Digital
                            FirmaDigital firma = null;
                            string urlEndpoint = string.Format("{0}/firmas-digitales/por-solicitud/{1}/{2}/{3}", ConfigurationManager.AppSettings["url_base_api_cwrv"], solicitud, item, Session["Usuario"]);
                            log.Debug(string.Format("Inicio: GET [{0}]", urlEndpoint));
                            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlEndpoint);
                            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                firma = JsonConvert.DeserializeObject<FirmaDigital>(responseBody);
                            }
                            log.Debug(string.Format("Fin: GET [{0}]", urlEndpoint));

                            if (firma != null)
                            {
                                string urlAppFirmasDigitales = ConfigurationManager.AppSettings["url_app_firmas_digitales"];
                                urlAppFirmasDigitales = string.Format(urlAppFirmasDigitales, firma.gls_token);
                                nombreAgente = agente.Nombre;
                                nombreCliente = beneficiario.Nombre;

                                // Obtener asunto y plantilla
                                dynamic configuracion = Utilitarios.ObtenerConfiguracionSME(ConfigurationManager.AppSettings["SMEFirmaDigitalRV"], DateTime.Now);
                                asunto = configuracion.gls_configuracion;
                                html = configuracion.arc_documento_correo;

                                // Combinar plantilla
                                html = html
                                        .Replace("#!Id_nombres!#", ti.ToTitleCase(nombreCliente.ToLower()))
                                        .Replace("#!Id_link!#", urlAppFirmasDigitales)
                                        .Replace("#!Id_agente!#", nombreAgente);

                                // Renderizar asunto y plantilla
                                Asunto.Text = asunto;
                                PlantillaHTML.Text = html;
                            }
                            else
                            {
                                // 404
                                Response.Redirect("~/Error/404.aspx");
                            }
                        }
                        else
                        {
                            // 404
                            Response.Redirect("~/Error/404.aspx");
                        }
                        break;
                    case "4":
                        // Firma Digital RP
                        NombreProceso.Text = "Firma Digital Renta Particular";

                        solicitud = Request.QueryString["s"].ToString();
                        item = Convert.ToInt32(Request.QueryString["i"]);

                        if (!string.IsNullOrEmpty(solicitud) && item != 0)
                        {
                            // Obtener datos del beneficiario
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            SolicitudIFP solicitudIFP = servicioCotizador.ObtenerDatosSolicitudIFP(solicitud);
                            GrupoFamiliar beneficiario = solicitudIFP.Beneficiarios.Find(b => b.Id == item);
                            Agente agente = servicioCotizador.ObtenerAgente(Convert.ToInt32(solicitudIFP.Agente.Id), Session["Usuario"].ToString());

                            // Obtener el objeto de Firma Digital
                            FirmaDigital firma = null;
                            string urlEndpoint = string.Format("{0}/firmas-digitales/por-solicitud/{1}/{2}/{3}", ConfigurationManager.AppSettings["url_base_api_cwrv"], solicitud, item, Session["Usuario"]);
                            log.Debug(string.Format("Inicio: GET [{0}]", urlEndpoint));
                            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlEndpoint);
                            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                firma = JsonConvert.DeserializeObject<FirmaDigital>(responseBody);
                            }
                            log.Debug(string.Format("Fin: GET [{0}]", urlEndpoint));

                            if (firma != null)
                            {
                                string urlAppFirmasDigitales = ConfigurationManager.AppSettings["url_app_firmas_digitales"];
                                urlAppFirmasDigitales = string.Format(urlAppFirmasDigitales, firma.gls_token);
                                nombreAgente = agente.Nombre;
                                nombreCliente = beneficiario.Nombre;

                                // Obtener asunto y plantilla
                                dynamic configuracion = Utilitarios.ObtenerConfiguracionSME(ConfigurationManager.AppSettings["SMEFirmaDigitalRP"], DateTime.Now);
                                asunto = configuracion.gls_configuracion;
                                html = configuracion.arc_documento_correo;

                                // Combinar plantilla
                                html = html
                                        .Replace("#!Id_nombres!#", ti.ToTitleCase(nombreCliente.ToLower()))
                                        .Replace("#!Id_link!#", urlAppFirmasDigitales)
                                        .Replace("#!Id_agente!#", nombreAgente);

                                // Renderizar asunto y plantilla
                                Asunto.Text = asunto;
                                PlantillaHTML.Text = html;
                            }
                            else
                            {
                                // 404
                                Response.Redirect("~/Error/404.aspx");
                            }
                        }
                        else
                        {
                            // 404
                            Response.Redirect("~/Error/404.aspx");
                        }
                        break;
                    case "5":
                        // Flujo de Corrección RP
                        NombreProceso.Text = "Flujo de corrección Renta Particular";

                        solicitud = Request.QueryString["s"].ToString();
                        item = Convert.ToInt32(Request.QueryString["i"]);
                        version = Convert.ToInt32(Request.QueryString["v"]);

                        if (!string.IsNullOrEmpty(solicitud) && item > 0 && version > 0)
                        {
                            // Obtener datos del beneficiario
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            SolicitudIFP solicitudIFP = servicioCotizador.ObtenerDatosSolicitudIFP(solicitud);
                            GrupoFamiliar beneficiario = solicitudIFP.Beneficiarios.Find(b => b.Id == item);

                            // Obtener los datos de la versión solicitada y de la anterior para comparar
                            List<FormatoSolicitud> formatos = servicioCotizador.ListarFormatosSolicitud(solicitud, Session["Usuario"].ToString());
                            int idFormatoPrevio = formatos.Find(f => f.Correlativo == version).Id;

                            // Obtener versiones
                            FormatoSolicitud formatoActual = servicioCotizador.ObtenerFormatoSolicitudActualizado(solicitud, Session["Usuario"].ToString());
                            FormatoSolicitud formatoPrevio = servicioCotizador.ObtenerFormatoSolicitud(solicitud, idFormatoPrevio, Session["Usuario"].ToString());
                            List<FormatoSolicitudBeneficiario> formatoBeneficiariosActual = servicioCotizador.ListarFormatoSolicitudBeneficiarioActualizado(solicitud, Session["Usuario"].ToString());
                            List<FormatoSolicitudBeneficiario> formatoBeneficiariosPrevio = servicioCotizador.ListarFormatoSolicitudBeneficiario(solicitud, idFormatoPrevio, Session["Usuario"].ToString());
                            List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasActual = servicioCotizador.ListarFormatoSolicitudPersonaVinculadaActualizado(solicitud, Session["Usuario"].ToString());
                            List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasPrevio = servicioCotizador.ListarFormatoSolicitudPersonaVinculada(solicitud, idFormatoPrevio, Session["Usuario"].ToString());

                            // Comparar versiones
                            string tabla = Utilitarios.CrearTablaComparativa(formatoActual, formatoPrevio, formatoBeneficiariosActual, formatoBeneficiariosPrevio, formatoPersonasVinculadasActual, formatoPersonasVinculadasPrevio);

                            // Obtener el objeto de Firma Digital
                            FirmaDigital firma = null;
                            string urlEndpoint = string.Format("{0}/firmas-digitales/por-solicitud/{1}/{2}/{3}", ConfigurationManager.AppSettings["url_base_api_cwrv"], solicitud, item, Session["Usuario"]);
                            log.Debug(string.Format("Inicio: GET [{0}]", urlEndpoint));
                            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlEndpoint);
                            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                firma = JsonConvert.DeserializeObject<FirmaDigital>(responseBody);
                            }
                            log.Debug(string.Format("Fin: GET [{0}]", urlEndpoint));

                            if (firma != null)
                            {
                                string urlAppFirmasDigitales = ConfigurationManager.AppSettings["url_app_firmas_digitales"];
                                urlAppFirmasDigitales = string.Format(urlAppFirmasDigitales, firma.gls_token);
                                nombreCliente = beneficiario.Nombre;

                                // Obtener asunto y plantilla
                                dynamic configuracion = Utilitarios.ObtenerConfiguracionSME(ConfigurationManager.AppSettings["SMEFirmaDigitalRPFlujoCorreccion"], DateTime.Now);
                                asunto = configuracion.gls_configuracion;
                                html = configuracion.arc_documento_correo;

                                // Combinar plantilla
                                html = html
                                        .Replace("#!Id_nombre!#", ti.ToTitleCase(nombreCliente.ToLower()))
                                        .Replace("#!Id_link!#", urlAppFirmasDigitales)
                                        .Replace("#!Id_tabla!#", tabla);

                                // Renderizar asunto y plantilla
                                Asunto.Text = asunto;
                                PlantillaHTML.Text = html;
                            }
                            else
                            {
                                // 404
                                Response.Redirect("~/Error/404.aspx");
                            }
                        }
                        else
                        {
                            // 404
                            Response.Redirect("~/Error/404.aspx");
                        }
                        break;
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