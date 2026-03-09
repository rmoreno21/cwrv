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
    public partial class CondicionesAnticipo : System.Web.UI.Page
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

                            Anticipo anticipo = (Anticipo)Session["Anticipo"];
                            if (anticipo != null)
                            {
                                // Si existe en Session, poblar normalmente
                                NumeroSolicitud.Value = anticipo.Solicitud.Id;
                                NombreAgente.Text = anticipo.Agente.Nombre;
                                IdAgente.Text = anticipo.Agente.Id;
                                MontoACOM.Text = String.Format("{0:0,0.00}", anticipo.Monto);
                                NombreAfiliado.Text = anticipo.Afiliado.Nombre;
                                CUSPP.Text = anticipo.Afiliado.CUSPP;
                                MesesIngreso.Text = anticipo.MesesIngreso + " mes" + (anticipo.MesesIngreso != 1 ? "es" : String.Empty);
                                MontoMaximo.Text = String.Format("{0:0,0.00}", anticipo.MontoMaximo);
                                DiasDevolucion.Text = anticipo.DiasDevolucion + " día" + (anticipo.DiasDevolucion != 1 ? "s" : String.Empty);
                                NombreAgente2.Text = anticipo.Agente.Nombre;
                                Fecha.Text = DateTime.Now.ToString("dd/MM/yyyy");
                            }
                            // Si anticipo es null, JavaScript se encargará de poblar desde sessionStorage
                        }
                        else
                        {
                            // Si el agente aceptó las condiciones en esta pantalla, bloquear los campos
                            // y mostrar el reporte de Solicitud de Anticipo
                            if (Acepto.Checked)
                            {
                                Acepto.Enabled = false;
                                GuardarSolicitud.Enabled = false;
                                GuardarSolicitud.CssClass = "botonDeshabilitado gris gris_sharp";
                                ImprimirFormato.Visible = true;
                            }
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.SolicitudAnticipo.StringValue()));
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

        protected void GuardarSolicitud_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudAnticipo))
                    {
                        if (ValidarConfirmacionAnticipo())
                        {
                            List<Agente> agentes = (List<Agente>)Session["ListaAgentes"];
                            Agente agente = agentes.First(a => a.Id == IdAgente.Text);
                            if (agente != null)
                            {
                                Anticipo anticipo = (Anticipo)Session["Anticipo"];
                                anticipo.Usuario = new Usuario { NombreUsuario = (string)Session["Usuario"] };

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                Respuesta respuesta = servicioCotizador.RegistrarAnticipoAceptacion(anticipo);
                                if (respuesta.Estado == Constante.COD_OK)
                                {
                                    Session["Anticipo"] = null;
                                }
                                else
                                {
                                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { respuesta.Mensaje });
                                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    MCMEstado.Value = "1";
                                }
                            }
                            else
                            {
                                MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { "El número de solicitud no corresponde a su cartera." });
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
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
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

        private bool ValidarConfirmacionAnticipo()
        {
            bool esCorrecto = true;
            List<string> errores = new List<string>();

            MCMEstado.Value = "0";

            bool aceptar = Acepto.Checked;

            if (!aceptar)
            {
                errores.Add("Debe aceptar las condiciones para seguir.");
                esCorrecto = false;
            }

            esCorrecto = aceptar;

            if (!esCorrecto)
            {
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