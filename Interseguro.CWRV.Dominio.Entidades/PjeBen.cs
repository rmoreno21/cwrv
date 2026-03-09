using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class PjeBen
    {
        [DataMember]
        public int Correlativo { get; set; }
        [DataMember]
        public string TipoProducto { get; set; }
        [DataMember]
        public double PorcentajeBase { get; set; }
        [DataMember]
        public double PorcentajeModificado { get; set; }
        [DataMember]
        public double PorcentajeRetPeriodoDiferido { get; set; }
        [DataMember]
        public Int64 NumeroCorrelativoCotizacion { get; set; }
    }
}
