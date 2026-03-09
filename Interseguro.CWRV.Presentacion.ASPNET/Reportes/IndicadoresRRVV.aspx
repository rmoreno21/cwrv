<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="IndicadoresRRVV.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Reportes.IndicadoresRRVV" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

    <%--<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/jquery.numeric.js")%>"></script>--%>
    <%--<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.simuladores.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>--%>

<style type="text/css">
    .sharp {}
</style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

<div id="div" align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">
        <h1 class="simple" style="width:162px">Indicadores de RRVV</h1>
         
        <div id="divBotonera" align="left" class="pieBoton">
            <div class="formLinea" align="center">
                <asp:Button ID="BtnGenerarExcel" CssClass="boton darkblue sharp" Width="130px"
                    Height="29px" runat="server" Text="Generar Reporte" ClientIDMode="Static" OnClick="BtnGenerarExcel_Click" />
                <%--<a id="btnEnviarExcel"  style="width:120px;height:22px" class="boton darkblue sharp">Generar Excel</a>--%>
            </div>
        </div>

</div>

<%--Ventanas Modales--%>

    <div style="display:none">
        
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

    </div>

</asp:Content>
