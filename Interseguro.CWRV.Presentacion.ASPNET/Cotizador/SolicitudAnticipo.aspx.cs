using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Cotizador
{
    public partial class SolicitudAnticipo : System.Web.UI.Page
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
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudAnticipo))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.SolicitudAnticipo.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }

        protected void GenerarAnticipo_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudAnticipo))
                    {
                        if (ValidarAnticipo())
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Anticipo anticipo = servicioCotizador.ObtenerDatosAnticipo(Solicitud.Text);

                            if (anticipo != null)
                            {
                                if (anticipo.Estado == Enums.EstadoAnticipo.Ingresado.StringValue())
                                {
                                    List<Agente> agentes = (List<Agente>)Session["ListaAgentes"];
                                    Agente agente = agentes.Find(a => a.Id == anticipo.Agente.Id);
                                    if (agente != null)
                                    {
                                        Session["Anticipo"] = anticipo;
                                        Response.Redirect("~/Cotizador/CondicionesAnticipo.aspx");
                                    }
                                    else
                                    {
                                        Session["Anticipo"] = null;
                                        MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { "El número de solicitud no corresponde a su cartera." });
                                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                                        MCMEstado.Value = "1";
                                    }
                                }
                                else
                                {
                                    Session["Anticipo"] = null;
                                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { "La solicitud de anticipo ya fue impresa (procesada) anteriormente." });
                                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    MCMEstado.Value = "1";
                                }
                            }
                            else
                            {
                                Session["Anticipo"] = null;
                                MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { "No se ha generado Solicitud de ACOM para ese número." });
                                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                                MCMEstado.Value = "1";
                            }
                        }
                    }
                    else
                    {
                        Session["Anticipo"] = null;
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.SolicitudAnticipo.StringValue()));
                        MCMEstadoIcono.Value = Constante.COD_ERROR;
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    Session["Anticipo"] = null;
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    Session["Anticipo"] = null;
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }

        private bool ValidarAnticipo()
        {
            bool esCorrecto = true;
            List<string> errores = new List<string>();

            MCMEstado.Value = "0";
            Solicitud.CssClass = "formTextbox";

            bool solicitud = (Solicitud.Text.Trim().Length > 0) ? true : false;

            if (!solicitud)
            {
                errores.Add("Debe ingresar un criterio de búsqueda.");
                Solicitud.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }

            esCorrecto = solicitud;

            if (!esCorrecto)
            {
                Session["Anticipo"] = null;
                MCMMensaje.Text = Utilitarios.FormatearError(errores);
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Validacion.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                MCMEstado.Value = "1";
                return false;
            }

            return true;
        }
    }
}