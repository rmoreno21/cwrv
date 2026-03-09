using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Interseguro.CWRV.Presentacion.ASPNET.Cotizador
{
    public partial class SeleccionarBeneficiarioCierre : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BeneficiarioCierre));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            string nroSolicitud = Request.QueryString["s"];
            DateTime fechaCotizacion = Convert.ToDateTime(Request.QueryString["fc"], new CultureInfo("es-PE"));
            List<Beneficiario> beneficiarios;

            // Obtener los datos de la solicitud
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            Solicitud solicitud = servicioCotizador.ObtenerDatosSolicitud(nroSolicitud, fechaCotizacion);
            beneficiarios = servicioCotizador.ListarBeneficiarios(nroSolicitud, Session["Usuario"].ToString());

            if (solicitud.TipoPension.Id != Enums.TipoPension.Sobrevivencia.StringValue())
            {
                // Redirigir automáticamente al titular
                Beneficiario titular = beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());
                Response.Redirect(string.Format("~/Cotizador/BeneficiarioCierre.aspx?s={0}&fc={1}&c={2}", solicitud.Id, Request.QueryString["fc"], titular.numCorrelativo));
            }
            else
            {
                // Cargar la tabla de beneficiarios con el afiliado filtrado
                beneficiarios = beneficiarios.FindAll(b => b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue());
                TablaBeneficiarios.Beneficiarios = beneficiarios;

                // Obtener beneficiarios desde la cwrv_grupo_familiar
                List<GrupoFamiliar> beneficiariosGF = servicioCotizador.ListarGrupoFamiliar(solicitud.Afiliado.CUSPP);
                TablaBeneficiarios.BeneficiariosGF = beneficiariosGF;
            }
        }
    }
}