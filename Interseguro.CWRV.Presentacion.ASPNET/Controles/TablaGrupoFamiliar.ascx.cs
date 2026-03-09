using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaGrupoFamiliar : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> Grupos { get; set; }

        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool Consentimiento { get; set; }
        public bool ListaNegra { get; set; }
        public bool PermisoEliminar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            HListaNegra.Value = "N";
            HListaNegra.Value = (ListaNegra) ? "S" : "N";

            if (PermisoConsultar)
            {
                TabGrupoFamiliar.DataSource = Grupos;
                TabGrupoFamiliar.DataBind();
                if (Grupos.Count > 0)
                {
                    TabGrupoFamiliar.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                if (!Consentimiento)
                {
                    TabGrupoFamiliar.Columns[1].Visible = false;
                    TabGrupoFamiliar.Columns[2].Visible = false;
                    TabGrupoFamiliar.Columns[3].Visible = false;
                }

            }
            else
            {
                SinPermisos.Visible = true;
            }
        }

        protected void TabGrupoFamiliar_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!Consentimiento)
                {
                    if (e.Row.Cells[6].Text.Length > 3)
                    {
                        e.Row.Cells[6].Text = e.Row.Cells[6].Text.Substring(3, e.Row.Cells[6].Text.Length - 3);
                    }
                }
            }
        }
    }
}