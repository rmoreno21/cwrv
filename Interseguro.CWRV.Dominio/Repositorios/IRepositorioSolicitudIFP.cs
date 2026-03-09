using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{

    public interface IRepositorioSolicitudIFP : IRepositorio<ParametrosMotorIFP>
    {
        List<SolicitudIFP> Listar(string cuspp);

        ParametrosMotorIFP ObtenerParametroGenerales(string cod_tipo_temporalidad, string cod_moneda, DateTime fec_cotizacion, bool ind_flag, string usuario);

        ParametrosMotorIFP ObtenerParametroCotizacion(string num_solicitud, DateTime fec_cotizacion, int num_correlativo);

        List<ParametrosMotorIFP> ObtenerParametroCotizaciones(string num_solicitud, DateTime fec_cotizacion, string usuario);

        int CantidadSolicitudes(string cuspp, string moneda, double val_mto_prima_unica);

        void Registrar(ref SolicitudIFP entity);

        void Actualizar(ref SolicitudIFP entity);

        void RegistrarPjeBeneficiarios(string idSolicitud, string correlativos, string xml_pje, string xml_costo, string usuario);

        void RegistrarCotiza(string xml_cotiza, string usuario);

        SolicitudIFP ObtenerDatos(string idSolicitud);

        List<DatosSol> ObtenerDatosporSolicitudIFP(string num_Solicitud);

        int ObtenerIndicadorRescateIFP(string solicitud, string usuario);

        SolicitudIFP ValidarCotizacionVigente(string cod_tipo_documento, string num_documento, string usuario);

        void RegistrarPjeBen(SolicitudIFP solicitud);

    }

}
