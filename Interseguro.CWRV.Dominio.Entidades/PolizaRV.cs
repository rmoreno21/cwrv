using System;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class PolizaRV
    {
        [DataMember]
        public int num_poliza { get; set; }
        [DataMember]
        public string cod_tipo_identificacion { get; set; }
        [DataMember]
        public int num_identificacion { get; set; }
        [DataMember]
        public string cod_tipo_pension { get; set; }
        [DataMember]
        public string cod_tipo_producto { get; set; }
        [DataMember]
        public string cod_modalidad { get; set; }
        [DataMember]
        public string ind_modalidad { get; set; }
        [DataMember]
        public string ind_fallecimiento { get; set; }
        [DataMember]
        public DateTime fec_devengue { get; set; }
        [DataMember]
        public string cod_moneda { get; set; }
        [DataMember]
        public double val_pen_ref_mo { get; set; }
        [DataMember]
        public string num_cuispp { get; set; }
        [DataMember]
        public string cod_afp { get; set; }

        [DataMember]
        public int num_vendedor { get; set; }
        [DataMember]
        public string cod_origen_vta { get; set; }
        [DataMember]
        public int val_per_garantizado { get; set; }
        [DataMember]
        public int val_per_temporal { get; set; }
        [DataMember]
        public double val_mto_cia { get; set; }

        [DataMember]
        public string dig_poliza { get; set; }
        [DataMember]
        public int num_cotizacion_cierre { get; set; }
        [DataMember]
        public double val_pje_rent_temp { get; set; }
        [DataMember]
        public string cod_tipo_renta { get; set; }
        [DataMember]
        public DateTime fec_emision_poliza { get; set; }
        [DataMember]
        public string ind_tiene_cobertura { get; set; }
        [DataMember]
        public string ind_derecho_crecer { get; set; }
        [DataMember]
        public string ind_gratificacion { get; set; }
        [DataMember]
        public DateTime fec_sol_pension { get; set; }
        [DataMember]
        public DateTime fec_recepcion { get; set; }
        [DataMember]
        public string cod_categoria { get; set; }
        [DataMember]
        public string cod_cia_seguro { get; set; }
        [DataMember]
        public double val_mto_cta_individual { get; set; }
        [DataMember]
        public string cod_reajuste_pension_1 { get; set; }
        [DataMember]
        public double val_par_reapen_1 { get; set; }
        [DataMember]
        public double val_par_reapen_2 { get; set; }
        [DataMember]
        public string cod_reajuste_pension_2 { get; set; }
    }
}
