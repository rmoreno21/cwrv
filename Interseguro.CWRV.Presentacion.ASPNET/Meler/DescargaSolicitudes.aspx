<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="DescargaSolicitudes.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Meler.DescargaSolicitudes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.meler.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div id="Principal" align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e">
        <h1 class="simple" style="width:195px">Descarga de Solicitudes</h1>

        <div class="formLinea">
            <label id="LabArchivo" for="Archivo" class="formLabel">Archivo de Solicitudes*:</label>
            <asp:FileUpload ID="Archivo" ClientIDMode="Static" runat="server" />
        </div>

        <div class="formLinea" align="center">
            <asp:Button ID="GuardarArchivo" CssClass="boton darkblue sharp" Width="80"
                Height="24" runat="server" Text="Guardar" ClientIDMode="Static" OnClick="GuardarArchivo_Click" />
        </div>
    </div>
    
    <div id="Principal2" align="left" style="width:860px;margin-top:15px;padding:20px;background:#FFF; border:1px solid #00466e">
        <h1 class="simple" style="width:160px">Búsqueda de Lotes</h1>

        <div class="formLinea">
            <label id="LabNumeroLote" for="NumeroLote" class="formLabel">Número de Lote:</label>
            <asp:TextBox ID="NumeroLote" runat="server" CssClass="enteroPositivo formTextbox" Width="60" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="LabFechaDesde" for="FechaDesde" class="formLabel formLabel2Izq">Fecha cierre desde:</label>
            <asp:TextBox ID="FechaDesde" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="8"></asp:TextBox>

            <label id="LabFechaHasta" for="FechaHasta" class="formLabel formLabel2Der">Fecha cierre hasta:</label>
            <asp:TextBox ID="FechaHasta" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="8"></asp:TextBox>
        </div>

        <div class="formLinea" align="center">
            <asp:HyperLink ID="BuscarLote" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
        </div>

        <div id="TablaLotesCargando" align="center" style="display:none">
            <asp:Image ID="icoTablaLotesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando lotes, espere por favor...</span>
        </div>
        <div id="TablaLotesContenedor" style="display:none"></div>
        <div id="TablaLotesError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de lotes. <a id="TablaLotesReintentar">Intentar de nuevo</a>.</div>
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

        <%--Inicio Modal Cargando Solicitudes--%>
        <div id="ModalGenerandoReporte" title="Generando Reporte">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MGRcono" style="width:40px;height:40px" class="cargando"></td>
                        <td id="MGRontenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MGRContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0;color:#0060a9">
                                Generando el reporte, por favor espere un momento...
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <%--Fin Modal Cargando Solicitudes--%>
    </div>
</asp:Content>
