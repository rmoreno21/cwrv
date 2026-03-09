<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimuladorQueMeConviene.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.SimuladorQueMeConviene" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:Panel ID="SimuladorData" align="center" runat="server" ClientIDMode="Static" Visible="False" style="background-color:#FFF">
        <table border="0" cellspacing="0" cellpadding="3" width="100%">
            <tr>
                <th colspan="11" align="center" class="SimulacionTitulo">Pensión en el tiempo</th>
            </tr>
            <tr class="SimulacionCabecera">
                <th rowspan="2">MODALIDADES</th>
                <th colspan="2">5 AÑOS</th>
                <th colspan="2">10 AÑOS</th>
                <th colspan="2">15 AÑOS</th>
                <th colspan="2">20 AÑOS</th>
                <th colspan="2">25 AÑOS</th>
            </tr>
            <tr class="SimulacionCabecera">
                <th style="width:85px">Pen. x Mes</th>
                <th style="width:75px">Acumulado</th>
                <th style="width:85px">Pen. x Mes</th>
                <th style="width:75px">Acumulado</th>
                <th style="width:85px">Pen. x Mes</th>
                <th style="width:75px">Acumulado</th>
                <th style="width:85px">Pen. x Mes</th>
                <th style="width:75px">Acumulado</th>
                <th style="width:85px">Pen. x Mes</th>
                <th style="width:75px">Acumulado</th>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">MODALIDAD 1</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD105PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD105ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD110PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD110ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD115PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD115ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD120PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD120ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD125PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="MOD125ACU" runat="server"></asp:Label></td>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna">MODALIDAD 2</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD205PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD205ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD210PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD210ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD215PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD215ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD220PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD220ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="MOD225PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="MOD225ACU" runat="server"></asp:Label></td>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna">MODALIDAD 3</td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="MOD305PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="MOD305ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="MOD310PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="MOD310ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="MOD315PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="MOD315ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="MOD320PXM" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="MOD320ACU" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="MOD325PXM" runat="server"></asp:Label></td>
                <td align="right"><asp:Label ID="MOD325ACU" runat="server"></asp:Label></td>
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