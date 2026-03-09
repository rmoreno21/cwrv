using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaGestionVentas : System.Web.UI.UserControl
    {
        public List<GestionVentas> lstGestionVentas { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            TabGestionVentas.DataSource = lstGestionVentas;
            TabGestionVentas.DataBind();
            if (lstGestionVentas.Count > 0)
            {
                TabGestionVentas.HeaderRow.TableSection = TableRowSection.TableHeader;
                ModSolTotalRegistro.Text = lstGestionVentas.Count.ToString();
                ModSolTotalCIC.Text = lstGestionVentas.Sum(x => x.CIC).ToString("#,##0.00");
            }
            else
            {
                //divScrooll.Style.Clear();
                LabModSolLineaTotales.Visible = false;
            }
        }

        protected void TabGestionVentas_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}