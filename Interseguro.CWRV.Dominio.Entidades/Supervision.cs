using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Supervision
    {
        [DataMember]
        public int NumeroRegistro { get; set; }
        [DataMember]
        public string Jefe { get; set; }
        [DataMember]
        public string Supervisor { get; set; }
        [DataMember]
        public string Agente { get; set; }
        [DataMember]
        public string Usuario { get; set; }
        [DataMember]
        public DateTime FechaEvento { get; set; }
        [DataMember]
        public string Evento { get; set; }
        [DataMember]
        public string Detalle { get; set; }
    }
}
