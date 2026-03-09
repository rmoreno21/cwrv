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

var ParametroEspecial;
var ParametroEspecialNew;
var modificaEspecial = false;
var listaCuotasTra;

//Variable para bloquear Botones de Gestion de Ventas
var botoneraBloqueadoGestionVentas = false;
var opcionesSistema = localStorage.getItem("opcionesSistema");
opcionesSistema = opcionesSistema ? JSON.parse(opcionesSistema) : []


$(document).ready(function () {

    var permisoBusAfiExaminarSolicitud = ($("#PerBusAfiExaminarSolicitud").val() == "1" ? true : false);
    var permisoBusAfiBuscar = ($("#PerBusAfiBuscar").val() == "1" ? true : false);
    var permisoGuardar = ($("#PerGuardar").val() == "1" ? true : false);
    var permisoNuevaDireccion = ($("#PerNuevaDireccion").val() == "1" ? true : false);
    var permisoNuevoBeneficiario = ($("#PerNuevoBeneficiario").val() == "1" ? true : false);
    var permisoNuevaSolicitud = ($("#PerNuevaSolicitud").val() == "1" ? true : false);

    var permisoGrabarTra = ($("#HPerGrabar").val() == "1" ? true : false);

    permisoRechazarSolicitud = ($("#PerRechazarSolicitud").val() == "1" ? true : false);

    var idSolicitudTmp;
    var fechaCotizacionTmp;

    /*******************\
    |*    GENERALES    *|
    \*******************/

    /* Controlar backspace */
    $(document).keydown(function (e) {
        var element = e.target.nodeName.toLowerCase();
        if ((element != 'input' && element != 'textarea') || $(e.target).attr('readonly')) {
            if (e.keyCode === 8) {
                e.preventDefault();
            }
        }
    });

    /*******************\
    |*      LOGIN      *|
    \*******************/

    /* INICIO DE SESIÓN */
    /* Botón Iniciar Sesión */
    $('#IniSesion').live('click', function () {
        if (!botonesBloqueados) {
            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            $('#Usuario').removeClass('formTextboxLoginError');
            $('#Contrasenha').removeClass('formTextboxLoginError');

            // Usuario
            var usuario = true;
            if ($.trim($('#Usuario').val()).length == 0) {
                errores.push('Debe ingresar un <strong>Usuario</strong>.');
                $('#Usuario').addClass('formTextboxLoginError');
                $("input[name^='defaultvalue-clone']:eq(0)").addClass('formTextboxLoginError');
                usuario = false;
            }

            // Contraseña
            var contrasenha = true;
            if ($.trim($('#Contrasenha').val()).length == 0) {
                errores.push('Debe ingresar una <strong>Contraseña</strong>.');
                $('#Contrasenha').addClass('formTextboxLoginError');
                $("input[name^='defaultvalue-clone']:eq(1)").addClass('formTextboxLoginError');
                contrasenha = false;
            }

            esCorrecto = usuario & contrasenha;

            if (esCorrecto) {
                botonesBloqueados = true;
                $(this).attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            else {
                if ($(document).width() >= 1000) {
                    $('#MCMIcono').attr('class', 'validacion');
                    $('#MCMContenedor').html(formatearError(errores));
                    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                else {
                    $("#MensajeMovil").html("<b>Por favor valide:</b><br />" + formatearError(errores));
                    $('#MensajeMovil').attr("class", "validacion_movil");
                    $("#MensajeMovil").fadeIn();
                }
                return false;
            }
        }
    });


    /*******************\
    |*    COTIZADOR    *|
    \*******************/

    //<GTI.INI-29372>
    if ($('#hdnMostrarAporteAdicional').val() == "1") {
        $('#Pes4').show();
    }
    else {
        $('#Pes4').hide();
    }

    $("#ModalAdvertenciaAporte").hide();

    $('#pnlControlAporte #btnEliminarAporte').live('click', function () {
        $('#MAAIcono').attr('class', 'advertencia');
        $('#MAAContenedor').html('¿Está seguro de eliminar la información de aporte adicional?');
        $('#ModalAdvertenciaAporte').dialog({ title: 'Confirmación' });
        $('#ModalAdvertenciaAporte').dialog('open');
        return false;
    });

    $('#MAACancelar').live('click', function () {
        $('#ModalAdvertenciaAporte').dialog('close');
    });
    //<GTI.FIN-29372>

    /* Pestañas del Cotizador */
    /* Cambiar de pestaña */
    $('#Pestanhas li').live('click', function () {
        $('#Pestanhas li').attr('class', '');
        $('#Pestanhas div[id^=Pestanha]').hide();
        $('#Pestanha' + $(this).data('pestanha')).show();
        $(this).attr('class', 'seleccionado');

        if ($(this).data('pestanha') != 1) {
            $('#BusAfiBuscar,#BusAfiExaminarSolicitud').attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#BusAfiNroSolicitud,#BusAfiCUSPP').attr('readonly', true);
            $('#BusAfiNroSolicitud,#BusAfiCUSPP').addClass('formTextboxReadOnly');
            botonBusAfiBuscarBloqueado = true;
        }
        else {
            if (permisoBusAfiExaminarSolicitud) $('#BusAfiExaminarSolicitud').attr('class', 'boton darkblue sharp');
            if (permisoBusAfiBuscar) $('#BusAfiBuscar').attr('class', 'boton darkblue sharp');
            $('#BusAfiNroSolicitud,#BusAfiCUSPP').attr('readonly', false);
            $('#BusAfiNroSolicitud,#BusAfiCUSPP').removeClass('formTextboxReadOnly');
            botonBusAfiBuscarBloqueado = false;
        }
    });


    /* BÚSQUEDA DE AFILIADOS */
    var BusAfiApellidoPaterno;
    var BusAfiApellidoMaterno;
    var BusAfiNombres;
    /* Botón Examinar Solicitud */
    $('#BusAfiExaminarSolicitud').live('click', function (e) {
        if (!botonBusAfiBuscarBloqueado && permisoBusAfiExaminarSolicitud) {
            botonesBloqueados = false;
            LimpiarFormularioBusquedaAfiliados();
            $('#ModalBusquedaAfiliados').dialog('open');
            $('#ModBusAfiApellidoPaterno').focus();
        }
        else {
            return false;
        }
    });

    /* Botón Buscar Afiliado */
    $('#ModBusAfiBuscar').live('click', function (e) {
        if (!botonesBloqueados && permisoBusAfiExaminarSolicitud) {
            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            $("#ModBusAfiApellidoPaterno").removeClass("formTextboxError");
            $("#ModBusAfiApellidoMaterno").removeClass("formTextboxError");
            $("#ModBusAfiNombres").removeClass("formTextboxError");

            // Apellido Paterno
            var apellidoPaterno = true;
            var eApellidoPaterno = false;
            if ($.trim($("#ModBusAfiApellidoPaterno").val()).length > 0) {
                eApellidoPaterno = true;
                if ($.trim($("#ModBusAfiApellidoPaterno").val()).length < 2) {
                    errores.push("El campo <strong>Apellido Paterno</strong> debe contener al menos 2 caracteres.");
                    apellidoPaterno = false;
                }
            }

            // Apellido Materno
            var apellidoMaterno = true;
            var eApellidoMaterno = false;
            if ($.trim($("#ModBusAfiApellidoMaterno").val()).length > 0) {
                eApellidoMaterno = true;
                if ($.trim($("#ModBusAfiApellidoMaterno").val()).length < 2) {
                    errores.push("El campo <strong>Apellido Materno</strong> debe contener al menos 2 caracteres.");
                    apellidoMaterno = false;
                }
            }

            // Nombres
            var nombres = true;
            var eNombres = false;
            if ($.trim($("#ModBusAfiNombres").val()).length > 0) {
                eNombres = true;
                if ($.trim($("#ModBusAfiNombres").val()).length < 2) {
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

            if (!apellidoPaterno) $('#ModBusAfiApellidoPaterno').addClass('formTextboxError');
            if (!apellidoMaterno) $('#ModBusAfiApellidoMaterno').addClass('formTextboxError');
            if (!nombres) $('#ModBusAfiNombres').addClass('formTextboxError');

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & criterioMinimo;

            if (esCorrecto) {
                BusAfiApellidoPaterno = $('#ModBusAfiApellidoPaterno').val();
                BusAfiApellidoMaterno = $('#ModBusAfiApellidoMaterno').val();
                BusAfiNombres = $('#ModBusAfiNombres').val();

                $('#TabAfiliadosIndicePagina').val(1);
                $('#TabAfiliadosColumnaOrdenar').val(1);
                $('#TabAfiliadosDireccionOrdenar').val('A');

                CargarTablaAfiliados(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
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
        //<INIGTI_7012>

        var pathname = window.location.pathname;
        pathname = pathname.toLowerCase();
        //pathname = pathname.substring(pathname.lastIndexOf('/') + 1).toLowerCase();
        //if (pathname == ("/RentaPrivada/Cotizador.aspx").toLowerCase()) { return; };
        //if (pathname == ("/RentaPrivadaPlus/Cotizador.aspx").toLowerCase() ) { return; };
        //if (pathname == ("/RentaPrivadaPlus/ListadoCierrePlus.aspx").toLowerCase()) { return; };

        if (pathname.search(("/RentaPrivada/Cotizador.aspx").toLowerCase()) >= 0) { return; };
        if (pathname.search(("/RentaPrivadaPlus/Cotizador.aspx").toLowerCase()) >= 0) { return; };
        if (pathname.search(("/RentaPrivadaPlus/ListadoCierrePlus.aspx").toLowerCase()) >= 0) { return; };
        if (pathname.search(("/RentaIFP/Cotizador.aspx").toLowerCase()) >= 0) { return; };

        //<FINGTI_7012>

        if (!botonesBloqueados) {
            var pagina = $(this).data("pag");
            $('#TabAfiliadosIndicePagina').val(pagina);

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
            if (columna != $('#TabAfiliadosColumnaOrdenar').attr('value')) {
                $('#TabAfiliadosColumnaOrdenar').val(columna);
                $('#TabAfiliadosDireccionOrdenar').val('A');
            }
            else {
                if ($('#TabAfiliadosDireccionOrdenar').attr('value') == 'A') {
                    $('#TabAfiliadosDireccionOrdenar').val('D');
                }
                else {
                    $('#TabAfiliadosDireccionOrdenar').val('A');
                }
            }
            $('#TabAfiliadosIndicePagina').val(1);

            CargarTablaAfiliados2(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
        }
    });

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
    $('#ModBusAfiCancelar').live('click', function () {
        $('#ModalBusquedaAfiliados').dialog('close');
    });

    /* Cerrar Modal */
    $('#ModalBusquedaAfiliados').live("dialogclose", function () {
        if (ajaxTablaAfiliados != null) {
            ajaxTablaAfiliados.abort();
        }
    });

    /* Botón Aceptar */
    $('#ModBusAfiAceptar').live('click', function () {
        if (!botonesBloqueados && permisoBusAfiExaminarSolicitud) {
            // Validar que haya seleccionado una opción de la tabla
            if ($('#TabAfiliados').find('input[type=radio]:checked').length > 0) {
                $('#BusAfiNroSolicitud').val('');
                $('#BusAfiCUSPP').val($('#TabAfiliados input[type=radio]:checked').attr("value"));
                $('#ModalBusquedaAfiliados').dialog('close');
                $('#BusAfiCUSPP').focus();
                $('#BusAfiBuscar').trigger('click');
                // Simuladores
                $('#SimBusAfiBuscar').trigger('click');
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
    $('#BusAfiBuscar').live('click', function () {
        if (!botonBusAfiBuscarBloqueado && permisoBusAfiBuscar) {
            var esCorrecto = true;
            var errores = new Array();

            $('#BusAfiNroSolicitud').removeClass('formTextboxError');
            $('#BusAfiCUSPP').removeClass('formTextboxError');

            var solicitud = ($.trim($('#BusAfiNroSolicitud').val()).length > 0) ? true : false;
            var cuspp = ($.trim($('#BusAfiCUSPP').val()).length > 0) ? true : false;

            if (!(solicitud | cuspp)) {
                errores.push('Debe ingresar un criterio de búsqueda.');
                $('#BusAfiNroSolicitud').addClass('formTextboxError');
                $('#BusAfiCUSPP').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (solicitud & cuspp) {
                errores.push('Sólo debe ingresar un criterio de búsqueda.');
                $('#BusAfiNroSolicitud').addClass('formTextboxError');
                $('#BusAfiCUSPP').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (cuspp && $.trim($('#BusAfiCUSPP').val()).length != 12) {
                errores.push('El <strong>CUSPP</strong> debe contener 12 caracteres.');
                $('#BusAfiCUSPP').addClass('formTextboxError');
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
            $('#BusAfiBuscar').attr('class', 'botonDeshabilitado gris gris_sharp');
        }
        else {
            return false;
        }
    });

    /* Verificar si se ha modificado algún campo */
    $('#CorreoElectronico,#Categoria,#AFP,#SaldoCIC').bind('keyup keydown keypress change', function () {
        if ($.trim($('#CUSPP').val()).length > 0) {
            if ($(this).val() != jQuery.data(this, 'lastvalue')) {
                datosSinGuardar = true;
            }
            jQuery.data(this, 'lastvalue', $(this).val());
        }
    });

    /* Botón Guardar */
    $('#Guardar').live('click', function () {
        if (!botonesBloqueados && permisoGuardar) {
            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            //$('#CorreoElectronico').removeClass('formTextboxError');
            //$('#ConCategoria').removeClass('formComboboxErrorContenedor');
            $('#ConAFP').removeClass('formComboboxErrorContenedor');
            $('#SaldoCIC').removeClass('formTextboxError');

            //// Correo Electrónico
            //var correoElectronico = true;
            //if ($('#CorreoElectronico').length > 0) {
            //    if ($.trim($('#CorreoElectronico').val()).length == 0) {
            //        errores.push("Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.");
            //        correoElectronico = false;
            //    }
            //}

            //// Categoría
            //var categoria = true;
            //if ($('#Categoria').val() == "0") {
            //    errores.push("Ingrese el campo <strong>Categoría</strong>. Dato Obligatorio.");
            //    categoria = false;
            //}

            // AFP
            var afp = true;
            if ($('#AFP').val() == "0") {
                errores.push("Ingrese el campo <strong>AFP</strong>. Dato Obligatorio.");
                afp = false;
            }

            // Saldo CIC
            var saldoCIC = true;
            if ($('#SaldoCIC').length > 0) {
                if ($.trim($('#SaldoCIC').val()).length == 0) {
                    errores.push("Ingrese el campo <strong>Saldo CIC</strong>. Dato Obligatorio.");
                    saldoCIC = false;
                }
            }

            // Clases de controles
            //if (!correoElectronico) { $('#CorreoElectronico').attr('class', 'formTextbox formTextboxError'); } else { $('#CorreoElectronico').attr('class', 'formTextbox'); }
            //if (!categoria) { $('#ConCategoria').attr('class', 'formComboboxContenedor formComboboxErrorContenedor'); } else { $('#ConCategoria').attr('class', 'formComboboxContenedor'); }
            if (!afp) { $('#ConAFP').attr('class', 'formComboboxContenedor formComboboxErrorContenedor'); } else { $('#ConAFP').attr('class', 'formComboboxContenedor'); }
            if (!saldoCIC) { $('#SaldoCIC').attr('class', 'formTextbox formTextboxError'); } else { $('#SaldoCIC').attr('class', 'formTextbox'); }

            //esCorrecto = correoElectronico & categoria & afp & saldoCIC;
            esCorrecto = afp & saldoCIC;

            if (!esCorrecto) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }

            // Pasó las validaciones
            botonesBloqueados = true;
            $('#Guardar').attr('class', 'botonDeshabilitado gris gris_sharp');
        }
        else {
            return false;
        }
    });

    /* DIRECCIONES */
    var idDireccion;
    /* Botón Nueva */
    $('#NuevaDireccion').live('click', function () {
        if (permisoNuevaDireccion) {
            $('#ModDirModo').val('N');

            LimpiarFormularioDirecciones();

            $('#ModDirCargando').hide();
            $('#ModalDireccion').dialog('open');
            $('#ModDirDireccion').focus();
        }
        else {
            return false;
        }
    });

    /* Botón Modificar */
    $('#TabDirecciones .grilla_editar').live('click', function () {
        $('#ModDirModo').val('M');
        LimpiarFormularioDirecciones();
        $('#ModDirCargando').show();
        $('#ModalDireccion').dialog('open');
        idDireccion = $(this).data('direccion');

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
    $('#ModDirAceptar').live('click', function () {
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
                errores.push('Ingrese el campo <strong>Ciudad</strong>. Dato Obligatorio.');
                $('#ConModDirCiudad').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!comuna) {
                errores.push('Ingrese el campo <strong>Comuna</strong>. Dato Obligatorio.');
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
            $('#ModDirAceptar').attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#ModDirCargando').fadeIn();

            if ($('#ModDirModo').val() == 'N') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    cuspp: $('#CUSPP').val(),
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
                    cuspp: $('#CUSPP').val(),
                    direccion: $('#ModDirDireccion').val(),
                    idDepartamento: $('#ModDirDepartamento').val(),
                    idCiudad: $('#ModDirCiudad').val(),
                    idComuna: $('#ModDirComuna').val(),
                    idPrincipal: $('#ModDirPrincipal').val(),
                    glsEspacioUrbano: $('#ModDirEspacioUrbano').val(),
                    idDomicilio: $('#ModDomicilio').val()
                }
                var postUrl = 'Cotizador.aspx/ModificarDireccion';
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
                        $('#ModalDireccion').dialog('close');

                        /* Recargar la grilla de direcciones */
                        CargarTablaDirecciones();

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
                    $('#ModDirAceptar').attr('class', 'boton darkblue sharp');
                }
            });
        }
    });

    /* Botón Cancelar */
    $('#ModDirCancelar').live('click', function () {
        $('#ModalDireccion').dialog('close');
    });


    /* TELÉFONOS */
    var idTelefono;
    /* Botón Nuevo */
    $('#NuevoTelefono').live('click', function () {
        if (permisoNuevaDireccion) {
            $('#ModTelModo').val('N');

            LimpiarFormularioTelefonos();

            $('#ModTelCargando').hide();
            $('#ModalTelefono').dialog('open');
            $('#ModTelTipo').focus();
        }
        else {
            return false;
        }
    });

    /* Botón Modificar */
    $('#TabTelefonos .grilla_editar').live('click', function () {
        $('#ModTelModo').val('M');
        LimpiarFormularioTelefonos();
        $('#ModTelCargando').show();
        $('#ModalTelefono').dialog('open');
        idTelefono = $(this).data('telefono');

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
    $('#ModTelAceptar').live('click', function () {
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
            $('#ModTelAceptar').attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#ModTelCargando').fadeIn();

            if ($('#ModTelModo').val() == 'N') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    cuspp: $('#CUSPP').val(),
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
                    cuspp: $('#CUSPP').val(),
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
                        $('#ModalTelefono').dialog('close');

                        /* Recargar la grilla de teléfonos */
                        CargarTablaTelefonos();

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
                    $('#ModTelAceptar').attr('class', 'boton darkblue sharp');
                }
            });
        }
    });

    /* Botón Cancelar */
    $('#ModTelCancelar').live('click', function () {
        $('#ModalTelefono').dialog('close');
    });


    /* GRUPO FAMILIAR */
    var idGrupoFamiliar;
    /* Botón Nuevo */
    $('#NuevoBeneficiario').live('click', function () {
        if (permisoNuevoBeneficiario) {
            $('#ModGruFamModo').val('N');

            LimpiarFormularioGrupoFamiliar();

            $('#ModGruFamCargando').hide();
            $('#ModalGrupoFamiliar').dialog('open');
            $('#ModGruFamApellidoPaterno').focus();
        }
        else {
            return false;
        }
    });

    /* Botón Modificar */
    $('#TabGrupoFamiliar .grilla_editar').live('click', function () {
        $('#ModGruFamModo').val('M');

        LimpiarFormularioGrupoFamiliar();

        $('#ModGruFamCargando').show();
        $('#ModalGrupoFamiliar').dialog('open');
        idGrupoFamiliar = $(this).data('grupofamiliar');

        var params = {
            idGrupoFamiliar: idGrupoFamiliar
        }

        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/ObtenerDatosGrupoFamiliar',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                var grupo = data.d;
                // Apellido Paterno
                $('#ModGruFamApellidoPaterno').val(grupo.ApellidoPaterno);
                // Apellido Materno
                $('#ModGruFamApellidoMaterno').val(grupo.ApellidoMaterno);
                // Nombres
                $('#ModGruFamNombres').val(grupo.Nombre);
                // Tipo de Identificación
                $('#ModGruFamTipoIdentificacion').val(grupo.Identificacion.IdTipo);
                $('#TexModGruFamTipoIdentificacion').html($('#ModGruFamTipoIdentificacion').find(':selected').text());
                // Nro. de Identificación
                $('#ModGruFamNumeroIdentificacion').val(grupo.Identificacion.Numero);
                // Parentesco
                $('#ModGruFamParentesco').val(grupo.Parentesco.Id);
                $('#TexModGruFamParentesco').html($('#ModGruFamParentesco').find(':selected').text());
                if (grupo.Parentesco.Id == '80') {
                    $('#ModGruFamParentesco').attr('disabled', 'disabled');
                    $('#ConModGruFamParentesco').addClass('formComboboxReadOnlyContenedor');
                }
                else {
                    $('#ModGruFamParentesco').removeAttr('disabled');
                    $('#ConModGruFamParentesco').removeClass('formComboboxReadOnlyContenedor');
                }
                // Sexo
                $('#ModGruFamSexo').val(grupo.Sexo);
                $('#TexModGruFamSexo').html($('#ModGruFamSexo').find(':selected').text());
                if (grupo.Parentesco.Id == '80') {
                    $('#ModGruFamSexo').attr('disabled', 'disabled');
                    $('#ConModGruFamSexo').addClass('formComboboxReadOnlyContenedor');
                }
                else {
                    $('#ModGruFamSexo').removeAttr('disabled');
                    $('#ConModGruFamSexo').removeClass('formComboboxReadOnlyContenedor');
                }
                // Fecha de Nacimiento
                $('#ModGruFamFechaNacimiento').val(new Date(+grupo.FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));

                // Indicador de Invalidez
                $('#ModGruFamIndInvalidez').val(grupo.Invalido ? 'S' : 'N');
                $('#TexModGruFamIndInvalidez').html($('#ModGruFamIndInvalidez').find(':selected').text());
                if (grupo.Invalido) {
                    // Tipo de Invalidez
                    $('#ModGruFamTipoInvalidez').val(grupo.TipoInvalidez.Id);
                    $('#TexModGruFamTipoInvalidez').html($('#ModGruFamTipoInvalidez').find(':selected').text());
                    $('#ModGruFamTipoInvalidez').removeAttr('disabled');
                    $('#ConModGruFamTipoInvalidez').removeClass('formComboboxReadOnlyContenedor');
                    // Fecha de Invalidez
                    if (grupo.FechaInvalidez != null)
                        $('#ModGruFamFechaInvalidez').val(new Date(+grupo.FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                    $('#ModGruFamFechaInvalidez').removeAttr('disabled');
                    $('#ModGruFamFechaInvalidez').removeClass('formCalendarReadOnly');
                }
                else {
                    // Tipo de Invalidez
                    $('#ModGruFamTipoInvalidez').val('N');
                    $('#TexModGruFamTipoInvalidez').html($('#ModGruFamTipoInvalidez').find(':selected').text());
                    $('#ModGruFamTipoInvalidez').attr('disabled', 'disabled');
                    // Fecha de Invalidez
                    $('#ModGruFamFechaInvalidez').val('');
                    $('#ModGruFamFechaInvalidez').attr('disabled', 'disabled');
                }

                //Apoderado
                $("#ModGruFamFlgApoderado").prop("checked", grupo.IndTieneApoderado);
                $('#Apoderado').toggle(grupo.IndTieneApoderado);

                $('#ModGruFamApellidoPaternoAprdo').val(grupo.ApellidoPaternoApdo);
                $('#ModGruFamApellidoMaternoAprdo').val(grupo.ApellidoMaternoApdo);
                $('#ModGruFamNombresAprdo').val(grupo.NombresApdo);

                if (grupo.IdentificacionApdo != null) {
                    $('#ModGruFamTipoIdentificacionAprdo').val(grupo.IdentificacionApdo.IdTipo);
                    $('#TexModGruFamTipoIdentificacionAprdo').html($('#ModGruFamTipoIdentificacionAprdo').find(':selected').text());

                    $('#ModGruFamNumeroIdentificacionAprdo').val(grupo.IdentificacionApdo.Numero);
                }

                $('#ModGruFamSexoAprdo').val(grupo.SexoApdo);
                $('#TexModGruFamSexoAprdo').html($('#ModGruFamSexoAprdo').find(':selected').text());

                if (grupo.FechaNacimientoApdo != null) {
                    $('#ModGruFamFechaNacimientoAprdo').val(new Date(+grupo.FechaNacimientoApdo.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                }

                $('#ModGruFamApellidoPaterno').focus();
                $('#ModGruFamCargando').fadeOut();
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
    $('#ModGruFamIndInvalidez').live('change', function (e) {
        if ($(this).val() == 'S') {
            $('#ModGruFamTipoInvalidez').removeAttr('disabled');
            $('#ConModGruFamTipoInvalidez').removeClass('formComboboxReadOnlyContenedor');
            $('#ModGruFamFechaInvalidez').removeAttr('disabled');
            $('#ModGruFamFechaInvalidez').removeClass('formCalendarReadOnly');

            $('#AstModGruFamFechaInvalidez').html('*');
        }
        else if ($(this).val() == 'N') {
            $('#ModGruFamTipoInvalidez').val('N');
            $('#ModGruFamTipoInvalidez').trigger('change');
            $('#ModGruFamTipoInvalidez').attr('disabled', 'disabled');
            $('#ConModGruFamTipoInvalidez').addClass('formComboboxReadOnlyContenedor');
            $('#ModGruFamFechaInvalidez').val('');
            $('#ModGruFamFechaInvalidez').trigger('change');
            $('#ModGruFamFechaInvalidez').attr('disabled', 'disabled');
            $('#ModGruFamFechaInvalidez').addClass('formCalendarReadOnly');

            $('#AstModGruFamFechaInvalidez').html('');
        }
        else {
            $('#ModGruFamTipoInvalidez').attr('disabled', 'disabled');
            $('#ConModGruFamTipoInvalidez').addClass('formComboboxReadOnlyContenedor');
            $('#ModGruFamFechaInvalidez').attr('disabled', 'disabled');
            $('#ModGruFamFechaInvalidez').addClass('formCalendarReadOnly');

            $('#AstModGruFamFechaInvalidez').html('');
        }
    });

    var ModGruFamBotonesInactivos = false;
    /* Botón Aceptar */
    $('#ModGruFamAceptar').live('click', function () {
        if (!ModGruFamBotonesInactivos) {
            var esCorrecto = true;
            var errores = new Array();

            $('#ModGruFamApellidoPaterno').removeClass('formTextboxError');
            $('#ModGruFamApellidoMaterno').removeClass('formTextboxError');
            $('#ModGruFamNombres').removeClass('formTextboxError');
            $('#ConModGruFamTipoIdentificacion').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamNumeroIdentificacion').removeClass('formTextboxError');
            $('#ConModGruFamParentesco').removeClass('formComboboxErrorContenedor');
            $('#ConModGruFamSexo').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamFechaNacimiento').removeClass('formTextboxError formCalendarError');
            $('#ConModGruFamIndInvalidez').removeClass('formComboboxErrorContenedor');
            $('#ConModGruFamTipoInvalidez').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamFechaInvalidez').removeClass('formTextboxError formCalendarError');

            $('#ModGruFamApellidoPaternoAprdo').removeClass('formTextboxError');
            $('#ModGruFamApellidoMaternoAprdo').removeClass('formTextboxError');
            $('#ModGruFamNombresAprdo').removeClass('formTextboxError');
            $('#ModGruFamTipoIdentificacionAprdo').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamNumeroIdentificacionAprdo').removeClass('formTextboxError');
            $('#ModGruFamSexoAprdo').removeClass('formComboboxErrorContenedor');
            $('#ModGruFamFechaNacimientoAprdo').removeClass('formTextboxError');

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
            if ($('#ModGruFamParentesco').val() == '0') {
                errores.push('Ingrese el campo <strong>Parentesco</strong>. Dato Obligatorio.');
                parentesco = false;
            }

            // Sexo
            var sexo = true;
            if ($('#ModGruFamSexo').val() == '0') {
                errores.push('Ingrese el campo <strong>Sexo</strong>. Dato Obligatorio.');
                sexo = false;
            }

            // Fecha de Nacimiento
            var fechaNacimiento = true;
            if ($.trim($('#ModGruFamFechaNacimiento').val()).length == 0) {
                errores.push('Ingrese el campo <strong>Fecha de Nacimiento</strong>. Dato Obligatorio.');
                fechaNacimiento = false;
            }

            // Invalidez
            var invalidez = true;
            var tipoInvalidez = true;
            var fechaInvalidez = true;
            if ($('#ModGruFamIndInvalidez').val() == '0') {
                errores.push('Ingrese el campo <strong>Indicador de Invalidez</strong>. Dato Obligatorio.');
                invalidez = false;
            }
            else if ($('#ModGruFamIndInvalidez').val() == 'N') {
                if ($('#ModGruFamTipoInvalidez').val() != 'N') {
                    errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                    tipoInvalidez = false;
                }
                if ($.trim($('#ModGruFamFechaInvalidez').val()).length > 0) {
                    errores.push('El campo <strong>Fecha de Invalidez</strong> sólo debe ser ingresado cuando el Indicador de Invalidez es Sí.');
                    fechaInvalidez = false;
                }
            }
            else if ($('#ModGruFamIndInvalidez').val() == 'S') {
                if ($('#ModGruFamTipoInvalidez').val() != 'P' && $('#ModGruFamTipoInvalidez').val() != 'T') {
                    errores.push('El campo <strong>Tipo de Invalidez</strong> tiene un valor no válido para el Indicador de Invalidez seleccionado.');
                    tipoInvalidez = false;
                }

                if ($.trim($('#ModGruFamFechaInvalidez').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Fecha de Invalidez</strong>. Dato Obligatorio cuando el Indicador de Invalidez es Sí.');
                    fechaInvalidez = false;
                }
            }

            if ($('#ModGruFamTipoInvalidez').val() == '0') {
                errores.push('Ingrese el campo <strong>Tipo de Invalidez</strong>. Dato Obligatorio.');
                tipoInvalidez = false;
            }

            if (!apellidoPaterno) $('#ModGruFamApellidoPaterno').addClass('formTextboxError');
            if (!apellidoMaterno) $('#ModGruFamApellidoMaterno').addClass('formTextboxError');
            if (!nombres) $('#ModGruFamNombres').addClass('formTextboxError');
            if (!tipoIdentificacion) $('#ConModGruFamTipoIdentificacion').addClass('formComboboxErrorContenedor');
            if (!numeroIdentificacion) $('#ModGruFamNumeroIdentificacion').addClass('formTextboxError');
            if (!parentesco) $('#ConModGruFamParentesco').addClass('formComboboxErrorContenedor');
            if (!sexo) $('#ConModGruFamSexo').addClass('formComboboxErrorContenedor');
            if (!fechaNacimiento) $('#ModGruFamFechaNacimiento').addClass('formTextboxError formCalendarError');
            if (!invalidez) $('#ConModGruFamIndInvalidez').addClass('formComboboxErrorContenedor');
            if (!tipoInvalidez) $('#ConModGruFamTipoInvalidez').addClass('formComboboxErrorContenedor');
            if (!fechaInvalidez) $('#ModGruFamFechaInvalidez').addClass('formTextboxError formCalendarError');

            esCorrecto = apellidoPaterno & apellidoMaterno & nombres & tipoIdentificacion & numeroIdentificacion & parentesco & sexo & fechaNacimiento & invalidez & tipoInvalidez & fechaInvalidez;

            if ($('#ModGruFamFlgApoderado').is(":checked")) {
                var apellidoPaternoApod = true;
                if ($('#ModGruFamApellidoPaternoAprdo').val() == '') {
                    errores.push('Ingrese el campo <strong>Apellido Paterno Apod.</strong>. Dato Obligatorio.');
                    apellidoPaternoApod = false;
                    $('#ModGruFamApellidoPaternoAprdo').addClass('formTextboxError');
                }

                var apellidoMaternoApod = true;
                if ($('#ModGruFamApellidoMaternoAprdo').val() == '') {
                    errores.push('Ingrese el campo <strong>Apellido Materno Apod.</strong>. Dato Obligatorio.');
                    apellidoMaternoApod = false;
                    $('#ModGruFamApellidoMaternoAprdo').addClass('formTextboxError');
                }

                var nombresApod = true;
                if ($('#ModGruFamNombresAprdo').val() == '') {
                    errores.push('Ingrese el campo <strong>Nombres Apod.</strong>. Dato Obligatorio.');
                    nombresApod = false;
                    $('#ModGruFamNombresAprdo').addClass('formTextboxError');
                }

                var tipoIdentificacionApod = true;
                if ($('#ModGruFamTipoIdentificacionAprdo').val() == '0') {
                    errores.push('Ingrese el campo <strong>Tipo de Identif. Apod.</strong>. Dato Obligatorio.');
                    tipoIdentificacionApod = false;
                    $('#ConModGruFamTipoIdentificacionAprdo').addClass('formComboboxErrorContenedor');
                }

                var numeroIdentificacionApod = true;
                if ($('#ModGruFamNumeroIdentificacionAprdo').val() == '') {
                    errores.push('Ingrese el campo <strong>Nro. de Identif. Apod.</strong>. Dato Obligatorio.');
                    numeroIdentificacionApod = false;
                    $('#ModGruFamNumeroIdentificacionAprdo').addClass('formTextboxError');
                }

                var sexoApod = true;
                if ($('#ModGruFamSexoAprdo').val() == '0') {
                    errores.push('Ingrese el campo <strong>Sexo Apod.</strong>. Dato Obligatorio.');
                    sexoApod = false;
                    $('#ConModGruFamSexoAprdo').addClass('formComboboxErrorContenedor');
                }

                var fechaNacimientoApod = true;
                if ($.trim($('#ModGruFamFechaNacimientoAprdo').val()).length == 0) {
                    errores.push('Ingrese el campo <strong>Fecha de Nacim. Apod.</strong>. Dato Obligatorio.');
                    fechaNacimientoApod = false;
                    $('#ModGruFamFechaNacimientoAprdo').addClass('formTextboxError formCalendarError');
                }

                esCorrecto = esCorrecto & apellidoPaternoApod & apellidoMaternoApod & nombresApod & tipoIdentificacionApod & numeroIdentificacionApod & sexoApod & fechaNacimientoApod;
            }

            if (!esCorrecto) {
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }

            /* Pasó las validaciones */
            ModGruFamBotonesInactivos = true;
            $('#ModGruFamAceptar').attr('class', 'botonDeshabilitado gris gris_sharp');
            $('#ModGruFamCargando').fadeIn();

            if ($('#ModGruFamModo').val() == 'N') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    cuspp: $('#HCUSPP').val(),
                    apellidoPaterno: ($('#ModGruFamApellidoPaterno').length > 0) ? $('#ModGruFamApellidoPaterno').val() : '',
                    apellidoMaterno: ($('#ModGruFamApellidoMaterno').length > 0) ? $('#ModGruFamApellidoMaterno').val() : '',
                    nombres: ($('#ModGruFamNombres').length > 0) ? $('#ModGruFamNombres').val() : '',
                    tipoIdentificacion: ($('#ModGruFamTipoIdentificacion').length > 0) ? $('#ModGruFamTipoIdentificacion').val() : '',
                    numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion').length > 0) ? $('#ModGruFamNumeroIdentificacion').val() : '',
                    parentesco: $('#ModGruFamParentesco').val(),
                    sexo: $('#ModGruFamSexo').val(),
                    fechaNacimiento: $('#ModGruFamFechaNacimiento').val(),
                    invalidez: $('#ModGruFamIndInvalidez').val(),
                    tipoInvalidez: $('#ModGruFamTipoInvalidez').val(),
                    fechaInvalidez: $('#ModGruFamFechaInvalidez').val(),
                    apellidoPaternoApdo: ($('#ModGruFamApellidoPaternoAprdo').length > 0) ? $('#ModGruFamApellidoPaternoAprdo').val() : '',
                    apellidoMaternoApdo: ($('#ModGruFamApellidoMaternoAprdo').length > 0) ? $('#ModGruFamApellidoMaternoAprdo').val() : '',
                    nombresApdo: ($('#ModGruFamNombresAprdo').length > 0) ? $('#ModGruFamNombresAprdo').val() : '',
                    tipoIdentificacionApdo: ($('#ModGruFamTipoIdentificacionAprdo').length > 0) ? $('#ModGruFamTipoIdentificacionAprdo').val() : '',
                    numeroIdentificacionApdo: ($('#ModGruFamNumeroIdentificacionAprdo').length > 0) ? $('#ModGruFamNumeroIdentificacionAprdo').val() : '',
                    sexoApdo: ($('#ModGruFamSexoAprdo').length > 0) ? $('#ModGruFamSexoAprdo').val() : '',
                    fechaNacimientoApdo: ($('#ModGruFamFechaNacimientoAprdo').length > 0) ? $('#ModGruFamFechaNacimientoAprdo').val() : '',
                    indTieneApoderado: $('#ModGruFamFlgApoderado').is(":checked")
                }
                var postUrl = 'Cotizador.aspx/InsertarGrupoFamiliar';
            }
            else if ($('#ModGruFamModo').val() == 'M') {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    idGrupoFamiliar: idGrupoFamiliar,
                    cuspp: $('#HCUSPP').val(),
                    apellidoPaterno: ($('#ModGruFamApellidoPaterno').length > 0) ? $('#ModGruFamApellidoPaterno').val() : '',
                    apellidoMaterno: ($('#ModGruFamApellidoMaterno').length > 0) ? $('#ModGruFamApellidoMaterno').val() : '',
                    nombres: ($('#ModGruFamNombres').length > 0) ? $('#ModGruFamNombres').val() : '',
                    tipoIdentificacion: ($('#ModGruFamTipoIdentificacion').length > 0) ? $('#ModGruFamTipoIdentificacion').val() : '',
                    numeroIdentificacion: ($('#ModGruFamNumeroIdentificacion').length > 0) ? $('#ModGruFamNumeroIdentificacion').val() : '',
                    parentesco: $('#ModGruFamParentesco').val(),
                    sexo: $('#ModGruFamSexo').val(),
                    fechaNacimiento: $('#ModGruFamFechaNacimiento').val(),
                    invalidez: $('#ModGruFamIndInvalidez').val(),
                    tipoInvalidez: $('#ModGruFamTipoInvalidez').val(),
                    fechaInvalidez: $('#ModGruFamFechaInvalidez').val(),
                    flagRenta: 'vitalicia',
                    apellidoPaternoApdo: ($('#ModGruFamApellidoPaternoAprdo').length > 0) ? $('#ModGruFamApellidoPaternoAprdo').val() : '',
                    apellidoMaternoApdo: ($('#ModGruFamApellidoMaternoAprdo').length > 0) ? $('#ModGruFamApellidoMaternoAprdo').val() : '',
                    nombresApdo: ($('#ModGruFamNombresAprdo').length > 0) ? $('#ModGruFamNombresAprdo').val() : '',
                    tipoIdentificacionApdo: ($('#ModGruFamTipoIdentificacionAprdo').length > 0) ? $('#ModGruFamTipoIdentificacionAprdo').val() : '',
                    numeroIdentificacionApdo: ($('#ModGruFamNumeroIdentificacionAprdo').length > 0) ? $('#ModGruFamNumeroIdentificacionAprdo').val() : '',
                    sexoApdo: ($('#ModGruFamSexoAprdo').length > 0) ? $('#ModGruFamSexoAprdo').val() : '',
                    fechaNacimientoApdo: ($('#ModGruFamFechaNacimientoAprdo').length > 0) ? $('#ModGruFamFechaNacimientoAprdo').val() : '',
                    indTieneApoderado: $('#ModGruFamFlgApoderado').is(":checked")
                }
                var postUrl = 'Cotizador.aspx/ModificarGrupoFamiliar';
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
                        $('#ModalGrupoFamiliar').dialog('close');

                        /* Recargar la grilla de teléfonos */
                        CargarTablaGrupoFamiliar();

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
                            if (data.d.Controles[0].length) $('#ModGruFamApellidoPaterno').attr('class', data.d.Controles[0]);
                            if (data.d.Controles[1].length) $('#ModGruFamApellidoMaterno').attr('class', data.d.Controles[1]);
                            if (data.d.Controles[2].length) $('#ModGruFamNombres').attr('class', data.d.Controles[2]);
                            if (data.d.Controles[3].length) $('#ConModGruFamTipoIdentificacion').attr('class', data.d.Controles[3]);
                            if (data.d.Controles[4].length) $('#ModGruFamNumeroIdentificacion').attr('class', data.d.Controles[4]);
                            if (data.d.Controles[5].length) $('#ConModGruFamParentesco').attr('class', data.d.Controles[5]);
                            if (data.d.Controles[6].length) $('#ConModGruFamSexo').attr('class', data.d.Controles[6]);
                            if (data.d.Controles[7].length) $('#ModGruFamFechaNacimiento').attr('class', data.d.Controles[7]);
                            if (data.d.Controles[8].length) $('#ConModGruFamIndInvalidez').attr('class', data.d.Controles[8]);
                            if (data.d.Controles[9].length) $('#ConModGruFamTipoInvalidez').attr('class', data.d.Controles[9]);
                            if (data.d.Controles[10].length) $('#ModGruFamFechaInvalidez').attr('class', data.d.Controles[10]);
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
                    $('#ModGruFamCargando').fadeOut();
                    ModGruFamBotonesInactivos = false;
                    $('#ModGruFamAceptar').attr('class', 'boton darkblue sharp');
                }
            });
        }
    });

    /* Botón Cancelar */
    $('#ModGruFamCancelar').live('click', function () {
        $('#ModalGrupoFamiliar').dialog('close');
    });

    /* Botón Nueva */
    $('#NuevaSolicitud').live('click', function () {
        if (permisoNuevaSolicitud) {
            $('#ModSolModo').val('N');
            $('#BeneficiariosOriginales').hide();
            $('#TablaRviBenefiContenedor').hide();
            $('#ManSolPestanhas li:eq(0)').trigger('click');

            $('#ModSolCargando').show();
            $('#ModalSolicitud').dialog('open');

            LimpiarFormularioSolicitud();
            $('#ManSolTipoSolicitud').val('EXTRAOFICIAL');
            $("#ModSolLineaMontoACOM").hide();

            $.ajax({
                type: 'POST',
                url: 'Cotizador.aspx/CrearDatosSolicitud',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                success: function (data) {
                    /* Solicitud Creada */
                    Solicitud = data.d;
                    $('#ModSolFecUltActualizacion').val(new Date(+Solicitud.FechaSolicitud.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));

                    //<INIGTI_4022>
                    if (Solicitud.Agente.IdNivel == 0) {
                        $("#LabModSolNivel").attr('title', '');
                    } else {
                        $("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
                    }
                    //<FINGTI_4022>

                    /* Cargar Tabla de Cotizaciones con las cotizaciones iniciales */
                    var params = {
                        idTipoPension: $('#ModSolTipoPension').val()
                    }
                    $.ajax({
                        type: 'POST',
                        url: 'Cotizador.aspx/CargarComboProductos',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: $.toJSON(params),
                        success: function (data) {
                            CargarTablaCotizaciones(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud').val());
                            CargarTablaBeneficiarios(null);

                            $('#ModSolCargando').fadeOut();
                            $('#ModSolTipoCambio').focus();

                            //<GTIINI-758>
                            $("#Precargando").fadeIn(2000);

                            $.ajax({
                                type: "POST",
                                url: "Cotizador.aspx/PrecargarParametros",
                                contentType: "application/json; charset=iso-8859-1",
                                dataType: 'json',
                                success: function (data) {
                                    $("#Precargando").fadeOut(2000);
                                },
                                error: function (XMLHttpRequest, textStatus, errorThrown) {
                                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                        /* Sesión caducada */
                                        document.location.reload(true);
                                    }
                                    else {
                                        $('#ModalSolicitud').dialog('close');

                                        $("#MCMIcono").attr("class", "error");
                                        $("#MCMContenedor").html("Ha ocurrido un error al cargar los parámetros de cotización.");
                                        $("#ModalCuadroMensaje").dialog({ title: "Error" });
                                        $("#ModalCuadroMensaje").dialog("open");
                                    }
                                }
                            });
                            //<GTIFIN-758>
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                /* Sesión caducada */
                                document.location.reload(true);
                            }
                            else {
                                $('#ModalSolicitud').dialog('close');

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
        }
        else {
            return false;
        }
    });

    /* Botón Modificar */
    $('#TabSolicitudes .grilla_editar').live('click', function () {

        $('#ModSolModo').val('M');
        $('#BeneficiariosOriginales').show();
        $('#ManSolPestanhas li:eq(0)').trigger('click');

        $('#ModSolCargando').show();

        LimpiarFormularioSolicitud();

        var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();

        $('#ManSolTipoSolicitud').val($(this).parent('td').parent('tr').children().eq(3).find('span').html());
        if ($('#ManSolTipoSolicitud').val() != "EXTRAOFICIAL") {
            $("#ModSolLineaMontoACOM").show();
            botonModSolAceptarBloqueado = true;
            $('#ModSolAceptar').attr('class', 'botonDeshabilitado gris gris_sharp');
        }
        else {
            $("#ModSolLineaMontoACOM").hide();
            botonModSolAceptarBloqueado = false;
            $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
        }

        $('#ModalSolicitud').dialog('open');

        var params = {
            idSolicitud: $(this).data('solicitud'),
            fecCotizacion: fecCotizacion
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/ObtenerDatosSolicitud',
            contentType: "application/json; charset=iso-8859-1",
            data: $.toJSON(params),
            dataType: 'json',
            success: function (data) {
                Solicitud = data.d;

                $('#ModSolMontoACOM').val("0")

                $('#ModSolNroSolicitud').val(Solicitud.Id);
                $('#ModSolTipoCambio').val(formatearMonto(Solicitud.TipoCambio));
                $('#ModSolTipoPension').val(Solicitud.TipoPension.Id);
                $('#TexModSolTipoPension').html($('#ModSolTipoPension').find(':selected').text());
                $('#ModSolFechaDevengue').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolFecUltActualizacion').val(new Date(+Solicitud.FechaSolicitud.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolFechaRecepcion').val(new Date(+Solicitud.FechaRecepcion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolFechaPlazoAFP').val(new Date(+Solicitud.FechaPlazoAFP.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolSaldoCIC').val(formatearMonto(Solicitud.SaldoCIC));
                $('#ModSolListaCIC').val(formatearMonto2(Solicitud.SaldoCIC));
                $('#TexModSolListaCIC').html($('#ModSolListaCIC').find(':selected').text());
                $('#ModSolFactorTasa').val(Solicitud.FactorTasa);
                $('#TexModSolFactorTasa').html($('#ModSolFactorTasa').find(':selected').text());
                $('#ModSolFechaCotizacion').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolFechaSolicitudPension').val(new Date(+Solicitud.FechaSolicitudPension.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                $('#ModSolACOM').val(formatearMonto(Solicitud.PorcentajeAumentoComision));
                $('#ModSolDCOM').val(formatearMonto(Solicitud.PorcentajeDescuentoComision));
                /* $("#ModSolMontoACOM").val(formatearMonto(Solicitud.MontoAumentoComision)); */

                //<INIGTI_4022>
                if (Solicitud.Agente.IdNivel == 0) {
                    $("#LabModSolNivel").attr('title', '');
                } else {
                    $("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
                }
                //<FINGTI_4022>

                CargarTablaBeneficiarios(null);
                CargarTablaBeneficiarios(Solicitud.Beneficiarios);

                /* Cargar Tabla de Cotizaciones con las cotizaciones obtenidas */
                var params = {
                    idTipoPension: $('#ModSolTipoPension').val()
                }
                $.ajax({
                    type: 'POST',
                    url: 'Cotizador.aspx/CargarComboProductos',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        CargarTablaCotizaciones(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud').val());
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

                $('#ModSolCargando').fadeOut();
                $('#ModSolSaldoCIC').focus();
                $('#ModSolACOM').focus();
                $('#ModSolDCOM').focus();
                if ($("#ManSolTipoSolicitud").val() != "EXTRAOFICIAL") {
                    $("#ModSolMontoACOM").focus();
                }
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
                $('#ModalSolicitud').dialog('close');
            }
        });
    });

    $('#TablaRviBenefiReintentar').live('click', function () {
        $('#TablaRviBenefiError').hide();
        CargarTablaBeneficiarios(Solicitud.Beneficiarios);
    });

    /* Botón PDF */
    $("#TabSolicitudes .grilla_pdf,#TabCotizacionCotizaciones .grilla_pdf").live("click", function () {

        //<GTIINI-754>
        var sTipoCotizacion = $(this).data("tipocotizacion");

        if (sTipoCotizacion == "EXTRAOFICIAL") {
            var idSolicitud = $(this).data('solicitud');
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
                            window.location.href = "../Reportes/DetalleCotizacionMovil.aspx";
                        }
                        else {
                            var w = 800;
                            var h = 600;
                            var left = (screen.width / 2) - (w / 2);
                            var top = (screen.height / 2) - (h / 2);
                            var nuevaVentana = window.open("../Reportes/DetalleCotizacion.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
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
                        $('#MCMContenedor').html('Ha ocurrido un error al exportar la solicitud.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                }
            });
        } else {
            $("#ModalElegirFormatoReporte").dialog("open");

            $("#RepDetalleCotizacionIdSolicitud").val($(this).data("solicitud"));
            $("#RepDetalleCotizacionFechaCotizacion").val($(this).data("fechacotizacion"));
            $("#RepDetalleCotizacionNumeroAgente").val($(this).data("numagente"));
        }



        //<GTIFIN-754>
    });

    //$("#TabSolicitudes .grilla_pdf_poliza").live("click", function () {
    //    var numPoliza = $(this).data('poliza');
    //    console.log(numPoliza);
    //});

    //<GTIINI-754>
    $("#MEFRAceptar").live("click", function () {
        var params = {
            idSolicitud: $("#RepDetalleCotizacionIdSolicitud").val(),
            fecCotizacion: $("#RepDetalleCotizacionFechaCotizacion").val(),
            numAgente: $("#RepDetalleCotizacionNumeroAgente").val()
        }

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/ExportarSolicitudPDF",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    //if (jQuery.browser.mobile) {
                    if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                        if ($("#TabFormatoReporte input[type=radio]:checked").val() == "B") {
                            window.location.href = "../Reportes/DetalleCotizacionMovil.aspx";
                        }
                        else {
                            window.location.href = "../Reportes/DetalleCotizacionMembretadoMovil.aspx";
                        }
                    }
                    else {
                        var w = 800;
                        var h = 600;
                        var left = (screen.width / 2) - (w / 2);
                        var top = (screen.height / 2) - (h / 2);
                        if ($("#TabFormatoReporte input[type=radio]:checked").val() == "B") {
                            var nuevaVentana = window.open("../Reportes/DetalleCotizacion.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                        }
                        else {
                            var nuevaVentana = window.open("../Reportes/DetalleCotizacionMembretado.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                        }
                    }

                    $("#ModalElegirFormatoReporte").dialog("close");

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
                    $("#MCMContenedor").html("Ha ocurrido un error al exportar la solicitud.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            }
        });
    });

    $("#MEFRCancelar").live("click", function () {
        $("#ModalElegirFormatoReporte").dialog("close");
    });
    //<GTIFIN-754>

    $("#TabSolicitudes .grilla_pdf_poliza").live('click', function (e) {
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
                        window.open('../ArchivosTemporales/RVI/Poliza/' + data.d.Archivos[nombre], '_blank');
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
                nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP').val()),
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
                nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP').val()),
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
                nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP').val()),
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
                nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP').val()),
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
                nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP').val()),
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

    /*<INIGTI_2145>*/
    /* Botón Correo Electrónico Simulador */
    $('#btnNuevoCorreoComparativo').live('click', function () {

        if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

            $('#ModEnvCorCargandoSimulador').show();

            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
                nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
                cuspp: $.trim($('#BusAfiCUSPP').val()),
                rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
            }

            LimpiarFormularioCorreo();

            DeslizarPantallaCorreo();

            $.ajax({
                type: 'POST',
                url: 'CompararPlazoFijo.aspx/CrearDatosCorreo',
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
    /*<FINGTI_2145>*/

    /* Botón Correo Electrónico */
    $('#TabSolicitudes .grilla_email').live('click', function () {
        if ($.trim($('#CorreoElectronicoRegistrado').val()).length > 0) {
            $('#ModEnvCorCargando').show();

            var idSolicitud = $(this).data('solicitud');
            var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
            var tipoCotizacion = $(this).parent('td').parent('tr').children().eq(3).find('span').html();

            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                idSolicitud: idSolicitud,
                fecCotizacion: fecCotizacion,
                tipoCotizacion: tipoCotizacion,
                nombre: $.trim($('#Nombres').val()),
                apellidoPaterno: $.trim($('#ApellidoPaterno').val()),
                apellidoMaterno: $.trim($('#ApellidoMaterno').val()),
                sexo: $('#Sexo').val()
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
                        Correo.Para = $('#CorreoElectronicoRegistrado').val();
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

        //<INIGTI_754>
        //var maxAcom = $('input[type="radio"]:checked').val();
        var maxAcom = $('input:radio[name=id]:checked').val();
        //<FINGTI_754>

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
    $('#TabSolicitudes .grilla_rep_escenario').live('click', function () {

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
                    $('#ModalSeleccionAcom').dialog({ title: "ACOM - Escenario" });
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
    });

    //<INIGTI_2145>
    /* Botón Enviar Correo Electrónico Simulador Comparativo*/
    $('#ModEnvCorEnviarSimuladorComparativo').live('click', function () {
        $('#ModEnvCorCargandoSimulador').fadeIn();

        Correo.Mensaje = $('#ModEnvCorMensaje').val();
        Correo.Respuesta = null;

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            correo: Correo
        }
        $.ajax({
            type: 'POST',
            url: 'CompararPlazoFijo.aspx/EnviarCorreoElectronicoSimulador',
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

    //<FINGTI_2145>


    /* Botón Cancelar Correo Electrónico */
    $('#ModEnvCorCancelar').live('click', function () {
        $('#ModalEnvioCorreo').dialog('close');
        //<SRI.INI-20322>
        $('#ModalEnvioCorreoSimulador').dialog('close');
        //<SRI.FIN-20322>
    });


    /* Cambiar combobox Tipo Pensión */
    $('#ModSolTipoPension').live('change', function () {
        var params = {
            idTipoPension: $(this).val()
        }
        $.ajax({
            type: 'POST',
            url: 'Cotizador.aspx/CargarComboProductos',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
                    Solicitud.Cotizaciones[i].Producto.Id = '0';
                }
                CargarTablaCotizaciones(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud').val());
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
    $('#TabCotizacionesAgregar').live('click', function () {
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
                CargarTablaCotizaciones(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud').val());

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
    $('#TabCotizaciones select').live('change', function () {
        var celda = $(this).parent('td');
        var col = celda.parent('tr').children().index(celda);

        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        switch (col) {
            case 2: //Moneda
                Solicitud.Cotizaciones[fil].Moneda.Id = $(this).val();
                break;
            case 3: //Producto
                Solicitud.Cotizaciones[fil].Producto.Id = $(this).val();
                break;
            case 4: //Modalidad
                Solicitud.Cotizaciones[fil].Modalidad.Id = $(this).val();

                //<SOLINIGTI_754>
                ObtenerPeriodoTemporal($(this).val(), fila.find('.ModSolPeriodoDiferido'));

                fila.find('.ModSolGratificacion').removeAttr('disabled');
                if (Solicitud.Cotizaciones[fil].Modalidad.Id == 'D') {
                    Solicitud.Cotizaciones[fil].PeriodoDiferido = '1';
                    fila.find('.ModSolPeriodoDiferido').removeAttr('disabled');
                    fila.find('.ModSolPeriodoDiferido').val('1');

                    Solicitud.Cotizaciones[fil].PorcentajeEntreRentas = '50';
                    fila.find('.ModSolPorcentajeRentas').val('50');
                    fila.find('.ModSolPorcentajeRentas').attr('disabled', 'disabled');
                } else if (Solicitud.Cotizaciones[fil].Modalidad.Id == 'I-RVE') {
                    Solicitud.Cotizaciones[fil].PeriodoDiferido = '1';
                    fila.find('.ModSolPeriodoDiferido').removeAttr('disabled');
                    fila.find('.ModSolPeriodoDiferido').val('1');

                    Solicitud.Cotizaciones[fil].PorcentajeEntreRentas = '50';
                    fila.find('.ModSolPorcentajeRentas').val('50');
                    fila.find('.ModSolPorcentajeRentas').removeAttr('disabled');

                    fila.find('.ModSolGratificacion').val('N');
                    fila.find('.ModSolGratificacion').attr('disabled', 'disabled');
                    Solicitud.Cotizaciones[fil].Gratificacion = false;

                }
                else {
                    Solicitud.Cotizaciones[fil].PeriodoDiferido = '0';
                    fila.find('.ModSolPeriodoDiferido').attr('disabled', 'disabled');
                    fila.find('.ModSolPeriodoDiferido').val('0');

                    Solicitud.Cotizaciones[fil].PorcentajeEntreRentas = '0';
                    fila.find('.ModSolPorcentajeRentas').val('0');
                    fila.find('.ModSolPorcentajeRentas').attr('disabled', 'disabled');
                }

                //<SOLFINGTI_754>

                //<SRIINI18360>
                //if (Solicitud.Cotizaciones[fil].Modalidad.Id == 'I-RM' || Solicitud.Cotizaciones[fil].Modalidad.Id == 'I-RB') {
                //<SRI.INI-20322>
                if (Solicitud.Cotizaciones[fil].Modalidad.Id == 'I-RM' || Solicitud.Cotizaciones[fil].Modalidad.Id == 'I-RB') {
                    //if (Solicitud.Cotizaciones[fil].Modalidad.Id == 'I-RM' || Solicitud.Cotizaciones[fil].Modalidad.Id == 'I-RC' || Solicitud.Cotizaciones[fil].Modalidad.Id == 'I-RB') {
                    //<SRI.FIN-20322>
                    //<SRIFIN18360>
                    Solicitud.Cotizaciones[fil].Capital.Id = '03';
                    fila.find('.ModSolCapital').val('03');
                }
                //<SRI.INI-20322>
                else if (Solicitud.Cotizaciones[fil].Modalidad.Id == 'I-RC') {
                    Solicitud.Cotizaciones[fil].Capital.Id = '04';
                    fila.find('.ModSolCapital').val('04');
                }
                //<SRI.FIN-20322>
                else {
                    Solicitud.Cotizaciones[fil].Capital.Id = '-';
                    fila.find('.ModSolCapital').val('-');
                }



                break;
            case 5: //Período diferido
                if ($(this).val() != '0') {
                    Solicitud.Cotizaciones[fil].PeriodoDiferido = $(this).val();
                }
                else {
                    $(this).val('1');
                    Solicitud.Cotizaciones[fil].PeriodoDiferido = '1';
                }
                break;
            case 6: //Porcentaje entre rentas
                //<SOLINIGTI_754>
                if (Solicitud.Cotizaciones[fil].Modalidad.Id == "I-RVE") {
                    if ($(this).val() != '0') {
                        Solicitud.Cotizaciones[fil].PorcentajeEntreRentas = $(this).val();
                    } else {
                        $(this).val('50');
                        Solicitud.Cotizaciones[fil].PorcentajeEntreRentas = '50';
                    }
                } else {
                    Solicitud.Cotizaciones[fil].PorcentajeEntreRentas = $(this).val();
                }
                //<SOLFINGTI_754>

                break;
            case 7: //Período garantizado
                Solicitud.Cotizaciones[fil].PeriodoGarantizado = $(this).val();
                //<SOLINIGTI_754>
                if (Solicitud.Cotizaciones[fil].Modalidad.Id == "I-RVE") {
                    if (Solicitud.Cotizaciones[fil].PeriodoGarantizado != 0) {
                        Solicitud.Cotizaciones[fil].PeriodoDiferido = $(this).val();
                        fila.find('.ModSolPeriodoDiferido').val($(this).val());
                    }
                }
                //<SOLFINGTI_754>

                break;
            case 8: //Gratificación
                Solicitud.Cotizaciones[fil].Gratificacion = ($(this).val() == 'S') ? true : false;
                break;
            case 9: //Capital
                Solicitud.Cotizaciones[fil].Capital.Id = $(this).val();
                break;
        }
    });

    /* Modificar el Ajuste TRA en la Tabla de Cotizaciones */
    $('#TabCotizaciones input[type=text]').live('keyup', function () {
        var celda = $(this).parent('td');
        //<INIGTI_4081>
        var col = celda.parent('tr').children().index(celda);
        //<FINGTI_4081>
        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        Solicitud.Cotizaciones[fil].AjusteTRA = $(this).val();

        //<INIGTI_4081>
        //switch (col) {
        //    case 12: //AjusteTRA
        //        Solicitud.Cotizaciones[fil].AjusteTRA = $(this).val();
        //        //alert("TRA");
        //        break;
        //    case 13:
        //        Solicitud.Cotizaciones[fil].pbs = $(this).val();
        //        if (Solicitud.Cotizaciones[fil].pbs == "")
        //            Solicitud.Cotizaciones[fil].pbs = "0";
        //        //alert("pbs");
        //        break;
        //};

        //Solicitud.Cotizaciones[fil].pbs = $(this).val();
        //if (Solicitud.Cotizaciones[fil].pbs == "")
        //    Solicitud.Cotizaciones[fil].pbs = "0";

        //<FINGTI_4081>

    });

    //<INIGTI_4081>
    /* Modificar la Diferencia en la Tabla de TasaMaximaTraMinima */
    $('#TabTasaMaximaTraMinima input[type=text]').live('keyup', function () {
        var celda = $(this).parent('td');
        var col = celda.parent('tr').children().index(celda);

        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);
        ParametroEspecial[fil].ValDiferencia = $(this).val();

        modificaEspecial = true;

    });

    /* Modificar en la Tabla de CuotasTra */
    $('#TabCuotasTra input[type=text]').live('keyup change', function () {
        var celda = $(this).parent('td');
        var col = celda.parent('tr').children().index(celda);

        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        switch (col) {
            case 3: //AjusteTRA
                listaCuotasTra[fil].FecFinVigenciaStr = $(this).val();
                break;
            case 4:
                listaCuotasTra[fil].NroCasosTotal = $(this).val();
                break;
        };



    });
    //<FINGTI_4081>
    /* Botón Eliminar */
    $('#TabCotizaciones .grilla_eliminar').live('click', function () {
        idCotizacion = $(this).data('cotizacion');
        $('#MCATablaEliminar').val('cot');
        $('#MCAIcono').attr('class', 'advertencia');
        $('#MCAContenedor').html('¿Confirma la eliminación de la cotización <strong>N° ' + idCotizacion + '</strong>?');
        $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
        $('#ModalCuadroAdvertencia').dialog('open');
        return false;
    });

    /* Seleccionar beneficiario en tabla de beneficiarios */
    $('#TabBeneficiarios input[type=checkbox]').live('change', function () {
        $('#ManSolNumBeneficiarios').html('(' + $('#TabBeneficiarios input[type=checkbox]:checked').length + ')');
    });

    /* Mostrar montos cotizados */
    $('#TabCotizaciones select,#TabCotizaciones input[type=text]').live('focus', function () {
        var filaPadre = $(this).parent().parent();

        var celda = $(this).parent('td');
        var col = celda.parent('tr').children().index(celda);

        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        var radio = fila.find('input[type=radio]');
        //<INIGTI_4081>//Será TRUE cuando este en la solicitud de cambio
        if ($('#HBloqueaRadio').val() != "TRUE") {
            radio.attr('checked', 'checked');
        }
        var texto = fila.find('input[type=text]');
        //<FINGTI_4081>

        var clasePadre = filaPadre.attr('class');
        setTimeout(function () {
            if ($('#HBloqueaRadio').val() != "TRUE") {
                $("#TabCotizaciones tbody tr").removeClass("grilla_active");
                $("#TabCotizaciones tbody tr:even").addClass("grilla_alt1");
                $("#TabCotizaciones tbody tr:odd").addClass("grilla_alt2");

                filaPadre.toggleClass("grilla_active", radio.is(":checked"));
            }
            //filaPadre.toggleClass("grilla_active", texto);
        }, 0);

        $('#ModSolMontoCIA').val(formatearMonto(Solicitud.Cotizaciones[fil].MontoCia.toFixed(2)));
        $('#ModSolPensionCIA').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionCia.toFixed(2)));
        $('#ModSolPensionCIAMO').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionCiaMO.toFixed(2)));
        $('#ModSolTasaAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaAFP.toFixed(2)));
        $('#ModSolMontoAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].MontoAFP.toFixed(2)));
        $('#ModSolPensionAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionAFP.toFixed(2)));
        /*<SRI.INI-20322>*/
        $('#ModTasaVenta').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaVenta.toFixed(2)));
        $('#ModTasaVentaSbs').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaVentaSbs.toFixed(2)));
        /*<SRI.FIN-20322>*/
    });

    /* Seleccionar cotización */
    $('#TabCotizaciones input[type=radio]').live('change', function () {

        var radio = $(this);
        var filaPadre = radio.parent().parent();

        var celda = $(this).parent('td');
        var fila = celda.parent('tr');
        var fil = fila.parent('tbody').children().index(fila);

        ApiCotizadorRV.ObtenerMontoAcom(Solicitud.Id, Solicitud.PorcentajeAumentoComision || 0, Solicitud.Cotizaciones[fil].Correlativo)
            .then(response => {
                $('#ModSolMontoACOM').val(formatearMonto(response.monto.toFixed(2)))
            }).catch(err => {
                console.error('Error al obtener monto acom', err)
                $('#ModSolMontoACOM').val("0")
            })

        var clasePadre = filaPadre.attr('class');
        setTimeout(function () {
            $("#TabCotizaciones tbody tr").removeClass("grilla_active");
            $("#TabCotizaciones tbody tr:even").addClass("grilla_alt1");
            $("#TabCotizaciones tbody tr:odd").addClass("grilla_alt2");

            filaPadre.toggleClass("grilla_active", radio.is(":checked"));
        }, 0);

        $('#ModSolMontoCIA').val(formatearMonto(Solicitud.Cotizaciones[fil].MontoCia.toFixed(2)));
        $('#ModSolPensionCIA').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionCia.toFixed(2)));
        $('#ModSolPensionCIAMO').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionCiaMO.toFixed(2)));
        $('#ModSolTasaAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaAFP.toFixed(2)));
        $('#ModSolMontoAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].MontoAFP.toFixed(2)));
        /*<GTIINI754>*/
        //$('#ModSolPensionAFP').val(formatearMonto(Solicitud.Cotizaciones[fil].PensionAFP.toFixed(2)));
        if (Solicitud.Cotizaciones[fil].Modalidad.Id == "I-RVE") {
            $("#LabModSolPensionAFP").html("Pensión 2do Tramo:");
            $("#ModSolPensionAFP").val(formatearMonto(Solicitud.Cotizaciones[fil].PrimeraPensionRVD.toFixed(2)));
        }
        else if (Solicitud.Cotizaciones[fil].Modalidad.Id == "I-RB") {
            $("#LabModSolPensionAFP").html("Pensión CIA 2:");
            $("#ModSolPensionAFP").val(formatearMonto(Solicitud.Cotizaciones[fil].PensionAFP.toFixed(2)));
        }
        else {
            $("#LabModSolPensionAFP").html("Pensión AFP:");
            $("#ModSolPensionAFP").val(formatearMonto(Solicitud.Cotizaciones[fil].PensionAFP.toFixed(2)));
        }
        /*<GTIFIN754>*/
        /*<SRI.INI-20322>*/
        $('#ModTasaVenta').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaVenta.toFixed(2)));
        $('#ModTasaVentaSbs').val(formatearMonto(Solicitud.Cotizaciones[fil].TasaVentaSbs.toFixed(2)));
        /*<SRI.FIN-20322>*/
    });

    /* Botón Aceptar */
    $('#ModSolAceptar').live('click', function () {
        if (!botonModSolAceptarBloqueado) {
            if ($.trim($('#CorreoElectronicoRegistrado').val()).length > 0) {
                if ($('#ModSolModo').val() == 'N') {
                    var idBeneficiarios = new Array();

                    // Obtener la lista de beneficiarios
                    for (i = 0; i < $('#TabBeneficiarios tbody tr').length; i++) {
                        if ($('#TabBeneficiarios tbody tr:eq(' + i + ') input').is(':checked'))
                            idBeneficiarios.push(i);
                    }

                    //$('#ModSolCargando').fadeIn();
                    $('#MCIcono').attr('class', 'cargando');
                    $('#MCContenedor').html('Cotizando la solicitud, por favor espere un momento...');
                    $('#ModalCotizando').dialog({ title: 'Cotizando' });
                    $('#ModalCotizando').dialog('open');

                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        cuspp: $('#HCUSPP').val(),
                        afp: $('#HAFP').val(),
                        tipoCambio: $('#ModSolTipoCambio').val(),
                        tipoPension: $('#ModSolTipoPension').val(),
                        categoria: $('#ModSolCategoria').val(),
                        fechaDevengue: $('#ModSolFechaDevengue').val(),
                        fechaRecepcion: $('#ModSolFechaRecepcion').val(),
                        fechaPlazoAFP: $('#ModSolFechaPlazoAFP').val(),
                        saldoCIC: ($('#ModSolSaldoCIC').length > 0) ? $('#ModSolSaldoCIC').val() : $('#ModSolListaCIC').val(),
                        factorTasa: $('#ModSolFactorTasa').val(),
                        fechaCotizacion: $('#ModSolFechaCotizacion').val(),
                        fechaSolicitudPension: $('#ModSolFechaSolicitudPension').val(),
                        acom: ($('#ModSolACOM').length > 0) ? $('#ModSolACOM').val() : '',
                        dcom: ($('#ModSolDCOM').length > 0) ? $('#ModSolDCOM').val() : '',
                        cotizaciones: Solicitud.Cotizaciones,
                        idBeneficiarios: idBeneficiarios,
                        //<SRI.INI-20322_E2>
                        correo: $('#CorreoElectronico').val()
                        //<SRI.FIN-20322_E2>
                    }

                    fetch($('#url_api_rentas_rv').val() + '/cotizacion-extra-oficial/insertar', {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json; charset=iso-8859-1',
                            'x-username': $('#usuario_actual').val(),
                            'x-rol': $('#rol_azman').val()
                        },
                        body: JSON.stringify(params)
                    }).then(response => {
                        if (!response.ok) {
                            // Manejar errores de respuesta HTTP, como 404, 500, etc.
                            if (response.status === 401 || response.status === 12030) {
                                // Sesión caducada
                                document.location.reload(true);
                            } else {
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#ModalCuadroMensaje').dialog('open');
                                $('#ModalCotizando').dialog('close');
                            }
                            throw new Error('Network response was not ok.');
                        }
                        return response.json();
                    }).then(data => {
                        if (data.data.Respuesta.Estado == 'OK') {
                            // Imprimir número de solicitud generada
                            $('#ModSolNroSolicitud').val(data.data.Id);

                            // Cambiar la modal a modo de modificación
                            $('#ModSolModo').val('M');

                            // Cerrar modal de espera
                            $('#ModalCotizando').dialog('close');

                                // Mostrar mensaje de éxito
                                $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                                $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                                $('#ModalCuadroMensaje').dialog('open');

                            CargarTablaSolicitudes();

                            // Cargar la cotización actualizada
                            $('#BeneficiariosOriginales').show();
                            $('#ManSolPestanhas li:eq(0)').trigger('click');

                            $('#ModSolCargando').show();

                            var params3 = {
                                idSolicitud: $('#ModSolNroSolicitud').val(),
                                fecCotizacion: $('#ModSolFechaCotizacion').val()
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
                                        $('#ModSolNroSolicitud').val(Solicitud.Id);
                                        $('#ModSolTipoCambio').val(formatearMonto(Solicitud.TipoCambio));
                                        $('#ModSolTipoPension').val(Solicitud.TipoPension.Id);
                                        $('#TexModSolTipoPension').html($('#ModSolTipoPension').find(':selected').text());
                                        $('#ModSolFechaDevengue').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolFecUltActualizacion').val(new Date(+Solicitud.FechaSolicitud.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolFechaRecepcion').val(new Date(+Solicitud.FechaRecepcion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolFechaPlazoAFP').val(new Date(+Solicitud.FechaPlazoAFP.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolSaldoCIC').val(formatearMonto(Solicitud.SaldoCIC));
                                        $('#ModSolListaCIC').val(formatearMonto2(Solicitud.SaldoCIC));
                                        $('#TexModSolListaCIC').html($('#ModSolListaCIC').find(':selected').text());
                                        $('#ModSolFactorTasa').val(Solicitud.FactorTasa);
                                        $('#TexModSolFactorTasa').html($('#ModSolFactorTasa').find(':selected').text());
                                        $('#ModSolFechaCotizacion').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolFechaSolicitudPension').val(new Date(+Solicitud.FechaSolicitudPension.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolACOM').val(formatearMonto(Solicitud.PorcentajeAumentoComision));
                                        $('#ModSolDCOM').val(formatearMonto(Solicitud.PorcentajeDescuentoComision));

                                    //<INIGTI_4022>
                                    if (Solicitud.Agente.IdNivel == 0) {
                                        $("#LabModSolNivel").attr('title', '');
                                    } else {
                                        $("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
                                    }
                                    //<FINGTI_4022>

                                    //CargarTablaBeneficiarios(null);
                                    CargarTablaBeneficiarios(Solicitud.Beneficiarios);

                                        /* Cargar Tabla de Cotizaciones con las cotizaciones obtenidas */
                                        var params = {
                                            idTipoPension: $('#ModSolTipoPension').val()
                                        }
                                        $.ajax({
                                            type: 'POST',
                                            url: 'Cotizador.aspx/CargarComboProductos',
                                            contentType: "application/json; charset=iso-8859-1",
                                            dataType: 'json',
                                            data: $.toJSON(params),
                                            success: function (data) {
                                                CargarTablaCotizaciones(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud').val());

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
                                                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de productos.');
                                                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                                    $('#ModalCuadroMensaje').dialog('open');
                                                }
                                            }
                                        });

                                        $('#ModSolCargando').fadeOut();
                                        $('#ModSolSaldoCIC').focus();
                                        $('#ModSolACOM').focus();
                                        $('#ModSolDCOM').focus();
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
                                    if (data.d.Respuesta.Controles[0].length) $('#ModSolTipoCambio').attr('class', data.d.Respuesta.Controles[0]);
                                    if (data.d.Respuesta.Controles[1].length) $('#ConModSolTipoPension').attr('class', data.d.Respuesta.Controles[1]);
                                    if (data.d.Respuesta.Controles[2].length) $('#ConModSolCategoria').attr('class', data.d.Respuesta.Controles[2]);
                                    if (data.d.Respuesta.Controles[3].length) $('#ModSolFechaDevengue').attr('class', data.d.Respuesta.Controles[3]);
                                    if (data.d.Respuesta.Controles[4].length) $('#ModSolFecUltActualizacion').attr('class', data.d.Respuesta.Controles[4]);
                                    if (data.d.Respuesta.Controles[5].length) $('#ModSolFechaRecepcion').attr('class', data.d.Respuesta.Controles[5]);
                                    if (data.d.Respuesta.Controles[6].length) $('#ModSolFechaPlazoAFP').attr('class', data.d.Respuesta.Controles[6]);
                                    if (data.d.Respuesta.Controles[7].length) $('#ModSolSaldoCIC').attr('class', data.d.Respuesta.Controles[7]);
                                    if (data.d.Respuesta.Controles[8].length) $('#ConModSolFactorTasa').attr('class', data.d.Respuesta.Controles[8]);
                                    if (data.d.Respuesta.Controles[9].length) $('#ModSolFechaCotizacion').attr('class', data.d.Respuesta.Controles[9]);
                                    if (data.d.Respuesta.Controles[10].length) $('#ModSolFechaSolicitudPension').attr('class', data.d.Respuesta.Controles[10]);
                                    if (data.d.Respuesta.Controles[11].length) $('#ModSolACOM').attr('class', data.d.Respuesta.Controles[11]);
                                    if (data.d.Respuesta.Controles[12].length) $('#ModSolDCOM').attr('class', data.d.Respuesta.Controles[12]);

                                    $('#TabCotizaciones select').removeClass('formTextboxGridError');
                                    if (data.d.Respuesta.Controles.length > 13) {
                                        var celdaError;
                                        for (i = 13; i < data.d.Respuesta.Controles.length; i++) {
                                            celdaError = data.d.Respuesta.Controles[i].split(',');
                                            $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                        }
                                    }
                                }
                                $('#ModalCuadroMensaje').dialog('open');
                            }

                            $('#ModSolCargando').fadeOut();
                            //ModSolBotonesInactivos = false;
                            $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
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
                    for (i = 0; i < $('#TabBeneficiarios tbody tr').length; i++) {
                        if ($('#TabBeneficiarios tbody tr:eq(' + i + ') input').is(':checked'))
                            idBeneficiarios.push(i);
                    }

                    //$('#ModSolCargando').fadeIn();
                    $('#MCIcono').attr('class', 'cargando');
                    $('#MCContenedor').html('Cotizando la solicitud, por favor espere un momento...');
                    $('#ModalCotizando').dialog({ title: 'Cotizando' });
                    $('#ModalCotizando').dialog('open');

                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        idSolicitud: $('#ModSolNroSolicitud').val(),
                        cuspp: $('#HCUSPP').val(),
                        afp: $('#HAFP').val(),
                        tipoCambio: $('#ModSolTipoCambio').val(),
                        tipoPension: $('#ModSolTipoPension').val(),
                        categoria: $('#ModSolCategoria').val(),
                        fechaDevengue: $('#ModSolFechaDevengue').val(),
                        fechaRecepcion: $('#ModSolFechaRecepcion').val(),
                        fechaPlazoAFP: $('#ModSolFechaPlazoAFP').val(),
                        saldoCIC: ($('#ModSolSaldoCIC').length > 0) ? $('#ModSolSaldoCIC').val() : $('#ModSolListaCIC').val(),
                        factorTasa: $('#ModSolFactorTasa').val(),
                        fechaCotizacion: $('#ModSolFechaCotizacion').val(),
                        fechaSolicitudPension: $('#ModSolFechaSolicitudPension').val(),
                        acom: ($('#ModSolACOM').length > 0) ? $('#ModSolACOM').val() : '',
                        dcom: ($('#ModSolDCOM').length > 0) ? $('#ModSolDCOM').val() : '',
                        cotizaciones: Solicitud.Cotizaciones,
                        idBeneficiarios: idBeneficiarios,
                        //<SRI.INI-20322_E2>
                        correo: Solicitud.Afiliado.CorreoElectronico,
                        numAgenteSol: Solicitud.Agente.Id
                        //<SRI.FIN-20322_E2>
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

                                CargarTablaSolicitudes();

                                // Cargar la cotización actualizada
                                $('#BeneficiariosOriginales').show();
                                $('#ManSolPestanhas li:eq(0)').trigger('click');

                                $('#ModSolCargando').show();

                                var params3 = {
                                    idSolicitud: $('#ModSolNroSolicitud').val(),
                                    fecCotizacion: $('#ModSolFechaCotizacion').val()
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
                                        $('#ModSolNroSolicitud').val(Solicitud.Id);
                                        $('#ModSolTipoCambio').val(formatearMonto(Solicitud.TipoCambio));
                                        $('#ModSolTipoPension').val(Solicitud.TipoPension.Id);
                                        $('#TexModSolTipoPension').html($('#ModSolTipoPension').find(':selected').text());
                                        $('#ModSolFechaDevengue').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolFecUltActualizacion').val(new Date(+Solicitud.FechaSolicitud.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolFechaRecepcion').val(new Date(+Solicitud.FechaRecepcion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolFechaPlazoAFP').val(new Date(+Solicitud.FechaPlazoAFP.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolSaldoCIC').val(formatearMonto(Solicitud.SaldoCIC));
                                        $('#ModSolListaCIC').val(formatearMonto2(Solicitud.SaldoCIC));
                                        $('#TexModSolListaCIC').html($('#ModSolListaCIC').find(':selected').text());
                                        $('#ModSolFactorTasa').val(Solicitud.FactorTasa);
                                        $('#TexModSolFactorTasa').html($('#ModSolFactorTasa').find(':selected').text());
                                        $('#ModSolFechaCotizacion').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolFechaSolicitudPension').val(new Date(+Solicitud.FechaSolicitudPension.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
                                        $('#ModSolACOM').val(formatearMonto(Solicitud.PorcentajeAumentoComision));
                                        $('#ModSolDCOM').val(formatearMonto(Solicitud.PorcentajeDescuentoComision));

                                        //<INIGTI_4022>
                                        if (Solicitud.Agente.IdNivel == 0) {
                                            $("#LabModSolNivel").attr('title', '');
                                        } else {
                                            $("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
                                        }
                                        //<FINGTI_4022>

                                        //CargarTablaBeneficiarios(null);
                                        CargarTablaBeneficiarios(Solicitud.Beneficiarios);

                                        /* Cargar Tabla de Cotizaciones con las cotizaciones obtenidas */
                                        var params = {
                                            idTipoPension: $('#ModSolTipoPension').val()
                                        }
                                        $.ajax({
                                            type: 'POST',
                                            url: 'Cotizador.aspx/CargarComboProductos',
                                            contentType: "application/json; charset=iso-8859-1",
                                            dataType: 'json',
                                            data: $.toJSON(params),
                                            success: function (data) {
                                                CargarTablaCotizaciones(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud').val());

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
                                                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de productos.');
                                                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                                    $('#ModalCuadroMensaje').dialog('open');
                                                }
                                            }
                                        });

                                        $('#ModSolCargando').fadeOut();
                                        $('#ModSolSaldoCIC').focus();
                                        $('#ModSolACOM').focus();
                                        $('#ModSolDCOM').focus();
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
                                    if (data.d.Respuesta.Controles[0].length) $('#ModSolTipoCambio').attr('class', data.d.Respuesta.Controles[0]);
                                    if (data.d.Respuesta.Controles[1].length) $('#ConModSolTipoPension').attr('class', data.d.Respuesta.Controles[1]);
                                    if (data.d.Respuesta.Controles[2].length) $('#ConModSolCategoria').attr('class', data.d.Respuesta.Controles[2]);
                                    if (data.d.Respuesta.Controles[3].length) $('#ModSolFechaDevengue').attr('class', data.d.Respuesta.Controles[3]);
                                    if (data.d.Respuesta.Controles[4].length) $('#ModSolFecUltActualizacion').attr('class', data.d.Respuesta.Controles[4]);
                                    if (data.d.Respuesta.Controles[5].length) $('#ModSolFechaRecepcion').attr('class', data.d.Respuesta.Controles[5]);
                                    if (data.d.Respuesta.Controles[6].length) $('#ModSolFechaPlazoAFP').attr('class', data.d.Respuesta.Controles[6]);
                                    if (data.d.Respuesta.Controles[7].length) $('#ModSolSaldoCIC').attr('class', data.d.Respuesta.Controles[7]);
                                    if (data.d.Respuesta.Controles[8].length) $('#ConModSolFactorTasa').attr('class', data.d.Respuesta.Controles[8]);
                                    if (data.d.Respuesta.Controles[9].length) $('#ModSolFechaCotizacion').attr('class', data.d.Respuesta.Controles[9]);
                                    if (data.d.Respuesta.Controles[10].length) $('#ModSolFechaSolicitudPension').attr('class', data.d.Respuesta.Controles[10]);
                                    if (data.d.Respuesta.Controles[11].length) $('#ModSolACOM').attr('class', data.d.Respuesta.Controles[11]);
                                    if (data.d.Respuesta.Controles[12].length) $('#ModSolDCOM').attr('class', data.d.Respuesta.Controles[12]);

                                    $('#TabCotizaciones select').removeClass('formTextboxGridError');
                                    if (data.d.Respuesta.Controles.length > 13) {
                                        var celdaError;
                                        for (i = 13; i < data.d.Respuesta.Controles.length; i++) {
                                            celdaError = data.d.Respuesta.Controles[i].split(',');
                                            $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                        }
                                    }
                                }
                                $('#ModalCuadroMensaje').dialog('open');
                            }

                            $('#ModSolCargando').fadeOut();
                            //ModSolBotonesInactivos = false;
                            $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
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
                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html('No se tiene registrado un Correo Electrónico para el afiliado. Por favor ingrese uno en la pestaña <strong>Datos del Afiliado</strong> y luego presione el botón <strong>Guardar</strong>.');
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
            }
        }
        else {
            return false;
        }
    });

    /* Botón Cancelar */
    $('#ModSolCancelar').live('click', function () {
        $('#ModalSolicitud').dialog('close');
    });



    /* ACTIVIDADES */
    var idActividad;
    var Actividad;
    $('#TabActividades .grilla_consultar').live('click', function () {

        $('#ModActCargando').show();

        var idActividad = $(this).data('actividad');

        var params = {
            cuspp: $('#CUSPP').val(),
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

            $("#CUSPP").removeClass("formTextboxError");
            $("#FechaInicio").removeClass("formCalendarError");
            $("#FechaTermino").removeClass("formCalendarError");

            // CUSPP
            var cuspp = true;
            var eCuspp = true;
            if ($.trim($("#CUSPP").val()).length > 0) {
                eCuspp = true;
                if ($.trim($("#CUSPP").val()).length != 12) {
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
            if (!cuspp) { $('#CUSPP').attr('class', 'formTextbox formTextboxError'); } else { $('#FechaInicio').attr('class', 'formTextbox'); }
            if (!fechaInicio) { $('#FechaInicio').attr('class', 'formTextbox formTextboxError formCalendarError'); } else { $('#FechaInicio').attr('class', 'formTextbox formCalendar'); }
            if (!fechaTermino) { $('#FechaTermino').attr('class', 'formTextbox formTextboxError formCalendarError'); } else { $('#FechaTermino').attr('class', 'formTextbox formCalendar'); }

            esCorrecto = cuspp & fechaInicio & fechaTermino;

            if (esCorrecto) {
                SegIdJefe = ($('#Jefe').val() != null) ? $('#Jefe').val() : null;
                SegIdSupervisor = ($('#Supervisor').val() != null) ? $('#Supervisor').val() : null;
                SegIdAgente = $('#Agente').val();
                SegCUSPP = ($('#CUSPP').val() != null) ? $('#CUSPP').val() : '';
                SegFechaInicio = $('#FechaInicio').val();
                SegFechaTermino = $('#FechaTermino').val();

                $('#HJefe').val(SegIdJefe);
                $('#HSupervisor').val(SegIdSupervisor);
                $('#HAgente').val(SegIdAgente);
                $('#HCUSPP').val(SegCUSPP);
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
            $('#HCUSPP').val(SegCUSPP);
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
                SegCUSPP = ($('#CUSPP').val() != null) ? $('#CUSPP').val() : '';
                SegFechaInicio = $('#FechaInicio').val();
                SegFechaTermino = $('#FechaTermino').val();

                $('#HJefe').val(SegIdJefe);
                $('#HSupervisor').val(SegIdSupervisor);
                $('#HAgente').val(SegIdAgente);
                $('#HCUSPP').val(SegCUSPP);
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
            $('#HCUSPP').val(SegCUSPP);
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

    //<INIGTI_4081>
    /* Botón Aceptar */
    $('#MCMAceptarFlujo').live('click', function () {
        $('#ModalCuadroMensaje').dialog('close');
        window.location.href = 'BandejaFlujoCotizacion.aspx';
    });
    //<FINGTI_4081>

    /* CUADRO DE ADVERTENCIA */
    /* Botón Aceptar */
    $('#MCAAceptar').live('click', function () {
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
            CargarTablaCotizaciones(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud').val());
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
                        CargarTablaGrupoFamiliar();
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
    $('#MCACancelar').live('click', function () {
        $('#ModalCuadroAdvertencia').dialog('close');
    });


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

                CargarTablaSolicitudesOficiales(SegIdJefe, SegIdSupervisor, SegIdAgente, true, null);
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

        //<INIGTI_754>
        //var maxAcom = $('input[type="radio"]:checked').val();
        var maxAcom = $('input:radio[name=id]:checked').val();
        //<FINGTI_754>

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

                //<INIGTI_4022>
                if (Solicitud.Agente.IdNivel == 0) {
                    $("#LabModSolNivel").attr('title', '');
                } else {
                    $("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
                }
                //<FINGTI_4022>

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
                            $('#ModalSeleccionAcom').dialog({ title: "ACOM - Escenario" });
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

        //<SOLINI25781>
        //<INIGTI_4081>//Se comenta
        //document.getElementById('ControlTRA').style.display = 'none';
        //<FINGTI_4081>
        //<SOLFIN25781>

        $('#ModSolCargando').show();

        LimpiarFormularioSolicitudOficial();

        //<INIGTI_4081>
        $('#ModSolCalcularPBS').hide();

        $('#ModSolACOM').attr('class', 'formTextbox');
        $('#ModSolDCOM').attr('class', 'formTextbox');
        $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox');
        $('#ConModSolIndSeleccionado').removeClass('formComboboxReadOnlyContenedor');
        $('#ConModSolCompania').removeClass('formComboboxReadOnlyContenedor');


        $('#ModSolACOM').attr('readonly', false);
        $('#ModSolDCOM').attr('readonly', false);
        $('#ModSolValMontoAcomAgente').attr('readonly', false);

        $('#ModSolIndSeleccionado').removeAttr('disabled');
        $('#ModSolCompania').removeAttr('disabled');

        //<FINGTI_4081>

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

                $('#ModSolNroSolicitud').html(SolicitudEscenario.NumSolicitud);
                $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
                $('#ModSolACOM').val(formatearMonto2(SolicitudEscenario.PjeAumentoComision));
                $('#ModSolDCOM').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
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

                //<INIGTI_4081>
                $('#ModSolCompania').val(SolicitudEscenario.Compania.Id);
                $('#TexModSolCompania').html($('#ModSolCompania').find(':selected').text());
                //<FINGTI_4081>

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

                        //<INIGTI_4022>
                        if (Solicitud.Agente.IdNivel == 0) {
                            //$("#LabModSolNivel").attr('title', '');
                            $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Sin Nivel)');
                        } else {
                            //$("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
                            $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Nivel:' + Solicitud.Agente.IdNivel + ')');
                        }
                        //<FINGTI_4022>

                        CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);
                        CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, false, true);

                        //<INIGTI_1092>
                        if ($('#HRedLocal').val() == "FALSE") {
                            $('#LabModSolLineaACOMDCOM').hide();
                            $('#LabModSolLineaSelecMon').hide();
                            $('#ControlTRA').hide();
                            //<INIGTI_4081>
                            $('#ModSolCalcularPBS').hide();
                            //<FINGTI_4081>
                        }
                        else {
                            botonModSolAceptarBloqueado = false;
                            $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');
                        }
                        //<FINGTI_1092>



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

        //<INIGTI_4081>
        $('#ModSolCalcularPBS').hide();
        //<FINGTI_4081>

        //<INIGTI_4081>
        $('#TablaCotizacionesOficialesContenedor').hide();
        $('#TablaCotizacionesOficialesCargando').show();
        //<FINGTI_4081>

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

                ApiCotizadorRV.ObtenerMontoAcom(SolicitudEscenario.NumSolicitud, SolicitudEscenario.PjeAumentoComision || 0, SolicitudEscenario.NumCotizacionElegida)
                    .then(response => {
                        $('#ModSolValMontoAcomAgente').val(formatearMonto(response.monto.toFixed(2)));
                    }).catch(err => {
                        console.error('Error al obtener monto acom', err)
                        $('#ModSolValMontoAcomAgente').val("")
                    })

                $('#ModSolNroSolicitud').html(SolicitudEscenario.NumSolicitud);
                $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
                $('#ModSolACOM').val(formatearMonto2(SolicitudEscenario.PjeAumentoComision));
                $('#ModSolDCOM').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
                $('#ModSolIndSeleccionado').val(SolicitudEscenario.IndEstadoSeleccion);
                $('#TexModSolIndSeleccionado').html($('#ModSolIndSeleccionado').find(':selected').text());
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

                //<INIGTI_4081>
                $('#ModSolCompania').val(SolicitudEscenario.Compania.Id);
                $('#TexModSolCompania').html($('#ModSolCompania').find(':selected').text());
                //<FINGTI_4081>


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

                        //<INIGTI_4022>
                        if (Solicitud.Agente.IdNivel == 0) {
                            //$("#LabModSolNivel").attr('title', '');
                            $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Sin Nivel)');
                        } else {
                            //$("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
                            $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Nivel:' + Solicitud.Agente.IdNivel + ')');
                        }
                        //<FINGTI_4022>

                        CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);

                        $('#HCodTipoMovimiento').val(Solicitud.TipoMovimiento.Id);

                        //<SOLINI25781> //obtener el valor mas pequeño
                        //<INIGTI_4081>//SE COMENTA
                        ////var minimoTRA = 0;
                        ////for (i = 0; i < Solicitud.Cotizaciones.length; i++) {

                        ////    if (i == 0) {
                        ////        minimoTRA = Solicitud.Cotizaciones[i].AjusteTRA;
                        ////    }
                        ////    if (Solicitud.Cotizaciones[i].AjusteTRA < minimoTRA) {
                        ////        minimoTRA = Solicitud.Cotizaciones[i].AjusteTRA;
                        ////    }
                        ////}
                        ////$('#ModSolTRA').val(minimoTRA);
                        //<FINGTI_4081>
                        //<SOLFIN25781>

                        if (Solicitud.TipoMovimiento.Id == 0) {
                            botonModSolAceptarBloqueado = false;
                            permisoTra = true;
                            $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');

                            //<SOLINI25781>//true=muestra la caja de texto TRA.
                            //<INIGTI_4081>
                            ////if ($("#HOcultraColumnaTRA").val() == "TRUE") {
                            ////    document.getElementById('ControlTRA').style.display = 'block';
                            ////} else {
                            ////    document.getElementById('ControlTRA').style.display = 'none';
                            ////}
                            //<FINGTI_4081>
                            //<SOLFIN25781>

                            //<INIGTI_1092>
                            if ($('#HRedLocal').val() == "FALSE") {//FALSE = Externo, no es red local
                                $('#LabModSolLineaACOMDCOM').hide();
                                $('#LabModSolLineaSelecMon').hide();
                                $('#ControlTRA').hide();
                            }
                            else {
                                botonModSolAceptarBloqueado = false;
                                $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');
                            }
                            //<FINGTI_1092>


                        }
                        else {
                            botonModSolAceptarBloqueado = true;
                            permisoTra = true; //<INIGTI_4081>
                            $('#ModSolAceptarOficial').attr('class', 'botonDeshabilitado gris gris_sharp');

                            //<INIGTI_4081>
                            $('#ModSolCalcularPBS').hide();
                            //<FINGTI_4081>

                            //<SOLINI25781>
                            //<INIGTI_4081>//SE COMENTA
                            ////document.getElementById('ControlTRA').style.display = 'none';
                            //<FINGTI_4081>
                            //<SOLFIN25781>
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

                                //<INIGTI_4081>
                                if (Solicitud.TipoMovimiento.Id == 0) {

                                    //<INIGTI_4081>
                                    //Si es igual son oficiales
                                    if (SolicitudEscenario.NumOperacion.toString() == SolicitudEscenario.NumSolicitud) {
                                        botonModSolAceptarBloqueado = true;
                                        $('#ModSolAceptarOficial').attr('class', 'botonDeshabilitado gris gris_sharp');
                                        $('#ModSolCalcularPBS').hide();

                                        $('#ModSolACOM').attr('class', 'formTextbox formTextboxReadOnly');
                                        $('#ModSolDCOM').attr('class', 'formTextbox formTextboxReadOnly');
                                        $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox formTextboxReadOnly');
                                        $('#ConModSolIndSeleccionado').addClass('formComboboxReadOnlyContenedor');
                                        $('#ConModSolCompania').addClass('formComboboxReadOnlyContenedor');

                                        $('#ModSolACOM').attr('readonly', true);
                                        $('#ModSolDCOM').attr('readonly', true);
                                        $('#ModSolValMontoAcomAgente').attr('readonly', true);

                                        $('#ModSolIndSeleccionado').attr('disabled', 'disabled');
                                        $('#ModSolCompania').attr('disabled', 'disabled');

                                        //<GTI.INI-29372>
                                        if ($('#hdnMostrarEnvioObligatorio').val() == "1") {
                                            $('#ModSolAceptarEnvioObligatorio').show();
                                        }
                                        else {
                                            $('#ModSolAceptarEnvioObligatorio').hide();
                                        }
                                        //<GTI.FIN-29372>

                                    } else {//Sino, son escenarios
                                        botonModSolAceptarBloqueado = false;
                                        $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');

                                        if ($('#HRedLocal').val() == "FALSE") {//FALSE = Externo, no es red local
                                            $('#ModSolCalcularPBS').hide();
                                        }
                                        else {
                                            $('#ModSolCalcularPBS').show();
                                        }


                                        $('#ModSolACOM').attr('class', 'formTextbox');
                                        $('#ModSolDCOM').attr('class', 'formTextbox');
                                        $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox');
                                        $('#ConModSolIndSeleccionado').removeClass('formComboboxReadOnlyContenedor');
                                        $('#ConModSolCompania').removeClass('formComboboxReadOnlyContenedor');


                                        $('#ModSolACOM').attr('readonly', false);
                                        $('#ModSolDCOM').attr('readonly', false);
                                        $('#ModSolValMontoAcomAgente').attr('readonly', false);

                                        $('#ModSolIndSeleccionado').removeAttr('disabled');
                                        $('#ModSolCompania').removeAttr('disabled');

                                        //<GTI.INI-29372>
                                        $('#ModSolAceptarEnvioObligatorio').hide();
                                        //<GTI.FIN-29372>

                                    }
                                    //<FINGTI_4081>

                                } else {
                                    $('#ModSolACOM').attr('class', 'formTextbox formTextboxReadOnly');
                                    $('#ModSolDCOM').attr('class', 'formTextbox formTextboxReadOnly');
                                    $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox formTextboxReadOnly');
                                    $('#ConModSolIndSeleccionado').addClass('formComboboxReadOnlyContenedor');
                                    $('#ConModSolCompania').addClass('formComboboxReadOnlyContenedor');

                                    $('#ModSolACOM').attr('readonly', true);
                                    $('#ModSolDCOM').attr('readonly', true);
                                    $('#ModSolValMontoAcomAgente').attr('readonly', true);

                                    $('#ModSolIndSeleccionado').attr('disabled', 'disabled');
                                    $('#ModSolCompania').attr('disabled', 'disabled');

                                }
                                //<FINGTI_4081>

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

        //$('#ModalSolicitud').dialog('open');
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

                //<INIGTI_4022>
                if (Solicitud.Agente.IdNivel == 0) {
                    $("#LabModSolNivel").attr('title', '');
                } else {
                    $("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
                }
                //<FINGTI_4022>

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
                        if (data.data.Respuesta.Estado === 'OK') {
                            // Imprimir número de solicitud generada
                            $('#ModSolNroSolicitud').html(data.data.NumSolicitud);
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
                        else if (data.data.Estado === 'TOKEN') {
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
                        $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
                    })
                    .catch(error => {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');

                        $('#ModalCotizando').dialog('close');
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
    $("#ModSolAceptarOficial").live("click", function () {
        if (!botonModSolAceptarBloqueado) {
            // Validaciones
            var esCorrecto = true;
            var errores = new Array();

            $("#ModSolACOM").removeClass("formTextboxError");
            $("#ModSolDCOM").removeClass("formTextboxError");
            $("#ModSolIndSeleccionado").removeClass("formComboboxErrorContenedor");

            $("#ModSolValMontoAcomAgente").removeClass("formTextboxError");
            //<INIGTI_4081>
            $("#ConModSolIndSeleccionado").removeClass("formComboboxErrorContenedor");
            $("#ConModSolCompania").removeClass("formComboboxErrorContenedor");
            //<FINGTI_4081>

            // ACOM
            var vacom = true;
            if ($.trim($("#ModSolACOM").val()).length == 0) {
                errores.push("Ingrese el campo <strong>Porcentaje A</strong>. Dato Obligatorio.");
                vacom = false;
            }

            // DCOM
            var vdcom = true;
            if ($.trim($("#ModSolDCOM").val()).length == 0) {
                errores.push("Ingrese el campo <strong>Porcentaje D</strong>. Dato Obligatorio.");
                vdcom = false;
            }

            // Ind. Seleccionado
            var vindseleccionado = true;
            if ($("#ModSolIndSeleccionado").val() == "0") {
                errores.push("Ingrese el campo <strong>Ind. Seleccionado</strong>. Dato Obligatorio.");
                vindseleccionado = false;
            }

            // Seleccionar Cotización
            var idSolicitudElegida = 0;

            if ($("#TabCotizaciones input[type=radio]:checked").length > 0) {
                idSolicitudElegida = $("#TabCotizaciones input[type=radio]:checked").val();
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
                } else {
                    //<INIGTI_4081>
                    if ($("#ModSolACOM").val() > 0 && $("#ModSolIndSeleccionado").val() == "S") {
                        if (idSolicitudElegida == 0) {
                            errores.push("Asegúrese de haber seleccionado una modalidad para que el sistema pueda calcular el monto.");
                            vmontoacom = false;
                        }
                    }
                    //<FINGTI_4081>
                }
            }



            //<INIGTI_4081>
            var vcompania = true;
            var bAjusteTRa = false;
            for (var i = 0; i < Solicitud.Cotizaciones.length; i++) {
                if (Solicitud.Cotizaciones[i].AjusteTRA != 0) {
                    bAjusteTRa = true;
                }
            }

            if (bAjusteTRa) {
                if ($("#ModSolCompania").val() == "0") {
                    errores.push("Ingrese el campo <strong>Compañia</strong>. Dato Obligatorio.");
                    vcompania = false;
                }
            }

            //<FINGTI_4081>

            // Clases de controles
            if (!vacom) $("#ModSolACOM").addClass("formTextboxError");
            if (!vdcom) $("#ModSolDCOM").addClass("formTextboxError");
            if (!vindseleccionado) $("#ConModSolIndSeleccionado").addClass("formComboboxErrorContenedor");
            if (!vmontoacom) $("#ModSolValMontoAcomAgente").addClass("formTextboxError");

            //<INIGTI_4081>
            if (!vcompania) $("#ConModSolCompania").addClass("formComboboxErrorContenedor");
            //<FINGTI_4081>

            esCorrecto = vacom & vdcom & vindseleccionado & vmontoacom & vcompania;//<INIGTI_4081>//

            if (esCorrecto) {
                // Nuevo Escenario Oficial
                if ($("#ModSolModo").val() == "N") {
                    $("#MCIcono").attr("class", "cargando");
                    $("#MCContenedor").html("Cotizando la solicitud, por favor espere un momento...");
                    $("#ModalCotizando").dialog({ title: "Cotizando" });
                    $("#ModalCotizando").dialog("open");
                    //<INIGTI_4081>

                    // Validar la Seleccion de Solicitud
                    var params = {
                        tokenUsuario: $("#TokenUsuario").val(),
                        NumSolicitud: "0",
                        NumOperacion: SolicitudEscenario.NumOperacion,
                        IndSeleccion: $("#ModSolIndSeleccionado").val()
                    }
                    $.ajax({
                        type: "POST",
                        url: "CotizadorOficiales.aspx/ValidarSeleccionSolicitud",
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: "json",
                        data: $.toJSON(params),
                        success: function (data) {
                            if (data.d.Estado == "OK") {
                                if (data.d.Contenido == "") {

                                    var params = {
                                        //tokenUsuario: $("#TokenUsuario").val(),
                                        idSolicitud: $("#HNumSolicitud").val(),
                                        fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
                                        acom: $("#ModSolACOM").val(),
                                        dcom: $("#ModSolDCOM").val(),
                                        indEstadoSeleccion: $("#ModSolIndSeleccionado").val(),
                                        valMtoAgenteAcom: $("#ModSolValMontoAcomAgente").val(),
                                        numCotizacionElegida: (idSolicitudElegida == null) ? null : idSolicitudElegida,
                                        correo: Solicitud.Afiliado.CorreoElectronico,
                                        cod_compania: $("#ModSolCompania").val()//<INIGTI_4081>
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
                                            .then(response => response.json())
                                            .then(data => {
                                                if (data.data.Respuesta.Estado == 'OK') {
                                                    // Imprimir número de solicitud generada
                                                    $('#ModSolNroSolicitud').html(data.data.NumSolicitud);
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

                                                    CargarTablaSolicitudesOficiales($("#HJefe").val(), $("#HSupervisor").val(), $("#HAgente").val(), false, data.data.NumSolicitud);

                                                    // Cargar la cotización actualizada
                                                    $('#BeneficiariosOriginales').show();
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

                                                            $('#ModSolNroSolicitud').html(SolicitudEscenario.NumSolicitud);
                                                            $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
                                                            $('#ModSolACOM').val(formatearMonto2(SolicitudEscenario.PjeAumentoComision));
                                                            $('#ModSolDCOM').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
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

                                                            if ($('#HRedLocal').val() == "FALSE") {
                                                                $('#ModSolCalcularPBS').hide();
                                                            }
                                                            else {
                                                                $('#ModSolCalcularPBS').show();
                                                            }

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

                                                                    if (Solicitud.Agente.IdNivel == 0) {
                                                                        $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Sin Nivel');
                                                                    } else {
                                                                        $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Nivel:' + Solicitud.Agente.IdNivel + ')');
                                                                    }

                                                                    CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);
                                                                    CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

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
                                                else if (data.data.Estado == 'TOKEN') {
                                                    CerrarSesionExpirada();
                                                }
                                                else {
                                                    $('#ModalCotizando').dialog('close');
                                                    $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                                                    $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                                                    $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                                                    if (data.data.Respuesta.Controles != null && data.data.Respuesta.Controles > 0) {
                                                        if (data.data.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.data.Respuesta.Controles[0]);
                                                        if (data.data.Respuesta.Controles[1].length) $('#ModSolDCOM').attr('class', data.data.Respuesta.Controles[1]);
                                                        if (data.data.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.data.Respuesta.Controles[2]);
                                                        if (data.data.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.data.Respuesta.Controles[3]);

                                                        $('#TabCotizaciones select').removeClass('formTextboxGridError');
                                                        if (data.data.Respuesta.Controles.length > 13) {
                                                            var celdaError;
                                                            for (i = 13; i < data.data.Respuesta.Controles.length; i++) {
                                                                celdaError = data.data.Respuesta.Controles[i].split(',');
                                                                $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                                            }
                                                        }
                                                    }
                                                    $('#ModalCuadroMensaje').dialog('open');
                                                }
                                                $('#ModSolCargando').fadeOut();
                                            })
                                            .catch(error => {
                                                if (error.status === 401 || error.status === 12030) {
                                                    document.location.reload(true);
                                                } else {
                                                    $('#MCMIcono').attr('class', 'error');
                                                    $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                                                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                                    $('#ModalCuadroMensaje').dialog('open');
                                                    $('#ModalCotizando').dialog('close');
                                                }
                                            })
                                            .finally(() => {
                                                botonModSolAceptarBloqueado = false;
                                                $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
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
                                    //Existe otra solicitud seleccionada y en flujo o aprobada
                                    if (data.d.Titulo == "Confirmación") {
                                        // Los rangos NO están dentro de lo permitido por el usuario, mostrar el mensaje
                                        // de confirmación para enviar dicho mensaje al flujo de aprobación.
                                        $("#ModalCotizando").dialog("close");

                                        $("#MCAIcono").attr("class", "advertencia");
                                        $("#MCAContenedor").html(data.d.Contenido);
                                        $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                                        $("#ModalCuadroAdvertencia").dialog("open");

                                        //<INIGTI_4081>
                                        $("#MCAEstado").val("VALIDASELECCION_INS");
                                        //<FINGTI_4081>

                                    } else {
                                        //Mostrando el Error.
                                        $("#ModalCotizando").dialog("close");
                                        $('#MCMIcono').attr('class', "validacion");
                                        $('#MCMContenedor').html(data.d.Contenido);
                                        $('#ModalCuadroMensaje').dialog({ title: "Validación" });
                                        $('#ModalCuadroMensaje').dialog('open');
                                    }
                                }
                            }
                            else if (data.d.Estado == "TOKEN") {
                                CerrarSesionExpirada();
                            }
                            else if (data.d.Estado == "ERROR") {
                                // Mostrar mensaje de error
                                $('#MCMIcono').attr('class', data.d.Icono);
                                $('#MCMContenedor').html(data.d.Mensaje);
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                $('#ModalCuadroMensaje').dialog('open');
                            }
                        }
                    });

                    //<FINGTI_4081>
                }
                // Modificar Escenario (TRA)
                else if ($("#ModSolModo").val() == "M") {

                    $("#MCIcono").attr("class", "cargando");
                    $("#MCContenedor").html("Guardando datos de la cotización, por favor espere un momento...");
                    $("#ModalCotizando").dialog({ title: "Cotizando" });
                    $("#ModalCotizando").dialog("open");

                    //<INIGTI_4081>
                    // Validar si el usuario excede sus parámetros permitidos de ACOM y TRA

                    // Validar si el usuario excede sus parámetros permitidos de ACOM y TRA
                    var params = {
                        idSolicitud: $("#HNumSolicitud").val(),
                        //tokenUsuario: $("#TokenUsuario").val(),
                        fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
                        acom: $("#ModSolACOM").val(),
                        cotizaciones: Solicitud.Cotizaciones,
                        rechazo: false//<INIGTI_4081>
                    }

                    fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/validar-acom-tra', {
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

                                    //<INIGTI_4081>
                                    // Validar la Seleccion de Solicitud
                                    var params = {
                                        tokenUsuario: $("#TokenUsuario").val(),
                                        NumSolicitud: $("#HNumSolicitud").val(),
                                        NumOperacion: SolicitudEscenario.NumOperacion,
                                        IndSeleccion: $("#ModSolIndSeleccionado").val()
                                    }

                                    return fetch('CotizadorOficiales.aspx/ValidarSeleccionSolicitud', {
                                        method: 'POST',
                                        headers: {
                                            'Content-Type': 'application/json; charset=iso-8859-1'
                                        },
                                        body: JSON.stringify(params)
                                    });
                                } else {
                                    if (data.data.Titulo == "Confirmación") {
                                        //<INIGTI_4081>
                                        if ($("#ModSolCompania").val() == "0") {
                                            $("#ModalCotizando").dialog("close");
                                            $("#ConModSolCompania").addClass("formComboboxErrorContenedor");
                                            $('#MCMIcono').attr('class', 'validacion');
                                            $('#MCMContenedor').html("Ingrese el campo <strong>Compañia</strong>. Dato Obligatorio.");
                                            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                                            $('#ModalCuadroMensaje').dialog('open');
                                            throw new Error("Validación de compañía");
                                        }
                                        //<FINGTI_4081>

                                        // Los rangos NO están dentro de lo permitido por el usuario
                                        $("#ModalCotizando").dialog("close");
                                        $("#MCAIcono").attr("class", "advertencia");
                                        $("#MCAContenedor").html(data.data.Contenido);
                                        $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                                        $("#ModalCuadroAdvertencia").dialog("open");
                                        //<INIGTI_4081>
                                        $("#MCAEstado").val("VALIDAACOMTRA");
                                        //<FINGTI_4081>
                                    } else {
                                        $("#ModalCotizando").dialog("close");
                                        $('#MCMIcono').attr('class', "validacion");
                                        $('#MCMContenedor').html(data.data.Contenido);
                                        $('#ModalCuadroMensaje').dialog({ title: "Validación" });
                                        $('#ModalCuadroMensaje').dialog('open');
                                    }
                                    throw new Error("Validación fallida");
                                }
                            } else if (data.data.Estado == "TOKEN") {
                                CerrarSesionExpirada();
                                throw new Error("Sesión token expirada");
                            } else if (data.data.Estado == "ERROR") {
                                $('#ModalCotizando').dialog('close');
                                $('#MCMIcono').attr('class', data.data.Icono);
                                $('#MCMContenedor').html(data.data.Mensaje);
                                $('#ModalCuadroMensaje').dialog({ title: data.data.Titulo });
                                $('#ModalCuadroMensaje').dialog('open');
                                throw new Error("Error en la respuesta");
                            }
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
                            if (data.d.Estado == "OK") {
                                if (data.d.Contenido == "") {
                                    // Guardar los datos de la solicitud
                                    var params = {
                                        //tokenUsuario: $("#TokenUsuario").val(),
                                        idSolicitud: $("#HNumSolicitud").val(),
                                        fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
                                        acom: $("#ModSolACOM").val(),
                                        dcom: $("#ModSolDCOM").val(),
                                        indEstadoSeleccion: $("#ModSolIndSeleccionado").val(),
                                        valMtoAgenteAcom: $("#ModSolValMontoAcomAgente").val(),
                                        numCotizacionElegida: (idSolicitudElegida == null) ? null : idSolicitudElegida,
                                        cotizaciones: Solicitud.Cotizaciones,
                                        correo: Solicitud.Afiliado.CorreoElectronico,
                                        cod_compania: $("#ModSolCompania").val()
                                    }

                                    let tienePermiso = validarPermisoCWRV(opcionesSistema, 17);
                                    if (!tienePermiso) {
                                        $('#MCMIcono').attr('class', 'error');
                                        $('#MCMContenedor').html("Usted no tiene privilegios sobre esta opción.");
                                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                        $('#ModalCuadroMensaje').dialog('open');
                                        $('#ModalCotizando').dialog('close');
                                        throw new Error("Sin privilegios");
                                    }

                                    return fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/modificar', {
                                        method: 'POST',
                                        headers: {
                                            'Content-Type': 'application/json; charset=iso-8859-1',
                                            'x-username': $('#usuario_actual').val(),
                                            'x-rol': $('#rol_azman').val()
                                        },
                                        body: JSON.stringify(params)
                                    });
                                } else {
                                    if (data.d.Titulo == "Confirmación") {
                                        $("#ModalCotizando").dialog("close");
                                        $("#MCAIcono").attr("class", "advertencia");
                                        $("#MCAContenedor").html(data.d.Contenido);
                                        $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                                        $("#ModalCuadroAdvertencia").dialog("open");
                                        //<INIGTI_4081>
                                        $("#MCAEstado").val("VALIDASELECCION_MOD");
                                        //<FINGTI_4081>
                                    } else {
                                        $("#ModalCotizando").dialog("close");
                                        $('#MCMIcono').attr('class', "validacion");
                                        $('#MCMContenedor').html(data.d.Contenido);
                                        $('#ModalCuadroMensaje').dialog({ title: "Validación" });
                                        $('#ModalCuadroMensaje').dialog('open');
                                    }
                                    throw new Error("Validación fallida");
                                }
                            } else if (data.d.Estado == "TOKEN") {
                                CerrarSesionExpirada();
                                throw new Error("Sesión token expirada");
                            } else {
                                $('#MCMIcono').attr('class', data.d.Icono);
                                $('#MCMContenedor').html(data.d.Mensaje);
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                $('#ModalCuadroMensaje').dialog('open');
                                throw new Error("Error en la respuesta");
                            }
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

                                CargarTablaSolicitudesOficiales($("#HJefe").val(), $("#HSupervisor").val(), $("#HAgente").val(), false, data.data.NumSolicitud);

                                //<INIGTI_4081>
                                var paramsInner = {
                                    idSolicitud: $("#HNumSolicitud").val(),
                                    fecCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy")
                                }

                                return fetch('CotizadorOficiales.aspx/ObtenerDatosSolicitud', {
                                    method: 'POST',
                                    headers: {
                                        'Content-Type': 'application/json; charset=iso-8859-1'
                                    },
                                    body: JSON.stringify(paramsInner)
                                });
                            } else if (data.data.Estado == 'TOKEN') {
                                CerrarSesionExpirada();
                                throw new Error("Sesión token expirada");
                            } else {
                                $('#ModalCotizando').dialog('close');
                                $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                                $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                                $('#MCMContenedor').html(data.data.Respuesta.Mensaje);

                                if (data.data.Respuesta.Controles != null && data.data.Respuesta.Controles > 0) {
                                    if (data.data.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.data.Respuesta.Controles[0]);
                                    if (data.data.Respuesta.Controles[1].length) $('#ModSolDCOM').attr('class', data.data.Respuesta.Controles[1]);
                                    if (data.data.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.data.Respuesta.Controles[2]);
                                    if (data.data.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.data.Respuesta.Controles[3]);

                                    if (data.data.Respuesta.Controles.length > 13) {
                                        var celdaError;
                                        for (i = 13; i < data.data.Respuesta.Controles.length; i++) {
                                            celdaError = data.data.Respuesta.Controles[i].split(',');
                                            $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                        }
                                    }
                                }
                                $('#ModalCuadroMensaje').dialog('open');
                                throw new Error("Error en la respuesta");
                            }
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
                            Solicitud = data.d;
                            //<INIGTI_4081>
                            SolicitudEscenario.NumCotizacionElegida = idSolicitudElegida;
                            //<FINGTI_4081>

                            CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

                            //<INIGTI_1092>
                            if ($('#HRedLocal').val() == "FALSE") {
                                $('#LabModSolLineaACOMDCOM').hide();
                                $('#LabModSolLineaSelecMon').hide();
                                $('#ControlTRA').hide();
                            } else {
                                botonModSolAceptarBloqueado = false;
                                $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');
                            }
                            //<FINGTI_1092>

                            //<INIGTI_4081>
                            if (Solicitud.TipoMovimiento.Id == 0) {
                                $('#ModSolACOM').attr('class', 'formTextbox');
                                $('#ModSolDCOM').attr('class', 'formTextbox');
                                $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox');
                                $('#ConModSolIndSeleccionado').removeClass('formComboboxReadOnlyContenedor');
                                $('#ConModSolCompania').removeClass('formComboboxReadOnlyContenedor');

                                $('#ModSolACOM').attr('readonly', false);
                                $('#ModSolDCOM').attr('readonly', false);
                                $('#ModSolValMontoAcomAgente').attr('readonly', false);

                                $('#ModSolIndSeleccionado').removeAttr('disabled');
                                $('#ModSolCompania').removeAttr('disabled');

                                //<INIGTI_4081>
                                if ($('#HRedLocal').val() == "FALSE") {//FALSE = Externo, no es red local
                                    $('#ModSolCalcularPBS').hide();
                                } else {
                                    $('#ModSolCalcularPBS').show();
                                }
                                //<FINGTI_4081>
                            } else {
                                $('#ModSolACOM').attr('class', 'formTextbox formTextboxReadOnly');
                                $('#ModSolDCOM').attr('class', 'formTextbox formTextboxReadOnly');
                                $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox formTextboxReadOnly');
                                $('#ConModSolIndSeleccionado').addClass('formComboboxReadOnlyContenedor');
                                $('#ConModSolCompania').addClass('formComboboxReadOnlyContenedor');

                                $('#ModSolACOM').attr('readonly', true);
                                $('#ModSolDCOM').attr('readonly', true);
                                $('#ModSolValMontoAcomAgente').attr('readonly', true);

                                $('#ModSolIndSeleccionado').attr('disabled', 'disabled');
                                $('#ModSolCompania').attr('disabled', 'disabled');
                            }
                            //<FINGTI_4081>

                            // Actualizar temporizador
                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                            $clock.countdown(selectedDate.toString());

                            $('#ModSolCargando').fadeOut();
                            $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
                        })
                        .catch(error => {
                            if (error.message === "Sesión expirada" || error.message === "Sesión token expirada") {
                                document.location.reload(true);
                            } else if (error.message !== "Validación fallida" && error.message !== "Sin privilegios" && error.message !== "Validación de compañía") {
                                $("#ModalCotizando").dialog("close");
                                $("#MCMIcono").attr("class", "error");
                                $("#MCMContenedor").html("Ha ocurrido un error al procesar la solicitud.");
                                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                                $("#ModalCuadroMensaje").dialog("open");
                            }
                            $('#ModSolCargando').fadeOut();
                            $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
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

    //<INIGTI_4081>
    $("#ModSolCalcularPBS").live("click", function () {

        //Retorna si esta bloqueado
        if (botonModSolAceptarBloqueado) {
            return;
        }
        botonModSolAceptarBloqueado = true;
        $('#ModSolCalcularPBS').attr('class', 'botonDeshabilitado gris gris_sharp');


        $('#MCIcono').attr('class', 'cargando');
        //$('#MCContenedor').html('Cotizando la solicitud, por favor espere un momento...');
        $('#MCContenedor').html('Calculando el PBS, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Calculando' });
        $('#ModalCotizando').dialog('open');


        // Seleccionar Cotización
        var idSolicitudElegida = 0;

        if ($("#TabCotizaciones input[type=radio]:checked").length > 0) {
            idSolicitudElegida = $("#TabCotizaciones input[type=radio]:checked").val();
        }
        var params = {
            idSolicitud: $("#HNumSolicitud").val(),
            fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
            //tokenUsuario: $("#TokenUsuario").val(),
            cotizaciones: Solicitud.Cotizaciones,
            acom: $("#ModSolACOM").val(),
            dcom: $("#ModSolDCOM").val(),
            actualizaMovimiento: false,
            tipoMovimiento: Solicitud.TipoMovimiento.Id,
            montoAcom: $("#ModSolValMontoAcomAgente").val()
        }

        fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/calcular-pbs', {
            method: "POST",
            headers: {
                "Content-Type": "application/json; charset=iso-8859-1",
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
                if (data.data.Respuesta.Estado == "ERROR") {
                    $('#ModalCotizando').dialog('close');

                    // Mostrar mensaje de error
                    $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                    $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                else {
                    Solicitud.Cotizaciones = data.data.Cotizaciones;
                    SolicitudEscenario.NumCotizacionElegida = idSolicitudElegida;
                    CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);
                }

                botonModSolAceptarBloqueado = false;
                $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                $('#ModalCotizando').dialog('close');
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
            })
            .finally(() => {
                botonModSolAceptarBloqueado = false;
                $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                $('#ModalCotizando').dialog('close');
            });

    });


    $("#ModSolCalcularPBSBandeja").live("click", function () {

        //Retorna si esta bloqueado
        if (botonModSolAceptarBloqueado) {
            return;
        }
        botonModSolAceptarBloqueado = true;
        $('#ModSolCalcularPBS').attr('class', 'botonDeshabilitado gris gris_sharp');


        $('#MCIcono').attr('class', 'cargando');
        //$('#MCContenedor').html('Cotizando la solicitud, por favor espere un momento...');
        $('#MCContenedor').html('Calculando el PBS, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Calculando' });
        $('#ModalCotizando').dialog('open');

        //Registrando los parametros especiales, para quien tenga acceso

        try {

            if ($('#HGrupoEspeciales').val() == "TRUE") {
                for (var n = 0; n < ParametroEspecial.length; n++) {
                    ParametroEspecial[n].FecIniRangoStr = new Date(+ParametroEspecialNew[n].FecIniRango.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy");
                    ParametroEspecial[n].FecFinRangoStr = new Date(+ParametroEspecialNew[n].FecFinRango.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy");
                    ParametroEspecial[n].FecIniRango = null;
                    ParametroEspecial[n].FecFinRango = null;
                };
            } else {
                ParametroEspecial = null;
                modifica = false;
            }
        }
        catch (err) {
            //document.getElementById("demo").innerHTML = err.message;
            $("#ModalCotizando").dialog("close");

            $("#MCMIcono").attr("class", "error");
            $("#MCMContenedor").html("Ha ocurrido un error al calcular el PBS de la solicitud.");
            $("#ModalCuadroMensaje").dialog({ title: "Error" });
            $("#ModalCuadroMensaje").dialog("open");

            botonModSolAceptarBloqueado = false;
            $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
            return;
        }

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            parametros: ParametroEspecial,
            modifica: modificaEspecial
        }


        $.ajax({
            type: "POST",
            url: "BandejaFlujoCotizacionModificar.aspx/RegistrarCotizaValPar",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    //Calculando los PBS
                    const fechaCotizacion = new Date(Solicitud.FechaCotizacion).toString("dd/MM/yyyy");
                    var params = {
                        idSolicitud: $("#HNumSolicitud").val(),
                        fechaCotizacion: fechaCotizacion,
                        //tokenUsuario: $("#TokenUsuario").val(),
                        cotizaciones: Solicitud.Cotizaciones,
                        acom: $("#ModSolACOM").val(),
                        dcom: $("#ModSolDCOM").val(),
                        actualizaMovimiento: false,
                        tipoMovimiento: Solicitud.TipoMovimiento.Id,
                        montoAcom: $("#ModSolValMontoAcomAgente").val()
                    }

                    fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/calcular-pbs', {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json; charset=iso-8859-1",
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
                            if (data.data.Respuesta.Estado == "ERROR") {
                                $('#ModalCotizando').dialog('close');
                                // Mostrar mensaje de error
                                $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                                $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                                $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                                $('#ModalCuadroMensaje').dialog('open');
                            }
                            else {
                                Solicitud.Cotizaciones = data.data.Cotizaciones;
                                CargarTablaCotizacionesOficialesBandejaMovil(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);
                            }

                            botonModSolAceptarBloqueado = false;
                            $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                            $('#ModalCotizando').dialog('close');
                        })
                        .catch(error => {
                            if (error.message === "Sesión expirada") {
                                document.location.reload(true);
                            } else {
                                $("#ModalCotizando").dialog("close");
                                $("#MCMIcono").attr("class", "error");
                                $("#MCMContenedor").html("Ha ocurrido un error al calcular el PBS de la solicitud.");
                                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                                $("#ModalCuadroMensaje").dialog("open");
                            }
                            botonModSolAceptarBloqueado = false;
                            $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                            $('#ModalCotizando').dialog('close');
                        })
                        .finally(() => {
                            botonModSolAceptarBloqueado = false;
                            $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                            $('#ModalCotizando').dialog('close');
                        });
                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
                else if (data.d.Estado == "ERROR") {
                    // Mostrar mensaje de error
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                //    document.location.reload(true);
                //}
                //else {
                //    $("#ModalCotizando").dialog("close");

                //    $("#MCMIcono").attr("class", "error");
                //    $("#MCMContenedor").html("Ha ocurrido un error al guardar los datos de la solicitud.");
                //    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                //    $("#ModalCuadroMensaje").dialog("open");
                //}
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $("#ModalCotizando").dialog("close");

                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al calcular el PBS de la solicitud.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
                botonModSolAceptarBloqueado = false;
                $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                $('#ModalCotizando').dialog('close');
            },
            complete: function () {

            }
        });




    });

    //<FINGTI_4081>

    //<INIGTI_4081_2>
    $("#ModSolGrabarPBSBandeja").live("click", function () {

        //Retorna si esta bloqueado
        if (botonModSolAceptarBloqueado) {
            return;
        }

        //<INIGTI_7012>
        var esCorrecto = true;
        var errores = new Array();

        $("#ModSolACOM").removeClass("formTextboxError");
        $("#ModSolDCOM").removeClass("formTextboxError");


        $("#ModSolValMontoAcomAgente").removeClass("formTextboxError");

        // ACOM
        var vacom = true;
        if ($.trim($("#ModSolACOM").val()).length == 0) {
            errores.push("Ingrese el campo <strong>Porcentaje A</strong>. Dato Obligatorio.");
            vacom = false;
        }

        // DCOM
        var vdcom = true;
        if ($.trim($("#ModSolDCOM").val()).length == 0) {
            errores.push("Ingrese el campo <strong>Porcentaje D</strong>. Dato Obligatorio.");
            vdcom = false;
        }

        // Seleccionar Cotización
        var idSolicitudElegida = 0;

        if ($("#TabCotizaciones input[type=radio]:checked").length > 0) {
            idSolicitudElegida = $("#TabCotizaciones input[type=radio]:checked").val();
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
            } else {
                if ($("#ModSolACOM").val() > 0 && $("#ModSolIndSeleccionado").val() == "S") {
                    if (idSolicitudElegida == 0) {
                        errores.push("Asegúrese de haber seleccionado una modalidad para que el sistema pueda calcular el monto.");
                        vmontoacom = false;
                    }
                }
            }
        }

        // Clases de controles
        if (!vacom) $("#ModSolACOM").addClass("formTextboxError");
        if (!vdcom) $("#ModSolDCOM").addClass("formTextboxError");
        if (!vmontoacom) $("#ModSolValMontoAcomAgente").addClass("formTextboxError");

        esCorrecto = vacom & vdcom & vmontoacom;//<INIGTI_4081>//

        if (!esCorrecto) {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html(formatearError(errores));
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
            return false;
        }
        //<FINGTI_7012>


        botonModSolAceptarBloqueado = true;
        $('#ModSolCalcularPBS').attr('class', 'botonDeshabilitado gris gris_sharp');


        $('#MCIcono').attr('class', 'cargando');
        //$('#MCContenedor').html('Cotizando la solicitud, por favor espere un momento...');
        $('#MCContenedor').html('Guardando temporalmente, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Calculando' });
        $('#ModalCotizando').dialog('open');

        //Registrando los parametros especiales, para quien tenga acceso

        try {

            if ($('#HGrupoEspeciales').val() == "TRUE") {
                for (var n = 0; n < ParametroEspecial.length; n++) {
                    ParametroEspecial[n].FecIniRangoStr = new Date(+ParametroEspecialNew[n].FecIniRango.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy");
                    ParametroEspecial[n].FecFinRangoStr = new Date(+ParametroEspecialNew[n].FecFinRango.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy");
                    ParametroEspecial[n].FecIniRango = null;
                    ParametroEspecial[n].FecFinRango = null;
                };
            } else {
                ParametroEspecial = null;
                modifica = false;
            }
        }
        catch (err) {
            //document.getElementById("demo").innerHTML = err.message;
            $("#ModalCotizando").dialog("close");

            $("#MCMIcono").attr("class", "error");
            $("#MCMContenedor").html("Ha ocurrido un error al calcular el PBS de la solicitud.");
            $("#ModalCuadroMensaje").dialog({ title: "Error" });
            $("#ModalCuadroMensaje").dialog("open");

            botonModSolAceptarBloqueado = false;
            $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
            return;
        }

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            parametros: ParametroEspecial,
            modifica: modificaEspecial
        }


        $.ajax({
            type: "POST",
            url: "BandejaFlujoCotizacionModificar.aspx/RegistrarCotizaValPar",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    //Calculando los PBS

                    const fechaCotizacion = new Date(Solicitud.FechaCotizacion).toString("dd/MM/yyyy");
                    var params = {
                        idSolicitud: $("#HNumSolicitud").val(),
                        fechaCotizacion: fechaCotizacion,
                        //tokenUsuario: $("#TokenUsuario").val(),
                        cotizaciones: Solicitud.Cotizaciones,
                        acom: $("#ModSolACOM").val(),
                        dcom: $("#ModSolDCOM").val(),
                        actualizaMovimiento: true,
                        tipoMovimiento: Solicitud.TipoMovimiento.Id,
                        montoAcom: $("#ModSolValMontoAcomAgente").val()
                    }


                    fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/calcular-pbs', {
                        method: "POST",
                        headers: {
                            "Content-Type": "application/json; charset=iso-8859-1",
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
                            if (data.data.Respuesta.Estado == "ERROR") {
                                $('#ModalCotizando').dialog('close');
                                // Mostrar mensaje de error
                                $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                                $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                                $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                                $('#ModalCuadroMensaje').dialog('open');
                            }
                            else {
                                Solicitud.Cotizaciones = data.data.Cotizaciones;
                                CargarTablaCotizacionesOficialesBandejaMovil(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);
                            }

                            botonModSolAceptarBloqueado = false;
                            $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                            $('#ModalCotizando').dialog('close');
                        })
                        .catch(error => {
                            if (error.message === "Sesión expirada") {
                                document.location.reload(true);
                            } else {
                                $("#ModalCotizando").dialog("close");
                                $("#MCMIcono").attr("class", "error");
                                $("#MCMContenedor").html("Ha ocurrido un error al calcular el PBS de la solicitud.");
                                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                                $("#ModalCuadroMensaje").dialog("open");
                            }
                            botonModSolAceptarBloqueado = false;
                            $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                            $('#ModalCotizando').dialog('close');
                        })
                        .finally(() => {
                            botonModSolAceptarBloqueado = false;
                            $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                            $('#ModalCotizando').dialog('close');
                        });
                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
                else if (data.d.Estado == "ERROR") {
                    // Mostrar mensaje de error
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
                    $("#ModalCotizando").dialog("close");

                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al calcular el PBS de la solicitud.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
                botonModSolAceptarBloqueado = false;
                $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
                $('#ModalCotizando').dialog('close');
            },
            complete: function () {

            }
        });




    });
    //<FINGTI_4081_2>

    $("#MCAOAceptar").live("click", function () {
        $("#ModalCuadroAdvertencia").dialog("close");

        //$("#MCAEstado").val('VALIDAACOMTRA');
        //let variable_prueba = $("#MCAEstado").val();
        //Boton Aceptar, despues que valido la seleccion, y desea continuar
        if ($("#MCAEstado").val() == "VALIDASELECCION_INS") {
            // Seleccionar Cotización
            $("#MCIcono").attr("class", "cargando");
            $("#MCContenedor").html("Cotizando la solicitud, por favor espere un momento...");
            $("#ModalCotizando").dialog({ title: "Cotizando" });
            $("#ModalCotizando").dialog("open");

            var idSolicitudElegida = 0;

            if ($("#TabCotizaciones input[type=radio]:checked").length > 0) {
                idSolicitudElegida = $("#TabCotizaciones input[type=radio]:checked").val();
            }

            var params = {
                //tokenUsuario: $("#TokenUsuario").val(),
                idSolicitud: $("#HNumSolicitud").val(),
                fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
                acom: $("#ModSolACOM").val(),
                dcom: $("#ModSolDCOM").val(),
                indEstadoSeleccion: $("#ModSolIndSeleccionado").val(),
                valMtoAgenteAcom: $("#ModSolValMontoAcomAgente").val(),
                numCotizacionElegida: (idSolicitudElegida == null) ? null : idSolicitudElegida,
                correo: Solicitud.Afiliado.CorreoElectronico,
                cod_compania: $("#ModSolCompania").val()//<INIGTI_4081>
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
                    .then(response => response.json())
                    .then(data => {
                        if (data.data.Respuesta.Estado == 'OK') {
                            // Imprimir número de solicitud generada
                            $('#ModSolNroSolicitud').html(data.data.NumSolicitud);

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

                            CargarTablaSolicitudesOficiales($("#HJefe").val(), $("#HSupervisor").val(), $("#HAgente").val(), false, data.data.NumSolicitud);

                            // Cargar la cotización actualizada
                            $('#BeneficiariosOriginales').show();
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

                                    $('#ModSolNroSolicitud').html(SolicitudEscenario.NumSolicitud);
                                    $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
                                    $('#ModSolACOM').val(formatearMonto2(SolicitudEscenario.PjeAumentoComision));
                                    $('#ModSolDCOM').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
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

                                    $('#ModSolCalcularPBS').show();

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

                                            if (Solicitud.Agente.IdNivel == 0) {
                                                $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Sin Nivel)');
                                            } else {
                                                $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Nivel:' + Solicitud.Agente.IdNivel + ')');
                                            }

                                            CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);
                                            CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

                                            // Actualizar temporizador
                                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                                            $clock.countdown(selectedDate.toString());
                                        },
                                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                                            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                                document.location.reload(true);
                                            } else {
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
                                    } else {
                                        $('#MCMIcono').attr('class', 'error');
                                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                        $('#ModalCuadroMensaje').dialog('open');
                                    }
                                }
                            });
                        }
                        else if (data.data.Estado == 'TOKEN') {
                            CerrarSesionExpirada();
                        }
                        else {
                            $('#ModalCotizando').dialog('close');

                            $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                            $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                            $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                            if (data.data.Respuesta.Controles != null && data.data.Respuesta.Controles > 0) {
                                if (data.data.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.data.Respuesta.Controles[0]);
                                if (data.data.Respuesta.Controles[1].length) $('#ModSolDCOM').attr('class', data.data.Respuesta.Controles[1]);
                                if (data.data.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.data.Respuesta.Controles[2]);
                                if (data.data.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.data.Respuesta.Controles[3]);

                                $('#TabCotizaciones select').removeClass('formTextboxGridError');
                                if (data.data.Respuesta.Controles.length > 13) {
                                    var celdaError;
                                    for (i = 13; i < data.data.Respuesta.Controles.length; i++) {
                                        celdaError = data.data.Respuesta.Controles[i].split(',');
                                        $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                    }
                                }
                            }
                            $('#ModalCuadroMensaje').dialog('open');
                        }

                        $('#ModSolCargando').fadeOut();
                    })
                    .catch(error => {
                        if (error.status === 401 || error.status === 12030) {
                            document.location.reload(true);
                        } else {
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');

                            $('#ModalCotizando').dialog('close');
                        }
                    })
                    .finally(() => {
                        botonModSolAceptarBloqueado = false;
                        $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
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
            //            $('#ModSolNroSolicitud').html(data.d.NumSolicitud);

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

            //            CargarTablaSolicitudesOficiales($("#HJefe").val(), $("#HSupervisor").val(), $("#HAgente").val(), false, data.d.NumSolicitud);

            //            // Cargar la cotización actualizada
            //            $('#BeneficiariosOriginales').show();
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

            //                    $('#ModSolNroSolicitud').html(SolicitudEscenario.NumSolicitud);
            //                    $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
            //                    $('#ModSolACOM').val(formatearMonto2(SolicitudEscenario.PjeAumentoComision));
            //                    $('#ModSolDCOM').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
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

            //                    //<INIGTI_4081>
            //                    $('#ModSolCalcularPBS').show();
            //                    //<FINGTI_4081>

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

            //                            //<INIGTI_4022>
            //                            //if (Solicitud.Agente.IdNivel == 0) {
            //                            //    $("#LabModSolNivel").attr('title', '');
            //                            //} else {
            //                            //    $("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
            //                            //}
            //                            //<FINGTI_4022>
            //                            //<INIGTI_4022>
            //                            if (Solicitud.Agente.IdNivel == 0) {
            //                                $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Sin Nivel)');
            //                            } else {
            //                                $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Nivel:' + Solicitud.Agente.IdNivel + ')');
            //                            }
            //                            //<FINGTI_4022>

            //                            CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);
            //                            CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

            //                            //$('#ModSolAceptarOficial').attr('class', 'botonDeshabilitado gris gris_sharp');

            //                            // Actualizar temporizador
            //                            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
            //                            $clock.countdown(selectedDate.toString());

            //                            //<SOLINI25781>//true=muestra la caja de texto TRA.
            //                            //<INIGTI_4081>//Se comenta
            //                            ////if ($("#HOcultraColumnaTRA").val() == "TRUE") {

            //                            ////    document.getElementById('ControlTRA').style.display = 'block';
            //                            ////    $("#ModSolTRA").val('0.00');
            //                            ////} else {
            //                            ////    document.getElementById('ControlTRA').style.display = 'none';
            //                            ////}
            //                            //<FINGTI_4081>
            //                            //<SOLFIN25781>

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
            //            if (data.d.Respuesta.Controles != null && data.d.Respuesta.Controles > 0) {

            //                if (data.d.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.d.Respuesta.Controles[0]);
            //                if (data.d.Respuesta.Controles[1].length) $('#ModSolDCOM').attr('class', data.d.Respuesta.Controles[1]);
            //                if (data.d.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.d.Respuesta.Controles[2]);
            //                if (data.d.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.d.Respuesta.Controles[3]);

            //                $('#TabCotizaciones select').removeClass('formTextboxGridError');
            //                if (data.d.Respuesta.Controles.length > 13) {
            //                    var celdaError;
            //                    for (i = 13; i < data.d.Respuesta.Controles.length; i++) {
            //                        celdaError = data.d.Respuesta.Controles[i].split(',');
            //                        $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
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
            //        $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
            //    }
            //});

        }
        else if ($("#MCAEstado").val() == "VALIDASELECCION_MOD") {
            // Seleccionar Cotización
            var idSolicitudElegida = 0;

            if ($("#TabCotizaciones input[type=radio]:checked").length > 0) {
                idSolicitudElegida = $("#TabCotizaciones input[type=radio]:checked").val();
            }

            $("#MCIcono").attr("class", "cargando");
            $("#MCContenedor").html("Guardando datos de la cotización, por favor espere un momento...");
            $("#ModalCotizando").dialog({ title: "Cotizando" });
            $("#ModalCotizando").dialog("open");

            // Guardar los datos de la solicitud
            var params = {
                //tokenUsuario: $("#TokenUsuario").val(),
                idSolicitud: $("#HNumSolicitud").val(),
                fechaCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"),
                acom: $("#ModSolACOM").val(),
                dcom: $("#ModSolDCOM").val(),
                indEstadoSeleccion: $("#ModSolIndSeleccionado").val(),
                valMtoAgenteAcom: $("#ModSolValMontoAcomAgente").val(),
                numCotizacionElegida: (idSolicitudElegida == null) ? null : idSolicitudElegida,
                cotizaciones: Solicitud.Cotizaciones,
                correo: Solicitud.Afiliado.CorreoElectronico,
                cod_compania: $("#ModSolCompania").val()
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

                            CargarTablaSolicitudesOficiales($("#HJefe").val(), $("#HSupervisor").val(), $("#HAgente").val(), false, data.data.NumSolicitud);

                            var paramsInner = {
                                idSolicitud: $("#HNumSolicitud").val(),
                                fecCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy")
                            }

                            // Segunda llamada fetch anidada
                            return fetch('CotizadorOficiales.aspx/ObtenerDatosSolicitud', {
                                method: 'POST',
                                headers: {
                                    "Content-Type": "application/json; charset=iso-8859-1"
                                },
                                body: JSON.stringify(paramsInner)
                            });
                        } else if (data.data.Estado == 'TOKEN') {
                            CerrarSesionExpirada();
                            throw new Error("Sesión token expirada");
                        } else {
                            $('#ModalCotizando').dialog('close');

                            $('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
                            $('#MCMIcono').attr('class', data.data.Respuesta.Icono);
                            $('#MCMContenedor').html(data.data.Respuesta.Mensaje);
                            if (data.data.Respuesta.Controles != null && data.data.Respuesta.Controles > 0) {
                                if (data.data.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.data.Respuesta.Controles[0]);
                                if (data.data.Respuesta.Controles[1].length) $('#ModSolDCOM').attr('class', data.data.Respuesta.Controles[1]);
                                if (data.data.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.data.Respuesta.Controles[2]);
                                if (data.data.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.data.Respuesta.Controles[3]);

                                if (data.data.Respuesta.Controles.length > 13) {
                                    var celdaError;
                                    for (i = 13; i < data.data.Respuesta.Controles.length; i++) {
                                        celdaError = data.data.Respuesta.Controles[i].split(',');
                                        $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                                    }
                                }
                            }
                            $('#ModalCuadroMensaje').dialog('open');
                            $('#ModSolCargando').fadeOut();
                            $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
                            throw new Error("Error en la respuesta del servidor");
                        }
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
                        Solicitud = data.d;
                        SolicitudEscenario.NumCotizacionElegida = idSolicitudElegida;
                        CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

                        if ($('#HRedLocal').val() == "FALSE") {
                            $('#LabModSolLineaACOMDCOM').hide();
                            $('#LabModSolLineaSelecMon').hide();
                            $('#ControlTRA').hide();
                        }
                        else {
                            botonModSolAceptarBloqueado = false;
                            $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');
                        }

                        if (Solicitud.TipoMovimiento.Id == 0) {
                            $('#ModSolACOM').attr('class', 'formTextbox');
                            $('#ModSolDCOM').attr('class', 'formTextbox');
                            $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox');
                            $('#ConModSolIndSeleccionado').removeClass('formComboboxReadOnlyContenedor');
                            $('#ConModSolCompania').removeClass('formComboboxReadOnlyContenedor');

                            $('#ModSolACOM').attr('readonly', false);
                            $('#ModSolDCOM').attr('readonly', false);
                            $('#ModSolValMontoAcomAgente').attr('readonly', false);

                            $('#ModSolIndSeleccionado').removeAttr('disabled');
                            $('#ModSolCompania').removeAttr('disabled');
                        } else {
                            $('#ModSolACOM').attr('class', 'formTextbox formTextboxReadOnly');
                            $('#ModSolDCOM').attr('class', 'formTextbox formTextboxReadOnly');
                            $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox formTextboxReadOnly');
                            $('#ConModSolIndSeleccionado').addClass('formComboboxReadOnlyContenedor');
                            $('#ConModSolCompania').addClass('formComboboxReadOnlyContenedor');

                            $('#ModSolACOM').attr('readonly', true);
                            $('#ModSolDCOM').attr('readonly', true);
                            $('#ModSolValMontoAcomAgente').attr('readonly', true);

                            $('#ModSolIndSeleccionado').attr('disabled', 'disabled');
                            $('#ModSolCompania').attr('disabled', 'disabled');
                        }

                        // Actualizar temporizador
                        selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                        $clock.countdown(selectedDate.toString());
                    })
                    .catch(error => {
                        if (error.message === "Sesión expirada" || error.message === "Sesión token expirada") {
                            document.location.reload(true);
                        } else {
                            $("#ModalCotizando").dialog("close");
                            $("#MCMIcono").attr("class", "error");
                            $("#MCMContenedor").html("Ha ocurrido un error al guardar los datos de la solicitud.");
                            $("#ModalCuadroMensaje").dialog({ title: "Error" });
                            $("#ModalCuadroMensaje").dialog("open");
                        }
                        $('#ModalSolicitud').dialog('close');
                    });
            } else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html("Usted no tiene privilegios sobre esta opción.");
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
                $('#ModalCotizando').dialog('close');
            }



            //$.ajax({
            //    type: "POST",
            //    url: "CotizadorOficiales.aspx/ModificarSolicitud",
            //    contentType: "application/json; charset=iso-8859-1",
            //    dataType: "json",
            //    data: $.toJSON(params),
            //    success: function (data) {
            //        if (data.d.Respuesta.Estado == "OK") {
            //            // Cerrar modal de espera
            //            $('#ModalCotizando').dialog('close');

            //            // Mostrar mensaje de éxito
            //            $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
            //            $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
            //            $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
            //            $('#ModalCuadroMensaje').dialog('open');

            //            CargarTablaSolicitudesOficiales($("#HJefe").val(), $("#HSupervisor").val(), $("#HAgente").val(), false, data.d.NumSolicitud);

            //            var params = {
            //                idSolicitud: $("#HNumSolicitud").val(),
            //                fecCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy")
            //            }
            //            $.ajax({
            //                type: 'POST',
            //                url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitud',
            //                contentType: "application/json; charset=iso-8859-1",
            //                data: $.toJSON(params),
            //                dataType: 'json',
            //                success: function (data) {
            //                    Solicitud = data.d;

            //                    SolicitudEscenario.NumCotizacionElegida = idSolicitudElegida;
            //                    CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

            //                    if ($('#HRedLocal').val() == "FALSE") {
            //                        $('#LabModSolLineaACOMDCOM').hide();
            //                        $('#LabModSolLineaSelecMon').hide();
            //                        $('#ControlTRA').hide();
            //                    }
            //                    else {
            //                        botonModSolAceptarBloqueado = false;
            //                        $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');
            //                    }

            //                    if (Solicitud.TipoMovimiento.Id == 0) {

            //                        $('#ModSolACOM').attr('class', 'formTextbox');
            //                        $('#ModSolDCOM').attr('class', 'formTextbox');
            //                        $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox');
            //                        $('#ConModSolIndSeleccionado').removeClass('formComboboxReadOnlyContenedor');
            //                        $('#ConModSolCompania').removeClass('formComboboxReadOnlyContenedor');


            //                        $('#ModSolACOM').attr('readonly', false);
            //                        $('#ModSolDCOM').attr('readonly', false);
            //                        $('#ModSolValMontoAcomAgente').attr('readonly', false);

            //                        $('#ModSolIndSeleccionado').removeAttr('disabled');
            //                        $('#ModSolCompania').removeAttr('disabled');

            //                    } else {
            //                        $('#ModSolACOM').attr('class', 'formTextbox formTextboxReadOnly');
            //                        $('#ModSolDCOM').attr('class', 'formTextbox formTextboxReadOnly');
            //                        $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox formTextboxReadOnly');
            //                        $('#ConModSolIndSeleccionado').addClass('formComboboxReadOnlyContenedor');
            //                        $('#ConModSolCompania').addClass('formComboboxReadOnlyContenedor');

            //                        $('#ModSolACOM').attr('readonly', true);
            //                        $('#ModSolDCOM').attr('readonly', true);
            //                        $('#ModSolValMontoAcomAgente').attr('readonly', true);

            //                        $('#ModSolIndSeleccionado').attr('disabled', 'disabled');
            //                        $('#ModSolCompania').attr('disabled', 'disabled');

            //                    }

            //                    // Actualizar temporizador
            //                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
            //                    $clock.countdown(selectedDate.toString());
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
            //                    $('#ModalSolicitud').dialog('close');
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
            //            if (data.d.Respuesta.Controles != null && data.d.Respuesta.Controles > 0) {

            //                if (data.d.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.d.Respuesta.Controles[0]);
            //                if (data.d.Respuesta.Controles[1].length) $('#ModSolDCOM').attr('class', data.d.Respuesta.Controles[1]);
            //                if (data.d.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.d.Respuesta.Controles[2]);
            //                if (data.d.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.d.Respuesta.Controles[3]);

            //                if (data.d.Respuesta.Controles.length > 13) {
            //                    var celdaError;
            //                    for (i = 13; i < data.d.Respuesta.Controles.length; i++) {
            //                        celdaError = data.d.Respuesta.Controles[i].split(',');
            //                        $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
            //                    }
            //                }
            //            }
            //            $('#ModalCuadroMensaje').dialog('open');
            //        }

            //        $('#ModSolCargando').fadeOut();
            //        //ModSolBotonesInactivos = false;
            //        $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
            //    },
            //    error: function (XMLHttpRequest, textStatus, errorThrown) {
            //        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
            //            document.location.reload(true);
            //        }
            //        else {
            //            $("#ModalCotizando").dialog("close");

            //            $("#MCMIcono").attr("class", "error");
            //            $("#MCMContenedor").html("Ha ocurrido un error al guardar los datos de la solicitud.");
            //            $("#ModalCuadroMensaje").dialog({ title: "Error" });
            //            $("#ModalCuadroMensaje").dialog("open");
            //        }
            //    }
            //});


        }
        else if ($("#MCAEstado").val() == "VALIDAACOMTRA") {//Boton aceptar, despues que valido acom, tra, continua al flujo

            // Seleccionar Cotización
            var idSolicitudElegida = 0;

            if ($("#TabCotizaciones input[type=radio]:checked").length > 0) {
                idSolicitudElegida = $("#TabCotizaciones input[type=radio]:checked").val();
            }

            var esCorrecto = true;
            var errores = new Array();


            var vseleccionado = true;
            if ($("#ModSolIndSeleccionado").val() == "N") {
                errores.push("El campo <strong>Ind. Seleccionado</strong> debe estar en <strong>Si</strong> para continuar con el flujo.");
                vseleccionado = false;
            }

            var velegida = true;
            if (idSolicitudElegida == 0) {
                errores.push("Debe seleccionar una <strong>Cotización</strong> para continuar con el flujo.");
                velegida = false;
            }

            esCorrecto = vseleccionado & velegida;
            if (!esCorrecto) {
                if (!vseleccionado) $("#ConModSolIndSeleccionado").addClass("formComboboxErrorContenedor");

                $('#MCMIcono').attr('class', 'validacion');
                $('#MCMContenedor').html(formatearError(errores));
                $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
                $('#ModalCuadroMensaje').dialog('open');
                return false;
            }


            $("#MCIcono").attr("class", "cargando");
            $("#MCContenedor").html("Guardando datos de la cotización, por favor espere un momento...");
            $("#ModalCotizando").dialog({ title: "Cotizando" });
            $("#ModalCotizando").dialog("open");

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
                cod_compania: $("#ModSolCompania").val(),
                dcom: $("#ModSolDCOM").val(),
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
                        CargarTablaSolicitudesOficiales($("#HJefe").val(), $("#HSupervisor").val(), $("#HAgente").val(), false, $("#ModSolNroSolicitud").html());

                        var params = {
                            idSolicitud: $("#HNumSolicitud").val(),
                            fecCotizacion: new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy")
                        }
                        $.ajax({
                            type: 'POST',
                            url: 'CotizadorOficiales.aspx/ObtenerDatosSolicitud',
                            contentType: "application/json; charset=iso-8859-1",
                            data: $.toJSON(params),
                            dataType: 'json',
                            success: function (data) {
                                Solicitud = data.d;
                                SolicitudEscenario.NumCotizacionElegida = idSolicitudElegida;
                                //CargarTablaBeneficiariosOficial(Solicitud.Beneficiarios);
                                CargarTablaCotizacionesOficiales(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);
                                if ($('#HRedLocal').val() == "FALSE") {
                                    $('#LabModSolLineaACOMDCOM').hide();
                                    $('#LabModSolLineaSelecMon').hide();
                                }
                                if (Solicitud.TipoMovimiento.Id == 0) {
                                    $('#ModSolACOM').attr('class', 'formTextbox');
                                    $('#ModSolDCOM').attr('class', 'formTextbox');
                                    $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox');
                                    $('#ConModSolIndSeleccionado').removeClass('formComboboxReadOnlyContenedor');
                                    $('#ConModSolCompania').removeClass('formComboboxReadOnlyContenedor');
                                    $('#ModSolACOM').attr('readonly', false);
                                    $('#ModSolDCOM').attr('readonly', false);
                                    $('#ModSolValMontoAcomAgente').attr('readonly', false);
                                    $('#ModSolIndSeleccionado').removeAttr('disabled');
                                    $('#ModSolCompania').removeAttr('disabled');
                                } else {
                                    $('#ModSolACOM').attr('class', 'formTextbox formTextboxReadOnly');
                                    $('#ModSolDCOM').attr('class', 'formTextbox formTextboxReadOnly');
                                    $('#ModSolValMontoAcomAgente').attr('class', 'formTextbox formTextboxReadOnly');
                                    $('#ConModSolIndSeleccionado').addClass('formComboboxReadOnlyContenedor');
                                    $('#ConModSolCompania').addClass('formComboboxReadOnlyContenedor');
                                    $('#ModSolACOM').attr('readonly', true);
                                    $('#ModSolDCOM').attr('readonly', true);
                                    $('#ModSolValMontoAcomAgente').attr('readonly', true);
                                    $('#ModSolIndSeleccionado').attr('disabled', 'disabled');
                                    $('#ModSolCompania').attr('disabled', 'disabled');
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
                                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                    $('#ModalCuadroMensaje').dialog('open');
                                }
                                $('#ModalSolicitud').dialog('close');
                            }
                        });
                        botonModSolAceptarBloqueado = true;
                        $('#ModSolAceptarOficial').attr('class', 'botonDeshabilitado gris gris_sharp');
                        $('#ModSolCalcularPBS').hide();
                    }
                    else if (data.data.Estado == "TOKEN") {
                        CerrarSesionExpirada();
                    }
                    else {
                        $('#ModalCotizando').dialog('close');
                        $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
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
        };

        $("#MCAEstado").val("");

    });

    $("#MCAOCancelar").live("click", function () {
        $("#ModalCuadroAdvertencia").dialog("close");
    });

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
        /*         var params = {
                    numCusspp: numCusspp,
                    numSolicitud: numSolicitud,
                    numOperacion: numOperacion,
                }
                localStorage.setItem('solicitudEscenarioMovil', JSON.stringify({
                    NumOperacion: Number(numOperacion),
                    NumSolicitud: numSolicitud,
                    Afiliado: { CUSPP: numCusspp }
                })); */
        window.location.href = 'BandejaFlujoCotizacionModificar.aspx?numOperacion=' + numOperacion + '&numSolicitud=' + numSolicitud + '&numCusspp=' + numCusspp;
        /*  $.ajax({
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
         }); */
        //fin movil
        //}

    });

    /* Botón Ver Flujo*/
    $('#TabBandejaSolicitudOficial .grilla_flujo').live('click', function () {
        var numCusspp = $(this).data('cusspp');
        var numSolicitud = $(this).data('solicitud');
        var numOperacion = $(this).data('operacion');

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
                window.location.href = 'BandejaFlujoHistorial.aspx';
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

    /* Botón Rechazar Flujo*/
    $('#ModSolRechazarBandeja').live('click', function () {
        if (permisoRechazarSolicitud) {
            if (!botonModSolAceptarBloqueado) {

                if ($('#HCodTipoMovimiento').val() != 7 && $('#HCodTipoMovimiento').val() != 0) {
                    //if(!movil){
                    //RegistrarTipoMovimientoBandeja(true);    
                    //}
                    //else {
                    RegistrarTipoMovimientoBandejaMovil(true);
                    //}
                }
            }
        }
    });

    /* Botón Aprobar Flujo*/
    $('#ModSolAprobarBandeja').live('click', function () {
        if (!botonModSolAceptarBloqueado) {
            if ($('#HCodTipoMovimiento').val() != 7 && $('#HCodTipoMovimiento').val() != 0) {
                //if (!movil) {
                //    RegistrarTipoMovimientoBandeja(false);
                //}
                //else {
                RegistrarTipoMovimientoBandejaMovil(false);
                //}
            }
        }
    });
    //<SRI.FIN-20322_E2>

    //<INIGTI_4081>
    /*Botón Aprobar Flujo Bloque-  */
    $('#ModSolAprobarBandejaBloque').live('click', function () {
        RegistrarTipoMovimientoBandejaBloque(false);
    });

    $('#ModSolRechazarBandejaBloque').live('click', function () {
        if (permisoRechazarSolicitud) {
            if (!botonModSolAceptarBloqueado) {
                RegistrarTipoMovimientoBandejaBloque(true);
            }
        }
    });

    /* Calcular monto de ACOM - Inicio */
    $("#ModSolACOM,#ModSolIndSeleccionado,#TabCotizaciones input[type=radio]").live("change", function () {
        if (!botonModSolAceptarBloqueado) {

            // Validar sólo si existe el textbox de Monto ACOM (oficiales)
            if ($("#ModSolValMontoAcomAgente").length > 0) {

                if ($("#ModSolACOM").val() > 0 && $("#ModSolIndSeleccionado").val() == "S" && $("#TabCotizaciones input[type=radio]:checked").length > 0) {
                    // Bloquear botón Aceptar
                    $("#ModSolMontoACOMCargando").show();
                    botonModSolAceptarBloqueado = true;
                    $("#ModSolAceptarOficial").attr("class", "botonDeshabilitado gris gris_sharp");

                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        solicitud: Solicitud.Id,
                        acom: $("#ModSolACOM").val(),
                        cotizacion: $("#TabCotizaciones input[type=radio]:checked").val()
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

    $("#TabCotizaciones #TabCotTRA").live("focus", function () {
        if (!botonModSolAceptarBloqueado) {
            if ($("#TabCotizaciones input[type=radio]").is(':disabled')) return;
            // Validar sólo si existe el textbox de Monto ACOM (oficiales)
            if ($("#ModSolValMontoAcomAgente").length > 0) {

                if ($("#ModSolACOM").val() > 0 && $("#ModSolIndSeleccionado").val() == "S" && $("#TabCotizaciones input[type=radio]:checked").length > 0) {
                    // Bloquear botón Aceptar
                    $("#ModSolMontoACOMCargando").show();
                    botonModSolAceptarBloqueado = true;
                    $("#ModSolAceptarOficial").attr("class", "botonDeshabilitado gris gris_sharp");

                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        solicitud: Solicitud.Id,
                        acom: $("#ModSolACOM").val(),
                        cotizacion: $("#TabCotizaciones input[type=radio]:checked").val()
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
                        // Establecer datos en sesión si el API los devuelve
                        var anticipo = data.data.Respuesta.Anticipo
                        anticipo.mensaje="C"

                        if (data.data.Respuesta.Anticipo) {
                            sessionStorage.setItem('Anticipo', JSON.stringify(data.data.Respuesta.Anticipo));
                        }
                        window.location.href = "../Cotizador/CondicionesAnticipo.aspx";
                    }
                    else if (data.data.Respuesta.Mensaje == "R") {

                        // Usar la función reutilizable definida en CWRV.generarPdf.js
                        if (typeof window.generarPdfAdelanto === 'function') {
                            window.generarPdfAdelanto(idSolicitud);
                        } else {
                            console.error('La función generarPdfAdelanto no está disponible. Asegúrese de que CWRV.generarPdf.js esté cargado.');
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Error: No se pudo cargar la función para generar el PDF. Verifique que el archivo CWRV.generarPdf.js esté incluido.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }

                        //var paramsReporte = {
                        //    tokenUsuario: $("#TokenUsuario").val(),
                        //    solicitud: idSolicitud,
                        //    agente: numAgente
                        //}

                        //$.ajax({
                        //    type: "POST",
                        //    url: rutaGenerarReporteSolicitudAnticipo,
                        //    contentType: "application/json; charset=iso-8859-1",
                        //    dataType: "json",
                        //    data: $.toJSON(paramsReporte),
                        //    success: function (data) {
                        //        if (data.d.Estado == "OK") {
                        //            if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                        //                window.location.href = "../Reportes/SolicitudAnticipoMovil.aspx";
                        //            }
                        //            else {
                        //                var w = 800;
                        //                var h = 600;
                        //                var left = (screen.width / 2) - (w / 2);
                        //                var top = (screen.height / 2) - (h / 2);
                        //                var nuevaVentana = window.open("../Reportes/SolicitudAnticipo.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                        //            }

                        //            /*<SRIINI17003>*/
                        //            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                        //            $clock.countdown(selectedDate.toString());
                        //            /*<SRIFIN17003>*/
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
                        //            $('#MCMContenedor').html('Ha ocurrido un error al exportar la la solicitud.');
                        //            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        //            $('#ModalCuadroMensaje').dialog('open');
                        //        }
                        //    }
                        //});
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

    /* Botón toogle de la tabla de Solicitudes Oficiales */
    $(".toogleOficiales").live("click", function () {
        historialOficiales.push($(this).attr("id").replace("img", ""));
    })

    /*Begin Imprimir Meler */
    $('#TabCotizacionCliente .grilla_pdf2').live('click', function () {
        var numOperacion = $(this).data('operacion');

        // Mostrar modal de cargando
        $("#ModalGenerandoReporte").dialog("open");

        // LLamando al método para exportar el reporte
        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            numeroOperacion: numOperacion
        }
        $.ajax({
            type: "POST",
            url: "CotizadorOficiales.aspx/ExportarReporte",
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
                    $('#MCMContenedor').html('Ha ocurrido un error al exportar la solicitud.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
            },
            complete: function () {
                $("#ModalGenerandoReporte").dialog("close");
            }
        });


    });
    /*End Imprimir Meler*/

    $("#Imprimir").live("click", function () {
        ImprimirFicha($("#TokenUsuario").val(), $("#HCUSPP").val());
    })

    $('#GrabarTRA').live('click', function () {
        if (!permisoGrabarTra) {
            return;
        }
        var correcto = true;
        var errores = new Array();

        $(".TabNroCasosTotal").each(function (index, val) {
            var total = $(this).val();

            if (total.length == 0) {
                $(this).addClass('formTextboxGridError');
                correcto = false;
                errores.push("Debe ingresar una <b>Cantidad</b> correcta<br />");
            }
        });


        $(".fecha").each(function (index, val) {
            var total = $(this).val();

            if (total.length == 0) {
                $(this).addClass('formTextboxGridError');
                correcto = false;
                errores.push("Debe ingresar una <b>Fecha</b> correcta<br />");
            }
        });


        if (!correcto) {
            $('#MCMIcono').attr('class', 'error');
            $('#MCMContenedor').html(formatearError(errores));
            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
            $('#ModalCuadroMensaje').dialog('open');
            return false;
        }

        var now = new Date();

        for (var n = 0; n < listaCuotasTra.length; n++) {
            listaCuotasTra[n].Agente.FechaIngreso = now.toDateString();
            listaCuotasTra[n].FecInicioVigencia = null;
            listaCuotasTra[n].FecFinVigencia = null;
            listaCuotasTra[n].FecCotizacion = null;
        };


        $('#MCIcono').attr('class', 'cargando');
        $('#MCContenedor').html('Registrando las cuotas TRA, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Registrando' });
        $('#ModalCotizando').dialog('open');

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            lstCuotas: listaCuotasTra
        }

        $.ajax({
            type: "POST",
            url: "CuotaTra.aspx/RegistrarCuotas",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: JSONstringifyConFechas(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    var periodo = $('#Periodo').val();
                    var mes = $('#Mes').val();
                    CargarTablaCuotasTra(periodo, mes);

                    setTimeout("$('#ModalCotizando').dialog('close');", 2000);
                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
                }
                else if (data.d.Estado == "ERROR") {
                    $('#ModalCotizando').dialog('close');
                    // Mostrar mensaje de error
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');


                    if (data.d.Controles.length > 0) {
                        var celdaError;
                        for (i = 0; i < data.d.Controles.length; i++) {
                            celdaError = data.d.Controles[i].split(',');
                            $('#TabCuotasTra tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') input').addClass('formTextboxGridError');
                        }
                    }

                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {

                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $("#ModalCotizando").dialog("close");

                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al registrar las Cuotas TRA.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
                $('#ModalCotizando').dialog('close');
            },
            complete: function () {

            }
        });

    });


    $('#CancelarTRA').live('click', function () {
        window.location.href = "../Cotizador/Cotizador.aspx";
    });

    //Buscar Gestion Ventas
    $('#BuscarGestionVentas').live('click', function () {

        if (botoneraBloqueadoGestionVentas) {
            return;
        }
        botoneraBloqueadoGestionVentas = true;

        var esCorrecto = true;
        var errores = new Array();

        $('#FechaDesde').removeClass('formTextboxError formCalendarError');
        $('#FechaHasta').removeClass('formTextboxError formCalendarError');

        //Fecha desde
        var fechaDesde = true;
        if ($.trim($('#FechaDesde').val()).length == 0) {
            errores.push('Debe ingresar la <strong>Fecha Desde</strong>.');
            $('#FechaDesde').addClass('formTextboxError formCalendarError');
            fechaDesde = false;
        }

        //Fecha Hasta
        var fechaHasta = true;
        if ($.trim($('#FechaHasta').val()).length == 0) {
            errores.push('Debe ingresar la <strong>Fecha Hasta</strong>.');
            $('#FechaHasta').addClass('formTextboxError formCalendarError');
            fechaHasta = false;
        }

        esCorrecto = fechaDesde & fechaHasta;

        if (!esCorrecto) {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html(formatearError(errores));
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
            botoneraBloqueadoGestionVentas = false;
            return false;
        }

        $('#BuscarGestionVentas').attr('class', 'botonDeshabilitado gris gris_sharp');
        $('#EnviarExcelGestionVentas').attr('class', 'botonDeshabilitado gris gris_sharp');
        $('#EnviarCorreoGestionVentas').attr('class', 'botonDeshabilitado gris gris_sharp');

        CargarTablaGestionVentas();
    })

    //Generar Excel Gestion Ventas
    $('#EnviarExcelGestionVentas').live('click', function () {
        if (botoneraBloqueadoGestionVentas) {
            return;
        }
        botoneraBloqueadoGestionVentas = true;

        var esCorrecto = true;
        var errores = new Array();

        $('#FechaDesde').removeClass('formTextboxError formCalendarError');
        $('#FechaHasta').removeClass('formTextboxError formCalendarError');

        //Fecha desde
        var fechaDesde = true;
        if ($.trim($('#FechaDesde').val()).length == 0) {
            errores.push('Debe ingresar la <strong>Fecha Desde</strong>.');
            $('#FechaDesde').addClass('formTextboxError formCalendarError');
            fechaDesde = false;
        }

        //Fecha Hasta
        var fechaHasta = true;
        if ($.trim($('#FechaHasta').val()).length == 0) {
            errores.push('Debe ingresar la <strong>Fecha Hasta</strong>.');
            $('#FechaHasta').addClass('formTextboxError formCalendarError');
            fechaHasta = false;
        }

        esCorrecto = fechaDesde & fechaHasta;

        if (!esCorrecto) {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html(formatearError(errores));
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
            botoneraBloqueadoGestionVentas = false;
            return false;
        }


        $('#MCIcono').attr('class', 'cargando');
        $('#MCContenedor').html('Generando Excel de Gestión de Ventas, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Generando Excel' });
        $('#ModalCotizando').dialog('open');


        $('#BuscarGestionVentas').attr('class', 'botonDeshabilitado gris gris_sharp');
        $('#EnviarExcelGestionVentas').attr('class', 'botonDeshabilitado gris gris_sharp');
        $('#EnviarCorreoGestionVentas').attr('class', 'botonDeshabilitado gris gris_sharp');

        var supervisor = "0";
        if ($("#Supervisor").val() != undefined) {
            supervisor = $("#Supervisor").val();
        }

        var jefe = "0";
        if ($("#Jefe").val() != undefined) {
            jefe = $("#Jefe").val();
        }

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            fechaInicial: $("#FechaDesde").val(),
            fechaFinal: $("#FechaHasta").val(),
            numJefe: jefe,
            numSuperv: supervisor,//$("#Supervisor").val(),
            numAgente: $("#Agente").val(),
            indCierre: $("#CotizacionCerrada").val(),
            tipoCotizacion: $("#TipoCotizacion").val(),
            codCiaSeguro: $("#CiaSeguro").val(),
            correo: false,
            lstGVentas: null
        }

        $.ajax({
            type: "POST",
            url: "GestionVentas.aspx/GenerarExcelGestionVentas",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    location.replace(rutaGestionVentas);
                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
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
                    document.location.reload(true);
                }
                else {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al consultar la Gestión de Ventas.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
                $('#ModalCotizando').dialog('close');
                botoneraBloqueadoGestionVentas = false;
                $('#BuscarGestionVentas').attr('class', 'boton darkblue sharp');
                $('#EnviarExcelGestionVentas').attr('class', 'boton darkblue sharp');
                $('#EnviarCorreoGestionVentas').attr('class', 'boton darkblue sharp');
            }
        });

    })

    $('#EnviarCorreoGestionVentas').live('click', function () {
        if (botoneraBloqueadoGestionVentas) {
            return;
        }
        botoneraBloqueadoGestionVentas = true;

        var esCorrecto = true;
        var errores = new Array();

        $('#FechaDesde').removeClass('formTextboxError formCalendarError');
        $('#FechaHasta').removeClass('formTextboxError formCalendarError');

        //Fecha desde
        var fechaDesde = true;
        if ($.trim($('#FechaDesde').val()).length == 0) {
            errores.push('Debe ingresar la <strong>Fecha Desde</strong>.');
            $('#FechaDesde').addClass('formTextboxError formCalendarError');
            fechaDesde = false;
        }

        //Fecha Hasta
        var fechaHasta = true;
        if ($.trim($('#FechaHasta').val()).length == 0) {
            errores.push('Debe ingresar la <strong>Fecha Hasta</strong>.');
            $('#FechaHasta').addClass('formTextboxError formCalendarError');
            fechaHasta = false;
        }

        esCorrecto = fechaDesde & fechaHasta;

        if (!esCorrecto) {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html(formatearError(errores));
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
            botoneraBloqueadoGestionVentas = false;
            return false;
        }

        $('#MCIcono').attr('class', 'cargando');
        $('#MCContenedor').html('Enviando Excel de Gestión de Ventas, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Generando Excel' });
        $('#ModalCotizando').dialog('open');

        $('#BuscarGestionVentas').attr('class', 'botonDeshabilitado gris gris_sharp');
        $('#EnviarExcelGestionVentas').attr('class', 'botonDeshabilitado gris gris_sharp');
        $('#EnviarCorreoGestionVentas').attr('class', 'botonDeshabilitado gris gris_sharp');

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
            codCiaSeguro: $("#CiaSeguro").val(),
            correo: true,
            lstGVentas: null
        }

        $.ajax({
            type: "POST",
            url: "GestionVentas.aspx/EnviarExcelCorreoGestionVentas",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {

                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
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
                    document.location.reload(true);
                }
                else {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al consultar la Gestión de Ventas.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
                $('#ModalCotizando').dialog('close');
                botoneraBloqueadoGestionVentas = false;
                $('#BuscarGestionVentas').attr('class', 'boton darkblue sharp');
                $('#EnviarExcelGestionVentas').attr('class', 'boton darkblue sharp');
                $('#EnviarCorreoGestionVentas').attr('class', 'boton darkblue sharp');
            }
        });

    })

    $('#CotizacionCerrada').live('change', function () {
        if ($('#CotizacionCerrada').val() == "S") {
            $('#LabCerrada').html("Fecha de Cierre Comercial");
        } else {
            $('#LabCerrada').html("Fecha de Plazo AFP");
        }
    });

    $('#ModSolEnviarEmailPendiente').live("click", function () {

        $('#ModalCuadroAdvertencia').dialog({ title: 'Advertencia' });
        $('#MCAIcono').attr('class', 'advertencia');
        $('#MCAContenedor').html('¿Deseas Re-Enviar email de las solicitudes pendientes por aprobar?');
        $('#ModalCuadroAdvertencia').dialog('open');
    });

    $('#MCAOAceptarBandejaEmail').live("click", function () {

        $('#ModalCuadroAdvertencia').dialog('close');

        $('#MCIcono').attr('class', 'cargando');
        $('#MCContenedor').html('Re-Enviando Email, por favor espere un momento...');
        $('#ModalCotizando').dialog({ title: 'Re-Enviando Email' });
        $('#ModalCotizando').dialog('open');

        var params = {
            tokenUsuario: $("#TokenUsuario").val()
        }

        $.ajax({
            type: "POST",
            url: "BandejaFlujoCotizacion.aspx/EnviarEmailPendiente",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#MCMIcono").attr("class", data.d.Icono);
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                    $("#ModalCuadroMensaje").dialog("open");
                }
                else if (data.d.Estado == "TOKEN") {
                    CerrarSesionExpirada();
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
                    document.location.reload(true);
                }
                else {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al enviar el email.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
                $('#ModalCotizando').dialog('close');
            }
        });

    });

    $('#ChkSolicitudesReportePlus').live('change', function (e) {
        var solicitudes = new Array();
        var cotizaciones = new Array();
        var check = $(this);

        if ($(check).is(":checked")) {
            var Max = $(".TabCotizacionesReportePlus input.ChkCotizacionesReportePlus[type=checkbox]:checked").length;
            if (Max + 1 > parseInt($("#ModSolMaximo_RP").val())) {
                $(this).attr('checked', false);
                $("#MCMIcono").attr("class", "validacion");
                $("#MCMContenedor").html("Solo se puede seleccionar [" + $("#ModSolMaximo_RP").val() + "] cotizaciones");
                $("#ModalCuadroMensaje").dialog({ title: "Validación" });
                $("#ModalCuadroMensaje").dialog("open");

            } else {
                $("input.ChkCotizacionesReportePlus:checkbox[name=" + $(check).data("solicitud") + "]").each(function (index, val) {
                    if (Max < parseInt($("#ModSolMaximo_RP").val())) {
                        $(this).attr('checked', true);
                        var filaPadre = $(this).parent().parent();
                        var clasePadre = filaPadre.attr('class');

                        setTimeout(function () {
                            filaPadre.addClass("grilla_active");
                        }, 0);

                    } else {
                        return false;

                    }
                    Max = Max + 1;

                });

            }

        } else {
            $("input.ChkCotizacionesReportePlus:checkbox[name=" + $(check).data("solicitud") + "]").attr('checked', false);
            var filaPadre = $("input.ChkCotizacionesReportePlus:checkbox[name=" + $(check).data("solicitud") + "]").parent().parent();
            var clasePadre = filaPadre.attr('class');
            setTimeout(function () {
                filaPadre.closest('table').find('tr').removeClass("grilla_active");
            }, 0);
        }


        $("#TabSolicitudesReportePlus tbody tr input.ChkSolicitudesReportePlus[type=checkbox]:checked").each(function (index, val) {
            solicitudes.push($(this).data("solicitud"));
        });

        $(".TabCotizacionesReportePlus input.ChkCotizacionesReportePlus[type=checkbox]:checked").each(function (index, val) {
            cotizaciones.push($(this).val());
        });

        var params = {
            solicitudes: solicitudes,
            cotizaciones: cotizaciones
        }

        $.ajax({
            type: "POST",
            url: "CompararCotizacionPlus.aspx/GuardandoCheck",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al Guardar Check");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
            }
        });

    });

    /* Seleccionar registro de cotizacion */
    $("#TabCotizacionesReportePlus input[type=checkbox]").live('change', function () {
        var radio = $(this);
        var filaPadre = $(this).parent().parent();
        var clasePadre = filaPadre.attr('class');

        if ($("input.ChkCotizacionesReportePlus:checkbox[name =" + $(this).data("solicitud") + "]:checked").length > 0) {

            if ($(radio).is(':checked')) {
                var Max = $("input.ChkCotizacionesReportePlus[type=checkbox]:checked").length;
                if (Max > parseInt($("#ModSolMaximo_RP").val())) {
                    $(this).attr('checked', false);
                    $("#MCMIcono").attr("class", "validacion");
                    $("#MCMContenedor").html("Solo se puede seleccionar [" + $("#ModSolMaximo_RP").val() + "] cotizaciones");
                    $("#ModalCuadroMensaje").dialog({ title: "Validación" });
                    $("#ModalCuadroMensaje").dialog("open");
                    return;
                }
            }

            $("input.ChkSolicitudesReportePlus:checkbox[name=" + $(this).data("solicitud") + "]").attr('checked', true);
        }

        else {
            $("input.ChkSolicitudesReportePlus:checkbox[name=" + $(this).data("solicitud") + "]").attr('checked', false);
        }

        setTimeout(function () {
            if ($(radio).is(":checked")) {
                filaPadre.addClass("grilla_active");
            } else {
                filaPadre.removeClass("grilla_active");
            }
        }, 0);


        var solicitudes = new Array();
        var cotizaciones = new Array();

        $("#TabSolicitudesReportePlus tbody tr input.ChkSolicitudesReportePlus[type=checkbox]:checked").each(function (index, val) {
            solicitudes.push($(this).data("solicitud"));
        });

        $(".TabCotizacionesReportePlus input.ChkCotizacionesReportePlus[type=checkbox]:checked").each(function (index, val) {
            cotizaciones.push($(this).val());
        });

        var params = {
            solicitudes: solicitudes,
            cotizaciones: cotizaciones
        }

        $.ajax({
            type: "POST",
            url: "CompararCotizacionPlus.aspx/GuardandoCheck",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {

                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al Guardar Check");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {

            }
        });
    });

    /* Cambiar valor en combobox de Maximo */
    $('#ModSolMaximo_RP').live('change', function (e) {

        var solicitudes = new Array();
        var cotizaciones = new Array();

        $("#TabSolicitudesReportePlus tbody tr input.ChkSolicitudesReportePlus[type=checkbox]:checked").each(function (index, val) {
            var filaPadre = $(this).parent().parent();
            var clasePadre = filaPadre.attr('class');
            $(this).prop('checked', false);
            setTimeout(function () {
                filaPadre.removeClass("grilla_active");
            }, 0);
        });

        $(".TabCotizacionesReportePlus input.ChkCotizacionesReportePlus[type=checkbox]:checked").each(function (index, val) {
            var filaPadre = $(this).parent().parent();
            var clasePadre = filaPadre.attr('class');
            $(this).prop('checked', false);
            setTimeout(function () {
                filaPadre.removeClass("grilla_active");
            }, 0);
        });

        var params = {
            solicitudes: solicitudes,
            cotizaciones: cotizaciones
        }

        $.ajax({
            type: "POST",
            url: "CompararCotizacionPlus.aspx/GuardandoCheck",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {

                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al Guardar Check");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {

            }
        });

    });

    $("#LinkConsentimientoAsesoria").live("click", function (e) {
        $("#MCConstmAsesIcono").attr("class", "advertencia");
        $("#MCConstmAsesContenedor").html("<div class=\"alerta-agente-contenido\"><p>Usted va a enviar el enlace de consentimiento de asesoría del producto <span class=\"resaltado\">Rentas Vitalicias</span> al cliente <span class=\"resaltado\">" + $("#Nombres").val() + " " + $("#ApellidoPaterno").val() + " " + $("#ApellidoMaterno").val() + "</span> identificado con el <span class=\"resaltado\">" + $("#TipoDocumento").find(":selected").text() + "</span> <span class=\"resaltado\">" + $("#NumeroDocumento").val() + "</span> al siguiente correo electrónico: <span class=\"resaltado\">" + $("#CorreoElectronico").val() + "</span></p><p>Verifique que los datos sean correctos, en caso haya un error, por favor modifique los datos en el vtiger, luego actualice la página en el Cotizador Web de Rentas y vuelva a intentarlo.</p></div>");
        $("#ModalConsentimientoAsesoria").dialog({ title: "Consentimiento de asesoría" });
        $("#ModalConsentimientoAsesoria").dialog("open");
    });

    $("#LinkConsentimientoAsesoriaSMS").live("click", function (e) {
        $("#MCConstmAsesIconoSMS").attr("class", "advertencia");

        var contenido = "<p>Usted va a enviar el enlace de consentimiento de asesoría del producto <span class=\"resaltado\">Rentas Vitalicias</span> al cliente <span class=\"resaltado\">" + $("#Nombres").val() + " " + $("#ApellidoPaterno").val() + " " + $("#ApellidoMaterno").val() + "</span> identificado con el <span class=\"resaltado\">" + $("#TipoDocumento").find(":selected").text() + "</span> <span class=\"resaltado\">" + $("#NumeroDocumento").val() + "</span> al siguiente número de celular: <span class=\"resaltado\">" + $("#Celular").val() + "</span></p>"
        contenido += "<p>Verifique que los datos sean correctos, en caso haya un error, por favor modifique los datos en el vtiger, luego actualice la página en el Cotizador Web de Rentas y vuelva a intentarlo.</p>";

        $("#MCConstmAsesContenedorSMS").html("<div class=\"alerta-agente-contenido\">" + contenido + "</div>");
        $("#ModalConsentimientoAsesoriaSMS").dialog({ title: "Consentimiento de asesoría" });
        $("#ModalConsentimientoAsesoriaSMS").dialog("open");
    });

    /* Botón Cancelar Consentimiento Asesoría */
    $("#MCConstmAsesCancelar").live("click", function () {
        $("#ModalConsentimientoAsesoria").dialog("close");
    });

    /* Botón Cancelar Consentimiento Asesoría SMS */
    $("#MCConstmAsesCancelarSMS").live("click", function () {
        $("#ModalConsentimientoAsesoriaSMS").dialog("close");
    });

    /* Botón Enviar Consentimiento Asesoría */
    $("#MCConstmAsesEnviar").live("click", function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Enviando email con el enlace al Consentimiento de Asesoría, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            parametros: {
                nombres: $("#Nombres").val(),
                tipoDocumento: $("#TipoDocumento").val(),
                numeroDocumento: $("#NumeroDocumento").val(),
                correo: $("#CorreoElectronico").val(),
                cuspp: $("#HCUSPP").val(),
                token: $("#HToken").val(),
                categoria: $("#Categoria").val(),
                apellidoPaterno: $("#ApellidoPaterno").val(),
                apellidoMaterno: $("#ApellidoMaterno").val(),
                sexo: $("#Sexo").val(),
                fechaNacimiento: $("#FechaNacimiento").val(),
                afp: $("#AFP").find(":selected").text(),
                telefono: $("#Telefono").val(),
                celular: $("#Celular").val(),
                //idConsentimientoAsesoria: $("#HidConsentimientoAsesoria").val(),
                indConsentimiento: $("#HindConsentimiento").val(),
                glsCategoria: $("#Categoria option:selected").text(),
                canalComunicacion: "EMAIL"
            }
        }

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/EnviarConsentimientoAsesoria",
            contentType: "application/json",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#ModalConsentimientoAsesoria").dialog("close");

                    // Mostrar mensaje de éxito
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else {
                    $("#ModalConsentimientoAsesoria").dialog("close");

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

    /* Botón Enviar Consentimiento Asesoría SMS*/
    $("#MCConstmAsesEnviarSMS").live("click", function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Enviando SMS con el enlace al Consentimiento de Asesoría, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            parametros: {
                nombres: $("#Nombres").val(),
                tipoDocumento: $("#TipoDocumento").val(),
                numeroDocumento: $("#NumeroDocumento").val(),
                correo: $("#CorreoElectronico").val(),
                cuspp: $("#HCUSPP").val(),
                token: $("#HToken").val(),
                categoria: $("#Categoria").val(),
                apellidoPaterno: $("#ApellidoPaterno").val(),
                apellidoMaterno: $("#ApellidoMaterno").val(),
                sexo: $("#Sexo").val(),
                fechaNacimiento: $("#FechaNacimiento").val(),
                //afp: $("#AFP").find(":selected").text(),
                telefono: $("#Telefono").val(),
                celular: $("#Celular").val(),
                //idConsentimientoAsesoria: $("#HidConsentimientoAsesoria").val(),
                //indConsentimiento: $("#HindConsentimiento").val(),
                glsCategoria: $("#Categoria option:selected").text(),
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
                    $("#ModalConsentimientoAsesoriaSMS").dialog("close");

                    // Mostrar mensaje de éxito
                    $('#MCMIcono').attr('class', data.d.Icono);
                    $('#MCMContenedor').html(data.d.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else {
                    $("#ModalConsentimientoAsesoriaSMS").dialog("close");

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

    $("#PlantillaConsentimientoAsesoria").live("click", function (e) {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Descargando el Formato de Consentimiento, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Descargando" });
        $("#ModalCotizando").dialog("open");

        $('#ModConsentimientoAsesoriaCargando').show();

        var params = {
            idConsentimientoAsesoria: $("#HidConsentimientoAsesoria").val(),
            tipoDocumento: $("#TipoDocumento").val(),
            numeroDocumento: $("#NumeroDocumento").val()
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

                    window.open("../ArchivosTemporales/RVI/Consentimiento/Consentimiento_" + $("#TipoDocumento").val() + $("#NumeroDocumento").val() + ".pdf", "_blank");

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else {
                    $("#MCMIcono").attr("class", data.d.Icono);
                    $("#MCMContenedor").html(data.d.Mensaje);
                    $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                    $("#ModalCuadroMensaje").dialog("open");

                    $('#ModConsentimientoAsesoriaCargando').fadeOut();
                    $("#ModalConsentimientoAsesoria").dialog("close");
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

    $("#ReenviarPlantillaConsentimientoAsesoria").live("click", function () {
        $("#MRCAIcono").attr("class", "advertencia");
        if ($("#Categoria").val() != "D") {
            $("#MRCAContenedor").html("<div class=\"alerta-agente-contenido\"><p>Usted va a enviar el formato firmado de consentimiento de asesoría al cliente <span class=\"resaltado\">" + $("#Nombres").val() + " " + $("#ApellidoPaterno").val() + " " + $("#ApellidoMaterno").val() + "</span> al siguiente correo electrónico: <span class=\"resaltado\">" + $("#CorreoElectronico").val() + "</span></p><p>Verifique que los datos sean correctos antes de realizar el envío.</p></div>");
        }
        else {
            $("#MRCAContenedor").html("<div class=\"alerta-agente-contenido\"><p>Usted va a enviar el formato de consentimiento de asesoría firmado por el beneficiario al siguiente correo electrónico: <span class=\"resaltado\">" + $("#CorreoElectronico").val() + "</span></p><p>Verifique que los datos sean correctos antes de realizar el envío.</p></div>");
        }
        $("#ModalReenvioConsentimientoAsesoria").dialog({ title: "Reenviar Consentimiento de Asesoría" });
        $("#ModalReenvioConsentimientoAsesoria").dialog("open");
    });

    $("#MRCACancelar").live("click", function () {
        $("#ModalReenvioConsentimientoAsesoria").dialog("close");
    });

    $("#MRCAEnviar").live("click", function (e) {
        $("#ModalReenvioConsentimientoAsesoria").dialog("close");
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Reenviando el Formato de Consentimiento, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            idConsentimientoAsesoria: $("#HidConsentimientoAsesoria").val(),
            categoria: $("#Categoria").val(),
            tipoIdentificacion: $("#TipoDocumento").val(),
            numeroIdentificacion: $("#NumeroDocumento").val(),
            nombre: $("#Nombres").val(),
            apellidoPaterno: $("#ApellidoPaterno").val(),
            apellidoMaterno: $("#ApellidoMaterno").val(),
            email: $("#CorreoElectronico").val(),
            numAgente: $("#NumeroAgente").val()
        };

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/ReenviarFormatoConsentimientoAsesoria",
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

    $("#dlBeneficiarios").live("change", function () {
        $("#consentimiento_asesoria_sobrevivencia_pie").hide();
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Validando beneficiario, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Validando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            cod_beneficiario: $("#dlBeneficiarios").val(),
            correo_electronico: $("#CorreoElectronico").val(),
            cuspp: $("#CUSPP").val()
        }

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/ValidacionConsentimientoAsesoria",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $("#consentimiento_asesoria_sobrevivencia_pie").show();
                }
                else if (data.d.Estado == "CU") {
                    document.location.reload(true);
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
                    $("#MCMContenedor").html("Ha ocurrido un error.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: () => {
                $("#ModalCotizando").dialog("close");
            }
        });
    });

    $('#LinkConsentimientoAsesoriaSobrevivencia').live('click', function (e) {
        //var params = {
        //    cod_beneficiario: $('#dlBeneficiarios').val(),
        //    correo_electronico: $('#CorreoElectronico').val()
        //}

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/SobrevivenciaConsentimientoAsesoria",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: null,
            success: function (data) {
                if (data.d.Estado == "OK") {
                    $('#MCConstmAsesIcono').attr('class', 'advertencia');
                    $('#MCConstmAsesContenedor').html(data.d.Mensaje);
                    $('#ModalConsentimientoAsesoria').dialog({ title: 'Consentimiento de asesoría' });
                    $('#ModalConsentimientoAsesoria').dialog('open');
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
                    $("#MCMContenedor").html("Ha ocurrido un error.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            }
        });

    });

    $('#TabSolicitudes .grilla_cerrar').live('click', function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Cargando la información de la solicitud, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Cargando" });
        $("#ModalCotizando").dialog("open");

        //var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
        //idSolicitud = $(this).data('solicitud');
        //var tipo_cotizacion = $(this).data('tipo_cotizacion');
        //var flag_sobrevivencia = false;

        //if (tipo_cotizacion == 'D') {
        //    flag_sobrevivencia = true;
        //}

        //var params = {
        //    idSolicitud: idSolicitud,
        //    //fecCotizacion: fecCotizacion,
        //    //accion: 'CERRAR'
        //    flagSobrevivencia: flag_sobrevivencia
        //}

        //$.ajax({
        //    type: 'POST',
        //    url: 'Cotizador.aspx/SessionIdSolicitud',
        //    contentType: "application/json; charset=iso-8859-1",
        //    dataType: 'json',
        //    data: $.toJSON(params),
        //    success: function (data) {
        //        //var solicitud = data.d;

        //        //if (idSolicitud.toString() == solicitud.toString()) {
        //        //    window.location.href = "CerrarSolicitud.aspx";
        //        //}

        //        if (flag_sobrevivencia) {

        //        }
        //        else {
        //            window.location.href = "BeneficiarioCierre.aspx";
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
        //        $("#ModalCotizando").dialog("close");
        //    }
        //});

    });

    /* Cambiar valor en combobox de Departamento */
    $('#ModDepartamentoPrinc').live('change', function (e) {
        $('#ModCargandoProvincia').show();
        $.ajax({
            type: 'POST',
            url: 'BeneficiarioCierre.aspx/CargarComboCiudades',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idDepartamento:'" + $('#ModDepartamentoPrinc').val() + "'}",
            success: function (data) {
                $('#ModControlProvinciaPrinc').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#ModProvinciaPrinc');

                //$.each(data.d, function (key, registro) {
                //    $("#ModProvinciaPrinc").append('<option value=' + registro.Codigo + '>' + registro.descripcion + '</option>');
                //});

                $('#ModDistritoPrinc').val('0');
                $('#ModDistritoPrinc').trigger('change');
                $('#ModDistritoPrinc').attr('disabled', 'disabled');
                $('#ModProvinciaPrinc').focus();
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
    $('#ModProvinciaPrinc').live('change', function (e) {
        $('#ModCargandoDistrito').show();
        $.ajax({
            type: 'POST',
            url: 'BeneficiarioCierre.aspx/CargarComboComunas',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idCiudad:'" + $('#ModProvinciaPrinc').val() + "'}",
            success: function (data) {
                $('#ModControlDistritoPrinc').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#ModDistritoPrinc');
                $('#ModDistritoPrinc').focus();
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

    //$('#NuevaDireccion').live('click', function () {
    //    $('#Direccion').show();
    //    $('#NuevaDireccion').hide()
    //});

    $('#dlDirecciones').live('change', function (e) {
        if ($('#dlDirecciones').val() != 0) {
            //$('#Direccion').hide();
            //$('#NuevaDireccion').hide()

            var params = {
                idDireccion: $('#dlDirecciones').val()
            }

            $.ajax({
                type: 'POST',
                url: 'BeneficiarioCierre.aspx/DirecionPrincipal',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    direccion = data.d;
                    $('#ModTipoViaPrinc').val(direccion.TipoVia.Id);
                    $('#TexModTipoViaPrinc').html($('#ModTipoViaPrinc').find(':selected').text());
                    $('#ModDireccionPrinc').val(direccion.Glosa);
                    $('#ModEspacioUrbanoPrinc').val(direccion.EspacioUrbano);
                    $('#ModDepartamentoPrinc').val(direccion.Departamento.Id);
                    $('#TexModDepartamentoPrinc').html($('#ModDepartamentoPrinc').find(':selected').text());
                    //$('#ModProvinciaPrinc').val(direccion.Ciudad.Id);
                    //$('#TexModProvinciaPrinc').html($('#ModProvinciaPrinc').find(':selected').text());
                    //$('#ModDistritoPrinc').val(direccion.Departamento.Id);
                    //$('#TexModDistritoPrinc').html($('#ModDistritoPrinc').find(':selected').text());

                    $.ajax({
                        type: 'POST',
                        url: 'BeneficiarioCierre.aspx/CargarComboCiudades',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: "{idDepartamento:'" + $('#ModDepartamentoPrinc').val() + "'}",
                        success: function (data) {
                            $('#ModControlProvinciaPrinc').html($(data.d).find('#ContenidoDinamico').html());
                            $('#ModProvinciaPrinc').val(direccion.Ciudad.Id);
                            ActualizaEstiloCombobox('#ModProvinciaPrinc');
                            $.ajax({
                                type: 'POST',
                                url: 'BeneficiarioCierre.aspx/CargarComboComunas',
                                contentType: "application/json; charset=iso-8859-1",
                                dataType: 'json',
                                data: "{idCiudad:'" + $('#ModProvinciaPrinc').val() + "'}",
                                success: function (data) {
                                    $('#ModControlDistritoPrinc').html($(data.d).find('#ContenidoDinamico').html());
                                    $('#ModDistritoPrinc').val(direccion.Comuna.Id);
                                    ActualizaEstiloCombobox('#ModDistritoPrinc');
                                    //$('#ModDirDireccion').focus();
                                    //$('#ModDirCargando').fadeOut();
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
                    $("#ModalCotizando").dialog("close");
                }
            });
        }
        else {
            //$('#NuevaDireccion').show()
        }
    });

    $('#EnvioPoliza').live('change', function (e) {
        if ($('#EnvioPoliza').val() == 'F') {
            $('#DireccionAlterna').show()
        }
        else {
            $('#DireccionAlterna').hide()
        }
    });

    //$('#NuevaDireccionAlterna').live('click', function () {
    //    $('#DireccionAlternaNueva').show();
    //    $('#NuevaDireccionAlterna').hide()
    //});

    /* Cambiar valor en combobox de Departamento */
    $('#ModDepartamentoAlterna').live('change', function (e) {
        $('#ModCargandoProvinciaAlterna').show();
        $.ajax({
            type: 'POST',
            url: 'BeneficiarioCierre.aspx/CargarComboCiudadesAlterna',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idDepartamento:'" + $('#ModDepartamentoAlterna').val() + "'}",
            success: function (data) {
                $('#ModControlProvinciaAlterna').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#ModProvinciaAlterna');

                $('#ModDistritoAlterna').val('0');
                $('#ModDistritoAlterna').trigger('change');
                $('#ModDistritoAlterna').attr('disabled', 'disabled');
                $('#ModProvinciaAlterna').focus();
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
    $('#ModProvinciaAlterna').live('change', function (e) {
        $('#ModCargandoDistritoAlterna').show();
        $.ajax({
            type: 'POST',
            url: 'BeneficiarioCierre.aspx/CargarComboComunasAlterna',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: "{idCiudad:'" + $('#ModProvinciaAlterna').val() + "'}",
            success: function (data) {
                $('#ModControlDistritoAlterna').html($(data.d).find('#ContenidoDinamico').html());
                ActualizaEstiloCombobox('#ModDistritoAlterna');
                $('#ModDistritoAlterna').focus();
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

    $('#dlDireccionesAlterna').live('change', function (e) {
        if ($('#dlDireccionesAlterna').val() != 0) {
            var params = {
                idDireccion: $('#dlDireccionesAlterna').val()
            }

            $.ajax({
                type: 'POST',
                url: 'BeneficiarioCierre.aspx/DirecionPrincipal',
                contentType: "application/json; charset=iso-8859-1",
                dataType: 'json',
                data: $.toJSON(params),
                success: function (data) {
                    direccion = data.d;
                    $('#ModTipoViaAlterna').val(direccion.TipoVia.Id);
                    $('#TexModTipoViaAlterna').html($('#ModTipoViaAlterna').find(':selected').text());
                    $('#ModDireccionAlterna').val(direccion.Glosa);
                    $('#ModEspacioUrbanoAlterna').val(direccion.EspacioUrbano);
                    $('#ModDepartamentoAlterna').val(direccion.Departamento.Id);
                    $('#TexModDepartamentoAlterna').html($('#ModDepartamentoAlterna').find(':selected').text());

                    $.ajax({
                        type: 'POST',
                        url: 'BeneficiarioCierre.aspx/CargarComboCiudadesAlterna',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: "{idDepartamento:'" + $('#ModDepartamentoAlterna').val() + "'}",
                        success: function (data) {
                            $('#ModControlProvinciaAlterna').html($(data.d).find('#ContenidoDinamico').html());
                            $('#ModProvinciaAlterna').val(direccion.Ciudad.Id);
                            ActualizaEstiloCombobox('#ModProvinciaAlterna');
                            $.ajax({
                                type: 'POST',
                                url: 'BeneficiarioCierre.aspx/CargarComboComunasAlterna',
                                contentType: "application/json; charset=iso-8859-1",
                                dataType: 'json',
                                data: "{idCiudad:'" + $('#ModProvinciaAlterna').val() + "'}",
                                success: function (data) {
                                    $('#ModControlDistritoAlterna').html($(data.d).find('#ContenidoDinamico').html());
                                    $('#ModDistritoAlterna').val(direccion.Comuna.Id);
                                    ActualizaEstiloCombobox('#ModDistritoAlterna');
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
                    $("#ModalCotizando").dialog("close");
                }
            });
        }
        else {
            //$('#NuevaDireccion').show()
        }
    });

    /* Botón Cancelar */
    $('#ModCierreCancelar').live('click', function () {
        window.location.href = "Cotizador.aspx";
    });

    /* Botón Aceptar */
    $('#ModCierreAceptar').live('click', function () {
        var esCorrecto = true;
        var errores = new Array();
        var direccionAlterna = false;

        $('#CierreBeneficiarioMensajeExito').html("");
        $('#CierreBeneficiarioMensajeExito').hide();
        $('#CierreBeneficiarioMensajeError').html("");
        $('#CierreBeneficiarioMensajeError').hide();

        if ($('#EnvioPoliza').val() == 'F') {
            direccionAlterna = true;
        }

        $('#ApellidoPaterno').removeClass('formTextboxError');
        $('#ApellidoMaterno').removeClass('formTextboxError');
        $('#Nombres').removeClass('formTextboxError');
        $('#NumeroIdentificacion').removeClass('formTextboxError');
        $('#Telefono').removeClass('formTextboxError');
        $('#Celular').removeClass('formTextboxError');
        $('#CorreoElectronico').removeClass('formTextboxError');
        $('#CentroLabores').removeClass('formTextboxError');

        $('#ConTipoIdentificacion').removeClass('formComboboxErrorContenedor');

        $('#ModDireccionPrinc').removeClass('formTextboxError');
        $('#ModEspacioUrbanoPrinc').removeClass('formTextboxError');

        $('#ConModTipoViaPrinc').removeClass('formComboboxErrorContenedor');
        $('#ConModDepartamentoPrinc').removeClass('formComboboxErrorContenedor');
        $('#ConModProvinciaPrinc').removeClass('formComboboxErrorContenedor');
        $('#ConModDistritoPrinc').removeClass('formComboboxErrorContenedor');

        $('#ConEnvioPoliza').removeClass('formComboboxErrorContenedor');

        if (direccionAlterna) {
            $('#ModDireccionAlterna').removeClass('formTextboxError');
            $('#ModEspacioUrbanoAlterna').removeClass('formTextboxError');
            $('#ModPersonaAutorizadaAlterna').removeClass('formTextboxError');

            $('#ConModTipoViaAlterna').removeClass('formComboboxErrorContenedor');
            $('#ConModDepartamentoAlterna').removeClass('formComboboxErrorContenedor');
            $('#ConModProvinciaAlterna').removeClass('formComboboxErrorContenedor');
            $('#ConModDistritoAlterna').removeClass('formComboboxErrorContenedor');
        }

        var apellidoPaterno = ($.trim($('#ApellidoPaterno').val()).length > 0) ? true : false;
        var apellidoMaterno = ($.trim($('#ApellidoMaterno').val()).length > 0) ? true : false;
        var nombres = ($.trim($('#Nombres').val()).length > 0) ? true : false;
        var numeroIdentificacion = ($.trim($('#NumeroIdentificacion').val()).length > 0) ? true : false;
        var telefono = ($.trim($('#Telefono').val()).length > 0) ? true : false;
        var celular = ($.trim($('#Celular').val()).length > 0) ? true : false;
        var correoElectronico = ($.trim($('#CorreoElectronico').val()).length > 0) ? true : false;
        var centroLabores = ($.trim($('#CentroLabores').val()).length > 0) ? true : false;

        var tipoIdentificacion = ($('#TipoIdentificacion').val() != '0') ? true : false;


        var modDireccionPrinc = ($.trim($('#ModDireccionPrinc').val()).length > 0) ? true : false;
        var modEspacioUrbanoPrinc = ($.trim($('#ModEspacioUrbanoPrinc').val()).length > 0) ? true : false;

        var modTipoViaPrinc = ($('#ModTipoViaPrinc').val() != '0') ? true : false;
        var modDepartamentoPrinc = ($('#ModDepartamentoPrinc').val() != '0') ? true : false;
        var modProvinciaPrinc = ($('#ModProvinciaPrinc').val() != '0') ? true : false;
        var modDistritoPrinc = ($('#ModDistritoPrinc').val() != '0') ? true : false;


        var envioPoliza = ($('#EnvioPoliza').val() != '0') ? true : false;


        if (direccionAlterna) {
            var modDireccionAlterna = ($.trim($('#ModDireccionAlterna').val()).length > 0) ? true : false;
            var modEspacioUrbanoAlterna = ($.trim($('#ModEspacioUrbanoAlterna').val()).length > 0) ? true : false;
            var modPersonaAutorizadaAlterna = ($.trim($('#ModPersonaAutorizadaAlterna').val()).length > 0) ? true : false;

            var modTipoViaAlterna = ($('#ModTipoViaAlterna').val() != '0') ? true : false;
            var modDepartamentoAlterna = ($('#ModDepartamentoAlterna').val() != '0') ? true : false;
            var modProvinciaAlterna = ($('#ModProvinciaAlterna').val() != '0') ? true : false;
            var modDistritoAlterna = ($('#ModDistritoAlterna').val() != '0') ? true : false;
        }

        if (!apellidoPaterno) {
            errores.push('Ingrese el campo <strong>Apellido Paterno</strong>. Dato Obligatorio.');
            $('#ApellidoPaterno').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!apellidoMaterno) {
            errores.push('Ingrese el campo <strong>Apellido Materno</strong>. Dato Obligatorio.');
            $('#ApellidoMaterno').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!nombres) {
            errores.push('Ingrese el campo <strong>Nombres</strong>. Dato Obligatorio.');
            $('#Nombres').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!numeroIdentificacion) {
            errores.push('Ingrese el campo <strong>Número Identificacion</strong>. Dato Obligatorio.');
            $('#NumeroIdentificacion').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!telefono) {
            errores.push('Ingrese el campo <strong>Teléfono</strong>. Dato Obligatorio.');
            $('#Telefono').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!celular) {
            errores.push('Ingrese el campo <strong>Celular</strong>. Dato Obligatorio.');
            $('#Celular').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!correoElectronico) {
            errores.push('Ingrese el campo <strong>Correo Electrónico</strong>. Dato Obligatorio.');
            $('#CorreoElectronico').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!centroLabores) {
            errores.push('Ingrese el campo <strong>Centro de Labores</strong>. Dato Obligatorio.');
            $('#CentroLabores').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!tipoIdentificacion) {
            errores.push('Ingrese el campo <strong>Tipo de Identificación</strong>. Dato Obligatorio.');
            $('#ConTipoIdentificacion').addClass('formComboboxErrorContenedor');
            esCorrecto = false;
        }

        if (!modDireccionPrinc) {
            errores.push('Ingrese el campo <strong>Dirección Principal</strong>. Dato Obligatorio.');
            $('#ModDireccionPrinc').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!modEspacioUrbanoPrinc) {
            errores.push('Ingrese el campo <strong>Espacio Urbano Principal</strong>. Dato Obligatorio.');
            $('#ModEspacioUrbanoPrinc').addClass('formTextboxError');
            esCorrecto = false;
        }

        if (!modTipoViaPrinc) {
            errores.push('Ingrese el campo <strong>Tipo de Vía Principal</strong>. Dato Obligatorio.');
            $('#ConModTipoViaPrinc').addClass('formComboboxErrorContenedor');
            esCorrecto = false;
        }

        if (!modDepartamentoPrinc) {
            errores.push('Ingrese el campo <strong>Departamento Principal</strong>. Dato Obligatorio.');
            $('#ConModDepartamentoPrinc').addClass('formComboboxErrorContenedor');
            esCorrecto = false;
        }

        if (!modProvinciaPrinc) {
            errores.push('Ingrese el campo <strong>Provincia Principal</strong>. Dato Obligatorio.');
            $('#ConModProvinciaPrinc').addClass('formComboboxErrorContenedor');
            esCorrecto = false;
        }

        if (!modDistritoPrinc) {
            errores.push('Ingrese el campo <strong>Distrito Principal</strong>. Dato Obligatorio.');
            $('#ConModDistritoPrinc').addClass('formComboboxErrorContenedor');
            esCorrecto = false;
        }

        if (!envioPoliza) {
            errores.push('Ingrese el campo <strong>Envío Póliza</strong>. Dato Obligatorio.');
            $('#ConEnvioPoliza').addClass('formComboboxErrorContenedor');
            esCorrecto = false;
        }

        if (direccionAlterna) {
            if (!modDireccionAlterna) {
                errores.push('Ingrese el campo <strong>Dirección Alterna</strong>. Dato Obligatorio.');
                $('#ModDireccionAlterna').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (!modEspacioUrbanoAlterna) {
                errores.push('Ingrese el campo <strong>Espacio Urbano Alterna</strong>. Dato Obligatorio.');
                $('#ModEspacioUrbanoAlterna').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (!modPersonaAutorizadaAlterna) {
                errores.push('Ingrese el campo <strong>Persona Autorizada</strong>. Dato Obligatorio.');
                $('#ModPersonaAutorizadaAlterna').addClass('formTextboxError');
                esCorrecto = false;
            }

            if (!modTipoViaAlterna) {
                errores.push('Ingrese el campo <strong>Tipo Vía Alterna</strong>. Dato Obligatorio.');
                $('#ConModTipoViaAlterna').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!modDepartamentoAlterna) {
                errores.push('Ingrese el campo <strong>Departamento Alterna</strong>. Dato Obligatorio.');
                $('#ConModDepartamentoAlterna').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!modProvinciaAlterna) {
                errores.push('Ingrese el campo <strong>Provincia Alterna</strong>. Dato Obligatorio.');
                $('#ConModProvinciaAlterna').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }

            if (!modDistritoAlterna) {
                errores.push('Ingrese el campo <strong>Distrito Alterna</strong>. Dato Obligatorio.');
                $('#ConModDistritoAlterna').addClass('formComboboxErrorContenedor');
                esCorrecto = false;
            }
        }

        if (!esCorrecto) {
            $('#MCMIcono').attr('class', 'validacion');
            $('#MCMContenedor').html(formatearError(errores));
            $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
            $('#ModalCuadroMensaje').dialog('open');
            return false;
        }

        /* Pasó las validaciones */
        //ModDirBotonesInactivos = true;
        //$('#ModCierreAceptar').attr('class', 'botonDeshabilitado gris gris_sharp');
        //$('#ModCargando').fadeIn();

        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Guardando datos, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Guardando" });
        $("#ModalCotizando").dialog("open");

        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            apellidoPaterno: $('#ApellidoPaterno').val(),
            apellidoMaterno: $('#ApellidoMaterno').val(),
            nombres: $('#Nombres').val(),
            numeroIdentificacion: $('#NumeroIdentificacion').val(),
            telefono: $('#Telefono').val(),
            celular: $('#Celular').val(),
            correoElectronico: $('#CorreoElectronico').val(),
            centroLabores: $('#CentroLabores').val(),
            tipoIdentificacion: $('#TipoIdentificacion').val(),
            direccionPrinc: $('#ModDireccionPrinc').val(),
            espacioUrbanoPrinc: $('#ModEspacioUrbanoPrinc').val(),
            tipoViaPrinc: $('#ModTipoViaPrinc').val(),
            departamentoPrinc: $('#ModDepartamentoPrinc').val(),
            provinciaPrinc: $('#ModProvinciaPrinc').val(),
            distritoPrinc: $('#ModDistritoPrinc').val(),
            envioPoliza: $('#EnvioPoliza').val(),
            direccionAlterna: $('#ModDireccionAlterna').val(),
            espacioUrbanoAlterna: $('#ModEspacioUrbanoAlterna').val(),
            personaAutorizadaAlterna: $('#ModPersonaAutorizadaAlterna').val(),
            tipoViaAlterna: $('#ModTipoViaAlterna').val(),
            departamentoAlterna: $('#ModDepartamentoAlterna').val(),
            provinciaAlterna: $('#ModProvinciaAlterna').val(),
            distritoAlterna: $('#ModDistritoAlterna').val(),
            numSolicitud: $('#HNumSolicitud').val(),
            numCorrelativo: $('#HNumItem').val()
        }
        var postUrl = 'BeneficiarioCierre.aspx/Cierre';

        $.ajax({
            type: 'POST',
            url: postUrl,
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == 'OK') {

                    $('#HNombres').val($('#Nombres').val()),
                        $("#HCorreo").val($('#CorreoElectronico').val()),

                        //$('#MCMIcono').attr('class', data.d.Icono);
                        //$('#MCMContenedor').html(data.d.Mensaje);
                        //$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                        //$('#ModalCuadroMensaje').dialog('open');

                        $("#ModCierreEnviar").show();
                    $('#CierreBeneficiarioMensajeExito').html("La información se actualizó con éxito, ahora puede enviar el enlace al cliente con el boton <b>Enviar</b>.");
                    $('#CierreBeneficiarioMensajeExito').show();

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
                        if (data.d.Controles[0].length) $('#ApellidoPaterno').attr('class', data.d.Controles[0]);
                        if (data.d.Controles[1].length) $('#ApellidoMaterno').attr('class', data.d.Controles[1]);
                        if (data.d.Controles[2].length) $('#Nombres').attr('class', data.d.Controles[2]);
                        if (data.d.Controles[3].length) $('#NumeroIdentificacion').attr('class', data.d.Controles[3]);
                        if (data.d.Controles[4].length) $('#Telefono').attr('class', data.d.Controles[4]);
                        if (data.d.Controles[5].length) $('#Celular').attr('class', data.d.Controles[5]);
                        if (data.d.Controles[6].length) $('#CorreoElectronico').attr('class', data.d.Controles[6]);
                        if (data.d.Controles[7].length) $('#CentroLabores').attr('class', data.d.Controles[7]);
                        if (data.d.Controles[8].length) $('#ConTipoIdentificacion').attr('class', data.d.Controles[8]);
                        if (data.d.Controles[9].length) $('#ModDireccionPrinc').attr('class', data.d.Controles[9]);
                        if (data.d.Controles[10].length) $('#ModEspacioUrbanoPrinc').attr('class', data.d.Controles[10]);
                        if (data.d.Controles[11].length) $('#ConModTipoViaPrinc').attr('class', data.d.Controles[11]);
                        if (data.d.Controles[12].length) $('#ConModDepartamentoPrinc').attr('class', data.d.Controles[12]);
                        if (data.d.Controles[13].length) $('#ConModProvinciaPrinc').attr('class', data.d.Controles[13]);
                        if (data.d.Controles[14].length) $('#ConModDistritoPrinc').attr('class', data.d.Controles[14]);
                        if (data.d.Controles[15].length) $('#ConModEnvioPoliza').attr('class', data.d.Controles[15]);

                        if (envioPoliza == "F") {
                            if (data.d.Controles[16].length) $('#ModDireccionAlterna').attr('class', data.d.Controles[16]);
                            if (data.d.Controles[17].length) $('#ModEspacioUrbanoAlterna').attr('class', data.d.Controles[17]);
                            if (data.d.Controles[18].length) $('#ModPersonaAutorizadaAlterna').attr('class', data.d.Controles[18]);
                            if (data.d.Controles[19].length) $('#ConModTipoViaAlterna').attr('class', data.d.Controles[19]);
                            if (data.d.Controles[20].length) $('#ConModDepartamentoAlterna').attr('class', data.d.Controles[20]);
                            if (data.d.Controles[21].length) $('#ConModProvinciaAlterna').attr('class', data.d.Controles[21]);
                            if (data.d.Controles[22].length) $('#ConModDistritoAlterna').attr('class', data.d.Controles[22]);
                        }
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
                    //$('#MCMIcono').attr('class', 'error');
                    //$('#MCMContenedor').html('Ha ocurrido un error en el proceso de cierre.');
                    //$('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    //$('#ModalCuadroMensaje').dialog('open');

                    $('#CierreBeneficiarioMensajeError').html("Ocurrió un erro al actualizar los datos, por favor intente de nuevo, si el problema persiste contacte con <b>Mesa de Ayuda</b>.");
                    $('#CierreBeneficiarioMensajeError').show();
                }
            },
            complete: function () {
                //$('#ModCargando').fadeOut();
                //window.location.href = "Cotizador.aspx";
                $("#ModalCotizando").dialog("close");
            }
        });
    });

    $('#ModCierreEnviar').live('click', function () {
        //$('#ModConsentimientoAsesoriaCargando').show();
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Enviando información, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Enviando" });
        $("#ModalCotizando").dialog("open");

        $('#CierreBeneficiarioMensajeExito').html("");
        $('#CierreBeneficiarioMensajeExito').hide();
        $('#CierreBeneficiarioMensajeError').html("");
        $('#CierreBeneficiarioMensajeError').hide();

        var params = {
            nombres: $('#HNombres').val(),
            numSolicitud: $('#HNumSolicitud').val(),
            num_item: $('#HNumItem').val(),
            correo: $("#HCorreo").val(),
            token: $("#HToken").val()
        }

        $.ajax({
            type: "POST",
            url: "BeneficiarioCierre.aspx/EnviarSADP",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Estado == "OK") {

                    //$('#ModConsentimientoAsesoriaCargando').fadeOut();
                    //$("#ModalConsentimientoAsesoria").dialog("close");

                    $("#ModCierreEnviar").hide();

                    // Mostrar mensaje de éxito
                    //$('#MCMIcono').attr('class', data.d.Icono);
                    //$('#MCMContenedor').html(data.d.Mensaje);
                    //$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                    //$('#ModalCuadroMensaje').dialog('open');

                    $('#CierreBeneficiarioMensajeExito').html("Se envió satisfactoriamente el enlace al cliente para la Validación y Consentimiento para Tratamiento de Datos Personales (VCTP) a: <b>" + $("#HCorreo").val() + "</b>");
                    $('#CierreBeneficiarioMensajeExito').show();

                    selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                    $clock.countdown(selectedDate.toString());
                }
                else {
                    //$('#ModConsentimientoAsesoriaCargando').fadeOut();
                    //$("#ModalConsentimientoAsesoria").dialog("close");

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
                    //$("#MCMIcono").attr("class", "error");
                    //$("#MCMContenedor").html("Ha ocurrido un error al enviar el consentimiento de asesoría.");
                    //$("#ModalCuadroMensaje").dialog({ title: "Error" });
                    //$("#ModalCuadroMensaje").dialog("open");

                    $('#CierreBeneficiarioMensajeError').html("Ocurrió un erro al enviar la información, por favor intente de nuevo, si el problema persiste contacte con <b>Mesa de Ayuda</b>.");
                    $('#CierreBeneficiarioMensajeError').show();
                }
            },
            complete: function () {
                $("#ModalCotizando").dialog("close");
            }
        });
    });

    $('#TabSolicitudes .grilla_firma_digital').live('click', function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Generando reporte, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Cargando" });
        $("#ModalCotizando").dialog("open");

        let firmaDigitalToken = $(this).data("firmadigitaltoken");
        let solicitud = $(this).data("solicitud");

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            tokenFirmaDigital: firmaDigitalToken,
            solicitud: solicitud
        };

        $.ajax({
            type: "POST",
            url: "Cotizador.aspx/DescargarVCTP",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                const linkSource = `data:application/pdf;base64,${data.d}`;
                const downloadLink = document.createElement("a");
                const fileName = "FormatoVCTP_" + solicitud + ".pdf";
                downloadLink.href = linkSource;
                downloadLink.download = fileName;
                downloadLink.click();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
                else {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al descargar el formato VCTP.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            },
            complete: function () {
                $("#ModalCotizando").dialog("close");
            }
        });
    });

    $('#TipoIdentificacion, #ModTipoViaPrinc, #ModDepartamentoPrinc, #ModProvinciaPrinc, #ModDistritoPrinc, #EnvioPoliza, #ModTipoViaAlterna, #ModDepartamentoAlterna, #ModProvinciaAlterna, #ModDistritoAlterna').change(function () {
        $('#CierreBeneficiarioMensajeExito').html("");
        $('#CierreBeneficiarioMensajeExito').hide();
        $('#CierreBeneficiarioMensajeError').html("");
        $('#CierreBeneficiarioMensajeError').hide();
        $("#ModCierreEnviar").hide();
    });

    $('#ApellidoPaterno, #ApellidoMaterno, #Nombres, #NumeroIdentificacion, #Telefono, #Celular, #CorreoElectronico, #CentroLabores, #ModDireccionPrinc, #ModEspacioUrbanoPrinc, #ModDireccionAlterna, #ModEspacioUrbanoAlterna, #ModPersonaAutorizadaAlterna').keydown(function (event) {
        $('#CierreBeneficiarioMensajeExito').html("");
        $('#CierreBeneficiarioMensajeExito').hide();
        $('#CierreBeneficiarioMensajeError').html("");
        $('#CierreBeneficiarioMensajeError').hide();
        $("#ModCierreEnviar").hide();
    });

    $('#ModGruFamFechaNacimiento').live('change', function (e) {
        //MostrarApoderado();
    });

    $('#ModGruFamFlgApoderado').live('change', function (e) {
        //console.log($(this).is(":checked"));
        var checked = $(this).is(":checked");

        $('#Apoderado').toggle(checked);
    });

    //function MostrarApoderado() {
    //    day = $('#ModGruFamFechaNacimiento').val().split('/')[0];
    //    month = $('#ModGruFamFechaNacimiento').val().split('/')[1];
    //    year = $('#ModGruFamFechaNacimiento').val().split('/')[2];
    //    var fechaMayorEdad = new Date(parseInt(year) + 18, parseInt(month) - 1, parseInt(day));
    //    hoy = new Date();

    //    hoy.setHours(0, 0, 0, 0);

    //    if (fechaMayorEdad >= hoy) {
    //        $('#Apoderado').show();
    //    }
    //    else {
    //        $('#Apoderado').hide();
    //    };
    //}

    //<FIN.GTI_26560>

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

    /* Botón Aceptar */
    $("#ModSolAceptarEnvioObligatorio").live("click", function () {
        $("#MCIcono").attr("class", "cargando");
        $("#MCContenedor").html("Modificando la solicitud, por favor espere un momento...");
        $("#ModalCotizando").dialog({ title: "Solicitud" });
        $("#ModalCotizando").dialog("open");

        for (i = 0; i < $('#TabCotizaciones tbody tr').length; i++) {
            if ($('#TabCotizaciones tbody tr:eq(' + i + ') input').is(':checked')) {
                Solicitud.Cotizaciones[i].IndEnvioObligatorio = true;
            }
            else {
                Solicitud.Cotizaciones[i].IndEnvioObligatorio = false;
            }
        }

        // Guardar los datos de la solicitud
        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            idSolicitud: $("#HNumSolicitud").val(),
            cotizaciones: Solicitud.Cotizaciones
        }

        $.ajax({
            type: "POST",
            url: "CotizadorOficiales.aspx/ModificarSolicitudEnvioObligatorio",
            contentType: "application/json; charset=iso-8859-1",
            dataType: "json",
            data: $.toJSON(params),
            success: function (data) {
                if (data.d.Respuesta.Estado == "OK") {
                    // Cerrar modal de espera
                    $('#ModalCotizando').dialog('close');

                    // Mostrar mensaje de éxito
                    $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                    $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                    $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                    $('#ModalCuadroMensaje').dialog('open');

                }
                else if (data.d.Estado == 'TOKEN') {
                    CerrarSesionExpirada();
                }
                else {
                    $('#ModalCotizando').dialog('close');

                    $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
                    $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
                    $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
                    //if (data.d.Respuesta.Controles != null && data.d.Respuesta.Controles > 0) {

                    //    if (data.d.Respuesta.Controles[0].length) $('#ModSolACOM').attr('class', data.d.Respuesta.Controles[0]);
                    //    if (data.d.Respuesta.Controles[1].length) $('#ModSolDCOM').attr('class', data.d.Respuesta.Controles[1]);
                    //    if (data.d.Respuesta.Controles[2].length) $('#ConModSolIndSeleccionado').attr('class', data.d.Respuesta.Controles[2]);
                    //    if (data.d.Respuesta.Controles[3].length) $('#ModSolValMontoAcomAgente').attr('class', data.d.Respuesta.Controles[3]);

                    //    if (data.d.Respuesta.Controles.length > 13) {
                    //        var celdaError;
                    //        for (i = 13; i < data.d.Respuesta.Controles.length; i++) {
                    //            celdaError = data.d.Respuesta.Controles[i].split(',');
                    //            $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                    //        }
                    //    }
                    //}
                    $('#ModalCuadroMensaje').dialog('open');
                }

                $('#ModSolCargando').fadeOut();
                $('#ModSolAceptarEnvioObligatorio').attr('class', 'boton darkblue sharp');
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $("#ModalCotizando").dialog("close");

                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("Ha ocurrido un error al guardar los datos de la solicitud.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                }
            }
        });

    });

});

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
    $('#ModBusAfiApellidoPaterno').removeClass('formTextboxError');
    $('#ModBusAfiApellidoMaterno').removeClass('formTextboxError');
    $('#ModBusAfiNombres').removeClass('formTextboxError');

    $('#ModBusAfiApellidoPaterno').val('');
    $('#ModBusAfiApellidoMaterno').val('');
    $('#ModBusAfiNombres').val('');

    $('#ModBusAfiBuscar').attr('class', 'boton darkblue sharp');

    $('#ModBusAfiCargando').hide();
    $('#ModBusAfiTablaAfiliados').hide();
    $('#TablaAfiliadosError').hide();

    $('#TabAfiliadosIndicePagina').val('1');
    $('#TabAfiliadosTamanhoPagina').val('10');
    $('#TabAfiliadosColumnaOrdenar').val('1');
    $('#TabAfiliadosDireccionOrdenar').val('A');
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
    $('#ModGruFamApellidoPaterno').removeClass('formTextboxError');
    $('#ModGruFamApellidoMaterno').removeClass('formTextboxError');
    $('#ModGruFamNombres').removeClass('formTextboxError');
    $('#ConModGruFamTipoIdentificacion').removeClass('formComboboxErrorContenedor');
    $('#ModGruFamNumeroIdentificacion').removeClass('formTextboxError');
    $('#ConModGruFamParentesco').removeClass('formComboboxErrorContenedor formComboboxReadOnlyContenedor');
    $('#ConModGruFamSexo').removeClass('formComboboxErrorContenedor formComboboxReadOnlyContenedor');
    $('#ModGruFamFechaNacimiento').removeClass('formTextboxError formCalendarError');
    $('#ConModGruFamIndInvalidez').removeClass('formComboboxErrorContenedor');
    $('#ConModGruFamTipoInvalidez').removeClass('formComboboxErrorContenedor');
    $('#ModGruFamFechaInvalidez').removeClass('formTextboxError formCalendarError');

    //$('#ModGruFamApellidoPaternoAprdo').removeClass('formTextboxError');
    //$('#ModGruFamApellidoMaternoAprdo').removeClass('formTextboxError');
    //$('#ModGruFamNombresAprdo').removeClass('formTextboxError');
    //$('#ConModGruFamTipoIdentificacionAprdo').removeClass('formComboboxErrorContenedor');
    //$('#ModGruFamNumeroIdentificacionAprdo').removeClass('formTextboxError');
    //$('#ConModGruFamSexoAprdo').removeClass('formComboboxErrorContenedor formComboboxReadOnlyContenedor');
    //$('#ModGruFamFechaNacimientoAprdo').removeClass('formTextboxError formCalendarError');

    $('#ModGruFamApellidoPaterno').val('');
    $('#ModGruFamApellidoMaterno').val('');
    $('#ModGruFamNombres').val('');
    $('#ModGruFamTipoIdentificacion').val('0');
    $('#TexModGruFamTipoIdentificacion').html($('#ModGruFamTipoIdentificacion').find(':selected').text());
    $('#ModGruFamNumeroIdentificacion').val('');
    $('#ModGruFamParentesco').removeAttr('disabled');
    $('#ModGruFamParentesco').val('0');
    $('#TexModGruFamParentesco').html($('#ModGruFamParentesco').find(':selected').text());
    $('#ModGruFamSexo').removeAttr('disabled');
    $('#ModGruFamSexo').val('0');
    $('#TexModGruFamSexo').html($('#ModGruFamSexo').find(':selected').text());
    $('#ModGruFamFechaNacimiento').val('');
    $('#ModGruFamIndInvalidez').val('0');
    $('#TexModGruFamIndInvalidez').html($('#ModGruFamIndInvalidez').find(':selected').text());
    $('#ModGruFamTipoInvalidez').val('0');
    $('#TexModGruFamTipoInvalidez').html($('#ModGruFamTipoInvalidez').find(':selected').text());
    $('#ModGruFamTipoInvalidez').attr('disabled', 'disabled');
    $('#ConModGruFamTipoInvalidez').addClass('formComboboxReadOnlyContenedor');
    $('#ModGruFamFechaInvalidez').val('');
    $('#ModGruFamFechaInvalidez').attr('disabled', 'disabled');
    $('#ModGruFamFechaInvalidez').addClass('formCalendarReadOnly');

    $('#ModGruFamApellidoPaternoAprdo').val('');
    $('#ModGruFamApellidoMaternoAprdo').val('');
    $('#ModGruFamNombresAprdo').val('');
    $('#ModGruFamTipoIdentificacionAprdo').val('0');
    $('#TexModGruFamTipoIdentificacionAprdo').html($('#ModGruFamTipoIdentificacion').find(':selected').text());
    $('#ModGruFamNumeroIdentificacionAprdo').val('');
    $('#ModGruFamSexoAprdo').removeAttr('disabled');
    $('#ModGruFamSexoAprdo').val('0');
    $('#TexModGruFamSexoAprdo').html($('#ModGruFamSexo').find(':selected').text());
    $('#ModGruFamFechaNacimientoAprdo').val('');

}

function LimpiarFormularioSolicitud() {
    $('#ModSolNroSolicitud').removeClass('formTextboxError');
    $('#ModSolTipoCambio').removeClass('formTextboxError');
    $('#ConModSolTipoPension').removeClass('formComboboxErrorContenedor');
    $('#ConModSolCategoria').removeClass('formComboboxErrorContenedor');
    $('#ModSolFechaDevengue').removeClass('formTextboxError formCalendarError');
    $('#ModSolFecUltActualizacion').removeClass('formTextboxError formCalendarError');
    $('#ModSolFechaRecepcion').removeClass('formTextboxError formCalendarError');
    $('#ModSolFechaPlazoAFP').removeClass('formTextboxError formCalendarError');
    $('#ModSolSaldoCIC').removeClass('formTextboxError');
    $('#ConModSolFactorTasa').removeClass('formComboboxErrorContenedor');
    $('#ModSolFechaCotizacion').removeClass('formTextboxError formCalendarError');
    $('#ModSolFechaSolicitudPension').removeClass('formTextboxError formCalendarError');
    $('#ModSolACOM').removeClass('formTextboxError');
    $('#ModSolDCOM').removeClass('formTextboxError');

    $('#ModSolNroSolicitud').val('');
    $('#ModSolTipoCambio').val('');
    $('#ModSolTipoPension').val('V');
    $('#TexModSolTipoPension').html($('#ModSolTipoPension').find(':selected').text());
    $('#ModSolCategoria').val($('#HCategoria').val());
    $('#TexModSolCategoria').html($('#ModSolCategoria').find(':selected').text());
    $('#ModSolFechaDevengue').val('');
    $('#ModSolFecUltActualizacion').val('');
    $('#ModSolFechaRecepcion').val('');
    $('#ModSolFechaPlazoAFP').val('');
    $('#ModSolSaldoCIC').val($('#SaldoCIC').val());
    $('#ModSolFactorTasa').val('0');
    $('#TexModSolFactorTasa').html($('#ModSolFactorTasa').find(':selected').text());
    $('#ModSolFechaCotizacion').val('');
    $('#ModSolFechaSolicitudPension').val('');
    $('#ModSolACOM').val('');
    $('#ModSolDCOM').val('');

    botonModSolAceptarBloqueado = false;
    $('#ModSolAceptar').attr('class', 'boton darkblue sharp');

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

    //<INIGTI_4081>
    $('#ModSolCalcularPBS').attr('class', 'boton darkblue sharp');
    //<FINGTI_4081>
    //$('#TablaCotizacionesCargando').hide();
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

    //<INIGTI_4081_3>//DECIMAL
    if (x.length > 1) {
        x[1] = x[1].length == 1 ? x[1].toString() + '0' : x[1]
    };
    //<FINGTI_4081_3>

    x2 = x.length > 1 ? '.' + x[1] : '.00';
    //var rgx = /(\d+)(\d{3})/;
    //while (rgx.test(x1)) {
    //    x1 = x1.replace(rgx, '$1' + '' + '$2');
    //}
    return x1 + x2;
}

//<SRI.INI-20322_E2>
function LimpiarFormularioSolicitudOficial() {
    $('#ModSolACOM').removeClass('formTextboxError');
    $('#ModSolDCOM').removeClass('formTextboxError');
    $('#ConModSolIndSeleccionado').removeClass('formComboboxErrorContenedor');
    $('#ModSolValMontoAcomAgente').removeClass('formTextboxError');

    $('#ModSolNroSolicitud').html('');
    $('#ModSolNroMeller').html('');
    $('#ModSolACOM').val('');
    $('#ModSolDCOM').val('');
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


    //<INIGTI_4081>

    ////$('#ModSolACOM').html('');
    ////$('#ModSolDCOM').html('');
    ////$('#ModSolIndSeleccionado').html('N');
    ////$('#ModSolValMontoAcomAgente').html('');
    ////$('#ModSolCategoria').html('0');
    //<FINGTI_4081>

    //<SOLINI25781>
    //<INIGTI_4081>//SE COMENTA
    ////$("#ModSolTRA").val('');
    //<FINGTI_4081>
    //<SOLFIN25781>

    botonModSolAceptarBloqueado = false;
    $('#ModSolAceptarOficial').attr('class', 'boton darkblue sharp');

    //<GTI.INI-29372>
    $('#ModSolAceptarEnvioObligatorio').hide();
    //<GTI.FIN-29372>

    $('#TablaCotizacionesCargando').hide();

}
//<SRI.FIN-20322_E2>

//<SRI.INI-20322_E2>
async function RegistrarTipoMovimientoBandeja(rechazo) {
    // No se usa
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

    //<INIGTI_7012>
    var esCorrecto = true;
    var errores = new Array();

    $("#ModSolACOM").removeClass("formTextboxError");
    $("#ModSolDCOM").removeClass("formTextboxError");


    $("#ModSolValMontoAcomAgente").removeClass("formTextboxError");

    // ACOM
    var vacom = true;
    if ($.trim($("#ModSolACOM").val()).length == 0) {
        errores.push("Ingrese el campo <strong>Porcentaje A</strong>. Dato Obligatorio.");
        vacom = false;
    }

    // DCOM
    var vdcom = true;
    if ($.trim($("#ModSolDCOM").val()).length == 0) {
        errores.push("Ingrese el campo <strong>Porcentaje D</strong>. Dato Obligatorio.");
        vdcom = false;
    }

    // Seleccionar Cotización
    var idSolicitudElegida = 0;

    if ($("#TabCotizaciones input[type=radio]:checked").length > 0) {
        idSolicitudElegida = $("#TabCotizaciones input[type=radio]:checked").val();
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
        } else {
            if ($("#ModSolACOM").val() > 0 && $("#ModSolIndSeleccionado").val() == "S") {
                if (idSolicitudElegida == 0) {
                    errores.push("Asegúrese de haber seleccionado una modalidad para que el sistema pueda calcular el monto.");
                    vmontoacom = false;
                }
            }
        }
    }

    // Clases de controles
    if (!vacom) $("#ModSolACOM").addClass("formTextboxError");
    if (!vdcom) $("#ModSolDCOM").addClass("formTextboxError");
    if (!vmontoacom) $("#ModSolValMontoAcomAgente").addClass("formTextboxError");

    esCorrecto = vacom & vdcom & vmontoacom;//<INIGTI_4081>//

    if (!esCorrecto) {
        $('#MCMIcono').attr('class', 'validacion');
        $('#MCMContenedor').html(formatearError(errores));
        $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
        $('#ModalCuadroMensaje').dialog('open');
        return false;
    }
    //<FINGTI_7012>

    $('#MCIcono').attr('class', 'cargando');
    $('#MCContenedor').html('Generando Flujo Solicitud, por favor espere un momento...');
    $('#ModalCotizando').dialog({ title: 'Registrando' });
    $('#ModalCotizando').dialog('open');

    //<INIGTI_4081>
    if ($('#HGrupoEspeciales').val() == "TRUE") {
        for (var n = 0; n < ParametroEspecial.length; n++) {
            ParametroEspecial[n].FecIniRangoStr = new Date(+ParametroEspecialNew[n].FecIniRango.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy");
            ParametroEspecial[n].FecFinRangoStr = new Date(+ParametroEspecialNew[n].FecFinRango.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy");
            ParametroEspecial[n].FecIniRango = null;
            ParametroEspecial[n].FecFinRango = null;
        };
    } else {
        ParametroEspecial = null;
        modifica = false;
    }

    var params = {
        tokenUsuario: $("#TokenUsuario").val(),
        parametros: ParametroEspecial,
        modifica: modificaEspecial
    }

    $.ajax({
        type: "POST",
        url: "BandejaFlujoCotizacionModificar.aspx/RegistrarCotizaValPar",
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {

                // Validar si el usuario excede sus parámetros permitidos de ACOM y TRA
                var params = {
                    idSolicitud: $("#HNumSolicitud").val(),
                    //tokenUsuario: $("#TokenUsuario").val(),
                    fechaCotizacion: new Date(Solicitud.FechaCotizacion).toString("dd/MM/yyyy"),
                    //<INIGTI_4081>
                    //acom: $("#ModSolACOM").val(),
                    acom: $("#ModSolACOM").val(),
                    //<FINGTI_4081>
                    cotizaciones: Solicitud.Cotizaciones,
                    rechazo: rechazo//<INIGTI_4081>
                }

                fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/validar-acom-tra', {
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
                                throw new Error("Sesión expirada");
                            }
                            throw new Error("Error en la respuesta del servidor");
                        }
                        return response.json();
                    })
                    .then(data => {

                        if (data.data.Estado == "OK") {
                            if (data.data.Contenido == "") {
                                // Los rangos están dentro de lo permitido por el usuario, proceder.
                                var params2 = {
                                    //<INIGTI_4081>
                                    acom: $("#ModSolACOM").val(),
                                    montoAcom: $("#ModSolValMontoAcomAgente").val(),
                                    //<INIGTI_4081>
                                    cotizaciones: Solicitud.Cotizaciones,
                                    numSolicitud: Solicitud.Id,
                                    fechaCotizacion: Solicitud.FechaCotizacion,
                                    rechazo: rechazo,
                                    envioCorreo: true,//<INIGTI_4081>
                                    dcom: $("#ModSolDCOM").val(),
                                }

                                console.log({
                                    params2
                                })

                                ApiSolicitudesCambio.RegistrarCotizacionMovimiento(params2.numSolicitud, params2.fechaCotizacion, params2.envioCorreo, params2.rechazo, params2.cotizaciones, params2.acom, params2.dcom, params2.montoAcom)
                                    .then(data => {
                                        if (data.Estado == 'OK') {
                                            $('#ModalCotizando').dialog('close');

                                            $('#ModalCuadroMensaje').dialog({ title: data.Titulo });
                                            $('#MCMIcono').attr('class', data.Icono);
                                            $('#MCMContenedor').html(data.Mensaje);
                                            $('#ModalCuadroMensaje').dialog('open');

                                            //<INIGTI_4081>
                                            if (rechazo) {
                                                setTimeout(() => {
                                                    window.location.href = "BandejaFlujoCotizacion.aspx";
                                                }, 6000);
                                            } else {
                                                GrillaFlujoMovilSinLimpiar(Solicitud.Afiliado.CUSPP, Solicitud.Id, SolicitudEscenario.NumOperacion);

                                                if ($('#HGrupoEspeciales').val() == "TRUE") {
                                                    for (i = 0; i < $('#TabTasaMaximaTraMinima tbody tr').length; i++) {
                                                        $('#TabTasaMaximaTraMinima tbody tr:eq(' + i + ') input[type=text]').attr("disabled", "true");
                                                    }
                                                }
                                            }
                                            //<FINGTI_4081>

                                            botonModSolAceptarBloqueado = true;
                                            $('#ModSolRechazarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');
                                            $('#ModSolAprobarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');

                                            //<INIGTI_4081>
                                            $('#ModSolCalcularPBSBandeja').hide();
                                            $('#ModSolGrabarPBSBandeja').hide();
                                            //<FINGTI_4081>
                                        } else if (data.Estado == "TOKEN") {
                                            CerrarSesionExpirada();
                                            throw new Error("Sesión token expirada");
                                        } else if (data.Estado == "ERROR_TASA") {
                                            $('#ModalCotizando').dialog('close');

                                            $('#ModalCuadroMensaje').dialog({ title: "Error" });
                                            $('#MCMIcono').attr('class', "error");
                                            $('#MCMContenedor').html("Se ha producido un error al cotizar la solicitud, por favor revise.");
                                            $('#ModalCuadroMensaje').dialog('open');

                                            location.reload();
                                        } else {
                                            $('#ModalCotizando').dialog('close');

                                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                            $('#MCMIcono').attr('class', data.d.Icono);
                                            $('#MCMContenedor').html(data.d.Mensaje);
                                            $('#ModalCuadroMensaje').dialog('open');
                                        }
                                    }).catch(error => {
                                        console.log('error aprobando solicitud: ', error)
                                        if (error.message !== "Validación fallida") {
                                            $('#ModalCotizando').dialog('close');
                                            $('#MCMIcono').attr('class', 'error');
                                            $('#MCMContenedor').html('Ha ocurrido un error al procesar la solicitud.');
                                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                            $('#ModalCuadroMensaje').dialog('open');
                                        }
                                    })
                            } else {
                                // Los rangos NO están dentro de lo permitido por el usuario
                                $("#ModalCotizando").dialog("close");
                                $("#MCAIcono").attr("class", "advertencia");
                                $("#MCAContenedor").html(data.data.Contenido);
                                $("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
                                $("#ModalCuadroAdvertencia").dialog("open");
                                //<FINGTI_4081>
                                throw new Error("Validación fallida");
                            }
                        } else if (data.data.Estado == "TOKEN") {
                            CerrarSesionExpirada();
                            throw new Error("Sesión token expirada");
                        } else if (data.data.Estado == "ERROR") {
                            // Cerrar modal de espera
                            $('#ModalCotizando').dialog('close');
                            // Mostrar mensaje de error
                            $('#MCMIcono').attr('class', data.data.Icono);
                            $('#MCMContenedor').html(data.data.Mensaje);
                            $('#ModalCuadroMensaje').dialog({ title: data.data.Titulo });
                            $('#ModalCuadroMensaje').dialog('open');
                            throw new Error("Error en la respuesta");
                        }
                    })
            }
            else if (data.d.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
            else if (data.d.Estado == "ERROR") {
                // Mostrar mensaje de error
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
                $("#ModalCotizando").dialog("close");

                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al guardar los datos de la solicitud.");
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            }
        },
        complete: function () {

        }
    });
}

$("#MCAOAceptarBandeja").live("click", function () {


    $("#ModalCuadroAdvertencia").dialog("close");


    $('#MCIcono').attr('class', 'cargando');
    $('#MCContenedor').html('Generando Flujo Solicitud, por favor espere un momento...');
    $('#ModalCotizando').dialog({ title: 'Registrando' });
    $('#ModalCotizando').dialog('open');

    ApiSolicitudesCambio.RegistrarCotizacionMovimiento(Solicitud.Id, Solicitud.FechaCotizacion, true, false, Solicitud.Cotizaciones, $("#ModSolACOM").val(), $("#ModSolDCOM").val(), $("#ModSolValMontoAcomAgente").val())
        .then(data => {
            console.log('data aprobando solicitud: ', data)
            if (data.Estado == 'OK') {

                $('#ModalCotizando').dialog('close');

                $('#ModalCuadroMensaje').dialog({ title: data.Titulo });
                $('#MCMIcono').attr('class', data.Icono);
                $('#MCMContenedor').html(data.Mensaje);
                $('#ModalCuadroMensaje').dialog('open');

                GrillaFlujoMovilSinLimpiar(Solicitud.Afiliado.CUSPP, Solicitud.Id, SolicitudEscenario.NumOperacion);

                botonModSolAceptarBloqueado = true;
                $('#ModSolRechazarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');
                $('#ModSolAprobarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');

                //<INIGTI_4081>
                $('#ModSolCalcularPBSBandeja').hide();
                $('#ModSolGrabarPBSBandeja').hide();
                //<FINGTI_4081> 

                $('#ModalCotizando').dialog('close');
            }
            else if (data.Estado == "TOKEN") {
                CerrarSesionExpirada();
            }
            else if (data.Estado == "ERROR_TASA") {
                $('#ModalCotizando').dialog('close');

                $('#ModalCuadroMensaje').dialog({ title: "Error" });
                $('#MCMIcono').attr('class', "error");
                $('#MCMContenedor').html("Se ha producido un error al cotizar la solicitud, por favor revise.");
                $('#ModalCuadroMensaje').dialog('open');
                //CargarTablaCotizacionesOficialesBandejaMovil(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

                location.reload();
            }
            else {
                $('#ModalCotizando').dialog('close');

                $('#ModalCuadroMensaje').dialog({ title: data.Titulo });
                $('#MCMIcono').attr('class', data.Icono);
                $('#MCMContenedor').html(data.Mensaje);
                $('#ModalCuadroMensaje').dialog('open');
            }
            $('#ModalCotizando').dialog('close');

        }).catch(error => {
            console.log('error aprobando solicitud: ', error)
            $('#ModalCotizando').dialog('close');

            $('#MCMIcono').attr('class', 'error');
            $('#MCMContenedor').html('Ha ocurrido un error al registrar la información.');
            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
            $('#ModalCuadroMensaje').dialog('open');
        })
});

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

            $('#ModSolNroSolicitud').val(SolicitudEscenario.NumSolicitud);
            $('#ModSolNroMeller').val(SolicitudEscenario.NumOperacion);
            $('#ModSolACOM').val(formatearMonto2(SolicitudEscenario.PjeAumentoComision));
            $('#ModSolDCOM').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
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
            $('#ModSolDCOM').focus();


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

                    if (Solicitud.Agente.IdNivel == 0) {
                        $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Sin Nivel)');
                    } else {
                        $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Nivel:' + Solicitud.Agente.IdNivel + ')');
                    }

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
                            $('#ModSolDCOM').attr('readonly', true);
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

/* function GrillaEditarFlujoMovil(numCusspp, numSolicitud, numOperacion) {

    $('#ModSolModo').val('M');

    $('#ModSolCargando').show();

    LimpiarFormularioSolicitudOficial();

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

            $('#ModSolNroSolicitud').html(SolicitudEscenario.NumSolicitud);
            $('#ModSolNroMeller').html(SolicitudEscenario.NumOperacion);
            $('#ModSolACOM').val(formatearMonto2(SolicitudEscenario.PjeAumentoComision));

            $('#ModSolDCOM').val(formatearMonto2(SolicitudEscenario.CodPjeCesionComision));
            $('#ModSolIndSeleccionado').html(SolicitudEscenario.IndEstadoSeleccion);
            $('#ModSolIndSeleccionado').val(SolicitudEscenario.IndEstadoSeleccion);
            $('#ModSolValMontoAcomAgente').val(formatearMonto2(SolicitudEscenario.ValMtoAgenteAcom));
            $('#ModSolMontoCIC').html(formatearMonto(SolicitudEscenario.ValTotalCic));
            $('#ModFecCierre').html(new Date(+SolicitudEscenario.FecCierre.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            $('#ModSolCussp').html(SolicitudEscenario.Afiliado.CUSPP);
            $('#ModSolNomAfiliado').html(SolicitudEscenario.Afiliado.NombreEmpresa);
            $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id);
            $('#ModSolNomAgente').html(SolicitudEscenario.Agente.Nombre);

            $('#ModSolCargando').fadeOut();
            $('#ModSolACOM').focus();

            var params = {
                tokenUsuario: $('#TokenUsuario').val(),
                solicitud: SolicitudEscenario.NumSolicitud,
                acom: $("#ModSolACOM").val(),
                cotizacion: SolicitudEscenario.NumCotizacionElegida
            }
            $.ajax({
                type: "POST",
                url: rutaObtenerMontoACOM,
                contentType: "application/json; charset=iso-8859-1",
                data: $.toJSON(params),
                dataType: "json",
                success: function (data) {
                    $("#ModSolValMontoAcomAgente").autoNumeric("update", { vMax: data.d.Contenido, aDec: ".", aSep: "," });
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                        //document.location.reload(true);
                    }
                    else {
                        $("#MCMIcono").attr("class", "error");
                        $("#MCMContenedor").html("Ha ocurrido un error al cargar el Monto A.");
                        $("#ModalCuadroMensaje").dialog({ title: "Error" });
                        $("#ModalCuadroMensaje").dialog("open");
                    }
                },
                complete: function () {
                }
            });

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

                    if (Solicitud.Agente.IdNivel == 0) {
                        $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Sin Nivel)');
                    } else {
                        $('#ModSolNumAgente').html(SolicitudEscenario.Agente.Id + ' (Nivel:' + Solicitud.Agente.IdNivel + ')');
                    }

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
                        if (!permisoRechazarSolicitud) {
                            $('#ModSolRechazarBandeja').attr('class', 'botonDeshabilitado gris gris_sharp');
                        }
                    }

                    $('#ModSolIndSeleccionado').attr('readonly', true);
                    $('#ModSolValMontoAcomAgente').attr('readonly', false);

                    CargarTablaCotizacionesOficialesBandejaMovil(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

                    //sI HAY ACCESO, VISUALIZAR
                    if ($('#HGrupoEspeciales').val() == "TRUE") {
                        CargarTablaTraDefault(numSolicitud);

                        var fecCotizacion = new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy");
                        CargarTablaTasaMaximaTraMinima(numSolicitud, fecCotizacion);
                    }

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
} */

function GrillaFlujoMovilSinLimpiar(numCusspp, numSolicitud, numOperacion) {

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

            $('#ModSolCargando').fadeOut();

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

                    CargarTablaCotizacionesOficialesBandejaMovil(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

                    if (Solicitud.TipoMovimiento.Id == 0) {
                        $('#ModSolACOM').attr('class', 'formTextbox');
                        $('#ModSolACOM').attr('readonly', false);
                    } else {
                        $('#ModSolACOM').attr('class', 'formTextbox formTextboxReadOnly');
                        $('#ModSolACOM').attr('readonly', true);
                    }

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
}

function ImprimirFicha(tokenUsuario, numCuspp) {

    var params = {
        tokenUsuario: tokenUsuario,
        cuspp: numCuspp
    }
    $.ajax({
        type: 'POST',
        url: 'Cotizador.aspx/ImprimirFicha',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: $.toJSON(params),
        success: function (data) {
            if (data.d.Estado == "OK") {
                if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
                    window.location.href = "../Reportes/DetalleAfiliado.aspx";
                }
                else {
                    var w = 800;
                    var h = 600;
                    var left = (screen.width / 2) - (w / 2);
                    var top = (screen.height / 2) - (h / 2);
                    var nuevaVentana = window.open("../Reportes/DetalleAfiliado.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
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

}

function ObtenerPeriodoTemporal(codModalidad, objeto) {
    $.ajax({
        type: 'POST',
        url: 'Cotizador.aspx/ObtenerPeriodoTemporal',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: "{codModalidad:'" + codModalidad + "'}",
        success: function (data) {
            var objdata = data.d;
            objeto.empty();
            for (var n = 0; n < objdata.length; n++) {
                objeto[0].options[n] = new Option(objdata[n].Valor_1, objdata[n].Valor_1);
            };
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

$('#ChkSolicitudTodos').live('click', function () {
    $("input:checkbox").prop('checked', $(this).prop("checked"));

    var solicitudes = new Array();

    //for (i = 0; i < $("#TabSolicitudOficial tbody tr").length; i++) {
    //    if ($("#TabSolicitudOficial #TabBandejaSolicitudOficial tbody tr:eq(" + i + ") input").is(":checked")) {
    //        var cot = $("#TabSolicitudOficial #TabBandejaSolicitudOficial tbody tr:eq(" + i + ") input").data("solicitud");
    //        solicitudes.push(cot);
    //    }
    //}
    $("#TabSolicitudOficial #TabBandejaSolicitudOficial tbody tr input:checked").each(function (index, val) {
        //console.log($(this).data("solicitud"));
        solicitudes.push($(this).data("solicitud"));
    });

    var params = {
        solicitudes: solicitudes
    }

    $.ajax({
        type: "POST",
        url: "BandejaFlujoCotizacion.aspx/GuardandoCheck",
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                document.location.reload(true);
            }
            else {
                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al enviar el email.");
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            }
        },
        complete: function () {

        }
    });
});

$('#ChkSolicitud').live('click', function () {

    var solicitudes = new Array();

    $("#TabSolicitudOficial #TabBandejaSolicitudOficial tbody tr input:checked").each(function (index, val) {
        //console.log($(this).data("solicitud"));
        solicitudes.push($(this).data("solicitud"));
    });


    var params = {
        solicitudes: solicitudes
    }

    $.ajax({
        type: "POST",
        url: "BandejaFlujoCotizacion.aspx/GuardandoCheck",
        contentType: "application/json; charset=iso-8859-1",
        dataType: "json",
        data: $.toJSON(params),
        success: function (data) {

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {

            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                document.location.reload(true);
            }
            else {
                $("#MCMIcono").attr("class", "error");
                $("#MCMContenedor").html("Ha ocurrido un error al enviar el email.");
                $("#ModalCuadroMensaje").dialog({ title: "Error" });
                $("#ModalCuadroMensaje").dialog("open");
            }
        },
        complete: function () {

        }
    });

});

function RegistrarTipoMovimientoBandejaBloque(rechazo) {
    var mensaje = '';

    if (rechazo)
        mensaje = 'Rechazando la solicitud, por favor espere un momento...';
    else
        mensaje = 'Cotizando la solicitud, por favor espere un momento...';

    mensaje = mensaje + '<br>';
    mensaje = mensaje + '<label class="textoLibre"> <strong>No cierre ni actualice la página hasta que términe de ejecutar el proceso.</strong> <\label>';


    $('#MCIcono').attr('class', 'cargando');
    //$('#MCContenedor').html('Cotizando la solicitud, por favor espere un momento...');
    $('#MCContenedor').html(mensaje);
    $('#ModalCotizando').dialog({ title: 'Cotizando' });
    $('#ModalCotizando').dialog('open');
    //TabSolicitudOficial
    var solicitudes = new Array();

    for (i = 0; i < $("#TabSolicitudOficial tbody tr").length; i++) {
        if ($("#TabSolicitudOficial #TabBandejaSolicitudOficial tbody tr:eq(" + i + ") input").is(":checked")) {
            var cot = $("#TabSolicitudOficial #TabBandejaSolicitudOficial tbody tr:eq(" + i + ") input").data("solicitud");
            solicitudes.push(cot);
        }
    }

    var idSolicitudElegida = $("#TabBandejaSolicitudOficial input[type=checkbox]:checked").val();

    if (solicitudes.length == 0) {
        $('#ModalCotizando').dialog('close');
        $('#MCMIcono').attr('class', 'error');
        $('#MCMContenedor').html('Seleccione al menos una solicitud.');
        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        $('#ModalCuadroMensaje').dialog('open');
        return;
    };

    var params = {
        numSolicitudes: solicitudes,
        rechazo
    }

    ApiSolicitudesCambio.RegistrarCotizacionMovimientoBloque(params.numSolicitudes, params.rechazo)
        .then(data => {
            console.log(data)
            $('#ModalCotizando').dialog('close');
            //Solicitud = data.d;
            //alert(data.d.Mensaje);

            // Mostrar mensaje de éxito
            $('#MCMIcono').attr('class', data.Icono);
            $('#MCMContenedor').html(data.Mensaje);
            $('#ModalCuadroMensaje').dialog({ title: data.Titulo });
            $('#ModalCuadroMensaje').dialog('open');

            //window.location.href = 'BandejaFlujoCotizacion.aspx';
            // Actualizar temporizador
            selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
            $clock.countdown(selectedDate.toString());
        })
        .catch(error => {
            const response = error.response;
            console.log(response)
            if (response.status === 401 || response.status === 12030) {
                document.location.reload(true);
            }
            else {
                $('#MCMIcono').attr('class', 'error');
                $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                $('#ModalCuadroMensaje').dialog('open');
            }
            $('#ModalCotizando').dialog('close');
        })


    /*  $.ajax({
         type: 'POST',
         url: ApiSolicitudesCambio.GetUrlApi() + '/cotizacion-movimiento/bloque',
         contentType: "application/json; charset=iso-8859-1",
         data: $.toJSON(params),
         dataType: 'json',
         headers: {
             'X-Usuario': usuario.Matricula,
             'X-Rol': usuario.RolAzman
         },
         success: function (data) {
             
         },
         error: function (XMLHttpRequest, textStatus, errorThrown) {
             
         },
         beforeSend: function (xhr) {
             xhr.setRequestHeader('X-Usuario', usuario.Matricula);
             xhr.setRequestHeader('X-Rol', usuario.RolAzman);
         }
     }); */
}

const validarPermisoCWRV = (listaPermisos, permiso) => listaPermisos.some(o => o.IdAzman === permiso && o.Activa);