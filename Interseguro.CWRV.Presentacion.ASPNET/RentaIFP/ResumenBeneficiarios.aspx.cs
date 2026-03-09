using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
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

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
{
    public partial class ResumenBeneficiarios : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ResumenBeneficiarios));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.CotizacionesIFP))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                            ModGruFamsolicitud.Value = Convert.ToString((Session["Solicitud"]));
                            ModGruNumCorrelativo.Value = Convert.ToString((Session["NumCorrelativo"]));
                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.CotizacionesIFP.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionSeguridad"] });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ex.Message });
                    MCMEstadoIcono.Value = Enums.CuadroMensajeIcono.Error.StringValue();
                    MCMEstadoTitulo.Value = Enums.CuadroMensajeTitulo.Error.StringValue();
                    MCMEstado.Value = "1";
                }
            }

        }

        [WebMethod]
        public static string CargarTablaBeneficiarios(string tokenUsuario)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    string html = "";

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();

                        List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["ListaGrupoFamiliarCierre"];
                        if (lstGrupoFamiliar == null)
                            lstGrupoFamiliar = new List<GrupoFamiliar>();

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                        List<Parametro> lstparametroParentesco = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Parentesco];

                        lstGrupoFamiliar.ForEach(b =>
                        {
                            b.Parentesco.Nombre = lstparametroParentesco.FindAll(x => x.Id == b.Parentesco.Id).FirstOrDefault().Glosa;
                        });

                        var control = (TablaBeneficiarioIFP)pagina.LoadControl("~/Controles/TablaBeneficiarioIFP.ascx");

                        control.lstBeneficiarios = lstGrupoFamiliar;
                        HttpContext.Current.Session["Beneficiarios"] = control.lstBeneficiarios;

                        pagina.Controls.Add(control);

                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            html = sw.ToString();
                        }
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
        public static Respuesta ModificarPorcentaje(string tokenUsuario, int posicion, int porcentaje)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {

                Respuesta respuesta = new Respuesta();

                try
                {

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["ListaGrupoFamiliarCierre"];

                        if (lstGrupoFamiliar == null)
                            lstGrupoFamiliar = new List<GrupoFamiliar>();

                        lstGrupoFamiliar[posicion - 1].ValPjeRenta = porcentaje;

                        HttpContext.Current.Session["ListaGrupoFamiliarCierre"] = lstGrupoFamiliar;

                        double totalPorcentaje = 0;
                        for (int i = 0; i < lstGrupoFamiliar.Count; i++)
                        {
                            totalPorcentaje += lstGrupoFamiliar[i].ValPjeRenta;
                        }

                        respuesta.Estado = "OK";
                        respuesta.Mensaje = totalPorcentaje.ToString();
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                    return respuesta;
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                       ex.Source, ex.Message, ex.StackTrace));
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                    return respuesta;
                }
            }
        }
    }
}