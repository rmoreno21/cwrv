const EnumsPermisos = {
    VideoTutorial: 53,
    ManualUsuario: 52,

    MenuCotizador: 27,
    MenuReportes: 28,
    MenuSimuladores: 29,
    MenuConfiguracion: 30,
    MenuMeler: 43,

    CotizacionExtraoficial: 40,
    CotizacionOficial: 41,
    BandejaAprobacionOficiales: 42,
    SolicitudAnticipo: 48,
    ConsultaSolicitudesAnticipo: 51,

    ReporteSeguimiento: 21,
    ReporteSupervision: 22,
    ReporteGestionVentas: 70,

    SimuladorJubilarseHoy: 23,
    SimuladorRentaVitaliciaRetiroProgramado: 24,
    SimuladorInmediataDiferida: 25,
    SimuladorTipoMoneda: 26,
    SimuladorQueMeConviene: 39,
    SimuladorCapitalRequerido: 56,
    SimuladorRentasVitaliciasVsPlazoFijo: 59,

    BusquedaAfiliadoConsultar: 1,
    DatosAfiliadoConsultar: 2,
    DatosAfiliadoActualizar: 3,

    DireccionConsultar: 4,
    DireccionInsertar: 5,
    DireccionActualizar: 6,
    DireccionEliminar: 7,

    TelefonoConsultar: 8,
    TelefonoInsertar: 9,
    TelefonoActualizar: 10,
    TelefonoEliminar: 11,

    GrupoFamiliarConsultar: 12,
    GrupoFamiliarInsertar: 13,
    GrupoFamiliarActualizar: 14,

    SolicitudConsultar: 15,
    SolicitudInsertar: 16,
    SolicitudActualizar: 17,
    SolicitudEnviarCorreo: 18,
    SolicitudExportarPDF: 19,
    SolicitudReporteEscenarios: 32,
    SolicitudValidacionesACOM: 49,
    SolicitudValidacionesDTRA: 50,

    ConfiguracionMontoCIC: 31,
    MontoCICInsertar: 33,
    MontoCICActualizar: 34,
    MontoCICEliminar: 35,

    ActividadConsultar: 20,

    PermisoACOM: 36,
    PermisoDCOM: 37,
    PermisoTRA: 38,

    DescargaSolicitudes: 44,
    DescargaResultados: 93,
    CotizarLote: 45,
    CargaConfirmaciones: 46,
    CargaCotizacionesMeler: 47,

    PermisoEspeciales: 60,

    SolicitudPlusConsultar: 61,
    SolicitudPlusInsertar: 62,
    SolicitudPlusActualizar: 63,
    SolicitudPlusEnviarCorreo: 64,
    SolicitudPlusExportarPDF: 65,

    InformeA: 66,
    InformeB: 67,
    RechazarSolicitud: 68,
    PermisoTRAPlus: 69,

    CuotaTRA: 71,
    PermisoDCOMBandeja: 72,

    EnviarEmailPendiente: 73,
    SolicitudPlusCerrar: 74,
    SolicitudPlusGenerarPoliza: 77,

    CierrePolizaParticularPlus: 75,
    ReporteCotizacionPlus: 76,

    DatosAdicionalesGrupoFamiliar: 78,

    ListadoCotizacionesEvaluacion: 79,

    AprobarFlujoSolicitud: 80,
    ObservarFlujoSolicitud: 81,
    RechazarFlujoSolicitud: 82,

    CotizacionesIFP: 83,
    SolicitudIFPConsultar: 84,
    SolicitudIFPInsertar: 85,
    SolicitudIFPActualizar: 86,
    SolicitudIFPEnviarCorreo: 87,
    SolicitudIFPExportarPDF: 88,

    ReporteIndicadoresRRVV: 89,
    ContratoCotizacionInsertar: 90,
    ContratoCotizacionActualizar: 91,
    ContratoCotizacionEliminar: 35, // Duplicado del valor MontoCICEliminar

    ConsolidadoCumplimientoCdA: 94,
    DashboardCntoFD: 95,
    RecalculoCotizaciones: 96,
    CargaArchivosIndicadores: 97,
    ReporteIndicadorCDA: 98,
    ReporteIndicadorVCTP: 99,
    CierrePolizaRVI: 100,

    PlantillaCorreoElectronico: 101,
    LogCotizacionGrabar: 102,
    LogReservaGrabar: 103,
    ObtenerFormatoEstudioNecesidades: 104,
    RentaParticularPlus: 57,
    ParametrosRentas: 105
}

