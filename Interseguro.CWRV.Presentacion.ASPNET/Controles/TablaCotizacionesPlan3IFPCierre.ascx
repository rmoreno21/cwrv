<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesPlan3IFPCierre.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesPlan3IFPCierre" %>

<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">
        <div style="font-size: 1.6em; line-height: 200%;">Ingreso Vitalicio Seguro</div>
        <%--<br />
        <div>
            <table cellspacing="1" cellpadding="1" id="TabClausulaAdicional" style="width: 50%">
                <%--<thead>
                    <tr class="grilla_cabecera">
                        <th scope="col">Fec. Nac. Conyuge</th>
                        <th scope="col">Sexo Conyuge</th>
                        <th scope="col">Fec. Nac. Padre</th>
                        <th scope="col">Fec. Nac. Madre</th>
                    </tr>
                </thead>
                <tbody>
                    <tr class="grilla_alt1">
                        <td align="center" style="width: 90px;">
                            <asp:Label ID="lblCAFecNacConyuge" runat="server" Text="No aplica"></asp:Label>
                        </td>
                        <td align="center" style="width: 110px;">
                            <asp:Label ID="lblCASexoConyuge" runat="server" Text="No aplica"></asp:Label>
                        </td>
                        <td align="center" style="width: 60px;">
                            <asp:Label ID="lblCAFecNacPadre" runat="server" Text="No aplica"></asp:Label>
                        </td>
                        <td align="center" style="width: 70px;">
                            <asp:Label ID="lblCAFecNacMadre" runat="server" Text="No aplica"></asp:Label>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>--%>
        
        <asp:GridView ID="TabCotizaciones_IFP_P3" runat="server" Width="100%" CellPadding="0"
            CellSpacing="1" GridLines="None"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabCotizaciones_RB_RowDataBound">
            <AlternatingRowStyle CssClass="grilla_alt2" />

            <Columns>

                <asp:TemplateField>
                    <ItemTemplate>
                        <%# "<input type=\"radio\" name=\"id\" value=\"" + Eval("Correlativo") + "\" " + (Eval("IndSeleccionada").ToString() == "S" && (Eval("EstadoCotizacion").ToString() == "04" || Eval("EstadoCotizacion").ToString() == "05") ?  "checked=\"checked\"" :String.Empty) + " />" %>
                    </ItemTemplate>
                    <ItemStyle Width="23px" />
                </asp:TemplateField>

                <%--1--%>
                <asp:TemplateField HeaderText="N°">
                    <ItemTemplate>
                        <%# Container.DataItemIndex + 1 %>
                        <input type="hidden" id="custId" name="custId" value="<%#  Eval("Item").ToString() %>" class="ModSolItem">
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                </asp:TemplateField>

                <%--2--%>
                <asp:TemplateField HeaderText="Moneda">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Moneda" runat="server" Text="Moneda"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotMoneda" runat="server" Text='<%# Bind("Moneda.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="85px" />
                </asp:TemplateField>

                <%--3--%>
                <asp:TemplateField HeaderText="PG">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="PG" runat="server" Text="PG"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTemporalidad" runat="server" Text='<%# Bind("PeriodoGarantizado") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="45px" />
                </asp:TemplateField>

                <%--4--%>
                <asp:TemplateField HeaderText="Años Dif.">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Años Dif." runat="server" Text="Años Dif."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotPeriodDiferido" runat="server" Text='<%# Bind("ValPerDiferido") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="45px" />
                </asp:TemplateField>

                <%--5--%>
                <asp:TemplateField HeaderText="Años ingreso esc.">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Años ingreso esc." runat="server" Text="Años ingreso esc."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotPagoEscalonada" runat="server" Text='<%# Bind("PagoDoble") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="45px" />
                </asp:TemplateField>

                <%--6
                <asp:TemplateField HeaderText="% CA Cyge">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="% CA Cyge" runat="server" Text="% CA Cyge"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotDevolucion" runat="server" Text='<%# Eval("ValPjeCACy") + "%" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                </asp:TemplateField>

                7
                <asp:TemplateField HeaderText="% CA Padre">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="% CA Padre" runat="server" Text="% CA Padre"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotDevolucion" runat="server" Text='<%# Eval("ValPjeCAPa") + "%" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                </asp:TemplateField>

                8
                <asp:TemplateField HeaderText="% CA Madre">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="% CA Madre" runat="server" Text="% CA Madre"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotDevolucion" runat="server" Text='<%# Eval("ValPjeCAMa") + "%" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                </asp:TemplateField>

                9
                <asp:BoundField DataField="ValPjeCATotal"
                    HeaderText="Total % CA">
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:BoundField>--%>

                <%--6--%>
                <asp:TemplateField HeaderText="% Tramo 1">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="% Tramo 1" runat="server" Text="% Tramo 1"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTramoEscalonada" runat="server" Text='<%# Eval("PjePagoDoble") + "%" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="45px" />
                </asp:TemplateField>

                <%--7--%>
                <asp:TemplateField HeaderText="DCOM">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="DCOM" runat="server" Text="DCOM"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotPjeDCOM" runat="server" Text='<%# Eval("ValPjeDCOM") + "%" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                </asp:TemplateField>

                <%--
                8
                <asp:TemplateField HeaderText="Pje Conyu.">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Pje Conyu." runat="server" Text="Pje Conyu."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotPorcentajeConyuge" runat="server" Text='<%# Eval("ValPjeConyuge") + "%" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="45px" />
                </asp:TemplateField>
                --%>

                <%-- Se debe mostrar la pensión en su Moneda Original en la grilla --%>
                <%--8--%>
                <asp:TemplateField HeaderText="Renta tramo 1">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Renta tramo 1" runat="server" Text="Renta tramo 1"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="ItemRentaDoble" runat="server" Text='<%# string.Format("{0:#,##0.00}",Convert.ToDouble((Convert.ToInt32(Eval("PagoDoble"))>0)? Eval("PensionCiaMO")  :0.00)) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" Width="50px" />
                </asp:TemplateField>

                <%--9--%>
                <asp:TemplateField HeaderText="Renta mensual">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Renta mensual" runat="server" Text="Renta mensual"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="ItemRenta" runat="server" CssClass="RentaMensual" Text='<%# string.Format("{0:#,##0.00}",Convert.ToDouble((Convert.ToInt32(Eval("PagoDoble"))>0)? Eval("Pension2doTramo")  :Eval("PensionCiaMO"))) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:TemplateField>

                <%--10--%>
                <asp:BoundField DataField="TasaVentaSbs"
                    HeaderText="Tasa" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:BoundField>

                <%--11--%>
                <asp:BoundField DataField="TasaVenta"
                    HeaderText="Tasa IS" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:BoundField>

                <%--12--%>
                <asp:BoundField DataField="TasaRetornoAccionista"
                    HeaderText="TIR" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:BoundField>

                <%--13--%>
                <asp:BoundField DataField="TasaRetornoAccionistaMinima"
                    HeaderText="TIR MIN" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:BoundField>

                <%--14--%>
                <asp:TemplateField HeaderText="Dif. TRA">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Dif. TRA" runat="server" Text="Dif. TRA"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTRA" runat="server" Text='<%# Bind("AjusteTRA") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="40px" />
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
</form>
