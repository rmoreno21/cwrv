using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class EstructuraCntoCamposDinamicos
    {
        [DataMember]
        public string etiqueta { get; set; }
        [DataMember]
        public string valor { get; set; }
        [DataMember]
        public string icono { get; set; }
        [DataMember]
        public bool visible { get; set; }
    }

}
