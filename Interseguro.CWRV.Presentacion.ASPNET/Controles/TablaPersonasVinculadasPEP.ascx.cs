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
    public partial class TablaPersonasVinculadasPEP : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> PersonasPEP { get; set; }

        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabPEP.DataSource = PersonasPEP;
            TabPEP.DataBind();

            if (PersonasPEP.Count > 0)
            {
                TabPEP.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }

        protected void TabGrupoFamiliar_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                
            }
        }
    }
}