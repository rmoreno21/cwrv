using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class SimuladorTipoMoneda : System.Web.UI.UserControl
    {
        public double VAC15Acumulado { get; set; }
        public double VAC20Acumulado { get; set; }
        public double VAC25Acumulado { get; set; }
        public double SAJ15Acumulado { get; set; }
        public double SAJ20Acumulado { get; set; }
        public double SAJ25Acumulado { get; set; }
        public double DAJ15Acumulado { get; set; }
        public double DAJ20Acumulado { get; set; }
        public double DAJ25Acumulado { get; set; }
        
        public bool PermisoEjecutar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoEjecutar)
            {
                SinPermisos.Visible = false;
                SimuladorData.Visible = true;

                if (VAC15Acumulado > 0) VAC15.Text = String.Format("{0:0,0.00}", VAC15Acumulado); else VAC15.Text = "-";
                if (SAJ15Acumulado > 0) SAJ15.Text = String.Format("{0:0,0.00}", SAJ15Acumulado); else SAJ15.Text = "-";
                if (DAJ15Acumulado > 0) DAJ15.Text = String.Format("{0:0,0.00}", DAJ15Acumulado); else DAJ15.Text = "-";

                if (VAC20Acumulado > 0) VAC20.Text = String.Format("{0:0,0.00}", VAC20Acumulado); else VAC20.Text = "-";
                if (SAJ20Acumulado > 0) SAJ20.Text = String.Format("{0:0,0.00}", SAJ20Acumulado); else SAJ20.Text = "-";
                if (DAJ20Acumulado > 0) DAJ20.Text = String.Format("{0:0,0.00}", DAJ20Acumulado); else DAJ20.Text = "-";

                if (VAC25Acumulado > 0) VAC25.Text = String.Format("{0:0,0.00}", VAC25Acumulado); else VAC25.Text = "-";
                if (SAJ25Acumulado > 0) SAJ25.Text = String.Format("{0:0,0.00}", SAJ25Acumulado); else SAJ25.Text = "-";
                if (DAJ25Acumulado > 0) DAJ25.Text = String.Format("{0:0,0.00}", DAJ25Acumulado); else DAJ25.Text = "-";

                if (VAC15Acumulado > SAJ15Acumulado && VAC15Acumulado > DAJ15Acumulado) VAC15.Text = String.Format("<strong>{0}</strong>", VAC15.Text);
                else if (SAJ15Acumulado > VAC15Acumulado && SAJ15Acumulado > DAJ15Acumulado) SAJ15.Text = String.Format("<strong>{0}</strong>", SAJ15.Text);
                else if (DAJ15Acumulado > VAC15Acumulado && DAJ15Acumulado > SAJ15Acumulado) DAJ15.Text = String.Format("<strong>{0}</strong>", DAJ15.Text);

                if (VAC20Acumulado > SAJ20Acumulado && VAC20Acumulado > DAJ20Acumulado) VAC20.Text = String.Format("<strong>{0}</strong>", VAC20.Text);
                else if (SAJ20Acumulado > VAC20Acumulado && SAJ20Acumulado > DAJ20Acumulado) SAJ20.Text = String.Format("<strong>{0}</strong>", SAJ20.Text);
                else if (DAJ20Acumulado > VAC20Acumulado && DAJ20Acumulado > SAJ20Acumulado) DAJ20.Text = String.Format("<strong>{0}</strong>", DAJ20.Text);

                if (VAC25Acumulado > SAJ25Acumulado && VAC25Acumulado > DAJ25Acumulado) VAC25.Text = String.Format("<strong>{0}</strong>", VAC25.Text);
                else if (SAJ25Acumulado > VAC25Acumulado && SAJ25Acumulado > DAJ25Acumulado) SAJ25.Text = String.Format("<strong>{0}</strong>", SAJ25.Text);
                else if (DAJ25Acumulado > VAC25Acumulado && DAJ25Acumulado > SAJ25Acumulado) DAJ25.Text = String.Format("<strong>{0}</strong>", DAJ25.Text);
            }
            else
            {
                SinPermisos.Visible = true;
                SimuladorData.Visible = false;
            }
        }
    }
}