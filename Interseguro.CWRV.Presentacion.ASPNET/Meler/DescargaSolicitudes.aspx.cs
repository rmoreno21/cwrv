using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.Web;
using System.Web.Services;
using System.Web.UI;

namespace Interseguro.CWRV.Presentacion.ASPNET.Meler
{
    public partial class DescargaSolicitudes : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DescargaSolicitudes));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DescargaSolicitudes))
                {
                    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                        Enums.OpcionesSistema.DescargaSolicitudes.StringValue()));
                    Response.Redirect("~/Error/Permisos.aspx");
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
                MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                MCMEstado.Value = "1";
            }
        }

        protected void GuardarArchivo_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Archivo.HasFile)
                    {
                        string contenido;
                        using (StreamReader inputStreamReader = new StreamReader(Archivo.PostedFile.InputStream, System.Text.Encoding.Default, true))
                        {
                            contenido = inputStreamReader.ReadToEnd();
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            int lote = 0;
                            Respuesta respuesta = servicioCotizador.RegistrarDescargaSolicitudes(contenido, Session["usuario"].ToString(), ref lote);
                            NumeroLote.Text = lote.ToString();
                            FechaDesde.Text = string.Empty;
                            FechaHasta.Text = string.Empty;

                            MCMMensaje.Text = respuesta.Mensaje;
                            MCMEstadoIcono.Value = respuesta.Icono;
                            MCMEstadoTitulo.Value = respuesta.Titulo;
                            MCMEstado.Value = "1";
                        }
                    }
                    else
                    {
                        log.Error("No se ha seleccionado ningún archivo para ser guardado.");
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { "No se ha seleccionado ningún archivo" });
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
        public static Respuesta CargarTablaLotes(string tokenUsuario, string numeroLote, string fechaDesde, string fechaHasta)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaLotes)pagina.LoadControl("~/Controles/TablaLotes.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DescargaSolicitudes))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();
                            if (ValidarLote(numeroLote, fechaDesde, fechaHasta, errores, controles))
                            {
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                List<Lote> lotes = null;
                                if (numeroLote != "0")
                                {
                                    lotes = servicioCotizador.ListarLotePorNumero(Convert.ToInt32(numeroLote));
                                }
                                else
                                {
                                    lotes = servicioCotizador.ListarLotePorFecha(Convert.ToDateTime(fechaDesde, new CultureInfo("es-PE")), Convert.ToDateTime(fechaHasta, new CultureInfo("es-PE")));
                                }

                                control.Lotes = lotes;
                                control.PermisoConsultar = true;
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                                return respuesta;
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
                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Contenido = html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta ExportarReporte(string tokenUsuario, string numeroLote)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        HttpContext.Current.Session["numeroLote"] = numeroLote;
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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ex.Message });
                }

                return respuesta;
            }
        }

        private static bool ValidarLote(string numeroLote, string fechaDesde, string fechaHasta, List<string> errores, List<string> controles)
        {
            bool esCorrecto = true;

            // Número de Lote
            bool vNumeroLote = true;
            if (numeroLote.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Número de Lote</strong>. Dato Obligatorio.");
                vNumeroLote = false;
            }
            else
            {
                int iNumeroLote;
                if (!int.TryParse(numeroLote, out iNumeroLote))
                {
                    errores.Add("El campo <strong>Número de Lote</strong> debe contener un valor numérico.");
                    vNumeroLote = false;
                }
            }

            // Fecha plazo AFP desde
            bool vFechaAFPDesde = true;
            if (fechaDesde.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha plazo AFP desde</strong>. Dato Obligatorio.");
                vFechaAFPDesde = false;
            }
            else
            {
                DateTime dFechaAFPDesde;
                if (!DateTime.TryParse(fechaDesde, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out dFechaAFPDesde))
                {
                    errores.Add("El campo <strong>Fecha plazo AFP desde</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    vFechaAFPDesde = false;
                }
            }

            // Fecha plazo AFP hasta
            bool vFechaAFPHasta = true;
            if (fechaHasta.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha plazo AFP hasta</strong>. Dato Obligatorio.");
                vFechaAFPHasta = false;
            }
            else
            {
                DateTime dFechaAFPHasta;
                if (!DateTime.TryParse(fechaHasta, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out dFechaAFPHasta))
                {
                    errores.Add("El campo <strong>Fecha plazo AFP hasta</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    vFechaAFPHasta = false;
                }
            }

            // Clases de controles
            if (!vNumeroLote) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!vFechaAFPDesde) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!vFechaAFPHasta) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }

            esCorrecto = vNumeroLote | (vFechaAFPDesde & vFechaAFPHasta);

            return esCorrecto;
        }
    }
}