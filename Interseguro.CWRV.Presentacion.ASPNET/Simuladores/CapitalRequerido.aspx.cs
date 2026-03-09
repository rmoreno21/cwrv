using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Globalization;

//<SOLINI25621>
using System.Web.Services;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using System.IO;
using System.Xml.Serialization;
using System.Xml.Linq;
using Microsoft.Reporting.WebForms;
//<SOLFIN25621>

namespace Interseguro.CWRV.Presentacion.ASPNET.Simuladores
{
    public partial class CapitalRequerido : System.Web.UI.Page
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
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SimuladorCapitalRequerido))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                            LimpiarFormularios();
                            CargarInformacionPredeterminada();

                            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
                            {
                                BusAfiCUSPP.Text = Session["CUSPP"].ToString();
                                BusAfiBuscar_Click(sender, e);
                            }
                            else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                            {
                                BusAfiNroSolicitud.Text = Session["NroSolicitud"].ToString();
                                BusAfiBuscar_Click(sender, e);
                            }
                        }
                        else
                        {
                            // Si es postback cargar la tabla de beneficiarios desde el Textbox del CUSPP
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(BusAfiNroSolicitud.Text, BusAfiCUSPP.Text, "", "", "");

                            TablaGrupoFamiliar.Consentimiento = afiliado.Consentimiento;
                            //<SOLINI25621>
                            TablaGrupoFamiliar.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar)) ? true : false;
                            //<SOLFIN25621>
                            TablaGrupoFamiliar.Beneficiarios = servicioCotizador.ListarGrupoFamiliar(afiliado.CUSPP);
                        }

                        HMaxRegCalculoCapital.Value = ConfigurationManager.AppSettings["MaxCalculoCapital"];
                        if (HMaxRegCalculoCapital.Value == "")
                        {
                            HMaxRegCalculoCapital.Value = "0";
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.SimuladorCapitalRequerido.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error("Error de comunicación", ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
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
            IdSimulador.Value = ((int)Enums.OpcionesSistema.SimuladorCapitalRequerido).ToString();

            if (Session["Consentimiento"] != null)
                FormularioBusqueda.Visible = (bool)Session["Consentimiento"];

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
                InhabilitarControl(BusAfiBuscar);
                PerBusAfiBuscar.Value = "0";
            }

            //<SOLINI25621>
            // Permisos Insertar Grupo Familiar
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
            {
                PerNuevoBeneficiario.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevoBeneficiario);
                PerNuevoBeneficiario.Value = "0";
            }

            // Permisos Exportar PDF
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudExportarPDF))
            {
                PerbtnImprimir.Value = "1";
            }
            else
            {
                InhabilitarControl(btnImprimir);
                PerbtnImprimir.Value = "0";
            }

            // Permisos Exportar PDF
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudEnviarCorreo))
            {
                PerbtnEnviarEmail.Value = "1";
            }
            else
            {
                InhabilitarControl(btnEnviarEmail);
                PerbtnEnviarEmail.Value = "0";
            }

            //<SOLFIN25621>

            //<SOLINI25621>

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            CargarCombobox(Temporalidad, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Temporalidad]);
            //CargarCombobox(Moneda, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda]);


            Moneda.Items.Add(new ListItem("«Seleccione»", "-"));
            Session["ComboMoneda"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda];

            PeriodoGarantizado.Items.Add(new ListItem("«Seleccione»", "-"));

            //List<Parametro> comboPeriodoGarantizado = new List<Parametro>();
            //comboPeriodoGarantizado.Add(new Parametro { Id = "0", Glosa = "0" });
            //comboPeriodoGarantizado.Add(new Parametro { Id = "5", Glosa = "5" });
            //comboPeriodoGarantizado.Add(new Parametro { Id = "10", Glosa = "10" });
            //comboPeriodoGarantizado.Add(new Parametro { Id = "15", Glosa = "15" });
            //comboPeriodoGarantizado.Add(new Parametro { Id = "20", Glosa = "20" });
            //comboPeriodoGarantizado.Add(new Parametro { Id = "25", Glosa = "25" });
            //Session["ComboPeriodoGarantizado"] = comboPeriodoGarantizado;

            //CargarCombobox(PeriodoGarantizado, comboPeriodoGarantizado);
            //<SOLFIN25621>


            List<Parametro> comboTipoRenta = new List<Parametro>();
            comboTipoRenta.Add(new Parametro { Id = "RVI", Glosa = "Renta Vitalicia" });
            //comboTipoRenta.Add(new Parametro { Id = "RPV", Glosa = "Renta Privada" });

            //Session["ComboPeriodoGarantizado"] = comboPeriodoGarantizado;

            CargarCombobox(TipoRenta, comboTipoRenta);
        }


        private void CargarInformacionPredeterminada()
        {
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

        private void LimpiarFormularios()
        {
            CUSPP.Text = String.Empty;
            PensionRequerida.Text = "";
            //AnhosJubilarse.SelectedIndex = AnhosJubilarse.Items.IndexOf(AnhosJubilarse.Items.FindByValue(ConfigurationManager.AppSettings["EdadMinJubilacion"]));
            //Inflacion.Text = String.Empty;
            //PromedioRentabilitad.Text = String.Empty;
            //TipoCambio.Text = String.Empty;
            //TasaAjuste.Text = String.Empty;

            //AnhosJubilarse.CssClass = AnhosJubilarse.CssClass.Replace(" formComboboxError", String.Empty);
            //Inflacion.CssClass = Inflacion.CssClass.Replace(" formTextboxError", String.Empty);
            //PromedioRentabilitad.CssClass = PromedioRentabilitad.CssClass.Replace(" formTextboxError", String.Empty);
            //TipoCambio.CssClass = TipoCambio.CssClass.Replace(" formTextboxError", String.Empty);
            //TasaAjuste.CssClass = TasaAjuste.CssClass.Replace(" formTextboxError", String.Empty);
        }

        private bool ValidarBusquedaAfiliados()
        {
            bool esCorrecto = true;
            List<string> errores = new List<string>();

            MCMEstado.Value = "0";
            BusAfiNroSolicitud.CssClass = "formTextbox";
            BusAfiCUSPP.CssClass = "formTextbox";

            bool solicitud = (BusAfiNroSolicitud.Text.Trim().Length > 0) ? true : false;
            bool cuspp = (BusAfiCUSPP.Text.Trim().Length > 0) ? true : false;

            if (!(solicitud | cuspp))
            {
                errores.Add("Debe ingresar un criterio de búsqueda.");
                BusAfiNroSolicitud.CssClass = "formTextbox formTextboxError";
                BusAfiCUSPP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }

            if (solicitud & cuspp)
            {
                errores.Add("Sólo debe ingresar un criterio de búsqueda.");
                BusAfiNroSolicitud.CssClass = "formTextbox formTextboxError";
                BusAfiCUSPP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }

            if (cuspp && BusAfiCUSPP.Text.Trim().Length != 12)
            {
                errores.Add("El <strong>CUSPP</strong> debe contener 12 caracteres.");
                BusAfiCUSPP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }

            if (!esCorrecto)
            {
                MCMMensaje.Text = Utilitarios.FormatearError(errores);
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Validacion.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                MCMEstado.Value = "1";
                return false;
            }
            HttpContext.Current.Session["CapitalRequerido"] = null;
            return true;
        }

        protected void BusAfiBuscar_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
                    {
                        if (ValidarBusquedaAfiliados())
                        {
                            LimpiarFormularios();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(BusAfiNroSolicitud.Text, BusAfiCUSPP.Text, "", "", "");

                            log.Info("Usuario realizó búsqueda de afiliados por "
                                + ((BusAfiNroSolicitud.Text.Trim().Length != 0)
                                ? ("Solicitud [" + BusAfiNroSolicitud.Text.ToUpper() + "]")
                                : ("CUSPP [" + BusAfiCUSPP.Text.ToUpper() + "]")) + ".");

                            if (afiliado != null)
                            {
                                // Guardar los datos del afiliado para generarlo como beneficiario en caso de que no exista
                                servicioCotizador.ActualizarAfiliado(afiliado);

                                // MasterPage
                                Panel cabecera, cabeceraProtegida;

                                cabecera = (Panel)Master.FindControl("CabeceraSuperior");
                                cabeceraProtegida = (Panel)Master.FindControl("CabeceraSuperiorProtegida");

                                Session["Consentimiento"] = afiliado.Consentimiento;
                                if (!afiliado.Consentimiento)
                                {
                                    // MastePage
                                    cabecera.Visible = false;
                                    cabeceraProtegida.Visible = true;

                                    // Datos del Afiliado
                                    FormularioBusqueda.Visible = false;
                                    LineaCUSPP.Visible = false;
                                    afiliado.ApellidoPaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoPaterno);
                                    afiliado.ApellidoMaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoMaterno);
                                    afiliado.Nombre = Utilitarios.EnmascararNombre(afiliado.Nombre);
                                    LineaNacimiento.Visible = false;
                                    LineaCorreoElectronico.Visible = false;
                                    //LineaListaCIC.Visible = true;
                                    LineaAFP.Visible = false;
                                    //GrupoDireccion.Visible = false;
                                    //GrupoTelefono.Visible = false;
                                    //GrupoEmpresa.Visible = false;

                                    //ContenedorGuardar.Visible = false;

                                    // Grupo Familiar
                                    //ModGruFamLineaApellidos.Visible = false;
                                    //ModGruFamLineaNombres.Visible = false;
                                    //ModGruFamLineaIdentificacion.Visible = false;
                                    //ModGruFamCargando.Height = 130;

                                    // Solicitudes
                                    //ModSolSaldoCIC.Visible = false;
                                    //ModSolListaCIC.Visible = true;
                                }
                                else
                                {
                                    // MastePage
                                    cabecera.Visible = true;
                                    cabeceraProtegida.Visible = false;

                                    // Datos del Afiliado
                                    //LineaListaCIC.Visible = false;

                                    // Solicitudes
                                    //ModSolSaldoCIC.Visible = true;
                                    //ModSolListaCIC.Visible = false;
                                }
                                //<SRIFIN06326>

                                // Validar si el usuario tiene permiso para visualizar los datos del afiliado
                                if (((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == afiliado.Agente.Id))
                                {
                                    if (BusAfiCUSPP.Text.Trim().Length > 0)
                                    {
                                        Session["CUSPP"] = BusAfiCUSPP.Text;
                                        Session["NroSolicitud"] = null;
                                    }
                                    else if (BusAfiNroSolicitud.Text.Trim().Length > 0)
                                    {
                                        Session["CUSPP"] = null;
                                        Session["NroSolicitud"] = BusAfiNroSolicitud.Text;
                                    }

                                    // Datos principales
                                    DatosAfiliado.Visible = true;
                                    CUSPP.Text = afiliado.CUSPP.Trim();
                                    //HBusAfiCUSPP.Value = CUSPP.Text;
                                    Nombre.Text = String.Format("{0} {1}, {2}", afiliado.ApellidoPaterno.Trim(), afiliado.ApellidoMaterno.Trim(), afiliado.Nombre.Trim());
                                    FechaNacimiento.Text = afiliado.FechaNacimiento.Value.ToString("dd/MM/yyyy");
                                    string sexo = "Sin información";
                                    switch (afiliado.Sexo.ToString())
                                    {
                                        case "M":
                                            sexo = "Masculino";
                                            break;
                                        case "F":
                                            sexo = "Femenino";
                                            break;
                                    }
                                    Sexo.Text = sexo;
                                    CorreoElectronico.Text = afiliado.CorreoElectronico;
                                    CorreoElectronicoRegistrado.Value = afiliado.CorreoElectronico;
                                    HCategoria.Value = afiliado.Categoria.Id.ToString();
                                    HAFP.Value = afiliado.AFP.Id.ToString();
                                    AFP.Text = afiliado.AFP.Nombre;
                                    Session["Vendedor"] = afiliado.Agente.Id;
                                    Session["Cartera"] = afiliado.Agente.IdCartera;

                                    Categoria.Text = afiliado.Categoria.Nombre;
                                    Agente.Text = afiliado.Agente.Nombre;

                                    HSexo.Value = afiliado.Sexo.ToString();
                                    HApellidoMaterno.Value = afiliado.ApellidoMaterno;
                                    HApellidoPaterno.Value = afiliado.ApellidoPaterno;
                                    HNombres.Value = afiliado.Nombre;

                                    SaldoCIC.Text = afiliado.SaldoCIC.ToString();
                                    // Direcciones
                                    //NuevaDireccion.Visible = true;

                                    // Teléfonos
                                    //NuevoTelefono.Visible = true;

                                    // Datos de empresa
                                    //NombreEmpresa.Text = afiliado.NombreEmpresa.Trim();
                                    //DireccionEmpresa.Text = afiliado.DireccionEmpresa.Trim();

                                    // Botón Guardar Afiliado
                                    //if (afiliado.Consentimiento)
                                    //    ContenedorGuardar.Visible = true;
                                    //else
                                    //    ContenedorGuardar.Visible = false;

                                    // Grupo Familiar
                                    //<SOLINI25621>
                                    TablaGrupoFamiliar.Consentimiento = afiliado.Consentimiento;
                                    TablaGrupoFamiliar.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar)) ? true : false;
                                    NuevoBeneficiario.Visible = true;
                                    //<SOLFIN25621>

                                    DatosGrupoFamiliar.Visible = true;

                                    //<INIGTI_753>

                                    List<string> lstNoParentesco = CargarNoParentescos();

                                    TablaGrupoFamiliar.Beneficiarios = servicioCotizador.ListarGrupoFamiliar(afiliado.CUSPP).Where(p => !(lstNoParentesco.Contains(p.Id.ToString()))).ToList();

                                    //<FINGTI_753>
                                    HttpContext.Current.Session["Beneficiarios"] = TablaGrupoFamiliar.Beneficiarios;

                                    // Solicitudes
                                    DatosSimulacion.Visible = true;

                                    ModEnvCorPara.Text = afiliado.CorreoElectronico;

                                    //List<Ciudad> listaCiudades = new List<Ciudad>();
                                    //Ciudad ciudad = servicioCotizador.ObtenerDatosCiudad(afiliado.CiudadEmpresa.Id);
                                    //if (ciudad != null)
                                    //{
                                    //    listaCiudades.Add(ciudad);
                                    //}
                                    //CargarCombobox(CiudadEmpresa, listaCiudades);
                                    //CiudadEmpresa.SelectedIndex = CiudadEmpresa.Items.IndexOf(CiudadEmpresa.Items.FindByValue(afiliado.CiudadEmpresa.Id));

                                    //List<Comuna> listaComunas = new List<Comuna>();
                                    //Comuna comuna = servicioCotizador.ObtenerDatosComuna(afiliado.ComunaEmpresa.Id);
                                    //if (comuna != null)
                                    //{
                                    //    listaComunas.Add(comuna);
                                    //}
                                    //CargarCombobox(ComunaEmpresa, listaComunas);
                                    //ComunaEmpresa.SelectedIndex = ComunaEmpresa.Items.IndexOf(ComunaEmpresa.Items.FindByValue(afiliado.ComunaEmpresa.Id));

                                    //TelefonoEmpresa.Text = afiliado.TelefonoEmpresa;

                                    //ModSolTipoPension.SelectedIndex = ModSolTipoPension.Items.IndexOf(ModSolTipoPension.Items.FindByValue("V"));

                                    //HttpContext.Current.Session["indConsentimiento"] = afiliado.Consentimiento;  //JY

                                }
                                else
                                {
                                    List<String> errores = new List<String>();
                                    errores.Add("Cliente no pertenece a su cartera de ventas. Verifique.");
                                    LimpiarFormularios();
                                    MCMMensaje.Text = Utilitarios.FormatearError(errores);
                                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    MCMEstado.Value = "1";
                                }
                            }
                            else
                            {
                                List<String> errores = new List<String>();
                                if (BusAfiNroSolicitud.Text.Trim().Length > 0)
                                {
                                    errores.Add("Solicitud N° <strong>" + BusAfiNroSolicitud.Text.ToUpper() + "</strong> no se encuentra registrada. Verifique.");
                                }
                                else
                                {
                                    errores.Add("CUSPP <strong>" + BusAfiCUSPP.Text.ToUpper() + "</strong> no se encuentra registrado. Verifique.");
                                }
                                LimpiarFormularios();
                                MCMMensaje.Text = Utilitarios.FormatearError(errores);
                                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Informacion.StringValue();
                                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Informacion.StringValue();
                                MCMEstado.Value = "1";
                            }
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.DatosAfiliadoConsultar.StringValue()));
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstado.Value = "1";
                    }
                }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
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

        private List<string> CargarNoParentescos()
        {
            List<string> lstNoParentesco = new List<string>();

            lstNoParentesco.Add(Enums.Parentesco.Nieto.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Otros.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Sobrino.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Hermano.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Primo.StringValue());

            return lstNoParentesco;
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


        //<SOLINI25621>
        [WebMethod]
        public static List<Moneda> ObtenerMoneda(string tiporenta)
        {
            List<Moneda> monedas = new List<Moneda>();
            monedas.Add(new Moneda { Id = "-", Nombre = "«Seleccione»" });
            if (tiporenta == "RVI")
            {
                List<Parametro> combobox = new List<Parametro>();
                combobox = (List<Parametro>)HttpContext.Current.Session["ComboMoneda"];

                foreach (Parametro item in combobox)
                {
                    if (item.Id != "002")
                    {
                        monedas.Add(new Moneda { Id = item.Id, Nombre = item.Glosa });
                    }
                }
            }
            else if (tiporenta == "RPV")
            {
                List<Parametro> combobox = new List<Parametro>();
                combobox = (List<Parametro>)HttpContext.Current.Session["ComboMoneda"];

                foreach (Parametro item in combobox)
                {
                    monedas.Add(new Moneda { Id = item.Id, Nombre = item.Glosa, Simbolo = item.Nombre });
                }
            }


            return monedas;
        }

        [WebMethod]
        public static List<Parametro> ObtenerPeriodoGarantizado(string tiporenta, string temporalidad)
        {
            List<Parametro> parametros = new List<Parametro>();
            parametros.Add(new Parametro { Id = "-", Glosa = "«Seleccione»" });
            if (tiporenta == "RVI")
            {
                parametros.Add(new Parametro { Id = "0", Glosa = "0" });
                parametros.Add(new Parametro { Id = "10", Glosa = "10" });
                parametros.Add(new Parametro { Id = "15", Glosa = "15" });

            }
            else if (tiporenta == "RPV")
            {
                if (temporalidad != "0")
                {
                    int anho = 0;
                    parametros.Add(new Parametro { Id = "0", Glosa = "0" });
                    parametros.Add(new Parametro { Id = "5", Glosa = "5" });
                    parametros.Add(new Parametro { Id = "10", Glosa = "10" });
                    anho = 10;
                    switch (temporalidad)
                    {
                        case "T15":
                            for (var i = 1; i <= 1; i++)
                            {
                                anho = anho + 5;
                                parametros.Add(new Parametro { Id = anho.ToString(), Glosa = anho.ToString() });
                            }
                            break;
                        case "T20":
                            for (var i = 1; i <= 2; i++)
                            {
                                anho = anho + 5;
                                parametros.Add(new Parametro { Id = anho.ToString(), Glosa = anho.ToString() });
                            }
                            break;
                        case "T25":
                            for (var i = 1; i <= 3; i++)
                            {
                                anho = anho + 5;
                                parametros.Add(new Parametro { Id = anho.ToString(), Glosa = anho.ToString() });
                            }
                            break;
                        case "TVT":
                            for (var i = 1; i <= 3; i++)
                            {
                                anho = anho + 5;
                                parametros.Add(new Parametro { Id = anho.ToString(), Glosa = anho.ToString() });
                            }
                            break;
                            //parametros.Add(new Parametro { Id = "15", Glosa = "15" });
                            //break;
                    }
                }



                //                T10
                //T15
                //T20
                //T25
                //TVT
                //                HttpContext.Current.Session["ComboTemporalidad"];
                //List<MyClass> results = myClassList.FindAll(x => x.item1 == "abc");
                //parametros.Add(new Parametro { Id = "0", Glosa = "0" });
                //parametros.Add(new Parametro { Id = "5", Glosa = "5" });
                //parametros.Add(new Parametro { Id = "10", Glosa = "10" });
                //parametros.Add(new Parametro { Id = "15", Glosa = "15" });
            }
            return parametros;
        }


        [WebMethod]
        public static Respuesta CalcularCapital(string tokenUsuario,
            string num_cuissp, TipoRenta tipoRenta, Moneda moneda, string periodo_garantizado,
            Temporalidad temporalidad, string id_grupo_familiar, string pension_requerida
            )
        {
            List<Dominio.Entidades.CapitalRequerido> lstCapitalRequerido = new List<Dominio.Entidades.CapitalRequerido>();
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Dominio.Entidades.CapitalRequerido capitalRequerido = new Dominio.Entidades.CapitalRequerido();
                Respuesta respuesta = new Respuesta();
                try
                {

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (HttpContext.Current.Session["CapitalRequerido"] != null)
                        {
                            lstCapitalRequerido = (List<Dominio.Entidades.CapitalRequerido>)HttpContext.Current.Session["CapitalRequerido"];
                        }

                        capitalRequerido.num_cuissp = num_cuissp;
                        capitalRequerido.TipoRenta = new TipoRenta { Id = tipoRenta.Id, Nombre = tipoRenta.Nombre };
                        capitalRequerido.Moneda = new Moneda { Id = moneda.Id, Nombre = moneda.Nombre };
                        capitalRequerido.periodo_garantizado = periodo_garantizado;
                        capitalRequerido.Temporalidad = new Temporalidad { Id = temporalidad.Id, Nombre = temporalidad.Nombre };//.temporalidad = temporalidad;
                        capitalRequerido.num_vendedor = Convert.ToInt32(HttpContext.Current.Session["Vendedor"]);
                        capitalRequerido.id_grupo_familiar = id_grupo_familiar;
                        capitalRequerido.pension_requerida = Convert.ToDouble(pension_requerida, new CultureInfo("es-PE"));//(float)Convert.ToDouble(pension_requerida);

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.CotizarCapitalRequerido(ref capitalRequerido);

                        //capitalRequerido.temporalidad = (capitalRequerido.TipoRenta.Id=="RVI") ? "Vitalicia" : capitalRequerido.temporalidad;
                        capitalRequerido.Temporalidad = (capitalRequerido.TipoRenta.Id == "RVI") ? new Temporalidad { Id = "TVT", Nombre = "Vitalicia" } : temporalidad;
                        if (respuesta.Estado == Constante.COD_OK)
                        {
                            if (capitalRequerido.error == 0)
                            {
                                lstCapitalRequerido.Add(capitalRequerido);
                                respuesta.Mensaje = Convert.ToString(lstCapitalRequerido.Count);
                            }
                            else
                            {
                                respuesta.Estado = "VALIDACION";
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>Cotización no alcanza mínimo requerido, por lo cual no se simula</strong></div>";
                            }
                        }
                        HttpContext.Current.Session["CapitalRequerido"] = lstCapitalRequerido;


                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");

                    }
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    //capitalRequerido.Respuesta = respuesta;
                }
                return respuesta;// lstCapitalRequerido;
            }

        }

        public static string GeneraXmlCapital(List<Dominio.Entidades.CapitalRequerido> lstCapital)
        {
            XDocument documento = new XDocument();
            documento.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement sols = new XElement("Calculos");
            foreach (Dominio.Entidades.CapitalRequerido capital in lstCapital)
            {
                sols.Add(
                    new XElement("Calculo",
                        new XElement("Num_Cuisp", capital.num_cuissp),
                        new XElement("Moneda", capital.Moneda.Nombre),
                        new XElement("PensionRequerida", capital.pension_requerida),
                        new XElement("TipoRenta", capital.TipoRenta.Nombre),
                        new XElement("Temporalidad", capital.Temporalidad.Nombre),
                        new XElement("PeriodoGarantizado", capital.periodo_garantizado),
                        new XElement("CapitalRequerido", capital.capital_requerido),
                        new XElement("id_grupo_familiar", capital.id_grupo_familiar),
                        new XElement("Cod_TipoRenta", capital.TipoRenta.Id),
                        new XElement("Cod_Moneda", capital.Moneda.Id),
                        new XElement("TipoCambio", capital.tipo_cambio),
                        new XElement("TasaSBS", capital.tasaSBS),
                        new XElement("Num_Vendedor", capital.num_vendedor)

                    )
                );
            }
            documento.Add(sols);
            return documento.ToString();
        }

        [WebMethod]
        public static string CargarTablaCapitalRequerido()
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    List<Dominio.Entidades.CapitalRequerido> lstCapitalRequerido = new List<Dominio.Entidades.CapitalRequerido>();
                    var pagina = new Page();
                    var control = (TablaCapitalRequerido)pagina.LoadControl("~/Controles/TablaCapitalRequerido.ascx");

                    if (HttpContext.Current.Session["CapitalRequerido"] != null)
                    {
                        lstCapitalRequerido = (List<Dominio.Entidades.CapitalRequerido>)HttpContext.Current.Session["CapitalRequerido"];
                        control.capitalRequerido = lstCapitalRequerido;

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
        public static Respuesta Imprimir()
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    List<Dominio.Entidades.CapitalRequerido> lstCapitalRequerido = new List<Dominio.Entidades.CapitalRequerido>();
                    if (HttpContext.Current.Session["CapitalRequerido"] != null)
                    {
                        lstCapitalRequerido = (List<Dominio.Entidades.CapitalRequerido>)HttpContext.Current.Session["CapitalRequerido"];
                        var xml = GeneraXmlCapital(lstCapitalRequerido);
                        HttpContext.Current.Session["wl_xml_calculo"] = xml;
                        HttpContext.Current.Session["AgenteReporte"] = Convert.ToString(HttpContext.Current.Session["Vendedor"]);
                        respuesta.Estado = Constante.COD_OK;
                    }
                    else
                    {
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                }
                catch (Exception ex)
                {
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = "Error";
                    respuesta.Icono = "error";
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }
                return respuesta;
            }
        }



        [WebMethod]
        public static CorreoElectronico CrearDatosCorreo(string tokenUsuario, string tipoCotizacion, string nombre, string apellidoPaterno, string apellidoMaterno, string sexo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                CorreoElectronico correo;
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudEnviarCorreo))
                        {
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {

                                string tipo = "1";
                                string xml_calculo = "";

                                if (Imprimir().Estado == "OK")
                                {
                                    xml_calculo = (string)HttpContext.Current.Session["wl_xml_calculo"];
                                }
                                else
                                {
                                    throw new Exception("Error al generar xml para enviar al reporte");
                                }


                                ReportViewer visorReporte = new ReportViewer();
                                visorReporte.ProcessingMode = ProcessingMode.Remote;
                                visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteCalculoCapital"];

                                ReportParameter p1 = new ReportParameter("wl_tipo", tipo);
                                ReportParameter p2 = new ReportParameter("wl_xml_calculo", xml_calculo);

                                log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteDetalleCotizacion"]));
                                log.Debug(String.Format("Parámetros del reporte: wl_tipo[{0}] wl_xml_calculo[{1}].",
                                    tipo, xml_calculo));
                                visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2 });
                                log.Debug(String.Format("Reporte para calculo [{0}] procesado.", xml_calculo));

                                string format = "PDF", mimeType, encoding, extension;
                                string[] streamids;
                                Warning[] warnings;

                                log.Debug(String.Format("Se va a exportar a formato PDF el reporte de calculo [{0}].", xml_calculo));
                                byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                                HttpContext.Current.Session["ArchivoPDF"] = bytes;
                                log.Debug(String.Format("Reporte para solicitud [{0}] exportado y almacenado en sesión de usuario.", xml_calculo));

                                SeccionCorreo config = (SeccionCorreo)ConfigurationManager.GetSection("correo");

                                correo = new CorreoElectronico
                                {
                                    De = (string)HttpContext.Current.Session["CorreoElectronico"],
                                    DeNombre = (string)HttpContext.Current.Session["NombreCompleto"],
                                    ParaNombre = String.Format("{0} {1}, {2}", apellidoPaterno, apellidoMaterno, nombre),
                                    Asunto = config.Asunto.Texto
                                                    .Replace("{NombreAfiliado}", nombre)
                                                    .Replace("{ApellidoPaternoAfiliado}", apellidoPaterno)
                                                    .Replace("{ApellidoMaternoAfiliado}", apellidoMaterno),
                                    Adjunto = "CalculoCapital.pdf (" + Utilitarios.FormatearBytes(bytes.Length, false) + ")",
                                    Mensaje = config.Mensaje.Texto
                                                    .Replace("{TratamientoAfiliado}", (sexo == "M") ? ("Sr.") : ("Sra."))
                                                    .Replace("{ApellidoPaternoAfiliado}", apellidoPaterno)
                                                    .Replace("{ApellidoMaternoAfiliado}", apellidoMaterno)
                                                    .Replace("{NombreAfiliado}", nombre)
                                                    .Replace("{TipoCotizacion}", tipoCotizacion)
                                                    .Replace("{NombreAgente}", (string)HttpContext.Current.Session["NombreCompleto"])
                                                    .Replace("la cotización", "el")
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
        public static Respuesta EnviarCorreoElectronico(string tokenUsuario, CorreoElectronico correo)
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
                                correo.Adjunto = "CalculoCapital.pdf";
                                correo.BinarioAdjunto = (byte[])HttpContext.Current.Session["ArchivoPDF"];
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.EnviarCorreoElectronico(correo);
                                //<SRIFIN20322>
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }
                return respuesta;
            }
        }


        [WebMethod]
        public static string TipoCambio()
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                string tipocambio = "";
                try
                {

                    List<Dominio.Entidades.CapitalRequerido> lstCapitalRequerido = new List<Dominio.Entidades.CapitalRequerido>();
                    if (HttpContext.Current.Session["CapitalRequerido"] != null)
                    {
                        lstCapitalRequerido = (List<Dominio.Entidades.CapitalRequerido>)HttpContext.Current.Session["CapitalRequerido"];

                        tipocambio = Convert.ToString(lstCapitalRequerido[lstCapitalRequerido.Count - 1].tipo_cambio);

                    }
                    else
                    {
                        tipocambio = "TOKEN";
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
                    throw (ex);
                }
                return tipocambio;
            }
        }


        //<SOLFIN25621>




    }


}
