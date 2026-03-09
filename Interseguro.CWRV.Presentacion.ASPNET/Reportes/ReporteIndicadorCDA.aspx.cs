using Interseguro.CWRV.Infraestructura.General;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class ReporteIndicadorCDA : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(IndicadorCdA));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string usuario = Session["Usuario"].ToString();

                if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ReporteIndicadorCDA))
                {
                    log.Debug($"Se va ha generar el reporte de indicadores CDA, usuario: {usuario}");

                    string fileName = Session["nombreArchivoCDA"].ToString();
                    byte[] byteArrayExcel = (byte[])Session["byteArrayExcelCDA"];

                    Session.Remove("nombreArchivoCDA");
                    Session.Remove("byteArrayExcelCDA");

                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("Content-disposition", $"filename={fileName}");
                    Response.BinaryWrite(byteArrayExcel);
                    Response.Flush();
                    Response.End();
                }
                else
                {
                    log.Warn(string.Format("El usuario [{0}] intentó acceder a una opción con la que no cuenta con privilegios [{1}]", usuario, Enums.OpcionesSistema.DescargaResultados));
                    Response.Redirect("~/Error/Permisos.aspx");
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                log.Error("Se ha producido un error al exportar el reporte de indicadores CDA", ex);
                Response.Redirect("~/Error/500.aspx");
            }
        }
    }
}