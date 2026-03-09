<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaBeneficiariosPlan3IFP.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaBeneficiariosPlan3IFP" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabBeneficiarios" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None" UseAccessibleHeader="True"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        onrowdatabound="TabBeneficiarios_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:BoundField DataField="Id"
                   HeaderText="Id" Visible="False" />
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="20px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Parentesco">
                <ItemTemplate>
                    <asp:Label ID="ItemParentesco" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="84px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Apellidos y Nombres">
                <ItemTemplate>
                    <div style="width:254px;overflow:hidden;text-overflow:ellipsis;">
                        <asp:Label ID="ItemNomnbre" runat="server" Text='<%# Eval("ApellidoPaterno") + " " + Eval("ApellidoMaterno") + ", " + Eval("Nombre") %>'></asp:Label>
                    </div>
                </ItemTemplate>
                <ItemStyle Width="254px" />
            </asp:TemplateField>
            <asp:BoundField DataField="Sexo"
                   HeaderText="Sexo">
                <ItemStyle HorizontalAlign="Center" Width="39px" />
            </asp:BoundField>
            <asp:BoundField DataField="FechaNacimiento" 
                   HeaderText="Fec.Nacimiento" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" Width="104px" />
            </asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de beneficiarios.
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>
</div>
</form>