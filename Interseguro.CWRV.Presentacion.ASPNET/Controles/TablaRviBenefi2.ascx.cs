using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaRviBenefi2 : System.Web.UI.UserControl
    {
        public List<Beneficiario> Beneficiarios { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabRviBenefi.DataSource = Beneficiarios;
            TabRviBenefi.DataBind();
            if (Beneficiarios.Count > 0)
            {
                TabRviBenefi.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
    }
}