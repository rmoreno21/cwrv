using System;
using System.Collections.Generic;
using Interseguro.CWRV.Aplicacion.ModuloPrincipal;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Interseguro.CWRV.Tests;

[TestClass]
public class CotizadorServicioTests
{
    [TestMethod]
    public void ObtenerPreCubo_CuandoSeInvoca_DebeRetornarReporteDelRepositorio()
    {
        var fixture = new Fixture();
        var reporteEsperado = new SDAReporte();
        fixture.RepositorioParametroGeneral
            .Setup(r => r.obtenerPreCubo(10, "CUSPP"))
            .Returns(reporteEsperado);
        var servicio = fixture.CreateSut();

        var reporte = servicio.obtenerPreCubo(10, "CUSPP");

        Assert.AreSame(reporteEsperado, reporte);
    }

    [TestMethod]
    public void ObtenerMSAgente_CuandoSeInvoca_DebeRetornarAgenteDelRepositorio()
    {
        var fixture = new Fixture();
        var agenteEsperado = new Agente();
        fixture.RepositorioAgente.Setup(r => r.ObtenerMS("AG001")).Returns(agenteEsperado);
        var servicio = fixture.CreateSut();

        var agente = servicio.ObtenerMSAgente("AG001");

        Assert.AreSame(agenteEsperado, agente);
    }

    [TestMethod]
    public void ObtenerDatosAfiliado_CuandoSeInvoca_DebeRetornarAfiliadoDelRepositorio()
    {
        var fixture = new Fixture();
        var afiliadoEsperado = new Afiliado();
        fixture.RepositorioAfiliado
            .Setup(r => r.ObtenerDatos("SOL1", "CUSPP1", "DNI", "12345678", "RP"))
            .Returns(afiliadoEsperado);
        var servicio = fixture.CreateSut();

        var afiliado = servicio.ObtenerDatosAfiliado("SOL1", "CUSPP1", "DNI", "12345678", "RP");

        Assert.AreSame(afiliadoEsperado, afiliado);
    }

    [TestMethod]
    public void ListarGrupoFamiliar_CuandoSeInvoca_DebeRetornarListaDelRepositorio()
    {
        var fixture = new Fixture();
        var gruposEsperados = new List<GrupoFamiliar> { new GrupoFamiliar() };
        fixture.RepositorioGrupoFamiliar.Setup(r => r.Listar("CUSPP1")).Returns(gruposEsperados);
        var servicio = fixture.CreateSut();

        var grupos = servicio.ListarGrupoFamiliar("CUSPP1");

        Assert.AreSame(gruposEsperados, grupos);
    }

    [TestMethod]
    public void ObtenerDatosGrupoFamiliar_CuandoSeInvoca_DebeRetornarGrupoDelRepositorio()
    {
        var fixture = new Fixture();
        var grupoEsperado = new GrupoFamiliar();
        fixture.RepositorioGrupoFamiliar
            .Setup(r => r.ObtenerDatos(5, "SOL1"))
            .Returns(grupoEsperado);
        var servicio = fixture.CreateSut();

        var grupo = servicio.ObtenerDatosGrupoFamiliar(5, "SOL1");

        Assert.AreSame(grupoEsperado, grupo);
    }

    [TestMethod]
    public void ListarSolicitudPorCuspp_CuandoSeInvoca_DebeRetornarListaDelRepositorio()
    {
        var fixture = new Fixture();
        var solicitudesEsperadas = new List<Solicitud> { new Solicitud() };
        fixture.RepositorioSolicitud.Setup(r => r.Listar("CUSPP1")).Returns(solicitudesEsperadas);
        var servicio = fixture.CreateSut();

        var solicitudes = servicio.ListarSolicitud("CUSPP1");

        Assert.AreSame(solicitudesEsperadas, solicitudes);
    }

    [TestMethod]
    public void ListarSolicitudPorLote_CuandoSeInvoca_DebeRetornarListaDelRepositorio()
    {
        var fixture = new Fixture();
        var solicitudesEsperadas = new List<Solicitud> { new Solicitud() };
        fixture.RepositorioSolicitud.Setup(r => r.Listar(99)).Returns(solicitudesEsperadas);
        var servicio = fixture.CreateSut();

        var solicitudes = servicio.ListarSolicitud(99);

        Assert.AreSame(solicitudesEsperadas, solicitudes);
    }

