<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimuladorRentaVitaliciaRetiroProgramado.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.SimuladorRentaVitaliciaRetiroProgramado" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:Panel ID="SimuladorData" align="center" runat="server" ClientIDMode="Static" Visible="False" style="background-color:#FFF">
        <table border="0" cellspacing="0" cellpadding="3" width="100%">
            <tr>
                <th colspan="9" align="center" class="SimulacionTitulo">Pensión en el tiempo</th>
            </tr>
            <tr class="SimulacionCabecera">
                <th rowspan="2">Variación de la<br />pensión en el tiempo</th>
                <th colspan="2">5 AÑOS</th>
                <th colspan="2">10 AÑOS</th>
                <th colspan="2">15 AÑOS</th>
                <th colspan="2">20 AÑOS</th>
            </tr>
            <tr class="SimulacionCabecera">
                <th style="width:85px">Pensión x Mes</th>
                <th style="width:75px">Acumulado</th>
                <th style="width:85px">Pensión x Mes</th>
                <th style="width:75px">Acumulado</th>
                <th style="width:85px">Pensión x Mes</th>
                <th style="width:75px">Acumulado</th>
                <th style="width:85px">Pensión x Mes</th>
                <th style="width:75px">Acumulado</th>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">RENTA VITALICIA</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RV05PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RV05ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RV10PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RV10ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RV15PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RV15ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RV20PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="RV20ACU" runat="server"></asp:Label></td>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna">RETIRO PROGRAMADO</td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RP05PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RP05ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RP10PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RP10ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RP15PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RP15ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RP20PXM" runat="server"></asp:Label></td>
                <td align="right"><asp:Label ID="RP20ACU" runat="server"></asp:Label></td>
            </tr>
        </table>

        <br />

        <div id="Grafico"></div>

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