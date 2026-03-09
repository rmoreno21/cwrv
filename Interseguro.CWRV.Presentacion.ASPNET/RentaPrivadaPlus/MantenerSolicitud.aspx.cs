using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
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

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
{
    public partial class MantenerSolicitud : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MantenerSolicitud));
        private static IServicioCWRV servicioCotizador;
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

                            if (Session["ModSolModo"].ToString() != null)
                            {
                                HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();
                                HAFP_RP.Value = Session["AFP_RP"].ToString();

                                LabModSolAviso.Visible = false;

                                //Modificar
                                if (Session["ModSolModo"].ToString() == "M")
                                {
                                    // Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(Session["idSolicitud"].ToString());

                                    ModSolNroSolicitud_RP.Text = solicitud.Id;
                                    ModSolTemporalidad_RP.SelectedIndex = ModSolTemporalidad_RP.Items.IndexOf(ModSolTemporalidad_RP.Items.FindByValue(solicitud.Temporalidad.Id));
                                    ModSolMonedaPrimaUnica_RP.SelectedIndex = ModSolMonedaPrimaUnica_RP.Items.IndexOf(ModSolMonedaPrimaUnica_RP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    ModSolPrimaUnica_RP.Text = solicitud.PrimaUnica.ToString();

                                    //<INIGTI_753_3>

                                    ModSolFechaCotizacion_RP.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_RP.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                    ModSolFechaVigencia_RP.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");


                                    //<INIGTI_753_3>

                                    ModSolDCOM_RP.Text = solicitud.PorcentajeDescuentoComision.ToString();
                                    Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    Session["RP_Cotizaciones"] = solicitud.Cotizaciones;

                                    ModSolTipoPlan_RP.SelectedIndex = ModSolTipoPlan_RP.Items.IndexOf(ModSolTipoPlan_RP.Items.FindByValue(solicitud.TipoPlan.Id));

                                    var cotizacionSeleccionada = solicitud.Cotizaciones.FindAll(p => p.EstadoCotizacion == "04" && p.IndSeleccionada == "S");
                                    if (cotizacionSeleccionada.Count > 0)
                                    {
                                        HSeleccionada.Value = "S";
                                        LabModSolAviso.Visible = true;
                                    }
                                    //<INIGTI_753_3>}

                                    //<INIGTI_7012>
                                    HEstado.Value = solicitud.CodigoEstado.ToString();
                                    LabMensaje.Text = solicitud.EstadoSolicitud.ToString(); //"Solicitud Seleccionada";
                                    ModSolEstadoPoliza_RP.Text = solicitud.EstadoPoliza;

                                    switch (solicitud.CodigoEstado.ToString())
                                    {
                                        case "1":
                                            //LabMensaje.Text = "Solicitud Seleccionada";
                                            LabModSolAviso.CssClass = "grilla_info_verde";
                                            LabMensaje.ForeColor = System.Drawing.Color.Green;
                                            LabModSolAviso.Visible = true;
                                            break;
                                        case "2":
                                            //LabMensaje.Text = "Solicitud Cerrada";
                                            LabModSolAviso.CssClass = "grilla_info";
                                            LabModSolAviso.Visible = true;
                                            break;
                                        case "3":
                                            //LabMensaje.Text = "Solicitud Anulada";
                                            LabModSolAviso.CssClass = "grilla_info_rojo";
                                            LabMensaje.ForeColor = System.Drawing.Color.Red;
                                            LabModSolAviso.Visible = true;
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

                                    //<FINGTI_7012>
                                    ModSolTipoCambio.Text = solicitud.TipoCambio.ToString();
                                    ModSolTipoCambioPanel.Visible = true;
                                    //if (solicitud.MonedaPrimaUnica.Id.ToString().Equals("002"))
                                    //{
                                    //    ModSolTipoCambioPanel.Visible = true;
                                    //}

                                    //<FINGTI_753_3>

                                    //<INIGTI_7012>
                                    ModSolLineaNumPoliza_RP.Visible = false;
                                    if (solicitud.NumeroPoliza != 0)
                                    {
                                        ModSolLineaNumPoliza_RP.Visible = true;
                                        ModSolNumPoliza_RP.Text = solicitud.NumeroPoliza.ToString();
                                    }

                                    //<FINGTI_7012>

                                    ModSolTemporalidad_RP.SelectedValue = "TVT";
                                    ModSolTemporalidad_RP.Enabled = false;
                                    ModSolTemporalidad_RP.CssClass = "formCombobox formComboboxReadOnly";

                                }
                                //Copiar
                                else if (Session["ModSolModo"].ToString() == "C")
                                {
                                    // Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(Session["idSolicitud"].ToString());

                                    ModSolNroSolicitud_RP.Text = solicitud.Id;
                                    ModSolTemporalidad_RP.SelectedIndex = ModSolTemporalidad_RP.Items.IndexOf(ModSolTemporalidad_RP.Items.FindByValue(solicitud.Temporalidad.Id));
                                    ModSolMonedaPrimaUnica_RP.SelectedIndex = ModSolMonedaPrimaUnica_RP.Items.IndexOf(ModSolMonedaPrimaUnica_RP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    ModSolPrimaUnica_RP.Text = solicitud.PrimaUnica.ToString();


                                    ModSolFechaCotizacion_RP.Text = ((DateTime)DateTime.Now).ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_RP.Text = ((DateTime)DateTime.Today.AddDays(-(DateTime.Today.Day - 1))).ToString("dd/MM/yyyy");
                                    //diasVigencia
                                    ModSolFechaVigencia_RP.Text = ((DateTime)DateTime.Now.AddDays(Convert.ToInt32(HDiasVigencia.Value))).ToString("dd/MM/yyyy");

                                    //<INI.GTI_7012_3>//Cambiando de estado, por defecto 02
                                    solicitud.Cotizaciones.ForEach(p =>
                                        p.EstadoCotizacion = "02"
                                        );
                                    //<FIN.GTI_7012_3>

                                    //<INIGTI_753_3>

                                    ModSolDCOM_RP.Text = solicitud.PorcentajeDescuentoComision.ToString();
                                    Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    Session["RP_Cotizaciones"] = solicitud.Cotizaciones;

                                    ModSolTipoPlan_RP.SelectedIndex = ModSolTipoPlan_RP.Items.IndexOf(ModSolTipoPlan_RP.Items.FindByValue(solicitud.TipoPlan.Id));

                                    //<INIGTI_753_3>}

                                    ModSolTipoCambio.Text = solicitud.TipoCambio.ToString();
                                    ModSolTipoCambioPanel.Visible = true;

                                    //<FINGTI_753_3>

                                    //<INIGTI_7012>
                                    ModSolLineaNumPoliza_RP.Visible = false;
                                    //if (solicitud.NumeroPoliza != 0)
                                    //{
                                    //    ModSolLineaNumPoliza_RP.Visible = true;
                                    //    ModSolNumPoliza_RP.Text = solicitud.NumeroPoliza.ToString();
                                    //}

                                    //<FINGTI_7012>
                                    solicitud.NumeroPoliza = 0;

                                    //if (((string)Session["RolAzman"]) != "JEF.RVI.OPE")
                                    //{
                                        ModSolTemporalidad_RP.SelectedValue = "TVT";
                                        ModSolTemporalidad_RP.Enabled = false;
                                        ModSolTemporalidad_RP.CssClass = "formCombobox formComboboxReadOnly";
                                    //}

                                }

                                //Consultar
                                else if (Session["ModSolModo"].ToString() == "CONS")
                                {

                                    // Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(Session["idSolicitud"].ToString());

                                    ModSolNroSolicitud_RP.Text = solicitud.Id;
                                    ModSolTemporalidad_RP.SelectedIndex = ModSolTemporalidad_RP.Items.IndexOf(ModSolTemporalidad_RP.Items.FindByValue(solicitud.Temporalidad.Id));
                                    ModSolMonedaPrimaUnica_RP.SelectedIndex = ModSolMonedaPrimaUnica_RP.Items.IndexOf(ModSolMonedaPrimaUnica_RP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    ModSolPrimaUnica_RP.Text = solicitud.PrimaUnica.ToString();

                                    //<INIGTI_753_3>

                                    ModSolFechaCotizacion_RP.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_RP.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                    ModSolFechaVigencia_RP.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");


                                    //<INIGTI_753_3>

                                    ModSolDCOM_RP.Text = solicitud.PorcentajeDescuentoComision.ToString();
                                    Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    Session["RP_Cotizaciones"] = solicitud.Cotizaciones;

                                    ModSolTipoPlan_RP.SelectedIndex = ModSolTipoPlan_RP.Items.IndexOf(ModSolTipoPlan_RP.Items.FindByValue(solicitud.TipoPlan.Id));

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


                                    //Bloqueando controles
                                    ModSolTemporalidad_RP.Enabled = false;
                                    ModSolMonedaPrimaUnica_RP.Enabled = false;
                                    ModSolTipoPlan_RP.Enabled = false;

                                    ModSolFechaCotizacion_RP.Enabled = false;
                                    ModSolFechaDevengue_RP.Enabled = false;
                                    ModSolFechaVigencia_RP.Enabled = false;

                                    ModSolPrimaUnica_RP.ReadOnly = true;
                                    ModSolTipoCambio.ReadOnly = true;
                                    ModSolDCOM_RP.ReadOnly = true;



                                    ModSolTemporalidad_RP.CssClass = "formCombobox formComboboxReadOnly";
                                    ModSolMonedaPrimaUnica_RP.CssClass = "formCombobox formComboboxReadOnly";
                                    ModSolTipoPlan_RP.CssClass = "formCombobox formComboboxReadOnly";


                                    ModSolFechaCotizacion_RP.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";
                                    ModSolFechaDevengue_RP.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";
                                    ModSolFechaVigencia_RP.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";



                                    ModSolDCOM_RP.CssClass = "formTextbox formTextboxReadOnly";
                                    ModSolTipoCambio.CssClass = "formTextbox formTextboxReadOnly";
                                    ModSolPrimaUnica_RP.CssClass = "formTextbox formTextboxReadOnly";

                                    //<INIGTI_7012>
                                    ModSolLineaNumPoliza_RP.Visible = false;
                                    if (solicitud.NumeroPoliza != 0)
                                    {
                                        ModSolLineaNumPoliza_RP.Visible = true;
                                        ModSolNumPoliza_RP.Text = solicitud.NumeroPoliza.ToString();
                                    }

                                    //<FINGTI_7012>

                                    //<INIGTI_7012>
                                    HEstado.Value = solicitud.CodigoEstado.ToString();
                                    LabMensaje.Text = solicitud.EstadoSolicitud.ToString(); //"Solicitud Seleccionada";
                                    ModSolEstadoPoliza_RP.Text = solicitud.EstadoPoliza;
                                    switch (solicitud.CodigoEstado.ToString())
                                    {
                                        case "1":
                                            //LabMensaje.Text = "Solicitud Seleccionada";
                                            LabModSolAviso.CssClass = "grilla_info_verde";
                                            LabMensaje.ForeColor = System.Drawing.Color.Green;
                                            LabModSolAviso.Visible = true;
                                            break;
                                        case "2":
                                            //LabMensaje.Text = "Solicitud Cerrada";
                                            LabModSolAviso.CssClass = "grilla_info";
                                            LabModSolAviso.Visible = true;
                                            break;
                                        case "3":
                                            //LabMensaje.Text = "Solicitud Anulada";
                                            LabModSolAviso.CssClass = "grilla_info_rojo";
                                            LabMensaje.ForeColor = System.Drawing.Color.Red;
                                            LabModSolAviso.Visible = true;
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
                                    //<FINGTI_7012>

                                }

                                //Nuevo
                                else if (Session["ModSolModo"].ToString() == "N")
                                {
                                    HSeleccionada.Value = "N";
                                    HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();
                                    HAFP_RP.Value = Session["AFP_RP"].ToString();

                                    ModSolFechaCotizacion_RP.Text = ((DateTime)DateTime.Now).ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_RP.Text = ((DateTime)DateTime.Today.AddDays(-(DateTime.Today.Day - 1))).ToString("dd/MM/yyyy");

                                    //diasVigencia
                                    ModSolFechaVigencia_RP.Text = ((DateTime)DateTime.Now.AddDays(Convert.ToInt32(HDiasVigencia.Value))).ToString("dd/MM/yyyy");

                                    //<INIGTI_7012>
                                    ModSolLineaNumPoliza_RP.Visible = false;
                                    HEstado.Value = "0";
                                    //<FINGTI_7012>

                                    //if (((string)Session["RolAzman"]) != "JEF.RVI.OPE")
                                    //{
                                        ModSolTemporalidad_RP.SelectedValue = "TVT";
                                        ModSolTemporalidad_RP.Enabled = false;
                                        ModSolTemporalidad_RP.CssClass = "formCombobox formComboboxReadOnly";
                                    //}

                                }
                                ModSolModo.Value = Convert.ToString((Session["ModSolModo"]));
                                HCopia.Value = "";

                                if (Convert.ToString((Session["ModSolModo"])) == "C")
                                    HCopia.Value = "C";


                                ModSolPrimaUnica_RP.Visible = true;
                                ManSolTipoSolicitud_RP.Value = "EXTRAOFICIAL";


                                //<INIGTI_753_3>
                                HBloqueo.Value = "TRUE";
                                if (((string)Session["RolAzman"]) == "JEF.RVI.OPE" 
                                        || ((string)Session["RolAzman"]) == "AST.RVI.COM"
                                        || ((string)Session["RolAzman"]) == "JEF.VTA.LIM.RVI"
                                        || ((string)Session["RolAzman"]) == "JEF.VTA.PRO.RVI")
                                {
                                    HBloqueo.Value = "FALSE";
                                }
                                //<FINGTI_753_3>

                                //Validando la fecha de cotizacion
                                switch ((string)Session["RolAzman"])
                                {
                                    case "JEF.RVI.OPE"://JefeOperaciones
                                    case "AST.RVI.OPE"://AsistenteOperaciones
                                        if (Session["ModSolModo"].ToString() != "CONS")
                                        {
                                            ModSolFechaCotizacion_RP.Enabled = true;
                                        }
                                        break;
                                    default:
                                        ModSolFechaCotizacion_RP.Enabled = false;
                                        break;
                                }
                            }
                            else
                            {
                                Response.Redirect("Cotizador.aspx");
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

        [WebMethod]
        public static SolicitudRPPlus ObtenerCotizacionesBeneficiarios(string solicitud)
        {
            return new SolicitudRPPlus
            {
                Id = solicitud,
                Cotizaciones = (List<CotizacionRPPlus>)HttpContext.Current.Session["RP_Cotizaciones"],
                Beneficiarios = (List<GrupoFamiliar>)HttpContext.Current.Session["RP_Beneficiarios"]
            };
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
            //lstPjeDevolucion = lstPjeDevolucion.Sort(x=> x.Valor,0);
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

        private void CargarCombobox(DropDownList control, List<Ciudad> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Ciudad item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }

        private void CargarCombobox(DropDownList control, List<Comuna> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Comuna item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
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

        //<INIGTI_753>
        [WebMethod]
        public static string CargarTablaCotizaciones(List<CotizacionRPPlus> cotizaciones, string temporalidad, string moneda, string conyuge)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio MantenerSolicitud.CargarTablaCotizaciones WebMethod");

                    List<CotizacionRPPlus> cotizacionesNew = new List<CotizacionRPPlus>();
                    var pagina = new Page();
                    var control = (TablaCotizacionesRentaPrivadaPlus)pagina.LoadControl("~/Controles/TablaCotizacionesRentaPrivadaPlus.ascx");

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

                    log.Debug("Fin MantenerSolicitud.CargarTablaCotizaciones WebMethod");

                    return html;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    log.Debug("Fin MantenerSolicitud.CargarTablaCotizaciones WebMethod");
                    throw (ex);
                }
            }
        }

        //<FINGTI_753>

        //<INIGTI_753>
        [WebMethod]
        public static List<Parametro> ObtenerMonedaAjuste(string codMoneda)
        {

            log.Debug("Inicio MantenerSolicitud.ObtenerMonedaAjuste WebMethod");

            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametro2 = new List<Parametro>();
            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboMonedaAjustePlus"];

            if (lstParametro.Count > 0)
            {
                lstParametro
                            .FindAll(p => (
                                           (p.Id == codMoneda)
                                          )
                            )
                            .ForEach(p =>
                            {
                                for (var i = Convert.ToInt32(p.Valor_1); i <= Convert.ToInt32(p.Valor_2); i++)
                                {
                                    lstParametro2.Add(new Parametro { Valor_1 = i.ToString() + '%', Valor_2 = i.ToString() });
                                }

                            });
            }

            if (lstParametro2.Count == 0)
            {
                lstParametro2.Add(new Parametro { Valor_1 = "-", Valor_2 = "-1" });
            }

            log.Debug("Fin MantenerSolicitud.ObtenerMonedaAjuste WebMethod");

            return lstParametro2;
        }
        //<FINGTI_753>

    }
}