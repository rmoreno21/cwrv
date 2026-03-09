using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioSolicitudAcceso: IRepositorioSolicitudAcceso
    {
        public SolicitudAcceso ValidarToken(string token)
        {
            SolicitudAcceso solicitud = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            // Obtener los datos de la solicitud
            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_solicitud_acceso", token))
            {
                if (dr.Read())
                {
                    solicitud = new SolicitudAcceso();
                    solicitud.Codigo = Convert.ToInt64(dr["id_solicitud_acceso"]);
                    solicitud.Usuario = dr["cod_username"].ToString();
                    solicitud.CUSPP = dr["num_cuspp"].ToString();
                    solicitud.Token = dr["gls_token"].ToString();
                    if (dr["fec_consulta"] != DBNull.Value)
                        solicitud.FechaConsulta = Convert.ToDateTime(dr["fec_consulta"]);
                    if (dr["fec_expiracion"] != DBNull.Value)
                        solicitud.FechaExpiracion = Convert.ToDateTime(dr["fec_expiracion"]);
                    solicitud.Vigente = (dr["ind_vigencia"].ToString() == "S" ? true : false);
                    solicitud.IP = dr["num_ip"].ToString();

                    // Validar si el token ya expiró
                    if (solicitud.Vigente && solicitud.FechaConsulta > solicitud.FechaExpiracion)
                    {
                        // Destruir el token de acceso para que no pueda volver a ser usado
                        solicitud.Vigente = false;
                        Actualizar(solicitud);
                    }
                }
                return solicitud;
            }
        }

        public void Registrar(SolicitudAcceso entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_solicitud_acceso");

                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, entity.Usuario);
                db.AddInParameter(dbc, "@wl_num_cuspp", DbType.String, entity.CUSPP);
                db.AddInParameter(dbc, "@wl_gls_token", DbType.String, entity.Token);
                db.AddInParameter(dbc, "@wl_num_segundos_token", DbType.Int32, entity.ExpiracionToken);
                db.AddInParameter(dbc, "@wl_num_ip", DbType.String, entity.IP);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(SolicitudAcceso entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_solicitud_acceso");

                db.AddInParameter(dbc, "@id_solicitud_acceso", DbType.String, entity.Codigo);
                db.AddInParameter(dbc, "@num_cuspp", DbType.String, entity.CUSPP);
                db.AddInParameter(dbc, "@gls_token", DbType.String, entity.Token);
                db.AddInParameter(dbc, "@fec_expiracion", DbType.DateTime, entity.FechaExpiracion);
                db.AddInParameter(dbc, "@ind_vigencia", DbType.String, (entity.Vigente ? "S" : "N"));
                db.AddInParameter(dbc, "@num_ip", DbType.String, entity.IP);
                db.AddInParameter(dbc, "@cod_username", DbType.String, entity.Usuario);
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(SolicitudAcceso entity)
        {
            throw new NotImplementedException();
        }

        public SolicitudAcceso ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public SolicitudAcceso ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<SolicitudAcceso> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
