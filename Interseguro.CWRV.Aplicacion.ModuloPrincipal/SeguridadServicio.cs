using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Infraestructura.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Aplicacion.ModuloPrincipal
{
    public class SeguridadServicio: ISeguridadServicio
    {
        private readonly IRepositorioSolicitudAcceso repositorioSolicitudAcceso;
        private readonly IRepositorioIPPermitida repositorioIPPermitida;

        public SeguridadServicio(
            IRepositorioSolicitudAcceso repositorioSolicitudAcceso,
            IRepositorioIPPermitida repositorioIPPermitida
            )
        {
            this.repositorioSolicitudAcceso = repositorioSolicitudAcceso;
            this.repositorioIPPermitida = repositorioIPPermitida;
        }

        public Acceso SolicitarAcceso(SolicitudAcceso solicitud)
        {
            Acceso acceso;
            try
            {
                // Validar si la IP está dentro de las autorizadas
                IPPermitida ip = repositorioIPPermitida.ObtenerDatos(solicitud.IP);
                if (ip != null)
                {
                    // Generar el token aleatoriamente
                    solicitud.Token = Utilitarios.CadenaAleatoria(30);

                    // Colocarle la expiración según lo configurado para la IP del cliente del servicio
                    solicitud.ExpiracionToken = ip.SegundosExpiracion;

                    // Insertar la solicitud en la base de datos
                    repositorioSolicitudAcceso.Registrar(solicitud);

                    // Generar el objeto "Acceso" para ser devuelto al cliente
                    acceso = new Acceso
                    {
                        Codigo = 0,
                        Mensaje = "Solicitud de acceso aceptada.",
                        Token = solicitud.Token
                    };
                }
                else
                {
                    acceso = new Acceso
                    {
                        Codigo = -2,
                        Mensaje = "El Cotizador Web de Rentas le ha denegado el acceso. Por favor comuníquese con Soporte.",
                        Token = null
                    };
                }
                return acceso;
            }
            catch (Exception ex)
            {
                acceso = new Acceso
                {
                    Codigo = -9,
                    Mensaje = ex.Message,
                    Token = null
                };
            }
            return acceso;
        }

        public SolicitudAcceso ValidarToken(string token, string ip)
        {
            // Validar si la IP está dentro de las autorizadas
            IPPermitida vIP = repositorioIPPermitida.ObtenerDatos(ip);
            if (vIP != null)
            {
                var solicitud = repositorioSolicitudAcceso.ValidarToken(token);
                return solicitud;
            }
            else
            {
                throw new Exception(String.Format("La IP [{0}] no está autorizada para usar este método.", ip));
            }
        }

        public void ActualizarSolicitudAcceso(SolicitudAcceso solicitud)
        {
            repositorioSolicitudAcceso.Actualizar(solicitud);
        }
    }
}
