using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesOficiales : System.Web.UI.UserControl
    {

        public List<SolicitudEscenario> SolicitudesEscenario { get; set; }

        List<SolicitudEscenario> SolicitudClientes;
        List<SolicitudEscenario> SolicitudCotizaciones;

        public bool PermisoConsultar { get; set; }
        public bool PermisoModificar { get; set; }
        public bool PermisoCorreoElectronico { get; set; }
        public bool PermisoExportarPDF { get; set; }
        public bool PermisoReporteEscenario { get; set; }
        public bool PermisoSolicitudAnticipo { get; set; }
        public bool PermisoValidacionesACOM { get; set; }
        public bool PermisoValidacionesDTRA { get; set; }
        public bool Consentimiento { get; set; }
        public bool RedLocal { get; set; }
        public string SolicitudResaltar { get; set; }
        private int AnchoColumnaIconos;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                AnchoColumnaIconos = 66;
                if (Consentimiento) AnchoColumnaIconos += 32;
                if (RedLocal) AnchoColumnaIconos += 32;

                List<SolicitudEscenario> SolicitudFecha = new List<SolicitudEscenario>();

                //SoliCitudFecha = SolicitudesEscenario.Where(s => s.NumOperacion != null).Select(s => new List<SolicitudEscenario>()
                //{
                //    fec_presentacion = s.fec_presentacion
                //}).ToList;

                //SolicitudFecha = SolicitudesEscenario.Where(x => x.FecPresentacion != null && x.NumOperacion == 0).ToList();

                SolicitudFecha = SolicitudesEscenario.Where(x => x.FechaPresentacion != null && x.NumNivel == 1).ToList();

                TabSolicitudOficial.DataSource = SolicitudFecha;
                TabSolicitudOficial.DataBind();
                if (SolicitudFecha.Count > 0)
                {
                    TabSolicitudOficial.HeaderRow.TableSection = TableRowSection.TableHeader;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }


        protected void TabSolicitudOficial_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Cells[10].Width = AnchoColumnaIconos;

                SolicitudClientes = new List<SolicitudEscenario>();

                DateTime fecPresentacion = Convert.ToDateTime(TabSolicitudOficial.DataKeys[e.Row.RowIndex].Value.ToString());

                //SolicitudClientes = SolicitudesEscenario.Where(x => x.FecPresentacion == fecPresentacion &&
                //                                                    x.NumOperacion != 0).
                //                                                    GroupBy(item => new
                //                                                    {
                //                                                        item.FecPresentacion,
                //                                                        item.NumOperacion,
                //                                                        item.Agente.Id,
                //                                                        item.Agente.Nombre,
                //                                                        item.IndVigenciaAgente,
                //                                                        item.Supervision.Supervisor,
                //                                                        item.Afiliado.CUSPP,
                //                                                        item.Afiliado.NombreEmpresa
                //                                                    })
                //                                                    .Select(item => new SolicitudEscenario
                //                                                    {
                //                                                        FecPresentacion = item.Key.FecPresentacion,
                //                                                        NumOperacion = item.Key.NumOperacion,
                //                                                        Agente = new Agente { Id = item.Key.Id, Nombre = item.Key.Nombre },
                //                                                        IndVigenciaAgente = item.Key.IndVigenciaAgente,
                //                                                        Supervision = new Supervision { Supervisor = item.Key.Supervisor },
                //                                                        Afiliado = new Afiliado { CUSPP = item.Key.CUSPP,
                //                                                                                  NombreEmpresa = item.Key.NombreEmpresa
                //                                                                                }
                //                                                    }
                //                                                    )
                //                                                    .ToList();


                //SolicitudClientes = SolicitudesEscenario.Where(x => x.FecPresentacion == fecPresentacion &&
                //                                                    x.NumNivel == 2).
                //                                                    GroupBy(item => new
                //                                                    {
                //                                                        item.FecPresentacion,
                //                                                        item.NumOperacion,
                //                                                        item.Agente.Id,
                //                                                        item.Agente.Nombre,
                //                                                        item.IndVigenciaAgente,
                //                                                        item.Supervision.Supervisor,
                //                                                        item.Afiliado.CUSPP,
                //                                                        item.Afiliado.NombreEmpresa,
                //                                                        item.ValidarACOM,
                //                                                        item.ValidarDTRA
                //                                                    })
                //                                                    .Select(item => new SolicitudEscenario
                //                                                    {
                //                                                        FecPresentacion = item.Key.FecPresentacion,
                //                                                        NumOperacion = item.Key.NumOperacion,
                //                                                        Agente = new Agente { Id = item.Key.Id, Nombre = item.Key.Nombre },
                //                                                        IndVigenciaAgente = item.Key.IndVigenciaAgente,
                //                                                        Supervision = new Supervision { Supervisor = item.Key.Supervisor },
                //                                                        Afiliado = new Afiliado
                //                                                        {
                //                                                            CUSPP = item.Key.CUSPP,
                //                                                            NombreEmpresa = item.Key.NombreEmpresa
                //                                                        },
                //                                                        ValidarACOM = item.Key.ValidarACOM,
                //                                                        ValidarDTRA = item.Key.ValidarDTRA
                //                                                    }
                //                                                    )
                //                                                    .ToList();

                SolicitudClientes = SolicitudesEscenario.FindAll(x => x.FechaPresentacion == fecPresentacion &&
                                                                      x.NumNivel == 2);
                GridView TabCotizacionCliente = (GridView)e.Row.FindControl("TabCotizacionCliente");
                TabCotizacionCliente.DataSource = SolicitudClientes;
                TabCotizacionCliente.DataBind();
            }
        }

        protected void TabCotizacionCliente_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //e.Row.Cells[10].Width = AnchoColumnaIconos;

                SolicitudCotizaciones = new List<SolicitudEscenario>();
                GridView TabCotizacionCliente = sender as GridView;
                Int64 numOperacion = Convert.ToInt64(TabCotizacionCliente.DataKeys[e.Row.RowIndex].Value.ToString());

                //SolicitudCotizaciones = SolicitudesEscenario.Where(x => x.NumOperacion == numOperacion && 
                //                                                        x.NumSolicitud != null).ToList();

                SolicitudCotizaciones = SolicitudesEscenario.Where(x => x.NumOperacion == numOperacion &&
                                                                        x.NumNivel == 3).ToList();

                GridView TabCotizacionCotizaciones = (GridView)e.Row.FindControl("TabCotizacionCotizaciones");
                TabCotizacionCotizaciones.DataSource = SolicitudCotizaciones;
                TabCotizacionCotizaciones.DataBind();
            }
        }

        protected void TabCotizacionCotizaciones_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Ressaltar la última cotizada y la seleccionada
                SolicitudEscenario solicitud = (SolicitudEscenario)e.Row.DataItem;
                if (solicitud.IndEstadoSeleccion == "S")
                {
                    e.Row.CssClass = "grilla_active_yellow";
                }
                if (solicitud.NumSolicitud == SolicitudResaltar)
                {
                    if (solicitud.IndEstadoSeleccion != "S")
                    {
                        e.Row.CssClass = String.Format("grilla_highlight{0}", ((e.Row.RowIndex % 2) + 1));
                    }
                    else
                    {
                        e.Row.CssClass = "grilla_highlight3";
                    }
                }
            }
        }
    }
}