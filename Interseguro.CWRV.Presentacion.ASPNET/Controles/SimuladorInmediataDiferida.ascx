<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimuladorInmediataDiferida.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.SimuladorInmediataDiferida" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:Panel ID="SimuladorData" align="center" runat="server" ClientIDMode="Static" Visible="False" style="background-color:#FFF">
        <table border="0" cellspacing="0" cellpadding="3" width="100%">
            <tr>
                <th colspan="4" align="center" class="SimulacionTitulo">Recuperación del dinero de su fondo en el tiempo</th>
            </tr>
            <tr class="SimulacionCabecera">
                <th style="width:200px"></th>
                <th>15 AÑOS</th>
                <th>20 AÑOS</th>
                <th>25 AÑOS</th>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">RENTA VITALICIA INMEDIATA</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RVI15" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RVI20" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="RVI25" runat="server"></asp:Label></td>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">RENTA DIFERIDA A 1 AÑO</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RD115" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RD120" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="RD125" runat="server"></asp:Label></td>
            </tr>
            <%--SRI.INI-20322--%>
            <%--<tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna">RENTA DIFERIDA A 2 AÑOS</td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RD215" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RD220" runat="server"></asp:Label></td>
                <td align="right"><asp:Label ID="RD225" runat="server"></asp:Label></td>
            </tr>--%>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">RENTA DIFERIDA A 2 AÑOS</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RD215" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RD220" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="RD225" runat="server"></asp:Label></td>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">RENTA DIFERIDA A 3 AÑOS</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RD315" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RD320" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="RD325" runat="server"></asp:Label></td>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna" style="border-bottom:1px dotted #FFF">RENTA DIFERIDA A 4 AÑOS</td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RD415" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF;border-bottom:1px dotted #FFF"><asp:Label ID="RD420" runat="server"></asp:Label></td>
                <td align="right" style="border-bottom:1px dotted #FFF"><asp:Label ID="RD425" runat="server"></asp:Label></td>
            </tr>
            <tr class="SimulacionData">
                <td align="left" class="SimulacionPrimeraColumna">RENTA DIFERIDA A 5 AÑOS</td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RD515" runat="server"></asp:Label></td>
                <td align="right" style="border-right:1px dotted #FFF"><asp:Label ID="RD520" runat="server"></asp:Label></td>
                <td align="right"><asp:Label ID="RD525" runat="server"></asp:Label></td>
            </tr>
            <%--SRI.INI-20322--%>
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
