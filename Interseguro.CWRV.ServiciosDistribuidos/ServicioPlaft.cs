using System.Text;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Aplicacion.ModuloPrincipal;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Infraestructura.Transversal;
using log4net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Configuration;
using System;
using System.Linq;
using Interseguro.CWRV.ServiciosDistribuidos.Proxies.ModuloSeguridad;
using System.Net;
using System.Web.Script.Serialization;

namespace Interseguro.CWRV.ServiciosDistribuidos
{

    public class ServicioPlaft : IServicioPlaft
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServicioPlaft));

        public Acceso SolicitarAcceso()
        {
            Acceso acceso;
            try
            {
                // Obtener la dirección IP del cliente que consume el método   
                string ip = ObtenerIP();

                log.Debug(String.Format("Se ha solicitado acceso al cotizador web de rentas desde la siguiente IP:" + ip));

                ISeguridadServicio seguridadServicio = FabricaIoC.Contenedor.Resolver<ISeguridadServicio>();
                SolicitudAcceso solicitud = new SolicitudAcceso
                {
                    Usuario = "",
                    CUSPP = "",
                    IP = ip
                };
                acceso = seguridadServicio.SolicitarAcceso(solicitud);

                if (acceso.Codigo != 0)
                {
                    log.Error(acceso.Mensaje);
                }

                return acceso;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                acceso = new Acceso
                {
                    Codigo = -9,
                    Mensaje = ex.Message,
                    Token = null
                };
                return acceso;
            }
        }
        
        private string ObtenerIP()
        {
            // Obtener la dirección IP del cliente que consume el método
            OperationContext context = OperationContext.Current;
            MessageProperties prop = context.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string ip = endpoint.Address;

            return ip;
        }
        
        public RespuestaPlaft ActualizarSolicitud(string token, string num_solicitud, int cod_estado_plaft, string gls_observacion)
        {

            RespuestaPlaft respuestaPlaft = new RespuestaPlaft();
            try
            {
                log.Info("ActualizarSolicitud - Plaft - INI");

                log.Debug(String.Format("Token Recibido: " + token));
                log.Debug(String.Format("num_solicitud Recibido: " + num_solicitud));
                log.Debug(String.Format("cod_estado_plaft Recibido: " + cod_estado_plaft));
                log.Debug(String.Format("gls_observado Recibido: " + gls_observacion));

                string ip = ObtenerIP();

                ISeguridadServicio seguridadServicio = FabricaIoC.Contenedor.Resolver<ISeguridadServicio>();
                SolicitudAcceso solicitudAcceso = seguridadServicio.ValidarToken(token, ip);
                if (solicitudAcceso == null)
                {
                    respuestaPlaft.Estado = Constante.COD_ERROR;
                    respuestaPlaft.Mensaje = String.Format("El Token [{0}] con el que se ha accedido no ha sido generado por el CWRV.", token);
                }
                else
                {
                    // Validar que el Token esté vigente
                    if (solicitudAcceso.Vigente)
                    {
                        /*
                         EstadoPlaft:
                         * 0 = Cotizado
                         * 1 = Observado Corregir
                         * 2 = Rechazado Definitivo
                         * 3 = Aprobado
                         */

                        if (cod_estado_plaft.ToString() != Enums.EstadoPlaft.Observado.StringValue()
                            && cod_estado_plaft.ToString() != Enums.EstadoPlaft.Rechazado.StringValue()
                            && cod_estado_plaft.ToString() != Enums.EstadoPlaft.Aprobado.StringValue())
                        {
                            respuestaPlaft.Estado = Constante.COD_ERROR;
                            respuestaPlaft.Mensaje = "Los estados a recibir son: [1] = Rechazado Corregir, [2] = Rechazado Definitivo, [3] = Aprobado";
                            return respuestaPlaft;
                        }

                        if (num_solicitud.Length != 10)
                        {
                            respuestaPlaft.Estado = Constante.COD_ERROR;
                            respuestaPlaft.Mensaje = "El Nro. propuesta debe tener 10 caracteres.";
                            return respuestaPlaft;
                        }

                        string tipoFlujo = "0";
                        if (cod_estado_plaft.ToString() == Enums.EstadoPlaft.Observado.StringValue())
                        {
                            tipoFlujo = Enums.TipoFlujoEvaluacion.ObservadoPlaft.StringValue();
                            if (gls_observacion.Length == 0)
                            {
                                respuestaPlaft.Estado = Constante.COD_ERROR;
                                respuestaPlaft.Mensaje = "La observación es un campo obligatoria.";
                                return respuestaPlaft;
                            }
                        }

                        if (cod_estado_plaft.ToString() == Enums.EstadoPlaft.Rechazado.StringValue())
                        {
                            tipoFlujo = Enums.TipoFlujoEvaluacion.RechazadoPlaft.StringValue();
                            if (gls_observacion.Length == 0)
                            {
                                respuestaPlaft.Estado = Constante.COD_ERROR;
                                respuestaPlaft.Mensaje = "El motivo es un campo obligatorio.";
                                return respuestaPlaft;
                            }
                        }

                        if (cod_estado_plaft.ToString() == Enums.EstadoPlaft.Aprobado.StringValue())
                            tipoFlujo = Enums.TipoFlujoEvaluacion.AprobadoPlaft.StringValue();

                        ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                        Respuesta respuesta = cotizadorServicio.ActualizacionSolicitudPlusPlaft(num_solicitud, tipoFlujo, cod_estado_plaft, gls_observacion, "", "platf");
                        respuestaPlaft.Estado = respuesta.Estado;
                        respuestaPlaft.Mensaje = respuesta.Mensaje;

                        log.Info(respuesta.Mensaje);

                        /*Envio de Correo*/
                        if (respuestaPlaft.Estado == "OK")
                        {

                            string str_SupervisorAgente = string.Empty;
                            BEUsuario datosUsuario = null;
                            BEUsuario datosUsuarioCorreo = null;
                            IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();

                            SolicitudRPPlus solicitudRPPlus = cotizadorServicio.ObtenerEstadoSolicitudRPPlus(num_solicitud);
                            ServicioAzmanClient servicioAzman = new ServicioAzmanClient("epAzman");

                            //Obtener Arbol de agente
                            Agente AgenteSupervisor = usuarioServicio.ObtenerSupervisorAgente(solicitudRPPlus.Agente.Usuario);
                            bool validarComa = true;

                            //Cambio Inteligo
                            if (AgenteSupervisor.Nombre != null)
                            {

                                /*Agente*/
                                datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                        ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                        ConfigurationManager.AppSettings["DominioRed"],
                                        AgenteSupervisor.Nombre);

                                if (datosUsuario != null)
                                {
                                    if (datosUsuario.Correo != "")
                                    {
                                        str_SupervisorAgente = datosUsuario.Matricula;
                                    }
                                }

                                /*Supervisor*/
                                if (AgenteSupervisor.IdPadre != "")
                                {

                                    datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                            ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                            ConfigurationManager.AppSettings["DominioRed"],
                                            AgenteSupervisor.IdPadre);


                                    if (datosUsuario != null)
                                    {
                                        if (datosUsuario.Correo != "")
                                        {
                                            str_SupervisorAgente += "," + datosUsuario.Matricula + ",";
                                            validarComa = false;
                                        }
                                    }
                                }

                                if (validarComa)
                                {
                                    str_SupervisorAgente += ",";
                                }
                            }
                            
                            //PLAFT 1=Obervado, 2=Rechazado, 3=Aprobado
                            //Solici 5=Observado, 6=Aprobado 7=Rechazado
                            if (solicitudRPPlus.CodigoEstado == 5 || solicitudRPPlus.CodigoEstado == 6 || solicitudRPPlus.CodigoEstado == 7)
                            {
                                int cod_proceso = 0;

                                if (solicitudRPPlus.CodigoEstado == 6 && cod_estado_plaft == 3)
                                {
                                    cod_proceso = 2;//Aprobado
                                }
                                else if (solicitudRPPlus.CodigoEstado == 7 || cod_estado_plaft == 2)
                                {
                                    cod_proceso = 4;//Rechazada
                                }
                                else//5
                                {
                                    cod_proceso = 3;//Observado
                                }

                                /*Armando Observaciones para el Correo*/
                                string Observaciones = "";

                                if (solicitudRPPlus.GlsObservacionPlaft != "")
                                {
                                    Observaciones = "Plaft: " + solicitudRPPlus.GlsObservacionPlaft + "<br />";
                                }

                                if (solicitudRPPlus.GlsObservacionRpp != "")
                                {
                                    Observaciones += "Operaciones: " + solicitudRPPlus.GlsObservacionRpp + "<br />";
                                }


                                log.Info("Inicio de Armado de Correo");
                                ConfiguracionCorreo configuracionCorreo = ObtenerConfiguracionCorreo(cod_proceso, DateTime.Today);

                                //AGENTE Y SUPERVISOR//Descomentar
                                configuracionCorreo.gls_destinatario += "," + str_SupervisorAgente;

                                if (cod_proceso == 2)//Aprobado
                                {
                                    configuracionCorreo.gls_destinatario += "," + ConfigurationManager.AppSettings["destinatario_aprobacion_plaft_operaciones"];
                                }
                                else if (cod_proceso == 4)//Rechazada
                                {
                                    configuracionCorreo.gls_destinatario += "," + ConfigurationManager.AppSettings["destinatario_rechazado_plaft_operaciones"];
                                }
                                else if (cod_proceso == 3)//Observado
                                {
                                    //configuracionCorreo.gls_destinatario += "," + ConfigurationManager.AppSettings["destinatario_observado_plaft_operaciones"];
                                }

                                string usuarios_correo = configuracionCorreo.gls_destinatario;

                                string[] lista_usuarios_correo = usuarios_correo.Split(',');

                                string arc_documento_correo = configuracionCorreo.arc_documento_correo;

                                if (configuracionCorreo.enviar_correo == "S")
                                {
                                    foreach (string usuario_correo in lista_usuarios_correo)
                                    {
                                        if (usuario_correo.Trim() != "")
                                        {
                                            Notificacion notificacion = new Notificacion();
                                            if (!usuario_correo.Trim().Contains('@'))
                                            {
                                                datosUsuarioCorreo = new BEUsuario();
                                                datosUsuarioCorreo = servicioAzman.ObtenerDatosUsuarioSinClave(
                                                ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                                ConfigurationManager.AppSettings["DominioRed"],
                                                usuario_correo.Trim());
                                            }
                                            else
                                            {
                                                string usuarios_plaft = usuario_correo.Trim();
                                                string[] lista_usuarios_plaft = usuarios_plaft.Split(',');

                                                datosUsuarioCorreo = new BEUsuario();
                                                if (lista_usuarios_plaft.Length > 1)
                                                {
                                                    datosUsuarioCorreo.NombreCompleto = lista_usuarios_plaft[0];
                                                    datosUsuarioCorreo.Correo = lista_usuarios_plaft[1];
                                                }
                                                else
                                                {
                                                    datosUsuarioCorreo.Correo = lista_usuarios_plaft[0];
                                                    datosUsuarioCorreo.NombreCompleto = "";
                                                }
                                            }

                                            configuracionCorreo.arc_documento_correo = arc_documento_correo;

                                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{num_solicitud}", num_solicitud);
                                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{nombre}", datosUsuarioCorreo.NombreCompleto);
                                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{gls_observaciones}", Observaciones);

                                            if (cod_proceso == 3)//Observado
                                            {

                                                string url = string.Empty;

                                                if (num_solicitud.Substring(0, 3) == "RPP")
                                                {
                                                    url = ConfigurationManager.AppSettings["url_correo_observacion_rpp"] + "&solicitud=" + num_solicitud;
                                                }
                                                else
                                                {
                                                    url = ConfigurationManager.AppSettings["url_correo_observacion_ifp"] + "&solicitud=" + num_solicitud;
                                                }
                                                
                                                configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{url}", url);
                                            }
                                            else
                                            {
                                                configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("<a href=\"{url}\">Consulte la propuesta observada haciendo clic aquí</a>", "");
                                            }

                                            configuracionCorreo.gls_asunto = configuracionCorreo.gls_asunto.Replace("{num_solicitud}", num_solicitud);

                                            notificacion.p_destinatario = datosUsuarioCorreo.Correo;

                                            notificacion.p_remitente = configuracionCorreo.gls_remitente;
                                            notificacion.p_asunto = configuracionCorreo.gls_asunto;
                                            notificacion.p_mensaje = configuracionCorreo.arc_documento_correo;
                                            notificacion.p_displayName = configuracionCorreo.gls_display_name;
                                            notificacion.p_ruta_archivo_adjunto = "";

                                            log.Info("Fin de Armado de Correo");

                                            //Envio de manera Asincrono
                                            var tareaParalela = new System.Threading.Tasks.Task(() =>
                                            {
                                                //log.Info("Inicio de Envio Notificacion: " + notificacion.p_destinatario);
                                                log.Info(String.Format("Inicio de Envio Notificacion Correo:{0}", notificacion.p_destinatario));
                                                Respuesta RptaNotifica = cotizadorServicio.EnviarNotificacion(configuracionCorreo.gls_ruta_servicio, notificacion);
                                                log.Info(String.Format("RptaNotifica Envio Correo Estado:{0}, Mensaje:{1}", RptaNotifica.Estado, RptaNotifica.Mensaje));
                                                //log.Info("Fin de Envio Notificacion: " + notificacion.p_destinatario);
                                                log.Info(String.Format("Fin de Envio Notificacion Correo:{0}", notificacion.p_destinatario));
                                            });

                                            tareaParalela.Start();
                                        }
                                    }
                                }


                            }
                        }

                    }
                    else
                    {
                        respuestaPlaft.Estado = Constante.COD_ERROR;
                        respuestaPlaft.Mensaje = String.Format("El Token [{0}] con el que se ha accedido ya ha expirado.", token);
                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                respuestaPlaft.Estado = Constante.COD_ERROR;
                respuestaPlaft.Mensaje = ex.Message.ToString();
            }
            log.Info("ActualizarSolicitud - Plaft - FIN");
            return respuestaPlaft;
        }

        public ConfiguracionCorreo ObtenerConfiguracionCorreo(int cod_proceso, DateTime fec_solicitud)
        {
            ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
            ConfiguracionCorreo configuracionCorreo = cotizadorServicio.ObtenerConfiguracionCorreo(cod_proceso, DateTime.Today);
            configuracionCorreo.gls_ruta_servicio = ConfigurationManager.AppSettings["url_envio_correo"];
            configuracionCorreo.enviar_correo = ConfigurationManager.AppSettings["enviar_correo"];
            switch (configuracionCorreo.cod_proceso.ToString())
            {
                case "1"://destinatario_envio_evaluacion
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_envio_evaluacion_comercial"];
                    break;
                case "2"://destinatario_aprobacion_plaft_operaciones
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_aprobacion_plaft_comercial"];
                    break;
                case "3"://destinatario_observado_plaft_operaciones
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_observado_plaft_comercial"];
                    break;
                case "4"://destinatario_rechazado_plaft_operaciones
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_rechazado_plaft_comercial"];
                    break;
                case "5"://destinatario_lista_negra
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_lista_negra"];
                    break;
                default:
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_envio_evaluacion"];
                    break;
            }
            return configuracionCorreo;
        }
        
        public JsonEmail ObtenerEmailPlaft()
        {
            try
            {
                JsonEmail oEmail = null;
                JsonTokenPlaft jsonToken = ObtenerTokenPlaft();

                if (jsonToken == null)
                {
                    throw new Exception("Token no generado");
                }

                if (jsonToken._meta.status == "ERROR")
                {
                    throw new Exception(jsonToken._meta.status);
                }

                string url = ConfigurationManager.AppSettings["url_email"].ToString();
                log.Debug("Url: " + url);
                using (WebClient wc = new WebClient())
                {
                    wc.Headers["Authorization"] = jsonToken.records.token_type + " " + jsonToken.records.access_token;
                    try
                    {
                        var data = wc.UploadValues(url, "POST", wc.QueryString);
                        string responseString = UnicodeEncoding.UTF8.GetString(data);
                        log.Debug("Json Devuelto ObtenerEmailPlaft: " + responseString);
                        oEmail = new JavaScriptSerializer().Deserialize<JsonEmail>(responseString);
                    }
                    catch (Exception _plaft)
                    {
                        log.Error(_plaft.Message, _plaft);
                        throw new Exception("Servicio de email no disponible");
                    }

                }
                return oEmail;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public JsonTokenPlaft ObtenerTokenPlaft()
        {
            try
            {
                JsonTokenPlaft oToken = null;


                string url = ConfigurationManager.AppSettings["url_token"].ToString();
                string grant_type = ConfigurationManager.AppSettings["grant_type"].ToString();
                string client_id = ConfigurationManager.AppSettings["client_id"].ToString();
                string client_secret = ConfigurationManager.AppSettings["client_secret"].ToString();

                using (WebClient wc = new WebClient())
                {
                    wc.QueryString.Add("grant_type", grant_type);
                    wc.QueryString.Add("client_id", client_id);
                    wc.QueryString.Add("client_secret", client_secret);

                    try
                    {
                        var data = wc.UploadValues(url, "POST", wc.QueryString);

                        string responseString = UnicodeEncoding.UTF8.GetString(data);
                        oToken = new JavaScriptSerializer().Deserialize<JsonTokenPlaft>(responseString);
                    }
                    catch (Exception _plaft)
                    {
                        log.Error(_plaft.Message, _plaft);
                        throw new Exception("Servicio de token no disponible");
                    }

                }

                return oToken;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }


    }
}