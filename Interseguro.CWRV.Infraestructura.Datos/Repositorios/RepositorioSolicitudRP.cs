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
    public class RepositorioSolicitudRP: IRepositorioSolicitudRP
    {
        public List<SolicitudRP> Listar(string cuspp)
        {
            List<SolicitudRP> listaSolicitudes = new List<SolicitudRP>();
            SolicitudRP solicitud;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solicitudes_RtaPrvd", cuspp))
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
                    solicitud = new SolicitudRP();
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

                    listaSolicitudes.Add(solicitud);
                }
            }

            return listaSolicitudes;
        }

        public void Registrar(ref SolicitudRP entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_solicitud_RtaPrvd");

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

        public void Actualizar(ref SolicitudRP entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_solicitud_RtaPrvd");

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

        public SolicitudRP ObtenerDatos(string idSolicitud, DateTime fecCotizacion)
        {
            SolicitudRP solicitudRP = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_solicitud_RtaPrvd", idSolicitud, fecCotizacion))
            {
                // Datos de Solicitud
                if (dr.Read())
                {
                    solicitudRP = new SolicitudRP();
                    solicitudRP.Id = dr["num_solicitud"].ToString();
                    if (dr["fec_solicitud"] != DBNull.Value)
                        solicitudRP.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    if (dr["fec_cotizacion"] != DBNull.Value)
                        solicitudRP.FechaCotizacion = Convert.ToDateTime(dr["fec_cotizacion"]);
                    solicitudRP.Afiliado = new Afiliado
                    {
                        CUSPP = dr["num_cuispp"].ToString(),
                        CorreoElectronico = (dr["gls_mail"] != DBNull.Value && dr["gls_mail"].ToString() != string.Empty) ? dr["gls_mail"].ToString() : null,
                        AFP = new AFP { Id = dr["cod_afp"].ToString() }
                    };
                    solicitudRP.AFP = new AFP { Id = dr["cod_afp"].ToString() };
                    solicitudRP.Afiliado.AFP = new AFP { Id = dr["cod_afp"].ToString() };
                    solicitudRP.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                    solicitudRP.TipoPension = new TipoPension { Id = dr["cod_tipo_pension"].ToString() };
                    if (dr["fec_devengue"] != DBNull.Value)
                        solicitudRP.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);
                    if (dr["fec_ult_actualizacion"] != DBNull.Value)
                    {
                        //<GTIINI-754>
                        //solicitudRP.FechaSolicitud = Convert.ToDateTime(dr["fec_ult_actualizacion"]);
                        solicitudRP.FechaUltimaActualizacion = Convert.ToDateTime(dr["fec_ult_actualizacion"]);
                        //<GTIFIN-754>
                    }
                    ////if (dr["fec_recepcion"] != DBNull.Value)
                    ////    solicitud.FechaRecepcion = Convert.ToDateTime(dr["fec_recepcion"]);
                    ////if (dr["fec_presentacion"] != DBNull.Value)
                    ////    solicitud.FechaPlazoAFP = Convert.ToDateTime(dr["fec_presentacion"]);
                    solicitudRP.PrimaUnica = Convert.ToDouble(dr["val_mto_cta_individual"]); //AQUI ERA .SaldoCIC  //ARMV

                    solicitudRP.TipoCambio = 0;
                    if (dr["val_tasa_cambio"] != DBNull.Value)
                        solicitudRP.TipoCambio = Convert.ToDouble(dr["val_tasa_cambio"]);

                    solicitudRP.FactorTasa = dr["cod_factor_tasa"].ToString();
                    ////if (dr["fec_sol_pension"] != DBNull.Value)
                    ////    solicitud.FechaSolicitudPension = Convert.ToDateTime(dr["fec_sol_pension"]);
                    //////<SRIINI06326>
                    ////if (dr["pje_aumento_comision"] != DBNull.Value)
                    ////    solicitud.PorcentajeAumentoComision = Convert.ToDouble(dr["pje_aumento_comision"]);
                    ////else
                    ////    solicitud.PorcentajeAumentoComision = 0;
                    if (dr["pje_descuento_comision"] != DBNull.Value)
                        solicitudRP.PorcentajeDescuentoComision = Convert.ToDouble(dr["pje_descuento_comision"]);
                    else
                        solicitudRP.PorcentajeDescuentoComision = 0;
                    solicitudRP.Categoria = new Categoria { Id = dr["cod_categoria"].ToString() };
                    //<SRIFIN06326>
                    //<SRI.INI-20322_E2>
                    solicitudRP.Agente = new Agente { Id = dr["num_agente"].ToString(), IdCartera = dr["cod_cartera"].ToString() };
                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        solicitudRP.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"])
                            //Nombre = (dr["gls_tipo_movimiento"] != DBNull.Value && dr["gls_tipo_movimiento"].ToString() != string.Empty) ? dr["gls_tipo_movimiento"].ToString() : null
                        };

                    if (dr["cod_tipo_temporalidad"] != DBNull.Value && dr["cod_tipo_temporalidad"].ToString() != string.Empty)
                        solicitudRP.Temporalidad = new Temporalidad
                        {
                            Id = Convert.ToString(dr["cod_tipo_temporalidad"]),
                            Nombre = (dr["gls_tipo_temporalidad"] != DBNull.Value && dr["gls_tipo_temporalidad"].ToString() != string.Empty) ? dr["gls_tipo_temporalidad"].ToString() : null
                        };

                    if (dr["cod_moneda_cta_indiv"] != DBNull.Value && dr["cod_moneda_cta_indiv"].ToString() != string.Empty)
                        solicitudRP.MonedaPrimaUnica = new Moneda
                        {
                            Id = Convert.ToString(dr["cod_moneda_cta_indiv"])
                        };

                    //<SRI.FIN-20322_E2>
                }

                dr.NextResult();

                // Cotizaciones
                solicitudRP.Cotizaciones = new List<CotizacionRP>();
                while (dr.Read())
                {
                    CotizacionRP cotizacion = new CotizacionRP();
                    cotizacion.Correlativo = Convert.ToInt64(dr["num_correlativo"]);
                    cotizacion.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString() };
                    cotizacion.Producto = new Producto { Id = dr["cod_tipo_producto"].ToString(), Nombre = dr["gls_tipo_producto"].ToString() };
                    cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["val_per_garantizado"]);
                    cotizacion.AjusteTRA = Convert.ToDouble(dr["val_Tasa_ajuste_tra"]);

                    if (dr["val_pen_cia"] != DBNull.Value)
                        cotizacion.PensionCia = Convert.ToDouble(dr["val_pen_cia"]);
                    if (dr["val_pen_cia_mo"] != DBNull.Value)
                        cotizacion.PensionCiaMO = Convert.ToDouble(dr["val_pen_cia"]);

                    if (dr["val_tasa_venta_ash"] != DBNull.Value)
                        cotizacion.TasaVenta = Convert.ToDouble(dr["val_tasa_venta_ash"]);
                    if (dr["val_tasa_int_vit"] != DBNull.Value)
                        cotizacion.TasaVentaSbs = Convert.ToDouble(dr["val_tasa_int_vit"]);
                    if (dr["val_tra"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionista = Convert.ToDouble(dr["val_tra"]);

                    solicitudRP.Cotizaciones.Add(cotizacion);
                }
                dr.NextResult();

                // Beneficiarios
                solicitudRP.Beneficiarios = new List<GrupoFamiliar>();
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

                    solicitudRP.Beneficiarios.Add(beneficiario);
                }
            }

            return solicitudRP;
        }

        public List<ParametroCotizador> ObtenerParametrosCotizacionRP(string idSolicitud, DateTime fecCotizacion, Int64? numCorrelativo)
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

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_carpr_solicitud_RtaPrvd", idSolicitud, fecCotizacion, numCorrelativo))
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

        public List<ParametroCotizador> ObtenerParametrosCapitalRequeridoRP(CapitalRequerido capitalRequerido)
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
                        capitalRequerido.tipo_cambio =  Convert.ToDouble(dr["tipo_cambio"]);
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
            //usp_cwrv_carpr_solicitud_RtaPrvd
        }

        public List<FormatoSolicitud> ListarFormatosSolicitud(string solicitud, string usuario)
        {
            try
            {
                List<FormatoSolicitud> formatos = new List<FormatoSolicitud>();

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_formato_solicitud", solicitud, usuario))
                {
                    while (dr.Read())
                    {
                        FormatoSolicitud formato = new FormatoSolicitud();
                        formato.Id = Convert.ToInt32(dr["id_formato_solicitud"]);
                        formato.NumeroSolicitud = dr["num_solicitud"].ToString();
                        formato.Correlativo = Convert.ToInt32(dr["num_correlativo"]);
                        formato.TipoCotizacion = new TipoCotizacion();
                        formato.TipoCotizacion.Id = dr["cod_tipo_cotizacion"].ToString();
                        formato.Afiliado = new FormatoSolicitudBeneficiario();
                        formato.Afiliado.PEP = Convert.ToChar(dr["ind_pep"]);
                        formato.Afiliado.SujetoObligado = Convert.ToChar(dr["ind_sujeto_obligado"]);
                        formato.Rescate = Convert.ToBoolean(dr["ind_rescate"]);
                        if (dr["id_firma_digital_log"] != DBNull.Value)
                        {
                            formato.FirmaDigitalLog = new FirmaDigitalLog();
                            formato.FirmaDigitalLog.Id = Convert.ToInt32(dr["id_firma_digital_log"]);
                            if (dr["id_firma_digital_log"] != DBNull.Value)
                                formato.FirmaDigitalLog.FechaConsentimiento = Convert.ToDateTime(dr["fecha_firma"]);
                        }

                        formatos.Add(formato);
                    }
                    return formatos;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public FormatoSolicitud ObtenerFormatoSolicitud(string solicitud, int idFormatoSolicitud, string usuario)
        {
            try
            {
                FormatoSolicitud formato = null;

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_formato_solicitud", solicitud, idFormatoSolicitud, usuario))
                {
                    if (dr.Read())
                    {
                        formato = new FormatoSolicitud();

                        if (dr["id_formato_solicitud"] != DBNull.Value)
                            formato.Id = Convert.ToInt32(dr["id_formato_solicitud"]);
                        if (dr["num_solicitud"] != DBNull.Value)
                            formato.NumeroSolicitud = dr["num_solicitud"].ToString();
                        if (dr["fec_solicitud"] != DBNull.Value)
                            formato.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                        if (dr["num_correlativo"] != DBNull.Value)
                            formato.Correlativo = Convert.ToInt32(dr["num_correlativo"]);
                        if (dr["num_cotizacion"] != DBNull.Value)
                            formato.NumeroCotizacion = Convert.ToInt64(dr["num_cotizacion"]);
                        if (dr["cod_tipo_cotizacion"] != DBNull.Value)
                        {
                            formato.TipoCotizacion = new TipoCotizacion();
                            formato.TipoCotizacion.Id = dr["cod_tipo_cotizacion"].ToString();
                        }
                        if (dr["cod_plan"] != DBNull.Value)
                            formato.CodigoPlan = dr["cod_plan"].ToString();
                        if (dr["cod_tipo_plan_rpp"] != DBNull.Value)
                            formato.CodigoTipoPlanRPP = dr["cod_tipo_plan_rpp"].ToString();
                        if (dr["cod_moneda_cic"] != DBNull.Value)
                            formato.MonedaCIC = new Moneda
                            {
                                Id = dr["cod_moneda_cic"].ToString(),
                                Nombre = dr["gls_moneda_cic"].ToString()
                            };
                        if (dr["val_cic"] != DBNull.Value)
                            formato.CIC = Convert.ToDouble(dr["val_cic"]);
                        if (dr["val_cic"] != DBNull.Value)
                            formato.Temporalidad = new Temporalidad
                            {
                                Id = dr["cod_tipo_temporalidad"].ToString(),
                                Nombre = dr["gls_tipo_temporalidad"].ToString()
                            };
                        if (dr["cod_moneda"] != DBNull.Value)
                            formato.Moneda = new Moneda
                            {
                                Id = dr["cod_moneda"].ToString(),
                                Nombre = dr["gls_moneda"].ToString()
                            };
                        if (dr["val_pje_ajuste"] != DBNull.Value)
                            formato.PorcentajeAjuste = Convert.ToDouble(dr["val_pje_ajuste"]);
                        if (dr["num_meses_tramo1"] != DBNull.Value)
                            formato.MesesTramo1 = Convert.ToInt32(dr["num_meses_tramo1"]);
                        if (dr["val_pje_tramo2"] != DBNull.Value)
                            formato.PorcentajeTramo2 = Convert.ToDouble(dr["val_pje_tramo2"]);
                        if (dr["num_meses_garantizados"] != DBNull.Value)
                            formato.MesesGarantizados = Convert.ToInt32(dr["num_meses_garantizados"]);
                        if (dr["ind_cobertura_adicional_fallecimiento"] != DBNull.Value)
                            formato.IndCoberturaAdicionalFallecimiento = Convert.ToInt32(dr["ind_cobertura_adicional_fallecimiento"]);
                        if (dr["val_pje_devolucion_fallecimiento"] != DBNull.Value)
                            formato.PorcentajeDevolucionFallecimiento = Convert.ToDouble(dr["val_pje_devolucion_fallecimiento"]);
                        if (dr["ind_cobertura_adicional_devolucion"] != DBNull.Value)
                            formato.IndCoberturaAdicionalDevolucion = Convert.ToInt32(dr["ind_cobertura_adicional_devolucion"]);
                        if (dr["val_pje_devolucion_sobrevivencia"] != DBNull.Value)
                            formato.PorcentajeDevolucionSobrevivencia = Convert.ToDouble(dr["val_pje_devolucion_sobrevivencia"]);
                        if (dr["ind_sepelio"] != DBNull.Value)
                            formato.IndSepelio = Convert.ToChar(dr["ind_sepelio"]);
                        if (dr["val_renta"] != DBNull.Value)
                            formato.Renta = Convert.ToDouble(dr["val_renta"]);
                        if (dr["val_renta_tramo2"] != DBNull.Value)
                            formato.RentaTramo2 = Convert.ToDouble(dr["val_renta_tramo2"]);
                        if (dr["cod_agente"] != DBNull.Value)
                            formato.Agente = new Agente
                            {
                                Id = dr["cod_agente"].ToString()
                            };
                        if (dr["ind_consentimiento_necesario"] != DBNull.Value)
                            formato.IndConsentimientoNecesario = Convert.ToChar(dr["ind_consentimiento_necesario"]);
                        if (dr["ind_consentimiento_opcional"] != DBNull.Value)
                            formato.IndConsentimientoOpcional = Convert.ToChar(dr["ind_consentimiento_opcional"]);
                        if (dr["val_pje_conyuge"] != DBNull.Value)
                            formato.PorcentajeConyuge = Convert.ToDouble(dr["val_pje_conyuge"]);
                        if (dr["fec_vigencia"] != DBNull.Value)
                            formato.FechaVigencia = Convert.ToDateTime(dr["fec_vigencia"]);
                        if (dr["ind_rescate"] != DBNull.Value)
                            formato.Rescate = Convert.ToBoolean(dr["ind_rescate"]);
                        if (dr["ind_origen"] != DBNull.Value)
                            formato.Origen = Convert.ToInt32(dr["ind_origen"]);
                        if (dr["gls_declaracion_jurada"] != DBNull.Value)
                            formato.DeclaracionJurada = dr["gls_declaracion_jurada"].ToString();
                        if (dr["cod_tipo_via"] != DBNull.Value)
                        {
                            formato.Direccion = new Direccion
                            {
                                TipoVia = new Parametro
                                {
                                    Id = dr["cod_tipo_via"].ToString(),
                                    Nombre = dr["gls_tipo_via"].ToString()
                                },
                                Glosa = dr["gls_direccion"].ToString(),
                                EspacioUrbano = dr["gls_espacio_urbano"].ToString(),
                                Departamento = new Departamento
                                {
                                    Id = dr["cod_departamento"].ToString(),
                                    Nombre = dr["gls_departamento"].ToString()
                                },
                                Ciudad = new Ciudad
                                {
                                    Id = dr["cod_provincia"].ToString(),
                                    Nombre = dr["gls_provincia"].ToString()
                                },
                                Comuna = new Comuna
                                {
                                    Id = dr["cod_distrito"].ToString(),
                                    Nombre = dr["gls_distrito"].ToString()
                                }
                            };
                        }
                        if (dr["id_firma_digital_log"] != DBNull.Value)
                        {
                            formato.FirmaDigitalLog = new FirmaDigitalLog();
                            formato.FirmaDigitalLog.Id = Convert.ToInt32(dr["id_firma_digital_log"]);
                        }
                    }
                    return formato;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiario(string solicitud, int idFormatoSolicitud, string usuario)
        {
            try
            {
                List<FormatoSolicitudBeneficiario> formatos = new List<FormatoSolicitudBeneficiario>();

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_formato_solicitud_beneficiario", solicitud, idFormatoSolicitud, usuario))
                {
                    while (dr.Read())
                    {
                        FormatoSolicitudBeneficiario formato = new FormatoSolicitudBeneficiario();

                        if (dr["id_formato_solicitud_beneficiario"] != DBNull.Value)
                            formato.Id = Convert.ToInt32(dr["id_formato_solicitud_beneficiario"]);
                        if (dr["id_formato_solicitud"] != DBNull.Value)
                            formato.IdFormatoSolicitud = Convert.ToInt32(dr["id_formato_solicitud"]);
                        if (dr["num_correlativo"] != DBNull.Value)
                            formato.Correlativo = Convert.ToInt32(dr["num_correlativo"]);
                        if (dr["num_solicitud"] != DBNull.Value)
                            formato.NumeroSolicitud = dr["num_solicitud"].ToString();
                        if (dr["num_item"] != DBNull.Value)
                            formato.Item = Convert.ToInt32(dr["num_item"]);
                        if (dr["gls_nombres"] != DBNull.Value)
                            formato.Nombres = dr["gls_nombres"].ToString();
                        if (dr["gls_apellido_paterno"] != DBNull.Value)
                            formato.ApellidoPaterno = dr["gls_apellido_paterno"].ToString();
                        if (dr["gls_apellido_materno"] != DBNull.Value)
                            formato.ApellidoMaterno = dr["gls_apellido_materno"].ToString();
                        if (dr["fec_nacimiento"] != DBNull.Value)
                            formato.FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);
                        if (dr["cod_tipo_identificacion"] != DBNull.Value)
                        {
                            formato.Identificacion = new Identificacion
                            {
                                IdTipo = dr["cod_tipo_identificacion"].ToString(),
                                GlosaTipo = dr["gls_tipo_identificacion"].ToString(),
                                Numero = dr["gls_nro_identificacion"].ToString()
                            };
                        }
                        if (dr["cod_parentesco"] != DBNull.Value)
                        {
                            formato.Parentesco = new Parentesco
                            {
                                Id = dr["cod_parentesco"].ToString(),
                                Nombre = dr["gls_parentesco"].ToString()
                            };
                        }
                        if (dr["cod_parentesco"] != DBNull.Value)
                            formato.Sexo = dr["cod_sexo"].ToString();
                        if (dr["cod_estado_civil"] != DBNull.Value)
                        {
                            formato.EstadoCivil = new EstadoCivil
                            {
                                cod_estado_civil = dr["cod_estado_civil"].ToString(),
                                gls_estado_civil = dr["gls_estado_civil"].ToString()
                            };
                        }
                        if (dr["cod_nacionalidad"] != DBNull.Value)
                        {
                            formato.Nacionalidad = new Nacionalidad
                            {
                                cod_nacionalidad = dr["cod_nacionalidad"].ToString(),
                                gls_nacionalidad = dr["gls_nacionalidad"].ToString()
                            };
                        }
                        if (dr["cod_residencia"] != DBNull.Value)
                        {
                            formato.Residencia = new Departamento
                            {
                                Id = dr["cod_residencia"].ToString(),
                                Nombre = dr["gls_residencia"].ToString()
                            };
                        }
                        if (dr["cod_profesion"] != DBNull.Value)
                        {
                            formato.Profesion = new Profesion {
                                cod_profesion = dr["cod_profesion"].ToString(),
                                gls_profesion = dr["gls_profesion"].ToString()
                            };
                        }
                        if (dr["gls_centro_laboral"] != DBNull.Value)
                            formato.CentroLaboral = dr["gls_centro_laboral"].ToString();
                        if (dr["gls_cargo"] != DBNull.Value)
                            formato.Cargo = dr["gls_cargo"].ToString();
                        if (dr["gls_actividad_economica"] != DBNull.Value)
                            formato.ActividadEconomica = dr["gls_actividad_economica"].ToString();
                        if (dr["cod_moneda_ingreso"] != DBNull.Value)
                        {
                            formato.MonedaIngreso = new Moneda
                            {
                                Id = dr["cod_moneda_ingreso"].ToString(),
                                Nombre = dr["gls_moneda_ingreso"].ToString()
                            };
                        }
                        if (dr["val_ingreso_neto"] != DBNull.Value)
                            formato.IngresoNeto = Convert.ToDouble(dr["val_ingreso_neto"]);
                        if (dr["gls_telefono"] != DBNull.Value)
                            formato.Telefono = dr["gls_telefono"].ToString();
                        if (dr["gls_celular"] != DBNull.Value)
                            formato.Celular = dr["gls_celular"].ToString();
                        if (dr["gls_email"] != DBNull.Value)
                            formato.Email = dr["gls_email"].ToString();
                        if (dr["ind_pep"] != DBNull.Value)
                            formato.PEP = Convert.ToChar(dr["ind_pep"]);
                        if (dr["ind_sujeto_obligado"] != DBNull.Value)
                            formato.SujetoObligado = Convert.ToChar(dr["ind_sujeto_obligado"]);
                        if (dr["val_pje_renta"] != DBNull.Value)
                            formato.PorcentajeRenta = Convert.ToDouble(dr["val_pje_renta"]);
                        if (dr["cod_banco"] != DBNull.Value)
                            formato.CodigoBanco = dr["cod_banco"].ToString();
                        if (dr["gls_banco"] != DBNull.Value)
                            formato.NombreBanco = dr["gls_banco"].ToString();
                        if (dr["cod_tipo_cuenta"] != DBNull.Value)
                            formato.CodigoTipoCuenta = dr["cod_tipo_cuenta"].ToString();
                        if (dr["gls_tipo_cuenta"] != DBNull.Value)
                            formato.NombreTipoCuenta = dr["gls_tipo_cuenta"].ToString();
                        if (dr["gls_num_cuenta"] != DBNull.Value)
                            formato.NumeroCuenta = dr["gls_num_cuenta"].ToString();
                        if (dr["cod_confidencialidad_datos"] != DBNull.Value)
                            formato.CodigoConfidencialidadDatos = dr["cod_confidencialidad_datos"].ToString();
                        if (dr["cod_comunicacion"] != DBNull.Value)
                            formato.CodigoConfidencialidadDatos = dr["cod_comunicacion"].ToString();

                        if (dr["cod_tipo_periodo_beneficiario"] != DBNull.Value)
                            formato.codigoTipoPeriodoBeneficiario = Convert.ToInt32(dr["cod_tipo_periodo_beneficiario"].ToString());

                        formatos.Add(formato);
                    }
                    return formatos;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculada(string solicitud, int idFormatoSolicitud, string usuario)
        {
            try
            {
                List<FormatoSolicitudPersonaVinculada> formatos = new List<FormatoSolicitudPersonaVinculada>();

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_formato_solicitud_persona_vinculada", solicitud, idFormatoSolicitud, usuario))
                {
                    while (dr.Read())
                    {
                        FormatoSolicitudPersonaVinculada formato = new FormatoSolicitudPersonaVinculada();

                        if (dr["id_formato_solicitud_persona_vinculada"] != DBNull.Value)
                            formato.Id = Convert.ToInt32(dr["id_formato_solicitud_persona_vinculada"]);
                        if (dr["id_formato_solicitud"] != DBNull.Value)
                            formato.IdFormatoSolicitud = Convert.ToInt32(dr["id_formato_solicitud"]);
                        if (dr["num_solicitud"] != DBNull.Value)
                            formato.NumeroSolicitud = dr["num_solicitud"].ToString();
                        if (dr["gls_nombres"] != DBNull.Value)
                            formato.Nombres = dr["gls_nombres"].ToString();
                        if (dr["num_correlativo"] != DBNull.Value)
                            formato.Correlativo = Convert.ToInt32(dr["num_correlativo"]);
                        if (dr["gls_apellido_paterno"] != DBNull.Value)
                            formato.ApellidoPaterno = dr["gls_apellido_paterno"].ToString();
                        if (dr["gls_apellido_materno"] != DBNull.Value)
                            formato.ApellidoMaterno = dr["gls_apellido_materno"].ToString();
                        if (dr["cod_parentesco"] != DBNull.Value)
                        {
                            formato.Parentesco = new Parentesco {
                                Id = dr["cod_parentesco"].ToString(),
                                Nombre = dr["gls_parentesco"].ToString()
                            };
                        }
                        if (dr["cod_tipo_identificacion"] != DBNull.Value)
                        {
                            formato.Identificacion = new Identificacion
                            {
                                IdTipo = dr["cod_tipo_identificacion"].ToString(),
                                GlosaTipo = dr["gls_tipo_identificacion"].ToString(),
                                Numero = dr["gls_nro_identificacion"].ToString()
                            };
                        }

                        formatos.Add(formato);
                    }
                    return formatos;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public FormatoSolicitud ObtenerFormatoSolicitudActualizado(string solicitud, string usuario)
        {
            try
            {
                FormatoSolicitud formato = null;

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_formato_solicitud_actualizado", solicitud, usuario))
                {
                    if (dr.Read())
                    {
                        formato = new FormatoSolicitud();

                        if (dr["num_solicitud"] != DBNull.Value)
                            formato.NumeroSolicitud = dr["num_solicitud"].ToString();
                        if (dr["fec_solicitud"] != DBNull.Value)
                            formato.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                        if (dr["num_cotizacion"] != DBNull.Value)
                            formato.NumeroCotizacion = Convert.ToInt64(dr["num_cotizacion"]);
                        if (dr["cod_tipo_cotizacion"] != DBNull.Value)
                        {
                            formato.TipoCotizacion = new TipoCotizacion();
                            formato.TipoCotizacion.Id = dr["cod_tipo_cotizacion"].ToString();
                        }
                        if (dr["cod_plan"] != DBNull.Value)
                            formato.CodigoPlan = dr["cod_plan"].ToString();
                        if (dr["cod_tipo_plan_rpp"] != DBNull.Value)
                            formato.CodigoTipoPlanRPP = dr["cod_tipo_plan_rpp"].ToString();
                        if (dr["cod_moneda_cic"] != DBNull.Value)
                            formato.MonedaCIC = new Moneda
                            {
                                Id = dr["cod_moneda_cic"].ToString(),
                                Nombre = dr["gls_moneda_cic"].ToString()
                            };
                        if (dr["val_cic"] != DBNull.Value)
                            formato.CIC = Convert.ToDouble(dr["val_cic"]);
                        if (dr["val_cic"] != DBNull.Value)
                            formato.Temporalidad = new Temporalidad
                            {
                                Id = dr["cod_tipo_temporalidad"].ToString(),
                                Nombre = dr["gls_tipo_temporalidad"].ToString()
                            };
                        if (dr["cod_moneda"] != DBNull.Value)
                            formato.Moneda = new Moneda
                            {
                                Id = dr["cod_moneda"].ToString(),
                                Nombre = dr["gls_moneda"].ToString()
                            };
                        if (dr["val_pje_ajuste"] != DBNull.Value)
                            formato.PorcentajeAjuste = Convert.ToDouble(dr["val_pje_ajuste"]);
                        if (dr["num_meses_tramo1"] != DBNull.Value)
                            formato.MesesTramo1 = Convert.ToInt32(dr["num_meses_tramo1"]);
                        if (dr["val_pje_tramo2"] != DBNull.Value)
                            formato.PorcentajeTramo2 = Convert.ToDouble(dr["val_pje_tramo2"]);
                        if (dr["num_meses_garantizados"] != DBNull.Value)
                            formato.MesesGarantizados = Convert.ToInt32(dr["num_meses_garantizados"]);
                        if (dr["ind_cobertura_adicional_fallecimiento"] != DBNull.Value)
                            formato.IndCoberturaAdicionalFallecimiento = Convert.ToInt32(dr["ind_cobertura_adicional_fallecimiento"]);
                        if (dr["val_pje_devolucion_fallecimiento"] != DBNull.Value)
                            formato.PorcentajeDevolucionFallecimiento = Convert.ToDouble(dr["val_pje_devolucion_fallecimiento"]);
                        if (dr["ind_cobertura_adicional_devolucion"] != DBNull.Value)
                            formato.IndCoberturaAdicionalDevolucion = Convert.ToInt32(dr["ind_cobertura_adicional_devolucion"]);
                        if (dr["val_pje_devolucion_sobrevivencia"] != DBNull.Value)
                            formato.PorcentajeDevolucionSobrevivencia = Convert.ToDouble(dr["val_pje_devolucion_sobrevivencia"]);
                        if (dr["ind_sepelio"] != DBNull.Value)
                            formato.IndSepelio = Convert.ToChar(dr["ind_sepelio"]);
                        if (dr["val_renta"] != DBNull.Value)
                            formato.Renta = Convert.ToDouble(dr["val_renta"]);
                        if (dr["val_renta_tramo2"] != DBNull.Value)
                            formato.RentaTramo2 = Convert.ToDouble(dr["val_renta_tramo2"]);
                        if (dr["cod_agente"] != DBNull.Value)
                            formato.Agente = new Agente
                            {
                                Id = dr["cod_agente"].ToString()
                            };
                        if (dr["ind_consentimiento_necesario"] != DBNull.Value)
                            formato.IndConsentimientoNecesario = Convert.ToChar(dr["ind_consentimiento_necesario"]);
                        if (dr["ind_consentimiento_opcional"] != DBNull.Value)
                            formato.IndConsentimientoOpcional = Convert.ToChar(dr["ind_consentimiento_opcional"]);
                        if (dr["val_pje_conyuge"] != DBNull.Value)
                            formato.PorcentajeConyuge = Convert.ToDouble(dr["val_pje_conyuge"]);
                        if (dr["fec_vigencia"] != DBNull.Value)
                            formato.FechaVigencia = Convert.ToDateTime(dr["fec_vigencia"]);
                        if (dr["ind_rescate"] != DBNull.Value)
                            formato.Rescate = Convert.ToBoolean(dr["ind_rescate"]);
                        if (dr["ind_origen"] != DBNull.Value)
                            formato.Origen = Convert.ToInt32(dr["ind_origen"]);
                        if (dr["gls_declaracion_jurada"] != DBNull.Value)
                            formato.DeclaracionJurada = dr["gls_declaracion_jurada"].ToString();
                        if (dr["cod_tipo_via"] != DBNull.Value)
                        {
                            formato.Direccion = new Direccion
                            {
                                TipoVia = new Parametro
                                {
                                    Id = dr["cod_tipo_via"].ToString(),
                                    Nombre = dr["gls_tipo_via"].ToString()
                                },
                                Glosa = dr["gls_direccion"].ToString(),
                                EspacioUrbano = dr["gls_espacio_urbano"].ToString(),
                                Departamento = new Departamento
                                {
                                    Id = dr["cod_departamento"].ToString(),
                                    Nombre = dr["gls_departamento"].ToString()
                                },
                                Ciudad = new Ciudad
                                {
                                    Id = dr["cod_provincia"].ToString(),
                                    Nombre = dr["gls_provincia"].ToString()
                                },
                                Comuna = new Comuna
                                {
                                    Id = dr["cod_distrito"].ToString(),
                                    Nombre = dr["gls_distrito"].ToString()
                                }
                            };
                        }
                        if (dr["id_firma_digital_log"] != DBNull.Value)
                        {
                            formato.FirmaDigitalLog = new FirmaDigitalLog();
                            formato.FirmaDigitalLog.Id = Convert.ToInt32(dr["id_firma_digital_log"]);
                        }
                    }
                    return formato;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiarioActualizado(string solicitud, string usuario)
        {
            try
            {
                List<FormatoSolicitudBeneficiario> formatos = new List<FormatoSolicitudBeneficiario>();

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_formato_solicitud_beneficiario_actualizado", solicitud, usuario))
                {
                    while (dr.Read())
                    {
                        FormatoSolicitudBeneficiario formato = new FormatoSolicitudBeneficiario();

                        if (dr["num_solicitud"] != DBNull.Value)
                            formato.NumeroSolicitud = dr["num_solicitud"].ToString();
                        if (dr["num_item"] != DBNull.Value)
                            formato.Item = Convert.ToInt32(dr["num_item"]);
                        if (dr["gls_nombres"] != DBNull.Value)
                            formato.Nombres = dr["gls_nombres"].ToString();
                        if (dr["gls_apellido_paterno"] != DBNull.Value)
                            formato.ApellidoPaterno = dr["gls_apellido_paterno"].ToString();
                        if (dr["gls_apellido_materno"] != DBNull.Value)
                            formato.ApellidoMaterno = dr["gls_apellido_materno"].ToString();
                        if (dr["fec_nacimiento"] != DBNull.Value)
                            formato.FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);
                        if (dr["cod_tipo_identificacion"] != DBNull.Value)
                        {
                            formato.Identificacion = new Identificacion
                            {
                                IdTipo = dr["cod_tipo_identificacion"].ToString(),
                                GlosaTipo = dr["gls_tipo_identificacion"].ToString(),
                                Numero = dr["gls_nro_identificacion"].ToString()
                            };
                        }
                        if (dr["cod_parentesco"] != DBNull.Value)
                        {
                            formato.Parentesco = new Parentesco
                            {
                                Id = dr["cod_parentesco"].ToString(),
                                Nombre = dr["gls_parentesco"].ToString()
                            };
                        }
                        if (dr["cod_parentesco"] != DBNull.Value)
                            formato.Sexo = dr["cod_sexo"].ToString();
                        if (dr["cod_estado_civil"] != DBNull.Value)
                        {
                            formato.EstadoCivil = new EstadoCivil
                            {
                                cod_estado_civil = dr["cod_estado_civil"].ToString(),
                                gls_estado_civil = dr["gls_estado_civil"].ToString()
                            };
                        }
                        if (dr["cod_nacionalidad"] != DBNull.Value)
                        {
                            formato.Nacionalidad = new Nacionalidad
                            {
                                cod_nacionalidad = dr["cod_nacionalidad"].ToString(),
                                gls_nacionalidad = dr["gls_nacionalidad"].ToString()
                            };
                        }
                        if (dr["cod_residencia"] != DBNull.Value)
                        {
                            formato.Residencia = new Departamento
                            {
                                Id = dr["cod_residencia"].ToString(),
                                Nombre = dr["gls_residencia"].ToString()
                            };
                        }
                        if (dr["cod_profesion"] != DBNull.Value)
                        {
                            formato.Profesion = new Profesion
                            {
                                cod_profesion = dr["cod_profesion"].ToString(),
                                gls_profesion = dr["gls_profesion"].ToString()
                            };
                        }
                        if (dr["gls_centro_laboral"] != DBNull.Value)
                            formato.CentroLaboral = dr["gls_centro_laboral"].ToString();
                        if (dr["gls_cargo"] != DBNull.Value)
                            formato.Cargo = dr["gls_cargo"].ToString();
                        if (dr["gls_actividad_economica"] != DBNull.Value)
                            formato.ActividadEconomica = dr["gls_actividad_economica"].ToString();
                        if (dr["cod_moneda_ingreso"] != DBNull.Value)
                        {
                            formato.MonedaIngreso = new Moneda
                            {
                                Id = dr["cod_moneda_ingreso"].ToString(),
                                Nombre = dr["gls_moneda_ingreso"].ToString()
                            };
                        }
                        if (dr["val_ingreso_neto"] != DBNull.Value)
                            formato.IngresoNeto = Convert.ToDouble(dr["val_ingreso_neto"]);
                        if (dr["gls_telefono"] != DBNull.Value)
                            formato.Telefono = dr["gls_telefono"].ToString();
                        if (dr["gls_celular"] != DBNull.Value)
                            formato.Celular = dr["gls_celular"].ToString();
                        if (dr["gls_email"] != DBNull.Value)
                            formato.Email = dr["gls_email"].ToString();
                        if (dr["ind_pep"] != DBNull.Value)
                            formato.PEP = Convert.ToChar(dr["ind_pep"]);
                        if (dr["ind_sujeto_obligado"] != DBNull.Value)
                            formato.SujetoObligado = Convert.ToChar(dr["ind_sujeto_obligado"]);
                        if (dr["val_pje_renta"] != DBNull.Value)
                            formato.PorcentajeRenta = Convert.ToDouble(dr["val_pje_renta"]);
                        if (dr["cod_banco"] != DBNull.Value)
                            formato.CodigoBanco = dr["cod_banco"].ToString();
                        if (dr["gls_banco"] != DBNull.Value)
                            formato.NombreBanco = dr["gls_banco"].ToString();
                        if (dr["cod_tipo_cuenta"] != DBNull.Value)
                            formato.CodigoTipoCuenta = dr["cod_tipo_cuenta"].ToString();
                        if (dr["gls_tipo_cuenta"] != DBNull.Value)
                            formato.NombreTipoCuenta = dr["gls_tipo_cuenta"].ToString();
                        if (dr["gls_num_cuenta"] != DBNull.Value)
                            formato.NumeroCuenta = dr["gls_num_cuenta"].ToString();
                        if (dr["cod_confidencialidad_datos"] != DBNull.Value)
                            formato.CodigoConfidencialidadDatos = dr["cod_confidencialidad_datos"].ToString();
                        if (dr["cod_comunicacion"] != DBNull.Value)
                            formato.CodigoConfidencialidadDatos = dr["cod_comunicacion"].ToString();

                        formatos.Add(formato);
                    }
                    return formatos;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculadaActualizado(string solicitud, string usuario)
        {
            try
            {
                List<FormatoSolicitudPersonaVinculada> formatos = new List<FormatoSolicitudPersonaVinculada>();

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_formato_solicitud_persona_vinculada_actualizado", solicitud, usuario))
                {
                    while (dr.Read())
                    {
                        FormatoSolicitudPersonaVinculada formato = new FormatoSolicitudPersonaVinculada();

                        if (dr["num_solicitud"] != DBNull.Value)
                            formato.NumeroSolicitud = dr["num_solicitud"].ToString();
                        if (dr["gls_nombres"] != DBNull.Value)
                            formato.Nombres = dr["gls_nombres"].ToString();
                        //if (dr["num_correlativo"] != DBNull.Value)
                        //    formato.Correlativo = Convert.ToInt32(dr["num_correlativo"]);
                        if (dr["gls_apellido_paterno"] != DBNull.Value)
                            formato.ApellidoPaterno = dr["gls_apellido_paterno"].ToString();
                        if (dr["gls_apellido_materno"] != DBNull.Value)
                            formato.ApellidoMaterno = dr["gls_apellido_materno"].ToString();
                        if (dr["cod_parentesco"] != DBNull.Value)
                        {
                            formato.Parentesco = new Parentesco
                            {
                                Id = dr["cod_parentesco"].ToString(),
                                Nombre = dr["gls_parentesco"].ToString()
                            };
                        }
                        if (dr["cod_tipo_identificacion"] != DBNull.Value)
                        {
                            formato.Identificacion = new Identificacion
                            {
                                IdTipo = dr["cod_tipo_identificacion"].ToString(),
                                GlosaTipo = dr["gls_tipo_identificacion"].ToString(),
                                Numero = dr["gls_nro_identificacion"].ToString()
                            };
                        }

                        formatos.Add(formato);
                    }
                    return formatos;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RegistrarPjeBeneficiarios(string idSolicitud, string correlativos, string xml_pje, string xml_costo, string usuario)
        {
            throw new NotImplementedException();
        }

        public void RegistrarCotiza(string xml_cotiza, string usuario)
        {
            throw new NotImplementedException();
        }

        public void Registrar(SolicitudRP entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(SolicitudRP entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(SolicitudRP entity)
        {
            throw new NotImplementedException();
        }

        public SolicitudRP ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public SolicitudRP ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<SolicitudRP> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
