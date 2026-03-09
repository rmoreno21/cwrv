using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Comun
{
    public partial class BusquedaPersonas : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BusquedaPersonas));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static string CargarTablaPersonas(string tokenUsuario, string origen, string apellidoPaterno, string apellidoMaterno, string nombres)
        {
            try
            {
                if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                {
                    var pagina = new Page();
                    var control = (TablaPersonas)pagina.LoadControl("~/Controles/TablaPersonas.ascx");
                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.BusquedaAfiliadoConsultar))
                    {
                        int totalRegistros = 0;

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<Afiliado> afiliados = servicioCotizador.ListarAfiliado(apellidoPaterno, apellidoMaterno, nombres, 1, 99999, 1, 'A', ref totalRegistros);

                        control.Afiliados = afiliados;
                        control.Origen = origen;

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
                        log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [BusquedaAfiliadoConsultar] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                        HttpContext.Current.Response.Status = "403 Forbidden";
                        HttpContext.Current.Response.StatusCode = 403;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                else
                {
                    log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                    HttpContext.Current.Response.Status = "401 Unauthorized";
                    HttpContext.Current.Response.StatusCode = 401;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.Error("Se ha producido un error al realizar la búsqueda de personas.", ex);
                throw ex;
            }
        }
    }
}