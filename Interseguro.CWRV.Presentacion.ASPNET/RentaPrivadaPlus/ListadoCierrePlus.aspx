<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="ListadoCierrePlus.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus.ListadoCierrePlus" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.rpppoliza.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.rpppoliza.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

    <script type="text/javascript">

        $(document).ready(function () {

            //<INIGTI_7012>
//            CargarTablaSolicitudesCierre_RP();
            //<FINGTI_7012>
        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />


     <asp:Panel ID="BusquedaAfiliados_RP" runat="server" ClientIDMode="Static" align="left" style="width:860px;padding:20px;background:rgba(255, 255, 255, 0.8); border:1px solid #00466e">
        <h1 class="simple" style="width:276px">Cotizaciones de Rentas Particulares</h1>
        
        <fieldset>
            <legend>Búsqueda de afiliados</legend>
            <div class="formLinea">
                <label id="LabBusAfiNroSolicitud_RP" for="BusAfiNroSolicitud_RP" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                <asp:TextBox ID="BusAfiNroSolicitud_RP" runat="server" CssClass="formTextbox" style="text-transform:uppercase" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                <label id="LabBusAfiCUSPP_RP" for="BusAfiCUSPP_RP" class="formLabel formLabel2Der">CUSPP:</label>
                <asp:TextBox ID="BusAfiCUSPP_RP" runat="server" CssClass="formTextbox alfanumerico" style="text-transform:uppercase" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
            
                <asp:HyperLink ID="BusAfiExaminarSolicitud_RP" CssClass="boton darkblue sharp" style="width:22px;height:22px;position:relative;margin-left:5px" runat="server" ClientIDMode="Static">...</asp:HyperLink>
                <asp:HiddenField ID="PerBusAfiExaminarSolicitud_RP" runat="server" ClientIDMode="Static" />
            </div>

            <asp:Panel ID="LineaBuscarAfiliado_RP" runat="server" ClientIDMode="Static" CssClass="formLinea" align="center">
                <asp:Button ID="BusAfiBuscar_RP" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Buscar" onclick="BusAfiBuscar_RP_Click" 
                    ClientIDMode="Static" />
                <asp:HiddenField ID="PerBusAfiBuscar_RP" runat="server" ClientIDMode="Static" />
            </asp:Panel>
        </fieldset>
    
        <br />

        <fieldset>
            <legend>Listado de Solicitudes</legend>

            
            <div id="TablaSolicitudesCargando_RP" align="center" style="display:none">
                <asp:Image ID="icoTablaSolicitudesCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando solicitudes, espere por favor...</span>
            </div>
            <div id="TablaSolicitudesContenedor_RP" style="display:none"></div>
            <div id="TablaSolicitudesError_RP" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesReintentar_RP">Intentar de nuevo</a>.</div>

        
        </fieldset>
    </asp:Panel>

    <br />

    <%--Ventanas Modales--%> 
    <%--Ini Ya esta en controles--%>
    <div style="display:none">

        <%--Inicio Modal Búsqueda de Afiliados--%>
        <div id="ModalBusquedaAfiliados_RP" title="Búsqueda de afiliados">
            <div class="formLinea">
                <label id="LabModBusAfiApellidoPaterno_RP" for="ModBusAfiApellidoPaterno" class="formLabel formLabel2Izq">Apellido Paterno:</label>
                <asp:TextBox ID="ModBusAfiApellidoPaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static"></asp:TextBox>

                <label id="LabModBusAfiApellidoMaterno_RP" for="ModBusAfiApellidoMaterno" class="formLabel formLabel2Der">Apellido Materno:</label>
                <asp:TextBox ID="ModBusAfiApellidoMaterno_RP" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static"></asp:TextBox>
            </div>

            <div class="formLinea">
                <label id="LabModBusAfiNombres_RP" for="ModBusAfiNombres" class="formLabel formLabel2Izq">Nombres:</label>
                <asp:TextBox ID="ModBusAfiNombres_RP" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static"></asp:TextBox>
            </div>

            <div class="formLinea" align="center">
                <asp:HyperLink ID="ModBusAfiBuscar_RP" NavigateUrl="#" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
                <a id="ModBusAfiCancelar_RP" href="#" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
            </div>

            <br />

            <div id="ModBusAfiCargando_RP" align="center" style="display:none">
                <asp:Image ID="icoModBusAfiCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Buscando afiliados, espere por favor...</span>
            </div>
            <div id="TablaAfiliadosError_RP" align="center" style="display:none">
                <div class="grilla_error" align="left" style="width:365px;">
                    No se ha podido cargar la tabla de afiliados. <a id="TablaAfiliadosReintentar_RP" href="#">Intentar de nuevo</a>.
                </div>
            </div>

            <div id="ModBusAfiTablaAfiliados_RP" runat="server" style="display:none" clientidmode="Static"></div>

            <asp:HiddenField ID="TabAfiliadosIndicePagina_RP" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosTamanhoPagina_RP" runat="server" ClientIDMode="Static" Value="10" />
            <asp:HiddenField ID="TabAfiliadosColumnaOrdenar_RP" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosDireccionOrdenar_RP" runat="server" ClientIDMode="Static" Value="A" />

            <%--Control de fecha: <asp:TextBox ID="EjemploFecha" runat="server" CssClass="formCalendar" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>--%>
        </div>
        <%--Fin Modal Búsqueda de Afiliados--%>
        <%--Fin Ya esta en controles--%>


        <%--Inicio Modal Envío de Correo--%>
        <div id="ModalEnvioCorreo" title="Enviar Correo Electrónico">
            <div id="ModEnvCorCargando" class="modalCargandoContenido" style="width:526px;height:350px"></div>
            <div id="ModEnvCorContenido" class="modalContenido">
                <asp:HiddenField ID="ModEnvCorModo" runat="server" ClientIDMode="Static" />

                <div class="formLinea">
                    <label id="LabModEnvCorDe" class="formLabel formLabel2Izq">De:</label>
                    <asp:TextBox ID="ModEnvCorDe" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorPara" class="formLabel formLabel2Izq">Para:</label>
                    <asp:TextBox ID="ModEnvCorPara" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAsunto" class="formLabel formLabel2Izq">Asunto:</label>
                    <asp:TextBox ID="ModEnvCorAsunto" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAdjunto" class="formLabel formLabel2Izq">Adjunto:</label>
                    <%--<asp:Image ID="ModEnvCorIconoPDF" ImageUrl="~/Imagenes/iconoPDF.png" style="margin-left:-188px" runat="server" />--%>
                    <label id="ModEnvCorAdjunto" class="formLabelAdjunto">Cotizacion.pdf (189kb)</label>
                </div>

                <div class="formLinea" style="height:210px">
                    <%--<label id="LabModCorMensaje" class="formLabel formLabel2Izq">Mensaje:</label>--%>
                    <asp:TextBox ID="ModEnvCorMensaje" runat="server" CssClass="formTextbox" style="margin-left:0px;height:200px" Width="560" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModEnvCorEnviar" style="width:80px;height:22px" class="boton darkblue sharp">Enviar</a>
                    <a id="ModEnvCorCancelar" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
                </div>
            </div>
        </div>
        <%--Fin Modal Envío de Correo--%>


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

        <%--Inicio Modal Cuadro de advertencia--%>
        <div id="ModalCuadroAdvertencia">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCAIcono" style="width:40px;height:40px"></td>
                        <td id="MCAContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCAContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
                                <asp:HiddenField ID="MCAEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCAEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCAEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCAMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MCABotonera" align="center">
                <a id="MCAAceptar_Plus" class="boton darkblue sharp" style="width:80px">Aceptar</a>
                <a id="MCACancelar_Plus" class="boton darkblue sharp" style="width:80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>

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
