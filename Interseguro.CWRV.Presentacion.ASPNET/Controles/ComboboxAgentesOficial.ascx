<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComboboxAgentesOficial.ascx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Controles.ComboboxAgentesOficial" %>
<form id="form1" runat="server" enableviewstate="False">
    <div id="ContenidoDinamico">        
        <label id="LabAgente" class="formLabel formLabel2Izq">Agente:</label>
        <asp:DropDownList ID="OfiAgente" runat="server" CssClass="formCombobox" Width="356" ClientIDMode="Static">
        </asp:DropDownList>
        <span id="CargandoAgente" class="paginador_cargando"></span>
    </div>
</form>