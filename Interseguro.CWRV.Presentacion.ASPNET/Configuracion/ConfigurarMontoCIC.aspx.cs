using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using System.Net;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using System.Globalization;

namespace Interseguro.CWRV.Presentacion.ASPNET.Configuracion
{
    public partial class ConfigurarMontoCIC : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigurarMontoCIC));
        private static IServicioCWRV servicioCotizador;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ConfiguracionMontoCIC))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                    //        CargarInformacionInicialPantalla();
                    //        LimpiarFormularios();
                    //        CargarInformacionPredeterminada();
                            CargarInformacionInicial();
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ConfiguracionMontoCIC.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }
        }

        private void CargarInformacionInicial() 
        {
            servicioCotizador = LocalizadorProxy.ObtenerServicio(); 
            List<MontoCIC> montos  = servicioCotizador.ListarMontoCIC();

            montos.ForEach(item=>ListaMontos.Items.Add(new ListItem{Value=item.Id.ToString(),Text=string.Format("{0:0,0.00}",item.Valor)}));

            PerNuevoMonto.Value = Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.MontoCICInsertar)? "1":"0";
            PerEditarMonto.Value = Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.MontoCICActualizar) ? "1" : "0";
            PerEliminarMonto.Value = Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.MontoCICEliminar) ? "1" : "0";

            //<SOLINI25621>
            PerNuevoContrato.Value = Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ContratoCotizacionInsertar) ? "1" : "0";
            PerEditarContrato.Value = Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ContratoCotizacionActualizar) ? "1" : "0";
            PerEliminarContrato.Value = Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ContratoCotizacionEliminar) ? "1" : "0";

            List<Parametro> parametro = new List<Parametro>();
            parametro = servicioCotizador.ObtenerParametrosPorTabla("AUMENTO");
            if (parametro.Count>0 )
                PensionPorcen.Text = parametro[0].Valor_1;

            //<SOLFIN25621>

            //<GTI.INI-29372>
            List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
            CargarCombobox(ddlAFPContrato, (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Afp]);
            //<GTI.FIN-29372>

            //<GTI.INI-40661>
            parametro = new List<Parametro>();
            parametro = servicioCotizador.ObtenerParametrosPorTabla("INDLOGS");
            if (parametro.Count > 0)
            {
                ddlLogCotizacion.SelectedIndex = parametro.FindAll(p => p.Correlativo == 1)[0].Valor_1 == "S" ? 0 : 1;
                ddlLogReserva.SelectedIndex = parametro.FindAll(p => p.Correlativo == 2)[0].Valor_1 == "S" ? 0 : 1;
            }

            PerGrabarLogCotizacion.Value = Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.LogCotizacionGrabar) ? "1" : "0";
            PerGrabarLogReserva.Value = Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.LogReservaGrabar) ? "1" : "0";
            //<GTI.FIN-40661>
        }

        [WebMethod]
        public static Respuesta RegistrarMontoCIC(string tokenUsuario, string valorCIC)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        MontoCIC config = new MontoCIC();

                        config.Valor = Convert.ToDecimal(valorCIC);
                        config.UsuarioCreacion = (string)HttpContext.Current.Session["Usuario"];

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.RegistrarMontoCIC(config);
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        
        }

        [WebMethod]
        public static Respuesta ActualizarMontoCIC(string tokenUsuario,string codigo ,string valorCIC)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        MontoCIC config = new MontoCIC();

                        config.Id = Convert.ToInt32(codigo); 
                        config.Valor = Convert.ToDecimal(valorCIC);
                        config.UsuarioModificacion  = (string)HttpContext.Current.Session["Usuario"];

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.ActualizarMontoCIC(config);
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }

        }

        [WebMethod]
        public static Respuesta EliminarMontoCIC(string tokenUsuario, string codigo)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        MontoCIC config = new MontoCIC();

                        config.Id = Convert.ToInt32(codigo);

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.EliminarMontoCIC(config);
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }

        }

        [WebMethod]
        public static Respuesta CargarListadoMonto(string tokenUsuario)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<MontoCIC> montos = servicioCotizador.ListarMontoCIC();

                        string html = "";
                        foreach (var m in montos)
                        {
                            html += String.Format("<option value='{0}'>{1}</option>", m.Id,string.Format("{0:0,0.00}" ,m.Valor));
                        }

                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Contenido = html;
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        
        }


        [WebMethod]
        public static Respuesta ActualizarAumentoPorcentaje(string tokenUsuario,  string valor1)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        Parametro parametro = new Parametro();
                        parametro.Id = "AUMENTO";
                        parametro.Correlativo = 1;
                        parametro.Valor_1 = valor1;
                        parametro.UsuarioModificacion = (string)HttpContext.Current.Session["Usuario"];

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.ActualizarParametro(parametro);
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }

        }

        //<GTI.INI-29372>
        [WebMethod]
        public static string CargarListadoContratos(string tokenUsuario)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    var pagina = new Page();
                    var control = (TablaContratos)pagina.LoadControl("~/Controles/TablaContratos.ascx");

                    string usuario = (string)HttpContext.Current.Session["Usuario"];
                    servicioCotizador = LocalizadorProxy.ObtenerServicio();
                    List<Contrato> contratos = servicioCotizador.ListarContratosCotizaciones(usuario);

                    control.Contratos = contratos;

                    pagina.Controls.Add(control);

                    string html = "";
                    using (var sw = new StringWriter())
                    {
                        HttpContext.Current.Server.Execute(pagina, sw, false);
                        html = sw.ToString();
                    }
                    return html;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Respuesta RegistrarContratoCotizacion(string tokenUsuario, string glsContrato, string fechaInicio, string fechaFin, string codAFP)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        Contrato contrato = new Contrato();

                        contrato.GlsContratoCotizacion = glsContrato;
                        contrato.FecInicio = Convert.ToDateTime(fechaInicio, new CultureInfo("es-PE"));
                        contrato.FecFin = Convert.ToDateTime(fechaFin, new CultureInfo("es-PE"));
                        contrato.CodAfp = codAFP;
                        contrato.UsuarioCreacion = (string)HttpContext.Current.Session["Usuario"];

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.InsertarContratoCotizacion(contrato);
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Respuesta ModificarContratoCotizacion(string tokenUsuario, int idContrato, string glsContrato, string fechaInicio, string fechaFin, string codAFP)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        Contrato contrato = new Contrato();

                        contrato.IdContratoCotizacion = idContrato;
                        contrato.GlsContratoCotizacion = glsContrato;
                        contrato.FecInicio = Convert.ToDateTime(fechaInicio, new CultureInfo("es-PE"));
                        contrato.FecFin = Convert.ToDateTime(fechaFin, new CultureInfo("es-PE"));
                        contrato.CodAfp = codAFP;
                        contrato.UsuarioModificacion = (string)HttpContext.Current.Session["Usuario"];

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.ActualizarContratoCotizacion(contrato);
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Respuesta EliminarContratoCotizacion(string tokenUsuario, int idContrato)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.EliminarContratoCotizacion(idContrato, (string)HttpContext.Current.Session["Usuario"]);
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
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

        //<GTI.FIN-29372>

        //<GTI.INI-40661>
        [WebMethod]
        public static Respuesta GrabarLogCotizacion(string tokenUsuario, string valor1)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        Parametro parametro = new Parametro();
                        parametro.Id = "INDLOGS";
                        parametro.Correlativo = 1;
                        parametro.Valor_1 = valor1;
                        parametro.UsuarioModificacion = (string)HttpContext.Current.Session["Usuario"];

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.ActualizarParametro(parametro);
                    }

                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }

        [WebMethod]
        public static Respuesta GrabarLogReserva(string tokenUsuario, string valor1)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    Respuesta respuesta = new Respuesta();
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        Parametro parametro = new Parametro();
                        parametro.Id = "INDLOGS";
                        parametro.Correlativo = 2;
                        parametro.Valor_1 = valor1;
                        parametro.UsuarioModificacion = (string)HttpContext.Current.Session["Usuario"];

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        respuesta = servicioCotizador.ActualizarParametro(parametro);
                    }

                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.Source, ex.Message, ex.StackTrace));
                    if (ex.InnerException != null)
                    {
                        log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                            ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                    }
                    throw (ex);
                }
            }
        }
        //<GTI.FIN-40661>

    }
}