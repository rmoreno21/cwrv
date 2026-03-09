<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaDirecciones.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaDirecciones" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabDirecciones" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None" UseAccessibleHeader="True"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:BoundField DataField="Id"
                   HeaderText="ID" Visible="False" />
            <asp:TemplateField HeaderText="Dirección del Afiliado">
                <ItemTemplate>
                    <div style="width:240px;overflow:hidden;text-overflow:ellipsis;">
                        <%--<asp:Label ID="ItemDireccion" runat="server" Text='<%# Bind("Glosa") %>'></asp:Label>--%>
                        <%#  Eval("TipoVia.Glosa") + " " + Eval("Glosa") + " " + Eval("EspacioUrbano") %>
                    </div>
                </ItemTemplate>
                <ItemStyle Width="240px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Distrito">
                <ItemTemplate>
                    <asp:Label ID="ItemComuna" runat="server" Text='<%# Bind("Comuna.Nombre") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Provincia">
                <ItemTemplate>
                    <asp:Label ID="ItemCiudad" runat="server" Text='<%# Bind("Ciudad.Nombre") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Departamento">
                <ItemTemplate>
                    <asp:Label ID="ItemDepartamento" runat="server" Text='<%# Bind("Departamento.Nombre") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px" />
            </asp:TemplateField>
            <asp:BoundField DataField="FechaIngreso" 
                   HeaderText="Fecha de registro" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Principal" SortExpression="Active">
                <ItemTemplate>
                    <%# (Boolean.Parse(Eval("Principal").ToString())) ? "Sí" : "No" %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_editar\" data-direccion=\"" + Eval("Id")) : " grilla_editar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                    <%# "<a class=\"grilla_boton" + (PermisoEliminar ? (" grilla_eliminar\" data-direccion=\"" + Eval("Id")) : " grilla_eliminar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle Width="64px" />
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de dirección.
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>
    <asp:Panel ID="TabDireccionesBotonera" CssClass="formLinea" align="right" runat="server" Visible="False">
        <asp:HyperLink ID="TabDireccionesVerMas" CssClass="boton darkblue sharp" style="width:150px;height:22px" runat="server" ClientIDMode="Static">Ver más direcciones</asp:HyperLink>
    </asp:Panel>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
