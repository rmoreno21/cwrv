<%@ Page Title="" Language="C#" MasterPageFile="~/CWRV.Master" AutoEventWireup="true" CodeBehind="DashboardCntoFD.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Reportes.DashboardCntoFD" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../Estilos/app-assets/vendors/data-tables/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="../Estilos/app-assets/css/pages/data-tables.min.css" rel="stylesheet" />
    <%--<link href="../Estilos/app-assets/css/Estilos.css" rel="stylesheet" />--%>
    <link href="../Estilos/date-range-picker/daterangepicker.min.css" rel="stylesheet" />
    <link href="../Estilos/app-assets/vendors/chartist-js/chartist.min.css" rel="stylesheet" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="preloader-background">
        <div class="preloader-wrapper big active">
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
    </div>

    <%--<div class="navbar">Dashboard</div>--%>

    <%--<div id="main">--%>

    <%--busqueda fecha--%>
    <div class="row">

        <div class="col s12 m12 l12">

            <%--<div class="section">--%>
            <div class="card">
                <div class="card-content">
                    <span class="card-title blue-text text-darken-3">Dashboard de envíos</span>
                    <div class="row">
                        <%--<div class="col s1">
                        </div>--%>
                        <div class="col s4">

                            <div class="input-field">
                                <i class="material-icons prefix active">date_range</i>
                                <input id="date-range" size="40" placeholder="Seleccionar un rango de fechas" value="" />
                                <%--<label for="date-range">Rango de fechas</label>--%>
                            </div>

                        </div>
                    </div>

                </div>
            </div>
            <%--</div>--%>
        </div>
        <%--col--%>

        <div id="main">

            <%--chart consentimientos--%>
            <%--<div class="row">--%>
            <div class="col s6 m6 l6">

                <div id="cntoRV-chart" class="ct-chart card">
                    <div class="card-content">
                        <span class="card-title blue-text text-darken-3">Consentimiento de asesoría</span>
                        <%--<h4 class="card-title">Consentimiento de RRVV</h4>--%>
                        <p id="caption-cntoRV" class="caption">
                            La etiqueta verde claro representa a los consentimientos firmados, mientras que el rojo claro representa a los consentimientos no firmados.
                        </p>
                    </div>

                </div>

            </div>

            <div class="col s6 m6 l6">

                <div id="fdRV-chart" class="ct-chart card">
                    <div class="card-content">
                        <span class="card-title blue-text text-darken-3">VCTP de RRVV</span>
                        <%--<h4 class="card-title">VCTP de RRVV</h4>--%>
                        <p id="caption-fdRV" class="caption">
                            La etiqueta verde claro representa a los VCTP firmados, mientras que el rojo claro representa a los VCTP no firmados.
                        </p>
                    </div>

                </div>

            </div>
            <%--col--%>
            <%--</div>--%>

            <%--grilla consentimientos--%>
            <div class="col s12 m12">

                <%--<div id="main">--%>
                <%-- <div class="row">--%>
                <%--<div class="container">--%>

                <div class="section-data-tables">

                    <%--<div class="section section-data-tables">--%>

                    <!-- DataTables example -->
                    <%--<div class="row">--%>

                    <div id="button-trigger1" class="card">
                        <div class="card-content">
                            <span class="card-title blue-text text-darken-3">Consentimiento de asesoría
                                <%--<i class="material-icons float-right">more_vert</i>--%>
                                <div class="dropdown float-right">
                                    <a class="dropdown-trigger" href="#" data-target="1">
                                        <i class="material-icons ">more_vert</i>
                                    </a>
                                    <ul id="1" class="dropdown-content" tabindex="0" style="">
                                        <li class="consentimientoRV" tabindex="0">
                                            <a href="#!">
                                                <i class="material-icons">cloud_download</i>
                                                <span class="menu-item">Trazabilidad de envíos</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                            </span>
                            <%--<h4 class="card-title">Consentimiento de RRVV</h4>--%>
                            <%--<div class="row">--%>

                            <table id="table_Consentimiento_RV" class="display nowrap">
                                <thead>
                                    <tr>
                                        <th>Identificación</th>
                                        <th>Nombres y apellidos</th>
                                        <th>CUSPP</th>
                                        <th>Fec. primer env.</th>
                                        <th>Fec. último env.</th>
                                        <th>Firmado</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <%--<tr>
                                                                <td>Tiger Nixon</td>
                                                                <td>System Architect</td>
                                                                <td>Edinburgh</td>
                                                                <td>61</td>
                                                                <td>2011/04/25</td>
                                                                <td>2011/04/25</td>
                                                                <td>$320,800</td>
                                                            </tr>--%>
                                </tbody>

                                <tfoot>
                                    <tr>
                                        <th>Identificación</th>
                                        <th>Nombres y apellidos</th>
                                        <th>CUSPP</th>
                                        <th>Fec. primer env.</th>
                                        <th>Fec. último env.</th>
                                        <th>Firmado</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </tfoot>
                            </table>

                            <%--</div>--%>
                        </div>
                        <%--card content--%>
                    </div>

                    <%--card--%>

                    <%--</div>--%>
                </div>

                <%--</div>--%>
                <%--</div> <%--main--%>
                <%-- </div>--%>
            </div>
            <%--main--%>

            <%--<div class="row">--%>

            <%--grilla firma digital--%>
            <div class="col s12 m12 l12">

                <%--<div class="container">--%>

                <div class="section-data-tables">

                    <!-- DataTables example -->
                    <%--<div class="row">--%>

                    <div id="button-trigger3" class="card">
                        <div class="card-content">
                            <span class="card-title blue-text text-darken-3">VCTP de RRVV

                                <div class="dropdown float-right">
                                    <a class="dropdown-trigger" href="#" data-target="3">
                                        <i class="material-icons">more_vert</i>
                                    </a>
                                    <ul id="3" class="dropdown-content" tabindex="3" style="">
                                        <li class="firmadigitalRV" tabindex="3">
                                            <a href="#">
                                                <i class="material-icons">cloud_download</i>
                                                <span class="menu-item">Trazabilidad de envíos</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                            </span>
                            <%--<h4 class="card-title">VCTP de RRVV</h4>--%>
                            <%--<div class="row">--%>

                            <table id="table_Firma_Digital_RV" class="display nowrap">
                                <thead>
                                    <tr>
                                        <th>Solicitud</th>
                                        <th>Nombres y apellidos</th>
                                        <th>Identificación</th>
                                        <th>CUSPP</th>
                                        <th>Fec. primer env.</th>
                                        <th>Fec. último env.</th>
                                        <th>Firmado</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </thead>

                                <tbody>
                                </tbody>

                                <tfoot>
                                    <tr>
                                        <th>Solicitud</th>
                                        <th>Nombres y apellidos</th>
                                        <th>Identificación</th>
                                        <th>CUSPP</th>
                                        <th>Fec. primer env.</th>
                                        <th>Fec. último env.</th>
                                        <th>Firmado</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </tfoot>
                            </table>

                            <%--</div>--%>
                        </div>
                    </div>

                    <%--</div>--%>
                </div>

                <%--</div>--%>
            </div>

            <%--grilla póliza--%>            
            <div class="col s12 m12 l12">

                <%--<div class="container">--%>

                <div class="section-data-tables">

                    <!-- DataTables example -->
                    <%--<div class="row">--%>

                    <div id="button-trigger5" class="card">
                        <div class="card-content">
                            <span class="card-title blue-text text-darken-3">Póliza Electrónica de RRVV

                                <div class="dropdown float-right">
                                    <a class="dropdown-trigger" href="#" data-target="5">
                                        <i class="material-icons">more_vert</i>
                                    </a>
                                    <ul id="5" class="dropdown-content" tabindex="5" style="">
                                        <li class="polizaRV" tabindex="5">
                                            <a href="#">
                                                <i class="material-icons">cloud_download</i>
                                                <span class="menu-item">Trazabilidad de envíos</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                            </span>
                            <%--<h4 class="card-title">Póliza Electrónica de RRVV</h4>--%>
                            <%--<div class="row">--%>

                            <table id="table_Poliza_RV" class="display nowrap">
                                <thead>
                                    <tr>
                                        <th>Solicitud</th>
                                        <th>Nombres y apellidos</th>
                                        <th>Identificación</th>
                                        <th>CUSPP</th>
                                        <th>Póliza</th>
                                        <th>Fecha de envío</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <%--<tr>
                                                                <td>Tiger Nixon</td>
                                                                <td>System Architect</td>
                                                                <td>Edinburgh</td>
                                                                <td>61</td>
                                                                <td>2011/04/25</td>
                                                                <td>2011/04/25</td>
                                                                <td>$320,800</td>
                                                            </tr>--%>
                                </tbody>

                                <tfoot>
                                    <tr>
                                        <th>Solicitud</th>
                                        <th>Nombres y apellidos</th>
                                        <th>Identificación</th>
                                        <th>CUSPP</th>
                                        <th>Póliza</th>
                                        <th>Fecha de envío</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </tfoot>
                            </table>

                            <%--</div>--%>
                        </div>
                    </div>

                    <%--</div>--%>
                </div>

                <%--</div>--%>
            </div>


            <%--chart firma digital--%>
            <%--<div class="row">--%>

            <div class="col s6 m6 l6">

                <div id="cntoRP-chart" class="ct-chart card">
                    <div class="card-content">
                        <span class="card-title blue-text text-darken-3">VCTP + ADN de RPP & IFP</span>
                        <%--<h4 class="card-title">VCTP + ADN de RPP & IFP</h4>--%>
                        <p id="caption-cntoRP" class="caption">
                            La etiqueta verde claro representa a los consentimientos firmados, mientras que el rojo claro representa a los consentimientos no firmados.
                        </p>
                    </div>
                </div>

            </div>

            <div class="col s6 m6 l6">

                <div id="fdRP-chart" class="ct-chart card">
                    <div class="card-content">
                        <span class="card-title blue-text text-darken-3">Firma Digital de RPP & IFP</span>
                        <%--<h4 class="card-title">Firma Digital de RPP & IFP</h4>--%>
                        <p id="caption-fdRP" class="caption">
                            La etiqueta verde claro representa a las firmas digitales firmadas, mientras que el rojo claro representa a las firmas digitales no firmados.
                        </p>
                    </div>
                </div>

            </div>

            <%-- </div>--%>

            <%--grilla firma digital--%>
            <%-- <div class="row">--%>

            <%--grilla consentimientos--%>
            <div class="col s12 m12 l12">

                <%--<div id="main">--%>
                <%--<div class="container">--%>

                <div class="section-data-tables">

                    <!-- DataTables example -->
                    <%-- <div class="row">--%>

                    <div id="button-trigger2" class="card">
                        <div class="card-content">
                            <span class="card-title blue-text text-darken-3">VCTP + ADN de RPP & IFP

                                <div class="dropdown float-right">
                                    <a class="dropdown-trigger" href="#" data-target="2">
                                        <i class="material-icons">more_vert</i>
                                    </a>
                                    <ul id="2" class="dropdown-content" tabindex="2" style="">
                                        <li class="consentimientoRP" tabindex="2">
                                            <a href="#">
                                                <i class="material-icons">cloud_download</i>
                                                <span class="menu-item">Trazabilidad de envíos</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                            </span>
                            <%--<h4 class="card-title">VCTP + ADN de RPP & IFP</h4>--%>

                            <%-- <div class="row">--%>

                            <table id="table_Consentimiento_RP" class="display nowrap">
                                <thead>
                                    <tr>
                                        <th>Identificación</th>
                                        <th>Nombres y apellidos</th>
                                        <th>CUSPP</th>
                                        <th>Fec. primer env.</th>
                                        <th>Fec. último env.</th>
                                        <th>Firmado</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </thead>

                                <tbody>
                                </tbody>

                                <tfoot>
                                    <tr>
                                        <th>Identificación</th>
                                        <th>Nombres y apellidos</th>
                                        <th>CUSPP</th>
                                        <th>Fec. primer env.</th>
                                        <th>Fec. último env.</th>
                                        <th>Firmado</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </tfoot>
                            </table>

                            <%--</div>--%>
                        </div>
                    </div>

                </div>
                <%--</div>--%>
            </div>

            <%--</div>--%>

            <%--<div class="row">--%>

            <%--grilla firma digital--%>
            <div class="col s12 m12 l12">

                <%--<div class="container">--%>

                <div class="section-data-tables">

                    <!-- DataTables example -->
                    <%--<div class="row">--%>

                    <div id="button-trigger4" class="card">
                        <div class="card-content">
                            <span class="card-title blue-text text-darken-3">Firma Digital de RPP & IFP

                                <div class="dropdown float-right">
                                    <a class="dropdown-trigger" href="#" data-target="4">
                                        <i class="material-icons">more_vert</i>
                                    </a>
                                    <ul id="4" class="dropdown-content" tabindex="4" style="">
                                        <li class="firmadigitalRP" tabindex="4">
                                            <a href="#">
                                                <i class="material-icons">cloud_download</i>
                                                <span class="menu-item">Trazabilidad de envíos</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                            </span>
                            <%--<h4 class="card-title">Firma Digital de RPP & IFP</h4>--%>
                            <%--<div class="row">--%>

                            <table id="table_Firma_Digital_RP" class="display nowrap">
                                <thead>
                                    <tr>
                                        <th>Solicitud</th>
                                        <th>Nombres y apellidos</th>
                                        <th>Identificación</th>
                                        <th>CUSPP</th>
                                        <th>Fec. primer env.</th>
                                        <th>Fec. último env.</th>
                                        <th>Firmado</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </thead>

                                <tbody>
                                </tbody>

                                <tfoot>
                                    <tr>
                                        <th>Solicitud</th>
                                        <th>Nombres y apellidos</th>
                                        <th>Identificación</th>
                                        <th>CUSPP</th>
                                        <th>Fec. primer env.</th>
                                        <th>Fec. último env.</th>
                                        <th>Firmado</th>
                                        <th>Nro. env.</th>
                                        <th>Agente</th>
                                        <th>Supervisor</th>
                                        <th>Jefe</th>
                                    </tr>
                                </tfoot>
                            </table>

                            <%--</div>--%>
                        </div>
                    </div>

                    <%-- </div>--%>
                </div>

                <%--</div>--%>
            </div>

            <%--</div>--%>

            <%--grilla poliza--%>
            <%--<div class="row">--%>

            <%--</div>--%>

            <%--<div class="row">--%>
            <%--grilla poliza--%>
            <div class="col s12 m12 l12">

                <%--<div class="container">--%>

                <div class="section-data-tables">

                    <!-- DataTables example -->
                    <%--<div class="row">--%>

                    <div id="button-trigger6" class="card">
                        <div class="card-content">
                            <span class="card-title blue-text text-darken-3">Póliza Electrónica de RPP & IFP

                                <div class="dropdown float-right">
                                    <a class="dropdown-trigger" href="#" data-target="6">
                                        <i class="material-icons">more_vert</i>
                                    </a>
                                    <ul id="6" class="dropdown-content" tabindex="6" style="">
                                        <li class="polizaRP" tabindex="6">
                                            <a href="#">
                                                <i class="material-icons">cloud_download</i>
                                                <span class="menu-item">Trazabilidad de envíos</span>
                                            </a>
                                        </li>
                                    </ul>
                                </div>

                            </span>
                            <%--<h4 class="card-title">Póliza Electrónica de RPP & IFP</h4>--%>
                            <%--<div class="row">--%>

                                <table id="table_Poliza_RP" class="display nowrap">
                                    <thead>
                                        <tr>
                                            <th>Solicitud</th>
                                            <th>Nombres y apellidos</th>
                                            <th>Identificación</th>
                                            <th>CUSPP</th>
                                            <th>Póliza</th>
                                            <th>Fecha de envío</th>
                                            <th>Nro. env.</th>
                                            <th>Agente</th>
                                            <th>Supervisor</th>
                                            <th>Jefe</th>
                                        </tr>
                                    </thead>

                                    <tbody>
                                    </tbody>

                                    <tfoot>
                                        <tr>
                                            <th>Solicitud</th>
                                            <th>Nombres y apellidos</th>
                                            <th>Identificación</th>
                                            <th>CUSPP</th>
                                            <th>Póliza</th>
                                            <th>Fecha de envío</th>
                                            <th>Nro. env.</th>
                                            <th>Agente</th>
                                            <th>Supervisor</th>
                                            <th>Jefe</th>
                                        </tr>
                                    </tfoot>
                                </table>

                           <%-- </div>--%>
                        </div>
                    </div>

                    <%--</div>--%>
                </div>

                <%--</div>--%>
            </div>

            <%-- </div>--%>
        </div>

        <%--</div>--%>
    </div>
    <%--row--%>

    <%--busqueda textbox--%>
    <%--<div class="row">

                <div class="col s8">

                    <div class="content-area">
                        <div class="app-wrapper">
                            <div class="datatable-search">
                                <i class="material-icons mr-2 search-icon">search</i>
                                <input type="text" placeholder="Buscar un agente" class="app-filter" />
                            </div>
                        </div>
                    </div>

                </div>

            </div>--%>

    <%--<!-- Indeterminate Linear -->
            <div class="row">
                <div class="col s12">
                    <div id="indeterminate-linear" class="card card-tabs">
                        <div class="card-content">
                            <div class="card-title">
                                <div class="row">
                                    <div class="col s12 m6 l10">
                                        <h4 class="card-title">Indeterminate Linear</h4>
                                    </div>
                                </div>
                            </div>
                            <div id="view-indeterminate-linear">
                                <div class="row">
                                    <div class="col s12">
                                        <div class="progress">
                                            <div class="indeterminate"></div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div id="html-indeterminate-linear">
                                <pre><code class="language-markup">
                             </code></pre>
                            </div>
                        </div>
                    </div>
                </div>
            </div>--%>

    <%--</div>--%>




    <%--Ventanas Modales--%>
    <div style="display: none">

        <%--Inicio Modal Cuadro de mensajes--%>
        <div id="ModalCuadroMensaje">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCMIcono" style="width: 40px; height: 40px"></td>
                        <td id="MCMContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCMContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MCMEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCMEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCMEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCMMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MCMBotonera" align="center">
                <a id="MCMAceptar" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de mensajes--%>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="Scripts" runat="server">

    <script src="../Estilos/app-assets/js/scripts/data-tables.min.js"></script>
    <script src="../Estilos/app-assets/vendors/data-tables/js/jquery.dataTables.min.js"></script>
    <script src="../Estilos/date-range-picker/moment.min.js"></script>
    <script src="../Estilos/date-range-picker/jquery.daterangepicker.js"></script>
    <script src="../Scripts/CWRV.controles.dashboard.js"></script>
    <script src="../Scripts/CWRV.eventos.dashboard.js"></script>
    <script src="../Estilos/app-assets/vendors/chartist-js/chartist.min.js"></script>

</asp:Content>
