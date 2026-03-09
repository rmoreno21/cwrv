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

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioRolDtra : IRepositorioRolDtra
    {

        public List<RolDtra> ListarRolDtra(RolDtra rolDtra)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            var roles = new List<RolDtra>();

            DbCommand dbc = db.GetStoredProcCommand("dbo.SP_RENVI_SEL_DTRA_ROL");

            db.AddInParameter(dbc, "@wl_codigo_rol", DbType.String, rolDtra.RolAzman);
            db.AddInParameter(dbc, "@wl_fec_cierre", DbType.DateTime, rolDtra.FechaCotizacion);

            dbc.CommandTimeout = 0;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    RolDtra rol = new RolDtra();

                    if (dr["num_rango_ini"] != DBNull.Value)
                        rol.RangoInicial = Convert.ToDouble(dr["num_rango_ini"]);
                    if (dr["num_rango_fin"] != DBNull.Value)
                        rol.RangoFinal = Convert.ToDouble(dr["num_rango_fin"]);

                    roles.Add(rol);
                }
            }

            return roles;
        }

        public void Registrar(RolDtra entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(RolDtra entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(RolDtra entity)
        {
            throw new NotImplementedException();
        }

        public RolDtra ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public RolDtra ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<RolDtra> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
