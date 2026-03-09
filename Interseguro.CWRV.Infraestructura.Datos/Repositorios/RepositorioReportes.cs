using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioReportes : IRepositorioReportes
    {

        public List<ConsentimientosAgrupadosAge> ListarConsentimientosAgrupados(string periodo, string usuario)
        {

            List<ConsentimientosAgrupadosAge> listaConsentimientosAgrupadosAge = new List<ConsentimientosAgrupadosAge>();
            ConsentimientosAgrupadosAge consentimientosAgrupadosAge;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_datos_consentimiento", periodo, usuario))
            {
                while (dr.Read())
                {
                    consentimientosAgrupadosAge = new ConsentimientosAgrupadosAge();

                    consentimientosAgrupadosAge.num_agente = Convert.ToInt32(dr["num_agente"].ToString());
                    //consentimientosAgrupadosAge.gls_nombres_agente = dr["gls_nombres_agente"].ToString();
                    consentimientosAgrupadosAge.ind_consentimiento = dr["ind_consentimiento"].ToString();
                    consentimientosAgrupadosAge.cantidad_ind_consentimiento = Convert.ToInt32(dr["cantidad_ind_consentimiento"].ToString());
                    consentimientosAgrupadosAge.fec_primer_envio = dr["ind_consentimiento"].ToString();
                    consentimientosAgrupadosAge.fec_ultimo_envio = dr["ind_consentimiento"].ToString();
                    //consentimientosAgrupadosAge.fec_asignacion = dr["ind_consentimiento"].ToString();
                    consentimientosAgrupadosAge.entrega_plazo = Convert.ToInt32(dr["entrega_plazo"].ToString());

                    listaConsentimientosAgrupadosAge.Add(consentimientosAgrupadosAge);
                }
            }

            return listaConsentimientosAgrupadosAge;

        } //end class

        public List<FirmaDigitalDashboard> ObtenerFirmaDigitalesDashboard(string fechaInicio, string fechaFin, string usuario)
        {

            List<FirmaDigitalDashboard> listaFirmaDigitalDashboard = new List<FirmaDigitalDashboard>();
            FirmaDigitalDashboard firmaDigitalDashboard;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_firma_digital_fechas", fechaInicio, fechaFin, usuario))
            {
                while (dr.Read())
                {
                    firmaDigitalDashboard = new FirmaDigitalDashboard();

                    if (dr["id_firma_digital"] != DBNull.Value)
                        firmaDigitalDashboard.id_firma_digital = Convert.ToInt32(dr["id_firma_digital"].ToString());
                    if (dr["gls_identificador"] != DBNull.Value)
                        firmaDigitalDashboard.gls_identificador = dr["gls_identificador"].ToString();
                    if (dr["nombres"] != DBNull.Value)
                        firmaDigitalDashboard.nombres = dr["nombres"].ToString().ToUpper();
                    if (dr["fec_primer_envio"] != DBNull.Value)
                        firmaDigitalDashboard.fec_primer_envio = Convert.ToDateTime(dr["fec_primer_envio"]);
                    if (dr["fec_ultimo_envio"] != DBNull.Value)
                        firmaDigitalDashboard.fec_ultimo_envio = Convert.ToDateTime(dr["fec_ultimo_envio"]);
                    if (dr["ind_consentimiento"] != DBNull.Value)
                        firmaDigitalDashboard.ind_consentimiento = dr["ind_consentimiento"].ToString();
                    if (dr["id_proceso_envio"] != DBNull.Value)
                        firmaDigitalDashboard.id_proceso_envio = Convert.ToInt32(dr["id_proceso_envio"].ToString());
                    if (dr["cod_agente"] != DBNull.Value)
                        firmaDigitalDashboard.cod_agente = dr["cod_agente"].ToString();
                    if (dr["nom_agente"] != DBNull.Value)
                        firmaDigitalDashboard.nom_agente = dr["nom_agente"].ToString().ToUpper();
                    if (dr["cod_username"] != DBNull.Value)
                        firmaDigitalDashboard.cod_username = dr["cod_username"].ToString();
                    if (dr["val_cant_envios_consentimiento"] != DBNull.Value)
                        firmaDigitalDashboard.val_cant_envios_consentimiento = Convert.ToInt32(dr["val_cant_envios_consentimiento"].ToString());
                    if (dr["num_identificacion"] != DBNull.Value)
                        firmaDigitalDashboard.gls_num_identificacion = dr["num_identificacion"].ToString();
                    if (dr["num_cuspp"] != DBNull.Value)
                        firmaDigitalDashboard.gls_num_cuspp = dr["num_cuspp"].ToString();

                    listaFirmaDigitalDashboard.Add(firmaDigitalDashboard);
                }
            }

            return listaFirmaDigitalDashboard;

        } //end class

        public List<PolizasDashboard> ObtenerPolizasDashboard(string fechaInicio, string fechaFin, string usuario)
        {

            List<PolizasDashboard> listaPolizasDashboard = new List<PolizasDashboard>();
            PolizasDashboard polizasDashboard;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_envio_polizas_fechas", fechaInicio, fechaFin, usuario))
            {
                while (dr.Read())
                {
                    polizasDashboard = new PolizasDashboard();

                    if (dr["gls_identificador"] != DBNull.Value)
                        polizasDashboard.gls_identificador = dr["gls_identificador"].ToString();
                    if (dr["nombres"] != DBNull.Value)
                        polizasDashboard.nombres = dr["nombres"].ToString().ToUpper();
                    if (dr["num_poliza"] != DBNull.Value)
                        polizasDashboard.num_poliza = dr["num_poliza"].ToString();
                    if (dr["fec_envio"] != DBNull.Value)
                        polizasDashboard.fec_envio = Convert.ToDateTime(dr["fec_envio"]);
                    if (dr["id_proceso_envio"] != DBNull.Value)
                        polizasDashboard.id_proceso_envio = Convert.ToInt32(dr["id_proceso_envio"].ToString());
                    if (dr["cod_agente"] != DBNull.Value)
                        polizasDashboard.cod_agente = dr["cod_agente"].ToString();
                    if (dr["nom_agente"] != DBNull.Value)
                        polizasDashboard.nom_agente = dr["nom_agente"].ToString().ToUpper();
                    if (dr["cod_username"] != DBNull.Value)
                        polizasDashboard.cod_username = dr["cod_username"].ToString();
                    if (dr["val_cant_envios_consentimiento"] != DBNull.Value)
                        polizasDashboard.val_cant_envios_consentimiento = Convert.ToInt32(dr["val_cant_envios_consentimiento"].ToString());
                    if (dr["num_identificacion"] != DBNull.Value)
                        polizasDashboard.gls_num_identificacion = dr["num_identificacion"].ToString();
                    if (dr["num_cuspp"] != DBNull.Value)
                        polizasDashboard.gls_num_cuspp = dr["num_cuspp"].ToString();

                    listaPolizasDashboard.Add(polizasDashboard);
                }
            }

            return listaPolizasDashboard;

        } //end class

        public List<ReporteTrazabilidad> ObtenerTrazabilidad(string fechaInicio, string fechaFin, string idProceso, string usuario)
        {

            List<ReporteTrazabilidad> listaEnvioSeguimiento = new List<ReporteTrazabilidad>();
            ReporteTrazabilidad envioSeguimiento;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_trazabilidad", fechaInicio, fechaFin, idProceso, usuario))
            {
                while (dr.Read())
                {
                    envioSeguimiento = new ReporteTrazabilidad();

                    if (dr["id_envio_seguimiento"] != DBNull.Value)
                        envioSeguimiento.id_envio_seguimiento = Convert.ToInt32(dr["id_envio_seguimiento"].ToString());
                    if (dr["gls_identificador"] != DBNull.Value)
                        envioSeguimiento.gls_identificador = dr["gls_identificador"].ToString();
                    if (dr["id_proceso_envio"] != DBNull.Value)
                        envioSeguimiento.id_proceso_envio = Convert.ToInt32(dr["id_proceso_envio"].ToString());
                    if (dr["id_sme"] != DBNull.Value)
                        envioSeguimiento.id_sme = Convert.ToInt64(dr["id_sme"].ToString());
                    if (dr["cod_estado_trazabilidad"] != DBNull.Value)
                        envioSeguimiento.cod_estado_trazabilidad = dr["cod_estado_trazabilidad"].ToString();
                    if (dr["gls_mail"] != DBNull.Value)
                        envioSeguimiento.gls_mail = dr["gls_mail"].ToString();
                    if (dr["fec_envio"] != DBNull.Value)
                        envioSeguimiento.fec_envio = Convert.ToDateTime(dr["fec_envio"]).ToString("dd/MM/yyyy HH:mm:ss");
                    if (dr["cod_agente"] != DBNull.Value)
                        envioSeguimiento.cod_agente = dr["cod_agente"].ToString();
                    if (dr["gls_motivo_rebote"] != DBNull.Value)
                        envioSeguimiento.gls_motivo_rebote = dr["gls_motivo_rebote"].ToString();
                    if (dr["nom_agente"] != DBNull.Value)
                        envioSeguimiento.nom_agente = dr["nom_agente"].ToString().ToUpper();
                    if (dr["fec_firma"] != DBNull.Value)
                        envioSeguimiento.fec_firma = Convert.ToDateTime(dr["fec_firma"]).ToString("dd/MM/yyyy HH:mm:ss");
                    if (dr["num_cuspp"] != DBNull.Value)
                        envioSeguimiento.num_cuspp = dr["num_cuspp"].ToString();
                    if (dr["gls_persona"] != DBNull.Value)
                        envioSeguimiento.gls_persona = dr["gls_persona"].ToString();

                    listaEnvioSeguimiento.Add(envioSeguimiento);
                }
            }

            return listaEnvioSeguimiento;

        } //end class

        public DataSet ListarReporteIndicadoresVCTP(DateTime fecPeriodo, string codUsername, string codRol)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_reporte_indicadores_vctp");

                db.AddInParameter(dbc, "@wl_fec_periodo", DbType.String, fecPeriodo.ToString("yyyyMMdd"));
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, codUsername);
                db.AddInParameter(dbc, "@wl_cod_rol", DbType.String, codRol);

                var data = db.ExecuteDataSet(dbc);

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public ReporteRecalculoCotizacion ObtenerReporteRecalculo(List<ReporteRecalculoCotizacion> listaRecalculoCotizacion)
        {

            //List<ReporteRecalculoCotizacion> lstReporteRecalculoCotizacion = new List<ReporteRecalculoCotizacion>();
            //List<ReporteRecalculoCotizacionDetalle> lstReporteRecalculoCotizacionDetalle = new List<ReporteRecalculoCotizacionDetalle>();
            ReporteRecalculoCotizacion recalculoCotizacion = new ReporteRecalculoCotizacion();
            ReporteRecalculoCotizacionDetalle recalculoCotizacionDetalle = new ReporteRecalculoCotizacionDetalle();
            List<ReporteRecalculoCotizacionBeneficiarios> lstReporteRecalculoCotizacionBeneficiarios = new List<ReporteRecalculoCotizacionBeneficiarios>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            ReporteRecalculoCotizacion XmlRecalculoCotizacion = new ReporteRecalculoCotizacion();
            string xml = XmlRecalculoCotizacion.XMLParametrosRecalculo(listaRecalculoCotizacion);

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_reporte_recalculo_cotizacion", xml))
            {
                while (dr.Read())
                {
                    if (dr["fec_cotizacion"] != DBNull.Value)
                        recalculoCotizacion.fec_cotizacion = Convert.ToDateTime(dr["fec_cotizacion"].ToString());
                    if (dr["fec_devengue"] != DBNull.Value)
                        recalculoCotizacion.fec_devengue = Convert.ToDateTime(dr["fec_devengue"].ToString());
                    if (dr["num_solicitud"] != DBNull.Value)
                        recalculoCotizacion.num_solicitud = dr["num_solicitud"].ToString();
                    if (dr["num_cuispp"] != DBNull.Value)
                        recalculoCotizacion.num_cuispp = dr["num_cuispp"].ToString();
                    if (dr["num_vendedor"] != DBNull.Value)
                        recalculoCotizacion.num_vendedor = dr["num_vendedor"].ToString();
                    if (dr["nom_agente"] != DBNull.Value)
                        recalculoCotizacion.nom_agente = dr["nom_agente"].ToString();
                    if (dr["cod_cartera"] != DBNull.Value)
                        recalculoCotizacion.cod_cartera = dr["cod_cartera"].ToString();
                    if (dr["gls_cartera"] != DBNull.Value)
                        recalculoCotizacion.gls_cartera = dr["gls_cartera"].ToString();
                    if (dr["gls_persona"] != DBNull.Value)
                        recalculoCotizacion.gls_persona = dr["gls_persona"].ToString();
                    if (dr["Direccion"] != DBNull.Value)
                        recalculoCotizacion.direccion = dr["Direccion"].ToString();
                    if (dr["Ubigeo"] != DBNull.Value)
                        recalculoCotizacion.ubigeo = dr["Ubigeo"].ToString();
                    if (dr["gls_celular"] != DBNull.Value)
                        recalculoCotizacion.gls_celular = dr["gls_celular"].ToString();
                    if (dr["gls_afp"] != DBNull.Value)
                        recalculoCotizacion.gls_afp = dr["gls_afp"].ToString();
                    if (dr["gls_tipo_pension"] != DBNull.Value)
                        recalculoCotizacion.gls_tipo_pension = dr["gls_tipo_pension"].ToString();
                    if (dr["gls_categoria"] != DBNull.Value)
                        recalculoCotizacion.gls_categoria = dr["gls_categoria"].ToString();
                    if (dr["gls_tipo_cotizacion"] != DBNull.Value)
                        recalculoCotizacion.gls_tipo_cotizacion = dr["gls_tipo_cotizacion"].ToString();
                    if (dr["val_moneda"] != DBNull.Value)
                        recalculoCotizacion.val_moneda = Convert.ToDouble(dr["val_moneda"].ToString());
                    if (dr["tasaAssetshare"] != DBNull.Value)
                        recalculoCotizacion.tasaAssetshare = Convert.ToDouble(dr["tasaAssetshare"].ToString());
                    if (dr["tasaVenta"] != DBNull.Value)
                        recalculoCotizacion.tasaVenta = Convert.ToDouble(dr["tasaVenta"].ToString());
                    if (dr["TRA"] != DBNull.Value)
                        recalculoCotizacion.TRA = Convert.ToDouble(dr["TRA"].ToString());
                    if (dr["CRU"] != DBNull.Value)
                        recalculoCotizacion.CRU = Convert.ToDouble(dr["CRU"].ToString());
                    if (dr["val_mto_cta_individual"] != DBNull.Value)
                        recalculoCotizacion.val_mto_cta_individual = Convert.ToDouble(dr["val_mto_cta_individual"].ToString());
                    if (dr["gls_corta_moneda"] != DBNull.Value)
                        recalculoCotizacion.gls_corta_moneda = dr["gls_corta_moneda"].ToString();
                    if (dr["val_mto_cia"] != DBNull.Value)
                        recalculoCotizacion.val_mto_cia = Convert.ToDouble(dr["val_mto_cia"].ToString());
                    if (dr["fec_ult_actualizacion"] != DBNull.Value)
                        recalculoCotizacion.fec_ult_actualizacion = Convert.ToDateTime(dr["fec_ult_actualizacion"].ToString());
                    if (dr["val_tasa_int_temp"] != DBNull.Value)
                        recalculoCotizacion.val_tasa_int_temp = Convert.ToDouble(dr["val_tasa_int_temp"].ToString());
                    if (dr["val_mon_aju"] != DBNull.Value)
                        recalculoCotizacion.val_mon_aju = Convert.ToDouble(dr["val_mon_aju"].ToString());
                    if (dr["AjtMon"] != DBNull.Value)
                        recalculoCotizacion.ajtMon = Convert.ToInt32(dr["AjtMon"].ToString());
                    
                }

                dr.NextResult();
                
                while (dr.Read())
                {
                    if (dr["num_correlativo"] != DBNull.Value)
                        recalculoCotizacionDetalle.num_correlativo = Convert.ToInt32(dr["num_correlativo"].ToString());
                    if (dr["gls_corta_tipo_pro"] != DBNull.Value)
                        recalculoCotizacionDetalle.gls_corta_tipo_pro = dr["gls_corta_tipo_pro"].ToString();
                    if (dr["Modalidad"] != DBNull.Value)
                        recalculoCotizacionDetalle.modalidad = dr["Modalidad"].ToString();
                    if (dr["val_per_temporal"] != DBNull.Value)
                        recalculoCotizacionDetalle.val_per_temporal = Convert.ToInt32(dr["val_per_temporal"].ToString());
                    if (dr["val_pje_rent_temp"] != DBNull.Value)
                        recalculoCotizacionDetalle.val_pje_rent_temp = Convert.ToInt32(dr["val_pje_rent_temp"].ToString());
                    if (dr["val_per_garantizado"] != DBNull.Value)
                        recalculoCotizacionDetalle.val_per_garantizado = Convert.ToInt32(dr["val_per_garantizado"].ToString());
                    if (dr["ind_derecho_crecer"] != DBNull.Value)
                        recalculoCotizacionDetalle.ind_derecho_crecer = dr["ind_derecho_crecer"].ToString();
                    if (dr["ind_gratificacion"] != DBNull.Value)
                        recalculoCotizacionDetalle.ind_gratificacion = dr["ind_gratificacion"].ToString();
                    if (dr["AFPPension"] != DBNull.Value)
                        recalculoCotizacionDetalle.AFPPension = Convert.ToDouble(dr["AFPPension"].ToString());
                    if (dr["Pension"] != DBNull.Value)
                        recalculoCotizacionDetalle.pension = Convert.ToDouble(dr["Pension"].ToString());
                    if (dr["MonedaPension"] != DBNull.Value)
                        recalculoCotizacionDetalle.monedaPension = dr["MonedaPension"].ToString();
                    if (dr["gls_corta_moneda"] != DBNull.Value)
                        recalculoCotizacionDetalle.gls_corta_moneda = dr["gls_corta_moneda"].ToString();
                    if (dr["Capital"] != DBNull.Value)
                        recalculoCotizacionDetalle.capital = Convert.ToInt32(dr["Capital"].ToString());
                    
                }

                dr.NextResult();

                ReporteRecalculoCotizacionBeneficiarios recalculoCotizacionBeneficiarios;

                while (dr.Read())
                {
                    recalculoCotizacionBeneficiarios = new ReporteRecalculoCotizacionBeneficiarios();

                    if (dr["num_correlativo"] != DBNull.Value)
                        recalculoCotizacionBeneficiarios.num_correlativo = Convert.ToInt32(dr["num_correlativo"].ToString());
                    if (dr["gls_corta_parentezco"] != DBNull.Value)
                        recalculoCotizacionBeneficiarios.gls_corta_parentezco = dr["gls_corta_parentezco"].ToString();
                    if (dr["fec_nacimiento"] != DBNull.Value)
                        recalculoCotizacionBeneficiarios.fec_nacimiento = Convert.ToDateTime(dr["fec_nacimiento"].ToString());
                         if (dr["cod_sexo"] != DBNull.Value)
                        recalculoCotizacionBeneficiarios.cod_sexo = dr["cod_sexo"].ToString();
                         if (dr["ind_invalidez"] != DBNull.Value)
                        recalculoCotizacionBeneficiarios.ind_invalidez = dr["ind_invalidez"].ToString();
                         if (dr["gls_corta_tipinv"] != DBNull.Value)
                        recalculoCotizacionBeneficiarios.gls_corta_tipinv = dr["gls_corta_tipinv"].ToString();
                         if (dr["gls_persona"] != DBNull.Value)
                        recalculoCotizacionBeneficiarios.gls_persona = dr["gls_persona"].ToString();

                    lstReporteRecalculoCotizacionBeneficiarios.Add(recalculoCotizacionBeneficiarios);

                }

                recalculoCotizacion.reporteRecalculoCotizacionDetalle = recalculoCotizacionDetalle;
                recalculoCotizacion.listaReporteRecalculoCotizacionBeneficiarios = lstReporteRecalculoCotizacionBeneficiarios;

            }

            return recalculoCotizacion;

        } //end class

        public List<ReporteCotizacionesGanadas> ObtenerReporteCotizacionesGanadas(int numLote)
        {
            List<ReporteCotizacionesGanadas> lstReporteCotizacionesGanadas = new List<ReporteCotizacionesGanadas>();
            ReporteCotizacionesGanadas reporteCotizacionesGanadas;
            List<ReporteCotizacionesGanadasBeneficiarios> lstReporteCotizacionesGanadasBeneficiarios = new List<ReporteCotizacionesGanadasBeneficiarios>();
            ReporteCotizacionesGanadasBeneficiarios reporteCotizacionesGanadasBeneficiarios;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            
            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_reporte_cotizaciones_ganadas", numLote))
            {
                while (dr.Read())
                {
                    reporteCotizacionesGanadas = new ReporteCotizacionesGanadas();

                    if (dr["num_operacion"] != DBNull.Value)
                        reporteCotizacionesGanadas.num_operacion = Convert.ToInt32(dr["num_operacion"].ToString());
                    if (dr["gls_afp"] != DBNull.Value)
                        reporteCotizacionesGanadas.gls_afp = dr["gls_afp"].ToString();
                    if (dr["num_cuspp"] != DBNull.Value)
                        reporteCotizacionesGanadas.num_cuspp = dr["num_cuspp"].ToString();
                    if (dr["afiliado"] != DBNull.Value)
                        reporteCotizacionesGanadas.afiliado = dr["afiliado"].ToString();
                    if (dr["gls_tipo_identificacion"] != DBNull.Value)
                        reporteCotizacionesGanadas.gls_tipo_identificacion = dr["gls_tipo_identificacion"].ToString();
                    if (dr["num_documento"] != DBNull.Value)
                        reporteCotizacionesGanadas.num_documento = dr["num_documento"].ToString();
                    if (dr["cod_genero_afiliado"] != DBNull.Value)
                        reporteCotizacionesGanadas.cod_genero_afiliado = dr["cod_genero_afiliado"].ToString();
                    if (dr["fec_nacimiento_afiliado"] != DBNull.Value)
                        reporteCotizacionesGanadas.fec_nacimiento_afiliado = Convert.ToDateTime(dr["fec_nacimiento_afiliado"].ToString());
                    if (dr["cod_Grado_invalidez"] != DBNull.Value)
                        reporteCotizacionesGanadas.cod_Grado_invalidez = dr["cod_Grado_invalidez"].ToString();
                    if (dr["cod_estado_sobrevivencia"] != DBNull.Value)
                        reporteCotizacionesGanadas.cod_estado_sobrevivencia = dr["cod_estado_sobrevivencia"].ToString();
                    if (dr["gls_tipo_pension_sbs"] != DBNull.Value)
                        reporteCotizacionesGanadas.gls_tipo_pension_sbs = dr["gls_tipo_pension_sbs"].ToString();
                    if (dr["cod_cambio_modalidad"] != DBNull.Value)
                        reporteCotizacionesGanadas.cod_cambio_modalidad = dr["cod_cambio_modalidad"].ToString();
                    if (dr["ind_pension_preliminar"] != DBNull.Value)
                        reporteCotizacionesGanadas.ind_pension_preliminar = dr["ind_pension_preliminar"].ToString();
                    if (dr["val_tasa_rp_rt"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_tasa_rp_rt = Convert.ToDouble(dr["val_tasa_rp_rt"].ToString());
                    if (dr["val_tipo_cambio"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_tipo_cambio = Convert.ToDouble(dr["val_tipo_cambio"].ToString());
                    if (dr["fec_envio"] != DBNull.Value)
                        reporteCotizacionesGanadas.fec_envio = Convert.ToDateTime(dr["fec_envio"].ToString());
                    if (dr["fec_devengue"] != DBNull.Value)
                        reporteCotizacionesGanadas.fec_devengue = Convert.ToDateTime(dr["fec_devengue"].ToString());
                    if (dr["fec_cierre"] != DBNull.Value)
                        reporteCotizacionesGanadas.fec_cierre = Convert.ToDateTime(dr["fec_cierre"].ToString());
                    if (dr["cod_moneda_fondo"] != DBNull.Value)
                        reporteCotizacionesGanadas.cod_moneda_fondo = dr["cod_moneda_fondo"].ToString();
                    if (dr["val_capital_pension"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_capital_pension = Convert.ToDouble(dr["val_capital_pension"].ToString());
                    if (dr["val_saldo_cic"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_saldo_cic = Convert.ToDouble(dr["val_saldo_cic"].ToString());
                    if (dr["val_cuota"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_cuota = Convert.ToDouble(dr["val_cuota"].ToString());
                    if (dr["val_saldo_cuota"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_saldo_cuota = Convert.ToDouble(dr["val_saldo_cuota"].ToString());
                    if (dr["val_bono_actualizado"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_bono_actualizado = Convert.ToDouble(dr["val_bono_actualizado"].ToString());
                    if (dr["ind_tiene_cobertura"] != DBNull.Value)
                        reporteCotizacionesGanadas.ind_tiene_cobertura = dr["ind_tiene_cobertura"].ToString();
                    if (dr["val_aporte_adicional"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_aporte_adicional = Convert.ToDouble(dr["val_aporte_adicional"].ToString());
                    if (dr["val_tipo_cambio_compra_AA"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_tipo_cambio_compra_AA = Convert.ToDouble(dr["val_tipo_cambio_compra_AA"].ToString());
                    if (dr["gls_compania"] != DBNull.Value)
                        reporteCotizacionesGanadas.gls_compania = dr["gls_compania"].ToString();
                    if (dr["gls_modalidad"] != DBNull.Value)
                        reporteCotizacionesGanadas.gls_modalidad = dr["gls_modalidad"].ToString();
                    if (dr["cod_moneda_producto"] != DBNull.Value)
                        reporteCotizacionesGanadas.cod_moneda_producto = dr["cod_moneda_producto"].ToString();
                    if (dr["num_anos_RT"] != DBNull.Value)
                        reporteCotizacionesGanadas.num_anos_RT = Convert.ToInt32(dr["num_anos_RT"].ToString());
                    if (dr["val_porcentaje_RVD"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_porcentaje_RVD = Convert.ToInt32(dr["val_porcentaje_RVD"].ToString());
                    if (dr["val_periodo_garantizado"] != DBNull.Value)
                        reporteCotizacionesGanadas.val_periodo_garantizado = Convert.ToInt32(dr["val_periodo_garantizado"].ToString());
                    if (dr["ind_derecho_crecer"] != DBNull.Value)
                        reporteCotizacionesGanadas.ind_derecho_crecer = dr["ind_derecho_crecer"].ToString();
                    if (dr["ind_gratificacion"] != DBNull.Value)
                        reporteCotizacionesGanadas.ind_gratificacion = dr["ind_gratificacion"].ToString();
                    if (dr["pje_cobertura_conyuge"] != DBNull.Value)
                        reporteCotizacionesGanadas.pje_cobertura_conyuge = Convert.ToDouble(dr["pje_cobertura_conyuge"].ToString());
                    if (dr["cod_particion_capital"] != DBNull.Value)
                        reporteCotizacionesGanadas.cod_particion_capital = dr["cod_particion_capital"].ToString();
                    if (dr["ajusteMoneda"] != DBNull.Value)
                        reporteCotizacionesGanadas.ajusteMoneda = dr["ajusteMoneda"].ToString();

                    reporteCotizacionesGanadas.reporteCotizacionesGanadasBeneficiarios = new List<ReporteCotizacionesGanadasBeneficiarios>();

                    lstReporteCotizacionesGanadas.Add(reporteCotizacionesGanadas);
                }
                
                dr.NextResult();
                
                while (dr.Read())
                {
                    reporteCotizacionesGanadasBeneficiarios = new ReporteCotizacionesGanadasBeneficiarios();

                    if (dr["num_operacion"] != DBNull.Value)
                        reporteCotizacionesGanadasBeneficiarios.num_operacion = Convert.ToInt32(dr["num_operacion"].ToString());
                    if (dr["beneficiario"] != DBNull.Value)
                        reporteCotizacionesGanadasBeneficiarios.beneficiario = dr["beneficiario"].ToString();
                    if (dr["gls_parentezco"] != DBNull.Value)
                        reporteCotizacionesGanadasBeneficiarios.gls_parentezco = dr["gls_parentezco"].ToString();
                    if (dr["cod_condicion_invalidez"] != DBNull.Value)
                        reporteCotizacionesGanadasBeneficiarios.cod_condicion_invalidez = dr["cod_condicion_invalidez"].ToString();
                    if (dr["fec_nacimiento_beneficiario"] != DBNull.Value)
                        reporteCotizacionesGanadasBeneficiarios.fec_nacimiento_beneficiario = Convert.ToDateTime(dr["fec_nacimiento_beneficiario"].ToString());
                    if (dr["cod_genero_beneficiario"] != DBNull.Value)
                        reporteCotizacionesGanadasBeneficiarios.cod_genero_beneficiario = dr["cod_genero_beneficiario"].ToString();

                    lstReporteCotizacionesGanadas.FindAll(cg => (cg.num_operacion == Convert.ToInt32(dr["num_operacion"].ToString()))).ForEach(g => g.reporteCotizacionesGanadasBeneficiarios.Add(reporteCotizacionesGanadasBeneficiarios));
                    
                }
                
            }

            return lstReporteCotizacionesGanadas;

        } //end class
        
		
		public DataSet ListarReporteIndicadoresCDA(DateTime fecPeriodo, string codUsername, string codRol)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_reporte_indicadores_cda");

                db.AddInParameter(dbc, "@wl_fec_periodo", DbType.String, fecPeriodo.ToString("yyyyMMdd"));
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, codUsername);
                db.AddInParameter(dbc, "@wl_cod_rol", DbType.String, codRol);

                var data = db.ExecuteDataSet(dbc);

                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet ActualizarReporteIndicadoresCDA(DataSet dataReporte, List<ResponseVTiger> lstEstadoCRM)
        {
            try
            {
                var dt = dataReporte.Tables[0];
                var dataMatched = from row in dt.AsEnumerable()
                                join lst in lstEstadoCRM
                                on row.Field<string>("Cuspp") equals lst.numCuspp.ToString()
                                select new { DataRow = row, EstadoCRM = lst.subestado };

                foreach (var item in dataMatched)
                {
                    item.DataRow["EstadoCRM"] = item.EstadoCRM;
                }

                return dataReporte;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
