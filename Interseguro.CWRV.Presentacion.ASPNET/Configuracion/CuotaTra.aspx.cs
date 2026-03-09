using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using System.Net;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using System.Globalization;

namespace Interseguro.CWRV.Presentacion.ASPNET.Configuracion
{
    public partial class CuotaTra : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CuotaTra));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CuotaTRA))
                    {
                        if (!IsPostBack)
                        {
                            for (int i = 1; i <= 12; i++)
                            {
                                Mes.Items.Add(new ListItem { Value = i.ToString(), Text = CapitalizeFirstLetter(MonthName(i))});
                            }
                            Mes.SelectedValue = DateTime.Now.Month.ToString();
                            Periodo.Text = DateTime.Now.Year.ToString();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.CuotaTRA.StringValue()));
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

        public string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month);
        }

        public string CapitalizeFirstLetter(string value)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value);
        }


        [WebMethod]
        public static string CargarTablaCuotasTra(int periodo, int mes)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<CuotasTra> lstCuotas = servicioCotizador.ListarCuotasTra(periodo, mes);

                    List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                    List<Agente> listaJefe = listaAgentes.FindAll(e => e.IdNivel == 1);

                    List<CuotasTra> lstCuotasNuevo = new List<CuotasTra>();
                    lstCuotas = lstCuotas.OrderBy(e=> e.Agente.Id).ThenByDescending(e=>e.FecFinVigencia).ToList();
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
                            lstCuotasNuevo.Add(new CuotasTra{
                                Agente=item, NroCasosEfectivos=0, NroCasosSolicitados=0, NroCasosTotal=0,
                                FecInicioVigencia = fecInicioVigencia, FecFinVigencia=fecFinVigencia,
                                FecInicioVigenciaStr = fecInicioVigencia.ToString("dd/MM/yyyy"),
                                FecFinVigenciaStr = fecFinVigencia.ToString("dd/MM/yyyy")
                            });
                        }
                    }

                    //Añadiendo a toda la lista

                    lstCuotasNuevo.ForEach(p => lstCuotas.Add(p));

                    var pagina = new Page();

                    var control = (TablaCuotasTra)pagina.LoadControl("~/Controles/TablaCuotasTra.ascx");
                    control.lstCuotasTra = lstCuotas.OrderBy(e=> e.Agente.Nombre).ThenBy(e=> e.FecInicioVigencia).ToList();

                    pagina.Controls.Add(control);
                    log.Debug("Antes de");
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
        public static Respuesta RegistrarCuotas(string tokenUsuario, List<CuotasTra> lstCuotas)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
                {
                    string nombreTerminal = String.Empty;
                    string codUserName = (string)HttpContext.Current.Session["Usuario"];
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        List<String> errores = new List<String>();
                        List<String> controles = new List<String>();
                        if (!ValidarCuotas(lstCuotas, errores, controles))
                        {
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(errores);
                            respuesta.Controles = controles;
                            return respuesta;
                        }

                        for (int i = 0; i < lstCuotas.Count; i++)
                        {
                            lstCuotas[i].FecInicioVigencia = Convert.ToDateTime(lstCuotas[i].FecInicioVigenciaStr, new CultureInfo("es-PE"));
                            lstCuotas[i].FecFinVigencia = Convert.ToDateTime(lstCuotas[i].FecFinVigenciaStr, new CultureInfo("es-PE"));
                        }

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.RegistrarCuotas(lstCuotas, codUserName);

                        if (respuesta.Estado == Constante.COD_OK)
                        {
                            respuesta.Estado = Constante.COD_OK;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                            respuesta.Mensaje = "OK";
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

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
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }


        public static bool ValidarCuotas(List<CuotasTra> lstCuotas, List<String> errores, List<String> controles)
        {
            bool rpta = true;
            for (int i = 0; i < lstCuotas.Count; i++)
            {
                //Fecha Final
                if (lstCuotas[i].FecFinVigenciaStr == "")
                {
                    errores.Add("Cuota <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Fecha Final</strong>. Dato Obligatorio.");
                    rpta = false;
                    controles.Add(i + ",3");
                }

                if (lstCuotas[i].FecFinVigenciaStr != "")
                {
                    lstCuotas[i].FecInicioVigencia = Convert.ToDateTime(lstCuotas[i].FecInicioVigenciaStr, new CultureInfo("es-PE"));
                    lstCuotas[i].FecFinVigencia = Convert.ToDateTime(lstCuotas[i].FecFinVigenciaStr, new CultureInfo("es-PE"));

                    if (lstCuotas[i].FecFinVigencia < lstCuotas[i].FecInicioVigencia)
                    {
                        errores.Add("Cuota <strong>N° " + (i + 1) + "</strong>: La fecha final no puede ser menor que la final inicial.");
                        rpta = false;
                        controles.Add(i + ",3");
                    }   
                }

                //Nro. Casos Total 
                if (lstCuotas[i].NroCasosTotal< lstCuotas[i].NroCasosSolicitados)
                {
                    //errores.Add("Cuota <strong>N° " + (i + 1) + "</strong>: Ingrese el campo <strong>Moneda</strong>. Dato Obligatorio.");
                    errores.Add("Cuota <strong>N° " + (i + 1) + "</strong>: El Nro. total de cuotas no puede ser menor que Nro. total solicitados. Verifique.");
                    rpta = false;
                    controles.Add(i + ",4");   
                }

                if (lstCuotas[i].NroCasosTotal < lstCuotas[i].NroCasosEfectivos)
                {
                    errores.Add("Cuota <strong>N° " + (i + 1) + "</strong>: El Nro. total de cuotas no puede ser menor que Nro. total efectivos. Verifique.");
                    rpta = false;
                    controles.Add(i + ",4");
                }
            }
            return rpta;
        }
    }
}