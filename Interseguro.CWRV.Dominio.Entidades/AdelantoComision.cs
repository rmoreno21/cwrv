//<SRIINI06326>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class AdelantoComision
    {
        [DataMember]
        public decimal RangoInicial { get; set; }
        [DataMember]
        public decimal RangoFinal { get; set; }
        [DataMember]
        public AFP Afp { get; set; }
        [DataMember]
        public string CodigoTipoPension { get; set; }
        [DataMember]
        public string IndReja { get; set; }
        [DataMember]
        public double ValAcom { get; set; }
        [DataMember]
        public DateTime InicioVigencia { get; set; }
        [DataMember]
        public DateTime TerminoVigencia { get; set; }

        // Propiedades Auxiliares
        [DataMember]
        public decimal MontoCIC { get; set; }
        [DataMember]
        public DateTime FechaCotizacion { get; set; }
    }
}
//<SRIFIN06326>