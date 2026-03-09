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
using System.Threading;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using log4net;
using Newtonsoft.Json;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
{
    public partial class GrupoFamiliarAfiliado : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GrupoFamiliarAfiliado));
        private static IServicioCWRV servicioCotizador;
        private static GrupoFamiliar grupo = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar))
                    {
                        if (!IsPostBack)
                        {

                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                            var listaParentesco = listaCombobox[(int)Enums.CategoriaCombobox.Parentesco];
                            
                            // Se carga la información inicial de la pantalla (llenado de Comoboxes)
                            CargarInformacionInicialPantalla();

                            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
                            {
                                HCUSPP.Value = Convert.ToString((Session["CUSPP"]));
                            }
                            else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                            {
                                HCUSPP.Value = Convert.ToString((Session["CUSPP_PLUS"]));
                            }

                            // Se almacena el número de Solicitud, el Id del Grupo Familiar y la página de origen
                            // en variables locales para no comprometer la volatilidad de la variable de sesión
                            IdSolicitud.Value = Session["idSolicitud"].ToString();
                            IdGrupoFamiliar.Value = Session["idGrupoFamiliar"].ToString();
                            PaginaLlamada.Value = Session["PaginaLlamada"].ToString();

                            // Para todos los casos de Renta Particular ocultar las columnas de Invalidez pues no se usan
                            LineaInvalidez.Visible = false;
                            LineaFechaInvalidez.Visible = false;

                            if (IdGrupoFamiliar.Value == "0")
                            {
                                LineaBanco.Visible = false;
                                LineaNumeroCuenta.Visible = false;
                                LineaCorreoElectronico.Visible = false;
                                LineaEstadocivil.Visible = false;
                                LineaCentroLaboralCargo.Visible = false;
                                LineaActividadEconomica.Visible = false;
                                LineaIngreso.Visible = false;
                                LineaTelefonoCelular.Visible = false;
                                
                                CargarCombobox(Parentesco, listaParentesco.Where(par => par.Id != Enums.Parentesco.Afiliado.StringValue()).ToList());
                            }
                            else
                            {
                                // Se obtienen los datos del beneficiario para cargarlos en la pantalla
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                grupo = servicioCotizador.ObtenerDatosGrupoFamiliar(Convert.ToInt32(IdGrupoFamiliar.Value), IdSolicitud.Value);
                                HCUSPP.Value = grupo.Afiliado.CUSPP;

                                // Ocultar líneas en beneficiarios
                                if (grupo.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                                {
                                    LineaBanco.Visible = false;
                                    LineaNumeroCuenta.Visible = false;
                                    LineaComunicacion.Visible = false;
                                    LineaCorreoElectronico.Visible = false;
                                }

                                if (IdSolicitud.Value.Length > 0)
                                {
                                    // Ocultar líneas que vienen del CRM cuando es Fuerza de ventas
                                    if (grupo.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue() || grupo.SolicitudIFP.OrigenCotizacion == Enums.OrigenCotizacion.Interseguro.StringValue())
                                    {
                                        LineaEstadocivil.Visible = false;
                                        LineaCentroLaboralCargo.Visible = false;
                                        LineaActividadEconomica.Visible = false;
                                        LineaIngreso.Visible = false;
                                        LineaTelefonoCelular.Visible = false;
                                        LineaCorreoElectronico.Visible = false;
                                    }
                                    else
                                    {
                                        LineaEstadocivil.Visible = true;
                                        LineaCentroLaboralCargo.Visible = true;
                                        LineaActividadEconomica.Visible = true;
                                        LineaIngreso.Visible = true;
                                        LineaTelefonoCelular.Visible = true;
                                        LineaCorreoElectronico.Visible = true;
                                    }
                                }
                                else
                                {
                                    LineaEstadocivil.Visible = false;
                                    LineaCentroLaboralCargo.Visible = false;
                                    LineaActividadEconomica.Visible = false;
                                    LineaIngreso.Visible = false;
                                    LineaTelefonoCelular.Visible = false;
                                    LineaCorreoElectronico.Visible = false;
                                }
                                
                                // Bloquear la Fecha de Nacimiento, Sexo y Parentesco del Afiliado
                                // NOTA: Se deja libre los beneficiarios mientras no se implementen
                                //       los planes que no son full garantizados, cuando esto ocurra
                                //       debe bloquearse esta opción para los beneficiarios también
                                if (grupo.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                                {
                                    Parentesco.Enabled = false;
                                    Parentesco.CssClass = "formCombobox formComboboxReadOnly";

                                    Sexo.Enabled = false;
                                    Sexo.CssClass = "formCombobox formComboboxReadOnly";

                                    FechaNacimiento.ReadOnly = true;
                                    FechaNacimiento.CssClass = "formTextbox formTextboxReadOnly";

                                    CargarCombobox(Parentesco, listaParentesco);
                                }
                                else
                                {
                                    CargarCombobox(Parentesco, listaParentesco.Where(par => par.Id != Enums.Parentesco.Afiliado.StringValue()).ToList());
                                }

                                CompletarFormulario(grupo);
                            }

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

        private void CompletarFormulario(GrupoFamiliar grupo)
        {
            ApellidoPaterno.Text = grupo.ApellidoPaterno;
            ApellidoMaterno.Text = grupo.ApellidoMaterno;
            Nombres.Text = grupo.Nombre;
            TipoIdentificacion.SelectedIndex = TipoIdentificacion.Items.IndexOf(TipoIdentificacion.Items.FindByValue(grupo.Identificacion.IdTipo));
            NumeroIdentificacion.Text = grupo.Identificacion.Numero;
            Parentesco.SelectedIndex = Parentesco.Items.IndexOf(Parentesco.Items.FindByValue(grupo.Parentesco.Id));
            Sexo.SelectedIndex = Sexo.Items.IndexOf(Sexo.Items.FindByValue(grupo.Sexo.ToString()));
            FechaNacimiento.Text = (grupo.FechaNacimiento != null ? ((DateTime)grupo.FechaNacimiento).ToString("dd/MM/yyyy") : string.Empty);
            IndInvalidez.SelectedIndex = IndInvalidez.Items.IndexOf(IndInvalidez.Items.FindByValue(grupo.Invalido ? "S" : "N"));
            TipoInvalidez.SelectedIndex = TipoInvalidez.Items.IndexOf(TipoInvalidez.Items.FindByValue(grupo.TipoInvalidez.Id));
            FechaInvalidez.Text = (grupo.FechaInvalidez != null ? ((DateTime)grupo.FechaInvalidez).ToString("dd/MM/yyyy") : string.Empty);
            Nacionalidad.SelectedIndex = Nacionalidad.Items.IndexOf(Nacionalidad.Items.FindByValue(grupo.Nacionalidad.cod_parametro));
            Profesion.SelectedIndex = Profesion.Items.IndexOf(Profesion.Items.FindByValue(grupo.Profesion.cod_parametro));
            Residencia.SelectedIndex = Residencia.Items.IndexOf(Residencia.Items.FindByValue(grupo.Residencia.cod_parametro));
            PEP.SelectedIndex = PEP.Items.IndexOf(PEP.Items.FindByValue(grupo.ind_PEP ? "S" : "N"));
            SO.SelectedIndex = SO.Items.IndexOf(SO.Items.FindByValue(grupo.ind_SujetoObligado ? "S" : "N"));
            Banco.SelectedIndex = Banco.Items.IndexOf(Banco.Items.FindByValue(grupo.Banco.Id));
            CargarComboboxTipoCuenta(TipoCtaBanco, grupo.Banco.Id);
            TipoCtaBanco.SelectedIndex = TipoCtaBanco.Items.IndexOf(TipoCtaBanco.Items.FindByValue(grupo.TipoCtaBanco.Id));
            HTipoCtaBanco.Value = grupo.TipoCtaBanco.Id;
            NumeroCuenta.Text = grupo.NumeroBanco;

            //Comunicacion.SelectedIndex = Comunicacion.Items.IndexOf(Comunicacion.Items.FindByValue(grupo.Comunicacion.Id));
            Comunicacion.SelectedIndex = Comunicacion.Items.IndexOf(Comunicacion.Items.FindByValue("1"));
            Comunicacion.Enabled = false;
            Comunicacion.CssClass = "formCombobox formComboboxReadOnly";
            
            ConfidencialidadDatos.SelectedIndex = ConfidencialidadDatos.Items.IndexOf(ConfidencialidadDatos.Items.FindByValue(grupo.Confidencialidaddatos.Id));
            CorreoElectronico.Text = grupo.CorreoElectronico;

            // Campos para Inteligo
            if (grupo.SolicitudIFP != null)
            {
                if (grupo.SolicitudIFP.OrigenCotizacion == Enums.OrigenCotizacion.Inteligo.StringValue())
                {
                    EstadoCivil.SelectedIndex = EstadoCivil.Items.IndexOf(EstadoCivil.Items.FindByValue(grupo.estadoCivil));
                    CentroLaboral.Text = grupo.centroLaboral;
                    Cargo.Text = grupo.cargo;
                    ActividadEconomica.Text = grupo.actividadEconomica;
                    MonedaIngreso.SelectedIndex = MonedaIngreso.Items.IndexOf(MonedaIngreso.Items.FindByValue(grupo.monedaIngreso.Id));
                    IngresoNeto.Text = grupo.ingresoNeto.ToString();
                    Telefono.Text = grupo.telefono;
                    Celular.Text = grupo.celular;
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

            var usuario = (string)Session["Usuario"];

            CargarCombobox(TipoIdentificacion, listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);
            //CargarCombobox(Parentesco, listaCombobox[(int)Enums.CategoriaCombobox.Parentesco]);
            CargarCombobox(Sexo, listaCombobox[(int)Enums.CategoriaCombobox.Sexo]);
            CargarCombobox(TipoInvalidez, listaCombobox[(int)Enums.CategoriaCombobox.Invalidez]);

            CargarComboboxNuevasEntidadesDatos(Profesion, "Profesion", usuario);
            CargarComboboxNuevasEntidadesDatos(Nacionalidad, "Nacionalidad", usuario);
            CargarComboboxNuevasEntidadesDatos(Residencia, "Residencia", usuario);

            CargarComboboxNuevosDatos(Banco, "BANCO");
            CargarComboboxNuevosDatos(Comunicacion, "COMUNICACION");

            PEP.Items.Add(new ListItem("«Seleccione»", "0"));
            PEP.Items.Add(new ListItem("Sí", "S"));
            PEP.Items.Add(new ListItem("No", "N"));

            SO.Items.Add(new ListItem("«Seleccione»", "0"));
            SO.Items.Add(new ListItem("Sí", "S"));
            SO.Items.Add(new ListItem("No", "N"));

            List<ParametroGeneral> comboConfidencialidadDatos = new List<ParametroGeneral>();
            comboConfidencialidadDatos.Add(new ParametroGeneral { Codigo = "0", Descripcion = "Sí" });
            comboConfidencialidadDatos.Add(new ParametroGeneral { Codigo = "1", Descripcion = "No" });

            ConfidencialidadDatos.Items.Clear();

            foreach (ParametroGeneral item in comboConfidencialidadDatos)
            {
                ConfidencialidadDatos.Items.Add(new ListItem(item.Descripcion, item.Codigo));
            }

            IndInvalidez.Items.Add(new ListItem("«Seleccione»", "0"));
            IndInvalidez.Items.Add(new ListItem("Sí", "S"));
            IndInvalidez.Items.Add(new ListItem("No", "N"));

            TipoCtaBanco.Items.Add(new ListItem("«Seleccione»", "0"));

            CargarComboboxNuevosDatosenBlanco(EstadoCivil, "EstadoCivil", usuario);
            CargarCombobox(MonedaIngreso, listaCombobox[(int)Enums.CategoriaCombobox.MonedaRentaPrivada]);
        }

        private void CargarComboboxNuevosDatosenBlanco(DropDownList control, string tabla, string usuario)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("", "0"));

            List<EstadoCivil> listaCombobox = null;

            if (tabla == "EstadoCivil")
            {
                listaCombobox = servicioCotizador.ListarEstadoCivil(usuario);

                foreach (EstadoCivil item in listaCombobox)
                {
                    control.Items.Add(new ListItem(item.gls_estado_civil, item.cod_estado_civil));
                }
            }
            
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
        private void CargarComboboxNuevasEntidadesDatos(DropDownList control, string tabla, string usuario)
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
                List<Profesion> profesiones = servicioCotizador.ListarProfesion(usuario);

                foreach (Profesion item in profesiones)
                {
                    control.Items.Add(new ListItem(item.gls_profesion, item.cod_profesion));
                }
            }
            else if (tabla == "Nacionalidad")
            {
                List<Nacionalidad> listaCombobox = servicioCotizador.ListarNacionalidad(usuario);
                
                foreach (Nacionalidad item in listaCombobox)
                {
                    control.Items.Add(new ListItem(item.gls_nacionalidad, item.cod_nacionalidad));
                }
            }
            else if (tabla == "Residencia")
            {
                string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                JArray listaDepartamentos = new JArray();
                var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
                urlDepartamentos = string.Format(urlDepartamentos, usuario);
                listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, usuario, listaDepartamentos);

                foreach (var item in listaDepartamentos)
                {
                    control.Items.Add(new ListItem(item["gls_departamento"].ToString().ToUpper(), item["id_departamento"].ToString().ToLower()));
                }
            }
            
        }

        private void CargarComboboxNuevosDatos(DropDownList control, string tabla)
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerParametros(tabla);

            control.Items.Clear();

            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAdicionalesGrupoFamiliar))
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            else
            {
                control.Items.Add(new ListItem("", "0"));
            }

            foreach (Parametro item in listaParametro)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }

        private void CargarComboboxTipoCuenta(DropDownList control, string idBanco)
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerTipoCtaBancos(idBanco, "");
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Parametro item in listaParametro)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }

        //<SOLINI25621>
        [WebMethod]
        public static String SessionIdGrupoFamiliar(int idGrupoFamiliar, string paginaLlamada)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                //<INI.GTI_7012_V13>
                HttpContext.Current.Session["Soltud"] = "";
                HttpContext.Current.Session["EstadoSoltud"] = "";
                //<FIN.GTI_7012_V13>
                HttpContext.Current.Session["idGrupoFamiliar"] = idGrupoFamiliar;
                HttpContext.Current.Session["PaginaLlamada"] = paginaLlamada;

                HttpContext.Current.Session["NroSolicitud"] = null;

                if (idGrupoFamiliar == 0)
                {
                    HttpContext.Current.Session["ModGruFamModo"] = "N";
                }
                else
                {
                    HttpContext.Current.Session["ModGruFamModo"] = "M";
                }

                HttpContext.Current.Session["idSolicitud"] = "";

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
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez, "", cuspp))
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
                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
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
                                                        string flagRenta,
                                                        string correoElectronico)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar))
                        {
                            List<String> errores = new List<String>();
                            List<String> controles = new List<String>();
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez, correoElectronico, cuspp))
                            {
                                //if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
                                //{
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

                                if (tipoBanco.Trim() != "0")
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

                                //YRV.INI
                                if (correoElectronico.Trim() != String.Empty)
                                {
                                    gru.CorreoElectronico = correoElectronico;
                                }
                                //YRV.FIN

                                //<FINGTI_7012>

                                //<INI.GTI_7012_V13>
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                //string num_solicitud = (String)HttpContext.Current.Session["Soltud"];
                                string num_solicitud = (String)HttpContext.Current.Session["idSolicitud"];
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

                                //}
                                //else
                                //{
                                //    respuesta.Estado = Constante.COD_ERROR;
                                //    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                //    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                //    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
                                //}

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
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(errores);
                                respuesta.Controles = controles;
                            }
                        }
                        else
                        {
                            log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
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
                    return respuesta;
                }
                catch (Exception ex)
                {
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                GrupoFamiliar gru;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                gru = servicioCotizador.ObtenerDatosGrupoFamiliar(idGrupoFamiliar, "");
                return gru;
            }
        }

        [WebMethod]
        public static GrupoFamiliar ObtenerDatosGrupoFamiliarSolicitud(int idGrupoFamiliar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                string numSolicitud = HttpContext.Current.Session["idSolicitud"].ToString();

                GrupoFamiliar gru;
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                gru = servicioCotizador.ObtenerDatosGrupoFamiliar(idGrupoFamiliar, numSolicitud);
                return gru;
            }
        }

        private static bool ValidarGrupoFamiliar(List<String> errores, List<String> controles, string glsApellidoPaterno, string glsApellidoMaterno, string glsNombres, string idTipoIdentificacion, string glsNumeroIdentificacion, string idParentesco, string idSexo, string fecNacimiento, string idInvalidez, string idTipoInvalidez, string fecInvalidez, string glsMail, string cuspp)
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(cuspp);

            var beneficiarioRepetido = grupos.FindAll(gf => gf.Identificacion.IdTipo == idTipoIdentificacion && gf.Identificacion.Numero == glsNumeroIdentificacion);

            if (beneficiarioRepetido.Count > 1)
            {
                errores.Add("Error al administrar los beneficiarios, hay números de DNI iguales.");
                return false;
            }

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

            //// Invalidez
            //bool invalidez = true;
            //bool tipoInvalidez = true;
            //bool fechaInvalidez = true;
            //if (idInvalidez == "0")
            //{
            //    errores.Add("Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.");
            //    invalidez = false;
            //}
            //else if (idInvalidez == Enums.Invalidez.No.StringValue())
            //{
            //    if (idTipoInvalidez != Enums.TipoInvalidez.NoInvalido.StringValue())
            //    {
            //        errores.Add("El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.");
            //        tipoInvalidez = false;
            //    }
            //    if (fecInvalidez.Trim().Length > 0)
            //    {
            //        errores.Add("El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.");
            //        fechaInvalidez = false;
            //    }
            //}
            //else if (idInvalidez == Enums.Invalidez.Si.StringValue())
            //{
            //    if (idTipoInvalidez != Enums.TipoInvalidez.Parcial.StringValue() && idTipoInvalidez != Enums.TipoInvalidez.Total.StringValue())
            //    {
            //        errores.Add("El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.");
            //        tipoInvalidez = false;
            //    }

            //    if (fecInvalidez.Trim().Length == 0)
            //    {
            //        errores.Add("Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.");
            //        fechaInvalidez = false;
            //    }
            //    else
            //    {
            //        DateTime vFechaInvalidez;
            //        if (!DateTime.TryParse(fecInvalidez, CultureInfo.CreateSpecificCulture("es-PE"), DateTimeStyles.None, out vFechaInvalidez))
            //        {
            //            errores.Add("El campo <strong>Fecha de Invalidez</strong> debe contener una fecha válida (dd/mm/aaaa).");
            //            fechaInvalidez = false;
            //        }
            //    }
            //}

            //if (idTipoInvalidez == "0")
            //{
            //    errores.Add("Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.");
            //    tipoInvalidez = false;
            //}

            //YRV.INI
            var correoElectronico = true;
            if (idParentesco == Enums.Parentesco.Afiliado.StringValue())
            {
                if (glsMail.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.");
                    correoElectronico = false;
                }
            }
            //YRV.FIN

            // Clases de controles
            if (!apellidoPaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!apellidoMaterno) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!nombres) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!tipoIdentificacion) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!numeroIdentificacion) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }
            if (!parentesco) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!sexo) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            if (!fechaNacimiento) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            //if (!invalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            //if (!tipoInvalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            //if (!fechaInvalidez) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!correoElectronico) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            //& invalidez & tipoInvalidez & fechaInvalidez

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento & correoElectronico;

            return esCorrecto;
        }

        protected void Regresar_Click(object sender, EventArgs e)
        {
            // Regresar a la página desde donde se invocó el formulario
            Response.Redirect(PaginaLlamada.Value);
        }

        protected void GuardarBeneficiario_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar))
                    {
                        List<string> errores = new List<string>();
                        List<string> controles = new List<string>();
                        //if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez, correoElectronico))
                        
                        if (true)
                        {
                            if (IdGrupoFamiliar.Value == "0")
                            {
                                grupo = new GrupoFamiliar();
                            }

                            grupo.Id = Convert.ToInt32(IdGrupoFamiliar.Value);

                            if (HCUSPP.Value.Length > 0)
                            {
                                grupo.Afiliado = new Afiliado { CUSPP = HCUSPP.Value };
                            }
                            else
                            {
                                grupo.Afiliado = new Afiliado { CUSPP = Session["SHCUSPP"].ToString() };
                            }

                            //validar DNI repetidos
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(grupo.Afiliado.CUSPP);

                            if (IdGrupoFamiliar.Value == "0")
                            {
                                var beneficiarioRepetido = grupos.FindAll(gf => gf.Identificacion.IdTipo == TipoIdentificacion.SelectedValue && gf.Identificacion.Numero == NumeroIdentificacion.Text);

                                if (beneficiarioRepetido.Count > 0)
                                {
                                    log.Error("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                    throw new Exception("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                }
                            }
                            else
                            {
                                var beneficiarioRepetido = grupos.FindAll(gf => gf.Identificacion.IdTipo == TipoIdentificacion.SelectedValue && gf.Identificacion.Numero == NumeroIdentificacion.Text && gf.Id.ToString() != IdGrupoFamiliar.Value);

                                if (beneficiarioRepetido.Count == 1)
                                {
                                    log.Error("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                    throw new Exception("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                }
                            }

                            grupo.Identificacion = new Identificacion();
                            grupo.Parentesco = new Parentesco { Id = Parentesco.SelectedValue };
                            grupo.Sexo = Convert.ToChar(Sexo.SelectedValue);
                            grupo.FechaNacimiento = Convert.ToDateTime(FechaNacimiento.Text, new CultureInfo("es-PE"));
                            grupo.Invalido = (IndInvalidez.SelectedValue == "S") ? true : false;
                            grupo.TipoInvalidez = new TipoInvalidez { Id = TipoInvalidez.SelectedValue };
                            grupo.flagRenta = Enums.TipoProducto.IFP.StringValue();
                            grupo.Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] };

                            if (ApellidoPaterno.Text.Trim() != string.Empty)
                            {
                                grupo.ApellidoPaterno = ApellidoPaterno.Text;
                            }
                            if (ApellidoMaterno.Text.Trim() != string.Empty)
                            {
                                grupo.ApellidoMaterno = ApellidoMaterno.Text;
                            }
                            if (Nombres.Text.Trim() != string.Empty)
                            {
                                grupo.Nombre = Nombres.Text;
                            }
                            if (TipoIdentificacion.SelectedValue.Trim() != "0")
                            {
                                grupo.Identificacion.IdTipo = TipoIdentificacion.SelectedValue;
                            }
                            if (NumeroIdentificacion.Text.Trim() != String.Empty)
                            {
                                grupo.Identificacion.Numero = NumeroIdentificacion.Text;
                            }
                            if (FechaInvalidez.Text.Trim() != String.Empty)
                            {
                                grupo.FechaInvalidez = Convert.ToDateTime(FechaInvalidez.Text, new CultureInfo("es-PE"));
                            }
                            if (Nacionalidad.SelectedValue.Trim() != "0")
                            {
                                grupo.Nacionalidad = new Temporal { cod_parametro = Nacionalidad.SelectedValue };
                            }
                            else
                            {
                                grupo.Nacionalidad = new Temporal { cod_parametro = "0" };
                            }

                            if (Profesion.SelectedValue.Trim() != "0")
                            {
                                grupo.Profesion = new Temporal { cod_parametro = Profesion.SelectedValue };
                            }
                            else { grupo.Profesion = new Temporal { cod_parametro = "0" }; }

                            if (Residencia.SelectedValue.Trim() != "0")
                            {
                                grupo.Residencia = new Temporal { cod_parametro = Residencia.SelectedValue };
                            }
                            else
                            {
                                grupo.Residencia = new Temporal { cod_parametro = "0" };
                            }
                            grupo.ind_PEP = (PEP.SelectedValue == "S") ? true : false;
                            grupo.ind_SujetoObligado = (SO.SelectedValue == "S") ? true : false;
                            if (Banco.SelectedValue.Trim() != "0")
                            {
                                grupo.Banco = new Parametro { Id = Banco.SelectedValue };
                            }
                            else
                            {
                                grupo.Banco = new Parametro { Id = "" };
                            }
                            if (HTipoCtaBanco.Value.Trim() != "0")
                            {
                                grupo.TipoCtaBanco = new Parametro { Id = HTipoCtaBanco.Value };
                            }
                            else
                            {
                                grupo.TipoCtaBanco = new Parametro { Id = "" };
                            }
                            if (Comunicacion.SelectedValue.Trim() != "0")
                            {
                                grupo.Comunicacion = new Parametro { Id = Comunicacion.SelectedValue };
                            }
                            else
                            {
                                grupo.Comunicacion = new Parametro { Id = "0" };
                            }
                            if (NumeroCuenta.Text.Trim() != string.Empty)
                            {
                                grupo.NumeroBanco = NumeroCuenta.Text;
                            }
                            grupo.Confidencialidaddatos = new Parametro { Id = ConfidencialidadDatos.SelectedValue };
                            if (CorreoElectronico.Text.Trim() != String.Empty)
                            {
                                grupo.CorreoElectronico = CorreoElectronico.Text;
                            }

                            //
                            grupo.estadoCivil = EstadoCivil.SelectedValue;
                            grupo.centroLaboral = CentroLaboral.Text;
                            grupo.cargo = Cargo.Text;
                            grupo.actividadEconomica = ActividadEconomica.Text;
                            grupo.monedaIngreso = new Moneda { Id = MonedaIngreso.SelectedValue };
                            if (IngresoNeto.Text.Trim() != string.Empty)
                            {
                                grupo.ingresoNeto = Convert.ToSingle(IngresoNeto.Text, new CultureInfo("es-PE"));
                            }
                            grupo.telefono = Telefono.Text;
                            grupo.celular = Celular.Text;

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            grupo.SolicitudRPPlus = new SolicitudRPPlus { Id = "" };
                            if (IdSolicitud.Value != "")
                            {
                                grupo.SolicitudRPPlus = new SolicitudRPPlus { Id = IdSolicitud.Value };

                                if (grupo.ind_PEP == false && grupo.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                                {
                                    List<Parametro> lstTipoIdentificacion = servicioCotizador.ObtenerTipoIdentificacion(grupo.Identificacion.IdTipo, "", "");
                                    grupo.Identificacion.GlosaTipo = lstTipoIdentificacion.Find(p => p.Id == grupo.Identificacion.IdTipo).Nombre;
                                    JsonCoincidenciaLN jsonCoincidencia = servicioCotizador.ObtenerCoincidenciaLN(grupo);
                                    if (jsonCoincidencia != null)
                                    {
                                        if (jsonCoincidencia._meta.status == "SUCCESS")
                                        {
                                            if (jsonCoincidencia.records.PEP != 0)
                                            {
                                                MCMMensaje.Text = "El Asegurado <strong>sí es</strong> Persona Expuesta Politicamente, por favor corregir.";
                                                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                                                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                                                MCMEstado.Value = "1";
                                            }
                                        }

                                    }
                                }
                            }

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


                            if (IdGrupoFamiliar.Value == "0")
                            {
                                respuesta = servicioCotizador.RegistrarGrupoFamiliar(grupo);
    
                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                    NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                    Detalle = String.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "RegistrarGrupoFamiliar", Environment.NewLine, Environment.NewLine, "Grupo Familiar", JsonConvert.SerializeObject(grupo)),
                                    IdTipoEvento = Enums.EventoLog.RegistrarDatosBeneficiario.StringValue()
                                });

                            }
                            else
                            {
                                respuesta = servicioCotizador.ActualizarGrupoFamiliar(grupo);

                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                    NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                    Detalle = String.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "ActualizarGrupoFamiliar", Environment.NewLine, Environment.NewLine, "Grupo Familiar", JsonConvert.SerializeObject(grupo)),
                                    IdTipoEvento = Enums.EventoLog.ModificarDatosBeneficiario.StringValue()
                                });

                            }

                            CompletarFormulario(grupo);

                            if (respuesta.Estado == Constante.COD_OK)
                            {
                                MCMMensaje.Text = "Datos de beneficiario actualizados correctamente.";
                                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Exito.StringValue();
                                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                MCMEstado.Value = "1";
                            }
                            else
                            {
                                MCMMensaje.Text = string.Format("Se ha producido un error al guardar el beneficiario:<br>{0}", respuesta.Mensaje);
                                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                                MCMEstado.Value = "1";
                            }
                        }
                        else
                        {
                            MCMMensaje.Text = Utilitarios.FormatearError(errores);
                            MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Validacion.StringValue();
                            MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                            MCMEstado.Value = "1";
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.GrupoFamiliarActualizar.StringValue()));

                        MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstado.Value = "1";
                    }
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
        //<SOLFIN25621>
    
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
