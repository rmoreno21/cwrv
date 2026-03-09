using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaLotesResultado : System.Web.UI.UserControl
    {
        public List<Lote> Lotes { get; set; }
        public bool PermisoConsultar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabLotesResultado.DataSource = Lotes;
                TabLotesResultado.DataBind();
                if (Lotes.Count > 0)
                {
                    TabLotesResultado.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }
    }
}