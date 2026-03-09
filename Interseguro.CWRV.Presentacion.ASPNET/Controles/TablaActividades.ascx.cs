using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaActividades : System.Web.UI.UserControl
    {
        public List<Actividad> Actividades { get; set; }

        public bool PermisoConsultar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabActividades.DataSource = Actividades;
                TabActividades.DataBind();
                if (Actividades.Count > 0)
                {
                    TabActividades.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                if (Actividades.Count > 10)
                {
                    TabActividadesBotonera.Visible = true;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }
    }
}