using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.ServiceModel;
using System.Reflection;
using System.Threading;
using System.Web.Services;

using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;

using log4net;

//<SRI.INI-20322>
using Interseguro.CWRV.Presentacion.ASPNET.Builder.ConsultasXLS;

using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.HPSF;
//<SRI.FIN-20322>

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class Supervision : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Seguimiento));
        private static IServicioCWRV servicioCotizador;

        //<SRI.INI-20322>        
        HSSFWorkbook hssfworkbook;
        //<SRI.FIN-20322>

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteSupervision))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                            LimpiarFormularios();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ReporteSupervision.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
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

        private void LimpiarFormularios()
        {
            //throw new NotImplementedException();
        }

        private void CargarInformacionInicialPantalla()
        {
            switch (((string)Session["RolAzman"]).Substring(0, 3))
            {
                case "ANL":
                case "AST":
                case "GTE":
                    ControlJefe.Visible = true;
                    CargarCombobox(Jefe, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.IdNivel == 1), true);
                    Jefe.Enabled = true;

                    ControlSupervisor.Visible = true;
                    CargarCombobox(Supervisor, new List<Agente>(), true);
                    Supervisor.Enabled = false;

                    ControlAgente.Visible = true;
                    CargarCombobox(Agente, new List<Agente>(), true);
                    Agente.Enabled = false;
                    break;
                case "JEF":
                    ControlJefe.Visible = true;
                    CargarCombobox(Jefe, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.Usuario == (string)Session["Usuario"]), false);
                    Jefe.Enabled = false;

                    ControlSupervisor.Visible = true;

                    // Lista de supervisores
                    List<Agente> listaSupervisores = ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).FindAll(a => a.IdNivel == 2 && a.IdPadre == Jefe.SelectedValue);

                    // Eliminar los duplicados
                    listaSupervisores =
                        listaSupervisores
                            .GroupBy(s => s.Id)
                            .Select(s => s.First())
                            .ToList();

                    CargarCombobox(Supervisor, listaSupervisores, true);
                    Supervisor.Enabled = true;

                    ControlAgente.Visible = true;
                    CargarCombobox(Agente, new List<Agente>(), true);
                    Agente.Enabled = false;
                    break;
                case "SPV":
                    ControlJefe.Visible = false;

                    ControlSupervisor.Visible = true;
                    CargarCombobox(Supervisor, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.Usuario == (string)Session["Usuario"]), false);
                    Supervisor.Enabled = false;

                    ControlAgente.Visible = true;

                    // Lista de agentes
                    List<Agente> listaAgentes = ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).FindAll(a => a.IdNivel == 3 && a.IdPadre == Supervisor.SelectedValue);

                    // Eliminar los duplicados
                    listaAgentes =
                        listaAgentes
                            .GroupBy(a => a.Id)
                            .Select(a => a.First())
                            .ToList();

                    CargarCombobox(Agente, listaAgentes, true);
                    Agente.CssClass = "formCombobox";
                    Agente.Enabled = true;
                    break;
                case "AGT":
                    ControlJefe.Visible = false;

                    ControlSupervisor.Visible = false;

                    ControlAgente.Visible = true;
                    CargarCombobox(Agente, ((List<Agente>)Session["ListaAgentes"]).FindAll(a => a.Usuario == (string)Session["Usuario"]), false);
                    Agente.Enabled = false;
                    break;
            }
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

        [WebMethod]
        public static string CargarComboSupervisores(string idJefe)
        {
            var pagina = new Page();
            var control = (ComboboxSupervisores)pagina.LoadControl("~/Controles/ComboboxSupervisores.ascx");

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
        public static string CargarComboAgentes(string idSupervisor)
        {
            var pagina = new Page();
            var control = (ComboboxAgentes)pagina.LoadControl("~/Controles/ComboboxAgentes.ascx");

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
        public static Respuesta CargarTablaSupervision(string tokenUsuario, string idJefe, string idSupervisor, string idAgente, string fechaInicio, string fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar)
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
                        if (ValidarDatosSupervision(errores, controles, idJefe, idSupervisor, idAgente, fechaInicio, fechaTermino))
                        {
                            DateTime fecInicio = Convert.ToDateTime(fechaInicio, new CultureInfo("es-PE"));
                            DateTime fecTermino = Convert.ToDateTime(fechaTermino, new CultureInfo("es-PE"));

                            var pagina = new Page();
                            var control = (TablaSupervision)pagina.LoadControl("~/Controles/TablaSupervision.ascx");

                            if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteSupervision))
                            {
                                int totalRegistros = 0;

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                List<Dominio.Entidades.Supervision> supervision = servicioCotizador.ListarSupervision(idJefe, idSupervisor, idAgente, fecInicio, fecTermino, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar, ref totalRegistros);

                                HttpContext.Current.Session["SegJefe"] = idJefe;
                                HttpContext.Current.Session["SegSupervisor"] = idSupervisor;
                                HttpContext.Current.Session["SegAgente"] = idAgente;

                                control.DatosReporte = supervision;
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
                                    Enums.OpcionesSistema.ReporteSupervision.StringValue()));
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

        private static bool ValidarDatosSupervision(List<string> errores, List<string> controles, string idJefe, string idSupervisor, string idAgente, string fecInicio, string fecTermino)
        {
            bool esCorrecto = true;

            // Jefe
            bool jefe = true;

            // Supervisor
            bool supervisor = true;

            // Agente
            bool agente = true;

            // Fecha de Inicio
            bool fechaInicio = true;
            if (fecInicio.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Inicio</strong>. Dato Obligatorio.");
                fechaInicio = false;
            }
            else
            {
                DateTime vFechaInicio;
                if (!DateTime.TryParse(fecInicio, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaInicio))
                {
                    errores.Add("El campo <strong>Fecha de Inicio</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaInicio = false;
                }
            }

            // Fecha de Término
            bool fechaTermino = true;
            if (fecTermino.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Término</strong>. Dato Obligatorio.");
                fechaTermino = false;
            }
            else
            {
                DateTime vFechaTermino;
                if (!DateTime.TryParse(fecTermino, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaTermino))
                {
                    errores.Add("El campo <strong>Fecha de Término</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaTermino = false;
                }
            }

            // Consistencia de fechas
            if (fechaInicio & fechaTermino)
            {
                DateTime vFechaInicio = Convert.ToDateTime(fecInicio, new CultureInfo("es-PE"));
                DateTime vFechaTermino = Convert.ToDateTime(fecTermino, new CultureInfo("es-PE"));
                if (vFechaTermino < vFechaInicio)
                {
                    errores.Add("<strong>Fecha de Inicio</strong> no puede ser mayor a la <strong>Fecha de término</strong>. Verifique.");
                    fechaInicio = false;
                    fechaTermino = false;
                }
            }

            // Clases de controles
            if (!jefe) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!supervisor) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!agente) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!fechaInicio) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!fechaTermino) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }

            esCorrecto = jefe & supervisor & agente & fechaInicio & fechaTermino;

            return esCorrecto;
        }

        protected void ExportarExcel_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    //<SRI.INI-20322>
                    //DateTime fecInicio = Convert.ToDateTime(HFechaInicio.Value, new CultureInfo("es-PE"));
                    //DateTime fecTermino = Convert.ToDateTime(HFechaTermino.Value, new CultureInfo("es-PE"));

                    //var pagina = new Page();
                    //var control = (TablaSupervisionExcel)pagina.LoadControl("~/Controles/TablaSupervisionExcel.ascx");

                    //if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteSupervision))
                    //{
                    //    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    //    List<Dominio.Entidades.Supervision> supervision = servicioCotizador.ListarExcelSupervision((string)Session["SegJefe"], (string)Session["SegSupervisor"], (string)Session["SegAgente"], fecInicio, fecTermino, Convert.ToInt32(TabSupervisionColumnaOrdenar.Value), Convert.ToChar(TabSupervisionDireccionOrdenar.Value));

                    //    control.DatosReporte = supervision;
                    //    control.ColumnaOrdenar = Convert.ToInt32(TabSupervisionColumnaOrdenar.Value);
                    //    control.DireccionOrdenar = Convert.ToChar(TabSupervisionDireccionOrdenar.Value);
                    //    control.PermisoConsultar = true;
                    //}
                    //else
                    //{
                    //    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                    //        Enums.OpcionesSistema.ReporteSupervision.StringValue()));
                    //    control.PermisoConsultar = false;
                    //}

                    //pagina.Controls.Add(control);

                    //string html = "";
                    //using (var sw = new StringWriter())
                    //{
                    //    HttpContext.Current.Server.Execute(pagina, sw, false);
                    //    html = sw.ToString();
                    //}

                    //Response.Clear();
                    //Response.ClearHeaders();
                    //Response.Buffer = true;
                    //string filename = "ReporteSeguimiento_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
                    //Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                    ////Response.AddHeader("Cache-Control", "cache, must-revalidate");
                    //Response.Charset = "";
                    //Response.ContentType = "application/vnd.ms-excel";

                    //string style = @"<style> .textmode { mso-number-format:\@; } </style>";
                    //Response.Write(style);
                    //Response.Output.Write(html);
                    //Response.Flush();
                    //Response.End();
                    ////HttpContext.Current.ApplicationInstance.CompleteRequest();

                    List<Dominio.Entidades.Supervision> supervision = new List<Dominio.Entidades.Supervision>();

                    DateTime fecInicio = Convert.ToDateTime(HFechaInicio.Value, new CultureInfo("es-PE"));
                    DateTime fecTermino = Convert.ToDateTime(HFechaTermino.Value, new CultureInfo("es-PE"));

                    var pagina = new Page();
                    var control = (TablaSupervisionExcel)pagina.LoadControl("~/Controles/TablaSupervisionExcel.ascx");

                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteSupervision))
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        supervision = servicioCotizador.ListarExcelSupervision((string)Session["SegJefe"], (string)Session["SegSupervisor"], (string)Session["SegAgente"], fecInicio, fecTermino, Convert.ToInt32(TabSupervisionColumnaOrdenar.Value), Convert.ToChar(TabSupervisionDireccionOrdenar.Value));
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ReporteSupervision.StringValue()));
                        control.PermisoConsultar = false;
                    }

                    string NombreArchivo = "ReporteSupervisión_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

                    Response.ContentType = "application/vnd.ms-excel";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", NombreArchivo));
                    Response.Clear();

                    ExportarNpoiXLS builder = new ExportarNpoiXLS(hssfworkbook, NombreArchivo, supervision);
                    builder.InitializeWorkbook();
                    builder.BuildSupervision();
                    builder.GetExcelStream().WriteTo(Response.OutputStream);
                    Response.End();

                    //<SRI.FIN-20322>
                }
                catch (ThreadAbortException) { }
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
    }
}