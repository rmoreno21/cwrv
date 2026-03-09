/***************************\
|*          MELER          *|
\***************************/

$(document).ready(function () {
    /* Variables */
    var botonBuscarLoteBloqueado = false;
    var botonVerSolicitudesBloqueado = false;
    var botonCotizarLoteBloqueado = false;
    var botonBuscarConfirmacionesBloqueado = false;
    var botonConfirmarSolicitudesBloqueado = false;
    var botonBuscarCotizacionesMelerBloqueado = false;
    var botonGuardarCotizacionesMelerBloqueado = false;

    /* Botón Guardar Descarga de Solicitudes */
    $("#GuardarArchivo").live("click", function () {
        $("#ModalCargandoSolicitudes").dialog("open");
    });

    /* Botón Buscar Lote */
    $("#BuscarLote").live("click", function () {
        if (!botonBuscarLoteBloqueado) {
            //$('#GrupoCotizaciones').hide();
            //$('#DatosSimulacion').hide();

            var esCorrecto = true;
            var errores = new Array();

            $("#NumeroLote").removeClass("formTextboxError");
            $("#FechaDesde").removeClass("formTextboxError formCalendarError");
            $("#FechaHasta").removeClass("formTextboxError formCalendarError");

            var vlote = ($.trim($("#NumeroLote").val()).length > 0) ? true : false;
            var vfechadesde = ($.trim($("#FechaDesde").val()).length > 0) ? true : false;
            var vfechahasta = ($.trim($("#FechaHasta").val()).length > 0) ? true : false;

            if (!(vlote | vfechadesde | vfechahasta)) {
                errores.push("Debe ingresar un criterio de búsqueda.");
                esCorrecto = false;
            }

            if (vlote & (vfechadesde | vfechahasta)) {
                errores.push("Sólo debe ingresar un criterio de búsqueda (lote o fechas), no ambos.");
                if (vlote) $("#NumeroLote").addClass("formTextboxError");
                if (vfechadesde) $("#FechaDesde").addClass("formCalendarError");
                if (vfechahasta) $("#FechaHasta").addClass("formCalendarError");
                esCorrecto = false;
            }

            if (!vlote & ((vfechadesde & !vfechahasta) | (!vfechadesde & vfechahasta))) {
                var campo = "";
                if (!vfechadesde) {
                    $("#FechaDesde").addClass("formCalendarError");
                    campo = "Fecha Desde";
                }
                if (!vfechahasta) {
                    $("#FechaHasta").addClass("formCalendarError");
                    campo = "Fecha Hasta";
                }
                errores.push("Debe ingresar el campo <stong>" + campo + "</strong>.");
                esCorrecto = false;
            }

            if (!esCorrecto) {
                $("#MCMIcono").attr("class", "validacion");
                $("#MCMContenedor").html(formatearError(errores));
                $("#ModalCuadroMensaje").dialog({ title: "Validación" });
                $("#ModalCuadroMensaje").dialog("open");
                return false;
            }
            CargarTablaLotes();
        }
        else {
            return false;
        }
    });

    /* Botón Buscar Lote de Resultados*/
    $("#BuscarLoteResultados").live("click", function () {
        if (!botonBuscarLoteBloqueado) {

            var esCorrecto = true;
            var errores = new Array();

            $("#NumeroLote").removeClass("formTextboxError");
            $("#FechaDesde").removeClass("formTextboxError formCalendarError");
            $("#FechaHasta").removeClass("formTextboxError formCalendarError");

            var vlote = ($.trim($("#NumeroLote").val()).length > 0) ? true : false;
            var vfechadesde = ($.trim($("#FechaDesde").val()).length > 0) ? true : false;
            var vfechahasta = ($.trim($("#FechaHasta").val()).length > 0) ? true : false;

            if (!(vlote | vfechadesde | vfechahasta)) {
                errores.push("Debe ingresar un criterio de búsqueda.");
                esCorrecto = false;
            }

            if (vlote & (vfechadesde | vfechahasta)) {
                errores.push("Sólo debe ingresar un criterio de búsqueda (lote o fechas), no ambos.");
                if (vlote) $("#NumeroLote").addClass("formTextboxError");
                if (vfechadesde) $("#FechaDesde").addClass("formCalendarError");
                if (vfechahasta) $("#FechaHasta").addClass("formCalendarError");
                esCorrecto = false;
            }

            if (!vlote & ((vfechadesde & !vfechahasta) | (!vfechadesde & vfechahasta))) {
                var campo = "";
                if (!vfechadesde) {
                    $("#FechaDesde").addClass("formCalendarError");
                    campo = "Fecha Desde";
                }
                if (!vfechahasta) {
                    $("#FechaHasta").addClass("formCalendarError");
                    campo = "Fecha Hasta";
                }
                errores.push("Debe ingresar el campo <stong>" + campo + "</strong>.");
                esCorrecto = false;
            }

            if (!esCorrecto) {
                $("#MCMIcono").attr("class", "validacion");
                $("#MCMContenedor").html(formatearError(errores));
                $("#ModalCuadroMensaje").dialog({ title: "Validación" });
                $("#ModalCuadroMensaje").dialog("open");
                return false;
            }
            CargarTablaLotesResultado();
        }
        else {
            return false;
        }
    });

    /* Botón Reporte Descarga de Solicitudes */
    $("#TabLotes a.grilla_pdf").live("click", function () {
        // Obtener parámetro
        var numeroLote = $(this).data("lote");

        // Mostrar modal de cargando
        $("#ModalGenerandoReporte").dialog("open");

        // LLamando al método para exportar el reporte
        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            numeroLote: numeroLote
        }
        $.ajax({
            type: "POST",
            url: "DescargaSolicitudes.aspx/ExportarReporte",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                        window.location.href = "../Reportes/DescargaSolicitudesMovil.aspx";
                    }
                    else {
                        var w = 800;
                        var h = 600;
                        var left = (screen.width / 2) - (w / 2);
                        var top = (screen.height / 2) - (h / 2);
                        var nuevaVentana = window.open("../Reportes/DescargaSolicitudes.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                    }

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
                else {
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al generar el reporte.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            },
            complete: function () {
                $("#ModalGenerandoReporte").dialog("close");
            }
        });
    });

    /* Botón Reporte Descarga de Resultados */
    $("#TabLotesResultado a.grilla_pdf").live("click", function () {
        // Obtener parámetro
        var numeroLote = $(this).data("lote");
        window.open("../Reportes/LoteResultados.aspx?l=" + numeroLote);
    });

    /* Botón Ver Solicitudes */
    $("#VerSolicitudes").live("click", function () {
        if (!botonVerSolicitudesBloqueado) {
            //$('#GrupoCotizaciones').hide();
            //$('#DatosSimulacion').hide();

            var esCorrecto = true;
            var errores = new Array();

            $("#NumeroLote").removeClass("formTextboxError");

            var vlote = ($.trim($("#NumeroLote").val()).length > 0) ? true : false;

            if (!vlote) {
                errores.push("Debe ingresar un número de lote.");
                esCorrecto = false;
            }

            if (!esCorrecto) {
                $("#MCMIcono").attr("class", "validacion");
                $("#MCMContenedor").html(formatearError(errores));
                $("#ModalCuadroMensaje").dialog({ title: "Validación" });
                $("#ModalCuadroMensaje").dialog("open");
                return false;
            }
            CargarTablaSolicitudesLote();
        }
        else {
            return false;
        }
    });

    /* Botón Buscar Confirmaciones */
    $("#BuscarConfirmaciones").live("click", function () {
        if (!botonBuscarConfirmacionesBloqueado) {
            //$('#GrupoCotizaciones').hide();
            //$('#DatosSimulacion').hide();

            var esCorrecto = true;
            var errores = new Array();

            $("#FechaDesde").removeClass("formTextboxError formCalendarError");
            $("#FechaHasta").removeClass("formTextboxError formCalendarError");
            $("#ConConfirmacionEnviada").removeClass("formComboboxErrorContenedor");


            var vfechadesde = ($.trim($("#FechaDesde").val()).length > 0) ? true : false;
            var vfechahasta = ($.trim($("#FechaHasta").val()).length > 0) ? true : false;
            var venviada = ($("#ConfirmacionEnviada").val() != "0") ? true : false;

            if (!vfechadesde) {
                errores.push("Ingrese una <strong>Fec. confirmación desde</strong>. Campo obligatorio.");
                $("#FechaDesde").addClass("formTextboxError formCalendarError");
            }

            if (!vfechahasta) {
                errores.push("Ingrese una <strong>Fec. confirmación hasta</strong>. Campo obligatorio.");
                $("#FechaHasta").addClass("formTextboxError formCalendarError");
            }

            if (!venviada) {
                errores.push("Ingrese un valor en el campo <strong>Confirmación enviada</strong>. Campo obligatorio.");
                $("#ConConfirmacionEnviada").addClass("formComboboxErrorContenedor");
            }

            esCorrecto = vfechadesde & vfechahasta & venviada;

            if (!esCorrecto) {
                $("#MCMIcono").attr("class", "validacion");
                $("#MCMContenedor").html(formatearError(errores));
                $("#ModalCuadroMensaje").dialog({ title: "Validación" });
                $("#ModalCuadroMensaje").dialog("open");
                return false;
            }
            CargarTablaConfirmaciones();
        }
        else {
            return false;
        }
    });

    /* Botón Buscar Confirmaciones */
    $("#BuscarCotizacionesMeler").live("click", function () {
        if (!botonBuscarCotizacionesMelerBloqueado) {
            var esCorrecto = true;
            var errores = new Array();

            $("#FechaDesde").removeClass("formTextboxError formCalendarError");
            $("#FechaHasta").removeClass("formTextboxError formCalendarError");
            $("#ConSolicitudesEnviadas").removeClass("formComboboxErrorContenedor");


            var vfechadesde = ($.trim($("#FechaDesde").val()).length > 0) ? true : false;
            var vfechahasta = ($.trim($("#FechaHasta").val()).length > 0) ? true : false;
            var venviada = ($("#SolicitudesEnviadas").val() != "0") ? true : false;

            if (!vfechadesde) {
                errores.push("Ingrese una <strong>Fecha plazo AFP desde</strong>. Campo obligatorio.");
                $("#FechaDesde").addClass("formTextboxError formCalendarError");
            }

            if (!vfechahasta) {
                errores.push("Ingrese una <strong>Fecha plazo AFP hasta</strong>. Campo obligatorio.");
                $("#FechaHasta").addClass("formTextboxError formCalendarError");
            }

            if (!venviada) {
                errores.push("Ingrese un valor en el campo <strong>Solicitudes enviadas</strong>. Campo obligatorio.");
                $("#ConSolicitudesEnviadas").addClass("formComboboxErrorContenedor");
            }

            esCorrecto = vfechadesde & vfechahasta & venviada;

            if (!esCorrecto) {
                $("#MCMIcono").attr("class", "validacion");
                $("#MCMContenedor").html(formatearError(errores));
                $("#ModalCuadroMensaje").dialog({ title: "Validación" });
                $("#ModalCuadroMensaje").dialog("open");
                return false;
            }
            CargarTablaCotizacionesMeler();
        }
        else {
            return false;
        }
    });

    function CargarTablaLotes() {
        botonBuscarLoteBloqueado = true;
        $("#BuscarLote").attr("class", "botonDeshabilitado gris gris_sharp");
        $("#TablaLotesCargando").show();
        $("#TablaLotesContenedor").hide();

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            numeroLote: ($.trim($("#NumeroLote").val()).length > 0) ? $.trim($("#NumeroLote").val()) : "0",
            fechaDesde: $.trim($("#FechaDesde").val()),
            fechaHasta: $.trim($("#FechaHasta").val())
        }
        $.ajax({
            type: "POST",
            url: "DescargaSolicitudes.aspx/CargarTablaLotes",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#TablaLotesCargando").hide();
                    $("#TablaLotesContenedor").show();
                    $("#TablaLotesContenedor").html($(data.d.Contenido).find("#ContenidoDinamico").html());

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else if (data.d.Estado == "ERROR") {
                    $("#MCMIcono").attr("class", data.d.Icono);
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                    if (data.d.Controles != null) {
                        if (data.d.Controles[0].length) $('#NumeroLote').attr('class', data.d.Controles[0]);
                        if (data.d.Controles[1].length) $('#FechaDesde').attr('class', data.d.Controles[1]);
                        if (data.d.Controles[2].length) $('#FechaHasta').attr('class', data.d.Controles[2]);
                    }
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
            },
            complete: function () {
                botonBuscarLoteBloqueado = false;
                $("#TablaLotesCargando").hide();
                $("#BuscarLote").attr("class", "boton darkblue sharp");
            }
        });
    }

    function CargarTablaLotesResultado() {
        botonBuscarLoteBloqueado = true;
        $("#BuscarLoteResultados").attr("class", "botonDeshabilitado gris gris_sharp");
        $("#TablaLotesCargando").show();
        $("#TablaLotesContenedor").hide();

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            numeroLote: ($.trim($("#NumeroLote").val()).length > 0) ? $.trim($("#NumeroLote").val()) : "0",
            fechaDesde: $.trim($("#FechaDesde").val()),
            fechaHasta: $.trim($("#FechaHasta").val())
        }
        $.ajax({
            type: "POST",
            url: "DescargaResultados.aspx/CargarTablaLotes",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#TablaLotesCargando").hide();
                    $("#TablaLotesContenedor").show();
                    $("#TablaLotesContenedor").html($(data.d.Contenido).find("#ContenidoDinamico").html());

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else if (data.d.Estado == "ERROR") {
                    $("#MCMIcono").attr("class", data.d.Icono);
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                    if (data.d.Controles != null) {
                        if (data.d.Controles[0].length) $('#NumeroLote').attr('class', data.d.Controles[0]);
                        if (data.d.Controles[1].length) $('#FechaDesde').attr('class', data.d.Controles[1]);
                        if (data.d.Controles[2].length) $('#FechaHasta').attr('class', data.d.Controles[2]);
                    }
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
            },
            complete: function () {
                botonBuscarLoteBloqueado = false;
                $("#TablaLotesCargando").hide();
                $("#BuscarLoteResultados").attr("class", "boton darkblue sharp");
            }
        });
    }

    $("#TablaLotesReintentar").live("click", function () {
        $("#TablaLotesError").hide();
        CargarTablaLotes();
    });

    function CargarTablaSolicitudesLote() {
        botonBuscarLoteBloqueado = true;
        $("#VerSolicitudes").attr("class", "botonDeshabilitado gris gris_sharp");
        $("#TablaSolicitudesLoteCargando").show();
        $("#TablaSolicitudesLoteContenedor").hide();

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            lote: ($.trim($("#NumeroLote").val()).length > 0) ? $.trim($("#NumeroLote").val()) : 0
        }
        $.ajax({
            type: "POST",
            url: "CotizarLote.aspx/CargarTablaSolicitudesLote",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#TablaSolicitudesLoteCargando").hide();
                    $("#TablaSolicitudesLoteContenedor").show();
                    $("#TablaSolicitudesLoteContenedor").html($(data.d.Contenido).find("#ContenidoDinamico").html());

                    // Contador de solicitudes
                    $("#NroSolicitudes").html($(".chklote:enabled").length);
                    ValidarSolicitudesSeleccionadas();

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else if (data.d.Estado == "ERROR") {
                    $("TablaSolicitudesLoteError").show();
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

                $("TablaSolicitudesLoteError").show();
                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al cargar la información de los lotes.");
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            },
            complete: function () {
                botonBuscarLoteBloqueado = false;
                $("#TablaSolicitudesLoteCargando").hide();
                $("#VerSolicitudes").attr("class", "boton darkblue sharp");
            }
        });
    }

    $("#TablaSolicitudesLoteReintentar").live("click", function () {
        $("#TablaSolicitudesLoteError").hide();
        CargarTablaLotes();
    });

    function CargarTablaConfirmaciones() {
        botonBuscarConfirmacionesBloqueado = true;
        $("#BuscarConfirmaciones").attr("class", "botonDeshabilitado gris gris_sharp");
        $("#TablaConfirmacionesCargando").show();
        $("#TablaConfirmacionesContenedor").hide();

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            fechaInicio: $("#FechaDesde").val(),
            fechaFin: $("#FechaHasta").val(),
            enviado: $("#ConfirmacionEnviada").val()
        }
        $.ajax({
            type: "POST",
            url: "CargaConfirmaciones.aspx/CargarSolicitudesConfirmacion",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#TablaConfirmacionesCargando").hide();
                    $("#TablaConfirmacionesContenedor").show();
                    $("#TablaConfirmacionesContenedor").html($(data.d.Contenido).find("#ContenidoDinamico").html());

                    // Contador de solicitudes
                    $("#NroSolicitudes").html("0");
                    ValidarSolicitudesSeleccionadas();

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else if (data.d.Estado == "ERROR") {
                    $("TablaConfirmacionesError").show();
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

                $("TablaConfirmacionesError").show();
                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al cargar la información de las solicitudes.");
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            },
            complete: function () {
                botonBuscarConfirmacionesBloqueado = false;
                $("#TablaConfirmacionesCargando").hide();
                $("#BuscarConfirmaciones").attr("class", "boton darkblue sharp");
            }
        });
    }

    $("#TablaConfirmacionesReintentar").live("click", function () {
        $("#TablaConfirmacionesError").hide();
        CargarTablaConfirmaciones();
    });

    function CargarTablaCotizacionesMeler() {
        botonBuscarConfirmacionesBloqueado = true;
        $("#BuscarCotizacionesMeler").attr("class", "botonDeshabilitado gris gris_sharp");
        $("#TablaCotizacionesMelerCargando").show();
        $("#TablaCotizacionesMelerContenedor").hide();

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            fechaInicio: $("#FechaDesde").val(),
            fechaFin: $("#FechaHasta").val(),
            enviado: $("#SolicitudesEnviadas").val()
        }
        $.ajax({
            type: "POST",
            url: "CargaCotizaciones.aspx/CargarCotizacionesMeler",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#TablaCotizacionesMelerCargando").hide();
                    $("#TablaCotizacionesMelerContenedor").show();
                    $("#TablaCotizacionesMelerContenedor").html($(data.d.Contenido).find("#ContenidoDinamico").html());

                    // Contador de solicitudes
                    $("#NroSolicitudes").html("0");
                    ValidarSolicitudesSeleccionadas();

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else if (data.d.Estado == "ERROR") {
                    $("TablaCotizacionesMelerError").show();
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

                $("TablaCotizacionesMelerError").show();
                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al cargar la información de las solicitudes.");
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            },
            complete: function () {
                botonBuscarCotizacionesMelerBloqueado = false;
                $("#TablaCotizacionesMelerCargando").hide();
                $("#BuscarCotizacionesMeler").attr("class", "boton darkblue sharp");
            }
        });
    }

    $("#TablaCotizacionesMelerReintentar").live("click", function () {
        $("#TablaCotizacionesMelerError").hide();
        CargarTablaCotizacionesMeler();
    });

    $("#SeleccionarTodos").live("change", function () {
        if ($("#SeleccionarTodos").prop("checked")) {
            $(".chklote:enabled").prop("checked", true);
        }
        else {
            $(".chklote").removeProp("checked");
        }

        // Contador de solicitudes
        $("#NroSolicitudes").html($(".chklote:checked").length);

        ValidarSolicitudesSeleccionadas();
    });

    $(".chklote").live("change", function () {
        // Contador de solicitudes
        $("#NroSolicitudes").html($(".chklote:checked").length);

        ValidarSolicitudesSeleccionadas();
    });

    function ValidarSolicitudesSeleccionadas() {
        if ($(".chklote:checked").length == 0) {
            botonCotizarLoteBloqueado = true;
            botonConfirmarSolicitudesBloqueado = true;
            botonGuardarCotizacionesMelerBloqueado = true;
            $("#CotizarLote,#GuardarConfirmacion,#GuardarCotizacionesMeler")
                .attr("class", "botonDeshabilitado gris gris_sharp");
        }
        else {
            botonCotizarLoteBloqueado = false;
            botonConfirmarSolicitudesBloqueado = false;
            botonGuardarCotizacionesMelerBloqueado = false;
            $("#CotizarLote,#GuardarConfirmacion,#GuardarCotizacionesMeler")
                .attr("class", "boton darkblue sharp");
        }
    }

    /* Botón Cotizar Lote */
    $("#CotizarLote").live("click", function () {
        if (!botonCotizarLoteBloqueado) {
            // Bloquear botón
            $("#CotizarLote").attr("class", "botonDeshabilitado gris gris_sharp");
            botonCotizarLoteBloqueado = true;

            // Levantar modal "Cotizando Lote"
            $("#MCMensaje").html("Cotizando el lote, por favor espere un momento...");
            $("#MCAdvertencia").html("NO cierre su navegador hasta que el proceso termine.");
            $("#ModalCotizandoLote").dialog({ title: "Cotizando" });
            $("#ModalCotizandoLote").dialog("open");

            // Variables
            var contador = 0;
            var totalCotizaciones = $(".chklote:checked").length;
            var listaCotizaciones = "";

            $("#MCProgreso").html("Progreso: 0/" + totalCotizaciones + " solicitudes cotizadas (0%)");
            $(".BarraProgreso").css("width", 0);

            // URL para la primera llamada fetch
            const url = $('#url_api_rentas_rv').val() + '/cotizacion/cotizar-por-lote';

            // Cotizar todas las solicitudes marcadas
            $(".chklote:checked").each(function (index, element) {
                listaCotizaciones += $(this).val() + ",";
                var currentElement = $(this); // Capturar referencia del elemento actual
                var params = {
                    idSolicitud: currentElement.val(),
                    fechaCotizacion: $("#FechaCierre").val()
                }
                    (async () => {
                        try {
                            const response = await fetch(url, {
                                method: 'POST',
                                headers: {
                                    'Content-Type': 'application/json; charset=iso-8859-1',
                                    'x-username': $('#usuario_actual').val(),
                                    'x-rol': $('#rol_azman').val()
                                },
                                body: JSON.stringify(params)
                            });

                            const data = await response.json();

                            if (data.data.Estado == "OK") {
                                //$("#MCMensaje").html(currentElement.val() + ": " + data.data.Mensaje);

                                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                $clock.countdown(selectedDate.toString());
                            }
                            else if (data.data.Estado == "ERROR") {
                                $("TablaLotesError").show();
                                $("#MCMIcono").attr("class", data.data.Icono);
                                $("#MCMContenedor").html(data.data.Mensaje);
                                $("#ModalCuadroMensaje").dialog({ title: data.data.Titulo });
                                $("#ModalCuadroMensaje").dialog("open");
                            }
                            else if (data.data.Estado == "TOKEN") {
                                CerrarSesionExpirada();
                            }
                        } catch (error) {
                            if (error.status === 401 || error.status === 12030) {
                                /* Sesión caducada */
                                document.location.reload(true);
                            }

                            $("#MCMIcono").attr("class", "error");
                            $("#MCMContenedor").html("Ha ocurrido un error al cargar la información de los lotes.");
                            $("#ModalCuadroMensaje").dialog({ title: "Error" });
                            $("#ModalCuadroMensaje").dialog("open");
                        } finally {
                            contador++;

                            $("#MCProgreso").html("Progreso: " + contador + "/" + totalCotizaciones + " solicitudes cotizadas (" + Math.round((contador / totalCotizaciones) * 100) + "%)");
                            $(".BarraProgreso").animate({
                                width: Math.round((contador / totalCotizaciones) * 350)
                            }, 500, "linear", function () {
                                if (contador == totalCotizaciones) {
                                    $("#ModalCotizandoLote").dialog("close");
                                    $("#CotizarLote").attr("class", "boton darkblue sharp");
                                    botonCotizarLoteBloqueado = false;

                                    $("#MCMIcono").attr("class", "exito");
                                    $("#MCMContenedor").html("Lote cotizado correctamente.");
                                    $("#ModalCuadroMensaje").dialog({ title: "Mensaje" });
                                    $("#ModalCuadroMensaje").dialog("open");

                                    //<INIGTI_1092>
                                    var params3 = {
                                        lote: ($.trim($("#NumeroLote").val()).length > 0) ? $.trim($("#NumeroLote").val()) : 0,
                                        solicitudes: listaCotizaciones
                                    }
                                    listaCotizaciones = "";
                                    $.ajax({
                                        type: "POST",
                                        url: "CotizarLote.aspx/SolicitudesHabilitadas",
                                        contentType: "application/json; charset=iso-8859-1",
                                        dataType: "json",
                                        data: $.toJSON(params3),
                                        success: function (data) {
                                            listaCotizaciones = data.d;
                                            // Abrir reporte de Detalle Cotización del Lote
                                            var params2 = {
                                                idSolicitud: listaCotizaciones,
                                                fecCotizacion: $("#FechaCierre").val()
                                            }

                                            $.ajax({
                                                type: "POST",
                                                url: "CotizarLote.aspx/ExportarLotePDF",
                                                contentType: "application/json; charset=iso-8859-1",
                                                dataType: "json",
                                                data: $.toJSON(params2),
                                                success: function (data) {
                                                    if (data.d.Estado == "OK") {
                                                        if (movil) {
                                                            window.location.href = "../Reportes/DetalleCotizacionLoteMovil.aspx";
                                                        }
                                                        else {
                                                            var w = 800;
                                                            var h = 600;
                                                            var left = (screen.width / 2) - (w / 2);
                                                            var top = (screen.height / 2) - (h / 2);
                                                            var nuevaVentana = window.open("../Reportes/DetalleCotizacionLote.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                                                        }

                                                        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                                        $clock.countdown(selectedDate.toString());
                                                    }
                                                    else {
                                                        $("#MCMIcono").attr("class", data.d.Icono);
                                                        $("#MCMContenedor").html(data.d.Mensaje);
                                                        $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                                                        $("#ModalCuadroMensaje").dialog("open");
                                                    }
                                                },
                                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                                        /* Sesión caducada */
                                                        document.location.reload(true);
                                                    }
                                                    else {
                                                        $("#MCMIcono").attr("class", "error");
                                                        $("#MCMContenedor").html("Ha ocurrido un error al generar el reporte de las solicitudes.");
                                                        $("#ModalCuadroMensaje").dialog({ title: "Error" });
                                                        $("#ModalCuadroMensaje").dialog("open");
                                                    }
                                                }
                                            });

                                        },
                                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                                            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                                /* Sesión caducada */
                                                document.location.reload(true);
                                            }
                                            else {
                                                $("#MCMIcono").attr("class", "error");
                                                $("#MCMContenedor").html("Ha ocurrido un error al generar el reporte de las solicitudes.");
                                                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                                                $("#ModalCuadroMensaje").dialog("open");
                                            }
                                        }
                                    });

                                    //<INIGTI_1092>


                                }
                            });
                        }
                    })();
            });
        }
    });

    /* Botón Confirmar Solicitudes */
    $("#GuardarConfirmacion").live("click", function () {
        if (!botonConfirmarSolicitudesBloqueado) {
            // Bloquear botón
            $("#GuardarConfirmacion").attr("class", "botonDeshabilitado gris gris_sharp");
            botonConfirmarSolicitudesBloqueado = true;

            // Levantar modal "Guardando Confirmaciones"
            $("#ModalGuardandoConfirmaciones").dialog({ title: "Guardando" });
            $("#ModalGuardandoConfirmaciones").dialog("open");

            // Variables
            var contador = 0;
            var totalCotizaciones = $(".chklote:checked").length;

            // Armar arreglo
            var arrSolicitudes = new Array();

            $(".chklote:checked").each(function (index, element) {
                arrSolicitudes[contador] = $(this).val();
                contador++;
            });

            var params = {
                tokenUsuario: $("#TokenUsuario").val(),
                solicitudes: arrSolicitudes
            }
            $.ajax({
                type: "POST",
                url: "CargaConfirmaciones.aspx/ModificarSolicitudesConfirmacion",
                contentType: "application/json; charset=iso-8859-1",
                dataType: "json",
                data: $.toJSON(params),
                success: function (data) {
                    if (data.d.Estado == "OK") {
                        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                        $clock.countdown(selectedDate.toString());

                        $("#MCMIcono").attr("class", data.d.Icono);
                        $("#MCMContenedor").html(data.d.Mensaje);
                        $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                        $("#ModalCuadroMensaje").dialog("open");

                        // Descargar archivo XML
                        location.replace(rutaXMLConfirmacionMELER);
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
                    $("#MCMContenedor").html("Ha ocurrido un error al guardar la información de las solicitudes.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                },
                complete: function () {
                    $("#ModalGuardandoConfirmaciones").dialog("close");
                    $("#GuardarConfirmacion").attr("class", "boton darkblue sharp");
                    botonConfirmarSolicitudesBloqueado = false;
                }
            });
        }
    });

    /* Botón Guardar Cotizaciones Meler */
    $("#GuardarCotizacionesMeler").live("click", function () {
        if (!botonGuardarCotizacionesMelerBloqueado) {
            // Bloquear botón
            $("#GuardarCotizacionesMeler").attr("class", "botonDeshabilitado gris gris_sharp");
            botonGuardarCotizacionesMelerBloqueado = true;

            // Levantar modal "Guardando Confirmaciones"
            $("#ModalGuardandoCotizacionesMeler").dialog({ title: "Guardando" });
            $("#ModalGuardandoCotizacionesMeler").dialog("open");

            // Variables
            var contador = 0;
            var totalCotizaciones = $(".chklote:checked").length;


            // Armar arreglo
            var arrSolicitudes = new Array();

            $(".chklote:checked").each(function (index, element) {
                arrSolicitudes[contador] = $(this).val();
                contador++;
            });

            var params = {
                tokenUsuario: $("#TokenUsuario").val(),
                solicitudes: arrSolicitudes
            }
            $.ajax({
                type: "POST",
                url: "CargaCotizaciones.aspx/ModificarCotizacionesMeler",
                contentType: "application/json; charset=iso-8859-1",
                dataType: "json",
                data: $.toJSON(params),
                success: function (data) {
                    if (data.d.Estado == "OK") {
                        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                        $clock.countdown(selectedDate.toString());

                        $("#MCMIcono").attr("class", data.d.Icono);
                        $("#MCMContenedor").html(data.d.Mensaje);
                        $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                        $("#ModalCuadroMensaje").dialog("open");

                        // Descargar archivo XML
                        location.replace(rutaXMLCotizacionesMELER);
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
                    $("#MCMContenedor").html("Ha ocurrido un error al guardar la información de las solicitudes.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                },
                complete: function () {
                    $("#ModalGuardandoCotizacionesMeler").dialog("close");
                    $("#GuardarCotizacionesMeler").attr("class", "boton darkblue sharp");
                    botonGuardarCotizacionesMelerBloqueado = false;
                }
            });
        }
    });
});