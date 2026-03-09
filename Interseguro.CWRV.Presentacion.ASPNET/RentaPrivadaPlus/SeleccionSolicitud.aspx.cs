using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloSeguridad;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
{
    public partial class SeleccionSolicitud : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SeleccionSolicitud));
        private static IServicioCWRV servicioCotizador;
        private static IServicioAzman servicioAzman;

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
                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                            //LimpiarFormularios();

                            if (Session["ModSolModo"].ToString() != null)
                            {
                                HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();
                                HAFP_RP.Value = Session["AFP_RP"].ToString();

                                LabModSolAviso.Visible = false;

                                // Validar si la solicitud tiene firma digital aceptada
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                FirmaDigital firma = servicioCotizador.ObtenerFirmaDigital(Session["idSolicitud"].ToString(), 1, Session["usuario"].ToString());
                                
                                bool firmado = false;
                                if (firma != null)
                                {
                                    HFirmado.Value = firma.ind_consentimiento;
                                    firmado = firma.ind_consentimiento == "S";
                                }
                                
                                //Consultar
                                if (Session["ModSolModo"].ToString() == "CERRAR")
                                {

                                    // Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(Session["idSolicitud"].ToString());
                                    // TODO HLS: Leer de Benefi en lugar de grupo familiar

                                    List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                                    Session["ComboPorcentajeEscalonada"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.NuevoPorcentajeEscalonado];

                                    //GrupoFamiliar afiliado2 = servicioCotizador.ObtenerDatosGrupoFamiliar((int)afiliado.Id, Session["idSolicitud"].ToString());

                                    HNroSolicitud.Value = solicitud.Id;

                                    // Obtener los datos del afiliado
                                    var afiliado = servicioCotizador.ListarBeneficiarios(Session["idSolicitud"].ToString(), Session["usuario"].ToString()).Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

                                    // TODO HLS: Leer de Benefi en lugar de grupo familiar
                                    HAfiliadoNombreCompleto.Value = afiliado.Nombre + " " + afiliado.ApellidoPaterno + " " + afiliado.ApellidoMaterno;
                                    HAfiliadoEmail.Value = afiliado.CorreoElectronico;
                                    HAfiliadoTelefono.Value = afiliado.numCelular;
                                    HAfiliadoTipoDocumento.Value = afiliado.Identificacion.GlosaTipo;
                                    HAfiliadoNumeroDocumento.Value = afiliado.Identificacion.Numero;
                                    HAfiliadoPEP.Value = afiliado.indPEP;

                                    ModSolNroSolicitud_RP.Text = solicitud.Id;
                                    ModSolTemporalidad_RP.SelectedIndex = ModSolTemporalidad_RP.Items.IndexOf(ModSolTemporalidad_RP.Items.FindByValue(solicitud.Temporalidad.Id));
                                    ModSolMonedaPrimaUnica_RP.SelectedIndex = ModSolMonedaPrimaUnica_RP.Items.IndexOf(ModSolMonedaPrimaUnica_RP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    ModSolTipoPlan_RP.SelectedIndex = ModSolTipoPlan_RP.Items.IndexOf(ModSolTipoPlan_RP.Items.FindByValue(solicitud.TipoPlan.Id));
                                    ModSolPrimaUnica_RP.Text = solicitud.PrimaUnica.ToString("#,##0.00");

                                    ModSolTemporalidadText_RP.Text = ModSolTemporalidad_RP.SelectedItem.Text;
                                    ModSolMonedaPrimaUnicaText_RP.Text = ModSolMonedaPrimaUnica_RP.SelectedItem.Text;
                                    ModSolTipoPlanText_RP.Text = ModSolTipoPlan_RP.SelectedItem.Text;
                                    //<INIGTI_753_3>

                                    ModSolFechaCotizacion_RP.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_RP.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                    ModSolFechaVigencia_RP.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");

                                    ModSolDCOM_RP.Text = solicitud.PorcentajeDescuentoComision.ToString();
                                    Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    Session["RP_Cotizaciones"] = solicitud.Cotizaciones;

                                    CotizacionRPPlus cotizacionSeleccionada = solicitud.Cotizaciones.Find(p => (p.EstadoCotizacion == "04" || p.EstadoCotizacion == "05") && p.IndSeleccionada == "S");
                                    if (cotizacionSeleccionada != null)
                                    {
                                        if (cotizacionSeleccionada.EstadoCotizacion == "04" && firmado)
                                        {
                                            // Cotización cerrada y firmada por el cliente
                                            HSeleccionada.Value = "S";
                                            LabModSolAviso.Visible = true;

                                        }
                                        else if (cotizacionSeleccionada.EstadoCotizacion == "05" && !firmado)
                                        {
                                            // Cotización aún no firmada
                                            HSeleccionada.Value = "S";
                                            LabModSolAviso.Visible = false;
                                        }
                                    }

                                    ModSolTipoCambio.Text = solicitud.TipoCambio.ToString();
                                    ModSolTipoCambioPanel.Visible = false;
                                    if (solicitud.MonedaPrimaUnica.Id.ToString().Equals("002"))
                                    {
                                        ModSolTipoCambioPanel.Visible = true;
                                    }

                                    // Bloqueando controles
                                    ModSolTemporalidad_RP.Enabled = false;
                                    ModSolMonedaPrimaUnica_RP.Enabled = false;
                                    ModSolTipoPlan_RP.Enabled = false;

                                    ModSolFechaCotizacion_RP.Enabled = false;
                                    ModSolFechaDevengue_RP.Enabled = false;
                                    ModSolFechaVigencia_RP.Enabled = false;

                                    HEstado.Value = solicitud.CodigoEstado.ToString();
                                    HEstadoPlaft.Value = solicitud.CodigoEstadoPlaft.ToString();

                                    LabMensaje.Text = solicitud.EstadoSolicitud.ToString(); //"Solicitud Seleccionada";

                                    switch (solicitud.CodigoEstado.ToString())
                                    {
                                        case "0":
                                            LabModSolAviso.CssClass = "grilla_info";
                                            break;
                                        case "1":
                                            //LabMensaje.Text = "Solicitud Seleccionada";
                                            LabModSolAviso.CssClass = "grilla_info_verde";
                                            LabMensaje.ForeColor = System.Drawing.Color.Green;
                                            LabModSolAviso.Visible = true;

                                            ModSolEnviarEvaluacion.Visible = true;
                                            ModSolAgregarArchivos.Visible = firmado;
                                            if (!firmado)
                                            {
                                                ModSolAgregarArchivos.Visible = false;
                                            }
                                            break;
                                        case "2":
                                            LabModSolAviso.CssClass = "grilla_info";
                                            LabModSolAviso.Visible = true;
                                            break;
                                        case "3":
                                            //LabMensaje.Text = "Solicitud Anulada";
                                            LabModSolAviso.CssClass = "grilla_info_rojo";
                                            LabMensaje.ForeColor = System.Drawing.Color.Red;
                                            LabModSolAviso.Visible = true;
                                            break;
                                        case "4"://Evaluacion
                                            LabModSolAviso.CssClass = "grilla_info_verde";
                                            LabMensaje.ForeColor = System.Drawing.Color.Green;
                                            LabModSolAviso.Visible = true;

                                            ModSolEnviarEvaluacion.Visible = true;

                                            if (solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Observado.StringValue())
                                            {
                                                LabMensaje.Text = "Solicitud Observada";
                                            }
                                            break;
                                        case "5"://Observado

                                            LabModSolAviso.CssClass = "grilla_info";
                                            LabModSolAviso.Visible = true;

                                            if (firma.ind_consentimiento == "S")
                                            {
                                                ModSolEnviarEvaluacion.Visible = true;
                                            }
                                            if (Utilitarios.EsRolComercial(Session["RolAzman"].ToString()) && firma.ind_consentimiento != "R")
                                            {
                                                CorregirDocumentos.Visible = true;
                                            }
                                            ModSolAgregarArchivos.Visible = firmado;

                                            if (solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Evaluacion.StringValue())
                                            {
                                                LabMensaje.Text = "Solicitud en Evaluación";
                                            }

                                            break;
                                        case "6"://Aprobado

                                            LabModSolAviso.CssClass = "grilla_info";
                                            LabModSolAviso.Visible = true;

                                            if (solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Observado.StringValue())
                                            {
                                                LabMensaje.Text = "Solicitud Observada";
                                            }

                                            if (solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Evaluacion.StringValue())
                                            {
                                                LabMensaje.Text = "Solicitud en Evaluación";
                                            }

                                            break;
                                        case "7"://Rechazado
                                            LabModSolAviso.CssClass = "grilla_info_rojo";
                                            LabMensaje.ForeColor = System.Drawing.Color.Red;
                                            LabModSolAviso.Visible = true;
                                            break;
                                        default:
                                            LabModSolAviso.CssClass = "grilla_info";
                                            ModSolAgregarArchivos.Visible = firmado;
                                            break;
                                    }

                                    // CodigoEstado(5=Observado)
                                    // CodigoEstadoPlaft(5=Observado, 6=Aprobado)
                                    if (solicitud.CodigoEstado.ToString() == "5" && (
                                        solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Observado.StringValue() ||
                                        solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Aprobado.StringValue()
                                        ))
                                    {
                                        ModSolEnviarEvaluacion.Enabled = true;
                                        ModSolEnviarEvaluacion.Visible = true;
                                        //ModSolEnviarEvaluacion.CssClass = "botonDeshabilitado gris gris_sharp";
                                    }
                                    else if (solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Observado.StringValue() && (
                                       solicitud.CodigoEstado.ToString() == "5" ||
                                       solicitud.CodigoEstado.ToString() == "6"
                                       ))
                                    {
                                        ModSolEnviarEvaluacion.Enabled = true;
                                        ModSolEnviarEvaluacion.Visible = true;
                                        //ModSolEnviarEvaluacion.CssClass = "botonDeshabilitado gris gris_sharp";
                                    }
                                    else if (solicitud.CodigoEstadoPlaft.ToString() == "-1" &&
                                       solicitud.CodigoEstado.ToString() == "6"
                                       )
                                    {
                                        ModSolEnviarEvaluacion.Visible = false;
                                        ModSolEnviarEvaluacion.Enabled = false;
                                        ModSolEnviarEvaluacion.CssClass = "botonDeshabilitado gris gris_sharp";
                                    }
                                }

                                ModSolModo.Value = Convert.ToString((Session["ModSolModo"]));
                                HCopia.Value = "";

                                if (Convert.ToString((Session["ModSolModo"])) == "C")
                                    HCopia.Value = "C";

                                ModSolPrimaUnica_RP.Visible = true;
                                ManSolTipoSolicitud_RP.Value = "EXTRAOFICIAL";

                                HBloqueo.Value = "TRUE";
                                if (((string)Session["RolAzman"]) == "JEF.RVI.OPE")
                                {
                                    HBloqueo.Value = "FALSE";
                                    //no va true solo por pruebas
                                    //HBloqueo.Value = "TRUE";
                                }

                                //Validando la fecha de cotizacion
                                ModSolFechaCotizacion_RP.Enabled = false;

                                // Habilitar botón de Reenvío Manual si corresponde
                                if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.PlantillaCorreoElectronico) && firma != null)
                                {
                                    ReenvioManual.Visible = true;
                                    ReenvioManual.NavigateUrl = ResolveUrl(string.Format("~/Comun/PlantillaCorreoElectronico.aspx?tp=4&s={0}&i=1", Session["idSolicitud"]));
                                }
                            }
                            else
                            {
                                Response.Redirect("Cotizador.aspx");
                            }
                        }
                        else
                        {
                            if (ModSolPrimaUnica_RP.Text != string.Empty) ModSolPrimaUnica_RP.Text = Convert.ToDouble(ModSolPrimaUnica_RP.Text, new CultureInfo("es-PE")).ToString();
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.MenuCotizador.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ex.Message });
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

            CargarCombobox(ModSolTemporalidad_RP, listaCombobox[(int)Enums.CategoriaCombobox.Temporalidad]);
            CargarCombobox(ModSolMonedaPrimaUnica_RP, listaCombobox[(int)Enums.CategoriaCombobox.MonedaRentaPrivada]);
            CargarCombobox(ModSolTipoPlan_RP, listaCombobox[(int)Enums.CategoriaCombobox.TipoPlan]);
            Session["ComboMoneda"] = listaCombobox[(int)Enums.CategoriaCombobox.Moneda];
            Session["ComboPorcentajeEscalonada"] = listaCombobox[(int)Enums.CategoriaCombobox.NuevoPorcentajeEscalonado];
            List<Parametro> lstPjeDevolucion = new List<Parametro>();
            lstPjeDevolucion = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeDevolucion];
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

            Session["ComboPorcentajeConyuge"] = listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeConyuge];

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            LabModSolLineaACOMDCOM_RP.Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);

            if (!LabModSolLineaACOMDCOM_RP.Visible)
            {
                LabModSolFechaVigencia_RP.CssClass = "formLabel formLabel2Izq";
            }

            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;

            List<Parametro> lstParametro = new List<Parametro>();
            Session["ComboMonedaAjustePlus"] = listaCombobox[(int)Enums.CategoriaCombobox.MonedaAjustePlus];

            List<Parametro> lstDiasVigencia = new List<Parametro>();
            lstDiasVigencia = servicioCotizador.ObtenerParametrosPorTabla("PLUS");
            if (lstDiasVigencia.Count() > 0)
            {
                HDiasVigencia.Value = lstDiasVigencia[0].Valor_1;
            }
        }


        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            if (ModSolMonedaPrimaUnica_RP.ClientID != "ModSolMonedaPrimaUnica_RP")
            {
                control.Items.Add(new System.Web.UI.WebControls.ListItem("«Seleccione»", "0"));
            }
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new System.Web.UI.WebControls.ListItem(item.Glosa, item.Id));
            }
        }

        private void CargarCombobox(DropDownList control, List<MontoCIC> combobox)
        {
            control.Items.Clear();
            foreach (MontoCIC item in combobox)
            {
                control.Items.Add(new System.Web.UI.WebControls.ListItem(String.Format("{0:#,##0.00}", item.Valor), item.Valor.ToString()));
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
        public static Respuesta CerrarCotizacionPlus(string tokenUsuario, string num_solicitud, string num_correlativo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusCerrar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarCotizacion(num_solicitud, num_correlativo, errores, controles))
                            {
                                string usuario = (string)HttpContext.Current.Session["Usuario"];

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                // respuesta = servicioCotizador.CerrarCotizacionPlus(num_solicitud, Convert.ToInt32(num_correlativo), usuario, gf);
                                
                                // Se reemplaza el método de CerrarCotizacionPlus por el de Vista Previa porque este proceso ya no le cambiará
                                // el estado a 04 ni creará el step del flujo de aprobación, limitándose sólo a la creación de la entidad de
                                // Firma Digital en caso de que no exista y el envío del link al cliente
                                respuesta = servicioCotizador.VistaPreviaCotizacionPlus(num_solicitud, Convert.ToInt32(num_correlativo), usuario);

                                // Obtener los datos del afiliado
                                Beneficiario afiliado = servicioCotizador.ListarBeneficiarios(num_solicitud, usuario).Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());
                                
                                if (!string.IsNullOrEmpty(afiliado.numCuspp))
                                {
                                    log.Debug(string.Format("Se va a actualizar la dirección. CUSPP[{0}] Solicitud[{1}] Usuario[{2}]", afiliado.numCuspp, num_solicitud, usuario));
                                    Respuesta direccion = servicioCotizador.ActualizarDireccionSolicitud(afiliado.numCuspp, num_solicitud, usuario);
                                }

                                // Obtener datos del agente
                                List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                                Agente agente = listaAgentes.Find(a => a.Id == HttpContext.Current.Session["Vendedor"].ToString());

                                // Verificar si es que el cliente tiene ya un registro de Firma Digital
                                FirmaDigital firma = servicioCotizador.ObtenerFirmaDigital(num_solicitud, 1, usuario);
                                if (firma == null)
                                {
                                    // Crear la firma digital
                                    firma = new FirmaDigital
                                    {
                                        num_solicitud = num_solicitud,
                                        num_item = 1,
                                        ind_consentimiento = null,
                                        gls_browser_agent = HttpContext.Current.Request.UserAgent,
                                        aud_usr_ingreso = usuario
                                    };
                                    firma = servicioCotizador.RegistrarFirmaDigital(firma);
                                    string url = string.Format(ConfigurationManager.AppSettings["url_app_firmas_digitales"], firma.gls_token);

                                    // Armando el correo electrónico
                                    string remitente = ConfigurationManager.AppSettings["remitente_consentimiento"];
                                    string asunto = ConfigurationManager.AppSettings["asunto_firmas_digitales"];
                                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                                    string cuerpo = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\Plantilla\RP\SADP\SADP.html");
                                    cuerpo = cuerpo.Replace("{link}", url.Trim())
                                                   .Replace("{nombres}", ti.ToTitleCase(afiliado.Nombre.Trim().ToLower()))
                                                   .Replace("{agente}", ti.ToTitleCase(agente.Nombre.ToString().Trim().ToLower()));

                                    DocumentoSME documentoSME = new DocumentoSME
                                    {
                                        Email = afiliado.CorreoElectronico,
                                        NumeroPoliza = "N/A",
                                        NumeroDocumento = afiliado.Identificacion.Numero,
                                        Destinatario = string.Format("{0} {1} {2}", afiliado.Nombre, afiliado.ApellidoMaterno, afiliado.ApellidoMaterno).Trim(),
                                        ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEFirmaDigitalRP"]),
                                        CamposDinamicos = new
                                        {
                                            Id_nombres = ti.ToTitleCase(afiliado.Nombre.ToLower().Trim()),
                                            Id_link = url.Trim(),
                                            Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                                        }
                                    };

                                    log.Info("Enviando el correo SME");
                                    Respuesta estadoCorreo = Utilitario.EnviarDocumentoSME(documentoSME);
                                    if (estadoCorreo.Estado != Constante.COD_OK)
                                    {
                                        log.Error(string.Format("Error al enviar el correo del cliente [{0}]", afiliado.CorreoElectronico));
                                        throw new Exception("Error al enviar el correo del cliente.");
                                    }

                                    dynamic respDocumentoSME = JsonConvert.DeserializeObject(estadoCorreo.Mensaje);
                                    long idSME = respDocumentoSME.codigoSME;

                                    // Insertar en la tabla de seguimiento
                                    EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                                    {
                                        gls_identificador = string.Format("{0}|{1}", 1, num_solicitud),
                                        id_proceso_envio = (int)Enums.ProcesoEnvio.FirmaDigitalRP,
                                        id_sme = idSME,
                                        cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                        gls_mail = afiliado.CorreoElectronico,
                                        fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                                        cod_agente = agente.Id,
                                        aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                                    };
                                    string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                                    var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                                    string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                                    log.Info("Consumiendo API de envío de documentos SME");
                                    log.Debug(string.Format("Request Body[{0}]", jsonString));
                                    using (var client = new WebClient())
                                    {
                                        client.Encoding = Encoding.UTF8;
                                        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                        respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                                        respuesta.Estado = Constante.COD_OK;
                                    }

                                    respuesta.Estado = Constante.COD_OK;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                    respuesta.Mensaje = "Correo con el enlace para Firma Digital enviado correctamente";
                                    respuesta.Controles = controles;
                                }
                                else
                                {
                                    // Reenviar correo al cliente
                                    string url = string.Format(ConfigurationManager.AppSettings["url_app_firmas_digitales"], firma.gls_token);

                                    // Ya tiene firma digital, validar si es positiva
                                    if (firma.ind_consentimiento == "N" || firma.ind_consentimiento == null)
                                    {
                                        // Armando el cuerpo del correo
                                        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                                        var cuerpo = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\Plantilla\RP\SADP\SADP.html");
                                        cuerpo = cuerpo.Replace("{link}", url.Trim())
                                                       .Replace("{nombres}", ti.ToTitleCase(afiliado.Nombre.Trim().ToLower()))
                                                       .Replace("{agente}", ti.ToTitleCase(agente.Nombre.ToString().Trim().ToLower()));

                                        DocumentoSME documentoSME = new DocumentoSME
                                        {
                                            Email = afiliado.CorreoElectronico,
                                            NumeroPoliza = "N/A",
                                            NumeroDocumento = afiliado.Identificacion.Numero,
                                            Destinatario = string.Format("{0} {1} {2}", afiliado.Nombre, afiliado.ApellidoMaterno, afiliado.ApellidoMaterno).Trim(),
                                            ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEFirmaDigitalRP"]),
                                            CamposDinamicos = new
                                            {
                                                Id_nombres = ti.ToTitleCase(afiliado.Nombre.ToLower().Trim()),
                                                Id_link = url.Trim(),
                                                Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                                            }
                                        };

                                        log.Info("Enviando el correo SME");
                                        Respuesta estadoCorreo = Utilitario.EnviarDocumentoSME(documentoSME);
                                        if (estadoCorreo.Estado != Constante.COD_OK)
                                        {
                                            log.Error(string.Format("Error al enviar el correo del cliente [{0}]", afiliado.CorreoElectronico));
                                            throw new Exception("Error al enviar el correo del cliente.");
                                        }

                                        dynamic respDocumentoSME = JsonConvert.DeserializeObject(estadoCorreo.Mensaje);
                                        long idSME = respDocumentoSME.codigoSME;

                                        // Insertar en la tabla de seguimiento
                                        EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                                        {
                                            gls_identificador = string.Format("{0}|{1}", 1, num_solicitud),
                                            id_proceso_envio = (int)Enums.ProcesoEnvio.FirmaDigitalRP,
                                            id_sme = idSME,
                                            cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                            gls_mail = afiliado.CorreoElectronico,
                                            fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                                            cod_agente = agente.Id,
                                            aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                                        };
                                        string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                                        var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                                        string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                                        log.Info("Consumiendo API de envío de seguimiewnto");
                                        log.Debug(string.Format("Request Body[{0}]", jsonString));
                                        using (var client = new WebClient())
                                        {
                                            client.Encoding = Encoding.UTF8;
                                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                            respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                                            respuesta.Estado = Constante.COD_OK;
                                        }

                                        respuesta.Estado = Constante.COD_OK;
                                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                                        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                        respuesta.Mensaje = "Correo con el enlace para Firma Digital enviado correctamente";
                                        respuesta.Controles = controles;
                                    }
                                    else if (firma.ind_consentimiento == "R")
                                    {
                                        // Armando el correo electrónico para corrección de datos
                                        string remitente = ConfigurationManager.AppSettings["remitente_consentimiento"];
                                        string asunto = ConfigurationManager.AppSettings["asunto_firmas_digitales"];
                                        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                                        string cuerpo = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\Plantilla\RP\SADP\SADP.html");
                                        cuerpo = cuerpo.Replace("{link}", url.Trim())
                                                       .Replace("{nombres}", ti.ToTitleCase(afiliado.Nombre.Trim().ToLower()))
                                                       .Replace("{agente}", ti.ToTitleCase(agente.Nombre.ToString().Trim().ToLower()));

                                        // Obtener datos del beneficiario
                                        SolicitudIFP solicitudIFP = servicioCotizador.ObtenerDatosSolicitudIFP(num_solicitud);
                                        GrupoFamiliar beneficiario = solicitudIFP.Beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

                                        // Obtener los datos de la versión solicitada y de la anterior para comparar
                                        List<FormatoSolicitud> formatos = servicioCotizador.ListarFormatosSolicitud(num_solicitud, HttpContext.Current.Session["Usuario"].ToString());
                                        int idFormatoPrevio = formatos.OrderByDescending(f => f.Correlativo).First().Id;

                                        // Obtener versiones
                                        FormatoSolicitud formatoActual = servicioCotizador.ObtenerFormatoSolicitudActualizado(num_solicitud, HttpContext.Current.Session["Usuario"].ToString());
                                        FormatoSolicitud formatoPrevio = servicioCotizador.ObtenerFormatoSolicitud(num_solicitud, idFormatoPrevio, HttpContext.Current.Session["Usuario"].ToString());
                                        List<FormatoSolicitudBeneficiario> formatoBeneficiariosActual = servicioCotizador.ListarFormatoSolicitudBeneficiarioActualizado(num_solicitud, HttpContext.Current.Session["Usuario"].ToString());
                                        List<FormatoSolicitudBeneficiario> formatoBeneficiariosPrevio = servicioCotizador.ListarFormatoSolicitudBeneficiario(num_solicitud, idFormatoPrevio, HttpContext.Current.Session["Usuario"].ToString());
                                        List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasActual = servicioCotizador.ListarFormatoSolicitudPersonaVinculadaActualizado(num_solicitud, HttpContext.Current.Session["Usuario"].ToString());
                                        List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasPrevio = servicioCotizador.ListarFormatoSolicitudPersonaVinculada(num_solicitud, idFormatoPrevio, HttpContext.Current.Session["Usuario"].ToString());

                                        // Comparar versiones
                                        string tabla = Utilitarios.CrearTablaComparativa(formatoActual, formatoPrevio, formatoBeneficiariosActual, formatoBeneficiariosPrevio, formatoPersonasVinculadasActual, formatoPersonasVinculadasPrevio);

                                        DocumentoSME documentoSME = new DocumentoSME
                                        {
                                            Email = afiliado.CorreoElectronico,
                                            NumeroPoliza = "N/A",
                                            NumeroDocumento = afiliado.Identificacion.Numero,
                                            Destinatario = string.Format("{0} {1} {2}", afiliado.Nombre, afiliado.ApellidoMaterno, afiliado.ApellidoMaterno).Trim(),
                                            ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEFirmaDigitalRPFlujoCorreccion"]),
                                            CamposDinamicos = new
                                            {
                                                Id_nombre = ti.ToTitleCase(afiliado.Nombre.ToLower().Trim()),
                                                Id_link = url.Trim(),
                                                Id_tabla = tabla
                                            }
                                        };

                                        log.Info("Enviando el correo SME");
                                        Respuesta estadoCorreo = Utilitario.EnviarDocumentoSME(documentoSME);
                                        if (estadoCorreo.Estado != Constante.COD_OK)
                                        {
                                            log.Error(string.Format("Error al enviar el correo del cliente [{0}]", afiliado.CorreoElectronico));
                                            throw new Exception("Error al enviar el correo del cliente.");
                                        }

                                        dynamic respDocumentoSME = JsonConvert.DeserializeObject(estadoCorreo.Mensaje);
                                        long idSME = respDocumentoSME.codigoSME;

                                        // Insertar en la tabla de seguimiento
                                        EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                                        {
                                            gls_identificador = string.Format("{0}|{1}", 1, num_solicitud),
                                            id_proceso_envio = (int)Enums.ProcesoEnvio.FirmaDigitalRP,
                                            id_sme = idSME,
                                            cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                            gls_mail = afiliado.CorreoElectronico,
                                            fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                                            cod_agente = agente.Id,
                                            aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                                        };
                                        string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                                        var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                                        string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                                        log.Info("Consumiendo API de envío de seguimiento");
                                        log.Debug(string.Format("Request Body[{0}]", jsonString));
                                        using (var client = new WebClient())
                                        {
                                            client.Encoding = Encoding.UTF8;
                                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                            respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                                            respuesta.Estado = Constante.COD_OK;
                                        }

                                        respuesta.Estado = Constante.COD_OK;
                                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                                        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                        respuesta.Mensaje = "Correo con el enlace para Firma Digital enviado correctamente";
                                        respuesta.Controles = controles;
                                    }
                                    else
                                    {
                                        // El cliente ya firmó digitalmente, mostrar error
                                        respuesta.Estado = Constante.COD_ERROR;
                                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                        respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                        respuesta.Mensaje = "El cliente ya firmó digitalmente esta solicitud, no puede enviarla nuevamente.";
                                    }
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
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusCerrar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
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
        public static Respuesta ReaperturarDocumentos(string tokenUsuario, string num_solicitud, string num_correlativo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusCerrar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarCotizacion(num_solicitud, num_correlativo, errores, controles))
                            {
                                string usuario = (string)HttpContext.Current.Session["Usuario"];

                                // Cambiar el estado de la cotización a 05 en rvi_cotiza
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.VistaPreviaCotizacionPlus(num_solicitud, Convert.ToInt32(num_correlativo), usuario);

                                // Verificar si es que el cliente tiene ya un registro de Firma Digital
                                FirmaDigital firma = servicioCotizador.ObtenerFirmaDigital(num_solicitud, 1, usuario);
                                if (firma != null)
                                {
                                    // Actualizar la firma digital
                                    firma.ind_consentimiento = "R";
                                    firma.aud_usr_modificacion = HttpContext.Current.Session["Usuario"].ToString();
                                    firma = servicioCotizador.ActualizarFirmaDigital(firma);

                                    respuesta.Estado = Constante.COD_OK;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                    respuesta.Mensaje = "Correo con el enlace para Firma Digital enviado correctamente";
                                    respuesta.Controles = controles;
                                }
                                else
                                {
                                    // El cliente ya firmó digitalmente, mostrar error
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = "La solicitud firma digital de esta solicitud no pudo ser creada.";
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
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusCerrar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
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
        public static Respuesta VistaPreviaCotizacionPlus(string tokenUsuario, string num_solicitud, string num_correlativo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusCerrar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();

                            if (ValidarCotizacion(num_solicitud, num_correlativo, errores, controles))
                            {
                                string usuario = (string)HttpContext.Current.Session["Usuario"];

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                if (!servicioCotizador.isFirmaDigitalAprobada(num_solicitud, 1, usuario))
                                {
                                    respuesta = servicioCotizador.VistaPreviaCotizacionPlus(num_solicitud, Convert.ToInt32(num_correlativo), usuario);
                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_OK;
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
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusCerrar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                    return respuesta;
                }
            }
        }

        private static bool ValidarCotizacion(string num_solicitud, string num_cotizacion, List<String> errores, List<String> controles)
        {

            bool esCorrecto = true;

            // Solicitud
            bool solicitud = true;
            if (num_solicitud == "")
            {
                errores.Add("Seleccione la <strong>Solicitud</strong>. Dato Obligatorio.");
                solicitud = false;
            }

            // Cotización
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

            esCorrecto = solicitud & cotizacion;

            return esCorrecto;
        }

        //<INIGTI_7012>


        protected void ModSolVistaPrevia_RP_Click(object sender, EventArgs e)
        {
            //bool validacion_centrolaboral = false;
            //bool validacion_actividadeconomicaname = false;
            //bool validacion_ram = false;
            //bool validacion_jobtitle = false;
            //bool validacion_direccion = true;

            //List<string> errores = new List<string>();

            //List<DatosSol> lstDatossolicitud = new List<DatosSol>();

            //servicioCotizador = LocalizadorProxy.ObtenerServicio();

            //lstDatossolicitud = servicioCotizador.ObtenerDatosporSolicitud(ModSolNroSolicitud_RP.Text);

            //DatosSol datossolicitud = lstDatossolicitud.ToList().FirstOrDefault();

            //cont = (Contratante[])Session["ListadoContratante"];

            //if (cont.Count() == 0)
            //{
            //    log.Debug("El Afiliado no se encuentra registrado en el CRM");
            //    ClientScript.RegisterStartupScript(GetType(), "mensaje", "MostrarModal('El Afiliado no se encuentra registrado en el CRM');", true);
            //    return;
            //}

            //if (cont[0].inter_centrolaboral.Trim().Length > 0 && cont[0].inter_centrolaboral.ToString().ToUpper() != "NULL")
            //{
            //    validacion_centrolaboral = true;
            //}
            //else if (cont[0].inter_centrolaboral.ToString().ToUpper() == "NULL")
            //{
            //    validacion_centrolaboral = false;
            //    errores.Add("Debe ingresar el <strong>Centro de trabajo</strong> en el CRM. Dato Obligatorio.");
            //}
            //else
            //{
            //    validacion_centrolaboral = false;
            //    errores.Add("Debe ingresar el <strong>Centro de trabajo</strong> en el CRM. Dato Obligatorio.");
            //}

            //if (cont[0].ingreso.Trim().Length > 0 && cont[0].ingreso.ToString().ToUpper() != "NULL")
            //{
            //    validacion_ram = true;
            //}
            //else if (cont[0].ingreso.ToString().ToUpper() == "NULL")
            //{
            //    validacion_ram = false;
            //    errores.Add("Debe ingresar el <strong>Ingreso Neto Mensual</strong> en el CRM. Dato Obligatorio.");
            //}
            //else
            //{
            //    validacion_ram = false;
            //    errores.Add("Debe ingresar el <strong>Ingreso Neto Mensual</strong> en el CRM. Dato Obligatorio.");
            //}

            //if (cont[0].jobtitle.Trim().Length > 0 && cont[0].jobtitle.ToString().ToUpper() != "NULL")
            //{
            //    validacion_jobtitle = true;
            //}
            //else if (cont[0].jobtitle.ToString().ToUpper() == "NULL")
            //{
            //    validacion_jobtitle = false;
            //    errores.Add("Debe ingresar el <strong>Cargo</strong> en el CRM. Dato Obligatorio.");
            //}
            //else
            //{
            //    validacion_jobtitle = false;
            //    errores.Add("Debe ingresar el <strong>Cargo</strong> en el CRM. Dato Obligatorio.");
            //}

            //string CUSPP = Session["CUSPP_RP"].ToString();

            //List<Direccion> lstDireccion = servicioCotizador.ListarDireccion(CUSPP);
            //if (lstDireccion.Count == 0)
            //{
            //    validacion_direccion = false;
            //    errores.Add("El Afiliado no cuenta con dirección.");
            //}

            //bool validacion = true;

            //validacion = validacion_centrolaboral & validacion_ram & validacion_jobtitle & validacion_direccion;

            //if (!validacion)
            //{
            //    ClientScript.RegisterStartupScript(GetType(), "mensaje", "MostrarModal('" + Utilitarios.FormatearError(errores) + "');", true);
            //}
            //else
            //{
            //    //ModSolAgregarArchivos.Visible = firmado;
            //    EmisionCotizacion(lstDatossolicitud);
            //    OrigenFondos(datossolicitud);
            //    PEP(datossolicitud);
            //}
        }

        //<FINGTI_7012>



        [WebMethod]
        public static Respuesta EnviarEvaluacion(string tokenUsuario, string num_solicitud, string id_archivos)
        {
            //Contratante[] cont = null;
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusCerrar))
                        {
                            //bool validacion_centrolaboral = false;
                            //bool validacion_ram = false;
                            //bool validacion_jobtitle = false;
                            //bool validacion_direccion = true;
                            //bool validacion_contratante = true;
                            string usuario = HttpContext.Current.Session["Usuario"].ToString();

                            respuesta.Estado = Constante.COD_OK;


                            List<DatosSol> lstDatossolicitud = new List<DatosSol>();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            lstDatossolicitud = servicioCotizador.ObtenerDatosporSolicitud(num_solicitud);
                            DatosSol datosSol = lstDatossolicitud.Find(tit => tit.cod_parentesco_beneficiario == Enums.Parentesco.Afiliado.StringValue());

                            if (datosSol.CodigoEstado == 4 || datosSol.CodigoEstadoPlaft == 4)
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = "La solicitud ya se encuentra en evaluación.";
                                return respuesta;
                            }

                            FirmaDigital firma = servicioCotizador.ObtenerFirmaDigital(num_solicitud, 1, HttpContext.Current.Session["Usuario"].ToString());
                            if (firma.ind_consentimiento != "S")
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = "El cliente aún no ha firmado la solicitud, todavía no puede enviarse a evaluación.";
                                return respuesta;
                            }

                            Propuesta propuesta = new Propuesta();

                            //ParametroGeneral[] lstProfesion = (ParametroGeneral[])HttpContext.Current.Session["ListadoProfesion"];
                            //ParametroGeneral[] lstNacionalidad = (ParametroGeneral[])HttpContext.Current.Session["ListadoNacionalidad"];

                            //JArray lstDepartamento = (JArray)HttpContext.Current.Session["ListadoDepartamento"];
                            var departamentos = (List<Departamento>)HttpContext.Current.Session["ListadoDepartamento"];
                            JArray lstDepartamento = JArray.FromObject(departamentos);

                            string codigoProfesion = string.Empty;
                            string codigoNacion = string.Empty;
                            string descripcionResidencia = string.Empty;

                            //codigoNacion = lstNacionalidad.Where(pr => pr.Codigo.ToUpper() == datosSol.Nacionalidad_beneficiario_cod.ToUpper()).Select(prd => prd.Descripcion).FirstOrDefault();
                            codigoNacion = datosSol.Nacionalidad_beneficiario_cod;
                            //codigoProfesion = lstProfesion.Where(pr => pr.Codigo.ToUpper() == datosSol.Profesion_beneficiario_cod.ToUpper()).Select(prd => prd.Descripcion).FirstOrDefault();
                            codigoProfesion = datosSol.Profesion_beneficiario_cod;

                            descripcionResidencia = lstDepartamento.Where(pr => pr["id_departamento"].ToString().ToUpper() == datosSol.Residencia_beneficiario_cod.ToUpper()).Select(prd => prd["gls_departamento"].ToString()).FirstOrDefault();

                            propuesta.propuesta = datosSol.num_solicitud;
                            propuesta.prima_anualizada = datosSol.val_mto_cta_individual.Value;//
                                                                                               //propuesta.producto = ConfigurationManager.AppSettings["ProductoPlaftRPP"]; //"750101";
                            propuesta.producto = Enums.TipoProductoRamo.RPP.StringValue();
                            propuesta.moneda = (datosSol.cod_moneda == Enums.Moneda.Soles.StringValue() || datosSol.cod_moneda == Enums.Moneda.SolesAjustados.StringValue()) ? "1" : "2";

                            //string cod_documento_identidad = servicioCotizador.ObtenerTipoIdentificacion(datosSol.cod_tipo_documento_afiliado, "", "")[0].Nombre;
                            propuesta.contratante_tipo_documento = datosSol.cod_tipo_documento_afiliado;
                            propuesta.contratante_documento = datosSol.rut_persona_afiliado;
                            propuesta.contratante_actividad_economica = datosSol.actividadEconomica.ToString();
                            propuesta.contratante_profesion = codigoProfesion;
                            propuesta.contratante_sujeto_obligado = (datosSol.ind_SujetoObligado_afiliado == "S") ? "CLICSO-ESSO" : "CLICSO-NOSO";
                            propuesta.contratante_residencia = descripcionResidencia;
                            propuesta.contratante_nacionalidad = codigoNacion;
                            propuesta.contratante_pep = (datosSol.ind_PEP_afiliado == "S") ? "1" : "0";
                            propuesta.contratante_nombre1 = datosSol.nom_nombre_afiliado;
                            propuesta.contratante_nombre2 = "";
                            propuesta.contratante_nombre3 = "";
                            propuesta.contratante_apellido_paterno = datosSol.ape_paterno_afiliado;
                            propuesta.contratante_apellido_materno = datosSol.ape_materno_afiliado;
                            propuesta.contratante_razon_social = "";

                            propuesta.asegurado_tipo_documento = datosSol.cod_tipo_documento_afiliado;
                            propuesta.asegurado_documento = datosSol.rut_persona_afiliado;
                            propuesta.asegurado_actividad_economica = datosSol.actividadEconomica.ToString();
                            propuesta.asegurado_profesion = codigoProfesion;
                            propuesta.asegurado_sujeto_obligado = (datosSol.ind_SujetoObligado_afiliado == "S") ? "CLICSO-ESSO" : "CLICSO-NOSO";
                            propuesta.asegurado_residencia = descripcionResidencia;
                            propuesta.asegurado_nacionalidad = codigoNacion;
                            propuesta.asegurado_pep = (datosSol.ind_PEP_afiliado == "S") ? "1" : "0";
                            propuesta.asegurado_nombre1 = datosSol.nom_nombre_afiliado;
                            propuesta.asegurado_nombre2 = "";
                            propuesta.asegurado_nombre3 = "";
                            propuesta.asegurado_apellido_paterno = datosSol.ape_paterno_afiliado;
                            propuesta.asegurado_apellido_materno = datosSol.ape_materno_afiliado;
                            propuesta.asegurado_razon_social = "";
                            propuesta.usuario = (string)HttpContext.Current.Session["Usuario"];

                            propuesta.CodigoEstado = datosSol.CodigoEstado.ToString();
                            propuesta.CodigoEstadoPlaft = datosSol.CodigoEstadoPlaft.ToString();
                            propuesta.CodCanalDistribucion = datosSol.cod_canal_distribucion;

                            if (HttpContext.Current.Session["ListaAgentes"] != null)
                            {
                                List<Agente> lstAgenteCorreo = new List<Agente>();
                                List<Agente> lstAgente = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                                BEUsuario datosUsuario = null;

                                /*Agente*/
                                Agente agente = lstAgente.Find(p => p.Id == datosSol.num_agente);

                                try
                                {
                                    servicioAzman = LocalizadorProxy.ObtenerServicioSeguridad();

                                    datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                            ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                            ConfigurationManager.AppSettings["DominioRed"],
                                            agente.Usuario);
                                }
                                catch (Exception ex)
                                {
                                    log.Warn("Datos del Agente no encontrado");
                                    datosUsuario = null;
                                }
                                
                                if (datosUsuario != null)
                                {
                                    if (datosUsuario.Correo != "")
                                    {
                                        //agente.gls_email = datosUsuario.Correo;
                                        agente.Usuario = datosUsuario.Matricula;
                                        lstAgenteCorreo.Add(agente);
                                    }
                                }

                                /*Supervisor*/
                                if (agente.IdPadre == null)
                                    agente.IdPadre = "";

                                Agente supervidor = lstAgente.Find(p => p.Id == agente.IdPadre);

                                if (supervidor != null)
                                {
                                    if (supervidor.Usuario != null)
                                    {
                                        try
                                        {
                                            servicioAzman = LocalizadorProxy.ObtenerServicioSeguridad();

                                            datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                                    ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                                    ConfigurationManager.AppSettings["DominioRed"],
                                                    supervidor.Usuario);
                                        }
                                        catch (Exception ex)
                                        {
                                            log.Warn("Datos del Supervisor no encontrado");
                                            datosUsuario = null;
                                        }

                                        if (datosUsuario != null)
                                        {
                                            if (datosUsuario.Correo != "")
                                            {
                                                //supervidor.gls_email = datosUsuario.Correo;
                                                supervidor.Usuario = datosUsuario.Matricula;
                                                lstAgenteCorreo.Add(supervidor);
                                            }
                                        }

                                    }
                                    propuesta.Agentes = lstAgenteCorreo;
                                }
                            }

                            string url =
                            HttpContext.Current.Request.Url.Scheme + "://" +
                            HttpContext.Current.Request.Url.Authority +
                            HttpContext.Current.Request.ApplicationPath +
                            (HttpContext.Current.Request.ApplicationPath == "/" ? String.Empty : "/") +
                            "RentaPrivadaPlus/ListadoEvaluacion.aspx";

                            propuesta.ArchivosExistentes = id_archivos;

                            JsonPropuesta propuestaResult = servicioCotizador.ObtenerCalificacion(propuesta, url);

                            if (propuestaResult.records.codigo != -1)
                            {
                                respuesta.Estado = Constante.COD_OK;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = "Propuesta enviada a Evaluación Correctamente.";
                                //ClientScript.RegisterStartupScript(GetType(), "mensaje", "MostrarModalOk('Propuesta enviada a Evaluación de Plaft Correctamente.');", true);

                                //Envio de manera Asincrono
                                var tareaParalela = new System.Threading.Tasks.Task(() =>
                                {
                                    EstudioNecesidadAPI estudioNecesidadAPI = new EstudioNecesidadAPI();
                                    SolicitudEdNAPI solicitud = new SolicitudEdNAPI();
                                    List<CotizacionEdNAPI> cotizaciones_edn = new List<CotizacionEdNAPI>();
                                    CotizacionEdNAPI cotizacion;
                                    BeneficiarioEdNAPI beneficiario;

                                    SolicitudRPPlus solicitudRPPlus = servicioCotizador.ObtenerDatosSolicitudRPPlus(datosSol.num_solicitud);

                                    solicitud.num_solicitud = solicitudRPPlus.Id;
                                    solicitud.fec_solicitud = solicitudRPPlus.FechaSolicitud.Value;
                                    solicitud.val_mto_cta_individual = solicitudRPPlus.PrimaUnica;
                                    solicitud.cod_moneda_cta_indiv = solicitudRPPlus.MonedaPrimaUnica.Id;

                                    estudioNecesidadAPI.solicitud = solicitud;

                                    var listaCotiza = solicitudRPPlus.Cotizaciones;

                                    if (listaCotiza != null)
                                    {
                                        foreach (var cot in listaCotiza)
                                        {
                                            cotizacion = new CotizacionEdNAPI();
                                            cotizacion.num_correlativo = (int)cot.Correlativo;
                                            cotizacion.cod_tipo_temporalidad = solicitudRPPlus.Temporalidad.Id;
                                            cotizacion.val_per_diferido = cot.PeriodoDiferido;

                                            cotizaciones_edn.Add(cotizacion);
                                        }
                                        
                                        estudioNecesidadAPI.cotizaciones = new List<CotizacionEdNAPI>();
                                        estudioNecesidadAPI.cotizaciones = cotizaciones_edn;
                                    }

                                    estudioNecesidadAPI.beneficiarios = new List<BeneficiarioEdNAPI>();
                                    foreach (var benefi in solicitudRPPlus.Beneficiarios)
                                    {
                                        beneficiario = new BeneficiarioEdNAPI();
                                        beneficiario.ape_paterno = benefi.ApellidoPaterno;
                                        beneficiario.ape_materno = benefi.ApellidoMaterno;
                                        beneficiario.nom_persona = benefi.Nombre;

                                        switch (benefi.Identificacion.IdTipo)
                                        {
                                            case "D":
                                                beneficiario.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                                break;
                                            case "E":
                                                beneficiario.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.CE.StringValue();
                                                break;
                                            case "P":
                                                beneficiario.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.PAS.StringValue();
                                                break;
                                            default:
                                                beneficiario.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                                break;
                                        }

                                        beneficiario.num_identificacion = benefi.Identificacion.Numero;
                                        beneficiario.cod_parentezco = benefi.Parentesco.Id;

                                        estudioNecesidadAPI.beneficiarios.Add(beneficiario);
                                    }

                                    var respuestaEdN = Utilitario.GenerarEstudioNecesidades(estudioNecesidadAPI, usuario);
                                    log.Info("respuestaEdN: " + respuestaEdN.Estado + " " + respuestaEdN.Mensaje);
                                });
                                tareaParalela.Start();
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = propuestaResult.records.descripcion;
                                //ClientScript.RegisterStartupScript(GetType(), "mensaje", "MostrarModal('" + propuestaResult.records.descripcion + "');", true);
                            }

                            //}
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusCerrar.StringValue()));
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
        public static Respuesta ValidarAgregarArchivo(string tokenUsuario)
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
                            //bool validacion_centrolaboral = false;
                            //bool validacion_ram = false;
                            //bool validacion_jobtitle = false;
                            //bool validacion_direccion = true;
                            //bool validacion_contratante = true;

                            respuesta.Estado = Constante.COD_OK;


                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusCerrar.StringValue()));
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

    }
}