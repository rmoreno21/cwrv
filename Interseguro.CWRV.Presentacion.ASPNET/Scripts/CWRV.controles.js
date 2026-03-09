$(document).ready(function () {

    /* Poner estilo a los Combobox */
    ActualizaEstiloCombobox('select');

    $('select').live('change', function () {
        $(this).parent().find('#Tex' + $(this).attr('id')).html($(this).find(':selected').text());
    });

    /* Cambiar el estilo del control que tiene el foco */
    $(".formLinea").delegate("*", "focus blur", function (event) {
        var controlSeleccionado = $(this);
        if (!controlSeleccionado.is('div')) {

            var idControlSeleccionado = controlSeleccionado.attr('id');
            var labelSeleccionado = $("#Lab" + idControlSeleccionado);
            var contComboSeleccionado = $('#Con' + idControlSeleccionado);
            //alert(labelSeleccionado.Class);
            setTimeout(function () {
                controlSeleccionado.toggleClass("formFocus", controlSeleccionado.is(":focus"));
                contComboSeleccionado.toggleClass("formFocus", controlSeleccionado.is(":focus"));
                labelSeleccionado.toggleClass("formLabelFocus", controlSeleccionado.is(":focus"));
            }, 0);
        }
    });


    //<SRI.INI-20322_E2>
    //CargarTablaSolicitudesOficiales(0, 0, 0);
    //CargarTablaBandejaSolicitudesOficiales(0, 0, 0);
    //<SRI.FIN-20322_E2>
    //CargarTablaActividades();


    $('#TabDireccionesVerMas').live('click', function () {
        if ($('#TabDireccionesDesplegar').val() == '0') {
            $('#TabDireccionesDesplegar').val('1');
            $('#TabDirecciones tbody').find('tr:gt(0)').show();
            $('#TabDireccionesVerMas').html('Ver más reciente');
        }
        else {
            $('#TabDireccionesDesplegar').val('0');
            $('#TabDirecciones tbody').find('tr:gt(0)').hide();
            $('#TabDireccionesVerMas').html('Ver más direcciones');
        }
    });

    $('#TablaAfiliadosReintentar').live('click', function () {
        $('#TablaAfiliadosError').hide();
        CargarTablaAfiliados();
    });

    $('#TablaDireccionesReintentar').live('click', function () {
        $('#TablaDireccionesError').hide();
        CargarTablaDirecciones();
    });

    $('#TabTelefonosVerMas').live('click', function () {
        if ($('#TabTelefonosDesplegar').val() == '0') {
            $('#TabTelefonosDesplegar').val('1');
            $('#TabTelefonos tbody').find('tr:gt(0)').show();
            $('#TabTelefonosVerMas').html('Ver más reciente');
        }
        else {
            $('#TabTelefonosDesplegar').val('0');
            $('#TabTelefonos tbody').find('tr:gt(0)').hide();
            $('#TabTelefonosVerMas').html('Ver más teléfonos');
        }
    });

    $('#TablaTelefonosReintentar').live('click', function () {
        $('#TablaTelefonosError').hide();
        CargarTablaTelefonos();
    });

    $('#TablaGrupoFamiliarReintentar').live('click', function () {
        $('#TablaGrupoFamiliarError').hide();
        CargarTablaGrupoFamiliar();
    });

    $('#TablaSolicitudesReintentar').live('click', function () {
        $('#TablaSolicitudesError').hide();
        CargarTablaSolicitudes();
    });

    $('#TablaSolicitudesSimuladorReintentar').live('click', function () {
        $('#TablaSolicitudesError').hide();
        CargarTablaSolicitudesSimulador();
    });

    $('#TablaBeneficiariosReintentar').live('click', function () {
        $('#TablaBeneficiariosError').hide();
        CargarTablaBeneficiarios(null);
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

        CargarTablaSeguimiento($('#HJefe').val(), $('#HSupervisor').val(), $('#HAgente').val(), $('#HCUSPP').val(), $('#HFechaInicio').val(), $('#HFechaTermino').val());
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

        CargarTablaSolicitudesOficiales(SegIdJefe, SegIdSupervisor, SegIdAgente, true, null);
    });

    /*Agrupadores*/
    //<INIGTI_4081>
    $('#AgrupadorBeneficiario').click(function () {
        $('#DatosBeneficiario').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosBeneficiario').is(':visible'))
                $('#AgrupadorBeneficiario span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorBeneficiario span').attr('class', 'agrupador_titulo_mas');
        });
    });

    $('#AgrupadorInformeA').click(function () {
        $('#DatosInformeA').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosInformeA').is(':visible'))
                $('#AgrupadorInformeA span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorInformeA span').attr('class', 'agrupador_titulo_mas');
        });
    });

    $('#AgrupadorCotizacion').click(function () {
        $('#DatosCotizacion').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosCotizacion').is(':visible'))
                $('#AgrupadorCotizacion span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorCotizacion span').attr('class', 'agrupador_titulo_mas');
        });
    });

    $('#AgrupadorEspeciales').click(function () {
        $('#DatosEspeciales').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosEspeciales').is(':visible')) {
                $('#AgrupadorEspeciales span').attr('class', 'agrupador_titulo_menos')
                // Deslizar la pantalla hacia los datos de simulación
                $('html,body').animate({
                    scrollTop: $('#AgrupadorEspeciales').offset().top
                }, 'slow');
            }
            else {
                $('#AgrupadorEspeciales span').attr('class', 'agrupador_titulo_mas');

                // Deslizar la pantalla hacia los datos de simulación
                $('html,body').animate({
                    scrollTop: $('#AgrupadorCotizacion').offset().top
                }, 'slow');

            }

        });
    });

    //<FINGTI_4081>

    $('#AgrupadorDireccion').click(function () {
        $('#DatosDireccion').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosDireccion').is(':visible'))
                $('#AgrupadorDireccion span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorDireccion span').attr('class', 'agrupador_titulo_mas');
        });
    });

    $('#AgrupadorTelefono').click(function () {
        $('#DatosTelefono').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosTelefono').is(':visible'))
                $('#AgrupadorTelefono span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorTelefono span').attr('class', 'agrupador_titulo_mas');
        });
    });

    $('#AgrupadorEmpresa').click(function () {
        $('#DatosEmpresa').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosEmpresa').is(':visible'))
                $('#AgrupadorEmpresa span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorEmpresa span').attr('class', 'agrupador_titulo_mas');
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
    /*<SRI.INI-20322>*/
    $('#ModTasaVenta').autoNumeric();
    $('#ModTasaVentaSbs').autoNumeric();
    /*<SRI.FIN-20322>*/

    $(".enteroPositivo").numeric();
    $(".telefono").numeric({ allow: "#*+" });
    $(".alfanumerico").alphanumeric({ allow: "ñÑ" });
    $(".nombre").alpha({ allow: "áéíóúÁÉÍÓÚñÑäëïöüÄËÏÖÜàèìòùÀÈÌÒÙ'- " });
    $(".fecha").mask("99/99/9999");
});

