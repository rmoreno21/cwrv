using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [DataContract]
    public class ReporteIndicadores
    {
        [DataMember]
        public string rutaPlantilla { get; set; }
        [DataMember]
        public string celdaEscritura { get; set; }
        [DataMember]
        public string nombreArchivo { get; set; }
        [DataMember]
        public List<object> listaDatos { get; set; }
        [DataMember]
        public string rutaGenerar { get; set; }
    }
}
