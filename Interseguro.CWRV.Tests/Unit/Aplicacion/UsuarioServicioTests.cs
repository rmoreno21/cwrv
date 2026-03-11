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

        [TestMethod]
        public void ListarUsuario_CuandoSeInvoca_DebeRetornarListadoFiltradoDelRepositorio()
        {
            var usuariosEsperados = new List<Usuario> { new Usuario { Id = 10, NombreUsuario = "filtro" } };
            var repositorioUsuario = new RepositorioUsuarioFake { UsuariosFiltradosAListar = usuariosEsperados };
            var repositorioAgente = new RepositorioAgenteFake();
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var usuarios = servicio.ListarUsuario("nombre", "AG001");

            Assert.AreSame(usuariosEsperados, usuarios);
            Assert.AreEqual("nombre", repositorioUsuario.UltimoNombreUsuarioListar);
            Assert.AreEqual("AG001", repositorioUsuario.UltimoIdAgenteListar);
        }

        [TestMethod]
        public void ObtenerSupervisorAgente_CuandoSeInvoca_DebeRetornarSupervisorDelRepositorio()
        {
            var supervisorEsperado = new Agente();
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake { SupervisorAgente = supervisorEsperado };
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var supervisor = servicio.ObtenerSupervisorAgente("agente.usuario");

            Assert.AreSame(supervisorEsperado, supervisor);
            Assert.AreEqual("agente.usuario", repositorioAgente.UltimoUsuarioSupervisor);
        }

        [TestMethod]
        public void CrearAgente_CuandoSeInvoca_DebeRegistrarAgenteEnRepositorio()
        {
            var agente = new Agente();
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake();
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            servicio.CrearAgente(agente);

            Assert.IsTrue(repositorioAgente.RegistrarFueInvocado);
            Assert.AreSame(agente, repositorioAgente.UltimoAgenteRegistrado);
        }

        [TestMethod]
        public void ActualizarAgente_CuandoSeInvoca_DebeActualizarAgenteEnRepositorio()
        {
            var agente = new Agente();
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake();
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            servicio.ActualizarAgente(agente);

            Assert.IsTrue(repositorioAgente.ActualizarFueInvocado);
            Assert.AreSame(agente, repositorioAgente.UltimoAgenteActualizado);
        }

        [TestMethod]
        public void EliminarAgente_CuandoSeInvoca_DebeEliminarAgenteEnRepositorio()
        {
            var agente = new Agente();
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake();
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            servicio.EliminarAgente(agente);

            Assert.IsTrue(repositorioAgente.EliminarFueInvocado);
            Assert.AreSame(agente, repositorioAgente.UltimoAgenteEliminado);
        }

        [TestMethod]
        public void ObtenerAgente_CuandoSeInvoca_DebeRetornarAgentePorIdYUsuarioDelRepositorio()
        {
            var agenteEsperado = new Agente();
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake { AgentePorIdYUsuario = agenteEsperado };
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var agente = servicio.ObtenerAgente(99, "usr");

            Assert.AreSame(agenteEsperado, agente);
            Assert.AreEqual(99, repositorioAgente.UltimoIdAgenteConsultado);
            Assert.AreEqual("usr", repositorioAgente.UltimoUsuarioAgenteConsultado);
        }

        [TestMethod]
        public void ObtenerJerarquiaAgente_CuandoSeInvoca_DebeRetornarJerarquiaDelRepositorio()
        {
            var jerarquiaEsperada = new List<Agente> { new Agente(), new Agente() };
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake { JerarquiaAgentes = jerarquiaEsperada };
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var jerarquia = servicio.ObtenerJerarquiaAgente(2, "usr");

            Assert.AreSame(jerarquiaEsperada, jerarquia);
            Assert.AreEqual(2, repositorioAgente.UltimoNivelJerarquia);
            Assert.AreEqual("usr", repositorioAgente.UltimoUsuarioJerarquia);
        }

        [TestMethod]
        public void ListarAgenteExterno_CuandoSeInvoca_DebeRetornarListadoDelRepositorio()
        {
            var externosEsperados = new List<Agente> { new Agente() };
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake { AgentesExternos = externosEsperados };
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var externos = servicio.ListarAgenteExterno("texto", "usr");

            Assert.AreSame(externosEsperados, externos);
            Assert.AreEqual("texto", repositorioAgente.UltimoFiltroAgenteExterno);
            Assert.AreEqual("usr", repositorioAgente.UltimoUsuarioAgenteExterno);
        }

        [TestMethod]
        public void ObtenerUltimoAgentePorCartera_CuandoSeInvoca_DebeRetornarAgenteDelRepositorio()
        {
            var agenteEsperado = new Agente();
            var repositorioUsuario = new RepositorioUsuarioFake();
            var repositorioAgente = new RepositorioAgenteFake { UltimoAgentePorCartera = agenteEsperado };
            var servicio = new UsuarioServicio(repositorioUsuario, repositorioAgente);

            var agente = servicio.ObtenerUltimoAgentePorCartera("cartera-x", "usr");

            Assert.AreSame(agenteEsperado, agente);
            Assert.AreEqual("cartera-x", repositorioAgente.UltimaCarteraConsultada);
            Assert.AreEqual("usr", repositorioAgente.UltimoUsuarioCartera);
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
            public List<Usuario> UsuariosFiltradosAListar { get; set; } = new List<Usuario>();
            public string UltimoNombreUsuarioListar { get; private set; } = string.Empty;
            public string UltimoIdAgenteListar { get; private set; } = string.Empty;

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
            public List<Usuario> Listar(string nombreUsuario, string idAgente)
            {
                UltimoNombreUsuarioListar = nombreUsuario;
                UltimoIdAgenteListar = idAgente;
                return UsuariosFiltradosAListar;
            }
        }

        private class RepositorioAgenteFake : IRepositorioAgente
        {
            public List<Agente> AgentesAListar { get; set; } = new List<Agente>();
            public string UltimoUsuarioListar { get; private set; } = string.Empty;
            public string UltimoRolListar { get; private set; } = string.Empty;
            public bool RegistrarFueInvocado { get; private set; }
            public bool ActualizarFueInvocado { get; private set; }
            public bool EliminarFueInvocado { get; private set; }
            public Agente? UltimoAgenteRegistrado { get; private set; }
            public Agente? UltimoAgenteActualizado { get; private set; }
            public Agente? UltimoAgenteEliminado { get; private set; }
            public Agente SupervisorAgente { get; set; } = default!;
            public string UltimoUsuarioSupervisor { get; private set; } = string.Empty;
            public Agente AgentePorIdYUsuario { get; set; } = default!;
            public int UltimoIdAgenteConsultado { get; private set; }
            public string UltimoUsuarioAgenteConsultado { get; private set; } = string.Empty;
            public List<Agente> JerarquiaAgentes { get; set; } = new List<Agente>();
            public int UltimoNivelJerarquia { get; private set; }
            public string UltimoUsuarioJerarquia { get; private set; } = string.Empty;
            public List<Agente> AgentesExternos { get; set; } = new List<Agente>();
            public string UltimoFiltroAgenteExterno { get; private set; } = string.Empty;
            public string UltimoUsuarioAgenteExterno { get; private set; } = string.Empty;
            public Agente UltimoAgentePorCartera { get; set; } = default!;
            public string UltimaCarteraConsultada { get; private set; } = string.Empty;
            public string UltimoUsuarioCartera { get; private set; } = string.Empty;

            public void Registrar(Agente entity)
            {
                RegistrarFueInvocado = true;
                UltimoAgenteRegistrado = entity;
            }

            public void Actualizar(Agente entity)
            {
                ActualizarFueInvocado = true;
                UltimoAgenteActualizado = entity;
            }

            public void Eliminar(Agente entity)
            {
                EliminarFueInvocado = true;
                UltimoAgenteEliminado = entity;
            }
            public Agente ObtenerPorId(long Id) => default!;
            public Agente ObtenerPorId(string Id) => default!;
            public Agente ObtenerPorId(int Id, string usuario)
            {
                UltimoIdAgenteConsultado = Id;
                UltimoUsuarioAgenteConsultado = usuario;
                return AgentePorIdYUsuario;
            }
            public List<Agente> Listar() => new List<Agente>();
            public List<Agente> Listar(string usuario, string rol)
            {
                UltimoUsuarioListar = usuario;
                UltimoRolListar = rol;
                return AgentesAListar;
            }
            public Agente ObtenerMS(string idAgente) => default!;
            public List<Agente> ObtenerAgenteDeudaAcom(string idAgente) => new List<Agente>();
            public Agente ObtenerSupervisorAgente(string usuario)
            {
                UltimoUsuarioSupervisor = usuario;
                return SupervisorAgente;
            }
            public List<Agente> ObtenerJerarquiaAgente(int nivel, string usuario)
            {
                UltimoNivelJerarquia = nivel;
                UltimoUsuarioJerarquia = usuario;
                return JerarquiaAgentes;
            }
            public List<Agente> ListarAgenteExterno(string gls_agente, string usuario)
            {
                UltimoFiltroAgenteExterno = gls_agente;
                UltimoUsuarioAgenteExterno = usuario;
                return AgentesExternos;
            }
            public Agente ObtenerUltimoAgentePorCartera(string cartera, string usuario)
            {
                UltimaCarteraConsultada = cartera;
                UltimoUsuarioCartera = usuario;
                return UltimoAgentePorCartera;
            }
        }
    }
}
