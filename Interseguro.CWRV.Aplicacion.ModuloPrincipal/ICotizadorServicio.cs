using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Entidades;
//<SRIINI06326>
using System.Data;
//<SRIFIN06326>

namespace Interseguro.CWRV.Aplicacion.ModuloPrincipal
{
    public interface ICotizadorServicio
    {
        SDAReporte obtenerPreCubo(int numeroAgente, string cuspp);
        Agente ObtenerMSAgente(string idAgente);

        List<Afiliado> ListarAfiliado(string apellidoPaterno, string apellidoMaterno, string nombres, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros);
        Afiliado ObtenerDatosAfiliado(string solicitud, string cuspp, string tipoIdentificacion, string numIdentificacion, string producto);

        AporteAdicional ObtenerDatosAporteAdicional(string CUSPP);

        void ActualizarAporteAdicional(AporteAdicional aporte, string usuario);

        void EliminarAporteAdicional(AporteAdicional aporte, string usuario);

        void RegistrarAfiliado(Afiliado afiliado, string usuario, ref string numCUSPP);

        void ActualizarAfiliado(Afiliado afiliado);

        List<GrupoFamiliar> ListarGrupoFamiliar(string cuspp);
        GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar, string num_solicitud);//<INI.GTI_7012_V13>
        void RegistrarGrupoFamiliar(GrupoFamiliar grupo);
        void ActualizarGrupoFamiliar(GrupoFamiliar grupo);

        List<SolicitudEscenario> ListarSolicitudEscenario(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol);
        List<SolicitudEscenario> ListarSolicitudEscenarioCambios(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol);
        SolicitudEscenario ObtenerDatosSolicitudEscenario(string numSolicitud, string codUserName, string codRol);
        void RegistrarSolicitudEscenario(ref SolicitudEscenario solicitudEscenario);
        void ActualizarSolicitudEscenario(ref SolicitudEscenario solicitudEscenario, bool cotizar);
        void RegistrarSolicitudEscenarioExtraoficial(ref SolicitudEscenario solicitudEscenario);

        List<Cita> ListarCita(Cita citaIn);

        List<RolAzmanTipoMovimiento> ObtenerTipoMovimientoPorRolAzman(string codRol);
        List<CotizacionMovimiento> ObtenerCotizacionTipoMovimientoPorSolicitud(string numSolicitud);

