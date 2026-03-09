<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesReportePlus.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesReportePlus" %>


<form id="form1" runat="server" enableviewstate="False">

    <div id="ContenidoDinamico">

        <asp:HiddenField ID="HRegistros" runat="server" ClientIDMode="Static" Value="0" />

        <asp:GridView
            ID="TabSolicitudesReportePlus"
            runat="server"
            Width="100%"
            CellPadding="1"
            CellSpacing="1"
            GridLines="None"
            UseAccessibleHeader="True"
            ViewStateMode="Disabled"
            ClientIDMode="Static"
            AutoGenerateColumns="False"
            DataKeyNames="Id"
            OnRowDataBound="TabSolicitudesReportePlus_OnRowDataBound">

            <AlternatingRowStyle CssClass="grilla_alt2" />

            <Columns>
                
                <%--Grupo Fecha--%>
                <asp:TemplateField ItemStyle-Width="16px" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" >
                   
                    <ItemTemplate>    
                         <a id='hrfdiv<%# Eval("Id").ToString() %>' href="JavaScript:divexpandcollapse('div<%# Eval("Id").ToString() %>');">
					        <img alt="Clientes" id='imgdiv<%# Eval("Id").ToString()%>' src="../Imagenes/plus-icon.png" width="15" height="15" class="toogleOficiales" />
				         </a>
                        <!-- Adding the Div container -->
                        <%--Grupo Solicitudes--%>
                        <div id='div<%# Eval("Id").ToString() %>' style="display: none;">
                            <!-- Adding Child GridView -->

                            <asp:GridView ID="TabCotizacionesReportePlus" 
                                runat="server" 
                                Width="100%" 
                                CellPadding="0" 
                                CellSpacing="1" 
                                GridLines="None"
                                UseAccessibleHeader="True"
                                ViewStateMode="Disabled" 
                                ClientIDMode="Static" 
                                AutoGenerateColumns="False"
                                DataKeyNames="NumSolicitud"
                                onrowdatabound="TabCotizacionesReportePlus_RowDataBound"
                                CssClass="TabCotizacionesReportePlus">

                                <AlternatingRowStyle CssClass="grilla_alt2" />

                                <Columns>

                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <%# 
                                            "<input class='ChkCotizacionesReportePlus' type=\"checkbox\" name=\""  + Eval("NumSolicitud").ToString() +  "\" value=\"" + Eval("Correlativo") + "\"  data-solicitud= \"" + Eval("NumSolicitud").ToString() +   "\"/>" 
                                            
                                                %>    
                                        </ItemTemplate>
                                        <ItemStyle Width="23px" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cotización">
                                        <%--<ItemTemplate>
                                            <%# Container.DataItemIndex + 1 %>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="40px" />--%>

                                        <HeaderTemplate>
                                            <asp:Label ID="LabTabCotCorrelativo" ToolTip="Cotización" runat="server" Text="Cotización"></asp:Label>
                                        </HeaderTemplate>

                                        <ItemTemplate>
                                            <asp:Label ID="TabCotCorrelativo" runat="server" Text='<%#   Bind("Correlativo") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="85px" />

                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Moneda">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" ToolTip="Moneda" runat="server" Text="Moneda"></asp:Label>
                                        </HeaderTemplate>

                                        <ItemTemplate>
                                            <asp:Label ID="TabCotMoneda" runat="server" Text='<%#   Bind("Moneda.Nombre") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="85px" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Aj. Mon.">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" ToolTip="Aj. Mon." runat="server" Text="Aj. Mon."></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="TabCotAjusteMoneda" runat="server" Text='<%#  (Eval("ValMonAju").ToString()=="-1")?"-": Eval("ValMonAju").ToString() + "%"  %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="55px" />
                                    </asp:TemplateField>


                                    <asp:TemplateField HeaderText="Sep.">
                                         <HeaderTemplate>
                                            <asp:Label ID="Label1" ToolTip="Sep." runat="server" Text="Sep."></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="TabCotSepelio" runat="server" Text='<%#   Bind("IndGastoSepelio") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="55px" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="1° Tramo Años">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" ToolTip="1° Tramo Años" runat="server" Text="1° Tramo Años"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="TabCotPagoEscalonada" runat="server" Text='<%#   Bind("PagoEscalonada") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="65px" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="2° Tramo %">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" ToolTip="2° Tramo %" runat="server" Text="2° Tramo %"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="TabCotPjePagoEscalonada" runat="server" Text='<%#   Bind("PjePE") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="75px" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="% Dev.">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" ToolTip="% Dev." runat="server" Text="% Dev."></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="TabCotPjeDevolucion" runat="server" Text='<%#   Bind("ValPjeDev") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="75px" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Per. Gar.">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" ToolTip="Per. Gar." runat="server" Text="Per. Gar."></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="TabCotPeriodoGarantizado" runat="server" Text='<%#   Bind("PeriodoGarantizado") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="65px" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Pje. Conyu.">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" ToolTip="Pje. Conyu." runat="server" Text="Pje. Conyu."></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="TabCotPjeConyuge" runat="server" Text='<%#   Bind("ValPjeConyuge") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="65px" />
                                    </asp:TemplateField>
                
                
                                    <asp:BoundField DataField="PensionCiaMO"
                                        HeaderText="Renta" DataFormatString="{0:#,##0.00}">
                                        <ItemStyle HorizontalAlign="Right" Width="120px" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="TasaVentaSbs"
                                        HeaderText="Tasa Vta" DataFormatString="{0:#,##0.00}">
                                        <ItemStyle HorizontalAlign="Right" Width="70px" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Pension2doTramoSinAjuste"
                                        HeaderText="Renta 2° Tramo" DataFormatString="{0:#,##0.00}">
                                        <ItemStyle HorizontalAlign="Right" Width="120px" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="Pension2doTramo"
                                        HeaderText="Renta 2° Tramo Aj." DataFormatString="{0:#,##0.00}">
                                        <ItemStyle HorizontalAlign="Right" Width="120px" />
                                    </asp:BoundField>


                                    <asp:BoundField DataField="TasaVenta"
                                        HeaderText="Tas. Vta IS" DataFormatString="{0:#,##0.00}">
                                        <ItemStyle HorizontalAlign="Right" Width="60px" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="TasaRetornoAccionista"
                                        HeaderText="TRA" DataFormatString="{0:#,##0.00}">
                                        <ItemStyle HorizontalAlign="Right" Width="60px" />
                                    </asp:BoundField>


                                    <asp:BoundField DataField="IndCotiza"
                                        HeaderText="Ind.">
                                        <ItemStyle HorizontalAlign="Right" Width="60px" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Dif. TRA">
                                        <HeaderTemplate>
                                            <asp:Label ID="Label1" ToolTip="Dif. TRA" runat="server" Text="Dif. TRA"></asp:Label>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="TabCotPjeConyuge" runat="server" Text='<%#   Bind("AjusteTRA") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="45px" />
                                    </asp:TemplateField>

                                </Columns>

                                <EmptyDataTemplate>
                                    <div class="grilla_info">
                                        No se ha encontrado ningún registro de cotización.
                                    </div>
                                </EmptyDataTemplate>
                                <HeaderStyle CssClass="grilla_cabecera" />
                                <RowStyle CssClass="grilla_alt1" />
                            </asp:GridView>
                            
                        </div>


                    </ItemTemplate>
                    
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="">
                    <ItemTemplate>
                        <%#
                            "<input class='ChkSolicitudesReportePlus' type=\"checkbox\"  name=\""  + Eval("Id").ToString() +  "\" ID=\"ChkSolicitudesReportePlus\" data-solicitud=\"" + Eval("Id").ToString() + "\"/>" 
                        %>      
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="20px" />
                </asp:TemplateField>
                 
                <asp:TemplateField HeaderText="Numero Solicitud" HeaderStyle-HorizontalAlign="Left">
                    <ItemTemplate>
                        <%# "<a  class='NumSolicitudData' data-fecha='div" + Eval("Id").ToString() +  "'></a>"%>
                        <asp:Label ID="NumSolicitud"  runat="server" Text='<%# Eval("Id").ToString() %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

