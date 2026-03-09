using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesRentaPrivada : System.Web.UI.UserControl
    {
        public List<SolicitudRP> Solicitudes { get; set; }

        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoCorreoElectronico { get; set; }
        public bool PermisoExportarPDF { get; set; }
        //<SRIINI06326>
        public bool PermisoReporteEscenario { get; set; }
        public bool Consentimiento { get; set; }
        //public bool RedLocal { get; set; }
        private int AnchoColumnaIconos;
        //<SRIFIN06326>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                AnchoColumnaIconos = 64;
                if (Consentimiento) AnchoColumnaIconos += 32;
                //if (RedLocal) AnchoColumnaIconos += 32;

                TabSolicitudes_RP.DataSource = Solicitudes;
                TabSolicitudes_RP.DataBind();
                if (Solicitudes.Count > 0)
                {
                    TabSolicitudes_RP.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }

        protected void TabSolicitudes_RP_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //<SRI.INI-20322>
                //e.Row.Cells[8].Width = AnchoColumnaIconos;
                e.Row.Cells[7].Width = AnchoColumnaIconos;
                //<SRI.FIN-20322>
            }
        }

    }
}