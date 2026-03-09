
$(document).ready(function () {
    CargarTablaDirecciones();
    CargarTablaTelefonos();
    CargarTablaGrupoFamiliar_RP();
    CargarTablaSolicitudes_RP();

    $('#TabDireccionesVerMas').live('click', function () {
        if ($('#TabDireccionesDesplegar_RP').val() == '0') {
            $('#TabDireccionesDesplegar_RP').val('1');
            $('#TabDirecciones tbody').find('tr:gt(0)').show();
            $('#TabDireccionesVerMas').html('Ver más reciente');
        }
        else {
            $('#TabDireccionesDesplegar_RP').val('0');
            $('#TabDirecciones tbody').find('tr:gt(0)').hide();
            $('#TabDireccionesVerMas').html('Ver más direcciones');
        }
    });

    $('#TablaAfiliadosReintentar').live('click', function () {
        $('#TablaAfiliadosError_RP').hide();
        CargarTablaAfiliados_RP();
    });

    $('#TablaDireccionesReintentar').live('click', function () {
        $('#TablaDireccionesError_RP').hide();
        CargarTablaDirecciones();
    });

    $('#TabTelefonosVerMas').live('click', function () {
        if ($('#TabTelefonosDesplegar_RP').val() == '0') {
            $('#TabTelefonosDesplegar_RP').val('1');
            $('#TabTelefonos tbody').find('tr:gt(0)').show();
            $('#TabTelefonosVerMas').html('Ver más reciente');
        }
        else {
            $('#TabTelefonosDesplegar_RP').val('0');
            $('#TabTelefonos tbody').find('tr:gt(0)').hide();
            $('#TabTelefonosVerMas').html('Ver más teléfonos');
        }
    });

    $('#TablaTelefonosReintentar').live('click', function () {
        $('#TablaTelefonosError_RP').hide();
        CargarTablaTelefonos();
    });

    $('#TablaGrupoFamiliarReintentar_RP').live('click', function () {
        $('#TablaGrupoFamiliarError_RP').hide();
        CargarTablaGrupoFamiliar_RP();
    });

    $('#TablaSolicitudesReintentar_RP').live('click', function () {
        $('#TablaSolicitudesError_RP').hide();
        CargarTablaSolicitudes_RP();
    });

    $('#TablaSolicitudesSimuladorReintentar').live('click', function () {
        $('#TablaSolicitudesError_RP').hide();
        CargarTablaSolicitudesSimulador();
    });

    $('#TablaBeneficiariosReintentar_RP').live('click', function () {
        $('#TablaBeneficiariosError_RP').hide();
        CargarTablaBeneficiarios_RP(null);
    });

    $('#TabActividadesVerMas').live('click', function () {
        if ($('#TabActividadesDesplegar').val() == '0') {
            $('#TabActividadesDesplegar').val('1');
            $('#TabActividades tbody').find('tr:gt(9)').show();
            $('#TabActividadesVerMas').html('Ver más recientes');
        }
        else {
            $('#TabActividadesDesplegar').val('0');
            $('#TabActividades tbody').find('tr:gt(9)').hide();
            $('#TabActividadesVerMas').html('Ver más actividades');
        }
    });

    $('#TablaActividadesReintentar').live('click', function () {
        $('#TablaActividadesError').hide();
        CargarTablaActividades();
    });

    $('#TablaSeguimientoReintentar').live('click', function () {
        $('#TablaSeguimientoError').hide();

        CargarTablaSeguimiento($('#HJefe').val(), $('#HSupervisor').val(), $('#HAgente').val(), $('#HCUSPP_RP').val(), $('#HFechaInicio').val(), $('#HFechaTermino').val());
    });

    $('#TablaSolicitudesOficialesReintentar').live('click', function () {
        $('#TablaSolicitudesOficialesError').hide();

        SegIdJefe = ($('#OfiJefe').length > 0) ? $('#OfiJefe').val() : "0";
        SegIdSupervisor = ($('#OfiSupervisor').length > 0) ? $('#OfiSupervisor').val() : "0";
        SegIdAgente = $('#OfiAgente').val();

        $('#HJefe').val(SegIdJefe);
        $('#HSupervisor').val(SegIdSupervisor);
        $('#HAgente').val(SegIdAgente);

        $('#TabSeguimientoIndicePagina').val(1);
        $('#TabSeguimientoColumnaOrdenar').val(1);
        $('#TabSeguimientoDireccionOrdenar').val('A');

        CargarTablaSolicitudesOficiales(SegIdJefe, SegIdSupervisor, SegIdAgente);
    });

    /*Agrupadores*/
    $('#AgrupadorDireccion_RP').click(function () {
        $('#DatosDireccion_RP').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosDireccion_RP').is(':visible'))
                $('#AgrupadorDireccion_RP span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorDireccion_RP span').attr('class', 'agrupador_titulo_mas');
        });
    });

    $('#AgrupadorTelefono_RP').click(function () {
        $('#DatosTelefono_RP').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosTelefono_RP').is(':visible'))
                $('#AgrupadorTelefono_RP span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorTelefono_RP span').attr('class', 'agrupador_titulo_mas');
        });
    });

    $('#AgrupadorEmpresa_RP').click(function () {
        $('#DatosEmpresa_RP').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosEmpresa_RP').is(':visible'))
                $('#AgrupadorEmpresa_RP span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorEmpresa_RP span').attr('class', 'agrupador_titulo_mas');
        });
    });
});

function ActualizaEstiloCombobox(idControl) {
    $(idControl).each(function (i) {
        var listaClases = $(this).attr('class').split(/\s+/);
        var clases = '';
        $.each(listaClases, function (index, item) {
            clases += item + 'Contenedor ';
        });
        var margenOriginal = $(this).css('margin-left');
        $(this).wrap('<div id="Con' + $(this).attr('id') + '" class="' + $.trim(clases) + '" style="width:' + $(this).width() + 'px' + ((margenOriginal == "0px" || margenOriginal == "auto") ? '' : (';margin-left:' + $(this).css('margin-left'))) + '" />');
        $(this).css('margin-left', 'auto');
        $('<span id="Tex' + $(this).attr('id') + '" class="formComboboxSpan">' + $(this).find(':selected').text() + '</span>').insertAfter($(this));
    });
}

$(function () {
    $('.formCalendar').datepicker({
        yearRange: "1900:2200",
        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sá"],
        dateFormat: "dd/mm/yy",
        showAnim: "fadeIn",
        changeMonth: true,
        changeYear: true
    });

    $(document).tooltip();
});

