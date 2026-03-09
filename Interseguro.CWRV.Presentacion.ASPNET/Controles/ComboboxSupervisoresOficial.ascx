<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxSupervisoresOficial.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ComboboxSupervisoresOficial" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">        
        <label id="LabSupervisor" class="formLabel formLabel2Izq">Supervisor:</label>
        <asp:DropDownList ID="OfiSupervisor" runat="server" CssClass="formCombobox" Width="356" ClientIDMode="Static">
        </asp:DropDownList>
        <span id="CargandoSupervisor" class="paginador_cargando"></span>
    </div>
</form>