using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class ComboboxComunasAlterna : System.Web.UI.UserControl
    {
        public List<Comuna> Comunas { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            ModDistritoAlterna.Items.Clear();
            ModDistritoAlterna.Items.Add(new ListItem("«Seleccione»", "0"));

            foreach (Comuna item in Comunas)
            {
                ModDistritoAlterna.Items.Add(new ListItem(item.Nombre, item.Id));
            }

        }
    }
}