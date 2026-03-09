using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class ReportesCloudStorage : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportesCloudStorage));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                log.Info("Consumir reportes cloud storage.");

                string solicitud = Request.QueryString["solicitud"];
                //string tipoDocumento = Request.QueryString["tipdoc"];
                //string numeroDocumento = Request.QueryString["numdoc"];
                string reporte = Request.QueryString["formato"];

                string usuario = HttpContext.Current.Session["Usuario"].ToString();
                string nombreArchivo = string.Empty;

                //Armar Json EdN
                EstudioNecesidadAPI estudioNecesidadAPI = new EstudioNecesidadAPI();
                SolicitudEdNAPI solicitud_edn = new SolicitudEdNAPI();
                List<CotizacionEdNAPI> cotizaciones_edn = new List<CotizacionEdNAPI>();
                CotizacionEdNAPI cotizacion_edn;
                BeneficiarioEdNAPI beneficiario_edn;

                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                string tipo_solicitud = solicitud.Substring(0, 3);
                dynamic listaCotiza;
                List<GrupoFamiliar> beneficiarios = new List<GrupoFamiliar>();
                string temporalidad = string.Empty;

                if (tipo_solicitud == Enums.TipoCotizacion.RentaPrivadaIFP.StringValue())
                {
                    SolicitudIFP solicitudIFP = servicioCotizador.ObtenerDatosSolicitudIFP(solicitud);

                    solicitud_edn.num_solicitud = solicitudIFP.Id;
                    solicitud_edn.fec_solicitud = solicitudIFP.FechaSolicitud.Value;
                    solicitud_edn.val_mto_cta_individual = solicitudIFP.PrimaUnica;
                    solicitud_edn.cod_moneda_cta_indiv = solicitudIFP.MonedaPrimaUnica.Id;

                    listaCotiza = solicitudIFP.Cotizaciones;

                    //temporalidad = cotiza.Temporalidad.Anhos.ToString();
                    beneficiarios = solicitudIFP.Beneficiarios;
                }
                else
                {
                    SolicitudRPPlus solicitudRPPlus = servicioCotizador.ObtenerDatosSolicitudRPPlus(solicitud);

                    solicitud_edn.num_solicitud = solicitudRPPlus.Id;
                    solicitud_edn.fec_solicitud = solicitudRPPlus.FechaSolicitud.Value;
                    solicitud_edn.val_mto_cta_individual = solicitudRPPlus.PrimaUnica;
                    solicitud_edn.cod_moneda_cta_indiv = solicitudRPPlus.MonedaPrimaUnica.Id;

                    listaCotiza = solicitudRPPlus.Cotizaciones;

                    temporalidad = solicitudRPPlus.Temporalidad.Id;
                    beneficiarios = solicitudRPPlus.Beneficiarios;
                }

                estudioNecesidadAPI.solicitud = solicitud_edn;

                if (listaCotiza != null)
                {
                    foreach (var cot in listaCotiza)
                    {
                        cotizacion_edn = new CotizacionEdNAPI();
                        cotizacion_edn.num_correlativo = cot.Correlativo;

                        if (tipo_solicitud == Enums.TipoCotizacion.RentaPrivadaIFP.StringValue())
                        {
                            cotizacion_edn.cod_tipo_temporalidad = cot.Temporalidad.Anhos.ToString();
                            cotizacion_edn.val_per_diferido = cot.ValPerDiferido;
                        }
                        else
                        {
                            cotizacion_edn.cod_tipo_temporalidad = temporalidad;
                            cotizacion_edn.val_per_diferido = cot.PeriodoDiferido;
                        }
                        
                        cotizaciones_edn.Add(cotizacion_edn);
                    }
                    
                    estudioNecesidadAPI.cotizaciones = new List<CotizacionEdNAPI>();
                    estudioNecesidadAPI.cotizaciones = cotizaciones_edn;
                }

                log.Info("Cantidad de beneficiarios(reportes CS): " + beneficiarios.Count);
                beneficiarios = beneficiarios.GroupBy(be => new { be.Identificacion.IdTipo, be.Identificacion.Numero }).Select(s => s.First()).ToList();
                log.Info("Cantidad de beneficiarios(reportes CS): " + beneficiarios.Count);

                estudioNecesidadAPI.beneficiarios = new List<BeneficiarioEdNAPI>();
                foreach (var benefi in beneficiarios)
                {
                    beneficiario_edn = new BeneficiarioEdNAPI();
                    beneficiario_edn.ape_paterno = benefi.ApellidoPaterno;
                    beneficiario_edn.ape_materno = benefi.ApellidoMaterno;
                    beneficiario_edn.nom_persona = benefi.Nombre;

                    switch (benefi.Identificacion.IdTipo)
                    {
                        case "D":
                            beneficiario_edn.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                            break;
                        case "E":
                            beneficiario_edn.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.CE.StringValue();
                            break;
                        case "P":
                            beneficiario_edn.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.PAS.StringValue();
                            break;
                        default:
                            beneficiario_edn.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                            break;
                    }

                    beneficiario_edn.num_identificacion = benefi.Identificacion.Numero;
                    beneficiario_edn.cod_parentezco = benefi.Parentesco.Id;

                    estudioNecesidadAPI.beneficiarios.Add(beneficiario_edn);
                }


                switch (reporte)
                {
                    case "1":
                        //nombreArchivo = "EN_" + solicitud + "_" + numeroDocumento + ".pdf";
                        nombreArchivo = "EN_" + solicitud + "_" + beneficiarios.Find(ben => ben.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue()).Identificacion.Numero + ".pdf";
                        break;
                }
                
                var respuesta_api_EdN = Utilitario.ObtenerEstudioNecesidadesLocal(estudioNecesidadAPI, usuario);

                log.Info(string.Format("Obtener el formato de estudio de necesidades. archivo[{0}]", nombreArchivo));

                var respuesta_api_edn_deserializa = JsonConvert.DeserializeObject<string>(respuesta_api_EdN);

                Response.Clear();

                /*try
                {
                    using (var wc = new System.Net.WebClient())
                    {
                        formatoBinario = wc.DownloadData(respuesta_api_EdN);
                    }
                }
                catch (WebException wex)
                {
                    log.Error("Se ha producido un error en la descarga del documento Cloud Storage.", wex);

                    HttpWebResponse errorResponse = wex.Response as HttpWebResponse;
                    if (errorResponse.StatusCode == HttpStatusCode.NotFound)
                    {
                        Response.Redirect("~/Error/Documento.aspx");
                    }
                    else
                    {
                        Response.Redirect("~/Error/500.aspx");
                    }
                }*/

                MemoryStream ms = new MemoryStream(System.Convert.FromBase64String(respuesta_api_edn_deserializa));
                Response.ContentType = "application/pdf";                
                Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", nombreArchivo));
                Response.Buffer = true;
                ms.WriteTo(Response.OutputStream);
                
                Response.End();
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                log.Error("Se ha producido un error en ReportesCloudStorage.", ex);
                Response.Redirect("~/Error/500.aspx");
            }
        }
    }
}