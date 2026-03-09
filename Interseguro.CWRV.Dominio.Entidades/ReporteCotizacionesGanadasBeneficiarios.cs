using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class ReporteCotizacionesGanadasBeneficiarios
    {
        [DataMember]
        public int num_operacion { get; set; }
        [DataMember]
        public string beneficiario { get; set; }
        [DataMember]
        public string gls_parentezco { get; set; }
        [DataMember]
        public string cod_condicion_invalidez { get; set; }
        [DataMember]
        public DateTime fec_nacimiento_beneficiario { get; set; }
        [DataMember]
        public string cod_genero_beneficiario { get; set; }

    }

}
