using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;


namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class SimuladorPlazoFijo : System.Web.UI.UserControl
    {
        public List<GraficoLineal> GraficoCIA { get; set; }
        public List<GraficoLineal> GraficoAFP { get; set; }

        public double MOD101Pension { get; set; }
        public double MOD102Pension { get; set; }
        public double MOD103Pension { get; set; }
        public double MOD104Pension { get; set; }
        public double MOD105Pension { get; set; }
        public double MOD106Pension { get; set; }
        public double MOD107Pension { get; set; }
        public double MOD108Pension { get; set; }
        public double MOD109Pension { get; set; }
        public double MOD110Pension { get; set; }
        public double MOD111Pension { get; set; }
        public double MOD112Pension { get; set; }
        public double MOD113Pension { get; set; }
        public double MOD114Pension { get; set; }
        public double MOD115Pension { get; set; }
        public double MOD116Pension { get; set; }
        public double MOD117Pension { get; set; }
        public double MOD118Pension { get; set; }
        public double MOD119Pension { get; set; }
        public double MOD120Pension { get; set; }

        
        public double MOD2101TotalMensual { get; set; }
        public double MOD2102TotalMensual { get; set; }
        public double MOD2103TotalMensual { get; set; }
        public double MOD2104TotalMensual { get; set; }
        public double MOD2105TotalMensual { get; set; }
        public double MOD2106TotalMensual { get; set; }
        public double MOD2107TotalMensual { get; set; }
        public double MOD2108TotalMensual { get; set; }
        public double MOD2109TotalMensual { get; set; }
        public double MOD2110TotalMensual { get; set; }
        public double MOD2111TotalMensual { get; set; }
        public double MOD2112TotalMensual { get; set; }
        public double MOD2113TotalMensual { get; set; }
        public double MOD2114TotalMensual { get; set; }
        public double MOD2115TotalMensual { get; set; }
        public double MOD2116TotalMensual { get; set; }
        public double MOD2117TotalMensual { get; set; }
        public double MOD2118TotalMensual { get; set; }
        public double MOD2119TotalMensual { get; set; }
        public double MOD2120TotalMensual { get; set; }

        public string MOD2200Leyenda { get; set; }
        public double MOD2201TotalMensual { get; set; }
        public double MOD2202TotalMensual { get; set; }
        public double MOD2203TotalMensual { get; set; }
        public double MOD2204TotalMensual { get; set; }
        public double MOD2205TotalMensual { get; set; }
        public double MOD2206TotalMensual { get; set; }
        public double MOD2207TotalMensual { get; set; }
        public double MOD2208TotalMensual { get; set; }
        public double MOD2209TotalMensual { get; set; }
        public double MOD2210TotalMensual { get; set; }
        public double MOD2211TotalMensual { get; set; }
        public double MOD2212TotalMensual { get; set; }
        public double MOD2213TotalMensual { get; set; }
        public double MOD2214TotalMensual { get; set; }
        public double MOD2215TotalMensual { get; set; }
        public double MOD2216TotalMensual { get; set; }
        public double MOD2217TotalMensual { get; set; }
        public double MOD2218TotalMensual { get; set; }
        public double MOD2219TotalMensual { get; set; }
        public double MOD2220TotalMensual { get; set; }

        public string MOD2300Leyenda { get; set; }
        public double MOD2301TotalMensual { get; set; }
        public double MOD2302TotalMensual { get; set; }
        public double MOD2303TotalMensual { get; set; }
        public double MOD2304TotalMensual { get; set; }
        public double MOD2305TotalMensual { get; set; }
        public double MOD2306TotalMensual { get; set; }
        public double MOD2307TotalMensual { get; set; }
        public double MOD2308TotalMensual { get; set; }
        public double MOD2309TotalMensual { get; set; }
        public double MOD2310TotalMensual { get; set; }
        public double MOD2311TotalMensual { get; set; }
        public double MOD2312TotalMensual { get; set; }
        public double MOD2313TotalMensual { get; set; }
        public double MOD2314TotalMensual { get; set; }
        public double MOD2315TotalMensual { get; set; }
        public double MOD2316TotalMensual { get; set; }
        public double MOD2317TotalMensual { get; set; }
        public double MOD2318TotalMensual { get; set; }
        public double MOD2319TotalMensual { get; set; }
        public double MOD2320TotalMensual { get; set; }

        public string MOD2400Leyenda { get; set; }
        public double MOD2401TotalMensual { get; set; }
        public double MOD2402TotalMensual { get; set; }
        public double MOD2403TotalMensual { get; set; }
        public double MOD2404TotalMensual { get; set; }
        public double MOD2405TotalMensual { get; set; }
        public double MOD2406TotalMensual { get; set; }
        public double MOD2407TotalMensual { get; set; }
        public double MOD2408TotalMensual { get; set; }
        public double MOD2409TotalMensual { get; set; }
        public double MOD2410TotalMensual { get; set; }
        public double MOD2411TotalMensual { get; set; }
        public double MOD2412TotalMensual { get; set; }
        public double MOD2413TotalMensual { get; set; }
        public double MOD2414TotalMensual { get; set; }
        public double MOD2415TotalMensual { get; set; }
        public double MOD2416TotalMensual { get; set; }
        public double MOD2417TotalMensual { get; set; }
        public double MOD2418TotalMensual { get; set; }
        public double MOD2419TotalMensual { get; set; }
        public double MOD2420TotalMensual { get; set; }

        public bool PermisoEjecutar { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoEjecutar)
            {
                SinPermisos.Visible = false;
                SimuladorData.Visible = true;

                FormatearMonto();


                //MOD105PXM.Text = String.Format("{0:0,0.00}", MOD105Pension);
                //MOD205PXM.Text = String.Format("{0:0,0.00}", MOD205Pension);
                //MOD305PXM.Text = String.Format("{0:0,0.00}", MOD305Pension);

                //if (MOD105Pension > MOD205Pension && MOD105Pension > MOD305Pension) MOD105PXM.Text = String.Format("<strong>{0}</strong>", MOD105PXM.Text);
                //else if (MOD205Pension > MOD105Pension && MOD205Pension > MOD305Pension) MOD205PXM.Text = String.Format("<strong>{0}</strong>", MOD205PXM.Text);
                //else if (MOD305Pension > MOD105Pension && MOD305Pension > MOD205Pension) MOD305PXM.Text = String.Format("<strong>{0}</strong>", MOD305PXM.Text);

                //MOD110PXM.Text = String.Format("{0:0,0.00}", MOD110Pension);
                //MOD210PXM.Text = String.Format("{0:0,0.00}", MOD210Pension);
                //MOD310PXM.Text = String.Format("{0:0,0.00}", MOD310Pension);
                //if (MOD110Pension > MOD210Pension && MOD110Pension > MOD310Pension) MOD110PXM.Text = String.Format("<strong>{0}</strong>", MOD110PXM.Text);
                //else if (MOD210Pension > MOD110Pension && MOD210Pension > MOD310Pension) MOD210PXM.Text = String.Format("<strong>{0}</strong>", MOD210PXM.Text);
                //else if (MOD310Pension > MOD110Pension && MOD310Pension > MOD210Pension) MOD310PXM.Text = String.Format("<strong>{0}</strong>", MOD310PXM.Text);


                //MOD115PXM.Text = String.Format("{0:0,0.00}", MOD115Pension);
                //MOD215PXM.Text = String.Format("{0:0,0.00}", MOD215Pension);
                //MOD315PXM.Text = String.Format("{0:0,0.00}", MOD315Pension);
                //if (MOD115Pension > MOD215Pension && MOD115Pension > MOD315Pension) MOD115PXM.Text = String.Format("<strong>{0}</strong>", MOD115PXM.Text);
                //else if (MOD215Pension > MOD115Pension && MOD215Pension > MOD315Pension) MOD215PXM.Text = String.Format("<strong>{0}</strong>", MOD215PXM.Text);
                //else if (MOD315Pension > MOD115Pension && MOD315Pension > MOD215Pension) MOD315PXM.Text = String.Format("<strong>{0}</strong>", MOD315PXM.Text);

                //MOD120PXM.Text = String.Format("{0:0,0.00}", MOD120Pension);
                //MOD220PXM.Text = String.Format("{0:0,0.00}", MOD220Pension);
                //MOD320PXM.Text = String.Format("{0:0,0.00}", MOD320Pension);
                //if (MOD120Pension > MOD220Pension && MOD120Pension > MOD320Pension) MOD120PXM.Text = String.Format("<strong>{0}</strong>", MOD120PXM.Text);
                //else if (MOD220Pension > MOD120Pension && MOD220Pension > MOD320Pension) MOD220PXM.Text = String.Format("<strong>{0}</strong>", MOD220PXM.Text);
                //else if (MOD320Pension > MOD120Pension && MOD320Pension > MOD220Pension) MOD320PXM.Text = String.Format("<strong>{0}</strong>", MOD320PXM.Text);

                //MOD125PXM.Text = String.Format("{0:0,0.00}", MOD125Pension);
                //MOD225PXM.Text = String.Format("{0:0,0.00}", MOD225Pension);
                //MOD325PXM.Text = String.Format("{0:0,0.00}", MOD325Pension);
                //if (MOD125Pension > MOD225Pension && MOD125Pension > MOD325Pension) MOD125PXM.Text = String.Format("<strong>{0}</strong>", MOD125PXM.Text);
                //else if (MOD225Pension > MOD125Pension && MOD225Pension > MOD325Pension) MOD225PXM.Text = String.Format("<strong>{0}</strong>", MOD225PXM.Text);
                //else if (MOD325Pension > MOD125Pension && MOD325Pension > MOD225Pension) MOD325PXM.Text = String.Format("<strong>{0}</strong>", MOD325PXM.Text);

                //MOD105ACU.Text = String.Format("{0:0,0.00}", MOD105Acumulado);
                //MOD110ACU.Text = String.Format("{0:0,0.00}", MOD110Acumulado);
                //MOD115ACU.Text = String.Format("{0:0,0.00}", MOD115Acumulado);
                //MOD120ACU.Text = String.Format("{0:0,0.00}", MOD120Acumulado);
                //MOD125ACU.Text = String.Format("{0:0,0.00}", MOD125Acumulado);

                //MOD205ACU.Text = String.Format("{0:0,0.00}", MOD205Acumulado);
                //MOD210ACU.Text = String.Format("{0:0,0.00}", MOD210Acumulado);
                //MOD215ACU.Text = String.Format("{0:0,0.00}", MOD215Acumulado);
                //MOD220ACU.Text = String.Format("{0:0,0.00}", MOD220Acumulado);
                //MOD225ACU.Text = String.Format("{0:0,0.00}", MOD225Acumulado);

                //MOD305ACU.Text = String.Format("{0:0,0.00}", MOD305Acumulado);
                //MOD310ACU.Text = String.Format("{0:0,0.00}", MOD310Acumulado);
                //MOD315ACU.Text = String.Format("{0:0,0.00}", MOD315Acumulado);
                //MOD320ACU.Text = String.Format("{0:0,0.00}", MOD320Acumulado);
                //MOD325ACU.Text = String.Format("{0:0,0.00}", MOD325Acumulado);
            }
            else
            {
                SinPermisos.Visible = true;
                SimuladorData.Visible = false;
            }

        }



        void FormatearMonto()
        {
            MOD101.Text = String.Format("{0:0,0.00}", MOD101Pension);
            MOD102.Text = String.Format("{0:0,0.00}", MOD102Pension);
            MOD103.Text = String.Format("{0:0,0.00}", MOD103Pension);
            MOD104.Text = String.Format("{0:0,0.00}", MOD104Pension);
            MOD105.Text = String.Format("{0:0,0.00}", MOD105Pension);
            MOD106.Text = String.Format("{0:0,0.00}", MOD106Pension);
            MOD107.Text = String.Format("{0:0,0.00}", MOD107Pension);
            MOD108.Text = String.Format("{0:0,0.00}", MOD108Pension);
            MOD109.Text = String.Format("{0:0,0.00}", MOD109Pension);
            MOD110.Text = String.Format("{0:0,0.00}", MOD110Pension);
            MOD111.Text = String.Format("{0:0,0.00}", MOD111Pension);
            MOD112.Text = String.Format("{0:0,0.00}", MOD112Pension);
            MOD113.Text = String.Format("{0:0,0.00}", MOD113Pension);
            MOD114.Text = String.Format("{0:0,0.00}", MOD114Pension);
            MOD115.Text = String.Format("{0:0,0.00}", MOD115Pension);
            MOD116.Text = String.Format("{0:0,0.00}", MOD116Pension);
            MOD117.Text = String.Format("{0:0,0.00}", MOD117Pension);
            MOD118.Text = String.Format("{0:0,0.00}", MOD118Pension);
            MOD119.Text = String.Format("{0:0,0.00}", MOD119Pension);
            MOD120.Text = String.Format("{0:0,0.00}", MOD120Pension);

            MOD2101.Text = String.Format("{0:0,0.00}", MOD2101TotalMensual);
            MOD2102.Text = String.Format("{0:0,0.00}", MOD2102TotalMensual);
            MOD2103.Text = String.Format("{0:0,0.00}", MOD2103TotalMensual);
            MOD2104.Text = String.Format("{0:0,0.00}", MOD2104TotalMensual);
            MOD2105.Text = String.Format("{0:0,0.00}", MOD2105TotalMensual);
            MOD2106.Text = String.Format("{0:0,0.00}", MOD2106TotalMensual);
            MOD2107.Text = String.Format("{0:0,0.00}", MOD2107TotalMensual);
            MOD2108.Text = String.Format("{0:0,0.00}", MOD2108TotalMensual);
            MOD2109.Text = String.Format("{0:0,0.00}", MOD2109TotalMensual);
            MOD2110.Text = String.Format("{0:0,0.00}", MOD2110TotalMensual);
            MOD2111.Text = String.Format("{0:0,0.00}", MOD2111TotalMensual);
            MOD2112.Text = String.Format("{0:0,0.00}", MOD2112TotalMensual);
            MOD2113.Text = String.Format("{0:0,0.00}", MOD2113TotalMensual);
            MOD2114.Text = String.Format("{0:0,0.00}", MOD2114TotalMensual);
            MOD2115.Text = String.Format("{0:0,0.00}", MOD2115TotalMensual);
            MOD2116.Text = String.Format("{0:0,0.00}", MOD2116TotalMensual);
            MOD2117.Text = String.Format("{0:0,0.00}", MOD2117TotalMensual);
            MOD2118.Text = String.Format("{0:0,0.00}", MOD2118TotalMensual);
            MOD2119.Text = String.Format("{0:0,0.00}", MOD2119TotalMensual);
            MOD2120.Text = String.Format("{0:0,0.00}", MOD2120TotalMensual);

            MOD2200.Text = MOD2200Leyenda;
            MOD2201.Text = String.Format("{0:0,0.00}", MOD2201TotalMensual);
            MOD2202.Text = String.Format("{0:0,0.00}", MOD2202TotalMensual);
            MOD2203.Text = String.Format("{0:0,0.00}", MOD2203TotalMensual);
            MOD2204.Text = String.Format("{0:0,0.00}", MOD2204TotalMensual);
            MOD2205.Text = String.Format("{0:0,0.00}", MOD2205TotalMensual);
            MOD2206.Text = String.Format("{0:0,0.00}", MOD2206TotalMensual);
            MOD2207.Text = String.Format("{0:0,0.00}", MOD2207TotalMensual);
            MOD2208.Text = String.Format("{0:0,0.00}", MOD2208TotalMensual);
            MOD2209.Text = String.Format("{0:0,0.00}", MOD2209TotalMensual);
            MOD2210.Text = String.Format("{0:0,0.00}", MOD2210TotalMensual);
            MOD2211.Text = String.Format("{0:0,0.00}", MOD2211TotalMensual);
            MOD2212.Text = String.Format("{0:0,0.00}", MOD2212TotalMensual);
            MOD2213.Text = String.Format("{0:0,0.00}", MOD2213TotalMensual);
            MOD2214.Text = String.Format("{0:0,0.00}", MOD2214TotalMensual);
            MOD2215.Text = String.Format("{0:0,0.00}", MOD2215TotalMensual);
            MOD2216.Text = String.Format("{0:0,0.00}", MOD2216TotalMensual);
            MOD2217.Text = String.Format("{0:0,0.00}", MOD2217TotalMensual);
            MOD2218.Text = String.Format("{0:0,0.00}", MOD2218TotalMensual);
            MOD2219.Text = String.Format("{0:0,0.00}", MOD2219TotalMensual);
            MOD2220.Text = String.Format("{0:0,0.00}", MOD2220TotalMensual);

            MOD2300.Text = MOD2300Leyenda;
            MOD2301.Text = String.Format("{0:0,0.00}", MOD2301TotalMensual);
            MOD2302.Text = String.Format("{0:0,0.00}", MOD2302TotalMensual);
            MOD2303.Text = String.Format("{0:0,0.00}", MOD2303TotalMensual);
            MOD2304.Text = String.Format("{0:0,0.00}", MOD2304TotalMensual);
            MOD2305.Text = String.Format("{0:0,0.00}", MOD2305TotalMensual);
            MOD2306.Text = String.Format("{0:0,0.00}", MOD2306TotalMensual);
            MOD2307.Text = String.Format("{0:0,0.00}", MOD2307TotalMensual);
            MOD2308.Text = String.Format("{0:0,0.00}", MOD2308TotalMensual);
            MOD2309.Text = String.Format("{0:0,0.00}", MOD2309TotalMensual);
            MOD2310.Text = String.Format("{0:0,0.00}", MOD2310TotalMensual);
            MOD2311.Text = String.Format("{0:0,0.00}", MOD2311TotalMensual);
            MOD2312.Text = String.Format("{0:0,0.00}", MOD2312TotalMensual);
            MOD2313.Text = String.Format("{0:0,0.00}", MOD2313TotalMensual);
            MOD2314.Text = String.Format("{0:0,0.00}", MOD2314TotalMensual);
            MOD2315.Text = String.Format("{0:0,0.00}", MOD2315TotalMensual);
            MOD2316.Text = String.Format("{0:0,0.00}", MOD2316TotalMensual);
            MOD2317.Text = String.Format("{0:0,0.00}", MOD2317TotalMensual);
            MOD2318.Text = String.Format("{0:0,0.00}", MOD2318TotalMensual);
            MOD2319.Text = String.Format("{0:0,0.00}", MOD2319TotalMensual);
            MOD2320.Text = String.Format("{0:0,0.00}", MOD2320TotalMensual);

            MOD2400.Text = MOD2400Leyenda;
            MOD2401.Text = String.Format("{0:0,0.00}", MOD2401TotalMensual);
            MOD2402.Text = String.Format("{0:0,0.00}", MOD2402TotalMensual);
            MOD2403.Text = String.Format("{0:0,0.00}", MOD2403TotalMensual);
            MOD2404.Text = String.Format("{0:0,0.00}", MOD2404TotalMensual);
            MOD2405.Text = String.Format("{0:0,0.00}", MOD2405TotalMensual);
            MOD2406.Text = String.Format("{0:0,0.00}", MOD2406TotalMensual);
            MOD2407.Text = String.Format("{0:0,0.00}", MOD2407TotalMensual);
            MOD2408.Text = String.Format("{0:0,0.00}", MOD2408TotalMensual);
            MOD2409.Text = String.Format("{0:0,0.00}", MOD2409TotalMensual);
            MOD2410.Text = String.Format("{0:0,0.00}", MOD2410TotalMensual);
            MOD2411.Text = String.Format("{0:0,0.00}", MOD2411TotalMensual);
            MOD2412.Text = String.Format("{0:0,0.00}", MOD2412TotalMensual);
            MOD2413.Text = String.Format("{0:0,0.00}", MOD2413TotalMensual);
            MOD2414.Text = String.Format("{0:0,0.00}", MOD2414TotalMensual);
            MOD2415.Text = String.Format("{0:0,0.00}", MOD2415TotalMensual);
            MOD2416.Text = String.Format("{0:0,0.00}", MOD2416TotalMensual);
            MOD2417.Text = String.Format("{0:0,0.00}", MOD2417TotalMensual);
            MOD2418.Text = String.Format("{0:0,0.00}", MOD2418TotalMensual);
            MOD2419.Text = String.Format("{0:0,0.00}", MOD2419TotalMensual);
            MOD2420.Text = String.Format("{0:0,0.00}", MOD2420TotalMensual);


        }
    }

   
}