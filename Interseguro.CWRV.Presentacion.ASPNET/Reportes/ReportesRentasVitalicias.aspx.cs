using Interseguro.CWRV.Infraestructura.General;
using log4net;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;
using System.Web.UI;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class ReportesRentasVitalicias : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReportesRentasVitalicias));

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string solicitud = Request.QueryString["s"];
                string formato = Request.QueryString["f"];
                string tokenVCTP = Request.QueryString["t"];
                string urlBaseApi = ConfigurationManager.AppSettings["url_base_api_cwrv"].ToString();
                string usuario = Session["Usuario"].ToString();
                string tokenApi = string.Empty;

                string endpoint = string.Format("{0}/token", urlBaseApi);
                log.Info(string.Format("Se va a obtener el token. url[{0}]", endpoint));
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(endpoint);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string requestBody = JsonConvert.SerializeObject(new
                    {
                        usuario
                    });

                    streamWriter.Write(requestBody);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    string responseBody = streamReader.ReadToEnd();
                    tokenApi = ((dynamic)JsonConvert.DeserializeObject(responseBody)).accessToken;
                    log.Debug(string.Format("Token [{0}]", tokenApi));
                }

                if (tokenApi != "")
                {
                    endpoint = string.Empty;
                    string nombreArchivo = string.Empty;
                    switch (formato)
                    {
                        case "1":
                            // VCTP
                            endpoint = string.Format("{0}/firmas-digitales/formato-consentimiento/{1}/{2}", urlBaseApi, tokenVCTP, usuario);  ConfigurationManager.AppSettings["url_formato_solicitud_rpp"].ToString();
                            nombreArchivo = string.Format("FormatoVCTP_{0}.pdf", solicitud);
                            break;
                    }
                    endpoint = string.Format(endpoint, solicitud, usuario);

                    byte[] formatoBinario = Utilitarios.ConsumirServicio(endpoint, usuario, tokenApi);

                    Response.Clear();
                    MemoryStream ms = new MemoryStream(formatoBinario);
                    Response.ContentType = "application/pdf";
                    Response.AddHeader("content-disposition", string.Format("attachment;filename={0}", nombreArchivo));
                    Response.Buffer = true;
                    ms.WriteTo(Response.OutputStream);
                    Response.End();
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception ex)
            {
                log.Error("Se ha producido un error en ReportesRentasVitalicias", ex);
            }
        }
    }
}