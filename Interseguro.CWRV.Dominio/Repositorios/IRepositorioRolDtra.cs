using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioRolDtra : IRepositorio<RolDtra>
    {
        List<RolDtra> ListarRolDtra(RolDtra rolDtra);
    }
}
