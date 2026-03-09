using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioProducto : IRepositorio<Producto>
    {
        List<Producto> Listar(string idCategoria);
    }
}
