using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class GraficoLineal
    {
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public double Valor { get; set; }
    }
}
