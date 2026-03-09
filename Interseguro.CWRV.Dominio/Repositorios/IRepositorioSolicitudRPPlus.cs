using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioSolicitudRPPlus : IRepositorio<SolicitudRPPlus>
    {
        List<SolicitudRPPlus> Listar(string cuspp);
        void Registrar(ref SolicitudRPPlus sol);
        void Actualizar(ref SolicitudRPPlus sol);
        SolicitudRPPlus ObtenerDatos(string idSolicitud);
        List<ParametroCotizador> ObtenerParametrosCotizacionRPPlus(string idSolicitud, DateTime fecCotizacion, Int64? numCorrelativo);
        void RegistrarPjeBeneficiarios(string idSolicitud, string correlativos, string xml_pje, string xml_costo, string usuario);
        void RegistrarPjeBen(SolicitudRPPlus solicitud);
        void RegistrarCotiza(string xml_cotiza, string usuario);
        List<ParametroCotizador> ObtenerParametrosCapitalRequeridoRPPlus(CapitalRequerido capitalRequerido);
        void VistaPreviaCotizacion(string num_solicitud, int num_correlativo, string usuario, ref string cod_tipo_cotizacion);
        void CerrarCotizacion(string num_solicitud, int num_correlativo, string usuario, ref string cod_tipo_cotizacion);
        string ObtenerSecuenciaPolizaPlus();
        void GenerarPolizaPlus(string num_solicitud, int num_correlativo, Int64 num_poliza, string dig_poliza, string usuario);
        void AnularSolicitud(string num_solicitud, string usuario, string cod_causante);
        List<SolicitudRPPlus> ListarReporte(string cuspp);

        void ActualizarEnvioADMWR(string num_poliza, string usuario);
        void ActualizacionSolicitudPlusPlaft(string num_solicitud, string cod_tipo_flujo_evaluacion, int cod_estado, string gls_observacion, string gls_archivos_existentes, string usuario);
        SolicitudRPPlus ObtenerEstado(string num_solicitud);

        void RegistrarFlujoEvaluacion(FlujoEvaluacion flujoEvaluacion);

        List<SolicitudRPPlus> ListarSolicitudEvaluacion();
        List<SolicitudRPPlus> ListarSolicitudCierres(string cuspp);
    }
}
