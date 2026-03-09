using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Text;
using log4net;
using System.IO;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Download;
using System.Configuration;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios
{
    public class CwrvGoogleDrive
    {
        public static ILog log = log4net.LogManager.GetLogger(typeof(CwrvGoogleDrive));
        public static string[] Scopes = { DriveService.Scope.Drive };
        public static string ApplicationName = ConfigurationManager.AppSettings["applicationName_drive"];
        //public static string IdCarpetaCompartida = "1rGrbwmYaR-6PTiwhqgZe5S1scu65joKF";

        public static DriveService GetService()
        {

            DriveService service = null;

            try
            {

                log.Debug("Inicio CwrvGoogleDrive.GetService");

                var keyFilePath = ConfigurationManager.AppSettings["keyFilePath_drive"];
                var serviceAccountEmail = ConfigurationManager.AppSettings["serviceAccountEmail_drive"];  // found https://console.developers.google.com

                var certificate = new X509Certificate2(keyFilePath, ConfigurationManager.AppSettings["pwd_drive"], X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);
                var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(serviceAccountEmail)
                {
                    Scopes = Scopes

                }.FromCertificate(certificate));

                service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,

                });

                log.Debug("Fin CwrvGoogleDrive.GetService");

                return service;
            }
            catch (Exception ex)
            {
                //log.Error(ex);
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                throw;
            }
            finally
            {
                //if (service != null) service.Dispose();
            }

        }

        public static List<Google.Apis.Drive.v3.Data.File> GetDriveFiles(string idCarpeta)
        {

            DriveService service = null;

            try
            {

                log.Debug("Inicio CwrvGoogleDrive.GetDriveFiles");

                service = GetService();

                // Define parameters of request.
                FilesResource.ListRequest FileListRequest = service.Files.List();

                //listRequest.PageSize = 10;
                //listRequest.PageToken = 10;
                FileListRequest.Fields = "nextPageToken, files(id, name, size, version, trashed, createdTime, webContentLink, fileExtension, fullFileExtension, owners, thumbnailLink, webViewLink)";
                FileListRequest.Q = "'" + idCarpeta + "' in parents ";

                // List files
                IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;

                log.Debug("Fin CwrvGoogleDrive.GetDriveFiles");

                return files.ToList();

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                throw;
            }
            finally
            {
                if (service != null) service.Dispose();
            }

        }

        public static void FileUpload(HttpPostedFile file, string folderId)
        {

            DriveService service = null;
            Stream stream = null;

            try
            {

                if (file != null && file.ContentLength > 0)
                {

                    log.Debug("Inicio CwrvGoogleDrive.FileUpload");

                    service = GetService();

                    //string path = Path.Combine(HttpContext.Current.Server.MapPath("~" + ConfigurationManager.AppSettings["RutaRepositorioTemporal"]),
                    //Path.GetFileName(file.FileName));

                    //string path = Path.Combine(HttpContext.Current.Server.MapPath("~/GoogleDriveFiles"),
                    //Path.GetFileName(file.FileName));
                    //file.SaveAs(path);

                    var FileMetaData = new Google.Apis.Drive.v3.Data.File();

                    string archivo_prop = file.FileName;

                    if (archivo_prop.Contains("%"))
                    {
                        archivo_prop = archivo_prop.Replace("%", string.Empty);
                    }

                    FileMetaData.Name = Path.GetFileName(archivo_prop);

                    FileMetaData.MimeType = file.ContentType;
                    //FileMetaData.MimeType = System.Web.MimeMapping.GetMimeMapping "image/jpeg";// System.Web.Mime MimeMapping.GetMimeMapping(path);
                    FileMetaData.Parents = new List<string>{
                    folderId
                    };

                    FilesResource.CreateMediaUpload request;
                    stream = file.InputStream;

                    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    request.Fields = "id";
                    request.Upload();

                    //using (var stream = new System.IO.FileStream(path, System.IO.FileMode.Open))
                    //{
                    //    request = service.Files.Create(FileMetaData, stream, FileMetaData.MimeType);
                    //    request.Fields = "id";
                    //    request.Upload();
                    //}

                    log.Debug("Fin CwrvGoogleDrive.FileUpload");

                }

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                throw;
            }
            finally
            {
                if (service != null) service.Dispose();
                if (stream != null) stream.Dispose();
            }

        }

        public static string DownloadGoogleFile(string fileId)
        {

            DriveService service = null;
            MemoryStream stream1 = null;

            try
            {

                log.Debug("Inicio CwrvGoogleDrive.DownloadGoogleFile");

                service = GetService();

                string FolderPath = System.Web.HttpContext.Current.Server.MapPath("/GoogleDriveFiles/");
                FilesResource.GetRequest request = service.Files.Get(fileId);

                string FileName = request.Execute().Name;
                string FilePath = System.IO.Path.Combine(FolderPath, FileName);

                stream1 = new MemoryStream();

                request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                            {
                                //Console.WriteLine(progress.BytesDownloaded);
                                break;
                            }
                        case DownloadStatus.Completed:
                            {
                                //Console.WriteLine("Download complete.");
                                SaveStream(stream1, FilePath);
                                break;
                            }
                        case DownloadStatus.Failed:
                            {
                                //Console.WriteLine("Download failed.");
                                break;
                            }
                    }
                };
                request.Download(stream1);

                log.Debug("Fin CwrvGoogleDrive.DownloadGoogleFile");

                return FilePath;

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                throw;
            }
            finally
            {
                if (service != null) service.Dispose();
                if (stream1 != null) stream1.Dispose();
            }

        }

        private static void SaveStream(MemoryStream stream, string FilePath)
        {

            try
            {

                log.Debug("Inicio CwrvGoogleDrive.SaveStream");

                using (System.IO.FileStream file = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    stream.WriteTo(file);
                }

                log.Debug("Fin CwrvGoogleDrive.SaveStream");

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                throw;
            }

        }

        public static void DeleteFile(Google.Apis.Drive.v3.Data.File files)
        {

            log.Debug("Inicio CwrvGoogleDrive.DeleteFile");

            DriveService service = null;

            try
            {

                service = GetService();

                // Initial validation.
                if (service == null)
                    throw new ArgumentNullException("service");

                if (files == null)
                    throw new ArgumentNullException(files.Id);

                // Make the request.
                service.Files.Delete(files.Id).Execute();

                log.Debug("Fin CwrvGoogleDrive.DeleteFile");

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                throw new Exception("Request Files.Delete failed.", ex);
            }
            finally
            {
                if (service != null) service.Dispose();
            }

        }

        public static string CreateFolder(string folder)
        {
            DriveService service = null;
            try
            {

                log.Debug("Inicio CwrvGoogleDrive.CreateFolder");

                service = GetService();

                //List<Parametro> listaParametro = (List<Parametro>)HttpContext.Current.Session["ParametroTabla"];
                //Parametro parametroGoogleDrive = listaParametro.Where(x => x.Id == "GOOGLEDRIVE").First();

                string IdCarpetaCompartida = ConfigurationManager.AppSettings["id_carpeta_drive"];
                log.Info(IdCarpetaCompartida);
                string path = Path.Combine(HttpContext.Current.Server.MapPath("~" + ConfigurationManager.AppSettings["RutaRepositorioTemporal"] + "//" + folder));
                log.Info(path);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                //Thread.Sleep(2000);

                FilesResource.ListRequest FileListRequest = service.Files.List();
                FileListRequest.Fields = "nextPageToken, files(id, name, size, version, trashed, createdTime, webContentLink, fileExtension, fullFileExtension, owners, thumbnailLink, webViewLink)";
                FileListRequest.Q = "'" + IdCarpetaCompartida + "' in parents and mimeType = 'application/vnd.google-apps.folder' and name = '" + folder + "'";

                IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
                log.Info("cantidad carpetas: " + files.Count);
                log.Info("carpeta: " + folder);
                if (files.Count == 0)
                {
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File()
                    {
                        Name = folder,
                        MimeType = "application/vnd.google-apps.folder",
                        Parents = new List<string>{
                    IdCarpetaCompartida
                    }
                    };

                    var request = service.Files.Create(fileMetadata);

                    request.Fields = "id";
                    var file = request.Execute();

                    FilesResource.ListRequest FileListRequestRespuesta = service.Files.List();
                    FileListRequestRespuesta.Fields = "nextPageToken, files(id, name, size, version, trashed, createdTime, webContentLink, fileExtension, fullFileExtension, owners, thumbnailLink, webViewLink)";
                    FileListRequestRespuesta.Q = "'" + IdCarpetaCompartida + "' in parents and mimeType = 'application/vnd.google-apps.folder' and name = '" + folder + "'";

                    IList<Google.Apis.Drive.v3.Data.File> filesRespuesta = null;
                    filesRespuesta = FileListRequest.Execute().Files;

                    while (filesRespuesta.Count == 0)
                    {
                        filesRespuesta = FileListRequest.Execute().Files;
                        log.Info("cantidad carpetas despues de crear: " + filesRespuesta.Count);
                    }

                    log.Debug("Fin CwrvGoogleDrive.CreateFolder");

                    return file.Id;
                }
                else
                {

                    log.Debug("Fin CwrvGoogleDrive.CreateFolder");

                    return files[0].Id;
                }

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                throw;
            }
            finally
            {
                if (service != null) service.Dispose();
            }
        }

        public static string DownloadGoogleFile(string path, string fileId)
        {

            DriveService service = null;
            MemoryStream stream1 = null;
            try
            {

                log.Debug("Inicio CwrvGoogleDrive.DownloadGoogleFile");

                service = GetService();

                //string FolderPath = System.Web.HttpContext.Current.Server.MapPath("/GoogleDriveFiles/");
                FilesResource.GetRequest request = service.Files.Get(fileId);

                string FileName = request.Execute().Name;
                string FilePath = System.IO.Path.Combine(path, FileName);

                stream1 = new MemoryStream();

                request.MediaDownloader.ProgressChanged += (Google.Apis.Download.IDownloadProgress progress) =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Downloading:
                            {
                                //Console.WriteLine(progress.BytesDownloaded);
                                break;
                            }
                        case DownloadStatus.Completed:
                            {
                                //Console.WriteLine("Download complete.");
                                SaveStream(stream1, FilePath);
                                break;
                            }
                        case DownloadStatus.Failed:
                            {
                                //Console.WriteLine("Download failed.");
                                break;
                            }
                    }
                };
                request.Download(stream1);

                log.Debug("fin CwrvGoogleDrive.DownloadGoogleFile");

                return FilePath;

            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                throw;
            }
            finally
            {
                if (service != null) service.Dispose();
                if (stream1 != null) stream1.Dispose();
            }

        }

        public static void DeleteOldFiles(string fecha)
        {
            DriveService service = null;
            string carpetaEliminada = "";
            int carpetasEliminadas = 0;
            try
            {
                log.Debug("Inicio CwrvGoogleDrive.DeleteOldFiles");

                service = GetService();

                string IdCarpetaCompartida = ConfigurationManager.AppSettings["id_carpeta_drive"];
                log.Info(IdCarpetaCompartida);

                FilesResource.ListRequest FileListRequest = service.Files.List();

                FileListRequest.PageSize = 1000;
                FileListRequest.Fields = "nextPageToken, files(id, name, size, version, trashed, createdTime, webContentLink, fileExtension, fullFileExtension, owners, thumbnailLink, webViewLink)";
                FileListRequest.Q = "'" + IdCarpetaCompartida + "' in parents and mimeType = 'application/vnd.google-apps.folder' and createdTime <= '" + fecha + "T23:59:59'";

                IList<Google.Apis.Drive.v3.Data.File> files = FileListRequest.Execute().Files;
                log.Info("Cantidad de carpetas: " + files.Count);

                if (files.Count != 0)
                {
                    foreach (var file in files)
                    {
                        carpetaEliminada = file.Name;
                        DeleteFile(file);
                        carpetasEliminadas++;
                    }
                }

                log.Info("cantidad carpetas: " + files.Count);
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                log.Error("Se intentó eliminar la carpeta: " + carpetaEliminada);
                throw;
            }
            finally
            {
                log.Info("Cantidad de carpetas eliminadas: " + carpetasEliminadas);
                if (service != null) service.Dispose();
            }
        }
    }
}