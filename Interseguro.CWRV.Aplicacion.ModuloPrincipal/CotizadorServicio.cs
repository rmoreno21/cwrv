using HtmlAgilityPack;
using Interseguro.CWRV.Dominio.Entidades;
using Interseguro.CWRV.Dominio.Repositorios;
using Interseguro.CWRV.Infraestructura.General;
using iTextSharp.text;
using iTextSharp.text.pdf;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Transactions;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace Interseguro.CWRV.Aplicacion.ModuloPrincipal
{
    public class CotizadorServicio : ICotizadorServicio
    {
        private readonly IRepositorioAfiliado repositorioAfiliado;
        private readonly IRepositorioGrupoFamiliar repositorioGrupoFamiliar;
        private readonly IRepositorioSolicitud repositorioSolicitud;
        private readonly IRepositorioAporteAdicional repositorioAporteAdicional;
        private readonly IRepositorioSolicitudRP repositorioSolicitudRP;
        private readonly IRepositorioLote repositorioLote;
        private readonly IRepositorioSolicitudEscenario repositorioSolicitudEscenario;
        private readonly IRepositorioCita repositorioCita;
        private readonly IRepositorioTipoMovimiento repositorioTipoMovimiento;
        private readonly IRepositorioAnticipo repositorioAnticipo;
        private readonly IRepositorioActividad repositorioActividad;
        private readonly IRepositorioSeguimiento repositorioSeguimiento;
        private readonly IRepositorioSupervision repositorioSupervision;
        private readonly IRepositorioRolAcom repositorioRolAcom;
        private readonly IRepositorioRolDcom repositorioRolDcom;
        private readonly IRepositorioRolDtra repositorioRolDtra;
        private readonly IRepositorioParametroGeneral repositorioParametroGeneral;
        private readonly IRepositorioEscenario repositorioEscenario;
        private readonly IRepositorioAdelantoComision repositorioAdelantoComision;
        private readonly IRepositorioDescuentoComision repositorioDescuentoComision;
        private readonly IRepositorioSolicitudRPPlus repositorioSolicitudRPPlus;
        private readonly IRepositorioFlujoMovimiento repositorioFlujoMovimiento;
        private readonly IRepositorioCuotasTra repositorioRolCuotasTra;
        private readonly IRepositorioGestionVentas repositorioGestionVentas;
        private readonly IRepositorioAgente repositorioAgente;
        private readonly IRepositorioCausalPoliza repositorioCausalPoliza;
        private readonly IRepositorioEmisionPoliza repositorioEmisionPoliza;
        private readonly IRepositorioDireccion repositorioDireccion;
        private readonly IRepositorioSolicitudIFP repositorioSolicitudIFP;
        private readonly IRepositorioReporteIndicadores repositorioReporteIndicadores;
        private readonly IRepositorioReportes repositorioReportes;
        private readonly IRepositorioEstadoCivil repositorioEstadoCivil;
        private readonly IRepositorioNacionalidad repositorioNacionalidad;
        private readonly IRepositorioProfesion repositorioProfesion;
        private readonly IRepositorioMotorCalculo repositorioMotorCalculo;
        private static readonly ILog log = LogManager.GetLogger(typeof(CotizadorServicio));

        public CotizadorServicio(
            IRepositorioAfiliado repositorioAfiliado,
            IRepositorioGrupoFamiliar repositorioGrupoFamiliar,
            IRepositorioSolicitud repositorioSolicitud,
            IRepositorioAporteAdicional repositorioAporteAdicional,
            IRepositorioSolicitudRP repositorioSolicitudRP,
            IRepositorioLote repositorioLote,
            IRepositorioSolicitudEscenario repositorioSolicitudEscenario,
            IRepositorioCita repositorioCita,
            IRepositorioTipoMovimiento repositorioTipoMovimiento,
            IRepositorioAnticipo repositorioAnticipo,
            IRepositorioActividad repositorioActividad,
            IRepositorioSeguimiento repositorioSeguimiento,
            IRepositorioSupervision repositorioSupervision,
            IRepositorioParametroGeneral repositorioParametroGeneral,
            IRepositorioEscenario repositorioEscenario,
            IRepositorioAdelantoComision repositorioAdelantoComision,
            IRepositorioDescuentoComision repositorioDescuentoComision,
            IRepositorioRolAcom repositorioRolAcom,
            IRepositorioRolDcom repositorioRolDcom,
            IRepositorioRolDtra repositorioRolDtra,
            IRepositorioSolicitudRPPlus repositorioSolicitudRPPlus,
            IRepositorioFlujoMovimiento repositorioFlujoMovimiento,
            IRepositorioCuotasTra repositorioRolCuotasTra,
            IRepositorioGestionVentas repositorioGestionVentas,
            IRepositorioAgente repositorioAgente,
            IRepositorioCausalPoliza repositorioCausalPoliza,
            IRepositorioEmisionPoliza repositorioEmisionPoliza,
            IRepositorioDireccion repositorioDireccion,
            IRepositorioSolicitudIFP repositorioSolicitudIFP,
            IRepositorioReporteIndicadores repositorioReporteIndicadores,
            IRepositorioReportes repositorioReportes,
            IRepositorioEstadoCivil repositorioEstadoCivil,
            IRepositorioNacionalidad repositorioNacionalidad,
            IRepositorioProfesion repositorioProfesion,
            IRepositorioMotorCalculo repositorioMotorCalculo
          )
        {
            this.repositorioAfiliado = repositorioAfiliado;
            this.repositorioGrupoFamiliar = repositorioGrupoFamiliar;
            this.repositorioSolicitud = repositorioSolicitud;
            this.repositorioAporteAdicional = repositorioAporteAdicional;
            this.repositorioSolicitudRP = repositorioSolicitudRP;
            this.repositorioSolicitud = repositorioSolicitud;
            this.repositorioLote = repositorioLote;
            this.repositorioSolicitudEscenario = repositorioSolicitudEscenario;
            this.repositorioCita = repositorioCita;
            this.repositorioTipoMovimiento = repositorioTipoMovimiento;
            this.repositorioAnticipo = repositorioAnticipo;
            this.repositorioActividad = repositorioActividad;
            this.repositorioSeguimiento = repositorioSeguimiento;
            this.repositorioSupervision = repositorioSupervision;
            this.repositorioParametroGeneral = repositorioParametroGeneral;
            this.repositorioEscenario = repositorioEscenario;
            this.repositorioAdelantoComision = repositorioAdelantoComision;
            this.repositorioDescuentoComision = repositorioDescuentoComision;
            this.repositorioRolAcom = repositorioRolAcom;
            this.repositorioRolDcom = repositorioRolDcom;
            this.repositorioRolDtra = repositorioRolDtra;
            this.repositorioSolicitudRPPlus = repositorioSolicitudRPPlus;
            this.repositorioFlujoMovimiento = repositorioFlujoMovimiento;
            this.repositorioRolCuotasTra = repositorioRolCuotasTra;
            this.repositorioGestionVentas = repositorioGestionVentas;
            this.repositorioAgente = repositorioAgente;
            this.repositorioCausalPoliza = repositorioCausalPoliza;
            this.repositorioEmisionPoliza = repositorioEmisionPoliza;
            this.repositorioDireccion = repositorioDireccion;
            this.repositorioSolicitudIFP = repositorioSolicitudIFP;
            this.repositorioReporteIndicadores = repositorioReporteIndicadores;
            this.repositorioReportes = repositorioReportes;
            this.repositorioEstadoCivil = repositorioEstadoCivil;
            this.repositorioNacionalidad = repositorioNacionalidad;
            this.repositorioProfesion = repositorioProfesion;
            this.repositorioMotorCalculo = repositorioMotorCalculo;
        }

        public SDAReporte obtenerPreCubo(int numeroAgente, string cuspp)
        {
            var reporte = repositorioParametroGeneral.obtenerPreCubo(numeroAgente, cuspp);
            return reporte;
        }

        public Agente ObtenerMSAgente(string idAgente)
        {
            return repositorioAgente.ObtenerMS(idAgente);
        }

        public List<Afiliado> ListarAfiliado(string apellidoPaterno, string apellidoMaterno, string nombres, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros)
        {
            var afiliados = repositorioAfiliado.Listar(apellidoPaterno, apellidoMaterno, nombres, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar, ref totalRegistros);
            return afiliados;
        }
        public Afiliado ObtenerDatosAfiliado(string solicitud, string cuspp, string tipoIdentificacion, string numIdentificacion, string producto)
        {
            var afiliado = repositorioAfiliado.ObtenerDatos(solicitud, cuspp, tipoIdentificacion, numIdentificacion, producto);
            return afiliado;
        }

        public void RegistrarAfiliado(Afiliado afiliado, string usuario, ref string numCUSPP)
        {
            repositorioAfiliado.Registrar(afiliado, usuario, ref numCUSPP);
        }

        public void ActualizarAfiliado(Afiliado afiliado)
        {
            repositorioAfiliado.Actualizar(afiliado);
        }

        public AporteAdicional ObtenerDatosAporteAdicional(string CUSPP)
        {
            var aporte = repositorioAporteAdicional.ObtenerDatos(CUSPP);
            return aporte;
        }

        public void ActualizarAporteAdicional(AporteAdicional aporte, string usuario)
        {
            repositorioAporteAdicional.Actualizar(aporte, usuario);
        }

        public void EliminarAporteAdicional(AporteAdicional aporte, string usuario)
        {
            repositorioAporteAdicional.Eliminar(aporte, usuario);
        }

        public List<GrupoFamiliar> ListarGrupoFamiliar(string cuspp)
        {
            var grupos = repositorioGrupoFamiliar.Listar(cuspp);
            return grupos;
        }
        public GrupoFamiliar ObtenerDatosGrupoFamiliar(int idGrupoFamiliar, string num_solicitud)//<INI.GTI_7012_V13>
        {
            var grupo = repositorioGrupoFamiliar.ObtenerDatos(idGrupoFamiliar, num_solicitud);
            return grupo;
        }
        public void RegistrarGrupoFamiliar(GrupoFamiliar grupo)
        {
            repositorioGrupoFamiliar.Registrar(grupo);
        }
        public void ActualizarGrupoFamiliar(GrupoFamiliar grupo)
        {
            repositorioGrupoFamiliar.Actualizar(grupo);
        }
        public List<Solicitud> ListarSolicitud(string cuspp)
        {
            var solicitud = repositorioSolicitud.Listar(cuspp);
            return solicitud;
        }
        public List<Solicitud> ListarSolicitud(int lote)
        {
            var solicitud = repositorioSolicitud.Listar(lote);
            return solicitud;
        }
        public List<Solicitud> ListarSolicitudesPorFechaCierreAFP(DateTime fechaInicio, DateTime fechaFin, char enviada)
        {
            var solicitud = repositorioSolicitud.ListarSolicitudesPorFechaCierreAFP(fechaInicio, fechaFin, enviada);
            return solicitud;
        }
        public List<Solicitud> ListarConfirmaciones(DateTime fechaInicio, DateTime fechaFin, char enviada)
        {
            var solicitud = repositorioSolicitud.ListarConfirmaciones(fechaInicio, fechaFin, enviada);
            return solicitud;
        }
        public string ObtenerXMLConfirmacionesCargaMeler(string xml)
        {
            XDocument documento = new XDocument();
            documento.Declaration = new XDeclaration("1.0", "ISO-8859-1", null);
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            XNamespace noNamespaceSchemaLocation = XNamespace.Get(@"D:\Inetpub\wwwroot\meler\schemas\cargaConfirmaciones.xsd");
            XElement cargaConfirmaciones =
                new XElement("cargaConfirmaciones",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(xsi + "noNamespaceSchemaLocation", noNamespaceSchemaLocation)
                );
            //XNamespace ns3 = XNamespace.Get("http://www.example.com/b");
            //XNamespace ns2 = XNamespace.Get("http://www.example.com/c");
            //new XAttribute(XNamespace.Xmlns + "ns3", ns3),
            //new XAttribute(XNamespace.Xmlns + "ns2", ns2),
            documento.Add(cargaConfirmaciones);
            var solicitudes = repositorioSolicitud.ListarSolicitudesConfirmacionMeler(xml);
            foreach (Solicitud solicitud in solicitudes)
            {
                cargaConfirmaciones.Add(
                    new XElement("confirmacion",
                        new XElement("operacion", solicitud.Id),
                        new XElement("CUSPP", solicitud.Afiliado.CUSPP)
                    )
                );
                XElement pensionEESS =
                    new XElement("pensionEESS",
                        new XElement("numeroPoliza", solicitud.NumeroPoliza)
                    );
                if (solicitud.Cotizaciones[0].Modalidad.Id == "RV")
                {
                    pensionEESS.Add(new XElement("primeraPensionRV", String.Format("{0:0.00}", solicitud.Cotizaciones[0].PrimeraPensionRV)));
                }
                else if (solicitud.Cotizaciones[0].Modalidad.Id == "RTVD")
                {
                    pensionEESS.Add(new XElement("primeraPensionRT", String.Format("{0:0.00}", solicitud.Cotizaciones[0].PrimeraPensionRT)));
                    pensionEESS.Add(new XElement("primeraPensionRVD", String.Format("{0:0.00}", solicitud.Cotizaciones[0].PrimeraPensionRVD)));
                }
                else if (solicitud.Cotizaciones[0].Modalidad.Id == "RM")
                {
                    if (solicitud.Cotizaciones[0].Cotiza == "*")
                    {
                        pensionEESS.Add(new XElement("primeraPensionRT", "0.00"));
                        pensionEESS.Add(new XElement("primeraPensionRVD", "0.00"));
                    }
                    else
                    {
                        pensionEESS.Add(new XElement("primeraPensionRT", String.Format("{0:0.00}", solicitud.Cotizaciones[0].PrimeraPensionRT)));
                        pensionEESS.Add(new XElement("primeraPensionRVD", String.Format("{0:0.00}", solicitud.Cotizaciones[0].PrimeraPensionRVD)));
                    }
                }
                else if (solicitud.Cotizaciones[0].Modalidad.Id == "RC")
                {
                    if (solicitud.Cotizaciones[0].Cotiza == "*")
                    {
                        pensionEESS.Add(new XElement("primeraPensionRT", "0.00"));
                        pensionEESS.Add(new XElement("primeraPensionRVD", "0.00"));
                    }
                    else
                    {
                        pensionEESS.Add(new XElement("primeraPensionRT", String.Format("{0:0.00}", solicitud.Cotizaciones[0].PrimeraPensionRT)));
                        pensionEESS.Add(new XElement("primeraPensionRVD", String.Format("{0:0.00}", solicitud.Cotizaciones[0].PrimeraPensionRVD)));
                    }
                }
                else if (solicitud.Cotizaciones[0].Modalidad.Id == "RB")
                {
                    pensionEESS.Add(new XElement("primeraPensionRT", String.Format("{0:0.00}", solicitud.Cotizaciones[0].PrimeraPensionRT)));
                    pensionEESS.Add(new XElement("primeraPensionRVD", String.Format("{0:0.00}", solicitud.Cotizaciones[0].PrimeraPensionRVD)));
                }
                if (solicitud.Cotizaciones[0].Modalidad.Id != "RV")
                {
                    pensionEESS.Add(new XElement("primaUnicaAFPEESS", solicitud.Cotizaciones[0].PrimaUnicaAFPEESS));
                }
                pensionEESS.Add(new XElement("primaUnicaEESS", solicitud.Cotizaciones[0].PrimaUnicaEESS));
                pensionEESS.Add(new XElement("montoTransferido", solicitud.Cotizaciones[0].MontoCia));
                pensionEESS.Add(new XElement("inicioVigencia", Convert.ToDateTime(solicitud.FechaDevengue).ToString("yyyy-MM-dd")));
                cargaConfirmaciones.Add(pensionEESS);
            }
            documento.Add(cargaConfirmaciones);
            return documento.ToString();
        }
        public string ObtenerXMLSolicitudesCargaMeler(string xml)
        {
            XDocument documento = new XDocument();
            documento.Declaration = new XDeclaration("1.0", "ISO-8859-1", null);
            XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
            XNamespace noNamespaceSchemaLocation = XNamespace.Get(@"D:\Inetpub\wwwroot\meler\schemas\cargaCotizaciones.xsd");
            XElement cargaCotizaciones =
                new XElement("cargaCotizaciones",
                    new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                    new XAttribute(xsi + "noNamespaceSchemaLocation", noNamespaceSchemaLocation)
                );
            //XElement cargaCotizaciones = new XElement("cargaCotizaciones");
            var solicitudes = repositorioSolicitud.ListarSolicitudesCargaMeler(xml);

            foreach (Solicitud solicitud in solicitudes)
            {
                XElement cotizaciones =
                    new XElement("cotizaciones",
                        new XElement("nroOperacion", solicitud.Id),
                        new XElement("CUSPP", solicitud.Afiliado.CUSPP)
                    );
                foreach (Cotizacion cotizacion in solicitud.Cotizaciones)
                {
                    XElement productoCotizado =
                        new XElement("productoCotizado",
                            new XElement("modalidad", cotizacion.Modalidad.Id),
                            new XElement("moneda", cotizacion.Moneda.Simbolo)
                        );
                    if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Diferida.StringValue())
                    {
                        productoCotizado.Add(new XElement("anosRT", cotizacion.PeriodoDiferido));
                        productoCotizado.Add(new XElement("porcentajeRVD", cotizacion.PorcentajeEntreRentas));
                    }
                    //<SOLINIGTI_754>
                    if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Escalonada.StringValue())
                    {
                        productoCotizado.Add(new XElement("anosRT", cotizacion.PeriodoDiferido));
                        productoCotizado.Add(new XElement("porcentajeRVD", cotizacion.PorcentajeEntreRentas));
                    }
                    //<SOLFINGTI_754>
                    if (cotizacion.PeriodoGarantizado > 0)
                    {
                        productoCotizado.Add(new XElement("periodoGarantizado", cotizacion.PeriodoGarantizado));
                    }
                    if (cotizacion.PorcentajeConyuge > 0)
                    {
                        productoCotizado.Add(new XElement("coberturaConyuge", cotizacion.PorcentajeConyuge));
                    }
                    productoCotizado.Add(new XElement("derechoCrecer", (cotizacion.DerechoCrecer) ? "S" : "N"));
                    productoCotizado.Add(new XElement("gratificacion", (cotizacion.Gratificacion) ? "S" : "N"));
                    if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Mixta.StringValue() || cotizacion.Modalidad.Id == Enums.ModalidadMeler.Combinada.StringValue() || cotizacion.Modalidad.Id == Enums.ModalidadMeler.Bimoneda.StringValue())
                    {
                        productoCotizado.Add(new XElement("particionCapital", cotizacion.Capital.Id));
                    }
                    XElement cotizacionEESS = new XElement("cotizacionEESS");
                    //<GTI.INI-29372>
                    //cotizacionEESS.Add(new XElement("siCotizaNoCotiza", (cotizacion.IndCotizacion) ? "S" : "N"));
                    cotizacionEESS.Add(new XElement("siCotizaNoCotiza", (cotizacion.IndCotizacion && cotizacion.IndEnvioObligatorio) ? "S" : "N"));
                    if (cotizacion.IndCotizacion && cotizacion.IndEnvioObligatorio)
                    //if (cotizacion.IndCotizacion)
                    //<GTI.FIN-29372>
                    {
                        cotizacionEESS.Add(new XElement("nroCotizacion", cotizacion.Correlativo));
                        if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Mixta.StringValue() || cotizacion.Modalidad.Id == Enums.ModalidadMeler.Bimoneda.StringValue() || cotizacion.Modalidad.Id == Enums.ModalidadMeler.Combinada.StringValue() || cotizacion.Modalidad.Id == Enums.ModalidadMeler.Diferida.StringValue())
                        {
                            cotizacionEESS.Add(new XElement("primaUnicaAFPEESS", String.Format("{0:0.00}", cotizacion.PrimaUnicaAFPEESS)));
                        }
                        cotizacionEESS.Add(new XElement("primaUnicaEESS", String.Format("{0:0.00}", cotizacion.PrimaUnicaEESS)));
                        if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Inmediata.StringValue())
                        {
                            cotizacionEESS.Add(new XElement("primeraPensionRV", String.Format("{0:0.00}", cotizacion.PrimeraPensionRV)));
                            cotizacionEESS.Add(new XElement("tasaInteresRV", String.Format("{0:0.000}", cotizacion.TasaInteresRV)));
                        }
                        else if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Diferida.StringValue())
                        {
                            cotizacionEESS.Add(new XElement("primeraPensionRT", String.Format("{0:0.00}", cotizacion.PrimeraPensionRT)));
                            cotizacionEESS.Add(new XElement("tasaInteresRT", String.Format("{0:0.00}", cotizacion.TasaInteresRT)));
                            cotizacionEESS.Add(new XElement("primeraPensionRVD", String.Format("{0:0.00}", cotizacion.PrimeraPensionRVD)));
                            cotizacionEESS.Add(new XElement("tasaInteresRVD", String.Format("{0:0.000}", cotizacion.TasaInteresRVD)));
                        }
                        //<SOLINIGTI_754>
                        else if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Escalonada.StringValue())
                        {
                            cotizacionEESS.Add(new XElement("tasaInteresRV", String.Format("{0:0.000}", cotizacion.TasaInteresRVD)));
                            cotizacionEESS.Add(new XElement("primeraPensionRT", String.Format("{0:0.00}", cotizacion.PrimeraPensionRT)));
                            cotizacionEESS.Add(new XElement("primeraPensionRVD", String.Format("{0:0.00}", cotizacion.PrimeraPensionRVD)));
                        }
                        //<SOLFINGTI_754>
                        else if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Bimoneda.StringValue())
                        {
                            cotizacionEESS.Add(new XElement("primeraPensionRT", String.Format("{0:0.00}", cotizacion.PrimeraPensionRT)));
                            cotizacionEESS.Add(new XElement("tasaInteresRT", String.Format("{0:0.00}", cotizacion.TasaInteresRT)));
                            cotizacionEESS.Add(new XElement("primeraPensionRVD", String.Format("{0:0.00}", cotizacion.PrimeraPensionRVD)));
                            cotizacionEESS.Add(new XElement("tasaInteresRVD", String.Format("{0:0.000}", cotizacion.TasaInteresRVD)));
                        }
                        else if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Mixta.StringValue())
                        {
                            if (cotizacion.Cotiza == "*")
                            {
                                cotizacionEESS.Add(new XElement("primeraPensionRT", "0.00"));
                                cotizacionEESS.Add(new XElement("tasaInteresRT", String.Format("{0:0.00}", cotizacion.TasaInteresRT)));
                                cotizacionEESS.Add(new XElement("primeraPensionRVD", "0.00"));
                            }
                            else
                            {
                                cotizacionEESS.Add(new XElement("primeraPensionRT", String.Format("{0:0.00}", cotizacion.PrimeraPensionRT)));
                                cotizacionEESS.Add(new XElement("tasaInteresRT", String.Format("{0:0.00}", cotizacion.TasaInteresRT)));
                                cotizacionEESS.Add(new XElement("primeraPensionRVD", String.Format("{0:0.00}", cotizacion.PrimeraPensionRVD)));
                            }
                            cotizacionEESS.Add(new XElement("tasaInteresRVD", String.Format("{0:0.000}", cotizacion.TasaInteresRVD)));
                        }
                        else if (cotizacion.Modalidad.Id == Enums.ModalidadMeler.Combinada.StringValue())
                        {
                            if (cotizacion.Cotiza == "*")
                            {
                                cotizacionEESS.Add(new XElement("primeraPensionRT", "0.00"));
                                cotizacionEESS.Add(new XElement("tasaInteresRT", String.Format("{0:0.00}", cotizacion.TasaInteresRT)));
                                cotizacionEESS.Add(new XElement("primeraPensionRVD", "0.00"));
                            }
                            else
                            {
                                cotizacionEESS.Add(new XElement("primeraPensionRT", String.Format("{0:0.00}", cotizacion.PrimeraPensionRT)));
                                cotizacionEESS.Add(new XElement("tasaInteresRT", String.Format("{0:0.00}", cotizacion.TasaInteresRT)));
                                cotizacionEESS.Add(new XElement("primeraPensionRVD", String.Format("{0:0.00}", cotizacion.PrimeraPensionRVD)));
                            }
                            cotizacionEESS.Add(new XElement("tasaInteresRVD", String.Format("{0:0.000}", cotizacion.TasaInteresRVD)));
                        }
                    }
                    productoCotizado.Add(cotizacionEESS);
                    cotizaciones.Add(productoCotizado);
                }
                cargaCotizaciones.Add(cotizaciones);
            }
            documento.Add(cargaCotizaciones);
            documento.Save(@"C:\Temp\ejemplo.xml");
            StringBuilder builder = new StringBuilder();
            Encoding encoding = Encoding.UTF8; // <SOLINIGTI_754> Encoding.GetEncoding("ISO-8859-1"); //Encoding.UTF8 ;// <SOLFINGTI_754>
            using (TextWriter writer = new EncodingStringWriter(builder, encoding))
            {
                documento.Save(writer);
            }
            return builder.ToString();
        }
        //<SRI.INI_20322_E2>
        public List<SolicitudEscenario> ListarSolicitudEscenario(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol)
        {
            List<SolicitudEscenario> solicitudEscenario = repositorioSolicitudEscenario.Listar(numJefe, numSupervisor, numAgente, codUserName, codRol);
            return solicitudEscenario;
        }
        //<INIGTI_4081>
        public List<SolicitudEscenario> ListarSolicitudEscenarioCambios(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol)
        {
            List<SolicitudEscenario> solicitudEscenario = repositorioSolicitudEscenario.ListarCambios(numJefe, numSupervisor, numAgente, codUserName, codRol);
            return solicitudEscenario;
        }
        //<FINGTI_4081>
        public SolicitudEscenario ObtenerDatosSolicitudEscenario(string numSolicitud, string codUserName, string codRol)
        {
            SolicitudEscenario solicitud = repositorioSolicitudEscenario.ObtenerPorId(numSolicitud, codUserName, codRol);
            return solicitud;
        }
        public void RegistrarSolicitudEscenario(ref SolicitudEscenario solicitudEscenario)
        {
                repositorioSolicitudEscenario.Registrar(ref solicitudEscenario);
                Cotizar(solicitudEscenario.NumSolicitud, (DateTime)solicitudEscenario.FechaCotizacion, solicitudEscenario.Usuario.NombreUsuario);
        }
        public void ActualizarSolicitudEscenario(ref SolicitudEscenario solicitudEscenario, bool cotizar)
        {
                repositorioSolicitudEscenario.Actualizar(ref solicitudEscenario);
                if (cotizar)
                    Cotizar(solicitudEscenario.NumSolicitud, (DateTime)solicitudEscenario.FechaCotizacion, solicitudEscenario.Usuario.NombreUsuario);
        }
        public void RegistrarSolicitudEscenarioExtraoficial(ref SolicitudEscenario solicitudEscenario)
        {
                repositorioSolicitudEscenario.RegistrarExtraoficial(ref solicitudEscenario);
                Cotizar(solicitudEscenario.NumSolicitud, (DateTime)solicitudEscenario.FechaCotizacion, solicitudEscenario.Usuario.NombreUsuario);
                repositorioSolicitudEscenario.Actualizar(solicitudEscenario);
        }
        public List<Cita> ListarCita(Cita citaIn)
        {
            List<Cita> cita = repositorioCita.Listar(citaIn);
            return cita;
        }
        public List<RolAzmanTipoMovimiento> ObtenerTipoMovimientoPorRolAzman(string codRol)
        {
            List<RolAzmanTipoMovimiento> rolAzmanTipoMovimiento = repositorioTipoMovimiento.ObtenerTipoMovimientoPorRolAzman(codRol);
            return rolAzmanTipoMovimiento;
        }
        public List<CotizacionMovimiento> ObtenerCotizacionTipoMovimientoPorSolicitud(string numSolicitud)
        {
            List<CotizacionMovimiento> cotizacionMovimiento = repositorioTipoMovimiento.ObtenerCotizacionTipoMovimientoPorSolicitud(numSolicitud);
            return cotizacionMovimiento;
        }
        public void RegistrarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario)
        {
            repositorioSolicitud.RegistrarCotizacionMovimiento(XMLCotizacionMovimiento, usuario);
        }
        //<INIGTI_4081_2>
        public void ActualizarCotizacionMovimiento(string XMLCotizacionMovimiento, string usuario)
        {
            repositorioSolicitud.ActualizarCotizacionMovimiento(XMLCotizacionMovimiento, usuario);
        }
        //<FINGTI_4081_2>
        //<INIGTI_4081_3>
        public void RegistrarFlujoSolicitudCompleto(ref Respuesta rpta, ref Solicitud solicitud, string XML_CotizacionMovimiento, string codUserName, string codRol)
        {
                string accion = rpta.Contenido.Substring(1, rpta.Contenido.Length - 1);
                RegistrarCotizacionMovimiento(XML_CotizacionMovimiento, codUserName);
                if (accion == "APROBADO")
                {
                    if (solicitud != null)
                    {
                        ActualizarSolicitud(ref solicitud);
                    }
                    int tipoMovimientoDestino = Convert.ToInt32(rpta.Contenido.Substring(0, 1));
                    //tipoMovimientoDestino(6) Aprobado
                    if (tipoMovimientoDestino == 6)
                    {
                        SolicitudEscenario escenario = ObtenerDatosSolicitudEscenario(solicitud.Id, codUserName, codRol);
                        escenario.IndEstadoSeleccion = escenario.IndEstadoSeleccion;//estadoSeleccion;
                        escenario.ValMtoAgenteAcom = solicitud.MontoAumentoComision.Value;//Convert.ToDouble(montoAcom, new CultureInfo("es-PE"));
                        escenario.NumCotizacionElegida = Convert.ToInt64(escenario.NumCotizacionElegida);//elegida
                        escenario.Cotizaciones = solicitud.Cotizaciones;//listaCotizaciones;
                        escenario.Usuario = new Usuario { NombreUsuario = codUserName };
                        ActualizarSolicitudEscenario(ref escenario, false);
                    }
                }
        }
        //<FINGTI_4081_3>
        //<SRI.FIN_20322_E2>
        public Solicitud ObtenerDatosSolicitud(string idSolicitud, DateTime fecCotizacion)
        {
            var solicitud = repositorioSolicitud.ObtenerDatos(idSolicitud, fecCotizacion);
            return solicitud;
        }
        //<SOLINI26593>
        public SolicitudRP ObtenerDatosSolicitudRP(string idSolicitud, DateTime fecCotizacion)
        {
            var solicitudRP = repositorioSolicitudRP.ObtenerDatos(idSolicitud, fecCotizacion);
            return solicitudRP;
        }
        //<SOLFIN26593>
        public List<Actividad> ListarActividad(string cuspp)
        {
            var actividad = repositorioActividad.Listar(cuspp);
            return actividad;
        }
        public List<Seguimiento> ListarSeguimiento(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros)
        {
            var seguimientos = repositorioSeguimiento.Listar(idJefe, idSupervisor, idAgente, cuspp, fechaInicio, fechaTermino, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar, ref totalRegistros);
            return seguimientos;
        }
        public List<Seguimiento> ListarExcelSeguimiento(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar)
        {
            var seguimientos = repositorioSeguimiento.ListarExcel(idJefe, idSupervisor, idAgente, cuspp, fechaInicio, fechaTermino, columnaOrdenar, direccionOrdenar);
            return seguimientos;
        }
        public List<Supervision> ListarSupervision(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros)
        {
            var supervisiones = repositorioSupervision.Listar(idJefe, idSupervisor, idAgente, fechaInicio, fechaTermino, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar, ref totalRegistros);
            return supervisiones;
        }
        public List<Supervision> ListarExcelSupervision(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar)
        {
            var supervisiones = repositorioSupervision.ListarExcel(idJefe, idSupervisor, idAgente, fechaInicio, fechaTermino, columnaOrdenar, direccionOrdenar);
            return supervisiones;
        }
        public void RegistrarSolicitud(ref Solicitud solicitud)
        {
            repositorioSolicitud.Registrar(ref solicitud);
            Cotizar(solicitud.Id, (DateTime)solicitud.FechaCotizacion, solicitud.Usuario.NombreUsuario);
        }
        public void ActualizarSolicitud(ref Solicitud solicitud)
        {
            repositorioSolicitud.Actualizar(ref solicitud);
            Cotizar(solicitud.Id, (DateTime)solicitud.FechaCotizacion, solicitud.Usuario.NombreUsuario);
        }
        public string ActualizarSolicitudesCargaMeler(string xml, string usuario)
        {
            string xmlRespuesta = String.Empty;
            repositorioSolicitud.ActualizarSolicitudesCargaMeler(xml, usuario);
            xmlRespuesta = ObtenerXMLSolicitudesCargaMeler(xml);
            return xmlRespuesta;
        }
        public string ActualizarSolicitudesConfirmacionMeler(string xml, string usuario)
        {
            string xmlRespuesta = String.Empty;
            repositorioSolicitud.ActualizarSolicitudesConfirmacionMeler(xml, usuario);
            xmlRespuesta = ObtenerXMLConfirmacionesCargaMeler(xml);
            return xmlRespuesta;
        }

        public Solicitud ObtenerCotizacionRecalculo(string numeroSolicitud, string usuario)
        {
            var solicitud = repositorioSolicitud.ObtenerCotizacionRecalculo(numeroSolicitud, usuario);
            return solicitud;
        }

        //<GTI.28817-INI>
        public void ProcesarRecalculoCotizacion(string numSolicitud, DateTime fechaCotizacion, double montoCIC, double tipoCambio, string tipoCalculo, string usuario)
        {
            repositorioSolicitud.ProcesarRecalculoCotizacion(numSolicitud, fechaCotizacion, montoCIC, tipoCambio, tipoCalculo, usuario);
            Cotizar(numSolicitud, fechaCotizacion, usuario);
        }

        public DataSet ListarReporteIndicadoresVCTP(DateTime fecPeriodo, string codUsername, string codRol)
        {
            var data = repositorioReportes.ListarReporteIndicadoresVCTP(fecPeriodo, codUsername, codRol);

            return data;
        }

        public Respuesta ListarReporteIndicadoresCDA(DateTime fecPeriodo, string codUsername, string codRol)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                bool estadoPeticion = false;
                string responseContent = string.Empty;

                var data = repositorioReportes.ListarReporteIndicadoresCDA(fecPeriodo, codUsername, codRol);
                respuesta.Data = data;

                if (data.Tables[0].Rows.Count > 0)
                {
                    respuesta.Estado = Constante.COD_OK;
                    respuesta.Mensaje = Utilitarios.FormatearError(new List<string> { "Reporte generado correctamente." });
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Exito.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();
                    respuesta.Contenido = "SiData";

                    try
                    {
                        var listaCuspp = new List<string>();

                        foreach (DataRow row in data.Tables[0].Rows)
                        {
                            listaCuspp.Add(row["Cuspp"].ToString());
                        }

                        log.Info("Comunicación API vTiger");
                        using (var httpClient = new HttpClient())
                        {
                            var objRequest = new
                            {
                                action = "potentials_list_estados_by_cuspp",
                                data = listaCuspp
                            };

                            var content = JsonConvert.SerializeObject(objRequest);
                            log.Info($"RequestContent - Comunicación API vTiger: {content}");

                            string urlVTiger = ConfigurationManager.AppSettings["url_vTiger_api"].ToString();
                            log.Info($"URL - Comunicación API vTiger: {urlVTiger}");

                            var response = httpClient.PostAsync(urlVTiger, new StringContent(content, Encoding.UTF8, "application/json")).Result;
                            responseContent = response.Content.ReadAsStringAsync().Result;
                            log.Info($"ResponseContent - Comunicación API vTiger: {responseContent}");
                            estadoPeticion = response.IsSuccessStatusCode;

                            if (estadoPeticion)
                            {
                                if (!string.IsNullOrEmpty(responseContent) && responseContent != "[]")
                                {
                                    var estadosCuspp = new List<ResponseVTiger>();

                                    var propiedadesJson = JObject.Parse(responseContent).Properties();
                                    foreach (var propiedad in propiedadesJson)
                                    {
                                        var jObject = JObject.Parse(propiedad.Value.ToString());

                                        var estadoCuspp = new ResponseVTiger
                                        {
                                            numCuspp = propiedad.Name,
                                            estado = jObject["estado"].ToString(),
                                            subestado = jObject["subestado"].ToString()
                                        };

                                        estadosCuspp.Add(estadoCuspp);
                                    }

                                    if (estadosCuspp.Count > 0) respuesta.Data = repositorioReportes.ActualizarReporteIndicadoresCDA(data, estadosCuspp);
                                }
                            }
                            else
                            {
                                var htmlDoc = new HtmlDocument();
                                htmlDoc.LoadHtml(responseContent);
                                string error = htmlDoc.DocumentNode.SelectNodes("//p").First().InnerHtml;
                                throw new Exception(error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex.Message, ex);
                        respuesta.Estado = Constante.COD_OK;
                        respuesta.Mensaje = "<div style=\"margin: 5px 0\"><b>Se ha descargado el reporte, no se pudo obtener el campo de estados CRM.</b></div>";
                        respuesta.Titulo = Enums.CuadroMensajeTitulo.Advertencia.StringValue();
                        respuesta.Icono = Enums.CuadroMensajeIcono.Advertencia.StringValue();
                    }
                }
                else
                {
                    log.Error($"No se encontró información para el Reporte de Indicadores CDA, en el periodo {fecPeriodo.ToString("yyyyMMdd")}");
                    respuesta.Estado = Constante.COD_OK;
                    respuesta.Contenido = "NoData";
                    respuesta.Mensaje = "<div style=\"margin: 5px 0\"><b>No se encontró información para el periodo indicado.</b></div>";
                    respuesta.Titulo = Enums.CuadroMensajeTitulo.Advertencia.StringValue();
                    respuesta.Icono = Enums.CuadroMensajeIcono.Advertencia.StringValue();
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><b>No se ha podido completar el proceso debido al siguiente error:</b></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
            }

            return respuesta;
        }
        //<GTI.28817-FIN>

        //<SRIINI20322>
        public void CotizarOficial(string idSolicitud, DateTime fechaCotizacion, string usuario)
        {

            Cotizar(idSolicitud, fechaCotizacion, usuario);
        }
        public double ObtenerMontoACOM(string solicitud, double acom, long cotizacion)
        {
            double monto = repositorioSolicitud.ObtenerMontoACOM(solicitud, acom, cotizacion);
            monto = Math.Round(monto, 0);
            return monto;
        }
        public void ActualizarValidacion(string idSolicitud, string tipoValidacion, string valor, string usuario)
        {
            repositorioSolicitud.ActualizarValidacion(idSolicitud, tipoValidacion, valor, usuario);
        }
        //<SRIFIN20322>
        //<INIGTI_753>
        [DllImport(@"C:\3gl\RviCotMainPlus.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern void cot_main_plus(
                                            [param: MarshalAs(UnmanagedType.LPStr)]
                                            string cot_gls_ikey, string cot_num_soli, int cot_num_coti, int cot_num_mdif,
                                            int cot_num_mgar, int cot_num_nben, int cot_fec_fcal, int cot_fec_fdev,
                                            int cot_num_tpen, int cot_num_tcal, int cot_flg_idac, int cot_flg_igra,
                                            int cot_num_cmon, int cot_num_trea, int cot_num_frea, int cot_flg_irea,
                                            double cot_tas_vrea, double cot_tas_tasa, double cot_val_vpen, double cot_tas_vtra,
                                            double cot_tas_tafp, double cot_val_acom, double cot_val_dcom, double cot_val_puam,
                                            double cot_val_puni, double cot_por_prrt, double cot_val_tgfi, int cot_val_tope,
                                            ref double ppu_vllx, ref double ppu_vllx_vol,
                                            string cot_xml_benefi, string cot_xml_tabico, string cot_xml_parash,
                                            string cot_xml_parinv, string cot_xml_ajutdm, string cot_xml_fluaju,
                                            ref double ppu_arr_fmqx, ref int ppu_arr_inftdm, ref int ppu_arr_inftfm,
                                            ref double ppu_arr_fmqx_sbs, ref int ppu_arr_inftdm_sbs, ref int ppu_arr_inftfm_sbs,
                                            ref double x_cia_val_puud, ref double x_cia_val_devo, //<INIGTI_753>
                                            ref double x_cot_tas_vtva, ref double x_cot_val_mdco, ref double x_cia_val_pens,
                                            ref double x_cia_val_ppag, ref double x_cia_val_puni, ref double x_cia_val_puur,
                                            ref double x_afp_val_pens, ref double x_afp_val_puni, ref double x_afp_val_puur,
                                            ref double x_ash_val_vpen, ref double x_ash_tas_vtva, ref double x_ash_tas_vtra,
                                            ref double x_ash_tas_vtce, ref double x_ash_val_dura,
                                            [param: MarshalAs(UnmanagedType.LPStr), Out()]
                                            StringBuilder x_cot_xml_benefi,
                                            ref int x_cot_num_cmsg);
        //<FINGTI_753>
        //<GTI.INI-15819>
        [DllImport(@"C:\3gl\RviCotMain.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern void cot_main(
                                            [param: MarshalAs(UnmanagedType.LPStr)]
                                            string cot_gls_skey, int cot_num_coti, int cot_num_mdif,
                                            int cot_num_mgar, int cot_num_nben, int cot_fec_fcal, int cot_fec_fdev,
                                            int cot_num_tpen, int cot_num_tcal, int cot_flg_idac, int cot_flg_igra,
                                            int cot_num_cmon, int cot_num_trea, int cot_num_frea, int cot_flg_irea,
                                            double cot_tas_vrea, double cot_tas_tasa, double cot_val_vpen, double cot_tas_vtra,
                                            double cot_tas_tafp, double cot_val_acom, double cot_val_dcom, double cot_val_puam,
                                            double cot_val_puni, double cot_por_prrt, double cot_val_tgfi, int cot_val_tope,
                                            ref double ppu_vllx, ref double ppu_vllx_vol,
                                            string cot_xml_benefi, string cot_xml_tabico, string cot_xml_parash,
                                            string cot_xml_parinv, string cot_xml_ajutdm, string cot_xml_fluaju,
                                            ref double cot_fac_dto,
                                            ref double ppu_arr_fmqx, ref int ppu_arr_inftdm, ref int ppu_arr_inftfm,
                                            ref double ppu_arr_fmqx_sbs, ref int ppu_arr_inftdm_sbs, ref int ppu_arr_inftfm_sbs,
                                            ref double x_cot_tas_vtva, ref double x_cot_val_mdco, ref double x_cia_val_pens,
                                            ref double x_cia_val_ppag, ref double x_cia_val_puni, ref double x_cia_val_puur,
                                            ref double x_afp_val_pens, ref double x_afp_val_puni, ref double x_afp_val_puur,
                                            ref double x_ash_val_vpen, ref double x_ash_tas_vtva, ref double x_ash_tas_vtra,
                                            ref double x_ash_tas_vtce, ref double x_ash_val_dura, ref double x_ash_val_comm,
                                            [param: MarshalAs(UnmanagedType.LPStr), Out()]
                                            StringBuilder x_cot_xml_benefi,
                                            ref int x_cot_num_cmsg);
        //<GTIINI-6489>
        //public static extern void cot_main(
        //                                    [param: MarshalAs(UnmanagedType.LPStr)]
        //                                    string cot_gls_ikey, string cot_num_soli, int cot_num_coti, int cot_num_mdif,
        //                                    int cot_num_mgar, int cot_num_nben, int cot_fec_fcal, int cot_fec_fdev,
        //                                    int cot_num_tpen, int cot_num_tcal, int cot_flg_idac, int cot_flg_igra,
        //                                    int cot_num_cmon, int cot_num_trea, int cot_num_frea, int cot_flg_irea,
        //                                    double cot_tas_vrea, double cot_tas_tasa, double cot_val_vpen, double cot_tas_vtra,
        //                                    double cot_tas_tafp, double cot_val_acom, double cot_val_dcom, double cot_val_puam,
        //                                    double cot_val_puni, double cot_por_prrt, double cot_val_tgfi, int cot_val_tope,
        //                                    ref double ppu_vllx, ref double ppu_vllx_vol,
        //                                    string cot_xml_benefi, string cot_xml_tabico, string cot_xml_parash,
        //                                    string cot_xml_parinv, string cot_xml_ajutdm, string cot_xml_fluaju,
        //                                    ref double ppu_arr_fmqx, ref int ppu_arr_inftdm, ref int ppu_arr_inftfm,
        //                                    ref double ppu_arr_fmqx_sbs, ref int ppu_arr_inftdm_sbs, ref int ppu_arr_inftfm_sbs,
        //                                    ref double x_cot_tas_vtva, ref double x_cot_val_mdco, ref double x_cia_val_pens,
        //                                    ref double x_cia_val_ppag, ref double x_cia_val_puni, ref double x_cia_val_puur,
        //                                    ref double x_afp_val_pens, ref double x_afp_val_puni, ref double x_afp_val_puur,
        //                                    ref double x_ash_val_vpen, ref double x_ash_tas_vtva, ref double x_ash_tas_vtra,
        //                                    ref double x_ash_tas_vtce, ref double x_ash_val_dura, ref double x_ash_val_comm,
        //                                    [param: MarshalAs(UnmanagedType.LPStr), Out()]
        //                                    StringBuilder x_cot_xml_benefi,
        //                                    ref int x_cot_num_cmsg);
        //////<SOLINI-19737>
        ////public static extern void cot_main(
        ////                                    [param: MarshalAs(UnmanagedType.LPStr)]
        ////                                    string cot_gls_ikey, string cot_num_soli, int cot_num_coti, int cot_num_mdif,
        ////                                    int cot_num_mgar, int cot_num_nben, int cot_fec_fcal, int cot_fec_fdev,
        ////                                    int cot_num_tpen, int cot_num_tcal, int cot_flg_idac, int cot_flg_igra,
        ////                                    int cot_num_cmon, int cot_num_trea, int cot_num_frea, int cot_flg_irea,
        ////                                    double cot_tas_vrea, double cot_tas_tasa, double cot_val_vpen, double cot_tas_vtra,
        ////                                    double cot_tas_tafp, double cot_val_acom, double cot_val_dcom, double cot_val_puam,
        ////                                    double cot_val_puni, double cot_por_prrt, double cot_val_tgfi, int cot_val_tope,
        ////                                    ref double ppu_vllx, ref double ppu_vllx_vol,
        ////                                    string cot_xml_benefi, string cot_xml_tabico, string cot_xml_parash,
        ////                                    string cot_xml_parinv, string cot_xml_ajutdm, string cot_xml_fluaju,
        ////                                    ref double ppu_arr_fmqx, ref int ppu_arr_inftdm, ref int ppu_arr_inftfm,
        ////                                    ref double ppu_arr_fmqx_sbs, ref int ppu_arr_inftdm_sbs, ref int ppu_arr_inftfm_sbs,
        ////                                    ref double x_cot_tas_vtva, ref double x_cot_val_mdco, ref double x_cia_val_pens,
        ////                                    ref double x_cia_val_ppag, ref double x_cia_val_puni, ref double x_cia_val_puur,
        ////                                    ref double x_afp_val_pens, ref double x_afp_val_puni, ref double x_afp_val_puur,
        ////                                    ref double x_ash_val_vpen, ref double x_ash_tas_vtva, ref double x_ash_tas_vtra,
        ////                                    ref double x_ash_tas_vtce, ref double x_ash_val_dura,
        ////                                    [param: MarshalAs(UnmanagedType.LPStr), Out()]
        ////                                    StringBuilder x_cot_xml_benefi,
        ////                                    ref int x_cot_num_cmsg);
        //////public static extern void cot_main(
        //////                                    [param: MarshalAs(UnmanagedType.LPStr)]
        //////                                    string cot_gls_ikey, string cot_num_soli, int cot_num_coti, int cot_num_mdif,
        //////                                    int cot_num_mgar, int cot_num_nben, int cot_fec_fcal, int cot_fec_fdev,
        //////                                    int cot_num_tpen, int cot_num_tcal, int cot_flg_idac, int cot_flg_igra,
        //////                                    int cot_num_cmon, int cot_num_trea, int cot_num_frea, int cot_flg_irea,
        //////                                    double cot_tas_vrea, double cot_tas_tasa, double cot_val_vpen, double cot_tas_vtra,
        //////                                    double cot_tas_tafp, double cot_val_acom, double cot_val_dcom, double cot_val_puam,
        //////                                    double cot_val_puni, double cot_por_prrt, double cot_val_tgfi,
        //////                                    ref double ppu_vllx, ref double ppu_arr_vllx,
        //////                                    string cot_xml_benefi, string cot_xml_tabico, string cot_xml_parash,
        //////                                    string cot_xml_parinv, string cot_xml_ajutdm, string cot_xml_fluaju,
        //////                                    ref double x_cot_tas_vtva, ref double x_cot_val_mdco, ref double x_cia_val_pens,
        //////                                    ref double x_cia_val_ppag, ref double x_cia_val_puni, ref double x_cia_val_puur,
        //////                                    ref double x_afp_val_pens, ref double x_afp_val_puni, ref double x_afp_val_puur,
        //////                                    ref double x_ash_val_vpen, ref double x_ash_tas_vtva, ref double x_ash_tas_vtra,
        //////                                    ref double x_ash_tas_vtce, ref double x_ash_val_dura,
        //////                                    [param: MarshalAs(UnmanagedType.LPStr), Out()]
        //////                                    StringBuilder x_cot_xml_benefi,
        //////                                    ref int x_cot_num_cmsg);
        //////<SOLFIN-19737>
        ////<GTIFIN-6489>
        //<GTI.FIN-15819>
        [DllImport(@"C:\3gl\RviFncGene.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi)]
        public static extern double lib_s_round(double nValInicial, int nNumDecimales, int nIndRedondear);
        public List<ParametroCotizadorWS> ObtenerParametrosCotizacion(string idSolicitud, DateTime fechaCotizacion, Int64? numCorrelativo, int anhosAdicionales, double? nuevoCIC)
        {
            var parametros = repositorioSolicitud.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, numCorrelativo, anhosAdicionales, nuevoCIC);
            List<ParametroCotizadorWS> parametrosWS = new List<ParametroCotizadorWS>();
            ParametroCotizadorWS pWS = null;
            parametros.ForEach(p =>
            {
                pWS = new ParametroCotizadorWS
                {
                    /*cod_moneda = p.cod_moneda,
                    cod_tipo_invalidez = p.cod_tipo_invalidez,
                    cod_tipo_pension = p.cod_tipo_pension,
                    cod_tipo_producto = p.cod_tipo_producto,*/
                    cot_fec_fcal = p.cot_fec_fcal,
                    cot_fec_fdev = p.cot_fec_fdev,
                    cot_flg_idac = p.cot_flg_idac,
                    cot_flg_igra = p.cot_flg_igra,
                    cot_flg_irea = p.cot_flg_irea,
                    cot_gls_ikey = p.cot_gls_ikey,
                    cot_num_cmon = p.cot_num_cmon,
                    cot_num_coti = p.cot_num_coti,
                    cot_num_frea = p.cot_num_frea,
                    cot_num_mdif = p.cot_num_mdif,
                    cot_num_mgar = p.cot_num_mgar,
                    cot_num_nben = p.cot_num_nben,
                    cot_num_soli = p.cot_num_soli,
                    cot_num_tcal = p.cot_num_tcal,
                    cot_num_tpen = p.cot_num_tpen,
                    cot_num_trea = p.cot_num_trea,
                    cot_por_prrt = p.cot_por_prrt,
                    cot_tas_tafp = p.cot_tas_tafp,
                    cot_tas_tasa = p.cot_tas_tasa,
                    cot_tas_vrea = p.cot_tas_vrea,
                    cot_tas_vtra = p.cot_tas_vtra,
                    cot_val_acom = p.cot_val_acom,
                    cot_val_dcom = p.cot_val_dcom,
                    cot_val_puam = p.cot_val_puam,
                    cot_val_puni = p.cot_val_puni,
                    cot_val_tgfi = p.cot_val_tgfi,
                    cot_val_vpen = p.cot_val_vpen,
                    cot_xml_ajutdm = p.cot_xml_ajutdm,
                    cot_xml_benefi = p.cot_xml_benefi,
                    cot_xml_fluaju = p.cot_xml_fluaju,
                    cot_xml_parash = p.cot_xml_parash,
                    cot_xml_parinv = p.cot_xml_parinv,
                    cot_xml_tabico = p.cot_xml_tabico
                    /*fec_cotizacion = p.fec_cotizacion,
                    fluaju = p.fluaju,
                    ind_modalidad = p.ind_modalidad,
                    ind_orden = p.ind_orden,
                    val_htit = p.val_htit,
                    val_ltit = p.val_ltit,
                    val_moneda = p.val_moneda,
                    wl_val_pension_minimo = p.wl_val_pension_minimo,
                    wl_XML_Pje = p.wl_XML_Pje,
                    x_afp_val_pens = p.x_afp_val_pens,
                    x_afp_val_puni = p.x_afp_val_puni,
                    x_afp_val_puur = p.x_afp_val_puur,
                    x_ash_tas_vtce = p.x_ash_tas_vtce,
                    x_ash_tas_vtra = p.x_ash_tas_vtra,
                    x_ash_tas_vtva = p.x_ash_tas_vtva,
                    x_ash_val_dura = p.x_ash_val_dura,
                    x_ash_val_vpen = p.x_ash_val_vpen,
                    x_cia_val_pens = p.x_cia_val_pens,
                    x_cia_val_ppag = p.x_cia_val_ppag,
                    x_cia_val_puni = p.x_cia_val_puni,
                    x_cia_val_puur = p.x_cia_val_puur,
                    x_cot_num_cmsg = p.x_cot_num_cmsg,
                    x_cot_tas_vtva = p.x_cot_tas_vtva,
                    x_cot_val_mdco = p.x_cot_val_mdco,
                    x_cot_xml_benefi = p.x_cot_xml_benefi*/
                };
                pWS.ppu_vllx = new double[1321][];
                //pWS.ppu_vllx = new double[132][];
                for (int i = 0; i < 1321; i++)
                //for (int i = 0; i < 132; i++)
                {
                    pWS.ppu_vllx[i] = new double[8];
                    for (int j = 0; j < 8; j++)
                        pWS.ppu_vllx[i][j] = p.ppu_vllx[i, j];
                }
                //////pWS.ppu_arr_vllx = new double[10569][];
                ////////pWS.ppu_arr_vllx = new double[132][];
                //////for (int i = 0; i < 10569; i++)
                ////////for (int i = 0; i < 132; i++)
                //////{
                //////    pWS.ppu_arr_vllx[i] = new double[4];
                //////    for (int j = 0; j < 4; j++)
                //////        pWS.ppu_arr_vllx[i][j] = p.ppu_arr_vllx[i, j];
                //////}
                //<INIGTI_2145>
                pWS.ppu_vllx_vol = new double[1321][];
                for (int i = 0; i < 1321; i++)
                {
                    pWS.ppu_vllx_vol[i] = new double[8];
                    for (int j = 0; j < 8; j++)
                        pWS.ppu_vllx_vol[i][j] = p.ppu_vllx_cot[i, j];
                }
                pWS.ppu_fmqx = new double[1321][]; //double[1320][]
                for (int i = 0; i < 1321; i++)
                {
                    pWS.ppu_fmqx[i] = new double[8];
                    for (int j = 0; j < 8; j++)
                        pWS.ppu_fmqx[i][j] = p.ppu_fmqx[i, j];
                }
                ////ppu_inf_fm
                //p.ppu_inf_fm = new int[3, 8];
                //for (int i = 0; i < 3; i++)
                //    for (int j = 0; j < 8; j++)
                //        p.ppu_inf_fm[i, j] = (int)pWS.ppu_inf_fm[i][j];
                pWS.ppu_inf_fm = new long[3][];
                for (int i = 0; i < 3; i++)
                {
                    pWS.ppu_inf_fm[i] = new long[8];
                    for (int j = 0; j < 8; j++)
                        pWS.ppu_inf_fm[i][j] = p.ppu_inf_fm[i, j];
                }
                ////ppu_inf_tm
                //p.ppu_inf_tm = new int[3, 8];
                //for (int i = 0; i < 3; i++)
                //    for (int j = 0; j < 8; j++)
                //        p.ppu_inf_tm[i, j] = (int)pWS.ppu_inf_tm[i][j];
                pWS.ppu_inf_tm = new long[3][];
                for (int i = 0; i < 3; i++)
                {
                    pWS.ppu_inf_tm[i] = new long[8];
                    for (int j = 0; j < 8; j++)
                        pWS.ppu_inf_tm[i][j] = p.ppu_inf_tm[i, j];
                }
                ////ppu_fmqx_sbs
                //p.ppu_fmqx_sbs = new double[1320, 8];
                //for (int i = 0; i < 1320; i++)
                //    for (int j = 0; j < 8; j++)
                //        p.ppu_fmqx_sbs[i, j] = pWS.ppu_fmqx_sbs[i][j];
                pWS.ppu_fmqx_sbs = new double[1321][];//double[1320][]
                for (int i = 0; i < 1321; i++)
                {
                    pWS.ppu_fmqx_sbs[i] = new double[8];
                    for (int j = 0; j < 8; j++)
                        pWS.ppu_fmqx_sbs[i][j] = p.ppu_fmqx_sbs[i, j];
                }
                ////ppu_inf_tm_sbs
                //p.ppu_inf_tm_sbs = new int[1320, 8];
                //for (int i = 0; i < 1320; i++)
                //    for (int j = 0; j < 8; j++)
                //        p.ppu_inf_tm_sbs[i, j] = pWS.ppu_inf_tm_sbs[i][j];
                pWS.ppu_inf_tm_sbs = new int[3][];
                for (int i = 0; i < 3; i++)
                {
                    pWS.ppu_inf_tm_sbs[i] = new int[8];
                    for (int j = 0; j < 8; j++)
                        pWS.ppu_inf_tm_sbs[i][j] = p.ppu_inf_tm_sbs[i, j];
                }
                ////ppu_inf_fm_sbs
                //p.ppu_inf_fm_sbs = new int[1320, 8];
                //for (int i = 0; i < 1320; i++)
                //    for (int j = 0; j < 8; j++)
                //        p.ppu_inf_fm_sbs[i, j] = pWS.ppu_inf_fm_sbs[i][j];
                pWS.ppu_inf_fm_sbs = new int[3][];
                for (int i = 0; i < 3; i++)
                {
                    pWS.ppu_inf_fm_sbs[i] = new int[8];
                    for (int j = 0; j < 8; j++)
                        pWS.ppu_inf_fm_sbs[i][j] = p.ppu_inf_fm_sbs[i, j];
                }
                //<FINGTI_2145>
                //p.ppu_fmqx = new double[1320, 8];
                //for (int i = 0; i < 1320; i++)
                //    for (int j = 0; j < 8; j++)
                //        p.ppu_fmqx[i, j] = pWS.ppu_fmqx[i][j];
                parametrosWS.Add(pWS);
            });
            return parametrosWS;
        }
        public Cotizacion Cotizar(ParametroCotizadorWS pWS)
        {
            Cotizacion cotizacion = new Cotizacion();
            double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                   x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                   x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                   x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                   x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
            StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
            int x_cot_num_cmsg = 0;
            ParametroCotizador p = null;
            p = new ParametroCotizador
            {
                cot_fec_fcal = pWS.cot_fec_fcal,
                cot_fec_fdev = pWS.cot_fec_fdev,
                cot_flg_idac = pWS.cot_flg_idac,
                cot_flg_igra = pWS.cot_flg_igra,
                cot_flg_irea = pWS.cot_flg_irea,
                cot_gls_ikey = pWS.cot_gls_ikey,
                cot_num_cmon = pWS.cot_num_cmon,
                cot_num_coti = pWS.cot_num_coti,
                cot_num_frea = pWS.cot_num_frea,
                cot_num_mdif = pWS.cot_num_mdif,
                cot_num_mgar = pWS.cot_num_mgar,
                cot_num_nben = pWS.cot_num_nben,
                cot_num_soli = pWS.cot_num_soli,
                cot_num_tcal = pWS.cot_num_tcal,
                cot_num_tpen = pWS.cot_num_tpen,
                cot_num_trea = pWS.cot_num_trea,
                cot_por_prrt = pWS.cot_por_prrt,
                cot_tas_tafp = pWS.cot_tas_tafp,
                cot_tas_tasa = pWS.cot_tas_tasa,
                cot_tas_vrea = pWS.cot_tas_vrea,
                cot_tas_vtra = pWS.cot_tas_vtra,
                cot_val_acom = pWS.cot_val_acom,
                cot_val_dcom = pWS.cot_val_dcom,
                cot_val_puam = pWS.cot_val_puam,
                cot_val_puni = pWS.cot_val_puni,
                cot_val_tgfi = pWS.cot_val_tgfi,
                cot_val_vpen = pWS.cot_val_vpen,
                cot_xml_ajutdm = pWS.cot_xml_ajutdm,
                cot_xml_benefi = pWS.cot_xml_benefi,
                cot_xml_fluaju = pWS.cot_xml_fluaju,
                cot_xml_parash = pWS.cot_xml_parash,
                cot_xml_parinv = pWS.cot_xml_parinv,
                cot_xml_tabico = pWS.cot_xml_tabico,
            };
            p.ppu_vllx = new double[1321, 8];
            for (int i = 0; i < 1321; i++)
                for (int j = 0; j < 8; j++)
                    p.ppu_vllx[i, j] = pWS.ppu_vllx[i][j];
            //<INIGTI_2145>
            //////p.ppu_arr_vllx = new double[10569, 4];
            //////for (int i = 0; i < 10569; i++)
            //////    for (int j = 0; j < 4; j++)
            //////        p.ppu_arr_vllx[i, j] = pWS.ppu_arr_vllx[i][j];
            //<FINGTI_2145>
            //TextWriter tw = null;
            //tw = new StreamWriter("C:\\temp\\sim_" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss.fff") + "_0.txt");
            //tw.WriteLine("wl_cot_kcot        : " + p.cot_gls_ikey);
            //tw.WriteLine("wl_num_solicitud   : " + p.cot_num_soli);
            //tw.WriteLine("wl_num_corr_cotiza : " + p.cot_num_coti);
            //tw.WriteLine("wl_cot_pdif        : " + p.cot_num_mdif);
            //tw.WriteLine("wl_cot_pgar        : " + p.cot_num_mgar);
            //tw.WriteLine("wl_cot_nben        : " + p.cot_num_nben);
            //tw.WriteLine("wl_cot_fcot        : " + p.cot_fec_fcal);
            //tw.WriteLine("wl_cot_fdev        : " + p.cot_fec_fdev);
            //tw.WriteLine("wl_cot_tpen        : " + p.cot_num_tpen);
            //tw.WriteLine("wl_cot_tcal        : " + p.cot_num_tcal);
            //tw.WriteLine("wl_cot_derc        : " + p.cot_flg_idac);
            //tw.WriteLine("wl_cot_grat        : " + p.cot_flg_igra);
            //tw.WriteLine("cot_mon_equi       : " + p.cot_num_cmon);
            //tw.WriteLine("cot_num_trea       : " + p.cot_num_trea);
            //tw.WriteLine("cot_num_frea       : " + p.cot_num_frea);
            //tw.WriteLine("cot_flg_irea       : " + p.cot_flg_irea);
            //tw.WriteLine("wg_val_ajuste_tasa_fija : " + p.cot_tas_vrea);
            //tw.WriteLine("wl_val_tasa_venta  : " + p.cot_tas_tasa);
            //tw.WriteLine("wl_cot_vpen        : " + p.cot_val_vpen);
            //tw.WriteLine("wl_val_vtra        : " + p.cot_tas_vtra);
            //tw.WriteLine("wl_val_tasa_afp    : " + p.cot_tas_tafp);
            //tw.WriteLine("wl_val_acom        : " + p.cot_val_acom);
            //tw.WriteLine("wl_cot_pdco        : " + p.cot_val_dcom);
            //tw.WriteLine("wl_val_puam        : " + p.cot_val_puam);
            //tw.WriteLine("wl_val_puni        : " + p.cot_val_puni);
            //tw.WriteLine("wl_cot_prrt        : " + p.cot_por_prrt);
            //tw.WriteLine("wl_val_tgfi        : " + p.cot_val_tgfi);
            //tw.WriteLine("arr_ppu_vllx       : ");
            //for (int i = 0; i < 1320; i++)
            //    for (int j = 0; j <= 7; j++)
            //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx[i, j]);
            //tw.WriteLine("arr_ppu_aju_vllx   : ");
            //for (int i = 0; i <= 10568; i++)
            //    for (int j = 0; j <= 3; j++)
            //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_arr_vllx[i, j]);
            //tw.WriteLine("cot_xml_ben        : " + p.cot_xml_benefi);
            //tw.WriteLine("cot_xml_icob       : " + p.cot_xml_tabico);
            //tw.WriteLine("cot_xml_ash        : " + p.cot_xml_parash);
            //tw.WriteLine("cot_xml_inv        : " + p.cot_xml_parinv);
            //tw.WriteLine("cot_xml_ajm        : " + p.cot_xml_ajutdm);
            //tw.WriteLine("cot_xml_fdv        : " + p.cot_xml_fluaju);
            //tw.Close();
            //<SOLINI-19737>
            p.ppu_vllx_cot = new double[1321, 8];
            for (int i = 0; i < 1321; i++)
                for (int j = 0; j < 8; j++)
                    p.ppu_vllx_cot[i, j] = pWS.ppu_vllx_vol[i][j];
            p.ppu_fmqx = new double[1321, 8];
            for (int i = 0; i < 1321; i++)
                for (int j = 0; j < 8; j++)
                    p.ppu_fmqx[i, j] = pWS.ppu_fmqx[i][j];
            //<INIGTI_2145>
            //ppu_inf_fm
            p.ppu_inf_fm = new int[3, 8];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 8; j++)
                    p.ppu_inf_fm[i, j] = (int)pWS.ppu_inf_fm[i][j];
            //ppu_inf_tm
            p.ppu_inf_tm = new int[3, 8];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 8; j++)
                    p.ppu_inf_tm[i, j] = (int)pWS.ppu_inf_tm[i][j];
            //ppu_fmqx_sbs
            p.ppu_fmqx_sbs = new double[1321, 8];
            for (int i = 0; i < 1321; i++)
                for (int j = 0; j < 8; j++)
                    p.ppu_fmqx_sbs[i, j] = pWS.ppu_fmqx_sbs[i][j];
            //ppu_inf_tm_sbs
            p.ppu_inf_tm_sbs = new int[3, 8];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 8; j++)
                    p.ppu_inf_tm_sbs[i, j] = pWS.ppu_inf_tm_sbs[i][j];
            //ppu_inf_fm_sbs
            p.ppu_inf_fm_sbs = new int[3, 8];
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 8; j++)
                    p.ppu_inf_fm_sbs[i, j] = pWS.ppu_inf_fm_sbs[i][j];
            //<FINGTI_2145>
            //<SOLFIN-19737>
            // LLamado a la DLL del cotizador (rvicotmain.dll)
            //  cot_main(
            //    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
            //    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
            //    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
            //    p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi,
            //    ref p.ppu_vllx[0, 0], ref p.ppu_arr_vllx[0, 0],
            //    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
            //    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
            //    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
            //    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
            //    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
            //    x_cot_xml_benefi, ref x_cot_num_cmsg);
            //<SOLINIGTI_754>
            //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
            int x_cot_ini_tra2 = 0;
            double x_cot_pje_rent = 0;
            if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
            {
                x_cot_ini_tra2 = p.x_cot_ini_tra2;
                x_cot_pje_rent = p.x_cot_pje_rent;
                p.cot_num_mdif = 0;
                p.cot_por_prrt = 0;
            }
            //<SOLFINGTI_754>
            //TextWriter tw = null;
            ////Imprimir Log DEGUG
            //if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
            //{
            //    //<SOLINIGTI_754>
            //    //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
            //    if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
            //    tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
            //    //<SOLFINGTI_754>
            //    tw.WriteLine("wl_cot_kcot        : " + p.cot_gls_ikey);
            //    tw.WriteLine("wl_num_solicitud   : " + p.cot_num_soli);
            //    tw.WriteLine("wl_num_corr_cotiza : " + p.cot_num_coti);
            //    tw.WriteLine("wl_cot_pdif        : " + p.cot_num_mdif);
            //    tw.WriteLine("wl_cot_pgar        : " + p.cot_num_mgar);
            //    tw.WriteLine("wl_cot_nben        : " + p.cot_num_nben);
            //    tw.WriteLine("wl_cot_fcot        : " + p.cot_fec_fcal);
            //    tw.WriteLine("wl_cot_fdev        : " + p.cot_fec_fdev);
            //    tw.WriteLine("wl_cot_tpen        : " + p.cot_num_tpen);
            //    tw.WriteLine("wl_cot_tcal        : " + p.cot_num_tcal);
            //    tw.WriteLine("wl_cot_derc        : " + p.cot_flg_idac);
            //    tw.WriteLine("wl_cot_grat        : " + p.cot_flg_igra);
            //    tw.WriteLine("cot_mon_equi       : " + p.cot_num_cmon);
            //    tw.WriteLine("cot_num_trea       : " + p.cot_num_trea);
            //    tw.WriteLine("cot_num_frea       : " + p.cot_num_frea);
            //    tw.WriteLine("cot_flg_irea       : " + p.cot_flg_irea);
            //    tw.WriteLine("wg_val_ajuste_tasa_fija : " + p.cot_tas_vrea);
            //    tw.WriteLine("wl_val_tasa_venta  : " + p.cot_tas_tasa);
            //    tw.WriteLine("wl_cot_vpen        : " + p.cot_val_vpen);
            //    tw.WriteLine("wl_val_vtra        : " + p.cot_tas_vtra);
            //    tw.WriteLine("wl_val_tasa_afp    : " + p.cot_tas_tafp);
            //    tw.WriteLine("wl_val_acom        : " + p.cot_val_acom);
            //    tw.WriteLine("wl_cot_pdco        : " + p.cot_val_dcom);
            //    tw.WriteLine("wl_val_puam        : " + p.cot_val_puam);
            //    tw.WriteLine("wl_val_puni        : " + p.cot_val_puni);
            //    tw.WriteLine("wl_cot_prrt        : " + p.cot_por_prrt);
            //    tw.WriteLine("wl_val_tgfi        : " + p.cot_val_tgfi);
            //    tw.WriteLine("wl_val_tope        : " + p.cot_val_tope);
            //    tw.WriteLine("arr_ppu_vllx       : ");
            //    for (int i = 0; i <= 1320; i++)
            //        for (int j = 0; j <= 7; j++)
            //            tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx[i, j]);
            //    //tw.WriteLine("arr_ppu_aju_vllx   : ");
            //    //for (int i = 0; i <= 10568; i++)
            //    //    for (int j = 0; j <= 3; j++)
            //    //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_arr_vllx[i, j]);
            //    tw.WriteLine("arr_ppu_vllx_cot   : ");
            //    for (int i = 0; i <= 1320; i++)
            //        for (int j = 0; j <= 7; j++)
            //            tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx_cot[i, j]);
            //    tw.WriteLine("cot_xml_ben        : " + p.cot_xml_benefi);
            //    tw.WriteLine("cot_xml_icob       : " + p.cot_xml_tabico);
            //    tw.WriteLine("cot_xml_ash        : " + p.cot_xml_parash);
            //    tw.WriteLine("cot_xml_inv        : " + p.cot_xml_parinv);
            //    tw.WriteLine("cot_xml_ajm        : " + p.cot_xml_ajutdm);
            //    tw.WriteLine("cot_xml_fdv        : " + p.cot_xml_fluaju);
            //    tw.WriteLine("arr_ppu_fm_cot     : ");
            //    for (int i = 0; i <= 1320; i++)
            //        for (int j = 0; j <= 7; j++)
            //            tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx[i, j]);
            //    tw.WriteLine("arr_ppu_inf_tm_cot : ");
            //    for (int i = 0; i <= 2; i++)
            //        for (int j = 0; j <= 7; j++)
            //            tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm[i, j]);
            //    tw.WriteLine("arr_ppu_inf_fm_cot : ");
            //    for (int i = 0; i <= 2; i++)
            //        for (int j = 0; j <= 7; j++)
            //            tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm[i, j]);
            //    tw.WriteLine("arr_ppu_fm_sbs     : ");
            //    for (int i = 0; i <= 1320; i++)
            //        for (int j = 0; j <= 7; j++)
            //            tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx_sbs[i, j]);
            //    tw.WriteLine("arr_ppu_inf_tm_sbs : ");
            //    for (int i = 0; i <= 2; i++)
            //        for (int j = 0; j <= 7; j++)
            //            tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm_sbs[i, j]);
            //    tw.WriteLine("arr_ppu_inf_fm_sbs : ");
            //    for (int i = 0; i <= 2; i++)
            //        for (int j = 0; j <= 7; j++)
            //            tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm_sbs[i, j]);
            //    tw.Close();
            //}
            //<GTI.INI-15819>
            string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
            ////<GTIINI-6489>
            ////cot_main(
            ////   p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
            ////   p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
            ////   p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
            ////   p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
            ////   ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
            ////   p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
            ////   p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
            ////   ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
            ////   ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
            ////   ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
            ////   ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
            ////   ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
            ////   x_cot_xml_benefi, ref x_cot_num_cmsg);
            //cot_main(
            //       p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
            //       p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
            //       p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
            //       p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
            //       ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
            //       p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
            //       p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
            //       ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
            //       ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
            //       ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
            //       ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
            //       ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
            //       x_cot_xml_benefi, ref x_cot_num_cmsg);
            ////<GTIFIN-6489>
            cot_main(
                   cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                   p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                   p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                   p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                   ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                   p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                   p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                   ref p.cot_fac_dto[0],
                   ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                   ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                   ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                   ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                   ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                   x_cot_xml_benefi, ref x_cot_num_cmsg);
            //<GTI.FIN-15819>
            cotizacion.PensionCia = x_cia_val_pens;
            cotizacion.PensionAFP = x_afp_val_pens;
            cotizacion.PuurCia = x_afp_val_puur;
            cotizacion.PuurAFP = x_afp_val_puur;
            return cotizacion;
        }
        public Cotizacion Cotizar(string idSolicitud, DateTime fechaCotizacion, Int64 numCorrelativo, int anhosAdicionales, double nuevoCIC)
        {
            Cotizacion cotizacion = new Cotizacion();
            double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                   x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                   x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                   x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                   x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
            StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
            int x_cot_num_cmsg = 0;
            //TextWriter tw = null;
            // Obtener los parámetros de cotización
            var parametros = repositorioSolicitud.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, numCorrelativo, anhosAdicionales, nuevoCIC);
            foreach (ParametroCotizador p in parametros)
            {
                //<SRIINI18360>
                // Para el caso de las Mixtas y Combinadas que cuentan con período garantizado se debe desactivar
                // el periodo garantizado al momento de cotizar la pensión de la AFP ya que este no debe afectarla
                if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                {
                    p.cot_num_mgar = 0;
                }
                //<SRIFIN18360>
                //Imprimir Log DEGUG
                //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                //tw.WriteLine("wl_cot_kcot        : " + p.cot_gls_ikey);
                //tw.WriteLine("wl_num_solicitud   : " + p.cot_num_soli);
                //tw.WriteLine("wl_num_corr_cotiza : " + p.cot_num_coti);
                //tw.WriteLine("wl_cot_pdif        : " + p.cot_num_mdif);
                //tw.WriteLine("wl_cot_pgar        : " + p.cot_num_mgar);
                //tw.WriteLine("wl_cot_nben        : " + p.cot_num_nben);
                //tw.WriteLine("wl_cot_fcot        : " + p.cot_fec_fcal);
                //tw.WriteLine("wl_cot_fdev        : " + p.cot_fec_fdev);
                //tw.WriteLine("wl_cot_tpen        : " + p.cot_num_tpen);
                //tw.WriteLine("wl_cot_tcal        : " + p.cot_num_tcal);
                //tw.WriteLine("wl_cot_derc        : " + p.cot_flg_idac);
                //tw.WriteLine("wl_cot_grat        : " + p.cot_flg_igra);
                //tw.WriteLine("cot_mon_equi       : " + p.cot_num_cmon);
                //tw.WriteLine("cot_num_trea       : " + p.cot_num_trea);
                //tw.WriteLine("cot_num_frea       : " + p.cot_num_frea);
                //tw.WriteLine("cot_flg_irea       : " + p.cot_flg_irea);
                //tw.WriteLine("wg_val_ajuste_tasa_fija : " + p.cot_tas_vrea);
                //tw.WriteLine("wl_val_tasa_venta  : " + p.cot_tas_tasa);
                //tw.WriteLine("wl_cot_vpen        : " + p.cot_val_vpen);
                //tw.WriteLine("wl_val_vtra        : " + p.cot_tas_vtra);
                //tw.WriteLine("wl_val_tasa_afp    : " + p.cot_tas_tafp);
                //tw.WriteLine("wl_val_acom        : " + p.cot_val_acom);
                //tw.WriteLine("wl_cot_pdco        : " + p.cot_val_dcom);
                //tw.WriteLine("wl_val_puam        : " + nuevoCIC);
                //tw.WriteLine("wl_val_puni        : " + nuevoCIC);
                //tw.WriteLine("wl_cot_prrt        : " + p.cot_por_prrt);
                //tw.WriteLine("wl_val_tgfi        : " + p.cot_val_tgfi);
                //tw.WriteLine("arr_ppu_vllx       : ");
                //for (int i = 0; i <= 1320; i++)
                //    for (int j = 0; j <= 7; j++)
                //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx[i, j]);
                //tw.WriteLine("arr_ppu_aju_vllx   : ");
                //for (int i = 0; i <= 10568; i++)
                //    for (int j = 0; j <= 3; j++)
                //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_arr_vllx[i, j]);
                //tw.WriteLine("cot_xml_ben        : " + p.cot_xml_benefi);
                //tw.WriteLine("cot_xml_icob       : " + p.cot_xml_tabico);
                //tw.WriteLine("cot_xml_ash        : " + p.cot_xml_parash);
                //tw.WriteLine("cot_xml_inv        : " + p.cot_xml_parinv);
                //tw.WriteLine("cot_xml_ajm        : " + p.cot_xml_ajutdm);
                //tw.WriteLine("cot_xml_fdv        : " + p.cot_xml_fluaju);
                //tw.Close();
                // LLamado a la DLL del cotizador (rvicotmain.dll)
                //<SOLINIGTI_754>
                //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                int x_cot_ini_tra2 = 0;
                double x_cot_pje_rent = 0;
                if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                {
                    x_cot_ini_tra2 = p.x_cot_ini_tra2;
                    x_cot_pje_rent = p.x_cot_pje_rent; //Convert.ToInt32(p.cot_por_prrt);
                    p.cot_num_mdif = 0;
                    p.cot_por_prrt = 0;
                }
                //<SOLFINGTI_754>
                //<GTI.INI-15819>
                string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                ////<GTIINI-6489>
                ////cot_main(
                ////    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                ////    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                ////    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                ////    p.cot_val_acom, p.cot_val_dcom, nuevoCIC, nuevoCIC, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                ////    ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                ////    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                ////    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                ////    ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                ////    ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                ////    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                ////    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                ////    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                ////    x_cot_xml_benefi, ref x_cot_num_cmsg);
                //cot_main(
                //    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                //    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                //    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                //    p.cot_val_acom, p.cot_val_dcom, nuevoCIC, nuevoCIC, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                //    ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                //    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                //    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                //    ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                //    ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                //    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                //    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                //    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                //    x_cot_xml_benefi, ref x_cot_num_cmsg);
                ////<GTIFIN-6489>
                cot_main(
                    cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    p.cot_val_acom, p.cot_val_dcom, nuevoCIC, nuevoCIC, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    ref p.cot_fac_dto[0],
                    ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                    x_cot_xml_benefi, ref x_cot_num_cmsg);
                //<GTI.FIN-15819>
                cotizacion.PensionCia = x_cia_val_pens;
                cotizacion.PensionAFP = x_afp_val_pens;
                cotizacion.PuurCia = x_afp_val_puur;
                cotizacion.PuurAFP = x_afp_val_puur;
            }
            return cotizacion;
        }
        public void Cotizar(string idSolicitud, DateTime fechaCotizacion, string usuario)
        {
            TextWriter tw = null;
            try
            {
                // Obtener los parámetros de cotización
                var parametros = repositorioSolicitud.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, null, 0, null);
                double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                        x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                        x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                        x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                        x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
                StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
                int x_cot_num_cmsg = 0;
                double val_total_garantizado = 0, cia_pens_mo = 0, cia_ppag_mo = 0, cia_puni_sin_com = 0,
                val_tasa_cesion, pension_mixta_dolares_referencia = 0;
                string wl_XML_CostoBen = String.Empty, wl_XML_Cotiza = String.Empty,
                        XML_CostoBen = String.Empty, XML_Cotiza = String.Empty, XML_Pje = String.Empty;
                RviCotiza RecRviCotiza_XML = new RviCotiza();
                foreach (ParametroCotizador p in parametros)
                {
                    //<SRIINI18360>
                    // Para el caso de las Mixtas y Combinadas que cuentan con período garantizado se debe desactivar
                    // el periodo garantizado al momento de cotizar la pensión de la AFP ya que este no debe afectarla
                    int cot_num_mgar_aux = 0;
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        cot_num_mgar_aux = p.cot_num_mgar;
                        p.cot_num_mgar = 0;
                    }
                    //<SRIFIN18360>
                    //<SOLINIGTI_754>
                    //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                    int x_cot_ini_tra2 = 0;
                    double x_cot_pje_rent = 0;
                    if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                    {
                        x_cot_ini_tra2 = p.x_cot_ini_tra2; //p.cot_num_mdif;
                        x_cot_pje_rent = p.x_cot_pje_rent; // Convert.ToInt32(p.cot_por_prrt);
                        p.cot_num_mdif = 0;
                        p.cot_por_prrt = 0;
                        p.cot_tas_tafp = 0;
                    }
                    //<SOLFINGTI_754>
                    //Imprimir Log DEGUG
                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("wl_cot_kcot        : " + p.cot_gls_ikey);
                        tw.WriteLine("wl_num_solicitud   : " + p.cot_num_soli);
                        tw.WriteLine("wl_num_corr_cotiza : " + p.cot_num_coti);
                        tw.WriteLine("wl_cot_pdif        : " + p.cot_num_mdif);
                        tw.WriteLine("wl_cot_pgar        : " + p.cot_num_mgar);
                        tw.WriteLine("wl_cot_nben        : " + p.cot_num_nben);
                        tw.WriteLine("wl_cot_fcot        : " + p.cot_fec_fcal);
                        tw.WriteLine("wl_cot_fdev        : " + p.cot_fec_fdev);
                        tw.WriteLine("wl_cot_tpen        : " + p.cot_num_tpen);
                        tw.WriteLine("wl_cot_tcal        : " + p.cot_num_tcal);
                        tw.WriteLine("wl_cot_derc        : " + p.cot_flg_idac);
                        tw.WriteLine("wl_cot_grat        : " + p.cot_flg_igra);
                        tw.WriteLine("cot_mon_equi       : " + p.cot_num_cmon);
                        tw.WriteLine("cot_num_trea       : " + p.cot_num_trea);
                        tw.WriteLine("cot_num_frea       : " + p.cot_num_frea);
                        tw.WriteLine("cot_flg_irea       : " + p.cot_flg_irea);
                        tw.WriteLine("wg_val_ajuste_tasa_fija : " + p.cot_tas_vrea);
                        tw.WriteLine("wl_val_tasa_venta  : " + p.cot_tas_tasa);
                        tw.WriteLine("wl_cot_vpen        : " + p.cot_val_vpen);
                        tw.WriteLine("wl_val_vtra        : " + p.cot_tas_vtra);
                        tw.WriteLine("wl_val_tasa_afp    : " + p.cot_tas_tafp);
                        tw.WriteLine("wl_val_acom        : " + p.cot_val_acom);
                        tw.WriteLine("wl_cot_pdco        : " + p.cot_val_dcom);
                        tw.WriteLine("wl_val_puam        : " + p.cot_val_puam);
                        tw.WriteLine("wl_val_puni        : " + p.cot_val_puni);
                        tw.WriteLine("wl_cot_prrt        : " + p.cot_por_prrt);
                        tw.WriteLine("wl_val_tgfi        : " + p.cot_val_tgfi);
                        tw.WriteLine("wl_val_tope        : " + p.cot_val_tope);
                        tw.WriteLine("arr_ppu_vllx       : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx[i, j]);
                        //tw.WriteLine("arr_ppu_aju_vllx   : ");
                        //for (int i = 0; i <= 10568; i++)
                        //    for (int j = 0; j <= 3; j++)
                        //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_arr_vllx[i, j]);
                        tw.WriteLine("arr_ppu_vllx_cot   : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx_cot[i, j]);
                        tw.WriteLine("cot_xml_ben        : " + p.cot_xml_benefi);
                        tw.WriteLine("cot_xml_icob       : " + p.cot_xml_tabico);
                        tw.WriteLine("cot_xml_ash        : " + p.cot_xml_parash);
                        tw.WriteLine("cot_xml_inv        : " + p.cot_xml_parinv);
                        tw.WriteLine("cot_xml_ajm        : " + p.cot_xml_ajutdm);
                        tw.WriteLine("cot_xml_fdv        : " + p.cot_xml_fluaju);
                        tw.WriteLine("arr_ppu_fm_cot     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm[i, j]);
                        tw.WriteLine("arr_ppu_fm_sbs     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm_sbs[i, j]);
                        tw.Close();
                    }
                    // LLamado a la DLL del cotizador (rvicotmain.dll)
                    //<GTI.INI-15819>
                    string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                    ////<GTIINI-6489>
                    //////<SOLINI-19737>
                    //////cot_main(
                    //////    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    //////    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    //////    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    //////    p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi,
                    //////    ref p.ppu_vllx[0, 0], ref p.ppu_arr_vllx[0, 0],
                    //////    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    //////    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    //////    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    //////    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    //////    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                    //////    x_cot_xml_benefi, ref x_cot_num_cmsg);
                    ////cot_main(
                    ////   p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    ////   p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    ////   p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    ////   p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope, 
                    ////   ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                    ////   p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    ////   p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    ////   ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    ////   ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0], 
                    ////   ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    ////   ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    ////   ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                    ////   x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //////<SOLFIN-19737>
                    //cot_main(
                    //   p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    //   p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    //   p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    //   p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    //   ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                    //   p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    //   p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    //   ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    //   ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    //   ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    //   ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    //   ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                    //   x_cot_xml_benefi, ref x_cot_num_cmsg);
                    ////<GTIFIN-6489>
                    cot_main(
                       cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                       p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                       p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                       p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                       ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                       p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                       p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                       ref p.cot_fac_dto[0],
                       ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                       ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                       ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                       ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                       ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                       x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //<GTI.FIN-15819>
                    //<SRIINI18360>
                    // Luego de calcular la pensión regresar al valor original
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        p.cot_num_mgar = cot_num_mgar_aux;
                    }
                    //<SRIFIN18360>
                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_1.txt");
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_1.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("Salida:");
                        tw.WriteLine("");
                        tw.WriteLine("wl_cia_vtva       : " + x_cot_tas_vtva);
                        tw.WriteLine("wl_cot_dcmm       : " + x_cot_val_mdco);
                        tw.WriteLine("wl_cia_pens       : " + x_cia_val_pens);
                        tw.WriteLine("wl_cia_ppag       : " + x_cia_val_ppag);
                        tw.WriteLine("wl_cia_puni       : " + x_cia_val_puni);
                        tw.WriteLine("wl_cia_puur       : " + x_cia_val_puur);
                        tw.WriteLine("wl_afp_pens       : " + x_afp_val_pens);
                        tw.WriteLine("wl_afp_puni       : " + x_afp_val_puni);
                        tw.WriteLine("wl_afp_puur       : " + x_afp_val_puur);
                        tw.WriteLine("wl_ash_vpen       : " + x_ash_val_vpen);
                        tw.WriteLine("wl_ash_vtra       : " + x_ash_tas_vtra);
                        tw.WriteLine("wl_ash_vtce       : " + x_ash_tas_vtce);
                        tw.WriteLine("wl_ash_dura       : " + x_ash_val_dura);
                        tw.WriteLine("cot_xml_benefi    : " + x_cot_xml_benefi.ToString().Trim());
                        tw.WriteLine("wl_num_error      : " + x_cot_num_cmsg);
                        tw.Close();
                    }
                    //<SOLINI25781>
                    double x_1era_prima_is = 0;
                    //<SOLFIN25781>
                    // Para la segunda pasada no se debe volver a calcular el total garantizado
                    if (p.ind_orden == 1)
                    {
                        // Secuencia post-cotización (Traducción extraída del s_renvi)
                        val_total_garantizado = 0;
                        if (p.cot_num_mgar > 0)
                        {
                            if (p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue())
                            {
                                val_total_garantizado = x_cia_val_ppag * p.cot_num_mgar;
                                if (p.cod_moneda == Enums.Moneda.Dolares.StringValue())
                                {
                                    val_total_garantizado /= p.val_moneda;
                                }
                            }
                            else
                            {
                                if (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue())
                                {
                                    for (int i = p.cot_num_mdif; i < (p.cot_num_mgar + p.cot_num_mdif); i++)
                                    {
                                        val_total_garantizado += (x_cia_val_ppag * p.fluaju[i]);
                                    }
                                    //<SRIINI18360>
                                    // El total garantizado de la modalidad Bimoneda debe mostrarse en Soles
                                    // por lo tanto no debe convertirse.
                                    //if (p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue())
                                    if (p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue() && p.ind_modalidad != Enums.Modalidad.Bimoneda.StringValue())
                                    //<SRIFIN18360>
                                    {
                                        val_total_garantizado /= p.val_moneda;
                                    }
                                }
                            }
                        }
                        else
                        {
                            val_total_garantizado = 0;
                        }
                        //<SOLINI25781>
                        if (p.ind_modalidad == Enums.Modalidad.Diferida.StringValue())// Enums.Modalidad.Mixta.StringValue())
                        {
                            //<GTIINI-576>
                            //x_1era_prima_is = x_cia_val_ppag * p.fluaju[Convert.ToInt32(p.cot_num_mdif)];
                            x_1era_prima_is = x_cia_val_ppag * p.fluaju[Convert.ToInt32(p.cot_num_mdif)] / p.val_moneda;
                            //<GTIFIN-576>
                        }
                        //<SOLFIN25781>
                        //<SOLINIGTI_754>
                        if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                        {
                            x_1era_prima_is = x_cia_val_ppag * p.fluaju[Convert.ToInt32(x_cot_ini_tra2)] / p.val_moneda;
                            x_1era_prima_is *= (Convert.ToDouble(x_cot_pje_rent) / 100);
                        }
                        //<SOLFINGTI_754>
                    }
                    if (x_cot_num_cmsg > 0)
                    {
                        x_cot_tas_vtva = 0; x_cot_val_mdco = 0; x_cia_val_pens = 0;
                        x_cia_val_ppag = 0; x_cia_val_puni = 0; x_cia_val_puur = 0;
                        x_afp_val_pens = 0; x_afp_val_puni = 0; x_afp_val_puur = 0;
                        x_ash_val_vpen = 0; x_ash_tas_vtce = 0; x_ash_val_dura = 0;
                    }
                    //<GTIINI-1092>
                    // Se vuelve a descomentar el cambio de tipo de Cálculo a 2 para mixtas y combinadas para
                    // que se guarden así en la base de datos
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue()) && ((p.cod_moneda == Enums.Moneda.Soles.StringValue()) || (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue())))
                    {
                        p.cot_num_tcal = 2;
                    }
                    //<SRIINI18360>
                    if ((p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && (p.ind_orden == 1))
                    {
                        p.cot_num_tcal = 2;
                    }
                    //<SRIFIN18360>
                    //<GTIfIN-1092>
                    cia_puni_sin_com = x_cia_val_puni;
                    val_tasa_cesion = 0;
                    if (p.val_moneda != 0)
                    {
                        cia_pens_mo = x_cia_val_pens / p.val_moneda;
                        cia_ppag_mo = x_cia_val_ppag / p.val_moneda;
                    }
                    else
                    {
                        cia_pens_mo = 0;
                        cia_ppag_mo = 0;
                    }
                    cia_pens_mo = lib_s_round(cia_pens_mo, 2, 0);
                    cia_ppag_mo = lib_s_round(cia_ppag_mo, 2, 0);
                    // Actualiza rvi_cotiza
                    // --------------------
                    RecRviCotiza_XML.cod_estado_cotizacion = Constante.COD_COTIZACION_CALCULADA;
                    RecRviCotiza_XML.val_descuento_comision = x_cot_val_mdco.ToString();
                    RecRviCotiza_XML.cod_tipo_calculo = p.cot_num_tcal.ToString();
                    RecRviCotiza_XML.num_solicitud = p.cot_num_soli;
                    //<SOLINI25781>
                    RecRviCotiza_XML.val_1era_prima_is = x_1era_prima_is;
                    //<SOLFIN25781>
                    //<GTIINI-6489>
                    RecRviCotiza_XML.val_mto_gasto_comision = x_ash_val_comm;
                    //<GTIFIN-6489>
                    //<SRIINI18360>
                    // Si NO es Bimoneda NI Renta Mixta NI Renta Combinada
                    //if (p.ind_modalidad != Enums.Modalidad.Mixta.StringValue() && p.ind_modalidad != Enums.Modalidad.Bimoneda.StringValue())
                    if (p.ind_modalidad != Enums.Modalidad.Mixta.StringValue() && p.ind_modalidad != Enums.Modalidad.Combinada.StringValue() && p.ind_modalidad != Enums.Modalidad.Bimoneda.StringValue())
                    //<SRIFIN18360>
                    {
                        RecRviCotiza_XML.val_fac_cia = x_cia_val_puur.ToString();
                        RecRviCotiza_XML.val_mto_cia = x_cia_val_puni.ToString();
                        RecRviCotiza_XML.val_mto_cia_sin_comision = cia_puni_sin_com.ToString();
                        RecRviCotiza_XML.val_pen_cia = x_cia_val_ppag.ToString();
                        RecRviCotiza_XML.val_pen_cia_mo = cia_ppag_mo.ToString();
                        RecRviCotiza_XML.val_pen_ref = x_cia_val_pens.ToString();
                        RecRviCotiza_XML.val_pen_ref_mo = cia_pens_mo.ToString();
                        RecRviCotiza_XML.val_tasa_int_vit = x_cot_tas_vtva.ToString("##.0000000000");
                        RecRviCotiza_XML.val_tasa_venta_ash = x_ash_tas_vtva.ToString();
                        RecRviCotiza_XML.val_tasa_ret_accion = x_ash_tas_vtra.ToString();
                        RecRviCotiza_XML.val_tasa_costo_equiv = x_ash_tas_vtce.ToString();
                        RecRviCotiza_XML.val_duration = x_ash_val_dura.ToString();
                        RecRviCotiza_XML.num_error_cot = x_cot_num_cmsg.ToString();
                        RecRviCotiza_XML.val_tasa_int_temp = p.cot_tas_tafp.ToString();
                        RecRviCotiza_XML.val_fac_afp = x_afp_val_puur.ToString();
                        RecRviCotiza_XML.val_mto_afp = x_afp_val_puni.ToString();
                        RecRviCotiza_XML.val_pen_afp = x_afp_val_pens.ToString();
                        RecRviCotiza_XML.val_afp_pen_ref = "0";
                        RecRviCotiza_XML.val_tasa_venta_ash_2 = "0";
                        RecRviCotiza_XML.val_tasa_ret_accion_2 = "0";
                        RecRviCotiza_XML.val_tasa_costo_equiv_2 = "0";
                        RecRviCotiza_XML.val_duration_2 = "0";
                        RecRviCotiza_XML.num_error_cot_2 = "0";
                        RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                        RecRviCotiza_XML.val_tasa_cesion_moneda2 = "";
                        // verificación de cotización sin error (no alcanza el TRA mínimo)
                        if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                        {
                            RecRviCotiza_XML.ind_cotiza = "**";
                        }
                        else
                        {
                            RecRviCotiza_XML.ind_cotiza = " ";
                        }
                        //<INIGTI_1092>
                        if (x_ash_tas_vtra < p.wl_val_ltra)
                        {
                            RecRviCotiza_XML.ind_cotiza = "**";
                            RecRviCotiza_XML.num_error_cot = "300";
                        }
                        //<FINGTI_1092>
                        if (RecRviCotiza_XML.ind_cotiza == " ")
                        {
                            if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                            {
                                RecRviCotiza_XML.ind_cotiza = " ";
                            }
                            else
                            {
                                if (p.ind_modalidad == Enums.Modalidad.Diferida.StringValue())
                                {
                                    RecRviCotiza_XML.ind_cotiza = "?";
                                }
                            }
                        }
                        RecRviCotiza_XML.val_total_garantizado = val_total_garantizado;
                    }
                    // Si es Bimoneda o Renta Mixta o Renta Combinada
                    else
                    {
                        // Si es Dólares
                        //<SRIINI18360>
                        //if (p.cod_moneda == Enums.Moneda.Dolares.StringValue() || p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue())
                        if (
                            //(p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() && p.cod_moneda == Enums.Moneda.Dolares.StringValue()) ||
                            //(p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() && p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()) ||
                            //(p.ind_modalidad == Enums.Modalidad.Combinada.StringValue() && p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue()) ||
                            //(p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue() && p.cod_moneda == Enums.Moneda.Dolares.StringValue()) ||
                            //(p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue() && p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue())
                            p.ind_orden == 1
                           )
                        //<SRIFIN18360>
                        {
                            RecRviCotiza_XML.val_fac_cia = x_cia_val_puur.ToString();
                            RecRviCotiza_XML.val_mto_cia = x_cia_val_puni.ToString();
                            RecRviCotiza_XML.val_mto_cia_sin_comision = cia_puni_sin_com.ToString();
                            RecRviCotiza_XML.val_pen_cia = x_cia_val_ppag.ToString();
                            RecRviCotiza_XML.val_pen_cia_mo = cia_ppag_mo.ToString();
                            RecRviCotiza_XML.val_pen_ref = x_cia_val_pens.ToString();
                            RecRviCotiza_XML.val_pen_ref_mo = cia_pens_mo.ToString();
                            RecRviCotiza_XML.val_tasa_int_vit = x_cot_tas_vtva.ToString("##.0000000000");
                            RecRviCotiza_XML.val_tasa_venta_ash = x_ash_tas_vtva.ToString();
                            RecRviCotiza_XML.val_tasa_ret_accion = x_ash_tas_vtra.ToString();
                            RecRviCotiza_XML.val_tasa_costo_equiv = x_ash_tas_vtce.ToString();
                            RecRviCotiza_XML.val_duration = x_ash_val_dura.ToString();
                            RecRviCotiza_XML.num_error_cot = x_cot_num_cmsg.ToString();
                            if (p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                                // verificación de cotización sin error (no alcanza el TRA mínimo)
                                if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                }
                                else
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                //<INIGTI_1092>
                                if (x_ash_tas_vtra < p.wl_val_ltra)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                    RecRviCotiza_XML.num_error_cot = "300";
                                }
                                //<FINGTI_1092>
                            }
                            else
                            {
                                RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                                pension_mixta_dolares_referencia = x_cia_val_pens;
                                if ((p.cod_tipo_pension == Enums.TipoPension.InvalidezConCobertura.StringValue()) && (p.cod_tipo_invalidez == Enums.TipoInvalidez.Total.StringValue() || p.cod_tipo_invalidez == Enums.TipoInvalidez.Parcial.StringValue()))
                                {
                                    if (p.cod_tipo_invalidez == Enums.TipoInvalidez.Parcial.StringValue())
                                    {
                                        pension_mixta_dolares_referencia /= 0.5;
                                    }
                                    else if (p.cod_tipo_invalidez == Enums.TipoInvalidez.Total.StringValue())
                                    {
                                        pension_mixta_dolares_referencia /= 0.7;
                                    }
                                }
                                // verificación de pensión mínima para Renta Mixta en Dólares
                                if (pension_mixta_dolares_referencia < p.wl_val_pension_minimo)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "*";
                                }
                                else
                                {
                                    RecRviCotiza_XML.ind_cotiza = String.Empty;
                                }
                                // verificación de cotización sin error (no alcanza el TRA mínimo)
                                if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                }
                                //<INIGTI_1092>
                                if (x_ash_tas_vtra < p.wl_val_ltra)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                    RecRviCotiza_XML.num_error_cot = "300";
                                }
                                //<FINGTI_1092>
                            }
                            if (RecRviCotiza_XML.ind_cotiza == " " || RecRviCotiza_XML.ind_cotiza == String.Empty)
                            {
                                if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                else
                                {
                                    if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue())
                                    {
                                        RecRviCotiza_XML.ind_cotiza = "?";
                                    }
                                }
                            }
                            RecRviCotiza_XML.val_total_garantizado = val_total_garantizado;
                        }
                        // Si es la segunda pasada
                        else
                        {
                            RecRviCotiza_XML.val_tasa_int_temp = x_cot_tas_vtva.ToString("##.0000000000");
                            RecRviCotiza_XML.val_fac_afp = x_cia_val_puur.ToString();
                            RecRviCotiza_XML.val_mto_afp = x_cia_val_puni.ToString();
                            RecRviCotiza_XML.val_pen_afp = x_cia_val_ppag.ToString();
                            RecRviCotiza_XML.val_afp_pen_ref = x_cia_val_pens.ToString();
                            if (p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.val_tasa_venta_ash_2 = x_ash_tas_vtva.ToString();
                                RecRviCotiza_XML.val_tasa_ret_accion_2 = x_ash_tas_vtra.ToString();
                                RecRviCotiza_XML.val_tasa_costo_equiv_2 = x_ash_tas_vtce.ToString();
                                RecRviCotiza_XML.val_duration_2 = x_ash_val_dura.ToString();
                                RecRviCotiza_XML.num_error_cot_2 = x_cot_num_cmsg.ToString();
                                RecRviCotiza_XML.val_tasa_cesion_moneda2 = val_tasa_cesion.ToString();
                            }
                            else
                            {
                                RecRviCotiza_XML.val_tasa_venta_ash_2 = "0";
                                RecRviCotiza_XML.val_tasa_ret_accion_2 = "0";
                                RecRviCotiza_XML.val_tasa_costo_equiv_2 = "0";
                                RecRviCotiza_XML.val_duration_2 = "0";
                                RecRviCotiza_XML.num_error_cot_2 = "0";
                                RecRviCotiza_XML.val_tasa_cesion = "0";
                                RecRviCotiza_XML.val_tasa_cesion_moneda2 = "0";
                            }
                            // verificación de cotización sin error (no alcanza el TRA mínimo)
                            if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot_2) > 0 && p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.ind_cotiza = "**";
                            }
                            //<INIGTI_1092>
                            if (x_ash_tas_vtra < p.wl_val_ltra)
                            {
                                RecRviCotiza_XML.ind_cotiza = "**";
                                RecRviCotiza_XML.num_error_cot = "300";
                            }
                            //<FINGTI_1092>
                            if (RecRviCotiza_XML.ind_cotiza == " ")
                            {
                                if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                else
                                {
                                    //<SRIINI18360>
                                    //if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue())
                                    if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue())
                                    //<SRIFIN18360>
                                    {
                                        RecRviCotiza_XML.ind_cotiza = "?";
                                    }
                                }
                            }
                            RecRviCotiza_XML.val_total_garantizado = val_total_garantizado;
                        }
                    }
                    //<SRIINI18360>
                    //if (!((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue()) && (p.cod_moneda == Enums.Moneda.Dolares.StringValue() || p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue())))
                    if (!(p.ind_orden == 2))
                    //<SRIFIN18360>
                    {
                        wl_XML_CostoBen = x_cot_xml_benefi.ToString().Trim();
                    }
                    //<SRIINI18360>
                    //if (!((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue()) && p.ind_orden == 1))
                    if (!((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue() || p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue()) && p.ind_orden == 1))
                    //<SRIFIN18360>
                    {
                        if (RecRviCotiza_XML.ind_cotiza == String.Empty)
                        {
                            RecRviCotiza_XML.ind_cotiza = " ";
                        }
                        wl_XML_Cotiza += "<cotiza>";
                        wl_XML_Cotiza += " <num_solicitud>" + RecRviCotiza_XML.num_solicitud + "</num_solicitud>";
                        wl_XML_Cotiza += " <fec_cotizacion>" + p.fec_cotizacion.ToString("dd/MM/yyyy") + "</fec_cotizacion>";
                        wl_XML_Cotiza += " <num_correlativo>" + p.cot_num_coti.ToString() + "</num_correlativo>";
                        wl_XML_Cotiza += " <cod_estado_cotizacion>" + RecRviCotiza_XML.cod_estado_cotizacion + "</cod_estado_cotizacion>";
                        wl_XML_Cotiza += " <val_descuento_comision>" + RecRviCotiza_XML.val_descuento_comision + "</val_descuento_comision>";
                        wl_XML_Cotiza += " <cod_tipo_calculo>" + RecRviCotiza_XML.cod_tipo_calculo + "</cod_tipo_calculo>";
                        wl_XML_Cotiza += " <ind_cotiza>" + RecRviCotiza_XML.ind_cotiza + "</ind_cotiza>";
                        wl_XML_Cotiza += " <val_fac_cia>" + RecRviCotiza_XML.val_fac_cia + "</val_fac_cia>";
                        wl_XML_Cotiza += " <val_mto_cia>" + RecRviCotiza_XML.val_mto_cia + "</val_mto_cia>";
                        wl_XML_Cotiza += " <val_mto_cia_sin_comision>" + RecRviCotiza_XML.val_mto_cia_sin_comision + "</val_mto_cia_sin_comision>";
                        wl_XML_Cotiza += " <val_pen_cia>" + RecRviCotiza_XML.val_pen_cia + "</val_pen_cia>";
                        wl_XML_Cotiza += " <val_pen_cia_mo>" + RecRviCotiza_XML.val_pen_cia_mo + "</val_pen_cia_mo>";
                        wl_XML_Cotiza += " <val_pen_ref>" + RecRviCotiza_XML.val_pen_ref + "</val_pen_ref>";
                        wl_XML_Cotiza += " <val_pen_ref_mo>" + RecRviCotiza_XML.val_pen_ref_mo + "</val_pen_ref_mo>";
                        wl_XML_Cotiza += " <val_tasa_int_vit>" + RecRviCotiza_XML.val_tasa_int_vit + "</val_tasa_int_vit>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash>" + RecRviCotiza_XML.val_tasa_venta_ash + "</val_tasa_venta_ash>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion>" + RecRviCotiza_XML.val_tasa_ret_accion + "</val_tasa_ret_accion>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv>" + RecRviCotiza_XML.val_tasa_costo_equiv + "</val_tasa_costo_equiv>";
                        wl_XML_Cotiza += " <val_duration>" + RecRviCotiza_XML.val_duration + "</val_duration>";
                        wl_XML_Cotiza += " <num_error_cot>" + RecRviCotiza_XML.num_error_cot + "</num_error_cot>";
                        wl_XML_Cotiza += " <val_tasa_int_temp>" + RecRviCotiza_XML.val_tasa_int_temp + "</val_tasa_int_temp>";
                        wl_XML_Cotiza += " <val_fac_afp>" + RecRviCotiza_XML.val_fac_afp + "</val_fac_afp>";
                        wl_XML_Cotiza += " <val_mto_afp>" + RecRviCotiza_XML.val_mto_afp + "</val_mto_afp>";
                        wl_XML_Cotiza += " <val_pen_afp>" + RecRviCotiza_XML.val_pen_afp + "</val_pen_afp>";
                        wl_XML_Cotiza += " <val_afp_pen_ref>" + RecRviCotiza_XML.val_afp_pen_ref + "</val_afp_pen_ref>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash_2>" + RecRviCotiza_XML.val_tasa_venta_ash_2 + "</val_tasa_venta_ash_2>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion_2>" + RecRviCotiza_XML.val_tasa_ret_accion_2 + "</val_tasa_ret_accion_2>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv_2>" + RecRviCotiza_XML.val_tasa_costo_equiv_2 + "</val_tasa_costo_equiv_2>";
                        wl_XML_Cotiza += " <val_duration_2>" + RecRviCotiza_XML.val_duration_2 + "</val_duration_2>";
                        wl_XML_Cotiza += " <num_error_cot_2>" + RecRviCotiza_XML.num_error_cot_2 + "</num_error_cot_2>";
                        wl_XML_Cotiza += " <val_tasa_cesion>" + RecRviCotiza_XML.val_tasa_cesion + "</val_tasa_cesion>";
                        wl_XML_Cotiza += " <val_tasa_cesion_moneda2>" + RecRviCotiza_XML.val_tasa_cesion_moneda2 + "</val_tasa_cesion_moneda2>";
                        //if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Mixta.StringValue())
                        if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue() || p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                        {
                            if ((p.cod_moneda == Enums.Moneda.Dolares.StringValue()) || (p.cod_moneda == Enums.Moneda.Soles.StringValue()))
                            {
                                wl_XML_Cotiza += " <cod_moneda>" + Enums.Moneda.Dolares.StringValue() + "</cod_moneda>";
                            }
                            else if ((p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()) || (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue()))
                            {
                                wl_XML_Cotiza += " <cod_moneda>" + Enums.Moneda.DolaresAjustados.StringValue() + "</cod_moneda>";
                            }
                        }
                        else
                        {
                            wl_XML_Cotiza += " <cod_moneda>" + p.cod_moneda + "</cod_moneda>";
                        }
                        wl_XML_Cotiza += " <wl_cod_username>" + usuario + "</wl_cod_username>";
                        wl_XML_Cotiza += " <val_total_garantizado>" + RecRviCotiza_XML.val_total_garantizado + "</val_total_garantizado>";
                        //<SOLINI25781>
                        wl_XML_Cotiza += " <val_1era_prima_is>" + RecRviCotiza_XML.val_1era_prima_is + "</val_1era_prima_is>";
                        //<SOLFIN25781>
                        //<GTIINI-6489>
                        wl_XML_Cotiza += " <val_mto_gasto_comision>" + RecRviCotiza_XML.val_mto_gasto_comision + "</val_mto_gasto_comision>";
                        //<GTIFIN-6489>
                        wl_XML_Cotiza += "</cotiza>";
                        XML_Pje = p.wl_XML_Pje;
                        XML_CostoBen += wl_XML_CostoBen;
                        XML_Cotiza += wl_XML_Cotiza;
                        if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                        {
                            //<SOLINIGTI_754>
                            //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_2.txt");
                            if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                            tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_2.txt");
                            //<SOLFINGTI_754>
                            tw.WriteLine("");
                            tw.WriteLine("XML_INSERCION");
                            tw.WriteLine("wl_XML_Pje        : " + XML_Pje);
                            tw.WriteLine("wl_XML_CostoBen   : " + XML_CostoBen);
                            tw.WriteLine("wl_XML_Cotiza     : " + XML_Cotiza);
                            tw.Close();
                        }
                    }
                }
                string correlativos = string.Empty;
                parametros.ForEach(p => correlativos += p.cot_num_coti.ToString() + ",");
                correlativos = correlativos.Substring(0, correlativos.Length - 1);
                // Guardar los resultados en Base de Datos
                repositorioSolicitud.RegistrarPjeBeneficiarios(idSolicitud, correlativos, XML_Pje, "<insert>" + XML_CostoBen + "</insert>", usuario);
                repositorioSolicitud.RegistrarCotiza("<insert>" + XML_Cotiza + "</insert>", usuario);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //<INIGTI_4081>

        //Duplicado del metodo "Cotizar" para retonar generación de xml y desacoplar la parte de persistencia en rentas-api-cotizador-rv
        public RespuestaCotizacion ProcesarCotizacion(string idSolicitud, DateTime fechaCotizacion, string usuario)
        {

            var resultado = new RespuestaCotizacion();
            TextWriter tw = null;

            try
            {
                // Obtener los parámetros de cotización
                var parametros = repositorioSolicitud.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, null, 0, null);
                double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                        x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                        x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                        x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                        x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
                StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
                int x_cot_num_cmsg = 0;
                double val_total_garantizado = 0, cia_pens_mo = 0, cia_ppag_mo = 0, cia_puni_sin_com = 0,
                val_tasa_cesion, pension_mixta_dolares_referencia = 0;
                string wl_XML_CostoBen = String.Empty, wl_XML_Cotiza = String.Empty,
                        XML_CostoBen = String.Empty, XML_Cotiza = String.Empty, XML_Pje = String.Empty;
                RviCotiza RecRviCotiza_XML = new RviCotiza();

                foreach (ParametroCotizador p in parametros)
                {
                    //<SRIINI18360>
                    // Para el caso de las Mixtas y Combinadas que cuentan con período garantizado se debe desactivar
                    // el periodo garantizado al momento de cotizar la pensión de la AFP ya que este no debe afectarla
                    int cot_num_mgar_aux = 0;
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        cot_num_mgar_aux = p.cot_num_mgar;
                        p.cot_num_mgar = 0;
                    }
                    //<SRIFIN18360>
                    //<SOLINIGTI_754>
                    //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                    int x_cot_ini_tra2 = 0;
                    double x_cot_pje_rent = 0;
                    if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                    {
                        x_cot_ini_tra2 = p.x_cot_ini_tra2; //p.cot_num_mdif;
                        x_cot_pje_rent = p.x_cot_pje_rent; // Convert.ToInt32(p.cot_por_prrt);
                        p.cot_num_mdif = 0;
                        p.cot_por_prrt = 0;
                        p.cot_tas_tafp = 0;
                    }
                    //<SOLFINGTI_754>

                    //Imprimir Log DEGUG
                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("wl_cot_kcot        : " + p.cot_gls_ikey);
                        tw.WriteLine("wl_num_solicitud   : " + p.cot_num_soli);
                        tw.WriteLine("wl_num_corr_cotiza : " + p.cot_num_coti);
                        tw.WriteLine("wl_cot_pdif        : " + p.cot_num_mdif);
                        tw.WriteLine("wl_cot_pgar        : " + p.cot_num_mgar);
                        tw.WriteLine("wl_cot_nben        : " + p.cot_num_nben);
                        tw.WriteLine("wl_cot_fcot        : " + p.cot_fec_fcal);
                        tw.WriteLine("wl_cot_fdev        : " + p.cot_fec_fdev);
                        tw.WriteLine("wl_cot_tpen        : " + p.cot_num_tpen);
                        tw.WriteLine("wl_cot_tcal        : " + p.cot_num_tcal);
                        tw.WriteLine("wl_cot_derc        : " + p.cot_flg_idac);
                        tw.WriteLine("wl_cot_grat        : " + p.cot_flg_igra);
                        tw.WriteLine("cot_mon_equi       : " + p.cot_num_cmon);
                        tw.WriteLine("cot_num_trea       : " + p.cot_num_trea);
                        tw.WriteLine("cot_num_frea       : " + p.cot_num_frea);
                        tw.WriteLine("cot_flg_irea       : " + p.cot_flg_irea);
                        tw.WriteLine("wg_val_ajuste_tasa_fija : " + p.cot_tas_vrea);
                        tw.WriteLine("wl_val_tasa_venta  : " + p.cot_tas_tasa);
                        tw.WriteLine("wl_cot_vpen        : " + p.cot_val_vpen);
                        tw.WriteLine("wl_val_vtra        : " + p.cot_tas_vtra);
                        tw.WriteLine("wl_val_tasa_afp    : " + p.cot_tas_tafp);
                        tw.WriteLine("wl_val_acom        : " + p.cot_val_acom);
                        tw.WriteLine("wl_cot_pdco        : " + p.cot_val_dcom);
                        tw.WriteLine("wl_val_puam        : " + p.cot_val_puam);
                        tw.WriteLine("wl_val_puni        : " + p.cot_val_puni);
                        tw.WriteLine("wl_cot_prrt        : " + p.cot_por_prrt);
                        tw.WriteLine("wl_val_tgfi        : " + p.cot_val_tgfi);
                        tw.WriteLine("wl_val_tope        : " + p.cot_val_tope);
                        tw.WriteLine("arr_ppu_vllx       : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx[i, j]);

                        tw.WriteLine("arr_ppu_vllx_cot   : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx_cot[i, j]);
                        tw.WriteLine("cot_xml_ben        : " + p.cot_xml_benefi);
                        tw.WriteLine("cot_xml_icob       : " + p.cot_xml_tabico);
                        tw.WriteLine("cot_xml_ash        : " + p.cot_xml_parash);
                        tw.WriteLine("cot_xml_inv        : " + p.cot_xml_parinv);
                        tw.WriteLine("cot_xml_ajm        : " + p.cot_xml_ajutdm);
                        tw.WriteLine("cot_xml_fdv        : " + p.cot_xml_fluaju);
                        tw.WriteLine("arr_ppu_fm_cot     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm[i, j]);
                        tw.WriteLine("arr_ppu_fm_sbs     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm_sbs[i, j]);
                        tw.Close();
                    }

                    // LLamado a la DLL del cotizador (rvicotmain.dll)
                    //<GTI.INI-15819>
                    string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                    cot_main(
                       cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                       p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                       p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                       p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                       ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                       p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                       p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                       ref p.cot_fac_dto[0],
                       ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                       ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                       ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                       ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                       ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                       x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //<GTI.FIN-15819>

                    //<SRIINI18360>
                    // Luego de calcular la pensión regresar al valor original
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        p.cot_num_mgar = cot_num_mgar_aux;
                    }
                    //<SRIFIN18360>

                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_1.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("Salida:");
                        tw.WriteLine("");
                        tw.WriteLine("wl_cia_vtva       : " + x_cot_tas_vtva);
                        tw.WriteLine("wl_cot_dcmm       : " + x_cot_val_mdco);
                        tw.WriteLine("wl_cia_pens       : " + x_cia_val_pens);
                        tw.WriteLine("wl_cia_ppag       : " + x_cia_val_ppag);
                        tw.WriteLine("wl_cia_puni       : " + x_cia_val_puni);
                        tw.WriteLine("wl_cia_puur       : " + x_cia_val_puur);
                        tw.WriteLine("wl_afp_pens       : " + x_afp_val_pens);
                        tw.WriteLine("wl_afp_puni       : " + x_afp_val_puni);
                        tw.WriteLine("wl_afp_puur       : " + x_afp_val_puur);
                        tw.WriteLine("wl_ash_vpen       : " + x_ash_val_vpen);
                        tw.WriteLine("wl_ash_vtra       : " + x_ash_tas_vtra);
                        tw.WriteLine("wl_ash_vtce       : " + x_ash_tas_vtce);
                        tw.WriteLine("wl_ash_dura       : " + x_ash_val_dura);
                        tw.WriteLine("cot_xml_benefi    : " + x_cot_xml_benefi.ToString().Trim());
                        tw.WriteLine("wl_num_error      : " + x_cot_num_cmsg);
                        tw.Close();
                    }

                    //<SOLINI25781>
                    double x_1era_prima_is = 0;
                    //<SOLFIN25781>

                    // Para la segunda pasada no se debe volver a calcular el total garantizado
                    if (p.ind_orden == 1)
                    {
                        // Secuencia post-cotización (Traducción extraída del s_renvi)
                        val_total_garantizado = 0;
                        if (p.cot_num_mgar > 0)
                        {
                            if (p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue())
                            {
                                val_total_garantizado = x_cia_val_ppag * p.cot_num_mgar;
                                if (p.cod_moneda == Enums.Moneda.Dolares.StringValue())
                                {
                                    val_total_garantizado /= p.val_moneda;
                                }
                            }
                            else
                            {
                                if (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue())
                                {
                                    for (int i = p.cot_num_mdif; i < (p.cot_num_mgar + p.cot_num_mdif); i++)
                                    {
                                        val_total_garantizado += (x_cia_val_ppag * p.fluaju[i]);
                                    }
                                    //<SRIINI18360>
                                    // El total garantizado de la modalidad Bimoneda debe mostrarse en Soles
                                    // por lo tanto no debe convertirse.
                                    if (p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue() && p.ind_modalidad != Enums.Modalidad.Bimoneda.StringValue())
                                    //<SRIFIN18360>
                                    {
                                        val_total_garantizado /= p.val_moneda;
                                    }
                                }
                            }
                        }
                        else
                        {
                            val_total_garantizado = 0;
                        }
                        //<SOLINI25781>
                        if (p.ind_modalidad == Enums.Modalidad.Diferida.StringValue())
                        {
                            //<GTIINI-576>
                            x_1era_prima_is = x_cia_val_ppag * p.fluaju[Convert.ToInt32(p.cot_num_mdif)] / p.val_moneda;
                            //<GTIFIN-576>
                        }
                        //<SOLFIN25781>
                        //<SOLINIGTI_754>
                        if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                        {
                            x_1era_prima_is = x_cia_val_ppag * p.fluaju[Convert.ToInt32(x_cot_ini_tra2)] / p.val_moneda;
                            x_1era_prima_is *= (Convert.ToDouble(x_cot_pje_rent) / 100);
                        }
                        //<SOLFINGTI_754>
                    }

                    if (x_cot_num_cmsg > 0)
                    {
                        x_cot_tas_vtva = 0; x_cot_val_mdco = 0; x_cia_val_pens = 0;
                        x_cia_val_ppag = 0; x_cia_val_puni = 0; x_cia_val_puur = 0;
                        x_afp_val_pens = 0; x_afp_val_puni = 0; x_afp_val_puur = 0;
                        x_ash_val_vpen = 0; x_ash_tas_vtce = 0; x_ash_val_dura = 0;
                    }

                    //<GTIINI-1092>
                    // Se vuelve a descomentar el cambio de tipo de Cálculo a 2 para mixtas y combinadas para
                    // que se guarden así en la base de datos
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue()) && ((p.cod_moneda == Enums.Moneda.Soles.StringValue()) || (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue())))
                    {
                        p.cot_num_tcal = 2;
                    }
                    //<SRIINI18360>
                    if ((p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && (p.ind_orden == 1))
                    {
                        p.cot_num_tcal = 2;
                    }
                    //<SRIFIN18360>
                    //<GTIfIN-1092>

                    cia_puni_sin_com = x_cia_val_puni;
                    val_tasa_cesion = 0;
                    if (p.val_moneda != 0)
                    {
                        cia_pens_mo = x_cia_val_pens / p.val_moneda;
                        cia_ppag_mo = x_cia_val_ppag / p.val_moneda;
                    }
                    else
                    {
                        cia_pens_mo = 0;
                        cia_ppag_mo = 0;
                    }
                    cia_pens_mo = lib_s_round(cia_pens_mo, 2, 0);
                    cia_ppag_mo = lib_s_round(cia_ppag_mo, 2, 0);

                    // Actualiza rvi_cotiza
                    // --------------------
                    RecRviCotiza_XML.cod_estado_cotizacion = Constante.COD_COTIZACION_CALCULADA;
                    RecRviCotiza_XML.val_descuento_comision = x_cot_val_mdco.ToString();
                    RecRviCotiza_XML.cod_tipo_calculo = p.cot_num_tcal.ToString();
                    RecRviCotiza_XML.num_solicitud = p.cot_num_soli;
                    //<SOLINI25781>
                    RecRviCotiza_XML.val_1era_prima_is = x_1era_prima_is;
                    //<SOLFIN25781>
                    //<GTIINI-6489>
                    RecRviCotiza_XML.val_mto_gasto_comision = x_ash_val_comm;
                    //<GTIFIN-6489>

                    //<SRIINI18360>
                    // Si NO es Bimoneda NI Renta Mixta NI Renta Combinada
                    if (p.ind_modalidad != Enums.Modalidad.Mixta.StringValue() && p.ind_modalidad != Enums.Modalidad.Combinada.StringValue() && p.ind_modalidad != Enums.Modalidad.Bimoneda.StringValue())
                    //<SRIFIN18360>
                    {
                        RecRviCotiza_XML.val_fac_cia = x_cia_val_puur.ToString();
                        RecRviCotiza_XML.val_mto_cia = x_cia_val_puni.ToString();
                        RecRviCotiza_XML.val_mto_cia_sin_comision = cia_puni_sin_com.ToString();
                        RecRviCotiza_XML.val_pen_cia = x_cia_val_ppag.ToString();
                        RecRviCotiza_XML.val_pen_cia_mo = cia_ppag_mo.ToString();
                        RecRviCotiza_XML.val_pen_ref = x_cia_val_pens.ToString();
                        RecRviCotiza_XML.val_pen_ref_mo = cia_pens_mo.ToString();
                        RecRviCotiza_XML.val_tasa_int_vit = x_cot_tas_vtva.ToString("##.0000000000");
                        RecRviCotiza_XML.val_tasa_venta_ash = x_ash_tas_vtva.ToString();
                        RecRviCotiza_XML.val_tasa_ret_accion = x_ash_tas_vtra.ToString();
                        RecRviCotiza_XML.val_tasa_costo_equiv = x_ash_tas_vtce.ToString();
                        RecRviCotiza_XML.val_duration = x_ash_val_dura.ToString();
                        RecRviCotiza_XML.num_error_cot = x_cot_num_cmsg.ToString();
                        RecRviCotiza_XML.val_tasa_int_temp = p.cot_tas_tafp.ToString();
                        RecRviCotiza_XML.val_fac_afp = x_afp_val_puur.ToString();
                        RecRviCotiza_XML.val_mto_afp = x_afp_val_puni.ToString();
                        RecRviCotiza_XML.val_pen_afp = x_afp_val_pens.ToString();
                        RecRviCotiza_XML.val_afp_pen_ref = "0";
                        RecRviCotiza_XML.val_tasa_venta_ash_2 = "0";
                        RecRviCotiza_XML.val_tasa_ret_accion_2 = "0";
                        RecRviCotiza_XML.val_tasa_costo_equiv_2 = "0";
                        RecRviCotiza_XML.val_duration_2 = "0";
                        RecRviCotiza_XML.num_error_cot_2 = "0";
                        RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                        RecRviCotiza_XML.val_tasa_cesion_moneda2 = "";
                        // verificación de cotización sin error (no alcanza el TRA mínimo)
                        if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                        {
                            RecRviCotiza_XML.ind_cotiza = "**";
                        }
                        else
                        {
                            RecRviCotiza_XML.ind_cotiza = " ";
                        }
                        //<INIGTI_1092>
                        if (x_ash_tas_vtra < p.wl_val_ltra)
                        {
                            RecRviCotiza_XML.ind_cotiza = "**";
                            RecRviCotiza_XML.num_error_cot = "300";
                        }
                        //<FINGTI_1092>
                        if (RecRviCotiza_XML.ind_cotiza == " ")
                        {
                            if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                            {
                                RecRviCotiza_XML.ind_cotiza = " ";
                            }
                            else
                            {
                                if (p.ind_modalidad == Enums.Modalidad.Diferida.StringValue())
                                {
                                    RecRviCotiza_XML.ind_cotiza = "?";
                                }
                            }
                        }
                        RecRviCotiza_XML.val_total_garantizado = val_total_garantizado;
                    }
                    // Si es Bimoneda o Renta Mixta o Renta Combinada
                    else
                    {
                        // Si es Dólares
                        //<SRIINI18360>
                        if (p.ind_orden == 1)
                        //<SRIFIN18360>
                        {
                            RecRviCotiza_XML.val_fac_cia = x_cia_val_puur.ToString();
                            RecRviCotiza_XML.val_mto_cia = x_cia_val_puni.ToString();
                            RecRviCotiza_XML.val_mto_cia_sin_comision = cia_puni_sin_com.ToString();
                            RecRviCotiza_XML.val_pen_cia = x_cia_val_ppag.ToString();
                            RecRviCotiza_XML.val_pen_cia_mo = cia_ppag_mo.ToString();
                            RecRviCotiza_XML.val_pen_ref = x_cia_val_pens.ToString();
                            RecRviCotiza_XML.val_pen_ref_mo = cia_pens_mo.ToString();
                            RecRviCotiza_XML.val_tasa_int_vit = x_cot_tas_vtva.ToString("##.0000000000");
                            RecRviCotiza_XML.val_tasa_venta_ash = x_ash_tas_vtva.ToString();
                            RecRviCotiza_XML.val_tasa_ret_accion = x_ash_tas_vtra.ToString();
                            RecRviCotiza_XML.val_tasa_costo_equiv = x_ash_tas_vtce.ToString();
                            RecRviCotiza_XML.val_duration = x_ash_val_dura.ToString();
                            RecRviCotiza_XML.num_error_cot = x_cot_num_cmsg.ToString();
                            if (p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                                // verificación de cotización sin error (no alcanza el TRA mínimo)
                                if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                }
                                else
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                //<INIGTI_1092>
                                if (x_ash_tas_vtra < p.wl_val_ltra)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                    RecRviCotiza_XML.num_error_cot = "300";
                                }
                                //<FINGTI_1092>
                            }
                            else
                            {
                                RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                                pension_mixta_dolares_referencia = x_cia_val_pens;
                                if ((p.cod_tipo_pension == Enums.TipoPension.InvalidezConCobertura.StringValue()) && (p.cod_tipo_invalidez == Enums.TipoInvalidez.Total.StringValue() || p.cod_tipo_invalidez == Enums.TipoInvalidez.Parcial.StringValue()))
                                {
                                    if (p.cod_tipo_invalidez == Enums.TipoInvalidez.Parcial.StringValue())
                                    {
                                        pension_mixta_dolares_referencia /= 0.5;
                                    }
                                    else if (p.cod_tipo_invalidez == Enums.TipoInvalidez.Total.StringValue())
                                    {
                                        pension_mixta_dolares_referencia /= 0.7;
                                    }
                                }
                                // verificación de pensión mínima para Renta Mixta en Dólares
                                if (pension_mixta_dolares_referencia < p.wl_val_pension_minimo)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "*";
                                }
                                else
                                {
                                    RecRviCotiza_XML.ind_cotiza = String.Empty;
                                }
                                // verificación de cotización sin error (no alcanza el TRA mínimo)
                                if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                }
                                //<INIGTI_1092>
                                if (x_ash_tas_vtra < p.wl_val_ltra)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                    RecRviCotiza_XML.num_error_cot = "300";
                                }
                                //<FINGTI_1092>
                            }
                            if (RecRviCotiza_XML.ind_cotiza == " " || RecRviCotiza_XML.ind_cotiza == String.Empty)
                            {
                                if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                else
                                {
                                    if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue())
                                    {
                                        RecRviCotiza_XML.ind_cotiza = "?";
                                    }
                                }
                            }
                            RecRviCotiza_XML.val_total_garantizado = val_total_garantizado;
                        }
                        // Si es la segunda pasada
                        else
                        {
                            RecRviCotiza_XML.val_tasa_int_temp = x_cot_tas_vtva.ToString("##.0000000000");
                            RecRviCotiza_XML.val_fac_afp = x_cia_val_puur.ToString();
                            RecRviCotiza_XML.val_mto_afp = x_cia_val_puni.ToString();
                            RecRviCotiza_XML.val_pen_afp = x_cia_val_ppag.ToString();
                            RecRviCotiza_XML.val_afp_pen_ref = x_cia_val_pens.ToString();
                            if (p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.val_tasa_venta_ash_2 = x_ash_tas_vtva.ToString();
                                RecRviCotiza_XML.val_tasa_ret_accion_2 = x_ash_tas_vtra.ToString();
                                RecRviCotiza_XML.val_tasa_costo_equiv_2 = x_ash_tas_vtce.ToString();
                                RecRviCotiza_XML.val_duration_2 = x_ash_val_dura.ToString();
                                RecRviCotiza_XML.num_error_cot_2 = x_cot_num_cmsg.ToString();
                                RecRviCotiza_XML.val_tasa_cesion_moneda2 = val_tasa_cesion.ToString();
                            }
                            else
                            {
                                RecRviCotiza_XML.val_tasa_venta_ash_2 = "0";
                                RecRviCotiza_XML.val_tasa_ret_accion_2 = "0";
                                RecRviCotiza_XML.val_tasa_costo_equiv_2 = "0";
                                RecRviCotiza_XML.val_duration_2 = "0";
                                RecRviCotiza_XML.num_error_cot_2 = "0";
                                RecRviCotiza_XML.val_tasa_cesion = "0";
                                RecRviCotiza_XML.val_tasa_cesion_moneda2 = "0";
                            }
                            // verificación de cotización sin error (no alcanza el TRA mínimo)
                            if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot_2) > 0 && p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.ind_cotiza = "**";
                            }
                            //<INIGTI_1092>
                            if (x_ash_tas_vtra < p.wl_val_ltra)
                            {
                                RecRviCotiza_XML.ind_cotiza = "**";
                                RecRviCotiza_XML.num_error_cot = "300";
                            }
                            //<FINGTI_1092>
                            if (RecRviCotiza_XML.ind_cotiza == " ")
                            {
                                if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                else
                                {
                                    //<SRIINI18360>
                                    if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue())
                                    //<SRIFIN18360>
                                    {
                                        RecRviCotiza_XML.ind_cotiza = "?";
                                    }
                                }
                            }
                            RecRviCotiza_XML.val_total_garantizado = val_total_garantizado;
                        }
                    }

                    //<SRIINI18360>
                    if (!(p.ind_orden == 2))
                    //<SRIFIN18360>
                    {
                        wl_XML_CostoBen = x_cot_xml_benefi.ToString().Trim();
                    }

                    //<SRIINI18360>
                    if (!((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue() || p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue()) && p.ind_orden == 1))
                    //<SRIFIN18360>
                    {
                        if (RecRviCotiza_XML.ind_cotiza == String.Empty)
                        {
                            RecRviCotiza_XML.ind_cotiza = " ";
                        }
                        wl_XML_Cotiza += "<cotiza>";
                        wl_XML_Cotiza += " <num_solicitud>" + RecRviCotiza_XML.num_solicitud + "</num_solicitud>";
                        wl_XML_Cotiza += " <fec_cotizacion>" + p.fec_cotizacion.ToString("dd/MM/yyyy") + "</fec_cotizacion>";
                        wl_XML_Cotiza += " <num_correlativo>" + p.cot_num_coti.ToString() + "</num_correlativo>";
                        wl_XML_Cotiza += " <cod_estado_cotizacion>" + RecRviCotiza_XML.cod_estado_cotizacion + "</cod_estado_cotizacion>";
                        wl_XML_Cotiza += " <val_descuento_comision>" + RecRviCotiza_XML.val_descuento_comision + "</val_descuento_comision>";
                        wl_XML_Cotiza += " <cod_tipo_calculo>" + RecRviCotiza_XML.cod_tipo_calculo + "</cod_tipo_calculo>";
                        wl_XML_Cotiza += " <ind_cotiza>" + RecRviCotiza_XML.ind_cotiza + "</ind_cotiza>";
                        wl_XML_Cotiza += " <val_fac_cia>" + RecRviCotiza_XML.val_fac_cia + "</val_fac_cia>";
                        wl_XML_Cotiza += " <val_mto_cia>" + RecRviCotiza_XML.val_mto_cia + "</val_mto_cia>";
                        wl_XML_Cotiza += " <val_mto_cia_sin_comision>" + RecRviCotiza_XML.val_mto_cia_sin_comision + "</val_mto_cia_sin_comision>";
                        wl_XML_Cotiza += " <val_pen_cia>" + RecRviCotiza_XML.val_pen_cia + "</val_pen_cia>";
                        wl_XML_Cotiza += " <val_pen_cia_mo>" + RecRviCotiza_XML.val_pen_cia_mo + "</val_pen_cia_mo>";
                        wl_XML_Cotiza += " <val_pen_ref>" + RecRviCotiza_XML.val_pen_ref + "</val_pen_ref>";
                        wl_XML_Cotiza += " <val_pen_ref_mo>" + RecRviCotiza_XML.val_pen_ref_mo + "</val_pen_ref_mo>";
                        wl_XML_Cotiza += " <val_tasa_int_vit>" + RecRviCotiza_XML.val_tasa_int_vit + "</val_tasa_int_vit>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash>" + RecRviCotiza_XML.val_tasa_venta_ash + "</val_tasa_venta_ash>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion>" + RecRviCotiza_XML.val_tasa_ret_accion + "</val_tasa_ret_accion>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv>" + RecRviCotiza_XML.val_tasa_costo_equiv + "</val_tasa_costo_equiv>";
                        wl_XML_Cotiza += " <val_duration>" + RecRviCotiza_XML.val_duration + "</val_duration>";
                        wl_XML_Cotiza += " <num_error_cot>" + RecRviCotiza_XML.num_error_cot + "</num_error_cot>";
                        wl_XML_Cotiza += " <val_tasa_int_temp>" + RecRviCotiza_XML.val_tasa_int_temp + "</val_tasa_int_temp>";
                        wl_XML_Cotiza += " <val_fac_afp>" + RecRviCotiza_XML.val_fac_afp + "</val_fac_afp>";
                        wl_XML_Cotiza += " <val_mto_afp>" + RecRviCotiza_XML.val_mto_afp + "</val_mto_afp>";
                        wl_XML_Cotiza += " <val_pen_afp>" + RecRviCotiza_XML.val_pen_afp + "</val_pen_afp>";
                        wl_XML_Cotiza += " <val_afp_pen_ref>" + RecRviCotiza_XML.val_afp_pen_ref + "</val_afp_pen_ref>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash_2>" + RecRviCotiza_XML.val_tasa_venta_ash_2 + "</val_tasa_venta_ash_2>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion_2>" + RecRviCotiza_XML.val_tasa_ret_accion_2 + "</val_tasa_ret_accion_2>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv_2>" + RecRviCotiza_XML.val_tasa_costo_equiv_2 + "</val_tasa_costo_equiv_2>";
                        wl_XML_Cotiza += " <val_duration_2>" + RecRviCotiza_XML.val_duration_2 + "</val_duration_2>";
                        wl_XML_Cotiza += " <num_error_cot_2>" + RecRviCotiza_XML.num_error_cot_2 + "</num_error_cot_2>";
                        wl_XML_Cotiza += " <val_tasa_cesion>" + RecRviCotiza_XML.val_tasa_cesion + "</val_tasa_cesion>";
                        wl_XML_Cotiza += " <val_tasa_cesion_moneda2>" + RecRviCotiza_XML.val_tasa_cesion_moneda2 + "</val_tasa_cesion_moneda2>";
                        if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue() || p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                        {
                            if ((p.cod_moneda == Enums.Moneda.Dolares.StringValue()) || (p.cod_moneda == Enums.Moneda.Soles.StringValue()))
                            {
                                wl_XML_Cotiza += " <cod_moneda>" + Enums.Moneda.Dolares.StringValue() + "</cod_moneda>";
                            }
                            else if ((p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()) || (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue()))
                            {
                                wl_XML_Cotiza += " <cod_moneda>" + Enums.Moneda.DolaresAjustados.StringValue() + "</cod_moneda>";
                            }
                        }
                        else
                        {
                            wl_XML_Cotiza += " <cod_moneda>" + p.cod_moneda + "</cod_moneda>";
                        }
                        wl_XML_Cotiza += " <wl_cod_username>" + usuario + "</wl_cod_username>";
                        wl_XML_Cotiza += " <val_total_garantizado>" + RecRviCotiza_XML.val_total_garantizado + "</val_total_garantizado>";
                        //<SOLINI25781>
                        wl_XML_Cotiza += " <val_1era_prima_is>" + RecRviCotiza_XML.val_1era_prima_is + "</val_1era_prima_is>";
                        //<SOLFIN25781>
                        //<GTIINI-6489>
                        wl_XML_Cotiza += " <val_mto_gasto_comision>" + RecRviCotiza_XML.val_mto_gasto_comision + "</val_mto_gasto_comision>";
                        //<GTIFIN-6489>
                        wl_XML_Cotiza += "</cotiza>";
                        XML_Pje = p.wl_XML_Pje;
                        XML_CostoBen += wl_XML_CostoBen;
                        XML_Cotiza += wl_XML_Cotiza;

                        if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                        {
                            //<SOLINIGTI_754>
                            if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                            tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_2.txt");
                            //<SOLFINGTI_754>
                            tw.WriteLine("");
                            tw.WriteLine("XML_INSERCION");
                            tw.WriteLine("wl_XML_Pje        : " + XML_Pje);
                            tw.WriteLine("wl_XML_CostoBen   : " + XML_CostoBen);
                            tw.WriteLine("wl_XML_Cotiza     : " + XML_Cotiza);
                            tw.Close();
                        }
                    }
                }

                // Construir correlativos
                string correlativos = string.Empty;
                parametros.ForEach(p => correlativos += p.cot_num_coti.ToString() + ",");
                correlativos = correlativos.Substring(0, correlativos.Length - 1);

                // Llenar el DTO con los resultados (en lugar de persistir en BD)
                resultado.ExitoOperacion = true;
                resultado.idSolicitud = idSolicitud;
                resultado.correlativos = correlativos;
                resultado.XmlBeneficiarios = XML_CostoBen;
                resultado.XmlCotizacion = XML_Cotiza;
                resultado.XmlPorcentajes = XML_Pje;

                return resultado;
            }
            catch (Exception ex)
            {
                resultado.ExitoOperacion = false;
                resultado.MensajeError = ex.Message;
                return resultado;
            }
        }
        public void ObtenerDTra(ref Solicitud solicitud, string usuario)
        {
            try
            {
                string idSolicitud;
                DateTime fechaCotizacion;
                idSolicitud = solicitud.Id;
                fechaCotizacion = solicitud.FechaCotizacion.Value;
                // Obtener los parámetros de cotización
                var parametros = repositorioSolicitud.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, null, 0, null);
                double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                        x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                        x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                        x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                        x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
                StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
                int x_cot_num_cmsg = 0;
                //<INIGTI_4081>
                //double cot_tas_vtra_anterior = 0.0, val_tasa_ajuste_tra_anterior = 0.0;
                //<FINGTI_4081>
                RviCotiza RecRviCotiza_XML = new RviCotiza();
                Cotizacion cotizacion = new Cotizacion();
                foreach (ParametroCotizador p in parametros)
                {
                    cotizacion = solicitud.Cotizaciones.Find(c => c.Correlativo == p.cot_num_coti);
                    //<SRIINI18360>
                    // Para el caso de las Mixtas y Combinadas que cuentan con período garantizado se debe desactivar
                    // el periodo garantizado al momento de cotizar la pensión de la AFP ya que este no debe afectarla
                    int cot_num_mgar_aux = 0;
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        cot_num_mgar_aux = p.cot_num_mgar;
                        p.cot_num_mgar = 0;
                    }
                    //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                    int x_cot_ini_tra2 = 0;
                    double x_cot_pje_rent = 0;
                    if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                    {
                        x_cot_ini_tra2 = p.x_cot_ini_tra2; //p.cot_num_mdif;
                        x_cot_pje_rent = p.x_cot_pje_rent; // Convert.ToInt32(p.cot_por_prrt);
                        p.cot_num_mdif = 0;
                        p.cot_por_prrt = 0;
                    }
                    //<INIGTI_4081>
                    RecRviCotiza_XML.val_tasa_ajuste_tra = p.x_val_tasa_ajuste_tra;
                    if (p.ind_orden == 1)
                    {
                        //Cuando el pbs es cero, su AjusteTRA o DifTra sera Cero.
                        if (cotizacion.pbs == 0)
                        {
                            solicitud.Cotizaciones
                                   .FindAll(c => c.Correlativo == p.cot_num_coti)
                                   .ForEach(c => c.AjusteTRA = 0);
                        }
                        else
                        {
                            //En Caso que sea diferente el enviado o con el existente ejecuta o calcula de nuevo difTra
                            if (cotizacion.pbs != p.x_pbs)
                            {
                                //En caso que el anterior pbs sea diferente de cero, calculara nuevo la tasa con difTra, para obtener la nueva tasa
                                if (p.x_pbs != 0)
                                {
                                    //El p.x_val_tasa_ajuste_tra guarda valor negativo
                                    p.cot_tas_vtra = p.cot_tas_vtra - p.x_val_tasa_ajuste_tra;
                                    //<GTI.INI-15819>
                                    string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                                    ////<GTIINI-6489>
                                    ////cot_main(
                                    ////   p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                                    ////   p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                                    ////   p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                                    ////   p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                                    ////   ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                                    ////   p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                                    ////   p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                                    ////   ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                                    ////   ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                                    ////   ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                                    ////   ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                                    ////   ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                                    ////   x_cot_xml_benefi, ref x_cot_num_cmsg);
                                    //cot_main(
                                    //   p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                                    //   p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                                    //   p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                                    //   p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                                    //   ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                                    //   p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                                    //   p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                                    //   ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                                    //   ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                                    //   ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                                    //   ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                                    //   ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                                    //   x_cot_xml_benefi, ref x_cot_num_cmsg);
                                    ////<GTIFIN-6489>
                                    cot_main(
                                       cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                                       p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                                       p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                                       p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                                       ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                                       p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                                       p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                                       ref p.cot_fac_dto[0],
                                       ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                                       ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                                       ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                                       ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                                       ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                                       x_cot_xml_benefi, ref x_cot_num_cmsg);
                                    //<GTI.FIN-15819>
                                    //Actualizó de nuevo la tasa obtenida
                                    p.x_val_tasa_int_vit = x_cot_tas_vtva;
                                }
                                //val_tasa_ajuste_tra_anterior = 0.0;
                                //cot_tas_vtra_anterior = 0.0;
                                bool encuentra = false;
                                double aumento = -0.05;
                                double origen = p.cot_tas_vtra;
                                double origenMenos5 = 0;
                                while (!encuentra)
                                {
                                    if (aumento == -0.05)
                                    {
                                        origenMenos5 = p.cot_tas_vtra;
                                    }
                                    p.cot_tas_vtra += aumento;
                                    //<GTI.INI-15819>
                                    string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                                    ////<GTIINI-6489>
                                    ////cot_main(
                                    ////   p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                                    ////   p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                                    ////   p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                                    ////   p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                                    ////   ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                                    ////   p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                                    ////   p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                                    ////   ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                                    ////   ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                                    ////   ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                                    ////   ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                                    ////   ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                                    ////   x_cot_xml_benefi, ref x_cot_num_cmsg);
                                    //cot_main(
                                    //   p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                                    //   p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                                    //   p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                                    //   p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                                    //   ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                                    //   p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                                    //   p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                                    //   ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                                    //   ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                                    //   ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                                    //   ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                                    //   ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                                    //   x_cot_xml_benefi, ref x_cot_num_cmsg);
                                    ////<GTIFIN-6489>
                                    cot_main(
                                       cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                                       p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                                       p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                                       p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                                       ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                                       p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                                       p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                                       ref p.cot_fac_dto[0],
                                       ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                                       ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                                       ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                                       ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                                       ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                                       x_cot_xml_benefi, ref x_cot_num_cmsg);
                                    //<GTI.FIN-15819>
                                    if (lib_s_round(p.x_val_tasa_int_vit + (cotizacion.pbs / 100), 2, 0) == lib_s_round(x_cot_tas_vtva, 2, 0))
                                    {
                                        if (x_ash_tas_vtra < p.wl_val_ltra)
                                        {
                                            throw new Exception("El PBS ingresado supera la TIR.");
                                        }
                                        encuentra = true;
                                        //p.val_tasa_ajuste_tra
                                        RecRviCotiza_XML.val_tasa_ajuste_tra = lib_s_round(p.cot_tas_vtra - origen, 2, 0);
                                        //val_tasa_ajuste_tra_anterior = RecRviCotiza_XML.val_tasa_ajuste_tra;
                                        //cot_tas_vtra_anterior = p.cot_tas_vtra;
                                        //cot_num_coti
                                        solicitud.Cotizaciones
                                            .FindAll(c => c.Correlativo == p.cot_num_coti)
                                            .ForEach(c => c.AjusteTRA = RecRviCotiza_XML.val_tasa_ajuste_tra);
                                    }
                                    else if (lib_s_round(p.x_val_tasa_int_vit + (cotizacion.pbs / 100), 2, 0) < lib_s_round(x_cot_tas_vtva, 2, 0))
                                    {
                                        aumento = 0.01;
                                    }
                                    else if (x_cot_num_cmsg == 300)
                                    {
                                        aumento = 0.01;
                                    }
                                    else if (lib_s_round(origenMenos5, 2, 0) == lib_s_round(p.cot_tas_vtra, 2, 0))
                                    {
                                        throw new Exception("Se ha excedido en PBS [" + cotizacion.pbs + "] para ésta cotización.");
                                    }
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void CotizarDifTRA(ref Solicitud solicitud, string usuario)
        {
            //
            try
            {
                string idSolicitud;
                DateTime fechaCotizacion;
                idSolicitud = solicitud.Id;
                fechaCotizacion = solicitud.FechaCotizacion.Value;
                // Obtener los parámetros de cotización
                var parametros = repositorioSolicitud.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, null, 0, null);
                double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                        x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                        x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                        x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                        x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
                StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
                int x_cot_num_cmsg = 0;
                //<INIGTI_4081>
                //double cot_tas_vtra_anterior = 0.0, val_tasa_ajuste_tra_anterior = 0.0;
                //<FINGTI_4081>
                RviCotiza RecRviCotiza_XML = new RviCotiza();
                Cotizacion cotizacion = new Cotizacion();
                foreach (ParametroCotizador p in parametros)
                {
                    cotizacion = solicitud.Cotizaciones.Find(c => c.Correlativo == p.cot_num_coti);
                    //<SRIINI18360>
                    // Para el caso de las Mixtas y Combinadas que cuentan con período garantizado se debe desactivar
                    // el periodo garantizado al momento de cotizar la pensión de la AFP ya que este no debe afectarla
                    int cot_num_mgar_aux = 0;
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        cot_num_mgar_aux = p.cot_num_mgar;
                        p.cot_num_mgar = 0;
                    }
                    //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                    int x_cot_ini_tra2 = 0;
                    double x_cot_pje_rent = 0;
                    if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                    {
                        x_cot_ini_tra2 = p.x_cot_ini_tra2; //p.cot_num_mdif;
                        x_cot_pje_rent = p.x_cot_pje_rent; // Convert.ToInt32(p.cot_por_prrt);
                        p.cot_num_mdif = 0;
                        p.cot_por_prrt = 0;
                    }
                    //<INIGTI_4081>
                    RecRviCotiza_XML.val_tasa_ajuste_tra = p.x_val_tasa_ajuste_tra;
                    //El p.x_val_tasa_ajuste_tra guarda valor negativo
                    p.cot_tas_vtra = p.cot_tas_vtra - p.x_val_tasa_ajuste_tra;
                    p.cot_tas_vtra = p.cot_tas_vtra + Convert.ToDouble(solicitud.Cotizaciones.Find(c => c.Correlativo == p.cot_num_coti).AjusteTRA);
                    //p.cot_val_dcom = Convert.ToDouble(solicitud.PorcentajeDescuentoComision);//PorcentajeDescuentoComision
                    p.cot_val_acom = Convert.ToDouble(solicitud.PorcentajeAumentoComision);//PorcentajeAumentoComision
                    p.cot_val_dcom = Convert.ToDouble(solicitud.PorcentajeDescuentoComision);
                    //<GTI.INI-15819>
                    string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                    ////<GTIINI-6489>
                    ////cot_main(
                    ////        p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    ////        p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    ////        p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    ////        p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    ////        ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                    ////        p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    ////        p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    ////        ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    ////        ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    ////        ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    ////        ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    ////        ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                    ////        x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //cot_main(
                    //        p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    //        p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    //        p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    //        p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    //        ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                    //        p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    //        p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    //        ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    //        ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    //        ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    //        ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    //        ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                    //        x_cot_xml_benefi, ref x_cot_num_cmsg);
                    ////<GTIFIN-6489>
                    cot_main(
                            cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                            p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                            p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                            p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                            ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                            p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                            p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                            ref p.cot_fac_dto[0],
                            ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                            ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                            ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                            ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                            ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                            x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //<GTI.FIN-15819>
                    //Esto retorna cero, cuando hay un error 300 y la pension no es calculada
                    if (x_cot_num_cmsg == 300)
                    {
                        throw new Exception("Ha ingresado parámetros no permitidos en la solicitud");
                    }
                    Double tasaSBSIni = 0.0;
                    Double tasaSBSFin = 0.0;
                    tasaSBSIni = Convert.ToDouble(solicitud.Cotizaciones.Find(c => c.Correlativo == p.cot_num_coti).TasaVentaSbsOrigen); //Convert.ToDouble(lib_s_round(p.x_val_tasa_int_vit, 2, 0));
                    tasaSBSFin = Convert.ToDouble(lib_s_round(x_cot_tas_vtva, 2, 0));
                    //<INIGTI_6842>
                    if (x_cot_num_cmsg > 0)
                    {
                        solicitud.Cotizaciones
                                        .FindAll(c => c.Correlativo == p.cot_num_coti)
                                        .ForEach(c =>
                                        {
                                            c.TasaRetornoAccionistaMinimo = p.wl_val_ltra;
                                            c.TasaVentaMaxima = p.wl_val_htva;
                                            c.TasaVentaSbsObjetivo = x_cot_tas_vtva;
                                            c.TasaRetornoAccionistaObjetivo = 0;
                                            c.pbs = Convert.ToInt32(lib_s_round((tasaSBSFin - tasaSBSIni) * 100, 0, 0));
                                            c.IndCotiza = "**";
                                            c.PensionAFPObjetivo = 0;
                                            c.PensionCiaObjetivo = 0;
                                            c.PensionCiaMOObjetivo = 0;
                                        });
                        throw new Exception("Ha ingresado parámetros no permitidos en la solicitud");
                    }
                    //<FINGTI_6842>
                    //Esto retorna cero, cuando hay un error 300 y la pension no es calculada
                    ////if (x_cot_num_cmsg == 300)
                    ////{
                    ////    throw new Exception("Ha ingresado parámetros no permitidos en la solicitud");
                    ////}
                    //<INIGTI_6623>
                    if (p.ind_orden == 1)
                    {
                        solicitud.Cotizaciones
                                        .FindAll(c => c.Correlativo == p.cot_num_coti)
                                        .ForEach(c =>
                                        {
                                            c.TasaRetornoAccionistaMinimo = p.wl_val_ltra;
                                            c.TasaVentaMaxima = p.wl_val_htva;
                                            c.TasaVentaSbsObjetivo = x_cot_tas_vtva;
                                            c.TasaRetornoAccionistaObjetivo = p.cot_tas_vtra;
                                            c.pbs = Convert.ToInt32(lib_s_round((tasaSBSFin - tasaSBSIni) * 100, 0, 0));
                                            //<INIGTI_6842>
                                            //c.IndCotiza = (x_cot_num_cmsg > 0) ? "**" : " ";
                                            c.IndCotiza = (x_cot_num_cmsg > 0) ? "**" : (x_ash_tas_vtra < p.wl_val_ltra) ? "**" : " ";
                                            //<FINGTI_6842>
                                            c.PensionAFPObjetivo = x_afp_val_pens;
                                            c.PensionCiaObjetivo = x_cia_val_ppag;//(c.Modalidad.Id == "I-RB") ? x_afp_val_pens : x_cia_val_ppag;
                                            c.PensionCiaMOObjetivo = (p.val_moneda != 0) ? lib_s_round(x_cia_val_ppag / p.val_moneda, 2, 0) : 0;
                                        });
                    }
                    else
                    {
                        solicitud.Cotizaciones
                                        .FindAll(c => c.Correlativo == p.cot_num_coti)
                                        .ForEach(c =>
                                        {
                                            c.PensionAFPObjetivo = x_cia_val_ppag;
                                            //<INIGTI_6842>
                                            if (c.IndCotiza.Trim() == "")
                                                c.IndCotiza = (x_cot_num_cmsg > 0) ? "**" : (x_ash_tas_vtra < p.wl_val_ltra) ? "**" : " ";
                                            //<FINGTI_6842>
                                        });
                    }
                    //<FINGTI_6623>
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<FlujoMovimiento> ObtenerFlujos(DateTime fecCotizacion, string evento, string rol)
        {
            var flujos = repositorioFlujoMovimiento.ObtenerFlujos(fecCotizacion, evento, rol);
            return flujos;
        }
        public List<SolicitudEscenario> ListarSolicitudesEmail(string idSolicitudes)
        {
            List<SolicitudEscenario> solicitudEscenario = repositorioSolicitudEscenario.ListarSolicitudesEmail(idSolicitudes);
            return solicitudEscenario;
        }
        public CuotasTra ObtenerCuotasTra(CuotasTra rolCuotas)
        {
            var rolCuotasTra = repositorioRolCuotasTra.ObtenerCuotasTra(rolCuotas);
            return rolCuotasTra;
        }
        public List<SolicitudEscenario> ListarSolicitudesValidaFlujo(string NumSolicitud, string NumOperacion)
        {
            var listaSolicitudes = repositorioSolicitudEscenario.ListarSolicitudesValidaFlujo(NumSolicitud, NumOperacion);
            return listaSolicitudes;
        }
        public bool ValidaFlujoSolicitudRol(string num_solicitud, string rol, string evento)
        {
            bool valida = repositorioFlujoMovimiento.ValidaFlujoSolicitudRol(num_solicitud, evento, rol);
            return valida;
        }
        public List<CuotasTra> ListarCuotasTra(int periodo, int mes)
        {
            var listaCuotas = repositorioRolCuotasTra.ListarCuotasTra(periodo, mes);
            return listaCuotas;
        }
        public void RegistrarCuotas(List<CuotasTra> lstCuotas, string usuario)
        {

            repositorioRolCuotasTra.RegistrarCuotas(lstCuotas, usuario);

        }
        public List<GestionVentas> ConsultarGestionVentas(DateTime fechaInicial, DateTime fechaFinal, int numJefe, int numSuperv, int numAgente, string indCierre, string tipoCotizacion, string codCiaSeguro)
        {
            var lstGestionVentas = repositorioGestionVentas.ConsultarGestionVentas(fechaInicial, fechaFinal, numJefe, numSuperv, numAgente, indCierre, tipoCotizacion, codCiaSeguro);
            return lstGestionVentas;
        }
        //<FINGTI_4081>
        //<SRIINI26593>
        public void CotizarRP(string idSolicitud, DateTime fechaCotizacion, string usuario)
        {
            TextWriter tw = null;
            try
            {
                // Obtener los parámetros de cotización
                var parametros = repositorioSolicitudRP.ObtenerParametrosCotizacionRP(idSolicitud, fechaCotizacion, null);
                double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                        x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                        x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                        x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                        x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
                StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
                int x_cot_num_cmsg = 0;
                double val_total_garantizado = 0, cia_pens_mo = 0, cia_ppag_mo = 0, cia_puni_sin_com = 0,
                val_tasa_cesion, pension_mixta_dolares_referencia = 0;
                string wl_XML_CostoBen = String.Empty, wl_XML_Cotiza = String.Empty,
                        XML_CostoBen = String.Empty, XML_Cotiza = String.Empty, XML_Pje = String.Empty;
                RviCotiza RecRviCotiza_XML = new RviCotiza();
                foreach (ParametroCotizador p in parametros)
                {
                    // Convertir el valor de la Prima Única a soles
                    p.cot_val_puni *= p.val_moneda;
                    //p.cot_val_puam *= p.val_moneda;
                    // Para el caso de las Mixtas y Combinadas que cuentan con período garantizado se debe desactivar
                    // el periodo garantizado al momento de cotizar la pensión de la AFP ya que este no debe afectarla
                    int cot_num_mgar_aux = 0;
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        cot_num_mgar_aux = p.cot_num_mgar;
                        p.cot_num_mgar = 0;
                    }
                    //<SOLINIGTI_754>
                    //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                    int x_cot_ini_tra2 = 0;
                    double x_cot_pje_rent = 0;
                    if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                    {
                        x_cot_ini_tra2 = p.x_cot_ini_tra2; //p.cot_num_mdif;
                        x_cot_pje_rent = p.x_cot_pje_rent;
                        p.cot_num_mdif = 0;
                        p.cot_por_prrt = 0;
                    }
                    //<SOLFINGTI_754>
                    //Imprimir Log DEGUG
                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("wl_cot_kcot        : " + p.cot_gls_ikey);
                        tw.WriteLine("wl_num_solicitud   : " + p.cot_num_soli);
                        tw.WriteLine("wl_num_corr_cotiza : " + p.cot_num_coti);
                        tw.WriteLine("wl_cot_pdif        : " + p.cot_num_mdif);
                        tw.WriteLine("wl_cot_pgar        : " + p.cot_num_mgar);
                        tw.WriteLine("wl_cot_nben        : " + p.cot_num_nben);
                        tw.WriteLine("wl_cot_fcot        : " + p.cot_fec_fcal);
                        tw.WriteLine("wl_cot_fdev        : " + p.cot_fec_fdev);
                        tw.WriteLine("wl_cot_tpen        : " + p.cot_num_tpen);
                        tw.WriteLine("wl_cot_tcal        : " + p.cot_num_tcal);
                        tw.WriteLine("wl_cot_derc        : " + p.cot_flg_idac);
                        tw.WriteLine("wl_cot_grat        : " + p.cot_flg_igra);
                        tw.WriteLine("cot_mon_equi       : " + p.cot_num_cmon);
                        tw.WriteLine("cot_num_trea       : " + p.cot_num_trea);
                        tw.WriteLine("cot_num_frea       : " + p.cot_num_frea);
                        tw.WriteLine("cot_flg_irea       : " + p.cot_flg_irea);
                        tw.WriteLine("wg_val_ajuste_tasa_fija : " + p.cot_tas_vrea);
                        tw.WriteLine("wl_val_tasa_venta  : " + p.cot_tas_tasa);
                        tw.WriteLine("wl_cot_vpen        : " + p.cot_val_vpen);
                        tw.WriteLine("wl_val_vtra        : " + p.cot_tas_vtra);
                        tw.WriteLine("wl_val_tasa_afp    : " + p.cot_tas_tafp);
                        tw.WriteLine("wl_val_acom        : " + p.cot_val_acom);
                        tw.WriteLine("wl_cot_pdco        : " + p.cot_val_dcom);
                        tw.WriteLine("wl_val_puam        : " + p.cot_val_puam);
                        tw.WriteLine("wl_val_puni        : " + p.cot_val_puni);
                        tw.WriteLine("wl_cot_prrt        : " + p.cot_por_prrt);
                        tw.WriteLine("wl_val_tgfi        : " + p.cot_val_tgfi);
                        tw.WriteLine("wl_val_tope        : " + p.cot_val_tope);
                        tw.WriteLine("arr_ppu_vllx       : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx[i, j]);
                        //tw.WriteLine("arr_ppu_aju_vllx   : ");
                        //for (int i = 0; i <= 10568; i++)
                        //    for (int j = 0; j <= 3; j++)
                        //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_arr_vllx[i, j]);
                        tw.WriteLine("arr_ppu_vllx_cot   : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx_cot[i, j]);
                        tw.WriteLine("cot_xml_ben        : " + p.cot_xml_benefi);
                        tw.WriteLine("cot_xml_icob       : " + p.cot_xml_tabico);
                        tw.WriteLine("cot_xml_ash        : " + p.cot_xml_parash);
                        tw.WriteLine("cot_xml_inv        : " + p.cot_xml_parinv);
                        tw.WriteLine("cot_xml_ajm        : " + p.cot_xml_ajutdm);
                        tw.WriteLine("cot_xml_fdv        : " + p.cot_xml_fluaju);
                        tw.WriteLine("arr_ppu_fm_cot     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm[i, j]);
                        tw.WriteLine("arr_ppu_fm_sbs     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm_sbs[i, j]);
                        tw.Close();
                    }
                    // LLamado a la DLL del cotizador (rvicotmain.dll)
                    //<GTI.INI-15819>
                    string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                    ////<GTIINI-6489>
                    ////cot_main(
                    ////    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    ////    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    ////    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    ////    p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    ////    ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0], //ANTES  ref p.ppu_arr_vllx[0, 0]
                    ////    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    ////    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    ////    ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    ////    ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    ////    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    ////    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    ////    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                    ////    x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //cot_main(
                    //    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    //    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    //    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    //    p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    //    ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0], //ANTES  ref p.ppu_arr_vllx[0, 0]
                    //    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    //    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    //    ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    //    ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    //    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    //    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    //    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                    //    x_cot_xml_benefi, ref x_cot_num_cmsg);
                    ////<GTIFIN-6489>
                    cot_main(
                        cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                        p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                        p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                        p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                        ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0], //ANTES  ref p.ppu_arr_vllx[0, 0]
                        p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                        p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                        ref p.cot_fac_dto[0],
                        ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                        ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                        ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                        ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                        ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                        x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //<GTI.FIN-15819>

                    x_cia_val_ppag = Math.Round(x_cia_val_ppag, 2, MidpointRounding.AwayFromZero);

                    // Luego de calcular la pensión regresar al valor original
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        p.cot_num_mgar = cot_num_mgar_aux;
                    }
                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_1.txt");
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_1.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("Salida:");
                        tw.WriteLine("");
                        tw.WriteLine("wl_cia_vtva       : " + x_cot_tas_vtva);
                        tw.WriteLine("wl_cot_dcmm       : " + x_cot_val_mdco);
                        tw.WriteLine("wl_cia_pens       : " + x_cia_val_pens);
                        tw.WriteLine("wl_cia_ppag       : " + x_cia_val_ppag);
                        tw.WriteLine("wl_cia_puni       : " + x_cia_val_puni);
                        tw.WriteLine("wl_cia_puur       : " + x_cia_val_puur);
                        tw.WriteLine("wl_afp_pens       : " + x_afp_val_pens);
                        tw.WriteLine("wl_afp_puni       : " + x_afp_val_puni);
                        tw.WriteLine("wl_afp_puur       : " + x_afp_val_puur);
                        tw.WriteLine("wl_ash_vpen       : " + x_ash_val_vpen);
                        tw.WriteLine("wl_ash_vtra       : " + x_ash_tas_vtra);
                        tw.WriteLine("wl_ash_vtce       : " + x_ash_tas_vtce);
                        tw.WriteLine("wl_ash_dura       : " + x_ash_val_dura);
                        tw.WriteLine("cot_xml_benefi    : " + x_cot_xml_benefi.ToString().Trim());
                        tw.WriteLine("wl_num_error      : " + x_cot_num_cmsg);
                        tw.Close();
                    }
                    // Para la segunda pasada no se debe volver a calcular el total garantizado
                    if (p.ind_orden == 1)
                    {
                        // Secuencia post-cotización (Traducción extraída del s_renvi)
                        val_total_garantizado = 0;
                        if (p.cot_num_mgar > 0)
                        {
                            if (p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue())
                            {
                                val_total_garantizado = x_cia_val_ppag * p.cot_num_mgar;
                                if (p.cod_moneda == Enums.Moneda.Dolares.StringValue())
                                {
                                    val_total_garantizado /= p.val_moneda;
                                }
                            }
                            else
                            {
                                if (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue())
                                {
                                    for (int i = p.cot_num_mdif; i < (p.cot_num_mgar + p.cot_num_mdif); i++)
                                    {
                                        val_total_garantizado += (x_cia_val_ppag * p.fluaju[i]);
                                    }
                                    // El total garantizado de la modalidad Bimoneda debe mostrarse en Soles
                                    // por lo tanto no debe convertirse.
                                    if (p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue() && p.ind_modalidad != Enums.Modalidad.Bimoneda.StringValue())
                                    {
                                        val_total_garantizado /= p.val_moneda;
                                    }
                                }
                            }
                        }
                        else
                        {
                            val_total_garantizado = 0;
                        }
                    }
                    if (x_cot_num_cmsg > 0)
                    {
                        x_cot_tas_vtva = 0; x_cot_val_mdco = 0; x_cia_val_pens = 0;
                        x_cia_val_ppag = 0; x_cia_val_puni = 0; x_cia_val_puur = 0;
                        x_afp_val_pens = 0; x_afp_val_puni = 0; x_afp_val_puur = 0;
                        x_ash_val_vpen = 0; x_ash_tas_vtce = 0; x_ash_val_dura = 0;
                    }
                    cia_puni_sin_com = x_cia_val_puni;
                    val_tasa_cesion = 0;
                    if (p.val_moneda != 0)
                    {
                        cia_pens_mo = x_cia_val_pens / p.val_moneda;
                        cia_ppag_mo = x_cia_val_ppag / p.val_moneda;
                    }
                    else
                    {
                        cia_pens_mo = 0;
                        cia_ppag_mo = 0;
                    }
                    cia_pens_mo = lib_s_round(cia_pens_mo, 2, 0);
                    cia_ppag_mo = lib_s_round(cia_ppag_mo, 2, 0);
                    // Actualiza rvi_cotiza
                    // --------------------
                    RecRviCotiza_XML.cod_estado_cotizacion = Constante.COD_COTIZACION_CALCULADA;
                    RecRviCotiza_XML.val_descuento_comision = x_cot_val_mdco.ToString();
                    RecRviCotiza_XML.cod_tipo_calculo = p.cot_num_tcal.ToString();
                    RecRviCotiza_XML.num_solicitud = p.cot_num_soli;
                    // Si NO es Bimoneda NI Renta Mixta NI Renta Combinada
                    if (p.ind_modalidad != Enums.Modalidad.Mixta.StringValue() && p.ind_modalidad != Enums.Modalidad.Combinada.StringValue() && p.ind_modalidad != Enums.Modalidad.Bimoneda.StringValue())
                    {
                        RecRviCotiza_XML.val_fac_cia = x_cia_val_puur.ToString();
                        RecRviCotiza_XML.val_mto_cia = x_cia_val_puni.ToString();
                        RecRviCotiza_XML.val_mto_cia_sin_comision = cia_puni_sin_com.ToString();
                        RecRviCotiza_XML.val_pen_cia = x_cia_val_ppag.ToString();
                        RecRviCotiza_XML.val_pen_cia_mo = cia_ppag_mo.ToString();
                        RecRviCotiza_XML.val_pen_ref = x_cia_val_pens.ToString();
                        RecRviCotiza_XML.val_pen_ref_mo = cia_pens_mo.ToString();
                        RecRviCotiza_XML.val_tasa_int_vit = x_cot_tas_vtva.ToString("##.0000000000");
                        RecRviCotiza_XML.val_tasa_venta_ash = x_ash_tas_vtva.ToString();
                        RecRviCotiza_XML.val_tasa_ret_accion = x_ash_tas_vtra.ToString();
                        RecRviCotiza_XML.val_tasa_costo_equiv = x_ash_tas_vtce.ToString();
                        RecRviCotiza_XML.val_duration = x_ash_val_dura.ToString();
                        RecRviCotiza_XML.num_error_cot = x_cot_num_cmsg.ToString();
                        RecRviCotiza_XML.val_tasa_int_temp = p.cot_tas_tafp.ToString();
                        RecRviCotiza_XML.val_fac_afp = x_afp_val_puur.ToString();
                        RecRviCotiza_XML.val_mto_afp = x_afp_val_puni.ToString();
                        RecRviCotiza_XML.val_pen_afp = x_afp_val_pens.ToString();
                        RecRviCotiza_XML.val_afp_pen_ref = "0";
                        RecRviCotiza_XML.val_tasa_venta_ash_2 = "0";
                        RecRviCotiza_XML.val_tasa_ret_accion_2 = "0";
                        RecRviCotiza_XML.val_tasa_costo_equiv_2 = "0";
                        RecRviCotiza_XML.val_duration_2 = "0";
                        RecRviCotiza_XML.num_error_cot_2 = "0";
                        RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                        RecRviCotiza_XML.val_tasa_cesion_moneda2 = "";
                        // verificación de cotización sin error (no alcanza el TRA mínimo)
                        if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                        {
                            RecRviCotiza_XML.ind_cotiza = "**";
                        }
                        else
                        {
                            RecRviCotiza_XML.ind_cotiza = " ";
                        }
                        if (RecRviCotiza_XML.ind_cotiza == " ")
                        {
                            if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                            {
                                RecRviCotiza_XML.ind_cotiza = " ";
                            }
                            else
                            {
                                if (p.ind_modalidad == Enums.Modalidad.Diferida.StringValue())
                                {
                                    RecRviCotiza_XML.ind_cotiza = "?";
                                }
                            }
                        }
                        RecRviCotiza_XML.val_total_garantizado = Math.Round(val_total_garantizado, 2, MidpointRounding.AwayFromZero);
                    }
                    // Si es Bimoneda o Renta Mixta o Renta Combinada
                    else
                    {
                        // Si es Dólares
                        if (
                            p.ind_orden == 1
                           )
                        {
                            RecRviCotiza_XML.val_fac_cia = x_cia_val_puur.ToString();
                            RecRviCotiza_XML.val_mto_cia = x_cia_val_puni.ToString();
                            RecRviCotiza_XML.val_mto_cia_sin_comision = cia_puni_sin_com.ToString();
                            RecRviCotiza_XML.val_pen_cia = x_cia_val_ppag.ToString();
                            RecRviCotiza_XML.val_pen_cia_mo = cia_ppag_mo.ToString();
                            RecRviCotiza_XML.val_pen_ref = x_cia_val_pens.ToString();
                            RecRviCotiza_XML.val_pen_ref_mo = cia_pens_mo.ToString();
                            RecRviCotiza_XML.val_tasa_int_vit = x_cot_tas_vtva.ToString("##.0000000000");
                            RecRviCotiza_XML.val_tasa_venta_ash = x_ash_tas_vtva.ToString();
                            RecRviCotiza_XML.val_tasa_ret_accion = x_ash_tas_vtra.ToString();
                            RecRviCotiza_XML.val_tasa_costo_equiv = x_ash_tas_vtce.ToString();
                            RecRviCotiza_XML.val_duration = x_ash_val_dura.ToString();
                            RecRviCotiza_XML.num_error_cot = x_cot_num_cmsg.ToString();
                            if (p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                                // verificación de cotización sin error (no alcanza el TRA mínimo)
                                if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                }
                                else
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                            }
                            else
                            {
                                RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                                pension_mixta_dolares_referencia = x_cia_val_pens;
                                if ((p.cod_tipo_pension == Enums.TipoPension.InvalidezConCobertura.StringValue()) && (p.cod_tipo_invalidez == Enums.TipoInvalidez.Total.StringValue() || p.cod_tipo_invalidez == Enums.TipoInvalidez.Parcial.StringValue()))
                                {
                                    if (p.cod_tipo_invalidez == Enums.TipoInvalidez.Parcial.StringValue())
                                    {
                                        pension_mixta_dolares_referencia /= 0.5;
                                    }
                                    else if (p.cod_tipo_invalidez == Enums.TipoInvalidez.Total.StringValue())
                                    {
                                        pension_mixta_dolares_referencia /= 0.7;
                                    }
                                }
                                // verificación de pensión mínima para Renta Mixta en Dólares
                                if (pension_mixta_dolares_referencia < p.wl_val_pension_minimo)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "*";
                                }
                                else
                                {
                                    RecRviCotiza_XML.ind_cotiza = String.Empty;
                                }
                                // verificación de cotización sin error (no alcanza el TRA mínimo)
                                if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                }
                            }
                            if (RecRviCotiza_XML.ind_cotiza == " " || RecRviCotiza_XML.ind_cotiza == String.Empty)
                            {
                                if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                else
                                {
                                    if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue())
                                    {
                                        RecRviCotiza_XML.ind_cotiza = "?";
                                    }
                                }
                            }
                            RecRviCotiza_XML.val_total_garantizado = Math.Round(val_total_garantizado, 2, MidpointRounding.AwayFromZero);
                        }
                        // Si es la segunda pasada
                        else
                        {
                            RecRviCotiza_XML.val_tasa_int_temp = x_cot_tas_vtva.ToString("##.0000000000");
                            RecRviCotiza_XML.val_fac_afp = x_cia_val_puur.ToString();
                            RecRviCotiza_XML.val_mto_afp = x_cia_val_puni.ToString();
                            RecRviCotiza_XML.val_pen_afp = x_cia_val_ppag.ToString();
                            RecRviCotiza_XML.val_afp_pen_ref = x_cia_val_pens.ToString();
                            if (p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.val_tasa_venta_ash_2 = x_ash_tas_vtva.ToString();
                                RecRviCotiza_XML.val_tasa_ret_accion_2 = x_ash_tas_vtra.ToString();
                                RecRviCotiza_XML.val_tasa_costo_equiv_2 = x_ash_tas_vtce.ToString();
                                RecRviCotiza_XML.val_duration_2 = x_ash_val_dura.ToString();
                                RecRviCotiza_XML.num_error_cot_2 = x_cot_num_cmsg.ToString();
                                RecRviCotiza_XML.val_tasa_cesion_moneda2 = val_tasa_cesion.ToString();
                            }
                            else
                            {
                                RecRviCotiza_XML.val_tasa_venta_ash_2 = "0";
                                RecRviCotiza_XML.val_tasa_ret_accion_2 = "0";
                                RecRviCotiza_XML.val_tasa_costo_equiv_2 = "0";
                                RecRviCotiza_XML.val_duration_2 = "0";
                                RecRviCotiza_XML.num_error_cot_2 = "0";
                                RecRviCotiza_XML.val_tasa_cesion = "0";
                                RecRviCotiza_XML.val_tasa_cesion_moneda2 = "0";
                            }
                            // verificación de cotización sin error (no alcanza el TRA mínimo)
                            if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot_2) > 0 && p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.ind_cotiza = "**";
                            }
                            if (RecRviCotiza_XML.ind_cotiza == " ")
                            {
                                if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                else
                                {
                                    if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue())
                                    {
                                        RecRviCotiza_XML.ind_cotiza = "?";
                                    }
                                }
                            }
                            RecRviCotiza_XML.val_total_garantizado = Math.Round(val_total_garantizado, 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    if (!(p.ind_orden == 2))
                    {
                        wl_XML_CostoBen = x_cot_xml_benefi.ToString().Trim();
                    }
                    if (!((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue() || p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue()) && p.ind_orden == 1))
                    {
                        if (RecRviCotiza_XML.ind_cotiza == String.Empty)
                        {
                            RecRviCotiza_XML.ind_cotiza = " ";
                        }
                        wl_XML_Cotiza += "<cotiza>";
                        wl_XML_Cotiza += " <num_solicitud>" + RecRviCotiza_XML.num_solicitud + "</num_solicitud>";
                        wl_XML_Cotiza += " <fec_cotizacion>" + p.fec_cotizacion.ToString("dd/MM/yyyy") + "</fec_cotizacion>";
                        wl_XML_Cotiza += " <num_correlativo>" + p.cot_num_coti.ToString() + "</num_correlativo>";
                        wl_XML_Cotiza += " <cod_estado_cotizacion>" + RecRviCotiza_XML.cod_estado_cotizacion + "</cod_estado_cotizacion>";
                        wl_XML_Cotiza += " <val_descuento_comision>" + RecRviCotiza_XML.val_descuento_comision + "</val_descuento_comision>";
                        wl_XML_Cotiza += " <cod_tipo_calculo>" + RecRviCotiza_XML.cod_tipo_calculo + "</cod_tipo_calculo>";
                        wl_XML_Cotiza += " <ind_cotiza>" + RecRviCotiza_XML.ind_cotiza + "</ind_cotiza>";
                        wl_XML_Cotiza += " <val_fac_cia>" + RecRviCotiza_XML.val_fac_cia + "</val_fac_cia>";
                        wl_XML_Cotiza += " <val_mto_cia>" + RecRviCotiza_XML.val_mto_cia + "</val_mto_cia>";
                        wl_XML_Cotiza += " <val_mto_cia_sin_comision>" + RecRviCotiza_XML.val_mto_cia_sin_comision + "</val_mto_cia_sin_comision>";
                        wl_XML_Cotiza += " <val_pen_cia>" + RecRviCotiza_XML.val_pen_cia + "</val_pen_cia>";
                        wl_XML_Cotiza += " <val_pen_cia_mo>" + RecRviCotiza_XML.val_pen_cia_mo + "</val_pen_cia_mo>";
                        wl_XML_Cotiza += " <val_pen_ref>" + RecRviCotiza_XML.val_pen_ref + "</val_pen_ref>";
                        wl_XML_Cotiza += " <val_pen_ref_mo>" + RecRviCotiza_XML.val_pen_ref_mo + "</val_pen_ref_mo>";
                        wl_XML_Cotiza += " <val_tasa_int_vit>" + RecRviCotiza_XML.val_tasa_int_vit + "</val_tasa_int_vit>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash>" + RecRviCotiza_XML.val_tasa_venta_ash + "</val_tasa_venta_ash>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion>" + RecRviCotiza_XML.val_tasa_ret_accion + "</val_tasa_ret_accion>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv>" + RecRviCotiza_XML.val_tasa_costo_equiv + "</val_tasa_costo_equiv>";
                        wl_XML_Cotiza += " <val_duration>" + RecRviCotiza_XML.val_duration + "</val_duration>";
                        wl_XML_Cotiza += " <num_error_cot>" + RecRviCotiza_XML.num_error_cot + "</num_error_cot>";
                        wl_XML_Cotiza += " <val_tasa_int_temp>" + RecRviCotiza_XML.val_tasa_int_temp + "</val_tasa_int_temp>";
                        wl_XML_Cotiza += " <val_fac_afp>" + RecRviCotiza_XML.val_fac_afp + "</val_fac_afp>";
                        wl_XML_Cotiza += " <val_mto_afp>" + RecRviCotiza_XML.val_mto_afp + "</val_mto_afp>";
                        wl_XML_Cotiza += " <val_pen_afp>" + RecRviCotiza_XML.val_pen_afp + "</val_pen_afp>";
                        wl_XML_Cotiza += " <val_afp_pen_ref>" + RecRviCotiza_XML.val_afp_pen_ref + "</val_afp_pen_ref>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash_2>" + RecRviCotiza_XML.val_tasa_venta_ash_2 + "</val_tasa_venta_ash_2>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion_2>" + RecRviCotiza_XML.val_tasa_ret_accion_2 + "</val_tasa_ret_accion_2>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv_2>" + RecRviCotiza_XML.val_tasa_costo_equiv_2 + "</val_tasa_costo_equiv_2>";
                        wl_XML_Cotiza += " <val_duration_2>" + RecRviCotiza_XML.val_duration_2 + "</val_duration_2>";
                        wl_XML_Cotiza += " <num_error_cot_2>" + RecRviCotiza_XML.num_error_cot_2 + "</num_error_cot_2>";
                        wl_XML_Cotiza += " <val_tasa_cesion>" + RecRviCotiza_XML.val_tasa_cesion + "</val_tasa_cesion>";
                        wl_XML_Cotiza += " <val_tasa_cesion_moneda2>" + RecRviCotiza_XML.val_tasa_cesion_moneda2 + "</val_tasa_cesion_moneda2>";
                        if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Mixta.StringValue())
                        {
                            if ((p.cod_moneda == Enums.Moneda.Dolares.StringValue()) || (p.cod_moneda == Enums.Moneda.Soles.StringValue()))
                            {
                                wl_XML_Cotiza += " <cod_moneda>" + Enums.Moneda.Dolares.StringValue() + "</cod_moneda>";
                            }
                            else if ((p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()) || (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue()))
                            {
                                wl_XML_Cotiza += " <cod_moneda>" + Enums.Moneda.DolaresAjustados.StringValue() + "</cod_moneda>";
                            }
                        }
                        else
                        {
                            wl_XML_Cotiza += " <cod_moneda>" + p.cod_moneda + "</cod_moneda>";
                        }
                        wl_XML_Cotiza += " <wl_cod_username>" + usuario + "</wl_cod_username>";
                        wl_XML_Cotiza += " <val_total_garantizado>" + RecRviCotiza_XML.val_total_garantizado + "</val_total_garantizado>";
                        wl_XML_Cotiza += "</cotiza>";
                        XML_Pje = p.wl_XML_Pje;
                        XML_CostoBen += wl_XML_CostoBen;
                        XML_Cotiza += wl_XML_Cotiza;
                        if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                        {
                            //<SOLINIGTI_754>
                            //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_2.txt");
                            if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                            tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_2.txt");
                            //<SOLFINGTI_754>
                            tw.WriteLine("");
                            tw.WriteLine("XML_INSERCION");
                            tw.WriteLine("wl_XML_Pje        : " + XML_Pje);
                            tw.WriteLine("wl_XML_CostoBen   : " + XML_CostoBen);
                            tw.WriteLine("wl_XML_Cotiza     : " + XML_Cotiza);
                            tw.Close();
                        }
                    }
                }
                string correlativos = String.Empty;
                parametros.ForEach(p => correlativos += p.cot_num_coti.ToString() + ",");
                correlativos = correlativos.Substring(0, correlativos.Length - 1);
                // Guardar los resultados en Base de Datos
                repositorioSolicitud.RegistrarPjeBeneficiarios(idSolicitud, correlativos, XML_Pje, "<insert>" + XML_CostoBen + "</insert>", usuario);
                repositorioSolicitud.RegistrarCotiza("<insert>" + XML_Cotiza + "</insert>", usuario);
            }
            catch (Exception ex)
            {
                //if (tw != null) tw.Close();
                throw;
            }
        }
        //<SRIFIN26593>

        //<INIGTI_753>
        public void CotizarRPPlus(string idSolicitud, DateTime fechaCotizacion, string usuario)
        {
            TextWriter tw = null;
            try
            {
                // Obtener los parámetros de cotización
                var parametros = repositorioSolicitudRPPlus.ObtenerParametrosCotizacionRPPlus(idSolicitud, fechaCotizacion, null);
                double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                        x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                        x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                        x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                        x_ash_tas_vtce = 0, x_ash_val_dura = 0;
                double x_cia_val_puud = 0, x_cia_val_devo = 0;//<INIGTI_753>
                StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
                int x_cot_num_cmsg = 0;
                double val_total_garantizado = 0, cia_pens_mo = 0, cia_ppag_mo = 0, cia_puni_sin_com = 0,
                val_tasa_cesion, pension_mixta_dolares_referencia = 0;
                string wl_XML_CostoBen = String.Empty, wl_XML_Cotiza = String.Empty,
                        XML_CostoBen = String.Empty, XML_Cotiza = String.Empty, XML_Pje = String.Empty;
                RviCotiza RecRviCotiza_XML = new RviCotiza();
                foreach (ParametroCotizador p in parametros)
                {
                    // Convertir el valor de la Prima Única a soles
                    p.cot_val_puni *= p.val_moneda;
                    //p.cot_val_puam *= p.val_moneda;
                    // Para el caso de las Mixtas y Combinadas que cuentan con período garantizado se debe desactivar
                    // el periodo garantizado al momento de cotizar la pensión de la AFP ya que este no debe afectarla
                    int cot_num_mgar_aux = 0;
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        cot_num_mgar_aux = p.cot_num_mgar;
                        p.cot_num_mgar = 0;
                    }
                    //<INIGTI_753>
                    //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                    int x_cot_ini_tra2 = p.x_cot_ini_tra2;
                    double x_cot_pje_rent = p.x_cot_pje_rent;
                    p.cot_num_mdif = 0;
                    p.cot_por_prrt = 0;
                    x_cia_val_puud = 0;
                    x_cia_val_devo = 0;
                    //<FINGTI_753>
                    //Imprimir Log DEGUG
                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("wl_cot_kcot        : " + p.cot_gls_ikey);
                        tw.WriteLine("wl_num_solicitud   : " + p.cot_num_soli);
                        tw.WriteLine("wl_num_corr_cotiza : " + p.cot_num_coti);
                        tw.WriteLine("wl_cot_pdif        : " + p.cot_num_mdif);
                        tw.WriteLine("wl_cot_pgar        : " + p.cot_num_mgar);
                        tw.WriteLine("wl_cot_nben        : " + p.cot_num_nben);
                        tw.WriteLine("wl_cot_fcot        : " + p.cot_fec_fcal);
                        tw.WriteLine("wl_cot_fdev        : " + p.cot_fec_fdev);
                        tw.WriteLine("wl_cot_tpen        : " + p.cot_num_tpen);
                        tw.WriteLine("wl_cot_tcal        : " + p.cot_num_tcal);
                        tw.WriteLine("wl_cot_derc        : " + p.cot_flg_idac);
                        tw.WriteLine("wl_cot_grat        : " + p.cot_flg_igra);
                        tw.WriteLine("cot_mon_equi       : " + p.cot_num_cmon);
                        tw.WriteLine("cot_num_trea       : " + p.cot_num_trea);
                        tw.WriteLine("cot_num_frea       : " + p.cot_num_frea);
                        tw.WriteLine("cot_flg_irea       : " + p.cot_flg_irea);
                        tw.WriteLine("wg_val_ajuste_tasa_fija : " + p.cot_tas_vrea);
                        tw.WriteLine("wl_val_tasa_venta  : " + p.cot_tas_tasa);
                        tw.WriteLine("wl_cot_vpen        : " + p.cot_val_vpen);
                        tw.WriteLine("wl_val_vtra        : " + p.cot_tas_vtra);
                        tw.WriteLine("wl_val_tasa_afp    : " + p.cot_tas_tafp);
                        tw.WriteLine("wl_val_acom        : " + p.cot_val_acom);
                        tw.WriteLine("wl_cot_pdco        : " + p.cot_val_dcom);
                        tw.WriteLine("wl_val_puam        : " + p.cot_val_puam);
                        tw.WriteLine("wl_val_puni        : " + p.cot_val_puni);
                        tw.WriteLine("wl_cot_prrt        : " + p.cot_por_prrt);
                        tw.WriteLine("wl_val_tgfi        : " + p.cot_val_tgfi);
                        tw.WriteLine("wl_val_tope        : " + p.cot_val_tope);
                        tw.WriteLine("arr_ppu_vllx       : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx[i, j]);
                        //tw.WriteLine("arr_ppu_aju_vllx   : ");
                        //for (int i = 0; i <= 10568; i++)
                        //    for (int j = 0; j <= 3; j++)
                        //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_arr_vllx[i, j]);
                        tw.WriteLine("arr_ppu_vllx_cot   : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx_cot[i, j]);
                        tw.WriteLine("cot_xml_ben        : " + p.cot_xml_benefi);
                        tw.WriteLine("cot_xml_icob       : " + p.cot_xml_tabico);
                        tw.WriteLine("cot_xml_ash        : " + p.cot_xml_parash);
                        tw.WriteLine("cot_xml_inv        : " + p.cot_xml_parinv);
                        tw.WriteLine("cot_xml_ajm        : " + p.cot_xml_ajutdm);
                        tw.WriteLine("cot_xml_fdv        : " + p.cot_xml_fluaju);
                        tw.WriteLine("arr_ppu_fm_cot     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm[i, j]);
                        tw.WriteLine("arr_ppu_fm_sbs     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm_sbs[i, j]);
                        tw.Close();
                    }
                    // LLamado a la DLL del cotizador (rvicotmain.dll)
                    cot_main_plus(
                        p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                        p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                        p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                        p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                        ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0], //ANTES  ref p.ppu_arr_vllx[0, 0]
                        p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                        p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                        ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                        ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                        ref x_cia_val_puud, ref x_cia_val_devo,
                        ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                        ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                        ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                        x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //<GTIINI.15930>

                    x_cia_val_ppag = Math.Round(x_cia_val_ppag, 2, MidpointRounding.AwayFromZero);

                    if (p.cot_val_tope == 0 || (p.cot_val_tope > 0 && p.cot_val_tope != p.cot_num_mgar))
                    {
                        // Si no es full garantizado, igualar la tasa IS con la tasa SBS
                        x_cot_tas_vtva = x_ash_tas_vtva;
                    }
                    //<GTIFIN.15930>
                    // Luego de calcular la pensión regresar al valor original
                    if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                    {
                        p.cot_num_mgar = cot_num_mgar_aux;
                    }
                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_1.txt");
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_1.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("Salida:");
                        tw.WriteLine("");
                        tw.WriteLine("wl_cia_vtva       : " + x_cot_tas_vtva);
                        tw.WriteLine("wl_cot_dcmm       : " + x_cot_val_mdco);
                        tw.WriteLine("wl_cia_pens       : " + x_cia_val_pens);
                        tw.WriteLine("wl_cia_ppag       : " + x_cia_val_ppag);
                        tw.WriteLine("wl_cia_puni       : " + x_cia_val_puni);
                        tw.WriteLine("wl_cia_puur       : " + x_cia_val_puur);
                        tw.WriteLine("wl_afp_pens       : " + x_afp_val_pens);
                        tw.WriteLine("wl_afp_puni       : " + x_afp_val_puni);
                        tw.WriteLine("wl_afp_puur       : " + x_afp_val_puur);
                        tw.WriteLine("wl_ash_vpen       : " + x_ash_val_vpen);
                        tw.WriteLine("wl_ash_vtra       : " + x_ash_tas_vtra);
                        tw.WriteLine("wl_ash_vtce       : " + x_ash_tas_vtce);
                        tw.WriteLine("wl_ash_dura       : " + x_ash_val_dura);
                        tw.WriteLine("cot_xml_benefi    : " + x_cot_xml_benefi.ToString().Trim());
                        tw.WriteLine("wl_num_error      : " + x_cot_num_cmsg);
                        tw.Close();
                    }
                    double x_1era_prima_is = 0;
                    double x_1era_prima_IS_sin_ajuste = 0;
                    // Para la segunda pasada no se debe volver a calcular el total garantizado
                    if (p.ind_orden == 1)
                    {
                        // Secuencia post-cotización (Traducción extraída del s_renvi)
                        val_total_garantizado = 0;
                        //<INIGTI_753>--Se Movio Aca Arriba
                        x_1era_prima_is = x_cia_val_ppag * p.fluaju[Convert.ToInt32(x_cot_ini_tra2)] / p.val_moneda;

                        //x_1era_prima_is *= (Convert.ToDouble(x_cot_pje_rent) / 100);
                        //x_1era_prima_IS_sin_ajuste = x_cia_val_ppag * (Convert.ToDouble(x_cot_pje_rent) / 100) / p.val_moneda;
                        x_1era_prima_is *= (x_cot_pje_rent / 100);
                        x_1era_prima_IS_sin_ajuste = x_cia_val_ppag * (x_cot_pje_rent / 100) / p.val_moneda;

                        x_1era_prima_is = Math.Round(x_1era_prima_is, 2, MidpointRounding.AwayFromZero);
                        x_1era_prima_IS_sin_ajuste = Math.Round(x_1era_prima_IS_sin_ajuste, 2, MidpointRounding.AwayFromZero);

                        //<FINGTI_753>
                        if (p.cot_num_mgar > 0)
                        {
                            if (p.cod_moneda == Enums.Moneda.Soles.StringValue() || p.cod_moneda == Enums.Moneda.Dolares.StringValue())
                            {
                                //<INIGTI_753>
                                if (x_cot_ini_tra2 > 0)
                                {
                                    if (p.cot_num_mgar > x_cot_ini_tra2)
                                    {
                                        val_total_garantizado = x_cia_val_ppag * x_cot_ini_tra2;
                                        val_total_garantizado += ((x_1era_prima_is * p.val_moneda) * (p.cot_num_mgar - x_cot_ini_tra2));
                                    }
                                    else
                                    {
                                        val_total_garantizado = x_cia_val_ppag * p.cot_num_mgar;
                                    }
                                }
                                else
                                {
                                    val_total_garantizado = x_cia_val_ppag * p.cot_num_mgar;
                                }
                                //val_total_garantizado = x_cia_val_ppag * p.cot_num_mgar;
                                //<FINGTI_753>
                                if (p.cod_moneda == Enums.Moneda.Dolares.StringValue())
                                {
                                    val_total_garantizado /= p.val_moneda;
                                }
                            }
                            else
                            {
                                if (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue() || p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue())
                                {
                                    //<INIGTI_753>
                                    //Total Garantizado 
                                    for (int i = 0; i < p.cot_num_mgar; i++)
                                    {
                                        if (x_cot_ini_tra2 == 0)
                                        {
                                            val_total_garantizado += (x_cia_val_ppag * p.fluaju[i]);
                                        }
                                        else
                                        {
                                            if (i < x_cot_ini_tra2)
                                            {
                                                val_total_garantizado += (x_cia_val_ppag * p.fluaju[i]);
                                            }
                                            else
                                            {
                                                val_total_garantizado += ((x_1era_prima_IS_sin_ajuste * p.val_moneda) * p.fluaju[i]);
                                            }
                                        }
                                    }
                                    ////Siguiente Tramo
                                    //for (int i = x_cot_ini_tra2; i < p.cot_num_mgar ; i++)
                                    //{
                                    //    val_total_garantizado += ((x_1era_prima_is * p.val_moneda) * p.fluaju[i]);
                                    //}
                                    //for (int i = p.cot_num_mdif; i < (p.cot_num_mgar + p.cot_num_mdif); i++)
                                    //{
                                    //    val_total_garantizado += (x_cia_val_ppag * p.fluaju[i]);
                                    //}
                                    //<FINGTI_753>
                                    // El total garantizado de la modalidad Bimoneda debe mostrarse en Soles
                                    // por lo tanto no debe convertirse.
                                    if (p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue() && p.ind_modalidad != Enums.Modalidad.Bimoneda.StringValue())
                                    {
                                        val_total_garantizado /= p.val_moneda;
                                    }
                                }
                            }
                        }
                        else
                        {
                            val_total_garantizado = 0;
                        }
                    }
                    if (x_cot_num_cmsg > 0)
                    {
                        x_cot_tas_vtva = 0; x_cot_val_mdco = 0; x_cia_val_pens = 0;
                        x_cia_val_ppag = 0; x_cia_val_puni = 0; x_cia_val_puur = 0;
                        x_afp_val_pens = 0; x_afp_val_puni = 0; x_afp_val_puur = 0;
                        x_ash_val_vpen = 0; x_ash_tas_vtce = 0; x_ash_val_dura = 0;
                        x_1era_prima_is = 0;
                    }
                    cia_puni_sin_com = x_cia_val_puni;
                    val_tasa_cesion = 0;
                    if (p.val_moneda != 0)
                    {
                        cia_pens_mo = x_cia_val_pens / p.val_moneda;
                        cia_ppag_mo = x_cia_val_ppag / p.val_moneda;
                    }
                    else
                    {
                        cia_pens_mo = 0;
                        cia_ppag_mo = 0;
                    }
                    cia_pens_mo = lib_s_round(cia_pens_mo, 2, 0);
                    cia_ppag_mo = lib_s_round(cia_ppag_mo, 2, 0);
                    // Actualiza rvi_cotiza
                    // --------------------
                    RecRviCotiza_XML.cod_estado_cotizacion = Constante.COD_COTIZACION_CALCULADA;
                    RecRviCotiza_XML.val_descuento_comision = x_cot_val_mdco.ToString();
                    RecRviCotiza_XML.cod_tipo_calculo = p.cot_num_tcal.ToString();
                    RecRviCotiza_XML.num_solicitud = p.cot_num_soli;
                    RecRviCotiza_XML.val_1era_prima_is = x_1era_prima_is;
                    RecRviCotiza_XML.val_fac_dev = x_cia_val_puud;
                    RecRviCotiza_XML.val_mto_dev = x_cia_val_devo;
                    // Si NO es Bimoneda NI Renta Mixta NI Renta Combinada
                    if (p.ind_modalidad != Enums.Modalidad.Mixta.StringValue() && p.ind_modalidad != Enums.Modalidad.Combinada.StringValue() && p.ind_modalidad != Enums.Modalidad.Bimoneda.StringValue())
                    {
                        RecRviCotiza_XML.val_fac_cia = x_cia_val_puur.ToString();
                        RecRviCotiza_XML.val_mto_cia = x_cia_val_puni.ToString();
                        RecRviCotiza_XML.val_mto_cia_sin_comision = cia_puni_sin_com.ToString();
                        RecRviCotiza_XML.val_pen_cia = x_cia_val_ppag.ToString();
                        RecRviCotiza_XML.val_pen_cia_mo = cia_ppag_mo.ToString();
                        RecRviCotiza_XML.val_pen_ref = x_cia_val_pens.ToString();
                        RecRviCotiza_XML.val_pen_ref_mo = cia_pens_mo.ToString();
                        RecRviCotiza_XML.val_tasa_int_vit = x_cot_tas_vtva.ToString("##.0000000000");
                        RecRviCotiza_XML.val_tasa_venta_ash = x_ash_tas_vtva.ToString();
                        RecRviCotiza_XML.val_tasa_ret_accion = x_ash_tas_vtra.ToString();
                        RecRviCotiza_XML.val_tasa_costo_equiv = x_ash_tas_vtce.ToString();
                        RecRviCotiza_XML.val_duration = x_ash_val_dura.ToString();
                        RecRviCotiza_XML.num_error_cot = x_cot_num_cmsg.ToString();
                        RecRviCotiza_XML.val_tasa_int_temp = p.cot_tas_tafp.ToString();
                        RecRviCotiza_XML.val_fac_afp = x_afp_val_puur.ToString();
                        RecRviCotiza_XML.val_mto_afp = x_afp_val_puni.ToString();
                        RecRviCotiza_XML.val_pen_afp = x_afp_val_pens.ToString();
                        RecRviCotiza_XML.val_afp_pen_ref = "0";
                        RecRviCotiza_XML.val_tasa_venta_ash_2 = "0";
                        RecRviCotiza_XML.val_tasa_ret_accion_2 = "0";
                        RecRviCotiza_XML.val_tasa_costo_equiv_2 = "0";
                        RecRviCotiza_XML.val_duration_2 = "0";
                        RecRviCotiza_XML.num_error_cot_2 = "0";
                        RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                        RecRviCotiza_XML.val_tasa_cesion_moneda2 = "";
                        // verificación de cotización sin error (no alcanza el TRA mínimo)
                        if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                        {
                            RecRviCotiza_XML.ind_cotiza = "**";
                        }
                        else
                        {
                            RecRviCotiza_XML.ind_cotiza = " ";
                        }
                        //<INIGTI_1092>
                        if (x_ash_tas_vtra < p.wl_val_ltra)
                        {
                            RecRviCotiza_XML.ind_cotiza = "**";
                            RecRviCotiza_XML.num_error_cot = "300";
                        }
                        //<FINGTI_1092>
                        if (RecRviCotiza_XML.ind_cotiza == " ")
                        {
                            if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                            {
                                RecRviCotiza_XML.ind_cotiza = " ";
                            }
                            else
                            {
                                if (p.ind_modalidad == Enums.Modalidad.Diferida.StringValue())
                                {
                                    RecRviCotiza_XML.ind_cotiza = "?";
                                }
                            }
                        }
                        RecRviCotiza_XML.val_total_garantizado = Math.Round(val_total_garantizado, 2, MidpointRounding.AwayFromZero);
                    }
                    // Si es Bimoneda o Renta Mixta o Renta Combinada
                    else
                    {
                        // Si es Dólares
                        if (
                            p.ind_orden == 1
                           )
                        {
                            RecRviCotiza_XML.val_fac_cia = x_cia_val_puur.ToString();
                            RecRviCotiza_XML.val_mto_cia = x_cia_val_puni.ToString();
                            RecRviCotiza_XML.val_mto_cia_sin_comision = cia_puni_sin_com.ToString();
                            RecRviCotiza_XML.val_pen_cia = x_cia_val_ppag.ToString();
                            RecRviCotiza_XML.val_pen_cia_mo = cia_ppag_mo.ToString();
                            RecRviCotiza_XML.val_pen_ref = x_cia_val_pens.ToString();
                            RecRviCotiza_XML.val_pen_ref_mo = cia_pens_mo.ToString();
                            RecRviCotiza_XML.val_tasa_int_vit = x_cot_tas_vtva.ToString("##.0000000000");
                            RecRviCotiza_XML.val_tasa_venta_ash = x_ash_tas_vtva.ToString();
                            RecRviCotiza_XML.val_tasa_ret_accion = x_ash_tas_vtra.ToString();
                            RecRviCotiza_XML.val_tasa_costo_equiv = x_ash_tas_vtce.ToString();
                            RecRviCotiza_XML.val_duration = x_ash_val_dura.ToString();
                            RecRviCotiza_XML.num_error_cot = x_cot_num_cmsg.ToString();
                            if (p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                                // verificación de cotización sin error (no alcanza el TRA mínimo)
                                if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                }
                                else
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                //<INIGTI_1092>
                                if (x_ash_tas_vtra < p.wl_val_ltra)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                    RecRviCotiza_XML.num_error_cot = "300";
                                }
                                //<FINGTI_1092>
                            }
                            else
                            {
                                RecRviCotiza_XML.val_tasa_cesion = val_tasa_cesion.ToString();
                                pension_mixta_dolares_referencia = x_cia_val_pens;
                                if ((p.cod_tipo_pension == Enums.TipoPension.InvalidezConCobertura.StringValue()) && (p.cod_tipo_invalidez == Enums.TipoInvalidez.Total.StringValue() || p.cod_tipo_invalidez == Enums.TipoInvalidez.Parcial.StringValue()))
                                {
                                    if (p.cod_tipo_invalidez == Enums.TipoInvalidez.Parcial.StringValue())
                                    {
                                        pension_mixta_dolares_referencia /= 0.5;
                                    }
                                    else if (p.cod_tipo_invalidez == Enums.TipoInvalidez.Total.StringValue())
                                    {
                                        pension_mixta_dolares_referencia /= 0.7;
                                    }
                                }
                                // verificación de pensión mínima para Renta Mixta en Dólares
                                if (pension_mixta_dolares_referencia < p.wl_val_pension_minimo)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "*";
                                }
                                else
                                {
                                    RecRviCotiza_XML.ind_cotiza = String.Empty;
                                }
                                // verificación de cotización sin error (no alcanza el TRA mínimo)
                                if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot) > 0)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                }
                                //<INIGTI_1092>
                                if (x_ash_tas_vtra < p.wl_val_ltra)
                                {
                                    RecRviCotiza_XML.ind_cotiza = "**";
                                    RecRviCotiza_XML.num_error_cot = "300";
                                }
                                //<FINGTI_1092>
                            }
                            if (RecRviCotiza_XML.ind_cotiza == " " || RecRviCotiza_XML.ind_cotiza == String.Empty)
                            {
                                if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                else
                                {
                                    if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue())
                                    {
                                        RecRviCotiza_XML.ind_cotiza = "?";
                                    }
                                }
                            }
                            RecRviCotiza_XML.val_total_garantizado = Math.Round(val_total_garantizado, 2, MidpointRounding.AwayFromZero); ;
                        }
                        // Si es la segunda pasada
                        else
                        {
                            RecRviCotiza_XML.val_tasa_int_temp = x_cot_tas_vtva.ToString("##.0000000000");
                            RecRviCotiza_XML.val_fac_afp = x_cia_val_puur.ToString();
                            RecRviCotiza_XML.val_mto_afp = x_cia_val_puni.ToString();
                            RecRviCotiza_XML.val_pen_afp = x_cia_val_ppag.ToString();
                            RecRviCotiza_XML.val_afp_pen_ref = x_cia_val_pens.ToString();
                            if (p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.val_tasa_venta_ash_2 = x_ash_tas_vtva.ToString();
                                RecRviCotiza_XML.val_tasa_ret_accion_2 = x_ash_tas_vtra.ToString();
                                RecRviCotiza_XML.val_tasa_costo_equiv_2 = x_ash_tas_vtce.ToString();
                                RecRviCotiza_XML.val_duration_2 = x_ash_val_dura.ToString();
                                RecRviCotiza_XML.num_error_cot_2 = x_cot_num_cmsg.ToString();
                                RecRviCotiza_XML.val_tasa_cesion_moneda2 = val_tasa_cesion.ToString();
                            }
                            else
                            {
                                RecRviCotiza_XML.val_tasa_venta_ash_2 = "0";
                                RecRviCotiza_XML.val_tasa_ret_accion_2 = "0";
                                RecRviCotiza_XML.val_tasa_costo_equiv_2 = "0";
                                RecRviCotiza_XML.val_duration_2 = "0";
                                RecRviCotiza_XML.num_error_cot_2 = "0";
                                RecRviCotiza_XML.val_tasa_cesion = "0";
                                RecRviCotiza_XML.val_tasa_cesion_moneda2 = "0";
                            }
                            // verificación de cotización sin error (no alcanza el TRA mínimo)
                            if (Convert.ToInt32(RecRviCotiza_XML.num_error_cot_2) > 0 && p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue())
                            {
                                RecRviCotiza_XML.ind_cotiza = "**";
                            }
                            //<INIGTI_1092>
                            if (x_ash_tas_vtra < p.wl_val_ltra)
                            {
                                RecRviCotiza_XML.ind_cotiza = "**";
                                RecRviCotiza_XML.num_error_cot = "300";
                            }
                            //<FINGTI_1092>
                            if (RecRviCotiza_XML.ind_cotiza == " ")
                            {
                                if (p.val_ltit <= p.cot_tas_tafp && p.cot_tas_tafp <= p.val_htit)
                                {
                                    RecRviCotiza_XML.ind_cotiza = " ";
                                }
                                else
                                {
                                    if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue())
                                    {
                                        RecRviCotiza_XML.ind_cotiza = "?";
                                    }
                                }
                            }
                            RecRviCotiza_XML.val_total_garantizado = Math.Round(val_total_garantizado, 2, MidpointRounding.AwayFromZero); ;
                        }
                    }
                    if (!(p.ind_orden == 2))
                    {
                        wl_XML_CostoBen = x_cot_xml_benefi.ToString().Trim();
                    }
                    if (!((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue() || p.ind_modalidad == Enums.Modalidad.Bimoneda.StringValue()) && p.ind_orden == 1))
                    {
                        if (RecRviCotiza_XML.ind_cotiza == String.Empty)
                        {
                            RecRviCotiza_XML.ind_cotiza = " ";
                        }
                        wl_XML_Cotiza += "<cotiza>";
                        wl_XML_Cotiza += " <num_solicitud>" + RecRviCotiza_XML.num_solicitud + "</num_solicitud>";
                        wl_XML_Cotiza += " <fec_cotizacion>" + p.fec_cotizacion.ToString("dd/MM/yyyy") + "</fec_cotizacion>";
                        wl_XML_Cotiza += " <num_correlativo>" + p.cot_num_coti.ToString() + "</num_correlativo>";
                        wl_XML_Cotiza += " <cod_estado_cotizacion>" + RecRviCotiza_XML.cod_estado_cotizacion + "</cod_estado_cotizacion>";
                        wl_XML_Cotiza += " <val_descuento_comision>" + RecRviCotiza_XML.val_descuento_comision + "</val_descuento_comision>";
                        wl_XML_Cotiza += " <cod_tipo_calculo>" + RecRviCotiza_XML.cod_tipo_calculo + "</cod_tipo_calculo>";
                        wl_XML_Cotiza += " <ind_cotiza>" + RecRviCotiza_XML.ind_cotiza + "</ind_cotiza>";
                        wl_XML_Cotiza += " <val_fac_cia>" + RecRviCotiza_XML.val_fac_cia + "</val_fac_cia>";
                        wl_XML_Cotiza += " <val_mto_cia>" + RecRviCotiza_XML.val_mto_cia + "</val_mto_cia>";
                        wl_XML_Cotiza += " <val_mto_cia_sin_comision>" + RecRviCotiza_XML.val_mto_cia_sin_comision + "</val_mto_cia_sin_comision>";
                        wl_XML_Cotiza += " <val_pen_cia>" + RecRviCotiza_XML.val_pen_cia + "</val_pen_cia>";
                        wl_XML_Cotiza += " <val_pen_cia_mo>" + RecRviCotiza_XML.val_pen_cia_mo + "</val_pen_cia_mo>";
                        wl_XML_Cotiza += " <val_pen_ref>" + RecRviCotiza_XML.val_pen_ref + "</val_pen_ref>";
                        wl_XML_Cotiza += " <val_pen_ref_mo>" + RecRviCotiza_XML.val_pen_ref_mo + "</val_pen_ref_mo>";
                        wl_XML_Cotiza += " <val_tasa_int_vit>" + RecRviCotiza_XML.val_tasa_int_vit + "</val_tasa_int_vit>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash>" + RecRviCotiza_XML.val_tasa_venta_ash + "</val_tasa_venta_ash>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion>" + RecRviCotiza_XML.val_tasa_ret_accion + "</val_tasa_ret_accion>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv>" + RecRviCotiza_XML.val_tasa_costo_equiv + "</val_tasa_costo_equiv>";
                        wl_XML_Cotiza += " <val_duration>" + RecRviCotiza_XML.val_duration + "</val_duration>";
                        wl_XML_Cotiza += " <num_error_cot>" + RecRviCotiza_XML.num_error_cot + "</num_error_cot>";
                        wl_XML_Cotiza += " <val_tasa_int_temp>" + RecRviCotiza_XML.val_tasa_int_temp + "</val_tasa_int_temp>";
                        wl_XML_Cotiza += " <val_fac_afp>" + RecRviCotiza_XML.val_fac_afp + "</val_fac_afp>";
                        wl_XML_Cotiza += " <val_mto_afp>" + RecRviCotiza_XML.val_mto_afp + "</val_mto_afp>";
                        wl_XML_Cotiza += " <val_pen_afp>" + RecRviCotiza_XML.val_pen_afp + "</val_pen_afp>";
                        wl_XML_Cotiza += " <val_afp_pen_ref>" + RecRviCotiza_XML.val_afp_pen_ref + "</val_afp_pen_ref>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash_2>" + RecRviCotiza_XML.val_tasa_venta_ash_2 + "</val_tasa_venta_ash_2>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion_2>" + RecRviCotiza_XML.val_tasa_ret_accion_2 + "</val_tasa_ret_accion_2>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv_2>" + RecRviCotiza_XML.val_tasa_costo_equiv_2 + "</val_tasa_costo_equiv_2>";
                        wl_XML_Cotiza += " <val_duration_2>" + RecRviCotiza_XML.val_duration_2 + "</val_duration_2>";
                        wl_XML_Cotiza += " <num_error_cot_2>" + RecRviCotiza_XML.num_error_cot_2 + "</num_error_cot_2>";
                        wl_XML_Cotiza += " <val_tasa_cesion>" + RecRviCotiza_XML.val_tasa_cesion + "</val_tasa_cesion>";
                        wl_XML_Cotiza += " <val_tasa_cesion_moneda2>" + RecRviCotiza_XML.val_tasa_cesion_moneda2 + "</val_tasa_cesion_moneda2>";
                        if (p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Mixta.StringValue())
                        {
                            if ((p.cod_moneda == Enums.Moneda.Dolares.StringValue()) || (p.cod_moneda == Enums.Moneda.Soles.StringValue()))
                            {
                                wl_XML_Cotiza += " <cod_moneda>" + Enums.Moneda.Dolares.StringValue() + "</cod_moneda>";
                            }
                            else if ((p.cod_moneda == Enums.Moneda.DolaresAjustados.StringValue()) || (p.cod_moneda == Enums.Moneda.SolesAjustados.StringValue()))
                            {
                                wl_XML_Cotiza += " <cod_moneda>" + Enums.Moneda.DolaresAjustados.StringValue() + "</cod_moneda>";
                            }
                        }
                        else
                        {
                            wl_XML_Cotiza += " <cod_moneda>" + p.cod_moneda + "</cod_moneda>";
                        }
                        wl_XML_Cotiza += " <wl_cod_username>" + usuario + "</wl_cod_username>";
                        wl_XML_Cotiza += " <val_total_garantizado>" + RecRviCotiza_XML.val_total_garantizado + "</val_total_garantizado>";
                        wl_XML_Cotiza += " <val_1era_prima_is>" + RecRviCotiza_XML.val_1era_prima_is + "</val_1era_prima_is>";
                        wl_XML_Cotiza += " <val_fac_dev>" + RecRviCotiza_XML.val_fac_dev + "</val_fac_dev>";
                        wl_XML_Cotiza += " <val_mto_dev>" + RecRviCotiza_XML.val_mto_dev + "</val_mto_dev>";
                        wl_XML_Cotiza += "</cotiza>";
                        XML_Pje = p.wl_XML_Pje;
                        XML_CostoBen += wl_XML_CostoBen;
                        XML_Cotiza += wl_XML_Cotiza;
                        if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                        {
                            //<SOLINIGTI_754>
                            //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_2.txt");
                            if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                            tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_2.txt");
                            //<SOLFINGTI_754>
                            tw.WriteLine("");
                            tw.WriteLine("XML_INSERCION");
                            tw.WriteLine("wl_XML_Pje        : " + XML_Pje);
                            tw.WriteLine("wl_XML_CostoBen   : " + XML_CostoBen);
                            tw.WriteLine("wl_XML_Cotiza     : " + XML_Cotiza);
                            tw.Close();
                        }
                    }
                }
                string correlativos = String.Empty;
                parametros.ForEach(p => correlativos += p.cot_num_coti.ToString() + ",");
                correlativos = correlativos.Substring(0, correlativos.Length - 1);
                // Guardar los resultados en Base de Datos
                repositorioSolicitudRPPlus.RegistrarPjeBeneficiarios(idSolicitud, correlativos, XML_Pje, "<insert>" + XML_CostoBen + "</insert>", usuario);
                repositorioSolicitudRPPlus.RegistrarCotiza("<insert>" + XML_Cotiza + "</insert>", usuario);
            }
            catch (Exception ex)
            {
                //if (tw != null) tw.Close();
                throw;
            }
        }
        //<FINGTI_753>

        //<SRIINI06326>
        public void GenerarReporteEscenarios(string idSolicitud, DateTime fechaCotizacion, string usuario, string maxAcom)
        {
            try
            {
                var listaAdelantos = repositorioAdelantoComision.ListarAdelantos();
                //<INIGTI_4022>
                var listaDescuentos = repositorioDescuentoComision.ListarDescuentos(fechaCotizacion);
                //<FINGTI_4022>
                var parametros = repositorioSolicitud.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, null, 0, null);
                var solicitud = repositorioSolicitud.ObtenerDatos(idSolicitud, fechaCotizacion);
                var cotizaciones = solicitud.Cotizaciones;
                double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                   x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                   x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                   x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                   x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
                StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
                int x_cot_num_cmsg = 0;
                double valComisionAgente = 0;
                double valParam = 0;
                double x_cia_val_ppag_mo = 0;
                string wl_XML_Cotiza = String.Empty;
                double? valMaximo = -1;
                if (maxAcom.Trim().Length > 0)
                {
                    valMaximo = Convert.ToDouble(maxAcom, new CultureInfo("es-PE")); //obtenerMaxComisionAdelanto(solicitud, fechaCotizacion);
                }
                else
                    valMaximo = obtenerMaxComisionAdelanto(solicitud, fechaCotizacion);
                if (solicitud.PorcentajeAumentoComision.HasValue)
                {
                    if (solicitud.PorcentajeAumentoComision > valMaximo)
                        valMaximo = solicitud.PorcentajeAumentoComision;
                }
                foreach (var cot in cotizaciones)
                {
                    //var parametrosCot = repositorioSolicitud.ObtenerParametrosCotizacion(idSolicitud, fechaCotizacion, cot.Correlativo, 0, null);
                    var parametrosCot = parametros.FindAll(p => (p.cot_num_coti == cot.Correlativo));
                    parametrosCot.ForEach(p =>
                    {
                        listaDescuentos.ForEach(des =>
                        {
                            foreach (var ade in listaAdelantos)
                            //listaAdelantos.ForEach(ade =>
                            {
                                if (ade.ValorAdicional > valMaximo)
                                {
                                    continue;
                                }
                                //<SRIINI18360>
                                // Para el caso de las Mixtas y Combinadas que cuentan con período garantizado se debe desactivar
                                // el periodo garantizado al momento de cotizar la pensión de la AFP ya que este no debe afectarla
                                if ((p.ind_modalidad == Enums.Modalidad.Mixta.StringValue() || p.ind_modalidad == Enums.Modalidad.Combinada.StringValue()) && p.ind_orden == 2)
                                {
                                    p.cot_num_mgar = 0;
                                }
                                //<SRIFIN18360>
                                // LLamado a la DLL del cotizador (rvicotmain.dll)
                                //cot_main(
                                //    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                                //    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                                //    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                                //    ade.ValorAdicional, des.ValorAdicional, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi,
                                //    ref p.ppu_vllx[0, 0], ref p.ppu_arr_vllx[0, 0],
                                //    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                                //    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                                //    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                                //    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                                //    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                                //    x_cot_xml_benefi, ref x_cot_num_cmsg);
                                //<SOLINIGTI_754>
                                //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                                int x_cot_ini_tra2 = 0;
                                double x_cot_pje_rent = 0;
                                if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                                {
                                    x_cot_ini_tra2 = p.x_cot_ini_tra2;
                                    x_cot_pje_rent = p.x_cot_pje_rent;
                                    p.cot_num_mdif = 0;
                                    p.cot_por_prrt = 0;
                                }
                                //<SOLFINGTI_754>
                                //<GTI.INI-15819>
                                string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                                ////<GTIINI-6489>
                                ////cot_main(
                                ////      p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                                ////      p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                                ////      p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                                ////      ade.ValorAdicional, des.ValorAdicional, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                                ////      ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                                ////      p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                                ////      p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                                ////      ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                                ////      ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                                ////      ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                                ////      ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                                ////      ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                                ////      x_cot_xml_benefi, ref x_cot_num_cmsg);
                                //cot_main(
                                //      p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                                //      p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                                //      p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                                //      ade.ValorAdicional, des.ValorAdicional, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                                //      ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                                //      p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                                //      p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                                //      ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                                //      ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                                //      ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                                //      ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                                //      ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                                //      x_cot_xml_benefi, ref x_cot_num_cmsg);
                                ////<GTIFIN-6489>
                                cot_main(
                      cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                      p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                      p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                      ade.ValorAdicional, des.ValorAdicional, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                      ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                      p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                      p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                      ref p.cot_fac_dto[0],
                      ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                      ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                      ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                      ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                      ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                      x_cot_xml_benefi, ref x_cot_num_cmsg);
                                //<GTI.FIN-15819>
                                if (x_cot_num_cmsg > 0)
                                {
                                    throw new Exception("Error " + x_cot_num_cmsg + " en la cotización, asegúrese de que la CIC sea lo suficientemente grande para soportar ACOM y DCOM.");
                                }
                                valParam = repositorioParametroGeneral.obtenerValorComisionAgente(p.cod_moneda);
                                valComisionAgente = valParam * x_cia_val_puni * ade.ValorAdicional / 100;
                                if ((p.cod_moneda == Enums.Moneda.Dolares.ToString()) || (p.cod_moneda == Enums.Moneda.DolaresAjustados.ToString()))
                                {
                                    if (p.val_moneda != 0)
                                        x_cia_val_ppag_mo = x_cia_val_ppag / p.val_moneda;
                                    else
                                        x_cia_val_ppag_mo = 0;
                                }
                                else
                                    x_cia_val_ppag_mo = x_cia_val_ppag;
                                x_cia_val_ppag_mo = lib_s_round(x_cia_val_ppag_mo, 2, 0);
                                wl_XML_Cotiza += "<formato>";
                                wl_XML_Cotiza += " <num_solicitud>" + p.cot_num_soli + "</num_solicitud>";
                                wl_XML_Cotiza += " <num_cotizacion>" + p.cot_num_coti + "</num_cotizacion>";
                                wl_XML_Cotiza += " <cod_moneda>" + p.cod_moneda + "</cod_moneda>";
                                wl_XML_Cotiza += " <val_cesion_comision>" + des.ValorAdicional + "</val_cesion_comision>";
                                wl_XML_Cotiza += " <val_anticipo_comision>" + ade.ValorAdicional + "</val_anticipo_comision>";
                                wl_XML_Cotiza += " <val_pension>" + x_cia_val_ppag + "</val_pension>";
                                wl_XML_Cotiza += " <val_pension_mo>" + x_cia_val_ppag_mo + "</val_pension_mo>";
                                wl_XML_Cotiza += " <val_pension_afp>" + x_afp_val_pens + "</val_pension_afp>";
                                wl_XML_Cotiza += " <val_mto_agente>" + valComisionAgente + "</val_mto_agente>";
                                wl_XML_Cotiza += " <gls_corta_cesion>" + des.Glosa + "</gls_corta_cesion>";
                                wl_XML_Cotiza += " <gls_corta_anticipo>" + ade.Glosa + "</gls_corta_anticipo>";
                                wl_XML_Cotiza += " <cod_modalidad>" + cot.Modalidad.Id + "</cod_modalidad>";
                                wl_XML_Cotiza += " <cod_username>" + usuario + "</cod_username>";
                                wl_XML_Cotiza += "</formato>";
                                valComisionAgente = 0;
                                valParam = 0;
                                x_cia_val_ppag_mo = 0;
                                x_cot_tas_vtva = 0;
                                x_cot_val_mdco = 0;
                                x_cia_val_pens = 0;
                                x_cia_val_ppag = 0;
                                x_cia_val_puni = 0;
                                x_cia_val_puur = 0;
                                x_afp_val_pens = 0;
                                x_afp_val_puni = 0;
                                x_afp_val_puur = 0;
                                x_ash_val_vpen = 0;
                                x_ash_tas_vtva = 0;
                                x_ash_tas_vtra = 0;
                                x_ash_tas_vtce = 0;
                                x_ash_val_dura = 0;
                            }  //);
                        });
                    });
                }
                wl_XML_Cotiza = "<insert>" + wl_XML_Cotiza + "</insert>";
                repositorioEscenario.Eliminar(idSolicitud, usuario);
                repositorioEscenario.Registrar(wl_XML_Cotiza, usuario);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet ObtenerReporteEscenarios(string idSolicitud, string usuario)
        {
            var data = repositorioEscenario.ObtenerDatosReporte(idSolicitud, usuario);
            return data;
        }
        private double obtenerMaxComisionAdelanto(Solicitud solicitud, DateTime fechaCotizacion)
        {
            double valor = -1;
            AdelantoComision paramConfig = new AdelantoComision();
            AdelantoComision respConfig = null;
            paramConfig.MontoCIC = Convert.ToDecimal(solicitud.SaldoCIC);
            paramConfig.FechaCotizacion = fechaCotizacion;
            paramConfig.Afp = solicitud.AFP;
            paramConfig.CodigoTipoPension = solicitud.TipoPension.Id.ToString();
            paramConfig.IndReja = solicitud.Categoria.Id == "N" ? "S" : "N";
            respConfig = repositorioAdelantoComision.ObtenerConfiguracionAdelanto(paramConfig);
            if (respConfig == null)
            {
                paramConfig.Afp = new AFP { Id = "-" };
                respConfig = repositorioAdelantoComision.ObtenerConfiguracionAdelanto(paramConfig);
            }
            if (respConfig != null)
                valor = respConfig.ValAcom;
            return valor;
        }
        //<SRIFIN06326>
        //<SRIINI20322>
        #region ROLACOM
        public List<RolAcom> ListarRolAcom(RolAcom rolAcom)
        {
            var roles = repositorioRolAcom.ListarRolAcom(rolAcom);
            return roles;
        }
        public List<RolAcom> ListaAcomEscenario(RolAcom rolAcom)
        {
            var solicitud = repositorioSolicitud.ObtenerDatos(rolAcom.NumSolicitud, rolAcom.FechaCotizacion);
            double? valMaximo = -1;
            valMaximo = obtenerMaxComisionAdelanto(solicitud, rolAcom.FechaCotizacion);
            rolAcom.ValorAcomMaximo = valMaximo;
            var roles = repositorioRolAcom.ListaAcomEscenario(rolAcom);
            return roles;
        }
        #endregion
        #region ROLDCOM
        public List<RolDcom> ListarRolDcom(RolDcom rolDcom)
        {
            var roles = repositorioRolDcom.ListarRolDcom(rolDcom);
            return roles;
        }
        //<GTIINI-10761>
        public List<RolDcom> ListarRolDcomRPP(RolDcom rolDcom)
        {
            List<RolDcom> lstRango = repositorioRolDcom.ListarRolDcomRPP(rolDcom);
            return lstRango;
        }
        //<GTIFIN-10761>
        public List<RolDcom> ListaDcomEscenario(RolDcom rolDcom)
        {
            var solicitud = repositorioSolicitud.ObtenerDatos(rolDcom.NumSolicitud, rolDcom.FechaCotizacion);
            double? valMaximo = -1;
            valMaximo = obtenerMaxComisionAdelanto(solicitud, rolDcom.FechaCotizacion);
            rolDcom.ValorDcomMaximo = valMaximo;
            var roles = repositorioRolDcom.ListaDcomEscenario(rolDcom);
            return roles;
        }
        #endregion
        #region ROLDTRA
        public List<RolDtra> ListarRolDtra(RolDtra rolDtra)
        {
            var roles = repositorioRolDtra.ListarRolDtra(rolDtra);
            return roles;
        }
        #endregion
        #region Meler
        public int RegistrarDescargaSolicitudes(string xml, string usuario)
        {
            int lote = 0;
            lote = repositorioSolicitud.RegistrarDescargaSolicitudes(xml, usuario);
            return lote;
        }

        public List<Solicitud> RegistrarDescargaResultados(string xml, string usuario, ref int lote)
        {
            List<Solicitud> solicitudes = null;
            solicitudes = repositorioSolicitud.RegistrarDescargaResultados(xml, usuario, ref lote);
            return solicitudes;
        }

        public List<Lote> ListarLote(int numero)
        {
            var lotes = repositorioLote.Listar(numero);
            return lotes;
        }
        public List<Lote> ListarLote(DateTime fechaCierreInicial, DateTime fechaCierreFinal)
        {
            var lotes = repositorioLote.Listar(fechaCierreInicial, fechaCierreFinal);
            return lotes;
        }

        public List<Lote> ListarLoteResultado(int numero)
        {
            var lotes = repositorioLote.ListarResultado(numero);
            return lotes;
        }
        public List<Lote> ListarLoteResultado(DateTime fechaCierreInicial, DateTime fechaCierreFinal)
        {
            var lotes = repositorioLote.ListarResultado(fechaCierreInicial, fechaCierreFinal);
            return lotes;
        }
        #endregion
        #region Anticipo
        public Anticipo ObtenerDatosAnticipo(string solicitud)
        {
            var anticipo = repositorioAnticipo.ObtenerDatos(solicitud);
            return anticipo;
        }
        public Anticipo ObtenerDatosAnticipoAceptacion(string solicitud, string agente)
        {
            var anticipo = repositorioAnticipo.ObtenerDatosAceptacion(solicitud, agente);
            return anticipo;
        }
        public Anticipo ObtenerDatosAnticipoCondiciones(string solicitud)
        {
            var anticipo = repositorioAnticipo.ObtenerDatosCondiciones(solicitud);
            return anticipo;
        }
        public List<Anticipo> ListarAnticipoAceptacion(string solicitud, string agente, DateTime? fechaInicio, DateTime? fechaFin, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar)
        {
            var anticipos = repositorioAnticipo.ListarAceptacion(solicitud, agente, fechaInicio, fechaFin, indicePagina, tamanhoPagina, columnaOrdenar, direccionOrdenar);
            return anticipos;
        }
        public void RegistrarAnticipoAceptacion(Anticipo anticipo)
        {
            repositorioAnticipo.RegistrarAceptacion(anticipo);
        }
        #endregion
        //<SRIFIN20322>
        //<SRIINI26593>
        #region Renta Privada
        public List<SolicitudRP> ListarSolicitudRP(string cuspp)
        {
            var solicitud = repositorioSolicitudRP.Listar(cuspp);
            return solicitud;
        }
        public void RegistrarSolicitudRP(ref SolicitudRP solicitud)
        {
            repositorioSolicitudRP.Registrar(ref solicitud);
            CotizarRP(solicitud.Id, (DateTime)solicitud.FechaCotizacion, solicitud.Usuario.NombreUsuario);
        }
        public void ActualizarSolicitudRP(ref SolicitudRP solicitud)
        {
            repositorioSolicitudRP.Actualizar(ref solicitud);
            CotizarRP(solicitud.Id, (DateTime)solicitud.FechaCotizacion, solicitud.Usuario.NombreUsuario);
        }
        #endregion
        //<SRIFIN26593>
        //<SOLINI25621>
        #region Capital Requerido
        public void CotizarCapitalRequerido(ref CapitalRequerido capitalRequerido)
        {
            try
            {
                //pension_inicial, pension_final, prima_unica_inicial, prima_unica_final, prioridad
                double x_pension_inicial = 0, x_pension_final = 0,
                    x_prima_unica_inicial = 0, x_prima_unica_final = 0,
                    x_prioridad = 0, x_aumento_porcentaje = 0;
                double x_pension_requerida = 0;
                //Renta Vitalicia
                if (capitalRequerido.TipoRenta.Id == "RVI")
                {
                    var parametros = repositorioSolicitud.ObtenerParametrosCapitalRequerido(capitalRequerido);
                    //double x = parametros[0].rango_capital[0, 1];
                    for (int i = 0; i <= (parametros[0].rango_capital.Length / 6) - 1; i++)
                    {
                        x_pension_inicial = (double)parametros[0].rango_capital[i, 0];
                        x_pension_final = (double)parametros[0].rango_capital[i, 1];
                        x_prima_unica_inicial = (double)parametros[0].rango_capital[i, 2];
                        x_prima_unica_final = (double)parametros[0].rango_capital[i, 3];
                        x_prioridad = (double)parametros[0].rango_capital[i, 4];
                        x_aumento_porcentaje = (double)parametros[0].rango_capital[i, 5];
                        x_pension_requerida = capitalRequerido.pension_requerida * (1 + (x_aumento_porcentaje / 100));
                        capitalRequerido.capital_requerido = x_prima_unica_inicial;
                        if (capitalRequerido.capital_requerido != 0)
                        {
                            CalcularCapital(parametros, ref capitalRequerido);
                            //if ((Int32)x_pension_requerida == (Int32)capitalRequerido.pension_calculada)
                            if (lib_s_round(x_pension_requerida, 2, 0) == lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                            {
                                return;
                            }
                            //else if ((Int32)x_pension_requerida < (Int32)capitalRequerido.pension_calculada)
                            else if (lib_s_round(x_pension_requerida, 2, 0) < lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                            {
                                continue;
                            }
                        }
                        capitalRequerido.capital_requerido = x_prima_unica_final;
                        CalcularCapital(parametros, ref capitalRequerido);
                        //if ((Int32)x_pension_requerida == (Int32)capitalRequerido.pension_calculada)
                        if (lib_s_round(x_pension_requerida, 2, 0) == lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                        {
                            return;
                        }
                        //else if ((Int32)x_pension_requerida > (Int32)capitalRequerido.pension_calculada)
                        else if (lib_s_round(x_pension_requerida, 2, 0) > lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                        {
                            continue;
                        }
                        while (capitalRequerido.pension_calculada != x_pension_requerida)
                        {
                            double x_diferencial = (x_prima_unica_final - x_prima_unica_inicial) / 2;
                            capitalRequerido.capital_requerido = x_prima_unica_inicial + x_diferencial;
                            CalcularCapital(parametros, ref capitalRequerido);
                            //if ((Int32)x_pension_requerida == (Int32)capitalRequerido.pension_calculada)
                            if (lib_s_round(x_pension_requerida, 2, 0) == lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                            {
                                return;
                            }
                            if (x_pension_requerida > capitalRequerido.pension_calculada)
                            {
                                x_prima_unica_inicial = x_prima_unica_inicial + x_diferencial;
                            }
                            if (x_pension_requerida < capitalRequerido.pension_calculada)
                            {
                                x_prima_unica_final = x_prima_unica_final - x_diferencial;
                            }
                            if (lib_s_round(x_diferencial, 2, 0) == 0 && capitalRequerido.error != 0)
                            {
                                throw new Exception("Cotización no alcanza mínimo requerido, por lo cual no se simula");
                            }
                            if (lib_s_round(x_diferencial, 2, 0) == 0 && capitalRequerido.error == 0)
                            {
                                return;
                            }
                            //if (lib_s_round(x_diferencial, 2, 0) == 0 && (capitalRequerido.error == 0 || capitalRequerido.error == 300))
                            //{
                            //    throw new Exception("Cotización no alcanza mínimo requerido, por lo cual no se simula");
                            //}
                            //if (lib_s_round(x_diferencial, 2, 0) != 0 && capitalRequerido.error == 300)
                            //{
                            //    throw new Exception("Cotización no alcanza mínimo requerido, por lo cual no se simula");
                            //}
                        }
                        throw new Exception("Cotización no alcanza mínimo requerido, por lo cual no se simula");
                    }
                    throw new Exception("Cotización no alcanza mínimo requerido, por lo cual no se simula");
                }
                //Renta Privada
                else
                {
                    var parametros = repositorioSolicitudRP.ObtenerParametrosCapitalRequeridoRP(capitalRequerido);
                    if (parametros[0].cot_val_puam <= 0)
                    {
                        throw new Exception("El afiliado no tiene registrado el Saldo CIC");
                    }
                    //double x = parametros[0].rango_capital[0, 1];
                    for (int i = 0; i <= (parametros[0].rango_capital.Length / 6) - 1; i++)
                    {
                        x_pension_inicial = (double)parametros[0].rango_capital[i, 0];
                        x_pension_final = (double)parametros[0].rango_capital[i, 1];
                        x_prima_unica_inicial = (double)parametros[0].rango_capital[i, 2];
                        x_prima_unica_final = (double)parametros[0].rango_capital[i, 3];
                        x_prioridad = (double)parametros[0].rango_capital[i, 4];
                        x_aumento_porcentaje = (double)parametros[0].rango_capital[i, 5];
                        x_pension_requerida = capitalRequerido.pension_requerida * (1 + (x_aumento_porcentaje / 100));
                        capitalRequerido.capital_requerido = x_prima_unica_inicial;
                        if (capitalRequerido.capital_requerido != 0)
                        {
                            CalcularCapitalRP(parametros, ref capitalRequerido);
                            //if ((Int32)x_pension_requerida == (Int32)capitalRequerido.pension_calculada)
                            if (lib_s_round(x_pension_requerida, 2, 0) == lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                            {
                                return;
                            }
                            //else if ((Int32)x_pension_requerida < (Int32)capitalRequerido.pension_calculada)
                            else if (lib_s_round(x_pension_requerida, 2, 0) < lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                            {
                                continue;
                            }
                        }
                        capitalRequerido.capital_requerido = x_prima_unica_final;
                        CalcularCapitalRP(parametros, ref capitalRequerido);
                        //if ((Int32)x_pension_requerida == (Int32)capitalRequerido.pension_calculada)
                        if (lib_s_round(x_pension_requerida, 2, 0) == lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                        {
                            return;
                        }
                        //else if ((Int32)x_pension_requerida > (Int32)capitalRequerido.pension_calculada)
                        else if (lib_s_round(x_pension_requerida, 2, 0) > lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                        {
                            continue;
                        }
                        while (capitalRequerido.pension_calculada != x_pension_requerida)
                        {
                            double x_diferencial = (x_prima_unica_final - x_prima_unica_inicial) / 2;
                            capitalRequerido.capital_requerido = x_prima_unica_inicial + x_diferencial;
                            CalcularCapitalRP(parametros, ref capitalRequerido);
                            //if ((Int32)x_pension_requerida == (Int32)capitalRequerido.pension_calculada)
                            if (lib_s_round(x_pension_requerida, 2, 0) == lib_s_round(capitalRequerido.pension_calculada, 2, 0))
                            {
                                return;
                            }
                            if (x_pension_requerida > capitalRequerido.pension_calculada)
                            {
                                x_prima_unica_inicial = x_prima_unica_inicial + x_diferencial;
                            }
                            if (x_pension_requerida < capitalRequerido.pension_calculada)
                            {
                                x_prima_unica_final = x_prima_unica_final - x_diferencial;
                            }
                            if (lib_s_round(x_diferencial, 2, 0) == 0 && capitalRequerido.error != 0)
                            {
                                throw new Exception("Cotización no alcanza mínimo requerido, por lo cual no se simula");
                            }
                            if (lib_s_round(x_diferencial, 2, 0) == 0 && capitalRequerido.error == 0)
                            {
                                return;
                            }
                            //if (lib_s_round(x_diferencial, 1, 0) == 0 && (capitalRequerido.error == 0 || capitalRequerido.error == 300))
                            //{
                            //    throw new Exception("Cotización no alcanza mínimo requerido, por lo cual no se simula");
                            //}
                        }
                        throw new Exception("Cotización no alcanza mínimo requerido, por lo cual no se simula");
                    }
                    throw new Exception("Cotización no alcanza mínimo requerido, por lo cual no se simula");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void CalcularCapital(List<ParametroCotizador> parametros, ref CapitalRequerido capitalRequerido)
        {
            TextWriter tw = null;
            try
            {
                double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                        x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                        x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                        x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                        x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
                StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
                int x_cot_num_cmsg = 0;
                double x_val_ajutra = 0;
                double cia_pens_mo = 0, cia_ppag_mo = 0, cia_puni_sin_com = 0;
                string wl_XML_CostoBen = String.Empty, wl_XML_Cotiza = String.Empty,
                        XML_CostoBen = String.Empty, XML_Cotiza = String.Empty, XML_Pje = String.Empty;
                RviCotiza RecRviCotiza_XML = new RviCotiza();
                foreach (ParametroCotizador p in parametros)
                {
                    p.cot_val_puni = capitalRequerido.capital_requerido; //x_prima_unica_inicial;
                    p.cot_val_puam = p.cot_val_puni;
                    //Obteniendo el % ajutra, segun su PU
                    for (var i = 0; i <= (p.rango_ajutra.Length / 3) - 1; i++)
                    {
                        double x_num_rango_ini = p.rango_ajutra[i, 1];
                        double x_num_rango_fin = p.rango_ajutra[i, 2];
                        if (x_num_rango_ini <= p.cot_val_puni && p.cot_val_puni < x_num_rango_fin)
                        {
                            x_val_ajutra = p.rango_ajutra[i, 0];
                            break;
                        }
                    }
                    p.cot_tas_vtra = p.cot_tas_vtra_sintra + x_val_ajutra;
                    //capitalRequerido.capital_requerido = capitalRequerido.capital_requerido * (float)p.val_moneda;
                    //if (capitalRequerido.Moneda.Id = "002" || capitalRequerido.Moneda.i)
                    //<SOLINIGTI_754>
                    //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                    int x_cot_ini_tra2 = 0;
                    double x_cot_pje_rent = 0;
                    if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                    {
                        x_cot_ini_tra2 = p.x_cot_ini_tra2;
                        x_cot_pje_rent = p.x_cot_pje_rent;
                        p.cot_num_mdif = 0;
                        p.cot_por_prrt = 0;
                    }
                    //<SOLFINGTI_754>
                    //Imprimir Log DEGUG
                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("wl_cot_kcot        : " + p.cot_gls_ikey);
                        tw.WriteLine("wl_num_solicitud   : " + p.cot_num_soli);
                        tw.WriteLine("wl_num_corr_cotiza : " + p.cot_num_coti);
                        tw.WriteLine("wl_cot_pdif        : " + p.cot_num_mdif);
                        tw.WriteLine("wl_cot_pgar        : " + p.cot_num_mgar);
                        tw.WriteLine("wl_cot_nben        : " + p.cot_num_nben);
                        tw.WriteLine("wl_cot_fcot        : " + p.cot_fec_fcal);
                        tw.WriteLine("wl_cot_fdev        : " + p.cot_fec_fdev);
                        tw.WriteLine("wl_cot_tpen        : " + p.cot_num_tpen);
                        tw.WriteLine("wl_cot_tcal        : " + p.cot_num_tcal);
                        tw.WriteLine("wl_cot_derc        : " + p.cot_flg_idac);
                        tw.WriteLine("wl_cot_grat        : " + p.cot_flg_igra);
                        tw.WriteLine("cot_mon_equi       : " + p.cot_num_cmon);
                        tw.WriteLine("cot_num_trea       : " + p.cot_num_trea);
                        tw.WriteLine("cot_num_frea       : " + p.cot_num_frea);
                        tw.WriteLine("cot_flg_irea       : " + p.cot_flg_irea);
                        tw.WriteLine("wg_val_ajuste_tasa_fija : " + p.cot_tas_vrea);
                        tw.WriteLine("wl_val_tasa_venta  : " + p.cot_tas_tasa);
                        tw.WriteLine("wl_cot_vpen        : " + p.cot_val_vpen);
                        tw.WriteLine("wl_val_vtra        : " + p.cot_tas_vtra);
                        tw.WriteLine("wl_val_tasa_afp    : " + p.cot_tas_tafp);
                        tw.WriteLine("wl_val_acom        : " + p.cot_val_acom);
                        tw.WriteLine("wl_cot_pdco        : " + p.cot_val_dcom);
                        tw.WriteLine("wl_val_puam        : " + p.cot_val_puam);
                        tw.WriteLine("wl_val_puni        : " + p.cot_val_puni);
                        tw.WriteLine("wl_cot_prrt        : " + p.cot_por_prrt);
                        tw.WriteLine("wl_val_tgfi        : " + p.cot_val_tgfi);
                        tw.WriteLine("wl_val_tope        : " + p.cot_val_tope);
                        tw.WriteLine("arr_ppu_vllx       : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx[i, j]);
                        //tw.WriteLine("arr_ppu_aju_vllx   : ");
                        //for (int i = 0; i <= 10568; i++)
                        //    for (int j = 0; j <= 3; j++)
                        //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_arr_vllx[i, j]);
                        tw.WriteLine("arr_ppu_vllx_cot   : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx_cot[i, j]);
                        tw.WriteLine("cot_xml_ben        : " + p.cot_xml_benefi);
                        tw.WriteLine("cot_xml_icob       : " + p.cot_xml_tabico);
                        tw.WriteLine("cot_xml_ash        : " + p.cot_xml_parash);
                        tw.WriteLine("cot_xml_inv        : " + p.cot_xml_parinv);
                        tw.WriteLine("cot_xml_ajm        : " + p.cot_xml_ajutdm);
                        tw.WriteLine("cot_xml_fdv        : " + p.cot_xml_fluaju);
                        tw.WriteLine("arr_ppu_fm_cot     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm[i, j]);
                        tw.WriteLine("arr_ppu_fm_sbs     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm_sbs[i, j]);
                        tw.Close();
                    }
                    // LLamado a la DLL del cotizador (rvicotmain.dll)
                    //<GTI.INI-15819>
                    string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                    ////<GTIINI-6489>
                    ////cot_main(
                    ////    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    ////    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    ////    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    ////    p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    ////    ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                    ////    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    ////    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    ////    ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    ////    ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    ////    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    ////    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    ////    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                    ////    x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //cot_main(
                    //    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    //    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    //    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    //    p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    //    ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                    //    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    //    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    //    ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    //    ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    //    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    //    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    //    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                    //    x_cot_xml_benefi, ref x_cot_num_cmsg);
                    ////<GTIFIN-6489>
                    cot_main(
                        cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                        p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                        p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                        p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                        ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                        p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                        p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                        ref p.cot_fac_dto[0],
                        ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                        ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                        ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                        ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                        ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                        x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //<GTI.FIN-15819>


                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_1.txt");
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_1.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("Salida:");
                        tw.WriteLine("");
                        tw.WriteLine("wl_cia_vtva       : " + x_cot_tas_vtva);
                        tw.WriteLine("wl_cot_dcmm       : " + x_cot_val_mdco);
                        tw.WriteLine("wl_cia_pens       : " + x_cia_val_pens);
                        tw.WriteLine("wl_cia_ppag       : " + x_cia_val_ppag);
                        tw.WriteLine("wl_cia_puni       : " + x_cia_val_puni);
                        tw.WriteLine("wl_cia_puur       : " + x_cia_val_puur);
                        tw.WriteLine("wl_afp_pens       : " + x_afp_val_pens);
                        tw.WriteLine("wl_afp_puni       : " + x_afp_val_puni);
                        tw.WriteLine("wl_afp_puur       : " + x_afp_val_puur);
                        tw.WriteLine("wl_ash_vpen       : " + x_ash_val_vpen);
                        tw.WriteLine("wl_ash_vtra       : " + x_ash_tas_vtra);
                        tw.WriteLine("wl_ash_vtce       : " + x_ash_tas_vtce);
                        tw.WriteLine("wl_ash_dura       : " + x_ash_val_dura);
                        tw.WriteLine("cot_xml_benefi    : " + x_cot_xml_benefi.ToString().Trim());
                        tw.WriteLine("wl_num_error      : " + x_cot_num_cmsg);
                        tw.Close();
                    }
                    if (x_cot_num_cmsg > 0)
                    {
                        x_cot_tas_vtva = 0; x_cot_val_mdco = 0; x_cia_val_pens = 0;
                        x_cia_val_ppag = 0; x_cia_val_puni = 0; x_cia_val_puur = 0;
                        x_afp_val_pens = 0; x_afp_val_puni = 0; x_afp_val_puur = 0;
                        x_ash_val_vpen = 0; x_ash_tas_vtce = 0; x_ash_val_dura = 0;
                    }
                    cia_puni_sin_com = x_cia_val_puni;
                    if (p.val_moneda != 0)
                    {
                        cia_pens_mo = x_cia_val_pens / p.val_moneda;
                        cia_ppag_mo = x_cia_val_ppag / p.val_moneda;
                        x_cia_val_pens = x_cia_val_pens / p.val_moneda;
                    }
                    else
                    {
                        cia_pens_mo = 0;
                        cia_ppag_mo = 0;
                    }
                    cia_pens_mo = lib_s_round(cia_pens_mo, 2, 0);
                    cia_ppag_mo = lib_s_round(cia_ppag_mo, 2, 0);
                    capitalRequerido.tasaSBS = (float)lib_s_round(x_cot_tas_vtva, 2, 0);
                    capitalRequerido.error = x_cot_num_cmsg;
                    capitalRequerido.pension_calculada = (float)x_cia_val_pens;
                    capitalRequerido.capital_requerido = (float)p.cot_val_puni;
                }
            }
            catch (Exception)
            {
                if (tw != null) tw.Close();
                throw;
            }
        }
        public void CalcularCapitalRP(List<ParametroCotizador> parametros, ref CapitalRequerido capitalRequerido)
        {
            TextWriter tw = null;
            try
            {
                // Obtener los parámetros de cotización
                //var parametros = repositorioSolicitudRP.ObtenerParametrosCotizacionRP(idSolicitud, fechaCotizacion, null);
                double x_cot_tas_vtva = 0, x_cot_val_mdco = 0, x_cia_val_pens = 0,
                        x_cia_val_ppag = 0, x_cia_val_puni = 0, x_cia_val_puur = 0,
                        x_afp_val_pens = 0, x_afp_val_puni = 0, x_afp_val_puur = 0,
                        x_ash_val_vpen = 0, x_ash_tas_vtva = 0, x_ash_tas_vtra = 0,
                        x_ash_tas_vtce = 0, x_ash_val_dura = 0, x_ash_val_comm = 0;
                StringBuilder x_cot_xml_benefi = new StringBuilder(5000);
                int x_cot_num_cmsg = 0;
                double x_val_ajutra = 0;
                double cia_pens_mo = 0, cia_ppag_mo = 0, cia_puni_sin_com = 0;
                string wl_XML_CostoBen = String.Empty, wl_XML_Cotiza = String.Empty,
                        XML_CostoBen = String.Empty, XML_Cotiza = String.Empty, XML_Pje = String.Empty;
                RviCotiza RecRviCotiza_XML = new RviCotiza();
                //wl_val_vtra
                foreach (ParametroCotizador p in parametros)
                {
                    //Pasando el capital requerido del tanteo
                    p.cot_val_puni = capitalRequerido.capital_requerido;
                    //convirtiendo a soles
                    p.cot_val_puni *= p.val_moneda;
                    //Obteniendo el % ajutra, segun su PU
                    for (var i = 0; i <= (p.rango_ajutra.Length / 3) - 1; i++)
                    {
                        double x_num_rango_ini = p.rango_ajutra[i, 1];
                        double x_num_rango_fin = p.rango_ajutra[i, 2];
                        if (x_num_rango_ini <= p.cot_val_puni && p.cot_val_puni < x_num_rango_fin)
                        {
                            x_val_ajutra = p.rango_ajutra[i, 0];
                            break;
                        }
                    }
                    p.cot_tas_vtra = p.cot_tas_vtra_sintra + x_val_ajutra;
                    // Convertir el valor de la Prima Única a soles
                    //p.cot_val_puam *= p.val_moneda;
                    //p.cot_val_puam *= p.val_moneda;
                    //<SOLINIGTI_754>
                    //Para que no entre a la condicion indicada por Fredy (en el cot_main) (9999)
                    int x_cot_ini_tra2 = 0;
                    double x_cot_pje_rent = 0;
                    if (p.ind_modalidad == Enums.Modalidad.Escalonada.StringValue())
                    {
                        x_cot_ini_tra2 = p.x_cot_ini_tra2;
                        x_cot_pje_rent = p.x_cot_pje_rent;
                        p.cot_num_mdif = 0;
                        p.cot_por_prrt = 0;
                    }
                    //<SOLFINGTI_754>
                    //Imprimir Log DEGUG
                    if (ConfigurationManager.AppSettings["DebugDLL"] == "S")
                    {
                        //<SOLINIGTI_754>
                        //tw = new StreamWriter("C:\\temp\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        if (!System.IO.Directory.Exists("C:\\temp\\" + p.cot_num_soli)) { System.IO.Directory.CreateDirectory("C:\\temp\\" + p.cot_num_soli); }
                        tw = new StreamWriter("C:\\temp\\" + p.cot_num_soli + "\\" + p.cot_gls_ikey + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + "_0.txt");
                        //<SOLFINGTI_754>
                        tw.WriteLine("wl_cot_kcot        : " + p.cot_gls_ikey);
                        tw.WriteLine("wl_num_solicitud   : " + p.cot_num_soli);
                        tw.WriteLine("wl_num_corr_cotiza : " + p.cot_num_coti);
                        tw.WriteLine("wl_cot_pdif        : " + p.cot_num_mdif);
                        tw.WriteLine("wl_cot_pgar        : " + p.cot_num_mgar);
                        tw.WriteLine("wl_cot_nben        : " + p.cot_num_nben);
                        tw.WriteLine("wl_cot_fcot        : " + p.cot_fec_fcal);
                        tw.WriteLine("wl_cot_fdev        : " + p.cot_fec_fdev);
                        tw.WriteLine("wl_cot_tpen        : " + p.cot_num_tpen);
                        tw.WriteLine("wl_cot_tcal        : " + p.cot_num_tcal);
                        tw.WriteLine("wl_cot_derc        : " + p.cot_flg_idac);
                        tw.WriteLine("wl_cot_grat        : " + p.cot_flg_igra);
                        tw.WriteLine("cot_mon_equi       : " + p.cot_num_cmon);
                        tw.WriteLine("cot_num_trea       : " + p.cot_num_trea);
                        tw.WriteLine("cot_num_frea       : " + p.cot_num_frea);
                        tw.WriteLine("cot_flg_irea       : " + p.cot_flg_irea);
                        tw.WriteLine("wg_val_ajuste_tasa_fija : " + p.cot_tas_vrea);
                        tw.WriteLine("wl_val_tasa_venta  : " + p.cot_tas_tasa);
                        tw.WriteLine("wl_cot_vpen        : " + p.cot_val_vpen);
                        tw.WriteLine("wl_val_vtra        : " + p.cot_tas_vtra);
                        tw.WriteLine("wl_val_tasa_afp    : " + p.cot_tas_tafp);
                        tw.WriteLine("wl_val_acom        : " + p.cot_val_acom);
                        tw.WriteLine("wl_cot_pdco        : " + p.cot_val_dcom);
                        tw.WriteLine("wl_val_puam        : " + p.cot_val_puam);
                        tw.WriteLine("wl_val_puni        : " + p.cot_val_puni);
                        tw.WriteLine("wl_cot_prrt        : " + p.cot_por_prrt);
                        tw.WriteLine("wl_val_tgfi        : " + p.cot_val_tgfi);
                        tw.WriteLine("wl_val_tope        : " + p.cot_val_tope);
                        tw.WriteLine("arr_ppu_vllx       : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx[i, j]);
                        //tw.WriteLine("arr_ppu_aju_vllx   : ");
                        //for (int i = 0; i <= 10568; i++)
                        //    for (int j = 0; j <= 3; j++)
                        //        tw.WriteLine("        " + i + " " + j + ":" + p.ppu_arr_vllx[i, j]);
                        tw.WriteLine("arr_ppu_vllx_cot   : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_vllx_cot[i, j]);
                        tw.WriteLine("cot_xml_ben        : " + p.cot_xml_benefi);
                        tw.WriteLine("cot_xml_icob       : " + p.cot_xml_tabico);
                        tw.WriteLine("cot_xml_ash        : " + p.cot_xml_parash);
                        tw.WriteLine("cot_xml_inv        : " + p.cot_xml_parinv);
                        tw.WriteLine("cot_xml_ajm        : " + p.cot_xml_ajutdm);
                        tw.WriteLine("cot_xml_fdv        : " + p.cot_xml_fluaju);
                        tw.WriteLine("arr_ppu_fm_cot     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_cot : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm[i, j]);
                        tw.WriteLine("arr_ppu_fm_sbs     : ");
                        for (int i = 0; i <= 1320; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_fmqx_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_tm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_tm_sbs[i, j]);
                        tw.WriteLine("arr_ppu_inf_fm_sbs : ");
                        for (int i = 0; i <= 2; i++)
                            for (int j = 0; j <= 7; j++)
                                tw.WriteLine("        " + i + " " + j + ":" + p.ppu_inf_fm_sbs[i, j]);
                        tw.Close();
                    }
                    // LLamado a la DLL del cotizador (rvicotmain.dll)
                    //<GTI.INI-15819>
                    string cot_gls_skey = p.cot_gls_ikey + '|' + p.cot_num_soli;
                    ////<GTIINI-6489>
                    ////cot_main(
                    ////    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    ////    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    ////    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    ////    p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    ////    ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                    ////    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    ////    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    ////    ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    ////    ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    ////    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    ////    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    ////    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura,
                    ////    x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //cot_main(
                    //    p.cot_gls_ikey, p.cot_num_soli, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                    //    p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                    //    p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                    //    p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                    //    ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                    //    p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                    //    p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                    //    ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                    //    ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                    //    ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                    //    ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                    //    ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                    //    x_cot_xml_benefi, ref x_cot_num_cmsg);
                    ////<GTIFIN-6489>
                    cot_main(
                        cot_gls_skey, p.cot_num_coti, p.cot_num_mdif, p.cot_num_mgar, p.cot_num_nben, p.cot_fec_fcal,
                        p.cot_fec_fdev, p.cot_num_tpen, p.cot_num_tcal, p.cot_flg_idac, p.cot_flg_igra, p.cot_num_cmon, p.cot_num_trea,
                        p.cot_num_frea, p.cot_flg_irea, p.cot_tas_vrea, p.cot_tas_tasa, p.cot_val_vpen, p.cot_tas_vtra, p.cot_tas_tafp,
                        p.cot_val_acom, p.cot_val_dcom, p.cot_val_puam, p.cot_val_puni, p.cot_por_prrt, p.cot_val_tgfi, p.cot_val_tope,
                        ref p.ppu_vllx[0, 0], ref p.ppu_vllx_cot[0, 0],
                        p.cot_xml_benefi, p.cot_xml_tabico, p.cot_xml_parash,
                        p.cot_xml_parinv, p.cot_xml_ajutdm, p.cot_xml_fluaju,
                        ref p.cot_fac_dto[0],
                        ref p.ppu_fmqx[0, 0], ref p.ppu_inf_tm[0, 0], ref p.ppu_inf_fm[0, 0],
                        ref p.ppu_fmqx_sbs[0, 0], ref p.ppu_inf_tm_sbs[0, 0], ref p.ppu_inf_fm_sbs[0, 0],
                        ref x_cot_tas_vtva, ref x_cot_val_mdco, ref x_cia_val_pens, ref x_cia_val_ppag, ref x_cia_val_puni,
                        ref x_cia_val_puur, ref x_afp_val_pens, ref x_afp_val_puni, ref x_afp_val_puur, ref x_ash_val_vpen,
                        ref x_ash_tas_vtva, ref x_ash_tas_vtra, ref x_ash_tas_vtce, ref x_ash_val_dura, ref x_ash_val_comm,
                        x_cot_xml_benefi, ref x_cot_num_cmsg);
                    //<GTI.FIN-15819>

                    if (x_cot_num_cmsg > 0)
                    {
                        x_cot_tas_vtva = 0; x_cot_val_mdco = 0; x_cia_val_pens = 0;
                        x_cia_val_ppag = 0; x_cia_val_puni = 0; x_cia_val_puur = 0;
                        x_afp_val_pens = 0; x_afp_val_puni = 0; x_afp_val_puur = 0;
                        x_ash_val_vpen = 0; x_ash_tas_vtce = 0; x_ash_val_dura = 0;
                    }
                    cia_puni_sin_com = x_cia_val_puni;
                    //val_tasa_cesion = 0;
                    if (p.val_moneda != 0)
                    {
                        cia_pens_mo = x_cia_val_pens / p.val_moneda;
                        cia_ppag_mo = x_cia_val_ppag / p.val_moneda;
                        p.cot_val_puni /= p.val_moneda;
                    }
                    else
                    {
                        cia_pens_mo = 0;
                        cia_ppag_mo = 0;
                        p.cot_val_puni = 0;
                    }
                    cia_pens_mo = lib_s_round(cia_pens_mo, 2, 0);
                    cia_ppag_mo = lib_s_round(cia_ppag_mo, 2, 0);
                    capitalRequerido.tasaSBS = (float)lib_s_round(x_cot_tas_vtva, 2, 0);
                    capitalRequerido.error = x_cot_num_cmsg;
                    capitalRequerido.pension_calculada = (float)cia_pens_mo;
                    capitalRequerido.capital_requerido = (float)p.cot_val_puni;
                }
            }
            catch (Exception ex)
            {
                if (tw != null) tw.Close();
                throw;
            }
        }
        //<SOLFIN25621>
        #endregion
        //<INIGTI_1092>
        public string SolicitudesHabilitadas(int lote, string solicitudes)
        {
            var nroSolicitud = repositorioSolicitud.SolicitudesHabilitadas(lote, solicitudes);
            return nroSolicitud;
        }
        //<FINGTI_1092>
        //<INIGTI_753>
        #region RentaPrivadaPlus
        public List<SolicitudRPPlus> ListarSolicitudRPPlus(string cuspp)
        {
            var solicitud = repositorioSolicitudRPPlus.Listar(cuspp);
            return solicitud;
        }
        public void RegistrarSolicitudRPPlus(ref SolicitudRPPlus solicitud)
        {
            repositorioSolicitudRPPlus.Registrar(ref solicitud);
            CotizarRPPlus(solicitud.Id, (DateTime)solicitud.FechaCotizacion, solicitud.Usuario.NombreUsuario);
        }
        public void ActualizarSolicitudRPPlus(ref SolicitudRPPlus solicitud)
        {
            repositorioSolicitudRPPlus.Actualizar(ref solicitud);
            CotizarRPPlus(solicitud.Id, (DateTime)solicitud.FechaCotizacion, solicitud.Usuario.NombreUsuario);
        }
        public SolicitudRPPlus ObtenerDatosSolicitudRPPlus(string idSolicitud)
        {
            var solicitudRPPlus = repositorioSolicitudRPPlus.ObtenerDatos(idSolicitud);
            return solicitudRPPlus;
        }
        #endregion
        //<FINGTI_753>
        //<INIGTI_6556>
        public List<SolicitudEscenario> ListarSolicitudesPendientesEmail(string cod_rol)
        {
            List<SolicitudEscenario> solicitudEscenario = repositorioSolicitudEscenario.ListarSolicitudesPendientesEmail(cod_rol);
            return solicitudEscenario;
        }
        //<FINGTI_6556>

        public void VistaPreviaCotizacionPlus(string num_solicitud, int num_correlativo, string usuario)
        {
                string cod_tipo_cotizacion = "";
                repositorioSolicitudRPPlus.VistaPreviaCotizacion(num_solicitud, num_correlativo, usuario, ref cod_tipo_cotizacion);
        }

        //<INIGTI_7012>
        public void CerrarCotizacionPlus(string num_solicitud, int num_correlativo, string usuario, List<GrupoFamiliar> lstGrupoFamiliar)
        {
                string cod_tipo_cotizacion = "";
                repositorioSolicitudRPPlus.CerrarCotizacion(num_solicitud, num_correlativo, usuario, ref cod_tipo_cotizacion);
                //<INI.GTI_7012_26>
                /*Registrando Flujo Seleccione*/
                FlujoEvaluacion flujoEvaluacion = new FlujoEvaluacion();
                flujoEvaluacion.cod_tipo_flujo_evaluacion = Convert.ToInt32(Enums.TipoFlujoEvaluacion.Seleccion.StringValue());
                flujoEvaluacion.fec_inicio_flujo_evaluacion = DateTime.Now;
                flujoEvaluacion.gls_observacion = "";
                flujoEvaluacion.num_correlativo = 0;
                flujoEvaluacion.num_solicitud = num_solicitud;
                flujoEvaluacion.fec_inicio_flujo_evaluacion = DateTime.Now;
                flujoEvaluacion.gls_archivos_existentes = "";
                flujoEvaluacion.aud_fec_ingreso = DateTime.Now;
                flujoEvaluacion.aud_usr_ingreso = usuario;
                repositorioSolicitudRPPlus.RegistrarFlujoEvaluacion(flujoEvaluacion);
                //<FIN.GTI_7012_26>
                if (lstGrupoFamiliar != null)
                {
                    if (lstGrupoFamiliar.Count > 0)
                    {
                        int cantidadBeneficiarios = 2;
                        lstGrupoFamiliar.ForEach(p => p.SolicitudIFP = new SolicitudIFP { Id = num_solicitud });
                        RegistrarBeneficiarios(lstGrupoFamiliar, cantidadBeneficiarios, null);
                    }
                }
        }

        public string GenerarPolizaPlus(string num_solicitud, int num_correlativo, string usuario, GrupoFamiliar grup_fam, ref string mensaje)
        {
            string numPoliza = "";
            int digito = 0;
            string digito_char = "";
            int lenNumPoliza = 0;
            string Arreglo = "0123456789K";
            string cod_tipo_cotizacion = "";
                repositorioSolicitudRPPlus.CerrarCotizacion(num_solicitud, num_correlativo, usuario, ref cod_tipo_cotizacion);
                numPoliza = repositorioSolicitudRPPlus.ObtenerSecuenciaPolizaPlus();
                lenNumPoliza = numPoliza.Length;
                //     select @wl_digito = 11 - (
                //convert(int,SUBSTRING(@wl_num_poliza_char, LEN(@wl_num_poliza_char),1)) * 2 +
                //convert(int,SUBSTRING(@wl_num_poliza_char, LEN(@wl_num_poliza_char)-1,1)) * 3 +
                //convert(int,SUBSTRING(@wl_num_poliza_char, LEN(@wl_num_poliza_char)-2,1)) * 5 +
                //convert(int,SUBSTRING(@wl_num_poliza_char, LEN(@wl_num_poliza_char)-3,1)) * 7 +
                //convert(int,SUBSTRING(@wl_num_poliza_char, LEN(@wl_num_poliza_char)-4,1)) * 8 +
                //convert(int,SUBSTRING(@wl_num_poliza_char, LEN(@wl_num_poliza_char)-5,1)) * 9 
                //) % 11
                //      If @wl_digito = 0 
                //          set @wl_digito = 11
                //      else 
                //          set @wl_digito_char = SUBSTRING(@wc_Arreglo, @wl_digito, 1)
                digito = 11 - (Convert.ToInt32(numPoliza.Substring(lenNumPoliza - 1, 1)) * 2 +
                    Convert.ToInt32(numPoliza.Substring(lenNumPoliza - 2, 1)) * 3 +
                    Convert.ToInt32(numPoliza.Substring(lenNumPoliza - 3, 1)) * 5 +
                    Convert.ToInt32(numPoliza.Substring(lenNumPoliza - 4, 1)) * 7 +
                    Convert.ToInt32(numPoliza.Substring(lenNumPoliza - 5, 1)) * 8 +
                    Convert.ToInt32(numPoliza.Substring(lenNumPoliza - 6, 1)) * 9) % 11;
                if (digito == 0)
                    digito = 11;
                else
                    digito_char = Arreglo.Substring(digito - 1, 1);
                repositorioSolicitudRPPlus.GenerarPolizaPlus(num_solicitud, num_correlativo, Convert.ToInt64(numPoliza), digito_char, usuario);
                //<INI.GTI_7012_26>
                /*Registrando Flujo Cerrado*/
                FlujoEvaluacion flujoEvaluacion = new FlujoEvaluacion();
                flujoEvaluacion.cod_tipo_flujo_evaluacion = Convert.ToInt32(Enums.TipoFlujoEvaluacion.Cerrado.StringValue());
                flujoEvaluacion.fec_inicio_flujo_evaluacion = DateTime.Now;
                flujoEvaluacion.gls_observacion = "";
                flujoEvaluacion.num_correlativo = 0;
                flujoEvaluacion.num_solicitud = num_solicitud;
                flujoEvaluacion.fec_inicio_flujo_evaluacion = DateTime.Now;
                flujoEvaluacion.gls_archivos_existentes = "";
                flujoEvaluacion.aud_fec_ingreso = DateTime.Now;
                flujoEvaluacion.aud_usr_ingreso = usuario;
                repositorioSolicitudRPPlus.RegistrarFlujoEvaluacion(flujoEvaluacion);
                //<FIN.GTI_7012_26>
                if (ConfigurationManager.AppSettings["IndTransaccionPago"] == "N")
                {
                }
                else
                {
                    //<INI.GTI_7012_ADMWR>
                    if (cod_tipo_cotizacion == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                    {
                        if (RegistrarPolizaADMWR(num_solicitud, usuario, grup_fam) != 0)
                        {
                            repositorioSolicitudRPPlus.ActualizarEnvioADMWR(numPoliza, usuario);
                        }
                        else
                        {
                            throw new Exception("No se completo el envío de póliza a ADMWR");
                        }
                    }
                    else
                    {
                        if (RegistrarPolizaIFPADMWR(num_solicitud, usuario, grup_fam) != 0)
                        {
                            repositorioSolicitudRPPlus.ActualizarEnvioADMWR(numPoliza, usuario);
                        }
                        else
                        {
                            throw new Exception("No se completo el envío de póliza a ADMWR");
                        }
                    }
                }
                //<FIN.GTI_7012_ADMWR>
            
            if (ConfigurationManager.AppSettings["IndTransaccionPago"] == "N")
            {
                if (cod_tipo_cotizacion == Enums.TipoCotizacion.RentaPrivadaPlus.StringValue())
                {
                    if (RegistrarPolizaADMWR(num_solicitud, usuario, grup_fam) != 0)
                    {
                        //Actualizar Estado Poliza
                        repositorioSolicitudRPPlus.ActualizarEnvioADMWR(numPoliza, usuario);
                    }
                    else
                    {
                        mensaje = "Póliza Generada Correctamente. Falta enviar a la aplicación ADMWR.";
                    }
                }
                else
                {
                    if (RegistrarPolizaIFPADMWR(num_solicitud, usuario, grup_fam) != 0)
                    {
                        //Actualizar Estado Poliza
                        repositorioSolicitudRPPlus.ActualizarEnvioADMWR(numPoliza, usuario);
                    }
                    else
                    {
                        mensaje = "Póliza Generada Correctamente. Falta enviar a la aplicación ADMWR.";
                    }
                }
            }
            return numPoliza.ToString();
        }

        public void AnularSolicitudPlus(string num_solicitud, string usuario, string cod_causante)
        {

            repositorioSolicitudRPPlus.AnularSolicitud(num_solicitud, usuario, cod_causante);

        }
        public List<CausalPoliza> ListarCausalPolizaPlus()
        {
            List<CausalPoliza> lstCausalPoliza = repositorioCausalPoliza.ListarCausalPolizaPlus();
            return lstCausalPoliza;
        }
        //<FINGTI_7012>
        //<INIGTI_7012>
        public List<DatosSol> ObtenerDatosporSolicitud(string num_Solicitud)
        {
            List<DatosSol> lstDatosSolicitud = repositorioSolicitud.ObtenerDatosporSolicitud(num_Solicitud);
            return lstDatosSolicitud;
        }
        //<FINGTI_7012>
        //<INIGTI_7012>
        public List<Temporal> ListarGruposFamiliaresxSolicitud(string num_solicitud)
        {
            var lstSolicitud = repositorioSolicitud.ListarGruposFamiliaresxSolicitud(num_solicitud);
            return lstSolicitud;
        }
        //<FINGTI_7012>
        //<INIGTI_7012>
        //public List<ReporteCotizacion> ListarEtiquetas()
        //{
        //    var reportecotizacion = repositoriorepo.ObtenerDatos();
        //    return afiliado;
        //}
        //<FINGTI_7012>
        //<INI.GTI_7012_2>
        public List<Agente> ObtenerAgenteDeudaAcom(string idAgente)
        {
            List<Agente> lstAgente = repositorioAgente.ObtenerAgenteDeudaAcom(idAgente);
            return lstAgente;
        }
        //<FIN.GTI_7012_2>
        //<INI.GTI_7012_3>
        public EmisionPoliza EmitirPoliza(string num_solicitud, int num_poliza, string dig_poliza)
        {
            EmisionPoliza emisionPoliza = repositorioEmisionPoliza.EmitirPoliza(num_solicitud, num_poliza, dig_poliza);
            return emisionPoliza;
        }
        //<FIN.GTI_7012_3>
        //<INI.GTI_7012_2_1>
        public List<SolicitudRPPlus> ListarReporteCotizacionPlus(string cuspp)
        {
            List<SolicitudRPPlus> lstSolicitud = repositorioSolicitudRPPlus.ListarReporte(cuspp);
            return lstSolicitud;
        }
        //<FIN.GTI_7012_2_1>
        //<INI.GTI_7012_ADMWR>
        public Int64 RegistrarPolizaADMWR(string num_solicitud, string usuario, GrupoFamiliar grup_fam)
        {
            EmisionPoliza emisionPoliza = repositorioEmisionPoliza.EmitirPoliza(num_solicitud, 0, "");
            List<Direccion> Direcciones = repositorioDireccion.Listar(num_solicitud, null, usuario).FindAll(dp => dp.Principal && dp.Vigencia);
            Proxies.ModuloADMWR.IServiceADMWR moduloADMWR = new Proxies.ModuloADMWR.ServiceADMWRClient("EPADMWR");
            Proxies.ModuloADMWR.Poliza poliza = new Proxies.ModuloADMWR.Poliza();
            poliza.gls_poliza = emisionPoliza.Poliza.NumPoliza.ToString();
            poliza.gls_dig_poliza = emisionPoliza.Poliza.DigPoliza;
            poliza.gls_solicitud = emisionPoliza.SolicitudRPPlus.Id;
            if (emisionPoliza.SolicitudRPPlus.FechaSolicitud != null)
                poliza.fec_solicitud = emisionPoliza.SolicitudRPPlus.FechaSolicitud.Value;
            if (emisionPoliza.SolicitudRPPlus.FechaDevengue != null)
                poliza.fec_devengue = emisionPoliza.SolicitudRPPlus.FechaDevengue.Value;
            poliza.Modalidad = new Proxies.ModuloADMWR.Modalidad { cod_modalidad = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Modalidad.Id, ind_modalidad = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Modalidad.Nombre };
            poliza.ind_fallecimiento = false;
            poliza.EstadoPoliza = new Proxies.ModuloADMWR.EstadoPoliza { cod_estado_poliza = emisionPoliza.SolicitudRPPlus.CodigoEstadoPoliza };
            //poliza.EstadoCausal = new Proxies.ModuloADMWR.EstadoCausal { cod_causal_estado = emisionPoliza.SolicitudRPPlus.CausalPoliza.Id };
            poliza.EstadoCausal = new Proxies.ModuloADMWR.EstadoCausal { cod_causal_estado = "01" };
            if (emisionPoliza.Poliza.FecEmision != null)
                poliza.fec_emision_poliza = emisionPoliza.Poliza.FecEmision;
            if (emisionPoliza.Poliza.FecInicioVigencia != null)
                poliza.fec_inicio_vigencia = emisionPoliza.Poliza.FecInicioVigencia;
            if (emisionPoliza.Poliza.FecFinVigencia != null)
                poliza.fec_fin_vigencia = emisionPoliza.Poliza.FecFinVigencia;
            if (emisionPoliza.Poliza.FecPago != null)
                poliza.fec_inicio_pago = emisionPoliza.Poliza.FecPago;
            poliza.num_cotizacion = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Correlativo;
            poliza.MonedaCotizacion = new Proxies.ModuloADMWR.Moneda { cod_moneda = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Moneda.Id };
            poliza.val_prima_unica = emisionPoliza.Poliza.ValPrimaBruta;
            poliza.Agente = new Proxies.ModuloADMWR.Agente { num_agente = Convert.ToInt32(emisionPoliza.SolicitudRPPlus.Agente.Id) };
            poliza.num_meses_temporalidad = emisionPoliza.SolicitudRPPlus.Temporalidad.Anhos * 12;
            poliza.num_meses_garantizados = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PeriodoGarantizado * 12;
            poliza.num_meses_primer_tramo = Convert.ToInt32(emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PagoEscalonada * 12);
            poliza.val_pje_renta_segundo_tramo = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PjePE;
            poliza.val_pje_conyuge = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValPjeConyuge;
            poliza.MonedaPrima = new Proxies.ModuloADMWR.Moneda { cod_moneda = emisionPoliza.SolicitudRPPlus.MonedaPrimaUnica.Id };
            poliza.TipoProducto = new Proxies.ModuloADMWR.TipoProducto { cod_tipo_producto = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Producto.Id }; // "01";
            poliza.TipoProducto = new Proxies.ModuloADMWR.TipoProducto { cod_tipo_producto = Enums.TipoCotizacion.RentaPrivadaPlus.StringValue() };
            poliza.TipoPlan = new Proxies.ModuloADMWR.TipoPlan { cod_tipo_plan = emisionPoliza.SolicitudRPPlus.TipoPlan.Id };
            //poliza.TipoCotizacion = new Proxies.ModuloADMWR.TipoCotizacion { cod_tipo_cotizacion = Enums.TipoCotizacion.RentaPrivadaPlus.StringValue() };
            poliza.PlanRP = new Proxies.ModuloADMWR.PlanRP { cod_plan = Enums.TipoCotizacion.RentaPrivadaPlus.StringValue() };
            poliza.MonedaPago = new Proxies.ModuloADMWR.Moneda { cod_moneda = poliza.MonedaCotizacion.cod_moneda };
            poliza.val_pje_ajuste_moneda = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValMonAju;
            poliza.val_renta_original = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PensionCiaMO;
            poliza.val_renta_base = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PensionCiaMO;
            poliza.val_renta_base_anterior = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].PensionCiaMO;
            poliza.val_pje_devolucion = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValPjeDev;
            poliza.ind_sepelio = (emisionPoliza.SolicitudRPPlus.Cotizaciones[0].IndGastoSepelio == "S" ? true : false);
            poliza.val_tasa_tra = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].TasaRetornoAccionista;
            poliza.val_tasa_venta = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].TasaVenta;
            poliza.val_tasa_venta_is = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].TasaVentaSbs;
            if (emisionPoliza.SolicitudRPPlus.PorcentajeDescuentoComision != null)
                poliza.val_dcom = emisionPoliza.SolicitudRPPlus.PorcentajeDescuentoComision.Value;
            poliza.aud_usr_ingreso = usuario;
            //<INI.GTI_7012_22>
            poliza.val_total_cic = emisionPoliza.SolicitudRPPlus.Afiliado.SaldoCIC;
            poliza.val_tasa_costo_equiv = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].ValTasaCostoEquiv;
            //<FIN.GTI_7012_22>
            //poliza.fec_envio_poliza = null;
            poliza.TipoPago = new Proxies.ModuloADMWR.TipoPago { cod_tipo_pago = "T" };
            poliza.num_meses_ajuste = emisionPoliza.Poliza.val_meses_periodo;
            poliza.ind_rescate = false;

            poliza.val_res_pension = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Val_prima_unica_pension;
            poliza.val_res_sepelio = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Val_prima_unica_sepelio;
            poliza.val_res_devolucion = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Val_prima_unica_devolucion;
            poliza.val_res_fallecimiento = emisionPoliza.SolicitudRPPlus.Cotizaciones[0].Val_prima_unica_fallecimiento;

            List<Proxies.ModuloADMWR.Beneficiario> beneficiarios = new List<Proxies.ModuloADMWR.Beneficiario>();
            foreach (var beneficiario in emisionPoliza.SolicitudRPPlus.Beneficiarios)
            {

                Proxies.ModuloADMWR.Beneficiario ben = new Proxies.ModuloADMWR.Beneficiario();
                Proxies.ModuloADMWR.Parentesco par = new Proxies.ModuloADMWR.Parentesco();
                Proxies.ModuloADMWR.Banco ban = new Proxies.ModuloADMWR.Banco();
                Proxies.ModuloADMWR.TipoCuentaBanco tipctabnc = new Proxies.ModuloADMWR.TipoCuentaBanco();
                Proxies.ModuloADMWR.TipoIdentificacion tipide = new Proxies.ModuloADMWR.TipoIdentificacion();
                Proxies.ModuloADMWR.Sexo sexo = new Proxies.ModuloADMWR.Sexo();
                par.cod_parentesco = beneficiario.Parentesco.Id;
                ben.Parentesco = par;
                ben.fec_inicio_vigencia = poliza.fec_inicio_vigencia;
                ben.fec_fin_vigencia = poliza.fec_fin_vigencia;
                if (ben.Parentesco.cod_parentesco == Enums.Parentesco.Afiliado.StringValue())
                {
                    //ben.EstadoBeneficiario = new Proxies.ModuloADMWR.EstadoBeneficiario { cod_estado_beneficiario = "ACTI" };
                    //ben.EstadoCausalBeneficiario = new Proxies.ModuloADMWR.EstadoCausalBeneficiario { cod_estado_causal = "ACTIVO" };
                    ben.EstadoBeneficiario = new Proxies.ModuloADMWR.EstadoBeneficiario { cod_estado_beneficiario = "SUSP" };
                    ben.EstadoCausalBeneficiario = new Proxies.ModuloADMWR.EstadoCausalBeneficiario { cod_estado_causal = "PENACTI" };
                }
                else
                {
                    ben.EstadoBeneficiario = new Proxies.ModuloADMWR.EstadoBeneficiario { cod_estado_beneficiario = "SUSP" };
                    ben.EstadoCausalBeneficiario = new Proxies.ModuloADMWR.EstadoCausalBeneficiario { cod_estado_causal = "PENACTI" };
                }
                ben.fec_pagar_desde = poliza.fec_inicio_pago;
                //ben.fec_suspension_pago = null;
                ben.val_pje_renta = beneficiario.ValPjeRenta * 100;
                ben.ind_invalidez = beneficiario.Invalido;
                ben.TipoInvalidez = new Proxies.ModuloADMWR.TipoInvalidez { cod_tipo_invalidez = beneficiario.TipoInvalidez.Id };
                ben.fec_invalidez = beneficiario.FechaInvalidez;
                ben.val_pje_adicional = 0;
                ban.cod_banco = beneficiario.Banco.Id;
                ben.Banco = ban;
                tipctabnc.cod_tipo_cta_banco = beneficiario.TipoCtaBanco.Id;
                ben.TipoCuentaBanco = tipctabnc;
                ben.gls_cta_bancaria = beneficiario.NumeroBanco;
                //ben.cod_persona_apoderado = null;
                //ben.val_pje_adicional = null;
                ben.ind_confidencialidad_datos = beneficiario.Confidencialidaddatos.Id == "0" ? true : false;
                if (ben.Parentesco.cod_parentesco == Enums.Parentesco.Afiliado.StringValue())
                {
                    ben.gls_correo_electronico = grup_fam.CorreoElectronico;
                    ben.gls_telefono = grup_fam.Telefono1;
                    ben.gls_celular = grup_fam.Telefono2;
                }
                else
                {
                    ben.gls_correo_electronico = "";
                    ben.gls_telefono = "";
                    ben.gls_celular = "";
                }
                //ben.fec_vigencia_supervivencia = null;
                ben.ind_pep = beneficiario.ind_PEP;
                ben.ind_sujeto_obligado = beneficiario.ind_SujetoObligado;
                ben.num_item = beneficiario.num_item;
                ben.MedioComunicacion = new Proxies.ModuloADMWR.MedioComunicacion { cod_medio_comunicacion = Convert.ToInt32(emisionPoliza.SolicitudRPPlus.Beneficiarios[0].Comunicacion.Id) };
                ben.Persona = new Proxies.ModuloADMWR.Persona();
                tipide.cod_tipo_identificacion = beneficiario.Identificacion.IdTipo;
                ben.Persona.TipoIdentificacion = tipide;
                ben.Persona.gls_nro_identificacion = beneficiario.Identificacion.Numero.ToString();
                ben.Persona.gls_ape_paterno = beneficiario.ApellidoPaterno;
                ben.Persona.gls_ape_materno = beneficiario.ApellidoMaterno;
                ben.Persona.gls_nom_persona = beneficiario.Nombre;
                ben.Persona.gls_persona = beneficiario.ApellidosNombres;
                ben.Persona.fec_nacimiento = beneficiario.FechaNacimiento;
                if (ben.Parentesco.cod_parentesco == Enums.Parentesco.Afiliado.StringValue())
                {
                    ben.Persona.EstadoCivil = new Proxies.ModuloADMWR.EstadoCivil { cod_estado_civil = grup_fam.estadoCivil };
                }
                else
                {
                    ben.Persona.EstadoCivil = new Proxies.ModuloADMWR.EstadoCivil { cod_estado_civil = "" };
                }
                ben.Persona.Nacionalidad = new Proxies.ModuloADMWR.Nacionalidad { cod_nacionalidad = beneficiario.Nacionalidad.cod_parametro };
                ben.Persona.Profesion = new Proxies.ModuloADMWR.Profesion { cod_profesion = beneficiario.Profesion.cod_parametro };

                ben.cod_tipo_periodo_beneficiario = beneficiario.IdTipoPeriodoBeneficiario;

                if (ben.Parentesco.cod_parentesco != Enums.Parentesco.Afiliado.StringValue())
                {
                    ben.cod_tipo_periodo_beneficiario = Convert.ToInt32(Enums.TipoPeriodoBeneficiario.BeneficiariosRPP.StringValue());
                }

                sexo.cod_sexo = beneficiario.Sexo.ToString();
                //ben.Persona.fec_fallecimiento = null;
                ben.Persona.Sexo = sexo;
                List<Proxies.ModuloADMWR.Direccion> direcciones = new List<Proxies.ModuloADMWR.Direccion>();
                foreach (var direccion in Direcciones)
                {
                    Proxies.ModuloADMWR.Direccion dir = new Proxies.ModuloADMWR.Direccion();
                    Proxies.ModuloADMWR.Distrito dis = new Proxies.ModuloADMWR.Distrito();
                    Proxies.ModuloADMWR.Provincia pro = new Proxies.ModuloADMWR.Provincia();
                    Proxies.ModuloADMWR.Departamento dep = new Proxies.ModuloADMWR.Departamento();
                    //<INI.GTI_7012_12>
                    Proxies.ModuloADMWR.TipoVia via = new Proxies.ModuloADMWR.TipoVia();
                    //<INI.GTI_7012_12>
                    dir.gls_direccion = direccion.Glosa;
                    //<INI.GTI_7012_12>
                    //dir.cod_tipo_via = direccion.TipoVia.Id;
                    via.cod_tipo_via = direccion.TipoVia.Id;
                    dir.TipoVia = via;
                    //<FIN.GTI_7012_12>
                    dir.cod_espacio_urbano = direccion.EspacioUrbano;
                    dis.cod_distrito = direccion.Comuna.Id;
                    dir.Distrito = dis;
                    pro.cod_provincia = direccion.Ciudad.Id;
                    dir.Provincia = pro;
                    dep.cod_departamento = direccion.Departamento.Id;
                    dir.Departamento = dep;
                    dir.ind_principal = direccion.Principal;
                    dir.ind_vigencia = true;
                    if (ben.Parentesco.cod_parentesco == Enums.Parentesco.Afiliado.StringValue())
                    {
                        direcciones.Add(dir);
                    }
                }
                ben.Direcciones = direcciones;
                //beneficiarios.Add(ben);
                if (ben.val_pje_renta > 0)
                {
                    beneficiarios.Add(ben);
                }
            }
            poliza.Beneficiarios = beneficiarios;
            Proxies.ModuloADMWR.Poliza RptaPoliza = new Proxies.ModuloADMWR.Poliza();
            RptaPoliza = moduloADMWR.RegistrarPoliza(poliza);
            return Convert.ToInt64(RptaPoliza.cod_poliza);
        }
        //<FIN.GTI_7012_ADMWR>
        public Respuesta ActualizacionSolicitudPlusPlaft(string num_solicitud, string cod_tipo_flujo_evaluacion, int cod_estado, string gls_observacion, string gls_archivos_existentes, string usuario)
        {

            Respuesta respuesta = new Respuesta();

            try
            {

                repositorioSolicitudRPPlus.ActualizacionSolicitudPlusPlaft(num_solicitud, cod_tipo_flujo_evaluacion, cod_estado, gls_observacion, gls_archivos_existentes, usuario);
                respuesta.Estado = Constante.COD_OK;
                respuesta.Mensaje = "Solicitud Actualizada";
            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
            }
            return respuesta;
        }
        //<INI.GTI_7012_26>
        public Respuesta EnviarNotificacion(string rutaServicio, Notificacion notificacion)
        {
            //Envio de manera Asincrono
            Object semilla = new object();
            lock (semilla)
            {
                Respuesta respuesta = new Respuesta();
                try
                {
                    List<string> errores = new List<string>();
                    if (notificacion == null)
                    {
                        errores.Add("Envíe una notificación completa. Dato Obligatorio");
                    }
                    else
                    {
                        if (notificacion.p_remitente == null || notificacion.p_remitente == "")
                        {
                            errores.Add("Ingrese Remitente del Correo. Dato Obligatorio.");
                        }
                    }
                    if (rutaServicio == "")
                    {
                        errores.Add("Ingrese ruta del servicio de correo. Dato Obligatorio");
                    }
                    if (errores.Count > 0)
                    {
                        respuesta.Mensaje = Utilitarios.FormatearErrorTexto(errores);
                        return respuesta;
                    }
                    if (notificacion.p_destinatario != null)
                    {
                        notificacion.p_destinatario = notificacion.p_destinatario.Trim();
                    }
                    using (var client = new WebClient())
                    {
                        client.Encoding = Encoding.UTF8;
                        var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
                        string jsonString = JsonSerializar.Serialize(notificacion);
                        client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        respuesta.Mensaje = client.UploadString(new Uri(rutaServicio), "POST", jsonString);
                        respuesta.Estado = Constante.COD_OK;
                    }
                }
                catch (Exception ex)
                {
                    respuesta.Estado = Constante.COD_ERROR;
                    respuesta.Mensaje = ex.Message;
                }
                return respuesta;
            }
        }
        //<FIN.GTI_7012_26>
        ////<INI.GTI_7012_26>
        //public Respuesta Test()
        //{
        //    //Envio de manera Asincrono
        //    Object semilla = new object();
        //    lock (semilla)
        //    {
        //        string rutaServicio = "http://dean/wsCWRV_2/ServicioPlaft.svc/plaft/actualizar-propuesta";
        //        Respuesta respuesta = new Respuesta();
        //        try
        //        {
        //            using (var client = new WebClient())
        //            {
        //                var JsonSerializar = new System.Web.Script.Serialization.JavaScriptSerializer();
        //                string jsonString = "{\n\t\"token\":\"OJZIUyFxQBH8iQazDKCy7Guk4FpY9G\",\n\t\"num_solicitud\":\"RPP_229627\",\n\t\"cod_estado_plaft\":2,\n\t\"GlsObservacionRpp\":\"\"\n}\n\n\t";
        //                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
        //                respuesta.Mensaje = client.UploadString(new Uri(rutaServicio), "POST", jsonString);
        //                respuesta.Estado = Constante.COD_OK;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            respuesta.Estado = Constante.COD_ERROR;
        //            respuesta.Mensaje = ex.Message;
        //        }
        //        return respuesta;
        //    }
        //}
        ////<FIN.GTI_7012_26>
        public ConfiguracionCorreo ObtenerConfiguracionCorreo(int cod_proceso, DateTime fec_solicitud)
        {
            return repositorioParametroGeneral.ObtenerConfiguracionCorreo(cod_proceso, fec_solicitud);
        }
        public SolicitudRPPlus ObtenerEstadoSolicitudRPPlus(string num_solicitud)
        {
            return repositorioSolicitudRPPlus.ObtenerEstado(num_solicitud);
        }
        ////<FIN.GTI_7012_26>
        public void RegistrarFlujoEvaluacion(FlujoEvaluacion flujoEvaluacion)
        {
            repositorioSolicitudRPPlus.RegistrarFlujoEvaluacion(flujoEvaluacion);
        }
        public List<SolicitudRPPlus> ListarSolicitudEvaluacion()
        {
            var solicitud = repositorioSolicitudRPPlus.ListarSolicitudEvaluacion();
            return solicitud;
        }
        public List<SolicitudRPPlus> ListarSolicitudCierres(string cuspp)
        {
            var solicitud = repositorioSolicitudRPPlus.ListarSolicitudCierres(cuspp);
            return solicitud;
        }
        public string ArchivosExistentes(string num_solicitud)
        {
            var solicitud = repositorioFlujoMovimiento.ObtenerArchivosExistentes(num_solicitud);
            return solicitud;
        }
        ////<FIN.GTI_7012_26>

        public void CotizarIFP(string tokenUsuario, ref SolicitudIFP solicitud1)
        {
            try
            {
                SolicitudIFP solicitud = solicitud1;

                //Proxies.MotorIFP.IServicioMotorIFP motorIFP = new Proxies.MotorIFP.ServicioMotorIFPClient("EPMotorIFPws");

                List<ParametrosMotorIFP> lstParametroCotizacion = repositorioSolicitudIFP.ObtenerParametroCotizaciones(solicitud.Id, solicitud.FechaSolicitud.Value, solicitud.Usuario.NombreUsuario);

                string wl_XML_Cotiza = String.Empty;
                string XML_Cotiza = String.Empty;
                string url = string.Empty;

                string endpoint = ConfigurationManager.AppSettings["url_motor_calculo_rentas"];
                string rutaLog = ConfigurationManager.AppSettings["RutaLog"];

                string nombreParametroGenerales = tokenUsuario.Replace("/", "_").Replace("=", "_");
                string rutaParametroGenerales = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\ArchivoParametroIFP\\parametroGenerales" + nombreParametroGenerales + ".rep";

                List<ParametrosMotorIFP> lstParametrosMotorIFP = (List<ParametrosMotorIFP>)LeerArchivoBinario<List<ParametrosMotorIFP>>(rutaParametroGenerales);

                url = string.Format(endpoint, "cotizar");

                int itemCotizacion = 0;
                foreach (var item in lstParametroCotizacion)
                {
                    string parametroGeneralIFP = string.Empty;
                    string cotizacion = string.Empty;

                    CotizacionMotorIFP cotizacionIFP = new CotizacionMotorIFP();

                    var parametro = lstParametrosMotorIFP.Find(p => p.cod_monedaIFP == item.cotizacion.cod_moneda && p.cod_tipo_temporalidadIFP == item.cotizacion.cod_tipo_temporalidad && p.fec_cotizacionIFP == item.cotizacion.fec_cotizacion);
                    parametro.cotizacion = item.cotizacion;
                    parametro.tipo_calculo = Enums.TipoCalculo.Cotizacion.StringValue();
                    parametro.tipo_producto = Enums.TipoProducto.IFP.StringValue();
                    parametro.cotizacion.fec_documento = parametro.cotizacion.fec_cotizacion;
                    //ARREGLO "TEMPORAL": SE ENVÍA SIEMPRE EN TRUE PORQUE ESE API FALLA SI VA COMO FALSE(FALTA REVISARLO), EN CWRV SE HA PUESTO EL INDICADOR DE LOG COMO NO PORQUE GENERA LOGS PESADOS
                    parametro.isLogCotizacion = true;//solicitud.isLogCotizacion;
                    parametro.isLogReserva = solicitud.isLogReserva;

                    foreach (var beneficiario in parametro.cotizacion.beneficiarios)
                    {
                        beneficiario.val_pje_renta = solicitud.Beneficiarios.Find(ben => ben.Parentesco.Id == beneficiario.cod_parentesco && ben.Identificacion.Numero == beneficiario.num_identificacion).ValPjeRenta;
                    }

                    parametroGeneralIFP = JsonConvert.SerializeObject(parametro);

                    log.Debug(string.Format("Request IFP [{0}]", parametro.cotizacion.num_solicitud));
                    if (solicitud.isLogCotizacion)
                        log.Debug(string.Format("Request Body [{0}]", parametroGeneralIFP));

                    //Proxies.MotorIFP.Parametros parametros = new Proxies.MotorIFP.Parametros();
                    //parametros = new JavaScriptSerializer().Deserialize<Proxies.MotorIFP.Parametros>(parametroGeneralIFP);
                    //parametros.tipo_calculo = Enums.TipoCalculo.Cotizacion.StringValue();
                    //parametros.tipo_producto = Enums.TipoProducto.IFP.StringValue();
                    //parametros.cotizacion.fec_documento = par.cotizacion.fec_cotizacion;
                    //parametros.cotizacion.Plan = new Proxies.MotorIFP.Plan { Id = item.cotizacion.cod_plan.ToString() };

                    //Proxies.MotorIFP.Cotizacion cotizacionMotorIFP = new Proxies.MotorIFP.Cotizacion();                    
                    //cotizacionMotorIFP = motorIFP.CotizarIFP(parametros);

                    log.Debug(string.Format("Se va a consumir el endpoint del motor de cálculo rentas que cotiza: POST [{0}]", url));

                    /*var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    httpWebRequest.ContentType = "application/json;charset=utf-8";
                    httpWebRequest.Method = "POST";

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        streamWriter.Write(parametroGeneralIFP);
                        streamWriter.Flush();
                        streamWriter.Close();
                    }

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var resultadoCotizacion = streamReader.ReadToEnd();
                        
                        cotizacionIFP = JsonConvert.DeserializeObject<CotizacionMotorIFP>(resultadoCotizacion);
                    }*/

                    using (var httpClient = new HttpClient())
                    {

                        var response = httpClient.PostAsync(url, new StringContent(parametroGeneralIFP, Encoding.UTF8, "application/json")).Result;
                        var responseContent = response.Content.ReadAsStringAsync().Result;

                        if (solicitud.isLogCotizacion)
                            log.Debug(string.Format("Response Body [{0}]", responseContent));

                        var estadoPeticion = response.IsSuccessStatusCode;
                        if (estadoPeticion)
                        {
                            cotizacionIFP = JsonConvert.DeserializeObject<CotizacionMotorIFP>(responseContent);
                        }
                        else
                        {
                            cotizacionIFP = parametro.cotizacion;
                            cotizacionIFP.ind_cotiza = false;
                            cotizacionIFP.num_error_cot = 300;
                        }
                    }

                    //cotizacion = new JavaScriptSerializer().Serialize(cotizacionMotorIFP);
                    //cotizacionIFP = new JavaScriptSerializer().Deserialize<CotizacionMotorIFP>(cotizacion);

                    if (Math.Round(cotizacionIFP.val_tasa_ret_accion, 2, MidpointRounding.AwayFromZero) < cotizacionIFP.parametro_ash.tas_ltra || Math.Round(cotizacionIFP.val_tasa_ret_accion, 2, MidpointRounding.AwayFromZero) > cotizacionIFP.parametro_ash.tas_htra)
                    {
                        cotizacionIFP.num_error_cot = 300;
                        cotizacionIFP.ind_cotiza = false;
                    }

                    wl_XML_Cotiza += "<cotiza>";
                    wl_XML_Cotiza += " <num_solicitud>" + cotizacionIFP.num_solicitud + "</num_solicitud>";
                    wl_XML_Cotiza += " <fec_cotizacion>" + cotizacionIFP.fec_cotizacion.ToString("dd/MM/yyyy") + "</fec_cotizacion>";
                    wl_XML_Cotiza += " <num_correlativo>" + cotizacionIFP.num_correlativo.ToString() + "</num_correlativo>";
                    wl_XML_Cotiza += " <cod_estado_cotizacion>" + "02" + "</cod_estado_cotizacion>";
                    wl_XML_Cotiza += " <val_descuento_comision>" + cotizacionIFP.val_descuento_comision + "</val_descuento_comision>";
                    wl_XML_Cotiza += " <cod_tipo_calculo>" + "2" + "</cod_tipo_calculo>";
                    wl_XML_Cotiza += " <ind_cotiza>" + ((cotizacionIFP.ind_cotiza) ? "" : "**") + "</ind_cotiza>";
                    wl_XML_Cotiza += " <val_fac_cia>" + cotizacionIFP.val_fac_cia + "</val_fac_cia>";
                    wl_XML_Cotiza += " <val_mto_cia>" + cotizacionIFP.val_mto_cia + "</val_mto_cia>";
                    wl_XML_Cotiza += " <val_mto_cia_sin_comision>" + cotizacionIFP.val_mto_cia_sin_comision + "</val_mto_cia_sin_comision>";
                    wl_XML_Cotiza += " <val_pen_cia>" + cotizacionIFP.val_renta + "</val_pen_cia>";
                    wl_XML_Cotiza += " <val_pen_cia_mo>" + cotizacionIFP.val_renta_mo + "</val_pen_cia_mo>";
                    wl_XML_Cotiza += " <val_pen_ref>" + 0 + "</val_pen_ref>";
                    wl_XML_Cotiza += " <val_pen_ref_mo>" + 0 + "</val_pen_ref_mo>";
                    wl_XML_Cotiza += " <val_tasa_int_vit>" + cotizacionIFP.val_tasa_int_vit + "</val_tasa_int_vit>";
                    wl_XML_Cotiza += " <val_tasa_venta_ash>" + cotizacionIFP.val_tasa_venta_ash + "</val_tasa_venta_ash>";
                    wl_XML_Cotiza += " <val_tasa_ret_accion>" + cotizacionIFP.val_tasa_ret_accion + "</val_tasa_ret_accion>";
                    wl_XML_Cotiza += " <val_tasa_costo_equiv>" + cotizacionIFP.val_tasa_costo_equiv + "</val_tasa_costo_equiv>";
                    wl_XML_Cotiza += " <val_duration>" + 0 + "</val_duration>";
                    wl_XML_Cotiza += " <num_error_cot>" + cotizacionIFP.num_error_cot + "</num_error_cot>";
                    wl_XML_Cotiza += " <val_tasa_int_temp>" + cotizacionIFP.val_tasa_int_temp + "</val_tasa_int_temp>";
                    wl_XML_Cotiza += " <val_fac_afp>" + 0 + "</val_fac_afp>";
                    wl_XML_Cotiza += " <val_mto_afp>" + 0 + "</val_mto_afp>";
                    wl_XML_Cotiza += " <val_pen_afp>" + 0 + "</val_pen_afp>";
                    wl_XML_Cotiza += " <val_afp_pen_ref>" + 0 + "</val_afp_pen_ref>";
                    wl_XML_Cotiza += " <val_tasa_venta_ash_2>" + 0 + "</val_tasa_venta_ash_2>";
                    wl_XML_Cotiza += " <val_tasa_ret_accion_2>" + 0 + "</val_tasa_ret_accion_2>";
                    wl_XML_Cotiza += " <val_tasa_costo_equiv_2>" + 0 + "</val_tasa_costo_equiv_2>";
                    wl_XML_Cotiza += " <val_duration_2>" + 0 + "</val_duration_2>";
                    wl_XML_Cotiza += " <num_error_cot_2>" + 0 + "</num_error_cot_2>";
                    wl_XML_Cotiza += " <val_tasa_cesion>" + cotizacionIFP.val_tasa_cesion + "</val_tasa_cesion>";
                    wl_XML_Cotiza += " <val_tasa_cesion_moneda2>" + 0 + "</val_tasa_cesion_moneda2>";
                    wl_XML_Cotiza += " <cod_moneda>" + cotizacionIFP.cod_moneda + "</cod_moneda>";
                    wl_XML_Cotiza += " <wl_cod_username>" + solicitud.Usuario.NombreUsuario + "</wl_cod_username>";
                    wl_XML_Cotiza += " <val_total_garantizado>" + cotizacionIFP.val_total_garantizado + "</val_total_garantizado>";
                    wl_XML_Cotiza += " <val_1era_prima_is>" + cotizacionIFP.val_1era_prima_is + "</val_1era_prima_is>";
                    wl_XML_Cotiza += " <val_fac_dev>" + cotizacionIFP.val_fac_dev + "</val_fac_dev>";
                    wl_XML_Cotiza += " <val_mto_dev>" + cotizacionIFP.val_mto_dev + "</val_mto_dev>";                    

                    if (cotizacionIFP.reserva != null)
                    {
                        wl_XML_Cotiza += " <val_res_pension>" + cotizacionIFP.reserva.val_prima_unica_pension + "</val_res_pension>";
                        wl_XML_Cotiza += " <val_res_sepelio>" + cotizacionIFP.reserva.val_prima_unica_sepelio + "</val_res_sepelio>";
                        wl_XML_Cotiza += " <val_res_devolucion>" + cotizacionIFP.reserva.val_prima_unica_devolucion + "</val_res_devolucion>";
                        wl_XML_Cotiza += " <val_res_fallecimiento>" + cotizacionIFP.reserva.val_prima_unica_fallecimiento + "</val_res_fallecimiento>";                        
                    }

                    wl_XML_Cotiza += " <val_para_duration>" + cotizacionIFP.val_para_duration + "</val_para_duration>";
                    wl_XML_Cotiza += " <val_1era_renta_is>" + cotizacionIFP.val_renta_sin_diferimiento + "</val_1era_renta_is>";
                    wl_XML_Cotiza += "</cotiza>";

                    if (cotizacionIFP.lstRescate != null)
                    {
                        foreach (var itemRescate in cotizacionIFP.lstRescate)
                        {
                            wl_XML_Cotiza += "<rescate>";
                            wl_XML_Cotiza += " <num_solicitud>" + cotizacionIFP.num_solicitud + "</num_solicitud>";
                            wl_XML_Cotiza += " <num_correlativo>" + cotizacionIFP.num_correlativo.ToString() + "</num_correlativo>";
                            wl_XML_Cotiza += " <num_mes_rescate>" + itemRescate.mes_rescate + "</num_mes_rescate>";
                            wl_XML_Cotiza += " <val_rescate>" + itemRescate.valor_rescate + "</val_rescate>";
                            wl_XML_Cotiza += " <val_tasa_rescate_mensual>" + itemRescate.tasa_rescate_mensual + "</val_tasa_rescate_mensual>";
                            wl_XML_Cotiza += " <val_tasa_rescate_anual>" + itemRescate.tasa_rescate_anual + "</val_tasa_rescate_anual>";
                            wl_XML_Cotiza += " <val_renta_mensual>" + itemRescate.valor_renta_mensual + "</val_renta_mensual>";
                            wl_XML_Cotiza += "</rescate>";
                        }
                    }

                    if (parametro.cotizacion.ind_cotiza)
                    {
                        solicitud.Cotizaciones[itemCotizacion].PensionCia = (double)parametro.cotizacion.val_renta;
                        solicitud.Cotizaciones[itemCotizacion].PensionCiaMO = parametro.cotizacion.val_renta_mo;
                        solicitud.Cotizaciones[itemCotizacion].ValTasaCostoEquiv = parametro.cotizacion.val_tasa_costo_equiv;
                        solicitud.Cotizaciones[itemCotizacion].TasaVenta = parametro.cotizacion.val_tasa_int_vit;
                        solicitud.Cotizaciones[itemCotizacion].TasaVentaSbs = parametro.cotizacion.val_tasa_venta_ash;
                        solicitud.Cotizaciones[itemCotizacion].TasaRetornoAccionista = parametro.cotizacion.val_tasa_ret_accion;
                        solicitud.Cotizaciones[itemCotizacion].Pension2doTramo = parametro.cotizacion.val_1era_prima_is;
                    }
                    solicitud.Cotizaciones[itemCotizacion].IndCotiza = parametro.cotizacion.ind_cotiza ? "" : "**";
                    solicitud.Cotizaciones[itemCotizacion].IndErrorCotiza = parametro.cotizacion.num_error_cot;

                    XML_Cotiza += wl_XML_Cotiza;
                    wl_XML_Cotiza = string.Empty;

                    log.Debug("solicitud.isLogCotizacion: " + solicitud.isLogCotizacion);
                    log.Debug("parametro.cotizacion.ind_cotiza: " + parametro.cotizacion.ind_cotiza);
                    if (solicitud.isLogCotizacion && parametro.cotizacion.ind_cotiza)
                    {
                        log.Debug("rutaLog: " + rutaLog);
                        generarArchivoLogCotizacion(rutaLog, "OutPut", cotizacionIFP.num_solicitud, cotizacionIFP.num_correlativo, cotizacionIFP.lstOutput);

                        generarArchivoLogCotizacion(rutaLog, "InPut", cotizacionIFP.num_solicitud, cotizacionIFP.num_correlativo, cotizacionIFP.lstInput);
                    }

                    solicitud.Cotizaciones[itemCotizacion].PorcentajeBeneficiarios = CrearPorcentajesBeneficiario(solicitud.Beneficiarios, cotizacionIFP.num_correlativo);

                    itemCotizacion++;
                }

                repositorioSolicitudIFP.RegistrarPjeBen(solicitud);
                repositorioSolicitudIFP.RegistrarCotiza("<insert>" + XML_Cotiza + "</insert>", solicitud.Usuario.NombreUsuario);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public int CantidadSolicitudes(string cuspp, string moneda, double val_mto_prima_unica)
        {
            int Cantidad = repositorioSolicitudIFP.CantidadSolicitudes(cuspp, moneda, val_mto_prima_unica);
            return Cantidad;
        }

        //<FIN.GTI_7012_S25>

        public ParametrosMotorIFP ObtenerParametroGenerales(string cod_tipo_temporalidad, string cod_moneda, DateTime fec_cotizacion, bool ind_flag, string tokenUsuario, string usuario)
        {
            try
            {
                ParametrosMotorIFP parametroGenerales = repositorioSolicitudIFP.ObtenerParametroGenerales(cod_tipo_temporalidad, cod_moneda, fec_cotizacion, ind_flag, usuario);
                List<ParametrosMotorIFP> lstParametrosMotorIFP = new List<ParametrosMotorIFP>();
                string nombreParametroGenerales = tokenUsuario.Replace("/", "_").Replace("=", "_");
                string rutaParametroGenerales = System.Web.Hosting.HostingEnvironment.MapPath("~") + "\\ArchivoParametroIFP\\parametroGenerales" + nombreParametroGenerales + ".rep";
                if (System.IO.File.Exists(rutaParametroGenerales))
                {
                    lstParametrosMotorIFP = (List<ParametrosMotorIFP>)LeerArchivoBinario<List<ParametrosMotorIFP>>(rutaParametroGenerales);
                }
                lstParametrosMotorIFP.Add(parametroGenerales);
                EscribirArchivoBinario(rutaParametroGenerales, lstParametrosMotorIFP);
                return parametroGenerales;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private static void EscribirArchivoBinario<T>(string RutaArchivo, T EscribirenObjeto)
        {
            try
            {
                using (Stream strm = File.Open(RutaArchivo, false ? FileMode.Append : FileMode.Create))
                {
                    var FormatoBinario = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    FormatoBinario.Serialize(strm, EscribirenObjeto);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        private static T LeerArchivoBinario<T>(string RutaArchivo)
        {
            try
            {
                using (Stream strm = File.Open(RutaArchivo, FileMode.Open))
                {
                    var FormatoBinario = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    return (T)FormatoBinario.Deserialize(strm);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        //public void RegistrarSolicitudIFP(List<ParametrosMotorIFP> lstParametrosMotorIFP, ref SolicitudIFP solicitud)
        public void RegistrarSolicitudIFP(string tokenUsuario, ref SolicitudIFP solicitud)
        {
            //TextWriter tw = null;
            //if (!System.IO.Directory.Exists("C:\\temp\\" + "log0")) { System.IO.Directory.CreateDirectory("C:\\temp\\" + "log0"); }
            //tw = new StreamWriter("C:\\temp\\" + "log0" + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt");
            //tw.WriteLine("Inicio CotizadorServicio.RegistrarSolicitudIFP en servicio" + DateTime.Now.ToString("yyyyMMdd_hhmmss"));

                //tw.WriteLine();
                //tw.WriteLine("  Inicio CotizadorServicio.repositorioSolicitudIFP.Registrar en servicio" + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
                repositorioSolicitudIFP.Registrar(ref solicitud);
                //tw.WriteLine("  Fin CotizadorServicio.repositorioSolicitudIFP.Registrar en servicio" + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
                //CotizarIFP(lstParametrosMotorIFP, ref solicitud);
                //tw.WriteLine();
                //tw.WriteLine("  Inicio CotizadorServicio.CotizarIFP en servicio" + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
                
                CotizarIFP(tokenUsuario, ref solicitud);

                //tw.WriteLine("  Fin CotizadorServicio.CotizarIFP en servicio" + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
                //tw.WriteLine();
                //tw.WriteLine("  Inicio CotizadorServicio.transaccion.Complete en servicio" + DateTime.Now.ToString("yyyyMMdd_hhmmss"));

                //tw.WriteLine("  Fin CotizadorServicio.transaccion.Complete en servicio" + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
                //tw.WriteLine();
            
            //tw.WriteLine("Fin CotizadorServicio.RegistrarSolicitudIFP en servicio" + DateTime.Now.ToString("yyyyMMdd_hhmmss"));
            //tw.Close();
        }

        public void ActualizarSolicitudIFP(string tokenUsuario, ref SolicitudIFP solicitud)
        {
            repositorioSolicitudIFP.Actualizar(ref solicitud);
            //CotizarIFP(lstParametrosMotorIFP, ref solicitud);
            CotizarIFP(tokenUsuario, ref solicitud);
        }

        public SolicitudIFP ObtenerDatosSolicitudIFP(string idSolicitud)
        {
            var solicitudIFP = repositorioSolicitudIFP.ObtenerDatos(idSolicitud);
            return solicitudIFP;
        }
        public List<SolicitudIFP> ListarSolicitudIFP(string cuspp)
        {
            var solicitud = repositorioSolicitudIFP.Listar(cuspp);
            return solicitud;
        }
        public void RegistrarBeneficiarios(List<GrupoFamiliar> lstEntity, int idGrupoFamiliar, string tipoPlan)
        {
            repositorioGrupoFamiliar.RegistrarBeneficiarios(lstEntity, idGrupoFamiliar, tipoPlan);
        }
        public List<DatosSol> ObtenerDatosporSolicitudIFP(string num_Solicitud)
        {
            List<DatosSol> lstDatosSolicitud = repositorioSolicitudIFP.ObtenerDatosporSolicitudIFP(num_Solicitud);
            return lstDatosSolicitud;
        }
        public List<RolDcom> ListarRangoDcomIFP(RolDcom rolDcom)
        {
            var rango = repositorioRolDcom.ListarRangoDcomIFP(rolDcom);
            return rango;
        }

        public long RegistrarPolizaIFPADMWR(string num_solicitud, string usuario, GrupoFamiliar grup_fam)
        {
            EmisionPoliza emisionPoliza = repositorioEmisionPoliza.EmitirPolizaIFP(num_solicitud, 0, "");
            List<Direccion> Direcciones = repositorioDireccion.Listar(num_solicitud, null, usuario).FindAll(dp => dp.Principal && dp.Vigencia);

            Proxies.ModuloADMWR.IServiceADMWR moduloADMWR = new Proxies.ModuloADMWR.ServiceADMWRClient("EPADMWR");
            Proxies.ModuloADMWR.Poliza poliza = new Proxies.ModuloADMWR.Poliza();

            poliza.gls_poliza = emisionPoliza.Poliza.NumPoliza.ToString();
            poliza.gls_dig_poliza = emisionPoliza.Poliza.DigPoliza;
            poliza.gls_solicitud = emisionPoliza.SolicitudIFP.Id;
            if (emisionPoliza.SolicitudIFP.FechaSolicitud != null)
                poliza.fec_solicitud = emisionPoliza.SolicitudIFP.FechaSolicitud.Value;
            if (emisionPoliza.SolicitudIFP.FechaDevengue != null)
                poliza.fec_devengue = emisionPoliza.SolicitudIFP.FechaDevengue.Value;
            poliza.Modalidad = new Proxies.ModuloADMWR.Modalidad { cod_modalidad = emisionPoliza.SolicitudIFP.Cotizaciones[0].Modalidad.Id, ind_modalidad = emisionPoliza.SolicitudIFP.Cotizaciones[0].Modalidad.Nombre };
            poliza.ind_fallecimiento = false;
            poliza.EstadoPoliza = new Proxies.ModuloADMWR.EstadoPoliza { cod_estado_poliza = emisionPoliza.SolicitudIFP.CodigoEstadoPoliza };
            poliza.EstadoCausal = new Proxies.ModuloADMWR.EstadoCausal { cod_causal_estado = "01" };
            if (emisionPoliza.Poliza.FecEmision != null)
                poliza.fec_emision_poliza = emisionPoliza.Poliza.FecEmision;
            if (emisionPoliza.Poliza.FecInicioVigencia != null)
                poliza.fec_inicio_vigencia = emisionPoliza.Poliza.FecInicioVigencia;
            if (emisionPoliza.Poliza.FecFinVigencia != null)
                poliza.fec_fin_vigencia = emisionPoliza.Poliza.FecFinVigencia;
            if (emisionPoliza.Poliza.FecPago != null)
                poliza.fec_inicio_pago = emisionPoliza.Poliza.FecPago;
            poliza.num_cotizacion = emisionPoliza.SolicitudIFP.Cotizaciones[0].Correlativo;
            poliza.MonedaCotizacion = new Proxies.ModuloADMWR.Moneda { cod_moneda = emisionPoliza.SolicitudIFP.Cotizaciones[0].Moneda.Id };
            poliza.val_prima_unica = emisionPoliza.Poliza.ValPrimaBruta;
            poliza.Agente = new Proxies.ModuloADMWR.Agente { num_agente = Convert.ToInt32(emisionPoliza.SolicitudIFP.Agente.Id) };
            poliza.num_meses_temporalidad = emisionPoliza.SolicitudIFP.Cotizaciones[0].Temporalidad.Anhos * 12;
            poliza.num_meses_garantizados = emisionPoliza.SolicitudIFP.Cotizaciones[0].PeriodoGarantizado * 12;
            poliza.num_meses_primer_tramo = Convert.ToInt32(emisionPoliza.SolicitudIFP.Cotizaciones[0].PagoDoble * 12);
            poliza.val_pje_renta_segundo_tramo = emisionPoliza.SolicitudIFP.Cotizaciones[0].PjePagoDoble;
            poliza.val_pje_conyuge = 0;//emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeConyuge;
            poliza.MonedaPrima = new Proxies.ModuloADMWR.Moneda { cod_moneda = emisionPoliza.SolicitudIFP.MonedaPrimaUnica.Id };
            poliza.TipoProducto = new Proxies.ModuloADMWR.TipoProducto { cod_tipo_producto = emisionPoliza.SolicitudIFP.Cotizaciones[0].Producto.Id }; // "01";
            poliza.TipoProducto = new Proxies.ModuloADMWR.TipoProducto { cod_tipo_producto = Enums.TipoCotizacion.RentaPrivadaIFP.StringValue() };
            poliza.TipoPlan = new Proxies.ModuloADMWR.TipoPlan { cod_tipo_plan = "" };
            poliza.PlanRP = new Proxies.ModuloADMWR.PlanRP { cod_plan = emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Id };
            poliza.num_meses_diferidos = Convert.ToInt32(emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPerDiferido * 12);

            poliza.val_pje_devolucion_fallec = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDevFallec;
            if (emisionPoliza.SolicitudIFP.Cotizaciones[0].Plan.Id == Enums.Planes.PLAN2.StringValue())
            {
                poliza.val_pje_devolucion_fallec = 0.00;
            }

            poliza.MonedaPago = new Proxies.ModuloADMWR.Moneda { cod_moneda = poliza.MonedaCotizacion.cod_moneda };
            poliza.val_pje_ajuste_moneda = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValMonAju;
            poliza.val_renta_original = emisionPoliza.SolicitudIFP.Cotizaciones[0].PensionCiaMO;
            poliza.val_renta_base = emisionPoliza.SolicitudIFP.Cotizaciones[0].PensionCiaMO;
            poliza.val_renta_base_anterior = emisionPoliza.SolicitudIFP.Cotizaciones[0].PensionCiaMO;
            poliza.val_pje_devolucion = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDev;
            poliza.ind_sepelio = (emisionPoliza.SolicitudIFP.Cotizaciones[0].IndGastoSepelio == "S" ? true : false);
            poliza.val_tasa_tra = emisionPoliza.SolicitudIFP.Cotizaciones[0].TasaRetornoAccionista;
            poliza.val_tasa_venta = emisionPoliza.SolicitudIFP.Cotizaciones[0].TasaVenta;
            poliza.val_tasa_venta_is = emisionPoliza.SolicitudIFP.Cotizaciones[0].TasaVentaSbs;
            poliza.val_dcom = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValPjeDCOM;
            poliza.aud_usr_ingreso = usuario;
            poliza.val_total_cic = emisionPoliza.SolicitudIFP.Afiliado.SaldoCIC;
            poliza.val_tasa_costo_equiv = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValTasaCostoEquiv;
            poliza.val_tipo_cambio = emisionPoliza.SolicitudIFP.TipoCambio;
            poliza.val_renta_seg_tramo = emisionPoliza.SolicitudIFP.Cotizaciones[0].Pension2doTramo;
            poliza.TipoPago = new Proxies.ModuloADMWR.TipoPago { cod_tipo_pago = "T" }; //tabla rpp.T_TIPPAG de ADMWR
            poliza.num_meses_ajuste = emisionPoliza.Poliza.val_meses_periodo;
            poliza.val_iva = emisionPoliza.Poliza.ValIva;
            poliza.val_dev = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValDev;
            poliza.val_res_pension = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResPension;
            poliza.val_res_sepelio = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResSepelio;            
            poliza.val_res_devolucion = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResDevolucion;
            poliza.val_res_fallecimiento = emisionPoliza.SolicitudIFP.Cotizaciones[0].ValResFallecimiento;
            poliza.cod_canal_distribucion = emisionPoliza.SolicitudIFP.CodCanalDistribucion;
            poliza.ind_rescate = emisionPoliza.Poliza.ind_rescate;

            List<Proxies.ModuloADMWR.Beneficiario> beneficiarios = new List<Proxies.ModuloADMWR.Beneficiario>();
            foreach (var beneficiario in emisionPoliza.SolicitudIFP.Beneficiarios)
            {
                Proxies.ModuloADMWR.Beneficiario ben = new Proxies.ModuloADMWR.Beneficiario();
                Proxies.ModuloADMWR.Parentesco par = new Proxies.ModuloADMWR.Parentesco();
                Proxies.ModuloADMWR.Banco ban = new Proxies.ModuloADMWR.Banco();
                Proxies.ModuloADMWR.TipoCuentaBanco tipctabnc = new Proxies.ModuloADMWR.TipoCuentaBanco();
                Proxies.ModuloADMWR.TipoIdentificacion tipide = new Proxies.ModuloADMWR.TipoIdentificacion();
                Proxies.ModuloADMWR.Sexo sexo = new Proxies.ModuloADMWR.Sexo();

                par.cod_parentesco = beneficiario.Parentesco.Id;
                ben.Parentesco = par;
                ben.fec_inicio_vigencia = poliza.fec_inicio_vigencia;
                ben.fec_fin_vigencia = poliza.fec_fin_vigencia;
                if (ben.Parentesco.cod_parentesco == Enums.Parentesco.Afiliado.StringValue())
                {
                    ben.EstadoBeneficiario = new Proxies.ModuloADMWR.EstadoBeneficiario { cod_estado_beneficiario = "SUSP" };
                    ben.EstadoCausalBeneficiario = new Proxies.ModuloADMWR.EstadoCausalBeneficiario { cod_estado_causal = "PENACTI" };
                }
                else
                {
                    ben.EstadoBeneficiario = new Proxies.ModuloADMWR.EstadoBeneficiario { cod_estado_beneficiario = "SUSP" };
                    ben.EstadoCausalBeneficiario = new Proxies.ModuloADMWR.EstadoCausalBeneficiario { cod_estado_causal = "PENACTI" };
                }
                ben.fec_pagar_desde = poliza.fec_inicio_pago;
                ben.val_pje_renta = beneficiario.ValPjeRenta * 100;
                ben.ind_invalidez = beneficiario.Invalido;
                ben.TipoInvalidez = new Proxies.ModuloADMWR.TipoInvalidez { cod_tipo_invalidez = beneficiario.TipoInvalidez.Id };
                ben.fec_invalidez = beneficiario.FechaInvalidez;
                ben.val_pje_adicional = 0;
                ban.cod_banco = beneficiario.Banco.Id;
                ben.Banco = ban;
                tipctabnc.cod_tipo_cta_banco = beneficiario.TipoCtaBanco.Id;
                ben.TipoCuentaBanco = tipctabnc;
                ben.gls_cta_bancaria = beneficiario.NumeroBanco;
                ben.ind_confidencialidad_datos = beneficiario.Confidencialidaddatos.Id == "0" ? true : false;
                if (ben.Parentesco.cod_parentesco == Enums.Parentesco.Afiliado.StringValue())
                {
                    ben.gls_correo_electronico = beneficiario.CorreoElectronico;
                    ben.gls_telefono = grup_fam.Telefono1;
                    ben.gls_celular = grup_fam.Telefono2;
                }
                else
                {
                    ben.gls_correo_electronico = "";
                    ben.gls_telefono = "";
                    ben.gls_celular = "";
                }
                ben.ind_pep = beneficiario.ind_PEP;
                ben.ind_sujeto_obligado = beneficiario.ind_SujetoObligado;
                ben.num_item = beneficiario.num_item;
                ben.MedioComunicacion = new Proxies.ModuloADMWR.MedioComunicacion { cod_medio_comunicacion = Convert.ToInt32(emisionPoliza.SolicitudIFP.Beneficiarios[0].Comunicacion.Id) };
                ben.Persona = new Proxies.ModuloADMWR.Persona();
                tipide.cod_tipo_identificacion = beneficiario.Identificacion.IdTipo;
                ben.Persona.TipoIdentificacion = tipide;
                ben.Persona.gls_nro_identificacion = beneficiario.Identificacion.Numero.ToString();
                ben.Persona.gls_ape_paterno = beneficiario.ApellidoPaterno;
                ben.Persona.gls_ape_materno = beneficiario.ApellidoMaterno;
                ben.Persona.gls_nom_persona = beneficiario.Nombre;
                ben.Persona.gls_persona = beneficiario.ApellidosNombres;
                ben.Persona.fec_nacimiento = beneficiario.FechaNacimiento;
                if (ben.Parentesco.cod_parentesco == Enums.Parentesco.Afiliado.StringValue())
                {
                    ben.Persona.EstadoCivil = new Proxies.ModuloADMWR.EstadoCivil { cod_estado_civil = grup_fam.estadoCivil };
                }
                else
                {
                    ben.Persona.EstadoCivil = new Proxies.ModuloADMWR.EstadoCivil { cod_estado_civil = "" };
                }
                ben.Persona.Nacionalidad = new Proxies.ModuloADMWR.Nacionalidad { cod_nacionalidad = beneficiario.Nacionalidad.cod_parametro };
                ben.Persona.Profesion = new Proxies.ModuloADMWR.Profesion { cod_profesion = beneficiario.Profesion.cod_parametro };

                ben.cod_tipo_periodo_beneficiario = beneficiario.IdTipoPeriodoBeneficiario;

                sexo.cod_sexo = beneficiario.Sexo.ToString();
                ben.Persona.Sexo = sexo;
                List<Proxies.ModuloADMWR.Direccion> direcciones = new List<Proxies.ModuloADMWR.Direccion>();
                foreach (var direccion in Direcciones)
                {
                    Proxies.ModuloADMWR.Direccion dir = new Proxies.ModuloADMWR.Direccion();
                    Proxies.ModuloADMWR.Distrito dis = new Proxies.ModuloADMWR.Distrito();
                    Proxies.ModuloADMWR.Provincia pro = new Proxies.ModuloADMWR.Provincia();
                    Proxies.ModuloADMWR.Departamento dep = new Proxies.ModuloADMWR.Departamento();
                    Proxies.ModuloADMWR.TipoVia via = new Proxies.ModuloADMWR.TipoVia();
                    dir.gls_direccion = direccion.Glosa;
                    via.cod_tipo_via = direccion.TipoVia.Id;
                    dir.TipoVia = via;
                    dir.cod_espacio_urbano = direccion.EspacioUrbano;
                    dis.cod_distrito = direccion.Comuna.Id;
                    dir.Distrito = dis;
                    pro.cod_provincia = direccion.Ciudad.Id;
                    dir.Provincia = pro;
                    dep.cod_departamento = direccion.Departamento.Id;
                    dir.Departamento = dep;
                    dir.ind_principal = direccion.Principal;
                    dir.ind_vigencia = true;
                    if (ben.Parentesco.cod_parentesco == Enums.Parentesco.Afiliado.StringValue())
                    {
                        direcciones.Add(dir);
                    }
                }
                ben.Direcciones = direcciones;
                if (ben.val_pje_renta > 0)
                {
                    beneficiarios.Add(ben);
                }
            }
            poliza.Beneficiarios = beneficiarios;

            var cotizacionRescates = new List<Proxies.ModuloADMWR.CotizacionRescate>();
            foreach (var rescate in emisionPoliza.SolicitudIFP.CotizacionRescates)
            {
                var cotRescate = new Proxies.ModuloADMWR.CotizacionRescate();

                cotRescate.num_mes = rescate.mes_rescate;
                cotRescate.val_rescate = rescate.valor_rescate;
                cotRescate.val_tasa_rescate_mensual = rescate.tasa_rescate_mensual;
                cotRescate.val_tasa_rescate_anual = rescate.tasa_rescate_anual;

                cotizacionRescates.Add(cotRescate);
            }
            poliza.cotizacionRescates = cotizacionRescates;

            Proxies.ModuloADMWR.Poliza RptaPoliza = new Proxies.ModuloADMWR.Poliza();
            RptaPoliza = moduloADMWR.RegistrarPoliza(poliza);
            return Convert.ToInt64(RptaPoliza.cod_poliza);
        }
        //<INI_GTI_7012_S30>
        public EmisionPoliza EmitirPolizaIFP(string num_solicitud, int num_poliza, string dig_poliza)
        {
            EmisionPoliza emisionPoliza = repositorioEmisionPoliza.EmitirPolizaIFP(num_solicitud, num_poliza, dig_poliza);
            return emisionPoliza;
        }
        //<FIN_GTI_7012_S30>
        public Respuesta ActualizarPolizaSME(string gls_poliza, int codigo_SME, string usuario)
        {
            Respuesta respuesta = new Respuesta();
            Proxies.ModuloADMWR.IServiceADMWR moduloADMWR = new Proxies.ModuloADMWR.ServiceADMWRClient("EPADMWR");
            Proxies.ModuloADMWR.Respuesta RptaPoliza = new Proxies.ModuloADMWR.Respuesta();
            //Proxies.ModuloADMWR.Poliza poliza = new Proxies.ModuloADMWR.Poliza();
            //RptaPoliza = moduloADMWR.GenerarPolizaElectronicaPDF(gls_poliza);

            RptaPoliza = moduloADMWR.ActualizarPolizaSME(gls_poliza, codigo_SME, usuario);

            //RptaPoliza= moduloADMWR.GenerarPolizaElectronicaPDF(gls_poliza);
            //RptaPoliza = moduloADMWR.GenerarPolizaElectronicaPDF(poliza);

            return respuesta;
        }

        //<INI_GTI_26560>
        public void ActualizarConsentimientoAfiliado(Afiliado afiliado, ConsentimientoAsesoria consentimientoAsesoria)
        {
            try
            {
                repositorioAfiliado.ActualizarConsentimiento(afiliado, consentimientoAsesoria);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<Beneficiario> ListarBeneficiarios(string numSolicitud, string usuario)
        {
            var beneficiarios = repositorioAfiliado.ListarBeneficiarios(numSolicitud, usuario);
            return beneficiarios;
        }

        public Respuesta ActualizarBeneficiario(Beneficiario beneficiario)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                // Actualizar datos del beneficiario
                repositorioAfiliado.ActualizarBeneficiario(beneficiario);

                // Actualizar direccción principal
                if (beneficiario.direccionPrincipal != null)
                    repositorioAfiliado.ActualizarDireccionBeneficiario(beneficiario.direccionPrincipal);

                // Actualizar direccción de envío de póliza física
                if (beneficiario.envioPoliza == "F")
                {
                    if (beneficiario.direccionAlterna != null)
                    {
                        if (beneficiario.direccionAlterna.numSolicitud != null)
                            repositorioAfiliado.ActualizarDireccionBeneficiario(beneficiario.direccionAlterna);
                    }
                }
                respuesta.Estado = Constante.COD_OK;
                respuesta.Mensaje = "Solicitud Actualizada";
            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                throw;
            }
            return respuesta;
        }
        //<FIN_GTI_26560>

        //<INI.GTI_26697>
        public void RegistrarPersonaVinculada(GrupoFamiliar grupo)
        {
            try
            {
                repositorioGrupoFamiliar.RegistrarPersonaVinculada(grupo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ActualizarPersonaVinculada(GrupoFamiliar grupo)
        {
            try
            {
                repositorioGrupoFamiliar.ActualizarPersonaVinculada(grupo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EliminarPersonaVinculada(int idPersonaVinculada, string usuario)
        {
            try
            {
                repositorioGrupoFamiliar.EliminarPersonaVinculada(idPersonaVinculada, usuario);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GrupoFamiliar> ObtenerPersonaVinculada(GrupoFamiliar grupo)
        {
            var personasVinculadas = repositorioGrupoFamiliar.ObtenerPersonaVinculada(grupo);
            return personasVinculadas;
        }

        public void ActualizarDireccionSolicitud(string numCuspp, string numSolicitud, string usuario)
        {
            try
            {
                repositorioDireccion.ActualizarDireccionSolicitud(numCuspp, numSolicitud, usuario);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //<FIN.GTI_26697>
        public void PreseleccionarCotizacion(string num_solicitud, string num_correlativo, string usuario)
        {
            try
            {
                repositorioSolicitud.PreseleccionarCotizacion(num_solicitud, num_correlativo, usuario);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Respuesta EnviarPolizaElectronicaRPP_PDF(int num_poliza, string usuario)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                Proxies.ModuloADMWR.Respuesta respuestaADMWR = new Proxies.ModuloADMWR.Respuesta();
                Proxies.ModuloADMWR.IServiceADMWR moduloADMWR = new Proxies.ModuloADMWR.ServiceADMWRClient("EPADMWR");

                respuestaADMWR = moduloADMWR.ReenviarPolizaPDF(num_poliza.ToString(), usuario);

                respuesta.Estado = respuestaADMWR.Estado;
                respuesta.Mensaje = respuestaADMWR.Mensaje;
                respuesta.Titulo = respuestaADMWR.Titulo;
                respuesta.Icono = respuestaADMWR.Icono;

            }
            catch (Exception ex)
            {
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = ex.Message;
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
                throw;
            }
            return respuesta;
        }

        public GrupoFamiliar ObtenerDatosBenefiCierre(int idGrupoFamiliar, string num_solicitud)
        {
            var grupo = repositorioAfiliado.ObtenerDatosCierre(idGrupoFamiliar, num_solicitud);
            return grupo;
        }

        public void CerrarBeneficiariosIFP(string num_solicitud, List<GrupoFamiliar> lstGrupoFamiliar, string tipoPlan, List<GrupoFamiliar> lstBeneficiariosPNoG, string usuario)
        {
            try
            {

                    if (lstGrupoFamiliar != null)
                    {
                        if (lstGrupoFamiliar.Count > 0)
                        {
                            repositorioAfiliado.EliminarBeneficiarioIFP(num_solicitud, tipoPlan, usuario);

                            int cantidadBeneficiarios = 2;

                            lstGrupoFamiliar.ForEach(p => p.SolicitudIFP = new SolicitudIFP { Id = num_solicitud });

                            if (tipoPlan == Enums.Planes.PLAN3.StringValue())
                            {
                                cantidadBeneficiarios = lstBeneficiariosPNoG.Count() + 2;
                                ActualizarBeneficiariosPNoG(lstBeneficiariosPNoG);
                            }
                            else
                            {
                                cantidadBeneficiarios = 2;
                            }

                            RegistrarBeneficiarios(lstGrupoFamiliar, cantidadBeneficiarios, tipoPlan);

                        }
                        else
                        {
                            if (tipoPlan == Enums.Planes.PLAN3.StringValue())
                            {
                                repositorioAfiliado.EliminarBeneficiarioIFP(num_solicitud, tipoPlan, usuario);
                            }
                        }
                    }

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ElminarGrupoFamiliar(GrupoFamiliar grupo)
        {
            try
            {
                repositorioGrupoFamiliar.Eliminar(grupo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //<GTI.INI-29372>
        public void ActualizarSolicitudEnvioObligatorio(ref SolicitudEscenario solicitudEscenario)
        {
                repositorioSolicitudEscenario.ActualizarEnvioObligatorio(ref solicitudEscenario);
        }

        public List<Solicitud> ListarSolicitudesCargaMeler(string xml)
        {
            var listaSolicitudes = repositorioSolicitud.ListarSolicitudesCargaMeler(xml);
            return listaSolicitudes;
        }
        //<GTI.FIN-29372>
        public List<ReporteIndicadoresRRVV> ListarReporteIndicadoresRRVV()
        {
            List<ReporteIndicadoresRRVV> lstReporteIndicadores = repositorioReporteIndicadores.ListarIndicadores();
            return lstReporteIndicadores;
        }

        public void SeleccionarBeneficiario(string num_solicitud, string num_correlativo, string usuario)
        {
            try
            {
                repositorioSolicitud.SeleccionarBeneficiario(num_solicitud, num_correlativo, usuario);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //public Respuesta EnvioAutomaticoVCTP(List<Solicitud> numerosSolicitudes, string usuario)
        //{
        //    Respuesta respuesta = new Respuesta();

        //    try
        //    {
        //        using (TransactionScope transaccion = new TransactionScope())
        //        { 
        //            respuesta.Estado = Constante.COD_OK;
        //            respuesta.Mensaje = "Fin proceso de envío automático del VCTP";
        //        }

        //        return respuesta;
        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Estado = Constante.COD_ERROR;
        //        respuesta.Mensaje = ex.Message;
        //        throw ex;
        //    }
        //}

        public List<ConsentimientosAgrupadosAge> ListarConsentimientosAgrupadosAge(string periodo, string usuario)
        {
            List<ConsentimientosAgrupadosAge> lstConsentimientosAgrupadosAge = repositorioReportes.ListarConsentimientosAgrupados(periodo, usuario);
            return lstConsentimientosAgrupadosAge;
        }

        public Respuesta TransferirDatosPolizaRRVV(string num_solicitud, int numeroCorrelativo, string usuario)
        {
            Respuesta respuesta = new Respuesta();

            try
            {
                string urlRviadm = ConfigurationManager.AppSettings["url_rviadm_backend"].ToString();
                string timestamp = DateTime.Now.ToString("YYYYMMDDHHmmss");

                string token = num_solicitud + timestamp;

                object sesion = new
                {
                    usuario,
                    token = token
                };

                string url = $"{urlRviadm}/accesos/crear-Sesion";

                log.Info($"Sesión rviadm - usuario: {usuario} - solicitud: {num_solicitud}");
                var resultado = BackendAPI.PostAsync(url, sesion, usuario, token);

                // Obtener datos de poliza
                var poliza = repositorioSolicitud.ObtenerDatosPoliza(num_solicitud, numeroCorrelativo, usuario);
                
                // Obtener datos de beneficiarios, personas y direcciones
                var beneficiarios = repositorioSolicitud.ObtenerDatosBeneficiarios(num_solicitud, numeroCorrelativo, usuario);

                #region Endoso cero

                // Insertar cabecera de endoso
                url = $"{urlRviadm}/endosos/inserta-endoso";
                log.Info($"Endoso: [{url}]");

                object objEndoso = new
                {
                    aud_usr_ingreso = usuario,
                    cod_cartera = "RVI",
                    num_poliza = poliza.num_poliza,
                    fec_ingreso_endoso = DateTime.Now.ToString("yyyy-MM-dd"),
                    fec_activacion = DateTime.Now.ToString("yyyy-MM-dd"),
                    cod_estado_endoso = "ACTI",
                    fec_estado_endoso = DateTime.Now,
                    cod_tipo_endoso = "POLIZA",
                    //gls_endoso = "",
                    //ind_endoso_emitido = "N",
                    //ind_despachado = "N",                        
                    ind_pension_calculada = "N",
                    //cod_ori_not_fallec = "01",
                    val_gasto_incurrido = 0,
                    //saldo_cic = solicitud.SaldoCIC,                        
                    //, num_item = 0,
                    cod_estado_flujo = 6
                };

                var data = JsonConvert.SerializeObject(objEndoso);
                log.Info($"data: {data}");

                resultado = BackendAPI.PostAsync(url, objEndoso, usuario, token);
                log.Info($"resultado: {resultado}");

                // Insertar endoso de poliza
                url = $"{urlRviadm}/endosos/inserta-endoso-poliza";
                log.Info($"Endoso poliza: [{url}]");

                object objEndosoPoliza = new
                {
                    cod_cartera = "RVI",
                    num_poliza = poliza.num_poliza,
                    num_endoso = 0,
                    cod_tipo_id_causante = poliza.cod_tipo_identificacion,
                    num_id_causante = poliza.num_identificacion,
                    num_solicitud = num_solicitud,
                    //num_plan = int,
                    cod_tipo_pension = poliza.cod_tipo_pension,
                    cod_tipo_producto = poliza.cod_tipo_producto,
                    cod_modalidad = poliza.cod_modalidad,
                    ind_modalidad = poliza.ind_modalidad,
                    ind_fallecimiento = poliza.ind_fallecimiento,
                    cod_estado_poliza = "01",
                    cod_causal_estado = "01",
                    fec_emision_poliza = DateTime.Now.ToString("yyyy-MM-dd"),
                    fec_inicio_vigencia = poliza.fec_devengue,
                    fec_fin_vigencia = new DateTime(2200, 12, 31),
                    cod_moneda_cot = poliza.cod_moneda,
                    cod_moneda_pago = poliza.cod_moneda,
                    val_pension_base = poliza.val_pen_ref_mo,
                    val_pension_original = poliza.val_pen_ref_mo,
                    num_cuispp = poliza.num_cuispp,
                    cod_afp = poliza.cod_afp,
                    num_vendedor = poliza.num_vendedor,
                    cod_origen_venta = poliza.cod_origen_vta,
                    num_meses_garantizados = poliza.val_per_garantizado,
                    num_meses_diferidos = poliza.val_per_temporal,
                    val_prima_unica = poliza.val_mto_cia,
                    dig_poliza = poliza.dig_poliza,
                    num_cotizacion = poliza.num_cotizacion_cierre,
                    val_pje_rent_temp = poliza.val_pje_rent_temp,
                    cod_tipo_renta = poliza.cod_tipo_renta,
                    fec_pago_afp = poliza.fec_emision_poliza,
                    fec_envio_poliza = DateTime.Now.ToString("yyyy-MM-dd"),
                    //val_aporte_complemento = 0,
                    ind_tiene_cobertura = poliza.ind_tiene_cobertura,
                    ind_derecho_crecer = poliza.ind_derecho_crecer,
                    ind_gratificacion = poliza.ind_gratificacion,
                    fec_ref_ajuste = poliza.fec_devengue,
                    //cod_particion_capital = string,
                    //val_prima_unica_dolares = 0,
                    val_pension_dolares = 0,
                    //val_tasa_dolares = 0,
                    cod_reajuste_pension_1 = poliza.cod_reajuste_pension_1,
                    val_par_reapen_1 = poliza.val_par_reapen_1,
                    cod_reajuste_pension_2 = poliza.cod_reajuste_pension_2,
                    val_par_reapen_2 = poliza.val_par_reapen_2,
                    //val_pension_dolares_original = 0,
                    fec_sol_pension = poliza.fec_sol_pension,
                    fec_recepcion = poliza.fec_recepcion,
                    cod_categoria = poliza.cod_categoria,
                    cod_cia_origen = poliza.cod_cia_seguro,
                    num_poliza_origen = poliza.num_poliza,
                    val_mto_cta_individual = poliza.val_mto_cta_individual,
                    cod_tipo_pension_inicial = poliza.cod_tipo_pension,
                    num_meses_ajuste = 3,
                    aud_usr_ingreso = usuario,
                };

                data = JsonConvert.SerializeObject(objEndosoPoliza);
                log.Info($"data: {data}");

                resultado = BackendAPI.PostAsync(url, objEndosoPoliza, usuario, token);
                log.Info($"resultado: {resultado}");

                // Insertar endosos de personas
                url = $"{urlRviadm}/endosos/inserta-endoso-persona";
                log.Info($"Endoso persona: [{url}]");

                foreach (var beneficiario in beneficiarios)
                {
                    object objPersona = new
                    {
                        aud_usr_ingreso = usuario,
                        cod_cartera = "RVI",
                        num_poliza = beneficiario.num_poliza,
                        num_endoso = 0,
                        cod_tipo_identificacion = beneficiario.cod_tipo_identificacion,
                        num_identificacion = beneficiario.num_identificacion,
                        fec_nacimiento = beneficiario.fec_nacimiento,
                        cod_sexo = beneficiario.cod_sexo,
                        ind_invalidez = beneficiario.ind_invalidez,
                        cod_tipo_invalidez = beneficiario.cod_tipo_invalidez,
                        fec_invalidez = beneficiario.fec_invalidez,
                        ape_paterno = beneficiario.ape_paterno.ToUpper(),
                        ape_materno = beneficiario.ape_materno.ToUpper(),
                        nom_persona = beneficiario.nom_persona.ToUpper(),
                        gls_persona = beneficiario.gls_persona.ToUpper(),
                        cod_nacionalidad = beneficiario.cod_equivalencia_nacionalidad,
                        fec_fallecimiento = beneficiario.fec_fallecimiento,
                        cod_pais_origen_doc = beneficiario.cod_pais_origen_doc,
                        cod_tipo_identificacion_pdt = beneficiario.cod_tipo_identificacion_pdt,
                        num_identificacion_pdt = beneficiario.num_identificacion_pdt,
                        fec_fin_vigenciaCS = beneficiario.cod_tipo_identificacion == "D" ? (DateTime?)new DateTime(2200, 12, 31) : null,
                        cod_user_fecfinCS = beneficiario.cod_tipo_identificacion == "D" ? usuario : null,
                    };

                    data = JsonConvert.SerializeObject(objPersona);
                    log.Info($"data: {data}");

                    resultado = BackendAPI.PostAsync(url, objPersona, usuario, token);
                    log.Info($"resultado: {resultado}");
                }

                // Insertar endosos de beneficiarios
                url = $"{urlRviadm}/endosos/inserta-endoso-beneneficiario";
                log.Info($"Endoso beneficiario: [{url}]");

                foreach (var beneficiario in beneficiarios)
                {
                    object objBeneficiario = new
                    {
                        cod_cartera = "RVI",
                        num_poliza = beneficiario.num_poliza,
                        num_item = beneficiario.num_correlativo,
                        num_endoso = 0,
                        cod_tipo_identificacion = beneficiario.cod_tipo_identificacion,
                        num_identificacion = beneficiario.num_identificacion,

                        cod_tipo_id_apoderado = beneficiario.cod_tipo_identificacion_apoderado,
                        num_id_apoderado = beneficiario.num_identificacion_apoderado,
                        cod_parentesco = beneficiario.cod_parentezco,
                        gls_beneficiario = beneficiario.gls_persona.ToUpper(),
                        fec_inicio_vigencia = poliza.fec_devengue,
                        fec_fin_vigencia =
                            (beneficiario.cod_parentezco == Enums.Parentesco.Hijo.StringValue() && beneficiario.ind_invalidez == Enums.Invalidez.No.StringValue())
                            ? (
                                poliza.fec_sol_pension >= new DateTime(2013, 08, 01)
                                ? beneficiario.fec_nacimiento.AddYears(28)
                                : beneficiario.fec_nacimiento.AddYears(18)
                            ).AddDays(-1)
                            : new DateTime(2200, 12, 31),
                        cod_estado_beneficiario =
                            beneficiario.cod_parentezco != Enums.Parentesco.Afiliado.StringValue()
                            ? "SUSP"
                            : (poliza.cod_tipo_pension == Enums.TipoPension.Sobrevivencia.StringValue()
                                ? "TERM"
                                : (poliza.ind_modalidad != Enums.Modalidad.Diferida.StringValue()
                                    ? "SUSP"
                                    : (poliza.fec_devengue.AddMonths(poliza.val_per_temporal) > poliza.fec_emision_poliza
                                        ? "ACTI"
                                        : "SUSP"
                                    )
                                )
                            ),
                        cod_causal_estado =
                            beneficiario.cod_parentezco != Enums.Parentesco.Afiliado.StringValue()
                            ? "PENACTI"
                            : (poliza.cod_tipo_pension == Enums.TipoPension.Sobrevivencia.StringValue()
                                ? "MUERTE"
                                : (poliza.ind_modalidad != Enums.Modalidad.Diferida.StringValue()
                                    ? "PENACTI"
                                    : (poliza.fec_devengue.AddMonths(poliza.val_per_temporal) > poliza.fec_emision_poliza
                                        ? "ACTIVO"
                                        : "PENACTI"
                                    )
                                )
                            ),
                        fec_pagar_desde =
                            (poliza.cod_tipo_pension != Enums.TipoPension.Sobrevivencia.StringValue() && beneficiario.cod_parentezco != Enums.Parentesco.Afiliado.StringValue())
                            ? (DateTime?)null
                            : (
                                poliza.ind_modalidad == Enums.Modalidad.Diferida.StringValue()
                                ? poliza.fec_devengue.AddMonths(poliza.val_per_temporal)
                                : poliza.fec_devengue
                            ),
                        fec_suspension_pago =
                            (beneficiario.cod_parentezco == Enums.Parentesco.Hijo.StringValue() && beneficiario.ind_invalidez == Enums.Invalidez.No.StringValue())
                            ? (
                                poliza.fec_sol_pension >= new DateTime(2013, 08, 01)
                                ? beneficiario.fec_nacimiento.AddYears(28)
                                : beneficiario.fec_nacimiento.AddYears(18)
                            ).AddDays(-1)
                            : new DateTime(2200, 12, 31),
                        pje_pension = beneficiario.pje_pension,
                        pje_adicional = 0,

                        cod_tipo_pacto_salud = beneficiario.cod_tipo_pacto_salud,
                        val_pactado_salud = 0,
                        num_via_pago = beneficiario.num_via_pago,

                        cod_docparentesco = beneficiario.cod_docparentesco,
                        num_docparentesco = beneficiario.num_docparentesco,
                        gls_mail = beneficiario.gls_mail,
                        ind_mail_autorizado = 'S',

                        aud_usr_ingreso = usuario,
                    };

                    data = JsonConvert.SerializeObject(objBeneficiario);
                    log.Info($"data: {data}");

                    resultado = BackendAPI.PostAsync(url, objBeneficiario, usuario, token);
                    log.Info($"resultado: {resultado}");
                }

                // Insertar endosos de direccion
                url = $"{urlRviadm}/endosos/direccion";
                log.Info($"Endoso direccion: [{url}]");

                foreach (var beneficiario in beneficiarios)
                {
                    if (beneficiario.num_identificacion != 0)
                    {
                        object objDireccion = new
                        {
                            cod_cartera = "RVI",
                            num_poliza = beneficiario.num_poliza,
                            num_endoso = 0,
                            cod_tipo_identificacion = beneficiario.cod_tipo_identificacion,
                            num_identificacion = beneficiario.num_identificacion,
                            num_direccion_envio = 1,
                            gls_direccion = beneficiario.gls_direccion,//rviadm concatena su propio gls_direccion
                            cod_comuna = beneficiario.cod_comuna,
                            cod_ciudad = beneficiario.cod_ciudad,
                            num_telefono = beneficiario.num_celular,
                            ind_vigencia = beneficiario.ind_vigencia,
                            //fec_actualizacion = (DateTime?) null,
                            cod_tipo_via = beneficiario.cod_tipo_via_rviadm,
                            gls_nom_via = beneficiario.gls_nom_via,
                            gls_num_via = beneficiario.gls_num_via,
                            gls_num_interior = beneficiario.gls_num_interior,
                            cod_tipo_zona = beneficiario.cod_tipo_zona,
                            gls_nom_zona = beneficiario.gls_nom_zona,
                            gls_referencia = beneficiario.gls_referencia,
                            cod_larga_distancia = beneficiario.cod_larga_distancia,
                            gls_departamento = beneficiario.gls_departamento,
                            gls_manzana = beneficiario.gls_manzana,
                            gls_lote = beneficiario.gls_lote,
                            gls_kilometro = beneficiario.gls_kilometro,
                            gls_block = beneficiario.gls_block,
                            gls_etapa = beneficiario.gls_etapa,
                            aud_usr_ingreso = usuario
                        };

                        data = JsonConvert.SerializeObject(objDireccion);
                        log.Info($"data: {data}");

                        resultado = BackendAPI.PostAsync(url, objDireccion, usuario, token);
                        log.Info($"resultado: {resultado}");
                    }
                }

                // Insertar apoderado
                url = $"{urlRviadm}/beneficiarios/apoderado";
                log.Info($"Apoderado: [{url}]");

                foreach (var beneficiario in beneficiarios)
                {
                    if (!string.IsNullOrEmpty(beneficiario.cod_tipo_identificacion_apoderado) && !string.IsNullOrEmpty(beneficiario.num_identificacion_apoderado))
                    {
                        object objApoderado = new
                        {
                            cod_cartera = "RVI",
                            cod_tipo_identificacion = beneficiario.cod_tipo_identificacion_apoderado,
                            num_identificacion = beneficiario.num_identificacion_apoderado,
                            //gls_persona = string,
                            num_poliza = beneficiario.num_poliza,
                            num_item = beneficiario.num_correlativo,
                            cod_tipo_apoderado = "02",
                            fec_inicio_vigencia = poliza.fec_devengue,
                            fec_fin_vigencia = (beneficiario.cod_parentezco == Enums.Parentesco.Hijo.StringValue() && beneficiario.ind_invalidez == Enums.Invalidez.No.StringValue())
                                                ? (
                                                    poliza.fec_sol_pension >= new DateTime(2013, 08, 01)
                                                    ? beneficiario.fec_nacimiento.AddYears(28)
                                                    : beneficiario.fec_nacimiento.AddYears(18)
                                                ).AddDays(-1)
                                                : new DateTime(2200, 12, 31),
                            num_via_pago = 0,
                            cod_sucursal_pago = "",
                            cod_banco = "",
                            num_cuenta_cte = "",
                            aud_usr_ingreso = usuario
                        };

                        data = JsonConvert.SerializeObject(objApoderado);
                        log.Info($"data: {data}");

                        resultado = BackendAPI.PostAsync(url, objApoderado, usuario, token);
                        log.Info($"resultado: {resultado}");
                    }
                }

                #endregion

                #region Generar poliza

                // Generar póliza, confirmar endoso cero.
                url = $"{urlRviadm}/endosos/generar-poliza";
                log.Info($"Generar póliza: [{url}]");

                object objPoliza = new
                {
                    cod_cartera = "RVI",
                    num_poliza = poliza.num_poliza,
                    num_endoso = 0,
                    //cod_tipo_identificacion = poliza.cod_tipo_identificacion,
                    //num_identificacion = poliza.num_identificacion,
                    aud_usr_ingreso = usuario,
                };

                data = JsonConvert.SerializeObject(objPoliza);
                log.Info($"data: {data}");

                resultado = BackendAPI.PostAsync(url, objPoliza, usuario, token);
                log.Info($"resultado: {resultado}");

                #endregion

                respuesta.Estado = Constante.COD_OK;
                respuesta.Mensaje = "Transferencia de datos exitosa.";
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Mensaje.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Exito.StringValue();

            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                respuesta.Estado = Constante.COD_ERROR;
                respuesta.Mensaje = "<div style=\"margin: 5px 0\"><b>No se ha podido completar el proceso debido al siguiente error:</b></div>" + Utilitarios.FormatearError(new List<String> { ex.Message });
                respuesta.Titulo = Enums.CuadroMensajeTitulo.Error.StringValue();
                respuesta.Icono = Enums.CuadroMensajeIcono.Error.StringValue();
            }

            return respuesta;
        }

        public List<FirmaDigitalDashboard> ObtenerFirmaDigitalesDashboard(string fechaInicio, string fechaFin, string usuario)
        {
            var listaFirmaDigitalesDashboard = repositorioReportes.ObtenerFirmaDigitalesDashboard(fechaInicio, fechaFin, usuario);
            return listaFirmaDigitalesDashboard;
        }

        public List<PolizasDashboard> ObtenerPolizasDashboard(string fechaInicio, string fechaFin, string usuario)
        {
            var listaPolizasDashboard = repositorioReportes.ObtenerPolizasDashboard(fechaInicio, fechaFin, usuario);
            return listaPolizasDashboard;
        }

        public List<ReporteTrazabilidad> ObtenerTrazabilidad(string fechaInicio, string fechaFin, string idProceso, string usuario)
        {
            var listaTrazabilidad = repositorioReportes.ObtenerTrazabilidad(fechaInicio, fechaFin, idProceso, usuario);
            return listaTrazabilidad;
        }

        public void GenerarPolizaRVI(List<Solicitud> solicitudes, string usuario)
        {
            try
            {

                foreach (var itemSolicitud in solicitudes)
                {
                    repositorioSolicitud.GenerarPolizaRVI(itemSolicitud.Id, usuario);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public byte[] ObtenerReporteRecalculoPDF(List<ReporteRecalculoCotizacion> listaRecalculoCotizacion, string PdfRecalculoTemplate, string RutaLogo)
        {
            try
            {
                log.Debug("Inicio ObtenerReporteRecalculoPDF");

                ReporteRecalculoCotizacion reporteRecalculoCotizacion = repositorioReportes.ObtenerReporteRecalculo(listaRecalculoCotizacion);

                // Open the file to read from.
                string readHtml = File.ReadAllText(PdfRecalculoTemplate);

                readHtml = readHtml.Replace("{ruta_logo}", RutaLogo);

                readHtml = readHtml.Replace("{fecha}", DateTime.Now.ToString("dd/MM/yyyy"));
                readHtml = readHtml.Replace("{hora}", DateTime.Now.ToString("h:mm tt"));

                if (reporteRecalculoCotizacion.fec_cotizacion != null)
                {
                    readHtml = readHtml.Replace("{fechaCotizacion}", reporteRecalculoCotizacion.fec_cotizacion.Value.ToString("dd/MM/yyyy"));
                }
                else
                {
                    readHtml = readHtml.Replace("{fechaCotizacion}", string.Empty);
                }

                if (reporteRecalculoCotizacion.fec_devengue != null)
                {
                    readHtml = readHtml.Replace("{fechaDevengue}", reporteRecalculoCotizacion.fec_devengue.Value.ToString("dd/MM/yyyy"));
                }
                else
                {
                    readHtml = readHtml.Replace("{fechaDevengue}", string.Empty);
                }

                //1° sección
                if (reporteRecalculoCotizacion.num_solicitud != null)
                {
                    readHtml = readHtml.Replace("{solicitud}", reporteRecalculoCotizacion.num_solicitud);
                }
                else
                {
                    readHtml = readHtml.Replace("{solicitud}", string.Empty);
                }

                if (reporteRecalculoCotizacion.num_cuispp != null)
                {
                    readHtml = readHtml.Replace("{cuspp}", (reporteRecalculoCotizacion.num_cuispp));
                }
                else
                {
                    readHtml = readHtml.Replace("{cuspp}", string.Empty);
                }

                if (reporteRecalculoCotizacion.nom_agente != null)
                {
                    readHtml = readHtml.Replace("{vendedor}", (reporteRecalculoCotizacion.num_vendedor + " - " + reporteRecalculoCotizacion.nom_agente.ToUpper()));
                }
                else
                {
                    readHtml = readHtml.Replace("{vendedor}", string.Empty);
                }

                if (reporteRecalculoCotizacion.gls_cartera != null)
                {
                    readHtml = readHtml.Replace("{carteraAsignada}", (reporteRecalculoCotizacion.cod_cartera + " - " + reporteRecalculoCotizacion.gls_cartera.ToUpper()));
                }
                else
                {
                    readHtml = readHtml.Replace("{carteraAsignada}", string.Empty);
                }

                if (reporteRecalculoCotizacion.gls_persona != null)
                {
                    readHtml = readHtml.Replace("{nombreAfiliado}", reporteRecalculoCotizacion.gls_persona.ToUpper());
                }
                else
                {
                    readHtml = readHtml.Replace("{nombreAfiliado}", string.Empty);
                }

                if (reporteRecalculoCotizacion.direccion != null && reporteRecalculoCotizacion.ubigeo != null)
                {
                    readHtml = readHtml.Replace("{direccionParticular}", reporteRecalculoCotizacion.direccion.ToUpper() + " - " + reporteRecalculoCotizacion.ubigeo.ToUpper());
                }
                else
                {
                    readHtml = readHtml.Replace("{direccionParticular}", string.Empty);
                }

                if (reporteRecalculoCotizacion.gls_celular != null)
                {
                    readHtml = readHtml.Replace("{celular}", reporteRecalculoCotizacion.gls_celular);
                }
                else
                {
                    readHtml = readHtml.Replace("{celular}", string.Empty);
                }

                if (reporteRecalculoCotizacion.gls_afp != null)
                {
                    readHtml = readHtml.Replace("{afp}", reporteRecalculoCotizacion.gls_afp.ToUpper());
                }
                else
                {
                    readHtml = readHtml.Replace("{afp}", string.Empty);
                }

                if (reporteRecalculoCotizacion.gls_tipo_pension != null)
                {
                    readHtml = readHtml.Replace("{tipoPension}", reporteRecalculoCotizacion.gls_tipo_pension.ToUpper());
                }
                else
                {
                    readHtml = readHtml.Replace("{tipoPension}", string.Empty);
                }

                if (reporteRecalculoCotizacion.gls_categoria != null)
                {
                    readHtml = readHtml.Replace("{categoriaCliente}", reporteRecalculoCotizacion.gls_categoria.ToUpper());
                }
                else
                {
                    readHtml = readHtml.Replace("{categoriaCliente}", string.Empty);
                }

                if (reporteRecalculoCotizacion.gls_tipo_cotizacion != null)
                {
                    readHtml = readHtml.Replace("{tipoCotizacion}", reporteRecalculoCotizacion.gls_tipo_cotizacion.ToUpper());
                }
                else
                {
                    readHtml = readHtml.Replace("{tipoCotizacion}", string.Empty);
                }

                if (reporteRecalculoCotizacion.val_moneda != null)
                {
                    readHtml = readHtml.Replace("{tipoCambio}", reporteRecalculoCotizacion.val_moneda.Value.ToString());
                }
                else
                {
                    readHtml = readHtml.Replace("{tipoCambio}", string.Empty);
                }

                if (reporteRecalculoCotizacion.tasaAssetshare != null)
                {
                    readHtml = readHtml.Replace("{tasaAssetshare}", reporteRecalculoCotizacion.tasaAssetshare.Value.ToString("#,##0.00"));
                }
                else
                {
                    readHtml = readHtml.Replace("{tasaAssetshare}", string.Empty);
                }

                if (reporteRecalculoCotizacion.TRA != null)
                {
                    readHtml = readHtml.Replace("{TRA}", reporteRecalculoCotizacion.TRA.Value.ToString("#,##0.00"));
                }
                else
                {
                    readHtml = readHtml.Replace("{TRA}", string.Empty);
                }

                if (reporteRecalculoCotizacion.val_mto_cta_individual != null)
                {
                    readHtml = readHtml.Replace("{capitalPension}", reporteRecalculoCotizacion.val_mto_cta_individual.Value.ToString("#,##0.00") + " " + reporteRecalculoCotizacion.gls_corta_moneda.ToUpper());
                }
                else
                {
                    readHtml = readHtml.Replace("{capitalPension}", string.Empty);
                }

                if (reporteRecalculoCotizacion.val_mto_cia != null)
                {
                    readHtml = readHtml.Replace("{montoTransfer}", reporteRecalculoCotizacion.val_mto_cia.Value.ToString("#,##0.00"));
                }
                else
                {
                    readHtml = readHtml.Replace("{montoTransfer}", string.Empty);
                }

                if (reporteRecalculoCotizacion.fec_ult_actualizacion != null)
                {
                    readHtml = readHtml.Replace("{ultimaActualizacion}", reporteRecalculoCotizacion.fec_ult_actualizacion.Value.ToString("dd/MM/yyyy"));
                }
                else
                {
                    readHtml = readHtml.Replace("{ultimaActualizacion}", string.Empty);
                }

                if (reporteRecalculoCotizacion.val_tasa_int_temp != null)
                {
                    readHtml = readHtml.Replace("{tasaAFP}", reporteRecalculoCotizacion.val_tasa_int_temp.Value.ToString("#,##0.00"));
                }
                else
                {
                    readHtml = readHtml.Replace("{tasaAFP}", string.Empty);
                }

                if (reporteRecalculoCotizacion.ajtMon != null)
                {
                    readHtml = readHtml.Replace("{ajustePension}", reporteRecalculoCotizacion.ajtMon.Value.ToString() + "%");
                }
                else
                {
                    readHtml = readHtml.Replace("{ajustePension}", string.Empty);
                }

                if (reporteRecalculoCotizacion.tasaVenta != null)
                {
                    readHtml = readHtml.Replace("{tasaVenta}", reporteRecalculoCotizacion.tasaVenta.Value.ToString("#,##0.00"));
                }
                else
                {
                    readHtml = readHtml.Replace("{tasaVenta}", string.Empty);
                }

                if (reporteRecalculoCotizacion.CRU != null)
                {
                    readHtml = readHtml.Replace("{CRU}", reporteRecalculoCotizacion.CRU.Value.ToString("#,##0.00"));
                }
                else
                {
                    readHtml = readHtml.Replace("{CRU}", string.Empty);
                }

                //2° sección   
                if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle != null)
                {

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.num_correlativo != null)
                    {
                        readHtml = readHtml.Replace("{nroCot}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.num_correlativo.Value.ToString());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{nroCot}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.gls_corta_tipo_pro != null)
                    {
                        readHtml = readHtml.Replace("{rentaVitalicia}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.gls_corta_tipo_pro.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{rentaVitalicia}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.modalidad != null)
                    {
                        readHtml = readHtml.Replace("{modalidad}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.modalidad.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{modalidad}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.val_per_temporal != null)
                    {
                        readHtml = readHtml.Replace("{anios}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.val_per_temporal.Value.ToString());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{anios}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.val_pje_rent_temp != null)
                    {
                        readHtml = readHtml.Replace("{prtas}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.val_pje_rent_temp.Value.ToString("#,##0.00"));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{prtas}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.val_per_garantizado != null)
                    {
                        readHtml = readHtml.Replace("{PG}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.val_per_garantizado.Value.ToString());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{PG}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.ind_derecho_crecer != null)
                    {
                        readHtml = readHtml.Replace("{DC}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.ind_derecho_crecer.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{DC}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.ind_gratificacion != null)
                    {
                        readHtml = readHtml.Replace("{GR}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.ind_gratificacion.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{GR}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.AFPPension != null)
                    {
                        readHtml = readHtml.Replace("{AFPPension}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.AFPPension.Value.ToString("#,##0.00"));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{AFPPension}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.gls_corta_moneda != null)
                    {
                        readHtml = readHtml.Replace("{moneda}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.gls_corta_moneda.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{moneda}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.pension != null)
                    {
                        readHtml = readHtml.Replace("{pension}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.pension.Value.ToString("#,##0.00"));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{pension}", string.Empty);
                    }

                    if (reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.capital != null)
                    {
                        readHtml = readHtml.Replace("{pCapital}", reporteRecalculoCotizacion.reporteRecalculoCotizacionDetalle.capital.Value.ToString());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{pCapital}", string.Empty);
                    }

                }

                //3° sección
                string detalleBeneficiario = "";
                string itemDetalleBeneficiario =
                            @"
                              <tr>
                                <td width='30'>
                                    {nro}
                                </td>
                                <td width='100'>
                                    {parentesco}
                                </td>
				                <td width='80'>
                                    {fechaNacimiento}
                                </td>
                                <td width='50'>
                                    {sexo}
                                </td>
				                <td width='50'>
                                    {invalidez}
                                </td>
                                <td width='80'>
                                    {tipoInvalidez}
                                </td>
				                <td width='220'>
                                    {nombreBeneficiario}
                                </td>
                              </tr>";

                foreach (var itemBeneficiario in reporteRecalculoCotizacion.listaReporteRecalculoCotizacionBeneficiarios)
                {
                    string detalle = itemDetalleBeneficiario;

                    if (itemBeneficiario.num_correlativo != null)
                    {
                        detalle = detalle.Replace("{nro}", itemBeneficiario.num_correlativo.Value.ToString());
                    }
                    else
                    {
                        detalle = detalle.Replace("{nro}", string.Empty);
                    }

                    if (itemBeneficiario.gls_corta_parentezco != null)
                    {
                        detalle = detalle.Replace("{parentesco}", itemBeneficiario.gls_corta_parentezco.ToUpper());
                    }
                    else
                    {
                        detalle = detalle.Replace("{parentesco}", string.Empty);
                    }

                    if (itemBeneficiario.fec_nacimiento != null)
                    {
                        detalle = detalle.Replace("{fechaNacimiento}", itemBeneficiario.fec_nacimiento.Value.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        detalle = detalle.Replace("{fechaNacimiento}", string.Empty);
                    }

                    if (itemBeneficiario.cod_sexo != null)
                    {
                        detalle = detalle.Replace("{sexo}", itemBeneficiario.cod_sexo.ToUpper());
                    }
                    else
                    {
                        detalle = detalle.Replace("{sexo}", string.Empty);
                    }

                    if (itemBeneficiario.ind_invalidez != null)
                    {
                        detalle = detalle.Replace("{invalidez}", itemBeneficiario.ind_invalidez.ToUpper());
                    }
                    else
                    {
                        detalle = detalle.Replace("{invalidez}", string.Empty);
                    }

                    if (itemBeneficiario.gls_corta_tipinv != null)
                    {
                        detalle = detalle.Replace("{tipoInvalidez}", itemBeneficiario.gls_corta_tipinv.ToUpper());
                    }
                    else
                    {
                        detalle = detalle.Replace("{tipoInvalidez}", string.Empty);
                    }

                    if (itemBeneficiario.gls_persona != null)
                    {
                        detalle = detalle.Replace("{nombreBeneficiario}", itemBeneficiario.gls_persona.ToUpper());
                    }
                    else
                    {
                        detalle = detalle.Replace("{nombreBeneficiario}", string.Empty);
                    }

                    detalleBeneficiario = detalleBeneficiario + detalle;
                }

                readHtml = readHtml.Replace("{Beneficiarios}", detalleBeneficiario);


                byte[] bytes;

                using (var ms = new MemoryStream())
                {
                    //Create an iTextSharp Document which is an abstraction of a PDF but **NOT** a PDF
                    using (var doc = new Document(PageSize.A4, 70f, 70f, 60f, 5f))
                    {
                        //Create a writer that's bound to our PDF abstraction and our stream
                        using (var writer = PdfWriter.GetInstance(doc, ms))
                        {
                            //Open the document for writing
                            doc.Open();

                            //XMLWorker also reads from a TextReader and not directly from a string
                            using (var srHtml = new StringReader(readHtml))
                            {
                                //Parse the HTML
                                iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml);
                            }
                            doc.Close();
                        }
                    }

                    //After all of the PDF "stuff" above is done and closed but **before** we
                    //close the MemoryStream, grab all of the active bytes from the stream
                    bytes = ms.ToArray();
                }

                log.Debug("Fin ObtenerReporteRecalculoPDF");

                return bytes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public byte[] ObtenerReporteCotizacionesGanadasPDF(int numLote, string PdfRecalculoTemplate, string RutaLogo)
        {
            try
            {
                log.Debug("Inicio ObtenerReporteCotizacionesGanadasPDF");

                List<ReporteCotizacionesGanadas> lstReporteCotizacionesGanadas = repositorioReportes.ObtenerReporteCotizacionesGanadas(numLote);

                List<string> lstReadHtml = new List<string>();

                foreach (var itemReporteCotizacionesGanadas in lstReporteCotizacionesGanadas)
                {
                    // Open the file to read from.
                    string readHtml = File.ReadAllText(PdfRecalculoTemplate);

                    readHtml = readHtml.Replace("{ruta_logo}", RutaLogo);

                    readHtml = readHtml.Replace("{fecha}", DateTime.Now.ToString("dd/MM/yyyy"));
                    readHtml = readHtml.Replace("{hora}", DateTime.Now.ToString("h:mm tt"));

                    readHtml = readHtml.Replace("{nroOperacion}", itemReporteCotizacionesGanadas.num_operacion.ToString());

                    if (itemReporteCotizacionesGanadas.gls_afp != null)
                    {
                        readHtml = readHtml.Replace("{afp}", itemReporteCotizacionesGanadas.gls_afp.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{afp}", string.Empty);
                    }

                    //1° sección
                    if (itemReporteCotizacionesGanadas.num_cuspp != null)
                    {
                        readHtml = readHtml.Replace("{cuspp}", itemReporteCotizacionesGanadas.num_cuspp);
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{cuspp}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.afiliado != null)
                    {
                        readHtml = readHtml.Replace("{Afiliado}", (itemReporteCotizacionesGanadas.afiliado.ToUpper()));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{Afiliado}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.gls_tipo_identificacion != null)
                    {
                        readHtml = readHtml.Replace("{tipoDocumento}", (itemReporteCotizacionesGanadas.gls_tipo_identificacion.ToUpper()));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{tipoDocumento}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.num_documento != null)
                    {
                        readHtml = readHtml.Replace("{numeroDocumento}", (itemReporteCotizacionesGanadas.num_documento));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{numeroDocumento}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.fec_nacimiento_afiliado != null)
                    {
                        readHtml = readHtml.Replace("{fechaNacimiento}", itemReporteCotizacionesGanadas.fec_nacimiento_afiliado.ToString("dd/MM/yyyy"));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{fechaNacimiento}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.cod_estado_sobrevivencia != null)
                    {
                        readHtml = readHtml.Replace("{estadoSobrevivencia}", itemReporteCotizacionesGanadas.cod_estado_sobrevivencia.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{estadoSobrevivencia}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.cod_genero_afiliado != null)
                    {
                        readHtml = readHtml.Replace("{sexo}", itemReporteCotizacionesGanadas.cod_genero_afiliado.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{sexo}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.cod_Grado_invalidez != null)
                    {
                        readHtml = readHtml.Replace("{gradoInvalidez}", itemReporteCotizacionesGanadas.cod_Grado_invalidez.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{gradoInvalidez}", string.Empty);
                    }

                    //2° sección
                    if (itemReporteCotizacionesGanadas.gls_tipo_pension_sbs != null)
                    {
                        readHtml = readHtml.Replace("{tipoPension}", (itemReporteCotizacionesGanadas.gls_tipo_pension_sbs.ToUpper()));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{tipoPension}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.cod_cambio_modalidad != null)
                    {
                        readHtml = readHtml.Replace("{cambioModalidad}", (itemReporteCotizacionesGanadas.cod_cambio_modalidad.ToUpper()));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{cambioModalidad}", string.Empty);
                    }

                    readHtml = readHtml.Replace("{tasaAFP}", (itemReporteCotizacionesGanadas.val_tasa_rp_rt.ToString("#,##0.00")));

                    if (itemReporteCotizacionesGanadas.fec_envio != null)
                    {
                        readHtml = readHtml.Replace("{fechaEnvio}", (itemReporteCotizacionesGanadas.fec_envio.ToString("dd/MM/yyyy")));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{fechaEnvio}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.fec_devengue != null)
                    {
                        readHtml = readHtml.Replace("{fechaDevengue}", (itemReporteCotizacionesGanadas.fec_devengue.ToString("dd/MM/yyyy")));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{fechaDevengue}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.ind_pension_preliminar != null)
                    {
                        readHtml = readHtml.Replace("{pensionPreliminar}", (itemReporteCotizacionesGanadas.ind_pension_preliminar.ToUpper()));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{pensionPreliminar}", string.Empty);
                    }

                    readHtml = readHtml.Replace("{tipoCambio}", (itemReporteCotizacionesGanadas.val_tipo_cambio.ToString("#,##0.00")));

                    if (itemReporteCotizacionesGanadas.ajusteMoneda != null)
                    {
                        readHtml = readHtml.Replace("{ajusteMoneda}", (itemReporteCotizacionesGanadas.ajusteMoneda.ToUpper()));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{ajusteMoneda}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.fec_cierre != null)
                    {
                        readHtml = readHtml.Replace("{fechaCierre}", (itemReporteCotizacionesGanadas.fec_cierre.ToString("dd/MM/yyyy")));
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{fechaCierre}", string.Empty);
                    }


                    //3° sección
                    readHtml = readHtml.Replace("{capitalPension}", itemReporteCotizacionesGanadas.cod_moneda_fondo.ToString() + " " + itemReporteCotizacionesGanadas.val_capital_pension.ToString("#,##0.00"));

                    readHtml = readHtml.Replace("{valorCuota}", itemReporteCotizacionesGanadas.val_cuota.ToString("#,##0.00"));

                    readHtml = readHtml.Replace("{bonoActualizado}", itemReporteCotizacionesGanadas.val_bono_actualizado.ToString("#,##0.00"));

                    readHtml = readHtml.Replace("{aporteAdicional}", itemReporteCotizacionesGanadas.val_aporte_adicional.ToString("#,##0.00"));

                    if (itemReporteCotizacionesGanadas.gls_compania != null)
                    {
                        readHtml = readHtml.Replace("{compania}", itemReporteCotizacionesGanadas.gls_compania.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{compania}", string.Empty);
                    }

                    readHtml = readHtml.Replace("{saldoCIC}", itemReporteCotizacionesGanadas.val_saldo_cic.ToString("#,##0.00"));

                    readHtml = readHtml.Replace("{saldoCuotas}", itemReporteCotizacionesGanadas.val_saldo_cuota.ToString("#,##0.00"));

                    if (itemReporteCotizacionesGanadas.ind_tiene_cobertura != null)
                    {
                        readHtml = readHtml.Replace("{tieneCobertura}", itemReporteCotizacionesGanadas.ind_tiene_cobertura.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{tieneCobertura}", string.Empty);
                    }

                    readHtml = readHtml.Replace("{tipoCompraAA}", itemReporteCotizacionesGanadas.val_tipo_cambio_compra_AA.ToString("#,##0.00"));

                    //4° sección
                    if (itemReporteCotizacionesGanadas.gls_modalidad != null)
                    {
                        readHtml = readHtml.Replace("{modalidad}", itemReporteCotizacionesGanadas.gls_modalidad.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{modalidad}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.cod_moneda_producto != null)
                    {
                        readHtml = readHtml.Replace("{moneda}", itemReporteCotizacionesGanadas.cod_moneda_producto.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{moneda}", string.Empty);
                    }

                    readHtml = readHtml.Replace("{aniosRT}", itemReporteCotizacionesGanadas.num_anos_RT.ToString());

                    readHtml = readHtml.Replace("{pRVD}", itemReporteCotizacionesGanadas.val_porcentaje_RVD.ToString());

                    readHtml = readHtml.Replace("{PG}", itemReporteCotizacionesGanadas.val_periodo_garantizado.ToString());

                    if (itemReporteCotizacionesGanadas.ind_derecho_crecer != null)
                    {
                        readHtml = readHtml.Replace("{DC}", itemReporteCotizacionesGanadas.ind_derecho_crecer.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{DC}", string.Empty);
                    }

                    if (itemReporteCotizacionesGanadas.ind_gratificacion != null)
                    {
                        readHtml = readHtml.Replace("{GR}", itemReporteCotizacionesGanadas.ind_gratificacion.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{GR}", string.Empty);
                    }

                    readHtml = readHtml.Replace("{pCobConyuge}", itemReporteCotizacionesGanadas.pje_cobertura_conyuge.ToString());

                    if (itemReporteCotizacionesGanadas.cod_particion_capital != null)
                    {
                        readHtml = readHtml.Replace("{pCapital}", itemReporteCotizacionesGanadas.cod_particion_capital.ToUpper());
                    }
                    else
                    {
                        readHtml = readHtml.Replace("{pCapital}", string.Empty);
                    }

                    //5° sección
                    string detalleBeneficiario = "";
                    string itemDetalleBeneficiario =
                                @"
                                  <tr>
                                    <td width='250'>
                                        {beneficiario}
                                    </td>
                                    <td width='60'>
                                        {parentesco}
                                    </td>
                                    <td width='30'>
                                        {invalidez}
                                    </td>
                                    <td width='60'>
                                        {fechaNacimiento}
                                    </td>
                                    <td width='60'>
                                        {sexo}
                                    </td>
                                  </tr>";

                    foreach (var itemBeneficiario in itemReporteCotizacionesGanadas.reporteCotizacionesGanadasBeneficiarios)
                    {
                        string detalle = itemDetalleBeneficiario;

                        if (itemBeneficiario.beneficiario != null)
                        {
                            detalle = detalle.Replace("{beneficiario}", itemBeneficiario.beneficiario.ToUpper());
                        }
                        else
                        {
                            detalle = detalle.Replace("{beneficiario}", string.Empty);
                        }

                        if (itemBeneficiario.gls_parentezco != null)
                        {
                            detalle = detalle.Replace("{parentesco}", itemBeneficiario.gls_parentezco.ToUpper());
                        }
                        else
                        {
                            detalle = detalle.Replace("{parentesco}", string.Empty);
                        }

                        if (itemBeneficiario.cod_condicion_invalidez != null)
                        {
                            detalle = detalle.Replace("{invalidez}", itemBeneficiario.cod_condicion_invalidez.ToUpper());
                        }
                        else
                        {
                            detalle = detalle.Replace("{invalidez}", string.Empty);
                        }

                        if (itemBeneficiario.fec_nacimiento_beneficiario != null)
                        {
                            detalle = detalle.Replace("{fechaNacimiento}", itemBeneficiario.fec_nacimiento_beneficiario.ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            detalle = detalle.Replace("{fechaNacimiento}", string.Empty);
                        }

                        if (itemBeneficiario.cod_genero_beneficiario != null)
                        {
                            detalle = detalle.Replace("{sexo}", itemBeneficiario.cod_genero_beneficiario.ToUpper());
                        }
                        else
                        {
                            detalle = detalle.Replace("{sexo}", string.Empty);
                        }

                        detalleBeneficiario = detalleBeneficiario + detalle;
                    }

                    readHtml = readHtml.Replace("{Beneficiarios}", detalleBeneficiario);

                    lstReadHtml.Add(readHtml);

                } //end foreach


                byte[] bytes;

                using (var ms = new MemoryStream())
                {
                    //Create an iTextSharp Document which is an abstraction of a PDF but **NOT** a PDF
                    using (var doc = new Document(PageSize.A4, 70f, 70f, 60f, 5f))
                    {
                        //Create a writer that's bound to our PDF abstraction and our stream
                        using (var writer = PdfWriter.GetInstance(doc, ms))
                        {
                            //Open the document for writing
                            doc.Open();

                            foreach (var itemReadHtml in lstReadHtml)
                            {

                                //XMLWorker also reads from a TextReader and not directly from a string
                                using (var srHtml = new StringReader(itemReadHtml))
                                {
                                    //Parse the HTML
                                    iTextSharp.tool.xml.XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, srHtml);
                                }

                                doc.NewPage();

                            }

                            doc.Close();
                        }
                    }

                    //After all of the PDF "stuff" above is done and closed but **before** we
                    //close the MemoryStream, grab all of the active bytes from the stream
                    bytes = ms.ToArray();
                }

                log.Debug("Fin ObtenerReporteCotizacionesGanadasPDF");

                return bytes;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public Solicitud ListarCotizacionesPorSolicitud(string numeroSolicitud, string usuario)
        {
            var solicitud = repositorioSolicitud.ListarCotizacionesPorSolicitud(numeroSolicitud, usuario);
            return solicitud;
        }

        public void ActualizarCotizacionGanadoraRVI(string solicitud, int correlativo, string usuario)
        {
            repositorioSolicitud.ActualizarCotizacionGanadoraRVI(solicitud, correlativo, usuario);
        }

        public List<EstadoCivil> ListarEstadoCivil(string usuario)
        {
            var listaEstadosCiviles = repositorioEstadoCivil.Listar(usuario);
            return listaEstadosCiviles;
        }

        public List<Profesion> ListarProfesion(string usuario)
        {
            var listaProfesiones = repositorioProfesion.Listar(usuario);
            return listaProfesiones;
        }

        public List<Nacionalidad> ListarNacionalidad(string usuario)
        {
            var listaNacionalidad = repositorioNacionalidad.Listar(usuario);
            return listaNacionalidad;
        }

        public int ObtenerIndicadorRescateIFP(string solicitud, string usuario)
        {
            int ind_Rescate = repositorioSolicitudIFP.ObtenerIndicadorRescateIFP(solicitud, usuario);
            return ind_Rescate;
        }

        public CotizacionRescate RecotizarSolicitudIFP(string numeroSolicitud, DateTime fecCotizacion, int numNesRescate, string usuario)
        {
            //using (TransactionScope transaccion = new TransactionScope())
            //{
            //repositorioSolicitudIFP.Actualizar(ref solicitud);
            CotizacionMotorIFP CotizacionIFP = RecotizarIFP(numeroSolicitud, fecCotizacion, usuario);

            //numNesRescate
            CotizacionRescate cotizacionRescate = CotizacionIFP.lstRescate.Find(rct => rct.mes_rescate == numNesRescate);

            return cotizacionRescate;

            //transaccion.Complete();
            //}
        }

        private CotizacionMotorIFP RecotizarIFP(string numSolicitud, DateTime fecCotizacion, string usuario)
        {
            try
            {
                log.Debug("Inicio CotizadorServicio.RecotizarIFP");
                //List<ParametrosMotorIFP> lstParametrosMotorIFP = new List<ParametrosMotorIFP>();

                log.Debug("fecCotizacion: " + fecCotizacion.Day + " " + fecCotizacion.Month + " " + fecCotizacion.Year);

                Proxies.MotorIFP.IServicioMotorIFP motorIFP = new Proxies.MotorIFP.ServicioMotorIFPClient("EPMotorIFPws");

                log.Debug("Consulta sp: usp_cwrv_carpr_cotizacion_ifp");
                List<ParametrosMotorIFP> lstParametroCotizacion = repositorioSolicitudIFP.ObtenerParametroCotizaciones(numSolicitud, fecCotizacion, usuario);

                log.Debug("Filtrar parámetro");
                var parametroCotizacion = lstParametroCotizacion.Find(cot => cot.cotizacion.cod_estado_cotizacion == Enums.EstadoCotizacion.Cerrado.StringValue());

                var titular = parametroCotizacion.cotizacion.beneficiarios.Find(ben => ben.cod_parentesco == Enums.Parentesco.Afiliado.StringValue());
                List<BeneficiarioMotorIFP> listaTitular = new List<BeneficiarioMotorIFP>();
                listaTitular.Add(titular);
                parametroCotizacion.cotizacion.beneficiarios = listaTitular;

                //actualizar fecha nacimiento
                List<DatosSol> solicitudRealFechaCotizacion = ObtenerDatosporSolicitudIFP(numSolicitud);
                DateTime cotizacionGanadaRealFechaCotizacion = solicitudRealFechaCotizacion.FirstOrDefault().fec_solicitud;
                //cotizacionGanadaRealFechaCotizacion = DateTime.ParseExact(cotizacionGanadaRealFechaCotizacion.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                //fechas sin horas
                DateTime fechaCotizacionReal = cotizacionGanadaRealFechaCotizacion.Date;
                DateTime fecha_proceso = fecCotizacion.Date;
                //fecha_proceso = DateTime.ParseExact(fecha_proceso.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                var diferenciafechasCotPrc = fecha_proceso.Subtract(fechaCotizacionReal);
                var diferenciadiasCotPrc = diferenciafechasCotPrc.Days;

                DateTime fechaNacimientoTitular = parametroCotizacion.cotizacion.beneficiarios.Find(ben => ben.cod_parentesco == Enums.Parentesco.Afiliado.StringValue()).fec_nacimiento;
                log.Debug("fechaNacimientoTitular: " + fechaNacimientoTitular.Day + " " + fechaNacimientoTitular.Month + " " + fechaNacimientoTitular.Year);

                //fechaNacimientoTitular = DateTime.ParseExact(fechaNacimientoTitular.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);

                log.Debug("fechaNacimientoTitular: " + fechaNacimientoTitular);
                log.Debug("diferenciadiasCotPrc: " + diferenciadiasCotPrc);
                log.Debug("nueva fechaNacimientoTitular: " + fechaNacimientoTitular.AddDays(diferenciadiasCotPrc));

                parametroCotizacion.cotizacion.beneficiarios.Find(ben => ben.cod_parentesco == Enums.Parentesco.Afiliado.StringValue()).fec_nacimiento = fechaNacimientoTitular.AddDays(diferenciadiasCotPrc);

                //carpr
                log.Debug("Consulta sp: usp_cwrv_carpr_solicitud_ifp");
                ParametrosMotorIFP parametroGenerales = repositorioSolicitudIFP.ObtenerParametroGenerales(parametroCotizacion.cotizacion.cod_tipo_temporalidad, parametroCotizacion.cotizacion.cod_moneda, fecCotizacion, true, usuario);

                //foreach (var item in lstParametroCotizacion)
                //{
                CotizacionMotorIFP cotizacionIFP = new CotizacionMotorIFP();
                string parametroGeneralIFP = string.Empty;
                string cotizacion = string.Empty;
                Proxies.MotorIFP.Cotizacion cotizacionMotorIFP = new Proxies.MotorIFP.Cotizacion();
                Proxies.MotorIFP.Parametros parametros = new Proxies.MotorIFP.Parametros();

                log.Debug("Cotización");
                //var par = lstParametrosMotorIFP.Find(p => p.cod_monedaIFP == item.cotizacion.cod_moneda && p.cod_tipo_temporalidadIFP == item.cotizacion.cod_tipo_temporalidad && p.fec_cotizacionIFP == item.cotizacion.fec_cotizacion);
                parametroGenerales.cotizacion = parametroCotizacion.cotizacion;

                log.Debug("Serialize");
                parametroGeneralIFP = new JavaScriptSerializer().Serialize(parametroGenerales);

                log.Debug("Deserilize");
                parametros = new JavaScriptSerializer().Deserialize<Proxies.MotorIFP.Parametros>(parametroGeneralIFP);

                log.Debug("Llenar datos");
                parametros.tipo_calculo = Enums.TipoCalculo.Cotizacion.StringValue();
                parametros.tipo_producto = Enums.TipoProducto.IFP.StringValue();
                parametros.cotizacion.fec_documento = parametroGenerales.cotizacion.fec_cotizacion;
                //parametros.cotizacion.Plan = new Proxies.MotorIFP.Plan { Id = parametroCotizacion.cotizacion.cod_plan.ToString() };

                log.Debug("Cotizar");
                cotizacionMotorIFP = motorIFP.CotizarIFP(parametros);

                log.Debug("Cotizado");
                cotizacion = new JavaScriptSerializer().Serialize(cotizacionMotorIFP);
                cotizacionIFP = new JavaScriptSerializer().Deserialize<CotizacionMotorIFP>(cotizacion);

                if (Math.Round(cotizacionIFP.val_tasa_ret_accion, 2, MidpointRounding.AwayFromZero) < cotizacionIFP.parametro_ash.tas_ltra || Math.Round(cotizacionIFP.val_tasa_ret_accion, 2, MidpointRounding.AwayFromZero) > cotizacionIFP.parametro_ash.tas_htra)
                {
                    cotizacionIFP.num_error_cot = 300;
                    cotizacionIFP.ind_cotiza = false;
                }

                //wl_XML_Cotiza += "<cotiza>";

                //if (cotizacionIFP.lstRescate != null)
                //{
                //    foreach (var itemRescate in cotizacionIFP.lstRescate)
                //    {
                //        wl_XML_Cotiza += "<rescate>";
                //    }
                //}

                log.Debug("Fin CotizadorServicio.RecotizarIFP");
                return cotizacionIFP;

                //}

            }
            catch (Exception)
            {
                throw;
            }
        }

        public SolicitudIFP ValidarCotizacionVigente(string cod_tipo_documento, string num_documento, string usuario)
        {
            var solicitud = repositorioSolicitudIFP.ValidarCotizacionVigente(cod_tipo_documento, num_documento, usuario);
            return solicitud;
        }

        public List<FormatoSolicitud> ListarFormatosSolicitud(string solicitud, string usuario)
        {
            var formatos = repositorioSolicitudRP.ListarFormatosSolicitud(solicitud, usuario);
            return formatos;
        }

        public FormatoSolicitud ObtenerFormatoSolicitud(string solicitud, int idFormatoSolicitud, string usuario)
        {
            var formato = repositorioSolicitudRP.ObtenerFormatoSolicitud(solicitud, idFormatoSolicitud, usuario);
            return formato;
        }

        public List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiario(string solicitud, int idFormatoSolicitud, string usuario)
        {
            var formatos = repositorioSolicitudRP.ListarFormatoSolicitudBeneficiario(solicitud, idFormatoSolicitud, usuario);
            return formatos;
        }

        public List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculada(string solicitud, int idFormatoSolicitud, string usuario)
        {
            var formatos = repositorioSolicitudRP.ListarFormatoSolicitudPersonaVinculada(solicitud, idFormatoSolicitud, usuario);
            return formatos;
        }

        public FormatoSolicitud ObtenerFormatoSolicitudActualizado(string solicitud, string usuario)
        {
            var formato = repositorioSolicitudRP.ObtenerFormatoSolicitudActualizado(solicitud, usuario);
            return formato;
        }

        public List<FormatoSolicitudBeneficiario> ListarFormatoSolicitudBeneficiarioActualizado(string solicitud, string usuario)
        {
            var formatos = repositorioSolicitudRP.ListarFormatoSolicitudBeneficiarioActualizado(solicitud, usuario);
            return formatos;
        }

        public List<FormatoSolicitudPersonaVinculada> ListarFormatoSolicitudPersonaVinculadaActualizado(string solicitud, string usuario)
        {
            var formatos = repositorioSolicitudRP.ListarFormatoSolicitudPersonaVinculadaActualizado(solicitud, usuario);
            return formatos;
        }

        public List<Dominio.Entidades.MotorCalculo.JuegoParametros> ObtenerParametrosRPP(string temporalidad, DateTime fechaCotizacion, string origen, string usuario)
        {
            var juegosParametros = repositorioMotorCalculo.ObtenerParametrosRPP(temporalidad, fechaCotizacion, origen, usuario);
            return juegosParametros;
        }

        public SolicitudRPPlus CotizarRPP(SolicitudRPPlus solicitud, List<Dominio.Entidades.MotorCalculo.Parametros> parametros)
        {
            try
            {
                    string wl_XML_Cotiza = string.Empty
                            , XML_Cotiza = string.Empty;
                    int i;

                    if (string.IsNullOrEmpty(solicitud.Id))
                    {
                        // Registrar la solicitud
                        repositorioSolicitudRPPlus.Registrar(ref solicitud);
                    }
                    else
                    {
                        // Actualizar la solicitud
                        repositorioSolicitudRPPlus.Actualizar(ref solicitud);
                    }

                    List<CotizacionRPPlus> cotizacionesAux = solicitud.Cotizaciones;
                    Usuario usuario = solicitud.Usuario;
                    solicitud = repositorioSolicitudRPPlus.ObtenerDatos(solicitud.Id);
                    i = 0;
                    solicitud.Cotizaciones.ForEach(cotizacion =>
                    {
                        // Se ajustan los valores del PG y Tramo 1 a meses ya que la BD los devuelve como años
                        cotizacion.PeriodoGarantizado *= 12;
                        cotizacion.PagoEscalonada *= 12;

                        // Se cargan el porcentaje de beneficiarios desde el arreglo auxiliar
                        cotizacion.PorcentajeBeneficiarios = cotizacionesAux[i].PorcentajeBeneficiarios;

                        i++;
                    });
                    solicitud.Usuario = usuario;

                    // Calcular cada cotización de la solicitud
                    i = 0;
                    foreach (var parametro in parametros)
                    {
                        parametro.cotizacion.num_solicitud = solicitud.Id;
                        parametro.cotizacion.num_correlativo = solicitud.Cotizaciones[i].Correlativo;
                        parametro.cotizacion = CotizarMotorCalculo(parametro);

                        // Validar si no se encuentra un TRA objetivo entre el LTRA y HTRA
                        //if (Math.Round(parametro.cotizacion.val_tasa_ret_accion, 2, MidpointRounding.AwayFromZero) < parametro.cotizacion.parametro_ash.tas_ltra || 
                        //    Math.Round(parametro.cotizacion.val_tasa_ret_accion, 2, MidpointRounding.AwayFromZero) > parametro.cotizacion.parametro_ash.tas_htra)
                        //{
                        //    parametro.cotizacion.num_error_cot = 300;
                        //    parametro.cotizacion.ind_cotiza = false;
                        //}

                        wl_XML_Cotiza += "<cotiza>";
                        wl_XML_Cotiza += " <num_solicitud>" + parametro.cotizacion.num_solicitud + "</num_solicitud>";
                        wl_XML_Cotiza += " <fec_cotizacion>" + parametro.cotizacion.fec_cotizacion.ToString("dd/MM/yyyy") + "</fec_cotizacion>";
                        wl_XML_Cotiza += " <num_correlativo>" + parametro.cotizacion.num_correlativo.ToString() + "</num_correlativo>";
                        wl_XML_Cotiza += " <cod_estado_cotizacion>" + "02" + "</cod_estado_cotizacion>";
                        wl_XML_Cotiza += " <val_descuento_comision>" + parametro.cotizacion.val_descuento_comision + "</val_descuento_comision>";
                        wl_XML_Cotiza += " <cod_tipo_calculo>" + "2" + "</cod_tipo_calculo>";
                        wl_XML_Cotiza += " <ind_cotiza>" + ((parametro.cotizacion.ind_cotiza) ? "" : "**") + "</ind_cotiza>";
                        wl_XML_Cotiza += " <val_fac_cia>" + parametro.cotizacion.val_fac_cia + "</val_fac_cia>";
                        wl_XML_Cotiza += " <val_mto_cia>" + parametro.cotizacion.val_mto_cia + "</val_mto_cia>";
                        wl_XML_Cotiza += " <val_mto_cia_sin_comision>" + parametro.cotizacion.val_mto_cia_sin_comision + "</val_mto_cia_sin_comision>";
                        wl_XML_Cotiza += " <val_pen_cia>" + parametro.cotizacion.val_renta + "</val_pen_cia>";
                        wl_XML_Cotiza += " <val_pen_cia_mo>" + parametro.cotizacion.val_renta_mo + "</val_pen_cia_mo>";
                        wl_XML_Cotiza += " <val_pen_ref>" + 0 + "</val_pen_ref>";
                        wl_XML_Cotiza += " <val_pen_ref_mo>" + 0 + "</val_pen_ref_mo>";
                        wl_XML_Cotiza += " <val_tasa_int_vit>" + parametro.cotizacion.val_tasa_int_vit + "</val_tasa_int_vit>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash>" + parametro.cotizacion.val_tasa_venta_ash + "</val_tasa_venta_ash>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion>" + parametro.cotizacion.val_tasa_ret_accion + "</val_tasa_ret_accion>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv>" + parametro.cotizacion.val_tasa_costo_equiv + "</val_tasa_costo_equiv>";
                        wl_XML_Cotiza += " <val_duration>" + 0 + "</val_duration>";
                        wl_XML_Cotiza += " <num_error_cot>" + parametro.cotizacion.num_error_cot + "</num_error_cot>";
                        wl_XML_Cotiza += " <val_tasa_int_temp>" + parametro.cotizacion.val_tasa_int_temp + "</val_tasa_int_temp>";
                        wl_XML_Cotiza += " <val_fac_afp>" + 0 + "</val_fac_afp>";
                        wl_XML_Cotiza += " <val_mto_afp>" + 0 + "</val_mto_afp>";
                        wl_XML_Cotiza += " <val_pen_afp>" + 0 + "</val_pen_afp>";
                        wl_XML_Cotiza += " <val_afp_pen_ref>" + 0 + "</val_afp_pen_ref>";
                        wl_XML_Cotiza += " <val_tasa_venta_ash_2>" + 0 + "</val_tasa_venta_ash_2>";
                        wl_XML_Cotiza += " <val_tasa_ret_accion_2>" + 0 + "</val_tasa_ret_accion_2>";
                        wl_XML_Cotiza += " <val_tasa_costo_equiv_2>" + 0 + "</val_tasa_costo_equiv_2>";
                        wl_XML_Cotiza += " <val_duration_2>" + 0 + "</val_duration_2>";
                        wl_XML_Cotiza += " <num_error_cot_2>" + 0 + "</num_error_cot_2>";
                        wl_XML_Cotiza += " <val_tasa_cesion>" + parametro.cotizacion.val_tasa_cesion + "</val_tasa_cesion>";
                        wl_XML_Cotiza += " <val_tasa_cesion_moneda2>" + 0 + "</val_tasa_cesion_moneda2>";
                        wl_XML_Cotiza += " <cod_moneda>" + parametro.cotizacion.cod_moneda + "</cod_moneda>";
                        wl_XML_Cotiza += " <wl_cod_username>" + solicitud.Usuario.NombreUsuario + "</wl_cod_username>";
                        wl_XML_Cotiza += " <val_total_garantizado>" + parametro.cotizacion.val_total_garantizado + "</val_total_garantizado>";
                        wl_XML_Cotiza += " <val_1era_prima_is>" + parametro.cotizacion.val_1era_prima_is + "</val_1era_prima_is>";
                        wl_XML_Cotiza += " <val_fac_dev>" + parametro.cotizacion.val_fac_dev + "</val_fac_dev>";
                        wl_XML_Cotiza += " <val_mto_dev>" + parametro.cotizacion.val_mto_dev + "</val_mto_dev>";

                        if (parametro.cotizacion.reserva != null)
                        {
                            //Campos vienen del motor
                            wl_XML_Cotiza += " <val_res_pension>" + parametro.cotizacion.reserva.val_prima_unica_pension + "</val_res_pension>";
                            wl_XML_Cotiza += " <val_res_sepelio>" + parametro.cotizacion.reserva.val_prima_unica_sepelio + "</val_res_sepelio>";
                            wl_XML_Cotiza += " <val_res_devolucion>" + parametro.cotizacion.reserva.val_prima_unica_devolucion + "</val_res_devolucion>";
                            wl_XML_Cotiza += " <val_res_fallecimiento>" + parametro.cotizacion.reserva.val_prima_unica_fallecimiento + "</val_res_fallecimiento>";                            
                        }
                        wl_XML_Cotiza += " <val_para_duration>" + parametro.cotizacion.val_para_duration + "</val_para_duration>";
                        wl_XML_Cotiza += " <val_1era_renta_is>" + parametro.cotizacion.val_renta_sin_diferimiento + "</val_1era_renta_is>";
                        wl_XML_Cotiza += "</cotiza>";

                        if (parametro.cotizacion.lstRescate != null)
                        {
                            foreach (var itemRescate in parametro.cotizacion.lstRescate)
                            {
                                wl_XML_Cotiza += "<rescate>";
                                wl_XML_Cotiza += " <num_solicitud>" + parametro.cotizacion.num_solicitud + "</num_solicitud>";
                                wl_XML_Cotiza += " <num_correlativo>" + parametro.cotizacion.num_correlativo.ToString() + "</num_correlativo>";
                                wl_XML_Cotiza += " <num_mes_rescate>" + itemRescate.mes_rescate + "</num_mes_rescate>";
                                wl_XML_Cotiza += " <val_rescate>" + itemRescate.valor_rescate + "</val_rescate>";
                                wl_XML_Cotiza += " <val_tasa_rescate_mensual>" + itemRescate.tasa_rescate_mensual + "</val_tasa_rescate_mensual>";
                                wl_XML_Cotiza += " <val_tasa_rescate_anual>" + itemRescate.tasa_rescate_anual + "</val_tasa_rescate_anual>";
                                wl_XML_Cotiza += "</rescate>";
                            }
                        }

                        if (parametro.cotizacion.ind_cotiza)
                        {
                            solicitud.Cotizaciones[i].PensionCia = (double)parametro.cotizacion.val_renta;
                            solicitud.Cotizaciones[i].PensionCiaMO = parametro.cotizacion.val_renta_mo;
                            solicitud.Cotizaciones[i].ValTasaCostoEquiv = parametro.cotizacion.val_tasa_costo_equiv;
                            solicitud.Cotizaciones[i].TasaVenta = parametro.cotizacion.val_tasa_int_vit;
                            solicitud.Cotizaciones[i].TasaVentaSbs = parametro.cotizacion.val_tasa_venta_ash;
                            solicitud.Cotizaciones[i].TasaRetornoAccionista = parametro.cotizacion.val_tasa_ret_accion;
                            solicitud.Cotizaciones[i].Pension2doTramo = parametro.cotizacion.val_1era_prima_is;
                            solicitud.Cotizaciones[i].Pension2doTramoSinAjuste = solicitud.Cotizaciones[i].PensionCia * (solicitud.Cotizaciones[i].PjePE / 100);
                        }
                        solicitud.Cotizaciones[i].IndCotiza = parametro.cotizacion.ind_cotiza ? "" : "**";
                        solicitud.Cotizaciones[i].IndErrorCotiza = parametro.cotizacion.num_error_cot;

                        XML_Cotiza += wl_XML_Cotiza;
                        wl_XML_Cotiza = string.Empty;
                        i++;
                    }

                    repositorioSolicitudRPPlus.RegistrarPjeBen(solicitud);
                    repositorioSolicitudRPPlus.RegistrarCotiza("<insert>" + XML_Cotiza + "</insert>", solicitud.Usuario.NombreUsuario);
                
                return solicitud;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Dominio.Entidades.MotorCalculo.Cotizacion CotizarMotorCalculo(Dominio.Entidades.MotorCalculo.Parametros parametros)
        {
            try
            {
                string url = string.Format(ConfigurationManager.AppSettings["url_motor_calculo_rentas"], "cotizar");
                log.Debug(string.Format("Se va a consumir el endpoint para cotizar: POST [{0}]", url));

                Dominio.Entidades.MotorCalculo.Cotizacion cotizacion = null;
                using (var httpClient = new HttpClient())
                {
                    var content = JsonConvert.SerializeObject(parametros);
                    log.Debug(string.Format("Request Body [{0}]", content));

                    var response = httpClient.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json")).Result;
                    var responseContent = response.Content.ReadAsStringAsync().Result;
                    log.Debug(string.Format("Response Body [{0}]", responseContent));

                    var estadoPeticion = response.IsSuccessStatusCode;
                    if (estadoPeticion)
                    {
                        cotizacion = JsonConvert.DeserializeObject<Dominio.Entidades.MotorCalculo.Cotizacion>(responseContent);
                    }
                    else
                    {
                        cotizacion = parametros.cotizacion;
                        cotizacion.ind_cotiza = false;
                        cotizacion.num_error_cot = 300;
                    }
                }

                return cotizacion;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        private void generarArchivoLogCotizacion(string ruta, string tipo, string num_solicitud, long num_correlativo, List<string> listaLog)
        {
            try
            {
                TextWriter writer;
                string rutaLog = ruta + num_solicitud;
                string nombreLog = rutaLog + "\\" + num_solicitud + "_" + num_correlativo.ToString() + "_" + tipo + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt";

                log.Debug("nombreLog: " + nombreLog);

                if (!Directory.Exists(rutaLog))
                {
                    Directory.CreateDirectory(rutaLog);
                }

                writer = new StreamWriter(nombreLog, false, Encoding.UTF8);

                foreach (var itemPut in listaLog)
                {
                    writer.WriteLine(itemPut);
                }

                writer.Close();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw ex;
            }
        }

        private List<PjeBen> CrearPorcentajesBeneficiario(List<GrupoFamiliar> beneficiarios, Int64 numCorrelativo)
        {
            List<PjeBen> porcentajes = new List<PjeBen>();

            int i = 0;
            beneficiarios.ForEach(beneficiario =>
            {
                PjeBen porcentaje = new PjeBen
                {
                    Correlativo = i + 1,
                    TipoProducto = "02",
                    PorcentajeBase = beneficiario.ValPjeRenta,
                    PorcentajeModificado = beneficiario.ValPjeRenta + beneficiario.ValPjeAdicional,
                    PorcentajeRetPeriodoDiferido = 0,
                    NumeroCorrelativoCotizacion = numCorrelativo
                };
                porcentajes.Add(porcentaje);
                i++;
            });
            return porcentajes;
        }

        public void ActualizarBeneficiariosPNoG(List<GrupoFamiliar> lstEntity)
        {
            repositorioGrupoFamiliar.ActualizarBeneficiariosPNoG(lstEntity);
        }

        public void ActualizarEstudioNecesidadSolicitud(string num_solicitud, int? id_estudio_necesidades, string usuario)
        {
            try
            {
                repositorioSolicitud.ActualizarEstudioNecesidad(num_solicitud, id_estudio_necesidades, usuario);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //<GTI.59048-INI>
        public AbonoPoliza ObtenerAbonoPorPoliza(int numeroPoliza, string usuario)
        {
            var abono = repositorioSolicitud.ObtenerAbonoPorPoliza(numeroPoliza, usuario);
            return abono;
        }

        public PolizaRV ObtenerDatosPolizaRV(string num_solicitud, int numeroCorrelativo, string usuario)
        {
            var poliza = repositorioSolicitud.ObtenerDatosPoliza(num_solicitud, numeroCorrelativo, usuario);
            return poliza;
        }
        //<GTI.59048-FIN>
    }
}
