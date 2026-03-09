using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioActividad: IRepositorio<Actividad>
    {
        List<Actividad> Listar(string cuspp);
    }
}
