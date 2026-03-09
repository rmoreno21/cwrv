using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaPersonas : UserControl
    {
        public List<Afiliado> Afiliados { get; set; }
        public string Origen { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabPersonas.DataSource = Afiliados;
            TabPersonas.DataBind();
            if (Afiliados.Count > 0)
            {
                TabPersonas.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
    }
}