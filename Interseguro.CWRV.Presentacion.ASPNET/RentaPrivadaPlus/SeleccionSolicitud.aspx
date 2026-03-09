<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="SeleccionSolicitud.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.RentaPrivadaPlus.SeleccionSolicitud" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/lightbox.css")%>" />
    <link rel="stylesheet" href="<%=ResolveUrl("~/Estilos/google-drive.css")%>" />

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.google.drive.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.rentaprivadaplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.rentaprivadaplus.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/ajaxfileupload.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>

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

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <asp:Panel ID="NuevaCotizacionRP" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 299px">Cerrar Cotización Renta Particular Plus</h1>
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

        <asp:HiddenField ID="HAfiliadoNombreCompleto" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAfiliadoEmail" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAfiliadoTelefono" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAfiliadoTipoDocumento" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAfiliadoNumeroDocumento" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HAfiliadoPEP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HFirmado" runat="server" ClientIDMode="Static" />

        <div id="ModSolContenido" class="modalContenido">
            <div id="ManSolPestanhas">
                <ul>
                    <li id="ManSolPes1" data-pestanha="1"><a href="#">Cotización</a></li>
                    <li id="ManSolPes2" data-pestanha="2"><a href="#">Beneficiarios <span id="ManSolNumBeneficiarios_RP"></span></a><span id="ManSolNumBeneficiariosCargando_RP" class="pestanhaCargando"></span></li>
                </ul>

                <asp:HiddenField ID="ManSolPestanhaActiva_RP" runat="server" Value="1" ClientIDMode="Static" />
                <asp:HiddenField ID="ManSolTipoSolicitud_RP" runat="server" ClientIDMode="Static" />

                <div style="visibility: hidden">
                    <asp:DropDownList ID="ModSolMonedaPrimaUnica_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                    </asp:DropDownList>

                    <asp:DropDownList ID="ModSolTipoPlan_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                    </asp:DropDownList>
                    <asp:DropDownList ID="ModSolTemporalidad_RP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                    </asp:DropDownList>
                </div>

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
                        <%--<asp:TextBox ID="ModSolNroSolicitud_RP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>--%>
                        <asp:Label ID="ModSolNroSolicitud_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

                        <asp:Panel ID="ModSolTipoCambioPanel" runat="server" Visible="true" ClientIDMode="Static">
                            <label id="LabModSolTipoCambio" for="ModSolTipoCambio" class="formLabel formLabel2Der">Tipo Cambio:</label>
                            <%--<asp:TextBox ID="ModSolTipoCambio" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static"  ReadOnly="True"  ></asp:TextBox>    --%>
                            <asp:Label ID="ModSolTipoCambio" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />
                        </asp:Panel>
                    </div>

                    <div class="formLinea">
                        <label id="LabModSolMonedaPrimaUnica_RP" for="ModSolMonedaPrimaUnica_RP" class="formLabel formLabel2Izq">Moneda de Prima Única*:</label>
                        <asp:Label ID="ModSolMonedaPrimaUnicaText_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

                        <label id="LabModSolPrimaUnica_RP" for="ModSolPrimaUnica_RP" class="formLabel formLabel2Der">Prima Única*:</label>
                        <%--<asp:TextBox ID="ModSolPrimaUnica_RP" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static"   ></asp:TextBox>--%>
                        <asp:Label ID="ModSolPrimaUnica_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

                    </div>

                    <div class="formLinea">
                        <label id="LabModSolFechaCotizacion_RP" for="ModSolFechaCotizacion_RP" class="formLabel formLabel2Izq">Fecha de Cotización*:</label>
                        <%--<asp:TextBox ID="ModSolFechaCotizacion_RP" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>                       --%>
                        <asp:Label ID="ModSolFechaCotizacion_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

                        <label id="LabModSolFechaDevengue_RP" for="ModSolFechaDevengue_RP" class="formLabel formLabel2Der">Fecha de Devengue*:</label>
                        <%--<asp:TextBox ID="ModSolFechaDevengue_RP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="180" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>--%>
                        <asp:Label ID="ModSolFechaDevengue_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

                    </div>

                    <div class="formLinea">
                        <label id="LabModSolTipoPlan_RP" for="ModSolTipoPlan_RP" class="formLabel formLabel2Izq">Tipo de Plan*:</label>
                        <asp:Label ID="ModSolTipoPlanText_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

                        <label id="LabModSolTemporalidad_RP" for="ModSolTemporalidad_RP" class="formLabel formLabel2Der">Temporalidad*:</label>
                        <asp:Label ID="ModSolTemporalidadText_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />
                    </div>

                    <asp:Panel ID="Panel1" runat="server" CssClass="formLinea">
                        <asp:Panel ID="LabModSolLineaACOMDCOM_RP" runat="server">
                            <label id="LabModSolDCOM_RP" for="ModSolDCOM_RP" class="formLabel formLabel2Izq">Porcentaje D*:</label>
                            <%--<asp:TextBox ID="ModSolDCOM_RP" runat="server" CssClass="formTextbox numerico" Width="180" data-v-max="100.00" data-v-min="-100.00" ClientIDMode="Static"></asp:TextBox>--%>
                            <asp:Label ID="ModSolDCOM_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />
                        </asp:Panel>

                        <asp:Label ID="LabModSolFechaVigencia_RP" AssociatedControlID="LabModSolFechaVigencia_RP" runat="server" ClientIDMode="Static" for="ModSolFechaVigencia_RP" class="formLabel formLabel2Der">Vigente Hasta*:</asp:Label>
                        <%--<asp:TextBox ID="ModSolFechaVigencia_RP" runat="server" CssClass="fecha formTextbox formCalendar formTextboxReadOnly" Width="180" ClientIDMode="Static" MaxLength="10" Enabled="false"></asp:TextBox>--%>
                        <asp:Label ID="ModSolFechaVigencia_RP" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static" />

                    </asp:Panel>

                    <%--<asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_infoDer" Visible="false">
                            Solicitud Cerrada
                        </asp:Panel>--%>
                    <%--<INIGTI_7012>--%>
                    <%--<asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false">
                            Solicitud Cerrada
                    </asp:Panel>--%>

                    <asp:Panel ID="LabModSolAviso" runat="server" CssClass="grilla_info" Visible="false" Style="margin-bottom: 10px">
                        <asp:Label ID="LabMensaje" runat="server" ClientIDMode="Static" />
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
                        <a id="ModSolVistaPrevia_RP" href="javascript:void(0);" style="width: 150px; height: 22px" class="boton darkblue sharp">Vista Previa</a>
                        <a id="ModSolAceptarCierre_RP" href="javascript:void(0);" style="width: 150px; height: 22px" class="boton darkblue sharp">Enviar a cliente</a>
                        <asp:HyperLink ID="ReenvioManual" Visible="false" ClientIDMode="Static" runat="server" style="width: 150px; height: 22px" CssClass="boton darkblue sharp">Reenvío manual</asp:HyperLink>
                        <%--<asp:Button ID="ModSolReporte_RP" Style="width: 152px; height: 24px" class="boton darkblue sharp" runat="server" Text="Imprimir Formatos" OnClick="ModSolReporte_RP_Click" Visible="false" />--%>
                        <asp:Button ID="ModSolAgregarArchivos" Style="width: 152px; height: 24px" class="boton darkblue sharp" runat="server" Text="Cargar Archivos" Visible="false" ClientIDMode="Static" />
                        <a id="ModSolCancelar_RP" href="javascript:void(0);" style="width: 150px; height: 22px" class="boton darkblue sharp">Cancelar</a>

                        <%--<a id="ModSolPrueba" href="javascript:void(0);" style="width: 180px; height: 22px" class="boton darkblue sharp">Prueba</a>--%>

                        <%--<asp:FileUpload ID="FileUpload1" runat="server" ClientIDMode="Static" AllowMultiple="true" />--%>
                    </div>

                    <div>
                        <div id="drive-box" class="hide">
                            <%--<div id="drive-info1" class="hide">
                                <div class="user-item">Welcome <span id="span-name"></span></div>
                                <div class="user-item">Total Quota: <span id="span-totalQuota"></span></div>
                                <div class="user-item">Used Quota: <span id="span-usedQuota"></span></div>
		                        <div class="user-item">Share Mode: <span id="span-sharemode">OFF</span></div>
                                <div class="user-item"><a id="link-logout" class="logout-link" onclick="handleSignoutClick()">Logout</a></div>
                            </div>--%>

                            <div id="drive-menu">
                                <div id="button-reload" title="Refrescar"></div>
                                <div id="button-upload" title="Cargar a Google Drive" class="button-opt"></div>
                            </div>
                            <div id="drive-content"></div>
                            <div id="error-message" class="flash hidden"></div>
                            <div id="status-message" class="flash hidden"></div>

                        </div>

                        <input type="file" id="fUpload" name="fUpload" class="hide" multiple accept=".jpg, .jpeg, .pdf, .png" length="1024" />

                        <%--                        <div class="float-box" id="float-box">
                            <div class="folder-form">
                                
                                <div class="close-x"><img id="imgClose" class="imgClose" src="<%=ResolveUrl("~/Estilos/images/button_close.png")%>" alt="close" /></div>
                                <h3 class="clear">Add New Folder</h3>
                                <div><input type="text" id="txtFolder" class="text-input" /></div>
		                        <button id="btnAddFolder" value="Save" class="button">Add</button>
		                        <button id="btnClose" value="Close" class="button btnClose">Close</button>
                            </div>
                        </div>
                        --%>
                        <%-- <div id="float-box-info" class="float-box">
                            <div class="info-form">
                                <div class="close-x"><img id="imgCloseInfo" class="imgClose" src="<%=ResolveUrl("~/Estilos/images/button_close.png")%>" alt="close" /></div>
                                <h3 class="clear">File information</h3>
                                <table cellpadding="0" cellspacing="0" class="tbl-info">
                                    <tr>
                                        <td class="label">Created Date</td>
                                        <td><span id="spanCreatedDate"></span></td>
                                    </tr>
                                    <tr>
                                        <td class="label">Modified Date</td>
                                        <td><span id="spanModifiedDate"></span></td>
                                    </tr>
                                    <tr>
                                        <td class="label">Owner</td>
                                        <td><span id="spanOwner"></span></td>
                                    </tr>
                                    <tr>
                                        <td class="label">Title</td>
                                        <td><span id="spanTitle"></span></td>
                                    </tr>
                                    <tr>
                                        <td class="label">Size</td>
                                        <td><span id="spanSize"></span></td>
                                    </tr>
                                    <tr>
                                        <td class="label">Extension</td>
                                        <td><span id="spanExtension"></span></td>
                                    </tr>
                                </table>
		                        <button id="btnCloseInfo" value="Close" class="button btnClose">Close</button>
                            </div>
                        </div>
                        --%>

                        <%--<div id="float-box-text" class="float-box">
                            <div class="info-form">
                                <div class="close-x"><img id="imgCloseText" class="imgClose" src="images/button_close.png" alt="close" /></div>
                                <h3 class="clear">Text Content</h3>
                                <div id="text-content"></div>
		                        <button id="btnCloseText" value="Close" class="button btnClose">Close</button>
                            </div>
                        </div>--%>
                    </div>

                    <div class="formLinea" id="divCargar" align="center" style="display: none">
                        <asp:Button ClientIDMode="Static" ID="ModSolEnviarEvaluacion" Style="width: 182px; height: 24px" class="boton darkblue sharp" runat="server" Text="Enviar a Evaluación" Visible="false" />
                        <asp:Button ClientIDMode="Static" ID="CorregirDocumentos" Style="width: 182px; height: 24px" class="boton darkblue sharp" runat="server" Text="Corregir Documentos" Visible="false" />
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

        <%--enviar--%>
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