    [TestMethod]
    public void ListarSolicitudesPorFechaCierreAfp_CuandoSeInvoca_DebeRetornarListaDelRepositorio()
    {
        var fixture = new Fixture();
        var fechaInicio = new DateTime(2026, 1, 1);
        var fechaFin = new DateTime(2026, 1, 31);
        var solicitudesEsperadas = new List<Solicitud> { new Solicitud() };
        fixture.RepositorioSolicitud
            .Setup(r => r.ListarSolicitudesPorFechaCierreAFP(fechaInicio, fechaFin, 'S'))
            .Returns(solicitudesEsperadas);
        var servicio = fixture.CreateSut();

        var solicitudes = servicio.ListarSolicitudesPorFechaCierreAFP(fechaInicio, fechaFin, 'S');

        Assert.AreSame(solicitudesEsperadas, solicitudes);
    }

    [TestMethod]
    public void ListarConfirmaciones_CuandoSeInvoca_DebeRetornarListaDelRepositorio()
    {
        var fixture = new Fixture();
        var fechaInicio = new DateTime(2026, 2, 1);
        var fechaFin = new DateTime(2026, 2, 28);
        var solicitudesEsperadas = new List<Solicitud> { new Solicitud() };
        fixture.RepositorioSolicitud
            .Setup(r => r.ListarConfirmaciones(fechaInicio, fechaFin, 'N'))
            .Returns(solicitudesEsperadas);
        var servicio = fixture.CreateSut();

        var solicitudes = servicio.ListarConfirmaciones(fechaInicio, fechaFin, 'N');

        Assert.AreSame(solicitudesEsperadas, solicitudes);
    }

    [TestMethod]
    public void ObtenerXmlConfirmacionesCargaMeler_CuandoSeInvoca_DebeEjecutarFlujosDeModalidadAntesDeFallarPorDocumentoInvalido()
    {
        var fixture = new Fixture();
        fixture.RepositorioSolicitud
            .Setup(r => r.ListarSolicitudesConfirmacionMeler("<xml/>"))
            .Returns(
            [
                CrearSolicitudConfirmacion("SOL-RV", "CUSPP-RV", 1001, "RV", string.Empty, 100.1, 0, 0, 0, 500.5, new DateTime(2026, 3, 1)),
                CrearSolicitudConfirmacion("SOL-RTVD", "CUSPP-RTVD", 1002, "RTVD", string.Empty, 0, 200.2, 300.3, 10.1, 600.6, new DateTime(2026, 3, 2)),
                CrearSolicitudConfirmacion("SOL-RM", "CUSPP-RM", 1003, "RM", "*", 0, 999.9, 888.8, 20.2, 700.7, new DateTime(2026, 3, 3)),
                CrearSolicitudConfirmacion("SOL-RC", "CUSPP-RC", 1004, "RC", "S", 0, 400.4, 500.5, 30.3, 800.8, new DateTime(2026, 3, 4)),
                CrearSolicitudConfirmacion("SOL-RB", "CUSPP-RB", 1005, "RB", "S", 0, 600.6, 700.7, 40.4, 900.9, new DateTime(2026, 3, 5))
            ]);
        var servicio = fixture.CreateSut();

        Assert.ThrowsException<InvalidOperationException>(
            () => servicio.ObtenerXMLConfirmacionesCargaMeler("<xml/>"));
    }

    [TestMethod]
    public void ObtenerDatosAporteAdicional_CuandoSeInvoca_DebeRetornarAporteDelRepositorio()
    {
        var fixture = new Fixture();
        var aporteEsperado = new AporteAdicional();
        fixture.RepositorioAporteAdicional
            .Setup(r => r.ObtenerDatos("CUSPP1"))
            .Returns(aporteEsperado);
        var servicio = fixture.CreateSut();

        var aporte = servicio.ObtenerDatosAporteAdicional("CUSPP1");

        Assert.AreSame(aporteEsperado, aporte);
    }

    [TestMethod]
    public void ActualizarAporteAdicional_CuandoSeInvoca_DebeDelegarEnRepositorio()
    {
        var fixture = new Fixture();
        var aporte = new AporteAdicional();
        var servicio = fixture.CreateSut();

        servicio.ActualizarAporteAdicional(aporte, "tester");

        fixture.RepositorioAporteAdicional.Verify(r => r.Actualizar(aporte, "tester"), Times.Once);
    }

    [TestMethod]
    public void EliminarAporteAdicional_CuandoSeInvoca_DebeDelegarEnRepositorio()
    {
        var fixture = new Fixture();
        var aporte = new AporteAdicional();
        var servicio = fixture.CreateSut();

        servicio.EliminarAporteAdicional(aporte, "tester");

        fixture.RepositorioAporteAdicional.Verify(r => r.Eliminar(aporte, "tester"), Times.Once);
    }

