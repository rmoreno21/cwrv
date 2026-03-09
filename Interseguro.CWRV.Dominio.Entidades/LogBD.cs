using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class LogBD
    {
        [DataMember]
        public string IdAplicacion { get; set; }
        [DataMember]
        public string NombreTerminal { get; set; }
        [DataMember]
        public string IP { get; set; }
        [DataMember]
        public string NombreUsuario { get; set; }
        [DataMember]
        public DateTime? FechaEvento { get; set; }
        [DataMember]
        public string IdTipoEvento { get; set; }
        [DataMember]
        public string Detalle { get; set; }
    }
}
