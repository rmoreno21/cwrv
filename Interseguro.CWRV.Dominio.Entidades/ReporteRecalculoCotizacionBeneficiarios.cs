using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{

    [DataContract]
    public class ReporteRecalculoCotizacionBeneficiarios
    {
        [DataMember]
        public int? num_correlativo { get; set; }
        [DataMember]
        public string gls_corta_parentezco { get; set; }
        [DataMember]
        public DateTime? fec_nacimiento { get; set; }
        [DataMember]
        public string cod_sexo { get; set; }
        [DataMember]
        public string ind_invalidez { get; set; }
        [DataMember]
        public string gls_corta_tipinv { get; set; }
        [DataMember]
        public string gls_persona { get; set; }
    }

}
