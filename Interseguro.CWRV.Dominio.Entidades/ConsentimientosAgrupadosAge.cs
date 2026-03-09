using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ConsentimientosAgrupadosAge
    {
        [DataMember]
        public int num_agente { get; set; }
        [DataMember]
        public string gls_nombres_agente { get; set; }
        [DataMember]
        public string ind_consentimiento { get; set; }
        [DataMember]
        public int cantidad_ind_consentimiento { get; set; }
        [DataMember]
        public string fec_primer_envio { get; set; }
        [DataMember]
        public string fec_ultimo_envio { get; set; }
        [DataMember]
        public string fec_asignacion { get; set; }
        [DataMember]
        public int entrega_plazo { get; set; }

    }
}
