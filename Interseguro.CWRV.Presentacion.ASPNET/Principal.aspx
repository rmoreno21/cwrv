<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" EnableSessionState="ReadOnly" AutoEventWireup="true" CodeBehind="Principal.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Principal" %>
<%@ Register TagPrefix="cwrv" TagName="MenuMosaico" Src="~/Controles/MenuMosaico.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div id="ContenedorPrincipal" align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e">
        <h1 class="simple" style="width:125px">Menú Principal</h1>
        
        <div align="center" style="overflow:hidden;width:100%;border:1px solid #000">
            <div class="text-center">
                <h2>Redirigiendo...</h2>
            </div>
            <!-- <cwrv:MenuMosaico id="MenuMosaico" runat="server" /> -->
        </div>
    </div>
</asp:Content>
