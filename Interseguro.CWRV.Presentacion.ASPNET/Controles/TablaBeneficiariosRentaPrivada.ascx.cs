using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaBeneficiariosRentaPrivada : System.Web.UI.UserControl
    {
        public List<GrupoFamiliar> Beneficiarios { get; set; }
        public bool Consentimiento { get; set; }

        //<INIGTI_753>
        public bool Conyuge { get; set; }
        //<FINGTI_753>
        //<INIGTI_7012>
        public string modo { get; set; }
        //<FINGTI_7012>

        protected void Page_Load(object sender, EventArgs e)
        {
            //<INIGTI_7012>
            modo= Session["ModSolModo"].ToString();
            
            if (modo != "CERRAR")
            {
                TabBeneficiarios_RP.DataSource = Beneficiarios;
                TabBeneficiarios_RP.DataBind();
                if (Beneficiarios.Count > 0)
                {
                    TabBeneficiarios_RP.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
                if (!Consentimiento)
                {
                    TabBeneficiarios_RP.Columns[3].Visible = false;
                }
                
            }
            //<FINGTI_7012>
            
        }
        protected void TabBeneficiarios_RP_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GrupoFamiliar beneficiario = (GrupoFamiliar)e.Row.DataItem;

                //<INIGTI_753>
                if (!Conyuge)
                {
                    e.Row.Cells[1].Text = "<input type=\"checkbox\" name=\"Correlativo\" data-parentesco=\"" + beneficiario.Parentesco.Id + "\"" //<INIGTI_753>
                   + "value=\"" + beneficiario.Id + "\""
                   //<INIGTI_7012>
                   + ((beneficiario.Parentesco.Id != "80") ? String.Empty : " disabled=\"disabled\"")
                   //+ ((beneficiario.Parentesco.Id != "80") ? (modo=="CERRAR")? " disabled=\"disabled\"": string.Empty : " disabled=\"disabled\"")
                   + ((beneficiario.Seleccionado) ? " checked=\"checked\"" : String.Empty)
                   //<FINGTI_7012>
                   + " />";

                }
                else {
                    switch (beneficiario.Parentesco.Id)
                    {
                        case "10":
                        case "80":
                            e.Row.Cells[1].Text = "<input type=\"checkbox\" name=\"Correlativo\" data-parentesco=\"" + beneficiario.Parentesco.Id + "\"" //<INIGTI_753>
                            + "value=\"" + beneficiario.Id + "\""
                            + ((beneficiario.Parentesco.Id != "80") ? String.Empty : " disabled=\"disabled\"")
                            + ((beneficiario.Seleccionado) ? " checked=\"checked\"" : String.Empty)
                            + " />";
                            break;
                        default:
                            e.Row.Cells[1].Text = "<input type=\"checkbox\" name=\"Correlativo\" data-parentesco=\"" + beneficiario.Parentesco.Id + "\"" //<INIGTI_753>
                            + "value=\"" + beneficiario.Id + "\""
                            + ((beneficiario.Parentesco.Id != "80") ? String.Empty : " disabled=\"disabled\"")
                            + ((beneficiario.Seleccionado) ? " " : String.Empty)
                            + " />";
                            break;
                    }
                }

                //<FINGTI_753>

                if (!Consentimiento)
                {
                    if (e.Row.Cells[5].Text.Length > 3)
                    {
                        e.Row.Cells[5].Text = e.Row.Cells[5].Text.Substring(3, e.Row.Cells[5].Text.Length - 3);
                    }
                }
            }
        }
    }
}