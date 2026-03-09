using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{

    [Serializable]
    [DataContract]
    public class Profesion
    {
        [DataMember]
        public string cod_profesion { get; set; }
        [DataMember]
        public string gls_profesion { get; set; }
        [DataMember]
        public string aud_usr_ingreso { get; set; }
        [DataMember]
        public DateTime? aud_fec_ingreso { get; set; }
    }

}
