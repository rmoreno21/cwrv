using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CausalPoliza
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string NombreCorto { get; set; }
        [DataMember]
        public string NombreLargo { get; set; }

    }
}
