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

$(document).ready(function () {
    var permisoBusAfiExaminarSolicitud = ($("#PerBusAfiExaminarSolicitud_RP").val() == "1" ? true : false);
    var permisoBusAfiBuscar = ($("#PerBusAfiBuscar_RP").val() == "1" ? true : false);
    var permisoGuardar = ($("#PerGuardar_RP").val() == "1" ? true : false);
    var permisoNuevaDireccion = ($("#PerNuevaDireccion_RP").val() == "1" ? true : false);
    var permisoNuevoTelefono = ($("#PerNuevoTelefono_RP").val() == "1" ? true : false);
    var permisoNuevoBeneficiario = ($("#PerNuevoBeneficiario_RP").val() == "1" ? true : false);
    var permisoNuevaSolicitud = ($("#PerNuevaSolicitud_RP").val() == "1" ? true : false);

    if ($("#HFirmado").val() == "S") $("#ModSolAceptarCierre_RP").hide();

    /*******************\
    |*    COTIZADOR    *|
    \*******************/

    /* Pestañas del Cotizador */
    /* Cambiar de pestaña */
    $('#Pestanhas li').live('click', function () {
        $('#Pestanhas li').attr('class', '');
        $('#Pestanhas div[id^=Pestanha]').hide();
        $('#Pestanha' + $(this).data('pestanha')).show();
        $(this).attr('class', 'seleccionado');

        /*YR AGREGAR FUNCIONALIDAD PARA DESHABILITAR CONTROLES EN BUSQUEDA POR DOCUMENTO*/
        if ($(this).data('pestanha') != 1) {
            $('#BusAfiBuscar_RP, #BusAfiBuscar2_RP, #BusAfiExaminarSolicitud_RP, #BusAfiExaminarSolicitudBusqueda_RP')
                .attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP, #NumeroDocumentoBusqueda_RP').attr('readonly', true);
            $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP, #NumeroDocumentoBusqueda_RP').addClass('formTextboxReadOnly');

            $('#TipoDocumentoBusqueda_RP').parent().attr('class', 'aspNetDisabledContenedor formComboboxContenedor formComboboxReadOnlyContenedor')
            $('#TipoDocumentoBusqueda_RP').prop('disabled', true);
            botonBusAfiBuscarBloqueado = true;
        }
        else {
            if (permisoBusAfiExaminarSolicitud) $('#BusAfiExaminarSolicitud_RP, #BusAfiExaminarSolicitudBusqueda_RP').attr('class', 'boton darkblue sharp');
            if (permisoBusAfiBuscar) $('#BusAfiBuscar_RP, #BusAfiBuscar2_RP').attr('class', 'boton darkblue sharp');
            $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP, #NumeroDocumentoBusqueda_RP').attr('readonly', false);
            $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP, #NumeroDocumentoBusqueda_RP').removeClass('formTextboxReadOnly');

            $('#TipoDocumentoBusqueda_RP').parent().attr('class', 'formComboboxContenedor');
            $('#TipoDocumentoBusqueda_RP').prop('disabled', false);
            botonBusAfiBuscarBloqueado = false;
        }
    });

    $('#PestanhasBusqueda li').live('click', function () {
        $('#PestanhasBusqueda li').attr('class', '');
        $('#PestanhasBusqueda div[id^=Pestanha]').hide();
        $('#PestanhaBusqueda' + $(this).data('pestanha')).show();
        $(this).attr('class', 'seleccionado');

        if ($(this).data('pestanha') == 1) {
            //$('#NumeroDocumentoBusqueda_RP').val('');
            //$("#TipoDocumentoBusqueda_RP").val(0);
            $('#TexTipoDocumentoBusqueda_RP').html($('#TipoDocumentoBusqueda_RP').find(':selected').text());
            $('#hindPestaniaActiva').val(1);
        } else if ($(this).data('pestanha') == 2) {
            //$('#BusAfiNroSolicitud_RP').val('');
            //$('#BusAfiCUSPP_RP').val('');
            $('#hindPestaniaActiva').val(2);
        }

        //if ($(this).data('pestanha') != 1) {
        //    $('#BusAfiBuscar_RP,#BusAfiExaminarSolicitud_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
        //    $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').attr('readonly', true);
        //    $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').addClass('formTextboxReadOnly');
        //    botonBusAfiBuscarBloqueado = true;
        //}
        //else {
        //    if (permisoBusAfiExaminarSolicitud) $('#BusAfiExaminarSolicitud_RP').attr('class', 'boton darkblue sharp');
        //    if (permisoBusAfiBuscar) $('#BusAfiBuscar_RP').attr('class', 'boton darkblue sharp');
        //    $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').attr('readonly', false);
        //    $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').removeClass('formTextboxReadOnly');
        //    botonBusAfiBuscarBloqueado = false;
        //}

    });

    /* BÚSQUEDA DE AFILIADOS */
    var BusAfiApellidoPaterno;
    var BusAfiApellidoMaterno;
    var BusAfiNombres;
    /* Botón Examinar Solicitud */
    $('#BusAfiExaminarSolicitud_RP').live('click', function (e) {
        if (!botonBusAfiBuscarBloqueado && permisoBusAfiExaminarSolicitud) {
            botonesBloqueados = false;
            LimpiarFormularioBusquedaAfiliados();
            $('#ModalBusquedaAfiliados_RP').dialog('open');
            $('#ModBusAfiApellidoPaterno_RP').focus();
        }
        else {
            return false;
        }
    });

    /* Botón Examinar Inteligo*/
    $('#BusAfiExaminarSolicitudBusqueda_RP').live('click', function (e) {
        //if (!botonBusAfiBuscarBloqueado && permisoBusAfiExaminarSolicitud) {
        //botonesBloqueados = false;
        //LimpiarFormularioBusquedaAfiliados();
        //$('#ModalBusquedaAfiliados_RP').dialog('open');
        //$('#ModBusAfiApellidoPaterno_RP').focus();
        if (!botonBusAfiBuscarBloqueado && permisoBusAfiExaminarSolicitud) {
            botonesBloqueados = false;
            LimpiarFormularioBusquedaAfiliados();
            $('#ModalBusquedaAfiliados_RP').dialog('open');
            $('#ModBusAfiApellidoPaterno_RP').focus();

            $('#TipoDocumentoBusqueda_RP').val(0);
            $('#TexTipoDocumentoBusqueda_RP').html($('#TipoDocumentoBusqueda_RP').find(':selected').text());

            $('#NumeroDocumentoBusqueda_RP').val('');


            $('#TipoDocumento_RP').val(0);
            $('#TexTipoDocumento_RP').html($('#TipoDocumento_RP').find(':selected').text());

            $('#NumeroDocumento_RP').val('');
            $('#ApellidoPaterno_RP').val('');
            $('#ApellidoMaterno_RP').val('');
            $('#Nombres_RP').val('');
            $('#FechaNacimiento_RP').val('');

            $('#Sexo_RP').val(0);
            $('#TexSexo_RP').html($('#Sexo_RP').find(':selected').text());

            $('#CorreoElectronico_RP').val('');

            $('#EstadoCivil_RP').val(0);
            $('#TexEstadoCivil_RP').html($('#EstadoCivil_RP').find(':selected').text());

            $('#Telefono_RP').val('');
            $('#Celular_RP').val('');
        }
        else {
            return false;
        }
    });

    /* Botón Buscar Afiliado */
    $('#ModBusAfiBuscar_RP').live('click', function (e) {
        if (!botonesBloqueados && permisoBusAfiExaminarSolicitud) {
            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            $("#ModBusAfiApellidoPaterno_RP").removeClass("formTextboxError");
            $("#ModBusAfiApellidoMaterno_RP").removeClass("formTextboxError");
            $("#ModBusAfiNombres_RP").removeClass("formTextboxError");

            // Apellido Paterno
            var apellidoPaterno = true;
            var eApellidoPaterno = false;
            if ($.trim($("#ModBusAfiApellidoPaterno_RP").val()).length > 0) {
                eApellidoPaterno = true;
                if ($.trim($("#ModBusAfiApellidoPaterno_RP").val()).length < 2) {
                    errores.push("El campo <strong>Apellido Paterno</strong> debe contener al menos 2 caracteres.");
                    apellidoPaterno = false;
                }
            }

            // Apellido Materno
            var apellidoMaterno = true;
            var eApellidoMaterno = false;
            if ($.trim($("#ModBusAfiApellidoMaterno_RP").val()).length > 0) {
                eApellidoMaterno = true;
                if ($.trim($("#ModBusAfiApellidoMaterno_RP").val()).length < 2) {
                    errores.push("El campo <strong>Apellido Materno</strong> debe contener al menos 2 caracteres.");
                    apellidoMaterno = false;
                }
            }

            // Nombres
            var nombres = true;
            var eNombres = false;
            if ($.trim($("#ModBusAfiNombres_RP").val()).length > 0) {
                eNombres = true;
                if ($.trim($("#ModBusAfiNombres_RP").val()).length < 2) {
                    errores.push("El campo <strong>Nombres</strong> debe contener al menos 2 caracteres.");
                    nombres = false;
                }
            }

            // Criterio mínimo
            var criterioMinimo = true;
            if (!(eApellidoPaterno | eApellidoMaterno | eNombres)) {
                errores.push("Debe seleccionar al menos un criterio de búsqueda.");
                criterioMinimo = false;
            }

            if (!apellidoPaterno) $('#ModBusAfiApellidoPaterno_RP').addClass('formTextboxError');
            if (!apellidoMaterno) $('#ModBusAfiApellidoMaterno_RP').addClass('formTextboxError');
            if (!nombres) $('#ModBusAfiNombres_RP').addClass('formTextboxError');

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & criterioMinimo;

            if (esCorrecto) {
                BusAfiApellidoPaterno = $('#ModBusAfiApellidoPaterno_RP').val();
                BusAfiApellidoMaterno = $('#ModBusAfiApellidoMaterno_RP').val();
                BusAfiNombres = $('#ModBusAfiNombres_RP').val();

                $('#TabAfiliadosIndicePagina_RP').val(1);
                $('#TabAfiliadosColumnaOrdenar_RP').val(1);
                $('#TabAfiliadosDireccionOrdenar_RP').val('A');

                //if ($('#hindPestaniaActiva').val() == '1') {
                //    CargarTablaAfiliados_RP(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
                //} else if ($('#hindPestaniaActiva').val() == '2') {
                //    CargarTablaAfiliadosExterno_RP(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
                //} else {
                //    CargarTablaAfiliados_RP(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
                //}
                CargarTablaAfiliados_RP(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
            }
            else {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }
        }
        else {
            return false;
        }
    });

    /* Botón de Paginador de Tabla de Afiliados */
    $('#TabAfiliadosPaginador a').live('click', function () {
        if (!botonesBloqueados) {
            var pagina = $(this).data("pag");
            $('#TabAfiliadosIndicePagina_RP').val(pagina);

            CargarTablaAfiliados2();
        }
        else {
            return false;
        }
    });

    /* Botón de Ordenar de Tabla de Afiliados */
    $('#TabAfiliados a').live('click', function () {
        if (!botonesBloqueados) {
            var columna = $(this).data("col");
            if (columna != $('#TabAfiliadosColumnaOrdenar_RP').attr('value')) {
                $('#TabAfiliadosColumnaOrdenar_RP').val(columna);
                $('#TabAfiliadosDireccionOrdenar_RP').val('A');
            }
            else {
                if ($('#TabAfiliadosDireccionOrdenar_RP').attr('value') == 'A') {
                    $('#TabAfiliadosDireccionOrdenar_RP').val('D');
                }
                else {
                    $('#TabAfiliadosDireccionOrdenar_RP').val('A');
                }
            }
            $('#TabAfiliadosIndicePagina_RP').val(1);

            CargarTablaAfiliados2();
        }
    });

    /* Cambiando la fecha de cotizacion */
    $('#ModSolFechaCotizacion_IFP').live('change', function () {
        //en el texto se ve: 14/03/2016 (dd/mm/yyyy)
        if ($("#ModSolFechaCotizacion_IFP").val() != "") {
            var fecha = $("#ModSolFechaCotizacion_IFP").val();
            var mes = fecha.substring(3, 5);
            var dia = fecha.substring(0, 2);
            var anho = fecha.substring(6, 10);
            var dt = new Date(anho + "/" + mes + "/" + dia);

            $("#ModSolFechaDevengue_IFP").val("01/" + mes + "/" + anho);

            var fecvigencia = sumarDias(dt, 15);

            dia = "0" + fecvigencia.getDate();
            mes = "00" + fecvigencia.getMonth();
            anho = fecvigencia.getFullYear();

            dia = dia.substring(dia.length - 2, dia.length);
            mes = mes.substring(mes.length - 2, mes.length);

            $("#ModSolFechaVigencia_IFP").val(dia + "/" + mes + "/" + anho);

            ListarPorcentajeDevolucion($("#ModSolFechaCotizacion_IFP").val(), "PLAN1");
            ListarPorcentajeDevolucion($("#ModSolFechaCotizacion_IFP").val(), "PLAN2");
        }
    });


    function ListarPorcentajeDevolucion(fec_cotizacion, plan) {

        var params = {
            fec_cotizacion: fec_cotizacion
            , plan: plan
        };

        $.ajax({
            type: 'POST',
            url: 'MantenerSolicitud.aspx/ListarPorcentajeDevolucion',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                var options = data.d;
                if (plan == "PLAN1") var $el = $("#TabCotizaciones_IFP_P1 .ModSolSobrevivencia");
                if (plan == "PLAN2") var $el = $("#TabCotizaciones_IFP_P2 .ModSolDevolucion");
                $el.empty();
                $.each(options, function (key, value) {
                    $el.append($("<option></option>")
                        .attr("value", value.Id).text(value.Glosa));
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    }


    //<INIGTI_753_3>
    function sumarDias(fecha, dias) {
        fecha.setDate(fecha.getDate() + dias);
        return fecha;
    };

    //<FINGTI_753_3>
    /* Seleccionar registro de afiliado */
    $('#TabAfiliados input[type=radio]').live('change', function () {
        var radio = $(this);
        var filaPadre = $(this).parent().parent();
        var clasePadre = filaPadre.attr('class');
        setTimeout(function () {
            $("#TabAfiliados tbody tr").removeClass("grilla_active");
            $("#TabAfiliados tbody tr:even").addClass("grilla_alt1");
            $("#TabAfiliados tbody tr:odd").addClass("grilla_alt2");

            filaPadre.toggleClass("grilla_active", radio.is(":checked"));
        }, 0);
    });

    /* Botón Cancelar */
    $('#ModBusAfiCancelar_RP').live('click', function () {
        $('#ModalBusquedaAfiliados_RP').dialog('close');
    });

    /* Cerrar Modal */
    $('#ModalBusquedaAfiliados_RP').live("dialogclose", function () {
        if (ajaxTablaAfiliados != null) {
            ajaxTablaAfiliados.abort();
        }
    });

    /* Botón Aceptar */
    $('#ModBusAfiAceptar').live('click', function () {
        if (!botonesBloqueados && permisoBusAfiExaminarSolicitud) {
            // Validar que haya seleccionado una opción de la tabla
            if ($('#TabAfiliados').find('input[type=radio]:checked').length > 0) {

                if ($('#hindPestaniaActiva').val() == 2) {
                    $('#ModalBusquedaAfiliados_RP').dialog('close');
                    $('#hcusppInteligo').val($('#TabAfiliados input[type=radio]:checked').attr("value"));
                    $('#BusAfiBuscar2_RP').trigger('click');
                } else {
                    $('#BusAfiNroSolicitud_RP').val('');
                    $('#BusAfiCUSPP_RP').val($('#TabAfiliados input[type=radio]:checked').attr("value"));
                    $('#ModalBusquedaAfiliados_RP').dialog('close');
                    $('#BusAfiCUSPP_RP').focus();
                    $('#BusAfiBuscar_RP').trigger('click');
                    // Simuladores
                    $('#SimBusAfiBuscar').trigger('click');
                    //<INI.GTI_7012_2_1>
                    //Reporte Cotizacion Plus
                    $('#RepCotPlusBusAfiBuscar').trigger('click');
                    //<FIN.GTI_7012_2_1>
                    $('hcusppInteligo').val('');
                }

            }
            else {
                var errores = new Array();
                errores.push('Por favor seleccione un afiliado de la lista antes de continuar.');

                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }
        }
        else {
            return false;
        }
    });

    /* Botón Buscar Solicitud */
    $('#BusAfiBuscar_RP').live('click', function () {

        if (!botonBusAfiBuscarBloqueado && permisoBusAfiBuscar) {
            var esCorrecto = true;
            var errores = new Array();

            $('#BusAfiNroSolicitud_RP').removeClass('formTextboxError');
            $('#BusAfiCUSPP_RP').removeClass('formTextboxError');

            var solicitud = ($.trim($('#BusAfiNroSolicitud_RP').val()).length > 0) ? true : false;
            var cuspp = ($.trim($('#BusAfiCUSPP_RP').val()).length > 0) ? true : false;

            if (!(solicitud | cuspp)) {
                errores.push('Debe ingresar un criterio de búsqueda.');
                $('#BusAfiNroSolicitud_RP').addClass('formTextboxError');
                $('#BusAfiCUSPP_RP').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (solicitud & cuspp) {
                errores.push('Sólo debe ingresar un criterio de búsqueda.');
                $('#BusAfiNroSolicitud_RP').addClass('formTextboxError');
                $('#BusAfiCUSPP_RP').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (cuspp && $.trim($('#BusAfiCUSPP_RP').val()).length != 12) {
                errores.push('El <strong>CUSPP</strong> debe contener 12 caracteres.');
                $('#BusAfiCUSPP_RP').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (!esCorrecto) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }

            botonBusAfiBuscarBloqueado = true;
            $('#BusAfiBuscar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
        }
        else {
            return false;
        }
    });

    ///* Verificar si se ha modificado algún campo */
    //$('#CorreoElectronico_RP,#Categoria_RP,#AFP_RP,#SaldoCIC_RP').bind('keyup keydown keypress change', function () {
    //    if ($.trim($('#CUSPP_RP').val()).length > 0) {
    //        if ($(this).val() != jQuery.data(this, 'lastvalue')) {
    //            datosSinGuardar = true;
    //        }
    //        jQuery.data(this, 'lastvalue', $(this).val());
    //    }
    //});

    /* Verificar si se ha modificado algún campo */
    $('#CorreoElectronico_RP').bind('keyup keydown keypress change', function () {
        if ($.trim($('#CUSPP_RP').val()).length > 0) {
            if ($(this).val() != jQuery.data(this, 'lastvalue')) {
                datosSinGuardar = true;
            }
            jQuery.data(this, 'lastvalue', $(this).val());
        }
    });

    ///* Botón Guardar */
    //$('#Guardar_RP').live('click', function () {
    //    if (!botonesBloqueados && permisoGuardar) {


    //        // Validaciones
    //        //var esCorrecto = true;
    //        //var errores = new Array();

    //        //$('#CorreoElectronico_RP').removeClass('formTextboxError');
    //        //$('#ConCategoria').removeClass('formComboboxErrorContenedor');
    //        //$('#ConAFP').removeClass('formComboboxErrorContenedor');

    //        //$('#SaldoCIC_RP').removeClass('formTextboxError');

    //        // Correo Electrónico
    //        //var correoElectronico = true;
    //        //if ($('#CorreoElectronico').length > 0) {
    //        //    if ($.trim($('#CorreoElectronico').val()).length == 0) {
    //        //        errores.push("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.");
    //        //        correoElectronico = false;
    //        //    }
    //        //}

    //        //// Categoría
    //        //var categoria = true;
    //        //if ($('#Categoria_RP').val() == "0") {
    //        //    errores.push("Ingrese el campo <strong>Categoría</strong>. Dato Obligatorio.");
    //        //    categoria = false;
    //        //}

    //        //// AFP
    //        //var afp = true;
    //        //if ($('#AFP_RP').val() == "0") {
    //        //    errores.push("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
    //        //    afp = false;
    //        //}

    //        //// Saldo CIC
    //        //var saldoCIC = true;
    //        //if ($('#SaldoCIC_RP').length > 0) {
    //        //    if ($.trim($('#SaldoCIC_RP').val()).length == 0) {
    //        //        errores.push("Ingrese el campo <strong>Saldo CIC</strong>. Dato Obligatorio.");
    //        //        saldoCIC = false;
    //        //    }
    //        //}

    //        //// Clases de controles
    //        ////if (!correoElectronico) { $('#CorreoElectronico_RP').attr('class', 'formTextbox formTextboxError'); } else { $('#CorreoElectronico').attr('class', 'formTextbox'); }
    //        //if (!categoria) { $('#ConCategoria').attr('class', 'formComboboxContenedor formComboboxErrorContenedor'); } else { $('#ConCategoria').attr('class', 'formComboboxContenedor'); }
    //        //if (!afp) { $('#ConAFP').attr('class', 'formComboboxContenedor formComboboxErrorContenedor'); } else { $('#ConAFP').attr('class', 'formComboboxContenedor'); }
    //        ////if (!saldoCIC) { $('#SaldoCIC_RP').attr('class', 'formTextbox formTextboxError'); } else { $('#SaldoCIC_RP').attr('class', 'formTextbox'); }

    //        //esCorrecto = correoElectronico & categoria & afp & saldoCIC;

    //        //if (!esCorrecto) {
    //        //    $('#MCMIcono').attr('class', 'validacion');
    //        //    $('#MCMContenedor').html(formatearError(errores));
    //        //    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
    //        //    $('#ModalCuadroMensaje').dialog('open');
    //        //    return false;
    //        //}

    //        //// Pasó las validaciones
    //        //botonesBloqueados = true;
    //        //$('#Guardar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

    //    }
    //    else {
    //        return false;
    //    }
    //});

    /* DIRECCIONES */
    var idDireccion;
    /* Botón Nueva */
    $('#NuevaDireccion_RP').live('click', function () {
        if (permisoNuevaDireccion) {

            idDireccion = 0;
            $.ajax({
                type: 'POST',
                url: 'Cotizador.aspx/SessionIdDreccion',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: "{idDireccion:'" + idDireccion + "'}",
                success: function (data) {

                    var direccion = data.d;
                    if (idDireccion.toString() == direccion.toString()) {
                        window.location.href = "DireccionAfiliado.aspx";
                    }

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                        document.location.reload(true);
                    }
                    else {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                    $('#ModalDireccion').dialog('close');
                }
            });

            //window.location.href = "../RentaPrivada/DireccionAfiliado.aspx";

            //$('#ModDirModo').val('N');


        }
        else {
            return false;
        }
    });

    /* Cargando ubigeo y direccion para modificar*/
    function CargandoUbigeo() {
        idDireccion = $("#ModIdDireccion").val();
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/ObtenerDatosDireccion',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idDireccion:'" + idDireccion + "'}",
            success: function (data) {
                //var direccion = $.evalJSON(data.d);
                var direccion = data.d;

                $('#ModDomicilio').val(direccion.TipoVia.Id);
                $('#TexModDomicilio').html($('#ModDomicilio').find(':selected').text());
                $('#ModDirDireccion').val(direccion.Glosa);
                $('#ModDirEspacioUrbano').val(direccion.EspacioUrbano);
                $('#ModDirDepartamento').val(direccion.Departamento.Id);
                $('#TexModDirDepartamento').html($('#ModDirDepartamento').find(':selected').text());
                $('#ModDirPrincipal').val(direccion.Principal ? 'S' : 'N');
                $('#TexModDirPrincipal').html($('#ModDirPrincipal').find(':selected').text());

                $.ajax({
                    type: 'POST',
                    url: 'Cotizador.aspx/CargarComboCiudades',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: "{idDepartamento:'" + $('#ModDirDepartamento').val() + "'}",
                    success: function (data) {
                        $('#ModDirControlCiudad').html($(data.d).find('#ContenidoDinamico').html());
                        $('#ModDirCiudad').val(direccion.Ciudad.Id);
                        ActualizaEstiloCombobox('#ModDirCiudad');
                        $.ajax({
                            type: 'POST',
                            url: 'Cotizador.aspx/CargarComboComunas',
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: 'json',
                            data: "{idCiudad:'" + $('#ModDirCiudad').val() + "'}",
                            success: function (data) {
                                $('#ModDirControlComuna').html($(data.d).find('#ContenidoDinamico').html());
                                $('#ModDirComuna').val(direccion.Comuna.Id);
                                ActualizaEstiloCombobox('#ModDirComuna');
                                $('#ModDirDireccion').focus();
                                $('#ModDirCargando').fadeOut();

                                /*<SRIINI17003>*/
                                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                $clock.countdown(selectedDate.toString());
                                /*<SRIFIN17003>*/
                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                    document.location.reload(true);
                                }
                                else {
                                    $('#MCMIcono').attr('class', 'error');
                                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de comunas.');
                                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                    $('#ModalCuadroMensaje').dialog('open');
                                }
                            }
                        });
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            document.location.reload(true);
                        }
                        else {
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de ciudades.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    }
                });
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                $('#ModalDireccion').dialog('close');
            }
        });
    }

    /* Botón Modificar */
    $('#TabDirecciones .grilla_editar').live('click', function () {

        idDireccion = $(this).data('direccion');
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/SessionIdDreccion',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idDireccion:'" + idDireccion + "'}",
            success: function (data) {

                var direccion = data.d;
                if (idDireccion.toString() == direccion.toString()) {
                    window.location.href = "DireccionAfiliado.aspx";
                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                $('#ModalDireccion').dialog('close');
            }
        });

    });

    /* Botón Eliminar */
    $('#TabDirecciones .grilla_eliminar').live('click', function () {
        idDireccion = $(this).data('direccion');
        $('#MCATablaEliminar').val('dir');
        $('#MCAIcono').attr('class', 'advertencia');
        $('#MCAContenedor').html('¿Está seguro de eliminar el registro?');
        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
        $('#ModalCuadroAdvertencia').dialog('open');
        return false;
    });

    /* Cambiar valor en combobox de Departamento */
    $('#ModDirDepartamento').live('change', function (e) {
        $('#ModDirCargandoCiudad').show();
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarComboCiudades',
            //url: 'Cotizador.aspx/CargarComboCiudades',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idDepartamento:'" + $('#ModDirDepartamento').val() + "'}",
            success: function (data) {
                $('#ModDirControlCiudad').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#ModDirCiudad');
                $('#ModDirComuna').val('0');
                $('#ModDirComuna').trigger('change');
                $('#ModDirComuna').attr('disabled', 'disabled');
                $('#ModDirCiudad').focus();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de ciudades.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });

    /* Cambiar valor en combobox de Ciudad*/
    $('#ModDirCiudad').live('change', function (e) {
        $('#ModDirCargandoComuna').show();
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarComboComunas',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idCiudad:'" + $('#ModDirCiudad').val() + "'}",
            success: function (data) {
                $('#ModDirControlComuna').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#ModDirComuna');
                $('#ModDirComuna').focus();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de comunas.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });

    var ModDirBotonesInactivos = false;
    /* Botón Aceptar */
    $('#ModDirAceptar_RP').live('click', function () {
        if (!ModDirBotonesInactivos) {
            var esCorrecto = true;
            var errores = new Array();

            $('#ModDirDireccion').removeClass('formTextboxError');
            $('#ConModDirDepartamento').removeClass('formComboboxErrorContenedor');
            $('#ConModDirCiudad').removeClass('formComboboxErrorContenedor');
            $('#ConModDirComuna').removeClass('formComboboxErrorContenedor');
            $('#ConModDirPrincipal').removeClass('formComboboxErrorContenedor');

            $('#ModDirEspacioUrbano').removeClass('formTextboxError');
            $('#ModDomicilio').removeClass('formComboboxErrorContenedor');

            var direccion = ($.trim($('#ModDirDireccion').val()).length > 0) ? true : false;
            var departamento = ($('#ModDirDepartamento').val() != '0') ? true : false;
            var ciudad = ($('#ModDirCiudad').val() != '0') ? true : false;
            var comuna = ($('#ModDirComuna').val() != '0') ? true : false;
            var principal = ($('#ModDirPrincipal').val() != '0') ? true : false;

            var espaciourbano = ($.trim($('#ModDirEspacioUrbano').val()).length > 0) ? true : false;
            var domicilio = ($('#ModDomicilio').val() != '0') ? true : false;

            if (!direccion) {
                errores.push('Ingrese el campo <strong>Dirección</strong>. Dato Obligatorio.');
                $('#ModDirDireccion').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (!departamento) {
                errores.push('Ingrese el campo <strong>Departamento</strong>. Dato Obligatorio.');
                $('#ConModDirDepartamento').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!ciudad) {
                errores.push('Ingrese el campo <strong>Provincia</strong>. Dato Obligatorio.');
                $('#ConModDirCiudad').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!comuna) {
                errores.push('Ingrese el campo <strong>Distrito</strong>. Dato Obligatorio.');
                $('#ConModDirComuna').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!principal) {
                errores.push('Ingrese el campo <strong>Principal</strong>. Dato Obligatorio.');
                $('#ConModDirPrincipal').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!espaciourbano) {
                errores.push('Ingrese el campo <strong>Espacio Urbano</strong>. Dato Obligatorio.');
                $('#ModDirEspacioUrbano').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (!domicilio) {
                errores.push('Ingrese el campo <strong>Tipos de Vía</strong>. Dato Obligatorio.');
                $('#ConModDomicilio').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!esCorrecto) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }

            /* Pasó las validaciones */
            ModDirBotonesInactivos = true;
            $('#ModDirAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#ModDirCargando').fadeIn();

            //var wl_cussp = $('#CUSPP_RP').val();

            //if ($.trim($('#HCUSPP_RP').val()).length > 0) {
            //    wl_cussp = $('#HCUSPP_RP').val()
            //}

            if ($('#ModDirModo').val() == 'N') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    cuspp: $('#HCUSPP_RP').val(),
                    direccion: $('#ModDirDireccion').val(),
                    idDepartamento: $('#ModDirDepartamento').val(),
                    idCiudad: $('#ModDirCiudad').val(),
                    idComuna: $('#ModDirComuna').val(),
                    idPrincipal: $('#ModDirPrincipal').val(),
                    glsEspacioUrbano: $('#ModDirEspacioUrbano').val(),
                    idDomicilio: $('#ModDomicilio').val()
                }
                var postUrl = 'Cotizador.aspx/InsertarDireccion';
            }
            else if ($('#ModDirModo').val() == 'M') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    idDireccion: idDireccion,
                    cuspp: $('#HCUSPP_RP').val(),
                    direccion: $('#ModDirDireccion').val(),
                    idDepartamento: $('#ModDirDepartamento').val(),
                    idCiudad: $('#ModDirCiudad').val(),
                    idComuna: $('#ModDirComuna').val(),
                    idPrincipal: $('#ModDirPrincipal').val(),
                    glsEspacioUrbano: $('#ModDirEspacioUrbano').val(),
                    idDomicilio: $('#ModDomicilio').val()
                }

                var postUrl = '../RentaIFP/Cotizador.aspx/ModificarDireccion';
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
                        //$('#ModalDireccion').dialog('close');

                        /* Recargar la grilla de direcciones */
                        //CargarTablaDirecciones();

                        /*<SRIINI17003>*/
                        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                        $clock.countdown(selectedDate.toString());


                        $('#MCMIcono').attr('class', 'exito');
                        $('#MCMContenedor').html('Dirección guardado correctamente.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Infomación' });
                        $('#ModalCuadroMensaje').dialog('open');
                        window.location.href = "Cotizador.aspx#nueva_direccion";
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
                            if (data.d.Controles[0].length) $('#ModDirDireccion').attr('class', data.d.Controles[0]);
                            if (data.d.Controles[1].length) $('#ConModDirDepartamento').attr('class', data.d.Controles[1]);
                            if (data.d.Controles[2].length) $('#ConModDirCiudad').attr('class', data.d.Controles[2]);
                            if (data.d.Controles[3].length) $('#ConModDirComuna').attr('class', data.d.Controles[3]);
                            if (data.d.Controles[4].length) $('#ConModDirPrincipal').attr('class', data.d.Controles[4]);
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
                        $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la dirección.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                },
                complete: function () {
                    $('#ModDirCargando').fadeOut();
                    ModDirBotonesInactivos = false;
                    $('#ModDirAceptar_RP').attr('class', 'boton darkblue sharp');
                }
            });
        }
    });

    /* Botón Cancelar */
    $('#ModDirCancelar_RP').live('click', function () {
        window.location.href = "Cotizador.aspx#nueva_direccion";
    });


    /* TELÉFONOS */
    var idTelefono;
    /* Botón Nuevo */
    $('#NuevoTelefono_RP').live('click', function () {
        if (permisoNuevoTelefono) {
            //$('#ModTelModo').val('N');

            //LimpiarFormularioTelefonos();

            //$('#ModTelCargando').hide();
            //$('#ModalTelefono').dialog('open');
            //$('#ModTelTipo').focus();

            idTelefono = 0;
            $.ajax({
                type: 'POST',
                url: 'Cotizador.aspx/SessionIdTelefono',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: "{idTelefono:'" + idTelefono + "'}",
                success: function (data) {

                    var telefono = data.d;
                    if (idTelefono.toString() == telefono.toString()) {
                        window.location.href = "TelefonoAfiliado.aspx";
                    }

                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                        document.location.reload(true);
                    }
                    else {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la información del teléfono.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                    $('#ModalTelefono').dialog('close');
                }
            });

        }
        else {
            return false;
        }
    });

    function CargandoTelefono() {
        //$('#ModTelModo').val('M');
        //LimpiarFormularioTelefonos();
        //$('#ModTelCargando').show();
        //$('#ModalTelefono').dialog('open');
        idTelefono = $("#ModIdTelefono").val(); // $(this).data('telefono');

        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/ObtenerDatosTelefono',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idTelefono:'" + idTelefono + "'}",
            success: function (data) {
                var telefono = data.d;
                $('#ModTelTipo').val(telefono.Tipo.Id);
                $('#TexModTelTipo').html($('#ModTelTipo').find(':selected').text());
                $('#ModTelNumero').val(telefono.Numero);
                $('#ModTelPrincipal').val(telefono.Principal ? 'S' : 'N');
                $('#TexModTelPrincipal').html($('#ModTelPrincipal').find(':selected').text());
                $('#ModTelTipo').focus();
                $('#ModTelCargando').fadeOut();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información del teléfono.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                $('#ModalTelefono').dialog('close');
            }
        });
    }

    /* Botón Modificar */
    $('#TabTelefonos .grilla_editar').live('click', function () {

        idTelefono = $(this).data('telefono');
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/SessionIdTelefono',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idTelefono:'" + idTelefono + "'}",
            success: function (data) {

                var telefono = data.d;
                if (idTelefono.toString() == telefono.toString()) {
                    window.location.href = "TelefonoAfiliado.aspx";
                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información del teléfono.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                $('#ModalTelefono').dialog('close');
            }
        });

        //$('#ModTelModo').val('M');
        //LimpiarFormularioTelefonos();
        //$('#ModTelCargando').show();
        //$('#ModalTelefono').dialog('open');
        //idTelefono = $(this).data('telefono');

        //$.ajax({
        //	type: 'POST',
        //	url: 'Cotizador.aspx/ObtenerDatosTelefono',
        //	contentType: "application/json; charset=iso-8859-1",
        //	dataType: 'json',
        //	data: "{idTelefono:'" + idTelefono + "'}",
        //	success: function (data) {
        //		var telefono = data.d;
        //		$('#ModTelTipo').val(telefono.Tipo.Id);
        //		$('#TexModTelTipo').html($('#ModTelTipo').find(':selected').text());
        //		$('#ModTelNumero').val(telefono.Numero);
        //		$('#ModTelPrincipal').val(telefono.Principal ? 'S' : 'N');
        //		$('#TexModTelPrincipal').html($('#ModTelPrincipal').find(':selected').text());
        //		$('#ModTelTipo').focus();
        //		$('#ModTelCargando').fadeOut();
        //	},
        //	error: function (XMLHttpRequest, textStatus, errorThrown) {
        //		if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //			document.location.reload(true);
        //		}
        //		else {
        //			$('#MCMIcono').attr('class', 'error');
        //			$('#MCMContenedor').html('Ha ocurrido un error al cargar la información del teléfono.');
        //			$('#ModalCuadroMensaje').dialog({ title: 'Error' });
        //			$('#ModalCuadroMensaje').dialog('open');
        //		}
        //		$('#ModalTelefono').dialog('close');
        //	}
        //});

    });

    /* Botón Eliminar */
    $('#TabTelefonos .grilla_eliminar').live('click', function () {
        idTelefono = $(this).data('telefono');
        $('#MCATablaEliminar').val('tel');
        $('#MCAIcono').attr('class', 'advertencia');
        $('#MCAContenedor').html('¿Está seguro de eliminar el registro?');
        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
        $('#ModalCuadroAdvertencia').dialog('open');
        return false;
    });

    var ModTelBotonesInactivos = false;
    /* Botón Aceptar */
    $('#ModTelAceptar_RP').live('click', function () {
        if (!ModTelBotonesInactivos) {
            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            $('#ConModTelTipo').removeClass('formComboboxErrorContenedor');
            $('#ModTelNumero').removeClass('formTextboxError');
            $('#ConModTelPrincipal').removeClass('formComboboxErrorContenedor');

            var tipo = ($('#ModTelTipo').val() != '0') ? true : false;
            var numero = ($.trim($('#ModTelNumero').val()).length > 0) ? true : false;
            var principal = ($('#ModTelPrincipal').val() != '0') ? true : false;

            if (!tipo) {
                errores.push('Ingrese el campo <strong>Tipo</strong>. Dato Obligatorio.');
                $('#ConModTelTipo').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!numero) {
                errores.push('Ingrese el campo <strong>Número</strong>. Dato Obligatorio.');
                $('#ModTelNumero').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (!principal) {
                errores.push('Ingrese el campo <strong>Principal</strong>. Dato Obligatorio.');
                $('#ConModTelPrincipal').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!esCorrecto) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }

            /* Pasó las validaciones */
            ModTelBotonesInactivos = true;
            $('#ModTelAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#ModTelCargando').fadeIn();

            if ($('#ModTelModo').val() == 'N') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    cuspp: $('#HCUSPP_RP').val(),
                    idTipo: $('#ModTelTipo').val(),
                    numero: $('#ModTelNumero').val(),
                    idPrincipal: $('#ModTelPrincipal').val()
                }
                var postUrl = 'Cotizador.aspx/InsertarTelefono';
            }
            else if ($('#ModTelModo').val() == 'M') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    idTelefono: idTelefono,
                    cuspp: $('#HCUSPP_RP').val(),
                    idTipo: $('#ModTelTipo').val(),
                    numero: $('#ModTelNumero').val(),
                    idPrincipal: $('#ModTelPrincipal').val()
                }
                var postUrl = 'Cotizador.aspx/ModificarTelefono';
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
                        ////$('#ModalTelefono').dialog('close');

                        /* Recargar la grilla de teléfonos */
                        ////CargarTablaTelefonos();

                        /*<SRIINI17003>*/
                        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                        $clock.countdown(selectedDate.toString());
                        /*<SRIFIN17003>*/


                        $('#MCMIcono').attr('class', 'exito');
                        $('#MCMContenedor').html('Téléfono guardado correctamente.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Infomación' });
                        $('#ModalCuadroMensaje').dialog('open');
                        window.location.href = "Cotizador.aspx#nuevo_telefono";

                    }
                    else if (data.d.Estado == 'TOKEN') {
                        CerrarSesionExpirada();
                    }
                    else {
                        $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                        $('#MCMIcono').attr('class', data.d.Icono);
                        $('#MCMContenedor').html(data.d.Mensaje);
                        if (data.d.Controles != null) {
                            if (data.d.Controles[0].length) $('#ConModTelTipo').attr('class', data.d.Controles[0]);
                            if (data.d.Controles[1].length) $('#ModTelNumero').attr('class', data.d.Controles[1]);
                            if (data.d.Controles[2].length) $('#ConModTelPrincipal').attr('class', data.d.Controles[2]);
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
                        $('#MCMContenedor').html('Ha ocurrido un error al guardar la información del teléfono.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                },
                complete: function () {
                    $('#ModTelCargando').fadeOut();
                    ModTelBotonesInactivos = false;
                    $('#ModTelAceptar_RP').attr('class', 'boton darkblue sharp');
                }
            });
        }
    });

    /* Botón Cancelar */
    $('#ModTelCancelar_RP').live('click', function () {
        //$('#ModalTelefono').dialog('close');
        window.location.href = "Cotizador.aspx#nuevo_telefono";
    });


    /* GRUPO FAMILIAR */
    var idGrupoFamiliar;
    /* Botón Nuevo */
    $('#NuevoBeneficiario_RP').live('click', function () {
        if (permisoNuevoBeneficiario) {
            idGrupoFamiliar = 0;
            $.ajax({
                type: 'POST',
                url: '../RentaIFP/GrupoFamiliarAfiliado.aspx/SessionIdGrupoFamiliar',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "', paginaLlamada:'../RentaIFP/Cotizador.aspx#grupo_familiar'}",

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
                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                    $('#ModalGrupoFamiliar').dialog('close');
                }
            });

        }
        else {
            return false;
        }
    });

    /* Cargando grupo familiar*/
    function CargandoGrupoFamiliar() {
        $('#ModGruFamModo_RP').val('M');

        LimpiarFormularioGrupoFamiliar();

        idGrupoFamiliar = $("#ModIdGrupoFamiliar").val();
        var params = {
            idGrupoFamiliar: idGrupoFamiliar
        }

        $.ajax({
            type: 'POST',
            url: '../RentaIFP/GrupoFamiliarAfiliado.aspx/ObtenerDatosGrupoFamiliarSolicitud',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                var grupo = data.d;

                if (grupo.Parentesco.Id != '80') {
                    $('#masInfo').hide();
                }

                //Apellido Paterno
                $('#ModGruFamApellidoPaterno_RP').val(grupo.ApellidoPaterno);
                //Apellido Materno
                $('#ModGruFamApellidoMaterno_RP').val(grupo.ApellidoMaterno);
                //Nombres
                $('#ModGruFamNombres_RP').val(grupo.Nombre);
                //Tipo de Identificación
                $('#ModGruFamTipoIdentificacion_RP').val(grupo.Identificacion.IdTipo);
                $('#TexModGruFamTipoIdentificacion_RP').html($('#ModGruFamTipoIdentificacion_RP').find(':selected').text());
                if (grupo.Parentesco.Id == '80') {
                    $('#ModGruFamTipoIdentificacion_RP').attr('disabled', 'disabled');
                    $('#ConModGruFamTipoIdentificacion_RP').addClass('formComboboxReadOnlyContenedor');
                }
                else {
                    $('#ModGruFamTipoIdentificacion_RP').removeAttr('disabled');
                    $('#ConModGruFamTipoIdentificacion_RP').removeClass('formComboboxReadOnlyContenedor');
                }
                //Nro. de Identificación
                $('#ModGruFamNumeroIdentificacion_RP').val(grupo.Identificacion.Numero);
                if (grupo.Parentesco.Id == '80') {
                    $('#ModGruFamNumeroIdentificacion_RP').attr('disabled', 'disabled');
                    $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxReadOnly');
                }
                else {
                    $('#ModGruFamNumeroIdentificacion_RP').removeAttr('disabled');
                    $('#ModGruFamNumeroIdentificacion_RP').removeClass('formTextboxReadOnly');
                }
                //Parentesco
                $('#ModGruFamParentesco_RP').val(grupo.Parentesco.Id);
                $('#TexModGruFamParentesco_RP').html($('#ModGruFamParentesco_RP').find(':selected').text());
                if (grupo.Parentesco.Id == '80') {
                    $('#ModGruFamParentesco_RP').attr('disabled', 'disabled');
                    $('#ConModGruFamParentesco_RP').addClass('formComboboxReadOnlyContenedor');
                }
                else {
                    $('#ModGruFamParentesco_RP').removeAttr('disabled');
                    $('#ConModGruFamParentesco_RP').removeClass('formComboboxReadOnlyContenedor');
                }
                //Sexo
                $('#ModGruFamSexo_RP').val(grupo.Sexo);
                $('#TexModGruFamSexo_RP').html($('#ModGruFamSexo_RP').find(':selected').text());
                if (grupo.Parentesco.Id == '80') {
                    $('#ModGruFamSexo_RP').attr('disabled', 'disabled');
                    $('#ConModGruFamSexo_RP').addClass('formComboboxReadOnlyContenedor');
                }
                else {
                    $('#ModGruFamSexo_RP').removeAttr('disabled');
                    $('#ConModGruFamSexo_RP').removeClass('formComboboxReadOnlyContenedor');
                }
                //Fecha de Nacimiento
                $('#ModGruFamFechaNacimiento_RP').val(new Date(+grupo.FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));

                if (grupo.Parentesco.Id == '80') {
                    $('#ModGruFamFechaNacimiento_RP').attr('disabled', 'disabled');
                    $('#ModGruFamFechaNacimiento_RP').addClass('formCalendarReadOnly');
                }
                else {
                    $('#ModGruFamFechaNacimiento_RP').removeAttr('disabled');
                    $('#ModGruFamFechaNacimiento_RP').removeClass('formCalendarReadOnly');
                }

                //Indicador de Invalidez
                $('#ModGruFamIndInvalidez_RP').val(grupo.Invalido ? 'S' : 'N');
                $('#TexModGruFamIndInvalidez_RP').html($('#ModGruFamIndInvalidez_RP').find(':selected').text());
                if (grupo.Invalido) {
                    //Tipo de Invalidez
                    $('#ModGruFamTipoInvalidez_RP').val(grupo.TipoInvalidez.Id);
                    $('#TexModGruFamTipoInvalidez_RP').html($('#ModGruFamTipoInvalidez_RP').find(':selected').text());
                    $('#ModGruFamTipoInvalidez_RP').removeAttr('disabled');
                    $('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxReadOnlyContenedor');
                    //Fecha de Invalidez
                    if (grupo.FechaInvalidez != null)
                        $('#ModGruFamFechaInvalidez_RP').val(new Date(+grupo.FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                    $('#ModGruFamFechaInvalidez_RP').removeAttr('disabled');
                    $('#ModGruFamFechaInvalidez_RP').removeClass('formCalendarReadOnly');
                }
                else {
                    //Tipo de Invalidez
                    $('#ModGruFamTipoInvalidez_RP').val('N');
                    $('#TexModGruFamTipoInvalidez_RP').html($('#ModGruFamTipoInvalidez_RP').find(':selected').text());
                    $('#ModGruFamTipoInvalidez_RP').attr('disabled', 'disabled');
                    //Fecha de Invalidez
                    $('#ModGruFamFechaInvalidez_RP').val('');
                    $('#ModGruFamFechaInvalidez_RP').attr('disabled', 'disabled');
                }

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
                //SujetoObligado
                $('#ModGruFamSO_RP').val(grupo.ind_SujetoObligado ? 'S' : 'N');
                $('#TexModGruFamSO_RP').html($('#ModGruFamSO_RP').find(':selected').text());

                //Banco
                $('#ModGruFamNumeroBanco_RP').val(grupo.NumeroBanco);
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

                //Comunicacion
                //$('#ModGruFamComunicacion_RP').val(grupo.Comunicacion.Id);
                $('#ModGruFamComunicacion_RP').val(1);
                $('#TexModGruFamComunicacion_RP').html($('#ModGruFamComunicacion_RP').find(':selected').text());

                $('#ModGruFamComunicacion_RP').attr('disabled', 'disabled');
                $('#ConModGruFamComunicacion_RP').addClass('formComboboxReadOnlyContenedor');
                //ConfidencialidadDatos
                $('#ModGruFamConfidencialidadDatos_RP').val(grupo.Confidencialidaddatos.Id);
                $('#TexModGruFamConfidencialidadDatos_RP').html($('#ModGruFamConfidencialidadDatos_RP').find(':selected').text());

                //Correo Electrónico
                $('#ModGruFamCorreoElectronico_RP').val(grupo.CorreoElectronico);

                $('#ModGruFamApellidoPaterno_RP').focus();
                $('#ModGruFamCargando_RP').fadeOut();
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
            }
        });
    }



    function MostrarModal() {
        $('#MCMIcono').attr('class', 'validacion');
        $('#MCMContenedor').html('Seleccione una <strong>Cotización</strong>. Dato Obligatorio.');
        $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
        $('#ModalCuadroMensaje').dialog('open');
    };

    /* Botón Modificar */
    $('#TabGrupoFamiliar .grilla_editar').live('click', function () {
        idGrupoFamiliar = $(this).data('grupofamiliar');
        $.ajax({
            type: 'POST',
            url: '../RentaIFP/GrupoFamiliarAfiliado.aspx/SessionIdGrupoFamiliar',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "', paginaLlamada:'../RentaIFP/Cotizador.aspx#grupo_familiar'}",

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

    /* Cambiar valor en combobox de Indicador de Invalidez */
    $('#ModGruFamIndInvalidez_RP').live('change', function (e) {
        if ($(this).val() == 'S') {
            $('#ModGruFamTipoInvalidez_RP').removeAttr('disabled');
            $('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxReadOnlyContenedor');
            $('#ModGruFamFechaInvalidez_RP').removeAttr('disabled');
            $('#ModGruFamFechaInvalidez_RP').removeClass('formCalendarReadOnly');

            $('#AstModGruFamFechaInvalidez_RP').html('*');
        }
        else if ($(this).val() == 'N') {
            $('#ModGruFamTipoInvalidez_RP').val('N');
            $('#ModGruFamTipoInvalidez_RP').trigger('change');
            $('#ModGruFamTipoInvalidez_RP').attr('disabled', 'disabled');
            $('#ConModGruFamTipoInvalidez_RP').addClass('formComboboxReadOnlyContenedor');
            $('#ModGruFamFechaInvalidez_RP').val('');
            $('#ModGruFamFechaInvalidez_RP').trigger('change');
            $('#ModGruFamFechaInvalidez_RP').attr('disabled', 'disabled');
            $('#ModGruFamFechaInvalidez_RP').addClass('formCalendarReadOnly');

            $('#AstModGruFamFechaInvalidez_RP').html('');
        }
        else {
            $('#ModGruFamTipoInvalidez_RP').attr('disabled', 'disabled');
            $('#ConModGruFamTipoInvalidez').addClass('formComboboxReadOnlyContenedor');
            $('#ModGruFamFechaInvalidez_RP').attr('disabled', 'disabled');
            $('#ModGruFamFechaInvalidez_RP').addClass('formCalendarReadOnly');

            $('#AstModGruFamFechaInvalidez_RP').html('');
        }
    });

    var ModGruFamBotonesInactivos = false;
    /* Botón Aceptar */
    $('#ModGruFamAceptar_RP').live('click', function () {
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
            $('ModGruFamCorreoElectronico_RP').removeClass('formTextboxError');

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

            //// Tipo de invalidez
            //if ($('#ModGruFamTipoInvalidez_RP').val() == '0') {
            //    errores.push('Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.');
            //    tipoInvalidez = false;
            //}

            // Correo electronico
            var correoElectronico = true;
            if ($('#ModGruFamParentesco_RP').val() == '80') {
                if ($.trim($('#ModGruFamCorreoElectronico_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.');
                    correoElectronico = false;
                }
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
            if (!correoElectronico) $('#ModGruFamCorreoElectronico_RP').addClass('formTextboxError');

            //& invalidez & tipoInvalidez & fechaInvalidez

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo
                & fechaNacimiento & correoElectronico;

            if (!esCorrecto) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }

            /* Pasó las validaciones */
            ModGruFamBotonesInactivos = true;
            $('#ModGruFamAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#ModGruFamCargando_RP').fadeIn();

            if ($('#ModGruFamModo_RP').val() == 'N') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
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
                    banco: '',
                    tipoBanco: '',
                    comunicacion: '',
                    numerobanco: '',
                    confidencialidadDatos: ''
                }
                var postUrl = '../RentaIFP/GrupoFamiliarAfiliado.aspx/InsertarGrupoFamiliar';
            }
            else if ($('#ModGruFamModo_RP').val() == 'M') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    idGrupoFamiliar: idGrupoFamiliar,
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
                    correoElectronico: $('#ModGruFamCorreoElectronico_RP').val()
                }
                var postUrl = '../RentaIFP/GrupoFamiliarAfiliado.aspx/ModificarGrupoFamiliar';
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

                        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                        $clock.countdown(selectedDate.toString());

                        $('#MCMIcono').attr('class', 'exito');
                        if ($('#ModGruFamModo_RP').val() == 'N') {
                            $('#MCMContenedor').html('Grupo familiar agregado correctamente.');
                        }
                        else {
                            $('#MCMContenedor').html('Grupo familiar modificado correctamente.');
                        }

                        $('#ModalCuadroMensaje').dialog({ title: 'Infomación' });
                        $('#ModalCuadroMensaje').dialog('open');

                        window.location.href = $("#ModPaginaLlamada").val();

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
                            if (data.d.Controles[11].length) $('#ModGruFamCorreoElectronico_RP').attr('class', data.d.Controles[11]);
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
                        $('#MCMContenedor').html('Ha ocurrido un error al guardar la información del beneficiario.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                },
                complete: function () {
                    $('#ModGruFamCargando_RP').fadeOut();
                    ModGruFamBotonesInactivos = false;
                    $('#ModGruFamAceptar_RP').attr('class', 'boton darkblue sharp');
                }
            });
        }
    });

    /* Botón Cancelar */
    $('#ModGruFamCancelar_RP').live('click', function () {
        //<SOLINI25621>
        window.location.href = $("#ModPaginaLlamada").val();
        //window.location.href = "Cotizador.aspx#grupo_familiar";
        //<SOLFIN25621>
    });

    //<SRI.INI-20322_E2>
    /* SOLICITUDES */
    //var idSolicitud;
    //var Solicitud;
    //var idCotizacion;
    //var Correo;
    //<SRI.FIN-20322_E2>

    function NuevaSolicitud() {
        $('#ModSolModo').val('N');
        $('#BeneficiariosOriginales_RP').hide();
        $('#TablaRviBenefiContenedor_RP').hide();
        $('#ManSolPestanhas li:eq(0)').trigger('click');

        $('#ModSolCargando').show();
        $('#ModalSolicitud').dialog('open');

        LimpiarFormularioSolicitud();
        $('#ManSolTipoSolicitud_RP').val('EXTRAOFICIAL');

        $('#TabCotizacionesLeyenda_RP').hide();

        var params = {
            tokenUsuario: $("#TokenUsuario").val()
        }

        $('#ModSolImprimir_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CrearDatosSolicitud',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                /* Solicitud Creada */
                Solicitud = data.d;
                Solicitud.CoberturasAdicionales = [];
                Solicitud.Cotizaciones = [];
                ApiCotizadorIFP.ObtenerGrupoFamiliarPorCuspp($('#HCUSPP_RP').val())
                    .then(beneficiarios => {
                        GuardarBeneficiariosSesionPlan3IFP(beneficiarios);
                        RenderizarTablaBeneficiariosPlan3IFP(Solicitud, beneficiarios, null);
                    })
                    .catch(error => {
                        console.log("[error] ObtenerGrupoFamiliarPorCuspp", error);
                    });
                $("#ModSolTipoCambioPanel").hide();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $('#ModalSolicitud').dialog('close');

                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al crear los datos de la solicitud.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    };

    /* Botón Nueva */
    $('#NuevaSolicitud_IFP').live('click', function () {

        var validacion = false;

        if ($('#HConsentimiento').val() != "OK") {
            $('#MCATablaEliminar').val('cierre');
            $("#MCAIcono").attr("class", "advertencia");
            $("#MCAContenedor").html("No se puede crear una nueva solicitud porque el cliente no ha brindado su consentimiento para el tratamiento de datos personales, puede enviarle en enlace de consentimiento en la pestaña de Datos del Afiliado");
            $("#ModalCuadroAdvertencia").dialog({ title: "Validación" });
            $("#ModalCuadroAdvertencia").dialog("open");
            $("#ModalCotizando").dialog("close");
            validacion = true;
        }

        if (validacion == false) {
            if (permisoNuevaSolicitud) {

                idSolicitud = "";
                var params = {
                    idSolicitud: idSolicitud,
                    fecCotizacion: '',
                    accion: 'N'
                }

                Utilitarios.setStorageItem("modSolModo", params.accion);
                Utilitarios.setStorageItem("idSolicitud", params.idSolicitud);
                Utilitarios.setStorageItem("fecCotizacion", params.fecCotizacion);
                window.location.href = `MantenerSolicitud.aspx?modSolModo=${params.accion}&idSolicitud=${params.idSolicitud}&fecCotizacion=${params.fecCotizacion}`;

                // $.ajax({
                //     type: 'POST',
                //     url: 'Cotizador.aspx/SessionIdSolicitud',
                //     contentType: "application/json; charset=iso-8859-1",
                //     dataType: 'json',
                //     data: $.toJSON(params),
                //     success: function (data) {
                //         var solicitud = data.d;
                //         if (idSolicitud.toString() == solicitud.toString()) {
                //             window.location.href = "MantenerSolicitud.aspx";
                //         }
                //     },
                //     error: function (XMLHttpRequest, textStatus, errorThrown) {
                //         if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                //             document.location.reload(true);
                //         }
                //         else {
                //             $('#MCMIcono').attr('class', 'error');
                //             $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                //             $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                //             $('#ModalCuadroMensaje').dialog('open');
                //         }
                //         $('#ModalSolicitud').dialog('close');
                //     }
                // });


                //$('#ModSolModo').val('N');
                //$('#BeneficiariosOriginales_RP').hide();
                //$('#TablaRviBenefiContenedor_RP').hide();
                //$('#ManSolPestanhas li:eq(0)').trigger('click');

                //$('#ModSolCargando').show();
                //$('#ModalSolicitud').dialog('open');

                //LimpiarFormularioSolicitud();
                //$('#ManSolTipoSolicitud_RP').val('EXTRAOFICIAL');

                //$.ajax({
                //	type: 'POST',
                //	url: 'Cotizador.aspx/CrearDatosSolicitud',
                //	contentType: "application/json; charset=iso-8859-1",
                //	dataType: 'json',
                //	success: function (data) {
                //		/* Solicitud Creada */
                //		Solicitud = data.d;
                //		$('#ModSolFecUltActualizacion').val(new Date(+Solicitud.FechaSolicitud.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));

                //		/* Cargar Tabla de Cotizaciones con las cotizaciones iniciales */
                //		var params = {
                //			idTipoPension: $('#ModSolTipoPension').val()
                //		}
                //		$.ajax({
                //			type: 'POST',
                //			url: 'Cotizador.aspx/CargarComboProductos',
                //			contentType: "application/json; charset=iso-8859-1",
                //			dataType: 'json',
                //			data: $.toJSON(params),
                //			success: function (data) {
                //				CargarTablaCotizaciones(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud_RP').val());
                //				CargarTablaBeneficiarios_RP(null);

                //				$('#ModSolCargando').fadeOut();
                //				$('#ModSolTipoCambio').focus();
                //			},
                //			error: function (XMLHttpRequest, textStatus, errorThrown) {
                //				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                //					/* Sesión caducada */
                //					document.location.reload(true);
                //				}
                //				else {
                //					$('#ModalSolicitud').dialog('close');

                //					$('#MCMIcono').attr('class', 'error');
                //					$('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de productos.');
                //					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
                //					$('#ModalCuadroMensaje').dialog('open');
                //				}
                //			}
                //		});
                //	},
                //	error: function (XMLHttpRequest, textStatus, errorThrown) {
                //		if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                //			/* Sesión caducada */
                //			document.location.reload(true);
                //		}
                //		else {
                //			$('#ModalSolicitud').dialog('close');

                //			$('#MCMIcono').attr('class', 'error');
                //			$('#MCMContenedor').html('Ha ocurrido un error al crear los datos de la solicitud.');
                //			$('#ModalCuadroMensaje').dialog({ title: 'Error' });
                //			$('#ModalCuadroMensaje').dialog('open');
                //		}
                //	}
                //});
            }
            else {
                return false;
            }
        }
    });

    /* CargandoSolicitud  */
    async function CargandoSolicitud() {
        $('#ManSolPestanhas li:eq(0)').trigger('click');

        botonModSolAceptarBloqueado = false;
        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');

        const solicitud = await obtenerSolicitudIFPAsync();

        if (window.location.pathname.includes(EnumFuncionesVistas.CargarBeneficiariosPlan3IFP)) {
            const beneficiarios = await ApiCotizadorIFP.ObtenerGrupoFamiliarPorCuspp($('#HCUSPP_RP').val());
            GuardarBeneficiariosSesionPlan3IFP(beneficiarios);
            RenderizarTablaBeneficiariosPlan3IFP(solicitud, beneficiarios, null);
        }

    }

    async function obtenerSolicitudIFPAsync() {
        try {
            const queryParams = new URLSearchParams(window.location.search);
            const idSolicitud = document.getElementById("ModSolNroSolicitud_IFP").value || $("#ModSolNroSolicitud_IFP").text() || queryParams.get('idSolicitud');

            const response = await ApiCotizadorIFP.ObtenerSolicitudIFP(
                idSolicitud
            );

            Solicitud = response;

            if ($("#HBloqueo").val() == "TRUE") {
                if ($("#ModSolFechaCotizacion_IFP").val() != $("#HFechaActual").val()) {
                    botonModSolAceptarBloqueado = true;
                    $("#ModSolAceptar_RP").attr(
                        "class",
                        "botonDeshabilitado gris gris_sharp"
                    );

                    $("#ModSolTipoCambio_IFP").attr("disabled", "disabled");
                    $("#ModSolFechaCotizacion_IFP").attr("disabled", "disabled");
                    $("#ModSolFechaDevengue_IFP").attr("disabled", "disabled");
                    $("#ModSolFechaVigencia_IFP").attr("disabled", "disabled");
                    $("#ModSolPrimaUnica_IFP").attr("disabled", "disabled");
                    $("#ConModSolMonedaPrimaUnica_IFP").attr("disabled", "disabled");
                    $("#ConModSolPlan_IFP").attr("disabled", "disabled");

                    $("#ModSolPrimaUnica_IFP").attr(
                        "class",
                        "formTextbox formTextboxReadOnly"
                    );
                    $("#ModSolTipoCambio_IFP").attr(
                        "class",
                        "formTextbox formTextboxReadOnly"
                    );
                    $("#ConModSolMonedaPrimaUnica_IFP").addClass(
                        "formComboboxReadOnlyContenedor"
                    );
                    $("#ConModSolPlan_IFP").addClass("formComboboxReadOnlyContenedor");

                    $("#ModSolFechaCotizacion_IFP").attr(
                        "class",
                        "fecha formTextbox formCalendar formTextboxReadOnly"
                    );
                    $("#ModSolFechaDevengue_IFP").attr(
                        "class",
                        "fecha formTextbox formCalendar formTextboxReadOnly"
                    );
                    $("#ModSolFechaVigencia_IFP").attr(
                        "class",
                        "fecha formTextbox formCalendar formTextboxReadOnly"
                    );
                } else {
                    botonModSolAceptarBloqueado = false;
                    $("#ModSolAceptar_RP").attr("class", "boton darkblue sharp");
                }
            } else {
                botonModSolAceptarBloqueado = false;
                $("#ModSolAceptar_RP").attr("class", "boton darkblue sharp");
            }

            var bPlan1 = false;
            var bPlan2 = false;
            var bPlan3 = false;

            $("#TabCotizacionesLeyenda_RP").hide();

            for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                if (Solicitud.Cotizaciones[i].Plan.Id == "PLAN1") {
                    bPlan1 = true;
                }
                if (Solicitud.Cotizaciones[i].Plan.Id == "PLAN2") {
                    bPlan2 = true;
                }
                if (Solicitud.Cotizaciones[i].Plan.Id == "PLAN3") {
                    bPlan3 = true;
                }

                if (Solicitud.Cotizaciones[i].IndCotiza == "**") {
                    $("#TabCotizacionesLeyenda_RP").show();
                }
            }

            if (bPlan1) {
                if ($("#ModSolModo").val() == "CERRAR") {
                    CargarTablaCotizacionesCierre_RP(
                        Solicitud,
                        $("#ModSolMonedaPrimaUnica_IFP").val(),
                        "PLAN1"
                    );
                } else {
                    CargarTablaCotizaciones_RP(
                        Solicitud,
                        $("#ModSolMonedaPrimaUnica_IFP").val(),
                        "PLAN1"
                    );
                }
                Plan.push("PLAN1");
            }

            if (bPlan2) {
                if ($("#ModSolModo").val() == "CERRAR") {
                    CargarTablaCotizacionesCierre_RP(
                        Solicitud,
                        $("#ModSolMonedaPrimaUnica_IFP").val(),
                        "PLAN2"
                    );
                } else {
                    CargarTablaCotizaciones_RP(
                        Solicitud,
                        $("#ModSolMonedaPrimaUnica_IFP").val(),
                        "PLAN2"
                    );
                }
                Plan.push("PLAN2");
            }

            if (bPlan3) {
                if ($("#ModSolModo").val() == "CERRAR") {
                    CargarTablaCotizacionesCierre_RP(
                        Solicitud,
                        $("#ModSolMonedaPrimaUnica_IFP").val(),
                        "PLAN3"
                    );
                } else {
                    CargarTablaCotizaciones_RP(
                        Solicitud,
                        $("#ModSolMonedaPrimaUnica_IFP").val(),
                        "PLAN3"
                    );
                    if ($("#ModSolModo").val() == "CONS") {
                        $("#txtCAFecNacConyuge").attr("disabled", "disabled");
                        $("#ddlCASexoConyuge").attr("disabled", "disabled");
                        $("#txtCAFecNacPadre").attr("disabled", "disabled");
                        $("#txtCAFecNacMadre").attr("disabled", "disabled");
                    }
                }

                // 017
                $("#ManSolPes2").show();
                Plan.push("PLAN3");
            }

            if ($("#ModSolMonedaPrimaUnica_IFP").val() == "001") {
                $("#ModSolTipoCambioPanel").hide();
            } else {
                $("#ModSolTipoCambioPanel").show();
            }
        } catch (error) {
            console.error("Error en obtenerSolicitudIFPAsync:", error);

            $("#MCMIcono").attr("class", "error");

            $("#MCMContenedor").html(
                "Ha ocurrido un error al cargar la información de la solicitud."
            );

            $("#ModalCuadroMensaje").dialog({ title: "Error" });

            $("#ModalCuadroMensaje").dialog("open");

            $("#ModalSolicitud").dialog("close");
        }

        return Solicitud;
    }

    async function RenderizarTablaBeneficiariosPlan3IFP(solicitud, grupoFamiliar, ben) {
        if ($.trim($('#HCUSPP_RP').val()).length > 0) {
            const beneficiariosSeleccionados = solicitud ? solicitud.Beneficiarios : null;
            const params = {
                beneficiarios: grupoFamiliar || [],
                beneficiariosSeleccionados: beneficiariosSeleccionados
            }

            const beneficiariosEstimados = (beneficiariosSeleccionados || grupoFamiliar).filter(b => b.Parentesco.Id != EnumParentesco.Otros);

            if (!beneficiariosSeleccionados) {
                Solicitud.Beneficiarios = beneficiariosEstimados.map(b => ({ ...b, Seleccionado: true }));
            }

            console.log({
                beneficiariosEstimados,
                benegiciarios: Solicitud.Beneficiarios
            });

            $.ajax({
                type: 'POST',
                url: 'MantenerSolicitud.aspx/RenderTablaBeneficiariosPlan3IFP',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    if (ben == null) {
                        $('#TablaBeneficiariosCargando_RP').hide();
                        $('#TablaBeneficiariosContenedor_RP').show();
                        $('#ManSolNumBeneficiariosCargando_RP').hide();
                        $('#TablaBeneficiariosContenedor_RP').html($(data.d).find('#ContenidoDinamico').html());
                        //$('#ManSolNumBeneficiarios').show();
                        $('#ManSolNumBeneficiarios_RP').html('(' + beneficiariosEstimados.length + ')');
                    }
                    /*else {
                        $('#TablaRviBenefiCargando').hide();
                        $('#TablaRviBenefiContenedor').show();
                        $('#TablaRviBenefiContenedor').html($(data.d).find('#ContenidoDinamico').html());
                    }*/
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

    /*function CargandoSolicitud() {

        //$('#ModSolModo').val('M');
        //$('#BeneficiariosOriginales_RP').show();
        $('#ManSolPestanhas li:eq(0)').trigger('click');

        //$('#ModSolCargando').show();

        //var fecCotizacion = $("#ModSolFecCotizacion_RP").val();
        //idSolicitud = $("#ModSolNroSolicitud_RP").val();
        //LimpiarFormularioSolicitud();

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

                if (bPlan1) {
                    if ($('#ModSolModo').val() == "CERRAR") {
                        CargarTablaCotizacionesCierre_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
                    } else {
                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
                    }
                    Plan.push('PLAN1');
                }

                if (bPlan2) {
                    if ($('#ModSolModo').val() == "CERRAR") {
                        CargarTablaCotizacionesCierre_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
                    } else {
                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
                    }
                    Plan.push('PLAN2');
                }

                if (bPlan3) {
                    if ($('#ModSolModo').val() == "CERRAR") {
                        CargarTablaCotizacionesCierre_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
                    } else {
                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
                        if ($('#ModSolModo').val() == "CONS") {
                            $('#txtCAFecNacConyuge').attr('disabled', 'disabled');
                            $('#ddlCASexoConyuge').attr('disabled', 'disabled');
                            $('#txtCAFecNacPadre').attr('disabled', 'disabled');
                            $('#txtCAFecNacMadre').attr('disabled', 'disabled');
                        }
                    }

                    // 017
                    $('#ManSolPes2').show();
                    Plan.push('PLAN3');
                }

                //////<INIGTI_753>
                ////// Beneficiarios cotizados
                ////CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);

                ////// Grupo familiar
                ////CargarTablaBeneficiarios_RP(null);

                //////<FINGTI_753>

                ////// Lista de cotizaciones
                //////<INIGTI_7012>
                ////if ($('#ModSolModo').val() == "CERRAR") {
                ////    CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_IFP").val(), $("#HConyuge").val());
                ////} else {
                ////    CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_IFP").val(), $("#HConyuge").val());//<INIGTI_753>
                ////}
                //////<FINGTI_7012>


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

    }*/

    /* Botón Modificar */
    $('#TabSolicitudes_RP .grilla_editar').live('click', function () {

        var validacion = false;

        if ($('#HConsentimiento').val() != "OK") {
            $('#MCATablaEliminar').val('cierre');
            $("#MCAIcono").attr("class", "advertencia");
            $("#MCAContenedor").html("No se puede editar la solicitud " + $(this).data('solicitud') + " porque el cliente no ha brindado su consentimiento para el tratamiento de datos personales, puede enviarle en enlace de consentimiento en la pestaña de Datos del Afiliado");
            $("#ModalCuadroAdvertencia").dialog({ title: "Validación" });
            $("#ModalCuadroAdvertencia").dialog("open");
            $("#ModalCotizando").dialog("close");
            validacion = true;
        }

        if (validacion == false) {
            var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
            idSolicitud = $(this).data('solicitud');
            var params = {
                idSolicitud: idSolicitud,
                fecCotizacion: fecCotizacion,
                accion: 'M'
            }

            Utilitarios.setStorageItem("modSolModo", params.accion);
            Utilitarios.setStorageItem("idSolicitud", params.idSolicitud);
            Utilitarios.setStorageItem("fecCotizacion", params.fecCotizacion);
            window.location.href = `MantenerSolicitud.aspx?modSolModo=${params.accion}&idSolicitud=${params.idSolicitud}&fecCotizacion=${params.fecCotizacion}`;

            // $.ajax({
            //     type: 'POST',
            //     url: 'Cotizador.aspx/SessionIdSolicitud',
            //     contentType: "application/json; charset=iso-8859-1",
            //     dataType: 'json',
            //     data: $.toJSON(params),
            //     success: function (data) {
            // 
            //         var solicitud = data.d;
            //         if (idSolicitud.toString() == solicitud.toString()) {
            //             window.location.href = "MantenerSolicitud.aspx";
            //         }
            //     },
            //     error: function (XMLHttpRequest, textStatus, errorThrown) {
            //         if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
            //             document.location.reload(true);
            //         }
            //         else {
            //             $('#MCMIcono').attr('class', 'error');
            //             $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
            //             $('#ModalCuadroMensaje').dialog({ title: 'Error' });
            //             $('#ModalCuadroMensaje').dialog('open');
            //         }
            //         $('#ModalSolicitud').dialog('close');
            //     }
            // });
        }



    });

    //<INIGTI_753_3>

    /*Botón copiar*/
    $('#TabSolicitudes_RP .grilla_copiar').live('click', function () {

        var validacion = false;

        if ($('#HConsentimiento').val() != "OK") {
            $('#MCATablaEliminar').val('cierre');
            $("#MCAIcono").attr("class", "advertencia");
            $("#MCAContenedor").html("No se puede clonar la solicitud " + $(this).data('solicitud') + " porque el cliente no ha brindado su consentimiento para el tratamiento de datos personales, puede enviarle en enlace de consentimiento en la pestaña de Datos del Afiliado");
            $("#ModalCuadroAdvertencia").dialog({ title: "Validación" });
            $("#ModalCuadroAdvertencia").dialog("open");
            $("#ModalCotizando").dialog("close");
            validacion = true;
        }

        if (validacion == false) {
            var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
            idSolicitud = $(this).data('solicitud');
            var params = {
                idSolicitud: idSolicitud,
                fecCotizacion: fecCotizacion,
                accion: 'C'
            }

            Utilitarios.setStorageItem("modSolModo", params.accion);
            Utilitarios.setStorageItem("idSolicitud", params.idSolicitud);
            Utilitarios.setStorageItem("fecCotizacion", params.fecCotizacion);
            window.location.href = `MantenerSolicitud.aspx?modSolModo=${params.accion}&idSolicitud=${params.idSolicitud}&fecCotizacion=${params.fecCotizacion}`;


            // $.ajax({
            //     type: 'POST',
            //     url: 'Cotizador.aspx/SessionIdSolicitud',
            //     contentType: "application/json; charset=iso-8859-1",
            //     dataType: 'json',
            //     data: $.toJSON(params),
            //     success: function (data) {
            //         var solicitud = data.d;
            //         if (idSolicitud.toString() == solicitud.toString()) {
            //             Utilitarios.setStorageItem("modSolModo", params.accion);
            //             Utilitarios.setStorageItem("idSolicitud", params.idSolicitud);
            //             Utilitarios.setStorageItem("fecCotizacion", params.fecCotizacion);
            //             window.location.href = "MantenerSolicitud.aspx";
            //         }
            //     },
            //     error: function (XMLHttpRequest, textStatus, errorThrown) {
            //         if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
            //             document.location.reload(true);
            //         }
            //         else {
            //             $('#MCMIcono').attr('class', 'error');
            //             $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
            //             $('#ModalCuadroMensaje').dialog({ title: 'Error' });
            //             $('#ModalCuadroMensaje').dialog('open');
            //         }
            //         $('#ModalSolicitud').dialog('close');
            //     }
            // });
        }
    });

    /*Botón Consultar*/
    $('#TabSolicitudes_RP .grilla_consultar').live('click', function () {

        var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
        idSolicitud = $(this).data('solicitud');
        var params = {
            idSolicitud: idSolicitud,
            fecCotizacion: fecCotizacion,
            accion: 'CONS'
        }

        Utilitarios.setStorageItem("modSolModo", params.accion);
        Utilitarios.setStorageItem("idSolicitud", params.idSolicitud);
        Utilitarios.setStorageItem("fecCotizacion", params.fecCotizacion);
        window.location.href = `MantenerSolicitud.aspx?modSolModo=${params.accion}&idSolicitud=${params.idSolicitud}&fecCotizacion=${params.fecCotizacion}`;

        // $.ajax({
        //     type: 'POST',
        //     url: 'Cotizador.aspx/SessionIdSolicitud',
        //     contentType: "application/json; charset=iso-8859-1",
        //     dataType: 'json',
        //     data: $.toJSON(params),
        //     success: function (data) {

        //         var solicitud = data.d;
        //         if (idSolicitud.toString() == solicitud.toString()) {
        //             window.location.href = "MantenerSolicitud.aspx";
        //         }
        //     },
        //     error: function (XMLHttpRequest, textStatus, errorThrown) {
        //         if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //             document.location.reload(true);
        //         }
        //         else {
        //             $('#MCMIcono').attr('class', 'error');
        //             $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
        //             $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        //             $('#ModalCuadroMensaje').dialog('open');
        //         }
        //         $('#ModalSolicitud').dialog('close');
        //     }
        // });



    });

    ////<FINGTI_753_3>
    ////<INIGTI_7012>
    //$('#TabSolicitudes_RP .grilla_cerrar').live('click', function () {

    //    var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
    //    idSolicitud = $(this).data('solicitud');
    //    var params = {
    //        idSolicitud: idSolicitud,
    //        fecCotizacion: fecCotizacion,
    //        accion: 'CERRAR'
    //    }

    //    $.ajax({
    //        type: 'POST',
    //        url: 'Cotizador.aspx/SessionIdSolicitud',
    //        contentType: "application/json; charset=iso-8859-1",
    //        dataType: 'json',
    //        data: $.toJSON(params),
    //        success: function (data) {

    //            var solicitud = data.d;
    //            if (idSolicitud.toString() == solicitud.toString()) {
    //                window.location.href = "SeleccionSolicitud.aspx";
    //            }
    //        },
    //        error: function (XMLHttpRequest, textStatus, errorThrown) {
    //            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
    //                document.location.reload(true);
    //            }
    //            else {
    //                $('#MCMIcono').attr('class', 'error');
    //                $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
    //                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
    //                $('#ModalCuadroMensaje').dialog('open');
    //            }
    //            $('#ModalSolicitud').dialog('close');
    //        }
    //    });
    //});

    ////<FINGTI_7012>

    //<INIGTI_7012>

    $('#TabSolicitudes_RP .grilla_cerrar, #TabSolicitudes_RP .grilla_cerrar_seleccionado').live('click', function () {
        const idSolicitud = $(this).data('solicitud');
        const fechaSolicitud = $(this).parent('td').parent('tr').children().eq(1).html();
        const estadoSolicitud = $(this).data('estado');
        console.log({ idSolicitud, fechaSolicitud, estadoSolicitud })
        CerrarSolicitud.ValidarCierre(idSolicitud, fechaSolicitud, estadoSolicitud);
    });
    //<FINGTI_7012>

    $('#TablaRviBenefiReintentar_RP').live('click', function () {
        $('#TablaRviBenefiError_RP').hide();
        CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);
    });

    /* Botón PDF */
    $("#TabSolicitudes_RP .grilla_pdf, #TabCotizacionCotizaciones .grilla_pdf").live("click", function () {

        var idSolicitud = $(this).data('solicitud');
        //var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
        var fecCotizacion = $(this).data('fechacotizacion');
        var numAgente = $(this).data("numagente");
        var cuspp = $("#CUSPP_RP").val();

        var params = {
            idSolicitud: idSolicitud,
            fecCotizacion: fecCotizacion,
            numAgente: numAgente
        }

        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/ExportarSolicitudPDF',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {

                    /*if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                        window.location.href = "../Reportes/DetallePropuestaIFPMovil.aspx";
                    }
                    else {
                        var w = 800;
                        var h = 600;
                        var left = (screen.width / 2) - (w / 2);
                        var top = (screen.height / 2) - (h / 2);
                        //<SOLINI26593>
                        var nuevaVentana = window.open("../Reportes/DetallePropuestaIFP.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                        //<SOLFIN26593>
                        //var nuevaVentana = window.open("../Reportes/DetalleCotizacion.aspx", "", "width=800,height=600,scrollbars=1,location=no,menubar=no,resizable=1,status=no,toolbar=no");
                    }*/

                    window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + idSolicitud + "&formato=8");

                    if (data.d.Contenido == "1") {
                        window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + idSolicitud + "&formato=7");
                    }

                    /*<SRIINI17003>*/
                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                    /*<SRIFIN17003>*/
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
                    $('#MCMContenedor').html('Ha ocurrido un error al exportar la la solicitud.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });

    /* Botón Correo Electrónico */
    $('#TabSolicitudes_RP .grilla_email').live('click', function () {
        if ($.trim($('#CorreoElectronicoRegistrado_RP').val()).length > 0) {
            $('#ModEnvCorCargando').show();

            var idSolicitud = $(this).data('solicitud');
            var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
            var tipoCotizacion = $(this).parent('td').parent('tr').children().eq(2).find('span').html();

            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                idSolicitud: idSolicitud,
                fecCotizacion: fecCotizacion,
                tipoCotizacion: tipoCotizacion,
                nombre: $.trim($('#Nombres_RP').val()),
                apellidoPaterno: $.trim($('#ApellidoPaterno_RP').val()),
                apellidoMaterno: $.trim($('#ApellidoMaterno_RP').val()),
                sexo: $('#Sexo_RP').val()
            }

            LimpiarFormularioCorreo();

            $.ajax({
                type: 'POST',
                url: 'Cotizador.aspx/CrearDatosCorreo',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    /* Correo Creado */
                    Correo = data.d;
                    if (Correo.Respuesta.Estado == 'OK') {
                        $('#ModEnvCorDe').val(Correo.De);
                        Correo.Para = $('#CorreoElectronicoRegistrado_RP').val();
                        $('#ModEnvCorPara').val(Correo.Para);
                        $('#ModEnvCorAsunto').val(Correo.Asunto);
                        $('#ModEnvCorAdjunto').html(Correo.Adjunto);
                        $('#ModEnvCorMensaje').val(Correo.Mensaje);
                        localStorage.setItem('pdf_cotizacion', Correo.BinarioAdjunto);
                        $('#ModEnvCorCargando').fadeOut();
                        $('#ModEnvCorMensaje').focus();
                    }
                    else if (Correo.Respuesta.Estado == 'TOKEN') {
                        CerrarSesionExpirada();
                    }
                    else {
                        $('#MCMIcono').attr('class', Correo.Respuesta.Icono);
                        $('#MCMContenedor').html(Correo.Respuesta.Mensaje);
                        $('#ModalCuadroMensaje').dialog({ title: Correo.Respuesta.Titulo });
                        $('#ModalCuadroMensaje').dialog('open');

                        $('#ModalEnvioCorreo').dialog('close');
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                        document.location.reload(true);
                    }
                    else {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html('Ha ocurrido un error al crear los datos del correo electrónico.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                    $('#ModalEnvioCorreo').dialog('close');
                }
            });

            $('#ModalEnvioCorreo').dialog('open');
        }
        else {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html('No se tiene registrado un Correo Electrónico para el afiliado. Por favor ingrese uno en la pestaña <strong>Datos del Afiliado</strong> y luego presione el botón <strong>Guardar</strong>.');
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
        }
    });

    $("#TabSolicitudes_RP .grilla_pdf_poliza").live('click', function (e) {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Generando PDF de la póliza, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Generando" });
        $("#ModalCotizando").dialog("open");

        var num_poliza = $(this).data('poliza');

        var params = {
            numPoliza: num_poliza
        }

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/ImprimirPoliza",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    // Mostrar mensaje de éxito
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    for (var nombre = 0; nombre < data.d.Archivos.length; nombre++) {
                        window.open('../ArchivosTemporales/IFP/Poliza/' + data.d.Archivos[nombre], '_blank');
                    }

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
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
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al generar PDF de la póliza.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
                $("#ModalCotizando").dialog("close");
            }
        });
    });

    /* Botón Enviar Correo Electrónico */
    $('#ModEnvCorEnviar').live('click', function () {
        $('#ModEnvCorCargando').fadeIn();

        Correo.Mensaje = $('#ModEnvCorMensaje').val();
        Correo.Respuesta = null;
        Correo.BinarioAdjunto = localStorage.getItem("pdf_cotizacion");
        let valorCompleto = $('#Agente').val() || '';
        let codigoAgente = null;
        if (valorCompleto.trim() !== '') {
            codigoAgente = valorCompleto.split(' - ')[0];
        }
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            correo: Correo,
            agente: codigoAgente
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/EnviarCorreoElectronico',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    $('#ModalEnvioCorreo').dialog('close');

                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    /*<SRIINI17003>*/
                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                    /*<SRIFIN17003>*/
                }
                else if (data.d.Estado == 'TOKEN') {
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
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');

                    $('#ModEnvCorCargando').fadeOut();
                }
            }
        });
    });

    /* Botón Cancelar Correo Electrónico */
    $('#ModEnvCorCancelar').live('click', function () {
        $('#ModalEnvioCorreo').dialog('close');
        //<SRI.INI-20322>
        $('#ModalEnvioCorreoSimulador').dialog('close');
        //<SRI.FIN-20322>
    });

    /* Botón Agregar */
    $('#TabCotizacionesAgregarPlan1_IFP, #TabCotizacionesAgregarPlan2_IFP, #TabCotizacionesAgregarPlan3_IFP').live('click', function () {
        var plan = "";

        if (this.id == "TabCotizacionesAgregarPlan1_IFP") {
            plan = 'PLAN1';
        } else if (this.id == "TabCotizacionesAgregarPlan2_IFP") {
            plan = 'PLAN2';
        } else if (this.id == "TabCotizacionesAgregarPlan3_IFP") {
            plan = 'PLAN3';
        }

        var params = {
            cotizaciones: Solicitud.Cotizaciones,
            plan: plan
        }

        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/AgregarCotizacionASolicitud',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {

                Solicitud.Cotizaciones = data.d;

                CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), plan);

                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                $clock.countdown(selectedDate.toString());

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al agregar una nueva cotización.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });

    /* Cambiar combobox Tabla Cotizaciones */
    $('#TabCotizaciones_IFP_P1 select').live('change', async function () {
        var celda = $(this).parent('td');
        var col = celda.parent('tr').children().index(celda);

        var fila = celda.parent('tr');
        var fil = fila.find('.ModSolItem').val();

        switch (col) {

            case 1: //Moneda
                Solicitud.Cotizaciones[fil].Moneda.Id = $(this).val();
                Solicitud.Cotizaciones[fil].Moneda.Nombre = $('option:selected', this).text();

                if ($('option:selected', this).text() == "Ajustados") {
                    Solicitud.Cotizaciones[fil].ValMonAju = 2.00;
                } else {
                    Solicitud.Cotizaciones[fil].ValMonAju = 0.00;
                }

                // if (Solicitud.Cotizaciones[fil].Moneda.Id != "000") {
                //     CargarParametroGeneralMotorIFP(Solicitud.Cotizaciones[fil].Temporalidad.Id,
                //         Solicitud.Cotizaciones[fil].Moneda.Id, $('#ModSolFechaCotizacion_IFP').val());
                // }

                break;
            case 2: //Temporalidad
                Solicitud.Cotizaciones[fil].Temporalidad.Id = $(this).val();
                Solicitud.Cotizaciones[fil].Temporalidad.Anhos = $('option:selected', this).text();

                await ObtenerPagoDiferimiento($('option:selected', this).text(), fila.find('.ModSolDiferimiento'), 'PLAN1', fila);

                fila.find('.ModSolSobrevivencia').removeAttr("disabled");
                fila.find('.ModSolFallecimiento').removeAttr("disabled");

                // if (Solicitud.Cotizaciones[fil].Moneda.Id != "000") {
                //     CargarParametroGeneralMotorIFP(Solicitud.Cotizaciones[fil].Temporalidad.Id,
                //         Solicitud.Cotizaciones[fil].Moneda.Id, $('#ModSolFechaCotizacion_IFP').val());
                // }

                //Full Diferido
                if (fila.find('.ModSolDiferimiento option:selected').text() == fila.find('.ModSolTemporalidad option:selected').text()) {
                    ObtenerMonedaFullDiferido($('#ModSolMonedaPrimaUnica_IFP').val(), fila.find('.ModSolMoneda'), fila)
                } else {
                    ObtenerMoneda($('#ModSolMonedaPrimaUnica_IFP').val(), fila.find('.ModSolMoneda'), fila)
                }

                break;
            case 3: //Diferimiento
                Solicitud.Cotizaciones[fil].ValPerDiferido = $(this).val();

                await ObtenerPagoDoble(Solicitud.Cotizaciones[fil].Temporalidad.Anhos, fila.find('.ModSolDiferimiento').val(), fila.find('.ModSolPagoEscalonada'), fila);

                //if (fila.find('.ModSolTemporalidad option:selected').text() == 5 && $(this).val() == 5) {
                //Full Diferido
                if (fila.find('.ModSolTemporalidad option:selected').text() == $(this).val()) {
                    fila.find('.ModSolSobrevivencia').val('100');
                    Solicitud.Cotizaciones[fil].ValPjeDev = 100;
                    Solicitud.Cotizaciones[fil].ValPjeDevFallec = 100;
                    await ObtenerDevFallecimiento(fila.find('.ModSolSobrevivencia').val(), fila.find('.ModSolFallecimiento'), true, fila.find('.ModSolFallecimiento'), fila);
                    fila.find('.ModSolSobrevivencia').attr('disabled', 'disabled');
                    fila.find('.ModSolFallecimiento').attr('disabled', 'disabled');

                    fila.find('.ModSolPagoEscalonada').attr('disabled', 'disabled');

                    ObtenerMonedaFullDiferido($('#ModSolMonedaPrimaUnica_IFP').val(), fila.find('.ModSolMoneda'), fila)

                } else {
                    fila.find('.ModSolSobrevivencia').removeAttr("disabled");
                    fila.find('.ModSolFallecimiento').removeAttr("disabled");
                    fila.find('.ModSolPagoEscalonada').removeAttr("disabled");

                    ObtenerMoneda($('#ModSolMonedaPrimaUnica_IFP').val(), fila.find('.ModSolMoneda'), fila)
                }

                break;
            case 4: //Pago Escalanodo ex doble
                Solicitud.Cotizaciones[fil].PagoDoble = $(this).val();

                if ($(this).val() == 0) {
                    fila.find('.ModSolTramoEscalonada').attr('disabled', 'disabled');
                    fila.find('.ModSolTramoEscalonada').append(new Option("100%", "0"));
                    fila.find('.ModSolTramoEscalonada').val('0');

                    Solicitud.Cotizaciones[fil].PjePagoDoble = fila.find('.ModSolTramoEscalonada').val();
                } else {
                    fila.find('.ModSolTramoEscalonada').removeAttr("disabled");
                    fila.find('.ModSolTramoEscalonada').find('[value="0"]').remove();

                    var pjeCalculo = ((100 / fila.find('.ModSolTramoEscalonada').val()) * 100).toFixed(6)
                    Solicitud.Cotizaciones[fil].PjePagoDoble = pjeCalculo;
                }

                break;
            case 5: //% Escalonada
                Solicitud.Cotizaciones[fil].PjePagoDoble = ((100 / $(this).val()) * 100).toFixed(6);
                break;
            case 6: //Devolución de Sobrevivencia
                Solicitud.Cotizaciones[fil].ValPjeDev = $(this).val(); //$('#TabCotDevolucion').val();

                await ObtenerDevFallecimiento($(this).val(), fila.find('.ModSolFallecimiento'), false, fila.find('.ModSolFallecimiento'), fila);
                break;
            case 7: //Devolución de Fallecimiento
                Solicitud.Cotizaciones[fil].ValPjeDevFallec = $(this).val();
                break;
            case 8: //DCOM
                Solicitud.Cotizaciones[fil].ValPjeDCOM = $(this).val();
                break;

        }

    });

    /* Cambiar combobox Tabla Cotizaciones */
    $('#TabCotizaciones_IFP_P2 select').live('change', async function () {
        var celda = $(this).parent('td');
        var col = celda.parent('tr').children().index(celda);

        var fila = celda.parent('tr');
        //var fil = fila.parent('tbody').children().index(fila);
        var fil = fila.find('.ModSolItem').val();

        switch (col) {

            case 1: //Moneda
                Solicitud.Cotizaciones[fil].Moneda.Id = $(this).val();
                Solicitud.Cotizaciones[fil].Moneda.Nombre = $('option:selected', this).text();

                if ($('option:selected', this).text() == "Ajustados") {
                    Solicitud.Cotizaciones[fil].ValMonAju = 2.00;
                } else {
                    Solicitud.Cotizaciones[fil].ValMonAju = 0.00;
                }

                // if (Solicitud.Cotizaciones[fil].Moneda.Id != "000") {
                //     CargarParametroGeneralMotorIFP(Solicitud.Cotizaciones[fil].Temporalidad.Id,
                //         Solicitud.Cotizaciones[fil].Moneda.Id, $('#ModSolFechaCotizacion_IFP').val());
                // }

                break;
            case 2: //Temporalidad
                Solicitud.Cotizaciones[fil].Temporalidad.Id = $(this).val();
                Solicitud.Cotizaciones[fil].Temporalidad.Anhos = $('option:selected', this).text();

                Solicitud.Cotizaciones[fil].PeriodoGarantizado = $('option:selected', this).text(); //Full Garantizado

                await ObtenerPagoDiferimiento($('option:selected', this).text(), fila.find('.ModSolDiferimiento'), 'PLAN2', fila);

                // if (Solicitud.Cotizaciones[fil].Moneda.Id != "000") {
                //     CargarParametroGeneralMotorIFP(Solicitud.Cotizaciones[fil].Temporalidad.Id,
                //         Solicitud.Cotizaciones[fil].Moneda.Id, $('#ModSolFechaCotizacion_IFP').val());
                // }

                //Full Diferido
                if (fila.find('.ModSolDiferimiento option:selected').text() == fila.find('.ModSolTemporalidad option:selected').text()) {
                    ObtenerMonedaFullDiferido($('#ModSolMonedaPrimaUnica_IFP').val(), fila.find('.ModSolMoneda'), fila)
                } else {
                    ObtenerMoneda($('#ModSolMonedaPrimaUnica_IFP').val(), fila.find('.ModSolMoneda'), fila)
                }

                break;
            case 3: //Diferimiento
                Solicitud.Cotizaciones[fil].ValPerDiferido = $(this).val();

                await ObtenerPagoDoble(Solicitud.Cotizaciones[fil].Temporalidad.Anhos, fila.find('.ModSolDiferimiento').val(), fila.find('.ModSolPagoEscalonada'), fila);

                if (fila.find('.ModSolTemporalidad option:selected').text() == $(this).val()) {
                    fila.find('.ModSolPagoEscalonada').attr('disabled', 'disabled');

                    Solicitud.Cotizaciones[fil].ValPjeDev = 100;
                    fila.find('.ModSolDevolucion').val('100');
                    fila.find('.ModSolDevolucion').attr('disabled', 'disabled');

                    ObtenerMonedaFullDiferido($('#ModSolMonedaPrimaUnica_IFP').val(), fila.find('.ModSolMoneda'), fila)

                } else {
                    fila.find('.ModSolPagoEscalonada').removeAttr("disabled");
                    ObtenerMoneda($('#ModSolMonedaPrimaUnica_IFP').val(), fila.find('.ModSolMoneda'), fila)
                }

                break;
            case 4: //Pago escalonado
                Solicitud.Cotizaciones[fil].PagoDoble = $(this).val();

                if ($(this).val() == 0) {
                    Solicitud.Cotizaciones[fil].PjePagoDoble = 0;

                    fila.find('.ModSolTramoEscalonada').attr('disabled', 'disabled');
                    fila.find('.ModSolTramoEscalonada').append(new Option("100%", "0"));
                    fila.find('.ModSolTramoEscalonada').val('0');

                    //Solicitud.Cotizaciones[fil].PjePagoDoble = fila.find('.ModSolTramoEscalonada').val();
                } else {
                    fila.find('.ModSolTramoEscalonada').removeAttr("disabled");
                    fila.find('.ModSolTramoEscalonada').find('[value="0"]').remove();

                    var pjeCalculo = ((100 / fila.find('.ModSolTramoEscalonada').val()) * 100).toFixed(6)
                    Solicitud.Cotizaciones[fil].PjePagoDoble = pjeCalculo;
                }

                break;
            case 5: //% Escalonada
                Solicitud.Cotizaciones[fil].PjePagoDoble = ((100 / $(this).val()) * 100).toFixed(6);
                break;
            case 6: //Devolución de Sobrevivencia
                Solicitud.Cotizaciones[fil].ValPjeDev = $(this).val();
                break;
            case 7: //DCOM
                Solicitud.Cotizaciones[fil].ValPjeDCOM = $(this).val();
                break;
        }
    });

    $('#TabCotizaciones_IFP_P2 input[type=checkbox]').live('change', function () {
        var celda = $(this).parent('span').parent('td');

        var fila = celda.parent('tr');
        var fil = fila.find('.ModSolItem').val();

        switch (this.id) {
            case 'TabCotRescate': //Rescate
                Solicitud.Cotizaciones[fil].IndRescate = this.checked;
                break;
        }
    });

    /* Cambiar combobox Tabla Cotizaciones */
    $('#TabCotizaciones_IFP_P3 select').live('change', async function () {
        var celda = $(this).parent('td');
        var col = celda.parent('tr').children().index(celda);

        var fila = celda.parent('tr');
        var fil = fila.find('.ModSolItem').val();

        var tabla = $(this).closest('table').attr('id');

        var filaGrillaPlan = parseInt(fila[0].children[0].innerText) - 1;

        switch (col) {

            case 1: //Moneda
                Solicitud.Cotizaciones[fil].Moneda.Id = $(this).val();
                Solicitud.Cotizaciones[fil].Moneda.Nombre = $('option:selected', this).text();

                if ($('option:selected', this).text() == "Ajustados") {
                    Solicitud.Cotizaciones[fil].ValMonAju = 2.00;
                } else {
                    Solicitud.Cotizaciones[fil].ValMonAju = 0.00;
                }

                // if (Solicitud.Cotizaciones[fil].Moneda.Id != "000") {
                //     CargarParametroGeneralMotorIFP(Solicitud.Cotizaciones[fil].Temporalidad.Id,
                //         Solicitud.Cotizaciones[fil].Moneda.Id, $('#ModSolFechaCotizacion_IFP').val());
                // }

                break;
            case 2: //Periodo Garantizado
                Solicitud.Cotizaciones[fil].PeriodoGarantizado = $(this).val();

                //ObtenerPagoDobleCPeriodoGarantizado($(this).val() - Solicitud.Cotizaciones[fil].ValPerDiferido, fila.find('.ModSolPagoEscalonada'), fila);
                //ObtenerDiferimientoCPeriodoGarantizado($(this).val() - Solicitud.Cotizaciones[fil].ValPerDiferido, fila.find('.ModSolDiferimiento'), fila);
                let periodoGarantizado = $(this).val();
                await ObtenerDiferimientoCPeriodoGarantizado(periodoGarantizado, fila.find('.ModSolDiferimiento'), fila);

                //if (Solicitud.Cotizaciones[fil].ValPerDiferido == 0) {
                //    ObtenerPagoDobleCPeriodoGarantizado($(this).val(), fila.find('.ModSolPagoEscalonada'), fila);
                //}

                break;
            case 3: //Diferimiento
                Solicitud.Cotizaciones[fil].ValPerDiferido = $(this).val();

                let diferimiento = $(this).val();
                let periodoGarantizadoDif = Solicitud.Cotizaciones[fil].PeriodoGarantizado - diferimiento;
                if (Solicitud.Cotizaciones[fil].PeriodoGarantizado == 0) {
                    periodoGarantizadoDif = 16;
                }
                await ObtenerPagoDobleCPeriodoGarantizado(periodoGarantizadoDif, fila.find('.ModSolPagoEscalonada'), fila);

                break;
            case 4: //Escalonado
                Solicitud.Cotizaciones[fil].PagoDoble = $(this).val();

                /*if ($(this).val() != 0) {
                    Solicitud.Cotizaciones[fil].PjePagoDoble = 50;
                } else {
                    Solicitud.Cotizaciones[fil].PjePagoDoble = 0;
                }*/

                if ($(this).val() == 0) {
                    Solicitud.Cotizaciones[fil].PjePagoDoble = 0;

                    fila.find('.ModSolTramoEscalonada').attr('disabled', 'disabled');
                    fila.find('.ModSolTramoEscalonada').append(new Option("100%", "0"));
                    fila.find('.ModSolTramoEscalonada').val('0');
                } else {
                    fila.find('.ModSolTramoEscalonada').removeAttr("disabled");
                    fila.find('.ModSolTramoEscalonada').find('[value="0"]').remove();

                    var pjeCalculo = ((100 / fila.find('.ModSolTramoEscalonada').val()) * 100).toFixed(6)
                    Solicitud.Cotizaciones[fil].PjePagoDoble = pjeCalculo;
                }

                break;
            case 5: //% Escalonada
                Solicitud.Cotizaciones[fil].PjePagoDoble = ((100 / $(this).val()) * 100).toFixed(6);
                break;
                /*case 5: //CA Cy
                    Solicitud.Cotizaciones[fil].ValPjeCACy = $(this).val();
                    Solicitud.Cotizaciones[fil].ValPjeCATotal = (parseInt(Solicitud.Cotizaciones[fil].ValPjeCACy) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAPa) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAMa)) + "%";
                    $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').text(Solicitud.Cotizaciones[fil].ValPjeCATotal);
    
                    if ((parseInt(Solicitud.Cotizaciones[fil].ValPjeCACy) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAPa) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAMa)) > 100) {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(5) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(6) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(7) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').addClass('formTextboxGridError');
                        $('#TabCotizacionesAgregarPlan3_IFP').hide();
                        $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                    } else {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(5) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(6) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(7) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').removeClass('formTextboxGridError');
                        $('#TabCotizacionesAgregarPlan3_IFP').show();
                        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                    }
    
                    break;*/
                /*case 6: //CA Pa
                    Solicitud.Cotizaciones[fil].ValPjeCAPa = $(this).val();
                    Solicitud.Cotizaciones[fil].ValPjeCATotal = (parseInt(Solicitud.Cotizaciones[fil].ValPjeCACy) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAPa) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAMa)) + "%"
                    $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').text(Solicitud.Cotizaciones[fil].ValPjeCATotal);
    
                    if ((parseInt(Solicitud.Cotizaciones[fil].ValPjeCACy) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAPa) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAMa)) > 100) {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(5) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(6) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(7) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').addClass('formTextboxGridError');
                        $('#TabCotizacionesAgregarPlan3_IFP').hide();
                        $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                    } else {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(5) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(6) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(7) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').removeClass('formTextboxGridError');
                        $('#TabCotizacionesAgregarPlan3_IFP').show();
                        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                    }
    
                    break;*/
                /*case 7: //CA Ma
                    Solicitud.Cotizaciones[fil].ValPjeCAMa = $(this).val();
                    Solicitud.Cotizaciones[fil].ValPjeCATotal = (parseInt(Solicitud.Cotizaciones[fil].ValPjeCACy) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAPa) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAMa)) + "%"
                    $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').text(Solicitud.Cotizaciones[fil].ValPjeCATotal);
    
                    if ((parseInt(Solicitud.Cotizaciones[fil].ValPjeCACy) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAPa) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAMa)) > 100) {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(5) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(6) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(7) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').addClass('formTextboxGridError');
                        $('#TabCotizacionesAgregarPlan3_IFP').hide();
                        $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                    } else {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(5) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(6) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(7) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').removeClass('formTextboxGridError');
                        $('#TabCotizacionesAgregarPlan3_IFP').show();
                        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                    }
    
                    break;*/
                /*case 8: //CA Tot
                    Solicitud.Cotizaciones[fil].ValPjeCATotal = (parseInt(Solicitud.Cotizaciones[fil].ValPjeCACy) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAPa) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAMa)) + "%"
                    $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').text(Solicitud.Cotizaciones[fil].ValPjeCATotal);
    
                    if ((parseInt(Solicitud.Cotizaciones[fil].ValPjeCACy) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAPa) + parseInt(Solicitud.Cotizaciones[fil].ValPjeCAMa)) > 100) {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(5) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(6) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(7) select').addClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').addClass('formTextboxGridError');
                        $('#TabCotizacionesAgregarPlan3_IFP').hide();
                        $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                    } else {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(5) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(6) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(7) select').removeClass('formTextboxGridError');
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + filaGrillaPlan + ') td:eq(8)').removeClass('formTextboxGridError');
                        $('#TabCotizacionesAgregarPlan3_IFP').show();
                        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                    }*/

                break;
            case 6: //DCOM
                Solicitud.Cotizaciones[fil].ValPjeDCOM = $(this).val();
                break;

        }

    });

    $('#TabClausulaAdicional input').live('change keyup', function () {
        var idEtiqueta = this.id;
        if (idEtiqueta == 'txtCAFecNacConyuge') actualizaArrayCA('10', '', 'FechaNacimiento', $(this).val());
        if (idEtiqueta == 'txtCAFecNacPadre') actualizaArrayCA('40', 'M', 'FechaNacimiento', $(this).val());
        if (idEtiqueta == 'txtCAFecNacMadre') actualizaArrayCA('40', 'F', 'FechaNacimiento', $(this).val());
    });

    $('#TabClausulaAdicional select').live('change', function () {
        var idEtiqueta = this.id;
        if (idEtiqueta == 'ddlCASexoConyuge') actualizaArrayCA('10', '', 'Sexo', $(this).val());
    });

    function actualizaArrayCA(parentesco, sexo, clave, valor) {
        var array = Solicitud.CoberturasAdicionales;
        var existe = false;
        for (i in array) {
            if (parentesco == '10' && array[i]['Parentesco'] == parentesco) {
                array[i][clave] = valor;
                existe = true;
            }
            else if (array[i]['Parentesco'] == parentesco && array[i]['Sexo'] == sexo) {
                array[i][clave] = valor;
                existe = true;
            }
        }

        if (!existe) {
            var ca = {
                FechaNacimiento: ''
                , Parentesco: parentesco
                , Sexo: sexo
            }
            ca[clave] = valor;
            array.push(ca)
        }
    }

    //<INI.GTI_7012>
    function pad(str, max) {
        str = str.toString();
        return str.length < max ? pad("0" + str, max) : str;
    }
    //<FIN.GTI_7012>

    /* Modificar el Ajuste TRA en la Tabla de Cotizaciones */
    $('#TabCotizaciones_IFP_P1 input[type=text]').live('keyup', function () {
        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        //var fil = fila.parent('tbody').children().index(fila);
        var fil = fila.find('.ModSolItem').val();
        Solicitud.Cotizaciones[fil].AjusteTRA = $(this).val();
    });

    $('#TabCotizaciones_IFP_P2 input[type=text]').live('keyup', function () {
        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        //var fil = fila.parent('tbody').children().index(fila);
        var fil = fila.find('.ModSolItem').val();
        Solicitud.Cotizaciones[fil].AjusteTRA = $(this).val();
    });

    $('#TabCotizaciones_IFP_P3 input[type=text]').live('keyup', function () {
        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        //var fil = fila.parent('tbody').children().index(fila);
        var fil = fila.find('.ModSolItem').val();
        Solicitud.Cotizaciones[fil].AjusteTRA = $(this).val();
    });

    /* Botón Eliminar */
    $('#TabCotizaciones_IFP_P1 .grilla_eliminar, #TabCotizaciones_IFP_P2 .grilla_eliminar, #TabCotizaciones_IFP_P3 .grilla_eliminar').live('click', function () {
        idCotizacion = $(this).data('cotizacion');
        $('#MCATablaEliminar').val('cot');
        $('#MCAIcono').attr('class', 'advertencia');
        $('#MCAContenedor').html('¿Confirma la eliminación de la cotización <strong>N° ' + idCotizacion + '</strong>?');

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        idCotizacion = fila.find('.ModSolItem').val();

        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
        $('#ModalCuadroAdvertencia').dialog('open');
        return false;
    });

    /* Seleccionar beneficiario en tabla de beneficiarios */
    $('#TabBeneficiarios_RP input[type=checkbox]').live('change', function () {
        $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.find('.ModSolBenId').val();
        const beneficiarioId = fila.find('input[name="Correlativo"]').val();

        var seleccion = $(fila).find(':checkbox').prop('checked');

        if (seleccion) {
            fila.find('.Porcentaje').attr('readonly', false);
        } else {
            fila.find('.Porcentaje').val(0);
            fila.find('.Porcentaje').attr('readonly', true);
            
            const { sumaPorcentaje, porcentajesBeneficiarios } = ObtenerPorcentajesBeneficiarios();

            $('.Porcentaje').autoNumeric({ vMin: '0', vMax: '100' });
            $('#ModGruSumaBenef_IFP').val(sumaPorcentaje);
            $('#ModGruSumaBenef_IFP').removeClass('formTextboxError');
            if (parseInt($('#ModGruSumaBenef_IFP').val()) > 100) {
                $('#ModGruSumaBenef_IFP').addClass('formTextboxError');
            }

            // $.ajax({
            //     type: 'POST',
            //     url: 'MantenerSolicitud.aspx/ModificarPorcentaje',
            //     contentType: "application/json; charset=iso-8859-1",
            //     dataType: 'json',
            //     data: $.toJSON(params),
            //     success: function (data) {
            //         if (data.d.Estado == 'OK') {
            //             $('.Porcentaje').autoNumeric({ vMin: '0', vMax: '100' });
            //             $('#ModGruSumaBenef_IFP').val(data.d.Mensaje);

            //             $('#ModGruSumaBenef_IFP').removeClass('formTextboxError');
            //             if (parseInt($('#ModGruSumaBenef_IFP').val()) > 100) {
            //                 $('#ModGruSumaBenef_IFP').addClass('formTextboxError');
            //             }
            //         }
            //         else if (data.d.Estado == "ERROR") {
            //             $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
            //             $('#MCMIcono').attr('class', data.d.Icono);
            //             $('#MCMContenedor').html(data.d.Mensaje);
            //             $('#ModalCuadroMensaje').dialog('open');
            //         }
            //         else if (data.d.Estado == 'TOKEN') {
            //             CerrarSesionExpirada();
            //         }
            //     },
            //     error: function (XMLHttpRequest, textStatus, errorThrown) {
            //         if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
            //             /* Sesión caducada */
            //             document.location.reload(true);
            //         }
            //     }
            // });
        }
    });

    function ObtenerPorcentajesBeneficiarios() {
        let total = 0;
        const beneficiariosLS = Utilitarios.getSessionStorageItem("beneficiarios_rp") ? JSON.parse(Utilitarios.getSessionStorageItem("beneficiarios_rp")) : [];
        const filasTabla = document.querySelectorAll("#TabBeneficiarios_RP tbody tr");
        const porcentajesBeneficiarios = [];
        
        for(const fila of filasTabla) {
            const idBeneficiarioGrupo = fila.querySelector("input[type='checkbox']").value;
            const porcentaje = fila.querySelector("#ItemPorcentajeRenta").value;
            const codParentesco = beneficiariosLS.find(b => b.Id == idBeneficiarioGrupo || b.IdGrupoFamiliar == idBeneficiarioGrupo).Parentesco.Id;

            if(codParentesco != EnumParentesco.Afiliado) {
                total += Number(porcentaje || 0);
            }

            porcentajesBeneficiarios.push({
                id: Number(idBeneficiarioGrupo),
                porcentaje: Number(porcentaje || 0),
                parentesco: String(codParentesco)
            });
        }

        return {
            sumaPorcentaje: total,
            porcentajesBeneficiarios
        }
    }

    function ModificarPorcentajeBeneficiario(beneficiarios, idBeneficiarioGrupo, porcentajeRenta) {
        const idxModificar = Solicitud.Beneficiarios.findIndex(b => b.Id == idBeneficiarioGrupo || b.IdGrupoFamiliar == idBeneficiarioGrupo);

        if(idxModificar > -1) {
            beneficiarios[idxModificar].ValPjeRenta = porcentajeRenta;
        } else {
            const beneficiariosLS = Utilitarios.getSessionStorageItem("beneficiarios_rp") ? JSON.parse(Utilitarios.getSessionStorageItem("beneficiarios_rp")) : [];
            const idxModificarLS = beneficiariosLS.findIndex(b => b.Id == idBeneficiarioGrupo || b.IdGrupoFamiliar == idBeneficiarioGrupo);

            if(idxModificarLS > -1) {
                beneficiariosLS[idxModificarLS].ValPjeRenta = porcentajeRenta;
            }

            GuardarBeneficiariosSesionPlan3IFP(beneficiariosLS);
        }

        const suma = beneficiarios.reduce((acc, curr) => {
            if (curr.Parentesco.Id != EnumParentesco.Afiliado) {
                acc += Number(curr.ValPjeRenta || 0);
            }
            return Number(acc || 0);
        }, 0);

        return {
            beneficiarios,
            suma,
        }
    }

    function GuardarBeneficiariosSesionPlan3IFP(beneficiarios) {
        console.log("Guardando beneficiarios: ", beneficiarios);
        Utilitarios.setSessionStorageItem(
            "beneficiarios_rp", 
            JSON.stringify(beneficiarios.filter(b => b.Parentesco.Id != EnumParentesco.Otros))
        );
    }

    /* Seleccionar cotización */

    $('#TabCotizaciones_IFP_P1 input[type=radio]').live('change', function () {

        $("#TabCotizaciones_IFP_P2 tbody tr").removeClass("grilla_active");
        $("#TabCotizaciones_IFP_P2 tbody tr:even").addClass("grilla_alt1");
        $("#TabCotizaciones_IFP_P2 tbody tr:odd").addClass("grilla_alt2");
        $("#TabCotizaciones_IFP_P1 tbody tr").removeClass("grilla_active");
        $("#TabCotizaciones_IFP_P1 tbody tr:even").addClass("grilla_alt1");
        $("#TabCotizaciones_IFP_P1 tbody tr:odd").addClass("grilla_alt2");

        var radio = $(this);
        var filaPadre = radio.parent().parent();

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        var clasePadre = filaPadre.attr('class');
        setTimeout(function () {
            $("#TabCotizaciones_IFP_P1 tbody tr").removeClass("grilla_active");
            $("#TabCotizaciones_IFP_P1 tbody tr:even").addClass("grilla_alt1");
            $("#TabCotizaciones_IFP_P1 tbody tr:odd").addClass("grilla_alt2");

            filaPadre.toggleClass("grilla_active", radio.is(":checked"));
        }, 0);

        //$('#ModSolMontoCIA').val(formatearMonto(Solicitud.Cotizaciones[fil].MontoCia.toFixed(2)));
        //$('#ModSolPensionCIA').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionCia.toFixed(2)));
        //$('#ModSolPensionCIAMO').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionCiaMO.toFixed(2)));
        //$('#ModSolTasaAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaAFP.toFixed(2)));
        //$('#ModSolMontoAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].MontoAFP.toFixed(2)));
        //$('#ModSolPensionAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionAFP.toFixed(2)));
        ///*<SRI.INI-20322>*/
        //$('#ModTasaVenta').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaVenta.toFixed(2)));
        //$('#ModTasaVentaSbs').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaVentaSbs.toFixed(2)));
        ///*<SRI.FIN-20322>*/
    });

    $('#TabCotizaciones_IFP_P2 input[type=radio]').live('change', function () {

        $("#TabCotizaciones_IFP_P1 tbody tr").removeClass("grilla_active");
        $("#TabCotizaciones_IFP_P1 tbody tr:even").addClass("grilla_alt1");
        $("#TabCotizaciones_IFP_P1 tbody tr:odd").addClass("grilla_alt2");
        $("#TabCotizaciones_IFP_P2 tbody tr").removeClass("grilla_active");
        $("#TabCotizaciones_IFP_P2 tbody tr:even").addClass("grilla_alt1");
        $("#TabCotizaciones_IFP_P2 tbody tr:odd").addClass("grilla_alt2");

        var radio = $(this);
        var filaPadre = radio.parent().parent();

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        var clasePadre = filaPadre.attr('class');
        setTimeout(function () {
            $("#TabCotizaciones_IFP_P2 tbody tr").removeClass("grilla_active");
            $("#TabCotizaciones_IFP_P2 tbody tr:even").addClass("grilla_alt1");
            $("#TabCotizaciones_IFP_P2 tbody tr:odd").addClass("grilla_alt2");

            filaPadre.toggleClass("grilla_active", radio.is(":checked"));
        }, 0);

    });

    /* Botón Aceptar */
    $("#ModSolAceptar_RP").live("click", function () {

        var disabled = this.attributes.class.value;
        if (disabled.indexOf("botonDeshabilitado") >= 0) {
            return false;
        }

        if (!botonModSolAceptarBloqueado) {

            //Validar % CA > 100
            $('#TabClausulaAdicional input').removeClass('formCalendarError');
            $('#TabClausulaAdicional select').removeClass('formTextboxGridError');
            for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                if ((parseInt(Solicitud.Cotizaciones[i].ValPjeCACy) + parseInt(Solicitud.Cotizaciones[i].ValPjeCAPa) + parseInt(Solicitud.Cotizaciones[i].ValPjeCAMa)) > 100) {
                    mensajeValidacion('La sumatoria de los porcentajes de las coberturas vitalicia: padre, madre y/o cónyuge no puede exceder el 100%, fila:' + i);
                    return;
                }

                //Validar datos adicionales
                if (Solicitud.Cotizaciones[i].ValPjeCACy != 0) {
                    //if (Solicitud.CoberturasAdicionales[0].FechaNacimiento == '') {
                    if ($('#txtCAFecNacConyuge').val() == '') {
                        $('#txtCAFecNacConyuge').addClass('formCalendarError');
                        mensajeValidacion('Debe indicar la fecha de nacimiento del cónyuge');
                        return;
                    }

                    //if (Solicitud.CoberturasAdicionales[0].Sexo == 0) {
                    if ($('#ddlCASexoConyuge').val() == '0') {
                        $('#ddlCASexoConyuge').addClass('formTextboxGridError');
                        mensajeValidacion('Debe indicar el sexo del cónyuge');
                        return;
                    }
                }

                if (Solicitud.Cotizaciones[i].ValPjeCAPa != 0) {
                    //if (Solicitud.CoberturasAdicionales[1].FechaNacimiento == '') {
                    if ($('#txtCAFecNacPadre').val() == '') {
                        $('#txtCAFecNacPadre').addClass('formCalendarError');
                        mensajeValidacion('Debe indicar la fecha de nacimiento del padre');
                        return;
                    }
                }

                if (Solicitud.Cotizaciones[i].ValPjeCAMa != 0) {
                    //if (Solicitud.CoberturasAdicionales[2].FechaNacimiento == '') {
                    if ($('#txtCAFecNacMadre').val() == '') {
                        $('#txtCAFecNacMadre').addClass('formCalendarError');
                        mensajeValidacion('Debe indicar la fecha de nacimiento de la madre');
                        return;
                    }
                }
            }

            let modSoloModo = $("#ModSolModo").val();
            if (Utilitarios.getStorageItem("modSolModo")) {
                modSoloModo = Utilitarios.getStorageItem("modSolModo");
            }

            console.log("ModSoloModo", modSoloModo);

            if (modSoloModo == "N" || modSoloModo == "C") { // Nuevo o Copia
                var idBeneficiarios = new Array();

                // Obtener la lista de beneficiarios
                for (i = 0; i < $("#TabBeneficiarios_RP tbody tr").length; i++) {
                    if ($("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").is(":checked"))
                        idBeneficiarios.push(i);
                }

                //idBeneficiarios.push(0);

                $("#ModSolCargando").fadeIn();
                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Cotizando la solicitud, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Cotizando" });
                $("#ModalCotizando").dialog("open");

                try {
                    const params = {
                        tokenUsuario: $("#TokenUsuario").val(),
                        cuspp: $("#HCUSPP_RP").val(),
                        //afp: $("#HAFP_RP").val(),
                        monedaPrimaUnica: $("#ModSolMonedaPrimaUnica_IFP").val(),
                        primaUnica: $("#ModSolPrimaUnica_IFP").val(),
                        fechaCotizacion: $("#ModSolFechaCotizacion_IFP").val(),
                        fechaDevengue: $("#ModSolFechaDevengue_IFP").val(),
                        cotizaciones: Solicitud.Cotizaciones,
                        idBeneficiarios: idBeneficiarios,
                        coberturasAdicionales: Solicitud.CoberturasAdicionales
                    }

                    console.log("Solicitud", params);

                    const session = Utilitarios.ObtenerSession();
                    const afiliadoIfp = Utilitarios.getStorageItem("afiliadoIfp") ? JSON.parse(Utilitarios.getStorageItem("afiliadoIfp")) : null;
                    const datosAgente = {
                        "vendedorId": afiliadoIfp.Agente.Id,
                        "carteraId": afiliadoIfp.Agente.IdCartera
                    };
                    const datosUsuario = {
                        "codigoUsuario": session.Matricula,
                        "codigoRol": session.RolAzman
                    }
                    const { porcentajesBeneficiarios } = ObtenerPorcentajesBeneficiarios();

                    ApiCotizadorIFP.RegistrarSolicitud({
                        solicitud: params,
                        datosAgente,
                        datosUsuario,
                        porcentajesBeneficiarios
                    })
                        .then(async (response) => {
                            const data = response;
                            $('#ModSolNroSolicitud_IFP').val(data.Id);

                            // Cambiar la modal a modo de modificación
                            $('#ModSolModo').val('M');

                            // Cerrar modal de espera
                            $('#ModalCotizando').dialog('close');

                            // Mostrar mensaje de éxito
                            $('#MCMIcono').attr('class', "exito");
                            $('#MCMContenedor').html("Solicitud cotizada correctamente.");
                            $('#ModalCuadroMensaje').dialog({ title: "Mensaje" });
                            $('#ModalCuadroMensaje').dialog('open');

                            $('#ModSolCargando').show();

                            // Actualizar la URL para que se muestre la solicitud como modificada
                            const uriParams = new URLSearchParams();
                            uriParams.set("modSolModo", "M");
                            uriParams.set("idSolicitud", data.Id);
                            uriParams.set("fecCotizacion", data.FechaCotizacion.substring(0, 10));

                            Utilitarios.setStorageItem("modSolModo", "M");
                            Utilitarios.setStorageItem("idSolicitud", data.Id);
                            Utilitarios.setStorageItem("fecCotizacion", data.FechaCotizacion.substring(0, 10));

                            const nuevaUrl = window.location.pathname + "?" + uriParams.toString();
                            window.history.pushState({}, '', nuevaUrl);

                            LimpiarFormularioSolicitud();

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

                            // Obtener los datos de la solicitud
                            const responseSolicitud = await ApiCotizadorIFP.ObtenerSolicitudIFP(
                                data.Id
                            );

                            console.log("response", responseSolicitud);

                            Solicitud = responseSolicitud;
                            $('#ModSolNroSolicitud_IFP').val(Solicitud.Id);
                            $('#ModSolFechaDevengue_IFP').val(new Date(Solicitud.FechaDevengue.replace("Z", "")).toString("dd/MM/yyyy"));
                            $('#ModSolPrimaUnica_IFP').val(formatearMonto(Solicitud.PrimaUnica));
                            $('#ModSolFechaCotizacion_IFP').val(new Date(Solicitud.FechaCotizacion.replace("Z", "")).toString("dd/MM/yyyy"));

                            $('#ModSolTipoCambio_IFP').val(formatearMonto(Solicitud.TipoCambio));

                            $('#HAgenteId').val(Solicitud.Agente.Id);

                            if ($("#ModSolMonedaPrimaUnica_IFP").val() == "001") {
                                $("#ModSolTipoCambioPanel").hide();
                            } else {
                                $("#ModSolTipoCambioPanel").show();
                            };

                            ////CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);

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

                            if (bPlan1) {
                                CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
                            }

                            if (bPlan2) {
                                CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
                            }

                            if (bPlan3) {
                                CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
                            }

                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                            $clock.countdown(selectedDate.toString());

                            $('#ModSolCargando').fadeOut();
                            $('#ModSolSaldoCIC_RP').focus();
                            $('#ModSolACOM').focus();
                            $('#ModSolTipoCambio_IFP').focus();

                            $('#ModSolImprimir_RP').attr('class', 'boton darkblue sharp');
                            /* $.ajax({
                                type: 'POST',
                                url: 'Cotizador.aspx/ObtenerDatosSolicitud',
                                contentType: "application/json; charset=iso-8859-1",
                                data: $.toJSON(params3),
                                dataType: 'json',
                                success: function (data) {
                                    Solicitud = data.d;
                                    $('#ModSolNroSolicitud_IFP').val(Solicitud.Id);
                                    $('#ModSolFechaDevengue_IFP').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    $('#ModSolPrimaUnica_IFP').val(formatearMonto(Solicitud.PrimaUnica));
                                    $('#ModSolFechaCotizacion_IFP').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));

                                    $('#ModSolTipoCambio_IFP').val(formatearMonto(Solicitud.TipoCambio));

                                    $('#HAgenteId').val(Solicitud.Agente.Id);

                                    if ($("#ModSolMonedaPrimaUnica_IFP").val() == "001") {
                                        $("#ModSolTipoCambioPanel").hide();
                                    } else {
                                        $("#ModSolTipoCambioPanel").show();
                                    };

                                    ////CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);

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

                                    if (bPlan1) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
                                    }

                                    if (bPlan2) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
                                    }

                                    if (bPlan3) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
                                    }

                                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                    $clock.countdown(selectedDate.toString());

                                    $('#ModSolCargando').fadeOut();
                                    $('#ModSolSaldoCIC_RP').focus();
                                    $('#ModSolACOM').focus();
                                    $('#ModSolTipoCambio_IFP').focus();

                                    $('#ModSolImprimir_RP').attr('class', 'boton darkblue sharp');

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
                                    $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                                }
                            });
                        })
                         */
                        }).catch(error => {
                            if (error.body && error.body.error) {
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html(`${error.body.error.message}`);
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            } else {
                                console.log("Error", error);
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            }
                            $('#ModalCuadroMensaje').dialog('open');
                            $('#ModalCotizando').dialog('close');
                        });
                } catch (error) {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.<br/>' + error.message);
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                    $('#ModalCotizando').dialog('close');
                }

                /*
                $.ajax({
                    type: 'POST',
                    url: 'Cotizador.aspx/InsertarSolicitud',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Respuesta.Estado == 'OK') {
                            // Imprimir número de solicitud generada
                            $('#ModSolNroSolicitud_IFP').val(data.d.Id);

                            // Cambiar la modal a modo de modificación
                            $('#ModSolModo').val('M');

                            // Cerrar modal de espera
                            $('#ModalCotizando').dialog('close');

                            // Mostrar mensaje de éxito
                            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                            $('#ModalCuadroMensaje').dialog('open');

                            $('#ModSolCargando').show();

                            var params3 = {
                                idSolicitud: $('#ModSolNroSolicitud_IFP').val(),
                                fecCotizacion: $('#ModSolFechaCotizacion_IFP').val()
                            }

                            LimpiarFormularioSolicitud();

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

                            $.ajax({
                                type: 'POST',
                                url: 'Cotizador.aspx/ObtenerDatosSolicitud',
                                contentType: "application/json; charset=iso-8859-1",
                                data: $.toJSON(params3),
                                dataType: 'json',
                                success: function (data) {
                                    Solicitud = data.d;
                                    $('#ModSolNroSolicitud_IFP').val(Solicitud.Id);
                                    $('#ModSolFechaDevengue_IFP').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    $('#ModSolPrimaUnica_IFP').val(formatearMonto(Solicitud.PrimaUnica));
                                    $('#ModSolFechaCotizacion_IFP').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));

                                    $('#ModSolTipoCambio_IFP').val(formatearMonto(Solicitud.TipoCambio));

                                    $('#HAgenteId').val(Solicitud.Agente.Id);

                                    if ($("#ModSolMonedaPrimaUnica_IFP").val() == "001") {
                                        $("#ModSolTipoCambioPanel").hide();
                                    } else {
                                        $("#ModSolTipoCambioPanel").show();
                                    };

                                    ////CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);

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

                                    if (bPlan1) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
                                    }

                                    if (bPlan2) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
                                    }

                                    if (bPlan3) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
                                    }

                                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                    $clock.countdown(selectedDate.toString());

                                    $('#ModSolCargando').fadeOut();
                                    $('#ModSolSaldoCIC_RP').focus();
                                    $('#ModSolACOM').focus();
                                    $('#ModSolTipoCambio_IFP').focus();

                                    $('#ModSolImprimir_RP').attr('class', 'boton darkblue sharp');

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
                                    $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                                }
                            });
                        }
                        else if (data.d.Estado == 'TOKEN') {
                            CerrarSesionExpirada();
                        }
                        else {
                            errorCotizacion(data.d.Respuesta);
                            //$('#ModalCotizando').dialog('close');
                            //$('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                            //$('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                            //$('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                            //if (data.d.Respuesta.Controles != null) {
                            //    if (data.d.Respuesta.Controles[0].length) $('#ModSolPrimaUnica_IFP').attr('class', data.d.Respuesta.Controles[0]);
                            //    if (data.d.Respuesta.Controles[1].length) $('#ModSolFechaCotizacion_IFP').attr('class', data.d.Respuesta.Controles[1]);
                            //    if (data.d.Respuesta.Controles[2].length) $('#ModSolFechaDevengue_IFP').attr('class', data.d.Respuesta.Controles[2]);
                            //    if (data.d.Respuesta.Controles[3].length) $('#ModSolMonedaPrimaUnica_IFP').attr('class', data.d.Respuesta.Controles[3]);

                            //    $('#TabCotizaciones_IFP_P1 select').removeClass('formTextboxGridError');
                            //    $('#TabCotizaciones_IFP_P2 select').removeClass('formTextboxGridError');
                            //    $('#TabCotizaciones_IFP_P3 select').removeClass('formTextboxGridError');
                            //    $('#TabClausulaAdicional select').removeClass('formTextboxGridError');
                            //    $('#TabClausulaAdicional input').removeClass('formCalendarError');

                            //    if (data.d.Respuesta.Controles.length > 4) {
                            //        var celdaError;
                            //        for (i = 4; i < data.d.Respuesta.Controles.length; i++) {
                            //            celdaError = data.d.Respuesta.Controles[i].split(',');

                            //            if (celdaError[2] == 'PLAN1') {
                            //                $('#TabCotizaciones_IFP_P1 tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                            //            } else if (celdaError[2] == 'PLAN2') {
                            //                $('#TabCotizaciones_IFP_P2 tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                            //            } else if (celdaError[2] == 'PLAN3') {
                            //                $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                            //            } else if (celdaError[2] == 'PLAN3_CA') {
                            //                var etiqueta = $('#TabClausulaAdicional tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') ').children();
                            //                if (etiqueta[0].tagName == 'INPUT') {
                            //                    etiqueta.addClass('formCalendarError')
                            //                } else if (etiqueta[0].tagName == 'SELECT') {
                            //                    etiqueta.addClass('formTextboxGridError')
                            //                }
                            //            }
                            //        }
                            //    }
                            //}
                            //$('#ModalCuadroMensaje').dialog('open');
                            //$('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                        }

                        $('#ModSolCargando').fadeOut();
                        //ModSolBotonesInactivos = false;
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
                });*/
            }
            else if (modSoloModo == 'M') { // Modificar
                var idBeneficiarios = new Array();

                // Obtener la lista de beneficiarios
                for (i = 0; i < $('#TabBeneficiarios_RP tbody tr').length; i++) {
                    if ($('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').is(':checked'))
                        idBeneficiarios.push(i);
                }

                //idBeneficiarios.push(0);

                //$('#ModSolCargando').fadeIn();
                $('#MCIcono').attr('class', 'cargando');
                $('#MCContenedor').html('Cotizando la solicitud, por favor espere un momento...');
                $('#ModalCotizando').dialog({ title: 'Cotizando' });
                $('#ModalCotizando').dialog('open');

                try {

                    const params = {
                        tokenUsuario: $("#TokenUsuario").val(),
                        id: $('#ModSolNroSolicitud_IFP').val(),
                        cuspp: $("#HCUSPP_RP").val(),
                        //afp: $("#HAFP_RP").val(),
                        monedaPrimaUnica: $("#ModSolMonedaPrimaUnica_IFP").val(),
                        primaUnica: $("#ModSolPrimaUnica_IFP").val(),
                        fechaCotizacion: $("#ModSolFechaCotizacion_IFP").val(),
                        fechaDevengue: $("#ModSolFechaDevengue_IFP").val(),
                        cotizaciones: Solicitud.Cotizaciones,
                        idBeneficiarios: idBeneficiarios,
                        coberturasAdicionales: Solicitud.CoberturasAdicionales
                    }

                    const session = Utilitarios.ObtenerSession();
                    const afiliadoIfp = Utilitarios.getStorageItem("afiliadoIfp") ? JSON.parse(Utilitarios.getStorageItem("afiliadoIfp")) : null;
                    const datosAgente = {
                        "vendedorId": afiliadoIfp.Agente.Id,
                        "carteraId": afiliadoIfp.Agente.IdCartera
                    };
                    const datosUsuario = {
                        "codigoUsuario": session.Matricula,
                        "codigoRol": session.RolAzman
                    }
                    const { porcentajesBeneficiarios } = ObtenerPorcentajesBeneficiarios();

                    ApiCotizadorIFP.ModificarSolicitud({
                        solicitud: params,
                        datosAgente,
                        datosUsuario,
                        porcentajesBeneficiarios
                    }, params.id)
                        .then(response => {
                            const data = response;
                            $('#ModalCotizando').dialog('close');

                            // Mostrar mensaje de éxito
                            $('#MCMIcono').attr('class', "exito");
                            $('#MCMContenedor').html("Solicitud cotizada correctamente.");
                            $('#ModalCuadroMensaje').dialog({ title: "Mensaje" });
                            $('#ModalCuadroMensaje').dialog('open');

                            //CargarTablaSolicitudes();

                            // Cargar la cotización actualizada
                            //$('#BeneficiariosOriginales_RP').show();
                            //<INIGTI_753_3>
                            //$('#ManSolPestanhas li:eq(0)').trigger('click');
                            //<FINGTI_753_3>

                            Utilitarios.setStorageItem("modSolModo", "M");
                            Utilitarios.setStorageItem("idSolicitud", data.Id);
                            Utilitarios.setStorageItem("fecCotizacion", data.FechaCotizacion.substring(0, 10));

                            $('#ModSolCargando').show();

                            const params3 = {
                                idSolicitud: $('#ModSolNroSolicitud_IFP').val(),
                                fecCotizacion: $('#ModSolFechaCotizacion_IFP').val()
                            }

                            LimpiarFormularioSolicitud();

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

                            $.ajax({
                                type: 'POST',
                                url: 'Cotizador.aspx/ObtenerDatosSolicitud',
                                contentType: "application/json; charset=iso-8859-1",
                                data: $.toJSON(params3),
                                dataType: 'json',
                                success: function (data) {
                                    Solicitud = data.d;
                                    $('#ModSolNroSolicitud_IFP').val(Solicitud.Id);
                                    $('#ModSolFechaDevengue_IFP').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    $('#ModSolPrimaUnica_IFP').val(formatearMonto(Solicitud.PrimaUnica));
                                    $('#ModSolFechaCotizacion_IFP').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    //$('#ModSolDCOM_RP').val(formatearMonto(Solicitud.PorcentajeDescuentoComision));

                                    //<INIGTI_753_3>
                                    $('#ModSolTipoCambio_IFP').val(formatearMonto(Solicitud.TipoCambio));
                                    if ($("#ModSolMonedaPrimaUnica_IFP").val() == "001") {
                                        $("#ModSolTipoCambioPanel").hide();
                                    } else {
                                        $("#ModSolTipoCambioPanel").show();
                                    };
                                    //<FINGTI_753_3>


                                    //CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);
                                    //CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_IFP").val(), $("#HConyuge").val());//<INIGTI_753>

                                    ////<INIGTI_753_3>
                                    //if ($("#HBloqueo").val() == "TRUE") {
                                    //    botonModSolAceptarBloqueado = true;
                                    //    $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                                    //}
                                    ////<FINGTI_753_3>

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

                                    if (bPlan1) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
                                    }

                                    if (bPlan2) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
                                    }

                                    if (bPlan3) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
                                    }

                                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                    $clock.countdown(selectedDate.toString());

                                    $("#ModSolCargando").fadeOut();
                                    $("#ModSolSaldoCIC_RP").focus();
                                    $("#ModSolACOM").focus();
                                    //$("#ModSolDCOM_RP").focus();
                                    $("#ModSolTipoCambio_IFP").focus();

                                    $('#ModSolImprimir_RP').attr('class', 'boton darkblue sharp');
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
                                }
                            });
                        })
                        .catch(error => {
                            if (error.body && error.body.error) {
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html(`${error.body.error.message}`);
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            } else {
                                console.log("Error", error);
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            }
                            $('#ModalCuadroMensaje').dialog('open');
                            $('#ModalCotizando').dialog('close');
                        });
                } catch (error) {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.<br/>' + error.message);
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                    $('#ModalCotizando').dialog('close');
                }

                /* $.ajax({
                    type: 'POST',
                    url: 'Cotizador.aspx/ModificarSolicitud',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Respuesta.Estado == 'OK') {
                            $('#ModalCotizando').dialog('close');

                            // Mostrar mensaje de éxito
                            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                            $('#ModalCuadroMensaje').dialog('open');

                            //CargarTablaSolicitudes();

                            // Cargar la cotización actualizada
                            //$('#BeneficiariosOriginales_RP').show();
                            //<INIGTI_753_3>
                            //$('#ManSolPestanhas li:eq(0)').trigger('click');
                            //<FINGTI_753_3>

                            $('#ModSolCargando').show();

                            var params3 = {
                                idSolicitud: $('#ModSolNroSolicitud_IFP').val(),
                                fecCotizacion: $('#ModSolFechaCotizacion_IFP').val()
                            }

                            LimpiarFormularioSolicitud();

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



                            $.ajax({
                                type: 'POST',
                                url: 'Cotizador.aspx/ObtenerDatosSolicitud',
                                contentType: "application/json; charset=iso-8859-1",
                                data: $.toJSON(params3),
                                dataType: 'json',
                                success: function (data) {
                                    Solicitud = data.d;
                                    $('#ModSolNroSolicitud_IFP').val(Solicitud.Id);
                                    $('#ModSolFechaDevengue_IFP').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    $('#ModSolPrimaUnica_IFP').val(formatearMonto(Solicitud.PrimaUnica));
                                    $('#ModSolFechaCotizacion_IFP').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    //$('#ModSolDCOM_RP').val(formatearMonto(Solicitud.PorcentajeDescuentoComision));

                                    //<INIGTI_753_3>
                                    $('#ModSolTipoCambio_IFP').val(formatearMonto(Solicitud.TipoCambio));
                                    if ($("#ModSolMonedaPrimaUnica_IFP").val() == "001") {
                                        $("#ModSolTipoCambioPanel").hide();
                                    } else {
                                        $("#ModSolTipoCambioPanel").show();
                                    };
                                    //<FINGTI_753_3>


                                    //CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);
                                    //CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_IFP").val(), $("#HConyuge").val());//<INIGTI_753>

                                    ////<INIGTI_753_3>
                                    //if ($("#HBloqueo").val() == "TRUE") {
                                    //    botonModSolAceptarBloqueado = true;
                                    //    $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                                    //}
                                    ////<FINGTI_753_3>

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

                                    if (bPlan1) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
                                    }

                                    if (bPlan2) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
                                    }

                                    if (bPlan3) {
                                        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
                                    }

                                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                    $clock.countdown(selectedDate.toString());

                                    $("#ModSolCargando").fadeOut();
                                    $("#ModSolSaldoCIC_RP").focus();
                                    $("#ModSolACOM").focus();
                                    //$("#ModSolDCOM_RP").focus();
                                    $("#ModSolTipoCambio_IFP").focus();

                                    $('#ModSolImprimir_RP').attr('class', 'boton darkblue sharp');
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
                                }
                            });
                        }
                        else if (data.d.Estado == 'TOKEN') {
                            CerrarSesionExpirada();
                        }
                        else {
                            errorCotizacion(data.d.Respuesta);
                            //$('#ModalCotizando').dialog('close');
                            //$('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                            //$('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                            //$('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                            //if (data.d.Respuesta.Controles != null) {
                            //    if (data.d.Respuesta.Controles[0].length) $('#ModSolPrimaUnica_IFP').attr('class', data.d.Respuesta.Controles[0]);
                            //    if (data.d.Respuesta.Controles[1].length) $('#ModSolFechaCotizacion_IFP').attr('class', data.d.Respuesta.Controles[1]);
                            //    if (data.d.Respuesta.Controles[2].length) $('#ModSolFechaDevengue_IFP').attr('class', data.d.Respuesta.Controles[2]);
                            //    if (data.d.Respuesta.Controles[3].length) $('#ModSolMonedaPrimaUnica_IFP').attr('class', data.d.Respuesta.Controles[3]);

                            //    $('#TabCotizaciones_IFP_P1 select').removeClass('formTextboxGridError');
                            //    $('#TabCotizaciones_IFP_P2 select').removeClass('formTextboxGridError');
                            //    $('#TabCotizaciones_IFP_P3 select').removeClass('formTextboxGridError');

                            //    if (data.d.Respuesta.Controles.length > 4) {
                            //        var celdaError;
                            //        for (i = 4; i < data.d.Respuesta.Controles.length; i++) {
                            //            celdaError = data.d.Respuesta.Controles[i].split(',');

                            //            if (celdaError[2] == 'PLAN1') {
                            //                $('#TabCotizaciones_IFP_P1 tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                            //            } else if (celdaError[2] == 'PLAN2') {
                            //                $('#TabCotizaciones_IFP_P2 tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                            //            } else if (celdaError[2] == 'PLAN3') {
                            //                $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                            //            }

                            //        }
                            //    }

                            //}
                            //$('#ModalCuadroMensaje').dialog('open');
                        }

                        $('#ModSolCargando').fadeOut();
                        //ModSolBotonesInactivos = false;
                        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
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
                }); */
            }
        }
        else {
            return false;
        }
    });

    /* Botón Cancelar */
    $('#ModSolCancelar_RP').live('click', function () {
        window.location.href = "Cotizador.aspx#datos_solicitud";
        //$('#ModalSolicitud').dialog('close');
    });

    /* Botón Imprimir */
    $('#ModSolImprimir_RP').live('click', function () {

        var disabled = this.attributes.class.value;
        if (disabled.indexOf("botonDeshabilitado") >= 0) {
            return false;
        }

        var idSolicitud = $('#ModSolNroSolicitud_IFP').val();
        var fecCotizacion = $('#ModSolFechaCotizacion_IFP').val();
        var numAgente = $('#HAgenteId').val();
        var cuspp = $("#CUSPP_RP").val();

        var params = {
            idSolicitud: idSolicitud,
            fecCotizacion: fecCotizacion,
            numAgente: numAgente
        }

        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/ExportarSolicitudPDF',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {

                    /*if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                        window.location.href = "../Reportes/DetallePropuestaIFPMovil.aspx";
                    }
                    else {
                        var w = 800;
                        var h = 600;
                        var left = (screen.width / 2) - (w / 2);
                        var top = (screen.height / 2) - (h / 2);

                        var nuevaVentana = window.open("../Reportes/DetallePropuestaIFP.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                    }*/

                    window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + idSolicitud + "&formato=8");

                    if (data.d.Contenido == "1") {
                        window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + idSolicitud + "&formato=7");
                    }

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());

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
                    $('#MCMContenedor').html('Ha ocurrido un error al exportar la la solicitud.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });

    /* Botón + */
    $("#ModSolAgregarPlan_IFP").live("click", async function () {

        var valPrimaUnica_IFP = false;
        var valMonedaPrimaUnica_IFP = false;
        var valPlan_IFP = false;
        var valPlan;

        if ($(this).hasClass("botonDeshabilitado")) {
            return;
        }

        if ($.trim($('#ModSolPrimaUnica_IFP').val()).length > 0) {
            if ($('#ModSolPrimaUnica_IFP').val() != 0.00) {
                valPrimaUnica_IFP = true;
            }
        }

        if ($('#ModSolMonedaPrimaUnica_IFP').val() != "0") {
            valMonedaPrimaUnica_IFP = true;
        }

        if ($('#ModSolPlan_IFP').val() != "0") {
            valPlan_IFP = true;
            valPlan = $('#ModSolPlan_IFP').val();
        }

        /*VALIDACION DE TOPE DE COTIZACIONES*/
        var vCantSolicitudes = false;
        var cantidadSolicitudes = 0;
        if (valPrimaUnica_IFP & valMonedaPrimaUnica_IFP) {
            var resultadoValidacion = await validaCantSolicitudes($('#ModSolMonedaPrimaUnica_IFP').val(), $('#ModSolPrimaUnica_IFP').val())

            vCantSolicitudes = resultadoValidacion.isValid;
            cantidadSolicitudes = resultadoValidacion.count;

            if (!vCantSolicitudes) {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('No es posible cotizar, ha alcanzado el límite de cotizaciones.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
                $('#ModalCotizando').dialog('close');
                return;
            }
        }

        esCorrecto = valPrimaUnica_IFP & valMonedaPrimaUnica_IFP & valPlan_IFP;

        if (!esCorrecto) {
            return false;
        }

        for (var i_plan = 0; i_plan < Plan.length; i_plan++) {
            if (Plan[i_plan] == $('#ModSolPlan_IFP').val()) {
                return false;
            }
        }

        if ($("#ModSolModo").val() == "N" || $("#ModSolModo").val() == "M" || $("#ModSolModo").val() == "C") {

            var url = "Mantenersolicitud.aspx/RenderizarTabla";

            //if (Solicitud.Cotizaciones[0].Plan.Id != "") {

            var bPlan1 = false;
            var bPlan2 = false;
            var bPlan3 = false;

            for (var i = 0; i < Solicitud.Cotizaciones.length; i++) {
                if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN1') {
                    bPlan1 = true;
                }
                if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN2') {
                    bPlan2 = true;
                }
                if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN3') {
                    bPlan3 = true;
                }
            }

            if (bPlan1 == false || bPlan2 == false || bPlan3 == false) {

                if (valPlan == 'PLAN1') {
                    var cot =
                    {
                        Item: Solicitud.Cotizaciones.length,
                        Moneda: { Id: "000" },
                        PeriodoGarantizado: 0,
                        AjusteTRA: 0,
                        PagoDoble: 0,
                        PjePagoDoble: 0,
                        IndGastoSepelio: "S",
                        ValPjeDev: 50,
                        ValMonAju: 0,
                        ValPjeDCOM: 0,
                        Temporalidad: { Id: "T05", Anhos: 5 },
                        ValPerDiferido: 0,
                        ValPjeDevFallec: 50,
                        PensionCiaMO: 0,
                        Pension2doTramo: 0,
                        TasaVenta: 0,
                        TasaRetornoAccionista: 0,
                        TasaRetornoAccionistaMinima: 0,
                        IndCotiza: "",
                        Plan: { Id: valPlan },
                        //ValPjeCACy: 0,
                        //ValPjeCAPa: 0,
                        //ValPjeCAMa: 0,
                        //ValPjeCATotal: 0
                    };
                } else if (valPlan == 'PLAN2') {
                    var cot =
                    {
                        Item: Solicitud.Cotizaciones.length,
                        Moneda: { Id: "000" },
                        PeriodoGarantizado: 5,
                        AjusteTRA: 0,
                        PagoDoble: 0,
                        PjePagoDoble: 0,
                        IndGastoSepelio: "S",
                        ValPjeDev: 0,
                        ValMonAju: 0,
                        ValPjeDCOM: 0,
                        Temporalidad: { Id: "T05", Anhos: 5 },
                        ValPerDiferido: 0,
                        ValPjeDevFallec: 0,
                        PensionCiaMO: 0,
                        Pension2doTramo: 0,
                        TasaVenta: 0,
                        TasaRetornoAccionista: 0,
                        TasaRetornoAccionistaMinima: 0,
                        IndCotiza: "",
                        Plan: { Id: valPlan },
                        //ValPjeCACy: 0,
                        //ValPjeCAPa: 0,
                        //ValPjeCAMa: 0,
                        //ValPjeCATotal: 0
                    };
                } else if (valPlan == 'PLAN3') {
                    var cot =
                    {
                        Item: Solicitud.Cotizaciones.length,
                        Moneda: { Id: "000" },
                        PeriodoGarantizado: 5,
                        AjusteTRA: 0,
                        PagoDoble: 0,
                        PjePagoDoble: 0,
                        IndGastoSepelio: "S",
                        //ValPjeDev: 0,
                        ValMonAju: 0,
                        ValPjeDCOM: 0,
                        ValPjeConyuge: 0,
                        Temporalidad: { Id: "TVT", Anhos: 0 },
                        ValPerDiferido: 0,
                        //ValPjeDevFallec: 0,
                        PensionCiaMO: 0,
                        Pension2doTramo: 0,
                        TasaVenta: 0,
                        TasaRetornoAccionista: 0,
                        TasaRetornoAccionistaMinima: 0,
                        IndCotiza: "",
                        Plan: { Id: valPlan },
                        //ValPjeCACy: 0,
                        //ValPjeCAPa: 0,
                        //ValPjeCAMa: 0,
                        //ValPjeCATotal: "0%"
                    };

                    Solicitud.CoberturasAdicionales = [
                        //{ Parentesco: '10', FechaNacimiento: '', Sexo: '0' },
                        //{ Parentesco: '40', FechaNacimiento: '', Sexo: 'M' },
                        //{ Parentesco: '40', FechaNacimiento: '', Sexo: 'F' }
                    ];
                };
            }
            Solicitud.Cotizaciones.push(cot);
        }

        //}

        if (Solicitud.Cotizaciones[0].Plan.Id == "") {
            Solicitud.Cotizaciones[0].Plan.Id = valPlan;
        }

        if (valPlan == 'PLAN1') {
            $("#TablaCotizacionesContenedor_IFP_Plan1").hide();
            $("#TablaCotizacionesCargando_RP").show();
        } else if (valPlan == 'PLAN2') {
            $("#TablaCotizacionesContenedor_IFP_Plan2").hide();
            $("#TablaCotizacionesCargando_RP").show();
        } else if (valPlan == 'PLAN3') {
            $("#TablaCotizacionesContenedor_IFP_Plan3").hide();
            $("#TablaCotizacionesCargando_RP").show();
            $('#ManSolPes2').show();
        }

        const permiteTra = Utilitarios.ValidarPermisoActivo(EnumsPermisos.PermisoTRAPlus);
        const permiteEspeciales = Utilitarios.ValidarPermisoActivo(EnumsPermisos.PermisoEspeciales);

        var params = {
            cotizaciones: Solicitud.Cotizaciones,
            moneda: $("#ModSolMonedaPrimaUnica_IFP").val(),
            montoPrimaUnica: $("#ModSolPrimaUnica_IFP").val(),
            plan: valPlan,
            coberturasAdicionales: Solicitud.CoberturasAdicionales,
            cantidad: cantidadSolicitudes,
            permiteTra: permiteTra,
            permiteEspeciales: permiteEspeciales,
            modSolModo: Utilitarios.getStorageItem("modSolModo") || "N"
        }

        $.ajax({
            type: 'POST',
            url: url,
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {

                if (valPlan == 'PLAN1') {
                    $('#TablaCotizacionesContenedor_IFP_Plan1').html($(data.d).find('#ContenidoDinamico').html());
                    $('#TablaCotizacionesCargando_RP').hide();
                    $('#TablaCotizacionesContenedor_IFP_Plan1').show();

                    if (data.d.indexOf("Agregar") >= 0) {
                        $('#ModSolAgregarPlan_IFP').show();
                    } else {
                        $('#TabCotizacionesAgregarPlan2_IFP').hide();
                        $('#TabCotizacionesAgregarPlan3_IFP').hide();
                        $('#ModSolAgregarPlan_IFP').hide();
                    }
                } else if (valPlan == 'PLAN2') {
                    $('#TablaCotizacionesContenedor_IFP_Plan2').html($(data.d).find('#ContenidoDinamico').html());
                    $('#TablaCotizacionesCargando_RP').hide();
                    $('#TablaCotizacionesContenedor_IFP_Plan2').show();

                    if (data.d.indexOf("Agregar") >= 0) {
                        $('#ModSolAgregarPlan_IFP').show();
                    } else {
                        $('#TabCotizacionesAgregarPlan1_IFP').hide();
                        $('#TabCotizacionesAgregarPlan3_IFP').hide();
                        $('#ModSolAgregarPlan_IFP').hide();
                    }
                } else if (valPlan == 'PLAN3') {
                    $('#TablaCotizacionesContenedor_IFP_Plan3').html($(data.d).find('#ContenidoDinamico').html());
                    $('#TablaCotizacionesCargando_RP').hide();
                    $('#TablaCotizacionesContenedor_IFP_Plan3').show();

                    if (data.d.indexOf("Agregar") >= 0) {
                        $('#ModSolAgregarPlan_IFP').show();
                    } else {
                        $('#TabCotizacionesAgregarPlan1_IFP').hide();
                        $('#TabCotizacionesAgregarPlan2_IFP').hide();
                        $('#ModSolAgregarPlan_IFP').hide();
                    }
                }

                $('.numerico').autoNumeric('init');

                //Agregando al arreglo del Plan
                Plan.push($('#ModSolPlan_IFP').val());
                var PlanSeleccionado = $('#ModSolPlan_IFP').val();
                $("#ModSolPlan_IFP option[value='" + PlanSeleccionado + "']").hide();
                $('#ModSolPlan_IFP').val('0');
                $('#ModSolPlan_IFP').change();

                ActivarFecha();

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

        $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');
        $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');

    });

    //<INIGTI_7012>
    /* Botón Aceptar Cierre*/
    $("#ModSolAceptarCierre_RP").live("click", function () {

        if (!botonModSolAceptarBloqueado) {
            if ($("#ModSolModo").val() == "CERRAR") {

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
                } else {
                    for (i = 0; i < $('#TabCotizaciones_IFP_P1 tbody tr').length; i++) {
                        if ($('#TabCotizaciones_IFP_P1 tbody tr:eq(' + i + ') input').is(":checked")) {
                            var renta = $('#TabCotizaciones_IFP_P1 tbody tr:eq(' + i + ') td span.RentaMensual').text();
                            if (renta == 0) {
                                $('#MCMIcono').attr('class', 'validacion');
                                $('#MCMContenedor').html('Seleccione una <strong>Cotización</strong> con una Renta mayor a 0');
                                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                                $('#ModalCuadroMensaje').dialog('open');
                                return;
                            }
                        }
                    }
                    for (i = 0; i < $('#TabCotizaciones_IFP_P2 tbody tr').length; i++) {
                        if ($('#TabCotizaciones_IFP_P2 tbody tr:eq(' + i + ') input').is(":checked")) {
                            var renta = $('#TabCotizaciones_IFP_P2 tbody tr:eq(' + i + ') td span.RentaMensual').text();
                            if (renta == 0) {
                                $('#MCMIcono').attr('class', 'validacion');
                                $('#MCMContenedor').html('Seleccione una <strong>Cotización</strong> con una Renta mayor a 0');
                                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                                $('#ModalCuadroMensaje').dialog('open');
                                return;
                            }
                        }
                    }
                    for (i = 0; i < $('#TabCotizaciones_IFP_P3 tbody tr').length; i++) {
                        if ($('#TabCotizaciones_IFP_P3 tbody tr:eq(' + i + ') input').is(":checked")) {
                            var renta = $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + i + ') td span.RentaMensual').text();
                            if (renta == 0) {
                                $('#MCMIcono').attr('class', 'validacion');
                                $('#MCMContenedor').html('Seleccione una <strong>Cotización</strong> con una Renta mayor a 0');
                                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                                $('#ModalCuadroMensaje').dialog('open');
                                return;
                            }
                        }
                    }
                }

                if ($('#HSeleccionada').val() == "S") {
                    $('#MCAAceptarCierre_Plus').trigger("click");
                }
                else {
                    $("#MCAIcono").attr("class", "advertencia");
                    $("#MCAContenedor").html("Deseas cerrar la cotización seleccionada?");
                    $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                    $("#ModalCuadroAdvertencia").dialog("open");
                }
            }
        }
        else {
            return false;
        }
    });
    //<FINGTI_7012>

    /* ACTIVIDADES */
    var idActividad;
    var Actividad;
    $('#TabActividades .grilla_consultar').live('click', function () {

        $('#ModActCargando').show();

        var idActividad = $(this).data('actividad');

        var params = {
            cuspp: $('#CUSPP_RP').val(),
            idActividad: idActividad
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/ObtenerDatosActividad',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                Actividad = data.d;
                $('#ModActEvento').val(Actividad.GlosaEvento);
                $('#ModActComentario').val(Actividad.Comentario);
                $('#ModActCargando').fadeOut();

                /*<SRIINI17003>*/
                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                $clock.countdown(selectedDate.toString());
                /*<SRIFIN17003>*/
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la actividad.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });

        $('#ModalActividad').dialog('open');
    });

    /* Botón Cerrar */
    $('#ModActCerrar').live('click', function () {
        $('#ModalActividad').dialog('close');
    });


    /* Botón Buscar */
    var SegIdJefe;
    var SegIdSupervisor;
    var SegIdAgente;
    var SegCUSPP;
    var SegFechaInicio;
    var SegFechaTermino;
    $('#Buscar').live('click', function (e) {
        if (!botonesBloqueados) {
            $('#TablaSeguimientoError').hide();

            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            $("#CUSPP_RP").removeClass("formTextboxError");
            $("#FechaInicio").removeClass("formCalendarError");
            $("#FechaTermino").removeClass("formCalendarError");

            // CUSPP
            var cuspp = true;
            var eCuspp = true;
            if ($.trim($("#CUSPP_RP").val()).length > 0) {
                eCuspp = true;
                if ($.trim($("#CUSPP_RP").val()).length != 12) {
                    errores.push("El campo <strong>CUSPP</strong> debe contener 12 caracteres.");
                    cuspp = false;
                }
            }

            // Fecha Inicio
            var fechaInicio = true;
            if ($.trim($('#FechaInicio').val()).length == 0) {
                errores.push('Ingrese el campo <strong>Fecha de Inicio</strong>. Dato Obligatorio.');
                fechaInicio = false;
            }

            // Fecha Término
            var fechaTermino = true;
            if ($.trim($('#FechaTermino').val()).length == 0) {
                errores.push('Ingrese el campo <strong>Fecha de Término</strong>. Dato Obligatorio.');
                fechaTermino = false;
            }

            // Clases de controles
            if (!cuspp) { $('#CUSPP_RP').attr('class', 'formTextbox formTextboxError'); } else { $('#FechaInicio').attr('class', 'formTextbox'); }
            if (!fechaInicio) { $('#FechaInicio').attr('class', 'formTextbox formTextboxError formCalendarError'); } else { $('#FechaInicio').attr('class', 'formTextbox formCalendar'); }
            if (!fechaTermino) { $('#FechaTermino').attr('class', 'formTextbox formTextboxError formCalendarError'); } else { $('#FechaTermino').attr('class', 'formTextbox formCalendar'); }

            esCorrecto = cuspp & fechaInicio & fechaTermino;

            if (esCorrecto) {
                SegIdJefe = ($('#Jefe').val() != null) ? $('#Jefe').val() : null;
                SegIdSupervisor = ($('#Supervisor').val() != null) ? $('#Supervisor').val() : null;
                SegIdAgente = $('#Agente').val();
                SegCUSPP = ($('#CUSPP_RP').val() != null) ? $('#CUSPP_RP').val() : '';
                SegFechaInicio = $('#FechaInicio').val();
                SegFechaTermino = $('#FechaTermino').val();

                $('#HJefe').val(SegIdJefe);
                $('#HSupervisor').val(SegIdSupervisor);
                $('#HAgente').val(SegIdAgente);
                $('#HCUSPP_RP').val(SegCUSPP);
                $('#HFechaInicio').val(SegFechaInicio);
                $('#HFechaTermino').val(SegFechaTermino);

                $('#TabSeguimientoIndicePagina').val(1);
                $('#TabSeguimientoColumnaOrdenar').val(1);
                $('#TabSeguimientoDireccionOrdenar').val('A');

                CargarTablaSeguimiento(SegIdJefe, SegIdSupervisor, SegIdAgente, SegCUSPP, SegFechaInicio, SegFechaTermino);
            }
            else {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }
        }
        else {
            return false;
        }
    });

    $('#Buscar').live('click', function (e) {
        if (!botonesBloqueados) {
            $('#HJefe').val(SegIdJefe);
            $('#HSupervisor').val(SegIdSupervisor);
            $('#HAgente').val(SegIdAgente);
            $('#HCUSPP_RP').val(SegCUSPP);
            $('#HFechaInicio').val(SegFechaInicio);
            $('#HFechaTermino').val(SegFechaTermino);
        }
        else {
            return false;
        }
    });


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
    /* Botón Aceptar */
    $('#MCAAceptar_Plus').live('click', function () {
        if ($('#MCATablaEliminar').val() == 'dir') {
            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                idDireccion: idDireccion
            }
            var postUrl = 'Cotizador.aspx/EliminarDireccion';
        }
        else if ($('#MCATablaEliminar').val() == 'tel') {
            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                idTelefono: idTelefono
            }
            var postUrl = 'Cotizador.aspx/EliminarTelefono';
        }
        else if ($('#MCATablaEliminar').val() == 'cot') {
            /*Nos sirve para capturar que grilla refrescará, según el plan*/
            var bPlan1 = false;
            var bPlan2 = false;
            var bPlan3 = false;

            if (Solicitud.Cotizaciones[idCotizacion].Plan.Id == 'PLAN1') {
                bPlan1 = true;
            }
            if (Solicitud.Cotizaciones[idCotizacion].Plan.Id == 'PLAN2') {
                bPlan2 = true;
            }
            if (Solicitud.Cotizaciones[idCotizacion].Plan.Id == 'PLAN3') {
                bPlan3 = true;
            }

            Solicitud.Cotizaciones.splice(idCotizacion, 1);
            $('#ModalCuadroAdvertencia').dialog('close');

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
            }

            if (bPlan1) {
                //alert('PLAN1');
                CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
            }

            if (bPlan2) {
                //alert('PLAN2');
                CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
            }
            if (bPlan3) {
                CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
            }

            return;
        }
        else if ($('#MCATablaEliminar').val() == 'cierre') {
            //$('#EstadoCivil_RP').addClass('formTextboxError');
            $('#Pes1').click()
            $('#ModalCuadroAdvertencia').dialog('close');
            return;
        }
        else if ($('#MCATablaEliminar').val() == 'nuevoAfi') {
            hdCamposAfi($('#TipoDocumento_RP'), true);
            hdCamposAfi($('#NumeroDocumento_RP'), true);
            hdCamposAfi($('#ApellidoPaterno_RP'), true);
            hdCamposAfi($('#ApellidoMaterno_RP'), true);
            hdCamposAfi($('#Nombres_RP'), true);
            hdCamposAfi($('#FechaNacimiento_RP'), true);
            hdCamposAfi($('#Sexo_RP'), true);
            hdCamposAfi($('#CorreoElectronico_RP'), true);
            hdCamposAfi($('#EstadoCivil_RP'), true);

            $('#ModalCuadroAdvertencia').dialog('close');

            $('#TipoDocumento_RP').val($('#TipoDocumentoBusqueda_RP').val());
            $('#TexTipoDocumento_RP').html($('#TipoDocumentoBusqueda_RP').find(':selected').text());
            $('#NumeroDocumento_RP').val($('#NumeroDocumentoBusqueda_RP').val());

            if (permisoGuardar) {

            }

            return;
        }
        else if ($('#MCATablaEliminar').val() == 'GF') {
            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                idGrupoFamiliar: idGrupoFamiliar
            }
            var postUrl = 'Cotizador.aspx/EliminarGrupoFamiliar';
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
                    $('#ModalCuadroAdvertencia').dialog('close');

                    if ($('#MCATablaEliminar').val() == 'dir') {
                        /* Recargar la grilla de direcciones */
                        CargarTablaDirecciones();
                    }
                    else if ($('#MCATablaEliminar').val() == 'tel') {
                        /* Recargar la grilla de teléfonos */
                        CargarTablaTelefonos();
                    }
                    else if ($('#MCATablaEliminar').val() == 'GF') {
                        CargarTablaGrupoFamiliar_RP();
                    }

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
                    $('#ModalCuadroMensaje').dialog('open');
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

    /* Botón Cancelar */
    $('#MCACancelar_Plus').live('click', function () {
        //$("#ModSolModo").val('CERRAR')
        $('#ModalCuadroAdvertencia').dialog('close');
        $('#ContenedorGuardar_RP').hide();
    });

    //<INIGTI_7012>
    /* Botón Aceptar Cierre*/
    $('#MCAAceptarCierre_Plus').live('click', function () {
        if (!botonModSolAceptarBloqueado) {
            if ($("#ModSolModo").val() == "CERRAR") {
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

                var fecSolicitud = $('#ModSolFechaCotizacion_IFP').text();
                var estadoSolicitud = Solicitud.CodigoEstado;
                var idSolicitud = Solicitud.Id;
                var codPlan = ""
                for (var i = 0; i < Solicitud.Cotizaciones.length; i++) {
                    if (Solicitud.Cotizaciones[i].Correlativo == idSolicitudElegida) {
                        codPlan = Solicitud.Cotizaciones[i].Plan.Id;
                    }
                }

                //-----------------------------------------------------------------------------------------
                var params = {
                    solicitud: idSolicitud,
                    fecha: fecSolicitud,
                    estado: estadoSolicitud,
                    numCorrelativo: idSolicitudElegida,
                    codPlan: codPlan,
                    paginaLlamada: '../RentaIFP/SeleccionSolicitud.aspx'
                }

                $("#HCotizado").val("1");

                $.ajax({
                    type: 'POST',
                    url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/SessionIdGrupoFamiliar',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (estadoSolicitud != 3 && estadoSolicitud != 7) {
                            var params2 = {
                                idGrupoFamiliar: '0', //Cero (0) inicial
                            }

                            $.ajax({
                                type: 'POST',
                                url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/BuscarGrupoFamiliarSession',
                                contentType: "application/json; charset=iso-8859-1",
                                dataType: 'json',
                                data: $.toJSON(params2),
                                success: function (data) {
                                    //window.location.href = "../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx";
                                    //$('#Contenido_ModSolSiguiente_RP').click();
                                    PreseleccionarCotizacion(idSolicitud, idSolicitudElegida);
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
                        else {
                            var params = {
                                idSolicitud: idSolicitud,
                                fecCotizacion: fecSoltud,
                                accion: 'CERRAR'
                            }
                            $.ajax({
                                type: 'POST',
                                url: 'Cotizador.aspx/SessionIdSolicitud',
                                contentType: "application/json; charset=iso-8859-1",
                                dataType: 'json',
                                data: $.toJSON(params),
                                success: function (data) {

                                    var solicitud = data.d;
                                    if (idSolicitud.toString() == solicitud.toString()) {
                                        window.location.href = "SeleccionSolicitud.aspx";
                                    }
                                },
                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                        document.location.reload(true);
                                    }
                                    else {
                                        $('#MCMIcono').attr('class', 'error');
                                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
                                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                        $('#ModalCuadroMensaje').dialog('open');
                                    }
                                    $('#ModalSolicitud').dialog('close');
                                }
                            });
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
            }
            else if ($("#ModSolModo").val() == "CorregirDocumentos") {
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

                $("#ModSolCargando").fadeIn();
                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Reaperturando los documentos, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Procesando" });
                $("#ModalCotizando").dialog("open");

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    num_solicitud: Solicitud.Id,
                    num_correlativo: idSolicitudElegida
                }

                $.ajax({
                    type: "POST",
                    url: "SeleccionSolicitud.aspx/ReaperturarDocumentos",
                    contentType: "application/json; charset=utf8",
                    dataType: "json",
                    data: JSON.stringify(params),
                    success: function (data) {
                        if (data.d.Estado == "OK") {
                            // Mostrar mensaje de éxito
                            $("#MCMIcono").attr("class", data.d.Icono);
                            $("#MCMContenedor").html(data.d.Mensaje);
                            $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                            $("#ModalCuadroMensaje").dialog("open");

                            const fecSolicitud = $('#ModSolFechaCotizacion_IFP').text();

                            setTimeout(function () {
                                window.location.href = "../RentaIFP/SeleccionSolicitud.aspx?origen=cotizador" + "&modSolModo=" + "CERRAR" + "&idSolicitud=" + Solicitud.Id + "&fechaSolicitud=" + fecSolicitud;
                            }, 2000);
                        }
                        else if (data.d.Estado == "TOKEN") {
                            CerrarSesionExpirada();
                        }
                        else {
                            $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                            $("#MCMIcono").attr("class", data.d.Icono);
                            $("#MCMContenedor").html(data.d.Mensaje);
                            $("#ModalCuadroMensaje").dialog("open");
                            $("#ModSolAceptar_RP").attr("class", "boton darkblue sharp");
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            document.location.reload(true);
                        }
                        else {
                            $("#MCMIcono").attr("class", "error");
                            $("#MCMContenedor").html("Ha ocurrido un error al guardar la información de la solicitud.");
                            $("#ModalCuadroMensaje").dialog({ title: "Error" });
                            $("#ModalCuadroMensaje").dialog('open');
                        }
                    },
                    complete: function () {
                        // Cerrar modal de espera
                        $("#ModalCotizando").dialog("close");
                    }
                });
            }
        }
        else {

            return false;
        }
    });

    /* Calcular monto de ACOM - Inicio */
    $("#ModSolACOM,#ModSolIndSeleccionado,#TabCotizaciones_IFP_P1 input[type=radio]").live("change", function () {
        if (!botonModSolAceptarBloqueado) {
            // Validar sólo si existe el textbox de Monto ACOM (oficiales)
            if ($("#ModSolValMontoAcomAgente").length > 0) {
                if ($("#ModSolACOM").val() > 0 && $("#ModSolIndSeleccionado").val() == "S" && $("#TabCotizaciones_IFP_P1 input[type=radio]:checked").length > 0) {
                    // Bloquear botón Aceptar
                    $("#ModSolMontoACOMCargando").show();
                    botonModSolAceptarBloqueado = true;
                    $("#ModSolAceptarOficial").attr("class", "botonDeshabilitado gris gris_sharp");

                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        solicitud: Solicitud.Id,
                        acom: $("#ModSolACOM").val(),
                        cotizacion: $("#TabCotizaciones_IFP_P1 input[type=radio]:checked").val()
                    }
                    $.ajax({
                        type: "POST",
                        url: rutaObtenerMontoACOM,
                        contentType: "application/json; charset=iso-8859-1",
                        data: $.toJSON(params),
                        dataType: "json",
                        success: function (data) {
                            $("#ModSolValMontoAcomAgente").val(data.d.Contenido);
                            $("#ModSolValMontoAcomAgente").autoNumeric("update", { vMax: data.d.Contenido, aDec: ".", aSep: "," });
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                document.location.reload(true);
                            }
                            else {
                                $("#MCMIcono").attr("class", "error");
                                $("#MCMContenedor").html("Ha ocurrido un error al cargar el Monto A.");
                                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                                $("#ModalCuadroMensaje").dialog("open");
                            }
                        },
                        complete: function () {
                            $("#ModSolMontoACOMCargando").hide();
                            botonModSolAceptarBloqueado = false;
                            $("#ModSolAceptarOficial").attr("class", "boton darkblue sharp");
                        }
                    });
                }
                else {
                    $("#ModSolValMontoAcomAgente").val("0.00");
                    $("#ModSolValMontoAcomAgente").autoNumeric("update", { vMax: "0.00" });
                }
            }
        }
    });
    /* Calcular monto de ACOM - Fin */

    if ($('#ModDirModo').val() == "M") {
        CargandoUbigeo();
    };

    if ($('#ModTelModo').val() == "M") {
        CargandoTelefono();
    };

    //<INIGTI_7012>
    if ($('#ModSolModo').val() == "N") {
        NuevaSolicitud();
    };

    if ($('#ModSolModo').val() == "M" || $('#ModSolModo').val() == "C" || $('#ModSolModo').val() == "CONS"
        || $('#ModSolModo').val() == "CERRAR") {
        CargandoSolicitud();
    };

    //<FINGTI_7012>

    /*if ($('#ModGruFamModo_RP').val() == "M") {
        CargandoGrupoFamiliar();
    }*/

    /* Cambiando la moneda */
    $("#ModSolMonedaPrimaUnica_IFP").live("change", async function () {

        var valPrimaUnica_IFP = false
        var valMonedaPrimaUnica_IFP = false
        var valPlan_IFP = false

        if ($.trim($('#ModSolPrimaUnica_IFP').val()).length > 0) {
            if ($('#ModSolPrimaUnica_IFP').val() != 0.00) {
                valPrimaUnica_IFP = true;
            }
        }

        if ($('#ModSolMonedaPrimaUnica_IFP').val() != "0") {
            valMonedaPrimaUnica_IFP = true;
        }

        if ($('#ModSolPlan_IFP').val() != "0") {
            valPlan_IFP = true;
        }

        esCorrecto = valPrimaUnica_IFP & valMonedaPrimaUnica_IFP & valPlan_IFP;

        /*VALIDACION DE TOPE DE COTIZACIONES*/
        var vCantSolicitudes = false;
        if (valPrimaUnica_IFP & valMonedaPrimaUnica_IFP) {
            var resultadoValidacion = await validaCantSolicitudes($('#ModSolMonedaPrimaUnica_IFP').val(), $('#ModSolPrimaUnica_IFP').val())

            vCantSolicitudes = resultadoValidacion.isValid;

            if (!vCantSolicitudes) {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('No es posible cotizar, ha alcanzado el límite de cotizaciones.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
                $('#ModalCotizando').dialog('close');

                $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            } else {
                $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
            }
        }

        if (esCorrecto && vCantSolicitudes) {
            $('#ModSolAgregarPlan_IFP').removeClass('botonDeshabilitado gris gris_sharp');
            $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');
            //$('#ModSolAgregarPlan_IFP').addClass('boton darkblue sharp');

            var flag_plan = false;

            for (var i_plan = 0; i_plan < Plan.length; i_plan++) {
                if (Plan[i_plan] == $('#ModSolPlan_IFP').val()) {
                    flag_plan = true;
                }
            }

            if (flag_plan) {
                $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
            }
            else {
                $('#ModSolAgregarPlan_IFP').addClass('boton darkblue sharp');
            }

        } else {
            $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');
            $('#ModSolAgregarPlan_IFP').removeClass('botonDeshabilitado gris gris_sharp');
            $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
        }

        var bPlan1 = false;
        var bPlan2 = false;
        var bPlan3 = false;

        for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
            Solicitud.Cotizaciones[i].Moneda.Id = "000";
            if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN1') {
                bPlan1 = true;
            }
            if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN2') {
                bPlan2 = true;
            }
            if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN3') {
                bPlan3 = true;
            }
        }

        if (bPlan1) {
            CargarTablaCotizaciones_RP(Solicitud, $('#ModSolMonedaPrimaUnica_IFP').val(), 'PLAN1');
        }
        if (bPlan2) {
            CargarTablaCotizaciones_RP(Solicitud, $('#ModSolMonedaPrimaUnica_IFP').val(), 'PLAN2');
        }
        if (bPlan3) {
            CargarTablaCotizaciones_RP(Solicitud, $('#ModSolMonedaPrimaUnica_IFP').val(), 'PLAN3');
        }

        //if ($("#ModSolMonedaPrimaUnica_IFP").val() == "001") {
        //    $("#ModSolTipoCambioPanel").hide();
        //} else {
        //    $("#ModSolTipoCambioPanel").show();
        //};

    });

    /*cambio de Plan*/
    $("#ModSolPlan_IFP").live("change", function () {

        var valPrimaUnica_IFP = false
        var valMonedaPrimaUnica_IFP = false
        var valPlan_IFP = false

        if ($.trim($('#ModSolPrimaUnica_IFP').val()).length > 0) {
            if ($('#ModSolPrimaUnica_IFP').val() != 0.00) {
                valPrimaUnica_IFP = true;
            }
        }

        if ($('#ModSolMonedaPrimaUnica_IFP').val() != "0") {
            valMonedaPrimaUnica_IFP = true;
        }

        if ($('#ModSolPlan_IFP').val() != "0") {
            valPlan_IFP = true;
        }

        esCorrecto = valPrimaUnica_IFP & valMonedaPrimaUnica_IFP & valPlan_IFP;

        //if (esCorrecto) {
        //	esCorrecto = validaCantSolicitudes($('#ModSolMonedaPrimaUnica_IFP').val(), $('#ModSolPrimaUnica_IFP').val())
        //}

        if (esCorrecto) {
            $('#ModSolAgregarPlan_IFP').removeClass('botonDeshabilitado gris gris_sharp');
            $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');
            //$('#ModSolAgregarPlan_IFP').addClass('boton darkblue sharp');

            //var plan_array = new Array(Plan.split(','));

            var flag_plan = false;
            /*
            for (var i_plan = 0; i_plan < plan_array[0].length; i_plan++) {
                if (plan_array[0][i_plan] == $('#ModSolPlan_IFP').val()) {
                    flag_plan = true;
                }
            }
            */
            for (var i_plan = 0; i_plan < Plan.length; i_plan++) {
                if (Plan[i_plan] == $('#ModSolPlan_IFP').val()) {
                    flag_plan = true;
                }
            }

            if (flag_plan) {
                $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
            }
            else {
                $('#ModSolAgregarPlan_IFP').addClass('boton darkblue sharp');
            }


        } else {
            $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');
            $('#ModSolAgregarPlan_IFP').removeClass('botonDeshabilitado gris gris_sharp');

            $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
        }

    });

    /*cambio de PU*/
    $("#ModSolPrimaUnica_IFP").live("change", function () {

        var valPrimaUnica_IFP = false
        var valMonedaPrimaUnica_IFP = false
        var valPlan_IFP = false

        if ($.trim($('#ModSolPrimaUnica_IFP').val()).length > 0) {
            if ($('#ModSolPrimaUnica_IFP').val() != 0.00) {
                valPrimaUnica_IFP = true;
            }
        }

        if ($('#ModSolMonedaPrimaUnica_IFP').val() != "0") {
            valMonedaPrimaUnica_IFP = true;
        }

        if ($('#ModSolPlan_IFP').val() != "0") {
            valPlan_IFP = true;
        }

        esCorrecto = valPrimaUnica_IFP & valMonedaPrimaUnica_IFP & valPlan_IFP;

        //if (esCorrecto) {
        //    $('#ModSolAgregarPlan_IFP').removeClass('botonDeshabilitado gris gris_sharp');
        //    $('#ModSolAgregarPlan_IFP').addClass('boton darkblue sharp');
        //} else {
        //    $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');
        //    $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
        //}

        if (esCorrecto) {
            $('#ModSolAgregarPlan_IFP').removeClass('botonDeshabilitado gris gris_sharp');
            $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');
            //$('#ModSolAgregarPlan_IFP').addClass('boton darkblue sharp');

            //var plan_array = new Array(Plan.split(','));
            var flag_plan = false;
            /*
            for (var i_plan = 0; i_plan < plan_array[0].length; i_plan++) {
                if (plan_array[0][i_plan] == $('#ModSolPlan_IFP').val()) {
                    flag_plan = true;
                }
            }
            */
            for (var i_plan = 0; i_plan < Plan.length; i_plan++) {
                if (Plan[i_plan] == $('#ModSolPlan_IFP').val()) {
                    flag_plan = true;
                }
            }

            if (flag_plan) {
                $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
            }
            else {
                $('#ModSolAgregarPlan_IFP').addClass('boton darkblue sharp');
            }


        } else {
            $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');
            $('#ModSolAgregarPlan_IFP').removeClass('botonDeshabilitado gris gris_sharp');

            $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
        }

    });

    $("#ModSolPrimaUnica_IFP").live('keyup', function () {

        var valPrimaUnica_IFP = false
        var valMonedaPrimaUnica_IFP = false
        var valPlan_IFP = false

        if ($.trim($('#ModSolPrimaUnica_IFP').val()).length > 0) {
            if ($('#ModSolPrimaUnica_IFP').val() != 0.00) {
                valPrimaUnica_IFP = true;
            }
        }

        if ($('#ModSolMonedaPrimaUnica_IFP').val() != "0") {
            valMonedaPrimaUnica_IFP = true;
        }

        if ($('#ModSolPlan_IFP').val() != "0") {
            valPlan_IFP = true;
        }

        esCorrecto = valPrimaUnica_IFP & valMonedaPrimaUnica_IFP & valPlan_IFP;

        //if (esCorrecto) {
        //	esCorrecto = validaCantSolicitudes($('#ModSolMonedaPrimaUnica_IFP').val(), $('#ModSolPrimaUnica_IFP').val())
        //}

        if (esCorrecto) {
            $('#ModSolAgregarPlan_IFP').removeClass('botonDeshabilitado gris gris_sharp');
            $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');

            var flag_plan = false;

            for (var i_plan = 0; i_plan < Plan.length; i_plan++) {
                if (Plan[i_plan] == $('#ModSolPlan_IFP').val()) {
                    flag_plan = true;
                }
            }

            if (flag_plan) {
                $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
            }
            else {
                $('#ModSolAgregarPlan_IFP').addClass('boton darkblue sharp');
            }


        } else {
            $('#ModSolAgregarPlan_IFP').removeClass('boton darkblue sharp');
            $('#ModSolAgregarPlan_IFP').removeClass('botonDeshabilitado gris gris_sharp');

            $('#ModSolAgregarPlan_IFP').addClass('botonDeshabilitado gris gris_sharp');
        }

    });

    async function validaCantSolicitudes(pMonedaPU, pPrimaUnica) {
        var resultado = { isValid: false, count: 0 };

        try {
            var cuspp = JSON.parse(localStorage.getItem("afiliadoIfp"))?.CUSPP;

            if (!cuspp) {
                throw new Error("CUSPP no encontrado en localStorage");
            }

            const response = await ApiCotizadorIFP.ValidarCantidadSolicitudes(
                cuspp,
                pMonedaPU,
                parseFloat(pPrimaUnica.replace(/,/g, ""))
            );

            if (response) {
                if (response.d === "TOPE") {
                    resultado.isValid = false;

                    // Show message about reaching the limit
                    $("#MCMIcono").attr("class", "warning");
                    $("#MCMContenedor").html("Ha alcanzado el número máximo de cotizaciones permitidas para este cliente.");
                    $("#ModalCuadroMensaje").dialog({ title: "Advertencia" });
                    $("#ModalCuadroMensaje").dialog("open");

                    $('#ModSolAgregarArchivos').removeAttr('disabled');
                    $('#ModSolAgregarArchivos').attr('class', 'boton darkblue sharp');
                }
                else if (response.d === "OK") {
                    resultado.isValid = true;
                }
                else {
                    // Handle unexpected responses
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Respuesta no reconocida del servidor.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }

                // Set the count from the response
                resultado.count = response.count || 0;
            } else {
                throw new Error("Formato de respuesta incorrecto del servidor");
            }

        } catch (error) {
            console.error("Error en validaCantSolicitudes:", error);

            $("#MCMIcono").attr("class", "error");
            $("#MCMContenedor").html("Ha ocurrido un error al validar solicitud: " + error.message);
            $("#ModalCuadroMensaje").dialog({ title: "Error" });
            $("#ModalCuadroMensaje").dialog("open");

            resultado.isValid = false;
            resultado.count = 0;
        }

        return resultado;
    }

    //$("#ModSolTemporalidad_RP").live("change", function () {
    //CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_IFP").val(), $("#HConyuge").val());//<INIGTI_753>
    //});


    /* boton cancelar y aceptar de nueva solicitud, recarga la pagina cotizador.aspx#datos_solicitud */
    if (window.location.hash == "#datos_solicitud") {
        $('#Pestanhas li').attr('class', '');
        $('#Pestanha1').hide();
        $('#Pestanha3').show();
        $("#Pes3").attr('class', 'seleccionado');
    }

    /* boton cancelar y aceptar de nueva direccion, recarga la pagina cotizador.aspx#nueva_direccion*/
    if (window.location.hash == "#nueva_direccion") {
        $('#DatosDireccion_RP').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosDireccion_RP').is(':visible'))
                $('#AgrupadorDireccion_RP span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorDireccion_RP span').attr('class', 'agrupador_titulo_mas');
        });
    }

    /* boton cancelar y aceptar de nueva direccion, recarga la pagina cotizador.aspx#nueva_direccion*/
    if (window.location.hash == "#nuevo_telefono") {
        $('#DatosTelefono_RP').slideToggle('fast', function () {
            //Animation complete.
            if ($('#DatosTelefono_RP').is(':visible'))
                $('#AgrupadorTelefono_RP span').attr('class', 'agrupador_titulo_menos')
            else
                $('#AgrupadorTelefono_RP span').attr('class', 'agrupador_titulo_mas');
        });
    }

    /* boton cancelar y aceptar de nueva solicitud, recarga la pagina cotizador.aspx#datos_solicitud */
    if (window.location.hash == "#grupo_familiar") {
        $('#Pestanhas li').attr('class', '');
        $('#Pestanha1').hide();
        $('#Pestanha2').show();
        $("#Pes2").attr('class', 'seleccionado');
    }

    ////<INIGTI_753>
    //$('#ModSolTipoPlan_RP').live('change', function () {
    //	// Obtener la lista de beneficiarios
    //	for (i = 0; i < $('#TabBeneficiarios_RP tbody tr').length; i++) {
    //		if ($('#ModSolTipoPlan_RP').val() == "01") {
    //			var idParentesco = $("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").data("parentesco");
    //			//<INIGTI_753>
    //			if (idParentesco != "80") {
    //				$('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').prop("checked", "checked");
    //				$('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').removeAttr("disabled");

    //				if (idParentesco == "10") {
    //					$("#HConyuge").val("TRUE");
    //				} else {
    //					$("#HConyuge").val("FALSE");
    //				}
    //			}

    //			//if (i == 1) {

    //			//} else {
    //			//    $("#HConyuge").val("FALSE");
    //			//}
    //			//<FINGTI_753>
    //		} else {
    //			var idParentesco = $("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").data("parentesco");
    //			if (idParentesco != "80") {
    //				$('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').prop("checked", "");
    //				$('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').attr("disabled", "true");
    //			}
    //			//<INIGTI_753>
    //			$("#HConyuge").val("FALSE");
    //			//<FINGTI_753>
    //		}

    //		$('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');
    //	}

    //	if ($("#HConyuge").val() == "TRUE") {
    //		if ($('#TabBeneficiarios_RP input[type=checkbox]:checked').length != 2) {
    //			$("#HConyuge").val("FALSE");
    //		}
    //	}

    //	// Lista de cotizaciones
    //	CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
    //	//
    //});

    //<INIGTI_753>
    $('#ManSolPes1').live('click', function () {

        // Lista de cotizaciones
        //<INIGTI_7012>
        if ($('#ModSolModo').val() == "CERRAR") {
            CargarTablaCotizacionesCierre_RP(Solicitud, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_IFP").val(), $("#HConyuge").val());
        } else {
            //<INIGTI_7012>
            //Si no selecciona ningun familiar adicional, entonces es tipo de plan (Individual)
            //if ($('#TabBeneficiarios_RP input[type=checkbox]:checked').length == 1) {
            //    $('#ModSolTipoPlan_RP').val("02");
            //    $('#TexModSolTipoPlan_RP').html($('#ModSolTipoPlan_RP').find(':selected').text());
            //}
            //<FINGTI_7012>
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
                for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                    Solicitud.Cotizaciones[i].ValPjeConyuge = "0";
                }
                $("#HConyuge").val("FALSE");
            }

            //CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_IFP").val(), $("#HConyuge").val());//<INIGTI_753>
        }
        //<FINGTI_7012>
    });

    //<INIGTI_753>

    //<FINGTI_753>

    //Alex
    //if ($('#ModSolModo').val() == "N") {

    //    // Lista de cotizaciones
    //    CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
    //}

    //$("#elementoMasInfo").mouseenter(function (e) {
    //    $("#masInfo").css("display", "block");
    //});

    //$("#elementoMasInfo").mouseleave(function (e) {
    //    $("#masInfo").css("display", "none");
    //});

    $("#mostrar").click(function () {

        if ($("#masInfo").is(":visible")) {
            $("#masInfo").hide();
            $('#mostrar').text('Mas información')
        }
        else {
            $("#masInfo").show();
            $('#mostrar').text('Ocultar')
        }

    });

    //$("#EstadoCivil_RP").change(function () {

    //});

    $('#ModSolAgregarArchivos').live('click', function () {

        $('#ModSolAgregarArchivos').attr('disabled', 'disabled');
        $('#ModSolAgregarArchivos').attr('class', 'botonDeshabilitado gris gris_sharp');

        $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
        $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');

        //var params = {
        //    tokenUsuario: $('#TokenUsuario').val(),
        //    num_solicitud: $('#ModSolNroSolicitud_IFP').text()
        //}

        //$.ajax({
        //    type: "POST",
        //    url: "SeleccionSolicitud.aspx/ValidarAgregarArchivo",
        //    contentType: "application/json; charset=iso-8859-1",
        //    data: $.toJSON(params),
        //    dataType: "json",
        //    success: function (data) {
        //        
        //        if (data.d.Estado == "OK") {
        $("#divCargar").show();

        //
        ObtenerArchivos($("#HNroSolicitud").val(), true);

        // Deslizar la pantalla hacia los datos de simulación
        $('html,body').animate({
            scrollTop: $('#ModSolEnviarEvaluacion').offset().top
        }, 'slow');

        //} else if (data.d.Estado == "ERROR") {
        //    $("#MCMIcono").attr("class", "error");
        //    $("#MCMContenedor").html(data.d.Mensaje);
        //    $("#ModalCuadroMensaje").dialog({ title: "Error" });
        //    $("#ModalCuadroMensaje").dialog("open");

        //    $('#ModSolAgregarArchivos').removeAttr('disabled');
        //    $('#ModSolAgregarArchivos').attr('class', 'boton darkblue sharp');

        //}
        //},
        //error: function (XMLHttpRequest, textStatus, errorThrown) {
        //    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //        document.location.reload(true);
        //    }
        //    else {
        //        $("#MCMIcono").attr("class", "error");
        //        $("#MCMContenedor").html("Ha ocurrido un error al validar solicitud.");
        //        $("#ModalCuadroMensaje").dialog({ title: "Error" });
        //        $("#ModalCuadroMensaje").dialog("open");
        //    }
        //}
        //});

        return false;
    });

    $('#ModSolEnviarEvaluacion').live('click', function () {

        $("#MCAIconoEnviar").attr("class", "advertencia");
        $("#MCAContenedorEnviar").html("¿Desea enviar la cotización a evaluación?");
        $("#ModalCuadroEnviar").dialog({ title: "Confirmación" });
        $("#ModalCuadroEnviar").dialog("open");
        return false;
    });

    $("#CorregirDocumentos").live("click", function () {
        $("#MCAIcono").attr("class", "advertencia");
        $("#MCAContenedor").html("Esta opción volverá abrir los documentos firmados para que se puedan hacer correcciones. <b>Esto requerirá que el cliente los <span style=\"color:red\">firme nuevamente</span> y la solicitud no podrá ser enviada a evaluación hasta que el cliente los haya vuelto a firmar</b>. ¿Confirma que desea continuar?");
        $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
        $("#ModalCuadroAdvertencia").dialog("open");
        $("#ModSolModo").val("CorregirDocumentos");
        return false;
    });

    //No carga los documentos
    //Evaluación, Observado, Aprobado
    if ($('#HEstado').val() == "4" || $('#HEstado').val() == "5" || $('#HEstado').val() == "6") {
        $("#divCargar").show();
        ObtenerArchivos($("#HNroSolicitud").val(), true);

        // Deslizar la pantalla hacia los datos de simulación
        if ($('#HEstado').val() == "4" || $('#HEstado').val() == "5") {
            $('html,body').animate({
                scrollTop: $('#ModSolEnviarEvaluacion').offset().top
            }, 'slow');
        }

    }

    if ($('#HCotizado').val() == "0") {
        $('#ModSolAgregarArchivos').hide();
    }

    $('#TipoDocumentoBusqueda_RP').on('change', function () {
        var responseId = $(this).val();
        if (responseId === "D") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 8);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            $('#NumeroDocumentoBusqueda_RP').val('');
            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 8);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("enteroPositivo");
            $(".enteroPositivo").numeric();

        } else if (responseId === "E") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 9);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            $('#NumeroDocumentoBusqueda_RP').val('');
            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 9);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("enteroPositivo");
            $(".enteroPositivo").numeric();

        } else if (responseId === "J") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 11);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            $('#NumeroDocumentoBusqueda_RP').val('');
            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 11);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("enteroPositivo");
            $(".enteroPositivo").numeric();

        } else if (responseId === "N") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 15);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            $('#NumeroDocumentoBusqueda_RP').val('');
            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 15);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("alfanumerico");
            $(".alfanumerico").alphanumeric({ allow: "ñÑ" });

        } else if (responseId === "P") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 12);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            $('#NumeroDocumentoBusqueda_RP').val('');
            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 12);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("alfanumerico");
            $(".alfanumerico").alphanumeric({ allow: "ñÑ" });

        } else if (responseId === "R") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 11);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            $('#NumeroDocumentoBusqueda_RP').val('');
            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 11);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("enteroPositivo");
            $(".enteroPositivo").numeric();

        } else {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 30);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            $('#NumeroDocumentoBusqueda_RP').val('');
            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 30);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("alfanumerico");
            $(".alfanumerico").alphanumeric({ allow: "ñÑ" });

        }

    });

    if ($("#indNuevoAfiliado").val() == 'SI') {
        ConfirmaNuevoAfiliado();
    }

    if ($('#hindPestaniaActiva').val() == '2') {

        if ($("#TipoDocumentoBusqueda_RP").val() == "D") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 8);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 8);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("enteroPositivo");
            $(".enteroPositivo").numeric();

        } else if ($("#TipoDocumentoBusqueda_RP").val() == "E") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 9);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 9);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("enteroPositivo");
            $(".enteroPositivo").numeric();

        } else if ($("#TipoDocumentoBusqueda_RP").val() == "J") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 11);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 11);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("enteroPositivo");
            $(".enteroPositivo").numeric();

        } else if ($("#TipoDocumentoBusqueda_RP").val() == "N") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 15);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 15);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("alfanumerico");
            $(".alfanumerico").alphanumeric({ allow: "ñÑ" });

        } else if ($("#TipoDocumentoBusqueda_RP").val() == "P") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 12);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 12);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("alfanumerico");
            $(".alfanumerico").alphanumeric({ allow: "ñÑ" });

        } else if ($("#TipoDocumentoBusqueda_RP").val() == "R") {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 11);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 11);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("enteroPositivo");
            $(".enteroPositivo").numeric();

        } else {

            $("#NumeroDocumentoBusqueda_RP").attr('maxlength', 30);
            $("#NumeroDocumentoBusqueda_RP").unbind();

            var str = $('#NumeroDocumentoBusqueda_RP').val();
            var final = str.substr(0, 30);
            $('#NumeroDocumentoBusqueda_RP').val(final);

            $("#NumeroDocumentoBusqueda_RP").removeClass("enteroPositivo");
            $("#NumeroDocumentoBusqueda_RP").removeClass("alfanumerico");

            $("#NumeroDocumentoBusqueda_RP").addClass("alfanumerico");
            $(".alfanumerico").alphanumeric({ allow: "ñÑ" });

        }

        if ($("#indNuevoAfiliado").val() == 'SI') {
            $(".grilla_info").show();
        }
    } else {
        if (typeof ($('#hindPestaniaActiva').val()) !== "undefined") {
            $(".grilla_info").hide();
        }
    }

    $('#BusAfiBuscar2_RP').live('click', function () {

        if (!botonBusAfiBuscarBloqueado && permisoBusAfiBuscar) {
            return true;
        }
        else {
            return false;
        }
    });

    function mensajeValidacion(mensaje) {
        $('#MCMIcono').attr('class', 'validacion');
        $('#MCMContenedor').html(mensaje);
        $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
        $('#ModalCuadroMensaje').dialog('open');
    }

    function errorCotizacion(respuesta) {
        $('#ModalCotizando').dialog('close');
        $('#ModalCuadroMensaje').dialog({ title: respuesta.Titulo });
        $('#MCMIcono').attr('class', respuesta.Icono);
        $('#MCMContenedor').html(respuesta.Mensaje);
        if (respuesta.Controles != null && respuesta.Controles.length > 0) {
            if (respuesta.Controles[0].length) $('#ModSolPrimaUnica_IFP').attr('class', respuesta.Controles[0]);
            if (respuesta.Controles[1].length) $('#ModSolFechaCotizacion_IFP').attr('class', respuesta.Controles[1]);
            if (respuesta.Controles[2].length) $('#ModSolFechaDevengue_IFP').attr('class', respuesta.Controles[2]);
            if (respuesta.Controles[3].length) $('#ModSolMonedaPrimaUnica_IFP').attr('class', respuesta.Controles[3]);

            $('#TabCotizaciones_IFP_P1 select').removeClass('formTextboxGridError');
            $('#TabCotizaciones_IFP_P2 select').removeClass('formTextboxGridError');
            $('#TabCotizaciones_IFP_P3 select').removeClass('formTextboxGridError');
            $('#TabClausulaAdicional select').removeClass('formTextboxGridError');
            $('#TabClausulaAdicional input').removeClass('formCalendarError');

            if (respuesta.Controles.length > 4) {
                var celdaError;
                for (i = 4; i < respuesta.Controles.length; i++) {
                    celdaError = respuesta.Controles[i].split(',');

                    if (celdaError[2] == 'PLAN1') {
                        $('#TabCotizaciones_IFP_P1 tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                    } else if (celdaError[2] == 'PLAN2') {
                        $('#TabCotizaciones_IFP_P2 tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                    } else if (celdaError[2] == 'PLAN3') {
                        $('#TabCotizaciones_IFP_P3 tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                    } else if (celdaError[2] == 'PLAN3_CA') {
                        var etiqueta = $('#TabClausulaAdicional tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') ').children();

                        if (etiqueta[0].tagName == 'INPUT') {
                            etiqueta.addClass('formCalendarError')
                        } else if (etiqueta[0].tagName == 'SELECT') {
                            etiqueta.addClass('formTextboxGridError')
                        }
                    }
                }
            }
        }
        $('#ModalCuadroMensaje').dialog('open');
        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
    }

    //<INI.GTI_26697>
    $('#LinkConsentimientoAsesoria_IFP').live('click', function (e) {
        $('#MCConstmAsesIcono_IFP').attr('class', 'advertencia');
        $('#MCConstmAsesContenedor_IFP').html('<div class="alerta-agente-contenido"> <p>Usted va a enviar el enlace de consentimiento y protección de datos personales al cliente <span class="resaltado">' + $('#Nombres_RP').val() + ' ' + $('#ApellidoPaterno_RP').val() + ' ' + $('#ApellidoMaterno_RP').val() + '</span> identificado con el <span class="resaltado">' + $('#TipoDocumento_RP').find(':selected').text() + '</span> <span class="resaltado">' + $('#NumeroDocumento_RP').val() + '</span> al siguiente correo electrónico: <span class="resaltado">' + $('#CorreoElectronico_RP').val() + '</span> </p> <p> Verifique que los datos son correctos, en caso haya un error, por favor modifique los datos en el vtiger, luego actualice la página en el Cotizador Web de Rentas y vuelva a intentarlo.</p></div>');
        $('#ModalConsentimientoAsesoria_IFP').dialog({ title: 'Consentimiento y protección de datos personales' });
        $('#ModalConsentimientoAsesoria_IFP').dialog('open');
    });

    $('#LinkConsentimientoAsesoria_IFP_SMS').live('click', function (e) {
        $('#MCConstmAsesIcono_IFP_SMS').attr('class', 'advertencia');

        var contenido = '<p>Usted va a enviar el enlace de consentimiento y protección de datos personales al cliente <span class="resaltado">' + $('#Nombres_RP').val() + ' ' + $('#ApellidoPaterno_RP').val() + ' ' + $('#ApellidoMaterno_RP').val() + '</span> identificado con el <span class="resaltado">' + $('#TipoDocumento_RP').find(':selected').text() + '</span> <span class="resaltado">' + $('#NumeroDocumento_RP').val() + '</span> al siguiente número de celular: <span class="resaltado">' + $("#Celular_RP").val() + '</span> </p>';
        contenido += '<p> Verifique que los datos sean correctos, en caso haya un error, por favor modifique los datos en el vtiger, luego actualice la página en el Cotizador Web de Rentas y vuelva a intentarlo.</p>';

        $("#MCConstmAsesContenedor_IFP_SMS").html("<div class=\"alerta-agente-contenido\">" + contenido + "</div>");
        $('#ModalConsentimientoAsesoria_IFP_SMS').dialog({ title: 'Consentimiento y protección de datos personales' });
        $('#ModalConsentimientoAsesoria_IFP_SMS').dialog('open');
    });

    /* Botón Cancelar Consentimiento Asesoria */
    $('#MCConstmAsesCancelar_IFP').live('click', function () {
        $('#ModalConsentimientoAsesoria_IFP').dialog('close');
    });

    /* Botón Cancelar Consentimiento Asesoria SMS*/
    $('#MCConstmAsesCancelar_IFP_SMS').live('click', function () {
        $('#ModalConsentimientoAsesoria_IFP_SMS').dialog('close');
    });

    /* Botón Enviar Consentimiento Asesoría */
    $("#MCConstmAsesEnviar_IFP").live("click", function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Enviando email con el enlace al Consentimiento de Asesoría, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            parametros: {
                nombres: $('#Nombres_RP').val(),
                tipoDocumento: $('#TipoDocumento_RP').val(),
                numeroDocumento: $("#NumeroDocumento_RP").val(),
                correo: $("#CorreoElectronico_RP").val(),
                cuspp: $("#HCUSPP_RP").val(),
                token: $("#HToken_IFP").val(),
                apellidoPaterno: $('#ApellidoPaterno_RP').val(),
                apellidoMaterno: $('#ApellidoMaterno_RP').val(),
                sexo: $('#Sexo_RP').val(),
                fechaNacimiento: $('#FechaNacimiento_RP').val(),
                telefono: $('#Telefono_RP').val(),
                celular: $('#Celular_RP').val(),
                idConsentimientoAsesoria: $('#HidConsentimientoAsesoria').val(),
                indConsentimiento: $('#HindConsentimiento').val(),
                canalComunicacion: "EMAIL"
            }
        }

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/EnviarConsentimientoAsesoria",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#ModalConsentimientoAsesoria_IFP").dialog("close");

                    // Mostrar mensaje de éxito
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else {
                    $("#ModalConsentimientoAsesoria_IFP").dialog("close");

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
                    $("#MCMContenedor").html("Ha ocurrido un error al enviar el consentimiento y protección de datos personales.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: () => {
                $("#ModalCotizando").dialog("close");
            }
        });
    });

    /* Botón Enviar Consentimiento Asesoría SMS*/
    $("#MCConstmAsesEnviar_IFP_SMS").live("click", function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Enviando SMS con el enlace al Consentimiento de Asesoría, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            parametros: {
                nombres: $('#Nombres_RP').val(),
                tipoDocumento: $('#TipoDocumento_RP').val(),
                numeroDocumento: $("#NumeroDocumento_RP").val(),
                correo: $("#CorreoElectronico_RP").val(),
                cuspp: $("#HCUSPP_RP").val(),
                token: $("#HToken_IFP").val(),
                apellidoPaterno: $('#ApellidoPaterno_RP').val(),
                apellidoMaterno: $('#ApellidoMaterno_RP').val(),
                sexo: $('#Sexo_RP').val(),
                fechaNacimiento: $('#FechaNacimiento_RP').val(),
                telefono: $('#Telefono_RP').val(),
                celular: $('#Celular_RP').val(),
                idConsentimientoAsesoria: $('#HidConsentimientoAsesoria').val(),
                indConsentimiento: $('#HindConsentimiento').val(),
                canalComunicacion: "SMS"
            }
        }

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/EnviarConsentimientoAsesoria",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#ModalConsentimientoAsesoria_IFP_SMS").dialog("close");

                    // Mostrar mensaje de éxito
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else {
                    $("#ModalConsentimientoAsesoria_IFP_SMS").dialog("close");

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
                    $("#MCMContenedor").html("Ha ocurrido un error al enviar el consentimiento y protección de datos personales.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: () => {
                $("#ModalCotizando").dialog("close");
            }
        });
    });

    $("#PlantillaConsentimientoAsesoria_IFP").live("click", function (e) {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Descargando el Formato de Consentimiento, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Descargando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            idConsentimientoAsesoria: $("#HidConsentimientoAsesoria").val(),
            tipoDocumento: $("#TipoDocumento_RP").val(),
            numeroDocumento: $("#NumeroDocumento_RP").val()
        }

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/FormatoConsentimientoAsesoria",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    // Mostrar mensaje de éxito
                    $("#MCMIcono").attr("class", data.d.Icono);
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                    $("#ModalCuadroMensaje").dialog("open");

                    window.open("../ArchivosTemporales/RVI/Consentimiento/Consentimiento_" + $("#TipoDocumento_RP").val() + $("#NumeroDocumento_RP").val() + ".pdf", "_blank");

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
                    $("#MCMContenedor").html("Ha ocurrido un error al enviar el consentimiento de asesoría.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
                $("#ModalCotizando").dialog("close");
            }
        });

    });

    $("#ReenviarPlantillaConsentimientoAsesoria_IFP").live("click", function () {
        $("#MRCAIcono").attr("class", "advertencia");
        $("#MRCAContenedor").html("<div class=\"alerta-agente-contenido\"><p>Usted va a enviar el formato firmado de consentimiento de asesoría al cliente <span class=\"resaltado\">" + $("#Nombres_RP").val() + " " + $("#ApellidoPaterno_RP").val() + " " + $("#ApellidoMaterno_RP").val() + "</span> al siguiente correo electrónico: <span class=\"resaltado\">" + $("#CorreoElectronico_RP").val() + "</span></p><p>Verifique que los datos sean correctos antes de realizar el envío.</p></div>");
        $("#ModalReenvioConsentimientoAsesoria").dialog({ title: "Reenviar Consentimiento de Asesoría" });
        $("#ModalReenvioConsentimientoAsesoria").dialog("open");
    });

    $("#MRCACancelar_IFP").live("click", function () {
        $("#ModalReenvioConsentimientoAsesoria").dialog("close");
    });

    $("#MRCAEnviar_IFP").live("click", function (e) {
        $("#ModalReenvioConsentimientoAsesoria").dialog("close");
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Reenviando el Formato de Consentimiento, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            cuspp: $("#CUSPP_RP").val(),
            idConsentimientoAsesoria: $("#HidConsentimientoAsesoria").val(),
            tipoIdentificacion: $("#TipoDocumento_RP").val(),
            numeroIdentificacion: $("#NumeroDocumento_RP").val(),
            nombre: $("#Nombres_RP").val(),
            apellidoPaterno: $("#ApellidoPaterno_RP").val(),
            apellidoMaterno: $("#ApellidoMaterno_RP").val(),
            email: $("#CorreoElectronico_RP").val(),
            numAgente: $("#NumeroAgente").val()
        };

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/ReenviarFormatoConsentimientoAsesoria",
            contentType: "application/json;",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    // Mostrar mensaje de éxito
                    $("#MCMIcono").attr("class", data.d.Icono);
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                    $("#ModalCuadroMensaje").dialog("open");

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
                    $("#MCMContenedor").html("Ha ocurrido un error al enviar el consentimiento de asesoría.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
                $("#ModalCotizando").dialog("close");
            }
        });
    });

    /* Botón Eliminar GF */
    $('#TabGrupoFamiliar .grilla_eliminar').live('click', function () {
        idGrupoFamiliar = $(this).data('grupofamiliar');
        $('#MCATablaEliminar').val('GF');
        $('#MCAIcono').attr('class', 'advertencia');
        $('#MCAContenedor').html('¿Está seguro de eliminar el beneficiario?');
        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
        $('#ModalCuadroAdvertencia').dialog('open');
        return false;
    });


    //Prueba
    $('#NuevaSolicitud_IFP2').live('click', function () {
        if (permisoNuevaSolicitud) {

            idSolicitud = "";
            var params = {
                idSolicitud: idSolicitud,
                fecCotizacion: '',
                accion: ''
            }

            $.ajax({
                type: 'POST',
                url: 'Cotizador.aspx/SessionIdSolicitud',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    var solicitud = data.d;
                    if (idSolicitud.toString() == solicitud.toString()) {
                        window.location.href = "IFP/Cotizador.aspx";
                    }
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
                }
            });

        }
        else {
            return false;
        }
    });

    //078 - Plan 3
    $('#TabBeneficiarios_RP input[type=text]').live('keyup', function () {

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.find('.ModSolBenId').val();
        var beneficiarioId = fila.find('input[name="Correlativo"]').val();

        var seleccion = $(fila).find(':checkbox').prop('checked');

        if (seleccion) {
            if (!Solicitud || !Solicitud.Beneficiarios) {
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                $('#ModalCuadroMensaje').dialog('open');
                return;
            }

            const { sumaPorcentaje, porcentajesBeneficiarios } = ObtenerPorcentajesBeneficiarios();

            $('.Porcentaje').autoNumeric({ vMin: '0', vMax: '100' });
            $('#ModGruSumaBenef_IFP').val(sumaPorcentaje);
            $('#ModGruSumaBenef_IFP').removeClass('formTextboxError');
            if (parseInt($('#ModGruSumaBenef_IFP').val()) > 100) {
                $('#ModGruSumaBenef_IFP').addClass('formTextboxError');
            }

            // $.ajax({
            //     type: 'POST',
            //     url: 'MantenerSolicitud.aspx/ModificarPorcentaje',
            //     contentType: "application/json; charset=iso-8859-1",
            //     dataType: 'json',
            //     data: $.toJSON(params),
            //     success: function (data) {
            //         if (data.d.Estado == 'OK') {
            //             $('.Porcentaje').autoNumeric({ vMin: '0', vMax: '100' });
            //             $('#ModGruSumaBenef_IFP').val(data.d.Mensaje);

            //             $('#ModGruSumaBenef_IFP').removeClass('formTextboxError');
            //             if (parseInt($('#ModGruSumaBenef_IFP').val()) > 100) {
            //                 $('#ModGruSumaBenef_IFP').addClass('formTextboxError');
            //             }
            //         }
            //         else if (data.d.Estado == "ERROR") {
            //             $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
            //             $('#MCMIcono').attr('class', data.d.Icono);
            //             $('#MCMContenedor').html(data.d.Mensaje);
            //             $('#ModalCuadroMensaje').dialog('open');
            //         }
            //         else if (data.d.Estado == 'TOKEN') {
            //             CerrarSesionExpirada();
            //         }
            //     },
            //     error: function (XMLHttpRequest, textStatus, errorThrown) {
            //         if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
            //             /* Sesión caducada */
            //             document.location.reload(true);
            //         }
            //         else {
            //             //$('#TablaBeneficiariosCargando_IFP').hide();
            //         }
            //     }
            // });
        }

    });

    $('#TabBeneficiarios_RP input[type=text]').live('change', function () {

        $(this).removeClass('formTextboxError');
        if (parseInt($(this).val()) == 0)
            $(this).addClass('formTextbox formTextboxError');
    });


    $("#TabSolicitudes_RP .grilla_pdf_estudio_necesidad").live('click', function (e) {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Obteniendo el PDF del estudio de necesidades, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Generando" });
        $("#ModalCotizando").dialog("open");

        var num_solicitud = $(this).data('edn');

        var params = {
            numSolicitud: num_solicitud
        }

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/ObtenerEstudioNecesidad",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    // Mostrar mensaje de éxito
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    //window.open(data.d.ArchivoSerializado, '_blank');
                    const archivoBase64 = `data:application/pdf;base64,${data.d.ArchivoSerializado}`;
                    const link = document.createElement("a");
                    const NombreArchivo = data.d.Contenido;

                    link.href = archivoBase64;
                    link.download = NombreArchivo;
                    link.click();

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
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
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al generar PDF de la póliza.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
                $("#ModalCotizando").dialog("close");
            }
        });
    });




});

