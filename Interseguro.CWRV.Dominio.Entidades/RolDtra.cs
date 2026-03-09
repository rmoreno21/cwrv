using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class RolDtra
    {
        [DataMember]
        public DateTime FechaCotizacion { get; set; }
        [DataMember]
        public double RangoInicial { get; set; }
        [DataMember]
        public double RangoFinal { get; set; }
        [DataMember]
        public string RolAzman { get; set; }
    }
}
