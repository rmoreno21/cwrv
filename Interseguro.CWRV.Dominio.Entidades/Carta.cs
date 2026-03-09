using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class RviCarta
    {
        [DataMember]
        public string num_solicitud { get; set; }
        [DataMember]
        public int num_correlativo { get; set; }
        [DataMember]
        public string cod_carta { get; set; }
        [DataMember]
        public string ind_emitida { get; set; }
        [DataMember]
        public string gls_documento { get; set; }
        [DataMember]
        public DateTime? fec_registro { get; set; }
        [DataMember]
        public DateTime? fec_emision { get; set; }
        [DataMember]
        public int num_lote_emision { get; set; }
        [DataMember]
        public string aud_usr_ingreso { get; set; }
        [DataMember]
        public DateTime? aud_fec_ingreso { get; set; }
        [DataMember]
        public string aud_usr_modificacion { get; set; }
        [DataMember]
        public DateTime? aud_fec_modificacion { get; set; }
    }
}
