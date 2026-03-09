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
    public class RepositorioComuna : IRepositorioComuna
    {
        public Comuna ObtenerDatos(string idComuna)
        {
            Comuna comuna = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_comuna", idComuna))
            {
                if (dr.Read())
                {
                    comuna = new Comuna();
                    comuna.Id = idComuna;
                    comuna.Nombre = dr["gls_comuna"].ToString();
                }
            }

            return comuna;
        }

        public List<Comuna> Listar(string idCiudad)
        {
            List<Comuna> listaComunas = new List<Comuna>();
            Comuna comuna;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_comuna", idCiudad))
            {
                while (dr.Read())
                {
                    comuna = new Comuna();
                    comuna.Id = dr["cod_comuna"].ToString();
                    comuna.Nombre = dr["gls_comuna"].ToString();

                    listaComunas.Add(comuna);
                }
            }

            return listaComunas;
        }

        public void Registrar(Comuna entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Comuna entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Comuna entity)
        {
            throw new NotImplementedException();
        }

        public Comuna ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Comuna ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Comuna> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
