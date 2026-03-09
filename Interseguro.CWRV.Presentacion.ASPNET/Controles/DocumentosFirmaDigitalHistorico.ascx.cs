using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class DocumentosFirmaDigitalHistorico : System.Web.UI.UserControl
    {
        public List<FormatoSolicitud> Formatos { get; set; }
        //<INI.GTI_52310>
        //public GrupoFamiliar Titular { get; set; }
        public bool IndicadorDescargaFormatoEN { get; set; }
        //<FIN.GTI_52310>

        protected void Page_Load(object sender, EventArgs e)
        {
            string tabs = string.Empty;
            string divs = string.Empty;

            if (Formatos != null)
            {
                int contador = 1;
                if (Formatos.Count > 0)
                {
                    tabs += "<div class=\"col s12\"><ul class=\"tabs\">";
                    foreach (FormatoSolicitud formato in Formatos)
                    {
                        tabs += string.Format("<li class=\"tab col s3\"><a {0}href=\"#firma{1}\">Versión {2}</a></li>", contador == Formatos.Count ? "class=\"active\" " : string.Empty, contador, formato.Correlativo);
                        divs += string.Format("<div id=\"firma{0}\" class=\"col s12\">", contador);
                        divs += string.Format("<div class=\"row\" style=\"margin-bottom:0\"><div class=\"col s12\"><p style=\"font-size:1rem;padding:10px\"><i class=\"material-icons light-blue-text\" style=\"margin-right:10px\">verified</i> Documentos firmados el {0} a las {1} hrs.</p></div></div>", formato.FirmaDigitalLog.FechaConsentimiento.ToString("dd/MM/yyyy"), formato.FirmaDigitalLog.FechaConsentimiento.ToString("hh:mm:ss"));
                        
                        divs += "<div class=\"col s12\"><table class=\"highlight\" style=\"font-size:1rem\"><thead><tr><th>N°</th><th>Documento</th><th class=\"center\">Acción</th></tr></thead><tbody>";

                        int orden = 1;

                        // (1) Solicitud RPP & (2) Solicitud IFP
                        int formatoId = 0;
                        if (formato.TipoCotizacion.Id == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue()) formatoId = 1;
                        else if (formato.TipoCotizacion.Id == Enums.TipoCotizacion.RentaPrivadaIFP.StringValue()) formatoId = 2;
                        divs += string.Format("<tr><td>{0}</td><td>Solicitud {1}</td><td class=\"center\"><a target=\"_blank\" href=\"{2}\"><i class=\"material-icons pink-text darken-2-text\">file_download</i></a></td></tr>", orden, formato.TipoCotizacion.Id, ResolveUrl(string.Format("~/Reportes/ReportesRentaParticular.aspx?solicitud={0}&formato={1}&fs={2}&v={3}", formato.NumeroSolicitud, formatoId, formato.Id, formato.Correlativo)));
                        orden++;

                        // (6) Detalle Cotización
                        int tipoFormato = 0;
                        if (formato.TipoCotizacion.Id == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                        { tipoFormato = 6; }
                        else if (formato.TipoCotizacion.Id == Enums.TipoCotizacion.RentaPrivadaIFP.StringValue())
                        { tipoFormato = 8; }

                        divs += string.Format("<tr><td>{0}</td><td>Detalle de Cotización</td><td class=\"center\"><a target=\"_blank\" href=\"{1}\"><i class=\"material-icons pink-text darken-2-text\">file_download</i></a></td></tr>", orden, ResolveUrl(string.Format("~/Reportes/ReportesRentaParticular.aspx?solicitud={0}&formato={1}&fs={2}&v={3}", formato.NumeroSolicitud, tipoFormato, formato.Id, formato.Correlativo)));
                        orden++;

                        // (7) Detalle Rescate
                        if (formato.Rescate)
                        {
                            divs += string.Format("<tr><td>{0}</td><td>Detalle de Rescate</td><td class=\"center\"><a target=\"_blank\" href=\"{1}\"><i class=\"material-icons pink-text darken-2-text\">file_download</i></a></td></tr>", orden, ResolveUrl(string.Format("~/Reportes/ReportesRentaParticular.aspx?solicitud={0}&formato={1}&fs={2}&v={3}", formato.NumeroSolicitud, 7, formato.Id, formato.Correlativo)));
                            orden++;
                        }

                        // (3) Origen de Fondos
                        divs += string.Format("<tr><td>{0}</td><td>Formato de Origen de Fondos</td><td class=\"center\"><a target=\"_blank\" href=\"{1}\"><i class=\"material-icons pink-text darken-2-text\">file_download</i></a></td></tr></tr>", orden, ResolveUrl(string.Format("~/Reportes/ReportesRentaParticular.aspx?solicitud={0}&formato={1}&fs={2}&v={3}", formato.NumeroSolicitud, 3, formato.Id, formato.Correlativo)));
                        orden++;

                        // (4) PEP
                        if (formato.Afiliado.PEP == 'S')
                        {
                            divs += string.Format("<tr><td>{0}</td><td>Formato PEP</td><td class=\"center\"><a target=\"_blank\" href=\"{1}\"><i class=\"material-icons pink-text darken-2-text\">file_download</i></a></td></tr>", orden, ResolveUrl(string.Format("~/Reportes/ReportesRentaParticular.aspx?solicitud={0}&formato={1}&fs={2}&v={3}", formato.NumeroSolicitud, 4, formato.Id, formato.Correlativo)));
                            orden++;
                        }

                        // (5) Constancia de Abono
                        divs += string.Format("<tr><td>{0}</td><td>Constancia de Abono</td><td class=\"center\"><a target=\"_blank\" href=\"{1}\"><i class=\"material-icons pink-text darken-2-text\">file_download</i></a></td></tr>", orden, ResolveUrl(string.Format("~/Reportes/ReportesRentaParticular.aspx?solicitud={0}&formato={1}&fs={2}&v={3}", formato.NumeroSolicitud, 5, formato.Id, formato.Correlativo)));
                        orden++;

                        //<INI.GTI_52310>
                        if (IndicadorDescargaFormatoEN)
                        {
                            divs += string.Format("<tr><td>{0}</td><td>Estudio de Necesidades</td><td class=\"center\"><a target=\"_blank\" href=\"{1}\"><i class=\"material-icons pink-text darken-2-text\">file_download</i></a></td></tr>", orden, ResolveUrl(string.Format("~/Reportes/ReportesCloudStorage.aspx?solicitud={0}&formato={1}", formato.NumeroSolicitud, 1)));
                            orden++;
                        }
                        //<FIN.GTI_52310>

                        divs += "</tbody></table></div></div>";
                        contador++;
                    }
                    tabs += "</ul></div>";
                }
                else
                {
                    tabs += "<div class=\"row\"><div class=\"col s12\"><p style=\"font-size:1rem;padding:10px\"><i class=\"material-icons light-blue-text\" style=\"margin-right:10px\">info</i> No se han encontrado documentos firmados para esta solicitud.</p></div></div>";
                }
            }
            else
            {
                tabs += "<div class=\"row\"><div class=\"col s12\"><p style=\"font-size:1rem;padding:10px\"><i class=\"material-icons light-blue-text\" style=\"margin-right:10px\">info</i> No se han encontrado documentos firmados para esta solicitud.</p></div></div>";
            }
            Pestanhas.Text = tabs + divs;
        }
    }
}