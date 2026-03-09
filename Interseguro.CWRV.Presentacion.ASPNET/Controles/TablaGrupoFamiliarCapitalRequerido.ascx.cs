using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaGrupoFamiliarCapitalRequerido : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> Beneficiarios { get; set; }
        public bool Consentimiento { get; set; }
        public bool PermisoModificar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabrupoFamiliarCapitalRequerido.DataSource = Beneficiarios;
            if (Beneficiarios != null)
            {
                TabrupoFamiliarCapitalRequerido.DataBind();
                if (Beneficiarios.Count > 0)
                {
                    TabrupoFamiliarCapitalRequerido.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                if (!Consentimiento)
                {
                    TabrupoFamiliarCapitalRequerido.Columns[3].Visible = false;
                }
            }
        }

        protected void TablaGrupoFamiliarCapitalRequerido_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
                GrupoFamiliar beneficiario = (GrupoFamiliar)e.Row.DataItem;
                e.Row.Cells[1].Text = "<input type=\"checkbox\" name=\"Correlativo\" "
                    + "value=\"" + beneficiario.Id + "\""
                    + ((beneficiario.Parentesco.Id != "80") ? String.Empty : " disabled=\"disabled\"")
                    //<SOLINI25621>
                    + " checked=\"checked\""
                    //< SOLFIN25621 >
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