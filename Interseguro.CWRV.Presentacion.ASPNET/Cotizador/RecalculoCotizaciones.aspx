<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="RecalculoCotizaciones.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.RecalculoCotizaciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiCotizadorRV.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3">Recálculo de Cotizaciones</span>
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

            <div id="DatosCotizacion" class="card" style="display: none">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3"><a name="datos-cotizacion"></a>Datos de la Cotización</span>

                    <blockquote id="Mensaje" style="display:none">
                        <i id="MensajeIcono" class="material-icons icono">done</i>
                        <span id="MensajeDetalle"></span>
                    </blockquote>

                    <div class="row">
                        <div class="input-field col s6">
                            <select id="TipoCalculo">
                                <option value="8">Primer Recálculo</option>
                                <option value="9">Recálculo Final</option>
                            </select>
                            <label for="TipoCalculo">Tipo de Recálculo</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m6">
                            <input id="Solicitud" type="text" readonly>
                            <label class="active" for="Solicitud">N° de Solicitud</label>
                        </div>

                        <div class="input-field col s12 m6">
                            <input id="Poliza" type="text" readonly>
                            <label class="active" for="Poliza">N° de Póliza</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12">
                            <input id="Modalidad" type="text" readonly>
                            <label class="active" for="Modalidad">Modalidad</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m6">
                            <input id="Correlativo" type="text" readonly>
                            <label class="active" for="Correlativo">N° de Correlativo</label>
                        </div>

                        <div class="input-field col s12 m6">
                            <input id="TipoCambio" type="text" class="validate numerico3">
                            <label class="active" for="TipoCambio">Tipo de Cambio</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m6">
                            <input id="FechaCotizacion" type="text" class="validate fecha datepicker">
                            <label class="active" for="FechaCotizacion">Fecha de Cotización</label>
                        </div>

                        <div class="input-field col s12 m6">
                            <input id="CIC" type="text" class="validate numerico">
                            <label class="active" for="CIC">CIC</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m6">
                            <input id="PensionAFP" type="text" readonly>
                            <label class="active" for="PensionAFP">Pensión AFP</label>
                        </div>

                        <div class="input-field col s12 m6">
                            <input id="MontoAFP" type="text" readonly>
                            <label class="active" for="MontoAFP">Monto AFP</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="input-field col s12 m6">
                            <input id="PensionIS" type="text" readonly>
                            <label class="active" for="PensionIS">Pensión IS</label>
                        </div>

                        <div class="input-field col s12 m6">
                            <input id="MontoIS" type="text" readonly>
                            <label class="active" for="MontoIS">Monto IS</label>
                        </div>
                    </div>

                    <button id="Recalcular" class="waves-effect waves-light btn blue darken-2">
                        <i class="material-icons left" style="margin-right: 5px">calculate</i>
                        Recalcular
                    </button>

                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        $(document).ready(function () {
            var ind_modalidad = "";

            $("#BuscarSolicitud").click(function (e) {
                e.preventDefault();

                limpiarErrores();
                deshabilitarBotones();
                $("#DatosCotizacion").hide();
                $("#Mensaje").hide();
                abrirModalCargando("Cargando datos de la cotización");

                // Desbloquear Combobox
                $("#TipoCalculo").removeAttr("disabled");
                actualizarCombobox("#TipoCalculo");

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    numeroSolicitud: $("#NumeroSolicitud").val()
                };

                $.ajax({
                    type: 'POST',
                    url: 'RecalculoCotizaciones.aspx/ObtenerCotizacionRecalculo',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    data: JSON.stringify(params),
                    success: function (data) {
                        console.log(data);
                        if (data.d != null) {
                            console.log(data.d);
                            $("#DatosCotizacion").show();

                            ind_modalidad = data.d.Cotizaciones[0].Modalidad.Indicador;
                            if (ind_modalidad == "I" || ind_modalidad == "I-RB" || ind_modalidad == "I-RVE") {
                                $("#TipoCalculo").val("9");
                                $("#TipoCalculo").prop("disabled", "disabled");

                                // Bloquear combobox
                                actualizarCombobox("#TipoCalculo");
                            }
                            else {
                                $("#TipoCalculo").val("8");
                                $("#TipoCalculo").removeAttr("disabled");

                                // Desbloquear combobox
                                actualizarCombobox("#TipoCalculo");
                            }

                            $("#Solicitud").val(data.d.Id);
                            $("#Poliza").val(data.d.NumeroPoliza);
                            $("#Modalidad").val(data.d.Cotizaciones[0].Modalidad.Nombre);
                            $("#Correlativo").val(data.d.Cotizaciones[0].Correlativo);
                            console.log(fecha(data.d.Cotizaciones[0].FechaCotizacion));
                            $("#TipoCambio").val(monto(data.d.Cotizaciones[0].ValorMoneda, 3));
                            $("#FechaCotizacion").val(fecha(data.d.Cotizaciones[0].FechaCotizacion));
                            $("#CIC").val(monto(data.d.SaldoCIC));
                            $("#PensionAFP").val("S/ " + monto(data.d.Cotizaciones[0].PensionAFP));
                            $("#MontoAFP").val("S/ " + monto(data.d.Cotizaciones[0].MontoAFP));
                            $("#PensionIS").val(data.d.Cotizaciones[0].Moneda.Simbolo + " " + monto(data.d.Cotizaciones[0].PensionCiaMO));
                            $("#MontoIS").val("S/ " + monto(data.d.Cotizaciones[0].MontoCia));
                            M.updateTextFields();
                            $("#FechaCotizacion").focus();
                            scroll("datos-cotizacion");
                        }
                        else {
                            $("#NumeroSolicitud").removeClass("valid");
                            $("#NumeroSolicitud").addClass("invalid");
                            $("#NumeroSolicitudHelper").addClass("helper-error");
                            $("#NumeroSolicitudHelper").html("No se han encontrado resultados para esta solicitud");
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                        $("#NumeroSolicitud").removeClass("valid");
                        $("#NumeroSolicitud").addClass("invalid");
                        $("#NumeroSolicitudHelper").addClass("helper-error");
                        $("#NumeroSolicitudHelper").html(XMLHttpRequest.responseJSON.Message);
                    },
                    complete: function () {
                        habilitarBotones();
                        cerrarModalCargando();
                    }
                });
            });

            $("#Recalcular").click(async function (e) {
                e.preventDefault();

                limpiarErrores();
                deshabilitarBotones();
                $("#Mensaje").hide();
                abrirModalCargando("Recalculando la cotización...");

                const fechaCotizacion = $("#FechaCotizacion").val();
                const fechaFormateada = fechaCotizacion.split("/").reverse().join("-");

                const montoCIC = +$("#CIC").val().replace(/,/g, "");
                const tipoCambio = +$("#TipoCambio").val();
                const tipoCalculo = $("#TipoCalculo").val();

                const params = {
                    numSolicitud: $("#NumeroSolicitud").val(),
                    fechaCotizacion: fechaFormateada,
                    montoCIC: montoCIC,
                    tipoCambio: tipoCambio,
                    tipoCalculo: tipoCalculo
                };
                console.log({params});
                try {
                    const data = await ApiCotizadorRV.RecalcularCotizacion(params);
                    console.log(data);
                    if (data != null) {
                        $("#Correlativo").val(data.Cotizaciones[0].Correlativo);
                        $("#TipoCambio").val(monto(data.Cotizaciones[0].ValorMoneda), 3);
                        $("#CIC").val(monto(data.SaldoCIC));
                        $("#PensionAFP").val("S/ " + monto(data.Cotizaciones[0].PensionAFP));
                        $("#MontoAFP").val("S/ " + monto(data.Cotizaciones[0].MontoAFP));
                        $("#PensionIS").val(data.Cotizaciones[0].Moneda.Simbolo + " " + monto(data.Cotizaciones[0].PensionCiaMO));
                        $("#MontoIS").val("S/ " + monto(data.Cotizaciones[0].MontoCia));

                        $("#Mensaje").show();
                        $("#Mensaje").attr("class", "ok");
                        $("#MensajeIcono").html("done");
                        $("#MensajeDetalle").html("Cotización recalculada correctamente");

                        // Descargar el reporte
                        window.open("<%=ResolveUrl("~/Reportes/RecalculoCotizaciones.aspx")%>?s=" + $("#NumeroSolicitud").val() + "&c=" + $("#Correlativo").val());
                    }
                } catch (error) {
                    console.log(error);
                    $("#Mensaje").show();
                    $("#Mensaje").attr("class", "error");
                    $("#MensajeIcono").html("error");
                    $("#MensajeDetalle").html(error.message || error?.body?.error?.message || "Ocurrió un error al recalcular la cotización");
                }

                habilitarBotones();
                cerrarModalCargando();
                scroll("datos-cotizacion");



                /* $.ajax({
                    type: 'POST',
                    url: 'RecalculoCotizaciones.aspx/RecalcularCotizacion',
                    contentType: "application/json; charset=utf-8",
                    dataType: 'json',
                    data: JSON.stringify(params),
                    success: function (data) {
                        console.log(data);
                        if (data.d != null) {
                            console.log(data.d);
                            console.log(data.d.Cotizaciones[0]);
                            $("#Correlativo").val(data.d.Cotizaciones[0].Correlativo);
                            $("#TipoCambio").val(monto(data.d.Cotizaciones[0].ValorMoneda), 3);
                            $("#CIC").val(monto(data.d.SaldoCIC));
                            $("#PensionAFP").val("S/ " + monto(data.d.Cotizaciones[0].PensionAFP));
                            $("#MontoAFP").val("S/ " + monto(data.d.Cotizaciones[0].MontoAFP));
                            $("#PensionIS").val(data.d.Cotizaciones[0].Moneda.Simbolo + " " + monto(data.d.Cotizaciones[0].PensionCiaMO));
                            $("#MontoIS").val("S/ " + monto(data.d.Cotizaciones[0].MontoCia));

                            $("#Mensaje").show();
                            $("#Mensaje").attr("class", "ok");
                            $("#MensajeIcono").html("done");
                            $("#MensajeDetalle").html("Cotización recalculada correctamente");

                            // Descargar el reporte
                            window.open("<%=ResolveUrl("~/Reportes/RecalculoCotizaciones.aspx")%>?s=" + $("#NumeroSolicitud").val() + "&c=" + $("#Correlativo").val());
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);

                        $("#Mensaje").show();
                        $("#Mensaje").attr("class", "error");
                        $("#MensajeIcono").html("error");
                        $("#MensajeDetalle").html(XMLHttpRequest.responseJSON.Message);
                    },
                    complete: function () {
                        habilitarBotones();
                        cerrarModalCargando();
                        scroll("datos-cotizacion");
                    }
                }); */
            });
        });

    </script>
</asp:Content>