jQuery(function ($) {
    $('.numerico').autoNumeric('init', { aSep: ',', aDec: '.' });

    $('#ModSolMontoCIA').autoNumeric();
    $('#ModSolPensionCIA').autoNumeric();
    $('#ModSolPensionCIAMO').autoNumeric();
    $('#ModSolTasaAFP').autoNumeric();
    $('#ModSolMontoAFP').autoNumeric();
    $('#ModSolPensionAFP').autoNumeric();
    $('#ModTasaVenta').autoNumeric();
    $('#ModTasaVentaSbs').autoNumeric();

    $("#ModSolSaldoCIC_RP").autoNumeric();

    $(".enteroPositivo").numeric();
    $(".telefono").numeric({ allow: "#*+" });
    $(".alfanumerico").alphanumeric({ allow: "ñÑ" });
    $(".nombre").alpha({ allow: "áéíóúÁÉÍÓÚñÑäëïöüÄËÏÖÜàèìòùÀÈÌÒÙ'- " });
    $(".fecha").mask("99/99/9999");


});

function CargarTablaAfiliados_RP(apellidoPaterno, apellidoMaterno, nombres) {
    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        apellidoPaterno: apellidoPaterno,
        apellidoMaterno: apellidoMaterno,
        nombres: nombres,
        indicePagina: $('#TabAfiliadosIndicePagina_RP').val(),
        tamanhoPagina: $('#TabAfiliadosTamanhoPagina_RP').val(),
        columnaOrdenar: $('#TabAfiliadosColumnaOrdenar_RP').val(),
        direccionOrdenar: $('#TabAfiliadosDireccionOrdenar_RP').val()
    }

    botonesBloqueados = true;
    $('#ModBusAfiBuscar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#ModBusAfiTablaAfiliados_RP').hide();
    $('#ModBusAfiCargando_RP').show();
    $('#ModalBusquedaAfiliados_RP').dialog({ position: ['center', 'center'] });

    ajaxTablaAfiliados = $.ajax({
        type: 'POST',
        url: rutaCargarTablaAfiliados,
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                botonesBloqueados = false;
                $('#ModBusAfiBuscar_RP').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiCargando_RP').hide();
                $('#ModBusAfiTablaAfiliados_RP').html($(data.d.Contenido).find('#ContenidoDinamico').html());
                $('#ModBusAfiTablaAfiliados_RP').show();
                $('#ModalBusquedaAfiliados_RP').dialog({ position: ['center', 'center'] });
            }
            else if (data.d.Estado == "ERROR") {
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                if (data.d.Controles != null) {
                    if (data.d.Controles[0].length) $('#ModBusAfiApellidoPaterno_RP').attr('class', data.d.Controles[0]);
                    if (data.d.Controles[1].length) $('#ModBusAfiApellidoMaterno_RP').attr('class', data.d.Controles[1]);
                    if (data.d.Controles[2].length) $('#ModBusAfiNombres_RP').attr('class', data.d.Controles[2]);
                }
                $('#ModalCuadroMensaje').dialog('open');

                botonesBloqueados = false;
                $('#ModBusAfiBuscar_RP').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiCargando_RP').hide();
                $('#ModalBusquedaAfiliados_RP').dialog({ position: ['center', 'center'] });
            }
            else if (data.d.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión expirada, recargar para redirigir a Iniciar Sesión */
                document.location.reload(true);
            }
            else {
                botonesBloqueados = false;
                $('#ModBusAfiBuscar_RP').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiCargando_RP').hide();
                $('#TablaAfiliadosError_RP').show();
            }
        }
    });
}

function CargarTablaAfiliados2(apellidoPaterno, apellidoMaterno, nombres) {
    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        apellidoPaterno: apellidoPaterno,
        apellidoMaterno: apellidoMaterno,
        nombres: nombres,
        indicePagina: $('#TabAfiliadosIndicePagina_RP').val(),
        tamanhoPagina: $('#TabAfiliadosTamanhoPagina_RP').val(),
        columnaOrdenar: $('#TabAfiliadosColumnaOrdenar_RP').val(),
        direccionOrdenar: $('#TabAfiliadosDireccionOrdenar_RP').val()
    }

    botonesBloqueados = true;
    $('#ModBusAfiBuscar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#ModBusAfiAceptar').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#TabAfiliadosPaginadorCargando').show();

    ajaxTablaAfiliados = $.ajax({
        type: 'POST',
        url: rutaCargarTablaAfiliados,
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                botonesBloqueados = false;
                $('#ModBusAfiBuscar_RP').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiAceptar').attr('class', 'boton darkblue sharp');
                $('#TabAfiliadosPaginadorCargando').hide();
                $('#ModBusAfiTablaAfiliados_RP').html($(data.d.Contenido).find('#ContenidoDinamico').html());
            }
            else if (data.d.Estado == "ERROR") {
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                if (data.d.Controles != null) {
                    if (data.d.Controles[0].length) $('#ModBusAfiApellidoPaterno_RP').attr('class', data.d.Controles[0]);
                    if (data.d.Controles[1].length) $('#ModBusAfiApellidoMaterno_RP').attr('class', data.d.Controles[1]);
                    if (data.d.Controles[2].length) $('#ModBusAfiNombres_RP').attr('class', data.d.Controles[2]);
                }
                $('#ModalCuadroMensaje').dialog('open');

                botonesBloqueados = false;
                $('#ModBusAfiBuscar_RP').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiAceptar').attr('class', 'boton darkblue sharp');
                $('#TabAfiliadosPaginadorCargando').hide();
            }
            else if (data.d.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión expirada, recargar para redirigir a Iniciar Sesión */
                document.location.reload(true);
            }
            else {
                botonesBloqueados = false;
                $('#ModBusAfiBuscar_RP').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiAceptar').attr('class', 'boton darkblue sharp');

                $('#ModBusAfiTablaAfiliados_RP').hide();
                $('#TabAfiliadosPaginadorCargando').hide();
                $('#TablaAfiliadosError_RP').show();
            }
        }
    });
}

function CargarTablaDirecciones() {
    if ($.trim($('#CUSPP_RP').val()).length > 0) {
        $('#TablaDireccionesContenedor_RP').hide();
        $('#TablaDireccionesCargando_RP').show();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#CUSPP_RP').val()
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaDirecciones',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaDireccionesCargando_RP').hide();
                    $('#TablaDireccionesContenedor_RP').show();
                    $('#TablaDireccionesContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());

                    if ($('#TabDireccionesDesplegar_RP').val() == '0') {
                        $('#TabDirecciones tbody').find('tr:gt(0)').hide();
                        $('#TabDireccionesVerMas').html('Ver más direcciones');
                    }
                    else {
                        $('#TabDireccionesVerMas').html('Ver más reciente');
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
                    $('#TablaDireccionesCargando_RP').hide();
                    $('#TablaDireccionesError_RP').show();
                }
            }
        });
    }
}

