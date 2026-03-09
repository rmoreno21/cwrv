using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class ComboboxAgentesOficial : System.Web.UI.UserControl
    {

        public List<Agente> Agentes { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

            OfiAgente.Items.Clear();
            OfiAgente.Items.Add(new ListItem("«Todos»", "0"));
            foreach (Agente item in Agentes)
            {
                OfiAgente.Items.Add(new ListItem(item.Nombre, item.Id));
            }

        }
    }
}