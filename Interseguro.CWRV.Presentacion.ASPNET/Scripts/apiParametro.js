const ApiParametro = {
  URL_API_PARAMETRO: window.API_PARAMETRO_URL,

  ObtenerComboboxIFP: async () => {
    const response = await UtilitariosManager.callApi(`${ApiParametro.URL_API_PARAMETRO}/parametro/ifp`);
    return response;
  },

  ObtenerParametroSistema: async () => {
    const response = await UtilitariosManager.callApi(`${ApiParametro.URL_API_PARAMETRO}/parametro/sistema`);
    return response;
  },

  ObtenerTipoIdentificacion: async (cod_tipo_identificacion, gls_tipo_identificacion, gls_corta_identificacion) => {
    const response = await UtilitariosManager.callApi(`${ApiParametro.URL_API_PARAMETRO}/parametro/tipo-identificacion`, {
      method: 'GET',
      params: {
        cod_tipo_identificacion,
        gls_tipo_identificacion,
        gls_corta_identificacion
      }
    });
    return response;
  },

  ObtenerEstadosCivil: async (usuario) => {
    if (Utilitarios.getStorageItem("estadosCivil")) {
      return JSON.parse(Utilitarios.getStorageItem("estadosCivil"));
    }
    const response = await UtilitariosManager.callApi(`${ApiParametro.URL_API_PARAMETRO}/parametro/estado-civil?usuario=${usuario}`, { ignoreAditionalHeaders: true });
    Utilitarios.setStorageItem("estadosCivil", JSON.stringify(response));
    return response;
  },

  ObtenerProfesiones: async (usuario) => {
    if (Utilitarios.getStorageItem("profesiones")) {
      return JSON.parse(Utilitarios.getStorageItem("profesiones"));
    }
    const response = await UtilitariosManager.callApi(`${ApiParametro.URL_API_PARAMETRO}/parametro/profesiones?usuario=${usuario}`, { ignoreAditionalHeaders: true });
    Utilitarios.setStorageItem("profesiones", JSON.stringify(response));
    return response;
  },

  ObtenerNacionalidades: async (usuario) => {
    if (Utilitarios.getStorageItem("nacionalidades")) {
      return JSON.parse(Utilitarios.getStorageItem("nacionalidades"));
    }
    const response = await UtilitariosManager.callApi(`${ApiParametro.URL_API_PARAMETRO}/parametro/nacionalidades?usuario=${usuario}`, { ignoreAditionalHeaders: true });
    Utilitarios.setStorageItem("nacionalidades", JSON.stringify(response));
    return response;
  },

  setup: function () {
    window.ApiParametro = ApiParametro;
  }
}

document.addEventListener('DOMContentLoaded', function () {
  ApiParametro.setup();
});

/* $(document).ready(function () {
  ApiParametro.setup();
}); */