function CargarTablaAfiliados(apellidoPaterno, apellidoMaterno, nombres) {
    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        apellidoPaterno: apellidoPaterno,
        apellidoMaterno: apellidoMaterno,
        nombres: nombres,
        indicePagina: $('#TabAfiliadosIndicePagina').val(),
        tamanhoPagina: $('#TabAfiliadosTamanhoPagina').val(),
        columnaOrdenar: $('#TabAfiliadosColumnaOrdenar').val(),
        direccionOrdenar: $('#TabAfiliadosDireccionOrdenar').val()
    }

    botonesBloqueados = true;
    $('#ModBusAfiBuscar').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#ModBusAfiTablaAfiliados').hide();
    $('#ModBusAfiCargando').show();
    $('#ModalBusquedaAfiliados').dialog({ position: ['center', 'center'] });

    ajaxTablaAfiliados = $.ajax({
        type: 'POST',
        url: rutaCargarTablaAfiliados,
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                botonesBloqueados = false;
                $('#ModBusAfiBuscar').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiCargando').hide();
                $('#ModBusAfiTablaAfiliados').html($(data.d.Contenido).find('#ContenidoDinamico').html());
                $('#ModBusAfiTablaAfiliados').show();
                $('#ModalBusquedaAfiliados').dialog({ position: ['center', 'center'] });
            }
            else if (data.d.Estado == "ERROR") {
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                if (data.d.Controles != null) {
                    if (data.d.Controles[0].length) $('#ModBusAfiApellidoPaterno').attr('class', data.d.Controles[0]);
                    if (data.d.Controles[1].length) $('#ModBusAfiApellidoMaterno').attr('class', data.d.Controles[1]);
                    if (data.d.Controles[2].length) $('#ModBusAfiNombres').attr('class', data.d.Controles[2]);
                }
                $('#ModalCuadroMensaje').dialog('open');

                botonesBloqueados = false;
                $('#ModBusAfiBuscar').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiCargando').hide();
                $('#ModalBusquedaAfiliados').dialog({ position: ['center', 'center'] });
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
                $('#ModBusAfiBuscar').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiCargando').hide();
                $('#TablaAfiliadosError').show();
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
        indicePagina: $('#TabAfiliadosIndicePagina').val(),
        tamanhoPagina: $('#TabAfiliadosTamanhoPagina').val(),
        columnaOrdenar: $('#TabAfiliadosColumnaOrdenar').val(),
        direccionOrdenar: $('#TabAfiliadosDireccionOrdenar').val()
    }

    botonesBloqueados = true;
    $('#ModBusAfiBuscar').attr('class', 'botonDeshabilitado gris gris_sharp');
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
                $('#ModBusAfiBuscar').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiAceptar').attr('class', 'boton darkblue sharp');
                $('#TabAfiliadosPaginadorCargando').hide();
                $('#ModBusAfiTablaAfiliados').html($(data.d.Contenido).find('#ContenidoDinamico').html());
            }
            else if (data.d.Estado == "ERROR") {
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                if (data.d.Controles != null) {
                    if (data.d.Controles[0].length) $('#ModBusAfiApellidoPaterno').attr('class', data.d.Controles[0]);
                    if (data.d.Controles[1].length) $('#ModBusAfiApellidoMaterno').attr('class', data.d.Controles[1]);
                    if (data.d.Controles[2].length) $('#ModBusAfiNombres').attr('class', data.d.Controles[2]);
                }
                $('#ModalCuadroMensaje').dialog('open');

                botonesBloqueados = false;
                $('#ModBusAfiBuscar').attr('class', 'boton darkblue sharp');
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
                $('#ModBusAfiBuscar').attr('class', 'boton darkblue sharp');
                $('#ModBusAfiAceptar').attr('class', 'boton darkblue sharp');

                $('#ModBusAfiTablaAfiliados').hide();
                $('#TabAfiliadosPaginadorCargando').hide();
                $('#TablaAfiliadosError').show();
            }
        }
    });
}

