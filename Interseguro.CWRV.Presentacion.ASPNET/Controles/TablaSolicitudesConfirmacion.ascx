<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesConfirmacion.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesConfirmacion" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabSolicitudesConfirmacion" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        UseAccessibleHeader="True">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:TemplateField >
                <HeaderTemplate>
                    <%# "<input id=\"SeleccionarTodos\" type=\"checkbox\" name=\"SeleccionarTodos\" />" %>
                </HeaderTemplate>
                <ItemTemplate>
                    <%# "<input type=\"checkbox\" name=\"solicitud\" class=\"chklote\" value=\"" + Eval("Id") + "\" />" %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="50" />
            </asp:TemplateField>
            <asp:BoundField DataField="Id" HeaderText="Solicitud">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="NumeroPoliza" HeaderText="Póliza">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Apellido Paterno">
                <ItemTemplate>
                    <asp:Label ID="ApellidoPaterno" runat="server" Text='<%# Bind("Afiliado.ApellidoPaterno") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Apellido Materno">
                <ItemTemplate>
                    <asp:Label ID="ApellidoMaterno" runat="server" Text='<%# Bind("Afiliado.ApellidoMaterno") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Nombres">
                <ItemTemplate>
                    <asp:Label ID="Nombres" runat="server" Text='<%# Bind("Afiliado.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="FechaCargaConfirmacion" HeaderText="Fecha Carga Confirmación" DataFormatString="{0:dd/MM/yyyy}">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Confirmada">
                <ItemTemplate>
                    <%# (Convert.ToBoolean(Eval("Enviada"))) ? "Sí" : "No" %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="50" />
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div align="center">
                <div class="grilla_info" align="left" style="width:225px">
                    No se ha encontrado ninguna solicitud.
                </div>
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>

    <div class="formLinea" align="center">
        <asp:HyperLink ID="GuardarConfirmacion" style="width:190px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Confirmar <span id="NroSolicitudes">100</span> Solicitudes</asp:HyperLink>
    </div>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
