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
    public class RepositorioCargaControlVCTP : IRepositorioCargaControlVCTP
    {

        public bool Validar(DateTime fecPeriodo, string codUsuario)
        {
            bool existe = false;
            int valida = 0;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_consultar_carga_control_vctp");

            db.AddInParameter(dbc, "@wl_fec_periodo", DbType.String, fecPeriodo.ToString("yyyyMMdd"));
            db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, codUsuario);

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                if (dr.Read())
                {
                    valida = Convert.ToInt32(dr["valida"].ToString());
                }
            }

            if (valida > 0) existe = true;

            return existe;
        }

        public void Registrar(CargaControlVCTP entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_carga_control_vctp");

                db.AddInParameter(dbc, "@wl_fec_periodo", DbType.String, entity.fec_periodo.ToString("yyyyMMdd"));
                db.AddInParameter(dbc, "@wl_num_cuspp", DbType.String, entity.num_cuspp);
                db.AddInParameter(dbc, "@wl_ind_medicion", DbType.String, entity.ind_medicion);
                db.AddInParameter(dbc, "@wl_gls_comentario", DbType.String, entity.gls_comentario);
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, entity.aud_usr_ingreso);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(CargaControlVCTP entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(CargaControlVCTP entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_carga_control_vctp");

                db.AddInParameter(dbc, "@wl_fec_periodo", DbType.String, entity.fec_periodo.ToString("yyyyMMdd"));
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, entity.aud_usr_ingreso);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CargaControlVCTP ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public CargaControlVCTP ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<CargaControlVCTP> Listar()
        {
            throw new NotImplementedException();
        }

    }
}