function CargarTablaTelefonos() {
    if ($.trim($('#CUSPP_RP').val()).length > 0) {
        $('#TablaTelefonosContenedor_RP').hide();
        $('#TablaTelefonosCargando_RP').show();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#CUSPP_RP').val()
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaTelefonos',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaTelefonosCargando_RP').hide();
                    $('#TablaTelefonosContenedor_RP').show();
                    $('#TablaTelefonosContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());

                    if ($('#TabTelefonosDesplegar_RP').val() == '0') {
                        $('#TabTelefonos tbody').find('tr:gt(0)').hide();
                        $('#TabTelefonosVerMas').html('Ver más teléfonos');
                    }
                    else {
                        $('#TabTelefonosVerMas').html('Ver más reciente');
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
                    $('#TablaTelefonosCargando_RP').hide();
                    $('#TablaTelefonosError_RP').show();
                }
            }
        });
    }
}

function CargarTablaGrupoFamiliar_RP() {
    $('#divListaNegra').hide();
    if ($.trim($('#HCUSPP_RP').val()).length > 0) {
        $('#TablaGrupoFamiliarContenedor_RP').hide();
        $('#TablaGrupoFamiliarCargando_RP').show();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#HCUSPP_RP').val()
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaGrupoFamiliar',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#divListaNegra').hide();
                    $('#TablaGrupoFamiliarCargando_RP').hide();
                    $('#TablaGrupoFamiliarContenedor_RP').show();
                    $('#TablaGrupoFamiliarContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());

                    if ($('#HListaNegra').val() == 'S') {
                        $('#divListaNegra').show();
                    } else {
                        $('#divListaNegra').hide();
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
                    $('#TablaGrupoFamiliarCargando_RP').hide();
                    $('#TablaGrupoFamiliarError_RP').show();
                }
            }
        });
    }
}

function CargarTablaSolicitudes_RP() {
    if ($.trim($('#HCUSPP_RP').val()).length > 0) {
        $('#TablaSolicitudesContenedor_RP').hide();
        $('#TablaSolicitudesCargando_RP').show();
        //<INI.GTI_7012_3>
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#HCUSPP_RP').val(),
            mostrarTodos: $('#chkTodasSolicitudes').is(':checked')
        }
        //<INI.GTI_7012_3>
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaSolicitudes',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaSolicitudesCargando_RP').hide();
                    $('#TablaSolicitudesContenedor_RP').show();
                    $('#TablaSolicitudesContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
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
                    $('#TablaSolicitudesCargando_RP').hide();
                    $('#TablaSolicitudesError_RP').show();
                }
            }
        });
    }
}

function CargarTablaBandejaSolicitudesOficiales(OfiJefe, OfiSupervisor, OfiAgente) {
    if ($.trim(OfiJefe).length > 0) {
        $('#TablaSolicitudesOficialesError').hide();
        $('#TablaSolicitudesOficialesContenedor').hide();
        $('#TablaSolicitudesOficialesCargando').show();

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            numJefe: OfiJefe,
            numSupervisor: OfiSupervisor,
            numAgente: OfiAgente
        }
        $.ajax({
            type: 'POST',
            url: 'BandejaFlujoCotizacion.aspx/CargarTablaBandejaSolicitudesOficiales',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaSolicitudesOficialesError').hide();
                    $('#TablaSolicitudesOficialesCargando').hide();
                    $('#TablaSolicitudesOficialesContenedor').show();
                    $('#TablaSolicitudesOficialesContenedor').html($(data.d).find('#ContenidoDinamico').html());
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
                    $('#TablaSolicitudesOficialesCargando').hide();
                    $('#TablaSolicitudesOficialesError').show();
                }
            }
        });
    }
}

function CargarTablaSolicitudesOficiales(OfiJefe, OfiSupervisor, OfiAgente) {
    if ($.trim(OfiJefe).length > 0) {
        $('#TablaSolicitudesOficialesError').hide();
        $('#TablaSolicitudesOficialesContenedor').hide();
        $('#TablaSolicitudesOficialesCargando').show();

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            numJefe: OfiJefe,
            numSupervisor: OfiSupervisor,
            numAgente: OfiAgente
        }
        $.ajax({
            type: 'POST',
            url: 'CotizadorOficiales.aspx/CargarTablaSolicitudesOficiales',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaSolicitudesOficialesError').hide();
                    $('#TablaSolicitudesOficialesCargando').hide();
                    $('#TablaSolicitudesOficialesContenedor').show();
                    $('#TablaSolicitudesOficialesContenedor').html($(data.d).find('#ContenidoDinamico').html());
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
                    $('#TablaSolicitudesOficialesCargando').hide();
                    $('#TablaSolicitudesOficialesError').show();
                }
            }
        });
    }
}

function CargarTablaCotizaciones_RP(cot, temporalidad, moneda, conyuge) {
    $("#TablaCotizacionesContenedor_RP").hide();
    $("#TablaCotizacionesCargando_RP").show();
    var params = {
        cotizaciones: cot,
        temporalidad: temporalidad,
        moneda: moneda,
        conyuge: conyuge //<INIGTI_753>
    }
    $.ajax({
        type: "POST",
        url: "MantenerSolicitud.aspx/CargarTablaCotizaciones",//<INIGTI_753>
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {
            $('#TablaCotizacionesContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaCotizacionesCargando_RP').hide();
            $('#TablaCotizacionesContenedor_RP').show();
            $('#ModalSolicitud').dialog({ position: ['center', 'center'] });

            $("#TabCotizaciones_RP select").trigger("change");

            $('.numerico').autoNumeric('init');

            //<INIGTI_753_3>

            if ($('#ModSolModo').val() == 'C') {
                $('#ModSolNroSolicitud_RP').val('');
                $('#ModSolModo').val('N');
                $('#ModSolTipoCambioPanel').hide();
                for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                    Solicitud.Cotizaciones[i].Correlativo = "0";

                }
            }
            else if ($('#ModSolModo').val() == 'CONS') {
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

            } else {
                if ($('#HSeleccionada').val() == "S") {
                    botonModSolAceptarBloqueado = true;
                    $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                } else {
                    //<INIGTI_753_3>
                    if ($("#HBloqueo").val() == "TRUE" && ($('#ModSolModo').val() == "M" || $('#ModSolModo').val() == "CONS")) {

                        botonModSolAceptarBloqueado = true;
                        $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                    } else {

                        //<INIGTI_7012>
                        if ($('#HEstado').val() == "1") {//Seleccionada
                            botonModSolAceptarBloqueado = true;
                            $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                        } else if ($('#HEstado').val() == "2") {//Cerrado
                            botonModSolAceptarBloqueado = true;
                            $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                        }
                        else if ($('#HEstado').val() == "3") {//Anulado
                            botonModSolAceptarBloqueado = true;
                            $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                        }
                        else {//Cotizado
                            botonModSolAceptarBloqueado = false;
                            $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                        }
                    }
                    //<FINGTI_7012>
                    //{
                    //    botonModSolAceptarBloqueado = false;
                    //    $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                    //}
                    //<FINGTI_753_3>
                    //botonModSolAceptarBloqueado = false;
                    //$('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                }

            }
            //<FINGTI_753_3>


            //ActualizaEstiloCombobox('#TabCotizaciones_RP select');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaCotizacionesCargando_RP').hide();
                $('#TablaCotizacionesError').show();
            }
        }
    });
}

