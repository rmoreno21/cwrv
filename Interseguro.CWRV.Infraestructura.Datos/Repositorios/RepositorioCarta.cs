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
    public class RepositorioCarta : IRepositorioCarta
    {
        public RviCarta ObtenerDatos(string solicitud, int correlativo, string usuario)
        {
            try
            {
                RviCarta carta = null;
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_carta", solicitud, correlativo, usuario))
                {
                    if (dr.Read())
                    {
                        carta = new RviCarta();
                        carta.num_solicitud = dr["num_solicitud"].ToString();
                        carta.num_correlativo = Convert.ToInt32(dr["num_correlativo"]);
                        if (dr["cod_carta"] != DBNull.Value)
                            carta.cod_carta = dr["cod_carta"].ToString();
                        if (dr["cod_carta"] != DBNull.Value)
                            carta.ind_emitida = dr["ind_emitida"].ToString();
                        if (dr["gls_documento"] != DBNull.Value)
                            carta.gls_documento = dr["gls_documento"].ToString();
                        if (dr["fec_registro"] != DBNull.Value)
                            carta.fec_registro = Convert.ToDateTime(dr["fec_registro"]);
                        if (dr["fec_emision"] != DBNull.Value)
                            carta.fec_emision = Convert.ToDateTime(dr["fec_emision"]);
                        if (dr["num_lote_emision"] != DBNull.Value)
                            carta.num_lote_emision = Convert.ToInt32(dr["num_lote_emision"]);
                        if (dr["aud_usr_ingreso"] != DBNull.Value)
                            carta.aud_usr_ingreso = dr["aud_usr_ingreso"].ToString();
                        if (dr["aud_fec_ingreso"] != DBNull.Value)
                            carta.aud_fec_ingreso = Convert.ToDateTime(dr["aud_fec_ingreso"]);
                        if (dr["aud_usr_modificacion"] != DBNull.Value)
                            carta.aud_usr_modificacion = dr["aud_usr_modificacion"].ToString();
                        if (dr["aud_fec_modificacion"] != DBNull.Value)
                            carta.aud_fec_modificacion = Convert.ToDateTime(dr["aud_fec_modificacion"]);
                    }
                    return carta;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Registrar(RviCarta entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_carta");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, entity.num_solicitud);
                db.AddInParameter(dbc, "@wl_num_correlativo", DbType.Int32, entity.num_correlativo);
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, entity.aud_usr_ingreso);
              
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(RviCarta entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(RviCarta entity)
        {
            throw new NotImplementedException();
        }

        public RviCarta ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public RviCarta ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<RviCarta> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
