using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class CartaAFP : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportesRentaParticular));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string solicitud = Request.QueryString["s"];
                int correlativo = Convert.ToInt32(Request.QueryString["c"]);
                int lote = 0;
                string usuario = Session["Usuario"].ToString();

                log.Debug("[CartaAFP] El usuario [" + usuario + "] ingresó a la vista CartaAFP.aspx");

                if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CierrePolizaRVI))
                {
                    string nombreArchivo = string.Format("Carta_AFP_{0}_{1}.pdf", solicitud, correlativo);

                    log.Debug(string.Format("[CartaAFP] Se va a generar la carta para la AFP de la Solicitud [{0}] Correlativo [{1}] Usuario [{2}]", solicitud, correlativo, usuario));

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    RviCarta carta = servicioCotizador.ObtenerDatosCarta(solicitud, correlativo, usuario);

                    if (carta == null || string.IsNullOrEmpty(carta.num_solicitud))
                    {
                        log.Debug("Resultado de ObtenerDatosCarta: [NULL]");
                        // Registrar carta
                        carta = new RviCarta
                        {
                            num_solicitud = solicitud,
                            num_correlativo = correlativo,
                            aud_usr_ingreso = usuario
                        };
                        servicioCotizador.RegistrarCarta(carta);
                        carta = servicioCotizador.ObtenerDatosCarta(solicitud, correlativo, usuario);
                    }

                    byte[] archivo;
                    using (var client = new HttpClient())
                    {
                        archivo = client.GetByteArrayAsync(string.Format("{0}/reportes/reporte-carta/{1}/{2}/{3}/{4}", ConfigurationManager.AppSettings["url_base_api_cwrv"], solicitud, correlativo, lote, usuario)).Result;

                        Response.ClearHeaders();
                        Response.Clear();
                        Response.AddHeader("Content-Type", "application/pdf");
                        Response.AddHeader("Content-Length", archivo.Length.ToString());
                        Response.AddHeader("Content-Disposition", string.Format("inline; filename={0}", nombreArchivo));

                        Response.BinaryWrite(archivo);
                        Response.Flush();
                        Response.End();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                    //Response.Redirect(string.Format("{0}/reportes/reporte-carta/{1}/{2}/{3}/{4}", ConfigurationManager.AppSettings["url_base_api_cwrv"], solicitud, correlativo, lote, usuario));
                }
                else
                {
                    log.Warn(string.Format("[CartaAFP] El usuario [{0}] intentó acceder a una opción con la que no cuenta con privilegios [{1}]", usuario, Enums.OpcionesSistema.DescargaResultados));
                    Response.Redirect("~/Error/Permisos.aspx");
                }
            }
            catch (ThreadAbortException) { 
                log.Error("[CartaAFP] Se ha producido un ThreadAbortException de la vista CartaAFP.aspx");
            }
            catch (Exception ex)
            {
                log.Error("[CartaAFP] Se ha producido un error al exportar el reporte de Recálculo de Cotizaciones", ex);
                Response.Redirect("~/Error/500.aspx");
            }
        }
    }
}