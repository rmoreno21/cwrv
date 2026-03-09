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
    public class RepositorioRolAcom : IRepositorioRolAcom
    {
        //<SRIINI10693>
            //Se creo el repositorio ROLACOM para su implementacion
        //<SRIFIN10693>

        public void Registrar(RolAcom entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(RolAcom entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(RolAcom entity)
        {
            throw new NotImplementedException();
        }

        public RolAcom ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public RolAcom ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<RolAcom> Listar()
        {
            throw new NotImplementedException();
        }

        public List<RolAcom> ListarRolAcom(RolAcom rolAcom)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            //var fechaPeriodoTexto = periodo.ToString("yyyyMMdd");
            var roles = new List<RolAcom>();

            //<SRIINI15069>
            var fecCotizacion = rolAcom.FechaCotizacion.ToString("yyyyMMdd");
            //<SRIFIN15069>

            DbCommand dbc = db.GetStoredProcCommand("dbo.SP_RENVI_SEL_ACOM_ROL");

            db.AddInParameter(dbc, "@wl_codigo_rol", DbType.String, Convert.ToString(rolAcom.CodRol));
            //<SRIINI15069>
            db.AddInParameter(dbc, "@wl_fec_cierre", DbType.String, Convert.ToString(fecCotizacion));
            //<SRIFIN15069>

            dbc.CommandTimeout = 0;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    var rol = new RolAcom();

                    if (dr["num_rango_ini"] != DBNull.Value)
                        rol.NumRangoIni = Convert.ToDouble(dr["num_rango_ini"]);
                    if (dr["num_rango_fin"] != DBNull.Value)
                        rol.NumRangoFin = Convert.ToDouble(dr["num_rango_fin"]);

                    roles.Add(rol);
                }
            }

            return roles;
        }

        public List<RolAcom> ListaAcomEscenario(RolAcom rolAcom)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            //var fechaPeriodoTexto = periodo.ToString("yyyyMMdd");
            var roles = new List<RolAcom>();

            DbCommand dbc = db.GetStoredProcCommand("dbo.SP_RENVI_SEL_ACOM_ROL_MAXIMO");

            db.AddInParameter(dbc, "@wl_codigo_rol", DbType.String, Convert.ToString(rolAcom.CodRol));
            db.AddInParameter(dbc, "@wl_acom_maximo", DbType.String, Convert.ToString(rolAcom.ValorAcomMaximo));
            
            dbc.CommandTimeout = 0;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    var rol = new RolAcom();

                    if (dr["cod_codigo"] != DBNull.Value)
                        rol.CodEscenario = Convert.ToString(dr["cod_codigo"]);
                    if (dr["gls_corta"] != DBNull.Value)
                        rol.GlsEscenario = Convert.ToString(dr["gls_corta"]);

                    roles.Add(rol);
                }
            }

            return roles;
        }
    }
}
