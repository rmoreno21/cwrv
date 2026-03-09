<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesRPP.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesRPP" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">

    <div class="row">
        <div class="col s12 right-align">
            <asp:GridView ID="TabCotizacionesRPP" runat="server" ViewStateMode="Disabled" ClientIDMode="Static"
                AutoGenerateColumns="False" OnRowDataBound="TabCotizacionesRPP_RowDataBound">
                <Columns>
                    <asp:TemplateField HeaderText="N°">
                        <ItemTemplate>
                            <span><%# Container.DataItemIndex + 1 %></span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Moneda">
                        <ItemTemplate>
                            <asp:DropDownList ID="TabCotMoneda" CssClass="rpp-moneda" runat="server" ClientIDMode="Static">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="1° Tramo Años">
                        <ItemTemplate>
                            <asp:DropDownList ID="TabCotPagoEscalonada" CssClass="rpp-tramo1-meses" runat="server" ClientIDMode="Static">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="1° Tramo %">
                        <ItemTemplate>
                            <asp:DropDownList ID="TabCotPjePagoEscalonada" CssClass="rpp-tramo1-porcentaje" runat="server" ClientIDMode="Static">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Per. Gar.">
                        <ItemTemplate>
                            <asp:DropDownList ID="TabCotPeriodoGarantizado" CssClass="rpp-periodo-garantizado" runat="server" ClientIDMode="Static">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Pje. Conyu.">
                        <ItemTemplate>
                            <asp:DropDownList ID="TabCotPjeConyuge" CssClass="rpp-porcentaje-conyuge" runat="server" ClientIDMode="Static">
                            </asp:DropDownList>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="TasaVenta"
                           HeaderText="Tasa Venta" DataFormatString="{0:#,##0.00}" >
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundField>

                    <%-- Se debe mostrar la pensión en su Moneda Original en la grilla --%>
                    <asp:BoundField DataField="PensionCiaMO"
                           HeaderText="Renta" DataFormatString="{0:#,##0.00}" >
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundField>

                    <asp:BoundField DataField="Pension2doTramoSinAjuste"
                           HeaderText="Renta 2° Tramo" DataFormatString="{0:#,##0.00}" >
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundField>

                    <asp:BoundField DataField="Pension2doTramo"
                           HeaderText="Renta 2° Tramo Aj." DataFormatString="{0:#,##0.00}" >
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundField>

                    <asp:BoundField DataField="TasaRetornoAccionista"
                           HeaderText="TRA" DataFormatString="{0:#,##0.00}" >
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundField>

                    <asp:BoundField DataField="IndCotiza"
                         HeaderText="Ind." >
                        <ItemStyle HorizontalAlign="Right" />
                    </asp:BoundField>

                    <asp:TemplateField HeaderText="Dif. TRA">
                        <ItemTemplate>
                            <asp:TextBox ID="TabCotTRA" runat="server" ClientIDMode="Static" CssClass="numerico inputgrilla rpp-dif-tra" data-v-min="-100.00" data-v-max="100.00" Text="0.00"></asp:TextBox>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <%# "<a class=\"center eliminar-cotizacion\" data-cotizacion=\"" + (Container.DataItemIndex + 1) + "\"><i class=\"material-icons pink-text darken-2-text\" title=\"Eliminar Cotización\">delete</i></a>"%>
                        </ItemTemplate>
                    </asp:TemplateField>
            
                </Columns>
                <EmptyDataTemplate>
                    <p style="font-size:1rem"><i class="material-icons blue-text text-darken-2" style="margin-right:10px">info</i> No se han encontrado cotizaciones para esta solicitud.</p>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
    <asp:Panel ID="TabCotizacionesBotonera_RP" CssClass="row" runat="server">
        <div class="col s12 right-align">
            <asp:HyperLink ID="AgregarCotizacion" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">add</i>Agregar</asp:HyperLink>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="TabCotizacionesLeyenda_RP" runat="server" Visible="false" style="margin-top:10px">
      <ul class="collection with-header left-align">
        <li class="collection-header"><h6><b>Leyenda</b></h6></li>
        <li class="collection-item"><span class="red-text" style="display: inline-block;width:25px;font-weight:bold">**</span>: Cotización no alcanza el TRA mínimo requerido, por lo cual no se simula.</li>
      </ul>
    </asp:Panel>
</div>
</form>