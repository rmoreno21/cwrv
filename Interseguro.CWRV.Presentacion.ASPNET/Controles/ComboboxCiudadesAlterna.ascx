<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxCiudadesAlterna.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ComboboxCiudadesAlterna" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">        
        <label id="LabModProvinciaAlterna" class="formLabel formLabel2Izq">Provincia*:</label>
        <asp:DropDownList ID="ModProvinciaAlterna" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static"></asp:DropDownList>
        <span id="ModCargandoProvinciaAlterna" class="paginador_cargando"></span>
    </div>
</form>