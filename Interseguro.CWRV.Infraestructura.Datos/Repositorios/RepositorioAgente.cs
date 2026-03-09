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
    public class RepositorioAgente : IRepositorioAgente
    {
        public List<Agente> Listar(string usuario, string rol)
        {
            List<Agente> listaAgentes = new List<Agente>();
            Agente agente;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_usuario", usuario, rol))
            {
                while (dr.Read())
                {
                    agente = new Agente();
                    agente.Id = dr["num_agente"].ToString();
                    agente.Nombre = dr["nom_agente"].ToString();
                    if (dr["num_agente_padre"] != DBNull.Value)
                        agente.IdPadre = dr["num_agente_padre"].ToString();
                    agente.IdNivel = Convert.ToInt32(dr["cod_nivel"]);
                    agente.IdAgencia = dr["cod_agencia"].ToString();
                    agente.IdContrato = dr["cod_contrato"].ToString();
                    if (dr["cod_departamento"] != DBNull.Value)
                        agente.Departamento = new Departamento { Id = dr["cod_departamento"].ToString() };
                    agente.IdUbicacionGeografica = dr["cod_ubicacion_geografica"].ToString();
                    if (dr["cod_username"] != DBNull.Value)
                        agente.Usuario = dr["cod_username"].ToString();

                    listaAgentes.Add(agente);
                }
            }

            return listaAgentes;
        }

        public Agente ObtenerMS(string idAgente)
        {
            Agente agente = new Agente();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_ms_agente", idAgente))
            {
                if (dr.Read())
                {
                    agente.Id = idAgente;
                    agente.MS1 = Convert.ToDouble(dr["ms1"]);
                    agente.MS6 = Convert.ToDouble(dr["ms6"]);
                }
            }

            return agente;
        }

        public void Registrar(Agente entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_jerarquia_agentes");

                db.AddInParameter(dbc, "@wl_num_agente", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_num_agente_padre", DbType.Int32, entity.IdPadre);
                db.AddInParameter(dbc, "@wl_cod_nivel", DbType.Int32, entity.IdNivel);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, entity.Usuario);
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(Agente entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_jerarquia_agentes");

                db.AddInParameter(dbc, "@wl_num_agente", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_num_agente_padre", DbType.Int32, entity.IdPadre);
                db.AddInParameter(dbc, "@wl_cod_nivel", DbType.Int32, entity.IdNivel);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, entity.Usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(Agente entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_jerarquia_agentes");

                db.AddInParameter(dbc, "@wl_num_agente", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, entity.Usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Agente ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Agente ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public Agente ObtenerPorId(int Id, string usuario)
        {
            try
            {
                Agente agente = new Agente();

                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_datos_agente", Id, usuario))
                {
                    if (dr.Read())
                    {
                        agente.Id = dr["num_agente"].ToString();
                        agente.CodTipoDocumento = dr["cod_tipo_documento"].ToString();
                        agente.NumDocumento = dr["rut_agente"].ToString();
                        agente.IdAgencia = dr["cod_agencia"].ToString();
                        agente.IdContrato = dr["cod_contrato"].ToString();
                        agente.ApellidoPaterno = dr["ape_paterno"].ToString();
                        agente.ApellidoMaterno = dr["ape_materno"].ToString();
                        agente.NombrePersona = dr["nom_persona"].ToString();
                        agente.Nombre = dr["nom_agente"].ToString();
                        agente.Usuario = dr["cod_username"].ToString();
                    }
                }

                return agente;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Agente> Listar()
        {
            throw new NotImplementedException();
        }

        public List<Agente> ObtenerAgenteDeudaAcom(string idAgente)
        {
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            List<Agente> lstAgente = new List<Agente>();
            using (IDataReader dr = db.ExecuteReader("dbo.sp_renvi_sel_agente_deuda_acom", idAgente))
            {
                if (dr.Read())
                {
                    Agente agente = new Agente();
                    agente.Id = dr["num_agente"].ToString();
                    agente.ValImporteDocumento = Convert.ToDouble(dr["val_importe_documento"]);
                    agente.ValImporteDevolucion = Convert.ToDouble(dr["val_importe_devolucion"]);
                    agente.CodTipoDocumento = dr["cod_tipo_documento"].ToString();
                    agente.NumDocumento = dr["num_documento"].ToString();
                    agente.CodCiaSbs = dr["cod_cia_sbs"].ToString();
                    lstAgente.Add(agente);
                }
            }

            return lstAgente;
        }

        public Agente ObtenerSupervisorAgente(string usuario)
        {
            Agente agente = new Agente();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_AgenteSupervisor", usuario))
            {
                while (dr.Read())
                {

                    agente.Nombre = dr["agente"].ToString();
                    agente.IdPadre = "";
                    if (dr["supervisor"] != DBNull.Value)
                        agente.IdPadre = dr["supervisor"].ToString();
                    agente.IdAgencia = dr["cod_agencia"].ToString();

                }
            }

            return agente;
        }

        public List<Agente> ObtenerJerarquiaAgente(int nivel, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                List<Agente> lstAgente = new List<Agente>();

                using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_jerarquia_agentes", nivel, usuario))
                {
                    while (dr.Read())
                    {
                        Agente agente = new Agente();

                        agente.Id = dr["num_agente"].ToString();
                        agente.Nombre = dr["nom_agente"].ToString();
                        agente.IdPadre = dr["nom_agente_padre"].ToString();

                        lstAgente.Add(agente);
                    }
                }

                return lstAgente;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Agente> ListarAgenteExterno(string gls_agente, string usuario)
        {
            List<Agente> listaAgentes = new List<Agente>();

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_agente_externo", gls_agente, usuario))
            {
                while (dr.Read())
                {
                    var agente = new Agente();
                    agente.Id = dr["cod_agente_externo"].ToString();
                    agente.Usuario = dr["gls_agente_externo"].ToString();

                    if (dr["cod_agente_externo_padre"] != DBNull.Value)
                        agente.IdPadre = dr["cod_agente_externo_padre"].ToString();

                    agente.IdNivel = (int)dr["cod_nivel"];

                    listaAgentes.Add(agente);
                }
            }

            return listaAgentes;
        }

        public Agente ObtenerUltimoAgentePorCartera (string cartera, string usuario)
        {
            Agente agente = null;
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_ultimo_agente_por_cartera", cartera, usuario))
            {
                if (dr.Read())
                {
                    agente = new Agente();
                    agente.Id = dr["cod_agente"].ToString();
                    agente.IdPadre = dr["cod_supervisor"].ToString();
                    agente.IdCartera = dr["cod_cartera"].ToString();
                    agente.CodTipoDocumento = dr["cod_tipo_identificacion"].ToString();
                    agente.NumDocumento = dr["num_identificacion"].ToString();
                    agente.IdAgencia = dr["cod_agencia"].ToString();
                    if (dr["cod_vigencia_agente"] != DBNull.Value)
                        agente.Vigente = dr["cod_vigencia_agente"].ToString() == "S";
                    if (dr["fec_inicio_vigencia"] != DBNull.Value)
                        agente.InicioVigencia = Convert.ToDateTime(dr["fec_inicio_vigencia"]);
                    if (dr["fec_fin_vigencia"] != DBNull.Value)
                        agente.FinVigencia = Convert.ToDateTime(dr["fec_fin_vigencia"]);
                    agente.IdContrato = dr["cod_contrato"].ToString();
                    agente.ApellidoPaterno = dr["gls_apellido_paterno"].ToString();
                    agente.ApellidoMaterno = dr["gls_apellido_materno"].ToString();
                    agente.NombrePersona = dr["gls_nombre"].ToString();
                    agente.Nombre = dr["gls_agente"].ToString();
                    if (dr["cod_username"] != DBNull.Value)
                        agente.Usuario = dr["cod_username"].ToString();
                }
            }
            return agente;
        }
    }
}
