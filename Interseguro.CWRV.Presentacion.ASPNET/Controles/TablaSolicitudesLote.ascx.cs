using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesLote : System.Web.UI.UserControl
    {
        public List<Solicitud> Solicitudes { get; set; }
        public bool PermisoConsultar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabSolicitudesLote.DataSource = Solicitudes;
                TabSolicitudesLote.DataBind();
                if (Solicitudes.Count > 0)
                {
                    TabSolicitudesLote.HeaderRow.TableSection = TableRowSection.TableHeader;
                    FechaCierre.Value = Convert.ToDateTime(Solicitudes[0].FechaPlazoAFP).ToString("dd/MM/yyyy");
                    FechaPresentacion.Text = Convert.ToDateTime(Solicitudes[0].FechaPlazoAFP).ToString("dd/MM/yyyy");
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }
    }
}