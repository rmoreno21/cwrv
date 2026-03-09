using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
{
    public partial class ResumenBeneficiariosPG : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ResumenBeneficiariosPG));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionesIFP))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            ModGruFamsolicitud.Value = Convert.ToString((Session["Solicitud"]));
                            ModGruNumCorrelativo.Value = Convert.ToString((Session["NumCorrelativo"]));

                            SolicitudIFP objetoSolicitud = new SolicitudIFP();

                            if (Page.PreviousPage != null)
                            {
                                var solicitudSerializado = (HiddenField)Page.PreviousPage.Form.FindControl("Contenido").FindControl("HSolicitudSerializado");
                                if (solicitudSerializado.Value == null)
                                {
                                    Response.Redirect("Cotizador.aspx");
                                }

                                if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
                                {
                                    HCUSPP.Value = Convert.ToString((Session["CUSPP"]));
                                }
                                else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                                {
                                    HCUSPP.Value = Convert.ToString((Session["CUSPP_PLUS"]));
                                }

                                objetoSolicitud = JsonConvert.DeserializeObject<SolicitudIFP>(solicitudSerializado.Value);
                                if (objetoSolicitud.Cotizaciones == null)
                                {
                                    Response.Redirect("Cotizador.aspx");
                                }

                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                List<Beneficiario> beneficiarios = servicioCotizador.ListarBeneficiarios(ModGruFamsolicitud.Value, Session["Usuario"].ToString());
                                Beneficiario afiliado = beneficiarios.Where(g => g.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue()).FirstOrDefault();
                                HNombres.Value = afiliado.Nombre.ToString() + " " + afiliado.ApellidoPaterno.ToString() + " " + afiliado.ApellidoMaterno.ToString();
                                HCorreo.Value = afiliado.CorreoElectronico.ToString();
                                HTipoDocumento.Value = afiliado.Identificacion.GlosaTipo.ToString();
                                HNumeroDocumento.Value = afiliado.Identificacion.Numero.ToString();
                                HTelefono.Value = afiliado.numCelular.ToString();
                                HToken.Value = afiliado.firmaDigital.gls_token.ToString();
                                HSoloNombre.Value = afiliado.Nombre.ToString();

                                var lstBeneficiarios = servicioCotizador.ObtenerDatosporSolicitudIFP(objetoSolicitud.Id);

                                var titular = lstBeneficiarios.Find(b => b.cod_parentesco_beneficiario == Enums.Parentesco.Afiliado.StringValue());
                                HPEP.Value = titular.ind_PEP_afiliado.ToString();

                                List<GrupoFamiliar> grupos = servicioCotizador.ListarGrupoFamiliar(HCUSPP.Value).Where(g => g.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue()).ToList();

                                SolicitudIFP lstSolicitudIFP = objetoSolicitud;
                                List<GrupoFamiliar> lstGrupoFamiliar = lstSolicitudIFP.Beneficiarios;

                                foreach (var itemIdentificacion in lstGrupoFamiliar)
                                {
                                    var numeroIdentificacion = itemIdentificacion.Identificacion.Numero;
                                    string nuevoNumeroIdentificacion = string.Empty;
                                    int cantidadNumeros = numeroIdentificacion.Count();
                                    if (itemIdentificacion.Identificacion.IdTipo == "D")
                                    {
                                        for (int item = 1; item <= (cantidadNumeros - 8); item++)
                                        {
                                            nuevoNumeroIdentificacion += "0";
                                        }
                                        itemIdentificacion.Identificacion.Numero = nuevoNumeroIdentificacion + "" + numeroIdentificacion;
                                    }
                                    if (itemIdentificacion.Identificacion.IdTipo == "E")
                                    {
                                        for (int item = 1; item <= (cantidadNumeros - 9); item++)
                                        {
                                            nuevoNumeroIdentificacion += "0";
                                        }
                                        itemIdentificacion.Identificacion.Numero = nuevoNumeroIdentificacion + "" + numeroIdentificacion;
                                    }

                                    //if (itemIdentificacion.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                                    //{
                                    //    itemIdentificacion.Ind_Cierre = true;
                                    //}
                                }

                                foreach (var itemGrupo in grupos)
                                {
                                    var numeroIdentificacion = itemGrupo.Identificacion.Numero;
                                    string nuevoNumeroIdentificacion = string.Empty;
                                    int cantidadNumeros = numeroIdentificacion.Count();
                                    if (itemGrupo.Identificacion.IdTipo == "D")
                                    {
                                        for (int item = 1; item <= (cantidadNumeros - 8); item++)
                                        {
                                            nuevoNumeroIdentificacion += "0";
                                        }
                                        numeroIdentificacion = nuevoNumeroIdentificacion + "" + numeroIdentificacion;
                                    }
                                    if (itemGrupo.Identificacion.IdTipo == "E")
                                    {
                                        for (int item = 1; item <= (cantidadNumeros - 9); item++)
                                        {
                                            nuevoNumeroIdentificacion += "0";
                                        }
                                        numeroIdentificacion = nuevoNumeroIdentificacion + "" + numeroIdentificacion;
                                    }

                                    //if (lstGrupoFamiliar.Where(gf => gf.Id == itemGrupo.Id).ToList().Count == 0 && lstGrupoFamiliar.Where(gf => gf.IdGrupoFamiliar == itemGrupo.Id).ToList().Count == 0)
                                    if (lstGrupoFamiliar.Find(gf => gf.Identificacion.IdTipo == itemGrupo.Identificacion.IdTipo
                                                                    && gf.Identificacion.Numero == numeroIdentificacion) == null)
                                    {
                                        //itemGrupo.Ind_Cierre = true;
                                        lstGrupoFamiliar.Add(itemGrupo);
                                    }

                                }

                                foreach (var grupo in lstGrupoFamiliar)
                                {
                                    if (grupo.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                                    {
                                        if (lstSolicitudIFP.Cotizaciones[0].Plan.Id == Enums.Planes.PLAN3.StringValue())
                                        {
                                            SolicitudIFP solicitud = servicioCotizador.ObtenerDatosSolicitudIFP(Session["Solicitud"].ToString());
                                            solicitud.Beneficiarios = solicitud.Beneficiarios.FindAll(ben => ben.IdTipoPeriodoBeneficiario.ToString() == Enums.TipoPeriodoBeneficiario.Garantizada.StringValue());

                                            if (solicitud.Beneficiarios.Find(gf => gf.Identificacion.IdTipo == grupo.Identificacion.IdTipo
                                                                    && gf.Identificacion.Numero == grupo.Identificacion.Numero) != null)
                                            {
                                                var porcentajeBeneficiario = solicitud.Beneficiarios.Find(gf => gf.Identificacion.IdTipo == grupo.Identificacion.IdTipo
                                                                                                            && gf.Identificacion.Numero == grupo.Identificacion.Numero);
                                                grupo.Ind_Cierre = true;
                                                grupo.ValPjeRenta = porcentajeBeneficiario.ValPjeBeneficiario;
                                            }
                                        }
                                        else
                                        {
                                            //if (grupo.ValPjeRenta == 0)
                                            //{
                                            var grupoFamiliar = servicioCotizador.ObtenerDatosGrupoFamiliar(Convert.ToInt32(grupo.IdGrupoFamiliar), ModGruFamsolicitud.Value);
                                            var pje = 0.00;

                                            if (grupoFamiliar != null)
                                            {
                                                pje = grupoFamiliar.ValPjeRenta;
                                            }

                                            grupo.ValPjeRenta = pje;

                                            grupo.Ind_Cierre = false;
                                            if (pje > 0)
                                            {
                                                grupo.Ind_Cierre = true;
                                            }
                                            //}
                                        }

                                    }
                                }

                                // Varificar si ya tiene un registro en firma digital
                                FirmaDigital firma;
                                string urlEndpoint = string.Format("{0}/firmas-digitales/por-solicitud/{1}/1/{2}", ConfigurationManager.AppSettings["url_base_api_cwrv"], ModGruFamsolicitud.Value, Session["Usuario"]);
                                log.Debug(string.Format("Inicio: GET [{0}]", urlEndpoint));
                                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlEndpoint);
                                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                                {
                                    string responseBody = streamReader.ReadToEnd();
                                    firma = JsonConvert.DeserializeObject<FirmaDigital>(responseBody);
                                }
                                log.Debug(string.Format("Fin: GET [{0}]", urlEndpoint));

                                // Habilitar botón de Reenvío Manual si corresponde
                                if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.PlantillaCorreoElectronico) && firma != null)
                                {
                                    ReenvioManual.Visible = true;
                                    ReenvioManual.NavigateUrl = ResolveUrl(string.Format("~/Comun/PlantillaCorreoElectronico.aspx?tp=4&s={0}&i=1", ModGruFamsolicitud.Value));
                                }

                                HSolicitudSerializado.Value = JsonConvert.SerializeObject(objetoSolicitud);

                                ModSolSiguiente_RP.PostBackUrl = "~/RentaIFP/SeleccionSolicitud.aspx";
                            }
                            else
                            {
                                Response.Redirect("Cotizador.aspx");
                            }
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.CotizacionesIFP.StringValue()));
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
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }

        }

        [WebMethod]
        public static string CargarTablaBeneficiarios(string tokenUsuario, string solicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    string html = "";

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();

                        var solicitudSerializado = JsonConvert.DeserializeObject<SolicitudIFP>(solicitud);
                        List<GrupoFamiliar> lstGrupoFamiliar = solicitudSerializado.Beneficiarios;
                        //List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["ListaGrupoFamiliarCierre"];

                        if (lstGrupoFamiliar == null)
                            lstGrupoFamiliar = new List<GrupoFamiliar>();

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                        List<Parametro> lstparametroParentesco = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Parentesco];
                        List<Parametro> lstparametroSexo = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo];

                        lstGrupoFamiliar.ForEach(b =>
                        {
                            b.Parentesco.Nombre = lstparametroParentesco.FindAll(x => x.Id == b.Parentesco.Id).FirstOrDefault().Glosa;
                            //b.Sexo = Convert.ToChar(lstparametroSexo.FindAll(s => s.Id == b.Sexo.ToString()).FirstOrDefault().Glosa);
                        });

                        var control = (TablaBeneficiarioIFP_PG)pagina.LoadControl("~/Controles/TablaBeneficiarioIFP_PG.ascx");

                        //control.lstBeneficiarios = lstGrupoFamiliar;
                        control.lstBeneficiarios = lstGrupoFamiliar.FindAll(b => b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue());
                        control.lstParametroSexo = lstparametroSexo;
                        //HttpContext.Current.Session["Beneficiarios"] = control.lstBeneficiarios;

                        pagina.Controls.Add(control);

                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            html = sw.ToString();
                        }
                    }
                    return html;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Respuesta ModificarPorcentaje(string tokenUsuario, int posicion, int porcentaje, string solicitud)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {

                Respuesta respuesta = new Respuesta();

                try
                {

                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        var solicitudSerializado = JsonConvert.DeserializeObject<SolicitudIFP>(solicitud);
                        List<GrupoFamiliar> lstGrupoFamiliar = solicitudSerializado.Beneficiarios;
                        //List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["ListaGrupoFamiliarCierre"];

                        if (lstGrupoFamiliar == null)
                            lstGrupoFamiliar = new List<GrupoFamiliar>();

                        //cierre de varios planes
                        var listaAfiliados = lstGrupoFamiliar.FindAll(ben => ben.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());
                        int cantidadAfiliados = listaAfiliados.Count() - 1;

                        bool indicadorCierre = lstGrupoFamiliar[posicion + cantidadAfiliados].Ind_Cierre;

                        if (indicadorCierre)
                        {
                            lstGrupoFamiliar[posicion + cantidadAfiliados].ValPjeRenta = porcentaje;
                        }

                        //HttpContext.Current.Session["ListaGrupoFamiliarCierre"] = lstGrupoFamiliar;

                        double totalPorcentaje = 0;
                        for (int i = 0; i < lstGrupoFamiliar.Count; i++)
                        {
                            if (lstGrupoFamiliar[i].Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                            {
                                totalPorcentaje += lstGrupoFamiliar[i].ValPjeRenta;
                            }

                        }

                        solicitudSerializado.Beneficiarios = lstGrupoFamiliar;
                        string objSolicitudRespuesta = JsonConvert.SerializeObject(solicitudSerializado);

                        respuesta.Mensaje = objSolicitudRespuesta;
                        respuesta.Estado = "OK";
                        respuesta.Contenido = totalPorcentaje.ToString();
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
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static Respuesta ModificarCheckBox(string tokenUsuario, int posicion, int indicador, string solicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {

                Respuesta respuesta = new Respuesta();

                try
                {

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        var solicitudSerializado = Newtonsoft.Json.JsonConvert.DeserializeObject<SolicitudIFP>(solicitud);
                        List<GrupoFamiliar> lstGrupoFamiliar = solicitudSerializado.Beneficiarios;
                        //List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["ListaGrupoFamiliarCierre"];

                        bool ind_seleccionado = false;

                        if (lstGrupoFamiliar == null)
                            lstGrupoFamiliar = new List<GrupoFamiliar>();

                        if (indicador == 1)
                        {
                            ind_seleccionado = true;
                        }

                        //cierre de varios planes
                        var listaAfiliados = lstGrupoFamiliar.FindAll(ben => ben.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());
                        int cantidadAfiliados = listaAfiliados.Count() - 1;

                        lstGrupoFamiliar[posicion + cantidadAfiliados].Ind_Cierre = ind_seleccionado;

                        if (!ind_seleccionado)
                        {
                            lstGrupoFamiliar[posicion + cantidadAfiliados].ValPjeRenta = 0;
                        }

                        //HttpContext.Current.Session["ListaGrupoFamiliarCierre"] = lstGrupoFamiliar;

                        solicitudSerializado.Beneficiarios = lstGrupoFamiliar;
                        string objSolicitudRespuesta = Newtonsoft.Json.JsonConvert.SerializeObject(solicitudSerializado);

                        respuesta.Mensaje = objSolicitudRespuesta;
                        respuesta.Estado = "OK";
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                       ex.Source, ex.Message, ex.StackTrace));
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    return respuesta;
                }
            }
        }

        [WebMethod]
        public static Respuesta VistaPreviaIFP(string tokenUsuario, string numSolicitud, string numCorrelativo, string numCuspp, string indPEP, string codPlan, Solicitud objSolicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusCerrar))
                        {
                            log.Info("Accediendo a las Key necesarias");
                            //string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();

                            //string urlFormatoSolicitud = ConfigurationManager.AppSettings["url_formato_solicitud_ifp"].ToString();
                            //string urlFormatoOrigenFondo = ConfigurationManager.AppSettings["url_formato_origen_fondo"].ToString();
                            //string urlFormatoPEP = ConfigurationManager.AppSettings["url_formato_pep"].ToString();
                            //string urlFormatoConstanciaAbono = ConfigurationManager.AppSettings["url_formato_constancia_abono"].ToString();
                            //string urlFormatoDetalleCotizacion = ConfigurationManager.AppSettings["url_formato_detalle_cotizacion"].ToString();

                            string usuario = (string)HttpContext.Current.Session["Usuario"].ToString();
                            //string token_generado = string.Empty;

                            //log.Info("Guardando dirección");
                            //servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            //Respuesta direccion = servicioCotizador.ActualizarDireccionSolicitud(numCuspp, numSolicitud, usuario);

                            log.Info("Validar indicador cierre de beneficiarios");
                            foreach (var benpg in objSolicitud.Beneficiarios)
                            {
                                if (benpg.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                                {
                                    if (benpg.ValPjeRenta == 0)
                                    {
                                        benpg.Ind_Cierre = false;
                                    }
                                }
                            }

                            log.Info("Guardando beneficiario");
                            List<GrupoFamiliar> lstGrupoFamiliar = objSolicitud.Beneficiarios.FindAll(b => b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue() && b.Ind_Cierre);
                            lstGrupoFamiliar.ForEach(p => p.Afiliado = new Afiliado { CUSPP = numCuspp });
                            lstGrupoFamiliar.ForEach(p => p.Usuario = new Usuario { NombreUsuario = usuario });

                            //for (int item = 0; item < objSolicitud.Beneficiarios.Count; item++)
                            for (int item = 0; item < lstGrupoFamiliar.Count; item++)
                            {
                                var numeroDni = lstGrupoFamiliar[item].Identificacion.Numero;

                                var beneficiarios = lstGrupoFamiliar.FindAll(gf => gf.Identificacion.Numero == numeroDni);

                                if (beneficiarios.Count > 1)
                                {
                                    log.Error("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                    throw new Exception("Error al administrar los beneficiarios, hay números de DNI iguales.");
                                }
                            }

                            //var respustaBeneficiario = servicioCotizador.CerrarBeneficiariosIFP(numSolicitud, lstGrupoFamiliar, usuario);

                            if (lstGrupoFamiliar != null)
                            {
                                servicioCotizador = LocalizadorProxy.ObtenerServicio();

                                //todos los beneficiarios modificados
                                List<GrupoFamiliar> lstGrupoFamiliarTodos = objSolicitud.Beneficiarios.FindAll(b => b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue());

                                //beneficiarios no garantizados
                                SolicitudIFP solicitudIFP = servicioCotizador.ObtenerDatosSolicitudIFP(numSolicitud);
                                List<GrupoFamiliar> lstBeneficiariosPNoG = solicitudIFP.Beneficiarios.FindAll(ben => ben.IdTipoPeriodoBeneficiario.ToString() == Enums.TipoPeriodoBeneficiario.NoGarantizada.StringValue());

                                List<GrupoFamiliar> lstBeneficiariosPNoGModificados = new List<GrupoFamiliar>();

                                foreach (var ben in lstBeneficiariosPNoG)
                                {
                                    var benefi = lstGrupoFamiliarTodos.Find(PNoG => PNoG.Identificacion.IdTipo == ben.Identificacion.IdTipo
                                                                            && PNoG.Identificacion.Numero == ben.Identificacion.Numero);

                                    if (benefi != null)
                                    {
                                        benefi.Id = ben.Id;
                                        benefi.SolicitudIFP = new SolicitudIFP { Id = numSolicitud };
                                        benefi.Afiliado = new Afiliado { CUSPP = numCuspp };
                                        benefi.Usuario = new Usuario { NombreUsuario = usuario };

                                        lstBeneficiariosPNoGModificados.Add(benefi);
                                    }
                                }

                                if (lstGrupoFamiliar.Count > 0)
                                {
                                    var respustaBeneficiario = servicioCotizador.CerrarBeneficiariosIFP(numSolicitud, lstGrupoFamiliar, codPlan, lstBeneficiariosPNoGModificados, usuario);

                                    if (respustaBeneficiario.Estado == Constante.COD_ERROR)
                                    {
                                        log.Error("Error al administrar los beneficiarios");
                                        throw new Exception("Error al administrar los beneficiarios.");
                                    }

                                }
                                else
                                {
                                    if (codPlan == Enums.Planes.PLAN3.StringValue() && lstBeneficiariosPNoGModificados.Count() > 0)
                                    {
                                        var respustaBeneficiario = servicioCotizador.CerrarBeneficiariosIFP(numSolicitud, lstGrupoFamiliar, codPlan, lstBeneficiariosPNoGModificados, usuario);

                                        if (respustaBeneficiario.Estado == Constante.COD_ERROR)
                                        {
                                            log.Error("Error al administrar los beneficiarios");
                                            throw new Exception("Error al administrar los beneficiarios.");
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
                                log.Warn(String.Format("No se ha podido resolver el nombre de terminal para la IP [{0}].", HttpContext.Current.Request.ServerVariables["remote_addr"]));
                            }

                            nombreTerminal += HttpContext.Current.Request.UserAgent;

                            string detalle = String.Format("Método: {0} {1} Parámetros: {2} {3} - {4}: {5} ", "CerrarBeneficiariosIFP", Environment.NewLine, Environment.NewLine, Environment.NewLine, "Número Solicitud", JsonConvert.SerializeObject(numSolicitud));
                            if (lstGrupoFamiliar != null)
                            {
                                detalle = String.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} {5} - {6}: {7} ", "CerrarBeneficiariosIFP", Environment.NewLine, Environment.NewLine, "Lista Grupo Familiar", JsonConvert.SerializeObject(lstGrupoFamiliar), Environment.NewLine, "Número Solicitud", JsonConvert.SerializeObject(numSolicitud));
                            }

                            servicioCotizador.RegistrarLog(new LogBD
                            {
                                IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                NombreTerminal = nombreTerminal,
                                IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                Detalle = detalle,
                                IdTipoEvento = Enums.EventoLog.RegistrarDatosBeneficiario.StringValue()
                            });

                            //log.Info("Consumiendo servicio token: " + urlToken);
                            ///*Obtener token*/
                            //var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                            //httpWebRequest.ContentType = "application/json";
                            //httpWebRequest.Method = "POST";

                            //log.Info("Pasando el json al servicio");
                            //using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            //{
                            //    string json = "{\"usuario\": \"" + usuario + "\"}";

                            //    streamWriter.Write(json);
                            //    streamWriter.Flush();
                            //    streamWriter.Close();
                            //}

                            //log.Info("Leyendo el servicio");
                            //var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            //using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            //{
                            //    var jsonResult = streamReader.ReadToEnd();
                            //    JObject jObject = JObject.Parse(jsonResult);
                            //    token_generado = (string)jObject["accessToken"];
                            //}

                            //if (token_generado != "")
                            //{
                            //log.Info("Consumiendo servicio solicitud: " + urlFormatoSolicitud);

                            //urlFormatoSolicitud = string.Format(urlFormatoSolicitud, numSolicitud, usuario);
                            //urlFormatoOrigenFondo = string.Format(urlFormatoOrigenFondo, numSolicitud, usuario);
                            //urlFormatoConstanciaAbono = string.Format(urlFormatoConstanciaAbono, numSolicitud, usuario);
                            //urlFormatoDetalleCotizacion = string.Format(urlFormatoDetalleCotizacion, numSolicitud, usuario);

                            log.Info("Eliminar formatos generados");
                            string rutaCarpeta = System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\Plantilla\\IFP\\";
                            string archivoSolicitud = "Solicitud.pdf";
                            string archivoOrigenFondos = "OrigenFondos.pdf";
                            string archivoConstanciaAbono = "ConstanciaAbono.pdf";
                            string archivoPersonaExpuestaPoliticamente = "PersonaExpuestaPoliticamente.pdf";
                            string archivoDetalleCotizacion = "DetalleCotizacion.pdf";

                            if (File.Exists(rutaCarpeta + archivoSolicitud))
                            {
                                File.Delete(rutaCarpeta + archivoSolicitud);
                            }

                            if (File.Exists(rutaCarpeta + archivoOrigenFondos))
                            {
                                File.Delete(rutaCarpeta + archivoOrigenFondos);
                            }

                            if (File.Exists(rutaCarpeta + archivoConstanciaAbono))
                            {
                                File.Delete(rutaCarpeta + archivoConstanciaAbono);
                            }

                            if (File.Exists(rutaCarpeta + archivoPersonaExpuestaPoliticamente))
                            {
                                File.Delete(rutaCarpeta + archivoPersonaExpuestaPoliticamente);
                            }

                            if (File.Exists(rutaCarpeta + archivoDetalleCotizacion))
                            {
                                File.Delete(rutaCarpeta + archivoDetalleCotizacion);
                            }

                            //respuesta.Archivos = new List<string>();

                            //byte[] formatoSolicitudArray = ConsumirServicio(urlFormatoSolicitud, usuario, token_generado);
                            //byte[] formatoOFArray = ConsumirServicio(urlFormatoOrigenFondo, usuario, token_generado);
                            //byte[] formatoConstanciaArray = ConsumirServicio(urlFormatoConstanciaAbono, usuario, token_generado);
                            //byte[] formatoDetalleCotizacionArray = ConsumirServicio(urlFormatoDetalleCotizacion, usuario, token_generado);

                            //respuesta.Archivos.Add(archivoSolicitud);
                            //File.WriteAllBytes(rutaCarpeta + archivoSolicitud, formatoSolicitudArray);

                            //respuesta.Archivos.Add(archivoOrigenFondos);
                            //File.WriteAllBytes(rutaCarpeta + archivoOrigenFondos, formatoOFArray);

                            //respuesta.Archivos.Add(archivoConstanciaAbono);
                            //File.WriteAllBytes(rutaCarpeta + archivoConstanciaAbono, formatoConstanciaArray);

                            //respuesta.Archivos.Add(archivoDetalleCotizacion);
                            //File.WriteAllBytes(rutaCarpeta + archivoDetalleCotizacion, formatoDetalleCotizacionArray);

                            //bool PEP = false;
                            //if (indPEP != null)
                            //{
                            //    if (indPEP == "S" || indPEP == "Si" || indPEP == "Sí")
                            //    {
                            //        PEP = true;
                            //    }
                            //    else
                            //    {
                            //        PEP = false;
                            //    }
                            //}
                            //else
                            //{
                            //    PEP = false;
                            //}

                            //if (PEP)
                            //{
                            //    urlFormatoPEP = string.Format(urlFormatoPEP, numSolicitud, usuario);
                            //    byte[] formatoPEPArray = ConsumirServicio(urlFormatoPEP, usuario, token_generado);
                            //    respuesta.Archivos.Add(archivoPersonaExpuestaPoliticamente);
                            //    File.WriteAllBytes(rutaCarpeta + archivoPersonaExpuestaPoliticamente, formatoPEPArray);
                            //}

                            //indicador de rescate 
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            int ind_rescate = servicioCotizador.ObtenerIndicadorRescateIFP(numSolicitud, usuario);

                            if (ind_rescate > 0)
                            {
                                ind_rescate = 1;
                            }
                            else
                            {
                                ind_rescate = 0;
                            }

                            respuesta.Estado = Constante.COD_OK;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                            respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "formatos generados correctamente." });
                            respuesta.Contenido = ind_rescate.ToString();
                            //}

                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.SolicitudPlusActualizar.StringValue()));
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
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    Respuesta respuesta = new Respuesta();
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    return respuesta;
                }
            }
        }

        private static byte[] ConsumirServicio(string url, string usuario, string token_generado)
        {
            log.Info("Leyendo el servicio: " + url);
            WebClient myWebClient = new WebClient();
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(usuario + ":" + token_generado));
            myWebClient.Headers[HttpRequestHeader.Authorization] = string.Format("Basic {0}", credentials);
            byte[] formatoByteArray = myWebClient.DownloadData(url);
            myWebClient.Dispose();
            return formatoByteArray;
        }

        [WebMethod]
        public static Respuesta EnviarSADP(string nombres, string numSolicitud, int num_item, string correo, string token, string numCuspp, string codPlan, Solicitud objSolicitud)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    log.Debug("Inicio metodo EnviarSADP (ResumenBeneficiariosPG)");

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                    string usuario = HttpContext.Current.Session["Usuario"].ToString();

                    log.Debug("numSolicitud: " + numSolicitud);
                    log.Debug("usuario: " + usuario);
                    log.Debug("numCuspp: " + numCuspp);

                    // Validar dirección del afiliado
                    log.Debug("Validando dirección");
                    ValidarDireccionAfiliado(numCuspp);
                    
                    // Guardar dirección del afiliado
                    log.Debug("Guardando dirección");
                    Respuesta direccion = servicioCotizador.ActualizarDireccionSolicitud(numCuspp, numSolicitud, usuario);

                    log.Info("Validar indicador cierre de beneficiarios");
                    foreach (var benpg in objSolicitud.Beneficiarios)
                    {
                        if (benpg.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                        {
                            if (benpg.ValPjeRenta == 0)
                            {
                                benpg.Ind_Cierre = false;
                            }
                        }
                    }

                    log.Info("Guardando beneficiario");
                    List<GrupoFamiliar> lstGrupoFamiliar = objSolicitud.Beneficiarios.FindAll(b => b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue() && b.Ind_Cierre);
                    lstGrupoFamiliar.ForEach(p => p.Afiliado = new Afiliado { CUSPP = numCuspp });
                    lstGrupoFamiliar.ForEach(p => p.Usuario = new Usuario { NombreUsuario = usuario });

                    //for (int item = 0; item < objSolicitud.Beneficiarios.Count; item++)
                    for (int item = 0; item < lstGrupoFamiliar.Count; item++)
                    {
                        var numeroDni = lstGrupoFamiliar[item].Identificacion.Numero;

                        var beneficiarios = lstGrupoFamiliar.FindAll(gf => gf.Identificacion.Numero == numeroDni);

                        if (beneficiarios.Count > 1)
                        {
                            log.Error("Error al administrar los beneficiarios, hay números de DNI repetidos.");
                            throw new Exception("Error al validar la información de los beneficiarios, se han encontrado documentos de identidad con números iguales");
                        }
                    }


                    if (lstGrupoFamiliar != null)
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();

                        //todos los beneficiarios modificados
                        List<GrupoFamiliar> lstGrupoFamiliarTodos = objSolicitud.Beneficiarios.FindAll(b => b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue());

                        //beneficiarios no garantizados
                        SolicitudIFP solicitudIFPCabecera = servicioCotizador.ObtenerDatosSolicitudIFP(numSolicitud);
                        List<GrupoFamiliar> lstBeneficiariosPNoG = solicitudIFPCabecera.Beneficiarios.FindAll(ben => ben.IdTipoPeriodoBeneficiario.ToString() == Enums.TipoPeriodoBeneficiario.NoGarantizada.StringValue());

                        List<GrupoFamiliar> lstBeneficiariosPNoGModificados = new List<GrupoFamiliar>();

                        foreach (var ben in lstBeneficiariosPNoG)
                        {
                            var benefi = lstGrupoFamiliarTodos.Find(PNoG => PNoG.Identificacion.IdTipo == ben.Identificacion.IdTipo
                                                                    && PNoG.Identificacion.Numero == ben.Identificacion.Numero);

                            if (benefi != null)
                            {
                                benefi.Id = ben.Id;
                                benefi.SolicitudIFP = new SolicitudIFP { Id = numSolicitud };
                                benefi.Afiliado = new Afiliado { CUSPP = numCuspp };
                                benefi.Usuario = new Usuario { NombreUsuario = usuario };

                                lstBeneficiariosPNoGModificados.Add(benefi);
                            }
                        }

                        if (lstGrupoFamiliar.Count > 0)
                        {
                            //TODO: pasar tipo plan, lstGrupoFamiliar
                            var respustaBeneficiario = servicioCotizador.CerrarBeneficiariosIFP(numSolicitud, lstGrupoFamiliar, codPlan, lstBeneficiariosPNoGModificados, usuario);
                        }
                        else
                        {
                            if (codPlan == Enums.Planes.PLAN3.StringValue() && lstBeneficiariosPNoGModificados.Count() > 0)
                            {
                                var respustaBeneficiario = servicioCotizador.CerrarBeneficiariosIFP(numSolicitud, lstGrupoFamiliar, codPlan, lstBeneficiariosPNoGModificados, usuario);
                            }
                        }
                    }

                    GrupoFamiliar afiliado = objSolicitud.Beneficiarios.Find(gf => gf.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

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

                    string detalle = String.Format("Método: {0} {1} Parámetros: {2} {3} - {4}: {5} ", "CerrarBeneficiariosIFP", Environment.NewLine, Environment.NewLine, Environment.NewLine, "Número Solicitud", JsonConvert.SerializeObject(numSolicitud));
                    if (lstGrupoFamiliar != null)
                    {
                        detalle = String.Format("Método: {0} {1} Parámetros: {2} - {3}: {4} {5} - {6}: {7} ", "CerrarBeneficiariosIFP", Environment.NewLine, Environment.NewLine, "Lista Grupo Familiar", JsonConvert.SerializeObject(lstGrupoFamiliar), Environment.NewLine, "Número Solicitud", JsonConvert.SerializeObject(numSolicitud));
                    }

                    servicioCotizador.RegistrarLog(new LogBD
                    {
                        IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                        NombreTerminal = nombreTerminal,
                        IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                        NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                        Detalle = detalle,
                        IdTipoEvento = Enums.EventoLog.RegistrarDatosBeneficiario.StringValue()
                    });


                    SolicitudIFP solicitudIFP = servicioCotizador.ObtenerDatosSolicitudIFP(numSolicitud);

                    log.Info("Accediendo a las Key necesarias: " + token + ", " + correo);
                    string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                    string urlFirmasDigitales = ConfigurationManager.AppSettings["url_firmas_digitales"].ToString();
                    string urlAppFirmasDigitales = ConfigurationManager.AppSettings["url_app_firmas_digitales"].ToString();
                    string urlAsesorInteligo = ConfigurationManager.AppSettings["url_asesor_inteligo"].ToString();

                    string flagProveedorCorreo = ConfigurationManager.AppSettings["flag_proveedor_correo"].ToString();
                    string flagCorreoCliente = ConfigurationManager.AppSettings["flag_correo_cliente"].ToString();
                    string flagCorreoAgente = ConfigurationManager.AppSettings["flag_correo_agente"].ToString();

                    string urlEnvioCorreo = string.Empty;
                    if (flagProveedorCorreo == "SME")
                    {
                        urlEnvioCorreo = ConfigurationManager.AppSettings["url_envio_correo_sme"].ToString();
                    }
                    if (flagProveedorCorreo == "AIZEN")
                    {
                        urlEnvioCorreo = ConfigurationManager.AppSettings["url_envio_correo"].ToString();
                    }

                    string remitente = ConfigurationManager.AppSettings["remitente_consentimiento"].ToString();
                    string destinatario = ConfigurationManager.AppSettings["destinatario_consentimiento"].ToString();
                    string asunto = ConfigurationManager.AppSettings["asunto_firmas_digitales"].ToString();

                    //comentar
                    if (destinatario != "N")
                    {
                        correo = destinatario;
                    }

                    string token_generado = string.Empty;
                    string tokenFirmaDigital = string.Empty;

                    //armando correo
                    TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                    string correoAgente = string.Empty;

                    log.Info("Obteniendo al agente");
                    List<Agente> listaAgentes = (List<Agente>)HttpContext.Current.Session["ListaAgentes"];
                    Agente agente = listaAgentes.Find(a => a.Id == HttpContext.Current.Session["Vendedor"].ToString());

                    log.Info("Obteniendo el cuerpo del correo");
                    var cuerpo = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~") + @"\\Plantilla\\RP\\SADP\\SADP.html");

                    log.Info("Consumiendo servicio token: " + urlToken);
                    /*Obtener token*/
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

                    correoAgente = string.Empty;

                    //if (flagCorreoAgente == "S")
                    //{
                    log.Info("Obteniendo el correo del agente");

                    if (HttpContext.Current.Session["RolAzman"].ToString() == Enums.RolAzman.AgenteExterno.StringValue())
                    {
                        try
                        {
                            urlAsesorInteligo = string.Format(urlAsesorInteligo, usuario);
                            log.Info("Consumiendo servicio consulta asesor inteligo: " + urlAsesorInteligo);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(urlAsesorInteligo);
                            httpWebRequest.Method = "GET";
                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                            log.Info("Leyendo el servicio");

                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                var jsonResult = streamReader.ReadToEnd();

                                if (jsonResult.Length > 0)
                                {
                                    JObject jObject = JObject.Parse(jsonResult);
                                    if (agente == null)
                                    {
                                        agente = new Agente() { Nombre = (string)jObject["gls_nombre_generico_asesor"], Id = (string)jObject["gls_codigo_asesor"] };
                                    }
                                    correoAgente = (string)jObject["gls_mail_asesor"];
                                }
                            }
                        }
                        catch (WebException e)
                        {
                            if (e.Status == WebExceptionStatus.ProtocolError)
                            {
                                throw new Exception("Error al obtener el asesor inteligo");
                            }
                        }

                    }
                    else
                    {
                        try
                        {
                            AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient servicioAzman = new AgenteServicios.Proxies.ModuloSeguridad.ServicioAzmanClient("epAzman");
                            var datosUsuario = servicioAzman.ObtenerDatosUsuarioSinClave(
                                    ConfigurationManager.AppSettings["AplicacionAZMAN"],
                                    ConfigurationManager.AppSettings["DominioRed"],
                                    agente.Usuario);

                            if (datosUsuario != null)
                            {
                                if (datosUsuario.Correo != null)
                                {
                                    if (datosUsuario.Correo != "")
                                    {
                                        correoAgente = datosUsuario.Correo;
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            log.Warn("No se ha enviado mail al agente " + agente.Usuario + " porque no se ha podido obtener su email");
                        }
                        //}
                    }

                    log.Info("Obteniendo el token: " + token);

                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    FirmaDigital firma = servicioCotizador.ObtenerFirmaDigital(numSolicitud, num_item, usuario);

                    if (firma != null)
                    {
                        token = firma.gls_token;
                    }

                    if (token.Length > 0)
                    {
                        Respuesta estadoCorreo = new Respuesta();

                        log.Info("Armando la url + token");
                        urlAppFirmasDigitales = string.Format(urlAppFirmasDigitales, token);
                        DocumentoSME documentoSME = null;

                        if (solicitudIFP.CodigoEstado != 5)
                        {
                            documentoSME = new DocumentoSME
                            {
                                Email = correo,
                                NumeroPoliza = "N/A",
                                NumeroDocumento = afiliado.Identificacion.Numero,
                                Destinatario = string.Format("{0} {1} {2}", afiliado.Nombre, afiliado.ApellidoMaterno, afiliado.ApellidoMaterno).Trim(),
                                ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEFirmaDigitalRP"]),
                                CamposDinamicos = new
                                {
                                    Id_nombres = ti.ToTitleCase(afiliado.Nombre.ToLower().Trim()),
                                    Id_link = urlAppFirmasDigitales,
                                    Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                                }
                            };
                        }
                        else
                        {
                            // Obtener datos del beneficiario
                            GrupoFamiliar beneficiario = solicitudIFP.Beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

                            // Obtener los datos de la versión solicitada y de la anterior para comparar
                            List<FormatoSolicitud> formatos = servicioCotizador.ListarFormatosSolicitud(solicitudIFP.Id, HttpContext.Current.Session["Usuario"].ToString());
                            int idFormatoPrevio = formatos.OrderByDescending(f => f.Correlativo).First().Id;

                            // Obtener versiones
                            FormatoSolicitud formatoActual = servicioCotizador.ObtenerFormatoSolicitudActualizado(solicitudIFP.Id, HttpContext.Current.Session["Usuario"].ToString());
                            FormatoSolicitud formatoPrevio = servicioCotizador.ObtenerFormatoSolicitud(solicitudIFP.Id, idFormatoPrevio, HttpContext.Current.Session["Usuario"].ToString());
                            List<FormatoSolicitudBeneficiario> formatoBeneficiariosActual = servicioCotizador.ListarFormatoSolicitudBeneficiarioActualizado(solicitudIFP.Id, HttpContext.Current.Session["Usuario"].ToString());
                            List<FormatoSolicitudBeneficiario> formatoBeneficiariosPrevio = servicioCotizador.ListarFormatoSolicitudBeneficiario(solicitudIFP.Id, idFormatoPrevio, HttpContext.Current.Session["Usuario"].ToString());
                            List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasActual = servicioCotizador.ListarFormatoSolicitudPersonaVinculadaActualizado(solicitudIFP.Id, HttpContext.Current.Session["Usuario"].ToString());
                            List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasPrevio = servicioCotizador.ListarFormatoSolicitudPersonaVinculada(solicitudIFP.Id, idFormatoPrevio, HttpContext.Current.Session["Usuario"].ToString());

                            // Comparar versiones
                            string tabla = Utilitarios.CrearTablaComparativa(formatoActual, formatoPrevio, formatoBeneficiariosActual, formatoBeneficiariosPrevio, formatoPersonasVinculadasActual, formatoPersonasVinculadasPrevio);

                            documentoSME = new DocumentoSME
                            {
                                Email = correo,
                                NumeroPoliza = "N/A",
                                NumeroDocumento = afiliado.Identificacion.Numero,
                                Destinatario = string.Format("{0} {1} {2}", afiliado.Nombre, afiliado.ApellidoMaterno, afiliado.ApellidoMaterno).Trim(),
                                ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEFirmaDigitalRPFlujoCorreccion"]),
                                CamposDinamicos = new
                                {
                                    Id_nombres = ti.ToTitleCase(afiliado.Nombre.ToLower().Trim()),
                                    Id_link = urlAppFirmasDigitales,
                                    Id_tabla = tabla
                                }
                            };
                        }

                        log.Info("Enviando el correo SME");
                        if (flagCorreoCliente == "S")
                        {
                            estadoCorreo = Utilitario.EnviarDocumentoSME(documentoSME);
                            if (estadoCorreo.Estado != Constante.COD_OK)
                            {
                                log.Error(string.Format("Error al enviar el correo del cliente [{0}]", correo));
                                throw new Exception("Error al enviar el correo del cliente.");
                            }

                            dynamic respDocumentoSME = JsonConvert.DeserializeObject(estadoCorreo.Mensaje);
                            long idSME = respDocumentoSME.codigoSME;

                            // Insertar en la tabla de seguimiento
                            EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                            {
                                gls_identificador = string.Format("{0}|{1}", num_item, numSolicitud),
                                id_proceso_envio = (int)Enums.ProcesoEnvio.FirmaDigitalRP,
                                id_sme = idSME,
                                cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                gls_mail = correo,
                                fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                                cod_agente = agente.Id,
                                aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                            };
                            string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                            var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                            string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                            log.Info("Consumiendo API de envío de documentos SME");
                            log.Debug(string.Format("Request Body[{0}]", jsonString));
                            using (var client = new WebClient())
                            {
                                client.Encoding = Encoding.UTF8;
                                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                                respuesta.Estado = Constante.COD_OK;
                            }
                        }

                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                        respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                        respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "Se envió documentos para solicitar firma digital al cliente." });
                    }
                    else
                    {

                        /*Obtener indicador cliente permitido*/
                        if (token_generado != "")
                        {
                            FirmaDigital firmaDigital = new FirmaDigital();
                            firmaDigital.num_solicitud = numSolicitud;
                            firmaDigital.num_item = num_item;
                            firmaDigital.ind_consentimiento = "N";
                            firmaDigital.aud_usr_ingreso = usuario;

                            var jsonFirmaCliente = JsonConvert.SerializeObject(firmaDigital);

                            log.Info("Json consentimiento: " + jsonFirmaCliente);

                            log.Info("Consumiendo servicio token: " + urlFirmasDigitales);
                            httpWebRequest = (HttpWebRequest)WebRequest.Create(urlFirmasDigitales);

                            var context = new HttpContextWrapper(HttpContext.Current);
                            HttpRequestBase request = context.Request;
                            httpWebRequest.UserAgent = request.UserAgent;

                            httpWebRequest.ContentType = "application/json";
                            httpWebRequest.Method = "POST";
                            httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                            log.Info("Pasando el json al servicio");
                            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                            {
                                streamWriter.Write(jsonFirmaCliente);
                                streamWriter.Flush();
                                streamWriter.Close();
                            }

                            log.Info("Leyendo el servicio");
                            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                            {
                                var jsonResult = streamReader.ReadToEnd();
                                JObject jObject = JObject.Parse(jsonResult);
                                tokenFirmaDigital = (string)jObject["gls_token"];
                            }

                            if (tokenFirmaDigital.Length > 0)
                            {
                                Respuesta estadoCorreo = new Respuesta();

                                log.Info("Armando la url + token");
                                urlAppFirmasDigitales = string.Format(urlAppFirmasDigitales, tokenFirmaDigital);

                                DocumentoSME documentoSME = null;

                                if (solicitudIFP.CodigoEstado != 5)
                                {
                                    documentoSME = new DocumentoSME
                                    {
                                        Email = correo,
                                        NumeroPoliza = "N/A",
                                        NumeroDocumento = afiliado.Identificacion.Numero,
                                        Destinatario = string.Format("{0} {1} {2}", afiliado.Nombre, afiliado.ApellidoMaterno, afiliado.ApellidoMaterno).Trim(),
                                        ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEFirmaDigitalRP"]),
                                        CamposDinamicos = new
                                        {
                                            Id_nombres = ti.ToTitleCase(afiliado.Nombre.ToLower().Trim()),
                                            Id_link = urlAppFirmasDigitales,
                                            Id_agente = ti.ToTitleCase(agente.Nombre.ToLower().Trim())
                                        }
                                    };
                                }
                                else
                                {
                                    // Obtener datos del beneficiario
                                    GrupoFamiliar beneficiario = solicitudIFP.Beneficiarios.Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

                                    // Obtener los datos de la versión solicitada y de la anterior para comparar
                                    List<FormatoSolicitud> formatos = servicioCotizador.ListarFormatosSolicitud(solicitudIFP.Id, HttpContext.Current.Session["Usuario"].ToString());
                                    int idFormatoPrevio = formatos.OrderByDescending(f => f.Correlativo).First().Id;

                                    // Obtener versiones
                                    FormatoSolicitud formatoActual = servicioCotizador.ObtenerFormatoSolicitudActualizado(solicitudIFP.Id, HttpContext.Current.Session["Usuario"].ToString());
                                    FormatoSolicitud formatoPrevio = servicioCotizador.ObtenerFormatoSolicitud(solicitudIFP.Id, idFormatoPrevio, HttpContext.Current.Session["Usuario"].ToString());
                                    List<FormatoSolicitudBeneficiario> formatoBeneficiariosActual = servicioCotizador.ListarFormatoSolicitudBeneficiarioActualizado(solicitudIFP.Id, HttpContext.Current.Session["Usuario"].ToString());
                                    List<FormatoSolicitudBeneficiario> formatoBeneficiariosPrevio = servicioCotizador.ListarFormatoSolicitudBeneficiario(solicitudIFP.Id, idFormatoPrevio, HttpContext.Current.Session["Usuario"].ToString());
                                    List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasActual = servicioCotizador.ListarFormatoSolicitudPersonaVinculadaActualizado(solicitudIFP.Id, HttpContext.Current.Session["Usuario"].ToString());
                                    List<FormatoSolicitudPersonaVinculada> formatoPersonasVinculadasPrevio = servicioCotizador.ListarFormatoSolicitudPersonaVinculada(solicitudIFP.Id, idFormatoPrevio, HttpContext.Current.Session["Usuario"].ToString());

                                    // Comparar versiones
                                    string tabla = Utilitarios.CrearTablaComparativa(formatoActual, formatoPrevio, formatoBeneficiariosActual, formatoBeneficiariosPrevio, formatoPersonasVinculadasActual, formatoPersonasVinculadasPrevio);

                                    documentoSME = new DocumentoSME
                                    {
                                        Email = correo,
                                        NumeroPoliza = "N/A",
                                        NumeroDocumento = afiliado.Identificacion.Numero,
                                        Destinatario = string.Format("{0} {1} {2}", afiliado.Nombre, afiliado.ApellidoMaterno, afiliado.ApellidoMaterno).Trim(),
                                        ProcesoSme = Convert.ToInt32(ConfigurationManager.AppSettings["SMEFirmaDigitalRPFlujoCorreccion"]),
                                        CamposDinamicos = new
                                        {
                                            Id_nombres = ti.ToTitleCase(afiliado.Nombre.ToLower().Trim()),
                                            Id_link = urlAppFirmasDigitales,
                                            Id_tabla = tabla
                                        }
                                    };
                                }

                                log.Info("Enviando el correo SME");
                                if (flagCorreoCliente == "S")
                                {
                                    estadoCorreo = Utilitario.EnviarDocumentoSME(documentoSME);
                                    if (estadoCorreo.Estado != Constante.COD_OK)
                                    {
                                        log.Error(string.Format("Error al enviar el correo del cliente [{0}]", correo));
                                        throw new Exception("Error al enviar el correo del cliente.");
                                    }

                                    dynamic respDocumentoSME = JsonConvert.DeserializeObject(estadoCorreo.Mensaje);
                                    long idSME = respDocumentoSME.codigoSME;

                                    // Insertar en la tabla de seguimiento
                                    EnvioSeguimiento envioSeguimiento = new EnvioSeguimiento
                                    {
                                        gls_identificador = string.Format("{0}|{1}", num_item, numSolicitud),
                                        id_proceso_envio = (int)Enums.ProcesoEnvio.FirmaDigitalRP,
                                        id_sme = idSME,
                                        cod_estado_trazabilidad = Enums.EstadoTrazabilidad.Enviado.StringValue(),
                                        gls_mail = correo,
                                        fec_envio = Convert.ToDateTime(DateTime.Now, new CultureInfo("es-PE")),
                                        cod_agente = agente.Id,
                                        aud_usr_ingreso = (string)HttpContext.Current.Session["usuario"]
                                    };
                                    string rutaEnvioSeguimiento = ConfigurationManager.AppSettings["url_envio_seguimiento"];
                                    var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                                    string jsonString = JsonSerializar.Serialize(envioSeguimiento);

                                    log.Info("Consumiendo API de envío de documentos SME");
                                    log.Debug(string.Format("Request Body[{0}]", jsonString));
                                    using (var client = new WebClient())
                                    {
                                        client.Encoding = Encoding.UTF8;
                                        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                                        respuesta.Mensaje = client.UploadString(new Uri(rutaEnvioSeguimiento), "POST", jsonString);
                                        respuesta.Estado = Constante.COD_OK;
                                    }
                                }

                                respuesta.Estado = Constante.COD_OK;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                                respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Solicitud para toma de firma digital enviada correctamente." });

                            }
                            else
                            {
                                log.Error("Hubo problemas al insertar la firma digital");
                                respuesta.Estado = Constante.COD_ERROR;
                                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>Hubo problemas al insertar la información de la solicitud de toma de fira digital</strong></div>";
                            }
                        }
                        else
                        {
                            log.Error("Hubo problemas con el Token");
                            respuesta.Estado = Constante.COD_ERROR;
                            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>Hubo problemas con el Token</strong></div>";
                        }
                    }

                    log.Debug("Fin metodo EnviarSADP (ResumenBeneficiariosPG)");

                }
                catch (Exception ex)
                {
                    log.Error("Error: " + Utilitarios.FormatearError(new List<String> { ex.Message }));
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

                return respuesta;
            }
        }

        /// <summary>
        /// Valida que el afiliado tenga una dirección principal y vigente con tipo de vía válido
        /// </summary>
        /// <param name="numCuspp">Número CUSPP del afiliado</param>
        /// <exception cref="Exception">Se lanza excepción cuando no se cumplan las validaciones</exception>
        private static void ValidarDireccionAfiliado(string numCuspp)
        {
            log.Debug("Validando dirección del afiliado");

            //Beneficiario afiliado = servicioCotizador.ListarBeneficiarios(numSolicitud, usuario).Find(b => b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue());

            //if (!string.IsNullOrEmpty(afiliado.numCuspp))
            //{
                //log.Debug("numCuspp afiliado: "+ afiliado.numCuspp);
                //string numCuspp = afiliado.numCuspp;
                List<Direccion> lstDirecciones = servicioCotizador.ListarDireccion(numCuspp);
                
                // Logear información de las direcciones encontradas
                //if (lstDirecciones != null)
                //{
                //    log.Debug($"Se encontraron {lstDirecciones.Count} direcciones para el CUSPP {numCuspp}");
                //    
                //    foreach (var dir in lstDirecciones)
                //    {
                //        try
                //        {
                //            log.Debug($"Dirección ID: {dir.Id}, Principal: {dir.Principal}, " +
                //                $"Vigente: {dir.Vigencia}, " +
                //                $"TipoVía: {(dir.TipoVia != null ? dir.TipoVia.Id : "null")}, " +
                //                $"Glosa: {dir.Glosa}");
                //        }
                //        catch (Exception ex)
                //        {
                //            log.Debug($"Error al mostrar detalles de dirección ID {dir.Id}: {ex.Message}");
                //        }
                //    }
                //}
                //else
                //{
                //    log.Debug($"No se encontraron direcciones para el CUSPP {numCuspp} (lstDirecciones es null)");
                //}
                //

                if (lstDirecciones == null || lstDirecciones.Count == 0)
                {
                    log.Error(string.Format("El Afiliado no cuenta con dirección, CUSPP [{0}].", numCuspp));
                    throw new Exception("El Afiliado no cuenta con dirección. Por favor registre la dirección del cliente.");
                }
                
                // Filtrar directamente por dirección principal y vigente
                // La propiedad Vigencia es un bool estándar
                //Direccion direccionPrincipal = lstDirecciones.Find(d => d.Principal && d.Vigencia);
                Direccion direccionPrincipal = lstDirecciones.Find(d => d.Principal);
                
                if (direccionPrincipal == null)
                {
                    log.Error(string.Format("El Afiliado no cuenta con dirección principal, CUSPP [{0}].", numCuspp));
                    throw new Exception("El Afiliado no cuenta con dirección principal. Por favor actualice la dirección del cliente.");
                }
                
                // Validar solo tipo de vía
                if (direccionPrincipal.TipoVia == null || 
                    string.IsNullOrEmpty(direccionPrincipal.TipoVia.Id) || 
                    direccionPrincipal.TipoVia.Id == "0")
                {
                    log.Error(string.Format("El Afiliado no cuenta con tipo vía válida, CUSPP [{0}].", numCuspp));
                    throw new Exception("El Afiliado no cuenta con tipo vía válida. Por favor actualice la dirección del cliente.");
                }
            //}
            
            log.Debug("Dirección del afiliado validada correctamente");
        }

        private static Respuesta EnviarNotificacion(string rutaServicio, Notificacion notificacion)
        {
            //Envio de manera Asincrono
            Respuesta respuesta = new Respuesta();
            try
            {
                List<string> errores = new List<string>();
                if (notificacion == null)
                {
                    errores.Add("Envíe una notificación completa. Dato Obligatorio");
                }
                else
                {
                    if (notificacion.p_remitente == null || notificacion.p_remitente == "")
                    {
                        errores.Add("Ingrese Remitente del Correo. Dato Obligatorio.");
                    }
                }

                if (rutaServicio == "")
                {
                    errores.Add("Ingrese ruta del servicio de correo. Dato Obligatorio");
                }

                if (errores.Count > 0)
                {
                    respuesta.Mensaje = Utilitarios.FormatearErrorTexto(errores);
                    return respuesta;
                }

                if (notificacion.p_destinatario != null)
                {
                    notificacion.p_destinatario = notificacion.p_destinatario.Trim();
                }

                log.Info("Ejecutando el servicio del correo");
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string jsonString = JsonSerializar.Serialize(notificacion);
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

        private static Respuesta EnviarNotificacionSME(string rutaServicio, NotificacionSME notificacionSME)
        {
            //Envio de manera Asincrono
            Respuesta respuesta = new Respuesta();
            try
            {
                List<string> errores = new List<string>();
                if (notificacionSME == null)
                {
                    errores.Add("Envíe una notificación completa. Dato Obligatorio");
                }
                else
                {
                    if (notificacionSME.De == null || notificacionSME.De == "")
                    {
                        errores.Add("Ingrese Remitente del Correo. Dato Obligatorio.");
                    }
                }

                if (rutaServicio == "")
                {
                    errores.Add("Ingrese ruta del servicio de correo. Dato Obligatorio");
                }

                if (errores.Count > 0)
                {
                    respuesta.Mensaje = Utilitarios.FormatearErrorTexto(errores);
                    return respuesta;
                }

                if (notificacionSME.Para != null)
                {
                    notificacionSME.Para = notificacionSME.Para.Trim();
                }

                log.Info("Ejecutando el servicio del correo");
                using (var client = new WebClient())
                {
                    client.Encoding = Encoding.UTF8;
                    var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                    string jsonString = JsonSerializar.Serialize(notificacionSME);
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

        [WebMethod]
        public static string CargarTablaBeneficiariosVitalicios(string tokenUsuario, string solicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    string html = "";

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();

                        var solicitudSerializado = JsonConvert.DeserializeObject<SolicitudIFP>(solicitud);

                        //List<GrupoFamiliar> grupoFamiliar = servicioCotizador.ListarGrupoFamiliar(solicitudSerializado.Afiliado.CUSPP);
                        //List<GrupoFamiliar> lstGrupoFamiliar = grupoFamiliar;

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        SolicitudIFP solicitudIFP = servicioCotizador.ObtenerDatosSolicitudIFP(solicitudSerializado.Id);

                        List<GrupoFamiliar> lstGrupoFamiliar = solicitudIFP.Beneficiarios.FindAll(ben => ben.IdTipoPeriodoBeneficiario.ToString() == Enums.TipoPeriodoBeneficiario.NoGarantizada.StringValue());

                        if (lstGrupoFamiliar == null)
                            lstGrupoFamiliar = new List<GrupoFamiliar>();

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                        List<Parametro> lstparametroParentesco = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Parentesco];
                        List<Parametro> lstparametroSexo = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo];

                        lstGrupoFamiliar.ForEach(b =>
                        {
                            b.Parentesco.Nombre = lstparametroParentesco.FindAll(x => x.Id == b.Parentesco.Id).FirstOrDefault().Glosa;
                        });

                        var control = (TablaBeneficiarioIFP_VIT)pagina.LoadControl("~/Controles/TablaBeneficiarioIFP_VIT.ascx");

                        control.lstBeneficiarios = lstGrupoFamiliar.FindAll(b => b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue());
                        control.lstParametroSexo = lstparametroSexo;

                        pagina.Controls.Add(control);

                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            html = sw.ToString();
                        }
                    }
                    return html;
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw (ex);
                }
            }
        }


    }
}