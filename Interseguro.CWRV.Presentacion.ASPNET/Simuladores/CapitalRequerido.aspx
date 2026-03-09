<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CapitalRequerido.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Simuladores.CapitalRequerido" EnableEventValidation="false" %>

<%@ Register Src="~/Controles/TablaGrupoFamiliarCapitalRequerido.ascx" TagPrefix="cwrv" TagName="TablaGrupoFamiliar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.simuladores.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">
        <h1 class="simple" style="width:232px">Cálculo de Capital Requerido</h1>
        
        <asp:HiddenField ID="HBusAfiNroSolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HBusAfiCUSPP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="BusAfiDatosCargados" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="PerBusAfiBuscar" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="IdSimulador" runat="server" ClientIDMode="Static" Value="" />

        <asp:HiddenField ID="HApellidoPaterno" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="HApellidoMaterno" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="HNombres" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="HSexo" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="HMaxRegCalculoCapital" runat="server" ClientIDMode="Static" Value="" />
        
        

        <asp:Panel ID="FormularioBusqueda" runat="server" ClientIDMode="Static">
            <fieldset>
                <legend>Búsqueda de afiliados</legend>
                <div class="formLinea">
                    <label id="LabBusAfiNroSolicitud" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                    <asp:TextBox ID="BusAfiNroSolicitud" runat="server" CssClass="formTextbox" style="text-transform:uppercase" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                    <label id="LabBusAfiCUSPP" class="formLabel formLabel2Der">CUSPP:</label>
                    <asp:TextBox ID="BusAfiCUSPP" runat="server" CssClass="formTextbox alfanumerico" style="text-transform:uppercase" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
            
                    <asp:HyperLink ID="BusAfiExaminarSolicitud" CssClass="boton darkblue sharp" style="width:22px;height:22px;position:relative;margin-left:5px" runat="server" ClientIDMode="Static">...</asp:HyperLink>
                    <asp:HiddenField ID="PerBusAfiExaminarSolicitud" runat="server" ClientIDMode="Static" />
                </div>

                <div class="formLinea" align="center">
                    <asp:Button ID="BusAfiBuscar" CssClass="boton darkblue sharp" Width="80"
                        Height="24" runat="server" Text="Buscar" ClientIDMode="Static" OnClick="BusAfiBuscar_Click" />
                </div>
            </fieldset>
        </asp:Panel>
        
        <asp:Panel ID="DatosAfiliado" runat="server" ClientIDMode="Static" Visible="False">
            <br />
            <fieldset>
                <legend>Datos del afiliado</legend>
                <asp:Panel ID="LineaCUSPP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabCUSPP" for="CUSPP" class="formLabel formLabel2Izq">CUSPP:</label>
                    <asp:Label ID="CUSPP" runat="server" ClientIDMode="Static" CssClass="formDato"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="LineaNombre" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabNombres" for="Nombres" class="formLabel formLabel2Izq">Nombre:</label>
                    <asp:Label ID="Nombre" runat="server" ClientIDMode="Static" CssClass="formDato"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="LineaNacimiento" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabFechaNacimiento" for="FechaNacimiento" class="formLabel formLabel2Izq">Fecha de nacimiento:</label>
                    <asp:Label ID="FechaNacimiento" runat="server" ClientIDMode="Static" CssClass="formDato"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="LineaSexo" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabSexo" for="Nombres" class="formLabel formLabel2Izq">Sexo:</label>
                    <asp:Label ID="Sexo" runat="server" ClientIDMode="Static" CssClass="formDato"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="LineaCorreoElectronico" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabCorreoElectronico" for="CorreoElectronico" class="formLabel formLabel2Izq">Correo Electrónico:</label>
                    <asp:Label ID="CorreoElectronico" runat="server" ClientIDMode="Static" CssClass="formDato"></asp:Label>
                    <asp:HiddenField ID="CorreoElectronicoRegistrado" runat="server" ClientIDMode="Static" />
                </asp:Panel>

                <asp:HiddenField ID="HCategoria" runat="server" ClientIDMode="Static" Value="0" />

                <asp:HiddenField ID="HAFP" runat="server" ClientIDMode="Static" Value="0" />
                <asp:Panel ID="LineaAFP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabAFP" for="AFP" class="formLabel formLabel2Izq">AFP:</label>
                    <asp:Label ID="AFP" runat="server" ClientIDMode="Static" CssClass="formDato"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="LineaCategoria" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabCategoria" for="Categoria" class="formLabel formLabel2Izq">Categoría:</label>
                    <asp:Label ID="Categoria" runat="server" ClientIDMode="Static" CssClass="formDato"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="LineaAgente" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabAgente" for="Agente" class="formLabel formLabel2Izq">Agente:</label>
                    <asp:Label ID="Agente" runat="server" ClientIDMode="Static" CssClass="formDato"></asp:Label>
                </asp:Panel>

                <asp:Panel ID="LineaSaldoCIC" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabSaldoCIC" for="SaldoCIC" class="formLabel formLabel2Izq">Saldo CIC:</label>
                    <asp:Label ID="SaldoCIC" runat="server" ClientIDMode="Static" CssClass="formDato numerico"></asp:Label>
                    <%--<asp:TextBox ID="SaldoCIC" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static"></asp:TextBox>--%>
                </asp:Panel>


            </fieldset>
        </asp:Panel>
        
        <asp:Panel ID="DatosGrupoFamiliar" runat="server" ClientIDMode="Static" Visible="False">
            <br />
            <fieldset>
                <legend>Grupo Familiar</legend>

                 <div class="formLinea" align="right">
                    <asp:HyperLink ID="NuevoBeneficiario" CssClass="boton darkblue sharp" style="width:80px;height:22px" runat="server" ClientIDMode="Static" Visible="False">Nuevo</asp:HyperLink>
                    <asp:HiddenField ID="PerNuevoBeneficiario" runat="server" ClientIDMode="Static" />
                </div>

                <cwrv:TablaGrupoFamiliar runat="server" ID="TablaGrupoFamiliar" />
            </fieldset>
            <a name="grupo_familiar"></a>
        </asp:Panel>
        
        <asp:Panel ID="DatosSimulacion" runat="server" ClientIDMode="Static" Visible="False">
            <br />
            <fieldset>
                <legend>Datos de la simulación</legend>
                <div class="formLinea">
                    <label id="LabTipoRenta" for="TipoRenta" class="formLabel formLabel2Izq">Tipo de Renta*:</label>
                    <asp:DropDownList ID="TipoRenta" runat="server" CssClass="formCombobox" Width="185" ClientIDMode="Static" Enabled="True">
                    </asp:DropDownList>


                    <div id="divTemporalidad">
                        <label id="LabTemporalidad" for="Temporalidad" class="formLabel formLabel2Der">Temporalidad*:</label>
                        <asp:DropDownList ID="Temporalidad" runat="server" CssClass="formCombobox" Width="180" ClientIDMode="Static" Enabled="True">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="formLinea">
                    <label id="LabMoneda" for="Moneda" class="formLabel formLabel2Izq">Moneda*:</label>
                    <asp:DropDownList ID="Moneda" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static" Enabled="True">
                    </asp:DropDownList>

                    <label id="LabPensionRequerida" for="PensionRequerida" class="formLabel formLabel2Der">Pensión Requerida*:</label>
                    <asp:TextBox ID="PensionRequerida" runat="server" CssClass="formTextbox numerico" Width="172" ClientIDMode="Static" data-v-min="0"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabPeriodoGarantizado" for="PeriodoGarantizado" class="formLabel formLabel2Izq">Período Garantizado*:</label>
                    <asp:DropDownList ID="PeriodoGarantizado" runat="server" CssClass="formCombobox" Width="186" ClientIDMode="Static" Enabled="True">
                    </asp:DropDownList>

                    
                </div>

                <div class="formLinea" align="center">
                    <%--<asp:HyperLink ID="Calcular"  CssClass="boton darkblue sharp" style="width:80px;height:22px" runat="server" ClientIDMode="Static" Visible="True">Calcular</asp:HyperLink>--%>
                    <a id="Calcular" href="javascript:void(0);" style="width:80px;height:22px" class="boton darkblue sharp">Calcular</a>

                    <asp:HiddenField ID="PerCalcular" runat="server" ClientIDMode="Static" />
                </div>


                <%--<cwrv:TablaCapitalRequerido runat="server" ID="TablaCapitalRequerido" />--%>

                <div id="TablaCapitalRequeridoCargando" align="center" style="height:50px;padding:82px 0">
                    <asp:Image ID="icoTablaCapitalRequeridoCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                    <span class="texto">Calculando capital requerido, espere por favor...</span>
                </div>
                <div id="TablaCapitalRequeridoContenedor" style="display:none"></div>


                <div class="formLinea" align="right" id="divImprimir">
                    <asp:HyperLink ID="btnEnviarEmail" CssClass="boton darkblue sharp" style="width:115px;height:22px" runat="server" ClientIDMode="Static" Visible="True">Enviar Correo</asp:HyperLink>
                    <%--<a id="btnEnviarEmail" href="javascript:void(0);" style="width:80px;height:22px" class="boton darkblue sharp">Enviar Email</a>--%>
                    <asp:HiddenField ID="PerbtnEnviarEmail" runat="server" ClientIDMode="Static" />

                    <asp:HyperLink ID="btnImprimir" CssClass="boton darkblue sharp" style="width:80px;height:22px" runat="server" ClientIDMode="Static" Visible="True">Imprimir</asp:HyperLink>
                    <%--<a id="btnImprimir" href="javascript:void(0);" style="width:80px;height:22px" class="boton darkblue sharp">Imprimir</a>--%>
                    <asp:HiddenField ID="PerbtnImprimir" runat="server" ClientIDMode="Static" />
                    
                    
                    
                </div>
                <p></p>
                <div align="left" id="divNota">
                    <label id="LabNota1" class="formLabel2Izq">* Los resultados son aproximados y pueden variar debido a las variaciones en las tasas, moneda de la pensión, o beneficiarios.</label>
                    <br />
                    <label id="LabNota2" class="formLabel2Izq">* Tipo de cambio referencial</label>
                    <br />
                    <label id="LabNota3" class="formLabel2Izq">* El fondo se estima considerando una Renta de Jubilación Inmediata.</label>
                </div>

            </fieldset>
        
        
            

        </asp:Panel>
    </div>

    <%--Ventanas Modales--%>

    <div style="display:none">
        <%--Inicio Modal Envío de Correo Simulador--%>
        <div  id="ModalEnvioCorreoSimulador" title="Enviar Correo Electrónico">
            <div id="ModEnvCorCargandoSimulador" class="modalCargandoContenido" style="width:526px;height:350px"></div>
            <div id="ModEnvCorContenidoSimulador" class="modalContenido">
               
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
                    
                    <label id="ModEnvCorAdjunto" class="formLabelAdjunto">Cotizacion.pdf (189kb)</label>
                </div>

                <div class="formLinea" style="height:210px">
                    <asp:TextBox ID="ModEnvCorMensaje" runat="server" CssClass="formTextbox" style="margin-left:0px;height:200px" Width="560" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModEnvCorEnviarSimuladorJubilarseHoyFuturo" style="width:80px;height:22px" class="boton darkblue sharp">Enviar</a>
                    <a id="ModEnvCorCancelar" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
                </div>
            </div>
        </div>
        <%--Fin Modal Envío de Correo Simulador--%>

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
                <a id="MCAAceptar" class="boton darkblue sharp" style="width:80px">Aceptar</a>
                <a id="MCACancelar" class="boton darkblue sharp" style="width:80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Cuadro de advertencia--%>
    
    
        <%--Inicio Modal Calculando--%>
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



        <%--Inicio Modal Envío de Correo--%>
        <div id="ModalEnvioCorreoCapital" title="Enviar Correo Electrónico">
            <div id="ModEnvCorCargandoCapital" class="modalCargandoContenido" style="width:526px;height:350px"></div>
            <div id="ModEnvCorContenidoCapital" class="modalContenido">
                <asp:HiddenField ID="ModEnvCorModoCapital" runat="server" ClientIDMode="Static" />

                <div class="formLinea">
                    <label id="LabModEnvCorDeCapital" class="formLabel formLabel2Izq">De:</label>
                    <asp:TextBox ID="ModEnvCorDeCapital" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorParaCapital" class="formLabel formLabel2Izq">Para:</label>
                    <asp:TextBox ID="ModEnvCorParaCapital" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAsuntoCapital" class="formLabel formLabel2Izq">Asunto:</label>
                    <asp:TextBox ID="ModEnvCorAsuntoCapital" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" style="margin-left:-188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAdjuntoCapital" class="formLabel formLabel2Izq">Adjunto:</label>
                    <label id="ModEnvCorAdjuntoCapital" class="formLabelAdjunto">Cotizacion.pdf (189kb)</label>
                </div>

                <div class="formLinea" style="height:210px">
                    <asp:TextBox ID="ModEnvCorMensajeCapital" runat="server" CssClass="formTextbox" style="margin-left:0px;height:200px" Width="560" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModEnvCorEnviarCapital" style="width:80px;height:22px" class="boton darkblue sharp">Enviar</a>
                    <a id="ModEnvCorCancelarCapital" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
                </div>
            </div>
        </div>
        <%--Fin Modal Envío de Correo--%>

        
    
    </div>
</asp:Content>
