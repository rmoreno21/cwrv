using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using iTextSharp.text.pdf;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading.Tasks;
using System.Net;
using System.Text;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
{
    public partial class CerrarSolicitud : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CerrarSolicitud));
        private static IServicioCWRV servicioCotizador;
        

        private bool flagPEP = false;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.MenuCotizador))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));


                            CargarInformacionInicialPantalla();
                            log.Info("CargarInformacionInicialPantalla");
                            //LimpiarFormularios();

                            if (Session["ModSolModo"] == null)
                                Session["ModSolModo"] = "";

                            if (Session["ModSolModo"].ToString() != null)
                            {

                                LabModSolAviso.Visible = false;

                                log.Info("ini solicitud");
                                // Obtener la información desde base de datos
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(Session["idSolicitud"].ToString());
                                log.Info("fin solicitud");

                                log.Info("ComboIFPMoneda");
                                List<List<Parametro>> listaParametro = servicioCotizador.ObtenerComboboxIFP();
                                Session["ComboIFPMoneda"] = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.Moneda];

                                log.Info("ComboTramoEscalonada");
                                Session["ComboTramoEscalonada"] = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.PorcentajeEscalon];

                                log.Info("solicitud.Afiliado.CUSPP.ToString()");
                                Session["CUSPP_RP"] = solicitud.Afiliado.CUSPP.ToString();
                                log.Info("solicitud.Afiliado.Session.ToString()");
                                //Session["CUSPP"] = Session["CUSPP_RP"];
                                log.Info("solicitud.Afiliado.AFP.ToString()");
                                Session["AFP_RP"] = solicitud.Afiliado.AFP.Id.ToString();

                                log.Info("HCUSPP_RP");
                                HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();
                                HAFP_RP.Value = Session["AFP_RP"].ToString();
                                Session["Consentimiento"] = true;

                               

                                

                                ModSolNroSolicitud_IFP.Text = solicitud.Id;
                                //ModSolTemporalidad_RP.SelectedIndex = ModSolTemporalidad_RP.Items.IndexOf(ModSolTemporalidad_RP.Items.FindByValue(solicitud.Temporalidad.Id));
                                ModSolMonedaPrimaUnica_IFP.SelectedIndex = ModSolMonedaPrimaUnica_IFP.Items.IndexOf(ModSolMonedaPrimaUnica_IFP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                //ModSolTipoPlan_RP.SelectedIndex = ModSolTipoPlan_RP.Items.IndexOf(ModSolTipoPlan_RP.Items.FindByValue(solicitud.TipoPlan.Id));
                                ModSolPrimaUnica_IFP.Text = solicitud.PrimaUnica.ToString("#,##0.00");

                                //ModSolTemporalidadText_IFP.Text = ModSolTemporalidad_RP.SelectedItem.Text;
                                ModSolMonedaPrimaUnicaText_IFP.Text = ModSolMonedaPrimaUnica_IFP.SelectedItem.Text;
                                //ModSolTipoPlanText_IFP.Text = ModSolTipoPlan_RP.SelectedItem.Text;

                                ModSolFechaCotizacion_IFP.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                ModSolFechaDevengue_IFP.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                ModSolFechaVigencia_IFP.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");

                                HEstado.Value = solicitud.CodigoEstado.ToString();
                                Session["HEstadoEsPoliza"] = solicitud.CodigoEstado.ToString();

                                //ModSolDCOM_IFP.Text = solicitud.PorcentajeDescuentoComision.ToString();
                                Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                Session["RP_Cotizaciones"] = solicitud.Cotizaciones;

                                var cotizacionSeleccionada = solicitud.Cotizaciones.FindAll(p => p.EstadoCotizacion == "04" && p.IndSeleccionada == "S");
                                if (cotizacionSeleccionada.Count > 0)
                                {
                                    HSeleccionada.Value = "S";
                                    LabModSolAviso.Visible = true;
                                }

                                ModSolTipoCambio_IFP.Text = solicitud.TipoCambio.ToString();
                                ModSolTipoCambioPanel.Visible = false;
                                if (solicitud.MonedaPrimaUnica.Id.ToString().Equals("002"))
                                {
                                    ModSolTipoCambioPanel.Visible = true;
                                }

                                ModSolLineaNumPoliza_IFP.Visible = false;
                                if (solicitud.NumeroPoliza != 0)
                                {
                                    ModSolLineaNumPoliza_IFP.Visible = true;
                                    ModSolNumPoliza_IFP.Text = solicitud.NumeroPoliza.ToString();
                                }

                                //Bloqueando controles
                                //ModSolTemporalidad_RP.Enabled = false;
                                ModSolMonedaPrimaUnica_IFP.Enabled = false;
                                //ModSolTipoPlan_RP.Enabled = false;

                                ModSolFechaCotizacion_IFP.Enabled = false;
                                ModSolFechaDevengue_IFP.Enabled = false;
                                ModSolFechaVigencia_IFP.Enabled = false;

                                LabMensaje.Text = solicitud.EstadoSolicitud.ToString(); //"Solicitud Seleccionada";
                                ModSolEstadoPoliza_IFP.Text = solicitud.EstadoPoliza;

                                Session["DNIIFP"] = solicitud.Afiliado.NumeroIdentificacion;
                                Session["NombresIFP"] = solicitud.Afiliado.Nombre.Trim();
                                Session["ApellidosIFP"] = solicitud.Afiliado.ApellidoPaterno.Trim() + " " + solicitud.Afiliado.ApellidoMaterno.Trim();

                                ModSolDNI_IFP.Text = Session["DNIIFP"].ToString();
                                ModSolNombres_IFP.Text = Session["NombresIFP"].ToString();
                                ModSolApellidos_IFP.Text = Session["ApellidosIFP"].ToString();

                                switch (solicitud.CodigoEstado.ToString())
                                {
                                    case "1":
                                        //LabMensaje.Text = "Solicitud Seleccionada";
                                        LabModSolAviso.CssClass = "grilla_info_verde";
                                        LabMensaje.ForeColor = System.Drawing.Color.Green;
                                        LabModSolAviso.Visible = true;
                                        //ModSolReporte_RP.Visible = true;
                                        break;
                                    case "2":
                                        //LabMensaje.Text = "Solicitud Cerrada";
                                        LabModSolAviso.CssClass = "grilla_info";
                                        LabModSolAviso.Visible = true;
                                        //ModSolReporte_RP.Visible = true;
                                        //if (solicitud.TipoCotizacion.Id.ToString() != "IFP")
                                        //{
                                        //    ModSolReportePoliza_RP.Visible = true;
                                        //}
                                        break;
                                    case "3":
                                        //LabMensaje.Text = "Solicitud Anulada";
                                        LabModSolAviso.CssClass = "grilla_info_rojo";
                                        LabMensaje.ForeColor = System.Drawing.Color.Red;
                                        LabModSolAviso.Visible = true;
                                        break;
                                    case "6":
                                        //LabMensaje.Text = "Solicitud Cerrada";
                                        //LabModSolAviso.CssClass = "grilla_info";
                                        //LabModSolAviso.Visible = true;
                                        //ModSolReporte_RP.Visible = true;
                                        //if (solicitud.TipoCotizacion.Id.ToString() != "IFP")
                                        //{
                                        //    ModSolReportePoliza_RP.Visible = true;
                                        //}
                                        break;
                                    default:
                                        LabModSolAviso.CssClass = "grilla_info";
                                        break;
                                }

                                LabModLineaCausal.Visible = false;
                                if (solicitud.CausalPoliza.Id != null)
                                {
                                    LabModLineaCausal.Visible = true;
                                    ModSolCausalPoliza_RP.Text = solicitud.CausalPoliza.NombreLargo;
                                }

                                HCodCanalDistribucion.Value = solicitud.CodCanalDistribucion;

                                ModSolModo.Value = Convert.ToString((Session["ModSolModo"]));
                                HCopia.Value = "";

                                if (Convert.ToString((Session["ModSolModo"])) == "C")
                                    HCopia.Value = "C";


                                ModSolPrimaUnica_IFP.Visible = true;
                                ManSolTipoSolicitud_RP.Value = "EXTRAOFICIAL";

                                HBloqueo.Value = "TRUE";
                                if (((string)Session["RolAzman"]) == "JEF.RVI.OPE")
                                {
                                    HBloqueo.Value = "FALSE";
                                }

                                //Validando la fecha de cotizacion
                                ModSolFechaCotizacion_IFP.Enabled = false;
                            }
                            else
                            {
                                Response.Redirect("ListadoCierrePlus.aspx");
                            }
                        }
                        else
                        {
                            if (ModSolPrimaUnica_IFP.Text != String.Empty) ModSolPrimaUnica_IFP.Text = Convert.ToDouble(ModSolPrimaUnica_IFP.Text, new CultureInfo("es-PE")).ToString();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.MenuCotizador.StringValue()));
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

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            log.Info("CargarCombobox");
            //CargarCombobox(ModSolTemporalidad_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Temporalidad]);
            CargarCombobox(ModSolMonedaPrimaUnica_IFP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.MonedaRentaPrivada]);


            //CargarCombobox(ModSolTipoPlan_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.TipoPlan]);

            log.Info("ComboMoneda");
            Session["ComboMoneda"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda];


            //Session["ComboPorcentajeEscalonada"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeEscalonado]; ;
            Session["ComboPorcentajeEscalonada"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.NuevoPorcentajeEscalonado];

            List<Parametro> lstPjeDevolucion = new List<Parametro>();
            lstPjeDevolucion = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeDevolucion];
            var PjeDevolucion = lstPjeDevolucion.OrderBy(a => Int32.Parse(a.Id));

            log.Info("ComboPorcentajeDevolucion");
            Session["ComboPorcentajeDevolucion"] = PjeDevolucion.ToList();

            List<Parametro> comboPeriodoGarantizado = new List<Parametro>();
            comboPeriodoGarantizado.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "5", Glosa = "5" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "7", Glosa = "7" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "10", Glosa = "10" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "15", Glosa = "15" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "20", Glosa = "20" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "25", Glosa = "25" });
            Session["ComboPeriodoGarantizado"] = comboPeriodoGarantizado;

            log.Info("ComboPorcentajeConyuge");
            Session["ComboPorcentajeConyuge"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeConyuge];

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            //LabModSolLineaACOMDCOM_RP.Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);

            //if (!LabModSolLineaACOMDCOM_RP.Visible)
            //{
            //    LabModSolFechaVigencia_RP.CssClass = "formLabel formLabel2Izq";
            //}

            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;

            log.Info("ComboMonedaAjustePlus");
            List<Parametro> lstParametro = new List<Parametro>();
            Session["ComboMonedaAjustePlus"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.MonedaAjustePlus];

            List<Parametro> lstDiasVigencia = new List<Parametro>();
            lstDiasVigencia = servicioCotizador.ObtenerParametrosPorTabla("PLUS");
            if (lstDiasVigencia.Count() > 0)
            {
                HDiasVigencia.Value = lstDiasVigencia[0].Valor_1;
            }

            log.Info("lstCausalPoliza");
            List<CausalPoliza> lstCausalPoliza = new List<CausalPoliza>();
            lstCausalPoliza = servicioCotizador.ListarCausalPolizaPlus();
            ModSolCausante_RP.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (var item in lstCausalPoliza)
            {
                ModSolCausante_RP.Items.Add(new ListItem(item.NombreLargo, item.Id));
            }

        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            if (ModSolMonedaPrimaUnica_IFP.ClientID != "ModSolMonedaPrimaUnica_RP")
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

        private void CargarCombobox(DropDownList control, List<MontoCIC> combobox)
        {
            control.Items.Clear();
            foreach (MontoCIC item in combobox)
            {
                control.Items.Add(new ListItem(String.Format("{0:#,##0.00}", item.Valor), item.Valor.ToString()));
            }
        }

        [WebMethod]
        public static string CargarTablaCotizaciones(List<CotizacionRPPlus> cotizaciones, string temporalidad, string moneda, string conyuge)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    List<CotizacionRPPlus> cotizacionesNew = new List<CotizacionRPPlus>();
                    var pagina = new Page();
                    var control = (TablaCotizacionesRentaPrivadaPlusCierre)pagina.LoadControl("~/Controles/TablaCotizacionesRentaPrivadaPlusCierre.ascx");

                    int iTemporalidad = 9999;
                    switch (temporalidad)
                    {
                        case "T05":
                            iTemporalidad = 5;
                            break;
                        case "T07":
                            iTemporalidad = 7;
                            break;
                        case "T10":
                            iTemporalidad = 10;
                            break;
                        case "T15":
                            iTemporalidad = 15;
                            break;
                        case "T20":
                            iTemporalidad = 20;
                            break;
                        case "T25":
                            iTemporalidad = 25;
                            break;
                        case "TVT":
                            iTemporalidad = 9999;
                            break;
                    }

                    foreach (CotizacionRPPlus cotizacion in cotizaciones)
                    {
                        if (cotizacion.PeriodoGarantizado > iTemporalidad)
                        {
                            cotizacion.PeriodoGarantizado = iTemporalidad;
                        }

                        if (conyuge == "FALSE")
                        {
                            cotizacion.ValPjeConyuge = 0;
                        }

                        if (HttpContext.Current.Session["ModSolModo"].ToString() == "C")
                        {
                            cotizacion.Correlativo = 0;
                            //cotizacion.PagoEscalonada = 0;
                            cotizacion.Pension2doTramo = 0;
                            cotizacion.Pension2doTramoSinAjuste = 0;
                            cotizacion.PensionCia = 0;
                            cotizacion.PensionCiaMO = 0;
                            cotizacion.TasaVenta = 0;
                            cotizacion.TasaVentaSbs = 0;
                            cotizacion.TasaRetornoAccionista = 0;
                            cotizacion.IndCotiza = "";
                        }
                        cotizacionesNew.Add(cotizacion);
                    }

                    control.CotizacionesRPPlus = cotizacionesNew;
                    control.Moneda = moneda;
                    control.Temporalidad = iTemporalidad;
                    control.Conyuge = (conyuge == "TRUE") ? true : false;

                    // Validando si el acceso es desde dentro de la red de Interseguro o desde Internet
                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                    {
                        control.PermisoTRA = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoTRAPlus)) ? true : false;
                        control.PermisoEspeciales = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoEspeciales)) ? true : false;
                    }
                    else
                    {
                        control.PermisoTRA = false;
                        control.PermisoEspeciales = false;
                    }

                    control.PermisoAgregar = true;
                    control.PermisoModificar = true;
                    control.PermisoEliminar = true;

                    if (HttpContext.Current.Session["ModSolModo"].ToString() == "CONS")
                    {
                        control.PermisoAgregar = false;
                        control.PermisoModificar = false;
                        control.PermisoEliminar = false;
                    }

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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Respuesta GenerarPoliza(string tokenUsuario, string num_solicitud, string num_correlativo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusGenerarPoliza))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            GrupoFamiliar grup_fam = new GrupoFamiliar();

                            if (ValidarCotizacion(num_solicitud, num_correlativo, errores, controles))
                            {
                                string usuario = (string)HttpContext.Current.Session["Usuario"];
                                
                                List<DatosSol> lstDatossolicitud = new List<DatosSol>();

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                lstDatossolicitud = servicioCotizador.ObtenerDatosporSolicitudIFP(num_solicitud);

                                DatosSol datossolicitud = lstDatossolicitud.ToList().FirstOrDefault();

                                log.Debug("Datos de la solicitud");
                                log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(datossolicitud));

                                //inteligo
                                if (datossolicitud.OrigenCotizacion == Enums.OrigenCotizacion.Inteligo.StringValue())
                                {
                                    string gls_estado_civil = "";

                                    switch (datossolicitud.EstadoCivil)
                                    {
                                        case "538560001":
                                            gls_estado_civil = "soltero";
                                            break;
                                        case "538560002":
                                            gls_estado_civil = "casado";
                                            break;
                                        case "538560003":
                                            gls_estado_civil = "viudo";
                                            break;
                                        case "538560004":
                                            gls_estado_civil = "divorciado";
                                            break;
                                        default:
                                            gls_estado_civil = "otros";
                                            break;
                                    }

                                    grup_fam.estadoCivil = datossolicitud.cod_EstadoCivil_afiliado.ToString();
                                    grup_fam.Telefono1 = datossolicitud.telefono.ToString();
                                    grup_fam.Telefono2 = datossolicitud.celular.ToString();
                                }
                                else
                                {
                                    grup_fam.estadoCivil = datossolicitud.cod_EstadoCivil_afiliado.ToString();
                                    grup_fam.Telefono1 = datossolicitud.telefono.ToString();
                                    grup_fam.Telefono2 = datossolicitud.celular.ToString();
                                }

                                //HLS - Obtener la dirección del afiliado para la póliza
                                string CUSPP = HttpContext.Current.Session["CUSPP_RP"].ToString();

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                Direccion direccion = servicioCotizador.ListarDireccion(CUSPP).Find(d => d.Principal);

                                grup_fam.Direccion = string.Format("{0} {1} {2}", direccion.TipoVia.Glosa, direccion.Glosa, direccion.EspacioUrbano);
                                grup_fam.Departamento = direccion.Departamento.Nombre.ToUpper();
                                grup_fam.Provincia = direccion.Ciudad.Nombre.ToUpper();
                                grup_fam.Distrito = direccion.Comuna.Nombre.ToUpper();

                                respuesta = servicioCotizador.GenerarPolizaPlus(num_solicitud, Convert.ToInt32(num_correlativo), usuario, grup_fam);

                                if (respuesta.Estado == Constante.COD_OK)
                                {
                                    if (num_solicitud.Substring(0, 3) == "IFP")
                                    {
                                        Respuesta respuestaPE = new Respuesta();
                                        respuestaPE = servicioCotizador.GenerarPolizaElectronicaPDF(num_solicitud, 0, "", grup_fam, Enums.TipoCotizacion.RentaPrivadaIFP.StringValue(), usuario);

                                        if (respuestaPE.Estado != Constante.COD_OK)
                                        {
                                            log.Error("Se emitió la póliza " + respuesta.Mensaje + ", pero falló el envío de la Póliza Electrónica, por favor envíela desde el ADMWR");
                                            respuesta.Estado = Constante.COD_ERROR;
                                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>Se emitió la póliza " + respuesta.Mensaje + ", pero falló el envío de la Póliza Electrónica, por favor envíela desde el ADMWR.</strong></div>";
                                        }
                                        else
                                        {
                                            try
                                            {
                                                log.Info("Se busca al agente en el arbol de agentes de la sesión");
                                                List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                                                Agente agente = listaAgentes.Find(a => a.Id == datossolicitud.num_agente);

                                                log.Debug("Datos del agente desde la sesión");
                                                log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(agente));

                                                if (agente != null)
                                                {
                                                    log.Info("Se consulta la información del agente a AZMAN");
                                                    AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient servicioAzman = new AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient("epAzman");
                                                    var datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                                            ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                                            ConfigurationManager.AppSettings["DominioRed"],
                                                            agente.Usuario);

                                                    log.Debug("Datos del agente desde AZMAN");
                                                    log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(datosUsuario));

                                                    if (datosUsuario != null)
                                                    {
                                                        if (!string.IsNullOrEmpty(datosUsuario.Correo))
                                                        {
                                                            NotificacionSME notificacionSME = new NotificacionSME();
                                                            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                                                            string rutaServicio = ConfigurationManager.AppSettings["url_envio_correo_sme"].ToString();
                                                            string remitente = ConfigurationManager.AppSettings["remitente_agente_poliza"].ToString();
                                                            string asunto = ConfigurationManager.AppSettings["asunto_agente_poliza"].ToString();

                                                            log.Info("ruta servicio: " + rutaServicio);
                                                            log.Info("remitente: " + remitente);
                                                            log.Info("asunto: " + asunto);

                                                            string nom_afiliado = datossolicitud.nom_nombre_afiliado + " " + datossolicitud.ape_paterno_afiliado + " " + datossolicitud.ape_materno_afiliado;
                                                            nom_afiliado = ti.ToTitleCase(nom_afiliado.ToString().Trim().ToLower());

                                                            asunto = string.Format(asunto, Convert.ToInt32(respuesta.Mensaje), nom_afiliado);

                                                            log.Info("Obteniendo el cuerpo del correo");
                                                            var cuerpo = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\Plantilla\\RP\\Agente\\Plantilla-CorreoNotificacionPoliza.html");
                                                            cuerpo = cuerpo.Replace("{agente}", ti.ToTitleCase(datossolicitud.nom_agente.ToString().Trim().ToLower())).Replace("{poliza}", respuesta.Mensaje).Replace("{cliente}", nom_afiliado);

                                                            string correoAgente = datosUsuario.Correo;
                                                            log.Info("Correo agente: " + correoAgente);

                                                            log.Info("Entidad correo SME");
                                                            notificacionSME.De = remitente;
                                                            notificacionSME.DeNombre = "Servicios Interseguro";
                                                            notificacionSME.Para = correoAgente;
                                                            notificacionSME.ResponderA = remitente;
                                                            notificacionSME.ResponderANombre = "Servicios Interseguro";
                                                            notificacionSME.Asunto = asunto;
                                                            notificacionSME.Cuerpo = cuerpo;

                                                            log.Info("Ejecutando el servicio del correo");
                                                            using (var client = new WebClient())
                                                            {
                                                                client.Encoding = Encoding.UTF8;
                                                                var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                                                                string jsonString = JsonSerializar.Serialize(notificacionSME);
                                                                log.Debug("json correo: " + jsonString);
                                                                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                                                var respuestaCorreo = client.UploadString(new Uri(rutaServicio), "POST", jsonString);
                                                                log.Info($"Respuesta de {rutaServicio}: {respuestaCorreo}");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                log.Error(ex.Message, ex);
                                            }

                                            respuesta.Estado = Constante.COD_OK;
                                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                            respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>Se emitió la póliza " + respuesta.Mensaje + " correctamente.</strong></div>";
                                        }
                                    }
                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                    respuesta.Mensaje = "Error en el proceso de generación de la póliza";
                                }
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusActualizar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static Respuesta AnularSolicitudPlus(string tokenUsuario, string num_solicitud, string cod_causante)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusCerrar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            string usuario = (string)HttpContext.Current.Session["Usuario"];

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            respuesta = servicioCotizador.AnularSolicitudPlus(num_solicitud, usuario, cod_causante);
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusActualizar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    return respuesta;
                }
            }
        }

        private static bool ValidarCotizacion(string num_solicitud, string num_cotizacion, List<String> errores, List<String> controles)
        {
            bool esCorrecto = true;

            // solicitud
            bool solicitud = true;
            if (num_solicitud == "")
            {
                errores.Add("Seleccione la <strong>Solicitud</strong>. Dato Obligatorio.");
                solicitud = false;
            }

            // cotizacion
            bool cotizacion = true;
            if (num_cotizacion.Trim().Length == 0)
            {
                errores.Add("Seleccione una <strong>Cotización</strong>. Dato Obligatorio.");
                cotizacion = false;
            }
            else
            {
                if (num_cotizacion == "0")
                {
                    errores.Add("Seleccione una <strong>Cotización</strong>. Dato Obligatorio.");
                    cotizacion = false;
                }
            }

            // cotizacion2
            bool cotizacion2 = true;


            esCorrecto = solicitud & cotizacion & cotizacion2;

            return esCorrecto;
        }
                
        //<FINGTI_7012>

        //<INI.GTI_7012_3>
        protected void ModSolReportePoliza_RP_Click(object sender, EventArgs e)
        {
            //PolizaWordtoPDF();
            //EmitirPolizaElectronica(ModSolNroSolicitud_IFP.Text, 0, "");
            //EmitirPoliza(ModSolNroSolicitud_IFP.Text, 0, "");
        }
        
        //<FIN.GTI_7012_3>
        
    }
}