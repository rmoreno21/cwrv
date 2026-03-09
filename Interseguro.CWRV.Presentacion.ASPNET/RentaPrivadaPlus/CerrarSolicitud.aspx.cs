using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Threading;

using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;

using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using Microsoft.Reporting.WebForms;
using log4net;
using iTextSharp.text.pdf;
using OpenXmlPowerTools;
using System.Threading.Tasks;
using System.Text;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
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
                            //LimpiarFormularios();

                            if (Session["ModSolModo"] == null)
                                Session["ModSolModo"] = "";

                            if (Session["ModSolModo"].ToString() != null)
                            {
                                //HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();
                                //HAFP_RP.Value = Session["AFP_RP"].ToString();

                                LabModSolAviso.Visible = false;

                                // Obtener la información desde base de datos
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(Session["idSolicitud"].ToString());

                                //<SOLINIS20>
                                Session["CUSPP_RP"] = solicitud.Afiliado.CUSPP.ToString();
                                //Session["CUSPP"] = Session["CUSPP_RP"];
                                Session["AFP_RP"] = solicitud.Afiliado.AFP.Id.ToString();

                                HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();
                                HAFP_RP.Value = Session["AFP_RP"].ToString();
                                Session["Consentimiento"] = true;
                                
                                //<SOLFINS20>
                                
                                ModSolNroSolicitud_RP.Text = solicitud.Id;
                                ModSolTemporalidad_RP.SelectedIndex = ModSolTemporalidad_RP.Items.IndexOf(ModSolTemporalidad_RP.Items.FindByValue(solicitud.Temporalidad.Id));
                                ModSolMonedaPrimaUnica_RP.SelectedIndex = ModSolMonedaPrimaUnica_RP.Items.IndexOf(ModSolMonedaPrimaUnica_RP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                ModSolTipoPlan_RP.SelectedIndex = ModSolTipoPlan_RP.Items.IndexOf(ModSolTipoPlan_RP.Items.FindByValue(solicitud.TipoPlan.Id));
                                ModSolPrimaUnica_RP.Text = solicitud.PrimaUnica.ToString("#,##0.00");
                                //<INIGTI_753_3>
                                ModSolTemporalidadText_RP.Text = ModSolTemporalidad_RP.SelectedItem.Text;
                                ModSolMonedaPrimaUnicaText_RP.Text = ModSolMonedaPrimaUnica_RP.SelectedItem.Text;
                                ModSolTipoPlanText_RP.Text = ModSolTipoPlan_RP.SelectedItem.Text;



                                ModSolFechaCotizacion_RP.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                ModSolFechaDevengue_RP.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                ModSolFechaVigencia_RP.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");


                                HEstado.Value = solicitud.CodigoEstado.ToString();
                                //<INIGTI_753_3>

                                ModSolDCOM_RP.Text = solicitud.PorcentajeDescuentoComision.ToString();
                                Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                Session["RP_Cotizaciones"] = solicitud.Cotizaciones;



                                var cotizacionSeleccionada = solicitud.Cotizaciones.FindAll(p => p.EstadoCotizacion == "04" && p.IndSeleccionada == "S");
                                if (cotizacionSeleccionada.Count > 0)
                                {
                                    HSeleccionada.Value = "S";
                                    LabModSolAviso.Visible = true;
                                }
                                //<INIGTI_753_3>}

                                ModSolTipoCambio.Text = solicitud.TipoCambio.ToString();
                                ModSolTipoCambioPanel.Visible = false;
                                if (solicitud.MonedaPrimaUnica.Id.ToString().Equals("002"))
                                {
                                    ModSolTipoCambioPanel.Visible = true;
                                }

                                ModSolLineaNumPoliza_RP.Visible = false;
                                if (solicitud.NumeroPoliza != 0)
                                {
                                    ModSolLineaNumPoliza_RP.Visible = true;
                                    ModSolNumPoliza_RP.Text = solicitud.NumeroPoliza.ToString();
                                }

                                //Bloqueando controles
                                ModSolTemporalidad_RP.Enabled = false;
                                ModSolMonedaPrimaUnica_RP.Enabled = false;
                                ModSolTipoPlan_RP.Enabled = false;

                                ModSolFechaCotizacion_RP.Enabled = false;
                                ModSolFechaDevengue_RP.Enabled = false;
                                ModSolFechaVigencia_RP.Enabled = false;

                                //ModSolPrimaUnica_RP.ReadOnly = true;
                                //ModSolTipoCambio.ReadOnly = true;
                                //ModSolDCOM_RP.ReadOnly = true;



                                //ModSolTemporalidad_RP.CssClass = "formCombobox formComboboxReadOnly";
                                //ModSolMonedaPrimaUnica_RP.CssClass = "formCombobox formComboboxReadOnly";
                                //ModSolTipoPlan_RP.CssClass = "formCombobox formComboboxReadOnly";

                                //ModSolFechaCotizacion_RP.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";
                                //ModSolFechaDevengue_RP.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";
                                //ModSolFechaVigencia_RP.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";

                                //ModSolDCOM_RP.CssClass = "formTextbox formTextboxReadOnly";
                                //ModSolTipoCambio.CssClass = "formTextbox formTextboxReadOnly";
                                //ModSolPrimaUnica_RP.CssClass = "formTextbox formTextboxReadOnly";

                                LabMensaje.Text = solicitud.EstadoSolicitud.ToString(); //"Solicitud Seleccionada";
                                ModSolEstadoPoliza_RP.Text = solicitud.EstadoPoliza;

                                switch (solicitud.CodigoEstado.ToString())
                                {
                                    case "1":
                                        //LabMensaje.Text = "Solicitud Seleccionada";
                                        LabModSolAviso.CssClass = "grilla_info_verde";
                                        LabMensaje.ForeColor = System.Drawing.Color.Green;
                                        LabModSolAviso.Visible = true;

                                        //<INIGTI_7012>
                                        //ModSolReporte_RP.Visible = true;
                                        //<FINGTI_7012>

                                        break;
                                    case "2":
                                        //LabMensaje.Text = "Solicitud Cerrada";
                                        LabModSolAviso.CssClass = "grilla_info";
                                        LabModSolAviso.Visible = true;

                                        //<INIGTI_7012>
                                        //ModSolReporte_RP.Visible = true;
                                        //<FINGTI_7012>
                                        //<INI.GTI_7012_3>
                                        //ModSolReportePoliza_RP.Visible = true;
                                        //<FIN.GTI_7012>

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



                                ModSolModo.Value = Convert.ToString((Session["ModSolModo"]));
                                HCopia.Value = "";

                                if (Convert.ToString((Session["ModSolModo"])) == "C")
                                    HCopia.Value = "C";


                                ModSolPrimaUnica_RP.Visible = true;
                                ManSolTipoSolicitud_RP.Value = "EXTRAOFICIAL";


                                //<INIGTI_753_3>
                                HBloqueo.Value = "TRUE";
                                if (((string)Session["RolAzman"]) == "JEF.RVI.OPE")
                                {
                                    HBloqueo.Value = "FALSE";
                                }
                                //<FINGTI_753_3>

                                //Validando la fecha de cotizacion
                                ModSolFechaCotizacion_RP.Enabled = false;
                            }
                            else
                            {
                                Response.Redirect("ListadoCierrePlus.aspx");
                            }
                        }
                        else
                        {
                            if (ModSolPrimaUnica_RP.Text != String.Empty) ModSolPrimaUnica_RP.Text = Convert.ToDouble(ModSolPrimaUnica_RP.Text, new CultureInfo("es-PE")).ToString();
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

            CargarCombobox(ModSolTemporalidad_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Temporalidad]);
            CargarCombobox(ModSolMonedaPrimaUnica_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.MonedaRentaPrivada]);


            CargarCombobox(ModSolTipoPlan_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.TipoPlan]);


            Session["ComboMoneda"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda];

            //Session["ComboPorcentajeEscalonada"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeEscalonado]; ;
            Session["ComboPorcentajeEscalonada"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.NuevoPorcentajeEscalonado];

            List<Parametro> lstPjeDevolucion = new List<Parametro>();
            lstPjeDevolucion = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeDevolucion];
            //            lstPjeDevolucion = lstPjeDevolucion.Sort(x=> x.Valor,0);
            var PjeDevolucion = lstPjeDevolucion.OrderBy(a => Int32.Parse(a.Id));

            Session["ComboPorcentajeDevolucion"] = PjeDevolucion.ToList(); //(List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeDevolucion];

            List<Parametro> comboPeriodoGarantizado = new List<Parametro>();
            comboPeriodoGarantizado.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "5", Glosa = "5" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "7", Glosa = "7" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "10", Glosa = "10" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "15", Glosa = "15" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "20", Glosa = "20" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "25", Glosa = "25" });
            Session["ComboPeriodoGarantizado"] = comboPeriodoGarantizado;

            ////Session["ComboCapital"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Capital];

            //<INIGTI_753>
            Session["ComboPorcentajeConyuge"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeConyuge];
            //<FINGTI_753>

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            LabModSolLineaACOMDCOM_RP.Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);

            if (!LabModSolLineaACOMDCOM_RP.Visible)
            {
                LabModSolFechaVigencia_RP.CssClass = "formLabel formLabel2Izq";
            }


            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;

            //<INIGTI_753>
            List<Parametro> lstParametro = new List<Parametro>();
            Session["ComboMonedaAjustePlus"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.MonedaAjustePlus];

            //<FINGTI_753>

            //<INIGRI_753_3>
            List<Parametro> lstDiasVigencia = new List<Parametro>();
            lstDiasVigencia = servicioCotizador.ObtenerParametrosPorTabla("PLUS");
            if (lstDiasVigencia.Count() > 0)
            {
                HDiasVigencia.Value = lstDiasVigencia[0].Valor_1;
            }
            //<FINGTI_753_3>
            //<INIGTI_7012>
            List<CausalPoliza> lstCausalPoliza = new List<CausalPoliza>();
            lstCausalPoliza = servicioCotizador.ListarCausalPolizaPlus();
            ModSolCausante_RP.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (var item in lstCausalPoliza)
            {
                ModSolCausante_RP.Items.Add(new ListItem(item.NombreLargo, item.Id));
            }
            //<FINGTI_7012>
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            if (ModSolMonedaPrimaUnica_RP.ClientID != "ModSolMonedaPrimaUnica_RP")
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }
        //private void CargarCombobox(DropDownList control, List<Ciudad> combobox)
        //{
        //    control.Items.Clear();
        //    control.Items.Add(new ListItem("«Seleccione»", "0"));
        //    foreach (Ciudad item in combobox)
        //    {
        //        control.Items.Add(new ListItem(item.Nombre, item.Id));
        //    }
        //}
        //private void CargarCombobox(DropDownList control, List<Comuna> combobox)
        //{
        //    control.Items.Clear();
        //    control.Items.Add(new ListItem("«Seleccione»", "0"));
        //    foreach (Comuna item in combobox)
        //    {
        //        control.Items.Add(new ListItem(item.Nombre, item.Id));
        //    }
        //}

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
                    log.Debug("Inicio CerrarSolicitud.CargarTablaCotizaciones WebMethod");

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
                        //<INIGTI_753>
                        if (conyuge == "FALSE")
                        {
                            cotizacion.ValPjeConyuge = 0;
                        }
                        //<FINGTI_753>

                        //<INIGTI_753_3>
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
                        //<FINGTI_753_3>


                    }


                    control.CotizacionesRPPlus = cotizacionesNew; //cotizaciones;
                    control.Moneda = moneda;
                    control.Temporalidad = iTemporalidad;
                    control.Conyuge = (conyuge == "TRUE") ? true : false;//<INIGTI_753>

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

                    //<INIGTI_753_3>
                    if (HttpContext.Current.Session["ModSolModo"].ToString() == "CONS")
                    {
                        control.PermisoAgregar = false;
                        control.PermisoModificar = false;
                        control.PermisoEliminar = false;
                    }
                    //<FINGTI_753_3>

                    pagina.Controls.Add(control);

                    string html = "";
                    using (var sw = new StringWriter())
                    {
                        HttpContext.Current.Server.Execute(pagina, sw, false);
                        html = sw.ToString();
                    }

                    log.Debug("Fin CerrarSolicitud.CargarTablaCotizaciones WebMethod");

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
                    log.Debug("Inicio CerrarSolicitud.GenerarPoliza WebMethod");

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
                                
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                var lstDatossolicitud = servicioCotizador.ObtenerDatosporSolicitud(num_solicitud);
                                DatosSol datossolicitud = lstDatossolicitud.ToList().FirstOrDefault();

                                log.Debug("Datos de la solicitud");
                                log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(datossolicitud));

                                //<INI.GTI_7012_S16_P27>
                                grup_fam.CorreoElectronico = datossolicitud.glsMail; //cont[0].emailaddress1.ToString();
                                grup_fam.estadoCivil = datossolicitud.cod_EstadoCivil_afiliado; //cont[0].inter_estadocivil.ToString();
                                grup_fam.Telefono1 = datossolicitud.telefono; //cont[0].inter_telefonofijo.ToString();
                                grup_fam.Telefono2 = datossolicitud.celular; //cont[0].mobilephone.ToString();
                                //<INI.GTI_7012_S16_P27>

                                respuesta = servicioCotizador.GenerarPolizaPlus(num_solicitud, Convert.ToInt32(num_correlativo), usuario, grup_fam);

                                if (respuesta.Estado == Constante.COD_OK)
                                {
                                    if (num_solicitud.Substring(0, 3) == "RPP")
                                    {
                                        Respuesta respuestaPE = new Respuesta();
                                        respuestaPE = servicioCotizador.EnviarPolizaElectronicaRPP_PDF(Convert.ToInt32(respuesta.Mensaje), usuario);
                                        if (respuestaPE.Estado != Constante.COD_OK)
                                        {
                                            log.Error("Se emitió la póliza " + respuesta.Mensaje + ", pero falló el envío de la Póliza Electrónica, por favor envíela desde el ADMWR");
                                            respuesta.Estado = Constante.COD_ERROR;
                                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>Se emitió la póliza " + respuesta.Mensaje + ", pero falló el envío de la Póliza Electrónica, por favor envíela desde el ADMWR</strong></div>";
                                        }
                                        else
                                        {
                                            try
                                            {
                                                log.Info("Obteniendo el correo del agente");
                                                List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                                                Agente agente = listaAgentes.Find(a => a.Id == datossolicitud.num_agente);

                                                log.Debug("Datos del agente desde la sesión");
                                                log.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(agente));

                                                if (agente != null)
                                                {
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
                                                            var cuerpo = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\Plantilla\\RP\\Agente\\Plantilla-CorreoNotificacionPoliza.html");
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

                    log.Debug("Fin CerrarSolicitud.GenerarPoliza WebMethod");

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
                    log.Debug("Inicio CerrarSolicitud.AnularSolicitudPlus WebMethod");

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

                    log.Debug("Fin CerrarSolicitud.AnularSolicitudPlus WebMethod");

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
    }
}
