using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class PolizasDashboard
    {
        [DataMember]
        public string gls_identificador { get; set; }
        [DataMember]
        public string nombres { get; set; }
        [DataMember]
        public string num_poliza { get; set; }
        [DataMember]
        public DateTime? fec_envio { get; set; }
        [DataMember]
        public int? id_proceso_envio { get; set; }
        [DataMember]
        public string cod_agente { get; set; }
        [DataMember]
        public string nom_agente { get; set; }
        [DataMember]
        public string cod_username { get; set; }
        [DataMember]
        public string gls_num_identificacion { get; set; }
        [DataMember]
        public string gls_num_cuspp { get; set; }
        [DataMember]
        public int? val_cant_envios_consentimiento { get; set; }
        [DataMember]
        public string gls_nombres_supervisor { get; set; }
        [DataMember]
        public string gls_nombres_jefe { get; set; }

    }

}
