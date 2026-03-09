using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

//<INIGTI_2145>
using Interseguro.CWRV.Infraestructura.General;
//<FINGTI_2145>
namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesSimulador : System.Web.UI.UserControl
    {
        public List<Solicitud> Solicitudes { get; set; }

        public bool PermisoConsultar { get; set; }

        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabSolicitudes.DataSource = Solicitudes;
                TabSolicitudes.DataBind();
                if (Solicitudes.Count > 0)
                {
                    TabSolicitudes.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }

        protected void TabSolicitudes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[0].Text = "<input type=\"radio\" name=\"solicitud\" value=\"" + e.Row.Cells[1].Text + "\" />";
            }
        }
    }
}