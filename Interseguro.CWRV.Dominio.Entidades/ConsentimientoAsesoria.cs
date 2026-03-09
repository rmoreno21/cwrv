using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ConsentimientoAsesoria
    {
        [DataMember]
        public int IdConsentimientoAsesoria { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public string CodTipoIdentificacion { get; set; }
        [DataMember]
        public string NroIdentificacion { get; set; }
        [DataMember]
        public string CodProducto { get; set; }
        [DataMember]
        public string Consentimiento { get; set; }
        [DataMember]
        public DateTime FecUltimoConsentimiento { get; set; }
        [DataMember]
        public int idContactoAsesoria { get; set; }
        [DataMember]
        public string tokenADN { get; set; }
        [DataMember]
        public string usuario { get; set; }
        
    }
}
