using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Threading;

using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;

using Microsoft.Reporting.WebForms;

using log4net;

namespace Interseguro.CWRV.Presentacion.ASPNET.Bandejas
{
    public partial class BandejaFlujoHistorial : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BandejaFlujoHistorial));
        private static IServicioCWRV servicioCotizador;

        private static SolicitudEscenario solEscenario;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteSeguimiento))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));

                            TablaFlujoSolError.Visible = false;
                            ModFlujoSolCargando.Visible = true;
                            TablaFlujoCotizacionesContenedor.Visible = false;

                            SolicitudEscenario solEscenario = new SolicitudEscenario();
                            
                            solEscenario= (SolicitudEscenario) Session["SolicitudEscenarioMovil"];

                            if (solEscenario != null)
                            {
                                var data = CargarTablaCotizacionesMovimiento(solEscenario.NumSolicitud);

                                
                                TablaFlujoCotizacionesContenedor.Controls.Add(new LiteralControl(data));
                                //LimpiarFormularios();
                                TablaFlujoSolError.Visible = false;
                                ModFlujoSolCargando.Visible = false;
                                TablaFlujoCotizacionesContenedor.Visible = true;

                            }
                            



                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ReporteSeguimiento.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
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
                    TablaFlujoSolError.Visible = true;
                    ModFlujoSolCargando.Visible = false;
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
                    TablaFlujoSolError.Visible = true;
                    ModFlujoSolCargando.Visible = false;
                }
            }
        }

        [WebMethod]
        public static string CargarTablaCotizacionesMovimiento(string numSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();
                    var control = (TablaCotizacionesMovimiento)pagina.LoadControl("~/Controles/TablaCotizacionesMovimiento.ascx");

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<CotizacionMovimiento> cotizacionMovimiento = servicioCotizador.ObtenerCotizacionTipoMovimientoPorSolicitud(numSolicitud);

                    control.cotizacionMovimiento = cotizacionMovimiento;

                    pagina.Controls.Add(control);

                    string html = "";
                    using (var sw = new StringWriter())
                    {
                        HttpContext.Current.Server.Execute(pagina, sw, false);
                        html = sw.ToString();
                    }
                    return html;
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
                    throw (ex);
                }
            }
        }

    }
}