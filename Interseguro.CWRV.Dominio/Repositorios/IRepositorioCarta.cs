using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioCarta : IRepositorio<RviCarta>
    {
        RviCarta ObtenerDatos(string solicitud, int correlativo, string usuario);
    }
}
