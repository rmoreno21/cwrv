using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCotizacionesCierreRVI : System.Web.UI.UserControl
    {
        public List<Cotizacion> Cotizaciones { get; set; }
        public bool PermisoConsultar { get; set; }
        public bool SoloLectura { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabCotizacionesCierreRVI.DataSource = Cotizaciones;
            TabCotizacionesCierreRVI.DataBind();
            if (Cotizaciones.Count > 0)
            {
                TabCotizacionesCierreRVI.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
    }
}