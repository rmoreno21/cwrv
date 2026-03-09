using Interseguro.CWRV.Aplicacion.ModuloPrincipal;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Infraestructura.Transversal;
using Interseguro.CWRV.ServiciosDistribuidos.Proxies.ModuloSeguridad;
using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Interseguro.CWRV.ServiciosDistribuidos
{
    public class ServicioCWRV : IServicioCWRV, IServicioCWRV_Publico
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServicioCWRV));
        //private static IServicioCWRV servicioCotizador;
        //private static readonly DateTime fechaFinVigencia = new DateTime(2099, 12, 31);

        //<SRI.INI-20322_E2>
        public SDAReporte obtenerPreCubo(int numeroAgente, string cuspp)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                SDAReporte reporte = cotizadorServicio.obtenerPreCubo(numeroAgente, cuspp);
                return reporte;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<SRI.FIN-20322_E2>

        //<GTIINI-6623>
        public Agente ObtenerMSAgente(string idAgente)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Agente agente = cotizadorServicio.ObtenerMSAgente(idAgente);
                return agente;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<GTIFIN-6623>

        public List<Afiliado> ListarAfiliado(string apellidoPaterno, string apellidoMaterno, string nombres, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Afiliado> afiliados = cotizadorServicio.ListarAfiliado(apellidoPaterno, apellidoMaterno, nombres, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar, ref totalRegistros);
                return afiliados;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Afiliado ObtenerDatosAfiliado(string solicitud, string cuspp, string tipoIdentificacion, string numIdentificacion, string producto)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Afiliado afiliado = cotizadorServicio.ObtenerDatosAfiliado(solicitud, cuspp, tipoIdentificacion, numIdentificacion, producto);
                return afiliado;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarAfiliado(Afiliado afiliado, string usuario, ref string numCUSPP)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarAfiliado(afiliado, usuario, ref numCUSPP);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarAfiliado(Afiliado afiliado)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarAfiliado(afiliado);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        //<GTI.INI-29372>
        public AporteAdicional ObtenerDatosAporteAdicional(string CUSPP)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                AporteAdicional aporte = cotizadorServicio.ObtenerDatosAporteAdicional(CUSPP);
                return aporte;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta ActualizarAporteAdicional(AporteAdicional aporte, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarAporteAdicional(aporte, usuario);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta EliminarAporteAdicional(AporteAdicional aporte, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.EliminarAporteAdicional(aporte, usuario);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }
        //<GTI.FIN-29372>


        public Ciudad ObtenerDatosCiudad(string idCiudad)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                Ciudad ciudad = generalesServicio.ObtenerDatosCiudad(idCiudad);
                return ciudad;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Ciudad> ListarCiudad(string idDepartamento)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Ciudad> ciudades = generalesServicio.ListarCiudad(idDepartamento);
                return ciudades;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Comuna ObtenerDatosComuna(string idComuna)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                Comuna comuna = generalesServicio.ObtenerDatosComuna(idComuna);
                return comuna;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Comuna> ListarComuna(string idCiudad)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Comuna> comunas = generalesServicio.ListarComuna(idCiudad);
                return comunas;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<List<Parametro>> ObtenerCombobox()
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<List<Parametro>> parametros = generalesServicio.ObtenerCombobox();
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Parametro> ObtenerParametrosSimuladores()
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Parametro> parametros = generalesServicio.ObtenerParametrosSimuladores();
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<SRI.INI-20322_E2>
        public List<Parametro> ObtenerParametrosPorTabla(string codTabla)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Parametro> parametros = generalesServicio.ObtenerParametrosPorTabla(codTabla);
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<SRI.FIN-20322_E2>

        //<SRI.INI-20322_E2>
        public Respuesta RegistrarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarCotizacionMovimiento(XMLCotizacionMovimiento, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Movimientos TRA de Cotizaciones registrados correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }
        //<SRI.FIN-20322_E2>

        public List<Direccion> ListarDireccion(string cuspp)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Direccion> direcciones = generalesServicio.ListarDireccion(cuspp);
                return direcciones;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Direccion ObtenerDatosDireccion(int idDireccion)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                Direccion direccion = generalesServicio.ObtenerDatosDireccion(idDireccion);
                return direccion;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void RegistrarDireccion(Direccion direccion)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.RegistrarDireccion(direccion);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void ActualizarDireccion(Direccion direccion)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.ActualizarDireccion(direccion);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void EliminarDireccion(Direccion direccion)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.EliminarDireccion(direccion);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Telefono> ListarTelefono(string cuspp)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Telefono> telefonos = generalesServicio.ListarTelefono(cuspp);
                return telefonos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Telefono ObtenerDatosTelefono(int idTelefono)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                Telefono telefono = generalesServicio.ObtenerDatosTelefono(idTelefono);
                return telefono;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarTelefono(Telefono telefono)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.RegistrarTelefono(telefono);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue(); ;
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarTelefono(Telefono telefono)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.ActualizarTelefono(telefono);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public void EliminarTelefono(Telefono telefono)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.EliminarTelefono(telefono);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<GrupoFamiliar> ListarGrupoFamiliar(string cuspp)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<GrupoFamiliar> grupo = cotizadorServicio.ListarGrupoFamiliar(cuspp);
                return grupo;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar, string num_solicitud) //<INI.GTI_7012_V13>
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                GrupoFamiliar grupo = cotizadorServicio.ObtenerDatosGrupoFamiliar(idGrupoFamiliar, num_solicitud);//<INI.GTI_7012_V13>
                return grupo;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarGrupoFamiliar(GrupoFamiliar grupo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarGrupoFamiliar(grupo);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarGrupoFamiliar(GrupoFamiliar grupo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarGrupoFamiliar(grupo);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        //<SRI.INI_20322-E2>
        public List<SolicitudEscenario> ListarSolicitudEscenario(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudEscenario> solicitud = cotizadorServicio.ListarSolicitudEscenario(numJefe, numSupervisor, numAgente, codUserName, codRol);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<INIGTI_4081>
        public List<SolicitudEscenario> ListarSolicitudEscenarioCambios(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudEscenario> solicitud = cotizadorServicio.ListarSolicitudEscenarioCambios(numJefe, numSupervisor, numAgente, codUserName, codRol);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<FINGTI_4081>

        public SolicitudEscenario ObtenerDatosSolicitudEscenario(string numSolicitud, string codUserName, string codRol)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                SolicitudEscenario solicitud = cotizadorServicio.ObtenerDatosSolicitudEscenario(numSolicitud, codUserName, codRol);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarSolicitudEscenario(ref SolicitudEscenario solicitudEscenario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarSolicitudEscenario(ref solicitudEscenario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud Escenario cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarSolicitudEscenario(ref SolicitudEscenario solicitudEscenario, bool cotizar)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarSolicitudEscenario(ref solicitudEscenario, cotizar);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud Escenario cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta RegistrarSolicitudEscenarioExtraoficial(ref SolicitudEscenario solicitudEscenario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarSolicitudEscenarioExtraoficial(ref solicitudEscenario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud Extraoficial N° " + solicitudEscenario.NumSolicitud + " generada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public List<Cita> ListarCita(Cita citaIn)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Cita> cita = cotizadorServicio.ListarCita(citaIn);
                return cita;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<RolAzmanTipoMovimiento> ObtenerTipoMovimientoPorRolAzman(string codRol)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<RolAzmanTipoMovimiento> rolAzmanTipoMovimiento = cotizadorServicio.ObtenerTipoMovimientoPorRolAzman(codRol);
                return rolAzmanTipoMovimiento;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<CotizacionMovimiento> ObtenerCotizacionTipoMovimientoPorSolicitud(string numSolicitud)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<CotizacionMovimiento> CotizacionMovimiento = cotizadorServicio.ObtenerCotizacionTipoMovimientoPorSolicitud(numSolicitud);
                return CotizacionMovimiento;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<SRI.FIN_20322-E2>

        public List<Solicitud> ListarSolicitudesPorFechaCierreAFP(DateTime fechaInicio, DateTime fechaFin, char enviada)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Solicitud> solicitud = cotizadorServicio.ListarSolicitudesPorFechaCierreAFP(fechaInicio, fechaFin, enviada);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Solicitud> ListarConfirmaciones(DateTime fechaInicio, DateTime fechaFin, char enviada)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Solicitud> solicitud = cotizadorServicio.ListarConfirmaciones(fechaInicio, fechaFin, enviada);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Solicitud> ListarSolicitud(string cuspp)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Solicitud> solicitud = cotizadorServicio.ListarSolicitud(cuspp);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarSolicitud(ref Solicitud solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarSolicitud(ref solicitud);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarSolicitud(ref Solicitud solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarSolicitud(ref solicitud);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarSolicitudesCargaMeler(string xml, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                respuesta.Contenido = cotizadorServicio.ActualizarSolicitudesCargaMeler(xml, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitudes actualizadas correctamente.<br />Archivo XML generado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarSolicitudesConfirmacionMeler(string xml, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                respuesta.Contenido = cotizadorServicio.ActualizarSolicitudesConfirmacionMeler(xml, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitudes actualizadas correctamente.<br />Archivo XML generado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Solicitud ObtenerCotizacionRecalculo(string numeroSolicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Solicitud solicitud = cotizadorServicio.ObtenerCotizacionRecalculo(numeroSolicitud, usuario);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        //<SRIINI20322>
        public Respuesta CotizarOficial(string idSolicitud, DateTime fechaCotizacion, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.CotizarOficial(idSolicitud, fechaCotizacion, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta CotizarRecalculo(string numSolicitud, DateTime fechaCotizacion, double montoCIC, double tipoCambio, string tipoCalculo, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();

                cotizadorServicio.ProcesarRecalculoCotizacion(numSolicitud, fechaCotizacion.Date, montoCIC, tipoCambio, tipoCalculo, usuario);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";

                return respuesta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public RviCarta ObtenerDatosCarta(string solicitud, int correlativo, string usuario)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                RviCarta carta = generalesServicio.ObtenerDatosCarta(solicitud, correlativo, usuario);
                return carta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public Respuesta RegistrarCarta(RviCarta carta)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();

                generalesServicio.RegistrarCarta(carta);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Carta ingresada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public bool ValidarCargaControlCdA(DateTime fecPeriodo, string codUsuario)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();

                return generalesServicio.ValidarCargaControlCdA(fecPeriodo, codUsuario);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return false;
            }
        }

        public bool ValidarCargaLocalidadVCTP(DateTime fecPeriodo, string codUsuario)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();

                return generalesServicio.ValidarCargaLocalidadVCTP(fecPeriodo, codUsuario);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return false;
            }
        }

        public bool ValidarCargaControlVCTP(DateTime fecPeriodo, string codUsuario)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();

                return generalesServicio.ValidarCargaControlVCTP(fecPeriodo, codUsuario);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return false;
            }
        }

        public Respuesta CargarReporteControlCdA(List<CargaControlCdA> listaCargaControlCdA)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();

                generalesServicio.CargarReporteControlCdA(listaCargaControlCdA);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "La información fue cargada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta CargarReporteLocalidadVCTP(List<CargaLocalidadVCTP> listaCargaLocalidadVCTP)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();

                generalesServicio.CargarReporteLocalidadVCTP(listaCargaLocalidadVCTP);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "La información fue cargada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta CargarReporteControlVCTP(List<CargaControlVCTP> listaCargaControlVCTP)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();

                generalesServicio.CargarReporteControlVCTP(listaCargaControlVCTP);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "La información fue cargada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ListarReporteIndicadoresVCTP(DateTime fecPeriodo, string codUsername, string codRol)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();

                respuesta.Data = cotizadorServicio.ListarReporteIndicadoresVCTP(fecPeriodo, codUsername, codRol);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "La información fue cargada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ListarReporteIndicadoresCDA(DateTime fecPeriodo, string codUsername, string codRol)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();

                respuesta = cotizadorServicio.ListarReporteIndicadoresCDA(fecPeriodo, codUsername, codRol);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }

            return respuesta;
        }

        public Respuesta ObtenerMontoACOM(string solicitud, double acom, long cotizacion)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                //respuesta.Contenido = String.Format("{0:0,0.00}", cotizadorServicio.ObtenerMontoACOM(solicitud, acom, cotizacion));
                respuesta.Contenido = String.Format("{0:0.00}", cotizadorServicio.ObtenerMontoACOM(solicitud, acom, cotizacion));
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarValidacion(string idSolicitud, string tipoValidacion, string valor, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarValidacion(idSolicitud, tipoValidacion, valor, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;

        }

        public Solicitud ObtenerDatosSolicitud(string idSolicitud, DateTime fecCotizacion)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Solicitud solicitud = cotizadorServicio.ObtenerDatosSolicitud(idSolicitud, fecCotizacion);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Actividad> ListarActividad(string cuspp)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Actividad> actividad = cotizadorServicio.ListarActividad(cuspp);
                return actividad;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Producto> ListarProducto(string idCategoria)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Producto> productos = generalesServicio.ListarProducto(idCategoria);
                return productos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarLog(LogBD logbd)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.RegistrarLog(logbd);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public List<Agente> ListarAgente(string usuario, string rol)
        {
            try
            {
                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                List<Agente> agentes = usuarioServicio.ListarAgentes(usuario, rol);
                return agentes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Agente> ListarAgenteExterno(string gls_agente, string usuario)
        {
            try
            {
                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                List<Agente> agentes = usuarioServicio.ListarAgenteExterno(gls_agente, usuario);
                return agentes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public Agente ObtenerUltimoAgentePorCartera(string cartera, string usuario)
        {
            try
            {
                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                Agente agente = usuarioServicio.ObtenerUltimoAgentePorCartera(cartera, usuario);
                return agente;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public string obtenerNumAgente(string codUsuario)
        {
            string numAgente = "";
            IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
            numAgente = usuarioServicio.obtenerNumAgente(codUsuario);
            return numAgente;
        }

        public List<Usuario> ListarUsuario(string nombreUsuario, string idAgente)
        {
            try
            {
                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                List<Usuario> usuarios = usuarioServicio.ListarUsuario(nombreUsuario, idAgente);
                return usuarios;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<SRI.INI-20322_E2>
        public List<Seguimiento> ListarSeguimiento(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Seguimiento> seguimientos = cotizadorServicio.ListarSeguimiento(idJefe, idSupervisor, idAgente, cuspp, fechaInicio, fechaTermino, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar, ref totalRegistros);
                return seguimientos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Seguimiento> ListarExcelSeguimiento(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Seguimiento> seguimientos = cotizadorServicio.ListarExcelSeguimiento(idJefe, idSupervisor, idAgente, cuspp, fechaInicio, fechaTermino, columnaOrdenar, direccionOrdenar);
                return seguimientos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Supervision> ListarSupervision(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Supervision> supervisiones = cotizadorServicio.ListarSupervision(idJefe, idSupervisor, idAgente, fechaInicio, fechaTermino, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar, ref totalRegistros);
                return supervisiones;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Supervision> ListarExcelSupervision(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Supervision> supervisiones = cotizadorServicio.ListarExcelSupervision(idJefe, idSupervisor, idAgente, fechaInicio, fechaTermino, columnaOrdenar, direccionOrdenar);
                return supervisiones;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<ParametroCotizadorWS> ObtenerParametrosCotizacion(string idSolicitud, DateTime fechaCotizacion, Int64? numCorrelativo, int anhosAdicionales)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<ParametroCotizadorWS> parametros = cotizadorServicio.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, numCorrelativo, anhosAdicionales, null);
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Cotizacion Cotizar(string idSolicitud, DateTime fechaCotizacion, Int64 numCorrelativo, int anhosAdicionales, double nuevoCIC)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Cotizacion cotizacion = cotizadorServicio.Cotizar(idSolicitud, fechaCotizacion, numCorrelativo, anhosAdicionales, nuevoCIC);
                return cotizacion;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Cotizacion CotizarConParametros(ParametroCotizadorWS p)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Cotizacion cotizacion = cotizadorServicio.Cotizar(p);
                return cotizacion;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //public Respuesta Cotizar(string idSolicitud, string usuario)
        //{
        //    Respuesta respuesta = new Respuesta();
        //    try
        //    {
        //        ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
        //        cotizadorServicio.Cotizar(idSolicitud, usuario);
        //        respuesta.Estado = Constante.COD_OK;
        //        respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
        //        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
        //        respuesta.Mensaje = "Solicitud cotizada correctamente.";
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message, ex);
        //        respuesta.Estado = Constante.COD_ERROR;
        //        respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        //        respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        //        respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
        //    }
        //    return respuesta;
        //}

        //<SRIINI06326>

        #region Monto CIC

        public List<MontoCIC> ListarMontoCIC()
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<MontoCIC> montos = generalesServicio.ListarMontoCIC();
                return montos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }

        }

        public Respuesta RegistrarMontoCIC(MontoCIC entity)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.RegistrarMontoCIC(entity);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Registro ingresado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarMontoCIC(MontoCIC entity)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.ActualizarMontoCIC(entity);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Registro actualizado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta EliminarMontoCIC(MontoCIC entity)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.EliminarMontoCIC(entity);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Registro eliminado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        #endregion

        #region reporte de escenarios

        public Respuesta GenerarReporteEscenarios(string idSolicitud, DateTime fechaCotizacion, string usuario, string maxAcom)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();

                cotizadorServicio.GenerarReporteEscenarios(idSolicitud, fechaCotizacion, usuario, maxAcom);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                //respuesta.Data = data;
                return respuesta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);

                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = ex.Message;

                return respuesta;
            }

        }

        public DataSet ObtenerReporteEscenarios(string idSolicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();

                var data = cotizadorServicio.ObtenerReporteEscenarios(idSolicitud, usuario);

                return data;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                return null;
            }
        }

        #endregion

        //<SRIFIN06326>

        //<SRIINI10693>

        #region ROLACOM
        public List<RolAcom> ListarRolAcom(RolAcom rolAcom)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<RolAcom> roles = cotizadorServicio.ListarRolAcom(rolAcom);
                return roles;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        public List<RolAcom> ListaAcomEscenario(RolAcom rolAcom)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<RolAcom> roles = cotizadorServicio.ListaAcomEscenario(rolAcom);
                return roles;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion

        //<SRIFIN10693>

        //<SRI.INI-20322_E2>

        #region ROLDCOM
        public List<RolDcom> ListarRolDcom(RolDcom rolDcom)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<RolDcom> roles = cotizadorServicio.ListarRolDcom(rolDcom);
                return roles;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<GTIINI-10761>
        public List<RolDcom> ListarRolDcomRPP(RolDcom rolDcom)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<RolDcom> roles = cotizadorServicio.ListarRolDcomRPP(rolDcom);
                return roles;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<GTIFIN-10761>

        public List<RolDcom> ListaDcomEscenario(RolDcom rolDcom)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<RolDcom> roles = cotizadorServicio.ListaDcomEscenario(rolDcom);
                return roles;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion

        //<SRI.FIN-20322_E2>

        //<SRIINI20322>

        #region ROLDTRA
        public List<RolDtra> ListarRolDtra(RolDtra rolDtra)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<RolDtra> roles = cotizadorServicio.ListarRolDtra(rolDtra);
                return roles;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion

        //<SRIFIN20322>

        //<SRIINI20322>

        #region Token de Acceso
        public SolicitudAcceso ValidarToken(string token)
        {
            try
            {
                // Obtener la direcció IP del cliente que consume el método
                OperationContext context = OperationContext.Current;
                MessageProperties prop = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string ip = endpoint.Address;

                log.Debug(String.Format("Se ha solicitado acceso al Cotizador Web de Rentas desde otra aplicación a través de la siguiente IP [{0}] con la siguiete clave Token [{1}]", ip, token));

                // Invocar al método de validación del Token
                ISeguridadServicio seguridadServicio = FabricaIoC.Contenedor.Resolver<ISeguridadServicio>();
                SolicitudAcceso solicitud = seguridadServicio.ValidarToken(token, ip);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
               throw;
            }
        }

        public Respuesta ActualizarSolicitudAcceso(SolicitudAcceso solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ISeguridadServicio seguridadServicio = FabricaIoC.Contenedor.Resolver<ISeguridadServicio>();
                seguridadServicio.ActualizarSolicitudAcceso(solicitud);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud de Acceso actualizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }
        #endregion

        #region Correo Electrónico



        public Respuesta EnviarCorreoElectronicoPoliza(CorreoElectronico correo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                MailMessage mensaje = new MailMessage();

                mensaje.To.Add(new MailAddress(correo.Para, correo.ParaNombre, System.Text.Encoding.UTF8));
                mensaje.Bcc.Add(new MailAddress(correo.De, correo.DeNombre, System.Text.Encoding.UTF8));
                mensaje.From = new MailAddress(correo.De, correo.DeNombre, System.Text.Encoding.UTF8);
                mensaje.Subject = correo.Asunto;
                mensaje.SubjectEncoding = System.Text.Encoding.UTF8;
                mensaje.Body = correo.Mensaje;
                mensaje.BodyEncoding = System.Text.Encoding.UTF8;
                mensaje.IsBodyHtml = correo.Html;
                if (correo.BinarioAdjunto != null)
                {
                    mensaje.Attachments.Add(new Attachment(new MemoryStream(correo.BinarioAdjunto), correo.Adjunto));
                }



                //log.Info(String.Format("Se va a establecer conexión con el Servidor SMTP[{0}] Puerto[{1}].",
                //    ConfigurationManager.AppSettings["DominioSMTP"],
                //    ConfigurationManager.AppSettings["PuertoSMTP"]));
                //SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["DominioSMTP"], Convert.ToInt32(ConfigurationManager.AppSettings["PuertoSMTP"]));

                //client.EnableSsl = true;
                //client.UseDefaultCredentials = true;
                //client.Credentials = credenciales;


                SmtpClient client = new SmtpClient("mail.iqproject.pe");
                client.Port = 587;
                client.Credentials = new NetworkCredential("wcervantes@iqproject.pe", "California31.");
                client.EnableSsl = true;


                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                log.Debug(String.Format("Usuario va a enviar correo electrónico con cotización adjunta a la dirección[{0} <{1}>].",
                correo.ParaNombre, correo.Para));
                client.Send(mensaje);
                log.Info(String.Format("Correo electrónico enviado correctamente a la dirección[{0} <{1}>].",
                correo.ParaNombre, correo.Para));

                respuesta.Estado = Constante.COD_OK;
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Correo electrónico enviado correctamente." });

            }
            catch (Exception ex)
            {

                log.Error(String.Format("Error al enviar correo electrónico a la dirección[{0} <{1}>].",
                        correo.ParaNombre, correo.Para));
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }

            return respuesta;
        }


        public Respuesta EnviarCorreoElectronico(CorreoElectronico correo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                //<INIGTI_4081>
                Object semilla = new object();
                lock (semilla)
                {
                    MailMessage mensaje = new MailMessage();

                    mensaje.To.Add(new MailAddress(correo.Para, correo.ParaNombre, System.Text.Encoding.UTF8));
                    mensaje.Bcc.Add(new MailAddress(correo.De, correo.DeNombre, System.Text.Encoding.UTF8));
                    mensaje.From = new MailAddress(correo.De, correo.DeNombre, System.Text.Encoding.UTF8);
                    mensaje.Subject = correo.Asunto;
                    mensaje.SubjectEncoding = System.Text.Encoding.UTF8;
                    mensaje.Body = correo.Mensaje;
                    mensaje.BodyEncoding = System.Text.Encoding.UTF8;
                    mensaje.IsBodyHtml = correo.Html;
                    if (correo.BinarioAdjunto != null)
                    {
                        mensaje.Attachments.Add(new Attachment(new MemoryStream(correo.BinarioAdjunto), correo.Adjunto));
                    }


                    log.Info(String.Format("Se va a establecer conexión con el Servidor SMTP[{0}] Puerto[{1}].",
                        ConfigurationManager.AppSettings["DominioSMTP"],
                        ConfigurationManager.AppSettings["PuertoSMTP"]));
                    SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["DominioSMTP"], Convert.ToInt32(ConfigurationManager.AppSettings["PuertoSMTP"]));

                    //client.EnableSsl = true;
                    client.UseDefaultCredentials = true;
                    //client.Credentials = credenciales;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    log.Debug(String.Format("Usuario va a enviar correo electrónico con cotización adjunta a la dirección[{0} <{1}>].",
                        correo.ParaNombre, correo.Para));
                    client.Send(mensaje);
                    log.Info(String.Format("Correo electrónico enviado correctamente a la dirección[{0} <{1}>].",
                        correo.ParaNombre, correo.Para));

                    respuesta.Estado = Constante.COD_OK;
                    respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Correo electrónico enviado correctamente." });
                }
                //<FINGTI_4081> 
            }
            catch (Exception ex)
            {
                //<INIGTI_4081>
                log.Error(String.Format("Error al enviar correo electrónico a la dirección[{0} <{1}>].",
                        correo.ParaNombre, correo.Para));
                //<FINGTI_4081>

                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }

            return respuesta;
        }

        public void EnviarCorreoElectronicoAsincrono(CorreoElectronico correo)
        {
            //Envio de manera Asincrono
            var tareaParalela = new System.Threading.Tasks.Task(() =>
            {
                EnviarCorreoElectronico(correo);
            });
            tareaParalela.Start();
        }
        #endregion

        #region Meler
        public Respuesta RegistrarDescargaSolicitudes(string xml, string usuario, ref int lote)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                lote = cotizadorServicio.RegistrarDescargaSolicitudes(xml, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = string.Format("Solicitudes cargadas correctamente. Lote N° {0} generado.", lote);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public List<Solicitud> RegistrarDescargaResultados(string xml, string usuario, ref int lote)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Solicitud> solicitudes = cotizadorServicio.RegistrarDescargaResultados(xml, usuario, ref lote);
                return solicitudes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Lote> ListarLotePorNumero(int numero)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Lote> lotes = cotizadorServicio.ListarLote(numero);
                return lotes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Lote> ListarLoteResultadoPorNumero(int numero)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Lote> lotes = cotizadorServicio.ListarLoteResultado(numero);
                return lotes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Lote> ListarLotePorFecha(DateTime fechaCierreInicial, DateTime fechaCierreFinal)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Lote> lotes = cotizadorServicio.ListarLote(fechaCierreInicial, fechaCierreFinal);
                return lotes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Lote> ListarLoteResultadoPorFecha(DateTime fechaCierreInicial, DateTime fechaCierreFinal)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Lote> lotes = cotizadorServicio.ListarLoteResultado(fechaCierreInicial, fechaCierreFinal);
                return lotes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Solicitud> ListarSolicitudesPorLote(int lote)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Solicitud> solicitudes = cotizadorServicio.ListarSolicitud(lote);
                return solicitudes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion

        #region Anticipo
        public Anticipo ObtenerDatosAnticipo(string solicitud)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Anticipo anticipo = cotizadorServicio.ObtenerDatosAnticipo(solicitud);
                return anticipo;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Anticipo ObtenerDatosAnticipoAceptacion(string solicitud, string agente)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Anticipo anticipo = cotizadorServicio.ObtenerDatosAnticipoAceptacion(solicitud, agente);
                return anticipo;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Anticipo ObtenerDatosAnticipoCondiciones(string solicitud)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Anticipo anticipo = cotizadorServicio.ObtenerDatosAnticipoCondiciones(solicitud);
                return anticipo;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Anticipo> ListarAnticipoAceptacion(string solicitud, string agente, string fechaInicio, string fechaFin, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar)
        {
            try
            {
                DateTime? dFechaInicio = null;
                DateTime? dFechaFin = null;
                if (fechaInicio != String.Empty)
                    dFechaInicio = Convert.ToDateTime(fechaInicio, new CultureInfo("es-PE"));

                if (fechaFin != String.Empty)
                    dFechaFin = Convert.ToDateTime(fechaFin, new CultureInfo("es-PE"));

                if (agente == String.Empty)
                    agente = "0";

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Anticipo> anticipos = cotizadorServicio.ListarAnticipoAceptacion(solicitud, agente, dFechaInicio, dFechaFin, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar);
                return anticipos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarAnticipoAceptacion(Anticipo anticipo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarAnticipoAceptacion(anticipo);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue(); ;
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }
        #endregion

        //<SRIFIN20322>

        //<SOLINI25621>

        public Respuesta ActualizarParametro(Parametro entity)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.ActualizarParametro(entity);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Registro actualizado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta CotizarCapitalRequerido(ref CapitalRequerido capitalRequerido)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.CotizarCapitalRequerido(ref capitalRequerido);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Capital requerido calculado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        //<SOLFIN25621>

        //<SOLINI26593>

        #region RentaPrivada
        public List<SolicitudRP> ListarSolicitudRP(string cuspp)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudRP> solicitud = cotizadorServicio.ListarSolicitudRP(cuspp);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public SolicitudRP ObtenerDatosSolicitudRP(string idSolicitud, DateTime fecCotizacion)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                SolicitudRP solicitudRP = cotizadorServicio.ObtenerDatosSolicitudRP(idSolicitud, fecCotizacion);
                return solicitudRP;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarSolicitudRP(ref SolicitudRP solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarSolicitudRP(ref solicitud);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarSolicitudRP(ref SolicitudRP solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarSolicitudRP(ref solicitud);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }
        #endregion

        //<SOLFIN26593>

        //<INIGTI_1092>

        public string SolicitudesHabilitadas(int lote, string solicitudes)
        {
            try
            {
                string nroSolicitudes = "";
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                nroSolicitudes = cotizadorServicio.SolicitudesHabilitadas(lote, solicitudes);
                return nroSolicitudes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FINGTI_1092>

        //<INIGTI_754>

        public ParametroCotizadorWS ObtenerParametroCotizacionWS(string idSolicitud, DateTime fechaCotizacion, long? numCorrelativo, int anhosAdicionales)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<ParametroCotizadorWS> parametros = cotizadorServicio.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, numCorrelativo, anhosAdicionales, null);
                return parametros[0];
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FINGTI_754>

        //<INIGTI_753>

        #region RentaPrivadaPlus

        public List<SolicitudRPPlus> ListarSolicitudRPPlus(string cuspp)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudRPPlus> solicitud = cotizadorServicio.ListarSolicitudRPPlus(cuspp);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public SolicitudRPPlus ObtenerDatosSolicitudRPPlus(string idSolicitud)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                SolicitudRPPlus solicitudRPPlus = cotizadorServicio.ObtenerDatosSolicitudRPPlus(idSolicitud);
                return solicitudRPPlus;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarSolicitudRPPlus(ref SolicitudRPPlus solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarSolicitudRPPlus(ref solicitud);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarSolicitudRPPlus(ref SolicitudRPPlus solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarSolicitudRPPlus(ref solicitud);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        #endregion

        //<FINGTI_753>

        //<INIGTI_4081>

        #region ObtenerDTRA
        public Respuesta ObtenerDTra(ref Solicitud solicitud, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ObtenerDTra(ref solicitud, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }
        #endregion

        #region ObtenerFlujos
        public List<FlujoMovimiento> ObtenerFlujos(DateTime fecCotizacion, string evento, string rol)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<FlujoMovimiento> flujos = cotizadorServicio.ObtenerFlujos(fecCotizacion, evento, rol);
                return flujos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion

        #region CotizarDifTRA
        public Respuesta CotizarDifTRA(ref Solicitud solicitud, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.CotizarDifTRA(ref solicitud, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }
        #endregion

        #region CotizarDifTRAPublic
        public Respuesta CotizarDifTRAPublic(ref Solicitud solicitud, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.CotizarDifTRA(ref solicitud, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }
        #endregion


        public List<ParametroEspecial> ObtenerTraDefault(string idSolicitud)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<ParametroEspecial> parametros = generalesServicio.ObtenerTraDefault(idSolicitud);
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<ParametroEspecial> ObtenerTasaMaximaTraMinima(string idSolicitud, DateTime fecCotizacion)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<ParametroEspecial> parametros = generalesServicio.ObtenerTasaMaximaTraMinima(idSolicitud, fecCotizacion);
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarCotizaValPar(List<ParametroEspecial> parametros, string idUsuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.RegistrarCotizaValPar(parametros, idUsuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public List<SolicitudEscenario> ListarSolicitudesEmail(string idSolicitudes)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudEscenario> solicitudes = cotizadorServicio.ListarSolicitudesEmail(idSolicitudes);
                return solicitudes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public CuotasTra ObtenerCuotasTra(CuotasTra rolCuotas)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                CuotasTra rolCuotasTra = cotizadorServicio.ObtenerCuotasTra(rolCuotas);
                return rolCuotasTra;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<SolicitudEscenario> ListarSolicitudesValidaFlujo(string NumSolicitud, string NumOperacion)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudEscenario> solicitudes = cotizadorServicio.ListarSolicitudesValidaFlujo(NumSolicitud, NumOperacion);
                return solicitudes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public bool ValidaFlujoSolicitudRol(string num_solicitud, string rol, string evento)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                bool valida = cotizadorServicio.ValidaFlujoSolicitudRol(num_solicitud, evento, rol);
                return valida;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<CuotasTra> ListarCuotasTra(int periodo, int mes)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<CuotasTra> listaCuotas = cotizadorServicio.ListarCuotasTra(periodo, mes);
                return listaCuotas;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarCuotas(List<CuotasTra> lstCuotas, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarCuotas(lstCuotas, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public List<GestionVentas> ConsultarGestionVentas(DateTime fechaInicial, DateTime fechaFinal, int numJefe, int numSuperv, int numAgente, string indCierre, string tipoCotizacion, string codCiaSeguro)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<GestionVentas> lstGestionVentas = cotizadorServicio.ConsultarGestionVentas(fechaInicial, fechaFinal, numJefe, numSuperv, numAgente, indCierre, tipoCotizacion, codCiaSeguro);
                return lstGestionVentas;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FINGTI_4081>

        //<INIGTI_4081_2>

        public Respuesta ActualizarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarCotizacionMovimiento(XMLCotizacionMovimiento, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Movimientos TRA de Cotizaciones Actualizados correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        //<FINGTI_4081_2>

        //<INIGTI_4081_3>

        public Respuesta RegistrarFlujoSolicitudCompleto(ref Respuesta rpta, ref Solicitud solicitud, string XML_CotizacionMovimiento, string codUserName, string codRol)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarFlujoSolicitudCompleto(ref rpta, ref solicitud, XML_CotizacionMovimiento, codUserName, codRol);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Movimientos TRA de Cotizaciones Actualizados correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        //<FINGTI_4081_3>

        //<INIGTI_6556>

        public List<SolicitudEscenario> ListarSolicitudesPendientesEmail(string cod_rol)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudEscenario> solicitudes = cotizadorServicio.ListarSolicitudesPendientesEmail(cod_rol);
                return solicitudes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FINGTI_6556>

        public Respuesta VistaPreviaCotizacionPlus(string num_solicitud, int num_correlativo, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.VistaPreviaCotizacionPlus(num_solicitud, num_correlativo, usuario);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Datos para vista previa actualizados correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        //<INIGTI_7012>
        public Respuesta CerrarCotizacionPlus(string num_solicitud, int num_correlativo, string usuario, List<GrupoFamiliar> lstGrupoFamiliar)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.CerrarCotizacionPlus(num_solicitud, num_correlativo, usuario, lstGrupoFamiliar);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Cotización Cerrada Correctamente.";//"Movimientos TRA de Cotizaciones Actualizados correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta GenerarPolizaPlus(string num_solicitud, int num_correlativo, string usuario, GrupoFamiliar grup_fam)
        {
            Respuesta respuesta = new Respuesta();
            try
            {

                log.Debug("Inicio ServicioCWRV.GenerarPolizaPlus ADMWR");

                //<INI.GTI_7012_7>
                string numPoliza = "";
                string mensaje = "";
                //<FIN.GTI_7012_7>
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                numPoliza = cotizadorServicio.GenerarPolizaPlus(num_solicitud, num_correlativo, usuario, grup_fam, ref mensaje);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = numPoliza;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }

            log.Debug("Fin ServicioCWRV.GenerarPolizaPlus ADMWR");

            return respuesta;
        }

        public Respuesta AnularSolicitudPlus(string num_solicitud, string usuario, string cod_causante)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.AnularSolicitudPlus(num_solicitud, usuario, cod_causante);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud Anulada Correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public List<CausalPoliza> ListarCausalPolizaPlus()
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<CausalPoliza> lstCausalPoliza = cotizadorServicio.ListarCausalPolizaPlus();
                return lstCausalPoliza;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FINGTI_7012>

        //<INIGTI_7012>

        //public List<Temporal> ObtenerTemporalesPorTabla(string tabla)
        //    {
        //    try
        //    {
        //        IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
        //        List<Temporal> parametros = generalesServicio.ObtenerTemporalesPorTabla(tabla);
        //        return parametros;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message, ex);
        //        throw;
        //    }
        //}

        public List<DatosSol> ObtenerDatosporSolicitud(string num_Solicitud)
        {
            try
            {
                ICotizadorServicio generalesServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<DatosSol> datosSolicitud = generalesServicio.ObtenerDatosporSolicitud(num_Solicitud);
                return datosSolicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FINGTI_7012>

        //<INIGTI_7012>

        public List<Temporal> ListarGruposFamiliaresxSolicitud(string num_solicitud)
        {
            try
            {
                ICotizadorServicio generalesServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Temporal> solicitud = generalesServicio.ListarGruposFamiliaresxSolicitud(num_solicitud);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Parametro> ObtenerParametros(string tabla)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Parametro> parametros = generalesServicio.ObtenerParametros(tabla);
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Parametro> ObtenerNroBancos(string tabla, string tipoBanco, string tipoCuenta)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Parametro> parametros = generalesServicio.ObtenerNroBancos(tabla, tipoBanco, tipoCuenta);
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FINGTI_7012>

        //<INI.GTI_7012_2>

        public List<Agente> ObtenerAgenteDeudaAcom(string idAgente)
        {
            try
            {
                ICotizadorServicio generalesServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Agente> lstAgente = generalesServicio.ObtenerAgenteDeudaAcom(idAgente);
                return lstAgente;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FIN.GTI_7012_2>

        //<INI.GTI_7012_2_1>

        public List<SolicitudRPPlus> ListarReporteCotizacionPlus(string cuspp)
        {
            try
            {
                ICotizadorServicio generalesServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudRPPlus> lstSolicitud = generalesServicio.ListarReporteCotizacionPlus(cuspp);
                return lstSolicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FIN.GTI_7012_2_1>

        //<INI.GTI_7012_3>
        public EmisionPoliza EmitirPoliza(string num_solicitud, int num_poliza, string dig_poliza, string TipoProducto)
        {
            try
            {
                ICotizadorServicio generalesServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                EmisionPoliza emisionPoliza;

                if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                {
                    emisionPoliza = generalesServicio.EmitirPoliza(num_solicitud, num_poliza, dig_poliza);
                }
                else
                {
                    emisionPoliza = generalesServicio.EmitirPolizaIFP(num_solicitud, num_poliza, dig_poliza);
                }

                return emisionPoliza;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FIN.GTI_7012_3>
        public List<byte[]> EmitirPolizaPDF(string num_solicitud, int num_poliza, string dig_poliza, GrupoFamiliar grupoFamiliar, string TipoProducto)
        {
            //string PDFAceptacionNew = string.Empty;
            //string sPdfCartaNew = string.Empty;
            //string sPdfPolizaNew = string.Empty;
            //string PDFPolizaCompleta = string.Empty;
            //string sPdfCondicionGeneralNew = string.Empty;
            //string sPdfResumenNew = string.Empty;

            //PdfReader oPdfCartaReader = null;
            //PdfStamper oPdfCartaJoin = null;
            //AcroFields oPdfCartaFields = null;

            //PdfReader oPdfPolizaReader = null;
            //PdfStamper oPdfPolizaJoin = null;
            //AcroFields oPdfPolizaFields = null;

            //PdfReader oPdfAceptacionReader = null;
            //PdfStamper oPdfAceptacionJoin = null;
            //AcroFields oPdfAceptacionFileds = null;

            //PdfReader oPdfCondicionGeneralReader = null;
            //PdfStamper oPdfCondicionGeneralJoin = null;
            //AcroFields oPdfCondicionGeneralFields = null;

            //PdfReader oPdfsPdfResumenNewReader = null;
            //PdfStamper oPdfsPdfResumenNewJoin = null;
            //AcroFields oPdfsPdfResumenNewFields = null;

            //Document doc = null;

            //string sPdfAceptacionTemplate = string.Empty;
            //string sPdfCartaTemplate = string.Empty;
            //string sPdfResumenTemplate = string.Empty;
            //string sPdfCndGeneralTemplate = string.Empty;
            //string sPdfPolizaTemplate = string.Empty;
            //string sPdfDevolucionTemplate = string.Empty;
            //string sPdfSepelioTemplate = string.Empty;
            //string sPdfPagoDobleTemplate = string.Empty;
            //string sPdfPeriodoGarantizadoTemplate = string.Empty;
            //string sPdfDevolucionPrimaFallecimientoTemplate = string.Empty;
            //string sPdfDevolucionPrimaSobrevivenciaTemplate = string.Empty;

            try
            {
                string url = ConfigurationManager.AppSettings["url_generadorpoliza"].ToString();
                string polizaIn = ConfigurationManager.AppSettings["polizaRPPIN"].ToString();
                string polizaOut = ConfigurationManager.AppSettings["polizaRPPOUT"].ToString();

                JsonArchivos jsonArchivos = new JsonArchivos();
                List<Archivo> lstArchivos = new List<Archivo>();
                List<Archivo> lstArchivosCarta = new List<Archivo>();
                List<byte[]> lstArchivosByte = new List<byte[]>();

                string PDFPoliza = string.Empty;
                string PDFAceptacion = string.Empty;

                string sexo = string.Empty;
                string telefono = string.Empty;
                string factorajusterenta = string.Empty;
                string fechafinvigencia = string.Empty;
                string reembolsogastossepelio = string.Empty;
                string clausulaperiodogarantizado = string.Empty;
                string fechaclausulaperiodogarantizado = string.Empty;
                string pagodoble = string.Empty;
                string aniospagodoble = string.Empty;
                string devolucionprimaUnica = string.Empty;

                log.Debug(string.Format("Inicio EmitirPoliza Solicitud Nro: {0}, Poliza Nro: {1}, DigPoliza: {2}", num_solicitud, num_poliza, dig_poliza));
                EmisionPoliza emisionPoliza = EmitirPoliza(num_solicitud, num_poliza, dig_poliza, TipoProducto);

                ////Archivos Plantillas
                //if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                //{
                //sPdfAceptacionTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\Carta de Aceptación.pdf";
                //sPdfCartaTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\Carta de Bienvenida.pdf";
                //sPdfResumenTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\Resumen Renta Particular Plus.pdf";
                //sPdfCndGeneralTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\CG RENTA PARTICULAR PLUS.pdf";
                //sPdfPolizaTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\CP Renta Particular Plus.pdf";

                //sPdfDevolucionTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\CA Devolución de prima a todo evento nueva.pdf";
                //sPdfSepelioTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\CA Gastos de Sepelio.pdf";
                //sPdfPagoDobleTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\CA Pago Doble por n años.pdf";
                //sPdfPeriodoGarantizadoTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\CA Período Garantizado.pdf";

                //Nuevos Archivos
                //PDFAceptacionNew = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\Aceptacion.pdf";
                //sPdfCartaNew = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\Carta.pdf";
                //sPdfPolizaNew = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\Poliza.pdf";
                //PDFPolizaCompleta = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RPP\\PolizaCompleta.pdf";
                //}

                ////Eliminando Archivos
                //log.Debug("Eliminado Archivos Existentes");

                //if (File.Exists(PDFAceptacionNew))
                //    File.Delete(PDFAceptacionNew);

                //if (File.Exists(sPdfCartaNew))
                //    File.Delete(sPdfCartaNew);

                ////if (File.Exists(sPdfCarnetNew))
                ////    File.Delete(sPdfCarnetNew);

                //if (File.Exists(sPdfPolizaNew))
                //    File.Delete(sPdfPolizaNew);

                //if (File.Exists(PDFPolizaCompleta))
                //    File.Delete(PDFPolizaCompleta);

                //if (File.Exists(sPdfCondicionGeneralNew))
                //    File.Delete(sPdfCondicionGeneralNew);

                //if (File.Exists(sPdfResumenNew))
                //    File.Delete(sPdfResumenNew);

                //Lista de Archivos a Unir a uno solo.
                //lstArchivos.Add(sPdfAceptacionNew);

                //if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                //{
                ////Carta Aceptacion
                ////----------------------------------------------------------------
                //log.Debug("Creando Archivo: " + sPdfAceptacionNew);
                //oPdfAceptacionReader = new PdfReader(sPdfAceptacionTemplate);
                //oPdfAceptacionJoin = new PdfStamper(oPdfAceptacionReader, new FileStream(sPdfAceptacionNew, FileMode.Create));
                //oPdfAceptacionFileds = oPdfAceptacionJoin.AcroFields;

                //oPdfAceptacionFileds.SetField("ApellidosNombres", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].ApellidosNombres);
                //oPdfAceptacionFileds.SetField("ApellidoPaterno", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].ApellidoPaterno + ":");

                //oPdfAceptacionFileds.SetField("Fecha", DateTime.Today.ToString("dd 'de' MMMM 'de' yyyy", CultureInfo.CreateSpecificCulture("es-PE")));
                //oPdfAceptacionFileds.SetField("Direccion", grupoFamiliar.Direccion.ToUpper());
                //oPdfAceptacionFileds.SetField("Distrito", grupoFamiliar.Distrito + " - " + grupoFamiliar.Provincia + " - " + grupoFamiliar.Departamento);
                //oPdfAceptacionFileds.SetField("NroPoliza", emisionPoliza.Poliza.NumPoliza.ToString());

                ////Deshabilita los campos, para no editar
                //oPdfAceptacionJoin.FormFlattening = true;

                ////Cierra el PDF
                //oPdfAceptacionJoin.Close();
                //oPdfAceptacionJoin.Dispose();

                //oPdfAceptacionReader.Close();
                //oPdfAceptacionReader.Dispose();
                //}

                ////Carta Bienvenida
                ////----------------------------------------------------------------
                //log.Debug("Creando Archivo: " + sPdfCartaNew);
                //oPdfCartaReader = new PdfReader(sPdfCartaTemplate);
                //oPdfCartaJoin = new PdfStamper(oPdfCartaReader, new FileStream(sPdfCartaNew, FileMode.Create));
                //oPdfCartaFields = oPdfCartaJoin.AcroFields;

                //if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                //{

                log.Debug("Creando información Carta");
                Carta cartaBienvenida = cartasRPP(emisionPoliza, grupoFamiliar);

                //}

                ////Deshabilita los campos, para no editar
                //oPdfCartaJoin.FormFlattening = true;

                ////Cierra el PDF
                //oPdfCartaJoin.Close();
                //oPdfCartaJoin.Dispose();

                //oPdfCartaReader.Close();
                //oPdfCartaReader.Dispose();

                //Poliza
                //----------------------------------------------------------------

                //log.Debug("Creando Archivo: " + sPdfPolizaNew);
                log.Debug("Creando Póliza");

                //oPdfPolizaReader = new PdfReader(sPdfPolizaTemplate);
                //oPdfPolizaJoin = new PdfStamper(oPdfPolizaReader, new FileStream(sPdfPolizaNew, FileMode.Create));
                //oPdfPolizaFields = oPdfPolizaJoin.AcroFields;

                ////1
                //oPdfPolizaFields.SetField("NroPoliza", "N° POLIZA VI" + emisionPoliza.Poliza.NumPoliza.ToString());

                //////2
                ////if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                ////{
                //    oPdfPolizaFields.SetField("ApellidosNombres", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].ApellidosNombres);
                //    oPdfPolizaFields.SetField("DocIdentidad", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Identificacion.GlosaTipo);

                //switch (emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Identificacion.IdTipo.ToUpper())
                //{
                //    case "D":
                //        //oPdfPolizaFields.SetField("NumeroDocIdentidad", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Identificacion.Numero.Value.ToString("00000000"));
                //        oPdfPolizaFields.SetField("NumeroDocIdentidad", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Identificacion.Numero.ToString());
                //        break;
                //    case "E":
                //        //oPdfPolizaFields.SetField("NumeroDocIdentidad", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Identificacion.Numero.Value.ToString("000000000"));
                //        oPdfPolizaFields.SetField("NumeroDocIdentidad", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Identificacion.Numero.ToString());
                //        break;
                //    default:
                //        oPdfPolizaFields.SetField("NumeroDocIdentidad", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Identificacion.Numero.ToString());
                //        break;
                //}

                //    oPdfPolizaFields.SetField("FechaNacimiento", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].FechaNacimiento.Value.ToString("dd/MM/yyyy"));

                sexo = "FEMENINO";
                if (emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Sexo.ToString() == "M")
                {
                    sexo = "MASCULINO";
                }

                //}

                //oPdfPolizaFields.SetField("Direccion", grupoFamiliar.Direccion.ToUpper());
                //oPdfPolizaFields.SetField("Distrito", grupoFamiliar.Distrito);
                //oPdfPolizaFields.SetField("Provincia", grupoFamiliar.Provincia);
                //oPdfPolizaFields.SetField("Departamento", grupoFamiliar.Departamento);

                telefono = grupoFamiliar.Telefono1;
                if (grupoFamiliar.Telefono2 != "")
                {
                    if (telefono == "")
                    {
                        telefono = grupoFamiliar.Telefono2;
                    }
                    telefono += " - " + grupoFamiliar.Telefono2;
                }

                //oPdfPolizaFields.SetField("Telefono", Telefono);
                //oPdfPolizaFields.SetField("CorreoElectronico", grupoFamiliar.CorreoElectronico);

                //////3
                ////if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                ////{
                //oPdfPolizaFields.SetField("PlanSeguro", emisionPoliza.SolicitudRPPlus.TipoPlan.Nombre);
                //oPdfPolizaFields.SetField("MonedaPrima", emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Nombre);

                if (emisionPoliza.SolicitudRPPlus.Temporalidad.Id == "TVT")
                {
                    fechafinvigencia = "-";
                }
                else
                {
                    fechafinvigencia = "23.59 HORAS DEL " + emisionPoliza.Poliza.FecFinVigencia.ToString("dd/MM/yyyy");
                }

                //oPdfPolizaFields.SetField("PlazoVigencia", emisionPoliza.SolicitudRPPlus.Temporalidad.Nombre);
                ////}

                //oPdfPolizaFields.SetField("FechaEmision", emisionPoliza.Poliza.FecEmision.ToString("dd/MM/yyyy"));
                //oPdfPolizaFields.SetField("FechaInicioVigencia", "00.00 HORAS DEL " + emisionPoliza.Poliza.FecInicioVigencia.ToString("dd/MM/yyyy"));

                ////5
                //string FactorAjusteRenta = "";
                ////if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                ////{
                //oPdfPolizaFields.SetField("MonedaPagoRenta", emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Nombre);

                if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Moneda.Id == "001")
                    factorajusterenta = "IPC";
                else
                {
                    if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValMonAju == 0)
                        factorajusterenta = "SIN AJUSTE";
                    else
                        factorajusterenta = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValMonAju.ToString("#,##0.00") + "%";
                }

                //oPdfPolizaFields.SetField("MontoBaseRentaMensual", emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PensionCiaMO.ToString("#,##0.00"));
                ////}

                //oPdfPolizaFields.SetField("FactorAjusteRenta", FactorAjusteRenta);
                //oPdfPolizaFields.SetField("FechaInicioPagoRentas", emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy"));

                ////6
                //if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                //{
                reembolsogastossepelio = "NO";
                if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].IndGastoSepelio == "S")
                {
                    reembolsogastossepelio = "S/. " + emisionPoliza.ValGastoSepelio.ToString("#,##0.00");
                    //lstArchivos.Add(sPdfSepelioTemplate);//6
                }

                fechaclausulaperiodogarantizado = "NO APLICA";
                clausulaperiodogarantizado = "NO";
                if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PeriodoGarantizado > 0)
                {
                    clausulaperiodogarantizado = "SI";
                    fechaclausulaperiodogarantizado = Utilitarios.ConvertirNroLetras(emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PeriodoGarantizado);
                    fechaclausulaperiodogarantizado = fechaclausulaperiodogarantizado + " (" + emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PeriodoGarantizado.ToString() + ") AÑOS ";
                    fechaclausulaperiodogarantizado = fechaclausulaperiodogarantizado + "contados desde " + emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy") + " hasta el ";
                    fechaclausulaperiodogarantizado = fechaclausulaperiodogarantizado + emisionPoliza.Poliza.FecPago.AddYears(emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PeriodoGarantizado).AddDays(-1).ToString("dd/MM/yyyy");

                    //lstArchivos.Add(sPdfPeriodoGarantizadoTemplate);//7
                }

                //oPdfPolizaFields.SetField("FechaClausula", fechaclausulaperiodogarantizado);

                aniospagodoble = "NO APLICA";
                pagodoble = "NO";
                if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PagoEscalonada > 0)
                {
                    pagodoble = "SI";
                    aniospagodoble = Utilitarios.ConvertirNroLetras(emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PagoEscalonada);
                    aniospagodoble = aniospagodoble + " (" + emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PagoEscalonada + ") AÑOS ";
                    aniospagodoble = aniospagodoble + "contados desde " + emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy") + " hasta el ";
                    aniospagodoble = aniospagodoble + emisionPoliza.Poliza.FecPago.AddYears(Convert.ToInt32(emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PagoEscalonada)).AddDays(-1).ToString("dd/MM/yyyy");

                    //lstArchivos.Add(sPdfPagoDobleTemplate);//8
                }

                //oPdfPolizaFields.SetField("AñosPagoDoble", aniospagodoble);

                devolucionprimaUnica = "NO";
                if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValPjeDev > 0)
                {
                    devolucionprimaUnica = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValPjeDev.ToString("#,##0.00") + "%";
                }

                //if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValPjeDev > 0)
                //{
                //    lstArchivos.Add(sPdfDevolucionTemplate);//9
                //}

                //}

                //////7
                ////if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                ////{
                //    oPdfPolizaFields.SetField("Banco", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Banco.Glosa.ToString());
                //    oPdfPolizaFields.SetField("NumCuentaAhorros", emisionPoliza.SolicitudRPPlus.Beneficiarios[0].NumeroBanco);
                ////}

                //////8
                ////if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                ////{
                //    oPdfPolizaFields.SetField("PrimaComercial", emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Simbolo.ToString() + " " + emisionPoliza.Poliza.ValPrimaNeta.ToString("#,##0.00"));
                //    oPdfPolizaFields.SetField("IGV", emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Simbolo.ToString() + " " + emisionPoliza.Poliza.ValIva.ToString("#,##0.00"));
                //    oPdfPolizaFields.SetField("PrimaComercialIGV", emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Simbolo.ToString() + " " + emisionPoliza.Poliza.ValPrimaBruta.ToString("#,##0.00"));
                //    //<INI.GTI_7012_ADMWR>
                //    //oPdfPolizaFields.SetField("TIRGarantizada", emisionPoliza.SolicitudRPPlus.Cotizaciones[0].TasaRetornoAccionista.ToString("#,##0.00") + "%");
                //    oPdfPolizaFields.SetField("TIRGarantizada", emisionPoliza.SolicitudRPPlus.Cotizaciones[0].TasaVenta.ToString("#,##0.00") + "%");
                //    //<FIN.GTI_7012_ADMWR>
                ////}

                //oPdfPolizaFields.SetField("ComisionPromotor", emisionPoliza.PjeComision.ToString("#,##0.00") + "%");

                //////4
                //////Llenando los Beneficiarios
                ////if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                ////{
                //if (emisionPoliza.SolicitudRPPlus.Beneficiarios.Count > 1)
                //    {
                //        for (int i = 1; i <= emisionPoliza.SolicitudRPPlus.Beneficiarios.Count - 1; i++)
                //        {
                //            oPdfPolizaFields.SetField("ApellidoPaterno" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].ApellidoPaterno.ToUpper());
                //            oPdfPolizaFields.SetField("ApellidoMaterno" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].ApellidoMaterno.ToUpper());
                //            oPdfPolizaFields.SetField("Nombres" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Nombre.ToUpper());
                //            oPdfPolizaFields.SetField("DocIdentidad" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Identificacion.GlosaTipo.ToUpper());

                //            switch (emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Identificacion.IdTipo.ToUpper())
                //            {
                //                case "D":
                //                    //oPdfPolizaFields.SetField("NroDocIdentidad" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Identificacion.Numero.Value.ToString("00000000"));
                //                    oPdfPolizaFields.SetField("NroDocIdentidad" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Identificacion.Numero.ToString());
                //                    break;
                //                case "E":
                //                    //oPdfPolizaFields.SetField("NroDocIdentidad" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Identificacion.Numero.Value.ToString("000000000"));
                //                    oPdfPolizaFields.SetField("NroDocIdentidad" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Identificacion.Numero.ToString());
                //                    break;
                //                default:
                //                    oPdfPolizaFields.SetField("NroDocIdentidad" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Identificacion.Numero.ToString().ToUpper());
                //                    break;
                //            }

                //            oPdfPolizaFields.SetField("FechaNacimiento" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].FechaNacimiento.Value.ToString("dd/MM/yyyy"));
                //            oPdfPolizaFields.SetField("Parentesco" + i, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Parentesco.Nombre.ToUpper());
                //            oPdfPolizaFields.SetField("PorcentajeRenta" + i, (emisionPoliza.SolicitudRPPlus.Beneficiarios[i].ValPjeRenta * 100).ToString("#,##0.00"));
                //        }
                //    }
                ////}

                log.Debug("Creando información de CP");
                CondicionesParticulares condicionesParticulares = CondicionesParticularesRPP(emisionPoliza, grupoFamiliar, sexo, telefono, fechafinvigencia, factorajusterenta, reembolsogastossepelio, clausulaperiodogarantizado, fechaclausulaperiodogarantizado, pagodoble, aniospagodoble, devolucionprimaUnica);

                log.Debug("Creando información Carta");
                Carta cartaAceptacion = cartasRPP(emisionPoliza, grupoFamiliar);

                ////4
                ////Llenando los Beneficiarios
                if (emisionPoliza.SolicitudRPPlus.Beneficiarios.Count > 1)
                {
                    for (int i = 1; i <= emisionPoliza.SolicitudRPPlus.Beneficiarios.Count - 1; i++)
                    {
                        condicionesParticulares.GetType().GetProperty("ApePat" + i).SetValue(condicionesParticulares, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].ApellidoPaterno.ToUpper(), null);
                        condicionesParticulares.GetType().GetProperty("ApeMat" + i).SetValue(condicionesParticulares, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].ApellidoMaterno.ToUpper(), null);
                        condicionesParticulares.GetType().GetProperty("Nombre" + i).SetValue(condicionesParticulares, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Nombre.ToUpper(), null);
                        condicionesParticulares.GetType().GetProperty("DocIden" + i).SetValue(condicionesParticulares, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Identificacion.GlosaTipo.ToUpper(), null);
                        condicionesParticulares.GetType().GetProperty("NroDocIden" + i).SetValue(condicionesParticulares, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Identificacion.Numero.ToString(), null);
                        condicionesParticulares.GetType().GetProperty("FecNac" + i).SetValue(condicionesParticulares, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].FechaNacimiento.Value.ToString("dd/MM/yyyy"), null);
                        condicionesParticulares.GetType().GetProperty("Parent" + i).SetValue(condicionesParticulares, emisionPoliza.SolicitudRPPlus.Beneficiarios[i].Parentesco.Nombre.ToUpper(), null);
                        condicionesParticulares.GetType().GetProperty("PorcRta" + i).SetValue(condicionesParticulares, (emisionPoliza.SolicitudRPPlus.Beneficiarios[i].ValPjeRenta * 100).ToString("#,##0.00"), null);
                    }
                }

                lstArchivos.Add(new Archivo() { RutaWord = polizaIn + "CP Renta Particular Plus V2.docx", Trama = condicionesParticulares });//1 sPdfPolizaNew
                lstArchivos.Add(new Archivo() { RutaWord = polizaIn + "Carta de Bienvenida.docx", Trama = cartaAceptacion });//2 sPdfCartaNew
                lstArchivos.Add(new Archivo() { RutaWord = polizaIn + "Resumen Renta Particular Plus.docx" });//3 sPdfResumenTemplate
                lstArchivos.Add(new Archivo() { RutaWord = polizaIn + "CP Renta Particular Plus V2.docx", Trama = condicionesParticulares });//4 sPdfPolizaNew
                lstArchivos.Add(new Archivo() { RutaWord = polizaIn + "CG Renta Particular Plus.docx" });//5 sPdfCndGeneralTemplate

                if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].IndGastoSepelio == "S")
                {
                    lstArchivos.Add(new Archivo() { RutaWord = polizaIn + "CA Gastos de Sepelio.docx" });//6
                }

                if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PeriodoGarantizado > 0)
                {
                    lstArchivos.Add(new Archivo() { RutaWord = polizaIn + "CA Período Garantizado.docx" });//7
                }

                if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PagoEscalonada > 0)
                {
                    lstArchivos.Add(new Archivo() { RutaWord = polizaIn + "CA Pago Doble por n años.docx" });//8
                }

                if (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValPjeDev > 0)
                {
                    lstArchivos.Add(new Archivo() { RutaWord = polizaIn + "CA Devolución de prima a todo evento nueva.docx" });//9
                }

                lstArchivosCarta.Add(new Archivo() { RutaWord = polizaIn + "Carta de Aceptación.docx", Trama = cartaBienvenida });

                jsonArchivos.Archivos = lstArchivosCarta;
                jsonArchivos.RutaArchivo = polizaOut;
                jsonArchivos.NombreArchivo = "CartaBienvenida" + ".pdf";

                string json = JsonConvert.SerializeObject(jsonArchivos);

                var httpWebRequestCartaBienvenida = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequestCartaBienvenida.ContentType = "application/json;charset=utf-8";
                httpWebRequestCartaBienvenida.Method = "POST";

                try
                {
                    log.Debug("GenerarCartaBienvenida");
                    log.Debug("Url: " + url);
                    log.Debug(String.Format("Inicio Campos num_solicitud:{0} -------------------------------------", num_solicitud));
                    log.Debug(json);
                    log.Debug(String.Format("Fin Campos num_solicitud {0}----------------------------------------", num_solicitud));
                }
                catch (Exception _log)
                {
                    log.Error(_log.Message, _log);
                }

                using (var streamWriter = new StreamWriter(httpWebRequestCartaBienvenida.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponseCartaBienvenida = (HttpWebResponse)httpWebRequestCartaBienvenida.GetResponse();

                using (var streamReaderCartaBienvenida = new StreamReader(httpResponseCartaBienvenida.GetResponseStream()))
                {
                    var result = streamReaderCartaBienvenida.ReadToEnd();
                }



                jsonArchivos.Archivos = lstArchivos;
                jsonArchivos.RutaArchivo = polizaOut;
                jsonArchivos.NombreArchivo = "PolizaVI" + emisionPoliza.Poliza.NumPoliza.ToString() + ".pdf";

                json = JsonConvert.SerializeObject(jsonArchivos);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";

                try
                {
                    log.Debug("GenerarPolizaPDF");
                    log.Debug("Url: " + url);
                    log.Debug(String.Format("Inicio Campos num_solicitud:{0} -------------------------------------", num_solicitud));
                    log.Debug(json);
                    log.Debug(String.Format("Fin Campos num_solicitud {0}----------------------------------------", num_solicitud));
                }
                catch (Exception _log)
                {
                    log.Error(_log.Message, _log);
                }

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                }

                ////Deshabilita los campos, para no editar
                //oPdfPolizaJoin.FormFlattening = true;

                //Cierra el PDF
                //oPdfPolizaJoin.Close();
                //oPdfPolizaJoin.Dispose();

                //oPdfPolizaReader.Close();
                //oPdfPolizaReader.Dispose();

                //PdfReader RD = null;

                ////doc = new Document();
                //log.Debug("Creando Archivo: " + sPdfPolizaCompleta);
                //FileStream fs = new FileStream(sPdfPolizaCompleta, FileMode.Append, FileAccess.Write, FileShare.None);

                //PdfCopy copy = new PdfCopy(doc, fs);
                //doc.Open();
                //copy.Open();

                //int nHojas;

                //try
                //{
                //    log.Debug("Inicio de union de Archivos");

                //    foreach (var item in lstArchivos)
                //    {
                //        RD = new PdfReader(item);
                //        nHojas = RD.NumberOfPages;
                //        int Pag = 0;
                //        RD.RemoveFields();
                //        RD.RemoveUnusedObjects();
                //        RD.RemoveAnnotations();

                //        while (Pag < nHojas)
                //        {
                //            Pag += 1;
                //            copy.AddPage(copy.GetImportedPage(RD, Pag));
                //        }

                //        copy.FreeReader(RD);

                //        RD.Close();
                //    }

                //    log.Debug("Fin de union de Archivos");
                //}
                //catch (Exception exx)
                //{
                //    log.Error(exx.Message, exx);
                //    if (RD != null)
                //        RD.Close(); RD.Dispose();

                //    if (doc != null)
                //        doc.Close(); doc.Dispose();

                //    fs.Close();
                //    throw new Exception(exx.Message.ToString());
                //}
                ////copy.SetFullCompression();

                //copy.Close();
                //doc.Close();
                //fs.Close();

                //if (TipoProducto == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                //{
                /*Convirtiendo el PDF Aceptacion en Bytes*/
                log.Debug("Convirtiendo Archivo Carta Bienvenida en bytes: " + PDFAceptacion);

                PDFAceptacion = polizaOut + "CartaBienvenida" + ".pdf";

                FileStream streamAceptacion = File.OpenRead(PDFAceptacion);
                byte[] fileBytesAceptacion = new byte[streamAceptacion.Length];

                streamAceptacion.Read(fileBytesAceptacion, 0, fileBytesAceptacion.Length);
                streamAceptacion.Close();
                streamAceptacion.Dispose();

                lstArchivosByte.Add(fileBytesAceptacion);

                /*Convirtiendo el PDF Emision Poliza en Bytes*/
                log.Debug("Convirtiendo Archivo Póliza en bytes: " + PDFPoliza);

                PDFPoliza = polizaOut + "PolizaVI" + emisionPoliza.Poliza.NumPoliza.ToString() + ".pdf";

                FileStream streamPoliza = File.OpenRead(PDFPoliza);

                byte[] fileBytesPoliza = new byte[streamPoliza.Length];

                streamPoliza.Read(fileBytesPoliza, 0, fileBytesPoliza.Length);
                streamPoliza.Close();
                streamPoliza.Dispose();

                lstArchivosByte.Add(fileBytesPoliza);

                log.Debug("Retorno Correctamente");
                return lstArchivosByte;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
            //finally
            //{
            ////Destruyendo
            //if (oPdfCartaJoin != null)
            //{
            //    oPdfCartaJoin.Close();
            //    oPdfCartaJoin.Dispose();
            //}

            //if (oPdfCartaReader != null)
            //{
            //    oPdfCartaReader.Close();
            //    oPdfCartaReader.Dispose();
            //}

            //if (oPdfPolizaJoin != null)
            //{
            //    oPdfPolizaJoin.Close();
            //    oPdfPolizaJoin.Dispose();
            //}

            //if (oPdfPolizaReader != null)
            //{
            //    oPdfPolizaReader.Close();
            //    oPdfPolizaReader.Dispose();
            //}

            //if (oPdfCondicionGeneralJoin != null)
            //{
            //    oPdfCondicionGeneralJoin.Close();
            //    oPdfCondicionGeneralJoin.Dispose();
            //}

            //if (oPdfCondicionGeneralReader != null)
            //{
            //    oPdfCondicionGeneralReader.Close();
            //    oPdfCondicionGeneralReader.Dispose();
            //}

            //if (oPdfsPdfResumenNewJoin != null)
            //{
            //    oPdfsPdfResumenNewJoin.Close();
            //    oPdfsPdfResumenNewJoin.Dispose();
            //}

            //if (oPdfsPdfResumenNewReader != null)
            //{
            //    oPdfsPdfResumenNewReader.Close();
            //    oPdfsPdfResumenNewReader.Dispose();
            //}

            //if (doc != null)
            //{
            //    doc.Close();
            //    doc.Dispose();
            //}

            ////Eliminando Archivos
            //log.Debug("Eliminando Archivos Creados finally()");
            //if (File.Exists(sPdfAceptacionNew))
            //    File.Delete(sPdfAceptacionNew);

            //if (File.Exists(sPdfCartaNew))
            //    File.Delete(sPdfCartaNew);

            //if (File.Exists(sPdfPolizaNew))
            //    File.Delete(sPdfPolizaNew);

            //if (File.Exists(PDFPolizaCompleta))
            //    File.Delete(PDFPolizaCompleta);

            //if (File.Exists(sPdfCondicionGeneralNew))
            //    File.Delete(sPdfCondicionGeneralNew);

            //if (File.Exists(sPdfResumenNew))
            //    File.Delete(sPdfResumenNew);

            //}

        }

        public CondicionesParticulares CondicionesParticularesRPP(EmisionPoliza emisionPoliza, GrupoFamiliar grupoFamiliar, string sexo, string telefono, string fechafinvigencia, string factorajusterenta, string reembolsogastossepelio, string clausulaperiodogarantizado, string fechaclausulaperiodogarantizado, string pagodoble, string aniospagodoble, string devolucionprimaunica)
        {
            CondicionesParticulares condicionesParticulares;

            condicionesParticulares = new CondicionesParticulares()
            {
                //1
                //N° POLIZA
                NroPoliza = "VI" + emisionPoliza.Poliza.NumPoliza.ToString(),

                //2
                ApellidosNombres = emisionPoliza.SolicitudRPPlus.Beneficiarios[0].ApellidosNombres,
                DocIdentidad = emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Identificacion.GlosaTipo,
                NumeroDocIdentidad = emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Identificacion.Numero.ToString(),
                FechaNacimiento = emisionPoliza.SolicitudRPPlus.Beneficiarios[0].FechaNacimiento.Value.ToString("dd/MM/yyyy"),
                Sexo = sexo,
                Direccion = grupoFamiliar.Direccion.ToUpper(),
                Distrito = grupoFamiliar.Distrito,
                Provincia = grupoFamiliar.Provincia,
                Departamento = grupoFamiliar.Departamento,
                Telefono = telefono,
                CorreoElectronico = grupoFamiliar.CorreoElectronico,

                //3
                PlanSeguro = emisionPoliza.SolicitudRPPlus.TipoPlan.Nombre,
                MonedaPrima = emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Nombre,
                FechaFinVigencia = fechafinvigencia,
                PlazoVigencia = emisionPoliza.SolicitudRPPlus.Temporalidad.Nombre,
                FechaEmision = emisionPoliza.Poliza.FecEmision.ToString("dd/MM/yyyy"),
                FechaInicioVigencia = "00.00 HORAS DEL " + emisionPoliza.Poliza.FecInicioVigencia.ToString("dd/MM/yyyy"),

                //5
                MonedaPagoRenta = emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Nombre,
                FactorAjusteRenta = factorajusterenta,
                FechaInicioPagoRentas = emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy"),
                MontoBaseRentaMensual = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PensionCiaMO.ToString("#,##0.00"),

                //6
                ReembolsoGastosSepelio = reembolsogastossepelio,
                PeriodoGarantizado = clausulaperiodogarantizado,
                FechaClausula = fechaclausulaperiodogarantizado,
                PagoDoble = pagodoble,
                AñosPagoDoble = aniospagodoble,
                DevolucionPrimaUnica = devolucionprimaunica,

                //7
                Banco = emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Banco.Glosa.ToString(),
                NumCuentaAhorros = emisionPoliza.SolicitudRPPlus.Beneficiarios[0].NumeroBanco,

                //8
                PrimaComercial = emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Simbolo.ToString() + " " + emisionPoliza.Poliza.ValPrimaNeta.ToString("#,##0.00"),
                IGV = emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Simbolo.ToString() + " " + emisionPoliza.Poliza.ValIva.ToString("#,##0.00"),
                PrimaComercialIGV = emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Simbolo.ToString() + " " + emisionPoliza.Poliza.ValPrimaBruta.ToString("#,##0.00"),
                TIRGarantizada = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].TasaVenta.ToString("#,##0.00") + "%",
                ComisionPromotor = emisionPoliza.PjeComision.ToString("#,##0.00") + "%"

            };

            return condicionesParticulares;
        }

        public Carta cartasRPP(EmisionPoliza emisionPoliza, GrupoFamiliar grupoFamiliar)
        {
            Carta carta;

            carta = new Carta()
            {
                ApellidosNombres = emisionPoliza.SolicitudRPPlus.Beneficiarios[0].ApellidosNombres,
                ApellidoPaterno = emisionPoliza.SolicitudRPPlus.Beneficiarios[0].ApellidoPaterno,
                Fecha = DateTime.Today.ToString("dd 'de' MMMM 'de' yyyy", CultureInfo.CreateSpecificCulture("es-PE")),
                Direccion = grupoFamiliar.Direccion.ToUpper(),
                Distrito = grupoFamiliar.Distrito + " - " + grupoFamiliar.Provincia + " - " + grupoFamiliar.Departamento,
                NroPoliza = emisionPoliza.Poliza.NumPoliza.ToString()
            };

            return carta;
        }

        public byte[] CompressPdf(byte[] src)
        {
            PdfReader reader = new PdfReader(src);
            using (MemoryStream ms = new MemoryStream())
            {
                using (PdfStamper stamper =
                    new PdfStamper(reader, ms, PdfWriter.VERSION_1_5))
                {

                    PdfWriter writer = stamper.Writer;
                    writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_5);
                    writer.CompressionLevel = PdfStream.BEST_COMPRESSION;
                    reader.RemoveFields();
                    reader.RemoveUnusedObjects();
                    stamper.Reader.RemoveUnusedObjects();

                    stamper.SetFullCompression();
                    stamper.Writer.SetFullCompression();
                    stamper.Close();

                    ////stamper.Writer.CompressionLevel = 100;
                    ////int total = reader.NumberOfPages + 1;
                    ////reader.RemoveFields();
                    ////reader.RemoveUnusedObjects();

                    ////for (int i = 1; i < total; i++)
                    ////{
                    ////    reader.SetPageContent(i, reader.GetPageContent(i),PdfStream.BEST_COMPRESSION,true);


                    ////}
                    ////stamper.Writer.CompressionLevel = 100;
                    ////stamper.SetFullCompression();



                    ////stamper.Reader.RemoveUnusedObjects();


                    ////stamper.Close();
                }
                return ms.ToArray();
            }
        }

        //<INI.GTI_7012_11>
        public List<Parametro> ObtenerTipoCtaBancos(string banco, string id)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Parametro> parametros = generalesServicio.ObtenerTipoCtaBancos(banco, id);
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<INI.GTI_7012_11>

        public JsonTokenPlaft ObtenerTokenPlaft()
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.ObtenerTokenPlaft PLAFT");
                JsonTokenPlaft oToken = null;

                string url = ConfigurationManager.AppSettings["url_token"].ToString();
                string grant_type = ConfigurationManager.AppSettings["grant_type"].ToString();
                string client_id = ConfigurationManager.AppSettings["client_id"].ToString();
                string client_secret = ConfigurationManager.AppSettings["client_secret"].ToString();

                using (WebClient wc = new WebClient())
                {
                    wc.QueryString.Add("grant_type", grant_type);
                    wc.QueryString.Add("client_id", client_id);
                    wc.QueryString.Add("client_secret", client_secret);

                    try
                    {
                        var data = wc.UploadValues(url, "POST", wc.QueryString);

                        string responseString = UnicodeEncoding.UTF8.GetString(data);
                        oToken = new JavaScriptSerializer().Deserialize<JsonTokenPlaft>(responseString);
                    }
                    catch (Exception _plaft)
                    {
                        log.Error(_plaft.Message, _plaft);
                        throw new Exception("Servicio de token no disponible");
                    }

                }

                log.Debug("Fin ServicioCWRV.ObtenerTokenPlaft PLAFT");

                return oToken;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public JsonEmail ObtenerEmailPlaft()
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.ObtenerEmailPlaft PLAFT");

                JsonEmail oEmail = null;
                JsonTokenPlaft JsonTokenPlaft = ObtenerTokenPlaft();

                if (JsonTokenPlaft == null)
                {
                    throw new Exception("Token no generado");
                }

                if (JsonTokenPlaft._meta.status == "ERROR")
                {
                    throw new Exception(JsonTokenPlaft._meta.status);
                }

                string url = ConfigurationManager.AppSettings["url_email"].ToString();
                log.Debug("Url: " + url);
                using (WebClient wc = new WebClient())
                {
                    wc.Headers["Authorization"] = JsonTokenPlaft.records.token_type + " " + JsonTokenPlaft.records.access_token;
                    try
                    {
                        var data = wc.UploadValues(url, "POST", wc.QueryString);
                        string responseString = UnicodeEncoding.UTF8.GetString(data);
                        log.Debug("Json Devuelto ObtenerEmailPlaft: " + responseString);
                        oEmail = new JavaScriptSerializer().Deserialize<JsonEmail>(responseString);
                    }
                    catch (Exception _plaft)
                    {
                        log.Error(_plaft.Message, _plaft);
                        throw new Exception("Servicio de email no disponible");
                    }

                }

                log.Debug("Fin ServicioCWRV.ObtenerEmailPlaft PLAFT");

                return oEmail;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public JsonCoincidencia ObtenerCoincidencia(GrupoFamiliar grupoFamiliar)
        {
            try
            {
                log.Debug("Inicio ServicioCWRV.ObtenerCoincidencia PLAFT");

                JsonTokenPlaft JsonTokenPlaft = ObtenerTokenPlaft();
                JsonCoincidencia oCoincidencia = null;
                if (JsonTokenPlaft == null)
                {
                    throw new Exception("Token de PLAFT no generado.");
                }

                if (JsonTokenPlaft._meta.status == "ERROR")
                {
                    throw new Exception(JsonTokenPlaft._meta.status);
                }

                string url = ConfigurationManager.AppSettings["url_coincidencia"].ToString();

                using (WebClient wc = new WebClient())
                {
                    wc.Headers["Authorization"] = JsonTokenPlaft.records.token_type + " " + JsonTokenPlaft.records.access_token;

                    string client_id2 = ConfigurationManager.AppSettings["client_id2"].ToString();
                    //string nroDocumento = grupoFamiliar.Identificacion.Numero.Value.ToString();
                    string nroDocumento = grupoFamiliar.Identificacion.Numero.ToString();

                    if (grupoFamiliar.Identificacion.IdTipo == "D")
                        //nroDocumento = grupoFamiliar.Identificacion.Numero.Value.ToString("00000000");
                        nroDocumento = grupoFamiliar.Identificacion.Numero.ToString();
                    else if (grupoFamiliar.Identificacion.IdTipo == "E")
                        //nroDocumento = grupoFamiliar.Identificacion.Numero.Value.ToString("000000000");
                        nroDocumento = grupoFamiliar.Identificacion.Numero.ToString();

                    wc.QueryString.Add("client_id", client_id2);
                    wc.QueryString.Add("tipo_documento", grupoFamiliar.Identificacion.GlosaTipo);
                    wc.QueryString.Add("documento", nroDocumento);
                    wc.QueryString.Add("apellido_paterno", grupoFamiliar.ApellidoPaterno);
                    wc.QueryString.Add("apellido_materno", grupoFamiliar.ApellidoMaterno);
                    wc.QueryString.Add("nombre1", grupoFamiliar.Nombre);
                    wc.QueryString.Add("nombre2", "");
                    wc.QueryString.Add("nombre3", "");
                    wc.QueryString.Add("razon_social", "");

                    try
                    {
                        log.Debug("Url: " + url);
                        log.Debug(string.Format("Inicio Campos  ObtenerCoincidencia"));
                        log.Debug(string.Format("client_id:{0}", client_id2));
                        log.Debug(string.Format("tipo_documento:{0}", grupoFamiliar.Identificacion.GlosaTipo));
                        log.Debug(string.Format("documento:{0}", nroDocumento));
                        log.Debug(string.Format("apellido_paterno:{0}", grupoFamiliar.ApellidoPaterno));
                        log.Debug(string.Format("apellido_materno:{0}", grupoFamiliar.ApellidoMaterno));
                        log.Debug(string.Format("nombre1:{0}", grupoFamiliar.Nombre));
                        log.Debug(string.Format("nombre2:{0}", ""));
                        log.Debug(string.Format("nombre3:{0}", ""));
                        log.Debug(string.Format("razon_social:{0}", ""));
                        log.Debug(string.Format("Fin Campos  ObtenerCoincidencia"));
                    }
                    catch (Exception _log)
                    {
                        log.Error(_log.Message, _log);
                    }

                    try
                    {
                        var data = wc.UploadValues(url, "POST", wc.QueryString);

                        string responseString = UnicodeEncoding.UTF8.GetString(data);
                        log.Debug("Json Devuelto ObtenerCoincidencia: " + responseString);
                        oCoincidencia = new JavaScriptSerializer().Deserialize<JsonCoincidencia>(responseString);
                    }
                    catch (Exception _plaft)
                    {
                        log.Error(_plaft.Message, _plaft);
                        throw new Exception("Servicio de coincidencia no disponible");
                    }

                }

                log.Debug("Fin ServicioCWRV.ObtenerCoincidencia PLAFT");

                return oCoincidencia;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public JsonCoincidenciaLN ObtenerCoincidenciaLN(GrupoFamiliar grupoFamiliar)
        {
            try
            {
                log.Debug("Inicio ServicioCWRV.ObtenerCoincidenciaLN PLAFT");

                JsonTokenPlaft JsonTokenPlaft = ObtenerTokenPlaft();
                JsonCoincidenciaLN oCoincidencia = null;
                if (JsonTokenPlaft == null)
                {
                    throw new Exception("Token de PLAFT no generado.");
                }

                if (JsonTokenPlaft._meta.status == "ERROR")
                {
                    throw new Exception(JsonTokenPlaft._meta.status);
                }


                string url = ConfigurationManager.AppSettings["url_coincidencia_ln"].ToString();
                using (WebClient wc = new WebClient())
                {
                    wc.Headers["Authorization"] = JsonTokenPlaft.records.token_type + " " + JsonTokenPlaft.records.access_token;

                    string client_id2 = ConfigurationManager.AppSettings["client_id2"].ToString();
                    //string nroDocumento = grupoFamiliar.Identificacion.Numero.Value.ToString();
                    string nroDocumento = grupoFamiliar.Identificacion.Numero.ToString();

                    if (grupoFamiliar.Identificacion.IdTipo == "D")
                        //nroDocumento = grupoFamiliar.Identificacion.Numero.Value.ToString("00000000");
                        nroDocumento = grupoFamiliar.Identificacion.Numero.ToString();
                    else if (grupoFamiliar.Identificacion.IdTipo == "E")
                        //nroDocumento = grupoFamiliar.Identificacion.Numero.Value.ToString("000000000");
                        nroDocumento = grupoFamiliar.Identificacion.Numero.ToString();

                    wc.QueryString.Add("client_id", client_id2);
                    wc.QueryString.Add("tipo_documento", grupoFamiliar.Identificacion.GlosaTipo);
                    wc.QueryString.Add("documento", nroDocumento);
                    wc.QueryString.Add("apellido_paterno", grupoFamiliar.ApellidoPaterno);
                    wc.QueryString.Add("apellido_materno", grupoFamiliar.ApellidoMaterno);
                    wc.QueryString.Add("nombre1", grupoFamiliar.Nombre);
                    wc.QueryString.Add("nombre2", "");
                    wc.QueryString.Add("nombre3", "");
                    wc.QueryString.Add("razon_social", "");

                    try
                    {
                        log.Debug("Url: " + url);
                        log.Debug(string.Format("Inicio Campos  ObtenerCoincidenciaLN"));
                        log.Debug(string.Format("client_id:{0}", client_id2));
                        log.Debug(string.Format("tipo_documento:{0}", grupoFamiliar.Identificacion.GlosaTipo));
                        log.Debug(string.Format("documento:{0}", nroDocumento));
                        log.Debug(string.Format("apellido_paterno:{0}", grupoFamiliar.ApellidoPaterno));
                        log.Debug(string.Format("apellido_materno:{0}", grupoFamiliar.ApellidoMaterno));
                        log.Debug(string.Format("nombre1:{0}", grupoFamiliar.Nombre));
                        log.Debug(string.Format("nombre2:{0}", ""));
                        log.Debug(string.Format("nombre3:{0}", ""));
                        log.Debug(string.Format("razon_social:{0}", ""));
                        log.Debug(string.Format("Fin Campos  ObtenerCoincidenciaLN"));
                    }
                    catch (Exception _log)
                    {
                        log.Error(_log.Message, _log);
                    }

                    try
                    {
                        var data = wc.UploadValues(url, "POST", wc.QueryString);

                        string responseString = UnicodeEncoding.UTF8.GetString(data);
                        log.Debug("Json Devuelto ObtenerCoincidenciaLN: " + responseString);
                        oCoincidencia = new JavaScriptSerializer().Deserialize<JsonCoincidenciaLN>(responseString);
                    }
                    catch (Exception _plaft)
                    {
                        log.Error(_plaft.Message, _plaft);
                        throw new Exception("Servicio de coincidencia LN no disponible");
                    }

                }

                log.Debug("Fin ServicioCWRV.ObtenerCoincidenciaLN PLAFT");

                return oCoincidencia;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public JsonPropuesta ObtenerCalificacionPlaft(Propuesta propuesta, JsonTokenPlaft JsonTokenPlaft)
        {
            JsonPropuesta oPropuesta = null;
            try
            {
                log.Debug("Inicio ServicioCWRV.ObtenerCalificacionPlaft PLAFT");

                string url = ConfigurationManager.AppSettings["url_propuesta"].ToString();
                using (WebClient wc = new WebClient())
                {
                    wc.Headers["Authorization"] = JsonTokenPlaft.records.token_type + " " + JsonTokenPlaft.records.access_token;

                    string client_id2 = ConfigurationManager.AppSettings["client_id2"].ToString();

                    wc.QueryString.Add("client_id", client_id2);
                    wc.QueryString.Add("propuesta", propuesta.propuesta);
                    wc.QueryString.Add("prima_anualizada", propuesta.prima_anualizada.ToString());
                    wc.QueryString.Add("producto", propuesta.producto);
                    wc.QueryString.Add("moneda", propuesta.moneda);
                    wc.QueryString.Add("contratante_tipo_documento", propuesta.contratante_tipo_documento);
                    wc.QueryString.Add("contratante_documento", propuesta.contratante_documento);
                    wc.QueryString.Add("contratante_actividad_economica", propuesta.contratante_actividad_economica);
                    wc.QueryString.Add("contratante_profesion", propuesta.contratante_profesion);
                    wc.QueryString.Add("contratante_sujeto_obligado", propuesta.contratante_sujeto_obligado);
                    wc.QueryString.Add("contratante_residencia", propuesta.contratante_residencia);
                    wc.QueryString.Add("contratante_nacionalidad", propuesta.contratante_nacionalidad);
                    wc.QueryString.Add("contratante_pep", propuesta.contratante_pep);
                    wc.QueryString.Add("contratante_nombre1", propuesta.contratante_nombre1);
                    wc.QueryString.Add("contratante_nombre2", propuesta.contratante_nombre2);
                    wc.QueryString.Add("contratante_nombre3", propuesta.contratante_nombre3);
                    wc.QueryString.Add("contratante_apellido_paterno", propuesta.contratante_apellido_paterno);
                    wc.QueryString.Add("contratante_apellido_materno", propuesta.contratante_apellido_materno);
                    wc.QueryString.Add("contratante_razon_social", propuesta.contratante_razon_social);
                    wc.QueryString.Add("asegurado_tipo_documento", propuesta.asegurado_tipo_documento);
                    wc.QueryString.Add("asegurado_documento", propuesta.asegurado_documento);
                    wc.QueryString.Add("asegurado_actividad_economica", propuesta.asegurado_actividad_economica);
                    wc.QueryString.Add("asegurado_profesion", propuesta.asegurado_profesion);
                    wc.QueryString.Add("asegurado_sujeto_obligado", propuesta.asegurado_sujeto_obligado);
                    wc.QueryString.Add("asegurado_residencia", propuesta.asegurado_residencia);
                    wc.QueryString.Add("asegurado_nacionalidad", propuesta.asegurado_nacionalidad);
                    wc.QueryString.Add("asegurado_pep", propuesta.asegurado_pep);
                    wc.QueryString.Add("asegurado_nombre1", propuesta.asegurado_nombre1);
                    wc.QueryString.Add("asegurado_nombre2", propuesta.asegurado_nombre2);
                    wc.QueryString.Add("asegurado_nombre3", propuesta.asegurado_nombre3);
                    wc.QueryString.Add("asegurado_apellido_paterno", propuesta.asegurado_apellido_paterno);
                    wc.QueryString.Add("asegurado_apellido_materno", propuesta.asegurado_apellido_materno);
                    wc.QueryString.Add("asegurado_razon_social", propuesta.asegurado_razon_social);
                    wc.QueryString.Add("canal_distribucion", propuesta.CodCanalDistribucion);

                    try
                    {
                        log.Debug("Url: " + url);
                        log.Debug(String.Format("Inicio Campos num_solicitud:{0} -------------------------------------", propuesta.propuesta));
                        log.Debug(string.Format("client_id:{0}", client_id2));
                        log.Debug(string.Format("propuesta:{0}", propuesta.propuesta));
                        log.Debug(string.Format("prima_anualizada:{0}", propuesta.prima_anualizada.ToString()));
                        log.Debug(string.Format("producto:{0}", propuesta.producto));
                        log.Debug(string.Format("moneda:{0}", propuesta.moneda));
                        log.Debug(string.Format("contratante_tipo_documento:{0}", propuesta.contratante_tipo_documento));
                        log.Debug(string.Format("contratante_documento:{0}", propuesta.contratante_documento));
                        log.Debug(string.Format("contratante_actividad_economica:{0}", propuesta.contratante_actividad_economica));
                        log.Debug(string.Format("contratante_profesion:{0}", propuesta.contratante_profesion));
                        log.Debug(string.Format("contratante_sujeto_obligado:{0}", propuesta.contratante_sujeto_obligado));
                        log.Debug(string.Format("contratante_residencia:{0}", propuesta.contratante_residencia));
                        log.Debug(string.Format("contratante_nacionalidad:{0}", propuesta.contratante_nacionalidad));
                        log.Debug(string.Format("contratante_pep:{0}", propuesta.contratante_pep));
                        log.Debug(string.Format("contratante_nombre1:{0}", propuesta.contratante_nombre1));
                        log.Debug(string.Format("contratante_nombre2:{0}", propuesta.contratante_nombre2));
                        log.Debug(string.Format("contratante_nombre3:{0}", propuesta.contratante_nombre3));
                        log.Debug(string.Format("contratante_apellido_paterno:{0}", propuesta.contratante_apellido_paterno));
                        log.Debug(string.Format("contratante_apellido_materno:{0}", propuesta.contratante_apellido_materno));
                        log.Debug(string.Format("contratante_razon_social:{0}", propuesta.contratante_razon_social));
                        log.Debug(string.Format("asegurado_tipo_documento:{0}", propuesta.asegurado_tipo_documento));
                        log.Debug(string.Format("asegurado_documento:{0}", propuesta.asegurado_documento));
                        log.Debug(string.Format("asegurado_actividad_economica:{0}", propuesta.asegurado_actividad_economica));
                        log.Debug(string.Format("asegurado_profesion:{0}", propuesta.asegurado_profesion));
                        log.Debug(string.Format("asegurado_sujeto_obligado:{0}", propuesta.asegurado_sujeto_obligado));
                        log.Debug(string.Format("asegurado_residencia:{0}", propuesta.asegurado_residencia));
                        log.Debug(string.Format("asegurado_nacionalidad:{0}", propuesta.asegurado_nacionalidad));
                        log.Debug(string.Format("asegurado_pep:{0}", propuesta.asegurado_pep));
                        log.Debug(string.Format("asegurado_nombre1:{0}", propuesta.asegurado_nombre1));
                        log.Debug(string.Format("asegurado_nombre2:{0}", propuesta.asegurado_nombre2));
                        log.Debug(string.Format("asegurado_nombre3:{0}", propuesta.asegurado_nombre3));
                        log.Debug(string.Format("asegurado_apellido_paterno:{0}", propuesta.asegurado_apellido_paterno));
                        log.Debug(string.Format("asegurado_apellido_materno:{0}", propuesta.asegurado_apellido_materno));
                        log.Debug(string.Format("asegurado_razon_social:{0}", propuesta.asegurado_razon_social));
                        log.Debug(string.Format("canal_distribucion:{0}", propuesta.CodCanalDistribucion));
                        log.Debug(String.Format("Fin Campos num_solicitud {0}----------------------------------------", propuesta.propuesta));
                    }
                    catch (Exception _log)
                    {
                        log.Error(_log.Message, _log);
                    }

                    try
                    {
                        log.Debug(string.Format("Iniciando Servicio Plaft(Calificacion) num_solicitud:{0}", propuesta.propuesta));
                        var data = wc.UploadValues(url, "POST", wc.QueryString);
                        string responseString = UnicodeEncoding.UTF8.GetString(data);
                        oPropuesta = new JavaScriptSerializer().Deserialize<JsonPropuesta>(responseString);
                        log.Debug(String.Format("Finalizando Servicio Plaft(Calificacion) num_solicitud: {0}", propuesta.propuesta));
                        log.Debug(String.Format("Propuesta {0} obtuvo el codigo: {1}, y la descripción: {2} ", propuesta.propuesta, oPropuesta.records.codigo, oPropuesta.records.descripcion));
                    }
                    catch (Exception _plaft)
                    {
                        log.Error(_plaft.Message, _plaft);

                        string mensajeError = "Se ha generando un error al enviar a evaluación de plaft";
                        mensajeError += "<br><strong>Error generado:</strong><br>";
                        mensajeError += _plaft.Message;

                        mensajeError += "<br><strong>Url:</strong>" + url + " <br>";
                        mensajeError += "<br><strong>Metodo:</strong>POST<br>";
                        mensajeError += "<br><strong>Valores Enviados:</strong><br>";
                        mensajeError += string.Format("client_id:{0}", client_id2);
                        mensajeError += "<br>" + string.Format("propuesta:{0}", propuesta.propuesta);
                        mensajeError += "<br>" + string.Format("prima_anualizada:{0}", propuesta.prima_anualizada.ToString());
                        mensajeError += "<br>" + string.Format("producto:{0}", propuesta.producto);
                        mensajeError += "<br>" + string.Format("moneda:{0}", propuesta.moneda);
                        mensajeError += "<br>" + string.Format("contratante_tipo_documento:{0}", propuesta.contratante_tipo_documento);
                        mensajeError += "<br>" + string.Format("contratante_documento:{0}", propuesta.contratante_documento);
                        mensajeError += "<br>" + string.Format("contratante_actividad_economica:{0}", propuesta.contratante_actividad_economica);
                        mensajeError += "<br>" + string.Format("contratante_profesion:{0}", propuesta.contratante_profesion);
                        mensajeError += "<br>" + string.Format("contratante_sujeto_obligado:{0}", propuesta.contratante_sujeto_obligado);
                        mensajeError += "<br>" + string.Format("contratante_residencia:{0}", propuesta.contratante_residencia);
                        mensajeError += "<br>" + string.Format("contratante_nacionalidad:{0}", propuesta.contratante_nacionalidad);
                        mensajeError += "<br>" + string.Format("contratante_pep:{0}", propuesta.contratante_pep);
                        mensajeError += "<br>" + string.Format("contratante_nombre1:{0}", propuesta.contratante_nombre1);
                        mensajeError += "<br>" + string.Format("contratante_nombre2:{0}", propuesta.contratante_nombre2);
                        mensajeError += "<br>" + string.Format("contratante_nombre3:{0}", propuesta.contratante_nombre3);
                        mensajeError += "<br>" + string.Format("contratante_apellido_paterno:{0}", propuesta.contratante_apellido_paterno);
                        mensajeError += "<br>" + string.Format("contratante_apellido_materno:{0}", propuesta.contratante_apellido_materno);
                        mensajeError += "<br>" + string.Format("contratante_razon_social:{0}", propuesta.contratante_razon_social);
                        mensajeError += "<br>" + string.Format("asegurado_tipo_documento:{0}", propuesta.asegurado_tipo_documento);
                        mensajeError += "<br>" + string.Format("asegurado_documento:{0}", propuesta.asegurado_documento);
                        mensajeError += "<br>" + string.Format("asegurado_actividad_economica:{0}", propuesta.asegurado_actividad_economica);
                        mensajeError += "<br>" + string.Format("asegurado_profesion:{0}", propuesta.asegurado_profesion);
                        mensajeError += "<br>" + string.Format("asegurado_sujeto_obligado:{0}", propuesta.asegurado_sujeto_obligado);
                        mensajeError += "<br>" + string.Format("asegurado_residencia:{0}", propuesta.asegurado_residencia);
                        mensajeError += "<br>" + string.Format("asegurado_nacionalidad:{0}", propuesta.asegurado_nacionalidad);
                        mensajeError += "<br>" + string.Format("asegurado_pep:{0}", propuesta.asegurado_pep);
                        mensajeError += "<br>" + string.Format("asegurado_nombre1:{0}", propuesta.asegurado_nombre1);
                        mensajeError += "<br>" + string.Format("asegurado_nombre2:{0}", propuesta.asegurado_nombre2);
                        mensajeError += "<br>" + string.Format("asegurado_nombre3:{0}", propuesta.asegurado_nombre3);
                        mensajeError += "<br>" + string.Format("asegurado_apellido_paterno:{0}", propuesta.asegurado_apellido_paterno);
                        mensajeError += "<br>" + string.Format("asegurado_apellido_materno:{0}", propuesta.asegurado_apellido_materno);
                        mensajeError += "<br>" + string.Format("asegurado_razon_social:{0}", propuesta.asegurado_razon_social);
                        mensajeError += "<br>" + string.Format("canal_distribucion:{0}", propuesta.CodCanalDistribucion);

                        var tareaParalela = new System.Threading.Tasks.Task(() =>
                        {
                            Notificacion notificacion = new Notificacion();
                            notificacion.p_destinatario = ConfigurationManager.AppSettings["destinatario_error"].ToString();

                            notificacion.p_remitente = "cwrv.error@interseguro.com.pe";
                            notificacion.p_asunto = "Error envío a evaluación PLAFT - Solicitud:" + propuesta.propuesta;
                            notificacion.p_mensaje = mensajeError;
                            notificacion.p_displayName = "Error envío a evaluación PLAFT";
                            notificacion.p_ruta_archivo_adjunto = "";

                            log.Info(String.Format("Inicio de Envio Notificacion Correo:{0}", notificacion.p_destinatario));
                            ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                            cotizadorServicio.EnviarNotificacion(ConfigurationManager.AppSettings["url_envio_correo"].ToString(), notificacion);

                            log.Info(String.Format("Fin de Envio Notificacion Correo:{0}", notificacion.p_destinatario));

                        });
                        tareaParalela.Start();

                        throw new Exception("Servicio de evaluación no disponible");
                    }

                }

                RegistrarLog(new LogBD
                {
                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                    NombreTerminal = "Interseguro.CWRV.ServiciosDistribuidos.ServicioCWRV.ObtenerCalificacion",
                    IP = "",
                    NombreUsuario = propuesta.usuario,
                    IdTipoEvento = Enums.EventoLog.EnviarevaluacionPlaft.StringValue(),
                    Detalle = String.Format("Propuesta {0} obtuvo el codigo: {1}, y la descripción: {2} ", propuesta.propuesta, oPropuesta.records.codigo, oPropuesta.records.descripcion)
                });

                log.Debug("Fin ServicioCWRV.ObtenerCalificacionPlaft PLAFT");

                return oPropuesta;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }

        }

        public JsonPropuesta ObtenerCalificacion(Propuesta propuesta, string urlCWRV)
        {
            try
            {
                log.Info("ObtenerCalificacion INI");

                log.Debug("Inicio ServicioCWRV.ObtenerCalificacion PLAFT");

                JsonPropuesta oPropuesta = new JsonPropuesta();

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                ServicioAzmanClient servicioAzman = new ServicioAzmanClient("epAzman");

                BEUsuario datosUsuarioCorreo = null;

                ConfiguracionCorreo configuracionCorreo = ObtenerConfiguracionCorreo(1, DateTime.Today);

                JsonTokenPlaft JsonTokenPlaft = null;

                string usuarios_correo = string.Empty;
                string[] lista_usuarios_correo = new string[0];
                bool tag_reenvio = false;

                //AGENTE Y SUPERVISOR//Descomentar
                List<string> lstAgentes = new List<string>();
                foreach (var agente in propuesta.Agentes)
                {
                    configuracionCorreo.gls_destinatario += "," + agente.Usuario + ",";
                    lstAgentes.Add(agente.Usuario);
                }

                if (propuesta.CodigoEstado == "1")
                {
                    configuracionCorreo.gls_destinatario += "," + ConfigurationManager.AppSettings["destinatario_envio_evaluacion"];//Operaciones
                    JsonTokenPlaft = ObtenerTokenPlaft();
                    oPropuesta = ObtenerCalificacionPlaft(propuesta, JsonTokenPlaft);
                    usuarios_correo = configuracionCorreo.gls_destinatario;

                    lista_usuarios_correo = usuarios_correo.Split(',');
                }

                if (propuesta.CodigoEstado == "5")
                {
                    configuracionCorreo.gls_destinatario += "," + ConfigurationManager.AppSettings["destinatario_envio_evaluacion"];//Operaciones
                    usuarios_correo = configuracionCorreo.gls_destinatario;
                    oPropuesta = new JsonPropuesta();
                    oPropuesta.records = new records_propuesta();
                    oPropuesta.records.codigo = 1;
                    lista_usuarios_correo = usuarios_correo.Split(',');
                    tag_reenvio = true;
                }

                if (propuesta.CodigoEstadoPlaft == "1")
                {
                    usuarios_correo = configuracionCorreo.gls_destinatario;
                    lista_usuarios_correo = usuarios_correo.Split(',');

                    JsonTokenPlaft = ObtenerTokenPlaft();
                    oPropuesta = ObtenerCalificacionPlaft(propuesta, JsonTokenPlaft);

                    tag_reenvio = true;
                }

                ////PLAFT
                if (oPropuesta.records.codigo != -1)
                {
                    var respuestaActualizacionSolicitud = new Respuesta();
                    if (oPropuesta.records.codigo == 0)
                    {
                        //Actualizar cod_estado_plaft = enviando a evaluación, cod_estado_rpp = enviando a evaluación
                        respuestaActualizacionSolicitud = cotizadorServicio.ActualizacionSolicitudPlusPlaft(propuesta.propuesta, Enums.TipoFlujoEvaluacion.Evaluacion.StringValue(), 4, "", propuesta.ArchivosExistentes, propuesta.usuario);
                    }
                    else
                    {
                        //Actualizar cod_estado_rpp = enviando a evaluación
                        respuestaActualizacionSolicitud = cotizadorServicio.ActualizacionSolicitudPlusPlaft(propuesta.propuesta, Enums.TipoFlujoEvaluacion.EvaluacionOperaciones.StringValue(), 4, "", propuesta.ArchivosExistentes, propuesta.usuario);
                    }

                    if (respuestaActualizacionSolicitud.Estado != Constante.COD_OK)
                    {
                        log.Error(String.Format("Se ha producido el siguiente error: [{0}]", respuestaActualizacionSolicitud.Mensaje));
                        throw new Exception(respuestaActualizacionSolicitud.Estado);
                    }

                    string arc_documento_correo = configuracionCorreo.arc_documento_correo;

                    configuracionCorreo.gls_asunto = configuracionCorreo.gls_asunto.Replace("{num_solicitud}", propuesta.propuesta);

                    if (tag_reenvio)
                    {
                        configuracionCorreo.gls_asunto = configuracionCorreo.gls_asunto.Replace("Envío", "Re-Envío");
                    }

                    if (configuracionCorreo.enviar_correo == "S")
                    {
                        foreach (string usuario_correo in lista_usuarios_correo)
                        {
                            if (usuario_correo.Trim() != "")
                            {
                                Notificacion notificacion = new Notificacion();
                                datosUsuarioCorreo = new BEUsuario();

                                log.Info(String.Format("Inicio de Armado de Correo:{0}", usuario_correo));

                                if (!usuario_correo.Trim().Contains('@'))
                                {
                                    datosUsuarioCorreo = servicioAzman.ObtenerDatosUsuarioSinClave(
                                    ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                    ConfigurationManager.AppSettings["DominioRed"],
                                    usuario_correo.Trim());
                                }
                                else
                                {
                                    log.Info("Usuarios con @: " + usuario_correo.Trim());
                                    string usuarios_plaft = usuario_correo.Trim();
                                    string[] lista_usuarios_plaft = usuarios_plaft.Split(',');

                                    if (lista_usuarios_plaft.Length > 1)
                                    {
                                        datosUsuarioCorreo.NombreCompleto = lista_usuarios_plaft[0];
                                        datosUsuarioCorreo.Correo = lista_usuarios_plaft[1];
                                    }
                                    else
                                    {
                                        datosUsuarioCorreo.NombreCompleto = "";
                                        datosUsuarioCorreo.Correo = lista_usuarios_plaft[0];
                                    }
                                }

                                configuracionCorreo.arc_documento_correo = arc_documento_correo;

                                configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{nombre}", datosUsuarioCorreo.NombreCompleto);
                                configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{num_solicitud}", propuesta.propuesta);

                                if (datosUsuarioCorreo.Matricula != null)
                                {
                                    if (lstAgentes.Contains(datosUsuarioCorreo.Matricula))
                                    {
                                        configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("<a href=\"{url}\">Consulte las propuestas en evaluación haciendo clic aquí</a>", "");
                                    }
                                    else
                                    {
                                        configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{url}", urlCWRV);
                                    }
                                }
                                else
                                {
                                    configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{url}", urlCWRV);
                                }

                                notificacion.p_destinatario = datosUsuarioCorreo.Correo;

                                notificacion.p_remitente = configuracionCorreo.gls_remitente;
                                notificacion.p_asunto = configuracionCorreo.gls_asunto;
                                notificacion.p_mensaje = configuracionCorreo.arc_documento_correo;
                                notificacion.p_displayName = configuracionCorreo.gls_display_name;
                                notificacion.p_ruta_archivo_adjunto = "";

                                log.Info(String.Format("Fin de Armado de Correo:{0}", usuario_correo));

                                Respuesta respuesta = new Respuesta();

                                var tareaParalela = new System.Threading.Tasks.Task(() =>
                                {
                                    log.Info(String.Format("Inicio de Envio Notificacion Correo:{0}", notificacion.p_destinatario));
                                    respuesta = cotizadorServicio.EnviarNotificacion(configuracionCorreo.gls_ruta_servicio, notificacion);
                                    if (respuesta.Estado != Constante.COD_OK)
                                    {
                                        log.Error(respuesta.Mensaje);
                                    }
                                    log.Info(String.Format("Fin de Envio Notificacion Correo:{0}", notificacion.p_destinatario));

                                });
                                tareaParalela.Start();

                            }
                        }
                    }

                }

                log.Debug("Fin ServicioCWRV.ObtenerCalificacion PLAFT");

                return oPropuesta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public List<Parametro> ObtenerTipoIdentificacion(string cod_tipo_identificacion, string gls_tipo_identificacion, string gls_corta_identificacion)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Parametro> parametros = generalesServicio.ObtenerTipoIdentificacion(cod_tipo_identificacion, gls_tipo_identificacion, gls_corta_identificacion);
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta ActualizarSolicitudOperaciones(string num_solicitud, int cod_estado_rpp, string gls_observacion, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            JsonRechazo respuestaPlaft = new JsonRechazo();

            try
            {

                log.Debug("Inicio ServicioCWRV.ActualizarSolicitudOperaciones");

                string tipoFlujo = "0";
                string mensajeError = "";
                string mensajeOk = "";

                if (cod_estado_rpp == 5)//Observado
                {
                    tipoFlujo = Enums.TipoFlujoEvaluacion.ObservadoOperacion.StringValue();
                    mensajeOk = "La solicitud fue observada correctamente";
                }

                if (cod_estado_rpp == 6)//Aprobado
                {
                    tipoFlujo = Enums.TipoFlujoEvaluacion.AprobadoOperacion.StringValue();
                    mensajeOk = "La solicitud fue aprobada correctamente";
                }

                if (cod_estado_rpp == 2)//Cerrada
                {
                    tipoFlujo = Enums.TipoFlujoEvaluacion.Cerrado.StringValue();
                    mensajeOk = "La solicitud fue cerrada correctamente";
                }

                if (cod_estado_rpp == 7)//Rechazado
                {
                    tipoFlujo = Enums.TipoFlujoEvaluacion.RechazadoOperaciones.StringValue();
                    mensajeOk = "La solicitud fue rechazada correctamente";
                }

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();

                SolicitudRPPlus validarSolicitudRPPlus = cotizadorServicio.ObtenerEstadoSolicitudRPPlus(num_solicitud);

                if (validarSolicitudRPPlus.CodigoEstado == 5)//Observado
                    mensajeError = "La solicitud ya se encuentra observada";

                if (validarSolicitudRPPlus.CodigoEstado == 6)//Aprobado
                    mensajeError = "La solicitud ya se encuentra aprobada";

                if (validarSolicitudRPPlus.CodigoEstado == 2)//Cerrada
                    mensajeError = "La solicitud ya se encuentra cerrada";

                if (validarSolicitudRPPlus.CodigoEstado == 7)//Rechazado
                    mensajeError = "La solicitud ya se encuentra rechazada";

                if (validarSolicitudRPPlus.CodigoEstado != 4)
                {
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Mensaje = mensajeError;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    return respuesta;
                }

                respuesta = cotizadorServicio.ActualizacionSolicitudPlusPlaft(num_solicitud, tipoFlujo, cod_estado_rpp, gls_observacion, "", usuario);

                respuesta.Mensaje = mensajeOk;
                log.Info(string.Format("Actualización de Solicitud:{0}, estado:{1}, mensaje:{2}", num_solicitud, respuesta.Estado, respuesta.Mensaje));

                if (cod_estado_rpp == 7)
                {
                    if (validarSolicitudRPPlus.CodigoEstadoPlaft.ToString() == Enums.EstadoPlaft.Evaluacion.StringValue())
                    {
                        respuestaPlaft = RechazarPlaft(num_solicitud, gls_observacion);

                        if (respuestaPlaft._meta.status == "ERROR")
                        {
                            respuesta.Mensaje = "CWRV: La solicitud fue rechazada correctamente";
                            respuesta.Mensaje += "<br> " + "PLAFT: " + respuestaPlaft.records.message;
                        }
                    }
                }

                if (respuesta.Estado == "OK")
                {
                    string str_SupervisorAgente = string.Empty;
                    BEUsuario datosUsuario = null;
                    BEUsuario datosUsuarioCorreo = null;
                    IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();

                    log.Info("ObtenerEstadoSolicitudRPPlus:" + num_solicitud);
                    SolicitudRPPlus solicitudRPPlus = cotizadorServicio.ObtenerEstadoSolicitudRPPlus(num_solicitud);
                    ServicioAzmanClient servicioAzman = new ServicioAzmanClient("epAzman");

                    //Obtener Arbol de agente
                    log.Info("ObtenerSupervisorAgente:" + solicitudRPPlus.Agente.Usuario);
                    Agente AgenteSupervisor = usuarioServicio.ObtenerSupervisorAgente(solicitudRPPlus.Agente.Usuario);

                    if (AgenteSupervisor.Nombre == null)
                    {
                        string str_destinatario_evaluacion_inteligo = ConfigurationManager.AppSettings["destinatario_envio_evaluacion_inteligo"];

                        str_SupervisorAgente = str_destinatario_evaluacion_inteligo;
                    }
                    else
                    {
                        try
                        {


                            /*Agente*/
                            log.Info(string.Format("servicioAzman.ObtenerDatosUsuarioSinClave: {0} - {1} - {2}",
                                ConfigurationManager.AppSettings["AplicacionAZMAN"], ConfigurationManager.AppSettings["DominioRed"],
                                AgenteSupervisor.Nombre
                                ));

                            datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                    ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                    ConfigurationManager.AppSettings["DominioRed"],
                                    AgenteSupervisor.Nombre);

                            if (datosUsuario != null)
                            {
                                if (datosUsuario.Correo != "")
                                {
                                    //str_SupervisorAgente = datosUsuario.Correo;
                                    str_SupervisorAgente = datosUsuario.Matricula;
                                }
                            }

                            /*Supervisor*/
                            if (AgenteSupervisor.IdPadre != "")
                            {
                                log.Info(string.Format("servicioAzman.ObtenerDatosUsuarioSinClave: {0} - {1} - {2}",
                                ConfigurationManager.AppSettings["AplicacionAZMAN"], ConfigurationManager.AppSettings["DominioRed"],
                                AgenteSupervisor.IdPadre
                                ));

                                datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                        ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                        ConfigurationManager.AppSettings["DominioRed"],
                                        AgenteSupervisor.IdPadre);

                                if (datosUsuario != null)
                                {
                                    if (datosUsuario.Correo != "")
                                    {
                                        str_SupervisorAgente += "," + datosUsuario.Matricula;
                                    }
                                }
                            }

                        }
                        catch (Exception)
                        {
                            log.Debug("No existe el correo para el agente");
                        }

                    }

                    //PLAFT 1=Observado, 2=Rechazado, 3=Aprobado, -1=No Evalúa
                    //Solic 5=Observado, 6=Aprobado, 7=Rechazada
                    if (solicitudRPPlus.CodigoEstadoPlaft == 1 || solicitudRPPlus.CodigoEstadoPlaft == 2 || solicitudRPPlus.CodigoEstadoPlaft == 3 || solicitudRPPlus.CodigoEstadoPlaft == -1)
                    {
                        int cod_proceso = 0;

                        if (cod_estado_rpp == 6 && (solicitudRPPlus.CodigoEstadoPlaft == 3 || solicitudRPPlus.CodigoEstadoPlaft == -1))
                        {
                            cod_proceso = 2;//Aprobado
                        }
                        else if (cod_estado_rpp == 7 || solicitudRPPlus.CodigoEstadoPlaft == 2)
                        {
                            cod_proceso = 4;//Rechazada
                        }
                        else//5
                        {
                            cod_proceso = 3;//Observado
                        }

                        /*Armando Observaciones para el Correo*/
                        string Observaciones = "";

                        if (solicitudRPPlus.GlsObservacionPlaft != "")
                        {
                            Observaciones = "Observaciones de Plaft: " + solicitudRPPlus.GlsObservacionPlaft + "<br />";
                        }

                        if (solicitudRPPlus.GlsObservacionRpp != "")
                        {
                            Observaciones += "Observaciones de Operaciones: " + solicitudRPPlus.GlsObservacionRpp + "<br />";
                        }

                        log.Info("Inicio de Armado de Correo");

                        ConfiguracionCorreo configuracionCorreo = ObtenerConfiguracionCorreo(cod_proceso, DateTime.Today);

                        //AGENTE Y SUPERVISOR //Descomentar
                        configuracionCorreo.gls_destinatario += "," + str_SupervisorAgente;

                        string usuarios_correo = configuracionCorreo.gls_destinatario;

                        if (cod_proceso == 2)//Aprobado
                        {
                            usuarios_correo += "," + ConfigurationManager.AppSettings["destinatario_aprobacion_plaft_operaciones"];
                        }

                        string[] lista_usuarios_correo = usuarios_correo.Split(',');

                        string arc_documento_correo = configuracionCorreo.arc_documento_correo;

                        log.Info("Cantidad Usuarios Correo lista_usuarios_correo:" + lista_usuarios_correo.Count());

                        if (configuracionCorreo.enviar_correo == "S")
                        {
                            foreach (string usuario_correo in lista_usuarios_correo)
                            {
                                if (usuario_correo.Trim() != "")
                                {
                                    Notificacion notificacion = new Notificacion();

                                    log.Info(String.Format("Inicio de Armado de Correo:{0}", usuario_correo));

                                    if (!usuario_correo.Trim().Contains('@'))
                                    {
                                        datosUsuarioCorreo = new BEUsuario();
                                        datosUsuarioCorreo = servicioAzman.ObtenerDatosUsuarioSinClave(
                                        ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                        ConfigurationManager.AppSettings["DominioRed"],
                                        usuario_correo.Trim());
                                    }
                                    else
                                    {
                                        datosUsuarioCorreo = new BEUsuario();
                                        datosUsuarioCorreo.Correo = usuario_correo.Trim();
                                        datosUsuarioCorreo.NombreCompleto = "";
                                    }

                                    configuracionCorreo.arc_documento_correo = arc_documento_correo;

                                    configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{num_solicitud}", num_solicitud);
                                    configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{nombre}", datosUsuarioCorreo.NombreCompleto);
                                    configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{gls_observaciones}", Observaciones);

                                    if (cod_proceso == 3)//Observado
                                    {
                                        string url = string.Empty;

                                        if (num_solicitud.Substring(0, 3) == "RPP")
                                        {
                                            url = ConfigurationManager.AppSettings["url_correo_observacion_rpp"] + "&solicitud=" + num_solicitud;
                                        }
                                        else
                                        {
                                            url = ConfigurationManager.AppSettings["url_correo_observacion_ifp"] + "&solicitud=" + num_solicitud;
                                        }

                                        configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{url}", url);
                                    }
                                    else
                                    {
                                        configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("<a href=\"{url}\">Consulte la propuesta observada haciendo clic aquí</a>", "");
                                    }

                                    configuracionCorreo.gls_asunto = configuracionCorreo.gls_asunto.Replace("{num_solicitud}", num_solicitud);

                                    notificacion.p_destinatario = datosUsuarioCorreo.Correo;

                                    notificacion.p_remitente = configuracionCorreo.gls_remitente;
                                    notificacion.p_asunto = configuracionCorreo.gls_asunto;
                                    notificacion.p_mensaje = configuracionCorreo.arc_documento_correo;
                                    notificacion.p_displayName = configuracionCorreo.gls_display_name;
                                    notificacion.p_ruta_archivo_adjunto = "";

                                    log.Info(String.Format("Fin de Armado de Correo:{0}", usuario_correo));

                                    //Envio de manera Asincrono
                                    var tareaParalela = new System.Threading.Tasks.Task(() =>
                                    {
                                        log.Info(String.Format("Inicio de Envio Notificacion Correo:{0}", notificacion.p_destinatario));
                                        Respuesta RptaNotifica = cotizadorServicio.EnviarNotificacion(configuracionCorreo.gls_ruta_servicio, notificacion);
                                        log.Info(String.Format("RptaNotifica Envio Correo Estado:{0}, Mensaje:{1}", RptaNotifica.Estado, RptaNotifica.Mensaje));
                                        log.Info(String.Format("Fin de Envio Notificacion Correo:{0}", notificacion.p_destinatario));
                                    });
                                    tareaParalela.Start();
                                }
                            }
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message.ToString();
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
            }

            log.Debug("Fin ServicioCWRV.ActualizarSolicitudOperaciones");

            return respuesta;
        }

        //<INI.GTI_7012_S16>

        public Agente ObtenerSupervisorAgente(string usuario)
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.ObtenerSupervisorAgente");

                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                Agente agente = usuarioServicio.ObtenerSupervisorAgente(usuario);

                log.Debug("Fin ServicioCWRV.ObtenerSupervisorAgente");

                return agente;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta RegistrarFlujoEvaluacion(FlujoEvaluacion flujoEvaluacion)
        {
            Respuesta respuesta = new Respuesta();
            try
            {

                log.Debug("Inicio ServicioCWRV.RegistrarFlujoEvaluacion");

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarFlujoEvaluacion(flujoEvaluacion);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();

                respuesta.Mensaje = "Flujo de Evaluación Generada Correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }

            log.Debug("Fin ServicioCWRV.RegistrarFlujoEvaluacion");

            return respuesta;
        }

        public List<SolicitudRPPlus> ListarSolicitudEvaluacion()
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.ListarSolicitudEvaluacion");

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudRPPlus> solicitud = cotizadorServicio.ListarSolicitudEvaluacion();

                log.Debug("Fin ServicioCWRV.ListarSolicitudEvaluacion");

                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public void EnviarEmailListaNegra(Propuesta propuesta)
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.EnviarEmailListaNegra");

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                ServicioAzmanClient servicioAzman = new ServicioAzmanClient("epAzman");
                BEUsuario datosUsuarioCorreo = null;

                JsonTokenPlaft JsonTokenPlaft = ObtenerTokenPlaft();
                JsonEmail jsonEmail = ObtenerEmailPlaft();

                ConfiguracionCorreo configuracionCorreo = ObtenerConfiguracionCorreo(5, DateTime.Today);

                string usuarios_correo = configuracionCorreo.gls_destinatario;

                string[] lista_usuarios_correo = usuarios_correo.Split(',');

                foreach (var email in jsonEmail.records)
                {
                    Array.Resize(ref lista_usuarios_correo, lista_usuarios_correo.Length + 1);
                    lista_usuarios_correo[lista_usuarios_correo.Length - 1] = email.nombre + "," + email.email;
                }

                string arc_documento_correo = configuracionCorreo.arc_documento_correo;

                if (configuracionCorreo.enviar_correo == "S")
                {
                    foreach (string usuario_correo in lista_usuarios_correo)
                    {
                        if (usuario_correo.Trim() != "")
                        {
                            Notificacion notificacion = new Notificacion();
                            if (!usuario_correo.Trim().Contains('@'))
                            {
                                datosUsuarioCorreo = servicioAzman.ObtenerDatosUsuarioSinClave(
                                ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                ConfigurationManager.AppSettings["DominioRed"],
                                usuario_correo.Trim());
                            }
                            else
                            {
                                string usuarios_plaft = usuario_correo.Trim();
                                string[] lista_usuarios_plaft = usuarios_plaft.Split(',');

                                if (lista_usuarios_plaft.Length > 1)
                                {
                                    datosUsuarioCorreo.NombreCompleto = lista_usuarios_plaft[0];
                                    datosUsuarioCorreo.Correo = lista_usuarios_plaft[1];
                                }
                                else
                                {
                                    datosUsuarioCorreo.Correo = lista_usuarios_plaft[0];
                                    datosUsuarioCorreo.NombreCompleto = "";
                                }
                            }

                            configuracionCorreo.arc_documento_correo = arc_documento_correo;
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{nombre}", datosUsuarioCorreo.NombreCompleto);
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{nombre_completo}", propuesta.contratante_apellido_paterno + " " + propuesta.contratante_apellido_materno + " " + propuesta.contratante_nombre1);
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{tipo_documento}", propuesta.contratante_tipo_documento);
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{numero_documento}", propuesta.contratante_documento);
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{fec_nacimiento}", propuesta.contratante_fec_nacimiento.ToString("dd/MM/yyyy"));
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{domicilio}", propuesta.contratante_residencia);
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{producto}", propuesta.producto);
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{prima_unica}", propuesta.moneda + propuesta.prima_anualizada.ToString("#,##0.00"));
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{profesion}", propuesta.contratante_profesion);
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{cargo}", propuesta.contratante_cargo);
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{centro_labores}", propuesta.contratante_centro_labores);
                            configuracionCorreo.arc_documento_correo = configuracionCorreo.arc_documento_correo.Replace("{ingreso_mensual}", propuesta.contratante_ingreso_mensual);

                            notificacion.p_destinatario = datosUsuarioCorreo.Correo;

                            notificacion.p_remitente = configuracionCorreo.gls_remitente;
                            notificacion.p_asunto = configuracionCorreo.gls_asunto;
                            notificacion.p_mensaje = configuracionCorreo.arc_documento_correo;
                            notificacion.p_displayName = configuracionCorreo.gls_display_name;
                            notificacion.p_ruta_archivo_adjunto = "";

                            //Envio de manera Asincrono
                            var tareaParalela = new System.Threading.Tasks.Task(() =>
                            {
                                log.Info("Inicio de Envio Notificacion");
                                cotizadorServicio.EnviarNotificacion(configuracionCorreo.gls_ruta_servicio, notificacion);
                                log.Info("Fin de Envio Notificacion");
                            });
                            tareaParalela.Start();
                        }
                    }
                }

                log.Debug("fin ServicioCWRV.EnviarEmailListaNegra");

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        public ConfiguracionCorreo ObtenerConfiguracionCorreo(int cod_proceso, DateTime fec_solicitud)
        {

            log.Debug("Inicio ServicioCWRV.ObtenerConfiguracionCorreo codProceso:" + cod_proceso.ToString());

            ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
            ConfiguracionCorreo configuracionCorreo = cotizadorServicio.ObtenerConfiguracionCorreo(cod_proceso, DateTime.Today);
            configuracionCorreo.gls_ruta_servicio = ConfigurationManager.AppSettings["url_envio_correo"];
            configuracionCorreo.enviar_correo = ConfigurationManager.AppSettings["enviar_correo"];

            switch (configuracionCorreo.cod_proceso.ToString())
            {
                case "1"://destinatario_envio_evaluacion
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_envio_evaluacion_comercial"];
                    break;
                case "2"://destinatario_aprobacion_plaft_operaciones
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_aprobacion_plaft_comercial"];
                    break;
                case "3"://destinatario_observado_plaft_operaciones
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_observado_plaft_comercial"];
                    break;
                case "4"://destinatario_rechazado_plaft_operaciones
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_rechazado_plaft_comercial"];
                    break;
                case "5"://destinatario_lista_negra
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_lista_negra"];
                    break;
                default:
                    configuracionCorreo.gls_destinatario = ConfigurationManager.AppSettings["destinatario_envio_evaluacion"];
                    break;
            }

            log.Debug("Fin ServicioCWRV.ObtenerConfiguracionCorreo");

            return configuracionCorreo;
        }

        public List<SolicitudRPPlus> ListarSolicitudCierresPlus(string cuspp)
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.ListarSolicitudCierresPlus");

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudRPPlus> solicitud = cotizadorServicio.ListarSolicitudCierres(cuspp);

                log.Debug("Fin ServicioCWRV.ListarSolicitudCierresPlus");

                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public SolicitudRPPlus ObtenerEstadoSolicitudRPPlus(string num_solicitud)
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.ObtenerEstadoSolicitudRPPlus");

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                SolicitudRPPlus solicitud = cotizadorServicio.ObtenerEstadoSolicitudRPPlus(num_solicitud);

                log.Debug("Fin ServicioCWRV.ObtenerEstadoSolicitudRPPlus");

                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public JsonRechazo RechazarPlaft(string num_solicitud, string motivo)
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.RechazarPlaft");

                JsonTokenPlaft JsonTokenPlaft = ObtenerTokenPlaft();
                JsonRechazo oRechazo = null;
                if (JsonTokenPlaft == null)
                {
                    throw new Exception("Token no generado");
                }

                if (JsonTokenPlaft._meta.status == "ERROR")
                {
                    throw new Exception(JsonTokenPlaft._meta.status);
                }


                string url = ConfigurationManager.AppSettings["url_rechazo"].ToString();
                log.Debug("Url:" + url);
                log.Debug(string.Format("num_solicitud:{0}, motivo:{1}", num_solicitud, motivo));

                using (WebClient wc = new WebClient())
                {
                    wc.Headers["Authorization"] = JsonTokenPlaft.records.token_type + " " + JsonTokenPlaft.records.access_token;

                    string client_id2 = ConfigurationManager.AppSettings["client_id2"].ToString();

                    wc.QueryString.Add("client_id", client_id2);
                    wc.QueryString.Add("propuesta", num_solicitud);
                    wc.QueryString.Add("fecha_rechazo", "");
                    wc.QueryString.Add("motivo", motivo);

                    try
                    {
                        var data = wc.UploadValues(url, "POST", wc.QueryString);

                        string responseString = UnicodeEncoding.UTF8.GetString(data);
                        log.Debug(string.Format("Json Devuelto Rechazo de Solicitud{0}: {1}", num_solicitud, responseString));
                        oRechazo = new JavaScriptSerializer().Deserialize<JsonRechazo>(responseString);
                    }
                    catch (Exception _plaft)
                    {
                        log.Error(_plaft.Message, _plaft);
                        throw new Exception("Servicio de Rechazo no disponible");
                    }

                }

                log.Debug("Fin ServicioCWRV.RechazarPlaft");

                return oRechazo;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public string ArchivosExistentes(string num_solicitud)
        {
            try
            {
                log.Debug("Inicio ServicioCWRV.ArchivosExistentes");

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                string Archivos_Existentes = cotizadorServicio.ArchivosExistentes(num_solicitud);

                log.Debug("Fin ServicioCWRV.ArchivosExistentes");

                return Archivos_Existentes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        //<FIN.GTI_7012_S16>


        //public CotizacionMotorIFP CotizarIFP(string cod_tipo_temporalidad, string cod_moneda, DateTime fec_cotizacion, bool ind_flag, string num_solicitud, int num_correlativo)
        //{
        //    try
        //    {
        //        ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
        //        //CotizacionMotorIFP
        //        var cotizacion = cotizadorServicio.CotizarIFP(cod_tipo_temporalidad, cod_moneda, fec_cotizacion, ind_flag, num_solicitud, num_correlativo);
        //        return cotizacion;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message, ex);
        //        throw;
        //    }
        //}

        //public List<CotizacionMotorIFP> CotizarIFPs(DateTime fec_cotizacion, bool ind_flag, string num_solicitud)
        //{
        //    try
        //    {
        //        //ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
        //        //var cotizacion = cotizadorServicio.CotizarIFP(fec_cotizacion, ind_flag, num_solicitud);
        //        //return cotizacion;

        //        ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
        //        var cotizacion = cotizadorServicio.CotizarIFPs(fec_cotizacion, ind_flag, num_solicitud);
        //        return cotizacion;
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(ex.Message, ex);
        //        throw;
        //    }
        //}

        //<INI.GTI_7012_S25>
        public int CantidadSolicitudes(string cuspp, string moneda, double val_mto_prima_unica)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                int Cantidad = cotizadorServicio.CantidadSolicitudes(cuspp, moneda, val_mto_prima_unica);
                return Cantidad;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<List<Parametro>> ObtenerComboboxIFP()
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<List<Parametro>> parametros = generalesServicio.ObtenerComboboxIFP();
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public ParametrosMotorIFP ObtenerParametroGenerales(string cod_tipo_temporalidad, string cod_moneda, DateTime fec_cotizacion, bool ind_flag, string tokenUsuario, ref bool cantidad_megas, string usuario)
        {

            ParametrosMotorIFP parametros = new ParametrosMotorIFP();
            string disco_megas_cotizar = ConfigurationManager.AppSettings["discoMegasCotizar"].ToString();
            long cantidad_megas_cotizar = Convert.ToInt64(ConfigurationManager.AppSettings["cantidadMegasCotizar"].ToString());

            try
            {

                DriveInfo drive = new DriveInfo(disco_megas_cotizar);

                if (drive.IsReady)
                {
                    //string AvailableFreeSpace = drive.AvailableFreeSpace.ToString();
                    //string DriveFormat = drive.DriveFormat;
                    //string DriveType = drive.DriveType.ToString();
                    //string Name = drive.Name;
                    //string RootDirectory = drive.RootDirectory.ToString();
                    //string TotalFreeSpace = drive.TotalFreeSpace.ToString();
                    //string TotalSize = drive.TotalSize.ToString();
                    //string VolumeLabel = drive.VolumeLabel;

                    if (drive.TotalFreeSpace >= cantidad_megas_cotizar)
                    {
                        ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                        parametros = cotizadorServicio.ObtenerParametroGenerales(cod_tipo_temporalidad, cod_moneda, fec_cotizacion, ind_flag, tokenUsuario, usuario);
                        cantidad_megas = false;
                    }
                    else
                    {
                        cantidad_megas = true;
                    }

                }

                return parametros;

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }

            //try
            //{
            //    ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
            //    ParametrosMotorIFP parametros = cotizadorServicio.ObtenerParametroGenerales(cod_tipo_temporalidad, cod_moneda, fec_cotizacion, ind_flag, tokenUsuario);

            //    return parametros;
            //}
            //catch (Exception ex)
            //{
            //    log.Error(ex.Message, ex);
            //    throw;
            //}
        }

        //public Respuesta RegistrarSolicitudIFP(List<ParametrosMotorIFP> lstParametrosMotorIFP, ref SolicitudIFP solicitud)
        public Respuesta RegistrarSolicitudIFP(string tokenUsuario, ref SolicitudIFP solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                log.Debug("Inicio servicioCWRV.RegistrarSolicitudIFP en servicio");
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                //cotizadorServicio.RegistrarSolicitudIFP(lstParametrosMotorIFP, ref solicitud);
                cotizadorServicio.RegistrarSolicitudIFP(tokenUsuario, ref solicitud);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
                log.Debug("Fin servicioCotizador.RegistrarSolicitudIFP en servicio");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        //public Respuesta ActualizarSolicitudIFP(List<ParametrosMotorIFP> lstParametrosMotorIFP, ref SolicitudIFP solicitud)
        public Respuesta ActualizarSolicitudIFP(string tokenUsuario, ref SolicitudIFP solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                log.Debug("Inicio servicioCWRV.ActualizarSolicitudIFP en servicio");
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                //cotizadorServicio.ActualizarSolicitudIFP(lstParametrosMotorIFP, ref solicitud);
                cotizadorServicio.ActualizarSolicitudIFP(tokenUsuario, ref solicitud);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
                log.Debug("Fin servicioCotizador.ActualizarSolicitudIFP en servicio");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public SolicitudIFP ObtenerDatosSolicitudIFP(string idSolicitud)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                SolicitudIFP solicitudIFP = cotizadorServicio.ObtenerDatosSolicitudIFP(idSolicitud);
                return solicitudIFP;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<SolicitudIFP> ListarSolicitudIFP(string cuspp)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<SolicitudIFP> solicitud = cotizadorServicio.ListarSolicitudIFP(cuspp);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<FIN.GTI_7012_S25>

        //<INI.GTI_7012_S27>
        public List<DatosSol> ObtenerDatosporSolicitudIFP(string num_Solicitud)
        {
            try
            {
                ICotizadorServicio generalesServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<DatosSol> datosSolicitud = generalesServicio.ObtenerDatosporSolicitudIFP(num_Solicitud);
                return datosSolicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<RolDcom> ListarRangoDcomIFP(RolDcom rolDcom)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<RolDcom> lstRango = cotizadorServicio.ListarRangoDcomIFP(rolDcom);
                return lstRango;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<FIN.GTI_7012_S27>
        public EmisionPoliza EmitirPolizaElectronica(string num_solicitud, int num_poliza, string dig_poliza, string TipoProducto)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                EmisionPoliza emisionPoliza;
                emisionPoliza = cotizadorServicio.EmitirPolizaIFP(num_solicitud, num_poliza, dig_poliza);
                return emisionPoliza;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        private Respuesta EnviarPolizaElectronicaPDF(string num_solicitud, int num_poliza, string dig_poliza, EmisionPoliza emisionPoliza, string Usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                string urlenviopoliza = ConfigurationManager.AppSettings["url_enviopolizaelectonica"].ToString();
                string rutapdf = ConfigurationManager.AppSettings["polizaElectronicaOUT"].ToString();
                string procesoSME = ConfigurationManager.AppSettings["procesoSME"].ToString();

                log.Debug(string.Format("Inicio Envio de Poliza Electronica Solicitud Nro: {0}, Poliza Nro: {1}, DigPoliza: {2}", num_solicitud, num_poliza, dig_poliza));

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlenviopoliza);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";

                JsonCorreoPolizaElectonica JsonCorreoPolizaElectonica = ConstructorCorreo(emisionPoliza, procesoSME, rutapdf);
                string json = JsonConvert.SerializeObject(JsonCorreoPolizaElectonica);

                try
                {
                    //log.Debug("EnviarPolizaElectronicaPDF");
                    log.Debug("Url: " + urlenviopoliza);
                    log.Debug(string.Format("Inicio Campos num_solicitud:{0} -------------------------------------", num_solicitud));
                    log.Debug(json);
                    log.Debug(string.Format("Fin Campos num_solicitud {0}----------------------------------------", num_solicitud));
                }
                catch (Exception _log)
                {
                    log.Error(_log.Message, _log);
                }

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                string SME;
                dynamic RptaSME;

                //log.Debug(string.Format("Inicio invocacion de Api envio de Correo SME, JSON: {}", json));

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    RptaSME = new JavaScriptSerializer().DeserializeObject(result);
                }

                SME = Convert.ToString(RptaSME["codigoSME"]);

                respuesta = ActualizarPolizaSME(emisionPoliza.Poliza.NumPoliza.ToString(), Convert.ToInt32(SME), Usuario);
                log.Debug("Fin servicioCotizador.EnviarPolizaElectronicaPDF en servicio");

                // Insertar en la tabla de seguimiento
                EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                {
                    gls_identificador = string.Format("{0}|{1}", 1, num_solicitud),
                    id_proceso_envio = (int)Enums.ProcesoEnvio.PolizaElectronicaIFP,
                    id_sme = Convert.ToInt64(SME),
                    cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                    gls_mail = JsonCorreoPolizaElectonica.Email,
                    fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                    cod_agente = emisionPoliza.SolicitudIFP.Agente.Id,
                    aud_usr_ingreso = Usuario
                };
                string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                var JsonSerializar = new JavaScriptSerializer();
                string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                log.Info("Consumiendo API de envío de seguimiento" + rutaEnvioSeguimiento);
                log.Debug(string.Format("Request Body[{0}]", jsonString));
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                    respuesta.Estado = Constante.COD_OK;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
            return respuesta;
        }

        public Respuesta GenerarPolizaElectronicaPDF(string num_solicitud, int num_poliza, string dig_poliza, GrupoFamiliar grupoFamiliar, string TipoProducto, string Usuario)
        {

            Respuesta respuesta = new Respuesta();

            try
            {
                log.Debug($"INICIO servicioCotizador.GenerarPolizaElectronicaPDF-Solicitud Nro: {num_solicitud}, Poliza Nro: {num_poliza}, DigPoliza: {dig_poliza}");

                log.Info("Accediendo a las Key necesarias");
                string urlApiGenerarPdfPoliza = ConfigurationManager.AppSettings["url_pdf_emision_poliza_admwr"].ToString();
                string usuarioAdmwrApi = ConfigurationManager.AppSettings["usuario_admwr_api"].ToString();
                string contraseñaAdmwrApi = ConfigurationManager.AppSettings["contraseña_admwr_api"].ToString();
                string carpetaPdfPolizaGenerada = ConfigurationManager.AppSettings["polizaElectronicaOUT"].ToString();

                EmisionPoliza emisionPoliza = EmitirPolizaElectronica(num_solicitud, num_poliza, dig_poliza, TipoProducto);

                var myWebClient = new WebClient();

                log.Info("Se asignan credenciales al header de la petición");
                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuarioAdmwrApi + ":" + contraseñaAdmwrApi));
                myWebClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

                log.Info("Consumiendo método para generar pdf de póliza");
                urlApiGenerarPdfPoliza = string.Format(urlApiGenerarPdfPoliza, emisionPoliza.Poliza.NumPoliza.ToString());
                byte[] polizaByteArray = myWebClient.DownloadData(urlApiGenerarPdfPoliza);
                myWebClient.Dispose();

                var nombreArchivo = "PolizaVI" + emisionPoliza.Poliza.NumPoliza.ToString() + ".pdf";
                var archivoTemporal = carpetaPdfPolizaGenerada + nombreArchivo;

                log.Info("Se crear archivo pdf en ruta indicada");
                if (File.Exists(archivoTemporal))
                    File.Delete(archivoTemporal);

                File.WriteAllBytes(archivoTemporal, polizaByteArray);

                EnviarPolizaElectronicaPDF(num_solicitud, num_poliza, dig_poliza, emisionPoliza, Usuario);
                respuesta.Estado = Constante.COD_OK;

                log.Debug("FIN servicioCotizador.GenerarPolizaElectronicaPDF");
            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                log.Error(ex.Message, ex);
            }

            return respuesta;
        }

        public JsonArchivos ConstruirArchivos(EmisionPoliza emisionPoliza, GrupoFamiliar grupoFamiliar)
        {
            JsonArchivos jsonArchivos = new JsonArchivos();
            List<Archivo> archivos = new List<Archivo>();
            string polizaElectronicaIn = ConfigurationManager.AppSettings["polizaElectronicaIN"].ToString();
            string polizaElectronicaOut = ConfigurationManager.AppSettings["polizaElectronicaOUT"].ToString();

            CondicionesGenerales condicionesGenerales = CondicionesGenerales(emisionPoliza);
            CondicionesParticulares condicionesParticulares = CondicionesParticulares(emisionPoliza, grupoFamiliar);

            //Archivos
            archivos.Add(new Archivo() { RutaWord = polizaElectronicaIn + "Resumen IFPplus.docx", Trama = condicionesGenerales });//0
            archivos.Add(new Archivo() { RutaWord = polizaElectronicaIn + "CG IFPlus.docx", Trama = condicionesGenerales });//1
            archivos.Add(new Archivo() { RutaWord = polizaElectronicaIn + "CP IFPlus V2.docx", Trama = condicionesParticulares });//2    

            //condicionado
            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Id == Enums.Planes.PLAN2.StringValue())
            {
                if (emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDev > 0)
                {
                    log.Debug("CA Devolución de prima a todo evento nueva");
                    archivos.Add(new Archivo() { RutaWord = polizaElectronicaIn + "CA Devolución de prima a todo evento nueva.docx" });//3
                }
            }

            //condicionado
            //if (emisionPoliza.SolicitudIFP.Cotizaciones[0].IndGastoSepelio != "S")
            //{
            log.Debug("CA Gastos de Sepelio");
            archivos.Add(new Archivo() { RutaWord = polizaElectronicaIn + "CA Gastos de Sepelio.docx" });//4)
            //}

            //condicionado
            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].PagoDoble > 0)
            {
                log.Debug("CA Pago Doble por n años");
                archivos.Add(new Archivo() { RutaWord = polizaElectronicaIn + "CA Pago Doble por n años.docx" });//5
            }

            //condicionado
            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].PeriodoGarantizado > 0)
            {
                log.Debug("CA Período Garantizado");
                archivos.Add(new Archivo() { RutaWord = polizaElectronicaIn + "CA Período Garantizado.docx" });//6
            }

            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Id == Enums.Planes.PLAN1.StringValue())
            {
                //condicionado
                if (emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDev > 0)
                {
                    log.Debug("CA Devolución de prima por Sobrevivencia");
                    archivos.Add(new Archivo() { RutaWord = polizaElectronicaIn + "CA Devolución de prima por Sobrevivencia.docx" });//7
                } //condicionado
                if (emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDevFallec > 0)
                {
                    log.Debug("CA Devolución de prima por fallecimiento");
                    archivos.Add(new Archivo() { RutaWord = polizaElectronicaIn + "CA Devolución de prima por fallecimiento.docx" });//8
                }
            }

            jsonArchivos.Archivos = archivos;
            jsonArchivos.RutaArchivo = polizaElectronicaOut;
            jsonArchivos.NombreArchivo = "PolizaVI" + emisionPoliza.Poliza.NumPoliza.ToString() + ".pdf";

            return jsonArchivos;
        }

        public static JsonCorreoPolizaElectonica ConstructorCorreo(EmisionPoliza emisionPoliza, string strprocesoSME, string rutapdf)
        {
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

            var beneficiario = emisionPoliza.SolicitudIFP.Beneficiarios.Find(ben => ben.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

            return new JsonCorreoPolizaElectonica()
            {
                Email = beneficiario.CorreoElectronico,
                NumeroPoliza = "VI" + emisionPoliza.Poliza.NumPoliza.ToString(),
                NumeroDocumento = beneficiario.Identificacion.Numero,
                Destinatario = beneficiario.ApellidosNombres,
                ProcesoSme = strprocesoSME,
                RutaPdf = rutapdf + "PolizaVI" + emisionPoliza.Poliza.NumPoliza.ToString() + ".pdf",
                Contrasenia = beneficiario.Identificacion.Numero, //DNI
                CamposDinamicos = new JsonCamposDinamicos
                {
                    Id_Nombre = ti.ToTitleCase(beneficiario.Nombre.ToLower()),
                    Id_Renta = emisionPoliza.SolicitudIFP.Cotizaciones[0].PensionCiaMO.ToString("#,##0.00"),
                    Id_Moneda = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Nombre,
                    Id_Temporalidad = emisionPoliza.SolicitudIFP.Cotizaciones[0].Temporalidad.Nombre.ToString(),
                    Id_FechaDevengue = ti.ToTitleCase(emisionPoliza.Poliza.FecPago.ToString("MMMM yyyy", CultureInfo.CreateSpecificCulture("es-PE")))
                }
            };
        }

        public CondicionesGenerales CondicionesGenerales(EmisionPoliza emisionPoliza)
        {
            CondicionesGenerales condicionesGenerales = new CondicionesGenerales();

            condicionesGenerales.Plan1 = emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Nombre.ToUpper();
            condicionesGenerales.Plan2 = emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Nombre.ToUpper();
            condicionesGenerales.CodigoSBS = Enums.CodigoSBS.IFP.StringValue();

            return condicionesGenerales;
        }

        public CondicionesParticulares CondicionesParticulares(EmisionPoliza emisionPoliza, GrupoFamiliar grupoFamiliar)
        {
            CondicionesParticulares condicionesParticulares;

            string NroDocumento = "", Sexo = "", FechaFinVigencia = "", FactorAjusteRenta = "", ReembolsoGastosSepelio = "", PeriodoGarantizado = "";
            string FechaClausula = "", PagoDoble = "", AñosPagoDoble = "", DevolucionPrimaUnica = "", PorcentajeDevolucionPrimaUnicaSobrvivencia = "";
            string DevolucionPrimaUnicaPorFallecimiento = "", PagoSepelio = "", CoberturaAdicional1 = "", CoberturaAdicional2 = "", Telefono = "";
            DateTime fechaDevengue;

            switch (emisionPoliza.SolicitudIFP.Beneficiarios[0].Identificacion.IdTipo.ToUpper())
            {
                case "D":
                    NroDocumento = emisionPoliza.SolicitudIFP.Beneficiarios[0].Identificacion.Numero.ToString();
                    break;
                case "E":
                    NroDocumento = emisionPoliza.SolicitudIFP.Beneficiarios[0].Identificacion.Numero.ToString();
                    break;
                default:
                    NroDocumento = emisionPoliza.SolicitudIFP.Beneficiarios[0].Identificacion.Numero.ToString();
                    break;
            }

            if (emisionPoliza.SolicitudIFP.Beneficiarios[0].Sexo.ToString() == "M")
                Sexo = "MASCULINO";
            else
                Sexo = "FEMENINO";

            Telefono = grupoFamiliar.Telefono1;

            if (grupoFamiliar.Telefono2 != "")
                Telefono += " - " + grupoFamiliar.Telefono2;

            fechaDevengue = Convert.ToDateTime(emisionPoliza.SolicitudIFP.FechaDevengue);

            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Temporalidad.Id == "TVT")
                FechaFinVigencia = "-";
            else
                FechaFinVigencia = "23.59 HORAS DEL " + emisionPoliza.Poliza.FecFinVigencia.ToString("dd/MM/yyyy");

            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Moneda.Id == "001")
                FactorAjusteRenta = "IPC";
            else
            {
                if (emisionPoliza.SolicitudIFP.Cotizaciones[0].ValMonAju == 0)
                    FactorAjusteRenta = "SIN AJUSTE";
                else
                    FactorAjusteRenta = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValMonAju.ToString("#,##0.00") + "%";
            }

            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].IndGastoSepelio == "S")
                ReembolsoGastosSepelio = "S/ " + emisionPoliza.ValGastoSepelio.ToString("#,##0.00");
            else
                ReembolsoGastosSepelio = "NO";

            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].PeriodoGarantizado > 0)
            {
                PeriodoGarantizado = "SI";
                FechaClausula = Utilitarios.ConvertirNroLetras(emisionPoliza.SolicitudIFP.Cotizaciones[0].PeriodoGarantizado);
                FechaClausula = FechaClausula + " (" + emisionPoliza.SolicitudIFP.Cotizaciones[0].PeriodoGarantizado.ToString() + ") AÑOS ";

                //FechaClausula = FechaClausula + "contados desde " + emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy") + " hasta el ";
                //FechaClausula = FechaClausula + emisionPoliza.Poliza.FecPago.AddYears(emisionPoliza.SolicitudIFP.Cotizaciones[0].PeriodoGarantizado).AddDays(-1).ToString("dd/MM/yyyy");

                FechaClausula = FechaClausula + "contados desde " + emisionPoliza.Poliza.FecInicioVigencia.ToString("dd/MM/yyyy") + " hasta el ";

                //if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Temporalidad.Anhos == emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPerDiferido)
                //{
                //FechaClausula = FechaClausula + emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy");
                //}
                //else
                //{
                //    FechaClausula = FechaClausula + "contados desde " + emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy") + " hasta el ";
                //FechaClausula = FechaClausula + emisionPoliza.Poliza.FecFinVigencia.ToString("dd/MM/yyyy");
                //}

                FechaClausula = FechaClausula + emisionPoliza.Poliza.FecFinVigencia.ToString("dd/MM/yyyy");
            }
            else
            {
                PeriodoGarantizado = "NO";
                FechaClausula = "NO APLICA";
            }

            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].PagoDoble > 0)
            {
                PagoDoble = "SI";
                AñosPagoDoble = Utilitarios.ConvertirNroLetras(emisionPoliza.SolicitudIFP.Cotizaciones[0].PagoDoble);
                AñosPagoDoble = AñosPagoDoble + " (" + emisionPoliza.SolicitudIFP.Cotizaciones[0].PagoDoble + ") AÑOS ";
                AñosPagoDoble = AñosPagoDoble + "contados desde " + emisionPoliza.Poliza.FecInicioPagoDoble.ToString("dd/MM/yyyy") + " hasta el ";
                AñosPagoDoble = AñosPagoDoble + emisionPoliza.Poliza.FecFinPagoDoble.ToString("dd/MM/yyyy");
            }
            else
            {
                PagoDoble = "NO";
                AñosPagoDoble = "NO APLICA";
            }

            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Id == Enums.Planes.PLAN2.StringValue())
            {
                if (emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDev > 0)
                    DevolucionPrimaUnica = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDev.ToString("#,##0.00") + "%";
                else
                    DevolucionPrimaUnica = "NO";
                PorcentajeDevolucionPrimaUnicaSobrvivencia = "NO";
                DevolucionPrimaUnicaPorFallecimiento = "NO";
            }
            else if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Id == Enums.Planes.PLAN1.StringValue())
            {
                DevolucionPrimaUnica = "NO";

                if (emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDev > 0)
                    PorcentajeDevolucionPrimaUnicaSobrvivencia = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDev.ToString("#,##0.00") + "%";
                else
                    PorcentajeDevolucionPrimaUnicaSobrvivencia = "NO";

                if (emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDevFallec > 0)
                    DevolucionPrimaUnicaPorFallecimiento = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDevFallec.ToString("#,##0.00") + "%";
                else
                    DevolucionPrimaUnicaPorFallecimiento = "NO";
            }

            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].ValDevFallec > 0)
            {
                PagoSepelio = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo.ToString() + " "
                    + emisionPoliza.SolicitudIFP.Cotizaciones[0].ValDevFallec.ToString("#,##0.00");
            }
            else
                PagoSepelio = " ";

            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Id.ToString() == Enums.Planes.PLAN1.StringValue())
            {
                CoberturaAdicional1 = Enums.DescripcionCobertura1.PLAN1.StringValue();
                CoberturaAdicional2 = Enums.DescripcionCobertura2.PLAN1.StringValue();
            }
            else if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Id.ToString() == Enums.Planes.PLAN2.StringValue())
            {
                CoberturaAdicional1 = Enums.DescripcionCobertura1.PLAN2.StringValue();
                CoberturaAdicional2 = Enums.DescripcionCobertura2.PLAN2.StringValue();
            }

            //string ApellidosNombresGrilla = "", DocIdentidadGrilla = "", FecNacGrilla = "", ParentescoGrilla = "", PorRentaGrilla = "", NroDocIdGrilla = "";
            //string ApellidosNombres = "", TipoDocIden = "", NroDocIde = "", FecNac = "", Parentesco = "", PorcCober = "";

            condicionesParticulares = new CondicionesParticulares()
            {
                Plan = emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Nombre.ToUpper(),
                SBS = Enums.CodigoSBS.IFP.StringValue(),
                NroPoliza = "VI" + emisionPoliza.Poliza.NumPoliza.ToString(),
                ApellidosNombres = emisionPoliza.SolicitudIFP.Beneficiarios[0].ApellidosNombres,
                DocIdentidad = emisionPoliza.SolicitudIFP.Beneficiarios[0].Identificacion.GlosaTipo,
                NumeroDocIdentidad = NroDocumento,
                FechaNacimiento = emisionPoliza.SolicitudIFP.Beneficiarios[0].FechaNacimiento.Value.ToString("dd/MM/yyyy"),
                Sexo = Sexo,
                Direccion = grupoFamiliar.Direccion,
                Distrito = grupoFamiliar.Distrito,
                Provincia = grupoFamiliar.Provincia,
                Departamento = grupoFamiliar.Departamento,
                Telefono = Telefono,
                CorreoElectronico = emisionPoliza.SolicitudIFP.Beneficiarios[0].CorreoElectronico,
                MonedaPrima = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Nombre,
                FechaEmision = emisionPoliza.Poliza.FecEmision.ToString("dd/MM/yyyy"),
                //FechaEmision = "15/03/2019",
                FechaInicioVigencia = "00.00 HORAS DEL " + emisionPoliza.Poliza.FecInicioVigencia.ToString("dd/MM/yyyy"),
                FechaFinVigencia = FechaFinVigencia,
                //FechaDevengueRenta = fechaDevengue.ToString("dd/MM/yyyy"),
                FechaDevengueRenta = emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy"),
                PlazoVigencia = emisionPoliza.SolicitudIFP.Cotizaciones[0].Temporalidad.Nombre,
                MonedaPagoRenta = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Nombre,
                FactorAjusteRenta = FactorAjusteRenta,
                FechaInicioPagoRentas = emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy"),
                //MontoBaseRentMensual = emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy"),
                MontoBaseRentMensual = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].PensionCiaMO.ToString("#,##0.00"),
                RembolsoGastoSepelio = ReembolsoGastosSepelio,
                PeriodoGara = PeriodoGarantizado,
                FechaClausula = FechaClausula,
                PagoDoble = PagoDoble,
                AñosPagoDoble = AñosPagoDoble,
                DevolucionPrimaUnica = DevolucionPrimaUnica,
                PDevoPUFalle = PorcentajeDevolucionPrimaUnicaSobrvivencia,
                DevoPUFallecimiento = DevolucionPrimaUnicaPorFallecimiento,
                Banco = emisionPoliza.SolicitudIFP.Beneficiarios[0].Banco.Glosa,
                NumCuentaAhorros = emisionPoliza.SolicitudIFP.Beneficiarios[0].NumeroBanco,
                PrimaComercialTotal = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.Poliza.ValPrimaNeta.ToString("#,##0.00"),
                IGVTotal = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.Poliza.ValIva.ToString("#,##0.00"),
                PrimaComercialTotalIGV = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.Poliza.ValPrimaNeta.ToString("#,##0.00"),
                TIRGarantizada = emisionPoliza.SolicitudIFP.Cotizaciones[0].TasaVenta.ToString("#,##0.00") + "%",
                PagoRenta = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].PensionCiaMO.ToString("#,##0.00"),
                PrimaComercial = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + Math.Round(emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResPension, 2, MidpointRounding.AwayFromZero).ToString("#,##0.00"),
                IGV = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.Poliza.ValIva.ToString("#,##0.00"),
                PrimaComercialIGV = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResPension.ToString("#,##0.00"),
                CoberturaAdicional1 = CoberturaAdicional1,
                CoberturaAdicional2 = CoberturaAdicional2,
                PagoSepelio = PagoSepelio,
                DevSepe = "S/ " + emisionPoliza.ValGastoSepelio.ToString("#,##0.00"),
                PrimaComercialSepelio = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResSepelio.ToString("#,##0.00"),
                IGVSepelio = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.Poliza.ValIva.ToString("#,##0.00"),
                PrimaComercialIGVSepelio = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResSepelio.ToString("#,##0.00"),
                PagoDevolucion = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].ValDev.ToString("#,##0.00"),
                PrimaComercialDevolucion = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResDevolucion.ToString("#,##0.00"),
                IGVDevolucion = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.Poliza.ValIva.ToString("#,##0.00"),
                PrimaComercialIGVDevo = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResDevolucion.ToString("#,##0.00"),
                ComisionPromotor = emisionPoliza.PjeComision.ToString("#,##0.00") + "%"
            };

            int cont = 0;

            if (emisionPoliza.SolicitudIFP.Beneficiarios.Count > 1)
            {
                for (int i = 1; i <= emisionPoliza.SolicitudIFP.Beneficiarios.Count - 1; i++)
                {
                    string apellidosNombres, docIdentidad, fecNac, parentesco, porRenta, nroDocId;

                    cont += 1;
                    apellidosNombres = emisionPoliza.SolicitudIFP.Beneficiarios[i].ApellidoPaterno.ToUpper() + " " + emisionPoliza.SolicitudIFP.Beneficiarios[i].ApellidoMaterno.ToUpper() + " " + emisionPoliza.SolicitudIFP.Beneficiarios[i].Nombre.ToUpper();
                    docIdentidad = emisionPoliza.SolicitudIFP.Beneficiarios[i].Identificacion.GlosaTipo.ToUpper();
                    fecNac = emisionPoliza.SolicitudIFP.Beneficiarios[i].FechaNacimiento.Value.ToString("dd/MM/yyyy");
                    parentesco = emisionPoliza.SolicitudIFP.Beneficiarios[i].Parentesco.Nombre.ToUpper();
                    porRenta = (emisionPoliza.SolicitudIFP.Beneficiarios[i].ValPjeRenta * 100).ToString("#,##0.00");

                    switch (emisionPoliza.SolicitudIFP.Beneficiarios[i].Identificacion.IdTipo.ToUpper())
                    {
                        case "D":
                            nroDocId = emisionPoliza.SolicitudIFP.Beneficiarios[i].Identificacion.Numero.ToString();
                            break;
                        case "E":
                            nroDocId = emisionPoliza.SolicitudIFP.Beneficiarios[i].Identificacion.Numero.ToString();
                            break;
                        default:
                            nroDocId = emisionPoliza.SolicitudIFP.Beneficiarios[i].Identificacion.Numero.ToString();
                            break;
                    }

                    condicionesParticulares.GetType().GetProperty("ApellidosNombresGrilla" + i).SetValue(condicionesParticulares, apellidosNombres, null);
                    condicionesParticulares.GetType().GetProperty("DocIdentidadGrilla" + i).SetValue(condicionesParticulares, docIdentidad, null);
                    condicionesParticulares.GetType().GetProperty("FecNacGrilla" + i).SetValue(condicionesParticulares, fecNac, null);
                    condicionesParticulares.GetType().GetProperty("ParentescoGrilla" + i).SetValue(condicionesParticulares, parentesco, null);
                    condicionesParticulares.GetType().GetProperty("PorRentaGrilla" + i).SetValue(condicionesParticulares, porRenta, null);
                    condicionesParticulares.GetType().GetProperty("NroDocIdGrilla" + i).SetValue(condicionesParticulares, nroDocId, null);
                }
            }
            for (int i = cont + 1; i <= 10; i++)
            {
                condicionesParticulares.GetType().GetProperty("ApellidosNombresGrilla" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("DocIdentidadGrilla" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("FecNacGrilla" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("ParentescoGrilla" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("PorRentaGrilla" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("NroDocIdGrilla" + i).SetValue(condicionesParticulares, "", null);
            }

            cont = 1;
            for (int i = cont; i <= 3; i++)
            {
                condicionesParticulares.GetType().GetProperty("ApellidosNombres" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("TipoDocIden" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("NroDocIde" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("FecNac" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("Parentesco" + i).SetValue(condicionesParticulares, "", null);
                condicionesParticulares.GetType().GetProperty("PorcCober" + i).SetValue(condicionesParticulares, "", null);
            }

            return condicionesParticulares;
        }

        public string ResumenIFPPlus(EmisionPoliza emisionPoliza)
        {
            const string quote = "\"";
            string json =
                         "{" +
                         quote + "plan1" + quote + ":" + quote + emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Nombre.ToString().ToUpper() + quote + "," +
                         quote + "codigosbs" + quote + ":" + quote + Enums.CodigoSBS.IFP.StringValue() + quote + "," +
                         "}";

            return json;


        }

        public string CartaBienvenida(EmisionPoliza emisionPoliza)
        {
            const string quote = "\"";
            string IngDoble = "";
            string TipoMoneda = "";
            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].PagoDoble > 0)
            {
                IngDoble = "SI";
            }
            else
            {
                IngDoble = "NO";
            }
            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Moneda.Id == "001")
                TipoMoneda = "Indexada";
            else
            {
                if (emisionPoliza.SolicitudIFP.Cotizaciones[0].ValMonAju == 0)
                    TipoMoneda = "Nominal";
                else
                    TipoMoneda = "Ajustada";
            }

            string json =
                           "{" +
                           quote + "Fecha" + quote + ":" + quote + DateTime.Today.ToString("'Lima, ' MMMM ' de ' yyyy", CultureInfo.CreateSpecificCulture("es-PE")) + quote + "," +
                           quote + "Titular" + quote + ":" + quote + emisionPoliza.SolicitudIFP.Beneficiarios[0].ApellidosNombres + quote + "," +
                           quote + "Poliza" + quote + ":" + quote + "N° " + emisionPoliza.Poliza.NumPoliza.ToString() + quote + "," +
                           //quote + "IngMensual" + quote + ":" + quote + emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Simbolo.ToString() + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].PensionCiaMO.ToString("#,##0.00") + quote + "," +
                           quote + "IngMensual" + quote + ":" + quote + " " + emisionPoliza.SolicitudIFP.Cotizaciones[0].PensionCiaMO.ToString("#,##0.00") + quote + "," +
                           quote + "IngDoble" + quote + ":" + quote + IngDoble + quote + "," +
                           quote + "TipoMoneda" + quote + ":" + quote + TipoMoneda + quote + "," +
                           quote + "Temporalidad" + quote + ":" + quote + emisionPoliza.SolicitudIFP.Cotizaciones[0].Temporalidad.Nombre + quote + "," +
                           quote + "IniPago" + quote + ":" + quote + emisionPoliza.Poliza.FecPago.ToString("dd/MM/yyyy") + quote +

                           "}";

            return json;

        }

        public Respuesta GenerarPolizaElectronica(EmisionPoliza emisionPoliza)
        {

            Respuesta respuesta = new Respuesta();
            try
            {
                //<INI.GTI_7012_7>
                string mensaje = "";
                //<FIN.GTI_7012_7>
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                //cotizadorServicio.GenerarPolizaElectronica(emisionPoliza);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                //respuesta.Mensaje = (mensaje == "") ? "Póliza Generada Correctamente." : mensaje;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;

        }

        public Respuesta ActualizarPolizaSME(string gls_poliza, int codigo_SME, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                log.Debug("Inicio envio servicioCotizador. Actualizar Campo SME Poliza en ADMWR");
                //<INI.GTI_7012_7>
                string mensaje = "";
                //<FIN.GTI_7012_7>
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarPolizaSME(gls_poliza, codigo_SME, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = (mensaje == "") ? "Correctamente." : mensaje;
                log.Debug("Fin envio servicioCotizador. Actualizar Campo SME Poliza en ADMWR");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                //respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta GenerarFormatoPEP(DatosSol entidadSolicitud, string nacionalidadDescripcion, string residenciaDescripcion, string institucionLaboralDescripcion, string cargoDescripcion, string profesionDescripcion)
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.GenerarFormatoPEP");

                Respuesta respuesta = new Respuesta();
                //var documentoLog = string.Empty;
                string url = ConfigurationManager.AppSettings["url_generadorpoliza"].ToString();

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";

                JsonArchivos jsonArchivos = new JsonArchivos();
                List<Archivo> archivos = new List<Archivo>();

                string RP = entidadSolicitud.num_solicitud.Substring(0, 3);
                bool IFP = false;
                bool RPP = false;
                string PEPIn = string.Empty;
                string PEPOut = string.Empty;

                if (RP == Enums.TipoProducto.IFP.StringValue())
                {
                    IFP = true;
                    RPP = false;
                }

                if (RP == Enums.TipoProducto.RPP.StringValue())
                {
                    RPP = true;
                    IFP = false;
                }

                if (IFP)
                {
                    PEPIn = ConfigurationManager.AppSettings["SolicitudIfpPepIn"].ToString();
                    PEPOut = ConfigurationManager.AppSettings["SolicitudIfpFormatosOut"].ToString();
                }

                if (RPP)
                {
                    PEPIn = ConfigurationManager.AppSettings["SolicitudRppPepIn"].ToString();
                    PEPOut = ConfigurationManager.AppSettings["SolicitudRppPepOut"].ToString();
                }

                //parametros
                JsonFormatoPEP formatoPEP = new JsonFormatoPEP();

                formatoPEP.propuesta = entidadSolicitud.num_solicitud;
                formatoPEP.apellidoPaterno = entidadSolicitud.ape_paterno_afiliado;
                formatoPEP.apellidoMaterno = entidadSolicitud.ape_materno_afiliado;
                formatoPEP.nombre = entidadSolicitud.nom_nombre_afiliado;

                if (entidadSolicitud.cod_tipo_documento_afiliado == "D")
                {
                    if (IFP)
                    {
                        formatoPEP.dni = "x";
                        formatoPEP.ce = "o";
                    }
                    if (RPP)
                    {
                        formatoPEP.dni = "x";
                        formatoPEP.ce = "o";
                    }
                    //documentoLog = "dni - X";
                }
                else
                {
                    if (IFP)
                    {
                        formatoPEP.ce = "x";
                        formatoPEP.dni = "o";
                    }
                    if (RPP)
                    {
                        formatoPEP.ce = "x";
                        formatoPEP.dni = "o";
                    }
                    //documentoLog = "ce - X";
                }

                formatoPEP.docIdentidad = entidadSolicitud.rut_persona_afiliado;
                formatoPEP.nacionalidadresidencia = nacionalidadDescripcion + " - " + residenciaDescripcion;
                formatoPEP.profesion = profesionDescripcion;
                formatoPEP.nombreinstitucionlabora = institucionLaboralDescripcion;
                formatoPEP.cargo = cargoDescripcion;
                formatoPEP.piepagina1 = entidadSolicitud.num_solicitud;
                formatoPEP.piepaginasolicitud1 = "página 1 de 1";

                archivos.Add(new Archivo() { RutaWord = PEPIn, Trama = formatoPEP });

                jsonArchivos.Archivos = archivos;
                jsonArchivos.RutaArchivo = PEPOut;
                jsonArchivos.NombreArchivo = "PersonaExpuestaPoliticamente.pdf";

                string json = JsonConvert.SerializeObject(jsonArchivos);

                try
                {
                    log.Debug("Generar Formato PEP");
                    log.Debug("Url: " + url);
                    log.Debug(json);
                }
                catch (Exception _log)
                {
                    log.Error(_log.Message, _log);
                }

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var oRespuesta = new JavaScriptSerializer().Deserialize<JsonArchivos>(result);
                    respuesta.ArchivoSerializado = oRespuesta.ArchivoSerializado;
                }

                respuesta.Estado = "OK";

                log.Debug("Fin ServicioCWRV.GenerarFormatoPEP");

                return respuesta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public bool EliminarParametrosGenerales()
        {
            bool resultado = false;

            try
            {
                DateTime fecha = DateTime.Now;

                string rutaArchivo = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\ArchivoParametroIFP";

                string[] Archivos = Directory.GetFiles(rutaArchivo);

                //Eliminando Archivos
                log.Debug("Eliminado Archivos Existentes");

                foreach (string Archivo in Archivos)
                {
                    DateTime modification = File.GetLastWriteTime(Archivo);

                    int diferencia_dias = (fecha - modification).Days;

                    if (diferencia_dias > 0)
                    {
                        File.Delete(Archivo);
                    }
                    else
                    {
                        int diferencia_horas = (fecha - modification).Hours;
                        if (diferencia_horas > 1)
                        {
                            File.Delete(Archivo);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return resultado = true;

        }

        public Respuesta GenerarFormatoOF(DatosSol entidadSolicitud, string descripcionNacionalidad, string descripcionProfesion, string descripcionDepartamento, string descripcionProvincia, string descripcionDistrito, string descripcionEstadoCivil, string descripcionSexo, string descripcionCentrotrabajo, string descripcionActividadeconomica, string descripcionIngresonetomensual, string descripcionCargo, string descripcionMontoOperacion, string descripcionDireccion, string descripcionTipoMoneda)
        {
            try
            {
                log.Debug("Inicio ServicioCWRV.GenerarFormatoOF");

                Respuesta respuesta = new Respuesta();
                //var documentoLog = string.Empty;
                string url = ConfigurationManager.AppSettings["url_generadorpoliza"].ToString();

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";

                JsonArchivos jsonArchivos = new JsonArchivos();
                List<Archivo> archivos = new List<Archivo>();

                string RP = entidadSolicitud.num_solicitud.Substring(0, 3);

                bool IFP = false;
                bool RPP = false;
                string OFIn = string.Empty;
                string OFOut = string.Empty;

                if (RP == Enums.TipoProducto.IFP.StringValue())
                {
                    IFP = true;
                    RPP = false;
                }

                if (RP == Enums.TipoProducto.RPP.StringValue())
                {
                    RPP = true;
                    IFP = false;
                }

                if (IFP)
                {
                    OFIn = ConfigurationManager.AppSettings["SolicitudIfpOFIn"].ToString();
                    OFOut = ConfigurationManager.AppSettings["SolicitudIfpFormatosOut"].ToString();
                }

                if (RPP)
                {
                    OFIn = ConfigurationManager.AppSettings["SolicitudRppOFIn"].ToString();
                    OFOut = ConfigurationManager.AppSettings["SolicitudRppOut"].ToString();
                }

                //parametros
                JsonFormatoOF formatoOF = new JsonFormatoOF();

                //formatoOF.propuesta = entidadSolicitud.num_solicitud;
                formatoOF.ApellidoPaterno = entidadSolicitud.ape_paterno_afiliado;
                formatoOF.ApellidoMaterno = entidadSolicitud.ape_materno_afiliado;
                formatoOF.Nombres = entidadSolicitud.nom_nombre_afiliado;

                //if (entidadSolicitud.cod_tipo_documento_afiliado == "D")
                //{
                //    formatoOF.dni = "X";
                //}
                //else
                //{
                //    formatoOF.ce = "X";
                //}

                formatoOF.FechaNacimiento = entidadSolicitud.fec_nacimiento_beneficiario_day1 + entidadSolicitud.fec_nacimiento_beneficiario_day2 + "/" + entidadSolicitud.fec_nacimiento_beneficiario_month1 + entidadSolicitud.fec_nacimiento_beneficiario_month2 + "/" + entidadSolicitud.fec_nacimiento_beneficiario_year1 + entidadSolicitud.fec_nacimiento_beneficiario_year2 + entidadSolicitud.fec_nacimiento_beneficiario_year3 + entidadSolicitud.fec_nacimiento_beneficiario_year4;
                formatoOF.DocIdentidad = entidadSolicitud.rut_persona_afiliado;
                formatoOF.EstadoCivil = descripcionEstadoCivil;
                formatoOF.Sexo = descripcionSexo;
                formatoOF.Pais = "PERÚ";
                formatoOF.Nacionalidad = descripcionNacionalidad;
                formatoOF.Domicilio = descripcionDireccion;
                formatoOF.Distrito = descripcionDistrito;
                formatoOF.Provincia = descripcionProvincia;
                formatoOF.Departamento = descripcionDepartamento;
                formatoOF.CentroTrabajo = descripcionCentrotrabajo;
                formatoOF.ActividadEconomica = descripcionActividadeconomica;
                formatoOF.IngresoNetoMensual = descripcionIngresonetomensual;
                formatoOF.Cargo = descripcionCargo;
                formatoOF.Ocupacion = descripcionProfesion;
                formatoOF.MontoOperacion = descripcionMontoOperacion;
                formatoOF.Moneda = descripcionTipoMoneda;
                formatoOF.NumeroSolicitud = entidadSolicitud.num_solicitud;

                //formatoOF.piepagina1 = entidadSolicitud.num_solicitud;
                //formatoOF.piepaginasolicitud1 = "página 1 de 1";

                archivos.Add(new Archivo() { RutaWord = OFIn, Trama = formatoOF });

                jsonArchivos.Archivos = archivos;
                jsonArchivos.RutaArchivo = OFOut;
                jsonArchivos.NombreArchivo = "OrigenFondos.pdf";

                string json = JsonConvert.SerializeObject(jsonArchivos);

                try
                {
                    log.Debug("Generar Formato OF");
                    log.Debug("Url: " + url);
                    log.Debug(json);
                }
                catch (Exception _log)
                {
                    log.Error(_log.Message, _log);
                }

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var oRespuesta = new JavaScriptSerializer().Deserialize<JsonArchivos>(result);
                    respuesta.ArchivoSerializado = oRespuesta.ArchivoSerializado;
                }

                respuesta.Estado = "OK";

                log.Debug("Fin ServicioCWRV.GenerarFormatoOF");

                return respuesta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public Respuesta GenerarFormatoSolicitud(JsonFormatoSolicitud entidadFormatoSolicitud)
        {
            try
            {

                log.Debug("Inicio ServicioCWRV.GenerarFormatoSolicitud");

                Respuesta respuesta = new Respuesta();
                string url = ConfigurationManager.AppSettings["url_generadorpoliza"].ToString();

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";

                JsonArchivos jsonArchivos = new JsonArchivos();
                List<Archivo> archivos = new List<Archivo>();

                string RP = entidadFormatoSolicitud.NumeroSolicitud.Substring(0, 3);

                bool IFP = false;
                bool RPP = false;
                string SolicitudIn = string.Empty;
                string SolicitudOut = string.Empty;

                if (RP == Enums.TipoProducto.IFP.StringValue())
                {
                    IFP = true;
                    RPP = false;
                }

                if (RP == Enums.TipoProducto.RPP.StringValue())
                {
                    RPP = true;
                    IFP = false;
                }

                if (IFP)
                {
                    SolicitudIn = ConfigurationManager.AppSettings["SolicitudIfpSolicitudIn"].ToString();
                    SolicitudOut = ConfigurationManager.AppSettings["SolicitudIfpFormatosOut"].ToString();
                }

                if (RPP)
                {
                    SolicitudIn = ConfigurationManager.AppSettings["SolicitudRppIn"].ToString();
                    SolicitudOut = ConfigurationManager.AppSettings["SolicitudRppOut"].ToString();
                }

                archivos.Add(new Archivo() { RutaWord = SolicitudIn, Trama = entidadFormatoSolicitud });

                jsonArchivos.Archivos = archivos;
                jsonArchivos.RutaArchivo = SolicitudOut;
                jsonArchivos.NombreArchivo = "Solicitud.pdf";

                string json = JsonConvert.SerializeObject(jsonArchivos);

                try
                {
                    log.Debug("Generar Formato Solicitud");
                    log.Debug("Url: " + url);
                    log.Debug(json);
                }
                catch (Exception _log)
                {
                    log.Error(_log.Message, _log);
                }

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    var oRespuesta = new JavaScriptSerializer().Deserialize<JsonArchivos>(result);
                    respuesta.ArchivoSerializado = oRespuesta.ArchivoSerializado;
                }

                respuesta.Estado = "OK";

                log.Debug("Fin ServicioCWRV.GenerarFormatoSolicitud");

                return respuesta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public string archivoLog(string nombreArchivo)
        {
            string carpetaLog = AppDomain.CurrentDomain.BaseDirectory + @"Logs\";
            string archivoLog = carpetaLog + nombreArchivo;
            string cArchivoLog = carpetaLog + "logTemporal.txt";
            string contenido = string.Empty;

            if (File.Exists(archivoLog))
            {
                log.Debug("archivoLog - " + archivoLog);

                //CREA COPIA TEMPORAL DE LOG
                File.Copy(archivoLog, cArchivoLog);

                contenido = File.ReadAllText(cArchivoLog);

                //CREA COPIA TEMPORAL DE LOG
                File.Delete(cArchivoLog);
            }

            return contenido;
        }

        //<INI.GTI_22543_01>
        public Respuesta CrearAgente(Agente agente)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                usuarioServicio.CrearAgente(agente);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = "ERROR";
            }
            return respuesta;
        }

        public Respuesta ActualizarAgente(Agente agente)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                usuarioServicio.ActualizarAgente(agente);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = "ERROR";
            }
            return respuesta;
        }

        public Respuesta EliminarAgente(Agente agente)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                usuarioServicio.EliminarAgente(agente);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = "ERROR";
            }
            return respuesta;
        }

        public List<Agente> ObtenerJerarquiaAgente(int nivelAgente, string usuario)
        {
            try
            {
                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                var agentes = usuarioServicio.ObtenerJerarquiaAgente(nivelAgente, usuario);
                return agentes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Agente ObtenerAgente(int IdAgente, string usuario)
        {
            try
            {
                IUsuarioServicio usuarioServicio = FabricaIoC.Contenedor.Resolver<IUsuarioServicio>();
                var agentes = usuarioServicio.ObtenerAgente(IdAgente, usuario);
                return agentes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<FIN.GTI_22543_01>

        //<INI.GTI_26560>
        public Respuesta ActualizarConsentimientoAfiliado(Afiliado afiliado, ConsentimientoAsesoria consentimientoAsesoria)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarConsentimientoAfiliado(afiliado, consentimientoAsesoria);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public List<Beneficiario> ListarBeneficiarios(string numSolicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                var beneficiarios = cotizadorServicio.ListarBeneficiarios(numSolicitud, usuario);
                return beneficiarios;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta ActualizarBeneficiario(Beneficiario beneficiario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                var beneficiarios = cotizadorServicio.ActualizarBeneficiario(beneficiario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
                //throw;
            }
            return respuesta;

        }

        //<FIN.GTI_26560>

        //<INI.GTI_26697>
        public Respuesta RegistrarPersonaVinculada(GrupoFamiliar grupo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.RegistrarPersonaVinculada(grupo);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta ActualizarPersonaVinculada(GrupoFamiliar grupo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarPersonaVinculada(grupo);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta EliminarPersonaVinculada(int idPersonaVinculada, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.EliminarPersonaVinculada(idPersonaVinculada, usuario);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
            }
            return respuesta;
        }

        public List<GrupoFamiliar> ObtenerPersonaVinculada(GrupoFamiliar grupo)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                var personasVinculadas = cotizadorServicio.ObtenerPersonaVinculada(grupo);
                return personasVinculadas;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta ActualizarDireccionSolicitud(string numCuspp, string numSolicitud, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarDireccionSolicitud(numCuspp, numSolicitud, usuario);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
            }
            return respuesta;
        }

        public bool isFirmaDigitalAprobada(string numSolicitud, int numItem, string usuario)
        {
            try
            {
                log.Debug("Inicio ServicioCWRV.ObtenerFirmaDigital");

                bool respuesta = false;
                string url = ConfigurationManager.AppSettings["url_cwrvApi"].ToString() + "/firmas-digitales/por-solicitud/" + numSolicitud + "/" + numItem + "/" + usuario;

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                try
                {
                    log.Debug("Obtener Firma Digital");
                    log.Debug("Url: " + url);
                }
                catch (Exception _log)
                {
                    log.Error(_log.Message, _log);
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    FirmaDigital oRespuesta = new JavaScriptSerializer().Deserialize<FirmaDigital>(result);

                    if (oRespuesta != null)
                    {
                        if (oRespuesta.ind_consentimiento == "S")
                            respuesta = true;
                    }
                }

                log.Debug("Fin ServicioCWRV.ObtenerFirmaDigital");

                return respuesta;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        //<FIN.GTI_26697>

        public FirmaDigital ObtenerFirmaDigital(string solicitud, int item, string usuario)
        {
            try
            {
                FirmaDigital firma = null;
                string url = string.Format("{0}/firmas-digitales/por-solicitud/{1}/{2}/{3}", ConfigurationManager.AppSettings["url_cwrvApi"], solicitud, item, usuario);

                log.Debug(string.Format("Se va a consumir el método de consulta de Firma Digital por Solicitud: GET [{0}]", url));
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    firma = new JavaScriptSerializer().Deserialize<FirmaDigital>(result);
                }

                return firma;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public FirmaDigital RegistrarFirmaDigital(FirmaDigital firma)
        {
            try
            {
                string requestBody = JsonConvert.SerializeObject(firma);

                string url = string.Format("{0}/firmas-digitales", ConfigurationManager.AppSettings["url_cwrvApi"]);

                log.Debug(string.Format("Se va a consumir el método para registrar Firma Digital: POST [{0}]", url));
                log.Debug(string.Format("Request Body [{0}]", requestBody));

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.UserAgent = firma.gls_browser_agent;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                // TODO HLS: Actualizar la autorización básica cuando se corrja en el API
                //httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(requestBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    log.Debug(string.Format("Respuesta: [{0}]", result));
                    firma = new JavaScriptSerializer().Deserialize<FirmaDigital>(result);
                }
                return firma;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public FirmaDigital ActualizarFirmaDigital(FirmaDigital firma)
        {
            try
            {
                string requestBody = JsonConvert.SerializeObject(firma);

                string url = string.Format("{0}/firmas-digitales", ConfigurationManager.AppSettings["url_cwrvApi"]);

                log.Debug(string.Format("Se va a consumir el método para actualizar Firma Digital: PUT [{0}]", url));
                log.Debug(string.Format("Request Body [{0}]", requestBody));

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.UserAgent = firma.gls_browser_agent;
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "PUT";
                // TODO HLS: Actualizar la autorización básica cuando se corrja en el API
                //httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(requestBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    log.Debug(string.Format("Respuesta: [{0}]", result));
                    firma = new JavaScriptSerializer().Deserialize<FirmaDigital>(result);
                }
                return firma;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public Respuesta PreseleccionarCotizacion(string num_solicitud, string num_correlativo, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.PreseleccionarCotizacion(num_solicitud, num_correlativo, usuario);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
            }
            return respuesta;
        }

        public NotificacionSME EnviarNotificacionSME(NotificacionSME notificacionSME)
        {
            try
            {
                using (var client = new WebClient())
                {
                    NotificacionSME notificacion = null;
                    client.Encoding = Encoding.UTF8;
                    var serializador = new JavaScriptSerializer();
                    string requestBody = serializador.Serialize(notificacionSME);
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    string result = client.UploadString(new Uri(ConfigurationManager.AppSettings["url_sme_envio_correos"]), "POST", requestBody);
                    notificacion = new JavaScriptSerializer().Deserialize<NotificacionSME>(result);
                    return notificacion;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public Respuesta EnviarNotificacionAizen(Notificacion notificacion)
        {
            //Envio de manera Asincrono
            Respuesta respuesta = new Respuesta();
            try
            {
                List<string> errores = new List<string>();
                if (notificacion == null)
                {
                    errores.Add("Envíe una notificación completa. Dato Obligatorio");
                }
                else
                {
                    if (notificacion.p_remitente == null || notificacion.p_remitente == "")
                    {
                        errores.Add("Ingrese Remitente del Correo. Dato Obligatorio.");
                    }
                }

                if (errores.Count > 0)
                {
                    respuesta.Mensaje = Utilitarios.FormatearErrorTexto(errores);
                    return respuesta;
                }

                if (notificacion.p_destinatario != null)
                {
                    notificacion.p_destinatario = notificacion.p_destinatario.Trim();
                }

                log.Debug("Se va a consumir servicio de Correo Electrónico AIZEN");
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var JsonSerializar = new JavaScriptSerializer();
                    string requestBody = JsonSerializar.Serialize(notificacion);
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    respuesta.Mensaje = client.UploadString(new Uri(ConfigurationManager.AppSettings["url_aizen_envio_correos"]), "POST", requestBody);
                    respuesta.Estado = Constante.COD_OK;
                }
            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
            }
            return respuesta;
        }

        public Respuesta EnviarPolizaElectronicaRPP_PDF(int num_poliza, string Usuario)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                log.Debug("Inicio servicioCotizador.EnviarPolizaElectronicaRPP_PDF en servicio");
                log.Debug(string.Format("Enviar Poliza Electronica Solicitud Poliza Nro: {0}", num_poliza));

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                respuesta = cotizadorServicio.EnviarPolizaElectronicaRPP_PDF(num_poliza, Usuario);
                //respuesta.Estado = Constante.COD_OK;

                log.Debug("Fin servicioCotizador.EnviarPolizaElectronicaRPP_PDF en servicio");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
            }

            return respuesta;
        }


        public GrupoFamiliar ObtenerDatosBenefiCierre(int idGrupoFamiliar, string num_solicitud)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                GrupoFamiliar grupo = cotizadorServicio.ObtenerDatosBenefiCierre(idGrupoFamiliar, num_solicitud);
                return grupo;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta CerrarBeneficiariosIFP(string num_solicitud, List<GrupoFamiliar> lstGrupoFamiliar, string tipoPlan, List<GrupoFamiliar> lstBeneficiariosPNoG, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.CerrarBeneficiariosIFP(num_solicitud, lstGrupoFamiliar, tipoPlan, lstBeneficiariosPNoG, usuario);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Cotización Cerrada Correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta EliminarGrupoFamiliar(GrupoFamiliar grupo)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ElminarGrupoFamiliar(grupo);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        //<GTI.INI-29372>
        public Respuesta ActualizarSolicitudEnvioObligatorio(ref SolicitudEscenario solicitudEscenario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarSolicitudEnvioObligatorio(ref solicitudEscenario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud modificada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public List<ReporteIndicadoresRRVV> ListarReporteIndicadoresRRVV()
        {
            try
            {
                ICotizadorServicio generalesServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<ReporteIndicadoresRRVV> lstReporteIndicadores = generalesServicio.ListarReporteIndicadoresRRVV();
                return lstReporteIndicadores;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta SeleccionarBeneficiario(string num_solicitud, string num_correlativo, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.SeleccionarBeneficiario(num_solicitud, num_correlativo, usuario);
                respuesta.Estado = Constante.COD_OK;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
            }
            return respuesta;
        }

        public List<ConsentimientosAgrupadosAge> ListarConsentimientosAgrupadosAge(string periodo, string usuario)
        {
            try
            {
                ICotizadorServicio generalesServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<ConsentimientosAgrupadosAge> lstConsentimientosAgrupadosAge = generalesServicio.ListarConsentimientosAgrupadosAge(periodo, usuario);
                return lstConsentimientosAgrupadosAge;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Solicitud> ListarSolicitudesCargaMeler(string xml)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Solicitud> solicitud = cotizadorServicio.ListarSolicitudesCargaMeler(xml);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Contrato> ListarContratosCotizaciones(string codUserName)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                List<Contrato> solicitud = generalesServicio.ListarContratosCotizaciones(codUserName);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta ActualizarContratoCotizacion(Contrato contrato)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.ActualizarContratoCotizacion(contrato);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Contrato modificado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta InsertarContratoCotizacion(Contrato contrato)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.InsertarContratoCotizacion(contrato);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Contrato registrado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public Respuesta EliminarContratoCotizacion(int idContrato, string codUserName)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                generalesServicio.EliminarContratoCotizacion(idContrato, codUserName);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Contrato eliminado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }
        //<GTI.FIN-29372>

        public List<FirmaDigitalDashboard> ObtenerFirmaDigitalesDashboard(string fechaInicio, string fechaFin, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<FirmaDigitalDashboard> firmaDigitalDashboard = cotizadorServicio.ObtenerFirmaDigitalesDashboard(fechaInicio, fechaFin, usuario);
                return firmaDigitalDashboard;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<PolizasDashboard> ObtenerPolizasDashboard(string fechaInicio, string fechaFin, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<PolizasDashboard> polizasDashboard = cotizadorServicio.ObtenerPolizasDashboard(fechaInicio, fechaFin, usuario);
                return polizasDashboard;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<ReporteTrazabilidad> ObtenerTrazabilidad(string fechaInicio, string fechaFin, string idProceso, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<ReporteTrazabilidad> listaTrazabilidad = cotizadorServicio.ObtenerTrazabilidad(fechaInicio, fechaFin, idProceso, usuario);
                return listaTrazabilidad;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Respuesta GenerarPolizaRVI(List<Solicitud> solicitudes, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.GenerarPolizaRVI(solicitudes, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Solicitud cotizada correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }


        public Respuesta TransferirDatosPolizaRRVV(string num_solicitud, int numeroCorrelativo, string usuario)
        {
            ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
            return cotizadorServicio.TransferirDatosPolizaRRVV(num_solicitud, numeroCorrelativo, usuario);

            //try
            //{
            //    ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
            //    respuesta = cotizadorServicio.TransferirDatosPolizaRRVV(num_solicitud, usuario);
            //}
            //catch (Exception ex)
            //{
            //    log.Error(ex.Message, ex);
            //    respuesta.Estado = Constante.COD_ERROR;
            //    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><b>No se ha podido completar el proceso debido al siguiente error:</b></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            //    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
            //    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
            //}

            //return respuesta;
        }

        public byte[] ObtenerReporteRecalculoPDF(List<ReporteRecalculoCotizacion> listaRecalculoCotizacion)
        {
            try
            {
                log.Debug("Inicio ObtenerReporteRecalculoPDF");

                //Archivos Plantillas
                string PdfRecalculoTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RVI\\Recalculo\\RecalculoCotizacion.html";

                string RutaLogo = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RVI\\Recalculo\\logo.png";

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                byte[] reporteRecalculoCotizacionbytes = cotizadorServicio.ObtenerReporteRecalculoPDF(listaRecalculoCotizacion, PdfRecalculoTemplate, RutaLogo);

                log.Debug("Fin ObtenerReporteRecalculoPDF");

                return reporteRecalculoCotizacionbytes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public byte[] ObtenerReporteCotizacionesGanadasPDF(int numLote)
        {
            try
            {
                log.Debug("Inicio ObtenerReporteCotizacionesGanadasPDF");

                //Archivos Plantillas
                string PdfRecalculoTemplate = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RVI\\CotizacionesGanadas\\CotizacionesGanadas.html";

                string RutaLogo = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RVI\\CotizacionesGanadas\\logo.png";

                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                byte[] reporteCotizacionesGanadasbytes = cotizadorServicio.ObtenerReporteCotizacionesGanadasPDF(numLote, PdfRecalculoTemplate, RutaLogo);

                log.Debug("Fin ObtenerReporteCotizacionesGanadasPDF");

                return reporteCotizacionesGanadasbytes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Solicitud ListarCotizacionesPorSolicitud(string numeroSolicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                Solicitud solicitud = cotizadorServicio.ListarCotizacionesPorSolicitud(numeroSolicitud, usuario);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public void ActualizarCotizacionGanadoraRVI(string solicitud, int correlativo, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarCotizacionGanadoraRVI(solicitud, correlativo, usuario);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public List<EstadoCivil> ListarEstadoCivil(string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<EstadoCivil> listaEstadoCivil = cotizadorServicio.ListarEstadoCivil(usuario);
                return listaEstadoCivil;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Profesion> ListarProfesion(string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Profesion> listaProfesion = cotizadorServicio.ListarProfesion(usuario);
                return listaProfesion;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public List<Nacionalidad> ListarNacionalidad(string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<Nacionalidad> listaNacionalidad = cotizadorServicio.ListarNacionalidad(usuario);
                return listaNacionalidad;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public int ObtenerIndicadorRescateIFP(string solicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                int ind_Rescate = cotizadorServicio.ObtenerIndicadorRescateIFP(solicitud, usuario);
                return ind_Rescate;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public CotizacionRescate RecotizarSolicitudIFP(string numSolicitud, DateTime fecCotizacion, int numMesRescate, string usuario)
        {
            CotizacionRescate cotizacionRescate = new CotizacionRescate();
            try
            {
                log.Debug("Inicio servicioCWRV.RecotizarSolicitudIFP en servicio");

                log.Debug("numeroSolicitud: " + numSolicitud);
                log.Debug("fecCotizacion: " + fecCotizacion);
                log.Debug("numNesRescate: " + numMesRescate);
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizacionRescate = cotizadorServicio.RecotizarSolicitudIFP(numSolicitud, fecCotizacion, numMesRescate, usuario);

                log.Debug("Fin servicioCotizador.RecotizarSolicitudIFP en servicio");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }

            return cotizacionRescate;
        }

        public SolicitudIFP ValidarCotizacionVigente(string cod_tipo_documento, string num_documento, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                SolicitudIFP solicitud = cotizadorServicio.ValidarCotizacionVigente(cod_tipo_documento, num_documento, usuario);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public List<FormatoSolicitud> ListarFormatosSolicitud(string solicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<FormatoSolicitud> formatos = cotizadorServicio.ListarFormatosSolicitud(solicitud, usuario);
                return formatos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public FormatoSolicitud ObtenerFormatoSolicitud(string solicitud, int idFormatoSolicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                FormatoSolicitud formato = cotizadorServicio.ObtenerFormatoSolicitud(solicitud, idFormatoSolicitud, usuario);
                return formato;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiario(string solicitud, int idFormatoSolicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<FormatoSolicitudBeneficiario> formatos = cotizadorServicio.ListarFormatoSolicitudBeneficiario(solicitud, idFormatoSolicitud, usuario);
                return formatos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculada(string solicitud, int idFormatoSolicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<FormatoSolicitudPersonaVinculada> formatos = cotizadorServicio.ListarFormatoSolicitudPersonaVinculada(solicitud, idFormatoSolicitud, usuario);
                return formatos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public FormatoSolicitud ObtenerFormatoSolicitudActualizado(string solicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                FormatoSolicitud formato = cotizadorServicio.ObtenerFormatoSolicitudActualizado(solicitud, usuario);
                return formato;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiarioActualizado(string solicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<FormatoSolicitudBeneficiario> formatos = cotizadorServicio.ListarFormatoSolicitudBeneficiarioActualizado(solicitud, usuario);
                return formatos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculadaActualizado(string solicitud, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                List<FormatoSolicitudPersonaVinculada> formatos = cotizadorServicio.ListarFormatoSolicitudPersonaVinculadaActualizado(solicitud, usuario);
                return formatos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public List<DateTime> ListarFeriados()
        {
            try
            {
                List<Feriado> feriados = null;
                string url = ConfigurationManager.AppSettings["url_workdays"];

                log.Debug(string.Format("Se va a consumir el endpoint que consulta los feriados: GET [{0}]", url));
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    feriados = JsonConvert.DeserializeObject<List<Feriado>>(result);
                }

                List<DateTime> fechas = new List<DateTime>();
                feriados.ForEach(f => fechas.Add(Convert.ToDateTime(f.Fecha, new CultureInfo("es-PE"))));

                return fechas;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public List<Dominio.Entidades.MotorCalculo.JuegoParametros> ObtenerParametrosRPP(string temporalidad, DateTime fechaCotizacion, string origen, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                var juegosParametros = cotizadorServicio.ObtenerParametrosRPP(temporalidad, fechaCotizacion, origen, usuario);
                return juegosParametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public SolicitudRPPlus CotizarRPP(SolicitudRPPlus solicitud, List<Dominio.Entidades.MotorCalculo.Parametros> parametros)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                solicitud = cotizadorServicio.CotizarRPP(solicitud, parametros);
                return solicitud;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw (ex);
            }
        }

        public double ObtenerTipoCambio(string codigo, DateTime fecha, string usuario)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                var juegosParametros = generalesServicio.ObtenerTipoCambio(codigo, fecha, usuario);
                return juegosParametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public long EnviarCorreoSME(SMEEnvio envio)
        {
            try
            {
                envio.CamposDinamicos = JsonConvert.DeserializeObject(envio.CamposDinamicosSerializados);

                string url_sme_envio_documentos = ConfigurationManager.AppSettings["url_enviopolizaelectonica"].ToString();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url_sme_envio_documentos);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";
                string requestBody = JsonConvert.SerializeObject(envio);

                log.Debug(string.Format("Se va a consumir endpoint para envío de correo [{0}]", url_sme_envio_documentos));
                log.Debug(string.Format("Request Body [{0}]", requestBody));

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(requestBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                dynamic responseBody;
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    responseBody = new JavaScriptSerializer().DeserializeObject(result);
                }

                return Convert.ToInt64(responseBody["codigoSME"]);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public Respuesta ActualizarEstudioNecesidadSolicitud(string num_solicitud, int? id_estudio_necesidades, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarEstudioNecesidadSolicitud(num_solicitud, id_estudio_necesidades, usuario);

                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "Estudio de necesidades actualizado correctamente.";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "No se ha podido completar el proceso debido al siguiente error:" + ex.Message;
            }
            return respuesta;
        }

        public List<Direccion> ListarRviDireccion(string solicitud, string usuario)
        {
            try
            {
                IGeneralesServicio generalesServicio = FabricaIoC.Contenedor.Resolver<IGeneralesServicio>();
                var direcciones = generalesServicio.ListarRviDireccion(solicitud, usuario);
                return direcciones;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public List<Departamento> ListarDepartamentos(string usuario)
        {
            try
            {
                string endpoint = string.Format("{0}/direcciones/departamentos/{1}", ConfigurationManager.AppSettings["url_cwrvApi"], usuario);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.Method = "GET";

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                List<Departamento> departamentos = null;
                log.Debug(string.Format("Endpoint: GET [{0}]", endpoint));
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Length > 0)
                    {
                        departamentos = JsonConvert.DeserializeObject<List<Departamento>>(result);
                    }
                }
                return departamentos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public List<Provincia> ListarProvincias(string idDepartamento, string usuario)
        {
            try
            {
                string endpoint = string.Format("{0}/direcciones/provincias/{1}/{2}", ConfigurationManager.AppSettings["url_cwrvApi"], idDepartamento, usuario);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.Method = "GET";

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                List<Provincia> provincias = null;
                log.Debug(string.Format("Endpoint: GET [{0}]", endpoint));
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Length > 0)
                    {
                        provincias = JsonConvert.DeserializeObject<List<Provincia>>(result);
                    }
                }
                return provincias;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public List<Distrito> ListarDistritos(string idProvincia, string usuario)
        {
            try
            {
                string endpoint = string.Format("{0}/direcciones/distritos/{1}/{2}", ConfigurationManager.AppSettings["url_cwrvApi"], idProvincia, usuario);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.Method = "GET";

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                List<Distrito> distritos = null;
                log.Debug(string.Format("Endpoint: GET [{0}]", endpoint));
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Length > 0)
                    {
                        distritos = JsonConvert.DeserializeObject<List<Distrito>>(result);
                    }
                }
                return distritos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public EnvioSeguimiento RegistrarEnvioSeguimiento(EnvioSeguimiento envioSeguimiento)
        {
            try
            {
                string endpoint = ConfigurationManager.AppSettings["url_envio_seguimiento"];

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";
                string requestBody = JsonConvert.SerializeObject(envioSeguimiento);

                log.Debug(string.Format("Endpoint: POST [{0}]", endpoint));
                log.Debug(string.Format("Request Body [{0}]", requestBody));

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(requestBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseBody = streamReader.ReadToEnd();
                    envioSeguimiento = JsonConvert.DeserializeObject<EnvioSeguimiento>(responseBody);
                }

                return envioSeguimiento;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public RviBdIni ObtenerAseguradoBdIni(string cuspp, string usuario)
        {
            try
            {
                string endpoint = string.Format("{0}/asegurados/bdini/{1}/{2}", ConfigurationManager.AppSettings["url_cwrvApi"], cuspp, usuario);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.Method = "GET";

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                RviBdIni asegurado = null;
                log.Debug(string.Format("Endpoint: GET [{0}]", endpoint));
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Length > 0)
                    {
                        asegurado = JsonConvert.DeserializeObject<RviBdIni>(result);
                    }
                }
                return asegurado;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public RviBdIni RegistrarAseguradoBdIni(RviBdIni asegurado)
        {
            try
            {
                string endpoint = string.Format("{0}/asegurados/bdini", ConfigurationManager.AppSettings["url_cwrvApi"]);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "POST";
                string requestBody = JsonConvert.SerializeObject(asegurado);

                log.Debug(string.Format("Endpoint: POST [{0}]", endpoint));
                log.Debug(string.Format("Request Body [{0}]", requestBody));

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(requestBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseBody = streamReader.ReadToEnd();
                    asegurado = JsonConvert.DeserializeObject<RviBdIni>(responseBody);
                }

                return asegurado;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public RviBdIni ActualizarAseguradoBdIni(RviBdIni asegurado)
        {
            try
            {
                string endpoint = string.Format("{0}/asegurados/bdini", ConfigurationManager.AppSettings["url_cwrvApi"]);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.ContentType = "application/json;charset=utf-8";
                httpWebRequest.Method = "PUT";
                string requestBody = JsonConvert.SerializeObject(asegurado);

                log.Debug(string.Format("Endpoint: POST [{0}]", endpoint));
                log.Debug(string.Format("Request Body [{0}]", requestBody));

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(requestBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var responseBody = streamReader.ReadToEnd();
                    asegurado = JsonConvert.DeserializeObject<RviBdIni>(responseBody);
                }

                return asegurado;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

                //<GTI.59048-INI>
        public AbonoPoliza ObtenerAbonoPorPoliza(int numeroPoliza, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                AbonoPoliza abono = cotizadorServicio.ObtenerAbonoPorPoliza(numeroPoliza, usuario);
                return abono;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public PolizaRV ObtenerDatosPolizaRV(string num_solicitud, int numeroCorrelativo, string usuario)
        {
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                PolizaRV poliza = cotizadorServicio.ObtenerDatosPolizaRV(num_solicitud, numeroCorrelativo, usuario);
                return poliza;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }
        //<GTI.59048-FIN>
		
		public List<ValPar> ListarParametrosValPar(string producto, DateTime fecha, string parametro, string usuario)
        {
            try
            {
                string endpoint = string.Format("{0}/parametro/{1}/{2}/{3}/{4}", ConfigurationManager.AppSettings["url_cwrvApi"], producto, fecha.ToString("yyyy-MM-dd"), parametro, usuario);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.Method = "GET";

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                List<ValPar> parametros = null;
                log.Debug(string.Format("Endpoint: GET [{0}]", endpoint));
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Length > 0)
                    {
                        parametros = JsonConvert.DeserializeObject<List<ValPar>>(result);
                    }
                }
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public List<ValPar> ListarParametrosVigentesValPar(string producto, string parametro, string usuario)
        {
            try
            {
                string endpoint = string.Format("{0}/parametro/{1}/vigentes/{2}/{3}", ConfigurationManager.AppSettings["url_cwrvApi"], producto, parametro, usuario);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.Method = "GET";

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                List<ValPar> parametros = null;
                log.Debug(string.Format("Endpoint: GET [{0}]", endpoint));
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (result.Length > 0)
                    {
                        parametros = JsonConvert.DeserializeObject<List<ValPar>>(result);
                    }
                }
                return parametros;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        public bool ProcesarParametrosvalPar(string producto, List<ValPar> parametros, string usuario)
        {
            try
            {
                string endpoint = string.Format("{0}/parametro/{1}", ConfigurationManager.AppSettings["url_cwrvApi"], producto);

                foreach (var parametro in parametros)
                {
                    //if (existeValPar(producto, parametro))
                    //    ActualizarParametrosValPar(endpoint, producto, parametro, parametro);
                    //else

                    InsertarParametrosValPar(endpoint, producto, parametro, usuario);
                };

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        private bool InsertarParametrosValPar(string endpoint, string producto, ValPar parametro, string usuario)
        {
            try
            {
                Respuesta respuesta = new Respuesta();

                bool continuarInsercion = true;

                if (producto != Enums.TipoProducto.RVI.StringValue())
                {
                    continuarInsercion = ActualizarParametroBase(endpoint, producto, parametro, usuario);
                }

                if (continuarInsercion)
                {
                    parametro.fec_ini_rango = parametro.fec_ini_rango.Date;
                    parametro.fec_fin_rango = parametro.fec_fin_rango.Date;
                    parametro.aud_usr_ingreso = usuario;
                    parametro.aud_fec_ingreso = DateTime.Now;

                    string json = JsonConvert.SerializeObject(parametro);

                    using (var client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        respuesta.Mensaje = client.UploadString(new Uri(endpoint), "POST", json);
                        respuesta.Estado = Constante.COD_OK;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        private bool ActualizarParametrosValPar(string endpoint, string producto, ValPar parametroOrigen, ValPar parametroActualizado)
        {
            try
            {
                Respuesta respuesta = new Respuesta();
                dynamic obj = new ExpandoObject();
                obj.objetoOrigen = parametroOrigen;
                obj.objetoActualizado = parametroActualizado;

                string json = JsonConvert.SerializeObject(obj);

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    respuesta.Mensaje = client.UploadString(new Uri(endpoint), "PUT", json);
                    respuesta.Estado = Constante.COD_OK;
                }

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        private bool existeValPar(string producto, ValPar parametro, string usuario)
        {
            try
            {
                var parametros = ListarParametrosValPar(producto, parametro.fec_fin_rango.Date, parametro.cod_parametro, usuario);
                return parametros.Exists(x => (x.cod_parametro.Equals(parametro.cod_parametro)
                                        && x.fec_ini_rango.Equals(parametro.fec_ini_rango)
                                        && x.fec_fin_rango.Equals(parametro.fec_fin_rango)
                                        && x.cod_moneda.Equals(parametro.cod_moneda)
                                        && x.num_tramo.Equals(parametro.num_tramo)
                                        && x.cod_tipo_temporalidad.Equals(parametro.cod_tipo_temporalidad)
                                        && x.ind_origen.Equals(parametro.ind_origen)));

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        private ValPar ObtenerValParBase(string producto, ValPar parametro, string usuario)
        {
            try
            {
                var ListaParametrosVigentes = ListarParametrosVigentesValPar(producto, parametro.cod_parametro, usuario);
                return ListaParametrosVigentes.Where(x => (x.cod_parametro.Equals(parametro.cod_parametro)
                                        && x.cod_moneda.Equals(parametro.cod_moneda)
                                        && x.num_tramo.Equals(parametro.num_tramo)
                                        && x.cod_tipo_temporalidad.Equals(parametro.cod_tipo_temporalidad)
                                        && x.ind_origen.Trim().Equals(parametro.ind_origen))).FirstOrDefault();

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        private bool ActualizarParametroBase(string endpoint, string producto, ValPar parametro, string usuario)
        {
            var parametroBase = ObtenerValParBase(producto, parametro, usuario);
            var parametroBaseActualizado = parametroBase;

            parametroBaseActualizado.fec_fin_rango = parametro.fec_ini_rango.AddDays(-1).Date;
            parametroBaseActualizado.aud_usr_modificacion = usuario;
            parametroBaseActualizado.aud_fec_modificacion = DateTime.Now;

            return ActualizarParametrosValPar(endpoint, producto, parametroBase, parametroBase);
        }

        public Respuesta CotizarPublic(string numSolicitud, DateTime fechaCotizacion, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.Cotizar(numSolicitud, fechaCotizacion, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "OK";
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }
            return respuesta;
        }

        public RespuestaCotizacion CotizarPublicV2(string numSolicitud, DateTime fechaCotizacion, string usuario)
        {
            RespuestaCotizacion respuesta = new RespuestaCotizacion();
            ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
            respuesta = cotizadorServicio.ProcesarCotizacion(numSolicitud, fechaCotizacion, usuario);

            return respuesta;
        }

        public Respuesta RegistrarPolizaIFPADMWRPublic(string num_solicitud, string usuario, GrupoFamiliar grup_fam)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                long resultado = cotizadorServicio.RegistrarPolizaIFPADMWR(num_solicitud, usuario, grup_fam);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "OK";
                respuesta.Contenido = resultado.ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }

            return respuesta;
        }

        public Respuesta RegistrarPolizaADMWRPublic(string num_solicitud, string usuario, GrupoFamiliar grup_fam)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                long resultado = cotizadorServicio.RegistrarPolizaADMWR(num_solicitud, usuario, grup_fam);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = "OK";
                respuesta.Contenido = resultado.ToString();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
            }

            return respuesta;
        }

        public Respuesta actualizarPolizaSMEAdmwrPublic(string gls_poliza, int codigo_SME, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                log.Debug("Inicio envio servicioCotizador. Actualizar Campo SME Poliza en ADMWR");
                string mensaje = "";
                ICotizadorServicio cotizadorServicio = FabricaIoC.Contenedor.Resolver<ICotizadorServicio>();
                cotizadorServicio.ActualizarPolizaSME(gls_poliza, codigo_SME, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                respuesta.Mensaje = (mensaje == "") ? "Correctamente." : mensaje;
                log.Debug("Fin envio servicioCotizador. Actualizar Campo SME Poliza en ADMWR");
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
            }
            return respuesta;
        }

    }
}