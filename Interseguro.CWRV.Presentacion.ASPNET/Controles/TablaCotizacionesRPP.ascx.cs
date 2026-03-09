using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCotizacionesRPP : System.Web.UI.UserControl
    {
        public List<CotizacionRPPlus> Cotizaciones { get; set; }
        public string Moneda { get; set; }
        public int Temporalidad { get; set; }
        public bool Conyuge { get; set; }
        public bool PermisoTRA { get; set; }
        public bool PermisoEspeciales { get; set; }
        public bool PermisoAgregar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoEliminar { get; set; }

        public char Modo { get; set; }
        public Int32 cantidadFechaCotizacion { get; set; }
        public Int32 cantidadFechaValidacionMonedaIPC { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            TabCotizacionesRPP.DataSource = Cotizaciones;
            TabCotizacionesRPP.DataBind();
            if (Cotizaciones.Count > 0)
            {
                TabCotizacionesRPP.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
            if (!PermisoAgregar || Cotizaciones.Count >= Convert.ToInt32(ConfigurationManager.AppSettings["MaxCotizaciones"]))
            {
                TabCotizacionesBotonera_RP.Visible = false;
            }

            // Porcentaje de Cónyuge
            TabCotizacionesRPP.Columns[5].Visible = Conyuge;

            // Botón Eliminar
            TabCotizacionesRPP.Columns[13].Visible = PermisoEliminar;

            // Permisos especiales
            TabCotizacionesRPP.Columns[10].Visible = PermisoEspeciales; // TRA
            TabCotizacionesRPP.Columns[11].Visible = PermisoEspeciales; // IndCotiza
            if (PermisoEspeciales)
            {
                var cotizacionesSinSolucion = Cotizaciones.FindAll(p => p.IndCotiza == "**");
                if (cotizacionesSinSolucion.Count > 0)
                    TabCotizacionesLeyenda_RP.Visible = true; //Leyenda
            }


            var CotizacionSeleccionada = Cotizaciones.FindAll(p => p.IndSeleccionada == "S" && p.EstadoCotizacion == "04");
            if (CotizacionSeleccionada.Count > 0)
            {
                //TabCotizacionesRPP.Columns[17].Visible = false;
                TabCotizacionesBotonera_RP.Visible = false;
            }
        }

        protected void TabCotizacionesRPP_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)e.Row.Cells[1].NamingContainer;

                if (Cotizaciones[e.Row.RowIndex].IndCotiza == "**")
                {
                    // Pintar la fila de rojo
                    fila.CssClass = "no-cotizable";
                }
                else if (Cotizaciones[e.Row.RowIndex].EstadoCotizacion == "04")
                {
                    // Pintar la fila de rojo
                    fila.CssClass = "seleccionada";
                }

                // Combobox Moneda
                DropDownList TabCotMoneda;
                TabCotMoneda = (DropDownList)fila.FindControl("TabCotMoneda");
                TabCotMoneda.ClearSelection();
                CargarCombobox(TabCotMoneda, (List<Parametro>)Session["RPPComboMoneda"], false, Moneda);
                TabCotMoneda.SelectedIndex = TabCotMoneda.Items.IndexOf(TabCotMoneda.Items.FindByValue(string.Format("{0}:{1}", Cotizaciones[e.Row.RowIndex].Moneda.Id, Cotizaciones[e.Row.RowIndex].ValMonAju)));
                TabCotMoneda.Enabled = PermisoModificar;

                // Combobox Período Garantizado
                DropDownList TabCotPeriodoGarantizado;
                TabCotPeriodoGarantizado = (DropDownList)fila.FindControl("TabCotPeriodoGarantizado");
                TabCotPeriodoGarantizado.ClearSelection();

                // Filtramos los Períodos Garantizados mayores a la temporalidad
                List<Parametro> listaComboPeriodoGarantizado = (List<Parametro>)Session["RPPComboPeriodoGarantizado"];
                listaComboPeriodoGarantizado = listaComboPeriodoGarantizado.FindAll(pg => Convert.ToInt32(pg.Id) <= Temporalidad * 12);
                CargarCombobox(TabCotPeriodoGarantizado, listaComboPeriodoGarantizado, false);
                TabCotPeriodoGarantizado.SelectedIndex = TabCotPeriodoGarantizado.Items.IndexOf(TabCotPeriodoGarantizado.Items.FindByValue((Cotizaciones[e.Row.RowIndex].PeriodoGarantizado).ToString()));
                TabCotPeriodoGarantizado.Enabled = PermisoModificar;

                // Combobox 1er Tramo Escalonada
                DropDownList TabCotPagoEscalonada;
                TabCotPagoEscalonada = (DropDownList)fila.FindControl("TabCotPagoEscalonada");
                TabCotPagoEscalonada.ClearSelection();

                // Filtramos los Pagos Escalonados mayores a la temporalidad
                List<Parametro> listaComboPagoDoble = (List<Parametro>)Session["RPPComboPagoDoble"];
                listaComboPagoDoble = listaComboPagoDoble.FindAll(pg => Convert.ToInt32(pg.Id) < Temporalidad * 12);
                CargarCombobox(TabCotPagoEscalonada, listaComboPagoDoble, false);
                TabCotPagoEscalonada.SelectedIndex = TabCotPagoEscalonada.Items.IndexOf(TabCotPagoEscalonada.Items.FindByValue((Cotizaciones[e.Row.RowIndex].PagoEscalonada).ToString()));
                TabCotPagoEscalonada.Enabled = PermisoModificar;

                // Combobox Renta Porcentaje Escalonada
                DropDownList TabCotPjePagoEscalonada;
                TabCotPjePagoEscalonada = (DropDownList)fila.FindControl("TabCotPjePagoEscalonada");
                TabCotPjePagoEscalonada.ClearSelection();
                List<Parametro> lstTramoEscalonada = (List<Parametro>)Session["RPPComboPorcentajeEscalonada"];
                CargarComboboxconValor(TabCotPjePagoEscalonada, lstTramoEscalonada);
                var valorEscalon = "0";
                if (Cotizaciones[e.Row.RowIndex].PjePE > 0)
                {
                    valorEscalon = lstTramoEscalonada.ToList().Find(esc => (((Convert.ToDouble(esc.Valor_1) - 0.000000001) * 100) <= Cotizaciones[e.Row.RowIndex].PjePE) && (((Convert.ToDouble(esc.Valor_1) + 0.000000001) * 100) >= Cotizaciones[e.Row.RowIndex].PjePE)).Id;
                }
                TabCotPjePagoEscalonada.SelectedIndex = TabCotPjePagoEscalonada.Items.IndexOf(TabCotPjePagoEscalonada.Items.FindByValue(valorEscalon));
                TabCotPjePagoEscalonada.Enabled = PermisoModificar;

                if (Cotizaciones[e.Row.RowIndex].PagoEscalonada > -0.000000001 && Cotizaciones[e.Row.RowIndex].PagoEscalonada < 0.000000001)
                {
                    TabCotPjePagoEscalonada.SelectedIndex = TabCotPjePagoEscalonada.Items.IndexOf(TabCotPjePagoEscalonada.Items.FindByValue("0"));
                    TabCotPjePagoEscalonada.Enabled = false;
                }
                else
                {
                    TabCotPjePagoEscalonada.Items.FindByValue("0").Attributes.Add("Disabled", "Disabled");
                }

                // Porcentaje al cónyuge
                DropDownList TabCotPjeConyuge;
                TabCotPjeConyuge = (DropDownList)(fila.FindControl("TabCotPjeConyuge"));
                TabCotPjeConyuge.ClearSelection();
                CargarCombobox(TabCotPjeConyuge, (List<Parametro>)Session["RPPComboPorcentajeConyuge"], false);
                TabCotPjeConyuge.SelectedIndex = TabCotPjeConyuge.Items.IndexOf(TabCotPjeConyuge.Items.FindByValue(Cotizaciones[e.Row.RowIndex].ValPjeConyuge.ToString()));
                TabCotPjeConyuge.Enabled = PermisoModificar;

                // TRA
                TextBox TabCotTRA;
                TabCotTRA = (TextBox)fila.FindControl("TabCotTRA");
                TabCotTRA.Text = Cotizaciones[e.Row.RowIndex].AjusteTRA.ToString();
                TabCotTRA.ReadOnly = !PermisoModificar;
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox, bool seleccione, string monedaFondo)
        {
            control.Items.Clear();
            if (seleccione)
            {
                control.Items.Add(new ListItem("«Seleccione»", "0"));
            }

            if (Modo == 'N')
            {
                Parametro moneda = new Parametro();
                //moneda.Id = "001:-1";
                moneda = combobox.FirstOrDefault(mon => mon.Id == "001:-1");
                combobox.Remove(moneda);
            }
            else
            {
                if (cantidadFechaCotizacion >= cantidadFechaValidacionMonedaIPC)
                {
                    Parametro moneda = new Parametro();
                    //moneda.Id = "001";
                    moneda = combobox.FirstOrDefault(mon => mon.Id == "001:-1");
                    combobox.Remove(moneda);
                }
            }

            foreach (Parametro item in combobox)
            {
                string codigoMoneda = item.Id.Substring(0, 3);
                if (monedaFondo != "") //Solo para la moneda se envia dato
                {
                    if (monedaFondo == Enums.Moneda.Soles.StringValue())
                    {
                        if (codigoMoneda == Enums.Moneda.Soles.StringValue() || codigoMoneda == Enums.Moneda.SolesAjustados.StringValue())
                        {
                            control.Items.Add(new ListItem(item.Glosa, item.Id));
                        }
                    }
                    else if (monedaFondo == Enums.Moneda.Dolares.StringValue())
                    {
                        if (codigoMoneda == Enums.Moneda.Dolares.StringValue() || codigoMoneda == Enums.Moneda.DolaresAjustados.StringValue())
                        {
                            control.Items.Add(new ListItem(item.Glosa, item.Id));
                        }
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

        private void CargarComboboxconValor(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("100%", "0"));
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }

        }
    }
}