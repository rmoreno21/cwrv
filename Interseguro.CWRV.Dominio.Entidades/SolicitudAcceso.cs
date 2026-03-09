using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class SolicitudAcceso
    {
        [DataMember]
        public Int64 Codigo { get; set; }
        [DataMember]
        public string Usuario { get; set; }
        [DataMember]
        public string CUSPP { get; set; }
        [DataMember]
        public string Token { get; set; }
        [DataMember]
        public int ExpiracionToken { get; set; }
        [DataMember]
        public DateTime FechaConsulta { get; set; }
        [DataMember]
        public DateTime FechaExpiracion { get; set; }
        [DataMember]
        public bool Vigente { get; set; }
        [DataMember]
        public string IP { get; set; }
        
    }
}
