$(document).ready(function () {

    $("#bControlCDA").live("click", function () {
        var periodo = $.trim($("#txtPeriodo").val())

        if (periodo == '') {
            $("#MCMIcono").attr("class", "error");
            $("#MCMContenedor").html("No se ha indicado el periodo para la carga de datos.");
            $("#ModalCuadroMensaje").dialog({ title: "Error" });
            $("#ModalCuadroMensaje").dialog("open");
            return;
        }

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            periodo
        }

        $.ajax({
            type: "POST",
            url: "CargaArchivosIndicadores.aspx/ValidaPeriodoControlCDA",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    if (data.d.Contenido == 'True') {
                        $('#MCADataPeriodo').val('PeriodoControlCDA');
                        $('#MCAIcono').attr('class', 'advertencia');
                        $('#MCAContenedor').html('Existe información de control CDA para el periodo indicado, ¿Desea reemplazarla?');
                        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
                        $('#ModalCuadroAdvertencia').dialog('open');
                    } else {
                        $("#btnControlCDA").click();
                    }
                }
                else if (data.d.Estado == "ERROR") {
                    $("#MCMIcono").attr("class", data.d.Icono);
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                    $("#ModalCuadroMensaje").dialog("open");
                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }

                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al cargar la información de los lotes.");
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            }
        });
    });

    $("#bLocalidadVCTP").live("click", function () {
        var periodo = $.trim($("#txtPeriodo").val())

        if (periodo == '') {
            $("#MCMIcono").attr("class", "error");
            $("#MCMContenedor").html("No se ha indicado el periodo para la carga de datos.");
            $("#ModalCuadroMensaje").dialog({ title: "Error" });
            $("#ModalCuadroMensaje").dialog("open");
            return;
        }

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            periodo
        }

        $.ajax({
            type: "POST",
            url: "CargaArchivosIndicadores.aspx/ValidaPeriodoLocalidadVCTP",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    if (data.d.Contenido == 'True') {
                        $('#MCADataPeriodo').val('PeriodoLocalidadVCTP');
                        $('#MCAIcono').attr('class', 'advertencia');
                        $('#MCAContenedor').html('Existe información de localidad VCTP para el periodo indicado, ¿Desea reemplazarla?');
                        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
                        $('#ModalCuadroAdvertencia').dialog('open');
                    } else {
                        $("#btnLocalidadVCTP").click();
                    }
                }
                else if (data.d.Estado == "ERROR") {
                    $("#MCMIcono").attr("class", data.d.Icono);
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                    $("#ModalCuadroMensaje").dialog("open");
                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }

                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al cargar la información de los lotes.");
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            }
        });
    });

    $("#bControlVCTP").live("click", function () {
        var periodo = $.trim($("#txtPeriodo").val())

        if (periodo == '') {
            $("#MCMIcono").attr("class", "error");
            $("#MCMContenedor").html("No se ha indicado el periodo para la carga de datos.");
            $("#ModalCuadroMensaje").dialog({ title: "Error" });
            $("#ModalCuadroMensaje").dialog("open");
            return;
        }

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            periodo
        }

        $.ajax({
            type: "POST",
            url: "CargaArchivosIndicadores.aspx/ValidaPeriodoControlVCTP",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    if (data.d.Contenido == 'True') {
                        $('#MCADataPeriodo').val('PeriodoControlVCTP');
                        $('#MCAIcono').attr('class', 'advertencia');
                        $('#MCAContenedor').html('Existe información de control VCTP para el periodo indicado, ¿Desea reemplazarla?');
                        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
                        $('#ModalCuadroAdvertencia').dialog('open');
                    } else {
                        $("#btnControlVCTP").click();
                    }
                }
                else if (data.d.Estado == "ERROR") {
                    $("#MCMIcono").attr("class", data.d.Icono);
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                    $("#ModalCuadroMensaje").dialog("open");
                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }

                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al cargar la información de los lotes.");
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            }
        });
    });

    /*BEGIN: Botón Aceptar Cuadro Advertencia */
    $('#MCAAceptar').live('click', function () {
        $('#ModalCuadroAdvertencia').dialog('close');

        if ($('#MCADataPeriodo').val() == 'PeriodoControlCDA') {
            $("#btnControlCDA").click();
        }

        if ($('#MCADataPeriodo').val() == 'PeriodoLocalidadVCTP') {
            $("#btnLocalidadVCTP").click();
        }

        if ($('#MCADataPeriodo').val() == 'PeriodoControlVCTP') {
            $("#btnControlVCTP").click();
        }
    });
    /*END: Botón Aceptar Cuado Advertencia */


    $('#MCACancelar').live('click', function () {
        $('#ModalCuadroAdvertencia').dialog('close');
    });
});