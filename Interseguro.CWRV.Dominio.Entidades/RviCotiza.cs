using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Interseguro.CWRV.Dominio.Entidades
{
    [Serializable]
    [DataContract]
    public class RviCotiza
    {
        [DataMember]
        public string num_solicitud;
        [DataMember]
        public string fec_cotizacion;
        [DataMember]
        public string num_correlativo;
        [DataMember]
        public string cod_tipo_cotizacion;
        [DataMember]
        public string cod_estado_cotizacion;
        [DataMember]
        public string cod_moneda;
        [DataMember]
        public string fec_valorizacion;
        [DataMember]
        public string val_moneda;
        [DataMember]
        public string cod_tipo_producto;
        [DataMember]
        public string cod_modalidad;
        [DataMember]
        public string ind_modalidad;
        [DataMember]
        public string val_per_garantizado;
        [DataMember]
        public string val_per_temporal;
        [DataMember]
        public string val_pje_rent_temp;
        [DataMember]
        public string val_fac_afp;
        [DataMember]
        public string val_mto_afp;
        [DataMember]
        public string val_pen_afp;
        [DataMember]
        public string val_fac_cia;
        [DataMember]
        public string val_mto_cia;
        [DataMember]
        public string val_pen_cia;
        [DataMember]
        public string val_pen_cia_mo;
        [DataMember]
        public string val_tasa_int_vit;
        [DataMember]
        public string val_tasa_int_temp;
        [DataMember]
        public string val_tasa_ret_accion;
        [DataMember]
        public string val_descuento_comision;
        [DataMember]
        public string fec_cierre;
        [DataMember]
        public string val_tasa_cia;
        [DataMember]
        public string num_cotizacion_cierre;
        [DataMember]
        public string cod_tipo_calculo;
        [DataMember]
        public string ind_cotizacion_seleccionada;
        [DataMember]
        public string val_pen_ref;
        [DataMember]
        public string val_pen_ref_mo;
        [DataMember]
        public string fec_interna;
        [DataMember]
        public string ind_aporte_adicional;
        [DataMember]
        public string val_mto_cia_sin_comision;
        [DataMember]
        public string ind_derecho_crecer;
        [DataMember]
        public string ind_gratificacion;
        [DataMember]
        public string val_tasa_cesion;
        [DataMember]
        public string cod_particion_capital;
        [DataMember]
        public string val_afp_pen_ref;
        [DataMember]
        public string ind_cotiza;
        [DataMember]
        public string val_tasa_cesion_moneda2;
        [DataMember]
        public string val_tasa_venta_ash;
        [DataMember]
        public string val_tasa_costo_equiv;
        [DataMember]
        public string val_duration;
        [DataMember]
        public string num_error_cot;
        [DataMember]
        public string val_tasa_venta_ash_2;
        [DataMember]
        public string val_tasa_ret_accion_2;
        [DataMember]
        public string val_tasa_costo_equiv_2;
        [DataMember]
        public string val_duration_2;
        [DataMember]
        public string num_error_cot_2;
        [DataMember]
        public double val_total_garantizado;

        //<SOLINI25781>
        [DataMember]
        public double val_1era_prima_is;
        //<SOLFIN25781>

        //<GTIINI-6489>
        public double val_mto_gasto_comision;
        //<GTIFIN-6489>

        //<INIGTI_753>
        [DataMember]
        public double val_fac_dev;

        [DataMember]
        public double val_mto_dev;
        //<FINGTI_753>

        //<INIGTI_4081>
        [DataMember]
        public double val_tasa_ajuste_tra;
        //<FINGTI_4081>
    }
}
