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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Cotizador
{
    public partial class BeneficiarioCierre : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BeneficiarioCierre));
        private static IServicioCWRV servicioCotizador;
        public Beneficiario beneficiario;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionOficial))
                    {
                        if (!IsPostBack)
                        {
                            if (Request.QueryString["s"] != null && Request.QueryString["fc"] != null && Request.QueryString["c"] != null)
                            {
                                // Setear variables ocultas
                                Solicitud.Value = Request.QueryString["s"];
                                FechaCotizacion.Value = Request.QueryString["fc"];
                                Correlativo.Value = Request.QueryString["c"];

                                DateTime fechaCotizacion = Convert.ToDateTime(Request.QueryString["fc"], new CultureInfo("es-PE"));

                                // Obtener los datos de la solicitud
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                Solicitud solicitud = servicioCotizador.ObtenerDatosSolicitud(Request.QueryString["s"], fechaCotizacion);

                                // Obtener los datos del afiliado
                                Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(solicitud.Id, "", "", "", "");

                                if (Utilitarios.EsRolVerAgentesCesados((string)Session["RolAzman"]) || ((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == solicitud.Agente.Id))
                                {
                                    List<string> errores = new List<string>();
                                    List<string> controles = new List<string>();

                                    // Obtener beneficiarios desde la rvi_benefi
                                    var beneficiarios = servicioCotizador.ListarBeneficiarios(solicitud.Id, Session["Usuario"].ToString());

                                    // Filtrar el beneficiario por correlativo
                                    beneficiario = beneficiarios.Find(b => b.numCorrelativo == Convert.ToInt32(Request.QueryString["c"]));

                                    if (beneficiario != null)
                                    {
                                        // Obtener beneficiarios desde la cwrv_grupo_familiar
                                        var beneficiariosGF = servicioCotizador.ListarGrupoFamiliar(solicitud.Afiliado.CUSPP);

                                        // Seleccionar beneficiario
                                        if (servicioCotizador.SeleccionarBeneficiario(solicitud.Id, Request.QueryString["c"], Session["Usuario"].ToString()).Estado == Constante.COD_OK)
                                        {
                                            // Buscar el beneficiario de cwrv_grupo_familiar en los beneficiarios
                                            // de rvi_benefi por Parentesco, Sexo y Fecha de Nacimiento
                                            var beneficiarioGF = beneficiariosGF.Find(b =>
                                                b.Parentesco.Id == beneficiario.Parentesco.Id &
                                                b.Sexo == beneficiario.Sexo &
                                                b.FechaNacimiento == beneficiario.FechaNacimiento
                                            );

                                            if (beneficiarioGF != null)
                                            {
                                                string tipoBeneficiario;
                                                if (beneficiario.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                                                {
                                                    tipoBeneficiario = "Afiliado";
                                                }
                                                else
                                                {
                                                    tipoBeneficiario = "Beneficiario";
                                                }

                                                Titulo.Text = string.Format("Datos del {0}", tipoBeneficiario);
                                                bool apoderado = beneficiarioGF.IndTieneApoderado;
                                                Apoderado.Value = apoderado.ToString();

                                                // En caso de que no se tengan datos en rvi_benefi
                                                // obtenerlos desde cwrv_grupo_familiar
                                                if (string.IsNullOrEmpty(beneficiario.ApellidoPaterno.Trim()))
                                                {
                                                    beneficiario.ApellidoPaterno = beneficiarioGF.ApellidoPaterno;
                                                }
                                                if (string.IsNullOrEmpty(beneficiario.ApellidoMaterno.Trim()))
                                                {
                                                    beneficiario.ApellidoMaterno = beneficiarioGF.ApellidoMaterno;
                                                }
                                                if (string.IsNullOrEmpty(beneficiario.Nombre.Trim()))
                                                {
                                                    beneficiario.Nombre = beneficiarioGF.Nombre;
                                                }
                                                if (string.IsNullOrEmpty(beneficiario.Identificacion.Numero.Trim()) || beneficiario.Identificacion.Numero.Trim() == "0")
                                                {
                                                    beneficiario.Identificacion.Numero = beneficiarioGF.Identificacion.Numero;
                                                }
                                                if (beneficiario.Identificacion.IdTipo == "0" || beneficiario.Identificacion.IdTipo == "O")
                                                {
                                                    beneficiario.Identificacion.IdTipo = beneficiarioGF.Identificacion.IdTipo;
                                                }

                                                if (string.IsNullOrEmpty(beneficiario.numTelefono))
                                                {
                                                    if (!string.IsNullOrEmpty(beneficiarioGF.telefono))
                                                    {
                                                        beneficiario.numTelefono = beneficiarioGF.telefono;
                                                    }
                                                    else
                                                    {
                                                        beneficiario.numTelefono = afiliado.Telefonos;
                                                    }
                                                }
                                                if (string.IsNullOrEmpty(beneficiario.numCelular))
                                                {
                                                    if (!string.IsNullOrEmpty(beneficiarioGF.celular))
                                                    {
                                                        beneficiario.numCelular = beneficiarioGF.celular;
                                                    }
                                                    else
                                                    {
                                                        beneficiario.numCelular = afiliado.Celulares;
                                                    }
                                                }
                                                if (string.IsNullOrEmpty(beneficiario.CorreoElectronico))
                                                {
                                                    if (!string.IsNullOrEmpty(beneficiarioGF.CorreoElectronico))
                                                    {
                                                        beneficiario.CorreoElectronico = beneficiarioGF.CorreoElectronico;
                                                    }
                                                    else
                                                    {
                                                        beneficiario.CorreoElectronico = afiliado.CorreoElectronico;
                                                    }
                                                }
                                                if (string.IsNullOrEmpty(beneficiario.centroLaboral))
                                                {
                                                    beneficiario.centroLaboral = beneficiarioGF.centroLaboral;
                                                }

                                                if (apoderado)
                                                {
                                                    beneficiario.ApellidoPaterno = beneficiarioGF.ApellidoPaternoApdo;
                                                    beneficiario.ApellidoMaterno = beneficiarioGF.ApellidoMaternoApdo;
                                                    beneficiario.Nombre = beneficiarioGF.NombresApdo;
                                                    beneficiario.Identificacion.Numero = beneficiarioGF.IdentificacionApdo.Numero;
                                                    beneficiario.Identificacion.IdTipo = beneficiarioGF.IdentificacionApdo.IdTipo;
                                                }

                                                List<Direccion> direcciones = servicioCotizador.ListarRviDireccion(solicitud.Id, Session["Usuario"].ToString());
                                                Direccion direccionSolicitud = direcciones.Find(d => d.Principal);
                                                Direccion direccionEnvio = direcciones.Find(d => !d.Principal);
                                                List<Direccion> direccionesCUSPP = null;

                                                // Si las direcciones de benefi vienen en blanco se autocompletan con las de cwrv_direccion
                                                if (direccionSolicitud == null)
                                                {
                                                    direccionesCUSPP = servicioCotizador.ListarDireccion(solicitud.Afiliado.CUSPP);
                                                    if (direccionesCUSPP.Count > 0)
                                                    {
                                                        direccionSolicitud = direccionesCUSPP.Find(d => d.Principal);
                                                        if (direccionSolicitud == null)
                                                        {
                                                            direccionSolicitud = direccionesCUSPP.First();
                                                        }
                                                    }
                                                }

                                                if (direccionEnvio == null)
                                                {
                                                    direccionEnvio = direccionSolicitud;
                                                }

                                                CargarInformacionInicialPantalla(beneficiario, direccionSolicitud, direccionEnvio);
                                                CargarControles(beneficiario, direccionSolicitud, direccionEnvio);

                                                // Validar si la solicitud tiene firma digital aceptada
                                                FirmaDigital firma = servicioCotizador.ObtenerFirmaDigital(solicitud.Id, Convert.ToInt32(Request.QueryString["c"]), Session["usuario"].ToString());
                                                if (firma != null)
                                                {
                                                    FirmaDigital.Value = firma.id_firma_digital.ToString();
                                                }

                                                // Habilitar botón de Envío Manual si corresponde
                                                if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.PlantillaCorreoElectronico) && firma != null)
                                                {
                                                    EnvioManual.Visible = true;
                                                    EnvioManual.NavigateUrl = ResolveUrl(string.Format("~/Comun/PlantillaCorreoElectronico.aspx?tp=3&s={0}&fc={1}&i={2}", solicitud.Id, fechaCotizacion.ToString("yyyy-MM-dd"), Request.QueryString["c"]));
                                                }
                                            }
                                            else
                                            {
                                                // Mostrar Error
                                                log.Error(string.Format("Los datos del beneficiario seleccionado (parentesco, sexo o fecha de nacimiento) no coinciden con los del Grupo Familiar. Solicitud[{0}] Correlativo[{1}]", solicitud.Id, Request.QueryString["c"]));
                                                Response.Redirect("~/Error/500.aspx");
                                            }
                                        }
                                        else
                                        {
                                            log.Error(string.Format("Se produjo un error al actualizar el flag de Beneficiario seleccionado. Solicitud[{0}] Correlativo[{1}]", solicitud.Id, Request.QueryString["c"]));
                                            Response.Redirect("~/Error/500.aspx");
                                        }
                                    }
                                    else
                                    {
                                        log.Error(string.Format("No se ha encontrado datos del beneficiario. Solicitud[{0}] Correlativo[{1}]", solicitud.Id, Request.QueryString["c"]));
                                        Response.Redirect("~/Error/404.aspx");
                                    }
                                }
                                else
                                {
                                    log.Error(string.Format("Solicitud [{0}] no pertenece a la cartera de ventas de [{1}] o agente/cartera no está vigente.", solicitud.Id, solicitud.Agente.Id));
                                    Response.Redirect("~/Error/403.aspx");
                                }
                            }
                            else
                            {
                                // No ha accedido correctamente a esta opción, redirigir a la pantalla principal de RRVV
                                Response.Redirect("~/Cotizador/Cotizador.aspx");
                            }
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.CotizacionOficial.StringValue()));
                        Response.Redirect("~/Error/403.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error("Error de comunicación.", ex);
                    Response.Redirect("~/Error/500.aspx");
                }
                catch (Exception ex)
                {
                    log.Error("Se ha producido un error.", ex);
                    Response.Redirect("~/Error/500.aspx");
                }
            }
        }

        private void CargarInformacionInicialPantalla(Beneficiario beneficiario, Direccion direccionSolicitud, Direccion direccionEnvio)
        {
            string usuario = Session["Usuario"].ToString();

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
            CargarCombobox(TipoDocumento, listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);

            List<Parametro> tiposVia = servicioCotizador.ObtenerParametros("TIPOVIA");
            CargarCombobox(TipoVia, tiposVia);
            CargarCombobox(TipoViaEntrega, tiposVia);

            List<Departamento> departamentos = servicioCotizador.ListarDepartamentos(usuario);
            CargarComboboxDepartamento(Departamento, departamentos);
            CargarComboboxDepartamento(DepartamentoEntrega, departamentos);

            if (direccionSolicitud != null)
            {
                List<Provincia> provincias = servicioCotizador.ListarProvincias(direccionSolicitud.Departamento.Id, usuario);
                CargarComboboxProvincia(Provincia, provincias);

                List<Distrito> distritos = servicioCotizador.ListarDistritos(direccionSolicitud.Ciudad.Id, usuario);
                CargarComboboxDistrito(Distrito, distritos);
            }

            if (direccionEnvio != null)
            {
                List<Provincia> provincias = servicioCotizador.ListarProvincias(direccionEnvio.Departamento.Id, usuario);
                CargarComboboxProvincia(ProvinciaEntrega, provincias);

                List<Distrito> distritos = servicioCotizador.ListarDistritos(direccionEnvio.Ciudad.Id, usuario);
                CargarComboboxDistrito(DistritoEntrega, distritos);
            }

            EnvioPoliza.Items.Add(new ListItem("«Seleccione»", "0"));
            EnvioPoliza.Items.Add(new ListItem("Físico", "F"));
            EnvioPoliza.Items.Add(new ListItem("Digital", "D"));
        }

        private void CargarControles(Beneficiario beneficiario, Direccion direccionSolicitud, Direccion direccionEnvio)
        {
            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"];
            string usuario = HttpContext.Current.Session["Usuario"].ToString();

            // Tarjeta Datos del Afiliado o Beneficiario
            ApellidoPaterno.Text = beneficiario.ApellidoPaterno;
            ApellidoMaterno.Text = beneficiario.ApellidoMaterno;
            Nombres.Text = beneficiario.Nombre;
            TipoDocumento.SelectedIndex = TipoDocumento.Items.IndexOf(TipoDocumento.Items.FindByValue(beneficiario.Identificacion.IdTipo));
            NumeroDocumento.Text = beneficiario.Identificacion.Numero;
            Telefono.Text = beneficiario.numTelefono;
            Celular.Text = beneficiario.numCelular;
            CorreoElectronico.Text = beneficiario.CorreoElectronico;
            CentroLaboral.Text = beneficiario.centroLaboral;

            // Tarjeta Dirección de Solicitud
            if (direccionSolicitud != null)
            {
                TipoVia.SelectedIndex = TipoVia.Items.IndexOf(TipoVia.Items.FindByValue(direccionSolicitud.TipoVia.Id));
                Direccion.Text = direccionSolicitud.Glosa;
                EspacioUrbano.Text = direccionSolicitud.EspacioUrbano;
                Departamento.SelectedIndex = Departamento.Items.IndexOf(Departamento.Items.FindByValue(direccionSolicitud.Departamento.Id.ToUpper()));
                Provincia.SelectedIndex = Provincia.Items.IndexOf(Provincia.Items.FindByValue(direccionSolicitud.Ciudad.Id.ToUpper()));
                Distrito.SelectedIndex = Distrito.Items.IndexOf(Distrito.Items.FindByValue(direccionSolicitud.Comuna.Id.ToUpper()));
            }

            // Tarjeta Dirección de Envío
            EnvioPoliza.SelectedIndex = EnvioPoliza.Items.IndexOf(EnvioPoliza.Items.FindByValue(beneficiario.envioPoliza));
            PersonaAutorizadaEntrega.Text = beneficiario.direccionAlterna.personaAutorizada;
            if (direccionEnvio != null)
            {
                TipoViaEntrega.SelectedIndex = TipoViaEntrega.Items.IndexOf(TipoViaEntrega.Items.FindByValue(direccionEnvio.TipoVia.Id));
                DireccionEntrega.Text = direccionEnvio.Glosa;
                EspacioUrbanoEntrega.Text = direccionEnvio.EspacioUrbano;
                DepartamentoEntrega.SelectedIndex = DepartamentoEntrega.Items.IndexOf(DepartamentoEntrega.Items.FindByValue(direccionEnvio.Departamento.Id.ToUpper()));
                ProvinciaEntrega.SelectedIndex = ProvinciaEntrega.Items.IndexOf(ProvinciaEntrega.Items.FindByValue(direccionEnvio.Ciudad.Id.ToUpper()));
                DistritoEntrega.SelectedIndex = DistritoEntrega.Items.IndexOf(DistritoEntrega.Items.FindByValue(direccionEnvio.Comuna.Id.ToUpper()));
            }
        }

        [WebMethod]
        public static Beneficiario GuardarBeneficiario(
            string tokenUsuario, string numSolicitud, string fecCotizacion, int correlativo,
            string apellidoPaterno, string apellidoMaterno, string nombres, string tipoDocumento,
            string numeroDocumento, string telefono, string celular, string correoElectronico,
            string centroLaboral, bool apoderado, string tipoVia, string direccion,
            string espacioUrbano, string departamento, string provincia, string distrito,
            string envioPoliza, string personaAutorizadaEntrega, string tipoViaEntrega, string direccionEntrega,
            string espacioUrbanoEntrega, string departamentoEntrega, string provinciaEntrega, string distritoEntrega
        )
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionOficial))
                        {
                            DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));

                            // Obtener los datos de la solicitud
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Solicitud solicitud = servicioCotizador.ObtenerDatosSolicitud(numSolicitud, fechaCotizacion);

                            if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]) || ((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == solicitud.Agente.Id))
                            {
                                // Obtener los datos de RviBdIni
                                RviBdIni afiliado = servicioCotizador.ObtenerAseguradoBdIni(solicitud.Afiliado.CUSPP, HttpContext.Current.Session["Usuario"].ToString());
                                afiliado.num_cuspp = solicitud.Afiliado.CUSPP;
                                afiliado.gls_celular = celular;
                                afiliado.gls_telefono = telefono;
                                afiliado.gls_mail = correoElectronico;
                                afiliado.gls_centro_laboral = centroLaboral;
                                afiliado.aud_usr_modificacion = HttpContext.Current.Session["Usuario"].ToString();

                                // Obtener beneficiarios desde la rvi_benefi
                                var beneficiarios = servicioCotizador.ListarBeneficiarios(solicitud.Id, HttpContext.Current.Session["Usuario"].ToString());

                                // Filtrar el beneficiario por correlativo
                                Beneficiario beneficiario = beneficiarios.Find(b => b.numCorrelativo == correlativo);

                                beneficiario.ApellidoPaterno = apellidoPaterno;
                                beneficiario.ApellidoMaterno = apellidoMaterno;
                                beneficiario.Nombre = nombres;
                                beneficiario.Identificacion = new Identificacion()
                                {
                                    IdTipo = tipoDocumento,
                                    Numero = numeroDocumento
                                };
                                beneficiario.numTelefono = telefono;
                                beneficiario.numCelular = celular;
                                beneficiario.CorreoElectronico = correoElectronico;
                                beneficiario.centroLaboral = centroLaboral;
                                beneficiario.envioPoliza = envioPoliza;
                                beneficiario.usuario = HttpContext.Current.Session["Usuario"].ToString();
                                beneficiario.numSolicitud = numSolicitud;
                                beneficiario.numCorrelativo = correlativo;
                                beneficiario.direccionPrincipal = new BeneficiarioDireccion
                                {
                                    numSolicitud = numSolicitud,
                                    direccion = direccion,
                                    tipoVia = new Parametro
                                    {
                                        Id = tipoVia
                                    },
                                    espacioUrbano = espacioUrbano,
                                    departamento = new Departamento
                                    {
                                        Id = departamento
                                    },
                                    provincia = new Ciudad
                                    {
                                        Id = provincia
                                    },
                                    distrito = new Comuna
                                    {
                                        Id = distrito
                                    },
                                    tipo = Enums.TipoDireccion.Principal.StringValue(),
                                    vigencia = "S",
                                    usuario = HttpContext.Current.Session["Usuario"].ToString()
                                };
                                beneficiario.Apoderado = apoderado;

                                if (envioPoliza == "F")
                                {
                                    beneficiario.direccionAlterna = new BeneficiarioDireccion
                                    {
                                        numSolicitud = numSolicitud,
                                        direccion = direccionEntrega,
                                        tipoVia = new Parametro
                                        {
                                            Id = tipoViaEntrega
                                        },
                                        espacioUrbano = espacioUrbanoEntrega,
                                        departamento = new Departamento
                                        {
                                            Id = departamentoEntrega
                                        },
                                        provincia = new Ciudad
                                        {
                                            Id = provinciaEntrega
                                        },
                                        distrito = new Comuna
                                        {
                                            Id = distritoEntrega
                                        },
                                        personaAutorizada = personaAutorizadaEntrega,
                                        tipo = Enums.TipoDireccion.EnvioPoliza.StringValue(),
                                        vigencia = "S",
                                        usuario = HttpContext.Current.Session["Usuario"].ToString()
                                    };
                                }

                                // Obtener beneficiarios desde la cwrv_grupo_familiar
                                List<GrupoFamiliar> beneficiariosGF = servicioCotizador.ListarGrupoFamiliar(solicitud.Afiliado.CUSPP);

                                // Buscar el beneficiario de cwrv_grupo_familiar en los beneficiarios
                                // de rvi_benefi por Parentesco, Sexo y Fecha de Nacimiento
                                GrupoFamiliar beneficiarioGF = beneficiariosGF.Find(b =>
                                    b.Parentesco.Id == beneficiario.Parentesco.Id &
                                    b.Sexo == beneficiario.Sexo &
                                    b.FechaNacimiento == beneficiario.FechaNacimiento
                                );

                                beneficiarioGF.Afiliado = new Afiliado
                                {
                                    CUSPP = solicitud.Afiliado.CUSPP
                                };
                                if (!beneficiarioGF.IndTieneApoderado)
                                {
                                    beneficiarioGF.ApellidoPaterno = apellidoPaterno;
                                    beneficiarioGF.ApellidoMaterno = apellidoMaterno;
                                    beneficiarioGF.Nombre = nombres;
                                    beneficiarioGF.Identificacion.IdTipo = tipoDocumento;
                                    beneficiarioGF.Identificacion.Numero = numeroDocumento;
                                }
                                else
                                {
                                    beneficiarioGF.ApellidoPaternoApdo = apellidoPaterno;
                                    beneficiarioGF.ApellidoMaternoApdo = apellidoMaterno;
                                    beneficiarioGF.NombresApdo = nombres;
                                    beneficiarioGF.IdentificacionApdo.IdTipo = tipoDocumento;
                                    beneficiarioGF.IdentificacionApdo.Numero = numeroDocumento;
                                }
                                beneficiarioGF.telefono = telefono;
                                beneficiarioGF.celular = celular;
                                beneficiarioGF.CorreoElectronico = correoElectronico;
                                beneficiarioGF.centroLaboral = centroLaboral;
                                beneficiarioGF.Usuario = new Usuario
                                {
                                    NombreUsuario = HttpContext.Current.Session["Usuario"].ToString()
                                };

                                // Obtener dirección principal
                                List<Direccion> direccionesGF = servicioCotizador.ListarDireccion(solicitud.Afiliado.CUSPP);
                                Direccion direccionGF = direccionesGF.Find(d => d.Principal);
                                if (direccionGF == null)
                                {
                                    direccionGF = new Direccion
                                    {
                                        TipoVia = new Parametro
                                        {
                                            Id = tipoVia
                                        },
                                        Glosa = direccion,
                                        EspacioUrbano = espacioUrbano,
                                        Departamento = new Departamento
                                        {
                                            Id = departamento
                                        },
                                        Ciudad = new Ciudad
                                        {
                                            Id = provincia
                                        },
                                        Comuna = new Comuna
                                        {
                                            Id = distrito
                                        },
                                        Principal = true
                                    };
                                }
                                else
                                {
                                    direccionGF.TipoVia.Id = tipoVia;
                                    direccionGF.Glosa = direccion;
                                    direccionGF.EspacioUrbano = espacioUrbano;
                                    direccionGF.Departamento.Id = departamento;
                                    direccionGF.Ciudad.Id = provincia;
                                    direccionGF.Comuna.Id = distrito;
                                }
                                direccionGF.Afiliado = new Afiliado
                                {
                                    CUSPP = solicitud.Afiliado.CUSPP
                                };
                                direccionGF.Usuario = new Usuario
                                {
                                    NombreUsuario = HttpContext.Current.Session["Usuario"].ToString()
                                };

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                // Actualizar Beneficiario
                                respuesta = servicioCotizador.ActualizarBeneficiario(beneficiario);

                                // Actualizar Grupo Familiar
                                servicioCotizador.ActualizarGrupoFamiliar(beneficiarioGF);

                                // Actualizar Afiliado
                                servicioCotizador.ActualizarAseguradoBdIni(afiliado);

                                // Actualizar Dirección
                                if (direccionGF.Id != 0)
                                {
                                    servicioCotizador.ActualizarDireccion(direccionGF);
                                }
                                else
                                {
                                    servicioCotizador.RegistrarDireccion(direccionGF);
                                }

                                string nombreTerminal = string.Empty;
                                try
                                {
                                    nombreTerminal = string.Format("[{0}] ", Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["remote_addr"]).HostName.Split(new char[] { '.' })[0].ToString());
                                }
                                catch (Exception)
                                {
                                    log.Warn(string.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].", HttpContext.Current.Request.ServerVariables["remote_addr"]));
                                }
                                nombreTerminal += HttpContext.Current.Request.UserAgent;

                                servicioCotizador.RegistrarLog(new LogBD
                                {
                                    IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                    NombreTerminal = nombreTerminal,
                                    IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                    NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                    Detalle = string.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} ", "ActualizarBeneficiario", Environment.NewLine, Environment.NewLine, "beneficiario", JsonConvert.SerializeObject(beneficiario)),
                                    IdTipoEvento = Enums.EventoLog.ModificarDatosBeneficiario.StringValue()
                                });

                                if (respuesta.Estado == Constante.COD_ERROR)
                                {
                                    throw new Exception(respuesta.Mensaje);
                                }

                                return beneficiario;
                            }
                            else
                            {
                                log.Error(string.Format("Solicitud [{0}] no pertenece a la cartera de ventas de [{1}] o agente/cartera no está vigente.", numSolicitud, solicitud.Agente.Id));
                                HttpContext.Current.Response.Status = "403 Forbidden";
                                HttpContext.Current.Response.StatusCode = 403;
                                HttpContext.Current.ApplicationInstance.CompleteRequest();
                                return null;
                            }
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [GuardarBeneficiario] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
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
                    log.Error("Se ha producido un error al guardar los datos del beneficiario", ex);
                    throw ex;
                }
            }
        }

        [WebMethod]
        public static void EnviarFirmaDigital(string tokenUsuario, string numSolicitud, string fecCotizacion, int correlativo)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionOficial))
                        {
                            DateTime fechaCotizacion = Convert.ToDateTime(fecCotizacion, new CultureInfo("es-PE"));
                            string usuario = (string)HttpContext.Current.Session["Usuario"];

                            // Obtener datos de la solicitud
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Solicitud solicitud = servicioCotizador.ObtenerDatosSolicitud(numSolicitud, fechaCotizacion);

                            // Obtener al beneficiario de la solicitud
                            List<Beneficiario> beneficiarios = servicioCotizador.ListarBeneficiarios(numSolicitud, usuario);
                            Beneficiario beneficiario = beneficiarios.Find(ben => ben.numCorrelativo == correlativo);

                            // Si cuenta con apoderado, usar los nombres y documento del apoderado
                            if (beneficiario.Apoderado)
                            {
                                beneficiario.ApellidoPaterno = beneficiario.ApellidoPaternoApodero;
                                beneficiario.ApellidoMaterno = beneficiario.ApellidoMaternoApodero;
                                beneficiario.Nombre = beneficiario.NombreApodero;
                                beneficiario.Identificacion.Numero = beneficiario.IdentificacionApodero.Numero;
                                beneficiario.Identificacion.IdTipo = beneficiario.IdentificacionApodero.IdTipo;
                            }

                            // Sobreescritura de destinatario (ambientes de prueba)
                            string destinatarioCliente = ConfigurationManager.AppSettings["destinatario_consentimiento"].ToString();
                            if (destinatarioCliente != "N")
                            {
                                beneficiario.CorreoElectronico = destinatarioCliente;
                            }

                            // Obtener datos del agente y supervisor de la solicitud
                            List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                            Agente agente = listaAgentes.Find(a => a.Id == solicitud.Agente.Id);
                            if (agente == null)
                            {
                                // Si el agente es nulo es porque ya no está vigente validar si es un usuario que puede ver los agentes cesados
                                if (Utilitarios.EsRolVerAgentesCesados((string)HttpContext.Current.Session["RolAzman"]))
                                {
                                    // Obtener los datos del afiliado
                                    Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(numSolicitud, "", "", "", "");

                                    // Obtener el último agente vigente por cartera
                                    agente = servicioCotizador.ObtenerUltimoAgentePorCartera(afiliado.Agente.IdCartera, usuario);
                                }
                                else
                                {
                                    log.Error(string.Format("Solicitud [{0}] no pertenece a la cartera de ventas de [{1}] o agente/cartera no está vigente.", numSolicitud, solicitud.Agente.Id));
                                    HttpContext.Current.Response.Status = "403 Forbidden";
                                    HttpContext.Current.Response.StatusCode = 403;
                                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                                }
                            }
                            Agente supervisor = listaAgentes.Find(a => a.Id == agente.IdPadre);

                            // Firma Digital
                            FirmaDigital firma = servicioCotizador.ObtenerFirmaDigital(numSolicitud, correlativo, (string)HttpContext.Current.Session["Usuario"]);
                            if (firma == null)
                            {
                                // Crear registro de Firma Digital
                                firma = new FirmaDigital
                                {
                                    num_solicitud = numSolicitud,
                                    num_item = correlativo,
                                    gls_ip = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                    gls_browser_agent = HttpContext.Current.Request.UserAgent,
                                    ind_consentimiento = "N",
                                    aud_usr_ingreso = (string)HttpContext.Current.Session["Usuario"]
                                };
                                firma = servicioCotizador.RegistrarFirmaDigital(firma);
                            }

                            // Construir link de Firma Digital
                            string linkFirmaDigital = string.Format(ConfigurationManager.AppSettings["url_app_firmas_digitales"], firma.gls_token);

                            // Enviar correo por el SME
                            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                            SMEEnvio envio = new SMEEnvio
                            {
                                Email = beneficiario.CorreoElectronico,
                                Destinatario = string.Format("{0} {1} {2}", beneficiario.Nombre, beneficiario.ApellidoPaterno, beneficiario.ApellidoPaterno),
                                NumeroDocumento = beneficiario.Identificacion.Numero,
                                NumeroPoliza = solicitud.NumeroPoliza.ToString(),
                                ProcesoSme = ConfigurationManager.AppSettings["SMEFirmaDigitalRV"],
                                CamposDinamicosSerializados = JsonConvert.SerializeObject(new
                                {
                                    Id_nombres = ti.ToTitleCase(beneficiario.Nombre.ToLower().Trim()),
                                    Id_link = linkFirmaDigital,
                                    Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                                })
                            };
                            long codigoSME = servicioCotizador.EnviarCorreoSME(envio);

                            try
                            {
                                // Registrar seguimiento
                                EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                                {
                                    gls_identificador = string.Format("{0}|{1}", correlativo, numSolicitud),
                                    id_proceso_envio = (int)Enums.ProcesoEnvio.FirmaDigitalRV,
                                    id_sme = codigoSME,
                                    cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                    gls_mail = beneficiario.CorreoElectronico,
                                    fec_envio = DateTime.UtcNow,
                                    cod_agente = agente.Id,
                                    aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                                };
                                envioSeguimiento = servicioCotizador.RegistrarEnvioSeguimiento(envioSeguimiento);
                            }
                            catch (Exception ex)
                            {
                                log.Error("Se ha producido un error al registrar el seguimiento del Envío de Firma Digital de RRVV.", ex);
                            }

                            // Cuando el envío lo hace el supervisor o jefe se debe notificar al agente
                            string flagCorreoAgente = ConfigurationManager.AppSettings["flag_correo_agente"].ToString();
                            log.Debug(string.Format("Se va a evaluar el envío de notificación al agente. flagCorreoAgente[{0}]", flagCorreoAgente));
                            try
                            {
                                string[] rolesNotificacion =
                                {
                                    Enums.RolAzman.AsistenteComercial.StringValue(),
                                    Enums.RolAzman.SupervisorLima.StringValue(),
                                    Enums.RolAzman.SupervisorProvincia.StringValue(),
                                    Enums.RolAzman.JefeVentaLima.StringValue(),
                                    Enums.RolAzman.JefeVentaProvincia.StringValue()
                                };
                                if (rolesNotificacion.Contains(HttpContext.Current.Session["RolAzman"]))
                                {
                                    if (flagCorreoAgente == "S")
                                    {
                                        string correoAgente = string.Empty;
                                        string destinatarioAgente = ConfigurationManager.AppSettings["destinatario_consentimiento_agente"].ToString();
                                        if (destinatarioAgente != "N")
                                        {
                                            correoAgente = destinatarioAgente;
                                        }

                                        // Obtener datos del Agente
                                        BEUsuario datosUsuario = null;
                                        log.Debug(string.Format("Obteniendo datos del usuario [{0}]", agente.Usuario));
                                        ServicioAzmanClient servicioAzman = new ServicioAzmanClient("epAzman");
                                        datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                                ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                                ConfigurationManager.AppSettings["DominioRed"],
                                                agente.Usuario);

                                        // Envío de Notificación
                                        log.Debug(string.Format("Se va a enviar una notificación al agente [{0}]", correoAgente));

                                        var glsAgente = ti.ToTitleCase(agente.Nombre.ToLower().Trim());
                                        var glsCliente = ti.ToTitleCase(string.Format("{0} {1} {2}", beneficiario.Nombre, beneficiario.ApellidoPaterno, beneficiario.ApellidoMaterno).ToLower().Trim());
                                        var glsCategoria = ti.ToTitleCase(beneficiario.glsCategoria.ToLower().Trim());

                                        var htmlCorreoAgente = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\Plantilla\\RVI\\Consentimiento\\NotificacionAgente.html");
                                        htmlCorreoAgente = htmlCorreoAgente
                                            .Replace("{agente}", glsAgente)
                                            .Replace("{usuario}", HttpContext.Current.Session["NombreCompleto"].ToString())
                                            .Replace("{solicitud}", numSolicitud)
                                            .Replace("{cliente}", glsCliente)
                                            .Replace("{cuspp}", beneficiario.numCuspp)
                                            .Replace("{categoria}", glsCategoria);

                                        var notificacion = new NotificacionSME()
                                        {
                                            De = "sme@interseguro.com.pe",
                                            DeNombre = "Interseguro",
                                            ResponderA = "noreply@interseguro.com.pe",
                                            ResponderANombre = "Interseguro",
                                            Para = correoAgente,
                                            Cuerpo = htmlCorreoAgente,
                                            Asunto = string.Format("Enlace de Firma Digital de Solicitud {0} fue enviada a {1}", numSolicitud, glsCliente)
                                        };
                                        servicioCotizador.EnviarNotificacionSME(notificacion);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error("Se ha producido un error al enviar la notificación del Envío de Firma Digital de RRVV al agente.", ex);
                            }
                        }
                        else
                        {
                            log.Error(string.Format("Usuario [{0}] ha intentado acceder a la opción [EnviarFirmaDigital] que no corresponde a su rol.", (string)HttpContext.Current.Session["Usuario"]));
                            HttpContext.Current.Response.Status = "403 Forbidden";
                            HttpContext.Current.Response.StatusCode = 403;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Se ha producido un error al enviar el enlace de Firma Digital al Cliente", ex);
                    throw ex;
                }
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> parametros)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Parametro parametro in parametros)
            {
                control.Items.Add(new ListItem(parametro.Glosa ?? parametro.Nombre, parametro.Id));
            }
        }

        private void CargarComboboxDepartamento(DropDownList control, List<Departamento> departamentos)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (var departamento in departamentos)
            {
                control.Items.Add(new ListItem(departamento.gls_departamento, departamento.id_departamento));
            }
        }

        private void CargarComboboxProvincia(DropDownList control, List<Provincia> provincias)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (var provincia in provincias)
            {
                control.Items.Add(new ListItem(provincia.gls_provincia, provincia.id_provincia));
            }
        }

        private void CargarComboboxDistrito(DropDownList control, List<Distrito> distritos)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (var distrito in distritos)
            {
                control.Items.Add(new ListItem(distrito.gls_distrito, distrito.id_distrito));
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
                            List<Parametro> parametros = new List<Parametro>();
                            var provincias = servicioCotizador.ListarProvincias(idDepartamento, (string)HttpContext.Current.Session["Usuario"]);
                            provincias.ForEach(p =>
                            {
                                parametros.Add(new Parametro
                                {
                                    Id = p.id_provincia,
                                    Glosa = p.gls_provincia
                                });
                            });
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
                            List<Parametro> parametros = new List<Parametro>();
                            var distritos = servicioCotizador.ListarDistritos(idProvincia, (string)HttpContext.Current.Session["Usuario"]);
                            distritos.ForEach(d =>
                            {
                                parametros.Add(new Parametro
                                {
                                    Id = d.id_distrito,
                                    Glosa = d.gls_distrito
                                });
                            });
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
    }
}