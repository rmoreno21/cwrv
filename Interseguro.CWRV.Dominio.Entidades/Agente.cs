using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Agente
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string IdCartera { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string IdPadre { get; set; }
        [DataMember]
        public int IdNivel { get; set; }
        [DataMember]
        public string IdAgencia { get; set; }
        [DataMember]
        public string IdContrato { get; set; }
        [DataMember]
        public DateTime FechaIngreso { get; set; }
        [DataMember]
        public Departamento Departamento { get; set; }
        [DataMember]
        public string IdUbicacionGeografica { get; set; }
        [DataMember]
        public string Usuario { get; set; }
        [DataMember]
        public double MS1 { get; set; }
        [DataMember]
        public double MS6 { get; set; }

        [DataMember]
        public double ValImporteDocumento { get; set; }
        [DataMember]
        public double ValImporteDevolucion { get; set; }
        [DataMember]
        public string CodTipoDocumento { get; set; }
        [DataMember]
        public string NumDocumento { get; set; }
        [DataMember]
        public string CodCiaSbs { get; set; }

        [DataMember]
        public string CorreoElectronico { get; set; }

        [DataMember]
        public string ApellidoPaterno { get; set; }
        [DataMember]
        public string ApellidoMaterno { get; set; }
        [DataMember]
        public string NombrePersona { get; set; }

        [DataMember]
        public bool Vigente { get; set; }
        [DataMember]
        public DateTime InicioVigencia { get; set; }
        [DataMember]
        public DateTime FinVigencia { get; set; }
    }
}
