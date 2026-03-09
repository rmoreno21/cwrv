<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaLotesResultado.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaLotesResultado" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabLotesResultado" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        UseAccessibleHeader="True">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:BoundField DataField="Numero" HeaderText="N° Lote">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="FechaCierre" HeaderText="Fecha de Cierre Comercial" DataFormatString="{0:dd/MM/yyyy}">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Generar Reporte">
                <ItemTemplate>
                    <%# "<a class=\"grilla_boton grilla_pdf\" data-lote=\"" + Eval("Numero") + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="130" />
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div align="center">
                <div class="grilla_info" align="left" style="width:195px">
                    No se ha encontrado ningún lote.
                </div>
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>