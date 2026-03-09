using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Identificacion
    {
        [DataMember]
        public string IdTipo { get; set; }
        [DataMember]
        public string GlosaTipo { get; set; }
        [DataMember]
        public string Numero { get; set; }
    }
}
