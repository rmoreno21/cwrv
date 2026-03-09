using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class MenuPrincipal : System.Web.UI.UserControl
    {
        public List<OpcionSistema> ListaOpciones { get; set; }
        public int AnchoSubmenu { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string menu = String.Empty;

            if (ListaOpciones != null && ListaOpciones.Count > 0)
            {
                //<INIGTI_4081>
                //menu += "<nav>";
                //<FINGTI_4081>
                menu += "<ul id=\"Menu\">";

                //1. Se obtienen los menúes principales: ID_OPCION_PADRE = 0 ordenados por campo Orden
                //SortedList<int, OpcionSistema> listaMenuPrincipal = new SortedList<int, OpcionSistema>();
                List<OpcionSistema> listaMenuPrincipal = ListaOpciones
                    .FindAll(m => m.IdPadre == 0 && m.Activa)
                    .OrderBy(m => m.Orden).ToList();

                //ListaOpciones
                    //.ForEach(m => { if (m.IdPadre == 0 && m.Activa) listaMenuPrincipal.Add(m.Orden, m); });

                for (int i = 0; i < listaMenuPrincipal.Count; i++)
                {
                    string submenu = String.Empty;
                    OpcionSistema opcionMenu = listaMenuPrincipal[i];
            
                    //SortedList<int, OpcionSistema> listaMenuHijos = new SortedList<int, OpcionSistema>();

                    List<OpcionSistema> listaMenuHijos = ListaOpciones
                        .FindAll(h => h.IdPadre == opcionMenu.Id && h.Activa)
                        .OrderBy(h => h.Orden).ToList();

                    //ListaOpciones
                    //    .FindAll(h => h.IdPadre == opcionMenu.Id && h.Activa)
                    //    .ForEach(h => listaMenuHijos.Add(h.Orden, h));

                    //
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

                    menu += submenu;
                }

                menu += "</ul>";
                //<INIGTI_4081>
                //menu += "</nav>";
                //<FINGTI_4081>
                ArbolMenu.Text = menu;
            }
        }
    }
}