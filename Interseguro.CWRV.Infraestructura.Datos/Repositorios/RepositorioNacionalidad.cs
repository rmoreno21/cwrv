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

    public class RepositorioNacionalidad : IRepositorioNacionalidad
    {

        public List<Nacionalidad> Listar(string usuario)
        {

            List<Nacionalidad> listaNacionalidades = new List<Nacionalidad>();
            Nacionalidad nacionalidad;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_nacionalidad", usuario))
            {
                while (dr.Read())
                {
                    nacionalidad = new Nacionalidad();
                    nacionalidad.cod_nacionalidad = dr["cod_nacionalidad"].ToString();
                    nacionalidad.gls_nacionalidad = dr["gls_nacionalidad"].ToString();
                    nacionalidad.cod_equivalencia_rviadm = dr["cod_equivalencia"].ToString();

                    listaNacionalidades.Add(nacionalidad);
                }
            }

            return listaNacionalidades;

        }

        public void Registrar(Nacionalidad entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Nacionalidad entity)
        {
            throw new NotImplementedException();
        }
        
        public Nacionalidad ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public Nacionalidad ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Nacionalidad entity)
        {
            throw new NotImplementedException();
        }

        public List<Nacionalidad> Listar()
        {
            throw new NotImplementedException();
        }
    }

}
