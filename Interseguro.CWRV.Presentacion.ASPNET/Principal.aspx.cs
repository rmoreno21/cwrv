using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Infraestructura.General;

namespace Interseguro.CWRV.Presentacion.ASPNET
{
    public partial class Principal : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ////// Verificar si se ha accedido mediante token
            //////if (Request.QueryString["t"] != null)
            //////{
            ////Response.Redirect("~/Cotizador/Cotizador.aspx");
            //////}

            string codRol = (string)HttpContext.Current.Session["RolAzman"];

            if (Enums.RolAzman.AgenteLima.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.AgenteLima.StringValue());
            }
            else if (Enums.RolAzman.AgenteProvincia.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.AgenteProvincia.StringValue());
            }
            else if (Enums.RolAzman.SupervisorLima.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.SupervisorLima.StringValue());
            }
            else if (Enums.RolAzman.SupervisorProvincia.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.SupervisorProvincia.StringValue());
            }
            else if (Enums.RolAzman.JefeVentaLima.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.JefeVentaLima.StringValue());
            }
            else if (Enums.RolAzman.JefeVentaProvincia.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.JefeVentaProvincia.StringValue());
            }
            else if (Enums.RolAzman.AsistenteComercial.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.AsistenteComercial.StringValue());
            }
            else if (Enums.RolAzman.GerenteDivision.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.GerenteDivision.StringValue());
            }
            else if (Enums.RolAzman.JefeOperaciones.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.JefeOperaciones.StringValue());
            }
            else if (Enums.RolAzman.AsistenteOperaciones.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.AsistenteOperaciones.StringValue());
            }
            else if (Enums.RolAzman.CoordinadorPlaft.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.CoordinadorPlaft.StringValue());
            }
            else if (Enums.RolAzman.AgenteExterno.StringValue() == codRol)
            {
                Response.Redirect(Enums.urlDefectoRol.AgenteExterno.StringValue());
            }
            else
            {
                Response.Redirect("~/Cotizador/Cotizador.aspx");
            }

        }
    }
}