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
    public partial class TablaSolicitudesBandejaOficiales : System.Web.UI.UserControl
    {

        public List<SolicitudEscenario> SolicitudesEscenario { get; set; }
        //<INIGTI_4081>
        public List<SolicitudEscenario> SolicitudesEscenarioDetalle { get; set; }
        //<FINGTI_4081>

        //List<SolicitudEscenario> SolicitudClientes;
        //List<SolicitudEscenario> SolicitudCotizaciones;

        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoCorreoElectronico { get; set; }
        public bool PermisoExportarPDF { get; set; }
        public bool PermisoReporteEscenario { get; set; }
        public bool Consentimiento { get; set; }
        public bool RedLocal { get; set; }
        private int AnchoColumnaIconos;

        public string rol { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (PermisoConsultar)
            {
                rol = ((string)Session["RolAzman"]);

                AnchoColumnaIconos = 66;
                if (Consentimiento) AnchoColumnaIconos += 32;
                if (RedLocal) AnchoColumnaIconos += 32;

                List<SolicitudEscenario> SolicitudFecha = new List<SolicitudEscenario>();

                
                List<DateTime?> Fechas = SolicitudesEscenario.Select(x => x.FechaPresentacion).Distinct().OrderBy(p => p.Value).ToList();

                for (int i = 0; i < Fechas.Count(); i++)
                {
                    SolicitudEscenario sol = new SolicitudEscenario();
                    sol.FechaPresentacion = Fechas[i].Value;
                    SolicitudFecha.Add(sol);
                }

                TabSolicitudOficial.DataSource = SolicitudFecha;
                TabSolicitudOficial.DataBind();
                panelBotonera.Visible = false;
                if (SolicitudFecha.Count > 0)
                {
                    TabSolicitudOficial.HeaderRow.TableSection = TableRowSection.TableHeader;
                    //<INIGTI_4081>
                    panelBotonera.Visible = true;
                    CotEstadoMovimiento.Text = SolicitudesEscenario[0].TipoMovimiento.Nombre;
                    //<FINGTI_4081>
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }

            if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.RechazarSolicitud))
            {
                PerRechazarSolicitud.Value = "1";
            }
            else
            {
                PerRechazarSolicitud.Value = "0";
            }

        }
        
        protected void TabBandejaSolicitudOficial_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    if (rol != "") {
            //        GridView TabCotizacionCliente = (GridView)e.Row.FindControl("TabBandejaSolicitudOficial");
            //        TabCotizacionCliente.Columns[13].Visible = false;
            //        TabCotizacionCliente.Columns[14].Visible = false;
            //    }
            //}
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    Label TabCotizacionCliente = (Label)e.Row.FindControl("LabCotPjeTasaVentaMaxima");
            //    Label CotPjeTasaVentaMaxima = (Label)e.Row.FindControl("CotPjeTasaVentaMaxima");
            //    if (rol == Enums.RolAzman.JefeVentaLima.StringValue() || rol == Enums.RolAzman.JefeVentaProvincia.StringValue())
            //    {
            //        TabCotizacionCliente.Visible = false;
            //        CotPjeTasaVentaMaxima.Visible = false;
            //    }
            //}
        }

        protected void TabSolicitudOficial_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                SolicitudesEscenarioDetalle = new List<SolicitudEscenario>();

                DateTime fecPresentacion = Convert.ToDateTime(TabSolicitudOficial.DataKeys[e.Row.RowIndex].Value.ToString());



                SolicitudesEscenarioDetalle = SolicitudesEscenario.FindAll(x => x.FechaPresentacion == fecPresentacion);
                GridView TabCotizacionCliente = (GridView)e.Row.FindControl("TabBandejaSolicitudOficial");

                SolicitudesEscenarioDetalle.ForEach(
                    p =>
                    {
                        p.Afiliado.CUSPP = (p.Recotizacion == "No") ? p.Afiliado.CUSPP : p.Afiliado.CUSPP + "<span id='asterisco'>*</span>";
                    }
                    );

                TabCotizacionCliente.DataSource = SolicitudesEscenarioDetalle;
                TabCotizacionCliente.DataBind();

                if (rol != "GTE.DIV.RVI")
                {
                    if (SolicitudesEscenarioDetalle.Count > 0)
                    {
                        TabCotizacionCliente.Columns[6].Visible = false;// Tasas
                        TabCotizacionCliente.Columns[7].Visible = false;//TRA
                    }
                }
                
            }
        }

    }
}