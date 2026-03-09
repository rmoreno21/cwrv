using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.IFP
{
    public partial class Principal : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Principal));
        private static IServicioCWRV servicioCotizador;

        public Afiliado Afiliado { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionesIFP))
                    {
                        // Obtener parámetros desde query string
                        string cuspp = Request.QueryString["c"];
                        string solicitud = Request.QueryString["s"];
                        string tipoDocumento = Request.QueryString["td"];
                        string numeroDocumento = Request.QueryString["nd"];

                        if (!string.IsNullOrEmpty(cuspp))
                        {
                            CargarInformacionInicialPantalla();

                            // Link de búsqueda avanzada
                            BusquedaAvanzada.NavigateUrl = string.Format("{0}&c={1}", BusquedaAvanzada.NavigateUrl, cuspp);

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado("", cuspp, "", "", Enums.TipoProducto.IFP.StringValue());
                            BusquedaCUSPP.Text = cuspp;

                            TextInfo textInfo = new CultureInfo("es-PE", false).TextInfo;
                            TituloNombre.Text = textInfo.ToTitleCase(string.Format("{0} {1}", afiliado.Nombre.ToLower(), afiliado.ApellidoPaterno.ToLower()));

                            CUSPP.Text = cuspp;
                            FechaNacimiento.Text = afiliado.FechaNacimiento.Value.ToString("dd/MM/yyyy");
                            TipoDocumento.SelectedIndex = TipoDocumento.Items.IndexOf(TipoDocumento.Items.FindByValue(afiliado.TipoIdentificacion));
                            NumeroDocumento.Text = afiliado.NumeroIdentificacion;
                            ApellidoPaterno.Text = afiliado.ApellidoPaterno;
                            ApellidoMaterno.Text = afiliado.ApellidoMaterno;
                            Nombres.Text = afiliado.Nombre;
                            Sexo.SelectedIndex = Sexo.Items.IndexOf(Sexo.Items.FindByValue(afiliado.Sexo.ToString()));
                            Telefono.Text = afiliado.Telefonos;
                            Celular.Text = afiliado.Celulares;
                            CorreoElectronico.Text = afiliado.CorreoElectronico;
                            EstadoCivil.SelectedIndex = EstadoCivil.Items.IndexOf(EstadoCivil.Items.FindByValue(afiliado.EstadoCivil.cod_parametro));
                            AFP.SelectedIndex = AFP.Items.IndexOf(AFP.Items.FindByValue(afiliado.AFP.Id));
                            CentroLaboral.Text = afiliado.CentroLaboral;
                            CIC.Text = afiliado.SaldoCIC.Value.ToString("N", CultureInfo.InvariantCulture);
                            RangoInversion.Text = afiliado.RangoInversion;
                            Agente.Text = string.Format("{0} - {1}", afiliado.Agente.Id, afiliado.Agente.Nombre);

                            // Direcciones
                            List<Direccion> direcciones = servicioCotizador.ListarDireccion(cuspp);
                            TablaDirecciones.DataSource = direcciones;
                            TablaDirecciones.DataBind();

                            // Grupo Familiar
                            List<GrupoFamiliar> beneficiarios = servicioCotizador.ListarGrupoFamiliar(cuspp);
                            TablaGrupoFamiliar.DataSource = beneficiarios;
                            TablaGrupoFamiliar.DataBind();

                            DatosPersona.Visible = true;
                        }
                        else if (!string.IsNullOrEmpty(solicitud))
                        {
                            // Link de búsqueda avanzada
                            BusquedaAvanzada.NavigateUrl = string.Format("{0}&s={1}", BusquedaAvanzada.NavigateUrl, solicitud);
                        }
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    Response.Redirect("~/Error/500.aspx");
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    Response.Redirect("~/Error/500.aspx");
                }
            }
        }

        // Evento de grillas
        protected void TablaGrupoFamiliar_RowDataBound(object sender, System.Web.UI.WebControls.GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
            }
        }

        private void CargarInformacionInicialPantalla()
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio();
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
            List<EstadoCivil> estadosCiviles = servicioCotizador.ListarEstadoCivil(Session["Usuario"].ToString());

            CargarCombobox(TipoDocumento, listaCombobox[(int)Enums.CategoriaCombobox.Identificacion]);
            CargarCombobox(AFP, listaCombobox[(int)Enums.CategoriaCombobox.Afp]);
            CargarComboboxEstadoCivil(EstadoCivil, estadosCiviles);
            CargarComboboxSexo();
        }

        private void CargarCombobox(DropDownList control, List<Parametro> parametros)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (Parametro parametro in parametros)
            {
                control.Items.Add(new ListItem(parametro.Glosa ?? parametro.Nombre, parametro.Id));
            }
        }

        private void CargarComboboxEstadoCivil(DropDownList control, List<EstadoCivil> estadosCiviles)
        {
            control.Items.Clear();
            control.Items.Add(new ListItem("«Seleccione»", "0"));
            foreach (var estadoCivil in estadosCiviles)
            {
                control.Items.Add(new ListItem(estadoCivil.gls_estado_civil, estadoCivil.cod_estado_civil));
            }
        }

        private void CargarComboboxSexo()
        {
            Sexo.Items.Add(new ListItem("«Seleccione»", "0"));
            Sexo.Items.Add(new ListItem("Femenino", "F"));
            Sexo.Items.Add(new ListItem("Masculino", "M"));
        }
    }
}