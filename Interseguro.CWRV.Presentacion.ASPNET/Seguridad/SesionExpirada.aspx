<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SesionExpirada.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Seguridad.SesionExpirada" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <meta http-equiv="X-UA-Compatible" content="IE=9; IE=8; IE=7; IE=EDGE" />

    <title>Interseguro | Cotizador Web de Rentas </title>

    <link type="image/x-icon" href="~/Imagenes/favicon.ico" rel="shortcut icon" />
    <link type="image/x-icon" href="~/Imagenes/favicon.ico" rel="icon" />

    <link rel="stylesheet" href="~/Estilos/CotWebRVI.css" type="text/css" />

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/jquery-1.8.3.min.js")%>"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            localStorage.clear();
            setTimeout(function () {
                window.location.href = '<%=ResolveUrl("~/Seguridad/IniciarSesion.aspx")%>';
            }, 5000);
        });
    </script>
</head>
<body>
    <div id="Contenedor">
        <form id="form1" runat="server">
        <div id="CabeceraPagina" align="center">
            <div style="width:900px;height:100px">
                <div id="CabeceraIzq" align="left" style="float:left;width:350px;height:100px">
                    <div id="LogoInterseguro"></div>
                    <div id="TituloSistema">Cotizador Web de Rentas </div>
                </div>
            </div>
        </div>

        <div id="CuerpoPagina" align="center">
            <div align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">
                <div class="grilla_error">
                    Su sesi�n ha expirado y ha sido finalizada. Por favor <asp:HyperLink ID="IniSesion" runat="server" NavigateUrl="~/Seguridad/IniciarSesion.aspx">inicie sesi�n</asp:HyperLink> nuevamente para acceder al sistema.
                </div>
            </div>
        </div>

        <div id="PiePagina"></div>
        </form>
    </div>
</body>
</html>
