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
    public class RepositorioTelefono: IRepositorioTelefono
    {
        public List<Telefono> Listar(string cuspp)
        {
            List<Telefono> listaTelefonos = new List<Telefono>();
            Telefono telefono;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_telefono", cuspp))
            {
                while (dr.Read())
                {
                    telefono = new Telefono();
                    telefono.Id = Convert.ToInt32(dr["id_telefono"]);
                    telefono.Tipo = new TipoTelefono { Nombre = dr["gls_tipo_telefono"].ToString() };
                    telefono.Numero = dr["num_telefono"].ToString();
                    telefono.FechaIngreso = Convert.ToDateTime(dr["fec_ingreso"]);
                    telefono.Principal = (dr["ind_principal"].ToString() == "S") ? true : false;

                    listaTelefonos.Add(telefono);
                }
            }

            return listaTelefonos;
        }

        public Telefono ObtenerDatos(int idTelefono)
        {
            Telefono telefono = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_telefono", idTelefono))
            {
                if (dr.Read())
                {
                    telefono = new Telefono();
                    telefono.Tipo = new TipoTelefono { Id = dr["cod_tipo_telefono"].ToString() };
                    telefono.Numero = dr["num_telefono"].ToString();
                    telefono.Principal = (dr["ind_principal"].ToString() == "S") ? true : false;
                }
            }

            return telefono;
        }

        public void Registrar(Telefono entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_telefono");

                db.AddInParameter(dbc, "@wl_num_cuissp", DbType.String, entity.Afiliado.CUSPP);
                db.AddInParameter(dbc, "@wl_cod_tipo_telefono", DbType.String, entity.Tipo.Id);
                db.AddInParameter(dbc, "@wl_num_telefono", DbType.String, entity.Numero);
                db.AddInParameter(dbc, "@wl_ind_principal", DbType.String, entity.Principal ? "S" : "N");
                db.AddInParameter(dbc, "@wl_usr_ingreso", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(Telefono entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_telefono");

                db.AddInParameter(dbc, "@wl_id_telefono", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_num_cuissp", DbType.String, entity.Afiliado.CUSPP);
                db.AddInParameter(dbc, "@wl_cod_tipo_telefono", DbType.String, entity.Tipo.Id);
                db.AddInParameter(dbc, "@wl_num_telefono", DbType.String, entity.Numero);
                db.AddInParameter(dbc, "@wl_ind_principal", DbType.String, entity.Principal ? "S" : "N");
                db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(Telefono entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_telefono");

                db.AddInParameter(dbc, "@wl_id_telefono", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Telefono ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Telefono ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Telefono> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
