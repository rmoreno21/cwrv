<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CargaArchivosIndicadores.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Configuracion.CargaArchivosIndicadores" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.configuracion.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/jquery.ui.monthpicker.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript">

    $(function () {
        $('input.monthpicker').monthpicker({
            monthNames: ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio','julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'],
            monthNamesShort: ['ene', 'feb', 'mar', 'abr', 'may', 'jun','jul', 'ago', 'sep', 'oct', 'nov', 'dic'],
            dateFormat: 'mm/yy',
            changeYear: true,
            maxDate: "+0 Y",
            buttonText: '',
            buttonImage: "../Imagenes/calendar.png",
            prevText: '', 
            nextText: '',
            disabled: false
        });
        $('input.monthpicker').mask("99/9999");
    });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div id="Principal" align="left" style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 150px">Carga de archivos</h1>

        <div class="formLinea">
            <label id="LabFechaDesde" for="FechaDesde" class="formLabel formLabel2Izq">Periodo:</label>
            <input type="text" id="txtPeriodo" class="formTextbox monthpicker" runat="server" clientidmode="Static" autocomplete="off" />
        </div>

        <br>

        <fieldset>
            <legend>Control CDA</legend>
            <div class="formLinea" align="center">
                <asp:FileUpload ID="fuControlCDA" ClientIDMode="Static" runat="server" />
            </div>

            <div class="formLinea" align="center">
                <input type="button" id="bControlCDA" class="boton darkblue sharp" style="width: 80px; height: 24px" value="Guardar" />
                <asp:Button ID="btnControlCDA" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Guardar" ClientIDMode="Static" OnClick="btnControlCDA_Click" Style="display: none" />
            </div>
        </fieldset>

        <br>

        <fieldset>
            <legend>Localidad VCTP</legend>
            <div class="formLinea" align="center">
                <asp:FileUpload ID="fuLocalidadVCTP" ClientIDMode="Static" runat="server" />
            </div>

            <div class="formLinea" align="center">
                <input type="button" id="bLocalidadVCTP" class="boton darkblue sharp" style="width: 80px; height: 24px" value="Guardar" />
                <asp:Button ID="btnLocalidadVCTP" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Guardar" ClientIDMode="Static" OnClick="btnLocalidadVCTP_Click" Style="display: none" />
            </div>
        </fieldset>

        <br>

        <fieldset>
            <legend>Control VCTP</legend>
            <div class="formLinea" align="center">
                <asp:FileUpload ID="fuControlVCTP" ClientIDMode="Static" runat="server" />
            </div>

            <div class="formLinea" align="center">
                <input type="button" id="bControlVCTP" class="boton darkblue sharp" style="width: 80px; height: 24px" value="Guardar" />
                <asp:Button ID="btnControlVCTP" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Guardar" ClientIDMode="Static" OnClick="btnControlVCTP_Click" Style="display: none" />
            </div>
        </fieldset>
    </div>

    <div id="VentanasModales" style="display: none">
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
        <%--Inicio Modal Cuadro de advertencia--%>
        <div id="ModalCuadroAdvertencia">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCAIcono" style="width: 40px; height: 40px"></td>
                        <td id="MCAContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCAContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
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
                <a id="MCAAceptar" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MCACancelar" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="MCADataPeriodo" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>
    </div>
</asp:Content>