    [TestMethod]
    public void ObtenerFlujos_CuandoSeInvoca_DebeRetornarFlujosDelRepositorio()
    {
        var fixture = new Fixture();
        var fecha = new DateTime(2026, 3, 10);
        var flujosEsperados = new List<FlujoMovimiento> { new FlujoMovimiento() };
        fixture.RepositorioFlujoMovimiento
            .Setup(r => r.ObtenerFlujos(fecha, "EVT", "ROL"))
            .Returns(flujosEsperados);
        var servicio = fixture.CreateSut();

        var flujos = servicio.ObtenerFlujos(fecha, "EVT", "ROL");

        Assert.AreSame(flujosEsperados, flujos);
    }

    [TestMethod]
    public void ValidaFlujoSolicitudRol_CuandoSeInvoca_DebeRetornarValorDelRepositorio()
    {
        var fixture = new Fixture();
        fixture.RepositorioFlujoMovimiento
            .Setup(r => r.ValidaFlujoSolicitudRol("SOL1", "APROBAR", "SUPERVISOR"))
            .Returns(true);
        var servicio = fixture.CreateSut();

        var resultado = servicio.ValidaFlujoSolicitudRol("SOL1", "SUPERVISOR", "APROBAR");

        Assert.IsTrue(resultado);
    }

    [TestMethod]
    public void ConsultarGestionVentas_CuandoSeInvoca_DebeRetornarListaDelRepositorio()
    {
        var fixture = new Fixture();
        var fechaInicio = new DateTime(2026, 1, 1);
        var fechaFin = new DateTime(2026, 1, 31);
        var gestionEsperada = new List<GestionVentas> { new GestionVentas() };
        fixture.RepositorioGestionVentas
            .Setup(r => r.ConsultarGestionVentas(fechaInicio, fechaFin, 1, 2, 3, "S", "RP", "CIA"))
            .Returns(gestionEsperada);
        var servicio = fixture.CreateSut();

        var gestion = servicio.ConsultarGestionVentas(fechaInicio, fechaFin, 1, 2, 3, "S", "RP", "CIA");

        Assert.AreSame(gestionEsperada, gestion);
    }

    [TestMethod]
    public void ListarRolAcom_CuandoSeInvoca_DebeRetornarListaDelRepositorio()
    {
        var fixture = new Fixture();
        var entrada = new RolAcom();
        var rolesEsperados = new List<RolAcom> { new RolAcom() };
        fixture.RepositorioRolAcom.Setup(r => r.ListarRolAcom(entrada)).Returns(rolesEsperados);
        var servicio = fixture.CreateSut();

        var roles = servicio.ListarRolAcom(entrada);

        Assert.AreSame(rolesEsperados, roles);
    }

    [TestMethod]
    public void RegistrarDescargaSolicitudes_CuandoSeInvoca_DebeRetornarLoteDelRepositorio()
    {
        var fixture = new Fixture();
        fixture.RepositorioSolicitud
            .Setup(r => r.RegistrarDescargaSolicitudes("<xml/>", "tester"))
            .Returns(123);
        var servicio = fixture.CreateSut();

        var lote = servicio.RegistrarDescargaSolicitudes("<xml/>", "tester");

        Assert.AreEqual(123, lote);
    }

    [TestMethod]
    public void ObtenerDatosAnticipo_CuandoSeInvoca_DebeRetornarAnticipoDelRepositorio()
    {
        var fixture = new Fixture();
        var anticipoEsperado = new Anticipo();
        fixture.RepositorioAnticipo.Setup(r => r.ObtenerDatos("SOL1")).Returns(anticipoEsperado);
        var servicio = fixture.CreateSut();

        var anticipo = servicio.ObtenerDatosAnticipo("SOL1");

        Assert.AreSame(anticipoEsperado, anticipo);
    }

    [TestMethod]
    public void SolicitudesHabilitadas_CuandoSeInvoca_DebeRetornarCadenaDelRepositorio()
    {
        var fixture = new Fixture();
        fixture.RepositorioSolicitud
            .Setup(r => r.SolicitudesHabilitadas(77, "S1,S2"))
            .Returns("S1,S2");
        var servicio = fixture.CreateSut();

        var resultado = servicio.SolicitudesHabilitadas(77, "S1,S2");

        Assert.AreEqual("S1,S2", resultado);
    }

    [TestMethod]
    public void ListarSolicitudRPPlus_CuandoSeInvoca_DebeRetornarListaDelRepositorio()
    {
        var fixture = new Fixture();
        var solicitudesEsperadas = new List<SolicitudRPPlus> { new SolicitudRPPlus() };
        fixture.RepositorioSolicitudRPPlus.Setup(r => r.Listar("CUSPP1")).Returns(solicitudesEsperadas);
        var servicio = fixture.CreateSut();

        var solicitudes = servicio.ListarSolicitudRPPlus("CUSPP1");

        Assert.AreSame(solicitudesEsperadas, solicitudes);
    }

