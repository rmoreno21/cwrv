<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="CotizadorOficiales.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Cotizador.CotizadorOficiales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.generarPdf.js")%>"></script>
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/CWRV.controles.load.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiCotizadorRV.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
	<script type="text/javascript" language="javascript">
		window.onload = function () {
			//<INIGTI_4081>
			SegIdJefe = ($('#OfiJefe').length > 0) ? $('#OfiJefe').val() : "0";
			SegIdSupervisor = ($('#OfiSupervisor').length > 0) ? $('#OfiSupervisor').val() : "0";
			SegIdAgente = $('#OfiAgente').val();

			//CargarTablaSolicitudesOficiales(0, 0, 0, true, null);
			CargarTablaSolicitudesOficiales(SegIdJefe, SegIdSupervisor, SegIdAgente, true, null);
			//<FINGTI_4081>
		}
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
	<div align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">
		<h1 class="simple" style="width:180px">Cotizaciones Oficiales</h1>

		<asp:Panel ID="ControlJefe" CssClass="formLinea" runat="server" ClientIDMode="Static">
			<label id="LabOfiJefe" for="OfiJefe" class="formLabel formLabel2Izq">Jefe:</label>
			<asp:DropDownList ID="OfiJefe" runat="server" CssClass="formCombobox" Width="356" Enabled="False" ClientIDMode="Static">
			</asp:DropDownList>
		</asp:Panel>
		<asp:HiddenField ID="HJefe" runat="server" Value="0" ClientIDMode="Static" />

		<asp:Panel ID="ControlSupervisor" CssClass="formLinea" runat="server" ClientIDMode="Static">
			<label id="LabOfiSupervisor" for="OfiSupervisor" class="formLabel formLabel2Izq">Supervisor:</label>
			<asp:DropDownList ID="OfiSupervisor" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="356" Enabled="False" ClientIDMode="Static">
			</asp:DropDownList>
			<span id="CargandoSupervisor" class="paginador_cargando"></span>
		</asp:Panel>
		<asp:HiddenField ID="HSupervisor" runat="server" Value="0" ClientIDMode="Static" />

		<asp:Panel ID="ControlAgente" CssClass="formLinea" runat="server" ClientIDMode="Static">
			<label id="LabOfiAgente" for="OfiAgente" class="formLabel formLabel2Izq">Agente:</label>
			<asp:DropDownList ID="OfiAgente" runat="server" CssClass="formCombobox formComboboxReadOnly" Width="356" Enabled="False" ClientIDMode="Static">
			</asp:DropDownList>
			<span id="CargandoAgente" class="paginador_cargando"></span>
		</asp:Panel>
		<asp:HiddenField ID="HAgente" runat="server" Value="0" ClientIDMode="Static" />

		<div class="formLinea" align="center">
			<asp:HyperLink ID="btnCotOfiBuscar" style="width:80px;height:22px" CssClass="boton darkblue sharp" runat="server" ClientIDMode="Static">Buscar</asp:HyperLink>
		</div>

		<br />

		<div id="Cargando" align="center" style="display:none">
			<asp:Image ID="icoCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
			<span class="texto">Buscando solicitudes, espere por favor...</span>
		</div>
		<div id="TablaOficialError" align="center" style="display:none">
			<div class="grilla_error" align="left" style="width:385px;">
				No se ha podido cargar la tabla de Oficiales. <a id="TablaOficialReintentar">Intentar de nuevo</a>.
			</div>
		</div>

		<div id="TablaSeguimiento" runat="server" style="display:none" clientidmode="Static"></div>

		<asp:HiddenField ID="TabSeguimientoIndicePagina" runat="server" ClientIDMode="Static" Value="1" />
		<asp:HiddenField ID="TabSeguimientoTamanhoPagina" runat="server" ClientIDMode="Static" Value="10" />
		<asp:HiddenField ID="TabSeguimientoColumnaOrdenar" runat="server" ClientIDMode="Static" Value="1" />
		<asp:HiddenField ID="TabSeguimientoDireccionOrdenar" runat="server" ClientIDMode="Static" Value="A" />
		<asp:HiddenField ID="HNumSolicitud" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="HFecCotizacion" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="HNumOperacion" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="HCodTipoMovimiento" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="HNumAgente" runat="server" ClientIDMode="Static" />

		<input type="hidden" id="usuario_actual" value="<%= Session["Usuario"] %>" />
		<input type="hidden" id="rol_azman" value="<%= Session["RolAzman"] %>" />
		<input type="hidden" id="url_api_rentas_rv" value="<%= System.Configuration.ConfigurationManager.AppSettings["ApiCotizadorRvUrl"] %>" />
    	<input type="hidden" id="url_api_reportes" value="<%= System.Configuration.ConfigurationManager.AppSettings["ApiReportesUrl"] %>" />

		<%--<SOLINI25781>--%>
		<asp:HiddenField ID="HOcultraColumnaTRA" runat="server" ClientIDMode="Static" />
		<%--<SOLFIN25781>--%>

		<%--<INIGTI_1092>--%>
		<asp:HiddenField ID="HRedLocal" runat="server" ClientIDMode="Static" />
		<%--<FINGTI_1092>--%>

		<%--<INIGTI_4081>--%>
			<asp:HiddenField ID="HMaxPBS" runat="server" ClientIDMode="Static" />
		<%--<FINGTI_4081>--%>

		<%--<div id="TablaSolicitudesCargando" align="center" style="display:none">
				<asp:Image ID="icoTablaSolicitudesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
				<span class="texto">Cargando solicitudes, espere por favor...</span>
		</div>
		<div id="TablaSolicitudesContenedor" style="display:none"></div>
		<div id="TablaSolicitudesError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesReintentar">Intentar de nuevo</a>.</div>--%>

		<div id="TablaSolicitudesOficialesCargando" align="center" style="display:none">
				<asp:Image ID="icoTablaSolicitudesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
				<span class="texto">Cargando solicitudes, espere por favor...</span>
		</div>
		<div id="TablaSolicitudesOficialesContenedor" style="display:none"></div>
		<div id="TablaSolicitudesOficialesError" class="grilla_error" style="display:none">No se ha podido cargar la tabla de solicitudes. <a id="TablaSolicitudesOficialesReintentar">Intentar de nuevo</a>.</div>

	</div>

	<div style="display:none">

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

		<%--<GTIINI-754>--%>
		<%--Inicio Modal Elegir Formato de Reporte--%>
		<asp:HiddenField ID="RepDetalleCotizacionIdSolicitud" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="RepDetalleCotizacionFechaCotizacion" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="RepDetalleCotizacionNumeroAgente" runat="server" ClientIDMode="Static" />
		<div id="ModalElegirFormatoReporte" title="Elegir Formato de Reporte">
			<div id="MEFROpcionesContenedor">
				<table id="TabFormatoReporte" border="0" cellspacing="5" cellpadding="0" width="100%">
					<tr style="height:150px">
						<td class="hojaBlanco">
							<input id="ModForHojaBlanco" type="radio" name="FormatoReporte" value="B" checked="checked" />
							<label for="ModForHojaBlanco"><span style="padding:60px 0 60px 105px;">Hoja en blanco</span></label>
						</td>
					</tr>
					<tr style="height:150px">
						<td class="hojaMembretada">
							<input id="ModForHojaMembretada" type="radio" name="FormatoReporte" value="M" />
							<label for="ModForHojaMembretada"><span style="padding:60px 0 60px 105px;">Hoja Membretada</span></label>
						</td>
					</tr>
				</table>
			</div>
			<div align="center" class="formLinea">
				<a id="MEFRAceptar" class="boton darkblue sharp" style="width:80px">Aceptar</a>
				<a id="MEFRCancelar" class="boton darkblue sharp" style="width:80px">Cancelar</a>
			</div>
		</div>
		<%--Fin Modal Elegir Formato de Reporte--%>
		<%--<GTIINI-754>--%>

		<%--Inicio Modal Mantenimiento de solicitud--%>
		<div id="ModalSolicitud" title="Datos de Escenario">
			<asp:HiddenField ID="ModSolModo" runat="server" ClientIDMode="Static" />
			<div id="ModSolCargando" class="modalCargandoContenido" style="width:848px;height:580px"></div>
			<div id="ModSolContenido" class="modalContenido">
				<div id="ManSolPestanhas">
					<ul>
						<li id="ManSolPes1" data-pestanha="1"><a>Cotización</a></li>
						<li id="ManSolPes2" data-pestanha="2"><a>Beneficiarios <span id="ManSolNumBeneficiarios"></span></a> <span id="ManSolNumBeneficiariosCargando" class="pestanhaCargando"></span></li>
					</ul>

					<asp:HiddenField ID="ManSolPestanhaActiva" runat="server" Value="1" ClientIDMode="Static" />
					<asp:HiddenField ID="ManSolTipoSolicitud" runat="server" ClientIDMode="Static" />
                    <%--<GTI.29372>--%>
                    <asp:HiddenField ID="hdnMostrarEnvioObligatorio" runat="server" ClientIDMode="Static" />
                    <%--</GTI.29372>--%>

					<div id="ManSolPestanha1" align="left" style="width:840px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
						<div class="formLinea">
							<label id="LabModSolNroSolicitud" for="ModSolNroSolicitud" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
							<asp:Label ID="ModSolNroSolicitud" runat="server" CssClass="formDato" Width="186" ClientIDMode="Static"></asp:Label>

							<label id="LabModSolNroMeller" for="ModSolNroMeller" class="formLabel formLabel2Der">Nro. Meler:</label>
							<asp:Label ID="ModSolNroMeller" runat="server" CssClass="formDato" Width="186" ClientIDMode="Static"></asp:Label>
						</div>

						<%--<div class="formLinea">
							<label id="LabModSolIndCondicionEspecial" for="ModSolIndCondicionEspecial" class="formLabel formLabel2Izq">Ind. Cond. Especial:</label>
							<asp:Label ID="ModSolIndCondicionEspecial" runat="server" CssClass="formDato" Width="186" ClientIDMode="Static"></asp:Label>

							<label id="LabModSolIndCondicionAprobado" for="ModSolIndCondicionAprobado" class="formLabel formLabel2Der">Ind. Cond.  Aprobado:</label>
							<asp:Label ID="ModSolIndCondicionAprobado" runat="server" CssClass="formDato" Width="186" ClientIDMode="Static"></asp:Label>
						</div>--%>

						<div class="formLinea">
							<label id="LabModSolMontoCIC" for="ModSolMontoCIC" class="formLabel formLabel2Izq">Monto CIC:</label>
							<asp:Label ID="ModSolMontoCIC" runat="server" CssClass="formDato" Width="186" ClientIDMode="Static"></asp:Label>

							<label id="LabModFecCierre" for="ModFecCierre" class="formLabel formLabel2Der">Fecha de Plazo AFP:</label>
							<asp:Label ID="ModFecCierre" runat="server" CssClass="formDato" Width="186" ClientIDMode="Static"></asp:Label>
						</div>

						<div class="formLinea">
							<label id="LabModSolCategoria" for="ModSolCategoria" class="formLabel formLabel2Izq">Categoría:</label>
							<asp:DropDownList ID="ModSolCategoria" runat="server" CssClass="formComboboxTexto" Width="190" ClientIDMode="Static" Enabled="false">
							</asp:DropDownList>

							<label id="LabModSolFechaRegistro" for="ModSolFechaRegistro" class="formLabel formLabel2Der">Fecha de Registro:</label>
							<asp:Label ID="ModSolFechaRegistro" runat="server" CssClass="formDato" Width="186" ClientIDMode="Static"></asp:Label>
						</div>

						<div class="formLinea">
							<label id="LabModSolCussp" for="ModSolCussp" class="formLabel formLabel2Izq">CUSSP:</label>
							<asp:Label ID="ModSolCussp" runat="server" CssClass="formDato" Width="186" ClientIDMode="Static"></asp:Label>

							<label id="LabModSolNomAfiliado" for="ModSolNomAfiliado" class="formLabel formLabel2Der">Nombre Afiliado:</label>
							<asp:Label ID="ModSolNomAfiliado" runat="server" CssClass="formDato" Width="240" ClientIDMode="Static"></asp:Label>
						</div>

						<div class="formLinea">
							<label id="LabModSolNumAgente" for="ModSolNumAgente" class="formLabel formLabel2Izq">Nro. Agente:</label>
							<asp:Label ID="ModSolNumAgente" runat="server" CssClass="formDato" Width="186" ClientIDMode="Static"></asp:Label>

							<label id="LabModSolNomAgente" for="ModSolNomAgente" class="formLabel formLabel2Der">Nombre Agente:</label>
							<asp:Label ID="ModSolNomAgente" runat="server" CssClass="formDato" Width="240" ClientIDMode="Static"></asp:Label>
						</div>

						<asp:Panel ID="LabModSolLineaACOMDCOM" runat="server" CssClass="formLinea"  ClientIDMode="Static">
							<label id="LabModSolACOM" for="ModSolACOM" class="formLabel formLabel2Izq">Porcentaje A*:</label>
							<asp:TextBox ID="ModSolACOM" runat="server" CssClass="formTextbox numerico" Width="70" data-v-max="100.00" data-v-min="0.00" ClientIDMode="Static" ReadOnly="false"></asp:TextBox>

							<label id="LabModSolDCOM" for="ModSolDCOM" class="formLabel formLabel2Der" style="margin-left:182px">Porcentaje D*:</label>
							<asp:TextBox ID="ModSolDCOM" runat="server" CssClass="formTextbox numerico" Width="70" data-v-max="100.00" data-v-min="0.00" ClientIDMode="Static" ReadOnly="false"></asp:TextBox>
							
							<asp:Panel ID="LabModSolNivel" runat="server" CssClass="grilla_infoLibre" ToolTip="Nivel" ClientIDMode="Static" Visible="false">
							</asp:Panel>
						</asp:Panel>

						<div class="formLinea" id ="LabModSolLineaSelecMon">
							<label id="LabModSolIndSeleccionado" for="ModSolIndSeleccionado" class="formLabel formLabel2Izq">Ind. Seleccionado*:</label>
							<asp:DropDownList ID="ModSolIndSeleccionado" runat="server" CssClass="formCombobox" Width="78" ClientIDMode="Static">
							</asp:DropDownList>
							<label id="LabModSolValMontoAcomAgente" for="ModSolValMontoAcomAgente" class="formLabel formLabel2Der" style="margin-left:182px;width:230px">Monto A*:</label>
							<asp:TextBox ID="ModSolValMontoAcomAgente" runat="server" CssClass="formTextbox numerico" Width="70" data-v-max="0.00" data-v-min="0.00" ClientIDMode="Static" ReadOnly="false" style="margin-left:-78px"></asp:TextBox>
							<span id="ModSolMontoACOMCargando" class="paginador_cargando"></span>
						</div>

						<%--<INIGTI_4081>--%>
						<div class="formLinea">
							<label id="LabModSolCompania" for="ModSolCompania" class="formLabel formLabel2Izq">Compañia:</label>
							<asp:DropDownList ID="ModSolCompania" runat="server" CssClass="formCombobox" Width="150" ClientIDMode="Static">
							</asp:DropDownList>
						</div>
						<%--<FINGTI_4081>--%>



						<%--<SOLINI25781>--%>
						<%--<INIGTI_4081>--%>
						<%--<asp:Panel ID="ControlTRA" CssClass="formLinea" runat="server" ClientIDMode="Static">

							<label id="Label1" for="ModSolTRA" class="formLabel formLabel2Izq">Dif. TRA:</label>
							<asp:TextBox ID="ModSolTRA" runat="server" CssClass="formTextbox numerico" Width="70" data-v-max="0.00" data-v-min="-100.00" ClientIDMode="Static" ReadOnly="false" ToolTip="Recuerde que el valor diferencial de TRA debe ser negativo."></asp:TextBox>
							
						</asp:Panel>--%>
						<%--<FINGTI_4081>--%>
						<%--<SOLFIN25781>--%>

						<div id="TablaCotizacionesOficialesCargando" align="center" style="height:50px;padding:82px 0">
							<asp:Image ID="icoTablaCotizacionesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
							<span class="texto">Cargando cotizaciones, espere por favor...</span>
						</div>
						
						<div id="TablaCotizacionesOficialesContenedor" style="display:none"></div>

						
						<%--<div class="formLinea">
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

						<div class="formLinea">
							<label id="LabModTasaVenta" class="formLabel formLabel2Izq">Tasa de Venta:</label>
							<asp:TextBox ID="ModTasaVenta" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>

							<label id="LabModTasaVentaSbs" class="formLabel formLabel2Der">Tasa de Venta SBS:</label>
							<asp:TextBox ID="ModTasaVentaSbs" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>
						</div>--%>

						<%--<div class="formLinea" align="center">
							<a id="ModSolAceptar" href="javascript:void(0);" style="width:80px;height:22px" class="boton darkblue sharp">Aceptar</a>
							<a id="ModSolCancelar" href="javascript:void(0);" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
						</div>--%>

						<div class="formLinea" align="center">
							<a id="ModSolCalcularPBS" style="width:110px;height:22px" class="boton darkblue sharp">Calcular PBS</a>
							<a id="ModSolAceptarOficial" style="width:80px;height:22px" class="boton darkblue sharp">Aceptar</a>
							<a id="ModSolCancelar" style="width:80px;height:22px" class="boton darkblue sharp">Cancelar</a>
                            <%--<GTI.29372>--%>
                            <a id="ModSolAceptarEnvioObligatorio" style="width:180px;height:22px;" class="boton darkblue sharp">Grabar envío obligatorio</a>
                            <%--</GTI.29372>--%>
						</div>

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
		</div>
		<%--Fin Modal Mantenimiento de solicitud--%>

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

		<%--Inicio Modal Generando Reporte--%>
		<div id="ModalGenerandoReporte">
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
		</div>
		<%--Fin Modal Generando Reporte--%>
	
		<%--Inicio Modal Seleccion de ACOM--%>
		<div id="ModalSeleccionAcom">
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
				<a id="MRVGuardarSeleccionAcomOficiales" class="boton darkblue sharp" style="width:80px;">Aceptar</a>
				<a id="MRVCancelarSeleccionAcomOficiales" class="boton darkblue sharp" style="width:80px;">Cancelar</a>
			</div>
		</div>
		<asp:HiddenField ID="hdKeyAcom" runat="server" ClientIDMode="Static" />
		<%--Fin Modal Seleccion de ACOM--%>

		<%--Inicio Modal Cuadro de advertencia--%>
		<div id="ModalCuadroAdvertencia">
			<div>
				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<tr>
						<td id="MCAIcono" style="width:40px;height:40px"></td>
						<td id="MCAContenedorMensaje" valign="middle" class="cuadroMensaje">
							<asp:Panel ID="MCAContenedor" runat="server" ClientIDMode="Static" align="left" style="margin:10px 0">
								
								<asp:HiddenField ID="MCAEstadoIcono" runat="server" ClientIDMode="Static" />
								<asp:HiddenField ID="MCAEstadoTitulo" runat="server" ClientIDMode="Static" />
								<asp:Literal ID="MCAMensaje" runat="server"></asp:Literal>
							</asp:Panel>
							<asp:HiddenField ID="MCAEstado" runat="server" ClientIDMode="Static" Value="0" />
						</td>
					</tr>
				</table>
			</div>
			<div id="MCABotonera" align="center">
				<a id="MCAOAceptar" class="boton darkblue sharp" style="width:80px">Aceptar</a>
				<a id="MCAOCancelar" class="boton darkblue sharp" style="width:80px">Cancelar</a>
			</div>
		</div>
		<asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
		<%--Fin Modal Cuadro de advertencia--%>
	</div>
</asp:Content>
