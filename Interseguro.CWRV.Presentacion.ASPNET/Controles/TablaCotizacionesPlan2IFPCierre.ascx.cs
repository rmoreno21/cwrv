using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{

    public partial class TablaCotizacionesPlan2IFPCierre : System.Web.UI.UserControl
    {
        public List<CotizacionIFP> CotizacionesIFP { get; set; }
        public bool PermisoAgregar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoEliminar { get; set; }
        public bool PermisoTRA { get; set; }
        public string Moneda { get; set; }
        public int Temporalidad { get; set; }

        public bool PermisoEspeciales { get; set; }

        public bool Conyuge { get; set; }

        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            TabCotizaciones_IFP_P2.Columns[8].Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);

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

            TabCotizaciones_IFP_P2.DataSource = CotizacionesIFP;
            TabCotizaciones_IFP_P2.DataBind();

            if (CotizacionesIFP.Count > 0)
            {
                TabCotizaciones_IFP_P2.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

            TabCotizaciones_IFP_P2.Columns[12].Visible = PermisoEspeciales;
            TabCotizaciones_IFP_P2.Columns[13].Visible = PermisoEspeciales;
            TabCotizaciones_IFP_P2.Columns[14].Visible = PermisoEspeciales;
            TabCotizaciones_IFP_P2.Columns[15].Visible = PermisoTRA;
        }

        protected void TabCotizaciones_RB_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                Label TabCotRescate = (Label)(fila.FindControl("TabCotRescate"));
                TabCotRescate.Text = CotizacionesIFP[e.Row.RowIndex].IndRescate ? "Sí" : "No";
                //TabCotRescate.Checked = CotizacionesIFP[e.Row.RowIndex].IndRescate;

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
