using System;
using System.Collections.Generic;
using Interseguro.CWRV.Aplicacion.ModuloPrincipal;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Interseguro.CWRV.Tests;

[TestClass]
public class GeneralesServicioTests
{
    [TestMethod]
    public void ObtenerCombobox_CuandoSeInvoca_DebeRetornarDatosDelRepositorio()
    {
        var fixture = new Fixture();
        var esperado = new List<List<Parametro>> { new List<Parametro> { new Parametro() } };
        fixture.RepositorioParametro.Setup(x => x.ObtenerCombobox()).Returns(esperado);
        var servicio = fixture.CreateSut();

        var resultado = servicio.ObtenerCombobox();

        Assert.AreSame(esperado, resultado);
    }

    [TestMethod]
    public void ConsultasDeParametros_CuandoSeInvocan_DebenRetornarDatosDelRepositorio()
    {
        var fixture = new Fixture();
        var simuladores = new List<Parametro> { new Parametro() };
        var porTabla = new List<Parametro> { new Parametro() };
        var parametros = new List<Parametro> { new Parametro() };
        var nroBancos = new List<Parametro> { new Parametro() };
        var tipoCta = new List<Parametro> { new Parametro() };
        var tipoIdentificacion = new List<Parametro> { new Parametro() };
        var comboboxIfp = new List<List<Parametro>> { new List<Parametro> { new Parametro() } };

        fixture.RepositorioParametro.Setup(x => x.ObtenerParametrosSimuladores()).Returns(simuladores);
        fixture.RepositorioParametro.Setup(x => x.ObtenerParametrosPorTabla("TAB")).Returns(porTabla);
        fixture.RepositorioParametro.Setup(x => x.ObtenerParametros("TAB2")).Returns(parametros);
        fixture.RepositorioParametro.Setup(x => x.ObtenerNroBancos("TAB3", "B", "C")).Returns(nroBancos);
        fixture.RepositorioParametro.Setup(x => x.ObtenerTipoCtaBancos("BCP", "01")).Returns(tipoCta);
        fixture.RepositorioParametro.Setup(x => x.ObtenerTipoIdentificacion("1", "DNI", "DN")).Returns(tipoIdentificacion);
        fixture.RepositorioParametro.Setup(x => x.ObtenerComboboxIFP()).Returns(comboboxIfp);
        var servicio = fixture.CreateSut();

        Assert.AreSame(simuladores, servicio.ObtenerParametrosSimuladores());
        Assert.AreSame(porTabla, servicio.ObtenerParametrosPorTabla("TAB"));
        Assert.AreSame(parametros, servicio.ObtenerParametros("TAB2"));
        Assert.AreSame(nroBancos, servicio.ObtenerNroBancos("TAB3", "B", "C"));
        Assert.AreSame(tipoCta, servicio.ObtenerTipoCtaBancos("BCP", "01"));
        Assert.AreSame(tipoIdentificacion, servicio.ObtenerTipoIdentificacion("1", "DNI", "DN"));
        Assert.AreSame(comboboxIfp, servicio.ObtenerComboboxIFP());
    }

    [TestMethod]
    public void ConsultasDeUbigeo_CuandoSeInvocan_DebenRetornarDatosDelRepositorio()
    {
        var fixture = new Fixture();
        var ciudad = new Ciudad();
        var ciudades = new List<Ciudad> { new Ciudad() };
        var comuna = new Comuna();
        var comunas = new List<Comuna> { new Comuna() };
        fixture.RepositorioCiudad.Setup(x => x.ObtenerDatos("01")).Returns(ciudad);
        fixture.RepositorioCiudad.Setup(x => x.Listar("15")).Returns(ciudades);
        fixture.RepositorioComuna.Setup(x => x.ObtenerDatos("001")).Returns(comuna);
        fixture.RepositorioComuna.Setup(x => x.Listar("1501")).Returns(comunas);
        var servicio = fixture.CreateSut();

        Assert.AreSame(ciudad, servicio.ObtenerDatosCiudad("01"));
        Assert.AreSame(ciudades, servicio.ListarCiudad("15"));
        Assert.AreSame(comuna, servicio.ObtenerDatosComuna("001"));
        Assert.AreSame(comunas, servicio.ListarComuna("1501"));
    }

