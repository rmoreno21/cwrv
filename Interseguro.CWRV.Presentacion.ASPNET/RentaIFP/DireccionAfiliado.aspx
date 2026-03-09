<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="DireccionAfiliado.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.DireccionAfiliado" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="Direccion_Afiliado" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 180px">Dirección del afiliado</h1>


        <asp:HiddenField ID="ModDirModo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModIdDireccion" runat="server" ClientIDMode="Static" />

        <div class="formLinea">
            <label id="LabModDomicilio" for="ModDomicilio" class="formLabel formLabel2Izq">Tipos de Vía*:</label>
            <asp:DropDownList ID="ModDomicilio" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static" ReadOnly="True">
            </asp:DropDownList>
        
        </div>

         <div class="formLinea">
            
            <label id="Label2" for="ModDirDireccion" class="formLabel formLabel2Izq">Dirección*:</label>
            <asp:TextBox ID="ModDirDireccion" runat="server" CssClass="formTextbox" Width="480" ClientIDMode="Static" MaxLength="80" Style="text-transform: uppercase"></asp:TextBox>

        </div>

        <div class="formLinea">
            <label id="LabModDirEspacioUrbano" for="ModDirEspacioUrbano" class="formLabel formLabel2Izq">N°/Mz/Lt/Dpto.*:</label>
            <asp:TextBox ID="ModDirEspacioUrbano" runat="server" CssClass="formTextbox" Width="110" ClientIDMode="Static" MaxLength="80"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="LabModDirDepartamento" for="ModDirDepartamento" class="formLabel formLabel2Izq">Departamento*:</label>
            <asp:DropDownList ID="ModDirDepartamento" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static" ReadOnly="True">
            </asp:DropDownList>
        </div>

        <div id="ModDirControlCiudad" class="formLinea">
            <label id="LabModDirCiudad" for="ModDirCiudad" class="formLabel formLabel2Izq">Provincia*:</label>
            <asp:DropDownList ID="ModDirCiudad" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static" Enabled="False">
            </asp:DropDownList>
            <span id="ModDirCargandoCiudad" class="paginador_cargando"></span>
        </div>

        <div id="ModDirControlComuna" class="formLinea">
            <label id="LabModDirComuna" for="ModDirComuna" class="formLabel formLabel2Izq">Distrito*:</label>
            <asp:DropDownList ID="ModDirComuna" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static" Enabled="False">
            </asp:DropDownList>
            <span id="ModDirCargandoComuna" class="paginador_cargando"></span>
        </div>

        <div class="formLinea">
            <label id="LabModDirPrincipal" for="ModDirPrincipal" class="formLabel formLabel2Izq">Principal*:</label>
            <asp:DropDownList ID="ModDirPrincipal" runat="server" CssClass="formCombobox" Width="110" ClientIDMode="Static" ReadOnly="True">
            </asp:DropDownList>
        </div>

        <div class="formLinea" align="center">
            <a id="ModDirAceptar_RP" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
            <a id="ModDirCancelar_RP" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
        </div>
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
