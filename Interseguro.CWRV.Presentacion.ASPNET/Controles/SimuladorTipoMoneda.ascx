<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimuladorTipoMoneda.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.SimuladorTipoMoneda" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:Panel ID="SimuladorData" align="center" runat="server" ClientIDMode="Static" Visible="False" style="background-color:#FFF">
        <table border="0" cellspacing="0" cellpadding="3" width="100%">
            <tr>
                <th align="center" colspan="4" class="SimulacionTitulo">Comparación por Tipo de Moneda</th>
            </tr>
            <tr class="SimulacionCabecera">
                <th style="width:200px"></th>
                <th>15 AÑOS</th>
                <th>20 AÑOS</th>
                <th>25 AÑOS</th>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">S/. INDEXADOS</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="VAC15" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="VAC20" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="VAC25" runat="server"></asp:Label></td>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">S/. AJUSTADOS</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="SAJ15" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="SAJ20" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="SAJ25" runat="server"></asp:Label></td>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna">$ AJUSTADOS</td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="DAJ15" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="DAJ20" runat="server"></asp:Label></td>
                <td align="right"><asp:Label ID="DAJ25" runat="server"></asp:Label></td>
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
