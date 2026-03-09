using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioSolicitud : IRepositorio<Solicitud>
    {
        List<Solicitud> Listar(string cuspp);
        List<Solicitud> Listar(int lote);
        List<Solicitud> ListarSolicitudesPorFechaCierreAFP(DateTime fechaInicio, DateTime fechaFin, char enviada);
        List<Solicitud> ListarConfirmaciones(DateTime fechaInicio, DateTime fechaFin, char enviada);
        List<Solicitud> ListarSolicitudesCargaMeler(string xml);
        List<Solicitud> ListarSolicitudesConfirmacionMeler(string xml);
        void Registrar(ref Solicitud sol);
        void Actualizar(ref Solicitud sol);
        void ActualizarSolicitudesCargaMeler(string xml, string usuario);
        void ActualizarSolicitudesConfirmacionMeler(string xml, string usuario);
        Solicitud ObtenerCotizacionRecalculo(string numeroSolicitud, string usuario);
        Solicitud ObtenerDatos(string idSolicitud, DateTime fecCotizacion);
        List<ParametroCotizador> ObtenerParametrosCotizacion(string idSolicitud, DateTime fecCotizacion, Int64? numCorrelativo, int anhosAdicionales, double? nuevoCIC);
        void RegistrarPjeBeneficiarios(string idSolicitud, string correlativos, string xml_pje, string xml_costo, string usuario);
        void RegistrarCotiza(string xml_cotiza, string usuario);
        int RegistrarDescargaSolicitudes(string xml, string usuario);
        List<Solicitud> RegistrarDescargaResultados(string xml, string usuario, ref int lote);
        void RegistrarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario);
        double ObtenerMontoACOM(string solicitud, double acom, long cotizacion);
        void ActualizarValidacion(string idSolicitud, string tipoValidacion, string valor, string usuario);
        List<ParametroCotizador> ObtenerParametrosCapitalRequerido(CapitalRequerido capitalRequerido);

        String SolicitudesHabilitadas(int lote, string solicitudes);

        void ActualizarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario);

        List<DatosSol> ObtenerDatosporSolicitud(string num_Solicitud);

        List<Temporal> ListarGruposFamiliaresxSolicitud(string num_Solicitud);
        void PreseleccionarCotizacion(string num_solicitud, string num_correlativo, string usuario);
        void SeleccionarBeneficiario(string num_solicitud, string num_correlativo, string usuario);

        void ProcesarRecalculoCotizacion(string numSolicitud, DateTime fechaCotizacion, double montoCIC, double tipoCambio, string tipoCalculo, string usuario);
        PolizaRV ObtenerDatosPoliza(string num_solicitud, int numeroCorrelativo, string usuario);
        List<BeneficiarioRV> ObtenerDatosBeneficiarios(string num_solicitud, int numeroCorrelativo, string usuario);
        void GenerarPolizaRVI(string numSolicitud, string usuario);
        Solicitud ListarCotizacionesPorSolicitud(string numeroSolicitud, string usuario);
        void ActualizarCotizacionGanadoraRVI(string solicitud, int correlativo, string usuario);

        void ActualizarEstudioNecesidad(string num_solicitud, int? idEstudioNecesidades, string usuario);
        //<GTI.59048-INI>
        AbonoPoliza ObtenerAbonoPorPoliza(int numeroPoliza, string usuario);
        //<GTI.59048-FIN>
    }
}
