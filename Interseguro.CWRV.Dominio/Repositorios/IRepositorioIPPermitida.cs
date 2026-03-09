using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioIPPermitida : IRepositorio<IPPermitida>
    {
        IPPermitida ObtenerDatos(string ip);
    }
}
