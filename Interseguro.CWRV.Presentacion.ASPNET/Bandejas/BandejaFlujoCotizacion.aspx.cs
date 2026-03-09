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

namespace Interseguro.CWRV.Presentacion.ASPNET.Bandejas
{
    public partial class BandejaFlujoCotizacion : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(BandejaFlujoCotizacion));
        private static IServicioCWRV servicioCotizador;

        private static Solicitud sol;

        private static SolicitudEscenario solEscenario;

        protected void Page_Load(object sender, EventArgs e)
        {

            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    //if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.BandejaAprobacionOficiales))
                    // {
                    if (!IsPostBack)
                    {
                        log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                        CargarInformacionInicialPantalla();

                        //<INIGTI_4081>
                        //TablaSolicitudesOficialesError.Visible = false;
                        //TablaSolicitudesOficialesContenedor.Visible = false;
                        //TablaSolicitudesOficialesCargando.Visible = true;

                        //string token = (string)Session["TokenUsuario"];
                        //////string data = CargarTablaBandejaSolicitudesOficiales(token, "0", "0", "0");
                        //////TablaSolicitudesOficialesContenedor.Controls.Add(new LiteralControl(data));

                        //////LimpiarFormularios();
                        //TablaSolicitudesOficialesError.Visible = false;
                        //TablaSolicitudesOficialesCargando.Visible = false;
                        //TablaSolicitudesOficialesContenedor.Visible = true;
                        //<FINGTI_4081>
                    }
                    HEnviarEmail.Value = "0";
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.EnviarEmailPendiente))
                    {
                        HEnviarEmail.Value = "1";
                    }
                    // }
                    // else
                    // {
                    //     log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                    //         Enums.OpcionesSistema.BandejaAprobacionOficiales.StringValue()));
                    //     Response.Redirect("~/Error/Permisos.aspx");
                    // }
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
                    ////<INIGTI_4081>
                    ////TablaSolicitudesOficialesCargando.Visible = false;
                    ////TablaSolicitudesOficialesError.Visible = true;
                    ////<FINGTI_4081>
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
                    ////<INIGTI_4081>
                    ////TablaSolicitudesOficialesCargando.Visible = false;
                    ////TablaSolicitudesOficialesError.Visible = true;
                    ////<FINGTI_4081>
                }
            }
        }

        private void CargarInformacionInicialPantalla()
        {
            //<INIGTI_6556>
            if (Session["SolicitudesCheck"] != null)
            {
                string[] arrSolicitudes = (string[])Session["SolicitudesCheck"];
                if (arrSolicitudes.Count() > 0)
                {
                    HSeleccionados.Value = string.Join(",", arrSolicitudes);// arrSolicitudes.Join
                }
            }
            //<FINGTI_6556>
            List<Parametro> parametroCita = new List<Parametro>();

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            parametroCita = servicioCotizador.ObtenerParametrosPorTabla("");

            HttpContext.Current.Session["ParametroCita"] = parametroCita;

            DatosCuotas.Visible = false;

            //
            string codRol = (string)HttpContext.Current.Session["RolAzman"];
            if (codRol == Enums.RolAzman.JefeVentaLima.StringValue() || codRol == Enums.RolAzman.JefeVentaProvincia.StringValue())
            {
                DatosCuotas.Visible = true;
                int numAgente = Convert.ToInt32((Session["NumAgente"].ToString() == "") ? 0 : Session["NumAgente"]);
                List<CuotasTra> lstCuotas = new List<CuotasTra>();
                lstCuotas = servicioCotizador.ListarCuotasTra(DateTime.Today.Year, DateTime.Today.Month);
                if (lstCuotas.Count > 0)
                {
                    CuotasTra cuotasTra = lstCuotas.Find(x => x.Agente.Id == numAgente.ToString());
                    if (cuotasTra != null)
                    {
                        LabFecIni1.Text = cuotasTra.FecInicioVigencia.Value.ToString("dd/MM/yyyy");
                        LabFecFin1.Text = cuotasTra.FecFinVigencia.Value.ToString("dd/MM/yyyy");

                        LabTotal1.Text = cuotasTra.NroCasosTotal.ToString();
                        LabSolicitado1.Text = cuotasTra.NroCasosSolicitados.ToString();
                        LabEfectivo1.Text = cuotasTra.NroCasosEfectivos.ToString();
                        LabSaldo1.Text = Convert.ToString(cuotasTra.NroCasosTotal - cuotasTra.NroCasosSolicitados);
                    }
                    else
                    {
                        LabFecIni1.Text = "01/" + DateTime.Today.Month.ToString("00") + "/" + DateTime.Today.Year.ToString("0000");
                        LabFecFin1.Text = Convert.ToDateTime(LabFecIni1.Text).AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");

                        LabTotal1.Text = "0";
                        LabSolicitado1.Text = "0";
                        LabEfectivo1.Text = "0";
                        LabSaldo1.Text = "0";
                    }
                }


                DateTime fechaNext = DateTime.Today.AddMonths(1);
                lstCuotas = servicioCotizador.ListarCuotasTra(fechaNext.Year, fechaNext.Month);
                if (lstCuotas.Count > 0)
                {
                    CuotasTra cuotasTra = lstCuotas.Find(x => x.Agente.Id == numAgente.ToString());
                    if (cuotasTra != null)
                    {
                        LabFecIni2.Text = cuotasTra.FecInicioVigencia.Value.ToString("dd/MM/yyyy");
                        LabFecFin2.Text = cuotasTra.FecFinVigencia.Value.ToString("dd/MM/yyyy");

                        LabTotal2.Text = cuotasTra.NroCasosTotal.ToString();
                        LabSolicitado2.Text = cuotasTra.NroCasosSolicitados.ToString();
                        LabEfectivo2.Text = cuotasTra.NroCasosEfectivos.ToString();
                        LabSaldo2.Text = Convert.ToString(cuotasTra.NroCasosTotal - cuotasTra.NroCasosSolicitados);
                    }
                    else
                    {
                        LabFecIni2.Text = "01/" + fechaNext.Month.ToString("00") + "/" + fechaNext.Year.ToString("0000");
                        LabFecFin2.Text = Convert.ToDateTime(LabFecIni2.Text).AddMonths(1).AddDays(-1).ToString("dd/MM/yyyy");

                        LabTotal2.Text = "0";
                        LabSolicitado2.Text = "0";
                        LabEfectivo2.Text = "0";
                        LabSaldo2.Text = "0";
                    }
                }
            }


        }


        public List<CuotasTra> CuotasTra(int periodo, int mes, int numagente)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    //servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<CuotasTra> lstCuotas = servicioCotizador.ListarCuotasTra(periodo, mes);

                    List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                    List<Agente> listaJefe = listaAgentes.FindAll(e => e.IdNivel == 1 && e.Id == numagente.ToString());

                    List<CuotasTra> lstCuotasNuevo = new List<CuotasTra>();
                    lstCuotas = lstCuotas.OrderBy(e => e.Agente.Id).ThenByDescending(e => e.FecFinVigencia).ToList();
                    //Validando la lista de jefes con los que se encuentra registrado
                    foreach (var item in listaJefe)
                    {
                        CuotasTra cuotas = lstCuotas.Find(e => e.Agente.Id == item.Id);
                        if (cuotas != null)
                        {
                            DateTime fecInicioVigencia = Convert.ToDateTime("01/" + mes.ToString("00") + "/" + periodo.ToString(), new CultureInfo("es-PE"));
                            DateTime fecFinVigencia = fecInicioVigencia.AddMonths(1).AddDays(-1);
                            //Agregando otro Item, cuando el cierre es antes de fin de mes
                            if (cuotas.FecFinVigencia != fecFinVigencia)
                            {
                                lstCuotasNuevo.Add(new CuotasTra
                                {
                                    Agente = item,
                                    NroCasosEfectivos = 0,
                                    NroCasosSolicitados = 0,
                                    NroCasosTotal = 0,
                                    FecInicioVigencia = cuotas.FecFinVigencia.Value.AddDays(1),
                                    FecFinVigencia = fecFinVigencia,
                                    FecInicioVigenciaStr = cuotas.FecFinVigencia.Value.AddDays(1).ToString("dd/MM/yyyy"),
                                    FecFinVigenciaStr = fecFinVigencia.ToString("dd/MM/yyyy")
                                });
                            }
                        }
                        else
                        {
                            //Agregando cuando el item no existe
                            DateTime fecInicioVigencia = Convert.ToDateTime("01/" + mes.ToString("00") + "/" + periodo.ToString(), new CultureInfo("es-PE"));
                            DateTime fecFinVigencia = fecInicioVigencia.AddMonths(1).AddDays(-1);
                            lstCuotasNuevo.Add(new CuotasTra
                            {
                                Agente = item,
                                NroCasosEfectivos = 0,
                                NroCasosSolicitados = 0,
                                NroCasosTotal = 0,
                                FecInicioVigencia = fecInicioVigencia,
                                FecFinVigencia = fecFinVigencia,
                                FecInicioVigenciaStr = fecInicioVigencia.ToString("dd/MM/yyyy"),
                                FecFinVigenciaStr = fecFinVigencia.ToString("dd/MM/yyyy")
                            });
                        }
                    }

                    //Añadiendo a toda la lista

                    lstCuotasNuevo.ForEach(p => lstCuotas.Add(p));

                    //var pagina = new Page();

                    //var control = (TablaCuotasTra)pagina.LoadControl("~/Controles/TablaCuotasTra.ascx");
                    //control.lstCuotasTra = lstCuotas.OrderBy(e => e.Agente.Nombre).ThenBy(e => e.FecInicioVigencia).ToList();

                    //pagina.Controls.Add(control);
                    //log.Debug("Antes de");
                    //string html = "";
                    //using (var sw = new StringWriter())
                    //{
                    //    HttpContext.Current.Server.Execute(pagina, sw, false);
                    //    html = sw.ToString();
                    //}
                    //return html;

                    return lstCuotas.OrderBy(e => e.Agente.Nombre).ThenBy(e => e.FecInicioVigencia).ToList();
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }
        //<INIGTI_4081>

        public Control CargarTablaBandejaSolicitudes(string tokenUsuario, string numJefe, string numSupervisor, string numAgente)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();

                        //var control = (TablaSolicitudesBandejaOficiales)pagina.LoadControl("~/Controles/TablaSolicitudesBandejaOficiales.ascx");
                        var control = (TablaSolicitudesBandejaOficiales)LoadControl("~/Controles/TablaSolicitudesBandejaOficiales.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudConsultar))
                        {

                            string codUserName = (string)HttpContext.Current.Session["Usuario"];
                            string codRol = (string)HttpContext.Current.Session["RolAzman"];

                            //Listamos todas las solicitudes
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            //<INIGTI_4081>
                            //List<SolicitudEscenario> solicitudesEscenario = servicioCotizador.ListarSolicitudEscenario(numJefe, numSupervisor, numAgente, codUserName, codRol);
                            List<SolicitudEscenario> solicitudesEscenario = servicioCotizador.ListarSolicitudEscenarioCambios(numJefe, numSupervisor, numAgente, codUserName, codRol);
                            //<FINGTI_4081>

                            //listamos los estados a visualizar por rol
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<RolAzmanTipoMovimiento> listaRolAzmanTipoMovimiento = servicioCotizador.ObtenerTipoMovimientoPorRolAzman(codRol);

                            //Obtenemos lista de agentes dependientes
                            List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];

                            List<SolicitudEscenario> solicitudesEscenarioAgente = new List<SolicitudEscenario>();

                            //filtramos las solicitudes de acuerdo a lista de agentes dependientes
                            solicitudesEscenarioAgente = solicitudesEscenario.Join(listaAgentes,
                                                             solicitudes => solicitudes.Agente.Id,
                                                             agentes => agentes.Id,
                                                             (solicitudes, agentes) => solicitudes).ToList();


                            //solicitudesEscenarioAgente = solicitudesEscenarioAgente.Where(x => x.TipoMovimiento.Id != 0).ToList();

                            //Filtramos solicitudes que se deben visualizar de acuerdo al rol
                            solicitudesEscenarioAgente = solicitudesEscenarioAgente.Join(listaRolAzmanTipoMovimiento,
                                                                                         solicitudes => solicitudes.TipoMovimiento.Id,
                                                                                         rolAzmanTipoMovimiento => rolAzmanTipoMovimiento.CodTipoMovimiento,
                                                                                         (solicitudes, rolAzmanTipoMovimiento) => solicitudes).ToList();


                            control.SolicitudesEscenario = solicitudesEscenarioAgente;

                            control.PermisoConsultar = true;

                            control.PermisoModificar = true;

                            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                            control.RedLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);
                        }
                        else
                        {
                            control.PermisoConsultar = false;
                        }

                        //pagina.Controls.Add(control);

                        //string html = "";
                        //using (var sw = new StringWriter())
                        //{
                        //    HttpContext.Current.Server.Execute(pagina, sw, false);
                        //    html = sw.ToString();
                        //}
                        return control;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return null;
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
        //<FINGTI_4081>

        [WebMethod]
        public static string CargarTablaBandejaSolicitudesOficiales(List<SolicitudEscenario> solicitudesEscenarioAgente)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    /* if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    { */
                    var pagina = new Page();
                    var control = (TablaSolicitudesBandejaOficiales)pagina.LoadControl("~/Controles/TablaSolicitudesBandejaOficiales.ascx");

                    /* if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudConsultar))
                    {

                         string codUserName = (string)HttpContext.Current.Session["Usuario"];
                        string codRol = (string)HttpContext.Current.Session["RolAzman"];

                        //Listamos todas las solicitudes
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        //<INIGTI_4081>
                        //List<SolicitudEscenario> solicitudesEscenario = servicioCotizador.ListarSolicitudEscenario(numJefe, numSupervisor, numAgente, codUserName, codRol);
                        List<SolicitudEscenario> solicitudesEscenario = servicioCotizador.ListarSolicitudEscenarioCambios(numJefe, numSupervisor, numAgente, codUserName, codRol);
                        //<FINGTI_4081>

                        //listamos los estados a visualizar por rol
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<RolAzmanTipoMovimiento> listaRolAzmanTipoMovimiento = servicioCotizador.ObtenerTipoMovimientoPorRolAzman(codRol);

                        //Obtenemos lista de agentes dependientes
                        List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];

                        List<SolicitudEscenario> solicitudesEscenarioAgente = new List<SolicitudEscenario>();

                        //filtramos las solicitudes de acuerdo a lista de agentes dependientes
                        solicitudesEscenarioAgente = solicitudesEscenario.Join(listaAgentes,
                                                         solicitudes => solicitudes.Agente.Id,
                                                         agentes => agentes.Id,
                                                         (solicitudes, agentes) => solicitudes).ToList();


                        //solicitudesEscenarioAgente = solicitudesEscenarioAgente.Where(x => x.TipoMovimiento.Id != 0).ToList();

                        //Filtramos solicitudes que se deben visualizar de acuerdo al rol
                        solicitudesEscenarioAgente = solicitudesEscenarioAgente.Join(listaRolAzmanTipoMovimiento,
                                                                                     solicitudes => solicitudes.TipoMovimiento.Id,
                                                                                     rolAzmanTipoMovimiento => rolAzmanTipoMovimiento.CodTipoMovimiento,
                                                                                     (solicitudes, rolAzmanTipoMovimiento) => solicitudes).ToList();


                        control.SolicitudesEscenario = solicitudesEscenarioAgente;

                        control.PermisoConsultar = true;

                        control.PermisoModificar = true;

                        // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
                        control.RedLocal = Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);
                    }
                    else
                    {
                        control.PermisoConsultar = false;
                    } */

                    control.SolicitudesEscenario = solicitudesEscenarioAgente;

                    control.PermisoConsultar = true;

                    control.PermisoModificar = true;

                    pagina.Controls.Add(control);

                    string html = "";
                    using (var sw = new StringWriter())
                    {
                        HttpContext.Current.Server.Execute(pagina, sw, false);
                        html = sw.ToString();
                    }
                    return html;
                    /* }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return Constante.COD_TOKEN;
                    } */
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
        public static SolicitudEscenario ObtenerDatosSolicitudEscenario(string numSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                solEscenario = new SolicitudEscenario();
                string codUserName = (string)HttpContext.Current.Session["Usuario"];
                string codRol = (string)HttpContext.Current.Session["RolAzman"];

                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                solEscenario = servicioCotizador.ObtenerDatosSolicitudEscenario(numSolicitud, codUserName, codRol);
                return solEscenario;
            }
        }

        [WebMethod]
        public static Solicitud ObtenerDatosSolicitud(string numCuspp, string numSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                //Datos afiliado
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado("", numCuspp, "", "", "");

                //Datos Fecha solicitud
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                List<Solicitud> solicitudes = servicioCotizador.ListarSolicitud(numCuspp);
                Solicitud solFecha = new Solicitud();
                solFecha = solicitudes.Where(x => x.Id == numSolicitud).First();

                DateTime fechaCotizacion = Convert.ToDateTime(solFecha.FechaCotizacion, new CultureInfo("es-PE"));

                //datos Solicitud
                Solicitud sol = new Solicitud();
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                sol = servicioCotizador.ObtenerDatosSolicitud(numSolicitud, fechaCotizacion);

                HttpContext.Current.Session["Vendedor"] = afiliado.Agente.Id;
                HttpContext.Current.Session["Cartera"] = afiliado.Agente.IdCartera;
                HttpContext.Current.Session["SolicitudOficial"] = sol;

                return sol;
            }
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
        public static string CargarTablaBeneficiarios(string cuspp, List<GrupoFamiliar> beneficiarios)
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
        public static string CargarTablaCotizacionesOficiales(List<Cotizacion> cotizaciones, Boolean permisoTRA, Boolean permisoRadio)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();
                    var control = (TablaCotizacionesOficiales)pagina.LoadControl("~/Controles/TablaCotizacionesOficiales.ascx");

                    control.Cotizaciones = cotizaciones;
                    control.PermisoTRA = permisoTRA;
                    control.PermisoRadio = permisoRadio;

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
        public static string CargarTablaCotizacionesMovimiento(string numSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();
                    var control = (TablaCotizacionesMovimiento)pagina.LoadControl("~/Controles/TablaCotizacionesMovimiento.ascx");

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<CotizacionMovimiento> cotizacionMovimiento = servicioCotizador.ObtenerCotizacionTipoMovimientoPorSolicitud(numSolicitud);

                    control.cotizacionMovimiento = cotizacionMovimiento;

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
        public static Respuesta RegistrarCotizacionMovimiento(List<Cotizacion> listaCotizaciones, Boolean rechazo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
                {

                    string nombreTerminal = String.Empty;

                    Solicitud solicitud = (Solicitud)HttpContext.Current.Session["SolicitudOficial"];

                    string url =
                        HttpContext.Current.Request.Url.Scheme + "://" +
                        HttpContext.Current.Request.Url.Authority +
                        HttpContext.Current.Request.ApplicationPath +
                        (HttpContext.Current.Request.ApplicationPath == "/" ? String.Empty : "/") +
                        "Bandejas/BandejaFlujoCotizacion.aspx";

                    respuesta = Utilitario.RegistrarCotizacionMovimiento(ref listaCotizaciones, solicitud, rechazo, false, url, "0");

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

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    servicioCotizador.RegistrarLog(new LogBD
                    {
                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                        NombreTerminal = nombreTerminal,
                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                        NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                        IdTipoEvento = Enums.EventoLog.FlujoAprobacion.StringValue(),
                        //Detalle = String.Format("Movimientos TRA de la Solicitud", solicitud.Id)
                        Detalle = "Flujo de aprobación - Solicitud: " + solicitud.Id + ", ACOM: " + solicitud.PorcentajeAumentoComision + ", DCOM: " + solicitud.PorcentajeDescuentoComision + ", DTRA: " + tra
                    });

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

        //<INIGTI_6556>
        [WebMethod]
        public static Respuesta EnviarEmailPendiente(string tokenUsuario)
        {
            Respuesta respuesta = new Respuesta();
            Respuesta rpta = new Respuesta();
            try
            {
                using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        log.Info(String.Format("Inicio de Envios de Email Pendientes.<--------"));

                        List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];

                        string codRol = (string)HttpContext.Current.Session["RolAzman"];
                        List<SolicitudEscenario> lstSolicitudEscenario = new List<SolicitudEscenario>();
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        lstSolicitudEscenario = servicioCotizador.ListarSolicitudesPendientesEmail(codRol);

                        string url =
                           HttpContext.Current.Request.Url.Scheme + "://" +
                           HttpContext.Current.Request.Url.Authority +
                           HttpContext.Current.Request.ApplicationPath +
                           (HttpContext.Current.Request.ApplicationPath == "/" ? String.Empty : "/") +
                           "Bandejas/BandejaFlujoCotizacion.aspx";

                        string idNroSolicitudes = "";
                        string contenido = "";

                        foreach (var solicitud in lstSolicitudEscenario)
                        {
                            Agente agente = listaAgentes.Find(a => a.Id == solicitud.Agente.Id);
                            if (agente != null)
                            {
                                rpta.Contenido = solicitud.TipoMovimiento.Id + solicitud.TipoMovimiento.Nombre;
                                if (solicitud.TipoMovimiento.Nombre == "ENVIADO")
                                {
                                    idNroSolicitudes += solicitud.NumSolicitud + ",";
                                    contenido = rpta.Contenido;
                                }
                                else
                                {
                                    //<INIGTI_6556>
                                    log.Info(String.Format("Inicio obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", solicitud.NumSolicitud));
                                    //<FINGTI_6556>
                                    List<SolicitudEscenario> lstSolicitudes;
                                    lstSolicitudes = servicioCotizador.ListarSolicitudesEmail(solicitud.NumSolicitud);

                                    //<INIGTI_6556>
                                    log.Info(String.Format("Fin obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", solicitud.NumSolicitud));
                                    //<FINGTI_6556>

                                    respuesta = Utilitario.EnviarEmail(lstSolicitudes, url, rpta);
                                }
                            }
                        }


                        if (idNroSolicitudes != "")
                        {
                            //<INIGTI_6556>
                            idNroSolicitudes = idNroSolicitudes.PadLeft(idNroSolicitudes.Length - 1);
                            log.Info(String.Format("Inicio obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", idNroSolicitudes));
                            //<FINGTI_6556>
                            List<SolicitudEscenario> lstSolicitudes;
                            lstSolicitudes = servicioCotizador.ListarSolicitudesEmail(idNroSolicitudes);
                            //<INIGTI_6556>
                            log.Info(String.Format("Fin obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", idNroSolicitudes));
                            //<FINGTI_6556>

                            rpta.Contenido = contenido;
                            respuesta = Utilitario.EnviarEmail(lstSolicitudes, url, rpta);
                        }
                        if (lstSolicitudEscenario.Count == 0)
                        {
                            respuesta.Estado = Constante.COD_OK;
                            respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                            respuesta.Mensaje = "No existe registros para enviar email.";
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                }

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            log.Info(String.Format("Fin de Envios de Email Pendientes.--------/>"));
            return respuesta;

        }

        [WebMethod]
        public static string GuardandoCheck(string[] solicitudes)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    HttpContext.Current.Session["SolicitudesCheck"] = solicitudes;
                    return "OK";
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

        //<FINGTI_6556>
    }
}