<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimuladorJubilarseHoyFuturo.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.SimuladorJubilarseHoyFuturo" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:Panel ID="SimuladorData" align="center" runat="server" ClientIDMode="Static" Visible="False" style="background-color:#FFF">
        <table border="0" cellspacing="0" cellpadding="3" width="100%">
            <tr>
                <th colspan="4" align="center" class="SimulacionTitulo"><asp:Label ID="GlsMoneda" runat="server"></asp:Label></th>
            </tr>
            <tr class="SimulacionCabecera">
                <th><strong>Línea de Tiempo</strong></th>
                <th align="center" style="width:93px"><strong>HOY</strong><br />(Edad Actual)</th>
                <th align="center" style="width:93px"><strong>EN <asp:Label ID="AnhosJub" runat="server"></asp:Label> AÑOS</strong><br />(Edad Estimada)</th>
                <%--SRI.INI-20322--%>
                <%--<th align="center" style="width:93px"><strong>EN <asp:Label ID="AnhosMax" runat="server"></asp:Label> AÑOS</strong><br />(Diferencia)</th>--%>
                <%--<th align="center" style="width:93px"><strong>DIFERENCIA</strong></th>--%>
                <%--SRI.FIN-20322--%>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">Edad del pensionista</td>
                <td align="center" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="EdadHoy" runat="server"></asp:Label></td>
                <td align="center" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="EdadJub" runat="server"></asp:Label></td>
                <%--SRI.INI-20322--%>
                <%--<td align="center" style="border-bottom:1px dotted #FFF"><asp:Label ID="EdadMax" runat="server"></asp:Label></td>--%>
                <%--SRI.FIN-20322--%>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">Saldo CIC</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="SaldoCICHoy" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="SaldoCICJub" runat="server"></asp:Label></td>
                <%--SRI.INI-20322--%>
                <%--<td style="border-bottom:1px dotted #FFF"></td>--%>
                <%--SRI.FIN-20322--%>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">Pensión mensual</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="PensionMensualHoy" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="PensionMensualJub" runat="server"></asp:Label></td>
                <%--SRI.INI-20322--%>
                <%--<td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="PensionMensualDif" runat="server"></asp:Label></td>--%>
                <%--SRI.FIN-20322--%>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna">Pensión anual</td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="PensionAnualHoy" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="PensionAnualJub" runat="server"></asp:Label></td>
                <%--SRI.INI-20322--%>
                <%--<td align="right"><asp:Label ID="PensionAnualDif" runat="server"></asp:Label></td>--%>
                <%--SRI.FIN-20322--%>
            </tr>
            <tr class="SimulacionBlanco">
                <td>&nbsp;</td>
                <td></td>
                <td></td>
                <%--SRI.INI-20322--%>
                <%--<td></td>--%>
                <%--SRI.FIN-20322--%>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna">Pensión Total</td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="PensionTotalHoy" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="PensionTotalJub" runat="server"></asp:Label></td>
                <%--SRI.INI-20322--%>
                <%--<td align="right"><asp:Label ID="PensionTotalDif" runat="server"></asp:Label></td>--%>
                <%--SRI.FIN-20322--%>
            </tr>
        </table>

        <asp:Literal ID="Grafico" runat="server"></asp:Literal>
        
        <br />

        <span class="SimuladorPie">Los datos contenidos en este reporte de simulación son referenciales, siendo los montos oficiales de pensión los contenidos en la sección IV, Acta de presentación de cotizaciones de pensión que le será presentado por su AFP.</span>
    </asp:Panel>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para ejecutar este reporte.
        </div>
    </asp:Panel>
</div>
</form>