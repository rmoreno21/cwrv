<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="BeneficiarioCierre.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.BeneficiarioCierre" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" ClientIDMode="Static">
    <asp:HiddenField ID="Solicitud" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="FechaCotizacion" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="Correlativo" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="Apoderado" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="FirmaDigital" runat="server" ClientIDMode="Static" />

    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3" style="line-height:20px"><i class="small material-icons">person</i> <asp:Literal ID="Titulo" runat="server"></asp:Literal></span>

                    <blockquote id="Mensaje" class="info">
                        <span id="instrucciones">
                            Por favor complete todos los campos que se muestran a continuación y verifique que toda la
                            información es correcta antes de presionar el botón <b>Guardar</b>. Use el botón <b>Enviar VCTP</b>
                            para mandar el correo electrónico con el enlace para que el cliente pueda firmar digitalmente su
                            solicitud.
                        </span>
                    </blockquote>
                    
                    <div class="row">
                        <a name="ApellidoPaternoAncla"></a>
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="ApellidoPaterno" runat="server" ClientIDMode="Static" MaxLength="40" style="text-transform: uppercase"></asp:TextBox>
                            <label for="ApellidoPaterno">Apellido Paterno</label>
                            <span id="ApellidoPaternoHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <a name="ApellidoMaternoAncla"></a>
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="ApellidoMaterno" runat="server" ClientIDMode="Static" MaxLength="40" style="text-transform: uppercase"></asp:TextBox>
                            <label for="ApellidoMaterno">Apellido Materno</label>
                            <span id="ApellidoMaternoHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>

                    <div class="row">
                        <a name="NombresAncla"></a>
                        <div class="input-field col s12">
                            <asp:TextBox ID="Nombres" runat="server" ClientIDMode="Static" MaxLength="80" style="text-transform: uppercase"></asp:TextBox>
                            <label for="Nombres">Nombres</label>
                            <span id="NombresHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>

                    <div class="row">
                        <a name="TipoDocumentoAncla"></a>
                        <div class="input-field col s12 m6">
                            <asp:DropDownList ID="TipoDocumento" runat="server"></asp:DropDownList>
                            <label for="TipoDocumento">Tipo de Documento</label>
                            <span id="TipoDocumentoHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <a name="NumeroDocumentoAncla"></a>
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="NumeroDocumento" runat="server" ClientIDMode="Static" MaxLength="20"></asp:TextBox>
                            <label for="NumeroDocumento">Número de Documento</label>
                            <span id="NumeroDocumentoHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>

                    <div class="row">
                        <a name="TelefonoAncla"></a>
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="Telefono" CssClass="enteroPositivo" runat="server" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
                            <label for="Telefono">Teléfono</label>
                            <span id="TelefonoHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <a name="CelularAncla"></a>
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="Celular" CssClass="enteroPositivo" runat="server" ClientIDMode="Static" MaxLength="9"></asp:TextBox>
                            <label for="Celular">Celular</label>
                            <span id="CelularHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>

                    <div class="row">
                        <a name="CorreoElectronicoAncla"></a>
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="CorreoElectronico" TextMode="Email" runat="server" ClientIDMode="Static" MaxLength="250"></asp:TextBox>
                            <label for="CorreoElectronico">Correo Electrónico</label>
                            <span id="CorreoElectronicoHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <a name="CentroLaboralAncla"></a>
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="CentroLaboral" runat="server" ClientIDMode="Static" MaxLength="250"></asp:TextBox>
                            <label for="CentroLaboral">Último Centro de Labores</label>
                            <span id="CentroLaboralHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3" style="line-height:20px"><i class="small material-icons">apartment</i> Datos de Dirección</span>

                    <div class="row">
                        <a name="TipoViaAncla"></a>
                        <div class="input-field col s12 m3">
                            <asp:DropDownList ID="TipoVia" runat="server"></asp:DropDownList>
                            <label for="TipoVia">Tipo de Vía</label>
                            <span id="TipoViaHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <a name="DireccionAncla"></a>
                        <div class="input-field col s12 m6">
                            <asp:TextBox ID="Direccion" runat="server" ClientIDMode="Static" MaxLength="250"></asp:TextBox>
                            <label for="Direccion">Direccion</label>
                            <span id="DireccionHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <a name="EspacioUrbanoAncla"></a>
                        <div class="input-field col s12 m3">
                            <asp:TextBox ID="EspacioUrbano" runat="server" ClientIDMode="Static" MaxLength="250"></asp:TextBox>
                            <label for="EspacioUrbano">N°/Mz/Lt/Dpto</label>
                            <span id="EspacioUrbanoHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>

                    <div class="row">
                        <a name="DepartamentoAncla"></a>
                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="Departamento" runat="server"></asp:DropDownList>
                            <label for="Departamento">Departamento</label>
                            <span id="DepartamentoHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <a name="ProvinciaAncla"></a>
                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="Provincia" runat="server"></asp:DropDownList>
                            <label for="Provincia">Provincia</label>
                            <span id="ProvinciaHelper" class="helper-text red-text" data-error=""></span>
                        </div>

                        <a name="DistritoAncla"></a>
                        <div class="input-field col s12 m4">
                            <asp:DropDownList ID="Distrito" runat="server"></asp:DropDownList>
                            <label for="Distrito">Distrito</label>
                            <span id="DistritoHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col s12">
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3" style="line-height:20px"><i class="small material-icons">email</i> Datos de Envío de Póliza</span>

                    <div class="row">
                        <a name="EnvioPolizaAncla"></a>
                        <div class="input-field col s12">
                            <asp:DropDownList ID="EnvioPoliza" runat="server"></asp:DropDownList>
                            <label for="EnvioPoliza">Envío de Póliza</label>
                            <span id="EnvioPolizaHelper" class="helper-text red-text" data-error=""></span>
                        </div>
                    </div>

                    <div id="EnvioFisico" style="display:none">
                        <div class="row">
                            <a name="PersonaAutorizadaEntregaAncla"></a>
                            <div class="input-field col s12">
                                <asp:TextBox ID="PersonaAutorizadaEntrega" runat="server" ClientIDMode="Static" MaxLength="250"></asp:TextBox>
                                <label for="PersonaAutorizadaEntrega">Persona Autorizada</label>
                                <span id="PersonaAutorizadaEntregaHelper" class="helper-text red-text" data-error=""></span>
                            </div>
                        </div>

                        <div class="row">
                            <a name="TipoViaEntregaAncla"></a>
                            <div class="input-field col s12 m3">
                                <asp:DropDownList ID="TipoViaEntrega" runat="server" ClientIDMode="Static"></asp:DropDownList>
                                <label for="TipoViaEntrega">Tipo de Vía</label>
                                <span id="TipoViaEntregaHelper" class="helper-text red-text" data-error=""></span>
                            </div>

                            <a name="DireccionEntregaAncla"></a>
                            <div class="input-field col s12 m6">
                                <asp:TextBox ID="DireccionEntrega" runat="server" ClientIDMode="Static" MaxLength="250"></asp:TextBox>
                                <label for="DireccionEntrega">Direccion</label>
                                <span id="DireccionEntregaHelper" class="helper-text red-text" data-error=""></span>
                            </div>

                            <a name="EspacioUrbanoEntregaAncla"></a>
                            <div class="input-field col s12 m3">
                                <asp:TextBox ID="EspacioUrbanoEntrega" runat="server" ClientIDMode="Static" MaxLength="250"></asp:TextBox>
                                <label for="EspacioUrbanoEntrega">N°/Mz/Lt/Dpto</label>
                                <span id="EspacioUrbanoEntregaHelper" class="helper-text red-text" data-error=""></span>
                            </div>
                        </div>

                        <div class="row">
                            <a name="DepartamentoEntregaAncla"></a>
                            <div class="input-field col s12 m4">
                                <asp:DropDownList ID="DepartamentoEntrega" runat="server"></asp:DropDownList>
                                <label for="DepartamentoEntrega">Departamento</label>
                                <span id="DepartamentoEntregaHelper" class="helper-text red-text" data-error=""></span>
                            </div>

                            <a name="ProvinciaEntregaAncla"></a>
                            <div class="input-field col s12 m4">
                                <asp:DropDownList ID="ProvinciaEntrega" runat="server"></asp:DropDownList>
                                <label for="ProvinciaEntrega">Provincia</label>
                                <span id="ProvinciaEntregaHelper" class="helper-text red-text" data-error=""></span>
                            </div>

                            <a name="DistritoEntregaAncla"></a>
                            <div class="input-field col s12 m4">
                                <asp:DropDownList ID="DistritoEntrega" runat="server"></asp:DropDownList>
                                <label for="DistritoEntrega">Distrito</label>
                                <span id="DistritoEntregaHelper" class="helper-text red-text" data-error=""></span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col s12 center">
            <asp:HyperLink ID="Guardar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">save</i>Guardar</asp:HyperLink>
            <asp:HyperLink ID="Enviar" ClientIDMode="Static" runat="server" NavigateUrl="#modal-confirmar" CssClass="waves-effect waves-light btn blue darken-2 modal-trigger" style="display:none"><i class="material-icons left">send</i>Enviar</asp:HyperLink>
            <asp:HyperLink ID="EnvioManual" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2" Visible="false"><i class="material-icons left">forward_to_inbox</i>Envío Manual</asp:HyperLink>
            <asp:HyperLink ID="Regresar" NavigateUrl="~/Cotizador/Cotizador.aspx#datos_solicitud" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">arrow_back</i>Regresar</asp:HyperLink>
        </div>
    </div>

    <!-- Modal Structure -->
    <div id="modal-confirmar" class="modal">
        <div class="modal-content">
            <h5>Firma Digital de documentos de <span class="red-text text-darken-4">Rentas Vitalicias</span></h5>
            <p>
                Usted va a enviar el enlace de Firma Digital a <b id="NombreCliente" class="red-text text-darken-4"></b>
                cuyo documento de identidad es <b id="TipoDocumentoCliente" class="red-text text-darken-4"></b>
                <b id="NumeroDocumentoCliente" class="red-text text-darken-4"></b> al siguiente correo electrónico:
                <b id="CorreoElectronicoCliente" class="red-text text-darken-4"></b><br>
                <br>
                Para que el cliente pueda firmar digitalmente sus documentos se le enviará una clave al siguiente
                número de celular: <b id="CelularCliente" class="red-text text-darken-4"></b>.<br>
                <br>
                Para evitar inconvenientes por favor <b>valide cuidadosamente</b> que toda la información del
                cliente es correcta antes de enviarle el enlace de Firma Digital.
            </p>
        </div>
        <div class="modal-footer">
            <button id="ConfirmarEnvio" class="waves-effect waves-light btn blue darken-2">
                Confirmar Envío
            </button>

            <button id="Cancelar" class="waves-effect waves-light btn blue darken-2">
                Cancelar
            </button>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
    <script>
        var nombreCliente = "<% Response.Write(string.Format("{0} {1} {2}", beneficiario.Nombre, beneficiario.ApellidoPaterno, beneficiario.ApellidoMaterno)); %>";
        var tipoDocumentoCliente = "<% Response.Write(beneficiario.Identificacion.GlosaTipo); %>";
        var numeroDocumentoCliente = "<% Response.Write(beneficiario.Identificacion.Numero); %>";
        var correoElectronicoCliente = "<% Response.Write(beneficiario.CorreoElectronico); %>";
        var celularCliente = "<% Response.Write(beneficiario.numCelular); %>";

        $(document).ready(function () {
            EvaluarEnvioPoliza();
            CompletarVariablesCliente();
            DeshabilitarSeleccione();
            EvaluarBotonEnviar();

            $("#EnvioPoliza").change(function () {
                EvaluarEnvioPoliza()
            });

            $("#Guardar").click(function () {
                deshabilitarBotones();
                abrirModalCargando("Guardando la información, por favor espere un momento...");
                if (ValidarFormulario()) {
                    var params = {
                        tokenUsuario: $("#TokenUsuario").val(),
                        numSolicitud: $("#Solicitud").val(),
                        fecCotizacion: $("#FechaCotizacion").val(),
                        correlativo: $("#Correlativo").val(),
                        apellidoPaterno: $("#ApellidoPaterno").val().trim(),
                        apellidoMaterno: $("#ApellidoMaterno").val().trim(),
                        nombres: $("#Nombres").val().trim(),
                        tipoDocumento: $("#TipoDocumento").val(),
                        numeroDocumento: $("#NumeroDocumento").val(),
                        telefono: $("#Telefono").val(),
                        celular: $("#Celular").val(),
                        correoElectronico: $("#CorreoElectronico").val().trim(),
                        centroLaboral: $("#CentroLaboral").val().trim(),
                        apoderado: $("#Apoderado").val(),
                        tipoVia: $("#TipoVia").val(),
                        direccion: $("#Direccion").val().trim(),
                        espacioUrbano: $("#EspacioUrbano").val().trim(),
                        departamento: $("#Departamento").val(),
                        provincia: $("#Provincia").val(),
                        distrito: $("#Distrito").val(),
                        envioPoliza: $("#EnvioPoliza").val(),
                        personaAutorizadaEntrega: $("#PersonaAutorizadaEntrega").val().trim(),
                        tipoViaEntrega: $("#TipoViaEntrega").val(),
                        direccionEntrega: $("#DireccionEntrega").val().trim(),
                        espacioUrbanoEntrega: $("#EspacioUrbanoEntrega").val().trim(),
                        departamentoEntrega: $("#DepartamentoEntrega").val(),
                        provinciaEntrega: $("#ProvinciaEntrega").val(),
                        distritoEntrega: $("#DistritoEntrega").val()
                    };

                    $.ajax({
                        type: "POST",
                        url: "BeneficiarioCierre.aspx/GuardarBeneficiario",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(params),
                        success: function (data) {
                            var beneficiario = data.d;
                            abrirModalAlerta("Mensaje", "Datos guardados correctamente. Para mandar el enlace de Firma Digital al cliente use el botón <b>Enviar</b>.", "done", "#4caf50");
                            nombreCliente = beneficiario.Nombre + " " + beneficiario.ApellidoPaterno + " " + beneficiario.ApellidoMaterno;
                            tipoDocumentoCliente = beneficiario.Identificacion.GlosaTipo;
                            numeroDocumentoCliente = beneficiario.Identificacion.Numero;
                            correoElectronicoCliente = beneficiario.CorreoElectronico;
                            celularCliente = beneficiario.numCelular;
                            CompletarVariablesCliente();
                            $("#Enviar").show().prop("disabled", false).removeClass("disabled");
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            ManejarError(XMLHttpRequest, textStatus, errorThrown);
                            $("#Enviar").hide();
                        },
                        complete: function () {
                            habilitarBotones();
                            cerrarModalCargando();
                        }
                    });
                }
                else {
                    habilitarBotones();
                    cerrarModalCargando();
                    return false;
                }
            });

            $("#ConfirmarEnvio").click(function (e) {
                e.preventDefault();
                deshabilitarBotones();
                abrirModalCargando("Enviando enlace de Firma Digital, por favor espere un momento...");

                if (ValidarFormulario()) {
                    var params = {
                        tokenUsuario: $("#TokenUsuario").val(),
                        numSolicitud: $("#Solicitud").val(),
                        fecCotizacion: $("#FechaCotizacion").val(),
                        correlativo: $("#Correlativo").val()
                    };

                    $.ajax({
                        type: "POST",
                        url: "BeneficiarioCierre.aspx/EnviarFirmaDigital",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(params),
                        success: function () {
                            abrirModalAlerta("Mensaje", "Enlace de Firma Digital enviado correctamente al cliente.", "done", "#4caf50");
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            ManejarError(XMLHttpRequest, textStatus, errorThrown);
                        },
                        complete: function () {
                            cerrarModal("modal-confirmar");
                            habilitarBotones();
                            cerrarModalCargando();
                        }
                    });
                }
                else {
                    habilitarBotones();
                    cerrarModalCargando();
                    return false;
                }
            });

            $("#Cancelar").click(function (e) {
                e.preventDefault();
                cerrarModal("modal-confirmar");
            });

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
                    url: "BeneficiarioCierre.aspx/ListarProvincias",
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

            $("#DepartamentoEntrega").change(function () {
                $("#ProvinciaEntrega").prop("disabled", true);
                $("#ProvinciaEntrega").val("0");
                $("#DistritoEntrega").prop("disabled", true);
                $("#DistritoEntrega").val("0");
                actualizarCombobox();

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    idDepartamento: $("#DepartamentoEntrega").val()
                };

                $.ajax({
                    type: "POST",
                    url: "BeneficiarioCierre.aspx/ListarProvincias",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(params),
                    success: function (data) {
                        var provincias = data.d;
                        PoblarCombobox("#ProvinciaEntrega", provincias);
                        $("#ProvinciaEntrega").prop("disabled", false);
                        $("#ProvinciaEntrega").val("0");
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
                    url: "BeneficiarioCierre.aspx/ListarDistritos",
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

            $("#ProvinciaEntrega").change(function () {
                $("#DistritoEntrega").prop("disabled", true);
                $("#DistritoEntrega").val("0");
                actualizarCombobox();

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    idProvincia: $("#ProvinciaEntrega").val()
                };

                $.ajax({
                    type: "POST",
                    url: "BeneficiarioCierre.aspx/ListarDistritos",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: JSON.stringify(params),
                    success: function (data) {
                        var distritos = data.d;
                        PoblarCombobox("#DistritoEntrega", distritos);
                        $("#DistritoEntrega").prop("disabled", false);
                        $("#DistritoEntrega").val("0");
                        actualizarCombobox();
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        ManejarError(XMLHttpRequest, textStatus, errorThrown);
                    }
                });
            });

            $("input").keydown(function () {
                $("#Enviar").prop("disabled", true).addClass("disabled");
            });

            $("select").change(function () {
                $("#Enviar").prop("disabled", true).addClass("disabled");
            });

            function ValidarFormulario() {
                // Validaciones
                limpiarErrores();
                var ancla = "";

                // Apellido Paterno
                var bApellidoPaterno = true;
                if ($("#ApellidoPaterno").val().trim() == "") {
                    $("#ApellidoPaternoHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "ApellidoPaternoAncla";
                    bApellidoPaterno = false;
                }

                // Apellido Materno
                var bApellidoMaterno = true;
                if ($("#ApellidoMaterno").val().trim() == "") {
                    $("#ApellidoMaternoHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "ApellidoMaternoAncla";
                    bApellidoMaterno = false;
                }

                // Nombres
                var bNombres = true;
                if ($("#Nombres").val().trim() == "") {
                    $("#NombresHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "NombresAncla";
                    bNombres = false;
                }

                // Tipo de Documento
                var bTipoDocumento = true;
                if ($("#TipoDocumento").val() == null || $("#TipoDocumento").val() == "0") {
                    $("#TipoDocumentoHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "TipoDocumentoAncla";
                    bTipoDocumento = false;
                }

                // Número de Documento
                var bNumeroDocumento = true;
                if ($("#NumeroDocumento").val().trim() == "") {
                    $("#NumeroDocumentoHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "NumeroDocumentoAncla";
                    bNumeroDocumento = false;
                }

                // Teléfono
                var bTelefono = true;
                if ($("#Telefono").val().trim() == "") {
                    $("#TelefonoHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "TelefonoAncla";
                    bTelefono = false;
                }

                // Celular
                var bCelular = true;
                if ($("#Celular").val().trim() == "") {
                    $("#CelularHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "CelularAncla";
                    bCelular = false;
                }

                // Correo Electrónico
                var bCorreoElectronico = true;
                if ($("#CorreoElectronico").val().trim() == "") {
                    $("#CorreoElectronicoHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "CorreoElectronicoAncla";
                    bCorreoElectronico = false;
                }

                // Centro Laboral
                var bCentroLaboral = true;
                if ($("#CentroLaboral").val().trim() == "") {
                    $("#CentroLaboralHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "CentroLaboralAncla";
                    bCentroLaboral = false;
                }

                // Tipo de Vía
                var bTipoVia = true;
                if ($("#TipoVia").val() == null || $("#TipoVia").val() == "0") {
                    $("#TipoViaHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "TipoViaAncla";
                    bTipoVia = false;
                }

                // Dirección
                var bDireccion = true;
                if ($("#Direccion").val().trim() == "") {
                    $("#DireccionHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "DireccionAncla";
                    bDireccion = false;
                }

                // Espacio Urbano
                var bEspacioUrbano = true;
                if ($("#EspacioUrbano").val().trim() == "") {
                    $("#EspacioUrbanoHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "EspacioUrbanoAncla";
                    bEspacioUrbano = false;
                }

                // Departamento
                var bDepartamento = true;
                if ($("#Departamento").val() == null || $("#Departamento").val() == "0") {
                    $("#DepartamentoHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "DepartamentoAncla";
                    bDepartamento = false;
                }

                // Provincia
                var bProvincia = true;
                if ($("#Provincia").val() == null || $("#Provincia").val() == "0") {
                    $("#ProvinciaHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "ProvinciaAncla";
                    bProvincia = false;
                }

                // Distrito
                var bDistrito = true;
                if ($("#Distrito").val() == null || $("#Distrito").val() == "0") {
                    $("#DistritoHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "DistritoAncla";
                    bDistrito = false;
                }

                // Envío Poliza
                var bEnvioPoliza = true;
                if ($("#EnvioPoliza").val() == null || $("#EnvioPoliza").val() == "0") {
                    $("#EnvioPolizaHelper").addClass("helper-error").text("Campo obligatorio");
                    if (ancla == "") ancla = "EnvioPolizaAncla";
                    bEnvioPoliza = false;
                }

                var bPersonaAutorizadaEntrega = true;
                var bTipoViaEntrega = true;
                var bDireccionEntrega = true;
                var bEspacioUrbanoEntrega = true;
                var bDepartamentoEntrega = true;
                var bProvinciaEntrega = true;
                var bDistritoEntrega = true;
                if ($("#EnvioPoliza").val() == "F") {
                    // Persona Autorizada
                    if ($("#PersonaAutorizadaEntrega").val().trim() == "") {
                        $("#PersonaAutorizadaEntregaHelper").addClass("helper-error").text("Campo obligatorio");
                        if (ancla == "") ancla = "PersonaAutorizadaEntregaAncla";
                        bPersonaAutorizadaEntrega = false;
                    }

                    // Tipo de Vía Entrega
                    if ($("#TipoViaEntrega").val() == null || $("#TipoViaEntrega").val() == "0") {
                        $("#TipoViaEntregaHelper").addClass("helper-error").text("Campo obligatorio");
                        if (ancla == "") ancla = "TipoViaEntregaAncla";
                        bTipoViaEntrega = false;
                    }

                    // Dirección Entrega
                    if ($("#DireccionEntrega").val().trim() == "") {
                        $("#DireccionEntregaHelper").addClass("helper-error").text("Campo obligatorio");
                        if (ancla == "") ancla = "DireccionEntregaAncla";
                        bDireccionEntrega = false;
                    }

                    // Espacio Urbano Entrega
                    if ($("#EspacioUrbanoEntrega").val().trim() == "") {
                        $("#EspacioUrbanoEntregaHelper").addClass("helper-error").text("Campo obligatorio");
                        if (ancla == "") ancla = "EspacioUrbanoEntregaAncla";
                        bEspacioUrbanoEntrega = false;
                    }

                    // Departamento Entrega
                    if ($("#DepartamentoEntrega").val() == null || $("#DepartamentoEntrega").val() == "0") {
                        $("#DepartamentoEntregaHelper").addClass("helper-error").text("Campo obligatorio");
                        if (ancla == "") ancla = "DepartamentoEntregaAncla";
                        bDepartamentoEntrega = false;
                    }

                    // Provincia Entrega
                    if ($("#ProvinciaEntrega").val() == null || $("#ProvinciaEntrega").val() == "0") {
                        $("#ProvinciaEntregaHelper").addClass("helper-error").text("Campo obligatorio");
                        if (ancla == "") ancla = "ProvinciaEntregaAncla";
                        bProvinciaEntrega = false;
                    }

                    // Distrito Entrega
                    if ($("#DistritoEntrega").val() == null || $("#DistritoEntrega").val() == "0") {
                        $("#DistritoEntregaHelper").addClass("helper-error").text("Campo obligatorio");
                        if (ancla == "") ancla = "DistritoEntregaAncla";
                        bDistritoEntrega = false;
                    }
                }

                var esValido =
                    bApellidoPaterno &
                    bApellidoMaterno &
                    bNombres &
                    bTipoDocumento &
                    bNumeroDocumento &
                    bTelefono &
                    bCelular &
                    bCorreoElectronico &
                    bCentroLaboral &
                    bTipoVia &
                    bDireccion &
                    bEspacioUrbano &
                    bDepartamento &
                    bProvincia &
                    bDistrito &
                    bEnvioPoliza &
                    bPersonaAutorizadaEntrega &
                    bTipoViaEntrega &
                    bDireccionEntrega &
                    bEspacioUrbanoEntrega &
                    bDepartamentoEntrega &
                    bProvinciaEntrega &
                    bDistritoEntrega;
                if (!esValido) scroll(ancla);
                return esValido;
            }

            function PoblarCombobox(control, arreglo) {
                $(control).html("");
                $(control).append($("<option>").text("«Seleccione»").attr("value", "0").attr("disabled", true));
                arreglo.forEach(a => {
                    $(control).append($("<option>").text(a.Glosa).attr("value", a.Id));
                });
            }

            function EvaluarEnvioPoliza() {
                if ($("#EnvioPoliza").val() == "F") {
                    $("#EnvioFisico").show();
                }
                else {
                    $("#EnvioFisico").hide();
                }
            }

            function CompletarVariablesCliente() {
                $("#NombreCliente").html(nombreCliente);
                $("#TipoDocumentoCliente").html(tipoDocumentoCliente);
                $("#NumeroDocumentoCliente").html(numeroDocumentoCliente);
                $("#CorreoElectronicoCliente").html(correoElectronicoCliente);
                $("#CelularCliente").html(celularCliente);
            }

            function DeshabilitarSeleccione() {
                $('option[value="0"]').attr("disabled", "disabled");
                actualizarCombobox();
            }

            function EvaluarBotonEnviar() {
                if ($("#FirmaDigital").val() != "") {
                    $("#Enviar").show();
                }
            }
        });
    </script>
</asp:Content>