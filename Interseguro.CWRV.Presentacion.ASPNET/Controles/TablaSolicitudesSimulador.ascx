<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesSimulador.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesSimulador" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabSolicitudes" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        onrowdatabound="TabSolicitudes_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="20px" />
            </asp:TemplateField>
            <asp:BoundField DataField="Id"
                   HeaderText="Solicitud" />
            <asp:BoundField DataField="FechaCotizacion"
                   HeaderText="Fec. Cotización" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="FechaSolicitud"
                   HeaderText="Fec. Solicitud" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Tipo Cotización">
                <ItemTemplate>
                    <asp:Label ID="ItemTipoCotizacion" runat="server" Text='<%# Bind("TipoCotizacion.Id") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="FechaRecepcion"
                   HeaderText="Fec. Recepción" DataFormatString="{0:dd/MM/yyyy}">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="FechaPlazoAFP"
                   HeaderText="Plazo AFP" DataFormatString="{0:dd/MM/yyyy}">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="FechaCierre"
                   HeaderText="Fec. Cierre" DataFormatString="{0:dd/MM/yyyy}">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>

            <%--<INIGTI_2145>--%>
            <asp:TemplateField HeaderText="Prima Única">
                <ItemTemplate>
                    <asp:Label ID="PrimaUnica" runat="server" Text='<%# Eval("SaldoCIC", "{0:#,##0.00}")  %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <%--<FINGTI_2145>--%>

        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de solicitud.
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
