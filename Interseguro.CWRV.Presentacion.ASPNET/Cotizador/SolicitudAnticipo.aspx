<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="SolicitudAnticipo.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.SolicitudAnticipo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div id="Principal" align="left" style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width:283px">Solicitud de Anticipo Extemporáneo</h1>

        <%--<div id="Ayuda" class="cuadroMensaje info">
            Aquí va un texto de ayuda
        </div>--%>

        <div class="formLinea">
            <label id="LabSolicitud" for="Solicitud" class="formLabel">Número de Solicitud:</label>
            <asp:TextBox ID="Solicitud" runat="server" CssClass="formTextbox" Width="80" ClientIDMode="Static"></asp:TextBox>
        </div>

        <div class="formLinea" align="center">
            <asp:Button ID="GenerarAnticipo" Text="Generar Solicitud" style="width: 140px; height: 28px;" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static" OnClick="GenerarAnticipo_Click"></asp:Button>
        </div>

        <div id="TablaSolicitudesLoteCargando" align="center" style="display: none">
            <asp:Image ID="icoTablaSolicitudesLoteCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Guardando datos de anticipo, espere por favor...</span>
        </div>
        <div id="TablaSolicitudesLoteContenedor" style="display: none"></div>
        <div id="TablaSolicitudesLoteError" class="grilla_error" style="display: none">No se ha podido cargar la lista de solicitudes del lote. <a id="TablaSolicitudesLoteReintentar">Intentar de nuevo</a>.</div>
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
