<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxCiudadesPrincipal.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ComboboxCiudadesPrincipal" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">        
        <label id="LabModProvinciaPrinc" class="formLabel formLabel2Izq">Provincia*:</label>
        <asp:DropDownList ID="ModProvinciaPrinc" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static">
        </asp:DropDownList>
        <span id="ModCargandoProvincia" class="paginador_cargando"></span>
    </div>
</form>
