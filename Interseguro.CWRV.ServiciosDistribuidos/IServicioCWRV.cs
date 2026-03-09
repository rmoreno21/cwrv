using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Aplicacion.ModuloPrincipal;
using Interseguro.CWRV.Infraestructura.Transversal;

using System.Data;
using System.Net.Mail;

namespace Interseguro.CWRV.ServiciosDistribuidos
{
    [ServiceContract]
    public interface IServicioCWRV
    {
        [OperationContract]
        SDAReporte obtenerPreCubo(int numeroAgente, string cuspp);

        //<GTIINI-6623>
        [OperationContract]
        Agente ObtenerMSAgente(string idAgente);
        //<GTIFIN-6623>

        [OperationContract]
        List<Afiliado> ListarAfiliado(string apellidoPaterno, string apellidoMaterno, string nombres, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros);

        [OperationContract]
        Afiliado ObtenerDatosAfiliado(string solicitud, string cuspp, string tipoIdentificacion, string numIdentificacion, string producto);

        //<GTI.INI-29372>
        [OperationContract]
        AporteAdicional ObtenerDatosAporteAdicional(string CUSPP);

        [OperationContract]
        Respuesta ActualizarAporteAdicional(AporteAdicional aporte, string usuario);

        [OperationContract]
        Respuesta EliminarAporteAdicional(AporteAdicional aporte, string usuario);
        //<GTI.FIN-29372>

        [OperationContract]
        Respuesta RegistrarAfiliado(Afiliado afiliado, string usuario, ref string numCUSPP);

        [OperationContract]
        Respuesta ActualizarAfiliado(Afiliado afiliado);

        [OperationContract]
        Ciudad ObtenerDatosCiudad(string idCiudad);

        [OperationContract]
        List<Ciudad> ListarCiudad(string idDepartamento);

        [OperationContract]
        Comuna ObtenerDatosComuna(string idComuna);

        [OperationContract]
        List<Comuna> ListarComuna(string idCiudad);

        [OperationContract]
        List<List<Parametro>> ObtenerCombobox();

        [OperationContract]
        List<Parametro> ObtenerParametrosSimuladores();

        //<SRI.INI-20322_E2>
        [OperationContract]
        List<Parametro> ObtenerParametrosPorTabla(string codTabla);
        //<SRI.FIN-20322_E2>

        [OperationContract]
        List<Direccion> ListarDireccion(string cuspp);

        [OperationContract]
        Direccion ObtenerDatosDireccion(int idDireccion);

        [OperationContract]
        void RegistrarDireccion(Direccion direccion);

        [OperationContract]
        void ActualizarDireccion(Direccion direccion);

        [OperationContract]
        void EliminarDireccion(Direccion direccion);

        [OperationContract]
        List<Telefono> ListarTelefono(string cuspp);

        [OperationContract]
        Telefono ObtenerDatosTelefono(int idTelefono);

        [OperationContract]
        Respuesta RegistrarTelefono(Telefono telefono);

        [OperationContract]
        Respuesta ActualizarTelefono(Telefono telefono);

        [OperationContract]
        void EliminarTelefono(Telefono telefono);

        [OperationContract]
        List<GrupoFamiliar> ListarGrupoFamiliar(string cuspp);

        [OperationContract]
        GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar, string num_solicitud); //<INI.GTI_7012_V13>

        [OperationContract]
        Respuesta RegistrarGrupoFamiliar(GrupoFamiliar grupo);

        [OperationContract]
        Respuesta ActualizarGrupoFamiliar(GrupoFamiliar grupo);

        //<SRI.INI_20322_E2>
        [OperationContract]
        List<SolicitudEscenario> ListarSolicitudEscenario(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol);

        //<INIGTI_4081>
        [OperationContract]
        List<SolicitudEscenario> ListarSolicitudEscenarioCambios(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol);
        //<FINGTI_4081>

        [OperationContract]
        SolicitudEscenario ObtenerDatosSolicitudEscenario(string numSolicitud, string codUserName, string codRol);

        [OperationContract]
        Respuesta RegistrarSolicitudEscenario(ref SolicitudEscenario solicitudEscenario);

        [OperationContract]
        Respuesta ActualizarSolicitudEscenario(ref SolicitudEscenario solicitudEscenario, bool cotizar);

