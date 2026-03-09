using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using log4net;
using System;
using System.IO;
using System.Text;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
{
    public partial class ProcesarTareas : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcesarTareas));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            string proceso = Request.QueryString["proceso"];

            if (proceso != null && proceso != "")
            {
                switch (proceso)
                {
                    case "help":
                        contenido.InnerHtml = "Hola mundo!";
                        break;
                    case "limpiarDrive":
                        limpiarDrive();
                        break;
                    case "mostrarLog":
                        mostrarLog();
                        break;
                }
            }
            else
            {
                Response.Redirect("~/Cotizador/Cotizador.aspx");
            }
        }
        private void limpiarDrive()
        {
            try
            {
                string fechaProcesar = Request.QueryString["fecha"];
                if (fechaProcesar != null && fechaProcesar != "")
                {
                    string anio = fechaProcesar.Substring(0, 4);
                    string mes = fechaProcesar.Substring(4, 2);
                    string dia = fechaProcesar.Substring(6, 2);
                    string fecha = anio + "-" + mes + "-" + dia;
                    DateTime tmp;
                    if (DateTime.TryParse(fecha, out tmp))
                    {
                        CwrvGoogleDrive.DeleteOldFiles(fecha);
                    }
                }
                Response.Redirect("~/Cotizador/Cotizador.aspx");
            }
            catch (Exception ex)
            {
                contenido.InnerHtml = ex.Message;
            }

        }
        private void mostrarLog()
        {
            try
            {
                string origen = Request.QueryString["origen"];
                string nombreArchivo = Request.QueryString["nombre"];

                if (nombreArchivo != null && nombreArchivo != "" && origen != null && origen != "")
                {
                    switch (origen)
                    {
                        case "f":
                            logFront(nombreArchivo);
                            break;
                        case "b":
                            logBack(nombreArchivo);
                            break;
                    }
                }
                else
                {
                    Response.Redirect("~/Cotizador/Cotizador.aspx");
                }
            }
            catch (Exception ex)
            {
                contenido.InnerHtml = ex.Message;
            }
        }
        private void logFront(string nombreArchivo)
        {
            string carpetaLog = AppDomain.CurrentDomain.BaseDirectory + @"Logs\";
            string archivoLog = carpetaLog + nombreArchivo;
            string cArchivoLog = carpetaLog + "logTemporal.txt";

            if (File.Exists(archivoLog))
            {
                //ELIMINA COPIA TEMPORAL DE LOG
                if (File.Exists(cArchivoLog))
                {
                    File.Delete(cArchivoLog);
                }
                //CREA COPIA TEMPORAL DE LOG
                File.Copy(archivoLog, cArchivoLog);

                /*MOSTRAR DESDE FRONT*/
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + nombreArchivo);
                Response.ContentType = "application/octet-stream";
                Response.WriteFile(cArchivoLog);
            }
            else
            {
                Response.Redirect("~/Cotizador/Cotizador.aspx");
            }
        }
        private void logBack(string nombreArchivo)
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            string contenido = servicioCotizador.archivoLog(nombreArchivo);

            if (contenido != "")
            {
                /*MOSTRAR DESDE BACK*/
                UTF8Encoding encoding = new UTF8Encoding();
                byte[] fs = encoding.GetBytes(contenido);

                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=" + nombreArchivo);
                Response.ContentType = "application/octet-stream";
                Response.BinaryWrite(fs);
            }
            else
            {
                Response.Redirect("~/Cotizador/Cotizador.aspx");
            }
        }
    }
}