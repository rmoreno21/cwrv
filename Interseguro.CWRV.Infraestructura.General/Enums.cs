using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Interseguro.CWRV.Infraestructura.General
{
    public static class Enums
    {
        #region StringValue

        public class StringValueAttribute : Attribute
        {
            public string StringValue { get; protected set; }
            public StringValueAttribute(string value)
            {
                StringValue = value;
            }
        }

        public static string StringValue(this Enum value)
        {
            // Get the type
            Type type = value.GetType();

            // Get fieldinfo for this type
            FieldInfo fieldInfo = type.GetField(value.ToString());

            // Get the stringvalue attributes
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(StringValueAttribute), false) as StringValueAttribute[];

            // Return the first if there was a match.
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }
        #endregion

        public enum CategoriaCombobox
        {
            Categoria = 0,
            Prestacion = 1,
            Afp = 2,
            Telefono = 3,
            Identificacion = 4,
            Parentesco = 5,
            Sexo = 6,
            Invalidez = 7,
            TipoCotizacion = 8,
            Moneda = 9,
            Capital = 10,
            FactorTRA = 11,
            Temporalidad = 12, //SE AGREGA SEGUN EL ORDEN DEL SP
            MonedaRentaPrivada = 13, //SE AGREGA SEGUN EL ORDEN DEL SP
            PorcentajeEscalonado = 14, //Pje Escalonado
            PorcentajeDevolucion = 15,
            TipoPlan = 16,
            PorcentajeConyuge = 17, //<INIGTI_753>
            Compania = 18,
            Departamento = 19,//SE MODIFICA ANTES ERA 12
            PeriodoTemporal = 20, //Para obtener el periodo ini - fin dinamico
            PagoEscalonado = 21, //Para obtener el periodo ini - fin dinamico
            MonedaAjustePlus = 22, //Para el ajuste de la moneda - fin dinamico
            MonedaIFP = 13,
            NuevoPorcentajeEscalonado = 23,
            ParentescoPVPEP = 24,
            ProductosRentas = 25,
            Origen = 26,
            ParametrosRentas = 27
        }

        public enum Parentesco
        {
            [StringValue("80")]
            Afiliado,
            [StringValue("10")]
            Conyuge,
            [StringValue("40")]
            Padre,
            [StringValue("30")]
            Hijo,
            [StringValue("90")]
            Concubino,
            [StringValue("91")]
            Primo,
            [StringValue("92")]
            Hermano,
            [StringValue("93")]
            Sobrino,
            [StringValue("94")]
            Otros,
            [StringValue("95")]
            Nieto
        }

        public enum Invalidez
        {
            [StringValue("S")]
            Si,
            [StringValue("N")]
            No
        }

        public enum TipoInvalidez
        {
            [StringValue("-")]
            Desconocido,
            [StringValue("N")]
            NoInvalido,
            [StringValue("P")]
            Parcial,
            [StringValue("T")]
            Total
        }

        public enum TipoCotizacion
        {
            [StringValue("E")]
            Extraoficial,
            [StringValue("O")]
            Oficial,
            [StringValue("P")]
            Prueba,
            [StringValue("RPV")]
            RentaPrivada,
            [StringValue("RPP")]
            RentaPrivadaPlus,
            [StringValue("IFP")]
            RentaPrivadaIFP
        }

        public enum Moneda
        {
            [StringValue("001")]
            Soles,
            [StringValue("002")]
            Dolares,
            [StringValue("013")]
            SolesAjustados,
            [StringValue("014")]
            DolaresAjustados
        }

        public enum Modalidad
        {
            [StringValue("I")]
            Inmediata,
            [StringValue("D")]
            Diferida,
            [StringValue("I-RM")]
            Mixta,
            [StringValue("I-RC")]
            Combinada,
            [StringValue("I-RB")]
            Bimoneda,
            [StringValue("I-RVE")]
            Escalonada
        }

        public enum ModalidadMeler
        {
            [StringValue("RV")]
            Inmediata,
            [StringValue("RTVD")]
            Diferida,
            [StringValue("RM")]
            Mixta,
            [StringValue("RC")]
            Combinada,
            [StringValue("RB")]
            Bimoneda,
            [StringValue("RVE")]
            Escalonada
        }

        public enum CategoriaRVI
        {
            [StringValue("A")]
            EdadLegal,
            [StringValue("B")]
            JubilacionAnticipadaOrdinaria,
            [StringValue("C")]
            Invalidez,
            [StringValue("D")]
            Sobrevivencia,
            [StringValue("F")]
            JubilacionAnticipadaRiesgoRegExtr,
            [StringValue("G")]
            JubilacionAnticipadaRiesgoGen,
            [StringValue("H")]
            Reja2002,
            [StringValue("I")]
            JubilacionAnticipada19990,
            [StringValue("M")]
            Reja2007,
            [StringValue("N")]
            Reja2010,
            [StringValue("O")]
            JubilacionAnticipadaEnfermedadTerminal,
            [StringValue("P")]
            RegimenEspecialDesempleados
        }

        public enum OrdenModalidad
        {
            [StringValue("1")]
            Modalidad_1,
            [StringValue("2")]
            Modalidad_2,
            [StringValue("3")]
            Modalidad_3
        }

        public enum TipoMovimiento
        {
            [StringValue("0")]
            SolicitudCotizada = 0,
            [StringValue("1")]
            EnEvaluacionAsistente = 1,
            [StringValue("2")]
            EnEvaluacionSupervisor = 2,
            [StringValue("3")]
            EnEvaluacionJefe = 3,
            [StringValue("4")]
            EnEvaluacionGerente = 4,
            [StringValue("5")]
            EnEvaluacionOperaciones = 5,
            [StringValue("6")]
            SolicitudAprobada = 6,
            [StringValue("7")]
            SolicitudRechazada = 7,
            [StringValue("8")]
            SolicitudSustituida = 8
        }

        public enum RolAzman
        {
            [StringValue("AGT.LIM.RVI")]
            AgenteLima,
            [StringValue("AGT.PRO.RVI")]
            AgenteProvincia,
            [StringValue("AST.RVI.COM")]
            AsistenteComercial,
            [StringValue("AST.RVI.OPE")]
            AsistenteOperaciones,
            [StringValue("ANL.RVI")]
            AnalistaOperaciones,
            [StringValue("GTE.DIV.RVI")]
            GerenteDivision,
            [StringValue("JEF.RVI.OPE")]
            JefeOperaciones,
            [StringValue("JEF.VTA.LIM.RVI")]
            JefeVentaLima,
            [StringValue("JEF.VTA.PRO.RVI")]
            JefeVentaProvincia,
            [StringValue("SPV.LIM.RVI")]
            SupervisorLima,
            [StringValue("SPV.PRO.RVI")]
            SupervisorProvincia,
            [StringValue("COR.PLAFT")]
            CoordinadorPlaft,
            [StringValue("AGT.EXT")]
            AgenteExterno
        }

        public enum ParametroTabla
        {
            [StringValue("ESTADO_CITA")]
            EstadoCita,
            [StringValue("CORREO")]
            Correo,
            [StringValue("CUBO")]
            Cubo,
            [StringValue("TRA")]
            Tra,
            [StringValue("VISITA_CITA")]
            VisitaCita
        }

        public enum Parametro
        {
            [StringValue("CITA_ESTADO_1")]
            CitaEstado1,
            [StringValue("CITA_ESTADO_3")]
            CitaEstado3,
            [StringValue("CORREO")]
            Correo,
            [StringValue("CUBO_2C")]
            Cubo2C,
            [StringValue("CUBO_3C")]
            Cubo3C,
            [StringValue("TRA_ASIS")]
            TraLimiteAsistente,
            [StringValue("VISITA_CITA_INICIO")]
            VisitaCitaInicio,
            [StringValue("VISITA_CITA_FIN")]
            VisitaCitaFin
        }

        public enum Seleccion
        {
            [StringValue("S")]
            Si,
            [StringValue("N")]
            No
        }

        public enum TipoPension
        {
            [StringValue("A")]
            Anticipada,
            [StringValue("ICC")]
            InvalidezConCobertura,
            [StringValue("ISC")]
            InvalidezSinCobertura,
            [StringValue("S")]
            Sobrevivencia,
            [StringValue("V")]
            Jubilacion
        }

        public enum CuadroMensajeIcono
        {
            [StringValue("info")]
            Informacion,
            [StringValue("error")]
            Error,
            [StringValue("validacion")]
            Validacion,
            [StringValue("advertencia")]
            Advertencia,
            [StringValue("exito")]
            Exito
        }

        public enum CuadroMensajeTitulo
        {
            [StringValue("Información")]
            Informacion,
            [StringValue("Error")]
            Error,
            [StringValue("Validación")]
            Validacion,
            [StringValue("Advertencia")]
            Advertencia,
            [StringValue("Confirmación")]
            Confirmacion,
            [StringValue("Éxito")]
            Exito,
            [StringValue("Mensaje")]
            Mensaje
        }

        public enum EventoLog
        {
            [StringValue("01")]
            IniciarSesion,
            [StringValue("02")]
            CerrarSesion,
            [StringValue("03")]
            CotizarSolicitud,
            [StringValue("04")]
            EnviarCorreoElectronico,
            [StringValue("05")]
            ExportarPDFSolicitud,
            [StringValue("06")]
            ModificarDatosCliente,
            [StringValue("07")]
            Reporte1,
            [StringValue("08")]
            Reporte2,
            [StringValue("09")]
            Reporte3,
            [StringValue("10")]
            Reporte4,
            [StringValue("11")]
            ReporteEscenarios,
            [StringValue("12")]
            ExportarPDFAfiliado,
            [StringValue("13")]
            ExportarPDFCalculo,
            [StringValue("14")]
            SimuladorVitaliciaVsPlazoFijo,
            [StringValue("15")]
            EnviarevaluacionPlaft,
            [StringValue("16")]
            RegistrarDatosCliente,
            [StringValue("22")]
            RegistrarDatosBeneficiario,
            [StringValue("23")]
            ModificarDatosBeneficiario,
            [StringValue("24")]
            EliminarDatosBeneficiario,
            [StringValue("25")]
            FlujoAprobacion
        }

        public enum OpcionesSistema
        {
            VideoTutorial = 53,
            ManualUsuario = 52,

            MenuCotizador = 27,
            MenuReportes = 28,
            MenuSimuladores = 29,
            MenuConfiguracion = 30,
            MenuMeler = 43,

            CotizacionExtraoficial = 40,
            CotizacionOficial = 41,
            BandejaAprobacionOficiales = 42,
            SolicitudAnticipo = 48,
            ConsultaSolicitudesAnticipo = 51,

            ReporteSeguimiento = 21,
            ReporteSupervision = 22,
            ReporteGestionVentas = 70,

            SimuladorJubilarseHoy = 23,
            SimuladorRentaVitaliciaRetiroProgramado = 24,
            SimuladorInmediataDiferida = 25,
            SimuladorTipoMoneda = 26,
            SimuladorQueMeConviene = 39,
            SimuladorCapitalRequerido = 56,
            SimuladorRentasVitaliciasVsPlazoFijo = 59,

            BusquedaAfiliadoConsultar = 1,
            DatosAfiliadoConsultar = 2,
            DatosAfiliadoActualizar = 3,

            DireccionConsultar = 4,
            DireccionInsertar = 5,
            DireccionActualizar = 6,
            DireccionEliminar = 7,

            TelefonoConsultar = 8,
            TelefonoInsertar = 9,
            TelefonoActualizar = 10,
            TelefonoEliminar = 11,

            GrupoFamiliarConsultar = 12,
            GrupoFamiliarInsertar = 13,
            GrupoFamiliarActualizar = 14,

            SolicitudConsultar = 15,
            SolicitudInsertar = 16,
            SolicitudActualizar = 17,
            SolicitudEnviarCorreo = 18,
            SolicitudExportarPDF = 19,
            SolicitudReporteEscenarios = 32,
            SolicitudValidacionesACOM = 49,
            SolicitudValidacionesDTRA = 50,

            ConfiguracionMontoCIC = 31,
            MontoCICInsertar = 33,
            MontoCICActualizar = 34,
            MontoCICEliminar = 35,

            ActividadConsultar = 20,

            PermisoACOM = 36,
            PermisoDCOM = 37,
            PermisoTRA = 38,

            DescargaSolicitudes = 44,
            DescargaResultados = 93,
            CotizarLote = 45,
            CargaConfirmaciones = 46,
            CargaCotizacionesMeler = 47,

            PermisoEspeciales = 60,

            SolicitudPlusConsultar = 61,
            SolicitudPlusInsertar = 62,
            SolicitudPlusActualizar = 63,
            SolicitudPlusEnviarCorreo = 64,
            SolicitudPlusExportarPDF = 65,

            InformeA = 66,
            InformeB = 67,
            RechazarSolicitud = 68,
            PermisoTRAPlus = 69,

            CuotaTRA = 71,
            PermisoDCOMBandeja = 72,

            EnviarEmailPendiente = 73,
            SolicitudPlusCerrar = 74,
            SolicitudPlusGenerarPoliza = 77,

            CierrePolizaParticularPlus = 75,
            ReporteCotizacionPlus = 76,

            DatosAdicionalesGrupoFamiliar = 78,

            ListadoCotizacionesEvaluacion = 79,

            AprobarFlujoSolicitud = 80,
            ObservarFlujoSolicitud = 81,
            RechazarFlujoSolicitud = 82,

            CotizacionesIFP = 83,
            SolicitudIFPConsultar = 84,
            SolicitudIFPInsertar = 85,
            SolicitudIFPActualizar = 86,
            SolicitudIFPEnviarCorreo = 87,
            SolicitudIFPExportarPDF = 88,

            ReporteIndicadoresRRVV = 89,
            ContratoCotizacionInsertar = 90,
            ContratoCotizacionActualizar = 91,
            ContratoCotizacionEliminar = 35,
            ConsolidadoCumplimientoCdA = 94,
            DashboardCntoFD = 95,
            RecalculoCotizaciones = 96,
            CargaArchivosIndicadores = 97,
            ReporteIndicadorCDA = 98,
            ReporteIndicadorVCTP = 99,
            CierrePolizaRVI = 100,

            PlantillaCorreoElectronico = 101,
            LogCotizacionGrabar = 102,
            LogReservaGrabar = 103,
            ObtenerFormatoEstudioNecesidades = 104,
            RentaParticularPlus = 57,
            ParametrosRentas = 105
        }

        public enum EstadoAnticipo
        {
            [StringValue("DESPL")]
            DescuentoPlanilla,
            [StringValue("DEVCH")]
            ChequeDevuelto,
            [StringValue("DEVPA")]
            DevolucionParcial,
            [StringValue("DEVTO")]
            DevolucionTotal,
            [StringValue("EMITI")]
            Emitido,
            [StringValue("ENTRE")]
            EntregadoAgente,
            [StringValue("ENVTE")]
            EnviadoTesoreria,
            [StringValue("GANAD")]
            CasoGanado,
            [StringValue("INGRE")]
            Ingresado,
            [StringValue("PERDI")]
            PendienteDevolucion,
            [StringValue("RECEP")]
            RecepcionadoComercial,
            [StringValue("RECHA")]
            RechazadoPerdidaCuenta,
            [StringValue("SINDS")]
            SinDescuento
        }

        public enum EstadoPlaft
        {
            [StringValue("0")]
            Cotizado,
            [StringValue("1")]
            Observado,
            [StringValue("2")]
            Rechazado,
            [StringValue("3")]
            Aprobado,
            [StringValue("4")]
            Evaluacion,
        }

        public enum TipoFlujoEvaluacion
        {
            [StringValue("1")]
            Seleccion,
            [StringValue("2")]
            Evaluacion,
            [StringValue("3")]
            EvaluacionOperaciones,
            [StringValue("4")]
            ObservadoPlaft,
            [StringValue("5")]
            RechazadoPlaft,
            [StringValue("6")]
            AprobadoPlaft,
            [StringValue("7")]
            ObservadoOperacion,
            [StringValue("8")]
            AprobadoOperacion,
            [StringValue("9")]
            Cerrado,
            [StringValue("10")]
            RechazadoOperaciones
        }

        public enum urlDefectoRol
        {
            [StringValue("~/Cotizador/Cotizador.aspx")]
            AgenteLima,
            [StringValue("~/Cotizador/Cotizador.aspx")]
            AgenteProvincia,
            [StringValue("~/Cotizador/Cotizador.aspx")]
            AsistenteComercial,
            [StringValue("~/Cotizador/Cotizador.aspx")]
            AsistenteOperaciones,
            [StringValue("~/Cotizador/Cotizador.aspx")]
            GerenteDivision,
            [StringValue("~/Cotizador/Cotizador.aspx")]
            JefeOperaciones,
            [StringValue("~/Cotizador/Cotizador.aspx")]
            JefeVentaLima,
            [StringValue("~/Cotizador/Cotizador.aspx")]
            JefeVentaProvincia,
            [StringValue("~/Cotizador/Cotizador.aspx")]
            SupervisorLima,
            [StringValue("~/Cotizador/Cotizador.aspx")]
            SupervisorProvincia,
            [StringValue("~/RentaParticular/SolicitudesEvaluacion.aspx")]
            CoordinadorPlaft,
            [StringValue("~/RentaIFP/Cotizador.aspx")]
            AgenteExterno
        }

        public enum Planes
        {
            [StringValue("RPP")]
            RPP,
            [StringValue("PLAN1")]
            PLAN1,
            [StringValue("PLAN2")]
            PLAN2,
            [StringValue("PLAN3")]
            PLAN3
        }

        public enum ComboboxIFP
        {
            Temporalidad = 0,
            Moneda = 1,
            PorcentajeDev = 2,
            PorcentajeEscalon = 3
        }

        public enum TipoMonedaIFP
        {
            [StringValue("Indexado")]
            Indexado,
            [StringValue("Nominal")]
            Nominal,
            [StringValue("Ajustados")]
            Ajustados
        }

        public enum CodigoSBS
        {
            [StringValue("VI2027500143")]
            RPP,
            [StringValue("VI2027500223")]
            IFP
        }

        public enum TipoCalculo
        {
            [StringValue("C")]
            Cotizacion,
            [StringValue("R")]
            Reserva,
        }

        public enum TipoProducto
        {
            [StringValue("RVI")]
            RVI,
            [StringValue("RPP")]
            RPP,
            [StringValue("IFP")]
            IFP
        }

        public enum DescripcionCobertura1
        {
            [StringValue("Devolución por Fallecimiento y Gasto Sepelio")]
            PLAN1,
            [StringValue("Gasto de Sepelio")]
            PLAN2,
        }

        public enum DescripcionCobertura2
        {
            [StringValue("Devolución por Sobrevivencia")]
            PLAN1,
            [StringValue("Devolucion Prima")]
            PLAN2,
        }

        public enum TipoDocumento
        {
            [StringValue("D")]
            DNI,
            [StringValue("E")]
            CE,
            [StringValue("J")]
            RUCJ,
            [StringValue("N")]
            PARNAC,
            [StringValue("P")]
            PAS,
            [StringValue("R")]
            RUCN,
        }

        public enum CanalDistribucion
        {
            [StringValue("FVEN")]
            FuerzaDeVentas,
            [StringValue("BSEG")]
            BancaSeguros,
            [StringValue("PVEN")]
            PuntoDeVenta,
            [StringValue("CDIS")]
            ComercializacionADistancia,
            [StringValue("BROK")]
            Broker
        }

        public enum TipoProductoRamo
        {
            [StringValue("750101")]
            RPP,
            [StringValue("750102")]
            IFP,
        }

        public enum OrigenCotizacion
        {
            [StringValue("1")]
            Interseguro,
            [StringValue("2")]
            Inteligo,
        }

        public enum Sexo
        {
            [StringValue("M")]
            Masculino,
            [StringValue("F")]
            Femenino,
        }

        public enum TipoDireccion
        {
            [StringValue("P")]
            Principal,
            [StringValue("E")]
            EnvioPoliza
        }

        public enum ConfiguracionConsentimiento
        {
            [StringValue("1")]
            RVI,
            [StringValue("2")]
            RP
        }

        public enum TratamientoConsentimiento
        {
            [StringValue("1")]
            RVI,
            [StringValue("2")]
            RP,
            [StringValue("3")]
            Inteligo,
            [StringValue("4")]
            InterseguroUniversal,
            [StringValue("9")]
            IntercorpAnalisisMercado,
            [StringValue("10")]
            IntercorpPublicidad
        }

        public enum ProcesoEnvio
        {
            ConsentimientoRV = 1,
            ConsentimientoRP = 2,
            FirmaDigitalRV = 3,
            FirmaDigitalRP = 4,
            AceptacionConsentimientoRV = 5,
            AceptacionConsentimientoRP = 6,
            AceptacionFirmaDigitalRV = 7,
            AceptacionFirmaDigitalRP = 8,
            PolizaElectronicaRV = 9,
            PolizaElectronicaRPP = 10,
            PolizaElectronicaIFP = 11
        }

        public enum EstadoTrazabilidad
        {
            [StringValue("ENV")]
            Enviado,
            [StringValue("LEE")]
            Leido,
            [StringValue("RBT")]
            Rebotado
        }

        public enum EstadoCotizacion
        {
            [StringValue("04")]
            Cerrado
        }

        public enum TipoPlan
        {
            [StringValue("01")]
            Familiar,
            [StringValue("02")]
            Individual
        }

        public enum TipoPeriodoBeneficiario
        {
            [StringValue("1")]
            Garantizada,
            [StringValue("2")]
            NoGarantizada,
            [StringValue("3")]
            BeneficiariosRPP
        }

        public enum TipoDocumentoCloudStorage
        {
            [StringValue("01")]
            DNI,
            [StringValue("02")]
            CE,
            [StringValue("03")]
            PAS
        }

        public enum ProveedorEnvioSms
        {
            Intico = 1,
            Infobip = 2
        }

        public enum MonedaIFPSimbolo
        {
            [StringValue("S/.")]
            Soles,
            [StringValue("US$")]
            Dolares,
            [StringValue("S/.Aj.")]
            SolesAjustados,
            [StringValue("US$Aj.")]
            DolaresAjustados
        }
    }
}
