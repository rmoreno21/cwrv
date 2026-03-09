<%@ Page Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="IndicadorCdA.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Reportes.IndicadorCdA" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/jquery.ui.monthpicker.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript">
        $(function () {
            $('input.monthpicker').monthpicker({
                monthNames: ['enero', 'febrero', 'marzo', 'abril', 'mayo', 'junio', 'julio', 'agosto', 'septiembre', 'octubre', 'noviembre', 'diciembre'],
                monthNamesShort: ['ene', 'feb', 'mar', 'abr', 'may', 'jun', 'jul', 'ago', 'sep', 'oct', 'nov', 'dic'],
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

            $("#btnDescargarCDA").live("click", function (e) {
                e.preventDefault();

                var periodo = $.trim($("#txtPeriodo").val())

                if (periodo == '') {
                    $("#MCMIcono").attr("class", "error");
                    $("#MCMContenedor").html("No se ha indicado el periodo para la generación del reporte.");
                    $("#ModalCuadroMensaje").dialog({ title: "Error" });
                    $("#ModalCuadroMensaje").dialog("open");
                    return;
                }

                var params = {
                    tokenUsuario: $("#TokenUsuario").val(),
                    periodo
                }

                $.ajax({
                    type: "POST",
                    url: "IndicadorCdA.aspx/descargarReporteCDA",
                    contentType: "application/json; charset=iso-8859-1",
                    dataType: "json",
                    data: $.toJSON(params),
                    success: function (data) {
                        if (data.d.Estado == 'OK') {
                            $("#MCMIcono").attr("class", data.d.Icono);
                            $("#MCMContenedor").html(data.d.Mensaje);
                            $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                            $("#ModalCuadroMensaje").dialog("open");

                            if (data.d.Contenido == 'SiData') {
                                window.open('../Reportes/ReporteIndicadorCDA.aspx');
                            }
                        }
                        else if (data.d.Estado == "ERROR") {
                            $("#MCMIcono").attr("class", data.d.Icono);
                            $("#MCMContenedor").html(data.d.Mensaje);
                            $("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
                            $("#ModalCuadroMensaje").dialog("open");
                        }
                        else if (data.d.Estado == "TOKEN") {
                            CerrarSesionExpirada();
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        console.log(XMLHttpRequest);
                        console.log(textStatus);
                        console.log(errorThrown);

                        if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                            /* Sesión caducada */
                            document.location.reload(true);
                        }

                        $("#MCMIcono").attr("class", "error");
                        $("#MCMContenedor").html("Ha ocurrido un error al generar el reporte.");
                        $("#ModalCuadroMensaje").dialog({ title: "Error" });
                        $("#ModalCuadroMensaje").dialog("open");
                    }
                });
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div id="Principal" align="left" style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 210px">Reporte Indicadores CDA</h1>

        <div class="formLinea">
            <label id="LabPeriodo" for="txtPeriodo" class="formLabel">Periodo:</label>
            <input type="text" id="txtPeriodo" class="formTextbox monthpicker" runat="server" clientidmode="Static" autocomplete="off" />
        </div>
        <div class="formLinea" align="center">
            <asp:Button ID="btnDescargarCDA" CssClass="boton darkblue sharp" Width="80"
                Height="24" runat="server" Text="Descargar" ClientIDMode="Static" />
        </div>
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
    </div>
</asp:Content>