    [TestMethod]
    public void OperacionesDireccionTelefonoProductoYLog_CuandoSeInvocan_DebenDelegarAlRepositorio()
    {
        var fixture = new Fixture();
        var direcciones = new List<Direccion> { new Direccion() };
        var direccion = new Direccion();
        var telefonos = new List<Telefono> { new Telefono() };
        var telefono = new Telefono();
        var productos = new List<Producto> { new Producto() };
        var log = new LogBD();
        fixture.RepositorioDireccion.Setup(x => x.Listar("CUSPP")).Returns(direcciones);
        fixture.RepositorioDireccion.Setup(x => x.ObtenerDatos(10)).Returns(direccion);
        fixture.RepositorioTelefono.Setup(x => x.Listar("CUSPP")).Returns(telefonos);
        fixture.RepositorioTelefono.Setup(x => x.ObtenerDatos(8)).Returns(telefono);
        fixture.RepositorioProducto.Setup(x => x.Listar("CAT")).Returns(productos);
        var servicio = fixture.CreateSut();

        Assert.AreSame(direcciones, servicio.ListarDireccion("CUSPP"));
        Assert.AreSame(direccion, servicio.ObtenerDatosDireccion(10));
        Assert.AreSame(telefonos, servicio.ListarTelefono("CUSPP"));
        Assert.AreSame(telefono, servicio.ObtenerDatosTelefono(8));
        Assert.AreSame(productos, servicio.ListarProducto("CAT"));

        servicio.RegistrarDireccion(direccion);
        servicio.ActualizarDireccion(direccion);
        servicio.EliminarDireccion(direccion);
        servicio.RegistrarTelefono(telefono);
        servicio.ActualizarTelefono(telefono);
        servicio.EliminarTelefono(telefono);
        servicio.RegistrarLog(log);

        fixture.RepositorioDireccion.Verify(x => x.Registrar(direccion), Times.Once);
        fixture.RepositorioDireccion.Verify(x => x.Actualizar(direccion), Times.Once);
        fixture.RepositorioDireccion.Verify(x => x.Eliminar(direccion), Times.Once);
        fixture.RepositorioTelefono.Verify(x => x.Registrar(telefono), Times.Once);
        fixture.RepositorioTelefono.Verify(x => x.Actualizar(telefono), Times.Once);
        fixture.RepositorioTelefono.Verify(x => x.Eliminar(telefono), Times.Once);
        fixture.RepositorioLog.Verify(x => x.Registrar(log), Times.Once);
    }

    [TestMethod]
    public void OperacionesMontoCicYParametro_CuandoSeInvocan_DebenDelegarAlRepositorio()
    {
        var fixture = new Fixture();
        var montos = new List<MontoCIC> { new MontoCIC() };
        var monto = new MontoCIC();
        var parametro = new Parametro();
        fixture.RepositorioConfiguracionMontoCic.Setup(x => x.Listar()).Returns(montos);
        var servicio = fixture.CreateSut();

        Assert.AreSame(montos, servicio.ListarMontoCIC());
        servicio.RegistrarMontoCIC(monto);
        servicio.ActualizarMontoCIC(monto);
        servicio.EliminarMontoCIC(monto);
        servicio.ActualizarParametro(parametro);

        fixture.RepositorioConfiguracionMontoCic.Verify(x => x.Registrar(monto), Times.Once);
        fixture.RepositorioConfiguracionMontoCic.Verify(x => x.Actualizar(monto), Times.Once);
        fixture.RepositorioConfiguracionMontoCic.Verify(x => x.Eliminar(monto), Times.Once);
        fixture.RepositorioParametro.Verify(x => x.Actualizar(parametro), Times.Once);
    }

    [TestMethod]
    public void ConsultasTra_CuandoSeInvocan_DebenRetornarDatosDelRepositorio()
    {
        var fixture = new Fixture();
        var traDefault = new List<ParametroEspecial> { new ParametroEspecial() };
        var traMaxMin = new List<ParametroEspecial> { new ParametroEspecial() };
        var fecha = new DateTime(2026, 3, 10);
        fixture.RepositorioParametro.Setup(x => x.ObtenerTraDefault("SOL1")).Returns(traDefault);
        fixture.RepositorioParametro.Setup(x => x.ObtenerTasaMaximaTraMinima("SOL1", fecha)).Returns(traMaxMin);
        var servicio = fixture.CreateSut();

        Assert.AreSame(traDefault, servicio.ObtenerTraDefault("SOL1"));
        Assert.AreSame(traMaxMin, servicio.ObtenerTasaMaximaTraMinima("SOL1", fecha));
    }

