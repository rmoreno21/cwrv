using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

using System.Data;
using System.Data.Common;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioProducto: IRepositorioProducto
    {
        public List<Producto> Listar(string idCategoria)
        {
            List<Producto> listaProductos = new List<Producto>();
            Producto producto;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_producto", idCategoria))
            {
                while (dr.Read())
                {
                    producto = new Producto();
                    producto.Id = dr["cod_tipo_producto"].ToString();
                    producto.Nombre = dr["gls_tipo_producto"].ToString();

                    listaProductos.Add(producto);
                }
            }

            return listaProductos;
        }

        public void Registrar(Producto entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Producto entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Producto entity)
        {
            throw new NotImplementedException();
        }

        public Producto ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Producto ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Producto> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
