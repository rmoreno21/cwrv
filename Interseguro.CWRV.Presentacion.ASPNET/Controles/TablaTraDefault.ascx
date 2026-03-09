<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaTraDefault.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaTraDefault" %>


<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabTraDefault" runat="server" Width="100%" CellPadding="1" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        onrowdatabound="TabTraDefault_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>

            <asp:BoundField DataField="Id" HeaderText="Parámetro">
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>

            <asp:TemplateField HeaderText="Moneda">
                <ItemTemplate>
                    <asp:Label ID="TabCotMoneda" runat="server" Text='<%# Bind("Moneda.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Fecha de Inicio">
                <ItemTemplate>
                    <asp:Label ID="CotFecIniRangoLabel" runat="server" Text='<%# Convert.ToDateTime(Eval("FecIniRango")).ToString("dd/MM/yyyy") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Fecha de Término">
                <ItemTemplate>
                    <asp:Label ID="CotFecFinRangoLabel" runat="server" Text='<%# Convert.ToDateTime(Eval("FecFinRango")).ToString("dd/MM/yyyy") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="ValParametro" HeaderText="TRA Default" DataFormatString="{0:0.00}">
                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>

            <asp:BoundField DataField="ValAjuTra" HeaderText="Ajuste TRA" DataFormatString="{0:0.00}">
                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>

            <asp:BoundField DataField="ValDiferencia" HeaderText="TRA Default + Ajuste TRA" DataFormatString="{0:0.00}">
                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>

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
