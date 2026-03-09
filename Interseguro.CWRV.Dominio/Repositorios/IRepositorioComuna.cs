using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioComuna : IRepositorio<Comuna>
    {
        Comuna ObtenerDatos(string idComuna);
        List<Comuna> Listar(string idCiudad);
    }
}
