<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesRentaPrivada.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesRentaPrivada" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabSolicitudes_RP" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        onrowdatabound="TabSolicitudes_RP_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:BoundField DataField="Id"
                   HeaderText="Solicitud" />
            <asp:BoundField DataField="FechaSolicitud"
                   HeaderText="Fec. Solicitud" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Tipo Cotización">
                <ItemTemplate>
                    <asp:Label ID="ItemTipoCotizacion" runat="server" Text='<%# Bind("TipoCotizacion.Id") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="FechaDevengue"
                   HeaderText="Fec. Devengue" DataFormatString="{0:dd/MM/yyyy}">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>

            
           <%-- <asp:BoundField DataField="Moneda" 
                   HeaderText="Moneda">
                <ItemStyle HorizontalAlign="Center" />
                <ItemTemplate>
                    <asp:Label ID="ItemTipoCotizacion" runat="server" Text='<%# Bind("TipoCotizacion.Id") %>'></asp:Label>
                </ItemTemplate>
            </asp:BoundField>--%>

            <asp:TemplateField HeaderText="Moneda">
                <ItemTemplate>
                    <asp:Label ID="ItemMoneda" runat="server" Text='<%# Bind("MonedaPrimaUnica.Simbolo") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>


            <%--PrimaUnica {0:#,##0.00}   {0:#,#.##} --%>
            <asp:BoundField DataField="PrimaUnica"
                   HeaderText="Prima Única" DataFormatString="{0:#,##0.00}"> 
                <ItemStyle HorizontalAlign="Right"/>
            </asp:BoundField>

            <asp:TemplateField HeaderText="Temporalidad">
                <ItemTemplate>
                    <asp:Label ID="ItemTemporalidad" runat="server" Text='<%# Bind("Temporalidad.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <%--Falta--%>
            <%--<asp:BoundField DataField="Temporalidad" 
                   HeaderText="Temporalidad">
                <ItemStyle HorizontalAlign="Center" />
            </asp:BoundField>--%>

            
            <asp:TemplateField>
                <ItemTemplate>
                    <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_editar\" data-solicitud=\"" + Eval("Id") + "\" title=\"Modificar Solicitud") : " grilla_editar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                    <%# "<a class=\"grilla_boton" + (PermisoExportarPDF ? (" grilla_pdf\" data-solicitud=\"" + Eval("Id") + "\" data-fechacotizacion=\"" + Eval("FechaSolicitud", "{0:dd/MM/yyyy}")  + "\" data-numagente=\"" + Eval("Agente.Id")  + "\" title=\"Reporte Detalle Cotización") : " grilla_pdf_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                    <%# (Consentimiento) ? "<a class=\"grilla_boton" + (PermisoCorreoElectronico ? (" grilla_email\" data-solicitud=\"" + Eval("Id") + "\" title=\"Enviar cotización por correo electrónico") : " grilla_email_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>" : ""%>
                    <%--<SRIINI06326>--%>
                    <%--<%# (RedLocal) ? "<a class=\"grilla_boton" + (PermisoReporteEscenario ? (" grilla_rep_escenario\" data-solicitud=\"" + Eval("Id") + "\" data-fechacotizacion=\"" + Eval("FechaSolicitud", "{0:dd/MM/yyyy}") + "\" data-numagente=\"" + Eval("Agente.Id") + "\" title=\"Generar Reporte de Escenarios") : " grilla_rep_escenario_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>" : ""%>--%>
                    <%--<SRIFIN06326>--%>
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
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
