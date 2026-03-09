<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesEvaluacion.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesEvaluacion" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">
        <asp:GridView ID="TabSolicitudes_RPP_Evaluacion" runat="server" Width="100%" CellPadding="3"
            CellSpacing="1" GridLines="None"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabSolicitudes_RPP_Evaluacion_RowDataBound">
            <AlternatingRowStyle CssClass="grilla_alt2" />
            <Columns>
                <asp:BoundField DataField="Id"
                    HeaderText="Solicitud" />

                <asp:BoundField DataField="FechaSolicitud"
                    HeaderText="Fec. Solicitud" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:TemplateField HeaderText="CUSPP">
                    <ItemTemplate>
                        <asp:Label ID="Cuspp" runat="server" Text='<%# Bind("Afiliado.CUSPP") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Asegurado">
                    <ItemTemplate>
                        <asp:Label ID="Asegurado" runat="server" Text='<%# Bind("Afiliado.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Tipo Cotización" Visible="false">
                    <ItemTemplate>
                        <asp:Label ID="ItemTipoCotizacion" runat="server" Text='<%# Bind("TipoCotizacion.Id") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="FechaDevengue" Visible="false"
                    HeaderText="Fec. Devengue" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:TemplateField HeaderText="Mon.">
                    <ItemTemplate>
                        <asp:Label ID="ItemMoneda" runat="server" Text='<%# Bind("MonedaPrimaUnica.Simbolo") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="PrimaUnica"
                    HeaderText="Prima Única" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>

                <asp:TemplateField HeaderText="Temporalidad" Visible="false">
                    <ItemTemplate>
                        <asp:Label ID="ItemTemporalidad" runat="server" Text='<%# Bind("Temporalidad.Nombre") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Estado Solicitud">
                    <ItemTemplate>
                        <asp:Label ID="ItemEstadoSolicitud" runat="server" Text='<%# Bind("EstadoSolicitud") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Estado Plaft">
                    <ItemTemplate>
                        <asp:Label ID="ItemEstadoPLAFT" runat="server" Text='<%# Bind("EstadoSolicitudPlaft") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="LN">
                    <ItemTemplate>
                        <asp:Label ID="ItemListaNegra" runat="server" Text='<%# (Convert.ToInt32(Eval("ListaNegra"))==0)?"No":"<strong>Si</strong>" %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# "<a class=\"grilla_boton" + " grilla_consultar\" data-solicitud=\"" + Eval("Id") + "\" title=\"Consultar Solicitud en Evaluación" + "\"></a>"%>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
            <EmptyDataTemplate>
                <div class="grilla_info">
                    No se ha encontrado ningún registro de solicitud.
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
