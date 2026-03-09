using System;
using System.Collections.Generic;
using System.Linq;

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;
using Interseguro.CWRV.Infraestructura.General;
using System.Threading;
using System.ServiceModel;
using System.Configuration;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Dominio.Entidades;
using System.Web.Services;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using System.IO;
using System.Xml.Linq;
using System.Globalization;
using System.Net;
using Microsoft.Reporting.WebForms;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters.Binary;

namespace Interseguro.CWRV.Presentacion.ASPNET.Simuladores
{
    public partial class QueMeConviene : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(QueMeConviene));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorQueMeConviene))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                            LimpiarFormularios();
                            CargarInformacionPredeterminada();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.SimuladorQueMeConviene.StringValue()));
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
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
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
                }
            }
        }

        private void CargarInformacionPredeterminada()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> parametros = servicioCotizador.ObtenerParametrosSimuladores();

            PromedioRentabilitad.Text = parametros[1].Valor.ToString();
            Inflacion.Text = parametros[2].Valor.ToString();
            TasaAjuste.Text = parametros[3].Valor.ToString();
            TipoCambio.Text = parametros[0].Valor.ToString();

            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
            {
                BusAfiCUSPP.Text = Session["CUSPP"].ToString();
                HBusAfiCUSPP.Value = Session["CUSPP"].ToString();
                BusAfiDatosCargados.Value = "1";
            }
            else if (Session["CUSPP"] == null && Session["NroSolicitud"] != null)
            {
                BusAfiNroSolicitud.Text = Session["NroSolicitud"].ToString();
                HBusAfiNroSolicitud.Value = Session["NroSolicitud"].ToString();
                BusAfiDatosCargados.Value = "1";
            }
        }

        private void LimpiarFormularios()
        {
            CUSPP.Value = String.Empty;

            PromedioRentabilitad.Text = String.Empty;
            Inflacion.Text = String.Empty;
            TasaAjuste.Text = String.Empty;
            TipoCambio.Text = String.Empty;

            PromedioRentabilitad.CssClass = PromedioRentabilitad.CssClass.Replace(" formTextboxError", String.Empty);
            Inflacion.CssClass = Inflacion.CssClass.Replace(" formTextboxError", String.Empty);
            TasaAjuste.CssClass = TasaAjuste.CssClass.Replace(" formTextboxError", String.Empty);
            TipoCambio.CssClass = TipoCambio.CssClass.Replace(" formTextboxError", String.Empty);
        }

        private void CargarInformacionInicialPantalla()
        {
            IdSimulador.Value = ((int)Enums.OpcionesSistema.SimuladorQueMeConviene).ToString();

            if (Session["Consentimiento"] != null)
                FormularioBusqueda.Visible = (bool)Session["Consentimiento"];

            // Cargar información de los Combobox
            //Modalidad.Items.Add(new ListItem("Inmediata", Enums.Modalidad.Inmediata.StringValue()));
            //Modalidad.Items.Add(new ListItem("Diferida a 1 año", Enums.Modalidad.Diferida.StringValue() + "1"));
            //Modalidad.Items.Add(new ListItem("Diferida a 2 años", Enums.Modalidad.Diferida.StringValue() + "2"));
            //Modalidad.Items.Add(new ListItem("Diferida a 3 años", Enums.Modalidad.Diferida.StringValue() + "3"));
            //Modalidad.Items.Add(new ListItem("Diferida a 4 años", Enums.Modalidad.Diferida.StringValue() + "4"));
            //Modalidad.Items.Add(new ListItem("Diferida a 5 años", Enums.Modalidad.Diferida.StringValue() + "5"));

            //PeriodoGarantizado.Items.Add("0");
            //PeriodoGarantizado.Items.Add("10");
            //PeriodoGarantizado.Items.Add("15");

            // Permisos Modal Búsqueda de Afiliados
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.BusquedaAfiliadoConsultar))
            {
                PerBusAfiExaminarSolicitud.Value = "1";
            }
            else
            {
                InhabilitarControl(BusAfiExaminarSolicitud);
                InhabilitarControl(ModBusAfiApellidoPaterno);
                InhabilitarControl(ModBusAfiApellidoMaterno);
                InhabilitarControl(ModBusAfiNombres);
                InhabilitarControl(ModBusAfiBuscar);
                PerBusAfiExaminarSolicitud.Value = "0";
            }

            // Permisos Consultar Afiliado
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
            {
                PerBusAfiBuscar.Value = "1";
            }
            else
            {
                InhabilitarControl(BusAfiNroSolicitud);
                InhabilitarControl(BusAfiCUSPP);
                InhabilitarControl(SimBusAfiBuscar);
                PerBusAfiBuscar.Value = "0";
            }
        }

        public void InhabilitarControl(Control control)
        {
            if (control is TextBox)
            {
                ((TextBox)control).ReadOnly = true;
                ((TextBox)control).CssClass = "formTextbox formTextboxReadOnly";
            }
            if (control is DropDownList)
            {
                ((DropDownList)control).Enabled = false;
                ((DropDownList)control).CssClass = "formCombobox formComboboxReadOnly";
            }
            if (control is HyperLink)
            {
                ((HyperLink)control).CssClass = "botonDeshabilitado gris gris_sharp";
                ((HyperLink)control).ToolTip = ConfigurationManager.AppSettings["MensajeSinPermisos"];
            }
            if (control is Button)
            {
                ((Button)control).CssClass = "botonDeshabilitado gris gris_sharp";
                ((Button)control).ToolTip = ConfigurationManager.AppSettings["MensajeSinPermisos"];
            }
        }


        [WebMethod]
        public static Respuesta SimularQueMeConviene(string tokenUsuario, string idSolicitud, DateTime fechaCotizacion,
                                                     DateTime fechaDevengue, List<Cotizacion> cotizaciones, List<GrupoFamiliar> beneficiarios,
                                                     double cic, double tipoCambio, double rentabilidadAFP, double ipc, double ajuste,
                                                     Int64? correlativo1, Int64? correlativo2, Int64? correlativo3)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();

                        SimuladorQueMeConviene control = (SimuladorQueMeConviene)pagina.LoadControl("~/Controles/SimuladorQueMeConviene.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorQueMeConviene))
                        {
                            control.PermisoEjecutar = true;

                            Cotizacion cotizacionCIA;
                            Cotizacion cotizacionAFP;
                            ParametroCotizadorWS parametrosAFP;

                            if (correlativo1 != null)
                            {
                                cotizacionCIA = new Cotizacion();
                                cotizacionAFP = new Cotizacion();
                                parametrosAFP = new ParametroCotizadorWS();

                                //cotizacionCIA = cotizaciones.Find(c => c.Correlativo == correlativo1);
                                cotizacionCIA = cotizaciones.Where(c => c.Correlativo == correlativo1).Select(c => new Cotizacion()
                                {
                                    Correlativo = c.Correlativo,
                                    Moneda = c.Moneda,
                                    Producto = c.Producto,
                                    Modalidad = c.Modalidad,
                                    PeriodoDiferido = c.PeriodoDiferido,
                                    PorcentajeEntreRentas = c.PorcentajeEntreRentas,
                                    PeriodoGarantizado = c.PeriodoGarantizado,
                                    DerechoCrecer = c.DerechoCrecer,
                                    Gratificacion = c.Gratificacion,
                                    Capital = c.Capital,
                                    AjusteTRA = c.AjusteTRA,
                                    MontoCia = c.MontoCia,
                                    PensionCia = c.PensionCia,
                                    PensionCiaMO = c.PensionCiaMO,
                                    PuurCia = c.PuurCia,
                                    TasaAFP = c.TasaAFP,
                                    MontoAFP = c.MontoAFP,
                                    PensionAFP = c.PensionAFP,
                                    PuurAFP = c.PuurAFP,
                                    TasaVenta = c.TasaVenta,
                                    TasaVentaSbs = c.TasaVentaSbs,
                                    PrimeraPensionRVD = c.PrimeraPensionRVD //<INIGTI_2145>
                                }).First();

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                //<INIGTI_754>
                                //parametrosAFP = servicioCotizador.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, correlativo1, 0)[0];
                                parametrosAFP = servicioCotizador.ObtenerParametroCotizacionWS(idSolicitud, fechaCotizacion, correlativo1, 0);
                                //<FINGTI_754>
                                CalcularPension(ref control, ref cotizacionCIA, ref cotizacionAFP, parametrosAFP,
                                                fechaCotizacion, fechaDevengue, cic, tipoCambio, rentabilidadAFP, ipc, ajuste, Enums.OrdenModalidad.Modalidad_1.StringValue());



                                if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue())
                                {
                                    respuesta.Grafico1 = control.GraficoCIA;
                                    respuesta.Grafico4 = null;
                                }
                                //<INIGTI_XXX>
                                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                {
                                    respuesta.Grafico1 = control.GraficoCIA;
                                    respuesta.Grafico4 = null;
                                }
                                //<FINGTI_XXX>
                                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                                {
                                    respuesta.Grafico1 = control.GraficoCIA;
                                    respuesta.Grafico4 = null;
                                }
                                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Mixta.StringValue()
                                    || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Combinada.StringValue()
                                    || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue())
                                {
                                    respuesta.Grafico1 = control.GraficoAFP;
                                    respuesta.Grafico4 = control.GraficoCIA;
                                }

                            }

                            if (correlativo2 != null)
                            {
                                cotizacionCIA = new Cotizacion();
                                cotizacionAFP = new Cotizacion();
                                parametrosAFP = new ParametroCotizadorWS();

                                //cotizacionCIA = cotizaciones.Find(c => c.Correlativo == correlativo2);
                                cotizacionCIA = cotizaciones.Where(c => c.Correlativo == correlativo2).Select(c => new Cotizacion()
                                {
                                    Correlativo = c.Correlativo,
                                    Moneda = c.Moneda,
                                    Producto = c.Producto,
                                    Modalidad = c.Modalidad,
                                    PeriodoDiferido = c.PeriodoDiferido,
                                    PorcentajeEntreRentas = c.PorcentajeEntreRentas,
                                    PeriodoGarantizado = c.PeriodoGarantizado,
                                    DerechoCrecer = c.DerechoCrecer,
                                    Gratificacion = c.Gratificacion,
                                    Capital = c.Capital,
                                    AjusteTRA = c.AjusteTRA,
                                    MontoCia = c.MontoCia,
                                    PensionCia = c.PensionCia,
                                    PensionCiaMO = c.PensionCiaMO,
                                    PuurCia = c.PuurCia,
                                    TasaAFP = c.TasaAFP,
                                    MontoAFP = c.MontoAFP,
                                    PensionAFP = c.PensionAFP,
                                    PuurAFP = c.PuurAFP,
                                    TasaVenta = c.TasaVenta,
                                    TasaVentaSbs = c.TasaVentaSbs,
                                    PrimeraPensionRVD = c.PrimeraPensionRVD //<INIGTI_2145>
                                }).First();

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                //<INIGTI_754>
                                //parametrosAFP = servicioCotizador.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, correlativo2, 0)[0];
                                parametrosAFP = servicioCotizador.ObtenerParametroCotizacionWS(idSolicitud, fechaCotizacion, correlativo2, 0);
                                //<FINGTI_754>

                                CalcularPension(ref control, ref cotizacionCIA, ref cotizacionAFP, parametrosAFP,
                                                fechaCotizacion, fechaDevengue, cic, tipoCambio, rentabilidadAFP, ipc, ajuste, Enums.OrdenModalidad.Modalidad_2.StringValue());



                                if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue())
                                {
                                    respuesta.Grafico2 = control.GraficoCIA;
                                    respuesta.Grafico5 = null;
                                }
                                //<INIGTI_XXX>
                                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                {
                                    respuesta.Grafico2 = control.GraficoCIA;
                                    respuesta.Grafico5 = null;
                                }
                                //<FINGTI_XXX>
                                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                                {
                                    respuesta.Grafico2 = control.GraficoCIA;
                                    respuesta.Grafico5 = null;
                                }
                                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Mixta.StringValue()
                                    || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Combinada.StringValue()
                                    || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue())
                                {
                                    respuesta.Grafico2 = control.GraficoAFP;
                                    respuesta.Grafico5 = control.GraficoCIA;
                                }

                            }

                            if (correlativo3 != null)
                            {
                                cotizacionCIA = new Cotizacion();
                                cotizacionAFP = new Cotizacion();
                                parametrosAFP = new ParametroCotizadorWS();

                                //cotizacionCIA = cotizaciones.Find(c => c.Correlativo == correlativo3);
                                cotizacionCIA = cotizaciones.Where(c => c.Correlativo == correlativo3).Select(c => new Cotizacion()
                                {
                                    Correlativo = c.Correlativo,
                                    Moneda = c.Moneda,
                                    Producto = c.Producto,
                                    Modalidad = c.Modalidad,
                                    PeriodoDiferido = c.PeriodoDiferido,
                                    PorcentajeEntreRentas = c.PorcentajeEntreRentas,
                                    PeriodoGarantizado = c.PeriodoGarantizado,
                                    DerechoCrecer = c.DerechoCrecer,
                                    Gratificacion = c.Gratificacion,
                                    Capital = c.Capital,
                                    AjusteTRA = c.AjusteTRA,
                                    MontoCia = c.MontoCia,
                                    PensionCia = c.PensionCia,
                                    PensionCiaMO = c.PensionCiaMO,
                                    PuurCia = c.PuurCia,
                                    TasaAFP = c.TasaAFP,
                                    MontoAFP = c.MontoAFP,
                                    PensionAFP = c.PensionAFP,
                                    PuurAFP = c.PuurAFP,
                                    TasaVenta = c.TasaVenta,
                                    TasaVentaSbs = c.TasaVentaSbs,
                                    PrimeraPensionRVD = c.PrimeraPensionRVD //<INIGTI_2145>
                                }).First();

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                //<INIGTI_754>
                                //parametrosAFP = servicioCotizador.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, correlativo3, 0)[0];
                                parametrosAFP = servicioCotizador.ObtenerParametroCotizacionWS(idSolicitud, fechaCotizacion, correlativo3, 0);
                                //<FINGTI_754>
                                CalcularPension(ref control, ref cotizacionCIA, ref cotizacionAFP, parametrosAFP,
                                                fechaCotizacion, fechaDevengue, cic, tipoCambio, rentabilidadAFP, ipc, ajuste, Enums.OrdenModalidad.Modalidad_3.StringValue());



                                if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue())
                                {
                                    respuesta.Grafico3 = control.GraficoCIA;
                                    respuesta.Grafico6 = null;
                                }
                                //<INIGTI_XXX>
                                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                {
                                    respuesta.Grafico3 = control.GraficoCIA;
                                    respuesta.Grafico6 = null;
                                }
                                //<FINGTI_XXX>
                                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                                {
                                    respuesta.Grafico3 = control.GraficoCIA;
                                    respuesta.Grafico6 = null;
                                }
                                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Mixta.StringValue()
                                    || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Combinada.StringValue()
                                    || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue())
                                {
                                    respuesta.Grafico3 = control.GraficoAFP;
                                    respuesta.Grafico6 = control.GraficoCIA;
                                }
                            }

                        }
                        else
                        {
                            control.PermisoEjecutar = false;
                        }

                        string nombreTerminal = String.Empty;
                        try
                        {
                            nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                        }
                        catch (Exception)
                        {
                            log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                HttpContext.Current.Request.ServerVariables["remote_addr"]));
                        }

                        nombreTerminal += HttpContext.Current.Request.UserAgent;

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        servicioCotizador.RegistrarLog(new LogBD
                        {
                            IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                            NombreTerminal = nombreTerminal,
                            IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                            NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                            IdTipoEvento = Enums.EventoLog.Reporte2.StringValue(),
                            Detalle = String.Format("Cotizaciones Nodalidad 1 [{0}] Nodalidad 2 [{1}] Nodalidad 3 [{2}] simuladas", correlativo1, correlativo2, correlativo3)
                        });

                        pagina.Controls.Add(control);

                        string html = "";
                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            html = sw.ToString();
                        }
                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Contenido = html;

                        respuesta.FechaHora = DateTime.Now;

                        HttpContext.Current.Session["CntSimulador"] = control;

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (FaultException ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { String.Format("La solicitud [{0}] no contiene los datos suficientes para generar la simulación, por favor intente generando una nueva cotización.", idSolicitud) });
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

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }
                return respuesta;
            }
        }


        public static void CalcularPension(ref SimuladorQueMeConviene control, ref Cotizacion cotizacionCIA, ref Cotizacion cotizacionAFP,
                                           ParametroCotizadorWS parametrosAFP, DateTime fechaCotizacion, DateTime fechaDevengue, double cic,
                                           double tipoCambio, double rentabilidadAFP, double ipc, double ajuste, string ordenModalidad)
        {


            double pensionAnhoCIA = 0;
            double pensionAnhoAFP = 0;
            double acumuladoCIA = 0;
            double acumuladoAFP = 0;
            //double saldo = cic;
            double saldo = cotizacionCIA.MontoAFP;
            double rentabilidad = 0;
            double tasaAjuste = 0;

            double acumuladoDevengue = 0;

            int aniosDiferido = 0;

            // Calcular si hay devengue
            DateTime fechaCero = new DateTime(1, 1, 1);
            int diferencia = -1;
            if (fechaCotizacion > fechaDevengue)
                diferencia = fechaCotizacion.Subtract(fechaDevengue).Days / 365;
            if (diferencia == 0)
                diferencia = -1;

            control.GraficoCIA = new List<GraficoLineal>();
            control.GraficoAFP = new List<GraficoLineal>();

            // Redondear las pensiones
            cotizacionCIA.PensionCia = Math.Round(cotizacionCIA.PensionCia, 2);
            cotizacionCIA.PensionCiaMO = Math.Round(cotizacionCIA.PensionCiaMO, 2);
            cotizacionCIA.PensionAFP = Math.Round(cotizacionCIA.PensionAFP, 2);

            //Inicio Presente (Año 0)
            //INICIO CÁLCULO CIA
            if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Mixta.StringValue()
                || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Combinada.StringValue()
                //<INIGTI_754> || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue()
                )
            {

                if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()) tasaAjuste = ipc;
                else if (cotizacionCIA.Moneda.Id == Enums.Moneda.Dolares.StringValue()) tasaAjuste = 1;
                else if (cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()
                    || cotizacionCIA.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue()) tasaAjuste = ajuste;

                //control.GraficoCIA = new List<GraficoLineal>();

                // Presente (Año 0)
                if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                    || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                {
                    control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionCiaMO });
                }
                else
                {
                    control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionCiaMO * tipoCambio });
                }

                pensionAnhoCIA = cotizacionCIA.PensionCiaMO * 12;
                acumuladoCIA += pensionAnhoCIA;

            }
            else
            {
                if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()) tasaAjuste = ipc;
                else if (cotizacionCIA.Moneda.Id == Enums.Moneda.Dolares.StringValue()) tasaAjuste = 1;
                else if (cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()
                    || cotizacionCIA.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue()) tasaAjuste = ajuste;

                //control.GraficoCIA = new List<GraficoLineal>();

                // Presente (Año 0)
                if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                {
                    control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionAFP });
                    pensionAnhoCIA = cotizacionCIA.PensionAFP * 12;
                }
                else
                {
                    control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionCia });
                    pensionAnhoCIA = cotizacionCIA.PensionCia * 12;
                }

                acumuladoCIA += pensionAnhoCIA;
            }
            //FIN CÁLCULO CIA


            //INICIO CÁLCULO AFP
            if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Mixta.StringValue()
                || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Combinada.StringValue()
                //<INIGTI_754> || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue()<FINGTI_754>
                )
            {

                //servicioCotizador = LocalizadorProxy.ObtenerServicio();
                //ParametroCotizadorWS parametrosAFP = servicioCotizador.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, correlativo1, 0)[0];

                // Modificación de parámetros para obtener la Pensión de la AFP
                parametrosAFP.cot_num_tcal = 6;
                parametrosAFP.cot_tas_tasa = parametrosAFP.cot_tas_tafp;
                XDocument xmlAsh = XDocument.Parse(parametrosAFP.cot_xml_parash);
                var queryAsh = from c in xmlAsh.Elements("PARASH").Elements("Registro").Elements("val_cmor")
                               select c;
                foreach (XElement cmor in queryAsh)
                {
                    cmor.Value = "0";
                }
                parametrosAFP.cot_xml_parash = xmlAsh.ToString(SaveOptions.DisableFormatting);

                XElement fluaju = new XElement("FLUAJU");
                for (int i = 0; i <= 1320; i++)
                {
                    XElement registro =
                        new XElement("Registro",
                            new XElement("indice", i.ToString()),
                            new XElement("val_ajuste", "1")
                        );
                    fluaju.Add(registro);
                }
                parametrosAFP.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);

                parametrosAFP.cot_val_puam = saldo + rentabilidad;
                parametrosAFP.cot_val_puni = saldo + rentabilidad;

                cotizacionAFP = servicioCotizador.CotizarConParametros(parametrosAFP);

                //control.GraficoAFP = new List<GraficoLineal>();

                // Presente (Año 0)
                control.GraficoAFP.Add(new GraficoLineal { Nombre = "RETIRO PROGRAMADO", Valor = cotizacionAFP.PensionCia });

                if (diferencia < 0)
                {
                    pensionAnhoAFP = cotizacionAFP.PensionCia * 12;
                    acumuladoAFP += pensionAnhoAFP;
                    saldo -= pensionAnhoAFP;
                    rentabilidad = saldo * (rentabilidadAFP / 100);
                }
                else
                {
                    acumuladoDevengue += cotizacionAFP.PensionCia * 12;
                    acumuladoAFP += acumuladoDevengue;
                    diferencia--;
                    if (diferencia < 0)
                    {
                        saldo -= acumuladoDevengue;
                        rentabilidad = saldo * (rentabilidadAFP / 100);
                    }
                }
            }
            //<INIGTI_754>
            else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue())
            {
                //cotizacionAFP.PensionCia
                cotizacionAFP.PensionCia = cotizacionCIA.PensionAFP;
                control.GraficoAFP.Add(new GraficoLineal { Nombre = "RETIRO PROGRAMADO", Valor = cotizacionAFP.PensionCia });
                pensionAnhoAFP = cotizacionAFP.PensionCia * 12;
                acumuladoAFP += pensionAnhoAFP;
            }
            //<FINGTI_754>

            //FIN CÁLCULO AFP
            //Fin Presente (Año 0)



            //Inicio años Diferidos
            if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
            {
                aniosDiferido = (int)cotizacionCIA.PeriodoDiferido;

                if (aniosDiferido > 1)
                {

                    for (int i = 2; i <= aniosDiferido; i++)
                    {
                        //Inicio calculo CIA

                        cotizacionCIA.PensionCia = Math.Round(cotizacionCIA.PensionCia * (1 + tasaAjuste / 100), 2);

                        control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionAFP });

                        pensionAnhoCIA = cotizacionCIA.PensionAFP * 12;
                        acumuladoCIA += pensionAnhoCIA;

                        //Fin calculo CIA

                        if (diferencia < 0)
                        {
                            // Aumentar un año a los beneficiarios
                            XDocument xml = XDocument.Parse(parametrosAFP.cot_xml_benefi);
                            var query = from c in xml.Elements("BENEFI").Elements("Registro").Elements("fec_fnac")
                                        select c;
                            foreach (XElement fecha in query)
                            {
                                fecha.Value = (DateTime.ParseExact(fecha.Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None).AddYears(-1)).ToString("yyyyMMdd");
                            }
                            parametrosAFP.cot_xml_benefi = xml.ToString(SaveOptions.DisableFormatting);
                        }

                        saldo += rentabilidad;

                        parametrosAFP.cot_val_puam = saldo;
                        parametrosAFP.cot_val_puni = saldo;

                        if (diferencia < 0)
                            cotizacionAFP = servicioCotizador.CotizarConParametros(parametrosAFP);

                        control.GraficoAFP.Add(new GraficoLineal { Nombre = "RETIRO PROGRAMADO", Valor = cotizacionAFP.PensionCia });

                        if (diferencia < 0)
                        {
                            pensionAnhoAFP = cotizacionAFP.PensionCia * 12;
                            acumuladoAFP += pensionAnhoAFP;
                            saldo -= pensionAnhoAFP;
                            rentabilidad = saldo * (rentabilidadAFP / 100);
                        }
                        else
                        {
                            acumuladoDevengue += cotizacionAFP.PensionCia * 12;
                            acumuladoAFP += (cotizacionAFP.PensionCia * 12);
                            diferencia--;
                            if (diferencia < 0)
                            {
                                saldo -= acumuladoDevengue;
                                rentabilidad = saldo * (rentabilidadAFP / 100);
                                parametrosAFP.cot_fec_fdev = parametrosAFP.cot_fec_fcal;
                            }
                        }



                        //inicio pintar data en cuadro
                        if (ordenModalidad == Enums.OrdenModalidad.Modalidad_1.StringValue())
                        {
                            if (i == 4)
                            {
                                control.MOD105Pension = cotizacionCIA.PensionAFP;
                                control.MOD105Acumulado = acumuladoCIA;
                            }
                        }
                        else if (ordenModalidad == Enums.OrdenModalidad.Modalidad_2.StringValue())
                        {
                            if (i == 4)
                            {
                                control.MOD205Pension = cotizacionCIA.PensionAFP;
                                control.MOD205Acumulado = acumuladoCIA;
                            }
                        }
                        else if (ordenModalidad == Enums.OrdenModalidad.Modalidad_3.StringValue())
                        {
                            if (i == 4)
                            {
                                control.MOD305Pension = cotizacionCIA.PensionAFP;
                                control.MOD305Acumulado = acumuladoCIA;
                            }
                        }
                        //fin pintar data en cuadro

                    }

                }
            }
            //Fin años Diferidos


            //<INIGTI_2145>
            //Inicio años 1ER TRAMO ESCALONADO
            if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
            {
                aniosDiferido = (int)cotizacionCIA.PeriodoDiferido;

                if (aniosDiferido > 1)
                {
                    for (int i = 2; i <= aniosDiferido; i++)
                    {
                        //<INIGTI_2145>
                        //Inicio calculo CIA

                        cotizacionCIA.PensionCia = Math.Round(cotizacionCIA.PensionCia * (1 + tasaAjuste / 100), 2);

                        control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionCia });

                        pensionAnhoCIA = cotizacionCIA.PensionCia * 12;
                        acumuladoCIA += pensionAnhoCIA;

                        //Fin calculo CIA
                        //<FINGTI_2145>

                        //if (diferencia < 0)
                        //{
                        //    // Aumentar un año a los beneficiarios
                        //    XDocument xml = XDocument.Parse(parametrosAFP.cot_xml_benefi);
                        //    var query = from c in xml.Elements("BENEFI").Elements("Registro").Elements("fec_fnac")
                        //                select c;
                        //    foreach (XElement fecha in query)
                        //    {
                        //        fecha.Value = (DateTime.ParseExact(fecha.Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None).AddYears(-1)).ToString("yyyyMMdd");
                        //    }
                        //    parametrosAFP.cot_xml_benefi = xml.ToString(SaveOptions.DisableFormatting);
                        //}

                        //saldo += rentabilidad;

                        //parametrosAFP.cot_val_puam = saldo;
                        //parametrosAFP.cot_val_puni = saldo;

                        //if (diferencia < 0)
                        //    cotizacionAFP = servicioCotizador.CotizarConParametros(parametrosAFP);

                        //control.GraficoAFP.Add(new GraficoLineal { Nombre = "RETIRO PROGRAMADO", Valor = cotizacionAFP.PensionCia });

                        //if (diferencia < 0)
                        //{
                        //    pensionAnhoAFP = cotizacionAFP.PensionCia * 12;
                        //    acumuladoAFP += pensionAnhoAFP;
                        //    saldo -= pensionAnhoAFP;
                        //    rentabilidad = saldo * (rentabilidadAFP / 100);
                        //}
                        //else
                        //{
                        //    acumuladoDevengue += cotizacionAFP.PensionCia * 12;
                        //    acumuladoAFP += (cotizacionAFP.PensionCia * 12);
                        //    diferencia--;
                        //    if (diferencia < 0)
                        //    {
                        //        saldo -= acumuladoDevengue;
                        //        rentabilidad = saldo * (rentabilidadAFP / 100);
                        //        parametrosAFP.cot_fec_fdev = parametrosAFP.cot_fec_fcal;
                        //    }
                        //}



                        //inicio pintar data en cuadro
                        if (ordenModalidad == Enums.OrdenModalidad.Modalidad_1.StringValue())
                        {
                            if (i == 5)
                            {
                                control.MOD105Pension = cotizacionCIA.PensionCia;
                                control.MOD105Acumulado = acumuladoCIA;
                            }
                            if (i == 10)
                            {
                                control.MOD110Pension = cotizacionCIA.PensionCia;
                                control.MOD110Acumulado = acumuladoCIA;
                            }
                            if (i == 15)
                            {
                                control.MOD115Pension = cotizacionCIA.PensionCia;
                                control.MOD115Acumulado = acumuladoCIA;
                            }
                            if (i == 20)
                            {
                                control.MOD120Pension = cotizacionCIA.PensionCia;
                                control.MOD120Acumulado = acumuladoCIA;
                            }
                            if (i == 25)
                            {
                                control.MOD125Pension = cotizacionCIA.PensionCia;
                                control.MOD125Acumulado = acumuladoCIA;
                            }
                        }
                        else if (ordenModalidad == Enums.OrdenModalidad.Modalidad_2.StringValue())
                        {
                            if (i == 5)
                            {
                                control.MOD205Pension = cotizacionCIA.PensionCia;
                                control.MOD205Acumulado = acumuladoCIA;
                            }
                            if (i == 10)
                            {
                                control.MOD210Pension = cotizacionCIA.PensionCia;
                                control.MOD210Acumulado = acumuladoCIA;
                            }
                            if (i == 15)
                            {
                                control.MOD215Pension = cotizacionCIA.PensionCia;
                                control.MOD215Acumulado = acumuladoCIA;
                            }
                            if (i == 20)
                            {
                                control.MOD220Pension = cotizacionCIA.PensionCia;
                                control.MOD220Acumulado = acumuladoCIA;
                            }
                            if (i == 25)
                            {
                                control.MOD225Pension = cotizacionCIA.PensionCia;
                                control.MOD225Acumulado = acumuladoCIA;
                            }
                        }
                        else if (ordenModalidad == Enums.OrdenModalidad.Modalidad_3.StringValue())
                        {
                            if (i == 5)
                            {
                                control.MOD305Pension = cotizacionCIA.PensionCia;
                                control.MOD305Acumulado = acumuladoCIA;
                            }
                            if (i == 10)
                            {
                                control.MOD310Pension = cotizacionCIA.PensionCia;
                                control.MOD310Acumulado = acumuladoCIA;
                            }
                            if (i == 15)
                            {
                                control.MOD315Pension = cotizacionCIA.PensionCia;
                                control.MOD315Acumulado = acumuladoCIA;
                            }
                            if (i == 20)
                            {
                                control.MOD320Pension = cotizacionCIA.PensionCia;
                                control.MOD320Acumulado = acumuladoCIA;
                            }
                            if (i == 25)
                            {
                                control.MOD325Pension = cotizacionCIA.PensionCia;
                                control.MOD325Acumulado = acumuladoCIA;
                            }
                        }
                        //fin pintar data en cuadro

                    }

                }
                //aniosDiferido += 1;
            }
            //Fin años Diferidos
            //<FINGTI_2145>

            //Inicio Resto de años

            if (aniosDiferido == 0)
            {
                aniosDiferido = 1;
            }


            //<INIGTI_2145>
            if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
            {
                if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                    || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                {
                    cotizacionCIA.PensionCiaMO = cotizacionCIA.PrimeraPensionRVD;
                    cotizacionCIA.PensionCia = cotizacionCIA.PrimeraPensionRVD;
                }
                else
                {
                    cotizacionCIA.PensionCiaMO = cotizacionCIA.PrimeraPensionRVD * tipoCambio;
                    cotizacionCIA.PensionCia = cotizacionCIA.PrimeraPensionRVD * tipoCambio;
                }
                //Quitando el 2% aprox.
                cotizacionCIA.PensionCia = Math.Round(cotizacionCIA.PensionCia / (1 + tasaAjuste / 100), 2);
                cotizacionCIA.PensionCiaMO = Math.Round(cotizacionCIA.PensionCiaMO / (1 + tasaAjuste / 100), 2);
            }
            //<FINGTI_2145>

            for (int i = aniosDiferido; i <= 25; i++)
            {
                //INICIO CÁLCULO CIA
                if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Mixta.StringValue()
                    || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Combinada.StringValue()
                    || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue()//<INIGTI_754>
                    )
                {
                    cotizacionCIA.PensionCiaMO = Math.Round(cotizacionCIA.PensionCiaMO * (1 + tasaAjuste / 100), 2);

                    if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                    || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                    {
                        control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionCiaMO });
                    }
                    else
                    {
                        control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionCiaMO * tipoCambio });
                    }

                    pensionAnhoCIA = cotizacionCIA.PensionCiaMO * 12;
                    acumuladoCIA += pensionAnhoCIA;
                }
                else
                {
                    cotizacionCIA.PensionCia = Math.Round(cotizacionCIA.PensionCia * (1 + tasaAjuste / 100), 2);

                    control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionCia });

                    pensionAnhoCIA = cotizacionCIA.PensionCia * 12;
                    acumuladoCIA += pensionAnhoCIA;
                }
                //FIN CÁLCULO CIA


                //INICIO CÁLCULO AFP
                if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Mixta.StringValue()
                    || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Combinada.StringValue()
                    //<INIGTI_754>|| cotizacionCIA.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue()//<FINGTI_754> 
                    )
                {

                    if (diferencia < 0)
                    {
                        // Aumentar un año a los beneficiarios
                        XDocument xml = XDocument.Parse(parametrosAFP.cot_xml_benefi);
                        var query = from c in xml.Elements("BENEFI").Elements("Registro").Elements("fec_fnac")
                                    select c;
                        foreach (XElement fecha in query)
                        {
                            fecha.Value = (DateTime.ParseExact(fecha.Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None).AddYears(-1)).ToString("yyyyMMdd");
                        }
                        parametrosAFP.cot_xml_benefi = xml.ToString(SaveOptions.DisableFormatting);
                    }

                    saldo += rentabilidad;

                    parametrosAFP.cot_val_puam = saldo;
                    parametrosAFP.cot_val_puni = saldo;

                    if (diferencia < 0)
                        cotizacionAFP = servicioCotizador.CotizarConParametros(parametrosAFP);

                    control.GraficoAFP.Add(new GraficoLineal { Nombre = "RETIRO PROGRAMADO", Valor = cotizacionAFP.PensionCia });

                    if (diferencia < 0)
                    {
                        pensionAnhoAFP = cotizacionAFP.PensionCia * 12;
                        acumuladoAFP += pensionAnhoAFP;
                        saldo -= pensionAnhoAFP;
                        rentabilidad = saldo * (rentabilidadAFP / 100);
                    }
                    else
                    {
                        acumuladoDevengue += cotizacionAFP.PensionCia * 12;
                        acumuladoAFP += (cotizacionAFP.PensionCia * 12);
                        diferencia--;
                        if (diferencia < 0)
                        {
                            saldo -= acumuladoDevengue;
                            rentabilidad = saldo * (rentabilidadAFP / 100);
                            parametrosAFP.cot_fec_fdev = parametrosAFP.cot_fec_fcal;
                        }
                    }

                }
                //<INIGTI_754>
                else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Bimoneda.StringValue())
                {
                    cotizacionAFP.PensionCia = Math.Round(cotizacionAFP.PensionCia * (1 + tasaAjuste / 100), 2);

                    control.GraficoAFP.Add(new GraficoLineal { Nombre = "RETIRO PROGRAMADO", Valor = cotizacionAFP.PensionCia });
                    pensionAnhoAFP = cotizacionAFP.PensionCia * 12;
                    acumuladoAFP += pensionAnhoAFP;
                }
                //<FINGTI_754>
                //FIN CÁLCULO AFP



                //inicio pintar data en cuadro
                if (ordenModalidad == Enums.OrdenModalidad.Modalidad_1.StringValue())
                {
                    if (i == 4)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD105Pension = cotizacionCIA.PensionCia;
                            control.MOD105Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD105Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD105Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD105Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD105Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 9)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD110Pension = cotizacionCIA.PensionCia;
                            control.MOD110Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD110Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD110Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD110Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD110Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 14)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD115Pension = cotizacionCIA.PensionCia;
                            control.MOD115Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD115Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD115Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD115Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD115Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 19)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD120Pension = cotizacionCIA.PensionCia;
                            control.MOD120Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD120Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD120Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD120Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD120Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 24)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD125Pension = cotizacionCIA.PensionCia;
                            control.MOD125Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD125Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD125Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD125Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD125Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                }
                else if (ordenModalidad == Enums.OrdenModalidad.Modalidad_2.StringValue())
                {
                    if (i == 4)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD205Pension = cotizacionCIA.PensionCia;
                            control.MOD205Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD205Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD205Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD205Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD205Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 9)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD210Pension = cotizacionCIA.PensionCia;
                            control.MOD210Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD210Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD210Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD210Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD210Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 14)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD215Pension = cotizacionCIA.PensionCia;
                            control.MOD215Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD215Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD215Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD215Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD215Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 19)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD220Pension = cotizacionCIA.PensionCia;
                            control.MOD220Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD220Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD220Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD220Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD220Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 24)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD225Pension = cotizacionCIA.PensionCia;
                            control.MOD225Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD225Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD225Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD225Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD225Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                }
                else if (ordenModalidad == Enums.OrdenModalidad.Modalidad_3.StringValue())
                {
                    if (i == 4)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD305Pension = cotizacionCIA.PensionCia;
                            control.MOD305Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD305Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD305Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD305Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD305Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 9)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD310Pension = cotizacionCIA.PensionCia;
                            control.MOD310Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD310Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD310Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD310Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD310Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 14)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD315Pension = cotizacionCIA.PensionCia;
                            control.MOD315Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD315Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD315Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD315Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD315Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 19)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD320Pension = cotizacionCIA.PensionCia;
                            control.MOD320Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD320Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD320Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD320Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD320Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                    if (i == 24)
                    {
                        if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue()
                            //<INIGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue()
                            //<FINGTI_XXX>
                            || cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                        {
                            control.MOD325Pension = cotizacionCIA.PensionCia;
                            control.MOD325Acumulado = acumuladoCIA;
                        }
                        else
                        {
                            if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()
                                || cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.MOD325Pension = cotizacionCIA.PensionCiaMO + cotizacionAFP.PensionCia;
                                control.MOD325Acumulado = acumuladoCIA + acumuladoAFP;
                            }
                            else
                            {
                                control.MOD325Pension = (cotizacionCIA.PensionCiaMO * tipoCambio) + cotizacionAFP.PensionCia;
                                control.MOD325Acumulado = (acumuladoCIA * tipoCambio) + acumuladoAFP;
                            }
                        }
                    }
                }
                //fin pintar data en cuadro

            }
            //fin for
            //Fin Resto de años
        }

        [WebMethod]
        public static CorreoElectronico CrearDatosCorreo(string tokenUsuario, string nroSolicitud, string nroCorrelativoSolicitud,
                                                         string cuspp, string rutaImagenSimulada)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                CorreoElectronico correo;
                Afiliado afiliado = null;
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudEnviarCorreo))
                        {
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {

                                SimuladorQueMeConviene CntSimulador = HttpContext.Current.Session["CntSimulador"] as SimuladorQueMeConviene;

                                string Mod1PensionMes5 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD105Pension).ToString();
                                string Mod1PensionAcum5 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD105Acumulado).ToString();
                                string Mod1PensionMes10 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD110Pension).ToString();
                                string Mod1PensionAcum10 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD110Acumulado).ToString();
                                string Mod1PensionMes15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD115Pension).ToString();
                                string Mod1PensionAcum15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD115Acumulado).ToString();
                                string Mod1PensionMes20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD120Pension).ToString();
                                string Mod1PensionAcum20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD120Acumulado).ToString();
                                string Mod1PensionMes25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD125Pension).ToString();
                                string Mod1PensionAcum25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD125Acumulado).ToString();

                                string Mod2PensionMes5 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD205Pension).ToString();
                                string Mod2PensionAcum5 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD205Acumulado).ToString();
                                string Mod2PensionMes10 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD210Pension).ToString();
                                string Mod2PensionAcum10 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD210Acumulado).ToString();
                                string Mod2PensionMes15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD215Pension).ToString();
                                string Mod2PensionAcum15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD215Acumulado).ToString();
                                string Mod2PensionMes20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD220Pension).ToString();
                                string Mod2PensionAcum20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD220Acumulado).ToString();
                                string Mod2PensionMes25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD225Pension).ToString();
                                string Mod2PensionAcum25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD225Acumulado).ToString();

                                string Mod3PensionMes5 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD305Pension).ToString();
                                string Mod3PensionAcum5 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD305Acumulado).ToString();
                                string Mod3PensionMes10 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD310Pension).ToString();
                                string Mod3PensionAcum10 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD310Acumulado).ToString();
                                string Mod3PensionMes15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD315Pension).ToString();
                                string Mod3PensionAcum15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD315Acumulado).ToString();
                                string Mod3PensionMes20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD320Pension).ToString();
                                string Mod3PensionAcum20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD320Acumulado).ToString();
                                string Mod3PensionMes25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD325Pension).ToString();
                                string Mod3PensionAcum25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.MOD325Acumulado).ToString();
                                string rutaImagen = rutaImagenSimulada.Substring(22, rutaImagenSimulada.Length - 22);

                                //string rutaImagen1 = "";
                                //string rutaImagen2 = "";

                                //if (rutaImagen.Length > 32737){
                                //    rutaImagen1 = rutaImagen.Substring(0, 32737);
                                //    rutaImagen2 = rutaImagen.Substring(32737, rutaImagen.Length - 32737);
                                //}
                                //else
                                //{
                                //    rutaImagen1 = rutaImagen;
                                //    rutaImagen2 = "";
                                //}


                                ReportViewer visorReporte = new ReportViewer();
                                visorReporte.ProcessingMode = ProcessingMode.Remote;
                                visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteSimulacionQueMeConviene"];

                                ReportParameter p1 = new ReportParameter("wl_mod1_pension_mes_5", Mod1PensionMes5);
                                ReportParameter p2 = new ReportParameter("wl_mod1_pension_acum_5", Mod1PensionAcum5);
                                ReportParameter p3 = new ReportParameter("wl_mod1_pension_mes_10", Mod1PensionMes10);
                                ReportParameter p4 = new ReportParameter("wl_mod1_pension_acum_10", Mod1PensionAcum10);
                                ReportParameter p5 = new ReportParameter("wl_mod1_pension_mes_15", Mod1PensionMes15);
                                ReportParameter p6 = new ReportParameter("wl_mod1_pension_acum_15", Mod1PensionAcum15);
                                ReportParameter p7 = new ReportParameter("wl_mod1_pension_mes_20", Mod1PensionMes20);
                                ReportParameter p8 = new ReportParameter("wl_mod1_pension_acum_20", Mod1PensionAcum20);
                                ReportParameter p9 = new ReportParameter("wl_mod1_pension_mes_25", Mod1PensionMes25);
                                ReportParameter p10 = new ReportParameter("wl_mod1_pension_acum_25", Mod1PensionAcum25);

                                ReportParameter p11 = new ReportParameter("wl_mod2_pension_mes_5", Mod2PensionMes5);
                                ReportParameter p12 = new ReportParameter("wl_mod2_pension_acum_5", Mod2PensionAcum5);
                                ReportParameter p13 = new ReportParameter("wl_mod2_pension_mes_10", Mod2PensionMes10);
                                ReportParameter p14 = new ReportParameter("wl_mod2_pension_acum_10", Mod2PensionAcum10);
                                ReportParameter p15 = new ReportParameter("wl_mod2_pension_mes_15", Mod2PensionMes15);
                                ReportParameter p16 = new ReportParameter("wl_mod2_pension_acum_15", Mod2PensionAcum15);
                                ReportParameter p17 = new ReportParameter("wl_mod2_pension_mes_20", Mod2PensionMes20);
                                ReportParameter p18 = new ReportParameter("wl_mod2_pension_acum_20", Mod2PensionAcum20);
                                ReportParameter p19 = new ReportParameter("wl_mod2_pension_mes_25", Mod2PensionMes25);
                                ReportParameter p20 = new ReportParameter("wl_mod2_pension_acum_25", Mod2PensionAcum25);

                                ReportParameter p21 = new ReportParameter("wl_mod3_pension_mes_5", Mod3PensionMes5);
                                ReportParameter p22 = new ReportParameter("wl_mod3_pension_acum_5", Mod3PensionAcum5);
                                ReportParameter p23 = new ReportParameter("wl_mod3_pension_mes_10", Mod3PensionMes10);
                                ReportParameter p24 = new ReportParameter("wl_mod3_pension_acum_10", Mod3PensionAcum10);
                                ReportParameter p25 = new ReportParameter("wl_mod3_pension_mes_15", Mod3PensionMes15);
                                ReportParameter p26 = new ReportParameter("wl_mod3_pension_acum_15", Mod3PensionAcum15);
                                ReportParameter p27 = new ReportParameter("wl_mod3_pension_mes_20", Mod3PensionMes20);
                                ReportParameter p28 = new ReportParameter("wl_mod3_pension_acum_20", Mod3PensionAcum20);
                                ReportParameter p29 = new ReportParameter("wl_mod3_pension_mes_25", Mod3PensionMes25);
                                ReportParameter p30 = new ReportParameter("wl_mod3_pension_acum_25", Mod3PensionAcum25);

                                ReportParameter p31 = new ReportParameter("wl_ruta_imagen", rutaImagen);

                                //ReportParameter p31 = new ReportParameter("wl_ruta_imagen_1", rutaImagen1);
                                //ReportParameter p32 = new ReportParameter("wl_ruta_imagen_2", rutaImagen2);

                                log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteSimulacionQueMeConviene"]));
                                log.Debug(String.Format("Parámetros del reporte: wl_mod1_pension_mes_5[{0}] wl_mod1_pension_acum_5[{1}] wl_mod1_pension_mes_10[{2}] wl_mod1_pension_acum_10[{3}] wl_mod1_pension_mes_15[{4}] wl_mod1_pension_acum_15[{5}] wl_mod1_pension_mes_20[{6}] wl_mod1_pension_acum_20[{7}] wl_mod1_pension_mes_25[{8}] wl_mod1_pension_acum_25[{9}] wl_mod2_pension_mes_5[{10}] wl_mod2_pension_acum_5[{11}] wl_mod2_pension_mes_10[{12}] wl_mod2_pension_acum_10[{13}] wl_mod2_pension_mes_15[{14}] wl_mod2_pension_acum_15[{15}] wl_mod2_pension_mes_20[{16}] wl_mod2_pension_acum_20[{17}] wl_mod2_pension_mes_25[{18}] wl_mod2_pension_acum_25[{19}] wl_mod3_pension_mes_5[{20}] wl_mod3_pension_acum_5[{21}] wl_mod3_pension_mes_10[{22}] wl_mod3_pension_acum_10[{23}] wl_mod3_pension_mes_15[{24}] wl_mod3_pension_acum_15[{25}] wl_mod3_pension_mes_20[{26}] wl_mod3_pension_acum_20[{27}] wl_mod3_pension_mes_25[{28}] wl_mod3_pension_acum_25[{29}] wl_ruta_imagen[{30}].",
                                    Mod1PensionMes5, Mod1PensionAcum5, Mod1PensionMes10, Mod1PensionAcum10, Mod1PensionMes15, Mod1PensionAcum15, Mod1PensionMes20, Mod1PensionAcum20, Mod1PensionMes25, Mod1PensionAcum25, Mod2PensionMes5, Mod2PensionAcum5, Mod2PensionMes10, Mod2PensionAcum10, Mod2PensionMes15, Mod2PensionAcum15, Mod2PensionMes20, Mod2PensionAcum20, Mod2PensionMes25, Mod2PensionAcum25, Mod3PensionMes5, Mod3PensionAcum5, Mod3PensionMes10, Mod3PensionAcum10, Mod3PensionMes15, Mod3PensionAcum15, Mod3PensionMes20, Mod3PensionAcum20, Mod3PensionMes25, Mod3PensionAcum25, rutaImagen));
                                visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17, p18, p19, p20, p21, p22, p23, p24, p25, p26, p27, p28, p29, p30, p31 });
                                log.Debug(String.Format("Reporte para solicitud [{0}] procesado.", nroSolicitud));

                                string format = "PDF", mimeType, encoding, extension;
                                string[] streamids;
                                Warning[] warnings;

                                log.Debug(String.Format("Se va a exportar a formato PDF el reporte para solicitud [{0}].", nroSolicitud));
                                byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                                HttpContext.Current.Session["ArchivoPDF"] = bytes;
                                log.Debug(String.Format("Reporte para solicitud [{0}] exportado y almacenado en sesión de usuario.", nroSolicitud));

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                afiliado = servicioCotizador.ObtenerDatosAfiliado(nroSolicitud, cuspp, "", "", "");

                                SeccionCorreo config = (SeccionCorreo)ConfigurationManager.GetSection("correo");

                                correo = new CorreoElectronico
                                {
                                    De = (string)HttpContext.Current.Session["CorreoElectronico"],
                                    DeNombre = (string)HttpContext.Current.Session["NombreCompleto"],
                                    Para = String.Format("{0}", afiliado.CorreoElectronico),
                                    ParaNombre = String.Format("{0} {1}, {2}", afiliado.ApellidoPaterno, afiliado.ApellidoMaterno, afiliado.Nombre),
                                    Asunto = config.Asunto.Texto
                                                    .Replace("{NombreAfiliado}", afiliado.Nombre)
                                                    .Replace("{ApellidoPaternoAfiliado}", afiliado.ApellidoPaterno)
                                                    .Replace("{ApellidoMaternoAfiliado}", afiliado.ApellidoMaterno),
                                    Adjunto = "Cotización.pdf (" + Utilitarios.FormatearBytes(bytes.Length, false) + ")",
                                    Mensaje = config.MensajeSimulacion.Texto
                                                    //.Replace("{TratamientoAfiliado}", (String.Equals(afiliado.Sexo, "M")) ? ("Sr.") : ("Sra."))
                                                    .Replace("{TratamientoAfiliado}", (Convert.ToString(afiliado.Sexo) == "M") ? ("Sr.") : ("Sra."))
                                                    .Replace("{ApellidoPaternoAfiliado}", afiliado.ApellidoPaterno)
                                                    .Replace("{ApellidoMaternoAfiliado}", afiliado.ApellidoMaterno)
                                                    .Replace("{NombreAfiliado}", afiliado.Nombre)
                                                    .Replace("{NombreAgente}", (string)HttpContext.Current.Session["NombreCompleto"])
                                };
                                correo.Respuesta = new Respuesta();
                                correo.Respuesta.Estado = Constante.COD_OK;
                            }
                            else
                            {
                                correo = new CorreoElectronico();
                                correo.Respuesta = new Respuesta();
                                correo.Respuesta.Estado = Constante.COD_ERROR;
                                correo.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                correo.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                correo.Respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DireccionActualizar.StringValue()));
                            correo = new CorreoElectronico();
                            correo.Respuesta = new Respuesta();
                            correo.Respuesta.Estado = Constante.COD_ERROR;
                            correo.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            correo.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            correo.Respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        correo = new CorreoElectronico();
                        correo.Respuesta = new Respuesta();
                        correo.Respuesta.Estado = Constante.COD_TOKEN;
                    }
                    return correo;
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
                    correo = new CorreoElectronico();
                    correo.Respuesta = new Respuesta();
                    correo.Respuesta.Estado = Constante.COD_ERROR;
                    correo.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    correo.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    correo.Respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    return correo;
                }
            }
        }

        [WebMethod]
        public static Respuesta EnviarCorreoElectronicoSimulador(string tokenUsuario, CorreoElectronico correo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudEnviarCorreo))
                        {
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                correo.Adjunto = "Cotizacion.pdf";
                                correo.BinarioAdjunto = (byte[])HttpContext.Current.Session["ArchivoPDF"];
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.EnviarCorreoElectronico(correo);
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DireccionActualizar.StringValue()));
                            correo = new CorreoElectronico();
                            correo.Respuesta = new Respuesta();
                            correo.Respuesta.Estado = Constante.COD_ERROR;
                            correo.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            correo.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            correo.Respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        correo = new CorreoElectronico();
                        correo.Respuesta = new Respuesta();
                        correo.Respuesta.Estado = Constante.COD_TOKEN;
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
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }
                return respuesta;
            }
        }

    }
}