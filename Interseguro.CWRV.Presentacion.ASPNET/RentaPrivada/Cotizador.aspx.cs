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
                    //<SRIINI20322>
                    // Validar si es un navegador móvil
                    //if (Request.Browser.IsMobileDevice)
                    //{
                    //    Response.Redirect("~/Cotizador/Cotizador.Movil.aspx");
                    //}
                    //<SRIFIN20322>

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
                                BusAfiCUSPP_RP.Text = Session["CUSPP"].ToString();
                                BusAfiBuscar_RP_Click(sender, e);
                            }
                            else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                            {
                                BusAfiNroSolicitud_RP.Text = Session["NroSolicitud"].ToString();
                                BusAfiBuscar_RP_Click(sender, e);
                            }
                        }
                        else
                        {
                            // Se vuelve a formatear el monto sin separador de miles para que el plugin autoNumeric no falle
                            if (SaldoCIC_RP.Text != String.Empty) SaldoCIC_RP.Text = Convert.ToDouble(SaldoCIC_RP.Text, new CultureInfo("es-PE")).ToString();
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

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            CargarCombobox(AFP_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Afp]);
            CargarCombobox(Categoria_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Categoria]);
            CargarCombobox(Sexo_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);
            //CargarCombobox(ModDirDepartamento, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Departamento]);
            //CargarCombobox(ModDirCiudad, new List<Parametro>());
            //CargarCombobox(ModDirComuna, new List<Parametro>());
            //CargarCombobox(ModTelTipo, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Telefono]);
            CargarCombobox(CiudadEmpresa_RP, new List<Parametro>());
            CargarCombobox(ComunaEmpresa_RP, new List<Parametro>());

            ////CargarCombobox(ModGruFamTipoIdentificacion_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);
            ////CargarCombobox(ModGruFamParentesco_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Parentesco]);
            ////CargarCombobox(ModGruFamSexo_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);
            ////CargarCombobox(ModGruFamTipoInvalidez_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Invalidez]);

            //CargarCombobox(ModSolTipoPension, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Prestacion]);
            //CargarCombobox(ModSolCategoria, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Categoria]);
            //CargarCombobox(ModSolFactorTasa, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.FactorTRA]);

            Session["ComboMoneda"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda];

            List<Parametro> comboModalidad = new List<Parametro>();
            comboModalidad.Add(new Parametro { Id = "I", Glosa = "I" });
            comboModalidad.Add(new Parametro { Id = "D", Glosa = "D" });
            comboModalidad.Add(new Parametro { Id = "I-RM", Glosa = "I-RM" });
            //<SRIINI18360>
            comboModalidad.Add(new Parametro { Id = "I-RC", Glosa = "I-RC" });
            //<SRIFIN18360>
            comboModalidad.Add(new Parametro { Id = "I-RB", Glosa = "I-RB" });
            Session["ComboModalidad"] = comboModalidad;

            List<Parametro> comboPeriodoDiferido = new List<Parametro>();
            comboPeriodoDiferido.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPeriodoDiferido.Add(new Parametro { Id = "1", Glosa = "1" });
            comboPeriodoDiferido.Add(new Parametro { Id = "2", Glosa = "2" });
            //<SRIINI18360>
            comboPeriodoDiferido.Add(new Parametro { Id = "3", Glosa = "3" });
            comboPeriodoDiferido.Add(new Parametro { Id = "4", Glosa = "4" });
            comboPeriodoDiferido.Add(new Parametro { Id = "5", Glosa = "5" });
            //<SRIFIN18360>
            Session["ComboPeriodoDiferido"] = comboPeriodoDiferido;

            List<Parametro> comboPorcentajeRentas = new List<Parametro>();
            comboPorcentajeRentas.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPorcentajeRentas.Add(new Parametro { Id = "50", Glosa = "50%" });
            Session["ComboPorcentajeRentas"] = comboPorcentajeRentas;

            List<Parametro> comboPeriodoGarantizado = new List<Parametro>();
            comboPeriodoGarantizado.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "10", Glosa = "10" });
            comboPeriodoGarantizado.Add(new Parametro { Id = "15", Glosa = "15" });
            Session["ComboPeriodoGarantizado"] = comboPeriodoGarantizado;

            Session["ComboCapital"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Capital];

            //CargarCombobox("Invalidez", (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Invalidez]);
            //CargarCombobox("Modalidad", (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Modalidad]);
            //CargarCombobox("Parentesco", (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Parentesco]);

            //CargarCombobox("TipoCotizacion", (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.TipoCotizacion]);

            //ModDirPrincipal.Items.Add(new ListItem("«Seleccione»", "0"));
            //ModDirPrincipal.Items.Add(new ListItem("Sí", "S"));
            //ModDirPrincipal.Items.Add(new ListItem("No", "N"));

            //ModTelPrincipal.Items.Add(new ListItem("«Seleccione»", "0"));
            //ModTelPrincipal.Items.Add(new ListItem("Sí", "S"));
            //ModTelPrincipal.Items.Add(new ListItem("No", "N"));

            ////ModGruFamIndInvalidez_RP.Items.Add(new ListItem("«Seleccione»", "0"));
            ////ModGruFamIndInvalidez_RP.Items.Add(new ListItem("Sí", "S"));
            ////ModGruFamIndInvalidez_RP.Items.Add(new ListItem("No", "N"));

            //<SRIINI06326>
            var montosCIC = servicioCotizador.ListarMontoCIC();
            //CargarCombobox(ListaCIC, montosCIC);
            //CargarCombobox(ModSolListaCIC, montosCIC);
            //<SRIFIN06326>

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            //LabModSolLineaACOMDCOM.Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);

            // Permisos Modal Búsqueda de Afiliados
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

            // Permisos Consultar Afiliado
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

            // Permisos Actualizar Afiliado
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoActualizar))
            {
                PerGuardar_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(CorreoElectronico_RP);
                InhabilitarControl(Categoria_RP);
                InhabilitarControl(AFP_RP);
                InhabilitarControl(SaldoCIC_RP);
                InhabilitarControl(Guardar_RP);
                PerGuardar_RP.Value = "0";
            }

            // Permisos Insertar Dirección
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionInsertar))
            {
                PerNuevaDireccion_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevaDireccion_RP);
                PerNuevaDireccion_RP.Value = "0";
            }

            // Permisos Insertar Teléfono
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoInsertar))
            {
                PerNuevoTelefono_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevoTelefono_RP);
                PerNuevoTelefono_RP.Value = "0";
            }

            // Permisos Insertar Grupo Familiar
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
            {
                PerNuevoBeneficiario_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevoBeneficiario_RP);
                PerNuevoBeneficiario_RP.Value = "0";
            }

            // Permisos Insertar Solicitud
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudInsertar))
            {
                PerNuevaSolicitud_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevaSolicitud_RP);
                PerNuevaSolicitud_RP.Value = "0";
            }

            //<SRIINI10693>
            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;
            //<SRIFIN10693>
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
        //<SRIINI06326>
        private void CargarCombobox(DropDownList control, List<MontoCIC> combobox)
        {
            control.Items.Clear();
            foreach (MontoCIC item in combobox)
            {
                control.Items.Add(new ListItem(String.Format("{0:#,##0.00}", item.Valor), item.Valor.ToString()));
            }
        }
        //<SRIFIN06326>

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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        List<String> errores = new List<String>();
                        List<String> controles = new List<String>();
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
                                log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
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
        public static string CargarTablaDirecciones(string tokenUsuario, string cuspp)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaDirecciones)pagina.LoadControl("~/Controles/TablaDirecciones.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Direccion> direcciones = servicioCotizador.ListarDireccion(cuspp);

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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaGrupoFamiliar)pagina.LoadControl("~/Controles/TablaGrupoFamiliar.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(cuspp);
                            //<INIGTI_753>
                            control.Grupos = grupos.Where(p => p.Parentesco.Id != "95").ToList();
                            //<FINGTI_753>
                            control.PermisoConsultar = true;
                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar)) ? true : false;
                            //<SRIINI06326>
                            control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                            //<SRIFIN06326>
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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesRentaPrivada)pagina.LoadControl("~/Controles/TablaSolicitudesRentaPrivada.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<SolicitudRP> solicitudes = servicioCotizador.ListarSolicitudRP(cuspp);

                            control.Solicitudes = solicitudes;
                            control.PermisoConsultar = true;

                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudActualizar)) ? true : false;
                            control.PermisoCorreoElectronico = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudEnviarCorreo)) ? true : false;
                            control.PermisoExportarPDF = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudExportarPDF)) ? true : false;
                            //<SRIINI06326>
                            control.PermisoReporteEscenario = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudReporteEscenarios)) ? true : false; ;
                            control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                            //<SRIFIN06326>

                            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                            //control.RedLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarTablaSolicitudesSimulador(string tokenUsuario, string cuspp)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
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
        public static string CargarTablaCotizaciones(List<CotizacionRP> cotizaciones, string temporalidad, string moneda)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();
                    var control = (TablaCotizacionesRentaPrivada)pagina.LoadControl("~/Controles/TablaCotizacionesRentaPrivada.ascx");

                    int iTemporalidad = 9999;
                    switch (temporalidad)
                    {
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

                    foreach (CotizacionRP cotizacion in cotizaciones)
                    {
                        if (cotizacion.PeriodoGarantizado > iTemporalidad)
                        {
                            cotizacion.PeriodoGarantizado = iTemporalidad;
                        }
                    }
                    control.CotizacionesRP = cotizaciones;
                    control.Moneda = moneda;
                    control.Temporalidad = iTemporalidad;

                    // Validando si el acceso es desde dentro de la red de Interseguro o desde Internet
                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                    {
                        control.PermisoTRA = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoTRA)) ? true : false;
                    }
                    else
                    {
                        control.PermisoTRA = false;
                    }

                    control.PermisoAgregar = true;
                    control.PermisoModificar = true;
                    control.PermisoEliminar = true;

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
        public static string CargarTablaCotizacionesSimulador(string tokenUsuario, List<Cotizacion> cotizaciones, int idSimulador, int filtro, bool movil, string modalidad, string moneda, int? periodoGarantizado)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    if (beneficiarios == null)
                    {
                        var control = (TablaBeneficiariosRentaPrivada)pagina.LoadControl("~/Controles/TablaBeneficiariosRentaPrivada.ascx");
                        List<GrupoFamiliar> grupoFamiliar = servicioCotizador.ListarGrupoFamiliar(cuspp);
                        grupoFamiliar.ForEach(g => g.Seleccionado = true);

                        //<INIGTI_753>
                        control.Beneficiarios = grupoFamiliar.Where(p => p.Parentesco.Id != "95").ToList();
                        //<FINGTI_753>
                        control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                        HttpContext.Current.Session["Beneficiarios"] = control.Beneficiarios;
                        pagina.Controls.Add(control);
                    }
                    else
                    {
                        var control = (TablaRviBenefiRentaPrivada)pagina.LoadControl("~/Controles/TablaRviBenefiRentaPrivada.ascx");
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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
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

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Ciudad> ciudades = servicioCotizador.ListarCiudad(idDepartamento);

            //control.Ciudades = ciudades;

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

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Comuna> comunas = servicioCotizador.ListarComuna(idCiudad);

            //control.Comunas = comunas;

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
            //Cargar combobox de Productos
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Producto> productos = servicioCotizador.ListarProducto(idTemporalidad);
            HttpContext.Current.Session["ComboProducto"] = productos;
        }

        [WebMethod]
        public static Direccion ObtenerDatosDireccion(int idDireccion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Direccion dir;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                dir = servicioCotizador.ObtenerDatosDireccion(idDireccion);
                return dir;
            }
        }

        [WebMethod]
        public static String SessionIdMonedaFondo(string idMonedaFondo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                HttpContext.Current.Session["idMonedaFondo"] = idMonedaFondo;

                List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                HttpContext.Current.Session["ComboMoneda"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda];

                //if (idDireccion == 0)
                //{
                //    HttpContext.Current.Session["ModDirModo"] = "N";
                //}
                //else
                //{
                //    HttpContext.Current.Session["ModDirModo"] = "M";
                //}

                return idMonedaFondo.ToString();
            }
        }

        [WebMethod]
        public static String SessionIdDreccion(int idDireccion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
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
        public static String SessionIdSolicitud(string idSolicitud, string fecCotizacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                HttpContext.Current.Session["idSolicitud"] = idSolicitud;
                HttpContext.Current.Session["fecCotizacion"] = fecCotizacion;
                if (idSolicitud == "")
                {
                    HttpContext.Current.Session["ModSolModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModSolModo"] = "M";
                }

                return idSolicitud.ToString();
            }
        }

        [WebMethod]
        public static String SessionIdTelefono(int idTelefono)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
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
        public static String SessionIdGrupoFamiliar(int idGrupoFamiliar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
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
        public static Respuesta InsertarDireccion(string tokenUsuario, string cuspp, string direccion, string idDepartamento, string idCiudad, string idComuna, string idPrincipal)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionInsertar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarDireccion(direccion, idDepartamento, idCiudad, idComuna, idPrincipal, errores, controles))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    Direccion dir = new Direccion
                                    {
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Glosa = direccion,
                                        Ciudad = new Ciudad { Id = idCiudad },
                                        Comuna = new Comuna { Id = idComuna },
                                        Principal = (idPrincipal == "S") ? true : false,
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
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
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
        public static Respuesta ModificarDireccion(string tokenUsuario, int idDireccion, string cuspp, string direccion, string idDepartamento, string idCiudad, string idComuna, string idPrincipal)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionActualizar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarDireccion(direccion, idDepartamento, idCiudad, idComuna, idPrincipal, errores, controles))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    Direccion dir = new Direccion
                                    {
                                        Id = Convert.ToInt32(idDireccion),
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Glosa = direccion,
                                        Ciudad = new Ciudad { Id = idCiudad },
                                        Comuna = new Comuna { Id = idComuna },
                                        Principal = (idPrincipal == "S") ? true : false,
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
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
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
                                Enums.OpcionesSistema.DireccionActualizar.StringValue()));
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
        public static Respuesta EliminarDireccion(string tokenUsuario, int idDireccion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionEliminar))
                        {
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
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DireccionEliminar.StringValue()));
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
        public static Telefono ObtenerDatosTelefono(int idTelefono)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Telefono tel;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                tel = servicioCotizador.ObtenerDatosTelefono(idTelefono);
                return tel;
            }
        }

        [WebMethod]
        public static Respuesta InsertarTelefono(string tokenUsuario, string cuspp, string idTipo, string numero, string idPrincipal)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoInsertar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarTelefono(idTipo, numero, idPrincipal, errores, controles))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    Telefono tel = new Telefono
                                    {
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Tipo = new TipoTelefono { Id = idTipo },
                                        Numero = numero,
                                        Principal = (idPrincipal == "S") ? true : false,
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.RegistrarTelefono(tel);
                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                    respuesta.Controles = controles;
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
                                Enums.OpcionesSistema.TelefonoInsertar.StringValue()));
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
        public static Respuesta ModificarTelefono(string tokenUsuario, int idTelefono, string cuspp, string idTipo, string numero, string idPrincipal)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoActualizar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarTelefono(idTipo, numero, idPrincipal, errores, controles))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    Telefono tel = new Telefono
                                    {
                                        Id = Convert.ToInt32(idTelefono),
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Tipo = new TipoTelefono { Id = idTipo },
                                        Numero = numero,
                                        Principal = (idPrincipal == "S") ? true : false,
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.ActualizarTelefono(tel);
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
                                Enums.OpcionesSistema.TelefonoActualizar.StringValue()));
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
        public static Respuesta EliminarTelefono(string tokenUsuario, int idTelefono)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoEliminar))
                        {
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                Telefono tel = new Telefono
                                {
                                    Id = Convert.ToInt32(idTelefono),
                                    Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                };

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                servicioCotizador.EliminarTelefono(tel);
                                respuesta.Estado = Constante.COD_OK;
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
                                Enums.OpcionesSistema.TelefonoEliminar.StringValue()));
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
        public static GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
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
                                Enums.OpcionesSistema.GrupoFamiliarInsertar.StringValue()));
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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
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
                                Enums.OpcionesSistema.GrupoFamiliarActualizar.StringValue()));
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
        public static SolicitudRP CrearDatosSolicitud()
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudRP sol = new SolicitudRP();

                sol.FechaSolicitud = DateTime.Now;
                sol.Cotizaciones = new List<CotizacionRP>();

                CotizacionRP cot;

                SeccionCotizacionesRP config = (SeccionCotizacionesRP)ConfigurationManager.GetSection("cotizacionesRP");

                HttpContext.Current.Session["idMonedaFondo"] = "001";

                foreach (ElementoCotizacionRP cotizacion in config.CotizacionesRP)
                {


                    cot = new CotizacionRP
                    {
                        Moneda = new Moneda { Id = cotizacion.Moneda },
                        //Producto = new Producto { Id = cotizacion.Producto },
                        PeriodoGarantizado = Convert.ToInt32(cotizacion.PeriodoGarantizado),
                        AjusteTRA = 0

                        //Modalidad = new Modalidad { Id = cotizacion.Modalidad },
                        //PeriodoDiferido = Convert.ToInt32(cotizacion.PeriodoDiferido),
                        //PorcentajeEntreRentas = Convert.ToInt32(cotizacion.PjeEntreRentras),

                        //DerechoCrecer = false,
                        //Gratificacion = (cotizacion.Gratificacion == "S") ? true : false,
                        //Capital = new Capital { Id = cotizacion.Capital },
                        //<SRIINI12770>

                        //<SRIFIN12770>
                    };

                    //if ((string)HttpContext.Current.Session["idMonedaFondo"] == cotizacion.MonedaFondo)
                    //{

                    //}
                    sol.Cotizaciones.Add(cot);



                }

                return sol;
            }
        }

        [WebMethod]
        public static SolicitudRP ObtenerDatosSolicitud(string idSolicitud, string fecCotizacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                fecCotizacion = fechaCotizacion.ToString("yyyyMMdd");

                SolicitudRP sol;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                sol = servicioCotizador.ObtenerDatosSolicitudRP(idSolicitud, fechaCotizacion);

                //<SOLINI26593>
                HttpContext.Current.Session["idMonedaFondo"] = sol.MonedaPrimaUnica.Id.ToString();//sol.MonedaPrimaUnica.Id.ToString();
                //<SOLINI26593>
                return sol;
            }
        }

        [WebMethod]
        public static List<CotizacionRP> AgregarCotizacionASolicitud(List<CotizacionRP> cot)
        {
            CotizacionRP cotizacion = new CotizacionRP
            {
                Moneda = new Moneda { Id = "0" },
                //Producto = new Producto { Id = "0" },
                //Modalidad = new Modalidad { Id = "0" },
                //PeriodoDiferido = 0,
                //PorcentajeEntreRentas = 0,
                PeriodoGarantizado = 0,
                //DerechoCrecer = false,
                //Gratificacion = false,
                //Capital = new Capital { Id = "-" },
                //<SRIINI12770>
                AjusteTRA = 0
                //<SRIFIN12770>
            };
            cot.Add(cotizacion);
            return cot;
        }

        [WebMethod]
        public static SolicitudRP InsertarSolicitud(string tokenUsuario,
                                                    string cuspp,
                                                    string afp,
                                                    string temporalidad,
                                                    string monedaPrimaUnica,
                                                    string primaUnica,
                                                    string fechaCotizacion,
                                                    string fechaDevengue,
                                                    string dcom,
                                                    List<CotizacionRP> cotizaciones,
                                                    List<int> idBeneficiarios)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudRP sol;
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudInsertar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            if (ValidarSolicitud(errores, controles, fechaCotizacion, fechaDevengue, primaUnica, dcom, cotizaciones, idBeneficiarios, cuspp, (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    sol = new SolicitudRP
                                    {
                                        Afiliado = new Afiliado { CUSPP = cuspp, AFP = new AFP { Id = afp } },
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
                                        TipoCotizacion = new TipoCotizacion { Id = Enums.TipoCotizacion.RentaPrivada.StringValue() },
                                        Temporalidad = new Temporalidad { Id = temporalidad },
                                        Cotizaciones = cotizaciones
                                    };

                                    // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                                    {
                                        sol.PorcentajeDescuentoComision = Convert.ToDouble(dcom, new CultureInfo("es-PE"));
                                    }
                                    else
                                    {
                                        sol.PorcentajeDescuentoComision = null;
                                    }

                                    List<GrupoFamiliar> lben = new List<GrupoFamiliar>();
                                    idBeneficiarios.ForEach(id => lben.Add(((List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"])[id]));

                                    sol.Beneficiarios = lben;

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.RegistrarSolicitudRP(ref sol);

                                    // Guardar en log de auditoría
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

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                        IdTipoEvento = Enums.EventoLog.CotizarSolicitud.StringValue(),
                                        Detalle = "Solicitud registrada: " + sol.Id
                                    });
                                }
                                else
                                {
                                    sol = new SolicitudRP();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                sol = new SolicitudRP();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            sol = new SolicitudRP();
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudInsertar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        sol = new SolicitudRP();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    sol.Respuesta = respuesta;
                    return sol;
                }
                catch (Exception ex)
                {
                    sol = new SolicitudRP();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    sol.Respuesta = respuesta;
                    return sol;
                }
            }
        }

        [WebMethod]
        public static SolicitudRP ModificarSolicitud(string tokenUsuario,
                                                     string idSolicitud,
                                                     string cuspp,
                                                     string afp,
                                                     string temporalidad,
                                                     string monedaPrimaUnica,
                                                     string primaUnica,
                                                     string fechaCotizacion,
                                                     string fechaDevengue,
                                                     string dcom,
                                                     List<CotizacionRP> cotizaciones,
                                                     List<int> idBeneficiarios)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudRP sol;
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudActualizar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            if (ValidarSolicitud(errores, controles, fechaCotizacion, fechaDevengue, primaUnica, dcom, cotizaciones, idBeneficiarios, cuspp, (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    sol = new SolicitudRP
                                    {
                                        Id = idSolicitud,
                                        Afiliado = new Afiliado { CUSPP = cuspp, AFP = new AFP { Id = afp } },
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
                                        TipoCotizacion = new TipoCotizacion { Id = Enums.TipoCotizacion.RentaPrivada.StringValue() },
                                        Temporalidad = new Temporalidad { Id = temporalidad },
                                        Cotizaciones = cotizaciones
                                    };

                                    // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                                    {
                                        sol.PorcentajeDescuentoComision = Convert.ToDouble(dcom, new CultureInfo("es-PE"));
                                    }
                                    else
                                    {
                                        sol.PorcentajeDescuentoComision = null;
                                    }

                                    List<GrupoFamiliar> lben = new List<GrupoFamiliar>();
                                    idBeneficiarios.ForEach(id => lben.Add(((List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"])[id]));

                                    sol.Beneficiarios = lben;

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.ActualizarSolicitudRP(ref sol);

                                    // Guardar en log de auditoría
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

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                        IdTipoEvento = Enums.EventoLog.CotizarSolicitud.StringValue(),
                                        Detalle = "Solicitud modificada: " + sol.Id
                                    });
                                }
                                else
                                {
                                    sol = new SolicitudRP();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                sol = new SolicitudRP();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            sol = new SolicitudRP();
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudActualizar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        sol = new SolicitudRP();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    sol.Respuesta = respuesta;
                    return sol;
                }
                catch (Exception ex)
                {
                    sol = new SolicitudRP();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    sol.Respuesta = respuesta;
                    return sol;
                }
            }
        }

        [WebMethod]
        public static Respuesta ExportarSolicitudPDF(string idSolicitud, string fecCotizacion, string numAgente)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
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
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static CorreoElectronico CrearDatosCorreo(string tokenUsuario, string idSolicitud, string fecCotizacion, string tipoCotizacion, string nombre, string apellidoPaterno, string apellidoMaterno, string sexo)
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
                                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                                fecCotizacion = fechaCotizacion.ToString("yyyyMMdd");
                                string num_lote = "1";
                                string grupo1 = "GRUPO1";
                                string grupo2 = "GRUPO2";
                                string id_benefi = "1";

                                ReportViewer visorReporte = new ReportViewer();
                                visorReporte.ProcessingMode = ProcessingMode.Remote;
                                visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                //<SOLINI26593>
                                visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteDetallePropuesta"];
                                //<SOLFIN26593>

                                ReportParameter p1 = new ReportParameter("wl_solicitud", idSolicitud);
                                ReportParameter p2 = new ReportParameter("wl_fec_cotizacion", fecCotizacion);
                                ReportParameter p3 = new ReportParameter("wl_num_lote", num_lote);
                                ReportParameter p4 = new ReportParameter("wl_grupo", grupo1);
                                ReportParameter p5 = new ReportParameter("wl_grupo2", grupo2);
                                ReportParameter p6 = new ReportParameter("wl_id_benefi", id_benefi);
                                //<SOLINI26593>
                                log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                    ConfigurationManager.AppSettings["DominioReportingServices"],
                                    ConfigurationManager.AppSettings["RutaReporteDetallePropuesta"]));
                                //<SOLFIN26593>
                                log.Debug(String.Format("Parámetros del reporte: wl_solicitud[{0}] wl_fec_cotizacion[{1}] wl_num_lote[{2}] wl_grupo1[{3}] wl_grupo2[{4}] wl_id_benefi[{5}].",
                                    idSolicitud, fecCotizacion, num_lote, grupo1, grupo2, id_benefi));
                                visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2, p3, p4, p5, p6 });
                                log.Debug(String.Format("Reporte para solicitud [{0}] procesado.", idSolicitud));

                                string format = "PDF", mimeType, encoding, extension;
                                string[] streamids;
                                Warning[] warnings;

                                log.Debug(String.Format("Se va a exportar a formato PDF el reporte para solicitud [{0}].", idSolicitud));
                                byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                                HttpContext.Current.Session["ArchivoPDF"] = bytes;
                                log.Debug(String.Format("Reporte para solicitud [{0}] exportado y almacenado en sesión de usuario.", idSolicitud));

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
                                //<SRIINI20322>
                                //MailMessage mensaje = new MailMessage();

                                //mensaje.To.Add(new MailAddress(correo.Para, correo.ParaNombre, System.Text.Encoding.UTF8));
                                //mensaje.Bcc.Add(new MailAddress(correo.De, correo.DeNombre, System.Text.Encoding.UTF8));
                                //mensaje.From = new MailAddress(correo.De, correo.DeNombre, System.Text.Encoding.UTF8);
                                //mensaje.Subject = correo.Asunto;
                                //mensaje.SubjectEncoding = System.Text.Encoding.UTF8;
                                //mensaje.Body = correo.Mensaje;
                                //mensaje.BodyEncoding = System.Text.Encoding.UTF8;
                                //mensaje.Attachments.Add(new Attachment(new MemoryStream((byte[])HttpContext.Current.Session["ArchivoPDF"]), "Cotizacion.pdf"));

                                //log.Info(String.Format("Se va a establecer conexión con el Servidor SMTP[{0}] Puerto[{1}].",
                                //    ConfigurationManager.AppSettings["DominioSMTP"],
                                //    ConfigurationManager.AppSettings["PuertoSMTP"]));
                                //SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["DominioSMTP"], Convert.ToInt32(ConfigurationManager.AppSettings["PuertoSMTP"]));

                                ////client.EnableSsl = true;
                                //client.UseDefaultCredentials = true;
                                ////client.Credentials = credenciales;
                                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                //log.Debug(String.Format("Usuario va a enviar correo electrónico con cotización adjunta a la dirección[{0} <{1}>].",
                                //    correo.ParaNombre, correo.Para));
                                //client.Send(mensaje);
                                //log.Info(String.Format("Correo electrónico enviado correctamente a la dirección[{0} <{1}>].",
                                //    correo.ParaNombre, correo.Para));

                                //string nombreTerminal = String.Empty;
                                //try
                                //{
                                //    nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                //}
                                //catch (Exception)
                                //{
                                //    log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                //        HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                //}

                                //nombreTerminal += HttpContext.Current.Request.UserAgent;

                                //servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                //servicioCotizador.RegistrarLog(new LogBD
                                //{
                                //    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                //    NombreTerminal = nombreTerminal,
                                //    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                //    NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                //    IdTipoEvento = Enums.EventoLog.EnviarCorreoElectronico.StringValue(),
                                //    Detalle = String.Format("Solicitud {0} enviada a {1} ({2})", HttpContext.Current.Session["idSolicitud"], correo.ParaNombre, correo.Para)
                                //});

                                //respuesta.Estado = Constante.COD_OK;
                                //respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                //respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                //respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Correo electrónico enviado correctamente." });

                                correo.Adjunto = "Cotizacion.pdf";
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
        public static Actividad ObtenerDatosActividad(string cuspp, string idActividad)
        {
            return ((List<Actividad>)HttpContext.Current.Session["Actividades"]).Find(x => x.Correlativo == idActividad);
        }

        [WebMethod]
        public static Afiliado ObtenerDatosAfiliado(string tokenUsuario, string nroSolicitud, string cuspp)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Afiliado afiliado = null;
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarBusquedaAfiliados(errores, controles, nroSolicitud, cuspp))
                            {
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                afiliado = servicioCotizador.ObtenerDatosAfiliado(nroSolicitud, cuspp, "", "", "");

                                log.Info("Usuario realizó búsqueda de afiliados por "
                                    + ((nroSolicitud.Trim().Length != 0)
                                    ? ("Solicitud [" + nroSolicitud.ToUpper() + "]")
                                    : ("CUSPP [" + cuspp.ToUpper() + "]")) + ".");

                                if (afiliado != null)
                                {
                                    // Validar si el usuario tiene permiso para visualizar los datos del afiliado
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
                                        afiliado.Respuesta = new Respuesta();
                                        afiliado.Respuesta.Estado = Constante.COD_ERROR;
                                        afiliado.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                        afiliado.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                        afiliado.Respuesta.Mensaje = Utilitarios.FormatearError(errores);
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
                                    afiliado = new Afiliado();
                                    afiliado.Respuesta = new Respuesta();
                                    afiliado.Respuesta.Estado = Constante.COD_ERROR;
                                    afiliado.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Informacion.StringValue();
                                    afiliado.Respuesta.Icono = Enums.CuadroMensajeIcono.Informacion.StringValue();
                                    afiliado.Respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                }
                            }
                            else
                            {
                                afiliado = new Afiliado();
                                afiliado.Respuesta = new Respuesta();
                                afiliado.Respuesta.Estado = Constante.COD_ERROR;
                                afiliado.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                afiliado.Respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                afiliado.Respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                afiliado.Respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DatosAfiliadoConsultar.StringValue()));
                            afiliado = new Afiliado();
                            afiliado.Respuesta = new Respuesta();
                            afiliado.Respuesta.Estado = Constante.COD_ERROR;
                            afiliado.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            afiliado.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            afiliado.Respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        afiliado = new Afiliado();
                        afiliado.Respuesta = new Respuesta();
                        afiliado.Respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    afiliado = new Afiliado();
                    afiliado.Respuesta = new Respuesta();
                    afiliado.Respuesta.Estado = Constante.COD_ERROR;
                    afiliado.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    afiliado.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    afiliado.Respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
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
                    afiliado = new Afiliado();
                    afiliado.Respuesta = new Respuesta();
                    afiliado.Respuesta.Estado = Constante.COD_ERROR;
                    afiliado.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    afiliado.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    afiliado.Respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message });
                }
                return afiliado;
            }
        }

        //<SRIINI06326>
        [WebMethod]
        public static Respuesta ExportarReporteEscenariosPDF(string tokenUsuario, string idSolicitud, string fecCotizacion, string maxAcom)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudReporteEscenarios))
                        {
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.GenerarReporteEscenarios(idSolicitud, fechaCotizacion, (string)HttpContext.Current.Session["Usuario"], maxAcom);

                                //<SRI.INI-20322_E2>
                                string nombreTerminal = String.Empty;

                                if (respuesta.Estado == Constante.COD_OK)
                                {
                                    //<SRI.FIN-20322_E2>

                                    ReportViewer visorReporte = new ReportViewer();
                                    visorReporte.ProcessingMode = ProcessingMode.Remote;
                                    visorReporte.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["DominioReportingServices"]);
                                    visorReporte.ServerReport.ReportPath = ConfigurationManager.AppSettings["RutaReporteEscenarios"];

                                    ReportParameter p1 = new ReportParameter("wl_num_solcitud", idSolicitud);
                                    ReportParameter p2 = new ReportParameter("wl_cod_username", (string)HttpContext.Current.Session["Usuario"]);

                                    log.Info(String.Format("Se va a establecer comunicación con el servidor Reporting Services [{0}] Reporte [{1}].",
                                            ConfigurationManager.AppSettings["DominioReportingServices"],
                                            ConfigurationManager.AppSettings["RutaReporteEscenarios"]));
                                    log.Debug(String.Format("Parámetros del reporte: wl_num_solcitud[{0}] wl_cod_username[{1}].",
                                            idSolicitud, (string)HttpContext.Current.Session["Usuario"]));
                                    visorReporte.ServerReport.SetParameters(new ReportParameter[] { p1, p2 });
                                    log.Debug(String.Format("Reporte para solicitud [{0}] procesado.", idSolicitud));

                                    string mimeType, encoding, extension;
                                    string[] streamids;
                                    Warning[] warnings;

                                    string format = "PDF";
                                    log.Debug(String.Format("Se va a exportar a formato PDF el reporte para solicitud [{0}].", idSolicitud));
                                    byte[] bytes = visorReporte.ServerReport.Render(format, "", out mimeType, out encoding, out extension, out streamids, out warnings);
                                    HttpContext.Current.Session["EscenariosPDF"] = bytes;
                                    log.Debug(String.Format("Reporte para solicitud [{0}] exportado y almacenado en sesión de usuario.", idSolicitud));

                                    //<SRI.INI-20322_E2>
                                    //string nombreTerminal = String.Empty;
                                    //<SRI.FIN-20322_E2>
                                    //<SRI.INI-20322_E2>
                                }
                                else if (respuesta.Estado == Constante.COD_ERROR)
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { respuesta.Mensaje });
                                }
                                //<SRI.FIN-20322_E2>

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

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                    NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                    IdTipoEvento = Enums.EventoLog.ReporteEscenarios.StringValue(),
                                    Detalle = String.Format("Escenarios para solicitud {0} exportados a formato PDF", idSolicitud)
                                });

                                //<SRI.INI-20322_E2>
                                //respuesta.Estado = Constante.COD_OK;
                                //<SRI.FIN-20322_E2>
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
                                    Enums.OpcionesSistema.SolicitudReporteEscenarios.StringValue()));
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

                }
                catch (Exception ex)
                {
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }
        //<SRIFIN06326>  


        protected void BusAfiBuscar_RP_Click(object sender, EventArgs e)
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
                            Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(BusAfiNroSolicitud_RP.Text, BusAfiCUSPP_RP.Text, "", "", "");

                            log.Info("Usuario realizó búsqueda de afiliados por "
                                + ((BusAfiNroSolicitud_RP.Text.Trim().Length != 0)
                                ? ("Solicitud [" + BusAfiNroSolicitud_RP.Text.ToUpper() + "]")
                                : ("CUSPP [" + BusAfiCUSPP_RP.Text.ToUpper() + "]")) + ".");

                            if (afiliado != null)
                            {
                                //<SRIINI06326>

                                // MasterPage
                                Panel cabecera, cabeceraProtegida;

                                cabecera = (Panel)Master.FindControl("CabeceraSuperior");
                                cabeceraProtegida = (Panel)Master.FindControl("CabeceraSuperiorProtegida");

                                Session["Consentimiento"] = afiliado.Consentimiento;
                                if (!afiliado.Consentimiento)
                                {
                                    // Guardar los datos del afiliado para generarlo como beneficiario en caso de que no exista
                                    servicioCotizador.ActualizarAfiliado(afiliado);

                                    // MastePage
                                    cabecera.Visible = false;
                                    cabeceraProtegida.Visible = true;

                                    // Datos del Afiliado
                                    BusquedaAfiliados_RP.Visible = false;
                                    LineaCUSPP_RP.Visible = false;
                                    afiliado.ApellidoPaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoPaterno);
                                    afiliado.ApellidoMaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoMaterno);
                                    afiliado.Nombre = Utilitarios.EnmascararNombre(afiliado.Nombre);
                                    LineaNacimientoSexo_RP.Visible = false;
                                    LineaCorreoElectronico_RP.Visible = false;
                                    LineaCategoria_RP.Visible = false;
                                    LineaSaldoCIC_RP.Visible = false;
                                    //LineaListaCIC.Visible = true;
                                    LineaAFP_RP.Visible = false;
                                    GrupoDireccion_RP.Visible = false;
                                    GrupoTelefono_RP.Visible = false;
                                    GrupoEmpresa_RP.Visible = false;

                                    ContenedorGuardar_RP.Visible = false;

                                    ////// Grupo Familiar
                                    ////ModGruFamLineaApellidos_RP.Visible = false;
                                    ////ModGruFamLineaNombres_RP.Visible = false;
                                    ////ModGruFamLineaIdentificacion_RP.Visible = false;
                                    ////ModGruFamCargando_RP.Height = 130;

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
                                    LineaSaldoCIC_RP.Visible = true;
                                    //LineaListaCIC.Visible = false;

                                    // Solicitudes
                                    //ModSolSaldoCIC.Visible = true;
                                    //ModSolListaCIC.Visible = false;
                                }
                                //<SRIFIN06326>

                                // Validar si el usuario tiene permiso para visualizar los datos del afiliado
                                if (((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == afiliado.Agente.Id))
                                {
                                    if (BusAfiCUSPP_RP.Text.Trim().Length > 0)
                                    {
                                        Session["CUSPP"] = BusAfiCUSPP_RP.Text;
                                        Session["NroSolicitud"] = null;
                                        //<INIGTI_753>
                                        Session["CUSPP_PRIVADA"] = null;
                                        //<FINGTI_753>
                                    }
                                    else if (BusAfiNroSolicitud_RP.Text.Trim().Length > 0)
                                    {
                                        Session["CUSPP"] = null;
                                        Session["NroSolicitud"] = BusAfiNroSolicitud_RP.Text;
                                        //<INIGTI_753>
                                        Session["CUSPP_PRIVADA"] = afiliado.CUSPP.Trim();
                                        //<FINGTI_753>
                                    }

                                    // Datos principales
                                    CUSPP_RP.Text = afiliado.CUSPP.Trim();
                                    HCUSPP_RP.Value = afiliado.CUSPP.Trim();
                                    ApellidoPaterno_RP.Text = afiliado.ApellidoPaterno.Trim();
                                    ApellidoMaterno_RP.Text = afiliado.ApellidoMaterno.Trim();
                                    Nombres_RP.Text = afiliado.Nombre.Trim();
                                    FechaNacimiento_RP.Text = afiliado.FechaNacimiento.Value.ToString("dd/MM/yyyy");
                                    Sexo_RP.SelectedIndex = Sexo_RP.Items.IndexOf(Sexo_RP.Items.FindByValue(afiliado.Sexo.ToString()));
                                    CorreoElectronico_RP.Text = afiliado.CorreoElectronico;
                                    CorreoElectronicoRegistrado_RP.Value = afiliado.CorreoElectronico;
                                    Categoria_RP.SelectedIndex = Categoria_RP.Items.IndexOf(Categoria_RP.Items.FindByValue(afiliado.Categoria.Id.ToString()));
                                    HCategoria_RP.Value = afiliado.Categoria.Id.ToString();
                                    HAFP_RP.Value = afiliado.AFP.Id.ToString();
                                    AFP_RP.SelectedIndex = AFP_RP.Items.IndexOf(AFP_RP.Items.FindByValue(afiliado.AFP.Id.ToString()));
                                    SaldoCIC_RP.Text = afiliado.SaldoCIC.ToString();
                                    Session["Vendedor"] = afiliado.Agente.Id;
                                    Session["Cartera"] = afiliado.Agente.IdCartera;
                                    Session["AFP_RP"] = afiliado.AFP.Id.ToString();
                                    Session["CUSPP_RP"] = afiliado.CUSPP.ToString();

                                    // Direcciones
                                    NuevaDireccion_RP.Visible = true;

                                    // Teléfonos
                                    NuevoTelefono_RP.Visible = true;

                                    // Datos de empresa
                                    NombreEmpresa_RP.Text = afiliado.NombreEmpresa.Trim();
                                    DireccionEmpresa_RP.Text = afiliado.DireccionEmpresa.Trim();

                                    // Botón Guardar Afiliado
                                    if (afiliado.Consentimiento)
                                        ContenedorGuardar_RP.Visible = true;
                                    else
                                        ContenedorGuardar_RP.Visible = false;

                                    // Grupo Familiar
                                    NuevoBeneficiario_RP.Visible = true;

                                    // Solicitudes
                                    NuevaSolicitud_RP.Visible = true;

                                    ModEnvCorPara.Text = afiliado.CorreoElectronico;

                                    List<Ciudad> listaCiudades = new List<Ciudad>();
                                    Ciudad ciudad = servicioCotizador.ObtenerDatosCiudad(afiliado.CiudadEmpresa.Id);
                                    if (ciudad != null)
                                    {
                                        listaCiudades.Add(ciudad);
                                    }
                                    CargarCombobox(CiudadEmpresa_RP, listaCiudades);
                                    CiudadEmpresa_RP.SelectedIndex = CiudadEmpresa_RP.Items.IndexOf(CiudadEmpresa_RP.Items.FindByValue(afiliado.CiudadEmpresa.Id));

                                    List<Comuna> listaComunas = new List<Comuna>();
                                    Comuna comuna = servicioCotizador.ObtenerDatosComuna(afiliado.ComunaEmpresa.Id);
                                    if (comuna != null)
                                    {
                                        listaComunas.Add(comuna);
                                    }
                                    CargarCombobox(ComunaEmpresa_RP, listaComunas);
                                    ComunaEmpresa_RP.SelectedIndex = ComunaEmpresa_RP.Items.IndexOf(ComunaEmpresa_RP.Items.FindByValue(afiliado.ComunaEmpresa.Id));

                                    TelefonoEmpresa_RP.Text = afiliado.TelefonoEmpresa;

                                    //ModSolTipoPension.SelectedIndex = ModSolTipoPension.Items.IndexOf(ModSolTipoPension.Items.FindByValue("V"));

                                    HttpContext.Current.Session["indConsentimiento"] = afiliado.Consentimiento;  //JY

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

        private void LimpiarFormularios()
        {
            // Datos principales
            CUSPP_RP.Text = String.Empty;
            HCUSPP_RP.Value = String.Empty;
            ApellidoPaterno_RP.Text = String.Empty;
            ApellidoMaterno_RP.Text = String.Empty;
            Nombres_RP.Text = String.Empty;
            FechaNacimiento_RP.Text = String.Empty;
            Sexo_RP.SelectedIndex = Sexo_RP.Items.IndexOf(Sexo_RP.Items.FindByValue("0"));
            CorreoElectronico_RP.Text = String.Empty;
            CorreoElectronicoRegistrado_RP.Value = String.Empty;
            Categoria_RP.SelectedIndex = Categoria_RP.Items.IndexOf(Categoria_RP.Items.FindByValue("0"));
            HCategoria_RP.Value = "0";
            AFP_RP.SelectedIndex = AFP_RP.Items.IndexOf(AFP_RP.Items.FindByValue("0"));
            HAFP_RP.Value = "0";
            SaldoCIC_RP.Text = String.Empty;
            Session["Vendedor"] = null;
            Session["Cartera"] = null;

            CorreoElectronico_RP.CssClass = CorreoElectronico_RP.CssClass.Replace(" formTextboxError", String.Empty);
            Categoria_RP.CssClass = Categoria_RP.CssClass.Replace(" formComboboxError", String.Empty);
            AFP_RP.CssClass = AFP_RP.CssClass.Replace(" formComboboxError", String.Empty);
            SaldoCIC_RP.CssClass = SaldoCIC_RP.CssClass.Replace(" formTextboxError", String.Empty);

            // Direcciones
            NuevaDireccion_RP.Visible = false;

            // Teléfonos
            NuevoTelefono_RP.Visible = false;

            // Datos de empresa
            NombreEmpresa_RP.Text = String.Empty;
            DireccionEmpresa_RP.Text = String.Empty;
            CiudadEmpresa_RP.SelectedIndex = CiudadEmpresa_RP.Items.IndexOf(CiudadEmpresa_RP.Items.FindByValue("0"));
            ComunaEmpresa_RP.SelectedIndex = ComunaEmpresa_RP.Items.IndexOf(ComunaEmpresa_RP.Items.FindByValue("0"));
            TelefonoEmpresa_RP.Text = String.Empty;

            // Botón Guardar Afiliado
            ContenedorGuardar_RP.Visible = false;

            // Grupo Familiar
            NuevoBeneficiario_RP.Visible = false;

            // Solicitudes
            NuevaSolicitud_RP.Visible = false;
            ModEnvCorDe.Text = String.Empty;
            ModEnvCorPara.Text = String.Empty;
            ModEnvCorAsunto.Text = String.Empty;
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

        private static bool ValidarBusquedaAfiliados(List<String> errores, List<String> controles, string nroSolicitud, string nroCuspp)
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

        private static bool ValidarDireccion(string glsDireccion, string idDepartamento, string idCiudad, string idComuna, string idPrincipal, List<String> errores, List<String> controles)
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
                errores.Add("Ingrese el campo <strong>Ciudad</strong>. Dato Obligatorio.");
                ciudad = false;
            }

            // Comuna
            bool comuna = true;
            if (idComuna == "0")
            {
                errores.Add("Ingrese el campo <strong>Comuna</strong>. Dato Obligatorio.");
                comuna = false;
            }

            // Principal
            bool principal = true;
            if (idPrincipal == "0")
            {
                errores.Add("Ingrese el campo <strong>Principal</strong>. Dato Obligatorio.");
                principal = false;
            }

            // Clases de controles
            if (!direccion) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!departamento) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!ciudad) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!comuna) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!principal) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }

            esCorrecto = direccion & departamento & ciudad & comuna & principal;

            return esCorrecto;
        }

        private static bool ValidarTelefono(string idTipo, string numTelefono, string idPrincipal, List<String> errores, List<String> controles)
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

        private static bool ValidarGrupoFamiliar(List<String> errores, List<String> controles, string glsApellidoPaterno, string glsApellidoMaterno, string glsNombres, string idTipoIdentificacion, string glsNumeroIdentificacion, string idParentesco, string idSexo, string fecNacimiento, string idInvalidez, string idTipoInvalidez, string fecInvalidez)
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

        private static bool ValidarSolicitud(List<String> errores, List<String> controles, string fecCotizacion, string fecDevengue, string valPrimaUnica, string valDcom, List<CotizacionRP> listaCotizaciones, List<int> idBeneficiarios, string cusspp, string numAgenteSol)
        {
            bool esCorrecto;

            // Prima Única
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

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            bool redLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);

            bool valRequisitosTra = true;
            List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];
            string numAgente = numAgenteSol;

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
                    if (valDcom.Trim().Length == 0) valDcom = "0";
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

            /* Implementacion ACOM, solamente cuando al configuracion sea S */
            if (fechaCotizacion)
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
                string dcomRangos = String.Empty;
                if (dcom && KeyDcom == "S" && listaRolDcom.Count > 0)
                {
                    if (valDcom.Trim().Length == 0) valDcom = "0";

                    double vDcomDouble = Convert.ToDouble(valDcom, new CultureInfo("es-PE"));
                    if (vDcomDouble != vDcomComp)
                    {
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
                    }
                }
                if (!dcom)
                {
                    errores.Add(String.Format("El campo <strong>Porcentaje D</strong> no se encuentra dentro de los rangos permitidos ({0}).", dcomRangos.TrimEnd()));
                }
            }

            // Clases de controles
            if (!primaUnica) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!fechaCotizacion) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!fechaDevengue) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!dcom) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            // Cotizaciones
            bool cotizaciones = true;
            if (!(listaCotizaciones.Count > 0))
            {
                errores.Add("Debe realizar al menos una cotización.");
                cotizaciones = false;
            }

            for (int i = 0; i < listaCotizaciones.Count; i++)
            {
                // Moneda
                if (listaCotizaciones[i].Moneda.Id == "0")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Moneda</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    controles.Add(i + ",2");
                }

                // Ajuste TRA
                if (listaCotizaciones[i].AjusteTRA.ToString().Length == 0)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Dif. TRA</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    controles.Add(i + ",4");
                }
            }

            // Beneficiarios
            bool beneficiarios = true;
            if (!(idBeneficiarios.Count > 0))
            {
                errores.Add("Debe seleccionar al menos un beneficiario para realizar la cotización.");
                beneficiarios = false;
            }

            esCorrecto = primaUnica & fechaCotizacion & fechaDevengue & dcom & cotizaciones & beneficiarios;

            return esCorrecto;
        }

        protected void Guardar_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoActualizar))
                    {
                        if (((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == (string)Session["Vendedor"]))
                        {
                            if (ValidarAfiliado())
                            {
                                Afiliado afiliado = new Afiliado
                                {
                                    CUSPP = CUSPP_RP.Text,
                                    CorreoElectronico = CorreoElectronico_RP.Text,
                                    Categoria = new Categoria { Id = Categoria_RP.SelectedValue },
                                    AFP = new AFP { Id = AFP_RP.SelectedValue },
                                    //<SRIINI06326>
                                    //SaldoCIC = Convert.ToDouble(SaldoCIC.Text, new CultureInfo("es-PE"))
                                    SaldoCIC = ((bool)Session["Consentimiento"]) ? (double?)Convert.ToDouble(SaldoCIC_RP.Text, new CultureInfo("es-PE")) : null
                                    //<SRIFIN06326>
                                };

                                HCategoria_RP.Value = Categoria_RP.SelectedValue;
                                HAFP_RP.Value = AFP_RP.SelectedValue;

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                Respuesta respuesta = servicioCotizador.ActualizarAfiliado(afiliado);

                                log.Info(String.Format("Usuario actualizó los datos del afiliado CUSPP[{0}].", CUSPP_RP.Text));
                                log.Debug(String.Format("Correo Electrónico[{0}] Categoría[{1}:{2}] AFP[{3}:{4}] Saldo CIC[{5}].",
                                    CorreoElectronico_RP.Text,
                                    Categoria_RP.SelectedValue, Categoria_RP.SelectedItem.Text,
                                    AFP_RP.SelectedValue, AFP_RP.SelectedItem.Text,
                                    SaldoCIC_RP.Text));

                                string nombreTerminal = String.Empty;
                                try
                                {
                                    nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                }
                                catch (Exception)
                                {
                                    log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
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
                                    IdTipoEvento = Enums.EventoLog.ModificarDatosCliente.StringValue()
                                });

                                if (respuesta.Estado == Constante.COD_OK)
                                {
                                    MCMMensaje.Text = "Datos del afiliado actualizados correctamente.";
                                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Exito.StringValue();
                                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                    MCMEstado.Value = "1";

                                    CorreoElectronicoRegistrado_RP.Value = afiliado.CorreoElectronico;
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
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.DatosAfiliadoActualizar.StringValue()));
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstado.Value = "1";
                    }
                }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
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
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }

        private static bool ValidarDatosAfiliado(List<String> errores, List<String> controles, string glsApellidoPaterno, string glsApellidoMaterno, string glsNombres)
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
            CorreoElectronico_RP.CssClass = "formTextbox";
            Categoria_RP.CssClass = "formCombobox";
            AFP_RP.CssClass = "formCombobox";
            SaldoCIC_RP.CssClass = "formTextbox";

            // Correo Electrónico
            bool correoElectronico = true;
            if ((bool)Session["Consentimiento"])
            {
                //if (ConfigurationManager.AppSettings["ValidacionesCorreo"] == "S")
                //{
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
                //}
            }

            // Categoría
            bool categoria = true;
            if (Categoria_RP.SelectedValue == "0")
            {
                errores.Add("Ingrese el campo <strong>Categoría</strong>. Dato Obligatorio.");
                categoria = false;
            }

            // AFP
            bool afp = true;
            if (AFP_RP.SelectedValue == "0")
            {
                errores.Add("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
                afp = false;
            }

            // Saldo CIC
            bool saldoCIC = true;
            if ((bool)Session["Consentimiento"])
            {
                if (SaldoCIC_RP.Text.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Saldo CIC</strong>. Dato Obligatorio.");
                    saldoCIC = false;
                }
                else
                {
                    double vSaldoCIC;
                    if (!double.TryParse(SaldoCIC_RP.Text, NumberStyles.Any, new CultureInfo("es-PE"), out vSaldoCIC))
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
            if (!correoElectronico) { CorreoElectronico_RP.CssClass = "formTextbox formTextboxError"; } else { CorreoElectronico_RP.CssClass = "formTextbox formTextboxReadOnly"; }
            if (!categoria) { Categoria_RP.CssClass = "formCombobox formComboboxError"; } else { Categoria_RP.CssClass = "formCombobox formComboboxReadOnly"; }
            if (!afp) { AFP_RP.CssClass = "formCombobox formComboboxError"; } else { AFP_RP.CssClass = "formCombobox"; }
            if (!saldoCIC) { SaldoCIC_RP.CssClass = "formTextbox formTextboxError numerico"; } else { SaldoCIC_RP.CssClass = "formTextbox numerico"; }

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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
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
        //    using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
        //    {
        //        Solicitud sol;
        //        try
        //        {
        //            Respuesta respuesta = new Respuesta();

        //            if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
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
        //            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
        //            return respuesta;
        //        }
        //    }
        //}
    }

}