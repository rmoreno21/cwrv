<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CompararCotizacionPlus.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Reportes.CompararCotizacionPlus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/jquery.numeric.js")%>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.simuladores.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

<div id="div" align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">
        <h1 class="simple" style="width:180px">Búsqueda de afiliados</h1>

        <asp:HiddenField ID="HBusAfiNroSolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HBusAfiCUSPP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="RepCotPlusBusAfiDatosCargados" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="PerBusAfiBuscar" runat="server" ClientIDMode="Static" />
        
        <asp:HiddenField ID="HCompara" runat="server" ClientIDMode="Static" />

        <asp:Panel ID="FormularioBusqueda" runat="server" ClientIDMode="Static">
            <div class="formLinea">
                <label id="LabBusAfiNroSolicitud" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                <asp:TextBox ID="BusAfiNroSolicitud" runat="server" CssClass="formTextbox" style="text-transform:uppercase" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                <label id="LabBusAfiCUSPP" class="formLabel formLabel2Der">CUSPP:</label>
                <asp:TextBox ID="BusAfiCUSPP" runat="server" CssClass="formTextbox alfanumerico" style="text-transform:uppercase" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>

                <asp:HyperLink ID="BusAfiExaminarSolicitud" CssClass="boton darkblue sharp" style="width:22px;height:22px;position:relative;margin-left:5px" runat="server" ClientIDMode="Static">...</asp:HyperLink>
                <asp:HiddenField ID="PerBusAfiExaminarSolicitud" runat="server" ClientIDMode="Static" />
            </div>

            <div class="formLinea" align="center">
                <asp:HyperLink ID="RepCotPlusBusAfiBuscar" NavigateUrl="#" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
                
                <%--<asp:Button ID="SimBusAfiBuscar" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Buscar" ClientIDMode="Static" />--%>
            </div>
        </asp:Panel>


        <div class="formLinea" id ="divMaximo" style="display:none" >
            <label id="LabModSolMaximo_RP" for="ModSolMaximo_RP" class="formLabel formLabel2Izq">Nro. Cotizaciones*:</label>
            <asp:DropDownList ID="ModSolMaximo_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
            </asp:DropDownList>
        </div>

        <p></p>

        <div id="TablaSolicitudesCargando" align="center" style="display:none">
            <asp:Image ID="icoTablaSolicitudesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando solicitudes, espere por favor...</span>
        </div>
        <div id="TablaSolicitudesContenedor" style="display:none"></div>
        <div id="TablaSolicitudesError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes. <a id="TablaReporteSolicitudesPlusReintentar">Intentar de nuevo</a>.</div>

        <br />
   
        <div id="divBotonera" align="left" class="pieBoton">
            <div class="formLinea" align="center">
                <a id="btnEnviarExcel"  style="width:120px;height:22px" class="boton darkblue sharp">Generar Excel</a>
            </div>
        </div>

    


        <asp:HiddenField ID="IdSimulador" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="CUSPP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HRutaImagenSimulada" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCorrelativoSolicitud" runat="server" ClientIDMode="Static" />

</div>
   


<%--Ventanas Modales--%>

    <div style="display:none">
        

        <%--Inicio Modal Búsqueda de Afiliados--%>
        <div id="ModalBusquedaAfiliados" title="Búsqueda de afiliados">
            <div class="formLinea">
                <label id="LabModBusAfiApellidoPaterno" class="formLabel formLabel2Izq">Apellido Paterno:</label>
                <asp:TextBox ID="ModBusAfiApellidoPaterno" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static"></asp:TextBox>

                <label id="LabModBusAfiApellidoMaterno" class="formLabel formLabel2Der">Apellido Materno:</label>
                <asp:TextBox ID="ModBusAfiApellidoMaterno" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static"></asp:TextBox>
            </div>

            <div class="formLinea">
                <label id="LabModBusAfiNombres" class="formLabel formLabel2Izq">Nombres:</label>
                <asp:TextBox ID="ModBusAfiNombres" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static"></asp:TextBox>
            </div>

            <div class="formLinea" align="center">
                <asp:HyperLink ID="ModBusAfiBuscar" NavigateUrl="#" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
                <a id="ModBusAfiCancelar" href="#" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
            </div>

            <br />

            <div id="ModBusAfiCargando" align="center" style="display:none">
                <asp:Image ID="icoModBusAfiCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Buscando afiliados, espere por favor...</span>
            </div>
            <div id="TablaAfiliadosError" align="center" style="display:none">
                <div class="grilla_error" align="left" style="width:365px;">
                    No se ha podido cargar la tabla de afiliados. <a id="TablaAfiliadosReintentar" href="#">Intentar de nuevo</a>.
                </div>
            </div>

            <div id="ModBusAfiTablaAfiliados" runat="server" style="display:none" clientidmode="Static"></div>

            <asp:HiddenField ID="TabAfiliadosIndicePagina" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosTamanhoPagina" runat="server" ClientIDMode="Static" Value="10" />
            <asp:HiddenField ID="TabAfiliadosColumnaOrdenar" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosDireccionOrdenar" runat="server" ClientIDMode="Static" Value="A" />
        </div>
        <%--Fin Modal Búsqueda de Afiliados--%>

        <%--Inicio Modal Cuadro de mensajes--%>
        <div id="ModalCuadroMensaje">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCMIcono" style="width:40px;height:40px"></td>
                        <td id="MCMContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCMContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
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
                <a id="MCMAceptar" class="boton darkblue sharp" style="width:80px">Aceptar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de mensajes--%>


        <%--Inicio Modal Cotizando--%>
        <div id="ModalCotizando">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCIcono" style="width:40px;height:40px"></td>
                        <td id="MCContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
                                <asp:HiddenField ID="MCEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <%--Fin Modal Cotizando--%>

        

    </div>

    
    
        
    
    
        

</asp:Content>
