using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class MenuMosaico : System.Web.UI.UserControl
    {
        public List<OpcionSistema> ListaOpciones { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string menu = String.Empty;

            if (ListaOpciones != null && ListaOpciones.Count > 0)
            {
                //1. Se obtienen los menúes principales: ID_OPCION_PADRE = 0 ordenados por campo Orden
                var listaMenuPrincipal = (from opcion in ListaOpciones
                                          where opcion.IdPadre == 0 && opcion.Activa
                                          orderby opcion.Orden
                                          select opcion).ToList();

                for (int i = 0; i < listaMenuPrincipal.Count; i++)
                {
                    menu += i + "<br />";

                    /*string submenu = String.Empty;
                    OpcionSistema opcionMenu = listaMenuPrincipal[i];

                    List<OpcionSistema> listaMenuHijos = ListaOpciones
                        .FindAll(h => h.IdPadre == opcionMenu.Id && h.Activa)
                        .OrderBy(h => h.Orden).ToList();

                    if (listaMenuHijos.Count > 0)
                    {
                        submenu += "<li>";
                        if (opcionMenu.RutaIcono != null && opcionMenu.RutaIcono != String.Empty)
                            submenu += String.Format("<img src=\"{0}\" /> ", ResolveUrl(opcionMenu.RutaIcono));
                        submenu += String.Format("{0}<ul style=\"width:{1}px\">", opcionMenu.Titulo, AnchoSubmenu);
                        for (int j = 0; j < listaMenuHijos.Count; j++)
                        {
                            OpcionSistema opcionHija = listaMenuHijos[j];

                            submenu += String.Format("<a href=\"{0}\"><li style=\"width:{1}px;{2}\">",
                                ((opcionHija.Ruta != String.Empty) ? ResolveUrl(opcionHija.Ruta) : "#"),
                                AnchoSubmenu,
                                ((j != 0) ? String.Empty : "margin-top:-1px"));
                            if (opcionHija.RutaIcono != null && opcionHija.RutaIcono != String.Empty)
                                submenu += String.Format("<img src=\"{0}\" /> ", ResolveUrl(opcionHija.RutaIcono));
                            submenu += String.Format("{0}</li></a>", opcionHija.Titulo);
                        }
                        submenu += "</ul></li>";
                    }
                    else
                    {
                        submenu += String.Format("<a href=\"{0}\"><li>", ResolveUrl(opcionMenu.Ruta));
                        if (opcionMenu.RutaIcono != null && opcionMenu.RutaIcono != String.Empty)
                            submenu += String.Format("<img src=\"{0}\" /> ", ResolveUrl(opcionMenu.RutaIcono));
                        submenu += String.Format("{0}</li></a>", opcionMenu.Titulo);
                    }

                    menu += submenu;*/
                }

                Menu.Text = menu;
            }
        }
    }
}