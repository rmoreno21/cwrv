<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CantidadBeneficiarios.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.CantidadBeneficiarios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/CantidadBeneficiarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="Cantidad_Beneficiarios" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">

        <h1 class="simple" style="width: 197px">Beneficiarios</h1>

        <asp:Panel ID="ModCantBenefLinea" runat="server" ClientIDMode="Static" CssClass="formLinea ">
            <label id="LabModGruCantBenef_IFP" for="ModGruCantBenef_IFP" class="formLabel ">N° de Beneficiarios:</label>
            <asp:TextBox ID="ModGruCantBenef_IFP" runat="server" CssClass="formTextbox" Width="40" ClientIDMode="Static"></asp:TextBox>
        </asp:Panel>

        <div class="formLinea" align="center">
            <a id="ModCantBenefAceptar_IFP" style="width: 180px; height: 22px" class="boton darkblue sharp">Siguiente</a>
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

</asp:Content>
