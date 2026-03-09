using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{

    [DataContract]
    public class ReporteRecalculoCotizacion
    {
        [DataMember]
        public DateTime? fec_cotizacion { get; set; }
        [DataMember]
        public DateTime? fec_devengue { get; set; }
        [DataMember]
        public string num_solicitud { get; set; }
        [DataMember]
        public string num_cuispp { get; set; }
        [DataMember]
        public string num_vendedor { get; set; }
        [DataMember]
        public string nom_agente { get; set; }
        [DataMember]
        public string cod_cartera { get; set; }
        [DataMember]
        public string gls_cartera { get; set; }
        [DataMember]
        public string gls_persona { get; set; }
        [DataMember]
        public string direccion { get; set; }
        [DataMember]
        public string ubigeo { get; set; }
        [DataMember]
        public string gls_celular { get; set; }
        [DataMember]
        public string gls_afp { get; set; }
        [DataMember]
        public string gls_tipo_pension { get; set; }
        [DataMember]
        public string gls_categoria { get; set; }
        [DataMember]
        public string gls_tipo_cotizacion { get; set; }
        [DataMember]
        public double? val_moneda { get; set; }
        [DataMember]
        public double? tasaAssetshare { get; set; }
        [DataMember]
        public double? tasaVenta { get; set; }
        [DataMember]
        public double? TRA { get; set; }
        [DataMember]
        public double? CRU { get; set; }
        [DataMember]
        public double? val_mto_cta_individual { get; set; }
        [DataMember]
        public string gls_corta_moneda { get; set; }
        [DataMember]
        public double? val_mto_cia { get; set; }
        [DataMember]
        public DateTime? fec_ult_actualizacion { get; set; }
        [DataMember]
        public double? val_tasa_int_temp { get; set; }
        [DataMember]
        public double? val_mon_aju { get; set; }
        [DataMember]
        public int? ajtMon { get; set; }
        [DataMember]
        public ReporteRecalculoCotizacionDetalle reporteRecalculoCotizacionDetalle { get; set; }
        [DataMember]
        public List<ReporteRecalculoCotizacionBeneficiarios> listaReporteRecalculoCotizacionBeneficiarios { get; set; }

        public string XMLParametrosRecalculo(List<ReporteRecalculoCotizacion> listaRecalculoCotizacion)
        {
            XDocument xml = new XDocument();
            xml.Declaration = new XDeclaration("1.0", "utf-8", "yes");
            XElement root = new XElement("formato");

            XElement parametros;

            foreach (var recalculo in listaRecalculoCotizacion)
            {
                //List<XAttribute> atributos = new List<XAttribute>();
                //atributos.Add(new XAttribute("num_solicitud", recalculo.num_solicitud));
                //atributos.Add(new XAttribute("num_correlativo", recalculo.reporteRecalculoCotizacionDetalle.num_correlativo));

                //parametros = new XElement("solicitud", atributos);

                parametros = new XElement("solicitud",
                                                new XElement("num_solicitud", recalculo.num_solicitud),                                                new XElement("num_correlativo", recalculo.reporteRecalculoCotizacionDetalle.num_correlativo));

                root.Add(parametros);
            }

            xml.Add(root);

            return xml.ToString();
        }


    }

}
