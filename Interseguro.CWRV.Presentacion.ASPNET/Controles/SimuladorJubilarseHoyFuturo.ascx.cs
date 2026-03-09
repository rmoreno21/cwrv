using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.VisualBasic;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class SimuladorJubilarseHoyFuturo : System.Web.UI.UserControl
    {
        public int Id { get; set; }
        public string Moneda { get; set; }
        public int EdadActual { get; set; }
        public int EdadJubilarse { get; set; }
        public double SaldoCIC1 { get; set; }
        public double SaldoCIC2 { get; set; }
        public double Pension1 { get; set; }
        public double Pension2 { get; set; }
        public double TasaAjuste { get; set; }

        public bool PermisoEjecutar { get; set; }

        //<SRI.INI-20322>
        public double PensionAnual1 { get; set; }
        public double PensionAnual2 { get; set; }
        public double PensionTotal1 { get; set; }
        public double PensionTotal2 { get; set; }
        //<SRI.FIN-20322>

        //<SRI.INI-20322_E2>
        public Int32 AniosJub { get; set; }
        //<SRI.FIN-20322_E2>
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoEjecutar)
            {
                SinPermisos.Visible = false;
                SimuladorData.Visible = true;

                GlsMoneda.Text = Moneda;

                // Años
                AnhosJub.Text = (EdadJubilarse - EdadActual).ToString();
                //<SRI.INI-20322_E2>
                AniosJub = (Int32)(EdadJubilarse - EdadActual);
                //<SRI.FIN-20322_E2>
                
                //<SRI.INI-20322>
                //AnhosMax.Text = (Convert.ToInt32(ConfigurationManager.AppSettings["EdadMaxJubilacion"]) - EdadActual).ToString();
                //<SRI.FIN-20322>

                // Edad
                EdadHoy.Text = EdadActual.ToString();
                EdadJub.Text = EdadJubilarse.ToString();
                //<SRI.INI-20322>
                //EdadMax.Text = ConfigurationManager.AppSettings["EdadMaxJubilacion"];
                //<SRI.FIN-20322>

                // Saldo CIC
                SaldoCICHoy.Text = String.Format("{0:0,0.00}", Math.Round(SaldoCIC1, 2));
                SaldoCICJub.Text = String.Format("{0:0,0.00}", Math.Round(SaldoCIC2, 2));

                // Pensión mensual
                PensionMensualHoy.Text = String.Format("{0:0,0.00}", Math.Round(Pension1, 2));
                PensionMensualJub.Text = String.Format("{0:0,0.00}", Math.Round(Pension2, 2));
                //<SRI.INI-20322>
                //PensionMensualDif.Text = String.Format("{0:0,0.00}", Math.Round(Pension2, 2) - Math.Round(Pension1, 2));
                //<SRI.FIN-20322>

                // Pensión anual
                double pensionAnualHoy = (Math.Round(Pension1, 2) * 12) * (1 + (TasaAjuste/100));
                double pensionAnualJub = (Math.Round(Pension2, 2) * 12) * (1 + (TasaAjuste/100));
                PensionAnualHoy.Text = String.Format("{0:0,0.00}", Math.Round(pensionAnualHoy, 2));
                PensionAnualJub.Text = String.Format("{0:0,0.00}", Math.Round(pensionAnualJub, 2));
                //<SRI.INI-20322>
                //PensionAnualDif.Text = String.Format("{0:0,0.00}", Math.Round(pensionAnualJub, 2) - Math.Round(pensionAnualHoy, 2));
                //<SRI.FIN-20322>
                
                //<SRI.INI-20322>
                PensionAnual1 = double.Parse(PensionAnualHoy.Text);
                PensionAnual2 = double.Parse(PensionAnualJub.Text);
                //<SRI.INI-20322>

                // Pensión total
                double pensionTotalHoy = ObtenerPensionTotalHoy();
                double pensionTotalJub = ObtenerPensionTotalJub();
                PensionTotalHoy.Text = String.Format("{0:0,0.00}", Math.Round(pensionTotalHoy, 2));
                PensionTotalJub.Text = String.Format("{0:0,0.00}", Math.Round(pensionTotalJub, 2));
                //<SRI.INI-20322>
                //PensionTotalDif.Text = String.Format("{0:0,0.00}", Math.Round(pensionTotalJub, 2) - Math.Round(pensionTotalHoy, 2));
                //<SRI.FIN-20322>

                //<SRI.INI-20322>
                PensionTotal1 = double.Parse(PensionTotalHoy.Text);
                PensionTotal2 = double.Parse(PensionTotalJub.Text);
                //<SRI.INI-20322>

                Grafico.Text = "<div id=\"Grafico" + Id + "\" style=\"width:410px; height:260px\"></div>";
            }
            else
            {
                SinPermisos.Visible = true;
                SimuladorData.Visible = false;
            }
        }

        public double ObtenerPensionTotalHoy()
        {
            return -Financial.FV(TasaAjuste / 100, (Convert.ToInt32(ConfigurationManager.AppSettings["EdadMaxJubilacion"]) - EdadActual), Pension1 * 12);
        }

        public double ObtenerPensionTotalJub()
        {
            return -Financial.FV(TasaAjuste / 100, (Convert.ToInt32(ConfigurationManager.AppSettings["EdadMaxJubilacion"]) - EdadJubilarse), Pension2 * 12);
        }
    }
}