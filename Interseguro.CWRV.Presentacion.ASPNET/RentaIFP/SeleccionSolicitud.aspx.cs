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
using System.Globalization;
using System.IO;
using System.Threading;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using log4net;
using iTextSharp.text.pdf;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloSeguridad;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
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
                    string origen = Request.QueryString["origen"];
                    if (origen != "cotizador")
                    {
                        if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionesIFP))
                        {
                            if (!IsPostBack)
                            {

                                string qs_source = Request.QueryString["source"];
                                string qs_solicitud = Request.QueryString["solicitud"];

                                log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));

                                if (qs_source == "correo")
                                {
                                    Session["ModSolModo"] = "CERRAR";

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudRPPlus solicitudRRP = servicioCotizador.ObtenerEstadoSolicitudRPPlus(qs_solicitud);

                                    Session["FecSoltud"] = Convert.ToDateTime(solicitudRRP.FechaSolicitud).ToString("dd/MM/yyyy");
                                    Session["EstadoSoltud"] = solicitudRRP.CodigoEstado;
                                    Session["CUSPP_PLUS"] = solicitudRRP.Afiliado.CUSPP;
                                    //Session["Vendedor"] = solicitudRRP.Afiliado.Agente.Id;
                                    Session["CUSPP_RP"] = solicitudRRP.Afiliado.CUSPP.ToString();

                                    if (Session["ListadoDepartamento"] == null)
                                    {
                                        JArray listaDepartamentos = new JArray();
                                        string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                                        var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
                                        string usuario = HttpContext.Current.Session["Usuario"].ToString();
                                        urlDepartamentos = string.Format(urlDepartamentos, usuario);
                                        listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, usuario, listaDepartamentos);
                                        Session["ListadoDepartamento"] = listaDepartamentos.ToObject<List<Departamento>>();
                                    }

                                    Session["Consentimiento"] = true;

                                    Session["idSolicitud"] = solicitudRRP.Id;
                                    Session["fecCotizacion"] = Session["FecSoltud"];

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(Session["idSolicitud"].ToString());

                                    Session["DNIIFP"] = solicitud.Afiliado.NumeroIdentificacion;
                                    Session["NombresIFP"] = solicitud.Afiliado.Nombre.Trim();
                                    Session["ApellidosIFP"] = solicitud.Afiliado.ApellidoPaterno.Trim() + " " + solicitud.Afiliado.ApellidoMaterno.Trim();
                                }

                                CargarInformacionInicialPantalla();

                                if (Session["ModSolModo"].ToString() != null)
                                {
                                    HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();
                                    //HAFP_RP.Value = Session["AFP_RP"].ToString();

                                    LabModSolAviso.Visible = false;

                                    if (Session["ModSolModo"].ToString() == "CERRAR")
                                    {
                                        //Obtener la información desde base de datos
                                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                        SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(Session["idSolicitud"].ToString());

                                        solicitud.Beneficiarios = solicitud.Beneficiarios.FindAll(ben => ben.IdTipoPeriodoBeneficiario.ToString() == Enums.TipoPeriodoBeneficiario.Garantizada.StringValue() || ben.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

                                        HSolicitudSerializado.Value = JsonConvert.SerializeObject(solicitud);

                                        List<List<Parametro>> listaParametro = servicioCotizador.ObtenerComboboxIFP();
                                        Session["ComboIFPMoneda"] = listaParametro[(int)Enums.ComboboxIFP.Moneda];

                                        Session["ComboTramoEscalonada"] = listaParametro[(int)Enums.ComboboxIFP.PorcentajeEscalon];

                                        HNroSolicitud.Value = solicitud.Id;
                                        ModSolNroSolicitud_IFP.Text = solicitud.Id;
                                        ModSolPrimaUnica_IFP.Text = solicitud.PrimaUnica.ToString("#,##0.00");

                                        ModSolMonedaPrimaUnica_IFP.SelectedIndex = ModSolMonedaPrimaUnica_IFP.Items.IndexOf(ModSolMonedaPrimaUnica_IFP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                        ModSolMonedaPrimaUnicaText_IFP.Text = ModSolMonedaPrimaUnica_IFP.SelectedItem.Text;

                                        ModSolFechaCotizacion_IFP.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                        ModSolFechaDevengue_IFP.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                        ModSolFechaVigencia_IFP.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");

                                        Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                        Session["RP_Cotizaciones"] = solicitud.Cotizaciones;
                                        Session["RP_CoberturasAdicionales"] = solicitud.CoberturasAdicionales;

                                        var pep = solicitud.Beneficiarios.Find(p => p.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue()).ind_PEP;
                                        HPEP.Value = pep ? "S" : "N";

                                        var cotizacionSeleccionada = solicitud.Cotizaciones.FindAll(p => (p.EstadoCotizacion == "04" || p.EstadoCotizacion == "05") && p.IndSeleccionada == "S");

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

                                        //Bloqueando controles
                                        ModSolMonedaPrimaUnica_IFP.Enabled = false;
                                        ModSolFechaCotizacion_IFP.Enabled = false;
                                        ModSolFechaDevengue_IFP.Enabled = false;
                                        ModSolFechaVigencia_IFP.Enabled = false;

                                        HEstado.Value = solicitud.CodigoEstado.ToString();
                                        HEstadoPlaft.Value = solicitud.CodigoEstadoPlaft.ToString();

                                        LabMensaje.Text = solicitud.EstadoSolicitud.ToString(); //"Solicitud Seleccionada";

                                        bool firmado = false;
                                        string usuario = (string)HttpContext.Current.Session["Usuario"];
                                        FirmaDigital firma = servicioCotizador.ObtenerFirmaDigital(Session["idSolicitud"].ToString(), 1, Session["usuario"].ToString());
                                        if (firma != null)
                                        {
                                            HFirmado.Value = firma.ind_consentimiento;
                                            firmado = firma.ind_consentimiento == "S";
                                        }

                                        switch (solicitud.CodigoEstado.ToString())
                                        {
                                            case "0":
                                                LabModSolAviso.CssClass = "grilla_info";
                                                ClientScript.RegisterStartupScript(GetType(), "-", "$('#ModImprimirDocumentosIFP').hide();", true);
                                                break;
                                            case "1":

                                                LabModSolAviso.CssClass = "grilla_info_verde";
                                                LabMensaje.ForeColor = System.Drawing.Color.Green;
                                                LabModSolAviso.Visible = true;
                                                ModSolEnviarEvaluacion.Visible = true;
                                                ModSolAgregarArchivos.Visible = firmado;

                                                if (Request.QueryString["seleccione"] != null)
                                                {
                                                    if (Request.QueryString["seleccione"].ToString() == "0")
                                                    {
                                                        ModSolAgregarArchivos.Visible = false;
                                                    }
                                                }

                                                break;
                                            case "2":

                                                LabModSolAviso.CssClass = "grilla_info";
                                                LabModSolAviso.Visible = true;

                                                break;
                                            case "3":

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

                                                if (firma != null && firma.ind_consentimiento == "S")
                                                {
                                                    ModSolEnviarEvaluacion.Visible = true;
                                                }
                                                if (Utilitarios.EsRolComercial(Session["RolAzman"].ToString()) && firma != null && firma.ind_consentimiento != "R")
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

                                        ModSolEnviarEvaluacion.Visible = true;
                                        ModSolEnviarEvaluacion.Enabled = false;
                                        ModSolEnviarEvaluacion.CssClass = "botonDeshabilitado gris gris_sharp";

                                        if (solicitud.CodigoEstado.ToString() == "5" && (
                                            solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Observado.StringValue() ||
                                            solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Aprobado.StringValue()))
                                        {
                                            ModSolEnviarEvaluacion.Enabled = true;
                                            ModSolEnviarEvaluacion.CssClass = "botonDeshabilitado gris gris_sharp";
                                        }
                                        else if (solicitud.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Observado.StringValue() && (
                                           solicitud.CodigoEstado.ToString() == "5" ||
                                           solicitud.CodigoEstado.ToString() == "6"))
                                        {
                                            ModSolEnviarEvaluacion.Enabled = true;
                                            ModSolEnviarEvaluacion.CssClass = "botonDeshabilitado gris gris_sharp";
                                        }
                                        else if (solicitud.CodigoEstadoPlaft.ToString() == "-1" &&
                                           solicitud.CodigoEstado.ToString() == "6")
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
                                    Response.Redirect("Cotizador.aspx");
                                }
                            }
                            else
                            {
                                if (ModSolPrimaUnica_IFP.Text != String.Empty) ModSolPrimaUnica_IFP.Text = Convert.ToDouble(ModSolPrimaUnica_IFP.Text, new CultureInfo("es-PE")).ToString();
                            }
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.MenuCotizador.StringValue()));
                            Response.Redirect("~/Error/Permisos.aspx");
                        }
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

            CargarCombobox(ModSolMonedaPrimaUnica_IFP, listaCombobox[(int)Enums.CategoriaCombobox.MonedaRentaPrivada]);

            Session["ComboSexo"] = listaCombobox[(int)Enums.CategoriaCombobox.Sexo];

            ModSolDNI_IFP.Text = Session["DNIIFP"].ToString();
            ModSolNombres_IFP.Text = Session["NombresIFP"].ToString();
            ModSolApellidos_IFP.Text = Session["ApellidosIFP"].ToString();
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
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
                control.Items.Add(new ListItem(string.Format("{0:#,##0.00}", item.Valor), item.Valor.ToString()));
            }
        }

        // Modificar cuando se reanude P#
        [WebMethod]
        public static string CargarTablaCotizaciones(List<CotizacionIFP> cotizaciones, string moneda, string plan, List<CoberturaAdicional> coberturasAdicionales)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Page pagina = new Page();

                    dynamic control;

                    // TablaCotizacionesPlan1IFPCierre
                    control = (dynamic)pagina.LoadControl("~/Controles/TablaCotizaciones" + plan + "IFPCierre.ascx");

                    if (plan == Enums.Planes.PLAN1.StringValue())
                    {
                        control = (TablaCotizacionesPlan1IFPCierre)control;
                    }
                    else if (plan == Enums.Planes.PLAN2.StringValue())
                    {
                        control = (TablaCotizacionesPlan2IFPCierre)control;
                    }
                    else if (plan == Enums.Planes.PLAN3.StringValue())
                    {
                        control = (TablaCotizacionesPlan3IFPCierre)control;
                        control.CoberturasAdicionales = coberturasAdicionales;

                        //control.Conyuge = false;

                        //List<GrupoFamiliar> beneficiarios = (List<GrupoFamiliar>)HttpContext.Current.Session["RP_Beneficiarios"];
                        //if (beneficiarios.Count == 2)
                        //{
                        //    if (beneficiarios.Find(ben => ben.Parentesco.Id == Enums.Parentesco.Conyuge.StringValue()) != null)
                        //    {
                        //        control.Conyuge = true;
                        //    }                            
                        //}

                        //if (!control.Conyuge)
                        //{
                            //foreach (var cotizacion in cotizaciones)
                            //{
                            //    cotizacion.ValPjeConyuge = 0.00;
                            //}
                        //}                        

                    }

                    control.CotizacionesIFP = cotizaciones.FindAll(c => c.Plan.Id == plan);
                    control.Moneda = moneda;

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

                    pagina.Controls.Add(control);

                    HttpContext.Current.Session["idMonedaFondo"] = moneda;

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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        // Modificar cuando se reanude P#
        [WebMethod]
        public static string CargarTablaCotizacionesP12(List<CotizacionIFP> cotizaciones, string moneda, string plan)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Page pagina = new Page();

                    dynamic control;

                    // TablaCotizacionesPlan1IFPCierre
                    control = (dynamic)pagina.LoadControl("~/Controles/TablaCotizaciones" + plan + "IFPCierre.ascx");

                    if (plan == Enums.Planes.PLAN1.StringValue())
                    {
                        control = (TablaCotizacionesPlan1IFPCierre)control;
                    }
                    else if (plan == Enums.Planes.PLAN2.StringValue())
                    {
                        control = (TablaCotizacionesPlan2IFPCierre)control;
                    }
                    else if (plan == Enums.Planes.PLAN3.StringValue())
                    {
                        control = (TablaCotizacionesPlan3IFPCierre)control;
                    }

                    control.CotizacionesIFP = cotizaciones.FindAll(c => c.Plan.Id == plan);
                    control.Moneda = moneda;

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

                    pagina.Controls.Add(control);

                    HttpContext.Current.Session["idMonedaFondo"] = moneda;

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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Respuesta CerrarCotizacionIFP(string tokenUsuario, string num_solicitud, string num_correlativo, Solicitud objSolicitud)
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
                            List<GrupoFamiliar> lstGrupoFamiliar = objSolicitud.Beneficiarios.FindAll(b => b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue());

                            if (ValidarCotizacion(num_solicitud, num_correlativo, lstGrupoFamiliar, errores, controles))
                            {
                                string usuario = (string)HttpContext.Current.Session["Usuario"];

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.CerrarCotizacionPlus(num_solicitud, Convert.ToInt32(num_correlativo), usuario, lstGrupoFamiliar);
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
                                Enums.OpcionesSistema.SolicitudPlusActualizar.StringValue()));
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


        private static bool ValidarCotizacion(string num_solicitud, string num_cotizacion, List<GrupoFamiliar> lstGrupoFamiliar, List<string> errores, List<string> controles)
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

            // Grupo Familiar PjeTotal
            bool pjeRenta = true;
            if (lstGrupoFamiliar != null)
            {
                if (lstGrupoFamiliar.Count > 0)
                {
                    double totalPje = 0;
                    foreach (var grupoFamiliar in lstGrupoFamiliar)
                    {
                        totalPje += grupoFamiliar.ValPjeRenta;
                    }
                    if (totalPje != 100)
                    {
                        errores.Add("La Suma de <strong>Porcentaje de Renta </strong>de beneficiarios debe ser igual a 100%. Dato Obligatorio.");
                        pjeRenta = false;
                    }
                }

            }

            // Grupo Familiar Conyuge-Padre-Madre
            bool conyuge = true;
            bool padre = true;
            bool madre = true;

            if (lstGrupoFamiliar != null)
            {
                if (lstGrupoFamiliar.Count > 0)
                {
                    List<GrupoFamiliar> lstGrupoFamiliarConyuge = lstGrupoFamiliar.FindAll(p => p.Parentesco.Id == Enums.Parentesco.Conyuge.StringValue());
                    if (lstGrupoFamiliarConyuge.Count > 1)
                    {
                        errores.Add("Sólo puede haber un Cónyuge. Verifique.");
                        conyuge = false;
                    }

                    List<GrupoFamiliar> lstGrupoFamiliarPadre = lstGrupoFamiliar.FindAll(p => p.Parentesco.Id == Enums.Parentesco.Padre.StringValue() && p.Sexo.ToString() == Enums.Sexo.Masculino.StringValue());
                    if (lstGrupoFamiliarPadre.Count > 1)
                    {
                        errores.Add("Sólo puede haber un Padre. Verifique.");
                        padre = false;
                    }

                    List<GrupoFamiliar> lstGrupoFamiliarMadre = lstGrupoFamiliar.FindAll(p => p.Parentesco.Id == Enums.Parentesco.Padre.StringValue() && p.Sexo.ToString() == Enums.Sexo.Femenino.StringValue());
                    if (lstGrupoFamiliarMadre.Count > 1)
                    {
                        errores.Add("Sólo puede haber una Madre. Verifique.");
                        madre = false;
                    }

                }
            }

            esCorrecto = solicitud & cotizacion & pjeRenta & conyuge & padre & madre;

            return esCorrecto;
        }

        [WebMethod]
        public static Respuesta EnviarEvaluacion(string tokenUsuario, string num_solicitud, string id_archivos)
        {
            //Contratante[] cont = null;

            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionesIFP))
                        {
                            string usuario = HttpContext.Current.Session["Usuario"].ToString();
                            respuesta.Estado = Constante.COD_OK;

                            List<DatosSol> lstDatossolicitud = new List<DatosSol>();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();

                            lstDatossolicitud = servicioCotizador.ObtenerDatosporSolicitudIFP(num_solicitud);
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
                            var departamentos = new List<Departamento>();
                            JArray listaDepartamentos = new JArray();
                            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                            var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
                            string usuarioCwrv = HttpContext.Current.Session["Usuario"].ToString();
                            urlDepartamentos = string.Format(urlDepartamentos, usuarioCwrv);
                            listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, usuarioCwrv, listaDepartamentos);
                            departamentos = listaDepartamentos.ToObject<List<Departamento>>();
                            JArray lstDepartamento = JArray.FromObject(departamentos);
                            
                            string codigoProfesion = string.Empty;
                            string codigoNacion = string.Empty;
                            string descripcionResidencia = string.Empty;

                            codigoNacion = datosSol.Nacionalidad_beneficiario_cod;
                            codigoProfesion = datosSol.Profesion_beneficiario_cod;

                            descripcionResidencia = lstDepartamento.Where(pr => pr["id_departamento"].ToString().ToUpper() == datosSol.Residencia_beneficiario_cod.ToUpper()).Select(prd => prd["gls_departamento"].ToString()).FirstOrDefault();

                            propuesta.propuesta = datosSol.num_solicitud;
                            propuesta.prima_anualizada = datosSol.val_mto_cta_individual.Value;//
                                                                                               //propuesta.producto = ConfigurationManager.AppSettings["ProductoPlaftRPP"]; //"750101";
                            propuesta.producto = Enums.TipoProductoRamo.IFP.StringValue();
                            propuesta.moneda = (datosSol.cod_moneda == Enums.Moneda.Soles.StringValue() || datosSol.cod_moneda == Enums.Moneda.SolesAjustados.StringValue()) ? "1" : "2";

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

                            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || datosSol.OrigenCotizacion == Enums.OrigenCotizacion.Inteligo.StringValue())
                            {
                                string str_destinatario_evaluacion_inteligo = ConfigurationManager.AppSettings["destinatario_envio_evaluacion_inteligo"];
                                string[] lista_destinatario_evaluacion_inteligo = str_destinatario_evaluacion_inteligo.Split(',');
                                List<Agente> lstAgenteInteligoCorreo = new List<Agente>();

                                foreach (string usuario_inteligo in lista_destinatario_evaluacion_inteligo)
                                {
                                    Agente datos_agente_inteligo = new Agente();
                                    datos_agente_inteligo.Usuario = usuario_inteligo;
                                    lstAgenteInteligoCorreo.Add(datos_agente_inteligo);
                                }

                                propuesta.Agentes = lstAgenteInteligoCorreo;
                            }
                            else
                            {
                                if (HttpContext.Current.Session["ListaAgentes"] != null)
                                {

                                    List<Agente> lstAgente = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                                    List<Agente> lstAgenteCorreo = new List<Agente>();
                                    BEUsuario datosUsuario = null;

                                    /* Agente */
                                    Agente agente = lstAgente.Find(p => p.Id == datosSol.num_agente);

                                    try
                                    {
                                            servicioAzman = LocalizadorProxy.ObtenerServicioSeguridad();

                                            datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                            ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                            ConfigurationManager.AppSettings["DominioRed"],
                                            agente.Usuario);

                                        if (datosUsuario != null)
                                        {
                                            if (datosUsuario.Correo != "")
                                            {
                                                agente.Usuario = datosUsuario.Matricula;
                                                lstAgenteCorreo.Add(agente);
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        log.Warn("No existe el correo para el agente: " + agente.Usuario);
                                    }

                                    /* Supervisor */
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

                                                if (datosUsuario != null)
                                                {
                                                    if (datosUsuario.Correo != "")
                                                    {
                                                        supervidor.Usuario = datosUsuario.Matricula;
                                                        lstAgenteCorreo.Add(supervidor);
                                                    }
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                log.Warn("No existe el correo para el agente: " + supervidor.Usuario);
                                            }
                                        }
                                        propuesta.Agentes = lstAgenteCorreo;
                                    }
                                }
                            }

                            string url =
                            HttpContext.Current.Request.Url.Scheme + "://" +
                            HttpContext.Current.Request.Url.Authority +
                            HttpContext.Current.Request.ApplicationPath +
                            (HttpContext.Current.Request.ApplicationPath == "/" ? string.Empty : "/") +
                            "RentaPrivadaPlus/ListadoEvaluacion.aspx";

                            propuesta.ArchivosExistentes = id_archivos;

                            JsonPropuesta propuestaResult = servicioCotizador.ObtenerCalificacion(propuesta, url);

                            if (propuestaResult.records.codigo != -1)
                            {
                                respuesta.Estado = Constante.COD_OK;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = "Propuesta enviada a Evaluación Correctamente.";

                                //Envio de manera Asincrono
                                var tareaParalela = new System.Threading.Tasks.Task(() =>
                                {
                                    EstudioNecesidadAPI estudioNecesidadAPI = new EstudioNecesidadAPI();
                                    SolicitudEdNAPI solicitud = new SolicitudEdNAPI();
                                    List<CotizacionEdNAPI> cotizaciones_edn = new List<CotizacionEdNAPI>();
                                    CotizacionEdNAPI cotizacion;
                                    BeneficiarioEdNAPI beneficiario;

                                    SolicitudIFP solicitudIFP = servicioCotizador.ObtenerDatosSolicitudIFP(datosSol.num_solicitud);

                                    solicitud.num_solicitud = solicitudIFP.Id;
                                    solicitud.fec_solicitud = solicitudIFP.FechaSolicitud.Value;
                                    solicitud.val_mto_cta_individual = solicitudIFP.PrimaUnica;
                                    solicitud.cod_moneda_cta_indiv = solicitudIFP.MonedaPrimaUnica.Id;

                                    estudioNecesidadAPI.solicitud = solicitud;

                                    var listaCotiza = solicitudIFP.Cotizaciones;

                                    if (listaCotiza != null)
                                    {
                                        foreach (var cot in listaCotiza)
                                        {
                                            cotizacion = new CotizacionEdNAPI();
                                            cotizacion.num_correlativo = (int)cot.Correlativo;
                                            cotizacion.cod_tipo_temporalidad = cot.Temporalidad.Anhos.ToString();
                                            cotizacion.val_per_diferido = cot.ValPerDiferido;

                                            cotizaciones_edn.Add(cotizacion);
                                        }
                                        
                                        estudioNecesidadAPI.cotizaciones = new List<CotizacionEdNAPI>();
                                        estudioNecesidadAPI.cotizaciones = cotizaciones_edn;
                                    }

                                    estudioNecesidadAPI.beneficiarios = new List<BeneficiarioEdNAPI>();

                                    log.Info("Cantidad de beneficiarios(enviar evaluación): " + solicitudIFP.Beneficiarios.Count);
                                    var Beneficiarios = solicitudIFP.Beneficiarios.GroupBy(be => new { be.Identificacion.IdTipo, be.Identificacion.Numero } ).Select(s => s.First()).ToList();
                                    log.Info("Cantidad de beneficiarios(enviar evaluación): " + Beneficiarios.Count);

                                    foreach (var benefi in Beneficiarios)
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
                            }

                            //}
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

        [WebMethod]
        public static Respuesta ValidarAgregarArchivo(string tokenUsuario, string num_solicitud)
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
                            respuesta.Estado = Constante.COD_OK;
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

        [WebMethod]
        public static Respuesta PreseleccionarCotizacion(string tokenUsuario, string num_solicitud, string num_correlativo)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    log.Debug("INICIO SeleccionSolicitud/PreseleccionarCotizacion");
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.PreseleccionarCotizacion(num_solicitud, num_correlativo, (string)HttpContext.Current.Session["Usuario"]);
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    log.Debug("FIN SeleccionSolicitud/PreseleccionarCotizacion");
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace));
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });

                }
                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta ImprimirDocumentosIFP(string tokenUsuario, string numSolicitud, string indPEP)
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
                            log.Info("Accediendo a las Key necesarias");

                            string usuario = HttpContext.Current.Session["Usuario"].ToString();

                            log.Info("Eliminar formatos generados");
                            string rutaCarpeta = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\Plantilla\\IFP\\";
                            string archivoSolicitud = "Solicitud.pdf";
                            string archivoOrigenFondos = "OrigenFondos.pdf";
                            string archivoConstanciaAbono = "ConstanciaAbono.pdf";
                            string archivoPersonaExpuestaPoliticamente = "PersonaExpuestaPoliticamente.pdf";
                            string archivoDetalleCotizacion = "DetalleCotizacion.pdf";

                            if (File.Exists(rutaCarpeta + archivoSolicitud))
                            {
                                File.Delete(rutaCarpeta + archivoSolicitud);
                            }

                            if (File.Exists(rutaCarpeta + archivoOrigenFondos))
                            {
                                File.Delete(rutaCarpeta + archivoOrigenFondos);
                            }

                            if (File.Exists(rutaCarpeta + archivoConstanciaAbono))
                            {
                                File.Delete(rutaCarpeta + archivoConstanciaAbono);
                            }

                            if (File.Exists(rutaCarpeta + archivoPersonaExpuestaPoliticamente))
                            {
                                File.Delete(rutaCarpeta + archivoPersonaExpuestaPoliticamente);
                            }

                            if (File.Exists(rutaCarpeta + archivoDetalleCotizacion))
                            {
                                File.Delete(rutaCarpeta + archivoDetalleCotizacion);
                            }

                            // indicador de rescate 
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            int ind_rescate = servicioCotizador.ObtenerIndicadorRescateIFP(numSolicitud, usuario);
                            
                            if (ind_rescate > 0)
                            {
                                ind_rescate = 1;
                            }
                            else
                            {
                                ind_rescate = 0;
                            }

                            respuesta.Estado = Constante.COD_OK;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "formatos generados correctamente." });
                            respuesta.Contenido = ind_rescate.ToString();
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusActualizar.StringValue()));
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

        private static byte[] ConsumirServicio(string url, string usuario, string token_generado)
        {
            log.Info("Leyendo el servicio: " + url);
            WebClient myWebClient = new WebClient();
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token_generado));
            myWebClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            byte[] formatoByteArray = myWebClient.DownloadData(url);
            myWebClient.Dispose();
            return formatoByteArray;
        }

        private static T ObtenerUbigeo<T>(string urlToken, string urlServicio, string usuario, T request)
        {
            T respuesta = default(T);
            string token_generado = string.Empty;

            try
            {
                log.Info("Consumiendo servicio token: " + urlToken);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                log.Info("Pasando el json al servicio");
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"usuario\": \"" + usuario + "\"}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                log.Info("Leyendo el servicio");
                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var jsonResult = streamReader.ReadToEnd();
                    JObject jObject = JObject.Parse(jsonResult);
                    token_generado = (string)jObject["accessToken"];
                }

                if (token_generado != "")
                {
                    log.Info("Consumiendo servicio" + urlServicio);
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServicio);
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                    log.Info("Leyendo el servicio");

                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        var jsonResult = streamReader.ReadToEnd();

                        if (jsonResult.Length > 0)
                        {
                            respuesta = (T)JsonConvert.DeserializeObject(jsonResult);
                        }
                    }
                }

            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    throw new Exception("Error en el servicio");
                }
            }
            return respuesta;
        }
    }
}
