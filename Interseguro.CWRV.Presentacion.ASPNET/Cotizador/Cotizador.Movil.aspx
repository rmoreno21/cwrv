<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Movil.Master" AutoEventWireup="true" CodeBehind="Cotizador.Movil.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.Cotizador_Movil" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Búsqueda de Afiliados</h2>

    <div class="ui-field-contain">
        <label for="BusAfiNroSolicitud">Número de Solicitud:</label>
        <asp:TextBox ID="BusAfiNroSolicitud" runat="server" ClientIDMode="Static" data-clear-btn="true"></asp:TextBox>
    </div>

    <div class="ui-field-contain">
        <label for="BusAfiCUSPP">CUSPP:</label>
        <asp:TextBox ID="BusAfiCUSPP" runat="server" ClientIDMode="Static" data-clear-btn="true"></asp:TextBox>
    </div>

    <asp:Button ID="BusAfiBuscar" runat="server" Text="Buscar" />
</asp:Content>