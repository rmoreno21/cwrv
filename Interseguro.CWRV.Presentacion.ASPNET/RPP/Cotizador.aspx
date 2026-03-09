<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="Cotizador.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RPP.Cotizador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        table {
            font-size: 12px;
        }

        table input {
            font-size: 12px !important;
        }

        td select {
            font-size: 12px !important;
        }

        .preloader-parametros {
            display: none;
            position: fixed;
            bottom: 5px;
            left: 5px;
            color: #0d47a1;
            border: 1px solid #0d47a1;
            padding: 5px;
            background: rgba(255, 255, 255, 0.5);
            -webkit-border-radius: 5px;
               -moz-border-radius: 5px;
                    border-radius: 5px;
        }

        .no-cotizable {
            background-color: #ffcdd2;
        }

        .seleccionada {
            background-color: #c8e6c9;
        }

        .eliminar-cotizacion {
            cursor: pointer;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="CUSPP" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="Modo" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="NumeroSolicitud" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="MontoCIC" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="TipoCambio" ClientIDMode="Static" Value="1" runat="server" />
    <asp:HiddenField ID="Semilla" ClientIDMode="Static" Value="1" runat="server" />

    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3">Cotizar Renta Particular Plus</span>
                    <div class="row">
                        <div class="col s12">
                            <ul class="tabs">
                                <li class="tab col s3"><a href="#cotizador">Cotizador</a></li>
                                <li class="tab col s3"><a href="#beneficiarios">Beneficiarios (<span id="total-beneficiarios"></span>)</a></li>
                            </ul>
                        </div>
                        <div id="cotizador" class="col s12">
                            <div class="row">
                                <div class="input-field col s12 m4">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">numbers</i>
                                    <asp:TextBox ID="RPPSolicitud" runat="server" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>
                                    <label for="RPPSolicitud">Nro. Solicitud</label>
                                </div>
                                <div class="input-field col s12 m4">
                                    <i class="material-icons prefix">schedule</i>
                                    <asp:DropDownList ID="RPPTemporalidad" runat="server" ClientIDMode="Static">
                                    </asp:DropDownList>
                                    <label for="RPPTemporalidad">Temporalidad</label>
                                </div>
                                <div class="input-field col s12 m4">
                                    <i class="material-icons prefix">group</i>
                                    <asp:DropDownList ID="RPPTipoPlan" runat="server" ClientIDMode="Static">
                                    </asp:DropDownList>
                                    <label for="RPPTipoPlan">Tipo de Plan</label>
                                </div>
                            </div>

                            <div class="row">
                                <div class="input-field col s12 m4">
                                    <i class="material-icons prefix">today</i>
                                    <asp:TextBox ID="RPPFechaCotizacion" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label class="active" for="RPPFechaCotizacion">Fecha de Cotización</label>
                                </div>
                                <div class="input-field col s12 m4">
                                    <i class="material-icons prefix">today</i>
                                    <asp:TextBox ID="RPPFechaDevengue" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label for="RPPFechaDevengue">Fecha de Devengue</label>
                                </div>
                                <div class="input-field col s12 m4">
                                    <i class="material-icons prefix">today</i>
                                    <asp:TextBox ID="RPPVigencia" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label for="RPPVigencia">Vigente hasta</label>
                                </div>
                            </div>

                            <div class="row">
                                <div class="input-field col s12 m4">
                                    <i class="material-icons prefix">price_change</i>
                                    <asp:DropDownList ID="RPPMonedaPrimaUnica" runat="server" ClientIDMode="Static">
                                    </asp:DropDownList>
                                    <label for="RPPMonedaPrimaUnica">Moneda de Prima Única</label>
                                </div>
                                <div class="input-field col s12 m4">
                                    <i class="material-icons prefix">payments</i>
                                    <asp:TextBox ID="RPPPrimaUnica" CssClass="numerico" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label for="RPPPrimaUnica">Prima Única</label>
                                    <span id="rpp-prima-unica-helper" class="helper-text red-text" data-error=""></span>
                                </div>
                                <div class="input-field col s12 m4">
                                    <i class="material-icons prefix">percent</i>
                                    <asp:TextBox ID="RPPDCOM" CssClass="enteroPositivo" MaxLength="3" runat="server" ClientIDMode="Static"></asp:TextBox>
                                    <label for="RPPDCOM">Porcentaje D</label>
                                    <span id="rpp-dcom-helper" class="helper-text red-text" data-error=""></span>
                                </div>
                            </div>

                            <div class="row">
                                <a name="ancla-cotizaciones"></a>
                                <div id="TablaCotizaciones" class="col s12 black-text center-align">
                                </div>
                                <div id="TablaCotizacionesCargando" class="col s12 valign-wrapper">
                                    <div class="preloader-wrapper small active" style="width:24px;height:24px">
                                        <div class="spinner-layer spinner-blue-only">
                                            <div class="circle-clipper left">
                                                <div class="circle"></div>
                                            </div><div class="gap-patch">
                                            <div class="circle"></div>
                                            </div><div class="circle-clipper right">
                                            <div class="circle"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <span style="margin-left:10px">
                                        Cargando tabla de cotizaciones...
                                    </span>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col s12 center-align">
                                    <asp:HyperLink ID="Cotizar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">calculate</i>Cotizar</asp:HyperLink>
                                    <asp:HyperLink ID="DecargarDetalleSolicitud" Target="_blank" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2" style="display:none"><i class="material-icons left">picture_as_pdf</i>Descargar</asp:HyperLink>
                                    <asp:HyperLink ID="Regresar" NavigateUrl="~/RentaPrivadaPlus/Cotizador.aspx#datos_solicitud" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">arrow_back</i>Regresar</asp:HyperLink>
                                </div>
                            </div>

                        </div>
                        <div id="beneficiarios" class="col s12">
                            <div id="grupo-familiar" class="row">
                                <div class="col s12">
                                    <asp:GridView
                                        ID="TablaRPPGrupoFamiliar" runat="server"
                                        ViewStateMode="Disabled" ClientIDMode="Static"
                                        AutoGenerateColumns="False" CssClass="highlight" OnRowDataBound="TablaRPPGrupoFamiliar_RowDataBound">    
                                        <Columns>
                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Parentesco">
                                                <ItemTemplate>
                                                    <asp:Label ID="Parentesco" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Apellidos y Nombres">
                                                <ItemTemplate>
                                                    <asp:Label ID="Nombres" runat="server" Text='<%# Eval("Nombre") + " " + Eval("ApellidoPaterno") + " " + Eval("ApellidoMaterno") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Sexo">
                                                <ItemTemplate>
                                                    <asp:Label ID="Sexo" runat="server" Text='<%# Bind("Sexo") %>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="FechaNacimiento"
                                                HeaderText="Fec.Nacimiento" DataFormatString="{0:dd/MM/yyyy}">
                                                <ItemStyle HorizontalAlign="Center" />
                                            </asp:BoundField>
                                        </Columns>
                                        <EmptyDataTemplate>
                                            <p style="font-size:1rem"><i class="material-icons amber-text text-darken-2" style="margin-right:10px">warning</i> No se han encontrado beneficiarios para cotizar, revise la pestaña "Grupo Familiar".</p>
                                        </EmptyDataTemplate>
                                    </asp:GridView>
                                </div>
                            </div>

                            <div id="beneficiarios-solicitud" class="row" style="display:none">
                                <div class="col s12">
                                    <h6>Beneficiarios de la Solicitud</h6>
                                </div>
                                <div id="TablaBeneficiarios" class="col s12 black-text center-align">
                                </div>
                                <div id="TablaBeneficiariosCargando" class="col s12 valign-wrapper">
                                    <div class="preloader-wrapper small active" style="width:24px;height:24px">
                                        <div class="spinner-layer spinner-blue-only">
                                            <div class="circle-clipper left">
                                                <div class="circle"></div>
                                            </div><div class="gap-patch">
                                            <div class="circle"></div>
                                            </div><div class="circle-clipper right">
                                            <div class="circle"></div>
                                            </div>
                                        </div>
                                    </div>
                                    <span style="margin-left:10px">
                                        Cargando tabla de beneficiarios...
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal Structure -->
    <div id="modal-eliminar-cotizacion" class="modal">
        <div class="modal-content">
            <h5>Confirmación</h5>
            <p>
                ¿Confirma la eliminación de la cotización <b>N° <span id="NroCotizacion"></span></b>?
            </p>
        </div>
        <div class="modal-footer">
            <button id="ConfirmarEliminacion" class="waves-effect waves-light btn blue darken-2">
                Aceptar
            </button>

            <button id="Cancelar" class="waves-effect waves-light btn blue darken-2">
                Cancelar
            </button>
        </div>
    </div>

    <div class="preloader-parametros valign-wrapper">
        <div class="preloader-wrapper small active" style="width:16px;height:16px">
            <div class="spinner-layer spinner-blue-only">
                <div class="circle-clipper left">
                    <div class="circle"></div>
                </div><div class="gap-patch">
                <div class="circle"></div>
                </div><div class="circle-clipper right">
                <div class="circle"></div>
                </div>
            </div>
        </div>
        Cargando parámetros...
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        var Solicitud = null;
        var idCotizacion;
        var parametrosCargados = false;
        var tablaCotizacionesCargada = false;

        $(document).ready(function () {
            evaluarDisponibilidadCotizar();

            // Validar cantidad de beneficiarios
            if ($("#TablaRPPGrupoFamiliar input[type=checkbox]").length == 1) {
                // Deshabilitar la opción FAMILIAR
                $("#RPPTipoPlan").find("option[value=01]").prop("disabled", true);
            }
            if ($("#Modo").val() == "N") {
                $("#TablaRPPGrupoFamiliar input[type=checkbox]").each(function () {
                    $(this).prop("checked", true);
                });
            }
            if ($("#TablaRPPGrupoFamiliar input[type=checkbox]:checked").length == 1) {
                // Setear INDIVIDUAL por defecto
                $("#RPPTipoPlan").val("02");
            }
            actualizarContadorBeneficiariosMarcados();

            // Modo consulta
            if ($("#Modo").val() == "C") {
                $("#Cotizar").addClass("disabled");
                $("#grupo-familiar").hide();
            }

            // Crear u obtener los datos de la solicitud según corresponda
            params = {
                tokenUsuario: $("#TokenUsuario").val(),
                cuspp: $("#CUSPP").val(),
                numeroSolicitud: $("#NumeroSolicitud").val(),
                temporalidad: $("#RPPTemporalidad").val(),
                modo: $("#Modo").val(),
                semilla: $("#Semilla").val()
            };

            $.ajax({
                type: "POST",
                url: "Cotizador.aspx/ObtenerSolicitud",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params),
                success: function (data) {
                    if (data.d != null) {
                        Solicitud = data.d;

                        // Validar si es que hay cotizaciones cerradas
                        var cotizacionCerrada = false;
                        Solicitud.Cotizaciones.forEach(c => {
                            if (c.EstadoCotizacion == "04" || c.EstadoCotizacion == "05") {
                                cotizacionCerrada = true;
                            }
                        });

                        // Si es que es un Agente o si eiste una cotización cerrada, pasar a modo lectura
                        if ($("#Modo").val() == "M") {
                            var fechaCotizacion = cadenaAFecha(fecha(Solicitud.FechaCotizacion)).setHours(0, 0, 0, 0);
                            var hoy = (new Date()).setHours(0, 0, 0, 0);
                            if (cotizacionCerrada || ((Solicitud.Usuario.Rol == "AGT.LIM.RVI" || Solicitud.Usuario.Rol == "AGT.PRO.RVI") && fechaCotizacion > hoy)) {
                                window.location.href = "<%=ResolveUrl("~/RPP/Cotizador.aspx")%>?c=" + $("#CUSPP").val() + "&m=C&s=" + Solicitud.Id;
                            }
                        }

                        $("#RPPSolicitud").val(Solicitud.Id);
                        $("#RPPFechaCotizacion").val(fecha(Solicitud.FechaCotizacion));
                        $("#RPPFechaDevengue").val(fecha(Solicitud.FechaDevengue));
                        $("#RPPVigencia").val(fecha(Solicitud.FechaVigencia));
                        $("#RPPMonedaPrimaUnica").val(Solicitud.MonedaPrimaUnica.Id);
                        $("#RPPPrimaUnica").val(Solicitud.PrimaUnica).trigger("change");
                        $("#RPPDCOM").val(Solicitud.PorcentajeDescuentoComision);
                        $("#MontoCIC").val(Solicitud.MontoCIC);
                        $("#TipoCambio").val(Solicitud.TipoCambio);
                        
                        // Modo consulta
                        if ($("#Modo").val() == "C") {
                            $("#RPPTipoPlan").val(Solicitud.TipoPlan.Id);
                        }

                        for (i = 0; i < Solicitud.Beneficiarios.length; i++) {
                            $("#TablaRPPGrupoFamiliar input[type=checkbox][value=" + Solicitud.Beneficiarios[i].IdGrupoFamiliar + "]").prop("checked",true);
                        }
                        actualizarContadorBeneficiariosMarcados();

                        if ($("#Modo").val() != "C") {
                            // Cargar Parámetros de RPP para la fecha de cotización obtenida
                            cargarParametros();
                        }
                        cargarTablaCotizaciones(false);
                        if ($("#Modo").val() != "N" && $("#Modo").val() != "D") {
                            cargarTablaBeneficiarios();
                        }
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ManejarError(XMLHttpRequest, textStatus, errorThrown);
                },
                complete: function () {
                    $("#TablaCotizacionesCargando").show();
                    $("#TablaCotizaciones").hide();
                    configurarMascaras();
                    actualizarCombobox();
                }
            });

            // EVENTOS

            // Combobox Tipo de Plan
            $("#RPPTipoPlan").change(function () {
                if ($(this).val() == "01") {
                    // FAMILIAR - Por defecto marcar todos los ckeckbox de la grilla de beneficiarios
                    $("#TablaRPPGrupoFamiliar input[type=checkbox]").each(function () {
                        if ($(this).data("parentesco") != "80") {
                            $(this).prop("checked", true);
                        }
                    });
                }
                else if ($(this).val() == "02") {
                    // INDIVIDUAL - Deshabilitar todos los beneficiarios salvo el afiliado
                    $("#TablaRPPGrupoFamiliar input[type=checkbox]").each(function () {
                        if ($(this).data("parentesco") != "80") {
                            $(this).prop("checked", false);
                        }
                    });
                }

                actualizarContadorBeneficiariosMarcados();
                cargarTablaCotizaciones(false);
            });

            // Combobox moneda
            $("#RPPMonedaPrimaUnica").change(function() {
                cargarTablaCotizaciones(false);
            });

            // Botón Cotizar
            $("#Cotizar").click(function () {
                // Validaciones
                limpiarValidaciones();

                // Prima única
                var bPrimaUnica = true;
                if ($("#RPPPrimaUnica").val() == "") {
                    $($("#rpp-prima-unica-helper").text("Campo obligatorio"));
                    bPrimaUnica = false;
                }
                else {
                    if ($("#RPPPrimaUnica").val() <= 0) {
                        $($("#rpp-prima-unica-helper").text("Valor no permitido"));
                        bPrimaUnica = false;
                    }
                }

                var bDCOM = true;
                if ($("#RPPDCOM").val() == "") {
                    $($("#rpp-dcom-helper").text("Campo obligatorio"));
                    bDCOM = false;
                }
                else {
                    // TODO: Validar según rol_dcom
                }

                var esValido = bPrimaUnica & bDCOM;

                if (esValido) {
                    abrirModalCargando("Cotizando...");

                    // Actualizar la solicitud con los valores del formulario
                    Solicitud.FechaCotizacion = cadenaAFecha($("#RPPFechaCotizacion").val());
                    Solicitud.FechaDevengue = cadenaAFecha($("#RPPFechaDevengue").val());
                    Solicitud.FechaVigencia = cadenaAFecha($("#RPPVigencia").val());
                    Solicitud.MonedaPrimaUnica.Id = $("#RPPMonedaPrimaUnica").val();
                    Solicitud.PrimaUnica = $("#RPPPrimaUnica").val().replaceAll(",", "");
                    Solicitud.PorcentajeDescuentoComision = $("#RPPDCOM").val();
                    Solicitud.MontoCIC = $("#MontoCIC").val().replaceAll(",", "");
                    Solicitud.TipoCambio = $("#TipoCambio").val().replaceAll(",", "");

                    // Actualizar los Beneficiarios seleccionados
                    let contador = 0;
                    beneficiariosSeleccionados = [];
                    $("#TablaRPPGrupoFamiliar input[type=checkbox]").each(function() {
                        if ($(this).is(":checked")) {
                            beneficiariosSeleccionados.push(contador);
                        }
                        contador++;
                    });

                    var params = {
                        tokenUsuario: $("#TokenUsuario").val(),
                        solicitud: Solicitud,
                        arrBeneficiarios: beneficiariosSeleccionados,
                        semilla: $("#Semilla").val()
                    };

                    $.ajax({
                        type: "POST",
                        url: "Cotizador.aspx/CotizarRPP",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSONstringifyConFechas(params),
                        success: function (data) {
                            Solicitud = data.d;
                            abrirModalAlerta("Mensaje", "Solicitud " + Solicitud.Id + " cotizada correctamente", "done", "#4caf50");
                            $("#RPPSolicitud").val(Solicitud.Id);
                            $("#Modo").val("M");
                            if ($("#DecargarDetalleSolicitud").length) {
	                            $("#DecargarDetalleSolicitud").show();
                                $("#DecargarDetalleSolicitud").prop("href", "<%=ResolveUrl("~/Reportes/ReportesRentaParticular.aspx?formato=6&solicitud=")%>" + Solicitud.Id);
                            }                            
                            cargarTablaCotizaciones(true);
                            cargarTablaBeneficiarios();
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            ManejarError(XMLHttpRequest, textStatus, errorThrown);
                        },
                        complete: function () {
                            cerrarModalCargando();
                        }
                    });
                }
                else {
                    scroll("ancla-solicitud");
                }
            });

            // Botón Agregar
            $(document).on("click", "#AgregarCotizacion", function () {
                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    cotizaciones: Solicitud.Cotizaciones
                };

                $.ajax({
                    type: "POST",
                    url: "Cotizador.aspx/AgregarCotizacionASolicitud",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(params),
                    success: function (data) {
                        Solicitud.Cotizaciones = data.d;+
                        cargarTablaCotizaciones(true);
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                    },
                    complete: function () {
                    }
                });
            });

            // Botón Eliminar
            $(document).on("click", "#TabCotizacionesRPP .eliminar-cotizacion", function () {
                idCotizacion = $(this).data("cotizacion");
                abrirModalConfirmacion("Confirmación", "¿Confirma la eliminación de la cotización <strong>N° " + idCotizacion + "</strong>?", "warning", "#fb8c00");
                return false;
            });

            // Botón Confirmar
            $("#modal-confirmar-aceptar").click(function (e) {
                e.preventDefault();
                Solicitud.Cotizaciones.splice(idCotizacion - 1, 1);
                cerrarModal("modal-confirmar");
                cargarTablaCotizaciones(true);
                return;
            });

            // Combobox Moneda
            $(document).on("change", ".rpp-moneda", function () {
                var tr = $(this).parent().parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val().split(":");
                Solicitud.Cotizaciones[indice - 1].Moneda.Id = valor[0];
                Solicitud.Cotizaciones[indice - 1].ValMonAju = valor[1];
            });

            // Combobox Meses tramo 1
            $(document).on("change", ".rpp-tramo1-meses", function () {
                var tr = $(this).parent().parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();
                Solicitud.Cotizaciones[indice - 1].PagoEscalonada = valor;
                var combobox = tr.find(".rpp-tramo1-porcentaje");
                if (valor != 0) {
                    // Activar combobox de porcentaje tramo 1, setear por default 200% y desactivar 100%
                    combobox.val("200");
                    Solicitud.Cotizaciones[indice - 1].PjePE = 10000.0 / 200;
                    combobox.prop("disabled", false);
                    combobox.find("option[value=0]").prop("disabled", true);
                    actualizarCombobox();
                }
                else {
                    // Setear el porcentaje tramo 1 en 100% y desactivar el control
                    combobox.find("option[value=0]").prop("disabled", false);
                    combobox.val("0");
                    Solicitud.Cotizaciones[indice - 1].PjePE = 0;
                    combobox.prop("disabled", true);
                    actualizarCombobox();
                }
            });

            // Combobox Porcentaje tramo 1
            $(document).on("change", ".rpp-tramo1-porcentaje", function () {
                var tr = $(this).parent().parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();
                Solicitud.Cotizaciones[indice - 1].PjePE = (10000.0 /valor).toFixed(6);
            });

            // Combobox Periodo Garantizado
            $(document).on("change", ".rpp-periodo-garantizado", function () {
                var tr = $(this).parent().parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();
                Solicitud.Cotizaciones[indice - 1].PeriodoGarantizado = valor;
            });

            // Combobox Porcentaje Cónyuge
            $(document).on("change", ".rpp-porcentaje-conyuge", function () {
                var tr = $(this).parent().parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();
                Solicitud.Cotizaciones[indice - 1].ValPjeConyuge = valor;
            });

            // Textbox DTRA
            $(document).on("change", ".rpp-dif-tra", function () {
                var tr = $(this).parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();
                if (valor == "") {
                    $(this).val("0.00");
                    valor = 0;
                }
                Solicitud.Cotizaciones[indice - 1].AjusteTRA = valor;
            });

            // Checkbox beneficiario
            $("#TablaRPPGrupoFamiliar input[type=checkbox]").change(function () {
                cargarTablaCotizaciones(false);

                if ($("#TablaRPPGrupoFamiliar input[type=checkbox]:checked").length > 1) {
                    // Familiar
                    $("#RPPTipoPlan").val("01");
                }
                else {
                    // Individual
                    $("#RPPTipoPlan").val("02");
                }

                actualizarContadorBeneficiariosMarcados();
            });
        });

        function limpiarValidaciones() {
            $(".helper-text").text("");
        }

        function mostrarPreloaderParametros() {
            $(".preloader-parametros").fadeIn(1000);
        }

        function ocultarPreloaderParametros() {
            $(".preloader-parametros").fadeOut(1000);
        }

        function evaluarDisponibilidadCotizar() {
            if (parametrosCargados && tablaCotizacionesCargada) {
                $("#Cotizar").removeClass("disabled");
            }
            else {
                $("#Cotizar").addClass("disabled");
            }
        }

        function cargarParametros() {
            mostrarPreloaderParametros();
            parametrosCargados = false;

            var params = {
                tokenUsuario: $("#TokenUsuario").val(),
                temporalidad: $("#RPPTemporalidad").val(),
                fechaCotizacion: $("#RPPFechaCotizacion").val()
            };

            $.ajax({
                type: "POST",
                url: "Cotizador.aspx/CargarParametrosRPP",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(params),
                success: function (data) {
                    parametrosCargados = true;
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ManejarError(XMLHttpRequest, textStatus, errorThrown);
                },
                complete: function () {
                    ocultarPreloaderParametros();
                    evaluarDisponibilidadCotizar();
                }
            });
        }

        function cargarTablaCotizaciones(anclar=false) {
            // Cargar Tabla de Cotizaciones
            $("#TablaCotizacionesCargando").show();
            $("#TablaCotizaciones").hide();
            $("#RPPMonedaPrimaUnica").prop("disabled", true);
            tablaCotizacionesCargada = false;
            evaluarDisponibilidadCotizar();

            // Evaluar flag de cónyuge
            var bConyuge = false;
            $("#TablaRPPGrupoFamiliar input[type=checkbox]").each(function () {
                if ($(this).data("parentesco") == "10" && $(this).is(":checked")) bConyuge = true;
                if (($(this).data("parentesco") == "30" || $(this).data("parentesco") == "40") && $(this).is(":checked")) bConyuge = false;
            });
            if (!bConyuge) {
                Solicitud.Cotizaciones.forEach(c => c.ValPjeConyuge = 0);
            }

            var params = {
                tokenUsuario: $("#TokenUsuario").val(),
                cotizaciones: Solicitud.Cotizaciones,
                temporalidad: $("#RPPTemporalidad").val(),
                moneda: $("#RPPMonedaPrimaUnica").val(),
                conyuge: bConyuge,
                modo: $("#Modo").val(),
                fechaCotizacion: $("#RPPFechaCotizacion").val()
            };

            $.ajax({
                type: "POST",
                url: "Cotizador.aspx/CargarTablaCotizaciones",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSONstringifyConFechas(params),
                success: function (data) {
                    $("#TablaCotizaciones").html($(data.d).find("#ContenidoDinamico").html());
                    tablaCotizacionesCargada = true;
                    if (anclar) {
                        scroll("ancla-cotizaciones");
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ManejarError(XMLHttpRequest, textStatus, errorThrown);
                },
                complete: function () {
                    $("#RPPMonedaPrimaUnica").prop("disabled", false);
                    configurarMascaras();
                    actualizarCombobox();
                    $("#TablaCotizaciones").show();
                    $("#TablaCotizacionesCargando").hide();
                    $(".rpp-moneda").trigger("change");
                    evaluarDisponibilidadCotizar();
                }
            });
        }

        function cargarTablaBeneficiarios() {
            // Cargar Tabla de Cotizaciones
            $("#TablaBeneficiariosCargando").show();
            $("#TablaBeneficiarios").hide();
            tablaBeneficiariosCargada = false;

            var params = {
                tokenUsuario: $("#TokenUsuario").val(),
                beneficiarios: Solicitud.Beneficiarios
            };

            $.ajax({
                type: "POST",
                url: "Cotizador.aspx/CargarTablaBeneficiarios",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSONstringifyConFechas(params),
                success: function (data) {
                    $("#TablaBeneficiarios").html($(data.d).find("#ContenidoDinamico").html());
                    $("#beneficiarios-solicitud").show();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ManejarError(XMLHttpRequest, textStatus, errorThrown);
                },
                complete: function () {
                    configurarMascaras();
                    actualizarCombobox();

                    $("#TablaBeneficiarios").show();
                    $("#TablaBeneficiariosCargando").hide();
                }
            });
        }

        function actualizarContadorBeneficiariosMarcados() {
            $("#total-beneficiarios").text($("#TablaRPPGrupoFamiliar input[type=checkbox]:checked").length);
        }
    </script>
</asp:Content>
