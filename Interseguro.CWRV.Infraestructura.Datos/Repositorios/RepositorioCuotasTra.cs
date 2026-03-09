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
    public class RepositorioCuotasTra : IRepositorioCuotasTra
    {
        public CuotasTra ObtenerCuotasTra(CuotasTra rolCuotas)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            
            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_obtener_cuotas_tra");

            db.AddInParameter(dbc, "@fecCotizacion", DbType.DateTime, rolCuotas.FecCotizacion);
            //db.AddInParameter(dbc, "@cod_rol", DbType.String, rolCuotas.RolAzman);
            db.AddInParameter(dbc, "@num_agente", DbType.String, rolCuotas.Agente.Id );

            dbc.CommandTimeout = 0;
            CuotasTra rol=null;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                if (dr.Read())
                {
                    rol = new CuotasTra();
                    //if (dr["cod_rol"] != DBNull.Value)
                    //    rol.RolAzman = dr["cod_rol"].ToString();

                    if (dr["num_agente"] != DBNull.Value)
                        rol.Agente = new Agente { 
                            Id = dr["num_agente"].ToString(), 
                            Usuario = dr["cod_username"].ToString() };

                    if (dr["fec_inicio_vigencia"] != DBNull.Value)
                        rol.FecInicioVigencia = Convert.ToDateTime(dr["fec_inicio_vigencia"]);

                    if (dr["fec_fin_vigencia"] != DBNull.Value)
                        rol.FecFinVigencia = Convert.ToDateTime(dr["fec_fin_vigencia"]);

                    if (dr["nro_casos_total"] != DBNull.Value)
                        rol.NroCasosTotal = Convert.ToInt32(dr["nro_casos_total"]);

                    if (dr["nro_casos_solicitados"] != DBNull.Value)
                        rol.NroCasosSolicitados = Convert.ToInt32(dr["nro_casos_solicitados"]);

                    if (dr["nro_casos_efectivos"] != DBNull.Value)
                        rol.NroCasosEfectivos = Convert.ToInt32(dr["nro_casos_efectivos"]);

                }   
            }
            return rol;
        }


        public List<CuotasTra> ListarCuotasTra(int periodo, int mes)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            DbCommand dbc = db.GetStoredProcCommand("dbo.usp_cwrv_listar_cuotas_tra");

            db.AddInParameter(dbc, "@periodo", DbType.Int32, periodo);
            db.AddInParameter(dbc, "@mes", DbType.Int32, mes);

            dbc.CommandTimeout = 0;

            List<CuotasTra> lstCuotasTra = new List<CuotasTra>();

            CuotasTra cuotas = null;

            using (IDataReader dr = db.ExecuteReader(dbc))
            {
                while (dr.Read())
                {
                    cuotas = new CuotasTra();

                    if (dr["num_agente"] != DBNull.Value)
                        cuotas.Agente = new Agente
                        {
                            Id = dr["num_agente"].ToString(),
                            Nombre = dr["nom_agente"].ToString(),
                            Usuario = dr["cod_username"].ToString()
                        };

                    if (dr["fec_inicio_vigencia"] != DBNull.Value)
                        cuotas.FecInicioVigencia = Convert.ToDateTime(dr["fec_inicio_vigencia"]);

                    if (dr["fec_fin_vigencia"] != DBNull.Value)
                        cuotas.FecFinVigencia = Convert.ToDateTime(dr["fec_fin_vigencia"]);

                    if (dr["nro_casos_total"] != DBNull.Value)
                        cuotas.NroCasosTotal = Convert.ToInt32(dr["nro_casos_total"]);

                    if (dr["nro_casos_solicitados"] != DBNull.Value)
                        cuotas.NroCasosSolicitados = Convert.ToInt32(dr["nro_casos_solicitados"]);

                    if (dr["nro_casos_efectivos"] != DBNull.Value)
                        cuotas.NroCasosEfectivos = Convert.ToInt32(dr["nro_casos_efectivos"]);


                    if (dr["fec_inicio_vigencia"] != DBNull.Value)
                        cuotas.FecInicioVigenciaStr = Convert.ToDateTime(dr["fec_inicio_vigencia"]).ToString("dd/MM/yyyy");

                    if (dr["fec_fin_vigencia"] != DBNull.Value)
                        cuotas.FecFinVigenciaStr = Convert.ToDateTime(dr["fec_fin_vigencia"]).ToString("dd/MM/yyyy");

                    lstCuotasTra.Add(cuotas);
                }
            }
            return lstCuotasTra;
        }

        public void Registrar(CuotasTra entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(CuotasTra entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(CuotasTra entity)
        {
            throw new NotImplementedException();
        }

        public CuotasTra ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public CuotasTra ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<CuotasTra> Listar()
        {
            throw new NotImplementedException();
        }


        public void RegistrarCuotas(List<CuotasTra> lstCuotas, string usuario)
        {

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_registrar_cuotas_tra");
            CuotasTra cuotas = new CuotasTra();
            string xml = cuotas.XMLCuotas(lstCuotas);
            db.AddInParameter(dbc, "@xml_data", DbType.String, xml);
            db.AddInParameter(dbc, "@usuario", DbType.String, usuario);
            db.ExecuteNonQuery(dbc);
        }
    }
}
