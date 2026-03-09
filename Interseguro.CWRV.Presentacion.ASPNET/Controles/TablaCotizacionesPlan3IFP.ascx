<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesPlan3IFP.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesPlan3IFP" %>

<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">
        <div style="font-size: 1.6em; line-height: 200%;">Ingreso Vitalicio Seguro</div>
        
        <asp:GridView ID="TabCotizaciones_IFP_P3" runat="server" Width="100%" CellPadding="0"
            CellSpacing="1" GridLines="None"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabCotizaciones_RB_RowDataBound">
            <AlternatingRowStyle CssClass="grilla_alt2" />

            <Columns>

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
                    <ItemTemplate>
                        <asp:DropDownList ID="TabCotMoneda" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolMoneda" Width="85">
                        </asp:DropDownList>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="85px" />
                </asp:TemplateField>

                <%--3--%>
                <asp:TemplateField HeaderText="PG">
                    <ItemTemplate>
                        <asp:DropDownList ID="TabCotPeriodoGarantizado" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolGarantizado" Width="45">
                        </asp:DropDownList>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="45px" />
                </asp:TemplateField>

                <%--4--%>
                <asp:TemplateField HeaderText="Años Dif.">
                    <ItemTemplate>
                        <asp:DropDownList ID="TabCotPeriodDiferido" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolDiferimiento" Width="45">
                        </asp:DropDownList>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="45px" />
                </asp:TemplateField>

                <%--5--%>
                <asp:TemplateField HeaderText="Años ingreso esc.">
                    <ItemTemplate>
                        <asp:DropDownList ID="TabCotPagoEscalonada" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPagoEscalonada" Width="50">
                        </asp:DropDownList>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </asp:TemplateField>

                <%--6--%>
                <asp:TemplateField HeaderText="% Tramo 1">
                    <ItemTemplate>
                        <asp:DropDownList ID="TabCotTramoEscalonada" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolTramoEscalonada" Width="60">
                        </asp:DropDownList>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="45px" />
                </asp:TemplateField>

                <%--7--%>
                <asp:TemplateField HeaderText="DCOM">
                    <ItemTemplate>
                        <asp:DropDownList ID="TabCotPjeDCOM" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPjeDCOM" Width="60">
                        </asp:DropDownList>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                </asp:TemplateField>

                <%--8--%>
                <asp:TemplateField HeaderText="Renta tramo 1">
                    <ItemTemplate>
                        <asp:Label ID="ItemRentaDoble" runat="server" Text='<%# string.Format("{0:#,##0.00}",Convert.ToDouble((Convert.ToInt32(Eval("PjePagoDoble"))>0)? Eval("PensionCiaMO")  :0.00)) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" Width="50px" />
                </asp:TemplateField>

                <%--9--%>
                <asp:TemplateField HeaderText="Renta mensual">
                    <ItemTemplate>
                        <asp:Label ID="ItemRenta" runat="server" Text='<%# string.Format("{0:#,##0.00}",Convert.ToDouble((Convert.ToInt32(Eval("PjePagoDoble"))>0)? Eval("Pension2doTramo")  :Eval("PensionCiaMO"))) %>'></asp:Label>
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
                    <ItemTemplate>
                        <asp:TextBox ID="TabCotTRA" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid numerico ModSolTRA" Width="40" data-v-min="-100.00" data-v-max="100.00" Text="0.00" Style="text-align: right;"></asp:TextBox>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="40px" />
                </asp:TemplateField>

                <asp:TemplateField>
                    <ItemTemplate>
                        <%# "<a class=\"grilla_boton" + (PermisoEliminar ? (" grilla_eliminar\" data-cotizacion=\"" + (Container.DataItemIndex + 1)) : " grilla_eliminar_deshabilitado") + "\"></a>"%>
                    </ItemTemplate>
                    <ItemStyle Width="10px" />
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
        <asp:Panel ID="TabCotizacionesBotonera_IFP" CssClass="formLinea" align="right" runat="server">
            <asp:HyperLink ID="TabCotizacionesAgregarPlan3_IFP" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px" runat="server" ClientIDMode="Static">Agregar</asp:HyperLink>
        </asp:Panel>
    </div>
</form>
