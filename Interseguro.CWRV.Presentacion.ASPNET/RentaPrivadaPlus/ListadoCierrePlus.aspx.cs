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

using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;

using Microsoft.Reporting.WebForms;

using log4net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
{
    public partial class ListadoCierrePlus : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ListadoCierrePlus));
        private static IServicioCWRV servicioCotizador;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CierrePolizaParticularPlus))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                            LimpiarFormularios();

                            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
                            {
                                BusAfiCUSPP_RP.Text = Session["CUSPP"].ToString();
                                BusAfiBuscar_RP_Click(sender, e);
                            }
                            else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                            {
                                BusAfiNroSolicitud_RP.Text = Session["NroSolicitud"].ToString();
                                BusAfiBuscar_RP_Click(sender, e);
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.CierrePolizaParticularPlus.StringValue()));
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

        protected void BusAfiBuscar_RP_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
                    {
                        if (ValidarBusquedaAfiliados())
                        {
                            LimpiarFormularios();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();


                            if (BusAfiNroSolicitud_RP.Text.Length > 0 || BusAfiCUSPP_RP.Text.Length > 0)
                            {
                                Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(BusAfiNroSolicitud_RP.Text, BusAfiCUSPP_RP.Text, "", "", "");

                                log.Info("Usuario realizó búsqueda de afiliados por "
                                    + ((BusAfiNroSolicitud_RP.Text.Trim().Length != 0)
                                    ? ("Solicitud [" + BusAfiNroSolicitud_RP.Text.ToUpper() + "]")
                                    : ("CUSPP [" + BusAfiCUSPP_RP.Text.ToUpper() + "]")) + ".");


                                if (afiliado != null)
                                {
                                    log.Info("Afiliado encontrado:" + afiliado.CUSPP);
                                    //<SRIINI06326>

                                    //<INI.GTI_8284>
                                    // Guardar los datos del afiliado para generarlo como beneficiario en caso de que no exista
                                    servicioCotizador.ActualizarAfiliado(afiliado);
                                    //<FIN.GTI_8284>

                                    // MasterPage
                                    Panel cabecera, cabeceraProtegida;

                                    cabecera = (Panel)Master.FindControl("CabeceraSuperior");
                                    cabeceraProtegida = (Panel)Master.FindControl("CabeceraSuperiorProtegida");

                                    Session["Consentimiento"] = afiliado.Consentimiento;
                                    if (!afiliado.Consentimiento)
                                    {
                                        //<INI.GTI_8284>
                                        // Guardar los datos del afiliado para generarlo como beneficiario en caso de que no exista
                                        //servicioCotizador.ActualizarAfiliado(afiliado);
                                        //<FIN.GTI_8284>


                                        // MastePage
                                        cabecera.Visible = false;
                                        cabeceraProtegida.Visible = true;

                                        // Datos del Afiliado
                                        BusquedaAfiliados_RP.Visible = false;

                                        afiliado.ApellidoPaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoPaterno);
                                        afiliado.ApellidoMaterno = Utilitarios.EnmascararNombre(afiliado.ApellidoMaterno);
                                        afiliado.Nombre = Utilitarios.EnmascararNombre(afiliado.Nombre);


                                    }
                                    else
                                    {
                                        // MastePage
                                        cabecera.Visible = true;
                                        cabeceraProtegida.Visible = false;
                                    }
                                    //<SRIFIN06326>

                                    // Validar si el usuario tiene permiso para visualizar los datos del afiliado
                                    if (((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == afiliado.Agente.Id) || (string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.JefeOperaciones.StringValue())
                                    {
                                    
                                        if (BusAfiCUSPP_RP.Text.Trim().Length > 0)
                                        {
                                            Session["CUSPP"] = BusAfiCUSPP_RP.Text;
                                            Session["NroSolicitud"] = null;
                                            Session["CUSPP_PLUS"] = null;
                                        }
                                        else if (BusAfiNroSolicitud_RP.Text.Trim().Length > 0)
                                        {
                                            Session["CUSPP"] = null;
                                            Session["NroSolicitud"] = BusAfiNroSolicitud_RP.Text;
                                            Session["CUSPP_PLUS"] = afiliado.CUSPP.Trim();
                                        }

                                        HCUSPP_RP.Value = afiliado.CUSPP.Trim();

                                        log.Info("HCUSPP_RP.Value:" + HCUSPP_RP.Value);

                                        Session["Vendedor"] = afiliado.Agente.Id;
                                        Session["Cartera"] = afiliado.Agente.IdCartera;
                                        Session["AFP_RP"] = afiliado.AFP.Id.ToString();
                                        Session["CUSPP_RP"] = afiliado.CUSPP.ToString();

                                        HttpContext.Current.Session["indConsentimiento"] = afiliado.Consentimiento;  //JY

                                    }
                                    else
                                    {
                                        List<String> errores = new List<String>();
                                        errores.Add("Cliente no pertenece a su cartera de ventas. Verifique.");
                                        LimpiarFormularios();
                                        MCMMensaje.Text = Utilitarios.FormatearError(errores);
                                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                                        MCMEstado.Value = "1";
                                    }
                                }
                                else
                                {
                                    List<String> errores = new List<String>();
                                    if (BusAfiNroSolicitud_RP.Text.Trim().Length > 0)
                                    {
                                        errores.Add("Solicitud N° <strong>" + BusAfiNroSolicitud_RP.Text.ToUpper() + "</strong> no se encuentra registrada. Verifique.");
                                    }
                                    else
                                    {
                                        errores.Add("CUSPP <strong>" + BusAfiCUSPP_RP.Text.ToUpper() + "</strong> no se encuentra registrado. Verifique.");
                                    }
                                    LimpiarFormularios();
                                    MCMMensaje.Text = Utilitarios.FormatearError(errores);
                                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Informacion.StringValue();
                                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Informacion.StringValue();
                                    MCMEstado.Value = "1";
                                    HCUSPP_RP.Value = "-";
                                }
                            }
                            else
                            {
                                HCUSPP_RP.Value = "";
                                Session["CUSPP"] = null;
                                Session["NroSolicitud"] = null;
                                Session["CUSPP_PLUS"] = null;

                                Session["Vendedor"] = null;
                                Session["Cartera"] = null;
                                Session["AFP_RP"] = null;
                                Session["CUSPP_RP"] = null;
                                Session["ListadoContratante"] = null;

                            }
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.DatosAfiliadoConsultar.StringValue()));
                        MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["MensajeSinPermisos"] });
                        MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                        MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                        MCMEstado.Value = "1";
                    }
                }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
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

        private void LimpiarFormularios()
        {
            ModEnvCorDe.Text = String.Empty;
            ModEnvCorPara.Text = String.Empty;
            ModEnvCorAsunto.Text = String.Empty;
        }

        private bool ValidarBusquedaAfiliados()
        {
            bool esCorrecto = true;
            List<string> errores = new List<string>();

            MCMEstado.Value = "0";
            BusAfiNroSolicitud_RP.CssClass = "formTextbox";
            BusAfiCUSPP_RP.CssClass = "formTextbox";

            bool solicitud = (BusAfiNroSolicitud_RP.Text.Trim().Length > 0) ? true : false;
            bool cuspp = (BusAfiCUSPP_RP.Text.Trim().Length > 0) ? true : false;

            //if (!(solicitud | cuspp))
            //{
            //    errores.Add("Debe ingresar un criterio de búsqueda.");
            //    BusAfiNroSolicitud_RP.CssClass = "formTextbox formTextboxError";
            //    BusAfiCUSPP_RP.CssClass = "formTextbox formTextboxError";
            //    esCorrecto = false;
            //}

            if (solicitud & cuspp)
            {
                errores.Add("Sólo debe ingresar un criterio de búsqueda.");
                BusAfiNroSolicitud_RP.CssClass = "formTextbox formTextboxError";
                BusAfiCUSPP_RP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }

            if (cuspp && BusAfiCUSPP_RP.Text.Trim().Length != 12)
            {
                errores.Add("El <strong>CUSPP</strong> debe contener 12 caracteres.");
                BusAfiCUSPP_RP.CssClass = "formTextbox formTextboxError";
                esCorrecto = false;
            }

            if (!esCorrecto)
            {
                MCMMensaje.Text = Utilitarios.FormatearError(errores);
                MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Validacion.StringValue();
                MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Validacion.StringValue();
                MCMEstado.Value = "1";
                return false;
            }

            return true;
        }

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            Session["ComboMoneda"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Moneda];

            List<Parametro> comboModalidad = new List<Parametro>();
            comboModalidad.Add(new Parametro { Id = "I", Glosa = "I" });
            comboModalidad.Add(new Parametro { Id = "D", Glosa = "D" });
            comboModalidad.Add(new Parametro { Id = "I-RM", Glosa = "I-RM" });
            //<SRIINI18360>
            comboModalidad.Add(new Parametro { Id = "I-RC", Glosa = "I-RC" });
            //<SRIFIN18360>
            comboModalidad.Add(new Parametro { Id = "I-RB", Glosa = "I-RB" });
            Session["ComboModalidad"] = comboModalidad;

            List<Parametro> comboPeriodoDiferido = new List<Parametro>();
            comboPeriodoDiferido.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPeriodoDiferido.Add(new Parametro { Id = "1", Glosa = "1" });
            comboPeriodoDiferido.Add(new Parametro { Id = "2", Glosa = "2" });
            //<SRIINI18360>
            comboPeriodoDiferido.Add(new Parametro { Id = "3", Glosa = "3" });
            comboPeriodoDiferido.Add(new Parametro { Id = "4", Glosa = "4" });
            comboPeriodoDiferido.Add(new Parametro { Id = "5", Glosa = "5" });
            //<SRIFIN18360>
            Session["ComboPeriodoDiferido"] = comboPeriodoDiferido;

            List<Parametro> comboPorcentajeRentas = new List<Parametro>();
            comboPorcentajeRentas.Add(new Parametro { Id = "0", Glosa = "0" });
            comboPorcentajeRentas.Add(new Parametro { Id = "50", Glosa = "50%" });
            Session["ComboPorcentajeRentas"] = comboPorcentajeRentas;

            //<INIGTI_753>
            //List<Parametro> comboPeriodoGarantizado = new List<Parametro>();
            //comboPeriodoGarantizado.Add(new Parametro { Id = "0", Glosa = "0" });
            //comboPeriodoGarantizado.Add(new Parametro { Id = "10", Glosa = "10" });
            //comboPeriodoGarantizado.Add(new Parametro { Id = "15", Glosa = "15" });
            //Session["ComboPeriodoGarantizado"] = comboPeriodoGarantizado;

            //<FINGTI_753>

            Session["ComboCapital"] = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Capital];

            //<INIGTI_753>
            //var montosCIC = servicioCotizador.ListarMontoCIC();
            //<FINGTI_753>

            List<Parametro> lstParametro = new List<Parametro>();
            lstParametro = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.PagoEscalonado];

            // Permisos Modal Búsqueda de Afiliados
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.BusquedaAfiliadoConsultar))
            {
                PerBusAfiExaminarSolicitud_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(BusAfiExaminarSolicitud_RP);
                InhabilitarControl(ModBusAfiApellidoPaterno_RP);
                InhabilitarControl(ModBusAfiApellidoMaterno_RP);
                InhabilitarControl(ModBusAfiNombres_RP);
                InhabilitarControl(ModBusAfiBuscar_RP);
                PerBusAfiExaminarSolicitud_RP.Value = "0";
            }

            // Permisos Consultar Afiliado
            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DatosAfiliadoConsultar))
            {
                PerBusAfiBuscar_RP.Value = "1";
            }
            else
            {
                InhabilitarControl(BusAfiNroSolicitud_RP);
                InhabilitarControl(BusAfiCUSPP_RP);
                InhabilitarControl(BusAfiBuscar_RP);
                PerBusAfiBuscar_RP.Value = "0";
            }

            string usuario = HttpContext.Current.Session["Usuario"].ToString();

            //Session["ListadoEstadoCivil"] = servicioCotizador.ListarEstadoCivil(usuario);

            //Session["ListadoProfesion"] = servicioCotizador.ListarProfesion(usuario);

            //Session["ListadoNacionalidad"] = servicioCotizador.ListarNacionalidad(usuario);

            //JArray listaDepartamentos = new JArray();            
            //string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
            //var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
            //urlDepartamentos = string.Format(urlDepartamentos, usuario);
            //listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, usuario, listaDepartamentos);
            //Session["ListadoDepartamento"] = listaDepartamentos.ToObject<List<Departamento>>();

        }

        public void InhabilitarControl(Control control)
        {
            if (control is TextBox)
            {
                ((TextBox)control).ReadOnly = true;
                ((TextBox)control).CssClass = "formTextbox formTextboxReadOnly";
            }
            if (control is DropDownList)
            {
                ((DropDownList)control).Enabled = false;
                ((DropDownList)control).CssClass = "formCombobox formComboboxReadOnly";
            }
            if (control is HyperLink)
            {
                ((HyperLink)control).CssClass = "botonDeshabilitado gris gris_sharp";
                ((HyperLink)control).ToolTip = ConfigurationManager.AppSettings["MensajeSinPermisos"];
            }
            if (control is Button)
            {
                ((Button)control).CssClass = "botonDeshabilitado gris gris_sharp";
                ((Button)control).ToolTip = ConfigurationManager.AppSettings["MensajeSinPermisos"];
            }
        }

        [WebMethod]
        public static string CargarTablaSolicitudes(string tokenUsuario, string cuspp)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio ListadoCierrePlus.CargarTablaSolicitudes WebMethod");

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaSolicitudesRentaPrivadaPlusCierre)pagina.LoadControl("~/Controles/TablaSolicitudesRentaPrivadaPlusCierre.ascx");

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusConsultar))
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<SolicitudRPPlus> solicitudes = servicioCotizador.ListarSolicitudCierresPlus(cuspp);
                            log.Info(string.Format("Cuspp:{0} Cantidad de Solicidutes: {1}", cuspp, solicitudes.Count));

                            var nroSolicitud = "";

                            if (cuspp != "")
                                nroSolicitud = (HttpContext.Current.Session["NroSolicitud"] != null) ? HttpContext.Current.Session["NroSolicitud"].ToString() : "";

                            log.Info(string.Format("Solicitud Individual Encontrada:{0} ", nroSolicitud));

                            //<INI.GTI_7012_26>
                            //var idEstados = new[] { 1, 2 };
                            var idEstados = new[] { 2, 6 };
                            var idEstadosPlaft = new[] { 0,-1, 3 };

                            control.Solicitudes = (nroSolicitud != "") ? solicitudes.FindAll(p => p.Id.ToUpper() == nroSolicitud.ToUpper() && idEstados.Contains(p.CodigoEstado) && idEstadosPlaft.Contains(p.CodigoEstadoPlaft)).ToList() : solicitudes.FindAll(p => idEstados.Contains(p.CodigoEstado) && idEstadosPlaft.Contains(p.CodigoEstadoPlaft)).ToList();
                            //<INI.GTI_7012_26>

                            control.PermisoConsultar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusConsultar)) ? true : false;
                            control.PermisoCerrar = (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusGenerarPoliza)) ? true : false;

                            log.Info(string.Format("Cantidad de Solicidutes Filtrada:{0} ", control.Solicitudes.Count));
                        }
                        else
                        {
                            control.PermisoConsultar = false;
                        }

                        pagina.Controls.Add(control);

                        string html = "";
                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            html = sw.ToString();
                        }

                        log.Debug("Fin ListadoCierrePlus.CargarTablaSolicitudes WebMethod");

                        return html;
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        log.Debug("Fin ListadoCierrePlus.CargarTablaSolicitudes WebMethod");

                        return Constante.COD_TOKEN;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
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

    }
}