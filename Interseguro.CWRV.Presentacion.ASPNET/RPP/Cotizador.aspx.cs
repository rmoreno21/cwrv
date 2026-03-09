using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Infraestructura.General;
using Interseguro.CWRV.Presentacion.AgenteServicios;
using Interseguro.CWRV.Presentacion.AgenteServicios.Proxies.ModuloPrincipal;
using Interseguro.CWRV.Presentacion.ASPNET.Builder.Utilitarios;
using Interseguro.CWRV.Presentacion.ASPNET.Controles;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Interseguro.CWRV.Presentacion.ASPNET.RPP
{
    public partial class Cotizador : Page
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Cotizador));
        private static IServicioCWRV servicioCotizador;
        private static int diasVigencia;
        private static double tipoCambioDia;

        protected void Page_Load(object sender, EventArgs e)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    string modo = Request.QueryString["m"];
                    string cuspp = Request.QueryString["c"];
                    string solicitud = Request.QueryString["s"];

                    Modo.Value = modo;
                    CUSPP.Value = cuspp;
                    NumeroSolicitud.Value = solicitud;
                    if (modo == "N" || modo == "D")
                    {
                        Random r = new Random();
                        Semilla.Value = string.Format("{0}{1}", Session["Usuario"], r.Next());
                    }
                    else
                    {
                        Semilla.Value = solicitud;
                    }

                    // Validar permisos
                    if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusConsultar) ||
                        Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusInsertar) ||
                        Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusActualizar))
                    {
                        if (!IsPostBack)
                        {
                            log.Info(string.Format("Usuario accedió a la opción [{0}] Modo[{1}] CUSPP [{2}] Solicitud[{3}]", Request.Url.AbsolutePath, modo, cuspp, solicitud));
                            CargarInformacionInicialPantalla();

                            // Consultando Beneficiarios del Grupo Familiar desde la BD
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            Afiliado afiliado = servicioCotizador.ObtenerDatosAfiliado(string.Empty, cuspp, string.Empty, string.Empty, string.Empty);
                            Session["RPPAfi" + Semilla.Value] = afiliado;

                            // Validar la cartera del agente
                            if (Utilitarios.EsRolVerAgentesCesados((string)Session["RolAzman"]) || ((List<Agente>)Session["ListaAgentes"]).Any(ag => ag.Id == afiliado.Agente.Id))
                            {
                                tipoCambioDia = servicioCotizador.ObtenerTipoCambio("009", DateTime.Today, Session["Usuario"].ToString());

                                List<GrupoFamiliar> beneficiarios = servicioCotizador.ListarGrupoFamiliar(cuspp);
                                beneficiarios = beneficiarios.FindAll(b =>
                                    b.Parentesco.Id == Enums.Parentesco.Afiliado.StringValue() ||
                                    b.Parentesco.Id == Enums.Parentesco.Conyuge.StringValue() ||
                                    b.Parentesco.Id == Enums.Parentesco.Hijo.StringValue() ||
                                    b.Parentesco.Id == Enums.Parentesco.Padre.StringValue() ||
                                    b.Parentesco.Id == Enums.Parentesco.Nieto.StringValue()
                                );
                                beneficiarios.ForEach(b =>
                                {
                                    if (b.Parentesco.Id != Enums.Parentesco.Afiliado.StringValue())
                                        b.Seleccionado = false;
                                    else
                                        b.Seleccionado = true;
                                });
                                Session["RPPBen" + Semilla.Value] = beneficiarios;
                                TablaRPPGrupoFamiliar.DataSource = beneficiarios;
                                TablaRPPGrupoFamiliar.DataBind();

                                MontoCIC.Value = afiliado.SaldoCIC.ToString();
                                TipoCambio.Value = tipoCambioDia.ToString();

                                switch (modo)
                                {
                                    case "N":
                                    case "M":
                                    case "D":
                                        // Fecha de Cotización = Hoy
                                        RPPFechaCotizacion.ReadOnly = true;

                                        // Fecha de Devengue = 1er día de mes
                                        RPPFechaDevengue.ReadOnly = true;

                                        // Vigencia de la Solicitud
                                        RPPVigencia.ReadOnly = true;

                                        // Setear Vitalicia por defecto
                                        RPPTemporalidad.SelectedValue = "TVT";
                                        RPPTemporalidad.Enabled = false;
                                        break;
                                    case "C":
                                        RPPTipoPlan.Enabled = false;
                                        RPPFechaCotizacion.ReadOnly = true;
                                        RPPFechaDevengue.ReadOnly = true;
                                        RPPVigencia.ReadOnly = true;
                                        RPPMonedaPrimaUnica.Enabled = false;
                                        RPPPrimaUnica.ReadOnly = true;
                                        RPPDCOM.ReadOnly = true;

                                        // Setear Vitalicia por defecto
                                        RPPTemporalidad.SelectedValue = "TVT";
                                        RPPTemporalidad.Enabled = false;
                                        break;
                                }

                                // Activar botón de Descargar sólo si es que se tienen los permisos
                                if (Utilitarios.ValidarPermiso(Session["OpcionesSistema"], Enums.OpcionesSistema.SolicitudPlusExportarPDF))
                                {
                                    DecargarDetalleSolicitud.Visible = true;

                                    // Activar el link del botón de descarga si es que existe una solicitud
                                    if (!string.IsNullOrEmpty(solicitud) && modo != "D")
                                    {
                                        DecargarDetalleSolicitud.Attributes.Add("style", "display:inline-block");
                                        DecargarDetalleSolicitud.NavigateUrl = string.Format("~/Reportes/ReportesRentaParticular.aspx?formato=6&solicitud={0}", solicitud);
                                    }
                                }
                                else
                                {
                                    DecargarDetalleSolicitud.Visible = false;
                                }
                            }
                            else
                            {
                                // Cliente no pertenece a cartera
                                Response.Redirect("~/Error/403.aspx");
                            }
                        }
                    }
                    else
                    {
                        log.Warn(string.Format("Usuario intentó acceder a una opción a la que no tiene privilegios [{0}].",
                            Enums.OpcionesSistema.MenuCotizador.StringValue()));
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

        [WebMethod]
        public static SolicitudRPPlus CotizarRPP(string tokenUsuario, SolicitudRPPlus solicitud, int[] arrBeneficiarios, string semilla)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        // Validación de Rol DCOM
                        bool dcom = false;
                        string dcomRangos = string.Empty;
                        foreach (RolDcom rolDcom in (List<RolDcom>)HttpContext.Current.Session["RPPRolDCOM"])
                        {
                            dcomRangos += "[" + rolDcom.NumRangoIni + "-" + rolDcom.NumRangoFin + "] ";
                            if (!(solicitud.PorcentajeDescuentoComision >= rolDcom.NumRangoIni && solicitud.PorcentajeDescuentoComision <= rolDcom.NumRangoFin))
                            {
                                dcom = false;
                            }
                            else
                            {
                                dcom = true;
                                break;
                            }
                        }
                        if (dcom)
                        {
                            // Obteniendo parámetros desde la memoria
                            var juegosParametros = (List<Dominio.Entidades.MotorCalculo.JuegoParametros>)HttpContext.Current.Session["ParametrosRPP" + ((DateTime)solicitud.FechaCotizacion).ToString("yyyyMMdd")];

                            // Evaluar tipo de cambio según moneda
                            if (solicitud.MonedaPrimaUnica.Id == Enums.Moneda.Soles.StringValue())
                            {
                                solicitud.TipoCambio = 1;
                            }
                            else
                            {
                                solicitud.TipoCambio = tipoCambioDia;
                            }

                            // Evaluar plan según número de beneficiarios
                            if (arrBeneficiarios.Length > 1)
                            {
                                solicitud.TipoPlan.Id = "01";
                            }
                            else
                            {
                                solicitud.TipoPlan.Id = "02";
                            }

                            List<Dominio.Entidades.MotorCalculo.Parametros> parametros = new List<Dominio.Entidades.MotorCalculo.Parametros>();
                            solicitud.Cotizaciones.ForEach(cotizacion =>
                            {
                                var parametro = juegosParametros.Find(p => p.CodigoMoneda == cotizacion.Moneda.Id).Parametros.Clonar<Dominio.Entidades.MotorCalculo.Parametros>();

                                parametro.cotizacion.fec_cotizacion = (DateTime)solicitud.FechaSolicitud;
                                parametro.cotizacion.fec_documento = (DateTime)solicitud.FechaSolicitud;
                                parametro.cotizacion.fec_devengue = (DateTime)solicitud.FechaDevengue;
                                parametro.cotizacion.fec_inicio_vigencia = (DateTime)solicitud.FechaDevengue;
                                parametro.cotizacion.fec_fin_vigencia = new DateTime(9999, 12, 31);
                                parametro.cotizacion.val_prima_unica = solicitud.PrimaUnica;
                                parametro.cotizacion.val_total_cic = solicitud.MontoCIC;
                                parametro.cotizacion.cod_moneda = cotizacion.Moneda.Id;
                                parametro.cotizacion.val_pje_moneda = cotizacion.ValMonAju;
                                parametro.cotizacion.val_dcom = (double)solicitud.PorcentajeDescuentoComision;
                                parametro.cotizacion.val_tipo_cambio = solicitud.TipoCambio;
                                parametro.cotizacion.num_meses_temporalidad = 1320;
                                parametro.cotizacion.num_meses_diferidos = cotizacion.PeriodoDiferido;
                                parametro.cotizacion.num_meses_garantizados = cotizacion.PeriodoGarantizado;
                                parametro.cotizacion.num_meses_pagos_doble = Convert.ToInt32(cotizacion.PagoEscalonada);
                                parametro.cotizacion.val_pje_segundo_periodo = cotizacion.PjePE;
                                parametro.cotizacion.ind_gratificacion = cotizacion.Gratificacion;
                                parametro.cotizacion.val_monto_sepelio = parametro.cotizacion.parametro_ash.val_cmor / solicitud.TipoCambio;
                                parametro.cotizacion.val_tasa_ret_accion = parametro.cotizacion.parametro_ash.val_dtra + (double)cotizacion.AjusteTRA;
                                parametro.cotizacion.ind_sepelio = cotizacion.IndGastoSepelio == "S";
                                parametro.isLogCotizacion = false; // parámetro nuevo
                                parametro.cotizacion.plan = new Dominio.Entidades.MotorCalculo.Plan
                                {
                                    Id = Enums.Planes.RPP.StringValue()
                                };
                                parametro.tipo_producto = Enums.TipoProducto.RPP.StringValue();
                                parametro.tipo_calculo = Enums.TipoCalculo.Cotizacion.StringValue();

                                // Capturar Beneficiarios desde el objeto Solicitud.Beneficiarios
                                parametro.cotizacion.beneficiarios = new List<Dominio.Entidades.MotorCalculo.Beneficiario>();

                                // Beneficiarios finales
                                List<GrupoFamiliar> beneficiariosCotizacion = new List<GrupoFamiliar>();
                                List<GrupoFamiliar> beneficiarios = (List<GrupoFamiliar>)HttpContext.Current.Session["RPPBen" + semilla];

                                for (int i = 0; i < arrBeneficiarios.Length; i++)
                                {
                                    var b = beneficiarios[arrBeneficiarios[i]];
                                    beneficiariosCotizacion.Add(b);
                                    var beneficiario = new Dominio.Entidades.MotorCalculo.Beneficiario()
                                    {
                                        item = i + 1,
                                        cod_parentesco = b.Parentesco.Id,
                                        fec_nacimiento = (DateTime)b.FechaNacimiento,
                                        ind_invalido = false,
                                        cod_sexo = b.Sexo.ToString(),
                                        val_pje_renta = 0,
                                        val_pje_adicional = 0
                                    };
                                    parametro.cotizacion.beneficiarios.Add(beneficiario);
                                }
                                solicitud.Beneficiarios = beneficiariosCotizacion;

                                var conyuge = beneficiariosCotizacion.FindAll(b => b.Parentesco.Id == Enums.Parentesco.Conyuge.StringValue());
                                var hijos = beneficiariosCotizacion.FindAll(b => b.Parentesco.Id == Enums.Parentesco.Hijo.StringValue() || b.Parentesco.Id == Enums.Parentesco.Nieto.StringValue());
                                var padres = beneficiariosCotizacion.FindAll(b => b.Parentesco.Id == Enums.Parentesco.Padre.StringValue());

                                if (conyuge.Count > 0)
                                {
                                    if (hijos.Count == 0 && padres.Count == 0)
                                    {
                                        // Sólo cónyuge
                                        if (cotizacion.ValPjeConyuge == 0) cotizacion.ValPjeConyuge = 42;
                                    }
                                    else
                                    {
                                        // Cónyuge + Hijo(s) y/o Padre(s)
                                        if (cotizacion.ValPjeConyuge == 0) cotizacion.ValPjeConyuge = 35;
                                    }
                                }

                                CalcularPorcentajesBeneficiarios(parametro.cotizacion.beneficiarios, (int)cotizacion.ValPjeConyuge);
                                cotizacion.PorcentajeBeneficiarios = CrearPorcentajesBeneficiario(parametro.cotizacion.beneficiarios);

                                parametros.Add(parametro);
                            });

                            // Cotizar
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            solicitud = servicioCotizador.CotizarRPP(solicitud, parametros);

                            // Se coloca el usuario en una variable local para que lo pueda usar el hilo
                            string usuario = (string)HttpContext.Current.Session["Usuario"];

                            // Registro de formato de Estudio de Necesidades
                            Task hilo = new Task(() =>
                            {
                                try
                                {
                                    EstudioNecesidadAPI estudioNecesidadAPI = new EstudioNecesidadAPI();
                                    SolicitudEdNAPI solicitudEDN = new SolicitudEdNAPI
                                    {
                                        num_solicitud = solicitud.Id,
                                        fec_solicitud = solicitud.FechaSolicitud.Value,
                                        val_mto_cta_individual = solicitud.PrimaUnica,
                                        cod_moneda_cta_indiv = solicitud.MonedaPrimaUnica.Id
                                    };

                                    estudioNecesidadAPI.solicitud = solicitudEDN;
                                    estudioNecesidadAPI.cotizaciones = new List<CotizacionEdNAPI>();
                                    foreach (CotizacionRPPlus c in solicitud.Cotizaciones)
                                    {
                                        CotizacionEdNAPI cotizacionEDN = new CotizacionEdNAPI
                                        {
                                            num_correlativo = c.Correlativo,
                                            cod_tipo_temporalidad = "TVT",
                                            val_per_diferido = 0
                                        };
                                        estudioNecesidadAPI.cotizaciones.Add(cotizacionEDN);
                                    }

                                    estudioNecesidadAPI.beneficiarios = new List<BeneficiarioEdNAPI>();
                                    foreach (GrupoFamiliar b in solicitud.Beneficiarios)
                                    {
                                        BeneficiarioEdNAPI beneficiarioEDN = new BeneficiarioEdNAPI
                                        {
                                            ape_paterno = b.ApellidoPaterno,
                                            ape_materno = b.ApellidoMaterno,
                                            nom_persona = b.Nombre,
                                            num_identificacion = b.Identificacion.Numero,
                                            cod_parentezco = b.Parentesco.Id
                                        };

                                        switch (b.Identificacion.IdTipo)
                                        {
                                            case "D":
                                                beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                                break;
                                            case "E":
                                                beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.CE.StringValue();
                                                break;
                                            case "P":
                                                beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.PAS.StringValue();
                                                break;
                                            default:
                                                beneficiarioEDN.cod_tipo_identificacion = Enums.TipoDocumentoCloudStorage.DNI.StringValue();
                                                break;
                                        }
                                        estudioNecesidadAPI.beneficiarios.Add(beneficiarioEDN);
                                    }

                                    log.Debug(string.Format("Se va a registrar el formato EDN de la solicitud [{0}]", solicitud.Id));
                                    var respuestaEdN = Utilitario.GenerarEstudioNecesidades(estudioNecesidadAPI, usuario);
                                    log.Debug(string.Format("Respuesta EDN: Solicitud[{0}] Estado[{1}] Mensaje[{2}]", solicitud.Id, respuestaEdN.Estado, respuestaEdN.Mensaje));
                                }
                                catch (Exception ex)
                                {
                                    log.Error(string.Format("Error en generación del formato de Estudio de Necesidades: Solicitud[{0}]", solicitud.Id), ex);
                                }
                            });
                            hilo.Start();

                            // Registrar en el log de eventos
                            servicioCotizador.RegistrarLog(new LogBD
                            {
                                IdAplicacion = Constante.APP_COTIZADOR_WEB_RENTAS_VITALICIAS,
                                NombreTerminal = Utilitarios.ObtenerNombreTerminal(HttpContext.Current.Request),
                                IP = HttpContext.Current.Request.ServerVariables["remote_addr"],
                                NombreUsuario = usuario,
                                IdTipoEvento = Enums.EventoLog.CotizarSolicitud.StringValue(),
                                Detalle = string.Format("Solicitud N° {0} cotizada", solicitud.Id)
                            });

                            return solicitud;
                        }
                        else
                        {
                            var error = string.Format("El campo <b>Porcentaje D</b> no se encuentra dentro de los rangos permitidos: ({0}).", dcomRangos.TrimEnd());
                            log.Error(error);
                            HttpContext.Current.Response.Status = "400 Bad Request";
                            HttpContext.Current.Response.StatusCode = 400;
                            HttpContext.Current.Response.StatusDescription = error;
                            HttpContext.Current.ApplicationInstance.CompleteRequest();
                            return null;
                        }
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
                    log.Error(string.Format("Se ha producido un error al cargar los datos de la cotización: {0}", ex.Message), ex);
                    throw ex;
                }
            }
        }

        private static void CalcularPorcentajesBeneficiarios(List<Dominio.Entidades.MotorCalculo.Beneficiario> beneficiarios, int conyuge = 42)
        {
            // Titular
            var beneficiario = beneficiarios.Find(b => b.cod_parentesco == Enums.Parentesco.Afiliado.StringValue());
            beneficiario.val_pje_renta = 100;

            // Cónyuge
            beneficiario = beneficiarios.Find(b => b.cod_parentesco == Enums.Parentesco.Conyuge.StringValue());
            if (beneficiario != null) 
            {
                if (beneficiarios.FindAll(b => (b.cod_parentesco == Enums.Parentesco.Hijo.StringValue() || b.cod_parentesco == Enums.Parentesco.Nieto.StringValue()) || b.cod_parentesco == Enums.Parentesco.Padre.StringValue()).Count > 0)
                {
                    beneficiario.val_pje_renta = 35;
                }
                else
                {
                    beneficiario.val_pje_renta = conyuge;
                }
            }

            // Hijos y Padres
            var hijosPadres = beneficiarios.FindAll(b => b.cod_parentesco == Enums.Parentesco.Hijo.StringValue() || b.cod_parentesco == Enums.Parentesco.Nieto.StringValue() || b.cod_parentesco == Enums.Parentesco.Padre.StringValue());
            if (hijosPadres.Count > 0)
            {
                hijosPadres.ForEach(b =>
                {
                    b.val_pje_renta = 14;
                });
            }

            // Porcentaje Adicional
            var conyugeHijosPadres = beneficiarios
                    .FindAll(b => b.cod_parentesco == Enums.Parentesco.Conyuge.StringValue() ||
                                  b.cod_parentesco == Enums.Parentesco.Hijo.StringValue() ||
                                  b.cod_parentesco == Enums.Parentesco.Padre.StringValue());
            if (conyugeHijosPadres.Count > 0)
            {
                if (conyugeHijosPadres.Sum(b => b.val_pje_renta) > 100)
                {
                    var conyugeHijos = beneficiarios
                            .FindAll(b => b.cod_parentesco == Enums.Parentesco.Conyuge.StringValue() ||
                                          b.cod_parentesco == Enums.Parentesco.Hijo.StringValue());
                    var padres = beneficiarios.FindAll(b => b.cod_parentesco == Enums.Parentesco.Padre.StringValue());
                    if (conyugeHijos.Sum(b => b.val_pje_renta) > 100)
                    {
                        // Padres a 0
                        if (padres.Count > 0)
                        {
                            padres.ForEach(b => b.val_pje_adicional = -14);
                        }

                        // Prorratear beneficiarios a 100
                        var suma = conyugeHijos.Sum(b => b.val_pje_renta);
                        conyugeHijos.ForEach(b =>
                        {
                            b.val_pje_adicional = 100.0 * b.val_pje_renta / suma - b.val_pje_renta;
                        });
                    }
                    else
                    {
                        // Hallar cuánto reducir a los padres
                        var resto = 100.0 - conyugeHijos.Sum(b => b.val_pje_renta);
                        padres.ForEach(b =>
                        {
                            b.val_pje_adicional = -resto / padres.Count;
                        });
                    }
                }
            }
        }

        private static List<PjeBen> CrearPorcentajesBeneficiario(List<Dominio.Entidades.MotorCalculo.Beneficiario> beneficiarios)
        {
            List<PjeBen> porcentajes = new List<PjeBen>();

            int i = 0;
            beneficiarios.ForEach(beneficiario =>
            {
                PjeBen porcentaje = new PjeBen
                {
                    Correlativo = i + 1,
                    TipoProducto = "01",
                    PorcentajeBase = beneficiario.val_pje_renta,
                    PorcentajeModificado = beneficiario.val_pje_renta + beneficiario.val_pje_adicional,
                    PorcentajeRetPeriodoDiferido = beneficiario.val_pje_renta + beneficiario.val_pje_adicional
                };
                porcentajes.Add(porcentaje);
                i++;
            });
            return porcentajes;
        }

        [WebMethod]
        public static void CargarParametrosRPP(string tokenUsuario, string temporalidad, string fechaCotizacion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    DateTime dFechaCotizacion = Convert.ToDateTime(fechaCotizacion, new CultureInfo("es-PE"));
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        if (HttpContext.Current.Session["ParametrosRPP" + dFechaCotizacion.ToString("yyyyMMdd")] == null)
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            HttpContext.Current.Session["ParametrosRPP" + dFechaCotizacion.ToString("yyyyMMdd")] = servicioCotizador.ObtenerParametrosRPP(temporalidad, dFechaCotizacion, Enums.OrigenCotizacion.Interseguro.StringValue(), HttpContext.Current.Session["Usuario"].ToString());
                        }

                        if (HttpContext.Current.Session["RPPRolDCOM"] == null)
                        {
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            HttpContext.Current.Session["RPPRolDCOM"] = servicioCotizador.ListarRolDcomRPP(new RolDcom
                            {
                                CodRol = (string)HttpContext.Current.Session["RolAzman"],
                                FechaCotizacion = dFechaCotizacion
                            });
                        }
                    }
                    else
                    {
                        log.Error("Token de usuario no coincide con el de la sesión original, se va a cerrar la sesión.");
                        HttpContext.Current.Response.Status = "401 Unauthorized";
                        HttpContext.Current.Response.StatusCode = 401;
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                    }
                }
                catch (Exception ex)
                {
                    log.Error(string.Format("Se ha producido un error al cargar los datos de la cotización: {0}", ex.Message), ex);
                    throw ex;
                }
            }
        }

        [WebMethod]
        public static SolicitudRPPlus ObtenerSolicitud(string tokenUsuario, string cuspp, string numeroSolicitud, string temporalidad, string modo, string semilla)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        SolicitudRPPlus solicitud = null;
                        Afiliado afiliado = (Afiliado)HttpContext.Current.Session["RPPAfi" + semilla];
                        if (numeroSolicitud == string.Empty)
                        {
                            // Nueva Solicitud, se obtienen las cotizaciones por default del web.config
                            solicitud = new SolicitudRPPlus
                            {
                                FechaSolicitud = DateTime.Today,
                                FechaCotizacion = DateTime.Today,
                                FechaDevengue = DateTime.Today.AddDays(-(DateTime.Today.Day - 1)),
                                FechaVigencia = DateTime.Today.AddDays(diasVigencia),
                                Cotizaciones = new List<CotizacionRPPlus>(),
                                Afiliado = new Afiliado
                                {
                                    CUSPP = cuspp,
                                    AFP = new AFP
                                    {
                                        Id = afiliado.AFP.Id
                                    }
                                },
                                AFP = new AFP
                                {
                                    Id = afiliado.AFP.Id
                                },
                                TipoCotizacion = new TipoCotizacion
                                {
                                    Id = Enums.TipoCotizacion.RentaPrivadaPlus.StringValue()
                                },
                                TipoPension = new TipoPension
                                {
                                    Id = Enums.TipoPension.Jubilacion.StringValue()
                                },
                                Categoria = new Categoria
                                {
                                    Id = "A"
                                },
                                Agente = new Agente
                                {
                                    Id = afiliado.Agente.Id,
                                    IdCartera = afiliado.Agente.IdCartera
                                },
                                FactorTasa = "O",
                                Temporalidad = new Temporalidad
                                {
                                    Id = temporalidad
                                },
                                TipoPlan = new TipoPlan
                                {
                                    Id = Enums.Planes.RPP.StringValue()
                                },
                                Usuario = new Usuario
                                {
                                    NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                    Rol = (string)HttpContext.Current.Session["RolAzman"]
                                }
                            };

                            SeccionCotizacionesRPPlus configuraciones = (SeccionCotizacionesRPPlus)ConfigurationManager.GetSection("cotizacionesRPPlus");
                            solicitud.MonedaPrimaUnica = new Moneda
                            {
                                Id = Enums.Moneda.Soles.StringValue()
                            };

                            CotizacionRPPlus cotizacion;
                            foreach (ElementoCotizacionRPPlus configuracion in configuraciones.CotizacionesRPPlus)
                            {
                                cotizacion = new CotizacionRPPlus
                                {
                                    Moneda = new Moneda { Id = configuracion.Moneda },
                                    //Producto = new Producto { Id = cotizacion.Producto },
                                    PeriodoGarantizado = Convert.ToInt32(configuracion.PeriodoGarantizado),
                                    AjusteTRA = 0,
                                    PagoEscalonada = 0,
                                    PjePE = 0,
                                    IndGastoSepelio = "S",
                                    ValPjeDev = 0,
                                    ValMonAju = (configuracion.Moneda == "001" || configuracion.Moneda == "002") ? -1 : 2,
                                    ValPjeConyuge = 0
                                };

                                solicitud.Cotizaciones.Add(cotizacion);
                            }

                            // Obtener beneficiarios desde cwrv_grupo_familiar
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            solicitud.Beneficiarios = servicioCotizador.ListarGrupoFamiliar(cuspp);
                        }
                        else
                        {
                            // Solicitud existente, se obtiene la lista de cortizaciones desde BD
                            servicioCotizador = LocalizadorProxy.ObtenerServicio();
                            solicitud = servicioCotizador.ObtenerDatosSolicitudRPPlus(numeroSolicitud);
                            solicitud.Cotizaciones.ForEach(c =>
                            {
                                c.PeriodoGarantizado *= 12;
                                c.PagoEscalonada *= 12;
                            });
                            solicitud.Usuario = new Usuario
                            {
                                NombreUsuario = (string)HttpContext.Current.Session["Usuario"],
                                Rol = (string)HttpContext.Current.Session["RolAzman"]
                            };

                            if (modo == "D")
                            {
                                solicitud.Id = string.Empty;
                                solicitud.FechaCotizacion = DateTime.Today;
                                solicitud.FechaSolicitud = DateTime.Today;
                                solicitud.FechaDevengue = DateTime.Today.AddDays(-(DateTime.Today.Day - 1));
                                solicitud.FechaVigencia = DateTime.Today.AddDays(diasVigencia);
                                solicitud.Cotizaciones.ForEach(c =>
                                {
                                    c.PensionCia = 0;
                                    c.PensionCiaMO = 0;
                                    c.Pension2doTramo = 0;
                                    c.Pension2doTramoSinAjuste = 0;
                                    c.TasaRetornoAccionista = 0;
                                    c.IndCotiza = string.Empty;
                                    c.TasaVenta = 0;
                                    c.TasaVentaSbs = 0;
                                    c.ValTasaCostoEquiv = 0;
                                });
                            }
                        }
                        return solicitud;
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
        public static string CargarTablaCotizaciones(string tokenUsuario, List<CotizacionRPPlus> cotizaciones, string temporalidad, string moneda, bool conyuge, char modo, string fechaCotizacion)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        //log.Debug(ConfigurationManager.AppSettings["fecha_validacion_moneda_IPC"]);

                        var pagina = new Page();
                        var control = (TablaCotizacionesRPP)pagina.LoadControl("~/Controles/TablaCotizacionesRPP.ascx");
                                                
                        int iTemporalidad = 110;
                        if (temporalidad != "TVT")
                        {
                            iTemporalidad = Convert.ToInt32(temporalidad.Substring(1));
                        }

                        foreach (CotizacionRPPlus cotizacion in cotizaciones)
                        {
                            if (cotizacion.PeriodoGarantizado > iTemporalidad * 12)
                            {
                                cotizacion.PeriodoGarantizado = iTemporalidad * 12;
                            }

                            if (!conyuge)
                            {
                                cotizacion.ValPjeConyuge = 0;
                            }

                            // El modo clonar pone todos los resultados en 0
                            if (modo == 'D')
                            {
                                cotizacion.Correlativo = 0;
                                cotizacion.Pension2doTramo = 0;
                                cotizacion.Pension2doTramoSinAjuste = 0;
                                cotizacion.PensionCia = 0;
                                cotizacion.PensionCiaMO = 0;
                                cotizacion.TasaVenta = 0;
                                cotizacion.TasaVentaSbs = 0;
                                cotizacion.TasaRetornoAccionista = 0;
                                cotizacion.IndCotiza = string.Empty;
                            }
                        }

                        control.Cotizaciones = cotizaciones;
                        control.Moneda = moneda;
                        control.Temporalidad = iTemporalidad;
                        control.Conyuge = conyuge;

                        // Validar si el acceso es desde dentro de la red de Interseguro o desde Internet
                        if (Utilitarios.ValidarRedLocal(HttpContext.Current.Request.UserHostAddress))
                        {
                            control.PermisoTRA = Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoTRAPlus) ? true : false;
                            control.PermisoEspeciales = Utilitarios.ValidarPermiso(HttpContext.Current.Session["OpcionesSistema"], Enums.OpcionesSistema.PermisoEspeciales) ? true : false;
                        }
                        else
                        {
                            control.PermisoTRA = false;
                            control.PermisoEspeciales = false;
                        }

                        control.PermisoAgregar = true;
                        control.PermisoModificar = true;
                        control.PermisoEliminar = true;

                        // Modo consulta no debe permitir actualizar las cotizaciones
                        if (modo == 'C')
                        {
                            control.PermisoAgregar = false;
                            control.PermisoModificar = false;
                            control.PermisoEliminar = false;
                        }

                        control.Modo = modo;

                        //log.Debug(fechaCotizacion);
                        var fecha_cotizacion = fechaCotizacion.Split('/');
                        //log.Debug(fecha_cotizacion);
                        Int32 cantidad_fecha_cotizacion = Convert.ToInt32(fecha_cotizacion[0]) + (Convert.ToInt32(fecha_cotizacion[1]) * 30) + ((Convert.ToInt32(fecha_cotizacion[2]) * 12) * 30);
                        //log.Debug(cantidad_fecha_cotizacion);

                        string fecha_moneda_IPC = ConfigurationManager.AppSettings["fecha_validacion_moneda_IPC"];
                        //log.Debug(fecha_moneda_IPC);
                        var fecha_IPC = fecha_moneda_IPC.Split('/');
                        //log.Debug(fecha_IPC);
                        Int32 cantidad_fecha_moneda_IPC = Convert.ToInt32(fecha_IPC[0]) + (Convert.ToInt32(fecha_IPC[1]) * 30) + ((Convert.ToInt32(fecha_IPC[2]) * 12) * 30); //DateTime.ParseExact(fecha_moneda_IPC, "dd/MM/yyyy", new CultureInfo("es-PE"));  //new DateTime(Convert.ToInt32(fecha_IPC[2]), Convert.ToInt32(fecha_IPC[1]), Convert.ToInt32(fecha_IPC[0]));
                        //log.Debug(cantidad_fecha_moneda_IPC);

                        control.cantidadFechaCotizacion = cantidad_fecha_cotizacion;
                        control.cantidadFechaValidacionMonedaIPC = cantidad_fecha_moneda_IPC;

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
        public static string CargarTablaBeneficiarios(string tokenUsuario, List<GrupoFamiliar> beneficiarios)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        var pagina = new Page();
                        var control = (TablaBeneficiariosRPP)pagina.LoadControl("~/Controles/TablaBeneficiariosRPP.ascx");

                        control.Beneficiarios = beneficiarios;
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
        public static List<CotizacionRPPlus> AgregarCotizacionASolicitud(string tokenUsuario, List<CotizacionRPPlus> cotizaciones)
        {
            using (NDC.Push(MethodBase.GetCurrentMethod().Name))
            {
                try
                {
                    if (string.Equals(tokenUsuario, (string)HttpContext.Current.Session["TokenUsuario"]))
                    {
                        CotizacionRPPlus cotizacion = new CotizacionRPPlus
                        {
                            Moneda = new Moneda { Id = "0" },
                            PeriodoGarantizado = 0,
                            AjusteTRA = 0,
                            IndGastoSepelio = "S",
                            PagoEscalonada = 0,
                            PjePE = 0,
                            ValPjeDev = 0
                        };
                        cotizaciones.Add(cotizacion);

                        return cotizaciones;
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
                    log.Error(string.Format("Se ha producido un error al cargar los datos de la cotización: {0}", ex.Message), ex);
                    throw ex;
                }
            }
        }

        private void CargarInformacionInicialPantalla()
        {
            // Cargar los parámetros de los Combobox únicamente si no han sido cargados antes
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

            CargarCombobox(RPPTemporalidad, listaCombobox[(int)Enums.CategoriaCombobox.Temporalidad]);
            CargarCombobox(RPPMonedaPrimaUnica, listaCombobox[(int)Enums.CategoriaCombobox.MonedaRentaPrivada]);
            CargarCombobox(RPPTipoPlan, listaCombobox[(int)Enums.CategoriaCombobox.TipoPlan]);


            List<Parametro> comboMoneda = new List<Parametro>
            {
                new Parametro { Id = "001:-1", Glosa = "Indexados" },
                new Parametro { Id = "013:0", Glosa = "Nominales" },
                new Parametro { Id = "013:2", Glosa = "Ajustados" },
                new Parametro { Id = "002:0", Glosa = "Nominales" },
                new Parametro { Id = "014:2", Glosa = "Ajustados" }
            };
            Session["RPPComboMoneda"] = comboMoneda;

            List<Parametro> lstParametro = new List<Parametro>();
            lstParametro = listaCombobox[(int)Enums.CategoriaCombobox.PagoEscalonado];
            LlenarPagoDoble(lstParametro);


            Session["RPPComboPorcentajeEscalonada"] = listaCombobox[(int)Enums.CategoriaCombobox.NuevoPorcentajeEscalonado];

            List<Parametro> lstPjeDevolucion = new List<Parametro>();
            lstPjeDevolucion = listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeDevolucion];
            var PjeDevolucion = lstPjeDevolucion.OrderBy(a => int.Parse(a.Id));

            Session["RPPComboPorcentajeDevolucion"] = PjeDevolucion.ToList();

            List<Parametro> comboPeriodoGarantizado = new List<Parametro>
            {
                new Parametro { Id = "0", Glosa = "0" },
                new Parametro { Id = "60", Glosa = "5" },
                new Parametro { Id = "84", Glosa = "7" },
                new Parametro { Id = "120", Glosa = "10" },
                new Parametro { Id = "180", Glosa = "15" },
                new Parametro { Id = "240", Glosa = "20" },
                new Parametro { Id = "300", Glosa = "25" }
            };
            Session["RPPComboPeriodoGarantizado"] = comboPeriodoGarantizado;

            Session["RPPComboPorcentajeConyuge"] = listaCombobox[(int)Enums.CategoriaCombobox.PorcentajeConyuge];

            // Validando si el acceso es desde dentro dela red de Interseguro o desde Internet
            //LabModSolLineaACOMDCOM_RP.Visible = Utilitarios.ValidarRedLocal(Request.UserHostAddress);

            //if (!LabModSolLineaACOMDCOM_RP.Visible)
            //{
            //    LabModSolFechaVigencia_RP.CssClass = "formLabel formLabel2Izq";
            //}


            /* Implementacion ACOM, solamente cuando al configuracion sea S */
            string KeyAcom = ConfigurationManager.AppSettings["keyAcom"];
            //hdKeyAcom.Value = KeyAcom;

            lstParametro = new List<Parametro>();
            Session["RPPComboMonedaAjustePlus"] = listaCombobox[(int)Enums.CategoriaCombobox.MonedaAjustePlus];

            // Días de vigencia de la solicitud
            if (Session["DiasVigenciaPlus"] == null)
            {
                diasVigencia = Convert.ToInt32(servicioCotizador.ObtenerParametrosPorTabla("PLUS")[0].Valor_1);
                Session["DiasVigenciaPlus"] = diasVigencia;
            }
            else
            {
                diasVigencia = (int)Session["DiasVigenciaPlus"];
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

        public void LlenarPagoDoble(List<Parametro> lstParametro)
        {
            List<Parametro> lstParametro2 = new List<Parametro>();

            if (lstParametro.Count > 0)
            {
                lstParametro
                    .FindAll(p => p.Id == "I-RPP")
                    .ForEach(p =>
                    {
                        for (var i = Convert.ToInt32(p.Valor_1); i <= Convert.ToInt32(p.Valor_2); i++)
                        {
                            lstParametro2.Add(new Parametro { Id = (i * 12).ToString(), Glosa = i.ToString() });
                        }
                    });
            }
            HttpContext.Current.Session["RPPComboPagoDoble"] = lstParametro2;
        }

        // Evento grillas
        protected void TablaRPPGrupoFamiliar_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                GrupoFamiliar beneficiario = (GrupoFamiliar)e.Row.DataItem;

                e.Row.Cells[0].Text = "<label><input type=\"checkbox\" name=\"Correlativo\" data-parentesco=\"" + beneficiario.Parentesco.Id + "\""
                   + "value=\"" + beneficiario.Id + "\""
                   + ((beneficiario.Parentesco.Id != "80") ? string.Empty : " disabled=\"disabled\"")
                   + ((beneficiario.Seleccionado) ? " checked=\"checked\"" : string.Empty)
                   + " /><span></span></label>";
            }
        }
    }
}