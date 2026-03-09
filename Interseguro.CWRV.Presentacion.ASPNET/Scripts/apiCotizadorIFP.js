const ApiCotizadorIFP = {
    URL_API_COTIZADOR_IFP: window.API_COTIZADOR_IFP_URL,

    ObtenerGrupoFamiliarPorCuspp: async (cuspp) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/grupo-familiar/cuspp/${cuspp}`
        );
        return response;
    },

    ObtenerSolicitudesPorCuspp: async (cuspp) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/cuspp/${cuspp}`
        );
        return response;
    },

    ObtenerSolicitudPorId: async (idSolicitud) => {
        const response = await UtilitariosManager.callApi(`${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/${idSolicitud}`);
        return response;
    },

    ObtenerAfiliadoPorCuspp: async (cuspp) => {
        const response = await UtilitariosManager.callApi(`${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/afiliado?cuspp=${cuspp}`);
        return response;
    },

    ObtenerAfiliadoCoincidenciaLN: async (payload) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/afiliado/coincidencia-ln`,
            {
                method: "POST",
                body: JSON.stringify(payload),
            }
        );
        return response;
    },

    RegistrarSolicitud: async (payload) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud`,
            {
                method: "POST",
                body: JSON.stringify(payload),
            }
        );
        return response;
    },

    ModificarSolicitud: async (payload, idSolicitud) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/${idSolicitud}`,
            {
                method: "PUT",
                body: JSON.stringify(payload),
            }
        );
        return response;
    },

    ValidarCantidadSolicitudes: async (cuspp, moneda, montoPrimaUnica) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/cantidad`,
            {
                method: "POST",
                body: JSON.stringify({
                    cuspp: cuspp,
                    moneda: moneda,
                    montoPrimaUnica: montoPrimaUnica,
                }),
            }
        );
        return response;
    },
    GenerarPoliza: async (payload, idSolicitud) => {
        const response = await UtilitariosManager.callApi(`${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/${idSolicitud}/generar-poliza`, {
            method: 'POST',
            body: JSON.stringify(payload)
        });
        return response;
    },

    AnularPoliza: async (payload, idSolicitud) => {
        const response = await UtilitariosManager.callApi(`${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/${idSolicitud}/anular`, {
            method: 'PUT',
            body: JSON.stringify(payload)
        });
        return response;
    },

    ValidarVigencia: async (idSolicitud) => {
        const response = await UtilitariosManager.callApi(`${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/${idSolicitud}/validar-vigencia`);
        return response;
    },

    ObtenerFirmaDigital: async (idSolicitud, numItem, usuario) => {
        const response = await UtilitariosManager.callApi(`${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/firmas-digitales/${idSolicitud}/${numItem}/${usuario}`);
        return response;
    },

    ActualizarGrupoFamiliar: async (payload) => {
        const response = await UtilitariosManager.callApi(`${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/afiliado/grupo-familiar`, {
            method: 'PUT',
            body: JSON.stringify(payload)
        });
        return response;
    },

    GetUrlApi: () => {
        return ApiCotizadorIFP.URL_API_COTIZADOR_IFP;
    },

    ObtenerPagoDoblePeriodoGarantizado: async (valPeriodoGarantizado) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/parametros/pago-doble-periodo-garantizado/${valPeriodoGarantizado}`
        );
        return response;
    },

    ObtenerDiferimientoPeriodoGarantizado: async (valPeriodoGarantizado) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/parametros/diferimiento-periodo-garantizado/${valPeriodoGarantizado}`
        );
        return response;
    },

    ObtenerSolicitudIFP: async (idSolicitud) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/${idSolicitud}`
        );
        return response;
    },

    ObtenerMonedaAjuste: async (codMoneda) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/parametros/moneda-ajuste/${codMoneda}`
        );
        return response;
    },

    ObtenerPagoDoble: async (valTemporalidad, valDiferido) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/parametros/pago-doble/${valTemporalidad}/${valDiferido}`
        );
        return response;
    },

    ObtenerPagoDiferimiento: async (strTemporalidad, plan) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/parametros/pago-diferimiento/${strTemporalidad}/${plan}`
        );
        return response;
    },

    ObtenerPagoDiferimientoCPagoDoble: async (
        strPagoDoble,
        numTemporalidad,
        plan
    ) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/parametros/pago-diferimiento-con-pago-doble/${strPagoDoble}/${numTemporalidad}/${plan}`
        );
        return response;
    },

    ObtenerDevFallecimiento: async (strDevolucion) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/parametros/dev-fallecimiento/${strDevolucion}`
        );
        return response;
    },

    ObtenerArchivosStorage: async (solicitud) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/obtener-archivos/${solicitud}`
        );
        return response;
    },

    BorrarArchivoStorage: async (solicitud, archivo) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/${solicitud}/eliminar-archivo/${archivo}`, {
            method: 'DELETE'
        });
        return response;
    },

    SubirArchivoStorage: async (solicitud, payload) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorIFP.URL_API_COTIZADOR_IFP}/cotizador-ifp/solicitud/${solicitud}/subir-archivo`, {
            method: 'POST',
            body: JSON.stringify(payload)
        });
        return response;
    },

    GetUrlApi: () => {
        return ApiCotizadorIFP.URL_API_COTIZADOR_IFP;
    },

    setup: function () {
        window.ApiCotizadorIFP = ApiCotizadorIFP;
    },
};

ApiCotizadorIFP.setup();