//Fin Ready

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

function LimpiarFormularioBusquedaAfiliados() {
    $('#ModBusAfiApellidoPaterno_RP').removeClass('formTextboxError');
    $('#ModBusAfiApellidoMaterno_RP').removeClass('formTextboxError');
    $('#ModBusAfiNombres_RP').removeClass('formTextboxError');

    $('#ModBusAfiApellidoPaterno_RP').val('');
    $('#ModBusAfiApellidoMaterno_RP').val('');
    $('#ModBusAfiNombres_RP').val('');

    $('#ModBusAfiBuscar_RP').attr('class', 'boton darkblue sharp');

    $('#ModBusAfiCargando_RP').hide();
    $('#ModBusAfiTablaAfiliados_RP').hide();
    $('#TablaAfiliadosError_RP').hide();

    $('#TabAfiliadosIndicePagina_RP').val('1');
    $('#TabAfiliadosTamanhoPagina_RP').val('10');
    $('#TabAfiliadosColumnaOrdenar_RP').val('1');
    $('#TabAfiliadosDireccionOrdenar_RP').val('A');
}

function LimpiarFormularioDirecciones() {
    $('#ModDirDireccion').removeClass('formTextboxError');
    $('#ConModDirDepartamento').removeClass('formComboboxErrorContenedor');
    $('#ConModDirCiudad').removeClass('formComboboxErrorContenedor');
    $('#ConModDirComuna').removeClass('formComboboxErrorContenedor');
    $('#ConModDirPrincipal').removeClass('formComboboxErrorContenedor');

    $('#ModDirDireccion').val('');
    $('#ModDirDepartamento').val('0');
    $('#TexModDirDepartamento').html($('#ModDirDepartamento').find(':selected').text());
    $('#ModDirCiudad').val('0');
    $('#TexModDirCiudad').html($('#ModDirCiudad').find(':selected').text());
    $('#ModDirCiudad').attr('disabled', 'disabled');
    $('#ModDirComuna').val('0');
    $('#TexModDirComuna').html($('#ModDirComuna').find(':selected').text());
    $('#ModDirComuna').attr('disabled', 'disabled');
    $('#ModDirPrincipal').val('0');
    $('#TexModDirPrincipal').html($('#ModDirPrincipal').find(':selected').text());
}

