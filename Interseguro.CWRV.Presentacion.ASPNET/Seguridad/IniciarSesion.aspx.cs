﻿using System;
using System.Collections.Generic;
using System.Text;
using Jose;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Web.Security;
using System.Web.UI;
using System.Web.Services;
using System.Web;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloSeguridad;
using log4net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Interseguro.CWRV.Presentacion.ASPNET.Seguridad
{
    public partial class IniciarSesion : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IniciarSesion));
        private static IServicioAzman servicioAzman;
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {

                lbApiAutenticacionUrl.Text = ConfigurationManager.AppSettings["ApiAutenticacionUrl"];

                try
                {
                    VersionSistema.Text = string.Format("Versión {0}", Constante.VERSION_SISTEMA);
                    reCAPTCHASiteKey.Value = ConfigurationManager.AppSettings["reCAPTCHASiteKey"];

                    if (Request.QueryString["t"] != null)
                    {
                        // Se está intentando acceder al CWRV a través de un Token
                        string token = Request.QueryString["t"];

                        if (token.Length > 0)
                        {
                            // Validar si el Token recibido es válido
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();

                            SolicitudAcceso solicitud = servicioCotizador.ValidarToken(token);
                            if (solicitud != null)
                            {
                                // Validar que el Token esté vigente
                                if (solicitud.Vigente)
                                {
                                    // Expirar el token para que no pueda volver a ser usado
                                    solicitud.Vigente = false;
                                    servicioCotizador.ActualizarSolicitudAcceso(solicitud);

                                    // Brindar el acceso
                                    log.Debug(string.Format("Se va a conectar al Web Service para la autenticación del usuario [{0}].", solicitud.Usuario));

                                    servicioAzman = LocalizadorProxy.ObtenerServicioSeguridad();

                                    log.Debug(string.Format("Se va a consumir el método de autenticación para el usuario [{0}\\{1}] en la aplicación AZMAN[{2}].",
                                        ConfigurationManager.AppSettings["DominioRed"],
                                        solicitud.Usuario,
                                        ConfigurationManager.AppSettings["AplicacionAZMAN"]));

                                    BEOpcionMenu[] permisosUsuario =
                                                servicioAzman.ObtenerListaPermisos(
                                                    ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                                    ConfigurationManager.AppSettings["DominioRed"],
                                                    solicitud.Usuario,
                                                    Convert.ToInt32(ConfigurationManager.AppSettings["IdSistemaSeguridad"]));

                                    List<OpcionSistema> opcionesSistema = new List<OpcionSistema>();
                                    foreach (BEOpcionMenu opcion in permisosUsuario)
                                    {
                                        OpcionSistema o = new OpcionSistema
                                        {
                                            Id = opcion.CodigoOpcion,
                                            IdPadre = opcion.CodigoOpcionPadre.HasValue ? opcion.CodigoOpcionPadre.Value : 0,

                                            IdAzman = opcion.NroOperacionAzman,
                                            Nombre = opcion.NombreOperacionzman,
                                            Descripcion = opcion.DescripcionOperacionAzman,

                                            Orden = opcion.Orden,
                                            Ruta = opcion.Url,
                                            RutaIcono = opcion.RutaIcono,
                                            Titulo = opcion.Titulo,
                                            ToolTip = opcion.ToolTip,
                                            TipoOpcion = opcion.TipoOpcion,
                                            Activa = opcion.Visible
                                        };
                                        opcionesSistema.Add(o);
                                    }
                                    RestringirOpcionesFueraRed(opcionesSistema);
                                    Session["OpcionesSistema"] = opcionesSistema;

                                    // Crear token para la sesión
                                    Session["TokenUsuario"] = CryptorEngine.CrearTokenUsuario(solicitud.Usuario);

                                    BEUsuario datosUsuario =
                                        servicioAzman.ObtenerDatosUsuarioSinClave(
                                            ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                            ConfigurationManager.AppSettings["DominioRed"],
                                            solicitud.Usuario);

                                    // Guardar en sesión los datos del usuario y los permisos del mismo
                                    Session["NombreCompleto"] = datosUsuario.NombreCompleto;
                                    Session["Apellidos"] = datosUsuario.Apellidos;
                                    Session["Nombres"] = datosUsuario.Nombres;
                                    Session["IdEmpleado"] = datosUsuario.CodigoEmpleado;
                                    Session["CorreoElectronico"] = datosUsuario.Correo;
                                    Session["Dominio"] = datosUsuario.Dominio;
                                    Session["Estado"] = datosUsuario.Estado;
                                    Session["Matricula"] = datosUsuario.Matricula;
                                    Session["Cargo"] = datosUsuario.Rol;
                                    Session["RolAzman"] = datosUsuario.RolAzman;

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                    List<Parametro> parametroTabla = servicioCotizador.ObtenerParametrosPorTabla("");
                                    Session["ParametroTabla"] = parametroTabla;

                                    log.Info(string.Format("Usuario [{0}\\{1}] autenticado correctamente en [{2}].",
                                        ConfigurationManager.AppSettings["DominioRed"],
                                        solicitud.Usuario,
                                        ConfigurationManager.AppSettings["AplicacionAZMAN"]));

                                    string nombreTerminal = string.Empty;
                                    try
                                    {
                                        nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
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
                                        NombreUsuario = solicitud.Usuario,
                                        IdTipoEvento = Enums.EventoLog.IniciarSesion.StringValue()
                                    });

                                    // Obtener la lista de agentes sobre la que se tiene permisos
                                    List<Agente> listaAgentes = new List<Agente>();
                                    List<Agente> listaAgentesExternos = new List<Agente>();
                                    Session["NumAgente"] = "";

                                    if (datosUsuario.RolAzman == Enums.RolAzman.AgenteExterno.StringValue())
                                    {
                                        listaAgentesExternos = servicioCotizador.ListarAgenteExterno(solicitud.Usuario, solicitud.Usuario);
                                    }
                                    else
                                    {
                                        listaAgentes = servicioCotizador.ListarAgente(solicitud.Usuario, datosUsuario.RolAzman);
                                        listaAgentes.FindAll(p => p.Usuario == solicitud.Usuario).ForEach(p => Session["NumAgente"] = p.Id.ToString());
                                    }

                                    Session["ListaAgentesExternos"] = listaAgentesExternos;
                                    Session["ListaAgentes"] = listaAgentes;

                                    // Asignar el CUSPP que viene desde el CRM
                                    if (solicitud.CUSPP != string.Empty)
                                        Session["CUSPP"] = solicitud.CUSPP;

                                    // Se registra el script usando una técnica diferente

                                    FormsAuthentication.RedirectFromLoginPage(solicitud.Usuario, false);
                                }
                                else
                                {
                                    log.Error(string.Format("El Token [{0}] con el que se ha accedido ya ha expirado.", token));
                                    //ContenedorMensaje.CssClass = "grilla_error";
                                    //MensajeError.Text = "Enlace no válido o expirado.";
                                    //Formulario.Visible = false;
                                    //Mensajes.Visible = true;
                                }
                            }
                            else
                            {
                                log.Error(string.Format("El Token [{0}] con el que se ha accedido no ha sido generado por el CWRV.", token));
                                //ContenedorMensaje.CssClass = "grilla_error";
                                //MensajeError.Text = "Acceso denegado.";
                                //Formulario.Visible = false;
                                //Mensajes.Visible = true;
                            }
                        }
                        else
                        {
                            log.Error(string.Format("El Token [{0}] con el que se ha accedido es inválido.", token));
                        }
                    }
                    else
                    {

                        // Se está accediendo directamente
                        // Validar si el CWRV puede ser accedido directamente o si debe ser accedido a través del CWRV.
                        if (ConfigurationManager.AppSettings["DependenciaCRM"] == "S")
                        {
                            log.Warn(string.Format("Se ha intentado acceder directamente al Cotizador Web de Rentas desde la IP [{0}].", Request.ServerVariables["remote_addr"]));
                            //ContenedorMensaje.CssClass = "grilla_advertencia";
                            //MensajeError.Text = "El acceso al Cotizador Web de Rentas está restringido, para acceder al mismo debe hacerlo a través del sistema Customer Relationship Managment (CRM) de Rentas Vitalicias.";
                            //Formulario.Visible = false;
                            //Mensajes.Visible = true;
                        }
                        else if (ConfigurationManager.AppSettings["DependenciaCRM"] != "N")
                        {
                            log.Fatal("No se ha configurado correctamente la variable \"DependenciaCRM\" en el web.config de la Aplicación Web. Debe contener únicamente los valores \"S\" o \"N\".");
                            //ContenedorMensaje.CssClass = "grilla_error";
                            //MensajeError.Text = "No se ha configurado correctamente la conexión del Cotizador Web de Rentas con el sistema Customer Relationship Managment (CRM) de Rentas Vitalicias. Comuníquese con Soporte.";
                            //Formulario.Visible = false;
                            //Mensajes.Visible = true;
                        }
                        //else
                        //{
                        //    Formulario.Visible = true;
                        //    Mensajes.Visible = false;
                        //}
                    }
                }
                catch (FaultException ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    //MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    //MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    //MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    //MCMEstado.Value = "1";
                }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    //MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                    //MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    //MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    //MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    //MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    //MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    //MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    //MCMEstado.Value = "1";
                }
            }
        }

        private static void RestringirOpcionesFueraRed(List<OpcionSistema> opciones)
        {
            if (!Utilitarios.ValidarRedLocal(ObtenerRequestIp()))
            {
                // Las siguientes opciones no están disponibles fuera de la red de Interseguro
                opciones.FindAll(o => o.IdAzman == (int)Enums.OpcionesSistema.MenuMeler
                                   //|| o.IdAzman == (int)Enums.OpcionesSistema.CotizacionOficial //<INIGTI_1092><FINGTI_1092>
                                   || o.IdAzman == (int)Enums.OpcionesSistema.SolicitudAnticipo
                                   || o.IdAzman == (int)Enums.OpcionesSistema.PermisoACOM
                                   || o.IdAzman == (int)Enums.OpcionesSistema.PermisoDCOM
                                   || o.IdAzman == (int)Enums.OpcionesSistema.PermisoTRA
                                   || o.IdAzman == (int)Enums.OpcionesSistema.DescargaSolicitudes
                                   || o.IdAzman == (int)Enums.OpcionesSistema.CotizarLote
                                   || o.IdAzman == (int)Enums.OpcionesSistema.CargaConfirmaciones
                                   || o.IdAzman == (int)Enums.OpcionesSistema.CargaCotizacionesMeler
                                   || o.IdAzman == (int)Enums.OpcionesSistema.PermisoTRAPlus
                ).ForEach(o => o.Activa = false);
            }
        }

        public static string ObtenerRequestIp()
        {
            // Accede al HttpContext actual
            string ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            // Si estás detrás de un proxy, puedes intentar obtener la IP real desde un encabezado
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }

            // Si aún no obtienes la IP, puedes usar la propiedad de la solicitud directamente
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.UserHostAddress;
            }

            return ip;
        }

        [WebMethod]
        public static object ObtenerIpCliente()
        {
            var resultado = new Dictionary<string, object>();
            resultado["ip"] = ObtenerRequestIp();
            return resultado;
        }

        public static bool EliminarArchivosGoogleDrive()
        {
            bool resultado = false;

            try
            {
                log.Debug("[start] EliminarArchivosGoogleDrive");
                DateTime fecha = DateTime.Now;

                //HttpContext.Current.Server.MapPath("~/ArchivosTemporales/"
                string rutaArchivo = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\ArchivosTemporales";

                string[] Archivos = System.IO.Directory.GetFiles(rutaArchivo);
                string[] Carpetas = System.IO.Directory.GetDirectories(rutaArchivo);
                //consentimiento
                string[] Archivos_consentimiento = System.IO.Directory.GetFiles(rutaArchivo + "\\RVI\\Consentimiento");

                //Eliminando Archivos
                log.Debug("Eliminado Archivos Existentes");

                foreach (string Archivo in Archivos)
                {
                    DateTime modification = System.IO.File.GetLastWriteTime(Archivo);

                    int diferencia_dias = (fecha - modification).Days;

                    if (!Archivo.Contains("Consentimiento"))
                    {
                        if (diferencia_dias > 0)
                        {
                            System.IO.File.Delete(Archivo);
                        }
                        else
                        {
                            int diferencia_horas = (fecha - modification).Hours;
                            if (diferencia_horas > 1)
                            {
                                System.IO.File.Delete(Archivo);
                            }
                        }
                    }
                }

                foreach (string Carpeta in Carpetas)
                {
                    DateTime modification = System.IO.Directory.GetLastWriteTime(Carpeta);

                    int diferencia_dias = (fecha - modification).Days;

                    if (!Carpeta.Contains("RVI"))
                    {
                        if (diferencia_dias > 0)
                        {
                            System.IO.Directory.Delete(Carpeta, true);
                        }
                        else
                        {
                            int diferencia_horas = (fecha - modification).Hours;
                            if (diferencia_horas > 1)
                            {
                                System.IO.Directory.Delete(Carpeta, true);
                            }
                        }
                    }
                }

                foreach (string Archivo in Archivos_consentimiento)
                {
                    DateTime modification = System.IO.File.GetLastWriteTime(Archivo);

                    int diferencia_dias = (fecha - modification).Days;

                    if (diferencia_dias > 0)
                    {
                        System.IO.File.Delete(Archivo);
                    }
                    else
                    {
                        int diferencia_horas = (fecha - modification).Hours;
                        if (diferencia_horas > 1)
                        {
                            System.IO.File.Delete(Archivo);
                        }
                    }
                }

                log.Debug("[end] EliminarArchivosGoogleDrive");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return resultado = true;

        }

        public static void EliminarArchivosTemporales(string usuario)
        {
            try
            {
                log.Info($"[start][{usuario}] IniciarSesion#EliminarArchivosTemporales");
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                bool eliminarArchivos = servicioCotizador.EliminarParametrosGenerales();
                log.Info($"[result][{usuario}] IniciarSesion#EliminarArchivosTemporales#EliminarParametrosGenerales: [{eliminarArchivos}]");
                EliminarArchivosGoogleDrive();
                log.Info($"[result][{usuario}] IniciarSesion#EliminarArchivosTemporales#EliminarArchivosGoogleDrive: [{true}]");
                log.Info($"[end][{usuario}] IniciarSesion#EliminarArchivosTemporales");
            
            }
            catch (System.Exception ex)
            {
                log.Error($"[error][{usuario}] IniciarSesion#EliminarArchivosTemporales", ex);
            }
        }

        [WebMethod]
        public static object CrearSesion(
            BEUsuario usuario,
            List<OpcionSistema> opcionesSistema,
            List<Parametro> parametroTabla,
            List<Agente> listaAgentes,
            List<Agente> listaAgentesExternos,
            string numAgente
        )
        {
            var resultado = new Dictionary<string, object>();
            try
            {
                log.Info($"[start][{usuario.Matricula}] IniciarSesion#CrearSesion()");
                log.Info($"[executing][{usuario.Matricula}] IniciarSesion#CrearSesion#BEUsuario: [Matricula: {usuario.Matricula}]");
                log.Info($"[executing][{usuario.Matricula}] IniciarSesion#CrearSesion#BEUsuario: [RolAzman: {usuario.RolAzman}]");
                log.Info($"[executing][{usuario.Matricula}] IniciarSesion#CrearSesion#opcionesSistema: [{opcionesSistema.Count}]");
                log.Info($"[executing][{usuario.Matricula}] IniciarSesion#CrearSesion#parametroTabla: [{parametroTabla.Count}]");
                log.Info($"[executing][{usuario.Matricula}] IniciarSesion#CrearSesion#listaAgentes: [{listaAgentes.Count}]");
                log.Info($"[executing][{usuario.Matricula}] IniciarSesion#CrearSesion#listaAgentesExternos: [{listaAgentesExternos.Count}]");

                // Crear token de usuario
                var tokenUsuario = CryptorEngine.CrearTokenUsuario(usuario.Matricula);
                log.Info($"[executing][{usuario.Matricula}] IniciarSesion#CrearSesion#tokenUsuario: [{tokenUsuario}]");

                // Guardar datos en sesión
                HttpContext.Current.Session["TokenUsuario"] = tokenUsuario;
                HttpContext.Current.Session["NombreCompleto"] = usuario.NombreCompleto;
                HttpContext.Current.Session["Apellidos"] = usuario.Apellidos;
                HttpContext.Current.Session["Nombres"] = usuario.Nombres;
                HttpContext.Current.Session["IdEmpleado"] = usuario.CodigoEmpleado;
                HttpContext.Current.Session["CorreoElectronico"] = usuario.Correo;
                HttpContext.Current.Session["Dominio"] = usuario.Dominio;
                HttpContext.Current.Session["Estado"] = usuario.Estado;
                HttpContext.Current.Session["Matricula"] = usuario.Matricula;
                HttpContext.Current.Session["Cargo"] = usuario.Rol;
                HttpContext.Current.Session["RolAzman"] = usuario.RolAzman;
                HttpContext.Current.Session["Usuario"] = usuario.Matricula;
                HttpContext.Current.Session["OpcionesSistema"] = opcionesSistema;
                HttpContext.Current.Session["ListaAgentesExternos"] = listaAgentesExternos;
                HttpContext.Current.Session["ListaAgentes"] = listaAgentes;
                HttpContext.Current.Session["ParametroTabla"] = parametroTabla;
                HttpContext.Current.Session["NumAgente"] = numAgente;

                resultado["success"] = true;

                // Crear cookie de autenticación
                log.Info($"[executing][{usuario.Matricula}] IniciarSesion#CrearSesion#FormsAuthentication.SetAuthCookie({usuario.Matricula}, false)");
                FormsAuthentication.SetAuthCookie(usuario.Matricula, false);
                log.Info($"[end][{usuario.Matricula}] IniciarSesion#CrearSesion#FormsAuthentication.SetAuthCookie({usuario.Matricula}, false)");

                // Eliminar archivos temporales Asincrónicamente
                Task hilo = new Task(() =>
                {
                    EliminarArchivosTemporales(usuario.Matricula);
                });
                hilo.Start();

                log.Info($"[end][{usuario.Matricula}] IniciarSesion#CrearSesion()");
                return resultado;
            }
            catch (Exception ex)
            {
                log.Error($"[error][{usuario.Matricula}] IniciarSesion#CrearSesion()", ex);
                resultado["success"] = false;
                resultado["mensaje"] = "Error al crear los datos de la sesión";
                return resultado;
            }
        }
    }
}