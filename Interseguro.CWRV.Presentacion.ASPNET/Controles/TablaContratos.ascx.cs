using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaContratos : System.Web.UI.UserControl
    {
        public List<Contrato> Contratos { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabContratos.DataSource = Contratos;
            TabContratos.DataBind();
        }
    }
}