using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Data;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioEmisionPoliza : IRepositorioEmisionPoliza
    {

        public EmisionPoliza EmitirPoliza(string num_solicitud, int num_poliza, string dig_poliza)
        {
            EmisionPoliza emisionPoliza = null;
            SolicitudRPPlus solicitud;
            Poliza poliza;
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_emision_poliza_RtaPrvdPlus", num_solicitud, num_poliza, dig_poliza))
            {
                solicitud = new SolicitudRPPlus();
                poliza = new Poliza();

                while (dr.Read())
                {
                    emisionPoliza = new EmisionPoliza();
                    /*Propue*/
                    solicitud.Id = dr["num_solicitud"].ToString();
                    if (dr["num_cuispp"] != DBNull.Value)
                        solicitud.Afiliado = new Afiliado
                        {
                            CUSPP = dr["num_cuispp"].ToString(),
                            SaldoCIC = Convert.ToDouble(dr["val_tot_cic"])
                        };

                    if (dr["fec_solicitud"] != DBNull.Value)
                        solicitud.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);

                    if (dr["cod_tipo_plan_rpp"] != DBNull.Value)
                        solicitud.TipoPlan = new TipoPlan
                        {
                            Id = dr["cod_tipo_plan_rpp"].ToString(),
                            Nombre = dr["gls_plan"].ToString()
                        };

                    solicitud.MonedaPrimaUnica = new Moneda { Id = "", Nombre = "", Simbolo = "" };
                    if (dr["cod_moneda_cta_indiv"] != DBNull.Value)
                        solicitud.MonedaPrimaUnica = new Moneda
                        {
                            Id = dr["cod_moneda_cta_indiv"].ToString(),
                            Nombre = dr["gls_corta_moneda"].ToString(),
                            Simbolo = dr["simbolo_moneda"].ToString()
                        };

                    if (dr["val_dcom"] != DBNull.Value)
                        solicitud.PorcentajeDescuentoComision = Convert.ToDouble(dr["val_dcom"]);

                    solicitud.Temporalidad = new Temporalidad { Id = "", Nombre = "", Anhos = 0 };
                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        solicitud.Temporalidad = new Temporalidad
                        {
                            Id = dr["cod_tipo_temporalidad"].ToString(),
                            Nombre = dr["gls_corta_temporalidad"].ToString().ToUpper(),
                            Anhos = Convert.ToInt32(dr["val_anos"])
                        };

                    if (dr["fec_devengue"] != DBNull.Value)
                        solicitud.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);

                    if (dr["pje_comision"] != DBNull.Value)
                        emisionPoliza.PjeComision = Convert.ToDouble(dr["pje_comision"]);

                    if (dr["gls_comision"] != DBNull.Value)
                        emisionPoliza.gls_PjeComision = dr["gls_comision"].ToString();

                    if (dr["val_gasto_sepelio"] != DBNull.Value)
                        emisionPoliza.ValGastoSepelio = Convert.ToDouble(dr["val_gasto_sepelio"]);

                    /*Poliza*/
                    if (dr["num_poliza"] != DBNull.Value)
                        poliza.NumPoliza = Convert.ToInt32(dr["num_poliza"]);

                    if (dr["dig_poliza"] != DBNull.Value)
                        poliza.DigPoliza = dr["dig_poliza"].ToString();

                    //if (dr["dig_poliza"] != DBNull.Value)
                    //    poliza.DigPoliza = dr["dig_poliza"].ToString();

                    if (dr["fec_emision_poliza"] != DBNull.Value)
                        poliza.FecEmision = Convert.ToDateTime(dr["fec_emision_poliza"]);

                    if (dr["fec_ini_vigencia_poliza"] != DBNull.Value)
                        poliza.FecInicioVigencia = Convert.ToDateTime(dr["fec_ini_vigencia_poliza"]);

                    if (dr["fec_fin_vigencia_poliza"] != DBNull.Value)
                        poliza.FecFinVigencia = Convert.ToDateTime(dr["fec_fin_vigencia_poliza"]);

                    if (dr["fec_ini_pago_doble"] != DBNull.Value)
                        poliza.FecInicioPagoDoble = Convert.ToDateTime(dr["fec_ini_pago_doble"]);

                    if (dr["fec_fin_pago_doble"] != DBNull.Value)
                        poliza.FecFinPagoDoble = Convert.ToDateTime(dr["fec_fin_pago_doble"]);

                    //if (dr["fec_fin_pago_doble"] != DBNull.Value)
                    //    poliza.FecInicioPagoDoble = Convert.ToDateTime(dr["fec_fin_pago_doble"]);

                    if (dr["fec_inicio_pago"] != DBNull.Value)
                        poliza.FecPago = Convert.ToDateTime(dr["fec_inicio_pago"]);

                    //if (dr["fec_inicio_pago"] != DBNull.Value)
                    //    poliza.FecPago = Convert.ToDateTime(dr["fec_inicio_pago"]);

                    if (dr["val_prima_neta"] != DBNull.Value)
                        poliza.ValPrimaNeta = Convert.ToDouble(dr["val_prima_neta"]);

                    if (dr["val_iva"] != DBNull.Value)
                        poliza.ValIva = Convert.ToDouble(dr["val_iva"]);

                    if (dr["val_prima_bruta"] != DBNull.Value)
                        poliza.ValPrimaBruta = Convert.ToDouble(dr["val_prima_bruta"]);

                    //<INI.GTI_7012_ADMWR>
                    if (dr["val_meses_periodo"] != DBNull.Value)
                        poliza.val_meses_periodo = Convert.ToInt32(dr["val_meses_periodo"]);
                    //<FIN.GTI_7012_ADMWR>

                    /*Cotizacion*/
                    CotizacionRPPlus cotizacion = new CotizacionRPPlus();
                    solicitud.Cotizaciones = new List<CotizacionRPPlus>();
                    if (dr["num_correlativo"] != DBNull.Value)
                        cotizacion.Correlativo = Convert.ToInt64(dr["num_correlativo"]);

                    if (dr["val_pen_cia_mo"] != DBNull.Value)
                        cotizacion.PensionCiaMO = Convert.ToDouble(dr["val_pen_cia_mo"]);

                    if (dr["ind_gasto_sepelio"] != DBNull.Value)
                        cotizacion.IndGastoSepelio = dr["ind_gasto_sepelio"].ToString();

                    if (dr["val_per_garantizado"] != DBNull.Value)
                        cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["val_per_garantizado"]);

                    cotizacion.Modalidad = new Modalidad { Id = "", Nombre = "" };
                    if (dr["cod_modalidad"] != DBNull.Value)
                        cotizacion.Modalidad = new Modalidad { Id = dr["cod_modalidad"].ToString(), Nombre = dr["ind_modalidad"].ToString() };

                    if (dr["val_per_temporal"] != DBNull.Value)
                        cotizacion.PagoEscalonada = Convert.ToDouble(dr["val_per_temporal"]);

                    if (dr["val_pje_dev"] != DBNull.Value)
                        cotizacion.ValPjeDev = Convert.ToDouble(dr["val_pje_dev"]);

                    if (dr["val_mon_aju"] != DBNull.Value)
                        cotizacion.ValMonAju = Convert.ToDouble(dr["val_mon_aju"]);

                    cotizacion.Moneda = new Moneda { Id = "", Nombre = "", Simbolo = "" };
                    if (dr["cod_moneda_cotizacion"] != DBNull.Value)
                        cotizacion.Moneda = new Moneda { Id = dr["cod_moneda_cotizacion"].ToString() };

                    //if (dr["tir"] != DBNull.Value)
                    //    cotizacion.TasaRetornoAccionista = Convert.ToDouble(dr["tir"]);

                    //<INI.GTI_7012_ADMWR>
                    if (dr["val_tasa_venta"] != DBNull.Value)
                        cotizacion.TasaVenta = Convert.ToDouble(dr["val_tasa_venta"]);

                    if (dr["val_tasa_venta_is"] != DBNull.Value)
                        cotizacion.TasaVentaSbs = Convert.ToDouble(dr["val_tasa_venta_is"]);

                    if (dr["val_tasa_tir"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionista = Convert.ToDouble(dr["val_tasa_tir"]);

                    if (dr["val_pje_rent_temp"] != DBNull.Value)
                        cotizacion.PjePE = Convert.ToDouble(dr["val_pje_rent_temp"]);

                    if (dr["val_pje_conyuge"] != DBNull.Value)
                        cotizacion.ValPjeConyuge = Convert.ToDouble(dr["val_pje_conyuge"]);
                    //<FIN.GTI_7012_ADMWR>

                    //<INI.GTI_7012_22>
                    if (dr["val_tasa_costo_equiv"] != DBNull.Value)
                        cotizacion.ValTasaCostoEquiv = Convert.ToDouble(dr["val_tasa_costo_equiv"]);
                    //<FIN.GTI_7012_22>

                    if (dr["cod_tipo_producto"] != DBNull.Value)
                        cotizacion.Producto = new Producto { Id = dr["cod_tipo_producto"].ToString() };

                    solicitud.Cotizaciones.Add(cotizacion);

                    if (dr["cod_estado_Poliza"] != DBNull.Value)
                        solicitud.CodigoEstadoPoliza = dr["cod_estado_Poliza"].ToString();

                    if (dr["cod_causal_estado_Poliza"] != DBNull.Value)
                        solicitud.CausalPoliza = new CausalPoliza { Id = dr["cod_causal_estado_Poliza"].ToString() };

                    if (dr["num_agente"] != DBNull.Value)
                        solicitud.Agente = new Agente { Id = dr["num_agente"].ToString() };

                    if (dr["val_res_sepelio"] != DBNull.Value)
                        cotizacion.Val_prima_unica_sepelio = Convert.ToDouble(dr["val_res_sepelio"]);

                    if (dr["val_res_pension"] != DBNull.Value)
                        cotizacion.Val_prima_unica_pension = Convert.ToDouble(dr["val_res_pension"]);

                    if (dr["val_res_devolucion"] != DBNull.Value)
                        cotizacion.Val_prima_unica_devolucion = Convert.ToDouble(dr["val_res_devolucion"]);

                    if (dr["val_res_fallecimiento"] != DBNull.Value)
                        cotizacion.Val_prima_unica_fallecimiento = Convert.ToDouble(dr["val_res_fallecimiento"]);
                }

                dr.NextResult();

                /*Beneficiarios*/
                solicitud.Beneficiarios = new List<GrupoFamiliar>();
                while (dr.Read())
                {
                    GrupoFamiliar grupoFamiliar = new GrupoFamiliar();
                    grupoFamiliar = new GrupoFamiliar();

                    if (dr["val_pje_ret"] != DBNull.Value)
                        grupoFamiliar.ValPjeRenta = Convert.ToDouble(dr["val_pje_ret"]);

                    //No Muestra Mayores de Edad o Segun el porcentaje de Renta
                    if (grupoFamiliar.ValPjeRenta > 0)
                    {
                        if (dr["ape_paterno"] != DBNull.Value)
                            grupoFamiliar.ApellidoPaterno = dr["ape_paterno"].ToString();

                        if (dr["ape_materno"] != DBNull.Value)
                            grupoFamiliar.ApellidoMaterno = dr["ape_materno"].ToString();

                        if (dr["nom_persona"] != DBNull.Value)
                            grupoFamiliar.Nombre = dr["nom_persona"].ToString();

                        if (dr["apellidos_nombres"] != DBNull.Value)
                            grupoFamiliar.ApellidosNombres = dr["apellidos_nombres"].ToString();

                        grupoFamiliar.Identificacion = new Identificacion { IdTipo = "", Numero = "0", GlosaTipo = "" };
                        if (dr["cod_tipo_identificacion"] != DBNull.Value)
                            grupoFamiliar.Identificacion = new Identificacion
                            {
                                IdTipo = dr["cod_tipo_identificacion"].ToString(),
                                Numero = dr["num_identificacion"].ToString(),
                                GlosaTipo = dr["gls_corta_identificacion"].ToString()
                            };

                        if (dr["fec_nacimiento"] != DBNull.Value)
                            grupoFamiliar.FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);

                        if (dr["cod_sexo"] != DBNull.Value)
                            grupoFamiliar.Sexo = Convert.ToChar(dr["cod_sexo"].ToString());

                        grupoFamiliar.Parentesco = new Parentesco { Id = "", Nombre = "" };
                        if (dr["cod_parentezco"] != DBNull.Value)
                            grupoFamiliar.Parentesco = new Parentesco
                            {
                                Id = dr["cod_parentezco"].ToString(),
                                Nombre = dr["abrev_parentesco"].ToString()
                            };

                        if (dr["apellidos_nombres"] != DBNull.Value)
                            grupoFamiliar.ApellidosNombres = dr["apellidos_nombres"].ToString();

                        grupoFamiliar.Banco = new Parametro { Id = "", Glosa = "" };
                        if (dr["cod_banco"] != DBNull.Value)
                            grupoFamiliar.Banco = new Parametro { Id = dr["cod_banco"].ToString(), Glosa = dr["gls_banco"].ToString() };

                        grupoFamiliar.TipoCtaBanco = new Parametro { Id = "", Glosa = "" };
                        if (dr["cod_tipo_cta_banco"] != DBNull.Value)
                            grupoFamiliar.TipoCtaBanco = new Parametro { Id = dr["cod_tipo_cta_banco"].ToString() };

                        if (dr["gls_NroCtaBancaria"] != DBNull.Value)
                            grupoFamiliar.NumeroBanco = dr["gls_NroCtaBancaria"].ToString();

                        if (dr["cod_Comunicacion"] != DBNull.Value)
                            grupoFamiliar.Comunicacion = new Parametro { Id = dr["cod_Comunicacion"].ToString() };

                        if (dr["cod_Confidencialidaddatos"] != DBNull.Value)
                            grupoFamiliar.Confidencialidaddatos = new Parametro { Id = dr["cod_Confidencialidaddatos"].ToString() };

                        if (dr["ind_PEP"] != DBNull.Value)
                            grupoFamiliar.ind_PEP = dr["ind_PEP"].ToString() == "S" ? true : false;

                        if (dr["ind_SujetoObligado"] != DBNull.Value)
                            grupoFamiliar.ind_SujetoObligado = dr["ind_SujetoObligado"].ToString() == "S" ? true : false;

                        if (dr["cod_Nacionalidad"] != DBNull.Value)
                            grupoFamiliar.Nacionalidad = new Temporal { cod_parametro = dr["cod_Nacionalidad"].ToString() };

                        if (dr["cod_Profesion"] != DBNull.Value)
                            grupoFamiliar.Profesion = new Temporal { cod_parametro = dr["cod_Profesion"].ToString() };

                        if (dr["cod_tipo_invalidez"] != DBNull.Value)
                            grupoFamiliar.TipoInvalidez = new TipoInvalidez { Id = dr["cod_tipo_invalidez"].ToString() };

                        if (dr["num_item"] != DBNull.Value)
                            grupoFamiliar.num_item = Convert.ToInt32(dr["num_item"].ToString());

                        //<INI.GTI_7012_ADMWR>
                        if (dr["fec_invalidez"] != DBNull.Value)
                            grupoFamiliar.FechaInvalidez = Convert.ToDateTime(dr["fec_invalidez"]);

                        if (dr["ind_invalidez"] != DBNull.Value)
                            grupoFamiliar.Invalido = dr["ind_invalidez"].ToString() == "S" ? true : false;
                        //<FIN.GTI_7012_ADMWR>

                        if (dr["cod_tipo_periodo_beneficiario"] != DBNull.Value)
                        {
                            grupoFamiliar.IdTipoPeriodoBeneficiario = Convert.ToInt32(dr["cod_tipo_periodo_beneficiario"].ToString());
                        }

                        solicitud.Beneficiarios.Add(grupoFamiliar);
                    }
                }

                if (emisionPoliza != null)
                {
                    emisionPoliza.SolicitudRPPlus = new SolicitudRPPlus();
                    emisionPoliza.SolicitudRPPlus = solicitud;
                    emisionPoliza.Poliza = new Poliza();
                    emisionPoliza.Poliza = poliza;
                }
            }
            return emisionPoliza;
        }

        public EmisionPoliza EmitirPolizaIFP(string num_solicitud, int num_poliza, string dig_poliza)
        {
            EmisionPoliza emisionPoliza = null;
            SolicitudIFP solicitud;
            Poliza poliza;
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_emision_poliza_ifp", num_solicitud, num_poliza, dig_poliza))
            {
                solicitud = new SolicitudIFP();
                poliza = new Poliza();

                while (dr.Read())
                {
                    emisionPoliza = new EmisionPoliza();
                    /*Propue*/
                    solicitud.Id = dr["num_solicitud"].ToString();
                    if (dr["num_cuispp"] != DBNull.Value)
                        solicitud.Afiliado = new Afiliado
                        {
                            CUSPP = dr["num_cuispp"].ToString(),
                            SaldoCIC = Convert.ToDouble(dr["val_tot_cic"]),
                            //CorreoElectronico = dr["gls_mail"].ToString()
                        };

                    if (dr["fec_solicitud"] != DBNull.Value)
                        solicitud.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);

                    //if (dr["cod_tipo_plan_rpp"] != DBNull.Value)
                    //    solicitud.TipoPlan = new TipoPlan
                    //    {
                    //        Id = dr["cod_tipo_plan_rpp"].ToString(),
                    //        Nombre = dr["gls_plan"].ToString()
                    //    };

                    solicitud.MonedaPrimaUnica = new Moneda { Id = "", Nombre = "", Simbolo = "" };
                    if (dr["cod_moneda_cta_indiv"] != DBNull.Value)
                        solicitud.MonedaPrimaUnica = new Moneda
                        {
                            Id = dr["cod_moneda_cta_indiv"].ToString(),
                            Nombre = dr["gls_corta_moneda"].ToString(),
                            Simbolo = dr["simbolo_moneda"].ToString()
                        };

                    //if (dr["val_dcom"] != DBNull.Value)
                    //    solicitud.PorcentajeDescuentoComision = Convert.ToDouble(dr["val_dcom"]);

                    //solicitud.Temporalidad = new Temporalidad { Id = "", Nombre = "", Anhos = 0 };
                    //if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                    //    solicitud.Temporalidad = new Temporalidad
                    //    {
                    //        Id = dr["cod_tipo_temporalidad"].ToString(),
                    //        Nombre = dr["gls_corta_temporalidad"].ToString().ToUpper(),
                    //        Anhos = Convert.ToInt32(dr["val_anos"])
                    //    };

                    if (dr["fec_devengue"] != DBNull.Value)
                        solicitud.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);

                    if (dr["cod_tipo_cotizacion"] != DBNull.Value)
                        solicitud.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };

                    //if (dr["val_moneda"] != DBNull.Value)
                    //    solicitud.TipoCambio = Convert.ToDouble(dr["val_moneda"]);

                    if (dr["pje_comision"] != DBNull.Value)
                        emisionPoliza.PjeComision = Convert.ToDouble(dr["pje_comision"]);

                    if (dr["gls_comision"] != DBNull.Value)
                        emisionPoliza.gls_PjeComision = dr["gls_comision"].ToString();

                    if (dr["val_gasto_sepelio"] != DBNull.Value)
                        emisionPoliza.ValGastoSepelio = Convert.ToDouble(dr["val_gasto_sepelio"]);

                    if (dr["cod_canal_distribucion"] != DBNull.Value)
                        solicitud.CodCanalDistribucion = dr["cod_canal_distribucion"].ToString();

                    /*Poliza*/
                    if (dr["num_poliza"] != DBNull.Value)
                        poliza.NumPoliza = Convert.ToInt32(dr["num_poliza"]);

                    if (dr["dig_poliza"] != DBNull.Value)
                        poliza.DigPoliza = dr["dig_poliza"].ToString();

                    if (dr["dig_poliza"] != DBNull.Value)
                        poliza.DigPoliza = dr["dig_poliza"].ToString();

                    if (dr["fec_emision_poliza"] != DBNull.Value)
                        poliza.FecEmision = Convert.ToDateTime(dr["fec_emision_poliza"]);

                    if (dr["fec_ini_vigencia_poliza"] != DBNull.Value)
                        poliza.FecInicioVigencia = Convert.ToDateTime(dr["fec_ini_vigencia_poliza"]);

                    if (dr["fec_fin_vigencia_poliza"] != DBNull.Value)
                        poliza.FecFinVigencia = Convert.ToDateTime(dr["fec_fin_vigencia_poliza"]);

                    if (dr["fec_ini_pago_doble"] != DBNull.Value)
                        poliza.FecInicioPagoDoble = Convert.ToDateTime(dr["fec_ini_pago_doble"]);

                    if (dr["fec_fin_pago_doble"] != DBNull.Value)
                        poliza.FecFinPagoDoble = Convert.ToDateTime(dr["fec_fin_pago_doble"]);

                    if (dr["fec_inicio_pago"] != DBNull.Value)
                        poliza.FecPago = Convert.ToDateTime(dr["fec_inicio_pago"]);

                    //if (dr["fec_inicio_pago"] != DBNull.Value)
                    //    poliza.FecPago = Convert.ToDateTime(dr["fec_inicio_pago"]);

                    if (dr["val_prima_neta"] != DBNull.Value)
                        poliza.ValPrimaNeta = Convert.ToDouble(dr["val_prima_neta"]);

                    if (dr["val_iva"] != DBNull.Value)
                        poliza.ValIva = Convert.ToDouble(dr["val_iva"]);

                    if (dr["val_prima_bruta"] != DBNull.Value)
                        poliza.ValPrimaBruta = Convert.ToDouble(dr["val_prima_bruta"]);

                    if (dr["val_meses_periodo"] != DBNull.Value)
                        poliza.val_meses_periodo = Convert.ToInt32(dr["val_meses_periodo"]);

                    poliza.ind_rescate = Convert.ToBoolean(dr["ind_rescate"]);

                    /*Cotizacion*/
                    CotizacionIFP cotizacion = new CotizacionIFP();
                    solicitud.Cotizaciones = new List<CotizacionIFP>();
                    if (dr["num_correlativo"] != DBNull.Value)
                        cotizacion.Correlativo = Convert.ToInt64(dr["num_correlativo"]);

                    if (dr["val_pen_cia_mo"] != DBNull.Value)
                        cotizacion.PensionCiaMO = Convert.ToDouble(dr["val_pen_cia_mo"]);

                    if (dr["ind_gasto_sepelio"] != DBNull.Value)
                        cotizacion.IndGastoSepelio = dr["ind_gasto_sepelio"].ToString();

                    if (dr["val_per_garantizado"] != DBNull.Value)
                        cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["val_per_garantizado"]);

                    cotizacion.Modalidad = new Modalidad { Id = "", Nombre = "" };
                    if (dr["cod_modalidad"] != DBNull.Value)
                        cotizacion.Modalidad = new Modalidad { Id = dr["cod_modalidad"].ToString(), Nombre = dr["ind_modalidad"].ToString() };

                    if (dr["val_per_temporal"] != DBNull.Value)
                        cotizacion.PagoDoble = Convert.ToDouble(dr["val_per_temporal"]);

                    if (dr["val_pje_dev"] != DBNull.Value)
                        cotizacion.ValPjeDev = Convert.ToDouble(dr["val_pje_dev"]);

                    if (dr["val_dev"] != DBNull.Value)
                        cotizacion.ValDev = Convert.ToDouble(dr["val_dev"]);

                    if (dr["val_mon_aju"] != DBNull.Value)
                        cotizacion.ValMonAju = Convert.ToDouble(dr["val_mon_aju"]);

                    cotizacion.Moneda = new Moneda { Id = "", Nombre = "", Simbolo = "" };
                    if (dr["cod_moneda_cotizacion"] != DBNull.Value)
                        cotizacion.Moneda = new Moneda { Id = dr["cod_moneda_cotizacion"].ToString() };

                    if (dr["val_tasa_venta"] != DBNull.Value)
                        cotizacion.TasaVenta = Convert.ToDouble(dr["val_tasa_venta"]);

                    if (dr["val_tasa_venta_is"] != DBNull.Value)
                        cotizacion.TasaVentaSbs = Convert.ToDouble(dr["val_tasa_venta_is"]);

                    if (dr["val_tasa_tir"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionista = Convert.ToDouble(dr["val_tasa_tir"]);

                    if (dr["val_pje_rent_temp"] != DBNull.Value)
                        cotizacion.PjePagoDoble = Convert.ToDouble(dr["val_pje_rent_temp"]);

                    if (dr["val_tasa_costo_equiv"] != DBNull.Value)
                        cotizacion.ValTasaCostoEquiv = Convert.ToDouble(dr["val_tasa_costo_equiv"]);

                    if (dr["cod_tipo_producto"] != DBNull.Value)
                        cotizacion.Producto = new Producto { Id = dr["cod_tipo_producto"].ToString() };


                    if (dr["val_dcom"] != DBNull.Value)
                        cotizacion.ValPjeDCOM = Convert.ToDouble(dr["val_dcom"]);

                    cotizacion.Temporalidad = new Temporalidad { Id = "", Nombre = "", Anhos = 0 };
                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        cotizacion.Temporalidad = new Temporalidad
                        {
                            Id = dr["cod_tipo_temporalidad"].ToString(),
                            Nombre = dr["gls_corta_temporalidad"].ToString().ToUpper(),
                            Anhos = Convert.ToInt32(dr["val_anos"])
                        };

                    if (dr["cod_plan"] != DBNull.Value)
                        cotizacion.Plan = new Plan
                        {
                            Id = dr["cod_plan"].ToString(),
                            Nombre = dr["gls_plan"].ToString()
                        };

                    if (dr["val_pje_dev_fallec"] != DBNull.Value)
                        cotizacion.ValPjeDevFallec = Convert.ToDouble(dr["val_pje_dev_fallec"]);

                    if (dr["val_dev_fallec"] != DBNull.Value)
                        cotizacion.ValDevFallec = Convert.ToDouble(dr["val_dev_fallec"]);

                    if (dr["val_per_diferido"] != DBNull.Value)
                        cotizacion.ValPerDiferido = Convert.ToDouble(dr["val_per_diferido"]);

                    if (dr["val_res_pension"] != DBNull.Value)
                        cotizacion.ValResPension = Convert.ToDouble(dr["val_res_pension"]);

                    if (dr["val_res_sepelio"] != DBNull.Value)
                        cotizacion.ValResSepelio = Convert.ToDouble(dr["val_res_sepelio"]);

                    if (dr["val_res_devolucion"] != DBNull.Value)
                        cotizacion.ValResDevolucion = Convert.ToDouble(dr["val_res_devolucion"]);

                    if (dr["val_res_fallecimiento"] != DBNull.Value)
                        cotizacion.ValResFallecimiento = Convert.ToDouble(dr["val_res_fallecimiento"]);

                    solicitud.Cotizaciones.Add(cotizacion);

                    if (dr["cod_estado_Poliza"] != DBNull.Value)
                        solicitud.CodigoEstadoPoliza = dr["cod_estado_Poliza"].ToString();

                    if (dr["cod_causal_estado_Poliza"] != DBNull.Value)
                        solicitud.CausalPoliza = new CausalPoliza { Id = dr["cod_causal_estado_Poliza"].ToString() };

                    if (dr["num_agente"] != DBNull.Value)
                        solicitud.Agente = new Agente { Id = dr["num_agente"].ToString() };

                }

                dr.NextResult();

                /*Beneficiarios*/
                solicitud.Beneficiarios = new List<GrupoFamiliar>();
                while (dr.Read())
                {
                    GrupoFamiliar grupoFamiliar = new GrupoFamiliar();

                    if (dr["val_pje_ret"] != DBNull.Value)
                        grupoFamiliar.ValPjeRenta = Convert.ToDouble(dr["val_pje_ret"]);

                    //No Muestra Mayores de Edad o Segun el porcentaje de Renta
                    if (grupoFamiliar.ValPjeRenta > 0)
                    {
                        if (dr["ape_paterno"] != DBNull.Value)
                            grupoFamiliar.ApellidoPaterno = dr["ape_paterno"].ToString();

                        if (dr["ape_materno"] != DBNull.Value)
                            grupoFamiliar.ApellidoMaterno = dr["ape_materno"].ToString();

                        if (dr["nom_persona"] != DBNull.Value)
                            grupoFamiliar.Nombre = dr["nom_persona"].ToString();

                        if (dr["nom_persona"] != DBNull.Value)
                            grupoFamiliar.ApellidosNombres = dr["apellidos_nombres"].ToString();

                        grupoFamiliar.Identificacion = new Identificacion { IdTipo = "", Numero = "0", GlosaTipo = "" };
                        if (dr["cod_tipo_identificacion"] != DBNull.Value)
                            grupoFamiliar.Identificacion = new Identificacion
                            {
                                IdTipo = dr["cod_tipo_identificacion"].ToString(),
                                //Numero = Convert.ToInt32(dr["num_identificacion"]),
                                Numero = dr["num_identificacion"].ToString(),
                                GlosaTipo = dr["gls_corta_identificacion"].ToString()
                            };

                        if (dr["fec_nacimiento"] != DBNull.Value)
                            grupoFamiliar.FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);

                        if (dr["cod_sexo"] != DBNull.Value)
                            grupoFamiliar.Sexo = Convert.ToChar(dr["cod_sexo"].ToString());

                        grupoFamiliar.Parentesco = new Parentesco { Id = "", Nombre = "" };
                        if (dr["cod_parentezco"] != DBNull.Value)
                            grupoFamiliar.Parentesco = new Parentesco
                            {
                                Id = dr["cod_parentezco"].ToString(),
                                Nombre = dr["abrev_parentesco"].ToString()
                            };

                        if (dr["nom_persona"] != DBNull.Value)
                            grupoFamiliar.ApellidosNombres = dr["apellidos_nombres"].ToString();

                        grupoFamiliar.Banco = new Parametro { Id = "", Glosa = "" };
                        if (dr["cod_banco"] != DBNull.Value)
                            grupoFamiliar.Banco = new Parametro { Id = dr["cod_banco"].ToString(), Glosa = dr["gls_banco"].ToString() };

                        grupoFamiliar.TipoCtaBanco = new Parametro { Id = "", Glosa = "" };
                        if (dr["cod_tipo_cta_banco"] != DBNull.Value)
                            grupoFamiliar.TipoCtaBanco = new Parametro { Id = dr["cod_tipo_cta_banco"].ToString() };

                        if (dr["gls_NroCtaBancaria"] != DBNull.Value)
                            grupoFamiliar.NumeroBanco = dr["gls_NroCtaBancaria"].ToString();

                        grupoFamiliar.Comunicacion = new Parametro { Id = "" };
                        if (dr["cod_Comunicacion"] != DBNull.Value)
                            grupoFamiliar.Comunicacion = new Parametro { Id = dr["cod_Comunicacion"].ToString() };

                        grupoFamiliar.Confidencialidaddatos = new Parametro { Id = "" };
                        if (dr["cod_Confidencialidaddatos"] != DBNull.Value)
                            grupoFamiliar.Confidencialidaddatos = new Parametro { Id = dr["cod_Confidencialidaddatos"].ToString() };

                        if (dr["ind_PEP"] != DBNull.Value)
                            grupoFamiliar.ind_PEP = dr["ind_PEP"].ToString() == "S" ? true : false;

                        if (dr["ind_SujetoObligado"] != DBNull.Value)
                            grupoFamiliar.ind_SujetoObligado = dr["ind_SujetoObligado"].ToString() == "S" ? true : false;

                        grupoFamiliar.Nacionalidad = new Temporal { cod_parametro = "" };
                        if (dr["cod_Nacionalidad"] != DBNull.Value)
                            grupoFamiliar.Nacionalidad = new Temporal { cod_parametro = dr["cod_Nacionalidad"].ToString() };

                        grupoFamiliar.Profesion = new Temporal { cod_parametro = "" };
                        if (dr["cod_Profesion"] != DBNull.Value)
                            grupoFamiliar.Profesion = new Temporal { cod_parametro = dr["cod_Profesion"].ToString() };

                        grupoFamiliar.TipoInvalidez = new TipoInvalidez { Id = "" };
                        if (dr["cod_tipo_invalidez"] != DBNull.Value)
                            grupoFamiliar.TipoInvalidez = new TipoInvalidez { Id = dr["cod_tipo_invalidez"].ToString() };

                        if (dr["num_item"] != DBNull.Value)
                            grupoFamiliar.num_item = Convert.ToInt32(dr["num_item"].ToString());

                        if (dr["fec_invalidez"] != DBNull.Value)
                            grupoFamiliar.FechaInvalidez = Convert.ToDateTime(dr["fec_invalidez"]);

                        if (dr["ind_invalidez"] != DBNull.Value)
                            grupoFamiliar.Invalido = dr["ind_invalidez"].ToString() == "S" ? true : false;

                        if (dr["gls_mail"] != DBNull.Value)
                            grupoFamiliar.CorreoElectronico = dr["gls_mail"].ToString();

                        if (dr["cod_tipo_periodo_beneficiario"] != DBNull.Value)
                            grupoFamiliar.IdTipoPeriodoBeneficiario = Convert.ToInt32(dr["cod_tipo_periodo_beneficiario"].ToString());

                        solicitud.Beneficiarios.Add(grupoFamiliar);
                    }
                }

                dr.NextResult();

                solicitud.CotizacionRescates = new List<CotizacionRescate>();
                while (dr.Read())
                {
                    var cotizacionRescate = new CotizacionRescate();

                    cotizacionRescate.mes_rescate = Convert.ToInt32(dr["num_mes_rescate"]);
                    cotizacionRescate.valor_rescate = Convert.ToDouble(dr["val_rescate"]);
                    cotizacionRescate.tasa_rescate_mensual = Convert.ToDouble(dr["val_tasa_rescate_mensual"]);
                    cotizacionRescate.tasa_rescate_anual = Convert.ToDouble(dr["val_tasa_rescate_anual"]);

                    solicitud.CotizacionRescates.Add(cotizacionRescate);
                }

                if (emisionPoliza != null)
                {
                    emisionPoliza.SolicitudIFP = new SolicitudIFP();
                    emisionPoliza.SolicitudIFP = solicitud;
                    emisionPoliza.Poliza = new Poliza();
                    emisionPoliza.Poliza = poliza;
                }
            }
            return emisionPoliza;
        }

        public void Registrar(EmisionPoliza entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(EmisionPoliza entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(EmisionPoliza entity)
        {
            throw new NotImplementedException();
        }

        public EmisionPoliza ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public EmisionPoliza ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<EmisionPoliza> Listar()
        {
            throw new NotImplementedException();
        }

    }
}
