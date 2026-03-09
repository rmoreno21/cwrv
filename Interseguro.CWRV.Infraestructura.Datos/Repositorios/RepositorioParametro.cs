using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;


namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioParametro : IRepositorioParametro
    {

        public List<List<Parametro>> ObtenerCombobox()
        {
            List<List<Parametro>> parametros = new List<List<Parametro>>();
            List<Parametro> combobox;
            Parametro itemCombobox;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_parametros"))
            {
                bool hayInformacion = true;
                while (hayInformacion)
                {
                    combobox = new List<Parametro>();
                    while (dr.Read())
                    {
                        itemCombobox = new Parametro();
                        itemCombobox.Id = dr[0].ToString();
                        itemCombobox.Glosa = dr[1].ToString();
                        combobox.Add(itemCombobox);
                    }
                    parametros.Add(combobox);
                    hayInformacion = dr.NextResult();
                }
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_departamento"))
            {
                combobox = new List<Parametro>();
                while (dr.Read())
                {
                    itemCombobox = new Parametro();
                    itemCombobox.Id = dr[0].ToString();
                    itemCombobox.Glosa = dr[1].ToString();
                    combobox.Add(itemCombobox);
                }
                parametros.Add(combobox);
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_periodo_temporal"))
            {
                combobox = new List<Parametro>();
                while (dr.Read())
                {
                    itemCombobox = new Parametro();
                    itemCombobox.Id = dr[0].ToString();
                    itemCombobox.Glosa = dr[1].ToString();
                    itemCombobox.Valor_1 = dr[2].ToString();
                    itemCombobox.Valor_2 = dr[3].ToString();
                    combobox.Add(itemCombobox);
                }
                parametros.Add(combobox);
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_periodo_pago_doble"))
            {
                combobox = new List<Parametro>();
                while (dr.Read())
                {
                    itemCombobox = new Parametro();
                    itemCombobox.Id = dr[0].ToString();
                    itemCombobox.Glosa = dr[1].ToString();
                    itemCombobox.Valor_1 = dr[2].ToString();
                    itemCombobox.Valor_2 = dr[3].ToString();
                    combobox.Add(itemCombobox);
                }
                parametros.Add(combobox);
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_moneda_ajuste_plus"))
            {
                combobox = new List<Parametro>();
                while (dr.Read())
                {
                    itemCombobox = new Parametro();
                    itemCombobox.Id = dr[0].ToString();
                    itemCombobox.Glosa = dr[1].ToString();
                    itemCombobox.Valor_1 = dr[2].ToString();
                    itemCombobox.Valor_2 = dr[3].ToString();
                    combobox.Add(itemCombobox);
                }
                parametros.Add(combobox);
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_porcentaje_escalonado_plus"))
            {
                combobox = new List<Parametro>();

                DateTime fecIni;
                DateTime fecFin;

                while (dr.Read())
                {
                    itemCombobox = new Parametro();
                    itemCombobox.Id = dr[0].ToString();
                    itemCombobox.Glosa = dr[1].ToString();
                    itemCombobox.Valor_1 = dr[2].ToString();
                    itemCombobox.Valor_2 = dr[3].ToString();

                    DateTime.TryParse(dr[4].ToString(), out fecIni);
                    DateTime.TryParse(dr[5].ToString(), out fecFin);
                    itemCombobox.FecInicioVigencia = fecIni;
                    itemCombobox.FecFinVigencia = fecFin;

                    combobox.Add(itemCombobox);
                }
                parametros.Add(combobox);
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_parentesco_persona_vinculada"))
            {
                combobox = new List<Parametro>();
                while (dr.Read())
                {
                    itemCombobox = new Parametro();
                    itemCombobox.Id = dr[0].ToString();
                    itemCombobox.Glosa = dr[1].ToString();
                    combobox.Add(itemCombobox);
                }
                parametros.Add(combobox);
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_productos"))
            {
                combobox = new List<Parametro>();
                while (dr.Read())
                {
                    itemCombobox = new Parametro();
                    itemCombobox.Id = dr[0].ToString();
                    itemCombobox.Glosa = dr[1].ToString();
                    combobox.Add(itemCombobox);
                }
                parametros.Add(combobox);
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_origen_venta"))
            {
                combobox = new List<Parametro>();
                while (dr.Read())
                {
                    itemCombobox = new Parametro();
                    itemCombobox.Id = dr[0].ToString();
                    itemCombobox.Glosa = dr[1].ToString();
                    combobox.Add(itemCombobox);
                }
                parametros.Add(combobox);
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_parametros_rentas"))
            {
                combobox = new List<Parametro>();
                while (dr.Read())
                {
                    itemCombobox = new Parametro();
                    itemCombobox.Id = dr[0].ToString();
                    itemCombobox.Glosa = dr[1].ToString();
                    combobox.Add(itemCombobox);
                }
                parametros.Add(combobox);
            }

            return parametros;
        }

        public List<Parametro> ObtenerParametrosSimuladores()
        {
            List<Parametro> parametros = new List<Parametro>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_parametros_simulador"))
            {
                bool hayInformacion = true;
                while (hayInformacion)
                {
                    while (dr.Read())
                    {
                        if (dr[0] != DBNull.Value)
                            parametros.Add(new Parametro { Valor = Convert.ToDouble(dr[0]) });
                        else
                            parametros.Add(new Parametro { Valor = 0 });
                    }
                    hayInformacion = dr.NextResult();
                }
            }

            return parametros;
        }

        public List<Parametro> ObtenerParametrosPorTabla(string codTabla)
        {
            List<Parametro> listaParametro = new List<Parametro>();
            Parametro parametro;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_lis_parametro_por_tabla", codTabla))
            {
                while (dr.Read())
                {
                    parametro = new Parametro();
                    parametro.Id = dr["cod_tabla"].ToString();
                    parametro.Nombre = dr["cod_parametro"].ToString();
                    parametro.Correlativo = Convert.ToInt32(dr["correlativo"]);
                    if (dr["valor_1"] != DBNull.Value && dr["valor_1"].ToString() != string.Empty)
                        parametro.Valor_1 = dr["valor_1"].ToString();
                    if (dr["valor_2"] != DBNull.Value && dr["valor_2"].ToString() != string.Empty)
                        parametro.Valor_2 = dr["valor_2"].ToString();
                    if (dr["gls_parametro"] != DBNull.Value && dr["gls_parametro"].ToString() != string.Empty)
                        parametro.Glosa = dr["gls_parametro"].ToString();

                    listaParametro.Add(parametro);
                }
            }

            return listaParametro;
        }

        public void Registrar(Parametro entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Parametro entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_configuracion_aumento_pension");

                db.AddInParameter(dbc, "@wl_tabla", DbType.String, entity.Id);
                db.AddInParameter(dbc, "@wl_correlativo", DbType.Int32, entity.Correlativo);
                db.AddInParameter(dbc, "@wl_valor1", DbType.String, entity.Valor_1);
                db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.UsuarioModificacion);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Eliminar(Parametro entity)
        {
            throw new NotImplementedException();
        }

        public Parametro ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Parametro ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Parametro> Listar()
        {
            throw new NotImplementedException();
        }

        public List<ParametroEspecial> ObtenerTraDefault(string idSolicitud)
        {
            List<ParametroEspecial> parametros = new List<ParametroEspecial>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.SP_RENVI_AJUTRA_EJE_PAR_ESP", idSolicitud))
            {
                while (dr.Read())
                {
                    ParametroEspecial parametroEspecial = new ParametroEspecial();

                    if (dr["cod_parametro"] != DBNull.Value)
                        parametroEspecial.Id = dr["cod_parametro"].ToString();

                    if (dr["cod_moneda"] != DBNull.Value && dr["moneda"] != DBNull.Value)
                        parametroEspecial.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["moneda"].ToString() };

                    if (dr["fec_ini_rango"] != DBNull.Value)
                        parametroEspecial.FecIniRango = Convert.ToDateTime(dr["fec_ini_rango"]);

                    if (dr["fec_fin_rango"] != DBNull.Value)
                        parametroEspecial.FecFinRango = Convert.ToDateTime(dr["fec_fin_rango"]);

                    if (dr["val_parametro"] != DBNull.Value)
                        parametroEspecial.ValParametro = Convert.ToDouble(dr["val_parametro"]);

                    if (dr["val_ajutra"] != DBNull.Value)
                        parametroEspecial.ValAjuTra = Convert.ToDouble(dr["val_ajutra"]);

                    if (dr["val_diferencia"] != DBNull.Value)
                        parametroEspecial.ValDiferencia = Convert.ToDouble(dr["val_diferencia"]);

                    parametros.Add(parametroEspecial);
                }
            }

            return parametros;
        }

        public List<ParametroEspecial> ObtenerTasaMaximaTraMinima(string idSolicitud, DateTime fecCotizacion)
        {
            List<ParametroEspecial> parametros = new List<ParametroEspecial>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            //HTVA
            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_listar_rvi_cotiza_valpar", idSolicitud, "HTVA"))
            {
                while (dr.Read())
                {
                    ParametroEspecial parametroEspecial = new ParametroEspecial();

                    if (dr["cod_parametro"] != DBNull.Value)
                        parametroEspecial.Id = dr["cod_parametro"].ToString();

                    if (dr["cod_moneda"] != DBNull.Value && dr["gls_moneda"] != DBNull.Value)
                        parametroEspecial.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString() };

                    if (dr["fec_ini_rango"] != DBNull.Value)
                        parametroEspecial.FecIniRango = Convert.ToDateTime(dr["fec_ini_rango"]);

                    if (dr["fec_fin_rango"] != DBNull.Value)
                        parametroEspecial.FecFinRango = Convert.ToDateTime(dr["fec_fin_rango"]);

                    if (dr["val_parametro"] != DBNull.Value)
                        parametroEspecial.ValParametro = Convert.ToDouble(dr["val_parametro"]);

                    if (dr["val_diferencia"] != DBNull.Value)
                        parametroEspecial.ValDiferencia = Convert.ToDouble(dr["val_diferencia"]);

                    parametroEspecial.NumSolicitud = idSolicitud;

                    parametros.Add(parametroEspecial);
                }
            }

            //LTRA
            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_listar_rvi_cotiza_valpar", idSolicitud, "LTRA"))
            {
                while (dr.Read())
                {
                    ParametroEspecial parametroEspecial = new ParametroEspecial();

                    if (dr["cod_parametro"] != DBNull.Value)
                        parametroEspecial.Id = dr["cod_parametro"].ToString();

                    if (dr["cod_moneda"] != DBNull.Value && dr["gls_moneda"] != DBNull.Value)
                        parametroEspecial.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString() };

                    if (dr["fec_ini_rango"] != DBNull.Value)
                        parametroEspecial.FecIniRango = Convert.ToDateTime(dr["fec_ini_rango"]);

                    if (dr["fec_fin_rango"] != DBNull.Value)
                        parametroEspecial.FecFinRango = Convert.ToDateTime(dr["fec_fin_rango"]);

                    if (dr["val_parametro"] != DBNull.Value)
                        parametroEspecial.ValParametro = Convert.ToDouble(dr["val_parametro"]);

                    if (dr["val_diferencia"] != DBNull.Value)
                        parametroEspecial.ValDiferencia = Convert.ToDouble(dr["val_diferencia"]);

                    parametroEspecial.NumSolicitud = idSolicitud;

                    parametros.Add(parametroEspecial);
                }
            }

            //Por primera Vez
            if (parametros.Count == 0)
            {
                //HTVA
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_listar_rvi_valpar", "HTVA", fecCotizacion))
                {
                    while (dr.Read())
                    {
                        ParametroEspecial parametroEspecial = new ParametroEspecial();

                        if (dr["cod_parametro"] != DBNull.Value)
                            parametroEspecial.Id = dr["cod_parametro"].ToString();

                        if (dr["cod_moneda"] != DBNull.Value && dr["gls_moneda"] != DBNull.Value)
                            parametroEspecial.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString() };

                        if (dr["fec_ini_rango"] != DBNull.Value)
                            parametroEspecial.FecIniRango = Convert.ToDateTime(dr["fec_ini_rango"]);

                        if (dr["fec_fin_rango"] != DBNull.Value)
                            parametroEspecial.FecFinRango = Convert.ToDateTime(dr["fec_fin_rango"]);

                        if (dr["val_parametro"] != DBNull.Value)
                            parametroEspecial.ValParametro = Convert.ToDouble(dr["val_parametro"]);

                        parametroEspecial.ValDiferencia = 0;
                        parametroEspecial.NumSolicitud = idSolicitud;

                        parametros.Add(parametroEspecial);
                    }
                }

                //LTRA
                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_listar_rvi_valpar", "LTRA", fecCotizacion))
                {
                    while (dr.Read())
                    {
                        ParametroEspecial parametroEspecial = new ParametroEspecial();

                        if (dr["cod_parametro"] != DBNull.Value)
                            parametroEspecial.Id = dr["cod_parametro"].ToString();

                        if (dr["cod_moneda"] != DBNull.Value && dr["gls_moneda"] != DBNull.Value)
                            parametroEspecial.Moneda = new Moneda { Id = dr["cod_moneda"].ToString(), Nombre = dr["gls_moneda"].ToString() };

                        if (dr["fec_ini_rango"] != DBNull.Value)
                            parametroEspecial.FecIniRango = Convert.ToDateTime(dr["fec_ini_rango"]);

                        if (dr["fec_fin_rango"] != DBNull.Value)
                            parametroEspecial.FecFinRango = Convert.ToDateTime(dr["fec_fin_rango"]);

                        if (dr["val_parametro"] != DBNull.Value)
                            parametroEspecial.ValParametro = Convert.ToDouble(dr["val_parametro"]);

                        parametroEspecial.ValDiferencia = 0;
                        parametroEspecial.NumSolicitud = idSolicitud;

                        parametros.Add(parametroEspecial);
                    }
                }
            }
            return parametros;
        }

        public void Registrar(List<ParametroEspecial> parametros, string idUsuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_rvi_cotiza_valpar");
                ParametroEspecial parametroEspecial = new ParametroEspecial();
                string xml = parametroEspecial.XMLParametro(parametros);
                db.AddInParameter(dbc, "@xml_rvi_cotiza_valpar", DbType.String, xml);
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, idUsuario);
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Parametro> ObtenerParametros(string tabla)
        {
            List<Parametro> parametros = new List<Parametro>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_parametros", tabla))
            {

                while (dr.Read())
                {
                    Parametro parametro = new Parametro();

                    parametro.Id = dr["cod_parametro"].ToString();
                    parametro.Nombre = dr["gls_parametro"].ToString();
                    parametro.Valor_1 = dr["Valor_1"].ToString();
                    parametro.Valor_2 = dr["Valor_2"].ToString();

                    parametros.Add(parametro);

                }

            }

            return parametros;
        }

        public List<Parametro> ObtenerNroBancos(string tabla, string tipoBanco, string tipoCuenta)
        {
            List<Parametro> parametros = new List<Parametro>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_banco_mascara", tabla, tipoCuenta))
            {

                while (dr.Read())
                {

                    if (dr["cod_parametro"].ToString() == tipoBanco)
                    {

                        Parametro parametro = new Parametro();

                        parametro.Id = dr["cod_parametro"].ToString();
                        parametro.Nombre = dr["gls_parametro"].ToString();
                        parametro.Valor_2 = dr["Valor_2"].ToString();

                        parametros.Add(parametro);

                    }

                }

            }

            return parametros;
        }

        public List<Parametro> ObtenerTipoCtaBancos(string tipoBanco, string id)
        {
            List<Parametro> parametros = new List<Parametro>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            if (id.ToString().Length > 0)
            {
                id = id;
            }
            else
            {
                id = null;
            }

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_tipo_cuenta_banco", tipoBanco, id))
            {

                while (dr.Read())
                {

                    Parametro parametro = new Parametro();

                    parametro.Id = dr["cod_tipo_cta_banco"].ToString();
                    parametro.Nombre = dr["gls_larga_tipo_cta_banco"].ToString();
                    parametro.Valor_2 = dr["gls_formato"].ToString();

                    parametros.Add(parametro);

                }

            }

            return parametros;
        }

        public List<Parametro> ObtenerTipoIdentificacion(string cod_tipo_identificacion, string gls_tipo_identificacion, string gls_corta_identificacion)
        {
            List<Parametro> listaParametro = new List<Parametro>();
            Parametro parametro;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_tipo_identificacion", cod_tipo_identificacion, gls_tipo_identificacion, gls_corta_identificacion))
            {
                while (dr.Read())
                {
                    parametro = new Parametro();
                    parametro.Id = dr["cod_tipo_identificacion"].ToString();
                    parametro.Glosa = dr["gls_tipo_identificacion"].ToString();
                    parametro.Nombre = dr["gls_corta_identificacion"].ToString();
                    listaParametro.Add(parametro);
                }
            }

            return listaParametro;
        }

        public List<List<Parametro>> ObtenerComboboxIFP()
        {
            List<List<Parametro>> parametros = new List<List<Parametro>>();
            List<Parametro> combobox;
            Parametro itemCombobox;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_parametros_IFP"))
            {
                DateTime fecIni;
                DateTime fecFin;

                bool hayInformacion = true;
                while (hayInformacion)
                {
                    combobox = new List<Parametro>();
                    while (dr.Read())
                    {
                        itemCombobox = new Parametro();
                        itemCombobox.Id = dr[0].ToString();
                        itemCombobox.Glosa = dr[1].ToString();
                        itemCombobox.Valor_1 = dr[2].ToString();
                        itemCombobox.Valor_2 = dr[3].ToString();
                        if (dr.FieldCount >= 5)
                        {
                            DateTime.TryParse(dr[4].ToString(), out fecIni);
                            DateTime.TryParse(dr[5].ToString(), out fecFin);
                            itemCombobox.FecInicioVigencia = fecIni;
                            itemCombobox.FecFinVigencia = fecFin;
                        }

                        combobox.Add(itemCombobox);
                    }
                    parametros.Add(combobox);
                    hayInformacion = dr.NextResult();
                }
            }

            return parametros;
        }

        public List<Contrato> ListarContratosCotizaciones(string codUserName)
        {
            List<Contrato> listaParametro = new List<Contrato>();
            Contrato parametro;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_rvi_contrato_cotizacion", codUserName))
            {
                while (dr.Read())
                {
                    parametro = new Contrato();
                    parametro.IdContratoCotizacion = Convert.ToInt32(dr["id_contrato_cotizacion"].ToString());
                    parametro.GlsContratoCotizacion = dr["gls_contrato_cotizacion"].ToString();
                    parametro.FecInicio = Convert.ToDateTime(dr["fec_ini_contrato"].ToString());
                    parametro.FecFin = Convert.ToDateTime(dr["fec_fin_contrato"].ToString());
                    parametro.CodAfp = dr["cod_afp"].ToString();
                    parametro.GlsAfp = dr["gls_afp"].ToString();
                    listaParametro.Add(parametro);
                }
            }

            return listaParametro;
        }

        public void ActualizarContratoCotizacion(Contrato entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_rvi_contrato_cotizacion");

                db.AddInParameter(dbc, "@wl_id_contrato_cotizacion", DbType.Int32, entity.IdContratoCotizacion);
                db.AddInParameter(dbc, "@wl_gls_contrato_cotizacion", DbType.String, entity.GlsContratoCotizacion);
                db.AddInParameter(dbc, "@wl_fec_ini_contrato", DbType.DateTime, entity.FecInicio);
                db.AddInParameter(dbc, "@wl_fec_fin_contrato", DbType.DateTime, entity.FecFin);
                db.AddInParameter(dbc, "@wl_cod_afp", DbType.String, entity.CodAfp);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, entity.UsuarioModificacion);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void InsertarContratoCotizacion(Contrato entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_rvi_contrato_cotizacion");

                db.AddInParameter(dbc, "@wl_gls_contrato_cotizacion", DbType.String, entity.GlsContratoCotizacion);
                db.AddInParameter(dbc, "@wl_fec_ini_contrato", DbType.DateTime, entity.FecInicio);
                db.AddInParameter(dbc, "@wl_fec_fin_contrato", DbType.DateTime, entity.FecFin);
                db.AddInParameter(dbc, "@wl_cod_afp", DbType.String, entity.CodAfp);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, entity.UsuarioCreacion);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EliminarContratoCotizacion(int idContrato, string codUserName)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_rvi_contrato_cotizacion");

                db.AddInParameter(dbc, "@wl_id_contrato_cotizacion", DbType.String, idContrato);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, codUserName);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
