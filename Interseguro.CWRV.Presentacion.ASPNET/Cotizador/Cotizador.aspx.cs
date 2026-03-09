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
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Hosting;

namespace Interseguro.CWRV.Presentacion.ASPNET.Cotizador
{
    public partial class Cotizador : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Cotizador));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionExtraoficial))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                            LimpiarFormularios();

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
                            // Se vuelve a formatear el monto sin separador de miles para que el plugin autoNumeric no falle
                            if (SaldoCIC.Text != string.Empty) SaldoCIC.Text = Convert.ToDouble(SaldoCIC.Text, new CultureInfo("es-PE")).ToString();
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.CotizacionExtraoficial.StringValue()));
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

            CargarCombobox(AFP, listaCombobox[(int)Enums.CategoriaCombobox.Afp]);
            CargarCombobox(Categoria, listaCombobox[(int)Enums.CategoriaCombobox.Categoria]);
            CargarCombobox(Sexo, listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);
            CargarComboboxenBlanco(TipoDocumento, listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);
            CargarComboboxNuevosDatos(ModDomicilio, "TIPOVIA");
            CargarCombobox(ModTelTipo, listaCombobox[(int)Enums.CategoriaCombobox.Telefono]);
            CargarCombobox(ModGruFamTipoIdentificacion, listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);

            //Apoderado
            CargarCombobox(ModGruFamTipoIdentificacionAprdo, listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);

            var lstParentesco = listaCombobox[(int)Enums.CategoriaCombobox.Parentesco];

            List<string> lstNoParentesco = CargarNoParentescos();

            CargarCombobox(ModGruFamParentesco, lstParentesco.Where(p => !(lstNoParentesco.Contains(p.Id))).ToList());

            CargarCombobox(ModGruFamSexo, listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);
            CargarCombobox(ModGruFamTipoInvalidez, listaCombobox[(int)Enums.CategoriaCombobox.Invalidez]);

            //Apoderado
            CargarCombobox(ModGruFamSexoAprdo, listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);

            CargarCombobox(ModSolTipoPension, listaCombobox[(int)Enums.CategoriaCombobox.Prestacion]);
            CargarCombobox(ModSolCategoria, listaCombobox[(int)Enums.CategoriaCombobox.Categoria]);
            CargarCombobox(ModSolFactorTasa, listaCombobox[(int)Enums.CategoriaCombobox.FactorTRA]);

            Session["ComboMoneda"] = listaCombobox[(int)Enums.CategoriaCombobox.Moneda];

            List<Parametro> comboModalidad = new List<Parametro>();
            comboModalidad.Add(new Parametro { Id = "I", Glosa = "I" });
            comboModalidad.Add(new Parametro { Id = "D", Glosa = "D" });
            comboModalidad.Add(new Parametro { Id = "I-RM", Glosa = "I-RM" });
            comboModalidad.Add(new Parametro { Id = "I-RC", Glosa = "I-RC" });
            comboModalidad.Add(new Parametro { Id = "I-RB", Glosa = "I-RB" });
            comboModalidad.Add(new Parametro { Id = "I-RVE", Glosa = "I-RVE" });
            Session["ComboModalidad"] = comboModalidad;

            Session["ComboPeriodoTemporal"] = listaCombobox[(int)Enums.CategoriaCombobox.PeriodoTemporal];

            List<Parametro> comboPorcentajeRentas = new List<Parametro>();
            comboPorcentajeRentas.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPorcentajeRentas.Add(new Parametro { Id = "50", Glosa = "50%" });
            comboPorcentajeRentas.Add(new Parametro { Id = "75", Glosa = "75%" });
            Session["ComboPorcentajeRentas"] = comboPorcentajeRentas;

            List<Parametro> comboPeriodoGarantizado = new List<Parametro>();
            comboPeriodoGarantizado.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "10", Glosa = "10" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "15", Glosa = "15" });
            Session["ComboPeriodoGarantizado"] = comboPeriodoGarantizado;

            Session["ComboCapital"] = listaCombobox[(int)Enums.CategoriaCombobox.Capital];

            ModDirPrincipal.Items.Add(new ListItem("«Seleccione»", "0"));
            ModDirPrincipal.Items.Add(new ListItem("Sí", "S"));
            ModDirPrincipal.Items.Add(new ListItem("No", "N"));

            ModTelPrincipal.Items.Add(new ListItem("«Seleccione»", "0"));
            ModTelPrincipal.Items.Add(new ListItem("Sí", "S"));
            ModTelPrincipal.Items.Add(new ListItem("No", "N"));

            ModGruFamIndInvalidez.Items.Add(new ListItem("«Seleccione»", "0"));
            ModGruFamIndInvalidez.Items.Add(new ListItem("Sí", "S"));
            ModGruFamIndInvalidez.Items.Add(new ListItem("No", "N"));

            var montosCIC = servicioCotizador.ListarMontoCIC();
            CargarCombobox(ModSolListaCIC, montosCIC);

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            LabModSolLineaACOMDCOM.Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);

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

            // Permisos Actualizar Afiliado
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoActualizar))
            {
                PerGuardar.Value = "1";
            }
            else
            {
                InhabilitarControl(CorreoElectronico);
                InhabilitarControl(Categoria);
                InhabilitarControl(AFP);
                InhabilitarControl(SaldoCIC);
                InhabilitarControl(Guardar);
                PerGuardar.Value = "0";
            }

            // Permisos Insertar Dirección
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionInsertar))
            {
                PerNuevaDireccion.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevaDireccion);
                PerNuevaDireccion.Value = "0";
            }

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

            // Permisos Insertar Solicitud
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudInsertar))
            {
                PerNuevaSolicitud.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevaSolicitud);
                PerNuevaSolicitud.Value = "0";
            }

            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;

            CargarCombobox(ddlMonedaReferencia, (List<Parametro>)Session["ComboMoneda"]);
            CargarCombobox(ddlMonedaPensionPago, (List<Parametro>)Session["ComboMoneda"]);

            switch ((string)Session["RolAzman"])
            {
                case "JEF.RVI.OPE":
                case "ANL.RVI":
                    hdnMostrarAporteAdicional.Value = "1";
                    break;
                default:
                    hdnMostrarAporteAdicional.Value = "0";
                    break;
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

        private void CargarComboboxNuevosDatos(DropDownList control, string tabla)
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerParametros(tabla);

            control.Items.Clear();

            control.Items.Add(new ListItem("«Seleccione»", "0"));

            foreach (Parametro item in listaParametro)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }

        private void CargarComboboxNuevosDatos(DropDownList control, string tabla, string parametro)
        {
            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
            string usuario = HttpContext.Current.Session["Usuario"].ToString();

            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));

            if (tabla == "Departamento")
            {
                JArray listaDepartamentos = new JArray();
                var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
                urlDepartamentos = string.Format(urlDepartamentos, usuario);
                listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, usuario, listaDepartamentos);

                foreach (var item in listaDepartamentos)
                {
                    control.Items.Add(new ListItem(item["gls_departamento"].ToString().ToUpper(), item["id_departamento"].ToString().ToLower()));
                }
            }
            else if (tabla == "Provincia")
            {

                JArray listaProvincias = new JArray();
                var urlProvincias = ConfigurationManager.AppSettings["url_lista_provincias"].ToString();
                urlProvincias = string.Format(urlProvincias, parametro, usuario);
                listaProvincias = ObtenerUbigeo(urlToken, urlProvincias, usuario, listaProvincias);

                foreach (var item in listaProvincias)
                {
                    control.Items.Add(new ListItem(item["gls_provincia"].ToString().ToUpper(), item["id_provincia"].ToString().ToLower()));
                }
            }
            else if (tabla == "Distrito")
            {
                JArray listaDistritos = new JArray();
                var urlDistritos = ConfigurationManager.AppSettings["url_lista_distritos"].ToString();
                urlDistritos = string.Format(urlDistritos, parametro, usuario);
                listaDistritos = ObtenerUbigeo(urlToken, urlDistritos, usuario, listaDistritos);

                foreach (var item in listaDistritos)
                {
                    control.Items.Add(new ListItem(item["gls_distrito"].ToString().ToUpper(), item["id_distrito"].ToString().ToLower()));
                }
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

                            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
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
                                    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                        Enums.OpcionesSistema.BusquedaAfiliadoConsultar.StringValue()));
                                    control.PermisoConsultar = false;
                                }
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
                            //<INIGTI_753>

                            List<string> lstNoParentesco = new List<string>();

                            lstNoParentesco.Add(Enums.Parentesco.Nieto.StringValue());
                            lstNoParentesco.Add(Enums.Parentesco.Otros.StringValue());
                            lstNoParentesco.Add(Enums.Parentesco.Sobrino.StringValue());
                            lstNoParentesco.Add(Enums.Parentesco.Hermano.StringValue());
                            lstNoParentesco.Add(Enums.Parentesco.Primo.StringValue());

                            control.Grupos = grupos.Where(p => !(lstNoParentesco.Contains(p.Parentesco.Id.ToString()))).ToList();
                            //control.Grupos = grupos.Where(p => p.Parentesco.Id != "95").ToList();  //95=NIETO

                            HttpContext.Current.Session["ssIdContacto"] = grupos.Find(t => t.Parentesco.Id == "80").Id;

                            //<FINGTI_753>
                            control.PermisoConsultar = true;
                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar)) ? true : false;
                            //<SRIINI06326>
                            control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                            //<SRIFIN06326>

                            control.PermisoEliminar = true;
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
        public static string CargarTablaSolicitudes(string tokenUsuario, string cuspp)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudes)pagina.LoadControl("~/Controles/TablaSolicitudes.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Solicitud> solicitudes = servicioCotizador.ListarSolicitud(cuspp);

                            control.Solicitudes = solicitudes;
                            control.PermisoConsultar = true;

                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudActualizar)) ? true : false;
                            control.PermisoCorreoElectronico = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudEnviarCorreo)) ? true : false;
                            control.PermisoExportarPDF = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudExportarPDF)) ? true : false;
                            //<SRIINI06326>
                            control.PermisoReporteEscenario = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudReporteEscenarios)) ? true : false;
                            control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                            //<SRIFIN06326>

                            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                            control.RedLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);
                            control.TieneLote = solicitudes.Exists(s => s.NumLoteCotizacion > 0);
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
        public static string CargarTablaSolicitudesSimulador(string tokenUsuario, string cuspp)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesSimulador)pagina.LoadControl("~/Controles/TablaSolicitudesSimulador.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Solicitud> solicitudes = servicioCotizador.ListarSolicitud(cuspp);

                            control.Solicitudes = solicitudes;
                            control.PermisoConsultar = true;
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
        public static string CargarTablaCotizaciones(List<Cotizacion> cotizaciones, string tipoCotizacion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();
                    var control = (TablaCotizaciones)pagina.LoadControl("~/Controles/TablaCotizaciones.ascx");

                    control.Cotizaciones = cotizaciones;

                    // Validando si el acceso es desde dentro de la red de Interseguro o desde Internet
                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                    {
                        control.PermisoTRA = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoTRA)) ? true : false;
                    }
                    else
                    {
                        control.PermisoTRA = false;
                    }

                    if (tipoCotizacion == "EXTRAOFICIAL")
                    {
                        control.PermisoAgregar = true;
                        control.PermisoModificar = true;
                        control.PermisoEliminar = true;
                    }
                    else
                    {
                        control.PermisoAgregar = false;
                        control.PermisoModificar = false;
                        control.PermisoEliminar = false;
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
        public static string CargarTablaCotizacionesSimulador(string tokenUsuario, List<Cotizacion> cotizaciones, int idSimulador, int filtro, bool movil, string modalidad, string moneda, int? periodoGarantizado)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaCotizacionesSimulador)pagina.LoadControl("~/Controles/TablaCotizacionesSimulador.ascx");

                        control.VerDragGraficar = !movil;
                        control.VerRadioGraficar1 = movil;

                        if (idSimulador == (int)Enums.OpcionesSistema.SimuladorJubilarseHoy)
                        {
                            cotizaciones =
                                cotizaciones
                                    .FindAll(c => (c.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue() || c.Modalidad.Id == Enums.Modalidad.Diferida.StringValue()) && c.Moneda.Id != Enums.Moneda.Dolares.StringValue());
                            control.VerNroCotizacion = true;
                            control.VerMoneda = true;
                            control.VerModalidad = true;
                            control.VerPeriodoDiferido = true;
                            control.VerPeriodoGarantizado = true;
                            //<SRI.INI-20322>
                            control.VerGratificacion = false;
                            control.idSimulador = idSimulador;
                            control.VerPension2 = false;
                            //<SRI.FIN-20322>
                            control.VerPension = true;
                            control.VerRadioGraficar2 = false;
                        }
                        else if (idSimulador == (int)Enums.OpcionesSistema.SimuladorRentaVitaliciaRetiroProgramado)
                        {
                            cotizaciones =
                                cotizaciones
                                    //<SRI.INI-20322>
                                    //.FindAll(c => c.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue() && c.PeriodoGarantizado == 0 && (c.Moneda.Id == Enums.Moneda.Soles.StringValue() || c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()));
                                    .FindAll(c => (c.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue() || c.Modalidad.Id == Enums.Modalidad.Diferida.StringValue()) && (c.Moneda.Id == Enums.Moneda.Soles.StringValue() || c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()));
                            //<SRI.FIN-20322>
                            control.VerNroCotizacion = true;
                            control.VerMoneda = true;
                            control.VerModalidad = true;
                            control.VerPeriodoDiferido = true;
                            control.VerPeriodoGarantizado = true;
                            //<SRI.INI-20322>
                            control.VerGratificacion = true;
                            control.idSimulador = idSimulador;
                            control.VerPension2 = false;
                            //<SRI.FIN-20322>
                            control.VerPension = true;
                        }
                        else if (idSimulador == (int)Enums.OpcionesSistema.SimuladorInmediataDiferida)
                        {
                            control.Filtro = filtro.ToString();
                            control.VerNroCotizacion = true;
                            control.VerMoneda = true;
                            control.VerModalidad = false;
                            control.VerPeriodoDiferido = false;
                            control.VerPeriodoGarantizado = true;
                            //<SRI.INI-20322>
                            control.VerGratificacion = false;
                            control.idSimulador = idSimulador;
                            control.VerPension2 = false;
                            //<SRI.FIN-20322>
                            control.VerPension = true;
                            switch (filtro)
                            {
                                case 1:
                                    cotizaciones =
                                        cotizaciones
                                            .FindAll(c => c.Modalidad.Id == Enums.Modalidad.Inmediata.StringValue() && c.Moneda.Id == moneda && c.PeriodoGarantizado == periodoGarantizado);
                                    break;
                                case 2:
                                    cotizaciones =
                                        cotizaciones
                                            .FindAll(c => c.Modalidad.Id == Enums.Modalidad.Diferida.StringValue() && c.PeriodoDiferido == 1 && c.Moneda.Id == moneda && c.PeriodoGarantizado == periodoGarantizado);
                                    break;
                                case 3:
                                    cotizaciones =
                                        cotizaciones
                                            .FindAll(c => c.Modalidad.Id == Enums.Modalidad.Diferida.StringValue() && c.PeriodoDiferido == 2 && c.Moneda.Id == moneda && c.PeriodoGarantizado == periodoGarantizado);
                                    break;
                                //<SRI.INI-20322>
                                case 4:
                                    cotizaciones =
                                        cotizaciones
                                            .FindAll(c => c.Modalidad.Id == Enums.Modalidad.Diferida.StringValue() && c.PeriodoDiferido == 3 && c.Moneda.Id == moneda && c.PeriodoGarantizado == periodoGarantizado);
                                    break;
                                case 5:
                                    cotizaciones =
                                        cotizaciones
                                            .FindAll(c => c.Modalidad.Id == Enums.Modalidad.Diferida.StringValue() && c.PeriodoDiferido == 4 && c.Moneda.Id == moneda && c.PeriodoGarantizado == periodoGarantizado);
                                    break;
                                case 6:
                                    cotizaciones =
                                        cotizaciones
                                            .FindAll(c => c.Modalidad.Id == Enums.Modalidad.Diferida.StringValue() && c.PeriodoDiferido == 5 && c.Moneda.Id == moneda && c.PeriodoGarantizado == periodoGarantizado);
                                    break;
                                    //<SRI.FIN-20322>
                            }
                        }
                        else if (idSimulador == (int)Enums.OpcionesSistema.SimuladorTipoMoneda)
                        {
                            string modal = modalidad.Substring(0, 1);
                            int pd = 0;
                            if (modal == Enums.Modalidad.Diferida.StringValue()) pd = Convert.ToInt32(modalidad.Substring(1, 1));
                            cotizaciones =
                                cotizaciones
                                    .FindAll(c => c.Modalidad.Id == modal && c.PeriodoDiferido == pd && c.Moneda.Id == moneda && c.PeriodoGarantizado == periodoGarantizado);
                            control.Filtro = filtro.ToString();
                            control.VerNroCotizacion = true;
                            control.VerMoneda = false;
                            control.VerModalidad = true;
                            control.VerPeriodoDiferido = true;
                            control.VerPeriodoGarantizado = true;
                            //<SRI.INI-20322>
                            control.VerGratificacion = false;
                            control.idSimulador = idSimulador;
                            control.VerPension2 = false;
                            //<SRI.FIN-20322>
                            control.VerPension = true;
                        }
                        //<SRI.INI-20322>
                        else if (idSimulador == (int)Enums.OpcionesSistema.SimuladorQueMeConviene)
                        {
                            //cotizaciones =
                            //    cotizaciones
                            //        .FindAll(c => c.PeriodoGarantizado == 0 && (c.Moneda.Id == Enums.Moneda.Soles.StringValue() || c.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()));
                            cotizaciones =
                                cotizaciones
                                    .FindAll(c => c.Moneda.Id == moneda);
                            control.Filtro = filtro.ToString();
                            control.VerNroCotizacion = false;
                            control.VerMoneda = true;
                            control.VerModalidad = true;
                            control.VerPeriodoDiferido = true;
                            control.VerPeriodoGarantizado = true;
                            control.VerGratificacion = false;
                            control.idSimulador = idSimulador;
                            control.VerPension = true;
                            control.VerPension2 = true;
                        }
                        //<SRI.FIN-20322>

                        //<INIGTI_2145>
                        else if (idSimulador == 90)
                        {
                            control.Filtro = filtro.ToString();
                            control.VerNroCotizacion = false;
                            control.VerMoneda = true;
                            control.VerModalidad = true;
                            control.VerPeriodoDiferido = true;
                            control.VerPeriodoGarantizado = true;
                            control.VerGratificacion = false;
                            control.idSimulador = idSimulador;
                            control.VerPension = true;
                            control.VerPension2 = true;
                        }
                        //<FINGTI_2145>

                        control.Cotizaciones = cotizaciones;
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
        public static string CargarTablaBeneficiarios(string cuspp, List<GrupoFamiliar> beneficiarios)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    if (beneficiarios == null)
                    {
                        var control = (TablaBeneficiarios)pagina.LoadControl("~/Controles/TablaBeneficiarios.ascx");
                        List<GrupoFamiliar> grupoFamiliar = servicioCotizador.ListarGrupoFamiliar(cuspp);
                        grupoFamiliar.ForEach(g => g.Seleccionado = true);
                        //<INIGTI_753>

                        List<string> lstNoParentesco = new List<string>();

                        lstNoParentesco.Add(Enums.Parentesco.Nieto.StringValue());
                        lstNoParentesco.Add(Enums.Parentesco.Otros.StringValue());
                        lstNoParentesco.Add(Enums.Parentesco.Sobrino.StringValue());
                        lstNoParentesco.Add(Enums.Parentesco.Hermano.StringValue());
                        lstNoParentesco.Add(Enums.Parentesco.Primo.StringValue());

                        control.Beneficiarios = grupoFamiliar.Where(p => !(lstNoParentesco.Contains(p.Parentesco.Id.ToString()))).ToList();
                        //control.Beneficiarios = grupoFamiliar.Where(p=>p.Parentesco.Id!="95").ToList(); //95=NIETO

                        //<FINGTI_753>
                        control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                        HttpContext.Current.Session["Beneficiarios"] = control.Beneficiarios;
                        pagina.Controls.Add(control);
                    }
                    else
                    {
                        var control = (TablaRviBenefi)pagina.LoadControl("~/Controles/TablaRviBenefi.ascx");
                        control.Beneficiarios = beneficiarios;
                        control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                        pagina.Controls.Add(control);
                    }

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
        public static string CargarTablaActividades(string tokenUsuario, string cuspp)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaActividades)pagina.LoadControl("~/Controles/TablaActividades.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Actividad> actividades = servicioCotizador.ListarActividad(cuspp);

                            control.Actividades = actividades;
                            HttpContext.Current.Session["Actividades"] = actividades;

                            control.PermisoConsultar = true;
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
        public static void CargarComboProductos(string idTipoPension)
        {
            //Cargar combobox de Productos
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Producto> productos = servicioCotizador.ListarProducto(idTipoPension);
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
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
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
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
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
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
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
                    Respuesta respuesta = new Respuesta
                    {
                        Estado = Constante.COD_ERROR,
                        Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                        Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                        Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message })
                    };
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
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
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
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
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
                                                        string fechaInvalidez,
                                                        string apellidoPaternoApdo,
                                                        string apellidoMaternoApdo,
                                                        string nombresApdo,
                                                        string tipoIdentificacionApdo,
                                                        string numeroIdentificacionApdo,
                                                        string sexoApdo,
                                                        string fechaNacimientoApdo,
                                                        bool indTieneApoderado
        )
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
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
                            {
                                // Validar la cartera de agente
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
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] },
                                        IndTieneApoderado = indTieneApoderado
                                    };
                                    if (apellidoPaterno.Trim() != string.Empty)
                                    {
                                        gru.ApellidoPaterno = apellidoPaterno;
                                    }
                                    if (apellidoMaterno.Trim() != string.Empty)
                                    {
                                        gru.ApellidoMaterno = apellidoMaterno;
                                    }
                                    if (nombres.Trim() != string.Empty)
                                    {
                                        gru.Nombre = nombres;
                                    }
                                    if (tipoIdentificacion.Trim() != "0")
                                    {
                                        gru.Identificacion.IdTipo = tipoIdentificacion;
                                    }
                                    if (numeroIdentificacion.Trim() != string.Empty)
                                    {
                                        gru.Identificacion.Numero = numeroIdentificacion;
                                    }
                                    if (fechaInvalidez.Trim() != string.Empty)
                                    {
                                        gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
                                    }

                                    //Apoderado
                                    gru.IdentificacionApdo = new Identificacion();

                                    if (apellidoPaternoApdo.Trim() != string.Empty)
                                    {
                                        gru.ApellidoPaternoApdo = apellidoPaternoApdo;
                                    }
                                    if (apellidoMaternoApdo.Trim() != string.Empty)
                                    {
                                        gru.ApellidoMaternoApdo = apellidoMaternoApdo;
                                    }
                                    if (nombresApdo.Trim() != string.Empty)
                                    {
                                        gru.NombresApdo = nombresApdo;
                                    }
                                    if (tipoIdentificacionApdo.Trim() != "0")
                                    {
                                        gru.IdentificacionApdo.IdTipo = tipoIdentificacionApdo;
                                    }
                                    if (numeroIdentificacionApdo.Trim() != string.Empty)
                                    {
                                        gru.IdentificacionApdo.Numero = numeroIdentificacionApdo;
                                    }
                                    if (sexoApdo.Trim() != "0")
                                    {
                                        gru.SexoApdo = Convert.ToChar(sexoApdo);
                                    }
                                    if (fechaNacimientoApdo.Trim() != string.Empty && fechaNacimientoApdo.Trim() != "0")
                                    {
                                        gru.FechaNacimientoApdo = Convert.ToDateTime(fechaNacimientoApdo, new CultureInfo("es-PE"));
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.RegistrarGrupoFamiliar(gru);

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

                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                        Detalle = string.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "RegistrarGrupoFamiliar", Environment.NewLine, Environment.NewLine, "Grupo Familiar", JsonConvert.SerializeObject(gru)),
                                        IdTipoEvento = Enums.EventoLog.RegistrarDatosBeneficiario.StringValue()
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
                    Respuesta respuesta = new Respuesta
                    {
                        Estado = Constante.COD_ERROR,
                        Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                        Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                        Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message })
                    };
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
                                                        string fechaInvalidez,
                                                        string flagRenta,
                                                        string apellidoPaternoApdo,
                                                        string apellidoMaternoApdo,
                                                        string nombresApdo,
                                                        string tipoIdentificacionApdo,
                                                        string numeroIdentificacionApdo,
                                                        string sexoApdo,
                                                        string fechaNacimientoApdo,
                                                        bool indTieneApoderado
        )
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
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
                            {
                                // Validar la cartera de agente
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
                                        flagRenta = flagRenta,
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] },
                                        IndTieneApoderado = indTieneApoderado
                                    };
                                    if (apellidoPaterno.Trim() != string.Empty)
                                    {
                                        gru.ApellidoPaterno = apellidoPaterno;
                                    }
                                    if (apellidoMaterno.Trim() != string.Empty)
                                    {
                                        gru.ApellidoMaterno = apellidoMaterno;
                                    }
                                    if (nombres.Trim() != string.Empty)
                                    {
                                        gru.Nombre = nombres;
                                    }
                                    if (tipoIdentificacion.Trim() != "0")
                                    {
                                        gru.Identificacion.IdTipo = tipoIdentificacion;
                                    }
                                    if (numeroIdentificacion.Trim() != string.Empty)
                                    {
                                        gru.Identificacion.Numero = numeroIdentificacion;
                                    }
                                    if (fechaInvalidez.Trim() != string.Empty)
                                    {
                                        gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
                                    }

                                    //Apoderado
                                    gru.IdentificacionApdo = new Identificacion();

                                    if (apellidoPaternoApdo.Trim() != string.Empty)
                                    {
                                        gru.ApellidoPaternoApdo = apellidoPaternoApdo;
                                    }
                                    if (apellidoMaternoApdo.Trim() != string.Empty)
                                    {
                                        gru.ApellidoMaternoApdo = apellidoMaternoApdo;
                                    }
                                    if (nombresApdo.Trim() != string.Empty)
                                    {
                                        gru.NombresApdo = nombresApdo;
                                    }
                                    if (tipoIdentificacionApdo.Trim() != "0")
                                    {
                                        gru.IdentificacionApdo.IdTipo = tipoIdentificacionApdo;
                                    }
                                    if (numeroIdentificacionApdo.Trim() != string.Empty)
                                    {
                                        gru.IdentificacionApdo.Numero = numeroIdentificacionApdo;
                                    }
                                    if (sexoApdo.Trim() != "0")
                                    {
                                        gru.SexoApdo = Convert.ToChar(sexoApdo);
                                    }
                                    if (fechaNacimientoApdo.Trim() != string.Empty && fechaNacimientoApdo.Trim() != "0")
                                    {
                                        gru.FechaNacimientoApdo = Convert.ToDateTime(fechaNacimientoApdo, new CultureInfo("es-PE"));
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.ActualizarGrupoFamiliar(gru);

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

                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                        Detalle = string.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "ActualizarGrupoFamiliar", Environment.NewLine, Environment.NewLine, "Grupo Familiar", JsonConvert.SerializeObject(gru)),
                                        IdTipoEvento = Enums.EventoLog.ModificarDatosBeneficiario.StringValue()
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
        public static Solicitud CrearDatosSolicitud()
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Solicitud sol = new Solicitud();

                sol.FechaSolicitud = DateTime.Now;
                sol.Cotizaciones = new List<Cotizacion>();

                Cotizacion cot;

                SeccionCotizaciones config = (SeccionCotizaciones)ConfigurationManager.GetSection("cotizaciones");
                foreach (ElementoCotizacion cotizacion in config.Cotizaciones)
                {
                    cot = new Cotizacion
                    {
                        Moneda = new Moneda { Id = cotizacion.Moneda },
                        Producto = new Producto { Id = cotizacion.Producto },
                        Modalidad = new Modalidad { Id = cotizacion.Modalidad },
                        PeriodoDiferido = Convert.ToInt32(cotizacion.PeriodoDiferido),
                        PorcentajeEntreRentas = Convert.ToInt32(cotizacion.PjeEntreRentras),
                        PeriodoGarantizado = Convert.ToInt32(cotizacion.PeriodoGarantizado),
                        DerechoCrecer = false,
                        Gratificacion = (cotizacion.Gratificacion == "S") ? true : false,
                        Capital = new Capital { Id = cotizacion.Capital },
                        //<SRIINI12770>
                        AjusteTRA = 0
                        //<SRIFIN12770>
                    };
                    sol.Cotizaciones.Add(cot);
                }

                //<INIGTI_4022>
                if (HttpContext.Current.Session["Nivel"] != null)
                    sol.Agente = new Agente { IdNivel = Convert.ToInt32(HttpContext.Current.Session["Nivel"]) };

                //<FINGTI_4022>

                return sol;
            }
        }

        [WebMethod]
        public static Solicitud ObtenerDatosSolicitud(string idSolicitud, string fecCotizacion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                fecCotizacion = fechaCotizacion.ToString("yyyyMMdd");

                Solicitud sol;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                sol = servicioCotizador.ObtenerDatosSolicitud(idSolicitud, fechaCotizacion);
                return sol;
            }
        }

        [WebMethod]
        public static List<Cotizacion> AgregarCotizacionASolicitud(List<Cotizacion> cot)
        {
            Cotizacion cotizacion = new Cotizacion
            {
                Moneda = new Moneda { Id = "0" },
                Producto = new Producto { Id = "0" },
                Modalidad = new Modalidad { Id = "0" },
                PeriodoDiferido = 0,
                PorcentajeEntreRentas = 0,
                PeriodoGarantizado = 0,
                DerechoCrecer = false,
                Gratificacion = false,
                Capital = new Capital { Id = "-" },
                //<SRIINI12770>
                AjusteTRA = 0
                //<SRIFIN12770>
            };
            cot.Add(cotizacion);
            return cot;
        }

        [WebMethod]
        public static Solicitud InsertarSolicitud(string tokenUsuario,
                                                  string cuspp,
                                                  string afp,
                                                  string tipoCambio,
                                                  string tipoPension,
                                                  string categoria,
                                                  string fechaDevengue,
                                                  string fechaRecepcion,
                                                  string fechaPlazoAFP,
                                                  string saldoCIC,
                                                  string factorTasa,
                                                  string fechaCotizacion,
                                                  string fechaSolicitudPension,
                                                  string acom,
                                                  string dcom,
                                                  List<Cotizacion> cotizaciones,
                                                  List<int> idBeneficiarios,
                                                  string correo
        )
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Solicitud sol;
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudInsertar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();

                            if (ValidarSolicitud(errores, controles, tipoCambio, tipoPension, categoria, fechaDevengue, DateTime.Now.ToString("dd/MM/yyyy"), fechaRecepcion, fechaPlazoAFP, saldoCIC, factorTasa, fechaCotizacion, fechaSolicitudPension, acom, dcom, cotizaciones, idBeneficiarios, cuspp, correo, (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                // Validar la cartera de agente
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(string.Empty, cuspp, string.Empty, string.Empty, string.Empty);
                                    sol = new Solicitud
                                    {
                                        Afiliado = new Afiliado { CUSPP = cuspp, AFP = new AFP { Id = afp } },
                                        TipoCambio = Convert.ToDouble(tipoCambio, new CultureInfo("es-PE")),
                                        TipoPension = new TipoPension { Id = tipoPension },
                                        Categoria = new Categoria { Id = categoria },
                                        FechaDevengue = Convert.ToDateTime(fechaDevengue, new CultureInfo("es-PE")),
                                        FechaSolicitud = DateTime.Now,
                                        FechaUltimaActualizacion = DateTime.Now,
                                        FechaRecepcion = Convert.ToDateTime(fechaRecepcion, new CultureInfo("es-PE")),
                                        FechaPlazoAFP = Convert.ToDateTime(fechaPlazoAFP, new CultureInfo("es-PE")),
                                        FechaSolicitudPension = Convert.ToDateTime(fechaSolicitudPension, new CultureInfo("es-PE")),
                                        SaldoCIC = Convert.ToDouble(saldoCIC, new CultureInfo("es-PE")),
                                        FactorTasa = factorTasa,
                                        FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"], Rol = (string)HttpContext.Current.Session["RolAzman"] },
                                        Agente = new Agente { Id = afiliado.Agente.Id, IdCartera = afiliado.Agente.IdCartera },
                                        TipoCotizacion = new TipoCotizacion { Id = Enums.TipoCotizacion.Extraoficial.StringValue() },
                                        Cotizaciones = cotizaciones
                                    };

                                    // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                                    {
                                        sol.PorcentajeAumentoComision = Convert.ToDouble(acom, new CultureInfo("es-PE"));
                                        sol.PorcentajeDescuentoComision = Convert.ToDouble(dcom, new CultureInfo("es-PE"));
                                    }
                                    else
                                    {
                                        sol.PorcentajeAumentoComision = null;
                                        sol.PorcentajeDescuentoComision = null;
                                    }

                                    List<GrupoFamiliar> lben = new List<GrupoFamiliar>();
                                    idBeneficiarios.ForEach(id => lben.Add(((List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"])[id]));
                                    sol.Beneficiarios = lben;

                                    respuesta = servicioCotizador.RegistrarSolicitud(ref sol);

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
                                    int correlativo = 1;
                                    foreach (var itemCot in sol.Cotizaciones)
                                    {
                                        tra += "[" + correlativo + ": " + itemCot.AjusteTRA + "] ";
                                        correlativo += 1;
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                        IdTipoEvento = Enums.EventoLog.CotizarSolicitud.StringValue(),
                                        Detalle = "Solicitud registrada: " + sol.Id + ", ACOM: " + sol.PorcentajeAumentoComision + ", DCOM: " + sol.PorcentajeDescuentoComision + ", DTRA: " + tra
                                    });
                                }
                                else
                                {
                                    sol = new Solicitud();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                sol = new Solicitud();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            sol = new Solicitud();
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudInsertar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        sol = new Solicitud();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    sol.Respuesta = respuesta;
                    return sol;
                }
                catch (Exception ex)
                {
                    sol = new Solicitud
                    {
                        Respuesta = new Respuesta
                        {
                            Estado = Constante.COD_ERROR,
                            Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                            Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                            Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message })
                        }
                    };
                    return sol;
                }
            }
        }

        [WebMethod]
        public static Solicitud ModificarSolicitud(string tokenUsuario,
                                                   string idSolicitud,
                                                   string cuspp,
                                                   string afp,
                                                   string tipoCambio,
                                                   string tipoPension,
                                                   string categoria,
                                                   string fechaDevengue,
                                                   string fechaRecepcion,
                                                   string fechaPlazoAFP,
                                                   string saldoCIC,
                                                   string factorTasa,
                                                   string fechaCotizacion,
                                                   string fechaSolicitudPension,
                                                   string acom,
                                                   string dcom,
                                                   List<Cotizacion> cotizaciones,
                                                   List<int> idBeneficiarios,
                                                   string correo,
                                                   string numAgenteSol
        )
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Solicitud sol;
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudActualizar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();

                            if (ValidarSolicitud(errores, controles, tipoCambio, tipoPension, categoria, fechaDevengue, DateTime.Now.ToString("dd/MM/yyyy"), fechaRecepcion, fechaPlazoAFP, saldoCIC, factorTasa, fechaCotizacion, fechaSolicitudPension, acom, dcom, cotizaciones, idBeneficiarios, cuspp, correo, numAgenteSol))
                            {
                                // Validar la cartera de agente
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(string.Empty, cuspp, string.Empty, string.Empty, string.Empty);
                                    sol = new Solicitud
                                    {
                                        Id = idSolicitud,
                                        Afiliado = new Afiliado { CUSPP = cuspp, AFP = new AFP { Id = afp } },
                                        TipoCambio = Convert.ToDouble(tipoCambio, new CultureInfo("es-PE")),
                                        TipoPension = new TipoPension { Id = tipoPension },
                                        Categoria = new Categoria { Id = categoria },
                                        FechaDevengue = Convert.ToDateTime(fechaDevengue, new CultureInfo("es-PE")),
                                        FechaSolicitud = DateTime.Now,
                                        FechaUltimaActualizacion = DateTime.Now,//<INIGTI_754>
                                        FechaRecepcion = Convert.ToDateTime(fechaRecepcion, new CultureInfo("es-PE")),
                                        FechaPlazoAFP = Convert.ToDateTime(fechaPlazoAFP, new CultureInfo("es-PE")),
                                        SaldoCIC = Convert.ToDouble(saldoCIC, new CultureInfo("es-PE")),
                                        FactorTasa = factorTasa,
                                        FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                        FechaSolicitudPension = Convert.ToDateTime(fechaSolicitudPension, new CultureInfo("es-PE")),
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"], Rol = (string)HttpContext.Current.Session["RolAzman"] },
                                        Agente = new Agente { Id = afiliado.Agente.Id, IdCartera = afiliado.Agente.IdCartera },
                                        TipoCotizacion = new TipoCotizacion { Id = Enums.TipoCotizacion.Extraoficial.StringValue() },
                                        Cotizaciones = cotizaciones
                                    };

                                    // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                                    {
                                        sol.PorcentajeAumentoComision = Convert.ToDouble(acom, new CultureInfo("es-PE"));
                                        sol.PorcentajeDescuentoComision = Convert.ToDouble(dcom, new CultureInfo("es-PE"));
                                    }
                                    else
                                    {
                                        sol.PorcentajeAumentoComision = null;
                                        sol.PorcentajeDescuentoComision = null;
                                    }

                                    List<GrupoFamiliar> lben = new List<GrupoFamiliar>();
                                    idBeneficiarios.ForEach(id => lben.Add(((List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"])[id]));
                                    sol.Beneficiarios = lben;

                                    respuesta = servicioCotizador.ActualizarSolicitud(ref sol);

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
                                    foreach (var itemCot in sol.Cotizaciones)
                                    {
                                        tra += "[" + itemCot.Correlativo + ": " + itemCot.AjusteTRA + "] ";
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                        IdTipoEvento = Enums.EventoLog.CotizarSolicitud.StringValue(),
                                        Detalle = "Solicitud modificada: " + sol.Id + ", ACOM: " + sol.PorcentajeAumentoComision + ", DCOM: " + sol.PorcentajeDescuentoComision + ", DTRA: " + tra
                                    });
                                }
                                else
                                {
                                    sol = new Solicitud();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                sol = new Solicitud();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            sol = new Solicitud();
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudActualizar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        sol = new Solicitud();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    sol.Respuesta = respuesta;
                    return sol;
                }
                catch (Exception ex)
                {
                    sol = new Solicitud();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                    sol.Respuesta = respuesta;
                    return sol;
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

                    respuesta.Estado = Constante.COD_OK;
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
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudEnviarCorreo))
                        {
                            // Validar la cartera de agente
                            if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                                fecCotizacion = fechaCotizacion.ToString("yyyyMMdd");
                                string num_lote = "1";
                                string grupo1 = "GRUPO1";
                                string grupo2 = "GRUPO2";
                                string id_benefi = "1";

                                ReportViewer visorReporte = new ReportViewer();
                                visorReporte.ProcessingMode = ProcessingMode.Remote;
                                visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteDetalleCotizacion"];

                                ReportParameter p1 = new ReportParameter("wl_solicitud", idSolicitud);
                                ReportParameter p2 = new ReportParameter("wl_fec_cotizacion", fecCotizacion);
                                ReportParameter p3 = new ReportParameter("wl_num_lote", num_lote);
                                ReportParameter p4 = new ReportParameter("wl_grupo", grupo1);
                                ReportParameter p5 = new ReportParameter("wl_grupo2", grupo2);
                                ReportParameter p6 = new ReportParameter("wl_id_benefi", id_benefi);

                                log.Info(string.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteDetalleCotizacion"]));
                                log.Debug(string.Format("Parámetros del reporte: wl_solicitud[{0}] wl_fec_cotizacion[{1}] wl_num_lote[{2}] wl_grupo1[{3}] wl_grupo2[{4}] wl_id_benefi[{5}].",
                                    idSolicitud, fecCotizacion, num_lote, grupo1, grupo2, id_benefi));
                                visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6 });
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
                                    Adjunto = "Cotizacion.pdf (" + Utilitarios.FormatearBytes(bytes.Length, false) + ")",
                                    Mensaje = config.Mensaje.Texto
                                                    .Replace("{TratamientoAfiliado}", (sexo == "M") ? "Sr." : "Sra.")
                                                    .Replace("{ApellidoPaternoAfiliado}", apellidoPaterno)
                                                    .Replace("{ApellidoMaternoAfiliado}", apellidoMaterno)
                                                    .Replace("{NombreAfiliado}", nombre)
                                                    .Replace("{TipoCotizacion}", tipoCotizacion)
                                                    .Replace("{NombreAgente}", (string)HttpContext.Current.Session["NombreCompleto"]),
                                    BinarioAdjunto = bytes
                                };
                                correo.Respuesta = new Respuesta
                                {
                                    Estado = Constante.COD_OK
                                };
                            }
                            else
                            {
                                correo = new CorreoElectronico
                                {
                                    Respuesta = new Respuesta
                                    {
                                        Estado = Constante.COD_ERROR,
                                        Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                                        Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                                        Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." })
                                    }
                                };
                            }
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DireccionActualizar.StringValue()));
                            correo = new CorreoElectronico
                            {
                                Respuesta = new Respuesta
                                {
                                    Estado = Constante.COD_ERROR,
                                    Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                                    Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                                    Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] })
                                }
                            };
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        correo = new CorreoElectronico
                        {
                            Respuesta = new Respuesta
                            {
                                Estado = Constante.COD_TOKEN
                            }
                        };
                    }
                    return correo;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    correo = new CorreoElectronico
                    {
                        Respuesta = new Respuesta
                        {
                            Estado = Constante.COD_ERROR,
                            Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                            Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                            Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message })
                        }
                    };
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
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudEnviarCorreo))
                        {
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
                            correo.Respuesta = new Respuesta
                            {
                                Estado = Constante.COD_ERROR,
                                Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                                Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                                Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] })
                            };
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
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                }
                return respuesta;
            }
        }

        //<GTIINI754>
        [WebMethod]
        public static void PrecargarParametros()
        {
            Thread.Sleep(3000);
            return;
        }
        //<GTIFIN754>

        [WebMethod]
        public static Actividad ObtenerDatosActividad(string cuspp, string idActividad)
        {
            return ((List<Actividad>)HttpContext.Current.Session["Actividades"]).Find(x => x.Correlativo == idActividad);
        }

        [WebMethod]
        public static Afiliado ObtenerDatosAfiliado(string tokenUsuario, string nroSolicitud, string cuspp)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Afiliado afiliado = null;
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarBusquedaAfiliados(errores, controles, nroSolicitud, cuspp))
                            {
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                afiliado = servicioCotizador.ObtenerDatosAfiliado(nroSolicitud, cuspp, "", "", Enums.TipoProducto.RVI.StringValue());

                                log.Info("Usuario realizó búsqueda de afiliados por "
                                    + ((nroSolicitud.Trim().Length != 0)
                                    ? ("Solicitud [" + nroSolicitud.ToUpper() + "]")
                                    : ("CUSPP [" + cuspp.ToUpper() + "]")) + ".");

                                if (afiliado != null)
                                {
                                    // Validar la cartera de agente
                                    if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == afiliado.Agente.Id))
                                    {
                                        if (cuspp.Trim().Length > 0)
                                        {
                                            HttpContext.Current.Session["CUSPP"] = cuspp;
                                            HttpContext.Current.Session["NroSolicitud"] = null;
                                        }
                                        else if (nroSolicitud.Trim().Length > 0)
                                        {
                                            HttpContext.Current.Session["CUSPP"] = null;
                                            HttpContext.Current.Session["NroSolicitud"] = nroSolicitud;
                                        }

                                        HttpContext.Current.Session["Consentimiento"] = afiliado.Consentimiento;

                                        afiliado.Respuesta = new Respuesta();
                                        afiliado.Respuesta.Estado = Constante.COD_OK;
                                    }
                                    else
                                    {
                                        errores.Add("Cliente no pertenece a su cartera de ventas. Verifique.");
                                        afiliado.Respuesta = new Respuesta
                                        {
                                            Estado = Constante.COD_ERROR,
                                            Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                                            Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                                            Mensaje = Utilitarios.FormatearError(errores)
                                        };
                                    }
                                }
                                else
                                {
                                    if (nroSolicitud.Trim().Length > 0)
                                    {
                                        errores.Add("Solicitud N° <strong>" + nroSolicitud.ToUpper() + "</strong> no se encuentra registrada. Verifique.");
                                    }
                                    else
                                    {
                                        errores.Add("CUSPP <strong>" + cuspp.ToUpper() + "</strong> no se encuentra registrado. Verifique.");
                                    }
                                    afiliado = new Afiliado
                                    {
                                        Respuesta = new Respuesta
                                        {
                                            Estado = Constante.COD_ERROR,
                                            Titulo = Enums.CuadroMensajeTitulo.Informacion.StringValue(),
                                            Icono = Enums.CuadroMensajeIcono.Informacion.StringValue(),
                                            Mensaje = Utilitarios.FormatearError(errores)
                                        }
                                    };
                                }
                            }
                            else
                            {
                                afiliado = new Afiliado
                                {
                                    Respuesta = new Respuesta
                                    {
                                        Estado = Constante.COD_ERROR,
                                        Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue(),
                                        Icono = Enums.CuadroMensajeIcono.Validacion.StringValue(),
                                        Mensaje = Utilitarios.FormatearError(errores),
                                        Controles = controles
                                    }
                                };
                            }
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DatosAfiliadoConsultar.StringValue()));
                            afiliado = new Afiliado
                            {
                                Respuesta = new Respuesta
                                {
                                    Estado = Constante.COD_ERROR,
                                    Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                                    Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                                    Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] })
                                }
                            };
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        afiliado = new Afiliado
                        {
                            Respuesta = new Respuesta
                            {
                                Estado = Constante.COD_TOKEN
                            }
                        };
                    }
                }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    afiliado = new Afiliado
                    {
                        Respuesta = new Respuesta
                        {
                            Estado = Constante.COD_ERROR,
                            Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                            Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                            Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] })
                        }
                    };
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    afiliado = new Afiliado
                    {
                        Respuesta = new Respuesta
                        {
                            Estado = Constante.COD_ERROR,
                            Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                            Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                            Mensaje = Utilitarios.FormatearError(new List<string> { ex.Message })
                        }
                    };
                }
                return afiliado;
            }
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
                            // Validar cartera del agente
                            if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.GenerarReporteEscenarios(idSolicitud, fechaCotizacion, (string)HttpContext.Current.Session["Usuario"], maxAcom);

                                string nombreTerminal = string.Empty;

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

        protected void BusAfiBuscar_Click(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
                    {
                        if (ValidarBusquedaAfiliados())
                        {
                            LimpiarFormularios();

                            CargarComboboxNuevosDatos(ModDirDepartamento, "Departamento", "");

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(BusAfiNroSolicitud.Text, BusAfiCUSPP.Text, "", "", Enums.TipoProducto.RVI.StringValue());

                            log.Info("Usuario realizó búsqueda de afiliados por "
                                + ((BusAfiNroSolicitud.Text.Trim().Length != 0)
                                ? ("Solicitud [" + BusAfiNroSolicitud.Text.ToUpper() + "]")
                                : ("CUSPP [" + BusAfiCUSPP.Text.ToUpper() + "]")) + ".");

                            if (afiliado != null)
                            {
                                // MasterPage
                                Panel cabecera, cabeceraProtegida;

                                cabecera = (Panel)Master.FindControl("CabeceraSuperior");
                                cabeceraProtegida = (Panel)Master.FindControl("CabeceraSuperiorProtegida");

                                Session["Consentimiento"] = afiliado.Consentimiento;

                                var ApellidoPaternoInicial = afiliado.ApellidoPaterno;
                                var ApellidoMaternoInicial = afiliado.ApellidoMaterno;
                                var NombreInicial = afiliado.Nombre;

                                if (!afiliado.Consentimiento)
                                {
                                    // MastePage
                                    cabecera.Visible = false;
                                    cabeceraProtegida.Visible = true;

                                    // Datos del Afiliado
                                    BusquedaAfiliados.Visible = false;
                                    LineaCUSPP.Visible = false;
                                    afiliado.ApellidoPaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoPaterno);
                                    afiliado.ApellidoMaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoMaterno);
                                    afiliado.Nombre = Utilitarios.EnmascararNombre(afiliado.Nombre);
                                    LineaNacimientoSexo.Visible = false;
                                    LineaCorreoElectronico.Visible = false;
                                    LineaCategoria.Visible = false;
                                    LineaSaldoCIC.Visible = false;
                                    LineaAFP.Visible = false;
                                    GrupoDireccion.Visible = false;

                                    ContenedorGuardar.Visible = false;

                                    // Grupo Familiar
                                    ModGruFamLineaApellidos.Visible = false;
                                    ModGruFamLineaNombres.Visible = false;
                                    ModGruFamLineaIdentificacion.Visible = false;
                                    ModGruFamCargando.Height = 130;

                                    // Solicitudes
                                    ModSolSaldoCIC.Visible = false;
                                    ModSolListaCIC.Visible = true;
                                }
                                else
                                {
                                    // MastePage
                                    cabecera.Visible = true;
                                    cabeceraProtegida.Visible = false;

                                    // Datos del Afiliado
                                    LineaSaldoCIC.Visible = true;

                                    // Solicitudes
                                    ModSolSaldoCIC.Visible = true;
                                    ModSolListaCIC.Visible = false;
                                }

                                // Si el agente está cesado no mostrar el botón de Nueva Solicitud
                                OpcionSistema opcionInsertarSolicitud = ((List<OpcionSistema>)Session["OpcionesSistema"]).Find(o => o.IdAzman == (int)Enums.OpcionesSistema.SolicitudInsertar);
                                if (opcionInsertarSolicitud.Activa)
                                {
                                    if (!Utilitario.PerteneceACartera(afiliado.Agente.Id, afiliado.CUSPP, (List<Agente>)Session["ListaAgentes"], (string)Session["RolAzman"], (string)Session["Usuario"], false))
                                    {
                                        InhabilitarControl(NuevaSolicitud);
                                        PerNuevaSolicitud.Value = "0";
                                    }
                                }

                                // Validar la cartera del agente
                                if (Utilitario.PerteneceACartera(afiliado.Agente.Id, afiliado.CUSPP, (List<Agente>)Session["ListaAgentes"], (string)Session["RolAzman"], (string)Session["Usuario"], true))
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
                                    CUSPP.Text = afiliado.CUSPP.Trim();
                                    HCUSPP.Value = afiliado.CUSPP.Trim();
                                    ApellidoPaterno.Text = afiliado.ApellidoPaterno.Trim();
                                    ApellidoMaterno.Text = afiliado.ApellidoMaterno.Trim();
                                    Nombres.Text = afiliado.Nombre.Trim();
                                    FechaNacimiento.Text = afiliado.FechaNacimiento.Value.ToString("dd/MM/yyyy");
                                    Sexo.SelectedIndex = Sexo.Items.IndexOf(Sexo.Items.FindByValue(afiliado.Sexo.ToString()));
                                    CorreoElectronico.Text = afiliado.CorreoElectronico;
                                    CorreoElectronicoRegistrado.Value = afiliado.CorreoElectronico;
                                    Categoria.SelectedIndex = Categoria.Items.IndexOf(Categoria.Items.FindByValue(afiliado.Categoria.Id.ToString()));
                                    HCategoria.Value = afiliado.Categoria.Id.ToString();
                                    HAFP.Value = afiliado.AFP.Id.ToString();
                                    AFP.SelectedIndex = AFP.Items.IndexOf(AFP.Items.FindByValue(afiliado.AFP.Id.ToString()));
                                    SaldoCIC.Text = afiliado.SaldoCIC.ToString();

                                    // Datos del agente
                                    Agente agente = ((List<Agente>)Session["ListaAgentes"]).Find(a => a.Id == afiliado.Agente.Id);
                                    if (agente == null && Utilitarios.EsRolVerAgentesCesados((string)Session["RolAzman"]))
                                    {
                                        agente = servicioCotizador.ObtenerUltimoAgentePorCartera(afiliado.Agente.IdCartera, (string)Session["Usuario"]);
                                    }
                                    NumeroAgente.Value = agente.Id;
                                    Cartera.Value = afiliado.Agente.IdCartera;
                                    NombreAgente.Value = agente.Nombre;
                                    Agente.Text = string.Format("{0} - {1}", agente.Id, agente.Nombre);
                                    Session["Vendedor"] = agente.Id;
                                    Session["Nivel"] = agente.IdNivel;

                                    // Direcciones
                                    NuevaDireccion.Visible = true;

                                    // Botón Guardar Afiliado
                                    if (afiliado.Consentimiento)
                                        ContenedorGuardar.Visible = true;
                                    else
                                        ContenedorGuardar.Visible = false;

                                    // Grupo Familiar
                                    NuevoBeneficiario.Visible = true;

                                    // Solicitudes
                                    NuevaSolicitud.Visible = true;

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

                                    ModSolTipoPension.SelectedIndex = ModSolTipoPension.Items.IndexOf(ModSolTipoPension.Items.FindByValue("V"));

                                    HttpContext.Current.Session["indConsentimiento"] = afiliado.Consentimiento;  //JY

                                    TipoDocumento.SelectedIndex = TipoDocumento.Items.IndexOf(TipoDocumento.Items.FindByValue(afiliado.TipoIdentificacion));
                                    NumeroDocumento.Text = afiliado.NumeroIdentificacion;

                                    Telefono.Text = afiliado.Telefonos;
                                    Celular.Text = afiliado.Celulares;

                                    //consentimiento de asesoria
                                    obtenerConsentimiento(afiliado);

                                    //Guardar los datos del afiliado para generarlo como beneficiario en caso de que no exista
                                    afiliado.ApellidoPaterno = ApellidoPaternoInicial;
                                    afiliado.ApellidoMaterno = ApellidoMaternoInicial;
                                    afiliado.Nombre = NombreInicial;
                                    servicioCotizador.ActualizarAfiliado(afiliado);

                                    pnlControlAporte.Visible = true;
                                    AporteAdicional aporte = servicioCotizador.ObtenerDatosAporteAdicional(afiliado.CUSPP.Trim());

                                    if (aporte != null)
                                    {
                                        txtPensionRef.Text = aporte.val_pension_referencia.ToString();
                                        ddlMonedaReferencia.SelectedValue = aporte.cod_moneda_pension_ref;
                                        txtTasaAporte.Text = aporte.val_tasa_aporte.ToString();
                                        dtpFechaPago.Text = aporte.fec_pagoapad.ToString("dd/MM/yyyy");
                                        txtMontoAporte.Text = aporte.val_monto_aporte.ToString();
                                        txtPensionPago.Text = aporte.val_pension_a_pago.ToString();
                                        ddlMonedaPensionPago.SelectedValue = aporte.cod_moneda_pension_a_pago;
                                        txtPensionElegida.Text = aporte.val_pension_elegida.ToString();
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
                                List<string> errores = new List<string>();
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
            CUSPP.Text = String.Empty;
            HCUSPP.Value = String.Empty;
            ApellidoPaterno.Text = String.Empty;
            ApellidoMaterno.Text = String.Empty;
            Nombres.Text = String.Empty;
            FechaNacimiento.Text = String.Empty;
            Sexo.SelectedIndex = Sexo.Items.IndexOf(Sexo.Items.FindByValue("0"));
            CorreoElectronico.Text = String.Empty;
            CorreoElectronicoRegistrado.Value = String.Empty;
            Categoria.SelectedIndex = Categoria.Items.IndexOf(Categoria.Items.FindByValue("0"));
            HCategoria.Value = "0";
            AFP.SelectedIndex = AFP.Items.IndexOf(AFP.Items.FindByValue("0"));
            HAFP.Value = "0";
            SaldoCIC.Text = string.Empty;
            Session["Vendedor"] = null;
            HToken.Value = string.Empty;

            CorreoElectronico.CssClass = CorreoElectronico.CssClass.Replace(" formTextboxError", String.Empty);
            Categoria.CssClass = Categoria.CssClass.Replace(" formComboboxError", String.Empty);
            AFP.CssClass = AFP.CssClass.Replace(" formComboboxError", String.Empty);
            SaldoCIC.CssClass = SaldoCIC.CssClass.Replace(" formTextboxError", String.Empty);

            // Direcciones
            NuevaDireccion.Visible = false;

            // Teléfonos
            //oculto
            //NuevoTelefono.Visible = false;

            // Datos de empresa
            //oculto
            //NombreEmpresa.Text = String.Empty;
            //DireccionEmpresa.Text = String.Empty;
            //CiudadEmpresa.SelectedIndex = CiudadEmpresa.Items.IndexOf(CiudadEmpresa.Items.FindByValue("0"));
            //ComunaEmpresa.SelectedIndex = ComunaEmpresa.Items.IndexOf(ComunaEmpresa.Items.FindByValue("0"));
            //TelefonoEmpresa.Text = String.Empty;

            // Botón Guardar Afiliado
            ContenedorGuardar.Visible = false;

            // Grupo Familiar
            NuevoBeneficiario.Visible = false;

            // Solicitudes
            NuevaSolicitud.Visible = false;
            ModEnvCorDe.Text = String.Empty;
            ModEnvCorPara.Text = String.Empty;
            ModEnvCorAsunto.Text = String.Empty;

            TipoDocumento.SelectedIndex = 0;
            NumeroDocumento.Text = string.Empty;

            Telefono.Text = string.Empty;
            Celular.Text = string.Empty;

            //<GTI.INI-29372>
            txtPensionRef.Text = string.Empty;
            ddlMonedaReferencia.SelectedIndex = 0;
            txtTasaAporte.Text = string.Empty;
            dtpFechaPago.Text = string.Empty;
            txtMontoAporte.Text = string.Empty;
            txtPensionPago.Text = string.Empty;
            ddlMonedaPensionPago.SelectedIndex = 0;
            txtPensionElegida.Text = string.Empty;

            pnlControlAporte.Visible = false;
            //<GTI.FIN-29372>
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

        private static bool ValidarGrupoFamiliar(List<string> errores, List<string> controles, string glsApellidoPaterno, string glsApellidoMaterno, string glsNombres, string idTipoIdentificacion, string glsNumeroIdentificacion, string idParentesco, string idSexo, string fecNacimiento, string idInvalidez, string idTipoInvalidez, string fecInvalidez)
        {
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
                //<SRIINI17003>
                else
                {
                    if (vFechaNacimiento > DateTime.Now)
                    {
                        errores.Add("La <strong>Fecha de Nacimiento</strong> no puede ser mayor al día de hoy.");
                        fechaNacimiento = false;
                    }
                }
                //<SRIFIN17003>
            }

            // Invalidez
            bool invalidez = true;
            bool tipoInvalidez = true;
            bool fechaInvalidez = true;
            if (idInvalidez == "0")
            {
                errores.Add("Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.");
                invalidez = false;
            }
            else if (idInvalidez == Enums.Invalidez.No.StringValue())
            {
                if (idTipoInvalidez != Enums.TipoInvalidez.NoInvalido.StringValue())
                {
                    errores.Add("El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.");
                    tipoInvalidez = false;
                }
                if (fecInvalidez.Trim().Length > 0)
                {
                    errores.Add("El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.");
                    fechaInvalidez = false;
                }
            }
            else if (idInvalidez == Enums.Invalidez.Si.StringValue())
            {
                if (idTipoInvalidez != Enums.TipoInvalidez.Parcial.StringValue() && idTipoInvalidez != Enums.TipoInvalidez.Total.StringValue())
                {
                    errores.Add("El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.");
                    tipoInvalidez = false;
                }

                if (fecInvalidez.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.");
                    fechaInvalidez = false;
                }
                else
                {
                    DateTime vFechaInvalidez;
                    if (!DateTime.TryParse(fecInvalidez, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaInvalidez))
                    {
                        errores.Add("El campo <strong>Fecha de Invalidez</strong> debe contener una fecha válida (dd/mm/aaaa).");
                        fechaInvalidez = false;
                    }
                }
            }

            if (idTipoInvalidez == "0")
            {
                errores.Add("Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.");
                tipoInvalidez = false;
            }

            // Clases de controles
            if (!apellidoPaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!apellidoMaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!nombres) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!tipoIdentificacion) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!numeroIdentificacion) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!parentesco) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!sexo) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!fechaNacimiento) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!invalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!tipoInvalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!fechaInvalidez) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento & invalidez & tipoInvalidez & fechaInvalidez;

            return esCorrecto;
        }

        //<SRI.INI-20322_E2>
        //private static bool ValidarSolicitud(List<string> errores, List<string> controles, string valTipoCambio, string idTipoPension, string idCategoria, string fecDevengue, string fecUltimaActualizacion, string fecRecepcion, string fecPlazoAFP, string valSaldoCIC, string idFactorTasa, string fecCotizacion, string fecSolicitudPension, string valAcom, string valDcom, List<Cotizacion> listaCotizaciones, List<int> idBeneficiarios)
        private static bool ValidarSolicitud(List<string> errores, List<string> controles, string valTipoCambio, string idTipoPension, string idCategoria, string fecDevengue, string fecUltimaActualizacion, string fecRecepcion, string fecPlazoAFP, string valSaldoCIC, string idFactorTasa, string fecCotizacion, string fecSolicitudPension, string valAcom, string valDcom, List<Cotizacion> listaCotizaciones, List<int> idBeneficiarios, string cusspp, string correo, string numAgenteSol)
        //<SRI.FIN-20322_E2>
        {
            bool esCorrecto;

            // Tipo de Cambio
            bool tipoCambio = true;
            if (valTipoCambio.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Tipo de Cambio</strong>. Dato Obligatorio.");
                tipoCambio = false;
            }
            else
            {
                double vTipoCambio;
                if (!double.TryParse(valTipoCambio, NumberStyles.Any, new CultureInfo("es-PE"), out vTipoCambio))
                {
                    errores.Add("El campo <strong>Tipo de Cambio</strong> debe contener un valor numérico.");
                    tipoCambio = false;
                }
                else
                {
                    if (vTipoCambio <= 0)
                    {
                        errores.Add("El campo <strong>Tipo de Cambio</strong> debe contener un valor positivo.");
                        tipoCambio = false;
                    }
                }
            }

            // Tipo de Pensión
            bool tipoPension = true;
            if (idTipoPension == "0")
            {
                errores.Add("Ingrese el campo <strong>Tipo de Pensión</strong>. Dato Obligatorio.");
                tipoPension = false;
            }

            // Categoría
            bool categoria = true;
            if (idCategoria == "0")
            {
                errores.Add("Ingrese el campo <strong>Categoría</strong>. Dato Obligatorio.");
                categoria = false;
            }

            // Fecha de Devengue
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

            // Fecha de Última Actualización
            bool fechaUltimaActualizacion = true;
            if (fecUltimaActualizacion.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha Últ. Actualización</strong>. Dato Obligatorio.");
                fechaUltimaActualizacion = false;
            }
            else
            {
                DateTime vFechaUltimaActualizacion;
                if (!DateTime.TryParse(fecUltimaActualizacion, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaUltimaActualizacion))
                {
                    errores.Add("El campo <strong>Fecha Últ. Actualización</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaUltimaActualizacion = false;
                }
            }

            // Fecha de Recepción
            bool fechaRecepcion = true;
            if (fecRecepcion.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Recepción</strong>. Dato Obligatorio.");
                fechaRecepcion = false;
            }
            else
            {
                DateTime vFechaRecepcion;
                if (!DateTime.TryParse(fecRecepcion, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaRecepcion))
                {
                    errores.Add("El campo <strong>Fecha de Recepción</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaRecepcion = false;
                }
            }

            // Fecha de Plazo AFP
            bool fechaPlazoAFP = true;
            DateTime vFechPlazoAFP = new DateTime();
            if (fecPlazoAFP.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Plazo AFP</strong>. Dato Obligatorio.");
                fechaPlazoAFP = false;
            }
            else
            {
                if (!DateTime.TryParse(fecPlazoAFP, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechPlazoAFP))
                {
                    errores.Add("El campo <strong>Fecha de Plazo AFP</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaPlazoAFP = false;
                }
            }

            // Saldo CIC
            bool saldoCIC = true;
            if (valSaldoCIC.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Saldo CIC</strong>. Dato Obligatorio.");
                saldoCIC = false;
            }
            else
            {
                double vSaldoCIC;
                if (!double.TryParse(valSaldoCIC, NumberStyles.Any, new CultureInfo("es-PE"), out vSaldoCIC))
                {
                    errores.Add("El campo <strong>Saldo CIC</strong> debe contener un valor numérico.");
                    saldoCIC = false;
                }
                else
                {
                    if (vSaldoCIC <= 0)
                    {
                        errores.Add("El campo <strong>Saldo CIC</strong> debe contener un valor positivo.");
                        saldoCIC = false;
                    }
                }
            }

            // Factor Tasa
            bool factorTasa = true;
            if (idFactorTasa == "0")
            {
                errores.Add("Ingrese el campo <strong>Factor Tasa</strong>. Dato Obligatorio.");
                factorTasa = false;
            }

            // Fecha de Cotización
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

            // Fecha de Solicitud de Pensión
            bool fechaSolicitudPension = true;
            if (fecSolicitudPension.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Fecha de Sol. Pensión</strong>. Dato Obligatorio.");
                fechaSolicitudPension = false;
            }
            else
            {
                DateTime vFechSolicitudPension;
                if (!DateTime.TryParse(fecSolicitudPension, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechSolicitudPension))
                {
                    errores.Add("El campo <strong>Fecha de Cotización</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaSolicitudPension = false;
                }
            }

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            bool redLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);

            //<SRI.INI-20322_E2>
            bool valRequisitosTra = true;

            //recuperamos sesión de parámetros
            List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];
            //recuperamos lista de parámetros
            string numAgente = numAgenteSol; // (string)HttpContext.Current.Session["NumAgente"];

            //borrar inicio
            //numAgente = "14489";
            //cusspp = "193251DCMAT3";
            //correo = "ext.jfernandez@interseguro.com.pe";
            //borrar fin
            //<SRI.FIN-20322_E2>

            // ACOM
            bool acom = true;
            if (redLocal)
            {
                if (valAcom.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Porcentaje A</strong>. Dato Obligatorio.");
                    acom = false;
                }
                else
                {
                    double vAcom;
                    if (!double.TryParse(valAcom, NumberStyles.Any, new CultureInfo("es-PE"), out vAcom))
                    {
                        errores.Add("El campo <strong>Porcentaje A</strong> debe contener un valor numérico.");
                        acom = false;
                    }
                    else
                    {
                        if (vAcom < 0)
                        {
                            errores.Add("El campo <strong>Porcentaje A</strong> debe contener un valor positivo.");
                            acom = false;
                        }
                        else if (vAcom > 0)
                        {
                            // Validaciones de Precubo y Citas
                            valRequisitosTra = false;

                            if (correo == "" || correo == null)
                            {
                                errores.Add("Debe ingresar un <strong>Correo Electrónico</strong> a la oportunidad para poder realizar la cotización.");
                                acom = false;
                            }

                            if (string.IsNullOrEmpty(numAgente))
                            {
                                errores.Add("El <strong>AGENTE</strong> No puede ser validado, ya que el usuario no tiene número de Agente asignado.");
                                acom = false;
                            }
                        }
                    }
                }
            }

            //<SRIINI10693>
            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            if (acom)
            {
                string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
                RolAcom rolAcom = new RolAcom
                {
                    CodRol = (string)HttpContext.Current.Session["RolAzman"],
                    //<SRIINI15069>
                    FechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE")),
                    //<SRIFIN15069>

                };
                //<SRI.INI-20322>
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                //<SRI.FIN-20322>
                List<RolAcom> listaRol = servicioCotizador.ListarRolAcom(rolAcom);
                double vAcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));
                if (acom && KeyAcom == "S" && listaRol.Count > 0)
                {
                    //<SRIINI17003>
                    if (valAcom.Trim().Length == 0) valAcom = "0";
                    //<SRIFIN17003>
                    double vAcomDouble = Convert.ToDouble(valAcom, new CultureInfo("es-PE"));
                    if (vAcomDouble != vAcomComp)
                    {
                        if (vAcomDouble < listaRol[0].NumRangoIni || vAcomDouble > listaRol[0].NumRangoFin)
                        {
                            errores.Add("El campo <strong>Porcentaje A</strong> debe pertenecer al rango:<strong>[" + listaRol[0].NumRangoIni + ":" + listaRol[0].NumRangoFin + "]</strong>.");
                            acom = false;
                        }
                    }
                }
            }
            //<SRIFIN10693>

            // DCOM
            bool dcom = true;
            if (redLocal)
            {
                if (valDcom.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Porcentaje D</strong>. Dato Obligatorio.");
                    dcom = false;
                }
                else
                {
                    double vDcom;
                    //<SRIINI17003>
                    if (valDcom.Trim().Length == 0) valDcom = "0";
                    //<SRIFIN17003>
                    if (!double.TryParse(valDcom, NumberStyles.Any, new CultureInfo("es-PE"), out vDcom))
                    {
                        errores.Add("El campo <strong>Porcentaje D</strong> debe contener un valor numérico.");
                        dcom = false;
                    }
                    else
                    {
                        if (vDcom < 0)
                        {
                            errores.Add("El campo <strong>Porcentaje D</strong> debe contener un valor positivo.");
                            dcom = false;
                        }
                    }
                }
            }

            //<SRI.INI-20322_E2>
            /*Implementacion DCOM, solamente cuando al configuracion sea S*/
            if (dcom)
            {
                string KeyDcom = (string)ConfigurationManager.AppSettings["keyDcom"];
                RolDcom rolDcom = new RolDcom
                {
                    CodRol = (string)HttpContext.Current.Session["RolAzman"]
                    //<SRIINI15069>
                    ,
                    FechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE")),
                    //<SRIFIN15069>

                };
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                List<RolDcom> listaRolDcom = servicioCotizador.ListarRolDcom(rolDcom);

                double vDcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));
                //<SOLINI25781>
                string dcomRangos = String.Empty;
                //<SOLFIN25781>
                if (acom && KeyDcom == "S" && listaRolDcom.Count > 0)
                {
                    if (valDcom.Trim().Length == 0) valDcom = "0";

                    double vDcomDouble = Convert.ToDouble(valDcom, new CultureInfo("es-PE"));
                    if (vDcomDouble != vDcomComp)
                    {
                        //<SOLINI25781>
                        //if (vDcomDouble < listaRolDcom[0].NumRangoIni || vDcomDouble > listaRolDcom[0].NumRangoFin)
                        //{
                        //    errores.Add("El campo <strong>Porcentaje D</strong> debe pertenecer al rango:<strong>[" + listaRolDcom[0].NumRangoIni + ":" + listaRolDcom[0].NumRangoFin + "]</strong>.");
                        //    dcom = false;
                        //}
                        foreach (RolDcom rol in listaRolDcom)
                        {
                            dcomRangos += "[" + rol.NumRangoIni + "-" + rol.NumRangoFin + "] ";
                            if (!(vDcomDouble >= rol.NumRangoIni && vDcomDouble <= rol.NumRangoFin))
                            {
                                dcom = false;
                            }
                            else
                            {
                                dcom = true;
                                break;
                            }
                        }
                        //<SOLINI25781>
                    }
                }
                //<SOLINI25781>
                if (!dcom)
                {
                    errores.Add(String.Format("El campo <strong>Porcentaje D</strong> no se encuentra dentro de los rangos permitidos ({0}).", dcomRangos.TrimEnd()));
                }
                //<SOLFIN25781>
            }
            //<SRI.FIN-20322_E2>


            // Clases de controles
            if (!tipoCambio) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!tipoPension) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!categoria) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!fechaDevengue) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!fechaUltimaActualizacion) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!fechaRecepcion) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!fechaPlazoAFP) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!saldoCIC) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!factorTasa) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!fechaCotizacion) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!fechaSolicitudPension) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!acom) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!dcom) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            // Cotizaciones
            bool cotizaciones = true;
            if (!(listaCotizaciones.Count > 0))
            {
                errores.Add("Debe realizar al menos una cotización.");
                cotizaciones = false;
            }

            //SRI.INI-20322_E2>
            bool traMayorCero = false;
            //SRI.FIN-20322_E2>

            for (int i = 0; i < listaCotizaciones.Count; i++)
            {
                // Moneda
                if (listaCotizaciones[i].Moneda.Id == "0")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Moneda</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    //<SRI.INI-20322>
                    //controles.Add(i + ",1");
                    controles.Add(i + ",2");
                    //<SRI.INI-20322>
                }

                // Producto
                if (listaCotizaciones[i].Producto.Id == "0")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Producto</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    //<SRI.INI-20322>
                    //controles.Add(i + ",2");
                    controles.Add(i + ",3");
                    //<SRI.FIN-20322>
                }

                // Modalidad
                if (listaCotizaciones[i].Modalidad.Id == "0")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Modalidad</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    //<SRI.INI-20322>
                    //controles.Add(i + ",3");
                    controles.Add(i + ",4");
                    //<SRI.FIN-20322>
                }

                //<SOLINIGTI_754>
                // Período Diferido
                ////if (    (listaCotizaciones[i].Modalidad.Id != "D" && listaCotizaciones[i].PeriodoDiferido != 0)
                ////    ||  (listaCotizaciones[i].Modalidad.Id == "D" && listaCotizaciones[i].PeriodoDiferido == 0))
                ////{
                ////    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Diferido</strong> tiene un valor no válido para la Modalidad seleccionada.");
                ////    cotizaciones = false;
                ////    //<SRI.INI-20322>
                ////    //controles.Add(i + ",4");
                ////    controles.Add(i + ",5");
                ////    //<SRI.FIN-20322>
                ////}

                if (listaCotizaciones[i].PeriodoDiferido != 0)
                {
                    switch (listaCotizaciones[i].Modalidad.Id)
                    {
                        case "I":
                        case "I-RB":
                        case "I-RM":
                        case "I-RC":
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Diferido</strong> tiene un valor no válido para la Modalidad seleccionada.");
                            cotizaciones = false;
                            controles.Add(i + ",5");
                            break;
                        case "I-RVE":
                            if (listaCotizaciones[i].PeriodoGarantizado > 0)
                            {
                                if (listaCotizaciones[i].PeriodoGarantizado != listaCotizaciones[i].PeriodoDiferido)
                                {
                                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Diferido</strong> tiene un valor no válido para la Modalidad y Periodo garantizado seleccionado.");
                                    cotizaciones = false;
                                    controles.Add(i + ",5");
                                }
                            }

                            break;
                    }
                }
                else
                {
                    switch (listaCotizaciones[i].Modalidad.Id)
                    {
                        case "D":
                        case "I-RVE":
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Diferido</strong> tiene un valor no válido para la Modalidad seleccionada.");
                            cotizaciones = false;
                            controles.Add(i + ",5");
                            break;
                    }
                }


                // Porcentaje entre Rentas
                ////if ((listaCotizaciones[i].Modalidad.Id != "D" && listaCotizaciones[i].PorcentajeEntreRentas != 0)
                ////    || (listaCotizaciones[i].Modalidad.Id == "D" && listaCotizaciones[i].PorcentajeEntreRentas == 0))
                ////{
                ////    //<SRI.INI-20322>
                ////    //errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Diferido</strong> tiene un valor no válido para la Modalidad seleccionada.");
                ////    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Renta</strong> tiene un valor no válido para la Modalidad seleccionada.");
                ////    //<SRI.FIN-20322>
                ////    cotizaciones = false;
                ////    //<SRI.INI-20322>
                ////    //controles.Add(i + ",5");
                ////    controles.Add(i + ",6");
                ////    //<SRI.FIN-20322>
                ////}
                if (listaCotizaciones[i].PorcentajeEntreRentas != 0)
                {
                    switch (listaCotizaciones[i].Modalidad.Id)
                    {
                        case "I":
                        case "I-RB":
                        case "I-RM":
                        case "I-RC":
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Renta</strong> tiene un valor no válido para la Modalidad seleccionada.");
                            cotizaciones = false;
                            controles.Add(i + ",6");
                            break;
                    }
                }
                else
                {
                    switch (listaCotizaciones[i].Modalidad.Id)
                    {
                        case "D":
                        case "I-RVE":
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Renta</strong> tiene un valor no válido para la Modalidad seleccionada.");
                            cotizaciones = false;
                            controles.Add(i + ",6");
                            break;
                    }
                }

                //<SOLFINGTI_754>

                // Porcentaje entre Rentas
                //<SRI.INI-20322>
                //if (!(listaCotizaciones[i].Capital.Id == "-" || listaCotizaciones[i].Capital.Id == "03"))
                if (!(listaCotizaciones[i].Capital.Id == "-" || listaCotizaciones[i].Capital.Id == "03" || listaCotizaciones[i].Capital.Id == "04"))
                //<SRI.FIN-20322>
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Capital</strong> tiene un valor no válido.");
                    cotizaciones = false;
                    //<SRI.INI-20322>
                    //controles.Add(i + ",8");
                    controles.Add(i + ",9");
                    //<SRI.FIN-20322>
                }

                // Ajuste TRA
                if (listaCotizaciones[i].AjusteTRA.ToString().Length == 0)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Dif. TRA</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                }

                //<SRI.INI-20322_E2>
                if (Math.Abs((double)listaCotizaciones[i].AjusteTRA) > 0)
                {
                    traMayorCero = true;
                }
                //<SRI.FIN-20322_E2>

            }


            //<SRIINI25781>
            // Se comenta temporalmente las validaciones de DTRA a petición del usuario
            ////<SRI.INI-20322_E2>
            ////INICIO VALIDACIÓN TRA
            //if (redLocal && valRequisitosTra && traMayorCero)
            //{

            //    if (correo == "" || correo == null)
            //    {
            //        errores.Add("Debe ingresar un <strong>Correo Electrónico</strong> a la oportunidad para poder realizar la cotización.");
            //        acom = false;
            //    }

            //    if (numAgente != "" && numAgente != null)
            //    {
            //        //Validar PreCubo
            //        if (!Utilitario.ValidarPreCubo(listaParametro, numAgente))
            //        {
            //            List<Parametro> listaPreCubos = listaParametro.Where(x => x.Id == Enums.ParametroTabla.Cubo.StringValue()).ToList();
            //            int total = listaPreCubos.Count;
            //            int contador = 0;
            //            string valores = String.Empty;
            //            foreach (Parametro precubo in listaPreCubos)
            //            {
            //                valores += precubo.Valor_1 + ", ";
            //                contador++;
            //                if (contador == total - 1)
            //                {
            //                    valores = valores.Trim(new char[] { ' ', ',' });
            //                    valores += " o ";
            //                }
            //                if (contador == total)
            //                {
            //                    valores = valores.Trim(new char[2] { ' ', ',' });
            //                }
            //            }
            //            errores.Add(String.Format("Para poder realizar cambios en el <strong>DTRA</strong> debe tener una de las siguientes calificaciones de <strong>Pre Cubo: {0}</strong>.", valores));
            //            acom = false;
            //        }

            //        //Validar Rango Visita
            //        if (!Utilitario.ValidarVisita(listaParametro, numAgente, cusspp, vFechPlazoAFP))
            //        {
            //            List<Parametro> listaParametroVisitaCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.VisitaCita.StringValue()).ToList();
            //            List<Parametro> listaParametroEstadosCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.EstadoCita.StringValue()).OrderBy(x => x.Correlativo).ToList();
            //            string mensaje = String.Format("Para porder realizar cambios en el <strong>DTRA</strong> debe tener una cita <strong>{0}</strong> o <strong>{1} desde {2} días antes hasta {3} días después</strong> de la <strong>Fecha de Plazo de la AFP</strong>.",
            //                listaParametroEstadosCita[0].Valor_2,
            //                listaParametroEstadosCita[1].Valor_2,
            //                listaParametroVisitaCita[0].Valor_1,
            //                listaParametroVisitaCita[1].Valor_1);

            //            errores.Add(mensaje);
            //            acom = false;
            //        }
            //    }
            //    else
            //    {
            //        errores.Add("El agente no puede ser validado, ya que el usuario no tiene número de Agente asignado.");
            //        acom = false;
            //    }

            //}
            ////FIN VALIDACIÓN TRA
            ////<SRI.FIN-20322_E2>
            //<SRIFIN25781>

            // Beneficiarios
            bool beneficiarios = true;
            if (!(idBeneficiarios.Count > 0))
            {
                errores.Add("Debe seleccionar al menos un beneficiario para realizar la cotización.");
                beneficiarios = false;
            }

            esCorrecto = tipoCambio & tipoPension & categoria & fechaDevengue & fechaUltimaActualizacion & fechaRecepcion & fechaPlazoAFP & saldoCIC & factorTasa & fechaCotizacion & fechaSolicitudPension & acom & dcom & cotizaciones & beneficiarios;

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
                        // Validar la cartera de agente
                        if (Utilitarios.EsRolVerAgentesCesados((string)Session["RolAzman"]) || ((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == (string)Session["Vendedor"]))
                        {
                            if (ValidarAfiliado())
                            {
                                Afiliado afiliado = new Afiliado
                                {
                                    CUSPP = CUSPP.Text,
                                    Categoria = new Categoria { Id = Categoria.SelectedValue },
                                    AFP = new AFP { Id = AFP.SelectedValue },
                                    SaldoCIC = ((bool)Session["Consentimiento"]) ? (double?)Convert.ToDouble(SaldoCIC.Text, new CultureInfo("es-PE")) : null,
                                    TipoIdentificacion = TipoDocumento.SelectedValue,
                                    NumeroIdentificacion = NumeroDocumento.Text
                                };

                                afiliado.CorreoElectronico = CorreoElectronico.Text;
                                afiliado.Telefonos = Telefono.Text;
                                afiliado.Celulares = Celular.Text;

                                HCategoria.Value = Categoria.SelectedValue;
                                HAFP.Value = AFP.SelectedValue;

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                Respuesta respuesta = servicioCotizador.ActualizarAfiliado(afiliado);

                                //consentimiento
                                obtenerConsentimiento(afiliado);

                                log.Info(string.Format("Usuario actualizó los datos del afiliado CUSPP[{0}].", CUSPP.Text));
                                log.Debug(string.Format("Correo Electrónico[{0}] Categoría[{1}:{2}] AFP[{3}:{4}] Saldo CIC[{5}].",
                                    CorreoElectronico.Text,
                                    Categoria.SelectedValue, Categoria.SelectedItem.Text,
                                    AFP.SelectedValue, AFP.SelectedItem.Text,
                                    SaldoCIC.Text));

                                string nombreTerminal = string.Empty;
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

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = Request.ServerVariables["remote_addr"],
                                    NombreUsuario = (string)Session["Usuario"],
                                    Detalle = string.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "ActualizarAfiliado", Environment.NewLine, Environment.NewLine, "afiliado", JsonConvert.SerializeObject(afiliado)),
                                    IdTipoEvento = Enums.EventoLog.ModificarDatosCliente.StringValue()
                                });

                                AporteAdicional aporte = new AporteAdicional();
                                aporte.num_cuispp = CUSPP.Text;
                                Respuesta rptAporte = new Respuesta();

                                if (!string.IsNullOrWhiteSpace(txtPensionRef.Text)
                                    && ddlMonedaReferencia.SelectedValue != "0"
                                    && !string.IsNullOrWhiteSpace(txtTasaAporte.Text)
                                    && !string.IsNullOrWhiteSpace(dtpFechaPago.Text)
                                    && !string.IsNullOrWhiteSpace(txtMontoAporte.Text))
                                {
                                    aporte.val_pension_referencia = Convert.ToDouble(txtPensionRef.Text);
                                    aporte.cod_moneda_pension_ref = ddlMonedaReferencia.SelectedValue.ToString();
                                    aporte.val_tasa_aporte = Convert.ToDouble(txtTasaAporte.Text);
                                    aporte.fec_pagoapad = Convert.ToDateTime(dtpFechaPago.Text, new CultureInfo("es-PE"));
                                    aporte.val_monto_aporte = Convert.ToDouble(txtMontoAporte.Text);
                                    rptAporte = servicioCotizador.ActualizarAporteAdicional(aporte, (string)Session["Usuario"]);
                                }
                                else
                                {
                                    txtPensionRef.Text = string.Empty;
                                    ddlMonedaReferencia.SelectedIndex = 0;
                                    txtTasaAporte.Text = string.Empty;
                                    dtpFechaPago.Text = string.Empty;
                                    txtMontoAporte.Text = string.Empty;

                                    rptAporte.Estado = Constante.COD_OK;
                                }

                                if (respuesta.Estado == Constante.COD_OK && rptAporte.Estado == Constante.COD_OK)
                                {
                                    MCMMensaje.Text = "Datos del afiliado actualizados correctamente.";
                                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Exito.StringValue();
                                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                    MCMEstado.Value = "1";

                                    CorreoElectronicoRegistrado.Value = afiliado.CorreoElectronico;
                                }
                                else
                                {
                                    log.Error("No se pudo actualizar la información de afiliado: [" + respuesta.Mensaje + "]");
                                    MCMMensaje.Text = "No se pudo actualizar la información: [" + respuesta.Mensaje + "]";
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

            // Apellido Paterno
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

            // Apellido Materno
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

            // Nombres
            bool nombres = true;
            bool eNombres = false;
            if (glsNombres.Trim().Length > 0)
            {
                eNombres = true;
                if (glsNombres.Trim().Length < 2)
                {
                    errores.Add("El campo <strong>Apellido Materno</strong> debe contener al menos 2 caracteres.");
                    nombres = false;
                }
            }

            // Criterio mínimo
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

            MCMEstado.Value = "0";
            CorreoElectronico.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro";
            Categoria.CssClass = "formComboboxTexto formTextboxReadOnly";
            AFP.CssClass = "formCombobox";
            SaldoCIC.CssClass = "formTextbox";

            // Correo Electrónico
            bool correoElectronico = true;
            if ((bool)Session["Consentimiento"])
            {
                if (CorreoElectronico.Text.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.");
                    correoElectronico = false;
                }
                else
                {
                    Regex regex = new Regex(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$");
                    Match match = regex.Match(CorreoElectronico.Text);
                    if (!match.Success || CorreoElectronico.Text.ToUpper().Contains("Ñ"))
                    {
                        errores.Add("El campo <strong>Correo Electrónico</strong> es inválido. Verifique.");
                        correoElectronico = false;
                    }
                }
            }

            // Categoría
            bool categoria = true;
            if (Categoria.SelectedValue == "0")
            {
                errores.Add("Ingrese el campo <strong>Categoría</strong>. Dato Obligatorio.");
                categoria = false;
            }

            // AFP
            bool afp = true;
            if (AFP.SelectedValue == "0")
            {
                errores.Add("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
                afp = false;
            }

            // Saldo CIC
            bool saldoCIC = true;
            if ((bool)Session["Consentimiento"])
            {
                if (SaldoCIC.Text.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Saldo CIC</strong>. Dato Obligatorio.");
                    saldoCIC = false;
                }
                else
                {
                    double vSaldoCIC;
                    if (!double.TryParse(SaldoCIC.Text, NumberStyles.Any, new CultureInfo("es-PE"), out vSaldoCIC))
                    {
                        errores.Add("El campo <strong>Saldo CIC</strong> debe contener un valor numérico.");
                        saldoCIC = false;
                    }
                    else
                    {
                        if (vSaldoCIC <= 0)
                        {
                            errores.Add("El campo <strong>Saldo CIC</strong> debe contener un valor positivo.");
                            saldoCIC = false;
                        }
                    }
                }
            }

            // Clases de controles
            if (!correoElectronico) { CorreoElectronico.CssClass = "formTextboxReadOnly formTextboxLetra formTextboxError"; } else { CorreoElectronico.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro"; }
            if (!categoria) { Categoria.CssClass = "formComboboxTexto formTextboxReadOnly formComboboxError"; } else { Categoria.CssClass = "formComboboxTexto formTextboxReadOnly"; }
            if (!afp) { AFP.CssClass = "formCombobox formComboboxError"; } else { AFP.CssClass = "formCombobox"; }
            if (!saldoCIC) { SaldoCIC.CssClass = "formTextbox formTextboxError numerico"; } else { SaldoCIC.CssClass = "formTextbox numerico"; }

            Telefono.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono";
            Celular.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono";

            esCorrecto = correoElectronico & categoria & afp & saldoCIC;

            if (!esCorrecto)
            {
                MCMMensaje.Text = Utilitarios.FormatearError(errores);
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Validacion.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                MCMEstado.Value = "1";
            }

            return esCorrecto;
        }

        /*******/

        /*<SRIINI10693>*/
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
        /*<SRIFIN10693>*/

        //[WebMethod]
        //public static Respuesta Cotizar(string tokenUsuario, string idSolicitud)
        //{
        //    using (NDC.Push(MethodBase.GetCurrentMethod().Name))
        //    {
        //        Solicitud sol;
        //        try
        //        {
        //            Respuesta respuesta = new Respuesta();

        //            if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
        //            {
        //                string nombreTerminal = String.Empty;
        //                try
        //                {
        //                    nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
        //                }
        //                catch (Exception)
        //                {
        //                    log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
        //                        HttpContext.Current.Request.ServerVariables["remote_addr"]));
        //                }

        //                nombreTerminal += HttpContext.Current.Request.UserAgent;

        //                servicioCotizador = LocalizadorProxy.ObtenerServicio();
        //                servicioCotizador.RegistrarLog(new LogBD
        //                {
        //                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
        //                    NombreTerminal = nombreTerminal,
        //                    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
        //                    NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
        //                    IdTipoEvento = Enums.EventoLog.CotizarSolicitud.StringValue(),
        //                    Detalle = "Solicitud: " + idSolicitud
        //                });

        //                servicioCotizador = LocalizadorProxy.ObtenerServicio();
        //                return servicioCotizador.Cotizar(idSolicitud, (string)HttpContext.Current.Session["Usuario"]);
        //            }
        //            else
        //            {
        //                sol = new Solicitud();
        //                log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
        //                respuesta.Estado = Constante.COD_TOKEN;
        //            }
        //            return respuesta;
        //        }
        //        catch (Exception ex)
        //        {
        //            Respuesta respuesta = new Respuesta();
        //            respuesta.Estado = Constante.COD_ERROR;
        //            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        //            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        //            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
        //            return respuesta;
        //        }
        //    }
        //}

        //<SOLINI25621>
        [WebMethod]
        public static Respuesta ImprimirFicha(string tokenUsuario, string cuspp)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        HttpContext.Current.Session["CUSPP_IMPRIMIR"] = cuspp;
                        respuesta.Estado = Constante.COD_OK;
                        //return Constante.COD_OK;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        //return Constante.COD_TOKEN;
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                }
                return respuesta;
            }
        }

        [WebMethod]
        public static List<Parametro> ObtenerPeriodoTemporal(string codModalidad)
        {
            //<INIGTI_754>
            List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametro2 = new List<Parametro>();
            lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboPeriodoTemporal"];

            if (lstParametro.Count > 0)
            {
                lstParametro
                            .FindAll(p => (
                                           (p.Id == codModalidad)
                                          )
                            )
                            .ForEach(p =>
                            {
                                for (var i = Convert.ToInt32(p.Valor_1); i <= Convert.ToInt32(p.Valor_2); i++)
                                {
                                    lstParametro2.Add(new Parametro { Valor_1 = i.ToString(), Valor_2 = i.ToString() });
                                }

                            });

                //Parametro parametro = lstParametro.Find(x => x.Id == codModalidad);
                //lstParametro = null;
                //lstParametro = new List<Parametro>();
                //lstParametro = null;
                //lstParametro = new List<Parametro>();
                //if (parametro != null)
                //{
                //    for (var i = Convert.ToInt32(parametro.Valor_1); i <= Convert.ToInt32(parametro.Valor_2); i++)
                //    {
                //        lstParametro.Add(new Parametro { Valor_1 = i.ToString(), Valor_2 = i.ToString()});       
                //    }
                //}   
            }
            return lstParametro2;
            //<FINGTI_754>
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

        [WebMethod]
        public static Respuesta EnviarConsentimientoAsesoria(ParametrosEnvioCDA parametros)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                    if (parametros.categoria == Enums.CategoriaRVI.Sobrevivencia.StringValue())
                    {
                        parametros.nombres = HttpContext.Current.Session["ssNombres"].ToString();
                        parametros.tipoDocumento = HttpContext.Current.Session["ssCodTipoDocumento"].ToString();
                        parametros.numeroDocumento = HttpContext.Current.Session["ssNumeroDocumento"].ToString();
                        parametros.correo = HttpContext.Current.Session["ssCorreo"].ToString();
                        parametros.apellidoPaterno = HttpContext.Current.Session["ssApellidosPaterno"].ToString();
                        parametros.apellidoMaterno = HttpContext.Current.Session["ssApellidosMaterno"].ToString();
                        parametros.sexo = HttpContext.Current.Session["ssSexo"].ToString();
                        parametros.fechaNacimiento = HttpContext.Current.Session["ssFechaNacimiento"].ToString();
                    }

                    if (string.IsNullOrEmpty(parametros.tipoDocumento)) parametros.tipoDocumento = "D";

                    string destinatarioCliente = ConfigurationManager.AppSettings["destinatario_consentimiento"].ToString();

                    if (destinatarioCliente != "N")
                    {
                        parametros.correo = destinatarioCliente;
                    }

                    string usuario = HttpContext.Current.Session["Usuario"].ToString();

                    Agente agente = ObtenerDatosAgente(parametros, usuario);

                    // Validar si es que el consentimiento ya existe
                    string urlConsultaConsentimiento = ConfigurationManager.AppSettings["url_consulta_consentimiento_cliente"];
                    urlConsultaConsentimiento = string.Format(urlConsultaConsentimiento, Enums.TratamientoConsentimiento.RVI.StringValue(), parametros.tipoDocumento, parametros.numeroDocumento, usuario);

                    log.Debug($"Consumiendo endpoint urlConsultaConsentimiento | [GET] {urlConsultaConsentimiento}");
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlConsultaConsentimiento);
                    httpWebRequest.Method = "GET";

                    ConsentimientoCliente consentimiento = null;

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        string responseBody = streamReader.ReadToEnd();
                        consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                    }

                    //log.Debug("Obteniendo el token: " + token);
                    string urlConsentimientoCliente = ConfigurationManager.AppSettings["url_consentimiento_cliente"];
                    string urlAppConsentimiento = ConfigurationManager.AppSettings["url_app_consentimiento"];
                    string flagCorreoCliente = ConfigurationManager.AppSettings["flag_correo_cliente"];
                    string token_generado = string.Empty;

                    if (consentimiento != null)
                    {
                        if (consentimiento.ind_consentimiento != "S")
                        {
                            ConsentimientoCliente consentimientoCliente = new ConsentimientoCliente();

                            consentimientoCliente.id_consentimiento_asesoria = consentimiento.id_consentimiento_asesoria;
                            consentimientoCliente.ind_consentimiento = null;
                            consentimientoCliente.gls_token = parametros.token;
                            consentimientoCliente.cod_tipo_identificacion = parametros.tipoDocumento;
                            consentimientoCliente.gls_num_identificacion = parametros.numeroDocumento;
                            consentimientoCliente.id_configuracion = Convert.ToInt32(Enums.ConfiguracionConsentimiento.RVI.StringValue());
                            consentimientoCliente.gls_nombres = parametros.nombres;
                            consentimientoCliente.gls_apellido_paterno = parametros.apellidoPaterno;
                            consentimientoCliente.gls_apellido_materno = parametros.apellidoMaterno;
                            consentimientoCliente.gls_sexo = parametros.sexo;
                            consentimientoCliente.fec_nacimiento = Convert.ToDateTime(parametros.fechaNacimiento, new CultureInfo("es-PE"));
                            consentimientoCliente.gls_mail = parametros.correo;
                            consentimientoCliente.gls_telefono = parametros.telefono;
                            consentimientoCliente.gls_celular = parametros.celular;
                            consentimientoCliente.gls_nombres_agente = ti.ToTitleCase(agente.Nombre.ToString().Trim().ToLower());
                            consentimientoCliente.gls_mail_agente = agente.CorreoElectronico;
                            consentimientoCliente.num_agente = Convert.ToInt32(agente.Id);
                            consentimientoCliente.aud_usr_modificacion = usuario;

                            var context = new HttpContextWrapper(HttpContext.Current);
                            HttpRequestBase request = context.Request;

                            log.Debug($"Consumiendo endpoint urlConsentimientoCliente | [PUT] {urlConsentimientoCliente}");
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(urlConsentimientoCliente);

                            httpWebRequest.UserAgent = request.UserAgent;
                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "PUT";
                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                            var jsonConsentimientoCliente = JsonConvert.SerializeObject(consentimientoCliente);
                            log.Debug($"jsonConsentimientoCliente | [BODY] {jsonConsentimientoCliente}");

                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            {
                                streamWriter.Write(jsonConsentimientoCliente);
                                streamWriter.Flush();
                                streamWriter.Close();
                            }

                            log.Info("Leyendo el servicio");
                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                consentimientoCliente = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                            }

                            log.Info("Armando la url + token");
                            urlAppConsentimiento = string.Format(urlAppConsentimiento, consentimientoCliente.gls_token);

                            log.Info("Enviando la solicitud de CDA por EMAIL o SMS");
                            if (flagCorreoCliente == "S")
                            {
                                if (parametros.canalComunicacion == "EMAIL")
                                {
                                    int codProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEConsentimientoRV"]);

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
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Consentimiento de asesoría enviado correctamente." });
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
                        ConsentimientoCliente consentimientoCliente = new ConsentimientoCliente();

                        consentimientoCliente.cod_tipo_identificacion = parametros.tipoDocumento;
                        consentimientoCliente.gls_num_identificacion = parametros.numeroDocumento;
                        consentimientoCliente.id_configuracion = Convert.ToInt32(Enums.ConfiguracionConsentimiento.RVI.StringValue());
                        consentimientoCliente.gls_nombres = parametros.nombres;
                        consentimientoCliente.gls_apellido_paterno = parametros.apellidoPaterno;
                        consentimientoCliente.gls_apellido_materno = parametros.apellidoMaterno;
                        consentimientoCliente.gls_sexo = parametros.sexo;
                        consentimientoCliente.fec_nacimiento = Convert.ToDateTime(parametros.fechaNacimiento, new CultureInfo("es-PE"));
                        consentimientoCliente.gls_mail = parametros.correo;
                        consentimientoCliente.gls_telefono = parametros.telefono;
                        consentimientoCliente.gls_celular = parametros.celular;
                        consentimientoCliente.gls_nombres_agente = ti.ToTitleCase(agente.Nombre.ToString().Trim().ToLower());
                        consentimientoCliente.gls_mail_agente = agente.CorreoElectronico;
                        consentimientoCliente.num_agente = Convert.ToInt32(agente.Id);
                        consentimientoCliente.aud_usr_ingreso = usuario;

                        var context = new HttpContextWrapper(HttpContext.Current);
                        HttpRequestBase request = context.Request;

                        log.Debug($"Consumiendo endpoint urlConsentimientoCliente | [POST] {urlConsentimientoCliente}");
                        httpWebRequest = (HttpWebRequest)WebRequest.Create(urlConsentimientoCliente);

                        httpWebRequest.UserAgent = request.UserAgent;
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";
                        httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                        var jsonConsentimientoCliente = JsonConvert.SerializeObject(consentimientoCliente);
                        log.Debug($"jsonConsentimientoCliente | [BODY] {jsonConsentimientoCliente}");

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            streamWriter.Write(jsonConsentimientoCliente);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        log.Info("Leyendo el servicio");
                        httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            string responseBody = streamReader.ReadToEnd();
                            consentimientoCliente = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                        }

                        if (consentimientoCliente.gls_token.Length > 0)
                        {
                            log.Info("Consumiendo el método de actualizar el consentimiento");

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();

                            Afiliado afiliado_ = new Afiliado()
                            {
                                CUSPP = parametros.cuspp
                            };

                            ConsentimientoAsesoria consentimientoAsesoria_ = new ConsentimientoAsesoria()
                            {
                                IdConsentimientoAsesoria = consentimientoCliente.id_consentimiento_asesoria,
                                idContactoAsesoria = Convert.ToInt32(HttpContext.Current.Session["ssIdContacto"]),
                                CodProducto = "RVI",
                                usuario = usuario
                            };

                            var actualizarConsentimiento = servicioCotizador.ActualizarConsentimientoAfiliado(afiliado_, consentimientoAsesoria_);
                            if (actualizarConsentimiento.Estado != Constante.COD_OK)
                            {
                                log.Error("Error al actualizar el consentimiento");
                                throw new Exception("Error al actualizar el consentimiento");
                            }

                            log.Info("Armando la url + token");
                            urlAppConsentimiento = string.Format(urlAppConsentimiento, consentimientoCliente.gls_token);

                            log.Info("Enviando el correo SME");
                            if (flagCorreoCliente == "S")
                            {
                                if (parametros.canalComunicacion == "EMAIL")
                                {
                                    int codProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEConsentimientoRV"]);

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
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Consentimiento de asesoría enviado correctamente." });
                        }
                        else
                        {
                            log.Warn("El cliente ya aceptó el consentimiento.");
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Advertencia.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Advertencia.StringValue();
                            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>El cliente ya aceptó el consentimiento, por favor refresque la página para poder visualizarlo.</strong></div>";
                        }
                    }

                    // Envío de notificación al agente
                    string[] rolesNotificacion = {
                            Enums.RolAzman.SupervisorLima.StringValue(),
                            Enums.RolAzman.SupervisorProvincia.StringValue(),
                            Enums.RolAzman.JefeVentaLima.StringValue(),
                            Enums.RolAzman.JefeVentaProvincia.StringValue()
                    };

                    if (rolesNotificacion.Contains(HttpContext.Current.Session["RolAzman"]))
                    {
                        if (respuesta.Estado == Constante.COD_OK)
                        {
                            var glsAgente = ti.ToTitleCase(agente.Nombre.ToLower().Trim());
                            var glsCliente = ti.ToTitleCase($"{parametros.nombres} {parametros.apellidoPaterno} {parametros.apellidoMaterno}".ToLower().Trim());

                            log.Debug(string.Format("Se va a enviar una notificación al agente [{0}]", agente.CorreoElectronico));
                            NotificarAgente(glsAgente, "Consentimiento de Asesoría", glsCliente, parametros.cuspp, agente.CorreoElectronico, parametros.glsCategoria);
                        }
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
            List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];

            Agente agente = listaAgentes.Find(a => a.Id == HttpContext.Current.Session["Vendedor"].ToString());

            if (agente == null && Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]))
            {
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado("", parametros.cuspp, "", "", "");
                agente = servicioCotizador.ObtenerUltimoAgentePorCartera(afiliado.Agente.IdCartera, usuario);
            }

            /* OBTENER CORREO DEL AGENTE */
            string destinatarioAgente = ConfigurationManager.AppSettings["destinatario_consentimiento_agente"].ToString();

            agente.CorreoElectronico = destinatarioAgente;

            try
            {
                log.Info("Obteniendo el correo del agente");
                AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient servicioAzman = new AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient("epAzman");

                var datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                        ConfigurationManager.AppSettings["AplicacionAZMAN"],
                        ConfigurationManager.AppSettings["DominioRed"],
                        agente.Usuario);

                if (datosUsuario == null)
                {
                    throw new Exception(string.Format("No se ha encontrado información para el agente [{0}]", agente.Usuario));
                }
                else
                {
                    if (destinatarioAgente == "N")
                    {
                        agente.CorreoElectronico = datosUsuario.Correo;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Warn($"Error al obtener el email del agente {agente.Usuario}.", ex);
            }

            return agente;
        }

        private static void EnviarSolicitudCdaEmail(ParametrosEnvioCDA parametros, string urlAppConsentimiento, int codProcesoSme, Agente agente)
        {
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

            int idProcesoEnvio = (int)Enums.ProcesoEnvio.ConsentimientoRV;

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

            int idProcesoEnvio = (int)Enums.ProcesoEnvio.ConsentimientoRV;

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

        private static void NotificarAgente(string agente, string solicitud, string cliente, string cuspp, string correoAgente, string glsCategoria)
        {
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            var rolesPermitidos = new List<string>
            {
                Enums.RolAzman.JefeVentaLima.StringValue(),
                Enums.RolAzman.JefeVentaProvincia.StringValue(),
                Enums.RolAzman.SupervisorLima.StringValue(),
                Enums.RolAzman.SupervisorProvincia.StringValue()
            };

            if (rolesPermitidos.Contains(HttpContext.Current.Session["RolAzman"].ToString()))
            {
                glsCategoria = ti.ToTitleCase(glsCategoria.ToLower().Trim());

                var htmlCorreoAgente = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\Plantilla\\RVI\\Consentimiento\\NotificacionAgente.html");
                htmlCorreoAgente = htmlCorreoAgente
                    .Replace("{agente}", agente)
                    .Replace("{usuario}", HttpContext.Current.Session["NombreCompleto"].ToString())
                    .Replace("{solicitud}", solicitud)
                    .Replace("{cliente}", cliente)
                    .Replace("{cuspp}", cuspp)
                    .Replace("{categoria}", glsCategoria);

                var notificacion = new NotificacionSME()
                {
                    De = "comunicaciones@interseguro.com.pe",
                    DeNombre = "Comunicaciones Interseguro",
                    ResponderA = "comunicaciones@interseguro.com.pe",
                    ResponderANombre = "Comunicaciones Interseguro",
                    Para = correoAgente,
                    Cuerpo = htmlCorreoAgente,
                    Asunto = $"{solicitud} enviado a {cliente}",
                };

                EnviarNotificacionSME(ConfigurationManager.AppSettings["url_envio_correo_sme"], notificacion);
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
                        urlFormatoConsentimientoCliente = string.Format(urlFormatoConsentimientoCliente, idConsentimientoAsesoria, usuario);
                        log.Debug("Consumiendo servicio pdf: " + urlFormatoConsentimientoCliente);

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
        public static Respuesta ReenviarFormatoConsentimientoAsesoria(string tokenUsuario, string idConsentimientoAsesoria, string categoria, string tipoIdentificacion, string numeroIdentificacion, string nombre, string apellidoPaterno, string apellidoMaterno, string email, string numAgente)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        log.Info("Inicio ReenviarFormatoConsentimientoAsesoria");
                        string urlFormatoConsentimientoCliente = ConfigurationManager.AppSettings["url_formato_consentimiento_cliente"].ToString();
                        string usuario = HttpContext.Current.Session["Usuario"].ToString();

                        if (categoria == Enums.CategoriaRVI.Sobrevivencia.StringValue())
                        {
                            string urlConsentimientoPorId = ConfigurationManager.AppSettings["url_consulta_consentimiento_id"].ToString();
                            string url = string.Format(urlConsentimientoPorId, idConsentimientoAsesoria, usuario);
                            log.Debug("Consumiendo endpoint: " + url);
                            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            httpWebRequest.Method = "GET";
                            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                ConsentimientoCliente consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);

                                tipoIdentificacion = consentimiento.cod_tipo_identificacion;
                                numeroIdentificacion = consentimiento.gls_num_identificacion;
                                if (!string.IsNullOrEmpty(consentimiento.gls_nombres)) nombre = consentimiento.gls_nombres;
                                if (!string.IsNullOrEmpty(consentimiento.gls_apellido_paterno)) apellidoPaterno = consentimiento.gls_apellido_paterno;
                                if (!string.IsNullOrEmpty(consentimiento.gls_apellido_materno)) apellidoMaterno = consentimiento.gls_apellido_materno;
                            }
                        }

                        urlFormatoConsentimientoCliente = string.Format(urlFormatoConsentimientoCliente, idConsentimientoAsesoria, usuario);
                        log.Info("Consumiendo servicio pdf: " + urlFormatoConsentimientoCliente);

                        WebClient myWebClient = new WebClient();

                        byte[] formatoByteArray = myWebClient.DownloadData(urlFormatoConsentimientoCliente);
                        myWebClient.Dispose();

                        string rutaArchivoConsentimiento = string.Format("{0}\\Consentimiento_{1}{2}.pdf", ConfigurationManager.AppSettings["ruta_Reporte_Generado_Trazabilidad"], tipoIdentificacion, numeroIdentificacion);
                        File.WriteAllBytes(rutaArchivoConsentimiento, formatoByteArray);

                        List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                        Agente agente = listaAgentes.Find(a => a.Id == numAgente);

                        SMEEnvio envio = new SMEEnvio
                        {
                            Email = email,
                            Destinatario = string.Format("{0} {1} {2}", nombre, apellidoPaterno, apellidoMaterno),
                            NumeroDocumento = numeroIdentificacion,
                            Contrasenia = "",
                            NumeroPoliza = "N/A",
                            ProcesoSme = ConfigurationManager.AppSettings["SMEConsentimientoRVRespuesta"],
                            RutaPdf = rutaArchivoConsentimiento,
                            CamposDinamicosSerializados = JsonConvert.SerializeObject(new
                            {
                                Id_nombres = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nombre.ToLower()),
                                Id_agente = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(agente.Nombre.ToLower())
                            })
                        };
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        long codigoSME = servicioCotizador.EnviarCorreoSME(envio);

                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                        respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Consentimiento de asesoría enviado correctamente." });
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

        [WebMethod]
        public static Respuesta ValidacionConsentimientoAsesoria(string cod_beneficiario, string correo_electronico, string cuspp)
        {
            Respuesta respuesta = new Respuesta();

            if (cod_beneficiario != "0")
            {
                List<string> errores = new List<string>();
                bool nombres = false;
                bool documento = false;
                bool correo = false;
                bool apoderado = false;

                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                GrupoFamiliar beneficiario = servicioCotizador.ObtenerDatosGrupoFamiliar(Convert.ToInt32(cod_beneficiario), "");

                if (beneficiario.IndTieneApoderado)
                {
                    apoderado = true;
                }

                //validación
                if (beneficiario.Nombre == null
                        || beneficiario.ApellidoPaterno == null
                        || beneficiario.ApellidoMaterno == null)
                {
                    errores.Add("Ingrese los campos <strong>Nombres y apellidos completos</strong>. Datos obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                    nombres = true;
                }
                else
                {
                    if (beneficiario.Nombre.Length == 0
                        || beneficiario.ApellidoPaterno.Length == 0
                        || beneficiario.ApellidoMaterno.Length == 0)
                    {
                        errores.Add("Ingrese los campos <strong>Nombres y apellidos completos</strong>. Datos obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                        nombres = true;
                    }
                    else
                    {
                        if (apoderado)
                        {
                            if (beneficiario.NombresApdo == null
                                    || beneficiario.ApellidoPaternoApdo == null
                                    || beneficiario.ApellidoMaternoApdo == null)
                            {
                                errores.Add("Ingrese los campos <strong>Nombres y apellidos completos del apoderado</strong>. Datos obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                                nombres = true;
                            }
                            else
                            {
                                if (beneficiario.NombresApdo.Length == 0
                                    || beneficiario.ApellidoPaternoApdo.Length == 0
                                    || beneficiario.ApellidoMaternoApdo.Length == 0)
                                {
                                    errores.Add("Ingrese los campos <strong>Nombres y apellidos completos del apoderado</strong>. Datos obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                                    nombres = true;
                                }
                                else
                                {
                                    HttpContext.Current.Session["ssIdContacto"] = beneficiario.Id;
                                    HttpContext.Current.Session["ssNombres"] = beneficiario.NombresApdo;
                                    HttpContext.Current.Session["ssApellidosPaterno"] = beneficiario.ApellidoPaternoApdo;
                                    HttpContext.Current.Session["ssApellidosMaterno"] = beneficiario.ApellidoMaternoApdo;
                                    HttpContext.Current.Session["ssSexo"] = beneficiario.SexoApdo;
                                    HttpContext.Current.Session["ssFechaNacimiento"] = beneficiario.FechaNacimientoApdo.Value.ToString("dd/MM/yyyy");
                                }
                            }

                        }
                        else
                        {
                            HttpContext.Current.Session["ssIdContacto"] = beneficiario.Id;
                            HttpContext.Current.Session["ssNombres"] = beneficiario.Nombre;
                            HttpContext.Current.Session["ssApellidosPaterno"] = beneficiario.ApellidoPaterno;
                            HttpContext.Current.Session["ssApellidosMaterno"] = beneficiario.ApellidoMaterno;
                            HttpContext.Current.Session["ssSexo"] = beneficiario.Sexo;
                            HttpContext.Current.Session["ssFechaNacimiento"] = beneficiario.FechaNacimiento.Value.ToString("dd/MM/yyyy");
                        }
                    }
                }

                if (beneficiario.Identificacion.IdTipo == null
                        || beneficiario.Identificacion.Numero == null)
                {
                    errores.Add("Ingrese los campos <strong>Tipo y número de documento</strong>. Datos Obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                    documento = true;
                }
                else
                {
                    if (beneficiario.Identificacion.IdTipo.Length == 0
                        || beneficiario.Identificacion.Numero.Length == 0 || beneficiario.Identificacion.Numero.Length <= 4)
                    {
                        errores.Add("Ingrese los campos <strong>Tipo y número de documento</strong>. Datos Obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                        documento = true;
                    }
                    else
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                        List<Parametro> tipoIdentificacion = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Identificacion];

                        var tipodoc = tipoIdentificacion.Find(ti => ti.Id == beneficiario.Identificacion.IdTipo).Glosa;

                        if (tipodoc == null)
                        {
                            errores.Add("Ingrese los campos <strong>Tipo y número de documento</strong>. Datos Obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                            documento = true;
                        }
                        else
                        {
                            if (apoderado)
                            {
                                if (beneficiario.IdentificacionApdo.IdTipo == null
                                            || beneficiario.IdentificacionApdo.Numero == null)
                                {
                                    errores.Add("Ingrese los campos <strong>Tipo y número de documento del apoderado</strong>. Datos Obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                                    documento = true;
                                }
                                else
                                {
                                    if (beneficiario.IdentificacionApdo.IdTipo.Length == 0
                                        || beneficiario.IdentificacionApdo.Numero.Length == 0 || beneficiario.IdentificacionApdo.Numero.Length <= 4)
                                    {
                                        errores.Add("Ingrese los campos <strong>Tipo y número de documento del apoderado</strong>. Datos Obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                                        documento = true;
                                    }
                                    else
                                    {
                                        var tipodocApo = tipoIdentificacion.Find(ti => ti.Id == beneficiario.IdentificacionApdo.IdTipo).Glosa;

                                        if (tipodocApo == null)
                                        {
                                            errores.Add("Ingrese los campos <strong>Tipo y número de documento del apoderado</strong>. Datos Obligatorios, actualizar la información en la pestaña <strong>Grupo Familiar</strong>.");
                                            documento = true;
                                        }
                                        else
                                        {
                                            HttpContext.Current.Session["ssTipoDocumento"] = tipoIdentificacion.Find(ti => ti.Id == beneficiario.IdentificacionApdo.IdTipo).Glosa;
                                            HttpContext.Current.Session["ssCodTipoDocumento"] = beneficiario.IdentificacionApdo.IdTipo;
                                            HttpContext.Current.Session["ssNumeroDocumento"] = beneficiario.IdentificacionApdo.Numero;
                                        }

                                    }
                                }

                            }
                            else
                            {
                                HttpContext.Current.Session["ssTipoDocumento"] = tipoIdentificacion.Find(ti => ti.Id == beneficiario.Identificacion.IdTipo).Glosa;
                                HttpContext.Current.Session["ssCodTipoDocumento"] = beneficiario.Identificacion.IdTipo;
                                HttpContext.Current.Session["ssNumeroDocumento"] = beneficiario.Identificacion.Numero;
                            }
                        }
                    }
                }

                if (correo_electronico.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio, actualizar la información en el <strong>vtiger</strong>.");
                    correo = true;
                }
                else
                {
                    HttpContext.Current.Session["ssCorreo"] = correo_electronico;
                }

                respuesta.Estado = Constante.COD_OK;

                if (nombres || documento || correo)
                {
                    //ClientScript.RegisterStartupScript(GetType(), "-", "$('#MCMIcono').attr('class', 'error'); $('#MCMContenedor').html(" + Utilitarios.FormatearError(errores) + "); $('#ModalCuadroMensaje').dialog({ title: 'Error' }); $('#ModalCuadroMensaje').dialog('open');", true);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(errores);
                }
                else
                {
                    // Consentimientos
                    string usuario = HttpContext.Current.Session["Usuario"].ToString();

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    string idConfiguracionUniversal = Enums.TratamientoConsentimiento.InterseguroUniversal.StringValue();

                    // Universal Interseguro
                    string urlConsultaConsentimientoUniversal = ConfigurationManager.AppSettings["url_consulta_consentimiento_universal"].ToString();
                    string url = string.Format(urlConsultaConsentimientoUniversal, idConfiguracionUniversal, beneficiario.Identificacion.IdTipo, beneficiario.Identificacion.Numero, usuario);
                    log.Debug("Consumiendo endpoint consulta consentimiento universal: " + url);

                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "GET";
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        string responseBody = streamReader.ReadToEnd();
                        ConsentimientoCliente consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                        if (consentimiento != null)
                        {
                            //JObject jObject = JObject.Parse(responseBody);
                            //var indicadorConsentimiento = jObject["ind_consentimiento"].ToString();
                            //var idConsentimiento = (int)jObject["id_consentimiento_asesoria"];
                            if (consentimiento.ind_consentimiento == "S")
                            {
                                ConsentimientoAsesoria consentimientoAsesoria = new ConsentimientoAsesoria
                                {
                                    IdConsentimientoAsesoria = consentimiento.id_consentimiento_asesoria,
                                    idContactoAsesoria = Convert.ToInt32(cod_beneficiario),
                                    usuario = usuario
                                };
                                Afiliado afiliado = new Afiliado
                                {
                                    CUSPP = cuspp
                                };
                                servicioCotizador = LocalizadorProxy.ObtenerServicio(); ;
                                servicioCotizador.ActualizarConsentimientoAfiliado(afiliado, consentimientoAsesoria);
                                respuesta.Estado = "CU";
                                return respuesta;
                            }
                        }
                    }

                    // Universal Intercorp
                    string idConfiguracionUniversalIntercorp = Enums.TratamientoConsentimiento.IntercorpPublicidad.StringValue();
                    url = string.Format(urlConsultaConsentimientoUniversal, idConfiguracionUniversalIntercorp, beneficiario.Identificacion.IdTipo, beneficiario.Identificacion.Numero, usuario);
                    log.Debug("Consumiendo endpoint consulta consentimiento universal intercorp: " + url);

                    httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "GET";
                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        string responseBody = streamReader.ReadToEnd();
                        ConsentimientoCliente consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                        if (consentimiento != null)
                        {
                            if (consentimiento.ind_consentimiento == "S")
                            {
                                ConsentimientoAsesoria consentimientoAsesoria = new ConsentimientoAsesoria
                                {
                                    IdConsentimientoAsesoria = consentimiento.id_consentimiento_asesoria,
                                    idContactoAsesoria = Convert.ToInt32(cod_beneficiario),
                                    usuario = usuario
                                };
                                Afiliado afiliado = new Afiliado
                                {
                                    CUSPP = cuspp
                                };
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                servicioCotizador.ActualizarConsentimientoAfiliado(afiliado, consentimientoAsesoria);
                                respuesta.Estado = "CU";
                                return respuesta;
                            }
                        }
                    }
                }
            }
            else
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Advertencia.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Advertencia.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>Seleccione un contacto</strong></div>";
            }
            return respuesta;
        }

        [WebMethod]
        public static Respuesta SobrevivenciaConsentimientoAsesoria()
        {
            Respuesta respuesta = new Respuesta();

            respuesta.Mensaje = "<div class=\"alerta-agente-contenido\"><p>Usted va a enviar el enlace de consentimiento de asesoría del producto <span class=\"resaltado\">Rentas Vitalicias</span> al cliente <span class=\"resaltado\">" + HttpContext.Current.Session["ssNombres"].ToString() + " " + HttpContext.Current.Session["ssApellidosPaterno"].ToString() + " " + HttpContext.Current.Session["ssApellidosMaterno"].ToString() + " </span> identificado con el <span class=\"resaltado\"> " + HttpContext.Current.Session["ssTipoDocumento"].ToString() + " </span> <span class=\"resaltado\"> " + HttpContext.Current.Session["ssNumeroDocumento"].ToString() + "</span> al siguiente correo electrónico: <span class=\"resaltado\"> " + HttpContext.Current.Session["ssCorreo"].ToString() + "</span></p> <p> Verifique que los datos son correctos, en caso haya un error, por favor modifique los datos en el pestaña <b>Grupo Familiar</b> y el correo en el <b>vtiger</b>, luego actualice la página en el Cotizador Web de Rentas y vuelva a intentarlo.</p></div>";
            respuesta.Estado = Constante.COD_OK;

            return respuesta;
        }

        private Respuesta validacionConsentimientoAsesoria(string nombre, string apellidoPaterno, string apellidoMaterno, string tipoIdentificacion, string numeroIdentificacion, string correoElectronico, Direccion direccionPrincipal)
        {
            Respuesta respuesta = new Respuesta();
            List<string> errores = new List<string>();
            bool validacion = false;
            bool nombres = false;
            bool documento = false;
            bool correo = false;
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
            respuesta.Estado = Constante.COD_OK;

            if (nombres || documento || correo || direccion)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = "<div style=\"margin: 5px 0\">No se puede enviar el Consentimiento de Asesoría al cliente porque están faltando los siguientes datos:</div>" + Utilitarios.FormatearError(errores) + "<div>Actualice la información y recargue la página.</div>";
            }

            return respuesta;
        }

        [WebMethod]
        public static string SessionIdSolicitud(string idSolicitud, bool flagSobrevivencia)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                HttpContext.Current.Session["idSolicitud"] = idSolicitud;
                HttpContext.Current.Session["flagSobrevivencia"] = flagSobrevivencia;

                return idSolicitud.ToString();
            }
        }

        private bool consentimiento(Afiliado afiliado, string usuario, ref DateTime fechaConsentimiento, ref string consentimientoToken, ref string telefonoCliente, ref string celularCliente, ref string correoCliente, ref string tratamientoConsentimiento)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    log.Info("Accediendo a las Key necesarias");
                    string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                    string urlConsultaConsentimientoCliente = ConfigurationManager.AppSettings["url_consulta_consentimiento_id"].ToString();
                    string token_generado = string.Empty;
                    string indicadorConsentimiento = string.Empty;

                    /*Obtener token*/
                    log.Info("Consumiendo servicio token: " + urlToken);
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Method = "POST";

                    log.Info("Pasando el json al servicio - " + usuario);
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = JsonConvert.SerializeObject(new
                        {
                            usuario
                        });

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
                        if (afiliado.IdConsentimientoAsesoria != "0")
                        {
                            // Si existe un consentimiento registrado usar la información del consentimiento

                            // Obtener los datos del consentimiento
                            string urlConsentimientoPorId = ConfigurationManager.AppSettings["url_consulta_consentimiento_id"].ToString();
                            string url = string.Format(urlConsentimientoPorId, afiliado.IdConsentimientoAsesoria, usuario);

                            log.Debug("Consumiendo endpoint: " + url);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            httpWebRequest.Method = "GET";
                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));
                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            ConsentimientoCliente consentimiento = null;
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                            }

                            // Detectando consentimiento de Rentas Vitalicias
                            string urlConsentimientoPorTratamiento = ConfigurationManager.AppSettings["url_consulta_consentimiento_universal"].ToString();
                            url = string.Format(urlConsentimientoPorTratamiento, Enums.TratamientoConsentimiento.RVI.StringValue(), consentimiento.cod_tipo_identificacion, consentimiento.gls_num_identificacion, usuario);

                            log.Debug("Consumiendo endpoint: " + url);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            httpWebRequest.Method = "GET";
                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));
                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            consentimiento = null;
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                            }

                            if (consentimiento != null && consentimiento.ind_consentimiento == "S")
                            {
                                HidConsentimientoAsesoria.Value = afiliado.IdConsentimientoAsesoria;
                                if (consentimiento.fec_ultimo_consentimiento != null) fechaConsentimiento = (DateTime)consentimiento.fec_ultimo_consentimiento;
                                consentimientoToken = consentimiento.gls_token;
                                tratamientoConsentimiento = "de Rentas Vitalicias";
                                return true;
                            }

                            // Detectando consentimiento Universal de Intercorp
                            url = string.Format(urlConsentimientoPorTratamiento, Enums.TratamientoConsentimiento.IntercorpPublicidad.StringValue(), consentimiento.cod_tipo_identificacion, consentimiento.gls_num_identificacion, usuario);

                            log.Debug("Consumiendo endpoint: " + url);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            httpWebRequest.Method = "GET";
                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));
                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            consentimiento = null;
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                            }

                            if (consentimiento != null && consentimiento.ind_consentimiento == "S")
                            {
                                HidConsentimientoAsesoria.Value = afiliado.IdConsentimientoAsesoria;
                                if (consentimiento.fec_ultimo_consentimiento != null) fechaConsentimiento = (DateTime)consentimiento.fec_ultimo_consentimiento;
                                consentimientoToken = consentimiento.gls_token;
                                tratamientoConsentimiento = "Universal de Intercorp";
                                return true;
                            }

                            // Detectando consentimiento Universal de Interseguro
                            url = string.Format(urlConsentimientoPorTratamiento, Enums.TratamientoConsentimiento.InterseguroUniversal.StringValue(), consentimiento.cod_tipo_identificacion, consentimiento.gls_num_identificacion, usuario);

                            log.Debug("Consumiendo endpoint: " + url);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                            httpWebRequest.Method = "GET";
                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));
                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            consentimiento = null;
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                string responseBody = streamReader.ReadToEnd();
                                consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                            }

                            if (consentimiento != null && consentimiento.ind_consentimiento == "S")
                            {
                                HidConsentimientoAsesoria.Value = afiliado.IdConsentimientoAsesoria;
                                if (consentimiento.fec_ultimo_consentimiento != null) fechaConsentimiento = (DateTime)consentimiento.fec_ultimo_consentimiento;
                                consentimientoToken = consentimiento.gls_token;
                                tratamientoConsentimiento = "Universal de Interseguro";
                                return true;
                            }
                        }
                        else
                        {
                            // Si no existe consentimiento es porque no tiene el de RV
                            // Se debe validar si es que tiene el consentimiento universal de otro producto

                            if (Categoria.SelectedValue != Enums.CategoriaRVI.Sobrevivencia.StringValue())
                            {
                                // En caso de ser distinto a sobrevivencia buscar con los datos del afiliado

                                // Detectando consentimiento Universal de Intercorp
                                string urlConsentimientoPorTratamiento = ConfigurationManager.AppSettings["url_consulta_consentimiento_universal"].ToString();
                                string url = string.Format(urlConsentimientoPorTratamiento, Enums.TratamientoConsentimiento.IntercorpPublicidad.StringValue(), afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion, usuario);

                                log.Debug("Consumiendo endpoint: " + url);
                                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                                httpWebRequest.Method = "GET";
                                httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));
                                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                ConsentimientoCliente consentimiento = null;
                                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                                {
                                    string responseBody = streamReader.ReadToEnd();
                                    consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                                }

                                if (consentimiento != null && consentimiento.ind_consentimiento == "S")
                                {
                                    HidConsentimientoAsesoria.Value = consentimiento.id_consentimiento_asesoria.ToString();
                                    if (consentimiento.fec_ultimo_consentimiento != null) fechaConsentimiento = (DateTime)consentimiento.fec_ultimo_consentimiento;
                                    consentimientoToken = consentimiento.gls_token;
                                    tratamientoConsentimiento = "Universal de Intercorp";
                                    return true;
                                }

                                // Detectando consentimiento Universal de Interseguro
                                url = string.Format(urlConsentimientoPorTratamiento, Enums.TratamientoConsentimiento.InterseguroUniversal.StringValue(), afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion, usuario);

                                log.Debug("Consumiendo endpoint: " + url);
                                httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                                httpWebRequest.Method = "GET";
                                httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));
                                httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                consentimiento = null;
                                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                                {
                                    string responseBody = streamReader.ReadToEnd();
                                    consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                                }

                                if (consentimiento != null && consentimiento.ind_consentimiento == "S")
                                {
                                    HidConsentimientoAsesoria.Value = consentimiento.id_consentimiento_asesoria.ToString();
                                    if (consentimiento.fec_ultimo_consentimiento != null) fechaConsentimiento = (DateTime)consentimiento.fec_ultimo_consentimiento;
                                    consentimientoToken = consentimiento.gls_token;
                                    tratamientoConsentimiento = "Universal de Interseguro";
                                    return true;
                                }
                            }
                            else
                            {
                                // En caso de ser sobrevivencia, consultar todos los beneficiarios del grupo familiar
                                // en caso de que alguno de ellos tenga firmado el consentimiento universal
                                List<GrupoFamiliar> beneficiarios = servicioCotizador.ListarGrupoFamiliar(afiliado.CUSPP).FindAll(b => b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue());
                                if (beneficiarios.Count > 0)
                                {
                                    foreach (GrupoFamiliar b in beneficiarios)
                                    {
                                        if (!string.IsNullOrEmpty(b.Identificacion.IdTipo) && !string.IsNullOrEmpty(b.Identificacion.Numero))
                                        {
                                            // Detectando consentimiento Universal de Intercorp
                                            string urlConsentimientoPorTratamiento = ConfigurationManager.AppSettings["url_consulta_consentimiento_universal"].ToString();
                                            string url = string.Format(urlConsentimientoPorTratamiento, Enums.TratamientoConsentimiento.IntercorpPublicidad.StringValue(), b.Identificacion.IdTipo, b.Identificacion.Numero, usuario);

                                            log.Debug("Consumiendo endpoint: " + url);
                                            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                                            httpWebRequest.Method = "GET";
                                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));
                                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                            ConsentimientoCliente consentimiento = null;
                                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                                            {
                                                string responseBody = streamReader.ReadToEnd();
                                                consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                                            }

                                            if (consentimiento != null && consentimiento.ind_consentimiento == "S")
                                            {
                                                HidConsentimientoAsesoria.Value = consentimiento.id_consentimiento_asesoria.ToString();
                                                if (consentimiento.fec_ultimo_consentimiento != null) fechaConsentimiento = (DateTime)consentimiento.fec_ultimo_consentimiento;
                                                consentimientoToken = consentimiento.gls_token;
                                                tratamientoConsentimiento = "Universal de Intercorp";
                                                return true;
                                            }

                                            // Detectando consentimiento Universal de Interseguro
                                            url = string.Format(urlConsentimientoPorTratamiento, Enums.TratamientoConsentimiento.InterseguroUniversal.StringValue(), b.Identificacion.IdTipo, b.Identificacion.Numero, usuario);

                                            log.Debug("Consumiendo endpoint: " + url);
                                            httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                                            httpWebRequest.Method = "GET";
                                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));
                                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                            consentimiento = null;
                                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                                            {
                                                string responseBody = streamReader.ReadToEnd();
                                                consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                                            }

                                            if (consentimiento != null && consentimiento.ind_consentimiento == "S")
                                            {
                                                HidConsentimientoAsesoria.Value = consentimiento.id_consentimiento_asesoria.ToString();
                                                if (consentimiento.fec_ultimo_consentimiento != null) fechaConsentimiento = (DateTime)consentimiento.fec_ultimo_consentimiento;
                                                consentimientoToken = consentimiento.gls_token;
                                                tratamientoConsentimiento = "Universal de Interseguro";
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        return false;
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

        private void habilitaCampo(TextBox campo, bool enabled, string clase, bool readOnly)
        {
            campo.Enabled = enabled;

            campo.CssClass = clase;

            campo.ReadOnly = readOnly;
        }

        private void obtenerConsentimiento(Afiliado afiliado)
        {
            DateTime fechaConsentimiento = DateTime.Now;
            string consentimientoToken = string.Empty;
            string telefonoCliente = string.Empty;
            string celularCliente = string.Empty;
            string correoCliente = string.Empty;
            string tratammientoConsentimiento = string.Empty;

            if (afiliado.IdConsentimientoAsesoria == null || afiliado.IdConsentimientoAsesoria.Trim().Length == 0)
            {
                afiliado.IdConsentimientoAsesoria = "0";
            }

            bool ind_consentimiento = consentimiento(afiliado, (string)Session["Usuario"], ref fechaConsentimiento, ref consentimientoToken, ref telefonoCliente, ref celularCliente, ref correoCliente, ref tratammientoConsentimiento);

            HToken.Value = consentimientoToken;

            if (ind_consentimiento)
            {
                var glsfechaConsentimiento = fechaConsentimiento.ToString("dd'/'MM'/'yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                cuadroMensajeConsentimiento.tieneConsentimiento = true;
                cuadroMensajeConsentimiento.Clase = "grilla_exito";
                cuadroMensajeConsentimiento.Mensaje = "Este cliente dio su consentimiento de asesoría " + tratammientoConsentimiento + " el " + glsfechaConsentimiento + ".<br><br>";
                cuadroMensajeConsentimiento.Mensaje += "<a id=\"PlantillaConsentimientoAsesoria\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">file_download</span>Descargar formato de consentimiento de asesoría</a><br>";
                cuadroMensajeConsentimiento.Mensaje += "<a id=\"ReenviarPlantillaConsentimientoAsesoria\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">email</span>Reenviar consentimiento de asesoría</a>";
                PanelConsentimiento.Controls.Add(cuadroMensajeConsentimiento);
            }
            else
            {
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                List<Direccion> direcciones = servicioCotizador.ListarDireccion(HCUSPP.Value);
                Direccion direccion = direcciones.Find(dir => dir.Principal == true);

                HttpContext.Current.Session["ssDireccion"] = direccion;

                var resultadoValidacionConsentimiento = validacionConsentimientoAsesoria(Nombres.Text, ApellidoPaterno.Text, ApellidoMaterno.Text, TipoDocumento.SelectedValue, NumeroDocumento.Text, CorreoElectronico.Text, direccion);

                if (resultadoValidacionConsentimiento.Estado != Constante.COD_OK)
                {
                    ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                    cuadroMensajeConsentimiento.tieneConsentimiento = true;
                    cuadroMensajeConsentimiento.Clase = "mensaje_advertencia_amarillo";
                    cuadroMensajeConsentimiento.Mensaje = resultadoValidacionConsentimiento.Mensaje;
                    PanelConsentimiento.Controls.Add(cuadroMensajeConsentimiento);
                }
                else
                {
                    if (Categoria.SelectedValue == "D")
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(CUSPP.Text);
                        grupos = grupos.Where(p => p.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue()).ToList();

                        if (grupos.Count > 0)
                        {
                            ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                            cuadroMensajeConsentimiento.sobrevivencia = true;
                            cuadroMensajeConsentimiento.Clase = "mensaje_advertencia_amarillo";
                            cuadroMensajeConsentimiento.CorreoElectronico = afiliado.CorreoElectronico.ToString();
                            cuadroMensajeConsentimiento.ListaSobrevivencia = grupos;

                            // Botón de envío manual
                            if (afiliado.IdConsentimientoAsesoria != "0" && Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.PlantillaCorreoElectronico))
                            {
                                cuadroMensajeConsentimiento.Mensaje += string.Format("<br><br>En caso de que el correo con trazabilidad no llegue al cliente por problemas entre el proveedor de envío ya la casilla del cliente puede intentar <a href=\"{0}?tp=1&td={1}&nd={2}\">enviarlo de manera manual</a>. (Esta opción no cuenta con seguimiento de trazabilidad ni notificación ante rebotes)", ResolveUrl(((List<OpcionSistema>)Session["OpcionesSistema"]).Find(o => o.IdAzman == Convert.ToInt32(Enums.OpcionesSistema.PlantillaCorreoElectronico)).Ruta), afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion);
                            }

                            PanelConsentimiento.Controls.Add(cuadroMensajeConsentimiento);

                            HttpContext.Current.Session["beneficiario_sobrevivencia"] = grupos;
                        }
                        else
                        {
                            ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                            cuadroMensajeConsentimiento.tieneConsentimiento = true;
                            cuadroMensajeConsentimiento.Clase = "mensaje_advertencia_amarillo";
                            cuadroMensajeConsentimiento.Mensaje = "Este es un caso de <b>Sobrevivencia</b>. <br/>Requiere ingresar al menos un contacto en la pestaña de <b>Grupo Familiar</b> para poder enviarle el Consentimiento de Asesoría.";
                            PanelConsentimiento.Controls.Add(cuadroMensajeConsentimiento);
                        }
                    }
                    else
                    {
                        ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                        cuadroMensajeConsentimiento.tieneConsentimiento = true;
                        cuadroMensajeConsentimiento.Clase = "mensaje_advertencia_amarillo";
                        cuadroMensajeConsentimiento.Mensaje = "Este cliente no ha brindado su consentimiento de asesoría para el producto <b>Rentas Vitalicias</b>.<br><br>";
                        cuadroMensajeConsentimiento.Mensaje += "<a id=\"LinkConsentimientoAsesoriaSMS\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">email</span><span>Enviar enlace de solicitud de CDA al celular <b>" + afiliado.Celulares.ToString() + "</b></span></a>";
                        cuadroMensajeConsentimiento.Mensaje += "<br>";
                        cuadroMensajeConsentimiento.Mensaje += "<a id=\"LinkConsentimientoAsesoria\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">email</span><span>Enviar enlace de solicitud de CDA al correo <b>" + afiliado.CorreoElectronico.ToString() + "</b></span></a>";

                        // Botón de envío manual
                        if (afiliado.IdConsentimientoAsesoria != "0" && Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.PlantillaCorreoElectronico))
                        {
                            cuadroMensajeConsentimiento.Mensaje += string.Format("<br><br>En caso de que el correo con trazabilidad no llegue al cliente por problemas con el proveedor de envío puede intentar <a href=\"{0}?tp=1&td={1}&nd={2}\">enviarlo de manera manual</a> (Esta opción no cuenta con seguimiento de trazabilidad ni notificación ante rebotes)", ResolveUrl(((List<OpcionSistema>)Session["OpcionesSistema"]).Find(o => o.IdAzman == Convert.ToInt32((object)Enums.OpcionesSistema.PlantillaCorreoElectronico)).Ruta), afiliado.TipoIdentificacion, afiliado.NumeroIdentificacion);
                        }

                        PanelConsentimiento.Controls.Add(cuadroMensajeConsentimiento);
                    }
                }

                //if (afiliado.Telefonos.Trim().Length > 0 && afiliado.Telefonos != null)
                //{
                //    Telefono.Text = afiliado.Telefonos;
                //}
                //else 
                //if (cont.Length > 0)
                //{
                //    if (cont[0].inter_telefonofijo.Trim().Length > 0 && cont[0].inter_telefonofijo.ToString() != "NULL")
                //    {
                //        Telefono.Text = cont[0].inter_telefonofijo.ToString();
                //        afiliado.Telefonos = cont[0].inter_telefonofijo.ToString();
                //    }
                //    else if (afiliado.Telefonos.Trim().Length > 0 && afiliado.Telefonos != null)
                //    {
                //        Telefono.Text = afiliado.Telefonos;
                //    }
                //}
                //else 
                if (afiliado.Telefonos.Trim().Length > 0 && afiliado.Telefonos != null)
                {
                    Telefono.Text = afiliado.Telefonos;
                }

                //if (afiliado.Celulares.Trim().Length > 0 && afiliado.Celulares != null)
                //{
                //    Celular.Text = afiliado.Celulares;
                //}
                //else 
                //if (cont.Length > 0)
                //{
                //    if (cont[0].mobilephone.Trim().Length > 0 && cont[0].mobilephone.ToString() != "NULL")
                //    {
                //        Celular.Text = cont[0].mobilephone.ToString();
                //        afiliado.Celulares = cont[0].mobilephone.ToString();
                //    }
                //    else if (afiliado.Celulares.Trim().Length > 0 && afiliado.Celulares != null)
                //    {
                //        Celular.Text = afiliado.Celulares;
                //    }
                //}
                //else 
                if (afiliado.Celulares.Trim().Length > 0 && afiliado.Celulares != null)
                {
                    Celular.Text = afiliado.Celulares;
                }

                ControlLabel(Telefono);
                ControlLabel(Celular);
                ControlLabel(CorreoElectronico);

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

        //<FIN.GTI_26560>

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
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
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
                                    nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                }
                                catch (Exception)
                                {
                                    log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                        HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                }

                                nombreTerminal += HttpContext.Current.Request.UserAgent;

                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                    NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                    Detalle = String.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "EliminarGrupoFamiliar", Environment.NewLine, Environment.NewLine, "Grupo Familiar", JsonConvert.SerializeObject(gf)),
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
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
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
                            //JArray respuestas = (JArray)JsonConvert.DeserializeObject(jsonResult);
                            //respuesta = respuestas.ToObject<T>();
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
        public static string DescargarVCTP(string tokenUsuario, string tokenFirmaDigital, string solicitud)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        log.Debug(string.Format("Se va a descargar formato VCTP. Token[{0}] Solicitud[{1}]", tokenFirmaDigital, solicitud));
                        string urlBaseApi = ConfigurationManager.AppSettings["url_base_api_cwrv"].ToString();
                        string usuario = HttpContext.Current.Session["Usuario"].ToString();
                        string tokenApi = string.Empty;

                        string endpoint = string.Format("{0}/token", urlBaseApi);
                        log.Info(string.Format("Endpoint: POST [{0}]", endpoint));
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "POST";

                        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                        {
                            string requestBody = JsonConvert.SerializeObject(new
                            {
                                usuario
                            });

                            streamWriter.Write(requestBody);
                            streamWriter.Flush();
                            streamWriter.Close();
                        }

                        var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            string responseBody = streamReader.ReadToEnd();
                            tokenApi = ((dynamic)JsonConvert.DeserializeObject(responseBody)).accessToken;
                            log.Debug(string.Format("Token del API [{0}]", tokenApi));
                        }

                        endpoint = string.Format("{0}/firmas-digitales/formato-consentimiento/{1}/{2}", urlBaseApi, tokenFirmaDigital, usuario); ConfigurationManager.AppSettings["url_formato_solicitud_rpp"].ToString();
                        string nombreArchivo = string.Format("FormatoVCTP_{0}.pdf", solicitud);
                        endpoint = string.Format(endpoint, solicitud, usuario);

                        byte[] formatoBinario = Utilitarios.ConsumirServicio(endpoint, usuario, tokenApi);

                        return Convert.ToBase64String(formatoBinario, 0, formatoBinario.Length);
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Se ha producido un error al descargar el formato VCTP", ex);
                    throw ex;
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
                    string urlCrearSesion = ConfigurationManager.AppSettings["url_crear_sesion_rviadm"].ToString();
                    string urlPdfPoliza = ConfigurationManager.AppSettings["url_pdf_emision_poliza_rviadm"].ToString();

                    string usuario = HttpContext.Current.Session["Usuario"].ToString();

                    using (var httpClient = new HttpClient())
                    {
                        var objRequest = new
                        {
                            usuario,
                            token = numPoliza
                        };

                        log.Info("Consumiendo método para crear sesión");
                        var content = JsonConvert.SerializeObject(objRequest);
                        var response = httpClient.PostAsync(urlCrearSesion, new StringContent(content, Encoding.UTF8, "application/json")).Result;
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        var resultadoCorrecto = response.IsSuccessStatusCode;

                        if (resultadoCorrecto)
                        {
                            var wc = new WebClient();
                            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + numPoliza));
                            wc.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

                            log.Info("Consumiendo método para generar pdf de póliza");
                            urlPdfPoliza = string.Format(urlPdfPoliza, "RVI", numPoliza);
                            byte[] polizaByteArray = wc.DownloadData(urlPdfPoliza);
                            wc.Dispose();

                            var nombreArchivo = "Poliza.pdf";
                            var rutaCarpeta = HostingEnvironment.MapPath("~") + "\\ArchivosTemporales\\RVI\\Poliza\\";
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
                        }
                        else
                        {
                            throw new Exception("Error al crear sesión en rviadm-admin. " + responseContent);
                        }
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error($"Se ha producido el siguiente error: [{ex.Message}]", ex);
                    Respuesta respuesta = new Respuesta
                    {
                        Estado = Constante.COD_ERROR,
                        Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                        Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                        Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message })
                    };
                    return respuesta;
                }
            }
        }

        protected void MAAEliminar_Click(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    AporteAdicional aporte = new AporteAdicional();
                    aporte.num_cuispp = CUSPP.Text;
                    aporte.ind_vigencia = "N";
                    respuesta = servicioCotizador.EliminarAporteAdicional(aporte, (string)Session["Usuario"]);

                    if (respuesta.Estado == Constante.COD_OK)
                    {
                        txtPensionRef.Text = string.Empty;
                        ddlMonedaReferencia.SelectedIndex = 0;
                        txtTasaAporte.Text = string.Empty;
                        dtpFechaPago.Text = string.Empty;
                        txtMontoAporte.Text = string.Empty;
                        txtPensionPago.Text = string.Empty;
                        ddlMonedaPensionPago.SelectedIndex = 0;
                        txtPensionElegida.Text = string.Empty;

                        MCMMensaje.Text = "Datos del afiliado actualizados correctamente.";
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Exito.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                        MCMEstado.Value = "1";
                    }
                    else
                    {
                        log.Error("No se pudo actualizar la información de afiliado: [" + respuesta.Mensaje + "]");
                        MCMMensaje.Text = "No se pudo actualizar la información: [" + respuesta.Mensaje + "]";
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstado.Value = "1";
                    }

                }
                catch (Exception ex)
                {
                    log.Error("No se pudo actualizar la información de afiliado: [" + ex.Message + "]");
                    MCMMensaje.Text = "No se pudo actualizar la información: [" + ex.Message + "]";
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";

                }
            }
        }
    }

    public class ParametrosEnvioCDA
    {
        public string nombres { get; set; }
        public string tipoDocumento { get; set; }
        public string numeroDocumento { get; set; }
        public string correo { get; set; }
        public string cuspp { get; set; }
        public string token { get; set; }
        public string categoria { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string sexo { get; set; }
        public string fechaNacimiento { get; set; }
        //public string afp { get; set; }
        public string telefono { get; set; }
        public string celular { get; set; }
        //public string idConsentimientoAsesoria { get; set; }
        //public string indConsentimiento { get; set; }
        public string glsCategoria { get; set; }
        public string canalComunicacion { get; set; }
    }
}
