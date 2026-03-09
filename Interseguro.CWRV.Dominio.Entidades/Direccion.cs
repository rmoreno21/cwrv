using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class Direccion
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Glosa { get; set; }
        [DataMember]
        public Comuna Comuna { get; set; }
        [DataMember]
        public Ciudad Ciudad { get; set; }
        [DataMember]
        public Departamento Departamento { get; set; }
        [DataMember]
        public DateTime FechaIngreso { get; set; }
        [DataMember]
        public bool Principal { get; set; }
        [DataMember]
        public Afiliado Afiliado { get; set; }
        [DataMember]
        public Usuario Usuario { get; set; }

        [DataMember]
        public string EspacioUrbano { get; set; }
        [DataMember]
        public Parametro TipoVia { get; set; }

        [DataMember]
        public bool Vigencia { get; set; }

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
