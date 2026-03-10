using System.Collections.Generic;
using Interseguro.CWRV.Aplicacion.ModuloPrincipal;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interseguro.CWRV.Tests
{
    [TestClass]
    public class UsuarioServicioTests
    {
        [TestMethod]
        public void EliminarUsuario_CuandoSeInvoca_DebeInvocarEliminarEnRepositorioConElMismoUsuario()
        {
            // Arrange
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake();
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var usuario = new Usuario
            {
                Id = 1,
                NombreUsuario = "tester",
                CodigoEmpleado = "EMP001",
                Nombre = "Usuario de Prueba",
                Rol = "Admin"
            };

            // Act
            servicio.EliminarUsuario(usuario);

            // Assert
            Assert.IsTrue(repositorioUsuario.EliminarFueInvocado);
            Assert.AreSame(usuario, repositorioUsuario.UltimoUsuarioEliminado);
        }

        [TestMethod]
        public void CrearUsuario_CuandoSeInvoca_DebeInvocarRegistrarEnRepositorioConElMismoUsuario()
        {
            // Arrange
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake();
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var usuario = new Usuario
            {
                Id = 2,
                NombreUsuario = "nuevo.usuario",
                CodigoEmpleado = "EMP002",
                Nombre = "Nuevo Usuario",
                Rol = "Agente"
            };

            // Act
            servicio.CrearUsuario(usuario);

            // Assert
            Assert.IsTrue(repositorioUsuario.RegistrarFueInvocado);
            Assert.AreSame(usuario, repositorioUsuario.UltimoUsuarioRegistrado);
        }

        [TestMethod]
        public void ObtenerUsuarios_CuandoSeInvoca_DebeRetornarListadoDelRepositorio()
        {
            var usuariosEsperados = new List<Usuario> { new Usuario { Id = 1, NombreUsuario = "u1" } };
            var repositorioUsuario = new RepositorioUsuarioFake { UsuariosAListar = usuariosEsperados };
            var repositorioAgente = new RepositorioAgenteFake();
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var usuarios = servicio.ObtenerUsuarios();

            Assert.AreSame(usuariosEsperados, usuarios);
        }

        [TestMethod]
        public void ObtenerUsuario_CuandoSeInvoca_DebeRetornarUsuarioPorIdDelRepositorio()
        {
            var usuarioEsperado = new Usuario { Id = 25, NombreUsuario = "u25" };
            var repositorioUsuario = new RepositorioUsuarioFake { UsuarioPorId = usuarioEsperado };
            var repositorioAgente = new RepositorioAgenteFake();
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var usuario = servicio.ObtenerUsuario(25);

            Assert.AreSame(usuarioEsperado, usuario);
            Assert.AreEqual(25, repositorioUsuario.UltimoIdSolicitado);
        }

        [TestMethod]
        public void ListarAgentes_CuandoSeInvoca_DebeRetornarListadoDelRepositorio()
        {
            var agentesEsperados = new List<Agente> { new Agente() };
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake { AgentesAListar = agentesEsperados };
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var agentes = servicio.ListarAgentes("usuario", "rol");

            Assert.AreSame(agentesEsperados, agentes);
            Assert.AreEqual("usuario", repositorioAgente.UltimoUsuarioListar);
            Assert.AreEqual("rol", repositorioAgente.UltimoRolListar);
        }

        [TestMethod]
        public void ObtenerNumAgente_CuandoSeInvoca_DebeRetornarValorDelRepositorio()
        {
            var repositorioUsuario = new RepositorioUsuarioFake { NumeroAgente = "A001" };
            var repositorioAgente = new RepositorioAgenteFake();
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var numeroAgente = servicio.obtenerNumAgente("usuario");

            Assert.AreEqual("A001", numeroAgente);
            Assert.AreEqual("usuario", repositorioUsuario.UltimoCodigoUsuarioConsultado);
        }

        private class RepositorioUsuarioFake : IRepositorioUsuario
        {
            public bool EliminarFueInvocado { get; private set; }
            public Usuario? UltimoUsuarioEliminado { get; private set; }
            public bool RegistrarFueInvocado { get; private set; }
            public Usuario? UltimoUsuarioRegistrado { get; private set; }
            public List<Usuario> UsuariosAListar { get; set; } = new List<Usuario>();
            public Usuario UsuarioPorId { get; set; } = default!;
            public string NumeroAgente { get; set; } = string.Empty;
            public long UltimoIdSolicitado { get; private set; }
            public string UltimoCodigoUsuarioConsultado { get; private set; } = string.Empty;

            public void Eliminar(Usuario entity)
            {
                EliminarFueInvocado = true;
                UltimoUsuarioEliminado = entity;
            }

            public void Registrar(Usuario entity)
            {
                RegistrarFueInvocado = true;
                UltimoUsuarioRegistrado = entity;
            }

            public void Actualizar(Usuario entity) { }
            public Usuario ObtenerPorId(long Id)
            {
                UltimoIdSolicitado = Id;
                return UsuarioPorId;
            }

            public Usuario ObtenerPorId(string Id) => default!;
            public List<Usuario> Listar() => UsuariosAListar;
            public List<Usuario> ListarPorNombres(string nombres) => new List<Usuario>();
            public string obtenerNumAgente(string nombreUsuario)
            {
                UltimoCodigoUsuarioConsultado = nombreUsuario;
                return NumeroAgente;
            }
            public List<Usuario> Listar(string nombreUsuario, string idAgente) => new List<Usuario>();
        }

        private class RepositorioAgenteFake : IRepositorioAgente
        {
            public List<Agente> AgentesAListar { get; set; } = new List<Agente>();
            public string UltimoUsuarioListar { get; private set; } = string.Empty;
            public string UltimoRolListar { get; private set; } = string.Empty;

            public void Registrar(Agente entity) { }
            public void Actualizar(Agente entity) { }
            public void Eliminar(Agente entity) { }
            public Agente ObtenerPorId(long Id) => default!;
            public Agente ObtenerPorId(string Id) => default!;
            public Agente ObtenerPorId(int Id, string usuario) => default!;
            public List<Agente> Listar() => new List<Agente>();
            public List<Agente> Listar(string usuario, string rol)
            {
                UltimoUsuarioListar = usuario;
                UltimoRolListar = rol;
                return AgentesAListar;
            }
            public Agente ObtenerMS(string idAgente) => default!;
            public List<Agente> ObtenerAgenteDeudaAcom(string idAgente) => new List<Agente>();
            public Agente ObtenerSupervisorAgente(string usuario) => default!;
            public List<Agente> ObtenerJerarquiaAgente(int nivel, string usuario) => new List<Agente>();
            public List<Agente> ListarAgenteExterno(string gls_agente, string usuario) => new List<Agente>();
            public Agente ObtenerUltimoAgentePorCartera(string cartera, string usuario) => default!;
        }
    }
}
