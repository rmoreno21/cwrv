<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesRentaPrivadaPlusCierre.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesRentaPrivadaPlusCierre" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">
        <asp:GridView ID="TabCotizaciones_RP" runat="server" Width="100%" CellPadding="0"
            CellSpacing="1" GridLines="None"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabCotizaciones_RB_RowDataBound">
            <AlternatingRowStyle CssClass="grilla_alt2" />
            <Columns>

                <asp:TemplateField>
                    <ItemTemplate>
                        <%# "<input type=\"radio\" name=\"id\" value=\"" + Eval("Correlativo") + "\" " + (Eval("IndSeleccionada").ToString() == "S" && (Eval("EstadoCotizacion").ToString() == "04" || Eval("EstadoCotizacion").ToString() == "05" ) ?  "checked=\"checked\"" :String.Empty) + " />" %>
                    </ItemTemplate>
                    <ItemStyle Width="23px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="N°">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="40px" />
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
                        <%--<asp:DropDownList ID="TabCotAjusteMoneda" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolAjusteMoneda" Width="45">
                        </asp:DropDownList>--%>
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
                        <%--<asp:CheckBox ID="ChkSepelio" runat="server" ClientIDMode="Static" />--%>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="55px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="1° Tramo Años">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="1° Tramo Años" runat="server" Text="1° Tramo Años"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%--<asp:DropDownList ID="TabCotPagoEscalonada" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPagoEscalonada" Width="50">
                        </asp:DropDownList>--%>
                        <asp:Label ID="TabCotPagoEscalonada" runat="server" Text='<%#   Bind("PagoEscalonada") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="65px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="1° Tramo %">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="1° Tramo %" runat="server" Text="1° Tramo %"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%--<asp:DropDownList ID="TabCotPjePagoEscalonada" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPjePagoEscalonada" Width="60">
                        </asp:DropDownList>--%>
                        <asp:Label ID="TabCotPjePagoEscalonada" runat="server" Text='<%#   Bind("PjePE") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="75px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="% Dev.">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="% Dev." runat="server" Text="% Dev."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%--<asp:DropDownList ID="TabCotPjeDevolucion" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPjeDevolucion" Width="57">
                        </asp:DropDownList>--%>
                        <asp:Label ID="TabCotPjeDevolucion" runat="server" Text='<%#   Bind("ValPjeDev") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="75px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Per. Gar.">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Per. Gar." runat="server" Text="Per. Gar."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%--<asp:DropDownList ID="TabCotPeriodoGarantizado" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="45">
                        </asp:DropDownList>--%>
                        <asp:Label ID="TabCotPeriodoGarantizado" runat="server" Text='<%#   Bind("PeriodoGarantizado") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="65px" />
                </asp:TemplateField>

                <%--<INIGTI_753>--%>
                <asp:TemplateField HeaderText="Pje. Conyu.">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Pje. Conyu." runat="server" Text="Pje. Conyu."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%--<asp:DropDownList ID="TabCotPjeConyuge" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="50">
                        </asp:DropDownList>--%>
                        <asp:Label ID="TabCotPjeConyuge" runat="server" Text='<%#   Bind("ValPjeConyuge") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="65px" />
                </asp:TemplateField>
                <%--<FINGTI_753>--%>


                <%-- Se debe mostrar la pensión en su Moneda Original en la grilla --%>
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
                        <%--<asp:TextBox ID="TabCotTRA" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid numerico ModSolTRA" Width="35" data-v-min="-100.00" data-v-max="100.00" Text="0.00" Style="text-align: right;"></asp:TextBox>--%>
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
        

        <asp:Panel ID="TabCotizacionesLeyenda_RP" runat="server" Visible="true" style="margin:10px 0">
            <fieldset>
                <legend>Leyenda</legend>
                <div style="font-family: Calibri; font-size: 13px; color: #0060A9">
                    <span style="display: inline-block; width: 25px;">**</span>: Cotización no alcanza el mínimo requerido, por lo cual no se simula.
                </div>
            </fieldset>
        </asp:Panel>

        <%--<div style="padding: 10px 0">
        

            
    </div>--%>
    </div>
</form>
