using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Moneda
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Simbolo { get; set; }
    }
}
