<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CerrarSesion.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Seguridad.CerrarSesion" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

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
    <form id="form1" runat="server">
    <div>
    
    </div>
    </form>
</body>
</html>