function CargarTablaDirecciones() {
    if ($.trim($('#CUSPP').val()).length > 0) {
        $('#TablaDireccionesContenedor').hide();
        $('#TablaDireccionesCargando').show();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#CUSPP').val()
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaDirecciones',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaDireccionesCargando').hide();
                    $('#TablaDireccionesContenedor').show();
                    $('#TablaDireccionesContenedor').html($(data.d).find('#ContenidoDinamico').html());

                    if ($('#TabDireccionesDesplegar').val() == '0') {
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
                    $('#TablaDireccionesCargando').hide();
                    $('#TablaDireccionesError').show();
                }
            }
        });
    }
}

function CargarTablaGestionVentas() {
    $('#TablaGestionVentasContenedor').hide();
    $('#TablaGestionVentasCargando').show();

    var supervisor = "0";
    if ($("#Supervisor").val() != undefined) {
        supervisor = $("#Supervisor").val();
    }
    //<INIGTI_4081_3>
    var jefe = "0";
    if ($("#Jefe").val() != undefined) {
        jefe = $("#Jefe").val();
    }
    //<FINGTI_4081_3>
    var params = {
        tokenUsuario: $("#TokenUsuario").val(),
        fechaInicial: $("#FechaDesde").val(),
        fechaFinal: $("#FechaHasta").val(),
        numJefe: jefe,
        numSuperv: supervisor,//$("#Supervisor").val(),
        numAgente: $("#Agente").val(),
        indCierre: $("#CotizacionCerrada").val(),
        tipoCotizacion: $("#TipoCotizacion").val(),
        codCiaSeguro: $("#CiaSeguro").val()
    }

    $.ajax({
        type: "POST",
        url: "GestionVentas.aspx/CargarTablaGestionventas",
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {
            if (data.d != "TOKEN") {
                $('#TablaGestionVentasCargando').hide();
                $('#TablaGestionVentasContenedor').show();

                $('#TablaGestionVentasContenedor').html($(data.d).find('#ContenidoDinamico').html());

                $('#TabGestionVentas').Scrollable({
                    ScrollHeight: 340,
                    Width: 860
                });

                botoneraBloqueadoGestionVentas = false;
                $('#BuscarGestionVentas').attr('class', 'boton darkblue sharp');
                $('#EnviarExcelGestionVentas').attr('class', 'boton darkblue sharp');
                $('#EnviarCorreoGestionVentas').attr('class', 'boton darkblue sharp');

                // Deslizar la pantalla hacia los datos de simulación
                $('html,body').animate({
                    scrollTop: $('#ControlJefe').offset().top
                }, 'slow');

            }
            else {
                CerrarSesionExpirada();
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                document.location.reload(true);
            }
            else {
                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al consultar la Gestión de Ventas.<br/>" + JSON.parse(XMLHttpRequest.responseText).Message);
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            }
        },
        complete: function () {
            botoneraBloqueadoGestionVentas = false;
            $('#BuscarGestionVentas').attr('class', 'boton darkblue sharp');
            $('#EnviarExcelGestionVentas').attr('class', 'boton darkblue sharp');
            $('#EnviarCorreoGestionVentas').attr('class', 'boton darkblue sharp');
            $('#TablaGestionVentasCargando').hide();
        }
    });

}

function CargarTablaTelefonos() {
    if ($.trim($('#CUSPP').val()).length > 0) {
        $('#TablaTelefonosContenedor').hide();
        $('#TablaTelefonosCargando').show();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#CUSPP').val()
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaTelefonos',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaTelefonosCargando').hide();
                    $('#TablaTelefonosContenedor').show();
                    $('#TablaTelefonosContenedor').html($(data.d).find('#ContenidoDinamico').html());

                    if ($('#TabTelefonosDesplegar').val() == '0') {
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
                    $('#TablaTelefonosCargando').hide();
                    $('#TablaTelefonosError').show();
                }
            }
        });
    }
}

function CargarTablaGrupoFamiliar() {
    if ($.trim($('#HCUSPP').val()).length > 0) {
        $('#TablaGrupoFamiliarContenedor').hide();
        $('#TablaGrupoFamiliarCargando').show();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#HCUSPP').val()
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaGrupoFamiliar',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaGrupoFamiliarCargando').hide();
                    $('#TablaGrupoFamiliarContenedor').show();
                    $('#TablaGrupoFamiliarContenedor').html($(data.d).find('#ContenidoDinamico').html());
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
                    $('#TablaGrupoFamiliarCargando').hide();
                    $('#TablaGrupoFamiliarError').show();
                }
            }
        });
    }
}

function CargarTablaSolicitudes() {
    if ($.trim($('#HCUSPP').val()).length > 0) {
        $('#TablaSolicitudesContenedor').hide();
        $('#TablaSolicitudesCargando').show();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#HCUSPP').val()
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaSolicitudes',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaSolicitudesCargando').hide();
                    $('#TablaSolicitudesContenedor').show();
                    $('#TablaSolicitudesContenedor').html($(data.d).find('#ContenidoDinamico').html());
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
                    $('#TablaSolicitudesCargando').hide();
                    $('#TablaSolicitudesError').show();
                }
            }
        });
    }
}

