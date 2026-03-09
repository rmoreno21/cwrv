using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioCiudad : IRepositorio<Ciudad>
    {
        Ciudad ObtenerDatos(string idCiudad);
        List<Ciudad> Listar(string idDepartamento);
    }
}