        void RegistrarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario);

        List<Solicitud> ListarSolicitud(string cuspp);
        List<Solicitud> ListarSolicitud(int lote);
        List<Solicitud> ListarSolicitudesPorFechaCierreAFP(DateTime fechaInicio, DateTime fechaFin, char enviada);
        List<Solicitud> ListarConfirmaciones(DateTime fechaInicio, DateTime fechaFin, char enviada);

        Solicitud ObtenerDatosSolicitud(string idSolicitud, DateTime fecCotizacion);

        SolicitudRP ObtenerDatosSolicitudRP(string idSolicitud, DateTime fecCotizacion);

        List<Actividad> ListarActividad(string cuspp);

        void RegistrarSolicitud(ref Solicitud solicitud);
        void ActualizarSolicitud(ref Solicitud solicitud);
        string ActualizarSolicitudesCargaMeler(string xml, string usuario);
        string ActualizarSolicitudesConfirmacionMeler(string xml, string usuario);
        Solicitud ObtenerCotizacionRecalculo(string numeroSolicitud, string usuario);

        void ProcesarRecalculoCotizacion(string numSolicitud, DateTime fechaCotizacion, double montoCIC, double tipoCambio, string tipoCalculo, string usuario);
        DataSet ListarReporteIndicadoresVCTP(DateTime fecPeriodo, string codUsername, string codRol);
        Respuesta ListarReporteIndicadoresCDA(DateTime fecPeriodo, string codUsername, string codRol);

        void CotizarOficial(string idSolicitud, DateTime fechaCotizacion, string usuario);
        double ObtenerMontoACOM(string solicitud, double acom, long cotizacion);
        void ActualizarValidacion(string idSolicitud, string tipoValidacion, string valor, string usuario);
        List<ParametroCotizadorWS> ObtenerParametrosCotizacion(string idSolicitud, DateTime fechaCotizacion, Int64? numCorrelativo, int anhosAdicionales, double? nuevoCIC);
        void Cotizar(string idSolicitud, DateTime fechaCotizacion, string usuario);
        RespuestaCotizacion ProcesarCotizacion(string idSolicitud, DateTime fechaCotizacion, string usuario);
        Cotizacion Cotizar(string idSolicitud, DateTime fechaCotizacion, Int64 numCorrelativo, int anhosAdicionales, double nuevoCIC);
        Cotizacion Cotizar(ParametroCotizadorWS p);
        void CotizarRP(string idSolicitud, DateTime fechaCotizacion, string usuario);

        List<Seguimiento> ListarSeguimiento(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros);
        List<Seguimiento> ListarExcelSeguimiento(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar);

        List<Supervision> ListarSupervision(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros);
        List<Supervision> ListarExcelSupervision(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar);

        void GenerarReporteEscenarios(string idSolicitud, DateTime fechaCotizacion, string usuario, string maxAcom);
        DataSet ObtenerReporteEscenarios(string idSolicitud, string usuario);

        #region ROLACOM
        List<RolAcom> ListarRolAcom(RolAcom rolAcom);
        List<RolAcom> ListaAcomEscenario(RolAcom rolAcom);
        #endregion

        #region ROLDCOM
        List<RolDcom> ListarRolDcom(RolDcom rolDcom);
        //<GTIINI-10761>
        List<RolDcom> ListarRolDcomRPP(RolDcom rolDcom);
        //<GTIFIN-10761>
        List<RolDcom> ListaDcomEscenario(RolDcom rolDcom);
        #endregion

        #region ROLDTRA
        List<RolDtra> ListarRolDtra(RolDtra rolDtra);
        #endregion

        #region Meler
        int RegistrarDescargaSolicitudes(string xml, string usuario);
        List<Solicitud> RegistrarDescargaResultados(string xml, string usuario, ref int lote);
        List<Lote> ListarLote(int numero);
        List<Lote> ListarLote(DateTime fechaCierreInicial, DateTime fechaCierreFinal);
        List<Lote> ListarLoteResultado(int numero);
        List<Lote> ListarLoteResultado(DateTime fechaCierreInicial, DateTime fechaCierreFinal);
        #endregion

        #region Anticipo
        Anticipo ObtenerDatosAnticipo(string solicitud);
        Anticipo ObtenerDatosAnticipoAceptacion(string solicitud, string agente);
        Anticipo ObtenerDatosAnticipoCondiciones(string solicitud);
        List<Anticipo> ListarAnticipoAceptacion(string solicitud, string agente, DateTime? fechaInicio, DateTime? fechaFin, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar);
        void RegistrarAnticipoAceptacion(Anticipo anticipo);
        #endregion

        List<SolicitudRP> ListarSolicitudRP(string cuspp);
        void RegistrarSolicitudRP(ref SolicitudRP solicitud);
        void ActualizarSolicitudRP(ref SolicitudRP solicitud);

        void CotizarCapitalRequerido(ref CapitalRequerido capitalRequerido);

        String SolicitudesHabilitadas(int lote, string solicitudes);



        List<SolicitudRPPlus> ListarSolicitudRPPlus(string cuspp);
        void RegistrarSolicitudRPPlus(ref SolicitudRPPlus solicitud);
        void ActualizarSolicitudRPPlus(ref SolicitudRPPlus solicitud);
        SolicitudRPPlus ObtenerDatosSolicitudRPPlus(string idSolicitud);

        void ObtenerDTra(ref Solicitud solicitud, string usuario);
        List<FlujoMovimiento> ObtenerFlujos(DateTime fecCotizacion, string evento, string rol);
        void CotizarDifTRA(ref Solicitud solicitud, string usuario);
        List<SolicitudEscenario> ListarSolicitudesEmail(string idSolicitudes);

        CuotasTra ObtenerCuotasTra(CuotasTra rolCuotas);
        List<SolicitudEscenario> ListarSolicitudesValidaFlujo(string NumSolicitud, string NumOperacion);
        bool ValidaFlujoSolicitudRol(string num_solicitud, string rol, string evento);

        List<CuotasTra> ListarCuotasTra(int periodo, int mes);

        void RegistrarCuotas(List<CuotasTra> lstCuotas, string usuario);

        List<GestionVentas> ConsultarGestionVentas(DateTime fechaInicial, DateTime fechaFinal, int numJefe, int numSuperv, int numAgente, string indCierre, string tipoCotizacion, string codCiaSeguro);

        void ActualizarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario);

        void RegistrarFlujoSolicitudCompleto(ref Respuesta rpta, ref Solicitud solicitud, string XML_CotizacionMovimiento, string codUserName, string codRol);

        List<SolicitudEscenario> ListarSolicitudesPendientesEmail(string cod_rol);

        void VistaPreviaCotizacionPlus(string num_solicitud, int num_correlativo, string usuario);

        void CerrarCotizacionPlus(string num_solicitud, int num_correlativo, string usuario, List<GrupoFamiliar> lstGrupoFamiliar);

        string GenerarPolizaPlus(string num_solicitud, int num_correlativo, string usuario, GrupoFamiliar grup_fam, ref string mensaje);

        void AnularSolicitudPlus(string num_solicitud, string usuario, string cod_causante);

        List<CausalPoliza> ListarCausalPolizaPlus();

        List<DatosSol> ObtenerDatosporSolicitud(string num_Solicitud);

        List<Temporal> ListarGruposFamiliaresxSolicitud(string num_solicitud);

        List<Agente> ObtenerAgenteDeudaAcom(string idAgente);

        EmisionPoliza EmitirPoliza(string num_solicitud, int num_poliza, string dig_poliza);

        List<SolicitudRPPlus> ListarReporteCotizacionPlus(string cuspp);

        Respuesta ActualizacionSolicitudPlusPlaft(string num_solicitud, string cod_tipo_flujo_evaluacion, int cod_estado, string gls_observacion, string gls_archivos_existentes, string usuario);

        Respuesta EnviarNotificacion(string rutaServicio, Notificacion notificacion);

        ConfiguracionCorreo ObtenerConfiguracionCorreo(int cod_proceso, DateTime fec_solicitud);

        SolicitudRPPlus ObtenerEstadoSolicitudRPPlus(string num_solicitud);

        void RegistrarFlujoEvaluacion(FlujoEvaluacion flujoEvaluacion);

        List<SolicitudRPPlus> ListarSolicitudEvaluacion();

        List<SolicitudRPPlus> ListarSolicitudCierres(string cuspp);

        string ArchivosExistentes(string num_solicitud);

        int CantidadSolicitudes(string cuspp, string moneda, double val_mto_prima_unica);

        ParametrosMotorIFP ObtenerParametroGenerales(string cod_tipo_temporalidad, string cod_moneda, DateTime fec_cotizacion, bool ind_flag, string tokenUsuario, string usuario);

        void RegistrarSolicitudIFP(string tokenUsuario, ref SolicitudIFP solicitud);

        void ActualizarSolicitudIFP(string tokenUsuario, ref SolicitudIFP solicitud);

        SolicitudIFP ObtenerDatosSolicitudIFP(string idSolicitud);

        void CotizarIFP(string rutaArchivoParametrosIFP, ref SolicitudIFP solicitud);

        List<SolicitudIFP> ListarSolicitudIFP(string cuspp);

        void RegistrarBeneficiarios(List<GrupoFamiliar> lstEntity, int idGrupoFamiliar, string tipoPlan);

        List<DatosSol> ObtenerDatosporSolicitudIFP(string num_Solicitud);

        List<RolDcom> ListarRangoDcomIFP(RolDcom rolDcom);

        Int64 RegistrarPolizaIFPADMWR(string num_solicitud, string usuario, GrupoFamiliar grup_fam);

        Int64 RegistrarPolizaADMWR(string num_solicitud, string usuario, GrupoFamiliar grup_fam);

        EmisionPoliza EmitirPolizaIFP(string num_solicitud, int num_poliza, string dig_poliza);

        Respuesta ActualizarPolizaSME(string gls_poliza, int codigo_SME, string usuario);

        void ActualizarConsentimientoAfiliado(Afiliado afiliado, ConsentimientoAsesoria consentimientoAsesoria);
        List<Beneficiario> ListarBeneficiarios(string numSolicitud, string usuario);

        Respuesta ActualizarBeneficiario(Beneficiario beneficiario);

        void RegistrarPersonaVinculada(GrupoFamiliar grupo);
        void ActualizarPersonaVinculada(GrupoFamiliar grupo);
        void EliminarPersonaVinculada(int idPersonaVinculada, string usuario);
        List<GrupoFamiliar> ObtenerPersonaVinculada(GrupoFamiliar grupo);

        void ActualizarDireccionSolicitud(string numCuspp, string numSolicitud, string usuario);
        void PreseleccionarCotizacion(string num_solicitud, string num_correlativo, string usuario);
        Respuesta EnviarPolizaElectronicaRPP_PDF(int num_poliza, string usuario);

        GrupoFamiliar ObtenerDatosBenefiCierre(int idGrupoFamiliar, string num_solicitud);
        void CerrarBeneficiariosIFP(string num_solicitud, List<GrupoFamiliar> lstGrupoFamiliar, string tipoPlan, List<GrupoFamiliar> lstBeneficiariosPNoG, string usuario);
        void ElminarGrupoFamiliar(GrupoFamiliar grupo);

        void ActualizarSolicitudEnvioObligatorio(ref SolicitudEscenario solicitudEscenario);
        List<Solicitud> ListarSolicitudesCargaMeler(string xml);
        List<ReporteIndicadoresRRVV> ListarReporteIndicadoresRRVV();
        void SeleccionarBeneficiario(string num_solicitud, string num_correlativo, string usuario);

        List<ConsentimientosAgrupadosAge> ListarConsentimientosAgrupadosAge(string periodo, string usuario);

        List<FirmaDigitalDashboard> ObtenerFirmaDigitalesDashboard(string fechaInicio, string fechaFin, string usuario);
        List<PolizasDashboard> ObtenerPolizasDashboard(string fechaInicio, string fechaFin, string usuario);

        List<ReporteTrazabilidad> ObtenerTrazabilidad(string fechaInicio, string fechaFin, string idProceso, string usuario);

        Respuesta TransferirDatosPolizaRRVV(string num_solicitud, int numeroCorrelativo, string usuario);

        void GenerarPolizaRVI(List<Solicitud> solicitudes, string usuario);

        byte[] ObtenerReporteRecalculoPDF(List<ReporteRecalculoCotizacion> listaRecalculoCotizacion, string PdfRecalculoTemplate, string RutaLogo);

        byte[] ObtenerReporteCotizacionesGanadasPDF(int numLote, string PdfRecalculoTemplate, string RutaLogo);

        Solicitud ListarCotizacionesPorSolicitud(string numeroSolicitud, string usuario);
        void ActualizarCotizacionGanadoraRVI(string solicitud, int correlativo, string usuario);

        List<EstadoCivil> ListarEstadoCivil(string usuario);

        List<Profesion> ListarProfesion(string usuario);

        List<Nacionalidad> ListarNacionalidad(string usuario);

        int ObtenerIndicadorRescateIFP(string solicitud, string usuario);

        CotizacionRescate RecotizarSolicitudIFP(string numeroSolicitud, DateTime fecCotizacion, int numNesRescate, string usuario);

        SolicitudIFP ValidarCotizacionVigente(string cod_tipo_documento, string num_documento, string usuario);

        List<FormatoSolicitud> ListarFormatosSolicitud(string solicitud, string usuario);
        FormatoSolicitud ObtenerFormatoSolicitud(string solicitud, int idFormatoSolicitud, string usuario);
        List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiario(string solicitud, int idFormatoSolicitud, string usuario);
        List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculada(string solicitud, int idFormatoSolicitud, string usuario);
        FormatoSolicitud ObtenerFormatoSolicitudActualizado(string solicitud, string usuario);
        List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiarioActualizado(string solicitud, string usuario);
        List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculadaActualizado(string solicitud, string usuario);

        List<Dominio.Entidades.MotorCalculo.JuegoParametros> ObtenerParametrosRPP(string temporatidad, DateTime fechaCotizacion, string origen, string usuario);
        SolicitudRPPlus CotizarRPP(SolicitudRPPlus solicitud, List<Dominio.Entidades.MotorCalculo.Parametros> parametros);

        void ActualizarBeneficiariosPNoG(List<GrupoFamiliar> lstEntity);

        void ActualizarEstudioNecesidadSolicitud(string num_solicitud, int? id_estudio_necesidades, string usuario);

        //<GTI.59048-INI>
        AbonoPoliza ObtenerAbonoPorPoliza(int numeroPoliza, string usuario);
        PolizaRV ObtenerDatosPolizaRV(string num_solicitud, int numeroCorrelativo, string usuario);
        //<GTI.59048-FIN>
    }
}
