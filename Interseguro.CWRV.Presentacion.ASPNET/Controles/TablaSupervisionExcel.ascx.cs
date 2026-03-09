using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSupervisionExcel : System.Web.UI.UserControl
    {
        public List<Supervision> DatosReporte { get; set; }

        public int ColumnaOrdenar { get; set; }
        public char DireccionOrdenar { get; set; }

        public bool PermisoConsultar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabSeguimiento.DataSource = DatosReporte;
                TabSeguimiento.DataBind();
            }
        }
    }
}