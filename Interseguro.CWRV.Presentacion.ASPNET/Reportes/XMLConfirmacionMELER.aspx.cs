using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class XMLConfirmacionMELER : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(XMLConfirmacionMELER));

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    string nombreArchivo = String.Format("CargaConfirmaciones_{0}.xml", DateTime.Now.ToString("yyyyMMdd"));
                    Response.ContentType = "text/xml";
                    Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", nombreArchivo));
                    Response.Clear();
                    Response.Write(Session["XMLCargaConfirmaciones"]);
                    Response.End();
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}: {1}]", ex.Source, ex.Message), ex);
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]", ex.Source, ex.Message, ex.StackTrace), ex);
                }
            }
        }
    }
}