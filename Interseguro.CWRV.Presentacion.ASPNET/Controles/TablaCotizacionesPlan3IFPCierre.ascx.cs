using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using log4net;
using System.Linq;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaCotizacionesPlan3IFPCierre : System.Web.UI.UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TablaCotizacionesPlan3IFP));
        public List<CotizacionIFP> CotizacionesIFP { get; set; }
        public List<CoberturaAdicional> CoberturasAdicionales { get; set; }
        //private List<Parametro> lstSexo;
        public bool PermisoAgregar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoEliminar { get; set; }
        public bool PermisoTRA { get; set; }
        public string Moneda { get; set; }
        public int Temporalidad { get; set; }

        public bool PermisoEspeciales { get; set; }

        
        private static IServicioCWRV servicioCotizador;


        //public bool Conyuge { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            TabCotizaciones_IFP_P3.Columns[7].Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);
            TabCotizaciones_IFP_P3.Columns[11].Visible = PermisoEspeciales;
            TabCotizaciones_IFP_P3.Columns[12].Visible = PermisoEspeciales;
            TabCotizaciones_IFP_P3.Columns[13].Visible = PermisoEspeciales;
            TabCotizaciones_IFP_P3.Columns[14].Visible = PermisoTRA;

            //TabCotizaciones_IFP_P3.Columns[8].Visible = Conyuge;

            //Moneda
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaParametro = servicioCotizador.ObtenerComboboxIFP();
            //Moneda
            List<Parametro> lstMoneda = listaParametro[(int)Enums.ComboboxIFP.Moneda];
            lstMoneda = lstMoneda.FindAll(m => (m.Valor_2.Contains(Moneda)));

            List<Parametro> lstTramoEscalonada = listaParametro[(int)Enums.ComboboxIFP.PorcentajeEscalon];
            lstTramoEscalonada = lstTramoEscalonada.ToList();

            foreach (var item in CotizacionesIFP)
            {
                if (item.Moneda.Id == Enums.Moneda.SolesAjustados.StringValue())
                {
                    if (item.ValMonAju == 0)
                    {
                        item.Moneda.Nombre = "Nominal";
                    }

                    if (item.ValMonAju == 2)
                    {
                        item.Moneda.Nombre = "Ajustados";
                    }
                }
                else
                {
                    if (item.Moneda.Id == Enums.Moneda.DolaresAjustados.StringValue())
                    {
                        if (item.ValMonAju == 2)
                        {
                            item.Moneda.Nombre = "Ajustados";
                        }
                    }
                }

                //Combobox Tramo Escalonada    
                var ValorEscalon = "0";
                if (item.PjePagoDoble > 0)
                {
                    ValorEscalon = lstTramoEscalonada.ToList().Find(esc => (((Convert.ToDouble(esc.Valor_1) - 0.000000001) * 100) <= (item.PjePagoDoble)) && (((Convert.ToDouble(esc.Valor_1) + 0.000000001) * 100) >= (item.PjePagoDoble))).Id;
                }
                else
                {
                    ValorEscalon = "100";
                }

                item.PjePagoDoble = Convert.ToDouble(ValorEscalon);
            }

            TabCotizaciones_IFP_P3.DataSource = CotizacionesIFP;
            TabCotizaciones_IFP_P3.DataBind();

            if (CotizacionesIFP.Count > 0)
            {
                TabCotizaciones_IFP_P3.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

            //lstSexo = (List<Parametro>)Session["ComboSexo"];
            //CargaCoberturasAdicionales();
        }

        //private void CargaCoberturasAdicionales()
        //{
        //    if (CoberturasAdicionales != null)
        //    {
        //        var CA_CY = CoberturasAdicionales.FindLast(c => c.Parentesco == Enums.Parentesco.Conyuge.StringValue());
        //        if (CA_CY != null)
        //        {
        //            log.Info(String.Format("Fecha[0] [{0}].", CA_CY.FechaNacimiento));
        //            lblCAFecNacConyuge.Text = CA_CY.FechaNacimiento;
        //            lblCASexoConyuge.Text = lstSexo.Find(s => s.Id == CA_CY.Sexo).Glosa;
        //            //ddlCASexoConyuge.SelectedIndex = ddlCASexoConyuge.Items.IndexOf(ddlCASexoConyuge.Items.FindByValue(CA_CY.Sexo));
        //        }

        //        var CA_PA = CoberturasAdicionales.FindLast(c => c.Parentesco == Enums.Parentesco.Padre.StringValue() && c.Sexo == "M");
        //        if (CA_PA != null)
        //        {
        //            log.Info(String.Format("Fecha[1] [{0}].", CA_PA.FechaNacimiento));
        //            lblCAFecNacPadre.Text = CA_PA.FechaNacimiento;
        //        }

        //        var CA_MA = CoberturasAdicionales.FindLast(c => c.Parentesco == Enums.Parentesco.Padre.StringValue() && c.Sexo == "F");
        //        if (CA_MA != null)
        //        {
        //            log.Info(String.Format("Fecha[2] [{0}].", CA_MA.FechaNacimiento));
        //            lblCAFecNacMadre.Text = CA_MA.FechaNacimiento;
        //        }
        //    }
        //}

        protected void TabCotizaciones_RB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                if (CotizacionesIFP[e.Row.RowIndex].IndCotiza == "**" || CotizacionesIFP[e.Row.RowIndex].IndCotiza == "***")
                {
                    e.Row.CssClass = "grilla_error_tra";
                }

                if (CotizacionesIFP[e.Row.RowIndex].EstadoCotizacion == "04" && CotizacionesIFP[e.Row.RowIndex].IndSeleccionada == "S")
                {
                    e.Row.CssClass = "grilla_active_green";
                }
                
            }
        }
    }
}