function LimpiarFormularioTelefonos() {
    $('#ConModTelTipo').removeClass('formComboboxErrorContenedor');
    $('#ModTelNumero').removeClass('formTextboxError');
    $('#ConModTelPrincipal').removeClass('formComboboxErrorContenedor');

    $('#ModTelTipo').val('0');
    $('#TexModTelTipo').html($('#ModTelTipo').find(':selected').text());
    $('#ModTelNumero').val('');
    $('#ModTelPrincipal').val('0');
    $('#TexModTelPrincipal').html($('#ModTelPrincipal').find(':selected').text());
}

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
    $('#ModGruFamCorreoElectronico_RP').removeClass('formTextboxError');

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

    $('#ModGruFamCorreoElectronico_RP').val('');

    $('#ModGruFamFechaInvalidez_RP').attr('disabled', 'disabled');
    $('#ModGruFamFechaInvalidez_RP').addClass('formCalendarReadOnly');
}

function LimpiarFormularioSolicitud() {
    $('#ModSolNroSolicitud_IFP').removeClass('formTextboxError');
    $('#ModSolTipoCambio_IFP').removeClass('formTextboxError');
    $('#ModSolFechaDevengue_IFP').removeClass('formTextboxError formCalendarError');
    $('#ModSolFechaCotizacion_IFP').removeClass('formTextboxError formCalendarError');
    $('#ModSolPrimaUnica_IFP').removeClass('formTextboxError');
    $('#ModSolNroSolicitud_IFP').val('');
    $('#ModSolTipoCambio_IFP').val('');
    botonModSolAceptarBloqueado = false;
    //$('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
    $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
    $('#TablaCotizacionesCargando_RP').hide();
}

