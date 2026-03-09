using Interseguro.CWRV.Dominio.Entidades.MotorCalculo;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Infraestructura.General;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioMotorCalculo: IRepositorioMotorCalculo
    {
        public List<JuegoParametros> ObtenerParametrosRPP(string temporalidad, DateTime fechaCotizacion, string origen, string usuario)
        {
            try
            {
                List<JuegoParametros> juegosParametros = new List<JuegoParametros>();

                // Insertamos las 4 monedas al juego maestro
                JuegoParametros juegoParametros;
                juegoParametros = new JuegoParametros { CodigoTemporalidad = temporalidad, CodigoMoneda = Enums.Moneda.Soles.StringValue(), CodigoOrigen = origen, Parametros = new Parametros() };
                juegosParametros.Add(juegoParametros);
                juegoParametros = new JuegoParametros { CodigoTemporalidad = temporalidad, CodigoMoneda = Enums.Moneda.SolesAjustados.StringValue(), CodigoOrigen = origen, Parametros = new Parametros() };
                juegosParametros.Add(juegoParametros);
                juegoParametros = new JuegoParametros { CodigoTemporalidad = temporalidad, CodigoMoneda = Enums.Moneda.Dolares.StringValue(), CodigoOrigen = origen, Parametros = new Parametros() };
                juegosParametros.Add(juegoParametros);
                juegoParametros = new JuegoParametros { CodigoTemporalidad = temporalidad, CodigoMoneda = Enums.Moneda.DolaresAjustados.StringValue(), CodigoOrigen = origen, Parametros = new Parametros() };
                juegosParametros.Add(juegoParametros);

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_carpr_solicitud_rpp", temporalidad, fechaCotizacion, origen, usuario))
                {
                    // cot_xml_ash - Parámetros de Asset Share
                    string monedaAnterior = string.Empty;
                    while (dr.Read())
                    {
                        string moneda = dr["cod_moneda"].ToString();
                        if (monedaAnterior != moneda)
                        {
                            juegoParametros = juegosParametros.Find(j => j.CodigoMoneda == moneda && j.CodigoTemporalidad == temporalidad && j.CodigoOrigen == origen);
                            juegoParametros.Parametros.cotizacion = new Cotizacion
                            {
                                parametro_ash = new ParametroAsh()
                            };
                            monedaAnterior = moneda;
                        }

                        ParametroAsh parametroAsh = new ParametroAsh
                        {
                            val_cmor = Convert.ToDouble(dr["val_cmor"]),
                            val_fcon = Convert.ToDouble(dr["val_fcon"]),
                            tas_htra = Convert.ToDouble(dr["tas_htra"]),
                            tas_htva = Convert.ToDouble(dr["tas_htva"]),
                            tas_ltra = Convert.ToDouble(dr["tas_ltra"]),
                            tas_ltva = Convert.ToDouble(dr["tas_ltva"]),
                            val_rend = Convert.ToDouble(dr["val_rend"]),
                            tas_tgpd = Convert.ToDouble(dr["tas_tgpd"]),
                            flg_ibtp = Convert.ToInt32(dr["flg_ibtp"]),
                            flg_ivnt = Convert.ToInt32(dr["flg_ivnt"]),
                            flg_ideb = Convert.ToInt32(dr["flg_ideb"]),
                            flg_iajm = Convert.ToInt32(dr["flg_iajm"]),
                            flg_icmo = Convert.ToInt32(dr["flg_icmo"]),
                            tas_timp = Convert.ToDouble(dr["tas_timp"]),
                            tas_tsbs = Convert.ToDouble(dr["tas_tsbs"]),
                            tas_ttec = Convert.ToDouble(dr["tas_ttec"]),
                            val_gfi1 = Convert.ToDouble(dr["val_gfi1"]),
                            val_gfi2 = Convert.ToDouble(dr["val_gfi2"]),
                            val_comi = Convert.ToDouble(dr["val_comi"]),
                            val_coba = Convert.ToDouble(dr["val_coba"]),
                            val_pumi = Convert.ToDouble(dr["val_pumi"]),
                            num_nins = Convert.ToDouble(dr["num_nins"]),
                            num_nper = Convert.ToDouble(dr["num_nper"]),
                            val_ltit = Convert.ToDouble(dr["val_ltit"]),
                            val_htit = Convert.ToDouble(dr["val_htit"]),
                            //ini_tra2 = Convert.ToDouble(dr["ini_tra2"]),
                            //val_tasa_inv = Convert.ToDouble(dr["val_tasa_inv"]),
                            val_tasa_inf = Convert.ToDouble(dr["val_tinf"]),
                            val_tasa_mrg_solv = Convert.ToDouble(dr["val_solv"]),
                            val_tasa_costo_cap = Convert.ToDouble(dr["val_cokp"]),
                            val_vtax = Convert.ToDouble(dr["val_vtax"]),
                            val_dtra = Convert.ToDouble(dr["val_dtra"])
                        };
                        juegoParametros.Parametros.cotizacion.parametro_ash = parametroAsh;
                    }

                    dr.NextResult();

                    // ppu_vllx - Tablas de Mortalidad
                    List<TablaMortalidad> tablasMortalidad = new List<TablaMortalidad>();
                    while (dr.Read())
                    {
                        TablaMortalidad tablaMortalidad = new TablaMortalidad
                        {
                            num_edad_mes = Convert.ToInt32(dr["num_edad_mes"]),
                            val_lx_mbh = Convert.ToDouble(dr["val_lx_mbh"]),
                            val_lx_mbm = Convert.ToDouble(dr["val_lx_mbm"]),
                            val_lx_mih = Convert.ToDouble(dr["val_lx_mih"]),
                            val_lx_mim = Convert.ToDouble(dr["val_lx_mim"]),
                            val_lx_mvh = Convert.ToDouble(dr["val_lx_mvh"]),
                            val_lx_mvm = Convert.ToDouble(dr["val_lx_mvm"])
                        };
                        tablasMortalidad.Add(tablaMortalidad);
                    }
                    juegosParametros.ForEach(j => j.Parametros.tablas_mortalidad = tablasMortalidad);

                    dr.NextResult();

                    // arr_ppu_fm_cot - Factores de Mejora
                    List<FactorMejora> factoresMejora = new List<FactorMejora>();
                    while (dr.Read())
                    {
                        FactorMejora factorMejora = new FactorMejora
                        {
                            num_edad_mes = Convert.ToInt32(dr["num_edad_mes"]),
                            val_lx_mbh = Convert.ToDouble(dr["val_lx_mbh"]),
                            val_lx_mbm = Convert.ToDouble(dr["val_lx_mbm"]),
                            val_lx_mih = Convert.ToDouble(dr["val_lx_mih"]),
                            val_lx_mim = Convert.ToDouble(dr["val_lx_mim"]),
                            val_lx_mvh = Convert.ToDouble(dr["val_lx_mvh"]),
                            val_lx_mvm = Convert.ToDouble(dr["val_lx_mvm"])
                        };
                        factoresMejora.Add(factorMejora);
                    }
                    juegosParametros.ForEach(j => j.Parametros.factores_mejoras = factoresMejora);

                    dr.NextResult();

                    // arr_inf_tm_cot - Información de Tablas de Mortalidad
                    List<InformacionFactorMejora> informacionesFM = new List<InformacionFactorMejora>();
                    while (dr.Read())
                    {
                        InformacionFactorMejora informacionFactorMejora = new InformacionFactorMejora
                        {
                            val_tipo_beneficiario = dr["cod_tipo_pensionista"].ToString(),
                            num_anio_factor = Convert.ToInt32(dr["num_anio"])
                        };
                        informacionesFM.Add(informacionFactorMejora);
                    }
                    juegosParametros.ForEach(j => j.Parametros.informacion_factores_mejoras = informacionesFM);

                    dr.NextResult();

                    // arr_inf_fm_cot - Información de Factores de Mejora

                    dr.NextResult();

                    // cot_xml_inv - Parámetros de Inversión
                    monedaAnterior = string.Empty;
                    while (dr.Read())
                    {
                        string moneda = dr["cod_moneda"].ToString();
                        if (monedaAnterior != moneda)
                        {
                            juegoParametros = juegosParametros.Find(j => j.CodigoMoneda == moneda && j.CodigoTemporalidad == temporalidad && j.CodigoOrigen == origen);
                            juegoParametros.Parametros.inversiones_vitalicias = new List<InversionVitalicia>();
                            monedaAnterior = moneda;
                        }

                        InversionVitalicia parametroInversion = new InversionVitalicia
                        {
                            cod_moneda = dr["cod_moneda"].ToString(),
                            cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString(),
                            num_anio = Convert.ToInt32(dr["num_nper"]),
                            val_inv = Convert.ToDouble(dr["val_fwd"])
                        };
                        juegoParametros.Parametros.inversiones_vitalicias.Add(parametroInversion);
                    }

                    dr.NextResult();

                    // cot_xml_ajm - Ajustes de Tabla de Mortalidad - NMES
                    List<FactorAntiSeleccion> ajustesNMES = new List<FactorAntiSeleccion>();
                    while (dr.Read())
                    {
                        FactorAntiSeleccion ajusteNMES = new FactorAntiSeleccion
                        {
                            cod_sexo = dr["num_columna"].ToString() == "5" ? "M" : "F",
                            pje_ajuste = Convert.ToDouble(dr["pje_ajuste"]),
                            val_tope = Convert.ToDouble(dr["val_tope"])
                        };
                        ajustesNMES.Add(ajusteNMES);
                    }
                    juegosParametros.ForEach(j => j.Parametros.factores_anti_seleccion = ajustesNMES);

                    dr.NextResult();

                    // cot_xml_ajm - Ajustes de Tabla de Mortalidad - PUMI
                    List<FactorPUMI> ajustesPUMI = new List<FactorPUMI>();
                    while (dr.Read())
                    {
                        FactorPUMI ajustePUMI = new FactorPUMI
                        {
                            cod_sexo = dr["num_columna"].ToString(),
                            pje_ajuste = Convert.ToDouble(dr["pje_ajuste"]),
                            val_tope = Convert.ToDouble(dr["val_tope"])
                        };
                        ajustesPUMI.Add(ajustePUMI);
                    }
                    juegosParametros.ForEach(j => j.Parametros.factores_pumi = ajustesPUMI);

                    dr.NextResult();

                    // arr_VTD - VTD
                    monedaAnterior = string.Empty;
                    while (dr.Read())
                    {
                        string moneda = dr["cod_moneda"].ToString();
                        if (monedaAnterior != moneda)
                        {
                            juegoParametros = juegosParametros.Find(j => j.CodigoMoneda == moneda && j.CodigoTemporalidad == temporalidad && j.CodigoOrigen == origen);
                            juegoParametros.Parametros.lista_VTD = new List<TablaVTD>();
                            monedaAnterior = moneda;
                        }

                        TablaVTD parametroVTD = new TablaVTD
                        {
                            cod_moneda = dr["cod_moneda"].ToString(),
                            cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString(),
                            num_mes = Convert.ToInt32(dr["num_mes"]),
                            val_vtd = Convert.ToDouble(dr["vtd"])
                        };
                        juegoParametros.Parametros.lista_VTD.Add(parametroVTD);
                    }

                    dr.NextResult();

                    // arr_VOLA - VOLA
                    monedaAnterior = string.Empty;
                    while (dr.Read())
                    {
                        string moneda = dr["cod_moneda"].ToString();
                        if (monedaAnterior != moneda)
                        {
                            juegoParametros = juegosParametros.Find(j => j.CodigoMoneda == moneda && j.CodigoTemporalidad == temporalidad && j.CodigoOrigen == origen);
                            juegoParametros.Parametros.lista_VOLA = new List<TablaVOLA>();
                            monedaAnterior = moneda;
                        }

                        TablaVOLA parametroVOLA = new TablaVOLA
                        {
                            cod_moneda = dr["cod_moneda"].ToString(),
                            cod_tipo_temporalidad = dr["cod_tipo_temporalidad"].ToString(),
                            num_mes = Convert.ToInt32(dr["num_mes"]),
                            val_vola = Convert.ToDouble(dr["vola"])
                        };
                        juegoParametros.Parametros.lista_VOLA.Add(parametroVOLA);
                    }

                    dr.NextResult();

                    // Ajustes de TRA

                    return juegosParametros;
                }
            }
            catch (NullReferenceException ex)
            {
                throw new Exception(string.Format("No hay datos suficientes para cotizar en la siguiente fecha [{0:dd/MM/yyyy}].", fechaCotizacion), ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
