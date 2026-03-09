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
    public class RepositorioDireccion: IRepositorioDireccion
    {
        public List<Direccion> Listar(string cuspp)
        {
            List<Direccion> listaDirecciones = new List<Direccion>();
            Direccion direccion;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_direccion", cuspp))
            {
                while (dr.Read())
                {
                    direccion = new Direccion();
                    direccion.Id = Convert.ToInt32(dr["id_direccion"]);
                    direccion.Glosa = dr["gls_direccion"].ToString();
                    direccion.Comuna = new Comuna { Id = dr["cod_comuna"].ToString().ToLower(), Nombre = dr["gls_comuna"].ToString().ToUpper() };
                    direccion.Ciudad = new Ciudad { Id = dr["cod_ciudad"].ToString().ToLower(), Nombre = dr["gls_ciudad"].ToString().ToUpper() };
                    direccion.Departamento = new Departamento { Id = dr["cod_depart"].ToString().ToLower(), Nombre = dr["gls_depart"].ToString().ToUpper() };
                    direccion.FechaIngreso = Convert.ToDateTime(dr["fec_ingreso"]);
                    direccion.Principal = (dr["ind_principal"].ToString() == "S") ? true : false;
                    direccion.TipoVia = new Parametro { Id = dr["cod_tipovia"].ToString(), Glosa = dr["gls_tipovia"].ToString() };
                    direccion.EspacioUrbano = dr["cod_EspacioUrbano"].ToString();
                    
                    listaDirecciones.Add(direccion);
                }
            }

            return listaDirecciones;
        }

        public Direccion ObtenerDatos(int idDireccion)
        {
            Direccion direccion = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_direccion", idDireccion))
            {
                if (dr.Read())
                {
                    direccion = new Direccion();
                    direccion.Glosa = dr["gls_direccion"].ToString();
                    direccion.Comuna = new Comuna { Id = dr["cod_comuna"].ToString().ToLower() };
                    direccion.Ciudad = new Ciudad { Id = dr["cod_ciudad"].ToString().ToLower() };
                    direccion.Departamento = new Departamento { Id = dr["cod_depart"].ToString().ToLower() };
                    direccion.TipoVia = new Parametro { Id = dr["cod_TipoVia"].ToString() };
                    direccion.EspacioUrbano = dr["cod_EspacioUrbano"].ToString();
                    direccion.Principal = (dr["ind_principal"].ToString() == "S") ? true : false;
                }
            }

            return direccion;
        }

        public void Registrar(Direccion entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_insertar_direccion");

                db.AddInParameter(dbc, "@wl_num_cuissp", DbType.String, entity.Afiliado.CUSPP);
                db.AddInParameter(dbc, "@wl_gls_direccion", DbType.String, entity.Glosa);
                db.AddInParameter(dbc, "@wl_cod_comuna", DbType.String, entity.Comuna.Id);
                db.AddInParameter(dbc, "@wl_cod_ciudad", DbType.String, entity.Ciudad.Id);
                db.AddInParameter(dbc, "@wl_ind_principal", DbType.String, entity.Principal ? "S": "N");
                db.AddInParameter(dbc, "@wl_usr_ingreso", DbType.String, entity.Usuario.NombreUsuario);
                db.AddInParameter(dbc, "@wl_cod_TipoVia", DbType.String, entity.TipoVia.Id);
                db.AddInParameter(dbc, "@wl_cod_EspacioUrbano", DbType.String, entity.EspacioUrbano); 
                db.AddInParameter(dbc, "@wl_cod_Departamento", DbType.String, entity.Departamento.Id);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Actualizar(Direccion entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_direccion");

                db.AddInParameter(dbc, "@wl_id_direccion", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_num_cuissp", DbType.String, entity.Afiliado.CUSPP);
                db.AddInParameter(dbc, "@wl_gls_direccion", DbType.String, entity.Glosa);
                db.AddInParameter(dbc, "@wl_cod_comuna", DbType.String, entity.Comuna.Id);
                db.AddInParameter(dbc, "@wl_cod_ciudad", DbType.String, entity.Ciudad.Id);
                db.AddInParameter(dbc, "@wl_ind_principal", DbType.String, entity.Principal ? "S" : "N");
                db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.Usuario.NombreUsuario);
                db.AddInParameter(dbc, "@wl_cod_TipoVia", DbType.String, entity.TipoVia.Id);
                db.AddInParameter(dbc, "@wl_cod_EspacioUrbano", DbType.String, entity.EspacioUrbano); 
                db.AddInParameter(dbc, "@wl_cod_Departamento", DbType.String, entity.Departamento.Id);
   
                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Eliminar(Direccion entity)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_eliminar_direccion");

                db.AddInParameter(dbc, "@wl_id_direccion", DbType.Int32, entity.Id);
                db.AddInParameter(dbc, "@wl_usr_modificacion", DbType.String, entity.Usuario.NombreUsuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Direccion ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Direccion ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Direccion> Listar()
        {
            throw new NotImplementedException();
        }

        public void ActualizarDireccionSolicitud(string numCuspp, string numSolicitud, string usuario)
        {
            try
            {
                Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");
                SqlCommand dbc = (SqlCommand)db.GetStoredProcCommand("dbo.usp_cwrv_actualizar_direccion_solicitud");

                db.AddInParameter(dbc, "@wl_num_cuissp", DbType.String, numCuspp);
                db.AddInParameter(dbc, "@wl_num_solicitud", DbType.String, numSolicitud);
                db.AddInParameter(dbc, "@wl_usuario", DbType.String, usuario);

                db.ExecuteNonQuery(dbc);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Direccion> Listar(string numSolicitud, string idDireccion, string usuario)
        {
            List<Direccion> listaDirecciones = new List<Direccion>();
            Direccion direccion;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_rvi_direccion", numSolicitud, null, usuario))
            {
                while (dr.Read())
                {
                    direccion = new Direccion();
                    direccion.Id = Convert.ToInt32(dr["id_direccion"]);
                    direccion.Glosa = dr["gls_direccion"].ToString();
                    direccion.Comuna = new Comuna { Id = dr["cod_distrito"].ToString().ToLower(), Nombre = dr["gls_distrito"].ToString().ToUpper() };
                    direccion.Ciudad = new Ciudad { Id = dr["cod_provincia"].ToString().ToLower(), Nombre = dr["gls_provincia"].ToString().ToUpper() };
                    direccion.Departamento = new Departamento { Id = dr["cod_departamento"].ToString().ToLower(), Nombre = dr["gls_departamento"].ToString().ToUpper() };
                    direccion.Principal = (dr["ind_tipo"].ToString() == "P") ? true : false;
                    direccion.Vigencia = (dr["ind_vigencia"].ToString() == "S") ? true : false;
                    direccion.TipoVia = new Parametro { Id = dr["cod_tipo_via"].ToString(), Glosa = dr["gls_tipo_via"].ToString() };
                    direccion.EspacioUrbano = dr["gls_espacio_urbano"].ToString();

                    //<GTI.59048-INI>
                    direccion.numeroTelefono = dr["num_telefono"].ToString();
                    direccion.nombreVia = dr["gls_nom_via"].ToString();
                    direccion.numeroVia = dr["gls_num_via"].ToString();
                    direccion.numeroInterior = dr["gls_num_interior"].ToString();
                    direccion.tipoZona = new Parametro { Id = dr["cod_tipo_zona"].ToString() };
                    direccion.nombreZona = dr["gls_nom_zona"].ToString();
                    direccion.referencia = dr["gls_referencia"].ToString();
                    direccion.largaDistancia = new Parametro { Id = dr["cod_larga_distancia"].ToString() };
                    direccion.numeroDepartamento = dr["num_departamento"].ToString();
                    direccion.manzana = dr["gls_manzana"].ToString();
                    direccion.numeroLote = dr["gls_lote"].ToString();
                    direccion.kilometro = dr["gls_kilometro"].ToString();
                    direccion.block = dr["gls_block"].ToString();
                    direccion.etapa = dr["gls_etapa"].ToString();
                    //<GTI.59048-FIN>

                    listaDirecciones.Add(direccion);
                }
            }

            return listaDirecciones;
        }

    }
}