//<SRI.INI-20322_E2>
function CargarTablaBandejaSolicitudesOficiales(solicitudesEscenarioAgente) {
    $('#TablaSolicitudesOficialesError').hide();
    $('#TablaSolicitudesOficialesContenedor').hide();
    $('#TablaSolicitudesOficialesCargando').show();

    var params = {
        solicitudesEscenarioAgente: solicitudesEscenarioAgente
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacion.aspx/CargarTablaBandejaSolicitudesOficiales',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            console.log(data)
            if (data.d != 'TOKEN') {
                $('#TablaSolicitudesOficialesError').hide();
                $('#TablaSolicitudesOficialesCargando').hide();
                $('#TablaSolicitudesOficialesContenedor').show();
                $('#TablaSolicitudesOficialesContenedor').html($(data.d).find('#ContenidoDinamico').html());

                //<INIGTI_4081>
                permisoRechazarSolicitud = ($('#PerRechazarSolicitud').val() == '1' ? true : false);
                if (!permisoRechazarSolicitud) {
                    $('#ModSolRechazarBandejaBloque').attr('class', 'botonDeshabilitado gris gris_sharp');
                }
                historialOficiales = new Array();
                //Abriendo o desplegando el arbol
                $(".FecPresentacionData").each(function (index, val) {
                    var sol = $(this).data("fecha");
                    historialOficiales.push(sol);
                    divexpandcollapse(sol);
                })
                //<FINGTI_4081>
                //<INIGTI_6556>
                if ($("#HSeleccionados").val() != "") {
                    var sol = $("#HSeleccionados").val();
                    var arrSolicitudes = new Array();
                    arrSolicitudes = sol.split(",");

                    $(".ChkSolicitudes").each(function (index, val) {
                        var t = $(this);
                        var itemSolicitud = $(this).data("solicitud");
                        $.each(arrSolicitudes, function (i, val) {
                            if (itemSolicitud == val) {
                                t.attr("checked", true);
                            };
                        });
                    });
                };
                //<FINGTI_6556>

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
//<SRI.FIN-20322_E2>

//<SRI.INI-20322_E2>
function CargarTablaSolicitudesOficiales(idJefe, idSupervisor, idAgente, primeraVez, idSolicitudResaltada) {
    if ($.trim(idJefe).length > 0) {
        $('#TablaSolicitudesOficialesError').hide();
        $('#TablaSolicitudesOficialesContenedor').hide();
        $('#TablaSolicitudesOficialesCargando').show();

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            numJefe: idJefe,
            numSupervisor: idSupervisor,
            numAgente: idAgente,
            idSolicitud: idSolicitudResaltada
        }
        $.ajax({
            type: 'POST',
            url: 'CotizadorOficiales.aspx/CargarTablaSolicitudesOficiales',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                // Refrescar la sesión
                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                $clock.countdown(selectedDate.toString());

                if (data.d != 'TOKEN') {
                    $('#TablaSolicitudesOficialesError').hide();
                    $('#TablaSolicitudesOficialesCargando').hide();
                    $('#TablaSolicitudesOficialesContenedor').show();
                    $('#TablaSolicitudesOficialesContenedor').html($(data.d).find('#ContenidoDinamico').html());

                    // Limpiar historial de la tabla
                    if (primeraVez) {
                        historialOficiales = new Array();
                    }
                    else {
                        ReplicarTablaSolicitudesOficiales();
                    }
                    //$(".grilla_highlight").addClass("grilla_active");

                    // Actualizar temporizadores para los lotes
                    $(".HoraCierre").each(function (index) {
                        var segundos = $(this).html();
                        if (segundos > 0) {
                            $relojlote = $(this).on("update.countdown", function (event) {
                                var semanas = event.strftime("%-w");
                                var dias = event.strftime("%-d");
                                var horas = event.strftime("%-H");
                                var minutos = event.strftime("%-M");
                                var color = "#000";
                                if (semanas == 0 && dias == 0 && horas == 0 && minutos <= 29) color = "#F00";
                                var formato = "%H:%M:%S";
                                if (semanas >= 1)
                                    formato = "%-w semana" + (semanas > 1 ? "s" : "") + " %-d día" + (dias != 1 ? "s" : "") + " " + formato;
                                else
                                    formato = "%-d día" + (dias != 1 ? "s" : "") + " " + formato;
                                $(this).html("<span style=\"color:" + color + "\">" + event.strftime(formato) + "</span>");
                            }).on('finish.countdown', function (event) {
                                $(this).html("<strong><span style=\"color:#F00\">Lote cerrado</span></strong>");
                            });

                            temporizador = new Date().valueOf() + parseFloat(segundos);
                            $relojlote.countdown(temporizador.toString());
                        }
                        else {
                            $(this).html("<strong><span style=\"color:#F00\">Lote cerrado</span></strong>");
                        }
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
                    $('#TablaSolicitudesOficialesCargando').hide();
                    $('#TablaSolicitudesOficialesError').show();
                }
            }
        });
    }
}

function divexpandcollapse(divname) {
    var img = "img" + divname;
    var htmlTable = "";
    if ($("#" + img).attr("src") == "../Imagenes/plus-icon.png") {
        htmlTable = $("#" + divname).html();
        $("#" + divname).remove();
        $("#" + img)
            .closest("tr")
            .after("<tr><td></td><td colspan = '100%'>" + htmlTable + "</td></tr>")
        $("#" + img).attr("src", "../Imagenes/minus-icon.png");
    } else {
        $("#" + img).closest("tr").next().css("display", "none");
        htmlTable = $("#" + img).closest("tr").next().html();
        htmlTable = htmlTable.replace("<td></td><td colspan=\"100%\">", "");
        htmlTable = htmlTable.substring(1, htmlTable.length - 5);
        htmlTable = htmlTable.replace("<div>", "<div id=\"" + divname + "\"  style=\"display:none\"> <div>");
        htmlTable = htmlTable + "</div>";
        $("#" + img).closest("tr").next().remove();
        $("#hrf" + divname).after(htmlTable);
        $("#" + img).attr("src", "../Imagenes/plus-icon.png");
    }
}
//<SRI.FIN-20322_E2>

function ReplicarTablaSolicitudesOficiales() {
    for (var i = 0; i < historialOficiales.length; i++) {
        divexpandcollapse(historialOficiales[i]);
    }
}

function CargarTablaCotizaciones(cot, tipoCotizacion) {
    $('#TablaCotizacionesContenedor').hide();
    $('#TablaCotizacionesCargando').show();
    var params = {
        cotizaciones: cot,
        tipoCotizacion: tipoCotizacion
    }
    $.ajax({
        type: 'POST',
        url: 'Cotizador.aspx/CargarTablaCotizaciones',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            $('#TablaCotizacionesContenedor').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaCotizacionesCargando').hide();
            $('#TablaCotizacionesContenedor').show();
            $('#ModalSolicitud').dialog({ position: ['center', 'center'] });

            $('.numerico').autoNumeric('init');
            //ActualizaEstiloCombobox('#TabCotizaciones select');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaCotizacionesCargando').hide();
                $('#TablaCotizacionesError').show();
            }
        }
    });
}

//<SRI.INI-20322_E2>
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
                var radio = $("#TabCotizaciones input[type=radio]:checked");
                var filaPadre = radio.parent().parent();
                filaPadre.addClass("grilla_active");
            }

            //<INIGTI_4081>
            if (Solicitud.TipoMovimiento.Id != 0) {
                for (i = 0; i < $('#TabCotizaciones tbody tr').length; i++) {
                    $('#TabCotizaciones tbody tr:eq(' + i + ') input[type=text]').attr("disabled", "true");
                }
            } else {
                if (SolicitudEscenario.NumOperacion.toString() == SolicitudEscenario.NumSolicitud) {
                    for (i = 0; i < $('#TabCotizaciones tbody tr').length; i++) {
                        $('#TabCotizaciones tbody tr:eq(' + i + ') input[type=text]').attr("disabled", "true");
                    }
                }
                //<GTI.INI-29372>
                else {
                    $('#TabCotizaciones th:nth-child(19)').hide();
                    $('#TabCotizaciones td:nth-child(19)').hide();
                }
                //<GTI.FIN-29372>
            }
            //<FINGTI_4081>

            //ActualizaEstiloCombobox('#TabCotizaciones select');
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
//<SRI.FIN-20322_E2>

