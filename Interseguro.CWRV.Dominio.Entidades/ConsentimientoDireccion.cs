using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class ConsentimientoDireccion
    {
        //[DataMember]
        //public int id_direccion { get; set; }
        [DataMember]
        public string gls_direccion { get; set; }
        [DataMember]
        public string cod_tipo_via { get; set; }
        [DataMember]
        public string gls_espacio_urbano { get; set; }
        [DataMember]
        public string cod_departamento { get; set; }
        [DataMember]
        public string cod_provincia { get; set; }
        [DataMember]
        public string cod_distrito { get; set; }
    }

}
