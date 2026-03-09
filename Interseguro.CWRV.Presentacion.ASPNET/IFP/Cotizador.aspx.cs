using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using Microsoft.Reporting.WebForms;
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
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.IFP
{
    public partial class Cotizador : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(Cotizador));
        private static IServicioCWRV servicioCotizador;


        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    //Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionesIFP))
                    {
                        HFechaActual.Value = Convert.ToDateTime(DateTime.Today, new CultureInfo("es-PE")).ToString("dd/MM/yyyy");

                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            
                            CargarInformacionInicialPantalla();

                            Session["CantidadSolicitudes"] = 0;
                           
                            if (Session["ModSolModo"].ToString() != null)
                            {
                                HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();

                                List<GrupoFamiliar> grupoFamiliar = servicioCotizador.ListarGrupoFamiliar(HCUSPP_RP.Value);
                                HttpContext.Current.Session["Beneficiarios"] = grupoFamiliar;

                                //LabModSolAviso.Visible = false;

                                if (Session["ModSolModo"].ToString() == "M")
                                {
                                    if (DateTime.Today != Convert.ToDateTime(Session["fecCotizacion"], new CultureInfo("es-PE")))
                                    {
                                        if (((string)Session["RolAzman"]) != "JEF.RVI.OPE"
                                                && ((string)Session["RolAzman"]) != "AST.RVI.COM"
                                                && ((string)Session["RolAzman"]) != "JEF.VTA.LIM.RVI"
                                                && ((string)Session["RolAzman"]) != "JEF.VTA.PRO.RVI")
                                        {
                                            Session["ModSolModo"] = "CONS";
                                        }
                                    }
                                }

                                log.Info("Consultando ObtenerParametrosGenerales");
                                ObtenerParametrosGenerales(Session["TokenUsuario"].ToString(), "001", "T05", Convert.ToDateTime(DateTime.Today, new CultureInfo("es-PE")));

                                //Modificar
                                if (Session["ModSolModo"].ToString() == "M")
                                {

                                    // Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(Session["idSolicitud"].ToString());
                                    
                                    foreach (CotizacionIFP cotizacionIFP in solicitud.Cotizaciones)
                                    {
                                        
                                        if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN1.StringValue())
                                        {
                                            IFPPlan.Items.Remove(new ListItem("Ingreso Flexible con Devolución", "PLAN1"));
                                        }
                                        else if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN2.StringValue())
                                        {
                                            IFPPlan.Items.Remove(new ListItem("Ingreso Seguro con Devolución", "PLAN2"));
                                        }
                                        else if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN3.StringValue())
                                        {
                                            IFPPlan.Items.Remove(new ListItem("Ingreso Vitalicio Seguro", "PLAN3"));
                                        }

                                    }

                                    IFPSolicitud.Text = solicitud.Id;
                                    IFPMonedaPrimaUnica.SelectedIndex = IFPMonedaPrimaUnica.Items.IndexOf(IFPMonedaPrimaUnica.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    IFPPrimaUnica.Text = solicitud.PrimaUnica.ToString();
                                    IFPFechaCotizacion.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                    IFPFechaDevengue.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                    IFPFechaVigencia.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");
                                    HAgenteId.Value = solicitud.Agente.Id;

                                    Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    Session["RP_Cotizaciones"] = solicitud.Cotizaciones;
                                    Session["RP_CoberturasAdicionales"] = solicitud.CoberturasAdicionales;

                                    var cotizacionSeleccionada = solicitud.Cotizaciones.FindAll(p => p.EstadoCotizacion == "04" && p.IndSeleccionada == "S");
                                    if (cotizacionSeleccionada.Count > 0)
                                    {
                                        HSeleccionada.Value = "S";
                                        //LabModSolAviso.Visible = true;
                                    }

                                    HEstado.Value = solicitud.CodigoEstado.ToString();
                                    //LabMensaje.Text = solicitud.EstadoSolicitud.ToString();
                                    //ModSolEstadoPoliza_RP.Text = solicitud.EstadoPoliza;

                                    switch (solicitud.CodigoEstado.ToString())
                                    {
                                        case "1":
                                            //LabModSolAviso.CssClass = "grilla_info_verde";
                                            //LabMensaje.ForeColor = System.Drawing.Color.Green;
                                            //LabModSolAviso.Visible = true;
                                            break;
                                        case "2":
                                            //LabModSolAviso.CssClass = "grilla_info";
                                            //LabModSolAviso.Visible = true;
                                            break;
                                        case "3":
                                            //LabModSolAviso.CssClass = "grilla_info_rojo";
                                            //LabMensaje.ForeColor = System.Drawing.Color.Red;
                                            //LabModSolAviso.Visible = true;
                                            break;
                                        default:
                                            //LabModSolAviso.CssClass = "grilla_info";
                                            break;
                                    }

                                    //LabModLineaCausal.Visible = false;
                                    if (solicitud.CausalPoliza.Id != null)
                                    {
                                        //LabModLineaCausal.Visible = true;
                                        //ModSolCausalPoliza_RP.Text = solicitud.CausalPoliza.NombreLargo;
                                    }

                                    //ModSolTipoCambio_IFP.Text = solicitud.TipoCambio.ToString();
                                    //ModSolTipoCambioPanel.Visible = true;
                                    //ModSolLineaNumPoliza_RP.Visible = false;

                                    if (solicitud.NumeroPoliza != 0)
                                    {
                                        //ModSolLineaNumPoliza_RP.Visible = true;
                                        //ModSolNumPoliza_RP.Text = solicitud.NumeroPoliza.ToString();
                                    }

                                    ModSolModo.Value = Convert.ToString((Session["ModSolModo"]));
                                    HCopia.Value = "";

                                    if (Convert.ToString((Session["ModSolModo"])) == "C")
                                        HCopia.Value = "C";

                                    IFPPrimaUnica.Visible = true;
                                    //ManSolTipoSolicitud_RP.Value = "EXTRAOFICIAL";

                                    HBloqueo.Value = "TRUE";
                                    if (((string)Session["RolAzman"]) == "JEF.RVI.OPE")
                                    {
                                        HBloqueo.Value = "FALSE";
                                    }

                                    //Validando la fecha de cotizacion
                                    switch ((string)Session["RolAzman"])
                                    {
                                        case "JEF.RVI.OPE"://JefeOperaciones
                                        case "AST.RVI.OPE"://AsistenteOperaciones
                                            if (Session["ModSolModo"].ToString() != "CONS")
                                            {
                                                IFPFechaCotizacion.Enabled = true;
                                            }
                                            break;
                                        default:
                                            IFPFechaCotizacion.Enabled = false;
                                            break;
                                    }

                                    foreach (CotizacionIFP cotizacionIFP in solicitud.Cotizaciones)
                                    {
                                        log.Info("Consultando ObtenerParametrosGenerales");
                                        /*Inicio carga parametros*/
                                        ObtenerParametrosGenerales(Session["TokenUsuario"].ToString(), cotizacionIFP.Moneda.Id, cotizacionIFP.Temporalidad.Id, solicitud.FechaCotizacion.Value);
                                        /*Fin carga parametros*/
                                    }

                                }
                                else if (Session["ModSolModo"].ToString() == "C")
                                {
                                    //Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(Session["idSolicitud"].ToString());

                                    foreach (CotizacionIFP cotizacionIFP in solicitud.Cotizaciones)
                                    {
                                        if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN1.StringValue())
                                        {
                                            IFPPlan.Items.Remove(new ListItem("Ingreso Flexible con Devolución", "PLAN1"));
                                        }
                                        else if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN2.StringValue())
                                        {
                                            IFPPlan.Items.Remove(new ListItem("Ingreso Seguro con Devolución", "PLAN2"));
                                        }
                                        else if (cotizacionIFP.Plan.Id == Enums.Planes.PLAN3.StringValue())
                                        {
                                            IFPPlan.Items.Remove(new ListItem("Ingreso Vitalicio Seguro", "PLAN3"));
                                        }

                                        ///*Inicio carga parametros*/
                                        ObtenerParametrosGenerales(Session["TokenUsuario"].ToString(), cotizacionIFP.Moneda.Id, cotizacionIFP.Temporalidad.Id, ((DateTime)DateTime.Today));
                                        ///*Fin carga parametros*/
                                    }

                                    //IFPSolicitud.Text = solicitud.Id;
                                    IFPMonedaPrimaUnica.SelectedIndex = IFPMonedaPrimaUnica.Items.IndexOf(IFPMonedaPrimaUnica.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    IFPPrimaUnica.Text = solicitud.PrimaUnica.ToString();

                                    IFPFechaCotizacion.Text = ((DateTime)DateTime.Now).ToString("dd/MM/yyyy");
                                    IFPFechaDevengue.Text = ((DateTime)DateTime.Today.AddDays(-(DateTime.Today.Day - 1))).ToString("dd/MM/yyyy");
                                    //diasVigencia
                                    IFPFechaVigencia.Text = ((DateTime)DateTime.Now.AddDays(Convert.ToInt32(HDiasVigencia.Value))).ToString("dd/MM/yyyy");

                                    //Cambiando de estado, por defecto 02
                                    solicitud.Cotizaciones.ForEach(p =>
                                        p.EstadoCotizacion = "02"
                                      );

                                    HAgenteId.Value = solicitud.Agente.Id;

                                    Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    Session["RP_Cotizaciones"] = solicitud.Cotizaciones;

                                    //ModSolTipoCambio_IFP.Text = solicitud.TipoCambio.ToString();
                                    //ModSolTipoCambioPanel.Visible = true;

                                    //ModSolLineaNumPoliza_RP.Visible = false;

                                    solicitud.NumeroPoliza = 0;

                                }
                                //Consultar
                                else if (Session["ModSolModo"].ToString() == "CONS")
                                {
                                    //Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(Session["idSolicitud"].ToString());

                                    IFPSolicitud.Text = solicitud.Id;
                                    IFPMonedaPrimaUnica.SelectedIndex = IFPMonedaPrimaUnica.Items.IndexOf(IFPMonedaPrimaUnica.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    IFPPrimaUnica.Text = solicitud.PrimaUnica.ToString();

                                    IFPFechaCotizacion.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                    IFPFechaDevengue.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                    IFPFechaVigencia.Text = ((DateTime)solicitud.FechaVigencia).ToString("dd/MM/yyyy");

                                    HAgenteId.Value = solicitud.Agente.Id;

                                    Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    Session["RP_Cotizaciones"] = solicitud.Cotizaciones;
                                    Session["RP_CoberturasAdicionales"] = solicitud.CoberturasAdicionales;

                                    var cotizacionSeleccionada = solicitud.Cotizaciones.FindAll(p => p.EstadoCotizacion == "04" && p.IndSeleccionada == "S");
                                    if (cotizacionSeleccionada.Count > 0)
                                    {
                                        HSeleccionada.Value = "S";
                                        //LabModSolAviso.Visible = true;
                                    }

                                    //ModSolTipoCambio_IFP.Text = solicitud.TipoCambio.ToString();
                                    //ModSolTipoCambioPanel.Visible = false;
                                    if (solicitud.MonedaPrimaUnica.Id.ToString().Equals("002"))
                                    {
                                        //ModSolTipoCambioPanel.Visible = true;
                                    }

                                    //Bloqueando controles
                                    IFPMonedaPrimaUnica.Enabled = false;

                                    IFPFechaCotizacion.Enabled = false;
                                    IFPFechaDevengue.Enabled = false;
                                    IFPFechaVigencia.Enabled = false;

                                    IFPPrimaUnica.ReadOnly = true;
                                    //ModSolTipoCambio_IFP.ReadOnly = true;

                                    IFPMonedaPrimaUnica.CssClass = "formCombobox formComboboxReadOnly";

                                    IFPFechaCotizacion.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";
                                    IFPFechaDevengue.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";
                                    IFPFechaVigencia.CssClass = "fecha formTextbox formCalendar formTextboxReadOnly";

                                    //ModSolTipoCambio_IFP.CssClass = "formTextbox formTextboxReadOnly";
                                    IFPPrimaUnica.CssClass = "formTextbox formTextboxReadOnly";

                                    //ModSolLineaNumPoliza_RP.Visible = false;
                                    if (solicitud.NumeroPoliza != 0)
                                    {
                                        //ModSolLineaNumPoliza_RP.Visible = true;
                                        //ModSolNumPoliza_RP.Text = solicitud.NumeroPoliza.ToString();
                                    }

                                    HEstado.Value = solicitud.CodigoEstado.ToString();
                                    //LabMensaje.Text = solicitud.EstadoSolicitud.ToString(); //"Solicitud Seleccionada";
                                    //ModSolEstadoPoliza_RP.Text = solicitud.EstadoPoliza;
                                    switch (solicitud.CodigoEstado.ToString())
                                    {
                                        case "1":
                                            //LabModSolAviso.CssClass = "grilla_info_verde";
                                            //LabMensaje.ForeColor = System.Drawing.Color.Green;
                                            //LabModSolAviso.Visible = true;
                                            break;
                                        case "2":
                                            //LabModSolAviso.CssClass = "grilla_info";
                                            //LabModSolAviso.Visible = true;
                                            break;
                                        case "3":
                                            //LabModSolAviso.CssClass = "grilla_info_rojo";
                                            //LabMensaje.ForeColor = System.Drawing.Color.Red;
                                            //LabModSolAviso.Visible = true;
                                            break;
                                        default:
                                            //LabModSolAviso.CssClass = "grilla_info";
                                            break;
                                    }

                                    //LabModLineaCausal.Visible = false;
                                    if (solicitud.CausalPoliza.Id != null)
                                    {
                                        //LabModLineaCausal.Visible = true;
                                        //ModSolCausalPoliza_RP.Text = solicitud.CausalPoliza.NombreLargo;
                                    }

                                }
                                //Nuevo
                                else if (Session["ModSolModo"].ToString() == "N")
                                {
                                    HSeleccionada.Value = "N";
                                    HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();

                                    IFPFechaCotizacion.Text = ((DateTime)DateTime.Now).ToString("dd/MM/yyyy");
                                    IFPFechaDevengue.Text = ((DateTime)DateTime.Today.AddDays(-(DateTime.Today.Day - 1))).ToString("dd/MM/yyyy");

                                    IFPFechaVigencia.Text = ((DateTime)DateTime.Now.AddDays(Convert.ToInt32(HDiasVigencia.Value))).ToString("dd/MM/yyyy");

                                    //ModSolLineaNumPoliza_RP.Visible = false;
                                    HEstado.Value = "0";

                                }
                                
                                ModSolModo.Value = Convert.ToString((Session["ModSolModo"]));
                                HCopia.Value = "";

                                if (Convert.ToString((Session["ModSolModo"])) == "C")
                                    HCopia.Value = "C";

                                IFPPrimaUnica.Visible = true;
                                //ManSolTipoSolicitud_RP.Value = "EXTRAOFICIAL";

                                HBloqueo.Value = "TRUE";
                                if (((string)Session["RolAzman"]) == "JEF.RVI.OPE"
                                        || (string)Session["RolAzman"] == "AST.RVI.COM"
                                        || (string)Session["RolAzman"] == "JEF.VTA.LIM.RVI"
                                        || (string)Session["RolAzman"] == "JEF.VTA.PRO.RVI")
                                {
                                    HBloqueo.Value = "FALSE";
                                }

                                ListarPorcentajeDevolucion(IFPFechaCotizacion.Text, "");

                                //Validando la fecha de cotizacion
                                switch ((string)Session["RolAzman"])
                                {
                                    case "JEF.RVI.OPE"://JefeOperaciones
                                    case "AST.RVI.OPE"://AsistenteOperaciones
                                        if (Session["ModSolModo"].ToString() != "CONS")
                                        {
                                            IFPFechaCotizacion.Enabled = true;
                                        }
                                        break;
                                    default:
                                        IFPFechaCotizacion.Enabled = false;
                                        break;
                                }

                            }
                            else
                            {
                                Response.Redirect("../RentaIFP/Cotizador.aspx");
                            }
                        }
                        else
                        {
                            if (IFPPrimaUnica.Text != String.Empty) IFPPrimaUnica.Text = Convert.ToDouble(IFPPrimaUnica.Text, new CultureInfo("es-PE")).ToString();
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
                    HBloqueo.Value = "TRUE";
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

            CargarCombobox(IFPMonedaPrimaUnica, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.MonedaIFP]);

            Session["ComboSexo"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo];

            CargarComboboxNuevosDatos(IFPPlan, "PLANRP");
            
            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
            {
                IFPPlan.Items.Clear();
                IFPPlan.Items.Add(new ListItem("Ingreso Seguro con Devolución", "PLAN2"));
                IFPPlan.SelectedIndex = 0;
                IFPPlan.Enabled = false;
                IFPPlan.CssClass = "formCombobox formComboboxReadOnly";
            }

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaParametro = servicioCotizador.ObtenerComboboxIFP();

            //Parametros IFP
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

            Session["ListaDevolucionSob"] = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.PorcentajeDev];
            Session["ComboIFPDevSobrevivencia"] = Session["ListaDevolucionSob"];

            Session["ComboTramoEscalonada"] = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.PorcentajeEscalon];

            Session["ComboIFPMoneda"] = (List<Parametro>)listaParametro[(int)Enums.ComboboxIFP.Moneda];
            
            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;

            List<Parametro> lstDiasVigencia = new List<Parametro>();
            lstDiasVigencia = servicioCotizador.ObtenerParametrosPorTabla("PLUS");
            if (lstDiasVigencia.Count() > 0)
            {
                HDiasVigencia.Value = lstDiasVigencia[0].Valor_1;
            }

            IFPDNI.Text = Session["DNIIFP"].ToString();
            IFPNombres.Text = Session["NombresIFP"].ToString();
            IFPApellidos.Text = Session["ApellidosIFP"].ToString();

            //TabCotizacionesLeyenda_RP.Visible = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoEspeciales)) ? true : false;
            
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

        private static void ObtenerParametrosGenerales(string tokenUsuario, string codigoMoneda, string codigoTemporalidad, DateTime fechaCotizacion)
        {
            List<ParametrosMotorIFP> lstParametrosMotorIFP = (List<ParametrosMotorIFP>)HttpContext.Current.Session["ParametroMotorGeneral"];
            string usuario = (string)HttpContext.Current.Session["Usuario"].ToString();

            if (lstParametrosMotorIFP == null)
                lstParametrosMotorIFP = new List<ParametrosMotorIFP>();

            if (lstParametrosMotorIFP.FindAll(p => p.cod_monedaIFP == codigoMoneda
                && p.cod_tipo_temporalidadIFP == codigoTemporalidad
                && p.fec_cotizacionIFP == fechaCotizacion).Count == 0)
            {
                bool cantidadMegas = false;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                ParametrosMotorIFP parametrosMotorIFP = servicioCotizador.ObtenerParametroGenerales(
                    codigoTemporalidad
                    ,codigoMoneda
                    ,fechaCotizacion
                    ,true
                    ,tokenUsuario
                    ,ref cantidadMegas
                    ,usuario);

                if (cantidadMegas)
                {
                    throw new Exception("Error en la carga de parámetros, contactarse con mesa de ayuda");
                }

                lstParametrosMotorIFP.Add(parametrosMotorIFP);
                HttpContext.Current.Session["ParametroMotorGeneral"] = lstParametrosMotorIFP;
            }
        }


        [WebMethod]
        public static List<Parametro> ListarPorcentajeDevolucion(string fechaCotizacion, string plan)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var listaPorcentajeDevolucion = (List<Parametro>)HttpContext.Current.Session["ListaDevolucionSob"];

                    DateTime valFechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE"));

                    var DevolucionSobrevivencia = listaPorcentajeDevolucion.FindAll(p => p.FecInicioVigencia <= valFechaCotizacion
                                            && p.FecFinVigencia >= valFechaCotizacion);

                    DevolucionSobrevivencia = DevolucionSobrevivencia.Where(p => p.Valor_2.Contains(plan)).ToList();

                    HttpContext.Current.Session["ComboIFPDevSobrevivencia"] = DevolucionSobrevivencia;

                    return DevolucionSobrevivencia;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }


        [WebMethod]
        public static string validarCantidadSolicitudes(string monedaPrimaUnica, string primaUnica)
        {
            string respuesta = string.Empty;

            if (monedaPrimaUnica != "" && primaUnica != "")
            {
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                int Cantidad = servicioCotizador.CantidadSolicitudes(HttpContext.Current.Session["CUSPPIFP"].ToString(), monedaPrimaUnica, Convert.ToDouble(primaUnica));
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
        public static SolicitudIFP CrearDatosSolicitud(string tokenUsuario)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudIFP sol = new SolicitudIFP();

                sol.FechaSolicitud = DateTime.Now;
                sol.Cotizaciones = new List<CotizacionIFP>();

                HttpContext.Current.Session["idMonedaFondo"] = Enums.Moneda.Soles.StringValue();

                return sol;
            }
        }


    }
}