using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.ConsultasXLS;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Cotizador
{
    public partial class ConsultaSolicitudesAnticipo : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Cotizador));
        private static IServicioCWRV servicioCotizador;
        HSSFWorkbook hssfworkbook;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string CargarTablaAceptacionAnticipo(string tokenUsuario, string solicitud, string fechaDesde, string fechaHasta, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesAnticipo)pagina.LoadControl("~/Controles/TablaSolicitudesAnticipo.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ConsultaSolicitudesAnticipo))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Anticipo> anticipos = servicioCotizador.ListarAnticipoAceptacion(solicitud, "", fechaDesde, fechaHasta, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar);

                            control.Anticipos = anticipos;

                            control.PermisoConsultar = true;
                            control.PermisoExportarExcel = true;
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

        protected void ExportarExcel_Click(object sender, EventArgs e)
        {
            try
            {
                List<Anticipo> solicitudesAnticipo = new List<Anticipo>();

                //DateTime fecInicio = Convert.ToDateTime(FechaDesde.Text, new CultureInfo("es-PE"));
                //DateTime fecTermino = Convert.ToDateTime(FechaHasta.Text, new CultureInfo("es-PE"));

                if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ConsultaSolicitudesAnticipo))
                {
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    solicitudesAnticipo = servicioCotizador.ListarAnticipoAceptacion(NumeroSolicitud.Text, String.Empty, FechaDesde.Text, FechaHasta.Text, 1, 999999999, 1, 'A');
                }
                else
                {
                    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                        Enums.OpcionesSistema.ConsultaSolicitudesAnticipo.StringValue()));
                }

                string NombreArchivo = "ConsultaSolicitudesAnticipo_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", NombreArchivo));
                Response.Clear();

                ExportarNpoiXLS builder = new ExportarNpoiXLS(hssfworkbook, NombreArchivo, solicitudesAnticipo);
                builder.InitializeWorkbook();
                builder.BuildSolicitudesAnticipo();
                builder.GetExcelStream().WriteTo(Response.OutputStream);
                Response.End();
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0]", ex.Message), ex);
                MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                MCMEstado.Value = "1";
            }
        }
    }
}