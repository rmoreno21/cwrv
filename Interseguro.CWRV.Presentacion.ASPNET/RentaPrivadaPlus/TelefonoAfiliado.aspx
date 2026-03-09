<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="TelefonoAfiliado.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus.TelefonoAfiliado" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.rentaprivadaplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.rentaprivadaplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="Telefono_Afiliado" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 180px">Teléfono del afiliado</h1>


        <%--Inicio Modal Mantenimiento de teléfonos--%>

        <%--<div id="ModTelCargando" class="modalCargandoContenido" style="width: 308px; height: 100px"></div>--%>
        
        <%--<div id="ModTelCargando" class="modalCargandoContenido centrarCargandoContenido"  ></div>--%>
        
        <asp:HiddenField ID="ModTelModo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModIdTelefono" runat="server" ClientIDMode="Static" />

        <div class="formLinea">
            <label id="LabModTelTipo" for="ModTelTipo" class="formLabel formLabel2Izq">Tipo*:</label>
            <asp:DropDownList ID="ModTelTipo" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static" ReadOnly="True">
            </asp:DropDownList>
        </div>

        <div class="formLinea">
            <label id="LabModTelNumero" for="ModTelNumero" class="formLabel formLabel2Izq">Número*:</label>
            <asp:TextBox ID="ModTelNumero" runat="server" CssClass="formTextbox telefono" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="LabModTelPrincipal" for="ModTelPrincipal" class="formLabel formLabel2Izq">Principal*:</label>
            <asp:DropDownList ID="ModTelPrincipal" runat="server" CssClass="formCombobox" Width="110" ClientIDMode="Static" ReadOnly="True">
            </asp:DropDownList>
        </div>

        <div class="formLinea" align="center">
            <a id="ModTelAceptar_RP" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
            <a id="ModTelCancelar_RP" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
        </div>



        <%--Fin Modal Mantenimiento de teléfonos--%>
    </asp:Panel>

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

