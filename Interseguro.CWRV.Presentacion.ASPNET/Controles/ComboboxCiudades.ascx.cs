using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class ComboboxCiudades : System.Web.UI.UserControl
    {
        public List<Ciudad> Ciudades { get; set; }
        //public Ubigeo[] Ciudades { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ModDirCiudad.Items.Clear();
            ModDirCiudad.Items.Add(new ListItem("«Seleccione»", "0"));

            foreach (Ciudad item in Ciudades)
            {
                ModDirCiudad.Items.Add(new ListItem(item.Nombre, item.Id));
            }

        }
    }
}