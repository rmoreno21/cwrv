using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel;
using System.Configuration;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Threading;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;

//<SRI.INI-20322_E2>
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
//<SRI.INI-20322_E2>

using Microsoft.Reporting.WebForms;

using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
{
    public partial class GrupoFamiliarAfiliado : System.Web.UI.Page
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
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.MenuCotizador))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                            //LimpiarFormularios();

                            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
                            {
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP"]));
                                ModGruFamModo_RP.Value = Convert.ToString((Session["ModGruFamModo"]));
                                ModIdGrupoFamiliar.Value = Convert.ToString((Session["idGrupoFamiliar"]));

                            }
                            else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                            {
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP_PLUS"]));
                                ModGruFamModo_RP.Value = Convert.ToString((Session["ModGruFamModo"]));
                                ModIdGrupoFamiliar.Value = Convert.ToString((Session["idGrupoFamiliar"]));
                            }
                            else
                            {
                                //<SOLINI25621>
                                //Response.Redirect("Cotizador.aspx");
                                Response.Redirect(Convert.ToString((Session["PaginaLlamada"])));
                                //<SOLFIN25621>
                            }

                            //<INI GTI_9155>
                            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAdicionalesGrupoFamiliar))
                            {
                                ComportamientoControl(ModGruFamNacional_RP, true);
                                ComportamientoControl(ModGruFamProfesion_RP, true);
                                ComportamientoControl(ModGruFamResidencia_RP, true);
                                ComportamientoControl(ModGruFamPEP_RP, true);
                                ComportamientoControl(ModGruFamSO_RP, true);
                                ComportamientoControl(ModGruFamBanco_RP, true);
                                ComportamientoControl(ModGruFamTipoCtaBanco_RP, true);
                                ComportamientoControl(ModGruFamComunicacion_RP, true);
                                ComportamientoControl(ModGruFamConfidencialidadDatos_RP, true);
                                ComportamientoControl(ModGruFamNumeroBanco_RP, true);
                            }
                            else
                            {
                                ComportamientoControl(ModGruFamNacional_RP, false);
                                ComportamientoControl(ModGruFamProfesion_RP, false);
                                ComportamientoControl(ModGruFamResidencia_RP, false);
                                ComportamientoControl(ModGruFamPEP_RP, false);
                                ComportamientoControl(ModGruFamSO_RP, false);
                                ComportamientoControl(ModGruFamBanco_RP, false);
                                ComportamientoControl(ModGruFamTipoCtaBanco_RP, false);
                                ComportamientoControl(ModGruFamComunicacion_RP, false);
                                ComportamientoControl(ModGruFamConfidencialidadDatos_RP, false);
                                ComportamientoControl(ModGruFamNumeroBanco_RP, false);
                            }
                            //<FIN GTI_9155>

                        }
                        else
                        {
                            // if (SaldoCIC.Text != String.Empty) SaldoCIC.Text = Convert.ToDouble(SaldoCIC.Text, new CultureInfo("es-PE")).ToString();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.MenuCotizador.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }

        //<INI GTI_9155>
        public void ComportamientoControl(Control control, bool habilitar)
        {
            if (control is TextBox)
            {
                if (habilitar)
                {
                    ((TextBox)control).ReadOnly = false;
                    ((TextBox)control).CssClass = "formTextbox";
                }
                else
                {
                    ((TextBox)control).ReadOnly = true;
                    ((TextBox)control).CssClass = "formTextbox formTextboxReadOnly formTextboxLetra ColorNegro";
                }
            }
            if (control is DropDownList)
            {

                if (habilitar)
                {
                    ((DropDownList)control).Enabled = true;
                    ((DropDownList)control).CssClass = "formCombobox";
                }
                else
                {
                    ((DropDownList)control).Enabled = false;
                    ((DropDownList)control).CssClass = "formComboboxTexto formTextboxReadOnly";
                }

            }
        }
        //<FIN GTI_9155>

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            

            

            CargarCombobox(ModGruFamTipoIdentificacion_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);

            List<string> lstNoParentesco = CargarNoParentescos();
            CargarCombobox(ModGruFamParentesco_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Parentesco].Where(p => !(lstNoParentesco.Contains(p.Id))).ToList());

            CargarCombobox(ModGruFamSexo_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);
            CargarCombobox(ModGruFamTipoInvalidez_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Invalidez]);

            //<INIGTI_7012>
            CargarComboboxNuevasEntidadesDatos(ModGruFamProfesion_RP, "Profesion");
            CargarComboboxNuevasEntidadesDatos(ModGruFamNacional_RP, "Nacionalidad");
            CargarComboboxNuevasEntidadesDatos(ModGruFamResidencia_RP, "Residencia");

            CargarComboboxNuevosDatos(ModGruFamBanco_RP, "BANCO");
            CargarComboboxNuevosDatos(ModGruFamComunicacion_RP, "COMUNICACION");

            //<INI GTI_9155>
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAdicionalesGrupoFamiliar))
            {
                ModGruFamPEP_RP.Items.Add(new ListItem("«Seleccione»", "0"));
                ModGruFamPEP_RP.Items.Add(new ListItem("Sí", "S"));
                ModGruFamPEP_RP.Items.Add(new ListItem("No", "N"));

                ModGruFamSO_RP.Items.Add(new ListItem("«Seleccione»", "0"));
                ModGruFamSO_RP.Items.Add(new ListItem("Sí", "S"));
                ModGruFamSO_RP.Items.Add(new ListItem("No", "N"));
            }
            else
            {
                ModGruFamPEP_RP.Items.Add(new ListItem("", "0"));
                ModGruFamPEP_RP.Items.Add(new ListItem("Sí", "S"));
                ModGruFamPEP_RP.Items.Add(new ListItem("No", "N"));

                ModGruFamSO_RP.Items.Add(new ListItem("", "0"));
                ModGruFamSO_RP.Items.Add(new ListItem("Sí", "S"));
                ModGruFamSO_RP.Items.Add(new ListItem("No", "N"));
            }
            //<FIN GTI_9155>



            List<ParametroGeneral> comboConfidencialidadDatos = new List<ParametroGeneral>();
            comboConfidencialidadDatos.Add(new ParametroGeneral { Codigo = "0", Descripcion = "Si" });
            comboConfidencialidadDatos.Add(new ParametroGeneral { Codigo = "1", Descripcion = "No" });

            ModGruFamConfidencialidadDatos_RP.Items.Clear();

            foreach (ParametroGeneral item in comboConfidencialidadDatos)
            {
                ModGruFamConfidencialidadDatos_RP.Items.Add(new ListItem(item.Descripcion, item.Codigo));
            }


            //<INIGTI_7012>

            ModGruFamIndInvalidez_RP.Items.Add(new ListItem("«Seleccione»", "0"));
            ModGruFamIndInvalidez_RP.Items.Add(new ListItem("Sí", "S"));
            ModGruFamIndInvalidez_RP.Items.Add(new ListItem("No", "N"));

            ModGruFamTipoCtaBanco_RP.Items.Add(new ListItem("«Seleccione»", "0"));

            //<SOLINI25621>
            ModPaginaLlamada.Value = Convert.ToString(Session["PaginaLlamada"]);
            //<SOLFIN25621>
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

            //<INI GTI_9155>
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAdicionalesGrupoFamiliar))
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            else
            {
                control.Items.Add(new ListItem("", "0"));
            }
            //<FIN GTI_9155>
            
            if (tabla == "Profesion")
            {
                List<Profesion> listaCombobox = null;
                listaCombobox = (List<Profesion>)Session["ListadoProfesion"];

                foreach (Profesion item in listaCombobox)
                {
                    control.Items.Add(new ListItem(item.gls_profesion, item.cod_profesion));
                }
            }
            else if (tabla == "Nacionalidad")
            {
                List<Nacionalidad> listaCombobox = null;
                listaCombobox = (List<Nacionalidad>)Session["ListadoNacionalidad"];

                foreach (Nacionalidad item in listaCombobox)
                {
                    control.Items.Add(new ListItem(item.gls_nacionalidad, item.cod_nacionalidad));
                }
            }
            else if (tabla == "Residencia")
            {
                JArray listaCombobox = new JArray();
                // listaCombobox = (JArray)Session["ListadoDepartamento"];
                var departamentos = (List<Departamento>)Session["ListadoDepartamento"];
                listaCombobox = JArray.FromObject(departamentos);

                foreach (var item in listaCombobox)
                {
                    control.Items.Add(new ListItem(item["gls_departamento"].ToString().ToUpper(), item["id_departamento"].ToString().ToLower()));
                }
            }
            
        }

        private void CargarComboboxNuevosDatos(DropDownList control, string tabla)
        {

            //List<Parametro> listaParametro = new List<Parametro>;

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerParametros(tabla);

            control.Items.Clear();

            //<INI GTI_9155>
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAdicionalesGrupoFamiliar))
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            else
            {
                control.Items.Add(new ListItem("", "0"));
            }
            //<FIN GTI_9155>

            foreach (Parametro item in listaParametro)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }

        }

        //<FINGTI_7012>

        //<SOLINI25621>
        [WebMethod]
        public static String SessionIdGrupoFamiliar(int idGrupoFamiliar, string paginaLlamada)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio GrupoFamiliarAfiliado.SessionIdGrupoFamiliar WebMethod");

                //<INI.GTI_7012_V13>
                HttpContext.Current.Session["Soltud"] = "";
                HttpContext.Current.Session["EstadoSoltud"] = "";
                //<FIN.GTI_7012_V13>
                HttpContext.Current.Session["idGrupoFamiliar"] = idGrupoFamiliar;
                HttpContext.Current.Session["PaginaLlamada"] = paginaLlamada;
                if (idGrupoFamiliar == 0)
                {
                    HttpContext.Current.Session["ModGruFamModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModGruFamModo"] = "M";
                }

                log.Debug("Fin GrupoFamiliarAfiliado.SessionIdGrupoFamiliar WebMethod");

                return idGrupoFamiliar.ToString();
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
                    log.Debug("Inicio GrupoFamiliarAfiliado.InsertarGrupoFamiliar WebMethod");

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

                                    //validar DNI repetidos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(cuspp);

                                    var beneficiarioRepetido = grupos.FindAll(gf => gf.Identificacion.IdTipo == tipoIdentificacion && gf.Identificacion.Numero == numeroIdentificacion);

                                    if (beneficiarioRepetido.Count > 0)
                                    {
                                        log.Error("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                        throw new Exception("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                    }


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

                    log.Debug("Fin GrupoFamiliarAfiliado.InsertarGrupoFamiliar WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin GrupoFamiliarAfiliado.InsertarGrupoFamiliar WebMethod");

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
                                                        string numerobanco,
                                                        string confidencialidadDatos,
                                                        string flagRenta)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio GrupoFamiliarAfiliado.ModificarGrupoFamiliar WebMethod");

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
                                    //validar DNI repetidos
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(cuspp);

                                    var beneficiarioRepetido = grupos.FindAll(gf => gf.Identificacion.IdTipo == tipoIdentificacion && gf.Identificacion.Numero == numeroIdentificacion && gf.Id.ToString() != idGrupoFamiliar);

                                    if (beneficiarioRepetido.Count == 1)
                                    {
                                        log.Error("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                        throw new Exception("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                    }

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
                                    else { gru.Banco = new Parametro { Id = "" }; } //<INI.GTI_7012_V13>

                                    if (tipoBanco.Trim() != "00")
                                    {
                                        gru.TipoCtaBanco = new Parametro { Id = tipoBanco };
                                    }
                                    else { gru.TipoCtaBanco = new Parametro { Id = "" }; }

                                    if (comunicacion.Trim() != "0")
                                    {
                                        gru.Comunicacion = new Parametro { Id = comunicacion };
                                    }
                                    else { gru.Comunicacion = new Parametro { Id = "0" }; }

                                    if (numerobanco.Trim() != String.Empty)
                                    {
                                        gru.NumeroBanco = numerobanco;
                                    }

                                    gru.Confidencialidaddatos = new Parametro { Id = confidencialidadDatos };

                                    //<FINGTI_7012>

                                    //<INI.GTI_7012_V13>
                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    string num_solicitud = (String)HttpContext.Current.Session["Soltud"];
                                    gru.SolicitudRPPlus = new SolicitudRPPlus { Id = "" };
                                    if (num_solicitud != "")
                                    {
                                        gru.SolicitudRPPlus = new SolicitudRPPlus { Id = num_solicitud };


                                        //<INI.GTI_7012_25>//Descomentar
                                        if (gru.ind_PEP == false && gru.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                                        //if (gru.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                                        {

                                            List<Parametro> lstTipoIdentificacion = servicioCotizador.ObtenerTipoIdentificacion(gru.Identificacion.IdTipo, "", "");
                                            gru.Identificacion.GlosaTipo = lstTipoIdentificacion.Find(p => p.Id == gru.Identificacion.IdTipo).Nombre;
                                            JsonCoincidenciaLN jsonCoincidencia = servicioCotizador.ObtenerCoincidenciaLN(gru);
                                            if (jsonCoincidencia != null)
                                            {
                                                if (jsonCoincidencia._meta.status == "SUCCESS")
                                                {
                                                    ////if (jsonCoincidencia.records.LN != 0)
                                                    ////{
                                                    ////    //<INI.GTI_7012_S19>
                                                    ////    Propuesta propuesta = new Propuesta();
                                                    ////    try
                                                    ////    {
                                                    ////        Contratante[] cont = (Contratante[])HttpContext.Current.Session["ListadoContratante"];

                                                    ////        propuesta.contratante_nombre1 = gru.Nombre;
                                                    ////        propuesta.contratante_apellido_paterno = gru.ApellidoPaterno;
                                                    ////        propuesta.contratante_apellido_materno = gru.ApellidoMaterno;
                                                    ////        propuesta.contratante_tipo_documento = gru.Identificacion.GlosaTipo;
                                                    ////        propuesta.contratante_documento = gru.Identificacion.Numero.ToString();

                                                    ////        if (gru.Identificacion.IdTipo=="D")
                                                    ////            propuesta.contratante_documento = gru.Identificacion.Numero.Value.ToString("00000000");
                                                    ////        else if (gru.Identificacion.IdTipo == "E")
                                                    ////            propuesta.contratante_documento = gru.Identificacion.Numero.Value.ToString("000000000");

                                                    ////        propuesta.contratante_fec_nacimiento = gru.FechaNacimiento.Value;

                                                    ////        propuesta.producto = ConfigurationManager.AppSettings["ProductoPlaftRPP"] + " - " + num_solicitud;

                                                    ////        ParametroGeneral[] lstProfesion = (ParametroGeneral[]) HttpContext.Current.Session["ListadoProfesion"];
                                                    ////        ParametroGeneral[] lstDepartamento= (ParametroGeneral[]) HttpContext.Current.Session["ListadoDepartamento"];

                                                    ////        propuesta.contratante_profesion = lstProfesion.Where(pr => pr.Codigo.ToUpper() == gru.Profesion.cod_parametro.ToUpper()).Select(prd => prd.Descripcion).FirstOrDefault();
                                                    ////        propuesta.contratante_residencia = lstDepartamento.Where(pr => pr.Codigo.ToUpper() == gru.Residencia.cod_parametro.ToUpper()).Select(prd => prd.Descripcion).FirstOrDefault();

                                                    ////        if (cont.Length > 0)
                                                    ////        {
                                                    ////            string simboloMoneda = "";

                                                    ////            //53AB0BC6-3C34-E211-B304-005056A6000F - Dólar de EE.UU.
                                                    ////            //B48CD55C-B034-E211-9DA0-005056A6000F - Nuevo Sol
                                                    ////            if (cont[0].monedaingreso.ToString().ToUpper() == "B48CD55C-B034-E211-9DA0-005056A6000F")
                                                    ////            {
                                                    ////                simboloMoneda = "S/ ";
                                                    ////            }
                                                    ////            else if (cont[0].monedaingreso.ToString().ToUpper() == "53AB0BC6-3C34-E211-B304-005056A6000F")
                                                    ////            {
                                                    ////                simboloMoneda = "US$ ";
                                                    ////            }
                                                    ////            propuesta.contratante_ingreso_mensual = simboloMoneda + String.Format("{0:#,##0.00}", Math.Round(Convert.ToDecimal(cont[0].ingreso.ToString()), 2));
                                                    ////            propuesta.contratante_centro_labores = cont[0].inter_centrolaboral.ToString();
                                                    ////            propuesta.contratante_cargo = cont[0].jobtitle.ToString();
                                                    ////        }

                                                    ////        SolicitudRPPlus solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(num_solicitud, DateTime.Now);
                                                    ////        if (solicitud != null) {
                                                    ////            propuesta.prima_anualizada = solicitud.PrimaUnica;
                                                    ////            propuesta.moneda = (solicitud.MonedaPrimaUnica.Id == Enums.Moneda.Soles.StringValue()) ? "S/ " : "US$ ";
                                                    ////        }

                                                    ////    }
                                                    ////    catch (Exception exPropuesta)
                                                    ////    {
                                                    ////        log.Error(exPropuesta.Message, exPropuesta);
                                                    ////    }
                                                    ////    finally
                                                    ////    {
                                                    ////        servicioCotizador.EnviarEmailListaNegra(propuesta);
                                                    ////    }
                                                    ////    //<FIN.GTI_7012_S19>

                                                    ////    respuesta.Estado = Constante.COD_ERROR;
                                                    ////    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                                    ////    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                                    ////    respuesta.Mensaje = "Este caso debe ser validado por el área PLAFT. Por favor contactarse con el área PLAFT.";
                                                    ////    return respuesta;
                                                    ////}
                                                    //if (jsonCoincidencia.records.PEP != 0 && gru.ind_PEP == false)
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
                                        //<FIN.GTI_7012_25>

                                    }
                                    //<FIN.GTI_7012_V13>


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

                    log.Debug("Fin GrupoFamiliarAfiliado.ModificarGrupoFamiliar WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });

                    log.Debug("Fin GrupoFamiliarAfiliado.ModificarGrupoFamiliar WebMethod");

                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio GrupoFamiliarAfiliado.ObtenerDatosGrupoFamiliar WebMethod");
                GrupoFamiliar gru;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                gru = servicioCotizador.ObtenerDatosGrupoFamiliar(idGrupoFamiliar, "");
                log.Debug("Fin GrupoFamiliarAfiliado.ObtenerDatosGrupoFamiliar WebMethod");
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

    }
}