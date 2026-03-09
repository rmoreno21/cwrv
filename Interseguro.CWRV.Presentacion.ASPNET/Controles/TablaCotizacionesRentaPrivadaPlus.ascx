<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesRentaPrivadaPlus.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesRentaPrivadaPlus" %>
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
                <ItemStyle HorizontalAlign="Center" Width="40px"/>
            </asp:TemplateField>

            <%--<asp:TemplateField HeaderText="Devol. Prima">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotDevolucion" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="70">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="90px" />
            </asp:TemplateField>--%>

            <asp:TemplateField HeaderText="Moneda">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotMoneda" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="65">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="85px" />
            </asp:TemplateField>

            <%--<INIGTI_753>--%>
            <asp:TemplateField HeaderText="Aj. Mon.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotAjusteMoneda" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolAjusteMoneda" Width="45">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="55px" />
            </asp:TemplateField>
            <%--<FINGTI_753>--%>

            <asp:TemplateField HeaderText="Sep.">
                <ItemTemplate>
                    <asp:CheckBox  ID="ChkSepelio" runat="server" ClientIDMode="Static" />
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="55px" />
            </asp:TemplateField>

            <%--<asp:TemplateField HeaderText="Cob. Adic.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotCobroAdicional" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="70">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="90px" />
            </asp:TemplateField>--%>

            <asp:TemplateField HeaderText="1° Tramo Años">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotPagoEscalonada" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPagoEscalonada" Width="50">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="65px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="1° Tramo %">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotPjePagoEscalonada" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPjePagoEscalonada" Width="60">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="75px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="% Dev.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotPjeDevolucion" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid ModSolPjeDevolucion" Width="57">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="75px" />
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Per. Gar.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotPeriodoGarantizado" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="45">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="65px"  />
            </asp:TemplateField>

            <%--<INIGTI_753>--%>
            <asp:TemplateField HeaderText="Pje. Conyu.">
                <ItemTemplate>
                    <asp:DropDownList ID="TabCotPjeConyuge" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid" Width="50">
                    </asp:DropDownList>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="65px" />
            </asp:TemplateField>
            <%--<FINGTI_753>--%>


            <%-- Se debe mostrar la pensión en su Moneda Original en la grilla --%>
            <asp:BoundField DataField="PensionCiaMO"
                   HeaderText="Renta" DataFormatString="{0:#,##0.00}" >
                <ItemStyle HorizontalAlign="Right" Width="120px" />
            </asp:BoundField>

            <asp:BoundField DataField="TasaVentaSbs"
                   HeaderText="Tasa Vta" DataFormatString="{0:#,##0.00}" >
                <ItemStyle HorizontalAlign="Right"  Width="70px" />
            </asp:BoundField>

            <asp:BoundField DataField="Pension2doTramoSinAjuste"
                   HeaderText="Renta 2° Tramo" DataFormatString="{0:#,##0.00}" >
                <ItemStyle HorizontalAlign="Right"  Width="120px" />
            </asp:BoundField>

            <asp:BoundField DataField="Pension2doTramo"
                   HeaderText="Renta 2° Tramo Aj." DataFormatString="{0:#,##0.00}" >
                <ItemStyle HorizontalAlign="Right"  Width="120px" />
            </asp:BoundField>


            <asp:BoundField DataField="TasaVenta"
                   HeaderText="Tas. Vta IS" DataFormatString="{0:#,##0.00}" >
                <ItemStyle HorizontalAlign="Right"  Width="60px" />
            </asp:BoundField>

            <asp:BoundField DataField="TasaRetornoAccionista"
                   HeaderText="TRA" DataFormatString="{0:#,##0.00}" >
                <ItemStyle HorizontalAlign="Right"  Width="60px" />
            </asp:BoundField>


            <asp:BoundField DataField="IndCotiza"
                 HeaderText="Ind." >
                <ItemStyle HorizontalAlign="Right"  Width="60px" />
            </asp:BoundField>

            <asp:TemplateField HeaderText="Dif. TRA">
                <ItemTemplate>
                    <asp:TextBox ID="TabCotTRA" runat="server" ClientIDMode="Static" CssClass="formTextboxGrid numerico ModSolTRA" Width="35" data-v-min="-100.00" data-v-max="100.00" Text="0.00" style="text-align:right;" ></asp:TextBox>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="45px" />
            </asp:TemplateField>

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
    
    <asp:Panel ID="TabCotizacionesLeyenda_RP" runat="server" Visible="true" style="margin-top:10px">
        <fieldset>
            <legend>Leyenda</legend>
            <div style="font-family: Calibri; font-size: 13px; color: #0060A9">
                <span style="display: inline-block; width:25px;">**</span>: Cotización no alcanza el mínimo requerido, por lo cual no se simula.
            </div>
        </fieldset>
    </asp:Panel>

    <%--<div style="padding: 10px 0">
        

            
    </div>--%>
    
</div>
</form>