using Interseguro.CWRV.Dominio.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{

    public class RepositorioProfesion : IRepositorioProfesion
    {

        public List<Profesion> Listar(string usuario)
        {

            List<Profesion> listaProfesiones = new List<Profesion>();
            Profesion Profesion;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_Profesion", usuario))
            {
                while (dr.Read())
                {
                    Profesion = new Profesion();
                    Profesion.cod_profesion = dr["cod_profesion"].ToString();
                    Profesion.gls_profesion = dr["gls_profesion"].ToString();

                    listaProfesiones.Add(Profesion);
                }
            }

            return listaProfesiones;

        }

        public void Registrar(Profesion entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Profesion entity)
        {
            throw new NotImplementedException();
        }

        public Profesion ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public Profesion ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Profesion entity)
        {
            throw new NotImplementedException();
        }

        public List<Profesion> Listar()
        {
            throw new NotImplementedException();
        }
    }

}
