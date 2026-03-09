using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class SDAReporte
    {
        [DataMember]
        public double AvanceCartera { get; set; }

        [DataMember]
        public string Precubo { get; set; }
    }
}