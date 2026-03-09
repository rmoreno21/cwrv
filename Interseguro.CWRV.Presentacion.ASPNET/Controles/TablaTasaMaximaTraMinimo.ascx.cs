using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaTasaMaximaTraMinimo : System.Web.UI.UserControl
    {
        public List<ParametroEspecial> parametroEspecial { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            TabTasaMaximaTraMinima.DataSource = parametroEspecial;
            TabTasaMaximaTraMinima.DataBind();
            if (parametroEspecial.Count > 0)
            {
                TabTasaMaximaTraMinima.HeaderRow.TableSection = TableRowSection.TableHeader;
                var json = new JavaScriptSerializer().Serialize(parametroEspecial);
                HCotizaValPar.Value = json;
                //HCotizaValPar.Value = parametroEspecial.ToString();
            }
        }

        protected void TabTasaMaximaTraMinima_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                TextBox TabCotDifTRA;
                TabCotDifTRA = (TextBox)(fila.FindControl("TabCotDifTRA"));
                TabCotDifTRA.Text = parametroEspecial[e.Row.RowIndex].ValDiferencia.ToString(); //Cotizaciones[e.Row.RowIndex].AjusteTRA.ToString();

            }

        }

    }
}