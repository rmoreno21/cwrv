//<SRIINI06326>
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class PorcentajeComision
    {
        [DataMember]
        public string Codigo { get; set; }
        [DataMember]
        public string Glosa { get; set; }
        [DataMember]
        public double ValorAdicional { get; set; }
    }
}
//<SRIFIN06326>