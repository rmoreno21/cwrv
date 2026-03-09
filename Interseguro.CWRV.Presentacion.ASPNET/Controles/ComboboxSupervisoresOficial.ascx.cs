using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class ComboboxSupervisoresOficial : System.Web.UI.UserControl
    {

        public List<Agente> Supervisores { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            OfiSupervisor.Items.Clear();
            OfiSupervisor.Items.Add(new ListItem("«Todos»", "0"));
            foreach (Agente item in Supervisores)
            {
                OfiSupervisor.Items.Add(new ListItem(item.Nombre, item.Id));
            }

        }
    }
}