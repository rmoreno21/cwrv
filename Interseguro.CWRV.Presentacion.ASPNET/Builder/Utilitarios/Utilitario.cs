using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloSeguridad;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios
{
    public class Utilitario
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Utilitario));
        private static IServicioCWRV servicioCotizador;
        private static IServicioAzman servicioAzman;

        public static bool ValidarVisita(List<Parametro> listaParametro, string numAgente, string cusspp, DateTime fechaPlazoAFP)
        {
            if (ConfigurationManager.AppSettings["ValidacionesCRM"] == "S")
            {
                bool resultado = true;

                DateTime? fechaEstadoCita;

                // Filtramos parametros de Visita Cita
                List<Parametro> listaParametroVisitaCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.VisitaCita.StringValue()).ToList();

                // Filtramos parametros de Estados Cita para listar las citas del CRM
                List<Parametro> listaParametroEstadosCita = listaParametro.Where(x => x.Id == Enums.ParametroTabla.EstadoCita.StringValue()).ToList();

                // Llenamos parámetros para consultar las citas
                Cita cita = new Cita
                {
                    Agente = new Agente { Id = numAgente },
                    Afiliado = new Afiliado { CUSPP = cusspp },
                    Parametro = listaParametroEstadosCita
                };

                //Obtenemos datos de citas, el cual viene en ordenada por fecha de estado descendente
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                List<Cita> listaCita = servicioCotizador.ListarCita(cita);

                if (listaCita.Count > 0)
                {
                    // Obtenemos la cita con mayor fecha (viene ordenado desde stored procedure)
                    Cita citaTemp = listaCita.First();

                    // Validamos si es que tiene cita, caso contrario el EstadoCita llegará como null
                    if (citaTemp.EstadoCita != null)
                    {
                        // Capturamos la fecha de estado de la cita
                        fechaEstadoCita = citaTemp.EstadoCita.FecEstadoUsuario;

                        // Le quitamos las horas, minutos y segundos a la fecha, ya que debe ser comparada a mivel de día
                        fechaEstadoCita = new DateTime(fechaEstadoCita.Value.Year, fechaEstadoCita.Value.Month, fechaEstadoCita.Value.Day);

                        //Capturamos el parametro limite inicio
                        Parametro parametroVisitaCitaInicio = listaParametroVisitaCita.Where(x => x.Nombre == Enums.Parametro.VisitaCitaInicio.StringValue()).First();

                        //Capturamos el parametro limite fin
                        Parametro parametroVisitaCitaFin = listaParametroVisitaCita.Where(x => x.Nombre == Enums.Parametro.VisitaCitaFin.StringValue()).First();

                        //validamos que este en el rango de plazos de la Fecha AFP
                        if (!(fechaPlazoAFP.AddDays(-1 * Convert.ToInt32(parametroVisitaCitaInicio.Valor_1)) <= fechaEstadoCita
                            && fechaPlazoAFP.AddDays(Convert.ToInt32(parametroVisitaCitaFin.Valor_1)) >= fechaEstadoCita))
                        {
                            // La última cita no está dentro del rango de fechas requerido
                            resultado = false;
                        }
                    }
                    else
                    {
                        // No tiene programada ninguna cita en el CRM
                        resultado = false;
                    }
                }
                else
                {
                    // Información insuficiente desde el CRM
                    resultado = false;
                }

                return resultado;
            }
            else
            {
                // Validaciones del CRM desactivadas
                return true;
            }
        }

        public static bool ValidarPreCubo(List<Parametro> listaParametro, string numAgente, string cuspp)
        {
            if (ConfigurationManager.AppSettings["ValidacionesSDA"] == "S")
            {
                bool resultado = true;

                //obtenemos datos de precubo
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                SDAReporte preCubo = servicioCotizador.obtenerPreCubo(Convert.ToInt32(numAgente), cuspp);

                //filtramos parametros de cubo
                List<Parametro> listaParametroCubo = listaParametro.Where(x => x.Id == Enums.ParametroTabla.Cubo.StringValue() && x.Valor_1 == preCubo.Precubo).ToList();

                //validamos si el agente cumple con el precubo
                if (listaParametroCubo.Count == 0)
                {
                    resultado = false;
                }

                return resultado;
            }
            else
            {
                return true;
            }
        }

        public static bool ValidarParametrosCorreo(List<Parametro> listaParametro, string correo)
        {
            if (ConfigurationManager.AppSettings["ValidacionesCorreo"] == "S")
            {
                bool resultado = true;
                bool bolCorreo = true;

                List<Parametro> parametroCorreo = new List<Parametro>();

                //capturar parametros correo
                parametroCorreo = listaParametro.Where(x => x.Id == Enums.ParametroTabla.Correo.StringValue()).ToList();

                for (int i = 0; i < parametroCorreo.Count; i++)
                {
                    bolCorreo = correo.Contains(parametroCorreo[i].Valor_1);

                    if (!bolCorreo)
                    {
                        break;
                    }
                }

                resultado = bolCorreo;

                return resultado;
            }
            else
            {
                return true;
            }
        }

        public static Respuesta ValidarSolicitud(List<Parametro> listaParametro, string cuspp, DateTime fechaPlazoAFP, string correo, string rol, string numAgenteSol)
        {
            Respuesta respuesta = new Respuesta();

            List<string> errores = new List<string>();

            string numAgente = numAgenteSol;

            if (errores.Count == 0)
            {
                respuesta.Estado = Constante.COD_OK;
            }
            else
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Interseguro.CWRV.Infraestructura.General.Utilitarios.FormatearError(errores);
            }

            return respuesta;
        }

        public static Respuesta RegistrarCotizacionMovimiento(ref List<Cotizacion> listaCotizaciones, Solicitud solicitud,
                                                              Boolean rechazo, Boolean primerMovimiento, string url, string numElegida)
        {
            Respuesta respuesta = new Respuesta();
            bool bAprobar = false;
            try
            {
                Solicitud sol = new Solicitud();
                List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];
                string rol = (string)HttpContext.Current.Session["RolAzman"];
                string tokenUsuario = (string)HttpContext.Current.Session["TokenUsuario"];
                List<Usuario> listaUsuariosCorreo = null;
                short personaCorreo = 0;
                bool vAcom = true;
                bool vDtra = true;

                string evento = "";
                string validaCuotas = ConfigurationManager.AppSettings["ValidaCuotas"].ToString();

                respuesta = ValidarSolicitud(listaParametro, solicitud.Afiliado.CUSPP, Convert.ToDateTime(solicitud.FechaPlazoAFP), solicitud.Afiliado.CorreoElectronico, rol, solicitud.Agente.Id);

                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                if (respuesta.Estado == Constante.COD_OK)
                {
                    if (numElegida == "")
                        numElegida = "0";

                    // Jefe

                    List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                    Agente agente = listaAgentes.Find(a => a.Id == solicitud.Agente.Id);
                    if (agente == null)
                    {
                        throw new Exception("No existe el agente en la estructura de jerarquía");
                    }
                    if (agente.IdPadre.ToString() == "")
                    {
                        throw new Exception("El agente " + agente.Nombre.ToString() + " no cuenta con supervisor asignado");
                    }
                    Agente agenteSupervisor = listaAgentes.Find(a => a.Id == agente.IdPadre.ToString());
                    if (agenteSupervisor == null)
                    {
                        throw new Exception("El supervisor del agente no existe en la estructura.");
                    }
                    if (agenteSupervisor.IdPadre.ToString() == "")
                    {
                        throw new Exception("El agente " + agente.Nombre.ToString() + " no cuenta con jefe asignado");
                    }
                    Agente agenteJefe = listaAgentes.Find(a => a.Id == agenteSupervisor.IdPadre.ToString());
                    if (agenteJefe == null)
                    {
                        throw new Exception("El jefe del agente no existe en la estructura.");
                    }

                    if (solicitud.PorcentajeAumentoComision > 0)
                    {
                        if (!ValidarAgenteDeuda(solicitud.Agente.Id))
                        {
                            throw new Exception("No puede solicitar <strong>Pje ACOM</strong> porque tiene deuda pendiente.");
                        }
                    }

                    DateTime fecCotizacion;
                    List<FlujoMovimiento> lstFlujo = null;
                    solicitud.Cotizaciones = listaCotizaciones;
                    respuesta = servicioCotizador.CotizarDifTRA(ref solicitud, tokenUsuario);
                    if (respuesta.Estado == Constante.COD_ERROR)
                    {
                        respuesta.Estado = Constante.COD_ERROR;
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                        respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                        respuesta.Mensaje = respuesta.Mensaje;
                        return respuesta;
                    }

                    listaCotizaciones = solicitud.Cotizaciones;

                    if (!rechazo)
                    {
                        Parametro parametroTRA = listaParametro.Where(x => x.Id == "TRA").Distinct().First();

                        // Validar ACOM
                        RolAcom rolAcom = new RolAcom
                        {
                            CodRol = rol,
                            FechaCotizacion = Convert.ToDateTime(solicitud.FechaCotizacion),
                        };


                        List<RolAcom> listaRolAcom = servicioCotizador.ListarRolAcom(rolAcom);

                        if (listaRolAcom.Count > 0)
                        {
                            if (solicitud.PorcentajeAumentoComision != 0 && (solicitud.PorcentajeAumentoComision < listaRolAcom[0].NumRangoIni || solicitud.PorcentajeAumentoComision > listaRolAcom[0].NumRangoFin))
                            {
                                vAcom = false;
                            }
                        }

                        listaCotizaciones = listaCotizaciones.OrderBy(x => x.AjusteTRA).ToList();

                        // Validar DTRA
                        RolDtra rolDtra = new RolDtra
                        {
                            RolAzman = rol,
                            FechaCotizacion = Convert.ToDateTime(solicitud.FechaCotizacion),
                        };
                        List<RolDtra> listaRolDtra = servicioCotizador.ListarRolDtra(rolDtra);


                        if (listaRolDtra.Count > 0)
                        {
                            vDtra = true;
                            // Obtener el TRA mínimo
                            double tramin = Convert.ToDouble(listaCotizaciones.OrderBy(x => x.AjusteTRA).ToList().First().AjusteTRA);
                            if (tramin < listaRolDtra[0].RangoInicial || tramin > listaRolDtra[0].RangoFinal)
                            {
                                vDtra = false;
                            }

                            // Obtener el TRA máximo
                            double tramax = Convert.ToDouble(listaCotizaciones.OrderByDescending(x => x.AjusteTRA).ToList().First().AjusteTRA);
                            if (tramax < listaRolDtra[0].RangoInicial || tramax > listaRolDtra[0].RangoFinal)
                            {
                                vDtra = false;
                            }
                        }

                        if (vDtra & vAcom)
                        {
                            fecCotizacion = Convert.ToDateTime(solicitud.FechaCotizacion);
                            evento = "APROBACIÓN";
                            lstFlujo = servicioCotizador.ObtenerFlujos(fecCotizacion, "A", rol);

                            if (lstFlujo == null)
                            {
                                throw new Exception("Ud. no cuenta con permiso para continuar con el flujo de [" + evento + "] <br /> Solicitud Nro. " + solicitud.Id);
                            }
                            if (lstFlujo.Count == 0)
                            {
                                throw new Exception("Ud. no cuenta con permiso para continuar con el flujo de [" + evento + "] <br /> Solicitud Nro. " + solicitud.Id);
                            }

                            if (!servicioCotizador.ValidaFlujoSolicitudRol(solicitud.Id, rol, "A"))
                            {
                                throw new Exception("La Solicitud Nro. <strong>" + solicitud.Id + "</strong> ya se encuentra en otro estado.<br /> Cancele y vuelva a cargar la solicitud");
                            }

                            bAprobar = true;

                            sol.Respuesta = new Respuesta { Estado = Constante.COD_OK };

                            solicitud.Cotizaciones
                                        .ForEach(c =>
                                        {
                                            if (c.IndCotiza == "**")
                                            {
                                                bAprobar = false;

                                                evento = "CONSENTIMIENTO DE SOLICITUD";
                                                lstFlujo = servicioCotizador.ObtenerFlujos(fecCotizacion, "E", rol);

                                                if (lstFlujo == null)
                                                {
                                                    throw new Exception("Ha ingresado parámetros no permitidos en la solicitud. Envío a aprobación.");
                                                }
                                                if (lstFlujo.Count == 0)
                                                {
                                                    throw new Exception("Ha ingresado parámetros no permitidos en la solicitud. Envío a aprobación.");
                                                }

                                                if (!servicioCotizador.ValidaFlujoSolicitudRol(solicitud.Id, rol, "E"))
                                                {
                                                    throw new Exception("La Solicitud Nro. <strong>" + solicitud.Id + "</strong> ya se encuentra en otro estado.<br /> Cancele y vuelva a cargar la solicitud");
                                                }

                                            }
                                        });
                        }
                        else
                        {
                            if (rol == Enums.RolAzman.JefeOperaciones.StringValue() || rol == Enums.RolAzman.AsistenteOperaciones.StringValue())
                            {
                                if (!vDtra && !vAcom)
                                {
                                    throw new Exception("El porcentaje de <strong>Dif.TRA</strong> y <strong>ACOM</strong> se encuentra fuera del rango permitido, para la solicitud Nro. " + solicitud.Id);
                                }
                                if (!vDtra)
                                {
                                    throw new Exception("El porcentaje de <strong>Dif.TRA</strong> se encuentra fuera del rango permitido, para la solicitud Nro. " + solicitud.Id);
                                }
                                if (!vAcom)
                                {
                                    throw new Exception("El porcentaje de <strong>ACOM</strong> se encuentra fuera del rango permitido, para la solicitud Nro. " + solicitud.Id);
                                }
                            }
                            else
                            {
                                sol.Respuesta = respuesta;
                            }

                        }
                    }
                    else
                    {
                        sol.Respuesta = respuesta;
                    }

                    if (sol.Respuesta.Estado == Constante.COD_OK)
                    {
                        string XML_CotizacionMovimiento = "";

                        bool vAsistenteACOM = true;
                        bool vAsistenteTRA = true;

                        fecCotizacion = Convert.ToDateTime(solicitud.FechaCotizacion);

                        if (bAprobar)
                        {
                            evento = "APROBACIÓN";
                            lstFlujo = servicioCotizador.ObtenerFlujos(fecCotizacion, "A", rol);
                        }
                        else
                        {
                            if (primerMovimiento)
                            {
                                evento = "CONSENTIMIENTO DE SOLICITUD";
                                lstFlujo = servicioCotizador.ObtenerFlujos(fecCotizacion, "E", rol);

                                if (!servicioCotizador.ValidaFlujoSolicitudRol(solicitud.Id, rol, "E"))
                                {
                                    throw new Exception("La Solicitud Nro. <strong>" + solicitud.Id + "</strong> ya se encuentra en otro estado.<br /> Cancele y vuelva a cargar la solicitud");
                                }
                            }
                            else
                            {
                                if (rechazo)
                                {
                                    evento = "RECHAZO";
                                    lstFlujo = servicioCotizador.ObtenerFlujos(fecCotizacion, "R", rol);

                                    if (!servicioCotizador.ValidaFlujoSolicitudRol(solicitud.Id, rol, "R"))
                                    {
                                        throw new Exception("La Solicitud Nro. <strong>" + solicitud.Id + "</strong> ya se encuentra en otro estado.<br /> Cancele y vuelva a cargar la solicitud");
                                    }
                                }
                                else
                                {
                                    evento = "CONSENTIMIENTO DE SOLICITUD";
                                    lstFlujo = servicioCotizador.ObtenerFlujos(fecCotizacion, "E", rol);

                                    if (!servicioCotizador.ValidaFlujoSolicitudRol(solicitud.Id, rol, "E"))
                                    {
                                        throw new Exception("La Solicitud Nro. <strong>" + solicitud.Id + "</strong> ya se encuentra en otro estado.<br /> Cancele y vuelva a cargar la solicitud");
                                    }
                                }
                            }
                        }

                        foreach (Cotizacion cotizacion in listaCotizaciones)
                        {
                            XML_CotizacionMovimiento += "<Movimiento>";
                            XML_CotizacionMovimiento += " <num_solicitud>" + solicitud.Id + "</num_solicitud>";
                            XML_CotizacionMovimiento += " <fec_cotizacion>" + Convert.ToDateTime(solicitud.FechaCotizacion).ToString("yyyyMMdd") + "</fec_cotizacion>";
                            XML_CotizacionMovimiento += " <num_correlativo>" + cotizacion.Correlativo + "</num_correlativo>";
                            XML_CotizacionMovimiento += " <val_acom>" + solicitud.PorcentajeAumentoComision + "</val_acom>";

                            //Enviando al XML
                            if (lstFlujo != null)
                            {
                                if (lstFlujo.Count > 0)
                                {
                                    XML_CotizacionMovimiento += " <cod_tipo_movimiento>" + lstFlujo[0].Destino + "</cod_tipo_movimiento>";
                                    XML_CotizacionMovimiento += " <gls_movimiento>" + lstFlujo[0].MsjFlujo + "</gls_movimiento>";
                                }
                                else
                                {
                                    throw new Exception("Ud. no cuenta con permiso para continuar con el flujo de [" + evento + "] <br /> Solicitud Nro. " + solicitud.Id);
                                }
                            }
                            else
                            {
                                throw new Exception("Ud. no cuenta con permiso para continuar con el flujo de [" + evento + "] <br /> Solicitud Nro. " + solicitud.Id);
                            }

                            XML_CotizacionMovimiento += " <val_tasa_ajuste_tra>" + cotizacion.AjusteTRA + "</val_tasa_ajuste_tra>";

                            XML_CotizacionMovimiento += " <pbs>" + cotizacion.pbs + "</pbs>";
                            XML_CotizacionMovimiento += " <val_tasa_int_vit>" + cotizacion.TasaVentaSbsObjetivo + "</val_tasa_int_vit>";
                            XML_CotizacionMovimiento += " <val_tasa_ret_accion>" + cotizacion.TasaRetornoAccionistaObjetivo + "</val_tasa_ret_accion>";
                            XML_CotizacionMovimiento += " <val_pen_cia>" + cotizacion.PensionCiaObjetivo + "</val_pen_cia>";
                            XML_CotizacionMovimiento += " <cod_rol>" + rol + "</cod_rol>";
                            XML_CotizacionMovimiento += " <valida_cuotas>" + validaCuotas + "</valida_cuotas>";

                            // Jefe
                            string idJefe = agenteJefe.Id;

                            XML_CotizacionMovimiento += " <num_agente_jefe>" + idJefe + "</num_agente_jefe>";

                            XML_CotizacionMovimiento += " <evento>" + evento + "</evento>";
                            XML_CotizacionMovimiento += " <val_dcom>" + solicitud.PorcentajeDescuentoComision + "</val_dcom>";
                            XML_CotizacionMovimiento += " <val_monto_acom>" + solicitud.MontoAumentoComision + "</val_monto_acom>";
                            XML_CotizacionMovimiento += " <num_cotizacion_elegida>" + numElegida + "</num_cotizacion_elegida>";
                            XML_CotizacionMovimiento += " <cod_compania>" + solicitud.Compania.Id + "</cod_compania>";
                            XML_CotizacionMovimiento += " <val_pen_cia_mo>" + cotizacion.PensionCiaMOObjetivo + "</val_pen_cia_mo>";
                            XML_CotizacionMovimiento += " <val_pen_afp>" + cotizacion.PensionAFPObjetivo + "</val_pen_afp>";
                            XML_CotizacionMovimiento += "</Movimiento>";
                        }

                        XML_CotizacionMovimiento = "<ROOT>" + XML_CotizacionMovimiento + "</ROOT>";

                        if (bAprobar)
                        {
                            respuesta.Mensaje = "Solicitud Aprobada Correctamente";
                            respuesta.Contenido = "APROBADO";
                        }
                        else if (rechazo)
                        {
                            respuesta.Mensaje = "Solicitud Rechazada Correctamente";
                            respuesta.Contenido = "RECHAZADO";
                        }
                        else
                        {
                            if (ConfigurationManager.AppSettings["ValidaCuotas"].ToString() == "S")
                            {
                                string codRol = (string)HttpContext.Current.Session["RolAzman"];

                                if (codRol == Enums.RolAzman.JefeVentaLima.StringValue() || codRol == Enums.RolAzman.JefeVentaProvincia.StringValue())
                                {
                                    // Jefe
                                    string idJefe = agenteJefe.Id;
                                    CuotasTra rolCuotas = new CuotasTra
                                    {
                                        FecCotizacion = solicitud.FechaCotizacion.Value,
                                        Agente = new Agente { Id = idJefe }
                                    };

                                    var rolCuotasTra = servicioCotizador.ObtenerCuotasTra(rolCuotas);
                                    if (rolCuotasTra != null)
                                    {
                                        if (rolCuotasTra.NroCasosTotal <= rolCuotasTra.NroCasosSolicitados)
                                        {
                                            throw new Exception("No puede exceder el Nro. de casos solicitados a la cuota asignada.");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Usted no tiene cuota asignada.");
                                    }
                                }
                            }
                            respuesta.Mensaje = "Solicitud Enviada a Aprobación Correctamente";
                            respuesta.Contenido = "ENVIADO";
                        }
                        respuesta.Contenido = lstFlujo[0].Destino + respuesta.Contenido;

                        sol = ModificarSolicitud(tokenUsuario, solicitud, XML_CotizacionMovimiento, ref respuesta);

                        if (sol.Respuesta.Estado == Constante.COD_OK)
                        {
                            if (bAprobar)
                            {
                                // Verificar si la solicitud no ha producido algún error de tasas
                                Respuesta resp = sol.Respuesta;
                                sol = servicioCotizador.ObtenerDatosSolicitud(sol.Id, (DateTime)sol.FechaCotizacion);
                                sol.Respuesta = resp;
                                foreach (Cotizacion cotizacion in sol.Cotizaciones)
                                {
                                    if (cotizacion.IndCotiza == "**" || cotizacion.IndCotiza == "***")
                                    {
                                        sol.Respuesta = new Respuesta();
                                        sol.Respuesta.Estado = "ERROR_TASA";
                                    }
                                }
                            }
                        }
                        else
                        {
                            respuesta = sol.Respuesta;
                        }
                    }
                    else
                    {
                        respuesta = sol.Respuesta;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Se ha producido el siguiente error [{0}]", ex.Message), ex);

                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Interseguro.CWRV.Infraestructura.General.Utilitarios.FormatearError(new List<string> { ex.Message });
            }

            return respuesta;
        }

        public static Respuesta EnviarEmail(List<SolicitudEscenario> solicitudes, string url, Respuesta rspta)
        {
            Respuesta respuesta = new Respuesta();
            string strSolicitudes = "";
            for (int i = 0; i < solicitudes.Count; i++)
            {
                strSolicitudes = strSolicitudes + solicitudes[i].NumSolicitud + ",";
            }
            strSolicitudes = strSolicitudes.PadLeft(strSolicitudes.Length - 1);
            log.Info(string.Format("Inicio Envio EnviarEmail, solicitud Nro. [{0}].", strSolicitudes));

            try
            {
                int personaCorreo = 0;
                List<Usuario> listaUsuariosCorreo = new List<Usuario>();
                string accion = "";
                if (ConfigurationManager.AppSettings["EnviarNotificaciones"] == "S")
                {
                    accion = rspta.Contenido.Substring(1, rspta.Contenido.Length - 1);

                    if (accion == "ENVIADO")
                    {
                        personaCorreo = Convert.ToInt32(rspta.Contenido.Substring(0, 1));
                    }
                    else
                    {
                        personaCorreo = 0;
                    }

                    string rol = (string)HttpContext.Current.Session["RolAzman"];
                    string tablaCotizacion = "";

                    tablaCotizacion += " <!DOCTYPE html>";
                    tablaCotizacion += " <html>";

                    tablaCotizacion += " <head>";

                    tablaCotizacion += " <style type='text/css'>";
                    tablaCotizacion += " .tg  {border-collapse:collapse;border-spacing:0;border-color:#aaa;}";
                    tablaCotizacion += " .tg td{font-family:Arial, sans-serif;font-size:10px;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#aaa;color:#333;background-color:#fff;}";
                    tablaCotizacion += " .tg th{font-family:Arial, sans-serif;font-size:10px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#aaa;color:#fff;background-color:#f38630;}";
                    tablaCotizacion += " .tg .tg-7ddf{background-color:#1d4e90;vertical-align:top}";
                    tablaCotizacion += " .tg .tg-yw4l{vertical-align:top}";
                    tablaCotizacion += " .tg .tg-right{vertical-align:top; text-align: right;}";
                    tablaCotizacion += " </style>";

                    tablaCotizacion += " </head>";

                    tablaCotizacion += " <body>";

                    tablaCotizacion += " <table class='tg'>";
                    tablaCotizacion += "  <tr>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Solicitud</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>CIA</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Mon.</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Categoría</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>CIC</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>AFP</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Mod</th>";

                    tablaCotizacion += "    <th class='tg-7ddf'>PG</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>PD</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>ACOM</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>DCOM</th>";


                    tablaCotizacion += "    <th class='tg-7ddf'>Dif. TRA</th>";

                    if (personaCorreo == 2)
                        tablaCotizacion += "    <th class='tg-7ddf'>Puntos Básicos</th>";

                    tablaCotizacion += "    <th class='tg-7ddf'>Pensión</th>";

                    tablaCotizacion += "    <th class='tg-7ddf'>Pensión Esperada</th>";

                    //4= llegara a GTE.DIV.RVI
                    if (personaCorreo == 2 || personaCorreo == 3 || personaCorreo == 4)
                        tablaCotizacion += "    <th class='tg-7ddf'>Tasa Vta.</th>";

                    //4= llegara a GTE.DIV.RVI
                    if (personaCorreo == 2 || personaCorreo == 3 || personaCorreo == 4)
                        tablaCotizacion += "    <th class='tg-7ddf'>Tasa Vta. Esperada</th>";


                    //4= llegara a GTE.DIV.RVI y 5 = JEF.OPERACIONES
                    if (personaCorreo == 3 || personaCorreo == 4 || personaCorreo == 5)
                    {
                        tablaCotizacion += "    <th class='tg-7ddf'>TIR</th>";
                    }


                    //5= JEF.OPERACIONES
                    if (personaCorreo == 3 || personaCorreo == 4 || personaCorreo == 5)
                        tablaCotizacion += "    <th class='tg-7ddf'>TIR Esperado</th>";

                    tablaCotizacion += "    <th class='tg-7ddf'>Agente</th>";

                    //5= JEF.OPERACIONES
                    if (personaCorreo == 3 || personaCorreo == 4 || personaCorreo == 5)
                        tablaCotizacion += "    <th class='tg-7ddf'>Supervisor</th>";

                    //5= JEF.OPERACIONES
                    if (personaCorreo == 4 || personaCorreo == 5)
                        tablaCotizacion += "    <th class='tg-7ddf'>Jefe</th>";

                    tablaCotizacion += "  </tr>";
                    foreach (var solicitud in solicitudes)
                    {
                        tablaCotizacion += "  <tr>";
                        tablaCotizacion += "    <td class='tg-yw4l'>" + solicitud.NumSolicitud + "</td>";
                        tablaCotizacion += "    <td class='tg-yw4l'>" + solicitud.Compania.Nombre + "</td>";
                        tablaCotizacion += "    <td class='tg-yw4l'>" + solicitud.Cotizaciones[0].Moneda.Nombre + "</td>";
                        tablaCotizacion += "    <td class='tg-yw4l'>" + solicitud.Categoria.Nombre + "</td>";
                        tablaCotizacion += "    <td class='tg-right'>" + solicitud.ValTotalCic.ToString("#,##0.00") + "</td>";
                        tablaCotizacion += "    <td class='tg-yw4l'>" + solicitud.AFP.Nombre + "</td>";
                        tablaCotizacion += "    <td class='tg-yw4l'>" + solicitud.Cotizaciones[0].Modalidad.Nombre + "</td>";

                        tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].PeriodoGarantizado + "</td>";
                        tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].PeriodoDiferido + "</td>";
                        tablaCotizacion += "    <td class='tg-right'>" + solicitud.PjeAumentoComision + "</td>";
                        tablaCotizacion += "    <td class='tg-right'>" + solicitud.CodPjeCesionComision + "</td>";

                        tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].AjusteTRA.Value.ToString("#,##0.00") + "</td>";

                        if (personaCorreo == 2)
                            tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].pbs.ToString() + "</td>";

                        if (solicitud.Cotizaciones[0].Modalidad.Nombre == Enums.Modalidad.Bimoneda.StringValue())
                        {
                            tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].PensionAFP.ToString("#,##0.00");
                            tablaCotizacion += "    <br/>" + solicitud.Cotizaciones[0].PensionCiaMO.ToString("#,##0.00");
                        }
                        else
                        {
                            tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].PensionCiaMO.ToString("#,##0.00");
                        }
                        tablaCotizacion += " </td>";

                        if (solicitud.Cotizaciones[0].Modalidad.Nombre == Enums.Modalidad.Bimoneda.StringValue())
                        {
                            tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].PensionAFPObjetivo.ToString("#,##0.00");
                            tablaCotizacion += "    <br/>" + solicitud.Cotizaciones[0].PensionCiaMOObjetivo.ToString("#,##0.00");
                        }
                        else
                        {
                            tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].PensionCiaMOObjetivo.ToString("#,##0.00");
                        }
                        tablaCotizacion += " </td>";

                        //4= llegara a GTE.DIV.RVI
                        if (personaCorreo == 2 || personaCorreo == 3 || personaCorreo == 4)
                            tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].TasaVentaSbs.ToString("#,##0.00") + "</td>";

                        //4= llegara a GTE.DIV.RVI
                        if (personaCorreo == 2 || personaCorreo == 3 || personaCorreo == 4)
                            tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].TasaVentaSbsObjetivo.ToString("#,##0.00") + "</td>";

                        //4= llegara a GTE.DIV.RVI Y 5=JEF.OPERACIONES
                        if (personaCorreo == 3 || personaCorreo == 4 || personaCorreo == 5)
                        {
                            tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].TasaRetornoAccionista.ToString("#,##0.00") + "</td>";
                        }


                        //5= JEF.OPERACIONES
                        if (personaCorreo == 3 || personaCorreo == 4 || personaCorreo == 5)
                            tablaCotizacion += "    <td class='tg-right'>" + solicitud.Cotizaciones[0].TasaRetornoAccionistaObjetivo.ToString("#,##0.00") + "</td>";

                        tablaCotizacion += "    <td class='tg-yw4l'>" + solicitud.Agente.Nombre + "</td>";

                        //5= JEF.OPERACIONES
                        if (personaCorreo == 3 || personaCorreo == 4 || personaCorreo == 5)
                            tablaCotizacion += "    <td class='tg-yw4l'>" + solicitud.Supervision.Supervisor + "</td>";

                        //5= JEF.OPERACIONES
                        if (personaCorreo == 4 || personaCorreo == 5)
                            tablaCotizacion += "    <td class='tg-yw4l'>" + solicitud.Supervision.Jefe + "</td>";

                        tablaCotizacion += "  </tr>";

                        /////////////////////////////////////////////////////////////////////////////////
                        List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                        // Enviar un correo electrónico a cada uno de los usuarios detallados en el parámetro
                        // 1. Obtener el o los usuarios a los que le tiene que llegar el correo
                        if (personaCorreo == 0)//1
                        {
                            // Agente
                            string cod_username = listaAgentes.Find(a => a.Id == solicitud.Agente.Id).Usuario;
                            if (cod_username != "")
                            {
                                listaUsuariosCorreo.Add(new Usuario { NombreUsuario = cod_username });
                            }

                            //Marcamos para luego obtener el nombre,ID=1
                            for (int fil = 0; fil < listaUsuariosCorreo.Count; fil++)
                            {
                                listaUsuariosCorreo[fil].Id = 1;
                            }
                        }
                        else if (personaCorreo == 1)//2
                        {
                            respuesta.Estado = Constante.COD_OK;
                            respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                            respuesta.Mensaje = "Correos enviados correctamente.";
                            return respuesta;
                        }
                        else if (personaCorreo == 2)//3
                        {
                            // Supervisor
                            string idSupervisor = listaAgentes.Find(a => a.Id == solicitud.Agente.Id).IdPadre;
                            string cod_username = listaAgentes.Find(a => a.Id == idSupervisor).Usuario.ToString();
                            if (cod_username.ToString() != "")
                            {
                                listaUsuariosCorreo.Add(new Usuario { NombreUsuario = cod_username });
                            }

                            //Marcamos para luego obtener el nombre,ID=1
                            for (int fil = 0; fil < listaUsuariosCorreo.Count; fil++)
                            {
                                listaUsuariosCorreo[fil].Id = 1;
                            }
                        }
                        else if (personaCorreo == 3)//4
                        {
                            // Jefe
                            string idSupervisor = listaAgentes.Find(a => a.Id == solicitud.Agente.Id).IdPadre;
                            string idJefe = listaAgentes.Find(a => a.Id == idSupervisor).IdPadre;
                            string cod_username = listaAgentes.Find(a => a.Id == idJefe).Usuario.ToString();
                            if (cod_username.ToString().Trim() != "")
                            {
                                listaUsuariosCorreo.Add(new Usuario { NombreUsuario = cod_username });
                            }

                            //Marcamos para luego obtener el nombre,ID=1
                            for (int fil = 0; fil < listaUsuariosCorreo.Count; fil++)
                            {
                                listaUsuariosCorreo[fil].Id = 1;
                            }
                        }
                        else if (personaCorreo == 4)//5
                        {
                            // Gerente
                            listaUsuariosCorreo = new List<Usuario>();
                            if (ConfigurationManager.AppSettings["UsuarioGerenteComercial"] != string.Empty)
                            {
                                foreach (string usuario in ConfigurationManager.AppSettings["UsuarioGerenteComercial"].Split(','))
                                {
                                    //Marcamos para luego obtener el nombre,ID=1
                                    listaUsuariosCorreo.Add(new Usuario { NombreUsuario = usuario, Id = 1 });
                                }
                            }
                        }
                        else if (personaCorreo == 5)//6
                        {
                            // Jefe Operaciones
                            listaUsuariosCorreo = new List<Usuario>();
                            if (ConfigurationManager.AppSettings["UsuarioJefeOperaciones"] != string.Empty)
                            {
                                foreach (string usuario in ConfigurationManager.AppSettings["UsuarioJefeOperaciones"].Split(','))
                                {
                                    //Marcamos para luego obtener el nombre,ID=1
                                    listaUsuariosCorreo.Add(new Usuario { NombreUsuario = usuario, Id = 1 });
                                }
                            }
                        }

                        // Agregar los usuarios genéricos
                        if (ConfigurationManager.AppSettings["UsuariosNotificaciones"] != string.Empty)
                        {
                            if (listaUsuariosCorreo.Count > 0)
                            {
                                foreach (string usuario in ConfigurationManager.AppSettings["UsuariosNotificaciones"].Split(','))
                                {
                                    listaUsuariosCorreo.Add(new Usuario { NombreUsuario = usuario });
                                }
                            }
                        }
                        /////////////////////////////////////////////////////////////////////////////////
                    }
                    tablaCotizacion += " </table>";

                    tablaCotizacion += " </body>";
                    tablaCotizacion += " </html>";

                    // Envío de notificaciones vía Correo Electrónico
                    CorreoElectronico correo = null;
                    int i = 0;
                    string nombreCompleto = "";
                    string nro_solicitud = "";

                    if (solicitudes.Count == 1)
                    {
                        nro_solicitud = solicitudes[0].NumSolicitud;
                    }

                    foreach (Usuario usuarioCorreo in listaUsuariosCorreo)
                    {
                        try
                        {
                            //2. Obtener los datos de la persona
                            servicioAzman = LocalizadorProxy.ObtenerServicioSeguridad();
                            BEUsuario datosUsuario =
                                servicioAzman.ObtenerDatosUsuarioSinClave(
                                    ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                    ConfigurationManager.AppSettings["DominioRed"],
                                    usuarioCorreo.NombreUsuario);

                            //4. Crear el correo electrónico
                            SeccionCorreo config = (SeccionCorreo)ConfigurationManager.GetSection("correo");
                            //Solo Aquellos que tengan ID=1, obtenemo el nombre
                            if (usuarioCorreo.Id == 1)
                            {
                                nombreCompleto = datosUsuario.Nombres + " " + datosUsuario.Apellidos;
                            }

                            log.Info(string.Format("Envío de Email al Correo [{0}], Tipo de Flujo [{1}].", datosUsuario.Correo, accion));

                            correo = new CorreoElectronico
                            {
                                De = "no-responder@interseguro.com.pe",
                                DeNombre = "Interseguro - Cotizador Web de Rentas ",
                                Para = datosUsuario.Correo, //"roberto.alegre@interseguro.com.pe", //
                                ParaNombre = datosUsuario.NombreCompleto,
                                Asunto = config.AsuntoParametrosEspeciales.Texto
                                                .Replace("{NroSolicitud}", nro_solicitud),
                                Mensaje = config.MensajeParametrosEspeciales.Texto
                                                .Replace("{Nombre}", nombreCompleto)
                                                .Replace("{InformeA}", tablaCotizacion)
                                                .Replace("{InformeB}", "")//Comentar
                                                .Replace("{UrlSolicitudes}", url),
                                Html = true
                            };

                            if (accion == "APROBADO")
                            {
                                correo.Mensaje = "<div style=\"color: #000; font-family:Arial; font-size: 10pt\">";
                                correo.Mensaje += "<img title=\"Interseguro\" alt=\"Interseguro\" src=\"https://app.interseguro.com.pe/CWRV/Imagenes/logo_is1.png\" /><br />";
                                correo.Mensaje += "<p style=\"color:#0060a9; font-size:16pt; margin: 0\">Cotizador Web de Rentas </p>";
                                correo.Mensaje += "<p>Su solicitud de parámetros especiales para el Escenario " + solicitudes[0].NumSolicitud + " ha sido aprobada.</p>";
                                correo.Mensaje += "<p>Atentamente,<br />Interseguro - Cotizador Web de Rentas </p></div>";
                            }

                            if (accion == "RECHAZADO")
                            {
                                correo.Mensaje = "<div style=\"color: #000; font-family:Arial; font-size: 10pt\">";
                                correo.Mensaje += "<img title=\"Interseguro\" alt=\"Interseguro\" src=\"https://app.interseguro.com.pe/CWRV/Imagenes/logo_is1.png\" /><br />";
                                correo.Mensaje += "<p style=\"color:#0060a9; font-size:16pt; margin: 0\">Cotizador Web de Rentas </p>";
                                correo.Mensaje += "<p>Su solicitud de parámetros especiales para el Escenario " + solicitudes[0].NumSolicitud + " ha sido rechazada.</p>";
                                correo.Mensaje += "<p>Atentamente,<br />Interseguro - Cotizador Web de Rentas </p></div>";
                                //correo.Mensaje += datosUsuario.Correo;//comentar
                            }

                            //5. Enviar el correo electrónico
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            servicioCotizador.EnviarCorreoElectronicoAsincrono(correo);
                            respuesta.Estado = Constante.COD_OK;
                            respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                            respuesta.Mensaje = "Correos enviados correctamente.";

                            i += 1;
                        }
                        catch (Exception ex)
                        {
                            log.Error(string.Format("Error al enviar el correo de notificación. Usuario[{0}]", usuarioCorreo.NombreUsuario), ex);
                        }
                    }
                }
            }
            catch (Exception ex1)
            {
                log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex1.Message), ex1);
            }
            log.Info(string.Format("Fin Envio EnviarEmail, solicitud Nro. [{0}].", strSolicitudes));
            return respuesta;
        }

        public static Solicitud ModificarSolicitud(string tokenUsuario, Solicitud solicitud, string XML_CotizacionMovimiento, ref Respuesta rpta)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Infraestructura.General.Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudActualizar))
                        {
                            List<string> errores = new List<string>();
                            List<string> controles = new List<string>();

                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == solicitud.Agente.Id))
                            {
                                if (!Infraestructura.General.Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                                {
                                    solicitud.PorcentajeAumentoComision = null;
                                    solicitud.PorcentajeDescuentoComision = null;
                                }

                                solicitud.Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"], Rol = (string)HttpContext.Current.Session["RolAzman"] };
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.RegistrarFlujoSolicitudCompleto(ref rpta, ref solicitud, XML_CotizacionMovimiento, solicitud.Usuario.NombreUsuario, solicitud.Usuario.Rol);

                                // Guardar en log de auditoría
                                string nombreTerminal = string.Empty;
                                try
                                {
                                    nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                }
                                catch (Exception)
                                {
                                    log.Warn(string.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                        HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                }

                                nombreTerminal += HttpContext.Current.Request.UserAgent;

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                    NombreUsuario = HttpContext.Current.Session["Usuario"].ToString(),
                                    IdTipoEvento = Enums.EventoLog.CotizarSolicitud.StringValue(),
                                    Detalle = "Solicitud modificada: " + solicitud.Id + ", ACOM: " + solicitud.PorcentajeAumentoComision + ", DCOM: " + solicitud.PorcentajeDescuentoComision
                                });
                            }
                            else
                            {
                                solicitud = new Solicitud();
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = Infraestructura.General.Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }
                        }
                        else
                        {
                            solicitud = new Solicitud();
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudActualizar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Interseguro.CWRV.Infraestructura.General.Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        solicitud = new Solicitud();
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }
                    solicitud.Respuesta = respuesta;
                    return solicitud;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error [{0}]", ex.Message), ex);
                    solicitud = new Solicitud();
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Interseguro.CWRV.Infraestructura.General.Utilitarios.FormatearError(new List<string> { ex.Message });
                    solicitud.Respuesta = respuesta;
                    return solicitud;
                }
            }
        }

        public static Respuesta ActualizaCotizacionMovimiento(Solicitud solicitud)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                string XML_CotizacionMovimiento = "";

                foreach (Cotizacion cotizacion in solicitud.Cotizaciones)
                {
                    XML_CotizacionMovimiento += "<Movimiento>";
                    XML_CotizacionMovimiento += " <num_solicitud>" + solicitud.Id + "</num_solicitud>";
                    XML_CotizacionMovimiento += " <num_correlativo>" + cotizacion.Correlativo + "</num_correlativo>";
                    XML_CotizacionMovimiento += " <cod_tipo_movimiento>" + solicitud.TipoMovimiento.Id + "</cod_tipo_movimiento>";
                    XML_CotizacionMovimiento += " <num_movimiento>" + cotizacion.NumMovimiento + "</num_movimiento>";

                    XML_CotizacionMovimiento += " <val_acom>" + solicitud.PorcentajeAumentoComision + "</val_acom>";
                    XML_CotizacionMovimiento += " <val_dcom>" + solicitud.PorcentajeDescuentoComision + "</val_dcom>";
                    XML_CotizacionMovimiento += " <val_tasa_ajuste_tra>" + cotizacion.AjusteTRA + "</val_tasa_ajuste_tra>";
                    XML_CotizacionMovimiento += " <pbs>" + cotizacion.pbs + "</pbs>";

                    XML_CotizacionMovimiento += " <val_tasa_int_vit>" + cotizacion.TasaVentaSbsObjetivo + "</val_tasa_int_vit>";
                    XML_CotizacionMovimiento += " <val_tasa_ret_accion>" + cotizacion.TasaRetornoAccionistaObjetivo + "</val_tasa_ret_accion>";
                    XML_CotizacionMovimiento += " <val_pen_cia>" + cotizacion.PensionCiaObjetivo + "</val_pen_cia>";
                    XML_CotizacionMovimiento += " <val_monto_acom>" + solicitud.MontoAumentoComision + "</val_monto_acom>";

                    XML_CotizacionMovimiento += " <val_pen_cia_mo>" + cotizacion.PensionCiaMOObjetivo + "</val_pen_cia_mo>";
                    XML_CotizacionMovimiento += " <val_pen_afp>" + cotizacion.PensionAFPObjetivo + "</val_pen_afp>";

                    XML_CotizacionMovimiento += "</Movimiento>";
                }

                XML_CotizacionMovimiento = "<ROOT>" + XML_CotizacionMovimiento + "</ROOT>";

                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                respuesta = servicioCotizador.ActualizarCotizacionMovimiento(XML_CotizacionMovimiento, (string)HttpContext.Current.Session["Usuario"]);

            }
            catch (Exception ex)
            {
                log.Error(string.Format("Se ha producido el siguiente error [{0}]", ex.Message), ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Interseguro.CWRV.Infraestructura.General.Utilitarios.FormatearError(new List<string> { ex.Message });
            }

            return respuesta;
        }

        public static bool ValidarAgenteDeuda(string idAgente)
        {
            bool valida = true;
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Iniciar instancia al Web Service
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<Agente> lstAgente = new List<Agente>();
                    lstAgente = servicioCotizador.ObtenerAgenteDeudaAcom(idAgente);
                    if (lstAgente.Count > 0)
                    {
                        valida = false;
                    }

                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                }

                return valida;
            }
        }
        //<FIN.GTI_7012_2>

        public static Respuesta EnviarEmailPolizaElectronica(string url, Respuesta rspta)
        {
            Respuesta respuesta = new Respuesta();
            log.Info(string.Format("Inicio Envio EnviarEmail, solicitud Nro. [{0}].", "strSolicitudes"));

            try
            {
                List<Usuario> listaUsuariosCorreo = new List<Usuario>();
                if (ConfigurationManager.AppSettings["EnviarNotificaciones"] == "S")
                {
                    string rol = (string)HttpContext.Current.Session["RolAzman"];
                    string tablaCotizacion = "";

                    tablaCotizacion += " <!DOCTYPE html>";
                    tablaCotizacion += " <html>";

                    tablaCotizacion += " <head>";

                    tablaCotizacion += " <style type='text/css'>";
                    tablaCotizacion += " .tg  {border-collapse:collapse;border-spacing:0;border-color:#aaa;}";
                    tablaCotizacion += " .tg td{font-family:Arial, sans-serif;font-size:10px;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#aaa;color:#333;background-color:#fff;}";
                    tablaCotizacion += " .tg th{font-family:Arial, sans-serif;font-size:10px;font-weight:normal;padding:10px 5px;border-style:solid;border-width:1px;overflow:hidden;word-break:normal;border-color:#aaa;color:#fff;background-color:#f38630;}";
                    tablaCotizacion += " .tg .tg-7ddf{background-color:#1d4e90;vertical-align:top}";
                    tablaCotizacion += " .tg .tg-yw4l{vertical-align:top}";
                    tablaCotizacion += " .tg .tg-right{vertical-align:top; text-align: right;}";
                    tablaCotizacion += " </style>";

                    tablaCotizacion += " </head>";

                    tablaCotizacion += " <body>";

                    tablaCotizacion += " <table class='tg'>";
                    tablaCotizacion += "  <tr>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Solicitud</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>CIA</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Mon.</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Categoría</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>CIC</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>AFP</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Mod</th>";

                    tablaCotizacion += "    <th class='tg-7ddf'>PG</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>PD</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>ACOM</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>DCOM</th>";

                    tablaCotizacion += "    <th class='tg-7ddf'>Dif. TRA</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Puntos Básicos</th>";

                    tablaCotizacion += "    <th class='tg-7ddf'>Pensión</th>";
                    tablaCotizacion += "    <th class='tg-7ddf'>Pensión Esperada</th>";

                    tablaCotizacion += "    <th class='tg-7ddf'>Agente</th>";

                    tablaCotizacion += "  </tr>";

                    tablaCotizacion += "  <tr>";
                    tablaCotizacion += "    <td class='tg-yw4l'>" + "solicitud.NumSolicitud" + "</td>";
                    tablaCotizacion += " </td>";
                    tablaCotizacion += " </td>";

                    tablaCotizacion += "  </tr>";

                    /////////////////////////////////////////////////////////////////////////////////
                    List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                    // Enviar un correo electrónico a cada uno de los usuarios detallados en el parámetro
                    // 1. Obtener el o los usuarios a los que le tiene que llegar el correo

                    // Agregar los usuarios genéricos
                    if (ConfigurationManager.AppSettings["UsuariosNotificaciones"] != string.Empty)
                    {
                        if (listaUsuariosCorreo.Count > 0)
                        {
                            foreach (string usuario in ConfigurationManager.AppSettings["UsuariosNotificaciones"].Split(','))
                            {
                                listaUsuariosCorreo.Add(new Usuario { NombreUsuario = usuario });
                            }
                        }
                    }

                    tablaCotizacion += " </table>";

                    tablaCotizacion += " </body>";
                    tablaCotizacion += " </html>";

                    // Envío de notificaciones vía Correo Electrónico
                    CorreoElectronico correo = null;
                    int i = 0;
                    string nombreCompleto = "";
                    string nro_solicitud = "45878";

                    try
                    {
                        //4. Crear el correo electrónico
                        SeccionCorreo config = (SeccionCorreo)ConfigurationManager.GetSection("correo");

                        log.Info(string.Format("Envío de Email al Correo [{0}], Tipo de Flujo [{1}].", "datosUsuario.Correo", "accion"));

                        correo = new CorreoElectronico
                        {
                            De = "no-responder@interseguro.com.pe",
                            DeNombre = "Interseguro - Cotizador Web de Rentas ",
                            Para = "wcervantes@iqproject.pe", //"roberto.alegre@interseguro.com.pe", //
                            ParaNombre = "Alex Cervantes",
                            Asunto = config.AsuntoParametrosEspeciales.Texto.Replace("{NroSolicitud}", nro_solicitud),
                            Mensaje = config.MensajeParametrosEspeciales.Texto.Replace("{Nombre}", nombreCompleto)
                                            .Replace("{InformeA}", tablaCotizacion)
                                            .Replace("{InformeB}", "")//Comentar
                                            .Replace("{UrlSolicitudes}", url),
                            Html = true
                        };

                        correo.Mensaje = "<div style=\"color: #000; font-family:Arial; font-size: 10pt\">";
                        correo.Mensaje += "<img title=\"Interseguro\" alt=\"Interseguro\" src=\"https://app.interseguro.com.pe/CWRV/Imagenes/logo_is1.png\" /><br />";
                        correo.Mensaje += "<p style=\"color:#0060a9; font-size:16pt; margin: 0\">Cotizador Web de Rentas </p>";
                        correo.Mensaje += "<p>Su solicitud de parámetros especiales para el Escenario " + "solicitudes[0].NumSolicitud" + " ha sido aprobada.</p>";
                        correo.Mensaje += "<p>Atentamente,<br />Interseguro - Cotizador Web de Rentas </p></div>";

                        //5. Enviar el correo electrónico
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        respuesta = servicioCotizador.EnviarCorreoElectronicoPoliza(correo);
                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                        respuesta.Mensaje = "Correos enviados correctamente.";

                        i += 1;
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Error al enviar el correo de notificación. Usuario[{0}]", "usuarioCorreo.NombreUsuario"), ex);
                    }
                }
            }
            catch (Exception ex1)
            {
                log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex1.Message), ex1);
            }
            log.Info(string.Format("Fin Envio EnviarEmail, solicitud Nro. [{0}].", "strSolicitudes"));
            return respuesta;
        }

        public static Respuesta EnviarDocumentoSME(DocumentoSME documento)
        {
            //Envio de manera Asincrona
            Respuesta respuesta = new Respuesta();
            try
            {
                string rutaServicio = string.Format("{0}/envios", ConfigurationManager.AppSettings["url_envio_documentos_sme"]);
                var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                string jsonString = JsonSerializar.Serialize(documento);

                log.Info("Consumiendo API de envío de documentos SME");
                log.Debug(string.Format("Request Body[{0}]", jsonString));
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    respuesta.Mensaje = client.UploadString(new Uri(rutaServicio), "POST", jsonString);
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

        public static Respuesta EnviarSMS(EnvioSMS envioSMS)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                string urlEnvioSms = ConfigurationManager.AppSettings["url_envio_sms"];

                string jsonEnvioSMS = JsonConvert.SerializeObject(envioSMS);

                log.Debug($"Consumiendo endpoint urlEnvioSms | [POST] {urlEnvioSms}");
                log.Debug($"jsonEnvioSMS | [BODY] {jsonEnvioSMS}");

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    respuesta.Mensaje = client.UploadString(new Uri(urlEnvioSms), "POST", jsonEnvioSMS);
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

        public static Respuesta GenerarSubirEdN(string json_generado_EdN)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                string rutaServicio = string.Format("{0}{1}", ConfigurationManager.AppSettings["estudio_necesidades_api"], ConfigurationManager.AppSettings["estudio_necesidades_api_subir"]);
                var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                //string jsonString = JsonSerializar.Serialize(json_generado_EdN);

                log.Debug("Consumiendo endpoint: " + rutaServicio);
                //log.Debug(string.Format("Request Body[{0}]", jsonString));
                log.Debug(string.Format("Request Body[{0}]", json_generado_EdN));

                string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["username_estudio_necesidades_api"] + ":" + ConfigurationManager.AppSettings["password_estudio_necesidades_api"]));
                ////client.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");

                    client.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", credentials));

                    respuesta.Mensaje = client.UploadString(new Uri(rutaServicio), "POST", json_generado_EdN);
                    respuesta.Estado = Constante.COD_OK;

                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
            }
            return respuesta;
        }

        public static EstudioNecesidadCWRV ConsultarEstudioNecesidadCWRV(DateTime fechaSolicitud, string usuario)
        {
            try
            {
                using (var client = new WebClient())
                {
                    string rutaServicio = string.Format("{0}{1}", ConfigurationManager.AppSettings["url_base_api_cwrv"], string.Format(ConfigurationManager.AppSettings["cwrv_api_estudio_necesidades_consultar"], usuario));

                    List<EstudioNecesidadCWRV> listaEstudioNecesidad = new List<EstudioNecesidadCWRV>();
                    EstudioNecesidadCWRV estudioNecesidad = null;
                    log.Debug("Consumiendo endpoint: " + rutaServicio);

                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(rutaServicio);
                    httpWebRequest.Method = "GET";

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        string responseBody = streamReader.ReadToEnd();
                        listaEstudioNecesidad = JsonConvert.DeserializeObject<List<EstudioNecesidadCWRV>>(responseBody);

                        estudioNecesidad = listaEstudioNecesidad.Find(en => fechaSolicitud >= en.fec_inicio_vigencia && fechaSolicitud <= en.fec_fin_vigencia);
                    }

                    return estudioNecesidad;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public static Respuesta ObtenerJSONEstudioNecesidadCWRV(EstudioNecesidadAPI estudioNecesidadAPI)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                string rutaServicio = string.Format("{0}{1}", ConfigurationManager.AppSettings["url_base_api_cwrv"], ConfigurationManager.AppSettings["cwrv_api_estudio_necesidades_obtener_JSON"]);
                var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                string jsonString = JsonSerializar.Serialize(estudioNecesidadAPI);

                log.Debug("Consumiendo endpoint: " + rutaServicio);
                log.Debug(string.Format("Request Body[{0}]", jsonString));

                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    respuesta.Mensaje = client.UploadString(new Uri(rutaServicio), "POST", jsonString);
                    respuesta.Estado = Constante.COD_OK;

                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
            }
            return respuesta;
        }

        public static Respuesta GenerarEstudioNecesidades(EstudioNecesidadAPI estudioNecesidadAPI, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            //**Crear y subir el EdN**//
            try
            {
                var respuestaConsultaApiCwrvEdN = ConsultarEstudioNecesidadCWRV(estudioNecesidadAPI.solicitud.fec_solicitud, usuario);

                if (respuestaConsultaApiCwrvEdN != null)
                {
                    estudioNecesidadAPI.codigo_plantilla = respuestaConsultaApiCwrvEdN.cod_formato.ToString();
                    var respuestaObtenerJSONApiCwrvEdN = ObtenerJSONEstudioNecesidadCWRV(estudioNecesidadAPI);

                    if (respuestaObtenerJSONApiCwrvEdN.Estado == Constante.COD_OK)
                    {
                        //string json_generado_EdN = " {\"producto\": \"RENTAS\", \"repositorioCS\": \"CRM\", \"carpetaCS\": \"02-01-45678909\", \"solicitud\": \"IFP_370213\", \"numerodocumento\": \"45678909\", \"cabeceras\": [ { \"campo\": \"CLIENTE\", \"valor\": \"ORLANDO ORTIZ DE FORONDA\" }, { \"campo\": \"N° DOCUMENTO\", \"valor\": \"45678909\" }, { \"campo\": \"FECHA\", \"valor\": \"26 /05/2022\" }, { \"campo\": \"N° SOLICITUD\", \"valor\": \"IFP_279070\" } ], \"preguntas\": [ { \"pregunta\": \"¿Cuánto es el monto de inversión que quiere realizar?\", \"respuesta\": \"50,000\" }, { \"pregunta\": \"¿En qué moneda quiere su inversión?\", \"respuesta\": \"Soles\" }, { \"pregunta\": \"¿Incluirá algún beneficiario?\", \"respuesta\": \"Sí, 3\" } ], \"cuadroDetalle\": [ [\"Cotizaciones|15%\", \"¿Cuál es el plazo de inversión que quiere elegir? (temporalidad)|30%\", \"¿Cuenta con alguna otra fuente de ingreso (diferimiento/ahorro)|30%\"], [\"3535240\", \"5\", \"No\"] ] } ";
                        string json_generado_EdN = respuestaObtenerJSONApiCwrvEdN.Mensaje;

                        var respuestaApiEdN = GenerarSubirEdN(json_generado_EdN);

                        if (respuestaApiEdN.Estado == Constante.COD_OK)
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            servicioCotizador.ActualizarEstudioNecesidadSolicitud(estudioNecesidadAPI.solicitud.num_solicitud, respuestaConsultaApiCwrvEdN.id_estudio_necesidades, usuario);
                            respuesta.Estado = Constante.COD_OK;
                        }
                        else
                        {
                            servicioCotizador.ActualizarEstudioNecesidadSolicitud(estudioNecesidadAPI.solicitud.num_solicitud, null, usuario);
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Mensaje = "Actualización del estudio de necesidades en null en la rvi_propue";
                        }
                    }
                    else
                    {
                        throw new Exception("Error al obtener el JSON del estudio de necesidades.");
                    }
                }
                else
                {
                    throw new Exception("No se encuentra o no existe el código del estudio de necesidades.");
                }

            }
            catch (Exception exEdN)
            {
                log.Error("Error: " + "Servicio api-estudioNecesidades " + "- " + exEdN.Message);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = exEdN.Message;
            }
            return respuesta;
        }

        public static string ObtenerEstudioNecesidades(string numSolicitud, string tipoIdentificacion, string numeroIdentificacion)
        {
            try
            {
                RespuestaEdNAPI resultado_url = new RespuestaEdNAPI();

                using (var client = new WebClient())
                {
                    string urlEdN = string.Format("{0}{1}", ConfigurationManager.AppSettings["estudio_necesidades_api"].ToString(), ConfigurationManager.AppSettings["estudio_necesidades_api_obtener"].ToString());
                    string usuarioEdNApi = ConfigurationManager.AppSettings["username_estudio_necesidades_api"].ToString();
                    string contrasenhaEdNApi = ConfigurationManager.AppSettings["password_estudio_necesidades_api"].ToString();

                    urlEdN = string.Format(urlEdN, ConfigurationManager.AppSettings["producto_cs"].ToString(), ConfigurationManager.AppSettings["repositorio_cs"].ToString(), ConfigurationManager.AppSettings["codigo_producto_cs"].ToString() + "-" + tipoIdentificacion + "-" + numeroIdentificacion, numSolicitud, numeroIdentificacion);
                    log.Info("Consumiendo endpoint: " + urlEdN);

                    string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuarioEdNApi + ":" + contrasenhaEdNApi));
                    //myWebClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";

                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(urlEdN);
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Headers.Add("Authorization", $"Basic {credentials}");

                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        string responseBody = streamReader.ReadToEnd();
                        resultado_url = JsonConvert.DeserializeObject<RespuestaEdNAPI>(responseBody);
                    }

                    return resultado_url.url;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public static string ObtenerEstudioNecesidadesLocal(EstudioNecesidadAPI estudioNecesidadAPI, string usuario)
        {
            try
            {
                string rutaServicio = string.Format("{0}{1}", ConfigurationManager.AppSettings["estudio_necesidades_api"], ConfigurationManager.AppSettings["estudio_necesidades_api_descargar_pdf"]);
                var respuesta_formato_edn = string.Empty;
                var respuestaConsultaApiCwrvEdN = ConsultarEstudioNecesidadCWRV(estudioNecesidadAPI.solicitud.fec_solicitud, usuario);

                if (respuestaConsultaApiCwrvEdN != null)
                {
                    estudioNecesidadAPI.codigo_plantilla = respuestaConsultaApiCwrvEdN.cod_formato.ToString();
                    var respuestaObtenerJSONApiCwrvEdN = ObtenerJSONEstudioNecesidadCWRV(estudioNecesidadAPI);

                    if (respuestaObtenerJSONApiCwrvEdN.Estado == Constante.COD_OK)
                    {

                        string json_generado_EdN = respuestaObtenerJSONApiCwrvEdN.Mensaje;

                        log.Debug("Consumiendo endpoint: " + rutaServicio);
                        log.Debug(string.Format("Request Body[{0}]", json_generado_EdN));

                        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["username_estudio_necesidades_api"] + ":" + ConfigurationManager.AppSettings["password_estudio_necesidades_api"]));

                        using (var client = new WebClient())
                        {
                            client.Encoding = Encoding.UTF8;
                            client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                            client.Headers.Add(HttpRequestHeader.Authorization, string.Format("Basic {0}", credentials));

                            respuesta_formato_edn = client.UploadString(new Uri(rutaServicio), "POST", json_generado_EdN);
                            client.Dispose();
                        }

                        return respuesta_formato_edn;
                    }
                    else
                    {
                        return "No se encontro pudo armar el formato de estudio de necesidades";
                    }
                }
                else
                {
                    return "No se encontro la información del formato de estudio de necesidades";
                }

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw ex;
            }
        }

        public static bool PerteneceACartera(string codigoAgente, string cuspp, List<Agente> listaAgentes, string rol, string usuario, bool incluirCesados)
        {
            bool pertenece = false;
            Agente agente = listaAgentes.Find(a => a.Id == codigoAgente);
            if (agente != null)
            {
                pertenece = true;
            }
            else if (incluirCesados)
            {
                if (Infraestructura.General.Utilitarios.EsRolVerAgentesCesados(rol))
                {
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(string.Empty, cuspp, string.Empty, string.Empty, string.Empty);
                    if (afiliado != null)
                    {
                        agente = servicioCotizador.ObtenerUltimoAgentePorCartera(afiliado.Agente.IdCartera, usuario);
                        if (agente != null)
                        {
                            Agente supervisor = listaAgentes.Find(s => s.IdPadre == agente.IdPadre);
                            if (supervisor != null)
                            {
                                pertenece = true;
                            }
                        }
                    }
                }
            }
            return pertenece;
        }
    }
}