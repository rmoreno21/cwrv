using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class Distrito
    {
        [DataMember]
        public string id_distrito { get; set; }
        [DataMember]
        public string id_provincia { get; set; }
        [DataMember]
        public string id_departamento { get; set; }
        [DataMember]
        public string gls_distrito { get; set; }
        [DataMember]
        public string cod_distrito { get; set; }
        [DataMember]
        public string cod_ubigeo { get; set; }
    }
}
