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
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using System.Web.Hosting;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
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
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.RentaParticularPlus))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
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
                            if (SaldoCIC_RP.Text != string.Empty) SaldoCIC_RP.Text = Convert.ToDouble(SaldoCIC_RP.Text, new CultureInfo("es-PE")).ToString();
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.RentaParticularPlus.StringValue()));
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
            log.Debug("Inicio Cotizador.ObtenerCombobox");
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
            log.Debug("Fin Cotizador.ObtenerCombobox");

            CargarCombobox(AFP_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Afp]);
            CargarComboboxenBlanco(Categoria_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Categoria]);
            CargarComboboxenBlanco(Sexo_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);

            //oculto
            //CargarCombobox(CiudadEmpresa_RP, new List<Parametro>());
            //CargarCombobox(ComunaEmpresa_RP, new List<Parametro>());

            CargarComboboxenBlanco(TipoDocumento_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);

            string usuario = HttpContext.Current.Session["Usuario"].ToString();

            Session["ListadoEstadoCivil"] = servicioCotizador.ListarEstadoCivil(usuario);

            Session["ListadoProfesion"] = servicioCotizador.ListarProfesion(usuario);
            Session["ListadoNacionalidad"] = servicioCotizador.ListarNacionalidad(usuario);

            JArray listaDepartamentos = new JArray();
            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
            var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
            urlDepartamentos = string.Format(urlDepartamentos, usuario);
            listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, usuario, listaDepartamentos);
            Session["ListadoDepartamento"] = listaDepartamentos.ToObject<List<Departamento>>();

            CargarComboboxNuevosDatosenBlanco(EstadoCivil_RP, "EstadoCivil");

            //List<ParametroGeneral> comboConfidencialidadDatos = new List<ParametroGeneral>();
            //comboConfidencialidadDatos.Add(new ParametroGeneral { Codigo = "0", Descripcion = "Si" });
            //comboConfidencialidadDatos.Add(new ParametroGeneral { Codigo = "1", Descripcion = "No" });

            //ConfidencialidadDatos_RP.Items.Clear();

            //foreach (ParametroGeneral item in comboConfidencialidadDatos)
            //{
            //    ConfidencialidadDatos_RP.Items.Add(new ListItem(item.Descripcion, item.Codigo));
            //}

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

                InhabilitarControl(RangoInversion_RP);
                InhabilitarControl(CentroLaboral_RP);
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
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusInsertar))
            {
                HabilitarControl(NuevaSolicitud_RP);
                PerNuevaSolicitud_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(NuevaSolicitud_RP);
                PerNuevaSolicitud_RP.Value = "0";
            }
            
            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
            hdKeyAcom.Value = KeyAcom;
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
                control.Items.Add(new ListItem(String.Format("{0:#,##0.00}", item.Valor), item.Valor.ToString()));
            }
        }

        private void CargarComboboxNuevosDatosenBlanco(DropDownList control, string tabla)
        {

            control.Items.Clear();
            control.Items.Add(new ListItem("", "0"));

            if (tabla == "EstadoCivil")
            {
                List<EstadoCivil> listaCombobox = null;

                listaCombobox = (List<EstadoCivil>)Session["ListadoEstadoCivil"];

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
        public void HabilitarControl(Control control)
        {
            /*
            if (control is TextBox)
            {
                ((TextBox)control).ReadOnly = true;
                ((TextBox)control).CssClass = "formTextbox formTextboxReadOnly";
            }
            if (control is DropDownList)
            {
                ((DropDownList)control).Enabled = false;
                ((DropDownList)control).CssClass = "formCombobox formComboboxReadOnly";
            }*/
            if (control is HyperLink)
            {
                ((HyperLink)control).CssClass = "boton darkblue sharp";
            }
            /*
            if (control is Button)
            {
                ((Button)control).CssClass = "botonDeshabilitado gris gris_sharp";
                ((Button)control).ToolTip = ConfigurationManager.AppSettings["MensajeSinPermisos"];
            }
            */
        }


        [WebMethod]
        public static Respuesta CargarTablaAfiliados(string tokenUsuario, string apellidoPaterno, string apellidoMaterno, string nombres, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio Cotizador.CargarTablaAfiliados WebMethod");

                    Respuesta respuesta = new Respuesta();
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
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
                                log.Debug("Inicio Cotizador.servicioCotizador.ListarAfiliado WebMethod");
                                List<Afiliado> afiliados = servicioCotizador.ListarAfiliado(apellidoPaterno, apellidoMaterno, nombres, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar, ref totalRegistros);
                                log.Debug("Fin Cotizador.servicioCotizador.ListarAfiliado WebMethod");

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

                    log.Debug("Fin Cotizador.CargarTablaAfiliados WebMethod");

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
                    log.Debug("Inicio Cotizador.CargarTablaDirecciones WebMethod");

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaDirecciones)pagina.LoadControl("~/Controles/TablaDirecciones.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();

                            log.Debug("Inicio Cotizador.servicioCotizador.ListarDireccion WebMethod");
                            List<Direccion> direcciones = servicioCotizador.ListarDireccion(cuspp);
                            log.Debug("Fin Cotizador.servicioCotizador.ListarDireccion WebMethod");

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

                        log.Debug("Fin Cotizador.CargarTablaDirecciones WebMethod");

                        return html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        log.Debug("Fin Cotizador.CargarTablaDirecciones WebMethod");
                        return Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    log.Debug("Fin Cotizador.CargarTablaDirecciones WebMethod");
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
                    log.Debug("Inicio Cotizador.CargarTablaTelefonos WebMethod");

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaTelefonos)pagina.LoadControl("~/Controles/TablaTelefonos.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            log.Debug("Inicio Cotizador.servicioCotizador.ListarTelefono WebMethod");
                            List<Telefono> telefonos = servicioCotizador.ListarTelefono(cuspp);
                            log.Debug("Fin Cotizador.servicioCotizador.ListarTelefono WebMethod");

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

                        log.Debug("Fin Cotizador.CargarTablaTelefonos WebMethod");

                        return html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        log.Debug("Fin Cotizador.CargarTablaTelefonos WebMethod");

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
                    log.Debug("Inicio Cotizador.CargarTablaGrupoFamiliar WebMethod");

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaGrupoFamiliar)pagina.LoadControl("~/Controles/TablaGrupoFamiliar.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            log.Debug("Inicio Cotizador.servicioCotizador.ListarGrupoFamiliar WebMethod");
                            List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(cuspp);
                            log.Debug("Fin Cotizador.servicioCotizador.ListarGrupoFamiliar WebMethod");

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

                                        log.Debug("Inicio Cotizador.servicioCotizador.ObtenerCoincidenciaLN Plaft");
                                        JsonCoincidenciaLN jsonCoincidencia = servicioCotizador.ObtenerCoincidenciaLN(gru);
                                        log.Debug("Fin Cotizador.servicioCotizador.ObtenerCoincidenciaLN Plaft");

                                        if (jsonCoincidencia != null)
                                        {
                                            if (jsonCoincidencia._meta.status == "SUCCESS")
                                            {
                                                if (jsonCoincidencia.records.LN != 0)
                                                {
                                                    //divListaNegra.Visible = true;
                                                    control.ListaNegra = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception listaNegra)
                            {
                                log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                                    listaNegra.Source, listaNegra.Message, listaNegra.StackTrace));

                                if (listaNegra.InnerException != null)
                                {
                                    log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
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

                        log.Debug("Fin Cotizador.CargarTablaGrupoFamiliar WebMethod");

                        return html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        log.Debug("Fin Cotizador.CargarTablaGrupoFamiliar WebMethod");

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
        public static string CargarTablaSolicitudes(string tokenUsuario, string cuspp, bool mostrarTodos)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio Cotizador.CargarTablaSolicitudes WebMethod");

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesRentaPrivadaPlus)pagina.LoadControl("~/Controles/TablaSolicitudesRentaPrivadaPlus.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            log.Debug("Inicio Cotizador.servicioCotizador.ListarSolicitudRPPlus WebMethod");
                            List<SolicitudRPPlus> solicitudes = servicioCotizador.ListarSolicitudRPPlus(cuspp);
                            log.Debug("Fin Cotizador.servicioCotizador.ListarSolicitudRPPlus WebMethod");

                            //<INI.GTI_7012_3>
                            if (!mostrarTodos)
                            {
                                DateTime fechaActual = DateTime.Today;
                                solicitudes = solicitudes.FindAll(p => p.FechaVigencia >= fechaActual);
                            }

                            //<FIN.GTI_7012_3>
                            control.Solicitudes = solicitudes;

                            control.PermisoInsertar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusInsertar)) ? true : false;

                            control.PermisoConsultar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusConsultar)) ? true : false;
                            control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusActualizar)) ? true : false;
                            control.PermisoCorreoElectronico = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusEnviarCorreo)) ? true : false;
                            control.PermisoExportarPDF = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusExportarPDF)) ? true : false;
                            control.PermisoReporteEscenario = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudReporteEscenarios)) ? true : false; ;
                            control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                            control.PermisoCerrar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusCerrar)) ? true : false;

                            control.PermisoObtenerEdN = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ObtenerFormatoEstudioNecesidades)) ? true : false;

                            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                            //control.RedLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);

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

                        log.Debug("Fin Cotizador.CargarTablaSolicitudes WebMethod");

                        return html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        log.Debug("Fin Cotizador.CargarTablaSolicitudes WebMethod");

                        return Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw ex;
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
                    log.Debug("Inicio Cotizador.CargarTablaSolicitudesSimulador WebMethod");

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesSimulador)pagina.LoadControl("~/Controles/TablaSolicitudesSimulador.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            log.Debug("Inicio Cotizador.servicioCotizador.ListarSolicitud WebMethod");
                            List<Solicitud> solicitudes = servicioCotizador.ListarSolicitud(cuspp);
                            log.Debug("Fin Cotizador.servicioCotizador.ListarSolicitud WebMethod");

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

                        log.Debug("Fin Cotizador.CargarTablaSolicitudesSimulador WebMethod");

                        return html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        log.Debug("Fin Cotizador.CargarTablaSolicitudesSimulador WebMethod");

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
        public static string CargarTablaCotizacionesSimulador(string tokenUsuario, List<Cotizacion> cotizaciones, int idSimulador, int filtro, bool movil, string modalidad, string moneda, int? periodoGarantizado)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio Cotizador.CargarTablaSolicitudesSimulador WebMethod");

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

                        control.Cotizaciones = cotizaciones;
                        pagina.Controls.Add(control);

                        string html = "";
                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            html = sw.ToString();
                        }

                        log.Debug("Fin Cotizador.CargarTablaSolicitudesSimulador WebMethod");

                        return html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        log.Debug("Fin Cotizador.CargarTablaSolicitudesSimulador WebMethod");

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
        public static string CargarTablaBeneficiarios(string cuspp, List<GrupoFamiliar> beneficiarios, string conyuge)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio Cotizador.CargarTablaBeneficiarios WebMethod");

                    var pagina = new Page();

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    if (beneficiarios == null)
                    {
                        var control = (TablaBeneficiariosRentaPrivada)pagina.LoadControl("~/Controles/TablaBeneficiariosRentaPrivada.ascx");
                        log.Debug("Inicio Cotizador.servicioCotizador.ListarGrupoFamiliar WebMethod");
                        List<GrupoFamiliar> grupoFamiliar = servicioCotizador.ListarGrupoFamiliar(cuspp);
                        log.Debug("Fin Cotizador.servicioCotizador.ListarGrupoFamiliar WebMethod");

                        grupoFamiliar.ForEach(g => g.Seleccionado = true);
                        control.Beneficiarios = grupoFamiliar;
                        control.Consentimiento = (bool)HttpContext.Current.Session["Consentimiento"];
                        HttpContext.Current.Session["Beneficiarios"] = control.Beneficiarios;

                        control.Conyuge = (conyuge == "TRUE") ? true : false;

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

                    log.Debug("Fin Cotizador.CargarTablaBeneficiarios WebMethod");

                    return html;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw ex;
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
                    log.Debug("Inicio Cotizador.CargarTablaActividades WebMethod");

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaActividades)pagina.LoadControl("~/Controles/TablaActividades.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.TelefonoConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            log.Debug("Inicio Cotizador.servicioCotizador.ListarActividad WebMethod");
                            List<Actividad> actividades = servicioCotizador.ListarActividad(cuspp);
                            log.Debug("Fin Cotizador.servicioCotizador.ListarActividad WebMethod");

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

                        log.Debug("Fin Cotizador.CargarTablaActividades WebMethod");

                        return html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        log.Debug("Fin Cotizador.CargarTablaActividades WebMethod");

                        return Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw ex;
                }
            }
        }

        [WebMethod]
        public static string CargarComboCiudades(string idDepartamento)
        {
            log.Debug("Inicio Cotizador.CargarComboCiudades WebMethod");

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

            log.Debug("Fin Cotizador.CargarComboCiudades WebMethod");

            return html;
        }

        [WebMethod]
        public static string CargarComboComunas(string idCiudad)
        {
            log.Debug("Inicio Cotizador.CargarComboComunas WebMethod");

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

            log.Debug("Fin Cotizador.CargarComboComunas WebMethod");

            return html;
        }

        [WebMethod]
        public static void CargarComboPeriodoGarantizado(string idTemporalidad)
        {
            //Cargar combobox de Productos
            log.Debug("Inicio Cotizador.CargarComboPeriodoGarantizado WebMethod");
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Producto> productos = servicioCotizador.ListarProducto(idTemporalidad);
            HttpContext.Current.Session["ComboProducto"] = productos;
            log.Debug("Fin Cotizador.CargarComboPeriodoGarantizado WebMethod");
        }

        [WebMethod]
        public static Direccion ObtenerDatosDireccion(int idDireccion)
        {
            log.Debug("Inicio Cotizador.ObtenerDatosDireccion WebMethod");
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Direccion dir;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                dir = servicioCotizador.ObtenerDatosDireccion(idDireccion);
                log.Debug("Fin Cotizador.ObtenerDatosDireccion WebMethod");
                return dir;
            }
        }

        [WebMethod]
        public static string SessionIdMonedaFondo(string idMonedaFondo)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.SessionIdMonedaFondo WebMethod");

                HttpContext.Current.Session["idMonedaFondo"] = idMonedaFondo;

                List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                HttpContext.Current.Session["ComboMoneda"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda];

                log.Debug("Fin Cotizador.SessionIdMonedaFondo WebMethod");

                return idMonedaFondo.ToString();
            }
        }

        [WebMethod]
        public static string SessionIdDreccion(int idDireccion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.SessionIdDreccion WebMethod");
                HttpContext.Current.Session["idDireccion"] = idDireccion;
                if (idDireccion == 0)
                {
                    HttpContext.Current.Session["ModDirModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModDirModo"] = "M";
                }
                log.Debug("Fin Cotizador.SessionIdDreccion WebMethod");

                return idDireccion.ToString();
            }
        }

        [WebMethod]
        public static string SessionIdSolicitud(string idSolicitud, string fecCotizacion, string accion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.SessionIdSolicitud WebMethod");

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

                log.Debug("Fin Cotizador.SessionIdSolicitud WebMethod");

                return idSolicitud.ToString();
            }
        }

        [WebMethod]
        public static string SessionIdTelefono(int idTelefono)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.SessionIdTelefono WebMethod");

                HttpContext.Current.Session["idTelefono"] = idTelefono;
                if (idTelefono == 0)
                {
                    HttpContext.Current.Session["ModTelModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModTelModo"] = "M";
                }

                log.Debug("Fin Cotizador.SessionIdTelefono WebMethod");

                return idTelefono.ToString();
            }
        }

        [WebMethod]
        public static string SessionIdGrupoFamiliar(int idGrupoFamiliar)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.SessionIdGrupoFamiliar WebMethod");

                HttpContext.Current.Session["idGrupoFamiliar"] = idGrupoFamiliar;
                if (idGrupoFamiliar == 0)
                {
                    HttpContext.Current.Session["ModGruFamModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModGruFamModo"] = "M";
                }

                log.Debug("Fin Cotizador.SessionIdGrupoFamiliar WebMethod");

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
                    log.Debug("Inicio Cotizador.InsertarDireccion WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionInsertar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarDireccion(direccion, idDepartamento, idCiudad, idComuna, idPrincipal, glsEspacioUrbano, idDomicilio, errores, controles))
                            {
                                // Validar cartera del agente
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
                                    log.Debug("Inicio Cotizador.servicioCotizador.RegistrarDireccion WebMethod");
                                    servicioCotizador.RegistrarDireccion(dir);
                                    log.Debug("Fin Cotizador.servicioCotizador.RegistrarDireccion WebMethod");

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

                    log.Debug("Fin Cotizador.InsertarDireccion WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin Cotizador.InsertarDireccion WebMethod");

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
                    log.Debug("Inicio Cotizador.ModificarDireccion WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionActualizar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarDireccion(direccion, idDepartamento, idCiudad, idComuna, idPrincipal, glsEspacioUrbano, idDomicilio, errores, controles))
                            {
                                // Validar cartera del agente
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
                                    log.Debug("Inicio Cotizador.servicioCotizador.ActualizarDireccion WebMethod");
                                    servicioCotizador.ActualizarDireccion(dir);
                                    log.Debug("Fin Cotizador.servicioCotizador.ActualizarDireccion WebMethod");

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

                    log.Debug("Fin Cotizador.ModificarDireccion WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin Cotizador.ModificarDireccion WebMethod");

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
                    log.Debug("Inicio Cotizador.EliminarDireccion WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DireccionEliminar))
                        {
                            // Validar cartera del agente
                            if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                Direccion dir = new Direccion
                                {
                                    Id = Convert.ToInt32(idDireccion),
                                    Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                };

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                log.Debug("Inicio Cotizador.servicioCotizador.EliminarDireccion WebMethod");
                                servicioCotizador.EliminarDireccion(dir);
                                log.Debug("Fin Cotizador.servicioCotizador.EliminarDireccion WebMethod");

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

                    log.Debug("Fin Cotizador.EliminarDireccion WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin Cotizador.EliminarDireccion WebMethod");

                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static Telefono ObtenerDatosTelefono(int idTelefono)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.ObtenerDatosTelefono WebMethod");
                Telefono tel;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                tel = servicioCotizador.ObtenerDatosTelefono(idTelefono);
                log.Debug("Fin Cotizador.ObtenerDatosTelefono WebMethod");
                return tel;
            }
        }

        [WebMethod]
        public static GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.ObtenerDatosGrupoFamiliar WebMethod");
                GrupoFamiliar gru;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                gru = servicioCotizador.ObtenerDatosGrupoFamiliar(idGrupoFamiliar, "");
                log.Debug("Fin Cotizador.ObtenerDatosGrupoFamiliar WebMethod");
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
                    log.Debug("Inicio Cotizador.InsertarGrupoFamiliar WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
                            {
                                // Validar cartera del agente
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

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    log.Debug("Inicio Cotizador.servicioCotizador.RegistrarGrupoFamiliar WebMethod");
                                    respuesta = servicioCotizador.RegistrarGrupoFamiliar(gru);
                                    log.Debug("Fin Cotizador.servicioCotizador.RegistrarGrupoFamiliar WebMethod");

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

                    log.Debug("Fin Cotizador.InsertarGrupoFamiliar WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta
                    {
                        Estado = Constante.COD_ERROR,
                        Titulo = Enums.CuadroMensajeTitulo.Error.StringValue(),
                        Icono = Enums.CuadroMensajeIcono.Error.StringValue(),
                        Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message })
                    };

                    log.Debug("Fin Cotizador.InsertarGrupoFamiliar WebMethod");

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
                    log.Debug("Inicio Cotizador.ModificarGrupoFamiliar WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
                            {
                                // Validar cartera del agente
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

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    log.Debug("Inicio Cotizador.servicioCotizador.ActualizarGrupoFamiliar WebMethod");
                                    respuesta = servicioCotizador.ActualizarGrupoFamiliar(gru);
                                    log.Debug("Fin Cotizador.servicioCotizador.ActualizarGrupoFamiliar WebMethod");

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

                    log.Debug("Fin Cotizador.ModificarGrupoFamiliar WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin Cotizador.ModificarGrupoFamiliar WebMethod");

                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static SolicitudRPPlus CrearDatosSolicitud()
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.CrearDatosSolicitud WebMethod");

                SolicitudRPPlus sol = new SolicitudRPPlus();

                sol.FechaSolicitud = DateTime.Now;
                sol.Cotizaciones = new List<CotizacionRPPlus>();

                CotizacionRPPlus cot;

                SeccionCotizacionesRPPlus config = (SeccionCotizacionesRPPlus)ConfigurationManager.GetSection("cotizacionesRPPlus");

                HttpContext.Current.Session["idMonedaFondo"] = "001";

                foreach (ElementoCotizacionRPPlus cotizacion in config.CotizacionesRPPlus)
                {


                    cot = new CotizacionRPPlus
                    {
                        Moneda = new Moneda { Id = cotizacion.Moneda },
                        //Producto = new Producto { Id = cotizacion.Producto },
                        PeriodoGarantizado = Convert.ToInt32(cotizacion.PeriodoGarantizado),
                        AjusteTRA = 0,
                        PagoEscalonada = 0,
                        PjePE = 0,
                        IndGastoSepelio = "S",
                        ValPjeDev = 0,
                        ValMonAju = (cotizacion.Moneda == "001" || cotizacion.Moneda == "002") ? -1 : 2,
                        ValPjeConyuge = 0
                    };

                    sol.Cotizaciones.Add(cot);



                }

                log.Debug("Fin Cotizador.CrearDatosSolicitud WebMethod");

                return sol;
            }
        }

        [WebMethod]
        public static SolicitudRPPlus ObtenerDatosSolicitud(string idSolicitud, string fecCotizacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.ObtenerDatosSolicitud WebMethod");

                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                fecCotizacion = fechaCotizacion.ToString("yyyyMMdd");

                SolicitudRPPlus sol;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                sol = servicioCotizador.ObtenerDatosSolicitudRPPlus(idSolicitud);

                //<SOLINI26593>
                HttpContext.Current.Session["idMonedaFondo"] = sol.MonedaPrimaUnica.Id.ToString();//sol.MonedaPrimaUnica.Id.ToString();
                                                                                                  //<SOLINI26593>

                log.Debug("Fin Cotizador.ObtenerDatosSolicitud WebMethod");

                return sol;
            }
        }

        [WebMethod]
        public static List<CotizacionRPPlus> AgregarCotizacionASolicitud(List<CotizacionRPPlus> cot)
        {
            log.Debug("Inicio Cotizador.AgregarCotizacionASolicitud WebMethod");
            CotizacionRPPlus cotizacion = new CotizacionRPPlus
            {
                Moneda = new Moneda { Id = "0" },
                PeriodoGarantizado = 0,
                AjusteTRA = 0,
                IndGastoSepelio = "S",
                PagoEscalonada = 0,
                PjePE = 0,
                ValPjeDev = 0
            };
            cot.Add(cotizacion);
            log.Debug("Fin Cotizador.AgregarCotizacionASolicitud WebMethod");

            return cot;
        }

        [WebMethod]
        public static SolicitudRPPlus InsertarSolicitud(string tokenUsuario,
                                                    string cuspp,
                                                    string afp,
                                                    string temporalidad,
                                                    string monedaPrimaUnica,
                                                    string primaUnica,
                                                    string fechaCotizacion,
                                                    string fechaDevengue,
                                                    string dcom,
                                                    List<CotizacionRPPlus> cotizaciones,
                                                    List<int> idBeneficiarios,
                                                    string tipoplan)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.InsertarSolicitud WebMethod");

                SolicitudRPPlus sol;
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusInsertar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();

                            if (ValidarSolicitud(errores, controles, fechaCotizacion, fechaDevengue, primaUnica, dcom, cotizaciones, idBeneficiarios, cuspp, (string)HttpContext.Current.Session["Vendedor"], temporalidad))
                            {
                                //Validando la fecha de cotizacion segun rol
                                switch ((string)HttpContext.Current.Session["RolAzman"])
                                {
                                    case "JEF.RVI.OPE"://JefeOperaciones
                                    case "AST.RVI.OPE"://AsistenteOperaciones
                                        break;
                                    default:
                                        fechaCotizacion = ((DateTime)DateTime.Today).ToString("dd/MM/yyyy");
                                        fechaDevengue = ((DateTime)DateTime.Today.AddDays(-(DateTime.Today.Day - 1))).ToString("dd/MM/yyyy");
                                        break;
                                }

                                // Validar cartera del agente
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    sol = new SolicitudRPPlus
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
                                        TipoCotizacion = new TipoCotizacion { Id = Enums.TipoCotizacion.RentaPrivadaPlus.StringValue() },
                                        Temporalidad = new Temporalidad { Id = temporalidad },
                                        Cotizaciones = cotizaciones,
                                        TipoPlan = new TipoPlan { Id = tipoplan }

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

                                    sol.FechaVigencia = sol.FechaCotizacion;
                                    List<Parametro> lstDiasVigencia = new List<Parametro>();
                                    lstDiasVigencia = servicioCotizador.ObtenerParametrosPorTabla("PLUS");

                                    if (lstDiasVigencia.Count() > 0)
                                    {
                                        sol.FechaVigencia = sol.FechaVigencia.Value.AddDays(Convert.ToInt32(lstDiasVigencia[0].Valor_1));
                                    }

                                    respuesta = servicioCotizador.RegistrarSolicitudRPPlus(ref sol);

                                    if (((string)HttpContext.Current.Session["RolAzman"]) == "JEF.RVI.OPE")
                                    {
                                        HttpContext.Current.Session["ModSolModo"] = "M";
                                    }
                                    else
                                    {
                                        HttpContext.Current.Session["ModSolModo"] = "CONS";
                                    }
                                    HttpContext.Current.Session["idSolicitud"] = sol.Id;

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
                                        Detalle = "Solicitud registrada: " + sol.Id + ", DCOM: " + sol.PorcentajeDescuentoComision + ", DTRA: " + tra
                                    });
                                }
                                else
                                {
                                    sol = new SolicitudRPPlus();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                sol = new SolicitudRPPlus();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            sol = new SolicitudRPPlus();
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusInsertar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        sol = new SolicitudRPPlus();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    sol.Respuesta = respuesta;

                    log.Debug("Fin Cotizador.InsertarSolicitud WebMethod");

                    return sol;
                }
                catch (Exception ex)
                {
                    sol = new SolicitudRPPlus();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    sol.Respuesta = respuesta;

                    log.Debug("Fin Cotizador.InsertarSolicitud WebMethod");

                    return sol;
                }
            }
        }

        [WebMethod]
        public static SolicitudRPPlus ModificarSolicitud(string tokenUsuario,
                                                     string idSolicitud,
                                                     string cuspp,
                                                     string afp,
                                                     string temporalidad,
                                                     string monedaPrimaUnica,
                                                     string primaUnica,
                                                     string fechaCotizacion,
                                                     string fechaDevengue,
                                                     string dcom,
                                                     List<CotizacionRPPlus> cotizaciones,
                                                     List<int> idBeneficiarios,
                                                     string tipoplan)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudRPPlus sol;
                try
                {
                    log.Debug("Inicio Cotizador.ModificarSolicitud WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusActualizar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();

                            if (ValidarSolicitud(errores, controles, fechaCotizacion, fechaDevengue, primaUnica, dcom, cotizaciones, idBeneficiarios, cuspp, (string)HttpContext.Current.Session["Vendedor"], temporalidad))
                            {
                                // Validar la cartera del agente
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    sol = new SolicitudRPPlus
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
                                        TipoCotizacion = new TipoCotizacion { Id = Enums.TipoCotizacion.RentaPrivadaPlus.StringValue() },
                                        Temporalidad = new Temporalidad { Id = temporalidad },
                                        Cotizaciones = cotizaciones,
                                        TipoPlan = new TipoPlan { Id = tipoplan }
                                    };

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                    SolicitudRPPlus solicitudPlus;
                                    DateTime fechaCotizacion2 = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE"));
                                    solicitudPlus = servicioCotizador.ObtenerDatosSolicitudRPPlus(sol.Id);

                                    // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                                    if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                                    {
                                        sol.PorcentajeDescuentoComision = Convert.ToDouble(dcom, new CultureInfo("es-PE"));
                                    }
                                    else
                                    {
                                        if (solicitudPlus != null)
                                        {
                                            sol.PorcentajeDescuentoComision = solicitudPlus.PorcentajeDescuentoComision;
                                        }
                                        else
                                        {
                                            sol.PorcentajeDescuentoComision = null;
                                        }
                                    }

                                    //Validando la fecha de cotizacion segun rol
                                    switch ((string)HttpContext.Current.Session["RolAzman"])
                                    {
                                        case "JEF.RVI.OPE"://JefeOperaciones
                                        case "AST.RVI.OPE"://AsistenteOperaciones
                                            break;
                                        default:
                                            sol.FechaCotizacion = Convert.ToDateTime(solicitudPlus.FechaSolicitud, new CultureInfo("es-PE"));
                                            sol.FechaSolicitud = Convert.ToDateTime(solicitudPlus.FechaSolicitud, new CultureInfo("es-PE"));
                                            sol.FechaDevengue = Convert.ToDateTime(solicitudPlus.FechaDevengue, new CultureInfo("es-PE"));

                                            break;
                                    }

                                    if (((string)HttpContext.Current.Session["RolAzman"]) != "JEF.RVI.OPE"
                                            && ((string)HttpContext.Current.Session["RolAzman"]) != "AST.RVI.COM"
                                            && ((string)HttpContext.Current.Session["RolAzman"]) != "JEF.VTA.LIM.RVI"
                                            && ((string)HttpContext.Current.Session["RolAzman"]) != "JEF.VTA.PRO.RVI")
                                    {
                                        throw new Exception("Ud. no tiene acceso para actualizar!!");
                                    }
                                    List<GrupoFamiliar> lben = new List<GrupoFamiliar>();
                                    idBeneficiarios.ForEach(id => lben.Add(((List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"])[id]));

                                    sol.Beneficiarios = lben;

                                    sol.FechaVigencia = sol.FechaCotizacion;
                                    List<Parametro> lstDiasVigencia = new List<Parametro>();
                                    lstDiasVigencia = servicioCotizador.ObtenerParametrosPorTabla("PLUS");

                                    if (lstDiasVigencia.Count() > 0)
                                    {
                                        sol.FechaVigencia = sol.FechaVigencia.Value.AddDays(Convert.ToInt32(lstDiasVigencia[0].Valor_1));
                                    }


                                    respuesta = servicioCotizador.ActualizarSolicitudRPPlus(ref sol);

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
                                        Detalle = "Solicitud modificada: " + sol.Id + ", DCOM: " + sol.PorcentajeDescuentoComision + ", DTRA: " + tra
                                    });
                                }
                                else
                                {
                                    sol = new SolicitudRPPlus();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                sol = new SolicitudRPPlus();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            sol = new SolicitudRPPlus();
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
                        sol = new SolicitudRPPlus();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    sol.Respuesta = respuesta;

                    log.Debug("Fin Cotizador.ModificarSolicitud WebMethod");

                    return sol;
                }
                catch (Exception ex)
                {
                    sol = new SolicitudRPPlus();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    sol.Respuesta = respuesta;

                    log.Debug("Fin Cotizador.ModificarSolicitud WebMethod");

                    return sol;
                }
            }
        }

        [WebMethod]
        public static Respuesta ExportarSolicitudPDF(string idSolicitud, string fecCotizacion, string numAgente)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.ExportarSolicitudPDF WebMethod");

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

                log.Debug("Fin Cotizador.ExportarSolicitudPDF WebMethod");
                return respuesta;
            }
        }

        [WebMethod]
        public static CorreoElectronico CrearDatosCorreo(string tokenUsuario, string idSolicitud, string fecCotizacion, string tipoCotizacion, string nombre, string apellidoPaterno, string apellidoMaterno, string sexo)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.CrearDatosCorreo WebMethod");

                CorreoElectronico correo;
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusEnviarCorreo))
                        {
                            // Validar cartera del agente
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
                                correo.Respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
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

                    log.Debug("Fin Cotizador.CrearDatosCorreo WebMethod");

                    return correo;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);                    
                    correo = new CorreoElectronico();
                    correo.Respuesta = new Respuesta();
                    correo.Respuesta.Estado = Constante.COD_ERROR;
                    correo.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    correo.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    correo.Respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin Cotizador.CrearDatosCorreo WebMethod");

                    return correo;
                }
            }
        }

        [WebMethod]
        public static Respuesta EnviarCorreoElectronico(string tokenUsuario, CorreoElectronico correo)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.EnviarCorreoElectronico WebMethod");

                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusEnviarCorreo))
                        {
                            // Validar cartera del agente
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
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                log.Debug("Fin Cotizador.EnviarCorreoElectronico WebMethod");

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
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio Cotizador.ObtenerDatosAfiliado WebMethod");

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
                                log.Debug("Inicio Cotizador.servicioCotizador.ObtenerDatosAfiliado WebMethod");
                                afiliado = servicioCotizador.ObtenerDatosAfiliado(nroSolicitud, cuspp, "", "", Enums.TipoProducto.RPP.StringValue());
                                log.Debug("Fin Cotizador.servicioCotizador.ObtenerDatosAfiliado WebMethod");

                                log.Info("Usuario realizó búsqueda de afiliados por "
                                    + ((nroSolicitud.Trim().Length != 0)
                                    ? ("Solicitud [" + nroSolicitud.ToUpper() + "]")
                                    : ("CUSPP [" + cuspp.ToUpper() + "]")) + ".");

                                if (afiliado != null)
                                {
                                    // Validar cartera del agente
                                    if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == afiliado.Agente.Id))
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
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DatosAfiliadoConsultar.StringValue()));
                            afiliado = new Afiliado();
                            afiliado.Respuesta = new Respuesta();
                            afiliado.Respuesta.Estado = Constante.COD_ERROR;
                            afiliado.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            afiliado.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            afiliado.Respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
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
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    afiliado = new Afiliado();
                    afiliado.Respuesta = new Respuesta();
                    afiliado.Respuesta.Estado = Constante.COD_ERROR;
                    afiliado.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    afiliado.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    afiliado.Respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    afiliado = new Afiliado();
                    afiliado.Respuesta = new Respuesta();
                    afiliado.Respuesta.Estado = Constante.COD_ERROR;
                    afiliado.Respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    afiliado.Respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    afiliado.Respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                log.Debug("Fin Cotizador.ObtenerDatosAfiliado WebMethod");

                return afiliado;
            }
        }

        protected void BusAfiBuscar_RP_Click(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio Cotizador.BusAfiBuscar_RP_Click");

                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
                    {
                        if (ValidarBusquedaAfiliados())
                        {
                            LimpiarFormularios();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            log.Debug("Inicio Cotizador.servicioCotizador.ObtenerDatosAfiliado");
                            Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(BusAfiNroSolicitud_RP.Text, BusAfiCUSPP_RP.Text, "", "", Enums.TipoProducto.RPP.StringValue());
                            log.Debug("Fin Cotizador.servicioCotizador.ObtenerDatosAfiliado");

                            log.Info("Usuario realizó búsqueda de afiliados por "
                                + ((BusAfiNroSolicitud_RP.Text.Trim().Length != 0)
                                ? ("Solicitud [" + BusAfiNroSolicitud_RP.Text.ToUpper() + "]")
                                : ("CUSPP [" + BusAfiCUSPP_RP.Text.ToUpper() + "]")) + ".");

                            if (afiliado != null)
                            {
                                // Guardar los datos del afiliado para generarlo como beneficiario en caso de que no exista
                                log.Debug("Inicio Cotizador.servicioCotizador.ActualizarAfiliado");
                                servicioCotizador.ActualizarAfiliado(afiliado);
                                log.Debug("Fin Cotizador.servicioCotizador.ActualizarAfiliado");

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
                                    BusquedaAfiliados_RP.Visible = false;
                                    LineaCUSPP_RP.Visible = false;
                                    afiliado.ApellidoPaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoPaterno);
                                    afiliado.ApellidoMaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoMaterno);
                                    afiliado.Nombre = Utilitarios.EnmascararNombre(afiliado.Nombre);
                                    LineaNacimientoSexo_RP.Visible = false;
                                    LineaCorreoElectronico_RP.Visible = false;
                                    LineaCategoria_RP.Visible = false;
                                    LineaSaldoCIC_RP.Visible = false;
                                    LineaAFP_RP.Visible = false;
                                    GrupoDireccion_RP.Visible = false;

                                    //oculto
                                    ContenedorGuardar_RP.Visible = false;
                                }
                                else
                                {
                                    // MastePage
                                    cabecera.Visible = true;
                                    cabeceraProtegida.Visible = false;

                                    // Datos del Afiliado
                                    LineaSaldoCIC_RP.Visible = true;
                                }

                                // Validar la cartera del agente
                                if (Utilitario.PerteneceACartera(afiliado.Agente.Id, afiliado.CUSPP, (List<Agente>)Session["ListaAgentes"], (string)Session["RolAzman"], (string)Session["Usuario"], true))
                                {
                                    if (BusAfiCUSPP_RP.Text.Trim().Length > 0)
                                    {
                                        Session["CUSPP"] = BusAfiCUSPP_RP.Text;
                                        Session["NroSolicitud"] = null;
                                        Session["CUSPP_PLUS"] = null;
                                    }
                                    else if (BusAfiNroSolicitud_RP.Text.Trim().Length > 0)
                                    {
                                        Session["CUSPP"] = null;
                                        Session["NroSolicitud"] = BusAfiNroSolicitud_RP.Text;
                                        Session["CUSPP_PLUS"] = afiliado.CUSPP.Trim();
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

                                    // Datos del agente
                                    Agente agente = ((List<Agente>)Session["ListaAgentes"]).Find(a => a.Id == afiliado.Agente.Id);
                                    if (agente == null && Utilitarios.EsRolVerAgentesCesados((string)Session["RolAzman"]))
                                    {
                                        agente = servicioCotizador.ObtenerUltimoAgentePorCartera(afiliado.Agente.IdCartera, (string)Session["Usuario"]);
                                    }
                                    NumeroAgente.Value = agente.Id;
                                    Cartera.Value = agente.IdCartera;
                                    NombreAgente.Value = agente.Nombre;
                                    Agente.Text = string.Format("{0} - {1}", agente.Id, agente.Nombre);
                                    Session["Vendedor"] = agente.Id;
                                    Session["Cartera"] = agente.IdCartera;
                                    Session["AFP_RP"] = afiliado.AFP.Id.ToString();
                                    Session["CUSPP_RP"] = afiliado.CUSPP.ToString();

                                    RangoInversion_RP.Text = afiliado.RangoInversion.ToString();

                                    if (afiliado.CentroLaboral.Length > 0 && afiliado.CentroLaboral != null)
                                    {
                                        CentroLaboral_RP.Text = afiliado.CentroLaboral.ToString();
                                    }

                                    if (afiliado.EstadoCivil.cod_parametro.Length > 0 && afiliado.EstadoCivil.cod_parametro != null)
                                    {
                                        EstadoCivil_RP.SelectedIndex = EstadoCivil_RP.Items.IndexOf(EstadoCivil_RP.Items.FindByValue(afiliado.EstadoCivil.cod_parametro.ToString()));
                                    }

                                    NuevaDireccion_RP.Visible = true;

                                    // Botón Guardar Afiliado
                                    if (afiliado.Consentimiento)
                                        ContenedorGuardar_RP.Visible = true;
                                    else
                                        ContenedorGuardar_RP.Visible = false;

                                    // Grupo Familiar
                                    NuevoBeneficiario_RP.Visible = true;

                                    // Solicitudes
                                    NuevaSolicitud_RP.Visible = true;
                                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusInsertar))
                                    {
                                        NuevaSolicitud_RP.NavigateUrl = ResolveUrl("~/RPP/Cotizador.aspx") + "?m=N&c=" + CUSPP_RP.Text;
                                    }
                                    else
                                    {
                                        NuevaSolicitud_RP.NavigateUrl = string.Empty;
                                    }

                                    // Si el agente está cesado no mostrar el botón de Nueva Solicitud
                                    OpcionSistema opcionInsertarSolicitud = ((List<OpcionSistema>)Session["OpcionesSistema"]).Find(o => o.IdAzman == (int)Enums.OpcionesSistema.SolicitudPlusInsertar);
                                    if (opcionInsertarSolicitud.Activa)
                                    {
                                        if (!Utilitario.PerteneceACartera(afiliado.Agente.Id, afiliado.CUSPP, (List<Agente>)Session["ListaAgentes"], (string)Session["RolAzman"], (string)Session["Usuario"], false))
                                        {
                                            InhabilitarControl(NuevaSolicitud_RP);
                                            PerNuevaSolicitud_RP.Value = "0";
                                            NuevaSolicitud_RP.Enabled = false;
                                        }
                                    }

                                    ModEnvCorPara.Text = afiliado.CorreoElectronico;

                                    List<Ciudad> listaCiudades = new List<Ciudad>();
                                    log.Debug("Inicio Cotizador.servicioCotizador.ObtenerDatosCiudad");
                                    Ciudad ciudad = servicioCotizador.ObtenerDatosCiudad(afiliado.CiudadEmpresa.Id);
                                    log.Debug("Fin Cotizador.servicioCotizador.ObtenerDatosCiudad");
                                    if (ciudad != null)
                                    {
                                        listaCiudades.Add(ciudad);
                                    }

                                    List<Comuna> listaComunas = new List<Comuna>();
                                    log.Debug("Inicio Cotizador.servicioCotizador.ObtenerDatosComuna");
                                    Comuna comuna = servicioCotizador.ObtenerDatosComuna(afiliado.ComunaEmpresa.Id);
                                    log.Debug("Fin Cotizador.servicioCotizador.ObtenerDatosComuna");
                                    if (comuna != null)
                                    {
                                        listaComunas.Add(comuna);
                                    }

                                    TipoDocumento_RP.SelectedIndex = TipoDocumento_RP.Items.IndexOf(TipoDocumento_RP.Items.FindByValue(afiliado.TipoIdentificacion));
                                    NumeroDocumento_RP.Text = afiliado.NumeroIdentificacion;

                                    HttpContext.Current.Session["indConsentimiento"] = afiliado.Consentimiento;  //JY

                                    //consentimiento de asesoria
                                    obtenerConsentimiento(afiliado, true);
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

                        log.Debug("Fin Cotizador.BusAfiBuscar_RP_Click");
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.DatosAfiliadoConsultar.StringValue()));
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstado.Value = "1";

                        log.Debug("Fin Cotizador.BusAfiBuscar_RP_Click");

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
            Categoria_RP.SelectedIndex = Categoria_RP.Items.IndexOf(Categoria_RP.Items.FindByValue("0"));
            HCategoria_RP.Value = "0";
            AFP_RP.SelectedIndex = AFP_RP.Items.IndexOf(AFP_RP.Items.FindByValue("0"));
            HAFP_RP.Value = "0";
            SaldoCIC_RP.Text = string.Empty;
            Session["Vendedor"] = null;
            Session["Cartera"] = null;

            RangoInversion_RP.Text = string.Empty;
            CentroLaboral_RP.Text = string.Empty;

            CorreoElectronico_RP.CssClass = CorreoElectronico_RP.CssClass.Replace(" formTextboxError", string.Empty);
            Categoria_RP.CssClass = Categoria_RP.CssClass.Replace(" formComboboxError", string.Empty);
            AFP_RP.CssClass = AFP_RP.CssClass.Replace(" formComboboxError", string.Empty);
            SaldoCIC_RP.CssClass = SaldoCIC_RP.CssClass.Replace(" formTextboxError", string.Empty);

            RangoInversion_RP.CssClass = RangoInversion_RP.CssClass.Replace(" formTextboxError", string.Empty);
            CentroLaboral_RP.CssClass = CentroLaboral_RP.CssClass.Replace(" formTextboxError", string.Empty);

            // Direcciones
            NuevaDireccion_RP.Visible = false;

            // Teléfonos
            //oculto
            //NuevoTelefono_RP.Visible = false;

            // Datos de empresa
            //oculto
            //NombreEmpresa_RP.Text = string.Empty;
            //DireccionEmpresa_RP.Text = string.Empty;
            //CiudadEmpresa_RP.SelectedIndex = CiudadEmpresa_RP.Items.IndexOf(CiudadEmpresa_RP.Items.FindByValue("0"));
            //ComunaEmpresa_RP.SelectedIndex = ComunaEmpresa_RP.Items.IndexOf(ComunaEmpresa_RP.Items.FindByValue("0"));
            //TelefonoEmpresa_RP.Text = string.Empty;

            // Botón Guardar Afiliado
            ContenedorGuardar_RP.Visible = false;

            // Grupo Familiar
            NuevoBeneficiario_RP.Visible = false;

            // Solicitudes
            NuevaSolicitud_RP.Visible = false;
            ModEnvCorDe.Text = string.Empty;
            ModEnvCorPara.Text = string.Empty;
            ModEnvCorAsunto.Text = string.Empty;

            //EstadoCivil_RP.Text = string.Empty;

            TipoDocumento_RP.SelectedIndex = 0;
            NumeroDocumento_RP.Text = string.Empty;

            Telefono_RP.Text = string.Empty;
            Celular_RP.Text = string.Empty;
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

        private static bool ValidarDireccion(string glsDireccion, string idDepartamento, string idCiudad, string idComuna, string idPrincipal, string glsEspacioUrbano, string idDomicilio, List<String> errores, List<String> controles)
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

        //private static bool ValidarSolicitud(List<String> errores, List<String> controles, string fecCotizacion, string fecDevengue, string valPrimaUnica, string valDcom, List<CotizacionRPPlus> listaCotizaciones, List<int> idBeneficiarios, string cusspp, string numAgenteSol)
        private static bool ValidarSolicitud(List<String> errores, List<String> controles, string fecCotizacion, string fecDevengue, string valPrimaUnica, string valDcom, List<CotizacionRPPlus> listaCotizaciones, List<int> idBeneficiarios, string cusspp, string numAgenteSol, string cod_temporalidad)
        ////private static bool ValidarSolicitud(List<String> errores, List<String> controles, string fecCotizacion, string fecDevengue, string valPrimaUnica, string valDcom, List<CotizacionRPPlus> listaCotizaciones, List<int> idBeneficiarios, string cusspp, string numAgenteSol, string cod_temporalidad)
        ////private static bool ValidarSolicitud(List<String> errores, List<String> controles, string fecCotizacion, string fecDevengue, string valPrimaUnica, string valDcom, List<CotizacionRPPlus> listaCotizaciones, List<int> idBeneficiarios, string cusspp, string numAgenteSol, string cod_temporalidad)
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
                //<GTIINI-10761>
                //List<RolDcom> listaRolDcom = servicioCotizador.ListarRolDcom(rolDcom);
                List<RolDcom> listaRolDcom = servicioCotizador.ListarRolDcomRPP(rolDcom);
                //<GTIFIN-10761>

                double vDcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));
                string dcomRangos = string.Empty;
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

            List<GrupoFamiliar> lben = new List<GrupoFamiliar>();
            if (idBeneficiarios.Count > 0)
            {
                idBeneficiarios.ForEach(id => lben.Add(((List<GrupoFamiliar>)HttpContext.Current.Session["Beneficiarios"])[id]));
            }

            for (int i = 0; i < listaCotizaciones.Count; i++)
            {

                // Moneda
                if (listaCotizaciones[i].Moneda.Id == "0")
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Moneda</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    controles.Add(i + ",1");
                }

                //Validando Años Escalonado y %
                if (listaCotizaciones[i].PagoEscalonada != 0 && listaCotizaciones[i].PjePE == 0)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>2° Tramo %</strong>. No puede ser Cero.");
                    cotizaciones = false;
                    controles.Add(i + ",4");
                }

                // Ajuste TRA
                if (listaCotizaciones[i].AjusteTRA.ToString().Length == 0)
                {
                    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Dif. TRA</strong>. Dato Obligatorio.");
                    cotizaciones = false;
                    controles.Add(i + ",11");
                }

                //Valida Conyuge
                //<INIGTI_753>
                if (idBeneficiarios.Count != 2)
                {
                    if (listaCotizaciones[i].ValPjeConyuge > 0)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: No Puede tener porcentaje de conyuge.");
                        cotizaciones = false;
                    }
                }
                else if (idBeneficiarios.Count == 2)
                {
                    if (lben[1].Parentesco.Id != "10")
                    {
                        if (listaCotizaciones[i].ValPjeConyuge > 0)
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: No Puede tener porcentaje de conyuge.");
                            cotizaciones = false;
                        }
                    }
                }
                //<FINGTI_753>
                //<INI.GTI_7012>
                if ('T' + listaCotizaciones[i].PeriodoGarantizado.ToString("00") == cod_temporalidad)
                {
                    //Validar Sepelio
                    if (listaCotizaciones[i].IndGastoSepelio == "N")
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Activar gasto de sepelio, según temporalidad y periodo garantizado.");
                        cotizaciones = false;
                        //controles.Add(i + ",3");
                    }
                    //<INI.GTI_15930>
                    if ((cod_temporalidad == "T05" || cod_temporalidad == "T07") && (listaCotizaciones[i].Moneda.Id == Enums.Moneda.Soles.StringValue() || listaCotizaciones[i].Moneda.Id == Enums.Moneda.SolesAjustados.StringValue()))
                    {
                        //Validar %Devolución
                        if (listaCotizaciones[i].ValPjeDev == 0 || listaCotizaciones[i].ValPjeDev == 25)
                        {
                            errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: No Puede tener porcentaje de devolución [0% ó 25%], según temporalidad y periodo garantizado.");
                            cotizaciones = false;
                            //controles.Add(i + ",6");
                        }
                    }
                    //<FIN.GTI_15930>

                }
                else
                {
                    //Validar %Devolución
                    if (listaCotizaciones[i].ValPjeDev > 0)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: No Puede tener porcentaje de devolución, según temporalidad y periodo garantizado.");
                        cotizaciones = false;
                        //controles.Add(i + ",6");
                    }
                }
                //<FIN.GTI_7012>
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
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoActualizar))
                    {
                        // Validar cartera del agente
                        if (Utilitarios.EsRolVerAgentesCesados((string)Session["RolAzman"]) || ((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == (string)Session["Vendedor"]))
                        {
                            if (ValidarAfiliado())
                            {
                                Afiliado afiliado = new Afiliado
                                {
                                    CUSPP = CUSPP_RP.Text,
                                    TipoIdentificacion = TipoDocumento_RP.SelectedValue,
                                    NumeroIdentificacion = NumeroDocumento_RP.Text,
                                    //CorreoElectronico = CorreoElectronico_RP.Text,
                                    Categoria = new Categoria { Id = Categoria_RP.SelectedValue },
                                    AFP = new AFP { Id = AFP_RP.SelectedValue },
                                    //<SRIINI06326>
                                    //SaldoCIC = Convert.ToDouble(SaldoCIC.Text, new CultureInfo("es-PE"))
                                    SaldoCIC = ((bool)Session["Consentimiento"]) ? (double?)Convert.ToDouble(SaldoCIC_RP.Text, new CultureInfo("es-PE")) : null,
                                    //<SRIFIN06326>
                                    //<INIGTI_7012>
                                    EstadoCivil = new Temporal { cod_parametro = EstadoCivil_RP.SelectedValue }
                                    //ConfidencialidadDatos = new Parametro{ Id = ConfidencialidadDatos_RP.SelectedValue }
                                    //<FINGTI_7012>

                                    //,Telefonos = Telefono_RP.Text
                                    //,Celulares = Celular_RP.Text
                                    ,
                                    RangoInversion = RangoInversion_RP.Text
                                    ,
                                    CentroLaboral = CentroLaboral_RP.Text
                                };

                                if (HToken_RPP.Value.Length > 0)
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

                                    afiliado.CorreoElectronicoCliente = "";
                                    afiliado.TelefonoCliente = "";
                                    afiliado.CelularCliente = "";
                                }

                                HCategoria_RP.Value = Categoria_RP.SelectedValue;
                                HAFP_RP.Value = AFP_RP.SelectedValue;

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                Respuesta respuesta = servicioCotizador.ActualizarAfiliado(afiliado);

                                obtenerConsentimiento(afiliado, false);

                                log.Info(String.Format("Usuario actualizó los datos del afiliado CUSPP[{0}].", CUSPP_RP.Text));
                                log.Debug(String.Format("Correo Electrónico[{0}] Categoría[{1}:{2}] AFP[{3}:{4}] Saldo CIC[{5}] Estado Civil[{6}:{7}].",
                                    CorreoElectronico_RP.Text,
                                    Categoria_RP.SelectedValue, Categoria_RP.SelectedItem.Text,
                                    AFP_RP.SelectedValue, AFP_RP.SelectedItem.Text,
                                    SaldoCIC_RP.Text,
                                    EstadoCivil_RP.SelectedValue, EstadoCivil_RP.SelectedItem.Text));
                                //ConfidencialidadDatos_RP.SelectedValue, ConfidencialidadDatos_RP.SelectedItem.Text

                                string nombreTerminal = string.Empty;
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
                                    Detalle = String.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "ActualizarAfiliado", Environment.NewLine, Environment.NewLine, "afiliado", JsonConvert.SerializeObject(afiliado)),
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
            //AFP_RP.CssClass = "formCombobox";
            SaldoCIC_RP.CssClass = "formTextbox";

            RangoInversion_RP.CssClass = "formTextbox";
            CentroLaboral_RP.CssClass = "formTextbox";

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

            //// AFP
            //bool afp = true;
            //if (AFP_RP.SelectedValue == "0")
            //{
            //    errores.Add("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
            //    afp = false;
            //}

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

            //Tipo de identificacion
            bool tipoIdentificacion = true;
            if (TipoDocumento_RP.SelectedIndex <= 0)
            {
                errores.Add("Seleccione el campo <strong>Tipo de Identificación</strong>. Dato Obligatorio.");
                tipoIdentificacion = false;
            }

            //Numero de identificacion
            bool numIdentificacion = true;
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

            // Clases de controles
            if (!correoElectronico) { CorreoElectronico_RP.CssClass = "formTextbox formTextboxError"; } else { CorreoElectronico_RP.CssClass = "formTextbox formTextboxReadOnly"; }
            if (!categoria) { Categoria_RP.CssClass = "formCombobox formComboboxError"; } else { Categoria_RP.CssClass = "formComboboxTexto formTextboxReadOnly"; }
            //if (!afp) { AFP_RP.CssClass = "formCombobox formComboboxError"; } else { AFP_RP.CssClass = "formCombobox"; }
            if (!saldoCIC) { SaldoCIC_RP.CssClass = "formTextbox formTextboxError numerico"; } else { SaldoCIC_RP.CssClass = "formTextbox numerico"; }

            Telefono_RP.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono";
            Celular_RP.CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono";

            esCorrecto = correoElectronico & categoria & saldoCIC & tipoIdentificacion & numIdentificacion;

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
                log.Debug("Inicio Cotizador.CargarRolEscenario WebMethod");

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

                    log.Debug("Inicio Cotizador.servicioCotizador.ListaAcomEscenario WebMethod");
                    List<RolAcom> listaRol = servicioCotizador.ListaAcomEscenario(rolAcom);
                    log.Debug("Fin Cotizador.servicioCotizador.ListaAcomEscenario WebMethod");

                    control.acomns = listaRol;

                    pagina.Controls.Add(control);

                    string html = "";
                    using (var sw = new StringWriter())
                    {
                        HttpContext.Current.Server.Execute(pagina, sw, false);
                        html = sw.ToString();
                    }

                    log.Debug("Fin Cotizador.CargarRolEscenario WebMethod");

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

                    log.Debug("Fin Cotizador.CargarRolEscenario WebMethod");

                    throw (ex);
                }


            }
        }
        /*<SRIFIN10693>*/

        public void LlenarPagoDoble(List<Parametro> lstParametro)
        {
            //List<Parametro> lstParametro = new List<Parametro>();
            List<Parametro> lstParametro2 = new List<Parametro>();
            //lstParametro = (List<Parametro>)HttpContext.Current.Session["ComboPagoDoble"];

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

        /*<INI.GTI_7012_3>*/
        [WebMethod]
        public static Respuesta ValidarVigencia(string tokenUsuario, string num_solicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio Cotizador.ValidarVigencia WebMethod");

                    Respuesta respuesta = new Respuesta();
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        List<String> errores = new List<String>();
                        List<String> controles = new List<String>();

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        DateTime fechaActual = DateTime.Today;
                        log.Debug("Inicio Cotizador.servicioCotizador.ObtenerDatosSolicitudRPPlus WebMethod");
                        SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(num_solicitud);
                        log.Debug("Fin Cotizador.servicioCotizador.ObtenerDatosSolicitudRPPlus WebMethod");

                        respuesta.Estado = Constante.COD_OK;

                        if (fechaActual > solicitud.FechaVigencia && solicitud.CodigoEstado == 0)
                        {
                            log.Warn(String.Format("La solicitud no se encuentra vigente [{0}].", num_solicitud));
                            errores.Add("La solicitud no se encuentra vigente");
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                        }
                        List<Direccion> lstDireccion = servicioCotizador.ListarDireccion(solicitud.Afiliado.CUSPP);
                        if (lstDireccion.Count == 0)
                        {
                            log.Warn(String.Format("El Afiliado no cuenta con dirección, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
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
                                log.Warn(String.Format("El Afiliado no cuenta con dirección principal, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
                                errores.Add("El Afiliado no cuenta con dirección principal");
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                            }
                            else
                            {
                                if (direccionPrincipal.TipoVia == null)
                                {
                                    log.Warn(String.Format("El Afiliado no cuenta con tipo vía válida, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
                                    errores.Add("El Afiliado no cuenta con tipo vía válida");
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                }
                                else if (direccionPrincipal.TipoVia.Id.Length == 0)
                                {
                                    log.Warn(String.Format("El Afiliado no cuenta con tipo vía válida, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
                                    errores.Add("El Afiliado no cuenta con tipo vía válida");
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                }
                                else if (direccionPrincipal.TipoVia.Id == "0")
                                {
                                    log.Warn(String.Format("El Afiliado no cuenta con tipo vía válida, CUSPP [{0}].", solicitud.Afiliado.CUSPP));
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

                    log.Debug("Fin Cotizador.ValidarVigencia WebMethod");

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
                    log.Debug("Fin Cotizador.ValidarVigencia WebMethod");
                    throw (ex);
                }
            }
        }
        /*<FIN.GTI_7012_3>*/

        //<INI.GTI_26697>
        private bool consentimiento(int idConfiguracion, string tipoDocumento, string numeroDocumento, string usuario, ref DateTime fechaConsentimiento, ref string consentimientoToken, ref string telefonoCliente, ref string celularCliente, ref string correoCliente, ref string tratamientoConsentimiento)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    log.Debug("Accediendo a las Key necesarias");
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

                    // Se busca el cliente por configuración 2 (RP), tipo y número de documento
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

                                    indicadorConsentimiento = jObject["ind_consentimiento"].ToString();
                                    HindConsentimiento.Value = indicadorConsentimiento;

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
                                // configuracion universal intercorp
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

                                        indicadorConsentimiento = jObject["ind_consentimiento"].ToString();
                                        HindConsentimiento.Value = indicadorConsentimiento;

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
        public static Respuesta EnviarConsentimientoAsesoria(string nombres, string tipoDocumento, string numeroDocumento, string correo, string cuspp, string token, string apellidoPaterno, string apellidoMaterno, string sexo, string fechaNacimiento, string telefono, string celular, string idConsentimientoAsesoria, string indConsentimiento)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    log.Info("Accediendo a las Key necesarias: token, correo");
                    string urlConsentimientoCliente = ConfigurationManager.AppSettings["url_consentimiento_cliente"].ToString();
                    string urlConsultaConsentimiento = ConfigurationManager.AppSettings["url_consulta_consentimiento_cliente"];
                    string urlAppConsentimiento = ConfigurationManager.AppSettings["url_app_consentimiento"].ToString();
                    string urlTratamiento = ConfigurationManager.AppSettings["url_tratamiento"].ToString();

                    string flagProveedorCorreo = ConfigurationManager.AppSettings["flag_proveedor_correo"].ToString();
                    string flagCorreoCliente = ConfigurationManager.AppSettings["flag_correo_cliente"].ToString();
                    string flagCorreoAgente = ConfigurationManager.AppSettings["flag_correo_agente"].ToString();

                    string remitenteConsentimiento = ConfigurationManager.AppSettings["remitente_consentimiento"].ToString();
                    string destinatarioConsentimiento = ConfigurationManager.AppSettings["destinatario_consentimiento"].ToString();
                    string asuntoConsentimiento = ConfigurationManager.AppSettings["asunto_consentimiento"].ToString();

                    if (destinatarioConsentimiento != "N")
                    {
                        correo = destinatarioConsentimiento;
                    }

                    string usuario = HttpContext.Current.Session["Usuario"].ToString();

                    // Armando correo
                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                    string correoAgente = string.Empty;

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado("", cuspp, "", "", "");

                    // Obteniendo datos del agente
                    List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                    Agente agente = listaAgentes.Find(a => a.Id == HttpContext.Current.Session["Vendedor"].ToString());
                    if (agente == null && Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]))
                    {
                        agente = servicioCotizador.ObtenerUltimoAgentePorCartera(afiliado.Agente.IdCartera, usuario);
                    }
                    Agente supervisor = listaAgentes.Find(a => a.Id == agente.IdPadre);

                    correoAgente = string.Empty;

                    log.Debug("Obteniendo el correo del agente");

                    try
                    {
                        AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient servicioAzman = new AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient("epAzman");
                        var datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                ConfigurationManager.AppSettings["DominioRed"],
                                agente.Usuario);

                        if (datosUsuario != null)
                        {
                            if (datosUsuario.Correo != null)
                            {
                                if (datosUsuario.Correo != "")
                                {
                                    correoAgente = datosUsuario.Correo;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        log.Warn("No se ha enviado mail al agente " + agente.Usuario + " porque no se ha podido obtener su email");
                    }

                    // Validar si es que el consentimiento ya existe
                    string url = string.Format(urlConsultaConsentimiento, Enums.TratamientoConsentimiento.RP.StringValue(), tipoDocumento, numeroDocumento, usuario);
                    log.Debug("Consumiendo endpoint: " + urlConsentimientoCliente);
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.Method = "GET";
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    ConsentimientoCliente consentimiento = null;
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        string responseBody = streamReader.ReadToEnd();
                        consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(responseBody);
                    }

                    log.Debug("Obteniendo el token: " + token);
                    if (consentimiento != null)
                    {
                        if (consentimiento.ind_consentimiento != "S")
                        {
                            // Actualizar el consentimiento si es que aún no ha sido firmado
                            Respuesta estadoCorreo = new Respuesta();

                            if (tipoDocumento == "" || tipoDocumento == null) tipoDocumento = "D";

                            consentimiento.ind_consentimiento = null;
                            consentimiento.gls_token = consentimiento.gls_token;
                            consentimiento.cod_tipo_identificacion = tipoDocumento;
                            consentimiento.gls_num_identificacion = numeroDocumento;
                            consentimiento.id_configuracion = Convert.ToInt32(Enums.ConfiguracionConsentimiento.RP.StringValue());
                            consentimiento.gls_nombres = nombres;
                            consentimiento.gls_apellido_paterno = apellidoPaterno;
                            consentimiento.gls_apellido_materno = apellidoMaterno;
                            consentimiento.gls_sexo = sexo;
                            consentimiento.fec_nacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE"));
                            consentimiento.gls_mail = correo;
                            consentimiento.gls_telefono = telefono;
                            consentimiento.gls_celular = celular;
                            consentimiento.gls_nombres_agente = ti.ToTitleCase(agente.Nombre.ToString().Trim().ToLower());
                            consentimiento.gls_mail_agente = correoAgente;
                            consentimiento.aud_usr_modificacion = usuario;
                            consentimiento.num_agente = Convert.ToInt32(agente.Id);

                            // Actualizar
                            log.Debug("Consumiendo endpoint: PUT " + urlConsentimientoCliente);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(urlConsentimientoCliente);

                            var context = new HttpContextWrapper(HttpContext.Current);
                            HttpRequestBase request = context.Request;
                            httpWebRequest.UserAgent = request.UserAgent;

                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "PUT";

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
                                var jsonResult = streamReader.ReadToEnd();
                                consentimiento = JsonConvert.DeserializeObject<ConsentimientoCliente>(jsonResult);
                            }

                            urlAppConsentimiento = string.Format(urlAppConsentimiento, consentimiento.gls_token);

                            DocumentoSME documentoSME = new DocumentoSME
                            {
                                Email = correo,
                                NumeroPoliza = "N/A",
                                NumeroDocumento = numeroDocumento,
                                Destinatario = string.Format("{0} {1} {2}", nombres, apellidoPaterno, apellidoMaterno).Trim(),
                                ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEConsentimientoRP"]),
                                CamposDinamicos = new
                                {
                                    Id_nombres = ti.ToTitleCase(nombres.ToLower().Trim()),
                                    Id_link = urlAppConsentimiento,
                                    Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                                }
                            };

                            log.Debug("Enviando el correo SME");
                            if (flagCorreoCliente == "S")
                            {
                                estadoCorreo = Utilitario.EnviarDocumentoSME(documentoSME);
                                if (estadoCorreo.Estado != Constante.COD_OK)
                                {
                                    log.Error(string.Format("Error al enviar el correo del cliente [{0}]", correo));
                                    throw new Exception("Error al enviar el correo del cliente.");
                                }

                                dynamic respDocumentoSME = JsonConvert.DeserializeObject(estadoCorreo.Mensaje);
                                long idSME = respDocumentoSME.codigoSME;

                                // Insertar en la tabla de seguimiento
                                EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                                {
                                    gls_identificador = string.Format("{0}|{1}", tipoDocumento, numeroDocumento),
                                    id_proceso_envio = (int)Enums.ProcesoEnvio.ConsentimientoRP,
                                    id_sme = idSME,
                                    cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                    gls_mail = correo,
                                    fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                                    cod_agente = agente.Id,
                                    aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                                };
                                string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                                var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                                string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                                log.Debug("Consumiendo API: " + rutaEnvioSeguimiento);
                                log.Debug(string.Format("Request Body [{0}]", jsonString));
                                using (var client = new WebClient())
                                {
                                    client.Encoding = Encoding.UTF8;
                                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                    respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                                    respuesta.Estado = Constante.COD_OK;
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
                        if (tipoDocumento == "" || tipoDocumento == null) tipoDocumento = "D";

                        consentimiento = new ConsentimientoCliente();
                        consentimiento.cod_tipo_identificacion = tipoDocumento;
                        consentimiento.gls_num_identificacion = numeroDocumento;
                        consentimiento.id_configuracion = Convert.ToInt32(Enums.ConfiguracionConsentimiento.RP.StringValue());
                        consentimiento.gls_nombres = nombres;
                        consentimiento.gls_apellido_paterno = apellidoPaterno;
                        consentimiento.gls_apellido_materno = apellidoMaterno;
                        consentimiento.gls_sexo = sexo;
                        consentimiento.fec_nacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE"));
                        consentimiento.gls_mail = correo;
                        consentimiento.gls_telefono = telefono;
                        consentimiento.gls_celular = celular;
                        consentimiento.gls_nombres_agente = ti.ToTitleCase(agente.Nombre.ToString().Trim().ToLower());
                        consentimiento.gls_mail_agente = correoAgente;
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

                        Respuesta estadoCorreo = new Respuesta();

                        urlAppConsentimiento = string.Format(urlAppConsentimiento, consentimiento.gls_token);
                        DocumentoSME documentoSME = new DocumentoSME
                        {
                            Email = correo,
                            NumeroPoliza = "N/A",
                            NumeroDocumento = numeroDocumento,
                            Destinatario = string.Format("{0} {1} {2}", nombres, apellidoPaterno, apellidoMaterno).Trim(),
                            ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEConsentimientoRP"]),
                            CamposDinamicos = new
                            {
                                Id_nombres = ti.ToTitleCase(nombres.ToLower().Trim()),
                                Id_link = urlAppConsentimiento,
                                Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                            }
                        };

                        log.Debug("Enviando el correo SME");
                        if (flagCorreoCliente == "S")
                        {
                            estadoCorreo = Utilitario.EnviarDocumentoSME(documentoSME);
                            if (estadoCorreo.Estado != Constante.COD_OK)
                            {
                                log.Error(string.Format("Error al enviar el correo del cliente [{0}]", correo));
                                throw new Exception("Error al enviar el correo del cliente.");
                            }

                            dynamic respDocumentoSME = JsonConvert.DeserializeObject(estadoCorreo.Mensaje);
                            long idSME = respDocumentoSME.codigoSME;

                            // Insertar en la tabla de seguimiento
                            EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                            {
                                gls_identificador = string.Format("{0}|{1}", tipoDocumento, numeroDocumento),
                                id_proceso_envio = (int)Enums.ProcesoEnvio.ConsentimientoRP,
                                id_sme = idSME,
                                cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                gls_mail = correo,
                                fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                                cod_agente = agente.Id,
                                aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                            };
                            string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                            var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                            string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                            log.Debug("Consumiendo endpoint de seguimiento: " + rutaEnvioSeguimiento);
                            log.Debug(string.Format("Request Body[{0}]", jsonString));
                            using (var client = new WebClient())
                            {
                                client.Encoding = Encoding.UTF8;
                                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                                respuesta.Estado = Constante.COD_OK;
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
                    log.Error("Error: " + Utilitarios.FormatearError(new List<String> { ex.Message }));
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
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

        //[WebMethod]
        //public static Respuesta FormatoConsentimientoAsesoria(string cuspp)
        //{
        //    using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
        //    {
        //        Respuesta respuesta = new Respuesta();
        //        try
        //        {
        //            log.Info("Accediendo a las Key necesarias: token");
        //            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
        //            string urlFormatoADN = ConfigurationManager.AppSettings["url_formato_adn"].ToString();

        //            string usuario = (string)HttpContext.Current.Session["Usuario"].ToString();
        //            string token_generado = string.Empty;

        //            log.Info("Consumiendo servicio token: " + urlToken);
        //            /*Obtener token*/
        //            var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
        //            httpWebRequest.ContentType = "application/json";
        //            httpWebRequest.Method = "POST";

        //            log.Info("Pasando el json al servicio");
        //            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        //            {
        //                string json = "{\"usuario\": \"" + usuario + "\"}";

        //                streamWriter.Write(json);
        //                streamWriter.Flush();
        //                streamWriter.Close();
        //            }

        //            log.Info("Leyendo el servicio");
        //            var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        //            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
        //            {
        //                var jsonResult = streamReader.ReadToEnd();
        //                JObject jObject = JObject.Parse(jsonResult);
        //                token_generado = (string)jObject["accessToken"];
        //            }

        //            /*Obtener indicador cliente permitido*/
        //            if (token_generado != "")
        //            {
        //                log.Info("Consumiendo servicio pdf: " + urlFormatoADN);

        //                urlFormatoADN = string.Format(urlFormatoADN, cuspp, usuario);

        //                log.Info("Leyendo el servicio");

        //                WebClient myWebClient = new WebClient();
        //                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token_generado));
        //                myWebClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

        //                byte[] formatoByteArray = myWebClient.DownloadData(urlFormatoADN);
        //                myWebClient.Dispose();

        //                File.WriteAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\ArchivosTemporales\\RVI\\Consentimiento\\Consentimiento_" + cuspp + ".pdf", formatoByteArray);
        //                respuesta.Estado = Constante.COD_OK;
        //                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
        //                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
        //                respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Consentimiento y protección de datos personales generado correctamente." });
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error("Error: " + Utilitarios.FormatearError(new List<String> { ex.Message }));
        //            respuesta.Estado = Constante.COD_ERROR;
        //            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        //            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        //            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
        //        }

        //        return respuesta;
        //    }
        //}

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
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
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

            using (var rng = new RNGCryptoServiceProvider())
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
            // Consentimiento de Asesoría
            DateTime fechaConsentimiento = DateTime.Now;
            string consentimientoToken = string.Empty;
            string telefonoCliente = string.Empty;
            string celularCliente = string.Empty;
            string correoCliente = string.Empty;
            string tratamientoConsentimiento = string.Empty;

            if (afiliado.IdConsentimientoAsesoria == null || afiliado.IdConsentimientoAsesoria.Trim().Length == 0)
            {
                afiliado.IdConsentimientoAsesoria = "0";
            }

            bool ind_consentimiento = consentimiento(Convert.ToInt32(Enums.ConfiguracionConsentimiento.RP.StringValue()), TipoDocumento_RP.SelectedValue, NumeroDocumento_RP.Text, (string)Session["Usuario"], ref fechaConsentimiento, ref consentimientoToken, ref telefonoCliente, ref celularCliente, ref correoCliente, ref tratamientoConsentimiento);

            HConsentimiento.Value = "NO";
            if (ind_consentimiento)
            {
                HConsentimiento.Value = "OK";
            }

            HToken_RPP.Value = consentimientoToken;

            if (ind_consentimiento)
            {
                var glsfechaConsentimiento = fechaConsentimiento.ToString("dd'/'MM'/'yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                cuadroMensajeConsentimiento.tieneConsentimiento = true;
                cuadroMensajeConsentimiento.Clase = "grilla_exito";
                cuadroMensajeConsentimiento.Mensaje = "Este cliente dio su consentimiento de asesoría " + tratamientoConsentimiento + " el " + glsfechaConsentimiento + ".<br><br>";
                cuadroMensajeConsentimiento.Mensaje += "<a id=\"PlantillaConsentimientoAsesoria_RPP\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">file_download</span>Descargar formato de consentimiento de asesoría</a><br>";
                cuadroMensajeConsentimiento.Mensaje += "<a id=\"ReenviarPlantillaConsentimientoAsesoria_RPP\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">email</span>Reenviar consentimiento de asesoría</a>";
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

                if (afiliado.Telefonos.Trim().Length > 0 && afiliado.Telefonos != null)
                {
                    Telefono_RP.Text = afiliado.Telefonos;
                }

                if (afiliado.Celulares.Trim().Length > 0 && afiliado.Celulares != null)
                {
                    Celular_RP.Text = afiliado.Celulares;
                }

                ControlLabel(Telefono_RP);
                ControlLabel(Celular_RP);
                ControlLabel(CorreoElectronico_RP);

                var resultadoValidacionConsentimiento = validacionConsentimientoAsesoria(Nombres_RP.Text, ApellidoPaterno_RP.Text, ApellidoMaterno_RP.Text, TipoDocumento_RP.SelectedValue, NumeroDocumento_RP.Text, CorreoElectronico_RP.Text, Telefono_RP.Text, Celular_RP.Text, SaldoCIC_RP.Text, RangoInversion_RP.Text, CentroLaboral_RP.Text, direccion);

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
                    ConsentimientoMensaje cuadroMensajeConsentimiento = (ConsentimientoMensaje)LoadControl("~/Controles/ConsentimientoMensaje.ascx");
                    cuadroMensajeConsentimiento.tieneConsentimiento = true;
                    cuadroMensajeConsentimiento.Clase = "mensaje_advertencia_amarillo";
                    cuadroMensajeConsentimiento.Mensaje = "Este cliente no ha brindado su consentimiento de asesoría para el producto <b>Renta Particular</b>.<br><br>";
                    cuadroMensajeConsentimiento.Mensaje += "<a id=\"LinkConsentimientoAsesoria_RPP\"><span class=\"material-icons\" style=\"font-size:20px;margin-right:5px;vertical-align:bottom\">email</span><span>Enviar enlace de consentimiento a <b>" + afiliado.CorreoElectronico.ToString() + "</b></span></a>";

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
                log.Debug("Inicio Cotizador.servicioCotizador.ActualizarAfiliado");
                servicioCotizador.ActualizarAfiliado(afiliado);
                log.Debug("Fin Cotizador.servicioCotizador.ActualizarAfiliado");
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
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
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

                                string nombreTerminal = string.Empty;
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
        public static Respuesta ImprimirPoliza(string numPoliza)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var respuesta = new Respuesta();

                    log.Info("Accediendo a las Key necesarias");
                    string urlPdfPoliza = ConfigurationManager.AppSettings["url_pdf_emision_poliza_admwr"].ToString();
                    string usuarioAdmwrApi = ConfigurationManager.AppSettings["usuario_admwr_api"].ToString();
                    string contraseñaAdmwrApi = ConfigurationManager.AppSettings["contraseña_admwr_api"].ToString();

                    var wc = new WebClient();
                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuarioAdmwrApi + ":" + contraseñaAdmwrApi));
                    wc.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

                    log.Info("Consumiendo método para generar pdf de póliza");
                    urlPdfPoliza = string.Format(urlPdfPoliza, numPoliza);
                    byte[] polizaByteArray = wc.DownloadData(urlPdfPoliza);
                    wc.Dispose();

                    var nombreArchivo = "Poliza.pdf";
                    var rutaCarpeta = HostingEnvironment.MapPath("~") + $"\\ArchivosTemporales\\RPP\\Poliza\\";
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
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
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

                    SolicitudRPPlus solicitudRPPlus = servicioCotizador.ObtenerDatosSolicitudRPPlus(numSolicitud);

                    solicitud_edn.num_solicitud = solicitudRPPlus.Id;
                    solicitud_edn.fec_solicitud = solicitudRPPlus.FechaSolicitud.Value;
                    solicitud_edn.val_mto_cta_individual = solicitudRPPlus.PrimaUnica;
                    solicitud_edn.cod_moneda_cta_indiv = solicitudRPPlus.MonedaPrimaUnica.Id;
                    
                    var listaCotiza = solicitudRPPlus.Cotizaciones;
                    
                    var beneficiarios = solicitudRPPlus.Beneficiarios;

                    estudioNecesidadAPI.solicitud = solicitud_edn;

                    if (listaCotiza != null)
                    {
                        foreach (var cot in listaCotiza)
                        {
                            cotizacion_edn = new CotizacionEdNAPI();
                            cotizacion_edn.num_correlativo = (int)cot.Correlativo;

                            cotizacion_edn.cod_tipo_temporalidad = solicitudRPPlus.Temporalidad.Id;

                            cotizacion_edn.val_per_diferido = cot.PeriodoDiferido;

                            cotizaciones_edn.Add(cotizacion_edn);
                        }

                        estudioNecesidadAPI.cotizaciones = new List<CotizacionEdNAPI>();
                        estudioNecesidadAPI.cotizaciones = cotizaciones_edn;
                    }

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



    }
}