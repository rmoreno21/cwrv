using System;
using System.Collections.Generic;
using Interseguro.CWRV.Aplicacion.ModuloPrincipal;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Interseguro.CWRV.Tests;

[TestClass]
public class SeguridadServicioTests
{
    [TestMethod]
    public void SolicitarAcceso_CuandoIpNoPermitida_DebeRetornarCodigoDenegadoSinRegistrar()
    {
        var repositorioSolicitud = new RepositorioSolicitudAccesoFake();
        var repositorioIp = new RepositorioIPPermitidaFake();
        var servicio = new SeguridadServicio(repositorioSolicitud, repositorioIp);
        var solicitud = new SolicitudAcceso { Usuario = "tester", IP = "10.10.10.10" };

        var acceso = servicio.SolicitarAcceso(solicitud);

        Assert.IsNotNull(acceso);
        Assert.AreEqual(-2, acceso.Codigo);
        Assert.IsNull(acceso.Token);
        Assert.IsFalse(repositorioSolicitud.RegistrarFueInvocado);
    }

    [TestMethod]
    public void SolicitarAcceso_CuandoRepositorioIpLanzaExcepcion_DebeRetornarCodigoErrorTecnico()
    {
        var repositorioSolicitud = new RepositorioSolicitudAccesoFake();
        var repositorioIp = new RepositorioIPPermitidaFake
        {
            ExcepcionAlObtenerDatos = new InvalidOperationException("Fallo de repositorio")
        };
        var servicio = new SeguridadServicio(repositorioSolicitud, repositorioIp);
        var solicitud = new SolicitudAcceso { Usuario = "tester", IP = "127.0.0.1" };

        var acceso = servicio.SolicitarAcceso(solicitud);

        Assert.AreEqual(-9, acceso.Codigo);
        Assert.AreEqual("Fallo de repositorio", acceso.Mensaje);
        Assert.IsNull(acceso.Token);
    }

    [TestMethod]
    public void ValidarToken_CuandoIpPermitida_DebeRetornarSolicitudDelRepositorio()
    {
        var solicitudEsperada = new SolicitudAcceso { Usuario = "tester", IP = "127.0.0.1", Token = "abc" };
        var repositorioSolicitud = new RepositorioSolicitudAccesoFake { SolicitudValidada = solicitudEsperada };
        var repositorioIp = new RepositorioIPPermitidaFake
        {
            IpPermitida = new IPPermitida { IP = "127.0.0.1", SegundosExpiracion = 30 }
        };
        var servicio = new SeguridadServicio(repositorioSolicitud, repositorioIp);

        var solicitud = servicio.ValidarToken("abc", "127.0.0.1");

        Assert.AreSame(solicitudEsperada, solicitud);
        Assert.AreEqual("abc", repositorioSolicitud.UltimoTokenValidado);
    }

    [TestMethod]
    public void ValidarToken_CuandoIpNoPermitida_DebeLanzarExcepcion()
    {
        var repositorioSolicitud = new RepositorioSolicitudAccesoFake();
        var repositorioIp = new RepositorioIPPermitidaFake();
        var servicio = new SeguridadServicio(repositorioSolicitud, repositorioIp);

        var ex = Assert.ThrowsException<Exception>(() => servicio.ValidarToken("abc", "10.10.10.10"));

        StringAssert.Contains(ex.Message, "no está autorizada");
    }

    [TestMethod]
    public void ActualizarSolicitudAcceso_CuandoSeInvoca_DebeDelegarEnRepositorio()
    {
        var repositorioSolicitud = new RepositorioSolicitudAccesoFake();
        var repositorioIp = new RepositorioIPPermitidaFake();
        var servicio = new SeguridadServicio(repositorioSolicitud, repositorioIp);
        var solicitud = new SolicitudAcceso { Usuario = "tester", IP = "127.0.0.1" };

        servicio.ActualizarSolicitudAcceso(solicitud);

        Assert.IsTrue(repositorioSolicitud.ActualizarFueInvocado);
        Assert.AreSame(solicitud, repositorioSolicitud.UltimaSolicitudActualizada);
    }

    private class RepositorioSolicitudAccesoFake : IRepositorioSolicitudAcceso
    {
        public bool RegistrarFueInvocado { get; private set; }
        public bool ActualizarFueInvocado { get; private set; }
        public SolicitudAcceso? UltimaSolicitudActualizada { get; private set; }
        public string? UltimoTokenValidado { get; private set; }
        public SolicitudAcceso? SolicitudValidada { get; set; }

        public void Registrar(SolicitudAcceso entity) => RegistrarFueInvocado = true;

        public void Actualizar(SolicitudAcceso entity)
        {
            ActualizarFueInvocado = true;
            UltimaSolicitudActualizada = entity;
        }

        public SolicitudAcceso ValidarToken(string token)
        {
            UltimoTokenValidado = token;
            return SolicitudValidada!;
        }

        public void Eliminar(SolicitudAcceso entity) { }
        public SolicitudAcceso ObtenerPorId(long Id) => default!;
        public SolicitudAcceso ObtenerPorId(string Id) => default!;
        public List<SolicitudAcceso> Listar() => new();
    }

    private class RepositorioIPPermitidaFake : IRepositorioIPPermitida
    {
        public IPPermitida? IpPermitida { get; set; }
        public Exception? ExcepcionAlObtenerDatos { get; set; }

        public IPPermitida ObtenerDatos(string ip)
        {
            if (ExcepcionAlObtenerDatos != null)
            {
                throw ExcepcionAlObtenerDatos;
            }

            return IpPermitida != null && IpPermitida.IP == ip ? IpPermitida : default!;
        }

        public void Registrar(IPPermitida entity) { }
        public void Actualizar(IPPermitida entity) { }
        public void Eliminar(IPPermitida entity) { }
        public IPPermitida ObtenerPorId(long Id) => default!;
        public IPPermitida ObtenerPorId(string Id) => default!;
        public List<IPPermitida> Listar() => new();
    }
}
