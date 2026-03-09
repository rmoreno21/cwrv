<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxCiudades.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ComboboxCiudades" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">        
        <label id="LabModDirCiudad" class="formLabel formLabel2Izq">Provincia*:</label>
        <asp:DropDownList ID="ModDirCiudad" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static">
        </asp:DropDownList>
        <span id="ModDirCargandoCiudad" class="paginador_cargando"></span>
    </div>
</form>