const EnumsRoles = {
    AgenteLima: "AGT.LIM.RVI",
    AgenteProvincia: "AGT.PRO.RVI",
    AsistenteComercial: "AST.RVI.COM",
    AsistenteOperaciones: "AST.RVI.OPE",
    AnalistaOperaciones: "ANL.RVI",
    GerenteDivision: "GTE.DIV.RVI",
    JefeOperaciones: "JEF.RVI.OPE",
    JefeVentaLima: "JEF.VTA.LIM.RVI",
    JefeVentaProvincia: "JEF.VTA.PRO.RVI",
    SupervisorLima: "SPV.LIM.RVI",
    SupervisorProvincia: "SPV.PRO.RVI",
    CoordinadorPlaft: "COR.PLAFT",
    AgenteExterno: "AGT.EXT"
}

const EnumParametroTabla = {
    EstadoCita: "ESTADO_CITA",
    Correo: "CORREO",
    Cubo: "CUBO",
    Tra: "TRA",
    VisitaCita: "VISITA_CITA"
}

const EnumFuncionesVistas = {
    CargarTablasBusqueda: ["RentaIFP/Cotizador"],
    CargarBeneficiariosPlan3IFP: ["RentaIFP/MantenerSolicitud"]
}

const EnumTipoPeriodoBeneficiario = {
    Garantizada: "1",
    NoGarantizada: "2",
    BeneficiariosRPP: "3"
}

const EnumParentesco = {
    Afiliado: "80",
    Conyuge: "10",
    Padre: "40",
    Hijo: "30",
    Concubino: "90",
    Primo: "91",
    Hermano: "92",
    Sobrino: "93",
    Otros: "94",
    Nieto: "95"
}

const EnumComboboxIFP = {
    Temporalidad: 0,
    Moneda: 1,
    PorcentajeDev: 2,
    PorcentajeEscalon: 3
}

const EnumEstadoPlaft = {
    Cotizado: "0",
    Observado: "1",
    Rechazado: "2",
    Aprobado: "3",
    Evaluacion: "4"
}

const EnumCuadroMensajeIcono = {
    Informacion: "info",
    Error: "error",
    Validacion: "validacion",
    Advertencia: "advertencia",
    Exito: "exito"
}

const EnumCuadroMensajeTitulo = {
    Informacion: "Información",
    Error: "Error",
    Validacion: "Validación",
    Advertencia: "Advertencia",
    Confirmacion: "Confirmación",
    Exito: "Éxito",
    Mensaje: "Mensaje"
}


