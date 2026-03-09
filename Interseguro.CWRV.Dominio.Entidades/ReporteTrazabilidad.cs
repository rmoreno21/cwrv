using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class ReporteTrazabilidad
    {
        [DataMember]
        public int id_envio_seguimiento { get; set; }
        [DataMember]
        public string gls_identificador { get; set; }
        [DataMember]
        public int id_proceso_envio { get; set; }
        [DataMember]
        public long id_sme { get; set; }
        [DataMember]
        public string cod_estado_trazabilidad { get; set; }
        [DataMember]
        public string gls_mail { get; set; }
        [DataMember]
        public string fec_envio { get; set; }
        [DataMember]
        public string cod_agente { get; set; }
        [DataMember]
        public string aud_usr_ingreso { get; set; }
        [DataMember]
        public DateTime aud_fec_ingreso { get; set; }
        [DataMember]
        public string aud_usr_modificacion { get; set; }
        [DataMember]
        public DateTime aud_fec_modificacion { get; set; }
        [DataMember]
        public string gls_motivo_rebote { get; set; }
        [DataMember]
        public string nom_agente { get; set; }
        [DataMember]
        public string fec_firma { get; set; }
        [DataMember]
        public string num_cuspp { get; set; }
        [DataMember]
        public string gls_persona { get; set; }
        [DataMember]
        public string gls_nombres_supervisor { get; set; }
        [DataMember]
        public string gls_nombres_jefe { get; set; }
    }
}
