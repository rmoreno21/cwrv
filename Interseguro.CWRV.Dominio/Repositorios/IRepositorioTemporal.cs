using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioTemporal : IRepositorio<Temporal>
    {
        //List<Temporal> ListarPorTabla(string tabla);
    }
}
