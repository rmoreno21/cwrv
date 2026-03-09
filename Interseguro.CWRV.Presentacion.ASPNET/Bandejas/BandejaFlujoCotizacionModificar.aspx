<%@ Page Title="" Language="C#" MasterPageFile="~/CotWebRVI.Master" AutoEventWireup="true" CodeBehind="BandejaFlujoCotizacionModificar.aspx.cs" Inherits="Interseguro.CWRV.Presentacion.ASPNET.Bandejas.BandejaFlujoCotizacionModificar" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Cabecera" runat="server">
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/utilitarios.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiSolicitudesCambio.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/bandejaFlujoCotizacionModificar.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
	<script type="text/javascript" src="<%=ResolveUrl("~/Scripts/apiCotizadorRV.js")%>?v=<% Response.Write(DateTime.Now.ToString("yyyyMMdd")); %>"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">

	<div align="left" style="width:860px;padding:20px;background:#FFF; border:1px solid #00466e;">

		<h1 class="simple" style="width:180px">Solicitudes de Cambio</h1>

		<div id="Cargando" align="center" style="display:none">
			<asp:Image ID="icoCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
			<span class="texto">Buscando solicitudes, espere por favor...</span>
		</div>
		<div id="TablaOficialError" align="center" style="display:none">
			<div class="grilla_error" align="left" style="width:385px;">
				No se ha podido cargar la tabla de Oficiales. <a id="TablaOficialReintentar" href="#">Intentar de nuevo</a>.
			</div>
		</div>

		<div id="TablaSeguimiento" runat="server" style="display:none" clientidmode="Static"></div>

		<asp:HiddenField ID="HCusspp" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="HNumSolicitud" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="HNumOperacion" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="HCodTipoMovimiento" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="HBloqueaRadio" runat="server" Value="TRUE" ClientIDMode="Static" />

		<asp:HiddenField ID="PerRechazarSolicitud" runat="server" ClientIDMode="Static" />
		<input type="hidden" id="url_api_rentas_rv" value="<%= System.Configuration.ConfigurationManager.AppSettings["ApiCotizadorRvUrl"] %>" />


		<div id="TabSolicitudFlujo">
		<%--<div id="ModalSolicitud" title="Datos de Escenario">--%>
			<asp:HiddenField ID="ModSolModo" runat="server" ClientIDMode="Static" />
			<%--<div id="ModSolCargando" class="modalCargandoContenido" style="width:848px;height:580px"></div>--%><%--<INIGTI_4081>--%>
			<div id="ModSolContenido" class="modalContenido">

				<asp:HiddenField ID="ManSolPestanhaActiva" runat="server" Value="1" ClientIDMode="Static" />
				<asp:HiddenField ID="ManSolTipoSolicitud" runat="server" ClientIDMode="Static" />

				<asp:Panel ID="GrupoBeneficiario" runat="server" ClientIDMode="Static">
					<div id="AgrupadorBeneficiario" class="agrupador">
						<span class="agrupador_titulo_mas">Beneficiarios</span>
					</div>

					<div id="DatosBeneficiario" style="display:none">

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
						<br />
					</div>
				</asp:Panel>

				<asp:Panel ID="GrupoInformeA" runat="server" ClientIDMode="Static" style="display:none">
					<div id="AgrupadorInformeA" class="agrupador">
						<span class="agrupador_titulo_mas">Indicadores</span>
					</div>
					<div id="DatosInformeA" align="left"  style="display:none; border:1px solid #00466e;padding:20px">

						<div class="formLinea">
							<label id="LabCondicion" for="Condicion" class="formLabel formLabel2Izq">Condición del cubo:</label>
							<asp:Label ID="Condicion" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

							<label id="LabPronostico" for="Pronostico" class="formLabel formLabel2Der">Pronóstico:</label>
							<asp:Label ID="Pronostico" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
						</div>

						<div class="formLinea">
							<label id="LabMSAgente" for="MSAgente" class="formLabel formLabel2Izq">%MS Agente:</label>
							<asp:Label ID="MSAgente" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

							<label id="LabMSAgente6" for="MSAgentge6" class="formLabel formLabel2Der">%MS Ag. (últ. 6 meses):</label>
							<asp:Label ID="MSAgentge6" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
						</div>

						<div class="formLinea">
							<label id="LabAvance" for="Avance" class="formLabel formLabel2Izq">AC. de oportunidad:</label>
							<asp:Label ID="Avance" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

							<label id="LabVisitasCompletadas" for="VisitasCompletadas" class="formLabel formLabel2Der">Nro. visitas completadas:</label>
							<asp:Label ID="VisitasCompletadas" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
						</div>

						<div class="formLinea">
							<label id="LabFechaUltimaVisita" for="FechaUltimaVisita" class="formLabel formLabel2Izq">Fecha última visita:</label>
							<asp:Label ID="FechaUltimaVisita" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
						</div>
					</div>
				</asp:Panel>

				<asp:Panel ID="GrupoCotizacion" runat="server" ClientIDMode="Static">
					<div id="AgrupadorCotizacion" class="agrupador">
						<span class="agrupador_titulo_mas">Cotización</span>
					</div>
					<div id="DatosCotizacion" align="left"  style="display:none; border:1px solid #00466e;padding:20px">
						<div class="formLinea">
							<label id="LabModSolNroSolicitud" for="ModSolNroSolicitud" class="formLabel formLabel2Izq">Nro. Solicitud:</label>
							<%--<asp:TextBox ID="ModSolNroSolicitud" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>--%>
							<asp:Label ID="ModSolNroSolicitud" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>


							<label id="LabModSolNroMeller" for="ModSolNroMeller" class="formLabel formLabel2Der">Nro. Meler:</label>
							<%--<asp:TextBox ID="ModSolNroMeller" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>--%>
							<asp:Label ID="ModSolNroMeller" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

						</div>
						
						<div class="formLinea">
							<label id="LabModSolMontoCIC" for="ModSolMontoCIC" class="formLabel formLabel2Izq">Monto CIC:</label>
							<%--<asp:TextBox ID="ModSolMontoCIC" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" data-v-max="100.00" data-v-min="-100.00" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>--%>
							<asp:Label ID="ModSolMontoCIC" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>


							<label id="LabModFecCierre" for="ModFecCierre" class="formLabel formLabel2Der">Fecha de Plazo AFP:</label>
							<%--<asp:TextBox ID="ModFecCierre" runat="server" CssClass="formTextbox formCalendar" Width="180" ClientIDMode="Static" ReadOnly="True" Enabled="false"></asp:TextBox>--%>
							<asp:Label ID="ModFecCierre" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
						</div>

						<div class="formLinea">
							<label id="LabModSolCussp" for="ModSolCussp" class="formLabel formLabel2Izq">CUSSP:</label>
							<%--<asp:TextBox ID="ModSolCussp" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>--%>
							<asp:Label ID="ModSolCussp" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

							<label id="LabModSolNomAfiliado" for="ModSolNomAfiliado" class="formLabel formLabel2Der">Nombre Afiliado:</label>
							<%--<asp:TextBox ID="ModSolNomAfiliado" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>--%>
							<asp:Label ID="ModSolNomAfiliado" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
						</div>

						<div class="formLinea">
							<label id="LabModSolNumAgente" for="ModSolNumAgente" class="formLabel formLabel2Izq">Nro. Agente:</label>
							<%--<asp:TextBox ID="ModSolNumAgente" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>--%>
							<asp:Label ID="ModSolNumAgente" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

							<label id="LabModSolNomAgente" for="ModSolNomAgente" class="formLabel formLabel2Der">Nombre Agente:</label>
							<%--<asp:TextBox ID="ModSolNomAgente" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="True"></asp:TextBox>--%>
							<asp:Label ID="ModSolNomAgente" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>
						</div>

						<asp:Panel ID="LabModSolLineaACOMDCOM" runat="server" CssClass="formLinea">
							<label id="LabModSolACOM" for="ModSolACOM" class="formLabel formLabel2Izq">Porcentaje A:</label>
							<asp:TextBox ID="ModSolACOM" runat="server" CssClass="formTextbox numerico" Width="175" data-v-max="100.00" data-v-min="-100.00" ClientIDMode="Static"></asp:TextBox>
							<%--<asp:Label ID="ModSolACOM" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>--%>

							<label id="LabModSolDCOM" for="ModSolDCOM" class="formLabel formLabel2Der">Porcentaje D:</label>
							<asp:TextBox ID="ModSolDCOM" runat="server" CssClass="formTextbox numerico" Width="180" data-v-max="100.00" data-v-min="0.00" ClientIDMode="Static"></asp:TextBox>
							<%--<asp:Label ID="ModSolDCOM" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>--%>


						</asp:Panel>
						
						<div class="formLinea">
							<label id="LabModSolIndSeleccionado" for="ModSolIndSeleccionado" class="formLabel formLabel2Izq">Ind. Seleccionado:</label>
							<%--<asp:TextBox ID="ModSolIndSeleccionado" runat="server" CssClass="formTextbox formTextboxReadOnly" Width="180" ClientIDMode="Static" ReadOnly="false"></asp:TextBox>--%>
							<asp:Label ID="ModSolIndSeleccionado" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>

							<label id="LabModSolValMontoAcomAgente" for="ModSolValMontoAcomAgente" class="formLabel formLabel2Der">Monto A:</label>
							<asp:TextBox ID="ModSolValMontoAcomAgente" runat="server" CssClass="formTextbox numerico" Width="180" ClientIDMode="Static" ReadOnly="false" data-v-max="0.00" data-v-min="0.00" ></asp:TextBox>
							<%--<asp:Label ID="ModSolValMontoAcomAgente" runat="server" CssClass="formDato" Width="180" ClientIDMode="Static"></asp:Label>--%>
						</div>
					</div>
				</asp:Panel>

				<div align="left" style="width:820px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
					
						<div id="TablaCotizacionesCargando" align="center" style="height:50px;padding:82px 0">
							<asp:Image ID="icoTablaCotizacionesCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
							<span class="texto">Cargando cotizaciones, espere por favor...</span>
						</div>
						
						
						<div id="TablaCotizacionesContenedor" style="display:none"></div>

						<div style="padding: 10px 0" id="Leyenda" runat="server" clientidmode="Static">
							<fieldset>
								<legend>Leyenda</legend>
								<div style="font-family: Calibri; font-size: 13px; color: #0060A9">
									<span style="display: inline-block; width:25px;">*</span>: Pensión debajo de la pensión mínima fijada por la SBS. Modalidad no se cotiza.
								</div>
								<div style="font-family: Calibri; font-size: 13px; color: #0060A9">
									<span style="display: inline-block; width:25px;">**</span>: Cotización no alcanza el mínimo requerido, por lo cual no se simula.
								</div>
								<div style="font-family: Calibri; font-size: 13px; color: #0060A9">
									<span style="display: inline-block; width:25px;">***</span>: Tasa AFP fuera del rago permitido.
								</div>
							</fieldset>
						</div>


						<div class="formLinea" align="center">
							<%--<INIGTI_4081>--%>
							<a id="ModSolGrabarPBSBandeja" style="width:120px;height:22px" class="boton darkblue sharp">Guardar</a>
							<%--<FINGTI_4081>--%>

							<%--<INIGTI_4081>--%>
							<a id="ModSolCalcularPBSBandeja" style="width:130px;height:22px" class="boton darkblue sharp">Calcular PBS</a>
							<%--<FINGTI_4081>--%>
							<a id="ModSolAprobarBandeja" href="javascript:void(0);" style="width:130px;height:22px" class="boton darkblue sharp">Aprobar y Enviar</a>
							<a id="ModSolRechazarBandeja" href="javascript:void(0);" style="width:130px;height:22px" class="boton darkblue sharp">Rechazar</a>
							<a id="ModFlujoSolCancelar" href="javascript:void(0);" style="width:130px;height:22px" class="boton darkblue sharp">Cancelar</a>
						</div>

				</div>

				<%--<INIGTI_4081>--%>
				<asp:HiddenField ID="HGrupoEspeciales" runat="server" Value="" ClientIDMode="Static" />
				<asp:Panel ID="GrupoEspeciales" runat="server" ClientIDMode="Static">
					<div id="AgrupadorEspeciales" class="agrupador">
						<span class="agrupador_titulo_mas">Parámetros Especiales</span>
					</div>
					<div id="DatosEspeciales" align="left"  style="display:none; border:1px solid #00466e;padding:20px">
						<asp:Label Text="TRA Default" runat="server" />

						<div id="TablaTraDefaultCargando" align="center" style="height:50px;padding:82px 0">
							<asp:Image ID="icoTablaTraDefaultCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
							<span class="texto">Cargando TRA Default, espere por favor...</span>
						</div>

						<div id="TablaTraDefaultContenedor" style="display:none"></div>

						<br />

						<asp:Label Text="Tasa de Venta Máxima y TRA Mínimo" runat="server" />

						<div id="TablaTasaMaximaTraMinimaCargando" align="center" style="height:50px;padding:82px 0">
							<asp:Image ID="icoTablaTasaMaximaTraMinimaCargando" ImageUrl="~/Imagenes/ajax-loader_1.gif" runat="server" /><br />
							<span class="texto">Cargando Tasa Máxima y TRA mínima, espere por favor...</span>
						</div>

						<div id="TablaTasaMaximaTraMinimaContenedor" style="display:none"></div>


					</div>
				</asp:Panel>

				<%--<FINGTI_4081>--%>

				


				<%--<div id="ManSolPestanhas">
					<ul>
						<li id="ManSolPes1" data-pestanha="1"><a href="#">Cotización</a></li>
						<li id="ManSolPes2" data-pestanha="2"><a href="#">Beneficiarios <span id="ManSolNumBeneficiarios"></span></a> <span id="ManSolNumBeneficiariosCargando" class="pestanhaCargando"></span></li>
					</ul>

					

					<div id="ManSolPestanha1" align="left" style="width:820px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
						
					</div>

					<div id="ManSolPestanha2" align="left" style="width:820px;padding:20px;margin-top:1px;background:#FFF;border:1px solid #00466e">
						
					</div>
				</div>--%>
			</div>
		</div>
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
				<a id="MCAOAceptarBandeja" class="boton darkblue sharp" style="width:80px">Aceptar</a>
				<a id="MCAOCancelar" class="boton darkblue sharp" style="width:80px">Cancelar</a>
			</div>
		</div>
		<asp:HiddenField ID="MCATablaEliminar" runat="server" ClientIDMode="Static" />
		<%--Fin Modal Cuadro de advertencia--%>

	</div>
</asp:Content>