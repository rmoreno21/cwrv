<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SimuladorPlazoFijo.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.SimuladorPlazoFijo" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:Panel ID="SimuladorData" align="center" runat="server" ClientIDMode="Static" Visible="False" style="background-color:#FFF">
       
        <table border="0" cellspacing="0"  cellpadding="3" width="100%">
            <tr>
                <th align="center" class="SimulacionTitulo">Pensión en el tiempo</th>
            </tr>
        </table>

        <div class="outer">
            <div class="inner" style="margin-left:188px;" id="divCuadro1" >
                <table border="0" cellspacing="0" cellpadding="4" width="1660px" id="tblCuadro1" >
                    <tr class="SimulacionCabecera">
                        <th style="width:180px; left:0px; position:absolute; height:20px; padding-top:6px" class="SimulacionCabecera">Valor del Fondo en el tiempo</th>
                        <th style="width: 75px; height:20px" >Año 1</th>
                        <th style="width: 75px">Año 2</th>
                        <th style="width: 75px">Año 3</th>
                        <th style="width: 75px">Año 4</th>
                        <th style="width: 75px">Año 5</th>
                        <th style="width: 75px">Año 6</th>
                        <th style="width: 75px">Año 7</th>
                        <th style="width: 75px">Año 8</th>
                        <th style="width: 75px">Año 9</th>
                        <th style="width: 75px">Año 10</th>
                        <th style="width: 75px">Año 11</th>
                        <th style="width: 75px">Año 12</th>
                        <th style="width: 75px">Año 13</th>
                        <th style="width: 75px">Año 14</th>
                        <th style="width: 75px">Año 15</th>
                        <th style="width: 75px">Año 16</th>
                        <th style="width: 75px">Año 17</th>
                        <th style="width: 75px">Año 18</th>
                        <th style="width: 75px">Año 19</th>
                        <th style="width: 75px">Año 20</th>
                    </tr>
                    <tr class="SimulacionData">
                        <td align="left" class="SimulacionPrimeraColumna" style="left:0px;position:absolute; border-bottom:1px dotted #FFF; width:180px;">Aplicación del IPC</td>

                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF ; height:20px">
                            <asp:Label ID="MOD101" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD102" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD103" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD104" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD105" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD106" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD107" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD108" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD109" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD110" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD111" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD112" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD113" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD114" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD115" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD116" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD117" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD118" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD119" runat="server"></asp:Label></td>
                        <td align="right" style="border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD120" runat="server"></asp:Label></td>
                    </tr>
                </table>
            </div>
        </div>

        <br />

        <%--Total Mensual--%>
        <table border="0" cellspacing="0"  cellpadding="3" width="100%">
            <tr>
                <th align="center" class="SimulacionTitulo">Total Mensual</th>
            </tr>
        </table>

        <div class="outer">
            <div class="inner" style="margin-left:188px;" id="divCuadro2">
                <table border="0" cellspacing="0" cellpadding="4" width="1660px" id="tblCuadro2" >
                    <tr class="SimulacionCabecera">
                        <th style="width:180px; left:0px; position:absolute; height:20px; padding-top:6px" class="SimulacionCabecera">Total Mensual</th>
                        <th style="width: 75px; height:20px" >Año 1</th>
                        <th style="width: 75px">Año 2</th>
                        <th style="width: 75px">Año 3</th>
                        <th style="width: 75px">Año 4</th>
                        <th style="width: 75px">Año 5</th>
                        <th style="width: 75px">Año 6</th>
                        <th style="width: 75px">Año 7</th>
                        <th style="width: 75px">Año 8</th>
                        <th style="width: 75px">Año 9</th>
                        <th style="width: 75px">Año 10</th>
                        <th style="width: 75px">Año 11</th>
                        <th style="width: 75px">Año 12</th>
                        <th style="width: 75px">Año 13</th>
                        <th style="width: 75px">Año 14</th>
                        <th style="width: 75px">Año 15</th>
                        <th style="width: 75px">Año 16</th>
                        <th style="width: 75px">Año 17</th>
                        <th style="width: 75px">Año 18</th>
                        <th style="width: 75px">Año 19</th>
                        <th style="width: 75px">Año 20</th>
                    </tr>

                    <%--Interes Plazo--%>
                    <tr class="SimulacionData" id ="PrimeraFila">
                        <td align="left" class="SimulacionPrimeraColumna" style="left:0px;position:absolute; border-bottom:1px dotted #FFF; width:180px;">
                            <asp:Label ID="MOD2100" runat="server" Text="Interés Plazo" ClientIDMode="Static" ></asp:Label>
                        </td>

                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF ; height:20px">
                            <asp:Label ID="MOD2101" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2102" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2103" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2104" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2105" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2106" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2107" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2108" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2109" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2110" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2111" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2112" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2113" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2114" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2115" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2116" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2117" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2118" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2119" runat="server"></asp:Label></td>
                        <td align="right" style="border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2120" runat="server"></asp:Label></td>
                    </tr>

                    <%--S/ IDX PG 15--%>
                    <tr class="SimulacionData" id="SegundaFila">
                        <td align="left" class="SimulacionPrimeraColumna" style="left:0px;position:absolute; border-bottom:1px dotted #FFF; width:180px;">
                            <asp:Label ID="MOD2200" runat="server" ClientIDMode="Static" ></asp:Label>
                        </td>

                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF ; height:20px">
                            <asp:Label ID="MOD2201" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2202" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2203" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2204" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2205" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2206" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2207" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2208" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2209" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2210" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2211" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2212" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2213" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2214" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2215" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2216" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2217" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2218" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2219" runat="server"></asp:Label></td>
                        <td align="right" style="border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2220" runat="server"></asp:Label></td>
                    </tr>
                
                    <%--IRVE S/ Ajust al TC PG 15--%>
                    <tr class="SimulacionData" id ="TerceraFila">
                        <td align="left" class="SimulacionPrimeraColumna" style="left:0px;position:absolute; border-bottom:1px dotted #FFF; width:180px;">
                            <asp:Label ID="MOD2300" runat="server" ClientIDMode="Static" ></asp:Label>
                        </td>

                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF ; height:20px">
                            <asp:Label ID="MOD2301" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2302" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2303" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2304" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2305" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2306" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2307" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2308" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2309" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2310" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2311" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2312" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2313" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2314" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2315" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2316" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2317" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2318" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2319" runat="server"></asp:Label></td>
                        <td align="right" style="border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2320" runat="server"></asp:Label></td>
                    </tr>
                
                    <%--IRVE S/ Ajust PG 15--%>
                     <tr class="SimulacionData" id="CuartaFila">
                        <td align="left" class="SimulacionPrimeraColumna" style="left:0px;position:absolute; border-bottom:1px dotted #FFF; width:180px;">
                            <asp:Label ID="MOD2400" runat="server" ClientIDMode="Static" ></asp:Label>
                        </td>

                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF ; height:20px">
                            <asp:Label ID="MOD2401" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2402" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2403" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2404" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2405" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2406" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2407" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2408" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2409" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2410" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2411" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2412" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2413" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2414" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2415" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2416" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2417" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2418" runat="server"></asp:Label></td>
                        <td align="right" style="border-right: 1px dotted #FFF; border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2419" runat="server"></asp:Label></td>
                        <td align="right" style="border-bottom: 1px dotted #FFF">
                            <asp:Label ID="MOD2420" runat="server"></asp:Label></td>
                    </tr>
                </table>
            </div>
        </div>

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