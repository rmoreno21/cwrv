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
    public class RepositorioReporteIndicadores : IRepositorioReporteIndicadores
    {

        public List<ReporteIndicadoresRRVV> ListarIndicadores()
        {

            List<ReporteIndicadoresRRVV> listaReporteIndicadores = new List<ReporteIndicadoresRRVV>();
            ReporteIndicadoresRRVV reporteIndicadores;

            Database db = EnterpriseLibraryContainer.Current.GetInstance<Database>("Conexion");

            using (IDataReader dr = db.ExecuteReader("dbo.usp_cwrv_reporte_indicadores_RRVV"))
            {
                while (dr.Read())
                {
                    reporteIndicadores = new ReporteIndicadoresRRVV();

                    reporteIndicadores.CUSPP = dr["CUSPP"].ToString();
                    reporteIndicadores.Solicitud = dr["Solicitud"].ToString();

                    if (dr["NroBeneficiario"] != DBNull.Value)
                        reporteIndicadores.NroBeneficiario = Convert.ToInt32(dr["NroBeneficiario"].ToString());

                    reporteIndicadores.TipoPensión = dr["TipoPensión"].ToString();
                    reporteIndicadores.Mail = dr["eMail"].ToString();
                    reporteIndicadores.ApellidoPaterno = dr["ApellidoPaterno"].ToString();
                    reporteIndicadores.ApellidoMaterno = dr["ApellidoMaterno"].ToString();
                    reporteIndicadores.Nombres = dr["Nombres"].ToString();

                    reporteIndicadores.ConsentimientoCdA = dr["ConsentimientoCdA"].ToString();
                    if (dr["FechaEnvioConsentimientoCdA"] != DBNull.Value)
                        reporteIndicadores.FechaEnvioConsentimientoCdA = Convert.ToDateTime(dr["FechaEnvioConsentimientoCdA"]).ToString("dd/MM/yyyy HH:mm:ss");
                    if (dr["FechaAceptacionConsentimientoCdA"] != DBNull.Value)
                        reporteIndicadores.FechaAceptacionConsentimientoCdA = Convert.ToDateTime(dr["FechaAceptacionConsentimientoCdA"]).ToString("dd/MM/yyyy HH:mm:ss");

                    reporteIndicadores.ConsentimientoVCTP = dr["ConsentimientoVCTP"].ToString();
                    if (dr["FechaEnvioConsentimientoVCTP"] != DBNull.Value)
                        reporteIndicadores.FechaEnvioConsentimientoVCTP = Convert.ToDateTime(dr["FechaEnvioConsentimientoVCTP"]).ToString("dd/MM/yyyy HH:mm:ss");
                    if (dr["FechaAceptacionConsentimientoVCTP"] != DBNull.Value)
                        reporteIndicadores.FechaAceptacionConsentimientoVCTP = Convert.ToDateTime(dr["FechaAceptacionConsentimientoVCTP"]).ToString("dd/MM/yyyy HH:mm:ss");
                    if (dr["FechaCierreComercial"] != DBNull.Value)
                        reporteIndicadores.FechaCierreComercial = Convert.ToDateTime(dr["FechaCierreComercial"]).ToString("dd/MM/yyyy");
                    listaReporteIndicadores.Add(reporteIndicadores);
                }
            }

            return listaReporteIndicadores;

        }

        public void Actualizar(ReporteIndicadores entity)
        {
            throw new NotImplementedException();
        }

        public void Eliminar(ReporteIndicadores entity)
        {
            throw new NotImplementedException();
        }

        public List<ReporteIndicadores> Listar()
        {
            throw new NotImplementedException();
        }

        public ReporteIndicadores ObtenerPorId(string Id)
        {
            throw new NotImplementedException();
        }

        public ReporteIndicadores ObtenerPorId(long Id)
        {
            throw new NotImplementedException();
        }

        public void Registrar(ReporteIndicadores entity)
        {
            throw new NotImplementedException();
        }
        
    }
}
