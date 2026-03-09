<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="MantenerSolicitud.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus.MantenerSolicitud" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.rentaprivadaplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.rentaprivadaplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <asp:Panel ID="NuevaCotizacionRP" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 250px">Cotización Renta Particular Plus</h1>
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

        <%--<INIGTI_7012>--%>
        <asp:HiddenField ID="HEstado" runat="server" ClientIDMode="Static" Value="0" />
        <%--<FINGTI_7012>--%>

        <div id="ModSolContenido" class="modalContenido">
            <div id="ManSolPestanhas">
                <ul>
                    <li id="ManSolPes1" data-pestanha="1"><a href="#">Cotización</a></li>
                    <li id="ManSolPes2" data-pestanha="2"><a href="#">Beneficiarios <span id="ManSolNumBeneficiarios_RP"></span></a><span id="ManSolNumBeneficiariosCargando_RP" class="pestanhaCargando"></span></li>
                </ul>

                <asp:HiddenField ID="ManSolPestanhaActiva_RP" runat="server" Value="1" ClientIDMode="Static" />
                <asp:HiddenField ID="ManSolTipoSolicitud_RP" runat="server" ClientIDMode="Static" />

                <div id="ManSolPestanha1" align="left" style="width: 820px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">

                    <%--<div>
                        <ul style="border:1px solid #000; height: 32px;padding:0;margin-bottom:10px">
                            <li style="border:1px solid #A00; background:none;color:#000;font-size:0.9em">Guardar</li>
                            <li>Reporte Detalle Cotizacion</li>
                            <li>Enviar por Correo Electrónico</li>
                            <li>Salir</li>
                        </ul>
                    </div>--%>

                    <div class="formLinea">
                        <label id="LabModSolNroSolicitud_RP" for="ModSolNroSolicitud_RP" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                        <asp:TextBox ID="ModSolNroSolicitud_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <asp:Panel ID="ModSolTipoCambioPanel" runat="server" Visible="true" ClientIDMode="Static">
                            <label id="LabModSolTipoCambio" for="ModSolTipoCambio" class="formLabel formLabel2Der">Tipo Cambio:</label>
                            <asp:TextBox ID="ModSolTipoCambio" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </asp:Panel>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolMonedaPrimaUnica_RP" for="ModSolMonedaPrimaUnica_RP" class="formLabel formLabel2Izq">Moneda de Prima Única*:</label>
                        <asp:DropDownList ID="ModSolMonedaPrimaUnica_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                        </asp:DropDownList>

                        <label id="LabModSolPrimaUnica_RP" for="ModSolPrimaUnica_RP" class="formLabel formLabel2Der">Prima Única*:</label>
                        <asp:TextBox ID="ModSolPrimaUnica_RP" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static"></asp:TextBox>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolFechaCotizacion_RP" for="ModSolFechaCotizacion_RP" class="formLabel formLabel2Izq">Fecha de Cotización*:</label>
                        <asp:TextBox ID="ModSolFechaCotizacion_RP" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                        <label id="LabModSolFechaDevengue_RP" for="ModSolFechaDevengue_RP" class="formLabel formLabel2Der">Fecha de Devengue*:</label>
                        <asp:TextBox ID="ModSolFechaDevengue_RP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="180" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>

                    </div>

                    <div class="formLinea">
                        <label id="LabModSolTipoPlan_RP" for="ModSolTipoPlan_RP" class="formLabel formLabel2Izq">Tipo de Plan*:</label>
                        <asp:DropDownList ID="ModSolTipoPlan_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                        </asp:DropDownList>

                        <label id="LabModSolTemporalidad_RP" for="ModSolTemporalidad_RP" class="formLabel formLabel2Der">Temporalidad*:</label>
                        <asp:DropDownList ID="ModSolTemporalidad_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                        </asp:DropDownList>
                    </div>

                    <asp:Panel runat="server" CssClass="formLinea">
                        <asp:Panel ID="LabModSolLineaACOMDCOM_RP" runat="server">
                            <label id="LabModSolDCOM_RP" for="ModSolDCOM_RP" class="formLabel formLabel2Izq">Porcentaje D*:</label>
                            <asp:TextBox ID="ModSolDCOM_RP" runat="server" CssClass="formTextbox numerico" Width="180" data-v-max="100.00" data-v-min="-100.00" ClientIDMode="Static"></asp:TextBox>
                        </asp:Panel>

                        <asp:Label ID="LabModSolFechaVigencia_RP" AssociatedControlID="LabModSolFechaVigencia_RP" runat="server" ClientIDMode="Static" for="ModSolFechaVigencia_RP" class="formLabel formLabel2Der">Vigente Hasta*:</asp:Label>
                        <%--<label id="LabModSolFechaVigencia_RP" for="ModSolFechaVigencia_RP" class="formLabel formLabel2Der">Fecha de Vigencia*:</label>--%>
                        <asp:TextBox ID="ModSolFechaVigencia_RP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="180" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>


                    </asp:Panel>

                    <%--<INIGTI_7012>--%>
                    <div class="formLinea" id="ModSolLineaNumPoliza_RP" runat="server">
                        <label id="LabModSolNumPoliza_RP" for="ModSolNumPoliza_RP" class="formLabel formLabel2Izq">Nro. Póliza:</label>
                        <asp:TextBox ID="ModSolNumPoliza_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>


                        <label id="LabModSolEstadoPoliza_RP" for="ModSolEstadoPoliza_RP" class="formLabel formLabel2Der">Estado Póliza:</label>
                        <asp:TextBox ID="ModSolEstadoPoliza_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                    </div>
                    <%--<FINGTI_7012>--%>

                    <%--<INIGTI_7012>--%>
                    <%--<asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false">
                            Solicitud Cerrada
                    </asp:Panel>--%>
                    <%--<asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false">
                        <asp:Label id ="LabMensaje" runat="server" ClientIDMode="Static" />
                    </asp:Panel>--%>

                    <asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false" Style="padding: 0px 0px 22px 30px">
                        <%--<label id = "LabMensaje" runat="server"></label>--%>
                        <asp:Label ID="LabMensaje" CssClass="formLabel2SinFondo" runat="server" ClientIDMode="Static" />

                        <div id="LabModLineaCausal" runat="server">
                            <label id="LabModSolCausalPoliza_RP" for="ModSolCausalPoliza_RP" class="formLabel" style="margin-left: 152px;">Causal:</label>
                            <%--<asp:Label id ="ModSolCausalPoliza_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>    --%>
                            <asp:TextBox ID="ModSolCausalPoliza_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>
                    </asp:Panel>

                    <%--<FINGTI_7012>--%>

                    <div id="TablaCotizacionesCargando_RP" align="center" style="height: 50px; padding: 82px 0">
                        <asp:Image ID="icoTablaCotizacionesCargando_RP" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando cotizaciones, espere por favor...</span>
                    </div>
                    <div id="TablaCotizacionesContenedor_RP" style="display: none"></div>

                    <%--<div class="formLinea">
                        <label id="LabModSolMontoCIA" class="formLabel formLabel2Izq">Monto CIA:</label>
                        <asp:TextBox ID="ModSolMontoCIA" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModSolTasaAFP" class="formLabel formLabel2Der">Tasa AFP:</label>
                        <asp:TextBox ID="ModSolTasaAFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>--%>

                    <%--<div class="formLinea">
                        <label id="LabModSolPensionCIA" class="formLabel formLabel2Izq">Pensión CIA:</label>
                        <asp:TextBox ID="ModSolPensionCIA" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModSolMontoAFP" class="formLabel formLabel2Der">Monto AFP:</label>
                        <asp:TextBox ID="ModSolMontoAFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>--%>

                    <%--<div class="formLinea">
                        <label id="LabModSolPensionCIAMO" class="formLabel formLabel2Izq">Pensión CIA (M.O.):</label>
                        <asp:TextBox ID="ModSolPensionCIAMO" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModSolPensionAFP" class="formLabel formLabel2Der">Pensión AFP:</label>
                        <asp:TextBox ID="ModSolPensionAFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>--%>

                    <%--SRI.INI-20322--%>
                    <%--<div class="formLinea">
                        <label id="LabModTasaVenta" class="formLabel formLabel2Izq">Tasa de Venta:</label>
                        <asp:TextBox ID="ModTasaVenta" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModTasaVentaSbs" class="formLabel formLabel2Der">Tasa de Venta SBS:</label>
                        <asp:TextBox ID="ModTasaVentaSbs" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>--%>
                    <%--SRI.FIN-20322--%>

                    <div class="formLinea" align="center">
                        <a id="ModSolAceptar_RP" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
                        <a id="ModSolCancelar_RP" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
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
                <a id="MCAAceptar_Plus" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
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
