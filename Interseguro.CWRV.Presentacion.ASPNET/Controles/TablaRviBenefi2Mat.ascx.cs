using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaRviBenefi2Mat : UserControl
    {
        public List<Beneficiario> Beneficiarios { get; set; }
        public List<GrupoFamiliar> BeneficiariosGF {  get; set; }
        public bool GFNoEncontrado { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            // Reset the flag before binding
            GFNoEncontrado = false;
            
            TabRviBenefi.DataSource = Beneficiarios;
            TabRviBenefi.DataBind();
            if (Beneficiarios.Count > 0)
            {
                TabRviBenefi.HeaderRow.TableSection = TableRowSection.TableHeader;
            }

            // Show disclaimer if there are red rows
            AdvertenciaBeneficiarios.Visible = GFNoEncontrado;
        }
        
        protected void TabRviBenefi_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Beneficiario beneficiario = (Beneficiario)e.Row.DataItem;
                
                if (BeneficiariosGF != null && BeneficiariosGF.Count > 0)
                {
                    var beneficiarioGF = BeneficiariosGF.Find(b =>
                        b.Parentesco.Id == beneficiario.Parentesco.Id &
                        b.Sexo == beneficiario.Sexo &
                        b.FechaNacimiento == beneficiario.FechaNacimiento
                    );

                    if (beneficiarioGF == null)
                    {
                        GFNoEncontrado = true;
                        
                        e.Row.BackColor = System.Drawing.ColorTranslator.FromHtml("#ffebee");
                        e.Row.ForeColor = System.Drawing.ColorTranslator.FromHtml("#c62828");
                        
                        HyperLink nombreBeneficiario = (HyperLink)e.Row.FindControl("NombreBeneficiario");
                        if (nombreBeneficiario != null)
                        {
                            nombreBeneficiario.NavigateUrl = "";
                            nombreBeneficiario.Enabled = false;
                            nombreBeneficiario.ForeColor = System.Drawing.ColorTranslator.FromHtml("#c62828");
                            nombreBeneficiario.CssClass = "disabled-beneficiary-link";
                            nombreBeneficiario.Style.Add("text-decoration", "none");
                            nombreBeneficiario.Style.Add("cursor", "default");
                        }
                    }
                }
            }
        }
    }
}