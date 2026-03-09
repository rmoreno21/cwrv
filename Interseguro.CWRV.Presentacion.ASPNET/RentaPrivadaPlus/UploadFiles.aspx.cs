using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloSeguridad;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
{
    public partial class UploadFiles : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UploadFiles));
        private static IServicioCWRV servicioCotizador;
        private static IServicioAzman servicioAzman;

        protected void Page_Load(object sender, EventArgs e)
        {

            log.Debug("Inicio UploadFiles.Page_Load");

            Respuesta respuesta = new Respuesta();

            if (Request.ContentLength < 2147483648)
            {
                if (Request.Files.Count > 0)
                {
                    string path = Server.MapPath("~" + ConfigurationManager.AppSettings["RutaRepositorioTemporal"]);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    /*Creando Carpeta en google*/
                    string numSolicitud = Session["idSolicitud"].ToString();
                    string idCarpeta = CwrvGoogleDrive.CreateFolder(numSolicitud);

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        System.Web.HttpPostedFile archivo = Request.Files[i];
                        CwrvGoogleDrive.FileUpload(archivo, idCarpeta);
                    }
                }
                respuesta.Estado = Constante.COD_OK;
                //respuesta.Mensaje = "Eduardo";
                //Response.Write("eduardo"); 
                //Response.Flush();
            }
            //<SOLFINSRI25424>
            else
            {
                respuesta.Estado = Constante.COD_ERROR;
                Response.Write(respuesta);
                Response.Flush();
            }

            log.Debug("Fin UploadFiles.Page_Load");

        }

        [WebMethod]
        public static List<Google.Apis.Drive.v3.Data.File> ObtenerArchivos(string tokenUsuario, string num_solicitud)
        {

            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio UploadFiles.ObtenerArchivos WebMethod");

                    log.Info(tokenUsuario);
                    log.Info(num_solicitud);
                    Respuesta respuesta = new Respuesta();
                    List<Google.Apis.Drive.v3.Data.File> lstArchivo = new List<Google.Apis.Drive.v3.Data.File>();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        string path = HttpContext.Current.Server.MapPath("~" + ConfigurationManager.AppSettings["RutaRepositorioTemporal"]);
                        log.Info(path);
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        path = path + "\\" + num_solicitud;
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        log.Info(path);
                        /*Creando Carpeta en google*/
                        string idCarpeta = CwrvGoogleDrive.CreateFolder(num_solicitud);
                        log.Info(idCarpeta);
                        lstArchivo = CwrvGoogleDrive.GetDriveFiles(idCarpeta);
                        log.Info(lstArchivo.Count);
                        string RutaAbsoluta = HttpContext.Current.Request.Url.AbsoluteUri.ToString();
                        int posicionRutaDomino = HttpContext.Current.Request.Url.AbsoluteUri.Count() - HttpContext.Current.Request.Url.AbsolutePath.Count();
                        string RutaDomino = RutaAbsoluta.Substring(0, posicionRutaDomino);
                        
                        string aplicacion = HttpContext.Current.Request.ApplicationPath;

                        if (aplicacion == "/")
                        {
                            aplicacion = "";
                        }

                        //7012-21
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        string archivos_Existentes = servicioCotizador.ArchivosExistentes(num_solicitud);
                        HttpContext.Current.Session["FilesIntegracionPlaft"] = archivos_Existentes;
                        //7012-21

                        string rutaCarpeta = ConfigurationManager.AppSettings["RutaRepositorioTemporal"];
                        path = string.Format("{0}{1}/{2}/{3}", RutaDomino, aplicacion, rutaCarpeta.Substring(1,rutaCarpeta.Length - 1), num_solicitud);
                        log.Info(path);
                        lstArchivo.ForEach(p =>
                        {
                            p.WebContentLink = path + "/" + p.Name;

                            //7012-21
                            if (!HttpContext.Current.Session["FilesIntegracionPlaft"].ToString().Contains(p.Id))
                            {
                                Google.Apis.Drive.v3.Data.File.CapabilitiesData gadfCapacidad = new Google.Apis.Drive.v3.Data.File.CapabilitiesData();
                                gadfCapacidad.CanDelete = true;
                                p.Capabilities = gadfCapacidad;
                            }
                            else {
                                Google.Apis.Drive.v3.Data.File.CapabilitiesData gadfCapacidad = new Google.Apis.Drive.v3.Data.File.CapabilitiesData();
                                gadfCapacidad.CanDelete = false;
                                p.Capabilities = gadfCapacidad;
                            }
                            //7012-21

                        });

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin UploadFiles.ObtenerArchivos WebMethod");

                    return lstArchivo;
                }
                catch (Exception ex)
                {
                    //log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                    //    ex.Source, ex.Message, ex.StackTrace));
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);

                    log.Debug("Fin UploadFiles.ObtenerArchivos WebMethod");

                    return null;
                }
            }
        }
        
        [WebMethod]
        public static Respuesta DescargaArchivo(string tokenUsuario, string idArchivo, string num_solicitud)
        {

            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio UploadFiles.DescargaArchivo WebMethod");

                    Respuesta respuesta = new Respuesta();
                    //List<Google.Apis.Drive.v3.Data.File> lstArchivo = new List<Google.Apis.Drive.v3.Data.File>();
                    
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        string path = HttpContext.Current.Server.MapPath("~" + ConfigurationManager.AppSettings["RutaRepositorioTemporal"]);
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        path = path + "\\" + num_solicitud;

                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);

                        CwrvGoogleDrive.DownloadGoogleFile(path, idArchivo);

                        respuesta.Estado = Constante.COD_OK;

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin UploadFiles.DescargaArchivo WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    //log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                    //    ex.Source, ex.Message, ex.StackTrace));
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);

                    log.Debug("Fin UploadFiles.DescargaArchivo WebMethod");

                    return null;
                }
            }
        }

        [WebMethod]
        public static Respuesta EliminarArchivo(string tokenUsuario, string idArchivo)
        {

            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio UploadFiles.EliminarArchivo WebMethod");

                    Respuesta respuesta = new Respuesta();
                    //List<Google.Apis.Drive.v3.Data.File> lstArchivo = new List<Google.Apis.Drive.v3.Data.File>();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        Google.Apis.Drive.v3.Data.File file = new Google.Apis.Drive.v3.Data.File();

                        file.Id = idArchivo;
                        
                        CwrvGoogleDrive.DeleteFile(file);

                        respuesta.Estado = Constante.COD_OK;

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin UploadFiles.EliminarArchivo WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    //log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                    //    ex.Source, ex.Message, ex.StackTrace));
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);

                    log.Debug("Fin UploadFiles.EliminarArchivo WebMethod");

                    return null;
                }
            }
        }

        [WebMethod]
        public static Respuesta CargarExtensiones(string tokenUsuario)
        {

            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio UploadFiles.CargarExtensiones WebMethod");

                    Respuesta respuesta = new Respuesta();
                    //List<Google.Apis.Drive.v3.Data.File> lstArchivo = new List<Google.Apis.Drive.v3.Data.File>();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        string tipoArhivo = ConfigurationManager.AppSettings["TipoArchivo"];
                        
                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Mensaje = tipoArhivo;

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin UploadFiles.CargarExtensiones WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    //log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                    //    ex.Source, ex.Message, ex.StackTrace));
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);

                    log.Debug("Fin UploadFiles.CargarExtensiones WebMethod");

                    return null;
                }
            }
        }

        [WebMethod]
        public static Respuesta CargarMegas(string tokenUsuario)
        {

            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio UploadFiles.CargarMegas WebMethod");

                    Respuesta respuesta = new Respuesta();
                   
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        string megasPermitidos = ConfigurationManager.AppSettings["MegasPermitido"];

                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Mensaje = megasPermitidos;

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    log.Debug("Fin UploadFiles.CargarMegas WebMethod");

                    return respuesta;
                }
                catch (Exception ex)
                {
                    //log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                    //    ex.Source, ex.Message, ex.StackTrace));
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);

                    log.Debug("Fin UploadFiles.CargarMegas WebMethod");

                    return null;
                }
            }
        }


    }
}