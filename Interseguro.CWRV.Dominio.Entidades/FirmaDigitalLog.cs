using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class FirmaDigitalLog
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdFirmaDigital { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string NumeroSolicitud { get; set; }
        [DataMember]
        public int Item { get; set; }
        [DataMember]
        public string IP { get; set; }
        [DataMember]
        public string BrowserAgent { get; set; }
        [DataMember]
        public DateTime FechaConsentimiento { get; set; }
        [DataMember]
        public char IndConsentimiento { get; set; }
        [DataMember]
        public DateTime? FechaPrimerEnvio { get; set; }
        [DataMember]
        public DateTime? FechaUltimoEnvio { get; set; }
    }
}