    [TestMethod]
    public void RegistrarCotizaValPar_CuandoSeInvoca_DebeRegistrarEnRepositorio()
    {
        var fixture = new Fixture();
        var parametros = new List<ParametroEspecial> { new ParametroEspecial() };
        var servicio = fixture.CreateSut();

        servicio.RegistrarCotizaValPar(parametros, "tester");

        fixture.RepositorioParametro.Verify(x => x.Registrar(parametros, "tester"), Times.Once);
    }

    [TestMethod]
    public void OperacionesContratos_CuandoSeInvocan_DebenDelegarAlRepositorio()
    {
        var fixture = new Fixture();
        var contratos = new List<Contrato> { new Contrato() };
        var contrato = new Contrato();
        fixture.RepositorioParametro.Setup(x => x.ListarContratosCotizaciones("tester")).Returns(contratos);
        var servicio = fixture.CreateSut();

        Assert.AreSame(contratos, servicio.ListarContratosCotizaciones("tester"));
        servicio.ActualizarContratoCotizacion(contrato);
        servicio.InsertarContratoCotizacion(contrato);
        servicio.EliminarContratoCotizacion(15, "tester");

        fixture.RepositorioParametro.Verify(x => x.ActualizarContratoCotizacion(contrato), Times.Once);
        fixture.RepositorioParametro.Verify(x => x.InsertarContratoCotizacion(contrato), Times.Once);
        fixture.RepositorioParametro.Verify(x => x.EliminarContratoCotizacion(15, "tester"), Times.Once);
    }

    [TestMethod]
    public void OperacionesCarta_CuandoSeInvocan_DebenRetornarYRegistrarEnRepositorio()
    {
        var fixture = new Fixture();
        var carta = new RviCarta();
        fixture.RepositorioCarta.Setup(x => x.ObtenerDatos("SOL1", 1, "tester")).Returns(carta);
        var servicio = fixture.CreateSut();

        var resultado = servicio.ObtenerDatosCarta("SOL1", 1, "tester");
        servicio.RegistrarCarta(carta);

        Assert.AreSame(carta, resultado);
        fixture.RepositorioCarta.Verify(x => x.Registrar(carta), Times.Once);
    }

    [TestMethod]
    public void ValidacionesCarga_CuandoSeInvocan_DebenRetornarValorDelRepositorio()
    {
        var fixture = new Fixture();
        var fecha = new DateTime(2026, 2, 1);
        fixture.RepositorioCargaControlCdA.Setup(x => x.Validar(fecha, "usr")).Returns(true);
        fixture.RepositorioCargaLocalidadVctp.Setup(x => x.Validar(fecha, "usr")).Returns(false);
        fixture.RepositorioCargaControlVctp.Setup(x => x.Validar(fecha, "usr")).Returns(true);
        var servicio = fixture.CreateSut();

        Assert.IsTrue(servicio.ValidarCargaControlCdA(fecha, "usr"));
        Assert.IsFalse(servicio.ValidarCargaLocalidadVCTP(fecha, "usr"));
        Assert.IsTrue(servicio.ValidarCargaControlVCTP(fecha, "usr"));
    }

    [TestMethod]
    public void CargarReporteControlCdA_CuandoSeInvoca_DebeEliminarYRegistrarTodosLosItems()
    {
        var fixture = new Fixture();
        var lista = new List<CargaControlCdA> { new CargaControlCdA(), new CargaControlCdA() };
        var servicio = fixture.CreateSut();

        servicio.CargarReporteControlCdA(lista);

        fixture.RepositorioCargaControlCdA.Verify(x => x.Eliminar(lista[0]), Times.Once);
        fixture.RepositorioCargaControlCdA.Verify(x => x.Registrar(It.IsAny<CargaControlCdA>()), Times.Exactly(2));
    }

