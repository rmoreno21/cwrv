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

namespace Interseguro.CWRV.Presentacion.ASPNET.Cotizador
{
    public partial class CotizadorOficiales : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(CotizadorOficiales));
        private static IServicioCWRV servicioCotizador;

        private static SolicitudEscenario sol;

        protected void Page_Load(object sender, EventArgs e)
        {

            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionOficial))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.CotizacionOficial.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(string.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
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
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }

        }

        private void CargarInformacionInicialPantalla()
        {

            //servicioCotizador = LocalizadorProxy.ObtenerServicio();
            //List<Parametro> parametroTabla = servicioCotizador.ObtenerParametrosPorTabla("");

            //HttpContext.Current.Session["ParametroCita"] = parametroTabla;


            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            CargarCombobox(ModSolCategoria, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Categoria]);

            //switch (((string)Session["RolAzman"]).Substring(0, 3))
            switch ((string)Session["RolAzman"])
            {
                case "AST.RVI.COM":
                case "GTE.DIV.RVI":
                case "JEF.RVI.OPE":
                    ControlJefe.Visible = true;
                    CargarCombobox(OfiJefe, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.IdNivel == 1), true);
                    OfiJefe.Enabled = true;

                    ControlSupervisor.Visible = true;
                    CargarCombobox(OfiSupervisor, new List<Agente>(), true);
                    OfiSupervisor.Enabled = false;

                    ControlAgente.Visible = true;
                    CargarCombobox(OfiAgente, new List<Agente>(), true);
                    OfiAgente.Enabled = false;
                    break;
                case "JEF.VTA.LIM.RVI":
                case "JEF.VTA.PRO.RVI":
                    ControlJefe.Visible = true;
                    CargarCombobox(OfiJefe, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.Usuario == (string)Session["Usuario"]), false);
                    OfiJefe.Enabled = false;

                    ControlSupervisor.Visible = true;

                    // Lista de supervisores
                    List<Agente> listaSupervisores = ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).FindAll(a => a.IdNivel == 2 && a.IdPadre == OfiJefe.SelectedValue);

                    // Eliminar los duplicados
                    listaSupervisores =
                        listaSupervisores
                            .GroupBy(s => s.Id)
                            .Select(s => s.First())
                            .ToList();

                    CargarCombobox(OfiSupervisor, listaSupervisores, true);
                    OfiSupervisor.Enabled = true;

                    ControlAgente.Visible = true;
                    CargarCombobox(OfiAgente, new List<Agente>(), true);
                    OfiAgente.Enabled = false;
                    break;
                case "SPV.LIM.RVI":
                case "SPV.PRO.RVI":
                    ControlJefe.Visible = false;

                    ControlSupervisor.Visible = true;
                    CargarCombobox(OfiSupervisor, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.Usuario == (string)Session["Usuario"]), false);
                    OfiSupervisor.Enabled = false;

                    ControlAgente.Visible = true;

                    // Lista de agentes
                    List<Agente> listaAgentes = ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).FindAll(a => a.IdNivel == 3 && a.IdPadre == OfiSupervisor.SelectedValue);

                    // Eliminar los duplicados
                    listaAgentes =
                        listaAgentes
                            .GroupBy(a => a.Id)
                            .Select(a => a.First())
                            .ToList();

                    CargarCombobox(OfiAgente, listaAgentes, true);
                    OfiAgente.CssClass = "formCombobox";
                    OfiAgente.Enabled = true;
                    break;
                case "AGT.LIM.RVI":
                case "AGT.PRO.RVI":
                    ControlJefe.Visible = false;

                    ControlSupervisor.Visible = false;

                    ControlAgente.Visible = true;
                    CargarCombobox(OfiAgente, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.Usuario == (string)Session["Usuario"]), false);
                    OfiAgente.Enabled = false;
                    break;
            }

            //<SOLINI25781>
            //Validando si es Visible
            //<INIGTI_4081>//Se Comenta
            ////switch ((string)Session["RolAzman"])
            ////{
            ////    case "AGT.LIM.RVI"://AgenteLima
            ////    case "AGT.PRO.RVI"://AgenteProvincia
            ////    case "SPV.LIM.RVI"://SupervisorLima
            ////    case "SPV.PRO.RVI"://SupervisorProvincia
            ////    case "JEF.VTA.LIM.RVI"://JefeVentaLima
            ////    case "JEF.VTA.PRO.RVI"://JefeVentaProvincia
            ////    case "GTE.DIV.RVI"://GerenteDivision
            ////        HOcultraColumnaTRA.Value = "TRUE";
            ////        break;

            ////    case "AST.RVI.COM"://AsistenteComercial
            ////    case "JEF.RVI.OPE"://JefeOperaciones
            ////    case "AST.RVI.OPE"://AsistenteOperaciones
            ////        HOcultraColumnaTRA.Value = "FALSE";
            ////        break;
            ////}
            //<FINGTI_4081>
            HOcultraColumnaTRA.Value = "FALSE";
            //<SOLFIN25781>

            // Combobox Ind. Seleccinado
            ModSolIndSeleccionado.Items.Add(new ListItem("Sí", "S"));
            ModSolIndSeleccionado.Items.Add(new ListItem("No", "N"));

            //<INIGTI_1092>
            HRedLocal.Value = Utilitarios.ValidarRedLocal(Request.ServerVariables["remote_addr"]).ToString().ToUpper();
            //<FINGTI_1092>

            //<INIGTI_4081>
            HMaxPBS.Value = ConfigurationManager.AppSettings["MaxItemPBS"];
            
            CargarCombobox(ModSolCompania, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Compania]);
            //<FINGTI_4081>

            //<GTI.INI-29372>
            string codRol = (string)HttpContext.Current.Session["RolAzman"];
            bool permisoModificarEnvioObligatorio;
            switch (codRol)
            {
                case "JEF.RVI.OPE"://JefeOperaciones
                    permisoModificarEnvioObligatorio = true;
                    break;
                default:
                    permisoModificarEnvioObligatorio = false;
                    break;
            };

            hdnMostrarEnvioObligatorio.Value = (permisoModificarEnvioObligatorio) ? "1" : "0";
            //<GTI.FIN-29372>

        }

        private void CargarCombobox(DropDownList control, List<Agente> combobox, bool todos)
        {
            control.Items.Clear();
            if (todos)
            {
                control.Items.Add(new ListItem("«Todos»", "0"));
            }
            foreach (Agente item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
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

        [WebMethod]
        public static string CargarComboSupervisoresOficiales(string idJefe)
        {
            var pagina = new Page();
            var control = (ComboboxSupervisoresOficial)pagina.LoadControl("~/Controles/ComboboxSupervisoresOficial.ascx");

            control.Supervisores = ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).FindAll(a => a.IdNivel == 2 && a.IdPadre == idJefe);

            // Eliminar los duplicados
            control.Supervisores =
                control.Supervisores
                    .GroupBy(s => s.Id)
                    .Select(s => s.First())
                    .ToList();

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
        public static string CargarComboAgentesOficiales(string idSupervisor)
        {
            var pagina = new Page();
            var control = (ComboboxAgentesOficial)pagina.LoadControl("~/Controles/ComboboxAgentesOficial.ascx");

            control.Agentes = ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).FindAll(a => a.IdNivel == 3 && a.IdPadre == idSupervisor);

            // Eliminar los duplicados
            control.Agentes =
                control.Agentes
                    .GroupBy(a => a.Id)
                    .Select(a => a.First())
                    .ToList();

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
        public static string CargarTablaSolicitudesOficiales(string tokenUsuario, string numJefe, string numSupervisor, string numAgente, string idSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesOficiales)pagina.LoadControl("~/Controles/TablaSolicitudesOficiales.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudConsultar))
                        {

                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            if (ValidarDatosJefatura(errores, controles, numJefe, numSupervisor, numAgente))
                            {

                                string codUserName = (string)HttpContext.Current.Session["Usuario"];
                                string codRol = (string)HttpContext.Current.Session["RolAzman"];
                                
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                List<SolicitudEscenario> solicitudesEscenario = servicioCotizador.ListarSolicitudEscenario(numJefe, numSupervisor, numAgente, codUserName, codRol);

                                control.SolicitudesEscenario = solicitudesEscenario;
                                control.SolicitudResaltar = idSolicitud;
                                control.PermisoConsultar = true;
                                control.PermisoModificar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudActualizar)) ? true : false;
                                control.PermisoCorreoElectronico = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudEnviarCorreo)) ? true : false;
                                control.PermisoExportarPDF = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudExportarPDF)) ? true : false;
                                control.PermisoReporteEscenario = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudReporteEscenarios)) ? true : false;
                                control.PermisoSolicitudAnticipo = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudAnticipo)) ? true : false;
                                control.Consentimiento = true;//(bool)HttpContext.Current.Session["Consentimiento"];
                                control.PermisoValidacionesACOM = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudValidacionesACOM)) ? true : false;
                                control.PermisoValidacionesDTRA = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudValidacionesDTRA)) ? true : false;

                                // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                                control.RedLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);

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
        public static string CargarTablaCotizacionesOficiales(List<Cotizacion> cotizaciones, Int64 numCotizacionElegida, Boolean permisoTRA, Boolean permisoRadio)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    //<SOLINI25781>
                    //Validando para ocultar la columna 10 "TRA", estos roles no veran la columna TRA
                    //<INIGTI_4081>//Se comenta la validación
                    ////switch ((string)HttpContext.Current.Session["RolAzman"])
                    ////{
                    ////    case "AGT.LIM.RVI"://AgenteLima
                    ////    case "AGT.PRO.RVI"://AgenteProvincia
                    ////    case "SPV.LIM.RVI"://SupervisorLima
                    ////    case "SPV.PRO.RVI"://SupervisorProvincia
                    ////    case "JEF.VTA.LIM.RVI"://JefeVentaLima
                    ////    case "JEF.VTA.PRO.RVI"://JefeVentaProvincia
                    ////    case "GTE.DIV.RVI"://GerenteDivision
                    ////        permisoTRA = false;
                    ////        break;
                    ////    //case "AST.RVI.COM"://AsistenteComercial
                    ////    //case "JEF.RVI.OPE"://JefeOperaciones
                    ////    //case "AST.RVI.OPE"://AsistenteOperaciones
                    ////}
                    //<FINGTI_4081>
                    //<SOLFIN25781>

                   
                    var pagina = new Page();
                    var control = (TablaCotizacionesOficiales)pagina.LoadControl("~/Controles/TablaCotizacionesOficiales.ascx");

                    control.Cotizaciones = cotizaciones;
                    //<INIGTI_4081>
                    if (permisoTRA)
                    { 
                        control.PermisoTRA = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoTRA)) ? true : false; 
                    }
                    else
                    {
                        control.PermisoTRA = false;
                        control.Modo = "N";
                    }
                    //control.PermisoTRA = permisoTRA;
                    //<FINGTI_4081>
                    control.PermisoRadio = permisoRadio;
                    control.CotizacionElegida = numCotizacionElegida;
                    control.MostrarPorcentajeCapital = false;//<INIGTI_4081>
                    control.MostrarTasas = false;
                    //<INIGTI_1092>
                    control.RedLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);
                    //<FINGTI_1092>
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

        private static bool ValidarDatosJefatura(List<string> errores, List<string> controles, string idJefe, string idSupervisor, string idAgente)
        {
            bool esCorrecto = true;

            // Jefe
            bool jefe = true;

            // Supervisor
            bool supervisor = true;

            // Agente
            bool agente = true;

            // Clases de controles
            if (!jefe) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!supervisor) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!agente) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            
            esCorrecto = jefe & supervisor & agente;

            return esCorrecto;
        }

        [WebMethod]
        public static SolicitudEscenario ObtenerDatosSolicitudEscenario(string numSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                sol = new SolicitudEscenario();
                string codUserName = (string)HttpContext.Current.Session["Usuario"];
                string codRol = (string)HttpContext.Current.Session["RolAzman"];

                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                sol = servicioCotizador.ObtenerDatosSolicitudEscenario(numSolicitud, codUserName, codRol);
                return sol;
            }
        }

        [WebMethod]
        public static Solicitud ObtenerDatosSolicitud(string idSolicitud, string fecCotizacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                fecCotizacion = fechaCotizacion.ToString("yyyyMMdd");

                Solicitud sol;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                sol = servicioCotizador.ObtenerDatosSolicitud(idSolicitud, fechaCotizacion);

                HttpContext.Current.Session["SolicitudOficial"] = sol;

                return sol;
            }

            //using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            //{
            //    //Datos afiliado
            //    servicioCotizador = LocalizadorProxy.ObtenerServicio();
            //    Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado("", numCuspp);
                
            //    //Datos Fecha solicitud
            //    servicioCotizador = LocalizadorProxy.ObtenerServicio();
            //    List<Solicitud> solicitudes = servicioCotizador.ListarSolicitud(numCuspp);
            //    Solicitud solFecha = new Solicitud();
            //    solFecha = solicitudes.Where(x => x.Id == numSolicitud).First();

            //    DateTime fechaCotizacion = Convert.ToDateTime(solFecha.FechaCotizacion, new CultureInfo("es-PE"));
                
            //    //datos Solicitud
            //    Solicitud sol=new Solicitud();
            //    servicioCotizador = LocalizadorProxy.ObtenerServicio();
            //    sol = servicioCotizador.ObtenerDatosSolicitud(idSolicitud, fechaCotizacion);

            //    HttpContext.Current.Session["Vendedor"] = afiliado.Agente.Id;
            //    HttpContext.Current.Session["Cartera"] = afiliado.Agente.IdCartera;
            //    HttpContext.Current.Session["SolicitudOficial"] = sol;

            //    return sol;
            //}
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
        public static string CargarTablaBeneficiarios(List<GrupoFamiliar> beneficiarios)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();

                        var control = (TablaRviBenefi)pagina.LoadControl("~/Controles/TablaRviBenefi.ascx");
                        control.Beneficiarios = beneficiarios;
                        control.Consentimiento = true;//(bool)HttpContext.Current.Session["Consentimiento"];
                        HttpContext.Current.Session["Beneficiarios"] = control.Beneficiarios;
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
        public static SolicitudEscenario InsertarSolicitud(string tokenUsuario,
                                                           string idSolicitud,
                                                           string fechaCotizacion,
                                                           string acom,
                                                           string dcom,
                                                           string indEstadoSeleccion,
                                                           string valMtoAgenteAcom,
                                                           string numCotizacionElegida,
                                                           string correo,
                                                           string cod_compania //<INIGTI_4081>
                                                )
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudEscenario solEscenario;
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudInsertar))
                        {
                            // Obtener los datos del escenario original
                            SolicitudEscenario sol = ObtenerDatosSolicitudEscenario(idSolicitud);

                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            if (ValidarSolicitud(errores, controles, fechaCotizacion, acom, dcom, indEstadoSeleccion, valMtoAgenteAcom, null, sol.Afiliado.CUSPP, Convert.ToDateTime(sol.FechaPresentacion), Convert.ToDateTime(sol.FechaCierreLote), correo, sol.Agente.Id, sol.ValidarACOM, sol.ValidarDTRA, idSolicitud, numCotizacionElegida, tokenUsuario))//INI.GTI_7012_2
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == sol.Agente.Id))
                                {
                                    
                                    double valTasaTra = 0;

                                    solEscenario = new SolicitudEscenario
                                    {
                                        NumSolicitud = idSolicitud,
                                        NumOperacion = sol.NumOperacion,//
                                        CodPjeCesionComision = Convert.ToDouble(dcom),
                                        PjeAumentoComision = Convert.ToDouble(acom),
                                        ValTasaAjusteTra = valTasaTra,
                                        IndCondicionEspecial = sol.IndCondicionEspecial,
                                        IndAprueba = (sol.IndAprueba != null) ? sol.IndAprueba : String.Empty,
                                        FecCierre = sol.FecCierre,
                                        Agente = sol.Agente,
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] },
                                        IndEstadoSeleccion = indEstadoSeleccion,
                                        NumSolicitudCopia = idSolicitud,//
                                        ValMtoAgenteAcom = Convert.ToDouble(valMtoAgenteAcom),
                                        NumCotizacionElegida = Convert.ToInt64(numCotizacionElegida),
                                        FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                        Compania = new Compania { Id=cod_compania}//<INIGTI_4081>
                                    };

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.RegistrarSolicitudEscenario(ref solEscenario);

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
                                        Detalle = "Solicitud registrada: " + solEscenario.NumSolicitud + ", ACOM: " + acom + ", DCOM: " + dcom
                                    });
                                }
                                else
                                {
                                    solEscenario = new SolicitudEscenario();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                solEscenario = new SolicitudEscenario();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            solEscenario = new SolicitudEscenario();
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
                        solEscenario = new SolicitudEscenario();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    solEscenario.Respuesta = respuesta;
                    return solEscenario;
                }
                catch (Exception ex)
                {
                    solEscenario = new SolicitudEscenario();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    solEscenario.Respuesta = respuesta;
                    return solEscenario;
                }
            }
        }

        [WebMethod]
        public static SolicitudEscenario ModificarSolicitud(string tokenUsuario,
                                                            string idSolicitud,
                                                            string fechaCotizacion,
                                                            string acom,
                                                            string dcom,
                                                            string indEstadoSeleccion,
                                                            string valMtoAgenteAcom,
                                                            string numCotizacionElegida,
                                                            List<Cotizacion> cotizaciones,
                                                            string correo,
                                                            string cod_compania //<INIGTI_4081>
            )
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudEscenario solEscenario;
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudActualizar))
                        {
                            // Obtener los datos del escenario original
                            SolicitudEscenario sol = ObtenerDatosSolicitudEscenario(idSolicitud);

                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            if (ValidarSolicitud(errores, controles, fechaCotizacion, acom, dcom, indEstadoSeleccion, valMtoAgenteAcom, cotizaciones, sol.Afiliado.CUSPP, Convert.ToDateTime(sol.FechaPresentacion), Convert.ToDateTime(sol.FechaCierreLote), correo, sol.Agente.Id, sol.ValidarACOM, sol.ValidarDTRA, idSolicitud, numCotizacionElegida, tokenUsuario))//INI.GTI_7012_2
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == sol.Agente.Id))
                                {
                                    //<INIGTI_4081>
                                    //for (int i = 0; i < cotizaciones.Count(); i++)
                                    //{
                                    //    cotizaciones[i].AjusteTRA = 0.0;
                                    //}
                                    //<FINGTI_4081>

                                    double valTasaTra = 0;

                                    solEscenario = new SolicitudEscenario
                                    {
                                        NumSolicitud = idSolicitud,
                                        NumOperacion = sol.NumOperacion,//
                                        CodPjeCesionComision = Convert.ToDouble(dcom),
                                        PjeAumentoComision = Convert.ToDouble(acom),
                                        ValTasaAjusteTra = valTasaTra,
                                        IndCondicionEspecial = sol.IndCondicionEspecial,
                                        IndAprueba = (sol.IndAprueba != null) ? sol.IndAprueba : String.Empty,
                                        FecCierre = sol.FecCierre,
                                        Agente = sol.Agente,
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] },
                                        IndEstadoSeleccion = indEstadoSeleccion,
                                        NumSolicitudCopia = idSolicitud,//
                                        ValMtoAgenteAcom = Convert.ToDouble(valMtoAgenteAcom),
                                        NumCotizacionElegida = Convert.ToInt64(numCotizacionElegida),
                                        FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                        Cotizaciones = cotizaciones,
                                        Compania = new Compania { Id=cod_compania}//<INIGTI_4081>
                                    };

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                    //<INIGTI_4081>
                                    Solicitud solicitud = new Solicitud
                                    {
                                        Id = idSolicitud,
                                        Agente = sol.Agente,
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] },
                                        FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                        Cotizaciones = cotizaciones,
                                        Compania = new Compania { Id = cod_compania }//<INIGTI_4081>
                                    };

                                    
                                    respuesta =  servicioCotizador.CotizarDifTRA(ref solicitud, tokenUsuario);

                                    if (respuesta.Estado == Constante.COD_ERROR)
                                    {
                                        solEscenario = new SolicitudEscenario();
                                        respuesta.Estado = Constante.COD_ERROR;
                                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                        respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                        solEscenario.Respuesta = respuesta;
                                        return solEscenario;
                                    }

                                    solEscenario.Cotizaciones = solicitud.Cotizaciones;
                                    //<FINGTI_4081>

                                    respuesta = servicioCotizador.ActualizarSolicitudEscenario(ref solEscenario, true);
                                    
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

                                    string tra = string.Empty;
                                    foreach (var itemCot in solEscenario.Cotizaciones)
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
                                        Detalle = "Solicitud actualizada: " + solEscenario.NumSolicitud + ", ACOM: " + acom + ", DCOM: " + dcom + ", DTRA: " + tra
                                    });
                                }
                                else
                                {
                                    solEscenario = new SolicitudEscenario();
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                solEscenario = new SolicitudEscenario();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            solEscenario = new SolicitudEscenario();
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
                        solEscenario = new SolicitudEscenario();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    solEscenario.Respuesta = respuesta;
                    return solEscenario;
                }
                catch (Exception ex)
                {
                    solEscenario = new SolicitudEscenario();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    solEscenario.Respuesta = respuesta;
                    return solEscenario;
                }
            }
        }

        private static bool ValidarSolicitud(List<String> errores, List<String> controles, string fecCotizacion, string valAcom, string valDcom, string indSeleccionado, string montoAgente, List<Cotizacion> listaCotizaciones, string cuspp, DateTime fechaPlazoAFP, DateTime fechaCierreLote, string correo, string numAgenteSol, bool validarACOM, bool validarDTRA, string idSolicitud, string idCotizacion, string sTokenUsuario)//INI.GTI_7012_2
        {
            bool esCorrecto;

            //<SRIINI25781>
            // Validar que la cotización se está realizando antes de la hora de cierre del Lote
            double segundos = (fechaCierreLote - DateTime.Now.AddTicks(-(DateTime.Now.Ticks % TimeSpan.TicksPerSecond))).TotalSeconds;
            if (segundos <= 0)
            {
                errores.Add("No se puede guardar la información porque se ha superado la hora de cierre: " + fechaCierreLote.ToString("hh:mm tt"));
                return false;
            }
            //<SRIFIN25781>
            
            //<INIGTI_4081>
            if (listaCotizaciones != null)
            {
                int MaxItemPBS = Convert.ToInt32(ConfigurationManager.AppSettings["MaxItemPBS"]);

                if (listaCotizaciones.Where(p => p.pbs != 0).Count() > MaxItemPBS)
                {
                    errores.Add("Solo puede ingresar <strong>" + MaxItemPBS + "</strong> PBS");
                    return false;
                }
            }            
            //<FINGTI_4081>

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            bool redLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);

            //<SRI.INI-20322_E2>
            //bool valRequisitosTra = true;

            // Recuperamos sesión de parámetros
            List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];
            // Recuperamos lista de parámetros
            string numAgente = numAgenteSol;

            // ACOM
            bool acom = true;
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
                    else
                    {   
                        //Validar Correo
                        if (correo != "" && correo != null)
                        {
                            if (!Utilitario.ValidarParametrosCorreo(listaParametro, correo))
                            {
                                errores.Add("El <strong>Correo Electrónico</strong> de la oportunidad debe contener el símbolo de @.");
                                acom = false;
                            }
                        }
                        else
                        {
                            errores.Add("Por ahora usted no puede continuar con la cotización debido a que la oportunidad no cuenta con <strong>Correo Electrónico</strong>.");
                            acom = false;
                        }

                        if (numAgente != "" && numAgente != null)
                        {
                            if (vAcom > 0)
                            {
                                //<INI.GTI_7012_2>
                                if (!ValidarAgenteDeuda(numAgente))
                                {
                                    errores.Add("No puede solicitar <strong>Pje ACOM</strong> porque tiene deuda pendiente.");
                                    acom = false;
                                }
                                //<FIN.GTI_7012_2>

                                if (validarACOM)
                                {
                                    //Validar PreCubo
                                    if (!Utilitario.ValidarPreCubo(listaParametro, numAgente, cuspp))
                                    {
                                        List<Parametro> listaPreCubos = listaParametro.Where(x => x.Id == Enums.ParametroTabla.Cubo.StringValue()).ToList();
                                        int total = listaPreCubos.Count;
                                        int contador = 0;
                                        string valores = String.Empty;
                                        //char[] car = new char[] {}
                                        foreach (Parametro precubo in listaPreCubos)
                                        {
                                            valores += precubo.Valor_1 + ", ";
                                            contador++;
                                            if (contador == total - 1)
                                            {
                                                valores = valores.Trim(new char[] { ' ', ',' });
                                                valores += " o ";
                                            }
                                            if (contador == total)
                                            {
                                                valores = valores.Trim(new char[2] { ' ', ',' });
                                            }
                                        }
                                        errores.Add(String.Format("Para poder realizar cambios en el <strong>Porcentaje A</strong> debe tener una de las siguientes calificaciones de <strong>Pre Cubo: {0}</strong>.", valores));
                                        acom = false;
                                    }

                                    //Validar Rango Visita
                                    if (!Utilitario.ValidarVisita(listaParametro, numAgente, cuspp, fechaPlazoAFP))
                                    {
                                        // Armar el mensaje de error
                                        List<Parametro> listaParametroVisitaCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.VisitaCita.StringValue()).ToList();
                                        List<Parametro> listaParametroEstadosCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.EstadoCita.StringValue()).OrderBy(x => x.Correlativo).ToList();
                                        string mensaje = String.Format("Para porder realizar cambios en el <strong>Porcentaje A</strong> debe tener una cita <strong>{0}</strong> o <strong>{1} desde {2} días antes hasta {3} días después</strong> de la <strong>Fecha de Plazo de la AFP</strong>.",
                                            listaParametroEstadosCita[0].Valor_2,
                                            listaParametroEstadosCita[1].Valor_2,
                                            listaParametroVisitaCita[0].Valor_1,
                                            listaParametroVisitaCita[1].Valor_1);

                                        errores.Add(mensaje);
                                        acom = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            errores.Add("El agente no puede ser validado, ya que el usuario no tiene número de Agente asignado.");
                            acom = false;
                        }
                    }
                }
            }

            //<SRIINI10693>
            /* Implementacion ACOM, solamente cuando al configuracion sea S */
            string KeyAcom = (string)ConfigurationManager.AppSettings["keyAcom"];
            RolAcom rolAcom = new RolAcom
            {
                CodRol = (string)HttpContext.Current.Session["RolAzman"],
                FechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE")),
            };

            servicioCotizador = LocalizadorProxy.ObtenerServicio();

            List<RolAcom> listaRol = servicioCotizador.ListarRolAcom(rolAcom);
            double vAcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));
            if (acom && KeyAcom == "S" && listaRol.Count > 0)
            {

                if (valAcom.Trim().Length == 0) valAcom = "0";

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

            // DCOM
            bool dcom = true;
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

            //<SRI.INI-20322_E2>
            /*Implementacion ACOM, solamente cuando al configuracion sea S*/
            string KeyDcom = (string)ConfigurationManager.AppSettings["keyDcom"];
            RolDcom rolDcom = new RolDcom
            {
                CodRol = (string)HttpContext.Current.Session["RolAzman"]
                ,
                FechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE")),
            };
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<RolDcom> listaRolDcom = servicioCotizador.ListarRolDcom(rolDcom);

            double vDcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));
            //<SOLINI25781>
            string dcomRangos = String.Empty;
            //<SOLFIN25781>
            if (dcom && KeyDcom == "S" && listaRolDcom.Count > 0)
            {
                if (valDcom.Trim().Length == 0) valDcom = "0";

                double vDcomDouble = Convert.ToDouble(valDcom, new CultureInfo("es-PE"));
                if (vDcomDouble != vDcomComp)
                {
                    foreach (RolDcom rol in listaRolDcom)
                    {
                        //<SOLINI25781>
                        dcomRangos += "[" + rol.NumRangoIni + "-" + rol.NumRangoFin + "] ";
                        //<SOLFIN25781>
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
                //<SOLINI25781>
                //errores.Add("El campo <strong>Porcentaje D</strong> no se encuentra dentro de los rangos permitidos.");
                errores.Add(String.Format("El campo <strong>Porcentaje D</strong> no se encuentra dentro de los rangos permitidos ({0}).", dcomRangos.TrimEnd()));
                //<SOLFIN25781>
            }

            //<SRI.FIN-20322_E2>

            //<SRI.INI-20322_E2>
            // Indica Seleccionado
            bool indicaSeleccionado = true;
            if (indSeleccionado.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Ind. Seleccionado</strong>. Dato Obligatorio.");
                indicaSeleccionado = false;
            }
            else
            {
                if (indSeleccionado != Enums.Seleccion.Si.StringValue() && indSeleccionado != Enums.Seleccion.No.StringValue())
                {
                    errores.Add("El campo <strong>Ind. Seleccionado</strong> debe S/N.");
                    indicaSeleccionado = false;
                }
            }
            //<SRI.FIN-20322_E2>

            //<SRI.INI-20322_E2>
            // Monto Agente
            bool valMontoAgente = true;
            if (montoAgente.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Monto A</strong>. Dato Obligatorio.");
                valMontoAgente = false;
            }
            else
            {
                double vMontoAgente;
                if (!double.TryParse(montoAgente, NumberStyles.Any, new CultureInfo("es-PE"), out vMontoAgente))
                {
                    errores.Add("El campo <strong>Monto A</strong> debe ser númerico.");
                    valMontoAgente = false;
                }
                else
                {
                    if (vMontoAgente < 0)
                    {
                        errores.Add("El campo <strong>Monto A</strong> debe contener un valor positivo.");
                        valMontoAgente = false;
                    }
                    //<INI.GTI_7012_2>//Se comenta, porque el montoAcom varia si se coloca ACOM,DCOM
                    else
                    {

                    }
                    ////else {
                    ////    if (acom)
                    ////    {
                    ////        if (indSeleccionado == "S")
                    ////        {
                    ////            Respuesta rptMontoAcom = new Respuesta();
                    ////            double vAcomDbl = Convert.ToDouble(valAcom, new CultureInfo("es-PE"));
                    ////            rptMontoAcom = ObtenerMontoACOM(sTokenUsuario, idSolicitud, vAcomDbl, Convert.ToInt32(idCotizacion));
                    ////            double vMontoAcom = 0;
                    ////            vMontoAcom = Convert.ToDouble(rptMontoAcom.Contenido.ToString());
                    ////            if (vMontoAgente > vMontoAcom)
                    ////            {
                    ////                errores.Add("El campo <strong>Monto A</strong> es mayor de lo permitido.");
                    ////                valMontoAgente = false;
                    ////            }
                    ////        }
                            
                    ////    }
                    ////}
                    //<FIN_GTI_7012_2>
                }
            }
            //<SRI.FIN-20322_E2>

            // Clases de controles
            if (!acom) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!dcom) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!indicaSeleccionado) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!valMontoAgente) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            // Cotizaciones
            bool cotizaciones = true;
            bool traMayorCero = false;
            if (listaCotizaciones != null)
            {
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

                    // Producto
                    if (listaCotizaciones[i].Producto.Id == "0")
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Producto</strong>. Dato Obligatorio.");
                        cotizaciones = false;
                        controles.Add(i + ",3");
                    }

                    // Modalidad
                    if (listaCotizaciones[i].Modalidad.Id == "0")
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Modalidad</strong>. Dato Obligatorio.");
                        cotizaciones = false;
                        controles.Add(i + ",4");
                    }

                    //<SOLINIGTI_754>
                    // Período Diferido
                    ////if ((listaCotizaciones[i].Modalidad.Id != "D" && listaCotizaciones[i].PeriodoDiferido != 0)
                    ////    || (listaCotizaciones[i].Modalidad.Id == "D" && listaCotizaciones[i].PeriodoDiferido == 0))
                    ////{
                    ////    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Período Diferido</strong> tiene un valor no válido para la Modalidad seleccionada.");
                    ////    cotizaciones = false;
                    ////    controles.Add(i + ",5");
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
                    ////    errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Porcentaje Renta</strong> tiene un valor no válido para la Modalidad seleccionada.");
                    ////    cotizaciones = false;
                    ////    controles.Add(i + ",6");
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
                    if (!(listaCotizaciones[i].Capital.Id == "-" || listaCotizaciones[i].Capital.Id == "03" || listaCotizaciones[i].Capital.Id == "04"))
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: El campo <strong>Capital</strong> tiene un valor no válido.");
                        cotizaciones = false;
                        controles.Add(i + ",9");
                    }

                    // Ajuste TRA
                    if (listaCotizaciones[i].AjusteTRA.ToString().Length == 0)
                    {
                        errores.Add("Cotización <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Dif. TRA</strong>. Dato Obligatorio.");
                        cotizaciones = false;
                    }

                    if (Math.Abs((double)listaCotizaciones[i].AjusteTRA) > 0)
                    {
                        traMayorCero = true;
                    }
                }
            }

            //if (valRequisitosTra && traMayorCero)
            if (traMayorCero)
            {

                if (correo != "" && correo != null)
                {
                    //Validar Correo
                    if (!Utilitario.ValidarParametrosCorreo(listaParametro, correo))
                    {
                        errores.Add("El <strong>CORREO</strong> debe contener el símbolo de @.");
                        acom = false;
                    }
                }
                else
                {
                    errores.Add("Debe ingresar un <strong>Correo Electrónico</strong> a la oportunidad para poder realizar la cotización.");
                    acom = false;
                }

                if (numAgente != "" && numAgente != null)
                {
                    if (validarDTRA)
                    {
                        //Validar PreCubo
                        if (!Utilitario.ValidarPreCubo(listaParametro, numAgente, cuspp))
                        {
                            List<Parametro> listaPreCubos = listaParametro.Where(x => x.Id == Enums.ParametroTabla.Cubo.StringValue()).ToList();
                            int total = listaPreCubos.Count;
                            int contador = 0;
                            string valores = String.Empty;
                            foreach (Parametro precubo in listaPreCubos)
                            {
                                valores += precubo.Valor_1 + ", ";
                                contador++;
                                if (contador == total - 1)
                                {
                                    valores = valores.Trim(new char[] { ' ', ',' });
                                    valores += " o ";
                                }
                                if (contador == total)
                                {
                                    valores = valores.Trim(new char[2] { ' ', ',' });
                                }
                            }
                            errores.Add(String.Format("Para poder realizar cambios en el <strong>DTRA</strong> debe tener una de las siguientes calificaciones de <strong>Pre Cubo: {0}</strong>.", valores));
                            acom = false;
                        }
                    
                        //Validar Rango Visita
                        if (!Utilitario.ValidarVisita(listaParametro, numAgente, cuspp, fechaPlazoAFP))
                        {
                            List<Parametro> listaParametroVisitaCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.VisitaCita.StringValue()).ToList();
                            List<Parametro> listaParametroEstadosCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.EstadoCita.StringValue()).OrderBy(x => x.Correlativo).ToList();
                            string mensaje = String.Format("Para porder realizar cambios en el <strong>DTRA</strong> debe tener una cita <strong>{0}</strong> o <strong>{1} desde {2} días antes hasta {3} días después</strong> de la <strong>Fecha de Plazo de la AFP</strong>.",
                                listaParametroEstadosCita[0].Valor_2,
                                listaParametroEstadosCita[1].Valor_2,
                                listaParametroVisitaCita[0].Valor_1,
                                listaParametroVisitaCita[1].Valor_1);

                            errores.Add(mensaje);
                            acom = false;
                        }
                    }
                }
                else
                {
                    errores.Add("El agente no puede ser validado, ya que el usuario no tiene número de Agente asignado.");
                    acom = false;
                }

            }

            esCorrecto = indicaSeleccionado & valMontoAgente & acom & dcom & cotizaciones;

            return esCorrecto;
        }

        [WebMethod]
        public static SolicitudEscenario InsertarSolicitudExtraoficial(string tokenUsuario,
                                                                       string numSolicitud,
                                                                       string fechaCotizacion,
                                                                       string numAgente)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudEscenario solEscenario;
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudInsertar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == numAgente))
                            {

                                solEscenario = new SolicitudEscenario
                                {

                                    NumSolicitud = numSolicitud,
                                    Agente = new Agente { Id = numAgente },
                                    Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] },
                                    NumSolicitudCopia = numSolicitud,
                                    FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE"))
                                };

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.RegistrarSolicitudEscenarioExtraoficial(ref solEscenario);

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
                                    Detalle = "Solicitud registrada: " + solEscenario.NumSolicitud + ", ACOM: " + solEscenario.PjeAumentoComision + ", DCOM: " + solEscenario.CodPjeCesionComision
                                });
                            }
                            else
                            {
                                solEscenario = new SolicitudEscenario();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }

                        }
                        else
                        {
                            solEscenario = new SolicitudEscenario();
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
                        solEscenario = new SolicitudEscenario();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    solEscenario.Respuesta = respuesta;
                    return solEscenario;
                }
                catch (Exception ex)
                {
                    solEscenario = new SolicitudEscenario();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    solEscenario.Respuesta = respuesta;
                    return solEscenario;
                }
            }
        }

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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}])", ex.Message), ex);
                    throw (ex);
                }


            }
        }

        [WebMethod]
        public static Respuesta ExportarReporteEscenariosPDF(string tokenUsuario, string idSolicitud, string fecCotizacion, string maxAcom, string idAgente)
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
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == idAgente))
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

                                    try
                                    {
                                        nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                    }
                                    catch (Exception)
                                    {
                                        log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                            HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                    }

                                }
                                else if (respuesta.Estado == Constante.COD_ERROR)
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { respuesta.Mensaje });
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

        [WebMethod]
        public static Respuesta RegistrarCotizacionMovimiento(double acom, string montoAcom, string estadoSeleccion, string elegida, List<Cotizacion> listaCotizaciones, string cod_compania, double dcom)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
                {
                    // Validar que se ha marcado "Sí" en el Indicador de Solicitud Seleccionda
                    if (estadoSeleccion == "S")
                    {
                        
                        string nombreTerminal = String.Empty;

                        Solicitud solicitud = (Solicitud)HttpContext.Current.Session["SolicitudOficial"];
                        solicitud.PorcentajeAumentoComision = acom;

                        //<INIGTI_6556>
                        log.Info(String.Format("Inicio Principal Oficial de envío al flujo, solicitud Nro. [{0}].", solicitud.Id ));
                        //<FINGTI_6556>


                        //<INIGTI_4081_2>
                        solicitud.PorcentajeDescuentoComision = dcom;
                        //<FINGTI_4081_2>
                        //<INIGTI_4081_3>
                        solicitud.MontoAumentoComision=Convert.ToDouble(montoAcom, new CultureInfo("es-PE"));
                        //<INIGTI_4081_3>
                        
                        //<INIGTI_6556>
                        //SolicitudEscenario escenario = ObtenerDatosSolicitudEscenario(solicitud.Id);
                        //<FINGTI_6556>
                        List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];

                        //<INIGTI_6556>
                        //escenario.Compania = new Compania { Id = cod_compania };//<INIGTI_4081>
                        //<FINGTI_6556>
                        List<String> errores = new List<String>();
                        //List<String> controles = new List<String>();

                        // Validar ACOM
                        bool vAcom = true;
                        if (solicitud.PorcentajeAumentoComision > 0)
                        {

                            if (solicitud.ValidarACOM)//escenario.ValidarACOM
                            {
                                //Validar PreCubo
                                if (!Utilitario.ValidarPreCubo(listaParametro, solicitud.Agente.Id, solicitud.Afiliado.CUSPP))
                                {
                                    List<Parametro> listaPreCubos = listaParametro.Where(x => x.Id == Enums.ParametroTabla.Cubo.StringValue()).ToList();
                                    int total = listaPreCubos.Count;
                                    int contador = 0;
                                    string valores = String.Empty;
                                    //char[] car = new char[] {}
                                    foreach (Parametro precubo in listaPreCubos)
                                    {
                                        valores += precubo.Valor_1 + ", ";
                                        contador++;
                                        if (contador == total - 1)
                                        {
                                            valores = valores.Trim(new char[] { ' ', ',' });
                                            valores += " o ";
                                        }
                                        if (contador == total)
                                        {
                                            valores = valores.Trim(new char[2] { ' ', ',' });
                                        }
                                    }
                                    errores.Add(String.Format("Para poder realizar cambios en el <strong>Porcentaje A</strong> debe tener una de las siguientes calificaciones de <strong>Pre Cubo: {0}</strong>.", valores));
                                    vAcom = false;
                                }

                                //Validar Rango Visita
                                if (!Utilitario.ValidarVisita(listaParametro, solicitud.Agente.Id, solicitud.Afiliado.CUSPP, (DateTime)solicitud.FechaPlazoAFP))
                                {
                                    // Armar el mensaje de error
                                    List<Parametro> listaParametroVisitaCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.VisitaCita.StringValue()).ToList();
                                    List<Parametro> listaParametroEstadosCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.EstadoCita.StringValue()).OrderBy(x => x.Correlativo).ToList();
                                    string mensaje = String.Format("Para porder realizar cambios en el <strong>Porcentaje A</strong> debe tener una cita <strong>{0}</strong> o <strong>{1} desde {2} días antes hasta {3} días después</strong> de la <strong>Fecha de Plazo de la AFP</strong>.",
                                        listaParametroEstadosCita[0].Valor_2,
                                        listaParametroEstadosCita[1].Valor_2,
                                        listaParametroVisitaCita[0].Valor_1,
                                        listaParametroVisitaCita[1].Valor_1);

                                    errores.Add(mensaje);
                                    vAcom = false;
                                }
                            }
                        }

                        // Validar DTRA
                        bool vDtra = true;
                        bool tieneDTRA = false;
                        for (int i = 0; i < listaCotizaciones.Count; i++)
                        {
                            if (Math.Abs((double)listaCotizaciones[i].AjusteTRA) > 0)
                            {
                                tieneDTRA = true;
                            }
                        }
                        if (tieneDTRA)
                        {
                            if (solicitud.ValidarDTRA)//escenario.ValidarDTRA
                            {
                                //Validar PreCubo
                                if (!Utilitario.ValidarPreCubo(listaParametro, solicitud.Agente.Id, solicitud.Afiliado.CUSPP))
                                {
                                    List<Parametro> listaPreCubos = listaParametro.Where(x => x.Id == Enums.ParametroTabla.Cubo.StringValue()).ToList();
                                    int total = listaPreCubos.Count;
                                    int contador = 0;
                                    string valores = String.Empty;
                                    foreach (Parametro precubo in listaPreCubos)
                                    {
                                        valores += precubo.Valor_1 + ", ";
                                        contador++;
                                        if (contador == total - 1)
                                        {
                                            valores = valores.Trim(new char[] { ' ', ',' });
                                            valores += " o ";
                                        }
                                        if (contador == total)
                                        {
                                            valores = valores.Trim(new char[2] { ' ', ',' });
                                        }
                                    }
                                    errores.Add(String.Format("Para poder realizar cambios en el <strong>DTRA</strong> debe tener una de las siguientes calificaciones de <strong>Pre Cubo: {0}</strong>.", valores));
                                    vDtra = false;
                                }

                                //Validar Rango Visita
                                if (!Utilitario.ValidarVisita(listaParametro, solicitud.Agente.Id, solicitud.Afiliado.CUSPP, (DateTime)solicitud.FechaPlazoAFP))
                                {
                                    List<Parametro> listaParametroVisitaCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.VisitaCita.StringValue()).ToList();
                                    List<Parametro> listaParametroEstadosCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.EstadoCita.StringValue()).OrderBy(x => x.Correlativo).ToList();
                                    string mensaje = String.Format("Para porder realizar cambios en el <strong>DTRA</strong> debe tener una cita <strong>{0}</strong> o <strong>{1} desde {2} días antes hasta {3} días después</strong> de la <strong>Fecha de Plazo de la AFP</strong>.",
                                        listaParametroEstadosCita[0].Valor_2,
                                        listaParametroEstadosCita[1].Valor_2,
                                        listaParametroVisitaCita[0].Valor_1,
                                        listaParametroVisitaCita[1].Valor_1);

                                    errores.Add(mensaje);
                                    vDtra = false;
                                }
                            }
                        }

                        if (vAcom & vDtra)
                        {                            
                            //<INIGTI_4081_2>
                            /*Implementacion DCOM, solamente cuando al configuracion sea S*/
                            string KeyDcom = (string)ConfigurationManager.AppSettings["keyDcom"];
                            RolDcom rolDcom = new RolDcom
                            {
                                CodRol = (string)HttpContext.Current.Session["RolAzman"],
                                FechaCotizacion = Convert.ToDateTime(solicitud.FechaCotizacion.Value, new CultureInfo("es-PE")),
                            };
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<RolDcom> listaRolDcom = servicioCotizador.ListarRolDcom(rolDcom);

                            double vDcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));

                            bool bdcom = true;
                            string dcomRangos = String.Empty;

                            if (KeyDcom == "S" && listaRolDcom.Count > 0)
                            {
                                double vDcomDouble = Convert.ToDouble(dcom, new CultureInfo("es-PE"));
                                if (vDcomDouble != vDcomComp)
                                {
                                    foreach (RolDcom rol in listaRolDcom)
                                    {
                                        dcomRangos += "[" + rol.NumRangoIni + "-" + rol.NumRangoFin + "] ";
                                        if (!(vDcomDouble >= rol.NumRangoIni && vDcomDouble <= rol.NumRangoFin))
                                        {
                                            bdcom = false;
                                        }
                                        else
                                        {
                                            bdcom = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!bdcom)
                            {
                                throw new Exception(String.Format("El campo <strong>Porcentaje D</strong> no se encuentra dentro de los rangos permitidos ({0}).", dcomRangos.TrimEnd()));
                            }

                            //<INIGTI_4081_2>

                            string url =
                                HttpContext.Current.Request.Url.Scheme + "://" +
                                HttpContext.Current.Request.Url.Authority +
                                HttpContext.Current.Request.ApplicationPath +
                                (HttpContext.Current.Request.ApplicationPath == "/" ? String.Empty : "/") +
                                "Bandejas/BandejaFlujoCotizacion.aspx";

                            solicitud.Compania = new Compania { Id = cod_compania };

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

                            string tra = string.Empty;
                            foreach (var itemCot in listaCotizaciones)
                            {
                                tra += "[" + itemCot.Correlativo + ": " + itemCot.AjusteTRA + "] ";
                            }

                            servicioCotizador.RegistrarLog(new LogBD
                            {
                                IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                NombreTerminal = nombreTerminal,
                                IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                IdTipoEvento = Enums.EventoLog.FlujoAprobacion.StringValue(),
                                //Detalle = String.Format("Movimientos TRA de la Solicitud", solicitud.Id)
                                Detalle = "Flujo de aprobación - Solicitud: " + solicitud.Id + ", ACOM: " + acom + ", DCOM: " + dcom + ", DTRA: " + tra
                            });
                            
                            respuesta = Utilitario.RegistrarCotizacionMovimiento(ref listaCotizaciones, solicitud, false, true, url, elegida);

                            if (respuesta.Estado == Constante.COD_OK)
                            {
                                //<INIGTI_6556>
                                log.Info(String.Format("Inicio obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", solicitud.Id));
                                //<FINGTI_6556>

                                //<INIGTI_4081_3>
                                List<SolicitudEscenario> lstSolicitudEscenario;
                                lstSolicitudEscenario = servicioCotizador.ListarSolicitudesEmail(solicitud.Id);

                                //<INIGTI_6556>
                                log.Info(String.Format("Fin obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", solicitud.Id));
                                //<FINGTI_6556>

                                //Aqui
                                Utilitario.EnviarEmail(lstSolicitudEscenario, url, respuesta);
                                //<FINGTI_4081_3>

                            }

                            //<INIGTI_6556>
                            log.Info(String.Format("Fin Principal Oficial de envío al flujo, solicitud Nro. [{0}].", solicitud.Id));
                            //<FINGTI_6556>

                            
                        }
                        else
                        {
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(errores);
                        }
                    }
                    else
                    {
                        respuesta.Estado = Constante.COD_ERROR;
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                        respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                        respuesta.Mensaje = "Debe marcar <strong>Sí</strong> en el campo <strong>Ind. Seleccionado</strong> para poder enviar esta solicitud al flujo de aprobación";
                    }
                }
            }
            catch (Exception ex)
            {
                //log.Error(String.Format("Se ha producido el siguiente error: {0}", ex.Message), ex);
                log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }

            return respuesta;

        }

        [WebMethod]
        public static Respuesta ObtenerMontoACOM(string tokenUsuario, string solicitud, double acom, long cotizacion)
        {
            Respuesta respuesta = new Respuesta();
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.ObtenerMontoACOM(solicitud, acom, cotizacion);
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: {0}", ex.Message), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }
            }
            return respuesta;
        }

        [WebMethod]
        public static Respuesta ValidarACOMyTRA(string tokenUsuario, string fechaCotizacion, double acom, List<Cotizacion> cotizaciones, bool rechazo)
        {
            //<SOLINI25781>
            bool bRespuesta = false;
            //<SOLFIN25781>

            Respuesta respuesta = new Respuesta();
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (!rechazo)//<INIGTI_4081>//
                    {
                        // Variables
                        bool vAcom = true;
                        bool vDtra = true;

                        
                        // Iniciar instancia al Web Service
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        // Validar ACOM
                        RolAcom rolAcom = new RolAcom
                        {
                            CodRol = (string)HttpContext.Current.Session["RolAzman"],
                            FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                        };
                        List<RolAcom> listaRolAcom = servicioCotizador.ListarRolAcom(rolAcom);
                        if (acom > 0)
                        {
                            if (listaRolAcom.Count > 0)
                            {
                                if (acom < listaRolAcom[0].NumRangoIni || acom > listaRolAcom[0].NumRangoFin)
                                {
                                    vAcom = false;
                                }
                            }
                        }

                        // Validar DTRA
                        RolDtra rolDtra = new RolDtra
                        {
                            RolAzman = (string)HttpContext.Current.Session["RolAzman"],
                            FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                        };

                        List<RolDtra> listaRolDtra = servicioCotizador.ListarRolDtra(rolDtra);
                        if (listaRolDtra.Count > 0)
                        {
                            //<INIGTI_6556>
                            // Obtener el TRA mínimo
                            double tramin = Convert.ToDouble(cotizaciones.OrderBy(x => x.AjusteTRA).ToList().First().AjusteTRA);
                            if (tramin < listaRolDtra[0].RangoInicial || tramin > listaRolDtra[0].RangoFinal)
                            {
                                vDtra = false;
                            }

                            double tramax = Convert.ToDouble(cotizaciones.OrderByDescending(x => x.AjusteTRA).ToList().First().AjusteTRA);
                            if (tramax < listaRolDtra[0].RangoInicial || tramax > listaRolDtra[0].RangoFinal)
                            {
                                vDtra = false;
                            }

                            //<FINGTI_6556>

                        }

                        bool rangoPermitido = vAcom & vDtra;

                        respuesta.Estado = Constante.COD_OK;
                        if (rangoPermitido)
                        {
                            respuesta.Contenido = String.Empty;

                            //<INIGTI_6842>
                            Solicitud solici = (Solicitud)HttpContext.Current.Session["SolicitudOficial"];

                            Solicitud solicitud = new Solicitud
                            {
                                Id = solici.Id,
                                //Agente = sol.Agente,
                                Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] },
                                FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                                Cotizaciones = cotizaciones
                            };

                            respuesta = servicioCotizador.CotizarDifTRA(ref solicitud, tokenUsuario);

                            //<INIGTI_6842>
                            string mensaje = String.Empty;
                            string persona = String.Empty;
                            string rolAzman = (string)HttpContext.Current.Session["RolAzman"];

                            solicitud.Cotizaciones
                                        .ForEach(c =>
                                        {
                                            if (c.IndCotiza == "**")
                                            {
                                                DateTime fecCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE"));

                                                List<FlujoMovimiento> ListaFlujo = servicioCotizador.ObtenerFlujos(fecCotizacion, "E", rolAzman);

                                                if (ListaFlujo != null)
                                                {
                                                    if (ListaFlujo.Count > 0)
                                                    {
                                                        persona = ListaFlujo[0].MsjAlerta;
                                                    }
                                                }
                                                mensaje = String.Format("Usted está intentando cotizar con parámetros no permitido, este escenario se enviará {0} para ser evaluado, una vez que se envíe este escenario <b>ya no podrá ser modificado después</b>. ¿Desea continuar?", persona);
                                            }
                                        });

                            respuesta.Mensaje = mensaje;
                            respuesta.Contenido = mensaje;
                            if (mensaje != "")
                            {
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Confirmacion.StringValue(); //"Confirmación";
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Estado = Constante.COD_OK;
                                respuesta.Contenido = mensaje;
                            }
                            
                            //<FINGTI_6842>
                        }
                        else
                        {
                            // Obtener condición
                            string condicion = String.Empty;
                            if (!vAcom & vDtra) condicion = "Porcentaje A";
                            if (vAcom & !vDtra) condicion = "TRA";
                            if (!vAcom & !vDtra) condicion = "Porcentaje A y TRA";

                            //<INIGTI_4081>
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Confirmacion.StringValue(); //"Confirmación";
                            //<FINGTI_4081>

                            // Obtener persona de aprobación
                            string persona = String.Empty;
                            string rolAzman = (string)HttpContext.Current.Session["RolAzman"];

                            //<INIGTI_4081>
                            DateTime fecCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE"));

                            List<FlujoMovimiento> ListaFlujo = servicioCotizador.ObtenerFlujos(fecCotizacion, "E", rolAzman);

                            if (ListaFlujo != null)
                            {
                                if (ListaFlujo.Count > 0)
                                {
                                    persona = ListaFlujo[0].MsjAlerta;
                                }
                            }

                            //<FINGTI_4081>

                            //////<INIGTI_4081>//SE COMENTA
                            ////if (rolAzman == Enums.RolAzman.AgenteLima.StringValue() || rolAzman == Enums.RolAzman.AgenteProvincia.StringValue())
                            ////{
                            ////    // Si el usuario es un agente validar si se le enviará al supervisor o a la asistente

                            ////    // Validar ACOM de Asistente
                            ////    bool vAsistenteACOM = true;
                            ////    rolAcom.CodRol = Enums.RolAzman.AsistenteComercial.StringValue();
                            ////    listaRolAcom = servicioCotizador.ListarRolAcom(rolAcom);
                            ////    if (listaRolAcom.Count > 0)
                            ////    {
                            ////        if (acom < listaRolAcom[0].NumRangoIni || acom > listaRolAcom[0].NumRangoFin)
                            ////        {
                            ////            vAsistenteACOM = false;
                            ////        }
                            ////        else
                            ////        {
                            ////            vAsistenteACOM = true;
                            ////        }
                            ////    }

                            ////    // Validar DTRA de Asistente
                            ////    bool vAsistenteTRA = true;

                            ////    rolDtra.RolAzman = Enums.RolAzman.AsistenteComercial.StringValue();
                            ////    listaRolDtra = servicioCotizador.ListarRolDtra(rolDtra);
                            ////    if (listaRolDtra.Count > 0)
                            ////    {
                            ////        if (tramax < listaRolDtra[0].RangoInicial || tramax > listaRolDtra[0].RangoFinal)
                            ////        {
                            ////            vAsistenteTRA = false;
                            ////        }
                            ////        else
                            ////        {
                            ////            vAsistenteTRA = true;
                            ////        }
                            ////    }

                            ////    if (vAsistenteACOM & vAsistenteTRA)
                            ////    {
                            ////        persona = "a la asistente de comercial";
                            ////        //<SOLINI25781>
                            ////        bRespuesta = true;
                            ////        respuesta.Titulo = Enums.CuadroMensajeTitulo.Confirmacion.StringValue(); //"Confirmación";
                            ////        //<SOLFIN25781>
                            ////    }
                            ////    else
                            ////    {
                            ////        persona = "a su supervisor";
                            ////    }


                            ////    persona = "a su supervisor";
                            ////}
                            ////else if (rolAzman == Enums.RolAzman.SupervisorLima.StringValue() || rolAzman == Enums.RolAzman.SupervisorProvincia.StringValue())
                            ////{
                            ////    persona = "a su jefe";
                            ////}
                            ////else if (rolAzman == Enums.RolAzman.JefeVentaLima.StringValue() || rolAzman == Enums.RolAzman.JefeVentaProvincia.StringValue())
                            ////{
                            ////    persona = "al Gerente de Rentas Vitalicias";
                            ////}
                            ////else if (rolAzman == Enums.RolAzman.JefeVentaLima.StringValue() || rolAzman == Enums.RolAzman.JefeVentaProvincia.StringValue())
                            ////{
                            ////    persona = "al área de Operaciones";
                            ////}
                            ////else if (rolAzman == Enums.RolAzman.AsistenteComercial.StringValue())
                            ////{
                            ////    persona = "al supervisor del agente";
                            ////}
                            //////<FINGTI_4081>

                            //<INIGTI_4081>
                            //////<SOLINI25781>
                            ////string mensaje = "";
                            ////if (bRespuesta == true)
                            ////{
                            ////    // Armar el mensaje de confirmación
                            ////    mensaje = String.Format("Usted está intentando cotizar con parámetros de {0} que exceden de su límite permitido, este escenario se enviará {1} para ser aprobado, una vez que se envíe este escenario <b>ya no podrá ser moficado después</b>. ¿Desea continuar?", condicion, persona);
                            ////}
                            ////else
                            ////{
                            ////    mensaje = String.Format("Usted está intentando cotizar con parámetros de {0} que exceden de su límite permitido.", condicion);
                            ////}
                            //////<SOLFIN25781>
                            //<INIGTI_4081>

                            //<INIGTI_4081>
                            string mensaje = "";
                            mensaje = String.Format("Usted está intentando cotizar con parámetros de {0} que exceden de su límite permitido, este escenario se enviará {1} para ser evaluado, una vez que se envíe este escenario <b>ya no podrá ser modificado después</b>. ¿Desea continuar?", condicion, persona);

                            //El jefe de operaciones no puede exceder su límite
                            if (rolAzman == Enums.RolAzman.JefeOperaciones.StringValue())
                            {
                                bool valida = true;
                                if (!vAcom && !vDtra)
                                {
                                    mensaje = "El porcentaje de <strong>Dif. TRA </strong> y <strong>ACOM</strong> se encuentra fuera del rango permitido";
                                    valida = false;
                                }

                                if (!vAcom)
                                {
                                    mensaje = "El porcentaje de <strong>ACOM</strong> se encuentra fuera del rango permitido";
                                    valida = false;
                                }

                                if (!vDtra)
                                {
                                    mensaje = "El porcentaje de <strong>Dif. TRA</strong> se encuentra fuera del rango permitido";
                                    valida = false;
                                }

                                if (!valida)
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                }
                                respuesta.Mensaje = mensaje;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            }

                            //<FINGTI_4081>
                            respuesta.Contenido = mensaje;
                        }
                    }
                    else {
                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Contenido = "";
                    }
                    
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: {0}", ex.Message), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }
            }
            return respuesta;
        }

        [WebMethod]
        public static Respuesta ValidarSolicitudAnticipo(string tokenUsuario, string solicitud, string agente)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudAnticipo))
                        {
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == agente))
                            {
                                // 1. Validar el estado del escenario
                                SolicitudEscenario escenario = ObtenerDatosSolicitudEscenario(solicitud);
                                if (escenario.TipoMovimiento.Id == (short)Enums.TipoMovimiento.SolicitudCotizada || escenario.TipoMovimiento.Id == (short)Enums.TipoMovimiento.SolicitudAprobada)
                                {
                                    // 2. Validar si ya se han aceptado las condiciones
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    Anticipo anticipo = servicioCotizador.ObtenerDatosAnticipoAceptacion(solicitud, agente);
                                    if (anticipo == null)
                                    {
                                        if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteLima.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteProvincia.StringValue())
                                        {
                                            HttpContext.Current.Session["Anticipo"] = servicioCotizador.ObtenerDatosAnticipoCondiciones(solicitud);
                                            respuesta.Estado = Constante.COD_OK;
                                            respuesta.Mensaje = "C";
                                        }
                                        else
                                        {
                                            respuesta.Estado = Constante.COD_ERROR;
                                            respuesta.Titulo = "Validación";
                                            respuesta.Icono = "validacion";
                                            respuesta.Mensaje = "El agente aún no ha aceptado los términos y condiciones de la Solicitud de Anticipo. De aceptarlas para poder imprimir el formato.";
                                        }
                                    }
                                    else
                                    {
                                        respuesta.Estado = Constante.COD_OK;
                                        respuesta.Mensaje = "R";
                                    }
                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = "Validación";
                                    respuesta.Icono = "validacion";
                                    respuesta.Mensaje = "Para generar una solicitud de anticipo, su escenario tiene que estar <strong>Cotizado</strong> o <strong>Aprobado</strong>. ";
                                    respuesta.Mensaje += "No es posible realizar la solicitud de anticipo en escenarios pendientes de aprobación o recazados.";
                                }
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
                                Enums.OpcionesSistema.SolicitudAnticipo.StringValue()));
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
                    respuesta.Titulo = "Error";
                    respuesta.Icono = "error";
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta GenerarReporteSolicitudAnticipo(string tokenUsuario, string solicitud, string agente)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        HttpContext.Current.Session["idSolicitud"] = solicitud;
                        HttpContext.Current.Session["AgenteReporte"] = agente;

                        respuesta.Estado = Constante.COD_OK;
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
                    respuesta.Titulo = "Error";
                    respuesta.Icono = "error";
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta ModificaValidacionesrSolicitud(string tokenUsuario,
                                                               string idSolicitud,
                                                               string idAgente,
                                                               string tipoValidacion,
                                                               string valor)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (
                            (tipoValidacion == "A" && Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudValidacionesACOM))
                            ||
                            (tipoValidacion == "T" && Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudValidacionesDTRA))
                           )
                        {
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == idAgente))
                            {
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.ActualizarValidacion(idSolicitud, tipoValidacion, valor, (string)HttpContext.Current.Session["Usuario"]);
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
                    log.Error("Error al activar o desactivar las validaciones de " + (tipoValidacion == "C" ? "Cita" : "Precubo"), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }
        //<SOLINI25621>
        [WebMethod]
        public static Respuesta ExportarReporte(string tokenUsuario, string numeroOperacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        HttpContext.Current.Session["numeroLote"] = "-1";
                        HttpContext.Current.Session["numeroOperacion"] = numeroOperacion;
                        respuesta.Estado = Constante.COD_OK;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }
        //<SOLFIN25621>


        //<INIGTI_4081>
        [WebMethod]
        public static Solicitud ObtenerDTra(string idSolicitud, string fechaCotizacion, string tokenUsuario, List<Cotizacion> cotizaciones)
        {
            Solicitud solicitud;
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    // Iniciar instancia al Web Service
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    solicitud = new Solicitud
                    {
                        Id = idSolicitud,
                        FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                        Cotizaciones = cotizaciones
                    };

                    respuesta = servicioCotizador.ObtenerDTra(ref solicitud, tokenUsuario);
                    solicitud.Respuesta = respuesta;
                    
                }
                catch (Exception ex)
                {
                    solicitud = new Solicitud();
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message });
                    solicitud.Respuesta = respuesta;
                }

                return solicitud;
            }
        }


        [WebMethod]
        public static Solicitud CalcularPBS(string idSolicitud, string fechaCotizacion, string tokenUsuario, List<Cotizacion> cotizaciones, double acom, double dcom, bool actualizaMovimiento, int tipoMovimiento, string montoAcom)
        {
            Solicitud solicitud;
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        // Iniciar instancia al Web Service
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        solicitud = new Solicitud
                        {
                            Id = idSolicitud,
                            FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                            PorcentajeAumentoComision = Convert.ToDouble(acom),
                            PorcentajeDescuentoComision = Convert.ToDouble(dcom),
                            MontoAumentoComision = Convert.ToDouble(montoAcom),
                            TipoMovimiento = new TipoMovimiento{Id= Convert.ToInt16(tipoMovimiento)},
                            Cotizaciones = cotizaciones
                        };

                        //<INIGTI_4081_2>
                        /*Implementacion ACOM, solamente cuando al configuracion sea S*/
                        string KeyDcom = (string)ConfigurationManager.AppSettings["keyDcom"];
                        RolDcom rolDcom = new RolDcom
                        {
                            CodRol = (string)HttpContext.Current.Session["RolAzman"],
                            FechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE")),
                        };
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<RolDcom> listaRolDcom = servicioCotizador.ListarRolDcom(rolDcom);

                        double vDcomComp = Convert.ToDouble(0, new CultureInfo("es-PE"));

                        bool bdcom=true;
                        string dcomRangos = String.Empty;
                        
                        if (KeyDcom == "S" && listaRolDcom.Count > 0)
                        {
                            double vDcomDouble = Convert.ToDouble(dcom, new CultureInfo("es-PE"));
                            if (vDcomDouble != vDcomComp)
                            {
                                foreach (RolDcom rol in listaRolDcom)
                                {
                                    dcomRangos += "[" + rol.NumRangoIni + "-" + rol.NumRangoFin + "] ";
                                    if (!(vDcomDouble >= rol.NumRangoIni && vDcomDouble <= rol.NumRangoFin))
                                    {
                                        bdcom = false;
                                    }
                                    else
                                    {
                                        bdcom = true;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!bdcom)
                        {
                            throw new Exception(String.Format("El campo <strong>Porcentaje D</strong> no se encuentra dentro de los rangos permitidos ({0}).", dcomRangos.TrimEnd()));
                        }


                        //<FINGTI_4081_2>


                        respuesta = servicioCotizador.CotizarDifTRA(ref solicitud, tokenUsuario);
                        //<INIGTI_4081_2>
                        if (respuesta.Estado == Constante.COD_OK) {
                            //Actualizará cwrv_cotiza_movimiento
                            if (actualizaMovimiento)
                            {
                                respuesta = Utilitario.ActualizaCotizacionMovimiento(solicitud);   
                            }
                        }
                        //<FINGTI_4081_2>
                        solicitud.Respuesta = respuesta;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                        solicitud = new Solicitud();
                        solicitud.Respuesta = respuesta;
                    }
                }
                catch (Exception ex)
                {
                    solicitud = new Solicitud();
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message });
                    solicitud.Respuesta = respuesta;
                }

                return solicitud;
            }
        }


        [WebMethod]
        public static Respuesta ValidarSeleccionSolicitud(string tokenUsuario, string NumSolicitud, string NumOperacion, string IndSeleccion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        string mensaje = "";
                        respuesta.Titulo = "";
                        if (IndSeleccion == "S")
                        {
                            // Iniciar instancia al Web Service
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<SolicitudEscenario> solicitudes = new List<SolicitudEscenario>();
                            solicitudes = servicioCotizador.ListarSolicitudesValidaFlujo(NumSolicitud, NumOperacion);
                            if (solicitudes.Count > 0)
                            {
                                mensaje = "Usted está intentando cotizar con el ";
                                mensaje += "campo <strong> Ind. Seleccionado </strong> en modo <strong> Sí </strong>, ";
                                mensaje += "esta solicitud sustituirá a la solicitud Nro. <strong>" + solicitudes[0].NumSolicitud + "</strong> que se encuentra en estado: <strong>" + solicitudes[0].TipoMovimiento.Nombre + "</strong>";
                                mensaje += "<br />";
                                mensaje += "¿Desea Continuar?";

                                respuesta.Titulo = "Confirmación";
                            }
                        }
                        respuesta.Contenido = mensaje;
                        respuesta.Estado = Constante.COD_OK;
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
                    respuesta.Titulo = "Error";
                    respuesta.Icono = "error";
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }

        //<FINGTI_4081>

        //<INI.GTI_7012_2>

        public static bool ValidarAgenteDeuda(string idAgente)
        {
            bool valida=true;
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Iniciar instancia al Web Service
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<Agente> lstAgente = new List<Agente>();
                    lstAgente = servicioCotizador.ObtenerAgenteDeudaAcom(idAgente);
                    if (lstAgente.Count > 0)
                    {
                        valida = false;
                    }
                    
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                }

                return valida;
            }
        }
        //<FIN.GTI_7012_2>

        [WebMethod]
        public static SolicitudEscenario ModificarSolicitudEnvioObligatorio(string tokenUsuario,
                                                                            string idSolicitud,
                                                                            List<Cotizacion> cotizaciones
                                                                            )
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                SolicitudEscenario solEscenario;
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudActualizar))
                        {
                            solEscenario = new SolicitudEscenario();
                            solEscenario.NumSolicitud = idSolicitud;
                            solEscenario.Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] };
                            solEscenario.Cotizaciones = cotizaciones;

                            respuesta = servicioCotizador.ActualizarSolicitudEnvioObligatorio(ref solEscenario);

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
                        }
                        else
                        {
                            solEscenario = new SolicitudEscenario();
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
                        solEscenario = new SolicitudEscenario();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    solEscenario.Respuesta = respuesta;
                    return solEscenario;
                }
                catch (Exception ex)
                {
                    solEscenario = new SolicitudEscenario();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    solEscenario.Respuesta = respuesta;
                    return solEscenario;
                }
            }
        }
    }
}