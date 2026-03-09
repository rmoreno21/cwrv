using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaTraDefault : System.Web.UI.UserControl
    {
        public List<ParametroEspecial> parametroEspecial  { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            TabTraDefault.DataSource = parametroEspecial;
            TabTraDefault.DataBind();
            if (parametroEspecial.Count > 0)
            {
                TabTraDefault.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void TabTraDefault_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}