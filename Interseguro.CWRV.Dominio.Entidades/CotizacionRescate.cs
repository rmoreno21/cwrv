using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CotizacionRescate
    {
        [DataMember]
        public int mes_rescate { get; set; }
        [DataMember]
        public double tasa_venta { get; set; }
        [DataMember]
        public double spread { get; set; }
        [DataMember]
        public double tasa_rescate_anual { get; set; }
        [DataMember]
        public double tasa_rescate_mensual { get; set; }
        [DataMember]
        public double valor_rescate { get; set; }
        [DataMember]
        public double valor_renta_mensual { get; set; }
    }

}
