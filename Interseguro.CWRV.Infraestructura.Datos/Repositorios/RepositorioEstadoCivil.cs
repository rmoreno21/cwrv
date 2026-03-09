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

    public class RepositorioEstadoCivil : IRepositorioEstadoCivil
    {
        public List<EstadoCivil> Listar(string usuario)
        {
            List<EstadoCivil> listaEstadosCiviles = new List<EstadoCivil>();
            EstadoCivil estadoCivil;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_listar_estado_civil", usuario))
            {
                while (dr.Read())
                {
                    estadoCivil = new EstadoCivil();
                    estadoCivil.cod_estado_civil = dr["cod_estado_civil"].ToString();
                    estadoCivil.gls_estado_civil = dr["gls_estado_civil"].ToString();

                    listaEstadosCiviles.Add(estadoCivil);
                }
            }

            return listaEstadosCiviles;
        }
        
        public void Registrar(EstadoCivil entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(EstadoCivil entity)
        {
            throw new NotImplementedException();
        }

        public EstadoCivil ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public EstadoCivil ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(EstadoCivil entity)
        {
            throw new NotImplementedException();
        }

        public List<EstadoCivil> Listar()
        {
            throw new NotImplementedException();
        }
    }

}
