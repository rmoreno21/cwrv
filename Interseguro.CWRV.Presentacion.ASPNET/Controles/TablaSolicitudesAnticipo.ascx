<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesAnticipo.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesAnticipo" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <br />
    <asp:GridView ID="TabSolicitudesAnticipo" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" OnRowDataBound="TabSolicitudesAnticipo_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:TemplateField HeaderText="N° Agente" ItemStyle-Width="80">
                <ItemTemplate>
                    <asp:Label ID="ItemNroAgente" runat="server" Text='<%# Bind("Agente.Id") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Nombre Agente">
                <ItemTemplate>
                    <asp:Label ID="ItemNombreAgente" runat="server" Text='<%# Bind("Agente.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="N° Solicitud" ItemStyle-Width="100">
                <ItemTemplate>
                    <asp:Label ID="ItemNroSolicitud" runat="server" Text='<%# Bind("Solicitud.Id") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Monto" ItemStyle-Width="100" ItemStyle-HorizontalAlign="Right">
                <ItemTemplate>
                    <asp:Label ID="ItemMonto" runat="server" Text='<%# Eval("Monto","S/ {0:0,0.00}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="FechaAceptacion"
                   HeaderText="Fecha Aceptación" DataFormatString="{0:dd/MM/yyyy hh:mm:ss tt}" >
                <ItemStyle HorizontalAlign="Center" Width="150" />
            </asp:BoundField>
        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de solicitudes de anticipo.
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>

    <asp:Panel ID="TabSolicitudesAnticipoContenedorPaginador" runat="server" style="margin-top:5px;height:30px" Visible="False">
        <asp:Literal ID="TabSolicitudesAnticipoPaginador" runat="server"></asp:Literal>
        <span id="TabSolicitudesAnticipoPaginadorCargando" class="paginador_cargando"></span>
        <asp:Label ID="TabSolicitudesAnticipoPaginadorIndice" style="float:right;font-size:13px;font-family:Tahoma;padding-top:5px" runat="server"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
