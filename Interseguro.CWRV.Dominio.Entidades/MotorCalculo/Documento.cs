using System;
using System.Collections.Generic;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class Documento
    {
        public string cod_tipo_temporalidad { get; set; }
        public string num_solicitud { get; set; }
        public Int64? num_correlativo { get; set; }
        public DateTime? fec_cotizacion { get; set; }
        public DateTime? fec_documento { get; set; }
        public DateTime? fec_devengue { get; set; }
        public DateTime? fec_inicio_vigencia { get; set; }
        public DateTime? fec_fin_vigencia { get; set; }
        public double? val_prima_unica { get; set; }
        public double? val_total_cic { get; set; }
        public string cod_moneda { get; set; }
        public double? val_tipo_cambio { get; set; }
        public double? val_pje_moneda { get; set; }
        public int? num_meses_temporalidad { get; set; }
        public int? num_meses_diferidos { get; set; }
        public int? num_meses_garantizados { get; set; }
        public int? num_meses_pagos_doble { get; set; }
        public double? val_pje_segundo_periodo { get; set; }
        public bool? ind_gratificacion { get; set; }
        public double? val_pje_dev { get; set; }
        public double? val_pje_dev_fallec { get; set; }
        public double? val_pje_fallec_ndeveng { get; set; }
        public bool? ind_sepelio { get; set; }
        public double? val_monto_sepelio { get; set; }
        public double? val_tasa_ret_accion { get; set; }
        public double? val_dcom { get; set; }
        public double? val_acom { get; set; }
        public int? cob_adic_dev { get; set; }
        public int? cob_adic_cap_fallec { get; set; }
        public double? val_tasa_venta_ash { get; set; }
        public double? val_tasa_costo_equiv { get; set; }
        public double? val_renta { get; set; }
        public double? val_renta_mo { get; set; }
        public double? val_fac_cia { get; set; }
        public double? val_mto_cia { get; set; }
        public double? val_tasa_int_vit { get; set; }
        public double? val_tasa_int_temp { get; set; }
        public double? val_descuento_comision { get; set; }
        public double? val_mto_cia_sin_comision { get; set; }
        public double? val_tasa_cesion { get; set; }
        public bool? ind_cotiza { get; set; }
        public int? num_error_cot { get; set; }
        public double? val_total_garantizado { get; set; }
        public double? val_1era_prima_is { get; set; }
        public double? val_fac_dev { get; set; }
        public double? val_mto_dev { get; set; }
        public double? val_tope_sepelio { get; set; }
        public Reserva reserva { get; set; }
        public Int64? cod_poliza { get; set; }
        public string gls_poliza { get; set; }
        public string gls_dig_poliza { get; set; }
        public DateTime? fec_emision_poliza { get; set; }
        public int? num_meses_primer_tramo { get; set; }
        public double? val_renta_base { get; set; }
        public double? val_renta_original { get; set; }
        public string gls_corta_producto { get; set; }
        public TipoPago TipoPago { get; set; }
        public Moneda MonedaPago { get; set; }
        public ParametroAsh parametro_ash { get; set; }
        public List<Beneficiario> beneficiarios { get; set; }
        public List<FactorDocumento> factores_documento { get; set; }
        public Respuesta respuesta { get; set; }
        public Plan plan { get; set; }
    }
}
