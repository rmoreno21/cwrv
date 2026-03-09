using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class RespuestaPlaft
    {
        [DataMember]
        public string Estado { get; set; }

        [DataMember]
        public string Mensaje { get; set; }
    }
}
