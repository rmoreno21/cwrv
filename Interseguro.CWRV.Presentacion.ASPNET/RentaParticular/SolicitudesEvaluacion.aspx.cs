using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaParticular
{
    public partial class SolicitudesEvaluacion : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SolicitudesEvaluacion));
        private static IServicioCWRV servicioCotizador;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ListadoCotizacionesEvaluacion))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(string.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));

                            // Cargar tabla de solicitudes en evaluación
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<SolicitudRPPlus> solicitudes = servicioCotizador.ListarSolicitudEvaluacion();
                            log.Info(string.Format("Cantidad de Solicidutes: {0}", solicitudes.Count));

                            var idEstadosRPP = new[] { 4 };
                            var idEstadosPlaft = new[] { 4 };

                            string rolAzman = Session["RolAzman"].ToString();

                            if (Utilitarios.EsRolOperaciones(rolAzman) || rolAzman == Enums.RolAzman.AsistenteComercial.StringValue())
                            {
                                solicitudes = solicitudes.FindAll(s => idEstadosRPP.Contains(s.CodigoEstado)).ToList();
                                TablaSolicitudesEvaluacionRP.Columns[8].Visible = false;
                            }
                            else if (rolAzman == Enums.RolAzman.CoordinadorPlaft.StringValue())
                            {
                                solicitudes = solicitudes.FindAll(s => idEstadosPlaft.Contains(s.CodigoEstadoPlaft)).ToList();
                                TablaSolicitudesEvaluacionRP.Columns[8].Visible = true;
                            }
                            else
                            {
                                List<SolicitudRPPlus> listSolicitudes = new List<SolicitudRPPlus>();

                                foreach (SolicitudRPPlus solicitudRPP in solicitudes)
                                {
                                    if (((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == solicitudRPP.Afiliado.Agente.Id))
                                    {
                                        listSolicitudes.Add(solicitudRPP);
                                    }
                                }
                            }

                            for (int i = 0; i < solicitudes.Count; i++)
                            {
                                if (solicitudes[i].CodigoEstadoPlaft != -1)
                                {
                                    GrupoFamiliar beneficiario = solicitudes[i].Beneficiarios[0];

                                    log.Debug(string.Format("Solicitud [{0}] Cliente [{1}], Doc.Identidad [{2}][{3}]", solicitudes[i].Id, beneficiario.ApellidosNombres, beneficiario.Identificacion.IdTipo, beneficiario.Identificacion.Numero));
                                    JsonCoincidenciaLN jsonCoincidencia = servicioCotizador.ObtenerCoincidenciaLN(beneficiario);
                                    if (jsonCoincidencia != null)
                                    {
                                        if (jsonCoincidencia._meta.status == "SUCCESS")
                                        {
                                            solicitudes[i].ListaNegra = jsonCoincidencia.records.LN;
                                            if (jsonCoincidencia.records.LN == 1)
                                                log.Info(string.Format("Lista Negra:SI"));
                                        }
                                    }
                                }
                            }

                            TablaSolicitudesEvaluacionRP.DataSource = solicitudes;
                            TablaSolicitudesEvaluacionRP.DataBind();
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].", Enums.OpcionesSistema.ListadoCotizacionesEvaluacion.StringValue()));
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
    }
}