using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Seguimiento
    {
        [DataMember]
        public string Jefe { get; set; }
        [DataMember]
        public string Supervisor { get; set; }
        [DataMember]
        public string Agente { get; set; }
        [DataMember]
        public string CUSPP { get; set; }
        [DataMember]
        public string Persona { get; set; }
        [DataMember]
        public string SitioGenerado { get; set; }
        [DataMember]
        public string Telefono { get; set; }
        [DataMember]
        public double SaldoCIC { get; set; }
        [DataMember]
        public string NroSolicitud { get; set; }
        [DataMember]
        public string Categoria { get; set; }
        [DataMember]
        public DateTime FechaIngreso { get; set; }
        [DataMember]
        public DateTime FechaCierre { get; set; }
        [DataMember]
        public string Companhia { get; set; }
    }
}
