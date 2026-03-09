using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesIFP : System.Web.UI.UserControl
    {
        public List<SolicitudIFP> Solicitudes { get; set; }

        public bool PermisoInsertar { get; set; }
        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoCorreoElectronico { get; set; }
        public bool PermisoExportarPDF { get; set; }
        public bool PermisoReporteEscenario { get; set; }
        public bool Consentimiento { get; set; }
        private int AnchoColumnaIconos;
        private int totalIconos;
        public bool PermisoCerrar { get; set; }
        public bool ExistePoliza { get; set; }

        public bool PermisoObtenerEdN { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                //<INIGTI_753_3>
                AnchoColumnaIconos = 66;//66;//64;
                totalIconos = 2;

                //if (Consentimiento) AnchoColumnaIconos += 33;
                //if (RedLocal) AnchoColumnaIconos += 32;

                if (PermisoConsultar) { AnchoColumnaIconos += 33; totalIconos++; }
                if (PermisoInsertar) { AnchoColumnaIconos += 33; totalIconos++; }
                if (ExistePoliza) { AnchoColumnaIconos += 33; totalIconos++; }
                if (PermisoCerrar) { AnchoColumnaIconos += 33; totalIconos++; }

                DateTime fechaActual = DateTime.Today;
                List<SolicitudIFP> solicitudesHoy = Solicitudes;

                if (solicitudesHoy != null)
                {
                    solicitudesHoy = solicitudesHoy.FindAll(p => p.FechaSolicitud == fechaActual);
                }

                if ((string)HttpContext.Current.Session["RolAzman"] == "JEF.RVI.OPE"
                        || (string)HttpContext.Current.Session["RolAzman"] == "AST.RVI.COM"
                        || (string)HttpContext.Current.Session["RolAzman"] == "JEF.VTA.LIM.RVI"
                        || (string)HttpContext.Current.Session["RolAzman"] == "JEF.VTA.PRO.RVI")
                {
                    if (PermisoModificar) { AnchoColumnaIconos += 33; totalIconos++; }
                }
                else
                {
                    if (solicitudesHoy != null)
                    {
                        if (solicitudesHoy.Count > 0)
                        {
                            if (PermisoModificar) { AnchoColumnaIconos += 33; totalIconos++; }
                        }
                    }
                }

                //<FINGTI_753_3>

                // Setear anchos con 2 filas de íconos
                if (totalIconos > 4 && totalIconos <= 6) { AnchoColumnaIconos = 66 + 32; } // Filas de 3 íconos
                if (totalIconos > 6 && totalIconos <= 8) { AnchoColumnaIconos = 66 + 64; } // Filas de 4 íconos
                if (totalIconos > 8 && totalIconos <= 10) { AnchoColumnaIconos = 66 + 96; } // Filas de 5 íconos


                TabSolicitudes_RP.DataSource = Solicitudes;
                TabSolicitudes_RP.DataBind();

                if (Solicitudes != null)
                {
                    if (Solicitudes.Count > 0)
                    {
                        TabSolicitudes_RP.HeaderRow.TableSection = TableRowSection.TableHeader;
                    }
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
                e.Row.Cells[7].Width = AnchoColumnaIconos;
            }
        }

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

                gls_html += "\"></a>";

            }
            else
            {
                gls_html = string.Empty;
            }

            return gls_html;
        }

        public bool ConfigurarBotonCerrar(object fechaSolicitud)
        //public bool ConfigurarBotonCerrar(object fechaSolicitud, object moneda)
        {
            //if (moneda.ToString() == Enums.MonedaIFPSimbolo.Soles.StringValue())
            //{
            //    PermisoModificar = false;
            //}
            //else
            //{
            DateTime fechaActual = DateTime.Today;
            DateTime fechaSolicitudDato = Convert.ToDateTime(fechaSolicitud);

            if (fechaSolicitudDato.ToString("yyyyMMdd") == fechaActual.ToString("yyyyMMdd"))
            {
                //PermisoModificar = false;
                //return true;
            }
            else
            {
                switch ((string)HttpContext.Current.Session["RolAzman"])
                {
                    case "AGT.LIM.RVI"://AgenteLima
                        PermisoModificar = false;
                        break;
                    case "AGT.PRO.RVI"://AgenteProvincia
                        PermisoModificar = false;
                        break;
                    case "SPV.LIM.RVI"://SupervisorLima
                        PermisoModificar = false;
                        break;
                    case "SPV.PRO.RVI"://SupervisorProvincia
                        PermisoModificar = false;
                        break;
                    case "JEF.VTA.LIM.RVI"://JefeVentaLima
                                            //PermisoModificar = false;
                        break;
                    case "JEF.VTA.PRO.RVI"://JefeVentaProvincia
                                            //PermisoModificar = false;
                        break;
                    case "GTE.DIV.RVI"://GerenteDivision
                        PermisoModificar = false;
                        break;
                    case "AST.RVI.COM"://AsistenteComercial
                                        //PermisoModificar = false;
                        break;
                    case "JEF.RVI.OPE"://JefeOperaciones
                        break;
                    case "AST.RVI.OPE"://AsistenteOperaciones
                        PermisoModificar = false;
                        break;
                }
            }
            //}

            return PermisoModificar;

        }

        public bool ConfigurarBotonCopiarParaSoles(object moneda)
        {
            if (moneda.ToString() == Enums.MonedaIFPSimbolo.Soles.StringValue())
            {
                PermisoInsertar = false;
            }
            else
            {
                PermisoInsertar = true;
            }

            return PermisoInsertar;
        }

    }
}