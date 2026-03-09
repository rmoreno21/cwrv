using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{

    [DataContract]
    public class ReporteRecalculoCotizacionDetalle
    {
        [DataMember]
        public int? num_correlativo { get; set; }
        [DataMember]
        public string gls_corta_tipo_pro { get; set; }
        [DataMember]
        public string modalidad { get; set; }
        [DataMember]
        public int? val_per_temporal { get; set; }
        [DataMember]
        public int? val_pje_rent_temp { get; set; }
        [DataMember]
        public int? val_per_garantizado { get; set; }
        [DataMember]
        public string ind_derecho_crecer { get; set; }
        [DataMember]
        public string ind_gratificacion { get; set; }
        [DataMember]
        public double? AFPPension { get; set; }
        [DataMember]
        public double? pension { get; set; }
        [DataMember]
        public string monedaPension { get; set; }
        [DataMember]
        public string gls_corta_moneda { get; set; }
        [DataMember]
        public int? capital { get; set; }

    }

}
