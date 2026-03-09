$(document).ready(function () {
    console.log("ready cierre");
});

function cargarBeneficiariosDireccion() {
    params = {
        tokenUsuario: $("#TokenUsuario").val(),
        numeroSolicitud: $("#NumeroSolicitud").val(),
        fecDevengue: fecDevengue,
        codTipoPension: codTipoPension
    };

    $.ajax({
        type: "POST",
        url: "CerrarSolicitud.aspx/CargarBeneficiariosDireccion",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSONstringifyConFechas(params),
        success: function (data) {

            var ul = $("#PasosCierre");
            $(ul).find('.li-direccion').remove();
            $(ul).find('.li-beneficiario').remove();

            $(data.d).find('.li-direccion').each(function () {
                var li = $(this)[0];
                ul.prepend(li.outerHTML);
            });

            $(data.d).find('.li-beneficiario').each(function () {
                var li = $(this)[0];
                ul.prepend(li.outerHTML);
            });

            inicializarStepper();

            actualizarCombobox();
            configurarMascaras();

            inicializarEventos();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            ManejarError(XMLHttpRequest, textStatus, errorThrown);
        },
        complete: function () {
            cerrarModalCargando();
        }
    });
}

function inicializarEventos() {
    $("#btnRegresar80").hide();

    $(".NumeroDocumento").change(function () {
        var parentesco = $(this).parent().parent().parent().find('.Parentesco').first().find(':selected').val();

        if (parentesco != "80") {
            var tipoDocumento = $(this).parent().parent().find('.TipoDocumento').first().find(':selected').val();
            var numeroDocumento = $(this).val();
            var correlativo = $(this).parent().parent().parent().find('.ModNumCorrelativo').first().val();

            if (tipoDocumento != null && tipoDocumento != "0") {
                if (numeroDocumento != "" && numeroDocumento != "0" && numeroDocumento.length > 1) {
                    buscarPersonaRviadm(tipoDocumento, numeroDocumento, solicitudConsultada, correlativo);
                }
            }
        }
    });

    $(".TipoDocumento").change(function () {
        var parentesco = $(this).parent().parent().parent().find('.Parentesco').first().find(':selected').val();

        if (parentesco != "80") {
            var tipoDocumento = $(this).val();
            var numeroDocumento = $(this).parent().parent().parent().find('.NumeroDocumento').first().val();
            var correlativo = $(this).parent().parent().parent().parent().find('.ModNumCorrelativo').first().val();

            if (tipoDocumento != null && tipoDocumento != "0") {
                if (numeroDocumento != "" && numeroDocumento != "0" && numeroDocumento.length > 1) {
                    buscarPersonaRviadm(tipoDocumento, numeroDocumento, solicitudConsultada, correlativo);
                }
            }
        }
    });

    if (codTipoPension == "S") {
        $("#divFechaFallecimiento").show();
    }
    else {
        $("#divFechaFallecimiento").hide();
    }

    $("#Departamento").change(function () {
        $("#Provincia").prop("disabled", true);
        $("#Provincia").val("0");
        $("#Distrito").prop("disabled", true);
        $("#Distrito").val("0");
        actualizarCombobox();

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            idDepartamento: $("#Departamento").val()
        };

        $.ajax({
            type: "POST",
            url: "CerrarSolicitud.aspx/ListarProvincias",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(params),
            success: function (data) {
                var provincias = data.d;
                PoblarCombobox("#Provincia", provincias);
                $("#Provincia").prop("disabled", false);
                $("#Provincia").val("0");
                actualizarCombobox();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ManejarError(XMLHttpRequest, textStatus, errorThrown);
            }
        });
    });

    $("#Provincia").change(function () {
        $("#Distrito").prop("disabled", true);
        $("#Distrito").val("0");
        actualizarCombobox();

        var params = {
            tokenUsuario: $("#TokenUsuario").val(),
            idProvincia: $("#Provincia").val()
        };

        $.ajax({
            type: "POST",
            url: "CerrarSolicitud.aspx/ListarDistritos",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify(params),
            success: function (data) {
                var distritos = data.d;
                PoblarCombobox("#Distrito", distritos);
                $("#Distrito").prop("disabled", false);
                $("#Distrito").val("0");
                actualizarCombobox();
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ManejarError(XMLHttpRequest, textStatus, errorThrown);
            }
        });
    });
}

