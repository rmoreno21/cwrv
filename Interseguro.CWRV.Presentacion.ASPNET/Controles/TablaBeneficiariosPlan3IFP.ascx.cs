using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaBeneficiariosPlan3IFP : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> Beneficiarios { get; set; }
        private Int32 val_sumaPorc = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            TabBeneficiarios_RP.DataSource = Beneficiarios;
            TabBeneficiarios_RP.DataBind();
            if (Beneficiarios.Count > 0)
            {
                TabBeneficiarios_RP.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
        }
        protected void TabBeneficiarios_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GrupoFamiliar beneficiario = (GrupoFamiliar)e.Row.DataItem;
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                e.Row.Cells[1].Text = "<input type=\"checkbox\" name=\"Correlativo\" "
                    + "value=\"" + beneficiario.Id + "\""
                    + ((beneficiario.Parentesco.Id != "80") ? String.Empty : " disabled=\"disabled\"")
                    + ((beneficiario.Seleccionado) ? " checked=\"checked\"" : String.Empty)
                    + " />";

                if (beneficiario.Parentesco.Id == "80")
                {
                    TextBox ItemPorcentajeRentaTitular;
                    ItemPorcentajeRentaTitular = (TextBox)(fila.FindControl("ItemPorcentajeRenta"));

                    ItemPorcentajeRentaTitular.Style.Add("display", "none");
                    ItemPorcentajeRentaTitular.ReadOnly = true;
                }
                else
                {
                    TextBox ItemPorcentajeRenta;

                    ItemPorcentajeRenta = (TextBox)(fila.FindControl("ItemPorcentajeRenta"));
                    ItemPorcentajeRenta.Text = Beneficiarios[e.Row.RowIndex].ValPjeRenta.ToString();

                    val_sumaPorc += Convert.ToInt32(ItemPorcentajeRenta.Text);
                    ModGruSumaBenef_IFP.Text = val_sumaPorc.ToString();
                    
                    if (Convert.ToInt32(ModGruSumaBenef_IFP.Text) > 100) { 
                        ModGruSumaBenef_IFP.CssClass = "formTextboxError";
                    }
                    else { 
                        //ModGruSumaBenef_IFP.CssClass = "formTextboxReadOnly";
                    }

                    if (!beneficiario.Seleccionado && ItemPorcentajeRenta.Text == "0")
                    {
                        ItemPorcentajeRenta.ReadOnly = true;
                    }

                }


            }
        }

    }
}