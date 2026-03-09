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
    public class RepositorioLogBD : IRepositorioLogBD
    {
        public void Registrar(LogBD entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_log");

                db.AddInParameter(dbc, "@wl_cod_aplicacion", DbType.String, entity.IdAplicacion);
                db.AddInParameter(dbc, "@wl_nombre_terminal", DbType.String, entity.NombreTerminal);
                db.AddInParameter(dbc, "@wl_direccion_ip", DbType.String, entity.IP);
                db.AddInParameter(dbc, "@wl_cod_username", DbType.String, entity.NombreUsuario);
                db.AddInParameter(dbc, "@wl_cod_tipo_evento", DbType.String, entity.IdTipoEvento);
                db.AddInParameter(dbc, "@wl_gls_detalle", DbType.String, entity.Detalle);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(LogBD entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(LogBD entity)
        {
            throw new NotImplementedException();
        }

        public LogBD ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public LogBD ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<LogBD> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
