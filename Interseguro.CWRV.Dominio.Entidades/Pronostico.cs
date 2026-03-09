using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    public class Pronostico
    {

        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Nombre { get; set; }

    }
}
