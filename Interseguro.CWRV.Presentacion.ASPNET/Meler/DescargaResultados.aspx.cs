using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Meler
{
    public partial class DescargaResultados : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DescargaResultados));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DescargaResultados))
                {
                    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                        Enums.OpcionesSistema.DescargaResultados.StringValue()));
                    Response.Redirect("~/Error/Permisos.aspx");
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
                }
                MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                MCMEstado.Value = "1";
            }
        }

        protected void GuardarArchivo_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Archivo.HasFile)
                    {
                        string xml;
                        using (StreamReader inputStreamReader = new StreamReader(Archivo.PostedFile.InputStream, Encoding.Default, true))
                        {
                            xml = inputStreamReader.ReadToEnd();
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            int lote = 0;
                            List<Solicitud> solicitudes = servicioCotizador.RegistrarDescargaResultados(xml, Session["usuario"].ToString(), ref lote);
                            log.Debug(string.Format("Se cargó correctamente el archivo de Descarga de Reultados. Lote[{0}] Solicitudes a evaluar envío de VCTP[{1}]", lote, solicitudes.Count));
                            NumeroLote.Text = lote.ToString();
                            FechaDesde.Text = string.Empty;
                            FechaHasta.Text = string.Empty;

                            Respuesta respuestaGenerar = servicioCotizador.GenerarPolizaRVI(solicitudes, Session["usuario"].ToString());

                            if(respuestaGenerar.Estado != Constante.COD_OK)
                            {
                                throw new Exception("Error al generar la pólizas RVI");
                            }

                            //Respuesta respuestaEnvio = EnvioAutomaticoVCTP(solicitudes);

                            //if (respuestaEnvio.Estado != Constante.COD_OK)
                            //{
                            //    throw new Exception("Error en el envío automático VCTP");
                            //}

                            if (solicitudes != null && solicitudes.Count > 0)
                            {
                                log.Debug(string.Format("Se van a evaluar {0} solicitudes de envío de VCTP", solicitudes.Count));
                                var respuesta = EnvioAutomaticoVCTP(solicitudes);

                                if (respuesta.Estado != Constante.COD_OK)
                                {
                                    log.Debug("Error en el envío automático VCTP");
                                }

                            }
                            else
                            {
                                log.Debug("No hay Solicitudes evaluar envío automatico VCTP");
                            }
                            
                            MCMMensaje.Text = string.Format("Se cargó el archivo de resultados correctamente, número de lote generado: {0}", lote);
                            MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Exito.StringValue();
                            MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Exito.StringValue();
                            MCMEstado.Value = "1";
                        }
                    }
                    else
                    {
                        log.Error("No se ha seleccionado ningún archivo para ser guardado.");
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { "No se ha seleccionado ningún archivo" });
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstado.Value = "1";
                    }
                }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }

        private Respuesta EnvioAutomaticoVCTP(List<Solicitud> numerosSolicitudes)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {

                Respuesta respuesta = new Respuesta();
                try
                {
                    List<string> errores = new List<string>();
                    List<string> controles = new List<string>();

                    string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                    var urlSolicitudesCerradas = ConfigurationManager.AppSettings["url_solicitudes_cerradas"].ToString();
                    var urlListaFirmasDigitales = ConfigurationManager.AppSettings["url_listar_firmas_digitales"].ToString();
                    var urlSolicitudesCerradas_post = ConfigurationManager.AppSettings["url_solicitudes_cerradas_post"].ToString();

                    log.Info("Obteniendo urlToken: " + urlToken);
                    log.Info("Obteniendo urlListaFirmasDigitales: " + urlListaFirmasDigitales);
                    log.Info("Obteniendo urlSolicitudesCerradas_post: " + urlSolicitudesCerradas_post);

                    string usuario = HttpContext.Current.Session["Usuario"].ToString();
                    log.Info("Obteniendo usuario: " + usuario);

                    List<SolicitudesCerradas> listaSolicitudesCerradas = new List<SolicitudesCerradas>();

                    urlSolicitudesCerradas = string.Format(urlSolicitudesCerradas, usuario);
                    log.Info("Obteniendo urlSolicitudesCerradas: " + urlSolicitudesCerradas);

                    log.Info("Obteniendo listaSolicitudesCerradas");
                    listaSolicitudesCerradas = ObtenerResultadoServicioGET(urlToken, urlSolicitudesCerradas, usuario, listaSolicitudesCerradas);

                    //numerosSolicitudes = new List<Solicitud>();
                    //Solicitud solicitudA = new Solicitud();
                    //solicitudA.Id = "118959";
                    //numerosSolicitudes.Add(solicitudA);
                    //Solicitud solicitudB = new Solicitud();
                    //solicitudB.Id = "79977";
                    //numerosSolicitudes.Add(solicitudB);
                    //Solicitud solicitudC = new Solicitud();
                    //solicitudC.Id = "154748";
                    //numerosSolicitudes.Add(solicitudC);
                    //Solicitud solicitudD = new Solicitud();
                    //solicitudD.Id = "152591";
                    //numerosSolicitudes.Add(solicitudD);

                    // Se filtran de la lista de solicitudes aquellas que ya se encuentran en la tabla cwrv_solicitudes_cerradas
                    if (listaSolicitudesCerradas != null)
                    {
                        if (listaSolicitudesCerradas.Count() > 0)
                        {
                            numerosSolicitudes = numerosSolicitudes.Where(s => !listaSolicitudesCerradas.Select(c => c.num_solicitud).Contains(s.Id)).ToList();
                        }
                    }

                    log.Info("foreach numerosSolicitudes");

                    respuesta.Estado = Constante.COD_OK;
                    respuesta.Estado = Constante.COD_OK;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Formatos VCTP procesados correctamente." });

                    foreach (var itemSolicitud in numerosSolicitudes)
                    {
                        int correlativo;
                        string estadoEnvioAut = "N";
                        Beneficiario beneficiario;

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<Beneficiario> beneficiarios = servicioCotizador.ListarBeneficiarios(itemSolicitud.Id, usuario);
                        log.Info("beneficiarios");

                        Beneficiario beneficiarioSob = beneficiarios.Find(ben => ben.ind_seleccionado == "S");

                        if (beneficiarioSob == null)
                        {
                            correlativo = 1;
                        }
                        else
                        {
                            correlativo = beneficiarioSob.numCorrelativo;
                        }

                        beneficiario = beneficiarios.Find(ben => ben.numCorrelativo == correlativo);

                        if (beneficiario.ind_tiene_apoderado)
                        {
                            beneficiario.ApellidoPaterno = beneficiario.ApellidoPaternoApodero;
                            beneficiario.ApellidoMaterno = beneficiario.ApellidoMaternoApodero;
                            beneficiario.Nombre = beneficiario.NombreApodero;
                            beneficiario.Identificacion.Numero = beneficiario.IdentificacionApodero.Numero;
                            beneficiario.Identificacion.IdTipo = beneficiario.IdentificacionApodero.IdTipo;
                        }

                        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                        string correoAgente = string.Empty;
                        string correoSupervisor = string.Empty;

                        log.Info("Obteniendo al agente");
                        List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                        Agente agente = listaAgentes.Find(a => a.Id == beneficiario.Afiliado.Agente.Id);

                        if (agente.IdPadre == null)
                            agente.IdPadre = "";

                        Agente supervisor = listaAgentes.Find(p => p.Id == agente.IdPadre);

                        log.Info("validarEnviar");
                        var validarEnviar = ValidarCierre(beneficiario.ApellidoPaterno, beneficiario.ApellidoMaterno, beneficiario.Nombre, beneficiario.Identificacion.Numero, beneficiario.numTelefono, beneficiario.numCelular, beneficiario.CorreoElectronico, beneficiario.centroLaboral, beneficiario.Identificacion.IdTipo, beneficiario.direccionPrincipal.direccion, beneficiario.direccionPrincipal.espacioUrbano, beneficiario.direccionPrincipal.tipoVia.Id, beneficiario.direccionPrincipal.departamento.Id, beneficiario.direccionPrincipal.provincia.Id, beneficiario.direccionPrincipal.distrito.Id, beneficiario.envioPoliza, beneficiario.direccionAlterna.direccion, beneficiario.direccionAlterna.espacioUrbano, beneficiario.direccionAlterna.personaAutorizada, beneficiario.direccionAlterna.tipoVia.Id, beneficiario.direccionAlterna.departamento.Id, beneficiario.direccionAlterna.provincia.Id, beneficiario.direccionAlterna.departamento.Id, errores, controles);

                        log.Info("validarEnviar: " + validarEnviar);
                        if (validarEnviar)
                        {
                            log.Info("Accediendo a las Key necesarias: token, correo");
                            string urlFirmasDigitales = ConfigurationManager.AppSettings["url_firmas_digitales"].ToString();
                            string urlAppFirmasDigitales = ConfigurationManager.AppSettings["url_app_firmas_digitales"].ToString();

                            string flagProveedorCorreo = ConfigurationManager.AppSettings["flag_proveedor_correo"].ToString();
                            string flagCorreoCliente = ConfigurationManager.AppSettings["flag_correo_cliente"].ToString();
                            string flagCorreoAgente = ConfigurationManager.AppSettings["flag_correo_agente"].ToString();

                            string remitente = ConfigurationManager.AppSettings["remitente_consentimiento"].ToString();
                            string destinatario = ConfigurationManager.AppSettings["destinatario_consentimiento"].ToString();
                            string asunto = ConfigurationManager.AppSettings["asunto_firmas_digitales"].ToString();

                            string token_generado = string.Empty;
                            string tokenFirmaDigital = string.Empty;
                            string token = string.Empty;

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            FirmaDigital firma = servicioCotizador.ObtenerFirmaDigital(itemSolicitud.Id, correlativo, usuario);
                            log.Info("FirmaDigital");

                            if (firma != null)
                            {
                                // Proceder con el envío únicamente si es que la solicitud no cuenta con firma digital
                                if (firma.ind_consentimiento != "S")
                                {
                                    token = firma.gls_token;

                                    if (token.Length > 0)
                                    {
                                        Respuesta estadoCorreo = new Respuesta();

                                        log.Info("Armando la url + token");
                                        urlAppFirmasDigitales = string.Format(urlAppFirmasDigitales, token);

                                        DocumentoSME documentoSME = new DocumentoSME
                                        {
                                            Email = beneficiario.CorreoElectronico,
                                            NumeroPoliza = "N/A",
                                            NumeroDocumento = beneficiario.Identificacion.Numero,
                                            Destinatario = string.Format("{0} {1} {2}", beneficiario.Nombre, beneficiario.ApellidoMaterno, beneficiario.ApellidoMaterno).Trim(),
                                            ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEFirmaDigitalRV"]),
                                            CamposDinamicos = new
                                            {
                                                Id_nombres = ti.ToTitleCase(beneficiario.Nombre.ToLower().Trim()),
                                                Id_link = urlAppFirmasDigitales,
                                                Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                                            }
                                        };

                                        log.Info("Enviando el correo SME");
                                        estadoCorreo = Utilitario.EnviarDocumentoSME(documentoSME);
                                        if (estadoCorreo.Estado != Constante.COD_OK)
                                        {
                                            log.Error(string.Format("Error al enviar el correo del cliente [{0}]", beneficiario.CorreoElectronico));
                                            throw new Exception("Error al enviar el correo del cliente.");
                                        }

                                        dynamic respDocumentoSME = JsonConvert.DeserializeObject(estadoCorreo.Mensaje);
                                        long idSME = respDocumentoSME.codigoSME;

                                        // Insertar en la tabla de seguimiento
                                        EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                                        {
                                            gls_identificador = string.Format("{0}|{1}", correlativo, itemSolicitud.Id),
                                            id_proceso_envio = (int)Enums.ProcesoEnvio.FirmaDigitalRV,
                                            id_sme = idSME,
                                            cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                            gls_mail = beneficiario.CorreoElectronico,
                                            fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                                            cod_agente = agente.Id,
                                            aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                                        };
                                        string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                                        var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                                        string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                                        log.Info("Consumiendo API de envío seguimiento");
                                        log.Debug(string.Format("Request Body[{0}]", jsonString));
                                        using (var client = new WebClient())
                                        {
                                            client.Encoding = Encoding.UTF8;
                                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                            respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                                            respuesta.Estado = Constante.COD_OK;
                                        }

                                        respuesta.Estado = Constante.COD_OK;
                                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                        respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Solicitud de firma de formato VCTP enviado correctamente." });
                                    } //hay token
                                    else
                                    { //no hay token
                                        log.Info("Consumiendo servicio token: " + urlToken);

                                        /*Obtener token*/
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

                                        /*Obtener indicador cliente permitido*/
                                        if (token_generado != "")
                                        {
                                            FirmaDigital firmaDigital = new FirmaDigital();
                                            firmaDigital.num_solicitud = itemSolicitud.Id;
                                            firmaDigital.num_item = correlativo;
                                            firmaDigital.ind_consentimiento = "N";
                                            firmaDigital.aud_usr_ingreso = usuario;

                                            var jsonFirmaCliente = JsonConvert.SerializeObject(firmaDigital);

                                            log.Info("Json consentimiento: " + jsonFirmaCliente);

                                            log.Info("Consumiendo servicio token: " + urlFirmasDigitales);
                                            httpWebRequest = (HttpWebRequest)WebRequest.Create(urlFirmasDigitales);

                                            var context = new HttpContextWrapper(HttpContext.Current);
                                            HttpRequestBase request = context.Request;
                                            httpWebRequest.UserAgent = request.UserAgent;

                                            httpWebRequest.ContentType = "application/json";
                                            httpWebRequest.Method = "POST";
                                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                                            log.Info("Pasando el json al servicio");
                                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                            {
                                                streamWriter.Write(jsonFirmaCliente);
                                                streamWriter.Flush();
                                                streamWriter.Close();
                                            }

                                            log.Info("Leyendo el servicio");
                                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                                            {
                                                var jsonResult = streamReader.ReadToEnd();
                                                JObject jObject = JObject.Parse(jsonResult);
                                                tokenFirmaDigital = (string)jObject["gls_token"];
                                            }

                                            if (tokenFirmaDigital.Length > 0)
                                            {
                                                Respuesta estadoCorreo = new Respuesta();

                                                log.Info("Armando la url + token");
                                                urlAppFirmasDigitales = string.Format(urlAppFirmasDigitales, tokenFirmaDigital);

                                                DocumentoSME documentoSME = new DocumentoSME
                                                {
                                                    Email = beneficiario.CorreoElectronico,
                                                    NumeroPoliza = "N/A",
                                                    NumeroDocumento = beneficiario.Identificacion.Numero,
                                                    Destinatario = string.Format("{0} {1} {2}", beneficiario.Nombre, beneficiario.ApellidoMaterno, beneficiario.ApellidoMaterno).Trim(),
                                                    ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEFirmaDigitalRV"]),
                                                    CamposDinamicos = new
                                                    {
                                                        Id_nombres = ti.ToTitleCase(beneficiario.Nombre.ToLower().Trim()),
                                                        Id_link = urlAppFirmasDigitales,
                                                        Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                                                    }
                                                };

                                                log.Info("Enviando el correo SME");

                                                estadoCorreo = Utilitario.EnviarDocumentoSME(documentoSME);
                                                if (estadoCorreo.Estado != Constante.COD_OK)
                                                {
                                                    log.Error(string.Format("Error al enviar el correo del cliente [{0}]", beneficiario.CorreoElectronico));
                                                    throw new Exception("Error al enviar el correo del cliente.");
                                                }

                                                dynamic respDocumentoSME = JsonConvert.DeserializeObject(estadoCorreo.Mensaje);
                                                long idSME = respDocumentoSME.codigoSME;

                                                // Insertar en la tabla de seguimiento
                                                EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                                                {
                                                    gls_identificador = string.Format("{0}|{1}", correlativo, itemSolicitud.Id),
                                                    id_proceso_envio = (int)Enums.ProcesoEnvio.FirmaDigitalRV,
                                                    id_sme = idSME,
                                                    cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                                    gls_mail = beneficiario.CorreoElectronico,
                                                    fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                                                    cod_agente = agente.Id,
                                                    aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                                                };
                                                string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                                                var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                                                log.Info("Consumiendo API de envío seguimiento");
                                                log.Debug(string.Format("Request Body[{0}]", jsonString));
                                                using (var client = new WebClient())
                                                {
                                                    client.Encoding = Encoding.UTF8;
                                                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                                    respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                                                    respuesta.Estado = Constante.COD_OK;
                                                }

                                                respuesta.Estado = Constante.COD_OK;
                                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                                respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Formato VCTP enviado correctamente." });
                                            } //token firma digital
                                            else
                                            { //error al generar el token firma digital
                                                log.Error("Hubo problemas al insertar la firma digital");
                                                respuesta.Estado = Constante.COD_ERROR;
                                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>Se producjo un error al registrar la informción del formato VCTP</strong></div>";
                                            }
                                        } //token api cwrv
                                        else
                                        { //error del token api cwrv
                                            log.Error("Hubo problemas con el Token");
                                            respuesta.Estado = Constante.COD_ERROR;
                                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>Hubo problemas con el Token</strong></div>";
                                        }
                                    }

                                    estadoEnvioAut = "S";
                                }
                                else
                                {
                                    estadoEnvioAut = "-";
                                }
                            }
                            
                            log.Debug("estadoEnvioAut: " + estadoEnvioAut);

                        } //validar enviar
                        else
                        {
                            //log.Error("Hubo problemas al insertar la firma digital");
                            //respuesta.Estado = Constante.COD_ERROR;
                            //respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            //respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            //respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>" + Utilitarios.FormatearError(errores) + "</strong></div>";

                            estadoEnvioAut = "N";
                            log.Info("estadoEnvioAut: " + estadoEnvioAut);
                        }

                        //solicitudes cerradas
                        string token_api = string.Empty;
                        log.Info("Consumiendo servicio token: " + urlToken);

                        /*Obtener token*/
                        var httpWebRequestSC = (HttpWebRequest)WebRequest.Create(urlToken);
                        httpWebRequestSC.ContentType = "application/json";
                        httpWebRequestSC.Method = "POST";

                        log.Info("Pasando el json al servicio");
                        using (var streamWriterSC = new StreamWriter(httpWebRequestSC.GetRequestStream()))
                        {
                            string json = "{\"usuario\": \"" + usuario + "\"}";

                            streamWriterSC.Write(json);
                            streamWriterSC.Flush();
                            streamWriterSC.Close();
                        }

                        log.Info("Leyendo el servicio");
                        var httpWebResponseSC = (HttpWebResponse)httpWebRequestSC.GetResponse();
                        using (var streamReaderSC = new StreamReader(httpWebResponseSC.GetResponseStream()))
                        {
                            var jsonResult = streamReaderSC.ReadToEnd();
                            JObject jObject = JObject.Parse(jsonResult);
                            token_api = (string)jObject["accessToken"];
                        }

                        if (token_api != "")
                        {
                            //servicio working days
                            DateTime fechaInicioVCTP = DateTime.Today;

                            // Obtener los feriados desde el API de Wowking Days
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            DateTime[] feriados = servicioCotizador.ListarFeriados().ToArray();

                            // Calcular fin de vigencia
                            DateTime fechaFinVCTP = aumentarDiasUtiles(fechaInicioVCTP, 3, ref feriados);

                            //urlSolicitudesCerradas
                            SolicitudesCerradas solicitudesCerradas = new SolicitudesCerradas();
                            solicitudesCerradas.num_solicitud = itemSolicitud.Id;
                            solicitudesCerradas.fec_inicio_plazo = fechaInicioVCTP;
                            solicitudesCerradas.fec_fin_plazo = fechaFinVCTP;
                            solicitudesCerradas.ind_envio_automatico_VCTP = estadoEnvioAut;
                            solicitudesCerradas.aud_usr_ingreso = usuario;

                            var jsonSolicitudesCerradas = JsonConvert.SerializeObject(solicitudesCerradas);

                            log.Info("Json Solicitudes Cerradas: " + jsonSolicitudesCerradas);

                            log.Info("Consumiendo servicio: " + urlSolicitudesCerradas_post);
                            var httpWebRequestPSC = (HttpWebRequest)WebRequest.Create(urlSolicitudesCerradas_post);

                            var contextPSC = new HttpContextWrapper(HttpContext.Current);
                            HttpRequestBase requestPSC = contextPSC.Request;
                            httpWebRequestPSC.UserAgent = requestPSC.UserAgent;

                            httpWebRequestPSC.ContentType = "application/json";
                            httpWebRequestPSC.Method = "POST";
                            httpWebRequestPSC.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_api));

                            log.Info("Pasando el json al servicio");
                            using (var streamWriter = new StreamWriter(httpWebRequestPSC.GetRequestStream()))
                            {
                                streamWriter.Write(jsonSolicitudesCerradas);
                                streamWriter.Flush();
                                streamWriter.Close();
                            }

                            log.Info("Leyendo el servicio");
                            var httpWebResponsePSC = (HttpWebResponse)httpWebRequestPSC.GetResponse();
                            using (var streamReaderPSC = new StreamReader(httpWebResponsePSC.GetResponseStream()))
                            {
                                var jsonResult = streamReaderPSC.ReadToEnd();
                                JObject jObject = JObject.Parse(jsonResult);
                                var id = (string)jObject["id_solicitudes_cerradas"];
                            }

                            //Envio de notificación al agente
                            if (estadoEnvioAut == "S" || estadoEnvioAut == "N")
                            {
                                string estado = "NO ha";
                                if (estadoEnvioAut == "S")
                                {
                                    estado = "ha";
                                }

                                string destinatarioAgente = ConfigurationManager.AppSettings["destinatario_consentimiento_agente"].ToString();

                                if (destinatarioAgente != "N")
                                {
                                    correoAgente = destinatarioAgente;
                                    correoSupervisor = destinatarioAgente;
                                }
                                else
                                {
                                    log.Info("Obteniendo el correo del agente");
                                    try
                                    {
                                        AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient servicioAzman = new AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient("epAzman");
                                        var datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                                ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                                ConfigurationManager.AppSettings["DominioRed"],
                                                agente.Usuario);

                                        if (datosUsuario != null)
                                        {
                                            if (datosUsuario.Correo != null)
                                            {
                                                if (datosUsuario.Correo != "")
                                                {
                                                    correoAgente = datosUsuario.Correo;
                                                }
                                            }
                                        }

                                        /*Supervisor*/
                                        if (supervisor != null)
                                        {
                                            if (supervisor.Usuario != null)
                                            {

                                                datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                                        ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                                        ConfigurationManager.AppSettings["DominioRed"],
                                                        supervisor.Usuario);

                                                if (datosUsuario != null)
                                                {
                                                    if (datosUsuario.Correo != "")
                                                    {
                                                        correoSupervisor = datosUsuario.Correo;
                                                    }
                                                }

                                            }
                                        }

                                    }
                                    catch (Exception)
                                    {
                                        log.Warn("No se ha enviado mail al agente " + agente.Usuario + " porque no se ha podido obtener su email");
                                    }
                                }

                                try
                                {
                                    NotificarAgente(itemSolicitud.Id, estado, agente.Nombre, correoAgente);
                                    NotificarAgente(itemSolicitud.Id, estado, supervisor.Nombre, correoSupervisor);
                                }
                                catch (Exception)
                                {
                                    log.Warn("No se ha enviado mail al agente " + agente.Usuario + " porque no se ha podido obtener su email");
                                }
                            }
                        }
                    } //for each
                }
                catch (Exception ex)
                {
                    log.Error("Error: " + Utilitarios.FormatearError(new List<string> { ex.Message }));
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }

        private DateTime aumentarDiasUtiles(DateTime fecha, int dias, ref DateTime[] feriados)
        {
            if (dias > 0)
            {
                DateTime fechaFinal = fecha.AddDays(1);
                if (fechaFinal.DayOfWeek == DayOfWeek.Saturday || fechaFinal.DayOfWeek == DayOfWeek.Sunday)
                {
                    fechaFinal = aumentarDiasUtiles(fechaFinal, 1, ref feriados);
                }
                if (feriados.Any(d => d.Ticks == fechaFinal.Ticks))
                {
                    fechaFinal = aumentarDiasUtiles(fechaFinal, 1, ref feriados);
                }
                return aumentarDiasUtiles(fechaFinal, dias - 1, ref feriados);
            }
            else
            {
                return fecha;
            }
        }

        private static List<T> ObtenerResultadoServicioGET<T>(string urlToken, string urlServicio, string usuario, List<T> request)
        {
            List<T> respuesta = default(List<T>);
            string token_generado = string.Empty;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"usuario\": \"" + usuario + "\"}";

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
                }

                if (token_generado != "")
                {
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServicio);
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        var jsonResult = streamReader.ReadToEnd();

                        if (jsonResult.Length > 0)
                        {
                            respuesta = JsonConvert.DeserializeObject<List<T>>(jsonResult);
                        }
                    }
                }

            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    throw new Exception("Error en el servicio");
                }
            }
            return respuesta;
        }

        private static bool ValidarCierre(string apellidoPaterno, string apellidoMaterno, string nombres, string numeroIdentificacion,
                                             string telefono, string celular, string correoElectronico, string centroLabores, string tipoIdentificacion,
                                             string direccionPrinc, string espacioUrbanoPrinc, string tipoViaPrinc, string departamentoPrinc, string provinciaPrinc,
                                             string distritoPrinc, string envioPoliza, string direccionAlterna, string espacioUrbanoAlterna, string personaAutorizadaAlterna, string tipoViaAlterna,
                                             string departamentoAlterna, string provinciaAlterna, string distritoAlterna, List<String> errores, List<String> controles)
        {
            bool esCorrecto = true;

            bool flagApellidoPaterno = true;
            bool flagApellidoMaterno = true;
            bool flagNombres = true;
            bool flagNumeroIdentificacion = true;
            bool flagTelefono = true;
            bool flagCelular = true;
            bool flagCorreoElectronico = true;
            bool flagCentroLabores = true;
            bool flagTipoIdentificacion = true;
            bool flagDireccionPrinc = true;
            bool flagEspacioUrbanoPrinc = true;
            bool flagTipoViaPrinc = true;
            bool flagDepartamentoPrinc = true;
            bool flagProvinciaPrinc = true;
            bool flagDistritoPrinc = true;
            bool flagEnvioPoliza = true;
            bool flagDireccionAlterna = true;
            bool flagEspacioUrbanoAlterna = true;
            bool flagPersonaAutorizadaAlterna = true;
            bool flagTipoViaAlterna = true;
            bool flagDepartamentoAlterna = true;
            bool flagProvinciaAlterna = true;
            bool flagDistritoAlterna = true;

            if (apellidoPaterno.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Apellido Paterno</strong>. Dato Obligatorio.");
                flagApellidoPaterno = false;
            }

            if (apellidoMaterno.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Apellido Materno</strong>. Dato Obligatorio.");
                flagApellidoMaterno = false;
            }

            if (nombres.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Nombres</strong>. Dato Obligatorio.");
                flagNombres = false;
            }

            if (numeroIdentificacion.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Número Identificacion</strong>. Dato Obligatorio.");
                flagNumeroIdentificacion = false;
            }

            if (telefono.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Teléfono</strong>. Dato Obligatorio.");
                flagTelefono = false;
            }

            if (celular.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Celular</strong>. Dato Obligatorio.");
                flagCelular = false;
            }

            if (correoElectronico.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.");
                flagCorreoElectronico = false;
            }

            if (centroLabores.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Centro de Labores</strong>. Dato Obligatorio.");
                flagCentroLabores = false;
            }

            if (tipoIdentificacion == "0" || tipoIdentificacion.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Tipo de Identificación</strong>. Dato Obligatorio.");
                flagTipoIdentificacion = false;
            }

            if (direccionPrinc.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Dirección Principal</strong>. Dato Obligatorio.");
                flagDireccionPrinc = false;
            }

            if (espacioUrbanoPrinc.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Espacio Urbano Principal</strong>. Dato Obligatorio.");
                flagEspacioUrbanoPrinc = false;
            }

            if (tipoViaPrinc == "0" || tipoViaPrinc.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Tipo de Vía Principal</strong>. Dato Obligatorio.");
                flagTipoViaPrinc = false;
            }

            if (departamentoPrinc == "0" || departamentoPrinc.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Departamento Principal</strong>. Dato Obligatorio.");
                flagDepartamentoPrinc = false;
            }

            if (provinciaPrinc == "0" || provinciaPrinc.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Provincia Principal</strong>. Dato Obligatorio.");
                flagProvinciaPrinc = false;
            }

            if (distritoPrinc == "0" || distritoPrinc.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Distrito Principal</strong>. Dato Obligatorio.");
                flagDistritoPrinc = false;
            }

            if (envioPoliza == "0" || envioPoliza.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Envío Póliza</strong>. Dato Obligatorio.");
                flagEnvioPoliza = false;
            }
            else
            {
                if (envioPoliza == "F")
                {
                    if (direccionAlterna.Trim().Length == 0)
                    {
                        errores.Add("Ingrese el campo <strong>Dirección Alterna</strong>. Dato Obligatorio.");
                        flagDireccionAlterna = false;
                    }

                    if (espacioUrbanoAlterna.Trim().Length == 0)
                    {
                        errores.Add("Ingrese el campo <strong>Espacio Urbano Alterna</strong>. Dato Obligatorio.");
                        flagEspacioUrbanoAlterna = false;
                    }

                    if (personaAutorizadaAlterna.Trim().Length == 0)
                    {
                        errores.Add("Ingrese el campo <strong>Persona Autorizada</strong>. Dato Obligatorio.");
                        flagPersonaAutorizadaAlterna = false;
                    }

                    if (tipoViaAlterna == "0" || tipoViaAlterna.Trim().Length == 0)
                    {
                        errores.Add("Ingrese el campo <strong>Tipo Vía Alterna</strong>. Dato Obligatorio.");
                        flagTipoViaAlterna = false;
                    }

                    if (departamentoAlterna == "0" || departamentoAlterna.Trim().Length == 0)
                    {
                        errores.Add("Ingrese el campo <strong>Departamento Alterna</strong>. Dato Obligatorio.");
                        flagDepartamentoAlterna = false;
                    }

                    if (provinciaAlterna == "0" || provinciaAlterna.Trim().Length == 0)
                    {
                        errores.Add("Ingrese el campo <strong>Provincia Alterna</strong>. Dato Obligatorio.");
                        flagProvinciaAlterna = false;
                    }

                    if (distritoAlterna == "0" || distritoAlterna.Trim().Length == 0)
                    {
                        errores.Add("Ingrese el campo <strong>Distrito Alterna</strong>. Dato Obligatorio.");
                        flagDistritoAlterna = false;
                    }
                }
            }

            // Clases de controles
            if (!flagApellidoPaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!flagApellidoMaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!flagNombres) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!flagNumeroIdentificacion) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!flagTelefono) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!flagCelular) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!flagCorreoElectronico) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!flagCentroLabores) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            if (!flagTipoIdentificacion) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }

            if (!flagDireccionPrinc) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!flagEspacioUrbanoPrinc) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            if (!flagTipoViaPrinc) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!flagDepartamentoPrinc) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!flagProvinciaPrinc) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!flagDistritoPrinc) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }

            if (!flagEnvioPoliza) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }

            if (envioPoliza == "F")
            {
                if (!flagDireccionAlterna) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
                if (!flagEspacioUrbanoAlterna) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
                if (!flagPersonaAutorizadaAlterna) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

                if (!flagTipoViaAlterna) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
                if (!flagDepartamentoAlterna) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
                if (!flagProvinciaAlterna) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
                if (!flagDistritoAlterna) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            }

            if (envioPoliza == "F")
            {
                esCorrecto = flagApellidoPaterno & flagApellidoMaterno & flagNombres & flagNumeroIdentificacion & flagTelefono
                            & flagCelular & flagCorreoElectronico & flagCentroLabores & flagTipoIdentificacion & flagDireccionPrinc
                            & flagEspacioUrbanoPrinc & flagTipoViaPrinc & flagDepartamentoPrinc & flagProvinciaPrinc & flagDistritoPrinc
                            & flagEnvioPoliza & flagDireccionAlterna & flagEspacioUrbanoAlterna & flagPersonaAutorizadaAlterna & flagTipoViaAlterna & flagDepartamentoAlterna
                            & flagProvinciaAlterna & flagDistritoAlterna;
            }
            else
            {
                esCorrecto = flagApellidoPaterno & flagApellidoMaterno & flagNombres & flagNumeroIdentificacion & flagTelefono
                            & flagCelular & flagCorreoElectronico & flagCentroLabores & flagTipoIdentificacion & flagDireccionPrinc
                            & flagEspacioUrbanoPrinc & flagTipoViaPrinc & flagDepartamentoPrinc & flagProvinciaPrinc & flagDistritoPrinc
                            & flagEnvioPoliza;
            }

            return esCorrecto;
        }

        private static void NotificarAgente(string glsSolicitud, string glsEstado, string NombreAgente, string correoAgente)
        {
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            var glsAgente = ti.ToTitleCase(NombreAgente.ToLower().Trim());

            var htmlCorreoAgente = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\Plantilla\\RVI\\SADP\\CorreoNotificacionVCTPautom.html");
            htmlCorreoAgente = htmlCorreoAgente
                .Replace("{agente}", glsAgente)
                .Replace("{solicitud}", glsSolicitud)
                .Replace("{estado}", glsEstado);

            var notificacion = new NotificacionSME()
            {
                De = "comunicaciones@interseguro.com.pe",
                DeNombre = "Comunicaciones Interseguro",
                ResponderA = "comunicaciones@interseguro.com.pe",
                ResponderANombre = "Comunicaciones Interseguro",
                Para = correoAgente,
                Cuerpo = htmlCorreoAgente,
                Asunto = $"{glsSolicitud} {glsEstado} sido notificada automáticamente",
            };

            EnviarNotificacionSME(ConfigurationManager.AppSettings["url_envio_correo_sme"], notificacion);

        }

        private static Respuesta EnviarNotificacionSME(string rutaServicio, NotificacionSME notificacionSME)
        {
            //Envio de manera Asincrono
            Respuesta respuesta = new Respuesta();
            try
            {
                List<string> errores = new List<string>();
                if (notificacionSME == null)
                {
                    errores.Add("Envíe una notificación completa. Dato Obligatorio");
                }
                else
                {
                    if (notificacionSME.De == null || notificacionSME.De == "")
                    {
                        errores.Add("Ingrese Remitente del Correo. Dato Obligatorio.");
                    }
                }

                if (rutaServicio == "")
                {
                    errores.Add("Ingrese ruta del servicio de correo. Dato Obligatorio");
                }

                if (errores.Count > 0)
                {
                    respuesta.Mensaje = Utilitarios.FormatearErrorTexto(errores);
                    return respuesta;
                }

                if (notificacionSME.Para != null)
                {
                    notificacionSME.Para = notificacionSME.Para.Trim();
                }

                log.Info("Ejecutando el servicio del correo");
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string jsonString = JsonSerializar.Serialize(notificacionSME);
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    respuesta.Mensaje = client.UploadString(new Uri(rutaServicio), "POST", jsonString);
                    respuesta.Estado = Constante.COD_OK;
                }

            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
            }
            return respuesta;
        }

        [WebMethod]
        public static Respuesta CargarTablaLotes(string tokenUsuario, string numeroLote, string fechaDesde, string fechaHasta)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaLotesResultado)pagina.LoadControl("~/Controles/TablaLotesResultado.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DescargaResultados))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarLote(numeroLote, fechaDesde, fechaHasta, errores, controles))
                            {
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                List<Lote> lotes = null;
                                if (numeroLote != "0")
                                {
                                    lotes = servicioCotizador.ListarLoteResultadoPorNumero(Convert.ToInt32(numeroLote));
                                }
                                else
                                {
                                    lotes = servicioCotizador.ListarLoteResultadoPorFecha(Convert.ToDateTime(fechaDesde, new CultureInfo("es-PE")), Convert.ToDateTime(fechaHasta, new CultureInfo("es-PE")));
                                }

                                control.Lotes = lotes;
                                control.PermisoConsultar = true;
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                                return respuesta;
                            }
                        }
                        else
                        {
                            control.PermisoConsultar = false;
                        }

                        pagina.Controls.Add(control);

                        string html = "";
                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            html = sw.ToString();
                        }
                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Contenido = html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }

        private static bool ValidarLote(string numeroLote, string fechaDesde, string fechaHasta, List<string> errores, List<string> controles)
        {
            bool esCorrecto = true;

            // Número de Lote
            bool vNumeroLote = true;
            if (numeroLote.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Número de Lote</strong>. Dato Obligatorio.");
                vNumeroLote = false;
            }
            else
            {
                int iNumeroLote;
                if (!int.TryParse(numeroLote, out iNumeroLote))
                {
                    errores.Add("El campo <strong>Número de Lote</strong> debe contener un valor numérico.");
                    vNumeroLote = false;
                }
            }

            // Fecha cierre comercial desde
            bool vFechaCierreComercial = true;
            if (fechaDesde.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha cierre comercial desde</strong>. Dato Obligatorio.");
                vFechaCierreComercial = false;
            }
            else
            {
                DateTime dFechaCierreComercialDesde;
                if (!DateTime.TryParse(fechaDesde, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out dFechaCierreComercialDesde))
                {
                    errores.Add("El campo <strong>Fecha cierre comercial desde</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    vFechaCierreComercial = false;
                }
            }

            // Fecha cierre comercial hasta
            bool vFechaCierreComerciaHasta = true;
            if (fechaHasta.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha cierre comercial hasta</strong>. Dato Obligatorio.");
                vFechaCierreComerciaHasta = false;
            }
            else
            {
                DateTime dFechaCierreComercialHasta;
                if (!DateTime.TryParse(fechaHasta, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out dFechaCierreComercialHasta))
                {
                    errores.Add("El campo <strong>Fecha cierre comercial hasta</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    vFechaCierreComerciaHasta = false;
                }
            }

            // Clases de controles
            if (!vNumeroLote) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!vFechaCierreComercial) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!vFechaCierreComerciaHasta) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }

            esCorrecto = vNumeroLote | (vFechaCierreComercial & vFechaCierreComerciaHasta);

            return esCorrecto;
        }

        [WebMethod]
        public static Respuesta ExportarReporte(string tokenUsuario, string numeroLote)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        HttpContext.Current.Session["numeroLote"] = numeroLote;
                        respuesta.Estado = Constante.COD_OK;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }
    }
}