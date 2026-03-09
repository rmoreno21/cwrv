using Interseguro.CWRV.Aplicacion.ModuloPrincipal;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Infraestructura.Transversal;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Web;

namespace Interseguro.CWRV.ServiciosAcceso
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class ServicioAccesoCWRV : IServicioAccesoCWRV
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ServicioAccesoCWRV));

        public Acceso SolicitarAcceso(string usuario, string cuspp)
        {
            Acceso acceso;
            try
            {
                // Validaciones básicas
                bool esValido = true;
                List<string> mensaje = new List<string>();

                // Usuario
                if (usuario == null || usuario.Trim().Length == 0)
                {
                    esValido = false;
                    mensaje.Add("No se ha especificado un usuario.");
                }

                // CUSPP
                //if (cuspp == null || cuspp.Trim().Length == 0)
                //{
                //    esValido = false;
                //    mensaje.Add("No se ha especificado un CUSPP.");
                //}

                // Verificar si pasa las validaciones
                if (!esValido)
                {
                    string mensajeError = String.Empty;
                    mensaje.ForEach(s => mensajeError += s + " ");
                    acceso = new Acceso
                    {
                        Codigo = -1,
                        Mensaje = mensajeError,
                        Token = null
                    };
                    return acceso;
                }

                // Obtener la dirección IP del cliente que consume el método
                OperationContext context = OperationContext.Current;
                MessageProperties prop = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = prop[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string ip = endpoint.Address;

                log.Debug(String.Format("El usuario [{0}] ha solicitado acceso al Cotizador Web de Rentas desde la siguiente IP [{1}] para consultar el siguiente CUSPP [{2}]", usuario, ip, cuspp));
                SolicitudAcceso solicitud = new SolicitudAcceso
                {
                    Usuario = usuario.ToLower(),
                    CUSPP = cuspp,
                    IP = ip
                };

                ISeguridadServicio seguridadServicio = FabricaIoC.Contenedor.Resolver<ISeguridadServicio>();
                acceso = seguridadServicio.SolicitarAcceso(solicitud);

                if (acceso.Codigo != 0)
                {
                    log.Error(acceso.Mensaje);
                }

                return acceso;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                acceso = new Acceso
                {
                    Codigo = -9,
                    Mensaje = ex.Message,
                    Token = null
                };
                return acceso;
            }
        }
    }
}