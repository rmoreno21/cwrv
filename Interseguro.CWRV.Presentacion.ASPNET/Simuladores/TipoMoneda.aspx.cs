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
    public partial class TipoMoneda : System.Web.UI.Page
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
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorTipoMoneda))
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
                            Enums.OpcionesSistema.SimuladorTipoMoneda.StringValue()));
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
            TipoCambio.Text = parametros[0].Valor.ToString();
            TasaAjuste.Text = parametros[3].Valor.ToString();
            AjusteTipoCambio.Text = "0.00";

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
            TipoCambio.Text = String.Empty;
            AjusteTipoCambio.Text = String.Empty;

            Inflacion.CssClass = Inflacion.CssClass.Replace(" formTextboxError", String.Empty);
            TasaAjuste.CssClass = TasaAjuste.CssClass.Replace(" formTextboxError", String.Empty);
        }

        private void CargarInformacionInicialPantalla()
        {
            IdSimulador.Value = ((int)Enums.OpcionesSistema.SimuladorTipoMoneda).ToString();

            if (Session["Consentimiento"] != null)
                FormularioBusqueda.Visible = (bool)Session["Consentimiento"];

            // Cargar información de los Combobox
            Modalidad.Items.Add(new ListItem("Inmediata", Enums.Modalidad.Inmediata.StringValue()));
            Modalidad.Items.Add(new ListItem("Diferida a 1 año", Enums.Modalidad.Diferida.StringValue() + "1"));
            Modalidad.Items.Add(new ListItem("Diferida a 2 años", Enums.Modalidad.Diferida.StringValue() + "2"));
            //<SRI.INI-20322>
            Modalidad.Items.Add(new ListItem("Diferida a 3 años", Enums.Modalidad.Diferida.StringValue() + "3"));
            Modalidad.Items.Add(new ListItem("Diferida a 4 años", Enums.Modalidad.Diferida.StringValue() + "4"));
            Modalidad.Items.Add(new ListItem("Diferida a 5 años", Enums.Modalidad.Diferida.StringValue() + "5"));
            //<SRI.FIN-20322>

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

        [WebMethod]
        public static Respuesta SimularTipoMoneda(string tokenUsuario, List<Cotizacion> cotizaciones, Int64? correlativo1, Int64? correlativo2, Int64? correlativo3, double ipc, double tasaAjuste, double tipoCambio, double tasaTipoCambio)
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
                        //var control = (SimuladorTipoMoneda)pagina.LoadControl("~/Controles/SimuladorTipoMoneda.ascx");
                        SimuladorTipoMoneda control = (SimuladorTipoMoneda)pagina.LoadControl("~/Controles/SimuladorTipoMoneda.ascx");
                        //<SRI.FIN-20322>

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorInmediataDiferida))
                        {
                            control.PermisoEjecutar = true;
                            
                            if (correlativo1 != null)
                            {
                                respuesta.Grafico1 = new List<GraficoLineal>();
                                double pensionCia1 = cotizaciones.Find(c => c.Correlativo == correlativo1).PensionCia;
                                double pension1 = pensionCia1;
                                respuesta.Grafico1.Add(new GraficoLineal { Nombre = "S/. Indexados", Valor = pension1 });
                                for (int i = 1; i < 25; i++)
                                {
                                    pension1 = pensionCia1 * Math.Pow((1 + (ipc / 100)), i);
                                    respuesta.Grafico1.Add(new GraficoLineal { Nombre = "S/. Indexados", Valor = pension1 });

                                    if (i == 14) control.VAC15Acumulado = pension1;
                                    if (i == 19) control.VAC20Acumulado = pension1;
                                    if (i == 24) control.VAC25Acumulado = pension1;
                                }
                            }

                            
                            if (correlativo2 != null)
                            {
                                respuesta.Grafico2 = new List<GraficoLineal>();
                                double pensionCia2 = cotizaciones.Find(c => c.Correlativo == correlativo2).PensionCia;
                                double pension2 = pensionCia2;
                                respuesta.Grafico2.Add(new GraficoLineal { Nombre = "S/. Ajustados", Valor = pension2 });
                                for (int i = 1; i < 25; i++)
                                {
                                    pension2 = pensionCia2 * Math.Pow((1 + (tasaAjuste / 100)), i);
                                    respuesta.Grafico2.Add(new GraficoLineal { Nombre = "S/. Ajustados", Valor = pension2 });

                                    if (i == 14) control.SAJ15Acumulado = pension2;
                                    if (i == 19) control.SAJ20Acumulado = pension2;
                                    if (i == 24) control.SAJ25Acumulado = pension2;
                                }
                            }

                            
                            if (correlativo3 != null)
                            {
                                respuesta.Grafico3 = new List<GraficoLineal>();
                                double pensionCia3 = cotizaciones.Find(c => c.Correlativo == correlativo3).PensionCiaMO * tipoCambio;
                                double pension3 = pensionCia3;
                                respuesta.Grafico3.Add(new GraficoLineal { Nombre = "$ Ajustados", Valor = pension3 });
                                for (int i = 1; i < 25; i++)
                                {
                                    pension3 = pensionCia3 * Math.Pow((1 + (tasaAjuste / 100)), i) * Math.Pow((1 + (tasaTipoCambio / 100)), i);
                                    respuesta.Grafico3.Add(new GraficoLineal { Nombre = "$ Ajustados", Valor = pension3 });

                                    if (i == 14) control.DAJ15Acumulado = pension3;
                                    if (i == 19) control.DAJ20Acumulado = pension3;
                                    if (i == 24) control.DAJ25Acumulado = pension3;
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
                            IdTipoEvento = Enums.EventoLog.Reporte4.StringValue(),
                            Detalle = String.Format("Cotizaciones S/. Indexados[{0}] S/. Ajustados[{1}] $ Ajustados[{2}] simuladas", correlativo1, correlativo2, correlativo3)
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

                                SimuladorTipoMoneda CntSimulador = HttpContext.Current.Session["CntSimulador"] as SimuladorTipoMoneda;

                                string solesIndexados15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.VAC15Acumulado).ToString();
                                string solesIndexados20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.VAC20Acumulado).ToString();
                                string solesIndexados25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.VAC25Acumulado).ToString();
                                string solesAjustados15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.SAJ15Acumulado).ToString();
                                string solesAjustados20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.SAJ20Acumulado).ToString();
                                string solesAjustados25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.SAJ25Acumulado).ToString();
                                string dolaresAjustados15 = String.Format("{0:###,###,###,##0.00}", CntSimulador.DAJ15Acumulado).ToString();
                                string dolaresAjustados20 = String.Format("{0:###,###,###,##0.00}", CntSimulador.DAJ20Acumulado).ToString();
                                string dolaresAjustados25 = String.Format("{0:###,###,###,##0.00}", CntSimulador.DAJ25Acumulado).ToString();
                                string rutaImagen = rutaImagenSimulada.Substring(22, rutaImagenSimulada.Length - 22);

                                ReportViewer visorReporte = new ReportViewer();
                                visorReporte.ProcessingMode = ProcessingMode.Remote;
                                visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteSimulacionTipoMoneda"];

                                ReportParameter p1 = new ReportParameter("wl_soles_indexados_quince", solesIndexados15);
                                ReportParameter p2 = new ReportParameter("wl_soles_indexados_veinte", solesIndexados20);
                                ReportParameter p3 = new ReportParameter("wl_soles_indexados_veinticinco", solesIndexados25);
                                ReportParameter p4 = new ReportParameter("wl_soles_ajustados_quince", solesAjustados15);
                                ReportParameter p5 = new ReportParameter("wl_soles_ajustados_veinte", solesAjustados20);
                                ReportParameter p6 = new ReportParameter("wl_soles_ajustados_veinticinco", solesAjustados25);
                                ReportParameter p7 = new ReportParameter("wl_dolares_ajustados_quince", dolaresAjustados15);
                                ReportParameter p8 = new ReportParameter("wl_dolares_ajustados_veinte", dolaresAjustados20);
                                ReportParameter p9 = new ReportParameter("wl_dolares_ajustados_veinticinco", dolaresAjustados25);
                                ReportParameter p10 = new ReportParameter("wl_ruta_imagen", rutaImagen);

                                log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteSimulacionTipoMoneda"]));
                                log.Debug(String.Format("Parámetros del reporte: wl_soles_indexados_quince[{0}] wl_soles_indexados_veinte[{1}] wl_soles_indexados_veinticinco[{2}] wl_soles_ajustados_quince[{3}] wl_soles_ajustados_veinte[{4}] wl_soles_ajustados_veinticinco[{5}] wl_dolares_ajustados_quince[{6}] wl_dolares_ajustados_veinte[{7}] wl_dolares_ajustados_veinticinco[{8}] wl_ruta_imagen[{9}].",
                                    solesIndexados15, solesIndexados20, solesIndexados25, solesAjustados15, solesAjustados20, solesAjustados25, dolaresAjustados15, dolaresAjustados20, dolaresAjustados25, rutaImagen));
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
        //<SRI.FIN-20322>
    }
}