<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="500.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Error._500" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        h1 {
            font-size: 2.1rem;
        }

        h2 {
            font-size: 1.3rem;
        }

        h3 {
            font-size: 1.2rem;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="row" style="margin-top:30px">
    <div class="col 12 center-align">
        <div class="recuadro-error">
            <asp:Image ID="ErrorImagen" ClientIDMode="Static" runat="server" ImageUrl="~/Imagenes/error-imagen.svg" />
            <asp:Image ID="ErrorIcono" ClientIDMode="Static" runat="server" ImageUrl="~/Imagenes/error-icono.svg"/>
            <h1 class="pink-text text-lighten-1">Error 500</h1>
            <h2 class="blue-grey-text text-darken-3">Ocurrió un error en el sistema, por favor inténtalo nuevamente, en caso de que el error persista por favor comunícate con <b>Mesa de Ayuda</b>.</h2>
            <h3 class="blue-grey-text text-lighten-1"><%= DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss tt") %></h3>
        </div>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">
</asp:Content>