function CargarTablaCotizacionesCierre_RP(cot, temporalidad, moneda, conyuge) {
    $("#TablaCotizacionesContenedor_RP").hide();
    $("#TablaCotizacionesCargando_RP").show();
    var params = {
        cotizaciones: cot,
        temporalidad: temporalidad,
        moneda: moneda,
        conyuge: conyuge
    }
    $.ajax({
        type: "POST",
        url: "SeleccionSolicitud.aspx/CargarTablaCotizaciones",
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {
            $('#TablaCotizacionesContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaCotizacionesCargando_RP').hide();
            $('#TablaCotizacionesContenedor_RP').show();
            $('#ModalSolicitud').dialog({ position: ['center', 'center'] });

            $("#TabCotizaciones_RP select").trigger("change");

            $('.numerico').autoNumeric('init');

            if ($('#ModSolModo').val() == 'C') {
                $('#ModSolNroSolicitud_RP').val('');
                $('#ModSolModo').val('N');
                $('#ModSolTipoCambioPanel').hide();
                for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                    Solicitud.Cotizaciones[i].Correlativo = "0";

                }
            }

            if ($('#HSeleccionada').val() == "S") {
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

                for (i = 0; i < $('#TabCotizaciones_RP tbody tr').length; i++) {
                    $('#TabCotizaciones_RP tbody tr:eq(' + i + ') input[type=radio]').attr("disabled", "true");
                }

                $('#ModSolVistaPrevia_RP').text('Imprimir documentos');
            }

            // Resaltar una fila si es que está marcada
            for (i = 0; i < $('#TabCotizaciones_RP tbody tr').length; i++) {
                const checkboxSeleccionado = $('#TabCotizaciones_RP tbody tr:eq(' + i + ') input[type=radio]:checked')
                if (checkboxSeleccionado.length > 0) {
                    const tr = checkboxSeleccionado.parent().parent();
                    tr.addClass("grilla_active");
                }
            }

            if ($('#HEstado').val() == "1") {//Seleccionada
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            } else if ($('#HEstado').val() == "2") {//Cerrado
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            else if ($('#HEstado').val() == "3") {//Anulado
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            else if ($('#HEstado').val() == "4") {//Evaluacion
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            else if ($('#HEstado').val() == "5") {//Observado
                botonModSolAceptarBloqueado = false;
                $('#ModSolAceptarCierre_RP').attr('class', 'boton darkblue sharp');
            }
            else if ($('#HEstado').val() == "6") {//Aprobado
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            else if ($('#HEstado').val() == "7") {//Rechazado
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            else {//Cotizado
                botonModSolAceptarBloqueado = false;
                $('#ModSolAceptarCierre_RP').attr('class', 'boton darkblue sharp');
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaCotizacionesCargando_RP').hide();
                $('#TablaCotizacionesError').show();
            }
        }
    });
}

function CargarTablaCotizacionesOficiales(cot, elegida, perTRA, perRadio) {

    $('#TablaCotizacionesOficialesContenedor').hide();
    $('#TablaCotizacionesOficialesCargando').show();
    var params = {
        cotizaciones: cot,
        numCotizacionElegida: elegida,
        permisoTRA: perTRA,
        permisoRadio: perRadio
    }
    $.ajax({
        type: 'POST',
        url: 'CotizadorOficiales.aspx/CargarTablaCotizacionesOficiales',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {

            $('#TablaCotizacionesOficialesContenedor').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaCotizacionesOficialesCargando').hide();
            $('#TablaCotizacionesOficialesContenedor').show();
            $('#ModalSolicitud').dialog({ position: ['center', 'center'] });

            $('.numerico').autoNumeric('init');
            if (elegida > 0) {
                var radio = $("#TabCotizaciones_RP input[type=radio]:checked");
                var filaPadre = radio.parent().parent();
                filaPadre.addClass("grilla_active");
            }
            //ActualizaEstiloCombobox('#TabCotizaciones_RP select');
            //alert(Solicitud.Cotizaciones[0].AjusteTRA);
            //alert(Solicitud);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaCotizacionesOficialesCargando').hide();
                $('#TablaCotizacionesOficialesError').show();
            }
        }
    });
}

function CargarTablaCotizacionesOficialesBandeja(cot, elegida, perTRA, perRadio) {

    $('#TablaCotizacionesContenedor_RP').hide();
    $('#TablaCotizacionesCargando_RP').show();
    var params = {
        cotizaciones: cot,
        numCotizacionElegida: elegida,
        permisoTRA: perTRA,
        permisoRadio: perRadio
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacion.aspx/CargarTablaCotizacionesOficiales',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {

            $('#TablaCotizacionesContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaCotizacionesCargando_RP').hide();
            $('#TablaCotizacionesContenedor_RP').show();
            $('#ModalSolicitud').dialog({ position: ['center', 'center'] });

            $('.numerico').autoNumeric('init');
            if (elegida > 0) {
                var radio = $("#TabCotizaciones_RP input[type=radio]:checked");
                var filaPadre = radio.parent().parent();
                filaPadre.addClass("grilla_active");
            }
            //ActualizaEstiloCombobox('#TabCotizaciones_RP select');
            //alert(Solicitud.Cotizaciones[0].AjusteTRA);
            //alert(Solicitud);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaCotizacionesCargando_RP').hide();
                $('#TablaCotizacionesError').show();
            }
        }
    });
}

function CargarTablaCotizacionesOficialesBandejaMovil(cot, elegida, perTRA, perRadio) {

    $('#TablaCotizacionesContenedor_RP').hide();
    $('#TablaCotizacionesCargando_RP').show();
    var params = {
        cotizaciones: cot,
        numCotizacionElegida: elegida,
        permisoTRA: perTRA,
        permisoRadio: perRadio
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacionModificar.aspx/CargarTablaCotizacionesOficiales',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {

            $('#TablaCotizacionesContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaCotizacionesCargando_RP').hide();
            $('#TablaCotizacionesContenedor_RP').show();
            $('#ModalSolicitud').dialog({ position: ['center', 'center'] });

            $('.numerico').autoNumeric('init');
            if (elegida > 0) {
                var radio = $("#TabCotizaciones_RP input[type=radio]:checked");
                var filaPadre = radio.parent().parent();
                filaPadre.addClass("grilla_active");
            }
            //ActualizaEstiloCombobox('#TabCotizaciones_RP select');
            //alert(Solicitud.Cotizaciones[0].AjusteTRA);
            //alert(Solicitud);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaCotizacionesCargando_RP').hide();
                $('#TablaCotizacionesError').show();
            }
        }
    });
}

