using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class ComboboxCiudadesAlterna : System.Web.UI.UserControl
    {
        public List<Ciudad> Ciudades { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            ModProvinciaAlterna.Items.Clear();
            ModProvinciaAlterna.Items.Add(new ListItem("«Seleccione»", "0"));

            foreach (Ciudad item in Ciudades)
            {
                ModProvinciaAlterna.Items.Add(new ListItem(item.Nombre, item.Id));
            }

        }
    }
}