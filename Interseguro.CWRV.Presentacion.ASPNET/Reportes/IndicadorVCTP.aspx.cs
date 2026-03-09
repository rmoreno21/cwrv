using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class IndicadorVCTP : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IndicadorVCTP));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (!Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteIndicadorVCTP))
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ReporteIndicadorVCTP.StringValue()));
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
        }

        protected void btnDescargarVCTP_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (!string.IsNullOrEmpty(txtPeriodo.Value))
                    {
                        DateTime fecPeriodo = DateTime.Parse(txtPeriodo.Value);
                        string codUserName = (string)HttpContext.Current.Session["Usuario"];
                        string codRol = (string)HttpContext.Current.Session["RolAzman"];

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.ListarReporteIndicadoresVCTP(fecPeriodo, codUserName, codRol);

                        string templatePath = ConfigurationManager.AppSettings["ruta_plantilla_reporte_indicadoresVCTP"];

                        FileInfo templateFile = new FileInfo(templatePath);
                        string fileName = string.Format("Reporte_Indicadores_VCTP_{0}.xlsx", fecPeriodo.ToString("yyyyMMdd"));

                        Response.Clear();
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        Response.ContentEncoding = System.Text.Encoding.UTF8;
                        Response.Cache.SetCacheability(HttpCacheability.NoCache);
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileName, System.Text.Encoding.UTF8));

                        using (ExcelPackage pck = new ExcelPackage(templateFile))
                        {
                            ExcelWorksheet ws = pck.Workbook.Worksheets[1];

                            ws.Cells["A1"].LoadFromDataTable(respuesta.Data.Tables[0], true);
                            var memoryStream = new MemoryStream();
                            pck.SaveAs(memoryStream);

                            memoryStream.WriteTo(Response.OutputStream);
                        }
                        Response.Flush();
                        Response.End();
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
    }
}