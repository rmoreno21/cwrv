using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioReportes
    {
        List<ConsentimientosAgrupadosAge> ListarConsentimientosAgrupados(string periodo, string usuario);
        List<FirmaDigitalDashboard> ObtenerFirmaDigitalesDashboard(string fechaInicio, string fechaFin, string usuario);
        List<PolizasDashboard> ObtenerPolizasDashboard(string fechaInicio, string fechaFin, string usuario);
        List<ReporteTrazabilidad> ObtenerTrazabilidad(string fechaInicio, string fechaFin, string idProceso, string usuario);
        ReporteRecalculoCotizacion ObtenerReporteRecalculo(List<ReporteRecalculoCotizacion> listaRecalculoCotizacion);
        List<ReporteCotizacionesGanadas> ObtenerReporteCotizacionesGanadas(int numLote);
        DataSet ListarReporteIndicadoresVCTP(DateTime fecPeriodo, string codUsername, string codRol);
        DataSet ListarReporteIndicadoresCDA(DateTime fecPeriodo, string codUsername, string codRol);
        DataSet ActualizarReporteIndicadoresCDA(DataSet dataReporte, List<ResponseVTiger> lstEstadoCRM);
    }
}