    [TestMethod]
    public void CargarReporteLocalidadVctp_CuandoSeInvoca_DebeEliminarYRegistrarTodosLosItems()
    {
        var fixture = new Fixture();
        var lista = new List<CargaLocalidadVCTP> { new CargaLocalidadVCTP(), new CargaLocalidadVCTP() };
        var servicio = fixture.CreateSut();

        servicio.CargarReporteLocalidadVCTP(lista);

        fixture.RepositorioCargaLocalidadVctp.Verify(x => x.Eliminar(lista[0]), Times.Once);
        fixture.RepositorioCargaLocalidadVctp.Verify(x => x.Registrar(It.IsAny<CargaLocalidadVCTP>()), Times.Exactly(2));
    }

    [TestMethod]
    public void CargarReporteControlVctp_CuandoSeInvoca_DebeEliminarYRegistrarTodosLosItems()
    {
        var fixture = new Fixture();
        var lista = new List<CargaControlVCTP> { new CargaControlVCTP(), new CargaControlVCTP() };
        var servicio = fixture.CreateSut();

        servicio.CargarReporteControlVCTP(lista);

        fixture.RepositorioCargaControlVctp.Verify(x => x.Eliminar(lista[0]), Times.Once);
        fixture.RepositorioCargaControlVctp.Verify(x => x.Registrar(It.IsAny<CargaControlVCTP>()), Times.Exactly(2));
    }

    [TestMethod]
    public void ObtenerTipoCambioYListarRviDireccion_CuandoSeInvocan_DebenRetornarDatosDelRepositorio()
    {
        var fixture = new Fixture();
        var fecha = new DateTime(2026, 3, 10);
        var rviDirecciones = new List<Direccion> { new Direccion() };
        fixture.RepositorioParametroGeneral.Setup(x => x.ObtenerTipoCambio("USD", fecha, "tester")).Returns(3.75);
        fixture.RepositorioDireccion.Setup(x => x.Listar("SOL1", null, "tester")).Returns(rviDirecciones);
        var servicio = fixture.CreateSut();

        var tipoCambio = servicio.ObtenerTipoCambio("USD", fecha, "tester");
        var direcciones = servicio.ListarRviDireccion("SOL1", "tester");

        Assert.AreEqual(3.75, tipoCambio);
        Assert.AreSame(rviDirecciones, direcciones);
    }

    private sealed class Fixture
    {
        public Mock<IRepositorioParametro> RepositorioParametro { get; } = new();
        public Mock<IRepositorioParametroGeneral> RepositorioParametroGeneral { get; } = new();
        public Mock<IRepositorioDireccion> RepositorioDireccion { get; } = new();
        public Mock<IRepositorioTelefono> RepositorioTelefono { get; } = new();
        public Mock<IRepositorioCiudad> RepositorioCiudad { get; } = new();
        public Mock<IRepositorioComuna> RepositorioComuna { get; } = new();
        public Mock<IRepositorioProducto> RepositorioProducto { get; } = new();
        public Mock<IRepositorioLogBD> RepositorioLog { get; } = new();
        public Mock<IRepositorioConfiguracionMontoCIC> RepositorioConfiguracionMontoCic { get; } = new();
        public Mock<IRepositorioTemporal> RepositorioTemporal { get; } = new();
        public Mock<IRepositorioCarta> RepositorioCarta { get; } = new();
        public Mock<IRepositorioCargaControlCdA> RepositorioCargaControlCdA { get; } = new();
        public Mock<IRepositorioCargaLocalidadVCTP> RepositorioCargaLocalidadVctp { get; } = new();
        public Mock<IRepositorioCargaControlVCTP> RepositorioCargaControlVctp { get; } = new();

        public GeneralesServicio CreateSut()
        {
            return new GeneralesServicio(
                RepositorioParametro.Object,
                RepositorioParametroGeneral.Object,
                RepositorioDireccion.Object,
                RepositorioTelefono.Object,
                RepositorioCiudad.Object,
                RepositorioComuna.Object,
                RepositorioProducto.Object,
                RepositorioConfiguracionMontoCic.Object,
                RepositorioLog.Object,
                RepositorioTemporal.Object,
                RepositorioCarta.Object,
                RepositorioCargaControlCdA.Object,
                RepositorioCargaLocalidadVctp.Object,
                RepositorioCargaControlVctp.Object);
        }
    }
}
