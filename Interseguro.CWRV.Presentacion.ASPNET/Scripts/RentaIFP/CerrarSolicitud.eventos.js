
/* Determinar si se trata de un dispositivo móvil */
if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
    movil = true;
}
else {
    movil = false;
}

//<SRI.INI-20322_E2>
/* SOLICITUDES */
var idSolicitud;
var Solicitud;
var idCotizacion;
var Correo;
//<SRI.FIN-20322_E2>

$(document).ready(function () {
    var permisoBusAfiExaminarSolicitud = ($('#PerBusAfiExaminarSolicitud_RP').val() == '1' ? true : false);
    var permisoBusAfiBuscar = ($('#PerBusAfiBuscar_RP').val() == '1' ? true : false);
    var permisoGuardar = ($('#PerGuardar_RP').val() == '1' ? true : false);
    var permisoNuevaDireccion = ($('#PerNuevaDireccion_RP').val() == '1' ? true : false);
    var permisoNuevoTelefono = ($('#PerNuevoTelefono_RP').val() == '1' ? true : false);
    var permisoNuevoBeneficiario = ($('#PerNuevoBeneficiario_RP').val() == '1' ? true : false);
    var permisoNuevaSolicitud = ($('#PerNuevoBeneficiario_RP').val() == '1' ? true : false);

    function sumarDias(fecha, dias) {
        fecha.setDate(fecha.getDate() + dias);
        return fecha;
    };

    function CargandoSolicitud() {

        $('#BeneficiariosOriginales_RP').show();
        $('#ManSolPestanhas li:eq(0)').trigger('click');


        $('#ManSolTipoSolicitud_RP').val($(this).parent('td').parent('tr').children().eq(3).find('span').html());

        botonModSolAceptarBloqueado = false;
        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');

        var params = {
            solicitud: $("#ModSolNroSolicitud_IFP").text()
        }
        $.ajax({
            type: 'POST',
            url: 'MantenerSolicitud.aspx/ObtenerCotizacionesBeneficiarios',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                Solicitud = data.d;

                if ($("#HBloqueo").val() == "TRUE") {
                    if ($("#ModSolFechaCotizacion_IFP").val() != $("#HFechaActual").val()) {
                        botonModSolAceptarBloqueado = true;
                        $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

                        $('#ModSolTipoCambio_IFP').attr('disabled', 'disabled');
                        $('#ModSolFechaCotizacion_IFP').attr('disabled', 'disabled');
                        $('#ModSolFechaDevengue_IFP').attr('disabled', 'disabled');
                        $('#ModSolFechaVigencia_IFP').attr('disabled', 'disabled');
                        $('#ModSolPrimaUnica_IFP').attr('disabled', 'disabled');
                        $('#ConModSolMonedaPrimaUnica_IFP').attr('disabled', 'disabled');
                        $('#ConModSolPlan_IFP').attr('disabled', 'disabled');

                        $('#ModSolPrimaUnica_IFP').attr('class', 'formTextbox formTextboxReadOnly');
                        $('#ModSolTipoCambio_IFP').attr('class', 'formTextbox formTextboxReadOnly');
                        $('#ConModSolMonedaPrimaUnica_IFP').addClass('formComboboxReadOnlyContenedor');
                        $('#ConModSolPlan_IFP').addClass('formComboboxReadOnlyContenedor');

                        $('#ModSolFechaCotizacion_IFP').attr('class', 'fecha formTextbox formCalendar formTextboxReadOnly');
                        $('#ModSolFechaDevengue_IFP').attr('class', 'fecha formTextbox formCalendar formTextboxReadOnly');
                        $('#ModSolFechaVigencia_IFP').attr('class', 'fecha formTextbox formCalendar formTextboxReadOnly');
                    } else {
                        botonModSolAceptarBloqueado = false;
                        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                    };

                } else {
                    botonModSolAceptarBloqueado = false;
                    $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                }

                var bPlan1 = false;
                var bPlan2 = false;
                var bPlan3 = false;

                $('#TabCotizacionesLeyenda_RP').hide();

                for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                    if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN1') {
                        bPlan1 = true;
                    }
                    if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN2') {
                        bPlan2 = true;
                    }
                    if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN3') {
                        bPlan3 = true;
                    }

                    if (Solicitud.Cotizaciones[i].IndCotiza == '**') {
                        $('#TabCotizacionesLeyenda_RP').show();
                    }

                }

                //S38 Beneficiarios cotizados
                CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);

                if (bPlan1) {
                    CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
                }

                if (bPlan2) {
                    CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
                }

                if (bPlan3) {
                    CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
                }

                if ($("#ModSolMonedaPrimaUnica_IFP").val() == "001") {
                    $("#ModSolTipoCambioPanel").hide();
                } else {
                    $("#ModSolTipoCambioPanel").show();
                };


            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                $('#ModalSolicitud').dialog('close');
            },
            complete: function () {

            }
        });

    }

    /* Botón Cancelar Cierre */
    $('#ModSolCancelar_RP').live('click', function () {
        window.location.href = "../RentaPrivadaPlus/ListadoCierrePlus.aspx";
    });

    //<INIGTI_7012>
    /* Botón Aceptar Cierre*/
    $("#ModSolAceptarCierre_RP").live("click", function () {

        if (!botonModSolAceptarBloqueado) {
            $("#lineaCausante").hide();
            // Seleccionar Cotización
            var idSolicitudElegida = 0;

            if ($("#TabCotizaciones_IFP_P1 input[type=radio]:checked").length > 0) {
                idSolicitudElegida = $("#TabCotizaciones_IFP_P1 input[type=radio]:checked").val();
            }

            if (idSolicitudElegida == 0) {
                if ($("#TabCotizaciones_IFP_P2 input[type=radio]:checked").length > 0) {
                    idSolicitudElegida = $("#TabCotizaciones_IFP_P2 input[type=radio]:checked").val();
                }
            }

            if (idSolicitudElegida == 0) {
                if ($("#TabCotizaciones_IFP_P3 input[type=radio]:checked").length > 0) {
                    idSolicitudElegida = $("#TabCotizaciones_IFP_P3 input[type=radio]:checked").val();
                }
            }



            if (idSolicitudElegida == 0) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html('Seleccione una <strong>Cotización</strong>. Dato Obligatorio.');
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return;
            }


            $("#MCAIcono").attr("class", "advertencia");
            $("#MCAContenedor").html("¿Deseas generar la Póliza de la cotización seleccionada?");
            $("#HCondicion").val("GENERAR");
            $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
            $("#ModalCuadroAdvertencia").dialog("open");
        }
        else {
            return false;
        }
    });
    //<FINGTI_7012>

    //<INIGTI_7012>
    /* Botón Anular Cierre*/
    $("#ModSolAnularCierre_RP").live("click", function () {

        if (!botonModSolAnularBloqueado) {
            $("#lineaCausante").show();
            $("#MCAIcono").attr("class", "advertencia");
            $("#MCAContenedor").html("¿Deseas Anular la Solicitud?");
            $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
            $("#ModalCuadroAdvertencia").dialog("open");
            $('#HCondicion').val('ANULAR');
        }
        else {
            return false;
        }
    });
    //<FINGTI_7012>

    /* CUADRO DE MENSAJE */
    /* Validaciones desde servidor */
    if ($('#MCMEstado').val() == '1') {
        if ($(document).width() >= 1000) {
            $('#MCMIcono').attr('class', $('#MCMEstadoIcono').val());
            $('#ModalCuadroMensaje').dialog({ title: $('#MCMEstadoTitulo').val() });
            $('#ModalCuadroMensaje').dialog({ position: ['center', 'center'] });
            $('#ModalCuadroMensaje').dialog('open');
        }
        else {
            $('#MensajeMovil').attr("class", $('#MCMEstadoIcono').val() + "_movil");
            $("#MensajeMovil").fadeIn();
        }
    }

    /* Botón Aceptar */
    $('#MCMAceptar').live('click', function () {
        $('#ModalCuadroMensaje').dialog('close');
    });

    /* CUADRO DE ADVERTENCIA */

    /* Botón Cancelar */
    $('#MCACancelar_Plus').live('click', function () {
        $('#ModalCuadroAdvertencia').dialog('close');
    });

    //<INIGTI_7012>
    /* Botón Aceptar Cierre*/
    $('#MCAAceptarCierre_Plus').live('click', async function () {
        if (!botonModSolAceptarBloqueado || !botonModSolAnularBloqueado) {

            if ($("#HCondicion").val() == "GENERAR") {

                $("#ModalCuadroAdvertencia").dialog("close");

                // Seleccionar Cotización
                var idSolicitudElegida = 0;

                if ($("#TabCotizaciones_IFP_P1 input[type=radio]:checked").length > 0) {
                    idSolicitudElegida = $("#TabCotizaciones_IFP_P1 input[type=radio]:checked").val();
                }

                if (idSolicitudElegida == 0) {
                    if ($("#TabCotizaciones_IFP_P2 input[type=radio]:checked").length > 0) {
                        idSolicitudElegida = $("#TabCotizaciones_IFP_P2 input[type=radio]:checked").val();
                    }
                }

                if (idSolicitudElegida == 0) {
                    if ($("#TabCotizaciones_IFP_P3 input[type=radio]:checked").length > 0) {
                        idSolicitudElegida = $("#TabCotizaciones_IFP_P3 input[type=radio]:checked").val();
                    }
                }

                //if ($("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
                //    idSolicitudElegida = $("#TabCotizaciones_RP input[type=radio]:checked").val();
                //}

                $("#ModSolCargando").fadeIn();
                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Generando Póliza, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Generando" });
                $("#ModalCotizando").dialog("open");

                try {
                    const opcionesSistema = JSON.parse(localStorage.getItem("opcionesSistema") || '[]');
                    if (opcionesSistema.some(o => o.IdAzman == EnumsPermisos.SolicitudPlusGenerarPoliza && o.Activa)) {
                var params = {
                    numCorrelativo: idSolicitudElegida
                        }

                        try {
                            $('#ModSolCargando').fadeIn();
                            const response = await ApiCotizadorIFP.GenerarPoliza(params, $("#ModSolNroSolicitud_IFP").html());
                            const data = { d: response }; // Mantenemos la estructura data.d

                            if (data.d.Estado == 'OK') {
                                // Cambiar la modal a modo de modificación
                                $('#ModSolModo').val('');
                                // Cerrar modal de espera
                                $('#ModalCotizando').dialog('close');

                                // Mostrar mensaje de éxito
                                $('#MCMIcono').attr('class', data.d.Icono);
                                $('#MCMContenedor').html(data.d.Mensaje);
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                $('#ModalCuadroMensaje').dialog('open');

                                botonModSolAceptarBloqueado = true;
                                $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                                $("#HCondicion").val("OK");

                                //setTimeout("$('#ModalCotizando').dialog('close');", 2000);
                                setTimeout(function () {
                                    window.location.href = "CerrarSolicitud.aspx";
                                }, 3000);
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

                                setTimeout(function () {
                                    window.location.href = "CerrarSolicitud.aspx";
                                }, 3000);
                            }
                            $('#ModSolCargando').fadeOut();

                        } catch (error) {
                            if (error.status === 401 || error.status === 12030) {
                                document.location.reload(true);
                            }
                            else {
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al generar la poliza de la solicitud.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#ModalCuadroMensaje').dialog('open');

                                $('#ModalCotizando').dialog('close');
                            }
                        }

                    } else {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html("Usted no tiene privilegios sobre esta opción.");
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                        $('#ModalCotizando').dialog('close');
                    }
                } catch (error) {
                    console.error('Error al validar permisos:', error);
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html("Error al validar los permisos del usuario.");
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                    $('#ModalCotizando').dialog('close');
                }
               

            };

            //Anular la Solicitud
            if ($("#HCondicion").val() == "ANULAR") {

                if ($("#ModSolCausante_RP").val() == "0") {
                    $('#MCMIcono').attr('class', 'validacion');
                    $('#MCMContenedor').html('Seleccione Causal de Anulación.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return;
                }


                $("#ModalCuadroAdvertencia").dialog("close");

                $("#ModSolCargando").fadeIn();
                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Anulando la Solicitud, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Anulando" });
                $("#ModalCotizando").dialog("open");

                try {
                    const opcionesSistema = JSON.parse(localStorage.getItem("opcionesSistema") || '[]');
                    if (opcionesSistema.some(o => o.IdAzman == EnumsPermisos.SolicitudPlusCerrar && o.Activa)) {
                     var params = {
                         codCausante: $("#ModSolCausante_RP").val()
                     }
                     try {
                         $('#ModSolCargando').fadeIn();
                         const data = await ApiCotizadorIFP.AnularPoliza(params, $("#ModSolNroSolicitud_IFP").html());
                         if (data.Estado == 'OK') {
                        $('#ModSolModo').val('');
                        $('#ModalCotizando').dialog('close');
                        $('#MCMIcono').attr('class', data.Icono);
                        $('#MCMContenedor').html(data.Mensaje);
                        $('#ModalCuadroMensaje').dialog({ title: data.Titulo });
                        $('#ModalCuadroMensaje').dialog('open');
                        botonModSolAceptarBloqueado = true;
                        $('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                        botonModSolAnularBloqueado = true;
                        $('#ModSolAnularCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                        setTimeout(function () {
                            window.location.href = "CerrarSolicitud.aspx";
                        }, 1000);
                    }
                         else if (data.Estado == 'TOKEN') {
                        CerrarSesionExpirada();
                    }
                         else {
                        $('#ModalCotizando').dialog('close');
                        $('#ModalCuadroMensaje').dialog({ title: data.Titulo });
                        $('#MCMIcono').attr('class', data.Icono);
                        $('#MCMContenedor').html(data.Mensaje);
                        $('#ModalCuadroMensaje').dialog('open');
                        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                    }
                     } catch (error) {
                         if (error.status == 401 || error.status == 12030) {
                        document.location.reload(true);
                    }
                         else {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html('Ha ocurrido un error al anular la información de la solicitud.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                        $('#ModalCotizando').dialog('close');
                    }
                     } finally {
                         $('#ModSolCargando').fadeOut();
                     }
                } else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html("Usted no tiene privilegios sobre esta opción.");
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                    $('#ModalCotizando').dialog('close');
                }
            } catch (error) {
                console.error('Error al validar permisos:', error);
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html("Error al validar los permisos del usuario.");
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
                $('#ModalCotizando').dialog('close');
            }
               
            };


        }
        else {
            return false;
        }
    });

    //<FINGTI_7012>

    /* Botón Aceptar */
    $('#MCSCancelar').live('click', function () {
        $('#ModalCerrarSesion').dialog('close');
    });

    $("#MCAOCancelar").live("click", function () {
        $("#ModalCuadroAdvertencia").dialog("close");
    });

    if ($('#ModSolModo').val() == "M" || $('#ModSolModo').val() == "C" || $('#ModSolModo').val() == "CONS"
        || $('#ModSolModo').val() == "CERRAR") {
        CargandoSolicitud();
    };

    /* Botón Modificar */
    $('#TabRviBenefi_RP .grilla_editar').live('click', function () {
        // Bloquear pantalla
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Cargando la información del beneficiario, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Cargando" });
        $("#ModalCotizando").dialog("open");

        idGrupoFamiliar = $(this).data('grupofamiliar');
        $.ajax({
            type: 'POST',
            url: '../RentaIFP/GrupoFamiliarAfiliado.aspx/SessionIdGrupoFamiliar',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            //data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "', paginaLlamada:'../RentaIFP/CerrarSolicitud.aspx#beneficiarios'}",
            data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "', paginaLlamada:'../RentaPrivadaPlus/ListadoCierrePlus.aspx'}",

            success: function (data) {

                var grupofamiliar = data.d;
                if (idGrupoFamiliar.toString() == grupofamiliar.toString()) {
                    window.location.href = "../RentaIFP/GrupoFamiliarAfiliado.aspx";
                }

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

    if (window.location.hash == "#beneficiarios") {
        $('#ManSolPestanhas li').attr('class', '');
        $('#ManSolPestanha1').hide();
        $('#ManSolPestanha2').show();
        $("#ManSolPes2").attr('class', 'seleccionado');
    }

});

//<INI.GTI_7012_3>
function MostrarModal(mensaje) {

    $('#ModalCuadroMensaje').dialog({
        autoOpen: false,
        resizable: false,
        width: 500,
        minHeight: 130,
        show: "fade",
        hide: "explode",
        modal: true
    });

    $('#MCMIcono').attr('class', 'validacion');
    $('#MCMContenedor').html(mensaje);
    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
    $('#ModalCuadroMensaje').dialog('open');
};
//<FIN.GTI_7012_3>

function formatearError(errores) {
    var cadena = '';
    for (var i = 0; i < errores.length; i++) {
        cadena += '<div style="margin:5px 0">' + errores[i] + '</div>';
    }
    return cadena;
}

function formatearMonto(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}

function formatearMonto2(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '.00';
    //var rgx = /(\d+)(\d{3})/;
    //while (rgx.test(x1)) {
    //    x1 = x1.replace(rgx, '$1' + '' + '$2');
    //}
    return x1 + x2;
}

//<INIGTI_753>
function ObtenerMonedaAjuste(codMoneda, objeto, fil) {
    $.ajax({
        type: 'POST',
        url: 'MantenerSolicitud.aspx/ObtenerMonedaAjuste',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: "{codMoneda:'" + codMoneda + "'}",
        success: function (data) {
            var objdata = data.d;
            objeto.empty();
            for (var n = 0; n < objdata.length; n++) {
                objeto[0].options[n] = new Option(objdata[n].Valor_1, objdata[n].Valor_2);
            };


            if ($('#ModSolModo').val() == "CONS") {
                objeto.attr('disabled', 'disabled');
            }
            else if ($('#ModSolModo').val() == "N") {
                objeto.removeAttr('disabled', 'disabled');
            }
            else {
                if ($("#HBloqueo").val() == "TRUE") {
                    objeto.attr('disabled', 'disabled');
                } else {
                    objeto.removeAttr('disabled', 'disabled');
                }

            }



            if (objdata[0].Valor_1 == "-") {
                objeto.attr('disabled', 'disabled');
                Solicitud.Cotizaciones[fil].ValMonAju = "-1";
                objeto.val("-1");
                //fila.find('.ModSolAjusteMoneda').removeAttr('disabled', 'disabled');
                //if (fila.find('.ModSolAjusteMoneda').val() == "-") {
                //    fila.find('.ModSolAjusteMoneda').attr('disabled', 'disabled');
                //}
            } else {
                if (Solicitud.Cotizaciones[fil].ValMonAju == "-1") {
                    Solicitud.Cotizaciones[fil].ValMonAju = objdata[0].Valor_2;
                    objeto.val(Solicitud.Cotizaciones[fil].ValMonAju);
                } else {
                    objeto.val(Solicitud.Cotizaciones[fil].ValMonAju);
                }
            }

            // Actualizar temporizador
            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
            $clock.countdown(selectedDate.toString());
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                document.location.reload(true);
            }
            else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de periodo garantizado.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
    });
}
//<FINGTI_753>
