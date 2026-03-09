using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class AporteAdicional
    {
        [DataMember]
        public string num_cuispp { get; set; }

        [DataMember]
        public DateTime fec_pagoapad { get; set; }

        [DataMember]
        public double val_pension_referencia { get; set; }

        [DataMember]
        public string cod_moneda_pension_ref { get; set; }

        [DataMember]
        public double val_tasa_aporte { get; set; }

        [DataMember]
        public double val_monto_aporte { get; set; }

        [DataMember]
        public double val_pension_a_pago { get; set; }

        [DataMember]
        public string cod_moneda_pension_a_pago { get; set; }

        [DataMember]
        public double val_pension_elegida { get; set; }

        [DataMember]
        public string ind_vigencia { get; set; }

        [DataMember]
        public string aud_usr_ingreso { get; set; }

        [DataMember]
        public DateTime aud_fec_ingreo { get; set; }

        [DataMember]
        public string aud_usr_modificacion { get; set; }

        [DataMember]
        public DateTime aud_fec_modificacion { get; set; }
    }
}
