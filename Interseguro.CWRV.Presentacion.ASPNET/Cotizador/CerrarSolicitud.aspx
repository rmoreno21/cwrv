<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="CerrarSolicitud.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.CerrarSolicitud" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/stepper/mstepper.css")%>" type="text/css" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3">Cerrar Solicitud</span>
                    <div class="row">
                        <div class="input-field col s12 m6">
                            <input id="NumeroSolicitud" type="text" class="enteroPositivo" maxlength="7">
                            <label for="NumeroSolicitud">N° de Solicitud</label>
                            <span id="NumeroSolicitudHelper" class="helper-text"></span>
                        </div>
                    </div>

                    <button id="BuscarSolicitud" class="waves-effect waves-light btn blue darken-2">
                        <i class="material-icons left" style="margin-right: 5px">search</i>
                        Buscar
                    </button>

                </div>
            </div>

            <div class="card" id="cardPasosCierre" style="display: none">
                <input type="hidden" id="usuario_actual" value="<%= Session["Usuario"] %>" />Add commentMore actions
		        <input type="hidden" id="rol_azman" value="<%= Session["RolAzman"] %>" />
                <input type="hidden" id="url_api_rentas_rv" value="<%= System.Configuration.ConfigurationManager.AppSettings["ApiCotizadorRvUrl"] %>" />
                <div class="card-content">
                    <%--<div class="row">--%>
                        <ul class="stepper linear" id="PasosCierre">
                            <li class="step" id="pasoCotizaciones">
                                <div class="step-title waves-effect">Cotizaciones</div>
                                <div class="step-content">

                                    <div id="DatosCierre" class="card">
                                        <div class="card-content">
                                            <asp:HiddenField ID="ModEntidad" runat="server" ClientIDMode="Static" Value="C" />
                                            <span class="card-title blue-text text-darken-3"><a name="datos-cotizacion"></a>Cierre de Póliza <span id="NumeroPoliza"></span></span>
                                            <div class="row">
                                                <div class="input-field col s12 black-text">
                                                    <blockquote id="Mensaje" style="display: none">
                                                        <i id="MensajeIcono" class="material-icons icono">done</i>
                                                        <span id="MensajeDetalle"></span>
                                                    </blockquote>
                                                </div>
                                            </div>

                                            <div class="row">
                                                <div id="TablaCotizaciones" class="input-field col s12 black-text">
                                                </div>
                                            </div>

                                            <div class="center">
                                                <button id="Transmitir" data-target="modal-transmitir" class="waves-effect waves-light btn blue darken-2 modal-trigger">
                                                    <i class="material-icons left" style="margin-right: 5px">double_arrow</i>
                                                    Transmitir
                                                </button>
                                                <button id="CartaAFP" class="waves-effect waves-light btn blue darken-2">
                                                    <i class="material-icons left" style="margin-right: 5px">article</i>
                                                    Carta a la AFP
                                                </button>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="step-actions">
                                        <button class="waves-effect waves-dark btn-flat previous-step" id="btnRegresarPasoCotizaciones">REGRESAR</button>
                                    </div>
                                </div>
                            </li>
                        </ul>
                    <%--</div>--%>
                </div>
            </div>

            <%--<div id="DatosCotizaciones" class="card" style="display: none">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3"><a name="datos-cotizacion"></a>Cierre de Póliza <span id="NumeroPoliza"></span></span>
                    <div class="row">
                        <div class="input-field col s12 black-text">
                            <blockquote id="Mensaje" style="display: none">
                                <i id="MensajeIcono" class="material-icons icono">done</i>
                                <span id="MensajeDetalle"></span>
                            </blockquote>
                        </div>
                    </div>

                    <div class="row">
                        <div id="TablaCotizaciones" class="input-field col s12 black-text">
                        </div>
                    </div>

                    <div class="center">
                        <button id="Transmitir" data-target="modal-transmitir" class="waves-effect waves-light btn blue darken-2 modal-trigger">
                            <i class="material-icons left" style="margin-right: 5px">double_arrow</i>
                            Transmitir
                        </button>
                        <button id="CartaAFP" class="waves-effect waves-light btn blue darken-2">
                            <i class="material-icons left" style="margin-right: 5px">article</i>
                            Carta a la AFP
                        </button>
                    </div>
                </div>
            </div>--%>
        </div>
    </div>

    <!-- Modal Structure -->
    <div id="modal-transmitir" class="modal">
        <div class="modal-content">
            <h5>Transmitir Póliza</h5>
            <p>
                ¿Confirma que desea transmitir la información de la Solicititud <b><span id="ConfSolicitud" class="red-text text-darken-4"></span></b>
                Cotización <b><span id="ConfCotizacion" class="red-text text-darken-4"></span></b>hacia la póliza
                <b><span id="ConfPoliza" class="red-text text-darken-4"></span></b>?
            </p>
            <p>
                <b>Esta acción no puede ser revertida.</b>
            </p>
        </div>
        <div class="modal-footer">
            <button id="ConfirmarTransmision" class="waves-effect waves-light btn blue darken-2">
                Aceptar
            </button>

            <button id="Cancelar" class="waves-effect waves-light btn blue darken-2">
                Cancelar
            </button>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaVitalicia/CierreSolicitud.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Estilos/stepper/mstepper.js")%>"></script>

    <script>
        var transmitir = false;
        var carta = false;
        var solicitudConsultada;
        var contizacionSeleccionada;
        var modalTransmitir;

        var codTipoPension;
        var listaCotizaciones;
        var listaBeneficiarios;
        var fecDevengue;

        $(document).ready(function () {
            $("#BuscarSolicitud").click(function (e) {
                e.preventDefault();

                if ($("#NumeroSolicitud").val() == "") {
                    return false;
                }

                solicitudConsultada = $("#NumeroSolicitud").val();

                limpiarErrores();
                deshabilitarBotones();
                //$("#DatosCotizaciones").hide();
                abrirModalCargando("Cargando beneficiarios y cotizaciones de la solicitud...");
                $("#cardPasosCierre").hide();

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    numeroSolicitud: $("#NumeroSolicitud").val()
                };

                $.ajax({
                    type: "POST",
                    url: "CerrarSolicitud.aspx/ObtenerCotizaciones",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(params),
                    success: function (data) {
                        console.log(data);
                        if (data.d != null) {
                            // Validaciones
                            if (data.d.Cotizaciones.length == 0) {
                                cerrarModalCargando();

                                $("#NumeroSolicitud").removeClass("valid");
                                $("#NumeroSolicitud").addClass("invalid");
                                $("#NumeroSolicitudHelper").addClass("helper-error");
                                $("#NumeroSolicitudHelper").html("No se han encontrado datos para esta solicitud");
                            }
                            else if (data.d.TipoCotizacion.Id != "O") {
                                cerrarModalCargando();

                                $("#NumeroSolicitud").removeClass("valid");
                                $("#NumeroSolicitud").addClass("invalid");
                                $("#NumeroSolicitudHelper").addClass("helper-error");
                                $("#NumeroSolicitudHelper").html("Esta solicitud no corresponde a Rentas Vitalicias");
                            }
                            else if (data.d.NumeroPoliza == 0 || data.d.NumeroPoliza == null) {
                                cerrarModalCargando();

                                $("#NumeroSolicitud").removeClass("valid");
                                $("#NumeroSolicitud").addClass("invalid");
                                $("#NumeroSolicitudHelper").addClass("helper-error");
                                $("#NumeroSolicitudHelper").html("Esta solicitud no ha sido ganada por Interseguro");
                            }
                            else {
                                //$("#DatosCotizaciones").show();
                                $("#NumeroPoliza").html(data.d.NumeroPoliza);
                                $("#cardPasosCierre").show();
                                listaCotizaciones = data.d.Cotizaciones;

                                var elegida = false;
                                data.d.Cotizaciones.forEach(c => {
                                    if (c.EstadoCotizacion == "04") {
                                        cotizacionSeleccionada = c.Correlativo;
                                        elegida = true;
                                        transmitir = false;
                                        carta = true;
                                    }
                                });

                                var ultimoRecalculo = false;
                                data.d.Cotizaciones.forEach(c => {
                                    if (c.TipoCalculo != null) {
                                        if (c.TipoCalculo.Id == "9") {
                                            ultimoRecalculo = true;
                                        }
                                    }
                                });

                                if (elegida) {
                                    $("#Mensaje").show();
                                    $("#Mensaje").attr("class", "ok");
                                    $("#MensajeIcono").html("done");
                                    $("#MensajeDetalle").html("Esta póliza ya fue transmitida al sistema de Rentas");

                                    console.log("transmitida");
                                    $("#btnRegresarPasoCotizaciones").hide();
                                    cargarTablaCotizaciones(true);
                                }
                                else {
                                    $("#Mensaje").show();
                                    $("#Mensaje").attr("class", "info");
                                    $("#MensajeIcono").html("info");
                                    $("#MensajeDetalle").html("Elija la cotización a cerrar y luego presione el botón Transmitir");

                                    console.log("no transmitida");
                                    codTipoPension = data.d.TipoPension.Id;
                                    listaBeneficiarios = data.d.BeneficiariosBenefi;
                                    fecDevengue = data.d.FechaDevengue;

                                    if (ultimoRecalculo) {
                                        console.log("ultimoRecalculo");
                                        $("#btnRegresarPasoCotizaciones").show();
                                        cargarTablaCotizaciones(false);
                                        cargarBeneficiariosDireccion($("#NumeroSolicitud").val());
                                    }
                                    else {
                                        console.log("ultimoRecalculo NO");
                                        $("#btnRegresarPasoCotizaciones").hide();
                                        cargarTablaCotizaciones(true);
                                    }
                                    
                                }
                            }
                        }
                        else {
                            cerrarModalCargando();

                            $("#NumeroSolicitud").removeClass("valid");
                            $("#NumeroSolicitud").addClass("invalid");
                            $("#NumeroSolicitudHelper").addClass("helper-error");
                            $("#NumeroSolicitudHelper").html("No se han encontrado datos para esta solicitud");
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                        cerrarModalCargando();
                    },
                    complete: function () {
                        habilitarBotones();
                        //cerrarModalCargando();
                        validarBotonesLocales();
                    }
                });
            });

            $("#Transmitir").click(function (e) {
                e.preventDefault();

                $("#ConfSolicitud").html(solicitudConsultada);
                $("#ConfCotizacion").html(cotizacionSeleccionada);
                $("#ConfPoliza").html($("#NumeroPoliza").html());
            });

            $("#CartaAFP").click(function (e) {
                e.preventDefault();

                // Descargar el reporte
                window.open("<%=ResolveUrl("~/Reportes/CartaAFP.aspx")%>?s=" + solicitudConsultada + "&c=" + cotizacionSeleccionada);
            });

            $("#ConfirmarTransmision").on("click", async function (e) {
                e.preventDefault();
                cerrarModal("modal-transmitir");

                limpiarErrores();
                deshabilitarBotones();
                abrirModalCargando("Transmitiendo información de la Solicitud " + solicitudConsultada + " a la póliza " + $("#NumeroPoliza").html() + " en el sistema de Rentas...");

                const params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    numeroSolicitud: solicitudConsultada,
                    numeroCorrelativo: cotizacionSeleccionada,
                    numeroPoliza: $("#NumeroPoliza").html()
                };

                try {
                    const URL_CERRAR_SOLICITUD = $('#url_api_rentas_rv').val() + '/cotizacion/cerrar-solicitud'
                    const response = await fetch(URL_CERRAR_SOLICITUD, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json; charset=iso-8859-1',
                            'x-username': $('#usuario_actual').val(),
                            'x-rol': $('#rol_azman').val()
                        },
                        body: JSON.stringify(params)
                    });

                    const data = await response.json();
                    const result = data.data;

                    $("#Mensaje").show();

                    if (result) {
                        if (result.Estado === 'OK') {
                            $("#Mensaje").attr("class", "ok");
                            $("#MensajeIcono").html("done");
                            $("#MensajeDetalle").html("Esta póliza ya fue transmitida al sistema de Rentas");

                            $("#TabCotizacionesCierreRVI input").attr("disabled", "disabled");
                            transmitir = false;
                            carta = true;
                            $("#btnRegresarPasoCotizaciones").hide();
                        } else {
                            $("#Mensaje").attr("class", "info");
                            $("#MensajeIcono").html("info");
                            $("#MensajeDetalle").html(result.Mensaje);
                        }
                    } else {
                        $("#Mensaje").show();
                        $("#Mensaje").attr("class", "error");
                        $("#MensajeIcono").html("done");
                        $("#MensajeDetalle").html("Error al transmitir la póliza al sistema de Rentas");
                    }
                } catch (error) {
                    console.error('Error en la transmisión:', error);
                    $("#Mensaje").show();
                    $("#Mensaje").attr("class", "error");
                    $("#MensajeIcono").html("error");
                    $("#MensajeDetalle").html("Error al transmitir la póliza al sistema de Rentas");
                } finally {
                    habilitarBotones();
                    cerrarModalCargando();
                    validarBotonesLocales();
                }
            });

            $("#Cancelar").click(function (e) {
                e.preventDefault();
                cerrarModal("modal-transmitir");
            });

            $(document).on("change", "input[name='cot']", function () {
                if (this.checked) {
                    validarTablaCotizaciones($(this).attr("id"));
                }
                cotizacionSeleccionada = $(this).attr("id");
                transmitir = true;
                carta = true;
                validarBotonesLocales();
            });

            function validarTablaCotizaciones(id) {
                $("#TabCotizacionesCierreRVI tbody tr").removeClass("fila-seleccionada");
                var tr = $("#" + id).parent().parent().parent();
                tr.addClass("fila-seleccionada");
            }

            function validarBotonesLocales() {
                if (transmitir) {
                    $("#Transmitir").removeAttr("disabled");
                }
                else {
                    $("#Transmitir").attr("disabled", "disabled");
                }

                if (carta) {
                    $("#CartaAFP").removeAttr("disabled");
                }
                else {
                    $("#CartaAFP").attr("disabled", "disabled");
                }
            }

            function cargarTablaCotizaciones(iniciarStepper) {
                //$("#DatosCotizaciones").hide();
                //abrirModalCargando("Cargando cotizaciones de la solicitud...");

                params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    cotizaciones: listaCotizaciones
                };

                $.ajax({
                    type: "POST",
                    url: "CerrarSolicitud.aspx/CargarTablaCotizaciones",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSONstringifyConFechas(params),
                    success: function (data) {
                        //$("#DatosCotizaciones").show();
                        $("#TablaCotizaciones").html($(data.d).find('#ContenidoDinamico').html());
                        validarTablaCotizaciones($("#TabCotizacionesCierreRVI input:checked").attr("id"));

                        if (iniciarStepper) {
                            //ingresa cuando la solicitud ya fue transmitida y sólo se muestra cotizaciones
                            //elimina los pasos de beneficiarios y dirección por si ya fueron cargados previamente
                            $("#PasosCierre").find('.li-direccion').remove();
                            $("#PasosCierre").find('.li-beneficiario').remove();
                            inicializarStepper();
                            cerrarModalCargando();
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                    },
                    complete: function () {
                        habilitarBotones();
                        //cerrarModalCargando();
                        validarBotonesLocales();
                    }
                });
            }
        });
    </script>
</asp:Content>
