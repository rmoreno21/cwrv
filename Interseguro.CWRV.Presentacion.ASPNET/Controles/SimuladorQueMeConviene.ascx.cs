using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class SimuladorQueMeConviene : System.Web.UI.UserControl
    {
        public List<GraficoLineal> GraficoCIA { get; set; }
        public List<GraficoLineal> GraficoAFP { get; set; }

        public double MOD105Pension { get; set; }
        public double MOD110Pension { get; set; }
        public double MOD115Pension { get; set; }
        public double MOD120Pension { get; set; }
        public double MOD125Pension { get; set; }

        public double MOD205Pension { get; set; }
        public double MOD210Pension { get; set; }
        public double MOD215Pension { get; set; }
        public double MOD220Pension { get; set; }
        public double MOD225Pension { get; set; }

        public double MOD305Pension { get; set; }
        public double MOD310Pension { get; set; }
        public double MOD315Pension { get; set; }
        public double MOD320Pension { get; set; }
        public double MOD325Pension { get; set; }

        public double MOD105Acumulado { get; set; }
        public double MOD110Acumulado { get; set; }
        public double MOD115Acumulado { get; set; }
        public double MOD120Acumulado { get; set; }
        public double MOD125Acumulado { get; set; }

        public double MOD205Acumulado { get; set; }
        public double MOD210Acumulado { get; set; }
        public double MOD215Acumulado { get; set; }
        public double MOD220Acumulado { get; set; }
        public double MOD225Acumulado { get; set; }

        public double MOD305Acumulado { get; set; }
        public double MOD310Acumulado { get; set; }
        public double MOD315Acumulado { get; set; }
        public double MOD320Acumulado { get; set; }
        public double MOD325Acumulado { get; set; }


        public bool PermisoEjecutar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoEjecutar)
            {
                SinPermisos.Visible = false;
                SimuladorData.Visible = true;

                MOD105PXM.Text = String.Format("{0:0,0.00}", MOD105Pension);
                MOD205PXM.Text = String.Format("{0:0,0.00}", MOD205Pension);
                MOD305PXM.Text = String.Format("{0:0,0.00}", MOD305Pension);

                if (MOD105Pension > MOD205Pension && MOD105Pension > MOD305Pension) MOD105PXM.Text = String.Format("<strong>{0}</strong>", MOD105PXM.Text);
                else if (MOD205Pension > MOD105Pension && MOD205Pension > MOD305Pension) MOD205PXM.Text = String.Format("<strong>{0}</strong>", MOD205PXM.Text);
                else if (MOD305Pension > MOD105Pension && MOD305Pension > MOD205Pension) MOD305PXM.Text = String.Format("<strong>{0}</strong>", MOD305PXM.Text);

                MOD110PXM.Text = String.Format("{0:0,0.00}", MOD110Pension);
                MOD210PXM.Text = String.Format("{0:0,0.00}", MOD210Pension);
                MOD310PXM.Text = String.Format("{0:0,0.00}", MOD310Pension);
                if (MOD110Pension > MOD210Pension && MOD110Pension > MOD310Pension) MOD110PXM.Text = String.Format("<strong>{0}</strong>", MOD110PXM.Text);
                else if (MOD210Pension > MOD110Pension && MOD210Pension > MOD310Pension) MOD210PXM.Text = String.Format("<strong>{0}</strong>", MOD210PXM.Text);
                else if (MOD310Pension > MOD110Pension && MOD310Pension > MOD210Pension) MOD310PXM.Text = String.Format("<strong>{0}</strong>", MOD310PXM.Text);


                MOD115PXM.Text = String.Format("{0:0,0.00}", MOD115Pension);
                MOD215PXM.Text = String.Format("{0:0,0.00}", MOD215Pension);
                MOD315PXM.Text = String.Format("{0:0,0.00}", MOD315Pension);
                if (MOD115Pension > MOD215Pension && MOD115Pension > MOD315Pension) MOD115PXM.Text = String.Format("<strong>{0}</strong>", MOD115PXM.Text);
                else if (MOD215Pension > MOD115Pension && MOD215Pension > MOD315Pension) MOD215PXM.Text = String.Format("<strong>{0}</strong>", MOD215PXM.Text);
                else if (MOD315Pension > MOD115Pension && MOD315Pension > MOD215Pension) MOD315PXM.Text = String.Format("<strong>{0}</strong>", MOD315PXM.Text);

                MOD120PXM.Text = String.Format("{0:0,0.00}", MOD120Pension);
                MOD220PXM.Text = String.Format("{0:0,0.00}", MOD220Pension);
                MOD320PXM.Text = String.Format("{0:0,0.00}", MOD320Pension);
                if (MOD120Pension > MOD220Pension && MOD120Pension > MOD320Pension) MOD120PXM.Text = String.Format("<strong>{0}</strong>", MOD120PXM.Text);
                else if (MOD220Pension > MOD120Pension && MOD220Pension > MOD320Pension) MOD220PXM.Text = String.Format("<strong>{0}</strong>", MOD220PXM.Text);
                else if (MOD320Pension > MOD120Pension && MOD320Pension > MOD220Pension) MOD320PXM.Text = String.Format("<strong>{0}</strong>", MOD320PXM.Text);

                MOD125PXM.Text = String.Format("{0:0,0.00}", MOD125Pension);
                MOD225PXM.Text = String.Format("{0:0,0.00}", MOD225Pension);
                MOD325PXM.Text = String.Format("{0:0,0.00}", MOD325Pension);
                if (MOD125Pension > MOD225Pension && MOD125Pension > MOD325Pension) MOD125PXM.Text = String.Format("<strong>{0}</strong>", MOD125PXM.Text);
                else if (MOD225Pension > MOD125Pension && MOD225Pension > MOD325Pension) MOD225PXM.Text = String.Format("<strong>{0}</strong>", MOD225PXM.Text);
                else if (MOD325Pension > MOD125Pension && MOD325Pension > MOD225Pension) MOD325PXM.Text = String.Format("<strong>{0}</strong>", MOD325PXM.Text);

                MOD105ACU.Text = String.Format("{0:0,0.00}", MOD105Acumulado);
                MOD110ACU.Text = String.Format("{0:0,0.00}", MOD110Acumulado);
                MOD115ACU.Text = String.Format("{0:0,0.00}", MOD115Acumulado);
                MOD120ACU.Text = String.Format("{0:0,0.00}", MOD120Acumulado);
                MOD125ACU.Text = String.Format("{0:0,0.00}", MOD125Acumulado);

                MOD205ACU.Text = String.Format("{0:0,0.00}", MOD205Acumulado);
                MOD210ACU.Text = String.Format("{0:0,0.00}", MOD210Acumulado);
                MOD215ACU.Text = String.Format("{0:0,0.00}", MOD215Acumulado);
                MOD220ACU.Text = String.Format("{0:0,0.00}", MOD220Acumulado);
                MOD225ACU.Text = String.Format("{0:0,0.00}", MOD225Acumulado);

                MOD305ACU.Text = String.Format("{0:0,0.00}", MOD305Acumulado);
                MOD310ACU.Text = String.Format("{0:0,0.00}", MOD310Acumulado);
                MOD315ACU.Text = String.Format("{0:0,0.00}", MOD315Acumulado);
                MOD320ACU.Text = String.Format("{0:0,0.00}", MOD320Acumulado);
                MOD325ACU.Text = String.Format("{0:0,0.00}", MOD325Acumulado);
            }
            else
            {
                SinPermisos.Visible = true;
                SimuladorData.Visible = false;
            }
        }
    }
}