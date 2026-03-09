$(document).ready(function () {

    var permisoAprobarFlujoSolicitud;
    var permisoObservarFlujoSolicitud;
    var permisoRechazarFlujoSolicitud;
    var tipoFlujo;
       
	$("#divCargarObservacion").show();
    
	$("#ModSolNroSolicitud_RP").text($("#HNroSolicitud").val())
	$("#ModSolCuspp_RP").text($("#Hcuspp").val())
	$("#ModSolNombre_RP").text($("#Hnombre").val())
	$("#ModSolEstadoOpe_RP").text($("#HDescEstOpe").val())
	$("#ModSolEstadoPlaft_RP").text($("#HDescEstPlaft").val())

	permisoAprobarFlujoSolicitud = ($('#HAprobarFlujoSolicitud').val() == '1' ? true : false);
	permisoObservarFlujoSolicitud = ($('#HObservarFlujoSolicitud').val() == '1' ? true : false);
	permisoRechazarFlujoSolicitud = ($('#HRechazarFlujoSolicitud').val() == '1' ? true : false);

	if (!permisoAprobarFlujoSolicitud) {
	    $('#ModSolAprobar_RPP').css('display', 'none');
	}

	if (!permisoObservarFlujoSolicitud) {
	    $('#ModSolObservar_RPP').css('display', 'none');
	    $('#divCargarObservacion').css('display', 'none');
	}

	if (!permisoRechazarFlujoSolicitud) {
	    $('#ModSolRechazar_RPP').css('display', 'none');
	}

    /* Botón Aprobar */
	$("#ModSolAprobar_RPP").live("click", function () {
        	
	    tipoFlujo = 6;

	    $("#MCAIcono").attr("class", "advertencia");
	    $("#MCAContenedor").html("¿Deseas Aprobar la solicitud?");
	    $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
	    $("#ModalCuadroAdvertencia").dialog("open");

	});
    	
    /* Botón Rechazar */
	$("#ModSolRechazar_RPP").live("click", function () {

	    tipoFlujo = 7;

	    $("#MCAIcono").attr("class", "advertencia");
	    $("#MCAContenedor").html("¿Deseas rechazar la solicitud?");
	    $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
	    $("#ModalCuadroAdvertencia").dialog("open");

	});

    /* Botón Observar */
	$('#ModSolObservar_RPP').live('click', function () {
	    
	    tipoFlujo = 5;

	    $("#MCAIcono").attr("class", "advertencia");
	    $("#MCAContenedor").html("¿Deseas Observar la solicitud?");
	    $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
	    $("#ModalCuadroAdvertencia").dialog("open");
        
	});

    /* Botón Cancelar */
	$('#ModSolCancelar_RPP').live('click', function () {
	    window.location.href = "ListadoEvaluacion.aspx";
	});
    
    /* Botón Aceptar */
	$('#MCAAceptarFlujo_Plus').live('click', function () {
	   	        
	    $("#ModalCuadroAdvertencia").dialog("close");

	    CargandoFlujo(tipoFlujo);

	    var id_archivos = "";

	    if (DRIVE_FILES != null) {
	        if (DRIVE_FILES.length > 0) {
	            for (var i = 0; i < DRIVE_FILES.length; i++) {
	                id_archivos += DRIVE_FILES[i].Id + ",";
	            }
	        }
	    }

	    if (tipoFlujo == 6) {
	        
	        var params = {
	            tokenUsuario: $("#TokenUsuario").val(),
	            numSolicitud: $("#HNroSolicitud").val(),
	            observacion: $("#observacion").val(),
	            codEstadoRPP: "6",
	            id_archivos: id_archivos
	        }

	        $.ajax({
	            type: "POST",
	            url: "ListadoArchivoDrive.aspx/flujoSolicitud",
	            contentType: "application/json; charset=iso-8859-1",
	            dataType: "json",
	            data: $.toJSON(params),
	            success: function (data) {
	                $('#MCMEstado').val('');
	                $('#ModalCotizando').dialog('close');

	                if (data.d.Estado == "OK") {

	                    //window.location.href = "ListadoEvaluacion.aspx";
	                    $('#MCMEstado').val("OK");
	                    $('#MCMIcono').attr('class', "exito");
	                    $('#MCMContenedor').html(data.d.Mensaje);
	                    $('#ModalCuadroMensaje').dialog({ title: 'Éxito' });
	                    $('#ModalCuadroMensaje').dialog('open');
                        
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
	                $('#ModalCotizando').dialog('close');
	                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
	                    /* Sesión caducada */
	                    document.location.reload(true);
	                }
	                else {
	                    $('#MCMIcono').attr('class', 'error');
	                    $('#MCMContenedor').html('Ha ocurrido un error al aprobar la solicitud.');
	                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
	                    $('#ModalCuadroMensaje').dialog('open');
	                }
	                
	            }
	        });

	    }
	    else if (tipoFlujo == 7) {
	        var esCorrecto = true;
	        var errores = new Array();

	        var observacion = true;

	        $("#observacion").removeClass("formTextboxError");

	        if ($.trim($("#observacion").val()).length == 0) {
	            errores.push("El campo <strong>Observación</strong> debe ser ingresado.");
	            observacion = false;
	        }

	        if (!observacion) $('#observacion').addClass('formTextboxError');

	        esCorrecto = observacion;

	        if (!esCorrecto) {
	            $('#ModalCotizando').dialog('close');
	            $('#MCMIcono').attr('class', 'validacion');
	            $('#MCMContenedor').html(formatearError(errores));
	            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
	            $('#ModalCuadroMensaje').dialog('open');
	            return false;
	        }
	       

	        var params = {
	            tokenUsuario: $("#TokenUsuario").val(),
	            numSolicitud: $("#HNroSolicitud").val(),
	            observacion: $("#observacion").val(),
	            codEstadoRPP: "7",
	            id_archivos: id_archivos
	        }

	        $.ajax({
	            type: "POST",
	            url: "ListadoArchivoDrive.aspx/flujoSolicitud",
	            contentType: "application/json; charset=iso-8859-1",
	            dataType: "json",
	            data: $.toJSON(params),
	            success: function (data) {
	                $('#MCMEstado').val('');
	                $('#ModalCotizando').dialog('close');

	                if (data.d.Estado == "OK") {

	                    //window.location.href = "ListadoEvaluacion.aspx";
	                    $('#MCMEstado').val("OK");
	                    $('#MCMIcono').attr('class', "exito");
	                    $('#MCMContenedor').html(data.d.Mensaje);
	                    $('#ModalCuadroMensaje').dialog({ title: 'Éxito' });
	                    $('#ModalCuadroMensaje').dialog('open');

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
	                $('#ModalCotizando').dialog('close');
	                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
	                    /* Sesión caducada */
	                    document.location.reload(true);
	                }
	                else {
	                    $('#MCMIcono').attr('class', 'error');
	                    $('#MCMContenedor').html('Ha ocurrido un error al rechazar la solicitud.');
	                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
	                    $('#ModalCuadroMensaje').dialog('open');
	                }
	            }
	        });

        }
	    else if (tipoFlujo == 5) {
	       
	        var esCorrecto = true;
	        var errores = new Array();

	        var observacion = true;

	        $("#observacion").removeClass("formTextboxError");

	        if ($.trim($("#observacion").val()).length == 0) {
	            errores.push("El campo <strong>Observación</strong> debe ser ingresado.");
	            observacion = false;
	        }

	        if (!observacion) $('#observacion').addClass('formTextboxError');

	        esCorrecto = observacion;

	        if (!esCorrecto){
	            $('#ModalCotizando').dialog('close');
	            $('#MCMIcono').attr('class', 'validacion');
	            $('#MCMContenedor').html(formatearError(errores));
	            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
	            $('#ModalCuadroMensaje').dialog('open');
	            return false;
	        }

	        var params = {
	            tokenUsuario: $("#TokenUsuario").val(),
	            numSolicitud: $("#HNroSolicitud").val(),
	            observacion: $("#observacion").val(),
	            codEstadoRPP: "5",
	            id_archivos: id_archivos
	        }
	        $.ajax({
	            type: "POST",
	            url: "ListadoArchivoDrive.aspx/flujoSolicitud",
	            contentType: "application/json; charset=iso-8859-1",
	            dataType: "json",
	            data: $.toJSON(params),
	            success: function (data) {
	                
	                $('#MCMEstado').val('');
	                $('#ModalCotizando').dialog('close');

	                if (data.d.Estado == "OK") {
	                    $('#MCMEstado').val("OK");
	                    //window.location.href = "ListadoEvaluacion.aspx";

	                    $('#MCMIcono').attr('class', "exito");
	                    $('#MCMContenedor').html(data.d.Mensaje);
	                    $('#ModalCuadroMensaje').dialog({ title: 'Éxito' });
	                    $('#ModalCuadroMensaje').dialog('open');
                        
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
	                $('#ModalCotizando').dialog('close');
	                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
	                    /* Sesión caducada */
	                    document.location.reload(true);
	                }
	                else {
	                    $('#MCMIcono').attr('class', 'error');
	                    $('#MCMContenedor').html('Ha ocurrido un error al aprobar la solicitud.');
	                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
	                    $('#ModalCuadroMensaje').dialog('open');
	                }
	            }
	        });
	        
	    }

	});

    /* Botón Cancelar */
	$('#MCACancelarFlujo_Plus').live('click', function () {
	    $('#ModalCuadroAdvertencia').dialog('close');
	});

	$('#MCMAceptar').live('click', function () {
	    if ($('#MCMEstado').val() == 'OK') {
	        window.location.href = "ListadoEvaluacion.aspx";
	    } 
	});

	$("#divCargar").show();

	
    ObtenerArchivos($("#HNroSolicitud").val(), true);

	


});

