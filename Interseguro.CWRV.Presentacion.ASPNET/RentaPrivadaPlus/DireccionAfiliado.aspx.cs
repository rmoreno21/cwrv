using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using log4net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ServiceModel;
using System.Configuration;
using System.Reflection;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
{
    public partial class DireccionAfiliado : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Cotizador));
        private static IServicioCWRV servicioCotizador;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.MenuCotizador))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            CargarInformacionInicialPantalla();
                            LimpiarFormularios();

                            if (Session["CUSPP"] != null && Session["NroSolicitud"] == null)
                            {
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP"]));
                                ModDirModo.Value = Convert.ToString((Session["ModDirModo"]));
                                ModIdDireccion.Value = Convert.ToString((Session["idDireccion"]));

                            }
                            else if (Session["NroSolicitud"] != null && Session["CUSPP"] == null)
                            {
                                HCUSPP_RP.Value = Convert.ToString((Session["CUSPP_PLUS"]));
                                ModDirModo.Value = Convert.ToString((Session["ModDirModo"]));
                                ModIdDireccion.Value = Convert.ToString((Session["idDireccion"]));
                            }
                            else
                            {
                                Response.Redirect("Cotizador.aspx");
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

        private void LimpiarFormularios()
        {
            // Datos principales

            // Solicitudes
            ModDirDireccion.Text = "";
            ModDirDepartamento.SelectedIndex = 0;
            ModDirCiudad.SelectedIndex = 0;
            ModDirComuna.SelectedIndex = 0;
            ModDirPrincipal.SelectedIndex = 0;
        }

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();

            CargarComboboxNuevasEntidadesDatos(ModDirDepartamento, "Departamento", "");
            CargarComboboxNuevasEntidadesDatos(ModDirCiudad, "Provincia", "");
            CargarComboboxNuevasEntidadesDatos(ModDirComuna, "Distrito", "");

            CargarComboboxNuevosDatos(ModDomicilio, "TIPOVIA");

            ModDirPrincipal.Items.Add(new ListItem("«Seleccione»", "0"));
            ModDirPrincipal.Items.Add(new ListItem("Sí", "S"));
            ModDirPrincipal.Items.Add(new ListItem("No", "N"));
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

        private void CargarCombobox(DropDownList control, List<Ciudad> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Ciudad item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }

        private void CargarCombobox(DropDownList control, List<Comuna> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Comuna item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }

        private void CargarCombobox(DropDownList control, List<MontoCIC> combobox)
        {
            control.Items.Clear();
            foreach (MontoCIC item in combobox)
            {
                control.Items.Add(new ListItem(string.Format("{0:#,##0.00}", item.Valor), item.Valor.ToString()));
            }
        }

        private void CargarComboboxNuevosDatos(DropDownList control, string tabla)
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<Parametro> listaParametro = servicioCotizador.ObtenerParametros(tabla);

            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));

            foreach (Parametro item in listaParametro)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }

        private void CargarComboboxNuevasEntidadesDatos(DropDownList control, string tabla, string parametro)
        {

            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
            string usuario = HttpContext.Current.Session["Usuario"].ToString();

            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));

            if (tabla == "Departamento")
            {
                JArray listaDepartamentos = new JArray();

                if (Session["ListadoDepartamento"] == null)
                {
                    var urlDepartamentos = ConfigurationManager.AppSettings["url_lista_departamentos"].ToString();
                    urlDepartamentos = string.Format(urlDepartamentos, usuario);
                    listaDepartamentos = ObtenerUbigeo(urlToken, urlDepartamentos, usuario, listaDepartamentos);
                }
                else
                {
                    listaDepartamentos = (JArray)Session["ListadoDepartamento"];
                }

                foreach (var item in listaDepartamentos)
                {
                    control.Items.Add(new ListItem(item["gls_departamento"].ToString().ToUpper(), item["id_departamento"].ToString().ToLower()));
                }
            }
            else if (tabla == "Provincia")
            {
                JArray listaProvincias = new JArray();
                var urlProvincias = ConfigurationManager.AppSettings["url_lista_provincias"].ToString();
                urlProvincias = string.Format(urlProvincias, parametro, usuario);
                listaProvincias = ObtenerUbigeo(urlToken, urlProvincias, usuario, listaProvincias);

                foreach (var item in listaProvincias)
                {
                    control.Items.Add(new ListItem(item["gls_provincia"].ToString().ToUpper(), item["id_provincia"].ToString().ToLower()));
                }
            }
            else if (tabla == "Distrito")
            {
                JArray listaDistritos = new JArray();
                var urlDistritos = ConfigurationManager.AppSettings["url_lista_distritos"].ToString();
                urlDistritos = string.Format(urlDistritos, parametro, usuario);
                listaDistritos = ObtenerUbigeo(urlToken, urlDistritos, usuario, listaDistritos);

                foreach (var item in listaDistritos)
                {
                    control.Items.Add(new ListItem(item["gls_distrito"].ToString().ToUpper(), item["id_distrito"].ToString().ToLower()));
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
