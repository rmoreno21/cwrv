using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CotizacionRP
    {
        [DataMember]
        public Int64 Correlativo { get; set; }
        [DataMember]
        public Moneda Moneda { get; set; }
        [DataMember]
        public Producto Producto { get; set; }
        [DataMember]
        public Modalidad Modalidad { get; set; }
        [DataMember]
        public int PeriodoGarantizado { get; set; }
        [DataMember]
        public double? AjusteTRA { get; set; }
        
        [DataMember]
        public double PensionCia { get; set; }
        [DataMember]
        public double PensionCiaMO { get; set; }
        [DataMember]
        public double PuurCia { get; set; }
        [DataMember]
        public double TasaVenta { get; set; }
        [DataMember]
        public double TasaVentaSbs { get; set; }
        [DataMember]
        public double TasaRetornoAccionista { get; set; }
    }
}
