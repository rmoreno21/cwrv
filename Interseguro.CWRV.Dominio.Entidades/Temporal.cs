using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
   
    [Serializable]
    [DataContract]
    public class Temporal
    {

        [DataMember]
        public string cod_tabla { get; set; }
        [DataMember]
        public int correlativo { get; set; }
        [DataMember]
        public string cod_parametro { get; set; }
        [DataMember]
        public string gls_parametro { get; set; }

        [DataMember]
        public int item { get; set; }
        [DataMember]
        public string codigo { get; set; }
        [DataMember]
        public int cantidad { get; set; }

    }

}