function CargarTablaBeneficiarios_RP(ben) {
    if ($.trim($('#HCUSPP_RP').val()).length > 0) {
        if (ben == null) {
            $('#TablaBeneficiariosContenedor_RP').hide();
            $('#TablaBeneficiariosCargando_RP').show();

            $('#ManSolNumBeneficiarios_RP').hide();
            $('#ManSolNumBeneficiariosCargando_RP').show();
        }
        else {
            $('#TablaRviBenefiContenedor_RP').hide();
            $('#TablaRviBenefiCargando_RP').show();

            // Convertir las Fechas a Date de Javascript para que sean correctamente parseados por el servidor
            for (var i = 0; i < ben.length; i++) {
                if (ben[i].FechaNacimiento != null)
                    ben[i].FechaNacimiento = new Date(+ben[i].FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (ben[i].FechaInvalidez != null)
                    ben[i].FechaInvalidez = new Date(+ben[i].FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1"))
            }
            $("#HConyuge").val("FALSE");
            if (ben.length == 2) {
                if (ben[1].Parentesco.Id == "10") {
                    $("#HConyuge").val("TRUE");
                } else {
                    $("#HConyuge").val("FALSE");
                }
            }
        }

        var params = {
            cuspp: $('#HCUSPP_RP').val(),
            beneficiarios: ben,
            conyuge: $("#HConyuge").val()
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaBeneficiarios',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (ben == null) {
                    $('#TablaBeneficiariosCargando_RP').hide();
                    $('#TablaBeneficiariosContenedor_RP').show();
                    $('#ManSolNumBeneficiariosCargando_RP').hide();
                    $('#TablaBeneficiariosContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
                    $('#ManSolNumBeneficiarios_RP').show();
                    //<INIGTI_7012>
                    if ($('#ModSolModo').val() == "CERRAR") {
                        //$('#ManSolNumBeneficiarios_RP').html('(' + $('#TabRviBenefi_RP tbody tr').length + ')');
                    }
                    else {
                        //<INIGTI_7012>
                        if ($('#ModSolModo').val() == "N") {
                            if ($('#TabBeneficiarios_RP input[type=checkbox]:checked').length == 1) {
                                $('#ModSolTipoPlan_RP').val("02");
                                $('#TexModSolTipoPlan_RP').html($('#ModSolTipoPlan_RP').find(':selected').text());
                                $('#ModSolTipoPlan_RP').attr('disabled', 'disabled');
                                $('#ConModSolTipoPlan_RP').addClass('formComboboxReadOnlyContenedor');
                            }
                        }
                        //<FINGTI_7012>
                        $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');
                    }
                    //<FINGTI_7012>

                    //<INIGTI_753>
                    if ($('#ModSolTipoPlan_RP').val() == "02") {
                        for (i = 1; i < $('#TabBeneficiarios_RP tbody tr').length; i++) {
                            $('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').prop("checked", "")
                            $('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').attr("disabled", "true")
                        }
                        //<INIGTI_7012>
                        if ($('#ModSolModo').val() == "CERRAR") {
                            $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabRviBenefi_RP tbody tr').length + ')');
                        }
                        else {
                            $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');
                        }
                        //<FINGTI_7012>

                    }

                    //<INIGTI_753_3>
                    //if ($('#ModSolModo').val() == "N") {
                    if ($('#ModSolModo').val() == "N" && $('#HCopia').val() == "") {
                        //<FINGTI_753_3>

                        var valida = false;
                        for (i = 0; i < $("#TabBeneficiarios_RP tbody tr").length; i++) {
                            if ($("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").is(":checked")) {
                                var idParentesco = $("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").data("parentesco");
                                if (idParentesco != "80") {
                                    if (idParentesco == "10") {
                                        valida = true;
                                    } else {
                                        valida = false;
                                    }

                                    if (valida == false) {
                                        break;
                                    }
                                }
                            }
                        }

                        if (valida == true) {
                            $("#HConyuge").val("TRUE");
                        } else {
                            $("#HConyuge").val("FALSE");
                        }
                        //<INIGTI_7012>
                        if ($('#ModSolModo').val() == "CERRAR") {
                            CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());
                        } else {
                            CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
                        }
                        //<FINGTI_7012>
                    }
                    //<FINGTI_753>

                }
                else {
                    $('#TablaRviBenefiCargando_RP').hide();
                    $('#TablaRviBenefiContenedor_RP').show();
                    $('#TablaRviBenefiContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
                    //<INIGTI_7012>
                    if ($('#ModSolModo').val() == "CERRAR") {
                        $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabRviBenefi_RP tbody tr').length + ')');
                    }
                    //<FINGTI_7012>
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    if (ben == null) {
                        $('#TablaBeneficiariosCargando_RP').hide();
                        $('#TablaBeneficiariosError_RP').show();
                    }
                    else {
                        $('#TablaRviBenefiCargando_RP').hide();
                        $('#TablaRviBenefiError_RP').show();
                    }
                }
            }
        });
    }
}

