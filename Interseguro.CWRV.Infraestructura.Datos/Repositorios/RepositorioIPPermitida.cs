using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioIPPermitida : IRepositorioIPPermitida
    {
        public IPPermitida ObtenerDatos(string ip)
        {
            IPPermitida ippermitida = null;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_obtener_ip_permitida", ip))
            {
                if (dr.Read())
                {
                    ippermitida = new IPPermitida();
                    ippermitida.Id = Convert.ToInt32(dr["id_ip_permitida"]);
                    ippermitida.Descripcion = dr["gls_descripcion"].ToString();
                    ippermitida.IP = dr["num_ip"].ToString();
                    ippermitida.SegundosExpiracion = Convert.ToInt32(dr["val_segundos_expiracion"]);
                }
            }

            return ippermitida;
        }

        public void Registrar(IPPermitida entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(IPPermitida entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(IPPermitida entity)
        {
            throw new NotImplementedException();
        }

        public IPPermitida ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public IPPermitida ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<IPPermitida> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
