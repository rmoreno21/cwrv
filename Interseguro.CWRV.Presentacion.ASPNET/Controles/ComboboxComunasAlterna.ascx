<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxComunasAlterna.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ComboboxComunasAlterna" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">        
        <label id="LabModDistritoAlterna" class="formLabel formLabel2Izq">Distrito*:</label>
        <asp:DropDownList ID="ModDistritoAlterna" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static">
        </asp:DropDownList>
        <span id="ModDirCargandoComuna" class="paginador_cargando"></span>
    </div>
</form>