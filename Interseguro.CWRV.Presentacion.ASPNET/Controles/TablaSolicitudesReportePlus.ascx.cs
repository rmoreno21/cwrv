using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesReportePlus : System.Web.UI.UserControl
    {
        public List<SolicitudRPPlus> Solicitudes { get; set; }

        public List<CotizacionRPPlus> Cotizaciones { get; set; }
        
        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoCorreoElectronico { get; set; }
        public bool PermisoExportarPDF { get; set; }
        public bool PermisoReporteEscenario { get; set; }
        public bool Consentimiento { get; set; }
        public bool RedLocal { get; set; }
        private int AnchoColumnaIconos;

        public string rol { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                rol = ((string)Session["RolAzman"]);

                AnchoColumnaIconos = 66;
                if (Consentimiento) AnchoColumnaIconos += 32;
                if (RedLocal) AnchoColumnaIconos += 32;

                
                TabSolicitudesReportePlus.DataSource =  Solicitudes; //SolicitudFecha;
                TabSolicitudesReportePlus.DataBind();
                if (Solicitudes.Count > 0)
                {
                    TabSolicitudesReportePlus.HeaderRow.TableSection = TableRowSection.TableHeader;
                    HRegistros.Value = Solicitudes.Count.ToString();
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }

        protected void TabCotizacionesReportePlus_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void TabSolicitudesReportePlus_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                Cotizaciones = new List<CotizacionRPPlus>();

                string numSolicitud = TabSolicitudesReportePlus.DataKeys[e.Row.RowIndex].Value.ToString();


                Cotizaciones = Solicitudes.Find(x => x.Id == numSolicitud).Cotizaciones;
                GridView TabCotizaciones= (GridView)e.Row.FindControl("TabCotizacionesReportePlus");

                TabCotizaciones.DataSource = Cotizaciones;
                TabCotizaciones.DataBind();

                if (Solicitudes.Count > 0)
                {
                    TabCotizaciones.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
        }

    }
}