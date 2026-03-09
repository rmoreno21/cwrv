using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaTelefonos : System.Web.UI.UserControl
    {
        public List<Telefono> Telefonos { get; set; }

        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoEliminar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabTelefonos.DataSource = Telefonos;
                TabTelefonos.DataBind();
                if (Telefonos.Count > 0)
                {
                    TabTelefonos.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                if (Telefonos.Count > 1)
                {
                    TabTelefonosBotonera.Visible = true;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }
    }
}