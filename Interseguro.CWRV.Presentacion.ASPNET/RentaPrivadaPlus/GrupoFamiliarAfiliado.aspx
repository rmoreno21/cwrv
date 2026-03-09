<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="GrupoFamiliarAfiliado.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus.GrupoFamiliarAfiliado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.rentaprivadaplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.rentaprivadaplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

    <script type="text/javascript">

        $(document).ready(function () {

            $('#ModGruFamNumeroBanco_RP').mask("99999999999999999999");
            //$("#ModGruFamNumeroBanco_RP").prop("readonly", true);
            $("#ModGruFamNumeroBanco_RP").prop('disabled', true);

            $("#ModGruFamBanco_RP").change(function () {

                $.ajax({
                    type: 'POST',
                    //url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerNroBancos',
                    url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: "{banco: '" + $("#ModGruFamBanco_RP").val() + "', id: ''}",

                    success: function (data) {

                        //if ($("#ModGruFamBanco_RP").val() == "SCOTIA") {
                        //    //$('#ModGruFamNumeroBanco_RP').mask("999-9999999");
                        //    //$("#ModGruFamNumeroBanco_RP").prop("readonly", false);
                        //    $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                        //    $('#ModGruFamNumeroBanco_RP').mask(data.d[0].Valor_2);
                        //} else if ($("#ModGruFamBanco_RP").val() == "IBK") {
                        //    //$('#ModGruFamNumeroBanco_RP').mask("999-999999999-9");
                        //    //$("#ModGruFamNumeroBanco_RP").prop("readonly", false);
                        //    $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                        //    $('#ModGruFamNumeroBanco_RP').mask(data.d[0].Valor_2);
                        //} else if ($("#ModGruFamBanco_RP").val() == "BBVA") {
                        //    //$('#ModGruFamNumeroBanco_RP').mask("9999-9999-99-9999999999");
                        //    //$("#ModGruFamNumeroBanco_RP").prop("readonly", false);
                        //    $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                        //    $('#ModGruFamNumeroBanco_RP').mask(data.d[0].Valor_2);
                        //} else if ($("#ModGruFamBanco_RP").val() == "BCP") {
                        //    //$('#ModGruFamNumeroBanco_RP').mask("999-9999999-9-99");
                        //    //$("#ModGruFamNumeroBanco_RP").prop("readonly", false);
                        //    //$('#ModGruFamNumeroBanco_RP').val('')
                        //    $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                        //    $('#ModGruFamNumeroBanco_RP').mask(data.d[0].Valor_2);
                        //} else {
                        //    //$("#ModGruFamNumeroBanco_RP").prop("readonly", true);
                        //    $("#ModGruFamNumeroBanco_RP").prop('disabled', true);
                        //    $('#ModGruFamNumeroBanco_RP').mask("99999999999999999999");
                        //}

                        var items;

                        //$.each(data, function (key, val) {
                        //    items += "<option value='" + val[posicion].Id + "'>" + val[posicion].Nombre + "</option>";
                        //    posicion += 1;
                        //});

                        for (var posicion = 0; posicion < data.d.length; posicion++) {
                            items += "<option value='" + data.d[posicion].Id + "'>" + data.d[posicion].Nombre + "</option>";
                        }

                        var header = '<option value=\'00\'>«Seleccione»</option>';

                        $('#ModGruFamTipoCtaBanco_RP').html(header + items);

                        $('#TexModGruFamTipoCtaBanco_RP').html($('#ModGruFamTipoCtaBanco_RP option:eq(0)').text());

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

            });

            $("#ModGruFamTipoCtaBanco_RP").change(function () {

                $.ajax({
                    type: 'POST',
                    url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: "{banco: '" + $("#ModGruFamBanco_RP").val() + "', id: '" + $("#ModGruFamTipoCtaBanco_RP").val() + "'}",

                    success: function (data) {

                        if ($("#ModGruFamBanco_RP").val() == "SCOTIA") {
                            $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                            $('#ModGruFamNumeroBanco_RP').mask(data.d[0].Valor_2);
                        } else if ($("#ModGruFamBanco_RP").val() == "IBK") {
                            $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                            $('#ModGruFamNumeroBanco_RP').mask(data.d[0].Valor_2);
                        } else if ($("#ModGruFamBanco_RP").val() == "BBVA") {
                            $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                            $('#ModGruFamNumeroBanco_RP').mask(data.d[0].Valor_2);
                        } else if ($("#ModGruFamBanco_RP").val() == "BCP") {
                            $("#ModGruFamNumeroBanco_RP").prop('disabled', false);
                            $('#ModGruFamNumeroBanco_RP').mask(data.d[0].Valor_2);
                        } else {
                            $("#ModGruFamNumeroBanco_RP").prop('disabled', true);
                            $('#ModGruFamNumeroBanco_RP').mask("99999999999999999999");
                        }

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

            });



        });

    </script>

    <style type="text/css">
        .masInfo {
            background-color: #FFF;
            padding: 20px;
            display: none;
            position: absolute;
            text-decoration: underline;
            z-index: 3;
        }

        .modal-header {
            border-bottom: #eee solid 1px;
        }

        .modal-footer {
            border-top: #eee solid 1px;
            text-align: right;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="Grupo_Familiar" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 114px">Grupo Familiar</h1>

        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruFamModo_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModIdGrupoFamiliar" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModPaginaLlamada" runat="server" ClientIDMode="Static" />

        <asp:Panel ID="ModGruFamLineaApellidos_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruFamApellidoPaterno_RP" for="ModGruFamApellidoPaterno_RP" class="formLabel formLabel2Izq">Apellido Paterno:</label>
            <asp:TextBox ID="ModGruFamApellidoPaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>

            <label id="LabModGruFamApellidoMaterno_RP" for="ModGruFamApellidoMaterno_RP" class="formLabel formLabel2Der">Apellido Materno:</label>
            <asp:TextBox ID="ModGruFamApellidoMaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="ModGruFamLineaNombres_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruFamNombres_RP" for="ModGruFamNombres_RP" class="formLabel formLabel2Izq">Nombres:</label>
            <asp:TextBox ID="ModGruFamNombres_RP" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static" MaxLength="80" Style="text-transform: uppercase"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="ModGruFamLineaIdentificacion_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruFamTipoIdentificacion_RP" for="ModGruFamTipoIdentificacion_RP" class="formLabel formLabel2Izq">Tipo de Identificación:</label>
            <asp:DropDownList ID="ModGruFamTipoIdentificacion_RP" runat="server" CssClass="formCombobox" Width="260" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabModGruFamNumeroIdentificacion_RP" for="ModGruFamNumeroIdentificacion_RP" class="formLabel formLabel2Der" style="margin-left: 22px;">Nro. de Identificación:</label>
            <asp:TextBox ID="ModGruFamNumeroIdentificacion_RP" runat="server" CssClass="formTextbox enteroPositivo" Width="180" ClientIDMode="Static" MaxLength="9"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="ModGruFamLineaParentesco_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruFamParentesco_RP" for="ModGruFamParentesco_RP" class="formLabel formLabel2Izq">Parentesco*:</label>
            <asp:DropDownList ID="ModGruFamParentesco_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
            </asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="ModGruFamLineaSexoNacimiento_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruFamSexo_RP" for="ModGruFamSexo_RP" class="formLabel formLabel2Izq">Sexo*:</label>
            <asp:DropDownList ID="ModGruFamSexo_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabModGruFamFechaNacimiento_RP" for="ModGruFamFechaNacimiento_RP" class="formLabel formLabel2Der">Fecha de Nacimiento*:</label>
            <asp:TextBox ID="ModGruFamFechaNacimiento_RP" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="ModGruFamLineaInvalidez_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruFamIndInvalidez_RP" for="ModGruFamIndInvalidez_RP" class="formLabel formLabel2Izq">Indicador de Invalidez*:</label>
            <asp:DropDownList ID="ModGruFamIndInvalidez_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabModGruFamTipoInvalidez_RP" for="ModGruFamTipoInvalidez_RP" class="formLabel formLabel2Der">Tipo de Invalidez*:</label>
            <asp:DropDownList ID="ModGruFamTipoInvalidez_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
            </asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="ModGruFamLineaFechaInvalidez_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruFamFechaInvalidez_RP" for="ModGruFamFechaInvalidez_RP" class="formLabel formLabel2Izq">Fecha de Invalidez<span id="AstModGruFamFechaInvalidez_RP"></span>:</label>
            <asp:TextBox ID="ModGruFamFechaInvalidez_RP" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
        </asp:Panel>

        <br />
        <a href="#" id="mostrar" class="grilla_info" style="width: 100px; cursor: help">Mas información</a>
        <br />
        <br />

        <%--<br />--%>

        <div id="masInfo" style="display: none;">

            <%--class="masInfo"--%>

            <%--<div class="modal-header">
            </div>--%>

            <asp:Panel ID="ModGruFamLineaNacionalProfesion_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabModGruFamNacional_RP" for="ModGruFamNacional_RP" class="formLabel formLabel2Izq">Nacionalidad:</label>
                <asp:DropDownList ID="ModGruFamNacional_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                </asp:DropDownList>

                <label id="LabModGruFamProfesion_RP" for="ModGruFamProfesion_RP" class="formLabel formLabel2Der">Profesión:</label>
                <asp:DropDownList ID="ModGruFamProfesion_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                </asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaResidencia_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabModGruFamResidencia_RP" for="ModGruFamResidencia_RP" class="formLabel formLabel2Izq">Residencia:</label>
                <asp:DropDownList ID="ModGruFamResidencia_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="625" ClientIDMode="Static" Enabled="False">
                </asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaPEP_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabModGruFamPEP_RP" for="ModGruFamPEP_RP" class="formLabel formLabel2Izq" title="¿Es Persona Expuesta Políticamente?">¿Es Pers. Exp. Polít.?</label>
                <asp:DropDownList ID="ModGruFamPEP_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                </asp:DropDownList>

                <label id="LabModGruFamSO_RP" for="ModGruFamSO_RP" class="formLabel formLabel2Der">¿Es sujeto obligado?</label>
                <asp:DropDownList ID="ModGruFamSO_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                </asp:DropDownList>
            </asp:Panel>

            <%--<INI.GTI_7012_20>--%>
            <div style="display: none">
                <asp:Panel ID="ModGruFamLineaBanco_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamBanco_RP" for="ModGruFamBanco_RP" class="formLabel formLabel2Izq">Banco:</label>
                    <asp:DropDownList ID="ModGruFamBanco_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="260" ClientIDMode="Static" Enabled="False">
                    </asp:DropDownList>

                    <label id="labModGruFamTipoCtaBanco_RP" for="ModGruFamTipoCtaBanco_RP" class="formLabel formLabel2Der" style="margin-left: 22px;" title="Tipo de Cuenta">Tipo de Cuenta:</label>
                    <asp:DropDownList ID="ModGruFamTipoCtaBanco_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                    </asp:DropDownList>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaNroBanco_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">

                    <label id="labModGruFamNumeroBanco_RP" for="ModGruFamNumeroBanco_RP" class="formLabel formLabel2Izq" title="Número de cuenta">Nro. de cuenta:</label>
                    <asp:TextBox ID="ModGruFamNumeroBanco_RP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" MaxLength="20" ReadOnly="True"></asp:TextBox>

                </asp:Panel>
            </div>
            <%--<FIN.GTI_7012_20>--%>

            <asp:Panel ID="ModGruFamLineaComunicacion_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabModGruFamLineaComunicacion_RP" for="ModGruFamLineaComunicacion_RP" class="formLabel formLabel2Izq" title="Mecanismo de comunicación">Mecan. de comunic.:</label>
                <asp:DropDownList ID="ModGruFamComunicacion_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="260" ClientIDMode="Static" Enabled="False">
                </asp:DropDownList>

                <label id="LabGruFamConfidencialidadDatos_RP" for="ModGruFamConfidencialidadDatos_RP" class="formLabel formLabel2Der" style="margin-left: 22px;" title="Comercialización de datos personales">Comerc. de Datos Pers.:</label>
                <asp:DropDownList ID="ModGruFamConfidencialidadDatos_RP" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False"></asp:DropDownList>
            </asp:Panel>

            <%--<div class="modal-footer">
            </div>--%>
        </div>

        <div class="formLinea" align="center">
            <a id="ModGruFamAceptar_RP" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
            <a id="ModGruFamCancelar_RP" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
        </div>
    </asp:Panel>
    <%--  --%>
    <%--Inicio Modal Mantenimiento de grupo familiar--%>

    <%--    <div id="ModalGrupoFamiliar_RP" title="Grupo Familiar">
        <asp:Panel ID="ModGruFamCargando_RP" runat="server" ClientIDMode="Static" CssClass="modalCargandoContenido" Width="748" Height="230"></asp:Panel>
        <div id="ModGruFamContenido_RP" class="modalContenido">
            
        </div>
    </div>--%>

    <%--Fin Modal Mantenimiento de grupo familiar--%>


    <%--===================================================================--%>


    <%--Inicio Modal Cuadro de mensajes--%>
    <div id="ModalCuadroMensaje">
        <div>
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
        <div id="MCMBotonera" align="center">
            <a id="MCMAceptar" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
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
</asp:Content>
