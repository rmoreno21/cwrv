<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCapitalRequerido.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCapitalRequerido" %>

<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    

    <asp:GridView ID="TabCapitalRequerido"  runat="server" Width="100%" CellPadding="1" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" OnRowDataBound="TabCapitalRequerido_RowDataBound">
    <AlternatingRowStyle CssClass="grilla_alt2" />
    <Columns>

        <asp:TemplateField HeaderText="Moneda">
            <ItemTemplate>
                <asp:Label ID="ItemMoneda" runat="server" Text='<%# Bind("Moneda.Nombre") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Center" />
            <ItemStyle Width="104px" />
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Tipo Renta">
            <ItemTemplate>
                <asp:Label ID="ItemTipoRenta" runat="server" Text='<%# Bind("TipoRenta.Nombre") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Width="120px" />
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>

        <%--<asp:BoundField DataField="temporalidad" HeaderText="Temporalidad">
                <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>--%>

        <asp:TemplateField HeaderText="Temporalidad">
            <ItemTemplate>
                <asp:Label ID="ItemTipoRenta" runat="server" Text='<%# Bind("Temporalidad.Nombre") %>'></asp:Label>
            </ItemTemplate>
            <ItemStyle Width="120px" />
            <ItemStyle HorizontalAlign="Center" />
        </asp:TemplateField>


        <asp:BoundField DataField="periodo_garantizado" HeaderText="Periodo Garantizado">
                <ItemStyle HorizontalAlign="Center" />
        </asp:BoundField>


        <asp:TemplateField HeaderText="Tasa de Venta">
            <ItemTemplate>
                <asp:Label ID="tasaSBS" runat="server" Text='<%#   System.Math.Round( Convert.ToDouble(Eval("tasaSBS")),2,MidpointRounding.AwayFromZero).ToString("#,##0.00") %>' ></asp:Label>
                &nbsp;
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Right" />
            <ItemStyle Width="50px" />
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Pensión Requerida">
            <ItemTemplate>
                <asp:Label ID="pension_requerida" runat="server" Text='<%#   System.Math.Round( Convert.ToDouble(Eval("pension_requerida")),2,MidpointRounding.AwayFromZero).ToString("#,##0.00") %>' ></asp:Label>
                &nbsp;
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Right" />
            <ItemStyle Width="130px" />
        </asp:TemplateField>

       
        <asp:TemplateField HeaderText="Capital Requerido">
            <ItemTemplate>
                <asp:Label ID="simbolo_moneda_equi" runat="server" Text='<%#   Eval("simbolo_moneda_equi" ) %>' ></asp:Label>
                &nbsp;&nbsp;
                <asp:Label ID="capital_requerido" runat="server" Text='<%#  System.Math.Round( Convert.ToDouble(Eval("capital_requerido")),2,MidpointRounding.AwayFromZero).ToString("#,##0.00")  %>' ></asp:Label>
                &nbsp;
            </ItemTemplate>
            <ItemStyle HorizontalAlign="Right" />
            <ItemStyle Width="150px" />
        </asp:TemplateField>


    </Columns>
    <EmptyDataTemplate>
        <div class="grilla_info">
            No se ha encontrado ningún registro de cálculo capital.
        </div>
    </EmptyDataTemplate>
    <HeaderStyle CssClass="grilla_cabecera" />
    <RowStyle CssClass="grilla_alt1" />
</asp:GridView>

</div>
</form>