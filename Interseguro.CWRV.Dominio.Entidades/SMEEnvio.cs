using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class SMEEnvio
    {
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string NumeroPoliza { get; set; }
        [DataMember]
        public string NumeroDocumento { get; set; }
        [DataMember]
        public string Destinatario { get; set; }
        [DataMember]
        public string ProcesoSme { get; set; }
        [DataMember]
        public string RutaPdf { get; set; }
        [DataMember]
        public string Contrasenia { get; set; }
        [DataMember]
        public object CamposDinamicos { get; set; }
        [DataMember]
        public string CamposDinamicosSerializados { get; set; }
    }
}
