using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaParticular
{
    public partial class SolicitudEvaluacion : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SolicitudEvaluacion));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ListadoCotizacionesEvaluacion))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));

                            string usuario = Session["Usuario"].ToString();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(Request.QueryString["s"]);
                            GrupoFamiliar afiliado = solicitud.Beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

                            NumeroSolicitud.Text = solicitud.Id;
                            CUSPP.Text = solicitud.Afiliado.CUSPP;
                            Titular.Text = string.Format("{0} {1} {2}", afiliado.Nombre, afiliado.ApellidoPaterno, afiliado.ApellidoMaterno);
                            EstadoOperaciones.Text = solicitud.EstadoSolicitud;
                            EstadoPLAFT.Text = solicitud.EstadoSolicitudPlaft;

                            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.AprobarFlujoSolicitud))
                            {
                                Aprobar.Visible = true;
                            }
                            else
                            {
                                Aprobar.Visible = false;
                            }

                            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ObservarFlujoSolicitud))
                            {
                                Observar.Visible = true;
                            }
                            else
                            {
                                Observar.Visible = false;
                            }

                            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.RechazarFlujoSolicitud))
                            {
                                Rechazar.Visible = true;
                            }
                            else
                            {
                                Rechazar.Visible = false;
                            }

                            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.AprobarFlujoSolicitud) ||
                                Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ObservarFlujoSolicitud) ||
                                Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.RechazarFlujoSolicitud))
                            {
                                SeccionObservacion.Visible = true;
                            }
                            else
                            {
                                SeccionObservacion.Visible = false;
                            }

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<FormatoSolicitud> formatos = servicioCotizador.ListarFormatosSolicitud(solicitud.Id, usuario);

                            //<INI.GTI_52310>
                            bool isDescargaFormatoEN = false;

                            var respuestaConsultaApiCwrvEdN = Utilitario.ConsultarEstudioNecesidadCWRV(Convert.ToDateTime(solicitud.FechaSolicitud), usuario);
                            if (respuestaConsultaApiCwrvEdN != null)
                            {
                                isDescargaFormatoEN = true;
                            }

                            //DocumentosFirmaDigitalHistorico.Titular = afiliado;
                            DocumentosFirmaDigitalHistorico.IndicadorDescargaFormatoEN = isDescargaFormatoEN;
                            //<FIN.GTI_52310>
                            DocumentosFirmaDigitalHistorico.Formatos = formatos;
                            DocumentosFirmaDigitalHistorico.DataBind();
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.ListadoCotizacionesEvaluacion.StringValue()));
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
        public static Respuesta ActualizarFlujoSolicitud(string tokenUsuario, string solicitud, string observacion, int codEstadoRPP)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, HttpContext.Current.Session["TokenUsuario"].ToString()))
                    {
                        string usuario = HttpContext.Current.Session["Usuario"].ToString();
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.ActualizarSolicitudOperaciones(solicitud, codEstadoRPP, observacion, usuario);
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0]", ex.Message), ex);
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = string.Format("Se ha producido un error al Actualizar el Flijo de la Solicitud {0}", solicitud);
                    return respuesta;
                }
            }
        }
    }
}