using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
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
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
{
    public partial class GrupoFamiliarAfiliadoCierre : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(GrupoFamiliarAfiliadoCierre));
        private static IServicioCWRV servicioCotizador;

        private string cargaParentesco = "0";

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
                            string indiceBeneficiario = string.Empty;
                            if (Page.PreviousPage != null)
                            {
                                var solicitudSerializado = (HiddenField)Page.PreviousPage.Form.FindControl("Contenido").FindControl("HSolicitudSerializado");
                                if (solicitudSerializado.Value == null)
                                {
                                    Response.Redirect("Cotizador.aspx#datos_solicitud");
                                }
                                SolicitudIFP objetoSolicitud = Newtonsoft.Json.JsonConvert.DeserializeObject<SolicitudIFP>(solicitudSerializado.Value);
                                if (objetoSolicitud.Cotizaciones == null)
                                {
                                    Response.Redirect("Cotizador.aspx#datos_solicitud");
                                }
                                objetoSolicitud.Cotizaciones = objetoSolicitud.Cotizaciones.FindAll(c => c.Correlativo == Convert.ToInt64(Session["NumCorrelativo"].ToString()));

                                if (objetoSolicitud.Cotizaciones == null)
                                {
                                    Response.Redirect("Cotizador.aspx#datos_solicitud");
                                }

                                HSolicitudSerializado.Value = Newtonsoft.Json.JsonConvert.SerializeObject(objetoSolicitud);

                                //RECIBE INDICE DE BENEFICIARIO EN LISTA TEMPORAL
                                indiceBeneficiario = Request.QueryString["indiceBeneficiario"];
                                if (indiceBeneficiario != null && indiceBeneficiario != "0")
                                {
                                    indiceBeneficiario = (Convert.ToInt32(indiceBeneficiario) + 1).ToString();
                                    //GrupoFamiliar beneficiario = objetoSolicitud.Beneficiarios.Find(b => b.Id.ToString() == indiceBeneficiario);
                                    //if (beneficiario.TipoCobertura == "CA") ModGruFamLineaPorcentaje_RP.Visible = false;

                                }

                                if (indiceBeneficiario != null)
                                {
                                    if (Request.QueryString["form"] == "CA")
                                    {
                                        ModSolSiguiente_RP.PostBackUrl = "~/RentaIFP/ResumenBeneficiariosCA.aspx";
                                        ModSolCancelar_RP.Value = "ResumenBeneficiariosCA.aspx";
                                    }
                                    if (Request.QueryString["form"] == "PG")
                                    {
                                        ModSolSiguiente_RP.PostBackUrl = "~/RentaIFP/ResumenBeneficiariosPG.aspx";
                                        ModSolCancelar_RP.Value = "ResumenBeneficiariosPG.aspx";
                                        cargaParentesco = "1";
                                        HTipoPeriodoBeneficiario.Value = Request.QueryString["pg"];
                                    }
                                }
                                else
                                {
                                    ModSolSiguiente_RP.PostBackUrl = "~/RentaIFP/SeleccionSolicitud.aspx";
                                    ModSolCancelar_RP.Value = "Cotizador.aspx#datos_solicitud";

                                    if (objetoSolicitud.Cotizaciones[0].PeriodoGarantizado > 0 || objetoSolicitud.Cotizaciones[0].Plan.Id == Enums.Planes.PLAN3.StringValue())
                                    {
                                        ModSolSiguiente_RP.PostBackUrl = "~/RentaIFP/ResumenBeneficiariosPG.aspx";
                                    }
                                    if (objetoSolicitud.Cotizaciones[0].ValPjeCACy > 0 || objetoSolicitud.Cotizaciones[0].ValPjeCAPa > 0 || objetoSolicitud.Cotizaciones[0].ValPjeCAMa > 0 || indiceBeneficiario != null)
                                    {
                                        ModSolSiguiente_RP.PostBackUrl = "~/RentaIFP/ResumenBeneficiariosCA.aspx";
                                    }
                                }

                                if (cargaParentesco == "1")
                                {
                                    cargaParentesco = Enums.Parentesco.Afiliado.StringValue();

                                    if ((indiceBeneficiario != null && indiceBeneficiario != "0"))
                                    {
                                        var valor_tipoidentificacion = Request.QueryString["tipoidentificacion"];
                                        var valor_numeroidentificacion = Request.QueryString["numeroidentificacion"];

                                        //GrupoFamiliar beneficiario = objetoSolicitud.Beneficiarios.Find(b => b.Id.ToString() == indiceBeneficiario);
                                        GrupoFamiliar beneficiario = objetoSolicitud.Beneficiarios.Find(b => b.Identificacion.IdTipo == valor_tipoidentificacion && 
                                                                                                            b.Identificacion.Numero == valor_numeroidentificacion);
                                        indiceBeneficiario = beneficiario.Id.ToString();

                                        if (beneficiario == null)
                                        {
                                            indiceBeneficiario = Request.QueryString["id"];
                                            beneficiario = objetoSolicitud.Beneficiarios.Find(b => b.Id.ToString() == indiceBeneficiario);
                                        }

                                        if (beneficiario.TipoCobertura != "CA")
                                        {
                                            if (objetoSolicitud.Cotizaciones[0].ValPjeCACy > 0)
                                            {
                                                cargaParentesco += "," + Enums.Parentesco.Conyuge.StringValue();
                                            }
                                            if (objetoSolicitud.Cotizaciones[0].ValPjeCAPa > 0 || objetoSolicitud.Cotizaciones[0].ValPjeCAMa > 0)
                                            {
                                                cargaParentesco += "," + Enums.Parentesco.Padre.StringValue();
                                            }
                                        }
                                        
                                    }
                                    else
                                    {
                                        if (objetoSolicitud.Cotizaciones[0].ValPjeCACy > 0)
                                        {
                                            cargaParentesco += "," + Enums.Parentesco.Conyuge.StringValue();
                                        }
                                        if (objetoSolicitud.Cotizaciones[0].ValPjeCAPa > 0 || objetoSolicitud.Cotizaciones[0].ValPjeCAMa > 0)
                                        {
                                            cargaParentesco += "," + Enums.Parentesco.Padre.StringValue();
                                        }
                                    }
                                    
                                }
                                else
                                {
                                    cargaParentesco = string.Empty;
                                }

                                //List<CotizacionIFP> objetoCotizaciones = objetoSolicitud.Cotizaciones.ToList();
                                //CotizacionIFP objetoCotizacion = objetoCotizaciones.Find(c => c.Correlativo == Convert.ToInt64(Session["NumCorrelativo"].ToString()));
                                //if (objetoCotizacion == null)
                                //{
                                //    Response.Redirect("Cotizador.aspx");
                                //}

                                //HSolicitudSerializado.Value = Newtonsoft.Json.JsonConvert.SerializeObject(objetoCotizacion);

                            }
                            else
                            {
                                Response.Redirect("Cotizador.aspx#datos_solicitud");
                            }

                            //string source = Request.QueryString["source"];
                            //string solicitud = Request.QueryString["solicitud"];
                            //log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            //if (source == "correo")
                            //{
                            //    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            //    List<Temporal> listaCombobox = servicioCotizador.ListarGruposFamiliaresxSolicitud(solicitud);

                            //    Session["listGrupoFamiliar"] = listaCombobox;
                            //    Session["idGrupoFamiliar"] = listaCombobox[0].codigo;
                            //    Session["Cantidad"] = listaCombobox[0].item;
                            //    Session["Total"] = listaCombobox[0].cantidad;

                            //    Session["ModGruFamModo"] = "MC";
                            //    Session["Solicitud"] = solicitud;
                            //    Session["PaginaLlamada"] = "../RentaPrivadaPlus/SeleccionSolicitud.aspx";
                            //    Session["NroSolicitud"] = solicitud;

                            //    SolicitudRPPlus solicitudRRP = servicioCotizador.ObtenerEstadoSolicitudRPPlus(solicitud);

                            //    Session["FecSolicitud"] = Convert.ToDateTime(solicitudRRP.FechaSolicitud).ToString("dd/MM/yyyy");
                            //    Session["EstadoSolicitud"] = solicitudRRP.CodigoEstado;
                            //    Session["CUSPP_PLUS"] = solicitudRRP.Afiliado.CUSPP;
                            //    Session["Vendedor"] = solicitudRRP.Afiliado.Agente.Id;
                            //    Session["CUSPP_RP"] = solicitudRRP.Afiliado.CUSPP.ToString();
                            //    Session["AFP_RP"] = solicitudRRP.Afiliado.AFP.Id.ToString();




                            //    Session["Consentimiento"] = true;
                            //}
                            //else
                            //{                                
                            //}

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

                            if (Session["CUSPP"] == null && Session["NroSolicitud"] == null)
                            {
                                SolicitudRPPlus solicitudRRP = servicioCotizador.ObtenerEstadoSolicitudRPPlus(Session["Solicitud"].ToString());

                                Session["FecSolicitud"] = Convert.ToDateTime(solicitudRRP.FechaSolicitud).ToString("dd/MM/yyyy");
                                Session["EstadoSolicitud"] = solicitudRRP.CodigoEstado;
                                Session["CUSPP_PLUS"] = solicitudRRP.Afiliado.CUSPP;
                                Session["CUSPP"] = solicitudRRP.Afiliado.CUSPP;
                                Session["Vendedor"] = solicitudRRP.Afiliado.Agente.Id;
                                Session["CUSPP_RP"] = solicitudRRP.Afiliado.CUSPP.ToString();
                                Session["AFP_RP"] = solicitudRRP.Afiliado.AFP.Id.ToString();


                                Session["Consentimiento"] = true;
                            }

                            //LimpiarFormularios();

                            ModGruFamModo_RP.Value = Convert.ToString((Session["ModGruFamModo"]));

                            indiceBeneficiario = indiceBeneficiario == null ? Session["idGrupoFamiliar"].ToString() : indiceBeneficiario == "0" ? indiceBeneficiario : indiceBeneficiario;
                            //ModIdGrupoFamiliar.Value = indiceBeneficiario != null ? (Convert.ToInt32(indiceBeneficiario) + 1).ToString() : Convert.ToString((Session["idGrupoFamiliar"]));
                            ModIdGrupoFamiliar.Value = indiceBeneficiario;

                            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
                            {
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP"]));
                                ModGruFamModo_RP.Value = Convert.ToString((Session["ModGruFamModo"]));
                                //ModIdGrupoFamiliar.Value = Convert.ToString((Session["idGrupoFamiliar"]));
                                ModGruFamCantidad.Value = Convert.ToString((Session["Cantidad"]));
                                ModGruFamTotal.Value = Convert.ToString((Session["Total"]));
                                ModGruFamsolicitud.Value = Convert.ToString((Session["Solicitud"]));
                                ModGruFamfecsolicitud.Value = Convert.ToString((Session["FecSolicitud"]));
                                ModGruFamestadosolicitud.Value = Convert.ToString((Session["EstadoSolicitud"]));
                                ModGruPlan.Value = Convert.ToString((Session["CodPlan"]));
                                ModGruNumCorrelativo.Value = Convert.ToString((Session["NumCorrelativo"]));
                            }
                            else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                            {
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP_PLUS"]));
                                ModGruFamModo_RP.Value = Convert.ToString((Session["ModGruFamModo"]));
                                //ModIdGrupoFamiliar.Value = Convert.ToString((Session["idGrupoFamiliar"]));
                                ModGruFamCantidad.Value = Convert.ToString((Session["Cantidad"]));
                                ModGruFamTotal.Value = Convert.ToString((Session["Total"]));
                                ModGruFamsolicitud.Value = Convert.ToString((Session["Solicitud"]));
                                ModGruFamfecsolicitud.Value = Convert.ToString((Session["FecSolicitud"]));
                                ModGruFamestadosolicitud.Value = Convert.ToString((Session["EstadoSolicitud"]));
                                ModGruPlan.Value = Convert.ToString((Session["CodPlan"]));
                                ModGruNumCorrelativo.Value = Convert.ToString((Session["NumCorrelativo"]));
                            }
                            else
                            {
                                Response.Redirect(Convert.ToString((Session["PaginaLlamada"])));
                            }

                            List<Beneficiario> beneficiarios = servicioCotizador.ListarBeneficiarios(ModGruFamsolicitud.Value, Session["Usuario"].ToString());
                            Beneficiario afiliado = beneficiarios.Where(g => g.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue()).FirstOrDefault();

                            HToken.Value = afiliado.firmaDigital.gls_token.ToString();

                            CargarInformacionInicialPantalla();

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

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            //Mostrando sin Asegurado o titular
            List<Parametro> lstParametroParentesco = new List<Parametro>();
            List<Parametro> lstParametroParentescoOriginal = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Parentesco];

            string[] lstCargaParentesco = cargaParentesco.Split(',');

            if (lstCargaParentesco != null)
            {
                foreach (string parentesco in lstCargaParentesco)
                {
                    var entidadParentesco = lstParametroParentescoOriginal.SingleOrDefault(p => p.Id == parentesco);

                    if (entidadParentesco != null) lstParametroParentescoOriginal.Remove(entidadParentesco);
                }
            }

            CargarCombobox(ModGruFamParentesco_RP, lstParametroParentescoOriginal);
            CargarCombobox(ModGruFamTipoIdentificacion_RP, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);
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

        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

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

        [WebMethod]
        public static String SessionIdGrupoFamiliar(string solicitud, string fecha, int estado, int numCorrelativo, string codPlan, string paginaLlamada)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {

                //HttpContext.Current.Session["idGrupoFamiliar"] = idGrupoFamiliar;

                HttpContext.Current.Session["ModGruFamModo"] = "MC";
                HttpContext.Current.Session["Solicitud"] = solicitud;
                HttpContext.Current.Session["FecSolicitud"] = fecha;
                HttpContext.Current.Session["EstadoSolicitud"] = estado;
                HttpContext.Current.Session["NumCorrelativo"] = numCorrelativo;
                HttpContext.Current.Session["CodPlan"] = codPlan;
                HttpContext.Current.Session["PaginaLlamada"] = paginaLlamada;

                return "OK";
            }
        }

        [WebMethod]
        public static String IngresarCantidadFamiliar(int total, int cantidad, string paginaLlamada)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                HttpContext.Current.Session["Total"] = total;
                HttpContext.Current.Session["Cantidad"] = cantidad;
                HttpContext.Current.Session["idGrupoFamiliar"] = cantidad;
                HttpContext.Current.Session["PaginaLlamada"] = paginaLlamada;
                HttpContext.Current.Session["ModGruFamModo"] = "NC";

                return "OK";
            }
        }

        ////[WebMethod]
        ////public static Respuesta InsertarGrupoFamiliar(string tokenUsuario,
        ////                                                string cuspp,
        ////                                                string apellidoPaterno,
        ////                                                string apellidoMaterno,
        ////                                                string nombres,
        ////                                                string tipoIdentificacion,
        ////                                                string numeroIdentificacion,
        ////                                                string parentesco,
        ////                                                string sexo,
        ////                                                string fechaNacimiento,
        ////                                                string invalidez,
        ////                                                string tipoInvalidez,
        ////                                                string fechaInvalidez,
        ////                                                string PEP,
        ////                                                string sujetoObligado,
        ////                                                string nacionalidad,
        ////                                                string profesion,
        ////                                                string residencia)
        ////{
        ////    using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
        ////    {
        ////        try
        ////        {
        ////            Respuesta respuesta = new Respuesta();

        ////            if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
        ////            {
        ////                if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarInsertar))
        ////                {
        ////                    List<String> errores = new List<String>();
        ////                    List<String> controles = new List<String>();
        ////                    if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
        ////                    {
        ////                        if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
        ////                        {
        ////                            GrupoFamiliar gru = new GrupoFamiliar
        ////                            {
        ////                                Afiliado = new Afiliado { CUSPP = cuspp },
        ////                                Identificacion = new Identificacion(),
        ////                                Parentesco = new Parentesco { Id = parentesco },
        ////                                Sexo = Convert.ToChar(sexo),
        ////                                FechaNacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE")),
        ////                                Invalido = (invalidez == "S") ? true : false,
        ////                                TipoInvalidez = new TipoInvalidez { Id = tipoInvalidez },
        ////                                Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
        ////                            };
        ////                            if (apellidoPaterno.Trim() != String.Empty)
        ////                            {
        ////                                gru.ApellidoPaterno = apellidoPaterno;
        ////                            }
        ////                            if (apellidoMaterno.Trim() != String.Empty)
        ////                            {
        ////                                gru.ApellidoMaterno = apellidoMaterno;
        ////                            }
        ////                            if (nombres.Trim() != String.Empty)
        ////                            {
        ////                                gru.Nombre = nombres;
        ////                            }
        ////                            if (tipoIdentificacion.Trim() != "0")
        ////                            {
        ////                                gru.Identificacion.IdTipo = tipoIdentificacion;
        ////                            }
        ////                            if (numeroIdentificacion.Trim() != String.Empty)
        ////                            {
        ////                                gru.Identificacion.Numero = Convert.ToInt32(numeroIdentificacion);
        ////                            }
        ////                            if (fechaInvalidez.Trim() != String.Empty)
        ////                            {
        ////                                gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
        ////                            }

        ////                            //<INIGTI_7012>
        ////                            if (nacionalidad.Trim() != "0")
        ////                            {
        ////                                gru.Nacionalidad = new Temporal { cod_parametro = nacionalidad };
        ////                            }
        ////                            else { gru.Nacionalidad = new Temporal { cod_parametro = "0" }; }

        ////                            if (profesion.Trim() != "0")
        ////                            {
        ////                                gru.Profesion = new Temporal { cod_parametro = profesion };
        ////                            }
        ////                            else { gru.Profesion = new Temporal { cod_parametro = "0" }; }

        ////                            if (residencia.Trim() != "0")
        ////                            {
        ////                                gru.Residencia = new Temporal { cod_parametro = residencia };
        ////                            }
        ////                            else { gru.Residencia = new Temporal { cod_parametro = "0" }; }

        ////                            gru.ind_PEP = (PEP == "S") ? true : false;
        ////                            gru.ind_SujetoObligado = (sujetoObligado == "S") ? true : false;
        ////                            //<FINGTI_7012>

        ////                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
        ////                            respuesta = servicioCotizador.RegistrarGrupoFamiliar(gru);

        ////                            //respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
        ////                            //respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
        ////                            //respuesta.Mensaje = "Grupo familiar agregado correctamente.";
        ////                        }
        ////                        else
        ////                        {
        ////                            respuesta.Estado = Constante.COD_ERROR;
        ////                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        ////                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        ////                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
        ////                        }
        ////                    }
        ////                    else
        ////                    {
        ////                        respuesta.Estado = Constante.COD_ERROR;
        ////                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
        ////                        respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
        ////                        respuesta.Mensaje = Utilitarios.FormatearError(errores);
        ////                        respuesta.Controles = controles;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
        ////                        Enums.OpcionesSistema.GrupoFamiliarInsertar.StringValue()));
        ////                    respuesta.Estado = Constante.COD_ERROR;
        ////                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        ////                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        ////                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
        ////                }
        ////            }
        ////            else
        ////            {
        ////                log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
        ////                respuesta.Estado = Constante.COD_TOKEN;
        ////            }
        ////            return respuesta;
        ////        }
        ////        catch (Exception ex)
        ////        {
        ////            Respuesta respuesta = new Respuesta();
        ////            respuesta.Estado = Constante.COD_ERROR;
        ////            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        ////            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        ////            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
        ////            return respuesta;
        ////        }
        ////    }
        ////}

        ////[WebMethod]
        ////public static Respuesta ModificarGrupoFamiliar(string tokenUsuario,
        ////                                                string idGrupoFamiliar,
        ////                                                string cuspp,
        ////                                                string apellidoPaterno,
        ////                                                string apellidoMaterno,
        ////                                                string nombres,
        ////                                                string tipoIdentificacion,
        ////                                                string numeroIdentificacion,
        ////                                                string parentesco,
        ////                                                string sexo,
        ////                                                string fechaNacimiento,
        ////                                                string invalidez,
        ////                                                string tipoInvalidez,
        ////                                                string fechaInvalidez,
        ////                                                string PEP,
        ////                                                string sujetoObligado,
        ////                                                string nacionalidad,
        ////                                                string profesion,
        ////                                                string residencia,
        ////                                                string banco,
        ////                                                string comunicacion,
        ////                                                string numerobanco,
        ////                                                string flagRenta)
        ////{
        ////    using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
        ////    {
        ////        try
        ////        {
        ////            Respuesta respuesta = new Respuesta();

        ////            if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
        ////            {
        ////                if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.GrupoFamiliarActualizar))
        ////                {
        ////                    List<String> errores = new List<String>();
        ////                    List<String> controles = new List<String>();
        ////                    if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez))
        ////                    {
        ////                        if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]))
        ////                        {
        ////                            GrupoFamiliar gru = new GrupoFamiliar
        ////                            {
        ////                                Id = Convert.ToInt32(idGrupoFamiliar),
        ////                                Afiliado = new Afiliado { CUSPP = cuspp },
        ////                                Identificacion = new Identificacion(),
        ////                                Parentesco = new Parentesco { Id = parentesco },
        ////                                Sexo = Convert.ToChar(sexo),
        ////                                FechaNacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE")),
        ////                                Invalido = (invalidez == "S") ? true : false,
        ////                                TipoInvalidez = new TipoInvalidez { Id = tipoInvalidez },
        ////                                flagRenta = flagRenta,
        ////                                Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
        ////                            };
        ////                            if (apellidoPaterno.Trim() != String.Empty)
        ////                            {
        ////                                gru.ApellidoPaterno = apellidoPaterno;
        ////                            }
        ////                            if (apellidoMaterno.Trim() != String.Empty)
        ////                            {
        ////                                gru.ApellidoMaterno = apellidoMaterno;
        ////                            }
        ////                            if (nombres.Trim() != String.Empty)
        ////                            {
        ////                                gru.Nombre = nombres;
        ////                            }
        ////                            if (tipoIdentificacion.Trim() != "0")
        ////                            {
        ////                                gru.Identificacion.IdTipo = tipoIdentificacion;
        ////                            }
        ////                            if (numeroIdentificacion.Trim() != String.Empty)
        ////                            {
        ////                                gru.Identificacion.Numero = Convert.ToInt32(numeroIdentificacion);
        ////                            }
        ////                            if (fechaInvalidez.Trim() != String.Empty)
        ////                            {
        ////                                gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
        ////                            }

        ////                            //<INIGTI_7012>
        ////                            if (nacionalidad.Trim() != "0")
        ////                            {
        ////                                gru.Nacionalidad = new Temporal { cod_parametro = nacionalidad };
        ////                            }
        ////                            else { gru.Nacionalidad = new Temporal { cod_parametro = "0" }; }

        ////                            if (profesion.Trim() != "0")
        ////                            {
        ////                                gru.Profesion = new Temporal { cod_parametro = profesion };
        ////                            }
        ////                            else { gru.Profesion = new Temporal { cod_parametro = "0" }; }

        ////                            if (residencia.Trim() != "0")
        ////                            {
        ////                                gru.Residencia = new Temporal { cod_parametro = residencia };
        ////                            }
        ////                            else { gru.Residencia = new Temporal { cod_parametro = "0" }; }

        ////                            gru.ind_PEP = (PEP == "S") ? true : false;
        ////                            gru.ind_SujetoObligado = (sujetoObligado == "S") ? true : false;

        ////                            if (banco.Trim() != "0")
        ////                            {
        ////                                gru.Banco = new Parametro { Id = banco };
        ////                            }
        ////                            else { gru.Banco = new Parametro { Id = "0" }; }

        ////                            if (comunicacion.Trim() != "0")
        ////                            {
        ////                                gru.Comunicacion = new Parametro { Id = comunicacion };
        ////                            }
        ////                            else { gru.Comunicacion = new Parametro { Id = "0" }; }

        ////                            if (numerobanco.Trim() != String.Empty)
        ////                            {
        ////                                gru.NumeroBanco = numerobanco;
        ////                            }

        ////                            //<FINGTI_7012>

        ////                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
        ////                            respuesta = servicioCotizador.ActualizarGrupoFamiliar(gru);

        ////                            //respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
        ////                            //respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
        ////                            //respuesta.Mensaje = "Grupo familiar modificado correctamente.";

        ////                        }
        ////                        else
        ////                        {
        ////                            respuesta.Estado = Constante.COD_ERROR;
        ////                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        ////                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        ////                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Cliente no pertenece a su cartera de ventas. Verifique." });
        ////                        }
        ////                    }
        ////                    else
        ////                    {
        ////                        respuesta.Estado = Constante.COD_ERROR;
        ////                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Validacion.StringValue();
        ////                        respuesta.Icono = Enums.CuadroMensajeIcono.Validacion.StringValue();
        ////                        respuesta.Mensaje = Utilitarios.FormatearError(errores);
        ////                        respuesta.Controles = controles;
        ////                    }
        ////                }
        ////                else
        ////                {
        ////                    log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
        ////                        Enums.OpcionesSistema.GrupoFamiliarActualizar.StringValue()));
        ////                    respuesta.Estado = Constante.COD_ERROR;
        ////                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        ////                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        ////                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
        ////                }
        ////            }
        ////            else
        ////            {
        ////                log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
        ////                respuesta.Estado = Constante.COD_TOKEN;
        ////            }
        ////            return respuesta;
        ////        }
        ////        catch (Exception ex)
        ////        {
        ////            Respuesta respuesta = new Respuesta();
        ////            respuesta.Estado = Constante.COD_ERROR;
        ////            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        ////            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        ////            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
        ////            return respuesta;
        ////        }
        ////    }
        ////}

        [WebMethod]
        public static Respuesta InsertarGrupoFamiliar(string tokenUsuario,
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
                                                        string pjeRenta,
                                                        string objSolicitud
                                                                        )
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

                            SolicitudIFP lstSolicitudIFP = Newtonsoft.Json.JsonConvert.DeserializeObject<SolicitudIFP>(objSolicitud);
                            List<GrupoFamiliar> lstGrupoFamiliar = lstSolicitudIFP.Beneficiarios;
                            GrupoFamiliar grupoFamiliar = lstGrupoFamiliar.Find(g => g.Id.ToString() == idGrupoFamiliar);
                            bool validaPjeRenta = true;

                            if (grupoFamiliar != null && grupoFamiliar.TipoCobertura == "CA") validaPjeRenta = false;

                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez, pjeRenta, vPjeRenta: validaPjeRenta))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"])
                                    || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue()
                                    || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue()
                                    || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue()
                                    || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
                                {
                                    GrupoFamiliar gru;

                                    if (grupoFamiliar == null) gru = new GrupoFamiliar();
                                    else gru = grupoFamiliar;

                                    //gru.Id = Convert.ToInt32(idGrupoFamiliar);
                                    gru.Afiliado = new Afiliado { CUSPP = cuspp };
                                    gru.Identificacion = new Identificacion();
                                    gru.Parentesco = new Parentesco { Id = parentesco };
                                    gru.Sexo = Convert.ToChar(sexo);
                                    gru.FechaNacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE"));
                                    gru.Invalido = (invalidez == "S") ? true : false;
                                    gru.TipoInvalidez = new TipoInvalidez { Id = tipoInvalidez };
                                    gru.Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] };
                                    gru.ValPjeRenta = Convert.ToDouble(pjeRenta);

                                    //GrupoFamiliar gru = new GrupoFamiliar
                                    //{
                                    //    Id = Convert.ToInt32(idGrupoFamiliar),
                                    //    Afiliado = new Afiliado { CUSPP = cuspp },
                                    //    Identificacion = new Identificacion(),
                                    //    Parentesco = new Parentesco { Id = parentesco },
                                    //    Sexo = Convert.ToChar(sexo),
                                    //    FechaNacimiento = Convert.ToDateTime(fechaNacimiento, new CultureInfo("es-PE")),
                                    //    Invalido = (invalidez == "S") ? true : false,
                                    //    TipoInvalidez = new TipoInvalidez { Id = tipoInvalidez },
                                    //    Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] },
                                    //    ValPjeRenta = Convert.ToDouble(pjeRenta),
                                    //};
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
                                    if (fechaInvalidez.Trim() != String.Empty)
                                    {
                                        gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
                                    }
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
                                        gru.Banco = new Parametro { Id = "" };
                                    }
                                    else { gru.Banco = new Parametro { Id = "" }; }

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

                                    //SolicitudIFP lstSolicitudIFP = Newtonsoft.Json.JsonConvert.DeserializeObject<SolicitudIFP>(objSolicitud);
                                    //List<GrupoFamiliar> lstGrupoFamiliar = lstSolicitudIFP.Beneficiarios;

                                    //if (lstGrupoFamiliar == null)
                                    //    lstGrupoFamiliar = new List<GrupoFamiliar>();

                                    //if (lstGrupoFamiliar.Find(p => p.Id == gru.Id) != null)
                                    //{
                                    //    for (int i = 0; i < lstGrupoFamiliar.Count; i++)
                                    //    {
                                    //        if (lstGrupoFamiliar[i].Id == gru.Id)
                                    //        {
                                    //            lstGrupoFamiliar[i] = gru;
                                    //        }
                                    //    }
                                    //}

                                    if (grupoFamiliar != null)
                                    {
                                        for (int i = 0; i < lstGrupoFamiliar.Count; i++)
                                        {
                                            if (lstGrupoFamiliar[i].Id.ToString() == idGrupoFamiliar || lstGrupoFamiliar[i].IdGrupoFamiliar.ToString() == idGrupoFamiliar)
                                            {
                                                lstGrupoFamiliar[i] = gru;
                                            }
                                        }

                                        //if (lstGrupoFamiliar.Where(gf => gf.Id == idGrupoFamiliar).ToList().Count > 0 || lstGrupoFamiliar.Where(gf => gf.IdGrupoFamiliar == idGrupoFamiliar).ToList().Count > 0)

                                    }
                                    else
                                    {
                                        gru.Id = lstGrupoFamiliar.Count + 1;
                                        gru.Ind_Cierre = true;
                                        lstGrupoFamiliar.Add(gru);
                                    }

                                    //lstSolicitudIFP.Beneficiarios = lstGrupoFamiliar;

                                    string objSolicitudRespuesta = Newtonsoft.Json.JsonConvert.SerializeObject(lstSolicitudIFP);

                                    respuesta.Mensaje = objSolicitudRespuesta;
                                    respuesta.Estado = "OK";
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
                                                        string pjeRenta,
                                                        string estadoCivil,
                                                        string correoElectronico,
                                                        string centrolaboral,
                                                        string cargo,
                                                        string actividadeconomica,
                                                        string monedaingreso,
                                                        string ingreso,
                                                        string telefono,
                                                        string celular,
                                                        string origen_fondo
            )
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
                            if (ValidarGrupoFamiliar(errores, controles, apellidoPaterno, apellidoMaterno, nombres, tipoIdentificacion, numeroIdentificacion, parentesco, sexo, fechaNacimiento, invalidez, tipoInvalidez, fechaInvalidez, pjeRenta, estadoCivil, correoElectronico, centrolaboral, cargo, actividadeconomica, monedaingreso, ingreso, telefono, celular))
                            {
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"])
                                    || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue()
                                    || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue()
                                    || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue()
                                    || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
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
                                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] },
                                        ValPjeRenta = Convert.ToDouble(pjeRenta),
                                        CorreoElectronico = correoElectronico

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
                                        gru.Identificacion.Numero = numeroIdentificacion;
                                    }
                                    if (fechaInvalidez.Trim() != String.Empty)
                                    {
                                        gru.FechaInvalidez = Convert.ToDateTime(fechaInvalidez, new CultureInfo("es-PE"));
                                    }
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
                                    else { gru.Banco = new Parametro { Id = "" }; }

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

                                    if (estadoCivil.Trim() != "0")
                                    {
                                        gru.estadoCivil = estadoCivil;
                                    }
                                    if (correoElectronico.Trim() != String.Empty)
                                    {
                                        gru.CorreoElectronico = correoElectronico;
                                    }
                                    if (centrolaboral.Trim() != String.Empty)
                                    {
                                        gru.centroLaboral = centrolaboral;
                                    }
                                    if (cargo.Trim() != String.Empty)
                                    {
                                        gru.cargo = cargo;
                                    }
                                    if (actividadeconomica.Trim() != String.Empty)
                                    {
                                        gru.actividadEconomica = actividadeconomica;
                                    }
                                    if (monedaingreso.Trim() != "0")
                                    {
                                        gru.monedaIngreso = new Moneda { Id = monedaingreso };
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
                                    if (origen_fondo.Trim() != string.Empty)
                                    {
                                        gru.OrigenFondo = new OrigenFondo() { declaracionJurada = origen_fondo };
                                    }

                                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                                    string num_solicitud = (String)HttpContext.Current.Session["Solicitud"];
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

                                    //if (gru.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                                    //{
                                    //    respuesta = servicioCotizador.ActualizarGrupoFamiliar(gru);
                                    //}

                                    //List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["ListaGrupoFamiliarCierre"];

                                    //if (lstGrupoFamiliar == null)
                                    //    lstGrupoFamiliar = new List<GrupoFamiliar>();

                                    //if (gru.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                                    //{
                                    //    lstGrupoFamiliar = new List<GrupoFamiliar>();
                                    //    respuesta = servicioCotizador.ActualizarGrupoFamiliar(gru);
                                    //}
                                    //else
                                    //{
                                    //    for (int i = 0; i < lstGrupoFamiliar.Count; i++)
                                    //    {
                                    //        if (lstGrupoFamiliar[i].Id == gru.Id)
                                    //        {
                                    //            lstGrupoFamiliar[i] = gru;
                                    //        }
                                    //    }
                                    //    respuesta.Estado = "OK";
                                    //}

                                    //HttpContext.Current.Session["ListaGrupoFamiliarCierre"] = lstGrupoFamiliar;

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
        public static GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar, string hSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                string num_solicitud = "";
                GrupoFamiliar gru;

                SolicitudIFP solicitudIFP = Newtonsoft.Json.JsonConvert.DeserializeObject<SolicitudIFP>(hSolicitud);
                List<GrupoFamiliar> lstGrupoFamiliar = solicitudIFP.Beneficiarios;

                //List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["ListaGrupoFamiliarCierre"];
                //if (lstGrupoFamiliar == null)
                //    lstGrupoFamiliar = new List<GrupoFamiliar>();

                //gru = lstGrupoFamiliar.Find(p => p.Id == idGrupoFamiliar);

                //gru = idGrupoFamiliar == 0 ? new GrupoFamiliar() : lstGrupoFamiliar.Find(p => p.Id == idGrupoFamiliar);

                if (idGrupoFamiliar == 0)
                {
                    gru = new GrupoFamiliar
                    {
                        Parentesco = new Parentesco()
                    };
                }
                else
                {
                    gru = lstGrupoFamiliar.Find(p => p.Id == idGrupoFamiliar);
                }

                if (gru != null)
                {
                    if (gru.Identificacion == null) gru.Identificacion = new Identificacion();
                    if (gru.TipoInvalidez == null) gru.TipoInvalidez = new TipoInvalidez();
                    if (gru.Residencia == null) gru.Residencia = new Temporal();
                    if (gru.Profesion == null) gru.Profesion = new Temporal();
                    if (gru.Nacionalidad == null) gru.Nacionalidad = new Temporal();
                    if (gru.Comunicacion == null) gru.Comunicacion = new Parametro();
                    if (gru.Banco == null) gru.Banco = new Parametro();
                    if (gru.TipoCtaBanco == null) gru.TipoCtaBanco = new Parametro();
                    if (gru.Confidencialidaddatos == null) gru.Confidencialidaddatos = new Parametro();
                    if (gru.monedaIngreso == null) gru.monedaIngreso = new Moneda();
                }
                else
                {
                    num_solicitud = (string)HttpContext.Current.Session["Solicitud"];
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    gru = servicioCotizador.ObtenerDatosGrupoFamiliar(idGrupoFamiliar, num_solicitud);

                    //Residencia
                    List<Direccion> lstDireccion = servicioCotizador.ListarDireccion(gru.Afiliado.CUSPP);
                    gru.Residencia.cod_parametro = lstDireccion.FirstOrDefault().Departamento.Id;

                }

                return gru;
            }
        }

        private static bool ValidarGrupoFamiliar(List<String> errores, List<String> controles, string glsApellidoPaterno, string glsApellidoMaterno, string glsNombres, string idTipoIdentificacion, string glsNumeroIdentificacion, string idParentesco, string idSexo, string fecNacimiento, string idInvalidez, string idTipoInvalidez, string fecInvalidez, string pjeRenta, string estadoCivil = "0", string correoElectronico = "", string centrolaboral = "", string cargo = "", string actividadeconomica = "", string monedaingreso = "0", string ingreso = "", string telefono = "", string celular = "", bool vPjeRenta = true)
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

            bool bPjeRenta = true;
            if (vPjeRenta)
            {
                if (pjeRenta == "" || pjeRenta == "0")
                {
                    //errores.Add("Ingrese el campo <strong>Porcentaje (%)</strong>. Dato Obligatorio.");
                    //bPjeRenta = false;
                }
                else
                {
                    double pje;
                    if (Double.TryParse(pjeRenta, out pje))
                    {
                        if (pje <= 0)
                        {
                            errores.Add("Ingrese el campo <strong>Porcentaje (%)</strong>. Dato Obligatorio.");
                            bPjeRenta = false;
                        }
                    }
                    else
                    {
                        errores.Add("Ingrese el campo <strong>Porcentaje (%)</strong>. Dato Obligatorio.");
                        bPjeRenta = false;
                    }
                }
            }

            bool bEstadoCivil = true;
            bool bCorreoElectronico = true;
            bool bCentrolaboral = true;
            bool bCargo = true;
            bool bActividadeconomica = true;
            bool bMonedaingreso = true;
            bool bIngreso = true;
            //bool bTelefono = true;
            bool bCelular = true;

            //inteligo
            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() && idParentesco == "80")
            {
                if (estadoCivil == "0")
                {
                    errores.Add("Seleccione el campo <strong>Estado Civil</strong>. Dato Obligatorio.");
                    bEstadoCivil = false;
                }
                if (correoElectronico.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.");
                    bCorreoElectronico = false;
                }
                if (centrolaboral.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Centro Laboral</strong>. Dato Obligatorio.");
                    bCentrolaboral = false;
                }
                if (cargo.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Cargo</strong>. Dato Obligatorio.");
                    bCargo = false;
                }
                if (actividadeconomica.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Actividad Económica</strong>. Dato Obligatorio.");
                    bActividadeconomica = false;
                }
                if (monedaingreso == "0")
                {
                    errores.Add("Seleccione el campo <strong>Moneda de Ingreso</strong>. Dato Obligatorio.");
                    bMonedaingreso = false;
                }

                if (ingreso.Trim().Length == 0)
                {
                    errores.Add("Ingrese el campo <strong>Ingreso Neto</strong>. Dato Obligatorio.");
                    bIngreso = false;
                }
                //if (telefono.Trim().Length == 0 || telefono.Trim().Length < 7)
                //{
                //    errores.Add("Ingrese el campo <strong>Teléfono</strong>. Dato Obligatorio.");
                //    bTelefono = false;
                //}
                if (celular.Trim().Length == 0 || celular.Trim().Length < 9)
                {
                    errores.Add("Ingrese el campo <strong>Celular</strong>. Dato Obligatorio.");
                    bCelular = false;
                }

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
            //if (!invalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            //if (!tipoInvalidez) { controles.Add("formComboboxContenedor formComboboxErrorContenedor"); } else { controles.Add("formComboboxContenedor"); }
            //if (!fechaInvalidez) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }

            if (!bPjeRenta) { controles.Add("formTextbox formTextboxError"); } else { controles.Add("formTextbox"); }

            if (!bEstadoCivil) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!bCorreoElectronico) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!bCentrolaboral) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!bCargo) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!bActividadeconomica) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!bMonedaingreso) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!bIngreso) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            //if (!bTelefono) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }
            if (!bCelular) { controles.Add("formTextbox formCalendar formTextboxError formCalendarError"); } else { controles.Add("formTextbox formCalendar"); }

            //& invalidez & tipoInvalidez & fechaInvalidez

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento & bPjeRenta & bEstadoCivil & bCorreoElectronico & bCentrolaboral & bCargo & bActividadeconomica & bMonedaingreso & bIngreso & bCelular;

            return esCorrecto;
        }

        //<SOLFIN25621>

        //<INIGTI_7012>
        [WebMethod]
        public static string BuscarGrupoFamiliarSession(int idGrupoFamiliar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                if (idGrupoFamiliar == 0)//Es el inicial o titular
                {
                    string num_solicitud = (string)HttpContext.Current.Session["Solicitud"];
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<Temporal> listaCombobox = servicioCotizador.ListarGruposFamiliaresxSolicitud(num_solicitud);

                    HttpContext.Current.Session["idGrupoFamiliar"] = listaCombobox[idGrupoFamiliar].codigo;
                    HttpContext.Current.Session["Cantidad"] = listaCombobox[idGrupoFamiliar].item;
                }
                else
                {
                    /*El id y la Cantidad actual, Son iguales de la session temporal*/
                    HttpContext.Current.Session["idGrupoFamiliar"] = idGrupoFamiliar;
                    HttpContext.Current.Session["Cantidad"] = idGrupoFamiliar;
                }

                return "OK";
            }
        }

        [WebMethod]
        public static string ExisteGrupoFamiliarSession(int idGrupoFamiliar)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["ListaGrupoFamiliarCierre"];
                if (lstGrupoFamiliar == null)
                    lstGrupoFamiliar = new List<GrupoFamiliar>();

                if (lstGrupoFamiliar.Find(p => p.Id == idGrupoFamiliar) == null)
                {
                    return "FALSE";
                }
                else
                {
                    return "TRUE";
                }

            }
        }

        [WebMethod]
        public static List<Parametro> ObtenerNroBancos(string tabla, string tipoBanco, string tipoCuenta)
        {

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerNroBancos(tabla, tipoBanco, tipoCuenta);

            return listaParametro;

        }
        //<FINGTI_7012>

        //<INI.GTI_7012_11>
        [WebMethod]
        public static List<Parametro> ObtenerTipoCtaBancos(string banco, string id)
        {

            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerTipoCtaBancos(banco, id);

            return listaParametro;

        }
        //<FIN.GTI_7012_11>

        [WebMethod]
        public static List<string> ObtenerPasosCierre(string paso, string objeto)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio Cotizador.ObtenerPasosCierre WebMethod");

                    List<string> respuesta = new List<string>();
                    string html = string.Empty;
                    string paginaSiguiente = string.Empty;
                    //bool indCA = false;

                    //<h2>Datos del Asegurado</h2><section></section><h2>Beneficiarios de C.A.</h2><section></section><h2>Beneficiarios de P.G.</h2><section></section><h2>Cierre de la Solicitud</h2><section></section>

                    SolicitudIFP objetoSolicitud = Newtonsoft.Json.JsonConvert.DeserializeObject<SolicitudIFP>(objeto);

                    html = "<h2>Datos del Asegurado</h2><section></section>";

                    if (objetoSolicitud.Cotizaciones[0].ValPjeCACy > 0 || objetoSolicitud.Cotizaciones[0].ValPjeCAPa > 0 || objetoSolicitud.Cotizaciones[0].ValPjeCAMa > 0)
                    {
                        html += "<h2>Beneficiarios de C.A.</h2><section></section>";
                        paginaSiguiente = "2";
                    }

                    if (objetoSolicitud.Cotizaciones[0].PeriodoGarantizado > 0)
                    {
                        var plan3 = false;
                        foreach (var cotizacion in objetoSolicitud.Cotizaciones)
                        {
                            if (cotizacion.Plan.Id == Enums.Planes.PLAN3.StringValue())
                            {
                                plan3 = true;
                                break;
                            }
                        }

                        if (!plan3)
                        {
                            html += "<h2>Beneficiarios de P.G.</h2><section></section>";
                        }

                        if (plan3)
                        {
                            html += "<h2>Beneficiarios</h2><section></section>";
                        }
                    }

                    //html += "<h2>Cierre de la Solicitud</h2><section></section>";

                    switch (paso)
                    {
                        case "1":
                            paginaSiguiente = "1";
                            break;
                        case "2":
                            paginaSiguiente = "2";
                            break;
                        case "3":
                            if (paginaSiguiente == "2")
                            {
                                paginaSiguiente = "3";
                            }
                            else
                            {
                                paginaSiguiente = "2";
                            }
                            break;
                        case "4":
                            break;
                        default:
                            break;
                    }

                    log.Debug("Fin Cotizador.ObtenerPasosCierre WebMethod");

                    respuesta.Add(html);
                    respuesta.Add(paginaSiguiente);

                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static string CargarTablaPersonasVinculadasPEP(string numSolicitud)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio GrupoFamiliarAfiliadoCierre.CargarTablaPersonasVinculadasPEP WebMethod");

                    var pagina = new Page();
                    var control = (TablaPersonasVinculadasPEP)pagina.LoadControl("~/Controles/TablaPersonasVinculadasPEP.ascx");

                    var PersonaPEP = new GrupoFamiliar
                    {
                        SolicitudRPPlus = new SolicitudRPPlus() { Id = numSolicitud },
                        Identificacion = new Identificacion(),
                        Parentesco = new Parentesco(),
                        Usuario = new Usuario { NombreUsuario = (string)HttpContext.Current.Session["Usuario"] }
                    };

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    control.PersonasPEP = servicioCotizador.ObtenerPersonaVinculada(PersonaPEP);

                    pagina.Controls.Add(control);

                    string html = "";
                    using (var sw = new StringWriter())
                    {
                        HttpContext.Current.Server.Execute(pagina, sw, false);
                        html = sw.ToString();
                    }

                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.CargarTablaPersonasVinculadasPEP WebMethod");

                    return html;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    log.Debug("Fin GrupoFamiliarAfiliadoCierre.CargarTablaPersonasVinculadasPEP WebMethod");
                    throw (ex);
                }
            }
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
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
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
                                if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == (string)HttpContext.Current.Session["Vendedor"]) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue() || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.GerenteDivision.StringValue())
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

        private List<string> CargarNoParentescosPersonaVinculada()
        {
            List<string> lstNoParentesco = new List<string>();

            lstNoParentesco.Add(Enums.Parentesco.Afiliado.StringValue());

            return lstNoParentesco;
        }

        [WebMethod]
        public static GrupoFamiliar ObtenerDatosBenefiCierre(int idGrupoFamiliar, string hSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                string num_solicitud = "";
                GrupoFamiliar gru;

                SolicitudIFP solicitudIFP = Newtonsoft.Json.JsonConvert.DeserializeObject<SolicitudIFP>(hSolicitud);
                List<GrupoFamiliar> lstGrupoFamiliar = solicitudIFP.Beneficiarios;

                if (idGrupoFamiliar == 0)
                {
                    gru = new GrupoFamiliar
                    {
                        Parentesco = new Parentesco()
                    };
                }
                else
                {
                    gru = lstGrupoFamiliar.Find(p => p.Id == idGrupoFamiliar);
                }

                if (gru != null)
                {
                    if (gru.Identificacion == null) gru.Identificacion = new Identificacion();
                    if (gru.TipoInvalidez == null) gru.TipoInvalidez = new TipoInvalidez();
                    if (gru.Residencia == null) gru.Residencia = new Temporal();
                    if (gru.Profesion == null) gru.Profesion = new Temporal();
                    if (gru.Nacionalidad == null) gru.Nacionalidad = new Temporal();
                    if (gru.Comunicacion == null) gru.Comunicacion = new Parametro();
                    if (gru.Banco == null) gru.Banco = new Parametro();
                    if (gru.TipoCtaBanco == null) gru.TipoCtaBanco = new Parametro();
                    if (gru.Confidencialidaddatos == null) gru.Confidencialidaddatos = new Parametro();
                    if (gru.monedaIngreso == null) gru.monedaIngreso = new Moneda();
                }
                else
                {
                    num_solicitud = (string)HttpContext.Current.Session["Solicitud"];
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    gru = servicioCotizador.ObtenerDatosBenefiCierre(idGrupoFamiliar, num_solicitud);

                    List<Direccion> lstDireccion = servicioCotizador.ListarDireccion(gru.Afiliado.CUSPP);
                    gru.Residencia.cod_parametro = lstDireccion.FirstOrDefault().Departamento.Id;

                }

                if (solicitudIFP.Cotizaciones[0].Plan.Id == Enums.Planes.PLAN3.StringValue())
                {
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    SolicitudIFP solicitud_beneficiarios_cotizacion = servicioCotizador.ObtenerDatosSolicitudIFP(solicitudIFP.Id);

                    List<GrupoFamiliar> lstBeneficiarios = solicitud_beneficiarios_cotizacion.Beneficiarios;

                    if (lstBeneficiarios.Find(ben => ben.Identificacion.IdTipo == gru.Identificacion.IdTipo && ben.Identificacion.Numero == gru.Identificacion.Numero) != null)
                    {
                        gru.IndBloquearCampos = true;
                        //ModGruFamParentesco_RP.Attributes.Add("disabled", "disabled");

                        //ModGruFamSexo_RP.Attributes.Add("disabled", "disabled");

                        //ModGruFamFechaNacimiento_RP.Attributes.Add("disabled", "disabled");
                        //ModGruFamFechaNacimiento_RP.CssClass = "fecha formTextbox formCalendar formCalendarReadOnly";

                        //ModGruFamPorcentaje_RP.Attributes.Add("disabled", "disabled");
                        //ModGruFamPorcentaje_RP.CssClass = "formTextbox formTextboxReadOnly";
                    }

                }

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