using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Net;
using System.Reflection;
using log4net;

using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Dominio.Entidades;
using System.ServiceModel;

namespace Interseguro.CWRV.Presentacion.ASPNET.Seguridad
{
    public partial class CerrarSesion : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CerrarSesion));
        private static IServicioCWRV servicioCotizador;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    string nombreTerminal = String.Empty;
                    try
                    {
                        nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                    }
                    catch (Exception)
                    {
                        log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                            Request.ServerVariables["remote_addr"]));
                    }

                    nombreTerminal += Request.UserAgent;

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    servicioCotizador.RegistrarLog(new LogBD
                    {
                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                        NombreTerminal = nombreTerminal,
                        IP = Request.ServerVariables["remote_addr"],
                        NombreUsuario = (string)Session["Usuario"],
                        IdTipoEvento = Enums.EventoLog.CerrarSesion.StringValue()
                    });

                    // Limpiar variables de sesión explícitamente
                    LimpiarVariablesSesion();

                    // Cerrar la autenticación
                    FormsAuthentication.SignOut();

                    // Abandonar la sesión
                    Session.Clear();
                    Session.Abandon();
                    Session.RemoveAll();

                    // Limpiar la cookie de sesión
                    LimpiarCookieSesion();

                    log.Info(String.Format("Usuario [{0}] ha cerrado sesión correctamente.", Session["Usuario"]));

                    Response.Redirect("~/Seguridad/IniciarSesion.aspx");
                }
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
                    }
                }
            }
        }

        private void LimpiarVariablesSesion()
        {
            try
            {
                // Obtener todas las claves de sesión
                List<string> sessionKeys = new List<string>();
                foreach (string key in Session.Keys)
                {
                    sessionKeys.Add(key);
                }

                // Registrar en el log las variables que se van a limpiar
                log.Info($"Limpiando {sessionKeys.Count} variables de sesión");

                // Limpiar cada variable de sesión
                foreach (string key in sessionKeys)
                {
                    try
                    {
                        var valor = Session[key];
                        if (valor != null)
                        {
                            // Registrar el tipo de objeto que se está limpiando
                            log.Debug($"Limpiando variable de sesión: {key} (Tipo: {valor.GetType().FullName})");
                            Session[key] = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Warn($"Error al limpiar la variable de sesión {key}: {ex.Message}");
                    }
                }

                // Forzar la recolección de basura
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Verificar que todas las variables se limpiaron
                bool todasLimpiadas = true;
                foreach (string key in Session.Keys)
                {
                    if (Session[key] != null)
                    {
                        log.Warn($"La variable de sesión {key} no se pudo limpiar completamente");
                        todasLimpiadas = false;
                    }
                }

                if (todasLimpiadas)
                {
                    log.Info("Todas las variables de sesión fueron limpiadas exitosamente");
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error al limpiar variables de sesión: {ex.Message}", ex);
            }
        }

        private void LimpiarCookieSesion()
        {
            try
            {
                // Limpiar la cookie de sesión de ASP.NET
                if (Response.Cookies["ASP.NET_SessionId"] != null)
                {
                    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddYears(-1);
                }

                // Limpiar la cookie de autenticación de Forms
                if (Response.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    Response.Cookies[FormsAuthentication.FormsCookieName].Value = string.Empty;
                    Response.Cookies[FormsAuthentication.FormsCookieName].Expires = DateTime.Now.AddYears(-1);
                }

                // Limpiar todas las cookies de la aplicación
                foreach (string cookieName in Request.Cookies.AllKeys)
                {
                    if (Response.Cookies[cookieName] != null)
                    {
                        Response.Cookies[cookieName].Value = string.Empty;
                        Response.Cookies[cookieName].Expires = DateTime.Now.AddYears(-1);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn($"Error al limpiar cookies: {ex.Message}");
            }
        }
    }
}