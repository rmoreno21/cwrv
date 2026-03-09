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
    public partial class DashboardCntoFD : System.Web.UI.Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DashboardCntoFD));
        private static IServicioCWRV servicioCotizador;
        protected void Page_Load(object sender, EventArgs e)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.DashboardCntoFD))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(String.Format("Usuario accedió a la opción [{0}].", Request.Url.AbsolutePath));

                            List<List<Agente>> lista = new List<List<Agente>>();

                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<Agente> listaTotalAgentes = servicioCotizador.ListarAgente(string.Empty, Enums.RolAzman.JefeOperaciones.StringValue());

                            List<Agente> listaJefes = listaTotalAgentes.FindAll(jf => jf.IdNivel == 1).Select(jf => new Agente { Id = jf.Id, Nombre = jf.Nombre, IdPadre = jf.IdPadre, IdNivel = jf.IdNivel, Usuario = jf.Usuario }).ToList();
                            List<Agente> listaSupervisores = listaTotalAgentes.FindAll(jf => jf.IdNivel == 2 && jf.IdPadre != null).Select(jf => new Agente { Id = jf.Id, Nombre = jf.Nombre, IdPadre = jf.IdPadre, IdNivel = jf.IdNivel, Usuario = jf.Usuario }).ToList();
                            List<Agente> listaAgentes = listaTotalAgentes.FindAll(jf => jf.IdNivel == 3 && jf.IdPadre != null).Select(jf => new Agente { Id = jf.Id, Nombre = jf.Nombre, IdPadre = jf.IdPadre, IdNivel = jf.IdNivel, Usuario = jf.Usuario }).ToList();

                            listaJefes = listaJefes.GroupBy(jf => new { jf.Id, jf.Nombre, jf.IdPadre, jf.IdNivel, jf.Usuario }).Select(jf => new Agente { Id = jf.Key.Id, Nombre = jf.Key.Nombre, IdPadre = jf.Key.IdPadre, IdNivel = jf.Key.IdNivel, Usuario = jf.Key.Usuario }).ToList();
                            listaSupervisores = listaSupervisores.GroupBy(jf => new { jf.Id, jf.Nombre, jf.IdPadre, jf.IdNivel, jf.Usuario }).Select(jf => new Agente { Id = jf.Key.Id, Nombre = jf.Key.Nombre, IdPadre = jf.Key.IdPadre, IdNivel = jf.Key.IdNivel, Usuario = jf.Key.Usuario }).ToList();
                            listaAgentes = listaAgentes.GroupBy(jf => new { jf.Id, jf.Nombre, jf.IdPadre, jf.IdNivel, jf.Usuario }).Select(jf => new Agente { Id = jf.Key.Id, Nombre = jf.Key.Nombre, IdPadre = jf.Key.IdPadre, IdNivel = jf.Key.IdNivel, Usuario = jf.Key.Usuario }).ToList();

                            List<Agente> unicaLista;

                            foreach (var itemJefe in listaJefes)
                            {
                                unicaLista = new List<Agente>();

                                unicaLista.Add(itemJefe);

                                List<Agente> listaitemJefesSuperv = listaSupervisores.FindAll(it => it.IdPadre == itemJefe.Id).ToList();

                                unicaLista.AddRange(listaitemJefesSuperv);

                                foreach (var item in listaitemJefesSuperv)
                                {
                                    List<Agente> listaitem = listaAgentes.FindAll(it => it.IdPadre == item.Id).ToList();

                                    unicaLista.AddRange(listaitem);
                                }

                                lista.Add(unicaLista);
                            }

                            Session["ListaArbol"] = lista;

                        }
                    }
                    else
                    {
                        log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.DashboardCntoFD.StringValue()));
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

        [WebMethod]
        public static List<List<ConsentimientoAsesoriaApi>> CargarTablaConsentimiento(string tokenUsuario, string rangoFecha)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DashboardCntoFD))
                        {

                            log.Info("Accediendo a las Key necesarias");
                            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                            string urlConsentimientoPorFecha = ConfigurationManager.AppSettings["url_consentimiento_por_fecha"].ToString();
                            string usuario = HttpContext.Current.Session["Usuario"].ToString();
                            List<ConsentimientoAsesoriaApi> lista_consentimientoRV = new List<ConsentimientoAsesoriaApi>();
                            List<ConsentimientoAsesoriaApi> lista_consentimientoRP = new List<ConsentimientoAsesoriaApi>();
                            List<List<ConsentimientoAsesoriaApi>> lista_consentimiento = new List<List<ConsentimientoAsesoriaApi>>();
                            JArray array_consentimiento = new JArray();
                            string fechaInicio = string.Empty;
                            string fechaFin = string.Empty;

                            int contadoRVS = 0;
                            int contadoRVN = 0;
                            int contadoRPS = 0;
                            int contadoRPN = 0;

                            log.Info("Verificando arbol");
                            List<List<Agente>> listaArbol = (List<List<Agente>>)HttpContext.Current.Session["ListaArbol"];

                            log.Info("Configurando fechas");
                            if (rangoFecha.Count() == 0)
                            {
                                DateTime fechaActual = DateTime.Now;
                                DateTime fechaInicial = fechaActual.AddMonths(-3);

                                fechaInicio = fechaInicial.Year + "-" + fechaInicial.ToString("MM") + "-" + fechaInicial.ToString("dd");
                                fechaFin = fechaActual.Year + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("dd");
                            }
                            else
                            {
                                var fechas = rangoFecha.Split('-');
                                fechaInicio = fechas[0].Substring(6, 4) + "-" + fechas[0].Substring(3, 2) + "-" + fechas[0].Substring(0, 2);
                                fechaFin = fechas[1].Substring(7, 4) + "-" + fechas[1].Substring(4, 2) + "-" + fechas[1].Substring(1, 2);
                            }

                            urlConsentimientoPorFecha = string.Format(urlConsentimientoPorFecha, fechaInicio, fechaFin, usuario);

                            log.Info("Consumiendo servicio consulta consentimiento por fecha: " + urlConsentimientoPorFecha);

                            array_consentimiento = ObtenerListaServicio(urlToken, urlConsentimientoPorFecha, usuario, array_consentimiento);

                            if (array_consentimiento.Count > 0)
                            {

                                log.Info("For each");
                                foreach (var consentimientoAPI in array_consentimiento)
                                {
                                    ConsentimientoAsesoriaApi consentimiento = new ConsentimientoAsesoriaApi();

                                    if (consentimientoAPI["gls_num_identificacion"] != null)
                                    {
                                        log.Info(consentimientoAPI["gls_num_identificacion"].ToString());
                                        consentimiento.gls_num_identificacion = consentimientoAPI["gls_num_identificacion"].ToString();
                                    }

                                    if (consentimientoAPI["gls_nombres"] != null && consentimientoAPI["gls_apellido_paterno"] != null && consentimientoAPI["gls_apellido_materno"] != null)
                                    {
                                        consentimiento.gls_nombres = consentimientoAPI["gls_nombres"].ToString().ToUpper() + " " + consentimientoAPI["gls_apellido_paterno"].ToString().ToUpper() + " " + consentimientoAPI["gls_apellido_materno"].ToString().ToUpper();
                                    }

                                    if (consentimientoAPI["gls_nombres_agente"] != null)
                                    {
                                        consentimiento.gls_nombres_agente = consentimientoAPI["gls_nombres_agente"].ToString().ToUpper();
                                    }

                                    //if (consentimiento.gls_nombres_agente == "ISMAEL CESAR CABALLERO OTOYA")
                                    //{
                                    //    var s = 2;
                                    //}

                                    if (consentimientoAPI["num_cuspp"] != null)
                                    {
                                        consentimiento.gls_num_cuspp = consentimientoAPI["num_cuspp"].ToString();
                                    }

                                    if (consentimientoAPI["num_agente"] != null)
                                    {
                                        consentimiento.num_agente = Convert.ToInt32(consentimientoAPI["num_agente"]);
                                    }

                                    if (consentimientoAPI["ind_consentimiento"] != null)
                                    {
                                        if (consentimientoAPI["ind_consentimiento"].ToString() == "S")
                                        {
                                            consentimiento.ind_consentimiento = "<span class='chip lighten-5 green green-text'>Si</span>";

                                            if (consentimientoAPI["id_configuracion"].ToString() == "1")
                                            {
                                                contadoRVS += 1;
                                            }
                                            else if (consentimientoAPI["id_configuracion"].ToString() == "2")
                                            {
                                                contadoRPS += 1;
                                            }

                                        }
                                        else
                                        {
                                            consentimiento.ind_consentimiento = "<span class='chip lighten-5 red red-text'>No</span>";

                                            if (consentimientoAPI["id_configuracion"].ToString() == "1")
                                            {
                                                contadoRVN += 1;
                                            }
                                            else if (consentimientoAPI["id_configuracion"].ToString() == "2")
                                            {
                                                contadoRPN += 1;
                                            }

                                        }

                                    }

                                    if (consentimientoAPI["fec_primer_envio"] != null && consentimientoAPI["fec_primer_envio"].Count() > 0)
                                    {
                                        consentimiento.fec_primer_envio = Convert.ToDateTime(consentimientoAPI["fec_primer_envio"]);
                                    }

                                    if (consentimientoAPI["fec_ultimo_envio"] != null && consentimientoAPI["fec_ultimo_envio"].Count() > 0)
                                    {
                                        consentimiento.fec_ultimo_envio = Convert.ToDateTime(consentimientoAPI["fec_ultimo_envio"]);
                                    }

                                    if (consentimientoAPI["val_cant_envios_consentimiento"] != null)
                                    {
                                        consentimiento.val_cant_envios_consentimiento = Convert.ToInt32(consentimientoAPI["val_cant_envios_consentimiento"]);
                                    }

                                    //Agente
                                    foreach (var item in listaArbol)
                                    {
                                        log.Info("Agente " + consentimiento.num_agente.Value.ToString());

                                        var infoAgente = item.Find(ag => ag.Id == consentimiento.num_agente.Value.ToString());

                                        if (infoAgente != null)
                                        {
                                            var infoSupervisor = item.Find(su => su.Id == infoAgente.IdPadre);
                                            if (infoSupervisor != null)
                                            {
                                                consentimiento.gls_nombres_supervisor = infoSupervisor.Nombre.ToUpper();
                                                var infoJefe = item.Find(su => su.Id == infoSupervisor.IdPadre);
                                                if (infoJefe != null)
                                                {
                                                    consentimiento.gls_nombres_jefe = infoJefe.Nombre.ToUpper();
                                                }
                                            }
                                        }
                                    }
                                    
                                    if (consentimientoAPI["id_configuracion"].ToString() == "1")
                                    {
                                        log.Info("CUSPP " + consentimiento.gls_num_cuspp);
                                        if (!string.IsNullOrEmpty(consentimiento.gls_num_cuspp))
                                        {
                                            dynamic campos = consentimiento.gls_num_cuspp;
                                            var camposDinamicos = JsonConvert.DeserializeObject<dynamic>(campos.ToString());
                                            foreach (var item in camposDinamicos)
                                            {
                                                if (item.etiqueta == "CUSPP")
                                                {
                                                    consentimiento.gls_num_cuspp = item.valor;
                                                }

                                            }
                                        }

                                        lista_consentimientoRV.Add(consentimiento);
                                    }
                                    else if (consentimientoAPI["id_configuracion"].ToString() == "2")
                                    {
                                        log.Info("CUSPP " + consentimiento.gls_num_cuspp);
                                        lista_consentimientoRP.Add(consentimiento);
                                    }

                                }

                                log.Info("Info chart");
                                if (lista_consentimientoRV.Count > 0)
                                {
                                    lista_consentimientoRV[0].val_cant_RV_S = contadoRVS;
                                    lista_consentimientoRV[0].val_cant_RV_N = contadoRVN;
                                }

                                if (lista_consentimientoRP.Count > 0)
                                {
                                    lista_consentimientoRP[0].val_cant_RP_S = contadoRPS;
                                    lista_consentimientoRP[0].val_cant_RP_N = contadoRPN;
                                }

                            }

                            string codigo = string.Empty;
                            int contador = 0;
                            int posicion = 0;
                            int nivel = 0;
                            string codigoPadre = string.Empty;


                            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue())
                            {
                                log.Info("Agregando listas");
                                lista_consentimiento.Add(lista_consentimientoRV);
                                lista_consentimiento.Add(lista_consentimientoRP);
                            }
                            else
                            {
                                List<ConsentimientoAsesoriaApi> lista_consentimientoRV_Roles = new List<ConsentimientoAsesoriaApi>();
                                List<ConsentimientoAsesoriaApi> lista_consentimientoRP_Roles = new List<ConsentimientoAsesoriaApi>();

                                foreach (var item in listaArbol)
                                {
                                    List<Agente> listaItem = item.FindAll(l => l.Usuario == usuario).ToList();

                                    if (listaItem.Count > 0)
                                    {
                                        codigo = listaItem.First().Id;
                                        nivel = listaItem.FirstOrDefault().IdNivel;
                                        posicion = contador;
                                        codigoPadre = listaItem.First().IdPadre;
                                        break;
                                    }

                                    contador += 1;
                                }

                                if (nivel == 1)
                                {
                                    foreach (var item in listaArbol[posicion])
                                    {
                                        foreach (var itemcntoRV in lista_consentimientoRV)
                                        {
                                            if (item.Id == itemcntoRV.num_agente.Value.ToString())
                                            {
                                                lista_consentimientoRV_Roles.Add(itemcntoRV);
                                            }
                                        }

                                        foreach (var itemcntoRP in lista_consentimientoRP)
                                        {
                                            if (item.Id == itemcntoRP.num_agente.Value.ToString())
                                            {
                                                lista_consentimientoRP_Roles.Add(itemcntoRP);
                                            }
                                        }
                                    }
                                }
                                else if (nivel == 2)
                                {
                                    var listaFinal = listaArbol[posicion].FindAll(l => l.IdPadre == codigo).ToList();
                                    listaFinal.Add(listaArbol[posicion].Find(l => l.Id == codigo));

                                    foreach (var item in listaFinal)
                                    {
                                        foreach (var itemcntoRV in lista_consentimientoRV)
                                        {
                                            if (item.Id == itemcntoRV.num_agente.Value.ToString())
                                            {
                                                lista_consentimientoRV_Roles.Add(itemcntoRV);
                                            }
                                        }

                                        foreach (var itemcntoRP in lista_consentimientoRP)
                                        {
                                            if (item.Id == itemcntoRP.num_agente.Value.ToString())
                                            {
                                                lista_consentimientoRP_Roles.Add(itemcntoRP);
                                            }
                                        }
                                    }
                                }
                                else if (nivel == 3)
                                {
                                    var listaFinal = listaArbol[posicion].FindAll(l => l.IdPadre == codigoPadre).ToList();

                                    foreach (var item in listaFinal)
                                    {
                                        foreach (var itemcntoRV in lista_consentimientoRV)
                                        {
                                            if (item.Id == itemcntoRV.num_agente.Value.ToString())
                                            {
                                                lista_consentimientoRV_Roles.Add(itemcntoRV);
                                            }
                                        }

                                        foreach (var itemcntoRP in lista_consentimientoRP)
                                        {
                                            if (item.Id == itemcntoRP.num_agente.Value.ToString())
                                            {
                                                lista_consentimientoRP_Roles.Add(itemcntoRP);
                                            }
                                        }
                                    }
                                }

                                log.Info("Info chart");
                                if (lista_consentimientoRV_Roles.Count > 0)
                                {
                                    lista_consentimientoRV_Roles[0].val_cant_RV_S = lista_consentimientoRV_Roles.FindAll(s => s.ind_consentimiento == "<span class='chip lighten-5 green green-text'>Si</span>").Count();
                                    lista_consentimientoRV_Roles[0].val_cant_RV_N = lista_consentimientoRV_Roles.FindAll(s => s.ind_consentimiento == "<span class='chip lighten-5 red red-text'>No</span>").Count();
                                }

                                if (lista_consentimientoRP_Roles.Count > 0)
                                {
                                    lista_consentimientoRP_Roles[0].val_cant_RP_S = lista_consentimientoRP_Roles.FindAll(s => s.ind_consentimiento == "<span class='chip lighten-5 green green-text'>Si</span>").Count();
                                    lista_consentimientoRP_Roles[0].val_cant_RP_N = lista_consentimientoRP_Roles.FindAll(s => s.ind_consentimiento == "<span class='chip lighten-5 red red-text'>No</span>").Count();
                                }

                                log.Info("Agregando listas");
                                lista_consentimiento.Add(lista_consentimientoRV_Roles);
                                lista_consentimiento.Add(lista_consentimientoRP_Roles);

                            }

                            return lista_consentimiento;

                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DashboardCntoFD.StringValue()));
                            return null;
                        }

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return null;
                    }

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
        public static List<List<FirmaDigitalDashboard>> CargarTablaFirmaDigital(string tokenUsuario, string rangoFecha)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DashboardCntoFD))
                        {

                            string usuario = HttpContext.Current.Session["Usuario"].ToString();
                            List<FirmaDigitalDashboard> lista_firmaDigitalRV = new List<FirmaDigitalDashboard>();
                            List<FirmaDigitalDashboard> lista_firmaDigitalRP = new List<FirmaDigitalDashboard>();
                            List<List<FirmaDigitalDashboard>> lista_firmaDigital = new List<List<FirmaDigitalDashboard>>();
                            string fechaInicio = string.Empty;
                            string fechaFin = string.Empty;

                            int contadoRVS = 0;
                            int contadoRVN = 0;
                            int contadoRPS = 0;
                            int contadoRPN = 0;

                            log.Info("Verificando arbol");
                            List<List<Agente>> listaArbol = (List<List<Agente>>)HttpContext.Current.Session["ListaArbol"];

                            log.Info("Configurando fechas");
                            if (rangoFecha.Count() == 0)
                            {
                                DateTime fechaActual = DateTime.Now;
                                DateTime fechaInicial = fechaActual.AddMonths(-3);

                                fechaInicio = fechaInicial.Year + "-" + fechaInicial.ToString("MM") + "-" + fechaInicial.ToString("dd");
                                fechaFin = fechaActual.Year + "-" + fechaActual.Month + "-" + fechaActual.Day;
                            }
                            else
                            {
                                var fechas = rangoFecha.Split('-');
                                fechaInicio = fechas[0].Substring(6, 4) + "-" + fechas[0].Substring(3, 2) + "-" + fechas[0].Substring(0, 2);
                                fechaFin = fechas[1].Substring(7, 4) + "-" + fechas[1].Substring(4, 2) + "-" + fechas[1].Substring(1, 2);
                            }

                            log.Info("Consultar servicio listas RV - RP");
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<FirmaDigitalDashboard> listaFirmaDigitalDashboard = servicioCotizador.ObtenerFirmaDigitalesDashboard(fechaInicio, fechaFin, usuario);

                            log.Info("Separar listas RV - RP");
                            lista_firmaDigitalRV = listaFirmaDigitalDashboard.FindAll(fdRV => fdRV.id_proceso_envio == (int)Enums.ProcesoEnvio.FirmaDigitalRV).ToList();
                            lista_firmaDigitalRP = listaFirmaDigitalDashboard.FindAll(fdRP => fdRP.id_proceso_envio == (int)Enums.ProcesoEnvio.FirmaDigitalRP).ToList();

                            log.Info("Contador chart");
                            contadoRVS = lista_firmaDigitalRV.FindAll(fdRV => fdRV.ind_consentimiento == "S").ToList().Count();
                            contadoRVN = lista_firmaDigitalRV.FindAll(fdRV => fdRV.ind_consentimiento == "N").ToList().Count();
                            contadoRPS = lista_firmaDigitalRP.FindAll(fdRP => fdRP.ind_consentimiento == "S").ToList().Count();
                            contadoRPN = lista_firmaDigitalRP.FindAll(fdRP => fdRP.ind_consentimiento == "N").ToList().Count();

                            if (lista_firmaDigitalRV.Count > 0)
                            {
                                log.Info("Indicador RV");
                                lista_firmaDigitalRV.FindAll(fdRV => fdRV.ind_consentimiento == "S").ToList().ForEach(fd => fd.ind_consentimiento = "<span class='chip lighten-5 green green-text'>Si</span>");
                                lista_firmaDigitalRV.FindAll(fdRV => fdRV.ind_consentimiento == "N").ToList().ForEach(fd => fd.ind_consentimiento = "<span class='chip lighten-5 red red-text'>No</span>");

                                foreach (var itemFirmaDigitalRV in lista_firmaDigitalRV)
                                {
                                    //Agente
                                    foreach (var item in listaArbol)
                                    {
                                        var infoAgente = item.Find(ag => ag.Id == itemFirmaDigitalRV.cod_agente.ToString());

                                        if (infoAgente != null)
                                        {
                                            var infoSupervisor = item.Find(su => su.Id == infoAgente.IdPadre);
                                            if (infoSupervisor != null)
                                            {
                                                itemFirmaDigitalRV.gls_nombres_supervisor = infoSupervisor.Nombre.ToUpper();
                                                var infoJefe = item.Find(su => su.Id == infoSupervisor.IdPadre);
                                                if (infoJefe != null)
                                                {
                                                    itemFirmaDigitalRV.gls_nombres_jefe = infoJefe.Nombre.ToUpper();
                                                }
                                            }
                                        }
                                    }
                                }

                                log.Info("Info chart RV");
                                lista_firmaDigitalRV.FirstOrDefault().val_cant_RV_S = contadoRVS;
                                lista_firmaDigitalRV.FirstOrDefault().val_cant_RV_N = contadoRVN;
                            }

                            if (lista_firmaDigitalRP.Count > 0)
                            {
                                log.Info("Indicador RP");
                                lista_firmaDigitalRP.FindAll(fdRP => fdRP.ind_consentimiento == "S").ToList().ForEach(fd => fd.ind_consentimiento = "<span class='chip lighten-5 green green-text'>Si</span>");
                                lista_firmaDigitalRP.FindAll(fdRP => fdRP.ind_consentimiento == "N").ToList().ForEach(fd => fd.ind_consentimiento = "<span class='chip lighten-5 red red-text'>No</span>");

                                foreach (var itemFirmaDigitalRP in lista_firmaDigitalRP)
                                {
                                    //Agente
                                    foreach (var item in listaArbol)
                                    {
                                        var infoAgente = item.Find(ag => ag.Id == itemFirmaDigitalRP.cod_agente.ToString());

                                        if (infoAgente != null)
                                        {
                                            var infoSupervisor = item.Find(su => su.Id == infoAgente.IdPadre);
                                            if (infoSupervisor != null)
                                            {
                                                itemFirmaDigitalRP.gls_nombres_supervisor = infoSupervisor.Nombre.ToUpper();
                                                var infoJefe = item.Find(su => su.Id == infoSupervisor.IdPadre);
                                                if (infoJefe != null)
                                                {
                                                    itemFirmaDigitalRP.gls_nombres_jefe = infoJefe.Nombre.ToUpper();
                                                }
                                            }
                                        }
                                    }
                                }

                                log.Info("Info chart RP");
                                lista_firmaDigitalRP.FirstOrDefault().val_cant_RP_S = contadoRPS;
                                lista_firmaDigitalRP.FirstOrDefault().val_cant_RP_N = contadoRPN;
                            }

                            string codigo = string.Empty;
                            int contador = 0;
                            int posicion = 0;
                            int nivel = 0;
                            string codigoPadre = string.Empty;

                            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue())
                            {
                                log.Info("Agregando listas");
                                lista_firmaDigital.Add(lista_firmaDigitalRV);
                                lista_firmaDigital.Add(lista_firmaDigitalRP);
                            }
                            else
                            {
                                List<FirmaDigitalDashboard> lista_firmaDigitalRV_Roles = new List<FirmaDigitalDashboard>();
                                List<FirmaDigitalDashboard> lista_firmaDigitalRP_Roles = new List<FirmaDigitalDashboard>();

                                foreach (var item in listaArbol)
                                {
                                    List<Agente> listaItem = item.FindAll(l => l.Usuario == usuario).ToList();

                                    if (listaItem.Count > 0)
                                    {
                                        codigo = listaItem.First().Id;
                                        nivel = listaItem.FirstOrDefault().IdNivel;
                                        posicion = contador;
                                        codigoPadre = listaItem.First().IdPadre;
                                        break;
                                    }

                                    contador += 1;
                                }

                                if (nivel == 1)
                                {
                                    foreach (var item in listaArbol[posicion])
                                    {
                                        foreach (var itemfdRV in lista_firmaDigitalRV)
                                        {
                                            if (item.Id == itemfdRV.cod_agente.ToString())
                                            {
                                                lista_firmaDigitalRV_Roles.Add(itemfdRV);
                                            }
                                        }

                                        foreach (var itemfdRP in lista_firmaDigitalRP)
                                        {
                                            if (item.Id == itemfdRP.cod_agente.ToString())
                                            {
                                                lista_firmaDigitalRP_Roles.Add(itemfdRP);
                                            }
                                        }
                                    }
                                }
                                else if (nivel == 2)
                                {
                                    var listaFinal = listaArbol[posicion].FindAll(l => l.IdPadre == codigo).ToList();
                                    listaFinal.Add(listaArbol[posicion].Find(l => l.Id == codigo));

                                    foreach (var item in listaFinal)
                                    {
                                        foreach (var itemfdRV in lista_firmaDigitalRV)
                                        {
                                            if (item.Id == itemfdRV.cod_agente.ToString())
                                            {
                                                lista_firmaDigitalRV_Roles.Add(itemfdRV);
                                            }
                                        }

                                        foreach (var itemfdRP in lista_firmaDigitalRP)
                                        {
                                            if (item.Id == itemfdRP.cod_agente.ToString())
                                            {
                                                lista_firmaDigitalRP_Roles.Add(itemfdRP);
                                            }
                                        }
                                    }
                                }
                                else if (nivel == 3)
                                {
                                    var listaFinal = listaArbol[posicion].FindAll(l => l.IdPadre == codigoPadre).ToList();

                                    foreach (var item in listaFinal)
                                    {
                                        foreach (var itemfdRV in lista_firmaDigitalRV)
                                        {
                                            if (item.Id == itemfdRV.cod_agente.ToString())
                                            {
                                                lista_firmaDigitalRV_Roles.Add(itemfdRV);
                                            }
                                        }

                                        foreach (var itemfdRP in lista_firmaDigitalRP)
                                        {
                                            if (item.Id == itemfdRP.cod_agente.ToString())
                                            {
                                                lista_firmaDigitalRP_Roles.Add(itemfdRP);
                                            }
                                        }
                                    }
                                }

                                log.Info("Info chart");
                                if (lista_firmaDigitalRV_Roles.Count > 0)
                                {
                                    lista_firmaDigitalRV_Roles[0].val_cant_RV_S = lista_firmaDigitalRV_Roles.FindAll(s => s.ind_consentimiento == "<span class='chip lighten-5 green green-text'>Si</span>").Count();
                                    lista_firmaDigitalRV_Roles[0].val_cant_RV_N = lista_firmaDigitalRV_Roles.FindAll(s => s.ind_consentimiento == "<span class='chip lighten-5 red red-text'>No</span>").Count();
                                }

                                if (lista_firmaDigitalRP_Roles.Count > 0)
                                {
                                    lista_firmaDigitalRP_Roles[0].val_cant_RP_S = lista_firmaDigitalRP_Roles.FindAll(s => s.ind_consentimiento == "<span class='chip lighten-5 green green-text'>Si</span>").Count();
                                    lista_firmaDigitalRP_Roles[0].val_cant_RP_N = lista_firmaDigitalRP_Roles.FindAll(s => s.ind_consentimiento == "<span class='chip lighten-5 red red-text'>No</span>").Count();
                                }

                                log.Info("Agregando listas");
                                lista_firmaDigital.Add(lista_firmaDigitalRV_Roles);
                                lista_firmaDigital.Add(lista_firmaDigitalRP_Roles);

                            }


                            return lista_firmaDigital;

                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DashboardCntoFD.StringValue()));
                            return null;
                        }

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return null;
                    }

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
        public static List<List<PolizasDashboard>> CargarTablaPoliza(string tokenUsuario, string rangoFecha)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DashboardCntoFD))
                        {

                            string usuario = HttpContext.Current.Session["Usuario"].ToString();
                            List<PolizasDashboard> lista_polizaRV = new List<PolizasDashboard>();
                            List<PolizasDashboard> lista_polizaRP = new List<PolizasDashboard>();
                            List<List<PolizasDashboard>> lista_poliza = new List<List<PolizasDashboard>>();
                            string fechaInicio = string.Empty;
                            string fechaFin = string.Empty;

                            log.Info("Verificando arbol");
                            List<List<Agente>> listaArbol = (List<List<Agente>>)HttpContext.Current.Session["ListaArbol"];

                            log.Info("Configurando fechas");
                            if (rangoFecha.Count() == 0)
                            {
                                DateTime fechaActual = DateTime.Now;
                                DateTime fechaInicial = fechaActual.AddMonths(-3);

                                fechaInicio = fechaInicial.Year + "-" + fechaInicial.ToString("MM") + "-" + fechaInicial.ToString("dd");
                                fechaFin = fechaActual.Year + "-" + fechaActual.Month + "-" + fechaActual.Day;
                            }
                            else
                            {
                                var fechas = rangoFecha.Split('-');
                                fechaInicio = fechas[0].Substring(6, 4) + "-" + fechas[0].Substring(3, 2) + "-" + fechas[0].Substring(0, 2);
                                fechaFin = fechas[1].Substring(7, 4) + "-" + fechas[1].Substring(4, 2) + "-" + fechas[1].Substring(1, 2);
                            }

                            log.Info("Consultar servicio listas póliza");
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<PolizasDashboard> listaPolizaDashboard = servicioCotizador.ObtenerPolizasDashboard(fechaInicio, fechaFin, usuario);

                            listaPolizaDashboard.FindAll(pRV => pRV.id_proceso_envio == (int)Enums.ProcesoEnvio.PolizaElectronicaIFP).ToList().ForEach(fd => fd.id_proceso_envio = (int)Enums.ProcesoEnvio.PolizaElectronicaRPP);

                            log.Info("Separar listas póliza RV - RP");
                            lista_polizaRV = listaPolizaDashboard.FindAll(pRV => pRV.id_proceso_envio == (int)Enums.ProcesoEnvio.PolizaElectronicaRV).ToList();
                            lista_polizaRP = listaPolizaDashboard.FindAll(pRP => pRP.id_proceso_envio == (int)Enums.ProcesoEnvio.PolizaElectronicaRPP).ToList();

                            foreach (var itemPolizaRV in lista_polizaRV)
                            {
                                //Agente
                                foreach (var item in listaArbol)
                                {
                                    var infoAgente = item.Find(ag => ag.Id == itemPolizaRV.cod_agente.ToString());

                                    if (infoAgente != null)
                                    {
                                        var infoSupervisor = item.Find(su => su.Id == infoAgente.IdPadre);
                                        if (infoSupervisor != null)
                                        {
                                            itemPolizaRV.gls_nombres_supervisor = infoSupervisor.Nombre.ToUpper();
                                            var infoJefe = item.Find(su => su.Id == infoSupervisor.IdPadre);
                                            if (infoJefe != null)
                                            {
                                                itemPolizaRV.gls_nombres_jefe = infoJefe.Nombre.ToUpper();
                                            }
                                        }
                                    }
                                }
                            }

                            foreach (var itemPolizaRP in lista_polizaRP)
                            {
                                //Agente
                                foreach (var item in listaArbol)
                                {
                                    var infoAgente = item.Find(ag => ag.Id == itemPolizaRP.cod_agente.ToString());

                                    if (infoAgente != null)
                                    {
                                        var infoSupervisor = item.Find(su => su.Id == infoAgente.IdPadre);
                                        if (infoSupervisor != null)
                                        {
                                            itemPolizaRP.gls_nombres_supervisor = infoSupervisor.Nombre.ToUpper();
                                            var infoJefe = item.Find(su => su.Id == infoSupervisor.IdPadre);
                                            if (infoJefe != null)
                                            {
                                                itemPolizaRP.gls_nombres_jefe = infoJefe.Nombre.ToUpper();
                                            }
                                        }
                                    }
                                }
                            }

                            string codigo = string.Empty;
                            int contador = 0;
                            int posicion = 0;
                            int nivel = 0;
                            string codigoPadre = string.Empty;

                            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue())
                            {
                                log.Info("Agregando listas");
                                lista_poliza.Add(lista_polizaRV);
                                lista_poliza.Add(lista_polizaRP);
                            }
                            else
                            {
                                List<PolizasDashboard> lista_polizaRV_Roles = new List<PolizasDashboard>();
                                List<PolizasDashboard> lista_polizaRP_Roles = new List<PolizasDashboard>();

                                foreach (var item in listaArbol)
                                {
                                    List<Agente> listaItem = item.FindAll(l => l.Usuario == usuario).ToList();

                                    if (listaItem.Count > 0)
                                    {
                                        codigo = listaItem.First().Id;
                                        nivel = listaItem.FirstOrDefault().IdNivel;
                                        posicion = contador;
                                        codigoPadre = listaItem.First().IdPadre;
                                        break;
                                    }

                                    contador += 1;
                                }

                                if (nivel == 1)
                                {
                                    foreach (var item in listaArbol[posicion])
                                    {
                                        foreach (var itemplzRV in lista_polizaRV)
                                        {
                                            if (item.Id == itemplzRV.cod_agente.ToString())
                                            {
                                                lista_polizaRV_Roles.Add(itemplzRV);
                                            }
                                        }

                                        foreach (var itemplzRP in lista_polizaRP)
                                        {
                                            if (item.Id == itemplzRP.cod_agente.ToString())
                                            {
                                                lista_polizaRP_Roles.Add(itemplzRP);
                                            }
                                        }
                                    }
                                }
                                else if (nivel == 2)
                                {
                                    var listaFinal = listaArbol[posicion].FindAll(l => l.IdPadre == codigo).ToList();
                                    listaFinal.Add(listaArbol[posicion].Find(l => l.Id == codigo));

                                    foreach (var item in listaFinal)
                                    {
                                        foreach (var itemplzRV in lista_polizaRV)
                                        {
                                            if (item.Id == itemplzRV.cod_agente.ToString())
                                            {
                                                lista_polizaRV_Roles.Add(itemplzRV);
                                            }
                                        }

                                        foreach (var itemplzRP in lista_polizaRP)
                                        {
                                            if (item.Id == itemplzRP.cod_agente.ToString())
                                            {
                                                lista_polizaRP_Roles.Add(itemplzRP);
                                            }
                                        }
                                    }
                                }
                                else if (nivel == 3)
                                {
                                    var listaFinal = listaArbol[posicion].FindAll(l => l.IdPadre == codigoPadre).ToList();

                                    foreach (var item in listaFinal)
                                    {
                                        foreach (var itemplzRV in lista_polizaRV)
                                        {
                                            if (item.Id == itemplzRV.cod_agente.ToString())
                                            {
                                                lista_polizaRV_Roles.Add(itemplzRV);
                                            }
                                        }

                                        foreach (var itemplzRP in lista_polizaRP)
                                        {
                                            if (item.Id == itemplzRP.cod_agente.ToString())
                                            {
                                                lista_polizaRP_Roles.Add(itemplzRP);
                                            }
                                        }
                                    }
                                }

                                log.Info("Agregando listas");
                                lista_poliza.Add(lista_polizaRV_Roles);
                                lista_poliza.Add(lista_polizaRP_Roles);

                            }



                            return lista_poliza;

                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DashboardCntoFD.StringValue()));
                            return null;
                        }

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return null;
                    }

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
        public static string TrazabilidadEnvio(string tokenUsuario, string rangoFecha, string idProceso)
        {
            using (log4net.NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (String.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {

                        if (Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.DashboardCntoFD))
                        {

                            //Generar archivo Excel
                            string urlToken = ConfigurationManager.AppSettings["url_token_APIcwrv"].ToString();
                            string urlServicio = ConfigurationManager.AppSettings["url_reporte_generar_excel"].ToString();
                            string urlRutaReportePlantilla = ConfigurationManager.AppSettings["ruta_Reporte_Plantilla_Trazabilidad"].ToString();
                            string urlRutaReporteGenerado = ConfigurationManager.AppSettings["ruta_Reporte_Generado_Trazabilidad"].ToString();
                            string token_generado = string.Empty;
                            string usuario = HttpContext.Current.Session["Usuario"].ToString();
                            string fechaInicio = string.Empty;
                            string fechaFin = string.Empty;
                            string rutaLocal = string.Empty;
                            string nombreDinamico = string.Empty;

                            log.Info("Verificando arbol");
                            List<List<Agente>> listaArbol = (List<List<Agente>>)HttpContext.Current.Session["ListaArbol"];

                            log.Info("Configurando fechas");
                            if (rangoFecha.Count() == 0)
                            {
                                DateTime fechaActual = DateTime.Now;
                                DateTime fechaInicial = fechaActual.AddMonths(-3);

                                fechaInicio = fechaInicial.Year + "-" + fechaInicial.ToString("MM") + "-" + fechaInicial.ToString("dd");
                                fechaFin = fechaActual.Year + "-" + fechaActual.Month + "-" + fechaActual.Day;
                            }
                            else
                            {
                                var fechas = rangoFecha.Split('-');
                                fechaInicio = fechas[0].Substring(6, 4) + "-" + fechas[0].Substring(3, 2) + "-" + fechas[0].Substring(0, 2);
                                fechaFin = fechas[1].Substring(7, 4) + "-" + fechas[1].Substring(4, 2) + "-" + fechas[1].Substring(1, 2);
                            }

                            log.Info("Consultar servicio listar trazabilidad");
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            List<ReporteTrazabilidad> listaEnvioSeguimiento = new List<ReporteTrazabilidad>();
                            listaEnvioSeguimiento = servicioCotizador.ObtenerTrazabilidad(fechaInicio, fechaFin, idProceso, usuario);

                            foreach (var itemEnvioSeguimiento in listaEnvioSeguimiento)
                            {
                                //CUSPP CNTO RV
                                if (itemEnvioSeguimiento.id_proceso_envio == 1)
                                {
                                    if (!string.IsNullOrEmpty(itemEnvioSeguimiento.num_cuspp))
                                    {
                                        dynamic campos = itemEnvioSeguimiento.num_cuspp;
                                        var camposDinamicos = JsonConvert.DeserializeObject<dynamic>(campos.ToString());
                                        foreach (var item in camposDinamicos)
                                        {
                                            if (item.etiqueta == "CUSPP")
                                            {
                                                itemEnvioSeguimiento.num_cuspp = item.valor;
                                            }

                                        }
                                    }
                                }

                                //Agente
                                foreach (var item in listaArbol)
                                {
                                    var infoAgente = item.Find(ag => ag.Id == itemEnvioSeguimiento.cod_agente.ToString());

                                    if (infoAgente != null)
                                    {
                                        var infoSupervisor = item.Find(su => su.Id == infoAgente.IdPadre);
                                        if (infoSupervisor != null)
                                        {
                                            itemEnvioSeguimiento.gls_nombres_supervisor = infoSupervisor.Nombre.ToUpper();
                                            var infoJefe = item.Find(su => su.Id == infoSupervisor.IdPadre);
                                            if (infoJefe != null)
                                            {
                                                itemEnvioSeguimiento.gls_nombres_jefe = infoJefe.Nombre.ToUpper();
                                            }
                                        }
                                    }
                                }
                            }

                            List<object> lstReporteObject = new List<object>();

                            string codigo = string.Empty;
                            int contador = 0;
                            int posicion = 0;
                            int nivel = 0;
                            string codigoPadre = string.Empty;

                            if ((string)HttpContext.Current.Session["RolAzman"] == Enums.RolAzman.AsistenteComercial.StringValue())
                            {
                                var listaEnvioSeguimientoReporte = listaEnvioSeguimiento.Select(es => new { es.gls_identificador, es.id_sme, es.cod_estado_trazabilidad, es.gls_mail, es.fec_envio, es.fec_firma, es.gls_motivo_rebote, es.nom_agente, es.gls_nombres_supervisor, es.gls_nombres_jefe, es.num_cuspp, es.gls_persona }).ToList();

                                foreach (var item in listaEnvioSeguimientoReporte)
                                {
                                    lstReporteObject.Add(item);
                                }
                            }
                            else
                            {
                                List<ReporteTrazabilidad> lista_trazabilidad_Roles = new List<ReporteTrazabilidad>();

                                foreach (var item in listaArbol)
                                {
                                    List<Agente> listaItem = item.FindAll(l => l.Usuario == usuario).ToList();

                                    if (listaItem.Count > 0)
                                    {
                                        codigo = listaItem.First().Id;
                                        nivel = listaItem.FirstOrDefault().IdNivel;
                                        posicion = contador;
                                        codigoPadre = listaItem.First().IdPadre;
                                        break;
                                    }

                                    contador += 1;
                                }

                                if (nivel == 1)
                                {
                                    foreach (var item in listaArbol[posicion])
                                    {
                                        foreach (var itemtrz in listaEnvioSeguimiento)
                                        {
                                            if (item.Id == itemtrz.cod_agente.ToString())
                                            {
                                                lista_trazabilidad_Roles.Add(itemtrz);
                                            }
                                        }

                                    }
                                }
                                else if (nivel == 2)
                                {
                                    var listaFinal = listaArbol[posicion].FindAll(l => l.IdPadre == codigo).ToList();
                                    listaFinal.Add(listaArbol[posicion].Find(l => l.Id == codigo));

                                    foreach (var item in listaFinal)
                                    {
                                        foreach (var itemtrz in listaEnvioSeguimiento)
                                        {
                                            if (item.Id == itemtrz.cod_agente.ToString())
                                            {
                                                lista_trazabilidad_Roles.Add(itemtrz);
                                            }
                                        }

                                    }
                                }
                                else if (nivel == 3)
                                {
                                    var listaFinal = listaArbol[posicion].FindAll(l => l.IdPadre == codigoPadre).ToList();

                                    foreach (var item in listaFinal)
                                    {
                                        foreach (var itemtrz in listaEnvioSeguimiento)
                                        {
                                            if (item.Id == itemtrz.cod_agente.ToString())
                                            {
                                                lista_trazabilidad_Roles.Add(itemtrz);
                                            }
                                        }

                                    }
                                }


                                var listaEnvioSeguimientoReporte = lista_trazabilidad_Roles.Select(es => new { es.gls_identificador, es.id_sme, es.cod_estado_trazabilidad, es.gls_mail, es.fec_envio, es.fec_firma, es.gls_motivo_rebote, es.nom_agente, es.gls_nombres_supervisor, es.gls_nombres_jefe, es.num_cuspp, es.gls_persona }).ToList();

                                foreach (var item in listaEnvioSeguimientoReporte)
                                {
                                    lstReporteObject.Add(item);
                                }

                            }



                            if (lstReporteObject.Count > 0)
                            {

                                log.Info("Consumiendo servicio token: " + urlToken);
                                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                                httpWebRequest.ContentType = "application/json";
                                httpWebRequest.Method = "POST";

                                log.Info("Pasando el json al servicio");
                                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                {
                                    string json = "{\"usuario\": \"" + usuario + "\"}";
                                    streamWriter.Write(json);
                                    streamWriter.Flush();
                                    streamWriter.Close();
                                }

                                log.Info("Leyendo el servicio");
                                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                                {
                                    var jsonResult = streamReader.ReadToEnd();
                                    JObject jObject = JObject.Parse(jsonResult);
                                    token_generado = (string)jObject["accessToken"];
                                }

                                if (token_generado != "")
                                {

                                    log.Info("Consumiendo servicio: " + urlServicio);
                                    httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServicio);
                                    httpWebRequest.ContentType = "application/json";
                                    httpWebRequest.Method = "POST";
                                    httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                                    nombreDinamico = "Trazabilidad" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                                    urlRutaReporteGenerado += nombreDinamico;

                                    log.Info("Pasando el json al servicio");
                                    ReporteIndicadores reporteTrazabilidad = new ReporteIndicadores()
                                    {
                                        rutaPlantilla = urlRutaReportePlantilla,
                                        rutaGenerar = urlRutaReporteGenerado,
                                        celdaEscritura = "A2",
                                        listaDatos = lstReporteObject
                                    };

                                    string json = JsonConvert.SerializeObject(reporteTrazabilidad);

                                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                                    {
                                        streamWriter.Write(json);
                                        streamWriter.Flush();
                                        streamWriter.Close();
                                    }

                                    log.Info("Leyendo el servicio");
                                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                                    {
                                        var jsonResult = streamReader.ReadToEnd();

                                        log.Info(nombreDinamico);
                                        rutaLocal = HttpContext.Current.Server.MapPath("~" + "/Plantilla/" + nombreDinamico);
                                        log.Info("rutaLocal: " + rutaLocal);

                                        //byte[] array = File.ReadAllBytes(urlRutaReporteGenerado);

                                        //HttpContext.Current.Response.Clear();
                                        //HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                        //HttpContext.Current.Response.AddHeader("Content-disposition", "filename=Trazabilidad_Envíos.xlsx");
                                        //HttpContext.Current.Response.BinaryWrite(array);
                                        //HttpContext.Current.Response.Flush();
                                        ////HttpContext.Current.Response.End();
                                        //HttpContext.Current.ApplicationInstance.CompleteRequest();

                                        //HttpContext.Current.Response.AppendHeader("content-disposition", "attachment;filename=Trazabilidad.xls");
                                        //HttpContext.Current.Response.Charset = "";
                                        //HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";
                                        //HttpContext.Current.Response.TransmitFile(urlRutaReporteGenerado);
                                        //HttpContext.Current.Response.Flush();

                                        //urlRutaReporteGenerado = @"\\hotei-dev\Rentas\Renta_Vitalicia\Reportes\Documentos_Generados\" + nombreDinamico;

                                        try
                                        {
                                            log.Info("urlRutaReporteGenerado: " + urlRutaReporteGenerado);
                                            if (File.Exists(urlRutaReporteGenerado))
                                            {
                                                File.Move(urlRutaReporteGenerado, rutaLocal);
                                            }
                                            else
                                            {
                                                nombreDinamico = string.Empty;
                                            }

                                        }
                                        catch (Exception)
                                        {
                                            log.Debug("Archivo no movido");
                                            nombreDinamico = string.Empty;
                                        }

                                        try
                                        {
                                            //eliminar archivo
                                            log.Info("Eliminar: " + urlRutaReporteGenerado);
                                            File.Delete(urlRutaReporteGenerado);

                                            if (!File.Exists(rutaLocal))
                                            {
                                                nombreDinamico = string.Empty;
                                            }

                                        }
                                        catch (Exception)
                                        {
                                            log.Debug("Archivo no eliminado");
                                            nombreDinamico = string.Empty;
                                        }


                                    }

                                }

                            }

                            return nombreDinamico;
                        }
                        else
                        {
                            log.Warn(String.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                                Enums.OpcionesSistema.DashboardCntoFD.StringValue()));
                            return string.Empty;
                        }

                    }
                    else
                    {
                        log.Warn("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        return string.Empty;
                    }

                }
                catch (Exception ex)
                {
                    log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace));
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
        public static bool EliminarArchivoTrazabilidad(string nombreArchivo)
        {
            try
            {
                string rutaLocal = HttpContext.Current.Server.MapPath("~" + "/Plantilla/" + nombreArchivo);
                File.Delete(rutaLocal);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Se ha producido el siguiente error: [{0}: {1}]\r\nStack Trace:\r\n{2}", ex.Source, ex.Message, ex.StackTrace));
                if (ex.InnerException != null)
                {
                    log.Error(String.Format("Inner Exception: [{0}: {1}]\r\nStack Trace:\r\n{2}",
                        ex.InnerException.Source, ex.InnerException.Message, ex.InnerException.StackTrace));
                }
                throw (ex);
            }
        }

        private static T ObtenerListaServicio<T>(string urlToken, string urlServicio, string usuario, T request)
        {
            T respuesta = default(T);
            string token_generado = string.Empty;

            try
            {
                log.Info("Consumiendo servicio token: " + urlToken);
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(urlToken);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                log.Info("Pasando el json al servicio");
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = "{\"usuario\": \"" + usuario + "\"}";

                    streamWriter.Write(json);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                log.Info("Leyendo el servicio");
                var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    var jsonResult = streamReader.ReadToEnd();
                    JObject jObject = JObject.Parse(jsonResult);
                    token_generado = (string)jObject["accessToken"];
                }

                if (token_generado != "")
                {
                    log.Info("Consumiendo servicio" + urlServicio);
                    httpWebRequest = (HttpWebRequest)WebRequest.Create(urlServicio);
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(usuario + ":" + token_generado));

                    log.Info("Leyendo el servicio");

                    httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                    {
                        var jsonResult = streamReader.ReadToEnd();

                        if (jsonResult.Length > 0)
                        {
                            respuesta = (T)JsonConvert.DeserializeObject(jsonResult);
                        }
                    }
                }

            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError)
                {
                    throw new Exception("Error en el servicio");
                }
            }
            return respuesta;
        }


    }
}
