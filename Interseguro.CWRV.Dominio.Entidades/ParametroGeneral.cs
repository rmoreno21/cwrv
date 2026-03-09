using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{

    [Serializable]
    [DataContract]
    public class ParametroGeneral
    {
        [DataMember]
        public string Codigo { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
    }

}
