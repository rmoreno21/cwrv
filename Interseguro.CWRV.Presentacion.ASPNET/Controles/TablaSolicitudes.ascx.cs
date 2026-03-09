using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudes : System.Web.UI.UserControl
    {
        public List<Solicitud> Solicitudes { get; set; }

        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoCorreoElectronico { get; set; }
        public bool PermisoExportarPDF { get; set; }
        //<SRIINI06326>
        public bool PermisoReporteEscenario { get; set; }
        public bool Consentimiento { get; set; }
        public bool RedLocal { get; set; }
        private int AnchoColumnaIconos;
        private int totalIconos;
        //<SRIFIN06326>
        public bool TieneLote { get; set; }
        public bool ExistePoliza { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                AnchoColumnaIconos = 66;
                totalIconos = 2; // Editar y Exportar a PDF
                if (Consentimiento) { AnchoColumnaIconos += 32; totalIconos++; } // Enviar por mail
                if (RedLocal) { AnchoColumnaIconos += 32; totalIconos++; } // Reporte de escenarios
                if (TieneLote) { AnchoColumnaIconos += 32; totalIconos++; } // Candado o VCTP
                if (ExistePoliza) { AnchoColumnaIconos += 32; totalIconos++; } // Póliza

                // Setear anchos con 2 filas de íconos
                if (totalIconos > 4 && totalIconos <= 6) { AnchoColumnaIconos = 66 + 32; } // Filas de 3 íconos
                if (totalIconos > 6 && totalIconos <= 8) { AnchoColumnaIconos = 66 + 64; } // Filas de 4 íconos
                if (totalIconos > 8 && totalIconos <= 10) { AnchoColumnaIconos = 66 + 96; } // Filas de 5 íconos

                TabSolicitudes.DataSource = Solicitudes;
                TabSolicitudes.DataBind();
                if (Solicitudes.Count > 0)
                {
                    TabSolicitudes.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }

        protected void TabSolicitudes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //<SRI.INI-20322>
                e.Row.Cells[8].Width = 20;
                e.Row.Cells[10].Width = AnchoColumnaIconos;
                //<SRI.FIN-20322>
            }
        }

        public string ConfigurarBotonCerrar(string TipoCotizacion, string companhia, string firmaDigitalToken, string NumLoteCotizacion)
        {

            string gls_html = string.Empty;

            if (TipoCotizacion == "OFICIAL")
            {
                if (Convert.ToInt32(NumLoteCotizacion) > 0)
                {
                    gls_html = "<a class=\"grilla_boton";

                    if (firmaDigitalToken.Length > 0)
                    {
                        gls_html += " grilla_firma_digital\" data-solicitud=\"" + Eval("Id") + "\" data-firmadigitaltoken=\"" + Eval("firmaDigitalToken") + "\" title=\"Descargar Formato VCTP";
                    }
                    else
                    {
                        gls_html += " grilla_cerrar\" href=\"" + ResolveUrl("~/Cotizador/SeleccionarBeneficiarioCierre.aspx") + "?s=" + Eval("Id") + "&fc=" + Eval("FechaCotizacion", "{0:dd/MM/yyyy}") + "\" \" data-solicitud=\"" + Eval("Id") + "\" title=\"Cerrar Solicitud";
                    }

                    gls_html += "\"></a>";
                }
            }

            return gls_html;
        }

    }
}