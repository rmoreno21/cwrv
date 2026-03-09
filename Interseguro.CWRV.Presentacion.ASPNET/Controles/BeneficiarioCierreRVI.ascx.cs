using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class BeneficiarioCierreRVI : System.Web.UI.UserControl
    {
        public Beneficiario Beneficiario { get; set; }
        public List<List<Parametro>> ListaCombobox { get; set; }
        public List<Nacionalidad> ListaNacionalidad { get; set; }
        public JArray ListaPaisOrigen { get; set; }
        public JArray ListaVinculoFamiliar { get; set; }
        public bool ExistePersonaRviadm { get; set; }
        public string GlosaTipoPension { get; set; }
        public JArray ListaESSALUD { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            CargarInformacionInicialPantalla();
            CargarBeneficiarios();
        }

        private void CargarInformacionInicialPantalla()
        {
            CargarCombobox(TipoDocumento, (List<Parametro>)ListaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);
            CargarCombobox(Parentesco, (List<Parametro>)ListaCombobox[(int)Enums.CategoriaCombobox.Parentesco]);
            CargarCombobox(Sexo, (List<Parametro>)ListaCombobox[(int)Enums.CategoriaCombobox.Sexo]);
            CargarCombobox(TipoInvalidez, (List<Parametro>)ListaCombobox[(int)Enums.CategoriaCombobox.Invalidez]);

            CargarComboboxOtrasEntidades(Nacionalidad, "nacionalidad", null, ListaNacionalidad);

            IndInvalidez.Items.Add(new ListItem("«Seleccione»", "0"));
            IndInvalidez.Items.Add(new ListItem("Sí", "S"));
            IndInvalidez.Items.Add(new ListItem("No", "N"));

            if (ListaPaisOrigen != null)
            {
                foreach (var item in ListaPaisOrigen)
                {
                    PaisOrigen.Items.Add(new ListItem(item["glosa"].ToString().ToUpper(), item["codigo"].ToString().ToLower()));
                }
            }

            VinculoFamiliar.Items.Add(new ListItem("Seleccione", ""));
            if (ListaVinculoFamiliar != null)
            {
                foreach (var item in ListaVinculoFamiliar)
                {
                    VinculoFamiliar.Items.Add(new ListItem(item["glosa"].ToString().ToUpper(), item["codigo"].ToString().ToLower()));
                }
            }

            //ESSALUD.Items.Add(new ListItem("Seleccione", ""));
            if (ListaESSALUD != null)
            {
                foreach (var item in ListaESSALUD)
                {
                    ESSALUD.Items.Add(new ListItem(item["glosa"].ToString().ToUpper(), item["codigo"].ToString().ToLower()));
                }
            }
        }

        private void CargarBeneficiarios()
        {
            // Tarjeta Datos del Afiliado o Beneficiario
            ModNumCorrelativo.ID = "ModNumCorrelativo" + Beneficiario.numCorrelativo;
            ModNumCorrelativo.Text = Convert.ToString(Beneficiario.numCorrelativo);

            ApellidoPaterno.ID = "ApellidoPaterno" + Beneficiario.numCorrelativo;
            ApellidoPaterno.Text = Beneficiario.ApellidoPaterno;

            ApellidoMaterno.ID = "ApellidoMaterno" + Beneficiario.numCorrelativo;
            ApellidoMaterno.Text = Beneficiario.ApellidoMaterno;

            Nombres.ID = "Nombres" + Beneficiario.numCorrelativo;
            Nombres.Text = Beneficiario.Nombre;

            TipoDocumento.ID = "TipoDocumento" + Beneficiario.numCorrelativo;
            TipoDocumento.SelectedIndex = TipoDocumento.Items.IndexOf(TipoDocumento.Items.FindByValue(Beneficiario.Identificacion.IdTipo));

            NumeroDocumento.ID = "NumeroDocumento" + Beneficiario.numCorrelativo;
            NumeroDocumento.Text = Beneficiario.Identificacion.Numero;

            Parentesco.ID = "Parentesco" + Beneficiario.numCorrelativo;
            Parentesco.SelectedValue = Beneficiario.Parentesco.Id;

            IndInvalidez.ID = "IndInvalidez" + Beneficiario.numCorrelativo;
            IndInvalidez.SelectedValue = Beneficiario.Invalido ? "S" : "N";

            TipoInvalidez.ID = "TipoInvalidez" + Beneficiario.numCorrelativo;
            TipoInvalidez.SelectedValue = Beneficiario.TipoInvalidez.Id;

            FechaInvalidez.ID = "FechaInvalidez" + Beneficiario.numCorrelativo;
            FechaInvalidez.Text = Beneficiario.FechaInvalidez != null ? Convert.ToDateTime(Beneficiario.FechaInvalidez).ToString("dd/MM/yyyy") : "";

            Sexo.ID = "Sexo" + Beneficiario.numCorrelativo;
            Sexo.SelectedValue = Beneficiario.Sexo.ToString();

            FechaNacimiento.ID = "FechaNacimiento" + Beneficiario.numCorrelativo;
            FechaNacimiento.Text = Beneficiario.FechaNacimiento != null ? Convert.ToDateTime(Beneficiario.FechaNacimiento).ToString("dd/MM/yyyy") : "";

            FechaFallecimiento.ID = "FechaFallecimiento" + Beneficiario.numCorrelativo;
            if (Beneficiario.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue())
                FechaFallecimiento.Text = Beneficiario.FechaFallecimiento != null ? Convert.ToDateTime(Beneficiario.FechaFallecimiento).ToString("dd/MM/yyyy") : "";

            PaisOrigen.ID = "PaisOrigen" + Beneficiario.numCorrelativo;
            if (string.IsNullOrEmpty(Beneficiario.PaisOrigen.cod_parametro))
                PaisOrigen.SelectedValue = "604";//Peruano
            else
                PaisOrigen.SelectedValue = Beneficiario.PaisOrigen.cod_parametro;

            CorreoElectronico.ID = "CorreoElectronico" + Beneficiario.numCorrelativo;
            CorreoElectronico.Text = Beneficiario.CorreoElectronico;

            Celular.ID = "Celular" + Beneficiario.numCorrelativo;
            Celular.Text = Beneficiario.numCelular;

            Nacionalidad.ID = "Nacionalidad" + Beneficiario.numCorrelativo;
            if (string.IsNullOrEmpty(Beneficiario.Nacionalidad.cod_nacionalidad))
                Nacionalidad.SelectedValue = "538560003";//Peruano
            else
                Nacionalidad.SelectedValue = Beneficiario.Nacionalidad.cod_nacionalidad;

            VinculoFamiliar.ID = "VinculoFamiliar" + Beneficiario.numCorrelativo;
            VinculoFamiliar.SelectedValue = Beneficiario.VinculoFamiliar.cod_parametro;

            NumeroVinculoFamiliar.ID = "NumeroVinculoFamiliar" + Beneficiario.numCorrelativo;
            NumeroVinculoFamiliar.Text = Beneficiario.NroVinculoFamiliar;

            PersonaHelperRviadm.ID = "PersonaHelperRviadm" + Beneficiario.numCorrelativo;
            if (ExistePersonaRviadm)
                PersonaHelperRviadm.Text = "Persona existente en Sistema Rentas";
            else
                PersonaHelperRviadm.Text = string.Empty;

            ESSALUD.ID = "ESSALUD" + Beneficiario.numCorrelativo;
            if (string.IsNullOrEmpty(Beneficiario.DescuentoESSALUD.cod_parametro))
                ESSALUD.SelectedValue = "01";//por defecto con descuento 4%
            else
                ESSALUD.SelectedValue = Beneficiario.DescuentoESSALUD.cod_parametro;
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

        private void CargarComboboxOtrasEntidades(DropDownList control, string tabla, List<Profesion> profesion, List<Nacionalidad> nacionalidad)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            if (tabla == "profesion")
            {
                foreach (var item in profesion)
                {
                    control.Items.Add(new ListItem(item.gls_profesion, item.cod_profesion));
                }
            }

            if (tabla == "nacionalidad")
            {
                foreach (var item in nacionalidad)
                {
                    control.Items.Add(new ListItem(item.gls_nacionalidad, item.cod_nacionalidad));
                }
            }

        }
    }
}