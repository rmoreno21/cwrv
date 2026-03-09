<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaCotizacionesMovimiento.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaCotizacionesMovimiento" %>

<form id="form1" runat="server" enableviewstate="False">

<div id="ContenidoDinamico">

    <asp:GridView 
        ID="TabCotizacionesMovimiento" 
        runat="server" 
        Width="100%" 
        CellPadding="1" 
        CellSpacing="1" 
        GridLines="None" 
        UseAccessibleHeader="True"
        ViewStateMode="Disabled" 
        ClientIDMode="Static" 
        AutoGenerateColumns="False"
	    DataKeyNames="NumSolicitud">

        <AlternatingRowStyle CssClass="grilla_alt2" />
        
        <Columns>

            <asp:TemplateField HeaderText="Num. Solicitud" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="CotNumSolicitud" runat="server" Text='<%# Eval("NumSolicitud") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Fec. Cotización" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="CotFecCotizacion" runat="server" Text='<%# Convert.ToDateTime(Eval("FecCotizacion")).ToString("dd/MM/yyyy") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:TemplateField HeaderText="Correlativo Cotización" ItemStyle-Width="20px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="CotCorrelativo" runat="server" Text='<%# Eval("Correlativo") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>--%>
            <asp:TemplateField HeaderText="Num. Movimiento" ItemStyle-Width="30px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="CotNumMovimiento" runat="server" Text='<%# Eval("NumMovimiento") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Tipo Movimiento" ItemStyle-Width="125px" ItemStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <asp:Label ID="CotGlsTipoMovimiento" runat="server" Text='<%# Eval("TipoMovimiento.Nombre") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Movimiento" ItemStyle-Width="125px" ItemStyle-HorizontalAlign="Left">
                <ItemTemplate>
                    <asp:Label ID="CotGlsMovimiento" runat="server" Text='<%# Eval("GlsMovimiento") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:TemplateField HeaderText="Valor Tasa TRA" ItemStyle-Width="50px">
                <ItemTemplate>
                    <asp:Label ID="CotValTra" runat="server" Text='<%# Eval("ValTasaAjusteTra") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>--%>
            <asp:TemplateField HeaderText="Fec. Inicio" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="CotFecInicio" runat="server" Text='<%# Eval("FecInicioMovimiento") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Fec. Fin" ItemStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="CotFecFin" runat="server" Text='<%# Eval("FecFinMovimiento") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Usuario" ItemStyle-Width="70px">
                <ItemTemplate>
                    <asp:Label ID="CotUsuMovimiento" runat="server" Text='<%# Eval("Usuario.NombreUsuario") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>

        <EmptyDataTemplate>
            <div class="grilla_info">
                No se ha encontrado ningún registro de ventas.
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
