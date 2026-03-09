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
    public partial class BandejaFlujoCotizacionModificar : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(BandejaFlujoCotizacion));
        private static IServicioCWRV servicioCotizador;

        //private static Solicitud sol;

        private static SolicitudEscenario solEscenario;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                /* try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.BandejaAprobacionOficiales))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.BandejaAprobacionOficiales.StringValue()));
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
                } */
            }
        }

        /* private void CargarInformacionInicialPantalla()
        {
            //<INIGTI_4081>
            if (Session["SolicitudEscenarioMovil"] == null)
            {
                Response.Redirect("BandejaFlujoCotizacion.aspx");
                return;
            }

            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.RechazarSolicitud))
            {
                PerRechazarSolicitud.Value = "1";
            }
            else
            {
                PerRechazarSolicitud.Value = "0";
            }

            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoDCOMBandeja))
            {
                ModSolDCOM.ReadOnly = false;
                ModSolDCOM.CssClass = "formTextbox numerico";
            }
            else
            {
                ModSolDCOM.ReadOnly = true;
                ModSolDCOM.CssClass = "formTextbox formTextboxReadOnly";
            }
            //<FINGTI_4081>

            List<Parametro> parametroCita = new List<Parametro>();

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            parametroCita = servicioCotizador.ObtenerParametrosPorTabla("");

            HttpContext.Current.Session["ParametroCita"] = parametroCita;


            //<INIGTI_4081>
            string codRol = (string)HttpContext.Current.Session["RolAzman"];

            HGrupoEspeciales.Value = "FALSE";
            GrupoEspeciales.Visible = false;
            Leyenda.Visible = false;
            if (
                codRol == Enums.RolAzman.JefeOperaciones.StringValue() ||
                codRol == Enums.RolAzman.AsistenteOperaciones.StringValue() ||
                codRol == Enums.RolAzman.AnalistaOperaciones.StringValue()
            )
            {
                GrupoEspeciales.Visible = true;
                HGrupoEspeciales.Value = "TRUE";
                Leyenda.Visible = true;
            }

            GrupoInformeA.Visible = false;
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.InformeA))
            {
                GrupoInformeA.Visible = true;
                //<GTIINI-6623>
                //SolicitudEscenario solEscenario = new SolicitudEscenario();
                //solEscenario = (SolicitudEscenario)Session["SolicitudEscenarioMovil"];
                //string codUserName = (string)HttpContext.Current.Session["Usuario"];
                //solEscenario = servicioCotizador.ObtenerDatosSolicitudEscenario(solEscenario.NumSolicitud, codUserName, codRol);

                SolicitudEscenario solEscenario = new SolicitudEscenario();
                solEscenario = (SolicitudEscenario)Session["SolicitudEscenarioMovil"];

                string codUserName = (string)HttpContext.Current.Session["Usuario"];

                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                solEscenario = servicioCotizador.ObtenerDatosSolicitudEscenario(solEscenario.NumSolicitud, codUserName, codRol);

                List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];

                Parametro parametroTRA = listaParametro.Where(x => x.Id == "TRA").Distinct().First();

                List<Parametro> listaParametroEstadosCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.EstadoCita.StringValue()).ToList();
                Cita cita = new Cita
                {
                    Agente = new Agente { Id = solEscenario.Agente.Id },
                    Afiliado = new Afiliado { CUSPP = solEscenario.Afiliado.CUSPP },
                    Parametro = listaParametroEstadosCita
                };
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                List<Cita> listaCita = servicioCotizador.ListarCita(cita);
                String informeA = String.Empty;
                if (listaCita.Count > 0)
                {
                    Cita citaTemp = listaCita.First();
                    Pronostico.Text = citaTemp.Pronostico.Nombre;
                    Avance.Text = String.Format("{0:0.00}%", citaTemp.AvanceCartera);
                    FechaUltimaVisita.Text = citaTemp.UltimaCitaEfectiva.ToString("dd/MM/yyyy");
                }
                else
                {
                    Pronostico.Text = "-";
                    Avance.Text = "-";
                    FechaUltimaVisita.Text = "-";
                }

                // Obtenemos datos desde el SDA
                SDAReporte reporte = servicioCotizador.obtenerPreCubo(Convert.ToInt32(solEscenario.Agente.Id), solEscenario.Afiliado.CUSPP);
                Condicion.Text = reporte.Precubo;
                Avance.Text = String.Format("{0:0.00}%", reporte.AvanceCartera);

                // Obtenemos datos del %MS
                Agente agente = servicioCotizador.ObtenerMSAgente(solEscenario.Agente.Id);

                MSAgente.Text = String.Format("{0:0.00}%", agente.MS1);
                MSAgentge6.Text = String.Format("{0:0.00}%", agente.MS6);

                VisitasCompletadas.Text = listaCita.Count.ToString();
                //<GTIFIN-6623>
            }
            //<FINGTI_4081>
        }
 */
        [WebMethod]
        public static SolicitudEscenario EnvioPostMovil(string numCusspp, string numSolicitud, string numOperacion)
        {
            SolicitudEscenario solEscenario = new SolicitudEscenario();
            solEscenario.NumOperacion = Convert.ToInt64(numOperacion);
            solEscenario.NumSolicitud = numSolicitud;
            solEscenario.Afiliado = new Afiliado { CUSPP = numCusspp };

            HttpContext.Current.Session["SolicitudEscenarioMovil"] = solEscenario;

            return solEscenario;
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarTablaCotizacionesOficiales(List<Cotizacion> cotizaciones, long numCotizacionElegida, Boolean permisoTRA, Boolean permisoRadio)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();
                    var control = (TablaCotizacionesOficiales)pagina.LoadControl("~/Controles/TablaCotizacionesOficiales.ascx");

                    control.MostrarPorcentajeCapital = false;
                    control.MostrarTasas = true;

                    control.Cotizaciones = cotizaciones;
                    control.PermisoTRA = permisoTRA;
                    control.PermisoRadio = permisoRadio;
                    control.CotizacionElegida = numCotizacionElegida;
                    control.RedLocal = true; //Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress);
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
        public static Respuesta RegistrarCotizacionMovimiento(double acom, string montoAcom, List<Cotizacion> listaCotizaciones, Boolean rechazo, bool correo, double dcom)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
                {
                    //<INIGTI_4081>
                    string usuario = (string)HttpContext.Current.Session["Usuario"];
                    string codRol = (string)HttpContext.Current.Session["RolAzman"];
                    Solicitud solicitud = (Solicitud)HttpContext.Current.Session["SolicitudOficial"];
                    //Validando las solicitudes cargadas
                    if (HttpContext.Current.Session["SolicitudEscenarioMovil"] == null)
                    {
                        throw new Exception("Usted a cargado dos ó más solicitudes diferentes, cancele la operación y vuelva a cargar!");
                    }

                    SolicitudEscenario solicitudEscenario = (SolicitudEscenario)HttpContext.Current.Session["SolicitudEscenarioMovil"];
                    if (solicitud.Id != solicitudEscenario.NumSolicitud)
                    {
                        throw new Exception("Usted a cargado dos ó más solicitudes diferentes, cancele la operación y vuelva a cargar!");
                    }

                    //<INIGTI_6556>
                    if (correo)
                        log.Info(String.Format("Inicio Principal Individual de envío al flujo, solicitud Nro. [{0}].", solicitud.Id));
                    else
                        log.Info(String.Format("Inicio por Item de envío al flujo, solicitud Nro. [{0}].", solicitud.Id));
                    //<FINGTI_6556>

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    if (rechazo)
                    {
                        if (!Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.RechazarSolicitud))
                        {
                            throw new Exception("Usted no cuenta con permiso para rechazar solicitudes");
                        }
                    }
                    //<FINGTI_4081>
                    string nombreTerminal = String.Empty;



                    string url =
                        HttpContext.Current.Request.Url.Scheme + "://" +
                        HttpContext.Current.Request.Url.Authority +
                        HttpContext.Current.Request.ApplicationPath +
                        (HttpContext.Current.Request.ApplicationPath == "/" ? String.Empty : "/") +
                        "Bandejas/BandejaFlujoCotizacion.aspx";

                    //<GTIINI-1092>
                    for (int i = 0; i < solicitud.Cotizaciones.Count; i++)
                    {
                        solicitud.Cotizaciones[i].AjusteTRA = listaCotizaciones[i].AjusteTRA;
                    }
                    //<GTIFIN-1092>

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


                    solicitud.PorcentajeDescuentoComision = dcom;

                    solicitud.PorcentajeAumentoComision = acom;
                    solicitud.MontoAumentoComision = Convert.ToDouble(montoAcom, new CultureInfo("es-PE"));

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
                        Detalle = "Flujo de aprobación - Solicitud: " + solicitud.Id + ", ACOM: " + acom + ", DCOM: " + dcom + ", DTRA: " + tra
                    });

                    respuesta = Utilitario.RegistrarCotizacionMovimiento(ref listaCotizaciones, solicitud, rechazo, false, url, "0");

                    //<INIGTI_4081>
                    if (respuesta.Estado == Constante.COD_OK)
                    {
                        //<INIGTI_4081_3>
                        //////string accion = respuesta.Contenido.Substring(1, respuesta.Contenido.Length - 1);

                        //////if (accion == "APROBADO")
                        //////{
                        //////    int tipoMovimientoDestino = Convert.ToInt32(respuesta.Contenido.Substring(0, 1));
                        //////    //tipoMovimientoDestino(6) Aprobado
                        //////    if (tipoMovimientoDestino == 6)
                        //////    {
                        //////        SolicitudEscenario escenario = ObtenerDatosSolicitudEscenario(solicitud.Id);

                        //////        escenario.IndEstadoSeleccion = escenario.IndEstadoSeleccion;//estadoSeleccion;
                        //////        escenario.ValMtoAgenteAcom = Convert.ToDouble(montoAcom, new CultureInfo("es-PE"));
                        //////        escenario.NumCotizacionElegida = Convert.ToInt64(escenario.NumCotizacionElegida);//elegida
                        //////        escenario.Cotizaciones = listaCotizaciones;
                        //////        escenario.Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] };

                        //////        //servicioCotizador = LocalizadorProxy.ObtenerServicio();//<INIGTI_4081>
                        //////        Respuesta respuestaEscenario = new Respuesta();
                        //////        respuestaEscenario = servicioCotizador.ActualizarSolicitudEscenario(ref escenario, false);
                        //////    }
                        //////}

                        if (correo)
                        {
                            //<INIGTI_6556>
                            log.Info(String.Format("Inicio obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", solicitud.Id));
                            //<FINGTI_6556>
                            List<SolicitudEscenario> lstSolicitudEscenario;
                            lstSolicitudEscenario = servicioCotizador.ListarSolicitudesEmail(solicitud.Id);
                            //<INIGTI_6556>
                            log.Info(String.Format("Fin obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", solicitud.Id));
                            //<FINGTI_6556>

                            Utilitario.EnviarEmail(lstSolicitudEscenario, url, respuesta);
                        }
                        //<FINGTI_4081_3>

                    }

                    //<FINGTI_4081>

                    //<INIGTI_6556>
                    if (correo)
                        log.Info(String.Format("Fin Principal Individual de envío al flujo, solicitud Nro. [{0}].", solicitud.Id));
                    else
                        log.Info(String.Format("Fin por Item de envío al flujo, solicitud Nro. [{0}].", solicitud.Id));
                    //<FINGTI_6556>

                    //<INIGTI_4081>
                    if (respuesta.Estado == Constante.COD_OK)
                    {
                        HttpContext.Current.Session["SolicitudEscenarioMovil"] = null;
                    }
                    //<FINGTI_4081>

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


        [WebMethod]
        public static Respuesta RegistrarCotizacionMovimientoBloque(string[] solicitudes, bool rechazo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
                {
                    string nombreTerminal = String.Empty;
                    string contenido = "";

                    if (solicitudes.Count() == 0)
                    {
                        respuesta.Mensaje = "Seleccione al menos una solicitud";
                        respuesta.Estado = Constante.COD_ERROR;
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                        respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                        return respuesta;
                    }
                    string idNroSolicitudes = "";
                    string url =
                        HttpContext.Current.Request.Url.Scheme + "://" +
                        HttpContext.Current.Request.Url.Authority +
                        HttpContext.Current.Request.ApplicationPath +
                        (HttpContext.Current.Request.ApplicationPath == "/" ? String.Empty : "/") +
                        "Bandejas/BandejaFlujoCotizacion.aspx";


                    log.Info(String.Format("Inicio Principal Bloque de envío al flujo, solicitudes Nro. [{0}]. <---------------------------------------", string.Join(",", solicitudes)));


                    for (int i = 0; i < solicitudes.Length; i++)
                    {
                        string numSolicitud = solicitudes[i];
                        solEscenario = new SolicitudEscenario();
                        string codUserName = (string)HttpContext.Current.Session["Usuario"];
                        string codRol = (string)HttpContext.Current.Session["RolAzman"];

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        solEscenario = servicioCotizador.ObtenerDatosSolicitudEscenario(numSolicitud, codUserName, codRol);
                        //<INIGTI_6556>//SE COMENTA
                        ////List<Solicitud> lstsolicitudes = servicioCotizador.ListarSolicitud(solEscenario.Afiliado.CUSPP);
                        ////Solicitud solFecha = new Solicitud();
                        ////solFecha = lstsolicitudes.Where(x => x.Id == numSolicitud).First();

                        ////DateTime fechaCotizacion = Convert.ToDateTime(solFecha.FechaCotizacion, new CultureInfo("es-PE"));

                        //<FINGTI_6556>
                        //DateTime fechaCotizacion = Convert.ToDateTime(solEscenario.FechaCotizacion, new CultureInfo("es-PE"));

                        //datos Solicitud
                        Solicitud sol = new Solicitud();
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        sol = servicioCotizador.ObtenerDatosSolicitud(numSolicitud, solEscenario.FechaPresentacion.Value);//<INIGTI_6556> fechaCotizacion
                        HttpContext.Current.Session["SolicitudOficial"] = sol;
                        //Para que pueda validar en RegistrarCotizacionMovimiento
                        HttpContext.Current.Session["SolicitudEscenarioMovil"] = solEscenario;

                        respuesta = RegistrarCotizacionMovimiento(solEscenario.PjeAumentoComision, solEscenario.ValMtoAgenteAcom.ToString(), sol.Cotizaciones, rechazo, false, solEscenario.CodPjeCesionComision);

                        if (respuesta.Estado == Constante.COD_OK)
                        {
                            string accion = respuesta.Contenido.Substring(1, respuesta.Contenido.Length - 1);
                            if (accion == "ENVIADO")
                            {
                                idNroSolicitudes += numSolicitud + ",";
                                contenido = respuesta.Contenido;
                            }
                            else
                            {
                                //<INIGTI_6556>
                                log.Info(String.Format("Inicio obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", numSolicitud));
                                //<FINGTI_6556>
                                List<SolicitudEscenario> lstSolicitudEscenario;
                                lstSolicitudEscenario = servicioCotizador.ListarSolicitudesEmail(numSolicitud);

                                //<INIGTI_6556>
                                log.Info(String.Format("Fin obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", numSolicitud));
                                //<FINGTI_6556>

                                Utilitario.EnviarEmail(lstSolicitudEscenario, url, respuesta);

                            }
                        }
                    }

                    log.Info(String.Format("Fin Principal Bloque de envío al flujo. ---------------------------------------/>"));

                    if (idNroSolicitudes != "")
                    {
                        //<INIGTI_6556>
                        idNroSolicitudes = idNroSolicitudes.PadLeft(idNroSolicitudes.Length - 1);
                        log.Info(String.Format("Inicio obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", idNroSolicitudes));
                        //<FINGTI_6556>
                        List<SolicitudEscenario> lstSolicitudEscenario;
                        lstSolicitudEscenario = servicioCotizador.ListarSolicitudesEmail(idNroSolicitudes);
                        //<INIGTI_6556>
                        log.Info(String.Format("Fin obteniendo ListarSolicitudesEmail, solicitud Nro. [{0}].", idNroSolicitudes));
                        //<FINGTI_6556>

                        respuesta.Contenido = contenido;
                        Utilitario.EnviarEmail(lstSolicitudEscenario, url, respuesta);

                    }

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
                    //    IdTipoEvento = Enums.EventoLog.ReporteEscenarios.StringValue(),
                    //    //Detalle = String.Format("Movimientos TRA de la Solicitud", solicitud.Id)
                    //});

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


        //<INIGTI_4081>
        [WebMethod]
        public static string CargarTablaTraDefault(string idSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<ParametroEspecial> parametros = servicioCotizador.ObtenerTraDefault(idSolicitud);


                    var pagina = new Page();

                    var control = (TablaTraDefault)pagina.LoadControl("~/Controles/TablaTraDefault.ascx");
                    control.parametroEspecial = parametros;
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
        public static string CargarTablaTasaMaximaTraMinima(string idSolicitud, string fecCotizacion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                    //fecCotizacion = fechaCotizacion.ToString("yyyyMMdd");

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<ParametroEspecial> parametros = servicioCotizador.ObtenerTasaMaximaTraMinima(idSolicitud, fechaCotizacion);


                    var pagina = new Page();

                    var control = (TablaTasaMaximaTraMinimo)pagina.LoadControl("~/Controles/TablaTasaMaximaTraMinimo.ascx");
                    control.parametroEspecial = parametros;
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
        public static Respuesta RegistrarCotizaValPar(string tokenUsuario, List<ParametroEspecial> parametros, bool modifica)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
                {

                    string nombreTerminal = String.Empty;

                    string codUserName = (string)HttpContext.Current.Session["Usuario"];

                    if (modifica)
                    {
                        for (int i = 0; i < parametros.Count; i++)
                        {
                            parametros[i].FecIniRango = Convert.ToDateTime(parametros[i].FecIniRangoStr, new CultureInfo("es-PE"));
                            parametros[i].FecFinRango = Convert.ToDateTime(parametros[i].FecFinRangoStr, new CultureInfo("es-PE"));

                        }

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.RegistrarCotizaValPar(parametros, codUserName);
                    }
                    else
                    {
                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                        respuesta.Mensaje = "OK";
                    }


                    //respuesta.Mensaje = "todo Ok";


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
                        //Detalle = String.Format("Movimientos TRA de la Solicitud", solicitud.Id)
                    });

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


        //<FINGTI_4081>
    }
}