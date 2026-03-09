using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Interseguro.CWRV.Dominio.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data;
using Interseguro.CWRV.Dominio.Repositorios;
using System.Data.SqlClient;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{

    public class RepositorioSolicitudIFP : IRepositorioSolicitudIFP
    {
        public List<SolicitudIFP> Listar(string cuspp)
        {
            List<SolicitudIFP> listaSolicitudes = new List<SolicitudIFP>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_consultar_solicitudes_ifp", cuspp);

            dbc.CommandTimeout = 180;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    var solicitud = new SolicitudIFP();
                    solicitud.Id = dr["num_solicitud"].ToString();

                    solicitud.Afiliado = new Afiliado { CUSPP = cuspp };
                    solicitud.FechaSolicitud = dr["fec_solicitud"] != DBNull.Value ? Convert.ToDateTime(dr["fec_solicitud"]) : (DateTime?)null;
                    solicitud.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                    solicitud.FechaDevengue = dr["fec_devengue"] != DBNull.Value ? Convert.ToDateTime(dr["fec_devengue"]) : (DateTime?)null;
                    solicitud.MonedaPrimaUnica = dr["cod_moneda_cta_indiv"] != DBNull.Value ? new Moneda { Simbolo = dr["cod_moneda_cta_indiv"].ToString() } : null;
                    solicitud.PrimaUnica = dr["mto_prima_unica"] != DBNull.Value ? Convert.ToDouble(dr["mto_prima_unica"]) : 0;
                    solicitud.Agente = new Agente { Id = dr["num_agente"].ToString() };
                    solicitud.CodigoEstado = Convert.ToInt32(dr["cod_estado_rpp"].ToString());
                    solicitud.EstadoSolicitud = dr["gls_estado_solicitud"].ToString();
                    solicitud.FechaVigencia = dr["fec_vigencia"] != DBNull.Value ? Convert.ToDateTime(dr["fec_vigencia"]) : (DateTime?)null;
                    solicitud.CodigoEstadoPlaft = Convert.ToInt32(dr["cod_estado_plaft"].ToString());
                    solicitud.CodCanalDistribucion = dr["cod_canal_distribucion"].ToString();
                    solicitud.AgenteCotizacion = dr["agente_cotizacion"].ToString();
                    solicitud.NumeroPoliza = Convert.ToInt32(dr["num_poliza"].ToString());
                    solicitud.OrigenCotizacion = dr["ind_origen"].ToString();
                    solicitud.IdEstudioNecesidades = dr["id_estudio_necesidades"] != DBNull.Value ? Convert.ToInt32(dr["id_estudio_necesidades"]) : 0;

                    listaSolicitudes.Add(solicitud);
                }
            }

            return listaSolicitudes;
        }

        public void Registrar(ParametrosMotorIFP entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(ParametrosMotorIFP entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(ParametrosMotorIFP entity)
        {
            throw new NotImplementedException();
        }

        public ParametrosMotorIFP ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public ParametrosMotorIFP ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<ParametrosMotorIFP> Listar()
        {
            throw new NotImplementedException();
        }

        public ParametrosMotorIFP ObtenerParametroGenerales(string cod_tipo_temporalidad, string cod_moneda, DateTime fec_cotizacion, bool ind_flag, string usuario)
        {
            ParametrosMotorIFP parametrosIFP = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_carpr_solicitud_ifp", cod_tipo_temporalidad, cod_moneda, fec_cotizacion, ind_flag, DBNull.Value, usuario);

            dbc.CommandTimeout = 180;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {

                parametrosIFP = new ParametrosMotorIFP();

                parametrosIFP.cod_monedaIFP = cod_moneda;
                parametrosIFP.cod_tipo_temporalidadIFP = cod_tipo_temporalidad;
                parametrosIFP.fec_cotizacionIFP = fec_cotizacion;

                //1. tablas_mortalidad 
                List<TablaMortalidadMotorIFP> lst_tabla_mortalidadIFP = new List<TablaMortalidadMotorIFP>();

                while (dr.Read())
                {

                    TablaMortalidadMotorIFP tabla_mortalidadIFP = new TablaMortalidadMotorIFP();

                    if (dr["num_edad_mes"] != DBNull.Value)
                        tabla_mortalidadIFP.num_edad_mes = Convert.ToInt32(dr["num_edad_mes"].ToString());

                    if (dr["val_lx_mbh"] != DBNull.Value)
                        tabla_mortalidadIFP.val_lx_mbh = Convert.ToDouble(dr["val_lx_mbh"].ToString());

                    if (dr["val_lx_mbm"] != DBNull.Value)
                        tabla_mortalidadIFP.val_lx_mbm = Convert.ToDouble(dr["val_lx_mbm"].ToString());

                    if (dr["val_lx_mih"] != DBNull.Value)
                        tabla_mortalidadIFP.val_lx_mih = Convert.ToDouble(dr["val_lx_mih"].ToString());

                    if (dr["val_lx_mim"] != DBNull.Value)
                        tabla_mortalidadIFP.val_lx_mim = Convert.ToDouble(dr["val_lx_mim"].ToString());

                    if (dr["val_lx_mvh"] != DBNull.Value)
                        tabla_mortalidadIFP.val_lx_mvh = Convert.ToDouble(dr["val_lx_mvh"].ToString());

                    if (dr["val_lx_mvm"] != DBNull.Value)
                        tabla_mortalidadIFP.val_lx_mvm = Convert.ToDouble(dr["val_lx_mvm"].ToString());

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        tabla_mortalidadIFP.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();

                    lst_tabla_mortalidadIFP.Add(tabla_mortalidadIFP);
                }

                dr.NextResult();

                //2. factores_mejoras
                List<FactorMejoraMotorIFP> lst_factor_mejoraIFP = new List<FactorMejoraMotorIFP>();

                while (dr.Read())
                {

                    FactorMejoraMotorIFP factor_mejoraIFP = new FactorMejoraMotorIFP();

                    if (dr["num_edad_mes"] != DBNull.Value)
                        factor_mejoraIFP.num_edad_mes = Convert.ToInt32(dr["num_edad_mes"].ToString());

                    if (dr["val_lx_mbh"] != DBNull.Value)
                        factor_mejoraIFP.val_lx_mbh = Convert.ToDouble(dr["val_lx_mbh"].ToString());

                    if (dr["val_lx_mbm"] != DBNull.Value)
                        factor_mejoraIFP.val_lx_mbm = Convert.ToDouble(dr["val_lx_mbm"].ToString());

                    if (dr["val_lx_mih"] != DBNull.Value)
                        factor_mejoraIFP.val_lx_mih = Convert.ToDouble(dr["val_lx_mih"].ToString());

                    if (dr["val_lx_mim"] != DBNull.Value)
                        factor_mejoraIFP.val_lx_mim = Convert.ToDouble(dr["val_lx_mim"].ToString());

                    if (dr["val_lx_mvh"] != DBNull.Value)
                        factor_mejoraIFP.val_lx_mvh = Convert.ToDouble(dr["val_lx_mvh"].ToString());

                    if (dr["val_lx_mvm"] != DBNull.Value)
                        factor_mejoraIFP.val_lx_mvm = Convert.ToDouble(dr["val_lx_mvm"].ToString());

                    lst_factor_mejoraIFP.Add(factor_mejoraIFP);
                }

                dr.NextResult();

                //3. informacion_factores_mejoras
                List<InformacionFactorMejoraMotorIFP> lst_informacion_factor_mejoraIFP = new List<InformacionFactorMejoraMotorIFP>();

                while (dr.Read())
                {

                    InformacionFactorMejoraMotorIFP informacion_factor_mejoraIFP = new InformacionFactorMejoraMotorIFP();

                    if (dr["cod_tipo_pensionista"] != DBNull.Value)
                        informacion_factor_mejoraIFP.val_tipo_beneficiario = dr["cod_tipo_pensionista"].ToString();

                    if (dr["num_anio"] != DBNull.Value)
                        informacion_factor_mejoraIFP.num_anio_factor = Convert.ToInt32(dr["num_anio"].ToString());

                    lst_informacion_factor_mejoraIFP.Add(informacion_factor_mejoraIFP);
                }

                dr.NextResult();

                //4. 
                /*while (dr.Read())
                {
                
                }

                dr.NextResult();*/

                //5. icob_nominales
                /*List<TablaIcobMotorIFP> lst_tabla_icobIFP_nominales = new List<TablaIcobMotorIFP>();

                while (dr.Read())
                {

                    TablaIcobMotorIFP tabla_icobIFP_nominales = new TablaIcobMotorIFP();

                    if (dr["num_indi"] != DBNull.Value)
                        tabla_icobIFP_nominales.num_indi = Convert.ToDouble(dr["num_indi"].ToString());

                    if (dr["num_imon"] != DBNull.Value)
                        tabla_icobIFP_nominales.num_imon = Convert.ToDouble(dr["num_imon"].ToString());

                    if (dr["val_icop"] != DBNull.Value)
                        tabla_icobIFP_nominales.val_icop = Convert.ToDouble(dr["val_icop"].ToString());

                    if (dr["val_lcop"] != DBNull.Value)
                        tabla_icobIFP_nominales.val_lcop = Convert.ToDouble(dr["val_lcop"].ToString());

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        tabla_icobIFP_nominales.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();

                    if (dr["cod_moneda"] != DBNull.Value)
                        tabla_icobIFP_nominales.cod_moneda = dr["cod_moneda"].ToString();

                    lst_tabla_icobIFP_nominales.Add(tabla_icobIFP_nominales);
                }

                dr.NextResult();*/

                //6. icob_ajustados
                /*List<TablaIcobMotorIFP> lst_tabla_icobIFP_ajustados = new List<TablaIcobMotorIFP>();

                while (dr.Read())
                {

                    TablaIcobMotorIFP tabla_icobIFP_ajustados = new TablaIcobMotorIFP();

                    if (dr["num_indi"] != DBNull.Value)
                        tabla_icobIFP_ajustados.num_indi = Convert.ToDouble(dr["num_indi"].ToString());

                    if (dr["num_imon"] != DBNull.Value)
                        tabla_icobIFP_ajustados.num_imon = Convert.ToDouble(dr["num_imon"].ToString());

                    if (dr["val_icop"] != DBNull.Value)
                        tabla_icobIFP_ajustados.val_icop = Convert.ToDouble(dr["val_icop"].ToString());

                    if (dr["val_lcop"] != DBNull.Value)
                        tabla_icobIFP_ajustados.val_lcop = Convert.ToDouble(dr["val_lcop"].ToString());

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        tabla_icobIFP_ajustados.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();

                    if (dr["cod_moneda"] != DBNull.Value)
                        tabla_icobIFP_ajustados.cod_moneda = dr["cod_moneda"].ToString();

                    lst_tabla_icobIFP_ajustados.Add(tabla_icobIFP_ajustados);
                }

                dr.NextResult();*/

                //7. inversiones_temporales
                /*List<InversionTemporalMotorIFP> lst_inversiones_temporales = new List<InversionTemporalMotorIFP>();

                while (dr.Read())
                {

                    InversionTemporalMotorIFP inversiones_temporales = new InversionTemporalMotorIFP();

                    if (dr["cod_moneda"] != DBNull.Value)
                        inversiones_temporales.cod_moneda = dr["cod_moneda"].ToString();

                    if (dr["num_instrumento"] != DBNull.Value)
                        inversiones_temporales.num_instrumento = Convert.ToInt32(dr["num_instrumento"].ToString());

                    if (dr["num_periodo"] != DBNull.Value)
                        inversiones_temporales.num_periodo = Convert.ToInt32(dr["num_periodo"].ToString());

                    if (dr["valc"] != DBNull.Value)
                        inversiones_temporales.valc = Convert.ToDouble(dr["valc"].ToString());

                    if (dr["porc"] != DBNull.Value)
                        inversiones_temporales.porc = Convert.ToDouble(dr["porc"].ToString());

                    if (dr["tirc"] != DBNull.Value)
                        inversiones_temporales.tirc = Convert.ToDouble(dr["tirc"].ToString());

                    if (dr["tasa_inv"] != DBNull.Value)
                        inversiones_temporales.tasa_inv = Convert.ToDouble(dr["tasa_inv"].ToString());

                    if (dr["ind_devolucion"] != DBNull.Value)
                        inversiones_temporales.ind_devolucion = (dr["ind_devolucion"].ToString() == "N") ? false : true;

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        inversiones_temporales.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();

                    lst_inversiones_temporales.Add(inversiones_temporales);
                }

                dr.NextResult();*/

                /*//8. inversiones_vitalicias
                List<InversionVitaliciaMotorIFP> lst_inversiones_vitalicias = new List<InversionVitaliciaMotorIFP>();

                while (dr.Read())
                {

                    InversionVitaliciaMotorIFP inversiones_vitalicias = new InversionVitaliciaMotorIFP();

                    if (dr["cod_moneda"] != DBNull.Value)
                        inversiones_vitalicias.cod_moneda = dr["cod_moneda"].ToString();

                    if (dr["num_nins"] != DBNull.Value)
                        inversiones_vitalicias.num_nins = Convert.ToInt32(dr["num_nins"].ToString());

                    if (dr["num_nflu"] != DBNull.Value)
                        inversiones_vitalicias.num_nflu = Convert.ToInt32(dr["num_nflu"].ToString());

                    if (dr["val_venc"] != DBNull.Value)
                        inversiones_vitalicias.val_venc = Convert.ToDouble(dr["val_venc"].ToString());

                    if (dr["ind_devolucion"] != DBNull.Value)
                        inversiones_vitalicias.ind_devolucion = (dr["ind_devolucion"].ToString() == "N") ? false : true;

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        inversiones_vitalicias.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();


                    lst_inversiones_vitalicias.Add(inversiones_vitalicias);
                }

                dr.NextResult();*/

                //9. 5. factores_anti_seleccion
                List<FactorAntiSeleccionMotorIFP> lst_factores_anti_seleccion = new List<FactorAntiSeleccionMotorIFP>();

                while (dr.Read())
                {

                    FactorAntiSeleccionMotorIFP factores_anti_seleccion = new FactorAntiSeleccionMotorIFP();

                    if (dr["num_columna"] != DBNull.Value)
                        factores_anti_seleccion.cod_sexo = (dr["num_columna"].ToString() == "5") ? "M" : "F";

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        factores_anti_seleccion.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();

                    if (dr["val_tope"] != DBNull.Value)
                        factores_anti_seleccion.val_tope = Convert.ToDouble(dr["val_tope"].ToString());

                    if (dr["pje_ajuste"] != DBNull.Value)
                        factores_anti_seleccion.pje_ajuste = Convert.ToDouble(dr["pje_ajuste"].ToString());

                    lst_factores_anti_seleccion.Add(factores_anti_seleccion);
                }

                dr.NextResult();

                //10. 6. factores_pumi
                List<FactorPUMIMotorIFP> lst_factores_pumi = new List<FactorPUMIMotorIFP>();

                while (dr.Read())
                {

                    FactorPUMIMotorIFP factores_pumi = new FactorPUMIMotorIFP();

                    if (dr["num_columna"] != DBNull.Value)
                        factores_pumi.cod_sexo = (dr["num_columna"].ToString() == "5") ? "M" : "F";

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        factores_pumi.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();

                    if (dr["val_tope"] != DBNull.Value)
                        factores_pumi.val_tope = Convert.ToDouble(dr["val_tope"].ToString());

                    if (dr["pje_ajuste"] != DBNull.Value)
                        factores_pumi.pje_ajuste = Convert.ToDouble(dr["pje_ajuste"].ToString());

                    lst_factores_pumi.Add(factores_pumi);
                }

                dr.NextResult();

                //7. VTD
                List<TablaVTD> lst_VTD = new List<TablaVTD>();

                while (dr.Read())
                {

                    TablaVTD VTD = new TablaVTD();

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        VTD.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();

                    if (dr["cod_moneda"] != DBNull.Value)
                        VTD.cod_moneda = dr["cod_moneda"].ToString();

                    if (dr["num_mes"] != DBNull.Value)
                        VTD.num_mes = Convert.ToInt32(dr["num_mes"].ToString());

                    if (dr["vtd"] != DBNull.Value)
                        VTD.val_vtd = Convert.ToDouble(dr["vtd"].ToString());

                    lst_VTD.Add(VTD);
                }

                dr.NextResult();

                //8.VOLA
                List<TablaVOLA> lst_VOLA = new List<TablaVOLA>();

                while (dr.Read())
                {

                    TablaVOLA VOLA = new TablaVOLA();

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        VOLA.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();

                    if (dr["cod_moneda"] != DBNull.Value)
                        VOLA.cod_moneda = dr["cod_moneda"].ToString();

                    if (dr["num_mes"] != DBNull.Value)
                        VOLA.num_mes = Convert.ToInt32(dr["num_mes"].ToString());

                    if (dr["vola"] != DBNull.Value)
                        VOLA.val_vola = Convert.ToDouble(dr["vola"].ToString());

                    lst_VOLA.Add(VOLA);
                }

                dr.NextResult();

                //11. 9. rendimiento_inversion
                List<RendimientosInversionTemporalMotorIFP> lst_rendimiento_inversion = new List<RendimientosInversionTemporalMotorIFP>();

                while (dr.Read())
                {

                    RendimientosInversionTemporalMotorIFP rendimiento_inversion = new RendimientosInversionTemporalMotorIFP();

                    if (dr["cod_moneda"] != DBNull.Value)
                        rendimiento_inversion.cod_moneda = dr["cod_moneda"].ToString();

                    if (dr["num_periodo"] != DBNull.Value)
                        rendimiento_inversion.num_periodo = Convert.ToInt32(dr["num_periodo"].ToString());

                    if (dr["val_parametro"] != DBNull.Value)
                        rendimiento_inversion.val_parametro = Convert.ToDouble(dr["val_parametro"].ToString());

                    lst_rendimiento_inversion.Add(rendimiento_inversion);
                }


                dr.NextResult();

                //12. 10. tasa_duration
                List<TasaDurationMotorIFP> lst_tasa_duration = new List<TasaDurationMotorIFP>();

                while (dr.Read())
                {
                    TasaDurationMotorIFP tasa_duration = new TasaDurationMotorIFP();

                    if (dr["cod_parametro"] != DBNull.Value)
                        tasa_duration.cod_parametro = dr["cod_parametro"].ToString();

                    if (dr["val_parametro"] != DBNull.Value)
                        tasa_duration.val_parametro = Convert.ToDouble(dr["val_parametro"].ToString());

                    lst_tasa_duration.Add(tasa_duration);
                }

                dr.NextResult();

                //13. 11. spread
                List<SpreadIFP> lst_spread = new List<SpreadIFP>();

                while (dr.Read())
                {
                    SpreadIFP spread = new SpreadIFP();

                    if (dr["num_mes"] != DBNull.Value)
                        spread.num_mes = Convert.ToInt32(dr["num_mes"]);

                    if (dr["val_spread"] != DBNull.Value)
                        spread.val_spread = Convert.ToDouble(dr["val_spread"].ToString());

                    lst_spread.Add(spread);
                }

                parametrosIFP.tablas_mortalidad = lst_tabla_mortalidadIFP;
                parametrosIFP.factores_mejoras = lst_factor_mejoraIFP;
                parametrosIFP.informacion_factores_mejoras = lst_informacion_factor_mejoraIFP;
                //parametrosIFP.icob_nominales = lst_tabla_icobIFP_nominales;
                //parametrosIFP.icob_ajustados = lst_tabla_icobIFP_ajustados;
                //parametrosIFP.inversiones_temporales = lst_inversiones_temporales;
                //parametrosIFP.inversiones_vitalicias = lst_inversiones_vitalicias;
                parametrosIFP.factores_anti_seleccion = lst_factores_anti_seleccion;
                parametrosIFP.factores_pumi = lst_factores_pumi;
                parametrosIFP.lista_VTD = lst_VTD;
                parametrosIFP.lista_VOLA = lst_VOLA;
                parametrosIFP.rendimientos_inversiones_temporales = lst_rendimiento_inversion;
                parametrosIFP.tasa_duration = lst_tasa_duration;
                parametrosIFP.spread_rescate = lst_spread;
            }

            return parametrosIFP;
        }

        public ParametrosMotorIFP ObtenerParametroCotizacion(string num_solicitud, DateTime fec_cotizacion, int num_correlativo)
        {
            ParametrosMotorIFP parametrosIFP = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_carpr_cotizacion_ifp", num_solicitud, fec_cotizacion, num_correlativo);

            dbc.CommandTimeout = 180;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {

                parametrosIFP = new ParametrosMotorIFP();

                //1. cotizacion 
                CotizacionMotorIFP cotizacionIFP = new CotizacionMotorIFP();

                if (dr.Read())
                {

                    cotizacionIFP = new CotizacionMotorIFP();

                    if (dr["wl_num_solicitud"] != DBNull.Value)
                        cotizacionIFP.num_solicitud = dr["wl_num_solicitud"].ToString();

                    if (dr["wl_num_corr_cotiza"] != DBNull.Value)
                        cotizacionIFP.num_correlativo = Convert.ToInt64(dr["wl_num_corr_cotiza"].ToString());

                    if (dr["wl_cot_fcot"] != DBNull.Value)
                        cotizacionIFP.fec_cotizacion = Convert.ToDateTime(dr["wl_cot_fcot"].ToString());

                    if (dr["wl_cot_fdev"] != DBNull.Value)
                        cotizacionIFP.fec_devengue = Convert.ToDateTime(dr["wl_cot_fdev"].ToString());

                    if (dr["fec_inicio_vigencia"] != DBNull.Value)
                        cotizacionIFP.fec_inicio_vigencia = Convert.ToDateTime(dr["fec_inicio_vigencia"].ToString());

                    if (dr["fec_fin_vigencia"] != DBNull.Value)
                        cotizacionIFP.fec_fin_vigencia = Convert.ToDateTime(dr["fec_fin_vigencia"].ToString());

                    if (dr["wl_val_puni"] != DBNull.Value)
                        cotizacionIFP.val_prima_unica = Convert.ToDouble(dr["wl_val_puni"].ToString());

                    if (dr["wl_val_puam"] != DBNull.Value)
                        cotizacionIFP.val_total_cic = Convert.ToDouble(dr["wl_val_puam"].ToString());

                    if (dr["cod_moneda"] != DBNull.Value)
                        cotizacionIFP.cod_moneda = dr["cod_moneda"].ToString();

                    if (dr["val_moneda"] != DBNull.Value)
                        cotizacionIFP.val_tipo_cambio = Convert.ToDouble(dr["val_moneda"].ToString());

                    if (dr["val_mon_aju"] != DBNull.Value)
                        cotizacionIFP.val_pje_moneda = Convert.ToDouble(dr["val_mon_aju"].ToString());

                    if (dr["val_num_tope"] != DBNull.Value)
                        cotizacionIFP.num_meses_temporalidad = Convert.ToInt32(dr["val_num_tope"].ToString());

                    if (dr["wl_cot_pdif"] != DBNull.Value)
                        cotizacionIFP.num_meses_diferidos = Convert.ToInt32(dr["wl_cot_pdif"].ToString());

                    if (dr["wl_cot_pgar"] != DBNull.Value)
                        cotizacionIFP.num_meses_garantizados = Convert.ToInt32(dr["wl_cot_pgar"].ToString());

                    if (dr["num_meses_pagos_doble"] != DBNull.Value)
                        cotizacionIFP.num_meses_pagos_doble = Convert.ToInt32(dr["num_meses_pagos_doble"].ToString());

                    if (dr["val_pje_segundo_periodo"] != DBNull.Value)
                        cotizacionIFP.val_pje_segundo_periodo = Convert.ToDouble(dr["val_pje_segundo_periodo"].ToString());

                    if (dr["wl_cot_grat"] != DBNull.Value)
                        cotizacionIFP.ind_gratificacion = (dr["wl_cot_grat"].ToString() == "N") ? false : true;

                    if (dr["val_pje_devolucion"] != DBNull.Value)
                        cotizacionIFP.val_pje_dev = Convert.ToDouble(dr["val_pje_devolucion"].ToString());

                    if (dr["val_pje_devolucion_fallec"] != DBNull.Value)
                        cotizacionIFP.val_pje_dev_fallec = Convert.ToDouble(dr["val_pje_devolucion_fallec"].ToString());

                    if (dr["val_pje_fallec_ndeveng"] != DBNull.Value)
                        cotizacionIFP.val_pje_fallec_ndeveng = Convert.ToDouble(dr["val_pje_fallec_ndeveng"].ToString());


                    if (dr["ind_sepelio"] != DBNull.Value)
                        cotizacionIFP.ind_sepelio = (dr["ind_sepelio"].ToString() == "N") ? false : true;

                    if (dr["val_monto_sepelio"] != DBNull.Value)
                        cotizacionIFP.val_monto_sepelio = Convert.ToDouble(dr["val_monto_sepelio"].ToString());

                    //if (dr["val_tasa_ret_accion"] != DBNull.Value)
                    //    cotizacionIFP.val_tasa_ret_accion = Convert.ToDouble(dr["val_tasa_ret_accion"].ToString());

                    if (dr["wl_val_vtra"] != DBNull.Value)
                        cotizacionIFP.val_tasa_ret_accion = Convert.ToDouble(dr["wl_val_vtra"].ToString());

                    if (dr["wl_cot_pdco"] != DBNull.Value)
                        cotizacionIFP.val_dcom = Convert.ToDouble(dr["wl_cot_pdco"].ToString());

                    if (dr["wl_val_acom"] != DBNull.Value)
                        cotizacionIFP.val_acom = Convert.ToDouble(dr["wl_val_acom"].ToString());

                    //wl_cot_nben                   cantidad de beneficiarios - 
                    //wl_cot_tpen                   tipo de pension - 
                    //wl_cot_tcal                   tipo de calculo - 
                    //wl_cot_derc                   derecho a crecer -
                    //cot_mon_equi                  moneda equivalente - 
                    //cot_num_trea                  periodo de pago - t_perpag
                    //cot_num_frea                  tipo de ajuste - t_tipaju
                    //cot_flg_irea                  inicio de ajuste - t_iniaju
                    //wg_val_ajuste_tasa_fija       rpp_ValPar cod_parametro = 'RPEN'
                    //wl_val_tasa_venta             0
                    //wl_cot_vpen                   0
                    //wl_val_vtra                   rpp_ValPar_COTIZADOR - val_parametro rpp_AjuTRA_COTIZADOR - val_ajutra val_tasa_ajuste_tra t_tipcot - val_factor_tra
                    //wl_val_tasa_afp               rvi_tasafp
                    //wl_cot_prrt                   val_pje_rent_temp - rvi_cotiza
                    //wl_val_tgfi                   porc_capital_dolares - t_parcap
                    //wl_val_pension_minimo         val_ranfec
                    //cod_tipo_pension              rvi_propue
                    //cod_tipo_invalidez            rvi_benefi
                    //cod_tipo_producto             rvi_cotiza
                    //ind_modalidad                 rvi_cotiza
                    //ind_orden                     rvi_cotiza
                    //val_ajuste_invalidez          rvi_categoria_invalidez          
                }

                dr.NextResult();

                //2. beneficiario
                List<BeneficiarioMotorIFP> lst_beneficiarioIFP = new List<BeneficiarioMotorIFP>();

                while (dr.Read())
                {

                    BeneficiarioMotorIFP beneficiarioIFP = new BeneficiarioMotorIFP();

                    if (dr["num_correlativo"] != DBNull.Value)
                        beneficiarioIFP.item = Convert.ToInt32(dr["num_correlativo"].ToString());

                    if (dr["wl_cot_crel_c"] != DBNull.Value)
                        beneficiarioIFP.cod_parentesco = dr["wl_cot_crel_c"].ToString();

                    if (dr["fec_nacimiento"] != DBNull.Value)
                        beneficiarioIFP.fec_nacimiento = Convert.ToDateTime(dr["fec_nacimiento"].ToString());

                    if (dr["ind_invalidez"] != DBNull.Value)
                        beneficiarioIFP.ind_invalido = (dr["ind_invalidez"].ToString() == "N") ? false : true;

                    if (dr["wl_cot_csex_c"] != DBNull.Value)
                        beneficiarioIFP.cod_sexo = dr["wl_cot_csex_c"].ToString();

                    if (dr["wl_val_pje_modificado"] != DBNull.Value)
                        beneficiarioIFP.val_pje_renta = Convert.ToDouble(dr["wl_val_pje_modificado"].ToString());

                    //if (dr[""] != DBNull.Value)
                    //    beneficiarioIFP.val_pje_adicional = Convert.ToDouble(dr[""].ToString());

                    lst_beneficiarioIFP.Add(beneficiarioIFP);

                    //wl_cot_cinv_c
                    //wl_cot_nben
                    //wl_num_nacimiento
                    //wl_val_pje_base
                    //wl_val_pje_periodo_diferido
                    //wl_mes_inicio
                    //wl_num_termino
                    //wl_num_columna_tabla
                    //num_esta
                    //num_tben
                    //wl_cot_igar
                    //cod_tipo_producto
                    //max_sumporc

                }

                dr.NextResult();

                //3. parametro_ash
                ParametroAshMotorIFP parametro_ashIFP = null;

                if (dr.Read())
                {
                    parametro_ashIFP = new ParametroAshMotorIFP();

                    if (dr["val_cmor"] != DBNull.Value)
                        parametro_ashIFP.val_cmor = Convert.ToDouble(dr["val_cmor"].ToString());

                    if (dr["val_fcon"] != DBNull.Value)
                        parametro_ashIFP.val_fcon = Convert.ToDouble(dr["val_fcon"].ToString());

                    if (dr["tas_htra"] != DBNull.Value)
                        parametro_ashIFP.tas_htra = Convert.ToDouble(dr["tas_htra"].ToString());

                    if (dr["tas_htva"] != DBNull.Value)
                        parametro_ashIFP.tas_htva = Convert.ToDouble(dr["tas_htva"].ToString());

                    if (dr["tas_ltra"] != DBNull.Value)
                        parametro_ashIFP.tas_ltra = Convert.ToDouble(dr["tas_ltra"].ToString());

                    if (dr["tas_ltva"] != DBNull.Value)
                        parametro_ashIFP.tas_ltva = Convert.ToDouble(dr["tas_ltva"].ToString());

                    if (dr["val_rend"] != DBNull.Value)
                        parametro_ashIFP.val_rend = Convert.ToDouble(dr["val_rend"].ToString());

                    if (dr["tas_tgpd"] != DBNull.Value)
                        parametro_ashIFP.tas_tgpd = Convert.ToDouble(dr["tas_tgpd"].ToString());

                    if (dr["flg_ibtp"] != DBNull.Value)
                        parametro_ashIFP.flg_ibtp = Convert.ToInt32(dr["flg_ibtp"].ToString());

                    if (dr["flg_ivnt"] != DBNull.Value)
                        parametro_ashIFP.flg_ivnt = Convert.ToInt32(dr["flg_ivnt"].ToString());

                    if (dr["flg_ideb"] != DBNull.Value)
                        parametro_ashIFP.flg_ideb = Convert.ToInt32(dr["flg_ideb"].ToString());

                    if (dr["flg_iajm"] != DBNull.Value)
                        parametro_ashIFP.flg_iajm = Convert.ToInt32(dr["flg_iajm"].ToString());

                    if (dr["flg_icmo"] != DBNull.Value)
                        parametro_ashIFP.flg_icmo = Convert.ToInt32(dr["flg_icmo"].ToString());

                    if (dr["tas_timp"] != DBNull.Value)
                        parametro_ashIFP.tas_timp = Convert.ToDouble(dr["tas_timp"].ToString());

                    if (dr["tas_tsbs"] != DBNull.Value)
                        parametro_ashIFP.tas_tsbs = Convert.ToDouble(dr["tas_tsbs"].ToString());

                    if (dr["tas_ttec"] != DBNull.Value)
                        parametro_ashIFP.tas_ttec = Convert.ToDouble(dr["tas_ttec"].ToString());

                    if (dr["val_gfi1"] != DBNull.Value)
                        parametro_ashIFP.val_gfi1 = Convert.ToDouble(dr["val_gfi1"].ToString());

                    if (dr["val_gfi2"] != DBNull.Value)
                        parametro_ashIFP.val_gfi2 = Convert.ToDouble(dr["val_gfi2"].ToString());

                    if (dr["val_comi"] != DBNull.Value)
                        parametro_ashIFP.val_comi = Convert.ToDouble(dr["val_comi"].ToString());

                    if (dr["val_coba"] != DBNull.Value)
                        parametro_ashIFP.val_coba = Convert.ToDouble(dr["val_coba"].ToString());

                    if (dr["val_pumi"] != DBNull.Value)
                        parametro_ashIFP.val_pumi = Convert.ToDouble(dr["val_pumi"].ToString());

                    if (dr["num_nins"] != DBNull.Value)
                        parametro_ashIFP.num_nins = Convert.ToDouble(dr["num_nins"].ToString());

                    if (dr["num_nper"] != DBNull.Value)
                        parametro_ashIFP.num_nper = Convert.ToDouble(dr["num_nper"].ToString());

                    if (dr["val_ltit"] != DBNull.Value)
                        parametro_ashIFP.val_ltit = Convert.ToDouble(dr["val_ltit"].ToString());

                    if (dr["val_htit"] != DBNull.Value)
                        parametro_ashIFP.val_htit = Convert.ToDouble(dr["val_htit"].ToString());

                    //if (dr["ini_tra2"] != DBNull.Value)
                    //    parametro_ashIFP.ini_tra2 = Convert.ToDouble(dr["ini_tra2"].ToString());


                    if (dr["val_tinf"] != DBNull.Value)
                        parametro_ashIFP.val_tasa_inf = Convert.ToDouble(dr["val_tinf"].ToString());

                    if (dr["val_solv"] != DBNull.Value)
                        parametro_ashIFP.val_tasa_mrg_solv = Convert.ToDouble(dr["val_solv"].ToString());

                    if (dr["val_cokp"] != DBNull.Value)
                        parametro_ashIFP.val_tasa_costo_cap = Convert.ToDouble(dr["val_cokp"].ToString());

                    if (dr["val_vtax"] != DBNull.Value)
                        parametro_ashIFP.val_vtax = Convert.ToDouble(dr["val_vtax"].ToString());


                    //cod_moneda                        
                    //cod_tipo_producto
                    //val_moneda
                    //ind_modalidad
                    //ind_orden
                    //pje_rent
                    //pje_dev

                }

                dr.NextResult();

                //4. factores
                //List<FactorCotizacionMotorIFP> lst_factor_cotizacionIFP = new List<FactorCotizacionMotorIFP>();

                /*while (dr.Read())
                {

                    FactorCotizacionMotorIFP factor_cotizacionIFP = new FactorCotizacionMotorIFP();

                    if (dr["num_mes"] != DBNull.Value)
                        factor_cotizacionIFP.num_mes = Convert.ToInt64(dr["num_mes"].ToString());

                    if (dr["fec_periodo"] != DBNull.Value)
                        factor_cotizacionIFP.fec_periodo = Convert.ToDateTime(dr["fec_periodo"].ToString());

                    if (dr["num_factor"] != DBNull.Value)
                        factor_cotizacionIFP.val_factor = Convert.ToDouble(dr["num_factor"].ToString());

                    if (dr["num_correlativo"] != DBNull.Value)
                        factor_cotizacionIFP.num_correlativo = Convert.ToInt64(dr["num_correlativo"].ToString());

                    lst_factor_cotizacionIFP.Add(factor_cotizacionIFP);

                }*/

                cotizacionIFP.beneficiarios = lst_beneficiarioIFP;
                cotizacionIFP.parametro_ash = parametro_ashIFP;
                //cotizacionIFP.factores_documento = lst_factor_cotizacionIFP.FindAll(f => f.num_correlativo == cotizacionIFP.num_correlativo);

                parametrosIFP.cotizacion = cotizacionIFP;

            }

            return parametrosIFP;
        }

        public List<ParametrosMotorIFP> ObtenerParametroCotizaciones(string num_solicitud, DateTime fec_cotizacion, string usuario)
        {
            List<ParametrosMotorIFP> lstParametroIFP = new List<ParametrosMotorIFP>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_carpr_cotizacion_ifp", num_solicitud, fec_cotizacion, usuario);

            dbc.CommandTimeout = 180;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {

                while (dr.Read())
                {
                    //1. cotizacion 
                    CotizacionMotorIFP cotizacionIFP = new CotizacionMotorIFP();
                    ParametrosMotorIFP parametrosIFP = new ParametrosMotorIFP();

                    //cotizacionIFP = new CotizacionMotorIFP();

                    if (dr["wl_num_solicitud"] != DBNull.Value)
                        cotizacionIFP.num_solicitud = dr["wl_num_solicitud"].ToString();

                    if (dr["cod_plan"] != DBNull.Value)
                        cotizacionIFP.plan = new Plan{ Id = dr["cod_plan"].ToString() };

                    if (dr["wl_num_corr_cotiza"] != DBNull.Value)
                        cotizacionIFP.num_correlativo = Convert.ToInt64(dr["wl_num_corr_cotiza"].ToString());

                    if (dr["wl_cot_fcot"] != DBNull.Value)
                        cotizacionIFP.fec_cotizacion = Convert.ToDateTime(dr["wl_cot_fcot"].ToString());

                    if (dr["wl_cot_fdev"] != DBNull.Value)
                        cotizacionIFP.fec_devengue = Convert.ToDateTime(dr["wl_cot_fdev"].ToString());

                    if (dr["fec_inicio_vigencia"] != DBNull.Value)
                        cotizacionIFP.fec_inicio_vigencia = Convert.ToDateTime(dr["fec_inicio_vigencia"].ToString());

                    if (dr["fec_fin_vigencia"] != DBNull.Value)
                        cotizacionIFP.fec_fin_vigencia = Convert.ToDateTime(dr["fec_fin_vigencia"].ToString());

                    if (dr["wl_val_puni"] != DBNull.Value)
                        cotizacionIFP.val_prima_unica = Convert.ToDouble(dr["wl_val_puni"].ToString());

                    if (dr["wl_val_puam"] != DBNull.Value)
                        cotizacionIFP.val_total_cic = Convert.ToDouble(dr["wl_val_puam"].ToString());

                    if (dr["cod_moneda"] != DBNull.Value)
                        cotizacionIFP.cod_moneda = dr["cod_moneda"].ToString();

                    if (dr["val_moneda"] != DBNull.Value)
                        cotizacionIFP.val_tipo_cambio = Convert.ToDouble(dr["val_moneda"].ToString());

                    if (dr["val_mon_aju"] != DBNull.Value)
                        cotizacionIFP.val_pje_moneda = Convert.ToDouble(dr["val_mon_aju"].ToString());

                    if (dr["val_num_tope"] != DBNull.Value)
                        cotizacionIFP.num_meses_temporalidad = Convert.ToInt32(dr["val_num_tope"].ToString());

                    if (dr["wl_cot_pdif"] != DBNull.Value)
                        cotizacionIFP.num_meses_diferidos = Convert.ToInt32(dr["wl_cot_pdif"].ToString());

                    if (dr["wl_cot_pgar"] != DBNull.Value)
                        cotizacionIFP.num_meses_garantizados = Convert.ToInt32(dr["wl_cot_pgar"].ToString());

                    if (dr["num_meses_pagos_doble"] != DBNull.Value)
                        cotizacionIFP.num_meses_pagos_doble = Convert.ToInt32(dr["num_meses_pagos_doble"].ToString());

                    if (dr["val_pje_segundo_periodo"] != DBNull.Value)
                        cotizacionIFP.val_pje_segundo_periodo = Convert.ToDouble(dr["val_pje_segundo_periodo"].ToString());

                    if (dr["wl_cot_grat"] != DBNull.Value)
                        cotizacionIFP.ind_gratificacion = (dr["wl_cot_grat"].ToString() == "N") ? false : true;

                    if (dr["val_pje_devolucion"] != DBNull.Value)
                        cotizacionIFP.val_pje_dev = Convert.ToDouble(dr["val_pje_devolucion"].ToString());

                    if (dr["val_pje_devolucion_fallec"] != DBNull.Value)
                        cotizacionIFP.val_pje_dev_fallec = Convert.ToDouble(dr["val_pje_devolucion_fallec"]);

                    if (dr["val_pje_fallec_ndeveng"] != DBNull.Value)
                        cotizacionIFP.val_pje_fallec_ndeveng = Convert.ToDouble(dr["val_pje_fallec_ndeveng"].ToString());

                    if (dr["ind_sepelio"] != DBNull.Value)
                        cotizacionIFP.ind_sepelio = (dr["ind_sepelio"].ToString() == "N") ? false : true;

                    if (dr["val_monto_sepelio"] != DBNull.Value)
                        cotizacionIFP.val_monto_sepelio = Convert.ToDouble(dr["val_monto_sepelio"].ToString());

                    //if (dr["val_tasa_ret_accion"] != DBNull.Value)
                    //    cotizacionIFP.val_tasa_ret_accion = Convert.ToDouble(dr["val_tasa_ret_accion"].ToString());

                    if (dr["wl_val_vtra"] != DBNull.Value)
                        cotizacionIFP.val_tasa_ret_accion = Convert.ToDouble(dr["wl_val_vtra"].ToString());

                    if (dr["wl_cot_pdco"] != DBNull.Value)
                        cotizacionIFP.val_dcom = Convert.ToDouble(dr["wl_cot_pdco"].ToString());

                    if (dr["wl_val_acom"] != DBNull.Value)
                        cotizacionIFP.val_acom = Convert.ToDouble(dr["wl_val_acom"].ToString());

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        cotizacionIFP.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();

                    if (dr["cob_adic_dev"] != DBNull.Value)
                        cotizacionIFP.cob_adic_dev = Convert.ToInt32(dr["cob_adic_dev"].ToString());

                    if (dr["cob_adic_cap_fallec"] != DBNull.Value)
                        cotizacionIFP.cob_adic_cap_fallec = Convert.ToInt32(dr["cob_adic_cap_fallec"].ToString());

                    if (dr["cod_estado_cotizacion"] != DBNull.Value)
                        cotizacionIFP.cod_estado_cotizacion = dr["cod_estado_cotizacion"].ToString();

                    //wl_cot_nben                   cantidad de beneficiarios - 
                    //wl_cot_tpen                   tipo de pension - 
                    //wl_cot_tcal                   tipo de calculo - 
                    //wl_cot_derc                   derecho a crecer -
                    //cot_mon_equi                  moneda equivalente - 
                    //cot_num_trea                  periodo de pago - t_perpag
                    //cot_num_frea                  tipo de ajuste - t_tipaju
                    //cot_flg_irea                  inicio de ajuste - t_iniaju
                    //wg_val_ajuste_tasa_fija       rpp_ValPar cod_parametro = 'RPEN'
                    //wl_val_tasa_venta             0
                    //wl_cot_vpen                   0
                    //wl_val_vtra                   rpp_ValPar_COTIZADOR - val_parametro rpp_AjuTRA_COTIZADOR - val_ajutra val_tasa_ajuste_tra t_tipcot - val_factor_tra
                    //wl_val_tasa_afp               rvi_tasafp
                    //wl_cot_prrt                   val_pje_rent_temp - rvi_cotiza
                    //wl_val_tgfi                   porc_capital_dolares - t_parcap
                    //wl_val_pension_minimo         val_ranfec
                    //cod_tipo_pension              rvi_propue
                    //cod_tipo_invalidez            rvi_benefi
                    //cod_tipo_producto             rvi_cotiza
                    //ind_modalidad                 rvi_cotiza
                    //ind_orden                     rvi_cotiza
                    //val_ajuste_invalidez          rvi_categoria_invalidez          

                    parametrosIFP.cotizacion = cotizacionIFP;

                    lstParametroIFP.Add(parametrosIFP);

                }

                dr.NextResult();

                //2. beneficiario
                List<BeneficiarioMotorIFP> lst_beneficiarioIFP = new List<BeneficiarioMotorIFP>();
                ParametrosMotorIFP par = null;
                while (dr.Read())
                {

                    BeneficiarioMotorIFP beneficiarioIFP = new BeneficiarioMotorIFP();

                    if (dr["num_correlativo"] != DBNull.Value)
                        beneficiarioIFP.item = Convert.ToInt32(dr["num_correlativo"].ToString());

                    if (dr["wl_cot_crel_c"] != DBNull.Value)
                        beneficiarioIFP.cod_parentesco = dr["wl_cot_crel_c"].ToString();

                    if (dr["fec_nacimiento"] != DBNull.Value)
                        beneficiarioIFP.fec_nacimiento = Convert.ToDateTime(dr["fec_nacimiento"].ToString());

                    if (dr["ind_invalidez"] != DBNull.Value)
                        beneficiarioIFP.ind_invalido = (dr["ind_invalidez"].ToString() == "N") ? false : true;

                    if (dr["wl_cot_csex_c"] != DBNull.Value)
                        beneficiarioIFP.cod_sexo = dr["wl_cot_csex_c"].ToString();

                    if (dr["wl_val_pje_modificado"] != DBNull.Value)
                        beneficiarioIFP.val_pje_renta = Convert.ToDouble(dr["wl_val_pje_modificado"].ToString());

                    if (dr["wl_num_corr_cotiza"] != DBNull.Value)
                        beneficiarioIFP.Cotizacion = new CotizacionMotorIFP { num_correlativo = Convert.ToInt64(dr["wl_num_corr_cotiza"].ToString()) };

                    //if (dr[""] != DBNull.Value)
                    //    beneficiarioIFP.val_pje_adicional = Convert.ToDouble(dr[""].ToString());

                    if (dr["num_identificacion"] != DBNull.Value)
                        beneficiarioIFP.num_identificacion = dr["num_identificacion"].ToString();

                    lst_beneficiarioIFP.Add(beneficiarioIFP);

                    //wl_cot_cinv_c
                    //wl_cot_nben
                    //wl_num_nacimiento
                    //wl_val_pje_base
                    //wl_val_pje_periodo_diferido
                    //wl_mes_inicio
                    //wl_num_termino
                    //wl_num_columna_tabla
                    //num_esta
                    //num_tben
                    //wl_cot_igar
                    //cod_tipo_producto
                    //max_sumporc

                    par = lstParametroIFP.Find(p => p.cotizacion.num_correlativo == Convert.ToInt32(dr["wl_num_corr_cotiza"].ToString()));
                    par.cotizacion.beneficiarios = lst_beneficiarioIFP.FindAll(p => p.Cotizacion.num_correlativo == Convert.ToInt32(dr["wl_num_corr_cotiza"].ToString()));
                }

                dr.NextResult();

                //3. parametro_ash
                while (dr.Read())
                {
                    ParametroAshMotorIFP parametro_ashIFP = null;
                    parametro_ashIFP = new ParametroAshMotorIFP();


                    if (dr["val_cmor"] != DBNull.Value)
                        parametro_ashIFP.val_cmor = Convert.ToDouble(dr["val_cmor"].ToString());

                    if (dr["val_fcon"] != DBNull.Value)
                        parametro_ashIFP.val_fcon = Convert.ToDouble(dr["val_fcon"].ToString());

                    if (dr["tas_htra"] != DBNull.Value)
                        parametro_ashIFP.tas_htra = Convert.ToDouble(dr["tas_htra"].ToString());

                    if (dr["tas_htva"] != DBNull.Value)
                        parametro_ashIFP.tas_htva = Convert.ToDouble(dr["tas_htva"].ToString());

                    if (dr["tas_ltra"] != DBNull.Value)
                        parametro_ashIFP.tas_ltra = Convert.ToDouble(dr["tas_ltra"].ToString());

                    if (dr["tas_ltva"] != DBNull.Value)
                        parametro_ashIFP.tas_ltva = Convert.ToDouble(dr["tas_ltva"].ToString());

                    if (dr["val_rend"] != DBNull.Value)
                        parametro_ashIFP.val_rend = Convert.ToDouble(dr["val_rend"].ToString());

                    if (dr["tas_tgpd"] != DBNull.Value)
                        parametro_ashIFP.tas_tgpd = Convert.ToDouble(dr["tas_tgpd"].ToString());

                    if (dr["flg_ibtp"] != DBNull.Value)
                        parametro_ashIFP.flg_ibtp = Convert.ToInt32(dr["flg_ibtp"].ToString());

                    if (dr["flg_ivnt"] != DBNull.Value)
                        parametro_ashIFP.flg_ivnt = Convert.ToInt32(dr["flg_ivnt"].ToString());

                    if (dr["flg_ideb"] != DBNull.Value)
                        parametro_ashIFP.flg_ideb = Convert.ToInt32(dr["flg_ideb"].ToString());

                    if (dr["flg_iajm"] != DBNull.Value)
                        parametro_ashIFP.flg_iajm = Convert.ToInt32(dr["flg_iajm"].ToString());

                    if (dr["flg_icmo"] != DBNull.Value)
                        parametro_ashIFP.flg_icmo = Convert.ToInt32(dr["flg_icmo"].ToString());

                    if (dr["tas_timp"] != DBNull.Value)
                        parametro_ashIFP.tas_timp = Convert.ToDouble(dr["tas_timp"].ToString());

                    if (dr["tas_tsbs"] != DBNull.Value)
                        parametro_ashIFP.tas_tsbs = Convert.ToDouble(dr["tas_tsbs"].ToString());

                    if (dr["tas_ttec"] != DBNull.Value)
                        parametro_ashIFP.tas_ttec = Convert.ToDouble(dr["tas_ttec"].ToString());

                    if (dr["val_gfi1"] != DBNull.Value)
                        parametro_ashIFP.val_gfi1 = Convert.ToDouble(dr["val_gfi1"].ToString());

                    if (dr["val_gfi2"] != DBNull.Value)
                        parametro_ashIFP.val_gfi2 = Convert.ToDouble(dr["val_gfi2"].ToString());

                    if (dr["val_comi"] != DBNull.Value)
                        parametro_ashIFP.val_comi = Convert.ToDouble(dr["val_comi"].ToString());

                    if (dr["val_coba"] != DBNull.Value)
                        parametro_ashIFP.val_coba = Convert.ToDouble(dr["val_coba"].ToString());

                    if (dr["val_pumi"] != DBNull.Value)
                        parametro_ashIFP.val_pumi = Convert.ToDouble(dr["val_pumi"].ToString());

                    if (dr["num_nins"] != DBNull.Value)
                        parametro_ashIFP.num_nins = Convert.ToDouble(dr["num_nins"].ToString());

                    if (dr["num_nper"] != DBNull.Value)
                        parametro_ashIFP.num_nper = Convert.ToDouble(dr["num_nper"].ToString());

                    if (dr["val_ltit"] != DBNull.Value)
                        parametro_ashIFP.val_ltit = Convert.ToDouble(dr["val_ltit"].ToString());

                    if (dr["val_htit"] != DBNull.Value)
                        parametro_ashIFP.val_htit = Convert.ToDouble(dr["val_htit"].ToString());

                    //if (dr["ini_tra2"] != DBNull.Value)
                    //    parametro_ashIFP.ini_tra2 = Convert.ToDouble(dr["ini_tra2"].ToString());
                    
                    if (dr["val_tinf"] != DBNull.Value)
                        parametro_ashIFP.val_tasa_inf = Convert.ToDouble(dr["val_tinf"].ToString());

                    if (dr["val_solv"] != DBNull.Value)
                        parametro_ashIFP.val_tasa_mrg_solv = Convert.ToDouble(dr["val_solv"].ToString());

                    if (dr["val_cokp"] != DBNull.Value)
                        parametro_ashIFP.val_tasa_costo_cap = Convert.ToDouble(dr["val_cokp"].ToString());

                    if (dr["val_vtax"] != DBNull.Value)
                        parametro_ashIFP.val_vtax = Convert.ToDouble(dr["val_vtax"].ToString());

                    //cod_moneda                        
                    //cod_tipo_producto
                    //val_moneda
                    //ind_modalidad
                    //ind_orden
                    //pje_rent
                    //pje_dev

                    var cot = lstParametroIFP.Find(p => p.cotizacion.num_correlativo == Convert.ToInt64(dr["num_correlativo"].ToString()));
                    cot.cotizacion.parametro_ash = parametro_ashIFP;

                }

                dr.NextResult();

                //4. factores
                //List<FactorCotizacionMotorIFP> lst_factor_cotizacionIFP = new List<FactorCotizacionMotorIFP>();

                /*while (dr.Read())
                {

                    FactorCotizacionMotorIFP factor_cotizacionIFP = new FactorCotizacionMotorIFP();

                    if (dr["num_mes"] != DBNull.Value)
                        factor_cotizacionIFP.num_mes = Convert.ToInt64(dr["num_mes"].ToString());

                    if (dr["fec_periodo"] != DBNull.Value)
                        factor_cotizacionIFP.fec_periodo = Convert.ToDateTime(dr["fec_periodo"].ToString());

                    if (dr["num_factor"] != DBNull.Value)
                        factor_cotizacionIFP.val_factor = Convert.ToDouble(dr["num_factor"].ToString());

                    if (dr["num_correlativo"] != DBNull.Value)
                        factor_cotizacionIFP.num_correlativo = Convert.ToInt64(dr["num_correlativo"].ToString());

                    lst_factor_cotizacionIFP.Add(factor_cotizacionIFP);

                    var fact = lstParametroIFP.Find(p => p.cotizacion.num_correlativo == Convert.ToInt64(dr["num_correlativo"].ToString()));
                    fact.cotizacion.factores_documento = lst_factor_cotizacionIFP.FindAll(p => p.num_correlativo == Convert.ToInt64(dr["num_correlativo"].ToString()));

                }*/

                //cotizacionIFP.beneficiarios = lst_beneficiarioIFP;
                //cotizacionIFP.parametro_ash = parametro_ashIFP;
                //cotizacionIFP.factores_cotizaciones = lst_factor_cotizacionIFP.FindAll(f => f.num_correlativo == cotizacionIFP.num_correlativo);

                //parametrosIFP.cotizacion = cotizacionIFP;

            }

            return lstParametroIFP;

            //return parametrosIFP;
        }

        public int CantidadSolicitudes(string cuspp, string moneda, double val_mto_prima_unica)
        {

            try
            {
                int Cantidad = 0;
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_consultar_cantidad_solicitudes_IFP", cuspp, val_mto_prima_unica, moneda);

                dbc.CommandTimeout = 180;

                using (IDataReader dr = db.ExecuteReader(dbc))
                {
                    while (dr.Read())
                    {
                        Cantidad = Convert.ToInt32(dr["val_Cantidad"]);
                    }
                }
                return Cantidad;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Registrar(ref SolicitudIFP entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_solicitud_ifp");

                dbc.CommandTimeout = 180;

                db.AddInParameter(dbc, "@xml_solicitud", DbType.String, entity.XMLSolicitud());
                db.AddInParameter(dbc, "@xml_beneficiario", DbType.String, entity.XMLBeneficiario());
                db.AddInParameter(dbc, "@xml_cotizacion", DbType.String, entity.XMLCotizacion());
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, entity.Usuario.NombreUsuario);
                db.AddInParameter(dbc, "@wl_cod_rol", DbType.String, entity.Usuario.Rol);
                db.AddInParameter(dbc, "@xml_cobertura_adicional", DbType.String, entity.XMLCoberturaAdicional());
                db.AddOutParameter(dbc, "@wl_num_solicitud", DbType.String, 20);

                db.ExecuteNonQuery(dbc);

                entity.Id = db.GetParameterValue(dbc, "@wl_num_solicitud").ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(ref SolicitudIFP entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_solicitud_ifp");

                dbc.CommandTimeout = 180;

                db.AddInParameter(dbc, "@xml_solicitud", DbType.String, entity.XMLSolicitud());
                db.AddInParameter(dbc, "@xml_beneficiario", DbType.String, entity.XMLBeneficiario());
                db.AddInParameter(dbc, "@xml_cotizacion", DbType.String, entity.XMLCotizacion());
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, entity.Usuario.NombreUsuario);
                db.AddInParameter(dbc, "@wl_cod_rol", DbType.String, entity.Usuario.Rol);
                db.AddInParameter(dbc, "@xml_cobertura_adicional", DbType.String, entity.XMLCoberturaAdicional());
                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.Id);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RegistrarPjeBeneficiarios(string idSolicitud, string correlativos, string xml_pje, string xml_costo, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.SP_RENVI_INS_PJEBENEFICIARIOS");

                dbc.CommandTimeout = 180;

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, idSolicitud);
                db.AddInParameter(dbc, "@wl_num_correlativo_cotizacion", DbType.String, correlativos);
                db.AddInParameter(dbc, "@wl_empdata", DbType.String, xml_pje);
                db.AddInParameter(dbc, "@wl_empdata_costo", DbType.String, xml_costo);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RegistrarCotiza(string xml_cotiza, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_cotizacion_ifp");

                dbc.CommandTimeout = 180;

                db.AddInParameter(dbc, "@wl_empdata", DbType.String, xml_cotiza);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SolicitudIFP ObtenerDatos(string idSolicitud)
        {
            SolicitudIFP SolicitudIFP = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_obtener_solicitud_IFP", idSolicitud);

            dbc.CommandTimeout = 180;            

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                // Datos de Solicitud
                if (dr.Read())
                {
                    SolicitudIFP = new SolicitudIFP();
                    SolicitudIFP.Id = dr["num_solicitud"].ToString();
                    if (dr["fec_solicitud"] != DBNull.Value)
                        SolicitudIFP.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    if (dr["fec_cotizacion"] != DBNull.Value)
                        SolicitudIFP.FechaCotizacion = Convert.ToDateTime(dr["fec_cotizacion"]);
                    SolicitudIFP.Afiliado = new Afiliado
                    {
                        CUSPP = dr["num_cuispp"].ToString(),
                        CorreoElectronico = (dr["gls_mail"] != DBNull.Value && dr["gls_mail"].ToString() != string.Empty) ? dr["gls_mail"].ToString() : null,
                        AFP = new AFP { Id = dr["cod_afp"].ToString() },
                        NumeroIdentificacion = dr["num_dociden"].ToString(),
                        Nombre = dr["Nombre"].ToString(),
                        ApellidoPaterno = dr["ApellidoPaterno"].ToString(),
                        ApellidoMaterno = dr["ApellidoMaterno"].ToString()
                    };

                    //SolicitudIFP.Afiliado.AFP = new AFP { Id = dr["cod_afp"].ToString() };
                    SolicitudIFP.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                    SolicitudIFP.TipoPension = new TipoPension { Id = dr["cod_tipo_pension"].ToString() };
                    if (dr["fec_devengue"] != DBNull.Value)
                        SolicitudIFP.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);

                    if (dr["fec_ult_actualizacion"] != DBNull.Value)
                        SolicitudIFP.FechaUltimaActualizacion = Convert.ToDateTime(dr["fec_ult_actualizacion"]);

                    SolicitudIFP.PrimaUnica = Convert.ToDouble(dr["val_mto_cta_individual"]);

                    SolicitudIFP.TipoCambio = 0;
                    if (dr["val_tasa_cambio"] != DBNull.Value)
                        SolicitudIFP.TipoCambio = Convert.ToDouble(dr["val_tasa_cambio"]);

                    SolicitudIFP.FactorTasa = dr["cod_factor_tasa"].ToString();


                    SolicitudIFP.Categoria = new Categoria { Id = dr["cod_categoria"].ToString() };

                    SolicitudIFP.Agente = new Agente { Id = dr["num_agente"].ToString(), IdCartera = dr["cod_cartera"].ToString() };

                    if (dr["cod_moneda_cta_indiv"] != DBNull.Value && dr["cod_moneda_cta_indiv"].ToString() != string.Empty)
                        SolicitudIFP.MonedaPrimaUnica = new Moneda
                        {
                            Id = Convert.ToString(dr["cod_moneda_cta_indiv"])
                        };

                    if (dr["fec_vigencia"] != DBNull.Value)
                        SolicitudIFP.FechaVigencia = Convert.ToDateTime(dr["fec_vigencia"]);

                    if (dr["num_poliza"] != DBNull.Value)
                        SolicitudIFP.NumeroPoliza = Convert.ToInt32(dr["num_poliza"]);

                    SolicitudIFP.CodigoEstado = 0;
                    if (dr["cod_estado_rpp"] != DBNull.Value)
                        SolicitudIFP.CodigoEstado = Convert.ToInt32(dr["cod_estado_rpp"]);

                    SolicitudIFP.EstadoSolicitud = "";
                    if (dr["gls_estado_solicitud"] != DBNull.Value)
                        SolicitudIFP.EstadoSolicitud = dr["gls_estado_solicitud"].ToString();

                    SolicitudIFP.CodigoEstadoPoliza = "";
                    if (dr["cod_estado_poliza"] != DBNull.Value)
                        SolicitudIFP.CodigoEstadoPoliza = dr["cod_estado_poliza"].ToString();

                    SolicitudIFP.EstadoPoliza = "";
                    if (dr["gls_estado_poliza"] != DBNull.Value)
                        SolicitudIFP.EstadoPoliza = dr["gls_estado_poliza"].ToString();

                    SolicitudIFP.CausalPoliza = new CausalPoliza();
                    if (dr["cod_causal_estado_poliza"] != DBNull.Value)
                        SolicitudIFP.CausalPoliza = new CausalPoliza
                        {
                            Id = dr["cod_causal_estado_poliza"].ToString(),
                            NombreLargo = dr["gls_causal_estado_poliza"].ToString()
                        };

                    SolicitudIFP.CodigoEstadoPlaft = 0;
                    if (dr["cod_estado_plaft"] != DBNull.Value)
                        SolicitudIFP.CodigoEstadoPlaft = Convert.ToInt32(dr["cod_estado_plaft"]);

                    SolicitudIFP.EstadoSolicitudPlaft = "";
                    if (dr["gls_estado_solicitud_plaft"] != DBNull.Value)
                        SolicitudIFP.EstadoSolicitudPlaft = dr["gls_estado_solicitud_plaft"].ToString();

                    SolicitudIFP.CodCanalDistribucion = "";
                    if (dr["cod_canal_distribucion"] != DBNull.Value)
                        SolicitudIFP.CodCanalDistribucion = dr["cod_canal_distribucion"].ToString();

                }

                dr.NextResult();

                // Cotizaciones
                SolicitudIFP.Cotizaciones = new List<CotizacionIFP>();
                int item = 0;
                while (dr.Read())
                {
                    CotizacionIFP cotizacion = new CotizacionIFP();
                    cotizacion.Correlativo = Convert.ToInt64(dr["num_correlativo"]);
                    cotizacion.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString() };
                    cotizacion.Producto = new Producto { Id = dr["cod_tipo_producto"].ToString(), Nombre = dr["gls_tipo_producto"].ToString() };
                    cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["val_per_garantizado"]);
                    cotizacion.AjusteTRA = Convert.ToDouble(dr["val_Tasa_ajuste_tra"]);
                    cotizacion.Item = item;

                    if (dr["val_pen_cia"] != DBNull.Value)
                        cotizacion.PensionCia = Convert.ToDouble(dr["val_pen_cia"]);
                    if (dr["val_pen_cia_mo"] != DBNull.Value)
                        cotizacion.PensionCiaMO = Convert.ToDouble(dr["val_pen_cia_mo"]);

                    if (dr["val_tasa_venta_ash"] != DBNull.Value)
                        cotizacion.TasaVenta = Convert.ToDouble(dr["val_tasa_venta_ash"]);
                    if (dr["val_tasa_int_vit"] != DBNull.Value)
                        cotizacion.TasaVentaSbs = Convert.ToDouble(dr["val_tasa_int_vit"]);
                    if (dr["val_tra"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionista = Convert.ToDouble(dr["val_tra"]);

                    if (dr["val_per_temporal"] != DBNull.Value)
                        cotizacion.PagoDoble = Convert.ToDouble(dr["val_per_temporal"]);

                    if (dr["val_pje_rent_temp"] != DBNull.Value)
                        cotizacion.PjePagoDoble = Convert.ToDouble(dr["val_pje_rent_temp"]);

                    cotizacion.IndGastoSepelio = "N";
                    if (dr["ind_gasto_sepelio"] != DBNull.Value)
                        cotizacion.IndGastoSepelio = dr["ind_gasto_sepelio"].ToString();

                    if (dr["val_pje_dev"] != DBNull.Value)
                        cotizacion.ValPjeDev = Convert.ToDouble(dr["val_pje_dev"]);

                    if (dr["val_1era_prima_is"] != DBNull.Value)
                    {
                        cotizacion.Pension2doTramo = Convert.ToDouble(dr["val_1era_prima_is"]);
                        cotizacion.Pension2doTramoSinAjuste = cotizacion.PensionCiaMO * (cotizacion.PjePagoDoble / 100);
                    }

                    if (dr["ind_cotiza"] != DBNull.Value)
                        cotizacion.IndCotiza = dr["ind_cotiza"].ToString();

                    if (dr["ind_error_cotiza"] != DBNull.Value)
                        cotizacion.IndErrorCotiza = Convert.ToInt32(dr["ind_error_cotiza"]);

                    if (dr["val_mon_aju"] != DBNull.Value)
                        cotizacion.ValMonAju = Convert.ToDouble(dr["val_mon_aju"]);
                    else
                        cotizacion.ValMonAju = -1;

                    cotizacion.EstadoCotizacion = dr["cod_estado_cotizacion"].ToString();
                    cotizacion.IndSeleccionada = dr["ind_cotizacion_seleccionada"].ToString();

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        cotizacion.Temporalidad = new Temporalidad { Id = dr["cod_tipo_temporalidad"].ToString(), Anhos = Convert.ToInt32(dr["val_anos"]) };
                    else
                        cotizacion.Temporalidad = new Temporalidad { Id = "", Anhos = 0 };

                    if (dr["cod_plan"] != DBNull.Value)
                        cotizacion.Plan = new Plan { Id = dr["cod_plan"].ToString() };
                    else
                        cotizacion.Plan = new Plan { Id = "" };

                    cotizacion.CobAdicDevengue = 0;
                    if (dr["cob_adic_dev"] != DBNull.Value)
                        cotizacion.CobAdicDevengue = Convert.ToDouble(dr["cob_adic_dev"]);

                    cotizacion.CobAdicCapFallecimiento = 0;
                    if (dr["cob_adic_cap_fallec"] != DBNull.Value)
                        cotizacion.CobAdicCapFallecimiento = Convert.ToDouble(dr["cob_adic_cap_fallec"]);

                    cotizacion.ValPjeDevFallec = 0;
                    if (dr["val_pje_dev_fallec"] != DBNull.Value)
                        cotizacion.ValPjeDevFallec = Convert.ToDouble(dr["val_pje_dev_fallec"]);

                    cotizacion.ValPerDiferido = 0;
                    if (dr["val_per_diferido"] != DBNull.Value)
                        cotizacion.ValPerDiferido = Convert.ToDouble(dr["val_per_diferido"]);

                    cotizacion.ValPjeDCOM = 0;
                    if (dr["val_dcom"] != DBNull.Value)
                        cotizacion.ValPjeDCOM = Convert.ToDouble(dr["val_dcom"]);

                    cotizacion.TasaRetornoAccionistaMinima = 0;
                    if (dr["val_tra_min"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionistaMinima = Convert.ToDouble(dr["val_tra_min"]);

                    cotizacion.ValPjeFallecNoDeveng = 0;
                    if (dr["val_pje_fallec_ndeveng"] != DBNull.Value)
                        cotizacion.ValPjeFallecNoDeveng = Convert.ToDouble(dr["val_pje_fallec_ndeveng"]);

                    cotizacion.ValPjeCACy = 0;
                    if (dr["ValPjeCACy"] != DBNull.Value)
                        cotizacion.ValPjeCACy = Convert.ToDouble(dr["ValPjeCACy"]);

                    cotizacion.ValPjeCAPa = 0;
                    if (dr["ValPjeCAPa"] != DBNull.Value)
                        cotizacion.ValPjeCAPa = Convert.ToDouble(dr["ValPjeCAPa"]);

                    cotizacion.ValPjeCAMa = 0;
                    if (dr["ValPjeCAMa"] != DBNull.Value)
                        cotizacion.ValPjeCAMa = Convert.ToDouble(dr["ValPjeCAMa"]);

                    cotizacion.ValPjeCATotal = "0%";
                    if (dr["ValPjeCATotal"] != DBNull.Value)
                        cotizacion.ValPjeCATotal = dr["ValPjeCATotal"] + "%";

                    cotizacion.ValPrimeraRentaIS = 0;
                    if (dr["ValPraRentaIS"] != DBNull.Value)
                        cotizacion.ValPrimeraRentaIS = Convert.ToDouble(dr["ValPraRentaIS"]);

                    cotizacion.IndRescate = false;
                    if (dr["ind_rescate"] != DBNull.Value)
                        cotizacion.IndRescate = Convert.ToBoolean(dr["ind_rescate"]);

                    SolicitudIFP.Cotizaciones.Add(cotizacion);
                    item++;
                }
                dr.NextResult();

                // Beneficiarios
                SolicitudIFP.Beneficiarios = new List<GrupoFamiliar>();
                while (dr.Read())
                {
                    GrupoFamiliar beneficiario = new GrupoFamiliar();
                    beneficiario.Id = Convert.ToInt64(dr["num_correlativo"]);
                    beneficiario.ApellidoPaterno = dr["ape_paterno"].ToString();
                    beneficiario.ApellidoMaterno = dr["ape_materno"].ToString();
                    beneficiario.Nombre = dr["nom_persona"].ToString();
                    beneficiario.Parentesco = new Parentesco { Id = dr["cod_parentezco"].ToString(), Nombre = dr["gls_parentezco"].ToString() };
                    beneficiario.Sexo = Convert.ToChar(dr["cod_sexo"]);
                    if (dr["fec_nacimiento"] != DBNull.Value)
                        beneficiario.FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);
                    beneficiario.Invalido = (dr["ind_invalidez"].ToString() == "S") ? true : false;
                    beneficiario.TipoInvalidez = new TipoInvalidez { Id = dr["cod_tipo_invalidez"].ToString(), Nombre = dr["gls_tipo_invalidez"].ToString() };
                    if (dr["fec_invalidez"] != DBNull.Value)
                        beneficiario.FechaInvalidez = Convert.ToDateTime(dr["fec_invalidez"]);
                    if (dr["id_grupo_familiar"] != DBNull.Value)
                        beneficiario.IdGrupoFamiliar = Convert.ToInt64(dr["id_grupo_familiar"]);

                    if (dr["cod_tipo_identificacion"] != DBNull.Value)
                        beneficiario.Identificacion = new Identificacion() { IdTipo = dr["cod_tipo_identificacion"].ToString(), Numero = dr["num_identificacion"].ToString() };

                    if (dr["cod_Nacionalidad"] != DBNull.Value)
                        beneficiario.Nacionalidad = new Temporal() { codigo = dr["cod_Nacionalidad"].ToString() };

                    if (dr["cod_profesion"] != DBNull.Value)
                        beneficiario.Profesion = new Temporal() { codigo = dr["cod_profesion"].ToString() };

                    if (dr["cod_residencia"] != DBNull.Value)
                        beneficiario.Residencia = new Temporal() { codigo = dr["cod_residencia"].ToString() };

                    if (dr["ind_pep"] != DBNull.Value)
                        beneficiario.ind_PEP = (dr["ind_pep"].ToString() == "S") ? true : false;

                    if (dr["ind_sujeto_obligado"] != DBNull.Value)
                        beneficiario.ind_SujetoObligado = (dr["ind_sujeto_obligado"].ToString() == "S") ? true : false;

                    if (dr["val_pje_beneficiario"] != DBNull.Value)
                        beneficiario.ValPjeRenta = Convert.ToDouble(dr["val_pje_beneficiario"]);

                    if (dr["cod_tipo_periodo_beneficiario"] != DBNull.Value)
                        beneficiario.IdTipoPeriodoBeneficiario = Convert.ToInt32(dr["cod_tipo_periodo_beneficiario"]);

                    if (dr["val_pje_base"] != DBNull.Value)
                        beneficiario.ValPjeBeneficiario = Convert.ToDouble(dr["val_pje_base"]);

                    SolicitudIFP.Beneficiarios.Add(beneficiario);
                }
                dr.NextResult();

                //CA
                SolicitudIFP.CoberturasAdicionales = new List<CoberturaAdicional>();
                while (dr.Read())
                {
                    CoberturaAdicional coberturasAdicionales = new CoberturaAdicional();
                    if (dr["fec_nacimiento"] != DBNull.Value)
                        coberturasAdicionales.FechaNacimiento = String.Format("{0:dd/MM/yyyy}", dr["fec_nacimiento"]);
                    if (dr["cod_parentesco"] != DBNull.Value)
                        coberturasAdicionales.Parentesco = dr["cod_parentesco"].ToString();
                    if (dr["cod_sexo"] != DBNull.Value)
                        coberturasAdicionales.Sexo = dr["cod_sexo"].ToString();
                    SolicitudIFP.CoberturasAdicionales.Add(coberturasAdicionales);
                }

            }

            return SolicitudIFP;
        }

        public List<DatosSol> ObtenerDatosporSolicitudIFP(string num_Solicitud)
        {
            List<DatosSol> listaDatosSolicitud = new List<DatosSol>();
            DatosSol DatosSolicitud;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_datos_solicitud_IFP_sel", num_Solicitud);

            dbc.CommandTimeout = 180;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    double ValPjeRenta = 0;
                    if (dr["val_pje_ret"] != DBNull.Value)
                        ValPjeRenta = Convert.ToDouble(dr["val_pje_ret"]);

                    ////Segun el porcentaje de Renta
                    //if (ValPjeRenta > 0)
                    //{
                    DatosSolicitud = new DatosSol();
                    DatosSolicitud.num_cuissp_afiliado = dr["num_cuissp_afiliado"].ToString();
                    DatosSolicitud.ape_paterno_afiliado = dr["ape_paterno_afiliado"].ToString();
                    DatosSolicitud.ape_materno_afiliado = dr["ape_materno_afiliado"].ToString();
                    DatosSolicitud.nom_nombre_afiliado = dr["nom_nombre_afiliado"].ToString();

                    if (dr["fec_nacimiento_afiliado"] != DBNull.Value)
                        DatosSolicitud.fec_nacimiento_afiliado = Convert.ToDateTime(dr["fec_nacimiento_afiliado"]);

                    DatosSolicitud.cod_tipo_documento_afiliado = dr["cod_tipo_documento_afiliado"].ToString();
                    DatosSolicitud.rut_persona_afiliado = dr["rut_persona_afiliado"].ToString();
                    DatosSolicitud.cod_sexo_afiliado = Convert.ToChar(dr["cod_sexo_afiliado"].ToString());
                    DatosSolicitud.cod_EstadoCivil_afiliado = dr["cod_EstadoCivil_afiliado"].ToString();
                    DatosSolicitud.id_grupo_familiar_beneficiario = Convert.ToInt32(dr["id_grupo_familiar_beneficiario"].ToString());
                    DatosSolicitud.num_cuissp_beneficiario = dr["num_cuissp_beneficiario"].ToString();
                    DatosSolicitud.ape_paterno_beneficiario = dr["ape_paterno_beneficiario"].ToString();
                    DatosSolicitud.ape_materno_beneficiario = dr["ape_materno_beneficiario"].ToString();
                    DatosSolicitud.nom_persona_beneficiario = dr["nom_persona_beneficiario"].ToString();
                    DatosSolicitud.valPjeRenta = Convert.ToDouble(dr["val_pje_ret"]) * 100;

                    if (dr["fec_nacimiento_beneficiario"] != DBNull.Value)
                        DatosSolicitud.fec_nacimiento_beneficiario = Convert.ToDateTime(dr["fec_nacimiento_beneficiario"]);

                    DatosSolicitud.cod_tipo_identificacion_beneficiario = dr["cod_tipo_identificacion_beneficiario"].ToString();
                    DatosSolicitud.num_identificacion_beneficiario = dr["num_identificacion_beneficiario"].ToString();
                    DatosSolicitud.cod_parentesco_beneficiario = dr["cod_parentesco_beneficiario"].ToString();
                    DatosSolicitud.gls_parentesco_beneficiario = dr["gls_parentesco_beneficiario"].ToString();

                    DatosSolicitud.cod_Nacionalidad_beneficiario = dr["cod_Nacionalidad_beneficiario"].ToString();
                    DatosSolicitud.cod_Profesion_beneficiario = dr["cod_Profesion_beneficiario"].ToString();
                    DatosSolicitud.cod_Residencia_beneficiario = dr["cod_Residencia_beneficiario"].ToString();

                    DatosSolicitud.ind_PEP_beneficiario = dr["ind_PEP_beneficiario"].ToString();
                    DatosSolicitud.ind_SujetoObligado_beneficiario = dr["ind_SujetoObligado_beneficiario"].ToString();

                    DatosSolicitud.ind_PEP_afiliado = dr["ind_PEP_afiliado"].ToString();
                    DatosSolicitud.ind_SujetoObligado_afiliado = dr["ind_SujetoObligado_afiliado"].ToString();

                    DatosSolicitud.num_solicitud = dr["num_solicitud"].ToString();
                    DatosSolicitud.fec_solicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    DatosSolicitud.cod_afp = dr["cod_afp"].ToString();
                    DatosSolicitud.cod_tipo_cotizacion = dr["cod_tipo_cotizacion"].ToString();
                    DatosSolicitud.val_mto_cta_individual = Convert.ToDouble(dr["val_mto_cta_individual"].ToString());
                    DatosSolicitud.cod_moneda_cta_indiv = dr["cod_moneda_cta_indiv"].ToString();
                    DatosSolicitud.fec_nacimiento_beneficiario_day1 = dr["fec_nacimiento_beneficiario_day1"].ToString();
                    DatosSolicitud.fec_nacimiento_beneficiario_day2 = dr["fec_nacimiento_beneficiario_day2"].ToString();
                    DatosSolicitud.fec_nacimiento_beneficiario_month1 = dr["fec_nacimiento_beneficiario_month1"].ToString();
                    DatosSolicitud.fec_nacimiento_beneficiario_month2 = dr["fec_nacimiento_beneficiario_month2"].ToString();
                    DatosSolicitud.fec_nacimiento_beneficiario_year1 = dr["fec_nacimiento_beneficiario_year1"].ToString();
                    DatosSolicitud.fec_nacimiento_beneficiario_year2 = dr["fec_nacimiento_beneficiario_year2"].ToString();
                    DatosSolicitud.fec_nacimiento_beneficiario_year3 = dr["fec_nacimiento_beneficiario_year3"].ToString();
                    DatosSolicitud.fec_nacimiento_beneficiario_year4 = dr["fec_nacimiento_beneficiario_year4"].ToString();

                    DatosSolicitud.nom_agente = dr["nom_agente"].ToString();
                    DatosSolicitud.rut_agente = dr["rut_agente"].ToString();
                    DatosSolicitud.num_agente = dr["num_agente"].ToString();

                    DatosSolicitud.cod_tipo_plan_rpp = dr["cod_tipo_plan_rpp"].ToString();
                    DatosSolicitud.cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString();
                    DatosSolicitud.val_pen_cia = dr["val_pen_cia"].ToString();
                    DatosSolicitud.val_mon_aju = dr["val_mon_aju"].ToString();
                    DatosSolicitud.cod_moneda = dr["cod_moneda"].ToString();
                    DatosSolicitud.val_per_garantizado = dr["val_per_garantizado"].ToString();
                    DatosSolicitud.ind_gasto_sepelio = dr["ind_gasto_sepelio"].ToString();
                    DatosSolicitud.val_per_temporal = dr["val_per_temporal"].ToString();
                    DatosSolicitud.val_pje_dev = dr["val_pje_dev"].ToString();

                    DatosSolicitud.nom_agencia = dr["nom_agencia"].ToString();
                    DatosSolicitud.MontoSepelio = dr["MontoSepelio"].ToString();

                    DatosSolicitud.Nacionalidad_beneficiario_cod = dr["Nacionalidad_beneficiario_cod"].ToString();
                    DatosSolicitud.Profesion_beneficiario_cod = dr["Profesion_beneficiario_cod"].ToString();
                    DatosSolicitud.Residencia_beneficiario_cod = dr["Residencia_beneficiario_cod"].ToString();

                    DatosSolicitud.Confidencialidaddatos = dr["cod_Confidencialidaddatos"].ToString();
                    DatosSolicitud.Banco = dr["cod_Banco"].ToString();
                    DatosSolicitud.NroCtaBancaria = dr["gls_NroCtaBancaria"].ToString();
                    DatosSolicitud.Comunicacion = dr["cod_Comunicacion"].ToString();

                    DatosSolicitud.direccion = dr["direccion"].ToString();
                    DatosSolicitud.cod_departamento = dr["cod_departamento"].ToString();
                    DatosSolicitud.cod_distrito = dr["cod_distrito"].ToString();
                    DatosSolicitud.cod_provincia = dr["cod_provincia"].ToString();
                    DatosSolicitud.cod_tipovia = dr["cod_tipovia"].ToString();
                    DatosSolicitud.nromzlt = dr["nromzlt"].ToString();

                    DatosSolicitud.CodigoEstado = 0;
                    if (dr["cod_estado_rpp"] != DBNull.Value)
                        DatosSolicitud.CodigoEstado = Convert.ToInt32(dr["cod_estado_rpp"]);

                    DatosSolicitud.CodigoEstadoPlaft = 0;
                    if (dr["cod_estado_plaft"] != DBNull.Value)
                        DatosSolicitud.CodigoEstadoPlaft = Convert.ToInt32(dr["cod_estado_plaft"]);

                    DatosSolicitud.coberturaAdicionalDevolucion = 0;
                    if (dr["cob_adic_dev"] != DBNull.Value)
                        DatosSolicitud.coberturaAdicionalDevolucion = Convert.ToInt32(dr["cob_adic_dev"]);

                    DatosSolicitud.coberturaAdicionalFallecimiento = 0;
                    if (dr["cob_adic_cap_fallec"] != DBNull.Value)
                        DatosSolicitud.coberturaAdicionalFallecimiento = Convert.ToInt32(dr["cob_adic_cap_fallec"]);

                    DatosSolicitud.pjeFallecimientonoDevengados = 0.00;
                    if (dr["val_pje_fallec_ndeveng"] != DBNull.Value)
                        DatosSolicitud.pjeFallecimientonoDevengados = Convert.ToDouble(dr["val_pje_fallec_ndeveng"]);

                    DatosSolicitud.pjeDevolucionFallecimiento = 0.00;
                    if (dr["val_pje_dev_fallec"] != DBNull.Value)
                        DatosSolicitud.pjeDevolucionFallecimiento = Convert.ToDouble(dr["val_pje_dev_fallec"]);

                    DatosSolicitud.periodoDiferido = 0;
                    if (dr["val_per_diferido"] != DBNull.Value)
                        DatosSolicitud.periodoDiferido = Convert.ToInt32(dr["val_per_diferido"]);

                    DatosSolicitud.Val_prima_unica_sepelio = 0.00;
                    if (dr["val_res_sepelio"] != DBNull.Value)
                        DatosSolicitud.Val_prima_unica_sepelio = Convert.ToDouble(dr["val_res_sepelio"]);

                    DatosSolicitud.Val_prima_unica_pension = 0.00;
                    if (dr["val_res_pension"] != DBNull.Value)
                        DatosSolicitud.Val_prima_unica_pension = Convert.ToDouble(dr["val_res_pension"]);

                    DatosSolicitud.Val_prima_unica_devolucion = 0.00;
                    if (dr["val_res_devolucion"] != DBNull.Value)
                        DatosSolicitud.Val_prima_unica_devolucion = Convert.ToDouble(dr["val_res_devolucion"]);

                    DatosSolicitud.Val_prima_unica_fallecimiento = 0.00;
                    if (dr["val_res_fallecimiento"] != DBNull.Value)
                        DatosSolicitud.Val_prima_unica_fallecimiento = Convert.ToDouble(dr["val_res_fallecimiento"]);


                    DatosSolicitud.codigoPlan = dr["cod_plan"].ToString();
                    DatosSolicitud.glsPlan = dr["gls_plan"].ToString();

                    DatosSolicitud.cod_canal_distribucion = dr["cod_canal_distribucion"].ToString();

                    DatosSolicitud.glsMail = dr["gls_mail"].ToString();

                    DatosSolicitud.telefono = dr["num_telefono"].ToString();
                    DatosSolicitud.celular = dr["num_celular"].ToString();
                    DatosSolicitud.cargo = dr["gls_cargo"].ToString();
                    DatosSolicitud.monedaIngreso = new Moneda { Id = dr["cod_moneda_ingreso"].ToString() };
                    DatosSolicitud.centroLaboral = dr["gls_centro_laboral"].ToString();
                    DatosSolicitud.actividadEconomica = dr["gls_actividad_economica"].ToString();
                    DatosSolicitud.ingresoNeto = Convert.ToSingle(dr["val_ingreso_neto"].ToString());
                    DatosSolicitud.EstadoCivil = dr["cod_estado_civil"].ToString();
                    DatosSolicitud.Nacionalidad = dr["cod_nacionalidad"].ToString();
                    DatosSolicitud.OrigenCotizacion = dr["ind_origen"].ToString();

                    DatosSolicitud.ValPjeCACy = Convert.ToInt32(dr["ValPjeCACy"].ToString());
                    DatosSolicitud.ValPjeCAPa = Convert.ToInt32(dr["ValPjeCAPa"].ToString());
                    DatosSolicitud.ValPjeCAMa = Convert.ToInt32(dr["ValPjeCAMa"].ToString());

                    DatosSolicitud.gls_identificacion_afiliado = dr["gls_corta_identificacion"].ToString();

                    listaDatosSolicitud.Add(DatosSolicitud);

                }

                dr.NextResult();

                List<BeneficiariosCA> listaBeneficiariosCA = new List<BeneficiariosCA>();
                while (dr.Read())
                {
                    BeneficiariosCA beneficiariosCA = new BeneficiariosCA();
                    if (dr["nombres"] != DBNull.Value)
                        beneficiariosCA.nombres = dr["nombres"].ToString();
                    if (dr["fec_nacimiento"] != DBNull.Value)
                        beneficiariosCA.fechaNacimiento = String.Format("{0:dd/MM/yyyy}", dr["fec_nacimiento"]);
                    if (dr["num_identificacion"] != DBNull.Value)
                        beneficiariosCA.docIdentidad = dr["num_identificacion"].ToString();
                    if (dr["parentesco"] != DBNull.Value)
                        beneficiariosCA.parentesco = dr["parentesco"].ToString();
                    if (dr["val_pje_base"] != DBNull.Value)
                        beneficiariosCA.renta = Convert.ToInt32(dr["val_pje_base"].ToString());

                    listaBeneficiariosCA.Add(beneficiariosCA);
                }

                listaDatosSolicitud[0].BeneficiariosCA = listaBeneficiariosCA;

            }

            return listaDatosSolicitud;
        }

        public int ObtenerIndicadorRescateIFP(string solicitud, string usuario)
        {

            try
            {
                int ind_Rescate = 0;
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_indicador_rescate", solicitud, usuario);

                dbc.CommandTimeout = 180;

                using (IDataReader dr = db.ExecuteReader(dbc))
                {
                    while (dr.Read())
                    {
                        ind_Rescate = Convert.ToInt32(dr["ind_rescate"]);
                    }
                }
                return ind_Rescate;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public SolicitudIFP ValidarCotizacionVigente(string cod_tipo_documento, string num_documento, string usuario)
        {
            SolicitudIFP SolicitudIFP = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_validar_cotizacion_vigente", cod_tipo_documento, num_documento, usuario);

            dbc.CommandTimeout = 180;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {

                // Datos de Solicitud
                if (dr.Read())
                {
                    SolicitudIFP = new SolicitudIFP();


                    SolicitudIFP.AgenteCotizacion = dr["agente_cotizacion"].ToString();
                    SolicitudIFP.OrigenCotizacion = dr["ind_origen"].ToString();

                    if (dr["fec_vigencia"] != DBNull.Value)
                        SolicitudIFP.FechaVigencia = Convert.ToDateTime(dr["fec_vigencia"]);

                    SolicitudIFP.Agente = new Agente { Id = dr["num_vendedor"].ToString() };
                    SolicitudIFP.Id = dr["num_solicitud"].ToString();

                    if (dr["fec_solicitud"] != DBNull.Value)
                        SolicitudIFP.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                }

            }

            return SolicitudIFP;
        }

        public void RegistrarPjeBen(SolicitudIFP solicitud)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_pjeben");

                db.AddInParameter(dbc, "@wl_xml", DbType.String, solicitud.XMLPjeBen());
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, solicitud.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
