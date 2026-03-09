<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="GrupoFamiliarAfiliadoCierre.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.GrupoFamiliarAfiliadoCierre" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/GrupoFamiliarAfiliadoCierre.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiCotizadorIFP.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/jquery.steps.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="../Scripts/Common.js"></script>
    <%--<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>--%>
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/jquery.steps.css")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>" type="text/css" />

    <script type="text/javascript">

        $(document).ready(function () {

            $('#ModGruFamNumeroBanco_RP').mask("99999999999999999999");
            $("#ModGruFamNumeroBanco_RP").prop('disabled', true);

            $("#ModGruFamBanco_RP").change(function () {

                $.ajax({
                    type: 'POST',
                    url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: 'json',
                    data: "{banco: '" + $("#ModGruFamBanco_RP").val() + "', id: ''}",

                    success: function (data) {

                        var items;

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
                        if (data.d.length > 0) {
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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="Grupo_Familiar" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruFamModo_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModIdGrupoFamiliar" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModPaginaLlamada" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruFamCantidad" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruFamTotal" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruFamsolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruFamfecsolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruFamestadosolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruPlan" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruNumCorrelativo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HSolicitudSerializado" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HPaginaSiguiente" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HTipoPeriodoBeneficiario" runat="server" ClientIDMode="Static" Value="0" />

        <asp:HiddenField ID="HToken" runat="server" ClientIDMode="Static" />

        <div id="pasosCierre"></div>
        <div id="pasos" class="grilla_info">
            Completar todos los campos que se muestran a continuación.
        </div>

        <br />

        <fieldset>
            <legend>Datos generales</legend>

            <asp:Panel ID="ModGruFamLineaApellidos_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamApellidoPaterno_RP" for="ModGruFamApellidoPaterno_RP" class="formLabel formLabel2Izq">Apellido Paterno:</label>
                <asp:TextBox ID="ModGruFamApellidoPaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>

                <label id="LabModGruFamApellidoMaterno_RP" for="ModGruFamApellidoMaterno_RP" class="formLabel formLabel2Der">Apellido Materno:</label>
                <asp:TextBox ID="ModGruFamApellidoMaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaNombres_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamNombres_RP" for="ModGruFamNombres_RP" class="formLabel formLabel2Izq">Nombres:</label>
                <asp:TextBox ID="ModGruFamNombres_RP" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static" MaxLength="80" Style="text-transform: uppercase"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaIdentificacion_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamTipoIdentificacion_RP" for="ModGruFamTipoIdentificacion_RP" class="formLabel formLabel2Izq">Tipo de Identificación:</label>
                <asp:DropDownList ID="ModGruFamTipoIdentificacion_RP" runat="server" CssClass="formCombobox" Width="260" ClientIDMode="Static">
                </asp:DropDownList>

                <label id="LabModGruFamNumeroIdentificacion_RP" for="ModGruFamNumeroIdentificacion_RP" class="formLabel formLabel2Der" style="margin-left: 22px;">Nro. de Identificación:</label>
                <asp:TextBox ID="ModGruFamNumeroIdentificacion_RP" runat="server" CssClass="formTextbox enteroPositivo" Width="178" ClientIDMode="Static" MaxLength="9"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaParentesco_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamParentesco_RP" for="ModGruFamParentesco_RP" class="formLabel formLabel2Izq">Parentesco*:</label>
                <asp:DropDownList ID="ModGruFamParentesco_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static" />

                <label id="LabModGruFamEstadoCivil_RP" for="ModGruFamEstadoCivil_RP" class="formLabel formLabel2Der" runat="server">Estado Civil:</label>
                <asp:DropDownList ID="ModGruFamEstadoCivil_RP" runat="server" CssClass="formCombobox" Width="182" ClientIDMode="Static" />
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaSexoNacimiento_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamSexo_RP" for="ModGruFamSexo_RP" class="formLabel formLabel2Izq">Sexo*:</label>
                <asp:DropDownList ID="ModGruFamSexo_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                </asp:DropDownList>

                <label id="LabModGruFamFechaNacimiento_RP" for="ModGruFamFechaNacimiento_RP" class="formLabel formLabel2Der">Fecha de Nacimiento*:</label>
                <asp:TextBox ID="ModGruFamFechaNacimiento_RP" runat="server" CssClass="fecha formTextbox formCalendar" Width="178" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaInvalidez_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo" Style="display: none">
                <label id="LabModGruFamIndInvalidez_RP" for="ModGruFamIndInvalidez_RP" class="formLabel formLabel2Izq">Indicador de Invalidez*:</label>
                <asp:DropDownList ID="ModGruFamIndInvalidez_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                </asp:DropDownList>

                <label id="LabModGruFamTipoInvalidez_RP" for="ModGruFamTipoInvalidez_RP" class="formLabel formLabel2Der">Tipo de Invalidez*:</label>
                <asp:DropDownList ID="ModGruFamTipoInvalidez_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                </asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaFechaInvalidez_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo" Style="display: none">
                <label id="LabModGruFamFechaInvalidez_RP" for="ModGruFamFechaInvalidez_RP" class="formLabel formLabel2Izq">Fecha de Invalidez<span id="AstModGruFamFechaInvalidez_RP"></span>:</label>
                <asp:TextBox ID="ModGruFamFechaInvalidez_RP" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaNacionalProfesion_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamNacional_RP" for="ModGruFamNacional_RP" class="formLabel formLabel2Izq">Nacionalidad:</label>
                <asp:DropDownList ID="ModGruFamNacional_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                </asp:DropDownList>

                <label id="LabModGruFamProfesion_RP" for="ModGruFamProfesion_RP" class="formLabel formLabel2Der">Profesión:</label>
                <asp:DropDownList ID="ModGruFamProfesion_RP" runat="server" CssClass="formCombobox" Width="182" ClientIDMode="Static">
                </asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaResidencia_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamResidencia_RP" for="ModGruFamResidencia_RP" class="formLabel formLabel2Izq">Residencia:</label>
                <asp:DropDownList ID="ModGruFamResidencia_RP" runat="server" CssClass="formCombobox" Width="624" ClientIDMode="Static">
                </asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaPEP_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamPEP_RP" for="ModGruFamPEP_RP" class="formLabel formLabel2Izq" title="¿Es Persona Expuesta Políticamente?">¿Es Pers. Exp. Polít.?</label>
                <asp:DropDownList ID="ModGruFamPEP_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                </asp:DropDownList>

                <label id="LabModGruFamSO_RP" for="ModGruFamSO_RP" class="formLabel formLabel2Der">¿Es sujeto obligado?</label>
                <asp:DropDownList ID="ModGruFamSO_RP" runat="server" CssClass="formCombobox" Width="182" ClientIDMode="Static">
                </asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaBanco_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamBanco_RP" for="ModGruFamBanco_RP" class="formLabel formLabel2Izq">Banco:</label>
                <asp:DropDownList ID="ModGruFamBanco_RP" runat="server" CssClass="formCombobox" Width="260" ClientIDMode="Static">
                </asp:DropDownList>

                <label id="labModGruFamTipoCtaBanco_RP" for="ModGruFamTipoCtaBanco_RP" class="formLabel formLabel2Der" style="margin-left: 22px;" title="Tipo de Cuenta">Tipo de Cuenta:</label>
                <asp:DropDownList ID="ModGruFamTipoCtaBanco_RP" runat="server" CssClass="formCombobox" Width="182" ClientIDMode="Static">
                </asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaNroBanco_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">

                <label id="labModGruFamNumeroBanco_RP" for="ModGruFamNumeroBanco_RP" class="formLabel formLabel2Izq" title="Número de cuenta">Nro. de cuenta:</label>
                <asp:TextBox ID="ModGruFamNumeroBanco_RP" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="20"></asp:TextBox>

            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaComunicacion_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamLineaComunicacion_RP" for="ModGruFamLineaComunicacion_RP" class="formLabel formLabel2Izq" title="Mecanismo de comunicación">Mecan. de comunic.:</label>
                <asp:DropDownList ID="ModGruFamComunicacion_RP" runat="server" CssClass="formCombobox" Width="260" ClientIDMode="Static">
                </asp:DropDownList>

                <label id="LabGruFamConfidencialidadDatos_RP" for="ModGruFamConfidencialidadDatos_RP" class="formLabel formLabel2Der" style="margin-left: 22px;" title="Comercialización de datos personales">Comerc. de Datos Pers.:</label>
                <asp:DropDownList ID="ModGruFamConfidencialidadDatos_RP" runat="server" CssClass="formCombobox" Width="182" ClientIDMode="Static"></asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaPorcentaje_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamLineaPorcentaje_RP" for="ModGruFamLineaPorcentaje_RP" class="formLabel formLabel2Izq" title="Porcentaje">Porcentaje (%):</label>
                <asp:TextBox ID="ModGruFamPorcentaje_RP" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="20"></asp:TextBox>
            </asp:Panel>

            <%--INI Nuevos Campos Inteligo--%>
            <asp:Panel ID="ModGruFamLineaEstadocivilMail_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamMail_RP" for="ModGruFamMail_RP" class="formLabel formLabel2Izq">Correo Electrónico:</label>
                <asp:TextBox ID="ModGruFamMail_RP" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="200"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaTelefonoCelular_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamTelefono_RP" for="ModGruFamTelefono_RP" class="formLabel formLabel2Izq">Teléfono:</label>
                <asp:TextBox ID="ModGruFamTelefono_RP" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>

                <label id="LabModGruFamCelular_RP" for="ModGruFamCelular_RP" class="formLabel formLabel2Der">Celular:</label>
                <asp:TextBox ID="ModGruFamCelular_RP" runat="server" CssClass="formTextbox telefono" Width="180" ClientIDMode="Static" MaxLength="9"></asp:TextBox>
            </asp:Panel>
            <%--FIN Nuevos Campos Inteligo--%>
        </fieldset>

        <br />

        <fieldset id="seccionOrigenFondo">
            <legend>Datos para orígenes de fondo</legend>
            <asp:Panel ID="ModGruFamLineaCentrolaboralCargo_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamCentrolaboral_RP" for="ModGruFamCentrolaboral_RP" class="formLabel formLabel2Izq">Centro Laboral:</label>
                <asp:TextBox ID="ModGruFamCentrolaboral_RP" runat="server" CssClass="formTextbox" Width="254" ClientIDMode="Static" MaxLength="50"></asp:TextBox>

                <label id="LabModGruFamCargo_RP" for="ModGruFamCargo_RP" class="formLabel formLabel2Der" style="margin-left: 20px;">Cargo:</label>
                <asp:TextBox ID="ModGruFamCargo_RP" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="50"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaActividadEconomica_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamActividadeconomica_RP" for="ModGruFamActividadeconomica_RP" class="formLabel formLabel2Izq">Actividad Económica:</label>
                <asp:TextBox ID="ModGruFamActividadeconomica_RP" runat="server" CssClass="formTextbox" Width="254" ClientIDMode="Static" MaxLength="50"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="ModGruFamLineaMonedaingresoIngreso_RP" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamMonedaingreso_RP" for="ModGruFamMonedaingreso_RP" class="formLabel formLabel2Izq">Moneda de Ingreso:</label>
                <asp:DropDownList ID="ModGruFamMonedaingreso_RP" runat="server" CssClass="formCombobox" Width="260" ClientIDMode="Static">
                </asp:DropDownList>

                <label id="LabModGruFamIngreso_RP" for="ModGruFamIngreso_RP" class="formLabel formLabel2Der" style="margin-left: 20px;">Ingreso Neto:</label>
                <asp:TextBox ID="ModGruFamIngreso_RP" runat="server" CssClass="formTextbox numerico" Width="178" ClientIDMode="Static"></asp:TextBox>
            </asp:Panel>

            <br />
            <asp:Panel ID="ModFruFamLineaDestinoFondos" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <label id="LabModGruFamDestinoFondos_RP" for="ModGruFamDestinoFondos_RP" style="width: 300px;" class="formLabel formLabel2Izq">Descripción jurada de origen y/o destino de fondos:</label>
            </asp:Panel>
            <asp:Panel ID="ModFruFamLineaDestinoFondosText" runat="server" ClientIDMode="Static" CssClass="formLineaGrupo">
                <textarea class="formTextbox ColorNegro" id="ModGruFamDestinoFondos_RP" style="height: 115px; width:780px; margin-left: 0px;" rows="6"></textarea>
            </asp:Panel>
        </fieldset>

        <br />

        <fieldset id="seccionPEP" style="display: none">
            <legend>Datos de las personas vinculadas al cliente PEP</legend>

            <div class="formLinea" align="right">
                <asp:HyperLink ID="NuevaPersonaVinculada_RPP" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px" runat="server" ClientIDMode="Static">Nueva</asp:HyperLink>
            </div>

            <div id="TablaPersonasVinculadasPEP_RP" align="center" style="height: 50px; padding: 82px 0">
                <asp:Image ID="icoTablaPersonasVinculadasPEP_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando personas vinculadas PEP, espere por favor...</span>
            </div>
            <div id="TablaPersonasVinculadasPEPContenedor_RP" style="display: none"></div>

            <br />

        </fieldset>

        <br />

        <div class="formLineaGrupo" align="center">
            <a id="ModGruFamCancelarCierre_RP" style="width: 180px; height: 22px" class="botonDeshabilitado gris gris_sharp">Anterior</a>
            <a id="ModGruFamAceptarCierre_RP" style="width: 180px; height: 22px" class="botonDeshabilitado gris gris_sharp">Siguiente</a>
            <a id="ModVistaPrevia" style="width: 180px; height: 22px" class="boton darkblue sharp" visible="false">Vista Previa</a>
            <a id="ModGruFamCancelar_RP" style="width: 180px; height: 22px" class="boton darkblue sharp">Cancelar</a>
            <asp:Button ID="ModSolSiguiente_RP" Style="display: none" runat="server" Text="Siguiente cierre" />
            <asp:HiddenField ID="ModSolCancelar_RP" runat="server" ClientIDMode="Static" />
        </div>
    </asp:Panel>

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
            <a id="MCASiBeneficiario" class="boton darkblue sharp" style="width: 80px">Si</a>
            <a id="MCANoBeneficiario" class="boton darkblue sharp" style="width: 80px">No</a>
        </div>
    </div>
    <asp:HiddenField ID="MCATablaPregunta" runat="server" ClientIDMode="Static" />
    <%--Fin Modal Cuadro de advertencia--%>

    <%--Inicio Modal Cotizando--%>
    <div id="ModalCotizando">
        <div>
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td id="MCIcono" style="width: 40px; height: 40px"></td>
                    <td id="MCContenedorMensaje" valign="middle" class="cuadroMensaje">
                        <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                            <asp:HiddenField ID="MCEstado" runat="server" ClientIDMode="Static" Value="0" />
                            <asp:HiddenField ID="MCEstadoIcono" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="MCEstadoTitulo" runat="server" ClientIDMode="Static" />
                            <asp:Literal ID="MCMensaje" runat="server"></asp:Literal>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <%--Fin Modal Cotizando--%>

    <div style="display: none">
        <%--Inicio Modal Mantenimiento de grupo familiar--%>
        <div id="ModalGrupoFamiliar_RP" title="Persona Vinculada a cliente PEP">
            <asp:Panel ID="ModGruFamCargando_RP" runat="server" ClientIDMode="Static" CssClass="modalCargandoContenido" Width="760" Height="147"></asp:Panel>
            <div id="ModGruFamContenido_RPP" class="modalContenido">
                <asp:HiddenField ID="ModGruFamModo_RPP" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="HidPersonaVinculada" runat="server" ClientIDMode="Static" />

                <asp:Panel ID="ModGruFamLineaApellidos_RPP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamApellidoPaterno_RPP" for="ModGruFamApellidoPaterno_RPP" class="formLabel formLabel2Izq">Apellido Paterno:</label>
                    <asp:TextBox ID="ModGruFamApellidoPaterno_RPP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40"></asp:TextBox>

                    <label id="LabModGruFamApellidoMaterno_RPP" for="ModGruFamApellidoMaterno_RPP" class="formLabel formLabel2Der">Apellido Materno:</label>
                    <asp:TextBox ID="ModGruFamApellidoMaterno_RPP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40"></asp:TextBox>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaNombres_RPP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamNombres_RPP" for="ModGruFamNombres_RPP" class="formLabel formLabel2Izq">Nombres:</label>
                    <asp:TextBox ID="ModGruFamNombres_RPP" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static" MaxLength="80"></asp:TextBox>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaIdentificacion_RPP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamTipoIdentificacion_RPP" for="ModGruFamTipoIdentificacion_RPP" class="formLabel formLabel2Izq">Tipo de Identificación:</label>
                    <asp:DropDownList ID="ModGruFamTipoIdentificacion_RPP" runat="server" CssClass="formCombobox" Width="260" ClientIDMode="Static">
                    </asp:DropDownList>

                    <label id="LabModGruFamNumeroIdentificacion_RPP" for="ModGruFamNumeroIdentificacion_RPP" class="formLabel formLabel2Der" style="margin-left: 22px;">Nro. de Identificación:</label>
                    <asp:TextBox ID="ModGruFamNumeroIdentificacion_RPP" runat="server" CssClass="formTextbox" Width="178" ClientIDMode="Static" MaxLength="20"></asp:TextBox>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaParentesco_RPP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamParentesco_RPP" for="ModGruFamParentesco_RPP" class="formLabel formLabel2Izq">Parentesco*:</label>
                    <asp:DropDownList ID="ModGruFamParentesco_RPP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                    </asp:DropDownList>
                </asp:Panel>

                <br />

                <div class="formLinea" align="center">
                    <a id="ModGruFamAceptar_RPP" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
                    <a id="ModGruFamCancelar_RPP" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
                </div>
            </div>
        </div>
        <%--Fin Modal Mantenimiento de grupo familiar--%>
    </div>
</asp:Content>
