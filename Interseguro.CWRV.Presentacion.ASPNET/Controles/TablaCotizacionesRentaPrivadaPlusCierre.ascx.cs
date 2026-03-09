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
    public partial class TablaCotizacionesRentaPrivadaPlusCierre : System.Web.UI.UserControl
    {
        public List<CotizacionRPPlus> CotizacionesRPPlus { get; set; }
        public bool PermisoAgregar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoEliminar { get; set; }
        public bool PermisoTRA { get; set; }
        public string Moneda { get; set; }
        public int Temporalidad { get; set; }

        public bool PermisoEspeciales { get; set; }

        //<INIGTI_753>
        public bool Conyuge { get; set; }
        //<FINGTI_753>
        protected void Page_Load(object sender, EventArgs e)
        {
            List<Parametro> lstTramoEscalonada = (List<Parametro>)Session["ComboPorcentajeEscalonada"];
            lstTramoEscalonada = lstTramoEscalonada.ToList();

            foreach (var item in CotizacionesRPPlus)
            {
                //Combobox Tramo Escalonada    
                var ValorEscalon = "0";
                if (item.PjePE > 0)
                {
                    ValorEscalon = lstTramoEscalonada.ToList().Find(esc => (((Convert.ToDouble(esc.Valor_1) - 0.000000001) * 100) <= (item.PjePE)) && (((Convert.ToDouble(esc.Valor_1) + 0.000000001) * 100) >= (item.PjePE))).Id;
                }
                else
                {
                    ValorEscalon = "100";
                }

                item.PjePE = Convert.ToDouble(ValorEscalon);

            }

            TabCotizaciones_RP.DataSource = CotizacionesRPPlus;
            TabCotizaciones_RP.DataBind();
            if (CotizacionesRPPlus.Count > 0)
            {
                TabCotizaciones_RP.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            
            //<INIGTI_753>

            TabCotizaciones_RP.Columns[9].Visible = Conyuge;
            TabCotizaciones_RP.Columns[14].Visible = PermisoEspeciales;
            TabCotizaciones_RP.Columns[15].Visible = PermisoEspeciales;
            TabCotizaciones_RP.Columns[16].Visible = PermisoEspeciales;
            TabCotizaciones_RP.Columns[17].Visible = PermisoTRA;

            //var CotizacionSeleccionada = CotizacionesIFP.FindAll(p => p.IndSeleccionada == "S" && p.EstadoCotizacion == "04");
            //if (CotizacionSeleccionada.Count > 0)
            //{
            //    TabCotizaciones_RP.Columns[17].Visible = false;
                
            //}


            //<FINGTI_753>


            TabCotizacionesLeyenda_RP.Visible = PermisoEspeciales;



        }

        protected void TabCotizaciones_RB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);


                // CheckBox Sepelio
                //CheckBox ChkSepelio;
                //ChkSepelio = (CheckBox)(fila.FindControl("ChkSepelio"));
                //if (CotizacionesIFP[e.Row.RowIndex].IndGastoSepelio == "S")
                //{
                //    ChkSepelio.Checked = true;
                //}
                //else
                //{
                //    ChkSepelio.Checked = false;
                //}

                // Combobox Moneda
                //DropDownList TabCotMoneda;
                //TabCotMoneda = (DropDownList)(fila.FindControl("TabCotMoneda"));
                //TabCotMoneda.ClearSelection();
                //CargarCombobox(TabCotMoneda, (List<Parametro>)Session["ComboMoneda"], true, Moneda);
                //TabCotMoneda.SelectedIndex = TabCotMoneda.Items.IndexOf(TabCotMoneda.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].Moneda.Id));

                // Combobox Período Garantizado
                //DropDownList TabCotPeriodoGarantizado;
                //TabCotPeriodoGarantizado = (DropDownList)(fila.FindControl("TabCotPeriodoGarantizado"));
                //TabCotPeriodoGarantizado.ClearSelection();
                //// Filtramos los Períodos Garantizados mayores a la temporalidad
                //List<Parametro> listaComboPeriodoGarantizado = (List<Parametro>)Session["ComboPeriodoGarantizado"];
                //listaComboPeriodoGarantizado = listaComboPeriodoGarantizado.FindAll(pg => Convert.ToInt32(pg.Id) <= Temporalidad);
                //CargarCombobox(TabCotPeriodoGarantizado, listaComboPeriodoGarantizado, false);
                //TabCotPeriodoGarantizado.SelectedIndex = TabCotPeriodoGarantizado.Items.IndexOf(TabCotPeriodoGarantizado.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].PeriodoGarantizado.ToString()));

                //// Combobox Pago Doble
                //DropDownList TabCotPagoEscalonada;
                //TabCotPagoEscalonada = (DropDownList)(fila.FindControl("TabCotPagoEscalonada"));
                //TabCotPagoEscalonada.ClearSelection();
                //// Filtramos los Pago Doble mayores a la temporalidad
                //List<Parametro> listaComboPagoDoble = (List<Parametro>)Session["ComboPagoDoble"];
                //listaComboPagoDoble = listaComboPagoDoble.FindAll(pg => Convert.ToInt32(pg.Id) < Temporalidad);
                //CargarCombobox(TabCotPagoEscalonada, listaComboPagoDoble, false);
                //TabCotPagoEscalonada.SelectedIndex = TabCotPagoEscalonada.Items.IndexOf(TabCotPagoEscalonada.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].PagoEscalonada.ToString()));


                //DropDownList TabCotPjePagoEscalonada;
                //TabCotPjePagoEscalonada = (DropDownList)(fila.FindControl("TabCotPjePagoEscalonada"));
                //TabCotPjePagoEscalonada.ClearSelection();
                //CargarCombobox(TabCotPjePagoEscalonada, (List<Parametro>)Session["ComboPorcentajeEscalonada"], false);
                //TabCotPjePagoEscalonada.SelectedIndex = TabCotPjePagoEscalonada.Items.IndexOf(TabCotPjePagoEscalonada.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].PjePE.ToString()));


                //DropDownList TabCotPjeDevolucion;
                //TabCotPjeDevolucion = (DropDownList)(fila.FindControl("TabCotPjeDevolucion"));
                //TabCotPjeDevolucion.ClearSelection();
                //CargarCombobox(TabCotPjeDevolucion, (List<Parametro>)Session["ComboPorcentajeDevolucion"], false);
                //TabCotPjeDevolucion.SelectedIndex = TabCotPjeDevolucion.Items.IndexOf(TabCotPjeDevolucion.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].ValPjeDev.ToString()));

                //TextBox TabCotTRA;
                //TabCotTRA = (TextBox)(fila.FindControl("TabCotTRA"));
                //TabCotTRA.Text = CotizacionesIFP[e.Row.RowIndex].AjusteTRA.ToString();


                //if (!PermisoModificar)
                //{
                //    TabCotMoneda.Enabled = false;
                //    TabCotPeriodoGarantizado.Enabled = false;
                //}

                if (CotizacionesRPPlus[e.Row.RowIndex].IndCotiza == "**" || CotizacionesRPPlus[e.Row.RowIndex].IndCotiza == "***")
                {
                    e.Row.CssClass = "grilla_error_tra";
                }

                if (CotizacionesRPPlus[e.Row.RowIndex].EstadoCotizacion == "04" && CotizacionesRPPlus[e.Row.RowIndex].IndSeleccionada == "S")
                {
                    e.Row.CssClass = "grilla_active_green";
                }

                //grilla_active_green

                //<INIGTI_753>

                //DropDownList TabCotPjeConyuge;
                //TabCotPjeConyuge = (DropDownList)(fila.FindControl("TabCotPjeConyuge"));
                //TabCotPjeConyuge.ClearSelection();
                //CargarCombobox(TabCotPjeConyuge, (List<Parametro>)Session["ComboPorcentajeConyuge"], false);
                //TabCotPjeConyuge.SelectedIndex = TabCotPjeConyuge.Items.IndexOf(TabCotPjeConyuge.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].ValPjeConyuge.ToString()));


                //if (!PermisoEspeciales)
                //    TabCotPjeConyuge.Width = 60;

                //<FINGTI_753>


                //<INIGTI_753>

                //DropDownList TabCotAjusteMoneda;
                //TabCotAjusteMoneda = (DropDownList)(fila.FindControl("TabCotAjusteMoneda"));
                //TabCotAjusteMoneda.ClearSelection();

                //if (!PermisoEspeciales)
                //    TabCotAjusteMoneda.Width = 60;

                //List<Parametro> lstParametro = new List<Parametro>();
                //lstParametro = (List<Parametro>)Session["ComboMonedaAjustePlus"];


                //if (lstParametro.Count > 0)
                //{
                //    lstParametro
                //            .FindAll(p => (
                //                           (p.Id == TabCotMoneda.SelectedValue)
                //                          )
                //            )
                //            .ForEach(p =>
                //            {
                //                for (var i = Convert.ToInt32(p.Valor_1); i <= Convert.ToInt32(p.Valor_2); i++)
                //                {
                //                    TabCotAjusteMoneda.Items.Add(new ListItem(i.ToString() + '%', i.ToString()));
                //                }
                //            });
                //    //En caso que no Registre % ejemplo: 001 - 002 monedas
                //    if (TabCotAjusteMoneda.Items.Count == 0)
                //    {
                //        TabCotAjusteMoneda.Items.Add(new ListItem("-", "-1"));
                //    }
                //    TabCotAjusteMoneda.SelectedIndex = TabCotAjusteMoneda.Items.IndexOf(TabCotAjusteMoneda.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].ValMonAju.ToString()));
                //}
                //<FINGTI_753>


                //<INIGTI_753_3>
                //if (HttpContext.Current.Session["ModSolModo"].ToString() == "CONS")
                //{
                //    ChkSepelio.Enabled = false;
                //    TabCotMoneda.Enabled = false;
                //    TabCotPeriodoGarantizado.Enabled = false;
                //    TabCotPagoEscalonada.Enabled = false;
                //    TabCotPjePagoEscalonada.Enabled = false;
                //    TabCotPjeDevolucion.Enabled = false;
                //    TabCotTRA.Enabled = false;
                //    TabCotPjeConyuge.Enabled = false;
                //    TabCotAjusteMoneda.Enabled = false;

                //    TabCotizaciones_RP.Columns[17].Visible = false;
                //    TabCotizacionesBotonera_RP.Visible = false;

                //    TabCotizaciones_RP.Enabled = false;
                //}

                //<FINGTI_753_3>
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
                else
                {
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
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }
    }
}