using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class IPPermitida
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public string IP { get; set; }
        [DataMember]
        public int SegundosExpiracion { get; set; }
    }
}
