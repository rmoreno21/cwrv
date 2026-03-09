using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class BeneficiarioDireccion
    {
        [DataMember]
        public int idDireccion { get; set; }
        [DataMember]
        public string numSolicitud { get; set; }
        [DataMember]
        public string direccion { get; set; }
        [DataMember]
        public Parametro tipoVia { get; set; }
        [DataMember]
        public string espacioUrbano { get; set; }
        [DataMember]
        public Departamento departamento { get; set; }
        [DataMember]
        public Ciudad provincia { get; set; }
        [DataMember]
        public Comuna distrito { get; set; }
        [DataMember]
        public string personaAutorizada { get; set; }
        [DataMember]
        public string tipo { get; set; }
        [DataMember]
        public string vigencia { get; set; }
        [DataMember]
        public string usuario { get; set; }
        //<GTI.59048-INI>
        [DataMember]
        public string numeroTelefono { get; set; }
        [DataMember]
        public string nombreVia { get; set; }
        [DataMember]
        public string numeroVia { get; set; }
        [DataMember]
        public string numeroInterior { get; set; }
        [DataMember]
        public Parametro tipoZona { get; set; }
        [DataMember]
        public string nombreZona { get; set; }
        [DataMember]
        public string referencia { get; set; }
        [DataMember]
        public string numeroDepartamento { get; set; }
        [DataMember]
        public string manzana { get; set; }
        [DataMember]
        public string numeroLote { get; set; }
        [DataMember]
        public string kilometro { get; set; }
        [DataMember]
        public string block { get; set; }
        [DataMember]
        public string etapa { get; set; }
        [DataMember]
        public Parametro largaDistancia { get; set; }
        //<GTI.59048-FIN>
    }
}
