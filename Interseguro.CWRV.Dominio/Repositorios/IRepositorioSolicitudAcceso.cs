using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioSolicitudAcceso: IRepositorio<SolicitudAcceso>
    {
        SolicitudAcceso ValidarToken(string token);
    }
}