function LimpiarFormularioCorreo() {
    $('#ModEnvCorDe').val('');
    $('#ModEnvCorPara').val('');
    $('#ModEnvCorAsunto').val('');
    $('#ModEnvCorMensaje').val('');
}

//function formatearMonto(val) {
//    return String(val).split("").reverse().join("")
//                      .replace(/(.{3}\B)/g, "$1,")
//                      .split("").reverse().join("");
//}

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
async function ObtenerMonedaAjuste(codMoneda, objeto, fil) {
    try {
        const response = await ApiCotizadorIFP.ObtenerMonedaAjuste(codMoneda);

        var objdata = response;

        objeto.empty();

        for (var m = 0; m < objdata.length; m++) {
            objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);
        }

        // Actualizar temporizador
        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        $clock.countdown(selectedDate.toString());

    } catch (error) {
        console.error("Error en ObtenerMonedaAjuste:", error);

        $('#MCMIcono').attr('class', 'error');
        $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        $('#ModalCuadroMensaje').dialog('open');
    }
}
//<FINGTI_753>

//<INIGTI_7012_S24>
async function ObtenerPagoDoble(temporalidad, diferido, objeto, fila) {
    var fil = fila.find('.ModSolItem').val();

    try {
        const response = await ApiCotizadorIFP.ObtenerPagoDoble(temporalidad, diferido);

        var objdata = response;
        var flatValor = false;

        objeto.empty();

        for (var m = 0; m < objdata.length; m++) {
            objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

            if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].PagoDoble) {
                flatValor = true;
            }
        }

        if (flatValor) {
            fila.find('.ModSolPagoEscalonada').val(Solicitud.Cotizaciones[fil].PagoDoble);
        } else {
            Solicitud.Cotizaciones[fil].PagoDoble = 0;
            Solicitud.Cotizaciones[fil].PjePagoDoble = 0;
            fila.find('.ModSolPagoEscalonada').val("0");
        }

        // Actualizar temporizador
        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        $clock.countdown(selectedDate.toString());

    } catch (error) {
        console.error("Error en ObtenerPagoDoble:", error);

        $('#MCMIcono').attr('class', 'error');
        $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        $('#ModalCuadroMensaje').dialog('open');
    }
}