//<SRI.FIN-20322_E2>
function CargarTablaBeneficiariosOficial(ben) {
    if (ben == null) {
        $('#TablaBeneficiariosContenedor_RP').hide();
        $('#TablaBeneficiariosCargando_RP').show();

        $('#ManSolNumBeneficiarios_RP').hide();
        $('#ManSolNumBeneficiariosCargando_RP').show();
    }
    else {
        $('#TablaRviBenefiContenedor_RP').hide();
        $('#TablaRviBenefiCargando_RP').show();

        // Convertir las Fechas a Date de Javascript para que sean correctamente parseados por el servidor
        for (var i = 0; i < ben.length; i++) {
            if (ben[i].FechaNacimiento != null)
                ben[i].FechaNacimiento = new Date(+ben[i].FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
            if (ben[i].FechaInvalidez != null)
                ben[i].FechaInvalidez = new Date(+ben[i].FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1"))
        }
    }

    var params = {
        beneficiarios: ben
    }
    $.ajax({
        type: 'POST',
        url: 'CotizadorOficiales.aspx/CargarTablaBeneficiarios',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (ben == null) {
                $('#TablaBeneficiariosCargando_RP').hide();
                $('#TablaBeneficiariosContenedor_RP').show();
                $('#ManSolNumBeneficiariosCargando_RP').hide();
                $('#TablaBeneficiariosContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
                $('#ManSolNumBeneficiarios_RP').show();
                $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');
            }
            else {
                $('#TablaRviBenefiCargando_RP').hide();
                $('#TablaRviBenefiContenedor_RP').show();
                $('#TablaRviBenefiContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                if (ben == null) {
                    $('#TablaBeneficiariosCargando_RP').hide();
                    $('#TablaBeneficiariosError_RP').show();
                }
                else {
                    $('#TablaRviBenefiCargando_RP').hide();
                    $('#TablaRviBenefiError_RP').show();
                }
            }
        }
    });
}

function CargarTablaBeneficiariosOficialBandeja(ben, cussp) {
    if (ben == null) {
        $('#TablaBeneficiariosContenedor_RP').hide();
        $('#TablaBeneficiariosCargando_RP').show();

        $('#ManSolNumBeneficiarios_RP').hide();
        $('#ManSolNumBeneficiariosCargando_RP').show();
    }
    else {
        $('#TablaRviBenefiContenedor_RP').hide();
        $('#TablaRviBenefiCargando_RP').show();

        // Convertir las Fechas a Date de Javascript para que sean correctamente parseados por el servidor
        for (var i = 0; i < ben.length; i++) {
            if (ben[i].FechaNacimiento != null)
                ben[i].FechaNacimiento = new Date(+ben[i].FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
            if (ben[i].FechaInvalidez != null)
                ben[i].FechaInvalidez = new Date(+ben[i].FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1"))
        }
    }

    var params = {
        cuspp: cussp,
        beneficiarios: ben
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacion.aspx/CargarTablaBeneficiarios',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (ben == null) {
                $('#TablaBeneficiariosCargando_RP').hide();
                $('#TablaBeneficiariosContenedor_RP').show();
                $('#ManSolNumBeneficiariosCargando_RP').hide();
                $('#TablaBeneficiariosContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
                $('#ManSolNumBeneficiarios_RP').show();
                $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');
            }
            else {
                $('#TablaRviBenefiCargando_RP').hide();
                $('#TablaRviBenefiContenedor_RP').show();
                $('#TablaRviBenefiContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                if (ben == null) {
                    $('#TablaBeneficiariosCargando_RP').hide();
                    $('#TablaBeneficiariosError_RP').show();
                }
                else {
                    $('#TablaRviBenefiCargando_RP').hide();
                    $('#TablaRviBenefiError_RP').show();
                }
            }
        }
    });
}

function CargarTablaBeneficiariosOficialBandejaMovil(ben, cussp) {

    if (ben == null) {
        $('#TablaBeneficiariosContenedor_RP').hide();
        $('#TablaBeneficiariosCargando_RP').show();

        $('#ManSolNumBeneficiarios_RP').hide();
        $('#ManSolNumBeneficiariosCargando_RP').show();
    }
    else {
        $('#TablaRviBenefiContenedor_RP').hide();
        $('#TablaRviBenefiCargando_RP').show();

        // Convertir las Fechas a Date de Javascript para que sean correctamente parseados por el servidor
        for (var i = 0; i < ben.length; i++) {
            if (ben[i].FechaNacimiento != null)
                ben[i].FechaNacimiento = new Date(+ben[i].FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
            if (ben[i].FechaInvalidez != null)
                ben[i].FechaInvalidez = new Date(+ben[i].FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1"))
        }
    }

    var params = {
        cuspp: cussp,
        beneficiarios: ben
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacionModificar.aspx/CargarTablaBeneficiarios',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (ben == null) {
                $('#TablaBeneficiariosCargando_RP').hide();
                $('#TablaBeneficiariosContenedor_RP').show();
                $('#ManSolNumBeneficiariosCargando_RP').hide();
                $('#TablaBeneficiariosContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
                $('#ManSolNumBeneficiarios_RP').show();
                $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');
            }
            else {
                $('#TablaRviBenefiCargando_RP').hide();
                $('#TablaRviBenefiContenedor_RP').show();
                $('#TablaRviBenefiContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                if (ben == null) {
                    $('#TablaBeneficiariosCargando_RP').hide();
                    $('#TablaBeneficiariosError_RP').show();
                }
                else {
                    $('#TablaRviBenefiCargando_RP').hide();
                    $('#TablaRviBenefiError_RP').show();
                }
            }
        }
    });
}

function CargarTablaCotizacionMovimiento(numSolicitud) {

    var params = {
        numSolicitud: numSolicitud
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacion.aspx/CargarTablaCotizacionesMovimiento',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {

            $('#ModFlujoSolCargando').hide();
            $('#TablaFlujoCotizacionesContenedor').show();
            $('#TablaFlujoCotizacionesContenedor').html($(data.d).find('#ContenidoDinamico').html());

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#ModalFlujoSolicitud').dialog('close');
                $('#ModFlujoSolCargando').hide();
                $('#TablaFlujoSolError').show();
            }
        }
    });
}

function CargarTablaSeguimiento(idJefe, idSupervisor, idAgente, cuspp, fechaInicio, fechaTermino) {
    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        idJefe: idJefe,
        idSupervisor: idSupervisor,
        idAgente: idAgente,
        cuspp: cuspp,
        fechaInicio: fechaInicio,
        fechaTermino: fechaTermino,
        indicePagina: $('#TabSeguimientoIndicePagina').val(),
        tamanhoPagina: $('#TabSeguimientoTamanhoPagina').val(),
        columnaOrdenar: $('#TabSeguimientoColumnaOrdenar').val(),
        direccionOrdenar: $('#TabSeguimientoDireccionOrdenar').val()
    }

    botonesBloqueados = true;
    $('#Buscar').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#TablaSeguimiento').hide();
    $('#ContenedorExportar').hide();
    $('#Cargando').show();

    ajaxTablaAfiliados = $.ajax({
        type: 'POST',
        url: 'Seguimiento.aspx/CargarTablaSeguimiento',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                botonesBloqueados = false;
                $('#Buscar').attr('class', 'boton darkblue sharp');
                $('#Cargando').hide();
                $('#TablaSeguimiento').html($(data.d.Contenido).find('#ContenidoDinamico').html());
                $('#TablaSeguimiento').show();
                if ($('#TabSeguimiento tbody tr').length > 0) {
                    if (!($('#TabSeguimiento tbody tr').length == 1 && $('#TabSeguimiento td').find('div').find('div').attr('class') == 'grilla_info')) {
                        $('#ContenedorExportar').show();
                    }
                    else {
                        $('#ContenedorExportar').hide();
                    }
                }
                else {
                    $('#ContenedorExportar').hide();
                }
            }
            else if (data.d.Estado == "ERROR") {
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                if (data.d.Controles != null) {
                    if (data.d.Controles[0].length) $('#ConJefe').attr('class', data.d.Controles[0]);
                    if (data.d.Controles[1].length) $('#ConSupervisor').attr('class', data.d.Controles[1]);
                    if (data.d.Controles[2].length) $('#ConAgente').attr('class', data.d.Controles[2]);
                    if (data.d.Controles[3].length) $('#CUSPP_RP').attr('class', data.d.Controles[3]);
                    if (data.d.Controles[4].length) $('#FechaInicio').attr('class', data.d.Controles[4]);
                    if (data.d.Controles[5].length) $('#FechaTermino').attr('class', data.d.Controles[5]);
                }
                $('#ModalCuadroMensaje').dialog('open');

                botonesBloqueados = false;
                $('#Buscar').attr('class', 'boton darkblue sharp');
                $('#Cargando').hide();
            }
            else if (data.d.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión expirada, recargar para redirigir a Iniciar Sesión */
                document.location.reload(true);
            }
            else {
                botonesBloqueados = false;
                $('#Buscar').attr('class', 'boton darkblue sharp');
                $('#Cargando').hide();
                $('#TablaSeguimientoError').show();
            }
        }
    });
}

