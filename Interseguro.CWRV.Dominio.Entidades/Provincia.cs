using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class Provincia
    {
        [DataMember]
        public string id_provincia { get; set; }
        [DataMember]
        public string id_departamento { get; set; }
        [DataMember]
        public string gls_provincia { get; set; }
        [DataMember]
        public string cod_provincia { get; set; }
        [DataMember]
        public string cod_ubigeo { get; set; }
    }
}
