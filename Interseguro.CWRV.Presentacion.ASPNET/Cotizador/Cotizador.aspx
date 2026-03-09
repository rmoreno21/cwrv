<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="Cotizador.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.Cotizador" %>

<%@ Register Src="~/Controles/ConsentimientoMensaje.ascx" TagPrefix="uc1" TagName="ConsentimientoMensaje" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">

    <script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.load.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiCotizadorRV.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>    
    <script type="text/javascript">

        $(document).ready(function () {

            if ($('#SaldoCIC').val() != 0) $('#SaldoCIC').val(formatearMonto($('#SaldoCIC').val()));
            if ($('#ModSolSaldoCIC').val() != 0) $('#ModSolSaldoCIC').val(formatearMonto($('#ModSolSaldoCIC').val()))

        });

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

    <asp:Panel ID="BusquedaAfiliados" runat="server" ClientIDMode="Static" align="left" Style="width: 860px; padding: 20px; background: #FFF; border: 1px solid #00466e">
        <h1 class="simple" style="width: 130px">Rentas Vitalicias</h1>

        <fieldset>
            <legend>Búsqueda de afiliados</legend>
            <div class="formLinea">
                <label id="LabBusAfiNroSolicitud" for="BusAfiNroSolicitud" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                <asp:TextBox ID="BusAfiNroSolicitud" runat="server" CssClass="formTextbox" Style="text-transform: uppercase" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                <label id="LabBusAfiCUSPP" for="BusAfiCUSPP" class="formLabel formLabel2Der">CUSPP:</label>
                <asp:TextBox ID="BusAfiCUSPP" runat="server" CssClass="formTextbox alfanumerico" Style="text-transform: uppercase" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>

                <asp:HyperLink ID="BusAfiExaminarSolicitud" CssClass="boton darkblue sharp" Style="width: 22px; height: 22px; position: relative; margin-left: 5px" runat="server" ClientIDMode="Static">...</asp:HyperLink>
                <asp:HiddenField ID="PerBusAfiExaminarSolicitud" runat="server" ClientIDMode="Static" />
            </div>

            <asp:Panel ID="LineaBuscarAfiliado" runat="server" ClientIDMode="Static" CssClass="formLinea" align="center">
                <asp:Button ID="BusAfiBuscar" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Buscar" OnClick="BusAfiBuscar_Click"
                    ClientIDMode="Static" />
                <asp:HiddenField ID="PerBusAfiBuscar" runat="server" ClientIDMode="Static" />
            </asp:Panel>
        </fieldset>
    </asp:Panel>

    <br />

    <div id="Pestanhas" style="width: 900px">
        <ul>
            <li id="Pes1" data-pestanha="1"><a href="#">Datos del Afiliado</a></li>
            <li id="Pes2" data-pestanha="2"><a href="#">Grupo Familiar</a></li>
            <li id="Pes3" data-pestanha="3"><a href="#">Datos Solicitud</a></li>
            <li id="Pes4" data-pestanha="4"><a href="#">Aporte Adicional</a></li>
        </ul>
        <asp:HiddenField ID="hdnMostrarAporteAdicional" runat="server" Value="0" ClientIDMode="Static" />
        <asp:HiddenField ID="PestanhaActiva" runat="server" Value="1" ClientIDMode="Static" />
           
        <input type="hidden" id="usuario_actual" value="<%= Session["Usuario"] %>" />Add commentMore actions
		<input type="hidden" id="rol_azman" value="<%= Session["RolAzman"] %>" />
		<input type="hidden" id="url_api_rentas_rv" value="<%= System.Configuration.ConfigurationManager.AppSettings["ApiCotizadorRvUrl"] %>" />

        <div id="Pestanha1" align="left" style="display: none; width: 860px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">

            <div class="grilla_info">Para actualizar los Datos del Afiliado (con excepción de la AFP y la CIC) por favor use el <b>vtiger</b>.</div>
            <br />

            <asp:Panel ID="PanelConsentimiento" runat="server" ClientIDMode="Static">
            </asp:Panel>
            <%--<SRIFIN06326>--%>
            <%--Ventanas Modales--%>            <%--Inicio Modal Búsqueda de Afiliados--%>            <%--Control de fecha: <asp:TextBox ID="EjemploFecha" runat="server" CssClass="formCalendar" ClientIDMode="Static" ReadOnly="true"></asp:TextBox>--%>            <%--Fin Modal Búsqueda de Afiliados--%>

            <br />

            <asp:HiddenField ID="HCUSPP" runat="server" ClientIDMode="Static" />
            <asp:Panel ID="LineaCUSPP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabCUSPP" for="CUSPP" class="formLabel formLabel2Izq">CUSPP:</label>
                <asp:TextBox ID="CUSPP" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="240" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
            </asp:Panel>

            <asp:HiddenField ID="HToken" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HidConsentimientoAsesoria" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HidConfiguracion" runat="server" ClientIDMode="Static" />
            <asp:HiddenField ID="HindConsentimiento" runat="server" ClientIDMode="Static" />

            <asp:Panel ID="LineaTipoDocumento" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabTipoDocumento" for="TipoDocumento" class="formLabel formLabel2Izq">Tipo de Documento*:</label>
                <asp:DropDownList ID="TipoDocumento" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="244" ClientIDMode="Static" Enabled="False"></asp:DropDownList>

                <label id="LabNumeroDocumento" for="NumeroDocumento" class="formLabel formLabel2Der">Número de Documento*:</label>
                <asp:TextBox ID="NumeroDocumento" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="LineaApellidos" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabApellidoPaterno" for="ApellidoPaterno" class="formLabel formLabel2Izq">Apellido Paterno:</label>
                <asp:TextBox ID="ApellidoPaterno" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="238" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                <label id="LabApellidoMaterno" for="ApellidoMaterno" class="formLabel formLabel2Der">Apellido Materno:</label>
                <asp:TextBox ID="ApellidoMaterno" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="LineaNombres" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabNombres" for="Nombres" class="formLabel formLabel2Izq">Nombres:</label>
                <asp:TextBox ID="Nombres" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="620" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="LineaNacimientoSexo" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabFechaNacimiento" for="FechaNacimiento" class="formLabel formLabel2Izq">Fecha de nacimiento:</label>
                <asp:TextBox ID="FechaNacimiento" runat="server" CssClass="formTextbox formCalendarReadOnly formTextboxLetra ColorNegro" Width="240" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                <label id="LabSexo" for="Sexo" class="formLabel formLabel2Der">Sexo:</label>
                <asp:DropDownList ID="Sexo" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="186" ClientIDMode="Static" Enabled="False">
                </asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="LineaCorreoElectronico" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabCorreoElectronico" for="CorreoElectronico" class="formLabel formLabel2Izq">Correo Electrónico*:</label>
                <asp:TextBox ID="CorreoElectronico" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="240" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                <asp:HiddenField ID="CorreoElectronicoRegistrado" runat="server" ClientIDMode="Static" />
            </asp:Panel>

            <asp:HiddenField ID="HCategoria" runat="server" ClientIDMode="Static" Value="0" />
            <asp:Panel ID="LineaCategoria" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabCategoria" for="Categoria" class="formLabel formLabel2Izq">Categoría*:</label>
                <asp:DropDownList ID="Categoria" runat="server" CssClass="formComboboxTexto formTextboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                </asp:DropDownList>
            </asp:Panel>

            <asp:HiddenField ID="HAFP" runat="server" ClientIDMode="Static" Value="0" />
            <asp:Panel ID="LineaAFP" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabAFP" for="AFP" class="formLabel formLabel2Izq">AFP*:</label>
                <asp:DropDownList ID="AFP" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                </asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="LineaSaldoCIC" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabSaldoCIC" for="SaldoCIC" class="formLabel formLabel2Izq">Saldo CIC*:</label>
                <asp:TextBox ID="SaldoCIC" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="LineaTelefonos" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabTelefono" for="Telefono" class="formLabel formLabel2Izq">Teléfono:</label>
                <asp:TextBox ID="Telefono" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono" Width="240" ClientIDMode="Static" ReadOnly="True" MaxLength="12"></asp:TextBox>

                <label id="LabCelular" for="Celular" class="formLabel formLabel2Der">Celular:</label>
                <asp:TextBox ID="Celular" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro telefono" Width="180" ClientIDMode="Static" ReadOnly="True" MaxLength="9"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="LineaAgente" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="LabAgente" for="Agente" class="formLabel formLabel2Izq">Agente:</label>
                <asp:TextBox ID="Agente" runat="server" CssClass="formTextbox formTextboxReadOnly formTextboxLetra ColorNegro" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                <asp:HiddenField ID="NombreAgente" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="NumeroAgente" runat="server" ClientIDMode="Static" />
                <asp:HiddenField ID="Cartera" runat="server" ClientIDMode="Static" />
            </asp:Panel>

            <%--Inicio Modal Mantenimiento de direcciones--%>

            <br />

            <asp:Panel ID="GrupoDireccion" runat="server" ClientIDMode="Static">
                <div id="AgrupadorDireccion" class="agrupador">
                    <span class="agrupador_titulo_mas">Dirección</span>
                </div>

                <div id="DatosDireccion" style="display: none">
                    <div class="formLinea" align="right">
                        <asp:HyperLink ID="NuevaDireccion" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px" runat="server" ClientIDMode="Static" Visible="False">Nueva</asp:HyperLink>
                        <asp:HiddenField ID="PerNuevaDireccion" runat="server" ClientIDMode="Static" />
                    </div>

                    <div id="TablaDireccionesCargando" align="center" style="display: none">
                        <asp:Image ID="icoTablaDireccionesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                        <span class="texto">Cargando direcciones, espere por favor...</span>
                    </div>
                    <div id="TablaDireccionesContenedor" style="display: none"></div>
                    <div id="TablaDireccionesError" class="grilla_error" style="display: none">No se ha podido cargar la tabla de direcciones. <a id="TablaDireccionesReintentar">Intentar de nuevo</a>.</div>
                    <input id="TabDireccionesDesplegar" type="hidden" value="0" />

                    <br />
                </div>
            </asp:Panel>

            <%--Fin Modal Mantenimiento de direcciones--%>
            <%--Inicio Modal Mantenimiento de teléfonos--%>

            <asp:Panel ID="ContenedorGuardar" CssClass="formLinea" align="center" runat="server" ClientIDMode="Static">
                <asp:Button ID="Guardar" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Guardar" ClientIDMode="Static"
                    OnClick="Guardar_Click" />
                <asp:HiddenField ID="PerGuardar" runat="server" ClientIDMode="Static" />

                <a id="Imprimir" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Imprimir</a>

            </asp:Panel>
        </div>

        <div id="Pestanha2" align="left" style="display: none; width: 860px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
            <div class="grilla_info">Antes de agregar nuevos beneficiarios, asegúrese de que el Afiliado ya figure en la lista, para ello guarde los datos del afiliado en la pestaña <strong>Datos del Afiliado</strong>.</div>

            <div class="formLinea" align="right">
                <asp:HyperLink ID="NuevoBeneficiario" CssClass="boton darkblue sharp" Style="width: 80px; height: 22px" runat="server" ClientIDMode="Static" Visible="False">Nuevo</asp:HyperLink>
                <asp:HiddenField ID="PerNuevoBeneficiario" runat="server" ClientIDMode="Static" />
            </div>

            <div id="TablaGrupoFamiliarCargando" align="center" style="display: none">
                <asp:Image ID="icoTablaGrupoFamiliarCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando grupo familiar, espere por favor...</span>
            </div>
            <div id="TablaGrupoFamiliarContenedor" style="display: none"></div>
            <div id="TablaGrupoFamiliarError" class="grilla_error" style="display: none">No se ha podido cargar la tabla de grupo familiar. <a id="TablaGrupoFamiliarReintentar">Intentar de nuevo</a>.</div>

            <br />
        </div>

        <div id="Pestanha3" align="left" style="display: none; width: 860px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
            <div class="formLinea" align="right">
                <asp:HyperLink ID="NuevaSolicitud" CssClass="boton darkblue sharp" Style="width: 150px" runat="server" ClientIDMode="Static" Visible="False">Nueva Extraoficial</asp:HyperLink>
                <asp:HiddenField ID="PerNuevaSolicitud" runat="server" ClientIDMode="Static" />
            </div>

            <div id="TablaSolicitudesCargando" align="center" style="display: none">
                <asp:Image ID="icoTablaSolicitudesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando solicitudes, espere por favor...</span>
            </div>
            <div id="TablaSolicitudesContenedor" style="display: none"></div>
            <div id="TablaSolicitudesError" class="grilla_error" style="display: none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesReintentar">Intentar de nuevo</a>.</div>

            <%--Fin Modal Mantenimiento de teléfonos--%>
            <div id="TablaReporteEscenarioCargado" align="center" style="display: none">
                <asp:Image ID="icoTablaReporteEscenarioCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Cargando reporte de escenarios, espere por favor...</span>
            </div>
            <%--Inicio Modal Mantenimiento de grupo familiar--%>
            <br />
        </div>

        <div id="Pestanha4" align="left" style="display: block; width: 860px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">

            <asp:Panel ID="pnlPensionRef" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="lblPensionRef" for="Nombres" class="formLabel formLabel2Izq">Pensión de Referencia:</label>
                <asp:TextBox ID="txtPensionRef" runat="server" CssClass="formTextbox numerico" Width="194" ClientIDMode="Static"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="pnlMonedaRef" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="lblMonedaRef" for="Nombres" class="formLabel formLabel2Izq">Moneda de Referencia:</label>
                <asp:DropDownList ID="ddlMonedaReferencia" runat="server" CssClass="formCombobox" Width="200" ClientIDMode="Static"></asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="pnlTasaAporte" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="lblTasaAporte" for="Nombres" class="formLabel formLabel2Izq">Tasa del Aporte:</label>
                <asp:TextBox ID="txtTasaAporte" runat="server" CssClass="formTextbox numerico" Width="194" ClientIDMode="Static"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="pnlFechaPagoAporte" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="lblFechaPagoAporte" for="Nombres" class="formLabel formLabel2Izq">Fecha de Pago del Aporte:</label>
                <asp:TextBox ID="dtpFechaPago" runat="server" CssClass="fecha formTextbox formCalendar" Width="194" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="pnlMontoAporte" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="lblMontoAporte" for="Nombres" class="formLabel formLabel2Izq">Monto del Aporte:</label>
                <asp:TextBox ID="txtMontoAporte" runat="server" CssClass="formTextbox numerico" Width="194" ClientIDMode="Static"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="pnlPensionPago" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="lblPensionPago" for="Nombres" class="formLabel formLabel2Izq">Pensión Pago:</label>
                <asp:TextBox ID="txtPensionPago" runat="server" CssClass="formTextbox formTextboxReadOnly numerico" Width="194" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
            </asp:Panel>

            <asp:Panel ID="pnlMonedaPensionPago" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="lblMonedaPensionPago" for="Nombres" class="formLabel formLabel2Izq">Moneda Pensión Pago:</label>
                <asp:DropDownList ID="ddlMonedaPensionPago" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="200" ClientIDMode="Static" Enabled="false"></asp:DropDownList>
            </asp:Panel>

            <asp:Panel ID="pnlPensionElegida" runat="server" ClientIDMode="Static" CssClass="formLinea">
                <label id="lblPensionElegida" for="Nombres" class="formLabel formLabel2Izq">Pensión Elegida:</label>
                <asp:TextBox ID="txtPensionElegida" runat="server" CssClass="formTextbox formTextboxReadOnly numerico" Width="194" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
            </asp:Panel>

            <br />

            <asp:Panel ID="pnlControlAporte" CssClass="formLinea" align="center" runat="server" ClientIDMode="Static">
                <asp:Button ID="btnGuardarAporte" CssClass="boton darkblue sharp" Width="80"
                    Height="24" runat="server" Text="Guardar" ClientIDMode="Static"
                    OnClick="Guardar_Click" />
                <a id="btnEliminarAporte" class="boton darkblue sharp" style="width: 80px; height: 22px">Eliminar</a>
            </asp:Panel>

        </div>

    </div>

    <br />

    <%--<div id="ModGruFamCargando" class="modalCargandoContenido" style="width:748px;height:230px"></div>--%>

    <%--<GTI.INI-29372>--%>
    <div id="ModalAdvertenciaAporte">
        <div>
            <table width="100%" border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td id="MAAIcono" style="width: 40px; height: 40px"></td>
                    <td id="MAAContenedorMensaje" valign="middle" class="cuadroMensaje">
                        <asp:Panel ID="MAAContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                            <asp:HiddenField ID="MAAEstado" runat="server" ClientIDMode="Static" Value="0" />
                            <asp:HiddenField ID="MAAEstadoIcono" runat="server" ClientIDMode="Static" />
                            <asp:HiddenField ID="MAAEstadoTitulo" runat="server" ClientIDMode="Static" />
                            <asp:Literal ID="MAAMensaje" runat="server"></asp:Literal>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </div>
        <div id="MAABotonera" align="center">
            <asp:Button ID="MAAEliminar" CssClass="boton darkblue sharp" Width="80"
                Height="24" runat="server" Text="Aceptar" ClientIDMode="Static" OnClick="MAAEliminar_Click" UseSubmitBehavior="False" data-dismiss="modal" />
            <a id="MAACancelar" class="boton darkblue sharp" style="width: 80px; height: 22px;">Cancelar</a>
        </div>
    </div>
    <%--<GTI.FIN-29372>--%>

    <div style="display: none">

        <%--<div id="ModGruFamCargando" class="modalCargandoContenido" style="width:748px;height:130px"></div>--%>
        <div id="ModalBusquedaAfiliados" title="Búsqueda de afiliados">
            <div class="formLinea">
                <label id="LabModBusAfiApellidoPaterno" for="ModBusAfiApellidoPaterno" class="formLabel formLabel2Izq">Apellido Paterno:</label>
                <asp:TextBox ID="ModBusAfiApellidoPaterno" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static"></asp:TextBox>

                <label id="LabModBusAfiApellidoMaterno" for="ModBusAfiApellidoMaterno" class="formLabel formLabel2Der">Apellido Materno:</label>
                <asp:TextBox ID="ModBusAfiApellidoMaterno" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static"></asp:TextBox>
            </div>

            <div class="formLinea">
                <label id="LabModBusAfiNombres" for="ModBusAfiNombres" class="formLabel formLabel2Izq">Nombres:</label>
                <asp:TextBox ID="ModBusAfiNombres" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static"></asp:TextBox>
            </div>

            <div class="formLinea" align="center">
                <asp:HyperLink ID="ModBusAfiBuscar" NavigateUrl="#" Style="width: 80px; height: 22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
                <a id="ModBusAfiCancelar" href="#" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
            </div>

            <br />

            <div id="ModBusAfiCargando" align="center" style="display: none">
                <asp:Image ID="icoModBusAfiCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                <span class="texto">Buscando afiliados, espere por favor...</span>
            </div>
            <div id="TablaAfiliadosError" align="center" style="display: none">
                <div class="grilla_error" align="left" style="width: 365px;">
                    No se ha podido cargar la tabla de afiliados. <a id="TablaAfiliadosReintentar" href="#">Intentar de nuevo</a>.
                </div>
            </div>

            <div id="ModBusAfiTablaAfiliados" runat="server" style="display: none" clientidmode="Static"></div>

            <asp:HiddenField ID="TabAfiliadosIndicePagina" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosTamanhoPagina" runat="server" ClientIDMode="Static" Value="10" />
            <asp:HiddenField ID="TabAfiliadosColumnaOrdenar" runat="server" ClientIDMode="Static" Value="1" />
            <asp:HiddenField ID="TabAfiliadosDireccionOrdenar" runat="server" ClientIDMode="Static" Value="A" />

            <%--Fin Modal Mantenimiento de grupo familiar--%>
        </div>
        <%--Inicio Modal Mantenimiento de solicitud--%>
        <%--SRI.INI-20322--%>
        <div id="ModalDireccion" title="Dirección del afiliado">
            <div id="ModDirCargando" class="modalCargandoContenido" style="width: 611px; height: 160px"></div>
            <div id="ModDirContenido" class="modalContenido">
                <asp:HiddenField ID="ModDirModo" runat="server" ClientIDMode="Static" />

                <div class="formLinea">
                    <label id="LabModDomicilio" for="ModDomicilio" class="formLabel formLabel2Izq">Tipos de Vía*:</label>
                    <asp:DropDownList ID="ModDomicilio" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static" ReadOnly="True">
                    </asp:DropDownList>

                </div>

                <div class="formLinea">
                    <label id="LabModDirDireccion" for="ModDirDireccion" class="formLabel formLabel2Izq">Dirección*:</label>
                    <asp:TextBox ID="ModDirDireccion" runat="server" CssClass="formTextbox" Width="480" ClientIDMode="Static" MaxLength="80"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModDirEspacioUrbano" for="ModDirEspacioUrbano" class="formLabel formLabel2Izq">N°/Mz/Lt/Dpto.*:</label>
                    <asp:TextBox ID="ModDirEspacioUrbano" runat="server" CssClass="formTextbox" Width="110" ClientIDMode="Static" MaxLength="80"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModDirDepartamento" for="ModDirDepartamento" class="formLabel formLabel2Izq">Departamento*:</label>
                    <asp:DropDownList ID="ModDirDepartamento" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static" ReadOnly="True">
                    </asp:DropDownList>
                </div>

                <div id="ModDirControlCiudad" class="formLinea">
                    <label id="LabModDirCiudad" for="ModDirCiudad" class="formLabel formLabel2Izq">Provincia*:</label>
                    <asp:DropDownList ID="ModDirCiudad" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static" Enabled="False">
                    </asp:DropDownList>
                    <span id="ModDirCargandoCiudad" class="paginador_cargando"></span>
                </div>

                <div id="ModDirControlComuna" class="formLinea">
                    <label id="LabModDirComuna" for="ModDirComuna" class="formLabel formLabel2Izq">Distrito*:</label>
                    <asp:DropDownList ID="ModDirComuna" runat="server" CssClass="formCombobox" Width="250" ClientIDMode="Static" Enabled="False">
                    </asp:DropDownList>
                    <span id="ModDirCargandoComuna" class="paginador_cargando"></span>
                </div>

                <div class="formLinea">
                    <label id="LabModDirPrincipal" for="ModDirPrincipal" class="formLabel formLabel2Izq">Principal*:</label>
                    <asp:DropDownList ID="ModDirPrincipal" runat="server" CssClass="formCombobox" Width="110" ClientIDMode="Static" ReadOnly="True">
                    </asp:DropDownList>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModDirAceptar" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
                    <a id="ModDirCancelar" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
                </div>

            </div>
        </div>
        <%--<label id="LabModTasaVenta" class="formLabel formLabel2Der">Tasa de Venta:</label>
                            <asp:TextBox ID="ModTasaVenta" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>--%>
        <%--SRI.FIN-20322--%>
        <div id="ModalTelefono" title="Teléfono del afiliado">
            <div id="ModTelCargando" class="modalCargandoContenido" style="width: 308px; height: 100px"></div>
            <div id="ModTelContenido" class="modalContenido">
                <asp:HiddenField ID="ModTelModo" runat="server" ClientIDMode="Static" />

                <div class="formLinea">
                    <label id="LabModTelTipo" for="ModTelTipo" class="formLabel formLabel2Izq">Tipo*:</label>
                    <asp:DropDownList ID="ModTelTipo" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static" ReadOnly="True">
                    </asp:DropDownList>
                </div>

                <div class="formLinea">
                    <label id="LabModTelNumero" for="ModTelNumero" class="formLabel formLabel2Izq">Número*:</label>
                    <asp:TextBox ID="ModTelNumero" runat="server" CssClass="formTextbox telefono" Width="180" ClientIDMode="Static" MaxLength="12"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModTelPrincipal" for="ModTelPrincipal" class="formLabel formLabel2Izq">Principal*:</label>
                    <asp:DropDownList ID="ModTelPrincipal" runat="server" CssClass="formCombobox" Width="110" ClientIDMode="Static" ReadOnly="True">
                    </asp:DropDownList>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModTelAceptar" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
                    <a id="ModTelCancelar" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
                </div>

            </div>
        </div>
        <%--Fin Modal Mantenimiento de solicitud--%>
        <%--Inicio Modal Envío de Correo--%>
        <div id="ModalGrupoFamiliar" title="Grupo Familiar">
            <%--<asp:Image ID="ModEnvCorIconoPDF" ImageUrl="~/Imagenes/iconoPDF.png" style="margin-left:-188px" runat="server" />--%>
            <asp:Panel ID="ModGruFamCargando" runat="server" ClientIDMode="Static" CssClass="modalCargandoContenido" Width="748" Height="230"></asp:Panel>
            <%--<label id="LabModCorMensaje" class="formLabel formLabel2Izq">Mensaje:</label>--%>
            <div id="ModGruFamContenido" class="modalContenido">
                <asp:HiddenField ID="ModGruFamModo" runat="server" ClientIDMode="Static" />

                <asp:Panel ID="ModGruFamLineaApellidos" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamApellidoPaterno" for="ModGruFamApellidoPaterno" class="formLabel formLabel2Izq">Apellido Paterno:</label>
                    <asp:TextBox ID="ModGruFamApellidoPaterno" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40"></asp:TextBox>

                    <label id="LabModGruFamApellidoMaterno" for="ModGruFamApellidoMaterno" class="formLabel formLabel2Der">Apellido Materno:</label>
                    <asp:TextBox ID="ModGruFamApellidoMaterno" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40"></asp:TextBox>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaNombres" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamNombres" for="ModGruFamNombres" class="formLabel formLabel2Izq">Nombres:</label>
                    <asp:TextBox ID="ModGruFamNombres" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static" MaxLength="80"></asp:TextBox>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaIdentificacion" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamTipoIdentificacion" for="ModGruFamTipoIdentificacion" class="formLabel formLabel2Izq">Tipo de Identificación:</label>
                    <asp:DropDownList ID="ModGruFamTipoIdentificacion" runat="server" CssClass="formCombobox" Width="260" ClientIDMode="Static">
                    </asp:DropDownList>

                    <label id="LabModGruFamNumeroIdentificacion" for="ModGruFamNumeroIdentificacion" class="formLabel formLabel2Der" style="margin-left: 22px;">Nro. de Identificación:</label>
                    <asp:TextBox ID="ModGruFamNumeroIdentificacion" runat="server" CssClass="formTextbox enteroPositivo" Width="180" ClientIDMode="Static" MaxLength="9"></asp:TextBox>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaParentesco" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamParentesco" for="ModGruFamParentesco" class="formLabel formLabel2Izq">Parentesco*:</label>
                    <asp:DropDownList ID="ModGruFamParentesco" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                    </asp:DropDownList>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaSexoNacimiento" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamSexo" for="ModGruFamSexo" class="formLabel formLabel2Izq">Sexo*:</label>
                    <asp:DropDownList ID="ModGruFamSexo" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                    </asp:DropDownList>

                    <label id="LabModGruFamFechaNacimiento" for="ModGruFamFechaNacimiento" class="formLabel formLabel2Der">Fecha de Nacimiento*:</label>
                    <asp:TextBox ID="ModGruFamFechaNacimiento" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaInvalidez" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamIndInvalidez" for="ModGruFamIndInvalidez" class="formLabel formLabel2Izq">Indicador de Invalidez*:</label>
                    <asp:DropDownList ID="ModGruFamIndInvalidez" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                    </asp:DropDownList>

                    <label id="LabModGruFamTipoInvalidez" for="ModGruFamTipoInvalidez" class="formLabel formLabel2Der">Tipo de Invalidez*:</label>
                    <asp:DropDownList ID="ModGruFamTipoInvalidez" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                    </asp:DropDownList>
                </asp:Panel>

                <asp:Panel ID="ModGruFamLineaFechaInvalidez" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamFechaInvalidez" for="ModGruFamFechaInvalidez" class="formLabel formLabel2Izq">Fecha de Invalidez<span id="AstModGruFamFechaInvalidez"></span>:</label>
                    <asp:TextBox ID="ModGruFamFechaInvalidez" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
                </asp:Panel>

                <asp:Panel ID="Panel1" runat="server" ClientIDMode="Static" CssClass="formLinea">
                    <label id="LabModGruFamFlgApoderado" for="ModGruFamFlgApoderado" class="formLabel formLabel2Izq">Tiene apoderado:</label>
                    <asp:CheckBox ID="ModGruFamFlgApoderado" runat="server" CssClass="formCheckbox" ClientIDMode="Static"></asp:CheckBox>
                </asp:Panel>

                <div id="Apoderado" style="display: none">
                    <asp:Panel ID="PanelApoderado" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabModGruFamApoderado" for="ModGruFamApoderado" class="formLabel formLabel2Izq">Apoderado</label>
                    </asp:Panel>

                    <asp:Panel ID="ModGruFamLineaApellidosAprdo" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabModGruFamApellidoPaternoAprdo" for="ModGruFamApellidoPaternoAprdo" class="formLabel formLabel2Izq">Apellido Paterno Apod.:</label>
                        <asp:TextBox ID="ModGruFamApellidoPaternoAprdo" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40"></asp:TextBox>

                        <label id="LabModGruFamApellidoMaternoAprdo" for="ModGruFamApellidoMaternoAprdo" class="formLabel formLabel2Der">Apellido Materno Apod.:</label>
                        <asp:TextBox ID="ModGruFamApellidoMaternoAprdo" runat="server" CssClass="formTextbox nombre" Width="180" ClientIDMode="Static" MaxLength="40"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="ModGruFamLineaNombresAprdo" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabModGruFamNombresAprdo" for="ModGruFamNombresAprdo" class="formLabel formLabel2Izq">Nombres Apod.:</label>
                        <asp:TextBox ID="ModGruFamNombresAprdo" runat="server" CssClass="formTextbox nombre" Width="620" ClientIDMode="Static" MaxLength="80"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="ModGruFamLineaIdentificacionAprdo" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabModGruFamTipoIdentificacionAprdo" for="ModGruFamTipoIdentificacionAprdo" class="formLabel formLabel2Izq">Tipo de Identif. Apod.:</label>
                        <asp:DropDownList ID="ModGruFamTipoIdentificacionAprdo" runat="server" CssClass="formCombobox" Width="260" ClientIDMode="Static"></asp:DropDownList>

                        <label id="LabModGruFamNumeroIdentificacionAprdo" for="ModGruFamNumeroIdentificacionAprdo" class="formLabel formLabel2Der" style="margin-left: 22px;">Nro. de Identif. Apod.:</label>
                        <asp:TextBox ID="ModGruFamNumeroIdentificacionAprdo" runat="server" CssClass="formTextbox enteroPositivo" Width="180" ClientIDMode="Static" MaxLength="9"></asp:TextBox>
                    </asp:Panel>

                    <asp:Panel ID="ModGruFamLineaSexoNacimientoAprdo" runat="server" ClientIDMode="Static" CssClass="formLinea">
                        <label id="LabModGruFamSexoAprdo" for="ModGruFamSexoAprdo" class="formLabel formLabel2Izq">Sexo Apod.:</label>
                        <asp:DropDownList ID="ModGruFamSexoAprdo" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static"></asp:DropDownList>

                        <label id="LabModGruFamFechaNacimientoAprdo" for="ModGruFamFechaNacimientoAprdo" class="formLabel formLabel2Der">Fecha de Nacim. Apod.:</label>
                        <asp:TextBox ID="ModGruFamFechaNacimientoAprdo" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
                    </asp:Panel>
                </div>

                <br />

                <div class="formLinea" align="center">
                    <a id="ModGruFamAceptar" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
                    <a id="ModGruFamCancelar" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
                </div>
            </div>
        </div>
        <%--Fin Modal Envío de Correo--%>
        <%--<GTIINI-754>--%>
        <div id="ModalSolicitud" title="Datos de Cotización">
            <asp:HiddenField ID="ModSolModo" runat="server" ClientIDMode="Static" />
            <div id="ModSolCargando" class="modalCargandoContenido" style="width: 848px; height: 580px"></div>
            <div id="ModSolContenido" class="modalContenido">
                <div id="ManSolPestanhas">
                    <ul>
                        <li id="ManSolPes1" data-pestanha="1"><a href="#">Cotización</a></li>
                        <li id="ManSolPes2" data-pestanha="2"><a href="#">Beneficiarios <span id="ManSolNumBeneficiarios"></span></a><span id="ManSolNumBeneficiariosCargando" class="pestanhaCargando"></span></li>
                    </ul>

                    <asp:HiddenField ID="ManSolPestanhaActiva" runat="server" Value="1" ClientIDMode="Static" />
                    <asp:HiddenField ID="ManSolTipoSolicitud" runat="server" ClientIDMode="Static" />

                    <div id="ManSolPestanha1" align="left" style="width: 840px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
                        <div class="formLinea">
                            <label id="LabModSolNroSolicitud" for="ModSolNroSolicitud" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
                            <asp:TextBox ID="ModSolNroSolicitud" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                            <label id="LabModSolTipoCambio" for="ModSolTipoCambio" class="formLabel formLabel2Der">Tipo de Cambio*:</label>
                            <asp:TextBox ID="ModSolTipoCambio" runat="server" CssClass="formTextbox numerico" Width="180" data-v-max="100.000" data-v-min="0.000" ClientIDMode="Static"></asp:TextBox>
                        </div>

                        <div class="formLinea">
                            <div style="float: left; width: 420px">
                                <label id="LabModSolCategoria" for="ModSolCategoria" class="formLabel formLabel2Izq">Categoría*:</label>
                                <asp:DropDownList ID="ModSolCategoria" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="188" ClientIDMode="Static" Enabled="False">
                                </asp:DropDownList>
                            </div>

                            <div style="float: left; margin-left: 20px; width: 400px">
                                <label id="LabModSolTipoPension" for="ModSolTipoPension" class="formLabel formLabel2Izq">Tipo de Pensión*:</label>
                                <asp:DropDownList ID="ModSolTipoPension" runat="server" CssClass="formCombobox" Width="195" ClientIDMode="Static">
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="formLinea">
                            <label id="LabModSolFechaDevengue" for="ModSolFechaDevengue" class="formLabel formLabel2Izq">Fecha de Devengue*:</label>
                            <asp:TextBox ID="ModSolFechaDevengue" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                            <label id="LabModSolFecUltActualizacion" for="ModSolFecUltActualizacion" class="formLabel formLabel2Der">Fecha Últ. Actualización*:</label>
                            <asp:TextBox ID="ModSolFecUltActualizacion" runat="server" CssClass="formTextbox formCalendarReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div class="formLinea">
                            <div style="float: left; width: 420px">
                                <label id="LabModSolFechaRecepcion" for="ModSolFechaRecepcion" class="formLabel formLabel2Izq">Fecha de Recepción*:</label>
                                <asp:TextBox ID="ModSolFechaRecepcion" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>
                            </div>

                            <div style="float: left; margin-left: 20px; width: 400px">
                                <label id="LabModSolFechaPlazoAFP" for="ModSolFechaPlazoAFP" class="formLabel formLabel2Izq">Fecha de Plazo AFP*:</label>
                                <asp:TextBox ID="ModSolFechaPlazoAFP" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>
                            </div>
                        </div>

                        <div class="formLinea">
                            <label id="LabModSolSaldoCIC" for="ModSolSaldoCIC" class="formLabel formLabel2Izq">Saldo CIC*:</label>
                            <asp:TextBox ID="ModSolSaldoCIC" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static"></asp:TextBox>
                            <asp:DropDownList ID="ModSolListaCIC" runat="server" CssClass="formCombobox" Width="188" ClientIDMode="Static">
                            </asp:DropDownList>

                            <label id="LabModSolFactorTasa" for="ModSolFactorTasa" class="formLabel formLabel2Der">Factor Tasa*:</label>
                            <asp:DropDownList ID="ModSolFactorTasa" runat="server" CssClass="formCombobox" Width="210" ClientIDMode="Static">
                            </asp:DropDownList>
                        </div>

                        <div class="formLinea">
                            <label id="LabModSolFechaCotizacion" for="ModSolFechaCotizacion" class="formLabel formLabel2Izq">Fecha de Cotización*:</label>
                            <asp:TextBox ID="ModSolFechaCotizacion" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>

                            <label id="LabModSolFechaSolicitudPension" for="ModSolFechaSolicitudPension" class="formLabel formLabel2Der">Fecha de Sol. Pensión*:</label>
                            <asp:TextBox ID="ModSolFechaSolicitudPension" runat="server" CssClass="fecha formTextbox formCalendar" Width="180" ClientIDMode="Static" MaxLength="10"></asp:TextBox>
                        </div>

                        <asp:Panel ID="LabModSolLineaACOMDCOM" runat="server" CssClass="formLinea">
                            <label id="LabModSolACOM" for="ModSolACOM" class="formLabel formLabel2Izq">Porcentaje A*:</label>
                            <asp:TextBox ID="ModSolACOM" runat="server" CssClass="formTextbox numerico" Width="180" data-v-max="100.00" data-v-min="-100.00" ClientIDMode="Static"></asp:TextBox>

                            <label id="LabModSolDCOM" for="ModSolDCOM" class="formLabel formLabel2Der">Porcentaje D*:</label>
                            <asp:TextBox ID="ModSolDCOM" runat="server" CssClass="formTextbox numerico" Width="180" data-v-max="100.00" data-v-min="-100.00" ClientIDMode="Static"></asp:TextBox>

                            <asp:Panel ID="LabModSolNivel" runat="server" CssClass="grilla_infoLibre" ToolTip="Nivel" ClientIDMode="Static">
                            </asp:Panel>

                        </asp:Panel>

                        <div id="ModSolLineaMontoACOM" class="formLinea">
                            <label id="LabModSolMontoACOM" for="ModSolMontoACOM" class="formLabel formLabel2Izq">Monto A:</label>
                            <asp:TextBox ID="ModSolMontoACOM" runat="server" CssClass="formTextbox formTextboxReadOnly numerico" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div id="TablaCotizacionesCargando" align="center" style="height: 50px; padding: 82px 0">
                            <asp:Image ID="icoTablaCotizacionesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando cotizaciones, espere por favor...</span>
                        </div>
                        <div id="TablaCotizacionesContenedor" style="display: none"></div>

                        <div class="formLinea">
                            <label id="LabModSolMontoCIA" class="formLabel formLabel2Izq">Monto CIA:</label>
                            <asp:TextBox ID="ModSolMontoCIA" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                            <label id="LabModSolTasaAFP" class="formLabel formLabel2Der">Tasa AFP:</label>
                            <asp:TextBox ID="ModSolTasaAFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div class="formLinea">
                            <label id="LabModSolPensionCIA" class="formLabel formLabel2Izq">Pensión CIA:</label>
                            <asp:TextBox ID="ModSolPensionCIA" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                            <label id="LabModSolMontoAFP" class="formLabel formLabel2Der">Monto AFP:</label>
                            <asp:TextBox ID="ModSolMontoAFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>

                        <div class="formLinea">
                            <label id="LabModSolPensionCIAMO" class="formLabel formLabel2Izq">Pensión CIA (M.O.):</label>
                            <asp:TextBox ID="ModSolPensionCIAMO" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                            <label id="LabModSolPensionAFP" class="formLabel formLabel2Der">Pensión AFP:</label>
                            <asp:TextBox ID="ModSolPensionAFP" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
                        </div>

                        <%--Inicio Modal Elegir Formato de Reporte--%>
                        <div class="formLinea">
                            <label id="LabModTasaVentaSbs" class="formLabel formLabel2Izq">Tasa de Venta SBS:</label>
                            <asp:TextBox ID="ModTasaVentaSbs" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

                            <%--Fin Modal Elegir Formato de Reporte--%>
                        </div>
                        <%--<GTIINI-754>--%>

                        <div class="formLinea" align="center">
                            <a id="ModSolAceptar" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Aceptar</a>
                            <a id="ModSolCancelar" href="javascript:void(0);" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
                        </div>
                    </div>

                    <div id="ManSolPestanha2" align="left" style="width: 840px; padding: 20px; margin-top: 1px; background: #FFF; border: 1px solid #00466e">
                        <div id="TablaBeneficiariosCargando" align="center" style="display: none">
                            <asp:Image ID="icoTablaBeneficiariosCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando beneficiarios, espere por favor...</span>
                        </div>
                        <div id="TablaBeneficiariosContenedor" style="display: none"></div>
                        <div id="TablaBeneficiariosError" class="grilla_error" style="display: none">No se ha podido cargar la tabla de beneficiarios. <a id="TablaBeneficiariosReintentar">Intentar de nuevo</a>.</div>

                        <br />

                        <div id="BeneficiariosOriginales" align="left" style="display: none">
                            Beneficiarios originales de la Solicitud:
                        </div>

                        <div id="TablaRviBenefiCargando" align="center" style="display: none">
                            <asp:Image ID="icoTablaRviBenefiCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                            <span class="texto">Cargando beneficiarios registrados originalmente en la solicitud, espere por favor...</span>
                        </div>
                        <div id="TablaRviBenefiContenedor" style="display: none"></div>
                        <div id="TablaRviBenefiError" class="grilla_error" style="display: none">No se ha podido cargar la tabla de beneficiarios originales. <a id="TablaRviBenefiReintentar">Intentar de nuevo</a>.</div>
                    </div>
                </div>
            </div>
        </div>
        <%--Inicio Modal Ver Actividad--%>
        <%--Fin Modal ver Actividad--%>
        <div id="ModalEnvioCorreo" title="Enviar Correo Electrónico">
            <div id="ModEnvCorCargando" class="modalCargandoContenido" style="width: 526px; height: 350px"></div>
            <div id="ModEnvCorContenido" class="modalContenido">
                <asp:HiddenField ID="ModEnvCorModo" runat="server" ClientIDMode="Static" />

                <div class="formLinea">
                    <label id="LabModEnvCorDe" class="formLabel formLabel2Izq">De:</label>
                    <asp:TextBox ID="ModEnvCorDe" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorPara" class="formLabel formLabel2Izq">Para:</label>
                    <asp:TextBox ID="ModEnvCorPara" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAsunto" class="formLabel formLabel2Izq">Asunto:</label>
                    <asp:TextBox ID="ModEnvCorAsunto" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -188px" Width="490" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea">
                    <label id="LabModEnvCorAdjunto" class="formLabel formLabel2Izq">Adjunto:</label>
                    <%--Inicio Modal Cuadro de mensajes--%>
                    <label id="ModEnvCorAdjunto" class="formLabelAdjunto">Cotizacion.pdf (189kb)</label>
                </div>

                <div class="formLinea" style="height: 210px">
                    <%--Fin Modal Cuadro de mensajes--%>
                    <asp:TextBox ID="ModEnvCorMensaje" runat="server" CssClass="formTextbox" Style="margin-left: 0px; height: 200px" Width="560" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModEnvCorEnviar" style="width: 80px; height: 22px" class="boton darkblue sharp">Enviar</a>
                    <a id="ModEnvCorCancelar" style="width: 80px; height: 22px" class="boton darkblue sharp">Cancelar</a>
                </div>
            </div>
        </div>
        <%--Inicio Modal Cuadro de advertencia--%>
        <%--Fin Modal Cuadro de advertencia--%>        <%--Inicio Modal Cotizando--%>
        <asp:HiddenField ID="RepDetalleCotizacionIdSolicitud" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="RepDetalleCotizacionFechaCotizacion" runat="server" ClientIDMode="Static" />
        <asp:HiddenField ID="RepDetalleCotizacionNumeroAgente" runat="server" ClientIDMode="Static" />
        <div id="ModalElegirFormatoReporte" title="Elegir Formato de Reporte">
            <div id="MEFROpcionesContenedor">
                <table id="TabFormatoReporte" border="0" cellspacing="5" cellpadding="0" width="100%">
                    <tr style="height: 150px">
                        <td class="hojaBlanco">
                            <input id="ModForHojaBlanco" type="radio" name="FormatoReporte" value="B" checked="checked" />
                            <label for="ModForHojaBlanco"><span style="padding: 60px 0 60px 105px;">Hoja en blanco</span></label>
                        </td>
                    </tr>
                    <tr style="height: 150px">
                        <td class="hojaMembretada">
                            <input id="ModForHojaMembretada" type="radio" name="FormatoReporte" value="M" />
                            <label for="ModForHojaMembretada"><span style="padding: 60px 0 60px 105px;">Hoja Membretada</span></label>
                        </td>
                    </tr>
                </table>
            </div>
            <div align="center" class="formLinea">
                <a id="MEFRAceptar" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MEFRCancelar" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <%--Fin Modal Cotizando--%>        <%--Inicio Modal Generando Reporte--%>
        <%--Fin Modal Generando Reporte--%>
        <div id="ModalActividad" title="Ver Actividad">
            <div id="ModActCargando" class="modalCargandoContenido" style="width: 526px; height: 250px"></div>
            <div id="ModActContenido" class="modalContenido">

                <div class="formLinea">
                    <label id="LabModActEvento" class="formLabel formLabel2Izq">Evento:</label>
                    <asp:TextBox ID="ModActEvento" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -168px" Width="470" ClientIDMode="Static"></asp:TextBox>
                </div>

                <div class="formLinea" style="height: 210px">
                    <label id="LabModActComentario" class="formLabel formLabel2Izq">Comentario:</label>
                    <asp:TextBox ID="ModActComentario" runat="server" CssClass="formTextbox formTextboxReadOnly" ReadOnly="True" Style="margin-left: -168px; height: 200px" Width="470" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="formLinea" align="center">
                    <a id="ModActCerrar" style="width: 80px; height: 22px" class="boton darkblue sharp">Cerrar</a>
                </div>

            </div>
        </div>
        <%--Inicio Modal Seleccion de ACOM--%>
        <%--Fin Modal Seleccion de ACOM--%>
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
        <%--Inicio Modal Consentimiento de Asesoria 26560--%>
        <%--Fin Modal Consentimiento de Asesoria 26560--%>
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
                <a id="MCAAceptar" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MCACancelar" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
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

        <%--Inicio Modal Generando Reporte--%>
        <div id="ModalGenerandoReporte">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MGRIcono" style="width: 40px; height: 40px"></td>
                        <td id="MGRContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MGRContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MGREstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MGREstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MRGEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MGRMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <%--Fin Modal Generando Reporte--%>

        <%--Inicio Modal Seleccion de ACOM--%>
        <div id="ModalSeleccionAcom">
            <div>
                <div id="TablaSeleccionAcomCargando" align="center" style="display: none">
                    <asp:Image ID="Image2" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
                    <span class="texto">Cargando registros de ACOM, espere por favor...</span>
                </div>

                <div id="DivErrorAcom" class="grilla_error" style="display: none">
                    No se ha podido cargar la tabla de ACOM. <a id="A1">Intentar de nuevo</a>.
                </div>

                <div id="TablaSeleccionAcomContenedor" style="display: none;"></div>
            </div>
            <br />
            <div id="DivBotoneraAcom" align="center">
                <a id="MRVGuardarSeleccionAcom" class="boton darkblue sharp" style="width: 80px">Aceptar</a>
                <a id="MRVCancelarSeleccionAcom" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>
        </div>
        <asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
        <%--Fin Modal Seleccion de ACOM--%>

        <%--Inicio Modal Consentimiento de Asesoria --%>
        <div id="ModalConsentimientoAsesoria">
            <div id="ModConsentimientoAsesoriaCargando" class="modalCargandoContenido" style="display: none; width: 645px; height: 100px"></div>

            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCConstmAsesIcono" style="width: 40px; height: 40px"></td>
                        <td id="MCConstmAsesContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCConstmAsesContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MCConstmAsesEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MCConstmAsesEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MCConstmAsesEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MCConstmAsesMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MCConstmAsesBotonera" align="center">
                <a id="MCConstmAsesEnviar" class="boton darkblue sharp" style="width: 80px">Enviar</a>
                <a id="MCConstmAsesCancelar" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>

        </div>
        <%--Fin Modal Consentimiento de Asesoria--%>

        <%--Inicio Modal Consentimiento de Asesoria SMS--%>
        <div id="ModalConsentimientoAsesoriaSMS">
            <div class="modalCargandoContenido" style="display: none; width: 645px; height: 100px"></div>

            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MCConstmAsesIconoSMS" style="width: 40px; height: 40px"></td>
                        <td valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MCConstmAsesContenedorSMS" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="HiddenField1" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="HiddenField2" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="HiddenField3" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="Literal1" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div align="center">
                <a id="MCConstmAsesEnviarSMS" class="boton darkblue sharp" style="width: 80px">Enviar</a>
                <a id="MCConstmAsesCancelarSMS" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>

        </div>
        <%--Fin Modal Consentimiento de Asesoria SMS--%>

        <%--Inicio Modal Reenvío Formato Consentimiento de Asesoria--%>
        <div id="ModalReenvioConsentimientoAsesoria">
            <div>
                <table width="100%" border="0" cellpadding="0" cellspacing="0">
                    <tr>
                        <td id="MRCAIcono" style="width: 40px; height: 40px"></td>
                        <td id="MRCAContenedorMensaje" valign="middle" class="cuadroMensaje">
                            <asp:Panel ID="MRCAContenedor" runat="server" ClientIDMode="Static" align="left" Style="margin: 10px 0">
                                <asp:HiddenField ID="MRCAEstado" runat="server" ClientIDMode="Static" Value="0" />
                                <asp:HiddenField ID="MRCAEstadoIcono" runat="server" ClientIDMode="Static" />
                                <asp:HiddenField ID="MRCAEstadoTitulo" runat="server" ClientIDMode="Static" />
                                <asp:Literal ID="MRCAMensaje" runat="server"></asp:Literal>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="MRCABotonera" align="center">
                <a id="MRCAEnviar" class="boton darkblue sharp" style="width: 80px">Enviar</a>
                <a id="MRCACancelar" class="boton darkblue sharp" style="width: 80px">Cancelar</a>
            </div>

        </div>
        <%--Fin Modal Reenvío Formato Consentimiento de Asesoria--%>
    </div>
</asp:Content>
