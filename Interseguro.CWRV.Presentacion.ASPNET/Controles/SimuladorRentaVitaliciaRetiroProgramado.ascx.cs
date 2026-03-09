using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class SimuladorRentaVitaliciaRetiroProgramado : System.Web.UI.UserControl
    {
        public List<GraficoLineal> GraficoCIA { get; set; }
        public List<GraficoLineal> GraficoAFP { get; set; }

        public double RV05Pension { get; set; }
        public double RV10Pension { get; set; }
        public double RV15Pension { get; set; }
        public double RV20Pension { get; set; }
        public double RP05Pension { get; set; }
        public double RP10Pension { get; set; }
        public double RP15Pension { get; set; }
        public double RP20Pension { get; set; }
        public double RV05Acumulado { get; set; }
        public double RV10Acumulado { get; set; }
        public double RV15Acumulado { get; set; }
        public double RV20Acumulado { get; set; }
        public double RP05Acumulado { get; set; }
        public double RP10Acumulado { get; set; }
        public double RP15Acumulado { get; set; }
        public double RP20Acumulado { get; set; }
        
        public bool PermisoEjecutar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoEjecutar)
            {
                SinPermisos.Visible = false;
                SimuladorData.Visible = true;

                RV05PXM.Text = String.Format("{0:0,0.00}", RV05Pension);
                RP05PXM.Text = String.Format("{0:0,0.00}", RP05Pension);
                if (RV05Pension > RP05Pension) RV05PXM.Text = String.Format("<strong>{0}</strong>", RV05PXM.Text);
                else if (RP05Pension > RV05Pension) RP05PXM.Text = String.Format("<strong>{0}</strong>", RP05PXM.Text);

                RV10PXM.Text = String.Format("{0:0,0.00}", RV10Pension);
                RP10PXM.Text = String.Format("{0:0,0.00}", RP10Pension);
                if (RV10Pension > RP10Pension) RV10PXM.Text = String.Format("<strong>{0}</strong>", RV10PXM.Text);
                else if (RP10Pension > RV10Pension) RP10PXM.Text = String.Format("<strong>{0}</strong>", RP10PXM.Text);

                RV15PXM.Text = String.Format("{0:0,0.00}", RV15Pension);
                RP15PXM.Text = String.Format("{0:0,0.00}", RP15Pension);
                if (RV15Pension > RP15Pension) RV15PXM.Text = String.Format("<strong>{0}</strong>", RV15PXM.Text);
                else if (RP15Pension > RV15Pension) RP15PXM.Text = String.Format("<strong>{0}</strong>", RP15PXM.Text);

                RV20PXM.Text = String.Format("{0:0,0.00}", RV20Pension);
                RP20PXM.Text = String.Format("{0:0,0.00}", RP20Pension);
                if (RV20Pension > RP20Pension) RV20PXM.Text = String.Format("<strong>{0}</strong>", RV20PXM.Text);
                else if (RP20Pension > RV20Pension) RP20PXM.Text = String.Format("<strong>{0}</strong>", RP20PXM.Text);

                RV05ACU.Text = String.Format("{0:0,0.00}", RV05Acumulado);
                RV10ACU.Text = String.Format("{0:0,0.00}", RV10Acumulado);
                RV15ACU.Text = String.Format("{0:0,0.00}", RV15Acumulado);
                RV20ACU.Text = String.Format("{0:0,0.00}", RV20Acumulado);
                RP05ACU.Text = String.Format("{0:0,0.00}", RP05Acumulado);
                RP10ACU.Text = String.Format("{0:0,0.00}", RP10Acumulado);
                RP15ACU.Text = String.Format("{0:0,0.00}", RP15Acumulado);
                RP20ACU.Text = String.Format("{0:0,0.00}", RP20Acumulado);
            }
            else
            {
                SinPermisos.Visible = true;
                SimuladorData.Visible = false;
            }
        }
    }
}