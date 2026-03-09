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
    public class RepositorioRolDcom : IRepositorioRolDcom
    {

        public void Registrar(RolDcom entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(RolDcom entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(RolDcom entity)
        {
            throw new NotImplementedException();
        }

        public RolDcom ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public RolDcom ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<RolDcom> Listar()
        {
            throw new NotImplementedException();
        }

        public List<RolDcom> ListarRolDcom(RolDcom rolAcom)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            List<RolDcom> roles = new List<RolDcom>();

            var fecCotizacion = rolAcom.FechaCotizacion.ToString("yyyyMMdd");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_sel_rol_dcom");

            db.AddInParameter(dbc, "@wl_cod_rol", DbType.String, Convert.ToString(rolAcom.CodRol));
            db.AddInParameter(dbc, "@wl_fec_cierre", DbType.String, Convert.ToString(fecCotizacion));

            dbc.CommandTimeout = 0;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    var rol = new RolDcom();

                    if (dr["num_rango_ini"] != DBNull.Value)
                        rol.NumRangoIni = Convert.ToDouble(dr["num_rango_ini"]);
                    if (dr["num_rango_fin"] != DBNull.Value)
                        rol.NumRangoFin = Convert.ToDouble(dr["num_rango_fin"]);

                    roles.Add(rol);
                }
            }

            return roles;
        }

        //<GTIINI-10761>
        public List<RolDcom> ListarRolDcomRPP(RolDcom rolDcom)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            List<RolDcom> roles = new List<RolDcom>();

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_sel_rol_dcom_rpp");

            db.AddInParameter(dbc, "@wl_cod_rol", DbType.String, Convert.ToString(rolDcom.CodRol));
            db.AddInParameter(dbc, "@wl_fec_cierre", DbType.DateTime, rolDcom.FechaCotizacion);

            dbc.CommandTimeout = 0;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    var rol = new RolDcom();

                    if (dr["num_rango_ini"] != DBNull.Value)
                        rol.NumRangoIni = Convert.ToDouble(dr["num_rango_ini"]);
                    if (dr["num_rango_fin"] != DBNull.Value)
                        rol.NumRangoFin = Convert.ToDouble(dr["num_rango_fin"]);

                    roles.Add(rol);
                }
            }

            return roles;
        }
        //<GTIFIN-10761>

        public List<RolDcom> ListaDcomEscenario(RolDcom rolAcom)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            List<RolDcom> roles = new List<RolDcom>();

            DbCommand dbc = db.GetStoredProcCommand("dbo.SP_RENVI_SEL_ACOM_ROL_MAXIMO");

            db.AddInParameter(dbc, "@wl_codigo_rol", DbType.String, Convert.ToString(rolAcom.CodRol));
            db.AddInParameter(dbc, "@wl_acom_maximo", DbType.String, Convert.ToString(rolAcom.ValorDcomMaximo));

            dbc.CommandTimeout = 0;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    var rol = new RolDcom();

                    if (dr["cod_codigo"] != DBNull.Value)
                        rol.CodEscenario = Convert.ToString(dr["cod_codigo"]);
                    if (dr["gls_corta"] != DBNull.Value)
                        rol.GlsEscenario = Convert.ToString(dr["gls_corta"]);

                    roles.Add(rol);
                }
            }

            return roles;
        }

        //<INIGTI_7012_S27>
        public List<RolDcom> ListarRangoDcomIFP(RolDcom rolDcom)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            List<RolDcom> roles = new List<RolDcom>();

            var fecCotizacion = rolDcom.FechaCotizacion.ToString("yyyyMMdd");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_sel_rol_dcom_ifp");

            db.AddInParameter(dbc, "@wl_cod_rol", DbType.String, Convert.ToString(rolDcom.CodRol));
            db.AddInParameter(dbc, "@wl_fec_cierre", DbType.String, Convert.ToString(fecCotizacion));

            dbc.CommandTimeout = 0;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    var rol = new RolDcom();

                    if (dr["num_rango"] != DBNull.Value)
                        rol.ValorDcom = Convert.ToDouble(dr["num_rango"]);

                    roles.Add(rol);
                }
            }

            return roles;
        }
        //<FINGTI_7012_S27>

    }
}