async function ObtenerPagoDiferimiento(temporalidad, objeto, plan, fila) {
    var fil = fila.find('.ModSolItem').val();

    try {
        const response = await ApiCotizadorIFP.ObtenerPagoDiferimiento(temporalidad, plan);

        var objdata = response;
        var flatValor = false;

        objeto.empty();

        for (var m = 0; m < objdata.length; m++) {
            objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

            if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].ValPerDiferido) {
                flatValor = true;
            }
        }

        if (flatValor) {
            fila.find('.ModSolDiferimiento').val(Solicitud.Cotizaciones[fil].ValPerDiferido);
        } else {
            Solicitud.Cotizaciones[fil].ValPerDiferido = 0;
            fila.find('.ModSolDiferimiento').val("0");
        }

        if (Solicitud.Cotizaciones[fil].ValPerDiferido == Solicitud.Cotizaciones[fil].Temporalidad.Anhos) {
            fila.find('.ModSolSobrevivencia').val('100');
            Solicitud.Cotizaciones[fil].ValPjeDev = 100;
            Solicitud.Cotizaciones[fil].ValPjeDevFallec = 100;

            if (Solicitud.Cotizaciones[fil].Plan.Id == 'PLAN1') {
                await ObtenerDevFallecimiento(fila.find('.ModSolSobrevivencia').val(), fila.find('.ModSolFallecimiento'), true, fila.find('.ModSolFallecimiento'), fila);
            }

            fila.find('.ModSolSobrevivencia').attr('disabled', 'disabled');
            fila.find('.ModSolFallecimiento').attr('disabled', 'disabled');
            fila.find('.ModSolPagoEscalonada').attr('disabled', 'disabled');
            fila.find('.ModSolDevolucion').val('100');
            fila.find('.ModSolDevolucion').attr('disabled', 'disabled');
        } else {
            fila.find('.ModSolSobrevivencia').removeAttr("disabled");
            fila.find('.ModSolFallecimiento').removeAttr("disabled");
            fila.find('.ModSolPagoEscalonada').removeAttr("disabled");
            fila.find('.ModSolDevolucion').removeAttr("disabled");
        }

        await ObtenerPagoDoble(temporalidad, fila.find('.ModSolDiferimiento').val(), fila.find('.ModSolPagoEscalonada'), fila);

        // Actualizar temporizador
        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        $clock.countdown(selectedDate.toString());

    } catch (error) {
        console.error("Error en ObtenerPagoDiferimiento:", error);

        $('#MCMIcono').attr('class', 'error');
        $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        $('#ModalCuadroMensaje').dialog('open');
    }
}

