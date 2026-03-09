<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaGrupoFamiliar.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaGrupoFamiliar" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">
        <asp:HiddenField ID="HListaNegra" runat="server" ClientIDMode="Static" />
        <asp:GridView ID="TabGrupoFamiliar" runat="server" Width="100%" CellPadding="3"
            CellSpacing="1" GridLines="None" UseAccessibleHeader="True"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabGrupoFamiliar_RowDataBound">
            <AlternatingRowStyle CssClass="grilla_alt2" />
            <Columns>
                <asp:BoundField DataField="Id"
                    HeaderText="ID" Visible="False" />
                <asp:TemplateField HeaderText="Ape.Paterno">
                    <ItemTemplate>
                        <div style="width: 135px; overflow: hidden; text-overflow: ellipsis;">
                            <asp:Label ID="ItemApellidoPaterno" runat="server" Text='<%# Bind("ApellidoPaterno") %>'></asp:Label>
                        </div>
                    </ItemTemplate>
                    <ItemStyle Width="150px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Ape.Materno">
                    <ItemTemplate>
                        <div style="width: 135px; overflow: hidden; text-overflow: ellipsis;">
                            <asp:Label ID="ItemApellidoMaterno" runat="server" Text='<%# Bind("ApellidoMaterno") %>'></asp:Label>
                        </div>
                    </ItemTemplate>
                    <ItemStyle Width="150px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Nombres">
                    <ItemTemplate>
                        <div style="width: 162px; overflow: hidden; text-overflow: ellipsis;">
                            <asp:Label ID="ItemNombre" runat="server" Text='<%# Bind("Nombre") %>'></asp:Label>
                        </div>
                    </ItemTemplate>
                    <ItemStyle Width="162px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Parentesco">
                    <ItemTemplate>
                        <asp:Label ID="ItemParentesco" runat="server" Text='<%# Bind("Parentesco.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="84px" />
                </asp:TemplateField>
                <asp:BoundField DataField="Sexo"
                    HeaderText="Sexo">
                    <ItemStyle HorizontalAlign="Center" Width="39px" />
                </asp:BoundField>
                <asp:BoundField DataField="FechaNacimiento"
                    HeaderText="Fec.Nacimiento" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" Width="104px" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Ind.Invalidez" SortExpression="Active">
                    <ItemTemplate>
                        <%# (Boolean.Parse(Eval("Invalido").ToString())) ? "Sí" : "No"%>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="84px" />
                </asp:TemplateField>
                <asp:TemplateField>
                    <ItemTemplate>
                        <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_editar\" data-grupofamiliar=\"" + Eval("Id")) : " grilla_editar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                        <%# Eval("Parentesco.Id").ToString() != "80" ? PermisoEliminar ? "<a class=\"grilla_boton" + (PermisoEliminar ? (" grilla_eliminar\" data-grupofamiliar=\"" + Eval("Id")) : " grilla_eliminar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>":"" : ""%>
                    </ItemTemplate>
                    <ItemStyle Width="30px" HorizontalAlign="Center" />
                </asp:TemplateField>
            </Columns>
            <EmptyDataTemplate>
                <div class="grilla_info">
                    No se ha encontrado ningún registro de grupo familiar.
                </div>
            </EmptyDataTemplate>
            <HeaderStyle CssClass="grilla_cabecera" />
            <RowStyle CssClass="grilla_alt1" />
        </asp:GridView>

        <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
            <div class="grilla_error" align="left" style="width: 375px">
                Usted no cuenta con privilegios para visualizar esta información.
            </div>
        </asp:Panel>

    </div>
</form>
