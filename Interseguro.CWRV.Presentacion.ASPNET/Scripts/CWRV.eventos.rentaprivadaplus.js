
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

var opcionesSistema = localStorage.getItem("opcionesSistema");
opcionesSistema = opcionesSistema ? JSON.parse(opcionesSistema) : []

$(document).ready(function () {
    var permisoBusAfiExaminarSolicitud = ($('#PerBusAfiExaminarSolicitud_RP').val() == '1' ? true : false);
    var permisoBusAfiBuscar = ($('#PerBusAfiBuscar_RP').val() == '1' ? true : false);
    var permisoGuardar = ($('#PerGuardar_RP').val() == '1' ? true : false);
    var permisoNuevaDireccion = ($('#PerNuevaDireccion_RP').val() == '1' ? true : false);
    var permisoNuevoTelefono = ($('#PerNuevoTelefono_RP').val() == '1' ? true : false);
    var permisoNuevoBeneficiario = ($('#PerNuevoBeneficiario_RP').val() == '1' ? true : false);
    var permisoNuevaSolicitud = ($('#PerNuevoBeneficiario_RP').val() == '1' ? true : false);

    if ($("#HFirmado").val() == 'S') $("#ModSolAceptarCierre_RP").hide();

    var idSolicitudTmp;
    var fechaCotizacionTmp;

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

        if ($(this).data('pestanha') != 1) {
            $('#BusAfiBuscar_RP,#BusAfiExaminarSolicitud_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').attr('readonly', true);
            $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').addClass('formTextboxReadOnly');
            botonBusAfiBuscarBloqueado = true;
        }
        else {
            if (permisoBusAfiExaminarSolicitud) $('#BusAfiExaminarSolicitud_RP').attr('class', 'boton darkblue sharp');
            if (permisoBusAfiBuscar) $('#BusAfiBuscar_RP').attr('class', 'boton darkblue sharp');
            $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').attr('readonly', false);
            $('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').removeClass('formTextboxReadOnly');
            botonBusAfiBuscarBloqueado = false;
        }
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

            CargarTablaAfiliados2(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
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

            CargarTablaAfiliados2(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
        }
    });

    /* Cambiando la fecha de cotizacion */
    $('#ModSolFechaCotizacion_RP').live('change', function () {
        //en el texto se ve: 14/03/2016 (dd/mm/yyyy)
        if ($("#ModSolFechaCotizacion_RP").val() != "") {
            var fecha = $("#ModSolFechaCotizacion_RP").val();
            var mes = fecha.substring(3, 5);
            var dia = fecha.substring(0, 2);
            var anho = fecha.substring(6, 10);
            var dt = new Date(anho + "/" + mes + "/" + dia);

            //if (mes == "12") {
            //    mes = "01";
            //    anho = "0" + (parseInt(anho) + 1);
            //    anho = anho.substring(anho.length, anho.length - 4);
            //} else {
            //    mes ="0" + (parseInt(mes) + 1);
            //    mes = mes.substring(mes.length, mes.length - 2);
            //}
            $("#ModSolFechaDevengue_RP").val("01/" + mes + "/" + anho);

            //var d = new Date();
            var fecvigencia = sumarDias(dt, 15);

            dia = "0" + fecvigencia.getDate();
            mes = "00" + fecvigencia.getMonth();
            anho = fecvigencia.getFullYear();

            dia = dia.substring(dia.length - 2, dia.length);
            mes = mes.substring(mes.length - 2, mes.length);


            $("#ModSolFechaVigencia_RP").val(dia + "/" + mes + "/" + anho);

            //alert(k);

        }
    });

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

    /* Verificar si se ha modificado algún campo */
    $('#CorreoElectronico_RP,#Categoria_RP,#AFP_RP,#SaldoCIC_RP').bind('keyup keydown keypress change', function () {
        if ($.trim($('#CUSPP_RP').val()).length > 0) {
            if ($(this).val() != jQuery.data(this, 'lastvalue')) {
                datosSinGuardar = true;
            }
            jQuery.data(this, 'lastvalue', $(this).val());
        }
    });

    /* Botón Guardar */
    $('#Guardar_RP').live('click', function () {
        if (!botonesBloqueados && permisoGuardar) {
            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            $('#CorreoElectronico_RP').removeClass('formTextboxError');
            $('#ConCategoria').removeClass('formComboboxErrorContenedor');
            $('#ConAFP').removeClass('formComboboxErrorContenedor');
            $('#SaldoCIC_RP').removeClass('formTextboxError');

            $('#RangoInversion_RP').removeClass('formTextboxError');
            $('#CentroLaboral_RP').removeClass('formTextboxError');

            // Correo Electrónico
            var correoElectronico = true;
            //if ($('#CorreoElectronico').length > 0) {
            //    if ($.trim($('#CorreoElectronico').val()).length == 0) {
            //        errores.push("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.");
            //        correoElectronico = false;
            //    }
            //}

            // Categoría
            var categoria = true;
            if ($('#Categoria_RP').val() == "0") {
                errores.push("Ingrese el campo <strong>Categoría</strong>. Dato Obligatorio.");
                categoria = false;
            }

            //// AFP
            //var afp = true;
            //if ($('#AFP_RP').val() == "0") {
            //    errores.push("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
            //    afp = false;
            //}

            // Saldo CIC
            var saldoCIC = true;
            if ($('#SaldoCIC_RP').length > 0) {
                if ($.trim($('#SaldoCIC_RP').val()).length == 0) {
                    errores.push("Ingrese el campo <strong>Saldo CIC</strong>. Dato Obligatorio.");
                    saldoCIC = false;
                }
            }

            // Clases de controles
            //if (!correoElectronico) { $('#CorreoElectronico_RP').attr('class', 'formTextbox formTextboxError'); } else { $('#CorreoElectronico').attr('class', 'formTextbox'); }
            if (!categoria) { $('#ConCategoria').attr('class', 'formComboboxContenedor formComboboxErrorContenedor'); } else { $('#ConCategoria').attr('class', 'formComboboxContenedor'); }
            //if (!afp) { $('#ConAFP').attr('class', 'formComboboxContenedor formComboboxErrorContenedor'); } else { $('#ConAFP').attr('class', 'formComboboxContenedor'); }
            if (!saldoCIC) { $('#SaldoCIC_RP').attr('class', 'formTextbox formTextboxError'); } else { $('#SaldoCIC_RP').attr('class', 'formTextbox'); }

            esCorrecto = correoElectronico & categoria & saldoCIC;

            if (!esCorrecto) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }

            // Pasó las validaciones
            botonesBloqueados = true;
            $('#Guardar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
        }
        else {
            return false;
        }
    });

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
                //var postUrl = '../RentaPrivada/Cotizador.aspx/InsertarDireccion';
                //window.location.href = "../RentaPrivada/DireccionAfiliado.aspx";
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
                //var postUrl = 'Cotizador.aspx/ModificarDireccion';
                var postUrl = '../RentaPrivadaPlus/Cotizador.aspx/ModificarDireccion';
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
        if (permisoNuevaDireccion) {
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
                //<SOLINI25621>
                //url: 'Cotizador.aspx/SessionIdGrupoFamiliar',
                url: '../RentaPrivadaPlus/GrupoFamiliarAfiliado.aspx/SessionIdGrupoFamiliar',
                //<SOLINI25621>
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                //<SOLINI25621>
                //data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "'}",
                data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "', paginaLlamada:'../RentaPrivadaPlus/Cotizador.aspx#grupo_familiar'}",
                //<SOLFIN25621>
                success: function (data) {

                    var grupofamiliar = data.d;
                    if (idGrupoFamiliar.toString() == grupofamiliar.toString()) {
                        //window.location.href = "DireccionAfiliado.aspx";
                        window.location.href = "../RentaPrivadaPlus/GrupoFamiliarAfiliado.aspx";
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

            ////$('#ModGruFamModo_RP').val('N');

            ////LimpiarFormularioGrupoFamiliar();

            ////$('#ModGruFamCargando_RP').hide();
            ////$('#ModalGrupoFamiliar_RP').dialog('open');
            ////$('#ModGruFamApellidoPaterno_RP').focus();
        }
        else {
            return false;
        }
    });

    /* Cargando grupo familiar*/
    function CargandoGrupoFamiliar() {
        $('#ModGruFamModo_RP').val('M');

        LimpiarFormularioGrupoFamiliar();

        //$('#ModGruFamCargando_RP').show();
        //$('#ModalGrupoFamiliar_RP').dialog('open');
        //idGrupoFamiliar = $(this).data('grupofamiliar');
        idGrupoFamiliar = $("#ModIdGrupoFamiliar").val();
        var params = {
            idGrupoFamiliar: idGrupoFamiliar
        }

        $.ajax({
            type: 'POST',
            url: '../RentaPrivadaPlus/GrupoFamiliarAfiliado.aspx/ObtenerDatosGrupoFamiliar',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                var grupo = data.d;
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
                $('#ModGruFamNumeroIdentificacion_RP').val(grupo.Identificacion.Numero);
                // Parentesco
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
                // Sexo
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

                // Fecha de Nacimiento
                $('#ModGruFamFechaNacimiento_RP').val(new Date(+grupo.FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                // Indicador de Invalidez
                $('#ModGruFamIndInvalidez_RP').val(grupo.Invalido ? 'S' : 'N');
                $('#TexModGruFamIndInvalidez_RP').html($('#ModGruFamIndInvalidez_RP').find(':selected').text());
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
                //SujetoObligado
                $('#ModGruFamSO_RP').val(grupo.ind_SujetoObligado ? 'S' : 'N');
                $('#TexModGruFamSO_RP').html($('#ModGruFamSO_RP').find(':selected').text());

                //Banco
                $('#ModGruFamNumeroBanco_RP').val(grupo.NumeroBanco);
                $('#ModGruFamBanco_RP').val(grupo.Banco.Id);
                $('#TexModGruFamBanco_RP').html($('#ModGruFamBanco_RP').find(':selected').text());

                //$('#ModGruFamBanco_RP').trigger('change');

                $.ajax({
                    type: 'POST',
                    url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: "{banco: '" + $("#ModGruFamBanco_RP").val() + "', id: ''}",
                    success: function (data) {
                        var items = "";
                        for (var posicion = 0; posicion < data.d.length; posicion++) {
                            items += "<option value='" + data.d[posicion].Id + "'>" + data.d[posicion].Nombre + "</option>";

                            //<INI.GTI_7012_V13>
                            if (grupo.TipoCtaBanco.Id == data.d[posicion].Id) {
                                $('#ModGruFamNumeroBanco_RP').mask(data.d[posicion].Valor_2);
                                $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                            }
                            //<FIN.GTI_7012_V13>
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

                //if (grupo.Banco.Id == "BCP")
                //{
                //    if (grupo.NumeroBanco.length == 16) 
                //    {
                //        $('#TexModGruFamBanco_RP').html($('#ModGruFamBanco_RP option:eq(5)').text());
                //        $('#ModGruFamBanco_RP option:contains("MAESTRA")').attr("selected", true);
                //        //$("#TexModGruFamBanco_RP option[value=" + $('#TexModGruFamBanco_RP').text() + "]").attr("selected", true);
                //        //$('#ModGruFamBanco_RP option:contains("INTERBANK")').attr("selected", true);
                //    }
                //    else 
                //    {
                //        $('#TexModGruFamBanco_RP').html($('#ModGruFamBanco_RP option:eq(4)').text());
                //        $('#ModGruFamBanco_RP option:contains("AHORROS")').attr("selected", true);
                //    }
                //}
                //else {
                //    $('#TexModGruFamBanco_RP').html($('#ModGruFamBanco_RP').find(':selected').text());
                //}

                //Comunicacion
                //$('#ModGruFamComunicacion_RP').val(grupo.Comunicacion.Id);
                $('#ModGruFamComunicacion_RP').val(1);
                $('#TexModGruFamComunicacion_RP').html($('#ModGruFamComunicacion_RP').find(':selected').text());

                $('#ModGruFamComunicacion_RP').attr('disabled', 'disabled');
                $('#ConModGruFamComunicacion_RP').addClass('formComboboxReadOnlyContenedor');
                //ConfidencialidadDatos
                $('#ModGruFamConfidencialidadDatos_RP').val(grupo.Confidencialidaddatos.Id);
                $('#TexModGruFamConfidencialidadDatos_RP').html($('#ModGruFamConfidencialidadDatos_RP').find(':selected').text());

                //<FINGTI_7012>

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
        //idGrupoFamiliar = 0;
        $.ajax({
            type: 'POST',
            //<SOLINI25621>
            //url: 'Cotizador.aspx/SessionIdGrupoFamiliar',
            url: '../RentaPrivadaPlus/GrupoFamiliarAfiliado.aspx/SessionIdGrupoFamiliar',
            //<SOLINI25621>
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            //<SOLINI25621>
            //data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "'}",
            data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "', paginaLlamada:'../RentaPrivadaPlus/Cotizador.aspx#grupo_familiar'}",
            //<SOLFIN25621>

            success: function (data) {

                var grupofamiliar = data.d;
                if (idGrupoFamiliar.toString() == grupofamiliar.toString()) {
                    window.location.href = "../RentaPrivadaPlus/GrupoFamiliarAfiliado.aspx";
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
            $('#ConModGruFamIndInvalidez_RP').removeClass('formComboboxErrorContenedor');
            $('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamFechaInvalidez_RP').removeClass('formTextboxError formCalendarError');

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

            // Invalidez
            var invalidez = true;
            var tipoInvalidez = true;
            var fechaInvalidez = true;
            if ($('#ModGruFamIndInvalidez_RP').val() == '0') {
                errores.push('Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.');
                invalidez = false;
            }
            else if ($('#ModGruFamIndInvalidez_RP').val() == 'N') {
                if ($('#ModGruFamTipoInvalidez_RP').val() != 'N') {
                    errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                    tipoInvalidez = false;
                }
                if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length > 0) {
                    errores.push('El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.');
                    fechaInvalidez = false;
                }
            }
            else if ($('#ModGruFamIndInvalidez_RP').val() == 'S') {
                if ($('#ModGruFamTipoInvalidez_RP').val() != 'P' && $('#ModGruFamTipoInvalidez_RP').val() != 'T') {
                    errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                    tipoInvalidez = false;
                }

                if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.');
                    fechaInvalidez = false;
                }
            }

            if ($('#ModGruFamTipoInvalidez_RP').val() == '0') {
                errores.push('Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.');
                tipoInvalidez = false;
            }

            if (!apellidoPaterno) $('#ModGruFamApellidoPaterno_RP').addClass('formTextboxError');
            if (!apellidoMaterno) $('#ModGruFamApellidoMaterno_RP').addClass('formTextboxError');
            if (!nombres) $('#ModGruFamNombres_RP').addClass('formTextboxError');
            if (!tipoIdentificacion) $('#ConModGruFamTipoIdentificacion_RP').addClass('formComboboxErrorContenedor');
            if (!numeroIdentificacion) $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxError');
            if (!parentesco) $('#ConModGruFamParentesco_RP').addClass('formComboboxErrorContenedor');
            if (!sexo) $('#ConModGruFamSexo_RP').addClass('formComboboxErrorContenedor');
            if (!fechaNacimiento) $('#ModGruFamFechaNacimiento_RP').addClass('formTextboxError formCalendarError');
            if (!invalidez) $('#ConModGruFamIndInvalidez_RP').addClass('formComboboxErrorContenedor');
            if (!tipoInvalidez) $('#ConModGruFamTipoInvalidez_RP').addClass('formComboboxErrorContenedor');
            if (!fechaInvalidez) $('#ModGruFamFechaInvalidez_RP').addClass('formTextboxError formCalendarError');

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento & invalidez & tipoInvalidez & fechaInvalidez;

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
                //<SOLINI25621>
                var postUrl = '../RentaPrivadaPlus/GrupoFamiliarAfiliado.aspx/InsertarGrupoFamiliar';
                //<SOLFIN25621>
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
                    flagRenta: 'particularplus'
                }
                //<SOLINI25621>
                var postUrl = '../RentaPrivadaPlus/GrupoFamiliarAfiliado.aspx/ModificarGrupoFamiliar';
                //<SOLFIN25621>
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

                        /* Recargar la grilla de teléfonos */
                        //CargarTablaGrupoFamiliar_RP();

                        /*<SRIINI17003>*/
                        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                        $clock.countdown(selectedDate.toString());
                        /*<SRIFIN17003>*/

                        $('#MCMIcono').attr('class', 'exito');
                        if ($('#ModGruFamModo_RP').val() == 'N') {
                            $('#MCMContenedor').html('Grupo familiar agregado correctamente.');
                        }
                        else {
                            $('#MCMContenedor').html('Grupo familiar modificado correctamente.');
                        }

                        $('#ModalCuadroMensaje').dialog({ title: 'Infomación' });
                        $('#ModalCuadroMensaje').dialog('open');

                        //$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                        //$('#MCMIcono').attr('class', data.d.Icono);
                        //$('#MCMContenedor').html(data.d.Mensaje);
                        //$('#ModalCuadroMensaje').dialog('open');
                        //<SOLINI25621>
                        window.location.href = $("#ModPaginaLlamada").val();
                        //window.location.href = "Cotizador.aspx#grupo_familiar";
                        //<SOLFIN25621>
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

    /* Botón Aceptar Cierre*/
    $('#ModGruFamAceptarCierre_RP').live('click', function () {

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
                $('#ConModGruFamIndInvalidez_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamFechaInvalidez_RP').removeClass('formTextboxError formCalendarError');

                $('#ModGruFamNacional_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamProfesion_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamResidencia_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamPEP_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamSO_RP').removeClass('formComboboxErrorContenedor');

                $('#ModGruFamBanco_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamTipoCtaBanco_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamComunicacion_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamNumeroBanco_RP').removeClass('formTextboxError');

                $('#ModGruFamMail_RP').removeClass('formTextboxError');
                $('#ModGruFamEstadoCivil_RP').removeClass('formTextboxError');
                $('#ModGruFamCentrolaboral_RP').removeClass('formTextboxError');
                $('#ModGruFamCargo_RP').removeClass('formTextboxError');
                $('#ModGruFamActividadeconomica_RP').removeClass('formTextboxError');
                $('#ModGruFamMonedaingreso_RP').removeClass('formTextboxError');
                $('#ModGruFamIngreso_RP').removeClass('formTextboxError');
                //$('#ModGruFamTelefono_RP').removeClass('formTextboxError');
                $('#ModGruFamCelular_RP').removeClass('formTextboxError');
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

                // Invalidez
                var invalidez = true;
                var tipoInvalidez = true;
                var fechaInvalidez = true;
                if ($('#ModGruFamIndInvalidez_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.');
                    invalidez = false;
                }
                else if ($('#ModGruFamIndInvalidez_RP').val() == 'N') {
                    if ($('#ModGruFamTipoInvalidez_RP').val() != 'N') {
                        errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                        tipoInvalidez = false;
                    }
                    if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length > 0) {
                        errores.push('El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.');
                        fechaInvalidez = false;
                    }
                }
                else if ($('#ModGruFamIndInvalidez_RP').val() == 'S') {
                    if ($('#ModGruFamTipoInvalidez_RP').val() != 'P' && $('#ModGruFamTipoInvalidez_RP').val() != 'T') {
                        errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                        tipoInvalidez = false;
                    }

                    if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length == 0) {
                        errores.push('Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.');
                        fechaInvalidez = false;
                    }
                }

                if ($('#ModGruFamTipoInvalidez_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.');
                    tipoInvalidez = false;
                }


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

                var email = true;
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
                        email = false;
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

                if (!apellidoPaterno) $('#ModGruFamApellidoPaterno_RP').addClass('formTextboxError');
                if (!apellidoMaterno) $('#ModGruFamApellidoMaterno_RP').addClass('formTextboxError');
                if (!nombres) $('#ModGruFamNombres_RP').addClass('formTextboxError');
                if (!tipoIdentificacion) $('#ConModGruFamTipoIdentificacion_RP').addClass('formComboboxErrorContenedor');
                if (!numeroIdentificacion) $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxError');
                if (!parentesco) $('#ConModGruFamParentesco_RP').addClass('formComboboxErrorContenedor');
                if (!sexo) $('#ConModGruFamSexo_RP').addClass('formComboboxErrorContenedor');
                if (!fechaNacimiento) $('#ModGruFamFechaNacimiento_RP').addClass('formTextboxError formCalendarError');
                if (!invalidez) $('#ConModGruFamIndInvalidez_RP').addClass('formComboboxErrorContenedor');
                if (!tipoInvalidez) $('#ConModGruFamTipoInvalidez_RP').addClass('formComboboxErrorContenedor');
                if (!fechaInvalidez) $('#ModGruFamFechaInvalidez_RP').addClass('formTextboxError formCalendarError');

                if (!nacionalidad) $('#ConModGruFamNacional_RP').addClass('formComboboxErrorContenedor');
                if (!profesion) $('#ConModGruFamProfesion_RP').addClass('formComboboxErrorContenedor');
                if (!residencia) $('#ConModGruFamResidencia_RP').addClass('formComboboxErrorContenedor');
                if (!pep) $('#ConModGruFamPEP_RP').addClass('formComboboxErrorContenedor');
                if (!sujetoObligado) $('#ConModGruFamSO_RP').addClass('formComboboxErrorContenedor');

                if (!Banco) $('#ConModGruFamBanco_RP').addClass('formComboboxErrorContenedor');
                if (!TipoBanco) $('#ConModGruFamTipoCtaBanco_RP').addClass('formComboboxErrorContenedor');
                if (!Comunicacion) $('#ConModGruFamComunicacion_RP').addClass('formComboboxErrorContenedor');
                if (!NumeroBanco) $('#ModGruFamNumeroBanco_RP').addClass('formTextboxError');

                if (!email) $('#ModGruFamMail_RP').addClass('formTextboxError');
                if (!estadoCivil) $('#ConModGruFamEstadoCivil_RP').addClass('formComboboxErrorContenedor');
                if (!centrolaboral) $('#ModGruFamCentrolaboral_RP').addClass('formTextboxError');
                if (!cargo) $('#ModGruFamCargo_RP').addClass('formTextboxError');
                if (!actividadeconomica) $('#ModGruFamActividadeconomica_RP').addClass('formTextboxError');
                if (!monedaingreso) $('#ConModGruFamMonedaingreso_RP').addClass('formComboboxErrorContenedor');
                if (!ingreso) $('#ModGruFamIngreso_RP').addClass('formTextboxError');
                //if (!telefono) $('#ModGruFamTelefono_RP').addClass('formTextboxError');
                if (!celular) $('#ModGruFamCelular_RP').addClass('formTextboxError');
                if (!origenFondo) $('#ModGruFamDestinoFondos_RP').addClass('formTextboxError');

                esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento & invalidez & tipoInvalidez & fechaInvalidez & nacionalidad & profesion & residencia & pep & sujetoObligado & Banco & TipoBanco & Comunicacion & NumeroBanco
                    & email & estadoCivil & centrolaboral & cargo & actividadeconomica & monedaingreso & ingreso & celular & origenFondo;

                if (!esCorrecto) {
                    $('#MCMIcono').attr('class', 'validacion');
                    $('#MCMContenedor').html(formatearError(errores));
                    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return false;
                }

                ///* Pasó las validaciones */
                //ModGruFamBotonesInactivos = true;
                $('#ModGruFamCancelarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                $('#ModGruFamAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                //$('#ModGruFamCargando_RP').fadeIn();

                //if ($('#ModGruFamModo_RP').val() == 'N') {
                //}
                //else if ($('#ModGruFamModo_RP').val() == 'M') {

                if ($('#ModGruFamParentesco_RP').val() != '80') {
                    $('#ModGruFamConfidencialidadDatos_RP').val('1')
                }

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
                    numeroBanco: $('#ModGruFamNumeroBanco_RP').val(),
                    confidencialidadDatos: $('#ModGruFamConfidencialidadDatos_RP').val(),
                    flagRenta: 'particularplus',
                    estadoCivil: str_estadoCivil,
                    correoElectronico: str_correoElectronico,
                    centroLaboral: str_centrolaboral,
                    cargo: str_cargo,
                    actividadEconomica: str_actividadeconomica,
                    monedaIngreso: str_monedaingreso,
                    ingreso: str_ingreso,
                    telefono: str_telefono,
                    celular: str_celular,
                    origen_fondo: str_origen_fondo
                }

                var postUrl = '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ModificarGrupoFamiliar';

                $.ajax({
                    type: 'POST',
                    url: postUrl,
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {

                            //$('#ModalGrupoFamiliar_RP').dialog('close');

                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                            $clock.countdown(selectedDate.toString());

                            //$('#MCMIcono').attr('class', 'exito');
                            //if ($('#ModGruFamModo_RP').val() == 'N') {
                            //    $('#MCMContenedor').html('Grupo familiar agregado correctamente.');
                            //}
                            //else {
                            //    $('#MCMContenedor').html('Grupo familiar modificado correctamente.');
                            //}

                            //$('#ModalCuadroMensaje').dialog({ title: 'Infomación' });
                            //$('#ModalCuadroMensaje').dialog('open');

                            //window.location.href = $("#ModPaginaLlamada").val();

                            if ($("#ModGruFamAceptarCierre_RP").text() == "Siguiente") {
                                //1
                                idSolicitud = $('#ModGruFamsoltud').val();
                                fecSoltud = $('#ModGruFamfecsoltud').val();
                                estadoSoltud = $('#ModGruFamestadosoltud').val();

                                cantidad = 0;

                                //<FIN.GTI_7012_20>
                                var params = {
                                    idGrupoFamiliar: cantidad,
                                    solitud: idSolicitud,
                                    fecha: fecSoltud,
                                    estado: estadoSoltud,
                                    paginaLlamada: $("#ModPaginaLlamada").val()
                                }
                                //<FIN.GTI_7012_20>

                                $.ajax({
                                    type: 'POST',
                                    url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/SessionIdGrupoFamiliar',
                                    contentType: "application/json; charset=iso-8859-1",
                                    dataType: 'json',
                                    //<INI.GTI_7012_20>
                                    //data: "{idGrupoFamiliar:'" + cantidad + "', solitud: '" + idSolicitud + "', fecha: '" + fecSoltud + "', estado: '" + estadoSoltud + "'}",
                                    data: $.toJSON(params),
                                    //<FIN.GTI_7012_20>
                                    success: function (data) {

                                        $.ajax({
                                            type: 'POST',
                                            url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerListaGruposFamiliares',
                                            contentType: "application/json; charset=iso-8859-1",
                                            dataType: 'json',
                                            data: "{idSolicitud:'" + $('#ModGruFamsoltud').val() + "', posicion: '" + parseInt($('#ModGruFamCantidad').val()) + "'}",

                                            success: function (data) {

                                                window.location.href = "../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx";

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
                            else if ($("#ModGruFamAceptarCierre_RP").text() == "Cerrar Solicitud") {

                                //var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
                                //idSolicitud = $(this).data('solicitud');

                                var fecCotizacion = $('#ModGruFamfecsoltud').val()
                                idSolicitud = $('#ModGruFamsoltud').val();

                                var params = {
                                    idSolicitud: idSolicitud,
                                    fecCotizacion: fecCotizacion,
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
                                            //<INI.GTI_7012_20>
                                            ////<INI.GTI_7012_V13>
                                            //if ($("#ModGruFamestadosoltud").val() == "1") {
                                            //    window.location.href = "CerrarSolicitud.aspx";
                                            //} else {
                                            //    window.location.href = "SeleccionSolicitud.aspx";
                                            //}
                                            ////<FIN.GTI_7012_V13>
                                            window.location.href = $('#ModPaginaLlamada').val();
                                            //<FIN.GTI_7012_20>
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
                            }
                            $('#ModalCuadroMensaje').dialog('open');

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
                        $('#ModGruFamCargando_RP').fadeOut();
                        ModGruFamBotonesInactivos = false;
                        $('#ModGruFamAceptar_RP').attr('class', 'boton darkblue sharp');
                    }
                });
            }

        }

    });

    /* Botón Aceptar */
    $('#ModGruFamCancelarCierre_RP').live('click', function () {

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
                $('#ConModGruFamIndInvalidez_RP').removeClass('formComboboxErrorContenedor');
                $('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamFechaInvalidez_RP').removeClass('formTextboxError formCalendarError');

                $('#ModGruFamNacional_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamProfesion_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamResidencia_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamPEP_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamSO_RP').removeClass('formComboboxErrorContenedor');

                $('#ModGruFamBanco_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamTipoCtaBanco_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamComunicacion_RP').removeClass('formComboboxErrorContenedor');
                $('#ModGruFamNumeroBanco_RP').removeClass('formTextboxError');

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

                // Invalidez
                var invalidez = true;
                var tipoInvalidez = true;
                var fechaInvalidez = true;
                if ($('#ModGruFamIndInvalidez_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.');
                    invalidez = false;
                }
                else if ($('#ModGruFamIndInvalidez_RP').val() == 'N') {
                    if ($('#ModGruFamTipoInvalidez_RP').val() != 'N') {
                        errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                        tipoInvalidez = false;
                    }
                    if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length > 0) {
                        errores.push('El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.');
                        fechaInvalidez = false;
                    }
                }
                else if ($('#ModGruFamIndInvalidez_RP').val() == 'S') {
                    if ($('#ModGruFamTipoInvalidez_RP').val() != 'P' && $('#ModGruFamTipoInvalidez_RP').val() != 'T') {
                        errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                        tipoInvalidez = false;
                    }

                    if ($.trim($('#ModGruFamFechaInvalidez_RP').val()).length == 0) {
                        errores.push('Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.');
                        fechaInvalidez = false;
                    }
                }

                if ($('#ModGruFamTipoInvalidez_RP').val() == '0') {
                    errores.push('Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.');
                    tipoInvalidez = false;
                }


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

                if (!apellidoPaterno) $('#ModGruFamApellidoPaterno_RP').addClass('formTextboxError');
                if (!apellidoMaterno) $('#ModGruFamApellidoMaterno_RP').addClass('formTextboxError');
                if (!nombres) $('#ModGruFamNombres_RP').addClass('formTextboxError');
                if (!tipoIdentificacion) $('#ConModGruFamTipoIdentificacion_RP').addClass('formComboboxErrorContenedor');
                if (!numeroIdentificacion) $('#ModGruFamNumeroIdentificacion_RP').addClass('formTextboxError');
                if (!parentesco) $('#ConModGruFamParentesco_RP').addClass('formComboboxErrorContenedor');
                if (!sexo) $('#ConModGruFamSexo_RP').addClass('formComboboxErrorContenedor');
                if (!fechaNacimiento) $('#ModGruFamFechaNacimiento_RP').addClass('formTextboxError formCalendarError');
                if (!invalidez) $('#ConModGruFamIndInvalidez_RP').addClass('formComboboxErrorContenedor');
                if (!tipoInvalidez) $('#ConModGruFamTipoInvalidez_RP').addClass('formComboboxErrorContenedor');
                if (!fechaInvalidez) $('#ModGruFamFechaInvalidez_RP').addClass('formTextboxError formCalendarError');
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

                esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento & invalidez & tipoInvalidez & fechaInvalidez & nacionalidad & profesion & residencia & pep & sujetoObligado & Banco & TipoBanco & Comunicacion & NumeroBanco;

                if (!esCorrecto) {
                    $('#MCMIcono').attr('class', 'validacion');
                    $('#MCMContenedor').html(formatearError(errores));
                    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return false;
                }

                ///* Pasó las validaciones */
                //ModGruFamBotonesInactivos = true;
                $('#ModGruFamCancelarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                $('#ModGruFamAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                //$('#ModGruFamCargando_RP').fadeIn();

                //if ($('#ModGruFamModo_RP').val() == 'N') {
                //}
                //else if ($('#ModGruFamModo_RP').val() == 'M') {

                if ($('#ModGruFamParentesco_RP').val() != '80') {
                    $('#ModGruFamConfidencialidadDatos_RP').val('1')
                }

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
                    flagRenta: 'particularplus'
                }

                var postUrl = '../RentaPrivadaPlus/GrupoFamiliarAfiliado.aspx/ModificarGrupoFamiliar';

                $.ajax({
                    type: 'POST',
                    url: postUrl,
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {

                            //$('#ModalGrupoFamiliar_RP').dialog('close');

                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                            $clock.countdown(selectedDate.toString());

                            //$('#MCMIcono').attr('class', 'exito');
                            //if ($('#ModGruFamModo_RP').val() == 'N') {
                            //    $('#MCMContenedor').html('Grupo familiar agregado correctamente.');
                            //}
                            //else {
                            //    $('#MCMContenedor').html('Grupo familiar modificado correctamente.');
                            //}

                            //$('#ModalCuadroMensaje').dialog({ title: 'Infomación' });
                            //$('#ModalCuadroMensaje').dialog('open');

                            //window.location.href = $("#ModPaginaLlamada").val();

                            //1
                            idSolicitud = $('#ModGruFamsoltud').val();
                            fecSoltud = $('#ModGruFamfecsoltud').val();
                            estadoSoltud = $('#ModGruFamestadosoltud').val();

                            cantidad = 0;

                            //<FIN.GTI_7012_20>
                            var params = {
                                idGrupoFamiliar: cantidad,
                                solitud: idSolicitud,
                                fecha: fecSoltud,
                                estado: estadoSoltud,
                                paginaLlamada: $('#ModPaginaLlamada').val()
                                //paginaLlamada: '../RentaPrivadaPlus/SeleccionSolicitud.aspx'

                            }
                            //<FIN.GTI_7012_20>

                            $.ajax({
                                type: 'POST',
                                url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/SessionIdGrupoFamiliar',
                                contentType: "application/json; charset=iso-8859-1",
                                dataType: 'json',
                                //data: "{idGrupoFamiliar:'" + cantidad + "', solitud: '" + idSolicitud + "', fecha: '" + fecSoltud + "', estado: '" + estadoSoltud + "'}",
                                data: $.toJSON(params),
                                success: function (data) {

                                    $.ajax({
                                        type: 'POST',
                                        url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerListaGruposFamiliares',
                                        contentType: "application/json; charset=iso-8859-1",
                                        dataType: 'json',
                                        data: "{idSolicitud:'" + $('#ModGruFamsoltud').val() + "', posicion: '" + (parseInt($('#ModGruFamCantidad').val()) - 2) + "'}",

                                        success: function (data) {

                                            window.location.href = "../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx";

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
                                if (data.d.Controles[8].length) $('#ConModGruFamIndInvalidez_RP').attr('class', data.d.Controles[8]);
                                if (data.d.Controles[9].length) $('#ConModGruFamTipoInvalidez_RP').attr('class', data.d.Controles[9]);
                                if (data.d.Controles[10].length) $('#ModGruFamFechaInvalidez_RP').attr('class', data.d.Controles[10]);
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
                        $('#ModGruFamCargando_RP').fadeOut();
                        ModGruFamBotonesInactivos = false;
                        $('#ModGruFamAceptar_RP').attr('class', 'boton darkblue sharp');
                    }
                });
            }



        }

    });

    /* Persona vinculada */
    var idGrupoFamiliar;

    /* Botón Nuevo */
    $('#NuevaPersonaVinculada_RPP').live('click', function () {
        $('#ModGruFamModo_RPP').val('N');

        LimpiarFormularioPersonasVinculadas();

        $('#ModGruFamCargando_RPP').hide();
        $('#ModalGrupoFamiliar_RPP').dialog('open');
        $('#ModGruFamApellidoPaterno_RPP').focus();
    });

    /* Botón Cancelar */
    $('#ModGruFamCancelar_RPP').live('click', function () {
        $('#ModalGrupoFamiliar_RPP').dialog('close');
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
                    numSolicitud: $('#ModGruFamsoltud').val(),
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
                    numSolicitud: $('#ModGruFamsoltud').val(),
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
                        $('#ModalGrupoFamiliar_RPP').dialog('close');

                        /* Recargar la grilla de personas vinculadas */
                        CargarTablaPersonasVinculadas_RP($('#ModGruFamsoltud').val())

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

    /* Botón Modificar */
    $('#TablaPersonasVinculadasPEPContenedor_RP .grilla_editar').live('click', function () {
        $('#ModGruFamModo_RPP').val('M');

        LimpiarFormularioPersonasVinculadas();

        $('#ModGruFamCargando_RPP').show();
        $('#ModalGrupoFamiliar_RPP').dialog('open');

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
                $('#ModGruFamCargando_RPP').fadeOut();
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
                $('#ModalGrupoFamiliar_RPP').dialog('close');
            }
        });
    });

    /* Botón Eliminar */
    $("#TablaPersonasVinculadasPEPContenedor_RP .grilla_eliminar").live("click", function () {
        $("#ModGruFamModo_RPP").val("EliminarPersonaVinculada");
        $("#HidPersonaVinculada").val($(this).data("idpersonavinculada"));

        $("#MCAIcono").attr("class", "advertencia");
        var mensaje = "<p>¿Confima que desea eliminar a <b>" + $(this).data("nombrepersonavinculada") + "</b> de la lista de Personas Vinculadas?</p>";
        $("#MCAContenedor").html(mensaje);
        $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
        $("#ModalCuadroAdvertencia").dialog("open");
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

        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CrearDatosSolicitud',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            success: function (data) {
                /* Solicitud Creada */
                Solicitud = data.d;
                $('#ModSolFecUltActualizacion').val(new Date(+Solicitud.FechaSolicitud.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));

                //CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>

                CargarTablaBeneficiarios_RP(null);

                $("#ModSolTipoCambioPanel").hide();




                //////<INIGTI_753>
                ////var valida = false;
                ////for (i = 1; i < $("#TabBeneficiarios_RP tbody tr").length; i++) {
                ////    if ($("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").is(":checked")) {
                ////        var idParentesco = $("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").data("parentesco");
                ////        if (idParentesco == "10") {
                ////            valida = true;
                ////        } else {
                ////            valida = false;
                ////        }

                ////        if (valida == false) {
                ////            break;
                ////        }
                ////    }
                ////}

                ////if (valida == true) {
                ////    $("#HConyuge").val("TRUE");
                ////} else {
                ////    for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                ////        Solicitud.Cotizaciones[i].ValPjeConyuge = "0";
                ////    }
                ////    $("#HConyuge").val("FALSE");
                ////}


                //<FINGTI_753>





                /* Cargar Tabla de Cotizaciones con las cotizaciones iniciales */
                //var params = {
                //	idTipoPension: $('#ModSolTipoPension').val()
                //}
                //$.ajax({
                //	type: 'POST',
                //	url: 'Cotizador.aspx/CargarComboProductos',
                //	contentType: "application/json; charset=iso-8859-1",
                //	dataType: 'json',
                //	data: $.toJSON(params),
                //	success: function (data) {
                //		CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud_RP').val());
                //		CargarTablaBeneficiarios_RP(null);

                //		$('#ModSolCargando').fadeOut();
                //		$('#ModSolTipoCambio').focus();
                //	},
                //	error: function (XMLHttpRequest, textStatus, errorThrown) {
                //		if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                //			/* Sesión caducada */
                //			document.location.reload(true);
                //		}
                //		else {
                //			$('#ModalSolicitud').dialog('close');

                //			$('#MCMIcono').attr('class', 'error');
                //			$('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de productos.');
                //			$('#ModalCuadroMensaje').dialog({ title: 'Error' });
                //			$('#ModalCuadroMensaje').dialog('open');
                //		}
                //	}
                //});
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

    /* CargandoSolicitud  */
    function CargandoSolicitud() {

        //$('#ModSolModo').val('M');
        $('#BeneficiariosOriginales_RP').show();
        $('#ManSolPestanhas li:eq(0)').trigger('click');

        //$('#ModSolCargando').show();

        //var fecCotizacion = $("#ModSolFecCotizacion_RP").val();
        //idSolicitud = $("#ModSolNroSolicitud_RP").val();
        //LimpiarFormularioSolicitud();

        $('#ManSolTipoSolicitud_RP').val($(this).parent('td').parent('tr').children().eq(3).find('span').html());

        botonModSolAceptarBloqueado = false;
        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');

        var params = {
            solicitud: $("#ModSolNroSolicitud_RP").val()
        }
        $.ajax({
            type: 'POST',
            url: 'MantenerSolicitud.aspx/ObtenerCotizacionesBeneficiarios',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                Solicitud = data.d;

                //<INIGTI_753>
                // Beneficiarios cotizados
                CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);

                // Grupo familiar
                CargarTablaBeneficiarios_RP(null);

                //<FINGTI_753>

                // Lista de cotizaciones
                //<INIGTI_7012>
                if ($('#ModSolModo').val() == "CERRAR") {
                    CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());
                } else {
                    CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
                }
                //<FINGTI_7012>

                //<INIGTI_753_3>
                if ($("#ModSolMonedaPrimaUnica_RP").val() == "001") {
                    $("#ModSolTipoCambioPanel").hide();
                } else {
                    $("#ModSolTipoCambioPanel").show();
                };
                //<FINGTI_753_3>

                $('#ModSolSaldoCIC_RP').focus();
                $('#ModSolDCOM_RP').focus();
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

    /* Botón Modificar */
    //$('#TabSolicitudes_RP .grilla_editar').live('click', function () {

    //    var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
    //    idSolicitud = $(this).data('solicitud');
    //    var params = {
    //        idSolicitud: idSolicitud,
    //        fecCotizacion: fecCotizacion,
    //        accion: 'M'
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
    //                window.location.href = "MantenerSolicitud.aspx";
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

    //<INIGTI_753_3>

    /*Botón copiar*/
    //$('#TabSolicitudes_RP .grilla_copiar').live('click', function () {

    //    var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
    //    idSolicitud = $(this).data('solicitud');
    //    var params = {
    //        idSolicitud: idSolicitud,
    //        fecCotizacion: fecCotizacion,
    //        accion: 'C'
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
    //                window.location.href = "MantenerSolicitud.aspx";
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


    /*Botón Consultar*/
    //$('#TabSolicitudes_RP .grilla_consultar').live('click', function () {

    //    var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
    //    idSolicitud = $(this).data('solicitud');
    //    var params = {
    //        idSolicitud: idSolicitud,
    //        fecCotizacion: fecCotizacion,
    //        accion: 'CONS'
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
    //                window.location.href = "MantenerSolicitud.aspx";
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

        var validacion = false;

        if ($('#EstadoCivil_RP').val() == "0") {
            $('#MCATablaEliminar').val('cierre');
            $("#MCAIcono").attr("class", "advertencia");
            $("#MCAContenedor").html("Debe seleccionar el Estado Civil del Afiliado en la pestaña 'Datos del Afiliado'");
            $("#ModalCuadroAdvertencia").dialog({ title: "Validación" });
            $("#ModalCuadroAdvertencia").dialog("open");
            validacion = true;
        }

        if ($(this).data('estado') == 0) {
            if ($('#HConsentimiento').val() != "OK") {
                $('#MCATablaEliminar').val('cierre');
                $("#MCAIcono").attr("class", "advertencia");
                $("#MCAContenedor").html("No se puede cerrar la solicitud " + $(this).data('solicitud') + " porque el cliente no ha brindado su consentimiento para el tratamiento de datos personales, puede enviarle en enlace de consentimiento en la pestaña de Datos del Afiliado");
                $("#ModalCuadroAdvertencia").dialog({ title: "Validación" });
                $("#ModalCuadroAdvertencia").dialog("open");
                $("#ModalCotizando").dialog("close");
                validacion = true;
            }
        }

        if (validacion == false) {
            //window.location.href = 'GrupoFamiliarAfiliadoCierre.aspx';
            var idSolicitud = $(this).data('solicitud');

            var params = {
                tokenUsuario: $("#TokenUsuario").val(),
                num_solicitud: idSolicitud
            };

            //1
            idSolicitud = $(this).data('solicitud');
            fecSoltud = $(this).parent('td').parent('tr').children().eq(1).html();
            estadoSoltud = $(this).data('estado');

            cantidad = 0;

            //
            $.ajax({
                type: "POST",
                url: "Cotizador.aspx/ValidarVigencia",
                contentType: "application/json; charset=iso-8859-1",
                data: $.toJSON(params),
                dataType: "json",
                success: function (data) {
                    //
                    if (data.d.Estado == "OK") {

                        //<FIN.GTI_7012_20>
                        var params = {
                            idGrupoFamiliar: cantidad,
                            solitud: idSolicitud,
                            fecha: fecSoltud,
                            estado: estadoSoltud,
                            paginaLlamada: '../RentaPrivadaPlus/SeleccionSolicitud.aspx'
                        }
                        //<FIN.GTI_7012_20>

                        $("#HCotizado").val("1");

                        $.ajax({
                            type: 'POST',
                            url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/SessionIdGrupoFamiliar',
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: 'json',
                            //data: "{idGrupoFamiliar:'" + cantidad + "', solitud: '" + idSolicitud + "', fecha: '" + fecSoltud + "', estado: '" + estadoSoltud + "'}",
                            data: $.toJSON(params),
                            success: function (data) {
                                //<INI.GTI_7012_20>
                                if (estadoSoltud != 3 && estadoSoltud != 7) {
                                    //<FIN.GTI_7012_20>
                                    $.ajax({
                                        type: 'POST',
                                        url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerListaGruposFamiliares',
                                        contentType: "application/json; charset=iso-8859-1",
                                        dataType: 'json',
                                        data: "{idSolicitud:'" + idSolicitud + "', posicion: '0'}",

                                        success: function (data) {
                                            window.location.href = "../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx";
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
                    else if (data.d.Estado == "ERROR") {
                        $("#MCMIcono").attr("class", data.d.Icono);
                        $("#MCMContenedor").html(data.d.Mensaje);
                        $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                        $("#ModalCuadroMensaje").dialog("open");
                    }
                    else if (data.d.Estado == "TOKEN") {
                        CerrarSesionExpirada();
                    }
                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //
                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                        /* Sesión caducada */
                        document.location.reload(true);
                    }

                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');

                },
                complete: function () {
                    $("#ModalPensionProyectadaCargando").dialog("close");
                }
            });

        }

    });
    //<FINGTI_7012>

    //<INIGTI_7012>
    /* Cargando grupo familiar Cierre*/
    function CargandoGrupoFamiliarCierre() {
        $('#ModGruFamModo_RP').val('M');

        LimpiarFormularioGrupoFamiliar();

        idGrupoFamiliar = $("#ModIdGrupoFamiliar").val();

        var params = {
            idGrupoFamiliar: idGrupoFamiliar
        }

        $.ajax({
            type: 'POST',
            url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerDatosBenefiCierre',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                var grupo = data.d;
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
                $('#ModGruFamNumeroIdentificacion_RP').val(grupo.Identificacion.Numero);
                // Parentesco
                $('#ModGruFamParentesco_RP').val(grupo.Parentesco.Id);
                $('#TexModGruFamParentesco_RP').html($('#ModGruFamParentesco_RP').find(':selected').text());

                //if (grupo.Parentesco.Id == '80') {
                $('#ModGruFamParentesco_RP').attr('disabled', 'disabled');
                $('#ConModGruFamParentesco_RP').addClass('formComboboxReadOnlyContenedor');
                //}
                //else {
                //    $('#ModGruFamParentesco_RP').removeAttr('disabled');
                //    $('#ConModGruFamParentesco_RP').removeClass('formComboboxReadOnlyContenedor');
                //}

                // Sexo
                $('#ModGruFamSexo_RP').val(grupo.Sexo);
                $('#TexModGruFamSexo_RP').html($('#ModGruFamSexo_RP').find(':selected').text());
                //if (grupo.Parentesco.Id == '80') {
                $('#ModGruFamSexo_RP').attr('disabled', 'disabled');
                $('#ConModGruFamSexo_RP').addClass('formComboboxReadOnlyContenedor');
                //}
                //else {
                //    $('#ModGruFamSexo_RP').removeAttr('disabled');
                //    $('#ConModGruFamSexo_RP').removeClass('formComboboxReadOnlyContenedor');
                //}

                // Fecha de Nacimiento
                $('#ModGruFamFechaNacimiento_RP').val(new Date(+grupo.FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModGruFamFechaNacimiento_RP').attr('disabled', 'disabled');
                //$('#ConModGruFamFechaNacimiento_RP').addClass('formComboboxReadOnlyContenedor');
                $('#ModGruFamFechaNacimiento_RP').addClass('formCalendarReadOnly');

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
                //SujetoObligado
                $('#ModGruFamSO_RP').val(grupo.ind_SujetoObligado ? 'S' : 'N');
                $('#TexModGruFamSO_RP').html($('#ModGruFamSO_RP').find(':selected').text());

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
                        url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: "{banco: '" + $("#ModGruFamBanco_RP").val() + "', id: ''}",
                        success: function (data) {
                            var items = "";
                            for (var posicion = 0; posicion < data.d.length; posicion++) {
                                items += "<option value='" + data.d[posicion].Id + "'>" + data.d[posicion].Nombre + "</option>";
                                //<INI.GTI_7012_V13>
                                if (grupo.TipoCtaBanco.Id == data.d[posicion].Id) {
                                    $('#ModGruFamNumeroBanco_RP').mask(data.d[posicion].Valor_2);
                                    $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                                }
                                //<FIN.GTI_7012_V13>
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

                    //if (grupo.Banco.Id == "BCP") {
                    //    if (grupo.NumeroBanco.length == 16) {
                    //        $('#TexModGruFamBanco_RP').html($('#ModGruFamBanco_RP option:eq(5)').text());
                    //        $('#ModGruFamBanco_RP option:contains("MAESTRA")').attr("selected", true);
                    //    }
                    //    else {
                    //        $('#TexModGruFamBanco_RP').html($('#ModGruFamBanco_RP option:eq(4)').text());
                    //        $('#ModGruFamBanco_RP option:contains("AHORROS")').attr("selected", true);
                    //    }
                    //}
                    //else {
                    //    $('#TexModGruFamBanco_RP').html($('#ModGruFamBanco_RP').find(':selected').text());
                    //}

                    //Confidencialidaddatos
                    $('#ModGruFamConfidencialidadDatos_RP').val(grupo.Confidencialidaddatos.Id);
                    $('#TexModGruFamConfidencialidadDatos_RP').html($('#ModGruFamConfidencialidadDatos_RP').find(':selected').text());

                    if (grupo.Parentesco.Id == '80') {

                        //$('#ModGruFamEstadoCivil_RP').val($('#HestadoCivil').val());
                        if (grupo.estadoCivil != null) {
                            if (grupo.estadoCivil.length > 0 && grupo.estadoCivil != 0) {
                                $('#ModGruFamEstadoCivil_RP').val(grupo.estadoCivil);
                            }
                        }
                        $('#TexModGruFamEstadoCivil_RP').html($('#ModGruFamEstadoCivil_RP').find(':selected').text());

                        //$('#ModGruFamMail_RP').val($('#HcorreoElectronico').val());
                        if (grupo.CorreoElectronico != null) {
                            if (grupo.CorreoElectronico.length > 0) {
                                $('#ModGruFamMail_RP').val(grupo.CorreoElectronico);
                            }
                        }

                        //$('#ModGruFamCentrolaboral_RP').val($('#HinstitucionLaboral').val());
                        if (grupo.centroLaboral != null) {
                            if (grupo.centroLaboral.length > 0) {
                                $('#ModGruFamCentrolaboral_RP').val(grupo.centroLaboral);
                            }
                        }

                        //$('#ModGruFamCargo_RP').val($('#Hcargo').val());
                        if (grupo.cargo != null) {
                            if (grupo.cargo.length > 0) {
                                $('#ModGruFamCargo_RP').val(grupo.cargo);
                            }
                        }

                        //$('#ModGruFamActividadeconomica_RP').val($('#HactividadEconomica').val());
                        if (grupo.actividadEconomica != null) {
                            if (grupo.actividadEconomica.length > 0) {
                                $('#ModGruFamActividadeconomica_RP').val(grupo.actividadEconomica);
                            }
                        }

                        //$('#ModGruFamMonedaingreso_RP').val($('#HmonedaIngreso').val());
                        if (grupo.monedaIngreso != null) {
                            if (grupo.monedaIngreso.Id.length > 0 && grupo.monedaIngreso.Id != 0) {
                                $('#ModGruFamMonedaingreso_RP').val(grupo.monedaIngreso.Id);
                            }
                        }
                        $('#TexModGruFamMonedaingreso_RP').html($('#ModGruFamMonedaingreso_RP').find(':selected').text());

                        $('#ModGruFamIngreso_RP').autoNumeric('init', { aSep: ',', aDec: '.' });
                        //$('#ModGruFamIngreso_RP').val($('#HingresoNetoMensual').val());
                        if (grupo.ingresoNeto != null) {
                            if (grupo.ingresoNeto > 0) {
                                $('#ModGruFamIngreso_RP').val(grupo.ingresoNeto);
                            }
                        }

                        $('.numerico').autoNumeric('init', { aSep: ',', aDec: '.' });

                        //$('#ModGruFamTelefono_RP').val($('#Htelefono').val());
                        if (grupo.telefono != null) {
                            if (grupo.telefono.length > 0) {
                                $('#ModGruFamTelefono_RP').val(grupo.telefono);
                            }
                        }

                        //$('#ModGruFamCelular_RP').val($('#Hcelular').val());
                        if (grupo.celular != null) {
                            if (grupo.celular.length > 0) {
                                $('#ModGruFamCelular_RP').val(grupo.celular);
                            }
                        }

                        if (grupo.OrigenFondo != null) {
                            if (grupo.OrigenFondo.declaracionJurada.length > 0) {
                                $('#ModGruFamDestinoFondos_RP').val(grupo.OrigenFondo.declaracionJurada);
                            }
                        }

                        $('#seccionPEP').hide();
                        if (grupo.ind_PEP) {
                            $('#seccionPEP').show();
                            CargarTablaPersonasVinculadas_RP($('#ModGruFamsoltud').val())
                        }

                    }

                }
                else {

                    //$('#ModGruFamComunicacion_RP').val(grupo.Comunicacion.Id);
                    $('#ModGruFamComunicacion_RP').val(1);
                    $('#TexModGruFamComunicacion_RP').html($('#ModGruFamComunicacion_RP').find(':selected').text());

                    $('#ModGruFamComunicacion_RP').attr('disabled', 'disabled');
                    $('#ConModGruFamComunicacion_RP').addClass('formComboboxReadOnlyContenedor');

                    $('#ModGruFamNumeroBanco_RP').val(grupo.NumeroBanco);
                    $('#ModGruFamBanco_RP').val(grupo.Banco.Id);

                    $.ajax({
                        type: 'POST',
                        url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
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

                    //INI GTI-26697
                    $('#ModGruFamLineaEstadocivilMail_RP').hide();
                    $('#ModGruFamLineaCentrolaboralCargo_RP').hide();
                    $('#ModGruFamLineaActividadEconomica_RP').hide();
                    $('#ModGruFamLineaMonedaingresoIngreso_RP').hide();
                    $('#ModGruFamLineaTelefonoCelular_RP').hide();
                    $('#ModFruFamLineaDestinoFondos').hide();
                    $('#ModFruFamLineaDestinoFondosText').hide();

                    $('#Contenido_LabModGruFamEstadoCivil_RP').hide();
                    $('#ConModGruFamEstadoCivil_RP').hide();

                    $('#seccionOrigenFondo').hide();
                    $('#seccionPEP').hide();
                    //FIN GTI-26697
                }

                //<FINGTI_7012>

                $('#pasos').append($('#ModGruFamCantidad').val() + " de " + $('#ModGruFamTotal').val())

                if ($('#ModGruFamCantidad').val() == 1) {
                    $('#ModGruFamCancelarCierre_RP').hide();
                }

                if ($('#ModGruFamCantidad').val() == $('#ModGruFamTotal').val()) {
                    $('#ModGruFamAceptarCierre_RP').html('Cerrar Solicitud');
                    //$("#ModGruFamAceptarCierre_RP").css("fontSize", 10);
                    //$("#ModGruFamAceptarCierre_RP").css('width', '120px').css('height', '22px');
                }

                $('#ModGruFamApellidoPaterno_RP').focus();
                $('#ModGruFamCargando_RP').fadeOut();

                $('#ModGruFamCancelarCierre_RP').removeClass('botonDeshabilitado gris gris_sharp');
                $('#ModGruFamAceptarCierre_RP').removeClass('botonDeshabilitado gris gris_sharp');

                $('#ModGruFamCancelarCierre_RP').addClass('boton darkblue sharp');
                $('#ModGruFamAceptarCierre_RP').addClass('boton darkblue sharp');

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
    //<FINGTI_7012>

    $('#TablaRviBenefiReintentar_RP').live('click', function () {
        $('#TablaRviBenefiError_RP').hide();
        CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);
    });

    /* Botón PDF */
    $("#TabSolicitudes_RP .grilla_pdf,#TabCotizacionCotizaciones .grilla_pdf").live("click", function () {

        var idSolicitud = $(this).data('solicitud');
        //var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
        var fecCotizacion = $(this).data('fechacotizacion');
        var numAgente = $(this).data("numagente");

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
                    //if (jQuery.browser.mobile) {
                    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                        window.location.href = "../Reportes/DetallePropuestaPlusMovil.aspx";
                    }
                    else {
                        var w = 800;
                        var h = 600;
                        var left = (screen.width / 2) - (w / 2);
                        var top = (screen.height / 2) - (h / 2);
                        //<SOLINI26593>
                        var nuevaVentana = window.open("../Reportes/DetallePropuestaPlus.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                        //<SOLFIN26593>
                        //var nuevaVentana = window.open("../Reportes/DetalleCotizacion.aspx", "", "width=800,height=600,scrollbars=1,location=no,menubar=no,resizable=1,status=no,toolbar=no");
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
                        window.open('../ArchivosTemporales/RPP/Poliza/' + data.d.Archivos[nombre], '_blank');
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

    /*<SRI.INI-20322>*/
    function DeslizarPantallaCorreo() {
        // Deslizar la pantalla hacia los datos de simulación
        //$('html,body').animate({
        //    scrollTop: $('#Simulaciones').offset().top
        //}, 'slow');
    }
    /*<SRI.FIN-20322>*/

    /*<SRI.INI-20322>*/
    /* Botón Correo Electrónico Que me Conviene*/
    $('#btnNuevoCorreoQueMeConviene').live('click', function () {

        if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

            $('#ModEnvCorCargandoSimulador').show();

            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                nroSolicitud: $.trim($('#BusAfiNroSolicitud_RP').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP_RP').val()),
                rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
            }

            LimpiarFormularioCorreo();

            DeslizarPantallaCorreo();

            $.ajax({
                type: 'POST',
                url: 'QueMeConviene.aspx/CrearDatosCorreo',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    /* Correo Creado */
                    Correo = data.d;
                    if (Correo.Respuesta.Estado == 'OK') {
                        $('#ModEnvCorDe').val(Correo.De);
                        $('#ModEnvCorPara').val(Correo.Para);
                        $('#ModEnvCorAsunto').val(Correo.Asunto);
                        $('#ModEnvCorAdjunto').html(Correo.Adjunto);
                        $('#ModEnvCorMensaje').val(Correo.Mensaje);

                        $('#ModEnvCorCargandoSimulador').fadeOut();
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

                        $('#ModalEnvioCorreoSimulador').dialog('close');
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
                    $('#ModalEnvioCorreoSimulador').dialog('close');
                }
            });

            $('#ModalEnvioCorreoSimulador').dialog('open');
        }
        else {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html('Error al guardar la Cotización para el envío');
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
        }
    });
    /*<SRI.FIN-20322>*/

    /*<SRI.INI-20322>*/
    /* Botón Correo Electrónico Simulador Jubilación Hoy o Futuro*/
    $('#btnNuevoCorreoJubiliarseHoyFuturo').live('click', function () {

        if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

            $('#ModEnvCorCargandoSimulador').show();

            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                nroSolicitud: $.trim($('#BusAfiNroSolicitud_RP').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP_RP').val()),
                rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
            }

            LimpiarFormularioCorreo();

            DeslizarPantallaCorreo();

            $.ajax({
                type: 'POST',
                url: 'JubilarseHoyFuturo.aspx/CrearDatosCorreo',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    /* Correo Creado */
                    Correo = data.d;
                    if (Correo.Respuesta.Estado == 'OK') {
                        $('#ModEnvCorDe').val(Correo.De);
                        $('#ModEnvCorPara').val(Correo.Para);
                        $('#ModEnvCorAsunto').val(Correo.Asunto);
                        $('#ModEnvCorAdjunto').html(Correo.Adjunto);
                        $('#ModEnvCorMensaje').val(Correo.Mensaje);

                        $('#ModEnvCorCargandoSimulador').fadeOut();
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

                        $('#ModalEnvioCorreoSimulador').dialog('close');
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
                    $('#ModalEnvioCorreoSimulador').dialog('close');
                }
            });

            $('#ModalEnvioCorreoSimulador').dialog('open');

        }
        else {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html('Error al guardar la Cotización para el envío');
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
        }
    });
    /*<SRI.FIN-20322>*/

    /*<SRI.INI-20322>*/
    /* Botón Correo Electrónico Simulador Renta Vitalicia Retiro Programado*/
    $('#btnNuevoCorreoRentaVitalicia').live('click', function () {

        if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

            $('#ModEnvCorCargandoSimulador').show();

            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                nroSolicitud: $.trim($('#BusAfiNroSolicitud_RP').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP_RP').val()),
                rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
            }

            LimpiarFormularioCorreo();

            DeslizarPantallaCorreo();

            $.ajax({
                type: 'POST',
                url: 'RentaVitaliciaRetiroProgramado.aspx/CrearDatosCorreo',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    /* Correo Creado */
                    Correo = data.d;
                    if (Correo.Respuesta.Estado == 'OK') {
                        $('#ModEnvCorDe').val(Correo.De);
                        $('#ModEnvCorPara').val(Correo.Para);
                        $('#ModEnvCorAsunto').val(Correo.Asunto);
                        $('#ModEnvCorAdjunto').html(Correo.Adjunto);
                        $('#ModEnvCorMensaje').val(Correo.Mensaje);

                        $('#ModEnvCorCargandoSimulador').fadeOut();
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

                        $('#ModalEnvioCorreoSimulador').dialog('close');
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
                    $('#ModalEnvioCorreoSimulador').dialog('close');
                }
            });

            $('#ModalEnvioCorreoSimulador').dialog('open');
        }
        else {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html('Error al guardar la Cotización para el envío');
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
        }
    });
    /*<SRI.FIN-20322>*/

    /* Botón Correo Electrónico Simulador Inmediata Diferida*/
    $('#btnNuevoCorreoInmediataDiferida').live('click', function () {

        if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

            $('#ModEnvCorCargandoSimulador').show();

            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                nroSolicitud: $.trim($('#BusAfiNroSolicitud_RP').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP_RP').val()),
                rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
            }

            LimpiarFormularioCorreo();

            DeslizarPantallaCorreo();

            $.ajax({
                type: 'POST',
                url: 'InmediataDiferida.aspx/CrearDatosCorreo',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    /* Correo Creado */
                    Correo = data.d;
                    if (Correo.Respuesta.Estado == 'OK') {
                        $('#ModEnvCorDe').val(Correo.De);
                        $('#ModEnvCorPara').val(Correo.Para);
                        $('#ModEnvCorAsunto').val(Correo.Asunto);
                        $('#ModEnvCorAdjunto').html(Correo.Adjunto);
                        $('#ModEnvCorMensaje').val(Correo.Mensaje);

                        $('#ModEnvCorCargandoSimulador').fadeOut();
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

                        $('#ModalEnvioCorreoSimulador').dialog('close');
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
                    $('#ModalEnvioCorreoSimulador').dialog('close');
                }
            });

            $('#ModalEnvioCorreoSimulador').dialog('open');
        }
        else {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html('Error al guardar la Cotización para el envío');
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
        }
    });
    /*<SRI.FIN-20322>*/

    /*<SRI.INI-20322>*/
    /* Botón Correo Electrónico Simulador Tipo Moneda*/
    $('#btnNuevoCorreoTipoMoneda').live('click', function () {

        if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

            $('#ModEnvCorCargandoSimulador').show();

            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                nroSolicitud: $.trim($('#BusAfiNroSolicitud_RP').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP_RP').val()),
                rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
            }

            LimpiarFormularioCorreo();

            DeslizarPantallaCorreo();

            $.ajax({
                type: 'POST',
                url: 'TipoMoneda.aspx/CrearDatosCorreo',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    /* Correo Creado */
                    Correo = data.d;
                    if (Correo.Respuesta.Estado == 'OK') {
                        $('#ModEnvCorDe').val(Correo.De);
                        $('#ModEnvCorPara').val(Correo.Para);
                        $('#ModEnvCorAsunto').val(Correo.Asunto);
                        $('#ModEnvCorAdjunto').html(Correo.Adjunto);
                        $('#ModEnvCorMensaje').val(Correo.Mensaje);

                        $('#ModEnvCorCargandoSimulador').fadeOut();
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

                        $('#ModalEnvioCorreoSimulador').dialog('close');
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
                    $('#ModalEnvioCorreoSimulador').dialog('close');
                }
            });

            $('#ModalEnvioCorreoSimulador').dialog('open');
        }
        else {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html('Error al guardar la Cotización para el envío');
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
        }
    });
    /*<SRI.FIN-20322>*/

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

    //<SRIINI06326>

    /*Begin Botón Cancelar ACOM */
    $('#MRVCancelarSeleccionAcom').live('click', function () {
        $('#ModalSeleccionAcom').dialog('close');
    });
    /*End Botón Cancelar ACOM */

    /*Begin Botón Aceptar ACOM*/
    $('#MRVGuardarSeleccionAcom').live('click', function () {

        $('#MGRIcono').attr('class', 'cargando');
        $('#MGRContenedor').html('Generando el reporte de escenarios, por favor espere un momento...');
        $('#ModalGenerandoReporte').dialog({ title: 'Generando' });
        $('#ModalGenerandoReporte').dialog('open');

        var maxAcom = $('input[type="radio"]:checked').val();

        //var idSolicitud = $(this).data('solicitud');
        //var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            idSolicitud: idSolicitudTmp,
            fecCotizacion: fechaCotizacionTmp,
            maxAcom: maxAcom
        }

        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/ExportarReporteEscenariosPDF',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                        window.location.href = "../Reportes/EscenariosMovil.aspx";
                    }
                    else {
                        var w = 800;
                        var h = 600;
                        var left = (screen.width / 2) - (w / 2);
                        var top = (screen.height / 2) - (h / 2);

                        $('#ModalGenerandoReporte').dialog('close');
                        var nuevaVentana = window.open("../Reportes/Escenarios.aspx", "_blank", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                    }

                    /*<SRIINI17003>*/
                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                    /*<SRIFIN17003>*/
                }
                else if (data.d.Estado == "ERROR") {
                    $('#ModalGenerandoReporte').dialog('close');

                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog('open');

                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#ModalGenerandoReporte').dialog('close');

                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al generar el reporte de escenarios.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });
    /*Begin Botón Aceptar ACOM*/

    /*Begin Seleccion ACOM*/
    $('#TabSeleccionAcom input[type=radio]').live('change', function () {
        //
        var radio = $(this);
        var filaPadre = radio.parent().parent();

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        var clasePadre = filaPadre.attr('class');
        setTimeout(function () {
            $("#TabSeleccionAcom tbody tr").removeClass("grilla_active");
            $("#TabSeleccionAcom tbody tr:even").addClass("grilla_alt1");
            $("#TabSeleccionAcom tbody tr:odd").addClass("grilla_alt2");

            filaPadre.toggleClass("grilla_active", radio.is(":checked"));
        }, 0);
    });
    /*End Seleccion ACOM*/

    /*Begin Cargar Acom */
    $('#TabSolicitudes_RP .grilla_rep_escenario').live('click', function () {

        //if ($('#hdKeyAcom').val() == 'S') {

        var idSolicitud = $(this).data('solicitud');
        var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
        var numAgente = $(this).data('numagente');

        idSolicitudTmp = idSolicitud;
        fechaCotizacionTmp = fecCotizacion;

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            idSolicitud: idSolicitud,
            fecCotizacion: fecCotizacion,
            numAgente: numAgente
        }

        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarRolEscenario',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d != 'TOKEN') {
                    $('#TablaSeleccionAcomCargando').hide();
                    $('#TablaSeleccionAcomContenedor').show();
                    $('#TablaSeleccionAcomContenedor').html($(data.d).find('#ContenidoDinamico').html());
                    $('#ModalSeleccionAcom').dialog({ title: 'Acom - Escenario' });
                    $('#ModalSeleccionAcom').dialog('open');

                    $('#TabSeleccionAcom tbody').each(function () {
                        $(this).find('tr').last().addClass("grilla_active");
                    });

                    var rowCount = $('#TabSeleccionAcom tr').length - 1;
                    $('#rad_' + rowCount).attr('checked', true);
                }
                else {
                    CerrarSesionExpirada();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    //Sesión caducada /
                    document.location.reload(true);
                }
                else {
                    $('#TablaSeleccionAcomCargando').hide();
                    $('#DivErrorAcom').show();
                }
            }
        });


        /* else {
    
             $('#MGRIcono').attr('class', 'cargando');
             $('#MGRContenedor').html('Generando el reporte de escenarios, por favor espere un momento...');
             $('#ModalGenerandoReporte').dialog({ title: 'Generando' });
             $('#ModalGenerandoReporte').dialog('open');
    
             var idSolicitud = $(this).data('solicitud');
             var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
    
             var params = {
                 tokenUsuario: $('#TokenUsuario').val(),
                 idSolicitud: idSolicitud,
                 fecCotizacion: fecCotizacion,
                 maxAcom: ''
             }
    
             $.ajax({
                 type: 'POST',
                 url: 'Cotizador.aspx/ExportarReporteEscenariosPDF',
                 contentType: "application/json; charset=iso-8859-1",
                 dataType: 'json',
                 data: $.toJSON(params),
                 success: function (data) {
                     if (data.d.Estado == 'OK') {
                         if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                             window.location.href = "../Reportes/EscenariosMovil.aspx";
                         }
                         else {
                             var w = 800;
                             var h = 600;
                             var left = (screen.width / 2) - (w / 2);
                             var top = (screen.height / 2) - (h / 2);
    
                             $('#ModalGenerandoReporte').dialog('close');
                             var nuevaVentana = window.open("../Reportes/Escenarios.aspx", "_blank", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                         }
                     }
                     else if (data.d.Estado == "ERROR") {
                         $('#ModalGenerandoReporte').dialog('close');
    
                         $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                         $('#MCMIcono').attr('class', data.d.Icono);
                         $('#MCMContenedor').html(data.d.Mensaje);
                         $('#ModalCuadroMensaje').dialog('open');
    
                     }
                     else if (data.d.Estado == "TOKEN") {
                         CerrarSesionExpirada();
                     }
                 },
                 error: function (XMLHttpRequest, textStatus, errorThrown) {
                     if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                         document.location.reload(true);
                     }
                     else {
                         $('#ModalGenerandoReporte').dialog('close');
    
                         $('#MCMIcono').attr('class', 'error');
                         $('#MCMContenedor').html('Ha ocurrido un error al generar el reporte de escenarios.');
                         $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                         $('#ModalCuadroMensaje').dialog('open');
                     }
                 }
             });
         }*/
    });
    /*End Cargar Acom */

    /*<<SRIFIN06326>>*/

    /*<SRI.INI-20322>*/
    /* Botón Enviar Correo Electrónico Simulador Jubilación Hoy o Futuro*/
    $('#ModEnvCorEnviarSimuladorJubilarseHoyFuturo').live('click', function () {
        $('#ModEnvCorCargandoSimulador').fadeIn();

        Correo.Mensaje = $('#ModEnvCorMensaje').val();
        Correo.Respuesta = null;

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            correo: Correo
        }
        $.ajax({
            type: 'POST',
            url: 'JubilarseHoyFuturo.aspx/EnviarCorreoElectronicoSimulador',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    $('#ModalEnvioCorreoSimulador').dialog('close');

                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
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

                    $('#ModEnvCorCargandoSimulador').fadeOut();
                }
            }
        });
    });
    /*<SRI.INI-20322>*/

    /*<SRI.INI-20322>*/
    /* Botón Enviar Correo Electrónico Simulador Renta Vitalicia*/
    $('#ModEnvCorEnviarSimuladorRentaVitalicia').live('click', function () {
        $('#ModEnvCorCargandoSimulador').fadeIn();

        Correo.Mensaje = $('#ModEnvCorMensaje').val();
        Correo.Respuesta = null;

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            correo: Correo
        }
        $.ajax({
            type: 'POST',
            url: 'RentaVitaliciaRetiroProgramado.aspx/EnviarCorreoElectronicoSimulador',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    $('#ModalEnvioCorreoSimulador').dialog('close');

                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
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

                    $('#ModEnvCorCargandoSimulador').fadeOut();
                }
            }
        });
    });
    /*<SRI.INI-20322>*/

    /*<SRI.INI-20322>*/
    /* Botón Enviar Correo Electrónico Simulador Inmediata Diferida*/
    $('#ModEnvCorEnviarSimuladorInmediataDiferida').live('click', function () {
        $('#ModEnvCorCargandoSimulador').fadeIn();

        Correo.Mensaje = $('#ModEnvCorMensaje').val();
        Correo.Respuesta = null;

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            correo: Correo
        }
        $.ajax({
            type: 'POST',
            url: 'InmediataDiferida.aspx/EnviarCorreoElectronicoSimulador',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    $('#ModalEnvioCorreoSimulador').dialog('close');

                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
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

                    $('#ModEnvCorCargandoSimulador').fadeOut();
                }
            }
        });
    });
    /*<SRI.INI-20322>*/

    /*<SRI.INI-20322>*/
    /* Botón Enviar Correo Electrónico Simulador Tipo Moneda*/
    $('#ModEnvCorEnviarSimuladorTipoMoneda').live('click', function () {
        $('#ModEnvCorCargandoSimulador').fadeIn();

        Correo.Mensaje = $('#ModEnvCorMensaje').val();
        Correo.Respuesta = null;

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            correo: Correo
        }
        $.ajax({
            type: 'POST',
            url: 'TipoMoneda.aspx/EnviarCorreoElectronicoSimulador',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    $('#ModalEnvioCorreoSimulador').dialog('close');

                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
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

                    $('#ModEnvCorCargandoSimulador').fadeOut();
                }
            }
        });
    });
    /*<SRI.INI-20322>*/

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
            //tokenUsuario: $('#TokenUsuario').val(),
            correo: Correo,
            agente: codigoAgente
        }
        fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/enviar-correo-electronico', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=iso-8859-1',
                'x-username': $('#usuario_actual').val(),
                'x-rol': $('#rol_azman').val()
            },
            body: JSON.stringify(params)
        })
            .then(response => {
                if (!response.ok) {
                    if (response.status === 401 || response.status === 12030) {
                        document.location.reload(true);
                        return;
                    }
                    throw new Error('Error en la respuesta del servidor');
                }
                return response.json();
            })
            .then(data => {
                if (data.data.Respuesta.Estado == 'OK') {
                    $('#ModalEnvioCorreo').dialog('close');

                    $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                    $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    /*<SRIINI17003>*/
                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                    /*<SRIFIN17003>*/
                }
                else if (data.data.Respuesta.Estado == 'TOKEN') {
                    CerrarSesionExpirada();
                }
                else {
                    $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                    $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            })
            .catch(error => {
                if (error.status === 401 || error.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');

                    $('#ModEnvCorCargando').fadeOut();
                }
            });
        //$.ajax({
        //    type: 'POST',
        //    url: 'Cotizador.aspx/EnviarCorreoElectronico',
        //    contentType: "application/json; charset=iso-8859-1",
        //    dataType: 'json',
        //    data: $.toJSON(params),
        //    success: function (data) {
        //        if (data.d.Estado == 'OK') {
        //            $('#ModalEnvioCorreo').dialog('close');

        //            $('#MCMIcono').attr('class', data.d.Icono);
        //            $('#MCMContenedor').html(data.d.Mensaje);
        //            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
        //            $('#ModalCuadroMensaje').dialog('open');

        //            /*<SRIINI17003>*/
        //            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        //            $clock.countdown(selectedDate.toString());
        //            /*<SRIFIN17003>*/
        //        }
        //        else if (data.d.Estado == 'TOKEN') {
        //            CerrarSesionExpirada();
        //        }
        //        else {
        //            $('#MCMIcono').attr('class', data.d.Icono);
        //            $('#MCMContenedor').html(data.d.Mensaje);
        //            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
        //            $('#ModalCuadroMensaje').dialog('open');
        //        }
        //    },
        //    error: function (XMLHttpRequest, textStatus, errorThrown) {
        //        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //            document.location.reload(true);
        //        }
        //        else {
        //            $('#MCMIcono').attr('class', 'error');
        //            $('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
        //            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        //            $('#ModalCuadroMensaje').dialog('open');

        //            $('#ModEnvCorCargando').fadeOut();
        //        }
        //    }
        //});
    });

    /* Botón Cancelar Correo Electrónico */
    $('#ModEnvCorCancelar').live('click', function () {
        $('#ModalEnvioCorreo').dialog('close');
        //<SRI.INI-20322>
        $('#ModalEnvioCorreoSimulador').dialog('close');
        //<SRI.FIN-20322>
    });

    /* Cambiar combobox Tipo Pensión */
    $("#ModSolTemporalidad_RP").live('change', function () {
        var params = {
            idTemporalidad: $(this).val()
        }
        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/CargarComboPeriodoGarantizado",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {

                if ($('#ModSolTemporalidad_RP').val() == "TVT") {
                    for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                        Solicitud.Cotizaciones[i].ValPjeDev = "0";
                    }
                }

                CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de productos.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });

    /* Botón Agregar */
    $('#TabCotizacionesAgregar_RP').live('click', function () {
        var params = {
            cot: Solicitud.Cotizaciones
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/AgregarCotizacionASolicitud',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                Solicitud.Cotizaciones = data.d;
                CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>

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
                    $('#MCMContenedor').html('Ha ocurrido un error al agregar una nueva cotización.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });

    /* Cambiar combobox Tabla Cotizaciones */
    $('#TabCotizaciones_RP select').live('change', function () {

        if ($('#ModSolModo').val() != "CONS") {

            var celda = $(this).parent('td');
            var col = celda.parent('tr').children().index(celda);

            var fila = celda.parent('tr');
            var fil = fila.parent('tbody').children().index(fila);

            switch (col) {
                case 1: //Moneda
                    Solicitud.Cotizaciones[fil].Moneda.Id = $(this).val();
                    ObtenerMonedaAjuste($(this).val(), fila.find('.ModSolAjusteMoneda'), fil);
                    break;
                case 2: //Moneda Ajuste
                    Solicitud.Cotizaciones[fil].ValMonAju = $(this).val();
                    break;
                case 4: //Pago Escalonada
                    Solicitud.Cotizaciones[fil].PagoEscalonada = $(this).val();

                    if ($(this).val() == 0) {
                        Solicitud.Cotizaciones[fil].PjePE = 0;

                        fila.find('.ModSolPjePagoEscalonada').attr('disabled', 'disabled');
                        fila.find('.ModSolPjePagoEscalonada').append(new Option("100%", "0"));
                        fila.find('.ModSolPjePagoEscalonada').val('0');
                    } else {
                        fila.find('.ModSolPjePagoEscalonada').removeAttr('disabled', 'disabled');
                        fila.find('.ModSolPjePagoEscalonada').find('[value="0"]').remove();

                        var pjeCalculo = ((100 / fila.find('.ModSolPjePagoEscalonada').val()) * 100).toFixed(6)
                        Solicitud.Cotizaciones[fil].PjePE = pjeCalculo;
                    }
                    break;

                case 5: //Pje PE
                    if ($(this).val() == 0) {
                        Solicitud.Cotizaciones[fil].PjePE = $(this).val();
                    } else {
                        Solicitud.Cotizaciones[fil].PjePE = ((100 / $(this).val()) * 100).toFixed(6);
                    }

                    break;
                case 6: //Pje Devolucion
                    Solicitud.Cotizaciones[fil].ValPjeDev = $(this).val();
                    if ($('#ModSolTemporalidad_RP').val() == "TVT") {
                        Solicitud.Cotizaciones[fil].ValPjeDev = "0";
                        $(this).val("0");
                        $(this).attr("disabled", "true");
                    }
                    break;
                case 7: //Período garantizado
                    Solicitud.Cotizaciones[fil].PeriodoGarantizado = $(this).val();

                    if ('T' + pad($(this).val(), 2) == $('#ModSolTemporalidad_RP').val()) {
                        Solicitud.Cotizaciones[fil].IndGastoSepelio = "S";
                        fila.find('#ChkSepelio').attr('disabled', 'disabled');
                        fila.find('#ChkSepelio').attr('checked', 'checked');

                        fila.find('.ModSolPjeDevolucion').removeAttr('disabled', 'disabled');

                        if (($('#ModSolTemporalidad_RP').val() == "T05" || $('#ModSolTemporalidad_RP').val() == "T07")
                            && (Solicitud.Cotizaciones[fil].Moneda.Id == "001" || Solicitud.Cotizaciones[fil].Moneda.Id == "013")
                        ) {

                            if (Solicitud.Cotizaciones[fil].ValPjeDev < 50) {
                                Solicitud.Cotizaciones[fil].ValPjeDev = "50";
                                fila.find('.ModSolPjeDevolucion').val('50');
                            }

                            $(".ModSolPjeDevolucion option[value=0]").attr('disabled', 'disabled');
                        }

                    } else {
                        fila.find('#ChkSepelio').removeAttr('disabled', 'disabled');

                        fila.find('.ModSolPjeDevolucion').val('0');
                        fila.find('.ModSolPjeDevolucion').attr('disabled', 'disabled');
                        Solicitud.Cotizaciones[fil].ValPjeDev = "0";
                    }
                    break;
                case 8: //Porcentaje Conyuge
                    Solicitud.Cotizaciones[fil].ValPjeConyuge = $(this).val();
                    break;
            }
        }
    });

    /**/
    //<INI.GTI_7012>
    function pad(str, max) {
        str = str.toString();
        return str.length < max ? pad("0" + str, max) : str;
    }
    //<FIN.GTI_7012>

    /* Modificar el Ajuste TRA en la Tabla de Cotizaciones */
    $('#TabCotizaciones_RP input[type=text]').live('keyup', function () {
        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);
        Solicitud.Cotizaciones[fil].AjusteTRA = $(this).val();
    });

    /* Modificar el check de sepelio IndGastoSepelio*/
    $('#TabCotizaciones_RP input[type=checkbox]').live('change', function () {
        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);
        if (this.checked) {
            Solicitud.Cotizaciones[fil].IndGastoSepelio = "S";
        } else {
            Solicitud.Cotizaciones[fil].IndGastoSepelio = "N";
        }

    });

    /* Botón Eliminar */
    $('#TabCotizaciones_RP .grilla_eliminar').live('click', function () {
        idCotizacion = $(this).data('cotizacion');
        $('#MCATablaEliminar').val('cot');
        $('#MCAIcono').attr('class', 'advertencia');
        $('#MCAContenedor').html('¿Confirma la eliminación de la cotización <strong>N° ' + idCotizacion + '</strong>?');
        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
        $('#ModalCuadroAdvertencia').dialog('open');
        return false;
    });

    /* Seleccionar beneficiario en tabla de beneficiarios */
    $('#TabBeneficiarios_RP input[type=checkbox]').live('change', function () {
        $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');
    });

    /* Mostrar montos cotizados */
    $('#TabCotizaciones_RP select,#TabCotizaciones_RP input[type=text]').live('focus', function () {
        var filaPadre = $(this).parent().parent();

        var celda = $(this).parent('td');
        var col = celda.parent('tr').children().index(celda);

        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        var radio = fila.find('input[type=radio]');
        radio.attr('checked', 'checked');

        var clasePadre = filaPadre.attr('class');
        setTimeout(function () {
            $("#TabCotizaciones_RP tbody tr").removeClass("grilla_active");
            $("#TabCotizaciones_RP tbody tr:even").addClass("grilla_alt1");
            $("#TabCotizaciones_RP tbody tr:odd").addClass("grilla_alt2");

            filaPadre.toggleClass("grilla_active", radio.is(":checked"));
        }, 0);

        //ARMV
        //////$('#ModSolMontoCIA').val(formatearMonto(Solicitud.Cotizaciones[fil].MontoCia.toFixed(2)));
        //////$('#ModSolPensionCIA').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionCia.toFixed(2)));
        //////$('#ModSolPensionCIAMO').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionCiaMO.toFixed(2)));
        //////$('#ModSolTasaAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaAFP.toFixed(2)));
        //////$('#ModSolMontoAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].MontoAFP.toFixed(2)));
        //////$('#ModSolPensionAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionAFP.toFixed(2)));
        ///////*<SRI.INI-20322>*/
        //////$('#ModTasaVenta').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaVenta.toFixed(2)));
        //////$('#ModTasaVentaSbs').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaVentaSbs.toFixed(2)));
        ///////*<SRI.FIN-20322>*/
    });

    /* Seleccionar cotización */
    //<INIGTI_7012>
    $('#TabCotizaciones_RP input[type=radio]').live('change', function () {
        var radio = $(this);
        var filaPadre = radio.parent().parent();

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        var clasePadre = filaPadre.attr('class');
        setTimeout(function () {
            $("#TabCotizaciones_RP tbody tr").removeClass("grilla_active");
            $("#TabCotizaciones_RP tbody tr:even").addClass("grilla_alt1");
            $("#TabCotizaciones_RP tbody tr:odd").addClass("grilla_alt2");

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
    //<FINGTI_7012>

    /* Botón Aceptar */
    $("#ModSolAceptar_RP").live("click", function () {

        if (!botonModSolAceptarBloqueado) {
            if ($("#ModSolModo").val() == "N") {
                var idBeneficiarios = new Array();

                // Obtener la lista de beneficiarios
                for (i = 0; i < $("#TabBeneficiarios_RP tbody tr").length; i++) {
                    if ($("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").is(":checked"))
                        idBeneficiarios.push(i);
                }

                //<INIGTI_753>
                if ($("#HConyuge").val() == "FALSE") {
                    for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                        Solicitud.Cotizaciones[i].ValPjeConyuge = "0";
                    }
                }
                //<FINGTI_753>

                $("#ModSolCargando").fadeIn();
                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Cotizando la solicitud, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Cotizando" });
                $("#ModalCotizando").dialog("open");

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    cuspp: $("#HCUSPP_RP").val(),
                    afp: $("#HAFP_RP").val(),
                    temporalidad: $("#ModSolTemporalidad_RP").val(),
                    monedaPrimaUnica: $("#ModSolMonedaPrimaUnica_RP").val(),
                    primaUnica: $("#ModSolPrimaUnica_RP").val(),
                    fechaCotizacion: $("#ModSolFechaCotizacion_RP").val(),
                    fechaDevengue: $("#ModSolFechaDevengue_RP").val(),
                    dcom: ($("#ModSolDCOM_RP").length > 0) ? $("#ModSolDCOM_RP").val() : "",
                    cotizaciones: Solicitud.Cotizaciones,
                    idBeneficiarios: idBeneficiarios,
                    tipoplan: $("#ModSolTipoPlan_RP").val()
                }

                $.ajax({
                    type: 'POST',
                    url: 'Cotizador.aspx/InsertarSolicitud',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Respuesta.Estado == 'OK') {
                            // Imprimir número de solicitud generada
                            $('#ModSolNroSolicitud_RP').val(data.d.Id);

                            // Cambiar la modal a modo de modificación
                            $('#ModSolModo').val('M');

                            // Cerrar modal de espera
                            $('#ModalCotizando').dialog('close');

                            // Mostrar mensaje de éxito
                            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                            $('#ModalCuadroMensaje').dialog('open');

                            //CargarTablaSolicitudes();

                            // Cargar la cotización actualizada
                            $('#BeneficiariosOriginales_RP').show();
                            //<INIGTI_753_3>
                            //$('#ManSolPestanhas li:eq(0)').trigger('click');
                            //<FINGTI_753_3>
                            $('#ModSolCargando').show();

                            var params3 = {
                                idSolicitud: $('#ModSolNroSolicitud_RP').val(),
                                fecCotizacion: $('#ModSolFechaCotizacion_RP').val()
                            }

                            LimpiarFormularioSolicitud();

                            //<INIGTI_753_3>

                            if ($("#HBloqueo").val() == "TRUE") {
                                botonModSolAceptarBloqueado = true;
                                $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');


                                $('#ModSolTemporalidad_RP').attr('disabled', 'disabled');
                                $('#ModSolMonedaPrimaUnica_RP').attr('disabled', 'disabled');
                                $('#ModSolTipoPlan_RP').attr('disabled', 'disabled');

                                $('#ModSolFechaCotizacion_RP').attr('disabled', 'disabled');
                                $('#ModSolFechaDevengue_RP').attr('disabled', 'disabled');
                                $('#ModSolFechaVigencia_RP').attr('disabled', 'disabled');

                                $('#ModSolDCOM_RP').attr('disabled', 'disabled');
                                $('#ModSolTipoCambio').attr('disabled', 'disabled');
                                $('#ModSolPrimaUnica_RP').attr('disabled', 'disabled');

                                ///////////////////////////////////////////
                                //ConModSolMonedaPrimaUnica_RP
                                //$('#ModSolTemporalidad_RP').attr('class', 'formCombobox formComboboxReadOnly');
                                //$('#ModSolMonedaPrimaUnica_RP').attr('class', 'formCombobox formComboboxReadOnly');
                                //$('#ModSolTipoPlan_RP').attr('class', 'formCombobox formComboboxReadOnly');

                                $('#ConModSolTemporalidad_RP').attr('disabled', 'disabled');
                                $('#ConModSolMonedaPrimaUnica_RP').attr('disabled', 'disabled');
                                $('#ConModSolTipoPlan_RP').attr('disabled', 'disabled');


                                $('#ModSolFechaCotizacion_RP').attr('class', 'fecha formTextbox formCalendar formTextboxReadOnly');
                                $('#ModSolFechaDevengue_RP').attr('class', 'fecha formTextbox formCalendar formTextboxReadOnly');
                                $('#ModSolFechaVigencia_RP').attr('class', 'fecha formTextbox formCalendar formTextboxReadOnly');



                                $('#ModSolDCOM_RP').attr('class', 'formTextbox formTextboxReadOnly');
                                $('#ModSolTipoCambio').attr('class', 'formTextbox formTextboxReadOnly');
                                $('#ModSolPrimaUnica_RP').attr('class', 'formTextbox formTextboxReadOnly');

                            } else {
                                botonModSolAceptarBloqueado = false;
                                $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                            }

                            //<FINGTI_753_3>

                            $.ajax({
                                type: 'POST',
                                url: 'Cotizador.aspx/ObtenerDatosSolicitud',
                                contentType: "application/json; charset=iso-8859-1",
                                data: $.toJSON(params3),
                                dataType: 'json',
                                success: function (data) {
                                    Solicitud = data.d;
                                    $('#ModSolNroSolicitud_RP').val(Solicitud.Id);
                                    $('#ModSolFechaDevengue_RP').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    $('#ModSolPrimaUnica_RP').val(formatearMonto(Solicitud.PrimaUnica));
                                    $('#ModSolFechaCotizacion_RP').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    $('#ModSolDCOM_RP').val(formatearMonto(Solicitud.PorcentajeDescuentoComision));

                                    //<INIGTI_753_3>
                                    $('#ModSolTipoCambio').val(formatearMonto(Solicitud.TipoCambio));

                                    if ($("#ModSolMonedaPrimaUnica_RP").val() == "001") {
                                        $("#ModSolTipoCambioPanel").hide();
                                    } else {
                                        $("#ModSolTipoCambioPanel").show();
                                    };
                                    //<FINGTI_753_3>
                                    //ModSolTipoCambioPanel

                                    //CargarTablaBeneficiarios_RP(null);
                                    CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);
                                    CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>



                                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                    $clock.countdown(selectedDate.toString());


                                    $('#ModSolCargando').fadeOut();
                                    $('#ModSolSaldoCIC_RP').focus();
                                    $('#ModSolACOM').focus();
                                    $('#ModSolDCOM_RP').focus();
                                    $('#ModSolTipoCambio').focus();


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
                            $('#ModalCotizando').dialog('close');

                            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                            if (data.d.Respuesta.Controles != null) {
                                if (data.d.Respuesta.Controles[0].length) $('#ModSolPrimaUnica_RP').attr('class', data.d.Respuesta.Controles[0]);
                                if (data.d.Respuesta.Controles[1].length) $('#ModSolFechaCotizacion_RP').attr('class', data.d.Respuesta.Controles[1]);
                                if (data.d.Respuesta.Controles[2].length) $('#ModSolFechaDevengue_RP').attr('class', data.d.Respuesta.Controles[2]);
                                if (data.d.Respuesta.Controles[3].length) $('#ModSolDCOM_RP').attr('class', data.d.Respuesta.Controles[3]);

                                $('#TabCotizaciones_RP select').removeClass('formTextboxGridError');
                                if (data.d.Respuesta.Controles.length > 4) {
                                    var celdaError;
                                    for (i = 4; i < data.d.Respuesta.Controles.length; i++) {
                                        celdaError = data.d.Respuesta.Controles[i].split(',');
                                        $('#TabCotizaciones_RP tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                    }
                                }
                            }
                            $('#ModalCuadroMensaje').dialog('open');
                            $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
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
                });
            }
            else if ($('#ModSolModo').val() == 'M') {
                var idBeneficiarios = new Array();

                // Obtener la lista de beneficiarios
                for (i = 0; i < $('#TabBeneficiarios_RP tbody tr').length; i++) {
                    if ($('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').is(':checked'))
                        idBeneficiarios.push(i);
                }
                //<INIGTI_753>
                if ($("#HConyuge").val() == "FALSE") {
                    for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                        Solicitud.Cotizaciones[i].ValPjeConyuge = "0";
                    }
                }
                //<FINGTI_753>
                //$('#ModSolCargando').fadeIn();
                $('#MCIcono').attr('class', 'cargando');
                $('#MCContenedor').html('Cotizando la solicitud, por favor espere un momento...');
                $('#ModalCotizando').dialog({ title: 'Cotizando' });
                $('#ModalCotizando').dialog('open');

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    idSolicitud: $('#ModSolNroSolicitud_RP').val(),
                    cuspp: $("#HCUSPP_RP").val(),
                    afp: $("#HAFP_RP").val(),
                    temporalidad: $("#ModSolTemporalidad_RP").val(),
                    monedaPrimaUnica: $("#ModSolMonedaPrimaUnica_RP").val(),
                    primaUnica: $("#ModSolPrimaUnica_RP").val(),
                    fechaCotizacion: $("#ModSolFechaCotizacion_RP").val(),
                    fechaDevengue: $("#ModSolFechaDevengue_RP").val(),
                    dcom: ($("#ModSolDCOM_RP").length > 0) ? $("#ModSolDCOM_RP").val() : "",
                    cotizaciones: Solicitud.Cotizaciones,
                    idBeneficiarios: idBeneficiarios,
                    tipoplan: $("#ModSolTipoPlan_RP").val()
                }

                $.ajax({
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
                            $('#BeneficiariosOriginales_RP').show();
                            //<INIGTI_753_3>
                            //$('#ManSolPestanhas li:eq(0)').trigger('click');
                            //<FINGTI_753_3>

                            $('#ModSolCargando').show();

                            var params3 = {
                                idSolicitud: $('#ModSolNroSolicitud_RP').val(),
                                fecCotizacion: $('#ModSolFechaCotizacion_RP').val()
                            }

                            LimpiarFormularioSolicitud();

                            $.ajax({
                                type: 'POST',
                                url: 'Cotizador.aspx/ObtenerDatosSolicitud',
                                contentType: "application/json; charset=iso-8859-1",
                                data: $.toJSON(params3),
                                dataType: 'json',
                                success: function (data) {
                                    Solicitud = data.d;
                                    $('#ModSolNroSolicitud_RP').val(Solicitud.Id);
                                    $('#ModSolFechaDevengue_RP').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    $('#ModSolPrimaUnica_RP').val(formatearMonto(Solicitud.PrimaUnica));
                                    $('#ModSolFechaCotizacion_RP').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                    $('#ModSolDCOM_RP').val(formatearMonto(Solicitud.PorcentajeDescuentoComision));

                                    //<INIGTI_753_3>
                                    $('#ModSolTipoCambio').val(formatearMonto(Solicitud.TipoCambio));
                                    if ($("#ModSolMonedaPrimaUnica_RP").val() == "001") {
                                        $("#ModSolTipoCambioPanel").hide();
                                    } else {
                                        $("#ModSolTipoCambioPanel").show();
                                    };
                                    //<FINGTI_753_3>

                                    //CargarTablaBeneficiarios_RP(null);
                                    CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);
                                    CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>

                                    ////<INIGTI_753_3>
                                    //if ($("#HBloqueo").val() == "TRUE") {
                                    //    botonModSolAceptarBloqueado = true;
                                    //    $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
                                    //}
                                    ////<FINGTI_753_3>


                                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                    $clock.countdown(selectedDate.toString());

                                    $("#ModSolCargando").fadeOut();
                                    $("#ModSolSaldoCIC_RP").focus();
                                    $("#ModSolACOM").focus();
                                    $("#ModSolDCOM_RP").focus();
                                    $("#ModSolTipoCambio").focus();
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
                            $('#ModalCotizando').dialog('close');

                            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                            if (data.d.Respuesta.Controles != null) {
                                if (data.d.Respuesta.Controles[0].length) $('#ModSolPrimaUnica_RP').attr('class', data.d.Respuesta.Controles[0]);
                                if (data.d.Respuesta.Controles[1].length) $('#ModSolFechaCotizacion_RP').attr('class', data.d.Respuesta.Controles[1]);
                                if (data.d.Respuesta.Controles[2].length) $('#ModSolFechaDevengue_RP').attr('class', data.d.Respuesta.Controles[2]);
                                if (data.d.Respuesta.Controles[3].length) $('#ModSolDCOM_RP').attr('class', data.d.Respuesta.Controles[3]);

                                $('#TabCotizaciones_RP select').removeClass('formTextboxGridError');
                                if (data.d.Respuesta.Controles.length > 4) {
                                    var celdaError;
                                    for (i = 4; i < data.d.Respuesta.Controles.length; i++) {
                                        celdaError = data.d.Respuesta.Controles[i].split(',');
                                        $('#TabCotizaciones_RP tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                    }
                                }


                            }
                            $('#ModalCuadroMensaje').dialog('open');
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
                });
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

    //<INIGTI_7012>
    /* Botón Aceptar Cierre*/
    $("#ModSolAceptarCierre_RP").live("click", function () {

        if (!botonModSolAceptarBloqueado) {
            if ($("#ModSolModo").val() == "CERRAR") {

                // Seleccionar Cotización
                var idSolicitudElegida = 0;

                if ($("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
                    idSolicitudElegida = $("#TabCotizaciones_RP input[type=radio]:checked").val();
                }

                if (idSolicitudElegida == 0) {
                    $('#MCMIcono').attr('class', 'validacion');
                    $('#MCMContenedor').html('Seleccione una <strong>Cotización</strong>. Dato Obligatorio.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return;
                }

                for (i = 0; i < $('#TabCotizaciones_RP tbody tr').length; i++) {
                    if ($('#TabCotizaciones_RP tbody tr:eq(' + i + ') input').is(":checked")) {
                        var renta = $('#TabCotizaciones_RP tbody tr:eq(' + i + ') td:eq(10)').text();
                        if (renta == 0) {
                            $('#MCMIcono').attr('class', 'validacion');
                            $('#MCMContenedor').html('Seleccione una <strong>Cotización con una Renta mayor a 0');
                            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                            $('#ModalCuadroMensaje').dialog('open');
                            return;
                        }
                        var indError = $('#TabCotizaciones_RP tbody tr:eq(' + i + ') td:eq(16)').text();
                        if (indError == "**") {
                            $('#MCMIcono').attr('class', 'validacion');
                            $('#MCMContenedor').html('No puede elegir esta cotización porque no alcanza el mínimo requerido');
                            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                            $('#ModalCuadroMensaje').dialog('open');
                            return;
                        }
                    }
                }

                $("#MCAIcono").attr("class", "advertencia");
                let mensaje = '<div class="alerta-agente-titulo">Firma Digital de documentos de <span class="resaltado">Renta Particular Plus</span></div>';
                mensaje += '<div class="alerta-agente-contenido"><p>Usted va a enviar el enlace de Firma Digital a <span class="resaltado">' + $("#HAfiliadoNombreCompleto").val() + '</span> cuyo documento de identidad es <span class="resaltado">' + $("#HAfiliadoTipoDocumento").val() + ' ' + $("#HAfiliadoNumeroDocumento").val() + '</span> al siguiente correo electrónico: <span class="resaltado">' + $("#HAfiliadoEmail").val() + '</span>.</p>';
                mensaje += '<p>Para que el cliente pueda firmar digitalmente sus documentos, luego de que valide su información se le enviará una clave al siguiente número de celular: <span class="resaltado">' + $("#HAfiliadoTelefono").val() + '</span>.</p>';
                mensaje += '<p>Utilice la <b>Vista Previa</b> de los documentos para validar que todos los datos que le enviará al cliente son correctos, en caso de que haya algún error por favor modifique los datos en el Cotizador Web de Rentas y luego vuelva a validar en la opción de <b>Vista Previa</b>.</p>';
                mensaje += '<p>Para evitar inconvenientes por favor <b>valide cuidadosamente</b> que toda la información del cliente es correcta antes de enviarle el enlace de Firma Digital.</p></div >';

                $("#MCAContenedor").html(mensaje);
                $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                $("#ModalCuadroAdvertencia").dialog("open");
            }
        }
        else {
            return false;
        }
    });
    //<FINGTI_7012>

    /* Botón Vista Previa */
    $("#ModSolVistaPrevia_RP").live("click", function () {
        if (!botonModSolVistaPReviaBloqueado) {
            if ($("#ModSolModo").val() == "CERRAR") {

                // Seleccionar Cotización
                var idSolicitudElegida = 0;

                if ($("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
                    idSolicitudElegida = $("#TabCotizaciones_RP input[type=radio]:checked").val();
                }

                if (idSolicitudElegida == 0) {
                    $('#MCMIcono').attr('class', 'validacion');
                    $('#MCMContenedor').html('Seleccione una <strong>Cotización</strong>. Dato Obligatorio.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return;
                }

                for (i = 0; i < $('#TabCotizaciones_RP tbody tr').length; i++) {
                    if ($('#TabCotizaciones_RP tbody tr:eq(' + i + ') input').is(":checked")) {
                        var renta = $('#TabCotizaciones_RP tbody tr:eq(' + i + ') td:eq(10)').text();
                        console.log('renta: ' + renta);
                        if (renta == 0) {
                            $('#MCMIcono').attr('class', 'validacion');
                            $('#MCMContenedor').html('Seleccione una <strong>Cotización con una Renta mayor a 0');
                            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                            $('#ModalCuadroMensaje').dialog('open');
                            return;
                        }
                        var indError = $('#TabCotizaciones_RP tbody tr:eq(' + i + ') td:eq(16)').text();
                        console.log('indError: ' + indError);
                        if (indError == "**") {
                            $('#MCMIcono').attr('class', 'validacion');
                            $('#MCMContenedor').html('No puede elegir esta cotización porque no alcanza el mínimo requerido');
                            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                            $('#ModalCuadroMensaje').dialog('open');
                            return;
                        }
                    }
                }

                // Ejecutar la vista previa
                $("#ModSolCargando").fadeIn();
                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Generando vista previa de documentos, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Procesando" });
                $("#ModalCotizando").dialog("open");

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    num_solicitud: $("#ModSolNroSolicitud_RP").html(),
                    num_correlativo: idSolicitudElegida
                }

                console.log(params);

                $.ajax({
                    type: 'POST',
                    url: 'SeleccionSolicitud.aspx/VistaPreviaCotizacionPlus',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {
                            // Cerrar modal de espera
                            $('#ModalCotizando').dialog('close');

                            const cuspp = $("#HCUSPP_RP").val();
                            const solicitud = $("#HNroSolicitud").val();
                            const esPEP = $("#HAfiliadoPEP").val();

                            // Cargar los reportes
                            window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=1");
                            window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=3");
                            if (esPEP === "S") {
                                window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=4");
                            }
                            window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=5");
                            window.open("../Reportes/ReportesRentaParticular.aspx?cuspp=" + cuspp + "&solicitud=" + solicitud + "&formato=6");
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
                        $('#ModSolCargando').fadeOut();

                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            document.location.reload(true);
                        }
                        else {
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al generar la vista previa de los documentos.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');

                            $('#ModalCotizando').dialog('close');
                        }
                    }
                });
            }
        }
        else {
            return false;
        }
    });

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


    /********************************\
    |*    REPORTE DE SEGUIMIENTO    *|
    \********************************/

    /* Cambiar valor en combobox de Jefes */
    $('#Jefe').live('change', function (e) {
        $('#CargandoSupervisor').show();
        var params = {
            idJefe: $('#Jefe').val()
        }
        $.ajax({
            type: 'POST',
            url: 'Seguimiento.aspx/CargarComboSupervisores',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                $('#ControlSupervisor').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#Supervisor');
                $('#Agente').val('0');
                $('#Agente').trigger('change');
                $('#Agente').attr('disabled', 'disabled');
                $('#ConAgente').addClass('formComboboxReadOnlyContenedor');
                $('#Supervisor').focus();

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
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de supervisores.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });

    /* Cambiar valor en combobox de Supervisores*/
    $('#Supervisor').live('change', function (e) {
        $('#CargandoAgente').show();
        var params = {
            idSupervisor: $('#Supervisor').val()
        }
        $.ajax({
            type: 'POST',
            url: 'Seguimiento.aspx/CargarComboAgentes',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                $('#ControlAgente').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#Agente');
                $('#Agente').focus();

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
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de agentes.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
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

    /* Botón de Paginador de Tabla de Seguimiento */
    $('#TabSeguimientoPaginador a').live('click', function () {
        if (!botonesBloqueados) {
            var pagina = $(this).data("pag");
            $('#TabSeguimientoIndicePagina').val(pagina);

            CargarTablaSeguimiento2(SegIdJefe, SegIdSupervisor, SegIdAgente, SegCUSPP, SegFechaInicio, SegFechaTermino);
        }
        else {
            return false;
        }
    });

    /* Botón de Ordenar de Tabla de Seguimiento */
    $('#TabSeguimiento a').live('click', function () {
        if (!botonesBloqueados) {
            var columna = $(this).data("col");
            if (columna != $('#TabSeguimientoColumnaOrdenar').attr('value')) {
                $('#TabSeguimientoColumnaOrdenar').val(columna);
                $('#TabSeguimientoDireccionOrdenar').val('A');
            }
            else {
                if ($('#TabSeguimientoDireccionOrdenar').attr('value') == 'A') {
                    $('#TabSeguimientoDireccionOrdenar').val('D');
                }
                else {
                    $('#TabSeguimientoDireccionOrdenar').val('A');
                }
            }
            $('#TabSeguimientoIndicePagina').val(1);

            CargarTablaSeguimiento2(SegIdJefe, SegIdSupervisor, SegIdAgente, SegCUSPP, SegFechaInicio, SegFechaTermino);
        }
    });

    /********************************\
    |*    REPORTE DE SUPERVISIÓN    *|
    \********************************/
    /* Botón Buscar */
    $('#SupBuscar').live('click', function (e) {
        if (!botonesBloqueados) {
            $('#TablaSeguimientoError').hide();

            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            $("#FechaInicio").removeClass("formCalendarError");
            $("#FechaTermino").removeClass("formCalendarError");

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
            if (!fechaInicio) { $('#FechaInicio').attr('class', 'formTextbox formTextboxError formCalendarError'); } else { $('#FechaInicio').attr('class', 'formTextbox formCalendar'); }
            if (!fechaTermino) { $('#FechaTermino').attr('class', 'formTextbox formTextboxError formCalendarError'); } else { $('#FechaTermino').attr('class', 'formTextbox formCalendar'); }

            esCorrecto = fechaInicio & fechaTermino;

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

                $('#TabSupervisionIndicePagina').val(1);
                $('#TabSupervisionColumnaOrdenar').val(1);
                $('#TabSupervisionDireccionOrdenar').val('A');

                CargarTablaSupervision(SegIdJefe, SegIdSupervisor, SegIdAgente, SegFechaInicio, SegFechaTermino);
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

    $('#SupBuscar').live('click', function (e) {
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

    /* Botón de Paginador de Tabla de Supervisión */
    $('#TabSupervisionPaginador a').live('click', function () {
        if (!botonesBloqueados) {
            var pagina = $(this).data("pag");
            $('#TabSupervisionIndicePagina').val(pagina);

            CargarTablaSupervision2(SegIdJefe, SegIdSupervisor, SegIdAgente, SegFechaInicio, SegFechaTermino);
        }
        else {
            return false;
        }
    });

    /* Botón de Ordenar de Tabla de Supervisión */
    $('#TabSupervision a').live('click', function () {
        if (!botonesBloqueados) {
            var columna = $(this).data("col");
            if (columna != $('#TabSupervisionColumnaOrdenar').attr('value')) {
                $('#TabSupervisionColumnaOrdenar').val(columna);
                $('#TabSupervisionDireccionOrdenar').val('A');
            }
            else {
                if ($('#TabSupervisionDireccionOrdenar').attr('value') == 'A') {
                    $('#TabSupervisionDireccionOrdenar').val('D');
                }
                else {
                    $('#TabSupervisionDireccionOrdenar').val('A');
                }
            }
            $('#TabSupervisionIndicePagina').val(1);
            CargarTablaSupervision2(SegIdJefe, SegIdSupervisor, SegIdAgente, SegFechaInicio, SegFechaTermino);
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
            Solicitud.Cotizaciones.splice(idCotizacion - 1, 1);
            $('#ModalCuadroAdvertencia').dialog('close');
            CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
            return;
        }
        else if ($('#MCATablaEliminar').val() == 'cierre') {
            //$('#EstadoCivil_RP').addClass('formTextboxError');
            $('#Pes1').click()
            $('#ModalCuadroAdvertencia').dialog('close');
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

    $("#MCAAceptar").live("click", function () {
        if ($("#ModGruFamModo_RPP").val() == "EliminarPersonaVinculada") {
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
                    CargarTablaPersonasVinculadas_RP($('#ModGruFamsoltud').val())
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

    /* Botón Cancelar */
    $('#MCACancelar_Plus').live('click', function () {
        //$("#ModSolModo").val('CERRAR')
        $('#ModalCuadroAdvertencia').dialog('close');
    });

    //<INIGTI_7012>
    /* Botón Aceptar Cierre*/
    $("#MCAAceptarCierre_Plus").live("click", function () {
        if (!botonModSolAceptarBloqueado) {
            if ($("#ModSolModo").val() == "CERRAR") {

                $("#ModalCuadroAdvertencia").dialog("close");

                // Seleccionar Cotización
                var idSolicitudElegida = 0;

                if ($("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
                    idSolicitudElegida = $("#TabCotizaciones_RP input[type=radio]:checked").val();
                }

                $("#ModSolCargando").fadeIn();
                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Cerrando la cotización, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Cerrando" });
                $("#ModalCotizando").dialog("open");

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    num_solicitud: $("#ModSolNroSolicitud_RP").html(),
                    num_correlativo: idSolicitudElegida
                }

                $.ajax({
                    type: 'POST',
                    url: 'SeleccionSolicitud.aspx/CerrarCotizacionPlus',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {
                            // Cambiar la modal a modo de modificación
                            //$('#ModSolModo').val('');
                            // Cerrar modal de espera
                            $('#ModalCotizando').dialog('close');

                            // Mostrar mensaje de éxito
                            $('#MCMIcono').attr('class', data.d.Icono);
                            $('#MCMContenedor').html(data.d.Mensaje);
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                            $('#ModalCuadroMensaje').dialog('open');

                            //botonModSolAceptarBloqueado = true;
                            //$('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

                            //setTimeout(function () {
                            //    window.location.href = "../RentaPrivadaPlus/SeleccionSolicitud.aspx?seleccione=0";
                            //}, 1000);

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
                        $('#ModSolCargando').fadeOut();

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
            else if ($("#ModSolModo").val() == "CorregirDocumentos") {
                $("#ModalCuadroAdvertencia").dialog("close");

                // Seleccionar Cotización
                var idSolicitudElegida = 0;

                if ($("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
                    idSolicitudElegida = $("#TabCotizaciones_RP input[type=radio]:checked").val();
                }

                $("#ModSolCargando").fadeIn();
                $("#MCIcono").attr("class", "cargando");
                $("#MCContenedor").html("Reaperturando los documentos, por favor espere un momento...");
                $("#ModalCotizando").dialog({ title: "Procesando" });
                $("#ModalCotizando").dialog("open");

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    num_solicitud: $("#ModSolNroSolicitud_RP").html(),
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

                            setTimeout(function () {
                                window.location.href = "../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx";
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

    //<FINGTI_7012>

    /* CERRAR SESIÓN */
    /* Botón Cerrar Sesión */
    $('#MenuSalir').live('click', function () {
        if (datosSinGuardar) {
            $('#ModalCerrarSesion').dialog('open');
            return false;
        }
    });

    /* Botón Aceptar */
    $('#MCSCancelar').live('click', function () {
        $('#ModalCerrarSesion').dialog('close');
    });

    //<SRI.INI-20322_E2>
    /* Cambiar valor en combobox de Jefes */
    $('#OfiJefe').live('change', function (e) {
        $('#CargandoSupervisor').show();
        var params = {
            idJefe: $('#OfiJefe').val()
        }
        $.ajax({
            type: 'POST',
            url: 'CotizadorOficiales.aspx/CargarComboSupervisoresOficiales',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                $('#ControlSupervisor').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#OfiSupervisor');
                $('#OfiAgente').val('0');
                $('#OfiAgente').trigger('change');
                $('#OfiAgente').attr('disabled', 'disabled');
                $('#ConAgente').addClass('formComboboxReadOnlyContenedor');
                $('#OfiSupervisor').focus();
                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                $clock.countdown(selectedDate.toString());
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de supervisores.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Cambiar valor en combobox de Supervisores*/
    $('#OfiSupervisor').live('change', function (e) {
        $('#CargandoAgente').show();
        var params = {
            idSupervisor: $('#OfiSupervisor').val()
        }
        $.ajax({
            type: 'POST',
            url: 'CotizadorOficiales.aspx/CargarComboAgentesOficiales',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                $('#ControlAgente').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#OfiAgente');
                $('#OfiAgente').focus();

                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                $clock.countdown(selectedDate.toString());
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de agentes.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Buscar Oficiales*/
    $('#btnCotOfiBuscar').live('click', function (e) {
        if (!botonesBloqueados) {
            $('#TablaSeguimientoError').hide();

            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            if (esCorrecto) {
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
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    $('#btnCotOfiBuscar').live('click', function (e) {
        if (!botonesBloqueados) {
            $('#HJefe').val(SegIdJefe);
            $('#HSupervisor').val(SegIdSupervisor);
            $('#HAgente').val(SegIdAgente);
        }
        else {
            return false;
        }
    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /*Begin Botón Cancelar ACOM */
    $('#MRVCancelarSeleccionAcomOficiales').live('click', function () {
        $('#ModalSeleccionAcom').dialog('close');
    });
    /*End Botón Cancelar ACOM */
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /*Begin Botón Aceptar ACOM*/
    $('#MRVGuardarSeleccionAcomOficiales').live('click', function () {

        $('#MGRIcono').attr('class', 'cargando');
        $('#MGRContenedor').html('Generando el reporte de escenarios, por favor espere un momento...');
        $('#ModalGenerandoReporte').dialog({ title: 'Generando' });
        $('#ModalGenerandoReporte').dialog('open');

        var maxAcom = $('input[type="radio"]:checked').val();

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            idSolicitud: idSolicitudTmp,
            fecCotizacion: fechaCotizacionTmp,
            maxAcom: maxAcom,
            idAgente: $("#HNumAgente").val()
        }

        $.ajax({
            type: 'POST',
            url: 'CotizadorOficiales.aspx/ExportarReporteEscenariosPDF',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {
                    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                        window.location.href = "../Reportes/EscenariosMovil.aspx";
                    }
                    else {
                        var w = 800;
                        var h = 600;
                        var left = (screen.width / 2) - (w / 2);
                        var top = (screen.height / 2) - (h / 2);

                        $('#ModalGenerandoReporte').dialog('close');
                        var nuevaVentana = window.open("../Reportes/Escenarios.aspx", "_blank", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                    }

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else if (data.d.Estado == "ERROR") {
                    $('#ModalGenerandoReporte').dialog('close');

                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog('open');

                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#ModalGenerandoReporte').dialog('close');

                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al generar el reporte de escenarios.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            }
        });
    });
    /*End Botón Aceptar ACOM*/
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /*Begin Cargar Acom */
    $('#TabCotizacionCliente .grilla_rep_escenario').live('click', function () {
        var numSolicitud = $(this).data('operacion');
        var fecCotizacion = $(this).data('fechacotizacion');

        $("#HNumAgente").val($(this).data('numagente'));

        idSolicitudTmp = numSolicitud;

        var params = {
            idSolicitud: numSolicitud,
            fecCotizacion: fecCotizacion
        }
        $.ajax({
            type: 'POST',
            url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitud',
            contentType: "application/json; charset=iso-8859-1",
            data: $.toJSON(params),
            dataType: 'json',
            success: function (data) {
                Solicitud = data.d;

                var fecCotizacion = new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy");
                fechaCotizacionTmp = fecCotizacion;

                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    idSolicitud: numSolicitud,
                    fecCotizacion: fecCotizacion,
                    numAgente: $("#HNumAgente").val()
                }

                $.ajax({
                    type: 'POST',
                    url: 'CotizadorOficiales.aspx/CargarRolEscenario',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d != 'TOKEN') {
                            $('#TablaSeleccionAcomCargando').hide();
                            $('#TablaSeleccionAcomContenedor').show();
                            $('#TablaSeleccionAcomContenedor').html($(data.d).find('#ContenidoDinamico').html());
                            $('#ModalSeleccionAcom').dialog({ title: 'Acom - Escenario' });
                            $('#ModalSeleccionAcom').dialog('open');

                            $('#TabSeleccionAcom tbody').each(function () {
                                $(this).find('tr').last().addClass("grilla_active");
                            });

                            var rowCount = $('#TabSeleccionAcom tr').length - 1;
                            $('#rad_' + rowCount).attr('checked', true);


                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                            $clock.countdown(selectedDate.toString());

                        }
                        else {
                            CerrarSesionExpirada();
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            //Sesión caducada /
                            document.location.reload(true);
                        }
                        else {
                            $('#TablaSeleccionAcomCargando').hide();
                            $('#DivErrorAcom').show();
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
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                $('#ModalSolicitud').dialog('close');
            }
        });

    });
    /*End Cargar Acom */
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Nuevo Escenario */
    $("#TabCotizacionCotizaciones .grilla_nuevo").live("click", function () {
        $("#ModSolModo").val("N");
        $('#ModSolCargando').show();

        LimpiarFormularioSolicitudOficial();

        $("#ModalSolicitud").dialog("open");
        var fecCotizacion = $(this).data("fechacotizacion");
        var numSolicitud = $(this).data("solicitud");
        var numOperacion = $(this).data('operacion');

        $("#HFecCotizacion").val(fecCotizacion);
        $('#HNumSolicitud').val(numSolicitud);
        $('#HNumOperacion').val(numOperacion);

        var params = {
            numSolicitud: numSolicitud
        }
        $.ajax({
            type: 'POST',
            url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitudEscenario',
            contentType: "application/json; charset=iso-8859-1",
            data: $.toJSON(params),
            dataType: 'json',
            success: function (data) {
                SolicitudEscenario = data.d;

                $('#ModSolNroSolicitud_RP').html(SolicitudEscenario.NumSolicitud);
                $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
                $('#ModSolACOM').val(formatearMonto2(SolicitudEscenario.PjeAumentoComision));
                $('#ModSolDCOM_RP').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
                $('#ModSolIndSeleccionado').val(SolicitudEscenario.IndEstadoSeleccion);
                $('#TexModSolIndSeleccionado').html($('#ModSolIndSeleccionado').find(':selected').text());
                $('#ModSolValMontoAcomAgente').val(formatearMonto2(SolicitudEscenario.ValMtoAgenteAcom));
                $('#ModSolIndCondicionEspecial').html(SolicitudEscenario.IndCondicionEspecial);
                $('#ModSolIndCondicionAprobado').html(SolicitudEscenario.IndAprueba);
                $('#ModSolMontoCIC').html(formatearMonto(SolicitudEscenario.ValTotalCic));
                $('#ModFecCierre').html(new Date(+SolicitudEscenario.FecCierre.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolCategoria').val(SolicitudEscenario.Categoria.Id.toString()).change();
                $('#ModSolFechaRegistro').html(new Date(+SolicitudEscenario.FecRegistroEscenario.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolCussp').html(SolicitudEscenario.Afiliado.CUSPP);
                $('#ModSolNomAfiliado').html(SolicitudEscenario.Afiliado.NombreEmpresa);
                $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id);
                $('#ModSolNomAgente').html(SolicitudEscenario.Agente.Nombre);

                $('#ModSolCargando').fadeOut();
                $('#ModSolACOM').focus();

                var params = {
                    idSolicitud: numSolicitud,
                    fecCotizacion: fecCotizacion
                }
                $.ajax({
                    type: 'POST',
                    url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitud',
                    contentType: "application/json; charset=iso-8859-1",
                    data: $.toJSON(params),
                    dataType: 'json',
                    success: function (data) {
                        Solicitud = data.d;

                        CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);
                        CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, false, true);

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
                            $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                        $('#ModalSolicitud').dialog('close');
                    }
                });
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
    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Modificar */
    $("#TabCotizacionCotizaciones .grilla_editar").live("click", function () {
        $("#ModSolModo").val("M");

        $('#ModSolCargando').show();

        LimpiarFormularioSolicitudOficial();

        $('#ModalSolicitud').dialog('open');
        //var numCusspp = $(this).parent('td').parent('tr').children().eq(9).html();
        var fecCotizacion = $(this).data('fechacotizacion');
        var numSolicitud = $(this).data('solicitud');
        var numOperacion = $(this).data('operacion');
        var permisoTra;

        $("#HFecCotizacion").val(fecCotizacion);
        $('#HNumSolicitud').val(numSolicitud);
        $('#HNumOperacion').val(numOperacion);

        var params = {
            numSolicitud: numSolicitud
        }
        $.ajax({
            type: 'POST',
            url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitudEscenario',
            contentType: "application/json; charset=iso-8859-1",
            data: $.toJSON(params),
            dataType: 'json',
            success: function (data) {
                SolicitudEscenario = data.d;

                $('#ModSolNroSolicitud_RP').html(SolicitudEscenario.NumSolicitud);
                $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
                $('#ModSolACOM').val(formatearMonto2(SolicitudEscenario.PjeAumentoComision));
                $('#ModSolDCOM_RP').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
                $('#ModSolIndSeleccionado').val(SolicitudEscenario.IndEstadoSeleccion);
                $('#TexModSolIndSeleccionado').html($('#ModSolIndSeleccionado').find(':selected').text());
                $('#ModSolValMontoAcomAgente').val(formatearMonto2(SolicitudEscenario.ValMtoAgenteAcom));
                $('#ModSolIndCondicionEspecial').html(SolicitudEscenario.IndCondicionEspecial);
                $('#ModSolIndCondicionAprobado').html(SolicitudEscenario.IndAprueba);
                $('#ModSolMontoCIC').html(formatearMonto(SolicitudEscenario.ValTotalCic));
                $('#ModFecCierre').html(new Date(+SolicitudEscenario.FecCierre.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolCategoria').val(SolicitudEscenario.Categoria.Id.toString()).change();
                $('#ModSolFechaRegistro').html(new Date(+SolicitudEscenario.FecRegistroEscenario.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolCussp').html(SolicitudEscenario.Afiliado.CUSPP);
                $('#ModSolNomAfiliado').html(SolicitudEscenario.Afiliado.NombreEmpresa);
                $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id);
                $('#ModSolNomAgente').html(SolicitudEscenario.Agente.Nombre);

                $('#ModSolCargando').fadeOut();
                $('#ModSolACOM').focus();

                var params = {
                    idSolicitud: numSolicitud,
                    fecCotizacion: fecCotizacion
                }
                $.ajax({
                    type: 'POST',
                    url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitud',
                    contentType: "application/json; charset=iso-8859-1",
                    data: $.toJSON(params),
                    dataType: 'json',
                    success: function (data) {
                        Solicitud = data.d;

                        CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);

                        $('#HCodTipoMovimiento').val(Solicitud.TipoMovimiento.Id);

                        if (Solicitud.TipoMovimiento.Id == 0) {
                            botonModSolAceptarBloqueado = false;
                            permisoTra = true;
                            $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');
                        }
                        else {
                            botonModSolAceptarBloqueado = true;
                            permisoTra = false;
                            $('#ModSolAceptarOficial').attr('class', 'botonDeshabilitado gris gris_sharp');
                        }

                        var params = {
                            idTipoPension: Solicitud.TipoPension.Id
                        }
                        $.ajax({
                            type: 'POST',
                            url: 'CotizadorOficiales.aspx/CargarComboProductos',
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: 'json',
                            data: $.toJSON(params),
                            success: function (data) {

                                $('#ModSolACOM').attr('readonly', false);
                                $('#ModSolDCOM_RP').attr('readonly', false);
                                $('#ModSolIndSeleccionado').attr('readonly', false);
                                $('#ModSolValMontoAcomAgente').attr('readonly', false);

                                CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, permisoTra, true);

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
                                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de productos.');
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
                            $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                        $('#ModalSolicitud').dialog('close');
                    }
                });



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
    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Aceptar */
    $('#TabCotizacionCotizaciones .grilla_copiar').live('click', function () {

        $('#ModSolModo').val('C');

        var fecCotizacion = $(this).data('fechacotizacion');
        var numSolicitud = $(this).data('solicitud');
        var numOperacion = $(this).data('operacion');
        var numAgente = $(this).data('numagente');

        $('#HFecCotizacion').val(fecCotizacion);
        $('#HNumSolicitud').val(numSolicitud);
        $('#HNumOperacion').val(numOperacion);

        $('#MCIcono').attr('class', 'cargando');
        $('#MCContenedor').html('Generando solicitud Extraoficial, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Copiando' });
        $('#ModalCotizando').dialog('open');

        var params = {
            idSolicitud: numSolicitud,
            fecCotizacion: fecCotizacion
        }
        $.ajax({
            type: 'POST',
            url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitud',
            contentType: "application/json; charset=iso-8859-1",
            data: $.toJSON(params),
            dataType: 'json',
            success: function (data) {
                Solicitud = data.d;

                var params = {
                    //tokenUsuario: $('#TokenUsuario').val(),
                    numSolicitud: $('#HNumSolicitud').val(),
                    fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
                    numAgente: numAgente
                }
                fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/insertar-solicitud-extraoficial', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json; charset=iso-8859-1',
                        'x-username': $('#usuario_actual').val(),
                        'x-rol': $('#rol_azman').val()
                    },
                    body: JSON.stringify(params)
                })
                    .then(response => {
                        if (!response.ok) {
                            if (response.status === 401 || response.status === 12030) {
                                document.location.reload(true);
                                return;
                            }
                            throw new Error('Error en la respuesta del servidor');
                        }
                        return response.json();
                    })
                    .then(data => {
                        if (data.data.Respuesta.Estado == 'OK') {
                            // Imprimir número de solicitud generada
                            $('#ModSolNroSolicitud_RP').html(data.data.NumSolicitud);
                            $('#HNumSolicitud').val(data.data.NumSolicitud);

                            // Cambiar la modal a modo de modificación
                            $('#ModSolModo').val('M');

                            // Cerrar modal de espera
                            $('#ModalCotizando').dialog('close');

                            // Mostrar mensaje de éxito
                            $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                            $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                            $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                        else if (data.data.Estado == 'TOKEN') {
                            CerrarSesionExpirada();
                        }
                        else {
                            $('#ModalCotizando').dialog('close');

                            $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                            $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                            $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                            $('#ModalCuadroMensaje').dialog('open');
                        }

                        $('#ModSolCargando').fadeOut();
                        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                    })
                    .catch(error => {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');

                        $('#ModalCotizando').dialog('close');
                    });
                //$.ajax({
                //    type: 'POST',
                //    url: 'CotizadorOficiales.aspx/InsertarSolicitudExtraoficial',
                //    contentType: "application/json; charset=iso-8859-1",
                //    dataType: 'json',
                //    data: $.toJSON(params),
                //    success: function (data) {

                //        if (data.d.Respuesta.Estado == 'OK') {

                //            // Imprimir número de solicitud generada
                //            $('#ModSolNroSolicitud_RP').html(data.d.NumSolicitud);

                //            $('#HNumSolicitud').val(data.d.NumSolicitud);

                //            // Cambiar la modal a modo de modificación
                //            $('#ModSolModo').val('M');

                //            // Cerrar modal de espera
                //            $('#ModalCotizando').dialog('close');

                //            // Mostrar mensaje de éxito
                //            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                //            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                //            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                //            $('#ModalCuadroMensaje').dialog('open');

                //        }
                //        else if (data.d.Estado == 'TOKEN') {
                //            CerrarSesionExpirada();
                //        }
                //        else {
                //            $('#ModalCotizando').dialog('close');

                //            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                //            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                //            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                //            $('#ModalCuadroMensaje').dialog('open');
                //        }

                //        $('#ModSolCargando').fadeOut();
                //        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                //    },
                //    error: function (XMLHttpRequest, textStatus, errorThrown) {
                //        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                //            document.location.reload(true);
                //        }
                //        else {
                //            $('#MCMIcono').attr('class', 'error');
                //            $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                //            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                //            $('#ModalCuadroMensaje').dialog('open');

                //            $('#ModalCotizando').dialog('close');
                //        }
                //    }
                //});


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


    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Aceptar */
    $("#ModSolAceptarOficial").live("click", function () {
        if (!botonModSolAceptarBloqueado) {
            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            $("#ModSolACOM").removeClass("formTextboxError");
            $("#ModSolDCOM_RP").removeClass("formTextboxError");
            $("#ModSolIndSeleccionado").removeClass("formComboboxErrorContenedor");
            $("#ModSolValMontoAcomAgente").removeClass("formTextboxError");

            // ACOM
            var vacom = true;
            if ($.trim($("#ModSolACOM").val()).length == 0) {
                errores.push("Ingrese el campo <strong>Porcentaje A</strong>. Dato Obligatorio.");
                vacom = false;
            }

            // DCOM
            var vdcom = true;
            if ($.trim($("#ModSolDCOM_RP").val()).length == 0) {
                errores.push("Ingrese el campo <strong>Porcentaje D</strong>. Dato Obligatorio.");
                vdcom = false;
            }

            // Ind. Seleccionado
            var vindseleccionado = true;
            if ($("#ModSolIndSeleccionado").val() == "0") {
                errores.push("Ingrese el campo <strong>Ind. Seleccionado</strong>. Dato Obligatorio.");
                vindseleccionado = false;
            }

            // Monto A
            var vmontoacom = true;
            if ($.trim($("#ModSolValMontoAcomAgente").val()).length == 0) {
                errores.push("Ingrese el campo <strong>Monto A</strong>. Dato Obligatorio.");
                vmontoacom = false;
            }
            else {
                if ($("#ModSolValMontoAcomAgente").val() == 0 && $("#ModSolACOM").val() > 0 && $("#ModSolIndSeleccionado").val() == "S") {
                    errores.push("El campo <strong>Monto A</strong> no puede ser 0 si está solicitando Porcentaje A, asegúrese de haber seleccionado una modalidad para que el sistema pueda calcular el monto.");
                    vmontoacom = false;
                }
            }

            // Seleccionar Cotización
            var idSolicitudElegida = 0;

            if ($("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
                idSolicitudElegida = $("#TabCotizaciones_RP input[type=radio]:checked").val();
            }

            // Clases de controles
            if (!vacom) $("#ModSolACOM").addClass("formTextboxError");
            if (!vdcom) $("#ModSolDCOM_RP").addClass("formTextboxError");
            if (!vindseleccionado) $("#ConModSolIndSeleccionado").addClass("formComboboxErrorContenedor");
            if (!vmontoacom) $("#ModSolValMontoAcomAgente").addClass("formTextboxError");

            esCorrecto = vacom & vdcom & vindseleccionado & vmontoacom;

            if (esCorrecto) {
                // Nuevo Escenario Oficial
                if ($("#ModSolModo").val() == "N") {
                    $("#MCIcono").attr("class", "cargando");
                    $("#MCContenedor").html("Cotizando la solicitud, por favor espere un momento...");
                    $("#ModalCotizando").dialog({ title: "Cotizando" });
                    $("#ModalCotizando").dialog("open");

                    var params = {
                        //tokenUsuario: $("#TokenUsuario").val(),
                        idSolicitud: $("#HNumSolicitud").val(),
                        fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
                        acom: $("#ModSolACOM").val(),
                        dcom: $("#ModSolDCOM_RP").val(),
                        indEstadoSeleccion: $("#ModSolIndSeleccionado").val(),
                        valMtoAgenteAcom: $("#ModSolValMontoAcomAgente").val(),
                        numCotizacionElegida: (idSolicitudElegida == null) ? null : idSolicitudElegida,
                        correo: Solicitud.Afiliado.CorreoElectronico
                    }

                    
                    let tienePermiso = validarPermisoCWRV(opcionesSistema, 16);
                    if (tienePermiso) {
                    fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/insertar', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json; charset=iso-8859-1',
                            'x-username': $('#usuario_actual').val(),
                            'x-rol': $('#rol_azman').val()
                        },
                        body: JSON.stringify(params)
                    })
                        .then(response => {
                            if (!response.ok) {
                                if (response.status === 401 || response.status === 12030) {
                                    document.location.reload(true);
                                    throw new Error('Sesión expirada');
                                }
                                throw new Error('Error en la respuesta del servidor');
                            }
                            return response.json();
                        })
                        .then(data => {
                            if (data.data.Respuesta.Estado == 'OK') {
                                // Imprimir número de solicitud generada
                                $('#ModSolNroSolicitud_RP').html(data.data.NumSolicitud);
                                $('#HNumSolicitud').val(data.data.NumSolicitud);

                                // Cambiar la modal a modo de modificación
                                $('#ModSolModo').val('M');

                                // Cerrar modal de espera
                                $('#ModalCotizando').dialog('close');

                                // Mostrar mensaje de éxito
                                $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                                $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                                $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                                $('#ModalCuadroMensaje').dialog('open');

                                CargarTablaSolicitudesOficiales($('#OfiJefe').val(), $('#OfiSupervisor').val(), $('#OfiAgente').val());

                                // Cargar la cotización actualizada
                                $('#BeneficiariosOriginales_RP').show();
                                $('#ManSolPestanhas li:eq(0)').trigger('click');

                                $('#ModSolCargando').show();

                                var params = {
                                    numSolicitud: $('#HNumSolicitud').val()
                                }

                                LimpiarFormularioSolicitudOficial();

                                $.ajax({
                                    type: 'POST',
                                    url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitudEscenario',
                                    contentType: "application/json; charset=iso-8859-1",
                                    data: $.toJSON(params),
                                    dataType: 'json',
                                    success: function (data) {
                                        SolicitudEscenario = data.d;

                                        $('#ModSolNroSolicitud_RP').html(SolicitudEscenario.NumSolicitud);
                                        $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
                                        $('#ModSolACOM').val(formatearMonto(SolicitudEscenario.PjeAumentoComision));
                                        $('#ModSolDCOM_RP').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
                                        $('#ModSolIndSeleccionado').val(SolicitudEscenario.IndEstadoSeleccion);
                                        $('#TexModSolIndSeleccionado').html($('#ModSolIndSeleccionado').find(':selected').text());
                                        $('#ModSolValMontoAcomAgente').val(formatearMonto2(SolicitudEscenario.ValMtoAgenteAcom));
                                        $('#ModSolIndCondicionEspecial').html(SolicitudEscenario.IndCondicionEspecial);
                                        $('#ModSolIndCondicionAprobado').html(SolicitudEscenario.IndAprueba);
                                        $('#ModSolMontoCIC').html(formatearMonto(SolicitudEscenario.ValTotalCic));
                                        $('#ModFecCierre').html(new Date(+SolicitudEscenario.FecCierre.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolCategoria').val(SolicitudEscenario.Categoria.Id.toString()).change();
                                        $('#ModSolFechaRegistro').html(new Date(+SolicitudEscenario.FecRegistroEscenario.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolCussp').html(SolicitudEscenario.Afiliado.CUSPP);
                                        $('#ModSolNomAfiliado').html(SolicitudEscenario.Afiliado.NombreEmpresa);
                                        $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id);
                                        $('#ModSolNomAgente').html(SolicitudEscenario.Agente.Nombre);

                                        $('#ModSolCargando').fadeOut();
                                        $('#ModSolACOM').focus();

                                        var params = {
                                            idSolicitud: $('#HNumSolicitud').val(),
                                            fecCotizacion: $('#HFecCotizacion').val()
                                        }
                                        $.ajax({
                                            type: 'POST',
                                            url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitud',
                                            contentType: "application/json; charset=iso-8859-1",
                                            data: $.toJSON(params),
                                            dataType: 'json',
                                            success: function (data) {
                                                Solicitud = data.d;

                                                CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);
                                                CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

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
                                                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                                                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                                    $('#ModalCuadroMensaje').dialog('open');
                                                }
                                                $('#ModalSolicitud').dialog('close');
                                            }
                                        });
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
                                $('#ModalCotizando').dialog('close');

                                $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                                $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                                $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                                if (data.d.Respuesta.Controles != null) {
                                    if (data.d.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.d.Respuesta.Controles[0]);
                                    if (data.d.Respuesta.Controles[1].length) $('#ModSolDCOM_RP').attr('class', data.d.Respuesta.Controles[1]);
                                    if (data.d.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.d.Respuesta.Controles[2]);
                                    if (data.d.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.d.Respuesta.Controles[3]);

                                    $('#TabCotizaciones_RP select').removeClass('formTextboxGridError');
                                    if (data.d.Respuesta.Controles.length > 13) {
                                        var celdaError;
                                        for (i = 13; i < data.d.Respuesta.Controles.length; i++) {
                                            celdaError = data.d.Respuesta.Controles[i].split(',');
                                            $('#TabCotizaciones_RP tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                        }
                                    }
                                }
                                $('#ModalCuadroMensaje').dialog('open');
                            }

                            $('#ModSolCargando').fadeOut();
                        })
                        .catch(error => {
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');

                            $('#ModalCotizando').dialog('close');
                        })
                        .finally(() => {
                            botonModSolAceptarBloqueado = false;
                            $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                        });
                    } else {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html("Usted no tiene privilegios sobre esta opción.");
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                        $('#ModalCotizando').dialog('close');
                    }

                    //$.ajax({
                    //    type: 'POST',
                    //    url: 'CotizadorOficiales.aspx/InsertarSolicitud',
                    //    contentType: "application/json; charset=iso-8859-1",
                    //    dataType: 'json',
                    //    data: $.toJSON(params),
                    //    success: function (data) {

                    //        if (data.d.Respuesta.Estado == 'OK') {

                    //            // Imprimir número de solicitud generada
                    //            $('#ModSolNroSolicitud_RP').html(data.d.NumSolicitud);

                    //            $('#HNumSolicitud').val(data.d.NumSolicitud);

                    //            // Cambiar la modal a modo de modificación
                    //            $('#ModSolModo').val('M');

                    //            // Cerrar modal de espera
                    //            $('#ModalCotizando').dialog('close');

                    //            // Mostrar mensaje de éxito
                    //            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                    //            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                    //            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                    //            $('#ModalCuadroMensaje').dialog('open');

                    //            CargarTablaSolicitudesOficiales($('#OfiJefe').val(), $('#OfiSupervisor').val(), $('#OfiAgente').val());

                    //            // Cargar la cotización actualizada
                    //            $('#BeneficiariosOriginales_RP').show();
                    //            $('#ManSolPestanhas li:eq(0)').trigger('click');

                    //            $('#ModSolCargando').show();


                    //            var params = {
                    //                numSolicitud: $('#HNumSolicitud').val()
                    //            }

                    //            LimpiarFormularioSolicitudOficial();

                    //            $.ajax({
                    //                type: 'POST',
                    //                url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitudEscenario',
                    //                contentType: "application/json; charset=iso-8859-1",
                    //                data: $.toJSON(params),
                    //                dataType: 'json',
                    //                success: function (data) {
                    //                    SolicitudEscenario = data.d;

                    //                    $('#ModSolNroSolicitud_RP').html(SolicitudEscenario.NumSolicitud);
                    //                    $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
                    //                    $('#ModSolACOM').val(formatearMonto(SolicitudEscenario.PjeAumentoComision));
                    //                    $('#ModSolDCOM_RP').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
                    //                    $('#ModSolIndSeleccionado').val(SolicitudEscenario.IndEstadoSeleccion);
                    //                    $('#TexModSolIndSeleccionado').html($('#ModSolIndSeleccionado').find(':selected').text());
                    //                    $('#ModSolValMontoAcomAgente').val(formatearMonto2(SolicitudEscenario.ValMtoAgenteAcom));
                    //                    $('#ModSolIndCondicionEspecial').html(SolicitudEscenario.IndCondicionEspecial);
                    //                    $('#ModSolIndCondicionAprobado').html(SolicitudEscenario.IndAprueba);
                    //                    $('#ModSolMontoCIC').html(formatearMonto(SolicitudEscenario.ValTotalCic));
                    //                    $('#ModFecCierre').html(new Date(+SolicitudEscenario.FecCierre.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                    //                    $('#ModSolCategoria').val(SolicitudEscenario.Categoria.Id.toString()).change();
                    //                    $('#ModSolFechaRegistro').html(new Date(+SolicitudEscenario.FecRegistroEscenario.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                    //                    $('#ModSolCussp').html(SolicitudEscenario.Afiliado.CUSPP);
                    //                    $('#ModSolNomAfiliado').html(SolicitudEscenario.Afiliado.NombreEmpresa);
                    //                    $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id);
                    //                    $('#ModSolNomAgente').html(SolicitudEscenario.Agente.Nombre);

                    //                    $('#ModSolCargando').fadeOut();
                    //                    $('#ModSolACOM').focus();

                    //                    var params = {
                    //                        idSolicitud: $('#HNumSolicitud').val(),
                    //                        fecCotizacion: $('#HFecCotizacion').val()
                    //                    }
                    //                    $.ajax({
                    //                        type: 'POST',
                    //                        url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitud',
                    //                        contentType: "application/json; charset=iso-8859-1",
                    //                        data: $.toJSON(params),
                    //                        dataType: 'json',
                    //                        success: function (data) {
                    //                            Solicitud = data.d;

                    //                            CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);
                    //                            CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

                    //                            //$('#ModSolAceptarOficial').attr('class', 'botonDeshabilitado gris gris_sharp');

                    //                            // Actualizar temporizador
                    //                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    //                            $clock.countdown(selectedDate.toString());
                    //                        },
                    //                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //                            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    //                                document.location.reload(true);
                    //                            }
                    //                            else {
                    //                                $('#MCMIcono').attr('class', 'error');
                    //                                $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                    //                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    //                                $('#ModalCuadroMensaje').dialog('open');
                    //                            }
                    //                            $('#ModalSolicitud').dialog('close');
                    //                        }
                    //                    });

                    //                },
                    //                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    //                        document.location.reload(true);
                    //                    }
                    //                    else {
                    //                        $('#MCMIcono').attr('class', 'error');
                    //                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                    //                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    //                        $('#ModalCuadroMensaje').dialog('open');
                    //                    }
                    //                }
                    //            });
                    //        }
                    //        else if (data.d.Estado == 'TOKEN') {
                    //            CerrarSesionExpirada();
                    //        }
                    //        else {

                    //            $('#ModalCotizando').dialog('close');

                    //            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                    //            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                    //            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                    //            if (data.d.Respuesta.Controles != null) {

                    //                if (data.d.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.d.Respuesta.Controles[0]);
                    //                if (data.d.Respuesta.Controles[1].length) $('#ModSolDCOM_RP').attr('class', data.d.Respuesta.Controles[1]);
                    //                if (data.d.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.d.Respuesta.Controles[2]);
                    //                if (data.d.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.d.Respuesta.Controles[3]);

                    //                $('#TabCotizaciones_RP select').removeClass('formTextboxGridError');
                    //                if (data.d.Respuesta.Controles.length > 13) {
                    //                    var celdaError;
                    //                    for (i = 13; i < data.d.Respuesta.Controles.length; i++) {
                    //                        celdaError = data.d.Respuesta.Controles[i].split(',');
                    //                        $('#TabCotizaciones_RP tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                    //                    }
                    //                }
                    //            }
                    //            $('#ModalCuadroMensaje').dialog('open');
                    //        }

                    //        $('#ModSolCargando').fadeOut();
                    //    },
                    //    error: function (XMLHttpRequest, textStatus, errorThrown) {
                    //        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    //            document.location.reload(true);
                    //        }
                    //        else {
                    //            $('#MCMIcono').attr('class', 'error');
                    //            $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                    //            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    //            $('#ModalCuadroMensaje').dialog('open');

                    //            $('#ModalCotizando').dialog('close');
                    //        }
                    //    },
                    //    complete: function () {
                    //        botonModSolAceptarBloqueado = false;
                    //        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                    //    }
                    //});

                }
                // Modificar Escenario (TRA)
                else if ($("#ModSolModo").val() == "M") {

                    $("#MCIcono").attr("class", "cargando");
                    $("#MCContenedor").html("Guardando datos de la cotización, por favor espere un momento...");
                    $("#ModalCotizando").dialog({ title: "Cotizando" });
                    $("#ModalCotizando").dialog("open");

                    // Validar si el usuario excede sus parámetros permitidos de ACOM y TRA
                    var params = {
                        idSolicitud: $("#HNumSolicitud").val(),
                        //tokenUsuario: $("#TokenUsuario").val(),
                        fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
                        acom: $("#ModSolACOM").val(),
                        cotizaciones: Solicitud.Cotizaciones
                    }

                    fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/validar-acom-tra', {
                        method: "POST",
                        headers: {
                            'Content-Type': 'application/json; charset=iso-8859-1',
                            'x-username': $('#usuario_actual').val(),
                            'x-rol': $('#rol_azman').val()
                        },
                        body: JSON.stringify(params)
                    })
                        .then(response => {
                            if (!response.ok) {
                                if (response.status === 401 || response.status === 12030) {
                                    document.location.reload(true);
                                    throw new Error("Sesión expirada");
                                }
                                throw new Error("Error en la respuesta del servidor");
                            }
                            return response.json();
                        })
                        .then(data => {
                            
                            if (data.data.Estado == "OK") {
                                if (data.data.Contenido == "") {
                                    // Los rangos están dentro de lo permitido por el usuario, proceder a cotizar.
                                    // Guardar los datos de la solicitud
                                    var params = {
                                        //tokenUsuario: $("#TokenUsuario").val(),
                                        idSolicitud: $("#HNumSolicitud").val(),
                                        fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
                                        acom: $("#ModSolACOM").val(),
                                        dcom: $("#ModSolDCOM_RP").val(),
                                        indEstadoSeleccion: $("#ModSolIndSeleccionado").val(),
                                        valMtoAgenteAcom: $("#ModSolValMontoAcomAgente").val(),
                                        numCotizacionElegida: (idSolicitudElegida == null) ? null : idSolicitudElegida,
                                        cotizaciones: Solicitud.Cotizaciones,
                                        correo: Solicitud.Afiliado.CorreoElectronico
                                    }

                                    let tienePermiso = validarPermisoCWRV(opcionesSistema, 17);
                                    if (tienePermiso) {
                                        fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/modificar', {
                                            method: "POST",
                                            headers: {
                                                'Content-Type': 'application/json; charset=iso-8859-1',
                                                'x-username': $('#usuario_actual').val(),
                                                'x-rol': $('#rol_azman').val()
                                            },
                                            body: JSON.stringify(params)
                                        })
                                            .then(response => {
                                                if (!response.ok) {
                                                    if (response.status === 401 || response.status === 12030) {
                                                        document.location.reload(true);
                                                        throw new Error("Sesión expirada");
                                                    }
                                                    throw new Error("Error en la respuesta del servidor");
                                                }
                                                return response.json();
                                            })
                                            .then(data => {
                                                if (data.data.Respuesta.Estado == "OK") {
                                                    // Cerrar modal de espera
                                                    $('#ModalCotizando').dialog('close');

                                                    // Mostrar mensaje de éxito
                                                    $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                                                    $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                                                    $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                                                    $('#ModalCuadroMensaje').dialog('open');

                                                    CargarTablaSolicitudesOficiales($('#OfiJefe').val(), $('#OfiSupervisor').val(), $('#OfiAgente').val());
                                                }
                                                else if (data.data.Estado == 'TOKEN') {
                                                    CerrarSesionExpirada();
                                                }
                                                else {
                                                    $('#ModalCotizando').dialog('close');

                                                    $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                                                    $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                                                    $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                                                    if (data.data.Respuesta.Controles != null) {
                                                        if (data.data.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.data.Respuesta.Controles[0]);
                                                        if (data.data.Respuesta.Controles[1].length) $('#ModSolDCOM_RP').attr('class', data.data.Respuesta.Controles[1]);
                                                        if (data.data.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.data.Respuesta.Controles[2]);
                                                        if (data.data.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.data.Respuesta.Controles[3]);

                                                        if (data.data.Respuesta.Controles.length > 13) {
                                                            var celdaError;
                                                            for (i = 13; i < data.data.Respuesta.Controles.length; i++) {
                                                                celdaError = data.data.Respuesta.Controles[i].split(',');
                                                                $('#TabCotizaciones_RP tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                                            }
                                                        }
                                                    }
                                                    $('#ModalCuadroMensaje').dialog('open');
                                                }

                                                $('#ModSolCargando').fadeOut();
                                                //ModSolBotonesInactivos = false;
                                                $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
                                            })
                                            .catch(error => {
                                                if (error.message === "Sesión expirada") {
                                                    document.location.reload(true);
                                                } else {
                                                    $("#ModalCotizando").dialog("close");

                                                    $("#MCMIcono").attr("class", "error");
                                                    $("#MCMContenedor").html("Ha ocurrido un error al guardar los datos de la solicitud.");
                                                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                                                    $("#ModalCuadroMensaje").dialog("open");
                                                }
                                            });
                                    } else {
                                        $('#MCMIcono').attr('class', 'error');
                                        $('#MCMContenedor').html("Usted no tiene privilegios sobre esta opción.");
                                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                        $('#ModalCuadroMensaje').dialog('open');
                                        $('#ModalCotizando').dialog('close');
                                    }
                                }
                                else {
                                    // Los rangos NO están dentro de lo permitido por el usuario, mostrar el mensaje
                                    // de confirmación para enviar dicho mensaje al flujo de aprobación.
                                    $("#ModalCotizando").dialog("close");

                                    $("#MCAIcono").attr("class", "advertencia");
                                    $("#MCAContenedor").html(data.data.Contenido);
                                    $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                                    $("#ModalCuadroAdvertencia").dialog("open");
                                }
                            }
                            else if (data.data.Estado == "TOKEN") {
                                CerrarSesionExpirada();
                            }
                            else if (data.data.Estado == "ERROR") {
                                // Mostrar mensaje de error
                                $('#MCMIcono').attr('class', data.data.Icono);
                                $('#MCMContenedor').html(data.data.Mensaje);
                                $('#ModalCuadroMensaje').dialog({ title: data.data.Titulo });
                                $('#ModalCuadroMensaje').dialog('open');
                            }
                        })
                        .catch(error => {
                            if (error.message === "Sesión expirada") {
                                document.location.reload(true);
                            } else {
                                $("#ModalCotizando").dialog("close");
                                $("#MCMIcono").attr("class", "error");
                                $("#MCMContenedor").html("Ha ocurrido un error al guardar los datos de la solicitud.");
                                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                                $("#ModalCuadroMensaje").dialog("open");
                            }
                        });

                }
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

    $("#MCAOAceptar").live("click", function () {

        $("#ModalCuadroAdvertencia").dialog("close");
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Guardando datos de la cotización, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Cotizando" });
        $("#ModalCotizando").dialog("open");

        // Seleccionar Cotización
        var idSolicitudElegida = 0;

        if ($("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
            idSolicitudElegida = $("#TabCotizaciones_RP input[type=radio]:checked").val();
        }

        const baseUrl = window.location.origin + '/' +
            (window.location.pathname.startsWith('/') ? '' : '/') +
            (window.location.pathname === '/' ? '' : window.location.pathname.split('/')[1] + '/');

        const finalUrl = baseUrl + 'Bandejas/BandejaFlujoCotizacion.aspx';

        var params = {
            acom: $("#ModSolACOM").val(),
            montoAcom: $("#ModSolValMontoAcomAgente").val(),
            estadoSeleccion: $("#ModSolIndSeleccionado").val(),
            elegida: idSolicitudElegida,
            listaCotizaciones: Solicitud.Cotizaciones,
            cod_compania: '',
            dcom:0,
            url: finalUrl,
            idSolicitud: $("#HNumSolicitud").val(),
            fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
        }

        fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/registrar-cotizacion-movimiento', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=iso-8859-1',
                'x-username': $('#usuario_actual').val(),
                'x-rol': $('#rol_azman').val(),
                'x-client-ip': localStorage.getItem("ipCliente")
            },
            body: JSON.stringify(params)
        })
            .then(response => {
                if (!response.ok) {
                    if (response.status === 401 || response.status === 12030) {
                        document.location.reload(true);
                        return;
                    }
                    throw new Error('Error en la respuesta del servidor');
                }
                return response.json();
            })
            .then(data => {
                if (data.data.Estado == 'OK') {
                    $('#ModalCotizando').dialog('close');
                    $('#ModalCuadroMensaje').dialog({ title: data.data.Titulo });
                    $('#MCMIcono').attr('class', data.data.Icono);
                    $('#MCMContenedor').html(data.data.Mensaje);
                    $('#ModalCuadroMensaje').dialog('open');
                    botonModSolAceptarBloqueado = true;
                    $('#ModSolAceptarOficial').attr('class', 'botonDeshabilitado gris gris_sharp');
                }
                else if (data.data.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
                else {
                    $('#ModalCotizando').dialog('close');
                    $('#ModalCuadroMensaje').dialog({ title: data.data.Titulo });
                    $('#MCMIcono').attr('class', data.data.Icono);
                    $('#MCMContenedor').html(data.data.Mensaje);
                    $('#ModalCuadroMensaje').dialog('open');
                }
            })
            .catch(error => {
                if (error.status === 401 || error.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#ModalCotizando').dialog('close');
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al registrar la información de la solicitud.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            });

        //$.ajax({
        //    type: 'POST',
        //    url: 'CotizadorOficiales.aspx/RegistrarCotizacionMovimiento',
        //    contentType: "application/json; charset=iso-8859-1",
        //    data: $.toJSON(params),
        //    dataType: 'json',
        //    success: function (data) {

        //        if (data.d.Estado == 'OK') {

        //            $('#ModalCotizando').dialog('close');

        //            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
        //            $('#MCMIcono').attr('class', data.d.Icono);
        //            $('#MCMContenedor').html(data.d.Mensaje);
        //            $('#ModalCuadroMensaje').dialog('open');

        //            botonModSolAceptarBloqueado = true;
        //            $('#ModSolAceptarOficial').attr('class', 'botonDeshabilitado gris gris_sharp');
        //        }
        //        else if (data.d.Estado == "TOKEN") {
        //            CerrarSesionExpirada();
        //        }
        //        else {
        //            $('#ModalCotizando').dialog('close');

        //            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
        //            $('#MCMIcono').attr('class', data.d.Icono);
        //            $('#MCMContenedor').html(data.d.Mensaje);
        //            $('#ModalCuadroMensaje').dialog('open');
        //        }
        //    },
        //    error: function (XMLHttpRequest, textStatus, errorThrown) {
        //        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //            document.location.reload(true);
        //        }
        //        else {
        //            $('#ModalCotizando').dialog('close');

        //            $('#MCMIcono').attr('class', 'error');
        //            $('#MCMContenedor').html('Ha ocurrido un error al registrar la información de la solicitud.');
        //            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        //            $('#ModalCuadroMensaje').dialog('open');
        //        }
        //    }
        //});

    });

    $("#MCAOCancelar").live("click", function () {
        $("#ModalCuadroAdvertencia").dialog("close");
    });


    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Modificar Flujo*/
    $('#TabBandejaSolicitudOficial .grilla_editar_flujo').live('click', function () {
        var numCusspp = $(this).data('cusspp');
        var numSolicitud = $(this).data('solicitud');
        var numOperacion = $(this).data('operacion');

        //if (!movil) {
        //    //ini nomal
        //    GrillaEditarFlujoPC(numCusspp, numSolicitud, numOperacion);
        //    //fin normal
        //} else {

        //inicio movil
        var params = {
            numCusspp: numCusspp,
            numSolicitud: numSolicitud,
            numOperacion: numOperacion,
        }
        $.ajax({
            type: 'POST',
            url: 'BandejaFlujoCotizacionModificar.aspx/EnvioPostMovil',
            contentType: "application/json; charset=iso-8859-1",
            data: $.toJSON(params),
            dataType: 'json',
            success: function (data) {
                window.location.href = 'BandejaFlujoCotizacionModificar.aspx';
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
        //fin movil
        //}

    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Ver Flujo*/
    $('#TabBandejaSolicitudOficial .grilla_flujo').live('click', function () {

        $('#ModFlujoSolCargando').show();

        $('#ModalFlujoSolicitud').dialog('open');

        var numSolicitud = $(this).data('solicitud');

        $('#HNumSolicitud').val(numSolicitud);

        CargarTablaCotizacionMovimiento(numSolicitud);

    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Cancelar Flujo*/
    $('#ModFlujoSolCancelar').live('click', function () {
        //if (!movil) {
        //    //ini nomal
        //    $('#ModalFlujoSolicitud').dialog('close');
        //    //fin normal
        //} else {
        //    //inicio movil
        window.location.href = 'BandejaFlujoCotizacion.aspx';
        //fin movil
        //}
    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Rechazar Flujo*/
    $('#ModSolRechazarBandeja').live('click', function () {
        if ($('#HCodTipoMovimiento').val() != 7 && $('#HCodTipoMovimiento').val() != 0) {
            //if(!movil){
            RegistrarTipoMovimientoBandeja(true);
            //}
            //else {
            //    RegistrarTipoMovimientoBandejaMovil(true);
            //}
        }
    });
    //<SRI.FIN-20322_E2>

    //<SRI.INI-20322_E2>
    /* Botón Aprobar Flujo*/
    $('#ModSolAprobarBandeja').live('click', function () {
        if ($('#HCodTipoMovimiento').val() != 7 && $('#HCodTipoMovimiento').val() != 0) {
            //if (!movil) {
            //    RegistrarTipoMovimientoBandeja(false);
            //}
            //else {
            RegistrarTipoMovimientoBandejaMovil(false);
            //}
        }
    });
    //<SRI.FIN-20322_E2>

    /* Calcular monto de ACOM - Inicio */
    $("#ModSolACOM,#ModSolIndSeleccionado,#TabCotizaciones_RP input[type=radio]").live("change", function () {
        if (!botonModSolAceptarBloqueado) {
            // Validar sólo si existe el textbox de Monto ACOM (oficiales)
            if ($("#ModSolValMontoAcomAgente").length > 0) {
                if ($("#ModSolACOM").val() > 0 && $("#ModSolIndSeleccionado").val() == "S" && $("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
                    // Bloquear botón Aceptar
                    $("#ModSolMontoACOMCargando").show();
                    botonModSolAceptarBloqueado = true;
                    $("#ModSolAceptarOficial").attr("class", "botonDeshabilitado gris gris_sharp");

                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        solicitud: Solicitud.Id,
                        acom: $("#ModSolACOM").val(),
                        cotizacion: $("#TabCotizaciones_RP input[type=radio]:checked").val()
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

    /* Botón Generar Solicitud de Anticipo */
    $("#TabCotizacionCotizaciones .grilla_anticipo").live("click", function () {
        var idSolicitud = $(this).data('solicitud');
        var numAgente = $(this).data("numagente");

        var params = {
            //tokenUsuario: $("#TokenUsuario").val(),
            solicitud: idSolicitud,
            agente: numAgente
        }
        fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/validar-solicitud-anticipo', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=iso-8859-1',
                'x-username': $('#usuario_actual').val(),
                'x-rol': $('#rol_azman').val()
            },
            body: JSON.stringify(params)
        })
            .then(response => {
                if (!response.ok) {
                    if (response.status === 401 || response.status === 12030) {
                        document.location.reload(true);
                        return;
                    }
                    throw new Error("Error en la respuesta del servidor");
                }
                return response.json();
            })
            .then(data => {
                if (data.data.Respuesta.Estado == "OK") {
                    if (data.data.Respuesta.Mensaje == "C") {
                        window.location.href = "CondicionesAnticipo.aspx";
                    }
                    else if (data.data.Respuesta.Mensaje == "R") {
                        $.ajax({
                            type: "POST",
                            url: rutaGenerarReporteSolicitudAnticipo,
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: "json",
                            data: $.toJSON(params),
                            success: function (data) {
                                if (data.d.Estado == "OK") {
                                    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                                        window.location.href = "../Reportes/SolicitudAnticipoMovil.aspx";
                                    }
                                    else {
                                        var w = 800;
                                        var h = 600;
                                        var left = (screen.width / 2) - (w / 2);
                                        var top = (screen.height / 2) - (h / 2);
                                        var nuevaVentana = window.open("../Reportes/SolicitudAnticipo.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                                    }

                                    /*<SRIINI17003>*/
                                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                    $clock.countdown(selectedDate.toString());
                                    /*<SRIFIN17003>*/
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
                                    $('#MCMContenedor').html('Ha ocurrido un error al exportar la la solicitud.');
                                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                    $('#ModalCuadroMensaje').dialog('open');
                                }
                            }
                        });
                    }
                    else {
                        $("#MCMIcono").attr('class', 'error');
                        $("#MCMContenedor").html("Ha ocurrido un error al evaluar la solicitud de Anticipo.");
                        $("#ModalCuadroMensaje").dialog({ title: "Error" });
                        $("#ModalCuadroMensaje").dialog("open");
                    }
                }
                else if (data.data.Respuesta.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
                else {
                    $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                    $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            })
            .catch(error => {
                if (error.status === 401 || error.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al evaluar la solicitud de Anticipo.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            });

        //$.ajax({
        //    type: "POST",
        //    url: "CotizadorOficiales.aspx/ValidarSolicitudAnticipo",
        //    contentType: "application/json; charset=iso-8859-1",
        //    dataType: "json",
        //    data: $.toJSON(params),
        //    success: function (data) {
        //        if (data.d.Estado == "OK") {
        //            if (data.d.Mensaje == "C") {
        //                window.location.href = "CondicionesAnticipo.aspx";
        //            }
        //            else if (data.d.Mensaje == "R") {
        //                $.ajax({
        //                    type: "POST",
        //                    url: rutaGenerarReporteSolicitudAnticipo,
        //                    contentType: "application/json; charset=iso-8859-1",
        //                    dataType: "json",
        //                    data: $.toJSON(params),
        //                    success: function (data) {
        //                        if (data.d.Estado == "OK") {
        //                            if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
        //                                window.location.href = "../Reportes/SolicitudAnticipoMovil.aspx";
        //                            }
        //                            else {
        //                                var w = 800;
        //                                var h = 600;
        //                                var left = (screen.width / 2) - (w / 2);
        //                                var top = (screen.height / 2) - (h / 2);
        //                                var nuevaVentana = window.open("../Reportes/SolicitudAnticipo.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
        //                            }

        //                            /*<SRIINI17003>*/
        //                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
        //                            $clock.countdown(selectedDate.toString());
        //                            /*<SRIFIN17003>*/
        //                        }
        //                        else if (data.d.Estado == "TOKEN") {
        //                            CerrarSesionExpirada();
        //                        }
        //                        else {
        //                            $('#MCMIcono').attr('class', data.d.Icono);
        //                            $('#MCMContenedor').html(data.d.Mensaje);
        //                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
        //                            $('#ModalCuadroMensaje').dialog('open');
        //                        }
        //                    },
        //                    error: function (XMLHttpRequest, textStatus, errorThrown) {
        //                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //                            /* Sesión caducada */
        //                            document.location.reload(true);
        //                        }
        //                        else {
        //                            $('#MCMIcono').attr('class', 'error');
        //                            $('#MCMContenedor').html('Ha ocurrido un error al exportar la la solicitud.');
        //                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        //                            $('#ModalCuadroMensaje').dialog('open');
        //                        }
        //                    }
        //                });
        //            }
        //            else {
        //                $("#MCMIcono").attr('class', 'error');
        //                $("#MCMContenedor").html("Ha ocurrido un error al evaluar la solicitud de Anticipo.");
        //                $("#ModalCuadroMensaje").dialog({ title: "Error" });
        //                $("#ModalCuadroMensaje").dialog("open");
        //            }
        //        }
        //        else if (data.d.Estado == "TOKEN") {
        //            CerrarSesionExpirada();
        //        }
        //        else {
        //            $('#MCMIcono').attr('class', data.d.Icono);
        //            $('#MCMContenedor').html(data.d.Mensaje);
        //            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
        //            $('#ModalCuadroMensaje').dialog('open');
        //        }
        //    },
        //    error: function (XMLHttpRequest, textStatus, errorThrown) {
        //        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //            /* Sesión caducada */
        //            document.location.reload(true);
        //        }
        //        else {
        //            $('#MCMIcono').attr('class', 'error');
        //            $('#MCMContenedor').html('Ha ocurrido un error al evaluar la solicitud de Anticipo.');
        //            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        //            $('#ModalCuadroMensaje').dialog('open');
        //        }
        //    }
        //});

    });

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

    if ($('#ModGruFamModo_RP').val() == "M") {
        CargandoGrupoFamiliar();
    }

    if ($('#ModGruFamModo_RP').val() == "MC") {
        CargandoGrupoFamiliarCierre();
    }

    /* Cambiando la moneda */
    $("#ModSolMonedaPrimaUnica_RP").live("change", function () {
        for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
            Solicitud.Cotizaciones[i].Moneda.Id = "0";
        }
        CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
        //SelectedIndexChanged
        //$('#ModMonedaFondo_RP').trigger('change');
        //$('#ModMonedaFondo_RP').trigger('SelectedIndexChanged');

        //var params = {
        //    idMonedaFondo: $("#ModSolMonedaPrimaUnica_RP").val()
        //}

        //$.ajax({
        //    type: 'POST',
        //    url: 'Cotizador.aspx/SessionIdMonedaFondo',
        //    contentType: "application/json; charset=iso-8859-1",
        //    dataType: 'json',
        //    data: $.toJSON(params),
        //    success: function (data) {

        //        var monedafondo = data.d;
        //        if ($('#ModSolMonedaPrimaUnica_RP').val() == monedafondo.toString()) {

        //            for (i = 0; i < Solicitud.Cotizaciones.length; i++) {

        //                Solicitud.Cotizaciones[i].Moneda.Id = "0";
        //            }
        //            CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val());
        //            //window.location.href = "DireccionAfiliado.aspx";
        //        }

        //    },
        //    error: function (XMLHttpRequest, textStatus, errorThrown) {
        //        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //            document.location.reload(true);
        //        }
        //        else {
        //            $('#MCMIcono').attr('class', 'error');
        //            $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
        //            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        //            $('#ModalCuadroMensaje').dialog('open');
        //        }
        //        $('#ModalDireccion').dialog('close');
        //    }
        //});

    });

    $("#ModSolTemporalidad_RP").live("change", function () {
        CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
    });


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




    //<INIGTI_753>
    $('#ModSolTipoPlan_RP').live('change', function () {
        // Obtener la lista de beneficiarios
        for (i = 0; i < $('#TabBeneficiarios_RP tbody tr').length; i++) {
            if ($('#ModSolTipoPlan_RP').val() == "01") {
                var idParentesco = $("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").data("parentesco");
                //<INIGTI_753>
                if (idParentesco != "80") {
                    $('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').prop("checked", "checked");
                    $('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').removeAttr("disabled");

                    if (idParentesco == "10") {
                        $("#HConyuge").val("TRUE");
                    } else {
                        $("#HConyuge").val("FALSE");
                    }
                }

                //if (i == 1) {

                //} else {
                //    $("#HConyuge").val("FALSE");
                //}
                //<FINGTI_753>
            } else {
                var idParentesco = $("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").data("parentesco");
                if (idParentesco != "80") {
                    $('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').prop("checked", "");
                    $('#TabBeneficiarios_RP tbody tr:eq(' + i + ') input').attr("disabled", "true");
                }
                //<INIGTI_753>
                $("#HConyuge").val("FALSE");
                //<FINGTI_753>
            }

            $('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');
        }

        if ($("#HConyuge").val() == "TRUE") {
            if ($('#TabBeneficiarios_RP input[type=checkbox]:checked').length != 2) {
                $("#HConyuge").val("FALSE");
            }
        }

        // Lista de cotizaciones
        CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
        //
    });

    //<INIGTI_753>
    $('#ManSolPes1').live('click', function () {

        // Lista de cotizaciones
        //<INIGTI_7012>
        if ($('#ModSolModo').val() == "CERRAR") {
            CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());
        } else {
            //<INIGTI_7012>
            //Si no selecciona ningun familiar adicional, entonces es tipo de plan (Individual)
            if ($('#TabBeneficiarios_RP input[type=checkbox]:checked').length == 1) {
                $('#ModSolTipoPlan_RP').val("02");
                $('#TexModSolTipoPlan_RP').html($('#ModSolTipoPlan_RP').find(':selected').text());
            }
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

            CargarTablaCotizaciones_RP(Solicitud.Cotizaciones, $('#ModSolTemporalidad_RP').val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());//<INIGTI_753>
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

        //handleAuthClick();

        $('#ModSolAgregarArchivos').attr('disabled', 'disabled');
        $('#ModSolAgregarArchivos').attr('class', 'botonDeshabilitado gris gris_sharp');

        $('#ModSolEnviarEvaluacion').attr('disabled', 'disabled');
        $('#ModSolEnviarEvaluacion').attr('class', 'botonDeshabilitado gris gris_sharp');

        //var params = {
        //    tokenUsuario: $('#TokenUsuario').val()
        //}

        //$.ajax({
        //type: "POST",
        //url: "SeleccionSolicitud.aspx/ValidarAgregarArchivo",
        //contentType: "application/json; charset=iso-8859-1",
        //data: $.toJSON(params),
        //dataType: "json",
        //success: function (data) {
        //
        //if (data.d.Estado == "OK") {
        $("#divCargar").show();
        //handleClientLoad();

        //
        ObtenerArchivos($("#HNroSolicitud").val(), true);

        // Deslizar la pantalla hacia los datos de simulación
        $('html,body').animate({
            scrollTop: $('#ModSolEnviarEvaluacion').offset().top
        }, 'slow');

        //} 
        // else if (data.d.Estado == "ERROR") {
        //    $("#MCMIcono").attr("class", "error");
        //    $("#MCMContenedor").html(data.d.Mensaje);
        //    $("#ModalCuadroMensaje").dialog({ title: "Error" });
        //    $("#ModalCuadroMensaje").dialog("open");

        //    $('#ModSolAgregarArchivos').removeAttr('disabled');
        //    $('#ModSolAgregarArchivos').attr('class', 'boton darkblue sharp');

        //}
        //},
        //error: function (XMLHttpRequest, textStatus, errorThrown) {
        //if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
        //    document.location.reload(true);
        //}
        //else {
        //    $("#MCMIcono").attr("class", "error");
        //    $("#MCMContenedor").html("Ha ocurrido un error al validar solicitud.");
        //    $("#ModalCuadroMensaje").dialog({ title: "Error" });
        //    $("#ModalCuadroMensaje").dialog("open");
        //}
        //}
        //});

        return false;
    });

    $("#ModSolEnviarEvaluacion").live("click", function () {
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

    //Evaluación, Observado, Aprobado
    //
    if ($('#HEstado').val() == "4" || $('#HEstado').val() == "5" || $('#HEstado').val() == "6") {
        $("#divCargar").show();
        //handleClientLoad();
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

    //<INI.GTI_26697>
    $('#LinkConsentimientoAsesoria_RPP').live('click', function (e) {
        $('#MCConstmAsesIcono_RPP').attr('class', 'advertencia');
        $('#MCConstmAsesContenedor_RPP').html('<div class="alerta-agente-contenido"> <p>Usted va a enviar el enlace de <b>Consentimiento y protección de datos personales</b> al cliente <span class="resaltado">' + $('#Nombres_RP').val() + ' ' + $('#ApellidoPaterno_RP').val() + ' ' + $('#ApellidoMaterno_RP').val() + '</span> identificado con el <span class="resaltado">' + $('#TipoDocumento_RP').find(':selected').text() + '</span> <span class="resaltado">' + $('#NumeroDocumento_RP').val() + '</span> al siguiente correo electrónico: <span class="resaltado">' + $('#CorreoElectronico_RP').val() + '</span> </p> <p> Verifique que los datos son correctos, en caso haya un error, por favor modifique los datos en el vtiger, luego actualice la página en el Cotizador Web de Rentas y vuelva a intentarlo.</p></div>');
        $('#ModalConsentimientoAsesoria_RPP').dialog({ title: 'Consentimiento y protección de datos personales' });
        $('#ModalConsentimientoAsesoria_RPP').dialog('open');
    });

    /* Botón Cancelar Consentimiento Asesoria */
    $('#MCConstmAsesCancelar_RPP').live('click', function () {
        $('#ModalConsentimientoAsesoria_RPP').dialog('close');
    });

    $("#MCConstmAsesEnviar_RPP").live("click", function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Enviando email con el enlace al Consentimiento de Asesoría, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            nombres: $('#Nombres_RP').val(),
            tipoDocumento: $('#TipoDocumento_RP').val(),
            numeroDocumento: $("#NumeroDocumento_RP").val(),
            correo: $("#CorreoElectronico_RP").val(),
            cuspp: $("#HCUSPP_RP").val(),
            token: $("#HToken_RPP").val(),
            apellidoPaterno: $('#ApellidoPaterno_RP').val(),
            apellidoMaterno: $('#ApellidoMaterno_RP').val(),
            sexo: $('#Sexo_RP').val(),
            fechaNacimiento: $('#FechaNacimiento_RP').val(),
            telefono: $('#Telefono_RP').val(),
            celular: $('#Celular_RP').val(),
            idConsentimientoAsesoria: $('#HidConsentimientoAsesoria').val(),
            indConsentimiento: $('#HindConsentimiento').val()
        }

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/EnviarConsentimientoAsesoria",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {

                    $("#ModalConsentimientoAsesoria_RPP").dialog("close");

                    // Mostrar mensaje de éxito
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else {
                    $("#ModalConsentimientoAsesoria_RPP").dialog("close");

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

    $("#PlantillaConsentimientoAsesoria_RPP").live("click", function (e) {
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
                    $("#ModalCuadroMensaje").dialog({ ModConsentimientoAsesoriaCargando_RPP: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
                $("#ModalCotizando").dialog("close");
            }
        });
    });

    $("#ReenviarPlantillaConsentimientoAsesoria_RPP").live("click", function () {
        $("#MRCAIcono").attr("class", "advertencia");
        $("#MRCAContenedor").html("<div class=\"alerta-agente-contenido\"><p>Usted va a enviar el formato firmado de consentimiento de asesoría al cliente <span class=\"resaltado\">" + $("#Nombres_RP").val() + " " + $("#ApellidoPaterno_RP").val() + " " + $("#ApellidoMaterno_RP").val() + "</span> al siguiente correo electrónico: <span class=\"resaltado\">" + $("#CorreoElectronico_RP").val() + "</span></p><p>Verifique que los datos sean correctos antes de realizar el envío.</p></div>");
        $("#ModalReenvioConsentimientoAsesoria").dialog({ title: "Reenviar Consentimiento de Asesoría" });
        $("#ModalReenvioConsentimientoAsesoria").dialog("open");
    });

    $("#MRCACancelar_RPP").live("click", function () {
        $("#ModalReenvioConsentimientoAsesoria").dialog("close");
    });

    $("#MRCAEnviar_RPP").live("click", function (e) {
        $("#ModalReenvioConsentimientoAsesoria").dialog("close");
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Reenviando el Formato de Consentimiento, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            cuspp: $("#CUSPP_RP"),
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

    $('#ModGruFamPEP_RP').change(function () {
        if ($('#ModGruFamPEP_RP').val() == 'S') {
            $('#seccionPEP').show();
            CargarTablaPersonasVinculadas_RP($('#ModGruFamsoltud').val())
        }
        else {
            $('#seccionPEP').hide();
        }
    });
    //<INI.GTI_26697>

    /* Botón Eliminar */
    $('#TabGrupoFamiliar .grilla_eliminar').live('click', function () {
        idGrupoFamiliar = $(this).data('grupofamiliar');
        $('#MCATablaEliminar').val('GF');
        $('#MCAIcono').attr('class', 'advertencia');
        $('#MCAContenedor').html('¿Está seguro de eliminar el beneficiario?');
        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
        $('#ModalCuadroAdvertencia').dialog('open');
        return false;
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

function MostrarModalOk(mensaje) {

    $('#ModalCuadroMensaje').dialog({
        autoOpen: false,
        resizable: false,
        width: 500,
        minHeight: 130,
        show: "fade",
        hide: "explode",
        modal: true
    });

    $('#MCMIcono').attr('class', 'info');
    $('#MCMContenedor').html(mensaje);
    $('#ModalCuadroMensaje').dialog({ title: 'Información' });
    $('#ModalCuadroMensaje').dialog('open');
};
//<FIN.GTI_7012_3>

$(".grilla_valacom").live("click", function () {
    var boton = $(this);

    boton.attr("class", "grilla_boton grilla_cargando");

    var idSolicitud = boton.data("operacion");
    var numAgente = boton.data("numagente");
    var valor = boton.data("valor");
    var valor2 = (valor == "S") ? "N" : "S";
    var encendido = (valor == "S") ? "on" : "off";
    var encendido2 = (valor == "S") ? "off" : "on";

    var params = {
        tokenUsuario: $("#TokenUsuario").val(),
        idSolicitud: idSolicitud,
        idAgente: numAgente,
        tipoValidacion: "A",
        valor: valor
    }

    $.ajax({
        type: "POST",
        url: "CotizadorOficiales.aspx/ModificaValidacionesrSolicitud",
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                boton.attr("class", "grilla_boton grilla_valacom grilla_switch_" + encendido);
                boton.data("valor", valor2);
                boton.attr("data-valor", valor2);

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

                boton.attr("class", "grilla_boton grilla_valacom grilla_switch_" + encendido2);
                boton.data("valor", valor);
                boton.attr("data-valor", valor);
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

$(".grilla_valdtra").live("click", function () {
    var boton = $(this);

    boton.attr("class", "grilla_boton grilla_cargando");
    //boton.attr("title", "Por favor espere un momento...");

    var idSolicitud = boton.data("operacion");
    var numAgente = boton.data("numagente");
    var valor = boton.data("valor");
    var valor2 = (valor == "S") ? "N" : "S";
    var encendido = (valor == "S") ? "on" : "off";
    var encendido2 = (valor == "S") ? "off" : "on";

    var params = {
        tokenUsuario: $("#TokenUsuario").val(),
        idSolicitud: idSolicitud,
        idAgente: numAgente,
        tipoValidacion: "T",
        valor: valor
    }

    $.ajax({
        type: "POST",
        url: "CotizadorOficiales.aspx/ModificaValidacionesrSolicitud",
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                boton.attr("class", "grilla_boton grilla_valdtra grilla_switch_" + encendido);
                boton.data("valor", valor2);
                boton.attr("data-valor", valor2);

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

                boton.attr("class", "grilla_boton grilla_valdtra grilla_switch_" + encendido2);
                boton.data("valor", valor);
                boton.attr("data-valor", valor);
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

    //<INIGTI_7012>
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

    //<FINGTI_7012>

    $('#ModGruFamFechaInvalidez_RP').attr('disabled', 'disabled');
    $('#ModGruFamFechaInvalidez_RP').addClass('formCalendarReadOnly');
}

function LimpiarFormularioSolicitud() {
    $('#ModSolNroSolicitud_RP').removeClass('formTextboxError');
    $('#ModSolTipoCambio').removeClass('formTextboxError');
    $('#ConModSolTipoPension').removeClass('formComboboxErrorContenedor');
    $('#ConModSolCategoria').removeClass('formComboboxErrorContenedor');
    $('#ModSolFechaDevengue_RP').removeClass('formTextboxError formCalendarError');
    $('#ModSolFecUltActualizacion').removeClass('formTextboxError formCalendarError');
    $('#ModSolFechaRecepcion').removeClass('formTextboxError formCalendarError');
    $('#ModSolFechaPlazoAFP').removeClass('formTextboxError formCalendarError');
    $('#ModSolSaldoCIC_RP').removeClass('formTextboxError');
    $('#ConModSolFactorTasa').removeClass('formComboboxErrorContenedor');
    $('#ModSolFechaCotizacion_RP').removeClass('formTextboxError formCalendarError');
    $('#ModSolFechaSolicitudPension').removeClass('formTextboxError formCalendarError');
    $('#ModSolACOM').removeClass('formTextboxError');
    $('#ModSolDCOM_RP').removeClass('formTextboxError');

    //<INIGTI_753>
    $('#ModSolPrimaUnica_RP').removeClass('formTextboxError');
    //<FINGTI_753>

    $('#ModSolNroSolicitud_RP').val('');
    $('#ModSolTipoCambio').val('');
    $('#ModSolTipoPension').val('V');
    $('#TexModSolTipoPension').html($('#ModSolTipoPension').find(':selected').text());
    $('#ModSolCategoria').val($('#HCategoria_RP').val());
    $('#TexModSolCategoria').html($('#ModSolCategoria').find(':selected').text());
    //$('#ModSolFechaDevengue_RP').val(''); //ARMV
    $('#ModSolFecUltActualizacion').val('');
    $('#ModSolFechaRecepcion').val('');
    $('#ModSolFechaPlazoAFP').val('');
    $('#ModSolSaldoCIC_RP').val($('#SaldoCIC_RP').val());
    $('#ModSolFactorTasa').val('0');
    $('#TexModSolFactorTasa').html($('#ModSolFactorTasa').find(':selected').text());
    //$('#ModSolFechaCotizacion_RP').val('');
    $('#ModSolFechaSolicitudPension').val('');
    $('#ModSolACOM').val('');
    $('#ModSolDCOM_RP').val('');

    botonModSolAceptarBloqueado = false;
    $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');

    $('#ModSolMontoCIA').val('');
    $('#ModSolPensionCIA').val('');
    $('#ModSolPensionCIAMO').val('');
    $('#ModSolTasaAFP').val('');
    $('#ModSolMontoAFP').val('');
    $('#ModSolPensionAFP').val('');
    /*<SRI.INI-20322>*/
    $('#ModTasaVenta').val('');
    $('#ModTasaVentaSbs').val('');
    /*<SRI.FIN-20322>*/

    $('#RangoInversion_RP').val('');
    $('#CentroLaboral_RP').val('');

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

//<SRI.INI_20322_E2>
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
//<SRI.FIN_20322_E2>

//<SRI.INI-20322_E2>
function LimpiarFormularioSolicitudOficial() {
    $('#ModSolACOM').removeClass('formTextboxError');
    $('#ModSolDCOM_RP').removeClass('formTextboxError');
    $('#ConModSolIndSeleccionado').removeClass('formComboboxErrorContenedor');
    $('#ModSolValMontoAcomAgente').removeClass('formTextboxError');

    $('#ModSolNroSolicitud_RP').html('');
    $('#ModSolNroMeller').html('');
    $('#ModSolACOM').val('');
    $('#ModSolDCOM_RP').val('');
    $('#ModSolIndSeleccionado').val('N');
    $('#TexModSolIndSeleccionado').html($('#ModSolIndSeleccionado').find(':selected').text());
    $('#ModSolValMontoAcomAgente').val('');
    $('#ModSolCategoria').val('0');
    $('#TexModSolCategoria').html($('#ModSolCategoria').find(':selected').text());
    $('#ModSolMontoCIC').html('');
    $('#ModFecCierre').html('');
    $('#ModSolCussp').html('');
    $('#ModSolNomAfiliado').html('');
    $('#ModSolNumAgente').html('');
    $('#ModSolNomAgente').html('');

    botonModSolAceptarBloqueado = false;
    $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');

    $('#TablaCotizacionesCargando_RP').hide();
}
//<SRI.FIN-20322_E2>

//<SRI.INI-20322_E2>
function RegistrarTipoMovimientoBandeja(rechazo) {
    $('#MCIcono').attr('class', 'cargando');
    $('#MCContenedor').html('Generando Flujo Solicitud, por favor espere un momento...');
    $('#ModalCotizando').dialog({ title: 'Registrando' });
    $('#ModalCotizando').dialog('open');

    var params = {
        listaCotizaciones: Solicitud.Cotizaciones,
        rechazo: rechazo
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacion.aspx/RegistrarCotizacionMovimiento',
        contentType: "application/json; charset=iso-8859-1",
        data: $.toJSON(params),
        dataType: 'json',
        success: function (data) {

            if (data.d.Estado == 'OK') {

                $('#ModalCotizando').dialog('close');

                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                $('#ModalCuadroMensaje').dialog('open');

                $('#ModSolRechazarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');
                $('#ModSolAprobarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');

                CargarTablaBandejaSolicitudesOficiales(0, 0, 0);

                $('#ModalSolicitud').dialog('close');

            }
            else if (data.d.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
            else {
                $('#ModalCotizando').dialog('close');

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

                $('#ModalCotizando').dialog('close');

                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al registrar la información de TRA.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
            $('#ModalSolicitud').dialog('close');
        }
    });
}
//<SRI.FIN-20322_E2>

//<SRI.INI-20322_E2>
function RegistrarTipoMovimientoBandejaMovil(rechazo) {

    $('#MCIcono').attr('class', 'cargando');
    $('#MCContenedor').html('Generando Flujo Solicitud, por favor espere un momento...');
    $('#ModalCotizando').dialog({ title: 'Registrando' });
    $('#ModalCotizando').dialog('open');

    var params = {
        acom: $("#ModSolACOM").val(),
        montoAcom: $("#ModSolValMontoAcomAgente").val(),
        listaCotizaciones: Solicitud.Cotizaciones,
        rechazo: rechazo
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacionModificar.aspx/RegistrarCotizacionMovimiento',
        contentType: "application/json; charset=iso-8859-1",
        data: $.toJSON(params),
        dataType: 'json',
        success: function (data) {

            if (data.d.Estado == 'OK') {

                $('#ModalCotizando').dialog('close');

                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                $('#MCMIcono').attr('class', data.d.Icono);
                $('#MCMContenedor').html(data.d.Mensaje);
                $('#ModalCuadroMensaje').dialog('open');

                $('#ModSolRechazarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');
                $('#ModSolAprobarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');

            }
            else if (data.d.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
            else {
                $('#ModalCotizando').dialog('close');

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

                $('#ModalCotizando').dialog('close');

                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al registrar la información de TRA.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
            //$('#ModalSolicitud').dialog('close');
        }
    });
}
//<SRI.FIN-20322_E2>

//<SRI.INI-20322_E2>
function GrillaEditarFlujoPC(numCusspp, numSolicitud, numOperacion) {

    $('#ModSolModo').val('M');

    $('#ModSolCargando').show();

    LimpiarFormularioSolicitudOficial();

    $('#ModalSolicitud').dialog('open');

    $('#HCusspp').val(numCusspp);
    $('#HNumSolicitud').val(numSolicitud);
    $('#HNumOperacion').val(numOperacion);

    var params = {
        numSolicitud: numSolicitud
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacion.aspx/ObtenerDatosSolicitudEscenario',
        contentType: "application/json; charset=iso-8859-1",
        data: $.toJSON(params),
        dataType: 'json',
        success: function (data) {
            SolicitudEscenario = data.d;

            $('#ModSolNroSolicitud_RP').val(SolicitudEscenario.NumSolicitud);
            $('#ModSolNroMeller').val(SolicitudEscenario.NumOperacion);
            $('#ModSolACOM').val(formatearMonto(SolicitudEscenario.PjeAumentoComision));
            $('#ModSolDCOM_RP').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
            $('#ModSolIndSeleccionado').val(SolicitudEscenario.IndEstadoSeleccion);
            $('#ModSolValMontoAcomAgente').val(formatearMonto2(SolicitudEscenario.ValMtoAgenteAcom));
            //$('#ModSolIndCondicionEspecial').val(SolicitudEscenario.IndCondicionEspecial);
            //$('#ModSolIndCondicionAprobado').val(SolicitudEscenario.IndAprueba);
            $('#ModSolMontoCIC').val(formatearMonto(SolicitudEscenario.ValTotalCic));
            $('#ModFecCierre').val(new Date(+SolicitudEscenario.FecCierre.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            //$('#ModSolCategoria').val(SolicitudEscenario.Categoria.Id.toString()).change();
            //$('#ModSolFechaRegistro').val(new Date(+SolicitudEscenario.FecRegistroEscenario.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            $('#ModSolCussp').val(SolicitudEscenario.Afiliado.CUSPP);
            $('#ModSolNomAfiliado').val(SolicitudEscenario.Afiliado.NombreEmpresa);
            $('#ModSolNumAgente').val(SolicitudEscenario.Agente.Id);
            $('#ModSolNomAgente').val(SolicitudEscenario.Agente.Nombre);

            $('#ModSolCargando').fadeOut();
            $('#ModSolACOM').focus();
            $('#ModSolDCOM_RP').focus();


            var params = {
                numCuspp: numCusspp,
                numSolicitud: numSolicitud
            }
            $.ajax({
                type: 'POST',
                url: 'BandejaFlujoCotizacion.aspx/ObtenerDatosSolicitud',
                contentType: "application/json; charset=iso-8859-1",
                data: $.toJSON(params),
                dataType: 'json',
                success: function (data) {
                    Solicitud = data.d;

                    CargarTablaBeneficiariosOficialBandeja(Solicitud.Beneficiarios);

                    $('#HCodTipoMovimiento').val(Solicitud.TipoMovimiento.Id);

                    if (Solicitud.TipoMovimiento.Id == 7 || Solicitud.TipoMovimiento.Id == 0) {
                        botonModSolAceptarBloqueado = true;
                        $('#ModSolAprobarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');
                        $('#ModSolRechazarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');
                    }
                    else {
                        botonModSolAceptarBloqueado = false;
                        $('#ModSolAprobarBandeja').attr('class', 'boton darkblue sharp');
                        $('#ModSolRechazarBandeja').attr('class', 'boton darkblue sharp');
                    }

                    var params = {
                        idTipoPension: Solicitud.TipoPension.Id
                    }
                    $.ajax({
                        type: 'POST',
                        url: 'BandejaFlujoCotizacion.aspx/CargarComboProductos',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: $.toJSON(params),
                        success: function (data) {

                            $('#ModSolACOM').attr('readonly', true);
                            $('#ModSolDCOM_RP').attr('readonly', true);
                            $('#ModSolIndSeleccionado').attr('readonly', true);
                            $('#ModSolValMontoAcomAgente').attr('readonly', true);

                            CargarTablaCotizacionesOficialesBandeja(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, false);

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
                                $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de productos.');
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
                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                    $('#ModalSolicitud').dialog('close');
                }
            });



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
//<SRI.FIN-20322_E2>

//<SRI.INI-20322_E2>
function GrillaEditarFlujoMovil(numCusspp, numSolicitud, numOperacion) {

    $('#ModSolModo').val('M');

    $('#ModSolCargando').show();

    LimpiarFormularioSolicitudOficial();

    //$('#ModalSolicitud').dialog('open');

    $('#HCusspp').val(numCusspp);
    $('#HNumSolicitud').val(numSolicitud);
    $('#HNumOperacion').val(numOperacion);


    var params = {
        numSolicitud: numSolicitud
    }
    $.ajax({
        type: 'POST',
        url: 'BandejaFlujoCotizacionModificar.aspx/ObtenerDatosSolicitudEscenario',
        contentType: "application/json; charset=iso-8859-1",
        data: $.toJSON(params),
        dataType: 'json',
        success: function (data) {
            SolicitudEscenario = data.d;

            $('#ModSolNroSolicitud_RP').val(SolicitudEscenario.NumSolicitud);
            $('#ModSolNroMeller').val(SolicitudEscenario.NumOperacion);
            $('#ModSolACOM').val(formatearMonto(SolicitudEscenario.PjeAumentoComision));
            $('#ModSolDCOM_RP').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
            $('#ModSolIndSeleccionado').val(SolicitudEscenario.IndEstadoSeleccion);
            $('#ModSolValMontoAcomAgente').val(formatearMonto2(SolicitudEscenario.ValMtoAgenteAcom));
            //$('#ModSolIndCondicionEspecial').val(SolicitudEscenario.IndCondicionEspecial);
            //$('#ModSolIndCondicionAprobado').val(SolicitudEscenario.IndAprueba);
            $('#ModSolMontoCIC').val(formatearMonto(SolicitudEscenario.ValTotalCic));
            $('#ModFecCierre').val(new Date(+SolicitudEscenario.FecCierre.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            //$('#ModSolCategoria').val(SolicitudEscenario.Categoria.Id.toString()).change();
            //$('#ModSolFechaRegistro').val(new Date(+SolicitudEscenario.FecRegistroEscenario.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            $('#ModSolCussp').val(SolicitudEscenario.Afiliado.CUSPP);
            $('#ModSolNomAfiliado').val(SolicitudEscenario.Afiliado.NombreEmpresa);
            $('#ModSolNumAgente').val(SolicitudEscenario.Agente.Id);
            $('#ModSolNomAgente').val(SolicitudEscenario.Agente.Nombre);

            $('#ModSolCargando').fadeOut();
            //$('#ModSolACOM').focus();
            //$('#ModSolDCOM_RP').focus();


            var params = {
                numCuspp: numCusspp,
                numSolicitud: numSolicitud
            }
            $.ajax({
                type: 'POST',
                url: 'BandejaFlujoCotizacionModificar.aspx/ObtenerDatosSolicitud',
                contentType: "application/json; charset=iso-8859-1",
                data: $.toJSON(params),
                dataType: 'json',
                success: function (data) {
                    Solicitud = data.d;

                    CargarTablaBeneficiariosOficialBandejaMovil(Solicitud.Beneficiarios, numCusspp);

                    $('#HCodTipoMovimiento').val(Solicitud.TipoMovimiento.Id);

                    if (Solicitud.TipoMovimiento.Id == 7 || Solicitud.TipoMovimiento.Id == 0) {
                        botonModSolAceptarBloqueado = true;
                        $('#ModSolAprobarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');
                        $('#ModSolRechazarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');
                    }
                    else {
                        botonModSolAceptarBloqueado = false;
                        $('#ModSolAprobarBandeja').attr('class', 'boton darkblue sharp');
                        $('#ModSolRechazarBandeja').attr('class', 'boton darkblue sharp');
                    }

                    var params = {
                        idTipoPension: Solicitud.TipoPension.Id
                    }
                    $.ajax({
                        type: 'POST',
                        url: 'BandejaFlujoCotizacionModificar.aspx/CargarComboProductos',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: $.toJSON(params),
                        success: function (data) {

                            $('#ModSolACOM').attr('readonly', true);
                            $('#ModSolDCOM_RP').attr('readonly', true);
                            $('#ModSolIndSeleccionado').attr('readonly', true);
                            $('#ModSolValMontoAcomAgente').attr('readonly', true);

                            CargarTablaCotizacionesOficialesBandejaMovil(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

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
                                $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de productos.');
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
                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                    $('#ModalSolicitud').dialog('close');
                }
            });



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
//<SRI.FIN-20322_E2>/* Determinar si se trata de un dispositivo móvil */


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

$("#chkTodasSolicitudes").live("change", function () {
    CargarTablaSolicitudes_RP();
})

//<INI.GTI_7012_3>

//<FIN.GTI_7012_3>

function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

const validarPermisoCWRV = (listaPermisos, permiso) => listaPermisos.some(o => o.IdAzman === permiso && o.Activa);