<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="PlanesIFP.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.PlanesIFP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/bootstrap.css")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>" type="text/css" />
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/bootstrap.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

     <style type="text/css">

        .glyphicon-lg{font-size:3em}
        .blockquote-box{margin-bottom:20px}
        .blockquote-box .square{width:100px;margin-right:3px;text-align:center!important;background-color:#E6E6E6;padding:20px 10px 16px 0px}

        .alert-info {
            margin-left: 113px;
            margin-right: 40px;
            margin-top: -7px;
            font-size: 14px;
        }

         .btn {
             font-weight: bold;
             padding: 10px 35px;
             font-size: 17px;
         }

         .alert {
             padding: 11px;
             margin-bottom: 0px;
         }

    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="NuevaCotizacionIFP" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">

        <h1 class="simple" style="width: 250px">Cotización Renta IFP</h1>

        <%--<div class="container">--%>

        <div class="row">

            <div class="col-md-12">

                <div class="blockquote-box clearfix">
                    <div class="square pull-left">
                        <img src="../Imagenes/P1.PNG" style="height: 80px; width: 88px; padding-left: 10px;" alt="" class="" />
                    </div>
                    <h4>
                        <label for="success" class="btn btn-success">Plan 1 </label>
                    </h4>
                    <div class="alert alert-info">
                        <strong>Renta Temporal</strong> sin Período Garantizado + Devolución de prima por sobrevivencia + Devolución de prima por fallecimiento.
                    </div>
                </div>

                <div class="blockquote-box clearfix">
                    <div class="square pull-left">
                        <img src="../Imagenes/P2.PNG" style="height: 80px; width: 88px; padding-left: 10px;" alt="" class="" />
                    </div>
                    <h4>
                        <label for="success" class="btn btn-success">Plan 2 </label>
                    </h4>
                    <div class="alert alert-info">
                        <strong>Renta Temporal</strong> sin Período Garantizado + Devolución de prima por sobrevivencia + Devolución de rentas no devengadas por fallecimiento.
                    </div>
                </div>

                <div class="blockquote-box clearfix">
                    <div class="square pull-left">
                        <img src="../Imagenes/P3.PNG" style="height: 80px; width: 88px; padding-left: 10px;" alt="" class="" />
                    </div>
                    <h4>
                        <label for="success" class="btn btn-success">Plan 3 </label>
                    </h4>
                    <div class="alert alert-info">
                        <strong>Renta Temporal</strong> Full Garantizada + Devolución de prima a todo evento.
                    </div>
                </div>

                <div class="blockquote-box clearfix">
                    <div class="square pull-left">
                        <img src="../Imagenes/P4.PNG" style="height: 80px; width: 88px; padding-left: 10px;" alt="" class="" />
                    </div>
                    <h4>
                        <label for="success" class="btn btn-success">Plan 4 </label>
                    </h4>
                    <div class="alert alert-info">
                        <strong>Renta Vitalicia</strong> sin Período Garantizado.
                    </div>
                </div>

                <div class="blockquote-box clearfix">
                    <div class="square pull-left">
                        <img src="../Imagenes/P5.PNG" style="height: 80px; width: 88px; padding-left: 10px;" alt="" class="" />
                    </div>
                    <h4>
                        <label for="success" class="btn btn-success">Plan 5 </label>
                    </h4>
                    <div class="alert alert-info">
                        <strong>Renta Vitalicia</strong> con Período Garantizado.
                    </div>
                </div>

            </div>

        </div>

        <%--</div>--%>
        
    </asp:Panel>
    
</asp:Content>
