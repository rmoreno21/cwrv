<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CargaCotizaciones.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Meler.CargaCotizaciones" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.meler.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div id="Principal" align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e">
        <h1 class="simple" style="width:370px">Generar Archivo XML de Carga de Cotizaciones</h1>

        <div class="formLinea">
            <label id="LabFechaDesde" for="FechaDesde" class="formLabel">Fecha plazo AFP desde:</label>
            <asp:TextBox ID="FechaDesde" runat="server" CssClass="fecha formTextbox formCalendar" Width="102" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="LabFechaHasta" for="FechaHasta" class="formLabel">Fecha plazo AFP hasta:</label>
            <asp:TextBox ID="FechaHasta" runat="server" CssClass="fecha formTextbox formCalendar" Width="102" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="LabSolicitudesEnviadas" for="SolicitudesEnviadas" class="formLabel">Solicitudes enviadas:</label>
            <asp:DropDownList ID="SolicitudesEnviadas" runat="server" CssClass="formCombobox formCombobox" Width="110" ClientIDMode="Static" Enabled="True">
            </asp:DropDownList>
        </div>

        <div class="formLinea" align="center">
            <asp:HyperLink ID="BuscarCotizacionesMeler" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
        </div>

        <div id="TablaCotizacionesMelerCargando" align="center" style="display:none">
            <asp:Image ID="icoTablaConfirmcionesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando solicitudes del lote, espere por favor...</span>
        </div>
        <div id="TablaCotizacionesMelerContenedor" style="display:none"></div>
        <div id="TablaCotizacionesMelerError" class="grilla_error" style="display:none">No se ha podido cargar la lista de solicitudes. <a id="TablaCotizacionesMelerReintentar">Intentar de nuevo</a>.</div>
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

        <%--Inicio Modal Cargando Cotizaciones--%>
        <div id="ModalGuardandoCotizacionesMeler">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCIcono" style="width:40px;height:40px" class="cargando"></td>
                        <td id="MCContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0;color:#0060a9">
                                Guardando la información, por favor espere un momento...
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <%--Fin Modal Cargando Cotizaciones--%>
    </div>
</asp:Content>
