using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{

    [Serializable]
    [DataContract]
    public class EstadoCivil
    {
        [DataMember]
        public string cod_estado_civil { get; set; }
        [DataMember]
        public string gls_estado_civil { get; set; }
        [DataMember]
        public string aud_usr_ingreso { get; set; }
        [DataMember]
        public DateTime? aud_fec_ingreso { get; set; }
    }

}