function inicializarStepper() {
    var stepper = document.querySelector('.stepper');
    var stepperInstace = new MStepper(stepper, {
        // options
        firstActive: 0, // this is the default
        autoFocusInput: true,
        validationFunction: ValidarFormulario,
        stepTitleNavigation: false
    });
}

async function siguientePaso(destroyFeedback, form, activeStepContent) {
    console.log("siguientePaso");
    var entidadEdit = $(activeStepContent).find('#ModEntidad').val();
    var grabarOk = false;
    console.log("siguientePaso inicio bool:" + grabarOk);

    if (entidadEdit == "B") {
        //grabarOk = guardarBeneficiario(activeStepContent, entidadEdit);//versión actual
        grabarOk = await guardarBeneficiario(activeStepContent, entidadEdit);
    }

    if (entidadEdit == "D") {
        grabarOk = await guardarDireccion(activeStepContent, entidadEdit);
    }

    console.log("siguientePaso: " + grabarOk);
    if (grabarOk) {
        console.log("siguientePaso destroyFeedback");
        // Call destroyFeedback() function when you're done
        // The true parameter will proceed to the next step besides destroying the preloader
        destroyFeedback(true);
    }
    else {
        console.log("siguientePaso destroyFeedback false");
        cerrarModalCargando();
        destroyFeedback(false);
    }
}

function guardarBeneficiario(activeStepContent, entidadEdit) {
    console.log("guardarBeneficiario");

    return new Promise(function (resolve, reject) {
        var correlativo = $(activeStepContent).find(".ModNumCorrelativo").val();
        var grabarOk = false;
        console.log("guardarBeneficiario inicio bool:" + grabarOk);
        $("#Mensaje" + correlativo).hide();
        listaBeneficiarios.forEach(benefi => {
            if (benefi.numCorrelativo == correlativo) {

                benefi.ApellidoPaterno = $(activeStepContent).find(".ApellidoPaterno").val().trim().toUpperCase();
                benefi.ApellidoMaterno = $(activeStepContent).find(".ApellidoMaterno").val().trim().toUpperCase();
                benefi.Nombre = $(activeStepContent).find(".Nombres").val().trim().toUpperCase();
                benefi.Identificacion.Id = $(activeStepContent).find(".TipoDocumento").val();
                benefi.Identificacion.IdTipo = $(activeStepContent).find(".TipoDocumento").val();
                benefi.Identificacion.Glosa = $(activeStepContent).find(".TipoDocumento").find(':selected').text();
                benefi.Identificacion.Numero = $(activeStepContent).find(".NumeroDocumento").val().trim();
                
                if (codTipoPension == "S" && $(activeStepContent).find(".Parentesco").val() == "80") {
                    benefi.FechaFallecimiento = cadenaAFecha($(activeStepContent).find(".FechaFallecimiento").val());
                }
                else {
                    benefi.FechaFallecimiento = null;
                }

                benefi.Nacionalidad = {
                    cod_nacionalidad: $(activeStepContent).find(".Nacionalidad").val(),
                    gls_nacionalidad: $(activeStepContent).find(".Nacionalidad").find(':selected').text()
                };
                benefi.PaisOrigen = {
                    cod_parametro: $(activeStepContent).find(".PaisOrigen").val(),
                    gls_parametro: $(activeStepContent).find(".PaisOrigen").find(':selected').text()
                };
                benefi.numCelular = $(activeStepContent).find(".Celular").val().trim();
                benefi.CorreoElectronico = $(activeStepContent).find(".CorreoElectronico").val().trim();
                benefi.VinculoFamiliar = {
                    cod_parametro: $(activeStepContent).find(".VinculoFamiliar").val(),
                    gls_parametro: $(activeStepContent).find(".VinculoFamiliar").find(':selected').text()
                };
                benefi.NroVinculoFamiliar = $(activeStepContent).find(".NumeroVinculoFamiliar").val().trim();
                benefi.Afiliado = null;
                benefi.firmaDigital = null;

                benefi.DescuentoESSALUD = {
                    cod_parametro: $(activeStepContent).find(".ESSALUD").val(),
                    gls_parametro: $(activeStepContent).find(".ESSALUD").find(':selected').text()
                };

                //grabarOk = grabarBeneficiariosDireccion(benefi, entidadEdit, correlativo);//versión actual

                return grabarBeneficiariosDireccion(benefi, entidadEdit, correlativo).then(function (result) {
                    console.log(result);
                    console.log("grabarBeneficiariosDireccion resultado promise:" + result);
                    grabarOk = result;
                    resolve(result);
                });
            }
        });
    });
}

