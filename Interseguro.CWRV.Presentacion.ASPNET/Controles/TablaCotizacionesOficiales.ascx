<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesOficiales.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesOficiales" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">
        <asp:GridView ID="TabCotizaciones" runat="server" Width="100%" CellPadding="1"
            CellSpacing="1" GridLines="None"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabCotizaciones_RowDataBound">
            <AlternatingRowStyle CssClass="grilla_alt2" />
            <Columns>

                <asp:TemplateField>
                    <ItemTemplate>
                        <%# "<input type=\"radio\" name=\"id\" value=\"" + Eval("Correlativo") + "\" " + (Eval("Correlativo").ToString() != CotizacionElegida.ToString() ? String.Empty : "checked=\"checked\"") + " />" %>
                    </ItemTemplate>
                    <ItemStyle Width="23px" />
                </asp:TemplateField>

                <asp:TemplateField HeaderText="N°"><%--<INIGTI_4081>--%>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="N° Correlativo" runat="server" Text="N°"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotCorrelativo" runat="server" Text='<%#   Bind("Correlativo") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>

<%--                <asp:BoundField DataField="Correlativo" HeaderText="N°">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>--%>


                <asp:TemplateField HeaderText="Moneda">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Moneda" runat="server" Text="Moneda"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotMoneda" runat="server" Text='<%#   Bind("Moneda.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Producto" Visible="false"><%--<INIGTI_4081>--%>
                    <ItemTemplate>
                        <asp:Label ID="TabCotProducto" runat="server" Text='<%# Bind("Producto.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Mod">
                    <HeaderTemplate>
                        <asp:Label ToolTip="Modalidad" runat="server" Text="Mod."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotModalidad" runat="server" Text='<%# Bind("Modalidad.Id") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Periodo Diferido" runat="server" Text="P. Dif"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotPeriodoDiferido" runat="server" Text='<%# Bind("PeriodoDiferido") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>


                <%--<asp:BoundField DataField="PeriodoDiferido" HeaderText="P. Dif.">

                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>--%>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Porcentaje entre  Rentas" runat="server" Text="Pje. Rent."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotPorcentajeEntreRentas" runat="server" Text='<%# Bind("PorcentajeEntreRentas") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>

                <%-- <asp:BoundField DataField="PorcentajeEntreRentas" HeaderText="Pje. Rent.">
                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>--%>


                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Periodo Garantizado" runat="server" Text="P. Gar."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotPeriodoGarantizado" runat="server" Text='<%# Bind("PeriodoGarantizado") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>

                <%--<asp:BoundField DataField="PeriodoGarantizado" HeaderText="P. Gar.">
                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>--%>

                <asp:TemplateField HeaderText="Gratif." Visible="false"><%--<INIGTI_4081>--%>
                    <ItemTemplate>
                        <asp:Label ID="TabCotGratificacion" runat="server" Text='<%# (Convert.ToBoolean(Eval("Gratificacion")) ? "Sí" : "No") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>

                <%--9--%>
                <asp:TemplateField HeaderText="Capital" Visible="false"><%--<INIGTI_4081>--%>
                    <ItemTemplate>
                        <asp:Label ID="TabCotCapital" runat="server" Text='<%# Bind("Capital.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Left" />
                </asp:TemplateField>

                
                <%--<INIGTI_4081>--%>
                <%--10--%>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Tasa de Venta SBS Original" runat="server" Text="Tas Vta SBS Ori."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTasaVentaSBSOriginal" runat="server" Text='<%# Bind("TasaVentaSBSOrigen") %>' CssClass="numerico"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>


                <%--11--%>
                <asp:TemplateField Visible="false">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Tasa de Venta" runat="server" Text="Tas Vta"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTasaVenta" runat="server" Text='<%# Bind("TasaVenta") %>' CssClass="numerico"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>


                <%--12--%>
                <asp:TemplateField Visible="false">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Tasa de Venta SBS" runat="server" Text="Tas Vta SBS"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTasaVentaSBS" runat="server" Text='<%# Bind("TasaVentaSBS") %>' CssClass="numerico"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>

                <%--13--%>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Tasa SBS Esperada" runat="server" Text="Tas SBS Esper."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTasaVentaSbsObjetivo" runat="server" Text='<%# Bind("TasaVentaSbsObjetivo") %>' CssClass="numerico"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>

                <%--14--%>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Tasa de Venta Máxima" runat="server" Text="Tas Vta Max"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTasaVentaMaxima" runat="server" Text='<%# Bind("TasaVentaMaxima") %>' CssClass="numerico"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>


                <%--15--%>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Pensión Original" runat="server" Text="Pensión Ori."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%--<INIGTI_6623>--%>
                        <%--<asp:Label ID="TabCotPensionCiaOriginal" runat="server" Text='<%# Bind("PensionCiaOrigen") %>' CssClass="numerico"></asp:Label>--%>
                        <asp:Label ID="TabCotPensionCiaOriginal" runat="server" Text='<%# (Eval("Modalidad.Nombre").ToString()=="RB" ? Eval("PensionAFPOrigen") : Eval("PensionCiaMOOrigen"))  %>' CssClass="numerico"></asp:Label>
                        <br />
                        <asp:Label ID="TabCotPensionCiaOriginalMo" runat="server" Text='<%# (Eval("Modalidad.Nombre").ToString()=="RB" ? Eval("PensionCiaMOOrigen") : "") %>' CssClass="numerico"></asp:Label>

                        <%--<FINGTI_6623>--%>

                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>

                <%--16--%>

                <asp:TemplateField Visible="false">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Pensión" runat="server" Text="Pensión"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotPensionCia" runat="server" Text='<%# Bind("PensionCia") %>' CssClass="numerico"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>


                <%--17--%>
                 <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Pensión Esperada" runat="server" Text="Pensión Esper."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <%--<INIGTI_6623>--%>
                        <%--<asp:Label ID="TabCotPensionCiaObjetivo" runat="server" Text='<%# Bind("PensionCiaObjetivo") %>' CssClass="numerico"></asp:Label>--%>
                        <asp:Label ID="TabCotPensionCiaObjetivo" runat="server" Text='<%# (Eval("Modalidad.Nombre").ToString()=="RB" ? Eval("PensionAFPObjetivo") : (Eval("Moneda.Id").ToString()=="002" || Eval("Moneda.Id").ToString()=="014" ? Eval("PensionCiaMOObjetivo"): Eval("PensionCiaObjetivo") )   ) %>' CssClass="numerico"></asp:Label>
                        <br />
                        <asp:Label ID="TabCotPensionCiaObjetivoMo" runat="server" Text='<%# (Eval("Modalidad.Nombre").ToString()=="RB" ? Eval("PensionCiaMOObjetivo") : "") %>' CssClass="numerico"></asp:Label>
                        <%--<FINGTI_6623>--%>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>
                                
                <%--18--%>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="TRA Mínima" runat="server" Text="TRA Min."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTasaRetornoAccionistaMinimo" runat="server" Text='<%# Bind("TasaRetornoAccionistaMinimo") %>' CssClass="numerico"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>

                <%--19--%>

                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="TRA" runat="server" Text="TRA"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTasaRetornoAccionista" runat="server" Text='<%# Bind("TasaRetornoAccionista") %>' CssClass="numerico"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>

                <%--20--%>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="TRA Esperada" runat="server" Text="TRA Esper."></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotTasaRetornoAccionistaObjetivo" runat="server" Text='<%# Bind("TasaRetornoAccionistaObjetivo") %>' CssClass="numerico"></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>


                <%--21--%>
                <asp:BoundField DataField="IndCotiza" HeaderText="Ind.">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <%--22--%>
                <asp:TemplateField HeaderText="Dif. TRA">
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Dif. TRA" runat="server" Text="Dif. TRA"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:TextBox ID="TabCotTRA" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid numerico" Style="text-align: right;" Width="45" data-v-min="-100.00" data-v-max="100.00" Text="0.00"></asp:TextBox>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>

                
                <%--23--%>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Puntos Básicos" runat="server" Text="PBS"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="TabCotpbs" runat="server" Text='<%# Bind("pbs") %>' ></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </asp:TemplateField>


                <%--<INIGTI_4081>--%>

                <%--<GTI.29372>--%>
                <%--24--%>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="Label1" ToolTip="Indicador de envío obligatorio" runat="server" Text="OBL"></asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkIndEnvioObl" runat="server" Checked='<%# Convert.ToBoolean(Eval("IndEnvioObligatorio")) ? true : false %>'/>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                </asp:TemplateField>
                <%--<GTI.29372>--%>

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
