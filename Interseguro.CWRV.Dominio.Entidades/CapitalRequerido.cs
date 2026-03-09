using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class CapitalRequerido
    {
        [DataMember]
        public string num_cuissp { get; set; }
        //[DataMember]
        //public string tipo_renta { get; set; }
        [DataMember]
        public TipoRenta TipoRenta { get; set; }
        [DataMember]
        public Moneda Moneda { get; set; }
        [DataMember]
        public string periodo_garantizado { get; set; }
        [DataMember]
        public Temporalidad Temporalidad { get; set; }
        //public string temporalidad { get; set; }
        [DataMember]
        public int num_vendedor { get; set; }
        [DataMember]
        public string id_grupo_familiar { get; set; }
        [DataMember]
        public double pension_requerida { get; set; }
        [DataMember]
        public double pension_calculada { get; set; }
        [DataMember]
        public double capital_requerido { get; set; }

        [DataMember]
        public int error { get; set; }

        [DataMember]
        public string simbolo_moneda_equi { get; set; }

        [DataMember]
        public double tasaSBS { get; set; }

        [DataMember]
        public double tipo_cambio { get; set; }



    }
}
