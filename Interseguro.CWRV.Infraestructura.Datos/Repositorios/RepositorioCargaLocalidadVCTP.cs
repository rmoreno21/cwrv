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
    public class RepositorioCargaLocalidadVCTP : IRepositorioCargaLocalidadVCTP
    {

        public bool Validar(DateTime fecPeriodo, string codUsuario)
        {
            bool existe = false;
            int valida = 0;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_consultar_carga_localidad_vctp");

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

        public void Registrar(CargaLocalidadVCTP entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_carga_localidad_vctp");

                db.AddInParameter(dbc, "@wl_fec_periodo", DbType.String, entity.fec_periodo.ToString("yyyyMMdd"));
                db.AddInParameter(dbc, "@wl_cod_supervisor", DbType.Int32, entity.cod_supervisor);
                db.AddInParameter(dbc, "@wl_nom_supervisor", DbType.String, entity.nom_supervisor);
                db.AddInParameter(dbc, "@wl_cod_jefe", DbType.Int32, entity.cod_jefe);
                db.AddInParameter(dbc, "@wl_nom_jefe", DbType.String, entity.nom_jefe);
                db.AddInParameter(dbc, "@wl_gls_localidad", DbType.String, entity.gls_localidad);
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, entity.aud_usr_ingreso);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(CargaLocalidadVCTP entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(CargaLocalidadVCTP entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_carga_localidad_vctp");

                db.AddInParameter(dbc, "@wl_fec_periodo", DbType.String, entity.fec_periodo.ToString("yyyyMMdd"));
                db.AddInParameter(dbc, "@wl_aud_usr_ingreso", DbType.String, entity.aud_usr_ingreso);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public CargaLocalidadVCTP ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public CargaLocalidadVCTP ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<CargaLocalidadVCTP> Listar()
        {
            throw new NotImplementedException();
        }

    }
}
