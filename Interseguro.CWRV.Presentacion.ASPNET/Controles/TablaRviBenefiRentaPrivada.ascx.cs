using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaRviBenefiRentaPrivada : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> Beneficiarios { get; set; }
        public bool Consentimiento { get; set; }
        public bool PermisoModificar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabRviBenefi_RP.DataSource = Beneficiarios;
            TabRviBenefi_RP.DataBind();
            if (Beneficiarios.Count > 0)
            {
                TabRviBenefi_RP.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            if (!Consentimiento)
            {
                TabRviBenefi_RP.Columns[2].Visible = false;
            }
        }

        protected void TabRviBenefi_RP_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!Consentimiento)
                {
                    if (e.Row.Cells[4].Text.Length > 3)
                    {
                        e.Row.Cells[4].Text = e.Row.Cells[4].Text.Substring(3, e.Row.Cells[4].Text.Length - 3);
                    }
                }
            }
        }
    }
}