function guardarDireccion(activeStepContent, entidadEdit) {
    var celular = listaBeneficiarios.find(x => x.Parentesco.Id == "80").numCelular;
    var direccion = listaBeneficiarios.find(x => x.Parentesco.Id == "80").direccionPrincipal;
    direccion.numSolicitud = $("#NumeroSolicitud").val();
    direccion.idDireccion = $(activeStepContent).find("#ModIdDireccion").val();
    direccion.tipo = $(activeStepContent).find("#ModDirPrincipal").val();
    direccion.direccion = $(activeStepContent).find("#Direccion").val();
    direccion.espacioUrbano = $(activeStepContent).find("#EspacioUrbano").val();
    direccion.tipoVia = {
        Id: $(activeStepContent).find("#TipoVia").val(),
        Glosa: $(activeStepContent).find("#TipoVia").find(':selected').text()
    };
    direccion.nombreVia = $(activeStepContent).find("#NombreVia").val();
    direccion.numeroVia = $(activeStepContent).find("#NumeroVia").val();
    direccion.numeroInterior = $(activeStepContent).find("#NumeroInterior").val();
    direccion.tipoZona = {
        Id: $(activeStepContent).find("#TipoZona").val(),
        Glosa: $(activeStepContent).find("#TipoZona").find(':selected').text()
    };
    direccion.nombreZona = $(activeStepContent).find("#NombreZona").val();
    direccion.referencia = $(activeStepContent).find("#Referencia").val();
    direccion.numeroDepartamento = $(activeStepContent).find("#NumeroDepartamento").val();
    direccion.manzana = $(activeStepContent).find("#Manzana").val();
    direccion.numeroLote = $(activeStepContent).find("#NumeroLote").val();
    direccion.kilometro = $(activeStepContent).find("#Kilometro").val();
    direccion.block = $(activeStepContent).find("#Block").val();
    direccion.etapa = $(activeStepContent).find("#Etapa").val();
    direccion.largaDistancia = {
        Id: $(activeStepContent).find("#LargaDistancia").val(),
        Glosa: $(activeStepContent).find("#LargaDistancia").find(':selected').text()
    };
    direccion.departamento = {
        Id: $(activeStepContent).find("#Departamento").val(),
        Nombre: $(activeStepContent).find("#Departamento").find(':selected').text(),
        gls_departamento: $(activeStepContent).find("#Departamento").find(':selected').text()
    };
    direccion.provincia = {
        Id: $(activeStepContent).find("#Provincia").val(),
        Nombre: $(activeStepContent).find("#Provincia").find(':selected').text()
    };
    direccion.distrito = {
        Id: $(activeStepContent).find("#Distrito").val(),
        Nombre: $(activeStepContent).find("#Distrito").find(':selected').text()
    };
    direccion.numeroTelefono = celular;
    direccion.vigencia = "S";

    var benefi = listaBeneficiarios.find(x => x.Parentesco.Id == "80");
    /*var grabarOk = grabarBeneficiariosDireccion(benefi, entidadEdit, 0);*///Versión actual
    return grabarBeneficiariosDireccion(benefi, entidadEdit, 0).then(function (result) {
        console.log(result);
        console.log("guardarDireccion resultado promise:" + result);
        return result;
    });
}

