using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using System.Net;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using System.Net.Mail;
using Microsoft.Reporting.WebForms;

namespace Interseguro.CWRV.Presentacion.ASPNET.Simuladores
{
    public partial class InmediataDiferida : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JubilarseHoyFuturo));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorInmediataDiferida))
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
                            Enums.OpcionesSistema.SimuladorInmediataDiferida.StringValue()));
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

            Inflacion.Text = parametros[2].Valor.ToString();
            TasaAjuste.Text = parametros[3].Valor.ToString();

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

            Inflacion.Text = String.Empty;
            TasaAjuste.Text = String.Empty;

            Inflacion.CssClass = Inflacion.CssClass.Replace(" formTextboxError", String.Empty);
            TasaAjuste.CssClass = TasaAjuste.CssClass.Replace(" formTextboxError", String.Empty);
        }

        private void CargarInformacionInicialPantalla()
        {
            IdSimulador.Value = ((int)Enums.OpcionesSistema.SimuladorInmediataDiferida).ToString();

            if (Session["Consentimiento"] != null)
                FormularioBusqueda.Visible = (bool)Session["Consentimiento"];

            // Cargar información de los Combobox
            Moneda.Items.Add(new ListItem("S/.", Enums.Moneda.Soles.StringValue()));
            Moneda.Items.Add(new ListItem("S/. Aj.", Enums.Moneda.SolesAjustados.StringValue()));
            //<SRIINI20322>
            Moneda.Items.Add(new ListItem("$ Aj.", Enums.Moneda.DolaresAjustados.StringValue()));
            //<SRIINI20322>

            PeriodoGarantizado.Items.Add("0");
            PeriodoGarantizado.Items.Add("10");
            PeriodoGarantizado.Items.Add("15");

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

        //[WebMethod]
        //public static Respuesta SimularInmediataDiferida(string tokenUsuario, List<Cotizacion> cotizaciones, Int64? correlativo1, Int64? correlativo2, Int64? correlativo3, double tasa)
        [WebMethod]
        public static Respuesta SimularInmediataDiferida(string tokenUsuario, List<Cotizacion> cotizaciones,
                                                         Int64? correlativo1, Int64? correlativo2, Int64? correlativo3,
                                                         Int64? correlativo4, Int64? correlativo5, Int64? correlativo6, double tasa)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        //<SRI.INI-20322>
                        //var control = (SimuladorInmediataDiferida)pagina.LoadControl("~/Controles/SimuladorInmediataDiferida.ascx");
                        SimuladorInmediataDiferida control = (SimuladorInmediataDiferida)pagina.LoadControl("~/Controles/SimuladorInmediataDiferida.ascx");
                        //<SRI.FIN-20322>

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorInmediataDiferida))
                        {
                            control.PermisoEjecutar = true;
                            if (correlativo1 != null)
                            {
                                respuesta.Grafico1 = new List<GraficoLineal>();
                                double pension1 = cotizaciones.Find(c => c.Correlativo == correlativo1).PensionCia;
                                double pensionAnho1 = pension1 * 12;
                                double acumulado1 = pensionAnho1;
                                respuesta.Grafico1.Add(new GraficoLineal { Nombre = "RENTA VITALICIA INMEDIATA", Valor = acumulado1 });
                                for (int i = 1; i < 25; i++)
                                {
                                    pension1 *= (1 + (tasa / 100));
                                    pensionAnho1 = pension1 * 12;
                                    acumulado1 += pensionAnho1;
                                    respuesta.Grafico1.Add(new GraficoLineal { Nombre = "RENTA VITALICIA INMEDIATA", Valor = acumulado1 });

                                    if (i == 14) control.RVI15Acumulado = acumulado1;
                                    if (i == 19) control.RVI20Acumulado = acumulado1;
                                    if (i == 24) control.RVI25Acumulado = acumulado1;
                                }
                            }

                            if (correlativo2 != null)
                            {
                                respuesta.Grafico2 = new List<GraficoLineal>();
                                double pension2 = cotizaciones.Find(c => c.Correlativo == correlativo2).PensionAFP;
                                double pension2Cia = cotizaciones.Find(c => c.Correlativo == correlativo2).PensionCia;
                                double pensionAnho2 = pension2 * 12;
                                double acumulado2 = pensionAnho2;
                                respuesta.Grafico2.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 1 AÑO", Valor = acumulado2 });
                                for (int i = 1; i < 25; i++)
                                {
                                    if (i == 1)
                                    {
                                        pension2 = pension2Cia * (1 + (tasa / 100));
                                    }
                                    else
                                    {
                                        pension2 *= (1 + (tasa / 100));
                                    }
                                    pensionAnho2 = pension2 * 12;
                                    acumulado2 += pensionAnho2;
                                    respuesta.Grafico2.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 1 AÑO", Valor = acumulado2 });

                                    if (i == 14) control.RD115Acumulado = acumulado2;
                                    if (i == 19) control.RD120Acumulado = acumulado2;
                                    if (i == 24) control.RD125Acumulado = acumulado2;
                                }
                            }


                            if (correlativo3 != null)
                            {
                                respuesta.Grafico3 = new List<GraficoLineal>();
                                double pension3 = cotizaciones.Find(c => c.Correlativo == correlativo3).PensionAFP;
                                double pension3Cia = cotizaciones.Find(c => c.Correlativo == correlativo3).PensionCia;
                                double pensionAnho3 = pension3 * 12;
                                double acumulado3 = pensionAnho3;
                                respuesta.Grafico3.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 2 AÑOS", Valor = acumulado3 });
                                for (int i = 1; i < 25; i++)
                                {
                                    if (i == 1)
                                    {
                                        pension3Cia *= (1 + (tasa / 100));
                                    }
                                    if (i == 2)
                                    {
                                        pension3 = pension3Cia * (1 + (tasa / 100));
                                    }
                                    else
                                    {
                                        pension3 *= (1 + (tasa / 100));
                                    }
                                    pensionAnho3 = pension3 * 12;
                                    acumulado3 += pensionAnho3;
                                    respuesta.Grafico3.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 2 AÑOS", Valor = acumulado3 });

                                    if (i == 14) control.RD215Acumulado = acumulado3;
                                    if (i == 19) control.RD220Acumulado = acumulado3;
                                    if (i == 24) control.RD225Acumulado = acumulado3;
                                }
                            }

                            //<SRI.INI-20322>
                            if (correlativo4 != null)
                            {
                                respuesta.Grafico4 = new List<GraficoLineal>();
                                double pension4 = cotizaciones.Find(c => c.Correlativo == correlativo4).PensionAFP;
                                double pension4Cia = cotizaciones.Find(c => c.Correlativo == correlativo4).PensionCia;
                                double pensionAnho4 = pension4 * 12;
                                double acumulado4 = pensionAnho4;
                                respuesta.Grafico4.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 3 AÑOS", Valor = acumulado4 });
                                for (int i = 1; i < 25; i++)
                                {
                                    if (i >= 1 && i <= 2)
                                    {
                                        pension4Cia *= (1 + (tasa / 100));
                                    }
                                    if (i == 3)
                                    {
                                        pension4 = pension4Cia * (1 + (tasa / 100));
                                    }
                                    else
                                    {
                                        pension4 *= (1 + (tasa / 100));
                                    }
                                    pensionAnho4 = pension4 * 12;
                                    acumulado4 += pensionAnho4;
                                    respuesta.Grafico4.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 3 AÑOS", Valor = acumulado4 });

                                    if (i == 14) control.RD315Acumulado = acumulado4;
                                    if (i == 19) control.RD320Acumulado = acumulado4;
                                    if (i == 24) control.RD325Acumulado = acumulado4;
                                }
                            }
                            else

                            if (correlativo5 != null)
                            {
                                respuesta.Grafico5 = new List<GraficoLineal>();
                                double pension5 = cotizaciones.Find(c => c.Correlativo == correlativo5).PensionAFP;
                                double pension5Cia = cotizaciones.Find(c => c.Correlativo == correlativo5).PensionCia;
                                double pensionAnho5 = pension5 * 12;
                                double acumulado5 = pensionAnho5;
                                respuesta.Grafico5.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 4 AÑOS", Valor = acumulado5 });
                                for (int i = 1; i < 25; i++)
                                {
                                    if (i >= 1 && i <= 3)
                                    {
                                        pension5Cia *= (1 + (tasa / 100));
                                    }
                                    if (i == 4)
                                    {
                                        pension5 = pension5Cia * (1 + (tasa / 100));
                                    }
                                    else
                                    {
                                        pension5 *= (1 + (tasa / 100));
                                    }
                                    pensionAnho5 = pension5 * 12;
                                    acumulado5 += pensionAnho5;
                                    respuesta.Grafico5.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 4 AÑOS", Valor = acumulado5 });

                                    if (i == 14) control.RD415Acumulado = acumulado5;
                                    if (i == 19) control.RD420Acumulado = acumulado5;
                                    if (i == 24) control.RD425Acumulado = acumulado5;
                                }
                            }

                            if (correlativo6 != null)
                            {
                                respuesta.Grafico6 = new List<GraficoLineal>();
                                double pension6 = cotizaciones.Find(c => c.Correlativo == correlativo6).PensionAFP;
                                double pension6Cia = cotizaciones.Find(c => c.Correlativo == correlativo6).PensionCia;
                                double pensionAnho6 = pension6 * 12;
                                double acumulado6 = pensionAnho6;
                                respuesta.Grafico6.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 5 AÑOS", Valor = acumulado6 });
                                for (int i = 1; i < 25; i++)
                                {
                                    if (i >= 1 && i <= 4)
                                    {
                                        pension6Cia *= (1 + (tasa / 100));
                                    }
                                    if (i == 5)
                                    {
                                        pension6 = pension6Cia * (1 + (tasa / 100));
                                    }
                                    else
                                    {
                                        pension6 *= (1 + (tasa / 100));
                                    }
                                    pensionAnho6 = pension6 * 12;
                                    acumulado6 += pensionAnho6;
                                    respuesta.Grafico6.Add(new GraficoLineal { Nombre = "RENTA TEMPORAL A 5 AÑOS", Valor = acumulado6 });

                                    if (i == 14) control.RD515Acumulado = acumulado6;
                                    if (i == 19) control.RD520Acumulado = acumulado6;
                                    if (i == 24) control.RD525Acumulado = acumulado6;
                                }
                            }
                            //<SRI.FIN-20322>
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
                            IdTipoEvento = Enums.EventoLog.Reporte3.StringValue(),
                            Detalle = String.Format("Cotizaciones Inmediata[{0}] Diferida 1 año[{1}] Diferida 2 años[{2}] Diferida 3 años[{3}] Diferida 4 años[{4}] Diferida 5 años[{5}] simuladas", correlativo1, correlativo2, correlativo3, correlativo4, correlativo5, correlativo6)
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

                        //<SRI.INI-20322>
                        HttpContext.Current.Session["CntSimulador"] = control;
                        //<SRI.FIN-20322>

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
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

        //<SRI.INI-20322>
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

                                SimuladorInmediataDiferida CntSimulador = HttpContext.Current.Session["CntSimulador"] as SimuladorInmediataDiferida;

                                string rviAcumulado15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.RVI15Acumulado).ToString();
                                string rviAcumulado20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.RVI20Acumulado).ToString();
                                string rviAcumulado25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.RVI25Acumulado).ToString();
                                string renDiferidaUno15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.RD115Acumulado).ToString();
                                string renDiferidaUno20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.RD120Acumulado).ToString();
                                string renDiferidaUno25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.RD125Acumulado).ToString();
                                string renDiferidaDos15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.RD215Acumulado).ToString();
                                string renDiferidaDos20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.RD220Acumulado).ToString();
                                string renDiferidaDos25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.RD225Acumulado).ToString();
                                string rutaImagen = rutaImagenSimulada.Substring(22, rutaImagenSimulada.Length - 22);

                                ReportViewer visorReporte = new ReportViewer();
                                visorReporte.ProcessingMode = ProcessingMode.Remote;
                                visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteSimulacionInmediataDiferida"];

                                ReportParameter p1 = new ReportParameter("wl_rvi_acum_quince", rviAcumulado15);
                                ReportParameter p2 = new ReportParameter("wl_rvi_acum_veinte", rviAcumulado20);
                                ReportParameter p3 = new ReportParameter("wl_rvi_acum_veinticinco", rviAcumulado25);
                                ReportParameter p4 = new ReportParameter("wl_ren_dif_uno_quince", renDiferidaUno15);
                                ReportParameter p5 = new ReportParameter("wl_ren_dif_uno_veinte", renDiferidaUno20);
                                ReportParameter p6 = new ReportParameter("wl_ren_dif_uno_veinticinco", renDiferidaUno25);
                                ReportParameter p7 = new ReportParameter("wl_ren_dif_dos_quince", renDiferidaDos15);
                                ReportParameter p8 = new ReportParameter("wl_ren_dif_dos_veinte", renDiferidaDos20);
                                ReportParameter p9 = new ReportParameter("wl_ren_dif_dos_veinticinco", renDiferidaDos25);
                                ReportParameter p10 = new ReportParameter("wl_ruta_imagen", rutaImagen);

                                log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteSimulacionInmediataDiferida"]));
                                log.Debug(String.Format("Parámetros del reporte: wl_rvi_acum_quince[{0}] wl_rvi_acum_veinte[{1}] wl_rvi_acum_veinticinco[{2}] wl_ren_dif_uno_quince[{3}] wl_ren_dif_uno_veinte[{4}] wl_ren_dif_uno_veinticinco[{5}] wl_ren_dif_dos_quince[{6}] wl_ren_dif_dos_veinte[{7}] wl_ren_dif_dos_veinticinco[{8}] wl_ruta_imagen[{9}].",
                                    rviAcumulado15, rviAcumulado20, rviAcumulado25, renDiferidaUno15, renDiferidaUno20, renDiferidaUno25, renDiferidaDos15, renDiferidaDos20, renDiferidaDos25, rutaImagen));
                                visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10 });
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
                                //<SRIINI20322>
                                //MailMessage mensaje = new MailMessage();

                                //mensaje.To.Add(new MailAddress(correo.Para, correo.ParaNombre, System.Text.Encoding.UTF8));
                                //mensaje.Bcc.Add(new MailAddress(correo.De, correo.DeNombre, System.Text.Encoding.UTF8));
                                //mensaje.From = new MailAddress(correo.De, correo.DeNombre, System.Text.Encoding.UTF8);
                                //mensaje.Subject = correo.Asunto;
                                //mensaje.SubjectEncoding = System.Text.Encoding.UTF8;
                                //mensaje.Body = correo.Mensaje;
                                //mensaje.BodyEncoding = System.Text.Encoding.UTF8;
                                //mensaje.Attachments.Add(new Attachment(new MemoryStream((byte[])HttpContext.Current.Session["ArchivoPDF"]), "Cotizacion.pdf"));

                                //log.Info(String.Format("Se va a establecer conexión con el Servidor SMTP[{0}] Puerto[{1}].",
                                //    ConfigurationManager.AppSettings["DominioSMTP"],
                                //    ConfigurationManager.AppSettings["PuertoSMTP"]));
                                //SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["DominioSMTP"], Convert.ToInt32(ConfigurationManager.AppSettings["PuertoSMTP"]));

                                ////client.EnableSsl = true;
                                //client.UseDefaultCredentials = true;
                                ////client.Credentials = credenciales;
                                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                //log.Debug(String.Format("Usuario va a enviar correo electrónico con cotización adjunta a la dirección[{0} <{1}>].",
                                //    correo.ParaNombre, correo.Para));
                                //client.Send(mensaje);
                                //log.Info(String.Format("Correo electrónico enviado correctamente a la dirección[{0} <{1}>].",
                                //    correo.ParaNombre, correo.Para));

                                //string nombreTerminal = String.Empty;
                                //try
                                //{
                                //    nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                //}
                                //catch (Exception)
                                //{
                                //    log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                //        HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                //}

                                //nombreTerminal += HttpContext.Current.Request.UserAgent;

                                //servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                //servicioCotizador.RegistrarLog(new LogBD
                                //{
                                //    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                //    NombreTerminal = nombreTerminal,
                                //    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                //    NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                //    IdTipoEvento = Enums.EventoLog.EnviarCorreoElectronico.StringValue(),
                                //    Detalle = String.Format("Solicitud {0} enviada a {1} ({2})", HttpContext.Current.Session["idSolicitud"], correo.ParaNombre, correo.Para)
                                //});

                                //respuesta.Estado = Constante.COD_OK;
                                //respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                //respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                //respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Correo electrónico enviado correctamente." });

                                correo.Adjunto = "Cotizacion.pdf";
                                correo.BinarioAdjunto = (byte[])HttpContext.Current.Session["ArchivoPDF"];
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.EnviarCorreoElectronico(correo);
                                //<SRIFIN20322>
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
        //<SRI.FIN-20322>
    }
}