using System;
using System.Collections.Generic;
using System.Text;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Aplicacion.ModuloPrincipal
{
    public interface IGeneralesServicio
    {
        List<List<Parametro>> ObtenerCombobox();
        List<Parametro> ObtenerParametrosSimuladores();
        //<SRI.INI-20322_E2>
        List<Parametro> ObtenerParametrosPorTabla(string codTabla);
        //<SRI.FIN-20322_E2>

        Ciudad ObtenerDatosCiudad(string idCiudad);
        List<Ciudad> ListarCiudad(string idDepartamento);

        Comuna ObtenerDatosComuna(string idComuna);
        List<Comuna> ListarComuna(string idCiudad);
        
        List<Direccion> ListarDireccion(string cussp);
        Direccion ObtenerDatosDireccion(int idDireccion);
        void RegistrarDireccion(Direccion direccion);
        void ActualizarDireccion(Direccion direccion);
        void EliminarDireccion(Direccion direccion);

        List<Telefono> ListarTelefono(string cuspp);
        Telefono ObtenerDatosTelefono(int idTelefono);
        void RegistrarTelefono(Telefono telefono);
        void ActualizarTelefono(Telefono telefono);
        void EliminarTelefono(Telefono telefono);

        List<Producto> ListarProducto(string idCategoria);

        void RegistrarLog(LogBD logbd);

        #region MontoCIC
        List<MontoCIC> ListarMontoCIC();
        void RegistrarMontoCIC(MontoCIC entity);
        void ActualizarMontoCIC(MontoCIC entity);
        void EliminarMontoCIC(MontoCIC entity);
        #endregion

        //<SOLINI25621>
        void ActualizarParametro(Parametro entity);
        //<SOLFIN25621>

        //<INIGTI_4081>
        List<ParametroEspecial> ObtenerTraDefault(string idSolicitud);
        List<ParametroEspecial> ObtenerTasaMaximaTraMinima(string idSolicitud, DateTime fecCotizacion);

        void RegistrarCotizaValPar(List<ParametroEspecial> parametros, string idUsuario);
        //<FINGTI_4081>

        //<INIGTI_7012>
        //List<Temporal> ObtenerTemporalesPorTabla(string tabla);
        List<Parametro> ObtenerParametros(string tabla);
        List<Parametro> ObtenerNroBancos(string tabla, string tipoBanco, string tipoCuenta);
        //<FINGTI_7012>

        //<INI.GTI_7012_11>
        List<Parametro> ObtenerTipoCtaBancos(string banco, string id);
        //<INI.GTI_7012_11>

        //<INI.GTI_7012_25>
        List<Parametro> ObtenerTipoIdentificacion(string cod_tipo_identificacion, string gls_tipo_identificacion, string gls_corta_identificacion);
        //<FIN.GTI_7012_25>

        //<INI.GTI_7012_S25>
        List<List<Parametro>> ObtenerComboboxIFP();
        //<FIN.GTI_7012_S25>

        //<GTI.INI-29372>
        List<Contrato> ListarContratosCotizaciones(string codUserName);
        void ActualizarContratoCotizacion(Contrato contrato);
        void InsertarContratoCotizacion(Contrato contrato);
        void EliminarContratoCotizacion(int idContrato, string codUserName);
        //<GTI.FIN-29372>

        RviCarta ObtenerDatosCarta(string solicitud, int correlativo, string usuario);
        void RegistrarCarta(RviCarta carta);
        bool ValidarCargaControlCdA(DateTime fecPeriodo, string codUsuario);
        bool ValidarCargaLocalidadVCTP(DateTime fecPeriodo, string codUsuario);
        bool ValidarCargaControlVCTP(DateTime fecPeriodo, string codUsuario);
        void CargarReporteControlCdA(List<CargaControlCdA> listaCargaControlVCTP);
        void CargarReporteLocalidadVCTP(List<CargaLocalidadVCTP> listaCargaLocalidadVCTP);
        void CargarReporteControlVCTP(List<CargaControlVCTP> listaCargaControlVCTP);
        double ObtenerTipoCambio(string codigo, DateTime fecha, string usuario);
        List<Direccion> ListarRviDireccion(string solicitud, string usuario);
    }
}
