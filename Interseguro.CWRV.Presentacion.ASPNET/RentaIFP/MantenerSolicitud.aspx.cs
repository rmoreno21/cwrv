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

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
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
                    //Validar permisos
                    //if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionesIFP))
                    //{
                        HFechaActual.Value = Convert.ToDateTime(DateTime.Today, new CultureInfo("es-PE")).ToString("dd/MM/yyyy");

                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            //servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            //int Cantidad = servicioCotizador.CantidadSolicitudes(HttpContext.Current.Session["CUSPPIFP"].ToString(), moneda, Convert.ToDouble(montoPrimaUnica));
                            //HttpContext.Current.Session["CantidadSolicitudes"] = Cantidad;

                            CargarInformacionInicialPantalla();

                            var modSolMod = Session["ModSolModo"];
                            var idSolicitud = Session["idSolicitud"];
                            var fechaCotizacion = Session["fecCotizacion"];
                            var cusppRP = Session["CUSPP_RP"];
                            var rolAzman = Session["RolAzman"];

                            if (modSolMod != null)
                            {
                                modSolMod = Session["ModSolModo"].ToString();
                            }

                            if(idSolicitud != null)
                            {
                                idSolicitud = Session["idSolicitud"].ToString();
                            }

                            string modoSolicitudParam = Request.QueryString["modSolModo"];
                            string idSolicitudParam = Request.QueryString["idSolicitud"];
                            string fecCotizacionParam = Request.QueryString["fecCotizacion"];

                            if(!string.IsNullOrEmpty(modoSolicitudParam))
                            {
                                modSolMod = modoSolicitudParam;
                            }

                            if (!string.IsNullOrEmpty(idSolicitudParam))
                            {
                                idSolicitud = idSolicitudParam;
                            }

                            if (!string.IsNullOrEmpty(fecCotizacionParam))
                            {
                                fechaCotizacion = fecCotizacionParam;
                            }



                            // Session["CantidadSolicitudes"] = 0;

                            if (modSolMod != null)
                            {
                                HCUSPP_RP.Value = cusppRP.ToString();
                                //HAFP_RP.Value = Session["AFP_RP"].ToString();

                                bool isPlan1 = false;

                                // List<GrupoFamiliar> grupoFamiliar = servicioCotizador.ListarGrupoFamiliar(HCUSPP_RP.Value);
                                // HttpContext.Current.Session["Beneficiarios"] = grupoFamiliar;

                                LabModSolAviso.Visible = false;

                                if (modSolMod.Equals("M"))
                                {
                                    if (DateTime.Today != Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")))
                                    {
                                        if (((string)rolAzman) != "JEF.RVI.OPE" 
                                                && ((string)rolAzman) != "AST.RVI.COM"
                                                && ((string)rolAzman) != "JEF.VTA.LIM.RVI"
                                                && ((string)rolAzman) != "JEF.VTA.PRO.RVI")
                                        {
                                            modSolMod = "CONS";
                                        }
                                    }
                                }

                                log.Info("Consultando ObtenerParametrosGenerales");
                                //Nuevo
                                // ObtenerParametrosGenerales(Session["TokenUsuario"].ToString(), "001", "T05", Convert.ToDateTime(DateTime.Today, new CultureInfo("es-PE")));

                                //Modificar
                                if (modSolMod.Equals("M"))
                                {

                                    // Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(Convert.ToString(idSolicitud));

                                    //Combo Planes
                                    //List<Parametro> listaParametro = new List<Parametro>();
                                    //listaParametro = CargarListaParametros("PLANRP");
                                    //ModSolPlan_IFP.Items.Clear();
                                    //ModSolPlan_IFP.Items.Add(new ListItem("«Seleccione»", "0"));
                                    //foreach (Parametro item in listaParametro)
                                    //{
                                    //    ModSolPlan_IFP.Items.Add(new ListItem(item.Nombre, item.Id));
                                    //}

                                    foreach (CotizacionIFP cotizacionIFP in solicitud.Cotizaciones)
                                    {

                                        //ParametrosMotorIFP parametrosMotorIFP = new ParametrosMotorIFP();
                                        //parametrosMotorIFP = servicioCotizador.ObtenerParametroGenerales(cotizacionIFP.Temporalidad.Id, cotizacionIFP.Moneda.Id, solicitud.FechaCotizacion.Value, true, Session["TokenUsuario"].ToString());

                                        if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN1.StringValue())
                                        {
                                            ModSolPlan_IFP.Items.Remove(new ListItem("Ingreso Flexible con Devolución", "PLAN1"));
                                        }
                                        else if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN2.StringValue())
                                        {
                                            ModSolPlan_IFP.Items.Remove(new ListItem("Ingreso Seguro con Devolución", "PLAN2"));
                                        }
                                        else if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN3.StringValue())
                                        {
                                            ModSolPlan_IFP.Items.Remove(new ListItem("Ingreso Vitalicio Seguro", "PLAN3"));
                                        }

                                        ///*Inicio carga parametros*/
                                        //ObtenerParametrosGenerales(Session["TokenUsuario"].ToString(), cotizacionIFP.Moneda.Id, cotizacionIFP.Temporalidad.Id, solicitud.FechaCotizacion.Value);
                                        ///*Fin carga parametros*/
                                    }

                                    ModSolNroSolicitud_IFP.Text = solicitud.Id;
                                    ModSolMonedaPrimaUnica_IFP.SelectedIndex = ModSolMonedaPrimaUnica_IFP.Items.IndexOf(ModSolMonedaPrimaUnica_IFP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    ModSolPrimaUnica_IFP.Text = solicitud.PrimaUnica.ToString();
                                    ModSolFechaCotizacion_IFP.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_IFP.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                    ModSolFechaVigencia_IFP.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");
                                    HAgenteId.Value = solicitud.Agente.Id;

                                    // Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    // Session["RP_Cotizaciones"] = solicitud.Cotizaciones;
                                    // Session["RP_CoberturasAdicionales"] = solicitud.CoberturasAdicionales;

                                    var cotizacionSeleccionada = solicitud.Cotizaciones.FindAll(p => p.EstadoCotizacion == "04" && p.IndSeleccionada == "S");
                                    if (cotizacionSeleccionada.Count > 0)
                                    {
                                        HSeleccionada.Value = "S";
                                        LabModSolAviso.Visible = true;
                                    }

                                    HEstado.Value = solicitud.CodigoEstado.ToString();
                                    LabMensaje.Text = solicitud.EstadoSolicitud.ToString();
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

                                    ModSolTipoCambio_IFP.Text = solicitud.TipoCambio.ToString();
                                    ModSolTipoCambioPanel.Visible = true;
                                    ModSolLineaNumPoliza_RP.Visible = false;

                                    if (solicitud.NumeroPoliza != 0)
                                    {
                                        ModSolLineaNumPoliza_RP.Visible = true;
                                        ModSolNumPoliza_RP.Text = solicitud.NumeroPoliza.ToString();
                                    }

                                    ModSolModo.Value = Convert.ToString(modSolMod);
                                    HCopia.Value = "";

                                    if (Convert.ToString(modSolMod) == "C")
                                        HCopia.Value = "C";

                                    ModSolPrimaUnica_IFP.Visible = true;
                                    ManSolTipoSolicitud_RP.Value = "EXTRAOFICIAL";

                                    HBloqueo.Value = "TRUE";
                                    if (((string)rolAzman) == "JEF.RVI.OPE")
                                    {
                                        HBloqueo.Value = "FALSE";
                                    }

                                    //Validando la fecha de cotizacion
                                    switch ((string)rolAzman)
                                    {
                                        case "JEF.RVI.OPE"://JefeOperaciones
                                        case "AST.RVI.OPE"://AsistenteOperaciones
                                            if (modSolMod.ToString() != "CONS")
                                            {
                                                ModSolFechaCotizacion_IFP.Enabled = true;
                                            }
                                            break;
                                        default:
                                            ModSolFechaCotizacion_IFP.Enabled = false;
                                            break;
                                    }

                                    // foreach (CotizacionIFP cotizacionIFP in solicitud.Cotizaciones)
                                    // {
                                    //     log.Info("Consultando ObtenerParametrosGenerales");
                                    //     /*Inicio carga parametros*/
                                    //     ObtenerParametrosGenerales(Session["TokenUsuario"].ToString(), cotizacionIFP.Moneda.Id, cotizacionIFP.Temporalidad.Id, solicitud.FechaCotizacion.Value);
                                    //     /*Fin carga parametros*/
                                    // }

                                    // ModSolMonedaPrimaUnica_IFP.Enabled = false;

                                }
                                else if (modSolMod.Equals("C"))
                                {
                                    //Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(idSolicitud.ToString());

                                    foreach (CotizacionIFP cotizacionIFP in solicitud.Cotizaciones)
                                    {
                                        if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN1.StringValue())
                                        {
                                            //ModSolPlan_IFP.Items.Remove(new ListItem("Ingreso Flexible con Devolución", "PLAN1"));
                                            isPlan1 = true;
                                        }
                                        else if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN2.StringValue())
                                        {
                                            ModSolPlan_IFP.Items.Remove(new ListItem("Ingreso Seguro con Devolución", "PLAN2"));
                                        }
                                        else if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN3.StringValue())
                                        {
                                            ModSolPlan_IFP.Items.Remove(new ListItem("Ingreso Vitalicio Seguro", "PLAN3"));
                                        }
                                        cotizacionIFP.AjusteTRA = 0;

                                        ///*Inicio carga parametros*/
                                        // ObtenerParametrosGenerales(Session["TokenUsuario"].ToString(), cotizacionIFP.Moneda.Id, cotizacionIFP.Temporalidad.Id, ((DateTime)DateTime.Today));
                                        ///*Fin carga parametros*/
                                    }

                                    //ModSolNroSolicitud_IFP.Text = solicitud.Id;
                                    ModSolMonedaPrimaUnica_IFP.SelectedIndex = ModSolMonedaPrimaUnica_IFP.Items.IndexOf(ModSolMonedaPrimaUnica_IFP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    ModSolPrimaUnica_IFP.Text = solicitud.PrimaUnica.ToString();

                                    ModSolFechaCotizacion_IFP.Text = ((DateTime)DateTime.Now).ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_IFP.Text = ((DateTime)DateTime.Today.AddDays(-(DateTime.Today.Day - 1))).ToString("dd/MM/yyyy");
                                    //diasVigencia
                                    ModSolFechaVigencia_IFP.Text = ((DateTime)DateTime.Now.AddDays(Convert.ToInt32(HDiasVigencia.Value))).ToString("dd/MM/yyyy");

                                    //Cambiando de estado, por defecto 02
                                    solicitud.Cotizaciones.ForEach(p =>
                                        p.EstadoCotizacion = "02"
                                      );

                                    HAgenteId.Value = solicitud.Agente.Id;

                                    // Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    // Session["RP_Cotizaciones"] = solicitud.Cotizaciones;

                                    ModSolTipoCambio_IFP.Text = solicitud.TipoCambio.ToString();
                                    ModSolTipoCambioPanel.Visible = true;

                                    ModSolLineaNumPoliza_RP.Visible = false;

                                    solicitud.NumeroPoliza = 0;
                                    // ModSolMonedaPrimaUnica_IFP.Enabled = false;
                                }
                                //Consultar
                                else if (modSolMod.Equals("CONS"))
                                {
                                    //Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(idSolicitud.ToString());

                                    ModSolNroSolicitud_IFP.Text = solicitud.Id;
                                    ModSolMonedaPrimaUnica_IFP.SelectedIndex = ModSolMonedaPrimaUnica_IFP.Items.IndexOf(ModSolMonedaPrimaUnica_IFP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    ModSolPrimaUnica_IFP.Text = solicitud.PrimaUnica.ToString();

                                    ModSolFechaCotizacion_IFP.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_IFP.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                    ModSolFechaVigencia_IFP.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");

                                    HAgenteId.Value = solicitud.Agente.Id;

                                    // Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    // Session["RP_Cotizaciones"] = solicitud.Cotizaciones;
                                    // Session["RP_CoberturasAdicionales"] = solicitud.CoberturasAdicionales;

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

                                    //Bloqueando controles
                                    ModSolMonedaPrimaUnica_IFP.Enabled = false;

                                    ModSolFechaCotizacion_IFP.Enabled = false;
                                    ModSolFechaDevengue_IFP.Enabled = false;
                                    ModSolFechaVigencia_IFP.Enabled = false;

                                    ModSolPrimaUnica_IFP.ReadOnly = true;
                                    ModSolTipoCambio_IFP.ReadOnly = true;

                                    ModSolMonedaPrimaUnica_IFP.CssClass = "formCombobox formComboboxReadOnly";

                                    ModSolFechaCotizacion_IFP.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";
                                    ModSolFechaDevengue_IFP.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";
                                    ModSolFechaVigencia_IFP.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";

                                    ModSolTipoCambio_IFP.CssClass = "formTextbox formTextboxReadOnly";
                                    ModSolPrimaUnica_IFP.CssClass = "formTextbox formTextboxReadOnly";

                                    ModSolLineaNumPoliza_RP.Visible = false;
                                    if (solicitud.NumeroPoliza != 0)
                                    {
                                        ModSolLineaNumPoliza_RP.Visible = true;
                                        ModSolNumPoliza_RP.Text = solicitud.NumeroPoliza.ToString();
                                    }

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

                                }
                                //Nuevo
                                else if (modSolMod.Equals("N"))
                                {
                                    HSeleccionada.Value = "N";
                                    HCUSPP_RP.Value = cusppRP.ToString();
                                    //HAFP_RP.Value = Session["AFP_RP"].ToString();

                                    ModSolFechaCotizacion_IFP.Text = DateTime.Now.ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_IFP.Text = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)).ToString("dd/MM/yyyy");
                                    ModSolFechaVigencia_IFP.Text = DateTime.Now.AddDays(Convert.ToInt32(HDiasVigencia.Value)).ToString("dd/MM/yyyy");

                                    ModSolLineaNumPoliza_RP.Visible = false;
                                    HEstado.Value = "0";

                                    // Session.Remove("RP_Beneficiarios");

                                    // ModSolMonedaPrimaUnica_IFP.Items.Clear();
                                    // ModSolMonedaPrimaUnica_IFP.Items.Add(new ListItem("«Seleccione»", "0"));
                                    // ModSolMonedaPrimaUnica_IFP.Items.Add(new ListItem("DOLAR(US$)", "002"));
                                }

                                ModSolModo.Value = Convert.ToString((modSolMod));
                                HCopia.Value = "";

                                if (Convert.ToString((modSolMod)) == "C")
                                    HCopia.Value = "C";

                                ModSolPrimaUnica_IFP.Visible = true;
                                ManSolTipoSolicitud_RP.Value = "EXTRAOFICIAL";

                                HBloqueo.Value = "TRUE";
                                if (((string)rolAzman) == "JEF.RVI.OPE" 
                                        || (string)rolAzman == "AST.RVI.COM"
                                        || (string)rolAzman == "JEF.VTA.LIM.RVI"
                                        || (string)rolAzman == "JEF.VTA.PRO.RVI")
                                {
                                    HBloqueo.Value = "FALSE";
                                }

                                ListarPorcentajeDevolucion(ModSolFechaCotizacion_IFP.Text, "");

                                //Validando la fecha de cotizacion
                                switch ((string)rolAzman)
                                {
                                    case "JEF.RVI.OPE"://JefeOperaciones
                                    case "AST.RVI.OPE"://AsistenteOperaciones
                                        if (!modSolMod.Equals("CONS"))
                                        {
                                            ModSolFechaCotizacion_IFP.Enabled = true;
                                        }
                                        break;
                                    default:
                                        ModSolFechaCotizacion_IFP.Enabled = false;
                                        break;
                                }

                                if (modSolMod.Equals("C") && isPlan1)
                                {
                                    HSeleccionada.Value = "S";
                                    throw new Exception("No se puede copiar una solicitud que contiene una cotización: Ingreso Flexible con Devolución - PLAN 1, por favor vuelva a la lista de solicitudes");
                                }

                            }
                            else
                            {
                                Response.Redirect("Cotizador.aspx");
                            }
                        }
                        else
                        {
                            if (ModSolPrimaUnica_IFP.Text != string.Empty) ModSolPrimaUnica_IFP.Text = Convert.ToDouble(ModSolPrimaUnica_IFP.Text, new CultureInfo("es-PE")).ToString();
                        }
                    //}
                    //else
                    //{
                    //    log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                    //        Enums.OpcionesSistema.MenuCotizador.StringValue()));
                    //    Response.Redirect("~/Error/Permisos.aspx");
                    //}
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
                    HBloqueo.Value = "TRUE";
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }

        [WebMethod]
        public static List<Parametro> ListarPorcentajeDevolucion(string fec_cotizacion, string plan)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var listaPorcentajeDevolucion = (List<Parametro>)HttpContext.Current.Session["ListaDevolucionSob"];

                    DateTime fechaCotizacion = Convert.ToDateTime(fec_cotizacion, new CultureInfo("es-PE"));

                    var ComboDevSobrevivencia = listaPorcentajeDevolucion.FindAll(p => p.FecInicioVigencia <= fechaCotizacion
                                            && p.FecFinVigencia >= fechaCotizacion);

                    ComboDevSobrevivencia = ComboDevSobrevivencia.Where(p => p.Valor_2.Contains(plan)).ToList();

                    HttpContext.Current.Session["ComboIFPDevSobrevivencia"] = ComboDevSobrevivencia;

                    return ComboDevSobrevivencia;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            CargarCombobox(ModSolMonedaPrimaUnica_IFP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.MonedaIFP]);

            Session["ComboSexo"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo];

            CargarComboboxNuevosDatos(ModSolPlan_IFP, "PLANRP");

            /*INI-TEMPORAL SOLO PARA PRUEBAS OPERACIONES*/
            //if ((string)HttpContext.Current.Session["RolAzman"] != Enums.RolAzman.JefeOperaciones.StringValue())
            //{
            //    ModSolPlan_IFP.Items.Remove(new ListItem("Ingreso Vitalicio Seguro", "PLAN3"));
            //}
            /*FIN-TEMPORAL SOLO PARA PRUEBAS OPERACIONES*/

            //GTI.57799: Desactivación IFD
            ModSolPlan_IFP.Items.Remove(new ListItem("Ingreso Flexible con Devolución", "PLAN1"));
            
            //if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
            //{
            //    ModSolPlan_IFP.Items.RemoveAt(1); // Plan 1
            //    /*
            //    ModSolPlan_IFP.Items.Clear();
            //    ModSolPlan_IFP.Items.Add(new ListItem("Ingreso Seguro con Devolución", "PLAN2"));
            //    ModSolPlan_IFP.SelectedIndex = 0;
            //    ModSolPlan_IFP.Enabled = false;
            //    ModSolPlan_IFP.CssClass = "formCombobox formComboboxReadOnly";
            //    */
            //}

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaParametro = servicioCotizador.ObtenerComboboxIFP();

            //Parametros IFP
            //Session["ComboIFPDCOM"] = CargarListaParametros("DCOMIFP");
            Session["ComboIFPPagoDoble"] = CargarListaParametros("PagoDobleIFP");
            Session["ComboIFPDiferimiento"] = CargarListaParametros("DiferimientoIFP");
            Session["ComboIFPDevFallecimiento"] = CargarListaParametros("DevFallecimientoIFP");

            RolDcom rolDcom = new RolDcom
            {
                CodRol = (string)HttpContext.Current.Session["RolAzman"],
                FechaCotizacion = Convert.ToDateTime(DateTime.Today, new CultureInfo("es-PE")),
            };

            Session["ComboRangoIFPDCOM"] = servicioCotizador.ListarRangoDcomIFP(rolDcom);

            List<Parametro> listaTemporalidad = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.Temporalidad];
            Session["ComboIFPTemporalidad"] = listaTemporalidad;

            //Session["ComboIFPDevSobrevivencia"] = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.PorcentajeDev];
            Session["ListaDevolucionSob"] = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.PorcentajeDev];
            Session["ComboIFPDevSobrevivencia"] = Session["ListaDevolucionSob"];

            Session["ComboTramoEscalonada"] = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.PorcentajeEscalon];

            Session["ComboIFPMoneda"] = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.Moneda];

            //Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            //LabModSolLineaACOMDCOM_RP.Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);

            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;

            List<Parametro> lstDiasVigencia = new List<Parametro>();
            lstDiasVigencia = servicioCotizador.ObtenerParametrosPorTabla("PLUS");
            if (lstDiasVigencia.Count() > 0)
            {
                HDiasVigencia.Value = lstDiasVigencia[0].Valor_1;
            }

            ModSolDNI_IFP.Text = Session["DNIIFP"].ToString();
            ModSolNombres_IFP.Text = Session["NombresIFP"].ToString();
            ModSolApellidos_IFP.Text = Session["ApellidosIFP"].ToString();

            TabCotizacionesLeyenda_RP.Visible = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoEspeciales)) ? true : false;
            
            //if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
            //{
            //    ContenedorPlna_RP.Visible = false;
            //    //ClientScript.RegisterStartupScript(GetType(), "-", "HabilitarPlan2Inteligo();", true);
            //}
            //else
            //{
            //    ContenedorPlna_RP.Visible = true;
            //}

            Session["ComboIFPPeriodoGarantizado"] = CargarListaParametros("PeriodoGarantizadoIFP");

            List<Parametro> lstIFPPorcentajeCA = new List<Parametro>();
            for (int iporcentajeCA = 0; iporcentajeCA <= 100; iporcentajeCA++)
            {
                Parametro porcentajeCA = new Parametro();
                porcentajeCA.Id = iporcentajeCA.ToString();
                porcentajeCA.Nombre = iporcentajeCA.ToString() + "%";
                porcentajeCA.Valor_1 = iporcentajeCA.ToString() + "%";
                porcentajeCA.Glosa = iporcentajeCA.ToString() + "%";

                lstIFPPorcentajeCA.Add(porcentajeCA);
            }

            Session["ComboIFPPorcentajeCA"] = lstIFPPorcentajeCA;

            //CargarCombobox(VitTipoPlan, listaCombobox[(int)Enums.CategoriaCombobox.TipoPlan]);
        }

        //INI cargar Combo
        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();

            control.Items.Add(new ListItem("«Seleccione»", "0"));

            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

        private void CargarComboboxNuevosDatos(DropDownList control, string tabla)
        {

            List<Parametro> listaParametro = new List<Parametro>();

            listaParametro = CargarListaParametros(tabla);

            if (tabla == "PLANRP")
            {
                listaParametro = listaParametro.Where(t => t.Valor_1 == Enums.TipoCotizacion.RentaPrivadaIFP.StringValue()).ToList();
            }

            control.Items.Clear();

            control.Items.Add(new ListItem("«Seleccione»", "0"));

            foreach (Parametro item in listaParametro)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }

        }

        private List<Parametro> CargarListaParametros(string tabla)
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerParametros(tabla);

            return listaParametro;
        }

        [WebMethod]
        public static string RenderizarTabla(List<CotizacionIFP> cotizaciones, string moneda, string montoPrimaUnica, string plan, List<CoberturaAdicional> coberturasAdicionales, int cantidad, bool permiteTra, bool permiteEspeciales, string modSolModo)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {

                    //if ((Convert.ToInt32(HttpContext.Current.Session["CantidadSolicitudes"]) + Convert.ToInt32(HttpContext.Current.Session["CantidadSolicitudesIPFPlan2"]) + cotizaciones.Count) >= Convert.ToInt32(ConfigurationManager.AppSettings["MaxCotizacionesIFP"]))
                    //{
                    //    TabCotizacionesBotonera_IFP.Visible = false;
                    //}

                    for (int cot = 0; cot < cotizaciones.Count; cot++)
                    {
                        if (cotizaciones[cot].Plan.Id == "")
                        {
                            cotizaciones[cot].Plan.Id = plan;
                        }
                    }

                    Page pagina = new Page();

                    dynamic control = (dynamic)pagina.LoadControl("~/Controles/TablaCotizaciones" + plan + "IFP.ascx");
                    if (plan == Enums.Planes.PLAN1.StringValue())
                    {
                        control = (TablaCotizacionesPlan1IFP)control;
                    }
                    else if (plan == Enums.Planes.PLAN2.StringValue())
                    {
                        control = (TablaCotizacionesPlan2IFP)control;
                    }
                    else if (plan == Enums.Planes.PLAN3.StringValue())
                    {
                        control = (TablaCotizacionesPlan3IFP)control;
                        control.CoberturasAdicionales = coberturasAdicionales;
                    }

                    control.CantidadCotizaciones = cotizaciones.Count();
                    control.CotizacionesIFP = cotizaciones.FindAll(c => c.Plan.Id == plan);

                    control.Moneda = moneda;

                    //Validando si el acceso es desde dentro de la red de Interseguro o desde Internet
                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                    {
                        control.PermisoTRA = permiteTra; // (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoTRAPlus)) ? true : false;
                        control.PermisoEspeciales = permiteEspeciales; // (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoEspeciales)) ? true : false;
                    }
                    else
                    {
                        control.PermisoTRA = false;
                        control.PermisoEspeciales = false;
                    }

                    control.PermisoAgregar = true;
                    control.PermisoModificar = true;
                    control.PermisoEliminar = true;
                    control.ModoSolicitud = modSolModo;

                    pagina.Controls.Add(control);

                    //HttpContext.Current.Session["idMonedaFondo"] = moneda;

                    /*servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    int Cantidad = servicioCotizador.CantidadSolicitudes(HttpContext.Current.Session["CUSPPIFP"].ToString(), moneda, Convert.ToDouble(montoPrimaUnica));*/
                    // HttpContext.Current.Session["CantidadSolicitudes"] = cantidad;

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
        public static string CargarTabla(List<CotizacionIFP> cotizaciones, string moneda, string montoPrimaUnica, string plan, List<CoberturaAdicional> coberturasAdicionales)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {

                    //if ((Convert.ToInt32(HttpContext.Current.Session["CantidadSolicitudes"]) + Convert.ToInt32(HttpContext.Current.Session["CantidadSolicitudesIPFPlan2"]) + cotizaciones.Count) >= Convert.ToInt32(ConfigurationManager.AppSettings["MaxCotizacionesIFP"]))
                    //{
                    //    TabCotizacionesBotonera_IFP.Visible = false;
                    //}

                    for (int cot = 0; cot < cotizaciones.Count; cot++)
                    {
                        if (cotizaciones[cot].Plan.Id == "")
                        {
                            cotizaciones[cot].Plan.Id = plan;
                        }
                    }

                    Page pagina = new Page();

                    dynamic control = (dynamic)pagina.LoadControl("~/Controles/TablaCotizaciones" + plan + "IFP.ascx");
                    if (plan == Enums.Planes.PLAN1.StringValue())
                    {
                        control = (TablaCotizacionesPlan1IFP)control;
                    }
                    else if (plan == Enums.Planes.PLAN2.StringValue())
                    {
                        control = (TablaCotizacionesPlan2IFP)control;
                    }
                    else if (plan == Enums.Planes.PLAN3.StringValue())
                    {
                        control = (TablaCotizacionesPlan3IFP)control;
                        control.CoberturasAdicionales = coberturasAdicionales;
                    }

                    control.CantidadCotizaciones = cotizaciones.Count();
                    control.CotizacionesIFP = cotizaciones.FindAll(c => c.Plan.Id == plan);

                    control.Moneda = moneda;

                    //Validando si el acceso es desde dentro de la red de Interseguro o desde Internet
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

                    //HttpContext.Current.Session["idMonedaFondo"] = moneda;

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    int Cantidad = servicioCotizador.CantidadSolicitudes(HttpContext.Current.Session["CUSPPIFP"].ToString(), moneda, Convert.ToDouble(montoPrimaUnica));
                    HttpContext.Current.Session["CantidadSolicitudes"] = Cantidad;

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
        public static string RenderTablaCotizaciones(List<CotizacionIFP> cotizaciones, string moneda, string plan, List<CoberturaAdicional> coberturasAdicionales, bool permiteTra, bool permiteEspeciales, string modSolModo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    List<CotizacionIFP> cotizacionesNew = new List<CotizacionIFP>();
                    var pagina = new Page();

                    dynamic control;

                    control = (dynamic)pagina.LoadControl("~/Controles/TablaCotizaciones" + plan + "IFP.ascx");

                    if (plan == Enums.Planes.PLAN1.StringValue())
                    {
                        control = (TablaCotizacionesPlan1IFP)control;
                    }
                    else if (plan == Enums.Planes.PLAN2.StringValue())
                    {
                        control = (TablaCotizacionesPlan2IFP)control;
                    }
                    else if (plan == Enums.Planes.PLAN3.StringValue())
                    {
                        control = (TablaCotizacionesPlan3IFP)control;
                        control.CoberturasAdicionales = coberturasAdicionales;
                    }

                    for (int i = 0; i < cotizaciones.Count; i++)
                    {
                        cotizaciones[i].Item = i;

                        if (modSolModo != null && modSolModo.ToString() == "C")
                        {
                            cotizaciones[i].Correlativo = 0;
                            cotizaciones[i].Pension2doTramo = 0;
                            cotizaciones[i].Pension2doTramoSinAjuste = 0;
                            cotizaciones[i].PensionCia = 0;
                            cotizaciones[i].PensionCiaMO = 0;
                            cotizaciones[i].ValPrimeraRentaIS = 0;
                            cotizaciones[i].TasaVenta = 0;
                            cotizaciones[i].TasaVentaSbs = 0;
                            cotizaciones[i].TasaRetornoAccionista = 0;
                            cotizaciones[i].TasaRetornoAccionistaMinima = 0;
                            cotizaciones[i].IndCotiza = "";
                            cotizaciones[i].AjusteTRA = 0;
                        }
                    }

                    control.CantidadCotizaciones = cotizaciones.Count();
                    control.CotizacionesIFP = cotizaciones.FindAll(p => p.Plan.Id == plan);

                    control.Moneda = moneda;

                    //Validando si el acceso es desde dentro de la red de Interseguro o desde Internet
                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                    {
                        control.PermisoTRA = permiteTra; // (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoTRAPlus)) ? true : false;
                        control.PermisoEspeciales = permiteEspeciales; // (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoEspeciales)) ? true : false;
                    }
                    else
                    {
                        control.PermisoTRA = false;
                        control.PermisoEspeciales = false;
                    }

                    control.PermisoAgregar = true;
                    control.PermisoModificar = true;
                    control.PermisoEliminar = true;
                    control.ModoSolicitud = modSolModo;

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
        public static List<Parametro> ObtenerMonedaAjuste(string codMoneda)
        {
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametroResultado = new List<Parametro>();

            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboIFPMoneda"];

            lstParametroResultado = lstParametro.FindAll(m => (m.Id == codMoneda));

            return lstParametroResultado;
        }

        [WebMethod]
        public static List<Parametro> ObtenerPagoDoble(double val_temporalidad, double val_diferido)
        {
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametroResultado = new List<Parametro>();

            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboIFPPagoDoble"];

            lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) + val_diferido < val_temporalidad);

            if (lstParametroResultado.Count == 0)
            {
                lstParametroResultado = lstParametro.FindAll(m => m.Id == "0");
            }

            return lstParametroResultado;
        }

        [WebMethod]
        public static List<Parametro> ObtenerPagoDiferimiento(int strTemporalidad, string plan)
        {
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametroResultado = new List<Parametro>();

            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboIFPDiferimiento"];

            if (plan == Enums.Planes.PLAN1.StringValue())
            {
                if (strTemporalidad == 15)
                {
                    lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= 7.5);
                }
                else if (strTemporalidad <= 10)
                {
                    lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= 5);
                }
                else
                {
                    lstParametroResultado = lstParametro.FindAll(m => m.Id == "0");
                }
            }
            else if (plan == Enums.Planes.PLAN2.StringValue())
            {
                lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= 10 && Convert.ToDouble(m.Nombre) <= strTemporalidad);
            }
            
            return lstParametroResultado;
        }

        [WebMethod]
        public static List<Parametro> ObtenerPagoDiferimientoCPagoDoble(int strPagoDoble, int num_temporalidad, string plan)
        {
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametroResultado = new List<Parametro>();

            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboIFPDiferimiento"];

            if (plan == Enums.Planes.PLAN1.StringValue())
            {
                if (strPagoDoble > 0)
                {

                    if (num_temporalidad == 15)
                    {
                        lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= 7.5 && Convert.ToDouble(m.Nombre) < strPagoDoble);
                    }
                    else if (num_temporalidad <= 10)
                    {
                        lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= 5 && Convert.ToDouble(m.Nombre) < strPagoDoble);
                    }
                    else
                    {
                        lstParametroResultado = lstParametro.FindAll(m => m.Id == "0");
                    }
                }
                else
                {
                    if (num_temporalidad == 15)
                    {
                        lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= 7.5);
                    }
                    else if (num_temporalidad <= 10)
                    {
                        lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= 5);
                    }
                    else
                    {
                        lstParametroResultado = lstParametro.FindAll(m => m.Id == "0");
                    }
                }
            }
            else if (plan == Enums.Planes.PLAN2.StringValue())
            {
                if (strPagoDoble > 0)
                {
                    lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= 10 && Convert.ToDouble(m.Nombre) < strPagoDoble);
                }
                else
                {
                    lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= 10 && Convert.ToDouble(m.Nombre) <= num_temporalidad);
                }

            }

            return lstParametroResultado;
        }

        [WebMethod]
        public static List<Parametro> ObtenerDevFallecimiento(int strDevolucion)
        {
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametroResultado = new List<Parametro>();

            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboIFPDevFallecimiento"];

            lstParametroResultado = lstParametro.FindAll(m => Convert.ToInt32(m.Id) <= strDevolucion);

            if (lstParametroResultado.Count == 0)
            {
                lstParametroResultado = lstParametro.FindAll(m => m.Id == "0");
            }

            return lstParametroResultado;
        }

        //<INI.GTI_7012_30>

        [WebMethod]
        public static List<Parametro> ObtenerMonedaFullDiferido(string codMoneda)
        {
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametroResultado = new List<Parametro>();

            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboIFPMoneda"];

            lstParametroResultado = lstParametro.FindAll(m => (m.Valor_2.Contains(codMoneda) && (m.Valor_1.Contains(Enums.TipoMonedaIFP.Indexado.StringValue())) ||
                (m.Valor_2.Contains(codMoneda) && (m.Valor_1.Contains(Enums.TipoMonedaIFP.Nominal.StringValue()))) ||
                (m.Valor_2.Contains(codMoneda) && (m.Valor_1.Contains(Enums.TipoMonedaIFP.Ajustados.StringValue()))) ||
                (m.Valor_2.Contains(codMoneda) && (m.Valor_1.Contains("Seleccione")))));

            // lstParametroResultado = lstParametro.FindAll(m => (m.Valor_2.Contains(codMoneda) && (m.Valor_1.Contains(Enums.TipoMonedaIFP.Nominal.StringValue())) ||
            //     (m.Valor_2.Contains(codMoneda) && (m.Valor_1.Contains(Enums.TipoMonedaIFP.Ajustados.StringValue()))) ||
            //     (m.Valor_2.Contains(codMoneda) && (m.Valor_1.Contains("Seleccione")))));

            return lstParametroResultado;
        }

        //<FIN.GTI_7012_30>

        [WebMethod]
        public static List<Parametro> ObtenerMoneda(string codMoneda)
        {
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametroResultado = new List<Parametro>();

            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboIFPMoneda"];

            lstParametroResultado = lstParametro.FindAll(m => (m.Valor_2.Contains(codMoneda)));

            return lstParametroResultado;
        }

        [WebMethod]
        public static Respuesta CargarParametroGeneralMotorIFP(string tokenUsuario, string cod_tipo_temporalidad, string cod_moneda, string fec_cotizacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        DateTime fechaCotizacion = Convert.ToDateTime(fec_cotizacion, new CultureInfo("es-PE"));

                        ObtenerParametrosGenerales(tokenUsuario, cod_moneda, cod_tipo_temporalidad, fechaCotizacion);
                    }
                    respuesta.Estado = Constante.COD_OK;
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Mensaje = "Error en la carga de parámetros, contactarse con mesa de ayuda";
                    respuesta.Estado = Constante.COD_ERROR;
                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static Respuesta CargarParametrosGeneralesMotorIFP(string tokenUsuario, List<CotizacionIFP> cotizaciones, string fec_cotizacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        DateTime fechaCotizacion = Convert.ToDateTime(fec_cotizacion, new CultureInfo("es-PE"));

                        foreach (var cotizacion in cotizaciones)
                        {
                            ObtenerParametrosGenerales(tokenUsuario, cotizacion.Moneda.Id, cotizacion.Temporalidad.Id, fechaCotizacion);
                        }
                    }
                    respuesta.Estado = Constante.COD_OK;
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Mensaje = "Error en la carga de parámetros, contactarse con mesa de ayuda";
                    respuesta.Estado = Constante.COD_ERROR;
                    return respuesta;
                }
            }
        }

        private static void ObtenerParametrosGenerales(string tokenUsuario, string cod_moneda, string cod_temporalidad, DateTime fec_cotizacion)
        {
            List<ParametrosMotorIFP> lstParametrosMotorIFP = (List<ParametrosMotorIFP>)HttpContext.Current.Session["ParametroMotorGeneral"];
            string usuario = (string)HttpContext.Current.Session["Usuario"].ToString();

            if (lstParametrosMotorIFP == null)
                lstParametrosMotorIFP = new List<ParametrosMotorIFP>();

            if (lstParametrosMotorIFP.FindAll(p => p.cod_monedaIFP == cod_moneda
                && p.cod_tipo_temporalidadIFP == cod_temporalidad
                && p.fec_cotizacionIFP == fec_cotizacion).Count == 0)
            {
                bool cantidad_megas = false;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                ParametrosMotorIFP parametrosMotorIFP = servicioCotizador.ObtenerParametroGenerales(cod_temporalidad
                    , cod_moneda
                    , fec_cotizacion
                    , true
                    , tokenUsuario, ref cantidad_megas, usuario);
                //log.Debug("cantidad de megas: " + cantidad_megas);
                if (cantidad_megas)
                {
                    throw new Exception("Error en la carga de parámetros, contactarse con mesa de ayuda");
                }

                lstParametrosMotorIFP.Add(parametrosMotorIFP);
                HttpContext.Current.Session["ParametroMotorGeneral"] = lstParametrosMotorIFP;
            }
        }

        [WebMethod]
        public static SolicitudIFP ObtenerCotizacionesBeneficiarios(string solicitud)
        {
            return new SolicitudIFP
            {
                Id = solicitud,
                Cotizaciones = (List<CotizacionIFP>)HttpContext.Current.Session["RP_Cotizaciones"],
                Beneficiarios = (List<GrupoFamiliar>)HttpContext.Current.Session["RP_Beneficiarios"],
                CoberturasAdicionales = (List<CoberturaAdicional>)HttpContext.Current.Session["RP_CoberturasAdicionales"]
            };
        }

        [WebMethod]
        public static string validaCantSolicitudes(string monedaPU, string primaUnica)
        {
            string respuesta = "";

            if (monedaPU != "" && primaUnica != "")
            {
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                int Cantidad = servicioCotizador.CantidadSolicitudes(HttpContext.Current.Session["CUSPPIFP"].ToString(), monedaPU, Convert.ToDouble(primaUnica));
                HttpContext.Current.Session["CantidadSolicitudes"] = Cantidad;
                if (Cantidad >= Convert.ToInt32(ConfigurationManager.AppSettings["MaxCotizacionesIFP"]))
                {
                    respuesta = "TOPE";
                }
                else
                    respuesta = "OK";
            }

            return respuesta;
        }

        [WebMethod]
        public static List<Parametro> ObtenerPagoDobleCPeriodoGarantizado(double val_periodo_garantizado)
        {
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametroResultado = new List<Parametro>();

            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboIFPPagoDoble"];

            lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) < val_periodo_garantizado);

            if (lstParametroResultado.Count == 0)
            {
                lstParametroResultado = lstParametro.FindAll(m => m.Id == "0");
            }

            return lstParametroResultado;
        }

        [WebMethod]
        public static List<Parametro> ObtenerDiferimientoCPeriodoGarantizado(double val_periodo_garantizado)
        {
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametroResultado = new List<Parametro>();

            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboIFPDiferimiento"];

            if (val_periodo_garantizado <= 10)
            {
                lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) < (val_periodo_garantizado - 0.5));
            } else
            {
                lstParametroResultado = lstParametro.FindAll(m => Convert.ToDouble(m.Nombre) <= val_periodo_garantizado);
            }

            if (lstParametroResultado.Count == 0)
            {
                lstParametroResultado = lstParametro.FindAll(m => m.Id == "0");
            }

            return lstParametroResultado;
        }

        [WebMethod]
        public static string CargarTablaBeneficiariosPlan3IFP(string cuspp, List<GrupoFamiliar> beneficiarios)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    if (beneficiarios == null)
                    {
                        List<GrupoFamiliar> grupoFamiliar = new List<GrupoFamiliar>();
                        var control = (TablaBeneficiariosPlan3IFP)pagina.LoadControl("~/Controles/TablaBeneficiariosPlan3IFP.ascx");
                        List<GrupoFamiliar> grupoFamiliarBeneficiarios = servicioCotizador.ListarGrupoFamiliar(cuspp);

                        foreach (var ben in grupoFamiliarBeneficiarios)
                        {
                            if (ben.Parentesco.Id != Enums.Parentesco.Otros.StringValue() )
                            {
                                grupoFamiliar.Add(ben);
                            } 
                        }

                        List<GrupoFamiliar> beneficiariosSeleccionados = (List<GrupoFamiliar>)HttpContext.Current.Session["RP_Beneficiarios"];
                        if (beneficiariosSeleccionados == null)
                        {
                            grupoFamiliar.ForEach(g => { g.Seleccionado = true; });
                        }
                        else
                        {
                            grupoFamiliar.ForEach(g =>
                            {
                                beneficiariosSeleccionados.ForEach(benSeleccionado =>
                                {
                                    if (g.Id == benSeleccionado.IdGrupoFamiliar)
                                    {
                                        g.Seleccionado = true;
                                        g.ValPjeRenta = benSeleccionado.ValPjeRenta;
                                    }
                                });
                            });
                        }
                        control.Beneficiarios = grupoFamiliar;
                        
                        HttpContext.Current.Session["Beneficiarios"] = control.Beneficiarios;
                        pagina.Controls.Add(control);
                    }
                    /*
                    else
                    {
                        var control = (TablaRviBenefi)pagina.LoadControl("~/Controles/TablaRviBenefi.ascx");
                        control.Beneficiarios = beneficiarios;
                        control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                        pagina.Controls.Add(control);
                    }
                    */
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string RenderTablaBeneficiariosPlan3IFP(List<GrupoFamiliar> beneficiarios, List<GrupoFamiliar> beneficiariosSeleccionados)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                    List<GrupoFamiliar> grupoFamiliar = new List<GrupoFamiliar>();
                    var control = (TablaBeneficiariosPlan3IFP)pagina.LoadControl("~/Controles/TablaBeneficiariosPlan3IFP.ascx");

                    foreach (var ben in beneficiarios)
                    {
                        if (ben.Parentesco.Id != Enums.Parentesco.Otros.StringValue() )
                        {
                            grupoFamiliar.Add(ben);
                        }
                    }

                    if (beneficiariosSeleccionados == null)
                    {
                        grupoFamiliar.ForEach(g => { g.Seleccionado = true; });
                    }
                    else
                    {
                        grupoFamiliar.ForEach(g =>
                        {
                            beneficiariosSeleccionados.ForEach(benSeleccionado =>
                            {
                                if (g.Id == benSeleccionado.IdGrupoFamiliar)
                                {
                                    g.Seleccionado = true;
                                    g.ValPjeRenta = benSeleccionado.ValPjeRenta;
                                }
                            });
                        });
                    }
                    control.Beneficiarios = grupoFamiliar;

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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Respuesta ModificarPorcentaje(string tokenUsuario, int posicion, int porcentaje)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {

                Respuesta respuesta = new Respuesta();

                try
                {

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"];

                        if (lstGrupoFamiliar == null)
                            lstGrupoFamiliar = new List<GrupoFamiliar>();

                        lstGrupoFamiliar[posicion - 1].ValPjeRenta = porcentaje;

                        HttpContext.Current.Session["Beneficiarios"] = lstGrupoFamiliar;

                        double totalPorcentaje = 0;
                        for (int i = 0; i < lstGrupoFamiliar.Count; i++)
                        {
                            if (lstGrupoFamiliar[i].Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                            {
                                totalPorcentaje += lstGrupoFamiliar[i].ValPjeRenta;
                            }
                        }

                        respuesta.Estado = "OK";
                        respuesta.Mensaje = totalPorcentaje.ToString();
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
