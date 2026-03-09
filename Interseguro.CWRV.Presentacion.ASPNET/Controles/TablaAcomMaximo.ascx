<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaAcomMaximo.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaAcomMaximo" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">

    <%--<div style="display:block;overflow-y:scroll;height:125px;">--%>
    <asp:GridView ID="TabSeleccionAcom" runat="server" Width="100%" CellPadding="1" 
        CellSpacing="1" GridLines="None" UseAccessibleHeader="True"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" >
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<input type=\"radio\" name=\"id\" id=\"rad_" + (Container.DataItemIndex + 1) + "\" value=\"" + Eval("CodEscenario") + "\" />"%>
                    <%--<%# "<input type=\"radio\" name=\"id\" id=\"rad_" + (Container.DataItemIndex + 1) + "\" value=\"" + Eval("CodEscenario") + "," + Eval("GlsEscenario") + "\" />"%>--%>
                </ItemTemplate>
                <ItemStyle Width="5%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ACOM">
                <ItemTemplate>
                    <asp:Label ID="ItemTipoOperacion" runat="server" Text='<%# Bind("CodEscenario") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="40%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="ESCENARIO">
                <ItemTemplate>
                    <asp:Label ID="ItemTipoOperacion" runat="server" Text='<%# Bind("GlsEscenario") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="55%" />
            </asp:TemplateField>

        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de ventas.
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
    <%--</div>--%>
</div>
</form>