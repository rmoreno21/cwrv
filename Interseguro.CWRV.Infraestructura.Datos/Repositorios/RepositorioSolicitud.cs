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
using System.Runtime.InteropServices;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioSolicitud : IRepositorioSolicitud
    {
        public List<Solicitud> Listar(string cuspp)
        {
            List<Solicitud> listaSolicitudes = new List<Solicitud>();
            Solicitud solicitud;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solicitudes", cuspp))
            {
                while (dr.Read())
                {
                    solicitud = new Solicitud();
                    solicitud.Id = dr["num_solicitud"].ToString();
                    if (dr["fec_solicitud"] != DBNull.Value)
                        solicitud.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    solicitud.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                    if (dr["fec_recepcion"] != DBNull.Value)
                        solicitud.FechaRecepcion = Convert.ToDateTime(dr["fec_recepcion"]);
                    if (dr["fec_presentacion"] != DBNull.Value)
                        solicitud.FechaPlazoAFP = Convert.ToDateTime(dr["fec_presentacion"]);
                    if (dr["fec_cierre_comercial"] != DBNull.Value)
                        solicitud.FechaCierre = Convert.ToDateTime(dr["fec_cierre_comercial"]);
                    if (dr["fec_cotizacion"] != DBNull.Value)
                        solicitud.FechaCotizacion = Convert.ToDateTime(dr["fec_cotizacion"]);
                    if (dr["gls_compania"] != DBNull.Value)
                        solicitud.Companhia = dr["gls_compania"].ToString();
                    //<SRI.INI-20322>
                    if (dr["num_lote_cotizacion"] != DBNull.Value)
                        solicitud.NumLoteCotizacion = Convert.ToInt32(dr["num_lote_cotizacion"]);
                    if (dr["ind_enviado_sbs"] != DBNull.Value)
                        solicitud.IndEnviadoSbs = dr["ind_enviado_sbs"].ToString();
                    solicitud.Agente = new Agente { Id = dr["num_agente"].ToString() };
                    //<SRI.FIN-20322>

                    //<INIGTI_2145>
                    //val_mto_cta_individual
                    if (dr["val_mto_cta_individual"] != DBNull.Value)
                        solicitud.SaldoCIC = Convert.ToDouble(dr["val_mto_cta_individual"]);
                    //<FINGTI_2145>

                    if (dr["firma_digital"] != DBNull.Value)
                        solicitud.firmaDigitalToken = dr["firma_digital"].ToString();

                    if (dr["num_poliza"] != DBNull.Value)
                        solicitud.NumeroPoliza = Convert.ToInt32(dr["num_poliza"]);

                    listaSolicitudes.Add(solicitud);
                }
            }

            return listaSolicitudes;
        }

        public List<Solicitud> Listar(int lote)
        {
            List<Solicitud> listaSolicitudes = new List<Solicitud>();
            Solicitud solicitud;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solictudes_x_lote", lote))
            {
                while (dr.Read())
                {
                    solicitud = new Solicitud();
                    solicitud.Id = dr["num_solicitud"].ToString();
                    //solicitud.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                    solicitud.Categoria = new Categoria { Id = dr["cod_categoria"].ToString(), Nombre = dr["gls_categoria"].ToString() };
                    solicitud.AFP = new AFP { Id = dr["cod_afp"].ToString(), Nombre = dr["gls_afp"].ToString() };
                    if (dr["fec_presentacion"] != DBNull.Value)
                        solicitud.FechaPlazoAFP = Convert.ToDateTime(dr["fec_presentacion"]);
                    if (dr["fec_devengue"] != DBNull.Value)
                        solicitud.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);
                    solicitud.Agente = new Agente { Id = dr["num_vendedor"].ToString(), Nombre = dr["nom_vendedor"].ToString() };
                    solicitud.Cartera = dr["gls_cartera"].ToString(); //cod_cartera
                    solicitud.OrigenTasa = dr["gls_origen_tasa"].ToString(); //cod_origen_tasa
                    solicitud.PorcentajeCesionComision = dr["gls_corta_pje_cesion_comision"].ToString(); //cod_pje_cesion_comision
                    solicitud.Afiliado = new Afiliado
                    {
                        CUSPP = dr["num_cuispp"].ToString(),
                        Nombre = dr["gls_nom_persona"].ToString(),
                        FechaNacimiento = Convert.ToDateTime(dr["fec_nacimiento"]),
                        SaldoCIC = Convert.ToDouble(dr["val_total_cic"])
                    };
                    solicitud.TipoCotizacion = new TipoCotizacion { Nombre = dr["gls_tipo_pension"].ToString() };
                    //gls_direccion	
                    //val_bono_actualizado	
                    //gls_ciudad_comuna	
                    solicitud.Habilitado = dr["ind_habilitado"].ToString() == "S" ? true : false;
                    solicitud.Observacion = dr["gls_observacion"].ToString();
                    //cod_tipo_beneficio	
                    solicitud.Enviada = dr["ind_solicitud_enviada"].ToString() == "S" ? true : false;
                    solicitud.TieneCotizacion = dr["ind_tiene_cotizacion"].ToString() == "S" ? true : false;
                    //<GTI.29372>
                    solicitud.IndCoberturaIS = dr["ind_cobertura_is"].ToString() == "S" ? true : false;
                    //<GTI.29372>
                    listaSolicitudes.Add(solicitud);
                }
            }

            return listaSolicitudes;
        }

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

        public void RegistrarCotiza(string xml_cotiza, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.SP_RENVI_INS_COTIZA");

                db.AddInParameter(dbc, "@wl_empdata", DbType.String, xml_cotiza);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Registrar(ref Solicitud entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_solicitud");

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

        public void Actualizar(ref Solicitud entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_solicitud");

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

        //<INIGTI_4081>
        [DllImport(@"C:\3gl\RviFncGene.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double lib_s_round(double nValInicial, int nNumDecimales, int nIndRedondear);
        //<FINGTI_4081>

        public Solicitud ObtenerDatos(string idSolicitud, DateTime fecCotizacion)
        {
            Solicitud solicitud = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_solicitud", idSolicitud, fecCotizacion))
            {
                // Datos de Solicitud
                if (dr.Read())
                {
                    solicitud = new Solicitud();
                    solicitud.Id = dr["num_solicitud"].ToString();
                    if (dr["fec_solicitud"] != DBNull.Value)
                        solicitud.FechaSolicitud = Convert.ToDateTime(dr["fec_solicitud"]);
                    if (dr["fec_cotizacion"] != DBNull.Value)
                        solicitud.FechaCotizacion = Convert.ToDateTime(dr["fec_cotizacion"]);
                    solicitud.Afiliado = new Afiliado
                    {
                        CUSPP = dr["num_cuispp"].ToString(),
                        //<SRI.INI-20322_E2>
                        CorreoElectronico = (dr["gls_mail"] != DBNull.Value && dr["gls_mail"].ToString() != string.Empty) ? dr["gls_mail"].ToString() : null,
                        AFP = new AFP
                        {
                            Id = dr["cod_afp"].ToString()
                        }
                        //<SRI.FIN-20322_E2>
                    };
                    solicitud.AFP = new AFP { Id = dr["cod_afp"].ToString() };
                    solicitud.Afiliado.AFP = new AFP { Id = dr["cod_afp"].ToString() };
                    solicitud.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                    solicitud.TipoPension = new TipoPension { Id = dr["cod_tipo_pension"].ToString() };
                    if (dr["fec_devengue"] != DBNull.Value)
                        solicitud.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);
                    if (dr["fec_ult_actualizacion"] != DBNull.Value)
                    {
                        //<GTIINI-754>
                        //solicitud.FechaSolicitud = Convert.ToDateTime(dr["fec_ult_actualizacion"]);
                        solicitud.FechaUltimaActualizacion = Convert.ToDateTime(dr["fec_ult_actualizacion"]);
                        //<GTIFIN-754>
                    }
                    if (dr["fec_recepcion"] != DBNull.Value)
                        solicitud.FechaRecepcion = Convert.ToDateTime(dr["fec_recepcion"]);
                    if (dr["fec_presentacion"] != DBNull.Value)
                        solicitud.FechaPlazoAFP = Convert.ToDateTime(dr["fec_presentacion"]);
                    solicitud.SaldoCIC = Convert.ToDouble(dr["val_mto_cta_individual"]);
                    solicitud.TipoCambio = Convert.ToDouble(dr["val_tasa_cambio"]);
                    solicitud.FactorTasa = dr["cod_factor_tasa"].ToString();
                    if (dr["fec_sol_pension"] != DBNull.Value)
                        solicitud.FechaSolicitudPension = Convert.ToDateTime(dr["fec_sol_pension"]);
                    //<SRIINI06326>
                    if (dr["pje_aumento_comision"] != DBNull.Value)
                        solicitud.PorcentajeAumentoComision = Convert.ToDouble(dr["pje_aumento_comision"]);
                    else
                        solicitud.PorcentajeAumentoComision = 0;
                    if (dr["pje_descuento_comision"] != DBNull.Value)
                        solicitud.PorcentajeDescuentoComision = Convert.ToDouble(dr["pje_descuento_comision"]);
                    else
                        solicitud.PorcentajeDescuentoComision = 0;
                    solicitud.Categoria = new Categoria { Id = dr["cod_categoria"].ToString() };
                    //<SRIFIN06326>
                    //<SRIINI25781>
                    if (dr["val_mto_acom_agente"] != DBNull.Value)
                        solicitud.MontoAumentoComision = Convert.ToDouble(dr["val_mto_acom_agente"]);
                    else
                        solicitud.MontoAumentoComision = 0;
                    //<SRIFIN25781>
                    //<SRI.INI-20322_E2>
                    //<INIGTI_4022>
                    //solicitud.Agente = new Agente { Id = dr["num_agente"].ToString(), IdCartera = dr["cod_cartera"].ToString() };
                    solicitud.Agente = new Agente { Id = dr["num_agente"].ToString(), IdCartera = dr["cod_cartera"].ToString(), IdNivel = Convert.ToInt32(dr["cod_nivel"]) };
                    //<FINGTI_4022>

                    if (dr["cod_tipo_movimiento"] != DBNull.Value && dr["cod_tipo_movimiento"].ToString() != string.Empty)
                        solicitud.TipoMovimiento = new TipoMovimiento
                        {
                            Id = Convert.ToInt16(dr["cod_tipo_movimiento"]),
                            Nombre = (dr["gls_tipo_movimiento"] != DBNull.Value && dr["gls_tipo_movimiento"].ToString() != string.Empty) ? dr["gls_tipo_movimiento"].ToString() : null
                        };
                    //<SRI.FIN-20322_E2>

                    //<INIGTI_4081>
                    if (dr["cod_compania"] != DBNull.Value)
                        solicitud.Compania = new Compania { Id = dr["cod_compania"].ToString() };
                    //<FINGTI_4081>

                    //<INIGTI_6556>
                    solicitud.ValidarACOM = dr["ind_validar_acom"].ToString() == "S" ? true : false;
                    solicitud.ValidarDTRA = dr["ind_validar_dtra"].ToString() == "S" ? true : false;
                    //<FINGTI_6556>
                }

                dr.NextResult();

                // Cotizaciones
                solicitud.Cotizaciones = new List<Cotizacion>();
                while (dr.Read())
                {
                    Cotizacion cotizacion = new Cotizacion();
                    cotizacion.Correlativo = Convert.ToInt64(dr["num_correlativo"]);
                    cotizacion.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString(), Simbolo = dr["gls_moneda_simbolo"].ToString() };
                    if (dr["val_tipo_cambio"] != DBNull.Value)
                        cotizacion.ValorMoneda = Convert.ToDouble(dr["val_tipo_cambio"].ToString());
                    cotizacion.Producto = new Producto { Id = dr["cod_tipo_producto"].ToString(), Nombre = dr["gls_tipo_producto"].ToString() };
                    cotizacion.Modalidad = new Modalidad { Id = dr["ind_modalidad"].ToString(), Nombre = dr["cod_modalidad"].ToString() };
                    cotizacion.PeriodoDiferido = Convert.ToInt32(dr["val_per_temporal"]);
                    cotizacion.PorcentajeEntreRentas = Convert.ToInt32(dr["val_pje_rent_temp"]);
                    cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["val_per_garantizado"]);
                    cotizacion.Gratificacion = (dr["ind_gratificacion"].ToString() == "S") ? true : false;
                    cotizacion.Capital = new Capital { Id = dr["cod_particion_capital"].ToString(), Nombre = dr["gls_particion_capital"].ToString() };
                    //<SRIINI06326>
                    cotizacion.AjusteTRA = Convert.ToDouble(dr["val_Tasa_ajuste_tra"]);
                    //<SRIFIN06326>
                    if (dr["val_mto_cia"] != DBNull.Value)
                        cotizacion.MontoCia = Convert.ToDouble(dr["val_mto_cia"]);
                    if (dr["val_pen_cia"] != DBNull.Value)
                        cotizacion.PensionCia = Convert.ToDouble(dr["val_pen_cia"]);
                    if (dr["val_pen_cia_mo"] != DBNull.Value)
                        cotizacion.PensionCiaMO = Convert.ToDouble(dr["val_pen_cia_mo"]);
                    if (dr["val_tasa_int_temp"] != DBNull.Value)
                        cotizacion.TasaAFP = Convert.ToDouble(dr["val_tasa_int_temp"]);
                    if (dr["val_mto_afp"] != DBNull.Value)
                        cotizacion.MontoAFP = Convert.ToDouble(dr["val_mto_afp"]);
                    if (dr["val_pen_afp"] != DBNull.Value)
                        cotizacion.PensionAFP = Convert.ToDouble(dr["val_pen_afp"]);
                    //<SRI.INI-20322>
                    if (dr["val_tasa_venta_ash"] != DBNull.Value)
                        cotizacion.TasaVenta = Convert.ToDouble(dr["val_tasa_venta_ash"]);
                    if (dr["val_tasa_int_vit"] != DBNull.Value)
                        cotizacion.TasaVentaSbs = Convert.ToDouble(dr["val_tasa_int_vit"]);
                    if (dr["val_tra"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionista = Convert.ToDouble(dr["val_tra"]);
                    //<SRI.FIN-20322>
                    //<SRIINI-25781>
                    if (dr["val_tasa_venta_max"] != DBNull.Value)
                        cotizacion.TasaVentaMaxima = Convert.ToDouble(dr["val_tasa_venta_max"]);
                    if (dr["val_tra_min"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionistaMinimo = Convert.ToDouble(dr["val_tra_min"]);
                    if (dr["ind_cotiza"] != DBNull.Value)
                        cotizacion.IndCotiza = dr["ind_cotiza"].ToString();
                    if (dr["ind_error_cotiza"] != DBNull.Value)
                        cotizacion.IndErrorCotiza = Convert.ToInt32(dr["ind_error_cotiza"]);
                    //<SRIFIN-25781>
                    //<GTIINI-754>
                    if (dr["val_1era_prima_is"] != DBNull.Value)
                        cotizacion.PrimeraPensionRVD = Convert.ToDouble(dr["val_1era_prima_is"]);
                    //<GTIFIN-754>

                    //<INIGTI_4081>
                    if (dr["pbs"] != DBNull.Value)
                        cotizacion.pbs = Convert.ToDouble(dr["pbs"]);

                    if (dr["val_tasa_int_vit_objetivo"] != DBNull.Value)
                        cotizacion.TasaVentaSbsObjetivo = Convert.ToDouble(dr["val_tasa_int_vit_objetivo"]);

                    if (dr["val_tra_objetivo"] != DBNull.Value)
                        cotizacion.TasaRetornoAccionistaObjetivo = Convert.ToDouble(dr["val_tra_objetivo"]);

                    if (dr["val_pen_cia_objetivo"] != DBNull.Value)
                        cotizacion.PensionCiaObjetivo = Convert.ToDouble(dr["val_pen_cia_objetivo"]);

                    if (dr["val_tasa_int_vit_ori"] != DBNull.Value)
                        cotizacion.TasaVentaSbsOrigen = Convert.ToDouble(dr["val_tasa_int_vit_ori"]);

                    if (dr["val_pen_cia_ori"] != DBNull.Value)
                        cotizacion.PensionCiaOrigen = Convert.ToDouble(dr["val_pen_cia_ori"]);

                    if (dr["val_pen_cia_mo_ori"] != DBNull.Value)
                        cotizacion.PensionCiaMOOrigen = Convert.ToDouble(dr["val_pen_cia_mo_ori"]);

                    //if (cotizacion.pbs == 0)
                    //{
                    //    cotizacion.pbs = Convert.ToInt32(lib_s_round((cotizacion.TasaVentaSbs - cotizacion.TasaVentaSbsOrigen) * 100, 0, 0));
                    //}
                    //<FINGTI_4081>

                    //<INIGTI_4081_2>
                    if (dr["num_movimiento"] != DBNull.Value)
                        cotizacion.NumMovimiento = Convert.ToInt64(dr["num_movimiento"]);
                    //<FINGTI_4081_2>

                    //<INIGTI_6623>
                    if (dr["val_pen_cia_mo_objetivo"] != DBNull.Value)
                        cotizacion.PensionCiaMOObjetivo = Convert.ToDouble(dr["val_pen_cia_mo_objetivo"]);

                    if (dr["val_pen_afp_ori"] != DBNull.Value)
                        cotizacion.PensionAFPOrigen = Convert.ToDouble(dr["val_pen_afp_ori"]);

                    if (dr["val_pen_afp_objetivo"] != DBNull.Value)
                        cotizacion.PensionAFPObjetivo = Convert.ToDouble(dr["val_pen_afp_objetivo"]);
                    //<FINGTI_6623>

                    //<GTI.INI-29372>
                    cotizacion.IndEnvioObligatorio = dr["ind_envio_obligatorio"].ToString() == "S" ? true : false;
                    //<GTI-FIN-29372>
                    solicitud.Cotizaciones.Add(cotizacion);
                }
                dr.NextResult();

                // Beneficiarios
                solicitud.Beneficiarios = new List<GrupoFamiliar>();
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

                    solicitud.Beneficiarios.Add(beneficiario);
                }
            }

            return solicitud;

        }

        public List<ParametroCotizador> ObtenerParametrosCotizacion(string idSolicitud, DateTime fecCotizacion, Int64? numCorrelativo, int anhosAdicionales, double? nuevoCIC)
        {
            List<ParametroCotizador> parametros = new List<ParametroCotizador>();
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                //SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_carpr_solicitud");
                DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_carpr_solicitud");
                dbc.CommandTimeout = 150;
                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, idSolicitud);
                db.AddInParameter(dbc, "@wl_fec_cotizacion", DbType.DateTime, fecCotizacion);
                db.AddInParameter(dbc, "@wl_num_correlativo", DbType.Int64, numCorrelativo);
                db.AddInParameter(dbc, "@wl_mto_cta_individual", DbType.Double, nuevoCIC);

                //db.ExecuteNonQuery(dbc2);

                //DbCommand comando = db.GetStoredProcCommand()

                //<INIGTI_1092>
                bool nullAjuste = false;
                string wl_sexo = "";
                //<FINGTI_1092>
                List<Parinv> lParinv = new List<Parinv>();
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Soles.StringValue() });
                //<SRIINI17003>
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Dolares.StringValue() });
                //<SRIFIN17003>
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.SolesAjustados.StringValue() });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.DolaresAjustados.StringValue() });

                using (IDataReader dr = db.ExecuteReader(dbc))
                //using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_carpr_solicitud", idSolicitud, fecCotizacion, numCorrelativo, nuevoCIC))
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

                        //<INIGTI_1092>
                        if (dr["val_ajuste_invalidez"] == DBNull.Value)
                        {
                            nullAjuste = true;
                        }
                        else
                        {
                            parametro.val_ajuste_invalidez = Convert.ToDouble(dr["val_ajuste_invalidez"]);
                        }
                        //<FINGTI_1092>

                        //<INIGTI_4081>
                        if (dr["pbs"] != DBNull.Value)
                            parametro.x_pbs = Convert.ToDouble(dr["pbs"]);
                        if (dr["val_tasa_int_vit"] != DBNull.Value)
                            parametro.x_val_tasa_int_vit = Convert.ToDouble(dr["val_tasa_int_vit"]);
                        if (dr["val_tasa_ajuste_tra"] != DBNull.Value)
                            parametro.x_val_tasa_ajuste_tra = Convert.ToDouble(dr["val_tasa_ajuste_tra"]);
                        //<FINGTI_4081>
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


                    //<GTI.INI-15819>
                    // arr_fac_dcto

                    double[,] arr_fac_dcto = new double[15, 1321];

                    i = 0;
                    int j = 0;

                    while (dr.Read())
                    {
                        i = Convert.ToInt32(dr["cod_moneda"].ToString());
                        arr_fac_dcto[i, j] = Convert.ToDouble(dr["val_factor_descuento"]);
                        j++;
                        if (j == 1321)
                        {
                            j = 0;
                        }
                    }

                    foreach (ParametroCotizador p in parametros)
                    {
                        p.cot_fac_dto = new double[1321];
                        i = Convert.ToInt32(p.cod_moneda.ToString());

                        for (j = 0; j <= 1320; j++)
                        {
                            p.cot_fac_dto[j] = arr_fac_dcto[i, j];
                        }
                    }


                    dr.NextResult();
                    //<GTI.FIN-15819>

                    //<INIGTI_753>//Se comentara la carga, porque tambien se comenta en el SP
                    // ppu_arr_vllx
                    ////i = 0;
                    ////while (dr.Read())
                    ////{
                    ////    parametros
                    ////        .FindAll(p => (
                    ////                       (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2
                    ////                      )
                    ////        )
                    ////        .ForEach(p =>
                    ////        {
                    ////            p.ppu_arr_vllx[i, 0] = Convert.ToDouble(dr["num_tramo"]);
                    ////            p.ppu_arr_vllx[i, 1] = Convert.ToDouble(dr["num_columna"]);
                    ////            p.ppu_arr_vllx[i, 2] = Convert.ToDouble(dr["val_tope"]);
                    ////            p.ppu_arr_vllx[i, 3] = Convert.ToDouble(dr["pje_ajuste_RM"]);
                    ////        });
                    ////    parametros
                    ////        .FindAll(p => !(
                    ////                        (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2
                    ////                       )
                    ////        )
                    ////        .ForEach(p =>
                    ////        {
                    ////            p.ppu_arr_vllx[i, 0] = Convert.ToDouble(dr["num_tramo"]);
                    ////            p.ppu_arr_vllx[i, 1] = Convert.ToDouble(dr["num_columna"]);
                    ////            p.ppu_arr_vllx[i, 2] = Convert.ToDouble(dr["val_tope"]);
                    ////            p.ppu_arr_vllx[i, 3] = Convert.ToDouble(dr["pje_ajuste"]);
                    ////        });
                    ////    i++;
                    ////}

                    ////dr.NextResult();
                    //<INIGTI_753>

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
                                new XElement("fec_fnac", (DateTime.ParseExact(dr["wl_num_nacimiento"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None).AddYears(-anhosAdicionales)).ToString("yyyyMMdd")),
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
                        //<INIGTI_1092>//80=TITULAR,wl_cot_csex_c(0=M, 1=F)
                        if (dr["wl_cot_crel_c"].ToString() == "80")
                        {
                            wl_sexo = dr["wl_cot_csex_c"].ToString() == "0" ? "3" : "4";
                        }
                        //<FINGTI_1092>
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
                                        //<SOLINIGTI_754>
                                        new XElement("ini_tra2", dr["ini_tra2"].ToString()),
                                        new XElement("pje_rent", dr["pje_rent"].ToString()),
                                        //<SOLFINGTI_754>
                                        //<GTI.INI-15819>
                                        new XElement("val_tpmc", dr["val_tpmc"].ToString())
                                    //<GTI.FIN-15819>
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

                        //<INIGTI_754>
                        par.x_cot_ini_tra2 = Convert.ToInt32(dr["ini_tra2"].ToString());
                        par.x_cot_pje_rent = Convert.ToInt32(dr["pje_rent"].ToString());
                        //<FINGTI_754>

                        //<INIGTI_4081>
                        par.wl_val_htva = Convert.ToDouble(dr["tas_htva"]);
                        //<FINGTI_4081>

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


                    //<INIGTI_1092>Solo Para cod_tipo_invalidez="T"
                    var param = parametros.Find(p => p.cod_tipo_invalidez == "T");

                    if (param != null)
                    {
                        if (nullAjuste == false)//Solo cuando no sea NUll
                        {
                            XElement registro =
                            new XElement("Registro",
                                new XElement("num_tram", "0"),
                                new XElement("num_colu", wl_sexo),
                                new XElement("num_mvig", "1320.00"),
                                new XElement("val_ajus", param.val_ajuste_invalidez.ToString())
                            );
                            ajutdm.Add(registro);
                        }
                        else
                        {
                            throw new Exception("No se han cargado los valores de ajuste de categoría por invalidez.");
                        }
                    }
                    //<FINGTI_1092>


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
        }

        public void RegistrarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_ins_cotiza_movimiento");

                db.AddInParameter(dbc, "@wl_xml_cotiza_movimiento", DbType.String, XMLCotizacionMovimiento);
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //<INIGTI_4081_2>
        public void ActualizarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_upd_cotiza_movimiento");

                db.AddInParameter(dbc, "@wl_xml_cotiza_movimiento", DbType.String, XMLCotizacionMovimiento);
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //<FINGTI_4081_2>

        public int RegistrarDescargaSolicitudes(string xml, string usuario)
        {
            try
            {
                XDocument xdoc = XDocument.Parse(xml);
                xdoc.Declaration = new XDeclaration("1.0", "", "yes");

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_carga_solicitudes_XML");
                dbc.CommandTimeout = 150;

                db.AddInParameter(dbc, "@doc", DbType.String, xdoc.ToString(SaveOptions.DisableFormatting));
                //db.AddInParameter(dbc, "@doc", DbType.String, xml);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, usuario);
                db.AddOutParameter(dbc, "@wl_lote_cotizacion", DbType.Int32, 10);

                db.ExecuteNonQuery(dbc);

                return Convert.ToInt32(db.GetParameterValue(dbc, "@wl_lote_cotizacion"));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Solicitud> RegistrarDescargaResultados(string xml, string usuario, ref int lote)
        {
            try
            {
                List<Solicitud> solicitudes = null;

                XDocument xdoc = XDocument.Parse(xml);
                xdoc.Declaration = new XDeclaration("1.0", "", "yes");

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_descarga_resultados_XML");
                dbc.CommandTimeout = 0;

                db.AddInParameter(dbc, "@doc", DbType.String, xdoc.ToString(SaveOptions.DisableFormatting));
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, usuario);
                db.AddOutParameter(dbc, "@wo_num_lote_resultado", DbType.Int32, 10);

                using (IDataReader dr = db.ExecuteReader(dbc))
                {
                    solicitudes = new List<Solicitud>();
                    while (dr.Read())
                    {
                        Solicitud solicitud = new Solicitud();
                        solicitud.Id = dr["num_operacion"].ToString();
                        solicitudes.Add(solicitud);
                    }
                }

                lote = Convert.ToInt32(db.GetParameterValue(dbc, "@wo_num_lote_resultado"));

                return solicitudes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Solicitud> ListarSolicitudesPorFechaCierreAFP(DateTime fechaInicio, DateTime fechaFin, char enviada)
        {
            try
            {
                List<Solicitud> listaSolicitudes = new List<Solicitud>();
                Solicitud solicitud;

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solicitudes_x_fec_cierre", enviada, fechaInicio, fechaFin))
                {
                    while (dr.Read())
                    {
                        solicitud = new Solicitud();
                        solicitud.Id = dr["num_operacion"].ToString();
                        solicitud.Afiliado = new Afiliado
                        {
                            Nombre = dr["gls_primer_nombre"].ToString() + " " + dr["gls_segundo_nombre"].ToString(),
                            ApellidoPaterno = dr["ape_paterno"].ToString(),
                            ApellidoMaterno = dr["ape_materno"].ToString(),
                            CUSPP = dr["num_cuspp"].ToString()
                        };
                        solicitud.AFP = new AFP
                        {
                            Id = dr["cod_afp"].ToString(),
                            Nombre = dr["gls_afp"].ToString()
                        };
                        if (dr["fec_cierre"] != DBNull.Value)
                            solicitud.FechaCierre = Convert.ToDateTime(dr["fec_cierre"]);
                        solicitud.Enviada = dr["ind_solicitud_enviada"].ToString() == "S" ? true : false;

                        listaSolicitudes.Add(solicitud);
                    }
                }

                return listaSolicitudes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Solicitud> ListarConfirmaciones(DateTime fechaInicio, DateTime fechaFin, char enviada)
        {
            try
            {
                List<Solicitud> listaSolicitudes = new List<Solicitud>();
                Solicitud solicitud;

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_carga_confirmacioines_meler", fechaInicio, fechaFin, enviada))
                {
                    while (dr.Read())
                    {
                        solicitud = new Solicitud();
                        solicitud.Id = dr["num_operacion"].ToString();
                        solicitud.NumeroPoliza = Convert.ToInt32(dr["num_poliza"]);
                        solicitud.Afiliado = new Afiliado
                        {
                            Nombre = dr["gls_primer_nombre"].ToString() + " " + dr["gls_segundo_nombre"].ToString(),
                            ApellidoPaterno = dr["ape_paterno"].ToString(),
                            ApellidoMaterno = dr["ape_materno"].ToString()
                        };
                        if (dr["fec_carga_confirmacion"] != DBNull.Value)
                            solicitud.FechaCargaConfirmacion = Convert.ToDateTime(dr["fec_carga_confirmacion"]);
                        solicitud.Enviada = dr["ind_confirmacion_enviada"].ToString() == "S" ? true : false;

                        listaSolicitudes.Add(solicitud);
                    }
                }

                return listaSolicitudes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizarSolicitudesCargaMeler(string xml, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_solicitudes_carga_MELER");

                db.AddInParameter(dbc, "@doc", DbType.String, xml);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizarSolicitudesConfirmacionMeler(string xml, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_solicitudes_carga_MELER");

                db.AddInParameter(dbc, "@doc", DbType.String, xml);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Solicitud ObtenerCotizacionRecalculo(string numeroSolicitud, string usuario)
        {
            Solicitud solicitud = null;
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_cotizacion_recalculo", numeroSolicitud, usuario))
            {
                if (dr.Read())
                {
                    solicitud = new Solicitud();
                    solicitud.Id = dr["num_solicitud"].ToString();
                    if (dr["num_poliza"] != DBNull.Value)
                        solicitud.NumeroPoliza = Convert.ToInt32(dr["num_poliza"]);
                    solicitud.Cotizaciones = new List<Cotizacion>();
                    Cotizacion cotizacion = new Cotizacion();
                    cotizacion.Correlativo = Convert.ToInt64(dr["num_correlativo"]);
                    if (dr["fec_cotizacion"] != DBNull.Value)
                        cotizacion.FechaCotizacion = Convert.ToDateTime(dr["fec_cotizacion"]);
                    if (dr["val_tipo_cambio"] != DBNull.Value)
                        cotizacion.ValorMoneda = Convert.ToDouble(dr["val_tipo_cambio"]);
                    solicitud.SaldoCIC = Convert.ToDouble(dr["val_mto_cta_individual"]);
                    if (dr["cod_modalidad"] != DBNull.Value)
                        cotizacion.Modalidad = new Modalidad { Id = dr["cod_modalidad"].ToString(), Indicador = dr["ind_modalidad"].ToString(), Nombre = dr["gls_capital"].ToString() };
                    if (dr["val_mto_cia"] != DBNull.Value)
                        cotizacion.MontoCia = Convert.ToDouble(dr["val_mto_cia"]);
                    if (dr["val_pen_cia"] != DBNull.Value)
                        cotizacion.PensionCia = Convert.ToDouble(dr["val_pen_cia"]);
                    if (dr["val_pen_cia_mo"] != DBNull.Value)
                        cotizacion.PensionCiaMO = Convert.ToDouble(dr["val_pen_cia_mo"]);
                    if (dr["val_tasa_int_temp"] != DBNull.Value)
                        cotizacion.TasaAFP = Convert.ToDouble(dr["val_tasa_int_temp"]);
                    if (dr["val_mto_afp"] != DBNull.Value)
                        cotizacion.MontoAFP = Convert.ToDouble(dr["val_mto_afp"]);
                    if (dr["val_pen_afp"] != DBNull.Value)
                        cotizacion.PensionAFP = Convert.ToDouble(dr["val_pen_afp"]);
                    cotizacion.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString(), Simbolo = dr["gls_adicional"].ToString() };
                    if (dr["val_tasa_venta_ash"] != DBNull.Value)
                        cotizacion.TasaVenta = Convert.ToDouble(dr["val_tasa_venta_ash"]);
                    if (dr["val_tasa_int_vit"] != DBNull.Value)
                        cotizacion.TasaVentaSbs = Convert.ToDouble(dr["val_tasa_int_vit"]);

                    solicitud.Cotizaciones.Add(cotizacion);
                }
            }

            return solicitud;
        }

        public List<Solicitud> ListarSolicitudesCargaMeler(string xml)
        {
            try
            {
                List<Solicitud> listaSolicitudes = new List<Solicitud>();
                Solicitud solicitud = null;
                Cotizacion cotizacion = null;

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solicitudes_carga_MELER", xml))
                {
                    string num_operacion_aux = String.Empty;
                    while (dr.Read())
                    {
                        if (num_operacion_aux != dr["num_operacion"].ToString())
                        {
                            if (solicitud != null) listaSolicitudes.Add(solicitud);
                            solicitud = new Solicitud();
                            solicitud.Id = dr["num_operacion"].ToString();
                            solicitud.Afiliado = new Afiliado { CUSPP = dr["cuspp"].ToString() };
                            solicitud.Cotizaciones = new List<Cotizacion>();
                            num_operacion_aux = dr["num_operacion"].ToString();
                        }

                        cotizacion = new Cotizacion();
                        cotizacion.Modalidad = new Modalidad { Id = dr["cod_modalidad"].ToString() };
                        cotizacion.PrimaUnicaAFPEESS = Convert.ToDouble(dr["primaUnicaAFPEESS"]);
                        cotizacion.PrimaUnicaEESS = Convert.ToDouble(dr["primaUnicaEESS"]);
                        cotizacion.Moneda = new Moneda { Simbolo = dr["cod_moneda"].ToString() };
                        if (dr["num_anos_temporal"] != DBNull.Value)
                            cotizacion.PeriodoDiferido = Convert.ToInt32(dr["num_anos_temporal"]);
                        if (dr["pje_rvd"] != DBNull.Value)
                            cotizacion.PorcentajeEntreRentas = Convert.ToInt32(dr["pje_rvd"]);
                        if (dr["per_garantizado"] != DBNull.Value)
                            cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["per_garantizado"]);
                        if (dr["pje_conyuge"] != DBNull.Value)
                            cotizacion.PorcentajeConyuge = Convert.ToInt32(dr["pje_conyuge"]);
                        cotizacion.IndCotizacion = dr["ind_cotizacion"].ToString() == "S" ? true : false;
                        cotizacion.Correlativo = Convert.ToInt64(dr["num_cotizacion"]);
                        cotizacion.MontoCia = Convert.ToDouble(dr["val_mto_cia"]);
                        cotizacion.PrimeraPensionRV = Convert.ToDouble(dr["val_primera_pension_RV"]);
                        cotizacion.TasaInteresRV = Convert.ToDouble(dr["val_tasa_interes_RV"]);
                        cotizacion.PrimeraPensionRT = Convert.ToDouble(dr["val_primera_pension_RT"]);
                        cotizacion.TasaInteresRT = Convert.ToDouble(dr["val_tasa_interes_RT"]);
                        cotizacion.PrimeraPensionRVD = Convert.ToDouble(dr["val_primera_pension_RVD"]);
                        cotizacion.TasaInteresRVD = Convert.ToDouble(dr["val_tasa_interes_rvd"]);
                        cotizacion.DerechoCrecer = dr["ind_derecho_crecer"].ToString() == "S" ? true : false;
                        cotizacion.Gratificacion = dr["ind_gratificacion"].ToString() == "S" ? true : false;
                        cotizacion.Capital = new Capital { Id = dr["cod_particion_capital"].ToString() };
                        cotizacion.Cotiza = dr["ind_cotiza"].ToString();

                        //<GTI.INI-29372>
                        cotizacion.IndEnvioObligatorio = dr["ind_envio_obligatorio"].ToString() == "S" ? true : false;
                        //<GTI.FIN-29372>

                        solicitud.Cotizaciones.Add(cotizacion);
                    }
                    if (solicitud != null) listaSolicitudes.Add(solicitud);
                }

                return listaSolicitudes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Solicitud> ListarSolicitudesConfirmacionMeler(string xml)
        {
            try
            {
                List<Solicitud> listaSolicitudes = new List<Solicitud>();
                Solicitud solicitud;
                Cotizacion cotizacion;

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solicitudes_carga_MELER", xml))
                {
                    while (dr.Read())
                    {
                        solicitud = new Solicitud();
                        solicitud.Id = dr["num_operacion"].ToString();
                        solicitud.Afiliado = new Afiliado { CUSPP = dr["cuspp"].ToString() };

                        cotizacion = new Cotizacion();
                        cotizacion.Modalidad = new Modalidad { Id = dr["cod_modalidad"].ToString() };
                        cotizacion.PrimaUnicaAFPEESS = Convert.ToDouble(dr["primaUnicaAFPEESS"]);
                        cotizacion.PrimaUnicaEESS = Convert.ToDouble(dr["primaUnicaEESS"]);
                        cotizacion.Moneda = new Moneda { Simbolo = dr["cod_moneda"].ToString() };
                        cotizacion.PeriodoDiferido = Convert.ToInt32(dr["num_anos_temporal"]);
                        cotizacion.PorcentajeEntreRentas = Convert.ToInt32(dr["pje_rvd"]);
                        cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["per_garantizado"]);
                        cotizacion.PorcentajeConyuge = Convert.ToInt32(dr["pje_conyuge"]);
                        cotizacion.IndCotizacion = dr["ind_cotizacion"].ToString() == "S" ? true : false;
                        cotizacion.Correlativo = Convert.ToInt64(dr["num_cotizacion"]);
                        cotizacion.MontoCia = Convert.ToDouble(dr["val_mto_cia"]);
                        if (dr["fec_inicio_vigencia"] != DBNull.Value)
                            solicitud.FechaDevengue = Convert.ToDateTime(dr["fec_inicio_vigencia"]);
                        cotizacion.PrimeraPensionRV = Convert.ToDouble(dr["val_primera_pension_RV"]);
                        cotizacion.TasaInteresRV = Convert.ToDouble(dr["val_tasa_interes_RV"]);
                        cotizacion.PrimeraPensionRT = Convert.ToDouble(dr["val_primera_pension_RT"]);
                        cotizacion.TasaInteresRT = Convert.ToDouble(dr["val_tasa_interes_RT"]);
                        cotizacion.PrimeraPensionRVD = Convert.ToDouble(dr["val_primera_pension_RVD"]);
                        cotizacion.TasaInteresRVD = Convert.ToDouble(dr["val_tasa_interes_rvd"]);
                        cotizacion.DerechoCrecer = dr["ind_derecho_crecer"].ToString() == "S" ? true : false;
                        cotizacion.Gratificacion = dr["ind_gratificacion"].ToString() == "S" ? true : false;
                        cotizacion.Capital = new Capital { Id = dr["cod_particion_capital"].ToString() };
                        cotizacion.Cotiza = dr["ind_cotiza"].ToString();

                        solicitud.Cotizaciones = new List<Cotizacion>();
                        solicitud.Cotizaciones.Add(cotizacion);

                        listaSolicitudes.Add(solicitud);
                    }
                }

                return listaSolicitudes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double ObtenerMontoACOM(string solicitud, double acom, long cotizacion)
        {
            try
            {
                double monto = 0;

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.SP_RENVI_SEL_ACOM_MAXIMO");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, solicitud);
                db.AddInParameter(dbc, "@wl_val_acom_elegido", DbType.Double, acom);
                db.AddInParameter(dbc, "@wl_num_cotizacion", DbType.String, cotizacion);
                db.AddOutParameter(dbc, "@wl_gls_error", DbType.String, 200);

                using (IDataReader dr = db.ExecuteReader(dbc))
                {
                    if (dr.Read())
                    {
                        monto = Convert.ToDouble(dr["val_mto_acom_agente"]);
                    }
                }

                return monto;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizarValidacion(string idSolicitud, string tipoValidacion, string valor, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_validaciones");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, idSolicitud);
                db.AddInParameter(dbc, "@wl_cod_validacion", DbType.String, tipoValidacion);
                db.AddInParameter(dbc, "@wl_val_validacion", DbType.String, valor);
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //<SOLINI25621>
        public List<ParametroCotizador> ObtenerParametrosCapitalRequerido(CapitalRequerido capitalRequerido)
        {
            int anhosAdicionales = 0;
            //<INIGTI_1092>
            bool nullAjuste = false;
            string wl_sexo = "";
            //<FINGTI_1092>
            List<ParametroCotizador> parametros = new List<ParametroCotizador>();
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                List<Parinv> lParinv = new List<Parinv>();
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Soles.StringValue() });
                //<SRIINI17003>
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.Dolares.StringValue() });
                //<SRIFIN17003>
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.SolesAjustados.StringValue() });
                lParinv.Add(new Parinv { IdMoneda = Enums.Moneda.DolaresAjustados.StringValue() });

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_carpr_solicitud_calculo_capital", capitalRequerido.num_cuissp,
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
                        parametro.cot_tas_vtra = Convert.ToDouble(dr["wl_val_vtra"]);//luego sera sumado
                        parametro.cot_tas_vtra_sintra = Convert.ToDouble(dr["wl_val_vtra"]);
                        parametro.cot_tas_tafp = Convert.ToDouble(dr["wl_val_tasa_afp"]);
                        parametro.cot_val_acom = Convert.ToDouble(dr["wl_val_acom"]);
                        parametro.cot_val_dcom = Convert.ToDouble(dr["wl_cot_pdco"]);
                        parametro.cot_val_puam = Convert.ToDouble(dr["wl_val_puam"]);
                        parametro.cot_val_puni = Convert.ToDouble(dr["wl_val_puni"]);
                        parametro.cot_por_prrt = Convert.ToDouble(dr["wl_cot_prrt"]);
                        parametro.cot_val_tgfi = Convert.ToDouble(dr["wl_val_tgfi"]);

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

                        //<INIGTI_1092>
                        if (dr["val_ajuste_invalidez"] == DBNull.Value)
                        {
                            nullAjuste = true;
                        }
                        else
                        {
                            parametro.val_ajuste_invalidez = Convert.ToDouble(dr["val_ajuste_invalidez"]);
                        }
                        //<FINGTI_1092>

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
                                new XElement("fec_fnac", (DateTime.ParseExact(dr["wl_num_nacimiento"].ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None).AddYears(-anhosAdicionales)).ToString("yyyyMMdd")),
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

                        //<INIGTI_1092>//80=TITULAR,wl_cot_csex_c(0=M, 1=F)
                        if (dr["wl_cot_crel_c"].ToString() == "80")
                        {
                            wl_sexo = dr["wl_cot_csex_c"].ToString() == "0" ? "3" : "4";
                        }
                        //<FINGTI_1092>

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

                    //<INIGTI_1092>Solo Para cod_tipo_invalidez="T"
                    var param = parametros.Find(p => p.cod_tipo_invalidez == "T");

                    if (param != null)
                    {
                        if (nullAjuste == false)//Solo cuando no sea NUll
                        {
                            XElement registro =
                            new XElement("Registro",
                                new XElement("num_tram", "0"),
                                new XElement("num_colu", wl_sexo),
                                new XElement("num_mvig", "1320.00"),
                                new XElement("val_ajus", param.val_ajuste_invalidez.ToString())
                            );
                            ajutdm.Add(registro);
                        }
                        else
                        {
                            throw new Exception("No se han cargado los valores de ajuste de categoría por invalidez.");
                        }
                    }
                    //<FINGTI_1092>

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
        //<SOLFIN25621>

        //<INIGTI_1092>
        public String SolicitudesHabilitadas(int lote, string solicitudes)
        {
            string nroSolicitudes = "";
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_solictudes_x_lote_habilitado", lote, solicitudes))
            {
                while (dr.Read())
                {
                    nroSolicitudes = nroSolicitudes + dr["num_solicitud"].ToString() + ",";
                }
            }
            if (nroSolicitudes.Trim() != "")
            {
                nroSolicitudes = nroSolicitudes.Substring(0, nroSolicitudes.Length - 1);
            }
            return nroSolicitudes;
        }
        //<FINGTI_1092>

        public void Registrar(Solicitud entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Solicitud entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Solicitud entity)
        {
            throw new NotImplementedException();
        }

        public Solicitud ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Solicitud ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Solicitud> Listar()
        {
            throw new NotImplementedException();
        }

        //<INIGTI_7012>

        public List<DatosSol> ObtenerDatosporSolicitud(string num_Solicitud)
        {
            List<DatosSol> listaDatosSolicitud = new List<DatosSol>();
            DatosSol DatosSolicitud;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_datos_solicitud_sel", num_Solicitud))
            {
                while (dr.Read())
                {
                    double ValPjeRenta = 0;
                    if (dr["val_pje_ret"] != DBNull.Value)
                        ValPjeRenta = Convert.ToDouble(dr["val_pje_ret"]);

                    //No Muestra Mayores de Edad o Segun el porcentaje de Renta
                    if (ValPjeRenta > 0)
                    {
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
                        DatosSolicitud.val_pje_conyuge = dr["val_pje_conyuge"].ToString();

                        DatosSolicitud.Val_prima_unica_sepelio = 0.00;
                        if (dr["val_res_sepelio"] != DBNull.Value)
                            DatosSolicitud.Val_prima_unica_sepelio = Convert.ToDouble(dr["val_res_sepelio"].ToString());

                        DatosSolicitud.Val_prima_unica_pension = 0.00;
                        if (dr["val_res_pension"] != DBNull.Value)
                            DatosSolicitud.Val_prima_unica_pension = Convert.ToDouble(dr["val_res_pension"].ToString());

                        DatosSolicitud.Val_prima_unica_devolucion = 0.00;
                        if (dr["val_res_devolucion"] != DBNull.Value)
                            DatosSolicitud.Val_prima_unica_devolucion = Convert.ToDouble(dr["val_res_devolucion"].ToString());

                        DatosSolicitud.Val_prima_unica_fallecimiento = 0.00;
                        if (dr["val_res_fallecimiento"] != DBNull.Value)
                            DatosSolicitud.Val_prima_unica_fallecimiento = Convert.ToDouble(dr["val_res_fallecimiento"].ToString());

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

                        DatosSolicitud.cod_canal_distribucion = dr["cod_canal_distribucion"].ToString();

                        DatosSolicitud.glsMail = dr["gls_mail"].ToString();
                        DatosSolicitud.telefono = dr["num_telefono"].ToString();
                        DatosSolicitud.celular = dr["num_celular"].ToString();
                        DatosSolicitud.centroLaboral = dr["gls_centro_laboral"].ToString();
                        DatosSolicitud.cargo = dr["gls_cargo"].ToString();
                        DatosSolicitud.actividadEconomica = dr["gls_actividad_economica"].ToString();
                        DatosSolicitud.monedaIngreso = new Moneda() { Id = dr["cod_moneda_ingreso"].ToString() };

                        if (dr["val_ingreso_neto"] != DBNull.Value)
                        {
                            if (dr["val_ingreso_neto"].ToString() == "")
                            {
                                DatosSolicitud.ingresoNeto = 0;
                            }
                            else
                            {
                                DatosSolicitud.ingresoNeto = (float)Convert.ToDouble(dr["val_ingreso_neto"].ToString());
                            }
                        }

                        DatosSolicitud.gls_identificacion_afiliado = dr["gls_corta_identificacion"].ToString();

                        listaDatosSolicitud.Add(DatosSolicitud);
                    }
                }
            }

            return listaDatosSolicitud;
        }

        //<FINGTI_7012>

        //<INIGTI_7012>

        public List<Temporal> ListarGruposFamiliaresxSolicitud(string num_Solicitud)
        {
            List<Temporal> listaSolicitudes = new List<Temporal>();
            Temporal solicitud;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_lista_grupo_familiar", num_Solicitud))
            {
                while (dr.Read())
                {

                    solicitud = new Temporal();
                    solicitud.item = Convert.ToInt32(dr["item"].ToString());
                    solicitud.codigo = dr["codigo"].ToString();
                    solicitud.cantidad = Convert.ToInt32(dr["cantidad"].ToString());

                    listaSolicitudes.Add(solicitud);

                }
            }

            return listaSolicitudes;
        }

        //<FINGTI_7012>

        public void PreseleccionarCotizacion(string num_solicitud, string num_correlativo, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_preseleccionar_cotizacion");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, num_solicitud);
                db.AddInParameter(dbc, "@wl_num_correlativo", DbType.Int32, num_correlativo);
                db.AddInParameter(dbc, "@wl_usr_auditoria", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SeleccionarBeneficiario(string num_solicitud, string num_correlativo, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_seleccionar_beneficiario");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, num_solicitud);
                db.AddInParameter(dbc, "@wl_num_correlativo", DbType.Int32, num_correlativo);
                db.AddInParameter(dbc, "@wl_usr_auditoria", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ProcesarRecalculoCotizacion(string numSolicitud, DateTime fechaCotizacion, double montoCIC, double tipoCambio, string tipoCalculo, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_procesar_recalculo_cotizacion");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, numSolicitud);
                db.AddInParameter(dbc, "@wl_fec_cotizacion", DbType.DateTime, fechaCotizacion);
                db.AddInParameter(dbc, "@wl_val_mto_cta_individual", DbType.Double, montoCIC);
                db.AddInParameter(dbc, "@wl_val_tipo_cambio", DbType.Double, tipoCambio);
                db.AddInParameter(dbc, "@wl_cod_tipo_calculo", DbType.String, tipoCalculo);
                db.AddInParameter(dbc, "@wl_aud_usuario", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public PolizaRV ObtenerDatosPoliza(string num_solicitud, int numeroCorrelativo, string usuario)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            var poliza = new PolizaRV();

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_datos_poliza_rv", num_solicitud, numeroCorrelativo, usuario))
            {
                while (dr.Read())
                {
                    poliza.num_poliza = Convert.ToInt32(dr["num_poliza"]);
                    poliza.cod_tipo_identificacion = dr["cod_tipo_identificacion"].ToString();
                    poliza.num_identificacion = Convert.ToInt32(dr["num_identificacion"]);
                    poliza.cod_tipo_pension = dr["cod_tipo_pension"].ToString();
                    poliza.cod_tipo_producto = dr["cod_tipo_producto"].ToString();
                    poliza.cod_modalidad = dr["cod_modalidad"].ToString();
                    poliza.ind_modalidad = dr["ind_modalidad"].ToString();
                    poliza.ind_fallecimiento = dr["ind_fallecimiento"].ToString();
                    poliza.fec_devengue = Convert.ToDateTime(dr["fec_devengue"]);
                    poliza.cod_moneda = dr["cod_moneda"].ToString();
                    poliza.num_cuispp = dr["num_cuispp"].ToString();
                    poliza.val_pen_ref_mo = Convert.ToDouble(dr["val_pen_ref_mo"]);
                    poliza.cod_afp = dr["cod_afp"].ToString();
                    poliza.num_vendedor = Convert.ToInt32(dr["num_vendedor"]);
                    poliza.cod_origen_vta = dr["cod_origen_vta"].ToString();
                    poliza.val_per_garantizado = Convert.ToInt32(dr["val_per_garantizado"]);
                    poliza.val_per_temporal = Convert.ToInt32(dr["val_per_temporal"]);
                    poliza.val_mto_cia = Convert.ToDouble(dr["val_mto_cia"].ToString());
                    poliza.dig_poliza = dr["dig_poliza"].ToString();
                    if (dr["num_cotizacion_cierre"] != DBNull.Value)
                        poliza.num_cotizacion_cierre = Convert.ToInt32(dr["num_cotizacion_cierre"]);
                    poliza.val_pje_rent_temp = Convert.ToDouble(dr["val_pje_rent_temp"]);
                    poliza.cod_tipo_renta = dr["cod_tipo_renta"].ToString();
                    poliza.fec_emision_poliza = Convert.ToDateTime(dr["fec_emision_poliza"]);
                    poliza.ind_tiene_cobertura = dr["ind_tiene_cobertura"].ToString();
                    poliza.ind_derecho_crecer = dr["ind_derecho_crecer"].ToString();
                    poliza.ind_gratificacion = dr["ind_gratificacion"].ToString();
                    if (dr["fec_sol_pension"] != DBNull.Value)
                        poliza.fec_sol_pension = Convert.ToDateTime(dr["fec_sol_pension"]);
                    poliza.fec_recepcion = Convert.ToDateTime(dr["fec_recepcion"]);
                    poliza.cod_categoria = dr["cod_categoria"].ToString();
                    poliza.cod_cia_seguro = dr["cod_cia_seguro"].ToString();
                    poliza.val_mto_cta_individual = Convert.ToDouble(dr["val_mto_cta_individual"]);
                    poliza.cod_reajuste_pension_1 = dr["cod_reajuste_pension_1"].ToString();
                    poliza.val_par_reapen_1 = Convert.ToDouble(dr["val_par_reapen_1"]);
                    if (dr["cod_reajuste_pension_2"] != DBNull.Value)
                        poliza.cod_reajuste_pension_2 = dr["cod_reajuste_pension_2"].ToString();
                    if (dr["val_par_reapen_2"] != DBNull.Value)
                        poliza.val_par_reapen_2 = Convert.ToDouble(dr["val_par_reapen_2"]);
                }
            }

            return poliza;
        }

        public List<BeneficiarioRV> ObtenerDatosBeneficiarios(string num_solicitud, int numeroCorrelativo, string usuario)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            var beneficiarios = new List<BeneficiarioRV>();
            var poliza = new PolizaRV();

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_datos_beneficiarios_rv", num_solicitud, numeroCorrelativo, usuario))
            {
                while (dr.Read())
                {
                    var beneficiario = new BeneficiarioRV();

                    beneficiario.num_poliza = Convert.ToInt32(dr["num_poliza"]);
                    beneficiario.cod_tipo_identificacion = dr["cod_tipo_identificacion"].ToString();
                    beneficiario.num_identificacion = Convert.ToInt32(dr["num_identificacion"]);
                    //Persona
                    beneficiario.fec_nacimiento = Convert.ToDateTime(dr["fec_nacimiento"]);
                    beneficiario.fec_fallecimiento = (dr["fec_fallecimiento"] != DBNull.Value) ? Convert.ToDateTime(dr["fec_fallecimiento"]) : (DateTime?)null;
                    beneficiario.cod_sexo = dr["cod_sexo"].ToString();
                    beneficiario.ind_invalidez = dr["ind_invalidez"].ToString();
                    beneficiario.cod_tipo_invalidez = dr["cod_tipo_invalidez"].ToString();
                    beneficiario.fec_invalidez = (dr["fec_invalidez"] != DBNull.Value) ? Convert.ToDateTime(dr["fec_invalidez"]) : (DateTime?) null;
                    beneficiario.ape_paterno = dr["ape_paterno"].ToString();
                    beneficiario.ape_materno = dr["ape_materno"].ToString();
                    beneficiario.nom_persona = dr["nom_persona"].ToString();
                    beneficiario.gls_persona = dr["gls_persona"].ToString();
                    beneficiario.cod_nacionalidad = dr["cod_nacionalidad"].ToString();
                    //<GTI.59048-INI>
                    beneficiario.cod_equivalencia_nacionalidad = dr["cod_equivalencia_nacionalidad"].ToString();
                    beneficiario.cod_tipo_pacto_salud = dr["cod_tipo_pacto_salud"].ToString();
                    //<GTI.59048-FIN>
                    beneficiario.cod_pais_origen_doc = dr["cod_pais_origen_doc"].ToString();
                    beneficiario.cod_tipo_identificacion_pdt = dr["cod_tipo_identificacion_pdt"].ToString();
                    beneficiario.num_identificacion_pdt = dr["num_identificacion_pdt"].ToString();
                    //Beneficiario
                    beneficiario.num_correlativo = Convert.ToInt32(dr["num_correlativo"]);
                    beneficiario.cod_tipo_identificacion_apoderado = dr["cod_tipo_identificacion_apoderado"].ToString();
                    beneficiario.num_identificacion_apoderado = dr["num_identificacion_apoderado"].ToString();
                    beneficiario.cod_parentezco = dr["cod_parentezco"].ToString();
                    beneficiario.cod_estado_beneficiario = dr["cod_estado_beneficiario"].ToString();
                    beneficiario.cod_causal_estado_ben = dr["cod_causal_estado_ben"].ToString();
                    beneficiario.pje_pension = (dr["pje_pension"] != DBNull.Value) ? Convert.ToDouble(dr["pje_pension"]) : 100;
                    beneficiario.num_via_pago = dr["num_via_pago"].ToString();
                    beneficiario.cod_docparentesco = dr["cod_docparentesco"].ToString();
                    beneficiario.num_docparentesco = dr["num_docparentesco"].ToString();
                    beneficiario.gls_mail = dr["gls_mail"].ToString();
                    //--
                    beneficiario.num_celular = dr["num_celular"].ToString();
                    //Direccion
                    beneficiario.gls_direccion = dr["gls_direccion"].ToString();
                    beneficiario.cod_comuna = dr["cod_comuna"].ToString();
                    beneficiario.cod_ciudad = dr["cod_ciudad"].ToString();
                    beneficiario.num_telefono = dr["num_telefono"].ToString();
                    beneficiario.ind_vigencia = dr["ind_vigencia"].ToString();
                    beneficiario.cod_tipo_via_rviadm = dr["cod_tipo_via_rviadm"].ToString();
                    beneficiario.gls_nom_via = dr["gls_nom_via"].ToString();
                    beneficiario.gls_espacio_urbano = dr["gls_espacio_urbano"].ToString();
                    beneficiario.gls_num_via = dr["gls_num_via"].ToString();
                    beneficiario.gls_num_interior = dr["gls_num_interior"].ToString();
                    beneficiario.cod_tipo_zona = dr["cod_tipo_zona"].ToString();
                    beneficiario.gls_nom_zona = dr["gls_nom_zona"].ToString();
                    beneficiario.gls_referencia = dr["gls_referencia"].ToString();
                    beneficiario.cod_larga_distancia = dr["cod_larga_distancia"].ToString();
                    beneficiario.gls_departamento = dr["gls_departamento"].ToString();
                    beneficiario.gls_manzana = dr["gls_manzana"].ToString();
                    beneficiario.gls_lote = dr["gls_lote"].ToString();
                    beneficiario.gls_kilometro = dr["gls_kilometro"].ToString();
                    beneficiario.gls_block = dr["gls_block"].ToString();
                    beneficiario.gls_etapa = dr["gls_etapa"].ToString();

                    beneficiarios.Add(beneficiario);
                }
            }

            return beneficiarios;
        }

        public void GenerarPolizaRVI(string numSolicitud, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_generar_poliza_RVI");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, numSolicitud);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Solicitud ListarCotizacionesPorSolicitud(string numeroSolicitud, string usuario)
        {
            try
            {
                Solicitud solicitud = new Solicitud();
                solicitud.Id = numeroSolicitud;
                List<Cotizacion> cotizaciones = new List<Cotizacion>();

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_cotizaciones", numeroSolicitud, usuario))
                {
                    while (dr.Read())
                    {
                        Cotizacion cotizacion = new Cotizacion();
                        if (dr["num_poliza"] != DBNull.Value)
                            solicitud.NumeroPoliza = Convert.ToInt32(dr["num_poliza"]);
                        solicitud.TipoCotizacion = new TipoCotizacion { Id = dr["cod_tipo_cotizacion"].ToString() };
                        solicitud.TipoPension = new TipoPension { Id = dr["cod_tipo_pension"].ToString() };
                        solicitud.FechaDevengue = Convert.ToDateTime(dr["fec_devengue"]);
                        cotizacion.FechaCotizacion = Convert.ToDateTime(dr["fec_cotizacion"]);
                        cotizacion.Correlativo = Convert.ToInt64(dr["num_correlativo"]);
                        cotizacion.EstadoCotizacion = dr["cod_estado_cotizacion"].ToString();
                        cotizacion.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString(), Simbolo = dr["gls_moneda_simbolo"].ToString() };
                        cotizacion.ValorMoneda = Convert.ToDouble(dr["val_moneda"]);
                        cotizacion.Producto = new Producto { Id = dr["cod_tipo_producto"].ToString() };
                        cotizacion.Modalidad = new Modalidad { Id = dr["cod_modalidad"].ToString(), Nombre = dr["gls_modalidad"].ToString(), Indicador = dr["ind_modalidad"].ToString() };
                        cotizacion.PeriodoGarantizado = Convert.ToInt32(dr["val_per_garantizado"]);
                        cotizacion.PeriodoDiferido = Convert.ToInt32(dr["val_per_temporal"]);
                        cotizacion.PorcentajeEntreRentas = Convert.ToInt32(dr["val_pje_rent_temp"]);
                        cotizacion.PuurAFP = Convert.ToDouble(dr["val_fac_afp"]);
                        cotizacion.MontoAFP = Convert.ToDouble(dr["val_mto_afp"]);
                        cotizacion.PensionAFP = Convert.ToDouble(dr["val_pen_afp"]);
                        cotizacion.PuurCia = Convert.ToDouble(dr["val_fac_cia"]);
                        cotizacion.MontoCia = Convert.ToDouble(dr["val_mto_cia"]);
                        cotizacion.PensionCia = Convert.ToDouble(dr["val_pen_cia"]);
                        cotizacion.PensionCiaMO = Convert.ToDouble(dr["val_pen_cia_mo"]);
                        cotizacion.TasaVenta = Convert.ToDouble(dr["val_tasa_int_vit"]);
                        cotizacion.TasaAFP = Convert.ToDouble(dr["val_tasa_int_temp"]);
                        if (dr["cod_tipo_calculo"] != DBNull.Value)
                            cotizacion.TipoCalculo = new TipoCalculo { Id = dr["cod_tipo_calculo"].ToString(), Nombre = dr["gls_tipo_calculo"].ToString() };
                        cotizacion.TasaRetornoAccionista = Convert.ToDouble(dr["val_tasa_ret_accion"]);
                        cotizacion.TasaVentaSbs = Convert.ToDouble(dr["val_tasa_venta_ash"]);
                        cotizacion.TasaCostoEquivalente = Convert.ToDouble(dr["val_tasa_costo_equiv"]);
                        cotizacion.Duration = Convert.ToDouble(dr["val_duration"]);
                        cotizacion.IndErrorCotiza = Convert.ToInt32(dr["num_error_cot"]);
                        if (dr["val_tasa_venta_ash_2"] != DBNull.Value)
                            cotizacion.TasaVentaSbs2 = Convert.ToDouble(dr["val_tasa_venta_ash_2"]);
                        if (dr["val_tasa_costo_equiv_2"] != DBNull.Value)
                            cotizacion.TasaCostoEquivalente2 = Convert.ToDouble(dr["val_tasa_costo_equiv_2"]);
                        if (dr["val_duration_2"] != DBNull.Value)
                            cotizacion.Duration2 = Convert.ToDouble(dr["val_duration_2"]);
                        if (dr["num_error_cot_2"] != DBNull.Value)
                            cotizacion.IndErrorCotiza2 = Convert.ToInt32(dr["num_error_cot_2"]);
                        cotizacion.TotalGarantizado = Convert.ToDouble(dr["val_total_garantizado"]);
                        cotizacion.AjusteTRA = Convert.ToDouble(dr["val_tasa_ajuste_tra"]);

                        cotizaciones.Add(cotizacion);
                    }
                    solicitud.Cotizaciones = cotizaciones;
                    return solicitud;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizarCotizacionGanadoraRVI(string solicitud, int correlativo, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_cotizacion_ganadora_rvi");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, solicitud);
                db.AddInParameter(dbc, "@wl_num_correlativo", DbType.Int32, correlativo);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizarEstudioNecesidad(string num_solicitud, int? idEstudioNecesidades, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_estudio_necesidad_solicitud");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, num_solicitud);
                db.AddInParameter(dbc, "@wl_id_estudio_necesidades", DbType.String, idEstudioNecesidades);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public AbonoPoliza ObtenerAbonoPorPoliza(int numeroPoliza, string usuario)
        {
            try
            {
                AbonoPoliza abono = new AbonoPoliza();

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_validar_poliza_recaudada", numeroPoliza, usuario))
                {
                    while (dr.Read())
                    {
                        abono.num_imputacion_mov = Convert.ToInt32(dr["num_imputacion_mov"]);
                        abono.cod_concepto_abono_cargo = dr["cod_concepto_abono_cargo"].ToString();
                        abono.val_pesos_abono = Convert.ToDouble(dr["val_pesos_abono"]);
                    }

                    return abono;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
