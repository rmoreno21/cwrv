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

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivada
{
    public partial class NuevaCotizacion : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Cotizador));
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

                                if (Session["ModSolModo"].ToString() == "M")
                                {
                                    // Obtener la información desde base de datos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    SolicitudRP solicitud = servicioCotizador.ObtenerDatosSolicitudRP(Session["idSolicitud"].ToString(), Convert.ToDateTime(Session["fecCotizacion"], new CultureInfo("es-PE")));

                                    ModSolNroSolicitud_RP.Text = solicitud.Id;
                                    ModSolTemporalidad_RP.SelectedIndex = ModSolTemporalidad_RP.Items.IndexOf(ModSolTemporalidad_RP.Items.FindByValue(solicitud.Temporalidad.Id));
                                    ModSolMonedaPrimaUnica_RP.SelectedIndex = ModSolMonedaPrimaUnica_RP.Items.IndexOf(ModSolMonedaPrimaUnica_RP.Items.FindByValue(solicitud.MonedaPrimaUnica.Id));
                                    ModSolPrimaUnica_RP.Text = solicitud.PrimaUnica.ToString();
                                    ModSolFechaCotizacion_RP.Text = ((DateTime)solicitud.FechaCotizacion).ToString("dd/MM/yyyy");
                                    ModSolFechaDevengue_RP.Text = ((DateTime)solicitud.FechaDevengue).ToString("dd/MM/yyyy");
                                    ModSolDCOM_RP.Text = solicitud.PorcentajeDescuentoComision.ToString();
                                    Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
                                    Session["RP_Cotizaciones"] = solicitud.Cotizaciones;
                                }
                                else if (Session["ModSolModo"].ToString() == "N")
                                {
                                    HCUSPP_RP.Value = Session["CUSPP_RP"].ToString();
                                    HAFP_RP.Value = Session["AFP_RP"].ToString();
                                }
                                ModSolModo.Value = Convert.ToString((Session["ModSolModo"]));

                                ModSolPrimaUnica_RP.Visible = true;
                                ManSolTipoSolicitud_RP.Value = "EXTRAOFICIAL";
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
        public static SolicitudRP ObtenerCotizacionesBeneficiarios(string solicitud)
        {
            return new SolicitudRP
            {
                Id = solicitud,
                Cotizaciones = (List<CotizacionRP>)HttpContext.Current.Session["RP_Cotizaciones"],
                Beneficiarios = (List<GrupoFamiliar>)HttpContext.Current.Session["RP_Beneficiarios"]
            };
        }

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            CargarCombobox(ModSolTemporalidad_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Temporalidad]);
            CargarCombobox(ModSolMonedaPrimaUnica_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.MonedaRentaPrivada]);

            
            Session["ComboMoneda"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda];

            ////List<Parametro> comboModalidad = new List<Parametro>();
            ////comboModalidad.Add(new Parametro { Id = "I", Glosa = "I" });
            ////comboModalidad.Add(new Parametro { Id = "D", Glosa = "D" });
            ////comboModalidad.Add(new Parametro { Id = "I-RM", Glosa = "I-RM" });
            //////<SRIINI18360>
            ////comboModalidad.Add(new Parametro { Id = "I-RC", Glosa = "I-RC" });
            //////<SRIFIN18360>
            ////comboModalidad.Add(new Parametro { Id = "I-RB", Glosa = "I-RB" });
            ////Session["ComboModalidad"] = comboModalidad;

            ////List<Parametro> comboPeriodoDiferido = new List<Parametro>();
            ////comboPeriodoDiferido.Add(new Parametro { Id = "0", Glosa = "0" });
            ////comboPeriodoDiferido.Add(new Parametro { Id = "1", Glosa = "1" });
            ////comboPeriodoDiferido.Add(new Parametro { Id = "2", Glosa = "2" });
            //////<SRIINI18360>
            ////comboPeriodoDiferido.Add(new Parametro { Id = "3", Glosa = "3" });
            ////comboPeriodoDiferido.Add(new Parametro { Id = "4", Glosa = "4" });
            ////comboPeriodoDiferido.Add(new Parametro { Id = "5", Glosa = "5" });
            //////<SRIFIN18360>
            ////Session["ComboPeriodoDiferido"] = comboPeriodoDiferido;

            ////List<Parametro> comboPorcentajeRentas = new List<Parametro>();
            ////comboPorcentajeRentas.Add(new Parametro { Id = "0", Glosa = "0" });
            ////comboPorcentajeRentas.Add(new Parametro { Id = "50", Glosa = "50%" });
            ////Session["ComboPorcentajeRentas"] = comboPorcentajeRentas;

            List<Parametro> comboPeriodoGarantizado = new List<Parametro>();
            comboPeriodoGarantizado.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "5", Glosa = "5" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "10", Glosa = "10" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "15", Glosa = "15" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "20", Glosa = "20" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "25", Glosa = "25" });
            Session["ComboPeriodoGarantizado"] = comboPeriodoGarantizado;

            ////Session["ComboCapital"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Capital];

            
            //<SRIINI06326>
            //var montosCIC = servicioCotizador.ListarMontoCIC();
            //CargarCombobox(ListaCIC, montosCIC);
            //CargarCombobox(ModSolListaCIC, montosCIC);
            //<SRIFIN06326>

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            //LabModSolLineaACOMDCOM.Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);

            
            //<SRIINI10693>
            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;
            //<SRIFIN10693>

            //string fechaDevengue =  "01/" + DateTime.Today.AddMonths(1).Month.ToString("00") + "/" + DateTime.Today.AddMonths(1).Year.ToString() ;
            //ModSolFechaDevengue.Text = fechaDevengue;

            //ModSolFechaCotizacion.Text = DateTime.Today.ToString("dd/MM/yyyy") ;

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
    }
}