async function ObtenerDevFallecimiento(devolucion, objeto, flag, combo, fila) {
    var fil = fila.find('.ModSolItem').val();

    try {
        const response = await ApiCotizadorIFP.ObtenerDevFallecimiento(devolucion);

        var objdata = response;
        var flatValor = false;

        objeto.empty();

        for (var m = 0; m < objdata.length; m++) {
            objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

            if (objdata[m].Id == Solicitud.Cotizaciones[fil].ValPjeDevFallec) {
                flatValor = true;
            }
        }

        if (flatValor) {
            fila.find('.ModSolFallecimiento').val(Solicitud.Cotizaciones[fil].ValPjeDevFallec);
        } else {
            Solicitud.Cotizaciones[fil].ValPjeDevFallec = 50;
            fila.find('.ModSolFallecimiento').val("50");
        }

        if (flag) {
            combo.val('100');
        }

        // Actualizar temporizador
        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        $clock.countdown(selectedDate.toString());

    } catch (error) {
        console.error("Error en ObtenerDevFallecimiento:", error);

        $('#MCMIcono').attr('class', 'error');
        $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        $('#ModalCuadroMensaje').dialog('open');
    }
}

async function ObtenerPagoDiferimientoCPagoDoble(pagoDoble, objeto, num_temporalidad, plan, fila) {
    var fil = fila.find('.ModSolItem').val();

    try {
        const response = await ApiCotizadorIFP.ObtenerPagoDiferimientoCPagoDoble(pagoDoble, num_temporalidad, plan);

        var objdata = response;
        var flatValor = false;

        objeto.empty();

        for (var m = 0; m < objdata.length; m++) {
            objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

            if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].ValPerDiferido) {
                flatValor = true;
            }
        }

        if (flatValor) {
            fila.find('.ModSolDiferimiento').val(Solicitud.Cotizaciones[fil].ValPerDiferido);
        } else {
            Solicitud.Cotizaciones[fil].ValPerDiferido = 0;
            Solicitud.Cotizaciones[fil].Moneda.Nombre = "Seleccione";
            fila.find('.ModSolDiferimiento').val("0");
        }

        // Actualizar temporizador
        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        $clock.countdown(selectedDate.toString());

    } catch (error) {
        console.error("Error en ObtenerPagoDiferimientoCPagoDoble:", error);

        $('#MCMIcono').attr('class', 'error');
        $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        $('#ModalCuadroMensaje').dialog('open');
    }
}

