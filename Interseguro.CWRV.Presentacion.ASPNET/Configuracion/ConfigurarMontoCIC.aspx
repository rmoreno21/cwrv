<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="ConfigurarMontoCIC.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Configuracion.ConfigurarMontoCIC" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript">

        //$(function () {
        //    CargarListadoContrato();
        //});

        //window.onload = function () {
        //    CargarListadoContrato();
        //};

        $(document).ready(function () {
            CargarListadoContrato();

            var codigoCIC = 0;

            var permisoNuevoMonto = ($('#PerNuevoMonto').val() == '1' ? true : false);
            var permisoEditarMonto = ($('#PerEditarMonto').val() == '1' ? true : false);
            var permisoEliminarMonto = ($('#PerEliminarMonto').val() == '1' ? true : false);

            //<GTI.29372>
            var permisoNuevoContrato = ($('#PerNuevoContrato').val() == '1' ? true : false);
            var permisoEditarContrato = ($('#PerEditarContrato').val() == '1' ? true : false);
            var permisoEliminarContrato = ($('#PerEliminarContrato').val() == '1' ? true : false);
            //</GTI.29372>

            //<GTI.40661>
            var permisoGrabarLogCotizacion = ($('#PerGrabarLogCotizacion').val() == '1' ? true : false);
            var permisoGrabarLogReserva = ($('#PerGrabarLogReserva').val() == '1' ? true : false);
            //</GTI.40661>

            /*BEGIN: deshabilitar boton segun permiso*/
            if (!permisoNuevoMonto) {
                $('#NuevoMonto').attr('class', 'botonDeshabilitado gris gris_sharp');
            }

            if (!permisoEditarMonto) {
                $('#EditarMonto').attr('class', 'botonDeshabilitado gris gris_sharp');
            }

            if (!permisoEliminarMonto) {
                $('#EliminarMonto').attr('class', 'botonDeshabilitado gris gris_sharp');
            }

            //<GTI.29372>
            if (!permisoNuevoContrato) {
                $('#btnNuevoContrato').attr('class', 'botonDeshabilitado gris gris_sharp');
            }

            if (!permisoEditarContrato) {
                $('#btnEditarContrato').attr('class', 'botonDeshabilitado gris gris_sharp');
            }

            if (!permisoEliminarContrato) {
                $('#btnEliminarContrato').attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            //</GTI.29372>

            //<GTI.40661>
            if (!permisoGrabarLogCotizacion) {
                $('#btnGrabarLogCotizacion').attr('class', 'botonDeshabilitado gris gris_sharp');
            }

            if (!permisoGrabarLogReserva) {
                $('#btnGrabarLogReserva').attr('class', 'botonDeshabilitado gris gris_sharp');
            }
            //</GTI.40661>
            /*END: deshabilitar boton segun permiso*/


            $('.numerico').autoNumeric('init', { aSep: ',' });

            //$('.parent() #TextListaMontos').hide();

            $('#TexListaMontos').hide();

            /*BEGIN: Guardar*/

            $('#MCDGuardar').live('click', function () {

                var correcto = true;
                var errores = new Array();

                if ($.trim($('#Monto').val()).length == 0) {
                    $('#Monto').addClass('formTextboxError');
                    $("input[name^='defaultvalue-clone']:eq(0)").addClass('formTextboxError');
                    correcto = false;
                    errores.push("Debe ingresar un <b>monto</b><br />");
                }
                else {
                    if ($('#Monto').val() == 0) {
                        $('#Monto').addClass('formTextboxError');
                        $("input[name^='defaultvalue-clone']:eq(0)").addClass('formTextboxError');
                        correcto = false;
                        errores.push("El monto tiene que ser diferente de <b>cero</b><br />");
                    }
                }

                if (!correcto) {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html(formatearError(errores));
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return false;
                }

                var opcion = $('#ModoGrilla').val();

                if (opcion == 0) {

                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        valorCIC: $('#Monto').val()
                    };

                    $.ajax({
                        type: 'POST',
                        url: 'ConfigurarMontoCIC.aspx/RegistrarMontoCIC',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: $.toJSON(params),
                        success: function (data) {

                            if (data.d.Estado == 'OK') {
                                CargarListadoMonto();
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                $('#MCMIcono').attr('class', data.d.Icono);
                                $('#MCMContenedor').html(data.d.Mensaje);
                                $('#ModalCuadroMensaje').dialog('open');
                                $('#ModalEdicion').dialog('close');
                            }
                            else if (data.d.Estado == "ERROR") {
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                $('#MCMIcono').attr('class', data.d.Icono);
                                $('#MCMContenedor').html(data.d.Mensaje);
                                $('#ModalCuadroMensaje').dialog('open');

                            }
                            else if (data.d.Estado == "REPETIDO") {
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
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al registrar el monto.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#ModalCuadroMensaje').dialog('open');

                                //                            $('#TablaComisionCargando').hide();
                                //                            $('#TablaComisionError').show();
                            }
                        }
                    });
                }

                else if (opcion == 1) {
                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        codigo: codigoCIC,
                        valorCIC: $('#Monto').val()
                    };

                    $.ajax({
                        type: 'POST',
                        url: 'ConfigurarMontoCIC.aspx/ActualizarMontoCIC',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: $.toJSON(params),
                        success: function (data) {

                            if (data.d.Estado == 'OK') {
                                CargarListadoMonto();
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                $('#MCMIcono').attr('class', data.d.Icono);
                                $('#MCMContenedor').html(data.d.Mensaje);
                                $('#ModalCuadroMensaje').dialog('open');
                                $('#ModalEdicion').dialog('close');
                            }
                            else if (data.d.Estado == "ERROR") {
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                $('#MCMIcono').attr('class', data.d.Icono);
                                $('#MCMContenedor').html(data.d.Mensaje);
                                $('#ModalCuadroMensaje').dialog('open');

                            }
                            else if (data.d.Estado == "REPETIDO") {
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
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al actualizar el monto.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#ModalCuadroMensaje').dialog('open');

                                //                            $('#TablaComisionCargando').hide();
                                //                            $('#TablaComisionError').show();
                            }
                        }
                    });
                }


            });

            /*END: Guardar*/


            function CargarListadoMonto() {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val()
                };

                $.ajax({
                    type: 'POST',
                    url: 'ConfigurarMontoCIC.aspx/CargarListadoMonto',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {
                            $("#ListaMontos").html("");
                            $("#ListaMontos").html(data.d.Contenido);
                        }
                        else if (data.d.Estado == "ERROR") {
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
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al consultar los montos registrados.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    }
                });
            }


            /*BEGIN: Botón Nuevo Registro */
            $('#NuevoMonto').click(function () {
                if (permisoNuevoMonto) {
                    $('#ModalEdicion').dialog('open');
                    $('#Monto').val("");
                    $('#ModoGrilla').val(0);
                }
                else {
                    return false;
                }
            });
            /*END: Botón Nuevo Registro */

            $('#EditarMonto').click(function () {
                if (permisoEditarMonto) {
                    var usosSeleccionados = $('#ListaMontos option:selected').length;
                    if (usosSeleccionados == 0)
                        return false;
                    $('#ListaMontos option:selected').each(function () {
                        $('#Monto').val($(this).text());
                        codigoCIC = $(this).val();
                    });
                    $('#ModalEdicion').dialog('open');
                    $('#ModoGrilla').val(1);
                }
                else {
                    return false;
                }

            });

            //-----------------------------------------------------------
            //<SOLINI25621>
            $('#EditarPorcen').click(function () {
                if (permisoEditarMonto) {
                    $("#PensionPorcentaje").val($("#PensionPorcen").val());
                    $('#ModalEdicionPorcen').dialog('open');

                }
                else {
                    return false;
                }

            });


            /*BEGIN: Guardar Aumento Porcentaje*/

            $('#MCDGuardarPorcen').live('click', function () {

                var correcto = true;
                var errores = new Array();

                if ($.trim($('#PensionPorcentaje').val()).length == 0) {
                    $('#PensionPorcentaje').addClass('formTextboxError');
                    $("input[name^='defaultvalue-clone']:eq(0)").addClass('formTextboxError');
                    correcto = false;
                    errores.push("Debe ingresar un <b>% de aumento</b><br />");
                }
                //else {
                //    if ($('#PensionPorcentaje').val() == 0) {
                //        $('#PensionPorcentaje').addClass('formTextboxError');
                //        $("input[name^='defaultvalue-clone']:eq(0)").addClass('formTextboxError');
                //        correcto = false;
                //        errores.push("El % de aumento tiene que ser diferente de <b>cero</b><br />");
                //    }
                //}

                if (!correcto) {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html(formatearError(errores));
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return false;
                }

                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    valor1: $('#PensionPorcentaje').val()
                };

                $.ajax({
                    type: 'POST',
                    url: 'ConfigurarMontoCIC.aspx/ActualizarAumentoPorcentaje',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {
                            //CargarListadoMonto();
                            $('#PensionPorcen').val($('#PensionPorcentaje').val());
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                            $('#MCMIcono').attr('class', data.d.Icono);
                            $('#MCMContenedor').html(data.d.Mensaje);
                            $('#ModalCuadroMensaje').dialog('open');
                            $('#ModalEdicionPorcen').dialog('close');
                        }
                        else if (data.d.Estado == "ERROR") {
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
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al actualizar el monto.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    }
                });



            });

            /*END: Guardar*/


            /*BEGIN: Botón Cancelar del cuadro modal de edicion */
            $('#MCDCancelarPorcen').click(function () {
                $('#ModalEdicionPorcen').dialog('close');
            });
            /*END: Botón Cancelar del cuadro modal de edicion */

            //<SOLFIN25621>
            //------------------------------------------------------------


            //        $('#ListaMontos').find('option').each(function () {
            //            $(this).dblclick(function () {
            //                $('#ModalEdicion').dialog('open');
            //                $('#monto').val($(this).text());
            //                codigoCIC = $(this).val();
            //                $('#ModoGrilla').val(1);
            //            });
            //        });


            $('#EliminarMonto').click(function () {
                if (permisoEliminarMonto) {
                    var usosSeleccionados = $('#ListaMontos option:selected').length;
                    if (usosSeleccionados == 0)
                        return false;

                    $('#ListaMontos option:selected').each(function () {
                        codigoCIC = $(this).val();
                    });
                    $('#MCATablaEliminar').val('EliminarConfiguracion');
                    $('#MCAIcono').attr('class', 'advertencia');
                    $('#MCAContenedor').html('¿Está seguro de eliminar el registro?');
                    $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
                    $('#ModalCuadroAdvertencia').dialog('open');
                    return false;
                }
                else {
                    return false;
                }
            });


            function EliminarConfiguracion() {

                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    codigo: codigoCIC
                };

                $.ajax({
                    type: 'POST',
                    url: 'ConfigurarMontoCIC.aspx/EliminarMontoCIC',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {
                            CargarListadoMonto();
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                            $('#MCMIcono').attr('class', data.d.Icono);
                            $('#MCMContenedor').html(data.d.Mensaje);
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                        else if (data.d.Estado == "ERROR") {
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
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al eliminar el monto.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    }
                });
            }


            /*BEGIN: Botón Aceptar Cuadro Advertencia */
            $('#MCAAceptar').live('click', function () {
                /*acepta eliminar de grilla de mantenimiento*/
                if ($('#MCATablaEliminar').val() == 'EliminarConfiguracion') {
                    EliminarConfiguracion();
                }

                //<GTI.29372>
                if ($('#MCATablaEliminar').val() == 'EliminarContrato') {
                    EliminarContrato();
                }
                //</GTI.29372>
                $('#ModalCuadroAdvertencia').dialog('close');
            });
            /*END: Botón Aceptar Cuado Advertencia */


            $('#MCACancelar').live('click', function () {
                $('#ModalCuadroAdvertencia').dialog('close');
            });


            /*BEGIN: Botón Cancelar del cuadro modal de edicion */
            $('#MCDCancelar').click(function () {
                $('#ModalEdicion').dialog('close');
            });
            /*END: Botón Cancelar del cuadro modal de edicion */


            /*BEGIN: modales*/

            /*BEGIN: Modal de Edicion */
            $('#ModalEdicion').dialog({
                autoOpen: false,
                resizable: false,
                width: 280,
                minHeight: 150,
                show: "fade",
                hide: "fade",
                modal: true,
                title: 'Monto CIC',
                open: function (event, ui) {
                    $('#Monto').removeClass('formTextboxError');
                },
                close: function (event, ui) {
                }
            });
            /*END: Modal de Edicion */

            //<SOLINI25621>
            /*BEGIN: Modal de Edicion */
            $('#ModalEdicionPorcen').dialog({
                autoOpen: false,
                resizable: false,
                width: 280,
                minHeight: 150,
                show: "fade",
                hide: "fade",
                modal: true,
                title: 'Aumento Pensión %',
                open: function (event, ui) {
                    $('#Monto').removeClass('formTextboxError');
                },
                close: function (event, ui) {
                }
            });
            /*END: Modal de Edicion */
            //<SOLIFIN25621>

            //<GTI.29372>
            $('#ModalEdicionContrato').dialog({
                autoOpen: false,
                resizable: false,
                width: 360,
                minHeight: 250,
                show: "fade",
                hide: "fade",
                modal: true,
                title: 'Contrato',
                open: function (event, ui) {
                    $('#txtGlosaContrato').removeClass('formTextboxError');
                    $('#txtFecIniContrato').removeClass('formTextboxError');
                    $('#txtFecFinContrato').removeClass('formTextboxError');
                    $('#ddlAFPContrato').removeClass('formTextboxError');
                },
                close: function (event, ui) {
                }
            });
            //</GTI.29372>

            /*END: modales*/

            //<GTI.29372>
            $('#btnNuevoContrato').click(function () {
                if (permisoNuevoContrato) {
                    $("#txtIdContrato").val("");
                    $("#txtGlosaContrato").val("");
                    $("#txtFecIniContrato").val("");
                    $("#txtFecFinContrato").val("");

                    $('#ModalEdicionContrato').dialog('open');
                    $('#ModoContrato').val(0);
                }
                else {
                    return false;
                }
            });

            $('#btnEditarContrato').click(function () {
                if (permisoEditarContrato) {
                    for (i = 0; i < $('#TabContratos tbody tr').length; i++) {
                        if ($('#TabContratos tbody tr:eq(' + i + ') input').is(':checked')) {
                            $("#txtIdContrato").val($('#TabContratos tbody tr:eq(' + i + ')').find('td').eq(1).text().trim());
                            $("#txtGlosaContrato").val($('#TabContratos tbody tr:eq(' + i + ')').find('td').eq(2).text().trim());
                            $("#txtFecIniContrato").val($('#TabContratos tbody tr:eq(' + i + ')').find('td').eq(3).text().trim());
                            $("#txtFecFinContrato").val($('#TabContratos tbody tr:eq(' + i + ')').find('td').eq(4).text().trim());
                            var textoAFP = $('#TabContratos tbody tr:eq(' + i + ')').find('td').eq(5).text().trim();
                            $("#ddlAFPContrato option:contains('" + textoAFP + "')").prop('selected', 'selected');
                            $('#TexddlAFPContrato').html($('#ddlAFPContrato').find(':selected').text());
                        }
                    }

                    $('#ModalEdicionContrato').dialog('open');
                    $('#ModoContrato').val(1);
                }
                else {
                    return false;
                }
            });

            $('#btnEliminarContrato').click(function () {
                if (permisoEliminarContrato) {
                    $('#MCATablaEliminar').val('EliminarContrato');
                    $('#MCAIcono').attr('class', 'advertencia');
                    $('#MCAContenedor').html('¿Está seguro de eliminar el registro?');
                    $('#ModalCuadroAdvertencia').dialog({ title: 'Confirmación' });
                    $('#ModalCuadroAdvertencia').dialog('open');
                    return false;
                }
                else {
                    return false;
                }
            });

            $('#btnGrabarContrato').live('click', function () {
                var correcto = true;
                var errores = new Array();

                if ($.trim($('#txtGlosaContrato').val()).length == 0) {
                    $('#txtGlosaContrato').addClass('formTextboxError');
                    $("input[name^='defaultvalue-clone']:eq(0)").addClass('formTextboxError');
                    correcto = false;
                    errores.push("Debe ingresar la <b>glosa</b><br />");
                }

                if ($.trim($('#txtFecIniContrato').val()).length == 0) {
                    $('#txtFecIniContrato').addClass('formTextboxError');
                    $("input[name^='defaultvalue-clone']:eq(0)").addClass('formTextboxError');
                    correcto = false;
                    errores.push("Debe ingresar la <b>fecha de inicio de vigencia</b><br />");
                }

                if ($.trim($('#txtFecFinContrato').val()).length == 0) {
                    $('#txtFecFinContrato').addClass('formTextboxError');
                    $("input[name^='defaultvalue-clone']:eq(0)").addClass('formTextboxError');
                    correcto = false;
                    errores.push("Debe ingresar la <b>fecha de fin de vigencia</b><br />");
                }

                if ($('#ddlAFPContrato').val() == "0") {
                    $('#ddlAFPContrato').addClass('formTextboxError');
                    $("input[name^='defaultvalue-clone']:eq(0)").addClass('formTextboxError');
                    correcto = false;
                    errores.push("Debe seleccionar la <b>AFP</b><br />");
                }

                if (!correcto) {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html(formatearError(errores));
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                    return false;
                }

                var opcion = $('#ModoContrato').val();

                if (opcion == 0) {
                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        glsContrato: $('#txtGlosaContrato').val(),
                        fechaInicio: $('#txtFecIniContrato').val(),
                        fechaFin: $('#txtFecFinContrato').val(),
                        codAFP: $('#ddlAFPContrato').val()
                    };

                    $.ajax({
                        type: 'POST',
                        url: 'ConfigurarMontoCIC.aspx/RegistrarContratoCotizacion',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: $.toJSON(params),
                        success: function (data) {
                            if (data.d.Estado == 'OK') {
                                CargarListadoContrato();
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                $('#MCMIcono').attr('class', data.d.Icono);
                                $('#MCMContenedor').html(data.d.Mensaje);
                                $('#ModalCuadroMensaje').dialog('open');
                                $('#ModalEdicionContrato').dialog('close');
                            }
                            else if (data.d.Estado == "ERROR") {
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
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al registrar el contrato.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#ModalCuadroMensaje').dialog('open');
                            }
                        }
                    });
                }
                else if (opcion == 1) {
                    var params = {
                        tokenUsuario: $('#TokenUsuario').val(),
                        idContrato: $('#txtIdContrato').val(),
                        glsContrato: $('#txtGlosaContrato').val(),
                        fechaInicio: $('#txtFecIniContrato').val(),
                        fechaFin: $('#txtFecFinContrato').val(),
                        codAFP: $('#ddlAFPContrato').val()
                    };

                    $.ajax({
                        type: 'POST',
                        url: 'ConfigurarMontoCIC.aspx/ModificarContratoCotizacion',
                        contentType: "application/json; charset=iso-8859-1",
                        dataType: 'json',
                        data: $.toJSON(params),
                        success: function (data) {
                            if (data.d.Estado == 'OK') {
                                CargarListadoContrato();
                                $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                                $('#MCMIcono').attr('class', data.d.Icono);
                                $('#MCMContenedor').html(data.d.Mensaje);
                                $('#ModalCuadroMensaje').dialog('open');
                                $('#ModalEdicionContrato').dialog('close');
                            }
                            else if (data.d.Estado == "ERROR") {
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
                                $('#MCMIcono').attr('class', 'error');
                                $('#MCMContenedor').html('Ha ocurrido un error al registrar el contrato.');
                                $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                                $('#ModalCuadroMensaje').dialog('open');
                            }
                        }
                    });
                }
            });

            $('#btnCancelarContrato').click(function () {
                $('#ModalEdicionContrato').dialog('close');
            });

            function EliminarContrato() {
                var contrato = 0;
                $("#idContrato:checked").each(function (index, element) {
                    contrato = $(this).val();
                });

                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    idContrato: contrato,
                };

                $.ajax({
                    type: 'POST',
                    url: 'ConfigurarMontoCIC.aspx/EliminarContratoCotizacion',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {
                            CargarListadoContrato();
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                            $('#MCMIcono').attr('class', data.d.Icono);
                            $('#MCMContenedor').html(data.d.Mensaje);
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                        else if (data.d.Estado == "ERROR") {
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
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al eliminar el contrato.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    }
                });
            }

            function CargarListadoContrato() {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val()
                };

                $.ajax({
                    type: 'POST',
                    url: 'ConfigurarMontoCIC.aspx/CargarListadoContratos',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: $.toJSON(params),
                    success: function (data) {
                        $('#TablaContratosContenedor').html($(data.d));
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        
                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            document.location.reload(true);
                        }
                        else {
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al consultar los contratos registrados.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    }
                });
            }
            //</GTI.29372>

            //<GTI.40661>
            $('#btnGrabarLogCotizacion').live('click', function () {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    valor1: $('#ddlLogCotizacion').val()
                };

                $.ajax({
                    type: 'POST',
                    contentType: 'application/json',
                    dataType: 'json',
                    url: 'ConfigurarMontoCIC.aspx/GrabarLogCotizacion',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                            $('#MCMIcono').attr('class', data.d.Icono);
                            $('#MCMContenedor').html(data.d.Mensaje);
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                        else if (data.d.Estado == "ERROR") {
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                            $('#MCMIcono').attr('class', data.d.Icono);
                            $('#MCMContenedor').html(data.d.Mensaje);
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                        else if (data.d.Estado == "TOKEN") {
                            CerrarSesionExpirada();
                        }
                    },
                    error: function(XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            document.location.reload(true);
                        }
                        else {
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al registrar el contrato.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    }
                });
            });

            $('#btnGrabarLogReserva').live('click', function () {
                var params = {
                    tokenUsuario: $('#TokenUsuario').val(),
                    valor1: $('#ddlLogReserva').val()
                };

                $.ajax({
                    type: 'POST',
                    contentType: 'application/json',
                    dataType: 'json',
                    url: 'ConfigurarMontoCIC.aspx/GrabarLogReserva',
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {
                            $('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
                            $('#MCMIcono').attr('class', data.d.Icono);
                            $('#MCMContenedor').html(data.d.Mensaje);
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                        else if (data.d.Estado == "ERROR") {
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
                            $('#MCMIcono').attr('class', 'error');
                            $('#MCMContenedor').html('Ha ocurrido un error al registrar el contrato.');
                            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                            $('#ModalCuadroMensaje').dialog('open');
                        }
                    }
                });
            });
            //</GTI.40661>

        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div align="left" style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e;">
        <h1 class="simple" style="width: 230px">Parámetros para cotización</h1>
        <%--//Parámetros para cotización
        //Configuración de monto CIC--%>


        <asp:Panel ID="DatosMontoCIC" runat="server" ClientIDMode="Static" Visible="True">
            <br />
            <fieldset>
                <legend>Configuración de monto CIC</legend>
                <div class="grilla_info">
                    Los valores de esta lista son los que aparecerán por omisión para colocar como CIC cuando se intente
                    cotizar a un afiliado que no tiene consentimiento de asesoría.
                </div>
                <div align="center" style="background: #FFF;">
                    <asp:HyperLink ID="NuevoMonto" CssClass="boton darkblue sharp" Style="width: 100px; height: 22px; float: none;" runat="server" ClientIDMode="Static">Nuevo</asp:HyperLink>&nbsp;
                    <asp:HyperLink ID="EditarMonto" CssClass="boton darkblue sharp" Style="width: 100px; height: 22px; float: none;" runat="server" ClientIDMode="Static">Editar</asp:HyperLink>&nbsp;
                    <asp:HyperLink ID="EliminarMonto" CssClass="boton darkblue sharp" Style="width: 100px; height: 22px; float: none;" runat="server" ClientIDMode="Static">Eliminar</asp:HyperLink>
                    <asp:HiddenField ID="PerNuevoMonto" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="PerEditarMonto" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="PerEliminarMonto" runat="server" ClientIDMode="Static" />
                </div>
                <br />
                <div align="center" style="background: #FFF;">
                    <asp:ListBox ID="ListaMontos" Style="width: 40%; direction: rtl;" runat="server" SelectionMode="Single" Rows="10" ClientIDMode="Static" CssClass="formListbox"></asp:ListBox>
                </div>
            </fieldset>
        </asp:Panel>


        <%--<SOLINI25621>--%>
        <asp:Panel ID="DatosConfPension" runat="server" ClientIDMode="Static" Visible="True">
            <br />
            <fieldset>
                <legend>Configuración de % de Pensión</legend>

                <div class="grilla_info">
                    El valor porcentual de este parámetro se añade a la pensión que se ingresa al momento de calcular
                    el capital requerido, haciendo que este sea mayor.
                </div>

                <div class="formLinea">
                    <label id="LabPensionPorcen" for="PensionPorcen" class="formLabel formLabel2Izq">Aumento porcentual de Pensión:</label>
                    <asp:TextBox ID="PensionPorcen" runat="server" Style="margin-left: 15px; text-align: right" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static" Enabled="false"></asp:TextBox>&nbsp;
                    <label class="formLabel2Izq">%</label>
                </div>

                <p></p>
                <div class="formLinea" align="center">
                    <asp:HyperLink ID="EditarPorcen" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" ClientIDMode="Static">Editar</asp:HyperLink>
                </div>
                <br />
            </fieldset>
        </asp:Panel>
        <%--<SOLFIN25621>--%>

        <%--<GTI.29372>--%>
        <asp:Panel ID="pnlContrato" runat="server" ClientIDMode="Static" Visible="True">
            <br />
            <fieldset>
                <legend>Contratos</legend>
                <div class="grilla_info">
                    Información sobre contratos.
                </div>
                <br />
                <div align="center" style="background: #FFF;">
                    <asp:HyperLink ID="btnNuevoContrato" CssClass="boton darkblue sharp" Style="width: 100px; height: 22px; float: none;" runat="server" ClientIDMode="Static">Nuevo</asp:HyperLink>&nbsp;
                    <asp:HyperLink ID="btnEditarContrato" CssClass="boton darkblue sharp" Style="width: 100px; height: 22px; float: none;" runat="server" ClientIDMode="Static">Editar</asp:HyperLink>&nbsp;
                    <asp:HyperLink ID="btnEliminarContrato" CssClass="boton darkblue sharp" Style="width: 100px; height: 22px; float: none;" runat="server" ClientIDMode="Static">Eliminar</asp:HyperLink>
                    <asp:HiddenField ID="PerNuevoContrato" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="PerEditarContrato" runat="server" ClientIDMode="Static" />
                    <asp:HiddenField ID="PerEliminarContrato" runat="server" ClientIDMode="Static" />
                </div>
                <br />
                <div id="TablaContratosContenedor" align="center" style="background: #FFF;">
                    <%--<asp:GridView ID="TabContratos" runat="server" Width="100%" CellPadding="1" CellSpacing="1" GridLines="None"
                        ClientIDMode="Static" AutoGenerateColumns="False">
                        <AlternatingRowStyle CssClass="grilla_alt2" />
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <%# "<input id=\"idContrato\" type=\"radio\" name=\"idContrato\" value=\"" + Eval("IdContratoCotizacion") + "\" checked=\"checked\" />" %>
                                </ItemTemplate>
                                <ItemStyle Width="23px" />
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Nro. Contrato">
                                <ItemTemplate>
                                    <asp:Label ID="NroContrato" runat="server" Text='<%# Bind("IdContratoCotizacion") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Contrato">
                                <ItemTemplate>
                                    <asp:Label ID="GlsContrato" runat="server" Text='<%# Bind("GlsContratoCotizacion") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Fecha Inicio">
                                <ItemTemplate>
                                    <asp:Label ID="FecInicio" runat="server" Text='<%# Convert.ToDateTime(Eval("FecInicio")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Fecha Fin">
                                <ItemTemplate>
                                    <asp:Label ID="FecFin" runat="server" Text='<%# Convert.ToDateTime(Eval("FecFin")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="AFP">
                                <ItemTemplate>
                                    <asp:Label ID="GlsAfp" runat="server" Text='<%# Bind("GlsAfp") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="grilla_info">
                                No se ha encontrado ningún registro de contrato.
                            </div>
                        </EmptyDataTemplate>
                        <HeaderStyle CssClass="grilla_cabecera" />
                        <RowStyle CssClass="grilla_alt1" />
                    </asp:GridView>--%>
                </div>

                <div id="ContratosCargando" align="center" style="display: none">
                    <asp:Image ID="Loading" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                    <span class="texto">Cargando contratos, espere por favor...</span>
                </div>
                <div id="ContratosContenedor" style="display: none"></div>
                <div id="ContratosError" class="grilla_error" style="display: none">No se ha podido cargar la tabla de contratos. <a id="Retry">Intentar de nuevo</a>.</div>
                <br />
            </fieldset>
        </asp:Panel>
        <%--</GTI.29372>--%>

        <%--<GTI.40661>--%>
        <asp:Panel ID="pnlLogCotizacion" runat="server" ClientIDMode="Static" Visible="True">
            <br />
            <fieldset>
                <legend>Configuración del Log de Cotización</legend>

                <div class="grilla_info">
                    El valor de este parámetro indica si se genera o no el Log de Cotización.
                </div>

                <div class="formLinea">
                    <label id="lblLogCotizacion" for="ddlLogCotizacion" class="formLabel formLabel2Izq">Generar Log Cotización:</label>
                    <asp:DropDownList ID="ddlLogCotizacion" runat="server" ClientIDMode="Static" CssClass="formCombobox" Width="80px">
                        <asp:ListItem Value="S">Sí</asp:ListItem>
                        <asp:ListItem Value="N">No</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <p></p>
                <div class="formLinea" align="center">
                    <asp:HiddenField ID="PerGrabarLogCotizacion" runat="server" ClientIDMode="Static" />
                    <asp:HyperLink ID="btnGrabarLogCotizacion" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" ClientIDMode="Static">Grabar</asp:HyperLink>
                </div>
                <br />
            </fieldset>
        </asp:Panel>

        <asp:Panel ID="pnlLogReserva" runat="server" ClientIDMode="Static" Visible="True">
            <br />
            <fieldset>
                <legend>Configuración del Log de Reservas</legend>

                <div class="grilla_info">
                    El valor de este parámetro indica si se genera o no el Log de Reservas.
                </div>

                <div class="formLinea">
                    <label id="lblLogReserva" for="ddlLogReserva" class="formLabel formLabel2Izq">Generar Log Reserva:</label>
                    <asp:DropDownList ID="ddlLogReserva" runat="server" ClientIDMode="Static" CssClass="formCombobox" Width="80px">
                        <asp:ListItem Value="S">Sí</asp:ListItem>
                        <asp:ListItem Value="N">No</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <p></p>
                <div class="formLinea" align="center">
                    <asp:HiddenField ID="PerGrabarLogReserva" runat="server" ClientIDMode="Static" />
                    <asp:HyperLink ID="btnGrabarLogReserva" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" ClientIDMode="Static">Grabar</asp:HyperLink>
                </div>
                <br />
            </fieldset>
        </asp:Panel>
        <%--</GTI.40661>--%>

        <br />
        <%--Inicio Modal Edicion--%>
        <div id="ModalEdicion">
            <div class="formLinea">
                <label id="LabMonto" class="formLabel formLabel2Izq" style="width: 205px;">Monto:</label>
                <asp:TextBox ID="Monto" runat="server" CssClass="formTextbox numerico" Width="100" ClientIDMode="Static"></asp:TextBox>
            </div>

            <br />
            <div id="MCDBotonera" align="center">
                <asp:Button ID="MCDGuardar" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" Text="Aceptar" ClientIDMode="Static" /><%--Guardar</asp:Button>&nbsp;&nbsp;&nbsp;--%>
                <asp:Button ID="MCDCancelar" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" Text="Cancelar" ClientIDMode="Static" /><%--Cancelar</asp:Button>--%>
            </div>
        </div>
        <asp:HiddenField ID="ModoGrilla" runat="server" ClientIDMode="Static" />
        <%--Inicio Modal Edicion--%>

        <%--Inicio Modal Cuadro de mensajes--%>
        <div id="ModalCuadroMensaje">
            <div id="ModalCuadroMensajeCuerpo">
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCMIcono" style="width: 40px; height: 40px"></td>
                        <td id="MCMContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCMContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MCMEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCMEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCMEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCMMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="ModalCuadroMensajeCargando" align="center" style="display: none">
                <asp:Image ID="Image2" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Espere por favor...</span>
            </div>
            <div id="MCMBotonera" align="center">
                <a id="MCMAceptar" class="boton darkblue sharp" style="width: 80px;">Aceptar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de mensajes--%>

        <%--Inicio Modal Cuadro de advertencia--%>
        <div id="ModalCuadroAdvertencia">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCAIcono" style="width: 40px; height: 40px"></td>
                        <td id="MCAContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCAContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MCAEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCAEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCAEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCAMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MCABotonera" align="center">
                <a id="MCAAceptar" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MCACancelar" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>


        <%--<SOLINI25621>--%>
        <%--Inicio Modal Edicion--%>
        <div id="ModalEdicionPorcen">
            <div class="formLinea">
                <label id="LabPensionPorcentaje" for="PensionPorcentaje" class="formLabel formLabel2Izq" style="width: 210px;">Aumento Pensión:</label>
                <asp:TextBox ID="PensionPorcentaje" runat="server" CssClass="formTextbox numerico" Style="text-align: right" Width="100" ClientIDMode="Static"></asp:TextBox>&nbsp;
                <label class="formLabel2Izq">%</label>
            </div>

            <br />
            <div id="MCDBotoneraPorcen" align="center">
                <asp:Button ID="MCDGuardarPorcen" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" Text="Aceptar" ClientIDMode="Static" /><%--Guardar</asp:Button>&nbsp;&nbsp;&nbsp;--%>
                <asp:Button ID="MCDCancelarPorcen" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" Text="Cancelar" ClientIDMode="Static" /><%--Cancelar</asp:Button>--%>
            </div>
        </div>

        <%--Inicio Modal Edicion--%>
    </div>
    <%--<SOLFIN25621>--%>

    <%--Modal Edición Contrato--%>
    <%--<GTI.29372>--%>
    <div id="ModalEdicionContrato">
        <div class="formLinea">
            <label id="lblIdContrato" for="txtIdContrato" class="formLabel formLabel2Izq" style="width: 210px;">Id Contrato:</label>
            <asp:TextBox ID="txtIdContrato" runat="server" CssClass="formTextbox numerico" Width="100" ClientIDMode="Static" Enabled="false"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="lblGlosaContrato" for="txtGlosaContrato" class="formLabel formLabel2Izq" style="width: 210px;">Contrato:</label>
            <asp:TextBox ID="txtGlosaContrato" runat="server" CssClass="formTextbox" Width="200" ClientIDMode="Static" MaxLength="59"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="lblFecIniContrato" for="txtFecIniContrato" class="formLabel formLabel2Izq" style="width: 210px;">Fecha Inicio:</label>
            <asp:TextBox ID="txtFecIniContrato" runat="server" CssClass="fecha formTextbox formCalendar" Width="100" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="lblFecFinContrato" for="txtFecFinContrato" class="formLabel formLabel2Izq" style="width: 210px;">Fecha Fin:</label>
            <asp:TextBox ID="txtFecFinContrato" runat="server" CssClass="fecha formTextbox formCalendar" Width="100" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="LabddlAFPContrato" for="ddlAFPContrato" class="formLabel formLabel2Izq" style="width: 210px;">AFP:</label>
            <asp:DropDownList ID="ddlAFPContrato" runat="server" ClientIDMode="Static" CssClass="formCombobox" Width="160"></asp:DropDownList>
        </div>

        <br />
        <div id="MCDBotoneraContrato" align="center">
            <asp:Button ID="btnGrabarContrato" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" Text="Aceptar" ClientIDMode="Static" />
            <asp:Button ID="btnCancelarContrato" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px;" runat="server" Text="Cancelar" ClientIDMode="Static" />
        </div>
    </div>
    <asp:HiddenField ID="ModoContrato" runat="server" ClientIDMode="Static" />
    <%--</GTI.29372>--%>
</asp:Content>
