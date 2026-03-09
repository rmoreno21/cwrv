using System.Collections.Generic;
using Interseguro.CWRV.Aplicacion.ModuloPrincipal;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interseguro.CWRV.Tests
{
    [TestClass]
    public class SmokeTests
    {
        [TestMethod]
        public void SolicitarAcceso_CuandoIpPermitida_DebeRetornarAccesoExitosoYRegistrarSolicitud()
        {
            // Arrange
            var repositorioSolicitud = new RepositorioSolicitudAccesoFake();
            var repositorioIp = new RepositorioIPPermitidaFake
            {
                IpPermitida = new IPPermitida
                {
                    IP = "127.0.0.1",
                    SegundosExpiracion = 120
                }
            };

            var servicio = new SeguridadServicio(repositorioSolicitud, repositorioIp);
            var solicitud = new SolicitudAcceso
            {
                Usuario = "tester",
                IP = "127.0.0.1"
            };

            // Act
            var acceso = servicio.SolicitarAcceso(solicitud);

            // Assert
            Assert.IsNotNull(acceso);
            Assert.AreEqual(0, acceso.Codigo);
            Assert.AreEqual("Solicitud de acceso aceptada.", acceso.Mensaje);
            Assert.IsFalse(string.IsNullOrWhiteSpace(acceso.Token));
            Assert.AreEqual(30, acceso.Token.Length);
            Assert.AreEqual(120, solicitud.ExpiracionToken);
            Assert.AreEqual(solicitud.Token, acceso.Token);
            Assert.IsTrue(repositorioSolicitud.RegistrarFueInvocado);
            Assert.AreSame(solicitud, repositorioSolicitud.UltimaSolicitudRegistrada);
        }

        private class RepositorioSolicitudAccesoFake : IRepositorioSolicitudAcceso
        {
            public bool RegistrarFueInvocado { get; private set; }
            public SolicitudAcceso? UltimaSolicitudRegistrada { get; private set; }

            public void Registrar(SolicitudAcceso entity)
            {
                RegistrarFueInvocado = true;
                UltimaSolicitudRegistrada = entity;
            }

            public void Actualizar(SolicitudAcceso entity) { }
            public void Eliminar(SolicitudAcceso entity) { }
            public SolicitudAcceso ObtenerPorId(long Id) => default!;
            public SolicitudAcceso ObtenerPorId(string Id) => default!;
            public List<SolicitudAcceso> Listar() => new List<SolicitudAcceso>();
            public SolicitudAcceso ValidarToken(string token) => default!;
        }

        private class RepositorioIPPermitidaFake : IRepositorioIPPermitida
        {
            public IPPermitida? IpPermitida { get; set; }

            public IPPermitida ObtenerDatos(string ip)
            {
                return IpPermitida != null && IpPermitida.IP == ip ? IpPermitida : default!;
            }

            public void Registrar(IPPermitida entity) { }
            public void Actualizar(IPPermitida entity) { }
            public void Eliminar(IPPermitida entity) { }
            public IPPermitida ObtenerPorId(long Id) => default!;
            public IPPermitida ObtenerPorId(string Id) => default!;
            public List<IPPermitida> Listar() => new List<IPPermitida>();
        }
    }
}