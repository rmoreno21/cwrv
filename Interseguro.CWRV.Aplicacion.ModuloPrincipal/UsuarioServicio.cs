using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Aplicacion.ModuloPrincipal
{
    public class UsuarioServicio : IUsuarioServicio
    {
        private readonly IRepositorioUsuario repositorioUsuario;
        private readonly IRepositorioAgente repositorioAgente;

        public UsuarioServicio(IRepositorioUsuario repositorioUsuario, IRepositorioAgente repositorioAgente)
        {
            this.repositorioUsuario = repositorioUsuario;
            this.repositorioAgente = repositorioAgente;
        }

        public List<Agente> ListarAgentes(string usuario, string rol)
        {
            var agentes = repositorioAgente.Listar(usuario, rol);
            return agentes;
        }

        public string obtenerNumAgente(string codUsuario)
        {
            string numAgente = "";
            numAgente = repositorioUsuario.obtenerNumAgente(codUsuario);
            return numAgente;
        }

        public List<Usuario> ListarUsuario(string nombreUsuario, string idAgente)
        {
            var usuarios = repositorioUsuario.Listar(nombreUsuario, idAgente);
            return usuarios;
        }

        public List<Usuario> ObtenerUsuarios()
        {
            var usuarios = repositorioUsuario.Listar();
            return usuarios;
        }

        public Usuario ObtenerUsuario(int id)
        {
            var usuario = repositorioUsuario.ObtenerPorId(id);
            return usuario;
        }

        public void CrearUsuario(Usuario usuario)
        {
            repositorioUsuario.Registrar(usuario);
        }

        public void EliminarUsuario(Usuario usuario)
        {
            repositorioUsuario.Eliminar(usuario);
        }

        public Agente ObtenerSupervisorAgente(string usuario)
        {
            var agente = repositorioAgente.ObtenerSupervisorAgente(usuario);
            return agente;
        }

        public void CrearAgente(Agente agente)
        {
            repositorioAgente.Registrar(agente);
        }

        public void ActualizarAgente(Agente agente)
        {
            repositorioAgente.Actualizar(agente);
        }

        public void EliminarAgente(Agente agente)
        {
            repositorioAgente.Eliminar(agente);
        }

        public Agente ObtenerAgente(int IdAgente, string usuario)
        {
            var agente = repositorioAgente.ObtenerPorId(IdAgente, usuario);
            return agente;
        }

        public List<Agente> ObtenerJerarquiaAgente(int nivelAgente, string usuario)
        {
            var agentes = repositorioAgente.ObtenerJerarquiaAgente(nivelAgente, usuario);
            return agentes;
        }

        public List<Agente> ListarAgenteExterno(string gls_agente, string usuario)
        {
            var agentes = repositorioAgente.ListarAgenteExterno(gls_agente, usuario);
            return agentes;
        }

        public Agente ObtenerUltimoAgentePorCartera(string cartera, string usuario)
        {
            var agentes = repositorioAgente.ObtenerUltimoAgentePorCartera(cartera, usuario);
            return agentes;
        }
    }
}
