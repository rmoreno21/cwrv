using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesConfirmacion : System.Web.UI.UserControl
    {
        public List<Solicitud> Solicitudes { get; set; }
        public bool PermisoConsultar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabSolicitudesConfirmacion.DataSource = Solicitudes;
                TabSolicitudesConfirmacion.DataBind();
                if (Solicitudes.Count > 0)
                {
                    TabSolicitudesConfirmacion.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }
    }
}