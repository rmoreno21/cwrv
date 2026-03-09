<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="ConsultaSolicitudesAnticipo.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.ConsultaSolicitudesAnticipo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.anticipos.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div id="Principal" align="left" style="width:860px;margin-top:15px;padding:20px;background:#FFF; border:1px solid #00466e">
        <h1 class="simple" style="width:278px">Consulta de Solicitudes de Anticipo</h1>

        <div class="formLinea">
            <label id="LabNumeroSolicitud" for="NumeroLote" class="formLabel">Número de Solicitud:</label>
            <asp:TextBox ID="NumeroSolicitud" runat="server" CssClass="formTextbox" Width="80" ClientIDMode="Static" MaxLength="10"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="LabFechaDesde" for="FechaDesde" class="formLabel formLabel2Izq">Fecha desde:</label>
            <asp:TextBox ID="FechaDesde" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="8"></asp:TextBox>

            <label id="LabFechaHasta" for="FechaHasta" class="formLabel formLabel2Der">Fecha hasta:</label>
            <asp:TextBox ID="FechaHasta" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
        </div>

        <div class="formLinea" align="center">
            <asp:HyperLink ID="BuscarAceptacionAnticipos" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
        </div>

        <asp:HiddenField ID="TabSolicitudesAnticipoIndicePagina" runat="server" ClientIDMode="Static" Value="1" />
        <asp:HiddenField ID="TabSolicitudesAnticipoTamanhoPagina" runat="server" ClientIDMode="Static" Value="25" />
        <asp:HiddenField ID="TabSolicitudesAnticipoColumnaOrdenar" runat="server" ClientIDMode="Static" Value="1" />
        <asp:HiddenField ID="TabSolicitudesAnticipoDireccionOrdenar" runat="server" ClientIDMode="Static" Value="A" />

        <div id="ContenedorExportar" class="formLinea" align="right" style="display:none">
            <asp:Button ID="ExportarExcel" runat="server" Text="Exportar a Excel" Width="130" Height="26"
                CssClass="boton darkblue sharp" ClientIDMode="Static" UseSubmitBehavior="False" OnClick="ExportarExcel_Click" />
        </div>

        <div id="TablaSolicitudesAnticipoCargando" align="center" style="display:none">
            <asp:Image ID="icoSolicitudesAnticipoCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando solicitudes de anticipo, espere por favor...</span>
        </div>
        <div id="TablaSolicitudesAnticipoContenedor" style="display:none"></div>
        <div id="TablaSolicitudesAnticipoError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes de anticipo. <a id="TablaSolicitudesAnticipoReintentar">Intentar de nuevo</a>.</div>
    </div>

    <div id="VentanasModales" style="display:none">
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

        <%--Inicio Modal Cargando Solicitudes--%>
        <div id="ModalCargandoSolicitudes" title="Cargando Solicitudes">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCIcono" style="width:40px;height:40px" class="cargando"></td>
                        <td id="MCContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0;color:#0060a9">
                                Cargando solitudes, por favor espere un momento...
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <%--Fin Modal Cargando Solicitudes--%>
    </div>
</asp:Content>
