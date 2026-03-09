using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace Interseguro.CWRV.Aplicacion.ModuloPrincipal
{
    public class GeneralesServicio : IGeneralesServicio
    {
        private readonly IRepositorioParametro repositorioParametro;
        private readonly IRepositorioParametroGeneral repositorioParametroGeneral;
        private readonly IRepositorioDireccion repositorioDireccion;
        private readonly IRepositorioTelefono repositorioTelefono;
        private readonly IRepositorioCiudad repositorioCiudad;
        private readonly IRepositorioComuna repositorioComuna;
        private readonly IRepositorioProducto repositorioProducto;
        private readonly IRepositorioLogBD repositorioLog;
        private readonly IRepositorioConfiguracionMontoCIC repositorioConfiguracionMontoCIC;
        private readonly IRepositorioTemporal repositorioTemporal;
        private readonly IRepositorioCarta repositorioCarta;
        private readonly IRepositorioCargaControlCdA repositorioCargaControlCdA;
        private readonly IRepositorioCargaLocalidadVCTP repositorioCargaLocalidadVCTP;
        private readonly IRepositorioCargaControlVCTP repositorioCargaControlVCTP;

        public GeneralesServicio(
            IRepositorioParametro repositorioParametro,
            IRepositorioParametroGeneral repositorioParametroGeneral,
            IRepositorioDireccion repositorioDireccion,
            IRepositorioTelefono repositorioTelefono,
            IRepositorioCiudad repositorioCiudad,
            IRepositorioComuna repositorioComuna,
            IRepositorioProducto repositorioProducto,
            IRepositorioConfiguracionMontoCIC repositorioConfiguracionMontoCIC,
            IRepositorioLogBD repositorioLog,
            IRepositorioTemporal repositorioTemporal,
            IRepositorioCarta repositorioCarta,
            IRepositorioCargaControlCdA repositorioCargaControlCdA,
            IRepositorioCargaLocalidadVCTP repositorioCargaLocalidadVCTP,
            IRepositorioCargaControlVCTP repositorioCargaControlVCTP
            )
        {
            this.repositorioParametro = repositorioParametro;
            this.repositorioParametroGeneral = repositorioParametroGeneral;
            this.repositorioDireccion = repositorioDireccion;
            this.repositorioTelefono = repositorioTelefono;
            this.repositorioCiudad = repositorioCiudad;
            this.repositorioComuna = repositorioComuna;
            this.repositorioProducto = repositorioProducto;
            this.repositorioLog = repositorioLog;
            this.repositorioConfiguracionMontoCIC = repositorioConfiguracionMontoCIC;
            this.repositorioTemporal = repositorioTemporal;
            this.repositorioCarta = repositorioCarta;
            this.repositorioCargaControlCdA = repositorioCargaControlCdA;
            this.repositorioCargaLocalidadVCTP = repositorioCargaLocalidadVCTP;
            this.repositorioCargaControlVCTP = repositorioCargaControlVCTP;
        }

        public List<List<Parametro>> ObtenerCombobox()
        {
            var parametros = repositorioParametro.ObtenerCombobox();
            return parametros;
        }

        public List<Parametro> ObtenerParametrosSimuladores()
        {
            var parametros = repositorioParametro.ObtenerParametrosSimuladores();
            return parametros;
        }

        public List<Parametro> ObtenerParametrosPorTabla(string codTabla)
        {
            var parametros = repositorioParametro.ObtenerParametrosPorTabla(codTabla);
            return parametros;
        }

        public Ciudad ObtenerDatosCiudad(string idCiudad)
        {
            var ciudad = repositorioCiudad.ObtenerDatos(idCiudad);
            return ciudad;
        }

        public List<Ciudad> ListarCiudad(string idDepartamento)
        {
            var ciudades = repositorioCiudad.Listar(idDepartamento);
            return ciudades;
        }

        public Comuna ObtenerDatosComuna(string idComuna)
        {
            var comuna = repositorioComuna.ObtenerDatos(idComuna);
            return comuna;
        }

        public List<Comuna> ListarComuna(string idCiudad)
        {
            var comunas = repositorioComuna.Listar(idCiudad);
            return comunas;
        }

        public List<Direccion> ListarDireccion(string cussp)
        {
            var direcciones = repositorioDireccion.Listar(cussp);
            return direcciones;
        }

        public Direccion ObtenerDatosDireccion(int idDireccion)
        {
            var direccion = repositorioDireccion.ObtenerDatos(idDireccion);
            return direccion;
        }

        public void RegistrarDireccion(Direccion direccion)
        {
            repositorioDireccion.Registrar(direccion);
        }

        public void ActualizarDireccion(Direccion direccion)
        {
            repositorioDireccion.Actualizar(direccion);
        }

        public void EliminarDireccion(Direccion direccion)
        {
            repositorioDireccion.Eliminar(direccion);
        }

        public List<Telefono> ListarTelefono(string cuspp)
        {
            var direcciones = repositorioTelefono.Listar(cuspp);
            return direcciones;
        }

        public Telefono ObtenerDatosTelefono(int idTelefono)
        {
            var telefono = repositorioTelefono.ObtenerDatos(idTelefono);
            return telefono;
        }

        public void RegistrarTelefono(Telefono telefono)
        {
            repositorioTelefono.Registrar(telefono);
        }

        public void ActualizarTelefono(Telefono telefono)
        {
            repositorioTelefono.Actualizar(telefono);
        }

        public void EliminarTelefono(Telefono telefono)
        {
            repositorioTelefono.Eliminar(telefono);
        }

        public List<Producto> ListarProducto(string idCategoria)
        {
            var productos = repositorioProducto.Listar(idCategoria);
            return productos;
        }

        public void RegistrarLog(LogBD logbd)
        {
            repositorioLog.Registrar(logbd);
        }

        #region MontoCIC

        public List<MontoCIC> ListarMontoCIC()
        {
            var configuracion = repositorioConfiguracionMontoCIC.Listar();
            return configuracion;
        }

        public void RegistrarMontoCIC(MontoCIC entity)
        {
            repositorioConfiguracionMontoCIC.Registrar(entity);
        }

        public void ActualizarMontoCIC(MontoCIC entity)
        {
            repositorioConfiguracionMontoCIC.Actualizar(entity);
        }

        public void EliminarMontoCIC(MontoCIC entity)
        {
            repositorioConfiguracionMontoCIC.Eliminar(entity);
        }

        #endregion

        public void ActualizarParametro(Parametro entity)
        {
            repositorioParametro.Actualizar(entity);
        }

        public List<ParametroEspecial> ObtenerTraDefault(string idSolicitud)
        {
            var parametros = repositorioParametro.ObtenerTraDefault(idSolicitud);
            return parametros;
        }

        public List<ParametroEspecial> ObtenerTasaMaximaTraMinima(string idSolicitud, DateTime fecCotizacion)
        {
            var parametros = repositorioParametro.ObtenerTasaMaximaTraMinima(idSolicitud, fecCotizacion);
            return parametros;
        }

        public void RegistrarCotizaValPar(List<ParametroEspecial> parametros, string idUsuario)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                repositorioParametro.Registrar(parametros, idUsuario);
                transaccion.Complete();
            }
        }

        public List<Parametro> ObtenerParametros(string tabla)
        {
            var parametros = repositorioParametro.ObtenerParametros(tabla);
            return parametros;
        }

        public List<Parametro> ObtenerNroBancos(string tabla, string tipoBanco, string tipoCuenta)
        {
            var parametros = repositorioParametro.ObtenerNroBancos(tabla, tipoBanco, tipoCuenta);
            return parametros;
        }

        public List<Parametro> ObtenerTipoCtaBancos(string banco, string id)
        {
            var parametros = repositorioParametro.ObtenerTipoCtaBancos(banco, id);
            return parametros;
        }

        public List<Parametro> ObtenerTipoIdentificacion(string cod_tipo_identificacion, string gls_tipo_identificacion, string gls_corta_identificacion)
        {
            var parametros = repositorioParametro.ObtenerTipoIdentificacion(cod_tipo_identificacion, gls_tipo_identificacion, gls_corta_identificacion);
            return parametros;
        }

        public List<List<Parametro>> ObtenerComboboxIFP()
        {
            var parametros = repositorioParametro.ObtenerComboboxIFP();
            return parametros;
        }

        public List<Contrato> ListarContratosCotizaciones(string codUserName)
        {
            var listaContratos = repositorioParametro.ListarContratosCotizaciones(codUserName);
            return listaContratos;
        }

        public void ActualizarContratoCotizacion(Contrato contrato)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                repositorioParametro.ActualizarContratoCotizacion(contrato);
                transaccion.Complete();
            }
        }

        public void InsertarContratoCotizacion(Contrato contrato)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                repositorioParametro.InsertarContratoCotizacion(contrato);
                transaccion.Complete();
            }
        }

        public void EliminarContratoCotizacion(int idContrato, string codUserName)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                repositorioParametro.EliminarContratoCotizacion(idContrato, codUserName);
                transaccion.Complete();
            }
        }
        //<GTI.FIN-29372>

        public RviCarta ObtenerDatosCarta(string solicitud, int correlativo, string usuario)
        {
            var carta = repositorioCarta.ObtenerDatos(solicitud, correlativo, usuario);
            return carta;
        }

        public void RegistrarCarta(RviCarta carta)
        {
            using (TransactionScope transaccion = new TransactionScope())
            {
                repositorioCarta.Registrar(carta);
                transaccion.Complete();
            }
        }

        public bool ValidarCargaControlCdA(DateTime fecPeriodo, string codUsuario)
        {
            return repositorioCargaControlCdA.Validar(fecPeriodo, codUsuario);
        }

        public bool ValidarCargaLocalidadVCTP(DateTime fecPeriodo, string codUsuario)
        {
            return repositorioCargaLocalidadVCTP.Validar(fecPeriodo, codUsuario);
        }

        public bool ValidarCargaControlVCTP(DateTime fecPeriodo, string codUsuario)
        {
            return repositorioCargaControlVCTP.Validar(fecPeriodo, codUsuario);
        }

        public void CargarReporteControlCdA(List<CargaControlCdA> listaCargaControlCdA)
        {
            repositorioCargaControlCdA.Eliminar(listaCargaControlCdA.FirstOrDefault());

            foreach (var item in listaCargaControlCdA)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    repositorioCargaControlCdA.Registrar(item);
                    transaccion.Complete();
                }
            }
        }

        public void CargarReporteLocalidadVCTP(List<CargaLocalidadVCTP> listaCargaLocalidadVCTP)
        {
            repositorioCargaLocalidadVCTP.Eliminar(listaCargaLocalidadVCTP.FirstOrDefault());

            foreach (var item in listaCargaLocalidadVCTP)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    repositorioCargaLocalidadVCTP.Registrar(item);
                    transaccion.Complete();
                }
            }
        }

        public void CargarReporteControlVCTP(List<CargaControlVCTP> listaCargaControlVCTP)
        {
            repositorioCargaControlVCTP.Eliminar(listaCargaControlVCTP.FirstOrDefault());

            foreach (var item in listaCargaControlVCTP)
            {
                using (TransactionScope transaccion = new TransactionScope())
                {
                    repositorioCargaControlVCTP.Registrar(item);
                    transaccion.Complete();
                }
            }
        }

        public double ObtenerTipoCambio(string codigo, DateTime fecha, string usuario)
        {
            var tipoCambio = repositorioParametroGeneral.ObtenerTipoCambio(codigo, fecha, usuario);
            return tipoCambio;
        }

        public List<Direccion> ListarRviDireccion(string solicitud, string usuario)
        {
            return repositorioDireccion.Listar(solicitud, null, usuario);
        }
    }
}
