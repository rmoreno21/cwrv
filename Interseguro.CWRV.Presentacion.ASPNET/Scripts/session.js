/**
 * Script para manejar la autenticación mediante WebMethod
 */

// Objeto principal para gestionar la autenticación
const SessionManager = {
    /**
     * Obtiene la IP del cliente
     * @returns {Promise} - Promise que resuelve con la respuesta del servidor
     */
    obtenerIpCliente: function () {
        return new Promise((resolve, reject) => {
            $.ajax({
                type: "POST",
                url: "IniciarSesion.aspx/ObtenerIpCliente",
                data: {},
                contentType: "application/json; charset=iso-8859-1",
                dataType: "json",
                cache: false,
                success: function (response) {
                    if (response.d && response.d.ip) {
                        console.log("IP del cliente:", response.d.ip);
                        resolve(response.d.ip);
                    } else {
                        reject("No se pudo obtener la IP del cliente");
                    }
                },
                error: function (xhr, status, error) {
                    reject("No se pudo obtener la IP del cliente");
                }
            });
        });
    },

    /**
     * Inicia sesión usando el WebMethod
     * @param {string} usuario - El nombre de usuario
     * @param {string} contrasena - La contraseña del usuario
     * @param {Function} onSuccess - Callback para éxito
     * @param {Function} onError - Callback para error
     */
    iniciarSesion: async function (usuario, contrasena, tokenRecaptcha, onSuccess, onError) {
        try {
            console.log("Iniciando llamada al WebMethod...");
            console.log("TokenRecaptcha", tokenRecaptcha)
            // Llamada al WebMethod para obtener la IP del cliente
            const ipCliente = await SessionManager.obtenerIpCliente();
            console.log("IP del cliente:", ipCliente);
    
            const payload = {
                "recaptchaToken": tokenRecaptcha,
                "ipAddress": ipCliente,
                "userAgent": navigator.userAgent,
                "usuario": usuario,
                "password": contrasena
            }
            
            localStorage.clear();

            const responseAuth = await ApiAutenticacion.IniciarSesion(payload);
            console.log("Respuesta de autenticación:", responseAuth);

            localStorage.setItem('ipCliente', ipCliente);
            localStorage.setItem('jwt', responseAuth.accessToken);
            localStorage.setItem('opcionesSistema', JSON.stringify(responseAuth.opcionesSistema));
            localStorage.setItem('parametroTabla', JSON.stringify(responseAuth.parametros));
            localStorage.setItem('listaAgentes', JSON.stringify(responseAuth.agentes));
            localStorage.setItem('listaAgentesExternos', JSON.stringify(responseAuth.agentesExternos));
    
            $.ajax({
                type: "POST",
                url: "IniciarSesion.aspx/CrearSesion",
                data: JSON.stringify({ 
                    usuario: responseAuth.datosUsuario,
                    opcionesSistema: responseAuth.opcionesSistema,
                    parametroTabla: responseAuth.parametros,
                    listaAgentes: responseAuth.agentes,
                    listaAgentesExternos: responseAuth.agentesExternos,
                    numAgente: responseAuth.datosUsuario ? responseAuth.datosUsuario.NumAgente : ""
                 }),
                contentType: "application/json; charset=iso-8859-1",
                dataType: "json",
                cache: false,
                beforeSend: function (xhr) {
                    
                    console.log("Enviando solicitud de autenticación...");
                    // Asegurarse de que no haya problemas con CORS o sesión
                    xhr.withCredentials = true;
                },
                success: function (response) {
                    const result = response.d;
    
                    if (result && result.success) {
                        if (onSuccess) {
                            onSuccess(result);
                        }
                    } else {
                        console.error("Error en inicio de sesión:", result ? result.mensaje : "Respuesta indefinida");
                        if (onError) {
                            onError(result && result.mensaje ? result.mensaje : "Error desconocido en el inicio de sesión");
                        }
                    }
                },
                error: function (xhr, status, error) {
                    console.error("Error en la llamada AJAX:", xhr, status, error);
                    let errorMessage = "Error de comunicación con el servidor.";
    
                    // Mostrar más detalles sobre el error
                    console.log("Estado de la respuesta:", xhr.status);
                    console.log("Texto de respuesta:", xhr.responseText);
    
                    // Intentar extraer mensaje de error más descriptivo
                    try {
                        if (xhr && xhr.responseJSON && xhr.responseJSON.Message) {
                            errorMessage = xhr.responseJSON.Message;
                        } else if (xhr && xhr.responseText) {
                            const errorObj = JSON.parse(xhr.responseText);
                            if (errorObj.Message) {
                                errorMessage = errorObj.Message;
                            } else if (errorObj.ExceptionMessage) {
                                errorMessage = errorObj.ExceptionMessage;
                            }
                        }
                    } catch (e) {
                        console.error("Error al procesar respuesta de error:", e);
                    }
    
                    if (onError) {
                        onError(errorMessage);
                    }
                }
            });
            
        } catch (error) {
            if (error.body && error.body.error) {
                onError(error.body.error.message);
            } else {
                onError("[Error] No se pudo iniciar sesión, por favor intente nuevamente.\n" + error.message);
            }
        }
    },

    redirigirPostLogin: function () {
        let urlANavegar = "../Principal.aspx";
        
        const urlParam = new URLSearchParams(window.location.search);
        const returnURL = urlParam.get('ReturnUrl');

        if (returnURL) {
            urlANavegar = returnURL;
        }

        window.location.href = urlANavegar;
    },

    /**
     * Actualiza la función Evaluar en IniciarSesion.aspx para usar el WebMethod
     */
    setupAuthentication: function () {
        if (typeof Evaluar === 'function') {
            // Variables de control a nivel global del script
            let procesandoAutenticacion = false;

            // Sobrescribir la función Evaluar
            window.Evaluar = function (e) {
                // Prevenir el comportamiento predeterminado
                e.preventDefault();

                // Si ya estamos procesando, no hacer nada
                if (procesandoAutenticacion) {
                    console.log("Ya hay un proceso de autenticación en curso");
                    return false;
                }

                // Capturar datos del formulario
                const usuario = $("#Usuario").val();
                const contrasena = $("#Contrasenha").val();

                // Validar datos básicos
                if (!usuario || !contrasena) {
                    $("#Error").html("Por favor, ingrese usuario y contraseña");
                    $("#Error").fadeIn();
                    return false;
                }

                // Mostrar estado de carga y deshabilitar el botón
                console.log("Deshabilitando botón...");
                procesandoAutenticacion = true;
                $("#Error").fadeOut();

                // Deshabilitar el botón de forma explícita
                $("#IniSesion").prop("disabled", true);
                $("#IniSesion").addClass("disabled");
                // También desactivar el parent si es un wrapper
                $("#IniSesion").parent().addClass("disabled");

                // También desactivar los inputs (temporalmente)
                $("#Usuario").prop("disabled", true);
                $("#Contrasenha").prop("disabled", true);

                // Añadir un indicador visual adicional
                $("#IniSesion").html("<i class='fa fa-spinner fa-spin'></i> Autenticando...");

                // Procesar reCAPTCHA si está disponible
                const ejecutarAutenticacion = function (token) {
                    // En caso de que se use reCAPTCHA
                    if (token) {
                        $("#reCAPTCHAToken").val(token);
                    }

                    // Llamar a nuestro método de inicio de sesión
                    SessionManager.iniciarSesion(
                        usuario,
                        contrasena,
                        token,
                        function (result) {
                            console.log("Autenticación exitosa, redirigiendo...");
                            // Éxito - redirigir a la página principal
                            SessionManager.redirigirPostLogin();
                        },
                        function (error) {
                            // Error - mostrar mensaje y RESETEAR ESTADO
                            console.log("Error de autenticación, habilitando botón...");
                            procesandoAutenticacion = false;

                            // Habilitar botón y restaurar texto
                            $("#IniSesion").prop("disabled", false);
                            $("#IniSesion").removeClass("disabled");
                            $("#IniSesion").parent().removeClass("disabled");
                            $("#IniSesion").html("Iniciar Sesión");

                            // Restaurar los inputs
                            $("#Usuario").prop("disabled", false);
                            $("#Contrasenha").prop("disabled", false);

                            // Mostrar error
                            $("#Error").html(error);
                            $("#Error").fadeIn();
                        }
                    );
                };

                // Si hay reCAPTCHA, procesarlo
                if (typeof grecaptcha !== 'undefined' && $("#reCAPTCHASiteKey").length) {
                    grecaptcha.ready(function () {
                        grecaptcha.execute($("#reCAPTCHASiteKey").val(), { action: "submit" })
                            .then(ejecutarAutenticacion)
                            .catch(function (error) {
                                console.log("Error en reCAPTCHA:", error);
                                procesandoAutenticacion = false;

                                // Habilitar botón y restaurar texto
                                $("#IniSesion").prop("disabled", false);
                                $("#IniSesion").removeClass("disabled");
                                $("#IniSesion").parent().removeClass("disabled");
                                $("#IniSesion").html("Iniciar Sesión");

                                // Mostrar error
                                $("#Error").html("Error en la verificación de seguridad");
                                $("#Error").fadeIn();
                            });
                    });
                } else {
                    // Sin reCAPTCHA, autenticar directamente
                    ejecutarAutenticacion();
                }

                return false;
            };
        }
    }
};

// Configurar la autenticación cuando el documento esté listo
$(document).ready(function () {
    SessionManager.setupAuthentication();
    SessionManager.obtenerIpCliente();
});