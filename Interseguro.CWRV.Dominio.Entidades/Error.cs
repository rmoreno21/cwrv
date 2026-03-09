using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class Error
    {
        [DataMember]
        public int Codigo { get; set; }
        [DataMember]
        public string Mensaje { get; set; }
    }
}
