using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
//using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;

using log4net;
using System.Reflection;
using System.Threading;
using System.ServiceModel;
using System.Web.Services;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Globalization;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
{
    public partial class GrupoFamiliarAfiliadoCierre : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(GrupoFamiliarAfiliado));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusCerrar))
                    {
                        if (!IsPostBack)
                        {

                            string source = Request.QueryString["source"];
                            string solicitud = Request.QueryString["solicitud"];
                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));

                            SolicitudRPPlus solicitudRRP = null;
                            string usuario = HttpContext.Current.Session["Usuario"].ToString();

                            if (Session["ListadoEstadoCivil"] == null)
                            {
                                Session["ListadoEstadoCivil"] = servicioCotizador.ListarEstadoCivil(usuario);
                            }

                            if (Session["ListadoProfesion"] == null)
                            {
                                Session["ListadoProfesion"] = servicioCotizador.ListarProfesion(usuario);
                            }

                            if (Session["ListadoNacionalidad"] == null)
                            {
                                Session["ListadoNacionalidad"] = servicioCotizador.ListarNacionalidad(usuario);
                            }

                            if (Session["ListadoDepartamento"] == null)
                            {
                                JArray listaDepartamentos = new JArray();
                                string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                                var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
                                urlDepartamentos = string.Format(urlDepartamentos, usuario);
                                listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, usuario, listaDepartamentos);
                                Session["ListadoDepartamento"] = listaDepartamentos.ToObject<List<Departamento>>();
                            }

                            if (source == "correo")
                            {

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                List<Temporal> listaCombobox = servicioCotizador.ListarGruposFamiliaresxSolicitud(solicitud);

                                Session["listGrupoFamiliar"] = listaCombobox;
                                Session["idGrupoFamiliar"] = listaCombobox[0].codigo;
                                Session["Cantidad"] = listaCombobox[0].item;
                                Session["Total"] = listaCombobox[0].cantidad;

                                Session["ModGruFamModo"] = "MC";
                                Session["Soltud"] = solicitud;
                                Session["PaginaLlamada"] = "../RentaPrivadaPlus/SeleccionSolicitud.aspx";
                                Session["NroSolicitud"] = solicitud;

                                solicitudRRP = servicioCotizador.ObtenerEstadoSolicitudRPPlus(solicitud);

                                Session["FecSoltud"] = Convert.ToDateTime(solicitudRRP.FechaSolicitud).ToString("dd/MM/yyyy");
                                Session["EstadoSoltud"] = solicitudRRP.CodigoEstado;
                                Session["CUSPP_PLUS"] = solicitudRRP.Afiliado.CUSPP;
                                Session["Vendedor"] = solicitudRRP.Afiliado.Agente.Id;
                                Session["CUSPP_RP"] = solicitudRRP.Afiliado.CUSPP.ToString();
                                Session["AFP_RP"] = solicitudRRP.Afiliado.AFP.Id.ToString();

                                Session["Consentimiento"] = true;

                            }
                            else
                            {
                                if (Session["CUSPP"] == null && Session["NroSolicitud"] == null)
                                {
                                    solicitudRRP = servicioCotizador.ObtenerEstadoSolicitudRPPlus(Session["Soltud"].ToString());

                                    Session["FecSoltud"] = Convert.ToDateTime(solicitudRRP.FechaSolicitud).ToString("dd/MM/yyyy");
                                    Session["EstadoSoltud"] = solicitudRRP.CodigoEstado;
                                    Session["CUSPP_PLUS"] = solicitudRRP.Afiliado.CUSPP;
                                    Session["CUSPP"] = solicitudRRP.Afiliado.CUSPP;
                                    Session["Vendedor"] = solicitudRRP.Afiliado.Agente.Id;
                                    Session["CUSPP_RP"] = solicitudRRP.Afiliado.CUSPP.ToString();
                                    Session["AFP_RP"] = solicitudRRP.Afiliado.AFP.Id.ToString();

                                    Session["Consentimiento"] = true;
                                }
                            }



                            CargarInformacionInicialPantalla();
                            //LimpiarFormularios();

                            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
                            {
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP"]));
                                ModGruFamModo_RP.Value = Convert.ToString((Session["ModGruFamModo"]));
                                ModIdGrupoFamiliar.Value = Convert.ToString((Session["idGrupoFamiliar"]));
                                ModGruFamCantidad.Value = Convert.ToString((Session["Cantidad"]));
                                ModGruFamTotal.Value = Convert.ToString((Session["Total"]));
                                ModGruFamsoltud.Value = Convert.ToString((Session["Soltud"]));
                                ModGruFamfecsoltud.Value = Convert.ToString((Session["FecSoltud"]));
                                ModGruFamestadosoltud.Value = Convert.ToString((Session["EstadoSoltud"]));
                            }
                            else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                            {
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP_PLUS"]));
                                ModGruFamModo_RP.Value = Convert.ToString((Session["ModGruFamModo"]));
                                ModIdGrupoFamiliar.Value = Convert.ToString((Session["idGrupoFamiliar"]));
                                ModGruFamCantidad.Value = Convert.ToString((Session["Cantidad"]));
                                ModGruFamTotal.Value = Convert.ToString((Session["Total"]));
                                ModGruFamsoltud.Value = Convert.ToString((Session["Soltud"]));
                                ModGruFamfecsoltud.Value = Convert.ToString((Session["FecSoltud"]));
                                ModGruFamestadosoltud.Value = Convert.ToString((Session["EstadoSoltud"]));
                            }
                            else
                            {
                                //<SOLINI25621>
                                //Response.Redirect("Cotizador.aspx");
                                Response.Redirect(Convert.ToString((Session["PaginaLlamada"])));
                                //<SOLFIN25621>
                            }

                            // Validar si es que el cliente ya tiene firma digital, en ese caso redirigir a la página de
                            // selección de solicitud pues el usuario ya o debe tener permitido modificar la información
                            // del afiliado ni de sus beneficiarios
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            if (servicioCotizador.isFirmaDigitalAprobada(ModGruFamsoltud.Value, 1, Session["usuario"].ToString()))
                            {
                                Session["ModSolModo"] = "CERRAR";
                                Session["idSolicitud"] = ModGruFamsoltud.Value;
                                Session["fecCotizacion"] = ModGruFamfecsoltud.Value;
                                Response.Redirect("SeleccionSolicitud.aspx");
                            }
                        }
                        else
                        {
                            // if (SaldoCIC.Text != String.Empty) SaldoCIC.Text = Convert.ToDouble(SaldoCIC.Text, new CultureInfo("es-PE")).ToString();
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.SolicitudPlusCerrar.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<string> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }

        }

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            CargarCombobox(ModGruFamTipoIdentificacion_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);

            List<string> lstNoParentesco = CargarNoParentescos();
            CargarCombobox(ModGruFamParentesco_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Parentesco].Where(p => !(lstNoParentesco.Contains(p.Id))).ToList());
            CargarCombobox(ModGruFamSexo_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);
            CargarCombobox(ModGruFamTipoInvalidez_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Invalidez]);
            CargarCombobox(ModGruFamMonedaingreso_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.MonedaRentaPrivada]);

            CargarComboboxNuevasEntidadesDatos(ModGruFamProfesion_RP, "Profesion");
            CargarComboboxNuevasEntidadesDatos(ModGruFamNacional_RP, "Nacionalidad");
            CargarComboboxNuevasEntidadesDatos(ModGruFamResidencia_RP, "Residencia");
            CargarComboboxNuevasEntidadesDatos(ModGruFamEstadoCivil_RP, "EstadoCivil");

            CargarComboboxNuevosDatos(ModGruFamBanco_RP, "BANCO");
            CargarComboboxNuevosDatos(ModGruFamComunicacion_RP, "COMUNICACION");

            ModGruFamPEP_RP.Items.Add(new ListItem("«Seleccione»", "0"));
            ModGruFamPEP_RP.Items.Add(new ListItem("Sí", "S"));
            ModGruFamPEP_RP.Items.Add(new ListItem("No", "N"));

            ModGruFamSO_RP.Items.Add(new ListItem("«Seleccione»", "0"));
            ModGruFamSO_RP.Items.Add(new ListItem("Sí", "S"));
            ModGruFamSO_RP.Items.Add(new ListItem("No", "N"));

            List<ParametroGeneral> comboConfidencialidadDatos = new List<ParametroGeneral>();
            comboConfidencialidadDatos.Add(new ParametroGeneral { Codigo = "0", Descripcion = "Si" });
            comboConfidencialidadDatos.Add(new ParametroGeneral { Codigo = "1", Descripcion = "No" });

            ModGruFamConfidencialidadDatos_RP.Items.Clear();

            foreach (ParametroGeneral item in comboConfidencialidadDatos)
            {
                ModGruFamConfidencialidadDatos_RP.Items.Add(new ListItem(item.Descripcion, item.Codigo));
            }

            ModGruFamIndInvalidez_RP.Items.Add(new ListItem("«Seleccione»", "0"));
            ModGruFamIndInvalidez_RP.Items.Add(new ListItem("Sí", "S"));
            ModGruFamIndInvalidez_RP.Items.Add(new ListItem("No", "N"));

            ModGruFamTipoCtaBanco_RP.Items.Add(new ListItem("«Seleccione»", "0"));
            ModPaginaLlamada.Value = Convert.ToString(Session["PaginaLlamada"]);

            CargarCombobox(ModGruFamTipoIdentificacion_RPP, listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);
            List<string> lstNoParentescoPersonaVinculada = CargarNoParentescosPersonaVinculada();
            CargarCombobox(ModGruFamParentesco_RPP, listaCombobox[(int)Enums.CategoriaCombobox.ParentescoPVPEP]);
        }

        private List<string> CargarNoParentescos()
        {
            List<string> lstNoParentesco = new List<string>();

            lstNoParentesco.Add(Enums.Parentesco.Primo.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Otros.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Sobrino.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Hermano.StringValue());

            return lstNoParentesco;
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

        //<INIGTI_7012>

        private void CargarComboboxNuevasEntidadesDatos(DropDownList control, string tabla)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));

            if (tabla == "Profesion")
            {
                var listaComboboxProfesion = (List<Profesion>)Session["ListadoProfesion"];

                foreach (Profesion item in listaComboboxProfesion)
                {
                    control.Items.Add(new ListItem(item.gls_profesion, item.cod_profesion));
                }
            }
            else if (tabla == "Nacionalidad")
            {
                var listaComboboxNacionalidad = (List<Nacionalidad>)Session["ListadoNacionalidad"];

                foreach (Nacionalidad item in listaComboboxNacionalidad)
                {
                    control.Items.Add(new ListItem(item.gls_nacionalidad, item.cod_nacionalidad));
                }
            }
            else if (tabla == "Residencia")
            {
                JArray listaComboboxResidencia = new JArray();
                // listaComboboxResidencia = (JArray)Session["ListadoDepartamento"];
                var departamentos = (List<Departamento>)Session["ListadoDepartamento"];
                listaComboboxResidencia = JArray.FromObject(departamentos);

                foreach (var item in listaComboboxResidencia)
                {
                    control.Items.Add(new ListItem(item["gls_departamento"].ToString().ToUpper(), item["id_departamento"].ToString().ToLower()));
                }

            }
            else if (tabla == "EstadoCivil")
            {
                var listaComboboxEstadoCivil = (List<EstadoCivil>)Session["ListadoEstadoCivil"];

                foreach (EstadoCivil item in listaComboboxEstadoCivil)
                {
                    control.Items.Add(new ListItem(item.gls_estado_civil, item.cod_estado_civil));
                }
            }

        }

        private void CargarComboboxNuevosDatos(DropDownList control, string tabla)
        {

            //List<Parametro> listaParametro = new List<Parametro>;

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerParametros(tabla);

            control.Items.Clear();

            control.Items.Add(new ListItem("«Seleccione»", "0"));

            foreach (Parametro item in listaParametro)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }

        }

        //<FINGTI_7012>

        //<SOLINI25621>

        [WebMethod]
        public static String SessionIdGrupoFamiliar(int idGrupoFamiliar, string solitud, string fecha, int estado, string paginaLlamada)//<INI.GTI_7012_20>
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio GrupoFamiliarAfiliadoCierre.SessionIdGrupoFamiliar WebMethod");
                //HttpContext.Current.Session["idGrupoFamiliar"] = idGrupoFamiliar;
                HttpContext.Current.Session["ModGruFamModo"] = "MC";
                HttpContext.Current.Session["Soltud"] = solitud;
                HttpContext.Current.Session["FecSoltud"] = fecha;
                HttpContext.Current.Session["EstadoSoltud"] = estado;
                //<INI.GTI_7012_20>
                HttpContext.Current.Session["PaginaLlamada"] = paginaLlamada;
                //<FIN.GTI_7012_20>
                log.Debug("Fin GrupoFamiliarAfiliadoCierre.SessionIdGrupoFamiliar WebMethod");

                return "OK";
            }
        }


        [WebMethod]
        public static Respuesta InsertarGrupoFamiliar(string tokenUsuario,
                                                        string cuspp,
                                                        string apellidoPaterno,
                                                        string apellidoMaterno,
                                                        string nombres,
                                                        string tipoIdentificacion,
                                                        string numeroIdentificacion,
                                                        string parentesco,
                                                        string sexo,
                                                        string fechaNacimiento,
                                                        string invalidez,
                                                        string tipoInvalidez,
                                                        string fechaInvalidez,
                                                        string PEP,
                                                        string sujetoObligado,
                                                        string nacionalidad,
                                                        string profesion,
                                                        string residencia)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio GrupoFamiliarAfiliadoCierre.InsertarGrupoFamiliar WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    GrupoFamiliar gru = new GrupoFamiliar
                                    {
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Identificacion = new Identificacion(),
                                        Parentesco = new Parentesco { Id = parentesco },
                                        Sexo = Convert.ToChar(sexo),
                                        FechaNacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE")),
                                        Invalido = (invalidez == "S") ? true : false,
                                        TipoInvalidez = new TipoInvalidez { Id = tipoInvalidez },
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };
                                    if (apellidoPaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoPaterno = apellidoPaterno;
                                    }
                                    if (apellidoMaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoMaterno = apellidoMaterno;
                                    }
                                    if (nombres.Trim() != String.Empty)
                                    {
                                        gru.Nombre = nombres;
                                    }
                                    if (tipoIdentificacion.Trim() != "0")
                                    {
                                        gru.Identificacion.IdTipo = tipoIdentificacion;
                                    }
                                    if (numeroIdentificacion.Trim() != String.Empty)
                                    {
                                        //gru.Identificacion.Numero = Convert.ToInt32(numeroIdentificacion);
                                        gru.Identificacion.Numero = numeroIdentificacion;
                                    }
                                    if (fechaInvalidez.Trim() != String.Empty)
                                    {
                                        gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
                                    }

                                    //<INIGTI_7012>
                                    if (nacionalidad.Trim() != "0")
                                    {
                                        gru.Nacionalidad = new Temporal { cod_parametro = nacionalidad };
                                    }
                                    else { gru.Nacionalidad = new Temporal { cod_parametro = "0" }; }

                                    if (profesion.Trim() != "0")
                                    {
                                        gru.Profesion = new Temporal { cod_parametro = profesion };
                                    }
                                    else { gru.Profesion = new Temporal { cod_parametro = "0" }; }

                                    if (residencia.Trim() != "0")
                                    {
                                        gru.Residencia = new Temporal { cod_parametro = residencia };
                                    }
                                    else { gru.Residencia = new Temporal { cod_parametro = "0" }; }

                                    gru.ind_PEP = (PEP == "S") ? true : false;
                                    gru.ind_SujetoObligado = (sujetoObligado == "S") ? true : false;
                                    //<FINGTI_7012>

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.RegistrarGrupoFamiliar(gru);

                                    //respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                    //respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                    //respuesta.Mensaje = "Grupo familiar agregado correctamente.";

                                    string nombreTerminal = String.Empty;

                                    try
                                    {
                                        nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                    }
                                    catch (Exception)
                                    {
                                        log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                            HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                    }

                                    nombreTerminal += HttpContext.Current.Request.UserAgent;

                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                        Detalle = String.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "RegistrarGrupoFamiliar", Environment.NewLine, Environment.NewLine, "Grupo Familiar", JsonConvert.SerializeObject(gru)),
                                        IdTipoEvento = Enums.EventoLog.RegistrarDatosBeneficiario.StringValue()
                                    });

                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.GrupoFamiliarInsertar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.InsertarGrupoFamiliar WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.InsertarGrupoFamiliar WebMethod");

                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static Respuesta ModificarGrupoFamiliar(string tokenUsuario,
                                                        string idGrupoFamiliar,
                                                        string cuspp,
                                                        string apellidoPaterno,
                                                        string apellidoMaterno,
                                                        string nombres,
                                                        string tipoIdentificacion,
                                                        string numeroIdentificacion,
                                                        string parentesco,
                                                        string sexo,
                                                        string fechaNacimiento,
                                                        string invalidez,
                                                        string tipoInvalidez,
                                                        string fechaInvalidez,
                                                        string PEP,
                                                        string sujetoObligado,
                                                        string nacionalidad,
                                                        string profesion,
                                                        string residencia,
                                                        string banco,
                                                        string tipoBanco,
                                                        string comunicacion,
                                                        string numeroBanco,
                                                        string confidencialidadDatos,
                                                        string flagRenta,
                                                        string estadoCivil,
                                                        string correoElectronico,
                                                        string centroLaboral,
                                                        string cargo,
                                                        string actividadEconomica,
                                                        string monedaIngreso,
                                                        string ingreso,
                                                        string telefono,
                                                        string celular,
                                                        string origen_fondo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio GrupoFamiliarAfiliadoCierre.ModificarGrupoFamiliar WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    GrupoFamiliar gru = new GrupoFamiliar
                                    {
                                        Id = Convert.ToInt32(idGrupoFamiliar),
                                        Afiliado = new Afiliado { CUSPP = cuspp },
                                        Identificacion = new Identificacion(),
                                        Parentesco = new Parentesco { Id = parentesco },
                                        Sexo = Convert.ToChar(sexo),
                                        FechaNacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE")),
                                        Invalido = (invalidez == "S") ? true : false,
                                        TipoInvalidez = new TipoInvalidez { Id = tipoInvalidez },
                                        flagRenta = flagRenta,
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };
                                    if (apellidoPaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoPaterno = apellidoPaterno;
                                    }
                                    if (apellidoMaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoMaterno = apellidoMaterno;
                                    }
                                    if (nombres.Trim() != String.Empty)
                                    {
                                        gru.Nombre = nombres;
                                    }
                                    if (tipoIdentificacion.Trim() != "0")
                                    {
                                        gru.Identificacion.IdTipo = tipoIdentificacion;
                                    }
                                    if (numeroIdentificacion.Trim() != String.Empty)
                                    {
                                        //gru.Identificacion.Numero = Convert.ToInt32(numeroIdentificacion);
                                        gru.Identificacion.Numero = numeroIdentificacion;
                                    }
                                    if (fechaInvalidez.Trim() != String.Empty)
                                    {
                                        gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
                                    }

                                    //<INIGTI_7012>
                                    if (nacionalidad.Trim() != "0")
                                    {
                                        gru.Nacionalidad = new Temporal { cod_parametro = nacionalidad };
                                    }
                                    else { gru.Nacionalidad = new Temporal { cod_parametro = "0" }; }

                                    if (profesion.Trim() != "0")
                                    {
                                        gru.Profesion = new Temporal { cod_parametro = profesion };
                                    }
                                    else { gru.Profesion = new Temporal { cod_parametro = "0" }; }

                                    if (residencia.Trim() != "0")
                                    {
                                        gru.Residencia = new Temporal { cod_parametro = residencia };
                                    }
                                    else { gru.Residencia = new Temporal { cod_parametro = "0" }; }

                                    gru.ind_PEP = (PEP == "S") ? true : false;
                                    gru.ind_SujetoObligado = (sujetoObligado == "S") ? true : false;

                                    if (banco.Trim() != "0")
                                    {
                                        gru.Banco = new Parametro { Id = banco };
                                    }
                                    else { gru.Banco = new Parametro { Id = "0" }; }

                                    if (tipoBanco.Trim() != "00")
                                    {
                                        gru.TipoCtaBanco = new Parametro { Id = tipoBanco };
                                    }

                                    if (comunicacion.Trim() != "0")
                                    {
                                        gru.Comunicacion = new Parametro { Id = comunicacion };
                                    }
                                    else { gru.Comunicacion = new Parametro { Id = "0" }; }

                                    if (numeroBanco.Trim() != String.Empty)
                                    {
                                        gru.NumeroBanco = numeroBanco;
                                    }

                                    gru.Confidencialidaddatos = new Parametro { Id = confidencialidadDatos };

                                    //<FINGTI_7012>

                                    if (estadoCivil.Trim() != "0")
                                    {
                                        gru.estadoCivil = estadoCivil;
                                    }
                                    if (correoElectronico.Trim() != String.Empty)
                                    {
                                        gru.CorreoElectronico = correoElectronico;
                                    }
                                    if (centroLaboral.Trim() != String.Empty)
                                    {
                                        gru.centroLaboral = centroLaboral;
                                    }
                                    if (cargo.Trim() != String.Empty)
                                    {
                                        gru.cargo = cargo;
                                    }
                                    if (actividadEconomica.Trim() != String.Empty)
                                    {
                                        gru.actividadEconomica = actividadEconomica;
                                    }
                                    if (monedaIngreso.Trim() != "0")
                                    {
                                        gru.monedaIngreso = new Moneda { Id = monedaIngreso };
                                    }
                                    if (ingreso.Trim() != String.Empty)
                                    {
                                        gru.ingresoNeto = Convert.ToSingle(ingreso.ToString());
                                    }
                                    if (telefono.Trim() != String.Empty)
                                    {
                                        gru.telefono = telefono;
                                    }
                                    if (celular.Trim() != String.Empty)
                                    {
                                        gru.celular = celular;
                                    }
                                    if (origen_fondo.Trim() != String.Empty)
                                    {
                                        gru.OrigenFondo = new OrigenFondo() { declaracionJurada = origen_fondo };
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    string num_solicitud = (String)HttpContext.Current.Session["Soltud"];

                                    gru.SolicitudRPPlus = new SolicitudRPPlus { Id = "" };

                                    if (num_solicitud != "")
                                    {
                                        gru.SolicitudRPPlus = new SolicitudRPPlus { Id = num_solicitud };

                                        if (gru.ind_PEP == false && gru.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                                        {
                                            List<Parametro> lstTipoIdentificacion = servicioCotizador.ObtenerTipoIdentificacion(gru.Identificacion.IdTipo, "", "");
                                            gru.Identificacion.GlosaTipo = lstTipoIdentificacion.Find(p => p.Id == gru.Identificacion.IdTipo).Nombre;
                                            JsonCoincidenciaLN jsonCoincidencia = servicioCotizador.ObtenerCoincidenciaLN(gru);
                                            if (jsonCoincidencia != null)
                                            {
                                                if (jsonCoincidencia._meta.status == "SUCCESS")
                                                {
                                                    if (jsonCoincidencia.records.PEP != 0)
                                                    {
                                                        respuesta.Estado = Constante.COD_ERROR;
                                                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                                        respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                                        respuesta.Mensaje = "El Asegurado <strong>Si Es </strong> Persona Expuesta Politicamente.";
                                                        return respuesta;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    respuesta = servicioCotizador.ActualizarGrupoFamiliar(gru);

                                    //respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                    //respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                    //respuesta.Mensaje = "Grupo familiar modificado correctamente.";

                                    string nombreTerminal = String.Empty;
                                    try
                                    {
                                        nombreTerminal = String.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' })[0].ToString());
                                    }
                                    catch (Exception)
                                    {
                                        log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].",
                                            HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                    }

                                    nombreTerminal += HttpContext.Current.Request.UserAgent;

                                    servicioCotizador.RegistrarLog(new LogBD
                                    {
                                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                        NombreTerminal = nombreTerminal,
                                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                        NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                        Detalle = String.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "ActualizarGrupoFamiliar", Environment.NewLine, Environment.NewLine, "Grupo Familiar", JsonConvert.SerializeObject(gru)),
                                        IdTipoEvento = Enums.EventoLog.ModificarDatosBeneficiario.StringValue()
                                    });

                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.GrupoFamiliarActualizar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.ModificarGrupoFamiliar WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.ModificarGrupoFamiliar WebMethod");

                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio GrupoFamiliarAfiliadoCierre.ObtenerDatosGrupoFamiliar WebMethod");
                //<INI.GTI_7012_V13>
                string num_solicitud = "";
                //if ((int)HttpContext.Current.Session["EstadoSoltud"] == 1 || (int)HttpContext.Current.Session["EstadoSoltud"] == 2) //<INI.GTI_7012_20>
                //{
                //    num_solicitud = (string)HttpContext.Current.Session["Soltud"];
                //}
                num_solicitud = (string)HttpContext.Current.Session["Soltud"];
                //<FIN.GTI_7012_V13>
                GrupoFamiliar gru;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                gru = servicioCotizador.ObtenerDatosGrupoFamiliar(idGrupoFamiliar, num_solicitud);
                log.Debug("Fin GrupoFamiliarAfiliadoCierre.ObtenerDatosGrupoFamiliar WebMethod");

                return gru;
            }
        }

        private static bool ValidarGrupoFamiliar(List<String> errores, List<String> controles, string glsApellidoPaterno, string glsApellidoMaterno, string glsNombres, string idTipoIdentificacion, string glsNumeroIdentificacion, string idParentesco, string idSexo, string fecNacimiento, string idInvalidez, string idTipoInvalidez, string fecInvalidez)
        {
            bool esCorrecto = true;

            // Apellido Paterno
            bool apellidoPaterno = true;

            // Apellido Materno
            bool apellidoMaterno = true;

            // Nombres
            bool nombres = true;

            // Tipo de Indentificación
            bool tipoIdentificacion = true;

            // Número de Identificación
            bool numeroIdentificacion = true;
            if (glsNumeroIdentificacion.Trim().Length > 0)
            {
                if (!Regex.IsMatch(glsNumeroIdentificacion, @"^\d+$"))
                {
                    errores.Add("El campo <strong>Nro. de Identificación</strong> debe contener un valor numérico.");
                    numeroIdentificacion = false;
                }
            }

            // Parentesco
            bool parentesco = true;
            if (idParentesco == "0")
            {
                errores.Add("Ingrese el campo <strong>Parentesco</strong>. Dato Obligatorio.");
                parentesco = false;
            }

            // Sexo
            bool sexo = true;
            if (idSexo == "0")
            {
                errores.Add("Ingrese el campo <strong>Sexo</strong>. Dato Obligatorio.");
                sexo = false;
            }

            // Fecha de Nacimiento
            bool fechaNacimiento = true;
            if (fecNacimiento.Trim().Length == 0)
            {
                errores.Add("Ingrese el campo <strong>Fecha de Nacimiento</strong>. Dato Obligatorio.");
                fechaNacimiento = false;
            }
            else
            {
                DateTime vFechaNacimiento;
                if (!DateTime.TryParse(fecNacimiento, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaNacimiento))
                {
                    errores.Add("El campo <strong>Fecha de Nacimiento</strong> debe contener una fecha válida (dd/mm/aaaa).");
                    fechaNacimiento = false;
                }
                //<SRIINI17003>
                else
                {
                    if (vFechaNacimiento > DateTime.Now)
                    {
                        errores.Add("La <strong>Fecha de Nacimiento</strong> no puede ser mayor al día de hoy.");
                        fechaNacimiento = false;
                    }
                }
                //<SRIFIN17003>
            }

            // Invalidez
            bool invalidez = true;
            bool tipoInvalidez = true;
            bool fechaInvalidez = true;
            if (idInvalidez == "0")
            {
                errores.Add("Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.");
                invalidez = false;
            }
            else if (idInvalidez == Enums.Invalidez.No.StringValue())
            {
                if (idTipoInvalidez != Enums.TipoInvalidez.NoInvalido.StringValue())
                {
                    errores.Add("El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.");
                    tipoInvalidez = false;
                }
                if (fecInvalidez.Trim().Length > 0)
                {
                    errores.Add("El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.");
                    fechaInvalidez = false;
                }
            }
            else if (idInvalidez == Enums.Invalidez.Si.StringValue())
            {
                if (idTipoInvalidez != Enums.TipoInvalidez.Parcial.StringValue() && idTipoInvalidez != Enums.TipoInvalidez.Total.StringValue())
                {
                    errores.Add("El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.");
                    tipoInvalidez = false;
                }

                if (fecInvalidez.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.");
                    fechaInvalidez = false;
                }
                else
                {
                    DateTime vFechaInvalidez;
                    if (!DateTime.TryParse(fecInvalidez, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaInvalidez))
                    {
                        errores.Add("El campo <strong>Fecha de Invalidez</strong> debe contener una fecha válida (dd/mm/aaaa).");
                        fechaInvalidez = false;
                    }
                }
            }

            if (idTipoInvalidez == "0")
            {
                errores.Add("Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.");
                tipoInvalidez = false;
            }

            // Clases de controles
            if (!apellidoPaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!apellidoMaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!nombres) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!tipoIdentificacion) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!numeroIdentificacion) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!parentesco) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!sexo) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!fechaNacimiento) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!invalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!tipoInvalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!fechaInvalidez) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento & invalidez & tipoInvalidez & fechaInvalidez;

            return esCorrecto;
        }

        //<SOLFIN25621>

        //<INIGTI_7012>
        [WebMethod]
        public static List<Temporal> ObtenerListaGruposFamiliares(string idSolicitud, int posicion)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio GrupoFamiliarAfiliadoCierre.ObtenerListaGruposFamiliares WebMethod");
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                List<Temporal> listaCombobox = servicioCotizador.ListarGruposFamiliaresxSolicitud(idSolicitud);

                HttpContext.Current.Session["listGrupoFamiliar"] = listaCombobox;
                HttpContext.Current.Session["idGrupoFamiliar"] = listaCombobox[posicion].codigo;
                HttpContext.Current.Session["Cantidad"] = listaCombobox[posicion].item;
                HttpContext.Current.Session["Total"] = listaCombobox[posicion].cantidad;
                log.Debug("Fin GrupoFamiliarAfiliadoCierre.ObtenerListaGruposFamiliares WebMethod");

                return listaCombobox;

            }
        }

        [WebMethod]
        public static List<Parametro> ObtenerNroBancos(string tabla, string tipoBanco, string tipoCuenta)
        {
            log.Debug("Inicio GrupoFamiliarAfiliadoCierre.ObtenerNroBancos WebMethod");
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerNroBancos(tabla, tipoBanco, tipoCuenta);
            log.Debug("Fin GrupoFamiliarAfiliadoCierre.ObtenerNroBancos WebMethod");

            return listaParametro;

        }
        //<FINGTI_7012>

        //<INI.GTI_7012_11>
        [WebMethod]
        public static List<Parametro> ObtenerTipoCtaBancos(string banco, string id)
        {

            log.Debug("Inicio GrupoFamiliarAfiliadoCierre.ObtenerTipoCtaBancos WebMethod");
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerTipoCtaBancos(banco, id);
            log.Debug("Fin GrupoFamiliarAfiliadoCierre.ObtenerTipoCtaBancos WebMethod");

            return listaParametro;

        }
        //<INI.GTI_7012_11>

        [WebMethod]
        public static string CargarTablaPersonasVinculadasPEP(string numSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio GrupoFamiliarAfiliadoCierre.CargarTablaPersonasVinculadasPEP WebMethod");

                    List<GrupoFamiliar> PersonasPEP = new List<GrupoFamiliar>();
                    GrupoFamiliar PersonaPEP = new GrupoFamiliar();
                    var pagina = new Page();
                    var control = (TablaPersonasVinculadasPEP)pagina.LoadControl("~/Controles/TablaPersonasVinculadasPEP.ascx");

                    PersonaPEP.SolicitudRPPlus = new SolicitudRPPlus() { Id = numSolicitud };
                    PersonaPEP.Identificacion = new Identificacion();
                    PersonaPEP.Parentesco = new Parentesco();
                    PersonaPEP.Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] };

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    PersonasPEP = servicioCotizador.ObtenerPersonaVinculada(PersonaPEP);

                    control.PersonasPEP = PersonasPEP;

                    pagina.Controls.Add(control);

                    string html = "";
                    using (var sw = new StringWriter())
                    {
                        HttpContext.Current.Server.Execute(pagina, sw, false);
                        html = sw.ToString();
                    }

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.CargarTablaCotizaciones WebMethod");

                    return html;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    log.Debug("Fin MantenerSolicitud.CargarTablaCotizaciones WebMethod");
                    throw (ex);
                }
            }
        }

        private List<string> CargarNoParentescosPersonaVinculada()
        {
            List<string> lstNoParentesco = new List<string>();

            lstNoParentesco.Add(Enums.Parentesco.Afiliado.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Primo.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Otros.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Sobrino.StringValue());
            lstNoParentesco.Add(Enums.Parentesco.Hermano.StringValue());

            return lstNoParentesco;
        }

        [WebMethod]
        public static Respuesta InsertarPersonaVinculada(string tokenUsuario,
                                                        string numSolicitud,
                                                        string apellidoPaterno,
                                                        string apellidoMaterno,
                                                        string nombres,
                                                        string tipoIdentificacion,
                                                        string numeroIdentificacion,
                                                        string parentesco)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio GrupoFamiliarAfiliadoCierre.InsertarPersonaVinculada WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            if (ValidarPersonaVinculada(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    GrupoFamiliar gru = new GrupoFamiliar
                                    {
                                        SolicitudRPPlus = new SolicitudRPPlus(),
                                        Identificacion = new Identificacion(),
                                        Parentesco = new Parentesco(),
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };

                                    if (numSolicitud.Trim() != String.Empty)
                                    {
                                        gru.SolicitudRPPlus.Id = numSolicitud;
                                    }

                                    if (apellidoPaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoPaterno = apellidoPaterno;
                                    }

                                    if (apellidoMaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoMaterno = apellidoMaterno;
                                    }

                                    if (nombres.Trim() != String.Empty)
                                    {
                                        gru.Nombre = nombres;
                                    }

                                    if (tipoIdentificacion.Trim() != "0")
                                    {
                                        gru.Identificacion.IdTipo = tipoIdentificacion;
                                    }

                                    if (numeroIdentificacion.Trim() != String.Empty)
                                    {
                                        gru.Identificacion.Numero = numeroIdentificacion;
                                    }

                                    if (parentesco.Trim() != "0")
                                    {
                                        gru.Parentesco.Id = parentesco;
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.RegistrarPersonaVinculada(gru);
                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.GrupoFamiliarInsertar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.InsertarPersonaVinculada WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.InsertarPersonaVinculada WebMethod");

                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static Respuesta ModificarPersonaVinculada(string tokenUsuario,
                                                        int idPersonaVinculada,
                                                        string numSolicitud,
                                                        string apellidoPaterno,
                                                        string apellidoMaterno,
                                                        string nombres,
                                                        string tipoIdentificacion,
                                                        string numeroIdentificacion,
                                                        string parentesco)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio GrupoFamiliarAfiliadoCierre.ModificarPersonaVinculada WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();

                            if (ValidarPersonaVinculada(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                {
                                    GrupoFamiliar gru = new GrupoFamiliar
                                    {
                                        Id = idPersonaVinculada,
                                        SolicitudRPPlus = new SolicitudRPPlus(),
                                        Identificacion = new Identificacion(),
                                        Parentesco = new Parentesco(),
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                                    };

                                    if (numSolicitud.Trim() != String.Empty)
                                    {
                                        gru.SolicitudRPPlus.Id = numSolicitud;
                                    }

                                    if (apellidoPaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoPaterno = apellidoPaterno;
                                    }

                                    if (apellidoMaterno.Trim() != String.Empty)
                                    {
                                        gru.ApellidoMaterno = apellidoMaterno;
                                    }

                                    if (nombres.Trim() != String.Empty)
                                    {
                                        gru.Nombre = nombres;
                                    }

                                    if (tipoIdentificacion.Trim() != "0")
                                    {
                                        gru.Identificacion.IdTipo = tipoIdentificacion;
                                    }

                                    if (numeroIdentificacion.Trim() != String.Empty)
                                    {
                                        gru.Identificacion.Numero = numeroIdentificacion;
                                    }

                                    if (parentesco.Trim() != "0")
                                    {
                                        gru.Parentesco.Id = parentesco;
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    respuesta = servicioCotizador.ActualizarPersonaVinculada(gru);
                                }
                                else
                                {
                                    respuesta.Estado = Constante.COD_ERROR;
                                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                }
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.GrupoFamiliarInsertar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.ModificarPersonaVinculada WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.InsertarPersonaVinculada WebMethod");

                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static Respuesta EliminarPersonaVinculada(string tokenUsuario, int idPersonaVinculada)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio GrupoFamiliarAfiliadoCierre.EliminarPersonaVinculada WebMethod");

                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar))
                        {
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                            {
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                respuesta = servicioCotizador.EliminarPersonaVinculada(idPersonaVinculada, (string)HttpContext.Current.Session["Usuario"]);
                            }
                            else
                            {
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                            }
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.GrupoFamiliarActualizar.StringValue()));
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        }
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.EliminarPersonaVinculada WebMethod");
                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<string> { ex.Message });
                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.EliminarPersonaVinculada WebMethod");
                    return respuesta;
                }
            }
        }

        private static bool ValidarPersonaVinculada(List<String> errores, List<String> controles, string glsApellidoPaterno, string glsApellidoMaterno, string glsNombres, string idTipoIdentificacion, string glsNumeroIdentificacion, string idParentesco)
        {
            bool esCorrecto = true;

            // Apellido Paterno
            bool apellidoPaterno = true;

            // Apellido Materno
            bool apellidoMaterno = true;

            // Nombres
            bool nombres = true;

            // Tipo de Indentificación
            bool tipoIdentificacion = true;

            // Número de Identificación
            bool numeroIdentificacion = true;

            if (glsNumeroIdentificacion.Trim().Length > 0)
            {
                if (!Regex.IsMatch(glsNumeroIdentificacion, @"^\d+$"))
                {
                    errores.Add("El campo <strong>Nro. de Identificación</strong> debe contener un valor numérico.");
                    numeroIdentificacion = false;
                }
            }

            // Parentesco
            bool parentesco = true;
            if (idParentesco == "0")
            {
                errores.Add("Ingrese el campo <strong>Parentesco</strong>. Dato Obligatorio.");
                parentesco = false;
            }

            // Clases de controles
            if (!apellidoPaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!apellidoMaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!nombres) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!tipoIdentificacion) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!numeroIdentificacion) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!parentesco) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco;

            return esCorrecto;
        }

        [WebMethod]
        public static GrupoFamiliar ObtenerPersonaVinculada(int idPersonaVinculada)
        {

            log.Debug("Inicio GrupoFamiliarAfiliadoCierre.ObtenerPersonaVinculada WebMethod");

            GrupoFamiliar PersonaPEP = new GrupoFamiliar();

            PersonaPEP.Id = idPersonaVinculada;
            PersonaPEP.SolicitudRPPlus = new SolicitudRPPlus();
            PersonaPEP.Identificacion = new Identificacion();
            PersonaPEP.Parentesco = new Parentesco();
            PersonaPEP.Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] };

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<GrupoFamiliar> PersonasPEP = servicioCotizador.ObtenerPersonaVinculada(PersonaPEP);

            log.Debug("Fin GrupoFamiliarAfiliadoCierre.ObtenerPersonaVinculada WebMethod");

            return PersonasPEP.FirstOrDefault();
        }

        [WebMethod]
        public static GrupoFamiliar ObtenerDatosBenefiCierre(int idGrupoFamiliar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio GrupoFamiliarAfiliadoCierre.ObtenerDatosBenefiCierre WebMethod");
                string num_solicitud = "";

                num_solicitud = (string)HttpContext.Current.Session["Soltud"];

                GrupoFamiliar gru;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                gru = servicioCotizador.ObtenerDatosBenefiCierre(idGrupoFamiliar, num_solicitud);
                log.Debug("Fin GrupoFamiliarAfiliadoCierre.ObtenerDatosBenefiCierre WebMethod");

                return gru;
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


    }
}