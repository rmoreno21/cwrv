using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Meler
{
    public partial class CotizarLote : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DescargaSolicitudes));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CotizarLote))
                {
                    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                        Enums.OpcionesSistema.CotizarLote.StringValue()));
                    Response.Redirect("~/Error/Permisos.aspx");
                }
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Error de comunicación: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                if (ex.InnerException != null)
                {
                    log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                }
                MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                MCMEstado.Value = "1";
            }
        }

        [WebMethod]
        public static Respuesta CargarTablaSolicitudesLote(string tokenUsuario, int lote)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesLote)pagina.LoadControl("~/Controles/TablaSolicitudesLote.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CotizarLote))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Solicitud> solicitudes = servicioCotizador.ListarSolicitudesPorLote(lote);

                            control.Solicitudes = solicitudes;
                            control.PermisoConsultar = true;
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message });
                    //respuesta.Controles = controles;
                    //throw (ex);
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta CotizarSolicitudLote(string tokenUsuario, string idSolicitud, string fechaCotizacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        DateTime fecCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE"));
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        //Solicitud solicitud = servicioCotizador.ObtenerDatosSolicitud(idSolicitud, fecCotizacion);

                        // Campos de auditoría
                        //solicitud.Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"], Rol = (string)HttpContext.Current.Session["RolAzman"] };

                        respuesta = servicioCotizador.CotizarOficial(idSolicitud, fecCotizacion, (string)HttpContext.Current.Session["Usuario"]);
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message });
                }
                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta ExportarLotePDF(string idSolicitud, string fecCotizacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));

                    HttpContext.Current.Session["idSolicitud"] = idSolicitud;
                    HttpContext.Current.Session["fecCotizacion"] = fechaCotizacion.ToString("yyyyMMdd");

                    respuesta.Estado = Constante.COD_OK;
                }
                catch (Exception ex)
                {
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = "Error";
                    respuesta.Icono = "error";
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }
    
        //<INIGTI_1092>
        [WebMethod]
        public static string SolicitudesHabilitadas(int lote, string solicitudes)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                string nroSolicitudes = "";
                try
                {
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    nroSolicitudes = servicioCotizador.SolicitudesHabilitadas(lote, solicitudes);
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                }
                return nroSolicitudes;
            }
        }
        //<FINGTI_1092>
    
    }
}