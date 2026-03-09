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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Interseguro.CWRV.Presentacion.ASPNET.Meler
{
    public partial class CargaCotizaciones : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DescargaSolicitudes));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CargaCotizacionesMeler))
                {
                    List<Parametro> comboSiNo = new List<Parametro>();
                    comboSiNo.Add(new Parametro { Id = "S", Glosa = "Sí" });
                    comboSiNo.Add(new Parametro { Id = "N", Glosa = "No" });

                    CargarCombobox(SolicitudesEnviadas, comboSiNo);
                }
                else
                {
                    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                        Enums.OpcionesSistema.CargaCotizacionesMeler.StringValue()));
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
        public static Respuesta CargarCotizacionesMeler(string tokenUsuario, string fechaInicio, string fechaFin, string enviado)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaCotizacionesMeler)pagina.LoadControl("~/Controles/TablaCotizacionesMeler.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CargaCotizacionesMeler))
                        {
                            DateTime dFechaInicio = new DateTime();
                            if (fechaInicio.Length > 0)
                            {
                                dFechaInicio = Convert.ToDateTime(fechaInicio, new CultureInfo("es-PE"));
                            }
                            DateTime dFechaFin = new DateTime();
                            if (fechaFin.Length > 0)
                            {
                                dFechaFin = Convert.ToDateTime(fechaFin, new CultureInfo("es-PE"));
                            }

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Solicitud> solicitudes = servicioCotizador.ListarSolicitudesPorFechaCierreAFP(dFechaInicio, dFechaFin, enviado[0]);

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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message });
                    //respuesta.Controles = controles;
                    //throw (ex);
                }

                return respuesta;
            }
        }

        [WebMethod]
        public static Respuesta ModificarCotizacionesMeler(string tokenUsuario, string[] solicitudes)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CargaCotizacionesMeler))
                        {
                            // Generar XML de input
                            XDocument documento = new XDocument();
                            documento.Declaration = new XDeclaration("1.0", "utf-8", "yes");
                            XElement sols = new XElement("Solicitudes");
                            foreach (string solicitud in solicitudes)
                            {
                                sols.Add(
                                    new XElement("Solicitud",
                                        new XElement("nroOperacion", solicitud)
                                    )
                                );
                            }
                            documento.Add(sols);

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            
                            //<GTI.INI-29372>
                            var listaSolicitudes = servicioCotizador.ListarSolicitudesCargaMeler(documento.ToString(SaveOptions.DisableFormatting));
                            StringBuilder validacion = new StringBuilder();
                            int contadorErrores = 0;

                            foreach (Solicitud solicitud in listaSolicitudes)
                            {
                                string num_solicitud = "";
                                foreach (Cotizacion cotizacion in solicitud.Cotizaciones)
                                {
                                    if (!cotizacion.IndCotizacion && cotizacion.IndEnvioObligatorio)
                                    {
                                        if (cotizacion.Cotiza == "*")
                                        {
                                            contadorErrores++;
                                            num_solicitud = solicitud.Id;
                                            //textoValidacion = "Pensión debajo de la pensión mínima fijada por la SBS. Modalidad no se cotiza";
                                        }
                                        else if (cotizacion.Cotiza == "**")
                                        {
                                            contadorErrores++;
                                            num_solicitud = solicitud.Id;
                                            //textoValidacion = "Cotización no alcanza el mínimo requerido, por lo cual no se simula";
                                        }
                                        else if (cotizacion.Cotiza == "***")
                                        {
                                            contadorErrores++;
                                            num_solicitud = solicitud.Id;
                                            //textoValidacion = "Tasa AFP fuera del rango permitido";
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(num_solicitud))
                                    validacion.Append("Solicitud " + num_solicitud + " <br />");
                            }

                            if (contadorErrores == 0)
                            {
                                //<GTI.FIN-29372>
                                respuesta = servicioCotizador.ActualizarSolicitudesCargaMeler(documento.ToString(SaveOptions.DisableFormatting), (string)HttpContext.Current.Session["Usuario"]);
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = "Archivo XML no generado.<br />Existen solicitudes obligatorias sin cotizar:<br />" + validacion.ToString();
                            }
                            
                            HttpContext.Current.Session["XMLCargaCotizaciones"] = respuesta.Contenido;
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.CargaConfirmaciones.StringValue()));
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }
    }
}