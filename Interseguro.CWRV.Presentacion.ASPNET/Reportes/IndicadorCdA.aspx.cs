using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class IndicadorCdA : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IndicadorCdA));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteIndicadorCDA))
            {
                if (!IsPostBack)
                {
                    log.Info($"Usuario accedió a la opción [{Request.Url.AbsolutePath}].");
                }
            }
            else
            {
                log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.ReporteIndicadorCDA.StringValue()));
                Response.Redirect("~/Error/Permisos.aspx");
            }
        }

        [WebMethod]
        public static Respuesta descargarReporteCDA(string tokenUsuario, string periodo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (!string.IsNullOrEmpty(periodo))
                        {

                            string uriPlantillaCDA = ConfigurationManager.AppSettings["ruta_plantilla_reporte_indicadoresCDA"].ToString();

                            DateTime fecPeriodo = DateTime.Parse(periodo);
                            string usuario = HttpContext.Current.Session["Usuario"].ToString();
                            string rolUsuario = HttpContext.Current.Session["RolAzman"].ToString();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();

                            log.Debug("Inicio comunicación método: ServicioCWRV.ListarReporteIndicadoresCDA");
                            respuesta = servicioCotizador.ListarReporteIndicadoresCDA(fecPeriodo, usuario, rolUsuario);
                            log.Debug("Fin comunicación método: ServicioCWRV.ListarReporteIndicadoresCDA");

                            if (respuesta.Data.Tables[0].Rows.Count > 0)
                            {
                                FileInfo templateFile = new FileInfo(uriPlantillaCDA);
                                string fileName = $"Reporte_Indicadores_CDA_{fecPeriodo.ToString("yyyyMMdd")}.xlsx";
                                byte[] byteArrayExcel = null;

                                using (ExcelPackage pck = new ExcelPackage(templateFile))
                                {
                                    ExcelWorksheet ws = pck.Workbook.Worksheets[1];

                                    ws.Cells["A1"].LoadFromDataTable(respuesta.Data.Tables[0], true);
                                    byteArrayExcel = pck.GetAsByteArray();
                                }

                                HttpContext.Current.Session["nombreArchivoCDA"] = fileName;
                                HttpContext.Current.Session["byteArrayExcelCDA"] = byteArrayExcel;
                            }
                            respuesta.Data = null;
                        }
                        else
                        {
                            log.Error("No se ha indicado un periodo para la generación del reporte.");
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "No se ha indicado un periodo para la generación del reporte." });
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
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
                    log.Error($"Se ha producido el siguiente error: [{ex.Message}]", ex);
                    throw ex;
                }
            }
        }
    }
}