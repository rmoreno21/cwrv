<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CondicionesAnticipo.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.CondicionesAnticipo" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.generarPdf.js")%>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.anticipos.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            // Cargar datos del anticipo desde sessionStorage
            var anticipoData = sessionStorage.getItem('Anticipo');
            if (anticipoData) {
                var anticipo = JSON.parse(anticipoData);
                console.log("Datos del anticipo:", anticipo);

                // Poblar los controles con los datos
                $("#NumeroSolicitud").val(anticipo.solicitud.id);
                $("#NombreAgente").text(anticipo.agente.nombre);
                $("#IdAgente").text(anticipo.agente.id);
                $("#MontoACOM").text(formatMoney(anticipo.monto));
                $("#NombreAfiliado").text(anticipo.afiliado.nombre);
                $("#CUSPP").text(anticipo.afiliado.CUSPP);
                $("#MesesIngreso").text(anticipo.mesesIngreso + " mes" + (anticipo.mesesIngreso != 1 ? "es" : ""));
                $("#MontoMaximo").text(formatMoney(anticipo.montoMaximo));
                $("#DiasDevolucion").text(anticipo.diasDevolucion + " día" + (anticipo.diasDevolucion != 1 ? "s" : ""));
                $("#NombreAgente2").text(anticipo.agente.nombre);
                $("#Fecha").text(new Date().toLocaleDateString('es-ES'));

                $("#Mensaje").val(anticipo.mensaje);

                if (anticipo.mensaje == "R") {
                    habilitarModoSolicitudProcesada();
                }

            }
            else {
                // Si no hay datos, redirigir de vuelta
                window.location.href = "../Cotizador/CotizadorOficiales.aspx";
            }
        });
        
        function formatMoney(amount) {
            return new Intl.NumberFormat('es-PE', {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            }).format(amount);
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div id="Principal" align="left" style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width:163px">Solicitud de Anticipo</h1>

        <h2 class="anticipo">CONDICIONES PARA EL ANTICIPO DE COMISIÓN</h2>
        
        <ul class="anticipo">
            <li>El suscrito, <asp:Label ID="NombreAgente" runat="server" ClientIDMode="Static"></asp:Label> con código <asp:Label ID="IdAgente" runat="server" ClientIDMode="Static"></asp:Label> declara estar recibiendo la suma de S/ <asp:Label ID="MontoACOM" runat="server" ClientIDMode="Static"></asp:Label> en calidad de anticipo de comisión.</li>
            <li>Dicho monto será considerado como pago a cuenta de las comisiones generadas por la colocación de una renta vitalicia por el afiliado <asp:Label ID="NombreAfiliado" runat="server" ClientIDMode="Static"></asp:Label> con CUSPP <asp:Label ID="CUSPP" runat="server" ClientIDMode="Static"></asp:Label> y liquidado en la planilla de comisiones del mes correspondiente.</li>
            <li>En caso mi fecha de ingreso sea menor a <asp:Label ID="MesesIngreso" runat="server" ClientIDMode="Static"></asp:Label> y/o el monto del importe sea superior a S/ <asp:Label ID="MontoMaximo" runat="server" ClientIDMode="Static"></asp:Label> Inteseguro emitirá el importe del pago a nombre de mi Supervisor o Jefe.</li>
            <li>Si como resultado del proceso de elección el afiliado no optara por Interseguro, acepto las condiciones de devolver el monto del anticipo en un plazo no mayor de <asp:Label ID="DiasDevolucion" runat="server" ClientIDMode="Static"></asp:Label> contados desde la fecha de la firma de la Sección V, a través del depósito en cuenta.</li>
            <li>De no cumplir con dicho compromiso en el plazo indicado autorizo a Interseguro descontar de mis haberes y/o liquidación de beneficios sociales la suma antes indicada, al margen de las restricciones que pueda producirse para futuros anticipos.</li>
        </ul>

        <div class="anticipo">
            <label for="Acepto" class="anticipo"><asp:CheckBox ID="Acepto" runat="server" ClientIDMode="Static" /> Acepto las condiciones</label>
        </div>
        <div class="anticipo">
            Nombre del Agente: <asp:Label ID="NombreAgente2" runat="server" ClientIDMode="Static"></asp:Label>
        </div>
        <div class="anticipo">
            Fecha: <asp:Label ID="Fecha" runat="server" ClientIDMode="Static"></asp:Label>
        </div>

        <div class="formLinea" align="center">
            <asp:HiddenField ID="NumeroSolicitud" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="Mensaje" runat="server" ClientIDMode="Static" />
            <input type="hidden" id="url_api_rentas_rv" value="<%= System.Configuration.ConfigurationManager.AppSettings["ApiCotizadorRvUrl"] %>" />
            <input type="hidden" id="url_api_reportes" value="<%= System.Configuration.ConfigurationManager.AppSettings["ApiReportesUrl"] %>" />
            <input type="hidden" id="usuario_actual" value="<%= Session["Usuario"] %>" />
            <input type="hidden" id="rol_azman" value="<%= Session["RolAzman"] %>" />
            <asp:Button ID="GuardarSolicitud" CssClass="boton darkblue sharp" Width="135" Height="28" runat="server" Text="Guardar Solicitud" ClientIDMode="Static" />
            <asp:HyperLink ID="ImprimirFormato" CssClass="boton darkblue sharp" Width="135" Height="26" runat="server" ClientIDMode="Static" style="display: none;">Imprimir Formato</asp:HyperLink>
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
