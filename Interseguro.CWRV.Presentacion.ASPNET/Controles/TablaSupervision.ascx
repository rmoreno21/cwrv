<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSupervision.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSupervision" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabSupervision" runat="server" Width="100%" CellPadding="3" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        UseAccessibleHeader="True" onrowdatabound="TabSupervision_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <%--<asp:BoundField DataField="NumeroRegistro" 
                   HeaderText="Nro" />--%>
            <asp:BoundField DataField="Jefe" 
                   HeaderText="Jefe" />
            <asp:BoundField DataField="Supervisor"
                   HeaderText="Supervisor" />
            <asp:BoundField DataField="Agente" 
                   HeaderText="Agente" />
            <asp:BoundField DataField="Usuario" 
                   HeaderText="Usuario" />
            <asp:BoundField DataField="FechaEvento" 
                   HeaderText="Fecha" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" Width="75" />
            </asp:BoundField>
            <asp:BoundField DataField="Evento" 
                   HeaderText="Evento" />
            <asp:BoundField DataField="Detalle" 
                   HeaderText="Detalle" />
        </Columns>
        <EmptyDataTemplate>
            <div align="center">
                <div class="grilla_info" align="left" style="width:220px">
                    No se ha encontrado ningún registro.
                </div>
            </div>
        </EmptyDataTemplate>
        <HeaderStyle CssClass="grilla_cabecera" />
        <RowStyle CssClass="grilla_alt1" />
    </asp:GridView>
    
    <asp:Panel ID="TabSupContenedorPaginador" runat="server" style="margin-top:5px;height:30px" Visible="False">
        <asp:Literal ID="TabSupervisionPaginador" runat="server"></asp:Literal>
        <span id="TabSupervisionPaginadorCargando" class="paginador_cargando"></span>
        <asp:Label ID="TabSupervisionPaginadorIndice" style="float:right;font-size:13px;font-family:Tahoma;padding-top:5px" runat="server"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
