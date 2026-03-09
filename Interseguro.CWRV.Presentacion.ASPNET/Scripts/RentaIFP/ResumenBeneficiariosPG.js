/* Determinar si se trata de un dispositivo móvil */
if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
    movil = true;
}
else {
    movil = false;
}

$(document).ready(function () {

    CargaPasosCierre();
    CargarTablaBeneficiariosCierre_IFP();
    CargarTablaBeneficiariosCierre_VIT_IFP();

    function CargaPasosCierre() {

        var params = {
            paso: 3,
            objeto: $("#HSolicitudSerializado").val()
        }

        $.ajax({
            type: 'POST',
            url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ObtenerPasosCierre',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                //pasos de cierre
                $("#pasosCierre").prepend(data.d[0]);

                $("#pasosCierre").steps({
                    headerTag: "h2",
                    bodyTag: "section",
                    transitionEffect: "slideLeft"
                });

                $($('.steps>ul').children()[data.d[1] - 1]).removeClass('disabled')
                $($('.steps>ul>li').children()[data.d[1] - 1]).click()

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar los pasos del cierre.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    }

    function CargarTablaBeneficiariosCierre_IFP() {

        $('#TablaBeneficiariosContenedor_IFP').hide();
        $('#TablaBeneficiariosCargando_IFP').show();

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            solicitud: $('#HSolicitudSerializado').val()
        }

        $.ajax({
            type: 'POST',
            url: 'ResumenBeneficiariosPG.aspx/CargarTablaBeneficiarios',
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

        if (fila.find('#seleccion').is(':checked')) {
           
        } else {
            fila.find('#ItemPorcentajeRenta').val(0);
        }

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            posicion: fil,
            porcentaje: $(this).val(),
            solicitud: $('#HSolicitudSerializado').val()
        }

        $.ajax({
            type: 'POST',
            url: 'ResumenBeneficiariosPG.aspx/ModificarPorcentaje',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    $('.Porcentaje').autoNumeric({ vMin: '0', vMax: '100' });
                    $('#ModGruSumaBenef_IFP').val(data.d.Contenido);

                    $('#ModGruSumaBenef_IFP').removeClass('formTextboxError');

                    if (parseInt($('#ModGruSumaBenef_IFP').val()) != 100) {
                        $('#ModGruSumaBenef_IFP').addClass('formTextboxError');
                    }

                    $('#HSolicitudSerializado').val(data.d.Mensaje)
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

    $('#TabBeneficiarios_IFP input[type=checkbox]').live('change', function () {

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.find('.ModSolBenId').val();

        var ind = 0;

        if (this.checked) {
            ind = 1;
        } else {
            ind = 0;
            fila.find('#ItemPorcentajeRenta').val(0);
            fila.find('#ItemPorcentajeRenta').trigger("keyup");
            //$("#ItemPorcentajeRenta").trigger("keyup");
        }

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            posicion: fil,
            indicador: ind,
            solicitud: $('#HSolicitudSerializado').val()
        }

        $.ajax({
            type: 'POST',
            url: 'ResumenBeneficiariosPG.aspx/ModificarCheckBox',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    $('#HSolicitudSerializado').val(data.d.Mensaje)
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

    /*Cerrar Solicitud*/
    //function CerrarSolicitud() {
    //    var params = {
    //        tokenUsuario: $("#TokenUsuario").val(),
    //        num_solicitud: $('#ModGruFamsolicitud').val(),
    //        num_correlativo: $('#ModGruNumCorrelativo').val(),
    //    }

    //    $.ajax({
    //        type: 'POST',
    //        url: 'SeleccionSolicitud.aspx/CerrarCotizacionIFP',
    //        contentType: "application/json; charset=iso-8859-1",
    //        dataType: 'json',
    //        data: $.toJSON(params),
    //        success: function (data) {
    //            if (data.d.Estado == 'OK') {

    //                // Cerrar modal de espera
    //                $('#ModalCotizando').dialog('close');

    //                // Mostrar mensaje de éxito
    //                $('#MCMIcono').attr('class', data.d.Icono);
    //                $('#MCMContenedor').html(data.d.Mensaje);
    //                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
    //                $('#ModalCuadroMensaje').dialog('open');

    //                botonModSolAceptarBloqueado = true;
    //                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

    //                setTimeout(function () {
    //                    window.location.href = "../RentaIFP/SeleccionSolicitud.aspx?seleccione=0";
    //                }, 1000);
    //            }
    //            else if (data.d.Estado == 'TOKEN') {
    //                CerrarSesionExpirada();
    //            }
    //            else {
    //                $('#ModalCotizando').dialog('close');

    //                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
    //                $('#MCMIcono').attr('class', data.d.Icono);
    //                $('#MCMContenedor').html(data.d.Mensaje);
    //                $('#ModalCuadroMensaje').dialog('open');
    //                $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
    //            }
    //            $('#ModSolCargando').fadeOut();

    //        },
    //        error: function (XMLHttpRequest, textStatus, errorThrown) {
    //            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
    //                document.location.reload(true);
    //            }
    //            else {
    //                $('#MCMIcono').attr('class', 'error');
    //                $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
    //                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
    //                $('#ModalCuadroMensaje').dialog('open');

    //                $('#ModalCotizando').dialog('close');
    //            }
    //        }
    //    });
    //}

    $('#ModResumenBenefAceptar_IFP').live('click', function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Cerrando, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Cerrando" });
        $("#ModalCotizando").dialog("open");

    });

    /* Botón Modificar */
    $('#ModAgregarBeneficiarios').live('click', function () {
        idGrupoFamiliar = 0;

        WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(this, "", false, "", "GrupoFamiliarAfiliadoCierre.aspx?indiceBeneficiario=" + idGrupoFamiliar + "&form=" + "PG", false, true))

    });

    /* Botón Modificar */
    $('.grilla_editar').live('click', function () {
        idGrupoFamiliar = $(this).data('grupofamiliar');

        WebForm_DoPostBackWithOptions(new WebForm_PostBackOptions(this, "", false, "", "GrupoFamiliarAfiliadoCierre.aspx?indiceBeneficiario=" + idGrupoFamiliar + "&form=" + "PG" + "&id=" + $(this).data('id') + "&pg=" + $(this).data('pg') + "&tipoidentificacion=" + $(this).data('tipoidentificacion') + "&numeroidentificacion=" + $(this).data('numeroidentificacion'), false, true))
    });

    /* Botón Cancelar */
    $('#ModGruFamCancelar_RP').live('click', function () {
        window.location.href = "Cotizador.aspx#datos_solicitud";
    });

    $('#ModAceptarCierre').live('click', function () {
        var existePorcentaje = false;
        var sumaPorcentaje = 0;

        $('#TabBeneficiarios_IFP input[type=checkbox]').each(function () {
            if (this.checked == true) {
                existePorcentaje = true;
            }
        });

        if (existePorcentaje) {
            for (i = 0; i < $("#TabBeneficiarios_IFP tbody tr").length; i++) {
                if ($("#TabBeneficiarios_IFP tbody tr:eq(" + i + ") input").is(":checked")) {
                    //sumaPorcentaje += Number($("#TabBeneficiarios_IFP tbody tr:eq(" + i + ") input#ItemPorcentajeRenta").val());
                    var porcentajeRenta = $("#TabBeneficiarios_IFP tbody tr:eq(" + i + ") input#ItemPorcentajeRenta").val();
                    if (porcentajeRenta == 0) {
                        $('#MCMIcono').attr('class', 'validacion');
                        $('#MCMContenedor').html('El porcentaje de cada beneficiario seleccionado debe ser mayor a 0');
                        $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                        $('#ModalCuadroMensaje').dialog('open');
                        return;
                    }
                    sumaPorcentaje += Number(porcentajeRenta);
                }
                else {
                    //$("#TabBeneficiarios_IFP tbody tr:eq(" + i + ") input#ItemPorcentajeRenta").val(0);
                }
            }

            $('#ModGruSumaBenef_IFP').val(sumaPorcentaje);

            if ($('#ModGruSumaBenef_IFP').val() == '100') {
                $('#MCATablaPregunta').val('cerrar');

                $("#MCAIcono").attr("class", "advertencia");

                var mensaje = '<div class="alerta-agente-titulo">Firma Digital de documentos de <span class="resaltado">Renta Particular IFP</span></div>';
                mensaje += '<div class="alerta-agente-contenido"><p>Usted va a enviar el enlace de Firma Digital a <span class="resaltado">' + $('#HNombres').val() + '</span> cuyo documento de identidad es <span class="resaltado">' + $("#HTipoDocumento").val() + ' ' + $("#HNumeroDocumento").val() + '</span> al siguiente correo electrónico: <span class="resaltado">' + $('#HCorreo').val() + '</span>.</p>';
                mensaje += '<p>Para que el cliente pueda firmar digitalmente sus documentos, luego de que valide su información se le enviará una clave al siguiente número de celular: <span class="resaltado">' + $("#HTelefono").val() + '</span>.</p>';
                mensaje += '<p>Utilice la <b>Vista Previa</b> de los documentos para validar que todos los datos que le enviará al cliente son correctos, en caso de que haya algún error por favor modifique los datos en el Cotizador Web de Rentas y luego vuelva a validar en la opción de <b>Vista Previa</b>.</p>';
                mensaje += '<p>Para evitar inconvenientes por favor <b>valide cuidadosamente</b> que toda la información del cliente es correcta antes de enviarle el enlace de Firma Digital.</p></div >';

                $("#MCAContenedor").html(mensaje);

                $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                $("#ModalCuadroAdvertencia").dialog("open");
                $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');

            } else {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html('La suma de porcentajes de los beneficiarios seleccionados, debe ser 100%');
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return;
            }
        } else {
            $('#MCATablaPregunta').val('cerrar');

            $("#MCAIcono").attr("class", "advertencia");

            var mensaje = '<div class="alerta-agente-titulo">Firma Digital de documentos de <span class="resaltado">Renta Particular IFP</span></div>';
            mensaje += '<div class="alerta-agente-contenido"><p>Usted va a enviar el enlace de Firma Digital a <span class="resaltado">' + $('#HNombres').val() + '</span> cuyo documento de identidad es <span class="resaltado">' + $("#HTipoDocumento").val() + ' ' + $("#HNumeroDocumento").val() + '</span> al siguiente correo electrónico: <span class="resaltado">' + $('#HCorreo').val() + '</span>.</p>';
            mensaje += '<p>Para que el cliente pueda firmar digitalmente sus documentos, luego de que valide su información se le enviará una clave al siguiente número de celular: <span class="resaltado">' + $("#HTelefono").val() + '</span>.</p>';
            mensaje += '<p>Utilice la <b>Vista Previa</b> de los documentos para validar que todos los datos que le enviará al cliente son correctos, en caso de que haya algún error por favor modifique los datos en el Cotizador Web de Rentas y luego vuelva a validar en la opción de <b>Vista Previa</b>.</p>';
            mensaje += '<p>Para evitar inconvenientes por favor <b>valide cuidadosamente</b> que toda la información del cliente es correcta antes de enviarle el enlace de Firma Digital.</p></div >';

            //$("#MCAContenedor").html("Se enviará el link al correo " + $('#HCorreo').val() + " del cliente " + $('#HNombres').val());
            $("#MCAContenedor").html(mensaje);

            $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
            $("#ModalCuadroAdvertencia").dialog("open");
            $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');
        }
    });

    /* Botón SI Beneficiario */
    $('#MCASiBeneficiario').live('click', function () {
        if ($('#MCATablaPregunta').val() == 'cerrar') {
            $('#ModalCuadroAdvertencia').dialog('close');
            //CerrarSolicitud();

            $("#MCIcono").attr("class", "cargando");
            $("#MCContenedor").html("Enviando información, por favor espere un momento...");
            $("#ModalCotizando").dialog({ title: "Enviando" });
            $("#ModalCotizando").dialog("open");

            var infoSolicitud = JSON.parse($('#HSolicitudSerializado').val())

            var params = {
                nombres: $('#HSoloNombre').val(),
                numSolicitud: $('#ModGruFamsolicitud').val(),
                num_item: 1,
                correo: $("#HCorreo").val(),
                token: $("#HToken").val(),
                numCuspp: $('#HCUSPP').val(),
                codPlan: infoSolicitud.Cotizaciones[0].Plan.Id,
                objSolicitud: JSON.parse($('#HSolicitudSerializado').val())
            }

            $.ajax({
                type: 'POST',
                url: 'ResumenBeneficiariosPG.aspx/EnviarSADP',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    if (data.d.Estado == 'OK') {

                        // Cerrar modal de espera
                        $('#ModalCotizando').dialog('close');

                        //$("#ModCierreEnviar").hide();

                        //$('#CierreBeneficiarioMensajeExito').html("Se envió satisfactoriamente el enlace al cliente para la Validación y Consentimiento para Tratamiento de Datos Personales (VCTP) a: <b>" + $("#HCorreo").val() + "</b>");
                        //$('#CierreBeneficiarioMensajeExito').show();

                        //Mostrar mensaje de éxito
                        $('#MCMIcono').attr('class', data.d.Icono);
                        $('#MCMContenedor').html(data.d.Mensaje);
                        $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                        $('#ModalCuadroMensaje').dialog('open');

                        botonModSolAceptarBloqueado = true;
                        $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

                        setTimeout(function () {
                            window.location.href = "../RentaIFP/Cotizador.aspx#datos_solicitud";
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
                    //$('#ModSolCargando').fadeOut();

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
    });

    /* Botón NO Beneficiario */
    $('#MCANoBeneficiario').live('click', function () {
        if ($('#MCATablaPregunta').val() == 'cerrar') {
            $('#ModalCuadroAdvertencia').dialog('close');
        }
    });

    $('#ModRegresar').live('click', function () {

        var validaBenCA = false;

        if ($('#HSolicitudSerializado') != undefined) {

            var lstBeneficiarios = JSON.parse($('#HSolicitudSerializado').val()).Beneficiarios;

            for (var b in lstBeneficiarios) {
                if (lstBeneficiarios[b].TipoCobertura == 'CA') {
                    validaBenCA = true;
                }
            }

        }

        if (validaBenCA) {
            postBack('ResumenBeneficiariosCA.aspx');
        }
        else {
            postBack('GrupoFamiliarAfiliadoCierre.aspx');
        }

    });

    //$("#ReenvioManual").live("click", function () {
    //    var url = 
    //});

    $('#ModVistaPrevia').live('click', function () {

        var existePorcentaje = false;
        var sumaPorcentaje = 0;

        $('#TabBeneficiarios_IFP input[type=checkbox]').each(function () {
            if (this.checked == true) {
                existePorcentaje = true;
            }
        });

        if (existePorcentaje) {

            for (i = 0; i < $("#TabBeneficiarios_IFP tbody tr").length; i++) {
                if ($("#TabBeneficiarios_IFP tbody tr:eq(" + i + ") input").is(":checked")) {
                    var porcentajeRenta = $("#TabBeneficiarios_IFP tbody tr:eq(" + i + ") input#ItemPorcentajeRenta").val();
                    if (porcentajeRenta == 0) {
                        $('#MCMIcono').attr('class', 'validacion');
                        $('#MCMContenedor').html('El porcentaje de cada beneficiario seleccionado debe ser mayor a 0');
                        $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                        $('#ModalCuadroMensaje').dialog('open');
                        return;
                    }
                    sumaPorcentaje += Number(porcentajeRenta);
                }
                else {
                    //$("#TabBeneficiarios_IFP tbody tr:eq(" + i + ") input#ItemPorcentajeRenta").val(0);
                }
            }

            $('#ModGruSumaBenef_IFP').val(sumaPorcentaje);

            if ($('#ModGruSumaBenef_IFP').val() != '100') {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html('La suma de porcentajes de los beneficiarios seleccionados, debe ser 100%');
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return;
            }
        }

        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Generando reporte, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Cargando" });
        $("#ModalCotizando").dialog("open");

        var infoSolicitud = JSON.parse($('#HSolicitudSerializado').val())

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            numSolicitud: $('#ModGruFamsolicitud').val(),
            numCorrelativo: $('#ModGruNumCorrelativo').val(),
            numCuspp: $('#HCUSPP').val(),
            indPEP: $('#HPEP').val(),
            codPlan: infoSolicitud.Cotizaciones[0].Plan.Id,
            objSolicitud: JSON.parse($('#HSolicitudSerializado').val())
        }

        $.ajax({
            type: 'POST',
            url: 'ResumenBeneficiariosPG.aspx/VistaPreviaIFP',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {

                    //// Mostrar mensaje de éxito
                    //$('#MCMIcono').attr('class', data.d.Icono);
                    //$('#MCMContenedor').html(data.d.Mensaje);
                    //$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    //$('#ModalCuadroMensaje').dialog('open');

                    //for (var nombre = 0; nombre < data.d.Archivos.length; nombre++) {
                    //    window.open('../Plantilla/IFP/' + data.d.Archivos[nombre], '_blank');
                    //}

                    const cuspp = $("#HCUSPP").val();
                    const solicitud = $("#ModGruFamsolicitud").val();
                    const esPEP = $("#HPEP").val();

                    // Cargar los reportes
                    window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=2");
                    window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=3");
                    if (esPEP === "S") {
                        window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=4");
                    }
                    window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=5");
                    window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=8");

                    if (data.d.Contenido == "1") {
                        window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=7");
                    }

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else if (data.d.Estado == 'TOKEN') {
                    CerrarSesionExpirada();
                }
                else {
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog('open');
                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al generar los formatos.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            },
            complete: function () {
                $("#ModalCotizando").dialog("close");
            }
        });
    });


    function CargarTablaBeneficiariosCierre_VIT_IFP() {

        var infoSolicitud = JSON.parse($('#HSolicitudSerializado').val())

        if (infoSolicitud.Cotizaciones[0] != null) {
            if (infoSolicitud.Cotizaciones[0].Plan.Id == 'PLAN3') {
                $('#tabla_VIT').show();

                $('#TablaBeneficiariosContenedor_IFP_VIT').hide();
                $('#TablaBeneficiariosCargando_IFP_VIT').show();

                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    solicitud: $('#HSolicitudSerializado').val()
                }

                $.ajax({
                    type: 'POST',
                    url: 'ResumenBeneficiariosPG.aspx/CargarTablaBeneficiariosVitalicios',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d != 'TOKEN') {
                            $('#TablaBeneficiariosCargando_IFP_VIT').hide();
                            $('#TablaBeneficiariosContenedor_IFP_VIT').show();
                            $('#TablaBeneficiariosContenedor_IFP_VIT').html($(data.d).find('#ContenidoDinamico').html());

                            //$('.Porcentaje').autoNumeric({ vMin: '0', vMax: '100' });
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
                            $('#TablaBeneficiariosCargando_IFP_VIT').hide();
                        }
                    }
                });

            }
            else {
                $('#tabla_VIT').hide();
            }
        } else {
            $('#tabla_VIT').hide();
        }

    }


});