function CargarTablaSeguimiento2(idJefe, idSupervisor, idAgente, cuspp, fechaInicio, fechaTermino) {
    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        idJefe: idJefe,
        idSupervisor: idSupervisor,
        idAgente: idAgente,
        cuspp: cuspp,
        fechaInicio: fechaInicio,
        fechaTermino: fechaTermino,
        indicePagina: $('#TabSeguimientoIndicePagina').val(),
        tamanhoPagina: $('#TabSeguimientoTamanhoPagina').val(),
        columnaOrdenar: $('#TabSeguimientoColumnaOrdenar').val(),
        direccionOrdenar: $('#TabSeguimientoDireccionOrdenar').val()
    }

    botonesBloqueados = true;
    $('#Buscar').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#ExportarExcel').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#TabSeguimientoPaginadorCargando').show();

    ajaxTablaAfiliados = $.ajax({
        type: 'POST',
        url: 'Seguimiento.aspx/CargarTablaSeguimiento',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                botonesBloqueados = false;
                $('#Buscar').attr('class', 'boton darkblue sharp');
                $('#ExportarExcel').attr('class', 'boton darkblue sharp');
                $('#TabSeguimientoPaginadorCargando').hide();
                $('#TablaSeguimiento').html($(data.d.Contenido).find('#ContenidoDinamico').html());
                if ($('#TabSeguimiento tbody tr').length > 0) {
                    if (!($('#TabSeguimiento tbody tr').length == 1 && $('#TabSeguimiento td').find('div').find('div').attr('class') == 'grilla_info')) {
                        $('#ContenedorExportar').show();
                    }
                    else {
                        $('#ContenedorExportar').hide();
                    }
                }
                else {
                    $('#ContenedorExportar').hide();
                }
            }
            else if (data.d.Estado == "ERROR") {
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                if (data.d.Controles != null) {
                    if (data.d.Controles[0].length) $('#ConJefe').attr('class', data.d.Controles[0]);
                    if (data.d.Controles[1].length) $('#ConSupervisor').attr('class', data.d.Controles[1]);
                    if (data.d.Controles[2].length) $('#ConAgente').attr('class', data.d.Controles[2]);
                    if (data.d.Controles[3].length) $('#CUSPP_RP').attr('class', data.d.Controles[3]);
                    if (data.d.Controles[4].length) $('#FechaInicio').attr('class', data.d.Controles[4]);
                    if (data.d.Controles[5].length) $('#FechaTermino').attr('class', data.d.Controles[5]);
                }
                $('#ModalCuadroMensaje').dialog('open');

                botonesBloqueados = false;
                $('#Buscar').attr('class', 'boton darkblue sharp');
                $('#ExportarExcel').attr('class', 'boton darkblue sharp');
                $('#TabSeguimientoPaginadorCargando').hide();
            }
            else if (data.d.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión expirada, recargar para redirigir a Iniciar Sesión */
                document.location.reload(true);
            }
            else {
                botonesBloqueados = false;
                $('#Buscar').attr('class', 'boton darkblue sharp');
                $('#ExportarExcel').attr('class', 'boton darkblue sharp');

                $('#TablaSeguimiento').hide();
                $('#TabSeguimientoPaginadorCargando').hide();
                $('#TablaSeguimientoError').show();
            }
        }
    });
}

function CargarTablaSupervision(idJefe, idSupervisor, idAgente, fechaInicio, fechaTermino) {
    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        idJefe: idJefe,
        idSupervisor: idSupervisor,
        idAgente: idAgente,
        fechaInicio: fechaInicio,
        fechaTermino: fechaTermino,
        indicePagina: $('#TabSupervisionIndicePagina').val(),
        tamanhoPagina: $('#TabSupervisionTamanhoPagina').val(),
        columnaOrdenar: $('#TabSupervisionColumnaOrdenar').val(),
        direccionOrdenar: $('#TabSupervisionDireccionOrdenar').val()
    }

    botonesBloqueados = true;
    $('#SupBuscar').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#TablaSupervision').hide();
    $('#ContenedorExportar').hide();
    $('#Cargando').show();

    ajaxTablaAfiliados = $.ajax({
        type: 'POST',
        url: 'Supervision.aspx/CargarTablaSupervision',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                botonesBloqueados = false;
                $('#SupBuscar').attr('class', 'boton darkblue sharp');
                $('#Cargando').hide();
                $('#TablaSupervision').html($(data.d.Contenido).find('#ContenidoDinamico').html());
                $('#TablaSupervision').show();
                if ($('#TabSupervision tbody tr').length > 0) {
                    if (!($('#TabSupervision tbody tr').length == 1 && $('#TabSupervision td').find('div').find('div').attr('class') == 'grilla_info')) {
                        $('#ContenedorExportar').show();
                    }
                    else {
                        $('#ContenedorExportar').hide();
                    }
                }
                else {
                    $('#ContenedorExportar').hide();
                }
            }
            else if (data.d.Estado == "ERROR") {
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                if (data.d.Controles != null) {
                    if (data.d.Controles[0].length) $('#ConJefe').attr('class', data.d.Controles[0]);
                    if (data.d.Controles[1].length) $('#ConSupervisor').attr('class', data.d.Controles[1]);
                    if (data.d.Controles[2].length) $('#ConAgente').attr('class', data.d.Controles[2]);
                    if (data.d.Controles[3].length) $('#FechaInicio').attr('class', data.d.Controles[3]);
                    if (data.d.Controles[4].length) $('#FechaTermino').attr('class', data.d.Controles[4]);
                }
                $('#ModalCuadroMensaje').dialog('open');

                botonesBloqueados = false;
                $('#SupBuscar').attr('class', 'boton darkblue sharp');
                $('#Cargando').hide();
            }
            else if (data.d.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión expirada, recargar para redirigir a Iniciar Sesión */
                document.location.reload(true);
            }
            else {
                botonesBloqueados = false;
                $('#SupBuscar').attr('class', 'boton darkblue sharp');
                $('#Cargando').hide();
                $('#TablaSupervisionError').show();
            }
        }
    });
}