//<INI.GTI_7012_30>
function ObtenerMonedaFullDiferido(moneda, objeto, fila) {

    var fil = fila.find('.ModSolItem').val();

    var params = {
        codMoneda: moneda
    };

    $.ajax({
        type: 'POST',
        url: 'MantenerSolicitud.aspx/ObtenerMonedaFullDiferido',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {

            var objdata = data.d;
            var flatValor = false;

            objeto.empty();

            for (var m = 0; m < objdata.length; m++) {
                objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

                if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].Moneda.Nombre) {
                    flatValor = true;
                }

            };

            if (flatValor) {

                if (moneda == '001') {
                    if (Solicitud.Cotizaciones[fil].Moneda.Nombre == "Ajustados") {
                        fila.find('.ModSolMoneda')[0].selectedIndex = 2;
                    } else if (Solicitud.Cotizaciones[fil].Moneda.Nombre == "Nominal") {
                        fila.find('.ModSolMoneda')[0].selectedIndex = 3;
                    }
                    else {
                        fila.find('.ModSolMoneda').val(Solicitud.Cotizaciones[fil].Moneda.Id)
                    }
                } else if (moneda == '002') {
                    if (Solicitud.Cotizaciones[fil].Moneda.Nombre == "Nominal") {
                        fila.find('.ModSolMoneda')[0].selectedIndex = 1;
                    } else if (Solicitud.Cotizaciones[fil].Moneda.Nombre == "Ajustados") {
                        fila.find('.ModSolMoneda')[0].selectedIndex = 2;
                    }
                    else {
                        fila.find('.ModSolMoneda').val(Solicitud.Cotizaciones[fil].Moneda.Id)
                    }
                }

                //fila.find('.ModSolMoneda').val(Solicitud.Cotizaciones[fil].Moneda.Id)
            } else {
                Solicitud.Cotizaciones[fil].Moneda.Id = "000";
                Solicitud.Cotizaciones[fil].Moneda.Nombre = "Seleccione";
                fila.find('.ModSolMoneda').val("000");
            }

            //Actualizar temporizador
            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
            $clock.countdown(selectedDate.toString());
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                document.location.reload(true);
            }
            else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al cargar el listado.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
    });
}
//<FIN.GTI_7012_30>

