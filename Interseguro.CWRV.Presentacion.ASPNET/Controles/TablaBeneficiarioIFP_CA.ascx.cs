using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaBeneficiarioIFP_CA : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> lstBeneficiarios { get; set; }
        public List<Parametro> lstparametroSexo { get; set; }
        public List<Parametro> lstparametroParentesco { get; set; }
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

                //Label ItemPorcentajeRenta;
                //ItemPorcentajeRenta = (Label)(fila.FindControl("ItemPorcentajeRenta"));
                //ItemPorcentajeRenta.Text = lstBeneficiarios[e.Row.RowIndex].ValPjeRenta.ToString();

                //val_sumaPorc += Convert.ToInt32(ItemPorcentajeRenta.Text);
                //ModGruSumaBenef_IFP.Text = val_sumaPorc.ToString();

                //if (Convert.ToInt32(ItemPorcentajeRenta.Text) == 0)
                //    ModGruSumaBenef_IFP.CssClass = "formTextbox formTextboxError";

                //if (Convert.ToInt32(ModGruSumaBenef_IFP.Text) != 100)
                //    ModGruSumaBenef_IFP.CssClass = "formTextbox formTextboxError";
                //else
                //    ModGruSumaBenef_IFP.CssClass = "formTextbox formTextboxReadOnly";

                Label ItemNacimiento;
                ItemNacimiento = (Label)(fila.FindControl("ItemNacimiento"));
                ItemNacimiento.Text = lstBeneficiarios[e.Row.RowIndex].FechaNacimiento.Value.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("es-PE"));

                Label ItemSexo;
                ItemSexo = (Label)(fila.FindControl("ItemSexo"));
                ItemSexo.Text = lstparametroSexo.Find(s => s.Id == lstBeneficiarios[e.Row.RowIndex].Sexo.ToString()).Glosa;

                Label ItemParentesco;
                ItemParentesco = (Label)(fila.FindControl("ItemParentesco"));
                ItemParentesco.Text = lstparametroParentesco.Find(p => p.Id == lstBeneficiarios[e.Row.RowIndex].Parentesco.Id.ToString()).Glosa;
            }
        }
    }
}