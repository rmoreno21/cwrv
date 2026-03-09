<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesRentaPrivada.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesRentaPrivada" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabCotizaciones_RP" runat="server" Width="100%" CellPadding="0" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        onrowdatabound="TabCotizaciones_RB_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>

            <asp:TemplateField HeaderText="N°">
                <ItemTemplate>
                    <%# Container.DataItemIndex + 1 %>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="38px"/>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Moneda">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotMoneda" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="156">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="100px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Per. Gar.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotPeriodoGarantizado" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="80">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="79px"  />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Dif. TRA">
                <ItemTemplate>
                    <asp:TextBox ID="TabCotTRA" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid numerico" Width="50" data-v-min="-100.00" data-v-max="100.00" Text="0.00" style="text-align:right;" ></asp:TextBox>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="70px" />
            </asp:TemplateField>

            <%-- Se debe mostrar la pensión en su Moneda Original en la grilla --%>
            <asp:BoundField DataField="PensionCiaMO"
                   HeaderText="Pensión CIA" DataFormatString="{0:#,##0.00}" >
                <ItemStyle HorizontalAlign="Right" Width="120px" />
            </asp:BoundField>

             <asp:BoundField DataField="TasaVentaSbs"
                   HeaderText="Tasa Venta" DataFormatString="{0:#,##0.00}" >
                <ItemStyle HorizontalAlign="Right"  Width="120px" />
            </asp:BoundField>
            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<a class=\"grilla_boton" + (PermisoEliminar ? (" grilla_eliminar\" data-cotizacion=\"" + (Container.DataItemIndex + 1)) : " grilla_eliminar_deshabilitado") + "\"></a>"%>
                </ItemTemplate>
                <ItemStyle Width="30px" />
            </asp:TemplateField>
            
        </Columns>
        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de cotización.
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>
    <asp:Panel ID="TabCotizacionesBotonera_RP" CssClass="formLinea" align="right" runat="server">
        <asp:HyperLink ID="TabCotizacionesAgregar_RP" CssClass="boton darkblue sharp" style="width:80px;height:22px" runat="server" ClientIDMode="Static">Agregar</asp:HyperLink>
    </asp:Panel>
</div>
</form>
