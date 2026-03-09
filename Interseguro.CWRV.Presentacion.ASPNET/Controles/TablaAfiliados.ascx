<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaAfiliados.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaAfiliados" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabAfiliados" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        onrowdatabound="TabAfiliados_RowDataBound" UseAccessibleHeader="True">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:BoundField DataField="ApellidoPaterno" 
                   HeaderText="Apellido Paterno" />
            <asp:BoundField DataField="ApellidoMaterno"
                   HeaderText="Apellido Materno" />
            <asp:BoundField DataField="Nombre" 
                   HeaderText="Nombre" />
            <asp:BoundField DataField="CUSPP" 
                   HeaderText="CUSPP" >
            <ItemStyle Width="100px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="">
                <ItemTemplate>
                </ItemTemplate>
                <ItemStyle HorizontalAlign="Center" Width="20px" />
            </asp:TemplateField>
        </Columns>
        <EmptyDataTemplate>
            <div align="center">
                <div class="grilla_info" align="left" style="width:280px">
                    No se ha encontrado ningún registro de afiliado.
                </div>
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>
    
    <asp:Panel ID="TabAfiContenedorPaginador" runat="server" style="margin-top:5px;height:30px" Visible="False">
        <asp:Literal ID="TabAfiliadosPaginador" runat="server"></asp:Literal>
        <span id="TabAfiliadosPaginadorCargando" class="paginador_cargando"></span>
        <asp:Label ID="TabAfiliadosPaginadorIndice" style="float:right;font-size:13px;font-family:Tahoma;padding-top:5px" runat="server"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="ModBusAfiContenedorAceptar" runat="server" CssClass="formLinea" align="center" Visible="False">
        <a id="ModBusAfiAceptar" href="javascript:void(0);" style="width:80px;height:22px" class="boton darkblue sharp">Aceptar</a>
    </asp:Panel>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
