const ApiCotizadorRentasVitalicias = {
    URL_API_COTIZADOR_RENTAS = "http://localhost:3205/api/v1",

    registrarSolicitud: async (codMoneda) => {
        const response = await UtilitariosManager.callApi(
            `${ApiCotizadorRentasVitalicias.URL_API_COTIZADOR_RENTAS}/cotizador-ifp/parametros/moneda-ajuste/${codMoneda}`
        );

        return response;
    },

}