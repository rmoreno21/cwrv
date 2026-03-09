<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaRviBenefi2.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaRviBenefi2" %>
<div id="ContenidoDinamico">
    <asp:GridView
        ID="TabRviBenefi"
        runat="server"
        Width="100%"
        CellPadding="3" 
        CellSpacing="1"
        GridLines="None"
        UseAccessibleHeader="True"
        ViewStateMode="Disabled"
        ClientIDMode="Static"
        AutoGenerateColumns="False"
    >
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:BoundField DataField="Id"
                   HeaderText="Id" Visible="False" />
            <asp:TemplateField HeaderText="Parentesco">
                <ItemTemplate>
                    <asp:Label ID="ItemParentesco" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="84px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Apellidos y Nombres">
                <ItemTemplate>
                    <div style="overflow:hidden;text-overflow:ellipsis;">
                        <asp:HyperLink ID="NombreBeneficiario" runat="server" NavigateUrl='<%# string.Format("~/Cotizador/BeneficiarioCierre.aspx?s={0}&fc={1}&c={2}", Eval("numSolicitud"), Request.QueryString["fc"], Eval("numCorrelativo")) %>'>
                            <asp:Label ID="ItemNomnbre" runat="server" Text='<%# Eval("ApellidoPaterno") + " " + Eval("ApellidoMaterno") + ", " + Eval("Nombre") %>'></asp:Label>
                        </asp:HyperLink>
                    </div>
                </ItemTemplate>
                <ItemStyle Width="281px" />
            </asp:TemplateField>
            <asp:BoundField DataField="Sexo"
                   HeaderText="Sexo">
                <ItemStyle HorizontalAlign="Center" Width="39px" />
            </asp:BoundField>
            <asp:BoundField DataField="FechaNacimiento" 
                   HeaderText="Fec.Nacimiento" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" Width="104px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Ind.Invalidez" SortExpression="Active">
                <ItemTemplate>
                    <%# (Boolean.Parse(Eval("Invalido").ToString())) ? "Sí" : "No"%>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="84px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Tipo Invalidez">
                <ItemTemplate>
                    <asp:Label ID="ItemTipoInvalidez" runat="server" Text='<%# Bind("TipoInvalidez.Nombre") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="104px" />
            </asp:TemplateField>
            <asp:BoundField DataField="FechaInvalidez" 
                   HeaderText="Fec.Invalidez" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" Width="94px" />
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
