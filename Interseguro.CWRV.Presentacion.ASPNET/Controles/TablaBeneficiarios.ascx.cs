using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaBeneficiarios : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> Beneficiarios { get; set; }
        public bool Consentimiento { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabBeneficiarios.DataSource = Beneficiarios;
            TabBeneficiarios.DataBind();
            if (Beneficiarios.Count > 0)
            {
                TabBeneficiarios.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            if (!Consentimiento)
            {
                TabBeneficiarios.Columns[3].Visible = false;
            }
        }

        protected void TabBeneficiarios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GrupoFamiliar beneficiario = (GrupoFamiliar)e.Row.DataItem;
                e.Row.Cells[1].Text = "<input type=\"checkbox\" name=\"Correlativo\" "
                    + "value=\"" + beneficiario.Id + "\""
                    + ((beneficiario.Parentesco.Id != "80") ? String.Empty : " disabled=\"disabled\"")
                    + ((beneficiario.Seleccionado) ? " checked=\"checked\"" : String.Empty)
                    + " />";

                if (!Consentimiento)
                {
                    if (e.Row.Cells[5].Text.Length > 3)
                    {
                        e.Row.Cells[5].Text = e.Row.Cells[5].Text.Substring(3, e.Row.Cells[5].Text.Length - 3);
                    }
                }
            }
        }
    }
}