const UtilitariosManager = {
    getStorageItem: function (key) {
        return localStorage.getItem(key);
    },
    setStorageItem: function (key, value) {
        localStorage.setItem(key, value);
    },
    removeStorageItem: function (key) {
        localStorage.removeItem(key);
    },
    getSessionStorageItem: function (key) {
        return sessionStorage.getItem(key);
    },
    setSessionStorageItem: function (key, value) {
        sessionStorage.setItem(key, value);
    },
    removeSessionStorageItem: function (key) {
        sessionStorage.removeItem(key);
    },
    DecryptJWT: function (token) {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        return JSON.parse(jsonPayload);
    },
    ObtenerSession: function () {
        const jwt = this.getStorageItem("jwt");
        if (!jwt) return null;

        const session = this.DecryptJWT(jwt);
        const datosUsuario = session.datosUsuario;
        return {
            "NombreCompleto": datosUsuario.NombreCompleto,
            "Apellidos": datosUsuario.Apellidos,
            "Nombres": datosUsuario.Nombres,
            "CodigoEmpleado": datosUsuario.CodigoEmpleado,
            "Correo": datosUsuario.Correo,
            "Dominio": datosUsuario.Dominio,
            "Estado": datosUsuario.Estado,
            "Matricula": datosUsuario.Matricula,
            "Rol": datosUsuario.Rol,
            "RolAzman": datosUsuario.RolAzman,
            "NumeroAgente": session.numAgente,
            "TokenUsuario": session.tokenUsario,
        }
    },
    ValidarPermisoActivo: function (permiso) {
        const opciones = JSON.parse(this.getStorageItem("opcionesSistema"));
        if (!opciones) return false;
        const opcion = opciones.find(o => o.IdAzman === permiso);
        return opcion !== null && opcion.Activa;
    },
    ValidarPermiso: function (permiso) {
        if (!this.ValidarPermisoActivo(permiso)) {
            window.location.href = "/Error/Permisos.aspx";
        }
    },
    esRolComercial: function (rol) {

        return rol === EnumsRoles.AgenteExterno ||
            rol === EnumsRoles.AgenteLima ||
            rol === EnumsRoles.AgenteProvincia ||
            rol === EnumsRoles.SupervisorLima ||
            rol === EnumsRoles.SupervisorProvincia ||
            rol === EnumsRoles.JefeVentaLima ||
            rol === EnumsRoles.JefeVentaProvincia ||
            rol === EnumsRoles.AsistenteComercial ||
            rol === EnumsRoles.GerenteDivision

    },
    callApi: async function (url, options) {
        return await new Promise((resolve, reject) => {
            const session = this.ObtenerSession();
            options = options || {};
            options.headers = options.headers || {};
            options.headers["Content-Type"] = "application/json";
            options.headers["Authorization"] = `Bearer ${this.getStorageItem("jwt")}`;
            if (options && !options.ignoreAditionalHeaders) {
                options.headers["x-username"] = session.Matricula;
                options.headers["x-rol"] = session.RolAzman;
                options.headers["x-client-ip"] = this.getStorageItem("ipCliente");
            }
            fetch(url, options)
                .then(async (response) => {
                    if (!response.ok) {
                        console.error(`Ocurrió un error en la llamada a la API ${url}`);
                        const errorText = await response.text();
                        reject({
                            status: response.status,
                            statusText: response.statusText,
                            body: JSON.parse(errorText),
                            url,
                        });
                    } else if (response.status === 200) {
                        const json = response.json();
                        json.then(data => {
                            resolve(data.data);
                        }).catch(error => {
                            console.error(`Ocurrió un error al obtener los datos de la API ${url}: ${error}`);
                            reject(error);
                        });
                    } else if (response.status === 204) {
                        resolve(null);
                    }
                })
                .catch(error => {
                    console.error(`Ocurrió un error en la llamada a la API ${url}: ${error}`);
                    reject(error);
                });
        });
    },
    ObtenerQueryParams: function (param) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(param);
    },
    format: {
        porcentaje: function (valor) {
            return valor.toFixed(2) + '%';
        }
    },
    date: {
        parseDate: function (date) {
            const arrDate = date.split('/');
            return new Date(arrDate[2], arrDate[1] - 1, arrDate[0]);
        }
    },
    ReiniciarTimeout: function () {
        const selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        $clock.countdown(selectedDate.toString());
    },
    setup: function () {
        window.Utilitarios = UtilitariosManager;
        window.EnumsPermisos = EnumsPermisos;
        window.EnumsRoles = EnumsRoles;
        window.EnumParametroTabla = EnumParametroTabla;
        window.EnumFuncionesVistas = EnumFuncionesVistas;
        window.EnumParentesco = EnumParentesco;
        window.EnumTipoPeriodoBeneficiario = EnumTipoPeriodoBeneficiario;
        window.EnumComboboxIFP = EnumComboboxIFP;
        window.EnumCuadroMensajeIcono = EnumCuadroMensajeIcono;
        window.EnumCuadroMensajeTitulo = EnumCuadroMensajeTitulo;
    }
};

UtilitariosManager.setup();