var DRIVE_FILES = [];
var cantidad_drive_files;

var bOcultarBotonDrive = false;
var idElminarArchivo;

var extensiones_permitidas;
var megas_permitidos;

async function ObtenerArchivos(num_solicitud, hide_loading) {
    $("#drive-box").show();
    $("#drive-box").css("display", "inline-block");
    showLoading();

    var params = {
        tokenUsuario: $("#TokenUsuario").val(),
        num_solicitud: num_solicitud
    };

    $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
    $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');

    if ($('#HEstado').val() == "4" || $('#HEstadoPlaft').val() == "4") {
        $("#button-upload").hide();
        $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
        $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');
    } else if ($('#HEstado').val() == "6" && $('#HEstadoPlaft').val() == "-1") {
        $("#button-upload").hide();
        $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
        $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');
    }

    //debugger
    //const gaa = await ApiCotizadorIFP.ObtenerArchivosStorage('IFP_411661');


    $.ajax({
        type: 'POST',
        url: '../Builder/Utilitarios/UploadFiles.aspx/ObtenerArchivos',
        contentType: "application/json; charset=utf8",
        dataType: 'json',
        data: JSON.stringify(params),
        success: function (data) {
            console.log(data);
            DRIVE_FILES = data.d;
            var fText = "";

            if (DRIVE_FILES != null) {
                if (DRIVE_FILES.length > 0) {

                    for (var i = 0; i < DRIVE_FILES.length; i++) {

                        DRIVE_FILES[i].ThumbnailLink = DRIVE_FILES[i].ThumbnailLink || '';
                        DRIVE_FILES[i].FileType = (DRIVE_FILES[i].FileExtension == null) ? "folder" : "file";



                        var textTitle = (DRIVE_FILES[i].FileType != "file") ? "Browse " + DRIVE_FILES[i].Name : DRIVE_FILES[i].Name;

                        fText += "<div class='" + DRIVE_FILES[i].FileType + "-box'>";

                        if (DRIVE_FILES[i].ThumbnailLink) {
                            fText += "<div class='image-icon'><div class='image-preview'><a href='" + DRIVE_FILES[i].ThumbnailLink.replace("s220", "s800") + "' data-lightbox='image-" + i + "'><img src='" + DRIVE_FILES[i].ThumbnailLink + "'/></a></div></div>";
                        } else {
                            fText += "<div class='file-icon'><div class='image-preview'><img src='../Estilos/images/" + DRIVE_FILES[i].FileExtension + "-icon.png" + "'/></div></div>";
                        }

                        fText += "<div class='item-title'>" + DRIVE_FILES[i].Name + "</div>";

                        //button actions
                        fText += "<div class='button-box'>";

                        fText += "<div class='button-text' title='Visualizar' data-id='" + DRIVE_FILES[i].Id + "' data-file-counter='" + i + "'></div>";

                        if (DRIVE_FILES[i].FileType != "folder") {
                            fText += "<div class='button-download' title='Descargar' data-id='" + DRIVE_FILES[i].Id + "' data-file-counter='" + i + "'></div>";
                        }

                        //if (bOcultarBotonDrive) {
                        if (DRIVE_FILES[i].Capabilities.CanDelete) {
                            fText += "<div class='button-delete' title='Eliminar' data-id='" + DRIVE_FILES[i].Id + "'></div>";
                        }
                        //}

                        fText += "</div>";

                        //closing div    
                        fText += "</div>";

                    }
                } else {
                    fText = 'No se encontraron archivos.';
                    $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
                    $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');
                }
                //$('#ModSolAgregarArchivos').removeAttr('disabled');
                //$('#ModSolAgregarArchivos').attr('class', 'boton darkblue sharp');
            }
            else {
                fText = 'No se encontraron archivos.';
                $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
                $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            hideStatus();
            $("#drive-content").html(fText);
            initDriveButtons();
            if (hide_loading) {
                hideLoading();

                //Plaft
                //-1 = No Eval, 1 = Obs, 2 = Rech, 3 = Aprob, 4 = Eval
                //Ope-rpp
                //1 = Selec, 2 = Cerr, 3 = Anul, 4 = Eval, 5 = Obser, 6 = Aprob, 7 = Rech

                if ($('#HEstado').val() == "1") {
                    if (fText != 'No se encontraron archivos.') {
                        $('#ModSolEnviarEvaluacion').removeAttr('disabled');
                        $('#ModSolEnviarEvaluacion').attr('class', 'boton darkblue sharp');
                    }
                }
                else if ($('#HEstado').val() == "6" && $('#HEstadoPlaft').val() == "3") {
                    $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
                    $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');
                }
                else if (($('#HEstado').val() == "5" || $('#HEstado').val() == "6") && ($('#HEstadoPlaft').val() == "-1" || $('#HEstadoPlaft').val() == "1" || $('#HEstadoPlaft').val() == "3")) {
                    if (fText != 'No se encontraron archivos.') {
                        $('#ModSolEnviarEvaluacion').removeAttr('disabled');
                        $('#ModSolEnviarEvaluacion').attr('class', 'boton darkblue sharp');
                    }
                }

                //if ($('#HEstado').val() == "1" || $('#HEstado').val() == "5" || $('#HEstadoPlaft').val() == "1" ) {
                //    if (fText != 'No se encontraron archivos.') {
                //        $('#ModSolEnviarEvaluacion').removeAttr('disabled');
                //        $('#ModSolEnviarEvaluacion').attr('class', 'boton darkblue sharp');
                //    } 
                //}

            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
        }
    });


}

function initDriveButtons() {
    //Initiate the delete button click
    $(".button-delete").unbind("click");
    $(".button-delete").click(function () {
        //var c = confirm("Are you sure you want to delete this?");
        //if (c) {
        //    showLoading();
        //    //showStatus("Eliminando archivo en progreso...");
        //    var request = gapi.client.drive.files.delete({
        //        'fileId': $(this).attr("data-id")
        //    });

        //    request.execute(function (resp) {
        //        hideStatus();
        //        if (resp.error) {
        //            showErrorMessage("Error: " + resp.error.message);
        //        }
        //        //getDriveFiles();
        //    });
        //}

        idElminarArchivo = $(this).attr("data-id");
        showAsk("¿Confirma la eliminación del archivo?");

    });

    //Initiate the download button
    $(".button-download").unbind("click");
    $(".button-download").click(function () {

        $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
        $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');

        showLoading();
        showStatus("Descarga de archivo en progreso...");
        FILE_COUNTER = $(this).attr("data-file-counter");

        var solicitud = $("#HNroSolicitud").length ? $("#HNroSolicitud").val() : $("#NumeroSolicitud").val();

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            idArchivo: $(this).attr("data-id"),
            num_solicitud: solicitud
        };

        $.ajax({
            type: 'POST',
            url: '../Builder/Utilitarios/UploadFiles.aspx/DescargaArchivo',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: JSON.stringify(params),
            success: function (data) {


                if (data.d.Estado = 'OK') {
                    setTimeout(function () {


                        var link = document.createElement("a");
                        link.download = DRIVE_FILES[FILE_COUNTER].Name;
                        link.href = DRIVE_FILES[FILE_COUNTER].WebContentLink;
                        document.body.appendChild(link);
                        link.click();
                        document.body.removeChild(link);
                        delete link;

                        hideLoading();
                        hideStatus();

                        $('#ModSolEnviarEvaluacion').removeAttr('disabled');
                        $('#ModSolEnviarEvaluacion').attr('class', 'boton darkblue sharp');

                    }, 1000);
                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
            }
        });

    });

    //Initiate the get text button
    $(".button-text").unbind("click");
    $(".button-text").click(function () {

        $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
        $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');


        showLoading();
        showStatus("Visualizando archivo en progreso...");
        FILE_COUNTER = $(this).attr("data-file-counter");

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            idArchivo: $(this).attr("data-id"),
            num_solicitud: $('#HNroSolicitud').val()
        }

        $.ajax({
            type: 'POST',
            url: '../Builder/Utilitarios/UploadFiles.aspx/DescargaArchivo',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: JSON.stringify(params),
            success: function (data) {


                if (data.d.Estado = 'OK') {
                    setTimeout(function () {

                        //window.open(DRIVE_FILES[FILE_COUNTER].WebContentLink, '_blank');
                        window.open(DRIVE_FILES[FILE_COUNTER].WebContentLink, '_blank');
                        hideLoading();
                        hideStatus();

                        $('#ModSolEnviarEvaluacion').removeAttr('disabled');
                        $('#ModSolEnviarEvaluacion').attr('class', 'boton darkblue sharp');

                    }, 1000);


                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
            }
        });

    });

    //Initiate the click folder browse icon
    $(".folder-icon").unbind("click");
    $(".folder-icon").click(function () {
        browseFolder($(this));
    });

    //Initiate the breadcrumb navigation link click
    $("#drive-breadcrumb a").unbind("click");
    $("#drive-breadcrumb a").click(function () {
        browseFolder($(this));
    });
}

/******************** NOTIFICATION ********************/

//show loading animation
function showLoading() {
    if ($("#drive-box-loading").length === 0) {
        $("#drive-box").prepend("<div id='drive-box-loading'></div>");
    }
    $("#drive-box-loading").html("<div id='loading-wrapper'><div id='loading'><img src='../Estilos/images/loading-bubble.gif'></div></div>");
}

//hide loading animation
function hideLoading() {
    $("#drive-box-loading").html("");
}

//show status message
function showStatus(text) {
    $("#status-message").show();
    $("#status-message").html(text);
}

//hide status message
function hideStatus() {
    $("#status-message").hide();
    $("#status-message").html("");
}

//show upload progress
function showProgressPercentage(percentageValue) {
    if ($("#upload-percentage").length == 0) {
        $("#drive-box").prepend("<div id='upload-percentage' class='flash'></div>");
    }
    if (!$("#upload-percentage").is(":visible")) {
        $("#upload-percentage").show(1000);
    }
    $("#upload-percentage").html(percentageValue.toString() + "%");
}

//show error message
function showErrorMessage(errorMessage) {
    $("#error-message").html(errorMessage);
    $("#error-message").show(100);
    setTimeout(function () {
        $("#error-message").hide(100);
    }, 3000);
}

//show error message
function showErrorMessageVisible(errorMessage) {
    $("#error-message").html(errorMessage);
    $("#error-message").show(100);
    setTimeout(function () {
        $("#error-message").hide(100);
    }, 20000);
}

//show status ask
function showAsk(text) {
    //$("#status-ask").show();
    //$("#status-ask").html(text);

    $("#MCAIconoEliminarArchivo").attr("class", "advertencia");
    $("#MCAContenedorEliminarArchivo").html(text);
    $("#ModalCuadroEliminarArchivo").dialog({ title: "Confirmación" });
    $("#ModalCuadroEliminarArchivo").dialog("open");
    //showLoading();

}

//cargar extensiones
function CargarExtensiones() {

    var params = {
        tokenUsuario: $('#TokenUsuario').val()
    }

    $.ajax({
        type: 'POST',
        url: '../Builder/Utilitarios/UploadFiles.aspx/CargarExtensiones',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: JSON.stringify(params),
        success: function (data) {
            //

            if (data.d.Estado = 'OK') {
                //extensiones_permitidas = new Array(".pdf", ".jpg", ".jpeg", ".png");
                extensiones_permitidas = new Array(data.d.Mensaje.split(','));
            }
            //alert(extensiones_permitidas);
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
        }
    });
}

//cargar Megas Permitidos
function CargarMegasPermitidos() {

    var params = {
        tokenUsuario: $('#TokenUsuario').val()
    }

    $.ajax({
        type: 'POST',
        url: '../Builder/Utilitarios/UploadFiles.aspx/CargarMegas',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: JSON.stringify(params),
        success: function (data) {
            //

            if (data.d.Estado = 'OK') {
                megas_permitidos = new Array(data.d.Mensaje);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
        }
    });
}

/******************** END NOTIFICATION ********************/

$(function () {
    $("#button-reload").click(function () {

        showLoading();
        showStatus("Cargando Archivos...");
        //getDriveFiles();
        ObtenerArchivos($("#HNroSolicitud").val(), true);

    });

    $("#button-upload").click(function () {

        $("#fUpload").click();
        //showLoading();
    });

    $("#fUpload").bind("change", function () {

        $("#error-message").hide();

        //$("#drive-box").show();
        //$("#drive-box").css("display", "inline-block");
        //$("#login-box").hide();

        $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
        $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');

        //showLoading();



        var totalFiles = document.getElementById("fUpload").files.length;
        var filesError = 0;
        var mierrorExtension = "<Strong>Comprueba la extensión de los archivos a subir. \nSólo se pueden subir archivos con extensiones: " + extensiones_permitidas[0].join() + "</Strong>";
        var mierrorTamanio = "<Strong>Comprueba el tamaño de los archivos. \nSólo se pueden subir archivos con " + megas_permitidos + " MB </Strong>";
        var filesErrorExtension = 0;
        var filesErrorTamanio = 0;

        for (var i = 0; i < totalFiles; i++) {

            var file = document.getElementById("fUpload").files[i];
            var fileValue = file.name;
            var fileSize = file.size;
            var siezeMegaByte = (parseInt(fileSize / 1024)) / 1024;

            var extension = (fileValue.substring(fileValue.lastIndexOf("."))).toLowerCase();
            var permitida = false;

            for (var i_extension = 0; i_extension < extensiones_permitidas[0].length; i_extension++) {
                if (extensiones_permitidas[0][i_extension].toLowerCase() == extension) {
                    permitida = true;
                    break;
                }
            }

            if (!permitida) {
                filesErrorExtension += 1;
                mierrorExtension += " <br/> " + fileValue;
            }

            if (siezeMegaByte > megas_permitidos) {
                filesErrorTamanio += 1;
                mierrorTamanio += " <br/> " + fileValue + ", tamaño: " + siezeMegaByte.toFixed(2) + " MB";
            }

        }

        if (filesErrorExtension + filesErrorTamanio > 0) {
            if (filesErrorExtension > 0) { mierrorExtension += " <br/> "; }
            showErrorMessageVisible(((filesErrorExtension == 0) ? "" : mierrorExtension) + ((filesErrorTamanio == 0) ? "" : mierrorTamanio));
        } else {

            if (totalFiles > 0) {


                showLoading();
                showStatus("Subiendo archivo en progreso...");

                for (var i = 0; i < totalFiles; i++) {

                    var file = document.getElementById("fUpload").files[i];
                    var fileValue = file.name;
                    var formData = new FormData();
                    var mierrorarchivoigual = "";
                    var cantVueltas = 1;

                    if (DRIVE_FILES != null) {
                        if (DRIVE_FILES.length > 0) {
                            for (var d = 0; d < DRIVE_FILES.length; d++) {
                                if (DRIVE_FILES[d].Name == fileValue) {
                                    mierrorarchivoigual = "Ya existe unarchivo con ese nombre " + fileValue;
                                    showErrorMessageVisible(mierrorarchivoigual)
                                }
                            }
                        }
                    }

                    formData.append("fUpload", file);
                    var ajaxRequest = $.ajax({
                        type: "POST",
                        url: "../Builder/Utilitarios/UploadFiles.aspx?idSolicitud=" + $("#HNroSolicitud").val(),
                        contentType: false,
                        processData: false,
                        data: formData
                    });

                    ajaxRequest.done(function (xhr, textStatus, data) {
                        // Do other operation
                        //alert(data.responseText);


                        if (cantVueltas == totalFiles) {
                            ObtenerArchivos($("#HNroSolicitud").val(), true);
                        }
                        else {
                            ObtenerArchivos($("#HNroSolicitud").val(), false);
                        }

                        cantVueltas = cantVueltas + 1;

                    });

                    ajaxRequest.fail(function (xhr, textStatus, errorThrown) {
                        showErrorMessageVisible("Error al subir el archivo: " + file.name)
                        ObtenerArchivos($("#HNroSolicitud").val(), true);
                        // alert("Error al subir el archivo: " + file.name);
                    });

                }

            }

        }
        //if (i == totalFiles) {

        //hideLoading();

        //if ($('#HEstado').val() == "1" || $('#HEstado').val() == "5" || $('#HEstadoPlaft').val() == "1") {
        //    $('#ModSolEnviarEvaluacion').removeAttr('disabled');
        //    $('#ModSolEnviarEvaluacion').attr('class', 'boton darkblue sharp');
        //}

        //}

        //hideLoading();
        //hideStatus();

    });

    if ($('#HEstado').val() == "1") {
        bOcultarBotonDrive = true;
    }

    if ($('#HEstado').val() == "5" || $('#HEstadoPlaft').val() == "1") {
        $('#ModSolEnviarEvaluacion').removeAttr('disabled');
        $('#ModSolEnviarEvaluacion').attr('class', 'boton darkblue sharp');
    }

    //Cargar extensiones
    CargarExtensiones();

    //Cargar Megas Permitidos
    CargarMegasPermitidos();

    $("#MCAAceptarCierre_Plaft").click(function () {
        //alert("aceptar");
        //showLoading();

        $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
        $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            idArchivo: idElminarArchivo
        }

        $.ajax({
            type: 'POST',
            url: '../Builder/Utilitarios/UploadFiles.aspx/EliminarArchivo',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: JSON.stringify(params),
            success: function (data) {


                if (data.d.Estado = 'OK') {
                    setTimeout(function () {

                        $("#button-reload").click();

                        //$('#ModSolEnviarEvaluacion').removeAttr('disabled');
                        //$('#ModSolEnviarEvaluacion').attr('class', 'boton darkblue sharp');

                    }, 1000);
                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
            }
        });

        $("#ModalCuadroEliminarArchivo").dialog("close");
        $("#drive-box-loading").html("");
        showLoading();

    });

    $("#MCACancelar_Plaft").click(function () {
        $("#ModalCuadroEliminarArchivo").dialog("close");
        $("#drive-box-loading").html("");
        //showLoading();
    });

    $("#MCAAceptarCierreEnviar_Plus").click(function () {

        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Enviando a Evaluación, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        var id_archivos = "";

        if (DRIVE_FILES != null) {
            if (DRIVE_FILES.length > 0) {
                for (var i = 0; i < DRIVE_FILES.length; i++) {
                    id_archivos += DRIVE_FILES[i].Id + ",";
                }
            }
        }

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            num_solicitud: $('#HNroSolicitud').val(),
            id_archivos: id_archivos
        }

        $.ajax({
            type: "POST",
            url: "SeleccionSolicitud.aspx/EnviarEvaluacion",
            contentType: "application/json; charset=iso-8859-1",
            data: JSON.stringify(params),
            dataType: "json",
            success: function (data) {

                $("#ModalCotizando").dialog("close");
                if (data.d.Estado == "OK") {
                    $("#HEstado").val("4");
                    $("#HEstadoPlaft").val("4");

                    $("#button-upload").hide();
                    $(".button-delete").hide();
                    $("#LabMensaje").text('Solicitud en Evaluación')

                    $("#MCMIcono").attr("class", "info");
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: "Información" });
                    $("#ModalCuadroMensaje").dialog("open");

                    $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');
                    $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');

                } else if (data.d.Estado == "ERROR") {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $("#ModalCotizando").dialog("close");
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al Enviar a Evaluación.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            }
        });

        $("#ModalCuadroEnviar").dialog("close");
        $("#drive-box-loading").html("");

    });

    $("#MCACancelarEnviar_Plus").click(function () {
        $("#ModalCuadroEnviar").dialog("close");
        $("#drive-box-loading").html("");
    });

});
