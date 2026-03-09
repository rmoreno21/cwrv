using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
using System.Configuration;
using System.Web.Configuration;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using log4net;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{

    public partial class TablaCotizacionesPlan3IFP : System.Web.UI.UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TablaCotizacionesPlan3IFP));
        public List<CotizacionIFP> CotizacionesIFP { get; set; }
        public List<CoberturaAdicional> CoberturasAdicionales { get; set; }

        private List<Parametro> lstSexo;
        public bool PermisoAgregar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoEliminar { get; set; }
        public bool PermisoTRA { get; set; }
        public string Moneda { get; set; }
        public int Temporalidad { get; set; }
        public bool PermisoEspeciales { get; set; }
        public int CantidadCotizaciones { get; set; }

        public string ModoSolicitud { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            TabCotizaciones_IFP_P3.DataSource = CotizacionesIFP;
            TabCotizaciones_IFP_P3.DataBind();

            if (CotizacionesIFP.Count > 0)
            {
                TabCotizaciones_IFP_P3.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

            if (!PermisoAgregar || (Convert.ToInt32(Session["CantidadSolicitudes"]) + CantidadCotizaciones) >= Convert.ToInt32(ConfigurationManager.AppSettings["MaxCotizacionesIFP"]))
            {
                TabCotizacionesBotonera_IFP.Visible = false;
            }
            //Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            TabCotizaciones_IFP_P3.Columns[6].Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);
            TabCotizaciones_IFP_P3.Columns[11].Visible = PermisoEspeciales;
            TabCotizaciones_IFP_P3.Columns[12].Visible = PermisoEspeciales;
            TabCotizaciones_IFP_P3.Columns[13].Visible = PermisoEspeciales;
            TabCotizaciones_IFP_P3.Columns[14].Visible = PermisoTRA;

            //if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AgenteExterno.StringValue())
            //{
            //    TabCotizaciones_IFP_P3.Columns[6].Visible = PermisoEspeciales;
            //}

            var CotizacionSeleccionada = CotizacionesIFP.FindAll(p => p.IndSeleccionada == "S" && p.EstadoCotizacion == "04");
            if (CotizacionSeleccionada.Count > 0)
            {
                TabCotizaciones_IFP_P3.Columns[14].Visible = false;
                TabCotizacionesBotonera_IFP.Visible = false;
            }

            lstSexo = (List<Parametro>)Session["ComboSexo"];
            //CargarComboboxSeleccione(ddlCASexoConyuge, lstSexo);
            //CargaCoberturasAdicionales();
        }
        /*
        private void CargaCoberturasAdicionales()
        {
            if (CoberturasAdicionales != null)
            {
                var CA_CY = CoberturasAdicionales.FindLast(c => c.Parentesco == Enums.Parentesco.Conyuge.StringValue());
                
                if (CA_CY != null)
                {
                    log.Info(String.Format("Fecha[0] [{0}].", CA_CY.FechaNacimiento));
                    txtCAFecNacConyuge.Text = CA_CY.FechaNacimiento;
                    ddlCASexoConyuge.SelectedIndex = ddlCASexoConyuge.Items.IndexOf(ddlCASexoConyuge.Items.FindByValue(CA_CY.Sexo));
                }
                
                var CA_PA = CoberturasAdicionales.FindLast(c => c.Parentesco == Enums.Parentesco.Padre.StringValue() && c.Sexo == "M");
                if (CA_PA != null)
                {
                    log.Info(String.Format("Fecha[1] [{0}].", CA_PA.FechaNacimiento));
                    txtCAFecNacPadre.Text = CA_PA.FechaNacimiento;
                }

                var CA_MA = CoberturasAdicionales.FindLast(c => c.Parentesco == Enums.Parentesco.Padre.StringValue() && c.Sexo == "F");
                if (CA_MA != null)
                {
                    log.Info(String.Format("Fecha[2] [{0}].", CA_MA.FechaNacimiento));
                    txtCAFecNacMadre.Text = CA_MA.FechaNacimiento;
                }
                
            }
        }
*/

        protected void TabCotizaciones_RB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                //Combobox DCOM
                DropDownList TabCotPjeDCOM;
                TabCotPjeDCOM = (DropDownList)(fila.FindControl("TabCotPjeDCOM"));
                TabCotPjeDCOM.ClearSelection();
                List<RolDcom> lstPjeDCOM = (List<RolDcom>)Session["ComboRangoIFPDCOM"];
                CargarParametros(TabCotPjeDCOM, lstPjeDCOM);
                TabCotPjeDCOM.SelectedIndex = TabCotPjeDCOM.Items.IndexOf(TabCotPjeDCOM.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].ValPjeDCOM.ToString()));

                //Combobox Moneda
                DropDownList TabCotMoneda;
                TabCotMoneda = (DropDownList)(fila.FindControl("TabCotMoneda"));
                TabCotMoneda.ClearSelection();
                List<Parametro> lstMoneda = (List<Parametro>)Session["ComboIFPMoneda"];

                //if (CotizacionesIFP[e.Row.RowIndex].Temporalidad.Anhos.ToString() == CotizacionesIFP[e.Row.RowIndex].ValPerDiferido.ToString())
                //{
                //    lstMoneda = lstMoneda.FindAll(m => (m.Valor_2.Contains(Moneda)) && (m.Valor_1.Contains(Enums.TipoMonedaIFP.Indexado.StringValue())) ||
                //                                            (m.Valor_2.Contains(Moneda)) && (m.Valor_1.Contains(Enums.TipoMonedaIFP.Nominal.StringValue())) ||
                //                                            (m.Valor_2.Contains(Moneda) && (m.Valor_1.Contains(Enums.TipoMonedaIFP.Ajustados.StringValue()))) ||
                //                                            (m.Valor_2.Contains(Moneda)) && (m.Valor_1.Contains("Seleccione"))
                //    );

                //    CargarCombobox(TabCotMoneda, lstMoneda, true);

                //    if (CotizacionesIFP[e.Row.RowIndex].ValMonAju == 0 && CotizacionesIFP[e.Row.RowIndex].Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                //        TabCotMoneda.SelectedIndex = TabCotMoneda.Items.IndexOf(TabCotMoneda.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].Moneda.Id)) + 1;
                //    else
                //        TabCotMoneda.SelectedIndex = TabCotMoneda.Items.IndexOf(TabCotMoneda.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].Moneda.Id));
                //}
                //else
                //{
                lstMoneda = lstMoneda.FindAll(m => (m.Valor_2.Contains(Moneda)));
                CargarCombobox(TabCotMoneda, lstMoneda, true);

                if (CotizacionesIFP[e.Row.RowIndex].ValMonAju == 0 && CotizacionesIFP[e.Row.RowIndex].Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                    TabCotMoneda.SelectedIndex = TabCotMoneda.Items.IndexOf(TabCotMoneda.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].Moneda.Id)) + 1;
                else
                    TabCotMoneda.SelectedIndex = TabCotMoneda.Items.IndexOf(TabCotMoneda.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].Moneda.Id));
                //}

                ////Combobox Temporalidad
                //DropDownList TabCotTemporalidad;
                //TabCotTemporalidad = (DropDownList)(fila.FindControl("TabCotTemporalidad"));
                //TabCotTemporalidad.ClearSelection();
                //List<Parametro> lstTemporalidad = (List<Parametro>)Session["ComboIFPTemporalidad"];
                //lstTemporalidad = lstTemporalidad.Where(t => t.Valor_2.Contains(Enums.Planes.PLAN1.StringValue())).ToList();
                //CargarCombobox(TabCotTemporalidad, lstTemporalidad, true);
                //TabCotTemporalidad.SelectedIndex = TabCotTemporalidad.Items.IndexOf(TabCotTemporalidad.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].Temporalidad.Id.ToString()));

                //Combobox Ingreso Garantizado
                DropDownList TabCotPeriodoGarantizado;
                TabCotPeriodoGarantizado = (DropDownList)(fila.FindControl("TabCotPeriodoGarantizado"));
                TabCotPeriodoGarantizado.ClearSelection();
                List<Parametro> lstPeriodoGarantizado = (List<Parametro>)Session["ComboIFPPeriodoGarantizado"];
                lstPeriodoGarantizado = lstPeriodoGarantizado.Where(t => t.Valor_2.Contains(Enums.Planes.PLAN3.StringValue())).ToList();
                CargarCombobox(TabCotPeriodoGarantizado, lstPeriodoGarantizado);
                TabCotPeriodoGarantizado.SelectedIndex = TabCotPeriodoGarantizado.Items.IndexOf(TabCotPeriodoGarantizado.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].PeriodoGarantizado.ToString()));

                //Combobox Periodo Diferido
                DropDownList TabCotValPerDiferido;
                TabCotValPerDiferido = (DropDownList)(fila.FindControl("TabCotPeriodDiferido"));
                TabCotValPerDiferido.ClearSelection();
                List<Parametro> lstPerDiferido = (List<Parametro>)Session["ComboIFPDiferimiento"];
                if (Convert.ToInt32(TabCotPeriodoGarantizado.SelectedItem.Text) == 0
                        || Convert.ToInt32(TabCotPeriodoGarantizado.SelectedItem.Text) == 15
                        || Convert.ToInt32(TabCotPeriodoGarantizado.SelectedItem.Text) == 20
                        || Convert.ToInt32(TabCotPeriodoGarantizado.SelectedItem.Text) == 25)
                {
                    lstPerDiferido = lstPerDiferido.FindAll(pd => Convert.ToDouble(pd.Nombre) <= 10);
                }
                else
                {
                    lstPerDiferido = lstPerDiferido.FindAll(pd => Convert.ToDecimal(pd.Nombre) <= Convert.ToInt32(TabCotPeriodoGarantizado.SelectedItem.Text) - 1);
                }

                if (lstPerDiferido.Count == 0)
                    lstPerDiferido.Add(new Parametro { Id = "0", Nombre = "0" });

                CargarParametros(TabCotValPerDiferido, lstPerDiferido);
                TabCotValPerDiferido.SelectedIndex = TabCotValPerDiferido.Items.IndexOf(TabCotValPerDiferido.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].ValPerDiferido.ToString()));

                //Combobox Pago Doble
                DropDownList TabCotPagoEscalonada;
                TabCotPagoEscalonada = (DropDownList)(fila.FindControl("TabCotPagoEscalonada"));
                TabCotPagoEscalonada.ClearSelection();
                List<Parametro> lstPagoEscalonada = (List<Parametro>)Session["ComboIFPPagoDoble"];

                //if (TabCotTemporalidad.SelectedItem.Text == "0")
                //{
                //lstPagoEscalonada = lstPagoEscalonada.FindAll(pd => pd.Id == "0");
                //}
                //else
                //{
                //lstPagoEscalonada = lstPagoEscalonada.FindAll(pd => Convert.ToDouble(pd.Id) <= 20);
                if (Convert.ToInt32(TabCotPeriodoGarantizado.SelectedItem.Text) == 0)
                {
                    lstPagoEscalonada = lstPagoEscalonada.FindAll(pd => Convert.ToDouble(pd.Id) <= 20);
                }
                else
                {
                    lstPagoEscalonada = lstPagoEscalonada.FindAll(pd => Convert.ToDouble(pd.Id) + Convert.ToDouble(TabCotValPerDiferido.SelectedItem.Text) < Convert.ToDouble(TabCotPeriodoGarantizado.SelectedItem.Text));
                }

                if (lstPagoEscalonada.Count == 0)
                    lstPagoEscalonada.Add(new Parametro { Id = "0", Nombre = "0" });
                //}

                CargarParametros(TabCotPagoEscalonada, lstPagoEscalonada);
                TabCotPagoEscalonada.SelectedIndex = TabCotPagoEscalonada.Items.IndexOf(TabCotPagoEscalonada.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].PagoDoble.ToString()));

                //Combobox Tramo Escalonada
                DropDownList TabCotTramoEscalonada;
                TabCotTramoEscalonada = (DropDownList)(fila.FindControl("TabCotTramoEscalonada"));
                TabCotTramoEscalonada.ClearSelection();
                List<Parametro> lstTramoEscalonada = (List<Parametro>)Session["ComboTramoEscalonada"];
                CargarComboPjeRentaEscalonada(TabCotTramoEscalonada, lstTramoEscalonada, (int)CotizacionesIFP[e.Row.RowIndex].PagoDoble);

                var ValorEscalon = "0";
                if (CotizacionesIFP[e.Row.RowIndex].PjePagoDoble > 0)
                {
                    ValorEscalon = lstTramoEscalonada.ToList().Find(esc => (((Convert.ToDouble(esc.Valor_1) - 0.000000001) * 100) <= (CotizacionesIFP[e.Row.RowIndex].PjePagoDoble)) && (((Convert.ToDouble(esc.Valor_1) + 0.000000001) * 100) >= (CotizacionesIFP[e.Row.RowIndex].PjePagoDoble))).Id;
                }

                TabCotTramoEscalonada.SelectedIndex = TabCotTramoEscalonada.Items.IndexOf(TabCotTramoEscalonada.Items.FindByValue(ValorEscalon));

                if (CotizacionesIFP[e.Row.RowIndex].PagoDoble.ToString() == "0")
                {
                    TabCotTramoEscalonada.Enabled = false;
                }
                //Combobox CA Cy
                /*
                DropDownList TabCotPjeCACy;
                TabCotPjeCACy = (DropDownList)(fila.FindControl("TabCotPjeCACy"));
                TabCotPjeCACy.ClearSelection();
                List<Parametro> lstPjeCACy = (List<Parametro>)Session["ComboIFPPorcentajeCA"];
                CargarCombobox(TabCotPjeCACy, lstPjeCACy);
                TabCotPjeCACy.SelectedIndex = TabCotPjeCACy.Items.IndexOf(TabCotPjeCACy.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].ValPjeCACy.ToString()));
                */

                //Combobox CA Pa
                /*
                DropDownList TabCotPjeCAPa;
                TabCotPjeCAPa = (DropDownList)(fila.FindControl("TabCotPjeCAPa"));
                TabCotPjeCAPa.ClearSelection();
                List<Parametro> lstPjeCAPa = (List<Parametro>)Session["ComboIFPPorcentajeCA"];
                CargarCombobox(TabCotPjeCAPa, lstPjeCAPa);
                TabCotPjeCAPa.SelectedIndex = TabCotPjeCAPa.Items.IndexOf(TabCotPjeCAPa.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].ValPjeCAPa.ToString()));
                */

                //Combobox CA Ma
                /*
                DropDownList TabCotPjeCAMa;
                TabCotPjeCAMa = (DropDownList)(fila.FindControl("TabCotPjeCAMa"));
                TabCotPjeCAMa.ClearSelection();
                List<Parametro> lstPjeCAMa = (List<Parametro>)Session["ComboIFPPorcentajeCA"];
                CargarCombobox(TabCotPjeCAMa, lstPjeCAMa);
                TabCotPjeCAMa.SelectedIndex = TabCotPjeCAMa.Items.IndexOf(TabCotPjeCAMa.Items.FindByValue(CotizacionesIFP[e.Row.RowIndex].ValPjeCAMa.ToString()));
                */

                TextBox TabCotTRA;
                TabCotTRA = (TextBox)(fila.FindControl("TabCotTRA"));
                TabCotTRA.Text = CotizacionesIFP[e.Row.RowIndex].AjusteTRA.ToString();

                if (CotizacionesIFP[e.Row.RowIndex].IndCotiza == "**" || CotizacionesIFP[e.Row.RowIndex].IndCotiza == "***")
                {
                    e.Row.CssClass = "grilla_error_tra";
                }

                if (CotizacionesIFP[e.Row.RowIndex].EstadoCotizacion == "04" && CotizacionesIFP[e.Row.RowIndex].IndSeleccionada == "S")
                {
                    e.Row.CssClass = "grilla_active_green";
                }

                if (ModoSolicitud.Equals("") || ModoSolicitud == null)
                {
                    ModoSolicitud = HttpContext.Current.Session["ModSolModo"].ToString();
                }

                if (ModoSolicitud != null && ModoSolicitud.ToString() == "CONS")
                {
                    TabCotPjeDCOM.Enabled = false;
                    TabCotMoneda.Enabled = false;
                    TabCotPagoEscalonada.Enabled = false;
                    TabCotTramoEscalonada.Enabled = false;
                    TabCotValPerDiferido.Enabled = false;
                    TabCotTRA.Enabled = false;
                    TabCotizaciones_IFP_P3.Columns[14].Visible = false;
                    TabCotizacionesBotonera_IFP.Visible = false;
                    TabCotizaciones_IFP_P3.Enabled = false;
                    //txtCAFecNacConyuge.Attributes.Add("disabled", "disabled");
                    //ddlCASexoConyuge.Attributes.Add("disabled", "disabled");
                    //txtCAFecNacPadre.Attributes.Add("disabled", "disabled");
                    //txtCAFecNacMadre.Attributes.Add("disabled", "disabled");
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
                else
                {
                    control.Items.Add(new ListItem(item.Glosa, item.Id));
                }
            }
        }

        private void CargarParametros(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();

            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }

        private void CargarParametros(DropDownList control, List<RolDcom> combobox)
        {
            control.Items.Clear();

            foreach (RolDcom item in combobox)
            {
                control.Items.Add(new ListItem(item.ValorDcom.ToString() + "%", item.ValorDcom.ToString()));
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();

            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox, bool temporalidad)
        {
            control.Items.Clear();

            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Valor_1, item.Id));
            }
        }

        private void CargarComboboxSeleccione(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();

            control.Items.Add(new ListItem("«Seleccione»", "0"));

            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

        //private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        //{
        //    control.Items.Clear();

        //    control.Items.Add(new ListItem("«Seleccione»", "0"));

        //    foreach (Parametro item in combobox)
        //    {
        //        control.Items.Add(new ListItem(item.Glosa, item.Id));
        //    }
        //}

        //private void CargarComboboxAj(DropDownList control, List<Parametro> combobox)
        //{
        //    control.Items.Clear();

        //    foreach (Parametro item in combobox)
        //    {
        //        control.Items.Add(new ListItem(item.Valor_1, item.Id));
        //    }
        //}

        private void CargarComboPjeRentaEscalonada(DropDownList control, List<Parametro> combobox, int aniosEscalonado)
        {
            control.Items.Clear();

            if (aniosEscalonado == 0)
            {
                control.Items.Add(new ListItem("100%", "0"));
            }

            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

    }

}