using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class EstudioNecesidadAPI
    {
        [DataMember]
        public string codigo_plantilla { get; set; }
        [DataMember]
        public SolicitudEdNAPI solicitud { get; set; }
        [DataMember]
        public List<CotizacionEdNAPI> cotizaciones { get; set; }
        [DataMember]
        public List<BeneficiarioEdNAPI> beneficiarios { get; set; }
    }

    [DataContract]
    public class SolicitudEdNAPI
    {
        [DataMember]
        public string num_solicitud { get; set; }
        [DataMember]
        public DateTime fec_solicitud { get; set; }
        [DataMember]
        public double val_mto_cta_individual { get; set; }
        [DataMember]
        public string cod_moneda_cta_indiv { get; set; }
    }

    [DataContract]
    public class CotizacionEdNAPI
    {
        [DataMember]
        public long num_correlativo { get; set; }
        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
        [DataMember]
        public double? val_per_diferido { get; set; }
    }

    public class BeneficiarioEdNAPI
    {
        [DataMember]
        public string ape_paterno { get; set; }
        [DataMember]
        public string ape_materno { get; set; }
        [DataMember]
        public string nom_persona { get; set; }
        [DataMember]
        public string cod_tipo_identificacion { get; set; }
        [DataMember]
        public string num_identificacion { get; set; }
        [DataMember]
        public string cod_parentezco { get; set; }
    }

    [DataContract]
    public class RespuestaEdNAPI
    {
        [DataMember]
        public string url { get; set; }
    }


}
