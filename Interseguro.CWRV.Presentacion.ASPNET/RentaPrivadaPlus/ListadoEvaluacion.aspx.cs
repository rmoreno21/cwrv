using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus
{
    public partial class ListadoEvaluacion1 : System.Web.UI.Page
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(ListadoCierrePlus));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ListadoCotizacionesEvaluacion))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ListadoCotizacionesEvaluacion.StringValue()));
                        Response.Redirect("~/Error/Permisos.aspx");
                    }
                }
                catch (ThreadAbortException) { }
                catch (CommunicationException ex)
                {
                    log.Error(String.Format("Error de comunicación: [{0}]", ex.Message), ex);

                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);

                }
            }
        }

        [WebMethod]
        public static string CargarTablaSolicitudesEvaluacion()
        {

            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    log.Debug("Inicio ListadoEvaluacion1.CargarTablaSolicitudesEvaluacion WebMethod");

                    var pagina = new Page();
                    var control = (TablaSolicitudesEvaluacion)pagina.LoadControl("~/Controles/TablaSolicitudesEvaluacion.ascx");


                    servicioCotizador = LocalizadorProxy.ObtenerServicio();

                    List<SolicitudRPPlus> solicitudes = servicioCotizador.ListarSolicitudEvaluacion();
                    log.Info(string.Format("Cantidad de Solicidutes: {0}", solicitudes.Count));

                    var idEstadosRPP = new[] { 4 };
                    var idEstadosPlaft = new[] { 4 };

                    string codRol = (string)HttpContext.Current.Session["RolAzman"];

                    if (codRol == Enums.RolAzman.JefeOperaciones.StringValue()
                        || codRol == Enums.RolAzman.AsistenteOperaciones.StringValue()
                        || codRol == Enums.RolAzman.AsistenteComercial.StringValue())
                    {
                        control.Solicitudes = solicitudes.FindAll(p => idEstadosRPP.Contains(p.CodigoEstado)).ToList();
                    }
                    else if (codRol == Enums.RolAzman.CoordinadorPlaft.StringValue())
                    {
                        control.Solicitudes = solicitudes.FindAll(p => idEstadosPlaft.Contains(p.CodigoEstadoPlaft)).ToList();
                    }
                    else
                    {
                        //control.Solicitudes = solicitudes.ToList();
                        List<SolicitudRPPlus> listSolicitudes = new List<SolicitudRPPlus>();

                        foreach (SolicitudRPPlus solicitudRPP in solicitudes)
                        {
                            if (((List<Agente>)HttpContext.Current.Session["ListaAgentes"]).Any(ag => ag.Id == solicitudRPP.Afiliado.Agente.Id))
                            {
                                //listSolicitudes = new List<SolicitudRPPlus>();
                                listSolicitudes.Add(solicitudRPP);
                            }
                        }

                        control.Solicitudes = listSolicitudes;

                    }

                    for (int i = 0; i < control.Solicitudes.Count; i++)
                    {
                        if (control.Solicitudes[i].CodigoEstadoPlaft != -1)
                        {
                            GrupoFamiliar gru = control.Solicitudes[i].Beneficiarios[0];

                            log.Info(string.Format("Ini. Solicitud{0} Cliente:{1}, Doc.Identidad{2}{3}", control.Solicitudes[i].Id, gru.ApellidosNombres, gru.Identificacion.IdTipo, gru.Identificacion.Numero));
                            JsonCoincidenciaLN jsonCoincidencia = servicioCotizador.ObtenerCoincidenciaLN(gru);
                            if (jsonCoincidencia != null)
                            {
                                if (jsonCoincidencia._meta.status == "SUCCESS")
                                {
                                    control.Solicitudes[i].ListaNegra = jsonCoincidencia.records.LN;
                                    if (jsonCoincidencia.records.LN == 1)
                                        log.Info(string.Format("Lista Negra:SI"));
                                }
                            }
                        }
                    }

                    pagina.Controls.Add(control);

                    string html = "";

                    using (var sw = new StringWriter())
                    {
                        HttpContext.Current.Server.Execute(pagina, sw, false);
                        html = sw.ToString();
                    }

                    log.Debug("Fin ListadoEvaluacion1.CargarTablaSolicitudesEvaluacion WebMethod");

                    return html;

                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}]", ex.Message), ex);
                    log.Debug("Fin ListadoEvaluacion1.CargarTablaSolicitudesEvaluacion WebMethod");

                    throw (ex);
                }
            }

        }

        [WebMethod]
        public static String SessionSolicitud(string solicitud, string fecCotizacion, string cuspp, string nombre)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                log.Debug("Inicio ListadoEvaluacion1.SessionSolicitud WebMethod");
                HttpContext.Current.Session["solicitudEvaluacion"] = solicitud;
                HttpContext.Current.Session["fecCotizacion"] = fecCotizacion;
                HttpContext.Current.Session["cuspp_rpp"] = cuspp;
                HttpContext.Current.Session["nombre_rpp"] = nombre;
                log.Debug("Fin ListadoEvaluacion1.SessionSolicitud WebMethod");
                return "OK";
            }
        }

    }

}