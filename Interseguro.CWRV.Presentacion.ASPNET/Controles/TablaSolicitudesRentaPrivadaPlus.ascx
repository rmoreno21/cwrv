<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudesRentaPrivadaPlus.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudesRentaPrivadaPlus" %>
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

                <asp:TemplateField HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# (PermisoConsultar)? "<a class=\"grilla_boton" + (PermisoConsultar ? (" grilla_consultar\" href=\"" + ResolveUrl("~/RPP/Cotizador.aspx") + "?m=C&c=" + Eval("Afiliado.CUSPP") + "&s=" + Eval("Id") + "\" data-solicitud=\"" + Eval("Id") + "\" title=\"Consultar Solicitud") : " grilla_consultar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>": ""%>

                        <%# (PermisoModificar) ? "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_editar\" href=\"" + ResolveUrl("~/RPP/Cotizador.aspx") + "?m=M&c=" + Eval("Afiliado.CUSPP") + "&s=" + Eval("Id") + "\" data-solicitud=\"" + Eval("Id") + "\" title=\"Modificar Solicitud") : " grilla_editar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>": ""%>
                        <%# "<a class=\"grilla_boton" + (PermisoExportarPDF ? (" grilla_pdf\" data-solicitud=\"" + Eval("Id") + "\" data-fechacotizacion=\"" + Eval("FechaSolicitud", "{0:dd/MM/yyyy}")  + "\" data-numagente=\"" + Eval("Agente.Id")  + "\" title=\"Reporte Detalle Cotización") : " grilla_pdf_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                        <%# (Consentimiento) ? "<a class=\"grilla_boton" + (PermisoCorreoElectronico ? (" grilla_email\" data-solicitud=\"" + Eval("Id") + "\" title=\"Enviar cotización por correo electrónico") : " grilla_email_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>" : ""%>
                        <%# "<a class=\"grilla_boton" + (PermisoInsertar ? (" grilla_copiar\" href=\"" + ResolveUrl("~/RPP/Cotizador.aspx") + "?m=D&c=" + Eval("Afiliado.CUSPP") + "&s=" + Eval("Id") + "\" data-solicitud=\"" + Eval("Id") + "\" title=\"Copiar Solicitud") : " grilla_copiar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
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