var ListadoHeight = 0;
function auto_grow(element) {
    element.style.height = "28px";
    element.style.height = (element.scrollHeight) + "px";

    if (ListadoHeight == 0) {
        ListadoHeight = $("#ListadoArchivosRPP").height();
    }
    $("#ListadoArchivosRPP").height(ListadoHeight + element.scrollHeight);
}

function CargandoFlujo(tipoFlujo) {

    $('#MCIcono').attr('class', 'cargando');
    
    if (tipoFlujo == 6) {
        $('#MCContenedor').html('Aprobando la solicitud, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Aprobando' });
    }
    else if (tipoFlujo == 7) {
        $('#MCContenedor').html('Rechazando la solicitud, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Rechazando' });
    }
    else if (tipoFlujo == 5) {
        $('#MCContenedor').html('Observando la solicitud, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Observando' });

    }

    $('#ModalCotizando').dialog('open');
}

// <INI.GTI_26697>

$("#TabDocumentos .grilla_pdf").live("click", function () {

    $("#MCIcono").attr("class", "cargando");
    $("#MCContenedor").html("Cargando la información del formato, por favor espere un momento...");
    $("#ModalCotizando").dialog({ title: "Cargando" });
    $("#ModalCotizando").dialog("open");

    var idSolicitud = $(this).data('solicitud');
    var fecCotizacion = $(this).data('fechacotizacion');
    var opcionPDF = $(this).data('opcion');

    var params = {
        numSolicitud: idSolicitud,
        opcionPDF: opcionPDF,
        fecCotizacion: fecCotizacion
    };

    //console.log('prueba formatos pdf');

    $.ajax({
        type: 'POST',
        url: 'ListadoArchivoDrive.aspx/DescargarFormato',
        contentType: 'application/json; charset=iso-8859-1',
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {

                console.log('prueba formatos pdf ok');
                console.log('Plantilla/IFP/' + data.d.Mensaje);

                window.open('../Plantilla/IFP/' + data.d.Mensaje, '_blank');

                $("#ModalCotizando").dialog("close");

                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                $clock.countdown(selectedDate.toString());
            }
            else {
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#ModalCuadroMensaje').dialog('open');

                $("#ModalCotizando").dialog("close");
            }
        },
        error: function (XMLHttpRequest) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al exportar el documento.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');

                $("#ModalCotizando").dialog("close");
            }
        }
    });
    
    
    
});

// <FIN.GTI_26697>