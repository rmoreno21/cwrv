using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
using System.Configuration;
using System.Web.Configuration;


namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCotizacionesRentaPrivada : System.Web.UI.UserControl
    {
        public List<CotizacionRP> CotizacionesRP { get; set; }
        public bool PermisoAgregar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoEliminar { get; set; }
        public bool PermisoTRA { get; set; }
        public string Moneda { get; set; }
        public int Temporalidad { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabCotizaciones_RP.DataSource = CotizacionesRP;
            TabCotizaciones_RP.DataBind();
            if (CotizacionesRP.Count > 0)
            {
                TabCotizaciones_RP.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            if (!PermisoAgregar || CotizacionesRP.Count >= Convert.ToInt32(ConfigurationManager.AppSettings["MaxCotizaciones"]))
            {
                TabCotizacionesBotonera_RP.Visible = false;
            }
            if (PermisoTRA)
            {
                TabCotizaciones_RP.Columns[3].Visible = true;
            }
            else
            {
                TabCotizaciones_RP.Columns[3].Visible = false;
            }
        }

        protected void TabCotizaciones_RB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                // Combobox Moneda
                DropDownList TabCotMoneda;
                TabCotMoneda = (DropDownList)(fila.FindControl("TabCotMoneda"));
                TabCotMoneda.ClearSelection();
                CargarCombobox(TabCotMoneda, (List<Parametro>)Session["ComboMoneda"], true, Moneda);
                TabCotMoneda.SelectedIndex = TabCotMoneda.Items.IndexOf(TabCotMoneda.Items.FindByValue(CotizacionesRP[e.Row.RowIndex].Moneda.Id));

                // Combobox Período Garantizado
                DropDownList TabCotPeriodoGarantizado;
                TabCotPeriodoGarantizado = (DropDownList)(fila.FindControl("TabCotPeriodoGarantizado"));
                TabCotPeriodoGarantizado.ClearSelection();
                // Filtramos los Períodos Garantizados mayores a la temporalidad
                List<Parametro> listaComboPeriodoGarantizado = (List<Parametro>)Session["ComboPeriodoGarantizado"];
                listaComboPeriodoGarantizado = listaComboPeriodoGarantizado.FindAll(pg => Convert.ToInt32(pg.Id) <= Temporalidad);
                CargarCombobox(TabCotPeriodoGarantizado, listaComboPeriodoGarantizado, false);
                TabCotPeriodoGarantizado.SelectedIndex = TabCotPeriodoGarantizado.Items.IndexOf(TabCotPeriodoGarantizado.Items.FindByValue(CotizacionesRP[e.Row.RowIndex].PeriodoGarantizado.ToString()));
               
                TextBox TabCotTRA;
                TabCotTRA = (TextBox)(fila.FindControl("TabCotTRA"));
                TabCotTRA.Text = CotizacionesRP[e.Row.RowIndex].AjusteTRA.ToString();


                if (!PermisoModificar)
                {
                    TabCotMoneda.Enabled = false;
                    TabCotPeriodoGarantizado.Enabled = false;
                }
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox, bool seleccione, string monedaFondo)
        {
            control.Items.Clear();
            if (seleccione)
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            foreach (Parametro item in combobox)
            {
                if (monedaFondo != "") //Solo para la moneda se envia dato
                {
                    if (monedaFondo == "001")
                    {
                        if (item.Id == "001") control.Items.Add(new ListItem(item.Glosa, item.Id)); //Sol
                        if (item.Id == "013") control.Items.Add(new ListItem(item.Glosa, item.Id)); //Sol Aju
                    }
                    else if (monedaFondo == "002")
                    {
                        if (item.Id == "002") control.Items.Add(new ListItem(item.Glosa, item.Id)); //Dol
                        if (item.Id == "014") control.Items.Add(new ListItem(item.Glosa, item.Id)); //Dol Aju
                    }
                }
                else {
                    control.Items.Add(new ListItem(item.Glosa, item.Id));
                }
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox, bool seleccione)
        {
            control.Items.Clear();
            if (seleccione)
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }
    }
}