using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data;

namespace Interseguro.CWRV.Infraestructura.Datos.Repositorios
{

    public class RepositorioReporteCotizacion : IRepositorioReporteCotizacion
    {
                
        public List<ReporteCotizacion> ListarEtiquetas()
        {

            List<ReporteCotizacion> listaReporteCotizacion = new List<ReporteCotizacion>();
            ReporteCotizacion reportecotizacion;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.uspReporteCotizacion_sel"))
            {
                while (dr.Read())
                {
                    reportecotizacion = new ReporteCotizacion();

                    reportecotizacion.cod_Id = Convert.ToInt32(dr["cod_parametro"].ToString());
                    reportecotizacion.gls_Titulo = dr["gls_parametro"].ToString();
                    reportecotizacion.gls_Descripcion = dr["cod_parametro"].ToString();
                    reportecotizacion.num_Tamanho = Convert.ToInt32(dr["gls_parametro"].ToString());
                    reportecotizacion.num_Posicion = Convert.ToInt32(dr["cod_parametro"].ToString());
                    reportecotizacion.gls_Negrita = dr["gls_parametro"].ToString();
                    reportecotizacion.gls_Color = dr["cod_parametro"].ToString();
                    reportecotizacion.gls_Alineamiento = dr["gls_parametro"].ToString();

                    listaReporteCotizacion.Add(reportecotizacion);
                }
            }

            return listaReporteCotizacion;

        }

        public void Registrar(ReporteCotizacion entity)
        {
            throw new NotImplementedException();
        }

        public void Actualizar(ReporteCotizacion entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(ReporteCotizacion entity)
        {
            throw new NotImplementedException();
        }

        public ReporteCotizacion ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public ReporteCotizacion ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public List<ReporteCotizacion> Listar()
        {
            throw new NotImplementedException();
        }
    }

}