//<SRI.INI-20322_E2>
function CargarTablaCotizacionesOficialesBandeja(cot, elegida, perTRA, perRadio) {

    $('#TablaCotizacionesContenedor').hide();
    $('#TablaCotizacionesCargando').show();
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

            $('#TablaCotizacionesContenedor').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaCotizacionesCargando').hide();
            $('#TablaCotizacionesContenedor').show();
            $('#ModalSolicitud').dialog({ position: ['center', 'center'] });

            $('.numerico').autoNumeric('init');
            if (elegida > 0) {
                var radio = $("#TabCotizaciones input[type=radio]:checked");
                var filaPadre = radio.parent().parent();
                filaPadre.addClass("grilla_active");
            }
            //ActualizaEstiloCombobox('#TabCotizaciones select');
            //alert(Solicitud.Cotizaciones[0].AjusteTRA);
            //alert(Solicitud);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaCotizacionesCargando').hide();
                $('#TablaCotizacionesError').show();
            }
        }
    });
}
//<SRI.FIN-20322_E2>

//<SRI.INI-20322_E2>
function CargarTablaCotizacionesOficialesBandejaMovil(cot, elegida, perTRA, perRadio) {
    $('#TablaCotizacionesContenedor').hide();
    $('#TablaCotizacionesCargando').show();
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

            $('#TablaCotizacionesContenedor').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaCotizacionesCargando').hide();
            $('#TablaCotizacionesContenedor').show();
            $('#ModalSolicitud').dialog({ position: ['center', 'center'] });

            $('.numerico').autoNumeric('init');
            if (elegida > 0) {
                var radio = $("#TabCotizaciones input[type=radio]:checked");
                var filaPadre = radio.parent().parent();
                filaPadre.addClass("grilla_active");
            }

            //<INIGTI_4081>
            if (Solicitud.TipoMovimiento.Id == 6) {
                for (i = 0; i < $('#TabCotizaciones tbody tr').length; i++) {
                    $('#TabCotizaciones tbody tr:eq(' + i + ') input[type=text]').attr("disabled", "true");
                }
            }
            if ($("#HBloqueaRadio").val() == "TRUE") {
                for (i = 0; i < $('#TabCotizaciones tbody tr').length; i++) {
                    $('#TabCotizaciones tbody tr:eq(' + i + ') input[type=radio]').attr("disabled", "true");
                }
            }
            //<FINGTI_4081>

            //ActualizaEstiloCombobox('#TabCotizaciones select');
            //alert(Solicitud.Cotizaciones[0].AjusteTRA);
            //alert(Solicitud);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaCotizacionesCargando').hide();
                $('#TablaCotizacionesError').show();
            }
        }
    });
}
//<SRI.FIN-20322_E2>

