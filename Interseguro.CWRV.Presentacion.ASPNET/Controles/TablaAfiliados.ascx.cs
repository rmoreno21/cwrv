using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class TablaAfiliados : System.Web.UI.UserControl
    {
        public List<Afiliado> Afiliados { get; set; }

        public int IndicePagina { get; set; }
        public int TamanhoPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int ColumnaOrdenar { get; set; }
        public char DireccionOrdenar { get; set; }

        public bool PermisoConsultar { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PermisoConsultar)
            {
                TabAfiliados.DataSource = Afiliados;
                TabAfiliados.DataBind();

                if (TotalRegistros > 0)
                {
                    TabAfiliados.HeaderRow.TableSection = TableRowSection.TableHeader;
                    TabAfiContenedorPaginador.Visible = true;
                    ModBusAfiContenedorAceptar.Visible = true;

                    if (Convert.ToBoolean(HttpContext.Current.Session["Externo"]))
                    {
                        TabAfiliados.Columns[3].Visible = false;
                    }
                }

                //Armar el paginador
                int totalPaginas = (int)Math.Ceiling((double)TotalRegistros / TamanhoPagina);
                if (totalPaginas > 1)
                {
                    string paginador;
                    paginador = "<ul id=\"TabAfiliadosPaginador\" class=\"tsc_pagination tsc_paginationB tsc_paginationB02\">";
                    if (IndicePagina > 1)
                    {
                        paginador += "<li><a href=\"javascript:void(0);\" data-pag=\"1\" class=\"first\">&lt;&lt;</a></li>";
                        paginador += "<li><a href=\"javascript:void(0);\" data-pag=\"" + (IndicePagina - 1) + "\" class=\"previous\">&lt;</a></li>";
                    }

                    int pagMinima;
                    pagMinima = (IndicePagina - 2 > 0) ? IndicePagina - 2 : 1;
                    //pagMaxima = (IndicePagina + 2 < totalPaginas) ? IndicePagina + 2 : totalPaginas;

                    int i;
                    for (i = pagMinima; i < pagMinima + 5; i++)
                    {
                        if (i > totalPaginas) break;

                        paginador += "<li><a href=\"javascript:void(0);\" data-pag=\"" + i + "\"";
                        if (i == IndicePagina)
                        {
                            paginador += " class=\"current\"";
                        }
                        paginador += ">" + i + "</a></li>";
                    }

                    if (IndicePagina < totalPaginas)
                    {
                        paginador += "<li><a href=\"javascript:void(0);\" data-pag=\"" + (IndicePagina + 1) + "\" class=\"next\">&gt;</a></li>";
                        paginador += "<li><a href=\"javascript:void(0);\" data-pag=\"" + totalPaginas + "\" class=\"last\">&gt;&gt;</a></li>";
                    }
                    paginador += "</ul>";

                    TabAfiliadosPaginador.Text = paginador;

                    TabAfiliadosPaginadorIndice.Text = "Página " + IndicePagina + "/" + totalPaginas;
                }
            }
            else
            {
                SinPermisos.Visible = true;
            }
        }

        protected void TabAfiliados_RowDataBound(object sender, GridViewRowEventArgs e)
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
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[4].Text = "<input type=\"radio\" name=\"CUSPP\" value=\"" + e.Row.Cells[3].Text + "\" />";
            }
        }
    }
}