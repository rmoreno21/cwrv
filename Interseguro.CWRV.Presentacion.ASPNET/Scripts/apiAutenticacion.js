const ApiAutenticacion = {
    URL_API_AUTENTICACION: window.API_AUTENTICACION_URL,

    IniciarSesion: async (payload) => {
        const response = await UtilitariosManager.callApi(`${ApiAutenticacion.URL_API_AUTENTICACION}/autenticacion/iniciar-sesion`, {
            method: "POST",
            body: JSON.stringify(payload),
            ignoreAditionalHeaders: true
        });
        return response;
    },

    setup: function () {
        window.ApiAutenticacion = ApiAutenticacion;
    }
}

$(document).ready(function () {
    ApiAutenticacion.setup();
});


