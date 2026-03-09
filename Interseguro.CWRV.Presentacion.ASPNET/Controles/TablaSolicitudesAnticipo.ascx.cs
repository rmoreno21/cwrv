using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaSolicitudesAnticipo : System.Web.UI.UserControl
    {
        public List<Anticipo> Anticipos { get; set; }

        public int IndicePagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int ColumnaOrdenar { get; set; }
        public char DireccionOrdenar { get; set; }

        public bool PermisoConsultar { get; set; }
        public bool PermisoExportarExcel { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabSolicitudesAnticipo.DataSource = Anticipos;
                TabSolicitudesAnticipo.DataBind();
                if (Anticipos.Count > 0)
                {
                    TabSolicitudesAnticipo.HeaderRow.TableSection = TableRowSection.TableHeader;
                }

                //Armar el paginador
                int totalPaginas = (int)Math.Ceiling((double)TotalRegistros / TamanhoPagina);
                if (totalPaginas > 1)
                {
                    string paginador;
                    paginador = @"<ul id=""TabSolicitudesAnticipoPaginador"" class=""tsc_pagination tsc_paginationB tsc_paginationB02"">";
                    if (IndicePagina > 1)
                    {
                        paginador += @"<li><a href=""javascript:void(0);"" data-pag=""1"" class=""first"">&lt;&lt;</a></li>";
                        paginador += @"<li><a href=""javascript:void(0);"" data-pag=""" + (IndicePagina - 1) + @""" class=""previous"">&lt;</a></li>";
                    }

                    int pagMinima;
                    pagMinima = (IndicePagina - 2 > 0) ? IndicePagina - 2 : 1;

                    int i;
                    for (i = pagMinima; i < pagMinima + 5; i++)
                    {
                        if (i > totalPaginas) break;

                        paginador += @"<li><a href=""javascript:void(0);"" data-pag=""" + i + @"""";
                        if (i == IndicePagina)
                        {
                            paginador += @" class=""current""";
                        }
                        paginador += ">" + i + "</a></li>";
                    }

                    if (IndicePagina < totalPaginas)
                    {
                        paginador += @"<li><a href=""javascript:void(0);"" data-pag=""" + (IndicePagina + 1) + @""" class=""next"">&gt;</a></li>";
                        paginador += @"<li><a href=""javascript:void(0);"" data-pag=""" + totalPaginas + @""" class=""last"">&gt;&gt;</a></li>";
                    }
                    paginador += "</ul>";

                    TabSolicitudesAnticipoPaginador.Text = paginador;

                    TabSolicitudesAnticipoPaginadorIndice.Text = "Página " + IndicePagina + "/" + totalPaginas;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }

        protected void TabSolicitudesAnticipo_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (i + 1 != ColumnaOrdenar)
                    {
                        e.Row.Cells[i].Text = "<a href=\"javascript:void(0);\" data-col=\"" + (i + 1) + "\">" + e.Row.Cells[i].Text + "</a>";
                    }
                    else
                    {
                        if (DireccionOrdenar == 'A')
                        {
                            e.Row.Cells[i].Text = "<a href=\"javascript:void(0);\" data-col=\"" + ColumnaOrdenar + "\">" + e.Row.Cells[i].Text + "</a> <img border=\"0\" src=\"../Imagenes/grilla_asc.gif\" />";
                        }
                        else
                        {
                            e.Row.Cells[i].Text = "<a href=\"javascript:void(0);\" data-col=\"" + ColumnaOrdenar + "\">" + e.Row.Cells[i].Text + "</a> <img border=\"0\" src=\"../Imagenes/grilla_desc.gif\" />";
                        }
                    }

                }
            }
        }
    }
}