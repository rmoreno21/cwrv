using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
//<SRI.INI-20322>
using Interseguro.CWRV.Infraestructura.General;
//<SRI.fIN-20322>

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCotizacionesSimulador : System.Web.UI.UserControl
    {
        public List<Cotizacion> Cotizaciones { get; set; }
        public string Filtro { get; set; }
        public bool VerDragGraficar { get; set; }
        public bool VerNroCotizacion { get; set; }
        public bool VerMoneda { get; set; }
        public bool VerModalidad { get; set; }
        public bool VerPeriodoDiferido { get; set; }
        public bool VerPeriodoGarantizado { get; set; }
        public bool VerPension { get; set; }
        public bool VerPension2 { get; set; }
        public bool VerRadioGraficar1 { get; set; }
        public bool VerRadioGraficar2 { get; set; }
        //<SRI.INI-20322>
        public bool VerGratificacion { get; set; }
        public int idSimulador { get; set; }
        //<SRI.fIN-20322>

        protected void Page_Load(object sender, EventArgs e)
        {
            TabCotizacionesSimulador.Columns[0].Visible = VerDragGraficar;
            TabCotizacionesSimulador.Columns[1].Visible = VerNroCotizacion;
            TabCotizacionesSimulador.Columns[2].Visible = VerMoneda;
            TabCotizacionesSimulador.Columns[3].Visible = VerModalidad;
            TabCotizacionesSimulador.Columns[4].Visible = VerPeriodoDiferido;
            TabCotizacionesSimulador.Columns[5].Visible = VerPeriodoGarantizado;
            //<SRI.INI-20322>
            //TabCotizacionesSimulador.Columns[6].Visible = VerPension;
            //TabCotizacionesSimulador.Columns[7].Visible = VerRadioGraficar1;
            //TabCotizacionesSimulador.Columns[8].Visible = VerRadioGraficar2;
            TabCotizacionesSimulador.Columns[6].Visible = VerGratificacion;
            TabCotizacionesSimulador.Columns[7].Visible = VerPension;
            TabCotizacionesSimulador.Columns[8].Visible = VerPension2;
            TabCotizacionesSimulador.Columns[9].Visible = VerRadioGraficar1;
            TabCotizacionesSimulador.Columns[10].Visible = VerRadioGraficar2;
            //<SRI.FIN-20322>

            if (VerRadioGraficar2)
            {
                //<SRI.INI-20322>
                //TabCotizacionesSimulador.Columns[7].HeaderText = "Graficar 1";
                //TabCotizacionesSimulador.Columns[8].HeaderText = "Graficar 2";
                //TabCotizacionesSimulador.Columns[7].ItemStyle.Width = 65;
                //TabCotizacionesSimulador.Columns[8].ItemStyle.Width = 65;
                TabCotizacionesSimulador.Columns[9].HeaderText = "Graficar 1";
                TabCotizacionesSimulador.Columns[10].HeaderText = "Graficar 2";
                TabCotizacionesSimulador.Columns[9].ItemStyle.Width = 65;
                TabCotizacionesSimulador.Columns[10].ItemStyle.Width = 65;
                //<SRI.FIN-20322>
            }

            //<SRI.INI-20322>
            if (idSimulador == (int)Enums.OpcionesSistema.SimuladorQueMeConviene)
            {
                TabCotizacionesSimulador.Columns[2].HeaderText = "Mon.";
                TabCotizacionesSimulador.Columns[3].HeaderText = "Mod.";
                TabCotizacionesSimulador.Columns[4].HeaderText = "PD";
                TabCotizacionesSimulador.Columns[7].HeaderText = "Pen. 1";
                TabCotizacionesSimulador.Columns[8].HeaderText = "Pen. 2";
            }
            else if (idSimulador == (int)Enums.OpcionesSistema.SimuladorTipoMoneda)
            {
                TabCotizacionesSimulador.Columns[4].HeaderText = "PD";
            }
            else
            {
                TabCotizacionesSimulador.Columns[2].HeaderText = "Moneda";
                TabCotizacionesSimulador.Columns[3].HeaderText = "Modalidad";
                TabCotizacionesSimulador.Columns[4].HeaderText = "P. Dif.";
                TabCotizacionesSimulador.Columns[7].HeaderText = "Pensión";
                TabCotizacionesSimulador.Columns[8].HeaderText = "Pensión 2";
            }
            //<SRI.FIN-20322>

            TabCotizacionesSimulador.DataSource = Cotizaciones;
            TabCotizacionesSimulador.DataBind();
            if (Cotizaciones.Count > 0)
            {
                TabCotizacionesSimulador.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

        }

    }
}