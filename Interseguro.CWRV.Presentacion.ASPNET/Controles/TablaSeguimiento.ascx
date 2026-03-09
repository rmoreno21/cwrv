<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TablaSeguimiento.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.TablaSeguimiento" %>
<form id="form1" runat="server" enableviewstate="False">
<div id="ContenidoDinamico">
    <asp:GridView ID="TabSeguimiento" runat="server" Width="100%" CellPadding="0" 
        CellSpacing="1" GridLines="None"
        ViewStateMode="Disabled" ClientIDMode="Static" AutoGenerateColumns="False" 
        UseAccessibleHeader="True" onrowdatabound="TabSeguimiento_RowDataBound">
        <AlternatingRowStyle CssClass="grilla_alt2" />
        <Columns>
            <asp:BoundField DataField="Jefe" 
                   HeaderText="Jef" />
            <asp:BoundField DataField="Supervisor"
                   HeaderText="Spv" />
            <asp:BoundField DataField="Agente" 
                   HeaderText="Agt" />
            <asp:BoundField DataField="CUSPP" 
                   HeaderText="CUSPP" />
            <asp:BoundField DataField="Persona" 
                   HeaderText="Persona" />
            <asp:BoundField DataField="SitioGenerado" 
                   HeaderText="Origen" />
            <asp:BoundField DataField="Telefono" 
                   HeaderText="Telef" />
            <asp:BoundField DataField="SaldoCIC" 
                   HeaderText="CIC" DataFormatString="{0:#,#.##}" >
                <ItemStyle HorizontalAlign="Right" />
            </asp:BoundField>
            <asp:BoundField DataField="NroSolicitud" 
                   HeaderText="Solicitud" />
            <asp:BoundField DataField="Categoria" 
                   HeaderText="Catego" />
            <asp:BoundField DataField="FechaIngreso" 
                   HeaderText="Ingreso" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" Width="75" />
            </asp:BoundField>
            <asp:BoundField DataField="FechaCierre" 
                   HeaderText="Cierre" DataFormatString="{0:dd/MM/yyyy}" >
                <ItemStyle HorizontalAlign="Center" Width="75" />
            </asp:BoundField>
            <asp:BoundField DataField="Companhia" 
                   HeaderText="Cía">
                 <ItemStyle HorizontalAlign="Center" Width="60" />
            </asp:BoundField>
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
    
    <asp:Panel ID="TabSegContenedorPaginador" runat="server" style="margin-top:5px;height:30px" Visible="False">
        <asp:Literal ID="TabSeguimientoPaginador" runat="server"></asp:Literal>
        <span id="TabSeguimientoPaginadorCargando" class="paginador_cargando"></span>
        <asp:Label ID="TabSeguimientoPaginadorIndice" style="float:right;font-size:13px;font-family:Tahoma;padding-top:5px" runat="server"></asp:Label>
    </asp:Panel>

    <asp:Panel ID="SinPermisos" align="center" runat="server" ClientIDMode="Static" Visible="False">
        <div class="grilla_error" align="left" style="width:375px">
            Usted no cuenta con privilegios para visualizar esta información.
        </div>
    </asp:Panel>
</div>
</form>