function CargarTablaBeneficiarios(ben) {
    if ($.trim($('#HCUSPP').val()).length > 0) {
        if (ben == null) {
            $('#TablaBeneficiariosContenedor').hide();
            $('#TablaBeneficiariosCargando').show();

            $('#ManSolNumBeneficiarios').hide();
            $('#ManSolNumBeneficiariosCargando').show();
        }
        else {
            $('#TablaRviBenefiContenedor').hide();
            $('#TablaRviBenefiCargando').show();

            // Convertir las Fechas a Date de Javascript para que sean correctamente parseados por el servidor
            for (var i = 0; i < ben.length; i++) {
                if (ben[i].FechaNacimiento != null)
                    ben[i].FechaNacimiento = new Date(+ben[i].FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
                if (ben[i].FechaInvalidez != null)
                    ben[i].FechaInvalidez = new Date(+ben[i].FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1"))
            }
        }

        var params = {
            cuspp: $('#HCUSPP').val(),
            beneficiarios: ben
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarTablaBeneficiarios',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (ben == null) {
                    $('#TablaBeneficiariosCargando').hide();
                    $('#TablaBeneficiariosContenedor').show();
                    $('#ManSolNumBeneficiariosCargando').hide();
                    $('#TablaBeneficiariosContenedor').html($(data.d).find('#ContenidoDinamico').html());
                    $('#ManSolNumBeneficiarios').show();
                    $('#ManSolNumBeneficiarios').html('(' + $('#TabBeneficiarios input[type=checkbox]:checked').length + ')');
                }
                else {
                    $('#TablaRviBenefiCargando').hide();
                    $('#TablaRviBenefiContenedor').show();
                    $('#TablaRviBenefiContenedor').html($(data.d).find('#ContenidoDinamico').html());
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    if (ben == null) {
                        $('#TablaBeneficiariosCargando').hide();
                        $('#TablaBeneficiariosError').show();
                    }
                    else {
                        $('#TablaRviBenefiCargando').hide();
                        $('#TablaRviBenefiError').show();
                    }
                }
            }
        });
    }
}

//<SRI.FIN-20322_E2>
function CargarTablaBeneficiariosOficial(ben) {
    if (ben == null) {
        $('#TablaBeneficiariosContenedor').hide();
        $('#TablaBeneficiariosCargando').show();

        $('#ManSolNumBeneficiarios').hide();
        $('#ManSolNumBeneficiariosCargando').show();
    }
    else {
        $('#TablaRviBenefiContenedor').hide();
        $('#TablaRviBenefiCargando').show();

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
                $('#TablaBeneficiariosCargando').hide();
                $('#TablaBeneficiariosContenedor').show();
                $('#ManSolNumBeneficiariosCargando').hide();
                $('#TablaBeneficiariosContenedor').html($(data.d).find('#ContenidoDinamico').html());
                $('#ManSolNumBeneficiarios').show();
                $('#ManSolNumBeneficiarios').html('(' + $('#TabBeneficiarios input[type=checkbox]:checked').length + ')');
            }
            else {
                $('#TablaRviBenefiCargando').hide();
                $('#TablaRviBenefiContenedor').show();
                $('#TablaRviBenefiContenedor').html($(data.d).find('#ContenidoDinamico').html());
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                if (ben == null) {
                    $('#TablaBeneficiariosCargando').hide();
                    $('#TablaBeneficiariosError').show();
                }
                else {
                    $('#TablaRviBenefiCargando').hide();
                    $('#TablaRviBenefiError').show();
                }
            }
        }
    });
}
//<SRI.INI-20322_E2>

//<SRI.FIN-20322_E2>
function CargarTablaBeneficiariosOficialBandeja(ben, cussp) {
    if (ben == null) {
        $('#TablaBeneficiariosContenedor').hide();
        $('#TablaBeneficiariosCargando').show();

        $('#ManSolNumBeneficiarios').hide();
        $('#ManSolNumBeneficiariosCargando').show();
    }
    else {
        $('#TablaRviBenefiContenedor').hide();
        $('#TablaRviBenefiCargando').show();

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
                $('#TablaBeneficiariosCargando').hide();
                $('#TablaBeneficiariosContenedor').show();
                $('#ManSolNumBeneficiariosCargando').hide();
                $('#TablaBeneficiariosContenedor').html($(data.d).find('#ContenidoDinamico').html());
                $('#ManSolNumBeneficiarios').show();
                $('#ManSolNumBeneficiarios').html('(' + $('#TabBeneficiarios input[type=checkbox]:checked').length + ')');
            }
            else {
                $('#TablaRviBenefiCargando').hide();
                $('#TablaRviBenefiContenedor').show();
                $('#TablaRviBenefiContenedor').html($(data.d).find('#ContenidoDinamico').html());
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                if (ben == null) {
                    $('#TablaBeneficiariosCargando').hide();
                    $('#TablaBeneficiariosError').show();
                }
                else {
                    $('#TablaRviBenefiCargando').hide();
                    $('#TablaRviBenefiError').show();
                }
            }
        }
    });
}
//<SRI.INI-20322_E2>

//<SRI.FIN-20322_E2>
function CargarTablaBeneficiariosOficialBandejaMovil(ben, cussp) {

    if (ben == null) {
        $('#TablaBeneficiariosContenedor').hide();
        $('#TablaBeneficiariosCargando').show();

        $('#ManSolNumBeneficiarios').hide();
        $('#ManSolNumBeneficiariosCargando').show();
    }
    else {
        $('#TablaRviBenefiContenedor').hide();
        $('#TablaRviBenefiCargando').show();

        // Convertir las Fechas a Date de Javascript para que sean correctamente parseados por el servidor
        /* for (var i = 0; i < ben.length; i++) {
            if (ben[i].FechaNacimiento != null)
                ben[i].FechaNacimiento = new Date(+ben[i].FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
            if (ben[i].FechaInvalidez != null)
                ben[i].FechaInvalidez = new Date(+ben[i].FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1"))
        } */
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
                $('#TablaBeneficiariosCargando').hide();
                $('#TablaBeneficiariosContenedor').show();
                $('#ManSolNumBeneficiariosCargando').hide();
                $('#TablaBeneficiariosContenedor').html($(data.d).find('#ContenidoDinamico').html());
                $('#ManSolNumBeneficiarios').show();
                $('#ManSolNumBeneficiarios').html('(' + $('#TabBeneficiarios input[type=checkbox]:checked').length + ')');
            }
            else {
                $('#TablaRviBenefiCargando').hide();
                $('#TablaRviBenefiContenedor').show();
                $('#TablaRviBenefiContenedor').html($(data.d).find('#ContenidoDinamico').html());
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                if (ben == null) {
                    $('#TablaBeneficiariosCargando').hide();
                    $('#TablaBeneficiariosError').show();
                }
                else {
                    $('#TablaRviBenefiCargando').hide();
                    $('#TablaRviBenefiError').show();
                }
            }
        }
    });
}
//<SRI.INI-20322_E2>

