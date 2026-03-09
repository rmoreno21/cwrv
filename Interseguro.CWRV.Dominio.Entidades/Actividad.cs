using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Actividad
    {
        [DataMember]
        public string Correlativo { get; set; }
        [DataMember]
        public string TipoEvento { get; set; }
        [DataMember]
        public string GlosaEvento { get; set; }
        [DataMember]
        public DateTime? FechaEvento { get; set; }
        [DataMember]
        public string Resultado { get; set; }
        [DataMember]
        public string Comentario { get; set; }
        [DataMember]
        public string Username { get; set; }
    }
}