function CargarTablaSupervision2(idJefe, idSupervisor, idAgente, fechaInicio, fechaTermino) {
    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        idJefe: idJefe,
        idSupervisor: idSupervisor,
        idAgente: idAgente,
        fechaInicio: fechaInicio,
        fechaTermino: fechaTermino,
        indicePagina: $('#TabSupervisionIndicePagina').val(),
        tamanhoPagina: $('#TabSupervisionTamanhoPagina').val(),
        columnaOrdenar: $('#TabSupervisionColumnaOrdenar').val(),
        direccionOrdenar: $('#TabSupervisionDireccionOrdenar').val()
    }

    botonesBloqueados = true;
    $('#SupBuscar').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#ExportarExcel').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#TabSupervisionPaginadorCargando').show();

    ajaxTablaAfiliados = $.ajax({
        type: 'POST',
        url: 'Supervision.aspx/CargarTablaSupervision',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                botonesBloqueados = false;
                $('#SupBuscar').attr('class', 'boton darkblue sharp');
                $('#ExportarExcel').attr('class', 'boton darkblue sharp');
                $('#TabSupervisionPaginadorCargando').hide();
                $('#TablaSupervision').html($(data.d.Contenido).find('#ContenidoDinamico').html());
                if ($('#TabSupervision tbody tr').length > 0) {
                    if (!($('#TabSupervision tbody tr').length == 1 && $('#TabSupervision td').find('div').find('div').attr('class') == 'grilla_info')) {
                        $('#ContenedorExportar').show();
                    }
                    else {
                        $('#ContenedorExportar').hide();
                    }
                }
                else {
                    $('#ContenedorExportar').hide();
                }
            }
            else if (data.d.Estado == "ERROR") {
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                if (data.d.Controles != null) {
                    if (data.d.Controles[0].length) $('#ConJefe').attr('class', data.d.Controles[0]);
                    if (data.d.Controles[1].length) $('#ConSupervisor').attr('class', data.d.Controles[1]);
                    if (data.d.Controles[2].length) $('#ConAgente').attr('class', data.d.Controles[2]);
                    if (data.d.Controles[3].length) $('#FechaInicio').attr('class', data.d.Controles[3]);
                    if (data.d.Controles[4].length) $('#FechaTermino').attr('class', data.d.Controles[4]);
                }
                $('#ModalCuadroMensaje').dialog('open');

                botonesBloqueados = false;
                $('#SupBuscar').attr('class', 'boton darkblue sharp');
                $('#ExportarExcel').attr('class', 'boton darkblue sharp');
                $('#TabSupervisionPaginadorCargando').hide();
            }
            else if (data.d.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión expirada, recargar para redirigir a Iniciar Sesión */
                document.location.reload(true);
            }
            else {
                botonesBloqueados = false;
                $('#SupBuscar').attr('class', 'boton darkblue sharp');
                $('#ExportarExcel').attr('class', 'boton darkblue sharp');

                $('#TablaSupervision').hide();
                $('#TabSupervisionPaginadorCargando').hide();
                $('#TablaSupervisionError').show();
            }
        }
    });
}

function CargarTablaSolicitudesSimulador() {
    if ($.trim($('#CUSPP_RP').val()).length > 0) {
        // Limpiar tabla de cotizaciones si estuviera activa
        $('#TablaCotizacionesError').hide();

        $('#TablaCotizacionesContenedor_RP').hide();
        $('#TablaCotizacionesNoSolicitud').show();
        $('#Simulaciones').hide();

        $('#TablaSolicitudesContenedor_RP').hide();
        $('#TablaSolicitudesCargando_RP').show();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#CUSPP_RP').val()
        }
        $.ajax({
            type: 'POST',
            url: rutaCargarTablaSolicitudesSimulador,
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaSolicitudesCargando_RP').hide();
                    $('#TablaSolicitudesContenedor_RP').show();
                    $('#TablaSolicitudesContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
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
                    $('#TablaSolicitudesCargando_RP').hide();
                    $('#TablaSolicitudesError_RP').show();
                }
            },
            complete: function () {
                botonBusAfiBuscarBloqueado = false;
                $('#SimBusAfiBuscar').attr('class', 'boton darkblue sharp');
            }
        });
    }
}

function CargarTablaCotizacionesSimulador(cot, idSimulador, filtro, movil, modalidad, moneda, periodoGarantizado) {
    if (filtro == 0) {
        $('#TablaCotizacionesError').hide();
        $('#TablaCotizacionesContenedor_RP').hide();
        $('#TablaCotizacionesNoSolicitud').hide();
        $('#TablaCotizacionesCargando_RP').show();
    }
    else {
        $('#TablaCotizaciones' + filtro + 'Error').hide();
        $('#TablaCotizaciones' + filtro + 'Contenedor').hide();
        $('#TablaCotizaciones' + filtro + 'NoSolicitud').hide();
        $('#TablaCotizaciones' + filtro + 'Cargando').show();
    }
    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        cotizaciones: cot,
        idSimulador: idSimulador,
        filtro: filtro,
        movil: movil,
        modalidad: modalidad,
        moneda: moneda,
        periodoGarantizado: periodoGarantizado
    }

    $.ajax({
        type: 'POST',
        url: rutaCargarTablaCotizacionesSimulador,
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d != 'TOKEN') {
                if (filtro == 0) {
                    $('#TablaCotizacionesContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
                    $('#TablaCotizacionesCargando_RP').hide();
                    $('#TablaCotizacionesContenedor_RP').show();

                    $('#Simulacion1,#Simulacion2,#Simulacion').html('');
                    $('#Simulaciones').show();
                }
                else {
                    $('#TablaCotizaciones' + filtro + 'Contenedor').html($(data.d).find('#ContenidoDinamico').html());
                    $('#TablaCotizaciones' + filtro + 'Cargando').hide();
                    $('#TablaCotizaciones' + filtro + 'Contenedor').show();
                    //alert('#TablaCotizaciones' + filtro + 'Contenedor');
                    //alert($('#TablaCotizaciones' + filtro + 'Contenedor').html($(data.d).find('#ContenidoDinamico').html()));

                    $('#Simulacion').html('');
                    $('#Simulaciones').show();
                }

                /* Elementos Drag & Drop */
                $('.grilla_graficar').draggable({
                    cursor: 'move',
                    revert: true
                });
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
                $('#TablaCotizacionesCargando_RP').hide();
                $('#TablaCotizacionesError').show();
            }
        }
    });
}

function CargarTablaPersonasVinculadas_RP(numSolicitud) {
    $("#TablaPersonasVinculadasPEPContenedor_RP").hide();
    $("#TablaPersonasVinculadasPEP_RP").show();

    var params = {
        numSolicitud: numSolicitud
    }
    $.ajax({
        type: "POST",
        url: "GrupoFamiliarAfiliadoCierre.aspx/CargarTablaPersonasVinculadasPEP",
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {
            $('#TablaPersonasVinculadasPEPContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaPersonasVinculadasPEP_RP').hide();
            $('#TablaPersonasVinculadasPEPContenedor_RP').show();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaPersonasVinculadasPEPContenedor_RP').hide();
                //$('#TablaCotizacionesError').show();
            }
        }
    });
}

/**
* jQuery.browser.mobile (http://detectmobilebrowser.com/)
*
* jQuery.browser.mobile will be true if the browser is a mobile device
*
**/
(function (a) { jQuery.browser.mobile = /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4)) })(navigator.userAgent || navigator.vendor || window.opera);

