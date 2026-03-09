function postBack(url) {
    var theForm = document.forms['form1'];
    if (!theForm) {
        theForm = document.form1;
    }

    if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
        theForm.action = url;
        theForm.submit();
    }
    //function __doPostBack(eventTarget, eventArgument) {
    //    if (!theForm.onsubmit || (theForm.onsubmit() != false)) {
    //        theForm.__EVENTTARGET.value = eventTarget;
    //        theForm.__EVENTARGUMENT.value = eventArgument;
    //        theForm.submit();
    //    }
    //}
}

function CerrarSolicitud() {

    var params = {
        tokenUsuario: $("#TokenUsuario").val(),
        num_solicitud: $('#ModGruFamsolicitud').val(),
        num_correlativo: $('#ModGruNumCorrelativo').val(),
        objSolicitud: JSON.parse($('#HSolicitudSerializado').val())
    }

    $.ajax({
        type: 'POST',
        url: 'SeleccionSolicitud.aspx/CerrarCotizacionIFP',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == 'OK') {
                // Cambiar la modal a modo de modificación
                $('#ModSolModo').val('');
                // Cerrar modal de espera
                $('#ModalCotizando').dialog('close');

                // Mostrar mensaje de éxito
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#ModalCuadroMensaje').dialog('open');

                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

                setTimeout(function () {
                    window.location.href = "../RentaIFP/SeleccionSolicitud.aspx?seleccione=0";
                }, 1000);
            }
            else if (data.d.Estado == 'TOKEN') {
                CerrarSesionExpirada();
            }
            else {
                $('#ModalCotizando').dialog('close');

                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                $('#ModalCuadroMensaje').dialog('open');
                $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
            }
            $('#ModSolCargando').fadeOut();

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                document.location.reload(true);
            }
            else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');

                $('#ModalCotizando').dialog('close');
            }
        }
    });
}