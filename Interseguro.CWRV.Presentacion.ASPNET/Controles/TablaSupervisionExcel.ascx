<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSupervisionExcel.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSupervisionExcel" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabSeguimiento" runat="server" Width="100%" CellPadding="4" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        ForeColor="#333333">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:BoundField DataField="Jefe" 
                   HeaderText="Jefe" />
            <asp:BoundField DataField="Supervisor"
                   HeaderText="Supervisor" />
            <asp:BoundField DataField="Agente" 
                   HeaderText="Agente" />
            <asp:BoundField DataField="Usuario" 
                   HeaderText="Usuario" />
            <asp:BoundField DataField="FechaEvento" 
                   HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" Width="75" />
            </asp:BoundField>
            <asp:BoundField DataField="Evento" 
                   HeaderText="Evento" />
            <asp:BoundField DataField="Detalle" 
                   HeaderText="Detalle" />
        </Columns>
        <EditRowStyle BackColor="#2461BF" />
        <FooterStyle BackColor="#507CD1" ForeColor="White" Font-Bold="True" />
        <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#2461BF" ForeColor="White" 
            HorizontalAlign="Center" />
        <RowStyle BackColor="#EFF3FB" />
        <SelectedRowStyle BackColor="#D1DDF1" ForeColor="#333333" Font-Bold="True" />
        <SortedAscendingCellStyle BackColor="#F5F7FB" />
        <SortedAscendingHeaderStyle BackColor="#6D95E1" />
        <SortedDescendingCellStyle BackColor="#E9EBEF" />
        <SortedDescendingHeaderStyle BackColor="#4870BE" />
    </asp:GridView>
</div>
</form>
