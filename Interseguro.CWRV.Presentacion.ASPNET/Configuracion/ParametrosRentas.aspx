<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="ParametrosRentas.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Configuracion.ParametrosRentas" %>
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

        .no-cotizable {
            background-color: #ffcdd2;
        }

        .seleccionada {
            background-color: #c8e6c9;
        }

        .modificar-parametro {
            cursor: pointer;
        }

        .datepicker-controls .select-month input {
            width: 80px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="row">
    <div class="col s12">
        <div class="card">
            <div class="card-content">
                <span class="card-title blue-text text-darken-3">
                    Parámetros de Rentas
                </span>
                <div class="row">
                    <%--<div class="col s12">
                        <ul class="tabs">
                            <li class="tab col s3"><a href="#valpar">Cotizador</a></li>
                            <li class="tab col s3"><a href="#parash">Asset Share</a></li>
                            <li class="tab col s3"><a href="#parinv">Inversiones</a></li>
                            <li class="tab col s3"><a href="#tabmor">Tablas de Mortalidad</a></li>
                        </ul>
                    </div>--%>
                    <div id="valpar" class="col s12">
                        <div id ="cabeceraBuscar">
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">shopping_cart</i>
                                    <asp:DropDownList ID="Producto" runat="server" ClientIDMode="Static">
                                    </asp:DropDownList>
                                    <label for="Producto">Producto</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <i class="material-icons prefix">calendar_today</i>
                                    <asp:TextBox ID="Fecha" runat="server" ClientIDMode="Static" CssClass="validate fecha datepicker"></asp:TextBox>
                                    <label for="Fecha">Fecha</label>
                                </div>
                            </div>
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">category</i>
                                    <asp:DropDownList ID="ParametroRentas" runat="server" ClientIDMode="Static">
                                    </asp:DropDownList>
                                    <label for="ParametroRentas">Parámetro</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <i class="material-icons prefix">price_change</i>
                                    <asp:DropDownList ID="Moneda" runat="server" ClientIDMode="Static">
                                    </asp:DropDownList>
                                    <label for="Moneda">Moneda</label>
                                </div>
                            </div>
                        </div>

                        <div id="cabeceraInsertar">
                            <div class="row">
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">shopping_cart</i>
                                    <asp:DropDownList ID="ProductoInsertar" runat="server" ClientIDMode="Static">
                                    </asp:DropDownList>
                                    <label for="Producto">Producto</label>
                                </div>
                                <div class="input-field col s12 m6">
                                    <a name="ancla-solicitud"></a>
                                    <i class="material-icons prefix">category</i>
                                    <asp:DropDownList ID="ParametroRentasInsertar" runat="server" ClientIDMode="Static">
                                    </asp:DropDownList>
                                    <label for="ParametroRentas">Parámetro</label>
                                </div>
                            </div>
                        </div>
                        
                        <div class="row center">
                            <div class="col s12">
                                <asp:HyperLink ID="Buscar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">search</i>Buscar</asp:HyperLink>
                                <asp:HyperLink ID="Nuevo" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">add</i>Nuevo</asp:HyperLink>
                            </div>
                        </div>
                        <div class="row center">
                            <a name="ancla-parametros"></a>
                            <div id="TablaParametros" class="col s12 black-text center-align" style="display:none">
                                aaa
                            </div>
                            <div id="TablaParametrosCargando" class="col s12 valign-wrapper center" style="display:none">
                                <div class="preloader-wrapper small active" style="width:24px;height:24px">
                                    <div class="spinner-layer spinner-blue-only">
                                        <div class="circle-clipper left">
                                            <div class="circle"></div>
                                        </div>
                                        <div class="gap-patch">
                                            <div class="circle"></div>
                                        </div>
                                        <div class="circle-clipper right">
                                            <div class="circle"></div>
                                        </div>
                                    </div>
                                </div>
                                <span style="margin-left:10px">
                                    Cargando parámetros...
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        var Parametros = null;
        var ParametrosEdicion = null;
        var tablaParametrosCargada = false;
        $(document).ready(function () {
            $("#cabeceraInsertar").hide();

            // Botón Buscar
            $("#Buscar").click(function (e) {
                e.preventDefault();
                // Cargar Tabla de Cotizaciones
                $("#TablaParametrosCargando").show();
                $("#TablaParametros").hide();
                deshabilitarBotones();
                tablaParametrosCargada = false;

                $("#ProductoInsertar").val($("#Producto").val());
                $("#ParametroRentasInsertar").val($("#ParametroRentas").val());

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    producto: $("#Producto").val(),
                    fecha: $("#Fecha").val(),
                    parametro: $("#ParametroRentas").val(),
                    moneda: $("#Moneda").val()
                };

                $.ajax({
                    type: "POST",
                    url: "ParametrosRentas.aspx/ListarParametros",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSONstringifyConFechas(params),
                    success: function (data) {
                        Parametros = data.d;
                        ParametrosEdicion = structuredClone(Parametros);
                        cargarTablaParametros("consulta");
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                    },
                    complete: function () {
                        configurarMascaras();
                        actualizarCombobox();
                        habilitarBotones();
                        $("#TablaParametros").show();
                        $("#TablaParametrosCargando").hide();
                    }
                });
            });

            // Botón Nuevo
            $("#Nuevo").click(function (e) {
                e.preventDefault();

                $('#ProductoInsertar').prop("disabled", false);
                $("#ParametroRentasInsertar").prop("disabled", false);

                $("#ProductoInsertar").val($("#Producto").val());
                $("#ParametroRentasInsertar").val($("#ParametroRentas").val());

                tablaParametrosCargada = false;
                ParametrosEdicion = [{
                    fec_ini_rango: obtenerFechaActual(),
                    fec_fin_rango: obtenerFechaActual(),
                    cod_parametro: $("#ParametroRentasInsertar").val()
                }];

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    parametros: ParametrosEdicion
                };

                $.ajax({
                    type: "POST",
                    url: "ParametrosRentas.aspx/AgregarParametro",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSONstringifyConFechas(params),
                    success: function (data) {
                        ParametrosEdicion = data.d;
                        cargarTablaParametros("insertar");
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                    },
                    complete: function () {
                        configurarMascaras();
                        actualizarCombobox();
                        habilitarBotones();
                        $("#ModoConsulta").hide();
                        $("#ModoEditar").show();
                        $("#TablaParametros").show();
                        $("#TablaParametrosCargando").hide();
                        $("#cabeceraBuscar").hide();
                        $("#cabeceraInsertar").show();
                    }
                });
            });

            // Botón Editar
            $(document).on("click", ".modificar-parametro", function () {
                cargarTablaParametros("insertar").then(() => {
                    $("#ModoConsulta").hide();
                    $("#ModoEditar").show();
                    $("#cabeceraBuscar").hide();
                    $("#cabeceraInsertar").show();

                    $('#ProductoInsertar').prop("disabled", true);
                    $("#ParametroRentasInsertar").prop("disabled", true);
                    $('.par-fechainicio').prop("disabled", true);
                    $('.par-fechafin').prop("disabled", true);
                });


            });

            // Cambiar fechas en grillas
            $(document).on("change", ".par-fechainicio", function () {
                // Replicar valor en todo el bloque
                var valor = $(this).val();
                $(".par-fechainicio").val(valor);

                var cantidadSlash = valor.split("/").length - 1;
                if (cantidadSlash == 2) {
                    var valorFormateado = valor.substr(3, 2) + "/" + valor.substr(0, 2) + "/" + valor.substr(6, 4)

                    for (i = 0; i < ParametrosEdicion.length; i++) {
                        ParametrosEdicion[i].fec_ini_rango = valorFormateado;
                    }
                }
            });
            $(document).on("change", ".par-fechafin", function () {
                // Replicar valor en todo el bloque
                var valor = $(this).val();
                $(".par-fechafin").val(valor);

                var cantidadSlash = valor.split("/").length - 1;
                if (cantidadSlash == 2) {
                    var valorFormateado = valor.substr(3, 2) + "/" + valor.substr(0, 2) + "/" + valor.substr(6, 4)

                    for (i = 0; i < ParametrosEdicion.length; i++) {
                        ParametrosEdicion[i].fec_fin_rango = valorFormateado;
                    }
                }
            });

            // Combobox Moneda
            $(document).on("change", ".par-moneda", function () {
                var tr = $(this).parent().parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();

                ParametrosEdicion[indice - 1].cod_moneda = valor;
            });

            // Combobox Temporalidad
            $(document).on("change", ".par-temporalidad", function () {
                var tr = $(this).parent().parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();

                ParametrosEdicion[indice - 1].cod_tipo_temporalidad = valor;
            });

            // Textbox Tramo
            $(document).on("change", ".par-tramo", function () {
                var tr = $(this).parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();

                ParametrosEdicion[indice - 1].num_tramo = valor;

            });

            // Textbox Valor
            $(document).on("change", ".par-valor", function () {
                var tr = $(this).parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();

                ParametrosEdicion[indice - 1].val_parametro = Number(valor.replace(",", ""));
            });

            // Combobox Origen
            $(document).on("change", ".par-origen", function () {
                var tr = $(this).parent().parent().parent();
                var indice = tr.children(":first").children("span").html();
                var valor = $(this).val();

                ParametrosEdicion[indice - 1].ind_origen = valor;
            });

            // Botón Cancelar Edición
            $(document).on("click", "#ModificarCancelar", function () {
                ParametrosEdicion = structuredClone(Parametros);
                cargarTablaParametros("consulta");

                $("#ModoConsulta").show();
                $("#ModoEditar").hide();
                $("#cabeceraBuscar").show();
                $("#cabeceraInsertar").hide();

                $('par-fechainicio').prop("disabled", false);
                $('par-fechafin').prop("disabled", false);
            });

            // Botón Guardar Edición
            $(document).on("click", "#ModificarGuardar", function () {
                abrirModalConfirmacion("Confirmación", "¿Confirma que desea guardar los parámetros ingresados?", "report_problem", "#ffab00");
                return false;
            });

            // Botón Confirmar
            $("#modal-confirmar-aceptar").click(function (e) {
                abrirModalCargando("Guardando los cambios, espere por favor...");
                //TODO...
                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    producto: $("#Producto").val(),
                    parametros: ParametrosEdicion
                };

                $.ajax({
                    type: "POST",
                    url: "ParametrosRentas.aspx/ActualizarParametro",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSONstringifyConFechas(params),
                    success: function (data) {
                        abrirModalAlerta("Satisfactorio", "Se registraron los parámetros correctamente", "done", "#4caf50");
                        cargarTablaParametros("consulta");
                        $("#cabeceraBuscar").show();
                        $("#cabeceraInsertar").hide();
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                    },
                    complete: function () {
                        configurarMascaras();
                        actualizarCombobox();
                        habilitarBotones();
                        $("#ModoConsulta").hide();
                        $("#ModoEditar").show();
                        $("#TablaParametros").show();
                        $("#TablaParametrosCargando").hide();
                        cerrarModalCargando();
                    }
                });
            });

            // Botón Agregar
            $(document).on("click", "#AgregarParametro", function () {
                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    parametros: ParametrosEdicion
                };

                $.ajax({
                    type: "POST",
                    url: "ParametrosRentas.aspx/AgregarParametro",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSONstringifyConFechas(params),
                    success: function (data) {
                        ParametrosEdicion = data.d;
                        cargarTablaParametros("insertar");
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                    },
                    complete: function () {
                        configurarMascaras();
                        actualizarCombobox();
                        habilitarBotones();
                        $("#ModoConsulta").hide();
                        $("#ModoEditar").show();
                        $("#TablaParametros").show();
                        $("#TablaParametrosCargando").hide();
                    }
                });
            });
        });

        async function cargarTablaParametros(modo) {
            $("#TablaParametrosCargando").show();
            $("#TablaParametros").hide();
            deshabilitarBotones();
            tablaParametrosCargada = false;

            var params = {
                tokenUsuario: $("#TokenUsuario").val(),
                producto: $("#Producto").val(),
                moneda: $("#Moneda").val(),
                parametros: Parametros
            };

            if (modo == "consulta") {
                params.parametros = Parametros
            }
            else {
                params.parametros = ParametrosEdicion
            }

            await $.ajax({
                type: "POST",
                url: "ParametrosRentas.aspx/CargarTablaParametros",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSONstringifyConFechas(params),
                success: function (data) {
                    $("#TablaParametros").html($(data.d).find("#ContenidoDinamico").html());
                    tablaParametrosCargada = true;

                    if (modo == "consulta") {
                        $("#ModoConsulta").show();
                        $("#ModoEditar").hide();
                    }
                    else {
                        $("#ModoConsulta").hide();
                        $("#ModoEditar").show();
                    }


                    // Inicializar fechas
                    configurarMascaras();
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    ManejarError(XMLHttpRequest, textStatus, errorThrown);
                },
                complete: function () {
                    configurarMascaras();
                    actualizarCombobox();
                    habilitarBotones();
                    $("#TablaParametros").show();
                    $("#TablaParametrosCargando").hide();
                }
            });
        }

        function obtenerFechaActual() {
            const today = new Date();
            const yyyy = today.getFullYear();
            let mm = today.getMonth() + 1; // Months start at 0!
            let dd = today.getDate();

            if (dd < 10) dd = '0' + dd;
            if (mm < 10) mm = '0' + mm;

            let formattedToday = yyyy + '/' + mm + '/' + dd;
            return formattedToday;

        }
    </script>
</asp:Content>