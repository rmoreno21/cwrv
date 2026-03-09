using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class MenuMaterialize : UserControl
    {
        public List<OpcionSistema> ListaOpciones { get; set; }
        public Usuario Usuario { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            string menu = string.Empty;
            string menuMobile = string.Empty;
            string submenus = string.Empty;
            string submenusMobile = string.Empty;

            if (ListaOpciones != null && ListaOpciones.Count > 0)
            {
                menu += "<ul class=\"right hide-on-med-and-down\">";
                menuMobile = "<ul class=\"sidenav\" id=\"mobile-menu\">";
                menuMobile += "<li><div class=\"user-view\"><div class=\"blue darken-4 background\"></div>";
                menuMobile += string.Format("<a href=\"#\"><span class=\"center-align name white-text\"><i class=\"material-icons\" style=\"font-size:64px\">account_circle</i></span></a>", Usuario.Nombre);
                menuMobile += string.Format("<a href=\"#\"><span class=\"center-align name white-text\">{0}</span></a>", Usuario.Nombre);
                menuMobile += string.Format("<a href=\"#\"><span class=\"center-align email white-text\">{0} - {1}</span></a>", Usuario.NombreUsuario, Usuario.Rol);
                menuMobile += "</div></li>";

                // 1. Se obtienen los menúes principales: ID_OPCION_PADRE = 0 ordenados por campo Orden
                // SortedList<int, OpcionSistema> listaMenuPrincipal = new SortedList<int, OpcionSistema>();
                List<OpcionSistema> listaMenuPrincipal = ListaOpciones
                    .FindAll(m => m.IdPadre == 0 && m.Activa)
                    .OrderBy(m => m.Orden).ToList();
                foreach (OpcionSistema opcion in listaMenuPrincipal)
                {
                    // 2. Validar si la opción tiene un submenú
                    List<OpcionSistema> listaSecundaria = ListaOpciones.FindAll(m => m.IdPadre == opcion.Id).OrderBy(m => m.Orden).ToList();
                    string dataTarget = string.Empty;
                    if (listaSecundaria.Count > 0)
                    {
                        dataTarget = string.Format(" data-target=\"dd{0}\"", opcion.Nombre);
                    }
                    if (opcion.Activa)
                    {
                        menu += string.Format("<li><a class=\"dropdown-trigger waves-effect waves-light\"{0}><img src=\"{1}\" style=\"height:16px;margin-bottom:-2px\" /> {2}</a></li>", dataTarget, ResolveUrl(opcion.RutaIcono), opcion.Titulo);
                        //menuMobile += string.Format("<li><a href=\"{0}\">{1}</a></li>", ResolveUrl(opcion.Ruta), opcion.Titulo);
                    }

                    string submenu = string.Empty;
                    int contador = 0;
                    foreach (OpcionSistema opcionSecundaria in listaSecundaria)
                    {
                        if (contador == 0)
                        {
                            submenu += string.Format("<ul id=\"dd{0}\" class=\"dropdown-content\" data-constrain-width=\"false\">", opcion.Nombre);
                            menuMobile += string.Format("<li><ul class=\"collapsible collapsible-accordion\"><li><a class=\"collapsible-header waves-effect waves-blue\">{0}</a><div class=\"collapsible-body\"><ul>", opcion.Titulo);
                        }

                        string icono = string.Empty;
                        if (!string.IsNullOrEmpty(opcionSecundaria.RutaIcono))
                        {
                            icono = opcionSecundaria.RutaIcono;
                        }
                        if (opcionSecundaria.Activa)
                        {
                            submenu += string.Format("<li><a href=\"{0}\" class=\"waves-effect waves-light\">{1}{2}</a></li>", ResolveUrl(opcionSecundaria.Ruta), icono, opcionSecundaria.Titulo);
                            menuMobile += string.Format("<li><a href=\"{0}\">{1}</a></li>", ResolveUrl(opcionSecundaria.Ruta), opcionSecundaria.Titulo);
                        }
                        contador++;
                    }
                    if (contador > 0)
                    {
                        submenu += "</ul>";
                        menuMobile += "</ul></div></li></ul</li>";
                    }
                    submenus += submenu;
                }

                // Agregar el usuario
                string nombre = Usuario.Nombre;
                try
                {
                    nombre = Usuario.Nombre.Split(',')[1];
                }
                catch (Exception) { }
                
                menu += string.Format("<li><a class=\"dropdown-trigger waves-effect waves-light\" data-target=\"ddUsuario\"><i class=\"material-icons left\">account_circle</i> {0}</a></li>", nombre);
                menu += "</ul>";
                menuMobile += "</ul>";

                menu += "<ul id=\"ddUsuario\" class=\"dropdown-content\" data-constrain-width=\"false\">";
                menu += string.Format("<li><a href=\"#!\"><i class=\"material-icons left\">assignment_ind</i>{0}</li></a>", Usuario.NombreUsuario);
                menu += string.Format("<li><a href=\"#!\"><i class=\"material-icons left\">person</i>{0}</a></li>", Usuario.Nombre);
                menu += string.Format("<li><a href=\"#!\"><i class=\"material-icons left\">work</i>{0}</a></li>", Usuario.Rol);
                menu += "<li class=\"divider\" tabindex=\"-1\"></li>";
                menu += string.Format("<li><a href=\"{0}\"><i class=\"material-icons left\">power_settings_new</i>Cerrar Sesión</a></li>", ResolveUrl("~/Seguridad/CerrarSesion.aspx"));
                menu += "</ul>";

                ArbolMenu.Text = menu + submenus + menuMobile;
            }
        }
    }
}