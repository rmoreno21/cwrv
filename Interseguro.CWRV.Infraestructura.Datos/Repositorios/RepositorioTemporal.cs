using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Data;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using System.Data;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioTemporal : IRepositorioTemporal
    {

        //public List<Temporal> ListarPorTabla(string tabla)
        //{

        //    List<Temporal> listaTemporal = new List<Temporal>();
        //    Temporal temporal;

        //    Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

        //    using (IDataReader dr = db.ExecuteReader("dbo.ups_cwrv_consultar_temporal", tabla))
        //    {
        //        while (dr.Read())
        //        {
        //            temporal = new Temporal();
        //            temporal.cod_parametro = dr["cod_parametro"].ToString();
        //            temporal.gls_parametro = dr["gls_parametro"].ToString();
                    
        //            listaTemporal.Add(temporal);
        //        }
        //    }

        //    return listaTemporal;

        //}

        public void Registrar(Temporal entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Temporal entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Temporal entity)
        {
            throw new NotImplementedException();
        }

        public Temporal ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Temporal ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }
        
        public List<Temporal> Listar()
        {
            throw new NotImplementedException();
        }

    }
}
