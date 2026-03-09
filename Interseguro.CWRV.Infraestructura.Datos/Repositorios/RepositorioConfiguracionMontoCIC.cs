//<SRIINI06326>
using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioConfiguracionMontoCIC: IRepositorioConfiguracionMontoCIC
    {

        public List<MontoCIC> Listar()
        {
            List<MontoCIC> listaMontosCIC = new List<MontoCIC>();
            MontoCIC montoCIC;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_configuracion_monto_cic"))
            {
                while (dr.Read())
                {
                    montoCIC = new MontoCIC ();
                    montoCIC.Id = Convert.ToInt32( dr["cod_configuracion"]);
                    montoCIC.Valor  =Convert.ToDecimal(dr["valor"]);

                    listaMontosCIC.Add(montoCIC);
                }
            }

            return listaMontosCIC;
        }

        public void Registrar(MontoCIC entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_configuracion_monto_cic");

                db.AddInParameter(dbc, "@wl_valor", DbType.Double, entity.Valor);
                db.AddInParameter(dbc, "@wl_usr_ingreso", DbType.String, entity.UsuarioCreacion);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(MontoCIC entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_configuracion_monto_cic");

                db.AddInParameter(dbc, "@wl_codigo", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_valor", DbType.Double, entity.Valor);
                db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.UsuarioModificacion);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(MontoCIC entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_configuracion_monto_cic");

                db.AddInParameter(dbc, "@wl_codigo", DbType.Int32, entity.Id);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MontoCIC ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public MontoCIC ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
//<SRIFIN06326>