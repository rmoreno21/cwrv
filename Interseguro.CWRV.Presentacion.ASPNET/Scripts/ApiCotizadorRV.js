const ApiCotizadorRV = {
  URL_API_COTIZADOR_RV: window.API_COTIZADOR_RV_URL,
  
  ObtenerMontoAcom: async (numSolicitud, acom, cotizacion) => {
    const response = await UtilitariosManager.callApi(`${ApiCotizadorRV.URL_API_COTIZADOR_RV}/cotizacion-oficial/obtener-monto-acom`, {
      method: 'POST',
      body: JSON.stringify({
        acom: acom.toString(),
        cotizacion: cotizacion.toString(),
        solicitud: numSolicitud.toString()
      })
    });
    return response;
  },
  
  RecalcularCotizacion: async (payload) => {
    const response = await UtilitariosManager.callApi(`${ApiCotizadorRV.URL_API_COTIZADOR_RV}/solicitud/recalculo-cotizacion`, {
      method: 'POST',
      body: JSON.stringify(payload)
    });
    return response;
  },

  GetUrlApi: () => {
    return ApiCotizadorRV.URL_API_COTIZADOR_RV;
  },

  setup: function () {
    window.ApiCotizadorRV = ApiCotizadorRV;
  }
}

ApiCotizadorRV.setup();