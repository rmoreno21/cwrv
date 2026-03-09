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

namespace Interseguro.CWRV.Presentacion.ASPNET.Simuladores
{
    public partial class RentaVitaliciaRetiroProgramado : System.Web.UI.Page
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
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorRentaVitaliciaRetiroProgramado))
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
                            Enums.OpcionesSistema.SimuladorRentaVitaliciaRetiroProgramado.StringValue()));
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
            PromedioRentabilitad.Text = parametros[1].Valor.ToString();
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
            PromedioRentabilitad.Text = String.Empty;
            TasaAjuste.Text = String.Empty;

            Inflacion.CssClass = Inflacion.CssClass.Replace(" formTextboxError", String.Empty);
            PromedioRentabilitad.CssClass = PromedioRentabilitad.CssClass.Replace(" formTextboxError", String.Empty);
            TasaAjuste.CssClass = TasaAjuste.CssClass.Replace(" formTextboxError", String.Empty);
        }

        private void CargarInformacionInicialPantalla()
        {
            IdSimulador.Value = ((int)Enums.OpcionesSistema.SimuladorRentaVitaliciaRetiroProgramado).ToString();

            if (Session["Consentimiento"] != null)
                FormularioBusqueda.Visible = (bool)Session["Consentimiento"];

            //servicioCotizador = LocalizadorProxy.ObtenerServicio();
            //List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

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
        public static Respuesta SimularRentaVitaliciaRetiroProgramado(string tokenUsuario, string idSolicitud, DateTime fechaCotizacion, DateTime fechaDevengue,
                                                                      List<Cotizacion> cotizaciones, List<GrupoFamiliar> beneficiarios, double cic,
                                                                      Int64 correlativo, double rentabilidadAFP, double ipc, double ajuste)
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
                        //var control = (SimuladorRentaVitaliciaRetiroProgramado)pagina.LoadControl("~/Controles/SimuladorRentaVitaliciaRetiroProgramado.ascx");
                        SimuladorRentaVitaliciaRetiroProgramado control = (SimuladorRentaVitaliciaRetiroProgramado)pagina.LoadControl("~/Controles/SimuladorRentaVitaliciaRetiroProgramado.ascx");
                        //<SRI.FIN-20322>

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorRentaVitaliciaRetiroProgramado))
                        {
                            control.PermisoEjecutar = true;

                            double pensionAnhoCIA = 0;
                            double pensionAnhoAFP = 0;
                            double acumuladoCIA = 0;
                            double acumuladoAFP = 0;
                            double saldo = cic;
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

                            //<SRI.INI-20322>
                            Cotizacion cotizacionCIA = cotizaciones.Find(c => c.Correlativo == correlativo);
                            Cotizacion cotizacionAFP = new Cotizacion();
                            //<SRI.FIN-20322>

                            //Imprimir Log DEGUG
                            //StreamWriter tw = new StreamWriter("C:\\temp\\RP.txt");

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            //ParametroCotizadorWS parametrosCIA = servicioCotizador.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, correlativo, 0)[0];
                            ParametroCotizadorWS parametrosAFP = servicioCotizador.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, correlativo, 0)[0];

                            //<SRI.INI-20322>
                            if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue())
                            {
                                //<SRI.FIN-20322>

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

                                parametrosAFP.cot_num_mdif = 0;
                                parametrosAFP.cot_num_mgar = 0;

                                //<SRI.INI-20322>
                                //Cotizacion cotizacionCIA = cotizaciones.Find(c => c.Correlativo == correlativo);
                                //Cotizacion cotizacionAFP = servicioCotizador.CotizarConParametros(parametrosAFP);
                                cotizacionAFP = servicioCotizador.CotizarConParametros(parametrosAFP);
                                //<SRI.FIN-20322>


                                if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()) tasaAjuste = ipc;
                                else if (cotizacionCIA.Moneda.Id == Enums.Moneda.Dolares.StringValue()) tasaAjuste = 1;
                                else if (cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue() || cotizacionCIA.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue()) tasaAjuste = ajuste;

                                control.GraficoCIA = new List<GraficoLineal>();
                                control.GraficoAFP = new List<GraficoLineal>();

                                // Presente (Año 0)
                                control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = Math.Round(cotizacionCIA.PensionCia, 2) });
                                control.GraficoAFP.Add(new GraficoLineal { Nombre = "RETIRO PROGRAMADO", Valor = Math.Round(cotizacionAFP.PensionCia, 2) });

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

                                pensionAnhoCIA = cotizacionCIA.PensionCia * 12;
                                acumuladoCIA += pensionAnhoCIA;


                                //<SRI.INI-20322>
                            }
                            else if (cotizacionCIA.Modalidad.Id == Enums.Modalidad.Diferida.StringValue())
                            {

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


                                if (cotizacionCIA.Moneda.Id == Enums.Moneda.Soles.StringValue()) tasaAjuste = ipc;
                                else if (cotizacionCIA.Moneda.Id == Enums.Moneda.Dolares.StringValue()) tasaAjuste = 1;
                                else if (cotizacionCIA.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue() || cotizacionCIA.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue()) tasaAjuste = ajuste;

                                control.GraficoCIA = new List<GraficoLineal>();
                                control.GraficoAFP = new List<GraficoLineal>();

                                // Redondear montos
                                cotizacionCIA.PensionCia = Math.Round(cotizacionCIA.PensionCia, 2);
                                cotizacionCIA.PensionAFP = Math.Round(cotizacionCIA.PensionAFP, 2);
                                cotizacionAFP.PensionCia = Math.Round(cotizacionAFP.PensionCia, 2);

                                // Presente (Año 0)
                                control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = cotizacionCIA.PensionAFP });
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

                                pensionAnhoCIA = cotizacionCIA.PensionAFP * 12;
                                acumuladoCIA += pensionAnhoCIA;


                                aniosDiferido = (int)cotizacionCIA.PeriodoDiferido;

                                if (aniosDiferido > 1)
                                {

                                    for (int i = 2; i <= aniosDiferido; i++)
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

                                        //cotizacionCIA.PensionAFP = cotizacionCIA.PensionAFP * (1 + tasaAjuste / 100);
                                        //cotizacionCIA.PensionCia = Math.Round(cotizacionCIA.PensionCia * (1 + tasaAjuste / 100), 2);
                                        cotizacionCIA.PensionCia = cotizacionCIA.PensionCia * (1 + tasaAjuste / 100);
                                        if (diferencia < 0)
                                            cotizacionAFP = servicioCotizador.CotizarConParametros(parametrosAFP);

                                        control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = Math.Round(cotizacionCIA.PensionAFP, 2) });
                                        control.GraficoAFP.Add(new GraficoLineal { Nombre = "RETIRO PROGRAMADO", Valor = Math.Round(cotizacionAFP.PensionCia, 2) });

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

                                        pensionAnhoCIA = cotizacionCIA.PensionAFP * 12;
                                        acumuladoCIA += pensionAnhoCIA;

                                        if (i == 4)
                                        {
                                            control.RV05Pension = cotizacionCIA.PensionAFP;
                                            control.RV05Acumulado = acumuladoCIA;
                                            control.RP05Pension = cotizacionAFP.PensionCia;
                                            control.RP05Acumulado = acumuladoAFP;
                                        }
                                        if (i == 9)
                                        {
                                            control.RV10Pension = cotizacionCIA.PensionAFP;
                                            control.RV10Acumulado = acumuladoCIA;
                                            control.RP10Pension = cotizacionAFP.PensionCia;
                                            control.RP10Acumulado = acumuladoAFP;
                                        }
                                        if (i == 14)
                                        {
                                            control.RV15Pension = cotizacionCIA.PensionAFP;
                                            control.RV15Acumulado = acumuladoCIA;
                                            control.RP15Pension = cotizacionAFP.PensionCia;
                                            control.RP15Acumulado = acumuladoAFP;
                                        }
                                        if (i == 19)
                                        {
                                            control.RV20Pension = cotizacionCIA.PensionAFP;
                                            control.RV20Acumulado = acumuladoCIA;
                                            control.RP20Pension = cotizacionAFP.PensionCia;
                                            control.RP20Acumulado = acumuladoAFP;
                                        }

                                    }

                                }

                            }
                            //<SRI.FIN-20322>


                            //tw.WriteLine(String.Format("Tasa AFP = {0}", parametrosAFP.cot_tas_tasa));
                            //tw.WriteLine(String.Format("Año\tCIC\tPensión\tPensión x año\tSaldo\tRentabilidad\tAcumulado", 1, saldo + pensionAnhoAFP, cotizacionAFP.PensionCia, pensionAnhoAFP, saldo, rentabilidad, acumuladoAFP));
                            //tw.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", 1, saldo + pensionAnhoAFP, cotizacionAFP.PensionCia, pensionAnhoAFP, saldo, rentabilidad, acumuladoAFP));

                            // Resto de años

                            if (aniosDiferido == 0)
                            {
                                aniosDiferido = 1;
                            }

                            for (int i = aniosDiferido; i < 25; i++)
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

                                //cotizacionCIA.PensionCia = Math.Round(cotizacionCIA.PensionCia * (1 + tasaAjuste / 100), 2);
                                cotizacionCIA.PensionCia = cotizacionCIA.PensionCia * (1 + tasaAjuste / 100);
                                if (diferencia < 0)
                                    cotizacionAFP = servicioCotizador.CotizarConParametros(parametrosAFP);

                                control.GraficoCIA.Add(new GraficoLineal { Nombre = "RENTA VITALICIA", Valor = Math.Round(cotizacionCIA.PensionCia, 2) });
                                control.GraficoAFP.Add(new GraficoLineal { Nombre = "RETIRO PROGRAMADO", Valor = Math.Round(cotizacionAFP.PensionCia, 2) });

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

                                pensionAnhoCIA = cotizacionCIA.PensionCia * 12;
                                acumuladoCIA += pensionAnhoCIA;

                                if (i == 4)
                                {
                                    control.RV05Pension = cotizacionCIA.PensionCia;
                                    control.RV05Acumulado = acumuladoCIA;
                                    control.RP05Pension = cotizacionAFP.PensionCia;
                                    control.RP05Acumulado = acumuladoAFP;
                                }
                                if (i == 9)
                                {
                                    control.RV10Pension = cotizacionCIA.PensionCia;
                                    control.RV10Acumulado = acumuladoCIA;
                                    control.RP10Pension = cotizacionAFP.PensionCia;
                                    control.RP10Acumulado = acumuladoAFP;
                                }
                                if (i == 14)
                                {
                                    control.RV15Pension = cotizacionCIA.PensionCia;
                                    control.RV15Acumulado = acumuladoCIA;
                                    control.RP15Pension = cotizacionAFP.PensionCia;
                                    control.RP15Acumulado = acumuladoAFP;
                                }
                                if (i == 19)
                                {
                                    control.RV20Pension = cotizacionCIA.PensionCia;
                                    control.RV20Acumulado = acumuladoCIA;
                                    control.RP20Pension = cotizacionAFP.PensionCia;
                                    control.RP20Acumulado = acumuladoAFP;
                                }

                                //tw.WriteLine(String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", (i + 1), saldo + pensionAnhoAFP, cotizacionAFP.PensionCia, pensionAnhoAFP, saldo, rentabilidad, acumuladoAFP));
                            }
                            respuesta.Grafico1 = control.GraficoCIA;
                            respuesta.Grafico2 = control.GraficoAFP;

                            //tw.Close();
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

                                SimuladorRentaVitaliciaRetiroProgramado CntSimulador = HttpContext.Current.Session["CntSimulador"] as SimuladorRentaVitaliciaRetiroProgramado;

                                string rviPensionMesCinco = String.Format("{0:###,###,###,##0.00}", CntSimulador.RV05Pension).ToString();
                                string rviPensionAcumCinco = String.Format("{0:###,###,###,##0.00}", CntSimulador.RV05Acumulado).ToString();
                                string rviPensionMesDiez = String.Format("{0:###,###,###,##0.00}", CntSimulador.RV10Pension).ToString();
                                string rviPensionAcumDiez = String.Format("{0:###,###,###,##0.00}", CntSimulador.RV10Acumulado).ToString();
                                string rviPensionMesQuince = String.Format("{0:###,###,###,##0.00}", CntSimulador.RV15Pension).ToString();
                                string rviPensionAcumQuince = String.Format("{0:###,###,###,##0.00}", CntSimulador.RV15Acumulado).ToString();
                                string rviPensionMesVeinte = String.Format("{0:###,###,###,##0.00}", CntSimulador.RV20Pension).ToString();
                                string rviPensionAcumVeinte = String.Format("{0:###,###,###,##0.00}", CntSimulador.RV20Acumulado).ToString();
                                string proPensionMesCinco = String.Format("{0:###,###,###,##0.00}", CntSimulador.RP05Pension).ToString();
                                string proPensionAcumCinco = String.Format("{0:###,###,###,##0.00}", CntSimulador.RP05Acumulado).ToString();
                                string proPensionMesDiez = String.Format("{0:###,###,###,##0.00}", CntSimulador.RP10Pension).ToString();
                                string proPensionAcumDiez = String.Format("{0:###,###,###,##0.00}", CntSimulador.RP10Acumulado).ToString();
                                string proPensionMesQuince = String.Format("{0:###,###,###,##0.00}", CntSimulador.RP15Pension).ToString();
                                string proPensionAcumQuince = String.Format("{0:###,###,###,##0.00}", CntSimulador.RP15Acumulado).ToString();
                                string proPensionMesVeinte = String.Format("{0:###,###,###,##0.00}", CntSimulador.RP20Pension).ToString();
                                string proPensionAcumVeinte = String.Format("{0:###,###,###,##0.00}", CntSimulador.RP20Acumulado).ToString();
                                string rutaImagen = rutaImagenSimulada.Substring(22, rutaImagenSimulada.Length - 22);

                                ReportViewer visorReporte = new ReportViewer();
                                visorReporte.ProcessingMode = ProcessingMode.Remote;
                                visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteSimulacionRentaVitaliciaRetiroProgramado"];

                                ReportParameter p1 = new ReportParameter("wl_rvi_pension_mes_cinco", rviPensionMesCinco);
                                ReportParameter p2 = new ReportParameter("wl_rvi_pension_acum_cinco", rviPensionAcumCinco);
                                ReportParameter p3 = new ReportParameter("wl_rvi_pension_mes_diez", rviPensionMesDiez);
                                ReportParameter p4 = new ReportParameter("wl_rvi_pension_acum_diez", rviPensionAcumDiez);
                                ReportParameter p5 = new ReportParameter("wl_rvi_pension_mes_quince", rviPensionMesQuince);
                                ReportParameter p6 = new ReportParameter("wl_rvi_pension_acum_quince", rviPensionAcumQuince);
                                ReportParameter p7 = new ReportParameter("wl_rvi_pension_mes_veinte", rviPensionMesVeinte);
                                ReportParameter p8 = new ReportParameter("wl_rvi_pension_acum_veinte", rviPensionAcumVeinte);
                                ReportParameter p9 = new ReportParameter("wl_pro_pension_mes_cinco", proPensionMesCinco);
                                ReportParameter p10 = new ReportParameter("wl_pro_pension_acum_cinco", proPensionAcumCinco);
                                ReportParameter p11 = new ReportParameter("wl_pro_pension_mes_diez", proPensionMesDiez);
                                ReportParameter p12 = new ReportParameter("wl_pro_pension_acum_diez", proPensionAcumDiez);
                                ReportParameter p13 = new ReportParameter("wl_pro_pension_mes_quince", proPensionMesQuince);
                                ReportParameter p14 = new ReportParameter("wl_pro_pension_acum_quince", proPensionAcumQuince);
                                ReportParameter p15 = new ReportParameter("wl_pro_pension_mes_veinte", proPensionMesVeinte);
                                ReportParameter p16 = new ReportParameter("wl_pro_pension_acum_veinte", proPensionAcumVeinte);
                                ReportParameter p17 = new ReportParameter("wl_ruta_imagen", rutaImagen);

                                log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteSimulacionRentaVitaliciaRetiroProgramado"]));
                                log.Debug(String.Format("Parámetros del reporte: wl_rvi_pension_mes_cinco[{0}] wl_rvi_pension_acum_cinco[{1}] wl_rvi_pension_mes_diez[{2}] wl_rvi_pension_acum_diez[{3}] wl_rvi_pension_mes_quince[{4}] wl_rvi_pension_acum_quince[{5}] wl_rvi_pension_mes_veinte[{6}] wl_rvi_pension_acum_veinte[{7}] wl_pro_pension_mes_cinco[{8}] wl_pro_pension_acum_cinco[{9}] wl_pro_pension_mes_diez[{10}] wl_pro_pension_acum_diez[{11}] wl_pro_pension_mes_quince[{12}] wl_pro_pension_acum_quince[{13}] wl_pro_pension_mes_veinte[{14}] wl_pro_pension_acum_veinte[{15}] wl_ruta_imagen[{16}].",
                                    rviPensionMesCinco, rviPensionAcumCinco, rviPensionMesDiez, rviPensionAcumDiez, rviPensionMesQuince, rviPensionAcumQuince, rviPensionMesVeinte, rviPensionAcumVeinte, proPensionMesCinco, proPensionAcumCinco, proPensionMesDiez, proPensionAcumDiez, proPensionMesQuince, proPensionAcumQuince, proPensionMesVeinte, proPensionAcumVeinte, rutaImagen));
                                visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6, p7, p8, p9, p10, p11, p12, p13, p14, p15, p16, p17 });
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