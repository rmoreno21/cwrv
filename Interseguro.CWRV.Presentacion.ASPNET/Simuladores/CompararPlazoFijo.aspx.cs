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
    public partial class CompararPlazoFijo : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CompararPlazoFijo));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    HCompara.Value = "TRUE";
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorRentasVitaliciasVsPlazoFijo))
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
                            Enums.OpcionesSistema.SimuladorRentasVitaliciasVsPlazoFijo.StringValue()));
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
            TasaAjusteAJS.Text = parametros[3].Valor.ToString();
            TipoCambioParametro.Text = parametros[0].Valor.ToString();
            //PromedioRentabilitad.Text = parametros[1].Valor.ToString();
            //Inflacion.Text = parametros[2].Valor.ToString();


            //List<Parametro> parametro = new List<Parametro>();
            List<Parametro> parametro = servicioCotizador.ObtenerParametrosPorTabla("PLAZOFIJO");
            if (parametro.Count > 0)
            {
                TEA.Text = parametro.FindAll(p => p.Correlativo == 1)[0].Valor_1;
                TasaAjusteIDX.Text = parametro.FindAll(p => p.Correlativo == 2)[0].Valor_1;
                Crecimiento.Text = parametro.FindAll(p => p.Correlativo == 3)[0].Valor_1;
            }


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

            //TEA.Text = "5.0";
            //Anhos.Text = "20";
            //TasaAjusteIDX.Text = "3.5";
            //TasaAjusteAJS.Text = "2.0";
            //Crecimiento.Text = "0.5";

            for (int año = 1; año <= 20; año++)
            {
                Anhos.Items.Add(new ListItem(año.ToString(), año.ToString()));
            }
            Anhos.SelectedValue = "20";

        }

        private void LimpiarFormularios()
        {
            CUSPP.Value = String.Empty;
            TipoCambioParametro.Text = String.Empty;
            TEA.Text = String.Empty;
            DepositoPlazo.Text = String.Empty;
            InteresDeposito.Text = String.Empty;
            Anhos.Text = String.Empty;
            //TEM.Text = String.Empty;
            TasaAjusteAJS.Text = String.Empty;
            TasaAjusteIDX.Text = String.Empty;
            Crecimiento.Text = String.Empty;

            TipoCambioParametro.CssClass = TipoCambioParametro.CssClass.Replace(" formTextboxError", String.Empty);

            TEA.CssClass = TEA.CssClass.Replace(" formTextboxError", String.Empty);
            DepositoPlazo.CssClass = DepositoPlazo.CssClass.Replace(" formTextboxError", String.Empty);
            InteresDeposito.CssClass = InteresDeposito.CssClass.Replace(" formTextboxError", String.Empty);
            Anhos.CssClass = Anhos.CssClass.Replace(" formTextboxError", String.Empty);
            //TEM.CssClass = TEM.CssClass.Replace(" formTextboxError", String.Empty);
            TasaAjusteAJS.CssClass = TasaAjusteAJS.CssClass.Replace(" formTextboxError", String.Empty);
            TasaAjusteIDX.CssClass = TasaAjusteIDX.CssClass.Replace(" formTextboxError", String.Empty);
            Crecimiento.CssClass = Crecimiento.CssClass.Replace(" formTextboxError", String.Empty);

        }

        private void CargarInformacionInicialPantalla()
        {
            IdSimulador.Value = "90";//((int)Enums.OpcionesSistema.SimuladorQueMeConviene).ToString();

            if (Session["Consentimiento"] != null)
                FormularioBusqueda.Visible = (bool)Session["Consentimiento"];



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

                                SimuladorPlazoFijo CntSimulador = HttpContext.Current.Session["CntSimulador"] as SimuladorPlazoFijo;

                                List<CuadroComparativo> lstCuadro = new List<CuadroComparativo>();
                                string sTitulo = "Pensión en el Tiempo";

                                //lstCuadro.Add(new CuadroComparativo{ Id=0, Titulo = sTitulo, Descripcion="Aplicación en el Fondo", Valor=CntSimulador.MOD101Pension});
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD101Pension, Anho = "Año 01" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD102Pension, Anho = "Año 02" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD103Pension, Anho = "Año 03" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD104Pension, Anho = "Año 04" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD105Pension, Anho = "Año 05" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD106Pension, Anho = "Año 06" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD107Pension, Anho = "Año 07" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD108Pension, Anho = "Año 08" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD109Pension, Anho = "Año 09" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD110Pension, Anho = "Año 10" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD111Pension, Anho = "Año 11" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD112Pension, Anho = "Año 12" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD113Pension, Anho = "Año 13" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD114Pension, Anho = "Año 14" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD115Pension, Anho = "Año 15" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD116Pension, Anho = "Año 16" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD117Pension, Anho = "Año 17" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD118Pension, Anho = "Año 18" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD119Pension, Anho = "Año 19" });
                                lstCuadro.Add(new CuadroComparativo { Id = 0, Titulo = sTitulo, Descripcion = "Aplicación del IPC", Valor = CntSimulador.MOD120Pension, Anho = "Año 20" });

                                var xmlCuadro1 = GeneraXmlCuadro(lstCuadro);

                                lstCuadro = new List<CuadroComparativo>();

                                sTitulo = "Total Mensual";

                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2101TotalMensual, Anho = "Año 01" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2102TotalMensual, Anho = "Año 02" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2103TotalMensual, Anho = "Año 03" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2104TotalMensual, Anho = "Año 04" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2105TotalMensual, Anho = "Año 05" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2106TotalMensual, Anho = "Año 06" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2107TotalMensual, Anho = "Año 07" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2108TotalMensual, Anho = "Año 08" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2109TotalMensual, Anho = "Año 09" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2110TotalMensual, Anho = "Año 10" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2111TotalMensual, Anho = "Año 11" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2112TotalMensual, Anho = "Año 12" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2113TotalMensual, Anho = "Año 13" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2114TotalMensual, Anho = "Año 14" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2115TotalMensual, Anho = "Año 15" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2116TotalMensual, Anho = "Año 16" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2117TotalMensual, Anho = "Año 17" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2118TotalMensual, Anho = "Año 18" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2119TotalMensual, Anho = "Año 19" });
                                lstCuadro.Add(new CuadroComparativo { Id = 1, Titulo = sTitulo, Descripcion = "Interés Plazo", Valor = CntSimulador.MOD2120TotalMensual, Anho = "Año 20" });

                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2201TotalMensual, Anho = "Año 01" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2202TotalMensual, Anho = "Año 02" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2203TotalMensual, Anho = "Año 03" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2204TotalMensual, Anho = "Año 04" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2205TotalMensual, Anho = "Año 05" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2206TotalMensual, Anho = "Año 06" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2207TotalMensual, Anho = "Año 07" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2208TotalMensual, Anho = "Año 08" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2209TotalMensual, Anho = "Año 09" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2210TotalMensual, Anho = "Año 10" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2211TotalMensual, Anho = "Año 11" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2212TotalMensual, Anho = "Año 12" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2213TotalMensual, Anho = "Año 13" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2214TotalMensual, Anho = "Año 14" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2215TotalMensual, Anho = "Año 15" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2216TotalMensual, Anho = "Año 16" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2217TotalMensual, Anho = "Año 17" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2218TotalMensual, Anho = "Año 18" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2219TotalMensual, Anho = "Año 19" });
                                lstCuadro.Add(new CuadroComparativo { Id = 2, Titulo = sTitulo, Descripcion = CntSimulador.MOD2200Leyenda, Valor = CntSimulador.MOD2220TotalMensual, Anho = "Año 20" });

                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2301TotalMensual, Anho = "Año 01" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2302TotalMensual, Anho = "Año 02" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2303TotalMensual, Anho = "Año 03" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2304TotalMensual, Anho = "Año 04" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2305TotalMensual, Anho = "Año 05" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2306TotalMensual, Anho = "Año 06" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2307TotalMensual, Anho = "Año 07" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2308TotalMensual, Anho = "Año 08" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2309TotalMensual, Anho = "Año 09" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2310TotalMensual, Anho = "Año 10" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2311TotalMensual, Anho = "Año 11" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2312TotalMensual, Anho = "Año 12" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2313TotalMensual, Anho = "Año 13" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2314TotalMensual, Anho = "Año 14" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2315TotalMensual, Anho = "Año 15" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2316TotalMensual, Anho = "Año 16" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2317TotalMensual, Anho = "Año 17" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2318TotalMensual, Anho = "Año 18" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2319TotalMensual, Anho = "Año 19" });
                                lstCuadro.Add(new CuadroComparativo { Id = 3, Titulo = sTitulo, Descripcion = CntSimulador.MOD2300Leyenda, Valor = CntSimulador.MOD2320TotalMensual, Anho = "Año 20" });

                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2401TotalMensual, Anho = "Año 01" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2402TotalMensual, Anho = "Año 02" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2403TotalMensual, Anho = "Año 03" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2404TotalMensual, Anho = "Año 04" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2405TotalMensual, Anho = "Año 05" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2406TotalMensual, Anho = "Año 06" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2407TotalMensual, Anho = "Año 07" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2408TotalMensual, Anho = "Año 08" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2409TotalMensual, Anho = "Año 09" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2410TotalMensual, Anho = "Año 10" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2411TotalMensual, Anho = "Año 11" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2412TotalMensual, Anho = "Año 12" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2413TotalMensual, Anho = "Año 13" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2414TotalMensual, Anho = "Año 14" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2415TotalMensual, Anho = "Año 15" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2416TotalMensual, Anho = "Año 16" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2417TotalMensual, Anho = "Año 17" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2418TotalMensual, Anho = "Año 18" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2419TotalMensual, Anho = "Año 19" });
                                lstCuadro.Add(new CuadroComparativo { Id = 4, Titulo = sTitulo, Descripcion = CntSimulador.MOD2400Leyenda, Valor = CntSimulador.MOD2420TotalMensual, Anho = "Año 20" });

                                var xmlCuadro2 = GeneraXmlCuadro(lstCuadro);

                                string rutaImagen = rutaImagenSimulada.Substring(22, rutaImagenSimulada.Length - 22);



                                ReportViewer visorReporte = new ReportViewer();
                                visorReporte.ProcessingMode = ProcessingMode.Remote;
                                visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteSimulacionComparativo"];

                                ReportParameter p1 = new ReportParameter("wl_data", xmlCuadro1);
                                ReportParameter p2 = new ReportParameter("wl_data2", xmlCuadro2);

                                ReportParameter p31 = new ReportParameter("wl_ruta_imagen", rutaImagen);

                                log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteSimulacionQueMeConviene"]));
                                log.Debug(String.Format("Parámetros del reporte: wl_data[{0}] wl_data2[{1}] wl_ruta_imagen[{2}] .", xmlCuadro1, xmlCuadro2, rutaImagen));

                                visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p31 });
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


        [WebMethod]
        public static Respuesta SimularPlazoFijo(string tokenUsuario, List<Cotizacion> cotizaciones, List<string> cotizacion, string depositoPlazo, string tea, // string tem, string interesDeposito, 
                                                string ajusteIDX, string ajusteAJS, string tipoCambio, string crecimiento, string anhos, string CUSPP)
        {

            Respuesta respuesta = new Respuesta();
            try
            {
                double DepositoPlazo = 0, Tem = 0, InteresDeposito = 0, AjusteIDX = 0, AjusteAJS = 0, TipoCambio = 0, Crecimiento = 0, TipoCambioOrigen = 0;


                double MOD2201TotMenOrigen = 0, MOD2202TotMenOrigen = 0, MOD2203TotMenOrigen = 0, MOD2204TotMenOrigen = 0, MOD2205TotMenOrigen = 0;
                double MOD2206TotMenOrigen = 0, MOD2207TotMenOrigen = 0, MOD2208TotMenOrigen = 0, MOD2209TotMenOrigen = 0, MOD2210TotMenOrigen = 0;
                double MOD2211TotMenOrigen = 0, MOD2212TotMenOrigen = 0, MOD2213TotMenOrigen = 0, MOD2214TotMenOrigen = 0, MOD2215TotMenOrigen = 0;
                double MOD2216TotMenOrigen = 0, MOD2217TotMenOrigen = 0, MOD2218TotMenOrigen = 0, MOD2219TotMenOrigen = 0, MOD2220TotMenOrigen = 0;

                double MOD2301TotMenOrigen = 0, MOD2302TotMenOrigen = 0, MOD2303TotMenOrigen = 0, MOD2304TotMenOrigen = 0, MOD2305TotMenOrigen = 0;
                double MOD2306TotMenOrigen = 0, MOD2307TotMenOrigen = 0, MOD2308TotMenOrigen = 0, MOD2309TotMenOrigen = 0, MOD2310TotMenOrigen = 0;
                double MOD2311TotMenOrigen = 0, MOD2312TotMenOrigen = 0, MOD2313TotMenOrigen = 0, MOD2314TotMenOrigen = 0, MOD2315TotMenOrigen = 0;
                double MOD2316TotMenOrigen = 0, MOD2317TotMenOrigen = 0, MOD2318TotMenOrigen = 0, MOD2319TotMenOrigen = 0, MOD2320TotMenOrigen = 0;

                double MOD2401TotMenOrigen = 0, MOD2402TotMenOrigen = 0, MOD2403TotMenOrigen = 0, MOD2404TotMenOrigen = 0, MOD2405TotMenOrigen = 0;
                double MOD2406TotMenOrigen = 0, MOD2407TotMenOrigen = 0, MOD2408TotMenOrigen = 0, MOD2409TotMenOrigen = 0, MOD2410TotMenOrigen = 0;
                double MOD2411TotMenOrigen = 0, MOD2412TotMenOrigen = 0, MOD2413TotMenOrigen = 0, MOD2414TotMenOrigen = 0, MOD2415TotMenOrigen = 0;
                double MOD2416TotMenOrigen = 0, MOD2417TotMenOrigen = 0, MOD2418TotMenOrigen = 0, MOD2419TotMenOrigen = 0, MOD2420TotMenOrigen = 0;

                double Ajuste1 = 0, Ajuste2 = 0, Ajuste3 = 3;

                double Tea = 0;
                string nombreTerminal = String.Empty;
                int añoMax;
                Tea = Convert.ToDouble(tea, new CultureInfo("es-PE"));
                var a = 1 + (Tea / 100.0);
                var b = 1 / 12.0;

                Tem = (Math.Pow(a, b) - 1) * 100.0;

                if (anhos.Equals(""))
                {
                    anhos = "20";
                }

                añoMax = Convert.ToInt16(Convert.ToDouble(anhos, new CultureInfo("es-PE")));


                DepositoPlazo = Convert.ToDouble(depositoPlazo, new CultureInfo("es-PE"));
                //Tem = Convert.ToDouble(tem, new CultureInfo("es-PE"));
                //InteresDeposito = Convert.ToDouble(interesDeposito, new CultureInfo("es-PE"));
                InteresDeposito = Math.Round(DepositoPlazo * (Tem / 100.0), 2, MidpointRounding.AwayFromZero);

                AjusteIDX = Convert.ToDouble(ajusteIDX, new CultureInfo("es-PE"));
                AjusteAJS = Convert.ToDouble(ajusteAJS, new CultureInfo("es-PE"));
                TipoCambio = Convert.ToDouble(tipoCambio, new CultureInfo("es-PE"));
                TipoCambioOrigen = TipoCambio;
                Crecimiento = Convert.ToDouble(crecimiento, new CultureInfo("es-PE"));


                //Convert.ToDouble(saldoCIC, new CultureInfo("es-PE")),

                if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                {
                    int año = 0;
                    var pagina = new Page();

                    SimuladorPlazoFijo control = (SimuladorPlazoFijo)pagina.LoadControl("~/Controles/SimuladorPlazoFijo.ascx");

                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorRentasVitaliciasVsPlazoFijo))
                    {
                        control.PermisoEjecutar = true;

                        Cotizacion cotizacion1 = null;
                        Cotizacion cotizacion2 = null;
                        Cotizacion cotizacion3 = null;

                        List<GraficoLineal> Grafico1 = new List<GraficoLineal>();
                        List<GraficoLineal> Grafico2 = new List<GraficoLineal>();
                        List<GraficoLineal> Grafico3 = new List<GraficoLineal>();
                        List<GraficoLineal> Grafico4 = new List<GraficoLineal>();

                        //Registrando en el Log


                        string wl_usuario = string.Empty;
                        try
                        {
                            nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                        }
                        catch (Exception)
                        {
                            log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                HttpContext.Current.Request.ServerVariables["remote_addr"]));
                        }

                        wl_usuario = HttpContext.Current.Session["Usuario"].ToString();
                        nombreTerminal += HttpContext.Current.Request.UserAgent;

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        servicioCotizador.RegistrarLog(new LogBD
                        {
                            IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                            NombreTerminal = nombreTerminal,
                            IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                            NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                            IdTipoEvento = Enums.EventoLog.SimuladorVitaliciaVsPlazoFijo.StringValue(),
                            Detalle = String.Format("Simulando Cálculo afiliado CUSPP {0} usuario {1} ", CUSPP, wl_usuario)
                        });


                        cotizacion1 = cotizaciones.Where(c => c.Correlativo == Convert.ToInt32(cotizacion[0])).Select(c => new Cotizacion()
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
                            PrimeraPensionRVD = c.PrimeraPensionRVD
                        }).First();

                        if (cotizacion.Count > 1)
                        {
                            cotizacion2 = cotizaciones.Where(c => c.Correlativo == Convert.ToInt32(cotizacion[1])).Select(c => new Cotizacion()
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
                                PrimeraPensionRVD = c.PrimeraPensionRVD
                            }).First();

                        }

                        if (cotizacion.Count > 2)
                        {
                            cotizacion3 = cotizaciones.Where(c => c.Correlativo == Convert.ToInt32(cotizacion[2])).Select(c => new Cotizacion()
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
                                PrimeraPensionRVD = c.PrimeraPensionRVD
                            }).First();

                        }

                        Ajuste1 = AjusteIDX;
                        Ajuste2 = AjusteIDX;
                        Ajuste3 = AjusteIDX;


                        if (cotizacion1 != null)
                        {
                            if (cotizacion1.Moneda.Id == "013" || cotizacion1.Moneda.Id == "014")
                                Ajuste1 = AjusteAJS;

                            for (año = 1; año <= añoMax; año++)
                            {

                                if (año > 1)
                                {
                                    TipoCambio = TipoCambio * (1 + (Crecimiento / 100.00));
                                }

                                control.MOD2200Leyenda = cotizacion1.Modalidad.Id + " " + cotizacion1.Moneda.Nombre + (cotizacion1.PeriodoGarantizado != 0 ? " PG. " + cotizacion1.PeriodoGarantizado : "");
                                switch (año)
                                {
                                    case 1:
                                        control.MOD101Pension = DepositoPlazo;
                                        control.MOD2101TotalMensual = InteresDeposito;
                                        MOD2201TotMenOrigen = cotizacion1.PensionCiaMO; //* TipoCambio;

                                        control.MOD2201TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2201TotMenOrigen : MOD2201TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2101TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2201TotalMensual });
                                        break;
                                    case 2:

                                        control.MOD102Pension = control.MOD101Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2102TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2202TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2202TotMenOrigen = MOD2201TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2202TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2202TotMenOrigen : MOD2202TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2102TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2202TotalMensual });
                                        break;
                                    case 3:
                                        control.MOD103Pension = control.MOD102Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2103TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2203TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2203TotMenOrigen = MOD2202TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2203TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2203TotMenOrigen : MOD2203TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2103TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2203TotalMensual });
                                        break;
                                    case 4:

                                        control.MOD104Pension = control.MOD103Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2104TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2204TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2204TotMenOrigen = MOD2203TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }



                                        control.MOD2204TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2204TotMenOrigen : MOD2204TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2104TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2204TotalMensual });
                                        break;
                                    case 5:

                                        control.MOD105Pension = control.MOD104Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2105TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2205TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2205TotMenOrigen = MOD2204TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2205TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2205TotMenOrigen : MOD2205TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2105TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2205TotalMensual });
                                        break;
                                    case 6:
                                        control.MOD106Pension = control.MOD105Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2106TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2206TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2206TotMenOrigen = MOD2205TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }



                                        control.MOD2206TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2206TotMenOrigen : MOD2206TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2106TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2206TotalMensual });
                                        break;
                                    case 7:
                                        control.MOD107Pension = control.MOD106Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2107TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2207TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2207TotMenOrigen = MOD2206TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2207TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2207TotMenOrigen : MOD2207TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2107TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2207TotalMensual });
                                        break;
                                    case 8:
                                        control.MOD108Pension = control.MOD107Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2108TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2208TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2208TotMenOrigen = MOD2207TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2208TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2208TotMenOrigen : MOD2208TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2108TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2208TotalMensual });
                                        break;
                                    case 9:
                                        control.MOD109Pension = control.MOD108Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2109TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2209TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2209TotMenOrigen = MOD2208TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2209TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2209TotMenOrigen : MOD2209TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2109TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2209TotalMensual });
                                        break;
                                    case 10:
                                        control.MOD110Pension = control.MOD109Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2110TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2210TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2210TotMenOrigen = MOD2209TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2210TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2210TotMenOrigen : MOD2210TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2110TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2210TotalMensual });
                                        break;
                                    case 11:
                                        control.MOD111Pension = control.MOD110Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2111TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2211TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2211TotMenOrigen = MOD2210TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }

                                        control.MOD2211TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2211TotMenOrigen : MOD2211TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2111TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2211TotalMensual });
                                        break;
                                    case 12:
                                        control.MOD112Pension = control.MOD111Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2112TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2212TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2212TotMenOrigen = MOD2211TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2212TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2212TotMenOrigen : MOD2212TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2112TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2212TotalMensual });
                                        break;
                                    case 13:
                                        control.MOD113Pension = control.MOD112Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2113TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2213TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2213TotMenOrigen = MOD2212TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2213TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2213TotMenOrigen : MOD2213TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2113TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2213TotalMensual });
                                        break;
                                    case 14:
                                        control.MOD114Pension = control.MOD113Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2114TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2214TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2214TotMenOrigen = MOD2213TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }

                                        control.MOD2214TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2214TotMenOrigen : MOD2214TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2114TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2214TotalMensual });
                                        break;
                                    case 15:
                                        control.MOD115Pension = control.MOD114Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2115TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2215TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2215TotMenOrigen = MOD2214TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2215TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2215TotMenOrigen : MOD2215TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2115TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2215TotalMensual });
                                        break;
                                    case 16:
                                        control.MOD116Pension = control.MOD115Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2116TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2216TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2216TotMenOrigen = MOD2215TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2216TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2216TotMenOrigen : MOD2216TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2116TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2216TotalMensual });
                                        break;
                                    case 17:
                                        control.MOD117Pension = control.MOD116Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2117TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2217TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2217TotMenOrigen = MOD2216TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }

                                        control.MOD2217TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2217TotMenOrigen : MOD2217TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2117TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2217TotalMensual });
                                        break;
                                    case 18:
                                        control.MOD118Pension = control.MOD117Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2118TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2218TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2218TotMenOrigen = MOD2217TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2218TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2218TotMenOrigen : MOD2218TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2118TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2218TotalMensual });
                                        break;
                                    case 19:
                                        control.MOD119Pension = control.MOD118Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2119TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2219TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2219TotMenOrigen = MOD2218TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }

                                        control.MOD2219TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2219TotMenOrigen : MOD2219TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2119TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2219TotalMensual });
                                        break;
                                    case 20:
                                        control.MOD120Pension = control.MOD119Pension * (1 - (AjusteIDX / 100.00));
                                        control.MOD2120TotalMensual = InteresDeposito;
                                        if (cotizacion1.PeriodoDiferido + 1 == año && cotizacion1.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2220TotMenOrigen = cotizacion1.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2220TotMenOrigen = MOD2219TotMenOrigen * (1 + (Ajuste1 / 100.00));
                                        }


                                        control.MOD2220TotalMensual = (cotizacion1.Moneda.Id == "001" || cotizacion1.Moneda.Id == "013") ? MOD2220TotMenOrigen : MOD2220TotMenOrigen * TipoCambio;

                                        Grafico1.Add(new GraficoLineal { Nombre = "PLAZO FIJO", Valor = control.MOD2120TotalMensual });
                                        Grafico2.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2220TotalMensual });
                                        break;
                                    default:
                                        Console.WriteLine("Default case");
                                        break;
                                }
                            }
                            respuesta.Grafico1 = Grafico1;
                            respuesta.Grafico2 = Grafico2;

                        }


                        if (cotizacion2 != null)
                        {
                            TipoCambio = TipoCambioOrigen;
                            if (cotizacion2.Moneda.Id == "013" || cotizacion2.Moneda.Id == "014")
                                Ajuste2 = AjusteAJS;

                            for (año = 1; año <= añoMax; año++)
                            {

                                if (año > 1)
                                {
                                    TipoCambio = TipoCambio * (1 + (Crecimiento / 100.00));
                                }


                                control.MOD2300Leyenda = cotizacion2.Modalidad.Id + " " + cotizacion2.Moneda.Nombre + (cotizacion2.PeriodoGarantizado != 0 ? " PG. " + cotizacion2.PeriodoGarantizado : "");

                                switch (año)
                                {
                                    case 1:
                                        MOD2301TotMenOrigen = cotizacion2.PensionCiaMO; //* TipoCambio;
                                        control.MOD2301TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2301TotMenOrigen : MOD2301TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2301TotalMensual });
                                        break;
                                    case 2:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2302TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2302TotMenOrigen = MOD2301TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2302TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2302TotMenOrigen : MOD2302TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2302TotalMensual });
                                        break;
                                    case 3:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2303TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2303TotMenOrigen = MOD2302TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2303TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2303TotMenOrigen : MOD2303TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2303TotalMensual });
                                        break;
                                    case 4:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2304TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2304TotMenOrigen = MOD2303TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }


                                        control.MOD2304TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2304TotMenOrigen : MOD2304TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2304TotalMensual });
                                        break;
                                    case 5:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2305TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2305TotMenOrigen = MOD2304TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }


                                        control.MOD2305TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2305TotMenOrigen : MOD2305TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2305TotalMensual });
                                        break;
                                    case 6:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2306TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2306TotMenOrigen = MOD2305TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2306TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2306TotMenOrigen : MOD2306TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2306TotalMensual });
                                        break;
                                    case 7:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2307TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2307TotMenOrigen = MOD2306TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }


                                        control.MOD2307TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2307TotMenOrigen : MOD2307TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2307TotalMensual });
                                        break;
                                    case 8:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2308TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2308TotMenOrigen = MOD2307TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2308TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2308TotMenOrigen : MOD2308TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2308TotalMensual });
                                        break;
                                    case 9:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2309TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2309TotMenOrigen = MOD2308TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }


                                        control.MOD2309TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2309TotMenOrigen : MOD2309TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2309TotalMensual });
                                        break;
                                    case 10:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2310TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2310TotMenOrigen = MOD2309TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2310TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2310TotMenOrigen : MOD2310TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2310TotalMensual });
                                        break;
                                    case 11:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2311TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2311TotMenOrigen = MOD2310TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }


                                        control.MOD2311TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2311TotMenOrigen : MOD2311TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2311TotalMensual });
                                        break;
                                    case 12:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2312TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2312TotMenOrigen = MOD2311TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2312TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2312TotMenOrigen : MOD2312TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2312TotalMensual });
                                        break;
                                    case 13:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2313TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2313TotMenOrigen = MOD2312TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }


                                        control.MOD2313TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2313TotMenOrigen : MOD2313TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2313TotalMensual });
                                        break;
                                    case 14:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2314TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2314TotMenOrigen = MOD2313TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2314TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2314TotMenOrigen : MOD2314TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2314TotalMensual });
                                        break;
                                    case 15:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2315TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2315TotMenOrigen = MOD2314TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2315TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2315TotMenOrigen : MOD2315TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2315TotalMensual });
                                        break;
                                    case 16:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2316TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2316TotMenOrigen = MOD2315TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2316TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2316TotMenOrigen : MOD2316TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2316TotalMensual });
                                        break;
                                    case 17:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2317TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2317TotMenOrigen = MOD2316TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2317TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2317TotMenOrigen : MOD2317TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2317TotalMensual });
                                        break;
                                    case 18:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2318TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2318TotMenOrigen = MOD2317TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2318TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2318TotMenOrigen : MOD2318TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2318TotalMensual });
                                        break;
                                    case 19:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2319TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2319TotMenOrigen = MOD2318TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }


                                        control.MOD2319TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2319TotMenOrigen : MOD2319TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2319TotalMensual });
                                        break;
                                    case 20:
                                        if (cotizacion2.PeriodoDiferido + 1 == año && cotizacion2.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2320TotMenOrigen = cotizacion2.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2320TotMenOrigen = MOD2319TotMenOrigen * (1 + (Ajuste2 / 100.00));
                                        }

                                        control.MOD2320TotalMensual = (cotizacion2.Moneda.Id == "001" || cotizacion2.Moneda.Id == "013") ? MOD2320TotMenOrigen : MOD2320TotMenOrigen * TipoCambio;

                                        Grafico3.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2320TotalMensual });
                                        break;
                                    default:
                                        Console.WriteLine("Default case");
                                        break;
                                }
                            }
                            respuesta.Grafico3 = Grafico3;

                        }

                        if (cotizacion3 != null)
                        {
                            TipoCambio = TipoCambioOrigen;
                            if (cotizacion3.Moneda.Id == "013" || cotizacion3.Moneda.Id == "014")
                                Ajuste3 = AjusteAJS;

                            for (año = 1; año <= añoMax; año++)
                            {

                                if (año > 1)
                                {
                                    TipoCambio = TipoCambio * (1 + (Crecimiento / 100.00));
                                }

                                control.MOD2400Leyenda = cotizacion3.Modalidad.Id + " " + cotizacion3.Moneda.Nombre + (cotizacion3.PeriodoGarantizado != 0 ? " PG. " + cotizacion3.PeriodoGarantizado : "");

                                switch (año)
                                {
                                    case 1:
                                        MOD2401TotMenOrigen = cotizacion3.PensionCiaMO; //* TipoCambio;
                                        control.MOD2401TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2401TotMenOrigen : MOD2401TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2401TotalMensual });
                                        break;
                                    case 2:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2402TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2402TotMenOrigen = MOD2401TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2402TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2402TotMenOrigen : MOD2402TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2402TotalMensual });
                                        break;
                                    case 3:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2403TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2403TotMenOrigen = MOD2402TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2403TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2403TotMenOrigen : MOD2403TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2403TotalMensual });
                                        break;
                                    case 4:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2404TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2404TotMenOrigen = MOD2403TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2404TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2404TotMenOrigen : MOD2404TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2404TotalMensual });
                                        break;
                                    case 5:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2405TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2405TotMenOrigen = MOD2404TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2405TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2405TotMenOrigen : MOD2405TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2405TotalMensual });
                                        break;
                                    case 6:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2406TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2406TotMenOrigen = MOD2405TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2406TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2406TotMenOrigen : MOD2406TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2406TotalMensual });
                                        break;
                                    case 7:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2407TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2407TotMenOrigen = MOD2406TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2407TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2407TotMenOrigen : MOD2407TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2407TotalMensual });
                                        break;
                                    case 8:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2408TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2408TotMenOrigen = MOD2407TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2408TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2408TotMenOrigen : MOD2408TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2408TotalMensual });
                                        break;
                                    case 9:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2409TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2409TotMenOrigen = MOD2408TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2409TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2409TotMenOrigen : MOD2409TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2409TotalMensual });
                                        break;
                                    case 10:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2410TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2410TotMenOrigen = MOD2409TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2410TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2410TotMenOrigen : MOD2410TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2410TotalMensual });
                                        break;
                                    case 11:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2411TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2411TotMenOrigen = MOD2410TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2411TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2411TotMenOrigen : MOD2411TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2411TotalMensual });
                                        break;
                                    case 12:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2412TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2412TotMenOrigen = MOD2411TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2412TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2412TotMenOrigen : MOD2412TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2412TotalMensual });
                                        break;
                                    case 13:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2413TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2413TotMenOrigen = MOD2412TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2413TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2413TotMenOrigen : MOD2413TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2413TotalMensual });
                                        break;
                                    case 14:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2414TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2414TotMenOrigen = MOD2413TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2414TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2414TotMenOrigen : MOD2414TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2414TotalMensual });
                                        break;
                                    case 15:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2415TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2415TotMenOrigen = MOD2414TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2415TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2415TotMenOrigen : MOD2415TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2415TotalMensual });
                                        break;
                                    case 16:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2416TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2416TotMenOrigen = MOD2415TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2416TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2416TotMenOrigen : MOD2416TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2416TotalMensual });
                                        break;
                                    case 17:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2417TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2417TotMenOrigen = MOD2416TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }

                                        control.MOD2417TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2417TotMenOrigen : MOD2417TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2417TotalMensual });
                                        break;
                                    case 18:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2418TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2418TotMenOrigen = MOD2417TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }

                                        control.MOD2418TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2418TotMenOrigen : MOD2418TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2418TotalMensual });
                                        break;
                                    case 19:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2419TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2419TotMenOrigen = MOD2418TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2419TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2419TotMenOrigen : MOD2419TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2419TotalMensual });
                                        break;
                                    case 20:
                                        if (cotizacion3.PeriodoDiferido + 1 == año && cotizacion3.Modalidad.Id == Enums.Modalidad.Escalonada.StringValue())
                                        {
                                            MOD2420TotMenOrigen = cotizacion3.PrimeraPensionRVD;
                                        }
                                        else
                                        {
                                            MOD2420TotMenOrigen = MOD2419TotMenOrigen * (1 + (Ajuste3 / 100.00));
                                        }


                                        control.MOD2420TotalMensual = (cotizacion3.Moneda.Id == "001" || cotizacion3.Moneda.Id == "013") ? MOD2420TotMenOrigen : MOD2420TotMenOrigen * TipoCambio;

                                        Grafico4.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = control.MOD2420TotalMensual });
                                        break;
                                    default:
                                        Console.WriteLine("Default case");
                                        break;
                                }
                            }
                            respuesta.Grafico4 = Grafico4;

                        }

                    }
                    else
                    {
                        control.PermisoEjecutar = false;
                    }

                    //string nombreTerminal = String.Empty;
                    try
                    {
                        nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                    }
                    catch (Exception)
                    {
                        nombreTerminal = "";
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
                        //Detalle = String.Format("Cotizaciones Nodalidad 1 [{0}] Nodalidad 2 [{1}] Nodalidad 3 [{2}] simuladas", correlativo1, correlativo2, correlativo3)

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
                //respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { String.Format("La solicitud [{0}] no contiene los datos suficientes para generar la simulación, por favor intente generando una nueva cotización.", idSolicitud) });
                respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { String.Format("La solicitud [{0}] no contiene los datos suficientes para generar la simulación, por favor intente generando una nueva cotización.") });
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


        public static string GeneraXmlCuadro(List<CuadroComparativo> lstCuadroComparativo)
        {
            XDocument documento = new XDocument();
            documento.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement sols = new XElement("Cuadros");
            int año = 1;
            foreach (CuadroComparativo cuadro in lstCuadroComparativo)
            {
                if (cuadro.Valor != 0)
                {
                    sols.Add(
                        new XElement("Cuadro",
                            new XElement("id", cuadro.Id),
                            new XElement("titulo", cuadro.Titulo),
                            new XElement("descripcion", cuadro.Descripcion),
                            new XElement("anho", cuadro.Anho),
                            new XElement("valor", cuadro.Valor)
                        )
                    );
                }
                año += año;
            }
            documento.Add(sols);
            return documento.ToString();
        }


    }
}
