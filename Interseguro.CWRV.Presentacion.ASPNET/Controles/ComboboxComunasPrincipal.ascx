<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxComunasPrincipal.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ComboboxComunasPrincipal" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">        
        <label id="LabModDistritoPrinc" class="formLabel formLabel2Izq">Distrito*:</label>
        <asp:DropDownList ID="ModDistritoPrinc" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static">
        </asp:DropDownList>
        <span id="ModDirCargandoComuna" class="paginador_cargando"></span>
    </div>
</form>