//<SRIINI06326>
using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioAdelantoComision: IRepositorio<AdelantoComision>
    {
        List<PorcentajeComision> ListarAdelantos();
        AdelantoComision ObtenerConfiguracionAdelanto(AdelantoComision parametros);
    }
}
//<SRIFIN06326> 