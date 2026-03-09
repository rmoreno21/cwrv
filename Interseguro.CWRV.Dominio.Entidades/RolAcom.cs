using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class RolAcom
    {
        //<SRIINI10693>
        //Se creo la entidad ROLACOM para su implementacion
        //<SRIFIN10693>

        [DataMember]
        public double NumRangoIni { get; set; }
        [DataMember]
        public double NumRangoFin { get; set; }
        [DataMember]
        public string CodRol { get; set; }

        [DataMember]
        public string CodEscenario { get; set; }
        [DataMember]
        public string GlsEscenario { get; set; }

        [DataMember]
        public double ValorAcom { get; set; }

        [DataMember]
        public string NumSolicitud { get; set; }
        [DataMember]
        public DateTime FechaCotizacion { get; set; }
        [DataMember]
        public double? ValorAcomMaximo { get; set; }
    }
}
