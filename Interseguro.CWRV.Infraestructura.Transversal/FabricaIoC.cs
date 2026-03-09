using Microsoft.Practices.Unity;
using Interseguro.CWRV.Infraestructura.Datos;
using Interseguro.CWRV.Infraestructura.Datos.Repositorios;
using Interseguro.CWRV.Dominio;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Aplicacion.ModuloPrincipal;

namespace Interseguro.CWRV.Infraestructura.Transversal
{
    public class FabricaIoC
    {
        private static readonly FabricaIoC _contenedor = new FabricaIoC();
        private readonly IUnityContainer _unityContainer;

        private FabricaIoC()
        {
            _unityContainer = new UnityContainer();

            _unityContainer.RegisterType<ICotizadorServicio, CotizadorServicio>();
            _unityContainer.RegisterType<IGeneralesServicio, GeneralesServicio>();
            _unityContainer.RegisterType<IUsuarioServicio, UsuarioServicio>();
            _unityContainer.RegisterType<ISeguridadServicio, SeguridadServicio>();

            _unityContainer.RegisterType<IRepositorioAfiliado, RepositorioAfiliado>();
            _unityContainer.RegisterType<IRepositorioGrupoFamiliar, RepositorioGrupoFamiliar>();
            _unityContainer.RegisterType<IRepositorioSolicitud, RepositorioSolicitud>();
            _unityContainer.RegisterType<IRepositorioCarta, RepositorioCarta>();
            _unityContainer.RegisterType<IRepositorioCargaControlCdA, RepositorioCargaControlCdA>();
            _unityContainer.RegisterType<IRepositorioCargaLocalidadVCTP, RepositorioCargaLocalidadVCTP>();
            _unityContainer.RegisterType<IRepositorioCargaControlVCTP, RepositorioCargaControlVCTP>();
            _unityContainer.RegisterType<IRepositorioAporteAdicional, RepositorioAporteAdicional>();
            _unityContainer.RegisterType<IRepositorioSolicitudRP, RepositorioSolicitudRP>();
            _unityContainer.RegisterType<IRepositorioLote, RepositorioLote>();
            _unityContainer.RegisterType<IRepositorioSolicitudEscenario, RepositorioSolicitudEscenario>();
            _unityContainer.RegisterType<IRepositorioCita, RepositorioCita>();
            _unityContainer.RegisterType<IRepositorioTipoMovimiento, RepositorioTipoMovimiento>();
            _unityContainer.RegisterType<IRepositorioAnticipo, RepositorioAnticipo>();
            _unityContainer.RegisterType<IRepositorioActividad, RepositorioActividad>();
            _unityContainer.RegisterType<IRepositorioSeguimiento, RepositorioSeguimiento>();
            _unityContainer.RegisterType<IRepositorioSupervision, RepositorioSupervision>();
            _unityContainer.RegisterType<IRepositorioDireccion, RepositorioDireccion>();
            _unityContainer.RegisterType<IRepositorioCiudad, RepositorioCiudad>();
            _unityContainer.RegisterType<IRepositorioComuna, RepositorioComuna>();
            _unityContainer.RegisterType<IRepositorioTelefono, RepositorioTelefono>();
            _unityContainer.RegisterType<IRepositorioParametro, RepositorioParametro>();
            _unityContainer.RegisterType<IRepositorioProducto, RepositorioProducto>();
            _unityContainer.RegisterType<IRepositorioLogBD, RepositorioLogBD>();
            _unityContainer.RegisterType<IRepositorioUsuario, RepositorioUsuario>();
            _unityContainer.RegisterType<IRepositorioAgente, RepositorioAgente>();
            _unityContainer.RegisterType<IRepositorioConfiguracionMontoCIC, RepositorioConfiguracionMontoCIC>();
            _unityContainer.RegisterType<IRepositorioParametroGeneral, RepositorioParametroGeneral>();
            _unityContainer.RegisterType<IRepositorioEscenario, RepositorioEscenario>();
            _unityContainer.RegisterType<IRepositorioAdelantoComision, RepositorioAdelantoComision>();
            _unityContainer.RegisterType<IRepositorioDescuentoComision, RepositorioDescuentoComision>();
            _unityContainer.RegisterType<IRepositorioRolAcom, RepositorioRolAcom>();
            _unityContainer.RegisterType<IRepositorioRolDcom, RepositorioRolDcom>();
            _unityContainer.RegisterType<IRepositorioRolDtra, RepositorioRolDtra>();
            _unityContainer.RegisterType<IRepositorioSolicitudAcceso, RepositorioSolicitudAcceso>();
            _unityContainer.RegisterType<IRepositorioIPPermitida, RepositorioIPPermitida>();
            _unityContainer.RegisterType<IRepositorioSolicitudRPPlus, RepositorioSolicitudRPPlus>();
            _unityContainer.RegisterType<IRepositorioFlujoMovimiento, RepositorioFlujoMovimiento>();
            _unityContainer.RegisterType<IRepositorioCuotasTra, RepositorioCuotasTra>();
            _unityContainer.RegisterType<IRepositorioGestionVentas, RepositorioGestionVentas>();
            _unityContainer.RegisterType<IRepositorioCausalPoliza, RepositorioCausalPoliza>();
            _unityContainer.RegisterType<IRepositorioTemporal, RepositorioTemporal>();
            _unityContainer.RegisterType<IRepositorioCausalPoliza, RepositorioCausalPoliza>();
            _unityContainer.RegisterType<IRepositorioEmisionPoliza, RepositorioEmisionPoliza>();
            _unityContainer.RegisterType<IRepositorioSolicitudIFP, RepositorioSolicitudIFP>();
            _unityContainer.RegisterType<IRepositorioReporteIndicadores, RepositorioReporteIndicadores>();
            _unityContainer.RegisterType<IRepositorioReportes, RepositorioReportes>();
            _unityContainer.RegisterType<IRepositorioEstadoCivil, RepositorioEstadoCivil>();
            _unityContainer.RegisterType<IRepositorioProfesion, RepositorioProfesion>();
            _unityContainer.RegisterType<IRepositorioNacionalidad, RepositorioNacionalidad>();
            _unityContainer.RegisterType<IRepositorioMotorCalculo, RepositorioMotorCalculo>();
        }

        public static FabricaIoC Contenedor
        {
            get { return _contenedor; }
        }

        /// <summary>
        ///   Crear una instancia de un objeto que implemente un tipo TServicio.
        /// </summary>
        /// <typeparam name = "TServicio">Tipo de servicio que deseamos resolver</typeparam>
        /// <returns></returns>
        public TServicio Resolver<TServicio>() where TServicio : class
        {
            return _unityContainer.Resolve<TServicio>();
        }
    }
}
