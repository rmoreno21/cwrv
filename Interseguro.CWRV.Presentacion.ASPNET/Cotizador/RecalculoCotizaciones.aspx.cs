using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;

namespace Interseguro.CWRV.Presentacion.ASPNET.Cotizador
{
    public partial class RecalculoCotizaciones : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Cotizador));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (!Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.RecalculoCotizaciones))
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.RecalculoCotizaciones.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
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

        [WebMethod]
        public static Solicitud ObtenerCotizacionRecalculo(string tokenUsuario, string numeroSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.RecalculoCotizaciones))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Solicitud solicitud = servicioCotizador.ObtenerCotizacionRecalculo(numeroSolicitud, (string)HttpContext.Current.Session["Usuario"]);
                            return solicitud;
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [RecalculoCotizaciones] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                            HttpContext.Current.Response.Status = "403 Forbidden";
                            HttpContext.Current.Response.StatusCode = 403;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return null;
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido un error al cargar los datos de la cotización: {0}", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Solicitud RecalcularCotizacion(string tokenUsuario, string numeroSolicitud, string fechaCotizacion, string montoCIC, string tipoCambio, string tipoCalculo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        DateTime fecCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE"));

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        Respuesta respuesta = servicioCotizador.CotizarRecalculo(numeroSolicitud, fecCotizacion, Convert.ToDouble(montoCIC, new CultureInfo("es-PE")), Convert.ToDouble(tipoCambio, new CultureInfo("es-PE")), tipoCalculo, (string)HttpContext.Current.Session["Usuario"]);

                        Solicitud sol;
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        sol = servicioCotizador.ObtenerDatosSolicitud(numeroSolicitud, fecCotizacion);
                        return sol;
                    }
                    else
                    {
                        throw new Exception("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido un error al recalcular la cotización: {0}", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static byte[] ReporteRecalculo(string tokenUsuario, string numeroSolicitud, int correlativo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                { 
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        List<ReporteRecalculoCotizacion> reportes = new List<ReporteRecalculoCotizacion>();
                        ReporteRecalculoCotizacion reporte = new ReporteRecalculoCotizacion
                        {
                            num_solicitud = numeroSolicitud,
                            reporteRecalculoCotizacionDetalle = new ReporteRecalculoCotizacionDetalle
                            {
                                num_correlativo = correlativo
                            }
                        };
                        reportes.Add(reporte);
                        return servicioCotizador.ObtenerReporteRecalculoPDF(reportes);
                    }
                    else
                    {
                        throw new Exception("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido un error al cargar los datos de la cotización: {0}", ex.Message), ex);
                    throw (ex);
                }
            }
        }
    }
}