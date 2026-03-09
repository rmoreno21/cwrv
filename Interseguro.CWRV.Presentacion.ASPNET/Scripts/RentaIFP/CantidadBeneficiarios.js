

$(document).ready(function () {

    $('#ModGruCantBenef_IFP').autoNumeric({ vMin: '0', vMax: '99' });

    /* Botón Cantidad Beneficiario */
    $('#ModCantBenefAceptar_IFP').live('click', function () {
        
        if ($('#ModGruCantBenef_IFP').val() == 0 || $('#ModGruCantBenef_IFP').val().length == 0) {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html('Error en la cantidad de Beneficiarios.');
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
            return false;
        }

        var params = {
            total: $('#ModGruCantBenef_IFP').val(),
            cantidad: 1,
            paginaLlamada: '../RentaIFP/SeleccionSolicitud.aspx'
        }

        $.ajax({
            type: 'POST',
            url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/IngresarCantidadFamiliar',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                window.location.href = "GrupoFamiliarAfiliadoCierre.aspx";
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información del beneficiario.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                $('#ModalGrupoFamiliar').dialog('close');
            }
        });

    });



});

