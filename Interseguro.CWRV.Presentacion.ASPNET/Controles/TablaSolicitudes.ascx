<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSolicitudes.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSolicitudes" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">
        <asp:GridView ID="TabSolicitudes" runat="server" Width="100%" CellPadding="3"
            CellSpacing="1" GridLines="None"
            ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False"
            OnRowDataBound="TabSolicitudes_RowDataBound">
            <AlternatingRowStyle CssClass="grilla_alt2" />
            <Columns>
                <asp:BoundField DataField="Id"
                    HeaderText="Solicitud" />
                <asp:BoundField DataField="FechaCotizacion"
                    HeaderText="Fec. Cotización" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="FechaSolicitud"
                    HeaderText="Fec. Solicitud" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Tipo Cotización">
                    <ItemTemplate>
                        <asp:Label ID="ItemTipoCotizacion" runat="server" Text='<%# Bind("TipoCotizacion.Id") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="FechaRecepcion"
                    HeaderText="Fec. Recepción" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="FechaPlazoAFP"
                    HeaderText="Plazo AFP" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="FechaCierre"
                    HeaderText="Fec. Cierre" DataFormatString="{0:dd/MM/yyyy}">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="NumLoteCotizacion"
                    HeaderText="Lote">
                    <ItemStyle HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:TemplateField>
                    <HeaderTemplate>
                        <asp:Label ID="CabEnviado" runat="server"
                             ToolTip="Enviado"
                             Text="Env." style="cursor:help">
                        </asp:Label>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:Label ID="LabEnviado" runat="server"
                             Text='<%# Eval("IndEnviadoSbs") %>'>
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Companhia"
                    HeaderText="Compañía" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <%# "<a class=\"grilla_boton" + (PermisoModificar ? (" grilla_editar\" data-solicitud=\"" + Eval("Id") + "\" title=\"Modificar Solicitud") : " grilla_editar_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                        <%# "<a class=\"grilla_boton" + (PermisoExportarPDF ? (" grilla_pdf\" data-solicitud=\"" + Eval("Id") + "\" data-fechacotizacion=\"" + Eval("FechaCotizacion", "{0:dd/MM/yyyy}")  + "\" data-numagente=\"" + Eval("Agente.Id") + "\" data-tipocotizacion=\"" + Eval("TipoCotizacion.Id")  + "\" title=\"Reporte Detalle Cotización") : " grilla_pdf_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>"%>
                        <%# (Consentimiento) ? "<a class=\"grilla_boton" + (PermisoCorreoElectronico ? (" grilla_email\" data-solicitud=\"" + Eval("Id") + "\" title=\"Enviar cotización por correo electrónico") : " grilla_email_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>" : ""%>
                        <%# (RedLocal) ? "<a class=\"grilla_boton" + (PermisoReporteEscenario ? (" grilla_rep_escenario\" data-solicitud=\"" + Eval("Id") + "\" data-fechacotizacion=\"" + Eval("FechaCotizacion", "{0:dd/MM/yyyy}") + "\" data-numagente=\"" + Eval("Agente.Id") + "\" title=\"Generar Reporte de Escenarios") : " grilla_rep_escenario_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>" : ""%>
                        <%# ConfigurarBotonCerrar(Eval("TipoCotizacion.Id").ToString(), (Eval("Companhia") == null) ? "" : Eval("Companhia").ToString(), Eval("firmaDigitalToken").ToString(), Eval("NumLoteCotizacion").ToString()) %>
                        <%# Convert.ToInt32(Eval("NumeroPoliza")) != 0 ? ("<a class=\"grilla_boton" + (PermisoExportarPDF && Convert.ToInt32(Eval("NumeroPoliza")) != 0 ? (" grilla_pdf_poliza\" data-poliza=\"" + Eval("NumeroPoliza") + "\" title=\"Descargar Póliza") : " grilla_pdf_poliza_deshabilitado\" title=\"Usted no tiene privilegios sobre esta opción.") + "\"></a>") : "" %>
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