        [OperationContract]
        Respuesta RegistrarSolicitudEscenarioExtraoficial(ref SolicitudEscenario solicitudEscenario);

        [OperationContract]
        List<Cita> ListarCita(Cita citaIn);

        [OperationContract]
        List<RolAzmanTipoMovimiento> ObtenerTipoMovimientoPorRolAzman(string codRol);

        [OperationContract]
        List<CotizacionMovimiento> ObtenerCotizacionTipoMovimientoPorSolicitud(string numSolicitud);

        [OperationContract]
        Respuesta RegistrarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario);
        //<SRI.FIN_20322_E2>

        [OperationContract]
        List<Solicitud> ListarSolicitud(string cuspp);

        [OperationContract]
        List<Solicitud> ListarSolicitudesPorFechaCierreAFP(DateTime fechaInicio, DateTime fechaFin, char enviada);

        [OperationContract]
        List<Solicitud> ListarConfirmaciones(DateTime fechaInicio, DateTime fechaFin, char enviada);

        [OperationContract]
        Solicitud ObtenerDatosSolicitud(string idSolicitud, DateTime fecCotizacion);

        [OperationContract]
        Respuesta RegistrarSolicitud(ref Solicitud solicitud);

        [OperationContract]
        Respuesta ActualizarSolicitud(ref Solicitud solicitud);

        [OperationContract]
        Respuesta ActualizarSolicitudesCargaMeler(string xml, string usuario);

        [OperationContract]
        Respuesta ActualizarSolicitudesConfirmacionMeler(string xml, string usuario);

        [OperationContract]
        Solicitud ObtenerCotizacionRecalculo(string numeroSolicitud, string usuario);

        [OperationContract]
        Respuesta CotizarRecalculo(string numSolicitud, DateTime fechaCotizacion, double montoCIC, double tipoCambio, string tipoCalculo, string usuario);
        [OperationContract]
        RviCarta ObtenerDatosCarta(string solicitud, int correlativo, string usuario);
        [OperationContract]
        Respuesta RegistrarCarta(RviCarta carta);

        [OperationContract]
        bool ValidarCargaControlCdA(DateTime fecPeriodo, string codUsuario);

        [OperationContract]
        bool ValidarCargaLocalidadVCTP(DateTime fecPeriodo, string codUsuario);

        [OperationContract]
        bool ValidarCargaControlVCTP(DateTime fecPeriodo, string codUsuario);

        [OperationContract]
        Respuesta CargarReporteControlCdA(List<CargaControlCdA> listaCargaControlCdA);

        [OperationContract]
        Respuesta CargarReporteLocalidadVCTP(List<CargaLocalidadVCTP> listaCargaLocalidadVCTP);

        [OperationContract]
        Respuesta CargarReporteControlVCTP(List<CargaControlVCTP> listaCargaControlVCTP);

        [OperationContract]
        Respuesta ListarReporteIndicadoresVCTP(DateTime fecPeriodo, string codUsername, string codRol);

        [OperationContract]
        Respuesta ListarReporteIndicadoresCDA(DateTime fecPeriodo, string codUsername, string codRol);

        //<SRIINI20322>
        [OperationContract]
        Respuesta CotizarOficial(string idSolicitud, DateTime fechaCotizacion, string usuario);

        [OperationContract]
        Respuesta ObtenerMontoACOM(string solicitud, double acom, long cotizacion);

        [OperationContract]
        Respuesta ActualizarValidacion(string idSolicitud, string tipoValidacion, string valor, string usuario);

        [OperationContract]
        List<Actividad> ListarActividad(string cuspp);

        [OperationContract]
        List<Producto> ListarProducto(string idCategoria);

        [OperationContract]
        Respuesta RegistrarLog(LogBD logbd);

        [OperationContract]
        List<Agente> ListarAgente(string usuario, string rol);

        //<SRI.INI-20322_E2>
        [OperationContract]
        string obtenerNumAgente(string codUsuario);

        [OperationContract]
        List<Usuario> ListarUsuario(string nombreUsuario, string idAgente);
        //<SRI.FIN-20322_E2>

        [OperationContract]
        List<Seguimiento> ListarSeguimiento(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros);

        [OperationContract]
        List<Seguimiento> ListarExcelSeguimiento(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar);

        [OperationContract]
        List<Supervision> ListarSupervision(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros);

        [OperationContract]
        List<Supervision> ListarExcelSupervision(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar);

        [OperationContract]
        List<ParametroCotizadorWS> ObtenerParametrosCotizacion(string idSolicitud, DateTime fechaCotizacion, Int64? numCorrelativo, int anhosAdicionales);

        [OperationContract]
        Cotizacion Cotizar(string idSolicitud, DateTime fechaCotizacion, Int64 numCorrelativo, int anhosAdicionales, double nuevoCIC);

        [OperationContract]
        Cotizacion CotizarConParametros(ParametroCotizadorWS p);

        //<INIGTI_754>
        [OperationContract]
        ParametroCotizadorWS ObtenerParametroCotizacionWS(string idSolicitud, DateTime fechaCotizacion, Int64? numCorrelativo, int anhosAdicionales);
        //<FINGTI_754>

        /******/

        //[OperationContract]
        //Respuesta Cotizar(string idSolicitud, string usuario);


        //<SRIINI06326>
        #region Monto CIC

        [OperationContract]
        List<MontoCIC> ListarMontoCIC();

        [OperationContract]
        Respuesta RegistrarMontoCIC(MontoCIC entity);

        [OperationContract]
        Respuesta ActualizarMontoCIC(MontoCIC entity);

        [OperationContract]
        Respuesta EliminarMontoCIC(MontoCIC entity);

        #endregion

        #region Reporte de escenarios

        [OperationContract]
        Respuesta GenerarReporteEscenarios(string idSolicitud, DateTime fechaCotizacion, string usuario, string maxAcom);

        [OperationContract]
        DataSet ObtenerReporteEscenarios(string idSolicitud, string usuario);

        #endregion
        //<SRIFIN06326>


        //<SRIINI10693>
        #region ROLACOM
        [OperationContract]
        List<RolAcom> ListarRolAcom(RolAcom rolAcom);
        [OperationContract]
        List<RolAcom> ListaAcomEscenario(RolAcom rolAcom);
        #endregion
        //<SRIFIN10693>

        //<SRI.INI-20322_E2>
        #region ROLDCOM
        [OperationContract]
        List<RolDcom> ListarRolDcom(RolDcom rolDcom);
        //<GTIINI-10761>
        [OperationContract]
        List<RolDcom> ListarRolDcomRPP(RolDcom rolDcom);
        //<GTIFIN-10761>
        [OperationContract]
        List<RolDcom> ListaDcomEscenario(RolDcom rolDcom);
        #endregion
        //<SRI.FIN-20322_E2>

        //<SRIINI20322>
        #region ROLDTRA
        [OperationContract]
        List<RolDtra> ListarRolDtra(RolDtra rolDtra);
        #endregion
        //<SRIFIN20322>

        //<SRIINI20322>
        #region Token de Acceso
        [OperationContract]
        SolicitudAcceso ValidarToken(string token);
        [OperationContract]
        Respuesta ActualizarSolicitudAcceso(SolicitudAcceso solicitud);
        #endregion

        #region Correo Electrónico
        [OperationContract]
        Respuesta EnviarCorreoElectronico(CorreoElectronico correo);

        [OperationContract]
        Respuesta EnviarCorreoElectronicoPoliza(CorreoElectronico correo);


        [OperationContract]
        void EnviarCorreoElectronicoAsincrono(CorreoElectronico correo);

        #endregion

        #region MELER
        [OperationContract]
        Respuesta RegistrarDescargaSolicitudes(string xml, string usuario, ref int lote);
        [OperationContract]
        List<Solicitud> RegistrarDescargaResultados(string xml, string usuario, ref int lote);
        [OperationContract]
        List<Lote> ListarLotePorNumero(int numero);
        [OperationContract]
        List<Lote> ListarLoteResultadoPorNumero(int numero);
        [OperationContract]
        List<Lote> ListarLotePorFecha(DateTime fechaCierreInicial, DateTime fechaCierreFinal);
        [OperationContract]
        List<Lote> ListarLoteResultadoPorFecha(DateTime fechaCierreInicial, DateTime fechaCierreFinal);
        [OperationContract]
        List<Solicitud> ListarSolicitudesPorLote(int lote);
        #endregion

        #region Anticipo
        [OperationContract]
        Anticipo ObtenerDatosAnticipo(string solicitud);
        [OperationContract]
        Anticipo ObtenerDatosAnticipoAceptacion(string solicitud, string agente);
        [OperationContract]
        Anticipo ObtenerDatosAnticipoCondiciones(string solicitud);
        [OperationContract]
        List<Anticipo> ListarAnticipoAceptacion(string solicitud, string num_agente, string fechaInicio, string fechaFin, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar);
        [OperationContract]
        Respuesta RegistrarAnticipoAceptacion(Anticipo anticipo);
        #endregion
        //<SRIFIN20322>

        //<SOLINI25621>
        [OperationContract]
        Respuesta ActualizarParametro(Parametro entity);

        [OperationContract]
        Respuesta CotizarCapitalRequerido(ref CapitalRequerido capitalRequerido);

        //<SOLFIN25621>

        //<SOLINI26593>
        [OperationContract]
        List<SolicitudRP> ListarSolicitudRP(string cuspp);

        [OperationContract]
        SolicitudRP ObtenerDatosSolicitudRP(string idSolicitud, DateTime fecCotizacion);

        [OperationContract]
        Respuesta RegistrarSolicitudRP(ref SolicitudRP solicitudRP);

        [OperationContract]
        Respuesta ActualizarSolicitudRP(ref SolicitudRP solicitudRP);
        //<SOLFIN26593>

        //<INIGTI_1092>
        [OperationContract]
        String SolicitudesHabilitadas(int lote, string solicitudes);
        //<FINGTI_1092>


        //<INIGTI_753>
        [OperationContract]
        List<SolicitudRPPlus> ListarSolicitudRPPlus(string cuspp);

        [OperationContract]
        SolicitudRPPlus ObtenerDatosSolicitudRPPlus(string idSolicitud);

        [OperationContract]
        Respuesta RegistrarSolicitudRPPlus(ref SolicitudRPPlus solicitud);

        [OperationContract]
        Respuesta ActualizarSolicitudRPPlus(ref SolicitudRPPlus solicitud);
        //<FINGTI_753>


        //<INIGTI_4081>
        [OperationContract]
        Respuesta ObtenerDTra(ref Solicitud solicitud, string usuario);

        [OperationContract]
        List<FlujoMovimiento> ObtenerFlujos(DateTime fecCotizacion, string evento, string rol);

        [OperationContract]
        List<ParametroEspecial> ObtenerTraDefault(string idSolicitud);

        [OperationContract]
        List<ParametroEspecial> ObtenerTasaMaximaTraMinima(string idSolicitud, DateTime fecCotizacion);

        [OperationContract]
        Respuesta RegistrarCotizaValPar(List<ParametroEspecial> parametros, string idUsuario);

        [OperationContract]
        List<SolicitudEscenario> ListarSolicitudesEmail(string idSolicitudes);

        [OperationContract]
        CuotasTra ObtenerCuotasTra(CuotasTra rolCuotas);

        [OperationContract]
        List<SolicitudEscenario> ListarSolicitudesValidaFlujo(string NumSolicitud, string NumOperacion);

        [OperationContract]
        bool ValidaFlujoSolicitudRol(string num_solicitud, string rol, string evento);

        [OperationContract]
        List<CuotasTra> ListarCuotasTra(int periodo, int mes);

        [OperationContract]
        Respuesta RegistrarCuotas(List<CuotasTra> lstCuotas, string usuario);

        [OperationContract]
        List<GestionVentas> ConsultarGestionVentas(DateTime fechaInicial, DateTime fechaFinal, int numJefe, int numSuperv, int numAgente, string indCierre, string tipoCotizacion, string codCiaSeguro);
        //<FINGTI_4081>

        //<INIGTI_4081_2>
        [OperationContract]
        Respuesta ActualizarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario);
        //<FINGTI_4081_2>

        //<INIGTI_4081_3>
        [OperationContract]
        Respuesta RegistrarFlujoSolicitudCompleto(ref Respuesta rpta, ref Solicitud solicitud, string XML_CotizacionMovimiento, string codUserName, string codRol);
        //<FINGTI_4081_3>

        //<INIGTI_6556>
        [OperationContract]
        List<SolicitudEscenario> ListarSolicitudesPendientesEmail(string cod_rol);
        //<FINGTI_6556>

        [OperationContract]
        Respuesta VistaPreviaCotizacionPlus(string num_solicitud, int num_correlativo, string usuario);

        //<INIGTI_7012>
        [OperationContract]
        Respuesta CerrarCotizacionPlus(string num_solicitud, int num_correlativo, string usuario, List<GrupoFamiliar> lstGrupoFamiliar);

        [OperationContract]
        Respuesta GenerarPolizaPlus(string num_solicitud, int num_correlativo, string usuario, GrupoFamiliar grup_fam);

        [OperationContract]
        Respuesta AnularSolicitudPlus(string num_solicitud, string usuario, string cod_causante);

        [OperationContract]
        List<CausalPoliza> ListarCausalPolizaPlus();

        //<FINGTI_7012>

        ////<INIGTI_7012>
        //[OperationContract]
        //List<Temporal> ObtenerTemporalesPorTabla(string tabla);

        [OperationContract]
        List<DatosSol> ObtenerDatosporSolicitud(string num_Solicitud);
        //<FINGTI_7012>

        //<INIGTI_7012>
        [OperationContract]
        List<Temporal> ListarGruposFamiliaresxSolicitud(string num_Solicitud);

        [OperationContract]
        List<Parametro> ObtenerParametros(string tabla);
        //<FINGTI_7012>


        //<INI.GTI_7012_2>
        [OperationContract]
        List<Agente> ObtenerAgenteDeudaAcom(string idAgente);

        //<INI.GTI_7012_2_1>
        [OperationContract]
        List<SolicitudRPPlus> ListarReporteCotizacionPlus(string cuspp);
        //<FIN.GTI_7012_2_1>

        [OperationContract]
        List<Parametro> ObtenerNroBancos(string tabla, string tipoBanco, string tipoCuenta);
        //<FIN.GTI_7012_2>

        //<INI.GTI_7012_3>
        [OperationContract]
        EmisionPoliza EmitirPoliza(string num_solicitud, int num_poliza, string dig_poliza, string TipoProducto);

        [OperationContract]
        List<byte[]> EmitirPolizaPDF(string num_solicitud, int num_poliza, string dig_poliza, GrupoFamiliar grupoFamiliar, string TipoProducto);
        //<FIN.GTI_7012_3>

        [OperationContract]
        Respuesta GenerarPolizaElectronicaPDF(string num_solicitud, int num_poliza, string dig_poliza, GrupoFamiliar grupoFamiliar, string TipoProducto, string Usuario);

        //[OperationContract]
        //Respuesta EnviarPolizaElectronicaPDF(string num_solicitud, int num_poliza, string dig_poliza, EmisionPoliza emisionPoliza, GrupoFamiliar grupoFamiliar, string TipoProducto);



        //<INI.GTI_7012_11>
        [OperationContract]
        List<Parametro> ObtenerTipoCtaBancos(string banco, string id);
        //<INI.GTI_7012_11>

        //<INI.GTI_7012_25>
        [OperationContract]
        JsonCoincidencia ObtenerCoincidencia(GrupoFamiliar grupoFamiliar);

        [OperationContract]
        JsonCoincidenciaLN ObtenerCoincidenciaLN(GrupoFamiliar grupoFamiliar);
        //<FIN.GTI_7012_25>

        //<INI.GTI_7012_25>
        [OperationContract]
        List<Parametro> ObtenerTipoIdentificacion(string cod_tipo_identificacion, string gls_tipo_identificacion, string gls_corta_identificacion);
        //<FIN.GTI_7012_25>

        //<INI.GTI_7012_25>
        [OperationContract]
        Respuesta ActualizarSolicitudOperaciones(string num_solicitud, int cod_estado_rpp, string gls_observacion, string usuario);
        //<FIN.GTI_7012_25>

        [OperationContract]
        JsonPropuesta ObtenerCalificacion(Propuesta propuesta, string urlCWRV);

        //<INI.GTI_7012_S16>
        [OperationContract]
        Agente ObtenerSupervisorAgente(string usuario);

        [OperationContract]
        Respuesta RegistrarFlujoEvaluacion(FlujoEvaluacion flujoEvaluacion);

        [OperationContract]
        List<SolicitudRPPlus> ListarSolicitudEvaluacion();
        //<FIN.GTI_7012_S16>

        [OperationContract]
        JsonEmail ObtenerEmailPlaft();

        [OperationContract]
        void EnviarEmailListaNegra(Propuesta propuesta);

        [OperationContract]
        List<SolicitudRPPlus> ListarSolicitudCierresPlus(string cuspp);

        [OperationContract]
        SolicitudRPPlus ObtenerEstadoSolicitudRPPlus(string num_solicitud);

        [OperationContract]
        string ArchivosExistentes(string num_solicitud);

        //<INI.GTI_7012_S22>
        //[OperationContract]
        //CotizacionMotorIFP CotizarIFP(string cod_tipo_temporalidad, string cod_moneda, DateTime fec_cotizacion, bool ind_flag, string num_solicitud, int num_correlativo);

        //[OperationContract]
        //List<CotizacionMotorIFP> CotizarIFPs(DateTime fec_cotizacion, bool ind_flag, string num_solicitud);

        //<FIN.GTI_7012_S22>

        //<INI.GTI_7012_S25>
        [OperationContract]
        int CantidadSolicitudes(string cuspp, string moneda, double val_mto_prima_unica);

        [OperationContract]
        List<List<Parametro>> ObtenerComboboxIFP();
        //<FIN.GTI_7012_S25>

        [OperationContract]
        ParametrosMotorIFP ObtenerParametroGenerales(string cod_tipo_temporalidad, string cod_moneda, DateTime fec_cotizacion, bool ind_flag, string tokenUsuario, ref bool cantidad_megas, string usuario);

        [OperationContract]
        Respuesta RegistrarSolicitudIFP(string tokenUsuario, ref SolicitudIFP solicitud);

        [OperationContract]
        Respuesta ActualizarSolicitudIFP(string tokenUsuario, ref SolicitudIFP solicitud);

        [OperationContract]
        SolicitudIFP ObtenerDatosSolicitudIFP(string idSolicitud);

        [OperationContract]
        List<SolicitudIFP> ListarSolicitudIFP(string cuspp);

        //<INI.GTI_7012_S27>
        [OperationContract]
        List<DatosSol> ObtenerDatosporSolicitudIFP(string num_Solicitud);

        [OperationContract]
        List<RolDcom> ListarRangoDcomIFP(RolDcom rolDcom);
        //<FIN.GTI_7012_S27>



        //[OperationContract]
        //Respuesta ActualizarPolizaSME(string gls_poliza, int codigo_SME, string usuario);

        [OperationContract]
        EmisionPoliza EmitirPolizaElectronica(string num_solicitud, int num_poliza, string dig_poliza, string TipoProducto);
        [OperationContract]
        //Respuesta GenerarPolizaElectronica(string gls_poliza);
        Respuesta GenerarPolizaElectronica(EmisionPoliza emisionPoliza);

        [OperationContract]
        Respuesta GenerarFormatoPEP(DatosSol entidadSolicitud, string nacionalidadDescripcion, string residenciaDescripcion, string institucionLaboralDescripcion, string cargoDescripcion, string profesionDescripcion);

        [OperationContract]
        bool EliminarParametrosGenerales();

        [OperationContract]
        Respuesta GenerarFormatoOF(DatosSol entidadDatosSolicitud, string descripcionNacionalidad, string descripcionProfesion, string descripcionDepartamento, string descripcionProvincia, string descripcionDistrito, string descripcionEstadoCivil, string descripcionSexo, string descripcionCentrotrabajo, string descripcionActividadeconomica, string descripcionIngresonetomensual, string descripcionCargo, string descripcionMontoOperacion, string descripcionDireccion, string descripcionTipoMoneda);

        [OperationContract]
        Respuesta GenerarFormatoSolicitud(JsonFormatoSolicitud entidadFormatoSolicitud);

        [OperationContract]
        string archivoLog(string nombreArchivo);

        //<INI.GTI_22543_01>
        [OperationContract]
        Respuesta CrearAgente(Agente agente);

        [OperationContract]
        Respuesta ActualizarAgente(Agente agente);

        [OperationContract]
        Respuesta EliminarAgente(Agente agente);

        [OperationContract]
        Agente ObtenerAgente(int IdAgente, string usuario);

        [OperationContract]
        List<Agente> ObtenerJerarquiaAgente(int nivelAgente, string usuario);

        //<FIN.GTI_22543_01>

        //<INI.GTI_26560>
        [OperationContract]
        Respuesta ActualizarConsentimientoAfiliado(Afiliado afiliado, ConsentimientoAsesoria consentimientoAsesoria);

        [OperationContract]
        List<Beneficiario> ListarBeneficiarios(string numSolicitud, string usuario);

        [OperationContract]
        Respuesta ActualizarBeneficiario(Beneficiario beneficiario);
        //<FIN.GTI_26560>

        [OperationContract]
        Respuesta RegistrarPersonaVinculada(GrupoFamiliar grupo);
        [OperationContract]
        Respuesta ActualizarPersonaVinculada(GrupoFamiliar grupo);
        [OperationContract]
        Respuesta EliminarPersonaVinculada(int idPersonaVinculada, string usuario);
        [OperationContract]
        List<GrupoFamiliar> ObtenerPersonaVinculada(GrupoFamiliar grupo);

        [OperationContract]
        Respuesta ActualizarDireccionSolicitud(string numCuspp, string numSolicitud, string usuario);

        [OperationContract]
        bool isFirmaDigitalAprobada(string numSolicitud, int numItem, string usuario);

        [OperationContract]
        FirmaDigital ObtenerFirmaDigital(string solicitud, int item, string usuario);
        [OperationContract]
        FirmaDigital RegistrarFirmaDigital(FirmaDigital firma);
        [OperationContract]
        FirmaDigital ActualizarFirmaDigital(FirmaDigital firma);

        [OperationContract]
        Respuesta PreseleccionarCotizacion(string num_solicitud, string num_correlativo, string usuario);

        [OperationContract]
        NotificacionSME EnviarNotificacionSME(NotificacionSME notificacionSME);

        [OperationContract]
        Respuesta EnviarNotificacionAizen(Notificacion notificacion);

        [OperationContract]
        Respuesta EnviarPolizaElectronicaRPP_PDF(int num_poliza, string Usuario);

        [OperationContract]
        GrupoFamiliar ObtenerDatosBenefiCierre(int idGrupoFamiliar, string num_solicitud);
        [OperationContract]
        Respuesta CerrarBeneficiariosIFP(string num_solicitud, List<GrupoFamiliar> lstGrupoFamiliar, string tipoPlan, List<GrupoFamiliar> lstBeneficiariosPNoG, string usuario);
        [OperationContract]
        Respuesta EliminarGrupoFamiliar(GrupoFamiliar grupo);

        //<GTI.INI-29372>
        [OperationContract]
        Respuesta ActualizarSolicitudEnvioObligatorio(ref SolicitudEscenario solicitudEscenario);
        [OperationContract]
        List<Solicitud> ListarSolicitudesCargaMeler(string xml);
        [OperationContract]
        List<Contrato> ListarContratosCotizaciones(string codUserName);
        [OperationContract]
        Respuesta ActualizarContratoCotizacion(Contrato contrato);
        [OperationContract]
        Respuesta InsertarContratoCotizacion(Contrato contrato);
        [OperationContract]
        Respuesta EliminarContratoCotizacion(int idContrato, string codUserName);
        //<GTI.FIN-29372>
        [OperationContract]
        List<ReporteIndicadoresRRVV> ListarReporteIndicadoresRRVV();
        [OperationContract]
        Respuesta SeleccionarBeneficiario(string num_solicitud, string num_correlativo, string usuario);
        [OperationContract]
        List<ConsentimientosAgrupadosAge> ListarConsentimientosAgrupadosAge(string periodo, string usuario);
        [OperationContract]
        List<FirmaDigitalDashboard> ObtenerFirmaDigitalesDashboard(string fechaInicio, string fechaFin, string usuario);
        [OperationContract]
        List<PolizasDashboard> ObtenerPolizasDashboard(string fechaInicio, string fechaFin, string usuario);
        [OperationContract]
        List<ReporteTrazabilidad> ObtenerTrazabilidad(string fechaInicio, string fechaFin, string idProceso, string usuario);

        [OperationContract]
        Respuesta TransferirDatosPolizaRRVV(string num_solicitud, int numeroCorrelativo, string usuario);

        [OperationContract]
        Respuesta GenerarPolizaRVI(List<Solicitud> solicitudes, string usuario);

        [OperationContract]
        byte[] ObtenerReporteRecalculoPDF(List<ReporteRecalculoCotizacion> listaRecalculoCotizacion);

        [OperationContract]
        byte[] ObtenerReporteCotizacionesGanadasPDF(int numLote);

        [OperationContract]
        Solicitud ListarCotizacionesPorSolicitud(string numeroSolicitud, string usuario);

        [OperationContract]
        void ActualizarCotizacionGanadoraRVI(string solicitud, int correlativo, string usuario);

        [OperationContract]
        List<EstadoCivil> ListarEstadoCivil(string usuario);

        [OperationContract]
        List<Profesion> ListarProfesion(string usuario);


        [OperationContract]
        List<Nacionalidad> ListarNacionalidad(string usuario);

        [OperationContract]
        int ObtenerIndicadorRescateIFP(string solicitud, string usuario);

        [OperationContract]
        CotizacionRescate RecotizarSolicitudIFP(string numeroSolicitud, DateTime fecCotizacion, int numNesRescate, string usuario);

        [OperationContract]
        List<Agente> ListarAgenteExterno(string gls_agente, string usuario);

        [OperationContract]
        Agente ObtenerUltimoAgentePorCartera(string cartera, string usuario);

        [OperationContract]
        SolicitudIFP ValidarCotizacionVigente(string cod_tipo_documento, string num_documento, string usuario);

        [OperationContract]
        List<FormatoSolicitud> ListarFormatosSolicitud(string solicitud, string usuario);
        [OperationContract]
        FormatoSolicitud ObtenerFormatoSolicitud(string solicitud, int idFormatoSolicitud, string usuario);
        [OperationContract]
        List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiario(string solicitud, int idFormatoSolicitud, string usuario);
        [OperationContract]
        List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculada(string solicitud, int idFormatoSolicitud, string usuario);
        [OperationContract]
        FormatoSolicitud ObtenerFormatoSolicitudActualizado(string solicitud, string usuario);
        [OperationContract]
        List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiarioActualizado(string solicitud, string usuario);
        [OperationContract]
        List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculadaActualizado(string solicitud, string usuario);
        [OperationContract]
        List<DateTime> ListarFeriados();
        [OperationContract]
        List<Dominio.Entidades.MotorCalculo.JuegoParametros> ObtenerParametrosRPP(string temporalidad, DateTime fechaCotizacion, string origen, string usuario);
        [OperationContract]
        SolicitudRPPlus CotizarRPP(SolicitudRPPlus solicitud, List<Dominio.Entidades.MotorCalculo.Parametros> parametros);
        [OperationContract]
        double ObtenerTipoCambio(string codigo, DateTime fecha, string usuario);
        [OperationContract]
        long EnviarCorreoSME(SMEEnvio envio);
        [OperationContract]
        Respuesta ActualizarEstudioNecesidadSolicitud(string num_solicitud, int? id_estudio_necesidades, string usuario);
        [OperationContract]
        List<Direccion> ListarRviDireccion(string solicitud, string usuario);
        [OperationContract]
        List<Departamento> ListarDepartamentos(string usuario);
        [OperationContract]
        List<Provincia> ListarProvincias(string idDepartamento, string usuario);
        [OperationContract]
        List<Distrito> ListarDistritos(string idProvincia, string usuario);
        [OperationContract]
        EnvioSeguimiento RegistrarEnvioSeguimiento(EnvioSeguimiento envioSeguimiento);
        [OperationContract]
        RviBdIni ObtenerAseguradoBdIni(string cuspp, string usuario);
        [OperationContract]
        RviBdIni RegistrarAseguradoBdIni(RviBdIni asegurado);
        [OperationContract]
        RviBdIni ActualizarAseguradoBdIni(RviBdIni asegurado);

        //<GTI.59048-INI>
        [OperationContract]
        AbonoPoliza ObtenerAbonoPorPoliza(int numeroPoliza, string usuario);
        [OperationContract]
        PolizaRV ObtenerDatosPolizaRV(string num_solicitud, int numeroCorrelativo, string usuario);
        //<GTI.59048-FIN>
        [OperationContract]
        List<ValPar> ListarParametrosValPar(string producto, DateTime fecha, string parametro, string usuario);
        [OperationContract]
        bool ProcesarParametrosvalPar(string producto, List<ValPar> parametros, string usuario);
        [OperationContract]
        Respuesta CotizarDifTRA(ref Solicitud solicitud, string usuario);

    }
}
