using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesRentaPrivadaPlusCierre : System.Web.UI.UserControl
    {
        public List<SolicitudRPPlus> Solicitudes { get; set; }
        public bool PermisoConsultar { get; set; }
        private int AnchoColumnaIconos;
        public bool PermisoCerrar { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                AnchoColumnaIconos = 33;
                //if (PermisoConsultar) AnchoColumnaIconos += 33;
                if (PermisoCerrar) AnchoColumnaIconos += 33;

               
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
                e.Row.Cells[7].Width = AnchoColumnaIconos;
            }
        }
    }
}