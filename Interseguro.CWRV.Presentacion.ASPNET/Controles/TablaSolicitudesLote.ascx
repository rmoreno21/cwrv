<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesLote.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesLote" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">

    <div class="formLinea">
        <label id="LabFechaPresentacion" for="FechaPresentacion" class="formLabel">Fecha de Plazo AFP:</label>
        <asp:Label ID="FechaPresentacion" CssClass="formText" runat="server" ClientIDMode="Static"></asp:Label>
    </div>

    <asp:GridView ID="TabSolicitudesLote" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:TemplateField >
                <HeaderTemplate>
                    <%# "<input id=\"SeleccionarTodos\" type=\"checkbox\" name=\"SeleccionarTodos\" checked=\"checked\" />" %>
                </HeaderTemplate>
                <ItemTemplate>
                    <%# "<input type=\"checkbox\" name=\"solicitud\" class=\"chklote\" value=\"" + Eval("Id") + "\" " + ((bool)Eval("Habilitado") ? "checked=\"checked\"" : "disabled=\"disabled\"") + " />" %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="50" />
            </asp:TemplateField>
            <asp:BoundField DataField="Id" HeaderText="Solicitud">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:BoundField DataField="Observacion" HeaderText="Observación">
                <ItemStyle HorizontalAlign="Left" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Categoría">
                <ItemTemplate>
                    <asp:Label ID="ItemCategoria" runat="server" Text='<%# Bind("Categoria.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="AFP">
                <ItemTemplate>
                    <asp:Label ID="ItemAFP" runat="server" Text='<%# Bind("AFP.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="FechaDevengue" HeaderText="Devengue" DataFormatString="{0:dd/MM/yyyy}">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="CIC">
                <ItemTemplate>
                    <asp:Label ID="ItemCIC" Text='<%# Eval("Afiliado.SaldoCIC","{0:0,0.00}")%>' runat="server"></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Right" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="CUSPP">
                <ItemTemplate>
                    <asp:Label ID="ItemCUSPP" runat="server" Text='<%# Bind("Afiliado.CUSPP") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Afiliado">
                <ItemTemplate>
                    <asp:Label ID="ItemAfiliado" runat="server" Text='<%# Bind("Afiliado.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Agente">
                <ItemTemplate>
                    <asp:Label ID="ItemAgente" runat="server" Text='<%# Bind("Agente.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Cobertura IS">
                <ItemTemplate>
                    <asp:CheckBox ID="chkCoberturaIS" runat="server" Checked='<%# Convert.ToBoolean(Eval("IndCoberturaIS")) ? true : false %>' Enabled="false" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
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

    <asp:HiddenField ID="FechaCierre" runat="server" ClientIDMode="Static"></asp:HiddenField>

    <div class="formLinea" align="center">
        <asp:HyperLink ID="CotizarLote" style="width:180px;height:22px;margin-left:-20px;" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Cotizar <span id="NroSolicitudes">100</span> Solicitudes</asp:HyperLink>
    </div>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
