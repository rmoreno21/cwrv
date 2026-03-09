using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET
{
    public partial class logs : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string servidor = Request.QueryString["servidor"];
            string nombre = Request.QueryString["nombre"];

            if (servidor != null && servidor != "")
            {
                if (nombre != null && nombre != "")
                {
                    mostrarLog(nombre);
                }
                else
                {
                    Response.Redirect("~/Cotizador/Cotizador.aspx", false);
                }
            }
            else
            {
                Response.Redirect("~/Cotizador/Cotizador.aspx", false);
            }

        }

        private void mostrarLog(string nombre)
        {
            try
            {
                string carpetaLog = AppDomain.CurrentDomain.BaseDirectory + @"Logs\";
                string archivoLog = carpetaLog + nombre;
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

                    Response.Clear();
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + nombre);
                    Response.ContentType = "application/octet-stream";
                    Response.WriteFile(cArchivoLog);
                    Response.Flush();
                    Response.Close();
                    //Response.End();

                    //System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    //string contenido = File.ReadAllText(cArchivoLog);
                    //string[] lineas = contenido.Split('\n');
                    //foreach (string linea in lineas)
                    //{
                    //    sb.Append(linea);
                    //}

                    //Response.Clear();
                    //System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + nombre);
                    //Response.Write(sb.ToString());
                    //Response.Flush();
                    ////Response.End();
                    //Response.Close();
                    
                }
                else
                {
                    Response.Redirect("~/Cotizador/Cotizador.aspx",false);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("~/Cotizador/Cotizador.aspx", false);
            }
        }

    }
}