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

        private class RepositorioUsuarioFake : IRepositorioUsuario
        {
            public bool EliminarFueInvocado { get; private set; }
            public Usuario UltimoUsuarioEliminado { get; private set; }
            public bool RegistrarFueInvocado { get; private set; }
            public Usuario UltimoUsuarioRegistrado { get; private set; }

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
            public Usuario ObtenerPorId(long Id) => default!;
            public Usuario ObtenerPorId(string Id) => default!;
            public List<Usuario> Listar() => new List<Usuario>();
            public List<Usuario> ListarPorNombres(string nombres) => new List<Usuario>();
            public string obtenerNumAgente(string nombreUsuario) => string.Empty;
            public List<Usuario> Listar(string nombreUsuario, string idAgente) => new List<Usuario>();
        }

        private class RepositorioAgenteFake : IRepositorioAgente
        {
            public void Registrar(Agente entity) { }
            public void Actualizar(Agente entity) { }
            public void Eliminar(Agente entity) { }
            public Agente ObtenerPorId(long Id) => default!;
            public Agente ObtenerPorId(string Id) => default!;
            public Agente ObtenerPorId(int Id, string usuario) => default!;
            public List<Agente> Listar() => new List<Agente>();
            public List<Agente> Listar(string usuario, string rol) => new List<Agente>();
            public Agente ObtenerMS(string idAgente) => default!;
            public List<Agente> ObtenerAgenteDeudaAcom(string idAgente) => new List<Agente>();
            public Agente ObtenerSupervisorAgente(string usuario) => default!;
            public List<Agente> ObtenerJerarquiaAgente(int nivel, string usuario) => new List<Agente>();
            public List<Agente> ListarAgenteExterno(string gls_agente, string usuario) => new List<Agente>();
            public Agente ObtenerUltimoAgentePorCartera(string cartera, string usuario) => default!;
        }
    }
}