function ObtenerMoneda(moneda, objeto, fila) {

    var fil = fila.find('.ModSolItem').val();

    var params = {
        codMoneda: moneda
    };

    $.ajax({
        type: 'POST',
        url: 'MantenerSolicitud.aspx/ObtenerMoneda',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {

            var objdata = data.d;
            var flatValor = false;

            objeto.empty();

            for (var m = 0; m < objdata.length; m++) {
                objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

                if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].Moneda.Nombre) {
                    flatValor = true;
                }

            };

            if (flatValor) {

                if (moneda == '001') {
                    if (Solicitud.Cotizaciones[fil].Moneda.Nombre == "Ajustados") {
                        fila.find('.ModSolMoneda')[0].selectedIndex = 2;
                    } else if (Solicitud.Cotizaciones[fil].Moneda.Nombre == "Nominal") {
                        fila.find('.ModSolMoneda')[0].selectedIndex = 3;
                    }
                    else {
                        fila.find('.ModSolMoneda').val(Solicitud.Cotizaciones[fil].Moneda.Id)
                    }
                } else if (moneda == '002') {
                    if (Solicitud.Cotizaciones[fil].Moneda.Nombre == "Nominal") {
                        fila.find('.ModSolMoneda')[0].selectedIndex = 1;
                    } else if (Solicitud.Cotizaciones[fil].Moneda.Nombre == "Ajustados") {
                        fila.find('.ModSolMoneda')[0].selectedIndex = 2;
                    }
                    else {
                        fila.find('.ModSolMoneda').val(Solicitud.Cotizaciones[fil].Moneda.Id)
                    }
                }



                //fila.find('.ModSolMoneda').val(Solicitud.Cotizaciones[fil].Moneda.Nombre)
            } else {
                Solicitud.Cotizaciones[fil].Moneda.Id = "000";
                fila.find('.ModSolMoneda').val("000");
            }

            //Actualizar temporizador
            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
            $clock.countdown(selectedDate.toString());
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                document.location.reload(true);
            }
            else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al cargar el listado.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
    });
}

function CargarParametroGeneralMotorIFP(cod_tipo_temporalidad, cod_moneda, fec_cotizacion) {

    $("#Precargando").fadeIn(1000);
    $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        cod_tipo_temporalidad: cod_tipo_temporalidad,
        cod_moneda: cod_moneda,
        fec_cotizacion: fec_cotizacion
    };

    $.ajax({
        type: 'POST',
        url: 'MantenerSolicitud.aspx/CargarParametroGeneralMotorIFP',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {

            if (data.d.Estado == 'ERROR') {
                $("#Precargando").fadeOut(1000);

                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html(data.d.Mensaje);
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
            else {
                $("#Precargando").fadeOut(1000);
                $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
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
                $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
    });
}
//<FINGTI_7012_S24>

$("#chkTodasSolicitudes").live("change", function () {
    CargarTablaSolicitudes_RP();
})

function ConfirmaNuevoAfiliado() {
    $('#MCATablaEliminar').val('nuevoAfi');
    $("#MCAIcono").attr("class", "advertencia");
    $("#MCAContenedor").html("Persona no encontrada, ¿Deseas registrarla?");
    $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
    $("#ModalCuadroAdvertencia").dialog("open");
}

function hdCamposAfi(object, accion) {
    //var tipoEtiqueta = object.is('select') ? 'Combobox' : 'Textbox';

    //if (object.is('select')) {
    //    //combobox
    //    $(object).prop('disabled', !accion);
    //    $(object).removeClass();
    //    $(object).parent().removeClass();
    //    if (accion) {
    //        $(object).addClass('formCombobox');
    //        $(object).parent().addClass('formComboboxContenedor');
    //    } else {
    //        $(object).addClass('aspNetDisabled formComboboxTexto formTextboxReadOnly');
    //        $(object).parent().addClass('aspNetDisabledContenedor formComboboxTextoContenedor formTextboxReadOnlyContenedor');
    //    }
    //} else {
    //    //textbox
    //    $(object).attr('readonly', !accion);
    //    if (accion) {
    //        $(object).removeClass('formTextboxReadOnly formTextboxLetra ColorNegro');
    //        if ($(object).hasClass('formCalendar')) {
    //            $(object).prop('disabled', !accion);
    //            $(object).removeClass('formCalendarReadOnly');
    //        }
    //    } else {
    //        $(object).addClass('formTextboxReadOnly formTextboxLetra ColorNegro');
    //        if ($(object).hasClass('formCalendar')) {
    //            $(object).prop('disabled', !accion);
    //            $(object).addClass('formCalendarReadOnly');
    //        }
    //    }
    //}
}

function HabilitarPlan2Inteligo() {
    $("#ModSolAgregarPlan_IFP").removeClass('botonDeshabilitado');
    valPlan_IFP = true;
    valPlan = 'PLAN2';

    $("#ModSolAgregarPlan_IFP").click();
}

function ActivarFecha() {
    $('.formCalendar').datepicker({
        yearRange: "1900:2200",
        monthNamesShort: ["Ene", "Feb", "Mar", "Abr", "May", "Jun", "Jul", "Ago", "Sep", "Oct", "Nov", "Dic"],
        dayNamesMin: ["Do", "Lu", "Ma", "Mi", "Ju", "Vi", "Sá"],
        dateFormat: "dd/mm/yy",
        showAnim: "fadeIn",
        changeMonth: true,
        changeYear: true
    });
    $(".fecha").mask("99/99/9999");

    //var regexDateValidator = function (fecha) {
    //    return (fecha).match(/^\d{4}\-\d{1,2}\-\d{1,2}$/);
    //}

    //$("#txtCAFecNacConyuge").blur(function () {
    //	accept = regexDateValidator($(this).val());
    //	if (!accept) $(this).val('');
    //});

    $('#txtCAFecNacConyuge').change(function () {
        for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
            if (Solicitud.Cotizaciones[i].ValPjeCACy != 0) {
                isValidDate($("#txtCAFecNacConyuge").val(), $("#txtCAFecNacConyuge"));
                return true;
            }
            else {
                $("#txtCAFecNacConyuge").removeClass('formCalendarError');
            }
        }
        //isValidDate($("#txtCAFecNacConyuge").val(), $("#txtCAFecNacConyuge"));
    });

    $('#txtCAFecNacPadre').change(function () {
        for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
            if (Solicitud.Cotizaciones[i].ValPjeCAPa != 0) {
                isValidDate($("#txtCAFecNacPadre").val(), $("#txtCAFecNacPadre"));
                return true;
            }
            else {
                $("#txtCAFecNacPadre").removeClass('formCalendarError');
            }
        }
        //isValidDate($("#txtCAFecNacPadre").val(), $("#txtCAFecNacPadre"));
    });

    $('#txtCAFecNacMadre').change(function () {
        for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
            if (Solicitud.Cotizaciones[i].ValPjeCAMa != 0) {
                isValidDate($("#txtCAFecNacMadre").val(), $("#txtCAFecNacMadre"));
                return true;
            }
            else {
                $("#txtCAFecNacMadre").removeClass('formCalendarError');
            }
        }
        //isValidDate($("#txtCAFecNacMadre").val(), $("#txtCAFecNacMadre"));
    });

    function isValidDate(dateString, objeto) {

        var fechaActual = new Date();

        //revisar el patrón
        if (!/^(\d{1,2})\/(\d{1,2})\/(\d{4})$/.test(dateString)) {

            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html("El formato de la fecha es incorrecto");
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');

            objeto.addClass('formCalendarError');
            return false;
        } else {
            objeto.removeClass('formCalendarError');
        }

        //convertir los números a enteros
        var parts = dateString.split("/");
        var day = parseInt(parts[0], 10);
        var month = parseInt(parts[1], 10);
        var year = parseInt(parts[2], 10);

        //Revisar los rangos de año y mes
        if ((year < 1900) || (year > fechaActual.getFullYear()) || (month == 0) || (month > 12) || (year + month + day) > (fechaActual.getFullYear() + (fechaActual.getMonth() + 1) + fechaActual.getDate())) {

            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html("La fecha no es válida");
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');

            objeto.addClass('formCalendarError');
            return false;
        } else {
            objeto.removeClass('formCalendarError');
        }

        var monthLength = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];

        //Ajustar para los años bisiestos
        if (year % 400 == 0 || (year % 100 != 0 && year % 4 == 0))
            monthLength[1] = 29;

        //Revisar el rango del día
        if (day > 0 && day <= monthLength[month - 1]) {
            objeto.removeClass('formCalendarError');
            //return true;
        } else {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html("El formato de la fecha es incorrecto");
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');

            objeto.addClass('formCalendarError');
            return false;
        }

    };

}

async function ObtenerPagoDobleCPeriodoGarantizado(periodoGarantizado, objeto, fila) {
    try {
        var fil = fila.find('.ModSolItem').val();

        var valorePeriodoDiferido = Solicitud.Cotizaciones[fil].ValPerDiferido;
        var valorEscalonado = 16;

        if (valorePeriodoDiferido > 0) {
            valorEscalonado = valorEscalonado - valorePeriodoDiferido;
        }

        var val_periodo_garantizado = Solicitud.Cotizaciones[fil].PeriodoGarantizado == 0 ? valorEscalonado : periodoGarantizado;

        console.log("ObtenerPagoDobleCPeriodoGarantizado: ", val_periodo_garantizado);
        const response = await ApiCotizadorIFP.ObtenerPagoDoblePeriodoGarantizado(val_periodo_garantizado);

        objdata = response;
        var flatValor = false;
        objeto.empty();

        for (var m = 0; m < objdata.length; m++) {
            objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

            if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].PagoDoble) {
                flatValor = true;
            }
        }

        if (flatValor) {
            fila.find('.ModSolPagoEscalonada').val(Solicitud.Cotizaciones[fil].PagoDoble);
        } else {
            Solicitud.Cotizaciones[fil].PagoDoble = 0;
            Solicitud.Cotizaciones[fil].PjePagoDoble = 0;

            fila.find('.ModSolTramoEscalonada').attr('disabled', 'disabled');
            fila.find('.ModSolTramoEscalonada').append(new Option("100%", "0"));
            fila.find('.ModSolTramoEscalonada').val('0');
            fila.find('.ModSolPagoEscalonada').val("0");
        }

        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        $clock.countdown(selectedDate.toString());

    } catch (error) {
        console.error("Error en ObtenerPagoDobleCPeriodoGarantizado:", error);

        $('#MCMIcono').attr('class', 'error');
        $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        $('#ModalCuadroMensaje').dialog('open');
    }

    /*$.ajax({
        type: 'POST',
        url: 'MantenerSolicitud.aspx/ObtenerPagoDobleCPeriodoGarantizado',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {

            var objdata = data.d;
            var flatValor = false;

            objeto.empty();

            for (var m = 0; m < objdata.length; m++) {
                objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

                if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].PagoDoble) {
                    flatValor = true;
                }

            };

            if (flatValor) {
                fila.find('.ModSolPagoEscalonada').val(Solicitud.Cotizaciones[fil].PagoDoble)
            } else {
                Solicitud.Cotizaciones[fil].PagoDoble = 0;
                Solicitud.Cotizaciones[fil].PjePagoDoble = 0;

                fila.find('.ModSolTramoEscalonada').attr('disabled', 'disabled');
                fila.find('.ModSolTramoEscalonada').append(new Option("100%", "0"));
                fila.find('.ModSolTramoEscalonada').val('0');
                fila.find('.ModSolPagoEscalonada').val("0");
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
                $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
    });*/
}

async function ObtenerDiferimientoCPeriodoGarantizado(periodoGarantizado, objeto, fila) {
    try {
        var tabla = $(objeto).closest('table').attr('id');
        var fil = fila.find('.ModSolItem').val();

        var val_periodo_garantizado = periodoGarantizado == 0 ? 16 : periodoGarantizado--;
        console.log("ObtenerDiferimientoCPeriodoGarantizado: ", val_periodo_garantizado);
        const response = await ApiCotizadorIFP.ObtenerDiferimientoPeriodoGarantizado(val_periodo_garantizado);

        objdata = response;
        var flatValorPagoDoble = false;
        var flatValorPerDiferido = false;

        objeto.empty();

        for (var m = 0; m < objdata.length; m++) {
            objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

            if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].PagoDoble) {
                flatValorPagoDoble = true;
            }
            if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].ValPerDiferido) {
                flatValorPerDiferido = true;
            }
        }

        if (flatValorPagoDoble) {
            fila.find('.ModSolPagoEscalonada').val(Solicitud.Cotizaciones[fil].PagoDoble);
        } else {
            Solicitud.Cotizaciones[fil].PagoDoble = 0;
            Solicitud.Cotizaciones[fil].PjePagoDoble = 0;

            fila.find('.ModSolTramoEscalonada').attr('disabled', 'disabled');
            fila.find('.ModSolTramoEscalonada').append(new Option("100%", "0"));
            fila.find('.ModSolTramoEscalonada').val('0');
            fila.find('.ModSolPagoEscalonada').val("0");
        }

        if (flatValorPerDiferido) {
            fila.find('.ModSolDiferimiento').val(Solicitud.Cotizaciones[fil].ValPerDiferido);

            if (Solicitud.Cotizaciones[fil].PeriodoGarantizado != 0) periodoGarantizado++;
            await ObtenerPagoDobleCPeriodoGarantizado(periodoGarantizado - Solicitud.Cotizaciones[fil].ValPerDiferido, fila.find('.ModSolPagoEscalonada'), fila);
        } else {
            Solicitud.Cotizaciones[fil].ValPerDiferido = 0;
            fila.find('.ModSolDiferimiento').val("0");

            if (Solicitud.Cotizaciones[fil].PeriodoGarantizado != 0) periodoGarantizado++;
            await ObtenerPagoDobleCPeriodoGarantizado(periodoGarantizado, fila.find('.ModSolPagoEscalonada'), fila);
        }

        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        $clock.countdown(selectedDate.toString());

    } catch (error) {
        console.error("Error en ObtenerDiferimientoCPeriodoGarantizado:", error);

        $('#MCMIcono').attr('class', 'error');
        $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        $('#ModalCuadroMensaje').dialog('open');
    }

    /*$.ajax({
        type: 'POST',
        url: 'MantenerSolicitud.aspx/ObtenerDiferimientoCPeriodoGarantizado',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {

            var objdata = data.d;
            var flatValorPagoDoble = false;
            var flatValorPerDiferido = false;

            objeto.empty();

            for (var m = 0; m < objdata.length; m++) {
                objeto[0].options[m] = new Option(objdata[m].Valor_1, objdata[m].Id);

                if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].PagoDoble) {
                    flatValorPagoDoble = true;
                }
                if (objdata[m].Valor_1 == Solicitud.Cotizaciones[fil].ValPerDiferido) {
                    flatValorPerDiferido = true;
                }

            };

            if (flatValorPagoDoble) {
                fila.find('.ModSolPagoEscalonada').val(Solicitud.Cotizaciones[fil].PagoDoble)
            } else {
                Solicitud.Cotizaciones[fil].PagoDoble = 0;
                Solicitud.Cotizaciones[fil].PjePagoDoble = 0;

                fila.find('.ModSolTramoEscalonada').attr('disabled', 'disabled');
                fila.find('.ModSolTramoEscalonada').append(new Option("100%", "0"));
                fila.find('.ModSolTramoEscalonada').val('0');
                fila.find('.ModSolPagoEscalonada').val("0");
            }

            if (flatValorPerDiferido) {
                fila.find('.ModSolDiferimiento').val(Solicitud.Cotizaciones[fil].ValPerDiferido)

                if (Solicitud.Cotizaciones[fil].PeriodoGarantizado != 0) periodoGarantizado++;
                ObtenerPagoDobleCPeriodoGarantizado(periodoGarantizado - Solicitud.Cotizaciones[fil].ValPerDiferido, fila.find('.ModSolPagoEscalonada'), fila);
            } else {
                Solicitud.Cotizaciones[fil].ValPerDiferido = 0;
                fila.find('.ModSolDiferimiento').val("0");

                if (Solicitud.Cotizaciones[fil].PeriodoGarantizado != 0) periodoGarantizado++;
                ObtenerPagoDobleCPeriodoGarantizado(periodoGarantizado, fila.find('.ModSolPagoEscalonada'), fila);
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
                $('#MCMContenedor').html('Ha ocurrido un error al cargar los listados.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
    });*/
}

function PreseleccionarCotizacion(numSolicitud, numCorrelativo) {
    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        num_solicitud: numSolicitud,
        num_correlativo: numCorrelativo
    }

    $.ajax({
        type: 'POST',
        url: '../RentaIFP/SeleccionSolicitud.aspx/PreseleccionarCotizacion',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == 'OK') {
                $('#Contenido_ModSolSiguiente_RP').click();
            }
            else if (data.d.Estado == 'TOKEN') {
                CerrarSesionExpirada();
            } else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al pre-seleccionar la cotización.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                document.location.reload(true);
            }
            else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al pre-seleccionar la cotización.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
            $('#ModalGrupoFamiliar').dialog('close');
        }
    });
}
