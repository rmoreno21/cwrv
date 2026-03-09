using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class ConsentimientoMensaje : System.Web.UI.UserControl
    {
        public bool tieneConsentimiento { get; set; } = false;
        public string Mensaje { get; set; }
        public string Clase { get; set; }

        //public string NombreCompleto { get; set; }
        //public string TipoDocumento { get; set; }
        //public string NumeroDocumento { get; set; }
        public string CorreoElectronico { get; set; }

        public bool sobrevivencia { get; set; } = false;
        public List<GrupoFamiliar> ListaSobrevivencia { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (sobrevivencia)
            {
                consentimiento_asesoria_sobrevivencia.Visible = true;
                consentimiento_asesoria_sobrevivencia.CssClass = Clase;

                dlBeneficiarios.Items.Clear();
                dlBeneficiarios.Items.Add(new ListItem("«Seleccione»", "0"));
                foreach (var itemBeneficiario in ListaSobrevivencia)
                {
                    dlBeneficiarios.Items.Add(new ListItem(itemBeneficiario.Nombre + " " + itemBeneficiario.ApellidoPaterno + " " + itemBeneficiario.ApellidoMaterno, itemBeneficiario.Id.ToString()));
                }

                lblCorreo.Text = CorreoElectronico;

                consentimiento_asesoria.Visible = false;

                //dlBeneficiarios.DataSource = ListaSobrevivencia;
                //dlBeneficiarios.DataValueField = "Id";
                //dlBeneficiarios.DataTextField = "Nombre";
                //dlBeneficiarios.DataBind();
            }
            else
            {
                consentimiento_asesoria.Visible = tieneConsentimiento;
                consentimiento_asesoria.CssClass = Clase;
                lblMensaje.Text = Mensaje;

                consentimiento_asesoria_sobrevivencia.Visible = false;
            }

        }
    }
}
