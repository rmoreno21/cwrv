using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCapitalRequerido : System.Web.UI.UserControl
    {
        public List<Dominio.Entidades.CapitalRequerido> capitalRequerido { get; set; }
        public bool Consentimiento { get; set; }
        public bool PermisoModificar { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
            TabCapitalRequerido.DataSource = capitalRequerido;
            if (capitalRequerido != null)
            {
                TabCapitalRequerido.DataBind();
                if (capitalRequerido.Count > 0)
                {
                    TabCapitalRequerido.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
        }

        protected void TabCapitalRequerido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
            }
        }

    }
}