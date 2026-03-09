
$(document).ready(function () {

    $('#TablaBeneficiariosReintentar_RP').live('click', function () {
        $('#TablaBeneficiariosError_RP').hide();
        CargarTablaBeneficiarios_RP(null);
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

    $("#ModSolSaldoCIC_RP").autoNumeric();

    $(".enteroPositivo").numeric();
    $(".telefono").numeric({ allow: "#*+" });
    $(".alfanumerico").alphanumeric({ allow: "ñÑ" });
    $(".nombre").alpha({ allow: "áéíóúÁÉÍÓÚñÑäëïöüÄËÏÖÜàèìòùÀÈÌÒÙ'- " });
    $(".fecha").mask("99/99/9999");

});

function CargarTablaCotizacionesCierre_RP(cot, moneda, plan) {

    if (plan == 'PLAN1') {
        $("#TablaCotizacionesContenedor_IFP_Plan1").hide();
        $("#TablaCotizacionesCargando_RP").show();
    } else if (plan == 'PLAN2') {
        $("#TablaCotizacionesContenedor_IFP_Plan2").hide();
        $("#TablaCotizacionesCargando_RP").show();
    } else if (plan == 'PLAN3') {
        $("#TablaCotizacionesContenedor_IFP_Plan3").hide();
        $("#TablaCotizacionesCargando_RP").show();
    }

    var params = {
        cotizaciones: cot,
        moneda: moneda,
        plan: plan,
        coberturasAdicionales: ""
    }

    $.ajax({
        type: "POST",
        url: "SeleccionSolicitud.aspx/CargarTablaCotizacionesP12",
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {

            if (plan == 'PLAN1') {
                $('#TablaCotizacionesContenedor_IFP_Plan1').html($(data.d).find('#ContenidoDinamico').html());
                $('#TablaCotizacionesCargando_RP').hide();
                $('#TablaCotizacionesContenedor_IFP_Plan1').show();
            } else if (plan == 'PLAN2') {
                $('#TablaCotizacionesContenedor_IFP_Plan2').html($(data.d).find('#ContenidoDinamico').html());
                $('#TablaCotizacionesCargando_RP').hide();
                $('#TablaCotizacionesContenedor_IFP_Plan2').show();
            } else if (plan == 'PLAN3') {
                $('#TablaCotizacionesContenedor_IFP_Plan3').html($(data.d).find('#ContenidoDinamico').html());
                $('#TablaCotizacionesCargando_RP').hide();
                $('#TablaCotizacionesContenedor_IFP_Plan3').show();
            }

            if ($('#HSeleccionada').val() == "S") {

                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

                if (plan == 'PLAN1') {
                    for (i = 0; i < $('#TabCotizaciones_IFP_P1 tbody tr').length; i++) {
                        $('#TabCotizaciones_IFP_P1 tbody tr:eq(' + i + ') input[type=radio]').attr("disabled", "true");
                    }
                } else if (plan == 'PLAN2') {
                    for (i = 0; i < $('#TabCotizaciones_IFP_P2 tbody tr').length; i++) {
                        $('#TabCotizaciones_IFP_P2 tbody tr:eq(' + i + ') input[type=radio]').attr("disabled", "true");
                    }
                } else if (plan == 'PLAN3') {
                    for (i = 0; i < $('#TabCotizaciones_IFP_P3 tbody tr').length; i++) {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + i + ') input[type=radio]').attr("disabled", "true");
                    }
                }
            }

            if ($('#HEstado').val() == "1") {//Seleccionada
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            } else if ($('#HEstado').val() == "2") {//Cerrado
                botonModSolAceptarBloqueado = true;
                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            } else if ($('#HEstado').val() == "3") {//Anulado
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
            url: '../RentaIFP/Cotizador.aspx/CargarTablaBeneficiarios',
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

                    if ($('#ModSolTipoPlan_RP').val() == "02") {
                        for (i = 1; i < $('#TabBeneficiarios_RP tbody tr').length; i++) {
                            $('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').prop("checked", "")
                            $('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').attr("disabled", "true")
                        }

                    }

                    if ($('#ModSolModo').val() == "N" && $('#HCopia').val() == "") {

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

                        CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());

                    }

                }
                else {
                    $('#TablaRviBenefiCargando_RP').hide();
                    $('#TablaRviBenefiContenedor_RP').show();
                    $('#TablaRviBenefiContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());

                    $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabRviBenefi_RP tbody tr').length + ')');

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

/**
* jQuery.browser.mobile (http://detectmobilebrowser.com/)
*
* jQuery.browser.mobile will be true if the browser is a mobile device
*
**/
(function (a) { jQuery.browser.mobile = /(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4)) })(navigator.userAgent || navigator.vendor || window.opera);
