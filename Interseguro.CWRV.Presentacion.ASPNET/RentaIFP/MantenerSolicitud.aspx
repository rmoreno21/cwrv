<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="MantenerSolicitud.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaIFP.MantenerSolicitud" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiCotizadorIFP.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.ingresoflexibleplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script>
        // $(document).ready(function () {
            // CargarTablaBeneficiariosPlan3IFP(null);
        // });
    </script>

    <style>
        .formLabel2Der {
            margin-left: 90px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="NuevaCotizacionRP" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">

        <h1 class="simple" style="width: 264px">Cotizaciones Ingreso Flexible Plus</h1>

        <asp:HiddenField ID="ModSolModo" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCUSPP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAFP_RP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HSeleccionada" runat="server" ClientIDMode="Static" Value="N" />
        <asp:HiddenField ID="HDiasVigencia" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HCopia" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="HBloqueo" runat="server" ClientIDMode="Static" Value="TRUE" />
        <asp:HiddenField ID="HEstado" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HFechaActual" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HAgenteId" runat="server" ClientIDMode="Static" Value="0" />

        <div id="ModSolContenido" class="modalContenido">

            <div id="ManSolPestanhas">

                <ul>
                    <li id="ManSolPes1" data-pestanha="1"><a href="#">Cotización</a></li>
                    <li id="ManSolPes2" data-pestanha="2" style="display: none;"><a href="#">Beneficiarios <span id="ManSolNumBeneficiarios_RP"></span></a> <span id="ManSolNumBeneficiariosCargando_RP" class="pestanhaCargando"></span></li>
                </ul>

                <asp:HiddenField ID="ManSolPestanhaActiva_RP" runat="server" Value="1" ClientIDMode="Static" />
                <asp:HiddenField ID="ManSolTipoSolicitud_RP" runat="server" ClientIDMode="Static" />

                <div id="ManSolPestanha1" align="left" style="width: 820px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">

                    <div class="formLinea">
                        <label id="LabModSolNroSolicitud_IFP" for="ModSolNroSolicitud_IFP" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                        <asp:TextBox ID="ModSolNroSolicitud_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModSolDNI_IFP" for="ModSolDNI_IFP" class="formLabel formLabel2Der">DNI:</label>
                        <asp:TextBox ID="ModSolDNI_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="193" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolNombres_IFP" for="ModSolNombres_IFP" class="formLabel formLabel2Izq">Nombres:</label>
                        <asp:TextBox ID="ModSolNombres_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModSolApellidos_IFP" for="ModSolApellidos_IFP" class="formLabel formLabel2Der">Apellidos:</label>
                        <asp:TextBox ID="ModSolApellidos_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="193" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolMonedaPrimaUnica_IFP" for="ModSolMonedaPrimaUnica_IFP" class="formLabel formLabel2Izq">Moneda de Prima Única*:</label>
                        <asp:DropDownList ID="ModSolMonedaPrimaUnica_IFP" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static"></asp:DropDownList>

                        <label id="LabModSolPrimaUnica_IFP" for="ModSolPrimaUnica_IFP" class="formLabel formLabel2Der">Prima Única*:</label>
                        <asp:TextBox ID="ModSolPrimaUnica_IFP" runat="server" CssClass="formTextbox numerico" Width="193" ClientIDMode="Static"></asp:TextBox>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolFechaCotizacion_IFP" for="ModSolFechaCotizacion_IFP" class="formLabel formLabel2Izq">Fecha de Cotización*:</label>
                        <asp:TextBox ID="ModSolFechaCotizacion_IFP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                        <asp:Label ID="LabModSolFechaVigencia_IFP" AssociatedControlID="ModSolFechaVigencia_IFP" runat="server" ClientIDMode="Static" for="ModSolFechaVigencia_RP" class="formLabel formLabel2Der">Vigente Hasta*:</asp:Label>
                        <asp:TextBox ID="ModSolFechaVigencia_IFP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="193" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>
                    </div>

                    <asp:Panel runat="server" CssClass="formLinea">
                        <label id="LabModSolFechaDevengue_IFP" for="ModSolFechaDevengue_IFP" class="formLabel formLabel2Izq">Fecha de Devengue*:</label>
                        <asp:TextBox ID="ModSolFechaDevengue_IFP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="180" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>

                        <%--<asp:Panel ID="ContenedorPlan_RP" runat="server" ClientIDMode="Static">--%>
                        <label id="LabModSolPlan_IFP" for="ModSolPlan_IFP" class="formLabel formLabel2Der">Agregar Plan:</label>
                        <asp:DropDownList ID="ModSolPlan_IFP" runat="server" CssClass="formCombobox" Width="199" ClientIDMode="Static"></asp:DropDownList>

                        <a id="ModSolAgregarPlan_IFP" href="javascript:void(0);" style="width: 20px; height: 22px; margin-left: 3px;" class="botonDeshabilitado gris gris_sharp">+</a>

                        <%--</asp:Panel>--%>
                    </asp:Panel>

                    <div class="formLinea">
                        <asp:Panel ID="ModSolTipoCambioPanel" runat="server" Visible="true" ClientIDMode="Static">
                            <label id="LabModSolTipoCambio_IFP" for="ModSolTipoCambio_IFP" class="formLabel formLabel2Izq">Tipo Cambio:</label>
                            <asp:TextBox ID="ModSolTipoCambio_IFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </asp:Panel>
                    </div>

                    <div class="formLinea" id="ModSolLineaNumPoliza_RP" runat="server">
                        <label id="LabModSolNumPoliza_RP" for="ModSolNumPoliza_RP" class="formLabel formLabel2Izq">Nro. Póliza:</label>
                        <asp:TextBox ID="ModSolNumPoliza_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                        <label id="LabModSolEstadoPoliza_RP" for="ModSolEstadoPoliza_RP" class="formLabel formLabel2Der">Estado Póliza:</label>
                        <asp:TextBox ID="ModSolEstadoPoliza_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                    </div>

                    <asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false" Style="padding: 0px 0px 22px 30px">
                        <asp:Label ID="LabMensaje" CssClass="formLabel2SinFondo" runat="server" ClientIDMode="Static" />

                        <div id="LabModLineaCausal" runat="server">
                            <label id="LabModSolCausalPoliza_RP" for="ModSolCausalPoliza_RP" class="formLabel" style="margin-left: 152px;">Causal:</label>
                            <asp:TextBox ID="ModSolCausalPoliza_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>
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
                        <a id="ModSolAceptar_RP" href="javascript:void(0);" style="width: 90px; height: 22px" class="boton darkblue sharp">Cotizar</a>
                        <a id="ModSolCancelar_RP" href="javascript:void(0);" style="width: 90px; height: 22px" class="boton darkblue sharp">Cotizaciones</a>
                        <a id="ModSolImprimir_RP" href="javascript:void(0);" style="width: 90px; height: 22px" class="boton darkblue sharp">Imprimir</a>

                    </div>

                    <div id="ManSolLeyenda">
                        <fieldset>
                            <legend>Leyenda</legend>
                            <asp:Panel ID="TabCotizacionesLeyenda_RP" runat="server" ClientIDMode="Static" Visible="true" Style="font-family: Calibri; font-size: 13px; color: #0060A9;">
                                <span class="grilla_error_tra" style="width: 25px !important; height: auto; display: inline-block">&nbsp;</span><span style="padding-left: 5px">Cotización no alcanza el mínimo requerido, por lo cual no se simula.</span>
                            </asp:Panel>
                            <div id="TabCotizacionesLeyenda_diferido_RP" style="font-family: Calibri; font-size: 13px; color: #0060A9;">
                                <span style="padding-left: 30px">Para las cotizaciones con moneda ajustada y diferimiento, mostrarán la renta ajustada al primer pago entre paréntesis "()".</span>
                            </div>
                        </fieldset>
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

    <asp:HiddenField ID="url_cotizador_ifp" runat="server" ClientIDMode="Static" Value='<%# System.Configuration.ConfigurationManager.AppSettings["url_cotizador_ifp"] %>' />
</asp:Content>
