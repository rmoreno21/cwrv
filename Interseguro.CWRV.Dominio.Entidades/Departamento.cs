using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Departamento
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public string id_departamento { get; set; }
        [DataMember]
        public string gls_departamento { get; set; }
        [DataMember]
        public string cod_departamento { get; set; }
        [DataMember]
        public string cod_ubigeo { get; set; }
    }
}
