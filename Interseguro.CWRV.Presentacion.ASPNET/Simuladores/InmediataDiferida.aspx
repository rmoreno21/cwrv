<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="InmediataDiferida.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Simuladores.InmediataDiferida" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script type="text/javascript">
        google.load('visualization', '1', { packages: ['corechart'], language: 'ja' });
    </script>
    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.eventos.simuladores.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <div align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">
        <h1 class="simple" style="width:180px">Búsqueda de afiliados</h1>

        <asp:HiddenField ID="HBusAfiNroSolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HBusAfiCUSPP" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="BusAfiDatosCargados" runat="server" ClientIDMode="Static" Value="0" />
        <asp:HiddenField ID="PerBusAfiBuscar" runat="server" ClientIDMode="Static" />
        
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
                <asp:HyperLink ID="SimBusAfiBuscar" NavigateUrl="#" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
            </div>
        </asp:Panel>

        <asp:HiddenField ID="IdSimulador" runat="server" ClientIDMode="Static" Value="" />
        <asp:HiddenField ID="CUSPP" runat="server" ClientIDMode="Static" />
        <!--SRI.INI-20322-->
        <asp:HiddenField ID="HRutaImagenSimulada" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="HCorrelativoSolicitud" runat="server" ClientIDMode="Static" />
        <!--SRI.FIN-20322-->

        <div id="TablaSolicitudesCargando" align="center" style="display:none">
            <asp:Image ID="icoTablaSolicitudesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
            <span class="texto">Cargando solicitudes, espere por favor...</span>
        </div>
        <div id="TablaSolicitudesContenedor" style="display:none"></div>
        <div id="TablaSolicitudesError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesSimuladorReintentar">Intentar de nuevo</a>.</div>
    </div>

    <br />

    <div align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">
        <h1 id="tituloSimulador" class="simple" style="width:180px">¿Inmediata o Diferida?</h1>

        <div class="formLinea">
            <label id="LabInflacion" class="formLabel formLabel2Izq">Inflación anual*:</label>
            <asp:TextBox ID="Inflacion" runat="server" CssClass="formTextbox formTextboxReadOnly numerico" Width="60" data-v-max="100.000" data-v-min="-100.000" style="margin-left:-40px" ClientIDMode="Static" Enabled="False"></asp:TextBox>
            <span style="display:block;height:17px;padding-top:3px"><a target="_blank" href="http://www.bcrp.gob.pe" class="formLink">www.bcrp.gob.pe</a></span>
        </div>

        <div class="formLinea">
            <label id="LabTasaAjuste" class="formLabel formLabel2Izq">Tasa de ajuste fija anual*:</label>
            <asp:TextBox ID="TasaAjuste" runat="server" CssClass="formTextbox formTextboxReadOnly numerico" Width="60" data-v-max="100.000" data-v-min="-100.000" style="margin-left:-40px" ClientIDMode="Static" Enabled="False"></asp:TextBox>
        </div>

        <div class="formLinea">
            <label id="LabMoneda" class="formLabel formLabel2Izq">Moneda*:</label>
            <asp:DropDownList ID="Moneda" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="68" style="margin-left:-40px" ClientIDMode="Static" Enabled="False">
            </asp:DropDownList>
        </div>

        <div class="formLinea">
            <label id="LabPeriodoGarantizado" class="formLabel formLabel2Izq">Período Garantizado*:</label>
            <asp:DropDownList ID="PeriodoGarantizado" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="68" style="margin-left:-40px" ClientIDMode="Static" Enabled="False">
            </asp:DropDownList>
        </div>

        <%--SRI.INI-20322--%>
        <div class="formLineaBotonSimulacion" align="left" style="margin:0">
            <asp:HyperLink ID="btnRecalcularInmediataDiferida" NavigateUrl="#" style="width:150px;height:22px;display:none" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Recalcular Gráfico</asp:HyperLink>
        </div>
        <%--SRI.INI-20322--%>

        <div class="formLinea">
            <label id="LabCotizaciones" class="formLabel formLabel2Izq">Cotizaciones:</label>
        </div>

        <div id="DatosSimulacion" style="display:none">
            <table width="100%" cellspacing="1" cellpadding="3" border="0">
                <tr class="grilla_cabecera">
                    <th>CUSPP</th>
                    <th>Afiliado</th>
                    <th>Solicitud</th>
                    <th>Agente</th>
                    <th>Fecha y hora</th>
                </tr>
                <tr class="grilla_alt1">
                    <td><asp:Label ID="DSCuspp" runat="server" ClientIDMode="Static"></asp:Label></td>
                    <td><asp:Label ID="DSAfiliado" runat="server" ClientIDMode="Static"></asp:Label></td>
                    <td><asp:Label ID="DSSolicitud" runat="server" ClientIDMode="Static"></asp:Label></td>
                    <td><asp:Label ID="DSAgente" runat="server" ClientIDMode="Static"></asp:Label></td>
                    <td><asp:Label ID="DSFechaHora" runat="server" ClientIDMode="Static"></asp:Label></td>
                </tr>
            </table>
        </div>

        <div id="CotizacionesSimulador">
            <table id="GrupoCotizaciones" width="860" border="0" cellspacing="0" cellpadding="0" style="display:none">
                <tr class="grilla_cabecera">
                    <th align="center" style="width:286px;border-left:1px solid #FFF;border-right:1px solid #FFF">INMEDIATA</th>
                    <th align="center" style="width:287px;border-left:1px solid #FFF;border-right:1px solid #FFF">TEMPORAL 1 AÑO</th>
                    <th align="center" style="width:287px;border-left:1px solid #FFF;border-right:1px solid #FFF">TEMPORAL 2 AÑOS</th>
                </tr>
                
                <tr>
                    <td valign="top">
                        <div id="TablaCotizaciones1NoSolicitud" class="grilla_info">Seleccione una solicitud para mostrar sus respectivas cotizaciones.</div>
                        <div id="TablaCotizaciones1Cargando" align="center" style="display:none;padding-top:30px">
                            <asp:Image ID="icoTablaCotizaciones1Cargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando cotizaciones, espere por favor...</span>
                        </div>
                        <div id="TablaCotizaciones1Contenedor" style="display:none"></div>
                        <div id="TablaCotizaciones1Error" class="grilla_error" style="display:none">No se ha podido cargar la tabla de cotizaciones. <a id="TablaCotizaciones1SimuladorReintentar">Intentar de nuevo</a>.</div>
                    </td>
                    <td valign="top">
                        <div id="TablaCotizaciones2NoSolicitud" class="grilla_info">Seleccione una solicitud para mostrar sus respectivas cotizaciones.</div>
                        <div id="TablaCotizaciones2Cargando" align="center" style="display:none;padding-top:30px">
                            <asp:Image ID="icoTablaCotizaciones2Cargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando cotizaciones, espere por favor...</span>
                        </div>
                        <div id="TablaCotizaciones2Contenedor" style="display:none"></div>
                        <div id="TablaCotizaciones2Error" class="grilla_error" style="display:none">No se ha podido cargar la tabla de cotizaciones. <a id="TablaCotizaciones2SimuladorReintentar">Intentar de nuevo</a>.</div>
                    </td>
                    <td valign="top">
                        <div id="TablaCotizaciones3NoSolicitud" class="grilla_info">Seleccione una solicitud para mostrar sus respectivas cotizaciones.</div>
                        <div id="TablaCotizaciones3Cargando" align="center" style="display:none;padding-top:30px">
                            <asp:Image ID="icoTablaCotizaciones3Cargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando cotizaciones, espere por favor...</span>
                        </div>
                        <div id="TablaCotizaciones3Contenedor" style="display:none"></div>
                        <div id="TablaCotizaciones3Error" class="grilla_error" style="display:none">No se ha podido cargar la tabla de cotizaciones. <a id="TablaCotizaciones3SimuladorReintentar">Intentar de nuevo</a>.</div>
                    </td>
                </tr>
                <%--SRI.INI-20322--%>
                <tr class="grilla_cabecera">
                    <th align="center" style="width:286px;border-left:1px solid #FFF;border-right:1px solid #FFF">TEMPORAL 3 AÑOS</th>
                    <th align="center" style="width:287px;border-left:1px solid #FFF;border-right:1px solid #FFF">TEMPORAL 4 AÑOS</th>
                    <th align="center" style="width:287px;border-left:1px solid #FFF;border-right:1px solid #FFF">TEMPORAL 5 AÑOS</th>
                </tr>
                <tr>
                    <td valign="top">
                        <div id="TablaCotizaciones4NoSolicitud" class="grilla_info">Seleccione una solicitud para mostrar sus respectivas cotizaciones.</div>
                        <div id="TablaCotizaciones4Cargando" align="center" style="display:none;padding-top:30px">
                            <asp:Image ID="Image4" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando cotizaciones, espere por favor...</span>
                        </div>
                        <div id="TablaCotizaciones4Contenedor" style="display:none"></div>
                        <div id="TablaCotizaciones4Error" class="grilla_error" style="display:none">No se ha podido cargar la tabla de cotizaciones. <a id="TablaCotizaciones4SimuladorReintentar">Intentar de nuevo</a>.</div>
                    </td>
                    <td valign="top">
                        <div id="TablaCotizaciones5NoSolicitud" class="grilla_info">Seleccione una solicitud para mostrar sus respectivas cotizaciones.</div>
                        <div id="TablaCotizaciones5Cargando" align="center" style="display:none;padding-top:30px">
                            <asp:Image ID="Image5" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando cotizaciones, espere por favor...</span>
                        </div>
                        <div id="TablaCotizaciones5Contenedor" style="display:none"></div>
                        <div id="TablaCotizaciones5Error" class="grilla_error" style="display:none">No se ha podido cargar la tabla de cotizaciones. <a id="TablaCotizaciones5SimuladorReintentar">Intentar de nuevo</a>.</div>
                    </td>
                    <td valign="top">
                        <div id="TablaCotizaciones6NoSolicitud" class="grilla_info">Seleccione una solicitud para mostrar sus respectivas cotizaciones.</div>
                        <div id="TablaCotizaciones6Cargando" align="center" style="display:none;padding-top:30px">
                            <asp:Image ID="Image6" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando cotizaciones, espere por favor...</span>
                        </div>
                        <div id="TablaCotizaciones6Contenedor" style="display:none"></div>
                        <div id="TablaCotizaciones6Error" class="grilla_error" style="display:none">No se ha podido cargar la tabla de cotizaciones. <a id="TablaCotizaciones6SimuladorReintentar">Intentar de nuevo</a>.</div>
                    </td>
                </tr>
                <%--SRI.FIN-20322--%>
            </table>
        </div>
        
        <%--SRI.INI-20322--%>
        <%--<div id="Simulaciones" style="display:none;height:603px">--%>
        <div id="Simulaciones" style="display:none;height:700px">
        <%--SRI.FIN-20322--%>
            <%--SRI.INI-20322--%>
            <%--<div id="Simulacion" style="margin:10px 2px;width:850px;height:593px;" class="areaSimulacion">--%>
            <div id="Simulacion" style="margin:10px 2px;width:850px;height:690px;" class="areaSimulacion">
            <%--SRI.FIN-20322--%>
            </div>
        </div>

        <%--SRI.INI-20322--%>
        <div class="formLineaBotonSimulacion" align="center">
            <asp:HyperLink ID="btnNuevoCorreoInmediataDiferida" style="width:150px;height:22px;display:none" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Enviar Correo</asp:HyperLink>
            <asp:HyperLink ID="btnPensionProyectadaInmediataDiferida" style="width:150px;height:22px;display:none" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Pensión Proyectada</asp:HyperLink>
        </div>

        <div id="ModalPensionProyectadaCargando" title="Generando reporte" align="center" style="display:none">
            <asp:Image ID="Image1" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" />
            <span class="texto">Calculando pensión proyectada, espere por favor...</span>
        </div>
        <%--SRI.INI-20322--%>

    </div>

    <br />

    <%--Ventanas Modales--%>

    <div style="display:none">
        <%--SRI.INI-20322--%>
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
                    <a id="ModEnvCorEnviarSimuladorInmediataDiferida" style="width:80px;height:22px" class="boton darkblue sharp">Enviar</a>
                    <a id="ModEnvCorCancelar" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
                </div>
            </div>
        </div>
        <%--Fin Modal Envío de Correo Simulador--%>
        <%--SRI.FIN-20322--%>

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
    
    </div>
</asp:Content>
