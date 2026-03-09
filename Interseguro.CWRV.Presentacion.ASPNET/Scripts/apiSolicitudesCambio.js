const ApiSolicitudesCambio = {
  URL_API_SOLICITUDES_CAMBIO: window.API_SOLICITUDES_CAMBIO_URL,

  ObtenerDatosSolicitudEscenario: async (numSolicitud) => {
    const response = await UtilitariosManager.callApi(`${ApiSolicitudesCambio.URL_API_SOLICITUDES_CAMBIO}/solicitud-escenario/${numSolicitud}`);
    return response;
  },

  ObtenerCita: async (cita) => {
    const response = await UtilitariosManager.callApi(`${ApiSolicitudesCambio.URL_API_SOLICITUDES_CAMBIO}/citas`, {
      method: "POST",
      body: JSON.stringify(cita)
    });
    return response;
  },

  ObtenerPreCubo: async (numeroAgente, cuspp) => {
    const response = await UtilitariosManager.callApi(`${ApiSolicitudesCambio.URL_API_SOLICITUDES_CAMBIO}/pre-cubo/${numeroAgente}/${cuspp}`);
    return response;
  },

  ObtenerMSAgente: async (idAgente) => {
    const response = await UtilitariosManager.callApi(`${ApiSolicitudesCambio.URL_API_SOLICITUDES_CAMBIO}/ms-agente/${idAgente}`);
    return response;
  },
  ObtenerSolicitudEscenarioCambio: async (numJefe, numSupervisor, numAgente) => {
    const response = await UtilitariosManager.callApi(`${ApiSolicitudesCambio.URL_API_SOLICITUDES_CAMBIO}/solicitud-escenario/cambios/${numJefe}/${numSupervisor}/${numAgente}`);
    return response;
  },

  RegistrarCotizacionMovimientoBloque: async (numSolicitudes, rechazo) => {
    const response = await UtilitariosManager.callApi(`${ApiSolicitudesCambio.URL_API_SOLICITUDES_CAMBIO}/cotizacion-movimiento/bloque`, {
      method: "POST",
      body: JSON.stringify({ numSolicitudes, rechazo })
    });
    return response;
  },

  RegistrarCotizacionMovimiento: async (numSolicitud, fechaCotizacion, envioCorreo, rechazo, cotizaciones, acom, dcom, montoAcom) => {
    const response = await UtilitariosManager.callApi(`${ApiSolicitudesCambio.URL_API_SOLICITUDES_CAMBIO}/cotizacion-movimiento`, {
      method: "POST",
      body: JSON.stringify({ numSolicitud, envioCorreo, rechazo, cotizaciones, acom, dcom, montoAcom, fechaCotizacion })
    });
    return response;
  },

  ObtenerDatosSolicitud: async (numSolicitud, fechaCotizacion) => {
    const response = await UtilitariosManager.callApi(`${ApiSolicitudesCambio.URL_API_SOLICITUDES_CAMBIO}/solicitud/${numSolicitud}/${fechaCotizacion}`);
    return response;
  },

  GetUrlApi: () => {
    return ApiSolicitudesCambio.URL_API_SOLICITUDES_CAMBIO;
  },

  setup: function () {
    window.ApiSolicitudesCambio = ApiSolicitudesCambio;
  }
}

$(document).ready(function () {
  ApiSolicitudesCambio.setup();
});
