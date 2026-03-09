using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{

    public interface IRepositorioReporteCotizacion : IRepositorio<ReporteCotizacion>
    {
        
        List<ReporteCotizacion> ListarEtiquetas();
        
    }

}
