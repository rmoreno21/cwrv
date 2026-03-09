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
    public partial class TelefonoAfiliado : System.Web.UI.Page
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
                            LimpiarFormularios();

                            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
                            {
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP"]));
                                ModTelModo.Value = Convert.ToString((Session["ModTelModo"]));
                                ModIdTelefono.Value = Convert.ToString((Session["idTelefono"]));

                            }
                            else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                            {
                                //Response.Redirect("Cotizador.aspx");
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP_PLUS"]));
                                ModTelModo.Value = Convert.ToString((Session["ModTelModo"]));
                                ModIdTelefono.Value = Convert.ToString((Session["idTelefono"]));
                            }
                            else
                            {
                                if ((Session["idTelefono"]).ToString().Length > 0)
                                {
                                    HCUSPP_RP.Value = Convert.ToString((Session["SHCUSPP"]));
                                    ModTelModo.Value = Convert.ToString((Session["ModTelModo"]));
                                    ModIdTelefono.Value = Convert.ToString((Session["idTelefono"]));
                                }
                                else
                                {
                                    Response.Redirect("Cotizador.aspx");
                                }
                            }
                        }
                        else
                        {
                            // if (SaldoCIC.Text != String.Empty) SaldoCIC.Text = Convert.ToDouble(SaldoCIC.Text, new CultureInfo("es-PE")).ToString();
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


        private void LimpiarFormularios()
        {
            // Datos principales

            // Solicitudes
            ////ModDirDireccion.Text = "";
            ////ModDirDepartamento.SelectedIndex = 0;
            ////ModDirCiudad.SelectedIndex = 0;
            ////ModDirComuna.SelectedIndex = 0;
            ////ModDirPrincipal.SelectedIndex = 0;

        }


        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            CargarCombobox(ModTelTipo, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Telefono]);
            ModTelPrincipal.Items.Add(new ListItem("«Seleccione»", "0"));
            ModTelPrincipal.Items.Add(new ListItem("Sí", "S"));
            ModTelPrincipal.Items.Add(new ListItem("No", "N"));
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

    }
}