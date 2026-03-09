using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaDirecciones : System.Web.UI.UserControl
    {
        public List<Direccion> Direcciones { get; set; }

        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoEliminar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabDirecciones.DataSource = Direcciones;
                TabDirecciones.DataBind();
                if (Direcciones.Count > 0)
                {
                    TabDirecciones.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                if (Direcciones.Count > 1)
                {
                    TabDireccionesBotonera.Visible = true;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }
    }
}