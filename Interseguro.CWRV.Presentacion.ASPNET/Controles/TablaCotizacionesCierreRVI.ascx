<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesCierreRVI.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesCierreRVI" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabCotizacionesCierreRVI" runat="server" Width="100%"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<label><input id=\"" + Eval("Correlativo") + "\" name=\"cot\" type=\"radio\" class=\"with-gap\" value=\"" + Eval("Correlativo") + "\" " + (Eval("EstadoCotizacion").ToString() == "04" ? "checked" : "") + " " + (SoloLectura ? "disabled" : "") + " /><span></span></label>" %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Correlativo">
                <ItemTemplate>
                    <%# Eval("Correlativo") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Fec. Cotización">
                <ItemTemplate>
                    <div class="center">
                        <%# Convert.ToDateTime(Eval("FechaCotizacion")).ToString("dd/MM/yyyy") %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Mon.">
                <ItemTemplate>
                    <%# Eval("Moneda.Simbolo") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Mod.">
                <ItemTemplate>
                    <%# Eval("Modalidad.Indicador") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="P. Dif.">
                <ItemTemplate>
                    <div class="right">
                        <%# Eval("PeriodoDiferido") %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="P. Gar.">
                <ItemTemplate>
                    <div class="right">
                        <%# Eval("PeriodoGarantizado") %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Pensión AFP">
                <ItemTemplate>
                    <div class="right">
                        <%# string.Format("{0:#,##0.00}", Eval("PensionAFP")) %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Pensión IS">
                <HeaderStyle CssClass="right" />
                <ItemTemplate>
                    <div class="right">
                        <%# string.Format("{0:#,##0.00}", Eval("PensionCiaMO")) %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <bloquote>
                <i class="material-icons">info</i> No se ha encontrado ninguna cotización para esta solicitud.
            </bloquote>
        </EmptyDataTemplate>
    </asp:GridView>
</div>
</form>
