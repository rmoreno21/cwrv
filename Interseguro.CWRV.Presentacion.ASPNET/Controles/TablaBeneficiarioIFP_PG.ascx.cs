using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaBeneficiarioIFP_PG : System.Web.UI.UserControl
    {

        public List<GrupoFamiliar> lstBeneficiarios { get; set; }
        public List<Parametro> lstParametroSexo { get; set; }
        private Int32 val_sumaPorc = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                TabBeneficiarios_IFP.DataSource = lstBeneficiarios;
                TabBeneficiarios_IFP.DataBind();

                if (lstBeneficiarios.Count > 0)
                {
                    TabBeneficiarios_IFP.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void TabBeneficiarios_IFP_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                TextBox ItemPorcentajeRenta;
                ItemPorcentajeRenta = (TextBox)(fila.FindControl("ItemPorcentajeRenta"));
                ItemPorcentajeRenta.Text = lstBeneficiarios[e.Row.RowIndex].ValPjeRenta.ToString();

                val_sumaPorc += Convert.ToInt32(ItemPorcentajeRenta.Text);
                ModGruSumaBenef_IFP.Text = val_sumaPorc.ToString();

                if (Convert.ToInt32(ItemPorcentajeRenta.Text) == 0)
                    ModGruSumaBenef_IFP.CssClass = "formTextbox formTextboxError";

                if (Convert.ToInt32(ModGruSumaBenef_IFP.Text) != 100)
                    ModGruSumaBenef_IFP.CssClass = "formTextbox formTextboxError";
                else
                    ModGruSumaBenef_IFP.CssClass = "formTextbox formTextboxReadOnly";

                Label ItemSexo;
                ItemSexo = (Label)(fila.FindControl("ItemSexo"));
                ItemSexo.Text = lstParametroSexo.Find(s => s.Id == lstBeneficiarios[e.Row.RowIndex].Sexo.ToString()).Glosa;

                CheckBox seleccion = (CheckBox)(fila.FindControl("seleccion"));
                seleccion.Checked = lstBeneficiarios[e.Row.RowIndex].Ind_Cierre;

            }

        }
    }
}