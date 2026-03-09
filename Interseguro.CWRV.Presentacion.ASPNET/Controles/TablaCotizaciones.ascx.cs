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
    public partial class TablaCotizaciones : System.Web.UI.UserControl
    {
        public List<Cotizacion> Cotizaciones { get; set; }
        public bool PermisoAgregar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoEliminar { get; set; }
        public bool PermisoTRA { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabCotizaciones.DataSource = Cotizaciones;
            TabCotizaciones.DataBind();
            if (Cotizaciones.Count > 0)
            {
                TabCotizaciones.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            if (!PermisoAgregar || Cotizaciones.Count >= Convert.ToInt32(ConfigurationManager.AppSettings["MaxCotizaciones"]))
            {
                TabCotizacionesBotonera.Visible = false;
            }
            if (PermisoTRA)
            {
                TabCotizaciones.Columns[10].Visible = true;
            }
            else
            {
                TabCotizaciones.Columns[10].Visible = false;
            }
        }

        protected void TabCotizaciones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                DropDownList TabCotMoneda;
                TabCotMoneda = (DropDownList)(fila.FindControl("TabCotMoneda"));
                TabCotMoneda.ClearSelection();
                CargarCombobox(TabCotMoneda, (List<Parametro>)Session["ComboMoneda"], true);
                TabCotMoneda.SelectedIndex = TabCotMoneda.Items.IndexOf(TabCotMoneda.Items.FindByValue(Cotizaciones[e.Row.RowIndex].Moneda.Id));

                DropDownList TabCotProducto;
                TabCotProducto = (DropDownList)(fila.FindControl("TabCotProducto"));
                TabCotProducto.ClearSelection();
                CargarCombobox(TabCotProducto, (List<Producto>)Session["ComboProducto"], true);
                TabCotProducto.SelectedIndex = TabCotProducto.Items.IndexOf(TabCotProducto.Items.FindByValue(Cotizaciones[e.Row.RowIndex].Producto.Id));

                DropDownList TabCotModalidad;
                TabCotModalidad = (DropDownList)(fila.FindControl("TabCotModalidad"));
                TabCotModalidad.ClearSelection();
                CargarCombobox(TabCotModalidad, (List<Parametro>)Session["ComboModalidad"], true);
                TabCotModalidad.SelectedIndex = TabCotModalidad.Items.IndexOf(TabCotModalidad.Items.FindByValue(Cotizaciones[e.Row.RowIndex].Modalidad.Id));

                //<SOLINIGTI_754>
                ////DropDownList TabCotPeriodoDiferido;
                ////TabCotPeriodoDiferido = (DropDownList)(fila.FindControl("TabCotPeriodoDiferido"));
                ////TabCotPeriodoDiferido.ClearSelection();
                ////CargarCombobox(TabCotPeriodoDiferido, (List<Parametro>)Session["ComboPeriodoDiferido"], false);
                ////TabCotPeriodoDiferido.SelectedIndex = TabCotPeriodoDiferido.Items.IndexOf(TabCotPeriodoDiferido.Items.FindByValue(Cotizaciones[e.Row.RowIndex].PeriodoDiferido.ToString()));

                DropDownList TabCotPeriodoDiferido;
                TabCotPeriodoDiferido = (DropDownList)(fila.FindControl("TabCotPeriodoDiferido"));
                TabCotPeriodoDiferido.ClearSelection();

                List<Parametro> lstParametro = new List<Parametro>();
                lstParametro =  (List<Parametro>)Session["ComboPeriodoTemporal"];

                if (lstParametro.Count > 0)
                {
                    //Parametro parametro = lstParametro.Find(x => x.Id == TabCotModalidad.SelectedValue);
                    //if (parametro != null)
                    //{
                    //    for (var i = Convert.ToInt32(parametro.Valor_1); i <= Convert.ToInt32(parametro.Valor_2); i++)
                    //    {
                    //        TabCotPeriodoDiferido.Items.Add(new ListItem(i.ToString(), i.ToString()));
                    //    }
                    //}

                    lstParametro
                            .FindAll(p => (
                                           (p.Id == TabCotModalidad.SelectedValue)
                                          )
                            )
                            .ForEach(p =>
                            {
                                for (var i = Convert.ToInt32(p.Valor_1); i <= Convert.ToInt32(p.Valor_2); i++)
                                {
                                    //TabCotPeriodoDiferido.Items.Add(new Parametro { Valor_1 = i.ToString(), Valor_2 = i.ToString() });
                                     TabCotPeriodoDiferido.Items.Add(new ListItem(i.ToString(), i.ToString()));
                                }

                            });

                    TabCotPeriodoDiferido.SelectedIndex = TabCotPeriodoDiferido.Items.IndexOf(TabCotPeriodoDiferido.Items.FindByValue(Cotizaciones[e.Row.RowIndex].PeriodoDiferido.ToString()));
                }
                

                



                //<SOLFINGTI_754>

                DropDownList TabCotPorcentajeRentas;
                TabCotPorcentajeRentas = (DropDownList)(fila.FindControl("TabCotPorcentajeRentas"));
                TabCotPorcentajeRentas.ClearSelection();
                CargarCombobox(TabCotPorcentajeRentas, (List<Parametro>)Session["ComboPorcentajeRentas"], false);
                TabCotPorcentajeRentas.SelectedIndex = TabCotPorcentajeRentas.Items.IndexOf(TabCotPorcentajeRentas.Items.FindByValue(Cotizaciones[e.Row.RowIndex].PorcentajeEntreRentas.ToString()));

                DropDownList TabCotPeriodoGarantizado;
                TabCotPeriodoGarantizado = (DropDownList)(fila.FindControl("TabCotPeriodoGarantizado"));
                TabCotPeriodoGarantizado.ClearSelection();
                CargarCombobox(TabCotPeriodoGarantizado, (List<Parametro>)Session["ComboPeriodoGarantizado"], false);
                TabCotPeriodoGarantizado.SelectedIndex = TabCotPeriodoGarantizado.Items.IndexOf(TabCotPeriodoGarantizado.Items.FindByValue(Cotizaciones[e.Row.RowIndex].PeriodoGarantizado.ToString()));

                //DropDownList TabCotDerechoCrecer;
                //TabCotDerechoCrecer = (DropDownList)(fila.FindControl("TabCotDerechoCrecer"));
                //TabCotDerechoCrecer.ClearSelection();
                //TabCotDerechoCrecer.SelectedIndex = TabCotDerechoCrecer.Items.IndexOf(TabCotDerechoCrecer.Items.FindByValue((Cotizaciones[e.Row.RowIndex].DerechoCrecer) ? "S" : "N"));

                DropDownList TabCotGratificacion;
                TabCotGratificacion = (DropDownList)(fila.FindControl("TabCotGratificacion"));
                TabCotGratificacion.ClearSelection();
                TabCotGratificacion.SelectedIndex = TabCotGratificacion.Items.IndexOf(TabCotGratificacion.Items.FindByValue((Cotizaciones[e.Row.RowIndex].Gratificacion) ? "S" : "N"));

                DropDownList TabCotCapital;
                TabCotCapital = (DropDownList)(fila.FindControl("TabCotCapital"));
                TabCotCapital.ClearSelection();
                CargarCombobox(TabCotCapital, (List<Parametro>)Session["ComboCapital"], false);
                TabCotCapital.SelectedIndex = TabCotCapital.Items.IndexOf(TabCotCapital.Items.FindByValue(Cotizaciones[e.Row.RowIndex].Capital.Id));

                TextBox TabCotTRA;
                TabCotTRA = (TextBox)(fila.FindControl("TabCotTRA"));
                TabCotTRA.Text = Cotizaciones[e.Row.RowIndex].AjusteTRA.ToString();

                if (TabCotModalidad.SelectedValue != "D")
                {
                    TabCotPeriodoDiferido.Enabled = false;
                    TabCotPorcentajeRentas.Enabled = false;
                }
                //<SOLINIGTI_754>
                if (TabCotModalidad.SelectedValue == "I-RVE")
                {
                    TabCotPeriodoDiferido.Enabled = true;
                    TabCotPorcentajeRentas.Enabled = true;
                    TabCotGratificacion.Enabled = false;
                }
                //<SOLFINGTI_754>

                if (!PermisoModificar)
                {
                    TabCotMoneda.Enabled = false;
                    TabCotProducto.Enabled = false;
                    TabCotModalidad.Enabled = false;
                    TabCotPeriodoDiferido.Enabled = false;
                    TabCotPorcentajeRentas.Enabled = false;
                    TabCotPeriodoGarantizado.Enabled = false;
                    TabCotGratificacion.Enabled = false;
                    TabCotCapital.Enabled = false;
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

        private void CargarCombobox(DropDownList control, List<Producto> combobox, bool seleccione)
        {
            control.Items.Clear();
            if (seleccione)
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }
            foreach (Producto item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }
    }
}