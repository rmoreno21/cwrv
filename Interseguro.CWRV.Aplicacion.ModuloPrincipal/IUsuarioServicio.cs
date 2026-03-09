using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Aplicacion.ModuloPrincipal
{
    public interface IUsuarioServicio
    {
        List<Agente> ListarAgentes(string usuario, string rol);
        string obtenerNumAgente(string codUsuario);
        List<Usuario> ListarUsuario(string nombreUsuario, string idAgente);
        Agente ObtenerSupervisorAgente(string usuario);
        void CrearAgente(Agente agente);
        void ActualizarAgente(Agente agente);
        void EliminarAgente(Agente agente);
        Agente ObtenerAgente(int IdAgente, string usuario);
        List<Agente> ObtenerJerarquiaAgente(int nivelAgente, string usuario);
        List<Agente> ListarAgenteExterno(string gls_agente, string usuario);
        Agente ObtenerUltimoAgentePorCartera(string cartera, string usuario);
    }
}
