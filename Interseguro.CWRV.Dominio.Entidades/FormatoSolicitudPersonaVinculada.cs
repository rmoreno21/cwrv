using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    public class FormatoSolicitudPersonaVinculada
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int IdFormatoSolicitud { get; set; }
        [DataMember]
        public int Correlativo { get; set; }
        [DataMember]
        public string NumeroSolicitud { get; set; }
        [DataMember]
        public int Item { get; set; }
        [DataMember]
        public string Nombres { get; set; }
        [DataMember]
        public string ApellidoPaterno { get; set; }
        [DataMember]
        public string ApellidoMaterno { get; set; }
        [DataMember]
        public Parentesco Parentesco { get; set; }
        [DataMember]
        public Identificacion Identificacion { get; set; }
    }
}
