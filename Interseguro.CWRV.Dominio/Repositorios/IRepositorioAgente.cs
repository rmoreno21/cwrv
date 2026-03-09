using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioAgente : IRepositorio<Agente>
    {
        List<Agente> Listar(string usuario, string rol);
        Agente ObtenerMS(string idAgente);
        List<Agente> ObtenerAgenteDeudaAcom(string idAgente);
        Agente ObtenerSupervisorAgente(string usuario);
        void Registrar(Agente entity);
        void Actualizar(Agente entity);
        void Eliminar(Agente entity);
        Agente ObtenerPorId(int Id, string usuario);
        List<Agente> ObtenerJerarquiaAgente(int nivel, string usuario);
        List<Agente> ListarAgenteExterno(string gls_agente, string usuario);
        Agente ObtenerUltimoAgentePorCartera(string cartera, string usuario);
    }
}
