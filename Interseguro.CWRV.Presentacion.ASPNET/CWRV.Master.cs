using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET
{
    public partial class CWRV : MasterPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CWRV));
        private static IServicioCWRV servicioCotizador;
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                lbApiSolicitudesCambioUrl.Text = ConfigurationManager.AppSettings["ApiSolicitudesCambioUrl"];

                lbApiCotizadorIfpUrl.Text = ConfigurationManager.AppSettings["ApiCotizadorIfpUrl"];

                lbApiCotizadorRvUrl.Text = ConfigurationManager.AppSettings["ApiCotizadorRvUrl"];

                lbApiParametroUrl.Text = ConfigurationManager.AppSettings["ApiParametroUrl"];

                lbApiAutenticacionUrl.Text = ConfigurationManager.AppSettings["ApiAutenticacionUrl"];

                lbApiReportesUrl.Text = ConfigurationManager.AppSettings["ApiReportesUrl"];

                SetearSpinner();

                if (!IsPostBack)
                {
                    TokenUsuario.Value = (string)Session["TokenUsuario"];
                    if (Session["OpcionesSistema"] != null)
                        MenuPrincipal.ListaOpciones = ((List<OpcionSistema>)Session["OpcionesSistema"]).FindAll(o => o.TipoOpcion == "M");
                }

                HttpCookie loginCookie = Request.Cookies["CotWebRVICookie"];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(loginCookie.Value);

                Session["Usuario"] = ticket.Name;

                Configuration conf = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                SessionStateSection section = (SessionStateSection)conf.GetSection("system.web/sessionState");
                int timeout = (int)section.Timeout.TotalMilliseconds;
                if (timeout > 120000)
                    timeout -= 120000;

                Timeout.Value = timeout.ToString();

                string rolUsuario = string.Empty;
                if (Session["RolAzman"] != null)
                {
                    rolUsuario = Session["RolAzman"].ToString();
                }
                else
                {
                    Response.Redirect("~/Seguridad/CerrarSesion.aspx", true);
                }

                Usuario usuario = new Usuario
                {
                    NombreUsuario = ticket.Name,
                    Rol = rolUsuario,
                    Nombre = Session["NombreCompleto"].ToString()
                };

                MenuPrincipal.Usuario = usuario;

                //CabUsuario.Text = ticket.Name + " (" + rolUsuario + ")";
                if (Session["NombreCompleto"] == null)
                {
                    log.Info("La sesión ha expirado. Se va a proceder a cerrar la sesión.");
                    CerrarSesion();
                }

                DateTime fechaActual = DateTime.Now;

                //CabFecha.Text = fechaActual.ToString("dd/MM/yyyy");
                //FechaProtegido.Text = string.Format("{0}, {1} de {2} del {3}", Utilitarios.PrimeraMayuscula(fechaActual.ToString("dddd", CultureInfo.CreateSpecificCulture("es-PE"))), fechaActual.ToString("dd"), fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-PE")), fechaActual.ToString("yyyy"));

                // Ocultar los botones de Video Tutorial y Manual de usuario y restringirlos sólo a la red de interseguro
                bool redLocal = Utilitarios.ValidarRedLocal(Request.ServerVariables["remote_addr"]);
                //if (!redLocal || !Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.VideoTutorial)) MenuTutorial.Visible = false;
                //if (!redLocal || !Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ManualUsuario)) MenuManual.Visible = false;

                // Control para cache al cierre de sesión
                Response.ClearHeaders();
                Response.AppendHeader("Cache-Control", "no-cache");
                Response.AppendHeader("Cache-Control", "private");
                Response.AppendHeader("Cache-Control", "no-store");
                Response.AppendHeader("Cache-Control", "must-revalidate");
                Response.AppendHeader("Cache-Control", "max-stale=0");
                Response.AppendHeader("Cache-Control", "post-check=0");
                Response.AppendHeader("Cache-Control", "pre-check=0");
                Response.AppendHeader("Pragma", "no-cache");
                Response.AppendHeader("Keep-Alive", "timeout=3, max=993");
                Response.AppendHeader("Expires", "Mon, 26 Jul 1997 05:00:00 GMT");
            }
        }

        private void SetearSpinner()
        {
            string clase = string.Empty;
            Random random = new Random();
            int numero = random.Next(13);
            switch (numero)
            {
                case 0:
                    clase = "ball-pulse";
                    break;
                case 1:
                    clase = "ball-grid-pulse";
                    break;
                case 2:
                    clase = "ball-scale";
                    break;
                case 3:
                    clase = "ball-scale-multiple";
                    break;
                case 4:
                    clase = "ball-spin-fade-loader";
                    break;
                case 5:
                    clase = "line-scale";
                    break;
                case 6:
                    clase = "line-scale-party";
                    break;
                case 7:
                    clase = "line-scale-pulse-out";
                    break;
                case 8:
                    clase = "line-scale-pulse-out-rapid";
                    break;
                case 9:
                    clase = "line-spin-fade-loader";
                    break;
                case 10:
                    clase = "square-spin";
                    break;
                case 11:
                    clase = "ball-clip-rotate";
                    break;
                case 12:
                    clase = "pacman";
                    break;
            }
            IconoCargando.CssClass = "loader-inner " + clase;
        }

        private void CerrarSesion()
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                // FormsAuthentication.SignOut();

                string nombreTerminal = string.Empty;
                try
                {
                    nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new char[] { '.' })[0].ToString());
                }
                catch (Exception)
                {
                    log.Warn(string.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
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

                // Limpiar variables de sesión
                LimpiarVariablesSesion();

                // Cerrar la autenticación
                FormsAuthentication.SignOut();

                // Limpiar las cookies
                LimpiarCookieSesion();

                // Limpiar la sesión
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();

                log.Info(string.Format("Usuario [{0}] ha cerrado sesión correctamente.", Session["Usuario"]));

                // Redirigir a la página de inicio de sesión
                Response.Redirect("~/Seguridad/IniciarSesion.aspx", true);
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