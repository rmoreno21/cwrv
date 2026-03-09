<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaContratos.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaContratos" %>
<form id="form1" runat="server" enableviewstate="False">
    <asp:GridView ID="TabContratos" runat="server" Width="100%" CellPadding="1" CellSpacing="1" GridLines="None"
    ClientIDMode="Static" AutoGenerateColumns="False">
    <AlternatingRowStyle CssClass="grilla_alt2" />
    <Columns>
        <asp:TemplateField>
            <ItemTemplate>
                <%# "<input id=\"idContrato\" type=\"radio\" name=\"idContrato\" value=\"" + Eval("IdContratoCotizacion") + "\" checked=\"checked\" />" %>
            </ItemTemplate>
            <ItemStyle Width="23px" />
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Nro. Contrato">
            <ItemTemplate>
                <asp:Label ID="NroContrato" runat="server" Text='<%# Bind("IdContratoCotizacion") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Contrato">
            <ItemTemplate>
                <asp:Label ID="GlsContrato" runat="server" Text='<%# Bind("GlsContratoCotizacion") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Fecha Inicio">
            <ItemTemplate>
                <asp:Label ID="FecInicio" runat="server" Text='<%# Convert.ToDateTime(Eval("FecInicio")).ToString("dd/MM/yyyy") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Fecha Fin">
            <ItemTemplate>
                <asp:Label ID="FecFin" runat="server" Text='<%# Convert.ToDateTime(Eval("FecFin")).ToString("dd/MM/yyyy") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="AFP">
            <ItemTemplate>
                <asp:Label ID="GlsAfp" runat="server" Text='<%# Bind("GlsAfp") %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EmptyDataTemplate>
        <div class="grilla_info">
            No se ha encontrado ningún registro de contrato.
        </div>
    </EmptyDataTemplate>
    <HeaderStyle CssClass="grilla_cabecera" />
    <RowStyle CssClass="grilla_alt1" />
</asp:GridView>
</form>