<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaTelefonos.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaTelefonos" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabTelefonos" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None" UseAccessibleHeader="True"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:BoundField DataField="Id"
                   HeaderText="ID" Visible="False" />
            <asp:TemplateField HeaderText="Tipo">
                <ItemTemplate>
                    <asp:Label ID="ItemTipo" runat="server" Text='<%# Bind("Tipo.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Numero"
                   HeaderText="Número" />
            <asp:BoundField DataField="FechaIngreso" 
                   HeaderText="Fecha de registro" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" Width="125px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Principal" SortExpression="Active">
                <ItemTemplate>
                    <%# (Boolean.Parse(Eval("Principal").ToString())) ? "Sí" : "No" %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_editar\" data-telefono=\"" + Eval("Id")) : " grilla_editar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                    <%# "<a class=\"grilla_boton" + (PermisoEliminar ? (" grilla_eliminar\" data-telefono=\"" + Eval("Id")) : " grilla_eliminar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle Width="64px" />
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de teléfono.
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>
    <asp:Panel ID="TabTelefonosBotonera" CssClass="formLinea" align="right" runat="server" Visible="False">
        <asp:HyperLink ID="TabTelefonosVerMas" CssClass="boton darkblue sharp" style="width:150px;height:22px" runat="server" ClientIDMode="Static">Ver más direcciones</asp:HyperLink>
    </asp:Panel>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
