using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaParametrosRentas : UserControl
    {
        public List<ValPar> Parametros { get; set; }
        public string Producto { get; set; }
        public List<Parametro> Monedas;
        public List<Parametro> Temporalidades;
        public List<Parametro> TiposPension;
        public List<Parametro> Departamentos;
        public List<Parametro> Origenes;

        protected void Page_Load(object sender, EventArgs e)
        {
            TablaParametros.DataSource = Parametros;
            TablaParametros.DataBind();
            TablaParametrosEditar.DataSource = Parametros;
            TablaParametrosEditar.DataBind();
            if (Parametros != null && Parametros.Count > 0)
            {
                TablaParametros.HeaderRow.TableSection = TableRowSection.TableHeader;
                TablaParametrosEditar.HeaderRow.TableSection = TableRowSection.TableHeader;

                switch (Producto)
                {
                    case "RPP":
                    case "IFP":
                        TablaParametros.Columns[7].Visible = false;
                        TablaParametros.Columns[8].Visible = false;
                        TablaParametrosEditar.Columns[7].Visible = false;
                        TablaParametrosEditar.Columns[8].Visible = false;
                        break;
                    case "RVI":
                        TablaParametros.Columns[4].Visible = false;
                        TablaParametros.Columns[9].Visible = false;
                        TablaParametrosEditar.Columns[4].Visible = false;
                        TablaParametrosEditar.Columns[9].Visible = false;
                        break;
                }
            }
        }

        protected void TablaParametros_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                switch (Producto)
                {
                    case "RPP":
                    case "IFP":
                        if (e.Row.Cells[4].Text.Trim() == "-")
                        {
                            e.Row.Cells[4].Text = "Todas";
                        }
                        else
                        {
                            e.Row.Cells[4].Text = Temporalidades.Find(t => t.Id == e.Row.Cells[4].Text).Glosa;
                        }
                        if (e.Row.Cells[9].Text.Trim() == "-")
                        {
                            e.Row.Cells[9].Text = "Todos";
                        }
                        else
                        {
                            e.Row.Cells[9].Text = Origenes.Find(o => o.Id == e.Row.Cells[9].Text).Glosa;
                        }
                        break;
                    case "RVI":
                        if (e.Row.Cells[7].Text.Trim() == "-")
                        {
                            e.Row.Cells[7].Text = "Todos";
                        }
                        else
                        {
                            e.Row.Cells[7].Text = TiposPension.Find(tp => tp.Id == e.Row.Cells[7].Text).Glosa;
                        }
                        if (e.Row.Cells[8].Text.Trim() == "-")
                        {
                            e.Row.Cells[8].Text = "Todos";
                        }
                        else
                        {
                            e.Row.Cells[8].Text = Departamentos.Find(d => d.Id == e.Row.Cells[8].Text).Glosa;
                        }
                        break;
                }
                // Moneda
                e.Row.Cells[3].Text = Monedas.Find(m => m.Id == e.Row.Cells[3].Text).Glosa;
            }
        }

        protected void TablaParametrosEditar_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)e.Row.Cells[1].NamingContainer;
                switch (Producto)
                {
                    case "RPP":
                    case "IFP":
                        // Combobox Temporalidad
                        DropDownList TabTemporalidad;
                        TabTemporalidad = (DropDownList)fila.FindControl("TabTemporalidad");
                        TabTemporalidad.ClearSelection();
                        CargarCombobox(TabTemporalidad, Temporalidades, true, "Todas");
                        TabTemporalidad.SelectedIndex = TabTemporalidad.Items.IndexOf(TabTemporalidad.Items.FindByValue(Parametros[e.Row.RowIndex].cod_tipo_temporalidad));

                        // Combobox Origen
                        DropDownList TabOrigen;
                        TabOrigen = (DropDownList)fila.FindControl("TabOrigen");
                        TabOrigen.ClearSelection();
                        CargarCombobox(TabOrigen, Origenes, true, "Todos");
                        TabOrigen.SelectedIndex = TabOrigen.Items.IndexOf(TabOrigen.Items.FindByValue(Parametros[e.Row.RowIndex].ind_origen));
                        break;
                    case "RVI":
                        // Combobox Tipo Pensión
                        DropDownList TabTipoPension;
                        TabTipoPension = (DropDownList)fila.FindControl("TabTipoPension");
                        TabTipoPension.ClearSelection();
                        CargarCombobox(TabTipoPension, TiposPension, true, "Todos");
                        TabTipoPension.SelectedIndex = TabTipoPension.Items.IndexOf(TabTipoPension.Items.FindByValue(Parametros[e.Row.RowIndex].cod_tipo_pension));

                        // Combobox Departamento
                        DropDownList TabDepartamento;
                        TabDepartamento = (DropDownList)fila.FindControl("TabDepartamento");
                        TabDepartamento.ClearSelection();
                        CargarCombobox(TabDepartamento, Departamentos, true, "Todos");
                        TabDepartamento.SelectedIndex = TabDepartamento.Items.IndexOf(TabDepartamento.Items.FindByValue(Parametros[e.Row.RowIndex].cod_departamento));
                        break;
                }

                // Inicio Vigencia
                TextBox TabInicioVigencia;
                TabInicioVigencia = (TextBox)fila.FindControl("TabInicioVigencia");
                TabInicioVigencia.Text = Parametros[e.Row.RowIndex].fec_ini_rango.ToString("dd/MM/yyyy");

                // Fin Vigencia
                TextBox TabFinVigencia;
                TabFinVigencia = (TextBox)fila.FindControl("TabFinVigencia");
                TabFinVigencia.Text = Parametros[e.Row.RowIndex].fec_fin_rango.ToString("dd/MM/yyyy");

                // Combobox Moneda
                DropDownList TabMoneda;
                TabMoneda = (DropDownList)fila.FindControl("TabMoneda");
                TabMoneda.ClearSelection();
                CargarCombobox(TabMoneda, Monedas, false);
                TabMoneda.SelectedIndex = TabMoneda.Items.IndexOf(TabMoneda.Items.FindByValue(Parametros[e.Row.RowIndex].cod_moneda));

                // Tramo
                TextBox TabTramo;
                TabTramo = (TextBox)fila.FindControl("TabTramo");
                TabTramo.Text = Parametros[e.Row.RowIndex].num_tramo.ToString();

                // Valor
                TextBox TabValor;
                TabValor = (TextBox)fila.FindControl("TabValor");
                TabValor.Text = Parametros[e.Row.RowIndex].val_parametro.ToString();
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox, bool seleccione, string glosa = "")
        {
            control.Items.Clear();
            if (seleccione)
            {
                control.Items.Add(new ListItem(glosa, "-"));
            }
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }
    }
}