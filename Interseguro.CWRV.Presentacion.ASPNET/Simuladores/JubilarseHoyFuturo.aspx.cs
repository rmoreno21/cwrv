using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;

using Microsoft.VisualBasic;

using log4net;
using Microsoft.Reporting.WebForms;
using System.Drawing;
using System.Net.Mail;

using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

namespace Interseguro.CWRV.Presentacion.ASPNET.Simuladores
{
    public partial class JubilarseHoyFuturo : System.Web.UI.Page
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
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorJubilarseHoy))
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
                            Enums.OpcionesSistema.SimuladorJubilarseHoy.StringValue()));
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

        private void CargarInformacionInicialPantalla()
        {
            IdSimulador.Value = ((int)Enums.OpcionesSistema.SimuladorJubilarseHoy).ToString();

            if (Session["Consentimiento"] != null)
                FormularioBusqueda.Visible = (bool)Session["Consentimiento"];

            // Cargar combobox Años estimados a jubilarse
            for (int i = Convert.ToInt32(ConfigurationManager.AppSettings["EdadMinJubilacion"]); i <= Convert.ToInt32(ConfigurationManager.AppSettings["EdadMaxJubilacion"]); i++ )
            {
                AnhosJubilarse.Items.Add(i.ToString());
            }

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

        private void CargarInformacionPredeterminada()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> parametros = servicioCotizador.ObtenerParametrosSimuladores();

            Inflacion.Text = parametros[2].Valor.ToString();
            PromedioRentabilitad.Text = parametros[1].Valor.ToString();
            TipoCambio.Text = parametros[0].Valor.ToString();
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

        private void LimpiarFormularios()
        {
            CUSPP.Value = String.Empty;

            AnhosJubilarse.SelectedIndex = AnhosJubilarse.Items.IndexOf(AnhosJubilarse.Items.FindByValue(ConfigurationManager.AppSettings["EdadMinJubilacion"]));
            Inflacion.Text = String.Empty;
            PromedioRentabilitad.Text = String.Empty;
            TipoCambio.Text = String.Empty;
            TasaAjuste.Text = String.Empty;

            AnhosJubilarse.CssClass = AnhosJubilarse.CssClass.Replace(" formComboboxError", String.Empty);
            Inflacion.CssClass = Inflacion.CssClass.Replace(" formTextboxError", String.Empty);
            PromedioRentabilitad.CssClass = PromedioRentabilitad.CssClass.Replace(" formTextboxError", String.Empty);
            TipoCambio.CssClass = TipoCambio.CssClass.Replace(" formTextboxError", String.Empty);
            TasaAjuste.CssClass = TasaAjuste.CssClass.Replace(" formTextboxError", String.Empty);
        }

        [WebMethod]
        public static Respuesta SimularJubilarseHoyFuturo(string tokenUsuario, string idSolicitud, DateTime fechaCotizacion, List<Cotizacion> cotizaciones, 
                                                          List<GrupoFamiliar> beneficiarios, double cic, Int64 correlativo, int edadJubilarse, 
                                                          double tipoCambio, double rentabilidadAFP, double ipc, double ajuste, int orden)
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
                        //var control = (SimuladorJubilarseHoyFuturo)pagina.LoadControl("~/Controles/SimuladorJubilarseHoyFuturo.ascx");
                        SimuladorJubilarseHoyFuturo control = (SimuladorJubilarseHoyFuturo)pagina.LoadControl("~/Controles/SimuladorJubilarseHoyFuturo.ascx");
                        //<SRI.FIN-20322>


                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorJubilarseHoy))
                        {
                            control.PermisoEjecutar = true;

                            Cotizacion cotizacion = cotizaciones.Find(c => c.Correlativo == correlativo);

                            string codMoneda = cotizacion.Moneda.Id;
                            if (codMoneda == Enums.Moneda.Soles.StringValue())
                            {
                                control.Moneda = "Indexada S/.";
                                control.TasaAjuste = ipc;
                            }
                            else if (codMoneda == Enums.Moneda.SolesAjustados.StringValue())
                            {
                                control.Moneda = "Ajustada S/.";
                                control.TasaAjuste = ajuste;
                            }
                            else if (codMoneda == Enums.Moneda.DolaresAjustados.StringValue())
                            {
                                control.Moneda = "Ajustada $";
                                control.TasaAjuste = ajuste;
                            }

                            control.Id = orden;

                            control.EdadActual = Utilitarios.ObtenerEdad(DateTime.Now, (DateTime)beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue()).FechaNacimiento);
                            control.EdadJubilarse = edadJubilarse;

                            // Validación de la edad
                            if (control.EdadActual >= edadJubilarse)
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "El campo <strong>Años estimados a jubilarse</strong> debe ser mayor a la edad actual del afiliado." });
                                return respuesta;
                            }
                            control.SaldoCIC1 = cic;
                            control.SaldoCIC2 = (cic) * Math.Pow(1 + (rentabilidadAFP / 100), edadJubilarse - control.EdadActual);

                            control.Pension1 = cotizacion.PensionCia;
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            control.Pension2 = servicioCotizador.Cotizar(idSolicitud, fechaCotizacion, correlativo, control.EdadJubilarse - control.EdadActual, control.SaldoCIC2).PensionCia;

                            respuesta.Grafico1 = new List<GraficoLineal>();
                            respuesta.Grafico1.Add(new GraficoLineal { Nombre = control.EdadActual + " años", Valor = control.ObtenerPensionTotalHoy() });
                            respuesta.Grafico1.Add(new GraficoLineal { Nombre = control.EdadJubilarse + " años", Valor = control.ObtenerPensionTotalJub() });

                            respuesta.FechaHora = DateTime.Now;

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
                            IdTipoEvento = Enums.EventoLog.Reporte1.StringValue(),
                            Detalle = String.Format("Cotización {0} simulada", correlativo)
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

        //<SRI.INI-20322>
        public static string DescargarImagen(string rutaImagen)
        {

            string ruta = "";

            try
            {
                byte[] imageBytes = Convert.FromBase64String(rutaImagen);
                MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

                ms.Write(imageBytes, 0, imageBytes.Length);

                ruta = GuardarImagen(ms, "Prueba");

            }
            catch( Exception ex)
            {
                System.Console.WriteLine( "Problem: " + ex.Message );

            }

            return ruta;
        }
        public static string GuardarImagen(MemoryStream ms, string FileName)
        {
            try
            {
                string appPath = HttpContext.Current.Request.ApplicationPath;
                string physicalPath = HttpContext.Current.Request.MapPath(appPath);
                string strpath = physicalPath + "Imagenes";
                string WorkingDirectory = strpath;

                System.Drawing.Image imgSave = System.Drawing.Image.FromStream(ms);
                Bitmap bmSave = new Bitmap(imgSave);
                Bitmap bmTemp = new Bitmap(bmSave);

                Graphics grSave = Graphics.FromImage(bmTemp);
                grSave.DrawImage(imgSave, 0, 0, imgSave.Width, imgSave.Height);

                bmTemp.Save(WorkingDirectory + "/" + FileName + ".png");

                imgSave.Dispose();
                bmSave.Dispose();
                bmTemp.Dispose();
                grSave.Dispose();

                return WorkingDirectory + "/" + FileName + ".png";

            }
            catch (Exception ex)
            {
                throw ex;
            }

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

                            SimuladorJubilarseHoyFuturo CntSimulador =  HttpContext.Current.Session["CntSimulador"] as SimuladorJubilarseHoyFuturo;

                            string edadPensionistaHoy = CntSimulador.EdadActual.ToString();
                            string edadPensionistaFuturo = CntSimulador.EdadJubilarse.ToString();
                            string saldoCicHoy = String.Format("{0:###,###,###,##0.00}", CntSimulador.SaldoCIC1).ToString();
                            string saldoCicFuturo = String.Format("{0:###,###,###,##0.00}", CntSimulador.SaldoCIC2).ToString();
                            string pensionMensualHoy = String.Format("{0:###,###,###,##0.00}", CntSimulador.Pension1).ToString();
                            string pensionMensualFuturo = String.Format("{0:###,###,###,##0.00}", CntSimulador.Pension2).ToString();
                            string pensionAnualHoy = String.Format("{0:###,###,###,##0.00}", CntSimulador.PensionAnual1).ToString();
                            string pensionAnualFuturo = String.Format("{0:###,###,###,##0.00}", CntSimulador.PensionAnual2).ToString();
                            string pensionTotalHoy = String.Format("{0:###,###,###,##0.00}", CntSimulador.PensionTotal1).ToString();
                            string pensionTotalFuturo = String.Format("{0:###,###,###,##0.00}", CntSimulador.PensionTotal2).ToString();
                            string tipoSolicitud = CntSimulador.Moneda.ToString();
                            string rutaImagen = rutaImagenSimulada.Substring(22, rutaImagenSimulada.Length - 22);
                            //<SRI.INI-20322_E2>
                            string aniosJubi = String.Format("{0:##0}", CntSimulador.AniosJub).ToString();
                            //<SRI.FIN-20322_E2>
                            
                            ReportViewer visorReporte = new ReportViewer();
                            visorReporte.ProcessingMode = ProcessingMode.Remote;
                            visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                            visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteSimulacionJubilarseHoyFuturo"];

                            ReportParameter p1 = new ReportParameter("wl_edad_pensionista_hoy", edadPensionistaHoy);
                            ReportParameter p2 = new ReportParameter("wl_edad_pensionista_futuro", edadPensionistaFuturo);
                            ReportParameter p3 = new ReportParameter("wl_saldo_cic_hoy", saldoCicHoy);
                            ReportParameter p4 = new ReportParameter("wl_saldo_cic_futuro", saldoCicFuturo);
                            ReportParameter p5 = new ReportParameter("wl_pension_mensual_hoy", pensionMensualHoy);
                            ReportParameter p6 = new ReportParameter("wl_pension_mensual_futuro", pensionMensualFuturo);
                            ReportParameter p7 = new ReportParameter("wl_pension_anual_hoy", pensionAnualHoy);
                            ReportParameter p8 = new ReportParameter("wl_pension_anual_futuro", pensionAnualFuturo);
                            ReportParameter p9 = new ReportParameter("wl_pension_total_hoy", pensionTotalHoy);
                            ReportParameter p10 = new ReportParameter("wl_pension_total_futuro", pensionTotalFuturo);
                            ReportParameter p11 = new ReportParameter("wl_tipo_solicitud", tipoSolicitud);
                            ReportParameter p12 = new ReportParameter("wl_ruta_imagen", rutaImagen);
                            //<SRI.INI-20322_E2>
                            ReportParameter p13 = new ReportParameter("wl_anios_jubilacion", aniosJubi);
                            //<SRI.FIN-20322_E2>

                            log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                ConfigurationManager.AppSettings["DominioReportingServices"],
                                ConfigurationManager.AppSettings["RutaReporteSimulacionJubilarseHoyFuturo"]));
                            //<SRI.INI-20322_E2>
                            //log.Debug(String.Format("Parámetros del reporte: wl_edad_pensionista_hoy[{0}] wl_edad_pensionista_futuro[{1}] wl_saldo_cic_hoy[{2}] wl_saldo_cic_futuro[{3}] wl_pension_mensual_hoy[{4}] wl_pension_mensual_futuro[{5}] wl_pension_anual_hoy[{6}] wl_pension_anual_futuro[{7}] wl_pension_total_hoy[{8}] wl_pension_total_futuro[{9}] wl_tipo_solicitud[{10}] wl_ruta_imagen[{11}].",
                           //edadPensionistaHoy, edadPensionistaFuturo, saldoCicHoy, saldoCicFuturo, pensionMensualHoy, pensionMensualFuturo, pensionAnualHoy, pensionAnualFuturo, pensionTotalHoy, pensionTotalFuturo, tipoSolicitud, rutaImagen));
                            //visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12 });
                                log.Debug(String.Format("Parámetros del reporte: wl_edad_pensionista_hoy[{0}] wl_edad_pensionista_futuro[{1}] wl_saldo_cic_hoy[{2}] wl_saldo_cic_futuro[{3}] wl_pension_mensual_hoy[{4}] wl_pension_mensual_futuro[{5}] wl_pension_anual_hoy[{6}] wl_pension_anual_futuro[{7}] wl_pension_total_hoy[{8}] wl_pension_total_futuro[{9}] wl_tipo_solicitud[{10}] wl_ruta_imagen[{11}] wl_anios_jubilacion[{12}].",
                                edadPensionistaHoy, edadPensionistaFuturo, saldoCicHoy, saldoCicFuturo, pensionMensualHoy, pensionMensualFuturo, pensionAnualHoy, pensionAnualFuturo, pensionTotalHoy, pensionTotalFuturo, tipoSolicitud, rutaImagen, aniosJubi));
                            visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13 });
                            //<SRI.FIN-20322_E2>
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