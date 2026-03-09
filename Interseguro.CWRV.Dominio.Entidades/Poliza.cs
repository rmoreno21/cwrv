using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Poliza
    {
        [DataMember]
        public int NumPoliza { get; set; }
        [DataMember]
        public string DigPoliza { get; set; }
        [DataMember]
        public DateTime FecEmision { get; set; }
        [DataMember]
        public DateTime FecPago { get; set; }
        [DataMember]
        public DateTime FecInicioVigencia { get; set; }
        [DataMember]
        public DateTime FecFinVigencia { get; set; }

        [DataMember]
        public DateTime FecInicioPagoDoble { get; set; }
        [DataMember]
        public DateTime FecFinPagoDoble { get; set; }

        [DataMember]
        public double ValPrimaNeta { get; set; }
        [DataMember]
        public double ValIva { get; set; }
        [DataMember]
        public double ValPrimaBruta { get; set; }

        [DataMember]
        public int val_meses_periodo { get; set; }

        [DataMember]
        public bool ind_rescate { get; set; }
    }
}
