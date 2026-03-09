using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.Configuracion
{
    public partial class ParametrosRentas : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ParametrosRentas));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ParametrosRentas))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(string.Format("Usuario accedió a la opción [Consultar Parámetros Rentas]"));
                            {
                                CargarInformacionInicialPantalla();
                            }
                        }
                    }
                    else
                    {
                        log.Warn(
                                String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.ParametrosRentas.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(string.Format("Error de comunicación: [{0}]", ex.Message), ex);
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                }
            }
        }

        private void CargarInformacionInicialPantalla()
        {
            string usuario = Session["Usuario"].ToString();

            // Colocar por default la fecha de hoy
            Fecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // Cargar los parámetros para obtener las monedas únicamente si no han sido cargados antes
            List<List<Parametro>> listaCombobox = null;
            if (Session["ListaCombobox"] == null)
            {
                servicioCotizador = LocalizadorProxy.ObtenerServicio();
                listaCombobox = servicioCotizador.ObtenerCombobox();
                Session["ListaCombobox"] = listaCombobox;
            }
            else
            {
                listaCombobox = (List<List<Parametro>>)Session["ListaCombobox"];
            }

            // Monedas
            List<Parametro> monedas = new List<Parametro>()
            {
                new Parametro
                {
                    Id = "-",
                    Glosa = "Todas"
                }
            };
            monedas.AddRange(listaCombobox[(int)Enums.CategoriaCombobox.Moneda]);
            CargarCombobox(Moneda, monedas);

            // Productos
            CargarCombobox(Producto, listaCombobox[(int)Enums.CategoriaCombobox.ProductosRentas]);
            CargarCombobox(ProductoInsertar, listaCombobox[(int)Enums.CategoriaCombobox.ProductosRentas]);

            // Parámetros de Rentas
            CargarComboboxId(ParametroRentas, listaCombobox[(int)Enums.CategoriaCombobox.ParametrosRentas].Where(x => x.Id == "CMOC" || x.Id == "CMOR").ToList());
            CargarComboboxId(ParametroRentasInsertar, listaCombobox[(int)Enums.CategoriaCombobox.ParametrosRentas].Where(x => x.Id == "CMOC" || x.Id == "CMOR").ToList());
        }

        [WebMethod]
        public static List<ValPar> ListarParametros(string tokenUsuario, string producto, string fecha, string parametro, string moneda)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        // Setear variables
                        string usuario = HttpContext.Current.Session["Usuario"].ToString();
                        DateTime dFecha = Convert.ToDateTime(fecha, new CultureInfo("es-PE"));

                        // Llamar al API del CWRV para obtener los parámetros
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<ValPar> parametros = servicioCotizador.ListarParametrosValPar(producto, dFecha, parametro, usuario);

                        return parametros;
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw ex;
                }
            }
        }

        [WebMethod]
        public static string CargarTablaParametros(string tokenUsuario, string producto, string moneda, List<ValPar> parametros)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        // Setear variables
                        string usuario = HttpContext.Current.Session["Usuario"].ToString();
                        List<List<Parametro>> listaCombobox = (List<List<Parametro>>)HttpContext.Current.Session["ListaCombobox"];

                        // Si la moneda no es - filtrar por código de moneda
                        if (moneda != "-")
                        {
                            parametros = parametros.FindAll(p => p.cod_moneda == moneda);
                        }

                        // Cargar el control de usuario
                        var pagina = new Page();
                        var control = (TablaParametrosRentas)pagina.LoadControl("~/Controles/TablaParametrosRentas.ascx");

                        // Setear la lista de parámetros y el producto
                        control.Parametros = parametros;
                        control.Producto = producto;
                        control.Monedas = listaCombobox[(int)Enums.CategoriaCombobox.Moneda];
                        control.Temporalidades = listaCombobox[(int)Enums.CategoriaCombobox.Temporalidad];
                        control.Origenes = listaCombobox[(int)Enums.CategoriaCombobox.Origen];
                        control.TiposPension = listaCombobox[(int)Enums.CategoriaCombobox.Prestacion];
                        control.Departamentos = listaCombobox[(int)Enums.CategoriaCombobox.Departamento];

                        // Renderizar el control de usuario
                        pagina.Controls.Add(control);
                        string html = string.Empty;
                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            html = sw.ToString();
                        }

                        return html;
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw ex;
                }
            }
        }

        [WebMethod]
        public static List<ValPar> AgregarParametro(string tokenUsuario, List<ValPar> parametros)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        DateTime fechaInicio = parametros[0].fec_ini_rango;
                        DateTime fechaFin = parametros[0].fec_fin_rango;
                        string codigoParametro = parametros[0].cod_parametro;
                        parametros.Add(new ValPar
                        {
                            cod_parametro = codigoParametro,
                            fec_ini_rango = fechaInicio,
                            fec_fin_rango = fechaFin,
                            cod_moneda = "001",
                            num_tramo = 0,
                            val_parametro = 0,
                            cod_tipo_temporalidad = "-",
                            ind_origen = "-",
                            cod_tipo_pension = "-",
                            cod_departamento = "-"
                        });
                        if (parametros[0].cod_moneda == null) parametros.RemoveAt(0);
                        return parametros;
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw ex;
                }
            }
        }

        [WebMethod]
        public static bool ActualizarParametro(string tokenUsuario, string producto, List<ValPar> parametros)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        string usuario = HttpContext.Current.Session["Usuario"].ToString();

                        var respuesa = servicioCotizador.ProcesarParametrosvalPar(producto, parametros, usuario);
                        return true;
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    throw ex;
                }
            }
        }

        private void CargarCombobox(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem(item.Glosa, item.Id));
            }
        }

        private void CargarComboboxId(DropDownList control, List<Parametro> combobox)
        {
            control.Items.Clear();
            foreach (Parametro item in combobox)
            {
                control.Items.Add(new ListItem($"{item.Id}-{item.Glosa}", item.Id));
            }
        }
    }
}