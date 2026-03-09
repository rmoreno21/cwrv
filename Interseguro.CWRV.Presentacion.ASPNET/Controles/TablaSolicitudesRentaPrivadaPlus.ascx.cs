using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesRentaPrivadaPlus : System.Web.UI.UserControl
    {
        public List<SolicitudRPPlus> Solicitudes { get; set; }

        public bool PermisoInsertar { get; set; }
        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoCorreoElectronico { get; set; }
        public bool PermisoExportarPDF { get; set; }
        //<SRIINI06326>
        public bool PermisoReporteEscenario { get; set; }
        public bool Consentimiento { get; set; }
        //public bool RedLocal { get; set; }
        private int AnchoColumnaIconos;
        //<SRIFIN06326>

        //<INIGTI_7012>
        public bool PermisoCerrar { get; set; }
        //<FINGTI_7012>

        public bool ExistePoliza { get; set; }

        public bool PermisoObtenerEdN { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                //<INIGTI_753_3>
                AnchoColumnaIconos = 66;//64
                if (PermisoConsultar) AnchoColumnaIconos += 33;
                if (Consentimiento) AnchoColumnaIconos += 33;
                if (PermisoModificar) AnchoColumnaIconos += 33;
                //if (RedLocal) AnchoColumnaIconos += 32;
                if (ExistePoliza) AnchoColumnaIconos += 32;

                //<INIGTI_7012>

                if (PermisoCerrar) AnchoColumnaIconos += 33;

                //<FINGTI_7012>

                //<FINGTI_753_3>

                TabSolicitudes_RP.DataSource = Solicitudes;
                TabSolicitudes_RP.DataBind();
                if (Solicitudes.Count > 0)
                {
                    TabSolicitudes_RP.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }
        protected void TabSolicitudes_RP_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //<SRI.INI-20322>
                //e.Row.Cells[8].Width = AnchoColumnaIconos;
                e.Row.Cells[7].Width = AnchoColumnaIconos;
                //<SRI.FIN-20322>
            }
        }

        //<INIGTI_7012>
        public string ConfigurarBotonCerrar(string codEstado, string glsEstado)
        {

            string gls_html = string.Empty;

            if (PermisoCerrar)
            {
                gls_html = "<a class=\"grilla_boton";

                if (codEstado == "0")
                {
                    gls_html += " grilla_cerrar\" data-estado=\"" + Eval("CodigoEstado") + "\" data-solicitud=\"" + Eval("Id") + "\" title=\"Cerrar Solicitud";
                }
                else
                {
                    gls_html += " grilla_cerrar_seleccionado\" data-estado=\"" + Eval("CodigoEstado") + "\" data-solicitud=\"" + Eval("Id") + "\" title=\"" + glsEstado + "";
                }

                //+ "data-estadoPlaft=\"" + Eval("CodigoEstadoPlaft")

                //else if (codEstado == "1") 
                //    {
                //        gls_html += " grilla_cerrar_seleccionado\" data-estado=\"" + Eval("CodigoEstado") + "\" data-solicitud=\"" + Eval("Id") + "\" title=\"" + glsEstado + "";                        
                //    }
                //else if (codEstado == "2")
                //    {
                //        gls_html += " grilla_cerrar_seleccionado\" data-estado=\"" + Eval("CodigoEstado") + "\" data-solicitud=\"" + Eval("Id") + "\" title=\"" + glsEstado + "";
                //    }
                //else if (codEstado == "3")
                //    {
                //        gls_html += " grilla_cerrar_seleccionado\" data-estado=\"" + Eval("CodigoEstado") + "\" data-solicitud=\"" + Eval("Id") + "\" title=\"" + glsEstado + "";
                //    }

                gls_html += "\"></a>";

            }
            else
            {
                gls_html = string.Empty;
            }

            return gls_html;
        }
        //<FINGTI_7012>

    }
}