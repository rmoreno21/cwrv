<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="Seguimiento.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Reportes.Seguimiento" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">
        <h1 class="simple" style="width:190px">Reporte de seguimiento</h1>

        <asp:Panel ID="ControlJefe" CssClass="formLinea" runat="server" ClientIDMode="Static">
            <label id="LabJefe" class="formLabel formLabel2Izq">Jefe:</label>
            <asp:DropDownList ID="Jefe" runat="server" CssClass="formCombobox" Width="356" Enabled="False" ClientIDMode="Static">
            </asp:DropDownList>
            <asp:HiddenField ID="HJefe" runat="server" ClientIDMode="Static" />
        </asp:Panel>

        <asp:Panel ID="ControlSupervisor" CssClass="formLinea" runat="server" ClientIDMode="Static">
            <label id="LabSupervisor" class="formLabel formLabel2Izq">Supervisor:</label>
            <asp:DropDownList ID="Supervisor" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="356" Enabled="False" ClientIDMode="Static">
            </asp:DropDownList>
            <span id="CargandoSupervisor" class="paginador_cargando"></span>
            <asp:HiddenField ID="HSupervisor" runat="server" ClientIDMode="Static" />
        </asp:Panel>

        <asp:Panel ID="ControlAgente" CssClass="formLinea" runat="server" ClientIDMode="Static">
            <label id="LabAgente" class="formLabel formLabel2Izq">Agente:</label>
            <asp:DropDownList ID="Agente" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="356" Enabled="False" ClientIDMode="Static">
            </asp:DropDownList>
            <span id="CargandoAgente" class="paginador_cargando"></span>
            <asp:HiddenField ID="HAgente" runat="server" ClientIDMode="Static" />
        </asp:Panel>

        <asp:Panel ID="ControlCUSPP" CssClass="formLinea" runat="server" ClientIDMode="Static">
            <label id="LabCUSPP" class="formLabel formLabel2Izq">CUSPP:</label>
            <asp:TextBox ID="CUSPP" runat="server" CssClass="formTextbox alfanumerico" style="text-transform:uppercase" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
            <asp:HiddenField ID="HCUSPP" runat="server" ClientIDMode="Static" />
        </asp:Panel>

        <div class="formLinea">
            <label id="LabFechaInicio" class="formLabel formLabel2Izq">Fecha de Inicio*:</label>
            <asp:TextBox ID="FechaInicio" runat="server" CssClass="formTextbox formCalendar" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
            <asp:HiddenField ID="HFechaInicio" runat="server" ClientIDMode="Static" />

            <label id="LabFechaTermino" class="formLabel formLabel2Der">Fecha de Término*:</label>
            <asp:TextBox ID="FechaTermino" runat="server" CssClass="formTextbox formCalendar" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
            <asp:HiddenField ID="HFechaTermino" runat="server" ClientIDMode="Static" />
        </div>

        <div class="formLinea" align="center">
            <asp:HyperLink ID="Buscar" NavigateUrl="#" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
        </div>

        <br />

        <div id="ContenedorExportar" class="formLinea" align="right" style="display:none">
            <asp:Button ID="ExportarExcel" runat="server" Text="Exportar a Excel" Width="130" Height="22"
                CssClass="boton darkblue sharp" ClientIDMode="Static" onclick="ExportarExcel_Click" UseSubmitBehavior="False" />
        </div>

        <div id="Cargando" align="center" style="display:none">
            <asp:Image ID="icoCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Buscando solicitudes, espere por favor...</span>
        </div>
        <div id="TablaSeguimientoError" align="center" style="display:none">
            <div class="grilla_error" align="left" style="width:385px;">
                No se ha podido cargar la tabla de seguimiento. <a id="TablaSeguimientoReintentar" href="#">Intentar de nuevo</a>.
            </div>
        </div>

        <div id="TablaSeguimiento" runat="server" style="display:none" clientidmode="Static"></div>

        <asp:HiddenField ID="TabSeguimientoIndicePagina" runat="server" ClientIDMode="Static" Value="1" />
        <asp:HiddenField ID="TabSeguimientoTamanhoPagina" runat="server" ClientIDMode="Static" Value="10" />
        <asp:HiddenField ID="TabSeguimientoColumnaOrdenar" runat="server" ClientIDMode="Static" Value="1" />
        <asp:HiddenField ID="TabSeguimientoDireccionOrdenar" runat="server" ClientIDMode="Static" Value="A" />
    </div>

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
