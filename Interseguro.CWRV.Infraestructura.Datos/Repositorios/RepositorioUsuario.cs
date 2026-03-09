using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data.Common;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;


namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioUsuario: IRepositorioUsuario
    {

        public RepositorioUsuario()
        {
            
        }

        public void Registrar(Usuario entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Usuario entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Usuario entity)
        {
            throw new NotImplementedException();
        }

        public Usuario ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Usuario ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Usuario> Listar()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();
            Usuario usuario;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("ConexionDesarrolloOracle");

            using (IDataReader dr = db.ExecuteReader("PKG_USUARIO.SP_Listar_Usuarios",null,null,null,null,null) )
            {
                while (dr.Read())
                {
                    usuario = new Usuario();
                    //usuario.UsuarioId = Convert.ToInt32(dr["codigo_usuario"]);
                    usuario.NombreUsuario = dr["nombre_usuario"].ToString();
                    //usuario.Clave = dr["clave"].ToString();
                    //usuario.Activo = Convert.ToInt32(dr["activo"]) != 0 ? true : false;
                    listaUsuarios.Add(usuario);
                }
            }

            return listaUsuarios;
        }

        public List<Usuario> ListarPorNombres(string nombres)
        {
            throw new NotImplementedException();
        }

        //<SRI.INI-20322_E2>
        public string obtenerNumAgente(string nombreUsuario)
        {
            string valor = "";
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_sel_prv_usuario", nombreUsuario, String.Empty))
            {
                while (dr.Read())
                {
                    valor = (dr["cod_empleado"] != DBNull.Value && dr["cod_empleado"].ToString() != string.Empty) ? dr["cod_empleado"].ToString() : "";
                }
            }
            return valor;
        }

        public List<Usuario> Listar(string nombreUsuario, string idAgente)
        {
            List<Usuario> usuarios = new List<Usuario>();
            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_sel_prv_usuario", nombreUsuario, idAgente))
            {
                while (dr.Read())
                {
                    Usuario usuario = new Usuario();
                    usuario.CodigoEmpleado = dr["cod_empleado"].ToString();
                    usuario.NombreUsuario = dr["cod_username"].ToString();
                    usuarios.Add(usuario);
                }
            }
            return usuarios;
        }
        //<SRI.INI-20322_E2>

    }
}