function ValidarFormulario(stepperForm, activeStepContent) {
    console.log("ValidarFormulario");
    var entidadEdit = $(activeStepContent).find('#ModEntidad').val();
    limpiarErrores();

    if (entidadEdit == "B") {
        if ($(activeStepContent).find(".ApellidoPaterno").val().trim() == "") {
            $(activeStepContent).find(".ApellidoPaternoHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        if ($(activeStepContent).find(".ApellidoMaterno").val().trim() == "") {
            $(activeStepContent).find(".ApellidoMaternoHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        if ($(activeStepContent).find(".Nombres").val().trim() == "") {
            $(activeStepContent).find(".NombresHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        if ($(activeStepContent).find(".TipoDocumento").val() == null || $(activeStepContent).find(".TipoDocumento").val() == "0") {
            $(activeStepContent).find(".TipoDocumentoHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        if ($(activeStepContent).find(".NumeroDocumento").val().trim() == "") {
            $(activeStepContent).find(".NumeroDocumentoHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        if (codTipoPension == "S" && $(activeStepContent).find(".Parentesco").val() == "80") {
            if ($(activeStepContent).find(".FechaFallecimiento").val().trim() == "") {
                $(activeStepContent).find(".FechaFallecimientoHelper").addClass("helper-error").text("Campo obligatorio");
                return false;
            }
        }

        if ($(activeStepContent).find(".Nacionalidad").val() == null || $(activeStepContent).find(".Nacionalidad").val() == "0") {
            $(activeStepContent).find(".NacionalidadHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        if ($(activeStepContent).find(".PaisOrigen").val() == null || $(activeStepContent).find(".PaisOrigen").val() == "0") {
            $(activeStepContent).find(".PaisOrigenHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        if ($(activeStepContent).find(".CorreoElectronico").val() != null && $(activeStepContent).find(".CorreoElectronico").val() != "") {
            if (!isEmail($(activeStepContent).find(".CorreoElectronico").val().trim())) {
                $(activeStepContent).find(".CorreoElectronicoHelper").addClass("helper-error").text("Correo Electrónico inválido");
                return false;
            }
        }

        if ($(activeStepContent).find(".ESSALUD").val() == null || $(activeStepContent).find(".ESSALUD").val() == "") {
            $(activeStepContent).find(".ESSALUDHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }
    }

    if (entidadEdit == "D") {
        if ($("#Departamento").val() == null || $("#Departamento").val() == "0") {
            $("#DepartamentoHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        if ($("#Provincia").val() == null || $("#Provincia").val() == "0") {
            $("#ProvinciaHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        if ($("#Distrito").val() == null || $("#Distrito").val() == "0") {
            $("#DistritoHelper").addClass("helper-error").text("Campo obligatorio");
            return false;
        }

        //Si el departamento es distinto a Lima se pide LargaDistancia
        if ($("#Departamento").val() != "cea98f63-e634-e211-9da0-005056a6000f") {
            if ($("#LargaDistancia").val() == null || $("#LargaDistancia").val() == "0") {
                $("#LargaDistanciaHelper").addClass("helper-error").text("Campo obligatorio");
                return false;
            }
        }
    }

    return true;
}

function buscarPersonaRviadm(tipo, numero, solicitud, numeroCorrelativo) {
    console.log("buscarPersonaRviadm");
    $("#" + numeroCorrelativo).find('.PersonaHelperRviadm').text("");

    params = {
        tokenUsuario: $("#TokenUsuario").val(),
        tipoDoc: tipo,
        numeroDoc: numero,
        numeroSolicitud: solicitud,
        numeroCorrelativo: numeroCorrelativo
    };

    $.ajax({
        type: "POST",
        url: "CerrarSolicitud.aspx/ObtenerDatosRviadm",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: JSONstringifyConFechas(params),
        success: function (data) {
            if (data.d != null) {
                console.log("existe persona");
                console.log(data.d);
                $("#" + numeroCorrelativo).find('.PersonaHelperRviadm').text("Persona existente en Sistema Rentas");

                $("#" + numeroCorrelativo).find('.ApellidoPaterno').val(data.d.ApellidoPaterno);
                $("#" + numeroCorrelativo).find('.ApellidoMaterno').val(data.d.ApellidoMaterno);
                $("#" + numeroCorrelativo).find('.Nombres').val(data.d.Nombre);
                $("#" + numeroCorrelativo).find('.IndInvalidez').val(data.d.Invalido ? "S" : "N");
                $("#" + numeroCorrelativo).find('.TipoInvalidez').val(data.d.TipoInvalidez.Id);
                $("#" + numeroCorrelativo).find('.Sexo').val(data.d.Sexo);
                $("#" + numeroCorrelativo).find('.PaisOrigen').val(data.d.PaisOrigen.cod_parametro);
                $("#" + numeroCorrelativo).find('.CorreoElectronico').val(data.d.CorreoElectronico);
                $("#" + numeroCorrelativo).find('.Celular').val(data.d.numCelular);
                $("#" + numeroCorrelativo).find('.Nacionalidad').val(data.d.Nacionalidad.cod_nacionalidad);

                if (data.d.FechaInvalidez != null) {
                    $("#" + numeroCorrelativo).find('.FechaInvalidez').val(fecha(data.d.FechaInvalidez));
                }
                
                if (data.d.FechaNacimiento != null) {
                    $("#" + numeroCorrelativo).find('.FechaNacimiento').val(fecha(data.d.FechaNacimiento));
                }

                if (codTipoPension == "S") {
                    var fechaNac = data.d.FechaFallecimiento != null ? fecha(data.d.FechaFallecimiento) : '';
                    $("#" + numeroCorrelativo).find('.FechaFallecimiento').val(fechaNac);
                }
                else {
                    $("#" + numeroCorrelativo).find('.FechaFallecimiento').val('');
                }

                actualizarCombobox();
                configurarMascaras();

                listaBeneficiarios.forEach(benefi => {
                    if (benefi.numCorrelativo == numeroCorrelativo) {
                        console.log("actualizar persona en lista");

                        benefi.ApellidoPaterno = data.d.ApellidoPaterno;
                        benefi.ApellidoMaterno = data.d.ApellidoMaterno;
                        benefi.Nombre = data.d.Nombre;
                        benefi.Invalido = data.d.Invalido == "S" ? true : false;
                        benefi.TipoInvalidez.Id = data.d.TipoInvalidez.Id;
                        benefi.FechaInvalidez = data.d.FechaInvalidez != null ? cadenaAFecha(data.d.FechaInvalidez) : "";
                        benefi.Sexo = data.d.Sexo;
                        benefi.FechaNacimiento = data.d.FechaNacimiento != null ? cadenaAFecha(data.d.FechaNacimiento) : "";
                        if (codTipoPension == "S") {
                            benefi.FechaFallecimiento = data.d.FechaFallecimiento != null ? cadenaAFecha(data.d.FechaFallecimiento) : "";
                        }
                        else {
                            benefi.FechaFallecimiento = null;
                        }

                        benefi.Nacionalidad = {
                            cod_nacionalidad: data.d.Nacionalidad.cod_nacionalidad
                        }
                        benefi.PaisOrigen = {
                            cod_parametro: data.d.PaisOrigen.cod_parametro
                        }
                        benefi.numCelular = data.d.numCelular;
                        benefi.CorreoElectronico = data.d.CorreoElectronico;
                    }
                });
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            ManejarError(XMLHttpRequest, textStatus, errorThrown);
        }
    });
}

function grabarBeneficiariosDireccion(beneficiario, entidadEdit, correlativo) {
    console.log("grabarBeneficiariosDireccion");

    return new Promise(function (resolve, reject) {

        params = {
            tokenUsuario: $("#TokenUsuario").val(),
            numeroSolicitud: $("#NumeroSolicitud").val(),
            beneficiario: beneficiario,
            indicadorEdicion: entidadEdit
        };

        $.ajax({
            type: "POST",
            url: "CerrarSolicitud.aspx/GrabarBeneficiarioDireccion",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSONstringifyConFechas(params),
            success: function (data) {
                if (data.d != null) {
                    if (data.d.Estado == 'OK') {
                        console.log("grabarBeneficiariosDireccion ok");
                        resolve(true);
                    }
                    else {
                        if (entidadEdit == "B") {
                            $("#Mensaje" + correlativo).show();
                            $("#Mensaje" + correlativo).attr("class", "info");
                            $("#MensajeIcono" + correlativo).html("info");
                            $("#MensajeDetalle" + correlativo).html(data.d.Mensaje);
                        }

                        console.log("grabarBeneficiariosDireccion error");
                        resolve(false);
                    }
                }
                else {
                    if (entidadEdit == "B") {
                        $("#Mensaje" + correlativo).show();
                        $("#Mensaje" + correlativo).attr("class", "info");
                        $("#MensajeIcono" + correlativo).html("info");
                        $("#MensajeDetalle" + correlativo).html(data.d.Mensaje);
                    }

                    resolve(false);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ManejarError(XMLHttpRequest, textStatus, errorThrown);
                reject(false);
            },
            complete: function () {
                //cerrarModalCargando();
            }
        });

    });
}

function PoblarCombobox(control, arreglo) {
    $(control).html("");
    $(control).append($("<option>").text("«Seleccione»").attr("value", "0").attr("disabled", true));
    arreglo.forEach(a => {
        $(control).append($("<option>").text(a.Glosa).attr("value", a.Id));
    });
}

function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}