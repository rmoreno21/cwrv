using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Controles
{
    public partial class DireccionCierreRVI : System.Web.UI.UserControl
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DireccionCierreRVI));
        public List<Parametro> ListaTipoVia { get; set; }
        public JArray ListaDepartamentos { get; set; }
        public JArray ListaProvincias { get; set; }
        public JArray ListaDistritos { get; set; }
        public JArray ListaTipoZona { get; set; }
        public JArray ListaLargaDistancia { get; set; }
        public Direccion DireccionSolicitud { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            CargarInformacionInicialPantalla();
            CargarDireccion();
        }

        private void CargarInformacionInicialPantalla()
        {
            CargarComboboxNuevosDatos(TipoVia, ListaTipoVia);

            //ModDirPrincipal.Items.Add(new ListItem("«Seleccione»", "0"));
            ModDirPrincipal.Items.Add(new ListItem("Sí", Enums.TipoDireccion.Principal.StringValue()));
            //ModDirPrincipal.Items.Add(new ListItem("No", "N"));

            if (ListaDepartamentos != null)
            {
                foreach (var item in ListaDepartamentos)
                {
                    Departamento.Items.Add(new ListItem(item["gls_departamento"].ToString().ToUpper(), item["id_departamento"].ToString().ToLower()));
                }
            }

            if (ListaProvincias != null)
            {
                foreach (var item in ListaProvincias)
                {
                    Provincia.Items.Add(new ListItem(item["gls_provincia"].ToString().ToUpper(), item["id_provincia"].ToString().ToLower()));
                }
            }

            if (ListaDistritos != null)
            {
                foreach (var item in ListaDistritos)
                {
                    Distrito.Items.Add(new ListItem(item["gls_distrito"].ToString().ToUpper(), item["id_distrito"].ToString().ToLower()));
                }
            }

            if (ListaTipoZona != null)
            {
                foreach (var item in ListaTipoZona)
                {
                    TipoZona.Items.Add(new ListItem(item["glosa"].ToString().ToUpper(), item["codigo"].ToString().ToLower()));
                }
            }

            LargaDistancia.Items.Add(new ListItem("Seleccione", "0"));
            if (ListaLargaDistancia != null)
            {
                foreach (var item in ListaLargaDistancia)
                {
                    LargaDistancia.Items.Add(new ListItem(item["glosa"].ToString().ToUpper(), item["codigo"].ToString().ToLower()));
                }
            }
        }

        private void CargarDireccion()
        {
            ModIdDireccion.Value = "0";

            // Tarjeta Dirección de Solicitud
            if (DireccionSolicitud != null)
            {
                ModIdDireccion.Value = Convert.ToString(DireccionSolicitud.Id);

                Direccion.Text = DireccionSolicitud.Glosa;
                EspacioUrbano.Text = DireccionSolicitud.EspacioUrbano;
                TipoVia.SelectedIndex = TipoVia.Items.IndexOf(TipoVia.Items.FindByValue(DireccionSolicitud.TipoVia.Id));
                Departamento.SelectedValue = DireccionSolicitud.Departamento.Id;
                Provincia.SelectedValue = DireccionSolicitud.Ciudad.Id;
                Distrito.SelectedValue = DireccionSolicitud.Comuna.Id;

                NombreVia.Text = DireccionSolicitud.nombreVia;
                NumeroVia.Text = DireccionSolicitud.numeroVia;
                NumeroInterior.Text = DireccionSolicitud.numeroInterior;
                if (DireccionSolicitud.tipoZona != null)
                    TipoZona.SelectedIndex = TipoZona.Items.IndexOf(TipoZona.Items.FindByValue(DireccionSolicitud.tipoZona.Id));
                NombreZona.Text = DireccionSolicitud.nombreZona;
                Referencia.Text = DireccionSolicitud.referencia;
                if (DireccionSolicitud.largaDistancia != null)
                    LargaDistancia.SelectedIndex = LargaDistancia.Items.IndexOf(LargaDistancia.Items.FindByValue(DireccionSolicitud.largaDistancia.Id));
                NumeroDepartamento.Text = DireccionSolicitud.numeroDepartamento;
                Manzana.Text = DireccionSolicitud.manzana;
                NumeroLote.Text = DireccionSolicitud.numeroLote;
                Kilometro.Text = DireccionSolicitud.kilometro;
                Block.Text = DireccionSolicitud.block;
                Etapa.Text = DireccionSolicitud.etapa;
            }

            ModDirPrincipal.SelectedIndex = 0;
        }

        private void CargarComboboxNuevosDatos(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Nombre, item.Id));
            }
        }
        
    }
}