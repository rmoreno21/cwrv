<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizaciones.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizaciones" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabCotizaciones" runat="server" Width="100%" CellPadding="0" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        onrowdatabound="TabCotizaciones_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<input type=\"radio\" name=\"id\" value=\"" + (Container.DataItemIndex + 1) + "\" />" %>
                </ItemTemplate>
                <ItemStyle Width="23px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="N°">
                <ItemTemplate>
                    <%# Container.DataItemIndex + 1 %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="28px"/>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Moneda">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotMoneda" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="106">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Producto">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotProducto" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Modalidad">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotModalidad" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="P. Dif.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotPeriodoDiferido" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPeriodoDiferido">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Pje. Rent.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotPorcentajeRentas" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPorcentajeRentas" Enabled="False">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Per. Gar.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotPeriodoGarantizado" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
<%--            <asp:TemplateField HeaderText="D. Crec.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotDerechoCrecer" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid">
                        <asp:ListItem Value="S">Sí</asp:ListItem>
                        <asp:ListItem Value="N">No</asp:ListItem>
                    </asp:DropDownList>
                </ItemTemplate>
            </asp:TemplateField>--%>
            <asp:TemplateField HeaderText="Gratif.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotGratificacion" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolGratificacion"><%--<SOLINIGTI_754>--%>
                        <asp:ListItem Value="S">Sí</asp:ListItem>
                        <asp:ListItem Value="N">No</asp:ListItem>
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Capital" Visible="false">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotCapital" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolCapital" Enabled="False">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Dif. TRA">
                <ItemTemplate>
                    <asp:TextBox ID="TabCotTRA" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid numerico" Width="50" data-v-min="-100.00" data-v-max="100.00" Text="0.00"></asp:TextBox>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" />
            </asp:TemplateField>

            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<a class=\"grilla_boton" + (PermisoEliminar ? (" grilla_eliminar\" data-cotizacion=\"" + (Container.DataItemIndex + 1)) : " grilla_eliminar_deshabilitado") + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle Width="30px" />
            </asp:TemplateField>
            
           <%-- <asp:BoundField DataField="Numero"
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
                    <%# "<a class=\"grilla_boton grilla_editar\" data-telefono=\"" + Eval("ID") + "\"></a>"%>
                    <%# "<a class=\"grilla_boton grilla_eliminar\" data-telefono=\"" + Eval("ID") + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle Width="60px" />
            </asp:TemplateField>--%>
        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de cotización.
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>
    <asp:Panel ID="TabCotizacionesBotonera" CssClass="formLinea" align="right" runat="server">
        <asp:HyperLink ID="TabCotizacionesAgregar" CssClass="boton darkblue sharp" style="width:80px;height:22px" runat="server" ClientIDMode="Static">Agregar</asp:HyperLink>
    </asp:Panel>
</div>
</form>
