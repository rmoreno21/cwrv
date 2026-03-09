/* Determinar si se trata de un dispositivo móvil */
if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
    movil = true;
}
else {
    movil = false;
}

/* SOLICITUDES */
var idSolicitud;
var Solicitud;
var idCotizacion;
var Correo;
var Plan = [];

var ModGruFamBotonesInactivos = false;

var nombrePersonaVinculada = "";

$(document).ready(function () {


    $('#ModGruFamPorcentaje_RP').autoNumeric({ vMin: '0', vMax: '100' });
    $('.numerico').autoNumeric('init', { aSep: ',', aDec: '.' });

    /* Limpiar Formulario*/
    function LimpiarFormularioGrupoFamiliar() {
        $('#ModGruFamApellidoPaterno_RP').removeClass('formTextboxError');
        $('#ModGruFamApellidoMaterno_RP').removeClass('formTextboxError');
        $('#ModGruFamNombres_RP').removeClass('formTextboxError');
        $('#ConModGruFamTipoIdentificacion_RP').removeClass('formComboboxErrorContenedor');
        $('#ModGruFamNumeroIdentificacion_RP').removeClass('formTextboxError');
        $('#ConModGruFamParentesco_RP').removeClass('formComboboxErrorContenedor formComboboxReadOnlyContenedor');
        $('#ConModGruFamSexo_RP').removeClass('formComboboxErrorContenedor formComboboxReadOnlyContenedor');
        $('#ModGruFamFechaNacimiento_RP').removeClass('formTextboxError formCalendarError');
        $('#ConModGruFamIndInvalidez_RP').removeClass('formComboboxErrorContenedor');
        $('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxErrorContenedor');
        $('#ModGruFamFechaInvalidez_RP').removeClass('formTextboxError formCalendarError');
        $('#ModGruFamNumeroBanco_RP').removeClass('formTextboxError');

        $('#ModGruFamApellidoPaterno_RP').val('');
        $('#ModGruFamApellidoMaterno_RP').val('');
        $('#ModGruFamNombres_RP').val('');
        $('#ModGruFamTipoIdentificacion_RP').val('0');
        $('#TexModGruFamTipoIdentificacion_RP').html($('#ModGruFamTipoIdentificacion_RP').find(':selected').text());
        $('#ModGruFamNumeroIdentificacion_RP').val('');
        $('#ModGruFamParentesco_RP').removeAttr('disabled');
        $('#ModGruFamParentesco_RP').val('0');
        $('#TexModGruFamParentesco_RP').html($('#ModGruFamParentesco_RP').find(':selected').text());
        $('#ModGruFamSexo_RP').removeAttr('disabled');
        $('#ModGruFamSexo_RP').val('0');
        $('#TexModGruFamSexo_RP').html($('#ModGruFamSexo_RP').find(':selected').text());
        $('#ModGruFamFechaNacimiento_RP').val('');
        $('#ModGruFamIndInvalidez_RP').val('0');
        $('#TexModGruFamIndInvalidez_RP').html($('#ModGruFamIndInvalidez_RP').find(':selected').text());
        $('#ModGruFamTipoInvalidez_RP').val('0');
        $('#TexModGruFamTipoInvalidez_RP').html($('#ModGruFamTipoInvalidez_RP').find(':selected').text());
        $('#ModGruFamTipoInvalidez_RP').attr('disabled', 'disabled');
        $('#ConModGruFamTipoInvalidez_RP').addClass('formComboboxReadOnlyContenedor');
        $('#ModGruFamFechaInvalidez_RP').val('');

        $('#ModGruFamNacional_RP').val('0');
        $('#TexModGruFamNacional_RP').html($('#ModGruFamNacional_RP').find(':selected').text());
        $('#ModGruFamProfesion_RP').val('0');
        $('#TexModGruFamProfesion_RP').html($('#ModGruFamProfesion_RP').find(':selected').text());
        $('#ModGruFamResidencia_RP').val('0');
        $('#TexModGruFamResidencia_RP').html($('#ModGruFamResidencia_RP').find(':selected').text());
        $('#ModGruFamPEP_RP').val('0');
        $('#TexModGruFamPEP_RP').html($('#ModGruFamPEP_RP').find(':selected').text());
        $('#ModGruFamSO_RP').val('0');
        $('#TexModGruFamSO_RP').html($('#ModGruFamSO_RP').find(':selected').text());

        $('#ModGruFamNumeroBanco_RP').val('');
        $('#ModGruFamBanco_RP').val('0');
        $('#TexModGruFamBanco_RP').html($('#ModGruFamBanco_RP').find(':selected').text());
        $('#ModGruFamTipoCtaBanco_RP').val('0');
        $('#TexModGruFamTipoCtaBanco_RP').html($('#ModGruFamTipoCtaBanco_RP').find(':selected').text());

        //$('#ModGruFamComunicacion_RP').val('0');
        $('#ModGruFamComunicacion_RP').val(1);
        $('#TexModGruFamComunicacion_RP').html($('#ModGruFamComunicacion_RP').find(':selected').text());

        $('#ModGruFamComunicacion_RP').attr('disabled', 'disabled');
        $('#ConModGruFamComunicacion_RP').addClass('formComboboxReadOnlyContenedor');

        $('#ModGruFamConfidencialidadDatos_RP').val('0');
        $('#TexModGruFamConfidencialidadDatos_RP').html($('#ModGruFamConfidencialidadDatos_RP').find(':selected').text());

        $('#ModGruFamFechaInvalidez_RP').attr('disabled', 'disabled');
        $('#ModGruFamFechaInvalidez_RP').addClass('formCalendarReadOnly');

        $('#ModGruFamPorcentaje_RP').val('0');
    }

    /* Cargando grupo familiar Cierre*/
    function CargandoGrupoFamiliarCierre() {

        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Cargando la información, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Cargando" });
        $("#ModalCotizando").dialog("open");

        $('#ModGruFamModo_RP').val('M');

        LimpiarFormularioGrupoFamiliar();

        idGrupoFamiliar = $("#ModIdGrupoFamiliar").val();

        var params = {
            idGrupoFamiliar: idGrupoFamiliar,
            hSolicitud: $('#HSolicitudSerializado').val()
        }

        $.ajax({
            type: 'POST',
            url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ObtenerDatosBenefiCierre',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                var grupo = data.d;

                //cargar pasos cierre
                var params = {
                    paso: 1,
                    objeto: $("#HSolicitudSerializado").val()
                }

                $.ajax({
                    type: 'POST',
                    url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ObtenerPasosCierre',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    //data: "{paso: '" + 1 + "',objeto: '" + $("#HSolicitudSerializado").val() + "'}",
                    data: $.toJSON(params),
                    success: function (data) {

                        if (grupo.Parentesco.Id == '80') {
                            //pasos de cierre
                            $("#pasosCierre").prepend(data.d[0]);

                            $("#pasosCierre").steps({
                                headerTag: "h2",
                                bodyTag: "section",
                                transitionEffect: "slideLeft"
                            });
                        }

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

                // Apellido Paterno
                $('#ModGruFamApellidoPaterno_RP').val(grupo.ApellidoPaterno);
                // Apellido Materno
                $('#ModGruFamApellidoMaterno_RP').val(grupo.ApellidoMaterno);
                // Nombres
                $('#ModGruFamNombres_RP').val(grupo.Nombre);
                // Tipo de Identificación
                $('#ModGruFamTipoIdentificacion_RP').val(grupo.Identificacion.IdTipo);
                $('#TexModGruFamTipoIdentificacion_RP').html($('#ModGruFamTipoIdentificacion_RP').find(':selected').text());
                // Nro. de Identificación
                if (grupo.Identificacion.IdTipo == 'D') {
                    $("#ModGruFamNumeroIdentificacion_RP").attr('maxlength', 8);
                    $('#ModGruFamNumeroIdentificacion_RP').val(grupo.Identificacion.Numero);
                }
                else {
                    $("#ModGruFamNumeroIdentificacion_RP").attr('maxlength', 9);
                    $('#ModGruFamNumeroIdentificacion_RP').val(grupo.Identificacion.Numero);
                };

                // Parentesco
                $('#ModGruFamParentesco_RP').val(grupo.Parentesco.Id);
                $('#TexModGruFamParentesco_RP').html($('#ModGruFamParentesco_RP').find(':selected').text());

                // Sexo
                $('#ModGruFamSexo_RP').val(grupo.Sexo);
                $('#TexModGruFamSexo_RP').html($('#ModGruFamSexo_RP').find(':selected').text());

                // Fecha de Nacimiento
                if (grupo.FechaNacimiento != null) {
                    $('#ModGruFamFechaNacimiento_RP').val(new Date(+grupo.FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                }

                if (grupo.Parentesco.Id == '80') {
                    $('#ModGruFamParentesco_RP').attr('disabled', 'disabled');
                    $('#ConModGruFamParentesco_RP').addClass('formComboboxReadOnlyContenedor');

                    $('#ModGruFamSexo_RP').attr('disabled', 'disabled');
                    $('#ConModGruFamSexo_RP').addClass('formComboboxReadOnlyContenedor');

                    $('#ModGruFamFechaNacimiento_RP').attr('disabled', 'disabled');
                    //$('#ConModGruFamFechaNacimiento_RP').addClass('formComboboxReadOnlyContenedor');
                    $('#ModGruFamFechaNacimiento_RP').addClass('formCalendarReadOnly');
                } else {
                    $('#ModGruFamLineaEstadocivilMail_RP').hide();
                    $('#ModGruFamLineaCentrolaboralCargo_RP').hide();
                    $('#ModGruFamLineaActividadEconomica_RP').hide();
                    $('#ModGruFamLineaMonedaingresoIngreso_RP').hide();
                    $('#ModGruFamLineaTelefonoCelular_RP').hide();

                    //cambio por llave en el cierre
                    $('#ModGruFamTipoIdentificacion_RP').attr('disabled', 'disabled');
                    $('#ConModGruFamTipoIdentificacion_RP').addClass('formComboboxReadOnlyContenedor');

                    $('#ModGruFamNumeroIdentificacion_RP').attr('disabled', 'disabled');
                    $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxReadOnly');

                    if (grupo.IndBloquearCampos) {
                        $('#ModGruFamParentesco_RP').attr('disabled', 'disabled');
                        $('#ConModGruFamParentesco_RP').addClass('formComboboxReadOnlyContenedor');

                        $('#ModGruFamSexo_RP').attr('disabled', 'disabled');
                        $('#ConModGruFamSexo_RP').addClass('formComboboxReadOnlyContenedor');

                        $('#ModGruFamFechaNacimiento_RP').attr('disabled', 'disabled');
                        $('#ModGruFamFechaNacimiento_RP').addClass('formCalendarReadOnly');

                        if ($('#HTipoPeriodoBeneficiario').val() == 0)
                        {
                            //$('#ModGruFamPorcentaje_RP').attr('disabled', 'disabled');
                            //$('#ModGruFamPorcentaje_RP').addClass('formTextboxReadOnly');
                            $('#ModGruFamLineaPorcentaje_RP').hide();
                        }

                    }
                }

                // Indicador de Invalidez
                $('#ModGruFamIndInvalidez_RP').val(grupo.Invalido ? 'S' : 'N');
                $('#TexModGruFamIndInvalidez_RP').html($('#ModGruFamIndInvalidez_RP').find(':selected').text());
                $('#ModGruFamIndInvalidez_RP').attr('disabled', 'disabled');
                $('#ConModGruFamIndInvalidez_RP').addClass('formComboboxReadOnlyContenedor');

                if (grupo.Invalido) {
                    // Tipo de Invalidez
                    $('#ModGruFamTipoInvalidez_RP').val(grupo.TipoInvalidez.Id);
                    $('#TexModGruFamTipoInvalidez_RP').html($('#ModGruFamTipoInvalidez_RP').find(':selected').text());
                    $('#ModGruFamTipoInvalidez_RP').removeAttr('disabled');
                    $('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxReadOnlyContenedor');
                    // Fecha de Invalidez
                    if (grupo.FechaInvalidez != null)
                        $('#ModGruFamFechaInvalidez_RP').val(new Date(+grupo.FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                    $('#ModGruFamFechaInvalidez_RP').removeAttr('disabled');
                    $('#ModGruFamFechaInvalidez_RP').removeClass('formCalendarReadOnly');
                }
                else {
                    // Tipo de Invalidez
                    $('#ModGruFamTipoInvalidez_RP').val('N');
                    $('#TexModGruFamTipoInvalidez_RP').html($('#ModGruFamTipoInvalidez_RP').find(':selected').text());
                    $('#ModGruFamTipoInvalidez_RP').attr('disabled', 'disabled');
                    // Fecha de Invalidez
                    $('#ModGruFamFechaInvalidez_RP').val('');
                    $('#ModGruFamFechaInvalidez_RP').attr('disabled', 'disabled');
                }

                $('#ModGruFamTipoInvalidez_RP').attr('disabled', 'disabled');
                $('#ConModGruFamTipoInvalidez_RP').addClass('formComboboxReadOnlyContenedor');
                $('#ModGruFamFechaInvalidez_RP').attr('disabled', 'disabled');
                $('#ConModGruFamFechaInvalidez_RP').addClass('formComboboxReadOnlyContenedor');

                //<INIGTI_7012>
                //Residencia
                $('#ModGruFamResidencia_RP').val(grupo.Residencia.cod_parametro);
                $('#TexModGruFamResidencia_RP').html($('#ModGruFamResidencia_RP').find(':selected').text());
                //Profesion
                $('#ModGruFamProfesion_RP').val(grupo.Profesion.cod_parametro);
                $('#TexModGruFamProfesion_RP').html($('#ModGruFamProfesion_RP').find(':selected').text());
                //Nacionalidad
                $('#ModGruFamNacional_RP').val(grupo.Nacionalidad.cod_parametro);
                $('#TexModGruFamNacional_RP').html($('#ModGruFamNacional_RP').find(':selected').text());
                //PEP
                $('#ModGruFamPEP_RP').val(grupo.ind_PEP ? 'S' : 'N');
                $('#TexModGruFamPEP_RP').html($('#ModGruFamPEP_RP').find(':selected').text());

                if (grupo.ind_PEP) {
                    $('#seccionPEP').show();
                }

                //SujetoObligado
                $('#ModGruFamSO_RP').val(grupo.ind_SujetoObligado ? 'S' : 'N');
                $('#TexModGruFamSO_RP').html($('#ModGruFamSO_RP').find(':selected').text());

                $('#ModGruFamPorcentaje_RP').val(grupo.ValPjeRenta);

                if ($('#ModGruFamParentesco_RP').val() == '80') {

                    //Comunicacion
                    //$('#ModGruFamComunicacion_RP').val(grupo.Comunicacion.Id);
                    $('#ModGruFamComunicacion_RP').val(1);
                    $('#TexModGruFamComunicacion_RP').html($('#ModGruFamComunicacion_RP').find(':selected').text());

                    $('#ModGruFamComunicacion_RP').attr('disabled', 'disabled');
                    $('#ConModGruFamComunicacion_RP').addClass('formComboboxReadOnlyContenedor');

                    //Numero Banco
                    $('#ModGruFamNumeroBanco_RP').val(grupo.NumeroBanco);

                    //Banco
                    $('#ModGruFamBanco_RP').val(grupo.Banco.Id);
                    $('#TexModGruFamBanco_RP').html($('#ModGruFamBanco_RP').find(':selected').text());

                    $.ajax({
                        type: 'POST',
                        url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: "{banco: '" + $("#ModGruFamBanco_RP").val() + "', id: ''}",
                        success: function (data) {
                            var items = "";
                            for (var posicion = 0; posicion < data.d.length; posicion++) {
                                items += "<option value='" + data.d[posicion].Id + "'>" + data.d[posicion].Nombre + "</option>";

                                if (grupo.TipoCtaBanco.Id == data.d[posicion].Id) {
                                    $('#ModGruFamNumeroBanco_RP').mask(data.d[posicion].Valor_2);
                                    $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                                }

                            }
                            var header = '<option value=\'00\'>«Seleccione»</option>';
                            $('#ModGruFamTipoCtaBanco_RP').html(header + items);
                            $('#TexModGruFamTipoCtaBanco_RP').html($('#ModGruFamTipoCtaBanco_RP option:eq(0)').text());

                            //Tipo Banco
                            $('#ModGruFamTipoCtaBanco_RP').val(grupo.TipoCtaBanco.Id);
                            $('#TexModGruFamTipoCtaBanco_RP').html($('#ModGruFamTipoCtaBanco_RP').find(':selected').text());

                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                document.location.reload(true);
                            }
                            else {
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al cargar el formato de las cuentas de los Bancos.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#ModalCuadroMensaje').dialog('open');
                            }
                        }
                    });

                    //Confidencialidaddatos
                    $('#ModGruFamConfidencialidadDatos_RP').val(grupo.Confidencialidaddatos.Id);
                    $('#TexModGruFamConfidencialidadDatos_RP').html($('#ModGruFamConfidencialidadDatos_RP').find(':selected').text());

                    $('#ModGruFamPorcentaje_RP').val("100.00");

                    $('#ModGruFamLineaPorcentaje_RP').hide();

                    if (grupo.Parentesco.Id == '80') {

                        $('#ModGruFamEstadoCivil_RP').val(grupo.estadoCivil);
                        $('#TexModGruFamEstadoCivil_RP').html($('#ModGruFamEstadoCivil_RP').find(':selected').text());

                        $('#ModGruFamMail_RP').val(grupo.CorreoElectronico);
                        $('#ModGruFamCentrolaboral_RP').val(grupo.centroLaboral);
                        $('#ModGruFamCargo_RP').val(grupo.cargo);
                        $('#ModGruFamActividadeconomica_RP').val(grupo.actividadEconomica);

                        $('#ModGruFamMonedaingreso_RP').val(grupo.monedaIngreso.Id);
                        $('#TexModGruFamMonedaingreso_RP').html($('#ModGruFamMonedaingreso_RP').find(':selected').text());

                        $('#ModGruFamIngreso_RP').autoNumeric('init', { aSep: ',', aDec: '.' });
                        $('#ModGruFamIngreso_RP').val(grupo.ingresoNeto);

                        $('#ModGruFamTelefono_RP').val(grupo.telefono);
                        $('#ModGruFamCelular_RP').val(grupo.celular);

                        if (grupo.OrigenFondo != null) {
                            if (grupo.OrigenFondo.declaracionJurada.length > 0) {
                                $('#ModGruFamDestinoFondos_RP').val(grupo.OrigenFondo.declaracionJurada);
                            }
                        }

                        CargarTablaPersonasVinculadas_RP($('#ModGruFamsolicitud').val())
                    }
                }
                else {
                    $('#ModGruFamModo_RP').val('NC');

                    //$('#ModGruFamComunicacion_RP').val(grupo.Comunicacion.Id);
                    $('#ModGruFamComunicacion_RP').val(1);
                    $('#TexModGruFamComunicacion_RP').html($('#ModGruFamComunicacion_RP').find(':selected').text());

                    $('#ModGruFamComunicacion_RP').attr('disabled', 'disabled');
                    $('#ConModGruFamComunicacion_RP').addClass('formComboboxReadOnlyContenedor');

                    $('#ModGruFamNumeroBanco_RP').val(grupo.NumeroBanco);
                    $('#ModGruFamBanco_RP').val(grupo.Banco.Id);

                    $.ajax({
                        type: 'POST',
                        url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: "{banco: '" + $("#ModGruFamBanco_RP").val() + "', id: ''}",
                        success: function (data) {
                            var items = "";
                            for (var posicion = 0; posicion < data.d.length; posicion++) {
                                items += "<option value='" + data.d[posicion].Id + "'>" + data.d[posicion].Nombre + "</option>";
                            }
                            var header = '<option value=\'00\'>«Seleccione»</option>';
                            $('#ModGruFamTipoCtaBanco_RP').html(header + items);
                            //$('#TexModGruFamTipoCtaBanco_RP').html($('#ModGruFamTipoCtaBanco_RP option:eq(0)').text());

                            //Tipo Banco
                            $('#ModGruFamTipoCtaBanco_RP').val(grupo.TipoCtaBanco.Id);
                            //$('#TexModGruFamTipoCtaBanco_RP').html($('#ModGruFamTipoCtaBanco_RP').find(':selected').text());

                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                document.location.reload(true);
                            }
                            else {
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al cargar el formato de las cuentas de los Bancos.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#ModalCuadroMensaje').dialog('open');
                            }
                        }
                    });

                    $('#ModGruFamLineaBanco_RP').hide();
                    $('#ModGruFamLineaNroBanco_RP').hide();
                    $('#ModGruFamLineaComunicacion_RP').hide();

                    $('#ModGruFamLineaInvalidez_RP').hide();
                    $('#ModGruFamLineaFechaInvalidez_RP').hide();

                    $('#seccionPEP').hide();
                    $('#seccionOrigenFondo').hide();
                    $('#Contenido_LabModGruFamEstadoCivil_RP').hide();
                    $('#ConModGruFamEstadoCivil_RP').hide();
                }

                if ($('#ModGruFamParentesco_RP').val() != '80') {
                    //$('#pasos').append("<br/>Beneficiario " + $('#ModGruFamCantidad').val() + " de " + $('#ModGruFamTotal').val())
                }

                if ($('#ModGruFamCantidad').val() == 1) {
                    $('#ModGruFamCancelarCierre_RP').hide();
                }

                //MANEJO DE TEXTO EN BOTON CONFIRMAR
                if (grupo.Parentesco.Id == '80') {
                    if ($('#ModGruPlan').val() == 'PLAN1') {
                        $('#ModGruFamAceptarCierre_RP').html('Enviar al cliente');
                        $('#ModVistaPrevia').show();
                    } else if ($('#ModGruPlan').val() == 'PLAN2') {
                        $('#ModGruFamAceptarCierre_RP').html('Siguiente');
                        $('#ModVistaPrevia').hide();
                    } else {
                        //if ($('#ModGruFamCantidad').val() == 1) {
                        //    $('#ModGruFamAceptarCierre_RP').html('Enviar al cliente');
                        //    $('#ModVistaPrevia').show();
                        //} else {
                        $('#ModGruFamAceptarCierre_RP').html('Siguiente');
                        $('#ModVistaPrevia').hide();
                        //}
                    }
                }
                else {
                    $('#ModGruFamAceptarCierre_RP').html('Guardar');
                    $('#ModVistaPrevia').hide();
                }

                //MANEJO DE CAMPO % PARA BENEFICIARIO CA
                if (grupo.TipoCobertura == 'CA') {
                    $('#ModGruFamLineaPorcentaje_RP').hide();
                    $('#ModGruFamParentesco_RP').attr('disabled', 'disabled');
                    $('#ModGruFamSexo_RP').attr('disabled', 'disabled');
                    $('#ModGruFamFechaNacimiento_RP').attr('disabled', 'disabled');
                }

                //if ($('#ModGruFamCantidad').val() == $('#ModGruFamTotal').val()) {
                //    if ($('#ModGruPlan').val() == 'PLAN1') {
                //        $('#ModGruFamAceptarCierre_RP').html('Cerrar Solicitud');
                //    } else if ($('#ModGruPlan').val() == 'PLAN2') {
                //        $('#ModGruFamAceptarCierre_RP').html('Siguiente');
                //    }
                //} else if ($('#ModGruPlan').val() == 'PLAN1') {
                //    $('#ModGruFamAceptarCierre_RP').html('Cerrar Solicitud');
                //}

                $('#ModGruFamApellidoPaterno_RP').focus();

                $('#ModGruFamCancelarCierre_RP').removeClass('botonDeshabilitado gris gris_sharp');
                $('#ModGruFamAceptarCierre_RP').removeClass('botonDeshabilitado gris gris_sharp');

                $('#ModGruFamCancelarCierre_RP').addClass('boton darkblue sharp');
                $('#ModGruFamAceptarCierre_RP').addClass('boton darkblue sharp');

                $("#ModalCotizando").dialog("close");
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
                $('#ModalGrupoFamiliar_RP').dialog('close');

                $("#ModalCotizando").dialog("close");
            }
        });
    }

    /*Cerrar Solicitud*/
    //function CerrarSolicitud() {

    //    var params = {
    //        tokenUsuario: $("#TokenUsuario").val(),
    //        num_solicitud: $('#ModGruFamsolicitud').val(),
    //        num_correlativo: $('#ModGruNumCorrelativo').val(),
    //        objSolicitud: JSON.parse($('#HSolicitudSerializado').val())
    //    }

    //    $.ajax({
    //        type: 'POST',
    //        url: 'SeleccionSolicitud.aspx/CerrarCotizacionIFP',
    //        contentType: "application/json; charset=iso-8859-1",
    //        dataType: 'json',
    //        data: $.toJSON(params),
    //        success: function (data) {
    //            if (data.d.Estado == 'OK') {
    //                // Cambiar la modal a modo de modificación
    //                $('#ModSolModo').val('');
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

    /* Botón Aceptar Cierre*/
    $('#ModGruFamAceptarCierre_RP').live('click', async function () {
        if ($('#ModGruFamAceptarCierre_RP').hasClass('boton darkblue sharp')) {

            if (!ModGruFamBotonesInactivos) {
                var esCorrecto = true;
                var errores = new Array();

                $('#ModGruFamApellidoPaterno_RP').removeClass('formTextboxError');
                $('#ModGruFamApellidoMaterno_RP').removeClass('formTextboxError');
                $('#ModGruFamNombres_RP').removeClass('formTextboxError');
                $('#ConModGruFamTipoIdentificacion_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamNumeroIdentificacion_RP').removeClass('formTextboxError');
                $('#ConModGruFamParentesco_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamSexo_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamFechaNacimiento_RP').removeClass('formTextboxError formCalendarError');
                //$('#ConModGruFamIndInvalidez_RP').removeClass('formComboboxErrorContenedor');
                //$('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxErrorContenedor');
                //$('#ModGruFamFechaInvalidez_RP').removeClass('formTextboxError formCalendarError');
                $('#ConModGruFamNacional_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamProfesion_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamResidencia_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamPEP_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamSO_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamBanco_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamTipoCtaBanco_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamComunicacion_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamNumeroBanco_RP').removeClass('formTextboxError');
                $('#ModGruFamPorcentaje_RP').removeClass('formTextboxError');
                $('#ModGruFamMail_RP').removeClass('formTextboxError');

                $('#ModGruFamEstadoCivil_RP').removeClass('formTextboxError');
                //$('#ModGruFamTelefono_RP').removeClass('formTextboxError');
                $('#ModGruFamCelular_RP').removeClass('formTextboxError');
                $('#ModGruFamCentrolaboral_RP').removeClass('formTextboxError');
                $('#ModGruFamCargo_RP').removeClass('formTextboxError');
                $('#ModGruFamActividadeconomica_RP').removeClass('formTextboxError');
                $('#ModGruFamMonedaingreso_RP').removeClass('formTextboxError');
                $('#ModGruFamIngreso_RP').removeClass('formTextboxError');
                $('#ModGruFamDestinoFondos_RP').removeClass('formTextboxError');

                //$('#ModGruFamConfidencialidadDatos_RP').removeClass('formTextboxError');

                // Apellido Paterno
                var apellidoPaterno = true;
                if ($.trim($('#ModGruFamApellidoPaterno_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Apellido Paterno</strong>. Dato Obligatorio.');
                    apellidoPaterno = false;
                }

                // Apellido Materno
                var apellidoMaterno = true;
                if ($.trim($('#ModGruFamApellidoMaterno_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Apellido Materno</strong>. Dato Obligatorio.');
                    apellidoMaterno = false;
                }

                // Nombres
                var nombres = true;
                if ($.trim($('#ModGruFamNombres_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Nombre</strong>. Dato Obligatorio.');
                    nombres = false;
                }

                // Tipo de Indentificación
                var tipoIdentificacion = true;
                if ($('#ModGruFamTipoIdentificacion_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Tipo de Identifación</strong>. Dato Obligatorio.');
                    tipoIdentificacion = false;
                }

                // Número de Identificación
                var numeroIdentificacion = true;
                if ($.trim($('#ModGruFamNumeroIdentificacion_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Numero de Identificación</strong>. Dato Obligatorio.');
                    numeroIdentificacion = false;
                }

                // Número de Identificación longitud
                var numeroIdentificacionlongitud = true;
                if ($('#ModGruFamTipoIdentificacion_RP').val() == 'D') {

                    if ($.trim($('#ModGruFamNumeroIdentificacion_RP').val()).length < 8) {
                        errores.push('Ingrese minimo 8 digitos <strong>Numero de Identificación</strong>. Dato Obligatorio.');
                        numeroIdentificacionlongitud = false;
                    }
                }

                // Parentesco
                var parentesco = true;
                if ($('#ModGruFamParentesco_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Parentesco</strong>. Dato Obligatorio.');
                    parentesco = false;
                }

                // Sexo
                var sexo = true;
                if ($('#ModGruFamSexo_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Sexo</strong>. Dato Obligatorio.');
                    sexo = false;
                }

                // Fecha de Nacimiento
                var fechaNacimiento = true;
                if ($.trim($('#ModGruFamFechaNacimiento_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Fecha de Nacimiento</strong>. Dato Obligatorio.');
                    fechaNacimiento = false;
                }

                //// Invalidez
                //var invalidez = true;
                //var tipoInvalidez = true;
                //var fechaInvalidez = true;
                //if ($('#ModGruFamIndInvalidez_RP').val() == '0') {
                //    errores.push('Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.');
                //    invalidez = false;
                //}
                //else if ($('#ModGruFamIndInvalidez_RP').val() == 'N') {
                //    if ($('#ModGruFamTipoInvalidez_RP').val() != 'N') {
                //        errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                //        tipoInvalidez = false;
                //    }
                //    if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length > 0) {
                //        errores.push('El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.');
                //        fechaInvalidez = false;
                //    }
                //}
                //else if ($('#ModGruFamIndInvalidez_RP').val() == 'S') {
                //    if ($('#ModGruFamTipoInvalidez_RP').val() != 'P' && $('#ModGruFamTipoInvalidez_RP').val() != 'T') {
                //        errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                //        tipoInvalidez = false;
                //    }

                //    if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length == 0) {
                //        errores.push('Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.');
                //        fechaInvalidez = false;
                //    }
                //}

                //if ($('#ModGruFamTipoInvalidez_RP').val() == '0') {
                //    errores.push('Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.');
                //    tipoInvalidez = false;
                //}

                var nacionalidad = true;
                if ($('#ModGruFamNacional_RP').val() == '0') {
                    errores.push('Seleccioné la <strong>Nacionalidad</strong>. Dato Obligatorio.');
                    nacionalidad = false;
                }

                var profesion = true;
                if ($('#ModGruFamProfesion_RP').val() == '0') {
                    errores.push('Seleccioné la <strong>Profesión</strong>. Dato Obligatorio.');
                    profesion = false;
                }

                var residencia = true;
                if ($('#ModGruFamResidencia_RP').val() == '0') {
                    errores.push('Seleccioné la <strong>Residencia</strong>. Dato Obligatorio.');
                    residencia = false;
                }

                var pep = true;
                if ($('#ModGruFamPEP_RP').val() == '0') {
                    errores.push('Seleccioné si <strong>¿Es Personas Expuestas Políticamente (PEP)?</strong>. Dato Obligatorio.');
                    pep = false;
                }

                var sujetoObligado = true;
                if ($('#ModGruFamSO_RP').val() == '0') {
                    errores.push('Seleccioné si <strong>¿Es Sujeto Obligado?</strong>. Dato Obligatorio.');
                    sujetoObligado = false;
                }

                var Banco = true;
                var TipoBanco = true;
                var Comunicacion = true;
                var NumeroBanco = true;
                var Mail = true;

                var estadoCivil = true;
                var centrolaboral = true;
                var cargo = true;
                var actividadeconomica = true;
                var monedaingreso = true;
                var ingreso = true;
                //var telefono = true;
                var celular = true;
                var origenFondo = true;

                if ($('#ModGruFamParentesco_RP').val() == '80') {

                    if ($('#ModGruFamBanco_RP').val() == '0') {
                        errores.push('Seleccioné el <strong>Banco</strong>. Dato Obligatorio.');
                        Banco = false;
                    }

                    if ($('#ModGruFamTipoCtaBanco_RP').val() == '0') {
                        errores.push('Seleccioné el <strong>Tipo de Cuenta</strong>. Dato Obligatorio.');
                        TipoBanco = false;
                    }

                    if ($('#ModGruFamComunicacion_RP').val() == '0') {
                        errores.push('Seleccioné el <strong>Mecanismo de comunicación</strong>. Dato Obligatorio.');
                        Comunicacion = false;
                    }

                    if ($.trim($('#ModGruFamNumeroBanco_RP').val()).length == 0) {
                        errores.push('Ingrese el <strong>Nro. de cuenta</strong>. Dato Obligatorio.');
                        NumeroBanco = false;
                    } else {
                        if ($('#ModGruFamBanco_RP').val() == '0') {
                            if (Banco) {
                                errores.push('Seleccioné el <strong>Banco</strong>. Dato Obligatorio.');
                                Banco = false;
                            }
                        }
                        if ($('#ModGruFamTipoCtaBanco_RP').val() == '0') {
                            if (TipoBanco) {
                                errores.push('Seleccioné el <strong>Tipo de Cuenta</strong>. Dato Obligatorio.');
                                TipoBanco = false;
                            }
                        }
                    }

                    if (!isEmail($('#ModGruFamMail_RP').val())) {
                        errores.push('<strong>Correo Electrónico</strong> inválido. Dato Obligatorio.');
                        Mail = false;
                    }

                    if ($('#ModGruFamEstadoCivil_RP').val() == '0') {
                        errores.push('Seleccioné el campo <strong>Estado Civil</strong>. Dato Obligatorio.');
                        estadoCivil = false;
                    }

                    if ($.trim($('#ModGruFamCentrolaboral_RP').val()).length == 0) {
                        errores.push('Ingrese el campo <strong>Centro Laboral</strong>. Dato Obligatorio.');
                        centrolaboral = false;
                    }

                    if ($.trim($('#ModGruFamCargo_RP').val()).length == 0) {
                        errores.push('Ingrese el campo <strong>Cargo</strong>. Dato Obligatorio.');
                        cargo = false;
                    }

                    if ($.trim($('#ModGruFamActividadeconomica_RP').val()).length == 0) {
                        errores.push('Ingrese el campo <strong>Actividad Económica</strong>. Dato Obligatorio.');
                        actividadeconomica = false;
                    }

                    if ($('#ModGruFamMonedaingreso_RP').val() == '0') {
                        errores.push('Seleccioné el campo <strong>Moneda de Ingreso</strong>. Dato Obligatorio.');
                        monedaingreso = false;
                    }

                    if ($.trim($('#ModGruFamIngreso_RP').val()).length == 0) {
                        errores.push('Ingrese el campo <strong>Ingreso Neto</strong>. Dato Obligatorio.');
                        ingreso = false;
                    }

                    //if ($.trim($('#ModGruFamTelefono_RP').val()).length == 0) {
                    //    errores.push('Ingrese el campo <strong>Teléfono</strong>. Dato Obligatorio.');
                    //    telefono = false;
                    //}

                    if ($.trim($('#ModGruFamCelular_RP').val()).length == 0) {
                        errores.push('Ingrese el campo <strong>Celular</strong>. Dato Obligatorio.');
                        celular = false;
                    }

                    if ($.trim($('#ModGruFamDestinoFondos_RP').val()).length == 0) {
                        errores.push('Ingrese el campo <strong>Descripción jurada de origen y/o destino de fondos</strong>. Dato Obligatorio.');
                        origenFondo = false;
                    }
                }

                var beneficiarioCA = false;
                if ($('#HSolicitudSerializado') != undefined) {
                    var lstBeneficiarios = JSON.parse($('#HSolicitudSerializado').val()).Beneficiarios;
                    for (var b in lstBeneficiarios) {
                        if (lstBeneficiarios[b].Id == $('#ModIdGrupoFamiliar').val()) {
                            if (lstBeneficiarios[b].TipoCobertura == 'CA') beneficiarioCA = true;
                        }
                    }
                }

                var porcentajeRenta = true;
                /*if (!beneficiarioCA) {
                    if ($('#ModGruFamPorcentaje_RP').val() == '0' || $('#ModGruFamPorcentaje_RP').val() == '') {
                        errores.push('Ingrese el campo <strong>Porcentaje (%)</strong>. Dato Obligatorio.');
                        porcentajeRenta = false;
                    }
                }*/

                if (!apellidoPaterno) $('#ModGruFamApellidoPaterno_RP').addClass('formTextboxError');
                if (!apellidoMaterno) $('#ModGruFamApellidoMaterno_RP').addClass('formTextboxError');
                if (!nombres) $('#ModGruFamNombres_RP').addClass('formTextboxError');
                if (!tipoIdentificacion) $('#ConModGruFamTipoIdentificacion_RP').addClass('formComboboxErrorContenedor');
                if (!numeroIdentificacion) $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxError');
                if (!numeroIdentificacionlongitud) $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxError'); //<GTI_7012_30>
                if (!parentesco) $('#ConModGruFamParentesco_RP').addClass('formComboboxErrorContenedor');
                if (!sexo) $('#ConModGruFamSexo_RP').addClass('formComboboxErrorContenedor');
                if (!fechaNacimiento) $('#ModGruFamFechaNacimiento_RP').addClass('formTextboxError formCalendarError');
                //if (!invalidez) $('#ConModGruFamIndInvalidez_RP').addClass('formComboboxErrorContenedor');
                //if (!tipoInvalidez) $('#ConModGruFamTipoInvalidez_RP').addClass('formComboboxErrorContenedor');
                //if (!fechaInvalidez) $('#ModGruFamFechaInvalidez_RP').addClass('formTextboxError formCalendarError');
                if (!nacionalidad) $('#ConModGruFamNacional_RP').addClass('formComboboxErrorContenedor');
                if (!profesion) $('#ConModGruFamProfesion_RP').addClass('formComboboxErrorContenedor');
                if (!residencia) $('#ConModGruFamResidencia_RP').addClass('formComboboxErrorContenedor');
                if (!pep) $('#ConModGruFamPEP_RP').addClass('formComboboxErrorContenedor');
                if (!sujetoObligado) $('#ConModGruFamSO_RP').addClass('formComboboxErrorContenedor');
                if (!Banco) $('#ConModGruFamBanco_RP').addClass('formComboboxErrorContenedor');
                if (!TipoBanco) $('#ConModGruFamTipoCtaBanco_RP').addClass('formComboboxErrorContenedor');
                if (!Comunicacion) $('#ConModGruFamComunicacion_RP').addClass('formComboboxErrorContenedor');
                if (!NumeroBanco) $('#ModGruFamNumeroBanco_RP').addClass('formTextboxError');
                if (!porcentajeRenta) $('#ModGruFamPorcentaje_RP').addClass('formTextboxError');
                if (!Mail) $('#ModGruFamMail_RP').addClass('formTextboxError');

                if (!estadoCivil) $('#ConModGruFamEstadoCivil_RP').addClass('formComboboxErrorContenedor');
                if (!centrolaboral) $('#ModGruFamCentrolaboral_RP').addClass('formTextboxError');
                if (!cargo) $('#ModGruFamCargo_RP').addClass('formTextboxError');
                if (!actividadeconomica) $('#ModGruFamActividadeconomica_RP').addClass('formTextboxError');
                if (!monedaingreso) $('#ConModGruFamMonedaingreso_RP').addClass('formComboboxErrorContenedor');
                if (!ingreso) $('#ModGruFamIngreso_RP').addClass('formTextboxError');
                //if (!telefono) $('#ModGruFamTelefono_RP').addClass('formTextboxError');
                if (!celular) $('#ModGruFamCelular_RP').addClass('formTextboxError');
                if (!origenFondo) $('#ModGruFamDestinoFondos_RP').addClass('formTextboxError');

                //& invalidez & tipoInvalidez & fechaInvalidez

                esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & numeroIdentificacionlongitud
                    & parentesco & sexo & fechaNacimiento & nacionalidad & profesion & residencia & pep
                    & sujetoObligado & Banco & TipoBanco & Comunicacion & NumeroBanco & porcentajeRenta & Mail & estadoCivil & centrolaboral & cargo
                    & actividadeconomica & monedaingreso & ingreso & celular & origenFondo;

                if (!esCorrecto) {
                    $('#MCMIcono').attr('class', 'validacion');
                    $('#MCMContenedor').html(formatearError(errores));
                    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return false;
                }

                ///* Pasó las validaciones */
                $('#ModGruFamCancelarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                $('#ModGruFamAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

                if ($('#ModGruFamParentesco_RP').val() != '80') {
                    $('#ModGruFamConfidencialidadDatos_RP').val('1')
                }

                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Validando beneficiario, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Validando" });
                $("#ModalCotizando").dialog("open");

                var str_estadoCivil = "";
                if ($('#ModGruFamEstadoCivil_RP').val() == null) {
                    str_estadoCivil = "";
                } else {
                    str_estadoCivil = $('#ModGruFamEstadoCivil_RP').val();
                }

                var str_correoElectronico = "";
                if ($('#ModGruFamMail_RP').val() == null) {
                    str_correoElectronico = "";
                } else {
                    str_correoElectronico = $('#ModGruFamMail_RP').val();
                }

                var str_centrolaboral = "";
                if ($('#ModGruFamCentrolaboral_RP').val() == null) {
                    str_centrolaboral = "";
                } else {
                    str_centrolaboral = $('#ModGruFamCentrolaboral_RP').val();
                }

                var str_cargo = "";
                if ($('#ModGruFamCargo_RP').val() == null) {
                    str_cargo = "";
                } else {
                    str_cargo = $('#ModGruFamCargo_RP').val();
                }

                var str_actividadeconomica = "";
                if ($('#ModGruFamActividadeconomica_RP').val() == null) {
                    str_actividadeconomica = "";
                } else {
                    str_actividadeconomica = $('#ModGruFamActividadeconomica_RP').val();
                }

                var str_monedaingreso = "";
                if ($('#ModGruFamMonedaingreso_RP').val() == null) {
                    str_monedaingreso = "";
                } else {
                    str_monedaingreso = $('#ModGruFamMonedaingreso_RP').val();
                }

                var str_ingreso = "";
                if ($('#ModGruFamIngreso_RP').val() == null) {
                    str_ingreso = "";
                } else {
                    str_ingreso = $('#ModGruFamIngreso_RP').val();
                }

                var str_telefono = "";
                if ($('#ModGruFamTelefono_RP').val() == null) {
                    str_telefono = "";
                } else {
                    str_telefono = $('#ModGruFamTelefono_RP').val();
                }

                var str_celular = "";
                if ($('#ModGruFamCelular_RP').val() == null) {
                    str_celular = "";
                } else {
                    str_celular = $('#ModGruFamCelular_RP').val();
                }

                var str_origen_fondo = "";
                if ($('#ModGruFamDestinoFondos_RP').val() == null) {
                    str_origen_fondo = "";
                } else {
                    str_origen_fondo = $('#ModGruFamDestinoFondos_RP').val();
                }

                /* var postUrl = '';
                var params = ''; */

                const solicitud = JSON.parse($('#HSolicitudSerializado').val());
                console.log({ solicitud })
                const session = UtilitariosManager.ObtenerSession();
                try {
                    let data = null;
                    if ($('#ModGruFamParentesco_RP').val() == '80') {
                        const body = {
                            idGrupoFamiliar: $('#ModIdGrupoFamiliar').val(),
                            cuspp: $('#HCUSPP_RP').val(),
                            apellidoPaterno: ($('#ModGruFamApellidoPaterno_RP').length > 0) ? $('#ModGruFamApellidoPaterno_RP').val() : '',
                            apellidoMaterno: ($('#ModGruFamApellidoMaterno_RP').length > 0) ? $('#ModGruFamApellidoMaterno_RP').val() : '',
                            nombres: ($('#ModGruFamNombres_RP').length > 0) ? $('#ModGruFamNombres_RP').val() : '',
                            tipoIdentificacion: ($('#ModGruFamTipoIdentificacion_RP').length > 0) ? $('#ModGruFamTipoIdentificacion_RP').val() : '',
                            numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion_RP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RP').val() : '',
                            parentesco: $('#ModGruFamParentesco_RP').val(),
                            sexo: $('#ModGruFamSexo_RP').val(),
                            fechaNacimiento: $('#ModGruFamFechaNacimiento_RP').val(),
                            invalidez: $('#ModGruFamIndInvalidez_RP').val(),
                            tipoInvalidez: $('#ModGruFamTipoInvalidez_RP').val(),
                            fechaInvalidez: $('#ModGruFamFechaInvalidez_RP').val(),
                            PEP: $('#ModGruFamPEP_RP').val(),
                            sujetoObligado: $('#ModGruFamSO_RP').val(),
                            nacionalidad: $('#ModGruFamNacional_RP').val(),
                            profesion: $('#ModGruFamProfesion_RP').val(),
                            residencia: $('#ModGruFamResidencia_RP').val(),
                            banco: $('#ModGruFamBanco_RP').val(),
                            tipoBanco: $('#ModGruFamTipoCtaBanco_RP').val(),
                            comunicacion: $('#ModGruFamComunicacion_RP').val(),
                            numerobanco: $('#ModGruFamNumeroBanco_RP').val(),
                            confidencialidadDatos: $('#ModGruFamConfidencialidadDatos_RP').val(),
                            flagRenta: 'particularplus',
                            pjeRenta: $('#ModGruFamPorcentaje_RP').val(),
                            estadoCivil: str_estadoCivil,
                            correoElectronico: str_correoElectronico,
                            centrolaboral: str_centrolaboral,
                            cargo: str_cargo,
                            actividadeconomica: str_actividadeconomica,
                            monedaingreso: str_monedaingreso,
                            ingreso: str_ingreso,
                            telefono: str_telefono,
                            celular: str_celular,
                            origen_fondo: str_origen_fondo,
                        }
                        console.log('Modificar Grupo Familiar')
                        console.log({ body })

                        const tienePermiso = UtilitariosManager.ValidarPermisoActivo(EnumsPermisos.GrupoFamiliarActualizar);
                        if (!tienePermiso) {
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Usted no tiene permisos para insertar un grupo familiar.');
                            $('#ModalCuadroMensaje').dialog('open');
                            $('#ModalCotizando').dialog('close');
                            return;
                        }

                        const {errores, controles} = validarGrupoFamiliar(
                            session.RolAzman,
                            body.apellidoPaterno,
                            body.apellidoMaterno,
                            body.nombres,
                            body.tipoIdentificacion,
                            body.numeroIdentificacion,
                            body.parentesco,
                            body.sexo,
                            body.fechaNacimiento,
                            body.invalidez,
                            body.tipoInvalidez,
                            body.fechaInvalidez,
                            body.pjeRenta,
                            body.estadoCivil,
                            body.correoElectronico,
                            body.centrolaboral,
                            body.cargo,
                            body.actividadeconomica,
                            body.monedaingreso,
                            body.ingreso,
                            body.telefono,
                            body.celular)

                        if (errores.length === 0) {
                            console.log(session.RolAzman)
                            const listaAgentes = JSON.parse(UtilitariosManager.getStorageItem('listaAgentes'))

                            if(
                            listaAgentes.find(agente => agente.Id === solicitud.Agente.Id) ||
                            session.RolAzman == EnumsRoles.AgenteExterno
                                || session.RolAzman == EnumsRoles.JefeOperaciones
                                || session.RolAzman == EnumsRoles.AsistenteComercial
                                || session.RolAzman == EnumsRoles.GerenteDivision) {
                                    gru = {}
                                    gru.Id = +body.idGrupoFamiliar;
                                    gru.Afiliado = { CUSPP: body.cuspp };
                                    gru.Identificacion = {};
                                    gru.Parentesco = { Id: body.parentesco };
                                    gru.Sexo = body.sexo.charAt(0);
                                    gru.FechaNacimiento = UtilitariosManager.date.parseDate(body.fechaNacimiento);
                                    gru.Invalido = (body.invalidez === "S") ? true : false;
                                    gru.TipoInvalidez = { Id: body.tipoInvalidez };
                                    gru.flagRenta = body.flagRenta;
                                    gru.Usuario = { NombreUsuario: session.Matricula };
                                    gru.ValPjeRenta = parseFloat(body.pjeRenta);
                                    gru.CorreoElectronico = body.correoElectronico;

                                    if (body.apellidoPaterno.trim() !== "") {
                                        gru.ApellidoPaterno = body.apellidoPaterno;
                                    }

                                    if (body.apellidoMaterno.trim() !== "") {
                                        gru.ApellidoMaterno = body.apellidoMaterno;
                                    }

                                    if (body.nombres.trim() !== "") {
                                        gru.Nombre = body.nombres;
                                    }

                                    if (body.tipoIdentificacion.trim() !== "0") {
                                        gru.Identificacion.IdTipo = body.tipoIdentificacion;
                                    }

                                    if (body.numeroIdentificacion.trim() !== "") {
                                        gru.Identificacion.Numero = body.numeroIdentificacion;
                                    }

                                    if (body.fechaInvalidez.trim() !== "") {
                                        gru.FechaInvalidez = new Date(body.fechaInvalidez);
                                    }

                                    if (body.nacionalidad.trim() !== "0") {
                                        gru.Nacionalidad = { cod_parametro: body.nacionalidad };
                                    } else {
                                        gru.Nacionalidad = { cod_parametro: "0" };
                                    }

                                    if (body.profesion.trim() !== "0") {
                                        gru.Profesion = { cod_parametro: body.profesion };
                                    } else {
                                        gru.Profesion = { cod_parametro: "0" };
                                    }

                                    if (body.residencia.trim() !== "0") {
                                        gru.Residencia = { cod_parametro: body.residencia };
                                    } else {
                                        gru.Residencia = { cod_parametro: "0" };
                                    }

                                    gru.ind_PEP = (body.PEP === "S") ? true : false;
                                    gru.ind_SujetoObligado = (body.sujetoObligado === "S") ? true : false;

                                    if (body.banco.trim() !== "0") {
                                        gru.Banco = { Id: body.banco };
                                    } else {
                                        gru.Banco = { Id: "" };
                                    }

                                    if (body.tipoBanco.trim() !== "00") {
                                        gru.TipoCtaBanco = { Id: body.tipoBanco };
                                    } else {
                                        gru.TipoCtaBanco = { Id: "" };
                                    }

                                    if (body.comunicacion.trim() !== "0") {
                                        gru.Comunicacion = { Id: body.comunicacion };
                                    } else {
                                        gru.Comunicacion = { Id: "0" };
                                    }

                                    if (body.numerobanco.trim() !== "") {
                                        gru.NumeroBanco = body.numerobanco;
                                    }

                                    gru.Confidencialidaddatos = { Id: body.confidencialidadDatos };

                                    if (body.estadoCivil.trim() !== "0") {
                                        gru.estadoCivil = body.estadoCivil;
                                    }

                                    if (body.correoElectronico.trim() !== "") {
                                        gru.CorreoElectronico = body.correoElectronico;
                                    }

                                    if (body.centrolaboral.trim() !== "") {
                                        gru.centroLaboral = body.centrolaboral;
                                    }

                                    if (body.cargo.trim() !== "") {
                                        gru.cargo = body.cargo;
                                    }

                                    if (body.actividadeconomica.trim() !== "") {
                                        gru.actividadEconomica = body.actividadeconomica;
                                    }

                                    if (body.monedaingreso.trim() !== "0") {
                                        gru.monedaIngreso = { Id: body.monedaingreso };
                                    }

                                    if (body.ingreso.trim() !== "") {
                                        gru.ingresoNeto = parseFloat(body.ingreso.toString());
                                    }

                                    if (body.telefono.trim() !== "") {
                                        gru.telefono = body.telefono;
                                    }

                                    if (body.celular.trim() !== "") {
                                        gru.celular = body.celular;
                                    }

                                    if (body.origen_fondo && body.origen_fondo.trim() !== "") {
                                        gru.OrigenFondo = { declaracionJurada: body.origen_fondo };
                                    }

                                    gru.SolicitudRPPlus = { Id: "" };
                                    if (solicitud.Id !== "") {
                                        gru.SolicitudRPPlus = { Id: solicitud.Id };
                                        if (gru.ind_PEP === false && gru.Parentesco.Id === EnumParentesco.Afiliado) {
                                            const payloadPlaft = {
                                                TipoIdentificacion: gru.Identificacion.IdTipo,
                                                NumeroIdentificacion: gru.Identificacion.Numero,
                                                ApellidoPaterno: gru.ApellidoPaterno,
                                                ApellidoMaterno: gru.ApellidoMaterno,
                                                Nombre: gru.Nombre
                                            }
                                            const coincidencia = await ApiCotizadorIFP.ObtenerAfiliadoCoincidenciaLN(payloadPlaft);
                                            if (coincidencia.PEP) {
                                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                                $('#MCMIcono').attr('class', 'error');
                                                $('#MCMContenedor').html('El Asegurado <strong>Si Es </strong> Persona Expuesta Politicamente.');
                                                $('#ModalCuadroMensaje').dialog('open');
                                            }
                                        }
                                    }

                                    console.log({grupoFamiliar: gru})

                                    data = await ApiCotizadorIFP.ActualizarGrupoFamiliar(gru);

                            } else {
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Cliente no pertenece a su cartera de ventas. Verifique.');
                                $('#ModalCuadroMensaje').dialog('open');
                                $('#ModalCotizando').dialog('close');
                            }
                        } else {
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html(errores.join('<br>'));
                            if (controles != null) {
                                if (controles[0].length) $('#ModGruFamApellidoPaterno_RP').attr('class', controles[0]);
                                if (controles[1].length) $('#ModGruFamApellidoMaterno_RP').attr('class', controles[1]);
                                if (controles[2].length) $('#ModGruFamNombres_RP').attr('class', controles[2]);
                                if (controles[3].length) $('#ConModGruFamTipoIdentificacion_RP').attr('class', controles[3]);
                                if (controles[4].length) $('#ModGruFamNumeroIdentificacion_RP').attr('class', controles[4]);
                                if (controles[5].length) $('#ConModGruFamParentesco_RP').attr('class', controles[5]);
                                if (controles[6].length) $('#ConModGruFamSexo_RP').attr('class', controles[6]);
                                if (controles[7].length) $('#ModGruFamFechaNacimiento_RP').attr('class', controles[7]);
                                //if (data.Controles[8].length) $('#ConModGruFamIndInvalidez_RP').attr('class', data.Controles[8]);
                                //if (data.d.Controles[9].length) $('#ConModGruFamTipoInvalidez_RP').attr('class', data.d.Controles[9]);
                                //if (data.Controles[10].length) $('#ModGruFamFechaInvalidez_RP').attr('class', data.Controles[10]);
                                if (controles[11].length) $('#ModGruFamPorcentaje_RP').attr('class', controles[11]);
                            }
                            $('#ModalCuadroMensaje').dialog('open');
                            $('#ModalCotizando').dialog('close');
                        }
                    } else {
                        const body = {
                            idGrupoFamiliar: $('#ModIdGrupoFamiliar').val(),
                            cuspp: $('#HCUSPP_RP').val(),
                            apellidoPaterno: ($('#ModGruFamApellidoPaterno_RP').length > 0) ? $('#ModGruFamApellidoPaterno_RP').val() : '',
                            apellidoMaterno: ($('#ModGruFamApellidoMaterno_RP').length > 0) ? $('#ModGruFamApellidoMaterno_RP').val() : '',
                            nombres: ($('#ModGruFamNombres_RP').length > 0) ? $('#ModGruFamNombres_RP').val() : '',
                            tipoIdentificacion: ($('#ModGruFamTipoIdentificacion_RP').length > 0) ? $('#ModGruFamTipoIdentificacion_RP').val() : '',
                            numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion_RP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RP').val() : '',
                            parentesco: $('#ModGruFamParentesco_RP').val(),
                            sexo: $('#ModGruFamSexo_RP').val(),
                            fechaNacimiento: $('#ModGruFamFechaNacimiento_RP').val(),
                            invalidez: $('#ModGruFamIndInvalidez_RP').val(),
                            tipoInvalidez: $('#ModGruFamTipoInvalidez_RP').val(),
                            fechaInvalidez: $('#ModGruFamFechaInvalidez_RP').val(),
                            PEP: $('#ModGruFamPEP_RP').val(),
                            sujetoObligado: $('#ModGruFamSO_RP').val(),
                            nacionalidad: $('#ModGruFamNacional_RP').val(),
                            profesion: $('#ModGruFamProfesion_RP').val(),
                            residencia: $('#ModGruFamResidencia_RP').val(),
                            banco: $('#ModGruFamBanco_RP').val(),
                            tipoBanco: $('#ModGruFamTipoCtaBanco_RP').val(),
                            comunicacion: $('#ModGruFamComunicacion_RP').val(),
                            numerobanco: $('#ModGruFamNumeroBanco_RP').val(),
                            confidencialidadDatos: $('#ModGruFamConfidencialidadDatos_RP').val(),
                            flagRenta: 'particularplus',
                            pjeRenta: $('#ModGruFamPorcentaje_RP').val(),
                            estadoCivil: str_estadoCivil,
                            correoElectronico: str_correoElectronico,
                            centrolaboral: str_centrolaboral,
                            cargo: str_cargo,
                            actividadeconomica: str_actividadeconomica,
                            monedaingreso: str_monedaingreso,
                            ingreso: str_ingreso,
                            telefono: str_telefono,
                            celular: str_celular,
                        }

                        console.log('Insertar Grupo Familiar')
                        console.log({ body })
                        
                        const tienePermiso = UtilitariosManager.ValidarPermisoActivo(EnumsPermisos.GrupoFamiliarInsertar);
                        if (!tienePermiso) {
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Usted no tiene permisos para insertar un grupo familiar.');
                            $('#ModalCuadroMensaje').dialog('open');
                            return;
                        }

                        const listGrupoFamiliar = solicitud.Beneficiarios;
                        const grupoFamiliar = listGrupoFamiliar.find(x => x.Id == body.idGrupoFamiliar);

                        let validaPjeRenta = true;

                        if(grupoFamiliar && grupoFamiliar.TipoCobertura === 'CA') {
                            validaPjeRenta = false;
                        }

                        const {errores, controles} = validarGrupoFamiliar(
                            session.RolAzman,
                            body.apellidoPaterno,
                            body.apellidoMaterno,
                            body.nombres,
                            body.tipoIdentificacion,
                            body.numeroIdentificacion,
                            body.parentesco,
                            body.sexo,
                            body.fechaNacimiento,
                            body.invalidez,
                            body.tipoInvalidez,
                            body.fechaInvalidez,
                            body.pjeRenta,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            validaPjeRenta)

                        if (errores.length === 0) {
                            if(
                            session.RolAzman == EnumsRoles.AgenteExterno
                                || session.RolAzman == EnumsRoles.JefeOperaciones
                                || session.RolAzman == EnumsRoles.AsistenteComercial
                                || session.RolAzman == EnumsRoles.GerenteDivision) {
                                        let gru;

                                        if (!grupoFamiliar) {
                                            gru = {};
                                        } else {
                                            gru = grupoFamiliar;
                                        }

                                        gru.Afiliado = { CUSPP: body.cuspp };
                                        gru.Identificacion = {};
                                        gru.Parentesco = { Id: body.parentesco };
                                        gru.Sexo = body.sexo.charAt(0);
                                        gru.FechaNacimiento = UtilitariosManager.date.parseDate(body.fechaNacimiento);
                                        gru.Invalido = (body.invalidez === "S") ? true : false;
                                        gru.TipoInvalidez = { Id: body.tipoInvalidez };
                                        gru.Usuario = { NombreUsuario: session.Matricula };
                                        gru.ValPjeRenta = parseFloat(body.pjeRenta);

                                        if (body.apellidoPaterno.trim() !== "") {
                                            gru.ApellidoPaterno = body.apellidoPaterno;
                                        }

                                        if (body.apellidoMaterno.trim() !== "") {
                                            gru.ApellidoMaterno = body.apellidoMaterno;
                                        }

                                        if (body.nombres.trim() !== "") {
                                            gru.Nombre = body.nombres;
                                        }

                                        if (body.tipoIdentificacion.trim() !== "0") {
                                            gru.Identificacion.IdTipo = body.tipoIdentificacion;
                                        }

                                        if (body.numeroIdentificacion.trim() !== "") {
                                            gru.Identificacion.Numero = body.numeroIdentificacion;
                                        }

                                        if (body.fechaInvalidez.trim() !== "") {
                                            gru.FechaInvalidez = new Date(body.fechaInvalidez);
                                        }

                                        if (body.nacionalidad.trim() !== "0") {
                                            gru.Nacionalidad = { cod_parametro: body.nacionalidad };
                                        } else {
                                            gru.Nacionalidad = { cod_parametro: "0" };
                                        }

                                        if (body.profesion.trim() !== "0") {
                                            gru.Profesion = { cod_parametro: body.profesion };
                                        } else {
                                            gru.Profesion = { cod_parametro: "0" };
                                        }

                                        if (body.residencia.trim() !== "0") {
                                            gru.Residencia = { cod_parametro: body.residencia };
                                        } else {
                                            gru.Residencia = { cod_parametro: "0" };
                                        }

                                        gru.ind_PEP = (body.PEP === "S") ? true : false;
                                        gru.ind_SujetoObligado = (body.sujetoObligado === "S") ? true : false;

                                        if (body.banco.trim() !== "0") {
                                            gru.Banco = { Id: "" };
                                        } else {
                                            gru.Banco = { Id: "" };
                                        }

                                        if (body.tipoBanco.trim() !== "00") {
                                            gru.TipoCtaBanco = { Id: body.tipoBanco };
                                        } else {
                                            gru.TipoCtaBanco = { Id: "" };
                                        }

                                        if (body.comunicacion.trim() !== "0") {
                                            gru.Comunicacion = { Id: body.comunicacion };
                                        } else {
                                            gru.Comunicacion = { Id: "0" };
                                        }

                                        if (body.numerobanco.trim() !== "") {
                                            gru.NumeroBanco = body.numerobanco;
                                        }

                                        if (grupoFamiliar) {
                                            for (let i = 0; i < listGrupoFamiliar.length; i++) {
                                                if (listGrupoFamiliar[i].Id.toString() === body.idGrupoFamiliar || 
                                                    listGrupoFamiliar[i].IdGrupoFamiliar.toString() === body.idGrupoFamiliar) {
                                                    listGrupoFamiliar[i] = gru;
                                                }
                                            }
                                            // if (lstGrupoFamiliar.filter(gf => gf.Id === idGrupoFamiliar).length > 0 || 
                                            //     lstGrupoFamiliar.filter(gf => gf.IdGrupoFamiliar === idGrupoFamiliar).length > 0)
                                        } else {
                                            gru.Id = listGrupoFamiliar.length + 1;
                                            gru.Ind_Cierre = true;
                                            listGrupoFamiliar.push(gru);
                                        }

                                        // lstSolicitudIFP.Beneficiarios = lstGrupoFamiliar;

                                        solicitud.Beneficiarios = listGrupoFamiliar;

                                        $('#HSolicitudSerializado').val(JSON.stringify(solicitud));

                                        data.Estado = 'OK';
                            } else {
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Cliente no pertenece a su cartera de ventas. Verifique.');
                                $('#ModalCuadroMensaje').dialog('open');
                                $('#ModalCotizando').dialog('close');
                            }
                        } else {
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html(errores.join('<br>'));
                            if (controles != null) {
                                if (controles[0].length) $('#ModGruFamApellidoPaterno_RP').attr('class', controles[0]);
                                if (controles[1].length) $('#ModGruFamApellidoMaterno_RP').attr('class', controles[1]);
                                if (controles[2].length) $('#ModGruFamNombres_RP').attr('class', controles[2]);
                                if (controles[3].length) $('#ConModGruFamTipoIdentificacion_RP').attr('class', controles[3]);
                                if (controles[4].length) $('#ModGruFamNumeroIdentificacion_RP').attr('class', controles[4]);
                                if (controles[5].length) $('#ConModGruFamParentesco_RP').attr('class', controles[5]);
                                if (controles[6].length) $('#ConModGruFamSexo_RP').attr('class', controles[6]);
                                if (controles[7].length) $('#ModGruFamFechaNacimiento_RP').attr('class', controles[7]);
                                //if (data.Controles[8].length) $('#ConModGruFamIndInvalidez_RP').attr('class', data.Controles[8]);
                                //if (data.d.Controles[9].length) $('#ConModGruFamTipoInvalidez_RP').attr('class', data.d.Controles[9]);
                                //if (data.Controles[10].length) $('#ModGruFamFechaInvalidez_RP').attr('class', data.Controles[10]);
                                if (controles[11].length) $('#ModGruFamPorcentaje_RP').attr('class', controles[11]);
                            }
                            $('#ModalCuadroMensaje').dialog('open');
                            $('#ModalGrupoFamiliar').dialog('close');
                        }
                    }

                    if (data) {
                        $('#ModalCotizando').dialog('close');
                        if (data.Estado == 'OK') {

                            /* Recargar la grilla de personas vinculadas */
                            CargarTablaPersonasVinculadas_RP($('#ModGruFamsolicitud').val())

                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                            $clock.countdown(selectedDate.toString());

                            //if ($("#ModGruFamAceptarCierre_RP").text() == "Cerrar Solicitud")
                            if ($("#ModGruFamAceptarCierre_RP").text() == "Enviar al cliente") {
                                $('#MCATablaPregunta').val('cerrar');

                                $("#MCAIcono").attr("class", "advertencia");

                                var mensaje = '<div class="alerta-agente-titulo">Firma Digital de documentos de <span class="resaltado">Renta Particular IFP</span></div>';
                                mensaje += '<div class="alerta-agente-contenido"><p>Usted va a enviar el enlace de Firma Digital a <span class="resaltado">' + $('#ModGruFamNombres_RP').val() + ' ' + $('#ModGruFamApellidoPaterno_RP').val() + ' ' + $('#ModGruFamApellidoMaterno_RP').val() + '</span> cuyo documento de identidad es <span class="resaltado">' + $('#ModGruFamTipoIdentificacion_RP').find(':selected').text() + ' ' + $("#ModGruFamNumeroIdentificacion_RP").val() + '</span> al siguiente correo electrónico: <span class="resaltado">' + $('#ModGruFamMail_RP').val() + '</span>.</p>';
                                mensaje += '<p>Para que el cliente pueda firmar digitalmente sus documentos, luego de que valide su información se le enviará una clave al siguiente número de celular: <span class="resaltado">' + $("#ModGruFamCelular_RP").val() + '</span>.</p>';
                                mensaje += '<p>Utilice la <b>Vista Previa</b> de los documentos para validar que todos los datos que le enviará al cliente son correctos, en caso de que haya algún error por favor modifique los datos en el Cotizador Web de Rentas y luego vuelva a validar en la opción de <b>Vista Previa</b>.</p>';
                                mensaje += '<p>Para evitar inconvenientes por favor <b>valide cuidadosamente</b> que toda la información del cliente es correcta antes de enviarle el enlace de Firma Digital.</p></div >';

                                //$("#MCAContenedor").html("Se procederá a cerrar la solicitud, ya no se podrá realizar modificaciones. <p>¿Desea cerrar la solicitud?");
                                $("#MCAContenedor").html(mensaje);

                                $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                                $("#ModalCuadroAdvertencia").dialog("open");
                                $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');
                            } else {
                                $("#Contenido_ModSolSiguiente_RP").click();
                            }

                        }
                        else if (data.Estado == 'TOKEN') {
                            CerrarSesionExpirada();
                        }
                        else {

                            $('#ModalCuadroMensaje').dialog({ title: data.Titulo });
                            $('#MCMIcono').attr('class', data.Icono);
                            $('#MCMContenedor').html(data.Mensaje);
                            if (data.Controles != null) {
                                if (data.Controles[0].length) $('#ModGruFamApellidoPaterno_RP').attr('class', data.Controles[0]);
                                if (data.Controles[1].length) $('#ModGruFamApellidoMaterno_RP').attr('class', data.Controles[1]);
                                if (data.Controles[2].length) $('#ModGruFamNombres_RP').attr('class', data.Controles[2]);
                                if (data.Controles[3].length) $('#ConModGruFamTipoIdentificacion_RP').attr('class', data.Controles[3]);
                                if (data.Controles[4].length) $('#ModGruFamNumeroIdentificacion_RP').attr('class', data.Controles[4]);
                                if (data.Controles[5].length) $('#ConModGruFamParentesco_RP').attr('class', data.Controles[5]);
                                if (data.Controles[6].length) $('#ConModGruFamSexo_RP').attr('class', data.Controles[6]);
                                if (data.Controles[7].length) $('#ModGruFamFechaNacimiento_RP').attr('class', data.Controles[7]);
                                //if (data.Controles[8].length) $('#ConModGruFamIndInvalidez_RP').attr('class', data.Controles[8]);
                                //if (data.d.Controles[9].length) $('#ConModGruFamTipoInvalidez_RP').attr('class', data.d.Controles[9]);
                                //if (data.Controles[10].length) $('#ModGruFamFechaInvalidez_RP').attr('class', data.Controles[10]);
                                if (data.Controles[11].length) $('#ModGruFamPorcentaje_RP').attr('class', data.Controles[11]);
                            }
                            $('#ModalCuadroMensaje').dialog('open');

                            $('#ModGruFamCancelarCierre_RP').attr('class', 'boton darkblue sharp');
                            $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');
                        }
                    }
                } catch (error) {
                    console.log(error)
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al guardar la información del beneficiario.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                } finally {
                    ModGruFamBotonesInactivos = false;
                    $('#ModGruFamCancelarCierre_RP').attr('class', 'boton darkblue sharp');
                    $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');
                }

                /* if ($('#ModGruFamParentesco_RP').val() == '80') {

                    params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        idGrupoFamiliar: $('#ModIdGrupoFamiliar').val(),
                        cuspp: $('#HCUSPP_RP').val(),
                        apellidoPaterno: ($('#ModGruFamApellidoPaterno_RP').length > 0) ? $('#ModGruFamApellidoPaterno_RP').val() : '',
                        apellidoMaterno: ($('#ModGruFamApellidoMaterno_RP').length > 0) ? $('#ModGruFamApellidoMaterno_RP').val() : '',
                        nombres: ($('#ModGruFamNombres_RP').length > 0) ? $('#ModGruFamNombres_RP').val() : '',
                        tipoIdentificacion: ($('#ModGruFamTipoIdentificacion_RP').length > 0) ? $('#ModGruFamTipoIdentificacion_RP').val() : '',
                        numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion_RP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RP').val() : '',
                        numeroIdentificacionlongitud: ($('#ModGruFamNumeroIdentificacion_RP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RP').val() : '', //<GTI_7012_30>
                        parentesco: $('#ModGruFamParentesco_RP').val(),
                        sexo: $('#ModGruFamSexo_RP').val(),
                        fechaNacimiento: $('#ModGruFamFechaNacimiento_RP').val(),
                        invalidez: $('#ModGruFamIndInvalidez_RP').val(),
                        tipoInvalidez: $('#ModGruFamTipoInvalidez_RP').val(),
                        fechaInvalidez: $('#ModGruFamFechaInvalidez_RP').val(),
                        PEP: $('#ModGruFamPEP_RP').val(),
                        sujetoObligado: $('#ModGruFamSO_RP').val(),
                        nacionalidad: $('#ModGruFamNacional_RP').val(),
                        profesion: $('#ModGruFamProfesion_RP').val(),
                        residencia: $('#ModGruFamResidencia_RP').val(),
                        banco: $('#ModGruFamBanco_RP').val(),
                        tipoBanco: $('#ModGruFamTipoCtaBanco_RP').val(),
                        comunicacion: $('#ModGruFamComunicacion_RP').val(),
                        numerobanco: $('#ModGruFamNumeroBanco_RP').val(),
                        confidencialidadDatos: $('#ModGruFamConfidencialidadDatos_RP').val(),
                        flagRenta: 'particularplus',
                        pjeRenta: $('#ModGruFamPorcentaje_RP').val(),
                        estadoCivil: str_estadoCivil,
                        correoElectronico: str_correoElectronico,
                        centrolaboral: str_centrolaboral,
                        cargo: str_cargo,
                        actividadeconomica: str_actividadeconomica,
                        monedaingreso: str_monedaingreso,
                        ingreso: str_ingreso,
                        telefono: str_telefono,
                        celular: str_celular,
                        origen_fondo: str_origen_fondo
                        //objSolicitud: $('#HSolicitudSerializado').val()
                    }

                    postUrl = '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ModificarGrupoFamiliar';
                } else {

                    params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        idGrupoFamiliar: $('#ModIdGrupoFamiliar').val(),
                        cuspp: $('#HCUSPP_RP').val(),
                        apellidoPaterno: ($('#ModGruFamApellidoPaterno_RP').length > 0) ? $('#ModGruFamApellidoPaterno_RP').val() : '',
                        apellidoMaterno: ($('#ModGruFamApellidoMaterno_RP').length > 0) ? $('#ModGruFamApellidoMaterno_RP').val() : '',
                        nombres: ($('#ModGruFamNombres_RP').length > 0) ? $('#ModGruFamNombres_RP').val() : '',
                        tipoIdentificacion: ($('#ModGruFamTipoIdentificacion_RP').length > 0) ? $('#ModGruFamTipoIdentificacion_RP').val() : '',
                        numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion_RP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RP').val() : '',
                        numeroIdentificacionlongitud: ($('#ModGruFamNumeroIdentificacion_RP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RP').val() : '', //<GTI_7012_30>
                        parentesco: $('#ModGruFamParentesco_RP').val(),
                        sexo: $('#ModGruFamSexo_RP').val(),
                        fechaNacimiento: $('#ModGruFamFechaNacimiento_RP').val(),
                        invalidez: $('#ModGruFamIndInvalidez_RP').val(),
                        tipoInvalidez: $('#ModGruFamTipoInvalidez_RP').val(),
                        fechaInvalidez: $('#ModGruFamFechaInvalidez_RP').val(),
                        PEP: $('#ModGruFamPEP_RP').val(),
                        sujetoObligado: $('#ModGruFamSO_RP').val(),
                        nacionalidad: $('#ModGruFamNacional_RP').val(),
                        profesion: $('#ModGruFamProfesion_RP').val(),
                        residencia: $('#ModGruFamResidencia_RP').val(),
                        banco: $('#ModGruFamBanco_RP').val(),
                        tipoBanco: $('#ModGruFamTipoCtaBanco_RP').val(),
                        comunicacion: $('#ModGruFamComunicacion_RP').val(),
                        numerobanco: $('#ModGruFamNumeroBanco_RP').val(),
                        confidencialidadDatos: $('#ModGruFamConfidencialidadDatos_RP').val(),
                        flagRenta: 'particularplus',
                        pjeRenta: $('#ModGruFamPorcentaje_RP').val(),
                        estadoCivil: str_estadoCivil,
                        correoElectronico: str_correoElectronico,
                        centrolaboral: str_centrolaboral,
                        cargo: str_cargo,
                        actividadeconomica: str_actividadeconomica,
                        monedaingreso: str_monedaingreso,
                        ingreso: str_ingreso,
                        telefono: str_telefono,
                        celular: str_celular,
                        objSolicitud: $('#HSolicitudSerializado').val()
                    }

                    postUrl = '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/InsertarGrupoFamiliar';
                }

                $.ajax({
                    type: 'POST',
                    url: postUrl,
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        $('#ModalCotizando').dialog('close');
                        if (data.d.Estado == 'OK') { */

                /* Recargar la grilla de personas vinculadas */
                /* CargarTablaPersonasVinculadas_RP($('#ModGruFamsolicitud').val())

                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                $clock.countdown(selectedDate.toString());

                if (data.d.Mensaje != null) {
                    $('#HSolicitudSerializado').val(data.d.Mensaje)
                }

                //if ($("#ModGruFamAceptarCierre_RP").text() == "Cerrar Solicitud")
                if ($("#ModGruFamAceptarCierre_RP").text() == "Enviar al cliente") {
                    $('#MCATablaPregunta').val('cerrar');

                    $("#MCAIcono").attr("class", "advertencia");

                    var mensaje = '<div class="alerta-agente-titulo">Firma Digital de documentos de <span class="resaltado">Renta Particular IFP</span></div>';
                    mensaje += '<div class="alerta-agente-contenido"><p>Usted va a enviar el enlace de Firma Digital a <span class="resaltado">' + $('#ModGruFamNombres_RP').val() + ' ' + $('#ModGruFamApellidoPaterno_RP').val() + ' ' + $('#ModGruFamApellidoMaterno_RP').val() + '</span> cuyo documento de identidad es <span class="resaltado">' + $('#ModGruFamTipoIdentificacion_RP').find(':selected').text() + ' ' + $("#ModGruFamNumeroIdentificacion_RP").val() + '</span> al siguiente correo electrónico: <span class="resaltado">' + $('#ModGruFamMail_RP').val() + '</span>.</p>';
                    mensaje += '<p>Para que el cliente pueda firmar digitalmente sus documentos, luego de que valide su información se le enviará una clave al siguiente número de celular: <span class="resaltado">' + $("#ModGruFamCelular_RP").val() + '</span>.</p>';
                    mensaje += '<p>Utilice la <b>Vista Previa</b> de los documentos para validar que todos los datos que le enviará al cliente son correctos, en caso de que haya algún error por favor modifique los datos en el Cotizador Web de Rentas y luego vuelva a validar en la opción de <b>Vista Previa</b>.</p>';
                    mensaje += '<p>Para evitar inconvenientes por favor <b>valide cuidadosamente</b> que toda la información del cliente es correcta antes de enviarle el enlace de Firma Digital.</p></div >';

                    //$("#MCAContenedor").html("Se procederá a cerrar la solicitud, ya no se podrá realizar modificaciones. <p>¿Desea cerrar la solicitud?");
                    $("#MCAContenedor").html(mensaje);

                    $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                    $("#ModalCuadroAdvertencia").dialog("open");
                    $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');
                } else {
                    $("#Contenido_ModSolSiguiente_RP").click();
                }

                //LLAMA A LA PANTALLA DE BENEFICIARIOS CA
                //window.location.href = "../RentaIFP/ResumenBeneficiariosCA.aspx";                            

                //if ($("#ModGruFamAceptarCierre_RP").text() == "Siguiente")
                //{
                //    if ($('#ModGruFamParentesco_RP').val() == '80') {
                //        $('#MCATablaPregunta').val('agregar');
                //        $("#MCAIcono").attr("class", "advertencia");
                //        $("#MCAContenedor").html("¿Desea Agregar Beneficiarios?");
                //        $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                //        $("#ModalCuadroAdvertencia").dialog("open");
                //        $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');

                //    }
                //    else {
                //        if ($('#ModGruFamCantidad').val() == $('#ModGruFamTotal').val()) {
                //            window.location.href = "../RentaIFP/ResumenBeneficiarios.aspx";
                //        } else {
                //            var params = {
                //                total: $('#ModGruFamTotal').val(),
                //                cantidad: parseInt($('#ModGruFamCantidad').val()) + 1,
                //                paginaLlamada: '../RentaIFP/SeleccionSolicitud.aspx'
                //            }

                //            $.ajax({
                //                type: 'POST',
                //                url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/IngresarCantidadFamiliar',
                //                contentType: "application/json; charset=iso-8859-1",
                //                dataType: 'json',
                //                data: $.toJSON(params),
                //                success: function (data) {
                //                    var params2 = {
                //                        idGrupoFamiliar: parseInt($('#ModGruFamCantidad').val()) + 1,
                //                    }

                //                    window.location.href = "../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx";

                //                },
                //                error: function (XMLHttpRequest, textStatus, errorThrown) {
                //                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                //                        document.location.reload(true);
                //                    }
                //                    else {
                //                        $('#MCMIcono').attr('class', 'error');
                //                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la información del beneficiario.');
                //                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                //                        $('#ModalCuadroMensaje').dialog('open');
                //                    }
                //                    $('#ModalGrupoFamiliar').dialog('close');
                //                }
                //            });

                //        }
                //    }
                //}
                //else if ($("#ModGruFamAceptarCierre_RP").text() == "Cerrar Solicitud") {
                //    $('#MCATablaPregunta').val('cerrar');
                //    $("#MCAIcono").attr("class", "advertencia");
                //    $("#MCAContenedor").html("Se procederá a cerrar la solicitud, ya no se podrá realizar modificaciones. <p>¿Desea cerrar la solicitud?");
                //    $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                //    $("#ModalCuadroAdvertencia").dialog("open");
                //    $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');
                //}

            }
            else if (data.d.Estado == 'TOKEN') {
                CerrarSesionExpirada();
            }
            else {

                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                if (data.d.Controles != null) {
                    if (data.d.Controles[0].length) $('#ModGruFamApellidoPaterno_RP').attr('class', data.d.Controles[0]);
                    if (data.d.Controles[1].length) $('#ModGruFamApellidoMaterno_RP').attr('class', data.d.Controles[1]);
                    if (data.d.Controles[2].length) $('#ModGruFamNombres_RP').attr('class', data.d.Controles[2]);
                    if (data.d.Controles[3].length) $('#ConModGruFamTipoIdentificacion_RP').attr('class', data.d.Controles[3]);
                    if (data.d.Controles[4].length) $('#ModGruFamNumeroIdentificacion_RP').attr('class', data.d.Controles[4]);
                    if (data.d.Controles[5].length) $('#ConModGruFamParentesco_RP').attr('class', data.d.Controles[5]);
                    if (data.d.Controles[6].length) $('#ConModGruFamSexo_RP').attr('class', data.d.Controles[6]);
                    if (data.d.Controles[7].length) $('#ModGruFamFechaNacimiento_RP').attr('class', data.d.Controles[7]);
                    //if (data.d.Controles[8].length) $('#ConModGruFamIndInvalidez_RP').attr('class', data.d.Controles[8]);
                    //if (data.d.Controles[9].length) $('#ConModGruFamTipoInvalidez_RP').attr('class', data.d.Controles[9]);
                    //if (data.d.Controles[10].length) $('#ModGruFamFechaInvalidez_RP').attr('class', data.d.Controles[10]);
                    if (data.d.Controles[11].length) $('#ModGruFamPorcentaje_RP').attr('class', data.d.Controles[11]);
                }
                $('#ModalCuadroMensaje').dialog('open');

                $('#ModGruFamCancelarCierre_RP').attr('class', 'boton darkblue sharp');
                $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                document.location.reload(true);
            }
            else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al guardar la información del beneficiario.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        },
        complete: function () {
            ModGruFamBotonesInactivos = false;
            $('#ModGruFamCancelarCierre_RP').attr('class', 'boton darkblue sharp');
            $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');
        }
    }); */
            }
        }
    });

    /* Botón Nuevo */
    $('#NuevaPersonaVinculada_RPP').live('click', function () {
        $('#ModGruFamModo_RPP').val('N');

        LimpiarFormularioPersonasVinculadas();

        $('#ModGruFamCargando_RP').hide();
        $('#ModalGrupoFamiliar_RP').dialog('open');
        $('#ModGruFamApellidoPaterno_RPP').focus();
    });

    /* Botón Cancelar */
    $('#ModGruFamCancelarCierre_RP').live('click', function () {

        if ($('#ModGruFamCancelarCierre_RP').hasClass('boton darkblue sharp')) {

            if (!ModGruFamBotonesInactivos) {
                var esCorrecto = true;
                var errores = new Array();

                $('#ModGruFamApellidoPaterno_RP').removeClass('formTextboxError');
                $('#ModGruFamApellidoMaterno_RP').removeClass('formTextboxError');
                $('#ModGruFamNombres_RP').removeClass('formTextboxError');
                $('#ConModGruFamTipoIdentificacion_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamNumeroIdentificacion_RP').removeClass('formTextboxError');
                $('#ConModGruFamParentesco_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamSexo_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamFechaNacimiento_RP').removeClass('formTextboxError formCalendarError');

                //$('#ConModGruFamIndInvalidez_RP').removeClass('formComboboxErrorContenedor');
                //$('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxErrorContenedor');
                //$('#ModGruFamFechaInvalidez_RP').removeClass('formTextboxError formCalendarError');

                $('#ConModGruFamNacional_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamProfesion_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamResidencia_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamPEP_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamSO_RP').removeClass('formComboboxErrorContenedor');

                $('#ModGruFamBanco_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamTipoCtaBanco_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamComunicacion_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamNumeroBanco_RP').removeClass('formTextboxError');

                $('#ModGruFamPorcentaje_RP').removeClass('formTextboxError');

                // Apellido Paterno
                var apellidoPaterno = true;
                if ($.trim($('#ModGruFamApellidoPaterno_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Apellido Paterno</strong>. Dato Obligatorio.');
                    apellidoPaterno = false;
                }

                // Apellido Materno
                var apellidoMaterno = true;
                if ($.trim($('#ModGruFamApellidoMaterno_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Apellido Materno</strong>. Dato Obligatorio.');
                    apellidoMaterno = false;
                }

                // Nombres
                var nombres = true;
                if ($.trim($('#ModGruFamNombres_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Nombre</strong>. Dato Obligatorio.');
                    nombres = false;
                }

                // Tipo de Indentificación
                var tipoIdentificacion = true;
                if ($('#ModGruFamTipoIdentificacion_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Tipo de Identifación</strong>. Dato Obligatorio.');
                    tipoIdentificacion = false;
                }

                // Número de Identificación
                var numeroIdentificacion = true;
                if ($.trim($('#ModGruFamNumeroIdentificacion_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Numero de Identificación</strong>. Dato Obligatorio.');
                    numeroIdentificacion = false;
                }

                // Parentesco
                var parentesco = true;
                if ($('#ModGruFamParentesco_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Parentesco</strong>. Dato Obligatorio.');
                    parentesco = false;
                }

                // Sexo
                var sexo = true;
                if ($('#ModGruFamSexo_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Sexo</strong>. Dato Obligatorio.');
                    sexo = false;
                }

                // Fecha de Nacimiento
                var fechaNacimiento = true;
                if ($.trim($('#ModGruFamFechaNacimiento_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Fecha de Nacimiento</strong>. Dato Obligatorio.');
                    fechaNacimiento = false;
                }

                //// Invalidez
                //var invalidez = true;
                //var tipoInvalidez = true;
                //var fechaInvalidez = true;
                //if ($('#ModGruFamIndInvalidez_RP').val() == '0') {
                //    errores.push('Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.');
                //    invalidez = false;
                //}
                //else if ($('#ModGruFamIndInvalidez_RP').val() == 'N') {
                //    if ($('#ModGruFamTipoInvalidez_RP').val() != 'N') {
                //        errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                //        tipoInvalidez = false;
                //    }
                //    if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length > 0) {
                //        errores.push('El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.');
                //        fechaInvalidez = false;
                //    }
                //}
                //else if ($('#ModGruFamIndInvalidez_RP').val() == 'S') {
                //    if ($('#ModGruFamTipoInvalidez_RP').val() != 'P' && $('#ModGruFamTipoInvalidez_RP').val() != 'T') {
                //        errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                //        tipoInvalidez = false;
                //    }

                //    if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length == 0) {
                //        errores.push('Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.');
                //        fechaInvalidez = false;
                //    }
                //}

                //if ($('#ModGruFamTipoInvalidez_RP').val() == '0') {
                //    errores.push('Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.');
                //    tipoInvalidez = false;
                //}


                var nacionalidad = true;
                if ($('#ModGruFamNacional_RP').val() == '0') {
                    errores.push('Seleccioné la <strong>Nacionalidad</strong>. Dato Obligatorio.');
                    nacionalidad = false;
                }

                var profesion = true;
                if ($('#ModGruFamProfesion_RP').val() == '0') {
                    errores.push('Seleccioné la <strong>Profesión</strong>. Dato Obligatorio.');
                    profesion = false;
                }

                var residencia = true;
                if ($('#ModGruFamResidencia_RP').val() == '0') {
                    errores.push('Seleccioné la <strong>Residencia</strong>. Dato Obligatorio.');
                    residencia = false;
                }

                var pep = true;
                if ($('#ModGruFamPEP_RP').val() == '0') {
                    errores.push('Seleccioné si <strong>¿Es Personas Expuestas Políticamente (PEP)?</strong>. Dato Obligatorio.');
                    pep = false;
                }

                var sujetoObligado = true;
                if ($('#ModGruFamSO_RP').val() == '0') {
                    errores.push('Seleccioné si <strong>¿Es Sujeto Obligado?</strong>. Dato Obligatorio.');
                    sujetoObligado = false;
                }



                var Banco = true;
                var TipoBanco = true;
                var Comunicacion = true;
                var NumeroBanco = true;

                if ($('#ModGruFamParentesco_RP').val() == '80') {

                    if ($('#ModGruFamBanco_RP').val() == '0') {
                        errores.push('Seleccioné el <strong>Banco</strong>. Dato Obligatorio.');
                        Banco = false;
                    }

                    if ($('#ModGruFamTipoCtaBanco_RP').val() == '0') {
                        errores.push('Seleccioné el <strong>Tipo de Cuenta</strong>. Dato Obligatorio.');
                        TipoBanco = false;
                    }

                    if ($('#ModGruFamComunicacion_RP').val() == '0') {
                        errores.push('Seleccioné el <strong>Mecanismo de comunicación</strong>. Dato Obligatorio.');
                        Comunicacion = false;
                    }

                    if ($.trim($('#ModGruFamNumeroBanco_RP').val()).length == 0) {
                        errores.push('Ingrese el <strong>Nro. de cuenta</strong>. Dato Obligatorio.');
                        NumeroBanco = false;
                    } else {
                        if ($('#ModGruFamBanco_RP').val() == '0') {
                            if (Banco) {
                                errores.push('Seleccioné el <strong>Banco</strong>. Dato Obligatorio.');
                                Banco = false;
                            }
                        }

                        if ($('#ModGruFamTipoCtaBanco_RP').val() == '0') {
                            if (TipoBanco) {
                                errores.push('Seleccioné el <strong>Tipo de Cuenta</strong>. Dato Obligatorio.');
                                TipoBanco = false;
                            }
                        }

                    }

                }

                var porcentajeRenta = true;
                if ($('#ModGruFamPorcentaje_RP').val() == '0' || $('#ModGruFamPorcentaje_RP').val() == '') {
                    errores.push('Ingrese el campo <strong>Porcentaje (%)</strong>. Dato Obligatorio.');
                    porcentajeRenta = false;
                }


                if (!apellidoPaterno) $('#ModGruFamApellidoPaterno_RP').addClass('formTextboxError');
                if (!apellidoMaterno) $('#ModGruFamApellidoMaterno_RP').addClass('formTextboxError');
                if (!nombres) $('#ModGruFamNombres_RP').addClass('formTextboxError');
                if (!tipoIdentificacion) $('#ConModGruFamTipoIdentificacion_RP').addClass('formComboboxErrorContenedor');
                if (!numeroIdentificacion) $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxError');
                if (!parentesco) $('#ConModGruFamParentesco_RP').addClass('formComboboxErrorContenedor');
                if (!sexo) $('#ConModGruFamSexo_RP').addClass('formComboboxErrorContenedor');
                if (!fechaNacimiento) $('#ModGruFamFechaNacimiento_RP').addClass('formTextboxError formCalendarError');
                //if (!invalidez) $('#ConModGruFamIndInvalidez_RP').addClass('formComboboxErrorContenedor');
                //if (!tipoInvalidez) $('#ConModGruFamTipoInvalidez_RP').addClass('formComboboxErrorContenedor');
                //if (!fechaInvalidez) $('#ModGruFamFechaInvalidez_RP').addClass('formTextboxError formCalendarError');
                //Cple
                if (!nacionalidad) $('#ConModGruFamNacional_RP').addClass('formComboboxErrorContenedor');
                if (!profesion) $('#ConModGruFamProfesion_RP').addClass('formComboboxErrorContenedor');
                if (!residencia) $('#ConModGruFamResidencia_RP').addClass('formComboboxErrorContenedor');
                if (!pep) $('#ConModGruFamPEP_RP').addClass('formComboboxErrorContenedor');
                if (!sujetoObligado) $('#ConModGruFamSO_RP').addClass('formComboboxErrorContenedor');

                if (!Banco) $('#ConModGruFamBanco_RP').addClass('formComboboxErrorContenedor');
                if (!TipoBanco) $('#ConModGruFamTipoCtaBanco_RP').addClass('formComboboxErrorContenedor');
                if (!Comunicacion) $('#ConModGruFamComunicacion_RP').addClass('formComboboxErrorContenedor');
                if (!NumeroBanco) $('#ModGruFamNumeroBanco_RP').addClass('formTextboxError');

                if (!porcentajeRenta) $('#ModGruFamPorcentaje_RP').addClass('formTextboxError');

                //& invalidez & tipoInvalidez & fechaInvalidez 

                esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento & nacionalidad & profesion & residencia & pep & sujetoObligado & Banco & TipoBanco & Comunicacion & NumeroBanco & porcentajeRenta;

                if (!esCorrecto) {
                    $('#MCMIcono').attr('class', 'validacion');
                    $('#MCMContenedor').html(formatearError(errores));
                    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return false;
                }

                ///* Pasó las validaciones */

                $('#ModGruFamCancelarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                $('#ModGruFamAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

                if ($('#ModGruFamParentesco_RP').val() != '80') {
                    $('#ModGruFamConfidencialidadDatos_RP').val('1')
                }

                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Validando beneficiario, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Validando" });
                $("#ModalCotizando").dialog("open");

                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    idGrupoFamiliar: $('#ModIdGrupoFamiliar').val(),
                    cuspp: $('#HCUSPP_RP').val(),
                    apellidoPaterno: ($('#ModGruFamApellidoPaterno_RP').length > 0) ? $('#ModGruFamApellidoPaterno_RP').val() : '',
                    apellidoMaterno: ($('#ModGruFamApellidoMaterno_RP').length > 0) ? $('#ModGruFamApellidoMaterno_RP').val() : '',
                    nombres: ($('#ModGruFamNombres_RP').length > 0) ? $('#ModGruFamNombres_RP').val() : '',
                    tipoIdentificacion: ($('#ModGruFamTipoIdentificacion_RP').length > 0) ? $('#ModGruFamTipoIdentificacion_RP').val() : '',
                    numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion_RP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RP').val() : '',
                    parentesco: $('#ModGruFamParentesco_RP').val(),
                    sexo: $('#ModGruFamSexo_RP').val(),
                    fechaNacimiento: $('#ModGruFamFechaNacimiento_RP').val(),
                    invalidez: $('#ModGruFamIndInvalidez_RP').val(),
                    tipoInvalidez: $('#ModGruFamTipoInvalidez_RP').val(),
                    fechaInvalidez: $('#ModGruFamFechaInvalidez_RP').val(),
                    PEP: $('#ModGruFamPEP_RP').val(),
                    sujetoObligado: $('#ModGruFamSO_RP').val(),
                    nacionalidad: $('#ModGruFamNacional_RP').val(),
                    profesion: $('#ModGruFamProfesion_RP').val(),
                    residencia: $('#ModGruFamResidencia_RP').val(),
                    banco: $('#ModGruFamBanco_RP').val(),
                    tipoBanco: $('#ModGruFamTipoCtaBanco_RP').val(),
                    comunicacion: $('#ModGruFamComunicacion_RP').val(),
                    numerobanco: $('#ModGruFamNumeroBanco_RP').val(),
                    confidencialidadDatos: $('#ModGruFamConfidencialidadDatos_RP').val(),
                    flagRenta: 'particularplus',
                    pjeRenta: $('#ModGruFamPorcentaje_RP').val()
                }

                var postUrl = '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/InsertarGrupoFamiliar';

                $.ajax({
                    type: 'POST',
                    url: postUrl,
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        $('#ModalCotizando').dialog('close');

                        if (data.d.Estado == 'OK') {

                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                            $clock.countdown(selectedDate.toString());

                            var params = {
                                solicitud: $('#ModGruFamsolicitud').val(),
                                fecha: $('#ModGruFamfecsolicitud').val(),
                                estado: $('#ModGruFamestadosolicitud').val(),
                                numCorrelativo: $('#ModGruNumCorrelativo').val(),
                                codPlan: $('#ModGruPlan').val(),
                                paginaLlamada: '../RentaIFP/SeleccionSolicitud.aspx'
                            }


                            $.ajax({
                                type: 'POST',
                                url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/SessionIdGrupoFamiliar',
                                contentType: "application/json; charset=iso-8859-1",
                                dataType: 'json',
                                data: $.toJSON(params),
                                success: function (data) {
                                    var params2 = {
                                        idGrupoFamiliar: parseInt($('#ModGruFamCantidad').val()) - 1
                                    }

                                    $.ajax({
                                        type: 'POST',
                                        url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/BuscarGrupoFamiliarSession',
                                        contentType: "application/json; charset=iso-8859-1",
                                        dataType: 'json',
                                        data: $.toJSON(params2),

                                        success: function (data) {

                                            window.location.href = "../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx";

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





                        }
                        else if (data.d.Estado == 'TOKEN') {
                            CerrarSesionExpirada();
                        }
                        else {
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                            $('#MCMIcono').attr('class', data.d.Icono);
                            $('#MCMContenedor').html(data.d.Mensaje);
                            if (data.d.Controles != null) {
                                if (data.d.Controles[0].length) $('#ModGruFamApellidoPaterno_RP').attr('class', data.d.Controles[0]);
                                if (data.d.Controles[1].length) $('#ModGruFamApellidoMaterno_RP').attr('class', data.d.Controles[1]);
                                if (data.d.Controles[2].length) $('#ModGruFamNombres_RP').attr('class', data.d.Controles[2]);
                                if (data.d.Controles[3].length) $('#ConModGruFamTipoIdentificacion_RP').attr('class', data.d.Controles[3]);
                                if (data.d.Controles[4].length) $('#ModGruFamNumeroIdentificacion_RP').attr('class', data.d.Controles[4]);
                                if (data.d.Controles[5].length) $('#ConModGruFamParentesco_RP').attr('class', data.d.Controles[5]);
                                if (data.d.Controles[6].length) $('#ConModGruFamSexo_RP').attr('class', data.d.Controles[6]);
                                if (data.d.Controles[7].length) $('#ModGruFamFechaNacimiento_RP').attr('class', data.d.Controles[7]);
                                //if (data.d.Controles[8].length) $('#ConModGruFamIndInvalidez_RP').attr('class', data.d.Controles[8]);
                                //if (data.d.Controles[9].length) $('#ConModGruFamTipoInvalidez_RP').attr('class', data.d.Controles[9]);
                                //if (data.d.Controles[10].length) $('#ModGruFamFechaInvalidez_RP').attr('class', data.d.Controles[10]);
                                if (data.d.Controles[11].length) $('#ModGruFamPorcentaje_RP').attr('class', data.d.Controles[11]);
                            }
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            document.location.reload(true);
                        }
                        else {
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al guardar la información del beneficiario.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    },
                    complete: function () {
                        ModGruFamBotonesInactivos = false;
                        $('#ModGruFamCancelarCierre_RP').attr('class', 'boton darkblue sharp');
                        $('#ModGruFamAceptarCierre_RP').attr('class', 'boton darkblue sharp');
                    }
                });
            }



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

            var params = {
                nombres: $('#ModGruFamNombres_RP').val(),
                numSolicitud: $('#ModGruFamsolicitud').val(),
                num_item: 1,
                correo: $("#ModGruFamMail_RP").val(),
                token: $("#HToken").val(),
                numCuspp: $('#HCUSPP_RP').val(),
                codPlan: $('#ModGruPlan').val(),
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
        else if ($("#MCATablaPregunta").val() == "EliminarPersonaVinculada") {
            $("#ModalCuadroAdvertencia").dialog("close");
            $("#MCIcono").attr("class", "cargando");
            $("#MCContenedor").html("Eliminando Persona Vinculada, por favor espere un momento...");
            $("#ModalCotizando").dialog({ title: "Procesando" });
            $("#ModalCotizando").dialog("open");

            var params = {
                tokenUsuario: $("#TokenUsuario").val(),
                idPersonaVinculada: $('#HidPersonaVinculada').val()
            };

            $.ajax({
                type: "POST",
                url: "GrupoFamiliarAfiliadoCierre.aspx/EliminarPersonaVinculada",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params),
                success: function (data) {
                    CargarTablaPersonasVinculadas_RP($("#ModGruFamsolicitud").val())
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                        document.location.reload(true);
                    }
                    else {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html('Ha ocurrido un error al eliminar a la Persona Vincualda.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                },
                complete: () => {
                    $("#ModalCotizando").dialog("close");
                }
            });
        }
    });

    /* Botón NO Beneficiario */
    $('#MCANoBeneficiario').live('click', function () {
        $('#ModalCuadroAdvertencia').dialog('close');
    });

    /* Botón Modificar */
    $('#TablaPersonasVinculadasPEPContenedor_RP .grilla_editar').live('click', function () {
        $('#ModGruFamModo_RPP').val('M');

        LimpiarFormularioPersonasVinculadas();

        $('#ModGruFamCargando_RP').show();
        $('#ModalGrupoFamiliar_RP').dialog('open');

        idPersonaVinculada = $(this).data('idpersonavinculada');
        $('#HidPersonaVinculada').val(idPersonaVinculada);

        var params = {
            idPersonaVinculada: idPersonaVinculada
        }

        $.ajax({
            type: 'POST',
            url: 'GrupoFamiliarAfiliadoCierre.aspx/ObtenerPersonaVinculada',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                var grupo = data.d;
                // Apellido Paterno
                $('#ModGruFamApellidoPaterno_RPP').val(grupo.ApellidoPaterno);
                // Apellido Materno
                $('#ModGruFamApellidoMaterno_RPP').val(grupo.ApellidoMaterno);
                // Nombres
                $('#ModGruFamNombres_RPP').val(grupo.Nombre);
                // Tipo de Identificación
                $('#ModGruFamTipoIdentificacion_RPP').val(grupo.Identificacion.IdTipo);
                $('#TexModGruFamTipoIdentificacion_RPP').html($('#ModGruFamTipoIdentificacion_RPP').find(':selected').text());
                // Nro. de Identificación
                $('#ModGruFamNumeroIdentificacion_RPP').val(grupo.Identificacion.Numero);
                // Parentesco
                $('#ModGruFamParentesco_RPP').val(grupo.Parentesco.Id);
                $('#TexModGruFamParentesco_RPP').html($('#ModGruFamParentesco_RPP').find(':selected').text());
                if (grupo.Parentesco.Id == '80') {
                    $('#ModGruFamParentesco_RPP').attr('disabled', 'disabled');
                    $('#ConModGruFamParentesco_RPP').addClass('formComboboxReadOnlyContenedor');
                }
                else {
                    $('#ModGruFamParentesco_RPP').removeAttr('disabled');
                    $('#ConModGruFamParentesco_RPP').removeClass('formComboboxReadOnlyContenedor');
                }

                $('#ModGruFamApellidoPaterno_RPP').focus();
                $('#ModGruFamCargando_RP').fadeOut();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la persona vincualda.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                $('#ModalGrupoFamiliar_RP').dialog('close');
            }
        });
    });

    /* Botón Eliminar */
    $("#TablaPersonasVinculadasPEPContenedor_RP .grilla_eliminar").live("click", function () {
        $("#ModGruFamModo_RPP").val("E");

        $("#MCATablaPregunta").val("EliminarPersonaVinculada");
        $("#HidPersonaVinculada").val($(this).data("idpersonavinculada"));

        $("#MCAIcono").attr("class", "advertencia");
        var mensaje = "<p>¿Confima que desea eliminar a <b>" + $(this).data("nombrepersonavinculada") + "</b> de la lista de Personas Vinculadas?</p>";
        $("#MCAContenedor").html(mensaje);
        $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
        $("#ModalCuadroAdvertencia").dialog("open");
    });

    /* Botón Cancelar */
    $('#ModGruFamCancelar_RPP').live('click', function () {
        $('#ModalGrupoFamiliar_RP').dialog('close');
    });

    /* Botón Aceptar */
    $('#ModGruFamAceptar_RPP').live('click', function () {
        if (!ModGruFamBotonesInactivos) {
            var esCorrecto = true;
            var errores = new Array();

            $('#ModGruFamApellidoPaterno_RPP').removeClass('formTextboxError');
            $('#ModGruFamApellidoMaterno_RPP').removeClass('formTextboxError');
            $('#ModGruFamNombres_RPP').removeClass('formTextboxError');
            $('#ConModGruFamTipoIdentificacion_RPP').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamNumeroIdentificacion_RPP').removeClass('formTextboxError');
            $('#ConModGruFamParentesco_RPP').removeClass('formComboboxErrorContenedor');

            // Apellido Paterno
            var apellidoPaterno = true;

            // Apellido Materno
            var apellidoMaterno = true;

            // Nombres
            var nombres = true;

            // Tipo de Indentificación
            var tipoIdentificacion = true;

            // Número de Identificación
            var numeroIdentificacion = true;

            // Parentesco
            var parentesco = true;
            if ($('#ModGruFamParentesco_RPP').val() == '0') {
                errores.push('Ingrese el campo <strong>Parentesco</strong>. Dato Obligatorio.');
                parentesco = false;
            }

            if (!apellidoPaterno) $('#ModGruFamApellidoPaterno_RPP').addClass('formTextboxError');
            if (!apellidoMaterno) $('#ModGruFamApellidoMaterno_RPP').addClass('formTextboxError');
            if (!nombres) $('#ModGruFamNombres_RPP').addClass('formTextboxError');
            if (!tipoIdentificacion) $('#ConModGruFamTipoIdentificacion_RPP').addClass('formComboboxErrorContenedor');
            if (!numeroIdentificacion) $('#ModGruFamNumeroIdentificacion_RPP').addClass('formTextboxError');
            if (!parentesco) $('#ConModGruFamParentesco_RPP').addClass('formComboboxErrorContenedor');

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco;

            if (!esCorrecto) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }

            /* Pasó las validaciones */
            ModGruFamBotonesInactivos = true;
            $('#ModGruFamAceptar_RPP').attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#ModGruFamCargando_RPP').fadeIn();

            if ($('#ModGruFamModo_RPP').val() == 'N') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    numSolicitud: $('#ModGruFamsolicitud').val(),
                    apellidoPaterno: ($('#ModGruFamApellidoPaterno_RPP').length > 0) ? $('#ModGruFamApellidoPaterno_RPP').val() : '',
                    apellidoMaterno: ($('#ModGruFamApellidoMaterno_RPP').length > 0) ? $('#ModGruFamApellidoMaterno_RPP').val() : '',
                    nombres: ($('#ModGruFamNombres_RPP').length > 0) ? $('#ModGruFamNombres_RPP').val() : '',
                    tipoIdentificacion: ($('#ModGruFamTipoIdentificacion_RPP').length > 0) ? $('#ModGruFamTipoIdentificacion_RPP').val() : '',
                    numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion_RPP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RPP').val() : '',
                    parentesco: $('#ModGruFamParentesco_RPP').val()
                }
                var postUrl = 'GrupoFamiliarAfiliadoCierre.aspx/InsertarPersonaVinculada';
            }
            else if ($('#ModGruFamModo_RPP').val() == 'M') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    idPersonaVinculada: $('#HidPersonaVinculada').val(),
                    numSolicitud: $('#ModGruFamsolicitud').val(),
                    apellidoPaterno: ($('#ModGruFamApellidoPaterno_RPP').length > 0) ? $('#ModGruFamApellidoPaterno_RPP').val() : '',
                    apellidoMaterno: ($('#ModGruFamApellidoMaterno_RPP').length > 0) ? $('#ModGruFamApellidoMaterno_RPP').val() : '',
                    nombres: ($('#ModGruFamNombres_RPP').length > 0) ? $('#ModGruFamNombres_RPP').val() : '',
                    tipoIdentificacion: ($('#ModGruFamTipoIdentificacion_RPP').length > 0) ? $('#ModGruFamTipoIdentificacion_RPP').val() : '',
                    numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion_RPP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RPP').val() : '',
                    parentesco: $('#ModGruFamParentesco_RPP').val()
                }
                var postUrl = 'GrupoFamiliarAfiliadoCierre.aspx/ModificarPersonaVinculada';
            }
            $.ajax({
                type: 'POST',
                url: postUrl,
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    if (data.d.Estado == 'OK') {
                        /* Cerrar la ventana popup */
                        $('#ModalGrupoFamiliar_RP').dialog('close');

                        /* Recargar la grilla de personas vinculadas */
                        CargarTablaPersonasVinculadas_RP($('#ModGruFamsolicitud').val())

                        /*<SRIINI17003>*/
                        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                        $clock.countdown(selectedDate.toString());
                        /*<SRIFIN17003>*/
                    }
                    else if (data.d.Estado == 'TOKEN') {
                        CerrarSesionExpirada();
                    }
                    else {
                        $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                        $('#MCMIcono').attr('class', data.d.Icono);
                        $('#MCMContenedor').html(data.d.Mensaje);
                        if (data.d.Controles != null) {
                            if (data.d.Controles[0].length) $('#ModGruFamApellidoPaterno_RPP').attr('class', data.d.Controles[0]);
                            if (data.d.Controles[1].length) $('#ModGruFamApellidoMaterno_RPP').attr('class', data.d.Controles[1]);
                            if (data.d.Controles[2].length) $('#ModGruFamNombres_RPP').attr('class', data.d.Controles[2]);
                            if (data.d.Controles[3].length) $('#ConModGruFamTipoIdentificacion_RPP').attr('class', data.d.Controles[3]);
                            if (data.d.Controles[4].length) $('#ModGruFamNumeroIdentificacion_RPP').attr('class', data.d.Controles[4]);
                            if (data.d.Controles[5].length) $('#ConModGruFamParentesco_RPP').attr('class', data.d.Controles[5]);
                        }
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
                        $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la persona vinculada PEP.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                },
                complete: function () {
                    $('#ModGruFamCargando_RPP').fadeOut();
                    ModGruFamBotonesInactivos = false;
                    $('#ModGruFamAceptar_RPP').attr('class', 'boton darkblue sharp');
                }
            });
        }
    });

    function LimpiarFormularioPersonasVinculadas() {
        $('#ModGruFamApellidoPaterno_RPP').removeClass('formTextboxError');
        $('#ModGruFamApellidoMaterno_RPP').removeClass('formTextboxError');
        $('#ModGruFamNombres_RPP').removeClass('formTextboxError');
        $('#ConModGruFamTipoIdentificacion_RPP').removeClass('formComboboxErrorContenedor');
        $('#ModGruFamNumeroIdentificacion_RPP').removeClass('formTextboxError');
        $('#ConModGruFamParentesco_RPP').removeClass('formComboboxErrorContenedor formComboboxReadOnlyContenedor');

        $('#ModGruFamApellidoPaterno_RPP').val('');
        $('#ModGruFamApellidoMaterno_RPP').val('');
        $('#ModGruFamNombres_RPP').val('');
        $('#ModGruFamTipoIdentificacion_RPP').val('0');
        $('#TexModGruFamTipoIdentificacion_RPP').html($('#ModGruFamTipoIdentificacion_RPP').find(':selected').text());
        $('#ModGruFamNumeroIdentificacion_RPP').val('');
        $('#ModGruFamParentesco_RPP').removeAttr('disabled');
        $('#ModGruFamParentesco_RPP').val('0');
        $('#TexModGruFamParentesco_RPP').html($('#ModGruFamParentesco_RPP').find(':selected').text());
    }

    if ($('#ModGruFamModo_RP').val() == "MC") {
        CargandoGrupoFamiliarCierre();
    }

    if ($('#ModGruFamModo_RP').val() == "NC") {
        $('#ModGruFamLineaBanco_RP').hide();
        $('#ModGruFamLineaNroBanco_RP').hide();
        $('#ModGruFamLineaComunicacion_RP').hide();
        $('#ModGruFamLineaInvalidez_RP').hide();
        $('#ModGruFamLineaFechaInvalidez_RP').hide();
        $('#ModGruFamIndInvalidez_RP').val('N');
        $('#ModGruFamTipoInvalidez_RP').val('N');

        //OCULTA NUEVOS CAMPOS
        $('#ModGruFamLineaEstadocivilMail_RP').hide();
        $('#ModGruFamLineaCentrolaboralCargo_RP').hide();
        $('#ModGruFamLineaActividadEconomica_RP').hide();
        $('#ModGruFamLineaMonedaingresoIngreso_RP').hide();
        $('#ModGruFamLineaTelefonoCelular_RP').hide();
        //OCULTA NUEVOS CAMPOS

        if ($('#ModGruFamCantidad').val() == $('#ModGruFamTotal').val()) {
            if ($('#ModGruPlan').val() == 'PLAN1') {
                $('#ModGruFamAceptarCierre_RP').html('Enviar al cliente');
                $('#ModVistaPrevia').show();
            } else if ($('#ModGruPlan').val() == 'PLAN2') {
                $('#ModGruFamAceptarCierre_RP').html('Siguiente');
                $('#ModVistaPrevia').hide();
            } else {
                //if ($('#ModGruFamCantidad').val() == 1) {
                //    $('#ModGruFamAceptarCierre_RP').html('Enviar al cliente');
                //    $('#ModVistaPrevia').show();
                //} else {
                    $('#ModGruFamAceptarCierre_RP').html('Siguiente');
                    $('#ModVistaPrevia').hide();
                //}
            }
        }

        if ($('#ModGruFamCantidad').val() == 1) {
            $('#ModGruFamCancelarCierre_RP').hide();
        }

        $('#ModIdGrupoFamiliar').val($('#ModGruFamCantidad').val());

        $('#ModGruFamApellidoPaterno_RP').focus();

        $('#ModGruFamCancelarCierre_RP').removeClass('botonDeshabilitado gris gris_sharp');
        $('#ModGruFamAceptarCierre_RP').removeClass('botonDeshabilitado gris gris_sharp');

        $('#ModGruFamCancelarCierre_RP').addClass('boton darkblue sharp');
        $('#ModGruFamAceptarCierre_RP').addClass('boton darkblue sharp');

        var params = {
            idGrupoFamiliar: $('#ModIdGrupoFamiliar').val()
        }

        $.ajax({
            type: 'POST',
            url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ExisteGrupoFamiliarSession',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d == 'TRUE') {
                    CargandoGrupoFamiliarCierre();
                } else {
                    //$('#pasos').append("<br/>Beneficiario " + $('#ModGruFamCantidad').val() + " de " + $('#ModGruFamTotal').val())
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
    };

    //<INI.GTI_7012_30>
    $('#ModGruFamTipoIdentificacion_RP').on('change', function () {
        var responseId = $(this).val();
        if (responseId === "D") {
            $("#ModGruFamNumeroIdentificacion_RP").attr('maxlength', 8);

            var str = $('#ModGruFamNumeroIdentificacion_RP').val();
            var n = str.substr(0, 8);
            $('#ModGruFamNumeroIdentificacion_RP').val(n);

        }
        else {
            $("#ModGruFamNumeroIdentificacion_RP").attr('maxlength', 9);
        }

    });
    //<FIN.GTI_7012_30>

    /* Botón Cancelar */
    $('#ModGruFamCancelar_RP').live('click', function () {
        postBack($('#ModSolCancelar_RP').val());
        //window.location.href = "Cotizador.aspx#datos_solicitud";
    });

    $('#ModGruFamPEP_RP').change(function () {
        if ($('#ModGruFamPEP_RP').val() == 'S'){
            $('#seccionPEP').show();
            CargarTablaPersonasVinculadas_RP($('#ModGruFamsolicitud').val())
        }
        else{
            $('#seccionPEP').hide();
        }
    });

    //botón vista previa
    $('#ModVistaPrevia').live('click', function () {

        if (!ModGruFamBotonesInactivos) {
            var esCorrecto = true;
            var errores = new Array();

            $('#ModGruFamApellidoPaterno_RP').removeClass('formTextboxError');
            $('#ModGruFamApellidoMaterno_RP').removeClass('formTextboxError');
            $('#ModGruFamNombres_RP').removeClass('formTextboxError');
            $('#ConModGruFamTipoIdentificacion_RP').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamNumeroIdentificacion_RP').removeClass('formTextboxError');
            $('#ConModGruFamParentesco_RP').removeClass('formComboboxErrorContenedor');
            $('#ConModGruFamSexo_RP').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamFechaNacimiento_RP').removeClass('formTextboxError formCalendarError');
            //$('#ConModGruFamIndInvalidez_RP').removeClass('formComboboxErrorContenedor');
            //$('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxErrorContenedor');
            //$('#ModGruFamFechaInvalidez_RP').removeClass('formTextboxError formCalendarError');
            $('#ConModGruFamNacional_RP').removeClass('formComboboxErrorContenedor');
            $('#ConModGruFamProfesion_RP').removeClass('formComboboxErrorContenedor');
            $('#ConModGruFamResidencia_RP').removeClass('formComboboxErrorContenedor');
            $('#ConModGruFamPEP_RP').removeClass('formComboboxErrorContenedor');
            $('#ConModGruFamSO_RP').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamBanco_RP').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamTipoCtaBanco_RP').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamComunicacion_RP').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamNumeroBanco_RP').removeClass('formTextboxError');
            $('#ModGruFamPorcentaje_RP').removeClass('formTextboxError');
            $('#ModGruFamMail_RP').removeClass('formTextboxError');

            $('#ModGruFamEstadoCivil_RP').removeClass('formTextboxError');
            //$('#ModGruFamTelefono_RP').removeClass('formTextboxError');
            $('#ModGruFamCelular_RP').removeClass('formTextboxError');
            $('#ModGruFamCentrolaboral_RP').removeClass('formTextboxError');
            $('#ModGruFamCargo_RP').removeClass('formTextboxError');
            $('#ModGruFamActividadeconomica_RP').removeClass('formTextboxError');
            $('#ModGruFamMonedaingreso_RP').removeClass('formTextboxError');
            $('#ModGruFamIngreso_RP').removeClass('formTextboxError');
            $('#ModGruFamDestinoFondos_RP').removeClass('formTextboxError');

            // Apellido Paterno
            var apellidoPaterno = true;
            if ($.trim($('#ModGruFamApellidoPaterno_RP').val()).length == 0) {
                errores.push('Ingrese el campo <strong>Apellido Paterno</strong>. Dato Obligatorio.');
                apellidoPaterno = false;
            }

            // Apellido Materno
            var apellidoMaterno = true;
            if ($.trim($('#ModGruFamApellidoMaterno_RP').val()).length == 0) {
                errores.push('Ingrese el campo <strong>Apellido Materno</strong>. Dato Obligatorio.');
                apellidoMaterno = false;
            }

            // Nombres
            var nombres = true;
            if ($.trim($('#ModGruFamNombres_RP').val()).length == 0) {
                errores.push('Ingrese el campo <strong>Nombre</strong>. Dato Obligatorio.');
                nombres = false;
            }

            // Tipo de Indentificación
            var tipoIdentificacion = true;
            if ($('#ModGruFamTipoIdentificacion_RP').val() == '0') {
                errores.push('Ingrese el campo <strong>Tipo de Identifación</strong>. Dato Obligatorio.');
                tipoIdentificacion = false;
            }

            // Número de Identificación
            var numeroIdentificacion = true;
            if ($.trim($('#ModGruFamNumeroIdentificacion_RP').val()).length == 0) {
                errores.push('Ingrese el campo <strong>Numero de Identificación</strong>. Dato Obligatorio.');
                numeroIdentificacion = false;
            }

            // Número de Identificación longitud
            var numeroIdentificacionlongitud = true;
            if ($('#ModGruFamTipoIdentificacion_RP').val() == 'D') {

                if ($.trim($('#ModGruFamNumeroIdentificacion_RP').val()).length < 8) {
                    errores.push('Ingrese minimo 8 digitos <strong>Numero de Identificación</strong>. Dato Obligatorio.');
                    numeroIdentificacionlongitud = false;
                }
            }

            // Parentesco
            var parentesco = true;
            if ($('#ModGruFamParentesco_RP').val() == '0') {
                errores.push('Ingrese el campo <strong>Parentesco</strong>. Dato Obligatorio.');
                parentesco = false;
            }

            // Sexo
            var sexo = true;
            if ($('#ModGruFamSexo_RP').val() == '0') {
                errores.push('Ingrese el campo <strong>Sexo</strong>. Dato Obligatorio.');
                sexo = false;
            }

            // Fecha de Nacimiento
            var fechaNacimiento = true;
            if ($.trim($('#ModGruFamFechaNacimiento_RP').val()).length == 0) {
                errores.push('Ingrese el campo <strong>Fecha de Nacimiento</strong>. Dato Obligatorio.');
                fechaNacimiento = false;
            }

            //// Invalidez
            //var invalidez = true;
            //var tipoInvalidez = true;
            //var fechaInvalidez = true;
            //if ($('#ModGruFamIndInvalidez_RP').val() == '0') {
            //    errores.push('Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.');
            //    invalidez = false;
            //}
            //else if ($('#ModGruFamIndInvalidez_RP').val() == 'N') {
                //if ($('#ModGruFamTipoInvalidez_RP').val() != 'N') {
                //    errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                //    tipoInvalidez = false;
                //}
                //if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length > 0) {
                //    errores.push('El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.');
                //    fechaInvalidez = false;
                //}
            //}
            //else if ($('#ModGruFamIndInvalidez_RP').val() == 'S') {
            //    if ($('#ModGruFamTipoInvalidez_RP').val() != 'P' && $('#ModGruFamTipoInvalidez_RP').val() != 'T') {
            //        errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
            //        tipoInvalidez = false;
            //    }

            //    if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length == 0) {
            //        errores.push('Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.');
            //        fechaInvalidez = false;
            //    }
            //}

            //if ($('#ModGruFamTipoInvalidez_RP').val() == '0') {
            //    errores.push('Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.');
            //    tipoInvalidez = false;
            //}

            var nacionalidad = true;
            if ($('#ModGruFamNacional_RP').val() == '0') {
                errores.push('Seleccioné la <strong>Nacionalidad</strong>. Dato Obligatorio.');
                nacionalidad = false;
            }

            var profesion = true;
            if ($('#ModGruFamProfesion_RP').val() == '0') {
                errores.push('Seleccioné la <strong>Profesión</strong>. Dato Obligatorio.');
                profesion = false;
            }

            var residencia = true;
            if ($('#ModGruFamResidencia_RP').val() == '0') {
                errores.push('Seleccioné la <strong>Residencia</strong>. Dato Obligatorio.');
                residencia = false;
            }

            var pep = true;
            if ($('#ModGruFamPEP_RP').val() == '0') {
                errores.push('Seleccioné si <strong>¿Es Personas Expuestas Políticamente (PEP)?</strong>. Dato Obligatorio.');
                pep = false;
            }

            var sujetoObligado = true;
            if ($('#ModGruFamSO_RP').val() == '0') {
                errores.push('Seleccioné si <strong>¿Es Sujeto Obligado?</strong>. Dato Obligatorio.');
                sujetoObligado = false;
            }

            var Banco = true;
            var TipoBanco = true;
            var Comunicacion = true;
            var NumeroBanco = true;
            var Mail = true;

            var estadoCivil = true;
            var centrolaboral = true;
            var cargo = true;
            var actividadeconomica = true;
            var monedaingreso = true;
            var ingreso = true;
            //var telefono = true;
            var celular = true;
            var origenFondo = true;

            if ($('#ModGruFamParentesco_RP').val() == '80') {

                if ($('#ModGruFamBanco_RP').val() == '0') {
                    errores.push('Seleccioné el <strong>Banco</strong>. Dato Obligatorio.');
                    Banco = false;
                }

                if ($('#ModGruFamTipoCtaBanco_RP').val() == '0') {
                    errores.push('Seleccioné el <strong>Tipo de Cuenta</strong>. Dato Obligatorio.');
                    TipoBanco = false;
                }

                if ($('#ModGruFamComunicacion_RP').val() == '0') {
                    errores.push('Seleccioné el <strong>Mecanismo de comunicación</strong>. Dato Obligatorio.');
                    Comunicacion = false;
                }

                if ($.trim($('#ModGruFamNumeroBanco_RP').val()).length == 0) {
                    errores.push('Ingrese el <strong>Nro. de cuenta</strong>. Dato Obligatorio.');
                    NumeroBanco = false;
                } else {
                    if ($('#ModGruFamBanco_RP').val() == '0') {
                        if (Banco) {
                            errores.push('Seleccioné el <strong>Banco</strong>. Dato Obligatorio.');
                            Banco = false;
                        }
                    }
                    if ($('#ModGruFamTipoCtaBanco_RP').val() == '0') {
                        if (TipoBanco) {
                            errores.push('Seleccioné el <strong>Tipo de Cuenta</strong>. Dato Obligatorio.');
                            TipoBanco = false;
                        }
                    }
                }

                if (!isEmail($('#ModGruFamMail_RP').val())) {
                    errores.push('<strong>Correo Electrónico</strong> inválido. Dato Obligatorio.');
                    Mail = false;
                }

                if ($('#ModGruFamEstadoCivil_RP').val() == '0') {
                    errores.push('Seleccioné el campo <strong>Estado Civil</strong>. Dato Obligatorio.');
                    estadoCivil = false;
                }

                if ($.trim($('#ModGruFamCentrolaboral_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Centro Laboral</strong>. Dato Obligatorio.');
                    centrolaboral = false;
                }

                if ($.trim($('#ModGruFamCargo_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Cargo</strong>. Dato Obligatorio.');
                    cargo = false;
                }

                if ($.trim($('#ModGruFamActividadeconomica_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Actividad Económica</strong>. Dato Obligatorio.');
                    actividadeconomica = false;
                }

                if ($('#ModGruFamMonedaingreso_RP').val() == '0') {
                    errores.push('Seleccioné el campo <strong>Moneda de Ingreso</strong>. Dato Obligatorio.');
                    monedaingreso = false;
                }

                if ($.trim($('#ModGruFamIngreso_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Ingreso Neto</strong>. Dato Obligatorio.');
                    ingreso = false;
                }

                //if ($.trim($('#ModGruFamTelefono_RP').val()).length == 0) {
                //    errores.push('Ingrese el campo <strong>Teléfono</strong>. Dato Obligatorio.');
                //    telefono = false;
                //}

                if ($.trim($('#ModGruFamCelular_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Celular</strong>. Dato Obligatorio.');
                    celular = false;
                }

                if ($.trim($('#ModGruFamDestinoFondos_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Descripción jurada de origen y/o destino de fondos</strong>. Dato Obligatorio.');
                    origenFondo = false;
                }
            }

            var beneficiarioCA = false;
            if ($('#HSolicitudSerializado') != undefined) {
                var lstBeneficiarios = JSON.parse($('#HSolicitudSerializado').val()).Beneficiarios;
                for (var b in lstBeneficiarios) {
                    if (lstBeneficiarios[b].Id == $('#ModIdGrupoFamiliar').val()) {
                        if (lstBeneficiarios[b].TipoCobertura == 'CA') beneficiarioCA = true;
                    }
                }
            }

            var porcentajeRenta = true;
            if (!beneficiarioCA) {
                if ($('#ModGruFamPorcentaje_RP').val() == '0' || $('#ModGruFamPorcentaje_RP').val() == '') {
                    errores.push('Ingrese el campo <strong>Porcentaje (%)</strong>. Dato Obligatorio.');
                    porcentajeRenta = false;
                }
            }

            if (!apellidoPaterno) $('#ModGruFamApellidoPaterno_RP').addClass('formTextboxError');
            if (!apellidoMaterno) $('#ModGruFamApellidoMaterno_RP').addClass('formTextboxError');
            if (!nombres) $('#ModGruFamNombres_RP').addClass('formTextboxError');
            if (!tipoIdentificacion) $('#ConModGruFamTipoIdentificacion_RP').addClass('formComboboxErrorContenedor');
            if (!numeroIdentificacion) $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxError');
            if (!numeroIdentificacionlongitud) $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxError'); //<GTI_7012_30>
            if (!parentesco) $('#ConModGruFamParentesco_RP').addClass('formComboboxErrorContenedor');
            if (!sexo) $('#ConModGruFamSexo_RP').addClass('formComboboxErrorContenedor');
            if (!fechaNacimiento) $('#ModGruFamFechaNacimiento_RP').addClass('formTextboxError formCalendarError');
            //if (!invalidez) $('#ConModGruFamIndInvalidez_RP').addClass('formComboboxErrorContenedor');
            //if (!tipoInvalidez) $('#ConModGruFamTipoInvalidez_RP').addClass('formComboboxErrorContenedor');
            //if (!fechaInvalidez) $('#ModGruFamFechaInvalidez_RP').addClass('formTextboxError formCalendarError');
            if (!nacionalidad) $('#ConModGruFamNacional_RP').addClass('formComboboxErrorContenedor');
            if (!profesion) $('#ConModGruFamProfesion_RP').addClass('formComboboxErrorContenedor');
            if (!residencia) $('#ConModGruFamResidencia_RP').addClass('formComboboxErrorContenedor');
            if (!pep) $('#ConModGruFamPEP_RP').addClass('formComboboxErrorContenedor');
            if (!sujetoObligado) $('#ConModGruFamSO_RP').addClass('formComboboxErrorContenedor');
            if (!Banco) $('#ConModGruFamBanco_RP').addClass('formComboboxErrorContenedor');
            if (!TipoBanco) $('#ConModGruFamTipoCtaBanco_RP').addClass('formComboboxErrorContenedor');
            if (!Comunicacion) $('#ConModGruFamComunicacion_RP').addClass('formComboboxErrorContenedor');
            if (!NumeroBanco) $('#ModGruFamNumeroBanco_RP').addClass('formTextboxError');
            if (!porcentajeRenta) $('#ModGruFamPorcentaje_RP').addClass('formTextboxError');
            if (!Mail) $('#ModGruFamMail_RP').addClass('formTextboxError');

            if (!estadoCivil) $('#ConModGruFamEstadoCivil_RP').addClass('formComboboxErrorContenedor');
            if (!centrolaboral) $('#ModGruFamCentrolaboral_RP').addClass('formTextboxError');
            if (!cargo) $('#ModGruFamCargo_RP').addClass('formTextboxError');
            if (!actividadeconomica) $('#ModGruFamActividadeconomica_RP').addClass('formTextboxError');
            if (!monedaingreso) $('#ConModGruFamMonedaingreso_RP').addClass('formComboboxErrorContenedor');
            if (!ingreso) $('#ModGruFamIngreso_RP').addClass('formTextboxError');
            //if (!telefono) $('#ModGruFamTelefono_RP').addClass('formTextboxError');
            if (!celular) $('#ModGruFamCelular_RP').addClass('formTextboxError');
            if (!origenFondo) $('#ModGruFamDestinoFondos_RP').addClass('formTextboxError');

            //& invalidez & tipoInvalidez & fechaInvalidez 

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & numeroIdentificacionlongitud
                & parentesco & sexo & fechaNacimiento & nacionalidad & profesion & residencia & pep
                & sujetoObligado & Banco & TipoBanco & Comunicacion & NumeroBanco & porcentajeRenta & Mail & estadoCivil & centrolaboral & cargo
                & actividadeconomica & monedaingreso & ingreso & celular & origenFondo;

            if (!esCorrecto) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }

            if ($('#ModGruFamParentesco_RP').val() != '80') {
                $('#ModGruFamConfidencialidadDatos_RP').val('1')
            }

            $("#MCIcono").attr("class", "cargando");
            $("#MCContenedor").html("Validando beneficiario, por favor espere un momento...");
            $("#ModalCotizando").dialog({ title: "Validando" });
            $("#ModalCotizando").dialog("open");

            var str_estadoCivil = "";
            if ($('#ModGruFamEstadoCivil_RP').val() == null) {
                str_estadoCivil = "";
            } else {
                str_estadoCivil = $('#ModGruFamEstadoCivil_RP').val();
            }

            var str_correoElectronico = "";
            if ($('#ModGruFamMail_RP').val() == null) {
                str_correoElectronico = "";
            } else {
                str_correoElectronico = $('#ModGruFamMail_RP').val();
            }

            var str_centrolaboral = "";
            if ($('#ModGruFamCentrolaboral_RP').val() == null) {
                str_centrolaboral = "";
            } else {
                str_centrolaboral = $('#ModGruFamCentrolaboral_RP').val();
            }

            var str_cargo = "";
            if ($('#ModGruFamCargo_RP').val() == null) {
                str_cargo = "";
            } else {
                str_cargo = $('#ModGruFamCargo_RP').val();
            }

            var str_actividadeconomica = "";
            if ($('#ModGruFamActividadeconomica_RP').val() == null) {
                str_actividadeconomica = "";
            } else {
                str_actividadeconomica = $('#ModGruFamActividadeconomica_RP').val();
            }

            var str_monedaingreso = "";
            if ($('#ModGruFamMonedaingreso_RP').val() == null) {
                str_monedaingreso = "";
            } else {
                str_monedaingreso = $('#ModGruFamMonedaingreso_RP').val();
            }

            var str_ingreso = "";
            if ($('#ModGruFamIngreso_RP').val() == null) {
                str_ingreso = "";
            } else {
                str_ingreso = $('#ModGruFamIngreso_RP').val();
            }

            var str_telefono = "";
            if ($('#ModGruFamTelefono_RP').val() == null) {
                str_telefono = "";
            } else {
                str_telefono = $('#ModGruFamTelefono_RP').val();
            }

            var str_celular = "";
            if ($('#ModGruFamCelular_RP').val() == null) {
                str_celular = "";
            } else {
                str_celular = $('#ModGruFamCelular_RP').val();
            }

            var str_origen_fondo = "";
            if ($('#ModGruFamDestinoFondos_RP').val() == null) {
                str_origen_fondo = "";
            } else {
                str_origen_fondo = $('#ModGruFamDestinoFondos_RP').val();
            }

            var postUrl = '';
            var params = '';

            if ($('#ModGruFamParentesco_RP').val() == '80') {

                params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    idGrupoFamiliar: $('#ModIdGrupoFamiliar').val(),
                    cuspp: $('#HCUSPP_RP').val(),
                    apellidoPaterno: ($('#ModGruFamApellidoPaterno_RP').length > 0) ? $('#ModGruFamApellidoPaterno_RP').val() : '',
                    apellidoMaterno: ($('#ModGruFamApellidoMaterno_RP').length > 0) ? $('#ModGruFamApellidoMaterno_RP').val() : '',
                    nombres: ($('#ModGruFamNombres_RP').length > 0) ? $('#ModGruFamNombres_RP').val() : '',
                    tipoIdentificacion: ($('#ModGruFamTipoIdentificacion_RP').length > 0) ? $('#ModGruFamTipoIdentificacion_RP').val() : '',
                    numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion_RP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RP').val() : '',
                    numeroIdentificacionlongitud: ($('#ModGruFamNumeroIdentificacion_RP').length > 0) ? $('#ModGruFamNumeroIdentificacion_RP').val() : '', //<GTI_7012_30>
                    parentesco: $('#ModGruFamParentesco_RP').val(),
                    sexo: $('#ModGruFamSexo_RP').val(),
                    fechaNacimiento: $('#ModGruFamFechaNacimiento_RP').val(),
                    invalidez: $('#ModGruFamIndInvalidez_RP').val(),
                    tipoInvalidez: $('#ModGruFamTipoInvalidez_RP').val(),
                    fechaInvalidez: $('#ModGruFamFechaInvalidez_RP').val(),
                    PEP: $('#ModGruFamPEP_RP').val(),
                    sujetoObligado: $('#ModGruFamSO_RP').val(),
                    nacionalidad: $('#ModGruFamNacional_RP').val(),
                    profesion: $('#ModGruFamProfesion_RP').val(),
                    residencia: $('#ModGruFamResidencia_RP').val(),
                    banco: $('#ModGruFamBanco_RP').val(),
                    tipoBanco: $('#ModGruFamTipoCtaBanco_RP').val(),
                    comunicacion: $('#ModGruFamComunicacion_RP').val(),
                    numerobanco: $('#ModGruFamNumeroBanco_RP').val(),
                    confidencialidadDatos: $('#ModGruFamConfidencialidadDatos_RP').val(),
                    flagRenta: 'particularplus',
                    pjeRenta: $('#ModGruFamPorcentaje_RP').val(),
                    estadoCivil: str_estadoCivil,
                    correoElectronico: str_correoElectronico,
                    centrolaboral: str_centrolaboral,
                    cargo: str_cargo,
                    actividadeconomica: str_actividadeconomica,
                    monedaingreso: str_monedaingreso,
                    ingreso: str_ingreso,
                    telefono: str_telefono,
                    celular: str_celular,
                    origen_fondo: str_origen_fondo
                }

                postUrl = '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ModificarGrupoFamiliar';
            } 

            $.ajax({
                type: 'POST',
                url: postUrl,
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    $('#ModalCotizando').dialog('close');
                    if (data.d.Estado == 'OK') {


                        $("#MCIcono").attr("class", "cargando");
                        $("#MCContenedor").html("Generando reporte, por favor espere un momento...");
                        $("#ModalCotizando").dialog({ title: "Cargando" });
                        $("#ModalCotizando").dialog("open");

                        var params = {
                            tokenUsuario: $("#TokenUsuario").val(),
                            numSolicitud: $('#ModGruFamsolicitud').val(),
                            numCorrelativo: $('#ModGruNumCorrelativo').val(),
                            numCuspp: $('#HCUSPP_RP').val(),
                            indPEP: $('#ModGruFamPEP_RP').find(':selected').text(),
                            codPlan: $('#ModGruPlan').val(),
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

                                    const cuspp = $("#HCUSPP_RP").val();
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
                        if (data.d.Controles != null) {
                            if (data.d.Controles[0].length) $('#ModGruFamApellidoPaterno_RP').attr('class', data.d.Controles[0]);
                            if (data.d.Controles[1].length) $('#ModGruFamApellidoMaterno_RP').attr('class', data.d.Controles[1]);
                            if (data.d.Controles[2].length) $('#ModGruFamNombres_RP').attr('class', data.d.Controles[2]);
                            if (data.d.Controles[3].length) $('#ConModGruFamTipoIdentificacion_RP').attr('class', data.d.Controles[3]);
                            if (data.d.Controles[4].length) $('#ModGruFamNumeroIdentificacion_RP').attr('class', data.d.Controles[4]);
                            if (data.d.Controles[5].length) $('#ConModGruFamParentesco_RP').attr('class', data.d.Controles[5]);
                            if (data.d.Controles[6].length) $('#ConModGruFamSexo_RP').attr('class', data.d.Controles[6]);
                            if (data.d.Controles[7].length) $('#ModGruFamFechaNacimiento_RP').attr('class', data.d.Controles[7]);
                            if (data.d.Controles[8].length) $('#ConModGruFamIndInvalidez_RP').attr('class', data.d.Controles[8]);
                            if (data.d.Controles[9].length) $('#ConModGruFamTipoInvalidez_RP').attr('class', data.d.Controles[9]);
                            if (data.d.Controles[10].length) $('#ModGruFamFechaInvalidez_RP').attr('class', data.d.Controles[10]);
                            if (data.d.Controles[11].length) $('#ModGruFamPorcentaje_RP').attr('class', data.d.Controles[11]);
                        }
                        $('#ModalCuadroMensaje').dialog('open');

                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                        document.location.reload(true);
                    }
                    else {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html('Ha ocurrido un error al guardar la información del beneficiario.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                },
                complete: function () {
                    ModGruFamBotonesInactivos = false;
                }
            });
        }
    });



});

function validarGrupoFamiliar(
    rolAzman,
    glsApellidoPaterno,
    glsApellidoMaterno,
    glsNombres,
    idTipoIdentificacion,
    glsNumeroIdentificacion,
    idParentesco,
    idSexo,
    fecNacimiento,
    idInvalidez,
    idTipoInvalidez,
    fecInvalidez,
    pjeRenta,
    estadoCivil = "0",
    correoElectronico = "",
    centrolaboral = "",
    cargo = "",
    actividadeconomica = "",
    monedaingreso = "0",
    ingreso = "",
    telefono = "",
    celular = "",
    vPjeRenta = true
) {

    const errores = [];
    const controles = [];

    // Apellido Paterno
    let apellidoPaterno = true;

    // Apellido Materno
    let apellidoMaterno = true;

    // Nombres
    let nombres = true;

    // Tipo de Identificación
    let tipoIdentificacion = true;

    // Número de Identificación
    let numeroIdentificacion = true;
    if (glsNumeroIdentificacion.trim().length > 0) {
        if (!/^\d+$/.test(glsNumeroIdentificacion)) {
            errores.push("El campo <strong>Nro. de Identificación</strong> debe contener un valor numérico.");
            numeroIdentificacion = false;
        }
    }

    // Parentesco
    let parentesco = true;
    if (idParentesco === "0") {
        errores.push("Ingrese el campo <strong>Parentesco</strong>. Dato Obligatorio.");
        parentesco = false;
    }

    // Sexo
    let sexo = true;
    if (idSexo === "0") {
        errores.push("Ingrese el campo <strong>Sexo</strong>. Dato Obligatorio.");
        sexo = false;
    }

    // Fecha de Nacimiento
    let fechaNacimiento = true;
    if (fecNacimiento.trim().length === 0) {
        errores.push("Ingrese el campo <strong>Fecha de Nacimiento</strong>. Dato Obligatorio.");
        fechaNacimiento = false;
    } else {
        // Intentar parsear la fecha (asumiendo formato dd/mm/yyyy)
        const fechaParts = fecNacimiento.split('/');
        let vFechaNacimiento = null;

        if (fechaParts.length === 3) {
            const dia = parseInt(fechaParts[0], 10);
            const mes = parseInt(fechaParts[1], 10) - 1; // JavaScript months are 0-based
            const año = parseInt(fechaParts[2], 10);
            vFechaNacimiento = new Date(año, mes, dia);
        }

        if (!vFechaNacimiento || isNaN(vFechaNacimiento.getTime())) {
            errores.push("El campo <strong>Fecha de Nacimiento</strong> debe contener una fecha válida (dd/mm/aaaa).");
            fechaNacimiento = false;
        } else {
            if (vFechaNacimiento > new Date()) {
                errores.push("La <strong>Fecha de Nacimiento</strong> no puede ser mayor al día de hoy.");
                fechaNacimiento = false;
            }
        }
    }

    let bPjeRenta = true;
    if (vPjeRenta) {
        if (pjeRenta === "" || pjeRenta === "0") {
            //errores.push("Ingrese el campo <strong>Porcentaje (%)</strong>. Dato Obligatorio.");
            //bPjeRenta = false;
        } else {
            const pje = parseFloat(pjeRenta);
            if (!isNaN(pje)) {
                if (pje <= 0) {
                    errores.push("Ingrese el campo <strong>Porcentaje (%)</strong>. Dato Obligatorio.");
                    bPjeRenta = false;
                }
            } else {
                errores.push("Ingrese el campo <strong>Porcentaje (%)</strong>. Dato Obligatorio.");
                bPjeRenta = false;
            }
        }
    }

    let bEstadoCivil = true;
    let bCorreoElectronico = true;
    let bCentrolaboral = true;
    let bCargo = true;
    let bActividadeconomica = true;
    let bMonedaingreso = true;
    let bIngreso = true;
    let bCelular = true;

    if (rolAzman === EnumsRoles.AgenteExterno && idParentesco === "80") { // Simplificado para el ejemplo
        if (estadoCivil === "0") {
            errores.push("Seleccione el campo <strong>Estado Civil</strong>. Dato Obligatorio.");
            bEstadoCivil = false;
        }
        if (correoElectronico.trim().length === 0) {
            errores.push("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.");
            bCorreoElectronico = false;
        }
        if (centrolaboral.trim().length === 0) {
            errores.push("Ingrese el campo <strong>Centro Laboral</strong>. Dato Obligatorio.");
            bCentrolaboral = false;
        }
        if (cargo.trim().length === 0) {
            errores.push("Ingrese el campo <strong>Cargo</strong>. Dato Obligatorio.");
            bCargo = false;
        }
        if (actividadeconomica.trim().length === 0) {
            errores.push("Ingrese el campo <strong>Actividad Económica</strong>. Dato Obligatorio.");
            bActividadeconomica = false;
        }
        if (monedaingreso === "0") {
            errores.push("Seleccione el campo <strong>Moneda de Ingreso</strong>. Dato Obligatorio.");
            bMonedaingreso = false;
        }
        if (ingreso.trim().length === 0) {
            errores.push("Ingrese el campo <strong>Ingreso Neto</strong>. Dato Obligatorio.");
            bIngreso = false;
        }
        //if (telefono.trim().length === 0 || telefono.trim().length < 7) {
        //    errores.push("Ingrese el campo <strong>Teléfono</strong>. Dato Obligatorio.");
        //    bTelefono = false;
        //}
        if (celular.trim().length === 0 || celular.trim().length < 9) {
            errores.push("Ingrese el campo <strong>Celular</strong>. Dato Obligatorio.");
            bCelular = false;
        }
    }

    // Clases de controles
    controles.push(!apellidoPaterno ? "formTextbox formTextboxError" : "formTextbox");
    controles.push(!apellidoMaterno ? "formTextbox formTextboxError" : "formTextbox");
    controles.push(!nombres ? "formTextbox formTextboxError" : "formTextbox");
    controles.push(!tipoIdentificacion ? "formComboboxContenedor formComboboxErrorContenedor" : "formComboboxContenedor");
    controles.push(!numeroIdentificacion ? "formTextbox formTextboxError" : "formTextbox");
    controles.push(!parentesco ? "formComboboxContenedor formComboboxErrorContenedor" : "formComboboxContenedor");
    controles.push(!sexo ? "formComboboxContenedor formComboboxErrorContenedor" : "formComboboxContenedor");
    controles.push(!fechaNacimiento ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    //controles.push(!invalidez ? "formComboboxContenedor formComboboxErrorContenedor" : "formComboboxContenedor");
    //controles.push(!tipoInvalidez ? "formComboboxContenedor formComboboxErrorContenedor" : "formComboboxContenedor");
    //controles.push(!fechaInvalidez ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");

    controles.push(!bPjeRenta ? "formTextbox formTextboxError" : "formTextbox");

    controles.push(!bEstadoCivil ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    controles.push(!bCorreoElectronico ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    controles.push(!bCentrolaboral ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    controles.push(!bCargo ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    controles.push(!bActividadeconomica ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    controles.push(!bMonedaingreso ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    controles.push(!bIngreso ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    //controles.push(!bTelefono ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    controles.push(!bCelular ? "formTextbox formCalendar formTextboxError formCalendarError" : "formTextbox formCalendar");
    return { errores, controles };
}


function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
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