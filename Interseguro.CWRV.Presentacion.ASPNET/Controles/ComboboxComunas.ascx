<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxComunas.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ComboboxComunas" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">        
        <label id="LabModDirComuna" class="formLabel formLabel2Izq">Distrito*:</label>
        <asp:DropDownList ID="ModDirComuna" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static">
        </asp:DropDownList>
        <span id="ModDirCargandoComuna" class="paginador_cargando"></span>
    </div>
</form>