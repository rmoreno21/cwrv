using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCotizacionesMovimiento : System.Web.UI.UserControl
    {

        public List<CotizacionMovimiento> cotizacionMovimiento { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            TabCotizacionesMovimiento.DataSource = cotizacionMovimiento;
            TabCotizacionesMovimiento.DataBind();
            if (cotizacionMovimiento.Count > 0)
            {
                TabCotizacionesMovimiento.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            
        }

    }
}