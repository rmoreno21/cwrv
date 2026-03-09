using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;


namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioUsuario: IRepositorio<Usuario>
    {
        List<Usuario> ListarPorNombres(string nombres);
        //<SRI.INI-20322_E2>
        string obtenerNumAgente(string nombreUsuario);
        List<Usuario> Listar(string nombreUsuario, string idAgente);
        //<SRI.FIN-20322_E2>
    }
}
