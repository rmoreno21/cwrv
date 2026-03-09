using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCotizacionesMeler : System.Web.UI.UserControl
    {
        public List<Solicitud> Solicitudes { get; set; }
        public bool PermisoConsultar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabCotizacionesMeler.DataSource = Solicitudes;
                TabCotizacionesMeler.DataBind();
                if (Solicitudes.Count > 0)
                {
                    TabCotizacionesMeler.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }
    }
}