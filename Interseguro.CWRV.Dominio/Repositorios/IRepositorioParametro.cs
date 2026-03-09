using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioParametro:IRepositorio<Parametro>
    {
        List<List<Parametro>> ObtenerCombobox();
        List<Parametro> ObtenerParametrosSimuladores();
        List<Parametro> ObtenerParametrosPorTabla(string codTabla);
        List<ParametroEspecial> ObtenerTraDefault(string idSolicitud);
        List<ParametroEspecial> ObtenerTasaMaximaTraMinima(string idSolicitud, DateTime fecCotizacion);
        void Registrar(List<ParametroEspecial> parametros, string idUsuario);

        List<Parametro> ObtenerParametros(string tabla);
        List<Parametro> ObtenerNroBancos(string tabla, string tipoBanco, string tipoCuenta);
        List<Parametro> ObtenerTipoCtaBancos(string tipoBanco, string id);

        List<Parametro> ObtenerTipoIdentificacion(string cod_tipo_identificacion, string gls_tipo_identificacion, string gls_corta_identificacion);

        List<List<Parametro>> ObtenerComboboxIFP();

        List<Contrato> ListarContratosCotizaciones(string codUserName);
        void ActualizarContratoCotizacion(Contrato contrato);
        void InsertarContratoCotizacion(Contrato contrato);
        void EliminarContratoCotizacion(int idContrato, string codUserName);
    }
}
