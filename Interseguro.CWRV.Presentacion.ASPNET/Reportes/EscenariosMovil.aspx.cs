using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using log4net;
using System.Reflection;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Dominio.Entidades;
using System.Threading;
using System.ServiceModel;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class EscenariosMovil : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DetalleCotizacionMovil));

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (!IsPostBack)
                    {
                        log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                    }
                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudReporteEscenarios))
                    {
                        if (Utilitarios.EsRolVerAgentesCesados((string)Session["RolAzman"]) || ((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["AgenteReporte"]))
                        {
                            Response.Clear();
                            Response.ClearHeaders();
                            Response.Buffer = true;
                            string filename = "Escenarios.pdf";
                            Response.AddHeader("Content-Disposition", "attachment;filename=" + filename);
                            Response.Charset = "";
                            Response.ContentType = "application/octet-stream";

                            Response.BinaryWrite((byte[])Session["EscenariosPDF"]);
                            Response.Flush();
                            Response.End();
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a la información de una solicitud a la que no tiene privilegios."));
                            Response.Redirect("~/Error/Permisos.aspx");
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.SolicitudReporteEscenarios.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                }
            }
        }
    }
}