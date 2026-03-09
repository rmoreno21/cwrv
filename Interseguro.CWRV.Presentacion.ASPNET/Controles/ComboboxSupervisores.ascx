<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxSupervisores.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ComboboxSupervisores" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">        
        <label id="LabSupervisor" class="formLabel formLabel2Izq">Supervisor:</label>
        <asp:DropDownList ID="Supervisor" runat="server" CssClass="formCombobox" Width="356" ClientIDMode="Static">
        </asp:DropDownList>
        <span id="CargandoSupervisor" class="paginador_cargando"></span>
    </div>
</form>