    [TestMethod]
    public void ObtenerDatosSolicitudRPPlus_CuandoSeInvoca_DebeRetornarSolicitudDelRepositorio()
    {
        var fixture = new Fixture();
        var solicitudEsperada = new SolicitudRPPlus();
        fixture.RepositorioSolicitudRPPlus
            .Setup(r => r.ObtenerDatos("SOL1"))
            .Returns(solicitudEsperada);
        var servicio = fixture.CreateSut();

        var solicitud = servicio.ObtenerDatosSolicitudRPPlus("SOL1");

        Assert.AreSame(solicitudEsperada, solicitud);
    }

    private sealed class Fixture
    {
        public Mock<IRepositorioAfiliado> RepositorioAfiliado { get; } = new();
        public Mock<IRepositorioGrupoFamiliar> RepositorioGrupoFamiliar { get; } = new();
        public Mock<IRepositorioSolicitud> RepositorioSolicitud { get; } = new();
        public Mock<IRepositorioAporteAdicional> RepositorioAporteAdicional { get; } = new();
        public Mock<IRepositorioParametroGeneral> RepositorioParametroGeneral { get; } = new();
        public Mock<IRepositorioFlujoMovimiento> RepositorioFlujoMovimiento { get; } = new();
        public Mock<IRepositorioGestionVentas> RepositorioGestionVentas { get; } = new();
        public Mock<IRepositorioRolAcom> RepositorioRolAcom { get; } = new();
        public Mock<IRepositorioAnticipo> RepositorioAnticipo { get; } = new();
        public Mock<IRepositorioSolicitudRPPlus> RepositorioSolicitudRPPlus { get; } = new();
        public Mock<IRepositorioAgente> RepositorioAgente { get; } = new();

        public CotizadorServicio CreateSut()
        {
            return new CotizadorServicio(
                RepositorioAfiliado.Object,
                RepositorioGrupoFamiliar.Object,
                RepositorioSolicitud.Object,
                RepositorioAporteAdicional.Object,
                Mock.Of<IRepositorioSolicitudRP>(),
                Mock.Of<IRepositorioLote>(),
                Mock.Of<IRepositorioSolicitudEscenario>(),
                Mock.Of<IRepositorioCita>(),
                Mock.Of<IRepositorioTipoMovimiento>(),
                RepositorioAnticipo.Object,
                Mock.Of<IRepositorioActividad>(),
                Mock.Of<IRepositorioSeguimiento>(),
                Mock.Of<IRepositorioSupervision>(),
                RepositorioParametroGeneral.Object,
                Mock.Of<IRepositorioEscenario>(),
                Mock.Of<IRepositorioAdelantoComision>(),
                Mock.Of<IRepositorioDescuentoComision>(),
                RepositorioRolAcom.Object,
                Mock.Of<IRepositorioRolDcom>(),
                Mock.Of<IRepositorioRolDtra>(),
                RepositorioSolicitudRPPlus.Object,
                RepositorioFlujoMovimiento.Object,
                Mock.Of<IRepositorioCuotasTra>(),
                RepositorioGestionVentas.Object,
                RepositorioAgente.Object,
                Mock.Of<IRepositorioCausalPoliza>(),
                Mock.Of<IRepositorioEmisionPoliza>(),
                Mock.Of<IRepositorioDireccion>(),
                Mock.Of<IRepositorioSolicitudIFP>(),
                Mock.Of<IRepositorioReporteIndicadores>(),
                Mock.Of<IRepositorioReportes>(),
                Mock.Of<IRepositorioEstadoCivil>(),
                Mock.Of<IRepositorioNacionalidad>(),
                Mock.Of<IRepositorioProfesion>(),
                Mock.Of<IRepositorioMotorCalculo>());
        }
    }

    private static Solicitud CrearSolicitudConfirmacion(
        string idSolicitud,
        string cuspp,
        int numeroPoliza,
        string modalidad,
        string cotiza,
        double primeraPensionRv,
        double primeraPensionRt,
        double primeraPensionRvd,
        double primaUnicaAfpEess,
        double primaUnicaEess,
        DateTime fechaDevengue)
    {
        return new Solicitud
        {
            Id = idSolicitud,
            NumeroPoliza = numeroPoliza,
            FechaDevengue = fechaDevengue,
            Afiliado = new Afiliado
            {
                CUSPP = cuspp
            },
            Cotizaciones =
            [
                new Cotizacion
                {
                    Modalidad = new Modalidad { Id = modalidad },
                    Cotiza = cotiza,
                    PrimeraPensionRV = primeraPensionRv,
                    PrimeraPensionRT = primeraPensionRt,
                    PrimeraPensionRVD = primeraPensionRvd,
                    PrimaUnicaAFPEESS = primaUnicaAfpEess,
                    PrimaUnicaEESS = primaUnicaEess,
                    MontoCia = primaUnicaEess
                }
            ]
        };
    }
}