//<SRI.FIN-20322_E2>
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
//<SRI.INI-20322_E2>



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
                    if (data.d.Controles[3].length) $('#CUSPP').attr('class', data.d.Controles[3]);
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
                    if (data.d.Controles[3].length) $('#CUSPP').attr('class', data.d.Controles[3]);
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
    if ($.trim($('#CUSPP').val()).length > 0) {
        // Limpiar tabla de cotizaciones si estuviera activa
        $('#TablaCotizacionesError').hide();

        $('#TablaCotizacionesContenedor').hide();
        $('#TablaCotizacionesNoSolicitud').show();
        $('#Simulaciones').hide();

        $('#TablaSolicitudesContenedor').hide();
        $('#TablaSolicitudesCargando').show();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#CUSPP').val()
        }
        $.ajax({
            type: 'POST',
            url: rutaCargarTablaSolicitudesSimulador,
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaSolicitudesCargando').hide();
                    $('#TablaSolicitudesContenedor').show();
                    $('#TablaSolicitudesContenedor').html($(data.d).find('#ContenidoDinamico').html());

                    //<INIGTI_2145>
                    $("#TabSolicitudes td:nth-child(9), #TabSolicitudes th:nth-child(9)").hide();
                    if ($("#HCompara").val() == "TRUE") {
                        $("#TabSolicitudes td:nth-child(9), #TabSolicitudes th:nth-child(9)").show();
                    }
                    //<INIGTI_2145>
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
                    $('#TablaSolicitudesCargando').hide();
                    $('#TablaSolicitudesError').show();
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
        $('#TablaCotizacionesContenedor').hide();
        $('#TablaCotizacionesNoSolicitud').hide();
        $('#TablaCotizacionesCargando').show();
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
                    $('#TablaCotizacionesContenedor').html($(data.d).find('#ContenidoDinamico').html());
                    $('#TablaCotizacionesCargando').hide();
                    $('#TablaCotizacionesContenedor').show();

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
                $('#TablaCotizacionesCargando').hide();
                $('#TablaCotizacionesError').show();
            }
        }
    });
}


//<INIGTI_4081>

function CargarTablaTraDefault(idSolicitud) {
    $('#TablaTraDefaultContenedor').hide();
    $('#TablaTraDefaultCargando').show();
    var params = {
        idSolicitud: idSolicitud
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacionModificar.aspx/CargarTablaTraDefault',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            $('#TablaTraDefaultContenedor').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaTraDefaultCargando').hide();
            $('#TablaTraDefaultContenedor').show();
            //$('#ModalSolicitud').dialog({ position: ['center', 'center'] });
            $('.numerico').autoNumeric('init');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaTraDefaultCargando').hide();
                $('#TablaTraDefaultError').show();

                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista Tra Default.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
    });
}

function CargarTablaTasaMaximaTraMinima(idSolicitud, fecCotizacion) {
    $('#TablaTasaMaximaTraMinimaContenedor').hide();
    $('#TablaTasaMaximaTraMinimaCargando').show();
    var params = {
        idSolicitud: idSolicitud,
        fecCotizacion: fecCotizacion
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacionModificar.aspx/CargarTablaTasaMaximaTraMinima',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            $('#TablaTasaMaximaTraMinimaContenedor').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaTasaMaximaTraMinimaCargando').hide();
            $('#TablaTasaMaximaTraMinimaContenedor').show();

            ParametroEspecial = JSON.parse($('#HCotizaValPar').val());
            ParametroEspecialNew = JSON.parse($('#HCotizaValPar').val());

            //$('#ModalSolicitud').dialog({ position: ['center', 'center'] });
            $('.numerico').autoNumeric('init');
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaTasaMaximaTraMinimaCargando').hide();
                $('#TablaTasaMaximaTraMinimaError').show();

                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista Tasa Máxima y TRA Mínima.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
    });
}

/*
<div id="TablaCuotasTraCargando" align="center" style="height: 50px; padding: 82px 0">
            <asp:Image ID="icoTablaCuotasTraCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando las Cuotas Tra, espere por favor...</span>
        </div>

        <div id="TablaCuotasTraContenedor" style="display: none"></div>

*/
function CargarTablaCuotasTra(periodo, mes) {
    $('#TablaCuotasTraContenedor').hide();
    $('#TablaCuotasTraCargando').show();
    var params = {
        periodo: periodo,
        mes: mes
    }
    $.ajax({
        type: 'POST',
        url: 'CuotaTra.aspx/CargarTablaCuotasTra',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            $('#TablaCuotasTraContenedor').html($(data.d).find('#ContenidoDinamico').html());
            $('#TablaCuotasTraCargando').hide();
            $('#TablaCuotasTraContenedor').show();
            $('.numerico').autoNumeric('init');
            $('.numerico_int').numeric();

            listaCuotasTra = JSON.parse($('#HCuotasTra').val());

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

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaCuotasTraCargando').hide();
                $('#TablaTasaMaximaTraMinimaError').show();

                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista cuotas de TRA.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
    });
}



