using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Cotizador
{
    public partial class CerrarSolicitud : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CerrarSolicitud));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (!Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CierrePolizaRVI))
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.CierrePolizaRVI.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                }
            }
        }

        [WebMethod]
        public static Solicitud ObtenerCotizaciones(string tokenUsuario, string numeroSolicitud)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CierrePolizaRVI))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Solicitud solicitud = servicioCotizador.ListarCotizacionesPorSolicitud(numeroSolicitud, (string)HttpContext.Current.Session["Usuario"]);
                            // Obtener beneficiarios desde la rvi_benefi
                            var beneficiarios = servicioCotizador.ListarBeneficiarios(numeroSolicitud, (string)HttpContext.Current.Session["Usuario"]);
                            solicitud.BeneficiariosBenefi = beneficiarios;

                            return solicitud;
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [CierrePolizaRVI] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                            HttpContext.Current.Response.Status = "403 Forbidden";
                            HttpContext.Current.Response.StatusCode = 403;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return null;
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido un error al cargar los datos de la cotización: {0}", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarTablaCotizaciones(string tokenUsuario, List<Cotizacion> cotizaciones)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CierrePolizaRVI))
                        {
                            var pagina = new Page();
                            var control = (TablaCotizacionesCierreRVI)pagina.LoadControl("~/Controles/TablaCotizacionesCierreRVI.ascx");

                            control.Cotizaciones = cotizaciones.FindAll(c => c.EstadoCotizacion == "02" || c.EstadoCotizacion == "04");
                            control.PermisoConsultar = true;
                            control.SoloLectura = cotizaciones.FindAll(c => c.EstadoCotizacion == "04").Count > 0;

                            pagina.Controls.Add(control);

                            string html = "";
                            using (var sw = new StringWriter())
                            {
                                HttpContext.Current.Server.Execute(pagina, sw, false);
                                html = sw.ToString();
                            }

                            return html;
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [CierrePolizaRVI] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                            HttpContext.Current.Response.Status = "403 Forbidden";
                            HttpContext.Current.Response.StatusCode = 403;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return null;
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido un error al cargar los datos de la cotización: {0}", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Respuesta TransmitirPoliza(string tokenUsuario, string numeroSolicitud, int numeroCorrelativo, string numeroPoliza)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CierrePolizaRVI))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();

                            //Validar que la nueva póliza esté recaudada en SAMP
                            bool conAbono = true;
                            string mensaje = string.Empty;
                            var abono = servicioCotizador.ObtenerAbonoPorPoliza(Convert.ToInt32(numeroPoliza), (string)HttpContext.Current.Session["Usuario"]);
                            if (abono == null || abono.num_imputacion_mov == null)
                            {
                                conAbono = false;
                                mensaje = string.Format("La poliza {0} ha emitir no tiene el abono recaudado.", numeroPoliza);
                            }
                            else
                            {
                                var poliza = servicioCotizador.ObtenerDatosPolizaRV(numeroSolicitud, numeroCorrelativo, (string)HttpContext.Current.Session["Usuario"]);
                                if (poliza.val_mto_cia < abono.val_pesos_abono)
                                {
                                    conAbono = false;
                                    mensaje = string.Format("La poliza {0} ha emitir tiene el monto recaudado mayor al que se debe cobrar.", numeroPoliza);
                                }

                                if (poliza.val_mto_cia > abono.val_pesos_abono)
                                {
                                    conAbono = false;
                                    if (abono.val_pesos_abono == 0 || abono.val_pesos_abono == 0.0)
                                        mensaje = string.Format("La poliza {0} ha emitir no tiene recaudada primera prima.", numeroPoliza);
                                    else
                                        mensaje = string.Format("La poliza {0} ha emitir tiene el monto recaudado menor al que se debe cobrar.", numeroPoliza);
                                }
                            }

                            if (conAbono == false)
                            {
                                Respuesta respuestaAbono = new Respuesta();
                                respuestaAbono.Estado = Constante.COD_ERROR;
                                respuestaAbono.Mensaje = mensaje;
                                respuestaAbono.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuestaAbono.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                return respuestaAbono;
                            }
                            
                            Respuesta respuesta = servicioCotizador.TransferirDatosPolizaRRVV(numeroSolicitud, numeroCorrelativo, (string)HttpContext.Current.Session["Usuario"]);
                            if (respuesta.Estado == Constante.COD_OK)
                                servicioCotizador.ActualizarCotizacionGanadoraRVI(numeroSolicitud, numeroCorrelativo, (string)HttpContext.Current.Session["Usuario"]);

                            return respuesta;
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [CierrePolizaRVI] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                            HttpContext.Current.Response.Status = "403 Forbidden";
                            HttpContext.Current.Response.StatusCode = 403;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return null;
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido un error al cargar los datos de la cotización: {0}", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarBeneficiariosDireccion(string tokenUsuario, string numeroSolicitud, DateTime fecDevengue, string codTipoPension)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CierrePolizaRVI))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            string urlCrearSesionRviadm = ConfigurationManager.AppSettings["url_crear_sesion_rviadm"].ToString();
                            string urlRviadm = ConfigurationManager.AppSettings["url_rviadm_backend"].ToString();

                            //BENEFICIARIOS
                            // Obtener beneficiarios desde la rvi_benefi, orden descendente para crear los pasos, para que salga afiliado primero en el html
                            var beneficiarios = servicioCotizador.ListarBeneficiarios(numeroSolicitud, (string)HttpContext.Current.Session["Usuario"]).OrderByDescending(x => x.numCorrelativo);

                            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                            var listaComboboxProfesion = servicioCotizador.ListarProfesion((string)HttpContext.Current.Session["Usuario"]);
                            var listaComboboxNacionalidad = servicioCotizador.ListarNacionalidad((string)HttpContext.Current.Session["Usuario"]);

                            JArray listaPaisOrigen = new JArray();
                            var urlPaisOrigen = urlRviadm + ConfigurationManager.AppSettings["rviadm_backend_tipos_paises"].ToString();
                            urlPaisOrigen = string.Format(urlPaisOrigen, (string)HttpContext.Current.Session["Usuario"]);
                            listaPaisOrigen = ConsumirRviadm(urlCrearSesionRviadm, urlPaisOrigen, (string)HttpContext.Current.Session["Usuario"], "CWRV-PAISES", listaPaisOrigen);

                            JArray listaVinculoFamiliar = new JArray();
                            var urlVinculoFamiliar = urlRviadm + ConfigurationManager.AppSettings["rviadm_backend_tipos_doc_parentesco"].ToString();
                            urlVinculoFamiliar = string.Format(urlVinculoFamiliar, (string)HttpContext.Current.Session["Usuario"]);
                            listaVinculoFamiliar = ConsumirRviadm(urlCrearSesionRviadm, urlVinculoFamiliar, (string)HttpContext.Current.Session["Usuario"], "CWRV-DOCPARENTESCO", listaVinculoFamiliar);

                            JArray listaESSALUD = new JArray();
                            var urlESSALUD = urlRviadm + ConfigurationManager.AppSettings["rviadm_backend_tipos_essalud"].ToString();
                            urlESSALUD = string.Format(urlESSALUD, (string)HttpContext.Current.Session["Usuario"]);
                            listaESSALUD = ConsumirRviadm(urlCrearSesionRviadm, urlESSALUD, (string)HttpContext.Current.Session["Usuario"], "CWRV-ESSALUD", listaESSALUD);

                            string html = "";
                            foreach (var beneficiario in beneficiarios)
                            {
                                var pagina = new Page();
                                var control = (BeneficiarioCierreRVI)pagina.LoadControl("~/Controles/BeneficiarioCierreRVI.ascx");

                                control.ListaCombobox = listaCombobox;
                                control.ListaNacionalidad = listaComboboxNacionalidad;
                                control.ListaPaisOrigen = listaPaisOrigen;
                                control.ListaVinculoFamiliar = listaVinculoFamiliar;
                                control.ExistePersonaRviadm = false;
                                control.ListaESSALUD = listaESSALUD;

                                var tipoPension = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Prestacion];
                                control.GlosaTipoPension = tipoPension.Where(x => x.Id == codTipoPension).FirstOrDefault().Glosa;

                                //Usuario no ingresa fecha de invalidez en s_rviadm (viejito), por lo que se asigna por defecto la fec_devengue de rvi_propue
                                if (beneficiario.TipoInvalidez.Id == Enums.TipoInvalidez.Parcial.StringValue() || beneficiario.TipoInvalidez.Id == Enums.TipoInvalidez.Total.StringValue())
                                {
                                    if (beneficiario.FechaInvalidez == null)
                                        beneficiario.FechaInvalidez = fecDevengue;
                                }

                                Beneficiario beneficiarioRviadm = null;
                                if (beneficiario.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                                {
                                    if (!string.IsNullOrEmpty(beneficiario.Identificacion.IdTipo) && !string.IsNullOrEmpty(beneficiario.Identificacion.Numero))
                                    {
                                        if (beneficiario.Identificacion.Numero != "0")
                                        {
                                            beneficiarioRviadm = ObtenerPersonaBeneficiarioRviadm(urlCrearSesionRviadm, urlRviadm, beneficiario.Identificacion.IdTipo, beneficiario.Identificacion.Numero, numeroSolicitud, beneficiario.numCorrelativo, listaComboboxNacionalidad);

                                            if (beneficiarioRviadm != null)
                                            {
                                                control.ExistePersonaRviadm = true;

                                                if (string.IsNullOrEmpty(beneficiario.ApellidoPaterno))
                                                    beneficiario.ApellidoPaterno = beneficiarioRviadm.ApellidoPaterno;

                                                if (string.IsNullOrEmpty(beneficiario.ApellidoMaterno))
                                                    beneficiario.ApellidoMaterno = beneficiarioRviadm.ApellidoMaterno;

                                                if (string.IsNullOrEmpty(beneficiario.Nombre))
                                                    beneficiario.Nombre = beneficiarioRviadm.Nombre;

                                                if (beneficiario.Nacionalidad == null || beneficiario.Nacionalidad.cod_nacionalidad == null)
                                                    beneficiario.Nacionalidad = beneficiarioRviadm.Nacionalidad;

                                                if (beneficiario.FechaNacimiento == null)
                                                    beneficiario.FechaNacimiento = beneficiarioRviadm.FechaNacimiento;

                                                if (beneficiario.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                                                {
                                                    if (beneficiario.FechaFallecimiento == null)
                                                        beneficiario.FechaFallecimiento = beneficiarioRviadm.FechaFallecimiento;
                                                }

                                                if (beneficiario.PaisOrigen == null || beneficiario.PaisOrigen.cod_parametro == null)
                                                    beneficiario.PaisOrigen = beneficiarioRviadm.PaisOrigen;

                                                if (beneficiario.TipoInvalidez == null || beneficiario.TipoInvalidez.Id == null)
                                                    beneficiario.TipoInvalidez = beneficiarioRviadm.TipoInvalidez;

                                                if (string.IsNullOrEmpty(beneficiario.CorreoElectronico))
                                                    beneficiario.CorreoElectronico = beneficiarioRviadm.CorreoElectronico;

                                                if (string.IsNullOrEmpty(beneficiario.numCelular))
                                                    beneficiario.numCelular = beneficiarioRviadm.numCelular;
                                            }
                                        }
                                    }
                                }
                                
                                control.Beneficiario = beneficiario;

                                pagina.Controls.Add(control);

                                using (var sw = new StringWriter())
                                {
                                    HttpContext.Current.Server.Execute(pagina, sw, false);
                                    html += sw.ToString();
                                }
                            }

                            //DIRECCION
                            html += CargarHtmlDireccion(numeroSolicitud);

                            return html;
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [CierrePolizaRVI] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                            HttpContext.Current.Response.Status = "403 Forbidden";
                            HttpContext.Current.Response.StatusCode = 403;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return null;
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido un error al cargar los datos de beneficiarios y dirección: {0}", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        private static string CargarHtmlDireccion(string numeroSolicitud)
        {
            try
            {
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                string urlCrearSesionRviadm = ConfigurationManager.AppSettings["url_crear_sesion_rviadm"].ToString();
                string urlRviadm = ConfigurationManager.AppSettings["url_rviadm_backend"].ToString();

                // Obtener los datos del afiliado
                Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(numeroSolicitud, "", "", "", "");

                List<Direccion> direcciones = servicioCotizador.ListarRviDireccion(numeroSolicitud, (string)HttpContext.Current.Session["Usuario"]);
                Direccion direccionSolicitud = direcciones.Find(d => d.Principal);
                List<Direccion> direccionesCUSPP = null;

                // Si las direcciones de benefi vienen en blanco se autocompletan con las de cwrv_direccion
                if (direccionSolicitud == null)
                {
                    direccionesCUSPP = servicioCotizador.ListarDireccion(afiliado.CUSPP);
                    if (direccionesCUSPP.Count > 0)
                    {
                        direccionSolicitud = direccionesCUSPP.Find(d => d.Principal);
                        if (direccionSolicitud == null)
                        {
                            direccionSolicitud = direccionesCUSPP.First();
                        }
                    }
                }

                //Obtener los datos de listas
                List<Parametro> listaTipoVia = servicioCotizador.ObtenerParametros("TIPOVIA");

                JArray listaDepartamentos = new JArray();
                var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
                urlDepartamentos = string.Format(urlDepartamentos, (string)HttpContext.Current.Session["Usuario"]);
                listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, (string)HttpContext.Current.Session["Usuario"], listaDepartamentos);

                JArray listaProvincias = new JArray();
                JArray listaDistritos = new JArray();
                if (direccionSolicitud != null)
                {
                    var urlProvincias = ConfigurationManager.AppSettings["url_lista_provincias"].ToString();
                    urlProvincias = string.Format(urlProvincias, direccionSolicitud.Departamento.Id, (string)HttpContext.Current.Session["Usuario"]);
                    listaProvincias = ObtenerUbigeo(urlToken, urlProvincias, (string)HttpContext.Current.Session["Usuario"], listaProvincias);

                    var urlDistritos = ConfigurationManager.AppSettings["url_lista_distritos"].ToString();
                    urlDistritos = string.Format(urlDistritos, direccionSolicitud.Ciudad.Id, (string)HttpContext.Current.Session["Usuario"]);
                    listaDistritos = ObtenerUbigeo(urlToken, urlDistritos, (string)HttpContext.Current.Session["Usuario"], listaDistritos);
                }

                JArray listaTipoZona = new JArray();
                var urlTipoZona = urlRviadm + ConfigurationManager.AppSettings["rviadm_backend_tipos_zona"].ToString();
                urlTipoZona = string.Format(urlTipoZona, (string)HttpContext.Current.Session["Usuario"]);
                listaTipoZona = ConsumirRviadm(urlCrearSesionRviadm, urlTipoZona, (string)HttpContext.Current.Session["Usuario"], "CWRV-TIPOZONA", listaTipoZona);

                JArray listaLargaDistancia = new JArray();
                var urlLargaDistancia = urlRviadm + ConfigurationManager.AppSettings["rviadm_backend_tipos_larga_distancia"].ToString();
                urlLargaDistancia = string.Format(urlLargaDistancia, (string)HttpContext.Current.Session["Usuario"]);
                listaLargaDistancia = ConsumirRviadm(urlCrearSesionRviadm, urlLargaDistancia, (string)HttpContext.Current.Session["Usuario"], "CWRV-LARGADISTANCIA", listaLargaDistancia);

                //obtener html
                var pagina = new Page();
                var control = (DireccionCierreRVI)pagina.LoadControl("~/Controles/DireccionCierreRVI.ascx");

                control.ListaTipoVia = listaTipoVia;
                control.ListaDepartamentos = listaDepartamentos;
                control.ListaProvincias = listaProvincias;
                control.ListaDistritos = listaDistritos;
                control.ListaTipoZona = listaTipoZona;
                control.ListaLargaDistancia = listaLargaDistancia;
                control.DireccionSolicitud = direccionSolicitud;

                pagina.Controls.Add(control);

                string html = "";
                using (var sw = new StringWriter())
                {
                    HttpContext.Current.Server.Execute(pagina, sw, false);
                    html = sw.ToString();
                }

                return html;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Se ha producido un error al cargar los datos de la dirección: {0}", ex.Message), ex);
                throw (ex);
            }
        }

        private static Beneficiario ObtenerPersonaBeneficiarioRviadm(string urlCrearSesionRviadm, string urlRviadm, string tipoDoc, string numeroDoc, string numeroSolicitud, int numeroCorrelativo, List<Nacionalidad> nacionalidades)
        {
            try
            {
                Beneficiario beneficiario = null;
                JObject persona = new JObject();
                var urlPersonaRviadm = urlRviadm + ConfigurationManager.AppSettings["rviadm_backend_obtener_persona"].ToString();
                urlPersonaRviadm = string.Format(urlPersonaRviadm, tipoDoc, numeroDoc, (string)HttpContext.Current.Session["Usuario"]);
                persona = ConsumirRviadm(urlCrearSesionRviadm, urlPersonaRviadm, (string)HttpContext.Current.Session["Usuario"], "CWRV-PERSONA", persona);

                if (persona != null)
                {
                    if (!string.IsNullOrEmpty(persona["cod_tipo_identificacion"].ToString()))
                    {
                        beneficiario = new Beneficiario();

                        beneficiario.numSolicitud = numeroSolicitud;
                        beneficiario.numCorrelativo = numeroCorrelativo;
                        beneficiario.usuario = HttpContext.Current.Session["Usuario"].ToString();
                        beneficiario.Identificacion = new Identificacion()
                        {
                            IdTipo = tipoDoc,
                            Numero = numeroDoc
                        };

                        beneficiario.ApellidoPaterno = persona["ape_paterno"].ToString().ToUpper();
                        beneficiario.ApellidoMaterno = persona["ape_materno"].ToString().ToUpper();
                        beneficiario.Nombre = persona["nom_persona"].ToString().ToUpper();
                        beneficiario.Sexo = Convert.ToChar(persona["cod_sexo"].ToString());

                        if (!string.IsNullOrEmpty(persona["cod_nacionalidad"].ToString()))
                        {
                            if (nacionalidades == null)
                                nacionalidades = servicioCotizador.ListarNacionalidad((string)HttpContext.Current.Session["Usuario"]);

                            string nacionalidadRviadm = persona["cod_nacionalidad"].ToString();
                            string nacionalidadExpserv = nacionalidades.Where(x => x.cod_equivalencia_rviadm == nacionalidadRviadm).FirstOrDefault().cod_nacionalidad;

                            beneficiario.Nacionalidad = new Nacionalidad()
                            {
                                cod_nacionalidad = nacionalidadExpserv,
                                gls_nacionalidad = persona["gls_nacionalidad"].ToString()
                            };
                        }

                        if (!string.IsNullOrEmpty(persona["fec_nacimiento"].ToString()))
                            beneficiario.FechaNacimiento = Convert.ToDateTime(persona["fec_nacimiento"].ToString());
                        if (!string.IsNullOrEmpty(persona["fec_fallecimiento"].ToString()))
                            beneficiario.FechaFallecimiento = Convert.ToDateTime(persona["fec_fallecimiento"].ToString());

                        if (!string.IsNullOrEmpty(persona["cod_pais_origen_doc"].ToString()))
                        {
                            beneficiario.PaisOrigen = new Temporal()
                            {
                                cod_parametro = persona["cod_pais_origen_doc"].ToString()
                            };
                        }

                        beneficiario.Invalido = persona["ind_invalidez"].ToString() == "S" ? true : false;

                        if (!string.IsNullOrEmpty(persona["cod_tipo_invalidez"].ToString()))
                        {
                            beneficiario.TipoInvalidez = new TipoInvalidez()
                            {
                                Id = persona["cod_tipo_invalidez"].ToString()
                            };
                        }

                        if (!string.IsNullOrEmpty(persona["fec_invalidez"].ToString()))
                            beneficiario.FechaInvalidez = Convert.ToDateTime(persona["fec_invalidez"].ToString());

                        JObject beneficiarioRviadm = new JObject();
                        var urlBeneficiarioRviadm = urlRviadm + ConfigurationManager.AppSettings["rviadm_backend_obtener_beneficiario"].ToString();
                        urlBeneficiarioRviadm = string.Format(urlBeneficiarioRviadm, tipoDoc, numeroDoc, (string)HttpContext.Current.Session["Usuario"]);
                        beneficiarioRviadm = ConsumirRviadm(urlCrearSesionRviadm, urlBeneficiarioRviadm, (string)HttpContext.Current.Session["Usuario"], "CWRV-BENEFICIARIO", beneficiarioRviadm);

                        if (beneficiarioRviadm != null)
                        {
                            if (!string.IsNullOrEmpty(beneficiarioRviadm["cod_tipo_identificacion"].ToString()))
                            {
                                beneficiario.CorreoElectronico = beneficiarioRviadm["gls_mail"].ToString();
                                beneficiario.numCelular = beneficiarioRviadm["num_telefono"].ToString();
                            }
                        }
                    }
                }

                return beneficiario;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Se ha producido un error al cargar los datos de persona y beneficiario desde origen de datos Rviadm: {0}", ex.Message), ex);
                throw (ex);
            }
        }

        [WebMethod]
        public static Beneficiario ObtenerDatosRviadm(string tokenUsuario, string tipoDoc, string numeroDoc, string numeroSolicitud, int numeroCorrelativo)
        {
            try
            {
                if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                {
                    string urlCrearSesionRviadm = ConfigurationManager.AppSettings["url_crear_sesion_rviadm"].ToString();
                    string urlRviadm = ConfigurationManager.AppSettings["url_rviadm_backend"].ToString();

                    Beneficiario beneficiario = ObtenerPersonaBeneficiarioRviadm(urlCrearSesionRviadm, urlRviadm, tipoDoc, numeroDoc, numeroSolicitud, numeroCorrelativo, null);

                    return beneficiario;
                }
                else
                {
                    log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                    HttpContext.Current.Response.Status = "401 Unauthorized";
                    HttpContext.Current.Response.StatusCode = 401;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.Error(string.Format("Se ha producido un error al cargar los datos de persona y beneficiario desde origen de datos Rviadm: {0}", ex.Message), ex);
                throw (ex);
            }
        }

        [WebMethod]
        public static List<Parametro> ListarProvincias(string tokenUsuario, string idDepartamento)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionOficial))
                        {
                            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                            JArray listaProvincias = new JArray();
                            var urlProvincias = ConfigurationManager.AppSettings["url_lista_provincias"].ToString();
                            urlProvincias = string.Format(urlProvincias, idDepartamento, (string)HttpContext.Current.Session["Usuario"]);
                            listaProvincias = ObtenerUbigeo(urlToken, urlProvincias, (string)HttpContext.Current.Session["Usuario"], listaProvincias);

                            List<Parametro> parametros = new List<Parametro>();
                            foreach (var item in listaProvincias)
                            {
                                parametros.Add(new Parametro
                                {
                                    Id = item["id_provincia"].ToString().ToLower(),
                                    Glosa = item["gls_provincia"].ToString().ToUpper()
                                });
                            }
                            
                            return parametros;
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [ListarProvincias] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                            HttpContext.Current.Response.Status = "403 Forbidden";
                            HttpContext.Current.Response.StatusCode = 403;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return null;
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Se ha producido un error al listar las provincias", ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static List<Parametro> ListarDistritos(string tokenUsuario, string idProvincia)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionOficial))
                        {
                            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                            JArray listaDistritos = new JArray();
                            var urlDistritos = ConfigurationManager.AppSettings["url_lista_distritos"].ToString();
                            urlDistritos = string.Format(urlDistritos, idProvincia, (string)HttpContext.Current.Session["Usuario"]);
                            listaDistritos = ObtenerUbigeo(urlToken, urlDistritos, (string)HttpContext.Current.Session["Usuario"], listaDistritos);

                            List<Parametro> parametros = new List<Parametro>();
                            foreach (var item in listaDistritos)
                            {
                                parametros.Add(new Parametro
                                {
                                    Id = item["id_distrito"].ToString().ToLower(),
                                    Glosa = item["gls_distrito"].ToString().ToUpper()
                                });
                            }

                            return parametros;
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [ListarDistritos] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                            HttpContext.Current.Response.Status = "403 Forbidden";
                            HttpContext.Current.Response.StatusCode = 403;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return null;
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Se ha producido un error al listar los distritos", ex);
                    throw (ex);
                }
            }
        }


        [WebMethod]
        public static Respuesta GrabarBeneficiarioDireccion(string tokenUsuario, string numeroSolicitud, Beneficiario beneficiario, string indicadorEdicion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CierrePolizaRVI))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();

                            beneficiario.numSolicitud = numeroSolicitud;
                            beneficiario.usuario = HttpContext.Current.Session["Usuario"].ToString();

                            if (indicadorEdicion == "B")//Beneficiario
                                beneficiario.direccionPrincipal = null;

                            if (indicadorEdicion == "D")//Dirección
                                beneficiario.direccionPrincipal.usuario = HttpContext.Current.Session["Usuario"].ToString();

                            // Actualizar Beneficiario y Dirección
                            var respuesta = servicioCotizador.ActualizarBeneficiario(beneficiario);
                            
                            return respuesta;
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [CierrePolizaRVI] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                            HttpContext.Current.Response.Status = "403 Forbidden";
                            HttpContext.Current.Response.StatusCode = 403;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return null;
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido un error al cargar los datos de la cotización: {0}", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        private static T ObtenerUbigeo<T>(string urlToken, string urlServicio, string usuario, T request)
        {
            T respuesta = default(T);
            string token_generado = string.Empty;

            try
            {
                log.Info("Consumiendo servicio token: " + urlToken);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                log.Info("Pasando el json al servicio");
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"usuario\": \"" + usuario + "\"}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                log.Info("Leyendo el servicio");
                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var jsonResult = streamReader.ReadToEnd();
                    JObject jObject = JObject.Parse(jsonResult);
                    token_generado = (string)jObject["accessToken"];
                }

                if (token_generado != "")
                {
                    log.Info("Consumiendo servicio" + urlServicio);
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServicio);
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                    log.Info("Leyendo el servicio");

                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        var jsonResult = streamReader.ReadToEnd();

                        if (jsonResult.Length > 0)
                        {
                            respuesta = (T)JsonConvert.DeserializeObject(jsonResult);
                        }
                    }
                }

            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    throw new Exception("Error en el servicio");
                }
            }
            return respuesta;
        }

        private static T ConsumirRviadm<T>(string urlCrearSesion, string urlServicio, string usuario, string token, T request)
        {
            T respuesta = default(T);

            try
            {
                log.Info("Accediendo a las Key necesarias");
                string token_acceso = token + DateTime.Now.ToString("yyyyMMddHHmmssfff");

                using (var httpClient = new HttpClient())
                {
                    var objRequest = new
                    {
                        usuario,
                        token = token_acceso
                    };

                    log.Info("Consumiendo método para crear sesión");
                    var content = JsonConvert.SerializeObject(objRequest);
                    var response = httpClient.PostAsync(urlCrearSesion, new StringContent(content, Encoding.UTF8, "application/json")).Result;
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    var resultadoCorrecto = response.IsSuccessStatusCode;

                    if (resultadoCorrecto)
                    {
                        log.Info("Consumiendo servicio" + urlServicio);
                        string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token_acceso));
                        var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServicio);
                        httpWebRequest.ContentType = "application/json";
                        httpWebRequest.Method = "GET";
                        httpWebRequest.Headers["Authorization"] = $"Basic {credentials}";

                        var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                        using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                        {
                            var jsonResult = streamReader.ReadToEnd();
                            if (jsonResult.Length > 0)
                            {
                                respuesta = JsonConvert.DeserializeObject<T>(jsonResult);
                            }
                        }
                    }
                }
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    throw new Exception("Error en el servicio");
                }
            }
            return respuesta;
        }
    }
}