using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;

namespace Interseguro.CWRV.Presentacion.ASPNET.Configuracion
{
    public partial class CargaArchivosIndicadores : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CargaArchivosIndicadores));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Validar permisos
                if (!Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CargaArchivosIndicadores))
                {
                    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                        Enums.OpcionesSistema.CargaArchivosIndicadores.StringValue()));
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
                MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
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
                MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ex.Message });
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                MCMEstado.Value = "1";
            }
        }

        protected void btnControlCDA_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (!string.IsNullOrEmpty(txtPeriodo.Value))
                    {
                        if (fuControlCDA.HasFile)
                        {
                            ExcelPackage package = new ExcelPackage(fuControlCDA.PostedFile.InputStream);
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                            int rows = worksheet.Dimension.Rows;

                            if (rows > 1)
                            {
                                var listaCargaControlCdA = new List<CargaControlCdA>();

                                for (int i = 2; i <= rows; i++)
                                {
                                    var cargaControlCdA = new CargaControlCdA
                                    {
                                        fec_periodo = DateTime.Parse(txtPeriodo.Value),
                                        num_cuspp = worksheet.Cells[i, 1].Value.ToString(),
                                        gls_validacion = worksheet.Cells[i, 2].Value == null ? null : worksheet.Cells[i, 2].Value.ToString(),
                                        gls_justificacion = worksheet.Cells[i, 3].Value == null ? null : worksheet.Cells[i, 3].Value.ToString(),
                                        aud_usr_ingreso = Session["Usuario"].ToString()
                                    };

                                    listaCargaControlCdA.Add(cargaControlCdA);
                                }

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                log.Debug("Inicio comunicación método: ServicioCWRV.CargarReporteControlCdA");
                                Respuesta respuesta = servicioCotizador.CargarReporteControlCdA(listaCargaControlCdA);
                                log.Debug("Fin comunicación método: ServicioCWRV.CargarReporteControlCdA");

                                MCMMensaje.Text = respuesta.Mensaje;
                                MCMEstadoIcono.Value = respuesta.Icono;
                                MCMEstadoTitulo.Value = respuesta.Titulo;
                                MCMEstado.Value = "1";
                            }
                        }
                        else
                        {
                            log.Error("No se ha seleccionado ningún archivo para la carga de datos.");
                            MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { "No se ha seleccionado ningún archivo" });
                            MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                            MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                            MCMEstado.Value = "1";
                        }
                    }
                    else
                    {
                        log.Error("No se ha indicado el periodo para la carga de datos.");
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { "No se ha indicado el periodo para la carga de datos." });
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

        protected void btnLocalidadVCTP_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (!string.IsNullOrEmpty(txtPeriodo.Value))
                    {
                        if (fuLocalidadVCTP.HasFile)
                        {
                            ExcelPackage package = new ExcelPackage(fuLocalidadVCTP.PostedFile.InputStream);
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                            int rows = worksheet.Dimension.Rows;

                            if (rows > 1)
                            {
                                var listaCargaLocalidadVCTP = new List<CargaLocalidadVCTP>();

                                for (int i = 2; i <= rows; i++)
                                {
                                    var cargaLocalidadVCTP = new CargaLocalidadVCTP()
                                    {
                                        fec_periodo = DateTime.Parse(txtPeriodo.Value),
                                        cod_supervisor = Convert.ToInt32(worksheet.Cells[i, 1].Value),
                                        nom_supervisor = worksheet.Cells[i, 2].Value.ToString(),
                                        cod_jefe = Convert.ToInt32(worksheet.Cells[i, 3].Value),
                                        nom_jefe = worksheet.Cells[i, 4].Value.ToString(),
                                        gls_localidad = worksheet.Cells[i, 5].Value == null ? null : worksheet.Cells[i, 5].Value.ToString(),
                                        aud_usr_ingreso = Session["Usuario"].ToString()
                                    };

                                    listaCargaLocalidadVCTP.Add(cargaLocalidadVCTP);
                                }

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                log.Debug("Inicio comunicación método: ServicioCWRV.CargarReporteLocalidadVCTP");
                                Respuesta respuesta = servicioCotizador.CargarReporteLocalidadVCTP(listaCargaLocalidadVCTP);
                                log.Debug("Fin comunicación método: ServicioCWRV.CargarReporteLocalidadVCTP");

                                MCMMensaje.Text = respuesta.Mensaje;
                                MCMEstadoIcono.Value = respuesta.Icono;
                                MCMEstadoTitulo.Value = respuesta.Titulo;
                                MCMEstado.Value = "1";
                            }
                        }
                        else
                        {
                            log.Error("No se ha seleccionado ningún archivo para la carga de datos.");
                            MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { "No se ha seleccionado ningún archivo" });
                            MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                            MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                            MCMEstado.Value = "1";
                        }
                    }
                    else
                    {
                        log.Error("No se ha indicado el periodo para la carga de datos.");
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { "No se ha indicado el periodo para la carga de datos." });
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

        protected void btnControlVCTP_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (!string.IsNullOrEmpty(txtPeriodo.Value))
                    {
                        if (fuControlVCTP.HasFile)
                        {
                            ExcelPackage package = new ExcelPackage(fuControlVCTP.PostedFile.InputStream);
                            ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();

                            int rows = worksheet.Dimension.Rows;

                            if (rows > 1)
                            {
                                var listaCargaControlVCTP = new List<CargaControlVCTP>();

                                for (int i = 2; i <= rows; i++)
                                {
                                    var cargaControlVCTP = new CargaControlVCTP()
                                    {
                                        fec_periodo = DateTime.Parse(txtPeriodo.Value),
                                        num_cuspp = worksheet.Cells[i, 1].Value.ToString(),
                                        ind_medicion = worksheet.Cells[i, 2].Value.ToString(),
                                        gls_comentario = worksheet.Cells[i, 3].Value == null ? null : worksheet.Cells[i, 3].Value.ToString(),
                                        aud_usr_ingreso = Session["Usuario"].ToString()
                                    };

                                    listaCargaControlVCTP.Add(cargaControlVCTP);
                                }

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                log.Debug("Inicio comunicación método: ServicioCWRV.CargarReporteControlVCTP");
                                Respuesta respuesta = servicioCotizador.CargarReporteControlVCTP(listaCargaControlVCTP);
                                log.Debug("Fin comunicación método: ServicioCWRV.CargarReporteControlVCTP");

                                MCMMensaje.Text = respuesta.Mensaje;
                                MCMEstadoIcono.Value = respuesta.Icono;
                                MCMEstadoTitulo.Value = respuesta.Titulo;
                                MCMEstado.Value = "1";
                            }
                        }
                        else
                        {
                            log.Error("No se ha seleccionado ningún archivo para la carga de datos.");
                            MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { "No se ha seleccionado ningún archivo" });
                            MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                            MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                            MCMEstado.Value = "1";
                        }
                    }
                    else
                    {
                        log.Error("No se ha indicado un periodo para la carga de datos.");
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { "No se ha indicado el periodo para la carga de datos." });
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

        [WebMethod]
        public static Respuesta ValidaPeriodoControlCDA(string tokenUsuario, string periodo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var dPeriodo = DateTime.Parse(periodo);
                        var usuario = HttpContext.Current.Session["Usuario"].ToString();

                        /*CODIGO PARA COMUNICACION CON WS*/
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        log.Debug("Inicio comunicación método: ServicioCWRV.ValidarCargaControlCdA");
                        var existe = servicioCotizador.ValidarCargaControlCdA(dPeriodo, usuario);
                        log.Debug("Fin comunicación método: ServicioCWRV.ValidarCargaControlCdA");

                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Contenido = existe.ToString();
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"Se ha producido el siguiente error: [{ex.Message}]", ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta ValidaPeriodoLocalidadVCTP(string tokenUsuario, string periodo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var dPeriodo = DateTime.Parse(periodo);
                        var usuario = HttpContext.Current.Session["Usuario"].ToString();

                        /*CODIGO PARA COMUNICACION CON WS*/
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        log.Debug("Inicio comunicación método: ServicioCWRV.ValidarCargaLocalidadVCTP");
                        var existe = servicioCotizador.ValidarCargaLocalidadVCTP(dPeriodo, usuario);
                        log.Debug("Fin comunicación método: ServicioCWRV.ValidarCargaLocalidadVCTP");

                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Contenido = existe.ToString();
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"Se ha producido el siguiente error: [{ex.Message}]", ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta ValidaPeriodoControlVCTP(string tokenUsuario, string periodo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var dPeriodo = DateTime.Parse(periodo);
                        var usuario = HttpContext.Current.Session["Usuario"].ToString();

                        /*CODIGO PARA COMUNICACION CON WS*/
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        log.Debug("Inicio comunicación método: ServicioCWRV.ValidarCargaControlVCTP");
                        var existe = servicioCotizador.ValidarCargaControlVCTP(dPeriodo, usuario);
                        log.Debug("Inicio comunicación método: ServicioCWRV.ValidarCargaControlVCTP");

                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Contenido = existe.ToString();
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"Se ha producido el siguiente error: [{ex.Message}]", ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }
    }
}