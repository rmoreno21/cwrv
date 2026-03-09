using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CargaControlCdA
    {
        [DataMember]
        public DateTime fec_periodo { get; set; }

        [DataMember]
        public string num_cuspp { get; set; }

        [DataMember]
        public string gls_validacion { get; set; }

        [DataMember]
        public string gls_justificacion { get; set; }

        [DataMember]
        public string aud_usr_ingreso { get; set; }

        [DataMember]
        public string aud_usr_modificacion { get; set; }
    }
}
