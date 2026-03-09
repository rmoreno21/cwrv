using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioTelefono : IRepositorio<Telefono>
    {
        List<Telefono> Listar(string cuspp);
        Telefono ObtenerDatos(int idTelefono);
    }
}
