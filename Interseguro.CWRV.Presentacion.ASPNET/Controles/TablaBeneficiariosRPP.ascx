<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaBeneficiariosRPP.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaBeneficiariosRPP" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView
        ID="TablaRPPBeneficiarios" runat="server"
        ViewStateMode="Disabled" ClientIDMode="Static"
        AutoGenerateColumns="False" CssClass="highlight">    
        <Columns>
            <asp:TemplateField HeaderText="Parentesco">
                <ItemTemplate>
                    <asp:Label ID="Parentesco" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Apellidos y Nombres">
                <ItemTemplate>
                    <asp:Label ID="Nombres" runat="server" Text='<%# Eval("Nombre") + " " + Eval("ApellidoPaterno") + " " + Eval("ApellidoMaterno") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Sexo">
                <ItemTemplate>
                    <asp:Label ID="Sexo" runat="server" Text='<%# Bind("Sexo") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:BoundField DataField="FechaNacimiento"
                HeaderText="Fec.Nacimiento" DataFormatString="{0:dd/MM/yyyy}">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            <p style="font-size:1rem"><i class="material-icons amber-text text-darken-2" style="margin-right:10px">warning</i> No se han encontrado beneficiarios.</p>
        </EmptyDataTemplate>
    </asp:GridView>
</div>
</form>