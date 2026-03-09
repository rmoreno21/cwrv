<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="GrupoFamiliarAfiliado.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaPrivada.GrupoFamiliarAfiliado" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.rentaprivada.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.rentaprivada.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="Grupo_Familiar" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 180px">Grupo Familiar</h1>

        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModGruFamModo_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModIdGrupoFamiliar" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModPaginaLlamada" runat="server" ClientIDMode="Static" />


        <asp:Panel ID="ModGruFamLineaApellidos_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruFamApellidoPaterno_RP" for="ModGruFamApellidoPaterno_RP" class="formLabel formLabel2Izq">Apellido Paterno:</label>
            <asp:TextBox ID="ModGruFamApellidoPaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="20"></asp:TextBox>

            <label id="LabModGruFamApellidoMaterno_RP" for="ModGruFamApellidoMaterno_RP" class="formLabel formLabel2Der">Apellido Materno:</label>
            <asp:TextBox ID="ModGruFamApellidoMaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="20"></asp:TextBox>
        </asp:Panel>

        <asp:Panel ID="ModGruFamLineaNombres_RP" runat="server" ClientIDMode="Static" CssClass="formLinea">
            <label id="LabModGruFamNombres_RP" for="ModGruFamNombres_RP" class="formLabel formLabel2Izq">Nombres:</label>
            <asp:TextBox ID="ModGruFamNombres_RP" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static" MaxLength="80"></asp:TextBox>
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

        <div class="formLinea" align="center">
            <a id="ModGruFamAceptar_RP" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
            <a id="ModGruFamCancelar_RP" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
        </div>
    </asp:Panel>

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



</asp:Content>

