<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesIFP.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesIFP" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">
        <asp:GridView ID="TabSolicitudes_RP" runat="server" Width="100%" CellPadding="3"
            CellSpacing="1" GridLines="None"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabSolicitudes_RP_RowDataBound">
            <AlternatingRowStyle CssClass="grilla_alt2" />
            <Columns>
                <asp:BoundField DataField="Id"
                    HeaderText="Solicitud" />
                <asp:BoundField DataField="FechaSolicitud"
                    HeaderText="Fec. Solicitud" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Tipo Cotización">
                    <ItemTemplate>
                        <asp:Label ID="ItemTipoCotizacion" runat="server" Text='<%# Bind("TipoCotizacion.Id") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="120px" />
                </asp:TemplateField>
                <asp:BoundField DataField="FechaDevengue"
                    HeaderText="Fec. Devengue" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:BoundField DataField="FechaVigencia"
                    HeaderText="Fec. Vigencia" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>

                <asp:TemplateField HeaderText="Mon.">
                    <ItemTemplate>
                        <asp:Label ID="ItemMoneda" runat="server" Text='<%# Bind("MonedaPrimaUnica.Simbolo") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>


                <%--PrimaUnica {0:#,##0.00}   {0:#,#.##} --%>
                <asp:BoundField DataField="PrimaUnica"
                    HeaderText="Prima Única" DataFormatString="{0:#,##0.00}">
                    <ItemStyle HorizontalAlign="Right" />
                </asp:BoundField>


                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# (PermisoConsultar)? "<a class=\"grilla_boton" + (PermisoConsultar ? (" grilla_consultar\" data-solicitud=\"" + Eval("Id") + "\" title=\"Consultar Solicitud") : " grilla_consultar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>": ""%>

                        <%--GTI.71684: Restringir cotizaciones en soles - Ingreso Flexible Plus--%>
                        <%# (ConfigurarBotonCerrar(Eval("FechaSolicitud"))) ? "<a class=\"grilla_boton" + (ConfigurarBotonCerrar(Eval("FechaSolicitud")) ? (" grilla_editar\" data-solicitud=\"" + Eval("Id") + "\" title=\"Modificar Solicitud") : " grilla_editar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>": ""%>
                        <%--<%# (ConfigurarBotonCerrar(Eval("FechaSolicitud"), Eval("MonedaPrimaUnica.Simbolo"))) ? "<a class=\"grilla_boton" + (ConfigurarBotonCerrar(Eval("FechaSolicitud"), Eval("MonedaPrimaUnica.Simbolo")) ? (" grilla_editar\" data-solicitud=\"" + Eval("Id") + "\" title=\"Modificar Solicitud") : " grilla_editar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>": ""%>--%>
                        
                        <%# "<a class=\"grilla_boton" + (PermisoExportarPDF ? (" grilla_pdf\" data-solicitud=\"" + Eval("Id") + "\" data-fechacotizacion=\"" + Eval("FechaSolicitud", "{0:dd/MM/yyyy}")  + "\" data-numagente=\"" + Eval("Agente.Id")  + "\" title=\"Reporte Detalle Cotización") : " grilla_pdf_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                        <%--<%# (Consentimiento) ? "<a class=\"grilla_boton" + (PermisoCorreoElectronico ? (" grilla_email\" data-solicitud=\"" + Eval("Id") + "\" title=\"Enviar cotización por correo electrónico") : " grilla_email_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>" : ""%>--%>
                        
                        <%--GTI.71684: Restringir cotizaciones en soles - Ingreso Flexible Plus--%>
                        <%# "<a class=\"grilla_boton" + (PermisoInsertar ? (" grilla_copiar\" data-solicitud=\"" + Eval("Id") + "\" title=\"Copiar Solicitud") : " grilla_copiar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                        <%--<%# (ConfigurarBotonCopiarParaSoles(Eval("MonedaPrimaUnica.Simbolo"))) ? "<a class=\"grilla_boton" + (ConfigurarBotonCopiarParaSoles(Eval("MonedaPrimaUnica.Simbolo")) ? (" grilla_copiar\" data-solicitud=\"" + Eval("Id") + "\" title=\"Copiar Solicitud") : " grilla_copiar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>" : ""%>--%>
                        
                        <%# ConfigurarBotonCerrar(Eval("CodigoEstado").ToString() , Eval("EstadoSolicitud").ToString()) %>
                        <%# Convert.ToInt32(Eval("NumeroPoliza")) != 0 ? ("<a class=\"grilla_boton" + (PermisoExportarPDF && Convert.ToInt32(Eval("NumeroPoliza")) != 0 ? (" grilla_pdf_poliza\" data-poliza=\"" + Eval("NumeroPoliza") + "\" title=\"Condicionado póliza") : " grilla_pdf_poliza_oculto") + "\"></a>") : "" %>
                        <%# Convert.ToInt32(Eval("IdEstudioNecesidades")) > 0 ? "<a class=\"grilla_boton" + (PermisoObtenerEdN ? (" grilla_pdf_estudio_necesidad\" data-edn=\"" + Eval("Id") + "\" title=\"Estudio de necesidades") : " grilla_pdf_estudio_necesidad_oculto") + "\"></a>": ""%>
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
