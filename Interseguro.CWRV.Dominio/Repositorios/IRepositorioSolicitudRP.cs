using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioSolicitudRP : IRepositorio<SolicitudRP>
    {
        List<SolicitudRP> Listar(string cuspp);
        void Registrar(ref SolicitudRP sol);
        void Actualizar(ref SolicitudRP sol);
        SolicitudRP ObtenerDatos(string idSolicitud, DateTime fecCotizacion);
        List<ParametroCotizador> ObtenerParametrosCotizacionRP(string idSolicitud, DateTime fecCotizacion, Int64? numCorrelativo);
        void RegistrarPjeBeneficiarios(string idSolicitud, string correlativos, string xml_pje, string xml_costo, string usuario);
        void RegistrarCotiza(string xml_cotiza, string usuario);
        List<ParametroCotizador> ObtenerParametrosCapitalRequeridoRP(CapitalRequerido capitalRequerido);
        List<FormatoSolicitud> ListarFormatosSolicitud(string solicitud, string usuario);
        FormatoSolicitud ObtenerFormatoSolicitud(string solicitud, int idFormatoSolicitud, string usuario);
        List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiario(string solicitud, int idFormatoSolicitud, string usuario);
        List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculada(string solicitud, int idFormatoSolicitud, string usuario);
        FormatoSolicitud ObtenerFormatoSolicitudActualizado(string solicitud, string usuario);
        List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiarioActualizado(string solicitud, string usuario);
        List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculadaActualizado(string solicitud, string usuario);
    }
}
