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
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
{
    public partial class Cotizador : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Cotizador));
        private static IServicioCWRV servicioCotizador;
        private bool estado_mensaje = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    HttpContext.Current.Session["Externo"] = false;
                    // Validar permisos
                    // if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionesIFP))
                    // {
                    if (!IsPostBack)
                    {
                        log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                        CargarInformacionInicialPantalla();
                        LimpiarFormularios();

                        // Variables de sesión
                        var rolAzman = HttpContext.Current.Session["RolAzman"];
                        var cuspp = Session["CUSPP"];
                        var numeroSolicitud = Session["NroSolicitud"];
                        var tipoDocumentoBusqueda = Session["TipoDocumentoBusqueda"];
                        var numeroDocumentoBusqueda = Session["NumeroDocumentoBusqueda"];
                        var shCuspp = Session["SHCUSPP"];

                        if ((string)rolAzman == Enums.RolAzman.AgenteExterno.StringValue())
                        {
                            cuspp = null;
                            numeroSolicitud = null;
                            Session["CUSPP"] = null;
                            Session["NroSolicitud"] = null;
                            BusAfiCUSPP_RP.Text = string.Empty;
                            BusAfiNroSolicitud_RP.Text = string.Empty;
                        }

                        if (cuspp != null && numeroSolicitud == null)
                        {
                            BusAfiCUSPP_RP.Text = cuspp.ToString();
                            BusAfiBuscar_RP_Click(sender, e);
                        }
                        else if (numeroSolicitud != null && cuspp == null)
                        {
                            BusAfiNroSolicitud_RP.Text = numeroSolicitud.ToString();
                            BusAfiBuscar_RP_Click(sender, e);
                        }
                        else
                        {
                            if ((string)rolAzman == Enums.RolAzman.AgenteExterno.StringValue())
                            {
                                if (tipoDocumentoBusqueda == null)
                                {
                                    if (shCuspp != null)
                                    {
                                        hcusppInteligo.Value = shCuspp.ToString();
                                        BusAfiBuscar2_RP_Click(sender, e);
                                    }
                                    else
                                    {
                                        TipoDocumentoBusqueda_RP.SelectedValue = "0";
                                        NumeroDocumentoBusqueda_RP.Text = string.Empty;
                                    }

                                }
                                else
                                {
                                    TipoDocumentoBusqueda_RP.SelectedValue = tipoDocumentoBusqueda.ToString();
                                    NumeroDocumentoBusqueda_RP.Text = numeroDocumentoBusqueda.ToString();
                                    BusAfiBuscar2_RP_Click(sender, e);
                                }
                            }
                            else
                            {
                                if (tipoDocumentoBusqueda == null)
                                {
                                    if (shCuspp != null)
                                    {
                                        hcusppInteligo.Value = shCuspp.ToString();
                                        BusAfiBuscar2_RP_Click(sender, e);
                                    }
                                    else
                                    {
                                        TipoDocumentoBusqueda_RP.SelectedValue = "0";
                                        NumeroDocumentoBusqueda_RP.Text = string.Empty;
                                    }

                                }
                                else
                                {
                                    TipoDocumentoBusqueda_RP.SelectedValue = tipoDocumentoBusqueda.ToString();
                                    NumeroDocumentoBusqueda_RP.Text = numeroDocumentoBusqueda.ToString();
                                    BusAfiBuscar2_RP_Click(sender, e);
                                }
                            }
                        }

                    }
                    else
                    {
                        manejoPestanasBusqueda();
                    }
                    //else
                    //{
                    //	// Se vuelve a formatear el monto sin separador de miles para que el plugin autoNumeric no falle
                    //	if (SaldoCIC_RP.Text != String.Empty) SaldoCIC_RP.Text = Convert.ToDouble(SaldoCIC_RP.Text, new CultureInfo("es-PE")).ToString();
                    //}
                    // }
                    // else
                    // {
                    //     log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                    //         Enums.OpcionesSistema.CotizacionesIFP.StringValue()));
                    //     Response.Redirect("~/Error/Permisos.aspx");
                    // }
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
            var usuario = HttpContext.Current.Session["Usuario"];
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            CargarCombobox(AFP_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Afp]);

            //69310
            CargarComboboxenBlanco(Categoria_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Categoria]);

            CargarComboboxenBlanco(Sexo_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);
            //INI.YRV
            CargarCombobox(TipoDocumentoBusqueda_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);
            CargarComboboxenBlanco(TipoDocumento_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);
            //FIN.YRV

            //CargarCombobox(CiudadEmpresa_RP, new List<Parametro>());
            //CargarCombobox(ComunaEmpresa_RP, new List<Parametro>());

            // string usuario = HttpContext.Current.Session["Usuario"].ToString();

            // Session["ListadoEstadoCivil"] = servicioCotizador.ListarEstadoCivil(usuario);
            // Session["ListadoProfesion"] = servicioCotizador.ListarProfesion(usuario);
            // Session["ListadoNacionalidad"] = servicioCotizador.ListarNacionalidad(usuario);

            // JArray listaDepartamentos = new JArray();
            // string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
            // var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
            // urlDepartamentos = string.Format(urlDepartamentos, usuario);
            // listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, usuario, listaDepartamentos);
            // Session["ListadoDepartamento"] = listaDepartamentos.ToObject<List<Departamento>>();

            CargarComboboxNuevosDatosenBlanco(EstadoCivil_RP, "EstadoCivil", usuario.ToString());

            Session["ComboMoneda"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda];

            List<Parametro> comboModalidad = new List<Parametro>();
            comboModalidad.Add(new Parametro { Id = "I", Glosa = "I" });
            comboModalidad.Add(new Parametro { Id = "D", Glosa = "D" });
            comboModalidad.Add(new Parametro { Id = "I-RM", Glosa = "I-RM" });
            comboModalidad.Add(new Parametro { Id = "I-RC", Glosa = "I-RC" });
            comboModalidad.Add(new Parametro { Id = "I-RB", Glosa = "I-RB" });
            Session["ComboModalidad"] = comboModalidad;

            List<Parametro> comboPeriodoDiferido = new List<Parametro>();
            comboPeriodoDiferido.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPeriodoDiferido.Add(new Parametro { Id = "1", Glosa = "1" });
            comboPeriodoDiferido.Add(new Parametro { Id = "2", Glosa = "2" });
            comboPeriodoDiferido.Add(new Parametro { Id = "3", Glosa = "3" });
            comboPeriodoDiferido.Add(new Parametro { Id = "4", Glosa = "4" });
            comboPeriodoDiferido.Add(new Parametro { Id = "5", Glosa = "5" });

            Session["ComboPeriodoDiferido"] = comboPeriodoDiferido;

            List<Parametro> comboPorcentajeRentas = new List<Parametro>();
            comboPorcentajeRentas.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPorcentajeRentas.Add(new Parametro { Id = "50", Glosa = "50%" });
            Session["ComboPorcentajeRentas"] = comboPorcentajeRentas;

            Session["ComboCapital"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Capital];

            List<Parametro> lstParametro = new List<Parametro>();
            lstParametro = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PagoEscalonado];
            LlenarPagoDoble(lstParametro);

            //Permisos Modal Búsqueda de Afiliados
            ValidarPermisos();

            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;

        }

        private void ValidarPermisos()
        {
            //inteligo
            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
            {
                PerBusAfiExaminarSolicitud_RP.Value = "1";
                hindPestaniaActiva.Value = "2";
                HttpContext.Current.Session["Externo"] = true;
            }
            else
            {
                //if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteLima.StringValue()
                //        || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteProvincia.StringValue())
                //{

                //}
                //else
                //{

                //}

                if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.BusquedaAfiliadoConsultar))
                {
                    PerBusAfiExaminarSolicitud_RP.Value = "1";
                }
                else
                {
                    InhabilitarControl(BusAfiExaminarSolicitud_RP);
                    InhabilitarControl(ModBusAfiApellidoPaterno_RP);
                    InhabilitarControl(ModBusAfiApellidoMaterno_RP);
                    InhabilitarControl(ModBusAfiNombres_RP);
                    InhabilitarControl(ModBusAfiBuscar_RP);
                    PerBusAfiExaminarSolicitud_RP.Value = "0";
                }

            }

            //Permisos Consultar Afiliado
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
            {
                PerBusAfiBuscar_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(BusAfiNroSolicitud_RP);
                InhabilitarControl(BusAfiCUSPP_RP);
                InhabilitarControl(BusAfiBuscar_RP);
                PerBusAfiBuscar_RP.Value = "0";
            }

            //Permisos Actualizar Afiliado
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoActualizar))
            {
                PerGuardar_RP.Value = "1";
                //if ((string)HttpContext.Current.Session["RolAzman"] != Enums.RolAzman.AgenteExterno.StringValue())
                //{
                //    Guardar_RP.Visible = false;
                //}
            }
            else
            {
                InhabilitarControl(CorreoElectronico_RP);
                //InhabilitarControl(Categoria_RP);
                InhabilitarControl(AFP_RP);
                InhabilitarControl(SaldoCIC_RP);
                InhabilitarControl(Guardar_RP);
                PerGuardar_RP.Value = "0";

                InhabilitarControl(RangoInversion_RP);
                InhabilitarControl(CentroLaboral_RP);
            }

            //Permisos Insertar Dirección
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionInsertar))
            {
                PerNuevaDireccion_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevaDireccion_RP);
                PerNuevaDireccion_RP.Value = "0";
            }

            //Permisos Insertar Teléfono
            //oculto
            //if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoInsertar))
            //{
            //    PerNuevoTelefono_RP.Value = "1";
            //}
            //else
            //{
            //    InhabilitarControl(NuevoTelefono_RP);
            //    PerNuevoTelefono_RP.Value = "0";
            //}

            //Permisos Insertar Grupo Familiar
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
            {
                PerNuevoBeneficiario_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevoBeneficiario_RP);
                PerNuevoBeneficiario_RP.Value = "0";
            }

            //Permisos Insertar Solicitud
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPInsertar))
            {
                PerNuevaSolicitud_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevaSolicitud_IFP);
                PerNuevaSolicitud_RP.Value = "0";
            }
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

        private void CargarComboboxenBlanco(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("", "0"));
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
                control.Items.Add(new ListItem(string.Format("{0:#,##0.00}", item.Valor), item.Valor.ToString()));
            }
        }

        private void CargarComboboxNuevosDatosenBlanco(DropDownList control, string tabla, string usuario)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("", "0"));

            if (tabla == "EstadoCivil")
            {
                List<EstadoCivil> listaCombobox = (List<EstadoCivil>)servicioCotizador.ListarEstadoCivil(usuario);

                // listaCombobox = (List<EstadoCivil>)Session["ListadoEstadoCivil"];

                foreach (EstadoCivil item in listaCombobox)
                {
                    control.Items.Add(new ListItem(item.gls_estado_civil, item.cod_estado_civil));
                }
            }

        }

        public void InhabilitarControl(Control control)
        {
            if (control is TextBox)
            {
                ((TextBox)control).ReadOnly = true;
                ((TextBox)control).CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro";
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

        [WebMethod]
        public static Respuesta CargarTablaAfiliados(string tokenUsuario, string apellidoPaterno, string apellidoMaterno, string nombres, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        List<string> errores = new List<string>();
                        List<string> controles = new List<string>();
                        if (ValidarDatosAfiliado(errores, controles, apellidoPaterno, apellidoMaterno, nombres))
                        {

                            var pagina = new Page();
                            var control = (TablaAfiliados)pagina.LoadControl("~/Controles/TablaAfiliados.ascx");

                            if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.BusquedaAfiliadoConsultar))
                            {
                                int totalRegistros = 0;

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                List<Afiliado> afiliados = servicioCotizador.ListarAfiliado(apellidoPaterno, apellidoMaterno, nombres, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar, ref totalRegistros);

                                control.Afiliados = afiliados;
                                control.IndicePagina = indicePagina;
                                control.TamanhoPagina = tamanhoPagina;
                                control.ColumnaOrdenar = columnaOrdenar;
                                control.DireccionOrdenar = direccionOrdenar;
                                control.TotalRegistros = totalRegistros;
                                control.PermisoConsultar = true;
                            }
                            else
                            {
                                log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                    Enums.OpcionesSistema.BusquedaAfiliadoConsultar.StringValue()));
                                control.PermisoConsultar = false;
                            }

                            pagina.Controls.Add(control);

                            string html = "";
                            using (var sw = new StringWriter())
                            {
                                HttpContext.Current.Server.Execute(pagina, sw, false);
                                html = sw.ToString();
                            }
                            respuesta.Estado = Constante.COD_OK;
                            respuesta.Contenido = html;
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
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarTablaDirecciones(string tokenUsuario, string cuspp)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaDirecciones)pagina.LoadControl("~/Controles/TablaDirecciones.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Direccion> direcciones = servicioCotizador.ListarDireccion(cuspp);



                            //foreach (Direccion direc in direcciones)
                            //{









                            //}

                            control.Direcciones = direcciones;

                            control.PermisoConsultar = true;
                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionActualizar)) ? true : false;
                            control.PermisoEliminar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionEliminar)) ? true : false;
                        }
                        else
                        {
                            control.PermisoConsultar = false;
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
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarTablaTelefonos(string tokenUsuario, string cuspp)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaTelefonos)pagina.LoadControl("~/Controles/TablaTelefonos.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Telefono> telefonos = servicioCotizador.ListarTelefono(cuspp);

                            control.Telefonos = telefonos;

                            control.PermisoConsultar = true;
                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoActualizar)) ? true : false;
                            control.PermisoEliminar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoEliminar)) ? true : false;
                        }
                        else
                        {
                            control.PermisoConsultar = false;
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
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarTablaGrupoFamiliar(string tokenUsuario, string cuspp)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaGrupoFamiliar)pagina.LoadControl("~/Controles/TablaGrupoFamiliar.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(cuspp);

                            control.Grupos = grupos;
                            control.PermisoConsultar = true;
                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar)) ? true : false;
                            control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];

                            control.PermisoEliminar = true;

                            try
                            {
                                control.ListaNegra = false;
                                GrupoFamiliar gru = grupos.Find(p => p.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

                                if (gru != null)
                                {
                                    log.Debug("Inicio Cotizador.servicioCotizador.ObtenerDatosGrupoFamiliar");
                                    gru = servicioCotizador.ObtenerDatosGrupoFamiliar(Convert.ToInt32(gru.Id), "");
                                    log.Debug("Fin Cotizador.servicioCotizador.ObtenerDatosGrupoFamiliar");

                                    if (gru.Identificacion.IdTipo != null)
                                    {
                                        log.Debug("Inicio Cotizador.servicioCotizador.ObtenerTipoIdentificacion");
                                        List<Parametro> lstTipoIdentificacion = servicioCotizador.ObtenerTipoIdentificacion(gru.Identificacion.IdTipo, "", "");
                                        log.Debug("Fin Cotizador.servicioCotizador.ObtenerTipoIdentificacion");

                                        gru.Identificacion.GlosaTipo = lstTipoIdentificacion.Find(p => p.Id == gru.Identificacion.IdTipo).Nombre;

                                        //descomentar
                                        log.Debug("Inicio Cotizador.servicioCotizador.ObtenerCoincidenciaLN Plaft");
                                        JsonCoincidenciaLN jsonCoincidencia = servicioCotizador.ObtenerCoincidenciaLN(gru);
                                        log.Debug("Fin Cotizador.servicioCotizador.ObtenerCoincidenciaLN Plaft");

                                        if (jsonCoincidencia != null)
                                        {
                                            if (jsonCoincidencia._meta.status == "SUCCESS")
                                            {
                                                if (jsonCoincidencia.records.LN != 0)
                                                {
                                                    control.ListaNegra = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception listaNegra)
                            {
                                log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                                    listaNegra.Source, listaNegra.Message, listaNegra.StackTrace));

                                if (listaNegra.InnerException != null)
                                {
                                    log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                                        listaNegra.InnerException.Source, listaNegra.InnerException.Message, listaNegra.InnerException.StackTrace));
                                }
                            }
                        }
                        else
                        {
                            control.PermisoConsultar = false;
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
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string RenderTablaGrupoFamiliar(
            List<GrupoFamiliar> grupos,
            bool permisoConsultar,
            bool permisoModificar,
            bool permisoEliminar,
            bool consentimiento,
            bool listaNegra
        )
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();
                    var control = (TablaGrupoFamiliar)pagina.LoadControl("~/Controles/TablaGrupoFamiliar.ascx");
                    control.Grupos = grupos;
                    control.PermisoConsultar = permisoConsultar;
                    control.PermisoModificar = permisoModificar;
                    control.PermisoEliminar = permisoEliminar;
                    control.Consentimiento = consentimiento;
                    control.ListaNegra = listaNegra;
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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarTablaSolicitudes(string tokenUsuario, string cuspp, bool mostrarTodos)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesIFP)pagina.LoadControl("~/Controles/TablaSolicitudesIFP.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<SolicitudIFP> solicitudes = servicioCotizador.ListarSolicitudIFP(cuspp);

                            if (!mostrarTodos)
                            {
                                DateTime fechaActual = DateTime.Today;
                                solicitudes = solicitudes.FindAll(p => p.FechaVigencia >= fechaActual);
                            }

                            //inteligo
                            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue()
                                || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue()
                                || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeVentaLima.StringValue()
                                || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeVentaProvincia.StringValue()
                                || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteOperaciones.StringValue()
                                || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
                            {
                                //control.Solicitudes = solicitudes.FindAll(cd => cd.CodCanalDistribucion == Enums.CanalDistribucion.BancaSeguros.StringValue() && cd.AgenteCotizacion == (string)HttpContext.Current.Session["Usuario"]);
                                control.Solicitudes = solicitudes;
                            }
                            else
                            {
                                //control.Solicitudes = solicitudes.FindAll(cd => cd.AgenteCotizacion == (string)HttpContext.Current.Session["Usuario"]);
                                if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
                                {
                                    List<Agente> listaAgentesExternos = (List<Agente>)HttpContext.Current.Session["ListaAgentesExternos"];

                                    //control.Solicitudes = solicitudes.FindAll(cd => cd.AgenteCotizacion == (string)HttpContext.Current.Session["Usuario"]);
                                    control.Solicitudes = solicitudes.Join(listaAgentesExternos, s => s.AgenteCotizacion, la => la.Usuario, (s, la) => s).ToList();
                                }
                                else
                                {
                                    control.Solicitudes = solicitudes.FindAll(cd => cd.OrigenCotizacion != "2");
                                }
                            }

                            control.PermisoInsertar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPInsertar)) ? true : false;
                            control.PermisoConsultar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPConsultar)) ? true : false;
                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPActualizar)) ? true : false;
                            control.PermisoCorreoElectronico = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPEnviarCorreo)) ? true : false;
                            control.PermisoExportarPDF = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPExportarPDF)) ? true : false;
                            control.PermisoReporteEscenario = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudReporteEscenarios)) ? true : false; ;
                            control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                            control.PermisoCerrar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusCerrar)) ? true : false;
                            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet

                            control.PermisoObtenerEdN = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ObtenerFormatoEstudioNecesidades)) ? true : false;

                            control.ExistePoliza = solicitudes.Exists(s => s.NumeroPoliza != 0);
                        }
                        else
                        {
                            control.PermisoConsultar = false;
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
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string RenderTablaSolicitudes(
            List<SolicitudIFP> solicitudes,
            List<Agente> listaAgentesExternos,
            string rolAzman,
            bool permisoConsultar,
            bool permisoInsertar,
            bool permisoModificar,
            bool permisoCorreoElectronico,
            bool permisoExportarPDF,
            bool permisoReporteEscenario,
            bool consentimiento,
            bool permisoCerrar,
            bool mostrarTodos,
            bool permisoObtenerEdN
        )
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();
                    var control = (TablaSolicitudesIFP)pagina.LoadControl("~/Controles/TablaSolicitudesIFP.ascx");

                    if (!mostrarTodos)
                    {
                        DateTime fechaActual = DateTime.Today;
                        solicitudes = solicitudes.FindAll(p => p.FechaVigencia >= fechaActual);
                    }

                    if (rolAzman == Enums.RolAzman.AsistenteComercial.StringValue()
                                || rolAzman == Enums.RolAzman.JefeOperaciones.StringValue()
                                || rolAzman == Enums.RolAzman.JefeVentaLima.StringValue()
                                || rolAzman == Enums.RolAzman.JefeVentaProvincia.StringValue()
                                || rolAzman == Enums.RolAzman.AsistenteOperaciones.StringValue()
                                || rolAzman == Enums.RolAzman.GerenteDivision.StringValue())
                    {
                        control.Solicitudes = solicitudes;
                    }
                    else
                    {
                        if (rolAzman == Enums.RolAzman.AgenteExterno.StringValue())
                        {
                            control.Solicitudes = solicitudes.Join(listaAgentesExternos, s => s.AgenteCotizacion, la => la.Usuario, (s, la) => s).ToList();
                        }
                        else
                        {
                            control.Solicitudes = solicitudes.FindAll(cd => cd.OrigenCotizacion != "2");
                        }
                    }

                    control.PermisoInsertar = permisoInsertar;
                    control.PermisoConsultar = permisoConsultar;
                    control.PermisoModificar = permisoModificar;
                    control.PermisoCorreoElectronico = permisoCorreoElectronico;
                    control.PermisoExportarPDF = permisoExportarPDF;
                    control.PermisoReporteEscenario = permisoReporteEscenario;
                    control.Consentimiento = consentimiento;
                    control.PermisoCerrar = permisoCerrar;

                    control.PermisoObtenerEdN = permisoObtenerEdN;

                    control.ExistePoliza = solicitudes.Exists(s => s.NumeroPoliza != 0);

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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarComboCiudades(string idDepartamento)
        {

            var pagina = new Page();
            var control = (ComboboxCiudades)pagina.LoadControl("~/Controles/ComboboxCiudades.ascx");

            List<Ciudad> listaCombobox = new List<Ciudad>();

            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
            string usuario = HttpContext.Current.Session["Usuario"].ToString();

            JArray listaProvincias = new JArray();
            var urlProvincias = ConfigurationManager.AppSettings["url_lista_provincias"].ToString();
            urlProvincias = string.Format(urlProvincias, idDepartamento, usuario);
            listaProvincias = ObtenerUbigeo(urlToken, urlProvincias, usuario, listaProvincias);

            if (listaProvincias != null)
            {
                foreach (var item in listaProvincias)
                {
                    Ciudad itemCombobox = new Ciudad();
                    itemCombobox.Id = item["id_provincia"].ToString().ToLower();
                    itemCombobox.Nombre = item["gls_provincia"].ToString().ToUpper();

                    listaCombobox.Add(itemCombobox);
                }
            }

            control.Ciudades = listaCombobox;

            pagina.Controls.Add(control);

            string html = "";
            using (var sw = new StringWriter())
            {
                HttpContext.Current.Server.Execute(pagina, sw, false);
                html = sw.ToString();
            }
            return html;

        }

        [WebMethod]
        public static string CargarComboComunas(string idCiudad)
        {

            var pagina = new Page();
            var control = (ComboboxComunas)pagina.LoadControl("~/Controles/ComboboxComunas.ascx");

            List<Comuna> listaCombobox = new List<Comuna>();

            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
            string usuario = HttpContext.Current.Session["Usuario"].ToString();

            JArray listaDistritos = new JArray();
            var urlDistritos = ConfigurationManager.AppSettings["url_lista_distritos"].ToString();
            urlDistritos = string.Format(urlDistritos, idCiudad, usuario);
            listaDistritos = ObtenerUbigeo(urlToken, urlDistritos, usuario, listaDistritos);

            if (listaDistritos != null)
            {
                foreach (var item in listaDistritos)
                {
                    Comuna itemCombobox = new Comuna();
                    itemCombobox.Id = item["id_distrito"].ToString().ToLower();
                    itemCombobox.Nombre = item["gls_distrito"].ToString().ToUpper();

                    listaCombobox.Add(itemCombobox);
                }
            }

            control.Comunas = listaCombobox;

            pagina.Controls.Add(control);

            string html = "";
            using (var sw = new StringWriter())
            {
                HttpContext.Current.Server.Execute(pagina, sw, false);
                html = sw.ToString();
            }
            return html;

        }

        [WebMethod]
        public static void CargarComboPeriodoGarantizado(string idTemporalidad)
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Producto> productos = servicioCotizador.ListarProducto(idTemporalidad);
            HttpContext.Current.Session["ComboProducto"] = productos;
        }

        [WebMethod]
        public static Direccion ObtenerDatosDireccion(int idDireccion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Direccion dir;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                dir = servicioCotizador.ObtenerDatosDireccion(idDireccion);
                return dir;
            }
        }

        [WebMethod]
        public static String SessionIdDreccion(int idDireccion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                HttpContext.Current.Session["idDireccion"] = idDireccion;
                if (idDireccion == 0)
                {
                    HttpContext.Current.Session["ModDirModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModDirModo"] = "M";
                }

                return idDireccion.ToString();
            }
        }

        [WebMethod]
        public static string SessionIdSolicitud(string idSolicitud, string fecCotizacion, string accion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                HttpContext.Current.Session["idSolicitud"] = idSolicitud;
                HttpContext.Current.Session["fecCotizacion"] = fecCotizacion;
                if (idSolicitud == "")
                {
                    HttpContext.Current.Session["ModSolModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModSolModo"] = accion;
                }

                return idSolicitud.ToString();
            }
        }

        [WebMethod]
        public static string SessionIdTelefono(int idTelefono)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                HttpContext.Current.Session["idTelefono"] = idTelefono;
                if (idTelefono == 0)
                {
                    HttpContext.Current.Session["ModTelModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModTelModo"] = "M";
                }

                return idTelefono.ToString();
            }
        }

        [WebMethod]
        public static string SessionIdGrupoFamiliar(int idGrupoFamiliar)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                HttpContext.Current.Session["idGrupoFamiliar"] = idGrupoFamiliar;
                if (idGrupoFamiliar == 0)
                {
                    HttpContext.Current.Session["ModGruFamModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModGruFamModo"] = "M";
                }

                return idGrupoFamiliar.ToString();
            }
        }

        [WebMethod]
        public static Respuesta InsertarDireccion(string tokenUsuario, string cuspp, string direccion, string idDepartamento, string idCiudad, string idComuna, string idPrincipal, string glsEspacioUrbano, string idDomicilio)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionInsertar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarDireccion(direccion, idDepartamento, idCiudad, idComuna, idPrincipal, glsEspacioUrbano, idDomicilio, errores, controles))
                            {
                                // Validar la cartera del agente
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
                                {
                                    Direccion dir = new Direccion
                                    {
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Glosa = direccion,
                                        Departamento = new Departamento { Id = idDepartamento },
                                        Ciudad = new Ciudad { Id = idCiudad },
                                        Comuna = new Comuna { Id = idComuna },
                                        Principal = (idPrincipal == "S") ? true : false,
                                        EspacioUrbano = glsEspacioUrbano,
                                        TipoVia = new Parametro { Id = idDomicilio },
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    servicioCotizador.RegistrarDireccion(dir);

                                    respuesta.Estado = Constante.COD_OK;
                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
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
                                Enums.OpcionesSistema.DireccionInsertar.StringValue()));
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
        public static Respuesta ModificarDireccion(string tokenUsuario, int idDireccion, string cuspp, string direccion, string idDepartamento, string idCiudad, string idComuna, string idPrincipal, string glsEspacioUrbano, string idDomicilio)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionActualizar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarDireccion(direccion, idDepartamento, idCiudad, idComuna, idPrincipal, glsEspacioUrbano, idDomicilio, errores, controles))
                            {
                                // Validar la cartera del agente
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
                                {
                                    Direccion dir = new Direccion
                                    {
                                        Id = Convert.ToInt32(idDireccion),
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Glosa = direccion,
                                        Departamento = new Departamento { Id = idDepartamento },
                                        Ciudad = new Ciudad { Id = idCiudad },
                                        Comuna = new Comuna { Id = idComuna },
                                        Principal = (idPrincipal == "S") ? true : false,
                                        EspacioUrbano = glsEspacioUrbano,
                                        TipoVia = new Parametro { Id = idDomicilio },
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    servicioCotizador.ActualizarDireccion(dir);

                                    respuesta.Estado = Constante.COD_OK;
                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
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
                                Enums.OpcionesSistema.DireccionActualizar.StringValue()));
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
        public static Respuesta EliminarDireccion(string tokenUsuario, int idDireccion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionEliminar))
                        {
                            // Validar la cartera del agente
                            if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
                            {
                                Direccion dir = new Direccion
                                {
                                    Id = Convert.ToInt32(idDireccion),
                                    Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                };

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                servicioCotizador.EliminarDireccion(dir);
                                respuesta.Estado = Constante.COD_OK;
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DireccionEliminar.StringValue()));
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
        public static Telefono ObtenerDatosTelefono(int idTelefono)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Telefono tel;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                tel = servicioCotizador.ObtenerDatosTelefono(idTelefono);
                return tel;
            }
        }

        [WebMethod]
        public static GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                GrupoFamiliar gru;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                gru = servicioCotizador.ObtenerDatosGrupoFamiliar(idGrupoFamiliar, "");
                return gru;
            }
        }

        [WebMethod]
        public static Respuesta InsertarGrupoFamiliar(string tokenUsuario,
                                                        string cuspp,
                                                        string apellidoPaterno,
                                                        string apellidoMaterno,
                                                        string nombres,
                                                        string tipoIdentificacion,
                                                        string numeroIdentificacion,
                                                        string parentesco,
                                                        string sexo,
                                                        string fechaNacimiento,
                                                        string invalidez,
                                                        string tipoInvalidez,
                                                        string fechaInvalidez)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez, cuspp))
                            {
                                // Validar la cartera del agente
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    GrupoFamiliar gru = new GrupoFamiliar
                                    {
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Identificacion = new Identificacion(),
                                        Parentesco = new Parentesco { Id = parentesco },
                                        Sexo = Convert.ToChar(sexo),
                                        FechaNacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE")),
                                        Invalido = (invalidez == "S") ? true : false,
                                        TipoInvalidez = new TipoInvalidez { Id = tipoInvalidez },
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };
                                    if (apellidoPaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoPaterno = apellidoPaterno;
                                    }
                                    if (apellidoMaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoMaterno = apellidoMaterno;
                                    }
                                    if (nombres.Trim() != String.Empty)
                                    {
                                        gru.Nombre = nombres;
                                    }
                                    if (tipoIdentificacion.Trim() != "0")
                                    {
                                        gru.Identificacion.IdTipo = tipoIdentificacion;
                                    }
                                    if (numeroIdentificacion.Trim() != String.Empty)
                                    {
                                        //gru.Identificacion.Numero = Convert.ToInt32(numeroIdentificacion);
                                        gru.Identificacion.Numero = numeroIdentificacion;
                                    }
                                    if (fechaInvalidez.Trim() != String.Empty)
                                    {
                                        gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.RegistrarGrupoFamiliar(gru);

                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                    respuesta.Mensaje = "Grupo familiar insertado correctamente.";
                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
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
                                Enums.OpcionesSistema.GrupoFamiliarInsertar.StringValue()));
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
        public static Respuesta ModificarGrupoFamiliar(string tokenUsuario,
                                                        string idGrupoFamiliar,
                                                        string cuspp,
                                                        string apellidoPaterno,
                                                        string apellidoMaterno,
                                                        string nombres,
                                                        string tipoIdentificacion,
                                                        string numeroIdentificacion,
                                                        string parentesco,
                                                        string sexo,
                                                        string fechaNacimiento,
                                                        string invalidez,
                                                        string tipoInvalidez,
                                                        string fechaInvalidez)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez, cuspp))
                            {
                                // Validar la cartera del agente
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    GrupoFamiliar gru = new GrupoFamiliar
                                    {
                                        Id = Convert.ToInt32(idGrupoFamiliar),
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Identificacion = new Identificacion(),
                                        Parentesco = new Parentesco { Id = parentesco },
                                        Sexo = Convert.ToChar(sexo),
                                        FechaNacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE")),
                                        Invalido = (invalidez == "S") ? true : false,
                                        TipoInvalidez = new TipoInvalidez { Id = tipoInvalidez },
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };
                                    if (apellidoPaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoPaterno = apellidoPaterno;
                                    }
                                    if (apellidoMaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoMaterno = apellidoMaterno;
                                    }
                                    if (nombres.Trim() != String.Empty)
                                    {
                                        gru.Nombre = nombres;
                                    }
                                    if (tipoIdentificacion.Trim() != "0")
                                    {
                                        gru.Identificacion.IdTipo = tipoIdentificacion;
                                    }
                                    if (numeroIdentificacion.Trim() != String.Empty)
                                    {
                                        //gru.Identificacion.Numero = Convert.ToInt32(numeroIdentificacion);
                                        gru.Identificacion.Numero = numeroIdentificacion;
                                    }
                                    if (fechaInvalidez.Trim() != String.Empty)
                                    {
                                        gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.ActualizarGrupoFamiliar(gru);

                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                    respuesta.Mensaje = "Grupo familiar modificado correctamente.";

                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
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
                                Enums.OpcionesSistema.GrupoFamiliarActualizar.StringValue()));
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
        public static SolicitudIFP CrearDatosSolicitud(string tokenUsuario)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudIFP sol = new SolicitudIFP();

                sol.FechaSolicitud = DateTime.Now;
                sol.Cotizaciones = new List<CotizacionIFP>();

                HttpContext.Current.Session["idMonedaFondo"] = Enums.Moneda.Soles.StringValue();

                //List<ParametrosMotorIFP> lstParametrosMotorIFP = (List<ParametrosMotorIFP>)HttpContext.Current.Session["ParametroMotorGeneral"];

                //if (lstParametrosMotorIFP == null)
                //    lstParametrosMotorIFP = new List<ParametrosMotorIFP>();

                //if (lstParametrosMotorIFP.FindAll(p => p.cod_monedaIFP == "001"
                //    && p.cod_tipo_temporalidadIFP == "T05" && p.fec_cotizacionIFP == DateTime.Today).Count == 0)
                //{
                //    bool cantidad_megas = false;
                //    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                //    ParametrosMotorIFP parametrosMotorIFP = servicioCotizador.ObtenerParametroGenerales("T05", "001", DateTime.Today, true, tokenUsuario, ref cantidad_megas);

                //    lstParametrosMotorIFP.Add(parametrosMotorIFP);
                //    HttpContext.Current.Session["ParametroMotorGeneral"] = lstParametrosMotorIFP;
                //}

                return sol;
            }
        }

        [WebMethod]
        public static SolicitudIFP ObtenerDatosSolicitud(string idSolicitud, string fecCotizacion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudIFP sol;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                sol = servicioCotizador.ObtenerDatosSolicitudIFP(idSolicitud);

                HttpContext.Current.Session["idMonedaFondo"] = sol.MonedaPrimaUnica.Id.ToString();

                return sol;
            }
        }

        [WebMethod]
        public static List<CotizacionIFP> AgregarCotizacionASolicitud(List<CotizacionIFP> cotizaciones, string plan)
        {

            CotizacionIFP cotizacion = new CotizacionIFP();
            if (plan == Enums.Planes.PLAN1.StringValue())
            {
                cotizacion = new CotizacionIFP
                {
                    Item = cotizaciones.Count(),
                    Moneda = new Moneda { Id = "000" },
                    PeriodoGarantizado = 0,
                    AjusteTRA = 0,
                    PagoDoble = 0,
                    PjePagoDoble = 0,
                    IndGastoSepelio = "S",
                    ValPjeDev = 50,
                    ValMonAju = 0,
                    ValPjeDCOM = 0,
                    Temporalidad = new Temporalidad { Id = "T05", Anhos = 5 },
                    ValPerDiferido = 0,
                    ValPjeDevFallec = 50,
                    ValPjeFallecNoDeveng = 0,

                    PensionCiaMO = 0,
                    Pension2doTramo = 0,
                    TasaVenta = 0,
                    TasaVentaSbs = 0,
                    TasaRetornoAccionista = 0,
                    TasaRetornoAccionistaMinima = 0,
                    IndCotiza = string.Empty,
                    Plan = new Plan { Id = plan },

                    ValPjeCACy = 0,
                    ValPjeCAPa = 0,
                    ValPjeCAMa = 0,
                    ValPjeCATotal = "0%"

                };
            }
            else if (plan == Enums.Planes.PLAN2.StringValue())
            {
                cotizacion = new CotizacionIFP
                {
                    Item = cotizaciones.Count(),
                    Moneda = new Moneda { Id = "000" },
                    PeriodoGarantizado = 5,
                    AjusteTRA = 0,
                    PagoDoble = 0,
                    PjePagoDoble = 0,
                    IndGastoSepelio = "S",
                    ValPjeDev = 0,
                    ValMonAju = 0,
                    ValPjeDCOM = 0,
                    Temporalidad = new Temporalidad { Id = "T05", Anhos = 5 },
                    ValPerDiferido = 0,
                    ValPjeDevFallec = 0,
                    ValPjeFallecNoDeveng = 0,

                    PensionCiaMO = 0,
                    Pension2doTramo = 0,
                    TasaVenta = 0,
                    TasaVentaSbs = 0,
                    TasaRetornoAccionista = 0,
                    TasaRetornoAccionistaMinima = 0,
                    IndCotiza = string.Empty,
                    Plan = new Plan { Id = plan },

                    ValPjeCACy = 0,
                    ValPjeCAPa = 0,
                    ValPjeCAMa = 0,
                    ValPjeCATotal = "0%"

                };
            }
            else if (plan == Enums.Planes.PLAN3.StringValue())
            {
                cotizacion = new CotizacionIFP
                {
                    Item = cotizaciones.Count(),
                    Moneda = new Moneda { Id = "000" },
                    PeriodoGarantizado = 5,
                    AjusteTRA = 0,
                    PagoDoble = 0,
                    PjePagoDoble = 0,
                    IndGastoSepelio = "S",
                    ValPjeDev = 0,
                    ValMonAju = 0,
                    ValPjeDCOM = 0,
                    Temporalidad = new Temporalidad { Id = "TVT", Anhos = 0 },
                    ValPerDiferido = 0,
                    ValPjeDevFallec = 0,
                    ValPjeFallecNoDeveng = 0,

                    PensionCiaMO = 0,
                    Pension2doTramo = 0,
                    TasaVenta = 0,
                    TasaVentaSbs = 0,
                    TasaRetornoAccionista = 0,
                    TasaRetornoAccionistaMinima = 0,
                    IndCotiza = string.Empty,
                    Plan = new Plan { Id = plan },

                    ValPjeCACy = 0,
                    ValPjeCAPa = 0,
                    ValPjeCAMa = 0,
                    ValPjeCATotal = "0%"

                };
            }

            cotizaciones.Add(cotizacion);

            for (int i = 0; i < cotizaciones.Count; i++)
            {
                cotizaciones[i].Item = i;
            }

            return cotizaciones;
        }

        [WebMethod]
        public static SolicitudIFP InsertarSolicitud(string tokenUsuario,
                                                    string cuspp,
                                                    string monedaPrimaUnica,
                                                    string primaUnica,
                                                    string fechaCotizacion,
                                                    string fechaDevengue,
                                                    List<CotizacionIFP> cotizaciones,
                                                    List<int> idBeneficiarios,
                                                    List<CoberturaAdicional> coberturasAdicionales)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudIFP solicitud;
                try
                {
                    log.Debug("Inicio Cotizador.InsertarSolicitud");
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPInsertar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();

                            //if (ValidarSolicitud(errores, controles, fechaCotizacion, fechaDevengue, primaUnica, dcom, cotizaciones, idBeneficiarios, cuspp, (string)HttpContext.Current.Session["Vendedor"], temporalidad))
                            if (ValidarSolicitud(errores, controles, fechaCotizacion, fechaDevengue, monedaPrimaUnica, primaUnica, cotizaciones, idBeneficiarios, coberturasAdicionales))
                            {
                                //Validando la fecha de cotizacion segun rol
                                switch ((string)HttpContext.Current.Session["RolAzman"])
                                {
                                    case "JEF.RVI.OPE"://JefeOperaciones
                                    case "AST.RVI.OPE"://AsistenteOperaciones
                                        break;
                                    default:
                                        fechaCotizacion = (DateTime.Today).ToString("dd/MM/yyyy");
                                        fechaDevengue = (DateTime.Today.AddDays(-(DateTime.Today.Day - 1))).ToString("dd/MM/yyyy");
                                        break;
                                }

                                // Validar la cartera del agente
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
                                {
                                    solicitud = new SolicitudIFP
                                    {
                                        Afiliado = new Afiliado { CUSPP = cuspp, AFP = new AFP { Id = "0" } },
                                        TipoPension = new TipoPension { Id = Enums.TipoPension.Jubilacion.StringValue() },
                                        Categoria = new Categoria { Id = "A" },
                                        FechaSolicitud = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                        FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                        FechaDevengue = Convert.ToDateTime(fechaDevengue, new CultureInfo("es-PE")),
                                        MonedaPrimaUnica = new Moneda { Id = monedaPrimaUnica },
                                        PrimaUnica = Convert.ToDouble(primaUnica, new CultureInfo("es-PE")),
                                        FactorTasa = "O",
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"], Rol = (string)HttpContext.Current.Session["RolAzman"] },
                                        Agente = new Agente { Id = (string)HttpContext.Current.Session["Vendedor"], IdCartera = (string)HttpContext.Current.Session["Cartera"] },
                                        TipoCotizacion = new TipoCotizacion { Id = Enums.TipoCotizacion.RentaPrivadaIFP.StringValue() },
                                        Cotizaciones = cotizaciones.OrderBy(p => p.Plan.Id).ThenBy(p => p.Item).ToList(),
                                        CodCanalDistribucion = Enums.CanalDistribucion.FuerzaDeVentas.StringValue(),
                                        CoberturasAdicionales = (coberturasAdicionales != null) ? coberturasAdicionales.Where(w => w.FechaNacimiento != "").ToList() : new List<CoberturaAdicional>()
                                    };

                                    if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
                                    {
                                        solicitud.OrigenCotizacion = Enums.OrigenCotizacion.Inteligo.StringValue();
                                    }
                                    else
                                    {
                                        solicitud.OrigenCotizacion = Enums.OrigenCotizacion.Interseguro.StringValue();
                                    }

                                    List<GrupoFamiliar> lben = new List<GrupoFamiliar>();
                                    idBeneficiarios.ForEach(id => lben.Add(((List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"])[id]));

                                    solicitud.Beneficiarios = lben;

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                    solicitud.FechaVigencia = solicitud.FechaCotizacion;
                                    List<Parametro> lstDiasVigencia = new List<Parametro>();
                                    lstDiasVigencia = servicioCotizador.ObtenerParametrosPorTabla("PLUS");

                                    if (lstDiasVigencia.Count() > 0)
                                    {
                                        solicitud.FechaVigencia = solicitud.FechaVigencia.Value.AddDays(Convert.ToInt32(lstDiasVigencia[0].Valor_1));
                                    }

                                    List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];
                                    if (listaParametro.Find(p => p.Id == "INDLOGS" && p.Nombre == "LogCotizacion") != null)
                                    {
                                        solicitud.isLogCotizacion = listaParametro.Find(p => p.Id == "INDLOGS" && p.Nombre == "LogCotizacion").Valor_1 == "S" ? true : false;
                                        solicitud.isLogReserva = false;
                                    }

                                    foreach (var cotizacion in solicitud.Cotizaciones)
                                    {
                                        if (cotizacion.Plan.Id == Enums.Planes.PLAN2.StringValue())
                                        {
                                            log.Debug("Insertar" + cotizacion.ValPjeDevFallec);
                                            cotizacion.ValPjeDevFallec = 0.00;
                                        }
                                    }

                                    //var conyuge = solicitud.Beneficiarios.FindAll(b => b.Parentesco.Id == Enums.Parentesco.Conyuge.StringValue());
                                    //var hijos = solicitud.Beneficiarios.FindAll(b => b.Parentesco.Id == Enums.Parentesco.Hijo.StringValue() || b.Parentesco.Id == Enums.Parentesco.Nieto.StringValue());
                                    //var padres = solicitud.Beneficiarios.FindAll(b => b.Parentesco.Id == Enums.Parentesco.Padre.StringValue());
                                    bool solicitudPlan3 = false;

                                    foreach (var itemCot in solicitud.Cotizaciones)
                                    {
                                        if (itemCot.Plan.Id == Enums.Planes.PLAN3.StringValue())
                                        {
                                            solicitudPlan3 = true;
                                            //if (conyuge.Count > 0)
                                            //{
                                            //    if (hijos.Count == 0 && padres.Count == 0)
                                            //    {
                                            //        // Sólo cónyuge
                                            //        if (itemCot.ValPjeConyuge == 0) itemCot.ValPjeConyuge = 42;
                                            //    }
                                            //    else
                                            //    {
                                            //        // Cónyuge + Hijo(s) y/o Padre(s)
                                            //        if (itemCot.ValPjeConyuge == 0) itemCot.ValPjeConyuge = 35;
                                            //    }
                                            //}
                                            //CalcularPorcentajesBeneficiarios(solicitud.Beneficiarios, (int)itemCot.ValPjeConyuge);

                                            solicitud.Beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue()).ValPjeRenta = 100.00;

                                            double pjeTotal = 0.00;
                                            solicitud.Beneficiarios.ForEach(ben =>
                                            {
                                                if (ben.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                                                { pjeTotal += ben.ValPjeRenta; }
                                            });

                                            if (pjeTotal > 100)
                                            {
                                                throw new Exception(string.Format("La suma del porcentaje total de los beneficiarios es superior a 100%"));
                                            }

                                            foreach (var beneficiarioPorcentaje in solicitud.Beneficiarios)
                                            {
                                                if (beneficiarioPorcentaje.ValPjeRenta == 0)
                                                {
                                                    throw new Exception(string.Format("El porcentaje del beneficiario no puede ser 0%"));
                                                    break;
                                                }
                                            }

                                        }

                                    }

                                    if (!solicitudPlan3)
                                    {
                                        var titular = solicitud.Beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());
                                        titular.ValPjeRenta = 100;
                                        titular.ValPjeAdicional = 0;

                                        List<GrupoFamiliar> beneficiarioTitular = new List<GrupoFamiliar>();
                                        beneficiarioTitular.Add(titular);
                                        solicitud.Beneficiarios = beneficiarioTitular;
                                    }

                                    if (solicitudPlan3)
                                    {
                                        if (solicitud.Beneficiarios.Count() > 1)
                                        {
                                            solicitud.TipoPlan = new TipoPlan { Id = Enums.TipoPlan.Familiar.StringValue() };
                                        }
                                        else
                                        {
                                            solicitud.TipoPlan = new TipoPlan { Id = Enums.TipoPlan.Individual.StringValue() };
                                        }
                                    }

                                    log.Debug("Inicio servicioCotizador.RegistrarSolicitudIFP fe servicio");
                                    //respuesta = servicioCotizador.RegistrarSolicitudIFP(lstParametrosMotorIFPEnvio, ref sol);
                                    respuesta = servicioCotizador.RegistrarSolicitudIFP(tokenUsuario, ref solicitud);
                                    log.Debug("Fin servicioCotizador.RegistrarSolicitudIFP fe servicio");

                                    if (((string)HttpContext.Current.Session["RolAzman"]) == "JEF.RVI.OPE")
                                    {
                                        HttpContext.Current.Session["ModSolModo"] = "M";
                                    }
                                    else
                                    {
                                        if (DateTime.Today == solicitud.FechaSolicitud)
                                            HttpContext.Current.Session["ModSolModo"] = "M";
                                        else
                                            HttpContext.Current.Session["ModSolModo"] = "CONS";
                                    }

                                    HttpContext.Current.Session["idSolicitud"] = solicitud.Id;
                                    HttpContext.Current.Session["fecCotizacion"] = solicitud.FechaCotizacion;

                                    //Guardar en log de auditoría
                                    string nombreTerminal = string.Empty;
                                    try
                                    {
                                        nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                    }
                                    catch (Exception)
                                    {
                                        log.Warn(string.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                            HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                    }

                                    nombreTerminal += HttpContext.Current.Request.UserAgent;

                                    string tra = string.Empty;
                                    string dcom = string.Empty;
                                    int correlativo = 1;
                                    foreach (var itemCot in solicitud.Cotizaciones)
                                    {
                                        tra += "[" + correlativo + ": " + itemCot.AjusteTRA + "] ";
                                        dcom += "[" + correlativo + ": " + itemCot.ValPjeDCOM + "] ";
                                        correlativo += 1;
                                    }

                                    // Se coloca el usuario en una variable local para que lo pueda usar el hilo
                                    string usuario = (string)HttpContext.Current.Session["Usuario"];

                                    // Registro de formato de Estudio de Necesidades
                                    Task hilo = new Task(() =>
                                    {
                                        try
                                        {
                                            // Se vuelve a obtener la solicitud desde la Base de datos para que
                                            // refresque los ´Número de Cotización nuevos
                                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                            solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(solicitud.Id);

                                            EstudioNecesidadAPI estudioNecesidadAPI = new EstudioNecesidadAPI();
                                            SolicitudEdNAPI solicitudEDN = new SolicitudEdNAPI
                                            {
                                                num_solicitud = solicitud.Id,
                                                fec_solicitud = solicitud.FechaSolicitud.Value,
                                                val_mto_cta_individual = solicitud.PrimaUnica,
                                                cod_moneda_cta_indiv = solicitud.MonedaPrimaUnica.Id
                                            };

                                            estudioNecesidadAPI.solicitud = solicitudEDN;
                                            estudioNecesidadAPI.cotizaciones = new List<CotizacionEdNAPI>();
                                            foreach (CotizacionIFP c in solicitud.Cotizaciones)
                                            {
                                                CotizacionEdNAPI cotizacionEDN = new CotizacionEdNAPI
                                                {
                                                    num_correlativo = c.Correlativo,
                                                    cod_tipo_temporalidad = c.Temporalidad.Anhos.ToString(),
                                                    val_per_diferido = c.ValPerDiferido
                                                };
                                                estudioNecesidadAPI.cotizaciones.Add(cotizacionEDN);
                                            }

                                            log.Info("Cantidad de beneficiarios(insertar cotización): " + solicitud.Beneficiarios.Count);
                                            var beneficiarios = solicitud.Beneficiarios.GroupBy(be => new { be.Identificacion.IdTipo, be.Identificacion.Numero }).Select(s => s.First()).ToList();
                                            log.Info("Cantidad de beneficiarios(insertar cotización): " + beneficiarios.Count);

                                            estudioNecesidadAPI.beneficiarios = new List<BeneficiarioEdNAPI>();
                                            foreach (GrupoFamiliar b in beneficiarios)
                                            {
                                                BeneficiarioEdNAPI beneficiarioEDN = new BeneficiarioEdNAPI
                                                {
                                                    ape_paterno = b.ApellidoPaterno,
                                                    ape_materno = b.ApellidoMaterno,
                                                    nom_persona = b.Nombre,
                                                    num_identificacion = b.Identificacion.Numero,
                                                    cod_parentezco = b.Parentesco.Id
                                                };

                                                switch (b.Identificacion.IdTipo)
                                                {
                                                    case "D":
                                                        beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                                        break;
                                                    case "E":
                                                        beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.CE.StringValue();
                                                        break;
                                                    case "P":
                                                        beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.PAS.StringValue();
                                                        break;
                                                    default:
                                                        beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                                        break;
                                                }
                                                estudioNecesidadAPI.beneficiarios.Add(beneficiarioEDN);
                                            }

                                            log.Debug(string.Format("Se va a registrar el formato EDN de la solicitud [{0}]", solicitud.Id));
                                            var respuestaEdN = Utilitario.GenerarEstudioNecesidades(estudioNecesidadAPI, usuario);
                                            log.Debug(string.Format("Respuesta EDN: Solicitud[{0}] Estado[{1}] Mensaje[{2}]", solicitud.Id, respuestaEdN.Estado, respuestaEdN.Mensaje));
                                        }
                                        catch (Exception ex)
                                        {
                                            log.Error(string.Format("Error en generación del formato de Estudio de Necesidades: Solicitud[{0}]", solicitud.Id), ex);
                                        }
                                    });
                                    hilo.Start();

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = usuario,
                                        IdTipoEvento = Enums.EventoLog.CotizarSolicitud.StringValue(),
                                        Detalle = "Solicitud registrada: " + solicitud.Id + ", DCOM: " + dcom + ", DTRA: " + tra
                                    });
                                }
                                else
                                {
                                    solicitud = new SolicitudIFP();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                solicitud = new SolicitudIFP();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            solicitud = new SolicitudIFP();
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.SolicitudIFPInsertar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        solicitud = new SolicitudIFP();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    solicitud.Respuesta = respuesta;
                    log.Debug("Fin Cotizador.InsertarSolicitud");
                    return solicitud;
                }
                catch (Exception ex)
                {
                    solicitud = new SolicitudIFP();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    solicitud.Respuesta = respuesta;
                    return solicitud;
                }
            }
        }

        [WebMethod]
        public static SolicitudIFP ModificarSolicitud(string tokenUsuario,
                                                     string idSolicitud,
                                                     string cuspp,
                                                     string monedaPrimaUnica,
                                                     string primaUnica,
                                                     string fechaCotizacion,
                                                     string fechaDevengue,
                                                     List<CotizacionIFP> cotizaciones,
                                                     List<int> idBeneficiarios,
                                                     List<CoberturaAdicional> coberturasAdicionales)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudIFP solicitud;
                try
                {

                    log.Debug("Inicio Cotizador.ModificarSolicitud");
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPActualizar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();

                            if (ValidarSolicitud(errores, controles, fechaCotizacion, fechaDevengue, monedaPrimaUnica, primaUnica, cotizaciones, idBeneficiarios, coberturasAdicionales))
                            {
                                // Validar la cartera del agente
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
                                {
                                    solicitud = new SolicitudIFP
                                    {
                                        Id = idSolicitud,
                                        //Afiliado = new Afiliado { CUSPP = cuspp, AFP = new AFP { Id = afp } },
                                        Afiliado = new Afiliado { CUSPP = cuspp, AFP = new AFP { Id = "0" } },
                                        TipoPension = new TipoPension { Id = Enums.TipoPension.Jubilacion.StringValue() },
                                        Categoria = new Categoria { Id = "A" },
                                        FechaSolicitud = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                        FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                        FechaDevengue = Convert.ToDateTime(fechaDevengue, new CultureInfo("es-PE")),
                                        MonedaPrimaUnica = new Moneda { Id = monedaPrimaUnica },
                                        PrimaUnica = Convert.ToDouble(primaUnica, new CultureInfo("es-PE")),
                                        FactorTasa = "O",
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"], Rol = (string)HttpContext.Current.Session["RolAzman"] },
                                        Agente = new Agente { Id = (string)HttpContext.Current.Session["Vendedor"], IdCartera = (string)HttpContext.Current.Session["Cartera"] },
                                        TipoCotizacion = new TipoCotizacion { Id = Enums.TipoCotizacion.RentaPrivadaIFP.StringValue() },
                                        Cotizaciones = cotizaciones.OrderBy(p => p.Plan.Id).ThenBy(p => p.Item).ToList(),
                                        CoberturasAdicionales = coberturasAdicionales.Where(w => w.FechaNacimiento != "").ToList()
                                    };

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                    SolicitudIFP solicitudPlus;
                                    DateTime fechaCotizacion2 = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE"));
                                    solicitudPlus = servicioCotizador.ObtenerDatosSolicitudIFP(solicitud.Id);

                                    HttpContext.Current.Session["fecCotizacion"] = fechaCotizacion2;

                                    ////// Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                                    ////if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                                    ////{
                                    ////    sol.PorcentajeDescuentoComision = Convert.ToDouble(dcom, new CultureInfo("es-PE"));
                                    ////}
                                    ////else
                                    ////{
                                    ////    if (solicitudPlus != null)
                                    ////    {
                                    ////        sol.PorcentajeDescuentoComision = solicitudPlus.PorcentajeDescuentoComision;
                                    ////    }
                                    ////    else
                                    ////    {
                                    ////        sol.PorcentajeDescuentoComision = null;
                                    ////    }
                                    ////}

                                    //Validando la fecha de cotizacion segun rol
                                    switch ((string)HttpContext.Current.Session["RolAzman"])
                                    {
                                        case "JEF.RVI.OPE"://JefeOperaciones
                                        case "AST.RVI.OPE"://AsistenteOperaciones
                                            break;
                                        default:
                                            solicitud.FechaCotizacion = Convert.ToDateTime(solicitudPlus.FechaSolicitud, new CultureInfo("es-PE"));
                                            solicitud.FechaSolicitud = Convert.ToDateTime(solicitudPlus.FechaSolicitud, new CultureInfo("es-PE"));
                                            solicitud.FechaDevengue = Convert.ToDateTime(solicitudPlus.FechaDevengue, new CultureInfo("es-PE"));

                                            break;
                                    }

                                    //if (((string)HttpContext.Current.Session["RolAzman"]) != "JEF.RVI.OPE")
                                    //{
                                    //    throw new Exception("Ud. no tiene acceso para actualizar!!");
                                    //}

                                    if (((string)HttpContext.Current.Session["RolAzman"]) == "JEF.RVI.OPE")
                                    {
                                        HttpContext.Current.Session["ModSolModo"] = "M";
                                    }
                                    else
                                    {
                                        if (DateTime.Today == solicitud.FechaSolicitud)
                                            HttpContext.Current.Session["ModSolModo"] = "M";
                                        else
                                            HttpContext.Current.Session["ModSolModo"] = "CONS";
                                    }

                                    List<GrupoFamiliar> lben = new List<GrupoFamiliar>();
                                    idBeneficiarios.ForEach(id => lben.Add(((List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"])[id]));

                                    solicitud.Beneficiarios = lben;

                                    //solicitud.FechaVigencia = solicitud.FechaCotizacion;
                                    //List<Parametro> lstDiasVigencia = new List<Parametro>();
                                    //lstDiasVigencia = servicioCotizador.ObtenerParametrosPorTabla("PLUS");
                                    //
                                    //if (lstDiasVigencia.Count() > 0)
                                    //{
                                    //    solicitud.FechaVigencia = solicitud.FechaVigencia.Value.AddDays(Convert.ToInt32(lstDiasVigencia[0].Valor_1));
                                    //}
                                    solicitud.FechaVigencia = solicitudPlus.FechaVigencia;

                                    //List<ParametrosMotorIFP> lstParametrosMotorIFP = (List<ParametrosMotorIFP>)HttpContext.Current.Session["ParametroMotorGeneral"];

                                    //if (lstParametrosMotorIFP == null)
                                    //    lstParametrosMotorIFP = new List<ParametrosMotorIFP>();


                                    List<ParametrosMotorIFP> lstParametrosMotorIFPEnvio = new List<ParametrosMotorIFP>();


                                    ////List<CotizacionIFP> lstCotizacion = (from x in sol.Cotizaciones
                                    ////                                     group x.Item by new { x.Temporalidad, x.Moneda }
                                    ////                                         into g
                                    ////                                         select new CotizacionIFP { Temporalidad = g.Key.Temporalidad, Moneda = g.Key.Moneda, Item= g.Sum() }).ToList();

                                    //var lstCotizacion = sol.Cotizaciones.Select(m => new { Temporalidad = m.Temporalidad.Id, Moneda = m.Moneda.Id }).Distinct().ToList();

                                    //foreach (var item in lstCotizacion)
                                    //{
                                    //    ParametrosMotorIFP parametrosMotorIFP = lstParametrosMotorIFP.Find(p => p.cod_monedaIFP == item.Moneda && p.cod_tipo_temporalidadIFP == item.Temporalidad && p.fec_cotizacionIFP == sol.FechaCotizacion.Value);
                                    //    if (parametrosMotorIFP == null)
                                    //    {
                                    //        parametrosMotorIFP = servicioCotizador.ObtenerParametroGenerales(item.Temporalidad, item.Moneda, sol.FechaCotizacion.Value, true);
                                    //        lstParametrosMotorIFP.Add(parametrosMotorIFP);
                                    //        HttpContext.Current.Session["ParametroMotorGeneral"] = lstParametrosMotorIFP;
                                    //    }
                                    //    lstParametrosMotorIFPEnvio.Add(parametrosMotorIFP);
                                    //}

                                    List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];
                                    if (listaParametro.Find(p => p.Id == "INDLOGS" && p.Nombre == "LogCotizacion") != null)
                                    {
                                        solicitud.isLogCotizacion = listaParametro.Find(p => p.Id == "INDLOGS" && p.Nombre == "LogCotizacion").Valor_1 == "S" ? true : false;
                                        solicitud.isLogReserva = false;
                                    }

                                    foreach (var cotizacion in solicitud.Cotizaciones)
                                    {
                                        if (cotizacion.Plan.Id == Enums.Planes.PLAN2.StringValue())
                                        {
                                            log.Debug("Modificar" + cotizacion.ValPjeDevFallec);
                                            cotizacion.ValPjeDevFallec = 0.00;
                                        }
                                    }

                                    if (solicitud.Beneficiarios.Count() > 1)
                                    {
                                        solicitud.TipoPlan = new TipoPlan { Id = Enums.TipoPlan.Familiar.StringValue() };
                                    }
                                    else
                                    {
                                        solicitud.TipoPlan = new TipoPlan { Id = Enums.TipoPlan.Individual.StringValue() };
                                    }

                                    //var conyuge = sol.Beneficiarios.FindAll(b => b.Parentesco.Id == Enums.Parentesco.Conyuge.StringValue());
                                    //var hijos = sol.Beneficiarios.FindAll(b => b.Parentesco.Id == Enums.Parentesco.Hijo.StringValue() || b.Parentesco.Id == Enums.Parentesco.Nieto.StringValue());
                                    //var padres = sol.Beneficiarios.FindAll(b => b.Parentesco.Id == Enums.Parentesco.Padre.StringValue());
                                    bool solicitudPlan3 = false;

                                    foreach (var itemCot in solicitud.Cotizaciones)
                                    {
                                        if (itemCot.Plan.Id == Enums.Planes.PLAN3.StringValue())
                                        {
                                            solicitudPlan3 = true;
                                            //if (conyuge.Count > 0)
                                            //{
                                            //    if (hijos.Count == 0 && padres.Count == 0)
                                            //    {
                                            //        // Sólo cónyuge
                                            //        if (itemCot.ValPjeConyuge == 0) itemCot.ValPjeConyuge = 42;
                                            //    }
                                            //    else
                                            //    {
                                            //        // Cónyuge + Hijo(s) y/o Padre(s)
                                            //        if (itemCot.ValPjeConyuge == 0) itemCot.ValPjeConyuge = 35;
                                            //    }
                                            //}
                                            //CalcularPorcentajesBeneficiarios(sol.Beneficiarios, (int)itemCot.ValPjeConyuge);

                                            solicitud.Beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue()).ValPjeRenta = 100.00;

                                            double pjeTotal = 0.00;

                                            solicitud.Beneficiarios.ForEach(ben =>
                                            {
                                                if (ben.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                                                { pjeTotal += ben.ValPjeRenta; }
                                            });

                                            if (pjeTotal > 100)
                                            {
                                                throw new Exception(string.Format("La suma del porcentaje total de los beneficiarios es superior a 100%"));
                                            }

                                            foreach (var beneficiarioPorcentaje in solicitud.Beneficiarios)
                                            {
                                                if (beneficiarioPorcentaje.ValPjeRenta == 0)
                                                {
                                                    throw new Exception(string.Format("El porcentaje del beneficiario no puede ser 0%"));
                                                    break;
                                                }
                                            }

                                        }

                                    }

                                    if (!solicitudPlan3)
                                    {
                                        var titular = solicitud.Beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());
                                        titular.ValPjeRenta = 100;
                                        titular.ValPjeAdicional = 0;

                                        List<GrupoFamiliar> beneficiarioTitular = new List<GrupoFamiliar>();
                                        beneficiarioTitular.Add(titular);
                                        solicitud.Beneficiarios = beneficiarioTitular;
                                    }

                                    if (solicitudPlan3)
                                    {
                                        if (solicitud.Beneficiarios.Count() > 1)
                                        {
                                            solicitud.TipoPlan = new TipoPlan { Id = Enums.TipoPlan.Familiar.StringValue() };
                                        }
                                        else
                                        {
                                            solicitud.TipoPlan = new TipoPlan { Id = Enums.TipoPlan.Individual.StringValue() };
                                        }
                                    }

                                    respuesta = servicioCotizador.ActualizarSolicitudIFP(tokenUsuario, ref solicitud);

                                    // Guardar en log de auditoría
                                    string nombreTerminal = string.Empty;
                                    try
                                    {
                                        nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                    }
                                    catch (Exception)
                                    {
                                        log.Warn(string.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                            HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                    }

                                    nombreTerminal += HttpContext.Current.Request.UserAgent;

                                    string tra = string.Empty;
                                    string dcom = string.Empty;
                                    int correlativo = 1;
                                    foreach (var itemCot in solicitud.Cotizaciones)
                                    {
                                        tra += "[" + itemCot.Correlativo + ": " + itemCot.AjusteTRA + "] ";
                                        dcom += "[" + itemCot.Correlativo + ": " + itemCot.ValPjeDCOM + "] ";
                                        correlativo += 1;
                                    }

                                    // Se coloca el usuario en una variable local para que lo pueda usar el hilo
                                    string usuario = (string)HttpContext.Current.Session["Usuario"];

                                    // Registro de formato de Estudio de Necesidades
                                    Task hilo = new Task(() =>
                                    {
                                        try
                                        {
                                            // Se vuelve a obtener la solicitud desde la Base de datos para que
                                            // refresque los ´Número de Cotización nuevos
                                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                            solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(solicitud.Id);

                                            EstudioNecesidadAPI estudioNecesidadAPI = new EstudioNecesidadAPI();
                                            SolicitudEdNAPI solicitudEDN = new SolicitudEdNAPI
                                            {
                                                num_solicitud = solicitud.Id,
                                                fec_solicitud = solicitud.FechaSolicitud.Value,
                                                val_mto_cta_individual = solicitud.PrimaUnica,
                                                cod_moneda_cta_indiv = solicitud.MonedaPrimaUnica.Id
                                            };

                                            estudioNecesidadAPI.solicitud = solicitudEDN;
                                            estudioNecesidadAPI.cotizaciones = new List<CotizacionEdNAPI>();
                                            foreach (CotizacionIFP c in solicitud.Cotizaciones)
                                            {
                                                CotizacionEdNAPI cotizacionEDN = new CotizacionEdNAPI
                                                {
                                                    num_correlativo = c.Correlativo,
                                                    cod_tipo_temporalidad = c.Temporalidad.Anhos.ToString(),
                                                    val_per_diferido = c.ValPerDiferido
                                                };
                                                estudioNecesidadAPI.cotizaciones.Add(cotizacionEDN);
                                            }

                                            log.Info("Cantidad de beneficiarios(modificar cotización): " + solicitud.Beneficiarios.Count);
                                            var beneficiarios = solicitud.Beneficiarios.GroupBy(be => new { be.Identificacion.IdTipo, be.Identificacion.Numero }).Select(s => s.First()).ToList();
                                            log.Info("Cantidad de beneficiarios(modificar cotización): " + beneficiarios.Count);

                                            estudioNecesidadAPI.beneficiarios = new List<BeneficiarioEdNAPI>();
                                            foreach (GrupoFamiliar b in beneficiarios)
                                            {
                                                BeneficiarioEdNAPI beneficiarioEDN = new BeneficiarioEdNAPI
                                                {
                                                    ape_paterno = b.ApellidoPaterno,
                                                    ape_materno = b.ApellidoMaterno,
                                                    nom_persona = b.Nombre,
                                                    num_identificacion = b.Identificacion.Numero,
                                                    cod_parentezco = b.Parentesco.Id
                                                };

                                                switch (b.Identificacion.IdTipo)
                                                {
                                                    case "D":
                                                        beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                                        break;
                                                    case "E":
                                                        beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.CE.StringValue();
                                                        break;
                                                    case "P":
                                                        beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.PAS.StringValue();
                                                        break;
                                                    default:
                                                        beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                                        break;
                                                }
                                                estudioNecesidadAPI.beneficiarios.Add(beneficiarioEDN);
                                            }

                                            log.Debug(string.Format("Se va a registrar el formato EDN de la solicitud [{0}]", solicitud.Id));
                                            var respuestaEdN = Utilitario.GenerarEstudioNecesidades(estudioNecesidadAPI, usuario);
                                            log.Debug(string.Format("Respuesta EDN: Solicitud[{0}] Estado[{1}] Mensaje[{2}]", solicitud.Id, respuestaEdN.Estado, respuestaEdN.Mensaje));
                                        }
                                        catch (Exception ex)
                                        {
                                            log.Error(string.Format("Error en generación del formato de Estudio de Necesidades: Solicitud[{0}]", solicitud.Id), ex);
                                        }
                                    });
                                    hilo.Start();

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = usuario,
                                        IdTipoEvento = Enums.EventoLog.CotizarSolicitud.StringValue(),
                                        Detalle = "Solicitud modificada: " + solicitud.Id + ", DCOM: " + dcom + ", DTRA: " + tra
                                    });
                                }
                                else
                                {
                                    solicitud = new SolicitudIFP();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                solicitud = new SolicitudIFP();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            solicitud = new SolicitudIFP();
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.SolicitudIFPActualizar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        solicitud = new SolicitudIFP();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    solicitud.Respuesta = respuesta;
                    log.Debug("Fin Cotizador.ModificarSolicitud");
                    return solicitud;
                }
                catch (Exception ex)
                {
                    solicitud = new SolicitudIFP();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    solicitud.Respuesta = respuesta;
                    return solicitud;
                }
            }
        }

        [WebMethod]
        public static Respuesta ExportarSolicitudPDF(string idSolicitud, string fecCotizacion, string numAgente)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));

                    HttpContext.Current.Session["idSolicitud"] = idSolicitud;
                    HttpContext.Current.Session["fecCotizacion"] = fechaCotizacion.ToString("yyyyMMdd");
                    HttpContext.Current.Session["AgenteReporte"] = numAgente;

                    string usuario = (string)HttpContext.Current.Session["Usuario"].ToString();

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    int ind_rescate = servicioCotizador.ObtenerIndicadorRescateIFP(idSolicitud, usuario);

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
                catch (Exception ex)
                {
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = "Error";
                    respuesta.Icono = "error";
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static CorreoElectronico CrearDatosCorreo(string tokenUsuario, string idSolicitud, string fecCotizacion, string tipoCotizacion, string nombre, string apellidoPaterno, string apellidoMaterno, string sexo)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                CorreoElectronico correo;
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPEnviarCorreo))
                        {
                            // Validar la cartera del agente
                            if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                                fecCotizacion = fechaCotizacion.ToString("yyyyMMdd");

                                ReportViewer visorReporte = new ReportViewer();
                                visorReporte.ProcessingMode = ProcessingMode.Remote;
                                visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteDetallePropuestaPlus"];

                                ReportParameter p1 = new ReportParameter("wl_solicitud", idSolicitud);

                                log.Info(string.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteDetallePropuestaPlus"]));

                                log.Debug(string.Format("Parámetros del reporte: wl_solicitud[{0}].",
                                    idSolicitud));

                                visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1 });
                                log.Debug(string.Format("Reporte para solicitud [{0}] procesado.", idSolicitud));

                                string format = "PDF", mimeType, encoding, extension;
                                string[] streamids;
                                Warning[] warnings;

                                log.Debug(string.Format("Se va a exportar a formato PDF el reporte para solicitud [{0}].", idSolicitud));
                                byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                                HttpContext.Current.Session["ArchivoPDF"] = bytes;
                                log.Debug(string.Format("Reporte para solicitud [{0}] exportado y almacenado en sesión de usuario.", idSolicitud));

                                SeccionCorreo config = (SeccionCorreo)ConfigurationManager.GetSection("correo");

                                correo = new CorreoElectronico
                                {
                                    De = (string)HttpContext.Current.Session["CorreoElectronico"],
                                    DeNombre = (string)HttpContext.Current.Session["NombreCompleto"],
                                    ParaNombre = string.Format("{0} {1}, {2}", apellidoPaterno, apellidoMaterno, nombre),
                                    Asunto = config.Asunto.Texto
                                                    .Replace("{NombreAfiliado}", nombre)
                                                    .Replace("{ApellidoPaternoAfiliado}", apellidoPaterno)
                                                    .Replace("{ApellidoMaternoAfiliado}", apellidoMaterno),
                                    Adjunto = "Propuesta.pdf (" + Utilitarios.FormatearBytes(bytes.Length, false) + ")",
                                    Mensaje = config.Mensaje.Texto
                                                    .Replace("{TratamientoAfiliado}", (sexo == "M") ? ("Sr.") : ("Sra."))
                                                    .Replace("{ApellidoPaternoAfiliado}", apellidoPaterno)
                                                    .Replace("{ApellidoMaternoAfiliado}", apellidoMaterno)
                                                    .Replace("{NombreAfiliado}", nombre)
                                                    .Replace("{TipoCotizacion}", tipoCotizacion)
                                                    .Replace("{NombreAgente}", (string)HttpContext.Current.Session["NombreCompleto"])
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
                                correo.Respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DireccionActualizar.StringValue()));
                            correo = new CorreoElectronico();
                            correo.Respuesta = new Respuesta();
                            correo.Respuesta.Estado = Constante.COD_ERROR;
                            correo.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            correo.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            correo.Respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    correo = new CorreoElectronico();
                    correo.Respuesta = new Respuesta();
                    correo.Respuesta.Estado = Constante.COD_ERROR;
                    correo.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    correo.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    correo.Respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                    return correo;
                }
            }
        }

        [WebMethod]
        public static Respuesta EnviarCorreoElectronico(string tokenUsuario, CorreoElectronico correo)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudIFPEnviarCorreo))
                        {
                            // Validar la cartera del agente
                            if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                correo.Adjunto = "Cotizacion.pdf";
                                correo.BinarioAdjunto = (byte[])HttpContext.Current.Session["ArchivoPDF"];
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.EnviarCorreoElectronico(correo);
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DireccionActualizar.StringValue()));
                            correo = new CorreoElectronico();
                            correo.Respuesta = new Respuesta();
                            correo.Respuesta.Estado = Constante.COD_ERROR;
                            correo.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            correo.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            correo.Respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                }
                return respuesta;
            }
        }

        [WebMethod]
        public static Actividad ObtenerDatosActividad(string cuspp, string idActividad)
        {
            return ((List<Actividad>)HttpContext.Current.Session["Actividades"]).Find(x => x.Correlativo == idActividad);
        }

        [WebMethod]
        public static Respuesta ExportarReporteEscenariosPDF(string tokenUsuario, string idSolicitud, string fecCotizacion, string maxAcom)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudReporteEscenarios))
                        {
                            // Validar la cartera del agente
                            if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.GenerarReporteEscenarios(idSolicitud, fechaCotizacion, (string)HttpContext.Current.Session["Usuario"], maxAcom);

                                string nombreTerminal = String.Empty;

                                if (respuesta.Estado == Constante.COD_OK)
                                {

                                    ReportViewer visorReporte = new ReportViewer();
                                    visorReporte.ProcessingMode = ProcessingMode.Remote;
                                    visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                    visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteEscenarios"];

                                    ReportParameter p1 = new ReportParameter("wl_num_solcitud", idSolicitud);
                                    ReportParameter p2 = new ReportParameter("wl_cod_username", (string)HttpContext.Current.Session["Usuario"]);

                                    log.Info(string.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                            ConfigurationManager.AppSettings["DominioReportingServices"],
                                            ConfigurationManager.AppSettings["RutaReporteEscenarios"]));
                                    log.Debug(string.Format("Parámetros del reporte: wl_num_solcitud[{0}] wl_cod_username[{1}].",
                                            idSolicitud, (string)HttpContext.Current.Session["Usuario"]));
                                    visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2 });
                                    log.Debug(string.Format("Reporte para solicitud [{0}] procesado.", idSolicitud));

                                    string mimeType, encoding, extension;
                                    string[] streamids;
                                    Warning[] warnings;

                                    string format = "PDF";
                                    log.Debug(string.Format("Se va a exportar a formato PDF el reporte para solicitud [{0}].", idSolicitud));
                                    byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                                    HttpContext.Current.Session["EscenariosPDF"] = bytes;
                                    log.Debug(string.Format("Reporte para solicitud [{0}] exportado y almacenado en sesión de usuario.", idSolicitud));

                                }
                                else if (respuesta.Estado == Constante.COD_ERROR)
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { respuesta.Mensaje });
                                }

                                try
                                {
                                    nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                }
                                catch (Exception)
                                {
                                    log.Warn(string.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                        HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                }

                                nombreTerminal += HttpContext.Current.Request.UserAgent;

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                    NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                    IdTipoEvento = Enums.EventoLog.ReporteEscenarios.StringValue(),
                                    Detalle = string.Format("Escenarios para solicitud {0} exportados a formato PDF", idSolicitud)
                                });

                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }

                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                    Enums.OpcionesSistema.SolicitudReporteEscenarios.StringValue()));
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

                }
                catch (Exception ex)
                {
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }

        protected void BusAfiBuscar_RP_Click(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var opcionesSistema = Session["OpcionesSistema"];
                    var listaAgentes = Session["ListaAgentes"];
                    var rolAzman = Session["RolAzman"];
                    var usuarioSesion = Session["Usuario"];

                    if (Utilitarios.ValidarPermiso(opcionesSistema, Enums.OpcionesSistema.DatosAfiliadoConsultar))
                    {

                        if (ValidarBusquedaAfiliados())
                        {
                            LimpiarFormularios();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(BusAfiNroSolicitud_RP.Text, BusAfiCUSPP_RP.Text, "", "", Enums.TipoProducto.IFP.StringValue());

                            log.Info("Usuario realizó búsqueda de afiliados por "
                                + ((BusAfiNroSolicitud_RP.Text.Trim().Length != 0)
                                ? ("Solicitud [" + BusAfiNroSolicitud_RP.Text.ToUpper() + "]")
                                : ("CUSPP [" + BusAfiCUSPP_RP.Text.ToUpper() + "]")) + ".");

                            if (afiliado != null)
                            {
                                CargarDatosAfiliado(afiliado, "CUSPP");

                                // Si el agente está cesado no mostrar el botón de Nueva Solicitud
                                OpcionSistema opcionInsertarSolicitud = ((List<OpcionSistema>)opcionesSistema).Find(o => o.IdAzman == (int)Enums.OpcionesSistema.SolicitudIFPInsertar);
                                if (opcionInsertarSolicitud.Activa)
                                {
                                    HabilitarControl(NuevaSolicitud_IFP);
                                    PerNuevaSolicitud_RP.Value = "1";
                                    if (!Utilitario.PerteneceACartera(afiliado.Agente.Id, afiliado.CUSPP, (List<Agente>)listaAgentes, (string)rolAzman, (string)usuarioSesion, false))
                                    {
                                        InhabilitarControl(NuevaSolicitud_IFP);
                                        PerNuevaSolicitud_RP.Value = "0";
                                    }
                                }

                                if (rolAzman.ToString() == Enums.RolAzman.AgenteExterno.StringValue())
                                {
                                    RestriccionInteligo(afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion, "1");
                                }
                                else
                                {
                                    RestriccionInteligo(afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion, "2");
                                }

                            }
                            else
                            {
                                List<string> errores = new List<string>();
                                if (BusAfiNroSolicitud_RP.Text.Trim().Length > 0)
                                {
                                    errores.Add("Solicitud N° <strong>" + BusAfiNroSolicitud_RP.Text.ToUpper() + "</strong> no se encuentra registrada. Verifique.");
                                }
                                else
                                {
                                    errores.Add("CUSPP <strong>" + BusAfiCUSPP_RP.Text.ToUpper() + "</strong> no se encuentra registrado. Verifique.");
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
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.DatosAfiliadoConsultar.StringValue()));
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstado.Value = "1";
                    }
                }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
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

        private void LimpiarFormularios()
        {
            // Datos principales
            CUSPP_RP.Text = string.Empty;
            HCUSPP_RP.Value = string.Empty;
            ApellidoPaterno_RP.Text = string.Empty;
            ApellidoMaterno_RP.Text = string.Empty;
            Nombres_RP.Text = string.Empty;
            FechaNacimiento_RP.Text = string.Empty;
            Sexo_RP.SelectedIndex = Sexo_RP.Items.IndexOf(Sexo_RP.Items.FindByValue("0"));
            CorreoElectronico_RP.Text = string.Empty;
            CorreoElectronicoRegistrado_RP.Value = string.Empty;
            //Categoria_RP.SelectedIndex = Categoria_RP.Items.IndexOf(Categoria_RP.Items.FindByValue("0"));
            //HCategoria_RP.Value = "0";
            AFP_RP.SelectedIndex = AFP_RP.Items.IndexOf(AFP_RP.Items.FindByValue("0"));
            HAFP_RP.Value = "0";
            SaldoCIC_RP.Text = string.Empty;
            Session["Vendedor"] = null;
            Session["Cartera"] = null;
            HToken_IFP.Value = string.Empty;

            RangoInversion_RP.Text = string.Empty;
            CentroLaboral_RP.Text = string.Empty;

            CorreoElectronico_RP.CssClass = CorreoElectronico_RP.CssClass.Replace(" formTextboxError", string.Empty);
            //Categoria_RP.CssClass = Categoria_RP.CssClass.Replace(" formComboboxError", String.Empty);
            AFP_RP.CssClass = AFP_RP.CssClass.Replace(" formComboboxError", string.Empty);
            SaldoCIC_RP.CssClass = SaldoCIC_RP.CssClass.Replace(" formTextboxError", string.Empty);

            RangoInversion_RP.CssClass = RangoInversion_RP.CssClass.Replace(" formTextboxError", string.Empty);
            CentroLaboral_RP.CssClass = CentroLaboral_RP.CssClass.Replace(" formTextboxError", string.Empty);

            // Direcciones
            NuevaDireccion_RP.Visible = false;

            // Teléfonos
            //oculto
            ////NuevoTelefono_RP.Visible = false;

            // Datos de empresa
            //NombreEmpresa_RP.Text = String.Empty;
            //DireccionEmpresa_RP.Text = String.Empty;
            //CiudadEmpresa_RP.SelectedIndex = CiudadEmpresa_RP.Items.IndexOf(CiudadEmpresa_RP.Items.FindByValue("0"));
            //ComunaEmpresa_RP.SelectedIndex = ComunaEmpresa_RP.Items.IndexOf(ComunaEmpresa_RP.Items.FindByValue("0"));
            //TelefonoEmpresa_RP.Text = String.Empty;

            // Botón Guardar Afiliado
            ContenedorGuardar_RP.Visible = false;

            // Grupo Familiar
            NuevoBeneficiario_RP.Visible = false;

            // Solicitudes
            NuevaSolicitud_IFP.Visible = false;
            ModEnvCorDe.Text = string.Empty;
            ModEnvCorPara.Text = string.Empty;
            ModEnvCorAsunto.Text = string.Empty;

            //INI.YRV
            TipoDocumento_RP.SelectedIndex = 0;
            NumeroDocumento_RP.Text = string.Empty;
            //FIN.YRV

            Telefono_RP.Text = string.Empty;
            Celular_RP.Text = string.Empty;

            //inteligo
            manejoPestanasBusqueda();
        }

        private bool ValidarBusquedaAfiliados()
        {
            bool esCorrecto = true;
            List<string> errores = new List<string>();

            MCMEstado.Value = "0";
            BusAfiNroSolicitud_RP.CssClass = "formTextbox";
            BusAfiCUSPP_RP.CssClass = "formTextbox";

            bool solicitud = (BusAfiNroSolicitud_RP.Text.Trim().Length > 0) ? true : false;
            bool cuspp = (BusAfiCUSPP_RP.Text.Trim().Length > 0) ? true : false;

            if (!(solicitud | cuspp))
            {
                errores.Add("Debe ingresar un criterio de búsqueda.");
                BusAfiNroSolicitud_RP.CssClass = "formTextbox formTextboxError";
                BusAfiCUSPP_RP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }

            if (solicitud & cuspp)
            {
                errores.Add("Sólo debe ingresar un criterio de búsqueda.");
                BusAfiNroSolicitud_RP.CssClass = "formTextbox formTextboxError";
                BusAfiCUSPP_RP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }

            if (cuspp && BusAfiCUSPP_RP.Text.Trim().Length != 12)
            {
                errores.Add("El <strong>CUSPP</strong> debe contener 12 caracteres.");
                BusAfiCUSPP_RP.CssClass = "formTextbox formTextboxError";
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

            return true;
        }

        private static bool ValidarBusquedaAfiliados(List<string> errores, List<string> controles, string nroSolicitud, string nroCuspp)
        {
            bool esCorrecto = true;

            // Nro. Solicitud
            bool solicitud = true;
            bool eSolicitud = (nroSolicitud.Trim().Length > 0) ? true : false;

            // CUSPP
            bool cuspp = true;
            bool eCuspp = (nroCuspp.Trim().Length > 0) ? true : false;

            if (!(eSolicitud | eCuspp))
            {
                errores.Add("Debe ingresar un criterio de búsqueda.");
                solicitud = false;
                cuspp = false;
            }

            if (eSolicitud & eCuspp)
            {
                errores.Add("Sólo debe ingresar un criterio de búsqueda.");
                solicitud = false;
                cuspp = false;
            }

            if (eCuspp && nroCuspp.Trim().Length != 12)
            {
                errores.Add("El <strong>CUSPP</strong> debe contener 12 caracteres.");
                cuspp = false;
            }

            // Clases de controles
            if (!solicitud) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!cuspp) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            esCorrecto = solicitud & cuspp;

            return esCorrecto;
        }

        private static bool ValidarDireccion(string glsDireccion, string idDepartamento, string idCiudad, string idComuna, string idPrincipal, string glsEspacioUrbano, string idDomicilio, List<string> errores, List<string> controles)
        {
            bool esCorrecto = true;

            // Dirección
            bool direccion = true;
            if (glsDireccion.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Dirección</strong>. Dato Obligatorio.");
                direccion = false;
            }

            // Departamento
            bool departamento = true;
            if (idDepartamento == "0")
            {
                errores.Add("Ingrese el campo <strong>Departamento</strong>. Dato Obligatorio.");
                departamento = false;
            }

            // Ciudad
            bool ciudad = true;
            if (idCiudad == "0")
            {
                errores.Add("Ingrese el campo <strong>Provincia</strong>. Dato Obligatorio.");
                ciudad = false;
            }

            // Comuna
            bool comuna = true;
            if (idComuna == "0")
            {
                errores.Add("Ingrese el campo <strong>Distrito</strong>. Dato Obligatorio.");
                comuna = false;
            }

            // Principal
            bool principal = true;
            if (idPrincipal == "0")
            {
                errores.Add("Ingrese el campo <strong>Principal</strong>. Dato Obligatorio.");
                principal = false;
            }

            // Espacio Urbano
            bool espaciourbano = true;
            if (glsEspacioUrbano.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Espacio Urbano</strong>. Dato Obligatorio.");
                espaciourbano = false;
            }

            // Domicilio
            bool domicilio = true;
            if (idDomicilio.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Domicilio</strong>. Dato Obligatorio.");
                domicilio = false;
            }

            // Clases de controles
            if (!direccion) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!departamento) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!ciudad) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!comuna) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!principal) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }

            if (!espaciourbano) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!domicilio) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }


            esCorrecto = direccion & departamento & ciudad & comuna & principal & espaciourbano & domicilio;

            return esCorrecto;
        }

        private static bool ValidarTelefono(string idTipo, string numTelefono, string idPrincipal, List<string> errores, List<string> controles)
        {
            bool esCorrecto = true;

            // Tipo
            bool tipo = true;
            if (idTipo == "0")
            {
                errores.Add("Ingrese el campo <strong>Tipo</strong>. Dato Obligatorio.");
                tipo = false;
            }

            // Número
            bool numero = true;
            if (numTelefono.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Número</strong>. Dato Obligatorio.");
                numero = false;
            }
            else
            {
                numTelefono =
                    numTelefono
                    .Replace('+', '0')
                    .Replace('*', '0')
                    .Replace('#', '0');
                if (!Regex.IsMatch(numTelefono, @"^\d+$"))
                {
                    errores.Add("El campo <strong>Número</strong> debe contener sólo números o los caracteres (+), (*) y (#).");
                    numero = false;
                }
            }

            // Principal
            bool principal = true;
            if (idPrincipal == "0")
            {
                errores.Add("Ingrese el campo <strong>Principal</strong>. Dato Obligatorio.");
                principal = false;
            }

            // Clases de controles
            if (!tipo) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!numero) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!principal) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }

            esCorrecto = tipo & numero & principal;

            return esCorrecto;
        }

        private static bool ValidarGrupoFamiliar(List<string> errores, List<string> controles, string glsApellidoPaterno, string glsApellidoMaterno, string glsNombres, string idTipoIdentificacion, string glsNumeroIdentificacion, string idParentesco, string idSexo, string fecNacimiento, string idInvalidez, string idTipoInvalidez, string fecInvalidez, string cuspp)
        {

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(cuspp);

            var beneficiarioRepetido = grupos.FindAll(gf => gf.Identificacion.IdTipo == idTipoIdentificacion && gf.Identificacion.Numero == glsNumeroIdentificacion);

            if (beneficiarioRepetido.Count > 1)
            {
                errores.Add("Error al administrar los beneficiarios, hay números de DNI iguales.");
                return false;
            }

            bool esCorrecto = true;

            // Apellido Paterno
            bool apellidoPaterno = true;

            // Apellido Materno
            bool apellidoMaterno = true;

            // Nombres
            bool nombres = true;

            // Tipo de Indentificación
            bool tipoIdentificacion = true;

            // Número de Identificación
            bool numeroIdentificacion = true;
            if (glsNumeroIdentificacion.Trim().Length > 0)
            {
                if (!Regex.IsMatch(glsNumeroIdentificacion, @"^\d+$"))
                {
                    errores.Add("El campo <strong>Nro. de Identificación</strong> debe contener un valor numérico.");
                    numeroIdentificacion = false;
                }
            }

            // Parentesco
            bool parentesco = true;
            if (idParentesco == "0")
            {
                errores.Add("Ingrese el campo <strong>Parentesco</strong>. Dato Obligatorio.");
                parentesco = false;
            }

            // Sexo
            bool sexo = true;
            if (idSexo == "0")
            {
                errores.Add("Ingrese el campo <strong>Sexo</strong>. Dato Obligatorio.");
                sexo = false;
            }

            // Fecha de Nacimiento
            bool fechaNacimiento = true;
            if (fecNacimiento.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Nacimiento</strong>. Dato Obligatorio.");
                fechaNacimiento = false;
            }
            else
            {
                DateTime vFechaNacimiento;
                if (!DateTime.TryParse(fecNacimiento, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaNacimiento))
                {
                    errores.Add("El campo <strong>Fecha de Nacimiento</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaNacimiento = false;
                }

                else
                {
                    if (vFechaNacimiento > DateTime.Now)
                    {
                        errores.Add("La <strong>Fecha de Nacimiento</strong> no puede ser mayor al día de hoy.");
                        fechaNacimiento = false;
                    }
                }

            }

            //// Invalidez
            //bool invalidez = true;
            //bool tipoInvalidez = true;
            //bool fechaInvalidez = true;
            //if (idInvalidez == "0")
            //{
            //    errores.Add("Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.");
            //    invalidez = false;
            //}
            //else if (idInvalidez == Enums.Invalidez.No.StringValue())
            //{
            //    if (idTipoInvalidez != Enums.TipoInvalidez.NoInvalido.StringValue())
            //    {
            //        errores.Add("El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.");
            //        tipoInvalidez = false;
            //    }
            //    if (fecInvalidez.Trim().Length > 0)
            //    {
            //        errores.Add("El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.");
            //        fechaInvalidez = false;
            //    }
            //}
            //else if (idInvalidez == Enums.Invalidez.Si.StringValue())
            //{
            //    if (idTipoInvalidez != Enums.TipoInvalidez.Parcial.StringValue() && idTipoInvalidez != Enums.TipoInvalidez.Total.StringValue())
            //    {
            //        errores.Add("El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.");
            //        tipoInvalidez = false;
            //    }

            //    if (fecInvalidez.Trim().Length == 0)
            //    {
            //        errores.Add("Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.");
            //        fechaInvalidez = false;
            //    }
            //    else
            //    {
            //        DateTime vFechaInvalidez;
            //        if (!DateTime.TryParse(fecInvalidez, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaInvalidez))
            //        {
            //            errores.Add("El campo <strong>Fecha de Invalidez</strong> debe contener una fecha válida (dd/mm/aaaa).");
            //            fechaInvalidez = false;
            //        }
            //    }
            //}

            //if (idTipoInvalidez == "0")
            //{
            //    errores.Add("Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.");
            //    tipoInvalidez = false;
            //}

            // Clases de controles
            if (!apellidoPaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!apellidoMaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!nombres) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!tipoIdentificacion) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!numeroIdentificacion) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!parentesco) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!sexo) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!fechaNacimiento) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            //if (!invalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            //if (!tipoInvalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            //if (!fechaInvalidez) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }

            //& invalidez & tipoInvalidez & fechaInvalidez

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento;

            return esCorrecto;
        }

        private static bool ValidarSolicitud(List<string> errores, List<string> controles, string fecCotizacion, string fecDevengue, string valMonedaPrimaUnica, string valPrimaUnica, List<CotizacionIFP> listaCotizaciones, List<int> idBeneficiarios, List<CoberturaAdicional> coberturasAdicionales)
        {
            bool esCorrecto;

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            int cotizacionesExistentes = servicioCotizador.CantidadSolicitudes(HttpContext.Current.Session["CUSPPIFP"].ToString(), valMonedaPrimaUnica, Convert.ToDouble(valPrimaUnica));
            int totalCotizaciones = listaCotizaciones.FindAll(c => c.Correlativo == 0).Count + cotizacionesExistentes;
            if (totalCotizaciones > Convert.ToInt32(ConfigurationManager.AppSettings["MaxCotizacionesIFP"]))
            {
                esCorrecto = false;
                errores.Add("No es posible cotizar, ha alcanzado el límite de cotizaciones.");
                return esCorrecto;
            }

            //Moneda Prima Única
            bool monedaPrimaUnica = true;
            if (valMonedaPrimaUnica == "0")
            {
                errores.Add("Seleccione el campo <strong>Moneda Prima Única</strong>. Dato Obligatorio.");
                monedaPrimaUnica = false;
            }

            //Prima Única
            bool primaUnica = true;
            if (valPrimaUnica.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Prima Única</strong>. Dato Obligatorio.");
                primaUnica = false;
            }
            else
            {
                double vPrimaUnica;
                if (!double.TryParse(valPrimaUnica, NumberStyles.Any, new CultureInfo("es-PE"), out vPrimaUnica))
                {
                    errores.Add("El campo <strong>Saldo CIC</strong> debe contener un valor numérico.");
                    primaUnica = false;
                }
                else
                {
                    if (vPrimaUnica <= 0)
                    {
                        errores.Add("El campo <strong>Saldo CIC</strong> debe contener un valor positivo.");
                        primaUnica = false;
                    }
                }
            }

            //Fecha de Cotización
            bool fechaCotizacion = true;
            if (fecCotizacion.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Cotización</strong>. Dato Obligatorio.");
                fechaCotizacion = false;
            }
            else
            {
                DateTime vFechCotizacion;
                if (!DateTime.TryParse(fecCotizacion, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechCotizacion))
                {
                    errores.Add("El campo <strong>Fecha de Cotización</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaCotizacion = false;
                }
            }

            //Fecha de Devengue
            bool fechaDevengue = true;
            if (fecDevengue.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Devengue</strong>. Dato Obligatorio.");
                fechaDevengue = false;
            }
            else
            {
                DateTime vFechaDevengue;
                if (!DateTime.TryParse(fecDevengue, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaDevengue))
                {
                    errores.Add("El campo <strong>Fecha de Devengue</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaDevengue = false;
                }
            }

            //bool valRequisitosTra = true;
            //List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];
            //string numAgente = numAgenteSol;

            // Clases de controles
            if (!primaUnica) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            //if (!monedaPrimaUnica) { controles.Add("formCombobox formComboboxError"); } else { controles.Add("formCombobox"); }
            if (!fechaCotizacion) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!fechaDevengue) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!monedaPrimaUnica) { controles.Add("formCombobox formComboboxError"); } else { controles.Add("formCombobox"); }
            //if (!dcom) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            //Cotizaciones
            bool cotizaciones = true;
            if (!(listaCotizaciones.Count > 0))
            {
                errores.Add("Debe realizar al menos una cotización.");
                cotizaciones = false;
            }

            //Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            bool redLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);
            bool dcomplan1 = true;
            bool dcomplan2 = true;
            bool dcomplan3 = true;

            List<CotizacionIFP> listaCotizacionesPlan1 = new List<CotizacionIFP>();
            List<CotizacionIFP> listaCotizacionesPlan2 = new List<CotizacionIFP>();
            List<CotizacionIFP> listaCotizacionesPlan3 = new List<CotizacionIFP>();

            listaCotizacionesPlan1 = listaCotizaciones.FindAll(c => c.Plan.Id == Enums.Planes.PLAN1.StringValue());

            for (int i = 0; i < listaCotizacionesPlan1.Count; i++)
            {

                //Validando el Sepelio
                if (listaCotizacionesPlan1[i].IndGastoSepelio != "S")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Seleccione el campo <strong>Sepelio</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                }

                //Validando la Moneda
                if (listaCotizacionesPlan1[i].Moneda.Id == "000")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Seleccione el campo <strong>Moneda</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    //controles.Add(listaCotizacionesPlan1[i].Item + ",1,PLAN1");
                    controles.Add((i) + ",1,PLAN1");
                }

                //Validando Pago Doble
                if (listaCotizacionesPlan1[i].PagoDoble > listaCotizacionesPlan1[i].Temporalidad.Anhos)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Pago Doble</strong>. No puede ser Mayor a la temporalidad.");
                    cotizaciones = false;
                    controles.Add((i) + ",4,PLAN1");
                }

                if (listaCotizacionesPlan1[i].PagoDoble > 0)
                {
                    if (listaCotizacionesPlan1[i].PjePagoDoble == 0)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Pago Doble</strong>. No puede ser 0.");
                        cotizaciones = false;
                    }

                }
                else
                {

                    if (listaCotizacionesPlan1[i].PjePagoDoble > 0)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Pago Doble</strong>. No puede ser mayor a 0.");
                        cotizaciones = false;
                    }

                }

                //Validando la Temporalidad
                if (Convert.ToInt32(listaCotizacionesPlan1[i].Temporalidad.Anhos) == 0 || Convert.ToInt32(listaCotizacionesPlan1[i].Temporalidad.Anhos) > 15)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Temporalidad</strong>. No se encuentra dentro de los rangos permitidos.");
                    cotizaciones = false;
                    controles.Add((i) + ",2,PLAN1");
                }
                else
                {

                    if (Convert.ToInt32(listaCotizacionesPlan1[i].Temporalidad.Anhos) == 15)
                    {
                        if (listaCotizacionesPlan1[i].ValPerDiferido > 7.5)
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Temporalidad</strong>. No se encuentra dentro de los rangos permitidos.");
                            cotizaciones = false;
                            controles.Add((i) + ",2,PLAN1");
                        }
                    }
                    else
                    {
                        if (listaCotizacionesPlan1[i].ValPerDiferido > 5)
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Temporalidad</strong>. No se encuentra dentro de los rangos permitidos.");
                            cotizaciones = false;
                            controles.Add((i) + ",2,PLAN1");
                        }
                    }

                }

                //Coberturas
                if (listaCotizacionesPlan1[i].ValPjeDev == 0)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>% Dev. Sobrev.</strong>. No puede ser 0.");
                    cotizaciones = false;
                    controles.Add((i) + ",5,PLAN1");
                }
                else
                {
                    if (listaCotizacionesPlan1[i].ValPjeDevFallec > listaCotizacionesPlan1[i].ValPjeDev)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>% Dev. Fallec.</strong>. No puede ser Mayor al % Dev. Sobrev.");
                        cotizaciones = false;
                        controles.Add((i) + ",6,PLAN1");
                    }
                }

                if (listaCotizacionesPlan1[i].ValPjeDevFallec == 0)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>% Dev. Fallec.</strong>. No puede ser 0.");
                    cotizaciones = false;
                    controles.Add((i) + ",6,PLAN1");
                }

                if (Convert.ToInt32(listaCotizacionesPlan1[i].Temporalidad.Anhos) == Convert.ToInt32(listaCotizacionesPlan1[i].ValPerDiferido))
                {

                    if (listaCotizacionesPlan1[i].ValPjeDev != 100)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>% Dev. Sobrev.</strong>. No puede ser diferente del 100%");
                        cotizaciones = false;
                        controles.Add((i) + ",5,PLAN1");
                    }

                    if (listaCotizacionesPlan1[i].ValPjeDevFallec != 100)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>% Dev. Fallec.</strong>. No puede ser diferente del 100%");
                        cotizaciones = false;
                        controles.Add((i) + ",6,PLAN1");
                    }

                }

                //DCOM
                if (redLocal)
                {
                    double vDcom;
                    if (listaCotizacionesPlan1[i].ValPjeDCOM.ToString().Trim().Length == 0) listaCotizacionesPlan1[i].ValPjeDCOM = 0;
                    if (!double.TryParse(listaCotizacionesPlan1[i].ValPjeDCOM.ToString(), NumberStyles.Any, new CultureInfo("es-PE"), out vDcom))
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje DCOM</strong> debe contener un valor numérico.");
                        dcomplan1 = false;
                        controles.Add((i) + ",7,PLAN1");
                    }
                    else
                    {
                        if (vDcom < 0)
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje DCOM</strong> debe contener un valor positivo.");
                            dcomplan1 = false;
                            controles.Add((i) + ",7,PLAN1");
                        }
                    }
                }

                /*Implementacion DCOM, solamente cuando al configuracion sea S*/
                if (fechaCotizacion)
                {
                    string KeyDcom = (string)ConfigurationManager.AppSettings["keyDcom"];

                    List<RolDcom> listaRolDcom = (List<RolDcom>)HttpContext.Current.Session["ComboRangoIFPDCOM"];

                    double vDcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));
                    string dcomRangos = String.Empty;

                    if (dcomplan1 && KeyDcom == "S" && listaRolDcom.Count > 0)
                    {
                        if (listaCotizacionesPlan1[i].ValPjeDCOM.ToString().Trim().Length == 0) listaCotizacionesPlan1[i].ValPjeDCOM = 0;

                        dcomplan1 = listaRolDcom.Any(dc => dc.ValorDcom == listaCotizacionesPlan1[i].ValPjeDCOM);
                    }

                    if (!dcomplan1)
                    {
                        errores.Add(string.Format("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje DCOM</strong>. No se encuentra dentro de los valores permitidos."));
                        controles.Add((i) + ",7,PLAN1");
                    }

                }

            }

            listaCotizacionesPlan2 = listaCotizaciones.FindAll(c => c.Plan.Id == Enums.Planes.PLAN2.StringValue());

            for (int i = 0; i < listaCotizacionesPlan2.Count; i++)
            {

                //Validando el Sepelio
                if (listaCotizacionesPlan2[i].IndGastoSepelio != "S")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Seleccione el campo <strong>Sepelio</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                }

                //Validando la Moneda
                if (listaCotizacionesPlan2[i].Moneda.Id == "000")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Seleccione el campo <strong>Moneda</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    controles.Add((i) + ",1,PLAN2");
                }

                //Validando Pago Doble
                if (listaCotizacionesPlan2[i].PagoDoble > listaCotizacionesPlan2[i].Temporalidad.Anhos)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Pago Doble</strong>. No puede ser Mayor a la temporalidad.");
                    cotizaciones = false;
                    controles.Add((i) + ",4,PLAN2");
                }

                if (listaCotizacionesPlan2[i].PagoDoble > 0)
                {
                    if (listaCotizacionesPlan2[i].PjePagoDoble == 0)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Pago Doble</strong>. No puede ser 0.");
                        cotizaciones = false;
                    }
                }
                else
                {
                    if (listaCotizacionesPlan2[i].PjePagoDoble > 0)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Pago Doble</strong>. No puede ser mayor a 0.");
                        cotizaciones = false;
                    }
                }

                //Validando la Temporalidad
                if (Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos) == 0 || Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos) > 25)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Temporalidad</strong>. No se encuentra dentro de los rangos permitidos.");
                    cotizaciones = false;
                    controles.Add((i) + ",2,PLAN2");
                }
                else
                {

                    if (Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos) != listaCotizacionesPlan2[i].PeriodoGarantizado)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Temporalidad</strong>. Debe ser igual al Período Garantizado.");
                        cotizaciones = false;
                        controles.Add(i + ",2,PLAN2");
                    }

                    if (Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos) == 5 || Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos) == 7 || Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos) == 10)
                    {
                        if (listaCotizacionesPlan2[i].ValPerDiferido > Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos))
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Diferido</strong>. No se encuentra dentro de los rangos permitidos.");
                            cotizaciones = false;
                            controles.Add((i) + ",3,PLAN2");
                        }
                    }
                    else if (Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos) == 15 || Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos) == 20 || Convert.ToInt32(listaCotizacionesPlan2[i].Temporalidad.Anhos) == 25)
                    {
                        if (listaCotizacionesPlan2[i].ValPerDiferido > 10)
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Temporalidad</strong>. No se encuentra dentro de los rangos permitidos.");
                            cotizaciones = false;
                            controles.Add((i) + ",2,PLAN2");
                        }
                    }

                }

                //DCOM
                if (redLocal)
                {
                    double vDcom;

                    if (listaCotizacionesPlan2[i].ValPjeDCOM.ToString().Trim().Length == 0) listaCotizacionesPlan2[i].ValPjeDCOM = 0;

                    if (!double.TryParse(listaCotizacionesPlan2[i].ValPjeDCOM.ToString(), NumberStyles.Any, new CultureInfo("es-PE"), out vDcom))
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje DCOM</strong> debe contener un valor numérico.");
                        dcomplan2 = false;
                        controles.Add((i) + ",6,PLAN2");
                    }
                    else
                    {
                        if (vDcom < 0)
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje DCOM</strong> debe contener un valor positivo.");
                            dcomplan2 = false;
                            controles.Add((i) + ",6,PLAN2");
                        }
                    }
                }

                /*Implementacion DCOM, solamente cuando al configuracion sea S*/
                if (fechaCotizacion)
                {
                    string KeyDcom = (string)ConfigurationManager.AppSettings["keyDcom"];

                    List<RolDcom> listaRolDcom = (List<RolDcom>)HttpContext.Current.Session["ComboRangoIFPDCOM"];

                    double vDcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));

                    if (dcomplan2 && KeyDcom == "S" && listaRolDcom.Count > 0)
                    {
                        if (listaCotizacionesPlan2[i].ValPjeDCOM.ToString().Trim().Length == 0) listaCotizacionesPlan2[i].ValPjeDCOM = 0;

                        dcomplan2 = listaRolDcom.Any(dc => dc.ValorDcom == listaCotizacionesPlan2[i].ValPjeDCOM);

                    }
                    if (!dcomplan2)
                    {
                        errores.Add(string.Format("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje DCOM</strong>. No se encuentra dentro de los valores permitidos."));
                        controles.Add((i) + ",6,PLAN2");
                    }
                }

            }

            listaCotizacionesPlan3 = listaCotizaciones.FindAll(c => c.Plan.Id == Enums.Planes.PLAN3.StringValue());

            bool bPjeCACy = false, bPjeCAPa = false, bPjeCAMa = false;

            for (int i = 0; i < listaCotizacionesPlan3.Count; i++)
            {

                //Validando el Sepelio
                if (listaCotizacionesPlan3[i].IndGastoSepelio != "S")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Seleccione el campo <strong>Sepelio</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                }

                //Validando la Moneda
                if (listaCotizacionesPlan3[i].Moneda.Id == "000")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Seleccione el campo <strong>Moneda</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    controles.Add((i) + ",1,PLAN3");
                }

                //Validando Pago Doble
                /*if (listaCotizacionesPlan3[i].PagoDoble > listaCotizacionesPlan3[i].PeriodoGarantizado)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Pago Doble</strong>. No puede ser Mayor a la temporalidad.");
                    cotizaciones = false;
                    controles.Add((i) + ",4,PLAN3");
                }*/

                if (listaCotizacionesPlan3[i].PagoDoble > 0)
                {
                    if (listaCotizacionesPlan3[i].PjePagoDoble == 0)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Pago Doble</strong>. No puede ser 0.");
                        cotizaciones = false;
                    }
                }
                else
                {
                    if (listaCotizacionesPlan3[i].PjePagoDoble > 0)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Pago Doble</strong>. No puede ser mayor a 0.");
                        cotizaciones = false;
                    }
                }

                //Validando la Período Garantizado
                //if (Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado) == 0 || Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado) > 25)
                if (Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado) > 25)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Garantizado</strong>. No se encuentra dentro de los rangos permitidos.");
                    cotizaciones = false;
                    controles.Add((i) + ",2,PLAN3");
                }
                else
                {

                    if (Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado) == 5 || Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado) == 7 || Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado) == 10)
                    {
                        if (listaCotizacionesPlan3[i].ValPerDiferido > Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado))
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Diferido</strong>. No se encuentra dentro de los rangos permitidos.");
                            cotizaciones = false;
                            controles.Add((i) + ",3,PLAN3");
                        }
                    }
                    else if (Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado) == 15 || Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado) == 20 || Convert.ToInt32(listaCotizacionesPlan3[i].PeriodoGarantizado) == 25)
                    {
                        if (listaCotizacionesPlan3[i].ValPerDiferido > 10)
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Garantizado</strong>. No se encuentra dentro de los rangos permitidos.");
                            cotizaciones = false;
                            controles.Add((i) + ",2,PLAN3");
                        }
                    }

                }

                //Coberturas Adicionales
                //double PjeCATot = listaCotizacionesPlan3[i].ValPjeCACy + listaCotizacionesPlan3[i].ValPjeCAPa + listaCotizacionesPlan3[i].ValPjeCAMa;

                /*if (PjeCATot > 100)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Total % CA</strong>. No puede exceder el 100%.");
                    cotizaciones = false;
                    controles.Add((i) + ",8,PLAN3");
                }*/

                //Coberturas Adicionales - fecha nacimiento
                if (coberturasAdicionales != null)
                {
                    if (listaCotizacionesPlan3[i].ValPjeCACy != 0 && !bPjeCACy)
                    {
                        var CA_CY = coberturasAdicionales.FindLast(c => c.Parentesco == Enums.Parentesco.Conyuge.StringValue());
                        if (CA_CY != null)
                        {
                            if (CA_CY.FechaNacimiento == "")
                            {
                                errores.Add("Debe indicar la <strong>fecha de nacimiento</strong> del cónyuge");
                                cotizaciones = false;
                                controles.Add("0,0,PLAN3_CA");
                                bPjeCACy = true;
                            }
                            else
                            {
                                if (!validaFecha(Convert.ToDateTime(CA_CY.FechaNacimiento, CultureInfo.CreateSpecificCulture("es-PE"))))
                                {
                                    errores.Add("El formato de la <strong>fecha</strong> es incorrecto");
                                    cotizaciones = false;
                                    controles.Add("0,0,PLAN3_CA");
                                    bPjeCACy = true;
                                }
                            }

                            if (CA_CY.Sexo == "0")
                            {
                                errores.Add("Debe indicar el <strong>sexo</strong> del cónyuge");
                                cotizaciones = false;
                                controles.Add("0,1,PLAN3_CA");
                                bPjeCACy = true;
                            }
                        }
                    }

                    if (listaCotizacionesPlan3[i].ValPjeCAPa != 0 && !bPjeCAPa)
                    {
                        var CA_PA = coberturasAdicionales.FindLast(c => c.Parentesco == Enums.Parentesco.Padre.StringValue() && c.Sexo == "M");
                        if (CA_PA != null)
                        {
                            if (CA_PA.FechaNacimiento == "")
                            {
                                errores.Add("Debe indicar la <strong>fecha de nacimiento</strong> del padre");
                                cotizaciones = false;
                                controles.Add("0,2,PLAN3_CA");
                                bPjeCAPa = true;
                            }
                            else
                            {
                                if (!validaFecha(Convert.ToDateTime(CA_PA.FechaNacimiento, CultureInfo.CreateSpecificCulture("es-PE"))))
                                {
                                    errores.Add("El formato de la <strong>fecha</strong> es incorrecto");
                                    cotizaciones = false;
                                    controles.Add("0,2,PLAN3_CA");
                                    bPjeCAPa = true;
                                }
                            }
                        }
                    }
                }

                if (listaCotizacionesPlan3[i].ValPjeCAMa != 0 && !bPjeCAMa)
                {
                    var CA_MA = coberturasAdicionales.FindLast(c => c.Parentesco == Enums.Parentesco.Padre.StringValue() && c.Sexo == "F");
                    if (CA_MA != null)
                    {
                        if (CA_MA.FechaNacimiento == "")
                        {
                            errores.Add("Debe indicar la <strong>fecha de nacimiento</strong> de la madre");
                            cotizaciones = false;
                            controles.Add("0,3,PLAN3_CA");
                            bPjeCAMa = true;
                        }
                        else
                        {
                            if (!validaFecha(Convert.ToDateTime(CA_MA.FechaNacimiento, CultureInfo.CreateSpecificCulture("es-PE"))))
                            {
                                errores.Add("El formato de la <strong>fecha</strong> es incorrecto");
                                cotizaciones = false;
                                controles.Add("0,3,PLAN3_CA");
                                bPjeCAMa = true;
                            }
                        }
                    }
                }

                //DCOM
                if (redLocal)
                {
                    double vDcom;

                    if (listaCotizacionesPlan3[i].ValPjeDCOM.ToString().Trim().Length == 0) listaCotizacionesPlan3[i].ValPjeDCOM = 0;

                    if (!double.TryParse(listaCotizacionesPlan3[i].ValPjeDCOM.ToString(), NumberStyles.Any, new CultureInfo("es-PE"), out vDcom))
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje DCOM</strong> debe contener un valor numérico.");
                        dcomplan3 = false;
                        controles.Add((i) + ",9,PLAN3");
                    }
                    else
                    {
                        if (vDcom < 0)
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje DCOM</strong> debe contener un valor positivo.");
                            dcomplan3 = false;
                            controles.Add((i) + ",9,PLAN3");
                        }
                    }
                }

                /*Implementacion DCOM, solamente cuando al configuracion sea S*/
                if (fechaCotizacion)
                {
                    string KeyDcom = (string)ConfigurationManager.AppSettings["keyDcom"];

                    List<RolDcom> listaRolDcom = (List<RolDcom>)HttpContext.Current.Session["ComboRangoIFPDCOM"];

                    double vDcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));

                    if (dcomplan3 && KeyDcom == "S" && listaRolDcom.Count > 0)
                    {
                        if (listaCotizacionesPlan3[i].ValPjeDCOM.ToString().Trim().Length == 0) listaCotizacionesPlan3[i].ValPjeDCOM = 0;

                        dcomplan3 = listaRolDcom.Any(dc => dc.ValorDcom == listaCotizacionesPlan3[i].ValPjeDCOM);

                    }
                    if (!dcomplan3)
                    {
                        errores.Add(string.Format("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje DCOM</strong>. No se encuentra dentro de los valores permitidos."));
                        controles.Add((i) + ",9,PLAN3");
                    }
                }
            }

            List<GrupoFamiliar> lben = new List<GrupoFamiliar>();
            if (idBeneficiarios.Count > 0)
            {
                idBeneficiarios.ForEach(id => lben.Add(((List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"])[id]));
            }

            // Beneficiarios
            bool beneficiarios = true;
            if (!(idBeneficiarios.Count > 0))
            {
                errores.Add("Debe seleccionar al menos un beneficiario para realizar la cotización.");
                beneficiarios = false;
            }

            esCorrecto = monedaPrimaUnica & primaUnica & fechaCotizacion & fechaDevengue & dcomplan1 & dcomplan2 & dcomplan3 & cotizaciones & beneficiarios;

            return esCorrecto;
        }

        protected void Guardar_Click(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoActualizar))
                    {
                        // Validar la cartera del agente
                        if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == (string)Session["Vendedor"]) || (string)Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
                        {
                            if (CUSPP_RP.Text.Trim().Length == 0)
                            {
                                Session["Consentimiento"] = true;
                            }

                            if (ValidarAfiliado())
                            {
                                Respuesta respuesta;
                                string idTipoEvento, glsOk, glsError, detalleLog = "", CUSPP = "";
                                //CultureInfo ci = new CultureInfo();

                                Afiliado afiliado = new Afiliado
                                {
                                    CUSPP = CUSPP_RP.Text,
                                    TipoIdentificacion = TipoDocumento_RP.SelectedValue,
                                    NumeroIdentificacion = NumeroDocumento_RP.Text,
                                    ApellidoPaterno = ApellidoPaterno_RP.Text.ToUpper(),
                                    ApellidoMaterno = ApellidoMaterno_RP.Text.ToUpper(),
                                    Nombre = Nombres_RP.Text.ToUpper(),
                                    FechaNacimiento = Convert.ToDateTime(FechaNacimiento_RP.Text, new CultureInfo("es-PE")),
                                    Sexo = Convert.ToChar(Sexo_RP.SelectedValue),

                                    //CorreoElectronico = CorreoElectronico_RP.Text,
                                    //Categoria = new Categoria { Id = string.Empty },
                                    AFP = new AFP { Id = AFP_RP.SelectedValue },
                                    SaldoCIC = ((bool)Session["Consentimiento"]) ? (double?)Convert.ToDouble(SaldoCIC_RP.Text, new CultureInfo("es-PE")) : null,

                                    EstadoCivil = new Temporal { cod_parametro = EstadoCivil_RP.SelectedValue }

                                    //,Telefonos = Telefono_RP.Text
                                    //,Celulares = Celular_RP.Text

                                    ,
                                    RangoInversion = RangoInversion_RP.Text
                                    ,
                                    CentroLaboral = CentroLaboral_RP.Text
                                };

                                if (HToken_IFP.Value.Length > 0)
                                {

                                    if (HConsentimiento.Value == "OK")
                                    {
                                        afiliado.CorreoElectronicoCliente = CorreoElectronico_RP.Text;
                                        afiliado.TelefonoCliente = Telefono_RP.Text;
                                        afiliado.CelularCliente = Celular_RP.Text;

                                        afiliado.CorreoElectronico = "";
                                        afiliado.Telefonos = "";
                                        afiliado.Celulares = "";
                                    }
                                    else
                                    {
                                        afiliado.CorreoElectronico = CorreoElectronico_RP.Text;
                                        afiliado.Telefonos = Telefono_RP.Text;
                                        afiliado.Celulares = Celular_RP.Text;
                                    }

                                    //afiliado.CorreoElectronicoCliente = CorreoElectronico_RP.Text;
                                    //afiliado.TelefonoCliente = Telefono_RP.Text;
                                    //afiliado.CelularCliente = Celular_RP.Text;

                                    //afiliado.CorreoElectronico = "";
                                    //afiliado.Telefonos = "";
                                    //afiliado.Celulares = "";
                                }
                                else
                                {
                                    afiliado.CorreoElectronico = CorreoElectronico_RP.Text;
                                    afiliado.Telefonos = Telefono_RP.Text;
                                    afiliado.Celulares = Celular_RP.Text;

                                    afiliado.CorreoElectronicoCliente = "";
                                    afiliado.TelefonoCliente = "";
                                    afiliado.CelularCliente = "";
                                }

                                //HCategoria_RP.Value = Categoria_RP.SelectedValue;
                                HAFP_RP.Value = AFP_RP.SelectedValue;

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                string nombreTerminal = String.Empty;
                                try
                                {
                                    nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                }
                                catch (Exception)
                                {
                                    log.Warn(string.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                        Request.ServerVariables["remote_addr"]));
                                }

                                nombreTerminal += Request.UserAgent;

                                if (CUSPP_RP.Text.Trim().Length == 0)
                                {
                                    var usuario = Session["Usuario"].ToString();

                                    respuesta = servicioCotizador.RegistrarAfiliado(afiliado, usuario, ref CUSPP);

                                    afiliado.CUSPP = CUSPP;
                                    respuesta = servicioCotizador.ActualizarAfiliado(afiliado);

                                    idTipoEvento = Enums.EventoLog.RegistrarDatosCliente.StringValue();
                                    glsOk = "registrados";
                                    glsError = "registrar";
                                    detalleLog = string.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "RegistrarAfiliado", Environment.NewLine, Environment.NewLine, "afiliado", JsonConvert.SerializeObject(afiliado));

                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = Request.ServerVariables["remote_addr"],
                                        NombreUsuario = (string)Session["Usuario"],
                                        Detalle = string.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "ActualizarAfiliado", Environment.NewLine, Environment.NewLine, "afiliado", JsonConvert.SerializeObject(afiliado)),
                                        IdTipoEvento = Enums.EventoLog.ModificarDatosCliente.StringValue()
                                    });

                                }
                                else
                                {
                                    respuesta = servicioCotizador.ActualizarAfiliado(afiliado);
                                    idTipoEvento = Enums.EventoLog.ModificarDatosCliente.StringValue();
                                    glsOk = "actualizados";
                                    glsError = "actualizar";
                                    detalleLog = string.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "ActualizarAfiliado", Environment.NewLine, Environment.NewLine, "afiliado", JsonConvert.SerializeObject(afiliado));
                                }

                                obtenerConsentimiento(afiliado, false);

                                log.Info(string.Format("Usuario actualizó los datos del afiliado CUSPP[{0}].", CUSPP_RP.Text));
                                log.Debug(string.Format("Correo Electrónico[{0}] Estado Civil[{1}:{2}].",
                                    CorreoElectronico_RP.Text,
                                    EstadoCivil_RP.SelectedValue, EstadoCivil_RP.SelectedItem.Text));

                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = Request.ServerVariables["remote_addr"],
                                    NombreUsuario = (string)Session["Usuario"],
                                    Detalle = detalleLog,
                                    IdTipoEvento = idTipoEvento
                                });

                                if (respuesta.Estado == Constante.COD_OK)
                                {
                                    MCMMensaje.Text = "Datos del afiliado " + glsOk + " correctamente.";
                                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Exito.StringValue();
                                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                    MCMEstado.Value = "1";
                                    estado_mensaje = true;
                                    //CorreoElectronicoRegistrado_RP.Value = afiliado.CorreoElectronico;

                                    if (idTipoEvento == Enums.EventoLog.RegistrarDatosCliente.StringValue())
                                    {
                                        if (indNuevoAfiliado.Value == "SI") indNuevoAfiliado.Value = "NO";
                                        CUSPP_RP.Text = CUSPP;

                                        HCUSPP_RP.Value = CUSPP;
                                        Session["SHCUSPP"] = HCUSPP_RP.Value;

                                        //Direcciones
                                        NuevaDireccion_RP.Visible = true;

                                        //Teléfonos
                                        //oculto
                                        //NuevoTelefono_RP.Visible = true;

                                        TipoDocumentoBusqueda_RP.SelectedIndex = TipoDocumento_RP.SelectedIndex;
                                        NumeroDocumentoBusqueda_RP.Text = NumeroDocumento_RP.Text;

                                        //ClientScript.RegisterStartupScript(GetType(), "-", "$('#MCMIcono').attr('class', $('#MCMEstadoIcono').val()); $('#ModalCuadroMensaje').dialog({ title: $('#MCMEstadoTitulo').val() }); $('#ModalCuadroMensaje').dialog({ position: ['center', 'center'] }); $('#ModalCuadroMensaje').dialog('open');", true);


                                        BusAfiBuscar2_RP_Click(sender, e);
                                    }

                                }
                                else
                                {
                                    log.Error("No se pudo " + glsError + " la información de afiliado: [" + respuesta.Mensaje + "]");
                                    MCMMensaje.Text = "No se pudo " + glsError + " la información: [" + respuesta.Mensaje + "]";
                                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    MCMEstado.Value = "1";
                                }
                            }
                        }
                        else
                        {
                            List<string> errores = new List<string>();
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
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.DatosAfiliadoActualizar.StringValue()));
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstado.Value = "1";
                    }
                }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
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

        private static bool ValidarDatosAfiliado(List<string> errores, List<string> controles, string glsApellidoPaterno, string glsApellidoMaterno, string glsNombres)
        {
            bool esCorrecto = true;

            //Apellido Paterno
            bool apellidoPaterno = true;
            bool eApellidoPaterno = false;
            if (glsApellidoPaterno.Trim().Length > 0)
            {
                eApellidoPaterno = true;
                if (glsApellidoPaterno.Trim().Length < 2)
                {
                    errores.Add("El campo <strong>Apellido Paterno</strong> debe contener al menos 2 caracteres.");
                    apellidoPaterno = false;
                }
            }

            //Apellido Materno
            bool apellidoMaterno = true;
            bool eApellidoMaterno = false;
            if (glsApellidoMaterno.Trim().Length > 0)
            {
                eApellidoMaterno = true;
                if (glsApellidoMaterno.Trim().Length < 2)
                {
                    errores.Add("El campo <strong>Apellido Materno</strong> debe contener al menos 2 caracteres.");
                    apellidoMaterno = false;
                }
            }

            //Nombres
            bool nombres = true;
            bool eNombres = false;
            if (glsNombres.Trim().Length > 0)
            {
                eNombres = true;
                if (glsNombres.Trim().Length < 2)
                {
                    errores.Add("El campo <strong>Nombres</strong> debe contener al menos 2 caracteres.");
                    nombres = false;
                }
            }

            //Criterio mínimo
            bool criterioMinimo = true;
            if (!(eApellidoPaterno | eApellidoMaterno | eNombres))
            {
                errores.Add("Debe seleccionar al menos un criterio de búsqueda.");
                criterioMinimo = false;
            }

            // Clases de controles
            if (!apellidoPaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!apellidoMaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!nombres) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & criterioMinimo;

            return esCorrecto;
        }

        private bool ValidarAfiliado()
        {
            bool esCorrecto = true;
            List<string> errores = new List<string>();

            bool apellidoPaterno = true;
            bool apellidoMaterno = true;
            bool nombres = true;
            bool correoElectronico = true;
            bool tipoIdentificacion = true;
            bool numIdentificacion = true;
            bool sexo = true;
            //bool estadoCivil = true;
            bool fechaNacimiento = true;

            MCMEstado.Value = "0";
            //CorreoElectronico_RP.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro";
            //Categoria_RP.CssClass = "formCombobox";
            AFP_RP.CssClass = "formCombobox";
            SaldoCIC_RP.CssClass = "formTextbox";
            RangoInversion_RP.CssClass = "formTextbox";
            CentroLaboral_RP.CssClass = "formTextbox";

            //Tipo de identificacion
            if (TipoDocumento_RP.SelectedIndex <= 0)
            {
                errores.Add("Seleccione el campo <strong>Tipo de Identificación</strong>. Dato Obligatorio.");
                tipoIdentificacion = false;
            }

            //Numero de identificacion
            if (NumeroDocumento_RP.Text.Trim().Length <= 0)
            {
                errores.Add("Ingrese el campo <strong>Número de Identificación</strong>. Dato Obligatorio.");
                numIdentificacion = false;
            }
            else
            {
                if (TipoDocumento_RP.SelectedValue == Enums.TipoDocumento.DNI.StringValue() && NumeroDocumento_RP.Text.Trim().Length != 8)
                {
                    errores.Add("El <strong>DOCUMENTO NACIONAL DE IDENTIDAD</strong> debe contener 8 caracteres.");
                    NumeroDocumento_RP.CssClass = "formTextbox formTextboxError";
                    numIdentificacion = false;
                }
                else if (TipoDocumento_RP.SelectedValue == Enums.TipoDocumento.CE.StringValue() && NumeroDocumento_RP.Text.Trim().Length != 9)
                {
                    errores.Add("El <strong>CARNET EXTRANJERIA</strong> debe contener 9 caracteres.");
                    NumeroDocumento_RP.CssClass = "formTextbox formTextboxError";
                    numIdentificacion = false;
                }
                else if (TipoDocumento_RP.SelectedValue == Enums.TipoDocumento.RUCJ.StringValue() && NumeroDocumento_RP.Text.Trim().Length != 11)
                {
                    errores.Add("El <strong>RUC PERSONA JURIDICA</strong> debe contener 11 caracteres.");
                    NumeroDocumento_RP.CssClass = "formTextbox formTextboxError";
                    numIdentificacion = false;
                }
                else if (TipoDocumento_RP.SelectedValue == Enums.TipoDocumento.RUCN.StringValue() && NumeroDocumento_RP.Text.Trim().Length != 11)
                {
                    errores.Add("El <strong>RUC PERSONA NATURAL</strong> debe contener 11 caracteres.");
                    NumeroDocumento_RP.CssClass = "formTextbox formTextboxError";
                    numIdentificacion = false;
                }
            }

            //Apellido Paterno
            if (ApellidoPaterno_RP.Text.Trim().Length > 0)
            {
                if (ApellidoPaterno_RP.Text.Trim().Length < 2)
                {
                    errores.Add("El campo <strong>Apellido Paterno</strong> debe contener al menos 2 caracteres.");
                    apellidoPaterno = false;
                }
            }
            else
            {
                errores.Add("Ingrese el campo <strong>Apellido Paterno</strong>. Dato Obligatorio.");
                apellidoPaterno = false;
            }

            //Apellido Materno            
            if (ApellidoMaterno_RP.Text.Trim().Length > 0)
            {
                if (ApellidoMaterno_RP.Text.Trim().Length < 2)
                {
                    errores.Add("El campo <strong>Apellido Materno</strong> debe contener al menos 2 caracteres.");
                    apellidoMaterno = false;
                }
            }
            else
            {
                errores.Add("Ingrese el campo <strong>Apellido Materno</strong>. Dato Obligatorio.");
                apellidoMaterno = false;
            }

            //Nombres            
            if (Nombres_RP.Text.Trim().Length > 0)
            {
                if (Nombres_RP.Text.Trim().Length < 2)
                {
                    errores.Add("El campo <strong>Nombres</strong> debe contener al menos 2 caracteres.");
                    nombres = false;
                }
            }
            else
            {
                errores.Add("Ingrese el campo <strong>Nombres</strong>. Dato Obligatorio.");
                nombres = false;
            }

            //Fecha Nacimiento
            if (FechaNacimiento_RP.Text.Trim().Length <= 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Nacimiento</strong>. Dato Obligatorio.");
                fechaNacimiento = false;
            }

            //Sexo
            if (Sexo_RP.SelectedIndex <= 0)
            {
                errores.Add("Seleccione el campo <strong>Sexo</strong>. Dato Obligatorio.");
                sexo = false;
            }

            //Correo Electrónico            
            if ((bool)Session["Consentimiento"])
            {

                if (CorreoElectronico_RP.Text.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.");
                    correoElectronico = false;
                }
                else
                {
                    Regex regex = new Regex(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$");
                    Match match = regex.Match(CorreoElectronico_RP.Text);
                    if (!match.Success || CorreoElectronico_RP.Text.ToUpper().Contains("Ñ"))
                    {
                        errores.Add("El campo <strong>Correo Electrónico</strong> es inválido. Verifique.");
                        correoElectronico = false;
                    }
                }
            }

            ////Estado Civil
            //if (EstadoCivil_RP.SelectedIndex <= 0)
            //{
            //    errores.Add("Seleccione el campo <strong>Estado Civil</strong>. Dato Obligatorio.");
            //    estadoCivil = false;
            //}

            //// Categoría
            //bool categoria = true;
            //if (Categoria_RP.SelectedValue == "0")
            //{
            //	errores.Add("Ingrese el campo <strong>Categoría</strong>. Dato Obligatorio.");
            //	categoria = false;
            //}

            //// AFP
            //bool afp = true;
            //if (AFP_RP.SelectedValue == "0")
            //{
            //	errores.Add("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
            //	afp = false;
            //}

            //// Saldo CIC
            //bool saldoCIC = true;
            //if ((bool)Session["Consentimiento"])
            //{
            //	if (SaldoCIC_RP.Text.Trim().Length == 0)
            //	{
            //		errores.Add("Ingrese el campo <strong>Saldo CIC</strong>. Dato Obligatorio.");
            //		saldoCIC = false;
            //	}
            //	else
            //	{
            //		double vSaldoCIC;
            //		if (!double.TryParse(SaldoCIC_RP.Text, NumberStyles.Any, new CultureInfo("es-PE"), out vSaldoCIC))
            //		{
            //			errores.Add("El campo <strong>Saldo CIC</strong> debe contener un valor numérico.");
            //			saldoCIC = false;
            //		}
            //		else
            //		{
            //			if (vSaldoCIC <= 0)
            //			{
            //				errores.Add("El campo <strong>Saldo CIC</strong> debe contener un valor positivo.");
            //				saldoCIC = false;
            //			}
            //		}
            //	}
            //}

            // Clases de controles
            if (CUSPP_RP.Text.Trim().Length == 0)
            {
                if (!apellidoPaterno) { ApellidoPaterno_RP.CssClass = "formTextbox formTextboxError"; } else { ApellidoPaterno_RP.CssClass = "formTextbox"; }
                if (!apellidoMaterno) { ApellidoMaterno_RP.CssClass = "formTextbox formTextboxError"; } else { ApellidoMaterno_RP.CssClass = "formTextbox"; }
                if (!nombres) { Nombres_RP.CssClass = "formTextbox formTextboxError"; } else { Nombres_RP.CssClass = "formTextbox"; }
                if (!correoElectronico) { CorreoElectronico_RP.CssClass = "formTextbox formTextboxError"; } else { CorreoElectronico_RP.CssClass = "formTextbox"; }
                if (!tipoIdentificacion) { TipoDocumento_RP.CssClass = "formCombobox formComboboxError"; } else { TipoDocumento_RP.CssClass = "formCombobox"; }
                if (!numIdentificacion) { NumeroDocumento_RP.CssClass = "formTextbox formTextboxError"; } else { NumeroDocumento_RP.CssClass = "formTextbox"; }
                if (!sexo) { Sexo_RP.CssClass = "formCombobox formComboboxError"; } else { Sexo_RP.CssClass = "formCombobox"; }
                //if (!estadoCivil) { EstadoCivil_RP.CssClass = "formCombobox formComboboxError"; } else { EstadoCivil_RP.CssClass = "formCombobox"; }
                if (!fechaNacimiento) { FechaNacimiento_RP.CssClass = "formTextbox formCalendar formTextboxError formCalendarError"; } else { FechaNacimiento_RP.CssClass = "fecha formTextbox formCalendar"; }

                Telefono_RP.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono";
                Celular_RP.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono";
            }

            //if (!correoElectronico) { CorreoElectronico_RP.CssClass = "formTextbox formTextboxError"; } else { CorreoElectronico_RP.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro"; }

            //if (!categoria) { Categoria_RP.CssClass = "formCombobox formComboboxError"; } else { Categoria_RP.CssClass = "formCombobox formComboboxReadOnly"; }
            //if (!afp) { AFP_RP.CssClass = "formCombobox formComboboxError"; } else { AFP_RP.CssClass = "formCombobox"; }
            //if (!saldoCIC) { SaldoCIC_RP.CssClass = "formTextbox formTextboxError numerico"; } else { SaldoCIC_RP.CssClass = "formTextbox numerico"; }

            //esCorrecto = correoElectronico & categoria & afp & saldoCIC;
            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numIdentificacion & correoElectronico & sexo & fechaNacimiento;

            if (!esCorrecto)
            {
                MCMMensaje.Text = Utilitarios.FormatearError(errores);
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Validacion.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                MCMEstado.Value = "1";
                indNuevoAfiliado.Value = "NO";
            }

            return esCorrecto;
        }

        [WebMethod]
        public static string CargarRolEscenario(string tokenUsuario, string idSolicitud, string fecCotizacion, string numAgente)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    HttpContext.Current.Session["AgenteReporte"] = numAgente;

                    var pagina = new Page();
                    var control = (TablaAcomMaximo)pagina.LoadControl("~/Controles/TablaAcomMaximo.ascx");

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                    DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                    RolAcom rolAcom = new RolAcom
                    {
                        CodRol = (string)HttpContext.Current.Session["RolAzman"],
                        FechaCotizacion = fechaCotizacion,
                        NumSolicitud = idSolicitud
                    };
                    List<RolAcom> listaRol = servicioCotizador.ListaAcomEscenario(rolAcom);

                    control.acomns = listaRol;

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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }


            }
        }

        public void LlenarPagoDoble(List<Parametro> lstParametro)
        {

            List<Parametro> lstParametro2 = new List<Parametro>();

            if (lstParametro.Count > 0)
            {
                lstParametro
                            .FindAll(p => (
                                           (p.Id == "I-RPP")
                                          )
                            )
                            .ForEach(p =>
                            {
                                for (var i = Convert.ToInt32(p.Valor_1); i <= Convert.ToInt32(p.Valor_2); i++)
                                {
                                    lstParametro2.Add(new Parametro { Id = i.ToString(), Glosa = i.ToString() });
                                }

                            });
            }
            HttpContext.Current.Session["ComboPagoDoble"] = lstParametro2;
        }

        [WebMethod]
        public static Respuesta ValidarVigencia(string tokenUsuario, string num_solicitud)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        List<string> errores = new List<string>();
                        List<string> controles = new List<string>();

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        DateTime fechaActual = DateTime.Today;
                        SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(num_solicitud);

                        respuesta.Estado = Constante.COD_OK;

                        //if (fechaActual > solicitud.FechaVigencia && solicitud.CodigoEstado == 0)
                        if (fechaActual > solicitud.FechaVigencia && (solicitud.CodigoEstado == 0 || solicitud.CodigoEstado == 1 || solicitud.CodigoEstado == 2))
                        {
                            log.Warn(string.Format("La solicitud no se encuentra vigente [{0}].", num_solicitud));
                            errores.Add("La solicitud no se encuentra vigente");
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                        }
                        List<Direccion> lstDireccion = servicioCotizador.ListarDireccion(solicitud.Afiliado.CUSPP);
                        if (lstDireccion.Count == 0)
                        {
                            log.Warn(string.Format("El Afiliado no cuenta con dirección, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
                            errores.Add("El Afiliado no cuenta con dirección");
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                        }
                        else
                        {
                            Direccion direccionPrincipal = lstDireccion.Find(d => d.Principal);
                            if (direccionPrincipal == null)
                            {
                                log.Warn(string.Format("El Afiliado no cuenta con dirección principal, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
                                errores.Add("El Afiliado no cuenta con dirección principal");
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                            }
                            else
                            {
                                if (direccionPrincipal.TipoVia == null)
                                {
                                    log.Warn(string.Format("El Afiliado no cuenta con tipo vía válida, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
                                    errores.Add("El Afiliado no cuenta con tipo vía válida");
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                }
                                else if (direccionPrincipal.TipoVia.Id.Length == 0)
                                {
                                    log.Warn(string.Format("El Afiliado no cuenta con tipo vía válida, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
                                    errores.Add("El Afiliado no cuenta con tipo vía válida");
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                }
                                else if (direccionPrincipal.TipoVia.Id == "0")
                                {
                                    log.Warn(string.Format("El Afiliado no cuenta con tipo vía válida, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
                                    errores.Add("El Afiliado no cuenta con tipo vía válida");
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                }

                            }
                        }

                        respuesta.Mensaje = Utilitarios.FormatearError(errores);
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
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        //INI.YRV
        protected void BusAfiBuscar2_RP_Click(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                var rolAzman = Session["RolAzman"];
                string usuario = Session["Usuario"].ToString();

                try
                {
                    bool indValida;

                    if (hcusppInteligo.Value.Length > 0)
                    {
                        indValida = true;
                    }
                    else
                    {
                        indValida = ValidarBusquedaAfiliadoIdentificacion();
                    }

                    if (indValida)
                    {
                        LimpiarFormularios();

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        Afiliado afiliado;

                        if (hcusppInteligo.Value.Length > 0)
                        {
                            afiliado = servicioCotizador.ObtenerDatosAfiliado("", hcusppInteligo.Value, "", "", Enums.TipoProducto.IFP.StringValue());
                        }
                        else
                        {
                            afiliado = servicioCotizador.ObtenerDatosAfiliado("", "", TipoDocumentoBusqueda_RP.SelectedValue, NumeroDocumentoBusqueda_RP.Text, Enums.TipoProducto.IFP.StringValue());
                        }

                        log.Info("Usuario realizó búsqueda de afiliados por "
                            + "TipoIdentificacion [" + TipoDocumentoBusqueda_RP.SelectedValue + "] "
                            + "NumeroIdentificacion [" + NumeroDocumentoBusqueda_RP.Text + "] ");

                        if (afiliado != null)
                        {
                            CargarDatosAfiliado(afiliado, "ID");

                            //if (Session["RolAzman"].ToString() == Enums.RolAzman.AgenteLima.StringValue() || Session["RolAzman"].ToString() == Enums.RolAzman.AgenteProvincia.StringValue())
                            //{
                            //    RestriccionInteligo(afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion, "2");
                            //}
                            //else 
                            if (rolAzman.ToString() == Enums.RolAzman.AgenteExterno.StringValue())
                            {
                                ValidarOrigenCotizacionVigente(afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion, usuario);
                            }
                            else
                            {
                                RestriccionInteligo(afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion, "2");
                            }

                            HCUSPP_RP.Value = afiliado.CUSPP;
                            Session["SHCUSPP"] = HCUSPP_RP.Value;
                            indNuevoAfiliado.Value = "NO";
                        }
                        else
                        {
                            if (rolAzman.ToString() == Enums.RolAzman.AgenteExterno.StringValue())
                            {
                                indNuevoAfiliado.Value = "SI";

                                //Botón Guardar Afiliado
                                ContenedorGuardar_RP.Visible = true;

                                ////Grupo Familiar
                                //NuevoBeneficiario_RP.Visible = true;

                                ////Solicitudes
                                //NuevaSolicitud_IFP.Visible = true; 
                            }
                            else
                            {
                                MCMMensaje.Text = "Cliente no encontrado. Verifique.";
                                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Informacion.StringValue();
                                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Informacion.StringValue();
                                MCMEstado.Value = "1";
                            }
                        }

                        if ((string)rolAzman == Enums.RolAzman.AgenteExterno.StringValue())
                        {
                            habilitaCampos();
                        }
                    }
                    else
                    {
                        manejoPestanasBusqueda();
                    }

                    hcusppInteligo.Value = "";

                }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
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

        private void ValidarOrigenCotizacionVigente(string TipoIdentificacion, string NumeroIdentificacion, string usuario)
        {
            var solicitud = servicioCotizador.ValidarCotizacionVigente(TipoIdentificacion, NumeroIdentificacion, usuario);

            bool puedeCotizar = false;

            if (solicitud != null)
            {
                if (solicitud.OrigenCotizacion == Enums.OrigenCotizacion.Interseguro.StringValue())
                {
                    puedeCotizar = false;
                }
                else if (solicitud.OrigenCotizacion == Enums.OrigenCotizacion.Inteligo.StringValue())
                {
                    List<Agente> listaAgentesExternos = (List<Agente>)HttpContext.Current.Session["ListaAgentesExternos"];
                    var agente = new Agente { Usuario = solicitud.AgenteCotizacion };
                    var usuarioAgente = listaAgentesExternos.Find(p => p.Usuario == agente.Usuario);

                    puedeCotizar = (usuarioAgente != null) ? true : false;
                }
            }
            else
            {
                puedeCotizar = true;
            }

            if (!puedeCotizar)
            {
                NuevaSolicitud_IFP.Visible = false;
                hPuedeCotizar.Value = "No será posible cotizar IFP hasta el " + solicitud.FechaVigencia.Value.ToString("dd/MM/yyyy");
            }
            else
            {
                NuevaSolicitud_IFP.Visible = true;
                hPuedeCotizar.Value = string.Empty;
            }
        }

        private void RestriccionInteligo(string tipoIdentificacion, string numeroIdentificacion, string origenEvaluar)
        {
            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
            string urlCPermitido = ConfigurationManager.AppSettings["url_cliente_permitido"].ToString();
            string usuario = Session["Usuario"].ToString(), accessToken = "";
            bool puedeCotizar = true;
            DateTime vigencia = new DateTime();

            /*Obtener token*/
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            log.Debug(string.Format("urlToken: {0}", urlToken));

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(
                    new
                    {
                        usuario = usuario
                    });

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                var jsonResult = streamReader.ReadToEnd();
                JObject jObject = JObject.Parse(jsonResult);
                accessToken = (string)jObject["accessToken"];
            }

            log.Debug(string.Format("accessToken: {0}", accessToken));

            /*Obtener indicador cliente permitido*/
            if (accessToken != "")
            {
                if (tipoIdentificacion == "" || tipoIdentificacion == null) tipoIdentificacion = "D";

                urlCPermitido = string.Format(urlCPermitido, tipoIdentificacion, numeroIdentificacion, origenEvaluar);

                httpWebRequest = (HttpWebRequest)WebRequest.Create(urlCPermitido);
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + accessToken));

                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var jsonResult = streamReader.ReadToEnd();
                    JObject jObject = JObject.Parse(jsonResult);
                    puedeCotizar = (bool)jObject["puedeCotizar"];
                    if (!puedeCotizar)
                    {
                        vigencia = Convert.ToDateTime(jObject["vigencia"]);
                    }
                }
            }

            if (!puedeCotizar)
            {
                NuevaSolicitud_IFP.Visible = false;
                hPuedeCotizar.Value = "No será posible cotizar IFP hasta el " + vigencia.ToString("dd/MM/yyyy");
            }
            else
            {
                hPuedeCotizar.Value = string.Empty;
            }
            //Session["puedeCotizar"] = puedeCotizar;
        }

        private void CargarDatosAfiliado(Afiliado afiliado, string tipoBusqueda)
        {
            string usuario = (string)HttpContext.Current.Session["Usuario"];

            //Guardar los datos del afiliado para generarlo como beneficiario en caso de que no exista
            //servicioCotizador.ActualizarAfiliado(afiliado);

            //MasterPage
            Panel cabecera, cabeceraProtegida;

            cabecera = (Panel)Master.FindControl("CabeceraSuperior");
            cabeceraProtegida = (Panel)Master.FindControl("CabeceraSuperiorProtegida");

            //inteligo
            //LineaTipoDocumento.Visible = false;
            manejoPestanasBusqueda();

            Session["Consentimiento"] = afiliado.Consentimiento;

            var ApellidoPaternoInicial = afiliado.ApellidoPaterno;
            var ApellidoMaternoInicial = afiliado.ApellidoMaterno;
            var NombreInicial = afiliado.Nombre;

            if (!afiliado.Consentimiento)
            {
                //MastePage
                cabecera.Visible = false;
                cabeceraProtegida.Visible = true;

                //Datos del Afiliado
                BusquedaAfiliados_RP.Visible = false;
                LineaCUSPP_RP.Visible = false;
                afiliado.ApellidoPaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoPaterno);
                afiliado.ApellidoMaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoMaterno);
                afiliado.Nombre = Utilitarios.EnmascararNombre(afiliado.Nombre);
                LineaNacimientoSexo_RP.Visible = false;
                LineaCorreoElectronico_RP.Visible = false;
                LineaAFP_RP.Visible = false;
                GrupoDireccion_RP.Visible = false;
                ContenedorGuardar_RP.Visible = false;
            }
            else
            {
                // MasterPage
                cabecera.Visible = true;
                cabeceraProtegida.Visible = false;
            }

            // Validar la cartera del agente
            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue()
                || Utilitario.PerteneceACartera(afiliado.Agente.Id, afiliado.CUSPP, (List<Agente>)Session["ListaAgentes"], (string)Session["RolAzman"], (string)Session["Usuario"], true))
            {
                if (tipoBusqueda == "CUSPP")
                {
                    if (BusAfiCUSPP_RP.Text.Trim().Length > 0)
                    {
                        Session["CUSPP"] = BusAfiCUSPP_RP.Text;
                        Session["NroSolicitud"] = null;
                        Session["CUSPP_PLUS"] = null;
                        Session["TipoDocumentoBusqueda"] = null;
                        Session["NumeroDocumentoBusqueda"] = null;
                        TipoDocumentoBusqueda_RP.SelectedIndex = 0;
                        NumeroDocumentoBusqueda_RP.Text = string.Empty;
                    }
                    else if (BusAfiNroSolicitud_RP.Text.Trim().Length > 0)
                    {
                        Session["CUSPP"] = null;
                        Session["NroSolicitud"] = BusAfiNroSolicitud_RP.Text;
                        Session["CUSPP_PLUS"] = afiliado.CUSPP.Trim();
                        Session["TipoDocumentoBusqueda"] = null;
                        Session["NumeroDocumentoBusqueda"] = null;
                        TipoDocumentoBusqueda_RP.SelectedIndex = 0;
                        NumeroDocumentoBusqueda_RP.Text = string.Empty;
                    }
                }
                else if (tipoBusqueda == "ID")
                {
                    if (TipoDocumentoBusqueda_RP.SelectedIndex != 0 && NumeroDocumentoBusqueda_RP.Text.Length > 0)
                    {
                        BusAfiCUSPP_RP.Text = string.Empty;
                        BusAfiNroSolicitud_RP.Text = string.Empty;
                        Session["CUSPP"] = null;
                        Session["NroSolicitud"] = null;
                        Session["CUSPP_PLUS"] = null;
                        Session["TipoDocumentoBusqueda"] = TipoDocumentoBusqueda_RP.SelectedValue;
                        Session["NumeroDocumentoBusqueda"] = NumeroDocumentoBusqueda_RP.Text;
                    }
                }
                // Datos principales
                CUSPP_RP.Text = afiliado.CUSPP.Trim();
                HCUSPP_RP.Value = afiliado.CUSPP.Trim();
                Session["SHCUSPP"] = HCUSPP_RP.Value;
                ApellidoPaterno_RP.Text = afiliado.ApellidoPaterno.Trim();
                ApellidoMaterno_RP.Text = afiliado.ApellidoMaterno.Trim();
                Nombres_RP.Text = afiliado.Nombre.Trim();

                if (afiliado.FechaNacimiento != null)
                {
                    FechaNacimiento_RP.Text = afiliado.FechaNacimiento.Value.ToString("dd/MM/yyyy");
                }

                Sexo_RP.SelectedIndex = Sexo_RP.Items.IndexOf(Sexo_RP.Items.FindByValue(afiliado.Sexo.ToString()));
                CorreoElectronico_RP.Text = afiliado.CorreoElectronico;
                CorreoElectronicoRegistrado_RP.Value = afiliado.CorreoElectronico;
                HAFP_RP.Value = afiliado.AFP.Id.ToString();
                AFP_RP.SelectedIndex = AFP_RP.Items.IndexOf(AFP_RP.Items.FindByValue(afiliado.AFP.Id.ToString()));

                //69310
                Categoria_RP.SelectedIndex = Categoria_RP.Items.IndexOf(Categoria_RP.Items.FindByValue(afiliado.Categoria.Id.ToString()));

                SaldoCIC_RP.Text = afiliado.SaldoCIC.ToString();

                RangoInversion_RP.Text = afiliado.RangoInversion.ToString();

                CentroLaboral_RP.Text = afiliado.CentroLaboral.ToString();

                if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
                    Session["Vendedor"] = "";
                else
                    Session["Vendedor"] = afiliado.Agente.Id;

                NumeroAgente.Value = afiliado.Agente.Id;
                Cartera.Value = afiliado.Agente.IdCartera;

                // Obteniendo datos del agente
                List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                Agente agente = listaAgentes.Find(a => a.Id == afiliado.Agente.Id);
                if (agente == null && Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]))
                {
                    agente = servicioCotizador.ObtenerUltimoAgentePorCartera(afiliado.Agente.IdCartera, usuario);
                }

                if (agente != null)
                {
                    NombreAgente.Value = agente.Nombre;
                    Agente.Text = string.Format("{0} - {1}", afiliado.Agente.Id, agente.Nombre);
                }
                else
                {
                    Agente.Text = "-";
                }

                Session["Cartera"] = afiliado.Agente.IdCartera;
                Session["AFP_RP"] = afiliado.AFP.Id.ToString();
                Session["CUSPP_RP"] = afiliado.CUSPP.ToString();
                Session["CUSPPIFP"] = afiliado.CUSPP.Trim();
                Session["DNIIFP"] = afiliado.NumeroIdentificacion;
                Session["NombresIFP"] = afiliado.Nombre.Trim();
                Session["ApellidosIFP"] = afiliado.ApellidoPaterno.Trim() + " " + afiliado.ApellidoMaterno.Trim();

                if (afiliado.EstadoCivil.cod_parametro.Trim().Length > 0)
                {
                    EstadoCivil_RP.SelectedIndex = EstadoCivil_RP.Items.IndexOf(EstadoCivil_RP.Items.FindByValue(afiliado.EstadoCivil.cod_parametro));
                }

                // Direcciones
                NuevaDireccion_RP.Visible = true;

                // Botón Guardar Afiliado
                if (afiliado.Consentimiento)
                    ContenedorGuardar_RP.Visible = true;
                else
                    ContenedorGuardar_RP.Visible = false;

                // Grupo Familiar
                NuevoBeneficiario_RP.Visible = true;

                // Solicitudes
                NuevaSolicitud_IFP.Visible = true;

                ModEnvCorPara.Text = afiliado.CorreoElectronico;

                List<Ciudad> listaCiudades = new List<Ciudad>();
                Ciudad ciudad = servicioCotizador.ObtenerDatosCiudad(afiliado.CiudadEmpresa.Id);
                if (ciudad != null)
                {
                    listaCiudades.Add(ciudad);
                }

                List<Comuna> listaComunas = new List<Comuna>();
                Comuna comuna = servicioCotizador.ObtenerDatosComuna(afiliado.ComunaEmpresa.Id);
                if (comuna != null)
                {
                    listaComunas.Add(comuna);
                }

                TipoDocumento_RP.SelectedIndex = TipoDocumento_RP.Items.IndexOf(TipoDocumento_RP.Items.FindByValue(afiliado.TipoIdentificacion));
                NumeroDocumento_RP.Text = afiliado.NumeroIdentificacion;

                HttpContext.Current.Session["indConsentimiento"] = afiliado.Consentimiento;

                obtenerConsentimiento(afiliado, true);

                //Guardar los datos del afiliado para generarlo como beneficiario en caso de que no exista
                afiliado.ApellidoPaterno = ApellidoPaternoInicial;
                afiliado.ApellidoMaterno = ApellidoMaternoInicial;
                afiliado.Nombre = NombreInicial;
                servicioCotizador.ActualizarAfiliado(afiliado);
            }
            else
            {
                List<string> errores = new List<string>();
                errores.Add("Cliente no pertenece a su cartera de ventas. Verifique.");
                LimpiarFormularios();
                MCMMensaje.Text = Utilitarios.FormatearError(errores);
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                MCMEstado.Value = "1";
            }

        }

        private bool ValidarBusquedaAfiliadoIdentificacion()
        {
            bool esCorrecto = true;
            List<string> errores = new List<string>();

            if (estado_mensaje)
            {
                MCMEstado.Value = "1";
            }
            else
            {
                MCMEstado.Value = "0";
            }

            TipoDocumentoBusqueda_RP.CssClass = "formCombobox";
            NumeroDocumentoBusqueda_RP.CssClass = "formTextbox";

            bool tipoIdentificacion = (TipoDocumentoBusqueda_RP.SelectedIndex > 0) ? true : false;
            bool numIdentificacion = (NumeroDocumentoBusqueda_RP.Text.Trim().Length > 0) ? true : false;

            if (!tipoIdentificacion || !numIdentificacion)
            {
                errores.Add("Debe ingresar ambos criterios para la busqueda.");
                TipoDocumentoBusqueda_RP.CssClass = "formCombobox formComboboxError";
                NumeroDocumentoBusqueda_RP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }

            if (TipoDocumentoBusqueda_RP.SelectedValue == Enums.TipoDocumento.DNI.StringValue() && NumeroDocumentoBusqueda_RP.Text.Trim().Length != 8)
            {
                errores.Add("El <strong>DOCUMENTO NACIONAL DE IDENTIDAD</strong> debe contener 8 caracteres.");
                NumeroDocumentoBusqueda_RP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }
            else if (TipoDocumentoBusqueda_RP.SelectedValue == Enums.TipoDocumento.CE.StringValue() && NumeroDocumentoBusqueda_RP.Text.Trim().Length != 9)
            {
                errores.Add("El <strong>CARNET EXTRANJERIA</strong> debe contener 9 caracteres.");
                NumeroDocumentoBusqueda_RP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }
            else if (TipoDocumentoBusqueda_RP.SelectedValue == Enums.TipoDocumento.RUCJ.StringValue() && NumeroDocumentoBusqueda_RP.Text.Trim().Length != 11)
            {
                errores.Add("El <strong>RUC PERSONA JURIDICA</strong> debe contener 11 caracteres.");
                NumeroDocumentoBusqueda_RP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }
            else if (TipoDocumentoBusqueda_RP.SelectedValue == Enums.TipoDocumento.RUCN.StringValue() && NumeroDocumentoBusqueda_RP.Text.Trim().Length != 11)
            {
                errores.Add("El <strong>RUC PERSONA NATURAL</strong> debe contener 11 caracteres.");
                NumeroDocumentoBusqueda_RP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }
            //else if (TipoDocumentoBusqueda_RP.SelectedValue == Enums.TipoDocumento.CE.StringValue() && NumeroDocumentoBusqueda_RP.Text.Trim().Length != 8)
            //{
            //    errores.Add("El <strong>PASAPORTE</strong> debe contener 8 caracteres.");
            //    NumeroDocumentoBusqueda_RP.CssClass = "formTextbox formTextboxError";
            //    esCorrecto = false;
            //}
            //else if (TipoDocumentoBusqueda_RP.SelectedValue == Enums.TipoDocumento.CE.StringValue() && NumeroDocumentoBusqueda_RP.Text.Trim().Length != 8)
            //{
            //    errores.Add("El <strong>PARTIDA NACIMIENTO</strong> debe contener 8 caracteres.");
            //    NumeroDocumentoBusqueda_RP.CssClass = "formTextbox formTextboxError";
            //    esCorrecto = false;
            //}

            if (!esCorrecto)
            {
                MCMMensaje.Text = Utilitarios.FormatearError(errores);
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Validacion.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                MCMEstado.Value = "1";
                return false;
            }

            return true;
        }

        private void manejoPestanasBusqueda()
        {
            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
            {
                LineaTipoDocumento.Visible = true;
                LineaCUSPP_RP.Visible = false;

                ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda2').show(); $('#PesBusquedaIFP').hide();", true);
                HttpContext.Current.Session["Externo"] = true;
            }
            else
            {

                LineaCUSPP_RP.Visible = true;

                if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteLima.StringValue()
                        || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteProvincia.StringValue())
                {
                    LineaTipoDocumento.Visible = true;
                    ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda1').show(); $('#PesBusquedaInteligo').hide();", true);
                }
                else
                {
                    LineaTipoDocumento.Visible = true;
                    //if (BusAfiNroSolicitud_RP.Text == string.Empty && BusAfiCUSPP_RP.Text == string.Empty && NumeroDocumentoBusqueda_RP.Text == string.Empty && TipoDocumentoBusqueda_RP.SelectedIndex == 0)
                    //{
                    //    ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda1').show();", true);
                    //}
                    //else if (BusAfiNroSolicitud_RP.Text == string.Empty && BusAfiCUSPP_RP.Text == string.Empty)
                    //{
                    //    ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda2').show();", true);
                    //}
                    //else
                    //{
                    //    ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda1').show();", true);
                    //}

                    if (hindPestaniaActiva.Value == "1")
                    {
                        ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda1').show();", true);
                        HttpContext.Current.Session["Externo"] = false;
                    }
                    else if (hindPestaniaActiva.Value == "2")
                    {
                        ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda2').show();", true);
                        //HttpContext.Current.Session["Externo"] = true;
                    }
                    else
                    {
                        if (Session["TipoDocumentoBusqueda"] != null || Session["NroSolicitudNumeroDocumentoBusqueda"] != null)
                        {
                            ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda2').show();", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda1').show();", true);
                        }

                        //if (Session["CUSPP"] != null || Session["NroSolicitud"] != null)
                        //{                                                       
                        //    ClientScript.RegisterStartupScript(GetType(), "-", "$('#PestanhaBusqueda1').show();", true);
                        //}

                        HttpContext.Current.Session["Externo"] = false;
                    }

                }
            }
        }

        private void habilitaCampos()
        {
            bool enabled = true, readOnly = false;

            TipoDocumento_RP.Enabled = enabled;
            NumeroDocumento_RP.Enabled = enabled;
            ApellidoPaterno_RP.Enabled = enabled;
            ApellidoMaterno_RP.Enabled = enabled;
            Nombres_RP.Enabled = enabled;
            FechaNacimiento_RP.Enabled = enabled;
            Sexo_RP.Enabled = enabled;
            CorreoElectronico_RP.Enabled = enabled;
            EstadoCivil_RP.Enabled = enabled;
            Telefono_RP.Enabled = enabled;
            Celular_RP.Enabled = enabled;

            TipoDocumento_RP.CssClass = "formCombobox";
            NumeroDocumento_RP.CssClass = "formTextbox";
            ApellidoPaterno_RP.CssClass = "formTextbox";
            ApellidoMaterno_RP.CssClass = "formTextbox";
            Nombres_RP.CssClass = "formTextbox";
            FechaNacimiento_RP.CssClass = "fecha formTextbox formCalendar";
            Sexo_RP.CssClass = "formCombobox";
            CorreoElectronico_RP.CssClass = "formTextbox";
            EstadoCivil_RP.CssClass = "formCombobox";
            Telefono_RP.CssClass = "formTextbox telefono";
            Celular_RP.CssClass = "formTextbox telefono";

            NumeroDocumento_RP.ReadOnly = readOnly;
            ApellidoPaterno_RP.ReadOnly = readOnly;
            ApellidoMaterno_RP.ReadOnly = readOnly;
            Nombres_RP.ReadOnly = readOnly;
            FechaNacimiento_RP.ReadOnly = readOnly;
            CorreoElectronico_RP.ReadOnly = readOnly;
            Telefono_RP.ReadOnly = readOnly;
            Celular_RP.ReadOnly = readOnly;
        }
        //FIN.YRV

        //S38

        [WebMethod]
        public static string CargarTablaBeneficiarios(string cuspp, List<GrupoFamiliar> beneficiarios, string conyuge)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio Cotizador.CargarTablaBeneficiarios WebMethod");

                    var pagina = new Page();

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    //if (beneficiarios == null)
                    //{
                    //    var control = (TablaBeneficiariosRentaPrivada)pagina.LoadControl("~/Controles/TablaBeneficiariosRentaPrivada.ascx");
                    //    log.Debug("Inicio Cotizador.servicioCotizador.ListarGrupoFamiliar WebMethod");
                    //    List<GrupoFamiliar> grupoFamiliar = servicioCotizador.ListarGrupoFamiliar(cuspp);
                    //    log.Debug("Fin Cotizador.servicioCotizador.ListarGrupoFamiliar WebMethod");

                    //    grupoFamiliar.ForEach(g => g.Seleccionado = true);
                    //    control.Beneficiarios = grupoFamiliar;
                    //    control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                    //    HttpContext.Current.Session["Beneficiarios"] = control.Beneficiarios;

                    //    //<INIGTI_753>
                    //    control.Conyuge = (conyuge == "TRUE") ? true : false;
                    //    //<FINGTI_753>

                    //    pagina.Controls.Add(control);
                    //}
                    //else
                    //{
                    var control = (TablaRviBenefiRentaPrivada)pagina.LoadControl("~/Controles/TablaRviBenefiRentaPrivada.ascx");
                    control.Beneficiarios = beneficiarios;
                    control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];

                    HttpContext.Current.Session["numCUSPP"] = cuspp;

                    if (HttpContext.Current.Session["HEstadoEsPoliza"].ToString() == "6")
                    {
                        control.PermisoModificar = true;
                    }
                    else
                    {
                        control.PermisoModificar = false;
                    }

                    pagina.Controls.Add(control);
                    //}

                    string html = "";
                    using (var sw = new StringWriter())
                    {
                        HttpContext.Current.Server.Execute(pagina, sw, false);
                        html = sw.ToString();
                    }

                    log.Debug("Fin Cotizador.CargarTablaBeneficiarios WebMethod");

                    return html;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        static bool validaFecha(DateTime fecha)
        {
            int dia = fecha.Day;
            int mes = fecha.Month;
            int anio = fecha.Year;

            if ((anio < 1900) || (anio > DateTime.Now.Year) || (mes == 0) || (mes > 12) || (dia == 0) || (dia > 31) || (anio + mes + dia) > (DateTime.Now.Year + (DateTime.Now.Month) + DateTime.Now.Day))
            {
                return false;
            }
            return true;
        }

        //<INI.GTI_26697>
        private bool consentimiento(int idConfiguracion, string tipoDocumento, string numeroDocumento, string usuario, ref DateTime fechaConsentimiento, ref string consentimientoToken, ref string telefonoCliente, ref string celularCliente, ref string correoCliente, ref string tratamientoConsentimiento)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    log.Info("Accediendo a las Key necesarias");
                    string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                    string urlConsultaConsentimientoCliente = ConfigurationManager.AppSettings["url_consulta_consentimiento_cliente"].ToString();
                    string urlConsultaConsentimientoUniversal = ConfigurationManager.AppSettings["url_consulta_consentimiento_universal"].ToString();
                    string token_generado = string.Empty;
                    int idConsentimiento = 0;
                    string indicadorConsentimiento = string.Empty;

                    /*Obtener token*/
                    log.Info("Consumiendo servicio token: " + urlToken);
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    log.Info("Pasando el json al servicio - " + usuario);
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

                    /*Obtener indicador cliente permitido*/
                    if (token_generado != "")
                    {
                        if (tipoDocumento == "" || tipoDocumento == null) tipoDocumento = "D";

                        ConsentimientoCliente consentimientoCliente = new ConsentimientoCliente()
                        {
                            id_configuracion = idConfiguracion,
                            cod_tipo_identificacion = tipoDocumento,
                            gls_num_identificacion = numeroDocumento,
                            aud_usr_ingreso = usuario
                        };

                        var jsonConsentimientoCliente = JsonConvert.SerializeObject(consentimientoCliente);

                        log.Info("Json consentimiento: " + jsonConsentimientoCliente);

                        urlConsultaConsentimientoCliente = string.Format(urlConsultaConsentimientoCliente, idConfiguracion.ToString(), tipoDocumento, numeroDocumento, usuario);

                        log.Info("Consumiendo servicio consulta consentimiento: " + urlConsultaConsentimientoCliente);
                        httpWebRequest = (HttpWebRequest)WebRequest.Create(urlConsultaConsentimientoCliente);
                        //httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "GET";
                        httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                        log.Info("Leyendo el servicio");
                        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            var jsonResult = streamReader.ReadToEnd();

                            if (jsonResult.Length > 0)
                            {
                                JObject jObject = JObject.Parse(jsonResult);

                                if (jObject["id_consentimiento_asesoria"] != null && jObject["id_consentimiento_asesoria"].ToString().Length > 0)
                                {
                                    idConsentimiento = (int)jObject["id_consentimiento_asesoria"];
                                    HidConsentimientoAsesoria.Value = idConsentimiento.ToString();
                                }

                                if (jObject["gls_token"] != null && jObject["gls_token"].ToString().Length > 0)
                                {
                                    consentimientoToken = jObject["gls_token"].ToString();
                                }

                                if (jObject["id_configuracion"] != null && jObject["id_configuracion"].ToString().Length > 0)
                                {
                                    HidConfiguracion.Value = jObject["id_configuracion"].ToString();
                                }

                                if (jObject["gls_telefono"] != null && jObject["gls_telefono"].ToString().Length > 0)
                                {
                                    telefonoCliente = jObject["gls_telefono"].ToString();
                                }

                                if (jObject["gls_celular"] != null && jObject["gls_celular"].ToString().Length > 0)
                                {
                                    celularCliente = jObject["gls_celular"].ToString();
                                }

                                if (jObject["gls_mail"] != null && jObject["gls_mail"].ToString().Length > 0)
                                {
                                    correoCliente = jObject["gls_mail"].ToString();
                                }

                                //if (jObject["ind_consentimiento"] != null && jObject["ind_consentimiento"].ToString().Length > 0)
                                //{
                                indicadorConsentimiento = jObject["ind_consentimiento"].ToString();
                                HindConsentimiento.Value = indicadorConsentimiento;
                                //}

                                if (jObject["fec_ultimo_consentimiento"] != null && jObject["fec_ultimo_consentimiento"].ToString().Length > 0)
                                {
                                    fechaConsentimiento = (DateTime)jObject["fec_ultimo_consentimiento"];
                                }
                            }

                        }

                        if (indicadorConsentimiento == "S")
                        {
                            tratamientoConsentimiento = "de Renta Particular";
                            return true;
                        }
                        else
                        {
                            string idConfiguracionUniversal = Enums.TratamientoConsentimiento.InterseguroUniversal.StringValue();
                            string url = string.Format(urlConsultaConsentimientoUniversal, idConfiguracionUniversal, tipoDocumento, numeroDocumento, usuario);

                            log.Info("Consumiendo servicio consulta consentimiento universal: " + url);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            //httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "GET";
                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                            log.Info("Leyendo el servicio");
                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                var jsonResult = streamReader.ReadToEnd();

                                if (jsonResult.Length > 0)
                                {
                                    JObject jObject = JObject.Parse(jsonResult);

                                    if (jObject["id_consentimiento_asesoria"] != null && jObject["id_consentimiento_asesoria"].ToString().Length > 0)
                                    {
                                        idConsentimiento = (int)jObject["id_consentimiento_asesoria"];
                                        HidConsentimientoAsesoria.Value = idConsentimiento.ToString();
                                    }

                                    if (jObject["gls_token"] != null && jObject["gls_token"].ToString().Length > 0)
                                    {
                                        consentimientoToken = jObject["gls_token"].ToString();
                                    }

                                    if (jObject["id_configuracion"] != null && jObject["id_configuracion"].ToString().Length > 0)
                                    {
                                        HidConfiguracion.Value = jObject["id_configuracion"].ToString();
                                    }

                                    if (jObject["gls_telefono"] != null && jObject["gls_telefono"].ToString().Length > 0)
                                    {
                                        telefonoCliente = jObject["gls_telefono"].ToString();
                                    }

                                    if (jObject["gls_celular"] != null && jObject["gls_celular"].ToString().Length > 0)
                                    {
                                        celularCliente = jObject["gls_celular"].ToString();
                                    }

                                    if (jObject["gls_mail"] != null && jObject["gls_mail"].ToString().Length > 0)
                                    {
                                        correoCliente = jObject["gls_mail"].ToString();
                                    }

                                    //if (jObject["ind_consentimiento"] != null && jObject["ind_consentimiento"].ToString().Length > 0)
                                    //{
                                    indicadorConsentimiento = jObject["ind_consentimiento"].ToString();
                                    HindConsentimiento.Value = indicadorConsentimiento;
                                    //}

                                    if (jObject["fec_ultimo_consentimiento"] != null && jObject["fec_ultimo_consentimiento"].ToString().Length > 0)
                                    {
                                        fechaConsentimiento = (DateTime)jObject["fec_ultimo_consentimiento"];
                                    }
                                }

                            }

                            if (indicadorConsentimiento == "S")
                            {
                                tratamientoConsentimiento = "Universal de Interseguro";
                                return true;
                            }
                            else
                            {
                                string idConfiguracionUniversalIntercorp = Enums.TratamientoConsentimiento.IntercorpPublicidad.StringValue();
                                url = string.Format(urlConsultaConsentimientoUniversal, idConfiguracionUniversalIntercorp, tipoDocumento, numeroDocumento, usuario);

                                log.Info("Consumiendo servicio consulta consentimiento universal intercorp: " + url);
                                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                                //httpWebRequest.ContentType = "application/json";
                                httpWebRequest.Method = "GET";
                                httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                                log.Info("Leyendo el servicio");
                                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                                {
                                    var jsonResult = streamReader.ReadToEnd();

                                    if (jsonResult.Length > 0)
                                    {
                                        JObject jObject = JObject.Parse(jsonResult);

                                        if (jObject["id_consentimiento_asesoria"] != null && jObject["id_consentimiento_asesoria"].ToString().Length > 0)
                                        {
                                            idConsentimiento = (int)jObject["id_consentimiento_asesoria"];
                                            HidConsentimientoAsesoria.Value = idConsentimiento.ToString();
                                        }

                                        if (jObject["gls_token"] != null && jObject["gls_token"].ToString().Length > 0)
                                        {
                                            consentimientoToken = jObject["gls_token"].ToString();
                                        }

                                        if (jObject["id_configuracion"] != null && jObject["id_configuracion"].ToString().Length > 0)
                                        {
                                            HidConfiguracion.Value = jObject["id_configuracion"].ToString();
                                        }

                                        if (jObject["gls_telefono"] != null && jObject["gls_telefono"].ToString().Length > 0)
                                        {
                                            telefonoCliente = jObject["gls_telefono"].ToString();
                                        }

                                        if (jObject["gls_celular"] != null && jObject["gls_celular"].ToString().Length > 0)
                                        {
                                            celularCliente = jObject["gls_celular"].ToString();
                                        }

                                        if (jObject["gls_mail"] != null && jObject["gls_mail"].ToString().Length > 0)
                                        {
                                            correoCliente = jObject["gls_mail"].ToString();
                                        }

                                        //if (jObject["ind_consentimiento"] != null && jObject["ind_consentimiento"].ToString().Length > 0)
                                        //{
                                        indicadorConsentimiento = jObject["ind_consentimiento"].ToString();
                                        HindConsentimiento.Value = indicadorConsentimiento;
                                        //}

                                        if (jObject["fec_ultimo_consentimiento"] != null && jObject["fec_ultimo_consentimiento"].ToString().Length > 0)
                                        {
                                            fechaConsentimiento = (DateTime)jObject["fec_ultimo_consentimiento"];
                                        }
                                    }

                                }

                                if (indicadorConsentimiento == "S")
                                {
                                    tratamientoConsentimiento = "Universal de Intercorp";
                                    return true;
                                }

                                log.Info("Problemas al obtener el idConsentimiento");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        log.Info("Problemas al generar el token");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error: " + Utilitarios.FormatearError(new List<string> { ex.Message }));
                    return false;
                }
            }
        }

        private Respuesta validacionConsentimientoAsesoria(string nombre, string apellidoPaterno, string apellidoMaterno, string tipoIdentificacion, string numeroIdentificacion, string correoElectronico, string telefono, string celular, string CIC, string rangoInversion, string centroLaboral, Direccion direccionPrincipal)
        {
            Respuesta respuesta = new Respuesta();
            List<string> errores = new List<string>();
            bool validacion = false;
            bool nombres = false;
            bool documento = false;
            bool correo = false;
            bool vtelefono = false;
            bool vcelular = false;
            //bool vAFP = false;
            bool vCIC = false;
            //bool vrangoInversion = false;
            bool vcentroLaboral = false;
            bool direccion = false;

            //validación
            if (nombre == null
                    || apellidoPaterno == null
                    || apellidoMaterno == null)
            {
                errores.Add("Ingrese los campos <strong>Nombres y apellidos completos</strong>. Datos obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                nombres = true;
            }
            else
            {
                if (nombre.Length == 0
                    || apellidoPaterno.Length == 0
                    || apellidoMaterno.Length == 0)
                {
                    errores.Add("Ingrese los campos <strong>Nombres y apellidos completos</strong>. Datos obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                    nombres = true;
                }
            }

            if (tipoIdentificacion == null
                    || numeroIdentificacion == null)
            {
                errores.Add("Ingrese los campos <strong>Tipo y número de documento</strong>. Datos Obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                documento = true;
            }
            else
            {
                if (tipoIdentificacion.Length == 0
                    || numeroIdentificacion.Length == 0)
                {
                    errores.Add("Ingrese los campos <strong>Tipo y número de documento</strong>. Datos Obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                    documento = true;
                }
            }

            if (correoElectronico.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio, actualizar la información en el <strong>vtiger</strong>.");
                correo = true;
            }

            if (direccionPrincipal == null)
            {
                errores.Add("Por favor ingrese la dirección del cliente en la sección <strong>Direcciones</strong>. Dato Obligatorio.");
                direccion = true;
            }
            else
            {
                if (string.IsNullOrEmpty(direccionPrincipal.Glosa) || string.IsNullOrEmpty(direccionPrincipal.EspacioUrbano))
                {
                    direccion = true;
                }

                if (direccionPrincipal.TipoVia == null)
                {
                    direccion = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(direccionPrincipal.TipoVia.Id) || direccionPrincipal.TipoVia.Id == "0")
                    {
                        direccion = true;
                    }
                }

                if (direccionPrincipal.Departamento == null)
                {
                    direccion = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(direccionPrincipal.Departamento.Id) || direccionPrincipal.Departamento.Id == "0")
                    {
                        direccion = true;
                    }
                }

                if (direccionPrincipal.Comuna == null)
                {
                    direccion = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(direccionPrincipal.Comuna.Id) || direccionPrincipal.Comuna.Id == "0")
                    {
                        direccion = true;
                    }
                }

                if (direccionPrincipal.Ciudad == null)
                {
                    direccion = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(direccionPrincipal.Ciudad.Id) || direccionPrincipal.Ciudad.Id == "0")
                    {
                        direccion = true;
                    }
                }

                if (direccion)
                {
                    errores.Add("Por favor complete la dirección del cliente en la sección <strong>Direcciones</strong>. Dato Obligatorio.");
                }

            }

            if (telefono == null)
            {
                errores.Add("Ingrese el campo <strong>Teléfono</strong>. Dato Obligatorio, actualizar la información en el <strong>vtiger</strong>.");
                vtelefono = true;
            }

            if (celular == null)
            {
                errores.Add("Ingrese el campo <strong>Celular</strong>. Dato Obligatorio, actualizar la información en el <strong>vtiger</strong>.");
                vcelular = true;
            }
            else
            {
                if (celular.Length != 9)
                {
                    errores.Add("Ingrese el campo <strong>Celular</strong>. Dato Obligatorio, actualizar la información en el <strong>vtiger</strong>.");
                    vcelular = true;
                }
            }

            //if (AFP == null)
            //{
            //    errores.Add("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
            //    vAFP = true;
            //}
            //else
            //{
            //    if (AFP == "0")
            //    {
            //        errores.Add("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
            //        vAFP = true;
            //    }

            //    if (AFP == "-")
            //    {
            //        errores.Add("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
            //        vAFP = true;
            //    }
            //}

            if (CIC == null)
            {
                errores.Add("Ingrese el campo <strong>Fondo Aproximado</strong>. Dato Obligatorio.");
                vCIC = true;
            }
            else
            {
                if (CIC.Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Fondo Aproximado</strong>. Dato Obligatorio.");
                    vCIC = true;
                }
            }

            //if (rangoInversion == null)
            //{
            //    errores.Add("Ingrese el campo <strong>Rango de Inversión</strong>. Dato Obligatorio.");
            //    vrangoInversion = true;
            //}
            //else
            //{
            //    if (rangoInversion.Length == 0)
            //    {
            //        errores.Add("Ingrese el campo <strong>Rango de Inversión</strong>. Dato Obligatorio.");
            //        vrangoInversion = true;
            //    }
            //}

            if (centroLaboral == null)
            {
                errores.Add("Ingrese el campo <strong>Centro Laboral</strong>. Dato Obligatorio.");
                vcentroLaboral = true;
            }
            else
            {
                if (centroLaboral.Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Centro Laboral</strong>. Dato Obligatorio.");
                    vcentroLaboral = true;
                }
            }

            respuesta.Estado = Constante.COD_OK;

            if (nombres || documento || correo || direccion || vtelefono || vcelular || vCIC || vcentroLaboral)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = "<div style=\"margin: 5px 0\">No se puede enviar el consentimiento y protección de datos personales al cliente porque están faltando los siguientes datos:</div>" + Utilitarios.FormatearError(errores) + "<div>Actualice la información y recargue la página.</div>";
            }

            return respuesta;
        }

        [WebMethod]
        public static Respuesta EnviarConsentimientoAsesoria(ParametrosEnvioCDA parametros)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                    log.Info("Accediendo a las Key necesarias: token, correo");
                    string urlConsentimientoCliente = ConfigurationManager.AppSettings["url_consentimiento_cliente"].ToString();
                    string urlConsultaConsentimiento = ConfigurationManager.AppSettings["url_consulta_consentimiento_cliente"];
                    string urlAppConsentimiento = ConfigurationManager.AppSettings["url_app_consentimiento"].ToString();
                    string flagCorreoCliente = ConfigurationManager.AppSettings["flag_correo_cliente"].ToString();
                    string destinatarioConsentimiento = ConfigurationManager.AppSettings["destinatario_consentimiento"].ToString();

                    if (destinatarioConsentimiento != "N")
                    {
                        parametros.correo = destinatarioConsentimiento;
                    }

                    if (string.IsNullOrEmpty(parametros.tipoDocumento)) parametros.tipoDocumento = "D";

                    string usuario = HttpContext.Current.Session["Usuario"].ToString();

                    Agente agente = ObtenerDatosAgente(parametros, usuario);

                    // Validar si es que el consentimiento ya existe
                    string url = string.Format(urlConsultaConsentimiento, Enums.TratamientoConsentimiento.RP.StringValue(), parametros.tipoDocumento, parametros.numeroDocumento, usuario);
                    log.Debug("Consumiendo endpoint: " + urlConsentimientoCliente);
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "GET";

                    var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    ConsentimientoCliente consentimiento = null;
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        string responseBody = streamReader.ReadToEnd();
                        consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                    }

                    log.Debug("Obteniendo el token: " + parametros.token);
                    if (consentimiento != null)
                    {
                        if (consentimiento.ind_consentimiento != "S")
                        {
                            // Actualizar el consentimiento si es que aún no ha sido firmado

                            consentimiento.ind_consentimiento = null;
                            consentimiento.gls_token = consentimiento.gls_token;
                            consentimiento.cod_tipo_identificacion = parametros.tipoDocumento;
                            consentimiento.gls_num_identificacion = parametros.numeroDocumento;
                            consentimiento.id_configuracion = Convert.ToInt32(Enums.ConfiguracionConsentimiento.RP.StringValue());
                            consentimiento.gls_nombres = parametros.nombres;
                            consentimiento.gls_apellido_paterno = parametros.apellidoPaterno;
                            consentimiento.gls_apellido_materno = parametros.apellidoMaterno;
                            consentimiento.gls_sexo = parametros.sexo;
                            consentimiento.fec_nacimiento = Convert.ToDateTime(parametros.fechaNacimiento, new CultureInfo("es-PE"));
                            consentimiento.gls_mail = parametros.correo;
                            consentimiento.gls_telefono = parametros.telefono;
                            consentimiento.gls_celular = parametros.celular;
                            consentimiento.gls_nombres_agente = ti.ToTitleCase(agente.Nombre.ToString().Trim().ToLower());
                            consentimiento.gls_mail_agente = agente.CorreoElectronico;
                            consentimiento.aud_usr_modificacion = usuario;
                            consentimiento.num_agente = Convert.ToInt32(agente.Id);

                            log.Debug("Consumiendo endpoint: PUT " + urlConsentimientoCliente);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(urlConsentimientoCliente);

                            var context = new HttpContextWrapper(HttpContext.Current);
                            HttpRequestBase request = context.Request;
                            httpWebRequest.UserAgent = request.UserAgent;
                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "PUT";

                            string jsonConsentimiento = JsonConvert.SerializeObject(consentimiento);
                            log.Debug(string.Format("Request Body [{0}]", jsonConsentimiento));
                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            {
                                streamWriter.Write(jsonConsentimiento);
                                streamWriter.Flush();
                                streamWriter.Close();
                            }

                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                var jsonResult = streamReader.ReadToEnd();
                                consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(jsonResult);
                            }

                            urlAppConsentimiento = string.Format(urlAppConsentimiento, consentimiento.gls_token);

                            log.Debug("Enviando el correo SME");
                            if (flagCorreoCliente == "S")
                            {
                                if (parametros.canalComunicacion == "EMAIL")
                                {
                                    int codProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEConsentimientoRP"]);

                                    EnviarSolicitudCdaEmail(parametros, urlAppConsentimiento, codProcesoSme, agente);
                                }
                                else if (parametros.canalComunicacion == "SMS")
                                {
                                    EnviarSolicitudCdaSMS(parametros, urlAppConsentimiento, agente);
                                }
                            }

                            respuesta.Estado = Constante.COD_OK;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Consentimiento y protección de datos personales enviado correctamente." });
                        }
                        else
                        {
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = "<div style=\"margin: 5px 0\">Este cliente ya ha firmado su Consentimiento de Asesoría. Por favor recargue la página para obtener la información respectiva.</div>";
                        }
                    }
                    else
                    {
                        // Crear consentimiento

                        consentimiento = new ConsentimientoCliente();
                        consentimiento.cod_tipo_identificacion = parametros.tipoDocumento;
                        consentimiento.gls_num_identificacion = parametros.numeroDocumento;
                        consentimiento.id_configuracion = Convert.ToInt32(Enums.ConfiguracionConsentimiento.RP.StringValue());
                        consentimiento.gls_nombres = parametros.nombres;
                        consentimiento.gls_apellido_paterno = parametros.apellidoPaterno;
                        consentimiento.gls_apellido_materno = parametros.apellidoMaterno;
                        consentimiento.gls_sexo = parametros.sexo;
                        consentimiento.fec_nacimiento = Convert.ToDateTime(parametros.fechaNacimiento, new CultureInfo("es-PE"));
                        consentimiento.gls_mail = parametros.correo;
                        consentimiento.gls_telefono = parametros.telefono;
                        consentimiento.gls_celular = parametros.celular;
                        consentimiento.gls_nombres_agente = ti.ToTitleCase(agente.Nombre.ToString().Trim().ToLower());
                        consentimiento.gls_mail_agente = agente.CorreoElectronico;
                        consentimiento.num_agente = Convert.ToInt32(agente.Id);
                        consentimiento.aud_usr_ingreso = usuario;

                        log.Debug("Consumiendo endpoint: POST " + urlConsentimientoCliente);
                        httpWebRequest = (HttpWebRequest)WebRequest.Create(urlConsentimientoCliente);
                        var context = new HttpContextWrapper(HttpContext.Current);
                        HttpRequestBase request = context.Request;
                        httpWebRequest.UserAgent = request.UserAgent;
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";

                        string requestBody = JsonConvert.SerializeObject(consentimiento);

                        log.Debug(string.Format("Request Body [{0}]", requestBody));
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(requestBody);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            string responseBody = streamReader.ReadToEnd();
                            consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                        }

                        urlAppConsentimiento = string.Format(urlAppConsentimiento, consentimiento.gls_token);

                        log.Debug("Enviando el correo SME");
                        if (flagCorreoCliente == "S")
                        {
                            if (parametros.canalComunicacion == "EMAIL")
                            {
                                int codProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEConsentimientoRP"]);

                                EnviarSolicitudCdaEmail(parametros, urlAppConsentimiento, codProcesoSme, agente);
                            }
                            else if (parametros.canalComunicacion == "SMS")
                            {
                                EnviarSolicitudCdaSMS(parametros, urlAppConsentimiento, agente);
                            }
                        }

                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                        respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Consentimiento y protección de datos personales enviado correctamente." });
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error: " + Utilitarios.FormatearError(new List<string> { ex.Message }));
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }

        private static Agente ObtenerDatosAgente(ParametrosEnvioCDA parametros, string usuario)
        {
            string urlAsesorInteligo = ConfigurationManager.AppSettings["url_asesor_inteligo"].ToString();
            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();

            string token_generado = string.Empty;

            /* OBTENER DATOS DEL AGENTE */
            List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];

            Agente agente = listaAgentes.Find(a => a.Id == HttpContext.Current.Session["Vendedor"].ToString());

            if (agente == null && Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]))
            {
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado("", parametros.cuspp, "", "", "");
                agente = servicioCotizador.ObtenerUltimoAgentePorCartera(afiliado.Agente.IdCartera, usuario);
            }

            log.Debug("Obteniendo el correo del agente");

            //System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;

            log.Info("Consumiendo servicio token: " + urlToken);
            /*Obtener token*/
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

            /* OBTENER CORREO DEL AGENTE */
            if (HttpContext.Current.Session["RolAzman"].ToString() == Enums.RolAzman.AgenteExterno.StringValue())
            {
                try
                {
                    urlAsesorInteligo = string.Format(urlAsesorInteligo, usuario);
                    log.Info("Consumiendo servicio consulta asesor inteligo: " + urlAsesorInteligo);
                    HttpWebRequest httpWebRequestRentas = (HttpWebRequest)WebRequest.Create(urlAsesorInteligo);
                    httpWebRequestRentas.Method = "GET";
                    httpWebRequestRentas.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                    log.Info("Leyendo el servicio");

                    HttpWebResponse httpWebResponseRentas = (HttpWebResponse)httpWebRequestRentas.GetResponse();
                    using (var streamReader = new StreamReader(httpWebResponseRentas.GetResponseStream()))
                    {
                        var jsonResult = streamReader.ReadToEnd();

                        if (jsonResult.Length > 0)
                        {
                            JObject jObject = JObject.Parse(jsonResult);
                            if (agente == null)
                            {
                                agente = new Agente() { Nombre = (string)jObject["gls_nombre_generico_asesor"], Id = (string)jObject["gls_codigo_asesor"] };
                            }
                            agente.CorreoElectronico = (string)jObject["gls_mail_asesor"];
                        }
                    }
                }
                catch (WebException e)
                {
                    if (e.Status == WebExceptionStatus.ProtocolError)
                    {
                        throw new Exception("Error al obtener el asesor inteligo");
                    }
                }
            }
            else
            {
                try
                {
                    AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient servicioAzman = new AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient("epAzman");

                    var datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                            ConfigurationManager.AppSettings["AplicacionAZMAN"],
                            ConfigurationManager.AppSettings["DominioRed"],
                            agente.Usuario);

                    if (datosUsuario != null && !string.IsNullOrEmpty(datosUsuario.Correo))
                    {
                        agente.CorreoElectronico = datosUsuario.Correo;
                    }
                }
                catch (Exception)
                {
                    log.Warn("No se ha enviado mail al agente " + agente.Usuario + " porque no se ha podido obtener su email");
                }
            }

            return agente;
        }

        private static void EnviarSolicitudCdaEmail(ParametrosEnvioCDA parametros, string urlAppConsentimiento, int codProcesoSme, Agente agente)
        {
            log.Debug("INICIO EnviarSolicitudCdaEmail");

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            DocumentoSME documentoSME = new DocumentoSME
            {
                Email = parametros.correo,
                NumeroPoliza = "N/A",
                NumeroDocumento = parametros.numeroDocumento,
                Destinatario = $"{parametros.nombres} {parametros.apellidoPaterno} {parametros.apellidoMaterno}".Trim(),
                ProcesoSme = codProcesoSme,
                CamposDinamicos = new
                {
                    Id_nombres = ti.ToTitleCase(parametros.nombres.ToLower().Trim()),
                    Id_link = urlAppConsentimiento,
                    Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                }
            };

            Respuesta respuesta = Utilitario.EnviarDocumentoSME(documentoSME);
            if (respuesta.Estado != Constante.COD_OK)
            {
                log.Error(string.Format("Error al enviar el correo del cliente [{0}]", parametros.correo));
                throw new Exception("Error al enviar el correo del cliente.");
            }

            dynamic respDocumentoSME = JsonConvert.DeserializeObject(respuesta.Mensaje);
            long codigoSME = respDocumentoSME.codigoSME;

            int idProcesoEnvio = (int)Enums.ProcesoEnvio.ConsentimientoRP;

            /* Registrar seguimiento de envío */
            RegistrarEnvioSeguimiento(parametros.tipoDocumento, parametros.numeroDocumento, idProcesoEnvio, codigoSME, parametros.correo, agente.Id);
        }

        private static void EnviarSolicitudCdaSMS(ParametrosEnvioCDA parametros, string urlAppConsentimiento, Agente agente)
        {
            log.Debug("INICIO EnviarSolicitudCdaSMS");

            int codProveedorEnvioSms = (int)Enums.ProveedorEnvioSms.Intico;

            SeguimientoEnvioSMS ultimoEnvio = ObtenerUltimoEnvioSMS(parametros.tipoDocumento, parametros.numeroDocumento);

            if (ultimoEnvio != null)
            {
                if (ultimoEnvio.cod_proveedor_envio_sms == (int)Enums.ProveedorEnvioSms.Intico)
                {
                    codProveedorEnvioSms = (int)Enums.ProveedorEnvioSms.Infobip;
                }
                else if (ultimoEnvio.cod_proveedor_envio_sms == (int)Enums.ProveedorEnvioSms.Infobip)
                {
                    codProveedorEnvioSms = (int)Enums.ProveedorEnvioSms.Intico;
                }
            }

            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            string mensaje = $"Estimad@ {ti.ToTitleCase(parametros.nombres.ToLower().Trim())},\n";
            mensaje += "Por favor ingrese al link para otorgarnos su consentimiento de asesoria:\n";
            mensaje += urlAppConsentimiento;

            EnvioSMS envioSMS = new EnvioSMS
            {
                codProveedor = codProveedorEnvioSms,
                numCelular = parametros.celular,
                mensaje = mensaje
            };

            Respuesta resultadoEnvio = Utilitario.EnviarSMS(envioSMS);

            int idProcesoEnvio = (int)Enums.ProcesoEnvio.ConsentimientoRP;

            /* Registrar seguimiento de envío */
            RegistrarSeguimientoEnvioSMS(parametros, agente.Id, idProcesoEnvio, codProveedorEnvioSms, resultadoEnvio);

            if (resultadoEnvio.Estado != Constante.COD_OK)
            {
                log.Error($"Error al enviar el SMS al cliente | codProveedor: {codProveedorEnvioSms} - celular: [{parametros.celular}]");
                throw new Exception("Error al enviar el SMS al cliente, por favor vuelva a intentarlo.");
            }
        }

        private static SeguimientoEnvioSMS ObtenerUltimoEnvioSMS(string codTipoIdentificacion, string glsNumIdentificacion)
        {
            log.Debug("INICIO ObtenerUltimoEnvioSMS");

            SeguimientoEnvioSMS resultado = null;

            string urlConsultarSeguimientoEnvio = ConfigurationManager.AppSettings["url_consultar_seguimiento_envio_sms"];

            urlConsultarSeguimientoEnvio = string.Format(urlConsultarSeguimientoEnvio, codTipoIdentificacion, glsNumIdentificacion);

            log.Debug($"Consumiendo endpoint urlConsultarSeguimientoEnvio | [GET] {urlConsultarSeguimientoEnvio}");

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string response = client.DownloadString(urlConsultarSeguimientoEnvio);

                List<SeguimientoEnvioSMS> envios = JsonConvert.DeserializeObject<List<SeguimientoEnvioSMS>>(response);

                resultado = envios.OrderByDescending(e => e.codigo).FirstOrDefault();
            }

            return resultado;
        }

        private static void RegistrarEnvioSeguimiento(string tipoDocumento, string numeroDocumento, int idProcesoEnvio, long idSME, string glsMail, string codAgente)
        {
            log.Debug("INICIO RegistrarEnvioSeguimiento");

            string urlEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];

            EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
            {
                gls_identificador = $"{tipoDocumento}|{numeroDocumento}",
                id_proceso_envio = idProcesoEnvio,
                id_sme = idSME,
                cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                gls_mail = glsMail,
                fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                cod_agente = codAgente,
                aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
            };

            var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
            string jsonEnvioSeguimiento = JsonSerializar.Serialize(envioSeguimiento);

            log.Debug($"Consumiendo endpoint urlEnvioSeguimiento | [POST] {urlEnvioSeguimiento}");
            log.Debug($"jsonEnvioSeguimiento | [BODY] {jsonEnvioSeguimiento}");

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string resultado = client.UploadString(new Uri(urlEnvioSeguimiento), "POST", jsonEnvioSeguimiento);

                log.Debug($"Resultado de petición | [RESPONSE] {resultado}");
            }
        }

        private static void RegistrarSeguimientoEnvioSMS(ParametrosEnvioCDA parametros, string codAgente, int idProcesoEnvio, int codProveedorEnvioSms, Respuesta resultadoEnvio)
        {
            log.Debug("INICIO RegistrarSeguimientoEnvioSMS");

            string urlSeguimientoEnvioSms = ConfigurationManager.AppSettings["url_seguimiento_envio_sms"];

            bool indEnviado = false;
            string idEnvio = null;

            if (resultadoEnvio.Estado == Constante.COD_OK)
            {
                indEnviado = true;
                dynamic response = JsonConvert.DeserializeObject(resultadoEnvio.Mensaje);
                idEnvio = response.idMensaje;
            }

            SeguimientoEnvioSMS seguimientoEnvio = new SeguimientoEnvioSMS
            {
                cod_tipo_identificacion = parametros.tipoDocumento,
                gls_num_identificacion = parametros.numeroDocumento,
                gls_celular = parametros.celular,
                cod_agente = codAgente,
                id_proceso_envio = idProcesoEnvio,
                fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                cod_proveedor_envio_sms = codProveedorEnvioSms,
                ind_enviado = indEnviado,
                id_envio = idEnvio,
                aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
            };

            string jsonSeguimientoEnvio = JsonConvert.SerializeObject(seguimientoEnvio);

            log.Debug($"Consumiendo endpoint urlSeguimientoEnvioSms | [POST] {urlSeguimientoEnvioSms}");
            log.Debug($"jsonSeguimientoEnvio | [BODY] {jsonSeguimientoEnvio}");

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string resultado = client.UploadString(new Uri(urlSeguimientoEnvioSms), "POST", jsonSeguimientoEnvio);

                log.Debug($"Resultado de petición | [RESPONSE] {resultado}");
            }
        }

        private static Respuesta EnviarNotificacion(string rutaServicio, Notificacion notificacion)
        {
            //Envio de manera Asincrono
            Respuesta respuesta = new Respuesta();
            try
            {
                List<string> errores = new List<string>();
                if (notificacion == null)
                {
                    errores.Add("Envíe una notificación completa. Dato Obligatorio");
                }
                else
                {
                    if (notificacion.p_remitente == null || notificacion.p_remitente == "")
                    {
                        errores.Add("Ingrese Remitente del Correo. Dato Obligatorio.");
                    }
                }

                if (rutaServicio == "")
                {
                    errores.Add("Ingrese ruta del servicio de correo. Dato Obligatorio");
                }

                if (errores.Count > 0)
                {
                    respuesta.Mensaje = Utilitarios.FormatearErrorTexto(errores);
                    return respuesta;
                }

                if (notificacion.p_destinatario != null)
                {
                    notificacion.p_destinatario = notificacion.p_destinatario.Trim();
                }

                log.Info("Ejecutando el servicio del correo");
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string jsonString = JsonSerializar.Serialize(notificacion);
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    respuesta.Mensaje = client.UploadString(new Uri(rutaServicio), "POST", jsonString);
                    respuesta.Estado = Constante.COD_OK;
                }

            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
            }
            return respuesta;
        }

        private static Respuesta EnviarNotificacionSME(string rutaServicio, NotificacionSME notificacionSME)
        {
            //Envio de manera Asincrono
            Respuesta respuesta = new Respuesta();
            try
            {
                List<string> errores = new List<string>();
                if (notificacionSME == null)
                {
                    errores.Add("Envíe una notificación completa. Dato Obligatorio");
                }
                else
                {
                    if (notificacionSME.De == null || notificacionSME.De == "")
                    {
                        errores.Add("Ingrese Remitente del Correo. Dato Obligatorio.");
                    }
                }

                if (rutaServicio == "")
                {
                    errores.Add("Ingrese ruta del servicio de correo. Dato Obligatorio");
                }

                if (errores.Count > 0)
                {
                    respuesta.Mensaje = Utilitarios.FormatearErrorTexto(errores);
                    return respuesta;
                }

                if (notificacionSME.Para != null)
                {
                    notificacionSME.Para = notificacionSME.Para.Trim();
                }

                log.Info("Ejecutando el servicio del correo");
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string jsonString = JsonSerializar.Serialize(notificacionSME);
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    respuesta.Mensaje = client.UploadString(new Uri(rutaServicio), "POST", jsonString);
                    respuesta.Estado = Constante.COD_OK;
                }

            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
            }
            return respuesta;
        }

        //private static string CrearTokenADN()
        //{
        //    byte[] aleatorio = new byte[8];
        //    byte[] fecha = UTF8Encoding.UTF8.GetBytes(DateTime.Now.ToString());

        //    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
        //    rng.GetBytes(aleatorio);

        //    return Convert.ToBase64String(aleatorio) + Convert.ToBase64String(fecha);
        //}

        private void habilitaCampo(TextBox campo, bool enabled, string clase, bool readOnly)
        {
            campo.Enabled = enabled;

            campo.CssClass = clase;

            campo.ReadOnly = readOnly;
        }

        [WebMethod]
        public static Respuesta FormatoConsentimientoAsesoria(string idConsentimientoAsesoria, string tipoDocumento, string numeroDocumento)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    log.Info("Accediendo a las Key necesarias: token");
                    string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                    string urlFormatoConsentimientoCliente = ConfigurationManager.AppSettings["url_formato_consentimiento_cliente"].ToString();

                    string usuario = HttpContext.Current.Session["Usuario"].ToString();
                    string token_generado = string.Empty;

                    log.Info("Consumiendo servicio token: " + urlToken);

                    /*Obtener token*/
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

                    /*Obtener indicador cliente permitido*/
                    if (token_generado != "")
                    {
                        log.Info("Consumiendo servicio pdf: " + urlFormatoConsentimientoCliente);

                        urlFormatoConsentimientoCliente = string.Format(urlFormatoConsentimientoCliente, idConsentimientoAsesoria, usuario);

                        log.Info("Leyendo el servicio");

                        WebClient myWebClient = new WebClient();
                        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token_generado));
                        myWebClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

                        byte[] formatoByteArray = myWebClient.DownloadData(urlFormatoConsentimientoCliente);
                        myWebClient.Dispose();

                        File.WriteAllBytes(HostingEnvironment.MapPath("~") + @"\\ArchivosTemporales\\RVI\\Consentimiento\\Consentimiento_" + tipoDocumento + numeroDocumento + ".pdf", formatoByteArray);

                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                        respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Consentimiento de asesoría generado correctamente." });
                    }

                }
                catch (Exception ex)
                {
                    log.Error("Error: " + Utilitarios.FormatearError(new List<string> { ex.Message }));
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta ReenviarFormatoConsentimientoAsesoria(string tokenUsuario, string cuspp, string idConsentimientoAsesoria, string tipoIdentificacion, string numeroIdentificacion, string nombre, string apellidoPaterno, string apellidoMaterno, string email, string numAgente)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        log.Info("Inicio ReenviarFormatoConsentimientoAsesoria");
                        string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                        string urlFormatoConsentimientoCliente = ConfigurationManager.AppSettings["url_formato_consentimiento_cliente"].ToString();

                        string usuario = HttpContext.Current.Session["Usuario"].ToString();
                        string token_generado = string.Empty;

                        log.Info("Consumiendo servicio token: " + urlToken);

                        /* Obtener token */
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";

                        log.Info("Pasando el json al servicio");
                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string json = JsonConvert.SerializeObject(new { usuario });

                            streamWriter.Write(json);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }
                        var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            var jsonResult = streamReader.ReadToEnd();
                            JObject jObject = JObject.Parse(jsonResult);
                            token_generado = (string)jObject["accessToken"];
                        }

                        /* Obtener indicador cliente permitido */
                        if (token_generado != "")
                        {
                            urlFormatoConsentimientoCliente = string.Format(urlFormatoConsentimientoCliente, idConsentimientoAsesoria, usuario);
                            log.Info("Consumiendo servicio pdf: " + urlFormatoConsentimientoCliente);

                            WebClient myWebClient = new WebClient();
                            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token_generado));
                            myWebClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

                            byte[] formatoByteArray = myWebClient.DownloadData(urlFormatoConsentimientoCliente);
                            myWebClient.Dispose();

                            string rutaArchivoConsentimiento = string.Format("{0}\\Consentimiento_{1}{2}.pdf", ConfigurationManager.AppSettings["ruta_Reporte_Generado_Trazabilidad"], tipoIdentificacion, numeroIdentificacion);
                            File.WriteAllBytes(rutaArchivoConsentimiento, formatoByteArray);


                            // Obteniendo datos del agente
                            List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                            Agente agente = listaAgentes.Find(a => a.Id == numAgente);
                            if (agente == null && Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]))
                            {
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado("", cuspp, "", "", "");
                                agente = servicioCotizador.ObtenerUltimoAgentePorCartera(afiliado.Agente.IdCartera, usuario);
                            }

                            SMEEnvio envio = new SMEEnvio
                            {
                                Email = email,
                                Destinatario = string.Format("{0} {1} {2}", nombre, apellidoPaterno, apellidoMaterno),
                                NumeroDocumento = numeroIdentificacion,
                                Contrasenia = "",
                                NumeroPoliza = "N/A",
                                ProcesoSme = ConfigurationManager.AppSettings["SMEConsentimientoRPRespuesta"],
                                RutaPdf = rutaArchivoConsentimiento,
                                CamposDinamicosSerializados = JsonConvert.SerializeObject(new
                                {
                                    Id_nombres = nombre,
                                    Id_agente = agente.Nombre
                                })
                            };
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            long codigoSME = servicioCotizador.EnviarCorreoSME(envio);

                            respuesta.Estado = Constante.COD_OK;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Consentimiento de asesoría enviado correctamente." });
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Se ha producido un error al Reenviar el Formato de Consentimiento", ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                }
                return respuesta;
            }
        }

        public static string CrearToken(int longitud, string caracteresPermitidos = "ABCDEFGHJKMNPQRSTUVWXY0123456789")
        {
            if (longitud < 0) throw new ArgumentOutOfRangeException("longitud", "La longitud no puede ser menor a cero.");
            if (string.IsNullOrEmpty(caracteresPermitidos)) throw new ArgumentException("El parámetro caracteresPermitidos no debe estar vacío.");

            const int byteSize = 0x100;
            var allowedCharSet = new HashSet<char>(caracteresPermitidos).ToArray();
            if (byteSize < allowedCharSet.Length) throw new ArgumentException(string.Format("El parámetro caracteresPermitidos no puede contener más de {0} caracteres.", byteSize));

            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                var result = new StringBuilder();
                var buf = new byte[128];
                while (result.Length < longitud)
                {
                    rng.GetBytes(buf);
                    for (var i = 0; i < buf.Length && result.Length < longitud; ++i)
                    {
                        var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                        if (outOfRangeStart <= buf[i]) continue;
                        result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                    }
                }
                return result.ToString();
            }
        }

        private void obtenerConsentimiento(Afiliado afiliado, bool guardar)
        {
            //consentimiento de asesoria
            DateTime fechaConsentimiento = DateTime.Now;
            string consentimientoToken = string.Empty;
            string telefonoCliente = string.Empty;
            string celularCliente = string.Empty;
            string correoCliente = string.Empty;
            string tratamientoConsentimiento = string.Empty;

            bool ind_consentimiento = consentimiento(Convert.ToInt32(Enums.ConfiguracionConsentimiento.RP.StringValue()), TipoDocumento_RP.SelectedValue, NumeroDocumento_RP.Text, (string)Session["Usuario"], ref fechaConsentimiento, ref consentimientoToken, ref telefonoCliente, ref celularCliente, ref correoCliente, ref tratamientoConsentimiento);

            HConsentimiento.Value = "NO";
            if (ind_consentimiento)
            {
                HConsentimiento.Value = "OK";
            }

            HToken_IFP.Value = consentimientoToken;

            if (ind_consentimiento)
            {

                var glsfechaConsentimiento = fechaConsentimiento.ToString("dd'/'MM'/'yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                PanelConsentimiento.Controls.Clear();
                ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                cuadroMensajeConsentimiento.tieneConsentimiento = true;
                cuadroMensajeConsentimiento.Clase = "grilla_exito";
                cuadroMensajeConsentimiento.Mensaje = "Este cliente dio su consentimiento de asesoría " + tratamientoConsentimiento + " el " + glsfechaConsentimiento + ".<br><br>";
                cuadroMensajeConsentimiento.Mensaje += "<a id=\"PlantillaConsentimientoAsesoria_IFP\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">file_download</span>Descargar formato de consentimiento de asesoría</a><br>";
                cuadroMensajeConsentimiento.Mensaje += "<a id=\"ReenviarPlantillaConsentimientoAsesoria_IFP\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">email</span>Reenviar consentimiento de asesoría</a>";
                PanelConsentimiento.Controls.Add(cuadroMensajeConsentimiento);

                if (afiliado.TelefonoCliente.Trim().Length > 0 && afiliado.TelefonoCliente != null)
                {
                    Telefono_RP.Text = afiliado.TelefonoCliente;
                    habilitaCampo(Telefono_RP, true, "formTextbox telefono", false);
                }
                else
                {
                    Telefono_RP.Text = telefonoCliente;
                    afiliado.TelefonoCliente = telefonoCliente;
                    afiliado.Telefonos = "";
                    habilitaCampo(Telefono_RP, true, "formTextbox telefono", false);
                }

                if (afiliado.CelularCliente.Trim().Length > 0 && afiliado.CelularCliente != null)
                {
                    Celular_RP.Text = afiliado.CelularCliente;
                    habilitaCampo(Celular_RP, true, "formTextbox telefono", false);
                }
                else
                {
                    Celular_RP.Text = celularCliente;
                    afiliado.CelularCliente = celularCliente;
                    afiliado.Celulares = "";
                    habilitaCampo(Celular_RP, true, "formTextbox telefono", false);
                }

                if (afiliado.CorreoElectronicoCliente.Trim().Length > 0 && afiliado.CorreoElectronicoCliente != null)
                {
                    CorreoElectronico_RP.Text = afiliado.CorreoElectronicoCliente;
                    habilitaCampo(CorreoElectronico_RP, true, "formTextbox", false);
                }
                else
                {
                    CorreoElectronico_RP.Text = correoCliente;
                    afiliado.CorreoElectronicoCliente = correoCliente;
                    afiliado.CorreoElectronico = "";
                    habilitaCampo(CorreoElectronico_RP, true, "formTextbox", false);
                }

            }
            else
            {
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                List<Direccion> direcciones = servicioCotizador.ListarDireccion(HCUSPP_RP.Value);
                Direccion direccion = direcciones.Find(dir => dir.Principal == true);

                HttpContext.Current.Session["ssDireccion"] = direccion;

                //if (afiliado.Telefonos.Trim().Length > 0 && afiliado.Telefonos != null)
                //{
                //    Telefono_RP.Text = afiliado.Telefonos;
                //}
                //else 

                if (afiliado.Telefonos.Trim().Length > 0 && afiliado.Telefonos != null)
                {
                    Telefono_RP.Text = afiliado.Telefonos;
                }

                if (afiliado.Celulares.Trim().Length > 0 && afiliado.Celulares != null)
                {
                    Celular_RP.Text = afiliado.Celulares;
                }

                if ((string)HttpContext.Current.Session["RolAzman"] != Enums.RolAzman.AgenteExterno.StringValue())
                {
                    ControlLabel(Telefono_RP);
                    ControlLabel(Celular_RP);
                    ControlLabel(CorreoElectronico_RP);
                }

                var resultadoValidacionConsentimiento = validacionConsentimientoAsesoria(Nombres_RP.Text, ApellidoPaterno_RP.Text, ApellidoMaterno_RP.Text, TipoDocumento_RP.SelectedValue, NumeroDocumento_RP.Text, CorreoElectronico_RP.Text, Telefono_RP.Text, Celular_RP.Text, SaldoCIC_RP.Text, RangoInversion_RP.Text, CentroLaboral_RP.Text, direccion);

                if (resultadoValidacionConsentimiento.Estado != Constante.COD_OK)
                {
                    PanelConsentimiento.Controls.Clear();
                    ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                    cuadroMensajeConsentimiento.tieneConsentimiento = true;
                    cuadroMensajeConsentimiento.Clase = "mensaje_advertencia_amarillo";
                    cuadroMensajeConsentimiento.Mensaje = resultadoValidacionConsentimiento.Mensaje;
                    PanelConsentimiento.Controls.Add(cuadroMensajeConsentimiento);
                }
                else
                {
                    PanelConsentimiento.Controls.Clear();
                    ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                    cuadroMensajeConsentimiento.tieneConsentimiento = true;
                    cuadroMensajeConsentimiento.Clase = "mensaje_advertencia_amarillo";
                    cuadroMensajeConsentimiento.Mensaje = "Este cliente no ha brindado su consentimiento de asesoría para el producto <b>Renta Particular</b>.<br><br>";
                    cuadroMensajeConsentimiento.Mensaje += "<a id=\"LinkConsentimientoAsesoria_IFP_SMS\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">email</span><span>Enviar enlace de solicitud de CDA al celular <b>" + afiliado.Celulares.ToString() + "</b></span></a>";
                    cuadroMensajeConsentimiento.Mensaje += "<br>";
                    cuadroMensajeConsentimiento.Mensaje += "<a id=\"LinkConsentimientoAsesoria_IFP\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">email</span><span>Enviar enlace de solicitud de CDA al correo <b>" + afiliado.CorreoElectronico.ToString() + "</b></span></a>";

                    // Botón de envío manual
                    if (HidConsentimientoAsesoria.Value != string.Empty && Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.PlantillaCorreoElectronico))
                    {
                        cuadroMensajeConsentimiento.Mensaje += string.Format("<br><br>En caso de que el correo con trazabilidad no llegue al cliente por problemas entre el proveedor de envío y la casilla del cliente, puede intentar <a href=\"{0}?tp=2&td={1}&nd={2}&c={3}\">enviarlo de manera manual</a>. (Esta opción no cuenta con seguimiento de trazabilidad ni notificación ante rebotes)", ResolveUrl(((List<OpcionSistema>)Session["OpcionesSistema"]).Find(o => o.IdAzman == Convert.ToInt32(Enums.OpcionesSistema.PlantillaCorreoElectronico)).Ruta), afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion, CUSPP_RP.Text);
                    }
                    PanelConsentimiento.Controls.Add(cuadroMensajeConsentimiento);
                }
            }

            if (guardar)
            {
                log.Debug("Inicio Cotizador.obtenerConsentimiento");
                servicioCotizador.ActualizarAfiliado(afiliado);
                log.Debug("Fin Cotizador.obtenerConsentimiento");
            }
        }

        public void ControlLabel(Control control)
        {
            if (control is TextBox)
            {
                ((TextBox)control).ReadOnly = true;
                ((TextBox)control).CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro";
            }
            if (control is DropDownList)
            {
                ((DropDownList)control).Enabled = false;
                ((DropDownList)control).CssClass = "formCombobox formComboboxReadOnly formTextboxReadOnly";
            }
        }

        //<FIN.GTI_26697>

        [WebMethod]
        public static Respuesta EliminarGrupoFamiliar(string tokenUsuario, int idGrupoFamiliar)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoEliminar))
                        {
                            // Validar la cartera del agente
                            if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
                            {
                                log.Debug("Eliminar beneficiario: " + idGrupoFamiliar + " , " + (string)HttpContext.Current.Session["Usuario"]);
                                GrupoFamiliar gf = new GrupoFamiliar
                                {
                                    Id = Convert.ToInt32(idGrupoFamiliar),
                                    Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                };

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                servicioCotizador.EliminarGrupoFamiliar(gf);
                                respuesta.Estado = Constante.COD_OK;

                                string nombreTerminal = String.Empty;
                                try
                                {
                                    nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                }
                                catch (Exception)
                                {
                                    log.Warn(string.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                        HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                }

                                nombreTerminal += HttpContext.Current.Request.UserAgent;

                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                    NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                    Detalle = string.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "EliminarGrupoFamiliar", Environment.NewLine, Environment.NewLine, "Grupo Familiar", JsonConvert.SerializeObject(gf)),
                                    IdTipoEvento = Enums.EventoLog.EliminarDatosBeneficiario.StringValue()
                                });

                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.TelefonoEliminar.StringValue()));
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
        public static Respuesta ImprimirPoliza(string numPoliza)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var respuesta = new Respuesta();

                    log.Info("Accediendo a las Key necesarias");
                    string urlPdfPoliza = ConfigurationManager.AppSettings["url_pdf_emision_poliza_admwr"].ToString();
                    string usuarioAdmwrApi = ConfigurationManager.AppSettings["usuario_admwr_api"].ToString();
                    string contraseñaAdmwrApi = ConfigurationManager.AppSettings["contraseña_admwr_api"].ToString();

                    // Configurar ServicePointManager para manejar SSL/TLS moderno
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072 | (SecurityProtocolType)768 | SecurityProtocolType.Tls;
                    ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                    var myWebClient = new WebClient();
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuarioAdmwrApi + ":" + contraseñaAdmwrApi));
                    myWebClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

                    log.Info("Consumiendo método para generar pdf de póliza");
                    urlPdfPoliza = string.Format(urlPdfPoliza, numPoliza);
                    byte[] polizaByteArray = myWebClient.DownloadData(urlPdfPoliza);
                    myWebClient.Dispose();

                    var nombreArchivo = "Poliza.pdf";
                    var rutaCarpeta = HostingEnvironment.MapPath("~") + $"\\ArchivosTemporales\\IFP\\Poliza\\";
                    var archivoTemporal = rutaCarpeta + nombreArchivo;

                    if (File.Exists(archivoTemporal))
                        File.Delete(archivoTemporal);

                    //Crear arbol de carpetas, si no existen
                    if (!Directory.Exists(rutaCarpeta)) Directory.CreateDirectory(rutaCarpeta);

                    File.WriteAllBytes(archivoTemporal, polizaByteArray);

                    respuesta.Archivos = new List<string>
                    {
                        nombreArchivo
                    };

                    respuesta.Estado = Constante.COD_OK;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                    respuesta.Mensaje = "PDF de póliza, generado correctamente.";

                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error($"Se ha producido el siguiente error: [{ex.Message}]", ex);
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                    return respuesta;
                }
            }
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

        [WebMethod]
        public static Respuesta ObtenerEstudioNecesidad(string numSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var respuesta = new Respuesta();
                    //string tipoDocumento = Enums.TipoDocumentoCloudStorage.DNI.StringValue();

                    //Armar Json EdN
                    EstudioNecesidadAPI estudioNecesidadAPI = new EstudioNecesidadAPI();
                    SolicitudEdNAPI solicitud_edn = new SolicitudEdNAPI();
                    List<CotizacionEdNAPI> cotizaciones_edn = new List<CotizacionEdNAPI>();
                    CotizacionEdNAPI cotizacion_edn;
                    BeneficiarioEdNAPI beneficiario_edn;

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                    List<GrupoFamiliar> beneficiarios = new List<GrupoFamiliar>();

                    SolicitudIFP solicitudIFP = servicioCotizador.ObtenerDatosSolicitudIFP(numSolicitud);

                    solicitud_edn.num_solicitud = solicitudIFP.Id;
                    solicitud_edn.fec_solicitud = solicitudIFP.FechaSolicitud.Value;
                    solicitud_edn.val_mto_cta_individual = solicitudIFP.PrimaUnica;
                    solicitud_edn.cod_moneda_cta_indiv = solicitudIFP.MonedaPrimaUnica.Id;

                    var listaCotiza = solicitudIFP.Cotizaciones;

                    beneficiarios = solicitudIFP.Beneficiarios;


                    estudioNecesidadAPI.solicitud = solicitud_edn;

                    if (listaCotiza != null)
                    {
                        foreach (var cot in listaCotiza)
                        {
                            cotizacion_edn = new CotizacionEdNAPI();
                            cotizacion_edn.num_correlativo = (int)cot.Correlativo;

                            cotizacion_edn.cod_tipo_temporalidad = cot.Temporalidad.Anhos.ToString();

                            cotizacion_edn.val_per_diferido = cot.ValPerDiferido;

                            cotizaciones_edn.Add(cotizacion_edn);
                        }

                        estudioNecesidadAPI.cotizaciones = new List<CotizacionEdNAPI>();
                        estudioNecesidadAPI.cotizaciones = cotizaciones_edn;
                    }

                    log.Info("Cantidad de beneficiarios(obtener EdN): " + solicitudIFP.Beneficiarios.Count);
                    beneficiarios = beneficiarios.GroupBy(be => new { be.Identificacion.IdTipo, be.Identificacion.Numero }).Select(s => s.First()).ToList();
                    log.Info("Cantidad de beneficiarios(obtener EdN): " + beneficiarios.Count);

                    estudioNecesidadAPI.beneficiarios = new List<BeneficiarioEdNAPI>();
                    foreach (var benefi in beneficiarios)
                    {
                        beneficiario_edn = new BeneficiarioEdNAPI();
                        beneficiario_edn.ape_paterno = benefi.ApellidoPaterno;
                        beneficiario_edn.ape_materno = benefi.ApellidoMaterno;
                        beneficiario_edn.nom_persona = benefi.Nombre;

                        switch (benefi.Identificacion.IdTipo)
                        {
                            case "D":
                                beneficiario_edn.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                break;
                            case "E":
                                beneficiario_edn.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.CE.StringValue();
                                break;
                            case "P":
                                beneficiario_edn.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.PAS.StringValue();
                                break;
                            default:
                                beneficiario_edn.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                break;
                        }

                        beneficiario_edn.num_identificacion = benefi.Identificacion.Numero;
                        beneficiario_edn.cod_parentezco = benefi.Parentesco.Id;

                        estudioNecesidadAPI.beneficiarios.Add(beneficiario_edn);
                    }


                    string nombreArchivo = "EN_" + numSolicitud + "_" + beneficiarios.Find(ben => ben.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue()).Identificacion.Numero + ".pdf";

                    var respuesta_api_EdN = Utilitario.ObtenerEstudioNecesidadesLocal(estudioNecesidadAPI, HttpContext.Current.Session["Usuario"].ToString());

                    //var respuesta_api_EdN = Utilitario.ObtenerEstudioNecesidades(numSolicitud, tipoDocumento, titular.Identificacion.Numero);

                    respuesta.ArchivoSerializado = JsonConvert.DeserializeObject<string>(respuesta_api_EdN);
                    respuesta.Contenido = nombreArchivo;

                    respuesta.Estado = Constante.COD_OK;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                    respuesta.Mensaje = "Se obtuvo el formato de estudio de necesidades correctamente.";

                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error($"Se ha producido el siguiente error: [{ex.Message}]", ex);
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    return respuesta;
                }
            }
        }

        public void HabilitarControl(Control control)
        {
            /*if (control is TextBox)
            {
                ((TextBox)control).ReadOnly = true;
                ((TextBox)control).CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro";
            }*/
            /*if (control is DropDownList)
            {
                ((DropDownList)control).Enabled = false;
                ((DropDownList)control).CssClass = "formCombobox formComboboxReadOnly";
            }*/
            if (control is HyperLink)
            {
                ((HyperLink)control).CssClass = "boton darkblue sharp";
                ((HyperLink)control).ToolTip = string.Empty;
            }
            /*if (control is Button)
            {
                ((Button)control).CssClass = "boton darkblue sharp";
                ((Button)control).ToolTip = string.Empty;
            }*/
        }
    }

    [Serializable]
    public class ParametrosEnvioCDA
    {
        public string nombres { get; set; }
        public string tipoDocumento { get; set; }
        public string numeroDocumento { get; set; }
        public string correo { get; set; }
        public string cuspp { get; set; }
        public string token { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string sexo { get; set; }
        public string fechaNacimiento { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }
        public string idConsentimientoAsesoria { get; set; }
        public string indConsentimiento { get; set; }
        public string canalComunicacion { get; set; }
    }
}