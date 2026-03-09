/* Determinar si se trata de un dispositivo móvil */
if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
    movil = true;
}
else {
    movil = false;
}

$(document).ready(function () {

    CargarTablaBeneficiariosCierre_IFP();

    function CargarTablaBeneficiariosCierre_IFP() {

        $('#TablaBeneficiariosContenedor_IFP').hide();
        $('#TablaBeneficiariosCargando_IFP').show();

        var params = {
            tokenUsuario: $('#TokenUsuario').val()
        }

        $.ajax({
            type: 'POST',
            url: 'ResumenBeneficiarios.aspx/CargarTablaBeneficiarios',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaBeneficiariosCargando_IFP').hide();
                    $('#TablaBeneficiariosContenedor_IFP').show();
                    $('#TablaBeneficiariosContenedor_IFP').html($(data.d).find('#ContenidoDinamico').html());

                    $('.Porcentaje').autoNumeric({ vMin: '0', vMax: '100' });
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
                    $('#TablaBeneficiariosCargando_IFP').hide();
                    //$('#TablaGrupoFamiliarError_RP').show();
                }
            }
        });

    }

    $('#TabBeneficiarios_IFP input[type=text]').live('keyup', function () {

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.find('.ModSolBenId').val();

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            posicion: fil,
            porcentaje: $(this).val()
        }

        $.ajax({
            type: 'POST',
            url: 'ResumenBeneficiarios.aspx/ModificarPorcentaje',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    $('.Porcentaje').autoNumeric({ vMin: '0', vMax: '100' });
                    $('#ModGruSumaBenef_IFP').val(data.d.Mensaje);

                    $('#ModGruSumaBenef_IFP').removeClass('formTextboxError');
                    if (parseInt($('#ModGruSumaBenef_IFP').val()) != 100) {
                        $('#ModGruSumaBenef_IFP').addClass('formTextboxError');
                    }
                }
                else if (data.d.Estado == "ERROR") {
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog('open');
                }
                else if (data.d.Estado == 'TOKEN') {
                    CerrarSesionExpirada();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $('#TablaBeneficiariosCargando_IFP').hide();
                    //$('#TablaGrupoFamiliarError_RP').show();
                }
            }
        });



    });

    $('#TabBeneficiarios_IFP input[type=text]').live('change', function () {

        $(this).removeClass('formTextboxError');
        if (parseInt($(this).val()) == 0)
            $(this).addClass('formTextbox formTextboxError');
    });

    /*Cerrar Solicitud*/
    function CerrarSolicitud() {
        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            num_solicitud: $('#ModGruFamsolicitud').val(),
            num_correlativo: $('#ModGruNumCorrelativo').val(),
        }

        $.ajax({
            type: 'POST',
            url: 'SeleccionSolicitud.aspx/CerrarCotizacionIFP',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {

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

    $('#ModResumenBenefAceptar_IFP').live('click', function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Cerrando, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Cerrando" });
        $("#ModalCotizando").dialog("open");

    });
});
