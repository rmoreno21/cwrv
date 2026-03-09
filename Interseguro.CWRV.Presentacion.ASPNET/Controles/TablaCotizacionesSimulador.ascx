<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesSimulador.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesSimulador" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabCotizacionesSimulador" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%#
                        (idSimulador!=90)?"<a class=\"grilla_boton grilla_graficar\" style=\"cursor:move\" data-cotizacion" + Filtro + "=\"" + (Eval("Correlativo")) + "\"></a>":
                        "<input type=\"checkbox\"  ID=\"ChkCotizacion\" data-cotizacion=\"" + Eval("Correlativo") + "\"/>"

                        
                    %>
                </ItemTemplate>
                <ItemStyle Width="30px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="N°">
                <ItemTemplate>
                    <%# Eval("Correlativo") %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="28px"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Moneda">
                <ItemTemplate>
                    <asp:Label ID="TabCotMoneda" runat="server" Text='<%# Bind("Moneda.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Modalidad">
                <ItemTemplate>
                    <%-- SRI.INI-20322 --%>
                    <%--<asp:Label ID="TabCotModalidad" runat="server" Text='<%# Bind("Modalidad.Nombre") %>'></asp:Label>--%>
                    <asp:Label ID="TabCotModalidad" runat="server" Text='<%# Bind("Modalidad.Id") %>'></asp:Label>
                    <%-- SRI.FIN-20322 --%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="PeriodoDiferido"
                   HeaderText="P. Dif." />
            <asp:BoundField DataField="PeriodoGarantizado"
                   HeaderText="PG" />
            <%--SRI.INI-20322--%>
            <asp:TemplateField HeaderText="Gratificación">
                <ItemTemplate>
                    <asp:Label ID="TabCotGratificacion" runat="server" Text='<%# (Convert.ToBoolean(Eval("Gratificacion")) ? "Si" : "No")  %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:BoundField DataField="PensionCia" 
                   HeaderText="Pensión" DataFormatString="{0:#,##0.00}" >
            <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>--%>
            <asp:TemplateField HeaderText="Pensión">
                <ItemTemplate>
                    <asp:Label ID="PensionCia" runat="server" Text='<%# ((Convert.ToString(Eval("Modalidad.Id")) == "I" || Convert.ToString(Eval("Modalidad.Id")) == "D" || Convert.ToString(Eval("Modalidad.Id")) == "I-RVE") ? Eval("PensionCia", "{0:#,##0.00}") : Eval("PensionAFP", "{0:#,##0.00}"))  %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Pensión2">
                <ItemTemplate>
                    <asp:Label ID="PensionCiaMO" runat="server" Text='<%# ((Convert.ToString(Eval("Modalidad.Id")) == "I-RB" || Convert.ToString(Eval("Modalidad.Id")) == "I-RC" || Convert.ToString(Eval("Modalidad.Id")) == "I-RM") ? Eval("PensionCiaMO", "{0:#,##0.00}") : "")  %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <%--SRI.FIN-20322--%>
            <asp:TemplateField HeaderText="Graf." Visible="False">
                <ItemTemplate>
                    <%# "<input type=\"radio\" name=\"graficar1" + Filtro + "\" value=\"" + (Eval("Correlativo")) + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle Width="55px" HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Graf." Visible="False">
                <ItemTemplate>
                    <%# "<input type=\"radio\" name=\"graficar2" + Filtro + "\" value=\"" + (Eval("Correlativo")) + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle Width="55px" HorizontalAlign="Center" />
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
