<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaActividades.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaActividades" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabActividades" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:BoundField DataField="Correlativo"
                   HeaderText="Item" >
                <ItemStyle Width="40px" HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Actividades">
                <ItemTemplate>
                    <%# Eval("TipoEvento") + ": " + Eval("Resultado") %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="FechaEvento" 
                   HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" Width="100px" />
            </asp:BoundField>
            <asp:BoundField DataField="Username"
                   HeaderText="Usuario" />
            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<a class=\"grilla_boton" + (PermisoConsultar ? (" grilla_consultar\" data-actividad=\"" + Eval("Correlativo")) : " grilla_consultar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle Width="30px" />
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de actividades.
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>
    <asp:Panel ID="TabActividadesBotonera" CssClass="formLinea" align="right" runat="server" Visible="False">
        <asp:HyperLink ID="TabActividadesVerMas" CssClass="boton darkblue sharp" style="width:150px;height:22px" runat="server" ClientIDMode="Static">Ver más actividades</asp:HyperLink>
    </asp:Panel>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
