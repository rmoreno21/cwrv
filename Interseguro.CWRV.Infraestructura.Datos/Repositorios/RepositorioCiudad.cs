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
    public class RepositorioCiudad : IRepositorioCiudad
    {
        public Ciudad ObtenerDatos(string idCiudad)
        {
            Ciudad ciudad = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_ciudad", idCiudad))
            {
                if (dr.Read())
                {
                    ciudad = new Ciudad();
                    ciudad.Id = idCiudad;
                    ciudad.Nombre = dr["gls_ciudad"].ToString();
                }
            }

            return ciudad;
        }

        public List<Ciudad> Listar(string idDepartamento)
        {
            List<Ciudad> listaCiudades = new List<Ciudad>();
            Ciudad ciudad;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_ciudad", idDepartamento))
            {
                while (dr.Read())
                {
                    ciudad = new Ciudad();
                    ciudad.Id = dr["cod_ciudad"].ToString();
                    ciudad.Nombre = dr["gls_ciudad"].ToString();

                    listaCiudades.Add(ciudad);
                }
            }

            return listaCiudades;
        }

        public void Registrar(Ciudad entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Ciudad entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Ciudad entity)
        {
            throw new NotImplementedException();
        }

        public Ciudad ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Ciudad ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Ciudad> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