//Hace Scroll a la grilla

(function ($) {
    $.fn.Scrollable = function (options) {
        var defaults = {
            ScrollHeight: 300,
            Width: 0,
            IsInUpdatePanel: false
        };
        var options = $.extend(defaults, options);
        return this.each(function () {
            var grid = $(this).get(0);
            var gridId = grid.id;
            MakeScrollable(grid, options);
            if (options.IsInUpdatePanel) {
                var prm = Sys.WebForms.PageRequestManager.getInstance();
                if (prm != null) {
                    prm.add_endRequest(function (sender, e) {
                        MakeScrollable($("#" + gridId).get(0), options);
                    });
                }
            }
        });
    };
})(jQuery);

function MakeScrollable(grid, options) {
    var gridId = grid.id;
    var gridHeight = grid.offsetHeight;
    var headerCellWidths = new Array();
    for (var i = 0; i < grid.getElementsByTagName("TH").length; i++) {
        headerCellWidths[i] = grid.getElementsByTagName("TH")[i].offsetWidth;
    }
    grid.parentNode.appendChild(document.createElement("div"));
    var parentDiv = grid.parentNode;

    var table = document.createElement("table");
    for (i = 0; i < grid.attributes.length; i++) {
        if (grid.attributes[i].specified && grid.attributes[i].name != "id") {
            table.setAttribute(grid.attributes[i].name, grid.attributes[i].value);
        }
    }
    table.style.cssText = grid.style.cssText;
    table.appendChild(document.createElement("tbody"));
    table.getElementsByTagName("tbody")[0].appendChild(grid.getElementsByTagName("TR")[0]);
    var cells = table.getElementsByTagName("TH");

    var gridRow = grid.getElementsByTagName("TR")[0];
    for (var i = 0; i < cells.length; i++) {
        var width;
        if (headerCellWidths[i] > gridRow.getElementsByTagName("TD")[i].offsetWidth) {
            width = headerCellWidths[i];
        }
        else {
            width = gridRow.getElementsByTagName("TD")[i].offsetWidth;
        }
        cells[i].style.width = parseInt(width) + "px";
        //gridRow.getElementsByTagName("TD")[i].style.width = parseInt(width) + "px";
        $("tr", $(grid)).each(function () {
            $("td", this).eq(i).css("width", width);
        });
    }
    parentDiv.removeChild(grid);

    var dummyHeader = document.createElement("div");
    dummyHeader.id = "header" + gridId;
    dummyHeader.appendChild(table);
    parentDiv.appendChild(dummyHeader);
    var scrollableDiv = document.createElement("div");


    dummyHeader = document.getElementById(dummyHeader.id);
    dummyHeader.style.width = "9999999999999px";
    var table_width = dummyHeader.getElementsByTagName("table")[0].offsetWidth;
    dummyHeader.style.width = (table_width + 17) + "px";
    scrollableDiv.style.cssText = "overflow:auto;height:" + options.ScrollHeight + "px;width:" + (table_width + 17) + "px";
    scrollableDiv.appendChild(grid);
    parentDiv.appendChild(scrollableDiv);
    if (options.Width > 0) {
        parentDiv.style.cssText = "overflow:auto;width:" + options.Width + "px";
    }
};

//<FINGTI_4081>

//<INI.GTI_7012_2_1>
function CargarTablaSolicitudesReportePlus() {
    if ($.trim($('#CUSPP').val()).length > 0) {
        // Limpiar tabla de cotizaciones si estuviera activa
        $('#TablaSolicitudesError').hide();

        $('#Simulaciones').hide();

        $('#TablaSolicitudesContenedor').hide();
        $('#TablaSolicitudesCargando').show();
        $('#divBotonera').hide();
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            cuspp: $('#CUSPP').val()
        }
        $.ajax({
            type: 'POST',
            url: 'CompararCotizacionPlus.aspx/CargarTablaSolicitudes',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaSolicitudesCargando').hide();
                    $('#TablaSolicitudesContenedor').show();
                    $('#TablaSolicitudesContenedor').html($(data.d).find('#ContenidoDinamico').html());

                    $('#divBotonera, #divMaximo').hide();

                    if ($("#HRegistros").val() > 0) {
                        $('#divBotonera, #divMaximo').show();
                        $("#divBotonera").removeClass("pieBoton");
                        if ($("#HRegistros").val() > 12) {
                            $("#divBotonera").addClass("pieBoton");
                        }
                    }

                    historialOficiales = new Array();
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
                    $('#TablaSolicitudesCargando').hide();
                    $('#TablaSolicitudesError').show();
                }
            },
            complete: function () {
                botonBusAfiBuscarBloqueado = false;
                $('#SimBusAfiBuscar').attr('class', 'boton darkblue sharp');
            }
        });
    }
}


$('#TablaReporteSolicitudesPlusReintentar').live('click', function () {
    $('#TablaSolicitudesError').hide();
    CargarTablaSolicitudesReportePlus();
});

//<FIN.GTI_7012_2_1>

/**
* jQuery.browser.mobile (http://detectmobilebrowser.com/)
*
* jQuery.browser.mobile will be true if the browser is a mobile device
*
**/
(function (a) { jQuery.browser.mobile = /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4)) })(navigator.userAgent || navigator.vendor || window.opera);

