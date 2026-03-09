using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaBeneficiarioIFP_VIT : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> lstBeneficiarios { get; set; }
        public List<Parametro> lstParametroSexo { get; set; }
        private Int32 val_sumaPorc = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                TabBeneficiarios_VIT_Cierre.DataSource = lstBeneficiarios;
                TabBeneficiarios_VIT_Cierre.DataBind();

                if (lstBeneficiarios.Count > 0)
                {
                    TabBeneficiarios_VIT_Cierre.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void TabBeneficiarios_VIT_Cierre_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GridViewRow fila = (GridViewRow)(e.Row.Cells[1].NamingContainer);

                Label ItemPorcentajeRenta;
                ItemPorcentajeRenta = (Label)(fila.FindControl("ItemPorcentajeRenta_VIT"));
                ItemPorcentajeRenta.Text = lstBeneficiarios[e.Row.RowIndex].ValPjeRenta.ToString();

                val_sumaPorc += Convert.ToInt32(ItemPorcentajeRenta.Text);
                ModGruSumaBenef_IFP_VIT.Text = val_sumaPorc.ToString();

                if (Convert.ToInt32(ItemPorcentajeRenta.Text) == 0)
                    ModGruSumaBenef_IFP_VIT.CssClass = "formTextbox formTextboxError";

                if (Convert.ToInt32(ModGruSumaBenef_IFP_VIT.Text) > 100)
                    ModGruSumaBenef_IFP_VIT.CssClass = "formTextbox formTextboxError";
                else
                    ModGruSumaBenef_IFP_VIT.CssClass = "formTextbox formTextboxReadOnly";

                Label ItemSexo;
                ItemSexo = (Label)(fila.FindControl("ItemSexo_VIT"));
                ItemSexo.Text = lstParametroSexo.Find(s => s.Id == lstBeneficiarios[e.Row.RowIndex].Sexo.ToString()).Glosa;                
            }

        }


    }
}