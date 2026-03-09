<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesRentaPrivadaPlusCierre.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesRentaPrivadaPlusCierre" %>
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
                <ItemStyle HorizontalAlign="Center" Width="50px"/>
            </asp:BoundField>
            <asp:TemplateField HeaderText="Tipo Cotización">
                <ItemTemplate>
                    <asp:Label ID="ItemTipoCotizacion" runat="server" Text='<%# Bind("TipoCotizacion.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="FechaDevengue"
                   HeaderText="Fec. Devengue" DataFormatString="{0:dd/MM/yyyy}">
                <ItemStyle HorizontalAlign="Center" Width="50px" />
            </asp:BoundField>

            <asp:TemplateField HeaderText="Mon.">
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
            
            <asp:TemplateField HeaderText="Estado Solicitud">
                <ItemTemplate>
                    <asp:Label ID="ItemEstadoSolicitud" runat="server" Text='<%# Bind("EstadoSolicitud") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="60px"/>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Estado Plaft">
                <ItemTemplate>
                    <asp:Label ID="ItemEstadoPLAFT" runat="server" Text='<%# Bind("EstadoSolicitudPlaft") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="80px"/>
            </asp:TemplateField>

            
            <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%--INI.GTI_7012_V13--%>
                    <%--<%# (PermisoCerrar)? "<a class=\"grilla_boton" + (PermisoCerrar ? (" grilla_grupo_familiar\" data-solicitud=\"" + Eval("Id") + "\" data-estado=\"" + Eval("CodigoEstado")   + "\" title=\"Modificar Beneficiarios") : " grilla_grupo_familiar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>": ""%>--%>
                    <%--<%# ConfigurarBotonModificarBeneficiario(Eval("CodigoEstado").ToString() , Eval("EstadoSolicitud").ToString()) %>--%>
                    <%--FIN.GTI_7012_V13--%>
                    <%# (PermisoCerrar)? "<a class=\"grilla_boton" + (PermisoCerrar ? (" grilla_cerrar\" data-tipo_cotizacion=\"" + Eval("TipoCotizacion.Id") + "\" data-solicitud=\"" + Eval("Id") + "\" title=\"Cerrar Solicitud") : " grilla_cerrar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>": ""%>
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
