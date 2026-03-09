using System;
using System.Collections.Generic;

namespace Interseguro.CWRV.Dominio.Entidades.MotorCalculo
{
    [Serializable]
    public class Poliza
    {
        public Int64? cod_poliza { get; set; }
        public string gls_poliza { get; set; }
        public string gls_dig_poliza { get; set; }
        public string gls_solicitud { get; set; }
        public DateTime? fec_solicitud { get; set; }
        public DateTime? fec_devengue { get; set; }
        public bool? ind_fallecimiento { get; set; }
        public DateTime? fec_emision_poliza { get; set; }
        public DateTime? fec_inicio_vigencia { get; set; }
        public DateTime? fec_fin_vigencia { get; set; }
        public DateTime? fec_inicio_pago { get; set; }
        public long? num_cotizacion { get; set; }
        public double? val_prima_unica { get; set; }
        public int? num_meses_temporalidad { get; set; }
        public int? num_meses_garantizados { get; set; }
        public int? num_meses_primer_tramo { get; set; }
        public double? val_pje_renta_segundo_tramo { get; set; }
        public double? val_pje_conyuge { get; set; }
        public DateTime? fec_envio_poliza { get; set; }
        public int? num_meses_ajuste { get; set; }
        public double? val_pje_ajuste_moneda { get; set; }
        public double? val_pje_devolucion { get; set; }
        public bool? ind_sepelio { get; set; }
        public DateTime? fec_recaudacion { get; set; }
        public double? val_tasa_venta_is { get; set; }
        public double? val_tasa_venta { get; set; }
        public double? val_tasa_tra { get; set; }
        public double? val_dcom { get; set; }
        public double? val_renta_original { get; set; }
        public double? val_renta_base { get; set; }
        public double? val_renta_base_anterior { get; set; }
        public double? val_factor_ultimo { get; set; }
        public double? val_total_cic { get; set; }
        public double? val_monto_sepelio { get; set; }
        public double? val_tasa_costo_equiv { get; set; }
        public double? val_tc { get; set; }
        public DateTime? fec_facturacion { get; set; }
        public double? val_pje_comision { get; set; }
        public DateTime? fec_ini_pago_doble { get; set; }
        public DateTime? fec_fin_pago_doble { get; set; }
        public string cod_version { get; set; }
        public int? num_meses_diferidos { get; set; }
        public double? val_pje_devolucion_fallec { get; set; }
        public double? val_tipo_cambio { get; set; }
        public string gls_corta_producto { get; set; }
        public TipoPago TipoPago { get; set; }
        public Plan plan { get; set; }
        public Moneda MonedaPago { get; set; }
        public List<Beneficiario> beneficiarios { get; set; }
        public List<FactorDocumento> factores_documento { get; set; }
    }
}