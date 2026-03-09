/*******************************\
|*          ANTICIPOS          *|
\*******************************/

$(document).ready(function () {
    /* Variables */
    var botonImprimirFormato = false;

    /* Botón Guardar Solicitud de Anticipo */
    $("#GuardarSolicitud").live("click", function () {
        // Validar que se haya aceptado las condiciones
        if (!$("#Acepto").is(':checked')) {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html('Debe aceptar las condiciones para seguir.');
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
            return false;
        }

        // Preparar los datos para enviar
        var params = {
            solicitud: $("#NumeroSolicitud").val(),
            idAgente: $("#IdAgente").text(),
        };

        // Realizar la llamada fetch al API
        console.log("llamando api", params);

        fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/registrar-anticipo-aceptacion', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=iso-8859-1',
                'x-username': $('#usuario_actual').val(),
                'x-rol': $('#rol_azman').val()
            },
            body: JSON.stringify(params)
        })
            .then(response => {
                if (!response.ok) {
                    if (response.status === 401 || response.status === 12030) {
                        document.location.reload(true);
                        return;
                    }
                    throw new Error("Error en la respuesta del servidor");
                }
                return response.json();
            })
            .then(data => {
                if (data.data.Respuesta.Estado == "OK") {
                       
                    // Deshabilitar controles y mostrar botón de imprimir

                    habilitarModoSolicitudProcesada();

                    var anticipoActual = sessionStorage.getItem('Anticipo');
                    if (anticipoActual) {
                        var anticipo = JSON.parse(anticipoActual);

                        anticipo.mensaje = "R";

                        sessionStorage.setItem('Anticipo', JSON.stringify(anticipo));
                    }

                    // Mostrar mensaje de éxito
                    $('#MCMIcono').attr('class', 'exito');
                    $('#MCMContenedor').html('La solicitud de anticipo ha sido guardada exitosamente.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Éxito' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                else if (data.data.Respuesta.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
                else {
                    $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                    $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            })
            .catch(error => {
                if (error.status === 401 || error.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al guardar la solicitud de anticipo.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            });

        return false;

        //$.ajax({
        //    type: "POST",
        //    url: rutaGenerarReporteSolicitudAnticipo,
        //    contentType: "application/json; charset=iso-8859-1",
        //    dataType: "json",
        //    data: $.toJSON(params),
        //    success: function (data) {
        //        if (data.d.Estado == "OK") {
        //            if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
        //                window.location.href = "../Reportes/SolicitudAnticipoMovil.aspx";
        //            }
        //            else {
        //                var w = 800;
        //                var h = 600;
        //                var left = (screen.width / 2) - (w / 2);
        //                var top = (screen.height / 2) - (h / 2);
        //                var nuevaVentana = window.open("../Reportes/SolicitudAnticipo.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
        //            }

        //            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        //            $clock.countdown(selectedDate.toString());
        //        }
        //        else if (data.d.Estado == "TOKEN") {
        //            CerrarSesionExpirada();
        //        }
        //        else {
        //            $('#MCMIcono').attr('class', data.d.Icono);
        //            $('#MCMContenedor').html(data.d.Mensaje);
        //            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
        //            $('#ModalCuadroMensaje').dialog('open');
        //        }
        //    },
        //    error: function (XMLHttpRequest, textStatus, errorThrown) {
        //        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //            /* Sesión caducada */
        //            document.location.reload(true);
        //        }
        //        else {
        //            $('#MCMIcono').attr('class', 'error');
        //            $('#MCMContenedor').html('Ha ocurrido un error al exportar la la solicitud.');
        //            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        //            $('#ModalCuadroMensaje').dialog('open');
        //        }
        //    }
        //});
    });

    $("#ImprimirFormato").live("click", function () {
        // Usar la función reutilizable definida en CWRV.generarPdf.js
        if (typeof window.generarPdfAdelanto === 'function') {
            window.generarPdfAdelanto($("#NumeroSolicitud").val());
        } else {
            console.error('La función generarPdfAdelanto no está disponible. Asegúrese de que CWRV.generarPdf.js esté cargado.');
            $('#MCMIcono').attr('class', 'error');
            $('#MCMContenedor').html('Error: No se pudo cargar la función para generar el PDF. Verifique que el archivo CWRV.generarPdf.js esté incluido.');
            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
            $('#ModalCuadroMensaje').dialog('open');
        }
    });

    // Consulta de Anticipos
    $("#BuscarAceptacionAnticipos").live("click", function () {
        CargarTablaSolicitudesAnticipo();
    });

    // Ordenar tabla de solicitudes de anticipo
    $("#TabSolicitudesAnticipo a").live("click", function () {
        //alert("Holi");
    });

    function CargarTablaSolicitudesAnticipo() {
        $("#ContenedorExportar").hide();
        $("#TablaSolicitudesAnticipoContenedor").hide();
        $("#TablaSolicitudesAnticipoCargando").show();

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            solicitud: $("#NumeroSolicitud").val(),
            fechaDesde: $("#FechaDesde").val(),
            fechaHasta: $("#FechaHasta").val(),
            indicePagina: $("#TabSolicitudesAnticipoIndicePagina").val(),
            tamanhoPagina: $("#TabSolicitudesAnticipoTamanhoPagina").val(),
            columnaOrdenar: $("#TabSolicitudesAnticipoColumnaOrdenar").val(),
            direccionOrdenar: $("#TabSolicitudesAnticipoDireccionOrdenar").val()
        }

        $.ajax({
            type: "POST",
            url: "ConsultaSolicitudesAnticipo.aspx/CargarTablaAceptacionAnticipo",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $("#TablaSolicitudesAnticipoCargando").hide();
                    $("#TablaSolicitudesAnticipoContenedor").show();
                    $("#TablaSolicitudesAnticipoContenedor").html($(data.d).find("#ContenidoDinamico").html());

                    if ($("#TabSolicitudesAnticipo thead").length > 0) {
                        $("#ContenedorExportar").show();
                    }
                }
                else {
                    CerrarSesionExpirada();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error cargar los datos de las solicitudes de anticipos.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");

                    $("#TablaSolicitudesAnticipoCargando").hide();
                    $("#TablaSolicitudesAnticipoContenedor").show();
                }
            }
        });
    }
});

function habilitarModoSolicitudProcesada() {
    // Deshabilitar checkbox de aceptación
    $("#Acepto").prop('disabled', true);

    // Deshabilitar botón de guardar
    $("#GuardarSolicitud").prop('disabled', true);
    $("#GuardarSolicitud").removeClass('boton darkblue sharp').addClass('botonDeshabilitado gris gris_sharp');

    // Mostrar botón de imprimir
    $("#ImprimirFormato").css('display', 'inline-block');
}