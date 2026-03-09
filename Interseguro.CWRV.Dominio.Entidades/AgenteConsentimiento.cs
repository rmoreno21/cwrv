using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class AgenteConsentimiento
    {
        [DataMember]
        public string supervisor { get; set; }
        [DataMember]
        public int entregPlazo { get; set; }
        [DataMember]
        public int porcentregPlazo { get; set; }
        [DataMember]
        public int noDevuelto { get; set; }
        [DataMember]
        public int porcNoDevuelto { get; set; }
        [DataMember]
        public int devuelta { get; set; }
        [DataMember]
        public int porcDevuelta { get; set; }
        [DataMember]
        public int total { get; set; }
        [DataMember]
        public int porcTotal { get; set; }
        [DataMember]
        public string padre { get; set; }
        [DataMember]
        public int nivel { get; set; }

    }

}
