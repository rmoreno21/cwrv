using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data.SqlClient;
using System.Xml.Linq;
using System.Linq;
using System.Globalization;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioSolicitudRPPlus : IRepositorioSolicitudRPPlus
    {
        public List<SolicitudRPPlus> Listar(string cuspp)
        {
            List<SolicitudRPPlus> listaSolicitudes = new List<SolicitudRPPlus>();
            SolicitudRPPlus solicitud;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solicitudes_RtaPrvdPlus", cuspp))
            //num_solicitud             --
            //fec_solicitud             --
            //cod_tipo_cotizacion       --
            //fec_devengue              --
            //cod_moneda_cta_indiv      --
            //mto_prima_unica           --
            //cod_tipo_temporalidad     --
            //num_agente                --
            {
                while (dr.Read())
                {
                    solicitud = new SolicitudRPPlus();
                    solicitud.Id = dr["num_solicitud"].ToString();
                    solicitud.Afiliado = new Afiliado { CUSPP = cuspp };
                    if (dr["fec_solicitud"] != DBNull.Value)
                        solicitud.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    solicitud.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                    if (dr["fec_devengue"] != DBNull.Value)
                        solicitud.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);

                    if (dr["cod_moneda_cta_indiv"] != DBNull.Value)
                        solicitud.MonedaPrimaUnica = new Moneda { Simbolo = dr["cod_moneda_cta_indiv"].ToString() };


                    if (dr["mto_prima_unica"] != DBNull.Value)
                        solicitud.PrimaUnica = Convert.ToDouble(dr["mto_prima_unica"]);



                    solicitud.Agente = new Agente { Id = dr["num_agente"].ToString() };

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        solicitud.Temporalidad = new Temporalidad { Nombre = dr["cod_tipo_temporalidad"].ToString() };

                    solicitud.CodigoEstado = Convert.ToInt32(dr["cod_estado_rpp"].ToString());
                    solicitud.EstadoSolicitud = dr["gls_estado_solicitud"].ToString();

                    if (dr["fec_vigencia"] != DBNull.Value)
                        solicitud.FechaVigencia = Convert.ToDateTime(dr["fec_vigencia"]);

                    solicitud.CodigoEstadoPlaft = Convert.ToInt32(dr["cod_estado_plaft"].ToString());
                    solicitud.NumeroPoliza = Convert.ToInt32(dr["num_poliza"].ToString());

                    solicitud.IdEstudioNecesidades = dr["id_estudio_necesidades"] != DBNull.Value ? Convert.ToInt32(dr["id_estudio_necesidades"]) : 0;

                    listaSolicitudes.Add(solicitud);
                }
            }

            return listaSolicitudes;
        }

        public void Registrar(ref SolicitudRPPlus entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_solicitud_RtaPrvdPlus");

                db.AddInParameter(dbc, "@xml_solicitud", DbType.String, entity.XMLSolicitud());
                db.AddInParameter(dbc, "@xml_beneficiario", DbType.String, entity.XMLBeneficiario());
                db.AddInParameter(dbc, "@xml_cotizacion", DbType.String, entity.XMLCotizacion());
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, entity.Usuario.NombreUsuario);
                db.AddInParameter(dbc, "@wl_cod_rol", DbType.String, entity.Usuario.Rol);
                db.AddOutParameter(dbc, "@wl_num_solicitud", DbType.String, 20);

                db.ExecuteNonQuery(dbc);

                entity.Id = db.GetParameterValue(dbc, "@wl_num_solicitud").ToString(); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(ref SolicitudRPPlus entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_solicitud_RtaPrvdPlus");

                db.AddInParameter(dbc, "@xml_solicitud", DbType.String, entity.XMLSolicitud());
                db.AddInParameter(dbc, "@xml_beneficiario", DbType.String, entity.XMLBeneficiario());
                db.AddInParameter(dbc, "@xml_cotizacion", DbType.String, entity.XMLCotizacion());
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, entity.Usuario.NombreUsuario);
                db.AddInParameter(dbc, "@wl_cod_rol", DbType.String, entity.Usuario.Rol);
                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.Id);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SolicitudRPPlus ObtenerDatos(string idSolicitud)
        {
            SolicitudRPPlus SolicitudRPPlus = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_solicitud_RtaPrvdPlus", idSolicitud))
            {
                // Datos de Solicitud
                if (dr.Read())
                {
                    SolicitudRPPlus = new SolicitudRPPlus();                  

                    SolicitudRPPlus.Id = dr["num_solicitud"].ToString();
                    if (dr["fec_solicitud"] != DBNull.Value)
                        SolicitudRPPlus.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    if (dr["fec_cotizacion"] != DBNull.Value)
                        SolicitudRPPlus.FechaCotizacion = Convert.ToDateTime(dr["fec_cotizacion"]);
                    SolicitudRPPlus.Afiliado = new Afiliado
                    {
                        CUSPP = dr["num_cuispp"].ToString(),
                        CorreoElectronico = (dr["gls_mail"] != DBNull.Value && dr["gls_mail"].ToString() != string.Empty) ? dr["gls_mail"].ToString() : null,
                        AFP = new AFP { Id = dr["cod_afp"].ToString() }
                    };
                    SolicitudRPPlus.AFP = new AFP { Id = dr["cod_afp"].ToString() };
                    SolicitudRPPlus.Afiliado.AFP = new AFP { Id = dr["cod_afp"].ToString() };
                    SolicitudRPPlus.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                    SolicitudRPPlus.TipoPension = new TipoPension { Id = dr["cod_tipo_pension"].ToString() };
                    if (dr["fec_devengue"] != DBNull.Value)
                        SolicitudRPPlus.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);
                    if (dr["fec_ult_actualizacion"] != DBNull.Value)
                    {
                        //<GTIINI-754>
                        //SolicitudIFP.FechaSolicitud = Convert.ToDateTime(dr["fec_ult_actualizacion"]);
                        SolicitudRPPlus.FechaUltimaActualizacion = Convert.ToDateTime(dr["fec_ult_actualizacion"]);
                        //<GTIFIN-754>
                    }

                    SolicitudRPPlus.PrimaUnica = Convert.ToDouble(dr["val_mto_cta_individual"]); //AQUI ERA .SaldoCIC  //ARMV

                    SolicitudRPPlus.TipoCambio = 0;
                    if (dr["val_tasa_cambio"] != DBNull.Value)
                        SolicitudRPPlus.TipoCambio = Convert.ToDouble(dr["val_tasa_cambio"]);

                    SolicitudRPPlus.FactorTasa = dr["cod_factor_tasa"].ToString();

                    if (dr["pje_descuento_comision"] != DBNull.Value)
                        SolicitudRPPlus.PorcentajeDescuentoComision = Convert.ToDouble(dr["pje_descuento_comision"]);
                    else
                        SolicitudRPPlus.PorcentajeDescuentoComision = 0;
                    SolicitudRPPlus.Categoria = new Categoria { Id = dr["cod_categoria"].ToString() };

                    SolicitudRPPlus.Agente = new Agente { Id = dr["num_agente"].ToString(), IdCartera = dr["cod_cartera"].ToString() };
                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        SolicitudRPPlus.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"])
                            //Nombre = (dr["gls_tipo_movimiento"] != DBNull.Value && dr["gls_tipo_movimiento"].ToString() != string.Empty) ? dr["gls_tipo_movimiento"].ToString() : null
                        };

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value && dr["cod_tipo_temporalidad"].ToString() != string.Empty)
                        SolicitudRPPlus.Temporalidad = new Temporalidad
                        {
                            Id = Convert.ToString(dr["cod_tipo_temporalidad"]),
                            Nombre = (dr["gls_tipo_temporalidad"] != DBNull.Value && dr["gls_tipo_temporalidad"].ToString() != string.Empty) ? dr["gls_tipo_temporalidad"].ToString() : null
                        };

                    if (dr["cod_moneda_cta_indiv"] != DBNull.Value && dr["cod_moneda_cta_indiv"].ToString() != string.Empty)
                        SolicitudRPPlus.MonedaPrimaUnica = new Moneda
                        {
                            Id = Convert.ToString(dr["cod_moneda_cta_indiv"])
                        };

                    if (dr["cod_tipo_plan_rpp"] != DBNull.Value && dr["cod_tipo_plan_rpp"].ToString() != string.Empty)
                        SolicitudRPPlus.TipoPlan = new TipoPlan
                        {
                            Id = Convert.ToString(dr["cod_tipo_plan_rpp"]),
                            Nombre = (dr["gls_tipo_plan_rpp"] != DBNull.Value && dr["gls_tipo_plan_rpp"].ToString() != string.Empty) ? dr["gls_tipo_plan_rpp"].ToString() : null
                        };

                    if (dr["fec_vigencia"] != DBNull.Value)
                        SolicitudRPPlus.FechaVigencia = Convert.ToDateTime(dr["fec_vigencia"]);

                    //<INIGTI_7012>
                    if (dr["num_poliza"] != DBNull.Value)
                        SolicitudRPPlus.NumeroPoliza = Convert.ToInt32(dr["num_poliza"]);

                    SolicitudRPPlus.CodigoEstado = 0;
                    if (dr["cod_estado_rpp"] != DBNull.Value)
                        SolicitudRPPlus.CodigoEstado = Convert.ToInt32(dr["cod_estado_rpp"]);

                    SolicitudRPPlus.EstadoSolicitud = "";
                    if (dr["gls_estado_solicitud"] != DBNull.Value)
                        SolicitudRPPlus.EstadoSolicitud = dr["gls_estado_solicitud"].ToString();

                    SolicitudRPPlus.CodigoEstadoPoliza = "";
                    if (dr["cod_estado_poliza"] != DBNull.Value)
                        SolicitudRPPlus.CodigoEstadoPoliza = dr["cod_estado_poliza"].ToString();

                    SolicitudRPPlus.EstadoPoliza = "";
                    if (dr["gls_estado_poliza"] != DBNull.Value)
                        SolicitudRPPlus.EstadoPoliza = dr["gls_estado_poliza"].ToString();

                    SolicitudRPPlus.CausalPoliza = new CausalPoliza();
                    if (dr["cod_causal_estado_poliza"] != DBNull.Value)
                        SolicitudRPPlus.CausalPoliza = new CausalPoliza
                        {
                            Id = dr["cod_causal_estado_poliza"].ToString(),
                            NombreLargo = dr["gls_causal_estado_poliza"].ToString()
                        };

                    //<INI.GTI_7012_26>
                    SolicitudRPPlus.CodigoEstadoPlaft = 0;
                    if (dr["cod_estado_plaft"] != DBNull.Value)
                        SolicitudRPPlus.CodigoEstadoPlaft = Convert.ToInt32(dr["cod_estado_plaft"]);

                    if (dr["gls_estado_solicitud_plaft"] != DBNull.Value)
                    SolicitudRPPlus.EstadoSolicitudPlaft = "";
                        SolicitudRPPlus.EstadoSolicitudPlaft = dr["gls_estado_solicitud_plaft"].ToString();

                    //<FIN.GTI_7012_26>

                    //<FINGTI_7012>
                }

                dr.NextResult();

                // Cotizaciones
                SolicitudRPPlus.Cotizaciones = new List<CotizacionRPPlus>();
                while (dr.Read())
                {
                    CotizacionRPPlus cotizacion = new CotizacionRPPlus();
                    cotizacion.Correlativo = Convert.ToInt64(dr["num_correlativo"]);
                    cotizacion.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString() };
                    cotizacion.Producto = new Producto { Id = dr["cod_tipo_producto"].ToString(), Nombre = dr["gls_tipo_producto"].ToString() };
                    cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["val_per_garantizado"]);
                    cotizacion.AjusteTRA = Convert.ToDouble(dr["val_Tasa_ajuste_tra"]);

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
                        cotizacion.PagoEscalonada = Convert.ToDouble(dr["val_per_temporal"]);

                    if (dr["val_pje_rent_temp"] != DBNull.Value)
                        cotizacion.PjePE = Convert.ToDouble(dr["val_pje_rent_temp"]);

                    cotizacion.IndGastoSepelio = "N";
                    if (dr["ind_gasto_sepelio"] != DBNull.Value)
                        cotizacion.IndGastoSepelio = dr["ind_gasto_sepelio"].ToString();

                    if (dr["val_pje_dev"] != DBNull.Value)
                        cotizacion.ValPjeDev = Convert.ToDouble(dr["val_pje_dev"]);

                    if (dr["val_1era_prima_is"] != DBNull.Value)
                    {
                        cotizacion.Pension2doTramo = Convert.ToDouble(dr["val_1era_prima_is"]);
                        cotizacion.Pension2doTramoSinAjuste = cotizacion.PensionCiaMO * (cotizacion.PjePE / 100);
                    }

                    if (dr["ind_cotiza"] != DBNull.Value)
                        cotizacion.IndCotiza = dr["ind_cotiza"].ToString();

                    if (dr["ind_error_cotiza"] != DBNull.Value)
                        cotizacion.IndErrorCotiza = Convert.ToInt32(dr["ind_error_cotiza"]);

                    //<INIGTI_753>
                    if (dr["val_pje_conyuge"] != DBNull.Value)
                    {
                        cotizacion.ValPjeConyuge = Convert.ToDouble(dr["val_pje_conyuge"]);
                    }
                    else
                    {
                        cotizacion.ValPjeConyuge = -1;
                    };

                    if (dr["val_mon_aju"] != DBNull.Value)
                    {
                        cotizacion.ValMonAju = Convert.ToDouble(dr["val_mon_aju"]);
                    }
                    else
                    {
                        cotizacion.ValMonAju = -1;
                    };

                    cotizacion.EstadoCotizacion = dr["cod_estado_cotizacion"].ToString();
                    cotizacion.IndSeleccionada = dr["ind_cotizacion_seleccionada"].ToString();

                    //<FINGTI_753>

                    SolicitudRPPlus.Cotizaciones.Add(cotizacion);

                }
                dr.NextResult();

                // Beneficiarios
                SolicitudRPPlus.Beneficiarios = new List<GrupoFamiliar>();
                while (dr.Read())
                {
                    GrupoFamiliar beneficiario = new GrupoFamiliar();
                    //<INI.GTI_52310>
                    Identificacion identificacion = new Identificacion();
                    //<FIN.GTI_52310>

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
                    beneficiario.ind_PEP = (dr["ind_pep"].ToString() == "S") ? true : false;
                    beneficiario.IdGrupoFamiliar = Convert.ToInt64(dr["id_grupo_familiar"]);

                    //<INI.GTI_52310>
                    identificacion.IdTipo = dr["cod_tipo_identificacion"].ToString();
                    identificacion.Numero = dr["num_identificacion"].ToString();
                    identificacion.GlosaTipo = dr["gls_corta_identificacion"].ToString();
                    beneficiario.Identificacion = identificacion;
                    //<FIN.GTI_52310>

                    SolicitudRPPlus.Beneficiarios.Add(beneficiario);
                }
            }

            return SolicitudRPPlus;
        }

        public List<ParametroCotizador> ObtenerParametrosCotizacionRPPlus(string idSolicitud, DateTime fecCotizacion, Int64? numCorrelativo)
        {
            List<ParametroCotizador> parametros = new List<ParametroCotizador>();
            try
            {

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                List<Parinv> lParinv = new List<Parinv>();
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Soles.StringValue(), IndDevolucion = "S" });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Dolares.StringValue(), IndDevolucion = "S" });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.SolesAjustados.StringValue(), IndDevolucion = "S" });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.DolaresAjustados.StringValue(), IndDevolucion = "S" });

                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Soles.StringValue(), IndDevolucion = "N" });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Dolares.StringValue(), IndDevolucion = "N" });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.SolesAjustados.StringValue(), IndDevolucion = "N" });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.DolaresAjustados.StringValue(), IndDevolucion = "N" });

                //<INIGTI_753>
                int lnMes = 0;
                //<FINGTI_753>

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_carpr_solicitud_RtaPrvdPlus", idSolicitud, fecCotizacion, numCorrelativo))
                {
                    // Parámetros de Cotización
                    ParametroCotizador parametro;
                    while (dr.Read())
                    {
                        parametro = new ParametroCotizador();
                        parametro.cot_gls_ikey = dr["wl_cot_kcot"].ToString();
                        parametro.cot_num_soli = dr["wl_num_solicitud"].ToString();
                        parametro.cot_num_coti = Convert.ToInt32(dr["wl_num_corr_cotiza"]);
                        parametro.ind_orden = Convert.ToInt32(dr["ind_orden"]);
                        parametro.cot_num_mdif = Convert.ToInt32(dr["wl_cot_pdif"]);
                        parametro.cot_num_mgar = Convert.ToInt32(dr["wl_cot_pgar"]);
                        parametro.cot_num_nben = Convert.ToInt32(dr["wl_cot_nben"]);
                        parametro.cot_fec_fcal = Convert.ToInt32(Convert.ToDateTime(dr["wl_cot_fcot"]).ToString("yyyyMMdd"));
                        parametro.cot_fec_fdev = Convert.ToInt32(Convert.ToDateTime(dr["wl_cot_fdev"]).ToString("yyyyMMdd"));
                        parametro.cot_num_tpen = Convert.ToInt32(dr["wl_cot_tpen"]);
                        parametro.cot_num_tcal = Convert.ToInt32(dr["wl_cot_tcal"]);
                        parametro.cot_flg_idac = Convert.ToInt32(dr["wl_cot_derc"]);
                        parametro.cot_flg_igra = Convert.ToInt32(dr["wl_cot_grat"]);
                        parametro.cot_num_cmon = Convert.ToInt32(dr["cot_mon_equi"]);
                        parametro.cot_num_trea = Convert.ToInt32(dr["cot_num_trea"]);
                        parametro.cot_num_frea = Convert.ToInt32(dr["cot_num_frea"]);
                        parametro.cot_flg_irea = Convert.ToInt32(dr["cot_flg_irea"]);
                        parametro.cot_tas_vrea = Convert.ToDouble(dr["wg_val_ajuste_tasa_fija"]);
                        parametro.cot_tas_tasa = Convert.ToDouble(dr["wl_val_tasa_venta"]);
                        parametro.cot_val_vpen = Convert.ToDouble(dr["wl_cot_vpen"]);
                        parametro.cot_tas_vtra = Convert.ToDouble(dr["wl_val_vtra"]);
                        parametro.cot_tas_tafp = Convert.ToDouble(dr["wl_val_tasa_afp"]);
                        parametro.cot_val_acom = Convert.ToDouble(dr["wl_val_acom"]);
                        parametro.cot_val_dcom = Convert.ToDouble(dr["wl_cot_pdco"]);
                        parametro.cot_val_puam = Convert.ToDouble(dr["wl_val_puam"]);
                        parametro.cot_val_puni = Convert.ToDouble(dr["wl_val_puni"]);
                        parametro.cot_por_prrt = Convert.ToDouble(dr["wl_cot_prrt"]);
                        parametro.cot_val_tgfi = Convert.ToDouble(dr["wl_val_tgfi"]);
                        parametro.cot_val_tope = Convert.ToInt32(dr["val_num_tope"]);

                        // Inicializando los demás parámetros
                        parametro.ppu_vllx = new double[1320, 8];
                        parametro.ppu_arr_vllx = new double[10569, 4];
                        parametro.x_cot_xml_benefi = new StringBuilder(5000);

                        // Parámetros auxiliares
                        parametro.fec_cotizacion = Convert.ToDateTime(dr["wl_cot_fcot"]);
                        parametro.wl_val_pension_minimo = Convert.ToDouble(dr["wl_val_pension_minimo"]);
                        parametro.cod_tipo_pension = dr["cod_tipo_pension"].ToString();
                        parametro.cod_tipo_invalidez = dr["cod_tipo_invalidez"].ToString();
                        parametro.cod_moneda = dr["cod_moneda"].ToString();
                        parametro.val_moneda = Convert.ToDouble(dr["val_moneda"]);
                        parametro.cod_tipo_producto = dr["cod_tipo_producto"].ToString();
                        parametro.ind_modalidad = dr["ind_modalidad"].ToString();

                        //<INIGTI_753>
                        if (dr["val_mon_aju"] != DBNull.Value)
                        {
                            parametro.x_val_mon_aju = Convert.ToDouble(dr["val_mon_aju"]);
                        }
                        else
                        {
                            parametro.x_val_mon_aju = -1;
                        }


                        lnMes = Convert.ToDateTime(dr["wl_cot_fdev"]).Month;

                        //<FINGTI_753>
                        parametros.Add(parametro);
                    }



                    dr.NextResult();

                    // ppu_vllx
                    double[,] arreglo_ppu_vllx = new double[1321, 8];
                    int i = 0;
                    while (dr.Read())
                    {
                        arreglo_ppu_vllx[i, 0] = Convert.ToDouble(dr["val_lx_mbh"]);
                        arreglo_ppu_vllx[i, 1] = Convert.ToDouble(dr["val_lx_mbm"]);
                        arreglo_ppu_vllx[i, 2] = Convert.ToDouble(dr["val_lx_mih"]);
                        arreglo_ppu_vllx[i, 3] = Convert.ToDouble(dr["val_lx_mim"]);
                        arreglo_ppu_vllx[i, 4] = Convert.ToDouble(dr["val_lx_mvh"]);
                        arreglo_ppu_vllx[i, 5] = Convert.ToDouble(dr["val_lx_mvm"]);
                        arreglo_ppu_vllx[i, 6] = Convert.ToDouble(dr["val_lx_mph"]);
                        arreglo_ppu_vllx[i, 7] = Convert.ToDouble(dr["val_lx_mpm"]);
                        i++;
                    }
                    parametros.ForEach(p => p.ppu_vllx = arreglo_ppu_vllx);


                    //<SOLINI-19737>
                    dr.NextResult();

                    // ppu_vllx_vol
                    double[,] arr_ppu_vllx_cot = new double[1321, 8];
                    i = 0;
                    while (dr.Read())
                    {
                        arr_ppu_vllx_cot[i, 0] = Convert.ToDouble(dr["val_lx_mbh"]);
                        arr_ppu_vllx_cot[i, 1] = Convert.ToDouble(dr["val_lx_mbm"]);
                        arr_ppu_vllx_cot[i, 2] = Convert.ToDouble(dr["val_lx_mih"]);
                        arr_ppu_vllx_cot[i, 3] = Convert.ToDouble(dr["val_lx_mim"]);
                        arr_ppu_vllx_cot[i, 4] = Convert.ToDouble(dr["val_lx_mvh"]);
                        arr_ppu_vllx_cot[i, 5] = Convert.ToDouble(dr["val_lx_mvm"]);
                        arr_ppu_vllx_cot[i, 6] = Convert.ToDouble(dr["val_lx_mph"]);
                        arr_ppu_vllx_cot[i, 7] = Convert.ToDouble(dr["val_lx_mpm"]);
                        i++;
                    }
                    parametros.ForEach(p => p.ppu_vllx_cot = arr_ppu_vllx_cot);

                    dr.NextResult();

                    // ppu_fmqx
                    double[,] arr_ppu_fm_cot = new double[1321, 8];
                    i = 0;
                    while (dr.Read())
                    {
                        arr_ppu_fm_cot[i, 0] = Convert.ToDouble(dr["val_lx_mbh"]);
                        arr_ppu_fm_cot[i, 1] = Convert.ToDouble(dr["val_lx_mbm"]);
                        arr_ppu_fm_cot[i, 2] = Convert.ToDouble(dr["val_lx_mih"]);
                        arr_ppu_fm_cot[i, 3] = Convert.ToDouble(dr["val_lx_mim"]);
                        arr_ppu_fm_cot[i, 4] = Convert.ToDouble(dr["val_lx_mvh"]);
                        arr_ppu_fm_cot[i, 5] = Convert.ToDouble(dr["val_lx_mvm"]);
                        arr_ppu_fm_cot[i, 6] = Convert.ToDouble(dr["val_lx_mph"]);
                        arr_ppu_fm_cot[i, 7] = Convert.ToDouble(dr["val_lx_mpm"]);
                        i++;
                    }
                    parametros.ForEach(p => p.ppu_fmqx = arr_ppu_fm_cot);

                    dr.NextResult();

                    // ppu_fmqx
                    double[,] arr_ppu_fm_sbs = new double[1321, 8];
                    i = 0;
                    while (dr.Read())
                    {
                        arr_ppu_fm_sbs[i, 0] = Convert.ToDouble(dr["val_lx_mbh"]);
                        arr_ppu_fm_sbs[i, 1] = Convert.ToDouble(dr["val_lx_mbm"]);
                        arr_ppu_fm_sbs[i, 2] = Convert.ToDouble(dr["val_lx_mih"]);
                        arr_ppu_fm_sbs[i, 3] = Convert.ToDouble(dr["val_lx_mim"]);
                        arr_ppu_fm_sbs[i, 4] = Convert.ToDouble(dr["val_lx_mvh"]);
                        arr_ppu_fm_sbs[i, 5] = Convert.ToDouble(dr["val_lx_mvm"]);
                        arr_ppu_fm_sbs[i, 6] = Convert.ToDouble(dr["val_lx_mph"]);
                        arr_ppu_fm_sbs[i, 7] = Convert.ToDouble(dr["val_lx_mpm"]);
                        i++;
                    }
                    parametros.ForEach(p => p.ppu_fmqx_sbs = arr_ppu_fm_sbs);

                    dr.NextResult();

                    // inf_tm
                    int[,] arr_inf_tm_cot = new int[3, 8];
                    i = 0;
                    while (dr.Read())
                    {

                        arr_inf_tm_cot[0, i] = Convert.ToInt32(dr["cod_tipo_dato"]);
                        arr_inf_tm_cot[1, i] = Convert.ToInt32(dr["ind_anual_mensual"]);
                        arr_inf_tm_cot[2, i] = Convert.ToInt32(dr["num_anio"]);

                        arr_inf_tm_cot[0, i + 1] = Convert.ToInt32(dr["cod_tipo_dato"]);
                        arr_inf_tm_cot[1, i + 1] = Convert.ToInt32(dr["ind_anual_mensual"]);
                        arr_inf_tm_cot[2, i + 1] = Convert.ToInt32(dr["num_anio"]);

                        i = i + 2;
                    }

                    parametros.ForEach(p => p.ppu_inf_tm = arr_inf_tm_cot);

                    dr.NextResult();

                    // inf_tm_sbs
                    int[,] arr_inf_tm_sbs = new int[3, 8];
                    i = 0;
                    while (dr.Read())
                    {

                        arr_inf_tm_sbs[0, i] = Convert.ToInt32(dr["cod_tipo_dato"]);
                        arr_inf_tm_sbs[1, i] = Convert.ToInt32(dr["ind_anual_mensual"]);
                        arr_inf_tm_sbs[2, i] = Convert.ToInt32(dr["num_anio"]);

                        arr_inf_tm_sbs[0, i + 1] = Convert.ToInt32(dr["cod_tipo_dato"]);
                        arr_inf_tm_sbs[1, i + 1] = Convert.ToInt32(dr["ind_anual_mensual"]);
                        arr_inf_tm_sbs[2, i + 1] = Convert.ToInt32(dr["num_anio"]);

                        i = i + 2;
                    }

                    parametros.ForEach(p => p.ppu_inf_tm_sbs = arr_inf_tm_sbs);

                    dr.NextResult();

                    // inf_fm
                    int[,] arr_inf_fm_cot = new int[3, 8];
                    i = 0;
                    while (dr.Read())
                    {
                        arr_inf_fm_cot[0, i] = Convert.ToInt32(dr["cod_tipo_dato"]);
                        arr_inf_fm_cot[1, i] = Convert.ToInt32(dr["ind_anual_mensual"]);
                        arr_inf_fm_cot[2, i] = Convert.ToInt32(dr["ind_aplica_fm"]);

                        arr_inf_fm_cot[0, i + 1] = Convert.ToInt32(dr["cod_tipo_dato"]);
                        arr_inf_fm_cot[1, i + 1] = Convert.ToInt32(dr["ind_anual_mensual"]);
                        arr_inf_fm_cot[2, i + 1] = Convert.ToInt32(dr["ind_aplica_fm"]);

                        i = i + 2;
                    }
                    parametros.ForEach(p => p.ppu_inf_fm = arr_inf_fm_cot);

                    dr.NextResult();

                    // inf_fm_sbs
                    int[,] arr_inf_fm_sbs = new int[3, 8];
                    i = 0;
                    while (dr.Read())
                    {
                        arr_inf_fm_sbs[0, i] = Convert.ToInt32(dr["cod_tipo_dato"]);
                        arr_inf_fm_sbs[1, i] = Convert.ToInt32(dr["ind_anual_mensual"]);
                        arr_inf_fm_sbs[2, i] = Convert.ToInt32(dr["ind_aplica_fm"]);

                        arr_inf_fm_sbs[0, i + 1] = Convert.ToInt32(dr["cod_tipo_dato"]);
                        arr_inf_fm_sbs[1, i + 1] = Convert.ToInt32(dr["ind_anual_mensual"]);
                        arr_inf_fm_sbs[2, i + 1] = Convert.ToInt32(dr["ind_aplica_fm"]);

                        i = i + 2;
                    }
                    parametros.ForEach(p => p.ppu_inf_fm_sbs = arr_inf_fm_sbs);

                    //<SOLFIN-19737>

                    dr.NextResult();

                    //////// ppu_arr_vllx
                    //////i = 0;
                    //////while (dr.Read())
                    //////{
                    //////    parametros
                    //////        .FindAll(p => (
                    //////                       (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2
                    //////                      )
                    //////        )
                    //////        .ForEach(p =>
                    //////        {
                    //////            p.ppu_arr_vllx[i, 0] = Convert.ToDouble(dr["num_tramo"]);
                    //////            p.ppu_arr_vllx[i, 1] = Convert.ToDouble(dr["num_columna"]);
                    //////            p.ppu_arr_vllx[i, 2] = Convert.ToDouble(dr["val_tope"]);
                    //////            p.ppu_arr_vllx[i, 3] = Convert.ToDouble(dr["pje_ajuste_RM"]);
                    //////        });
                    //////    parametros
                    //////        .FindAll(p => !(
                    //////                        (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2
                    //////                       )
                    //////        )
                    //////        .ForEach(p =>
                    //////        {
                    //////            p.ppu_arr_vllx[i, 0] = Convert.ToDouble(dr["num_tramo"]);
                    //////            p.ppu_arr_vllx[i, 1] = Convert.ToDouble(dr["num_columna"]);
                    //////            p.ppu_arr_vllx[i, 2] = Convert.ToDouble(dr["val_tope"]);
                    //////            p.ppu_arr_vllx[i, 3] = Convert.ToDouble(dr["pje_ajuste"]);
                    //////        });
                    //////    i++;
                    //////}

                    //////dr.NextResult();

                    // cot_xml_benefi
                    XElement benefi = null;
                    string correlativo = "";
                    while (dr.Read())
                    {
                        if (!String.Equals(correlativo, dr["wl_num_corr_cotiza"].ToString()))
                        {
                            if (benefi != null)
                            {
                                parametros.FindAll(p => p.cot_num_coti == Convert.ToInt32(correlativo)).ForEach(p =>
                                {
                                    p.cot_xml_benefi = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), benefi).ToString(SaveOptions.DisableFormatting);
                                });
                            }
                            correlativo = dr["wl_num_corr_cotiza"].ToString();
                            benefi = new XElement("BENEFI");
                        }

                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_item", dr["wl_cot_nben"].ToString()),
                                new XElement("fec_fnac", (DateTime.ParseExact(dr["wl_num_nacimiento"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None)).ToString("yyyyMMdd")),
                                //new XElement("fec_fnac", dr["wl_num_nacimiento"].ToString()),
                                new XElement("num_crel", dr["wl_cot_crel_c"].ToString()),
                                new XElement("num_csex", dr["wl_cot_csex_c"].ToString()),
                                new XElement("num_cinv", dr["wl_cot_cinv_c"].ToString()),
                                new XElement("por_prel", dr["wl_val_pje_periodo_diferido"].ToString()),
                                new XElement("por_prrp", dr["wl_val_pje_modificado"].ToString()),
                                new XElement("num_mini", dr["wl_mes_inicio"].ToString()),
                                new XElement("num_mfin", dr["wl_num_termino"].ToString()),
                                new XElement("num_cola", dr["wl_num_columna_tabla"].ToString()),
                                new XElement("num_esta", dr["num_esta"].ToString()),
                                new XElement("num_tben", dr["num_tben"].ToString()),
                                new XElement("flg_igar", dr["wl_cot_igar"].ToString())
                            );
                        benefi.Add(registro);
                    }


                    parametros.FindAll(p => p.cot_num_coti == Convert.ToInt32(correlativo)).ForEach(p =>
                    {
                        p.cot_xml_benefi = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), benefi).ToString(SaveOptions.DisableFormatting);
                    });

                    dr.NextResult();

                    // cot_xml_pje
                    XElement xml_pje = new XElement("insert");
                    while (dr.Read())
                    {
                        XElement formato =
                            new XElement("formato",
                                new XElement("num_solicitud", dr["wl_num_solicitud"].ToString()),
                                new XElement("num_correlativo_cotizacion", dr["wl_num_corr_cotiza"].ToString()),
                                new XElement("num_correlativo", dr["wl_cot_nben"].ToString()),
                                new XElement("cod_tipo_producto", dr["cod_tipo_producto"].ToString()),
                                new XElement("val_pje_base", dr["wl_val_pje_base"].ToString()),
                                new XElement("val_pje_modificado", dr["wl_val_pje_modificado"].ToString()),
                                new XElement("val_pje_ret_periodo_diferido", dr["wl_val_pje_periodo_diferido"].ToString())
                            );
                        xml_pje.Add(formato);
                    }
                    parametros.ForEach(p => p.wl_XML_Pje = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xml_pje).ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    // cot_xml_tabico
                    XElement tabico = new XElement("TABICO");
                    while (dr.Read())
                    {
                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_indi", dr["num_indi"].ToString()),
                                new XElement("num_imon", dr["num_imon"].ToString()),
                                new XElement("val_icop", dr["val_icop"].ToString()),
                                new XElement("val_lcop", dr["val_lcop"].ToString())
                            );
                        tabico.Add(registro);
                    }
                    XDocument xml_tabico = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), tabico);
                    parametros
                        .FindAll(p => (p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue()))
                        .ForEach(p => p.cot_xml_tabico = xml_tabico.ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    tabico = new XElement("TABICO");
                    while (dr.Read())
                    {
                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_indi", dr["num_indi"].ToString()),
                                new XElement("num_imon", dr["num_imon"].ToString()),
                                new XElement("val_icop", dr["val_icop"].ToString()),
                                new XElement("val_lcop", dr["val_lcop"].ToString())
                            );
                        tabico.Add(registro);
                    }
                    xml_tabico = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), tabico);
                    parametros
                        .FindAll(p => (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()))
                        .ForEach(p => p.cot_xml_tabico = xml_tabico.ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    // cot_xml_parash
                    ParametroCotizador par;
                    while (dr.Read())
                    {
                        XDocument xml_parash =
                            new XDocument(
                                new XDeclaration("1.0", "utf-8", "yes"),
                                new XElement("PARASH",
                                    new XElement("Registro",
                                        new XElement("val_cmor", dr["val_cmor"].ToString()),
                                        new XElement("val_fcon", dr["val_fcon"].ToString()),
                                        new XElement("tas_htra", dr["tas_htra"].ToString()),
                                        new XElement("tas_htva", dr["tas_htva"].ToString()),
                                        new XElement("tas_ltra", dr["tas_ltra"].ToString()),
                                        new XElement("tas_ltva", dr["tas_ltva"].ToString()),
                                        new XElement("val_rend", dr["val_rend"].ToString()),
                                        new XElement("tas_tgpd", dr["tas_tgpd"].ToString()),
                                        new XElement("flg_ibtp", dr["flg_ibtp"].ToString()),
                                        new XElement("flg_ivnt", dr["flg_ivnt"].ToString()),
                                        new XElement("flg_ideb", dr["flg_ideb"].ToString()),
                                        new XElement("flg_iajm", dr["flg_iajm"].ToString()),
                                        new XElement("flg_icmo", dr["flg_icmo"].ToString()),
                                        new XElement("tas_timp", dr["tas_timp"].ToString()),
                                        new XElement("tas_tsbs", dr["tas_tsbs"].ToString()),
                                        new XElement("tas_ttec", dr["tas_ttec"].ToString()),
                                        new XElement("val_gfi1", dr["val_gfi1"].ToString()),
                                        new XElement("val_gfi2", dr["val_gfi2"].ToString()),
                                        new XElement("val_comi", dr["val_comi"].ToString()),
                                        new XElement("val_coba", dr["val_coba"].ToString()),
                                        new XElement("val_pumi", dr["val_pumi"].ToString()),
                                        new XElement("num_nins", dr["num_nins"].ToString()),
                                        new XElement("num_nper", dr["num_nper"].ToString()),

                                        new XElement("ini_tra2", dr["ini_tra2"].ToString()),
                                        new XElement("pje_rent", dr["pje_rent"].ToString()),

                                        //<INIGTI_753>
                                        new XElement("pje_devo", dr["pje_dev"].ToString())
                                    //<FINGTI_753>

                                    )
                                )
                            );
                        par = parametros.Find(p => (p.cot_num_coti == Convert.ToInt32(dr["num_correlativo"])) && (p.ind_orden == Convert.ToInt32(dr["ind_orden"])));
                        par.cot_xml_parash = xml_parash.ToString(SaveOptions.DisableFormatting);
                        par.val_ltit = Convert.ToDouble(dr["val_ltit"]);
                        par.val_htit = Convert.ToDouble(dr["val_htit"]);

                        //<INIGTI_1092>
                        par.wl_val_ltra = Convert.ToDouble(dr["tas_ltra"]);
                        //<FINGTI_1092>

                        //<INIGTI_753>
                        par.x_cot_ini_tra2 = Convert.ToInt32(dr["ini_tra2"].ToString());
                        par.x_cot_pje_rent = Convert.ToDouble(dr["pje_rent"].ToString());

                        par.x_cot_pje_devo = Convert.ToInt32(dr["pje_dev"].ToString());

                        par.x_ind_devo = (par.x_cot_pje_devo == 0 || par.x_cot_pje_devo == 25) ? "N" : "S";
                        //<FINGTI_753>

                    }

                    dr.NextResult();

                    // cot_xml_parinv //ARMV AQUI
                    string moneda = "000";
                    string indDevolucion = "000";
                    List<XElement> listaParinv = null;
                    while (dr.Read())
                    {
                        if (!String.Equals(moneda, dr["cod_moneda"].ToString()))
                        {
                            if (listaParinv != null)
                                lParinv.FindAll(p => p.IdMoneda == moneda && p.IndDevolucion == indDevolucion).ForEach(p => p.lParinv = listaParinv);
                            moneda = dr["cod_moneda"].ToString();
                            indDevolucion = dr["ind_devolucion"].ToString();
                            listaParinv = new List<XElement>();
                        }
                        else if (!String.Equals(indDevolucion, dr["ind_devolucion"].ToString()))
                        {
                            if (listaParinv != null)
                                lParinv.FindAll(p => p.IdMoneda == moneda && p.IndDevolucion == indDevolucion).ForEach(p => p.lParinv = listaParinv);
                            moneda = dr["cod_moneda"].ToString();
                            indDevolucion = dr["ind_devolucion"].ToString();
                            listaParinv = new List<XElement>();
                        }

                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_nins", dr["num_instrumento"].ToString()),
                                new XElement("num_nper", dr["num_periodo"].ToString()),
                                new XElement("val_valc", Convert.ToDouble(dr["valc"])),
                                new XElement("por_porc", Convert.ToDouble(dr["porc"])),
                                new XElement("tas_tirc", Convert.ToDouble(dr["tirc"]))
                            );
                        listaParinv.Add(registro);
                    }
                    lParinv.FindAll(p => p.IdMoneda == moneda && p.IndDevolucion == indDevolucion).ForEach(p => p.lParinv = listaParinv);

                    dr.NextResult();

                    moneda = "000";
                    indDevolucion = "000";
                    listaParinv = null;
                    while (dr.Read())
                    {
                        if (!String.Equals(moneda, dr["cod_moneda"].ToString()))
                        {
                            if (listaParinv != null)
                            {
                                lParinv
                                    .FindAll(p => p.IdMoneda == moneda && p.IndDevolucion == indDevolucion)
                                    .ForEach(p => { p.lParinv = p.lParinv.Concat(listaParinv).ToList(); });
                            }
                            moneda = dr["cod_moneda"].ToString();
                            indDevolucion = dr["ind_devolucion"].ToString();
                            listaParinv = new List<XElement>();
                        }
                        else if (!String.Equals(indDevolucion, dr["ind_devolucion"].ToString()))
                        {
                            if (listaParinv != null)
                            {
                                lParinv
                                    .FindAll(p => p.IdMoneda == moneda && p.IndDevolucion == indDevolucion)
                                    .ForEach(p => { p.lParinv = p.lParinv.Concat(listaParinv).ToList(); });
                            }
                            moneda = dr["cod_moneda"].ToString();
                            indDevolucion = dr["ind_devolucion"].ToString();
                            listaParinv = new List<XElement>();
                        }

                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_nins", dr["num_nins"].ToString()),
                                new XElement("num_nflu", dr["num_nflu"].ToString()),
                                new XElement("val_venc", Convert.ToDouble(dr["val_venc"]))
                            );
                        listaParinv.Add(registro);
                    }
                    lParinv
                        .FindAll(p => p.IdMoneda == moneda && p.IndDevolucion == indDevolucion)
                        .ForEach(p => { p.lParinv = p.lParinv.Concat(listaParinv).ToList(); });

                    parametros.ForEach(p => p.cot_xml_parinv = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("PARINV", lParinv.Find(v => v.IdMoneda == p.cod_moneda && v.IndDevolucion == p.x_ind_devo).lParinv)).ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    // cot_xml_ajutdm
                    XElement ajutdm = new XElement("AJUTDM");
                    while (dr.Read())
                    {
                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_tram", dr["num_tramo"].ToString()),
                                new XElement("num_colu", dr["num_columna"].ToString()),
                                new XElement("num_mvig", dr["val_tope"].ToString()),
                                new XElement("val_ajus", dr["pje_ajuste"].ToString())
                            );
                        ajutdm.Add(registro);
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_tram", dr["num_tramo"].ToString()),
                                new XElement("num_colu", dr["num_columna"].ToString()),
                                new XElement("val_puni", dr["val_tope"].ToString()),
                                new XElement("val_ajus", dr["pje_ajuste"].ToString())
                            );
                        ajutdm.Add(registro);
                    }

                    parametros.ForEach(p => p.cot_xml_ajutdm = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), ajutdm).ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    // cot_xml_fluaju
                    XElement fluaju = null;
                    List<double> lFluaju = null;
                    string fdv = "000";
                    while (dr.Read())
                    {
                        if (!String.Equals(fdv, dr["cod_fdv"].ToString()))
                        {
                            if (fluaju != null)
                            {
                                parametros
                                    .FindAll(p => ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2))
                                    .FindAll(p => ((p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue()) && fdv == "NOM") ||
                                                  ((p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()) && fdv == "TFA"))
                                    .ForEach(p =>
                                    {
                                        p.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);
                                        p.fluaju = lFluaju;
                                    });

                                parametros
                                    .FindAll(p => !((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2))
                                    .FindAll(p => ((p.cod_moneda == Enums.Moneda.Soles.StringValue() && fdv == "IPC") ||
                                                   (p.cod_moneda == Enums.Moneda.Dolares.StringValue() && fdv == "NOM") ||
                                                   (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() && fdv == "TFA") ||
                                                   (p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue() && fdv == "TFA")))
                                    .ForEach(p =>
                                    {
                                        p.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);
                                        p.fluaju = lFluaju;
                                    });
                            }
                            fdv = dr["cod_fdv"].ToString();
                            fluaju = new XElement("FLUAJU");
                            lFluaju = new List<double>();
                        }

                        XElement registro =
                            new XElement("Registro",
                                new XElement("indice", dr["num_mes"].ToString()),
                                new XElement("val_ajuste", dr["num_factor"].ToString())
                            );
                        fluaju.Add(registro);
                        lFluaju.Add(Convert.ToDouble(dr["num_factor"]));
                    }
                    parametros
                        .FindAll(p => ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2))
                        .FindAll(p => ((p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue()) && fdv == "NOM") ||
                                      ((p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()) && fdv == "TFA"))
                        .ForEach(p =>
                        {
                            p.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);
                            p.fluaju = lFluaju;
                        });

                    parametros
                        .FindAll(p => !((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2))
                        .FindAll(p => ((p.cod_moneda == Enums.Moneda.Soles.StringValue() && fdv == "IPC") ||
                                       (p.cod_moneda == Enums.Moneda.Dolares.StringValue() && fdv == "NOM") ||
                                       (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() && fdv == "TFA") ||
                                       (p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue() && fdv == "TFA")))
                        .ForEach(p =>
                        {
                            p.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);
                            p.fluaju = lFluaju;
                        });

                    //<INIGTI_753>

                    double x_mon_ajuste = -1;
                    parametros
                        .FindAll(p => p.x_val_mon_aju != -1)
                        .OrderBy(p => p.x_val_mon_aju).ToList()
                        .ForEach(p =>
                        {
                            if (x_mon_ajuste != p.x_val_mon_aju)
                            {
                                x_mon_ajuste = p.x_val_mon_aju;
                                fluaju = new XElement("FLUAJU");
                                lFluaju = ObtenerMonedaAjuste(p.x_val_mon_aju, lnMes, ref fluaju);
                                parametros.FindAll(g => g.x_val_mon_aju == x_mon_ajuste)
                                    .ForEach(g =>
                                    {
                                        g.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);
                                        g.fluaju = lFluaju;
                                    });
                            }
                        });

                    //List<double> lstAjuste = new List<double>();

                    //var parametroAjuste = parametros.Where(y=> y.x_val_mon_aju!=-1).Select(x=> x.x_val_mon_aju ).Distinct();

                    //for (int ii = 0; i < parametroAjuste.Count(); ii++)
                    //{
                    //    var dd= parametroAjuste[0]

                    //}
                    //<FINGTI_753>
                }
                return parametros;
            }
            catch (NullReferenceException ex)
            {
                Exception ex1 = new Exception(String.Format("Esta solicitud [{0}] no contiene los datos suficientes para volver a cotizarse, por favor intente generando una nueva cotización.", idSolicitud), ex);
                throw ex1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //usp_cwrv_carpr_solicitud_RtaPrvd
        }

        //<INIGTI_753>
        public List<double> ObtenerMonedaAjuste(double tasa, int mes, ref XElement fluaju)
        {
            List<Double> lstAjuste = new List<double>();
            mes += 1;
            double residuo;
            double ajuste = 0.0000000;
            double doce = 12.0;
            tasa = 1 + (tasa / 100);
            for (int i = 0; i < 1321; i++)
            {
                if (i == 0)
                {
                    ajuste = Math.Pow((1 * tasa), (i / doce));
                }
                else
                {
                    residuo = (mes + i + 1) % 3;
                    if (residuo == 0)
                    {
                        ajuste = Math.Pow((1 * tasa), (i / doce));
                    }
                }

                XElement registro =
                            new XElement("Registro",
                                new XElement("indice", i.ToString()),
                                new XElement("val_ajuste", ajuste.ToString())
                            );
                fluaju.Add(registro);

                lstAjuste.Add(ajuste);

            }
            return lstAjuste;
        }
        //<FINGTI_753>

        public void RegistrarPjeBeneficiarios(string idSolicitud, string correlativos, string xml_pje, string xml_costo, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.SP_RENVI_INS_PJEBENEFICIARIOS");

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

        public void RegistrarPjeBen(SolicitudRPPlus solicitud)
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

        public void RegistrarCotiza(string xml_cotiza, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_ins_RtaPrvdPlus");

                db.AddInParameter(dbc, "@wl_empdata", DbType.String, xml_cotiza);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<ParametroCotizador> ObtenerParametrosCapitalRequeridoRPPlus(CapitalRequerido capitalRequerido)
        {
            List<ParametroCotizador> parametros = new List<ParametroCotizador>();
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                List<Parinv> lParinv = new List<Parinv>();
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Soles.StringValue() });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Dolares.StringValue() });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.SolesAjustados.StringValue() });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.DolaresAjustados.StringValue() });



                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_carpr_solicitud_RtaPrvd_calculo_capital", capitalRequerido.num_cuissp,
                    capitalRequerido.TipoRenta.Id, capitalRequerido.Moneda.Id, capitalRequerido.periodo_garantizado, capitalRequerido.Temporalidad.Id,
                    capitalRequerido.num_vendedor, capitalRequerido.id_grupo_familiar, capitalRequerido.pension_requerida
                    ))
                {
                    // Parámetros de Cotización
                    ParametroCotizador parametro;
                    while (dr.Read())
                    {
                        parametro = new ParametroCotizador();
                        parametro.cot_gls_ikey = dr["wl_cot_kcot"].ToString();
                        parametro.cot_num_soli = dr["wl_num_solicitud"].ToString();
                        parametro.cot_num_coti = Convert.ToInt32(dr["wl_num_corr_cotiza"]);
                        parametro.ind_orden = Convert.ToInt32(dr["ind_orden"]);
                        parametro.cot_num_mdif = Convert.ToInt32(dr["wl_cot_pdif"]);
                        parametro.cot_num_mgar = Convert.ToInt32(dr["wl_cot_pgar"]);
                        parametro.cot_num_nben = Convert.ToInt32(dr["wl_cot_nben"]);
                        parametro.cot_fec_fcal = Convert.ToInt32(Convert.ToDateTime(dr["wl_cot_fcot"]).ToString("yyyyMMdd"));
                        parametro.cot_fec_fdev = Convert.ToInt32(Convert.ToDateTime(dr["wl_cot_fdev"]).ToString("yyyyMMdd"));
                        parametro.cot_num_tpen = Convert.ToInt32(dr["wl_cot_tpen"]);
                        parametro.cot_num_tcal = Convert.ToInt32(dr["wl_cot_tcal"]);
                        parametro.cot_flg_idac = Convert.ToInt32(dr["wl_cot_derc"]);
                        parametro.cot_flg_igra = Convert.ToInt32(dr["wl_cot_grat"]);
                        parametro.cot_num_cmon = Convert.ToInt32(dr["cot_mon_equi"]);
                        parametro.cot_num_trea = Convert.ToInt32(dr["cot_num_trea"]);
                        parametro.cot_num_frea = Convert.ToInt32(dr["cot_num_frea"]);
                        parametro.cot_flg_irea = Convert.ToInt32(dr["cot_flg_irea"]);
                        parametro.cot_tas_vrea = Convert.ToDouble(dr["wg_val_ajuste_tasa_fija"]);
                        parametro.cot_tas_tasa = Convert.ToDouble(dr["wl_val_tasa_venta"]);
                        parametro.cot_val_vpen = Convert.ToDouble(dr["wl_cot_vpen"]);
                        parametro.cot_tas_vtra = Convert.ToDouble(dr["wl_val_vtra"]);//luego será sumado
                        parametro.cot_tas_vtra_sintra = Convert.ToDouble(dr["wl_val_vtra"]);
                        parametro.cot_tas_tafp = Convert.ToDouble(dr["wl_val_tasa_afp"]);
                        parametro.cot_val_acom = Convert.ToDouble(dr["wl_val_acom"]);
                        parametro.cot_val_dcom = Convert.ToDouble(dr["wl_cot_pdco"]);
                        parametro.cot_val_puam = Convert.ToDouble(dr["wl_val_puam"]);
                        parametro.cot_val_puni = Convert.ToDouble(dr["wl_val_puni"]);
                        parametro.cot_por_prrt = Convert.ToDouble(dr["wl_cot_prrt"]);
                        parametro.cot_val_tgfi = Convert.ToDouble(dr["wl_val_tgfi"]);
                        parametro.cot_val_tope = Convert.ToInt32(dr["val_num_tope"]);

                        // Inicializando los demás parámetros
                        parametro.ppu_vllx = new double[1320, 8];
                        parametro.ppu_arr_vllx = new double[10569, 4];
                        parametro.x_cot_xml_benefi = new StringBuilder(5000);

                        parametro.rango_capital = new double[18, 6];
                        parametro.rango_ajutra = new double[20, 3];

                        // Parámetros auxiliares
                        parametro.fec_cotizacion = Convert.ToDateTime(dr["wl_cot_fcot"]);
                        parametro.wl_val_pension_minimo = Convert.ToDouble(dr["wl_val_pension_minimo"]);
                        parametro.cod_tipo_pension = dr["cod_tipo_pension"].ToString();
                        parametro.cod_tipo_invalidez = dr["cod_tipo_invalidez"].ToString();
                        parametro.cod_moneda = dr["cod_moneda"].ToString();
                        parametro.val_moneda = Convert.ToDouble(dr["val_moneda"]);
                        parametro.cod_tipo_producto = dr["cod_tipo_producto"].ToString();
                        parametro.ind_modalidad = dr["ind_modalidad"].ToString();

                        parametros.Add(parametro);
                    }

                    dr.NextResult();

                    // ppu_vllx
                    double[,] arreglo_ppu_vllx = new double[1321, 8];
                    int i = 0;
                    while (dr.Read())
                    {
                        arreglo_ppu_vllx[i, 0] = Convert.ToDouble(dr["val_lx_mbh"]);
                        arreglo_ppu_vllx[i, 1] = Convert.ToDouble(dr["val_lx_mbm"]);
                        arreglo_ppu_vllx[i, 2] = Convert.ToDouble(dr["val_lx_mih"]);
                        arreglo_ppu_vllx[i, 3] = Convert.ToDouble(dr["val_lx_mim"]);
                        arreglo_ppu_vllx[i, 4] = Convert.ToDouble(dr["val_lx_mvh"]);
                        arreglo_ppu_vllx[i, 5] = Convert.ToDouble(dr["val_lx_mvm"]);
                        arreglo_ppu_vllx[i, 6] = Convert.ToDouble(dr["val_lx_mph"]);
                        arreglo_ppu_vllx[i, 7] = Convert.ToDouble(dr["val_lx_mpm"]);
                        i++;
                    }
                    parametros.ForEach(p => p.ppu_vllx = arreglo_ppu_vllx);

                    dr.NextResult();

                    // ppu_arr_vllx
                    i = 0;
                    while (dr.Read())
                    {
                        parametros
                            .FindAll(p => (
                                           (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2
                                          )
                            )
                            .ForEach(p =>
                            {
                                p.ppu_arr_vllx[i, 0] = Convert.ToDouble(dr["num_tramo"]);
                                p.ppu_arr_vllx[i, 1] = Convert.ToDouble(dr["num_columna"]);
                                p.ppu_arr_vllx[i, 2] = Convert.ToDouble(dr["val_tope"]);
                                p.ppu_arr_vllx[i, 3] = Convert.ToDouble(dr["pje_ajuste_RM"]);
                            });
                        parametros
                            .FindAll(p => !(
                                            (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2
                                           )
                            )
                            .ForEach(p =>
                            {
                                p.ppu_arr_vllx[i, 0] = Convert.ToDouble(dr["num_tramo"]);
                                p.ppu_arr_vllx[i, 1] = Convert.ToDouble(dr["num_columna"]);
                                p.ppu_arr_vllx[i, 2] = Convert.ToDouble(dr["val_tope"]);
                                p.ppu_arr_vllx[i, 3] = Convert.ToDouble(dr["pje_ajuste"]);
                            });
                        i++;
                    }

                    dr.NextResult();

                    // cot_xml_benefi
                    XElement benefi = null;
                    string correlativo = "";
                    while (dr.Read())
                    {
                        if (!String.Equals(correlativo, dr["wl_num_corr_cotiza"].ToString()))
                        {
                            if (benefi != null)
                            {
                                parametros.FindAll(p => p.cot_num_coti == Convert.ToInt32(correlativo)).ForEach(p =>
                                {
                                    p.cot_xml_benefi = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), benefi).ToString(SaveOptions.DisableFormatting);
                                });
                            }
                            correlativo = dr["wl_num_corr_cotiza"].ToString();
                            benefi = new XElement("BENEFI");
                        }

                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_item", dr["wl_cot_nben"].ToString()),
                                new XElement("fec_fnac", (DateTime.ParseExact(dr["wl_num_nacimiento"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None)).ToString("yyyyMMdd")),
                                //new XElement("fec_fnac", dr["wl_num_nacimiento"].ToString()),
                                new XElement("num_crel", dr["wl_cot_crel_c"].ToString()),
                                new XElement("num_csex", dr["wl_cot_csex_c"].ToString()),
                                new XElement("num_cinv", dr["wl_cot_cinv_c"].ToString()),
                                new XElement("por_prel", dr["wl_val_pje_periodo_diferido"].ToString()),
                                new XElement("por_prrp", dr["wl_val_pje_modificado"].ToString()),
                                new XElement("num_mini", dr["wl_mes_inicio"].ToString()),
                                new XElement("num_mfin", dr["wl_num_termino"].ToString()),
                                new XElement("num_cola", dr["wl_num_columna_tabla"].ToString()),
                                new XElement("num_esta", dr["num_esta"].ToString()),
                                new XElement("num_tben", dr["num_tben"].ToString()),
                                new XElement("flg_igar", dr["wl_cot_igar"].ToString())
                            );
                        benefi.Add(registro);
                    }


                    parametros.FindAll(p => p.cot_num_coti == Convert.ToInt32(correlativo)).ForEach(p =>
                    {
                        p.cot_xml_benefi = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), benefi).ToString(SaveOptions.DisableFormatting);
                    });

                    dr.NextResult();

                    // cot_xml_pje
                    XElement xml_pje = new XElement("insert");
                    while (dr.Read())
                    {
                        XElement formato =
                            new XElement("formato",
                                new XElement("num_solicitud", dr["wl_num_solicitud"].ToString()),
                                new XElement("num_correlativo_cotizacion", dr["wl_num_corr_cotiza"].ToString()),
                                new XElement("num_correlativo", dr["wl_cot_nben"].ToString()),
                                new XElement("cod_tipo_producto", dr["cod_tipo_producto"].ToString()),
                                new XElement("val_pje_base", dr["wl_val_pje_base"].ToString()),
                                new XElement("val_pje_modificado", dr["wl_val_pje_modificado"].ToString()),
                                new XElement("val_pje_ret_periodo_diferido", dr["wl_val_pje_periodo_diferido"].ToString())
                            );
                        xml_pje.Add(formato);
                    }
                    parametros.ForEach(p => p.wl_XML_Pje = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), xml_pje).ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    // cot_xml_tabico
                    XElement tabico = new XElement("TABICO");
                    while (dr.Read())
                    {
                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_indi", dr["num_indi"].ToString()),
                                new XElement("num_imon", dr["num_imon"].ToString()),
                                new XElement("val_icop", dr["val_icop"].ToString()),
                                new XElement("val_lcop", dr["val_lcop"].ToString())
                            );
                        tabico.Add(registro);
                    }
                    XDocument xml_tabico = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), tabico);
                    parametros
                        .FindAll(p => (p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue()))
                        .ForEach(p => p.cot_xml_tabico = xml_tabico.ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    tabico = new XElement("TABICO");
                    while (dr.Read())
                    {
                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_indi", dr["num_indi"].ToString()),
                                new XElement("num_imon", dr["num_imon"].ToString()),
                                new XElement("val_icop", dr["val_icop"].ToString()),
                                new XElement("val_lcop", dr["val_lcop"].ToString())
                            );
                        tabico.Add(registro);
                    }
                    xml_tabico = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), tabico);
                    parametros
                        .FindAll(p => (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()))
                        .ForEach(p => p.cot_xml_tabico = xml_tabico.ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    // cot_xml_parash
                    ParametroCotizador par;
                    while (dr.Read())
                    {
                        XDocument xml_parash =
                            new XDocument(
                                new XDeclaration("1.0", "utf-8", "yes"),
                                new XElement("PARASH",
                                    new XElement("Registro",
                                        new XElement("val_cmor", dr["val_cmor"].ToString()),
                                        new XElement("val_fcon", dr["val_fcon"].ToString()),
                                        new XElement("tas_htra", dr["tas_htra"].ToString()),
                                        new XElement("tas_htva", dr["tas_htva"].ToString()),
                                        new XElement("tas_ltra", dr["tas_ltra"].ToString()),
                                        new XElement("tas_ltva", dr["tas_ltva"].ToString()),
                                        new XElement("val_rend", dr["val_rend"].ToString()),
                                        new XElement("tas_tgpd", dr["tas_tgpd"].ToString()),
                                        new XElement("flg_ibtp", dr["flg_ibtp"].ToString()),
                                        new XElement("flg_ivnt", dr["flg_ivnt"].ToString()),
                                        new XElement("flg_ideb", dr["flg_ideb"].ToString()),
                                        new XElement("flg_iajm", dr["flg_iajm"].ToString()),
                                        new XElement("flg_icmo", dr["flg_icmo"].ToString()),
                                        new XElement("tas_timp", dr["tas_timp"].ToString()),
                                        new XElement("tas_tsbs", dr["tas_tsbs"].ToString()),
                                        new XElement("tas_ttec", dr["tas_ttec"].ToString()),
                                        new XElement("val_gfi1", dr["val_gfi1"].ToString()),
                                        new XElement("val_gfi2", dr["val_gfi2"].ToString()),
                                        new XElement("val_comi", dr["val_comi"].ToString()),
                                        new XElement("val_coba", dr["val_coba"].ToString()),
                                        new XElement("val_pumi", dr["val_pumi"].ToString()),
                                        new XElement("num_nins", dr["num_nins"].ToString()),
                                        new XElement("num_nper", dr["num_nper"].ToString())
                                    )
                                )
                            );
                        par = parametros.Find(p => (p.cot_num_coti == Convert.ToInt32(dr["num_correlativo"])) && (p.ind_orden == Convert.ToInt32(dr["ind_orden"])));
                        par.cot_xml_parash = xml_parash.ToString(SaveOptions.DisableFormatting);
                        par.val_ltit = Convert.ToDouble(dr["val_ltit"]);
                        par.val_htit = Convert.ToDouble(dr["val_htit"]);
                    }

                    dr.NextResult();

                    // cot_xml_parinv
                    string moneda = "000";
                    List<XElement> listaParinv = null;
                    while (dr.Read())
                    {
                        if (!String.Equals(moneda, dr["cod_moneda"].ToString()))
                        {
                            if (listaParinv != null)
                                lParinv.FindAll(p => p.IdMoneda == moneda).ForEach(p => p.lParinv = listaParinv);
                            moneda = dr["cod_moneda"].ToString();
                            listaParinv = new List<XElement>();
                        }

                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_nins", dr["num_instrumento"].ToString()),
                                new XElement("num_nper", dr["num_periodo"].ToString()),
                                new XElement("val_valc", Convert.ToDouble(dr["valc"])),
                                new XElement("por_porc", Convert.ToDouble(dr["porc"])),
                                new XElement("tas_tirc", Convert.ToDouble(dr["tirc"]))
                            );
                        listaParinv.Add(registro);
                    }
                    lParinv.FindAll(p => p.IdMoneda == moneda).ForEach(p => p.lParinv = listaParinv);

                    dr.NextResult();

                    moneda = "000";
                    listaParinv = null;
                    while (dr.Read())
                    {
                        if (!String.Equals(moneda, dr["cod_moneda"].ToString()))
                        {
                            if (listaParinv != null)
                            {
                                lParinv
                                    .FindAll(p => p.IdMoneda == moneda)
                                    .ForEach(p => { p.lParinv = p.lParinv.Concat(listaParinv).ToList(); });
                            }
                            moneda = dr["cod_moneda"].ToString();
                            listaParinv = new List<XElement>();
                        }

                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_nins", dr["num_nins"].ToString()),
                                new XElement("num_nflu", dr["num_nflu"].ToString()),
                                new XElement("val_venc", Convert.ToDouble(dr["val_venc"]))
                            );
                        listaParinv.Add(registro);
                    }
                    lParinv
                        .FindAll(p => p.IdMoneda == moneda)
                        .ForEach(p => { p.lParinv = p.lParinv.Concat(listaParinv).ToList(); });

                    parametros.ForEach(p => p.cot_xml_parinv = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("PARINV", lParinv.Find(v => v.IdMoneda == p.cod_moneda).lParinv)).ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    // cot_xml_ajutdm
                    XElement ajutdm = new XElement("AJUTDM");
                    while (dr.Read())
                    {
                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_tram", dr["num_tramo"].ToString()),
                                new XElement("num_colu", dr["num_columna"].ToString()),
                                new XElement("num_mvig", dr["val_tope"].ToString()),
                                new XElement("val_ajus", dr["pje_ajuste"].ToString())
                            );
                        ajutdm.Add(registro);
                    }

                    dr.NextResult();

                    while (dr.Read())
                    {
                        XElement registro =
                            new XElement("Registro",
                                new XElement("num_tram", dr["num_tramo"].ToString()),
                                new XElement("num_colu", dr["num_columna"].ToString()),
                                new XElement("val_puni", dr["val_tope"].ToString()),
                                new XElement("val_ajus", dr["pje_ajuste"].ToString())
                            );
                        ajutdm.Add(registro);
                    }

                    parametros.ForEach(p => p.cot_xml_ajutdm = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), ajutdm).ToString(SaveOptions.DisableFormatting));

                    dr.NextResult();

                    // cot_xml_fluaju
                    XElement fluaju = null;
                    List<double> lFluaju = null;
                    string fdv = "000";
                    while (dr.Read())
                    {
                        if (!String.Equals(fdv, dr["cod_fdv"].ToString()))
                        {
                            if (fluaju != null)
                            {
                                parametros
                                    .FindAll(p => ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2))
                                    .FindAll(p => ((p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue()) && fdv == "NOM") ||
                                                  ((p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()) && fdv == "TFA"))
                                    .ForEach(p =>
                                    {
                                        p.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);
                                        p.fluaju = lFluaju;
                                    });

                                parametros
                                    .FindAll(p => !((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2))
                                    .FindAll(p => ((p.cod_moneda == Enums.Moneda.Soles.StringValue() && fdv == "IPC") ||
                                                   (p.cod_moneda == Enums.Moneda.Dolares.StringValue() && fdv == "NOM") ||
                                                   (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() && fdv == "TFA") ||
                                                   (p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue() && fdv == "TFA")))
                                    .ForEach(p =>
                                    {
                                        p.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);
                                        p.fluaju = lFluaju;
                                    });
                            }
                            fdv = dr["cod_fdv"].ToString();
                            fluaju = new XElement("FLUAJU");
                            lFluaju = new List<double>();
                        }

                        XElement registro =
                            new XElement("Registro",
                                new XElement("indice", dr["num_mes"].ToString()),
                                new XElement("val_ajuste", dr["num_factor"].ToString())
                            );
                        fluaju.Add(registro);
                        lFluaju.Add(Convert.ToDouble(dr["num_factor"]));
                    }
                    parametros
                        .FindAll(p => ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2))
                        .FindAll(p => ((p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue()) && fdv == "NOM") ||
                                      ((p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()) && fdv == "TFA"))
                        .ForEach(p =>
                        {
                            p.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);
                            p.fluaju = lFluaju;
                        });

                    parametros
                        .FindAll(p => !((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2))
                        .FindAll(p => ((p.cod_moneda == Enums.Moneda.Soles.StringValue() && fdv == "IPC") ||
                                       (p.cod_moneda == Enums.Moneda.Dolares.StringValue() && fdv == "NOM") ||
                                       (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() && fdv == "TFA") ||
                                       (p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue() && fdv == "TFA")))
                        .ForEach(p =>
                        {
                            p.cot_xml_fluaju = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), fluaju).ToString(SaveOptions.DisableFormatting);
                            p.fluaju = lFluaju;
                        });


                    dr.NextResult();

                    //rango_capital
                    double[,] arreglo_rango_capital = new double[18, 6];
                    i = 0;
                    while (dr.Read())
                    {
                        arreglo_rango_capital[i, 0] = Convert.ToDouble(dr["pension_inicial"]);
                        arreglo_rango_capital[i, 1] = Convert.ToDouble(dr["pension_final"]);
                        arreglo_rango_capital[i, 2] = Convert.ToDouble(dr["prima_unica_inicial"]);
                        arreglo_rango_capital[i, 3] = Convert.ToDouble(dr["prima_unica_final"]);
                        arreglo_rango_capital[i, 4] = Convert.ToDouble(dr["prioridad"]);
                        arreglo_rango_capital[i, 5] = Convert.ToDouble(dr["porcentaje_sueldo"]);
                        i++;
                    }
                    parametros.ForEach(p => p.rango_capital = arreglo_rango_capital);

                    dr.NextResult();

                    //Moneda Equivalente TipoCambio
                    while (dr.Read())
                    {
                        capitalRequerido.simbolo_moneda_equi = Convert.ToString(dr["simbolo_moneda"]);
                        capitalRequerido.tipo_cambio = Convert.ToDouble(dr["tipo_cambio"]);
                    }

                    dr.NextResult();

                    //Lista de Ajutra
                    double[,] arreglo_rango_ajutra = new double[20, 3];
                    i = 0;
                    while (dr.Read())
                    {
                        arreglo_rango_ajutra[i, 0] = Convert.ToDouble(dr["val_AjuTRA"]);
                        arreglo_rango_ajutra[i, 1] = Convert.ToDouble(dr["num_rango_ini"]);
                        arreglo_rango_ajutra[i, 2] = Convert.ToDouble(dr["num_rango_fin"]);
                        i++;
                    }

                    parametros.ForEach(p => p.rango_ajutra = arreglo_rango_ajutra);


                }
                return parametros;
            }
            catch (NullReferenceException ex)
            {
                Exception ex1 = new Exception(String.Format("Esta solicitud no contiene los datos suficientes para volver a cotizarse, por favor intente generando una nueva cotización."), ex);
                throw ex1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Registrar(SolicitudRPPlus entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(SolicitudRPPlus entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(SolicitudRPPlus entity)
        {
            throw new NotImplementedException();
        }

        public SolicitudRPPlus ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public SolicitudRPPlus ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<SolicitudRPPlus> Listar()
        {
            throw new NotImplementedException();
        }

        public void VistaPreviaCotizacion(string num_solicitud, int num_correlativo, string usuario, ref string cod_tipo_cotizacion)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_vistaprevia_cotizacion_RtaPrvdPlus", num_solicitud, num_correlativo, usuario))
                {
                    if (dr.Read())
                    {
                        cod_tipo_cotizacion = dr["cod_tipo_cotizacion"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //<INIGTI_7012>
        public void CerrarCotizacion(string num_solicitud, int num_correlativo, string usuario, ref string cod_tipo_cotizacion)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                //SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_cerrar_cotizacion_RtaPrvdPlus");
                //db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, num_solicitud);
                //db.AddInParameter(dbc, "@wl_num_correlativo", DbType.Int64, num_correlativo);
                //db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);
                //db.ExecuteNonQuery(dbc);
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_cerrar_cotizacion_RtaPrvdPlus", num_solicitud, num_correlativo, usuario))
                {
                    if (dr.Read())
                    {
                        cod_tipo_cotizacion = dr["cod_tipo_cotizacion"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ObtenerSecuenciaPolizaPlus()
        {
            string valSecuencia = "";
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_secuencia_poliza_RtaPrvdPlus"))
            {
                while (dr.Read())
                {
                    valSecuencia = dr["val_secuencia"].ToString();
                }
            }
            return valSecuencia;
        }

        public void GenerarPolizaPlus(string num_solicitud, int num_correlativo, Int64 num_poliza, string dig_poliza, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_generar_poliza_RtaPrvdPlus");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, num_solicitud);
                db.AddInParameter(dbc, "@wl_num_correlativo", DbType.Int64, num_correlativo);
                db.AddInParameter(dbc, "@wl_num_poliza", DbType.Int64, num_poliza);
                db.AddInParameter(dbc, "@wl_dig_poliza", DbType.String, dig_poliza);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AnularSolicitud(string num_solicitud, string usuario, string cod_causante)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_anular_solicitud_RtaPrvdPlus");
                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, num_solicitud);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);
                db.AddInParameter(dbc, "@wl_cod_causante", DbType.String, cod_causante);
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //<FINGTI_7012>

        //<INI.GTI_7012_2_1>
        public List<SolicitudRPPlus> ListarReporte(string cuspp)
        {
            List<SolicitudRPPlus> lstSolicitudRPPlus = new List<SolicitudRPPlus>();
            SolicitudRPPlus SolicitudRPPlus = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_solicitudes_RtaPrvdPlus", cuspp))
            {
                // Datos de Solicitud
                while (dr.Read())
                {
                    SolicitudRPPlus = new SolicitudRPPlus();
                    SolicitudRPPlus.Beneficiarios = new List<GrupoFamiliar>();
                    SolicitudRPPlus.Id = dr["num_solicitud"].ToString();
                    if (dr["fec_solicitud"] != DBNull.Value)
                        SolicitudRPPlus.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    if (dr["fec_cotizacion"] != DBNull.Value)
                        SolicitudRPPlus.FechaCotizacion = Convert.ToDateTime(dr["fec_cotizacion"]);
                    SolicitudRPPlus.Afiliado = new Afiliado
                    {
                        CUSPP = dr["num_cuispp"].ToString(),
                        CorreoElectronico = (dr["gls_mail"] != DBNull.Value && dr["gls_mail"].ToString() != string.Empty) ? dr["gls_mail"].ToString() : null,
                        AFP = new AFP { Id = dr["cod_afp"].ToString() }
                    };
                    SolicitudRPPlus.AFP = new AFP { Id = dr["cod_afp"].ToString() };
                    SolicitudRPPlus.Afiliado.AFP = new AFP { Id = dr["cod_afp"].ToString() };
                    SolicitudRPPlus.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString(), Nombre = dr["gls_tipo_cotizacion"].ToString() };
                    SolicitudRPPlus.TipoPension = new TipoPension { Id = dr["cod_tipo_pension"].ToString() };
                    if (dr["fec_devengue"] != DBNull.Value)
                        SolicitudRPPlus.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);
                    if (dr["fec_ult_actualizacion"] != DBNull.Value)
                    {
                        SolicitudRPPlus.FechaUltimaActualizacion = Convert.ToDateTime(dr["fec_ult_actualizacion"]);
                    }

                    SolicitudRPPlus.PrimaUnica = Convert.ToDouble(dr["val_mto_cta_individual"]);

                    SolicitudRPPlus.TipoCambio = 0;
                    if (dr["val_tasa_cambio"] != DBNull.Value)
                        SolicitudRPPlus.TipoCambio = Convert.ToDouble(dr["val_tasa_cambio"]);

                    SolicitudRPPlus.FactorTasa = dr["cod_factor_tasa"].ToString();

                    if (dr["pje_descuento_comision"] != DBNull.Value)
                        SolicitudRPPlus.PorcentajeDescuentoComision = Convert.ToDouble(dr["pje_descuento_comision"]);
                    else
                        SolicitudRPPlus.PorcentajeDescuentoComision = 0;
                    SolicitudRPPlus.Categoria = new Categoria { Id = dr["cod_categoria"].ToString() };

                    SolicitudRPPlus.Agente = new Agente { Id = dr["num_agente"].ToString(), Nombre = dr["nom_agente"].ToString() };
                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        SolicitudRPPlus.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"])
                        };

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value && dr["cod_tipo_temporalidad"].ToString() != string.Empty)
                        SolicitudRPPlus.Temporalidad = new Temporalidad
                        {
                            Id = Convert.ToString(dr["cod_tipo_temporalidad"]),
                            Nombre = (dr["gls_tipo_temporalidad"] != DBNull.Value && dr["gls_tipo_temporalidad"].ToString() != string.Empty) ? dr["gls_tipo_temporalidad"].ToString() : null,
                            Anhos = Convert.ToInt32(dr["anhos_temporalidad"])

                        };

                    if (dr["cod_moneda_cta_indiv"] != DBNull.Value && dr["cod_moneda_cta_indiv"].ToString() != string.Empty)
                        SolicitudRPPlus.MonedaPrimaUnica = new Moneda
                        {
                            Id = Convert.ToString(dr["cod_moneda_cta_indiv"]),
                            Nombre = dr["gls_moneda"].ToString(),
                            Simbolo = dr["gls_simbolo"].ToString()
                        };

                    if (dr["cod_tipo_plan_rpp"] != DBNull.Value && dr["cod_tipo_plan_rpp"].ToString() != string.Empty)
                        SolicitudRPPlus.TipoPlan = new TipoPlan
                        {
                            Id = Convert.ToString(dr["cod_tipo_plan_rpp"]),
                            Nombre = (dr["gls_tipo_plan_rpp"] != DBNull.Value && dr["gls_tipo_plan_rpp"].ToString() != string.Empty) ? dr["gls_tipo_plan_rpp"].ToString() : null
                        };

                    if (dr["fec_vigencia"] != DBNull.Value)
                        SolicitudRPPlus.FechaVigencia = Convert.ToDateTime(dr["fec_vigencia"]);

                    if (dr["num_poliza"] != DBNull.Value)
                        SolicitudRPPlus.NumeroPoliza = Convert.ToInt32(dr["num_poliza"]);

                    SolicitudRPPlus.CodigoEstado = 0;
                    if (dr["cod_estado_rpp"] != DBNull.Value)
                        SolicitudRPPlus.CodigoEstado = Convert.ToInt32(dr["cod_estado_rpp"]);

                    SolicitudRPPlus.EstadoSolicitud = "";
                    if (dr["gls_estado_solicitud"] != DBNull.Value)
                        SolicitudRPPlus.EstadoSolicitud = dr["gls_estado_solicitud"].ToString();

                    SolicitudRPPlus.CodigoEstadoPoliza = "";
                    if (dr["cod_estado_poliza"] != DBNull.Value)
                        SolicitudRPPlus.CodigoEstadoPoliza = dr["cod_estado_poliza"].ToString();

                    SolicitudRPPlus.EstadoPoliza = "";
                    if (dr["gls_estado_poliza"] != DBNull.Value)
                        SolicitudRPPlus.EstadoPoliza = dr["gls_estado_poliza"].ToString();

                    SolicitudRPPlus.CausalPoliza = new CausalPoliza();
                    if (dr["cod_causal_estado_poliza"] != DBNull.Value)
                        SolicitudRPPlus.CausalPoliza = new CausalPoliza
                        {
                            Id = dr["cod_causal_estado_poliza"].ToString(),
                            NombreLargo = dr["gls_causal_estado_poliza"].ToString()
                        };

                    //<INI.GTI_7012_26>
                    SolicitudRPPlus.CodigoEstadoPlaft = 0;
                    if (dr["cod_estado_plaft"] != DBNull.Value)
                        SolicitudRPPlus.CodigoEstadoPlaft = Convert.ToInt32(dr["cod_estado_plaft"]);

                    //<FIN.GTI_7012_26>
                    SolicitudRPPlus.Cotizaciones = new List<CotizacionRPPlus>();
                    lstSolicitudRPPlus.Add(SolicitudRPPlus);
                }

                dr.NextResult();

                // Cotizaciones
                while (dr.Read())
                {
                    CotizacionRPPlus cotizacion = new CotizacionRPPlus();
                    cotizacion.Correlativo = Convert.ToInt64(dr["num_correlativo"]);
                    cotizacion.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString() };
                    cotizacion.Producto = new Producto { Id = dr["cod_tipo_producto"].ToString(), Nombre = dr["gls_tipo_producto"].ToString() };
                    cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["val_per_garantizado"]);
                    cotizacion.AjusteTRA = Convert.ToDouble(dr["val_Tasa_ajuste_tra"]);

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
                        cotizacion.PagoEscalonada = Convert.ToDouble(dr["val_per_temporal"]);

                    if (dr["val_pje_rent_temp"] != DBNull.Value)
                        cotizacion.PjePE = Convert.ToDouble(dr["val_pje_rent_temp"]);

                    cotizacion.IndGastoSepelio = "N";
                    if (dr["ind_gasto_sepelio"] != DBNull.Value)
                        cotizacion.IndGastoSepelio = dr["ind_gasto_sepelio"].ToString();

                    if (dr["val_pje_dev"] != DBNull.Value)
                        cotizacion.ValPjeDev = Convert.ToDouble(dr["val_pje_dev"]);

                    if (dr["val_1era_prima_is"] != DBNull.Value)
                    {
                        cotizacion.Pension2doTramo = Convert.ToDouble(dr["val_1era_prima_is"]);
                        cotizacion.Pension2doTramoSinAjuste = cotizacion.PensionCiaMO * (cotizacion.PjePE / 100);
                    }

                    if (dr["ind_cotiza"] != DBNull.Value)
                        cotizacion.IndCotiza = dr["ind_cotiza"].ToString();

                    if (dr["ind_error_cotiza"] != DBNull.Value)
                        cotizacion.IndErrorCotiza = Convert.ToInt32(dr["ind_error_cotiza"]);

                    if (dr["val_pje_conyuge"] != DBNull.Value)
                    {
                        cotizacion.ValPjeConyuge = Convert.ToDouble(dr["val_pje_conyuge"]);
                    }
                    else
                    {
                        cotizacion.ValPjeConyuge = -1;
                    };

                    if (dr["val_mon_aju"] != DBNull.Value)
                    {
                        cotizacion.ValMonAju = Convert.ToDouble(dr["val_mon_aju"]);
                    }
                    else
                    {
                        cotizacion.ValMonAju = -1;
                    };

                    cotizacion.EstadoCotizacion = dr["cod_estado_cotizacion"].ToString();
                    cotizacion.IndSeleccionada = dr["ind_cotizacion_seleccionada"].ToString();

                    cotizacion.NumSolicitud = dr["num_solicitud"].ToString();

                    if (dr["val_total_garantizado"] != DBNull.Value)
                        cotizacion.ValTotalPeriodoGarantizado = Convert.ToDouble(dr["val_total_garantizado"]);

                    lstSolicitudRPPlus
                        .FindAll(p => (p.Id == dr["num_solicitud"].ToString()))
                        .ForEach(p => p.Cotizaciones.Add(cotizacion));
                }


                dr.NextResult();

                // beneficiarios

                while (dr.Read())
                {

                    GrupoFamiliar grupoFamiliar = new GrupoFamiliar();
                    grupoFamiliar.ApellidoMaterno = dr["ape_materno"].ToString();
                    grupoFamiliar.ApellidoPaterno = dr["ape_paterno"].ToString();
                    grupoFamiliar.Nombre = dr["nom_persona"].ToString();

                    grupoFamiliar.Identificacion = new Identificacion { IdTipo = dr["cod_tipo_identificacion"].ToString(), GlosaTipo = dr["gls_corta_identificacion"].ToString(), Numero = dr["num_identificacion"].ToString() };


                    lstSolicitudRPPlus
                        .FindAll(p => (p.Afiliado.CUSPP == dr["num_cuissp"].ToString()))
                        .ForEach(p => p.Beneficiarios.Add(grupoFamiliar));
                }


            }

            return lstSolicitudRPPlus;
        }
        //<FIN.GTI_7012_2_1>

        public void ActualizarEnvioADMWR(string num_poliza, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_poliza_envio_admwr");
                db.AddInParameter(dbc, "@wl_num_poliza", DbType.String, num_poliza);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizacionSolicitudPlusPlaft(string num_solicitud, string cod_tipo_flujo_evaluacion, int cod_estado, string gls_observacion, string gls_archivos_existentes, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_solicitud_RtaPrvdPlusPlaft");
                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, num_solicitud);
                db.AddInParameter(dbc, "@wl_cod_tipo_flujo_evaluacion", DbType.String, cod_tipo_flujo_evaluacion);
                db.AddInParameter(dbc, "@wl_cod_estado", DbType.Int32, cod_estado);
                db.AddInParameter(dbc, "@wl_gls_observacion", DbType.String, gls_observacion);
                db.AddInParameter(dbc, "@wl_gls_archivos_existentes", DbType.String, gls_archivos_existentes);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);
                db.ExecuteNonQuery(dbc);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public SolicitudRPPlus ObtenerEstado(string num_solicitud)
        {
            SolicitudRPPlus SolicitudRPPlus = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_estado_solicitud_RtaPrvdPlusPlaft", num_solicitud))
            {
                // Datos de Solicitud
                if (dr.Read())
                {
                    SolicitudRPPlus = new SolicitudRPPlus();
                    SolicitudRPPlus.Id = dr["num_solicitud"].ToString();

                    Afiliado afiliado = new Afiliado();

                    afiliado.CUSPP = dr["num_cuispp"].ToString();

                    //SolicitudIFP.Afiliado = new Afiliado
                    //{
                    //    CUSPP = = dr["num_cuispp"].ToString()
                    //};

                    SolicitudRPPlus.Agente = new Agente { Id = dr["num_agente"].ToString(), Nombre = dr["nom_agente"].ToString(), Usuario = dr["cod_username"].ToString() };

                    SolicitudRPPlus.CodigoEstado = 0;
                    if (dr["cod_estado_rpp"] != DBNull.Value)
                        SolicitudRPPlus.CodigoEstado = Convert.ToInt32(dr["cod_estado_rpp"]);

                    SolicitudRPPlus.CodigoEstadoPlaft = 0;
                    if (dr["cod_estado_plaft"] != DBNull.Value)
                        SolicitudRPPlus.CodigoEstadoPlaft = Convert.ToInt32(dr["cod_estado_plaft"]);

                    SolicitudRPPlus.EstadoSolicitud = "";
                    if (dr["gls_estado_solicitud"] != DBNull.Value)
                        SolicitudRPPlus.EstadoSolicitud = dr["gls_estado_solicitud"].ToString();

                    SolicitudRPPlus.GlsObservacionRpp = dr["gls_observacion_rpp"].ToString();

                    SolicitudRPPlus.GlsObservacionPlaft = dr["gls_observacion_plaft"].ToString();

                    if (dr["fec_solicitud"] != DBNull.Value)
                        SolicitudRPPlus.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);

                    afiliado.Agente = new Agente { Id = dr["num_vendedor"].ToString() };
                    afiliado.AFP = new AFP { Id = dr["cod_afp"].ToString() };

                    SolicitudRPPlus.Afiliado = afiliado;

                }
            }

            return SolicitudRPPlus;
        }

        public void RegistrarFlujoEvaluacion(FlujoEvaluacion flujoEvaluacion)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_flujo_evaluacion_rpp");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, flujoEvaluacion.num_solicitud);
                db.AddInParameter(dbc, "@wl_cod_tipo_flujo_evaluacion", DbType.Int32, flujoEvaluacion.cod_tipo_flujo_evaluacion);
                db.AddInParameter(dbc, "@wl_gls_observacion", DbType.String, flujoEvaluacion.gls_observacion);
                db.AddInParameter(dbc, "@wl_fec_inicio_flujo_evaluacion", DbType.DateTime, flujoEvaluacion.fec_inicio_flujo_evaluacion);
                db.AddInParameter(dbc, "@wl_gls_archivos_existentes", DbType.String, flujoEvaluacion.gls_archivos_existentes);
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, flujoEvaluacion.aud_usr_ingreso);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //<INI_GTI_7012_S16>
        public List<SolicitudRPPlus> ListarSolicitudEvaluacion()
        {
            List<SolicitudRPPlus> listaSolicitudes = new List<SolicitudRPPlus>();
            SolicitudRPPlus solicitud;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solicitudes_RPP_Evaluacion"))

            {
                while (dr.Read())
                {
                    solicitud = new SolicitudRPPlus();
                    solicitud.Id = dr["num_solicitud"].ToString();
                    if (dr["fec_solicitud"] != DBNull.Value)
                        solicitud.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    solicitud.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                    if (dr["fec_devengue"] != DBNull.Value)
                        solicitud.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);

                    if (dr["cod_moneda_cta_indiv"] != DBNull.Value)
                        solicitud.MonedaPrimaUnica = new Moneda { Simbolo = dr["cod_moneda_cta_indiv"].ToString() };

                    if (dr["mto_prima_unica"] != DBNull.Value)
                        solicitud.PrimaUnica = Convert.ToDouble(dr["mto_prima_unica"]);

                    solicitud.Agente = new Agente { Id = dr["num_agente"].ToString() };

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        solicitud.Temporalidad = new Temporalidad { Nombre = dr["cod_tipo_temporalidad"].ToString() };

                    solicitud.CodigoEstado = Convert.ToInt32(dr["cod_estado_rpp"].ToString());
                    solicitud.EstadoSolicitud = dr["gls_estado_solicitud"].ToString();

                    if (dr["fec_vigencia"] != DBNull.Value)
                        solicitud.FechaVigencia = Convert.ToDateTime(dr["fec_vigencia"]);

                    solicitud.CodigoEstadoPlaft = Convert.ToInt32(dr["cod_estado_plaft"].ToString());
                    solicitud.EstadoSolicitudPlaft = dr["gls_estado_plaft"].ToString();

                    solicitud.Afiliado = new Afiliado { Nombre = dr["gls_persona"].ToString(), CUSPP = dr["num_cuispp"].ToString(), Agente = new Agente { Id = dr["num_agente"].ToString() } };

                    solicitud.Beneficiarios = new List<GrupoFamiliar>();
                    solicitud.Beneficiarios.Add(new GrupoFamiliar
                    {
                        ApellidoPaterno = dr["ape_paterno"].ToString(),
                        ApellidoMaterno = dr["ape_materno"].ToString(),
                        Nombre = dr["nom_persona"].ToString(),
                        ApellidosNombres = dr["gls_persona"].ToString(),
                        Identificacion = new Identificacion
                        {
                            IdTipo = dr["cod_tipo_identificacion"].ToString(),
                            GlosaTipo = dr["gls_corta_identificacion"].ToString(),
                            //Numero = Convert.ToInt32(dr["num_identificacion"]),
                            Numero = dr["num_identificacion"].ToString()
                        }

                    });

                    listaSolicitudes.Add(solicitud);
                }
            }

            return listaSolicitudes;
        }

        public List<SolicitudRPPlus> ListarSolicitudCierres(string cuspp)
        {
            List<SolicitudRPPlus> listaSolicitudes = new List<SolicitudRPPlus>();
            SolicitudRPPlus solicitud;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solicitudes_cierres_RtaPrvdPlus", cuspp))
            {
                while (dr.Read())
                {
                    solicitud = new SolicitudRPPlus();
                    solicitud.Id = dr["num_solicitud"].ToString();
                    if (dr["fec_solicitud"] != DBNull.Value)
                        solicitud.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    solicitud.TipoCotizacion = new TipoCotizacion
                    {
                        Id = dr["cod_tipo_cotizacion"].ToString(),
                        Nombre = dr["gls_tipo_cotizacion"].ToString()
                    };

                    if (dr["fec_devengue"] != DBNull.Value)
                        solicitud.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);

                    if (dr["cod_moneda_cta_indiv"] != DBNull.Value)
                        solicitud.MonedaPrimaUnica = new Moneda { Simbolo = dr["cod_moneda_cta_indiv"].ToString() };


                    if (dr["mto_prima_unica"] != DBNull.Value)
                        solicitud.PrimaUnica = Convert.ToDouble(dr["mto_prima_unica"]);

                    solicitud.Agente = new Agente { Id = dr["num_agente"].ToString() };

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value)
                        solicitud.Temporalidad = new Temporalidad { Nombre = dr["cod_tipo_temporalidad"].ToString() };

                    solicitud.CodigoEstado = Convert.ToInt32(dr["cod_estado_rpp"].ToString());
                    solicitud.EstadoSolicitud = dr["gls_estado_solicitud"].ToString();

                    if (dr["fec_vigencia"] != DBNull.Value)
                        solicitud.FechaVigencia = Convert.ToDateTime(dr["fec_vigencia"]);

                    solicitud.CodigoEstadoPlaft = Convert.ToInt32(dr["cod_estado_plaft"].ToString());

                    solicitud.EstadoSolicitud = dr["gls_estado_solicitud"].ToString();
                    solicitud.EstadoSolicitudPlaft = dr["gls_estado_plaft"].ToString();

                    listaSolicitudes.Add(solicitud);
                }
            }

            return listaSolicitudes;
        }

        
    }
}
