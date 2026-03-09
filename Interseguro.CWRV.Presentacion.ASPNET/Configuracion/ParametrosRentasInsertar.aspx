<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="ParametrosRentasInsertar.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Configuracion.ParametrosRentasInsertar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        table {
            font-size: 12px;
        }

        table input {
            font-size: 12px !important;
        }

        td select {
            font-size: 12px !important;
        }

        .no-cotizable {
            background-color: #ffcdd2;
        }

        .seleccionada {
            background-color: #c8e6c9;
        }

        .modificar-parametro {
            cursor: pointer;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="row">
    <div class="col s12">
        <div class="card">
            <div class="card-content">
                <span class="card-title blue-text text-darken-3">
                    Parámetros de Rentas
                </span>
                <div class="row">
                    <%--<div class="col s12">
                        <ul class="tabs">
                            <li class="tab col s3"><a href="#valpar">Cotizador</a></li>
                            <li class="tab col s3"><a href="#parash">Asset Share</a></li>
                            <li class="tab col s3"><a href="#parinv">Inversiones</a></li>
                            <li class="tab col s3"><a href="#tabmor">Tablas de Mortalidad</a></li>
                        </ul>
                    </div>--%>
                    <div id="valpar" class="col s12">
                        <div class="row">
                            <div class="input-field col s12 m6">
                                <a name="ancla-solicitud"></a>
                                <i class="material-icons prefix">shopping_cart</i>
                                <asp:DropDownList ID="Producto" runat="server" ClientIDMode="Static">
                                </asp:DropDownList>
                                <label for="Producto">Producto</label>
                            </div>
                            <div class="input-field col s12 m6">
                                <i class="material-icons prefix">calendar_today</i>
                                <asp:TextBox ID="Fecha" runat="server" ClientIDMode="Static" CssClass="validate fecha datepicker"></asp:TextBox>
                                <label for="Fecha">Fecha</label>
                            </div>
                        </div>
                        <div class="row">
                            <div class="input-field col s12 m6">
                                <a name="ancla-solicitud"></a>
                                <i class="material-icons prefix">category</i>
                                <asp:DropDownList ID="ParametroRentas" runat="server" ClientIDMode="Static">
                                </asp:DropDownList>
                                <label for="ParametroRentas">Parámetro</label>
                            </div>
                            <div class="input-field col s12 m6">
                                <i class="material-icons prefix">price_change</i>
                                <asp:DropDownList ID="Moneda" runat="server" ClientIDMode="Static">
                                </asp:DropDownList>
                                <label for="Moneda">Moneda</label>
                            </div>
                        </div>
                        <div class="row center">
                            <div class="col s12">
                                <asp:HyperLink ID="Buscar" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">search</i>Buscar</asp:HyperLink>
                                <asp:HyperLink ID="Nuevo" ClientIDMode="Static" runat="server" CssClass="waves-effect waves-light btn blue darken-2"><i class="material-icons left">add</i>Nuevo</asp:HyperLink>
                            </div>
                        </div>
                        <div class="row center">
                            <a name="ancla-parametros"></a>
                            <div id="TablaParametros" class="col s12 black-text center-align" style="display:none">
                                aaa
                            </div>
                            <div id="TablaParametrosCargando" class="col s12 valign-wrapper center" style="display:none">
                                <div class="preloader-wrapper small active" style="width:24px;height:24px">
                                    <div class="spinner-layer spinner-blue-only">
                                        <div class="circle-clipper left">
                                            <div class="circle"></div>
                                        </div>
                                        <div class="gap-patch">
                                            <div class="circle"></div>
                                        </div>
                                        <div class="circle-clipper right">
                                            <div class="circle"></div>
                                        </div>
                                    </div>
                                </div>
                                <span style="margin-left:10px">
                                    Cargando parámetros...
                                </span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>