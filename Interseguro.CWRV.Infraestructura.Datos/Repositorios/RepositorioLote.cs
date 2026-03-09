using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{
    public class RepositorioLote : IRepositorioLote
    {
        public List<Lote> Listar(int numero)
        {
            List<Lote> listaLotes = new List<Lote>();
            Lote lote;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_lotes_x_fec_cierre", numero, null, null))
            {
                while (dr.Read())
                {
                    lote = new Lote();
                    lote.Numero = Convert.ToInt32(dr["num_lote_cotizacion"]);
                    if (dr["fec_cierre"] != DBNull.Value)
                        lote.FechaCierre = Convert.ToDateTime(dr["fec_cierre"]);
                    if (dr["fec_envio"] != DBNull.Value)
                        lote.FechaEnvio = Convert.ToDateTime(dr["fec_envio"]);

                    listaLotes.Add(lote);
                }
            }

            return listaLotes;
        }

        public List<Lote> Listar(DateTime fechaCierreInicial, DateTime fechaCierreFinal)
        {
            List<Lote> listaLotes = new List<Lote>();
            Lote lote;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_lotes_x_fec_cierre", 0, fechaCierreInicial, fechaCierreFinal))
            {
                while (dr.Read())
                {
                    lote = new Lote();
                    lote.Numero = Convert.ToInt32(dr["num_lote_cotizacion"]);
                    if (dr["fec_cierre"] != DBNull.Value)
                        lote.FechaCierre = Convert.ToDateTime(dr["fec_cierre"]);
                    if (dr["fec_envio"] != DBNull.Value)
                        lote.FechaEnvio = Convert.ToDateTime(dr["fec_envio"]);

                    listaLotes.Add(lote);
                }
            }

            return listaLotes;
        }

        public List<Lote> ListarResultado(int numero)
        {
            List<Lote> listaLotes = new List<Lote>();
            Lote lote;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_lotes_resultado_x_fec_cierre", numero, null, null))
            {
                while (dr.Read())
                {
                    lote = new Lote();
                    lote.Numero = Convert.ToInt32(dr["num_lote_resultado"]);
                    if (dr["fec_cierre"] != DBNull.Value)
                        lote.FechaCierre = Convert.ToDateTime(dr["fec_cierre"]);

                    listaLotes.Add(lote);
                }
            }

            return listaLotes;
        }

        public List<Lote> ListarResultado(DateTime fechaCierreInicial, DateTime fechaCierreFinal)
        {
            List<Lote> listaLotes = new List<Lote>();
            Lote lote;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_consultar_lotes_resultado_x_fec_cierre", 0, fechaCierreInicial, fechaCierreFinal))
            {
                while (dr.Read())
                {
                    lote = new Lote();
                    lote.Numero = Convert.ToInt32(dr["num_lote_resultado"]);
                    if (dr["fec_cierre"] != DBNull.Value)
                        lote.FechaCierre = Convert.ToDateTime(dr["fec_cierre"]);

                    listaLotes.Add(lote);
                }
            }

            return listaLotes;
        }

        public void Registrar(Lote entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(Lote entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(Lote entity)
        {
            throw new NotImplementedException();
        }

        public Lote ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public Lote ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<Lote> Listar()
        {
            throw new NotImplementedException();
        }
    }
}
