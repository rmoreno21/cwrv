using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CoberturaAdicional
    {
        [DataMember]
        public string Parentesco { get; set; }

        [DataMember]
        public string FechaNacimiento { get; set; }

        [DataMember]
        public string Sexo { get; set; }
    }
}