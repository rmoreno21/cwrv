using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Reflection;
using System.Net;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Dominio.Entidades;
using log4net;
using System.IO;
using System.Globalization;
using System.Web.Configuration;
using System.Configuration;

namespace Interseguro.CWRV.Presentacion.ASPNET
{
    public partial class CotWebRVI : System.Web.UI.MasterPage
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CotWebRVI));
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

                if (!IsPostBack)
                {
                    TokenUsuario.Value = (string)Session["TokenUsuario"];
                    if (Session["OpcionesSistema"] != null)
                        MenuPrincipal.ListaOpciones = ((List<OpcionSistema>)Session["OpcionesSistema"]).FindAll(o => o.TipoOpcion == "M");

                    if (Session["Consentimiento"] != null)
                    {
                        if ((bool)Session["Consentimiento"])
                        {
                            CabeceraSuperior.Visible = true;
                            CabeceraSuperiorProtegida.Visible = false;
                        }
                        else
                        {
                            CabeceraSuperior.Visible = false;
                            CabeceraSuperiorProtegida.Visible = true;
                        }
                    }
                }
                else
                {
                    //if (!String.Equals(TokenUsuario.Value, (string)Session["TokenUsuario"]))
                    //{
                    //    log.Info("La sesión ha expirado. Se va a proceder a cerrar la sesión.");
                    //    Response.Redirect("~/Seguridad/CerrarSesion.aspx");
                    //}
                }

                HttpCookie loginCookie = Request.Cookies["CotWebRVICookie"];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(loginCookie.Value);

                Session["Usuario"] = ticket.Name;

                //<SRIINI17003>
                Configuration conf = WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                SessionStateSection section = (SessionStateSection)conf.GetSection("system.web/sessionState");
                int timeout = (int)section.Timeout.TotalMilliseconds;
                //<INIGTI_7012>
                if (timeout > 120000)
                    timeout = timeout - 120000;

                //<FINGTI_7012>

                Timeout.Value = timeout.ToString();
                //<SRIFIN17003>

                //<GTIINI-753>
                string rolUsuario = String.Empty;

                if (Session["RolAzman"] != null)
                {
                    rolUsuario = Session["RolAzman"].ToString();
                }
                else
                {
                    Response.Redirect("~/Seguridad/CerrarSesion.aspx", true);
                }
                //<GTIINI-753>

                CabUsuario.Text = ticket.Name + " (" + rolUsuario + ")";
                if (Session["NombreCompleto"] != null)
                {
                    CabNombre.Text = Session["NombreCompleto"].ToString();
                }
                else
                {
                    log.Info("La sesión ha expirado. Se va a proceder a cerrar la sesión.");
                    CerrarSesion();
                }

                DateTime fechaActual = DateTime.Now;

                CabFecha.Text = fechaActual.ToString("dd/MM/yyyy");
                FechaProtegido.Text = String.Format("{0}, {1} de {2} del {3}", Utilitarios.PrimeraMayuscula(fechaActual.ToString("dddd", CultureInfo.CreateSpecificCulture("es-PE"))), fechaActual.ToString("dd"), fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es-PE")), fechaActual.ToString("yyyy"));

                // Ocultar los botones de Video Tutorial y Manual de usuario y restringirlos sólo a la red de interseguro
                bool redLocal = Utilitarios.ValidarRedLocal(Request.ServerVariables["remote_addr"]);
                if (!redLocal || !Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.VideoTutorial)) MenuTutorial.Visible = false;
                if (!redLocal || !Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ManualUsuario)) MenuManual.Visible = false;

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

        protected void MenuSalir_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Info("Usuario seleccionó opción Cerrar Sesión.");
                CerrarSesion();
            }
        }

        private void CerrarSesion()
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                // FormsAuthentication.SignOut();
                
                
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

                // Limpiar variables de sesión
                LimpiarVariablesSesion();

                // Cerrar la autenticación
                FormsAuthentication.SignOut();

                // Limpiar la sesión
                Session.Clear();
                Session.Abandon();
                Session.RemoveAll();

                // Limpiar las cookies
                LimpiarCookieSesion();

                log.Info(String.Format("Usuario [{0}] ha cerrado sesión correctamente.", Session["Usuario"]));

                // Redirigir a la página de inicio de sesión
                Response.Redirect("~/Seguridad/IniciarSesion.aspx", true);
            }
        }

        protected void MenuTutorial_Click(object sender, EventArgs e)
        {
            using (FileStream fs = File.OpenRead(Server.MapPath("~/Recursos/Flujo_CWRV_Fase1.wmv")))
            {
                int length = (int)fs.Length;
                byte[] buffer;

                using (BinaryReader br = new BinaryReader(fs))
                {
                    buffer = br.ReadBytes(length);
                }

                Response.Clear();
                Response.ClearHeaders();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=CWRV_Tutorial.wmv");
                Response.ContentType = "video/x-ms-wmv";
                Response.BinaryWrite(buffer);
                Response.Flush();
                Response.End();
            }
        }

        protected void MenuManual_Click(object sender, EventArgs e)
        {
            using (FileStream fs = File.OpenRead(Server.MapPath("~/Recursos/GuiaUsuario.pdf")))
            {
                int length = (int)fs.Length;
                byte[] buffer;

                using (BinaryReader br = new BinaryReader(fs))
                {
                    buffer = br.ReadBytes(length);
                }

                Response.Clear();
                Response.ClearHeaders();
                Response.Buffer = true;
                Response.AddHeader("Content-Disposition", "attachment;filename=CWRV_GuiaUsuario.pdf");
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(buffer);
                Response.Flush();
                Response.End();
            }
        }

        protected void MenuInicio_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Principal.aspx", true);
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