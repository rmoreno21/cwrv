using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class FirmaDigital
    {
        [DataMember]
        public int id_firma_digital { get; set; }
        [DataMember]
        public string gls_token { get; set; }
        [DataMember]
        public string num_solicitud { get; set; }
        [DataMember]
        public int num_item { get; set; }
        [DataMember]
        public string gls_ip { get; set; }
        [DataMember]
        public string gls_browser_agent { get; set; }
        [DataMember]
        public DateTime fec_consentimiento { get; set; }
        [DataMember]
        public string ind_consentimiento { get; set; }
        [DataMember]
        public string aud_usr_ingreso { get; set; }
        [DataMember]
        public string aud_usr_modificacion { get; set; }
        [DataMember]
        public DateTime? fec_primer_envio { get; set; }
        [DataMember]
        public DateTime? fec_ultimo_envio { get; set; }
    }
}
