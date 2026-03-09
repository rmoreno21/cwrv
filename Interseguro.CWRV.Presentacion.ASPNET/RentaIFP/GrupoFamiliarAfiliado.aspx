<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="GrupoFamiliarAfiliado.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.GrupoFamiliarAfiliado" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiParametro.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

     <script type="text/javascript">

         $(document).ready(function () {
             //$('#NumeroCuenta').mask("99999999999999999999");
             //$("#ModGruFamNumeroBanco_RP").prop("readonly", true);
             //$("#NumeroCuenta").prop('disabled', true);

             $("#Banco").live("change", function () {

                 $.ajax({
                     type: 'POST',
                     //url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerNroBancos',
                     url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
                     contentType: "application/json; charset=iso-8859-1",
                     dataType: 'json',
                     data: "{banco: '" + $("#Banco").val() + "', id: ''}",

                     success: function (data) {
                         var items;

                         //$.each(data, function (key, val) {
                         //    items += "<option value='" + val[posicion].Id + "'>" + val[posicion].Nombre + "</option>";
                         //    posicion += 1;
                         //});

                         for (var posicion = 0; posicion < data.d.length; posicion++) {
                             items += "<option value='" + data.d[posicion].Id + "'>" + data.d[posicion].Nombre + "</option>";
                         }

                         var header = '<option value=\'0\'>«Seleccione»</option>';

                         $('#TipoCtaBanco').html(header + items);

                         $('#TexTipoCtaBanco').html($('#TipoCtaBanco option:eq(0)').text());

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

             $("#TipoCtaBanco").live('change', function () {
                 $("#HTipoCtaBanco").val($("#TipoCtaBanco").val());
                 $.ajax({
                     type: 'POST',
                     url: '../RentaIFP/GrupoFamiliarAfiliadoCierre.aspx/ObtenerTipoCtaBancos',
                     contentType: "application/json; charset=iso-8859-1",
                     dataType: 'json',
                     data: "{banco: '" + $("#Banco").val() + "', id: '" + $("#TipoCtaBanco").val() + "'}",

                     success: function (data) {

                         if ($("#Banco").val() == "SCOTIA") {
                             $("#NumeroCuenta").prop('disabled', false);
                             $('#NumeroCuenta').mask(data.d[0].Valor_2);
                         } else if ($("#Banco").val() == "IBK") {
                             $("#NumeroCuenta").prop('disabled', false);
                             $('#NumeroCuenta').mask(data.d[0].Valor_2);
                         } else if ($("#Banco").val() == "BBVA") {
                             $("#NumeroCuenta").prop('disabled', false);
                             $('#NumeroCuenta').mask(data.d[0].Valor_2);
                         } else if ($("#Banco").val() == "BCP") {
                             $("#NumeroCuenta").prop('disabled', false);
                             $('#NumeroCuenta').mask(data.d[0].Valor_2);
                         } else {
                             $("#NumeroCuenta").prop('disabled', true);
                             $('#NumeroCuenta').mask("99999999999999999999");
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

         $("#GuardarBeneficiario").live("click", function () {
             $("#MCIcono").attr("class", "cargando");
             $("#MCContenedor").html("Guardando los datos del beneficiario, por favor espere un momento...");
             $("#ModalCotizando").dialog({ title: "Guardando" });
             $("#ModalCotizando").dialog("open");
         });

         $("#Regresar").live("click", function () {
             $("#MCIcono").attr("class", "cargando");
             $("#MCContenedor").html("Cargando la información de la solicitud, por favor espere un momento...");
             $("#ModalCotizando").dialog({ title: "Cargando" });
             $("#ModalCotizando").dialog("open");
         });

    </script>

    <style type="text/css">
        .masInfo {
            background-color: #FFF;
            padding: 20px;
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
        <h1 class="simple" style="width: 175px">Datos del beneficiario</h1>

        <asp:HiddenField ID="IdSolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="IdGrupoFamiliar" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="PaginaLlamada" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCUSPP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruFamModo" runat="server" ClientIDMode="Static" />

        <asp:Panel ID="LineaApellidos_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabApellidoPaterno" for="ApellidoPaterno" class="formLabel formLabel2Izq">Apellido Paterno:</label>
            <asp:TextBox ID="ApellidoPaterno" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>

            <label id="LabApellidoMaterno" for="ApellidoMaterno" class="formLabel formLabel2Der">Apellido Materno:</label>
            <asp:TextBox ID="ApellidoMaterno" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40" Style="text-transform: uppercase"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="LineaNombres" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabNombres" for="Nombres" class="formLabel formLabel2Izq">Nombres:</label>
            <asp:TextBox ID="Nombres" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static" MaxLength="80" Style="text-transform: uppercase"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="LineaIdentificacion" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabTipoIdentificacion" for="TipoIdentificacion" class="formLabel formLabel2Izq">Tipo de Identificación:</label>
            <asp:DropDownList ID="TipoIdentificacion" runat="server" CssClass="formCombobox" Width="258" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabNumeroIdentificacion" for="NumeroIdentificacion" class="formLabel formLabel2Der" style="margin-left: 22px;">Nro. de Identificación:</label>
            <asp:TextBox ID="NumeroIdentificacion" runat="server" CssClass="formTextbox enteroPositivo" Width="180" ClientIDMode="Static" MaxLength="9"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="LineaParentesco" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabParentesco" for="Parentesco" class="formLabel formLabel2Izq">Parentesco*:</label>
            <asp:DropDownList ID="Parentesco" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
            </asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="LineaSexoNacimiento" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabSexo" for="Sexo" class="formLabel formLabel2Izq">Sexo*:</label>
            <asp:DropDownList ID="Sexo" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabFechaNacimiento" for="FechaNacimiento" class="formLabel formLabel2Der">Fecha de Nacimiento*:</label>
            <asp:TextBox ID="FechaNacimiento" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="LineaInvalidez" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabIndInvalidez" for="IndInvalidez" class="formLabel formLabel2Izq">Indicador de Invalidez*:</label>
            <asp:DropDownList ID="IndInvalidez" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabTipoInvalidez" for="TipoInvalidez" class="formLabel formLabel2Der">Tipo de Invalidez*:</label>
            <asp:DropDownList ID="TipoInvalidez" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
            </asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="LineaFechaInvalidez" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabFechaInvalidez" for="FechaInvalidez" class="formLabel formLabel2Izq">Fecha de Invalidez<span id="AstModGruFamFechaInvalidez_RP"></span>:</label>
            <asp:TextBox ID="FechaInvalidez" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="LineaNacionalidadProfesion" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabNacionalidad" for="Nacionalidad" class="formLabel formLabel2Izq">Nacionalidad:</label>
            <asp:DropDownList ID="Nacionalidad" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabProfesion" for="Profesion" class="formLabel formLabel2Der">Profesión:</label>
            <asp:DropDownList ID="Profesion" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static">
            </asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="LineaResidencia" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabResidencia" for="Residencia" class="formLabel formLabel2Izq">Residencia:</label>
            <asp:DropDownList ID="Residencia" runat="server" CssClass="formCombobox" Width="626" ClientIDMode="Static">
            </asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="LineaPEP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabPEP" for="PEP" class="formLabel formLabel2Izq" title="¿Es Persona Expuesta Políticamente?">¿Es Pers. Exp. Polít.?:</label>
            <asp:DropDownList ID="PEP" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabSO" for="SO" class="formLabel formLabel2Der">¿Es sujeto obligado?:</label>
            <asp:DropDownList ID="SO" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static">
            </asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="LineaBanco" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabBanco" for="Banco" class="formLabel formLabel2Izq">Banco:</label>
            <asp:DropDownList ID="Banco" runat="server" CssClass="formCombobox" Width="258" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabTipoCtaBanco" for="TipoCtaBanco" class="formLabel formLabel2Der" style="margin-left: 22px;" title="Tipo de Cuenta">Tipo de Cuenta:</label>
            <asp:DropDownList ID="TipoCtaBanco" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static">
            </asp:DropDownList>
            <asp:HiddenField ID="HTipoCtaBanco" runat="server" ClientIDMode="Static" />
        </asp:Panel>
            
        <asp:Panel ID="LineaNumeroCuenta" runat="server" ClientIDMode="Static" CssClass="formLinea">                
            <label id="LabNumeroCuenta" for="NumeroCuenta" class="formLabel formLabel2Izq" title="Número de cuenta">Nro. de cuenta:</label>
            <asp:TextBox ID="NumeroCuenta" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="20"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="LineaComunicacion" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabComunicacion" for="Comunicacion" class="formLabel formLabel2Izq" title="Mecanismo de comunicación">Mecan. de comunic.:</label>
            <asp:DropDownList ID="Comunicacion" runat="server" CssClass="formCombobox" Width="260" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabConfidencialidadDatos" for="ConfidencialidadDatos" class="formLabel formLabel2Der" style="margin-left: 21px;" title="Comercialización de datos personales">Comerc. de Datos Pers.:</label>
            <asp:DropDownList ID="ConfidencialidadDatos" runat="server" CssClass="formCombobox" Width="184" ClientIDMode="Static"></asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="LineaCorreoElectronico" runat="server" ClientIDMode="Static" CssClass="formLinea">                
            <label id="LabCorreoElectronico" for="CorreoElectronico" class="formLabel formLabel2Izq" title="Correo Electrónico">Correo Electrónico:</label>
            <asp:TextBox ID="CorreoElectronico" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static"></asp:TextBox>
        </asp:Panel>

        <%--Nuevos Campos Inteligo--%>
        <asp:Panel ID="LineaEstadocivil" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabEstadoCivil" for="EstadoCivil" class="formLabel formLabel2Izq">Estado Civil:</label>
            <asp:DropDownList ID="EstadoCivil" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
            </asp:DropDownList>
        </asp:Panel>

        <asp:Panel ID="LineaCentroLaboralCargo" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabCentroLaboral" for="CentroLaboral" class="formLabel formLabel2Izq">Centro Laboral:</label>
            <asp:TextBox ID="CentroLaboral" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="50"></asp:TextBox>

            <label id="LabCargo" for="Cargo" class="formLabel formLabel2Der">Cargo:</label>
            <asp:TextBox ID="Cargo" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="50"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="LineaActividadEconomica" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabActividadEconomica" for="ActividadEconomica" class="formLabel formLabel2Izq">Actividad Económica:</label>
            <asp:TextBox ID="ActividadEconomica" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="50"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="LineaIngreso" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabMonedaIngreso" for="MonedaIngreso" class="formLabel formLabel2Izq">Moneda de Ingreso:</label>
            <asp:DropDownList ID="MonedaIngreso" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static">
            </asp:DropDownList>

            <label id="LabIngresoNeto" for="IngresoNeto" class="formLabel formLabel2Der">Ingreso Neto:</label>
            <asp:TextBox ID="IngresoNeto" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="LineaTelefonoCelular" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabTelefono" for="Telefono" class="formLabel formLabel2Izq">Teléfono:</label>
            <asp:TextBox ID="Telefono" runat="server" CssClass="formTextbox" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>

            <label id="LabCelular" for="Celular" class="formLabel formLabel2Der">Celular:</label>
            <asp:TextBox ID="Celular" runat="server" CssClass="formTextbox telefono" Width="180" ClientIDMode="Static" MaxLength="9"></asp:TextBox>
        </asp:Panel>
        <%--Fin Nuevos Campos Inteligo--%>

        <div class="formLinea" align="center">
             <asp:Button ID="GuardarBeneficiario" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Guardar"
                    ClientIDMode="Static" OnClick="GuardarBeneficiario_Click" />

            <asp:Button ID="Regresar" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Regresar"
                    ClientIDMode="Static" OnClick="Regresar_Click" />

            <%--<a id="ModGruFamAceptar_RP" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
            <a id="ModGruFamCancelar_RP" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>--%>
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
                        <td id="MCMIcono" style="width:40px;height:40px"></td>
                        <td id="MCMContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCMContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
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
                <a id="MCMAceptar" class="boton darkblue sharp" style="width:80px">Aceptar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de mensajes--%>

        <%--Inicio Modal Cuadro de advertencia--%>
        <div id="ModalCuadroAdvertencia">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCAIcono" style="width:40px;height:40px"></td>
                        <td id="MCAContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCAContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
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
                <a id="MCAAceptar" class="boton darkblue sharp" style="width:80px">Aceptar</a>
                <a id="MCACancelar" class="boton darkblue sharp" style="width:80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>

        <%--Inicio Modal Cotizando--%>
        <div id="ModalCotizando">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCIcono" style="width:40px;height:40px"></td>
                        <td id="MCContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
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
        
   

    
</asp:Content>

