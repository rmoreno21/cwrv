using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;
using Interseguro.CWRV.Infraestructura.General;
using System.Threading;
using System.ServiceModel;
using System.Configuration;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using log4net;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Dominio.Entidades;
using System.Web.Services;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using OfficeOpenXml;

namespace Interseguro.CWRV.Presentacion.ASPNET.Reportes
{
    public partial class ConsolidadoCumplimientoCdA : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ConsolidadoCumplimientoCdA));
        private static IServicioCWRV servicioCotizador;
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.ConsolidadoCumplimientoCdA))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));

                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.ConsolidadoCumplimientoCdA.StringValue()));
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
                    MCMMensaje.Text = Utilitarios.FormatearError(new List<String> { ConfigurationManager.AppSettings["ExcepcionComunicacionCotizador"] });
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

        protected void BtnGenerarExcel_Click(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.ConsolidadoCumplimientoCdA))
                    {
                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        string usuario = (string)HttpContext.Current.Session["Usuario"];

                        List<AgenteConsentimiento> lstAgenteConsentimiento = new List<AgenteConsentimiento>();
                        List<AgenteConsentimiento> lstAgenteConsentimientoSuperv = new List<AgenteConsentimiento>();
                        List<AgenteConsentimiento> lstAgenteConsentimientoJefes = new List<AgenteConsentimiento>();
                        List<AgenteConsentimiento> lstsupervJefes = new List<AgenteConsentimiento>();
                        
                        List<Agente> listaAgentes = servicioCotizador.ListarAgente(string.Empty, Enums.RolAzman.JefeOperaciones.StringValue());
                        List<ConsentimientosAgrupadosAge> lstConsentimientosAgrupadosAge = servicioCotizador.ListarConsentimientosAgrupadosAge("202003", usuario);

                        List<ConsentimientosAgrupadosAge> lstConsentimientosNoDev = lstConsentimientosAgrupadosAge.FindAll(cnt => cnt.ind_consentimiento == "N");
                        List<ConsentimientosAgrupadosAge> lstConsentimientosDevEntPlz = lstConsentimientosAgrupadosAge.FindAll(cnt => cnt.ind_consentimiento == "S" && cnt.entrega_plazo == 1);
                        List<ConsentimientosAgrupadosAge> lstConsentimientosDevFurPlz = lstConsentimientosAgrupadosAge.FindAll(cnt => cnt.ind_consentimiento == "S" && cnt.entrega_plazo == 0);

                        foreach (var itemAgente in listaAgentes)
                        {
                            AgenteConsentimiento agenteConsentimiento = new AgenteConsentimiento();
                            bool bol_agregar = false;

                            ConsentimientosAgrupadosAge consentimientosAgrupadosAgeNoDev = new ConsentimientosAgrupadosAge();
                            consentimientosAgrupadosAgeNoDev = lstConsentimientosNoDev.Find(cnt => cnt.num_agente.ToString() == itemAgente.Id);

                            if (consentimientosAgrupadosAgeNoDev != null)
                            {
                                agenteConsentimiento.supervisor = consentimientosAgrupadosAgeNoDev.gls_nombres_agente;
                                agenteConsentimiento.padre = itemAgente.IdPadre;

                                agenteConsentimiento.noDevuelto = consentimientosAgrupadosAgeNoDev.cantidad_ind_consentimiento;
                                //agenteConsentimiento.PorcNoDevuelto = 0;

                                bol_agregar = true;
                            }

                            ConsentimientosAgrupadosAge consentimientosAgrupadosAgeDevEntPlz = new ConsentimientosAgrupadosAge();
                            consentimientosAgrupadosAgeDevEntPlz = lstConsentimientosDevEntPlz.Find(cnt => cnt.num_agente.ToString() == itemAgente.Id);

                            if (consentimientosAgrupadosAgeDevEntPlz != null)
                            {
                                agenteConsentimiento.supervisor = consentimientosAgrupadosAgeDevEntPlz.gls_nombres_agente;
                                agenteConsentimiento.padre = itemAgente.IdPadre;

                                agenteConsentimiento.entregPlazo = consentimientosAgrupadosAgeDevEntPlz.cantidad_ind_consentimiento;
                                //agenteConsentimiento.PorcentregPlazo = 0;

                                bol_agregar = true;
                            }

                            ConsentimientosAgrupadosAge consentimientosAgrupadosAgeDevFurPlz = new ConsentimientosAgrupadosAge();
                            consentimientosAgrupadosAgeDevFurPlz = lstConsentimientosDevFurPlz.Find(cnt => cnt.num_agente.ToString() == itemAgente.Id);

                            if (consentimientosAgrupadosAgeDevFurPlz != null)
                            {
                                agenteConsentimiento.supervisor = consentimientosAgrupadosAgeDevFurPlz.gls_nombres_agente;
                                agenteConsentimiento.padre = itemAgente.IdPadre;

                                agenteConsentimiento.devuelta = consentimientosAgrupadosAgeDevFurPlz.cantidad_ind_consentimiento;
                                //agenteConsentimiento.PorcDevuelta = 0;

                                bol_agregar = true;
                            }

                            //agenteConsentimiento.total = 0;
                            //agenteConsentimiento.PorcTotal = 0;

                            if (bol_agregar)
                            {
                                lstAgenteConsentimiento.Add(agenteConsentimiento);
                            }

                        }

                        log.Debug(JsonConvert.SerializeObject(lstAgenteConsentimiento));
                        
                        lstAgenteConsentimientoSuperv = lstAgenteConsentimiento.GroupBy(ac => ac.padre).Select(ac =>
                                                                new AgenteConsentimiento
                                                                {
                                                                    supervisor = listaAgentes.Find(sp => sp.Id == ac.First().padre).Nombre
                                                                        ,entregPlazo = ac.Sum(ep => ep.entregPlazo)
                                                                        ,noDevuelto = ac.Sum(ep => ep.noDevuelto)
                                                                        ,devuelta = ac.Sum(ep => ep.devuelta)
                                                                        ,total = ac.Sum(ep => ep.entregPlazo) + ac.Sum(ep => ep.noDevuelto) + ac.Sum(ep => ep.devuelta)
                                                                        //,porcentregPlazo = (ac.Sum(ep => ep.entregPlazo)) / (ac.Sum(ep => ep.entregPlazo) + ac.Sum(ep => ep.noDevuelto) + ac.Sum(ep => ep.devuelta))
                                                                        //,porcNoDevuelto = (ac.Sum(ep => ep.noDevuelto)) / (ac.Sum(ep => ep.entregPlazo) + ac.Sum(ep => ep.noDevuelto) + ac.Sum(ep => ep.devuelta))
                                                                        //,porcDevuelta = (ac.Sum(ep => ep.devuelta)) / (ac.Sum(ep => ep.entregPlazo) + ac.Sum(ep => ep.noDevuelto) + ac.Sum(ep => ep.devuelta))
                                                                        ,porcTotal = 100
                                                                        ,padre = listaAgentes.Find(sp => sp.Id == ac.First().padre).IdPadre == null ? "0" : listaAgentes.Find(sp => sp.Id == ac.First().padre).IdPadre
                                                                }).ToList();

                        //comntar que es
                        log.Debug(JsonConvert.SerializeObject(lstAgenteConsentimientoSuperv));

                        lstAgenteConsentimientoJefes = lstAgenteConsentimientoSuperv.GroupBy(ac => ac.padre).Select(ac =>
                                                                new AgenteConsentimiento
                                                                {
                                                                    supervisor = listaAgentes.Find(sp => sp.Id == ac.First().padre) == null ? string.Empty : listaAgentes.Find(sp => sp.Id == ac.First().padre).Nombre
                                                                    ,entregPlazo = ac.Sum(ep => ep.entregPlazo)
                                                                    ,noDevuelto = ac.Sum(ep => ep.noDevuelto)
                                                                    ,devuelta = ac.Sum(ep => ep.devuelta)
                                                                    ,total = ac.Sum(ep => ep.entregPlazo) + ac.Sum(ep => ep.noDevuelto) + ac.Sum(ep => ep.devuelta)
                                                                    //,porcentregPlazo = (ac.Sum(ep => ep.entregPlazo)) / (ac.Sum(ep => ep.entregPlazo) + ac.Sum(ep => ep.noDevuelto) + ac.Sum(ep => ep.devuelta))
                                                                    //,porcNoDevuelto = (ac.Sum(ep => ep.noDevuelto)) / (ac.Sum(ep => ep.entregPlazo) + ac.Sum(ep => ep.noDevuelto) + ac.Sum(ep => ep.devuelta))
                                                                    //,porcDevuelta = (ac.Sum(ep => ep.devuelta)) / (ac.Sum(ep => ep.entregPlazo) + ac.Sum(ep => ep.noDevuelto) + ac.Sum(ep => ep.devuelta))
                                                                    ,porcTotal = 100
                                                                    ,padre = listaAgentes.Find(sp => sp.Id == ac.First().padre) == null ? "0" : listaAgentes.Find(sp => sp.Id == ac.First().padre).Id
                                                                }).ToList();

                        log.Debug(JsonConvert.SerializeObject(lstAgenteConsentimientoJefes));

                        foreach (var itemJefes in lstAgenteConsentimientoJefes)
                        {
                            var totalJefes = itemJefes.entregPlazo + itemJefes.noDevuelto + itemJefes.devuelta;
                            itemJefes.porcentregPlazo = (itemJefes.entregPlazo / totalJefes) * 100;
                            itemJefes.porcNoDevuelto = (itemJefes.porcNoDevuelto / totalJefes) * 100;
                            itemJefes.porcDevuelta = (itemJefes.porcDevuelta / totalJefes) * 100;
                            itemJefes.nivel = 1;

                            lstsupervJefes.Add(itemJefes);

                            var lstDesagrupada = lstAgenteConsentimientoSuperv.FindAll(a => a.padre == itemJefes.padre).OrderBy(a => a.supervisor).ToList();

                            foreach (var itemSuperv in lstDesagrupada)
                            {
                                var total = itemSuperv.entregPlazo + itemSuperv.noDevuelto + itemSuperv.devuelta;
                                itemSuperv.porcentregPlazo = (itemSuperv.entregPlazo / total) * 100;
                                itemSuperv.porcNoDevuelto = (itemSuperv.porcNoDevuelto / total) * 100;
                                itemSuperv.porcDevuelta = (itemSuperv.porcDevuelta / total) * 100;
                                itemSuperv.nivel = 2;
                                
                                lstsupervJefes.Add(itemSuperv);
                            }
                        }

                        //var res4 = Newtonsoft.Json.JsonConvert.SerializeObject(lstsupervJefes);

                        byte[] archivoExcel = null;
                        string ruta_plantilla = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\Plantilla\\RVI\\Reporte\\reporte_CdA.xlsx";
                        log.Info("Ruta Plantilla:" + ruta_plantilla);

                        var ruta = new FileInfo(ruta_plantilla);
                        ExcelPackage wbReporteConsentimientoAgrupado = new ExcelPackage(ruta);
                        ExcelWorksheet wsReporteConsentimientoAgrupado = wbReporteConsentimientoAgrupado.Workbook.Worksheets["Reporte"];

                        int fila = 9;

                        foreach (var item in lstsupervJefes)
                        {
                            if (item.nivel == 1)
                            {
                                wsReporteConsentimientoAgrupado.Cells[string.Format("B{0}", fila)].Style.Font.Bold = true;
                                wsReporteConsentimientoAgrupado.Cells[string.Format("C{0}", fila)].Style.Font.Bold = true;
                                wsReporteConsentimientoAgrupado.Cells[string.Format("D{0}", fila)].Style.Font.Bold = true;
                                wsReporteConsentimientoAgrupado.Cells[string.Format("E{0}", fila)].Style.Font.Bold = true;
                                wsReporteConsentimientoAgrupado.Cells[string.Format("F{0}", fila)].Style.Font.Bold = true;
                                wsReporteConsentimientoAgrupado.Cells[string.Format("G{0}", fila)].Style.Font.Bold = true;
                                wsReporteConsentimientoAgrupado.Cells[string.Format("H{0}", fila)].Style.Font.Bold = true;
                                wsReporteConsentimientoAgrupado.Cells[string.Format("I{0}", fila)].Style.Font.Bold = true;
                                wsReporteConsentimientoAgrupado.Cells[string.Format("J{0}", fila)].Style.Font.Bold = true;
                            }
                            wsReporteConsentimientoAgrupado.Cells[string.Format("B{0}", fila)].Value = item.supervisor;
                            wsReporteConsentimientoAgrupado.Cells[string.Format("C{0}", fila)].Value = item.entregPlazo;
                            wsReporteConsentimientoAgrupado.Cells[string.Format("D{0}", fila)].Value = item.porcentregPlazo;
                            wsReporteConsentimientoAgrupado.Cells[string.Format("E{0}", fila)].Value = item.noDevuelto;
                            wsReporteConsentimientoAgrupado.Cells[string.Format("F{0}", fila)].Value = item.porcNoDevuelto;
                            wsReporteConsentimientoAgrupado.Cells[string.Format("G{0}", fila)].Value = item.devuelta;
                            wsReporteConsentimientoAgrupado.Cells[string.Format("H{0}", fila)].Value = item.porcDevuelta;
                            wsReporteConsentimientoAgrupado.Cells[string.Format("I{0}", fila)].Value = item.total;
                            wsReporteConsentimientoAgrupado.Cells[string.Format("J{0}", fila)].Value = item.porcTotal;
                            
                            fila += 1;
                        }
                        
                        using (MemoryStream m = new MemoryStream())
                        {
                            wbReporteConsentimientoAgrupado.SaveAs(m);
                            archivoExcel = m.ToArray();

                            Response.ContentType = "application/vnd.ms-excel";
                            Response.AddHeader("Content-Disposition", string.Format("attachment;filename={0}", "Reporte_CdA.xlsx"));
                            Response.Clear();
                            Response.BinaryWrite(m.GetBuffer());
                            Response.End();

                            m.Close();
                        }

                        //this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        //this.Response.AddHeader("content-disposition", string.Format("attachment;  filename={0}", "ExcellData.xlsx"));
                        //this.Response.BinaryWrite(archivoExcel);
                    
                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        respuesta.Estado = Constante.COD_TOKEN;
                    }

                }
                catch (ThreadAbortException) { }
                catch (FaultException ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<String> { "No se ha podido completar el proceso debido al siguiente error:" + ex.Message });
                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace), ex);

                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                }

            } //end using
        } //wnd void

    }
}