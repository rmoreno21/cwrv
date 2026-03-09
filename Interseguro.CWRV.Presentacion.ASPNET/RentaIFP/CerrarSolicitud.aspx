<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CerrarSolicitud.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.CerrarSolicitud" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiCotizadorIFP.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/CerrarSolicitud.eventos.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/RentaIFP/CerrarSolicitud.controles.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <asp:Panel ID="NuevaCotizacionRP" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 310px">Cierre de Póliza Ingreso Flexible Plus</h1>
        <asp:HiddenField ID="ModSolModo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="ModSolFecCotizacion_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAFP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HConyuge" runat="server" ClientIDMode="Static" Value="FALSE" />
        <asp:HiddenField ID="HSeleccionada" runat="server" ClientIDMode="Static" Value="N" />
        <asp:HiddenField ID="HDiasVigencia" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HEstado" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HCopia" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="HBloqueo" runat="server" ClientIDMode="Static" Value="TRUE" />
        <asp:HiddenField ID="HCodCanalDistribucion" runat="server" ClientIDMode="Static" />

        <div id="ModSolContenido" class="modalContenido">
            <div id="ManSolPestanhas">
                <ul>
                    <li id="ManSolPes1" data-pestanha="1"><a href="#">Cotización</a></li>
                    <li id="ManSolPes2" data-pestanha="2"><a href="#">Beneficiarios <span id="ManSolNumBeneficiarios_RP"></span></a><span id="ManSolNumBeneficiariosCargando_RP" class="pestanhaCargando"></span></li>
                </ul>

                <asp:HiddenField ID="ManSolPestanhaActiva_RP" runat="server" Value="1" ClientIDMode="Static" />
                <asp:HiddenField ID="ManSolTipoSolicitud_RP" runat="server" ClientIDMode="Static" />
                <div style="visibility: hidden">
                    <asp:DropDownList ID="ModSolMonedaPrimaUnica_IFP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                    </asp:DropDownList>

                    <%--<asp:DropDownList ID="ModSolTipoPlan_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static"  >
                        </asp:DropDownList>

                    <asp:DropDownList ID="ModSolTemporalidad_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static" >
                    </asp:DropDownList>--%>
                </div>

                <div id="ManSolPestanha1" align="left" style="width: 820px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
                    <%--<div class="formLinea">
                        <label id="LabModSolNroSolicitud_RP" for="ModSolNroSolicitud_RP" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                        <asp:Label id ="ModSolNroSolicitud_RP" runat="server"  CssClass="formDato" Width="180" ClientIDMode="Static" />

                        <asp:Panel ID="ModSolTipoCambioPanel" runat="server" Visible="true" ClientIDMode="Static">
                            <label id="LabModSolTipoCambio" for="ModSolTipoCambio" class="formLabel formLabel2Der">Tipo Cambio:</label>
                            <asp:Label id ="ModSolTipoCambio" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                        </asp:Panel>    
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolMonedaPrimaUnicaText_RP" for="ModSolMonedaPrimaUnicaText_RP" class="formLabel formLabel2Izq">Moneda de Prima Única:</label>
                        <asp:Label id ="ModSolMonedaPrimaUnicaText_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                        
                        <label id="LabModSolPrimaUnica_RP" for="ModSolPrimaUnica_RP" class="formLabel formLabel2Der">Prima Única*:</label>
                        <asp:Label id ="ModSolPrimaUnica_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolFechaCotizacion_RP" for="ModSolFechaCotizacion_RP" class="formLabel formLabel2Izq">Fecha de Cotización:</label>
                        <asp:Label id ="ModSolFechaCotizacion_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

                        <label id="LabModSolFechaDevengue_RP" for="ModSolFechaDevengue_RP" class="formLabel formLabel2Der">Fecha de Devengue:</label>
                        <asp:Label id ="ModSolFechaDevengue_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolTipoPlanText_RP" for="ModSolTipoPlanText_RP" class="formLabel formLabel2Izq">Tipo de Plan:</label>
                        <asp:Label id ="ModSolTipoPlanText_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                        
                        <label id="LabModSolTemporalidadText_RP" for="ModSolTemporalidadText_RP" class="formLabel formLabel2Der">Temporalidad:</label>
                        <asp:Label id ="ModSolTemporalidadText_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                    </div>

                    <asp:Panel ID="Panel1" runat="server" CssClass="formLinea">
                        <asp:Panel ID="LabModSolLineaACOMDCOM_RP" runat="server">
                            <label id="LabModSolDCOM_RP" for="ModSolDCOM_RP" class="formLabel formLabel2Izq">Porcentaje D:</label>
                            <asp:Label id ="ModSolDCOM_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

                        </asp:Panel>
                        
                        <asp:Label id="LabModSolFechaVigencia_RP"  AssociatedControlID="LabModSolFechaVigencia_RP" runat="server" ClientIDMode="Static" for="ModSolFechaVigencia_RP" class="formLabel formLabel2Der">Vigente Hasta*:</asp:Label>
                        <asp:Label id ="ModSolFechaVigencia_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                        
                    </asp:Panel>

                    <div class="formLinea" id="ModSolLineaNumPoliza_RP" runat="server">
                        <label id="LabModSolNumPoliza_RP" for="ModSolNumPoliza_RP" class="formLabel formLabel2Izq">Nro. Póliza:</label>
                        <asp:Label id ="ModSolNumPoliza_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

                    
                        <label id="LabModSolEstadoPoliza_RP" for="ModSolEstadoPoliza_RP" class="formLabel formLabel2Der">Estado Póliza:</label>
                        <asp:Label id ="ModSolEstadoPoliza_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                    </div>
                    
                        
                    <asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false" style="padding:0px 0px 22px 30px" >
                        <asp:Label id ="LabMensaje" CssClass="formLabel2SinFondo" runat="server" ClientIDMode="Static" />

                        <div id="LabModLineaCausal" runat="server">
                            <label id="LabModSolCausalPoliza_RP" for="ModSolCausalPoliza_RP" class="formLabel" style="margin-left: 146px;">Causal:</label>
                            <asp:Label id ="ModSolCausalPoliza_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>    

                        </div>
                    </asp:Panel>--%>


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
                        <label id="LabModSolMonedaPrimaUnica_IFP" for="ModSolMonedaPrimaUnicaText_IFP" class="formLabel formLabel2Izq">Moneda de Prima Única*:</label>
                        <asp:Label ID="ModSolMonedaPrimaUnicaText_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

                        <label id="LabModSolPrimaUnica_IFP" for="ModSolPrimaUnica_IFP" class="formLabel formLabel2Der">Prima Única*:</label>
                        <asp:Label ID="ModSolPrimaUnica_IFP" runat="server" CssClass="formDato" Width="193" ClientIDMode="Static"></asp:Label>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolFechaCotizacion_IFP" for="ModSolFechaCotizacion_IFP" class="formLabel formLabel2Izq">Fecha de Cotización*:</label>
                        <asp:Label ID="ModSolFechaCotizacion_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

                        <asp:Label ID="LabModSolFechaVigencia_IFP" AssociatedControlID="ModSolFechaVigencia_IFP" runat="server" ClientIDMode="Static" for="ModSolFechaVigencia_RP" class="formLabel formLabel2Der">Vigente Hasta*:</asp:Label>
                        <asp:Label ID="ModSolFechaVigencia_IFP" runat="server" CssClass="formDato" Width="193" ClientIDMode="Static"></asp:Label>
                    </div>

                    <asp:Panel ID="Panel1" runat="server" CssClass="formLinea">
                        <label id="LabModSolFechaDevengue_IFP" for="ModSolFechaDevengue_IFP" class="formLabel formLabel2Izq">Fecha de Devengue*:</label>
                        <asp:Label ID="ModSolFechaDevengue_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

                        <%--<label id="LabModSolPlan_IFP" for="ModSolPlan_IFP" class="formLabel formLabel2Der">Agregar Plan:</label>
                        <asp:Label ID="ModSolPlan_IFP" runat="server" CssClass="formDato" Width="199" ClientIDMode="Static"></asp:Label>--%>

                        <%--<a id="ModSolAgregarPlan_IFP" href="javascript:void(0);" style="width: 20px; height: 22px; margin-left: 3px;" class="botonDeshabilitado gris gris_sharp">+</a>--%>

                        <asp:Panel ID="ModSolTipoCambioPanel" runat="server" Visible="true" ClientIDMode="Static">
                            <label id="LabModSolTipoCambio_IFP" for="ModSolTipoCambio_IFP" class="formLabel formLabel2Der">Tipo Cambio:</label>
                            <asp:Label ID="ModSolTipoCambio_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                        </asp:Panel>
                    </asp:Panel>

                    <%-- <div class="formLinea">                       
                        <asp:Panel ID="ModSolTipoCambioPanel" runat="server" Visible="true" ClientIDMode="Static">
                            <label id="LabModSolTipoCambio_IFP" for="ModSolTipoCambio_IFP" class="formLabel formLabel2Izq">Tipo Cambio:</label>
                            <asp:Label ID="ModSolTipoCambio_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>    
                        </asp:Panel>    
                    </div>--%>

                    <div class="formLinea" id="ModSolLineaNumPoliza_IFP" runat="server">
                        <label id="LabModSolNumPoliza_IFP" for="ModSolNumPoliza_IFP" class="formLabel formLabel2Izq">Nro. Póliza:</label>
                        <asp:Label ID="ModSolNumPoliza_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>


                        <label id="LabModSolEstadoPoliza_IFP" for="ModSolEstadoPoliza_IFP" class="formLabel formLabel2Der">Estado Póliza:</label>
                        <asp:Label ID="ModSolEstadoPoliza_IFP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
                    </div>


                    <%--<asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false">--%>
                    <asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false" Style="padding: 0px 0px 22px 30px">
                        <%--<asp:Label id ="LabMensaje" runat="server" ClientIDMode="Static" />--%>
                        <asp:Label ID="LabMensaje" CssClass="formLabel2SinFondo" runat="server" ClientIDMode="Static" />
                        <div id="LabModLineaCausal" runat="server">
                            <label id="LabModSolCausalPoliza_RP" for="ModSolCausalPoliza_RP" class="formLabel" style="margin-left: 146px;">Causal:</label>
                            <asp:Label ID="ModSolCausalPoliza_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

                        </div>

                    </asp:Panel>

                    <%--<div id="TablaCotizacionesCargando_RP" align="center" style="height: 50px; padding: 82px 0">
                        <asp:Image ID="icoTablaCotizacionesCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando cotizaciones, espere por favor...</span>
                    </div>
                    <div id="TablaCotizacionesContenedor_RP" style="display: none"></div>--%>

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
                        <a id="ModSolAceptarCierre_RP" href="javascript:void(0);" style="width: 150px; height: 22px" class="boton darkblue sharp">Generar Póliza</a>
                        <a id="ModSolAnularCierre_RP" href="javascript:void(0);" style="width: 150px; height: 22px" class="boton darkblue sharp">Anular Solicitud</a>
                        <a id="ModSolCancelar_RP" href="javascript:void(0);" style="width: 150px; height: 22px" class="boton darkblue sharp">Cancelar</a>
                        <%--<asp:Button ID="ModSolReporte_RP" Style="width: 150px; height: 24px" class="boton darkblue sharp" runat="server" Text="Imprimir Formatos" OnClick="ModSolReporte_RP_Click" Visible="false" />--%>
                        <%--<asp:Button id="ModSolReportePoliza_RP" style="width: 150px; height: 24px" class="boton darkblue sharp" runat="server" Text="Imprimir Póliza" OnClick="ModSolReportePoliza_RP_Click" Visible="false" />--%>
                    </div>
                </div>

                <div id="ManSolPestanha2" align="left" style="width: 820px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
                    <div id="TablaBeneficiariosCargando_RP" align="center" style="display: none">
                        <asp:Image ID="icoTablaBeneficiariosCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando beneficiarios, espere por favor...</span>
                    </div>
                    <div id="TablaBeneficiariosContenedor_RP" style="display: none"></div>
                    <div id="TablaBeneficiariosError_RP" class="grilla_error" style="display: none">No se ha podido cargar la tabla de beneficiarios. <a id="TablaBeneficiariosReintentar_RP">Intentar de nuevo</a>.</div>

                    <br />

                    <div id="BeneficiariosOriginales_RP" align="left" style="display: none">
                        Beneficiarios originales de la Solicitud:
                    </div>

                    <div id="TablaRviBenefiCargando_RP" align="center" style="display: none">
                        <asp:Image ID="icoTablaRviBenefiCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando beneficiarios registrados originalmente en la solicitud, espere por favor...</span>
                    </div>
                    <div id="TablaRviBenefiContenedor_RP" style="display: none"></div>
                    <div id="TablaRviBenefiError_RP" class="grilla_error" style="display: none">No se ha podido cargar la tabla de beneficiarios originales. <a id="TablaRviBenefiReintentar_RP">Intentar de nuevo</a>.</div>

                    <a name="beneficiarios"></a>
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
                        <asp:HiddenField ID="HCondicion" runat="server" ClientIDMode="Static" Value="0" />
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


            <div class="formLinea" id="lineaCausante">
                <label id="LabModSolCausante_RP" for="ModSolCausante_RP" class="formLabel2SinFondo formLabel2Izq">Seleccione Causal:</label>
                <asp:DropDownList ID="ModSolCausante_RP" runat="server" CssClass="formCombobox" Width="164" ClientIDMode="Static">
                </asp:DropDownList>
            </div>
            <p></p>
            <div id="MCABotonera" align="center">
                <a id="MCAAceptarCierre_Plus" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MCACancelar_Plus" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>

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

</asp:Content>
