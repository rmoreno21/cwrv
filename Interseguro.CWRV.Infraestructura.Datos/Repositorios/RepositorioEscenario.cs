//<SRIINI06326>  
using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data.SqlClient;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioEscenario: IRepositorioEscenario
    {
        public void Registrar(string xmlData, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.SP_RENVI_INSERT_RPTESCENARIO");

                db.AddInParameter(dbc, "@wl_empdata", DbType.String, xmlData);
                db.AddInParameter(dbc, "@wl_aud_cod_username", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(string idSolicitud, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_reporte_escenario");

                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, idSolicitud);
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataSet ObtenerDatosReporte(string idSolicitud, string usuario) 
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_sel_rptescenario", idSolicitud,usuario))
            {
                var data = db.ExecuteDataSet(dbc);

                return data;
            }
        }
    }
}
//<SRIFIN06326>