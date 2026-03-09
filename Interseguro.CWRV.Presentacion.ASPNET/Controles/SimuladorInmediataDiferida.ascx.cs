using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class SimuladorInmediataDiferida : System.Web.UI.UserControl
    {
        public double RVI15Acumulado { get; set; }
        public double RVI20Acumulado { get; set; }
        public double RVI25Acumulado { get; set; }
        public double RD115Acumulado { get; set; }
        public double RD120Acumulado { get; set; }
        public double RD125Acumulado { get; set; }
        public double RD215Acumulado { get; set; }
        public double RD220Acumulado { get; set; }
        public double RD225Acumulado { get; set; }
        //<SRI.INI-20322>
        public double RD315Acumulado { get; set; }
        public double RD320Acumulado { get; set; }
        public double RD325Acumulado { get; set; }
        public double RD415Acumulado { get; set; }
        public double RD420Acumulado { get; set; }
        public double RD425Acumulado { get; set; }
        public double RD515Acumulado { get; set; }
        public double RD520Acumulado { get; set; }
        public double RD525Acumulado { get; set; }
        //<SRI.FIN-20322>
        
        public bool PermisoEjecutar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoEjecutar)
            {
                SinPermisos.Visible = false;
                SimuladorData.Visible = true;

                if (RVI15Acumulado > 0) RVI15.Text = String.Format("{0:0,0.00}", RVI15Acumulado); else RVI15.Text = "-";
                if (RD115Acumulado > 0) RD115.Text = String.Format("{0:0,0.00}", RD115Acumulado); else RD115.Text = "-";
                if (RD215Acumulado > 0) RD215.Text = String.Format("{0:0,0.00}", RD215Acumulado); else RD215.Text = "-";
                //<SRI.INI-20322>
                if (RD315Acumulado > 0) RD315.Text = String.Format("{0:0,0.00}", RD315Acumulado); else RD315.Text = "-";
                if (RD415Acumulado > 0) RD415.Text = String.Format("{0:0,0.00}", RD415Acumulado); else RD415.Text = "-";
                if (RD515Acumulado > 0) RD515.Text = String.Format("{0:0,0.00}", RD515Acumulado); else RD515.Text = "-";
                //<SRI.FIN-20322>

                if (RVI20Acumulado > 0) RVI20.Text = String.Format("{0:0,0.00}", RVI20Acumulado); else RVI20.Text = "-";
                if (RD120Acumulado > 0) RD120.Text = String.Format("{0:0,0.00}", RD120Acumulado); else RD120.Text = "-";
                if (RD220Acumulado > 0) RD220.Text = String.Format("{0:0,0.00}", RD220Acumulado); else RD220.Text = "-";
                //<SRI.INI-20322>
                if (RD320Acumulado > 0) RD320.Text = String.Format("{0:0,0.00}", RD320Acumulado); else RD320.Text = "-";
                if (RD420Acumulado > 0) RD420.Text = String.Format("{0:0,0.00}", RD420Acumulado); else RD420.Text = "-";
                if (RD520Acumulado > 0) RD520.Text = String.Format("{0:0,0.00}", RD520Acumulado); else RD520.Text = "-";
                //<SRI.FIN-20322>

                if (RVI25Acumulado > 0) RVI25.Text = String.Format("{0:0,0.00}", RVI25Acumulado); else RVI25.Text = "-";
                if (RD125Acumulado > 0) RD125.Text = String.Format("{0:0,0.00}", RD125Acumulado); else RD125.Text = "-";
                if (RD225Acumulado > 0) RD225.Text = String.Format("{0:0,0.00}", RD225Acumulado); else RD225.Text = "-";
                //<SRI.INI-20322>
                if (RD325Acumulado > 0) RD325.Text = String.Format("{0:0,0.00}", RD325Acumulado); else RD325.Text = "-";
                if (RD425Acumulado > 0) RD425.Text = String.Format("{0:0,0.00}", RD425Acumulado); else RD425.Text = "-";
                if (RD525Acumulado > 0) RD525.Text = String.Format("{0:0,0.00}", RD525Acumulado); else RD525.Text = "-";
                //<SRI.FIN-20322>
                
                //<SRI.INI-20322>
                //if (RVI15Acumulado > RD115Acumulado && RVI15Acumulado > RD215Acumulado) RVI15.Text = String.Format("<strong>{0}</strong>", RVI15.Text);
                //else if (RD115Acumulado > RVI15Acumulado && RD115Acumulado > RD215Acumulado) RD115.Text = String.Format("<strong>{0}</strong>", RD115.Text);
                //else if (RD215Acumulado > RVI15Acumulado && RD215Acumulado > RD115Acumulado) RD215.Text = String.Format("<strong>{0}</strong>", RD215.Text);
                if (RVI15Acumulado > RD115Acumulado && RVI15Acumulado > RD215Acumulado && RVI15Acumulado > RD315Acumulado && RVI15Acumulado > RD415Acumulado && RVI15Acumulado > RD515Acumulado) RVI15.Text = String.Format("<strong>{0}</strong>", RVI15.Text);
                else if (RD115Acumulado > RVI15Acumulado && RD115Acumulado > RD215Acumulado && RD115Acumulado > RD315Acumulado && RD115Acumulado > RD415Acumulado && RD115Acumulado > RD515Acumulado) RD115.Text = String.Format("<strong>{0}</strong>", RD115.Text);
                else if (RD215Acumulado > RVI15Acumulado && RD215Acumulado > RD115Acumulado && RD215Acumulado > RD315Acumulado && RD215Acumulado > RD415Acumulado && RD215Acumulado > RD515Acumulado) RD215.Text = String.Format("<strong>{0}</strong>", RD215.Text);
                else if (RD315Acumulado > RVI15Acumulado && RD315Acumulado > RD115Acumulado && RD315Acumulado > RD215Acumulado && RD315Acumulado > RD415Acumulado && RD315Acumulado > RD515Acumulado) RD315.Text = String.Format("<strong>{0}</strong>", RD315.Text);
                else if (RD415Acumulado > RVI15Acumulado && RD415Acumulado > RD115Acumulado && RD415Acumulado > RD215Acumulado && RD415Acumulado > RD315Acumulado && RD415Acumulado > RD515Acumulado) RD415.Text = String.Format("<strong>{0}</strong>", RD415.Text);
                else if (RD515Acumulado > RVI15Acumulado && RD515Acumulado > RD115Acumulado && RD515Acumulado > RD215Acumulado && RD515Acumulado > RD315Acumulado && RD515Acumulado > RD415Acumulado) RD515.Text = String.Format("<strong>{0}</strong>", RD515.Text);
                //<SRI.FIN-20322>

                //<SRI.INI-20322>
                //if (RVI20Acumulado > RD120Acumulado && RVI20Acumulado > RD220Acumulado) RVI20.Text = String.Format("<strong>{0}</strong>", RVI20.Text);
                //else if (RD120Acumulado > RVI20Acumulado && RD120Acumulado > RD220Acumulado) RD120.Text = String.Format("<strong>{0}</strong>", RD120.Text);
                //else if (RD220Acumulado > RVI20Acumulado && RD220Acumulado > RD120Acumulado) RD220.Text = String.Format("<strong>{0}</strong>", RD220.Text);
                if (RVI20Acumulado > RD120Acumulado && RVI20Acumulado > RD220Acumulado && RVI20Acumulado > RD320Acumulado && RVI20Acumulado > RD420Acumulado && RVI20Acumulado > RD520Acumulado) RVI20.Text = String.Format("<strong>{0}</strong>", RVI20.Text);
                else if (RD120Acumulado > RVI20Acumulado && RD120Acumulado > RD220Acumulado && RD120Acumulado > RD320Acumulado && RD120Acumulado > RD420Acumulado && RD120Acumulado > RD520Acumulado) RD120.Text = String.Format("<strong>{0}</strong>", RD120.Text);
                else if (RD220Acumulado > RVI20Acumulado && RD220Acumulado > RD120Acumulado && RD220Acumulado > RD320Acumulado && RD220Acumulado > RD420Acumulado && RD220Acumulado > RD520Acumulado) RD220.Text = String.Format("<strong>{0}</strong>", RD220.Text);
                else if (RD320Acumulado > RVI20Acumulado && RD320Acumulado > RD120Acumulado && RD320Acumulado > RD220Acumulado && RD320Acumulado > RD420Acumulado && RD320Acumulado > RD520Acumulado) RD320.Text = String.Format("<strong>{0}</strong>", RD320.Text);
                else if (RD420Acumulado > RVI20Acumulado && RD420Acumulado > RD120Acumulado && RD420Acumulado > RD220Acumulado && RD420Acumulado > RD320Acumulado && RD420Acumulado > RD520Acumulado) RD420.Text = String.Format("<strong>{0}</strong>", RD420.Text);
                else if (RD520Acumulado > RVI20Acumulado && RD520Acumulado > RD120Acumulado && RD520Acumulado > RD220Acumulado && RD520Acumulado > RD320Acumulado && RD520Acumulado > RD420Acumulado) RD520.Text = String.Format("<strong>{0}</strong>", RD520.Text);
                //<SRI.FIN-20322>

                //<SRI.INI-20322>
                //if (RVI25Acumulado > RD125Acumulado && RVI25Acumulado > RD225Acumulado) RVI25.Text = String.Format("<strong>{0}</strong>", RVI25.Text);
                //else if (RD125Acumulado > RVI25Acumulado && RD125Acumulado > RD225Acumulado) RD125.Text = String.Format("<strong>{0}</strong>", RD125.Text);
                //else if (RD225Acumulado > RVI25Acumulado && RD225Acumulado > RD125Acumulado) RD225.Text = String.Format("<strong>{0}</strong>", RD225.Text);
                if (RVI25Acumulado > RD125Acumulado && RVI25Acumulado > RD225Acumulado && RVI25Acumulado > RD325Acumulado && RVI25Acumulado > RD425Acumulado && RVI25Acumulado > RD525Acumulado) RVI25.Text = String.Format("<strong>{0}</strong>", RVI25.Text);
                else if (RD125Acumulado > RVI25Acumulado && RD125Acumulado > RD225Acumulado && RD125Acumulado > RD325Acumulado && RD125Acumulado > RD425Acumulado && RD125Acumulado > RD525Acumulado) RD125.Text = String.Format("<strong>{0}</strong>", RD125.Text);
                else if (RD225Acumulado > RVI25Acumulado && RD225Acumulado > RD125Acumulado && RD225Acumulado > RD325Acumulado && RD225Acumulado > RD425Acumulado && RD225Acumulado > RD525Acumulado) RD225.Text = String.Format("<strong>{0}</strong>", RD225.Text);
                else if (RD325Acumulado > RVI25Acumulado && RD325Acumulado > RD125Acumulado && RD325Acumulado > RD225Acumulado && RD325Acumulado > RD425Acumulado && RD325Acumulado > RD525Acumulado) RD325.Text = String.Format("<strong>{0}</strong>", RD325.Text);
                else if (RD425Acumulado > RVI25Acumulado && RD425Acumulado > RD125Acumulado && RD425Acumulado > RD225Acumulado && RD425Acumulado > RD325Acumulado && RD425Acumulado > RD525Acumulado) RD425.Text = String.Format("<strong>{0}</strong>", RD425.Text);
                else if (RD525Acumulado > RVI25Acumulado && RD525Acumulado > RD125Acumulado && RD525Acumulado > RD225Acumulado && RD525Acumulado > RD325Acumulado && RD525Acumulado > RD425Acumulado) RD525.Text = String.Format("<strong>{0}</strong>", RD525.Text);
                //<SRI.FIN-20322>
            }
            else
            {
                SinPermisos.Visible = true;
                SimuladorData.Visible = false;
            }
        }
    }
}