<%--
                    <asp:BoundField DataField="Id"
                       HeaderText="Solicitud" />--%>
                <asp:BoundField DataField="FechaSolicitud"
                       HeaderText="Fec. Solicitud" DataFormatString="{0:dd/MM/yyyy}" >
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Tipo Cotización">
                    <ItemTemplate>
                        <asp:Label ID="ItemTipoCotizacion" runat="server" Text='<%# Bind("TipoCotizacion.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FechaDevengue"
                       HeaderText="Fec. Devengue" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

            

                <asp:TemplateField HeaderText="Mon.">
                    <ItemTemplate>
                        <asp:Label ID="ItemMoneda" runat="server" Text='<%# Bind("MonedaPrimaUnica.Simbolo") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="PrimaUnica"
                       HeaderText="Prima Única" DataFormatString="{0:#,##0.00}"> 
                    <ItemStyle HorizontalAlign="Right"/>
                </asp:BoundField>

                <asp:TemplateField HeaderText="Temporalidad">
                    <ItemTemplate>
                        <asp:Label ID="ItemTemporalidad" runat="server" Text='<%# Bind("Temporalidad.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
            <EmptyDataTemplate>
                <div class="grilla_info">
                    No se ha encontrado ningún registro de ventas.
                </div>
            </EmptyDataTemplate>
            <HeaderStyle CssClass="grilla_cabecera" />
            <RowStyle CssClass="grilla_alt1" />
        </asp:GridView>



        <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
            <div class="grilla_error" align="left" style="width: 375px">
                Usted no cuenta con privilegios para visualizar esta información.
            </div>
        </asp:Panel>

    </div>
</form>
