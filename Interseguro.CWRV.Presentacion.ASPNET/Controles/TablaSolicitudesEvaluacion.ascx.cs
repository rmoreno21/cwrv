using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesEvaluacion : System.Web.UI.UserControl
    {

        public List<SolicitudRPPlus> Solicitudes { get; set; }
        private int AnchoColumnaIconos;

        protected void Page_Load(object sender, EventArgs e)
        {
            AnchoColumnaIconos = 33;

            TabSolicitudes_RPP_Evaluacion.DataSource = Solicitudes;
            TabSolicitudes_RPP_Evaluacion.DataBind();

            if (Solicitudes.Count > 0)
            {
                TabSolicitudes_RPP_Evaluacion.HeaderRow.TableSection = TableRowSection.TableHeader;
                switch ((string)HttpContext.Current.Session["RolAzman"])
                {
                    case "JEF.RVI.OPE"://JefeOperaciones
                    case "AST.RVI.OPE"://AsistenteOperaciones
                    case "COR.PLAFT"://CoordinadorPlaft
                        TabSolicitudes_RPP_Evaluacion.Columns[11].Visible = true;
                        break;
                    default:
                        TabSolicitudes_RPP_Evaluacion.Columns[11].Visible = false;
                        break;
                }
            }
        }

        protected void TabSolicitudes_RPP_Evaluacion_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Cells[7].Width = AnchoColumnaIconos;
                //<INI.GTI_7014_S19>
                //Validando la fecha de cotizacion segun rol

                //<FIN.GTI_7014_S19>
            }
        }

    }
}