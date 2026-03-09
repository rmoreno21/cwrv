using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaAcomMaximo : System.Web.UI.UserControl
    {
        public List<RolAcom> acomns { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            TabSeleccionAcom.DataSource = acomns;
            TabSeleccionAcom.DataBind();
            if (acomns.Count > 0)
            {
                TabSeleccionAcom.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

        }
    }
}