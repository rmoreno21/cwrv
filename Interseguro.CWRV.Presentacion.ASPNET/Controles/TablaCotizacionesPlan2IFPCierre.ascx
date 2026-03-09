<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesPlan2IFPCierre.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesPlan2IFPCierre" %>

<form id="form1" runat="server" enableviewstate="False">

    <div id="ContenidoDinamico">

        <div style="font-size: 1.6em; line-height: 200%;">Ingreso Seguro con Devolución</div>

        <asp:GridView ID="TabCotizaciones_IFP_P2" runat="server" Width="100%" CellPadding="0"
            CellSpacing="1" GridLines="None"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabCotizaciones_RB_RowDataBound">

            <AlternatingRowStyle CssClass="grilla_alt2" />

            <Columns>

                <asp:TemplateField>
                    <ItemTemplate>
                        <%# "<input type=\"radio\" name=\"id\" value=\"" + Eval("Correlativo") + "\" " + (Eval("IndSeleccionada").ToString() == "S" && (Eval("EstadoCotizacion").ToString() == "04" || Eval("EstadoCotizacion").ToString() == "05")  ?  "checked=\"checked\"" :String.Empty) + " />" %>
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
                <asp:TemplateField HeaderText="Plazo y PG">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Plazo y PG" runat="server" Text="Plazo y PG"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTemporalidad" runat="server" Text='<%# Bind("Temporalidad.Anhos") %>'></asp:Label>
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
                <asp:TemplateField HeaderText="% Devol.">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="% Devol." runat="server" Text="% Devol."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotDevolucion" runat="server" Text='<%# Eval("ValPjeDev") + "%" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                </asp:TemplateField>

                <%--8--%>
                <asp:TemplateField HeaderText="DCOM">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="DCOM" runat="server" Text="DCOM"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotPjeDCOM" runat="server" Text='<%# Eval("ValPjeDCOM") + "%" %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Res">
                    <ItemTemplate>
                        <asp:Label ID="TabCotRescate" runat="server" ClientIDMode="Static" ></asp:Label>
                        <%--<asp:CheckBox ID="TabCotRescate" runat="server" ClientIDMode="Static" CssClass="ModSolRescate" Width="30" />--%>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="30px" />
                </asp:TemplateField>

                <%-- Se debe mostrar la pensión en su Moneda Original en la grilla --%>

                <%--9--%>
                <asp:TemplateField HeaderText="Renta tramo 1">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Renta tramo 1" runat="server" Text="Renta tramo 1"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="ItemRentaDoble" runat="server" Text='<%# string.Format("{0:#,##0.00}",Convert.ToDouble((Convert.ToInt32(Eval("PagoDoble"))>0)? Eval("PensionCiaMO")  :0.00)) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" Width="50px" />
                </asp:TemplateField>

                <%--10--%>
                <asp:TemplateField HeaderText="Renta mensual">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Renta mensual" runat="server" Text="Renta mensual"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="ItemRenta" runat="server" CssClass="RentaMensual" Text='<%# string.Format("{0:#,##0.00}",Convert.ToDouble((Convert.ToInt32(Eval("PagoDoble"))>0)? Eval("Pension2doTramo")  :Eval("PensionCiaMO"))) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:TemplateField>

                <%--11--%>
                <asp:BoundField DataField="TasaVentaSbs"
                    HeaderText="Tasa" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:BoundField>

                <%--12--%>
                <asp:BoundField DataField="TasaVenta"
                    HeaderText="Tasa IS" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:BoundField>

                <%--13--%>
                <asp:BoundField DataField="TasaRetornoAccionista"
                    HeaderText="TIR" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:BoundField>

                <%--14--%>
                <asp:BoundField DataField="TasaRetornoAccionistaMinima"
                    HeaderText="TIR MIN" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" Width="40px" />
                </asp:BoundField>

                <%--15--%>
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

        <%--<asp:Panel ID="TabCotizacionesLeyenda_RP" runat="server" Visible="true">
            <fieldset>
                <legend>Leyenda</legend>
                <div style="font-family: Calibri; font-size: 13px; color: #0060A9">
                    <span style="display: inline-block; width: 25px;">**</span>: Cotización no alcanza el mínimo requerido, por lo cual no se simula.
                </div>
            </fieldset>
        </asp:Panel>--%>

        <%--<div style="padding: 10px 0">
        </div>--%>
    </div>

</form>
