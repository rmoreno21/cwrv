using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioRolDcom : IRepositorio<RolDcom>
    {
        
        List<RolDcom> ListarRolDcom(RolDcom rolDcom);
        //<GTIINI-10761>
        List<RolDcom> ListarRolDcomRPP(RolDcom rolDcom);
        //<GTIFIN-10761>
        List<RolDcom> ListaDcomEscenario(RolDcom rolDcom);

        //<INIGTI_7012_S27>
        List<RolDcom> ListarRangoDcomIFP(RolDcom rolDcom);
        //<FINGTI_7012_S27>

    }
}
