using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Anticipo
    {
        [DataMember]
        public Solicitud Solicitud { get; set; }
        [DataMember]
        public double Monto { get; set; }
        [DataMember]
        public string Estado { get; set; }
        [DataMember]
        public Agente Agente { get; set; }
        [DataMember]
        public Agente Supervisor { get; set; }
        [DataMember]
        public Afiliado Afiliado { get; set; }

        [DataMember]
        public double MontoMaximo { get; set; }
        [DataMember]
        public int MesesIngreso { get; set; }
        [DataMember]
        public int DiasDevolucion { get; set; }

        [DataMember]
        public DateTime FechaAceptacion { get; set; }

        [DataMember]
        public Usuario Usuario { get; set; }
    }
}
