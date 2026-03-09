<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="BandejaFlujoCotizacion.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Bandejas.BandejaFlujoCotizacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiSolicitudesCambio.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
    <script language="javascript">
    
        window.onload = async function () {
            UtilitariosManager.ValidarPermiso(EnumsPermisos.SolicitudConsultar);

            const solicitudEscenario = await ApiSolicitudesCambio.ObtenerSolicitudEscenarioCambio(0, 0, 0);
            console.log({solicitudEscenario})
            /* solicitudEscenario.forEach(x => {
                x.FechaPresentacion = x.FechaPresentacion.split('T')[0];
            }); */
            CargarTablaBandejaSolicitudesOficiales(solicitudEscenario);

            if (!permisoRechazarSolicitud) {
                $('#ModSolRechazarBandejaBloque').attr('class', 'botonDeshabilitado gris gris_sharp');
            }

            //<INIGTI_6556>
            if ($("#HEnviarEmail").val() == "1"){
                $('#divEmail').show();
                $('#ModSolEnviarEmailPendiente').show();
            }
            else {
                $('#divEmail').hide();
                $('#ModSolEnviarEmailPendiente').hide();
            }
                
            //<FINGTI_6556>

        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div align="left" style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e;">
        <h1 class="simple" style="width: 180px">Solicitudes de Cambio</h1>

        


        <div id="DatosCuotas" runat="server" clientidmode="Static" >
            <fieldset class="formFieldSet2Izq">
                <legend>Cuotas</legend>
                <table>
                    <tr>
                        <td>
                            <strong>
                                <label id="Label8" class="formLabelCuotas formLabel2Izq" style="width: 80px">Desde</label>
                            </strong>
                        </td>
                        <td>
                            <strong>
                                <label id="Label10" class="formLabelCuotas formLabel2Izq" style="width: 80px">Hasta</label>
                            </strong>
                        </td>
                        <td>
                            <strong>
                                <label id="Label12" class="formLabelCuotas formLabel2Izq" style="width: 80px; text-align: center">Nro. Casos Total</label>
                            </strong>
                        </td>
                        <td>
                            <strong>
                                <label id="Label14" class="formLabelCuotas formLabel2Izq" style="width: 80px; text-align: center">Nro. Casos Solicitados</label>
                            </strong>
                        </td>
                        <td>
                            <strong>
                                <label id="Label16" class="formLabelCuotas formLabel2Izq" style="width: 80px; text-align: center">Nro. Casos Efectivos</label>
                            </strong>
                        </td>
                        <td>
                            <strong>
                                <label id="Label1" class="formLabelCuotas formLabel2Izq" style="width: 80px; text-align: center">Saldo Cuotas</label>
                            </strong>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="LabFecIni1" runat="server" CssClass="formLabelCuotas formLabel2Izq" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabFecFin1" runat="server" CssClass="formLabelCuotas formLabel2Izq" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabTotal1" runat="server" CssClass="formLabelCuotas formLabel2Izq" Style="text-align: center; width: 80px" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabSolicitado1" runat="server" CssClass="formLabelCuotas formLabel2Izq" Style="text-align: center; width: 80px" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabEfectivo1" runat="server" CssClass="formLabelCuotas formLabel2Izq" Style="text-align: center; width: 80px" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabSaldo1" runat="server" CssClass="formLabelCuotas formLabel2Izq" Style="text-align: center; width: 80px" ClientIDMode="Static"></asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td>
                            <asp:Label ID="LabFecIni2" runat="server" CssClass="formLabelCuotas formLabel2Izq" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabFecFin2" runat="server" CssClass="formLabelCuotas formLabel2Izq" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabTotal2" runat="server" CssClass="formLabelCuotas formLabel2Izq" Style="text-align: center; width: 80px" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabSolicitado2" runat="server" CssClass="formLabelCuotas formLabel2Izq" Style="text-align: center; width: 80px" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabEfectivo2" runat="server" CssClass="formLabelCuotas formLabel2Izq" Style="text-align: center; width: 80px" ClientIDMode="Static"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="LabSaldo2" runat="server" CssClass="formLabelCuotas formLabel2Izq" Style="text-align: center; width: 80px" ClientIDMode="Static"></asp:Label>
                        </td>
                    </tr>

                </table>
            </fieldset>

        </div>

        <div class="formLinea" id="divEmail">
            <a id="ModSolEnviarEmailPendiente" style="width:200px;height:22px" class="boton darkblue sharp">Re-Enviar Email Pendientes</a>
        </div>

        <div id="Cargando" align="center" style="display: none">
            <asp:Image ID="icoCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Buscando solicitudes, espere por favor...</span>
        </div>
        <div id="TablaOficialError" align="center" style="display: none">
            <div class="grilla_error" align="left" style="width: 385px;">
                No se ha podido cargar la tabla de Oficiales. <a id="TablaOficialReintentar" href="#">Intentar de nuevo</a>.
            </div>
        </div>

        <div id="TablaSeguimiento" runat="server" style="display: none" clientidmode="Static"></div>

        <asp:HiddenField ID="TabSeguimientoIndicePagina" runat="server" ClientIDMode="Static" Value="1" />
        <asp:HiddenField ID="TabSeguimientoTamanhoPagina" runat="server" ClientIDMode="Static" Value="10" />
        <asp:HiddenField ID="TabSeguimientoColumnaOrdenar" runat="server" ClientIDMode="Static" Value="1" />
        <asp:HiddenField ID="TabSeguimientoDireccionOrdenar" runat="server" ClientIDMode="Static" Value="A" />
        <asp:HiddenField ID="HCusspp" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HNumSolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HNumOperacion" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCodTipoMovimiento" runat="server" ClientIDMode="Static" />

        <asp:HiddenField ID="HEnviarEmail" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="HSeleccionados" runat="server" ClientIDMode="Static" Value="" />

        <%--<div id="TablaSolicitudesCargando" align="center" style="display:none">
                <asp:Image ID="icoTablaSolicitudesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando solicitudes, espere por favor...</span>
        </div>
        <div id="TablaSolicitudesContenedor" style="display:none"></div>
        <div id="TablaSolicitudesError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesReintentar">Intentar de nuevo</a>.</div>--%>


        <%--<INIGTI_4081>--%>
        <%--
        <div id="TablaSolicitudesOficialesCargando" align="center" style="display:none">
                <asp:Image ID="icoTablaSolicitudesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando solicitudes, espere por favor...</span>
        </div>
        <div id="TablaSolicitudesOficialesContenedor" style="display:none"></div>
        <div id="TablaSolicitudesOficialesError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesReintentar">Intentar de nuevo</a>.</div>
        --%>
        <asp:Panel ID="TablaSolicitudesOficialesCargando" runat="server" ClientIDMode="Static" align="center">
            <asp:Image ID="icoTablaSolicitudesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando solicitudes, espere por favor...</span>
        </asp:Panel>

        <asp:Panel ID="TablaSolicitudesOficialesContenedor" runat="server" ClientIDMode="Static">
        </asp:Panel>

        <%--<FINGTI_4081>--%>


        <%-- <div class="formLinea" align="center">
        
        </div>--%>


        <asp:Panel ID="TablaSolicitudesOficialesError" runat="server" ClientIDMode="Static" class="grilla_error" style="display: none;">
            No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesReintentar">Intentar de nuevo</a>.
        </asp:Panel>


    </div>

    <div style="display: none">

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
                <a id="MCAOAceptarBandejaEmail" class="boton darkblue sharp" style="width:80px">Aceptar</a>
                <a id="MCAOCancelar" class="boton darkblue sharp" style="width:80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>


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
                <a id="MCMAceptarFlujo" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
            </div>
        </div>
        <%--Fin Modal Cuadro de mensajes--%>

        <%--Inicio Modal Mantenimiento de solicitud--%>

        <%--        <div id="ModalSolicitud" title="Datos de Escenario">
            <asp:HiddenField ID="ModSolModo" runat="server" ClientIDMode="Static" />
            <div id="ModSolCargando" class="modalCargandoContenido" style="width:848px;height:580px"></div>
            <div id="ModSolContenido" class="modalContenido">
                <div id="ManSolPestanhas">
                    <ul>
                        <li id="ManSolPes1" data-pestanha="1"><a href="#">Cotización</a></li>
                        <li id="ManSolPes2" data-pestanha="2"><a href="#">Beneficiarios <span id="ManSolNumBeneficiarios"></span></a> <span id="ManSolNumBeneficiariosCargando" class="pestanhaCargando"></span></li>
                    </ul>

                    <asp:HiddenField ID="ManSolPestanhaActiva" runat="server" Value="1" ClientIDMode="Static" />
                    <asp:HiddenField ID="ManSolTipoSolicitud" runat="server" ClientIDMode="Static" />

                    <div id="ManSolPestanha1" align="left" style="width:840px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
                        <div class="formLinea">
                            <label id="LabModSolNroSolicitud" for="ModSolNroSolicitud" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                            <asp:TextBox ID="ModSolNroSolicitud" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                            <label id="LabModSolNroMeller" for="ModSolNroMeller" class="formLabel formLabel2Der">Nro. Meller:</label>
                            <asp:TextBox ID="ModSolNroMeller" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>
                        
                        <div class="formLinea">
                            <label id="LabModSolMontoCIC" for="ModSolMontoCIC" class="formLabel formLabel2Izq">Monto CIC:</label>
                            <asp:TextBox ID="ModSolMontoCIC" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" data-v-max="100.00" data-v-min="-100.00" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                            <label id="LabModFecCierre" for="ModFecCierre" class="formLabel formLabel2Der">Fecha de Cierre:</label>
                            <asp:TextBox ID="ModFecCierre" runat="server" CssClass="formTextbox formCalendar" Width="180" ClientIDMode="Static" ReadOnly="True" Enabled="false"></asp:TextBox>
                        </div>

                        <asp:Panel ID="LabModSolLineaACOMDCOM" runat="server" CssClass="formLinea">
                            <label id="LabModSolACOM" for="ModSolACOM" class="formLabel formLabel2Izq">Porcentaje A:</label>
                            <asp:TextBox ID="ModSolACOM" runat="server" CssClass="formTextbox numerico" Width="180" data-v-max="100.00" data-v-min="-100.00" ClientIDMode="Static" ReadOnly="false"></asp:TextBox>

                            <label id="LabModSolDCOM" for="ModSolDCOM" class="formLabel formLabel2Der">Porcentaje D:</label>
                            <asp:TextBox ID="ModSolDCOM" runat="server" CssClass="formTextbox numerico" Width="180" data-v-max="100.00" data-v-min="-100.00" ClientIDMode="Static" ReadOnly="false"></asp:TextBox>
                        </asp:Panel>

                        <div class="formLinea">
                            <label id="LabModSolIndSeleccionado" for="ModSolIndSeleccionado" class="formLabel formLabel2Izq">Ind. Seleccionado:</label>
                            <asp:TextBox ID="ModSolIndSeleccionado" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="false"></asp:TextBox>

                            <label id="LabModSolValMontoAcomAgente" for="ModSolValMontoAcomAgente" class="formLabel formLabel2Der">Monto A:</label>
                            <asp:TextBox ID="ModSolValMontoAcomAgente" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static" ReadOnly="false"></asp:TextBox>
                        </div>

                        <div class="formLinea">
                            <label id="LabModSolCussp" for="ModSolCussp" class="formLabel formLabel2Izq">CUSSP:</label>
                            <asp:TextBox ID="ModSolCussp" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                            <label id="LabModSolNomAfiliado" for="ModSolNomAfiliado" class="formLabel formLabel2Der">Nombre Afiliado:</label>
                            <asp:TextBox ID="ModSolNomAfiliado" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div class="formLinea">
                            <label id="LabModSolNumAgente" for="ModSolNumAgente" class="formLabel formLabel2Izq">Nro. Agente:</label>
                            <asp:TextBox ID="ModSolNumAgente" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                            <label id="LabModSolNomAgente" for="ModSolNomAgente" class="formLabel formLabel2Der">Nombre Agente:</label>
                            <asp:TextBox ID="ModSolNomAgente" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div id="TablaCotizacionesCargando" align="center" style="height:50px;padding:82px 0">
                            <asp:Image ID="icoTablaCotizacionesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando cotizaciones, espere por favor...</span>
                        </div>
                        
                        
                        <div id="TablaCotizacionesContenedor" style="display:none"></div>

	                </div>

                    <div id="ManSolPestanha2" align="left" style="width:840px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
                        <div id="TablaBeneficiariosCargando" align="center" style="display:none">
                            <asp:Image ID="icoTablaBeneficiariosCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando beneficiarios, espere por favor...</span>
                        </div>
                        <div id="TablaBeneficiariosContenedor" style="display:none"></div>
                        <div id="TablaBeneficiariosError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de beneficiarios. <a id="TablaBeneficiariosReintentar" >Intentar de nuevo</a>.</div>

                        <br />

                        <div id="BeneficiariosOriginales" align="left" style="display:none">
                            Beneficiarios originales de la Solicitud:
                        </div>

                        <div id="TablaRviBenefiCargando" align="center" style="display:none">
                            <asp:Image ID="icoTablaRviBenefiCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando beneficiarios registrados originalmente en la solicitud, espere por favor...</span>
                        </div>
                        <div id="TablaRviBenefiContenedor" style="display:none"></div>
                        <div id="TablaRviBenefiError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de beneficiarios originales. <a id="TablaRviBenefiReintentar" >Intentar de nuevo</a>.</div>
                    </div>
                </div>
            </div>
        </div>--%>

        <%--Fin Modal Mantenimiento de solicitud--%>

        <%--Inicio Modal Vista Flujo--%>

        <%--<div id="ModalFlujoSolicitud" title="Datos de Flujo de Solicitud">
            
            <div id="ModFlujoSolCargando" class="modalCargandoContenido" style="width:848px;height:580px"></div>
            <div id="ModFlujoSolContenido" class="modalContenido">

                <asp:HiddenField ID="ManFlujoSolPestanhaActiva" runat="server" Value="1" ClientIDMode="Static" />
                <asp:HiddenField ID="ManFlujoSolTipoSolicitud" runat="server" ClientIDMode="Static" />

                <div id="ManFlujoSolPestanha1" align="left" style="width:840px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
                        

                    <div id="TablaFlujoCotizacionesContenedor" style="display:none"></div>


                    <div class="formLinea" align="center">
                        <a id="ModFlujoSolCancelar" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
                    </div>

                    <div id="TablaFlujoSolError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de movimientos de Cotización. <a id="TablaFlujoSolReintentar" >Intentar de nuevo</a>.</div>

	            </div>

            </div>
        </div>--%>

        <%--Fin Modal Vista Flujo--%>

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

        <%--Inicio Modal Generando Reporte--%>

        <%-- <div id="ModalGenerandoReporte">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MGRIcono" style="width:40px;height:40px"></td>
                        <td id="MGRContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MGRContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
                                <asp:HiddenField ID="MGREstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MGREstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MRGEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MGRMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>--%>

        <%--Fin Modal Generando Reporte--%>

        <%--Inicio Modal Seleccion de ACOM--%>

        <%--<div id="ModalSeleccionAcom">
            <div>
                <div id="TablaSeleccionAcomCargando" align="center" style="display:none">
                    <asp:Image ID="Image2" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                    <span class="texto">Cargando registros de ACOM, espere por favor...</span>
                </div>

                <div id="DivErrorAcom" class="grilla_error" style="display:none">
                    No se ha podido cargar la tabla de ACOM. <a id="A1">Intentar de nuevo</a>.
                </div>

                <div id="TablaSeleccionAcomContenedor" style="display:none;"></div>
            </div>
            <br />
            <div id="DivBotoneraAcom" align="center">
                <asp:Button ID="MRVGuardarSeleccionAcomOficiales" CssClass="boton darkblue sharp" style="width:80px;height:22px;" runat="server" Text="Aceptar" ClientIDMode="Static" />
                <asp:Button ID="MRVCancelarSeleccionAcomOficiales" CssClass="boton darkblue sharp" style="width:80px;height:22px;" runat="server" Text="Cancelar" ClientIDMode="Static" />
            </div>
        </div>--%>

        <asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Seleccion de ACOM--%>
    </div>
</asp:Content>
