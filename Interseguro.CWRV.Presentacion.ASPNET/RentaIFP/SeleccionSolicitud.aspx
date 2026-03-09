<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="SeleccionSolicitud.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.SeleccionSolicitud" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/lightbox.css")%>" />
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/google-drive.css")%>" />

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiParametro.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiCotizadorIFP.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/ajaxfileupload.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
        <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.google.drive.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

    <%--<script type="text/javascript" src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.4/jquery.min.js"></script>--%>
    <%--<script type="text/javascript" src="~/Scripts/google-drive.js"></script>--%>

    <%--<script async defer src="https://apis.google.com/js/api.js" 
	  onload="this.onload=function(){};handleClientLoad()" 
	  onreadystatechange="if (this.readyState === 'complete') this.onload()">
	</script>--%>

    <script src="https://apis.google.com/js/api.js" type="text/javascript">
    </script>

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/lightbox.min.js")%>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/upload.js")%>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/seleccionSolicitud.js")%>"></script>


</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="NuevaCotizacionRP" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">

        <h1 class="simple" style="width: 299px">Cerrar Cotización Ingreso Flexible Plus</h1>

        <asp:HiddenField ID="ModSolModo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModSolFecCotizacion_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAFP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HConyuge" runat="server" ClientIDMode="Static" Value="FALSE" />
        <asp:HiddenField ID="HSeleccionada" runat="server" ClientIDMode="Static" Value="N" />
        <asp:HiddenField ID="HDiasVigencia" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HCopia" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="HBloqueo" runat="server" ClientIDMode="Static" Value="TRUE" />
        <asp:HiddenField ID="HEstado" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HNroSolicitud" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HEstadoPlaft" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HPEP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HFirmado" runat="server" ClientIDMode="Static" />

        <asp:HiddenField ID="HSolicitudSerializado" runat="server" ClientIDMode="Static" Value="0" />

        <div id="ModSolContenido" class="modalContenido">

            <div id="ManSolPestanhas">

                <ul>
                    <li id="ManSolPes1" data-pestanha="1"><a href="#">Cotización</a></li>
                    <%--<li id="ManSolPes2" data-pestanha="2"><a href="#">Beneficiarios <span id="ManSolNumBeneficiarios_RP"></span></a><span id="ManSolNumBeneficiariosCargando_RP" class="pestanhaCargando"></span></li>--%>
                </ul>

                <asp:HiddenField ID="ManSolPestanhaActiva_RP" runat="server" Value="1" ClientIDMode="Static" />
                <asp:HiddenField ID="ManSolTipoSolicitud_RP" runat="server" ClientIDMode="Static" />

                <div style="visibility: hidden">
                    <asp:DropDownList ID="ModSolMonedaPrimaUnica_IFP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static"></asp:DropDownList>
                </div>

                <div id="ManSolPestanha1" align="left" style="width: 820px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">

                    <div class="formLinea">
                        <label id="LabModSolNroSolicitud_IFP" for="ModSolNroSolicitud_IFP" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                        <asp:Label ID="ModSolNroSolicitud_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

                        <label id="LabModSolDNI_IFP" for="ModSolDNI_IFP" class="formLabel formLabel2Der">DNI:</label>
                        <asp:Label ID="ModSolDNI_IFP" runat="server" CssClass="formDato" Width="193" ClientIDMode="Static" />
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolNombres_IFP" for="ModSolNombres_IFP" class="formLabel formLabel2Izq">Nombres:</label>
                        <asp:Label ID="ModSolNombres_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

                        <label id="LabModSolApellidos_IFP" for="ModSolApellidos_IFP" class="formLabel formLabel2Der">Apellidos:</label>
                        <asp:Label ID="ModSolApellidos_IFP" runat="server" CssClass="formDato" Width="193" ClientIDMode="Static"></asp:Label>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolMonedaPrimaUnica_IFP" for="ModSolMonedaPrimaUnica_IFP" class="formLabel formLabel2Izq">Moneda de Prima Única*:</label>
                        <asp:Label ID="ModSolMonedaPrimaUnicaText_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

                        <label id="LabModSolPrimaUnica_IFP" for="ModSolPrimaUnica_IFP" class="formLabel formLabel2Der">Prima Única*:</label>
                        <asp:Label ID="ModSolPrimaUnica_IFP" runat="server" CssClass="formDato" Width="174" ClientIDMode="Static"></asp:Label>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolFechaCotizacion_IFP" for="ModSolFechaCotizacion_IFP" class="formLabel formLabel2Izq">Fecha de Cotización*:</label>
                        <asp:Label ID="ModSolFechaCotizacion_IFP" runat="server" CssClass="formDato" Width="174" ClientIDMode="Static"></asp:Label>

                        <asp:Label ID="LabModSolFechaVigencia_IFP" AssociatedControlID="ModSolFechaVigencia_IFP" runat="server" ClientIDMode="Static" for="ModSolFechaVigencia_RP" class="formLabel formLabel2Der">Vigente Hasta*:</asp:Label>
                        <asp:Label ID="ModSolFechaVigencia_IFP" runat="server" CssClass="formDato" Width="174" ClientIDMode="Static"></asp:Label>
                    </div>

                    <asp:Panel ID="Panel1" runat="server" CssClass="formLinea">
                        <label id="LabModSolFechaDevengue_IFP" for="ModSolFechaDevengue_IFP" class="formLabel formLabel2Izq">Fecha de Devengue*:</label>
                        <asp:Label ID="ModSolFechaDevengue_IFP" runat="server" CssClass="formDato" Width="174" ClientIDMode="Static"></asp:Label>

                        <%--<label id="LabModSolPlan_IFP" for="ModSolPlan_IFP" class="formLabel formLabel2Der">Agregar Plan:</label>
						<asp:Label ID="ModSolPlan_IFP" runat="server" CssClass="formDato" Width="199" ClientIDMode="Static"></asp:Label>--%>

                        <%--<a id="ModSolAgregarPlan_IFP" href="javascript:void(0);" style="width: 20px; height: 22px; margin-left: 3px;" class="botonDeshabilitado gris gris_sharp">+</a>--%>
                    </asp:Panel>

                    <div class="formLinea">
                        <asp:Panel ID="ModSolTipoCambioPanel" runat="server" Visible="true" ClientIDMode="Static">
                            <label id="LabModSolTipoCambio_IFP" for="ModSolTipoCambio_IFP" class="formLabel formLabel2Izq">Tipo Cambio:</label>
                            <asp:Label ID="ModSolTipoCambio_IFP" runat="server" CssClass="formDato" Width="174" ClientIDMode="Static"></asp:Label>
                        </asp:Panel>
                    </div>

                    <%--<div class="formLinea" id="ModSolLineaNumPoliza_RP" runat="server">
						<label id="LabModSolNumPoliza_RP" for="ModSolNumPoliza_RP" class="formLabel formLabel2Izq">Nro. Póliza:</label>
						<asp:Label ID="ModSolNumPoliza_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
										
						<label id="LabModSolEstadoPoliza_RP"  for="ModSolEstadoPoliza_RP" class="formLabel formLabel2Der">Estado Póliza:</label>
						<asp:Label ID="ModSolEstadoPoliza_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
					</div>--%>

                    <asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="True" style="display: block;" ClientIDMode="Static">
                        <asp:Label ID="LabMensaje" runat="server" ClientIDMode="Static" />
                    </asp:Panel>

                    <div id="TablaCotizacionesCargando_RP" align="center" style="height: 50px; padding: 82px 0">
                        <asp:Image ID="icoTablaCotizacionesCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando cotizaciones, espere por favor...</span>
                    </div>

                    <div id="TablaCotizacionesContenedor_IFP_Plan1" style="display: none"></div>
                    <br />
                    <div id="TablaCotizacionesContenedor_IFP_Plan2" style="display: none"></div>
                    <br />
                    <div id="TablaCotizacionesContenedor_IFP_Plan3" style="display: none"></div>

                    <div class="formLinea" align="center">
                        <a id="ModSolAceptarCierre_RP" href="javascript:void(0);" style="width: 180px; height: 22px" class="boton darkblue sharp">Seleccionar Solicitud</a>
                        <a id="ModImprimirDocumentosIFP" style="width: 180px; height: 22px" class="boton darkblue sharp" >Imprimir Documentos</a>
                        <asp:Button ID="ModSolAgregarArchivos" Style="width: 182px; height: 24px" class="boton darkblue sharp" runat="server" Text="Cargar Archivos" Visible="true" ClientIDMode="Static" />
                        <a id="ModSolCancelar_RP" href="javascript:void(0);" style="width: 180px; height: 22px" class="boton darkblue sharp">Cancelar</a>
                        <asp:Button ID="ModSolSiguiente_RP" Style="display:none" runat="server" Text="Siguiente cierre" PostBackUrl="~/RentaIFP/GrupoFamiliarAfiliadoCierre.aspx" />
                        <%--<asp:Button ID="ModSolReporte_RP" Style="width: 182px; height: 24px" class="boton darkblue sharp" runat="server" Text="Imprimir Formatos" OnClick="ModSolReporte_RP_Click" Visible="false" />--%>
                    </div>

                    <asp:Panel ID="TabCotizacionesLeyenda_RP" runat="server" ClientIDMode="Static" Visible="true">
                        <fieldset>
                            <legend>Leyenda</legend>
                            <div class="formLinea" style="font-family: Calibri; font-size: 13px; color: #0060A9; display: inline">
                                <span class="grilla_error_tra" style="width: 25px !important;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span><span>&nbsp;&nbsp;Cotización no alcanza el mínimo requerido, por lo cual no se simula.</span>
                            </div>
                        </fieldset>
                    </asp:Panel>

                    <div>
                        <div id="drive-box" class="hide">
                            <div id="drive-menu">
                                <div id="button-reload" title="Refrescar"></div>
                                <div id="button-upload" title="Cargar a Google Drive" class="button-opt"></div>
                            </div>
                            <div id="drive-content"></div>
                            <div id="error-message" class="flash hidden"></div>
                            <div id="status-message" class="flash hidden"></div>
                        </div>
                        <input type="file" id="fUpload" name="fUpload" class="hide" multiple accept=".jpg, .jpeg, .pdf, .png" length="1024" />
                    </div>

                    <div class="formLinea" id="divCargar" align="center" style="display: none">
                        <asp:Button ClientIDMode="Static" ID="ModSolEnviarEvaluacion" Style="width: 182px; height: 24px" class="boton darkblue sharp" runat="server" Text="Enviar a Evaluación" Visible="true" />
                        <asp:Button ClientIDMode="Static" ID="CorregirDocumentos" Style="width: 182px; height: 24px" class="boton darkblue sharp" runat="server" Text="Corregir Documentos" Visible="true" />
                    </div>
                </div>
            </div>
        </div>

    </asp:Panel>

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

        <%--Inicio Enviar--%>
        <div id="ModalCuadroEnviar">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCAIconoEnviar" style="width: 40px; height: 40px"></td>
                        <td id="MCAContenedorMensajeEnviar" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCAContenedorEnviar" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MCAEstadoEnviar" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCAEstadoIconoEnviar" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCAEstadoTituloEnviar" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCAMensajeEnviar" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MCABotoneraEnviar" align="center">
                <a id="MCAAceptarCierreEnviar_Plus" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MCACancelarEnviar_Plus" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <%--Fin Enviar--%>

        <%--Inicio Modal Cuadro de advertencia--%>
        <div id="ModalCuadroAdvertencia">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCAIcono" style="width: 40px; height: 40px"></td>
                        <td id="MCAContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCAContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
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
                <a id="MCAAceptarCierre_Plus" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MCACancelar_Plus" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>

        <%--Inicio Modal Cuadro de Eliminar Archivo--%>
        <div id="ModalCuadroEliminarArchivo">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCAIconoEliminarArchivo" style="width: 40px; height: 40px"></td>
                        <td id="MCAContenedorMensajeEliminarArchivo" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCAContenedorEliminarArchivo" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MCAEstadoEliminarArchivo" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCAEstadoIconoEliminarArchivo" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCAEstadoTituloEliminarArchivo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCAMensajeEliminarArchivo" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MCABotoneraEliminarArchivo" align="center">
                <a id="MCAAceptarCierre_Plaft" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MCACancelar_Plaft" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de advertencia Eliminar Archivo--%>

        <%--Inicio Modal Cotizando--%>
        <div id="ModalCotizando">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCIcono" style="width: 40px; height: 40px"></td>
                        <td id="MCContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
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

    <div id="float-box-text" class="float-box">
        <div class="info-form">
            <%--<div class="close-x"><img id="imgCloseText" class="imgClose" src="images/button_close.png" alt="close" /></div>--%>
            <div class="close-x">
                <img id="imgCloseText" class="imgClose" src="<%=ResolveUrl("~/Estilos/images/button_close.png")%>" alt="close" />
            </div>
            <h3 class="clear">Text Content</h3>
            <div id="text-content"></div>
            <button id="btnCloseText" value="Close" class="button btnClose">Close</button>
        </div>
    </div>

</asp:Content>
