using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Interseguro.CWRV.Dominio.Entidades
{

    [DataContract]
    [Serializable]
    public class ParametrosMotorIFP
    {

        [DataMember]
        public string cod_monedaIFP { get; set; }

        [DataMember]
        public string cod_tipo_temporalidadIFP { get; set; }

        [DataMember]
        public DateTime fec_cotizacionIFP { get; set; }

        [DataMember]
        public CotizacionMotorIFP cotizacion { get; set; }

        [DataMember]
        public List<TablaMortalidadMotorIFP> tablas_mortalidad { get; set; }

        [DataMember]
        public List<FactorMejoraMotorIFP> factores_mejoras { get; set; }

        [DataMember]
        public List<InformacionFactorMejoraMotorIFP> informacion_factores_mejoras { get; set; }

        //[DataMember]
        //public List<TablaIcobMotorIFP> icob_nominales { get; set; }

        //[DataMember]
        //public List<TablaIcobMotorIFP> icob_ajustados { get; set; }

        //[DataMember]
        //public List<InversionTemporalMotorIFP> inversiones_temporales { get; set; }

        //[DataMember]
        //public List<InversionVitaliciaMotorIFP> inversiones_vitalicias { get; set; }

        [DataMember]
        public List<FactorAntiSeleccionMotorIFP> factores_anti_seleccion { get; set; }

        [DataMember]
        public List<FactorPUMIMotorIFP> factores_pumi { get; set; }

        [DataMember]
        public List<RendimientosInversionTemporalMotorIFP> rendimientos_inversiones_temporales { get; set; }

        [DataMember]
        public List<TasaDurationMotorIFP> tasa_duration { get; set; }

        [DataMember]
        public List<SpreadIFP> spread_rescate { get; set; }

        [DataMember]
        public List<TablaVTD> lista_VTD { get; set; }
        [DataMember]
        public List<TablaVOLA> lista_VOLA { get; set; }

        [DataMember]
        public bool isLogCotizacion { get; set; }
        [DataMember]
        public bool isLogReserva { get; set; }

        [DataMember]
        public string tipo_calculo { get; set; }
        [DataMember]
        public string tipo_producto { get; set; }

    }

    [DataContract]
    [Serializable]
    public class CotizacionMotorIFP
    {
        /*(INI) Campos de INPUT*/
        [DataMember]
        public string num_solicitud { get; set; }

        //public string cod_plan { get; set; }
        [DataMember]
        public Plan plan { get; set; }

        [DataMember]
        public Int64 num_correlativo { get; set; }

        [DataMember]
        public DateTime fec_cotizacion { get; set; }

        [DataMember]
        public DateTime? fec_documento { get; set; }

        [DataMember]
        public DateTime fec_devengue { get; set; }

        [DataMember]
        public DateTime fec_inicio_vigencia { get; set; }

        [DataMember]
        public DateTime fec_fin_vigencia { get; set; }

        [DataMember]
        public double val_prima_unica { get; set; }

        [DataMember]
        public double val_total_cic { get; set; }

        [DataMember]
        public string cod_moneda { get; set; }

        [DataMember]
        public double val_tipo_cambio { get; set; }

        [DataMember]
        public double val_pje_moneda { get; set; }

        [DataMember]
        public int num_meses_temporalidad { get; set; }

        [DataMember]
        public int num_meses_diferidos { get; set; }

        [DataMember]
        public int num_meses_garantizados { get; set; }

        [DataMember]
        public int num_meses_pagos_doble { get; set; }

        [DataMember]
        public double val_pje_segundo_periodo { get; set; }

        [DataMember]
        public bool ind_gratificacion { get; set; }

        [DataMember]
        public double val_pje_dev { get; set; }

        [DataMember]
        public double val_pje_dev_fallec { get; set; }

        [DataMember]
        public double val_pje_fallec_ndeveng { get; set; }

        [DataMember]
        public bool ind_sepelio { get; set; }

        [DataMember]
        public double val_monto_sepelio { get; set; }

        [DataMember]
        public double val_tasa_ret_accion { get; set; }

        [DataMember]
        public double val_dcom { get; set; }

        [DataMember]
        public double val_acom { get; set; }

        [DataMember]
        public int cob_adic_dev { get; set; }

        [DataMember]
        public int cob_adic_cap_fallec { get; set; }

        [DataMember]
        public string cod_tipo_temporalidad { get; set; }

        [DataMember]
        public List<BeneficiarioMotorIFP> beneficiarios { get; set; }

        //[DataMember]
        //public List<FactorCotizacionMotorIFP> factores_documento { get; set; }

        [DataMember]
        public ParametroAshMotorIFP parametro_ash { get; set; }

        /*(FIN) Campos de INPUT*/

        /*(INI) Campos de OUTPUT*/
        [DataMember]
        public double val_tasa_venta_ash { get; set; }

        [DataMember]
        public double val_tasa_costo_equiv { get; set; }

        [DataMember]
        public double val_renta { get; set; }

        [DataMember]
        public double val_renta_mo { get; set; }

        [DataMember]
        public double val_fac_cia { get; set; }

        [DataMember]
        public double val_mto_cia { get; set; }

        [DataMember]
        public double val_tasa_int_vit { get; set; }

        [DataMember]
        public double val_tasa_int_temp { get; set; }

        [DataMember]
        public double val_descuento_comision { get; set; }

        [DataMember]
        public double val_mto_cia_sin_comision { get; set; }

        [DataMember]
        public double val_tasa_cesion { get; set; }

        [DataMember]
        public bool ind_cotiza { get; set; }

        [DataMember]
        public int num_error_cot { get; set; }

        [DataMember]
        public double val_total_garantizado { get; set; }

        //[DataMember]
        //public double val_tasa_ajuste_tra { get; set; }

        [DataMember]
        public double val_1era_prima_is { get; set; }

        [DataMember]
        public double val_fac_dev { get; set; }

        [DataMember]
        public double val_mto_dev { get; set; }

        /*(FIN) Campos de OUTPUT*/

        [DataMember]
        public Respuesta respuesta { get; set; }

        [DataMember]
        public ReservaMotorIFP reserva { get; set; }

        [DataMember]
        public double val_para_duration { get; set; }

        [DataMember]
        public double? val_renta_sin_diferimiento { get; set; }

        [DataMember]
        public string cod_estado_cotizacion { get; set; }

        [DataMember]
        public List<CotizacionRescate> lstRescate { get; set; }

        [DataMember]
        public List<string> lstInput { get; set; }
        [DataMember]
        public List<string> lstOutput { get; set; }        
    }

    [DataContract]
    [Serializable]
    public class TablaMortalidadMotorIFP
    {
        [DataMember]
        public int num_edad_mes { get; set; }
        [DataMember]
        public double val_lx_mbh { get; set; }
        [DataMember]
        public double val_lx_mbm { get; set; }
        [DataMember]
        public double val_lx_mih { get; set; }
        [DataMember]
        public double val_lx_mim { get; set; }
        [DataMember]
        public double val_lx_mvh { get; set; }
        [DataMember]
        public double val_lx_mvm { get; set; }
        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
        [DataMember]
        public DateTime fec_ini_vig_tdm { get; set; }
        [DataMember]
        public DateTime fec_fin_vig_tdm { get; set; }
    }

    [DataContract]
    [Serializable]
    public class FactorMejoraMotorIFP
    {
        [DataMember]
        public int num_edad_mes { get; set; }
        [DataMember]
        public double val_lx_mbh { get; set; }
        [DataMember]
        public double val_lx_mbm { get; set; }
        [DataMember]
        public double val_lx_mih { get; set; }
        [DataMember]
        public double val_lx_mim { get; set; }
        [DataMember]
        public double val_lx_mvh { get; set; }
        [DataMember]
        public double val_lx_mvm { get; set; }
        [DataMember]
        public DateTime fec_ini_vig_tdm { get; set; }
        [DataMember]
        public DateTime fec_fin_vig_tdm { get; set; }
    }

    [DataContract]
    [Serializable]
    public class InformacionFactorMejoraMotorIFP
    {
        [DataMember]
        public string val_tipo_beneficiario { get; set; }
        [DataMember]
        public int num_anio_factor { get; set; }
    }

    [DataContract]
    [Serializable]
    public class TablaIcobMotorIFP
    {
        [DataMember]
        public double num_indi { get; set; }

        [DataMember]
        public double num_imon { get; set; }

        [DataMember]
        public double val_icop { get; set; }

        [DataMember]
        public double val_lcop { get; set; }

        [DataMember]
        public string cod_tipo_temporalidad { get; set; }

        [DataMember]
        public string cod_moneda { get; set; }

    }

    [DataContract]
    [Serializable]
    public class InversionTemporalMotorIFP
    {
        [DataMember]
        public string cod_moneda { get; set; }

        [DataMember]
        public int num_instrumento { get; set; }

        [DataMember]
        public int num_periodo { get; set; }

        [DataMember]
        public double valc { get; set; }

        [DataMember]
        public double porc { get; set; }

        [DataMember]
        public double tirc { get; set; }

        [DataMember]
        public double tasa_inv { get; set; }

        [DataMember]
        public bool ind_devolucion { get; set; }

        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
    }

    [DataContract]
    [Serializable]
    public class InversionVitaliciaMotorIFP
    {
        [DataMember]
        public string cod_moneda { get; set; }

        [DataMember]
        public int num_nins { get; set; }

        [DataMember]
        public int num_nflu { get; set; }

        [DataMember]
        public double val_venc { get; set; }

        [DataMember]
        public bool ind_devolucion { get; set; }

        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
    }

    [DataContract]
    [Serializable]
    public class FactorAntiSeleccionMotorIFP
    {
        [DataMember]
        public string cod_sexo { get; set; }
        [DataMember]
        public double val_tope { get; set; }
        [DataMember]
        public double pje_ajuste { get; set; }
        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
        [DataMember]
        public DateTime fec_ini_vigencia { get; set; }
        [DataMember]
        public DateTime fec_fin_vigencia { get; set; }
    }

    [DataContract]
    [Serializable]
    public class FactorPUMIMotorIFP
    {
        [DataMember]
        public string cod_sexo { get; set; }
        [DataMember]
        public double val_tope { get; set; }
        [DataMember]
        public double pje_ajuste { get; set; }
        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
        [DataMember]
        public DateTime fec_ini_vigencia { get; set; }
        [DataMember]
        public DateTime fec_fin_vigencia { get; set; }
    }

    [DataContract]
    [Serializable]
    public class RendimientosInversionTemporalMotorIFP
    {
        [DataMember]
        public string cod_moneda { get; set; }

        [DataMember]
        public int num_periodo { get; set; }

        [DataMember]
        public double val_parametro { get; set; }
    }

    [DataContract]
    [Serializable]
    public class TasaDurationMotorIFP
    {
        [DataMember]
        public string cod_parametro { get; set; }

        [DataMember]
        public double val_parametro { get; set; }
    }

    [DataContract]
    [Serializable]
    public class SpreadIFP
    {
        [DataMember]
        public int num_mes { get; set; }

        [DataMember]
        public double val_spread { get; set; }
    }

    [DataContract]
    [Serializable]
    public class TablaVTD
    {
        [DataMember]
        public string cod_moneda { get; set; }
        [DataMember]
        public int num_mes { get; set; }
        [DataMember]
        public double val_vtd { get; set; }
        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
    }

    [DataContract]
    [Serializable]
    public class TablaVOLA
    {
        [DataMember]
        public string cod_moneda { get; set; }
        [DataMember]
        public int num_mes { get; set; }
        [DataMember]
        public double val_vola { get; set; }
        [DataMember]
        public string cod_tipo_temporalidad { get; set; }
    }



    [DataContract]
    [Serializable]
    public class FactorCotizacionMotorIFP
    {
        [DataMember]
        public long cod_factor { get; set; }

        [DataMember]
        public Int64 num_mes { get; set; }

        [DataMember]
        public Int64 num_correlativo { get; set; }

        [DataMember]
        public DateTime fec_periodo { get; set; }

        [DataMember]
        public double val_factor { get; set; }
    }

    [DataContract]
    [Serializable]
    public class ParametroAshMotorIFP
    {
        [DataMember]
        public double val_cmor { get; set; }

        [DataMember]
        public double val_fcon { get; set; }

        [DataMember]
        public double tas_htra { get; set; }

        [DataMember]
        public double tas_htva { get; set; }

        [DataMember]
        public double tas_ltra { get; set; }

        [DataMember]
        public double tas_ltva { get; set; }

        [DataMember]
        public double val_rend { get; set; }

        [DataMember]
        public double tas_tgpd { get; set; }

        [DataMember]
        public int flg_ibtp { get; set; }

        [DataMember]
        public int flg_ivnt { get; set; }

        [DataMember]
        public int flg_ideb { get; set; }

        [DataMember]
        public int flg_iajm { get; set; }

        [DataMember]
        public int flg_icmo { get; set; }

        [DataMember]
        public double tas_timp { get; set; }

        [DataMember]
        public double tas_tsbs { get; set; }

        [DataMember]
        public double tas_ttec { get; set; }

        [DataMember]
        public double val_gfi1 { get; set; }

        [DataMember]
        public double val_gfi2 { get; set; }

        [DataMember]
        public double val_comi { get; set; }

        [DataMember]
        public double val_coba { get; set; }

        [DataMember]
        public double val_pumi { get; set; }

        [DataMember]
        public double num_nins { get; set; }

        [DataMember]
        public double num_nper { get; set; }

        [DataMember]
        public double val_ltit { get; set; }

        [DataMember]
        public double val_htit { get; set; }

        //[DataMember]
        //public double ini_tra2 { get; set; }

        //[DataMember]
        //public double val_tasa_inv { get; set; }

        [DataMember]
        public double val_tasa_inf { get; set; }
        [DataMember]
        public double val_tasa_mrg_solv { get; set; }
        [DataMember]
        public double val_tasa_costo_cap { get; set; }
        [DataMember]
        public double val_vtax { get; set; }
    }

    [DataContract]
    [Serializable]
    public class BeneficiarioMotorIFP
    {
        [DataMember]
        public int item { get; set; }

        [DataMember]
        public string cod_parentesco { get; set; }

        [DataMember]
        public DateTime fec_nacimiento { get; set; }

        [DataMember]
        public bool ind_invalido { get; set; }

        [DataMember]
        public string cod_sexo { get; set; }

        [DataMember]
        public double val_pje_renta { get; set; }

        [DataMember]
        public double val_pje_adicional { get; set; }

        [DataMember]
        public CotizacionMotorIFP Cotizacion { get; set; }

        [DataMember]
        public string num_identificacion { get; set; }
}

    [DataContract]
    [Serializable]
    public class RespuestaMotorIFP
    {
        [DataMember]
        public string estado { get; set; }
        [DataMember]
        public string titulo { get; set; }
        [DataMember]
        public string mensaje { get; set; }
        [DataMember]
        public DateTime fecha_hora { get; set; }
    }

    [DataContract]
    [Serializable]
    public class TemporalidadMonedaMotorIFP
    {
        [DataMember]
        public string cod_tipo_temporalidad { get; set; }

        [DataMember]
        public string cod_moneda { get; set; }
    }

    [DataContract]
    [Serializable]
    public class ReservaMotorIFP
    {
        /*INI - reserva*/

        [DataMember]
        public double val_res_pension { get; set; }

        [DataMember]
        public double val_res_pension_cpg { get; set; }

        [DataMember]
        public double val_res_pension_spg { get; set; }

        [DataMember]
        public double val_res_sepelio { get; set; }

        [DataMember]
        public double val_res_devolucion { get; set; }

        [DataMember]
        public double val_res_fallecimiento { get; set; }

        [DataMember]
        public double val_res_total { get; set; }

        /*FIN - reserva*/

        /*INI - CRU*/

        [DataMember]
        public double val_res_cru_pension { get; set; }

        [DataMember]
        public double val_res_cru_sepelio { get; set; }

        [DataMember]
        public double val_res_cru_devolucion { get; set; }

        [DataMember]
        public double val_res_cru_fallecimiento { get; set; }

        /*FIN - CRU*/

        //[DataMember]
        //public CotizacionMotorIFP cotizacion { get; set; }
        [DataMember]
        public double val_prima_unica_sepelio { get; set; }
        [DataMember]
        public double val_prima_unica_pension { get; set; }
        [DataMember]
        public double val_prima_unica_devolucion { get; set; }
        [DataMember]
        public double val_prima_unica_fallecimiento { get; set; }
    }

}