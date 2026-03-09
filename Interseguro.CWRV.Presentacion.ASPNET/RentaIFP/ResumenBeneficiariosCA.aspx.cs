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
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.RentaIFP
{
    public partial class GrupoFamiliarCA : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Cotizador));
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
                            if (Page.PreviousPage != null)
                            {
                                var solicitudSerializado = (HiddenField)Page.PreviousPage.Form.FindControl("Contenido").FindControl("HSolicitudSerializado");
                                if (solicitudSerializado.Value == null)
                                {
                                    Response.Redirect("Cotizador.aspx");
                                }
                                SolicitudIFP objetoSolicitud = Newtonsoft.Json.JsonConvert.DeserializeObject<SolicitudIFP>(solicitudSerializado.Value);
                                if (objetoSolicitud.Cotizaciones == null)
                                {
                                    Response.Redirect("Cotizador.aspx");
                                }

                                HSolicitudSerializado.Value = Newtonsoft.Json.JsonConvert.SerializeObject(objetoSolicitud);
                            }
                            else
                            {
                                Response.Redirect("Cotizador.aspx");
                            }
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
        public static List<string> CargarTablaBeneficiarios(string tokenUsuario, string solicitud)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    List<string> lstReturn = new List<string>();

                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        int indice;
                        var solicitudSerializado = Newtonsoft.Json.JsonConvert.DeserializeObject<SolicitudIFP>(solicitud);
                        List<GrupoFamiliar> lstGrupoFamiliar = solicitudSerializado.Beneficiarios;
                        List<GrupoFamiliar> lstGrupoFamiliarCA = new List<GrupoFamiliar>();

                        if (lstGrupoFamiliar == null)
                            lstGrupoFamiliar = new List<GrupoFamiliar>();

                        List<CoberturaAdicional> lstCoberturaAdicional = solicitudSerializado.CoberturasAdicionales;
                        if (lstGrupoFamiliar.Count == 1 && lstCoberturaAdicional.Count != 0)
                        {
                            //SE CREA EL LISTADO DE BENEFICIARIOS CA
                            lstCoberturaAdicional.FindAll(ca => ca.FechaNacimiento != "").ForEach(ca =>
                            {
                                lstGrupoFamiliarCA.Add(new GrupoFamiliar()
                                {
                                    Parentesco = new Parentesco() { Id = ca.Parentesco },
                                    Sexo = Convert.ToChar(ca.Sexo),
                                    FechaNacimiento = Convert.ToDateTime(ca.FechaNacimiento),
                                    ApellidoPaterno = "",
                                    ApellidoMaterno = "",
                                    Nombre = "",
                                    TipoCobertura = "CA"
                                });
                            });

                            //SE ASIGNA EL % A BENEFICIARIOS CA
                            CotizacionIFP cotizacionIFP = solicitudSerializado.Cotizaciones[0];
                            lstGrupoFamiliarCA.ForEach(b =>
                            {
                                if (cotizacionIFP.ValPjeCACy > 0 && b.Parentesco.Id == Enums.Parentesco.Conyuge.StringValue()) b.ValPjeRentaCA = cotizacionIFP.ValPjeCACy;

                                if (cotizacionIFP.ValPjeCAPa > 0 && b.Parentesco.Id == Enums.Parentesco.Padre.StringValue() && b.Sexo == 'M') b.ValPjeRentaCA = cotizacionIFP.ValPjeCAPa;

                                if (cotizacionIFP.ValPjeCAMa > 0 && b.Parentesco.Id == Enums.Parentesco.Padre.StringValue() && b.Sexo == 'F') b.ValPjeRentaCA = cotizacionIFP.ValPjeCAMa;
                            });

                            //SE QUITAN LOS BENEFICIARIOS CA SIN % DE RENTA
                            lstGrupoFamiliarCA = lstGrupoFamiliarCA.FindAll(b => b.ValPjeRentaCA != 0);

                            //ASIGNA INDICE A BENEFICIARIOS CA
                            indice = 1;
                            lstGrupoFamiliarCA.ForEach(b =>
                            {
                                b.Id = indice;
                                indice++;
                            });
                        }
                        else
                        {
                            lstGrupoFamiliarCA = lstGrupoFamiliar.FindAll(b => b.TipoCobertura == "CA");
                        }

                        servicioCotizador = LocalizadorProxy.ObtenerServicio();
                        List<List<Parametro>> listaCombobox = servicioCotizador.ObtenerCombobox();
                        List<Parametro> lstparametroParentesco = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Parentesco];
                        List<Parametro> lstparametroSexo = (List<Parametro>)listaCombobox[(int)Enums.CategoriaCombobox.Sexo];

                        //lstGrupoFamiliarCA.ForEach(b =>
                        //{
                        //    b.Parentesco.Nombre = lstparametroParentesco.FindAll(x => x.Id == b.Parentesco.Id).FirstOrDefault().Glosa;
                        //});

                        var control = (TablaBeneficiarioIFP_CA)pagina.LoadControl("~/Controles/TablaBeneficiarioIFP_CA.ascx");
                        control.lstBeneficiarios = lstGrupoFamiliarCA;
                        control.lstparametroSexo = lstparametroSexo;
                        control.lstparametroParentesco = lstparametroParentesco;

                        pagina.Controls.Add(control);
                        using (var sw = new StringWriter())
                        {
                            HttpContext.Current.Server.Execute(pagina, sw, false);
                            lstReturn.Add(sw.ToString());
                        }

                        if (lstGrupoFamiliar.Count == 1 && lstCoberturaAdicional.Count != 0)
                        {
                            //AGREGAMOS BENEFICIARIOS CA A LISTA FINAL DE BENEFICIARIOS
                            lstGrupoFamiliar.AddRange(lstGrupoFamiliarCA);

                            //SE RE-ASIGNA INDICE A BENEFICIARIOS
                            indice = 1;
                            lstGrupoFamiliar.ForEach(b =>
                            {
                                b.Id = indice;
                                indice++;
                            });

                            //ACTUALIZAMOS EL HIDDEN CON LOS NUEVOS BENEFICIARIOS CA
                            //solicitudSerializado.Beneficiarios = lstGrupoFamiliar;
                        }

                        lstReturn.Add(Newtonsoft.Json.JsonConvert.SerializeObject(solicitudSerializado));
                    }
                    return lstReturn;
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

        //[WebMethod]
        //public static Respuesta ModificarPorcentaje(string tokenUsuario, int posicion, int porcentaje)
        //{
        //    using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
        //    {

        //        Respuesta respuesta = new Respuesta();

        //        try
        //        {
        //            if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
        //            {
        //                List<GrupoFamiliar> lstGrupoFamiliar = (List<GrupoFamiliar>)HttpContext.Current.Session["ListaGrupoFamiliarCierre"];

        //                if (lstGrupoFamiliar == null)
        //                    lstGrupoFamiliar = new List<GrupoFamiliar>();

        //                lstGrupoFamiliar[posicion - 1].ValPjeRenta = porcentaje;

        //                HttpContext.Current.Session["ListaGrupoFamiliarCierre"] = lstGrupoFamiliar;

        //                double totalPorcentaje = 0;
        //                for (int i = 0; i < lstGrupoFamiliar.Count; i++)
        //                {
        //                    totalPorcentaje += lstGrupoFamiliar[i].ValPjeRenta;
        //                }

        //                respuesta.Estado = "OK";
        //                respuesta.Mensaje = totalPorcentaje.ToString();
        //            }
        //            else
        //            {
        //                log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
        //                respuesta.Estado = Constante.COD_TOKEN;
        //            }

        //            return respuesta;
        //        }
        //        catch (Exception ex)
        //        {
        //            log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}",
        //               ex.Source, ex.Message, ex.StackTrace));
        //            respuesta.Estado = Constante.COD_ERROR;
        //            respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
        //            respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
        //            respuesta.Mensaje = "<div style=\"margin: 5px 0\"><strong>No se ha podido completar el proceso debido al siguiente error:</strong></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
        //            return respuesta;
        //        }
        //    }
        //}
    }
}