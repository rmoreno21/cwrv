
/* Determinar si se trata de un dispositivo móvil */
if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
	movil = true;
}
else {
	movil = false;
}

//<SRI.INI-20322_E2>
/* SOLICITUDES */
var idSolicitud;
var Solicitud;
var idCotizacion;
var Correo;
//<SRI.FIN-20322_E2>

$(document).ready(function () {
	var permisoBusAfiExaminarSolicitud = ($('#PerBusAfiExaminarSolicitud_RP').val() == '1' ? true : false);
	var permisoBusAfiBuscar = ($('#PerBusAfiBuscar_RP').val() == '1' ? true : false);
	var permisoGuardar = ($('#PerGuardar_RP').val() == '1' ? true : false);
	var permisoNuevaDireccion = ($('#PerNuevaDireccion_RP').val() == '1' ? true : false);
	var permisoNuevoTelefono = ($('#PerNuevoTelefono_RP').val() == '1' ? true : false);
	var permisoNuevoBeneficiario = ($('#PerNuevoBeneficiario_RP').val() == '1' ? true : false);
	var permisoNuevaSolicitud = ($('#PerNuevoBeneficiario_RP').val() == '1' ? true : false);

	//<SRIINI06326>
	//var consentimientoAfiliado = false;
	//<SRIFIN06326>

	var idSolicitudTmp;
	var fechaCotizacionTmp;

	/*******************\
	|*    COTIZADOR    *|
	\*******************/

	/* Pestañas del Cotizador */
	/* Cambiar de pestaña */
	$('#Pestanhas li').live('click', function () {
		$('#Pestanhas li').attr('class', '');
		$('#Pestanhas div[id^=Pestanha]').hide();
		$('#Pestanha' + $(this).data('pestanha')).show();
		$(this).attr('class', 'seleccionado');

		if ($(this).data('pestanha') != 1) {
			$('#BusAfiBuscar_RP,#BusAfiExaminarSolicitud_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
			$('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').attr('readonly', true);
			$('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').addClass('formTextboxReadOnly');
			botonBusAfiBuscarBloqueado = true;
		}
		else {
			if (permisoBusAfiExaminarSolicitud) $('#BusAfiExaminarSolicitud_RP').attr('class', 'boton darkblue sharp');
			if (permisoBusAfiBuscar) $('#BusAfiBuscar_RP').attr('class', 'boton darkblue sharp');
			$('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').attr('readonly', false);
			$('#BusAfiNroSolicitud_RP,#BusAfiCUSPP_RP').removeClass('formTextboxReadOnly');
			botonBusAfiBuscarBloqueado = false;
		}
	});


	/* BÚSQUEDA DE AFILIADOS */
	var BusAfiApellidoPaterno;
	var BusAfiApellidoMaterno;
	var BusAfiNombres;
	/* Botón Examinar Solicitud */
	$('#BusAfiExaminarSolicitud_RP').live('click', function (e) {
		if (!botonBusAfiBuscarBloqueado && permisoBusAfiExaminarSolicitud) {
			botonesBloqueados = false;
			LimpiarFormularioBusquedaAfiliados();
			$('#ModalBusquedaAfiliados_RP').dialog('open');
			$('#ModBusAfiApellidoPaterno_RP').focus();
		}
		else {
			return false;
		}
	});

	/* Botón Buscar Afiliado */
	$('#ModBusAfiBuscar_RP').live('click', function (e) {
		if (!botonesBloqueados && permisoBusAfiExaminarSolicitud) {
			// Validaciones
			var esCorrecto = true;
			var errores = new Array();

			$("#ModBusAfiApellidoPaterno_RP").removeClass("formTextboxError");
			$("#ModBusAfiApellidoMaterno_RP").removeClass("formTextboxError");
			$("#ModBusAfiNombres_RP").removeClass("formTextboxError");

			// Apellido Paterno
			var apellidoPaterno = true;
			var eApellidoPaterno = false;
			if ($.trim($("#ModBusAfiApellidoPaterno_RP").val()).length > 0) {
				eApellidoPaterno = true;
				if ($.trim($("#ModBusAfiApellidoPaterno_RP").val()).length < 2) {
					errores.push("El campo <strong>Apellido Paterno</strong> debe contener al menos 2 caracteres.");
					apellidoPaterno = false;
				}
			}

			// Apellido Materno
			var apellidoMaterno = true;
			var eApellidoMaterno = false;
			if ($.trim($("#ModBusAfiApellidoMaterno_RP").val()).length > 0) {
				eApellidoMaterno = true;
				if ($.trim($("#ModBusAfiApellidoMaterno_RP").val()).length < 2) {
					errores.push("El campo <strong>Apellido Materno</strong> debe contener al menos 2 caracteres.");
					apellidoMaterno = false;
				}
			}

			// Nombres
			var nombres = true;
			var eNombres = false;
			if ($.trim($("#ModBusAfiNombres_RP").val()).length > 0) {
				eNombres = true;
				if ($.trim($("#ModBusAfiNombres_RP").val()).length < 2) {
					errores.push("El campo <strong>Nombres</strong> debe contener al menos 2 caracteres.");
					nombres = false;
				}
			}

			// Criterio mínimo
			var criterioMinimo = true;
			if (!(eApellidoPaterno | eApellidoMaterno | eNombres)) {
				errores.push("Debe seleccionar al menos un criterio de búsqueda.");
				criterioMinimo = false;
			}

			if (!apellidoPaterno) $('#ModBusAfiApellidoPaterno_RP').addClass('formTextboxError');
			if (!apellidoMaterno) $('#ModBusAfiApellidoMaterno_RP').addClass('formTextboxError');
			if (!nombres) $('#ModBusAfiNombres_RP').addClass('formTextboxError');

			esCorrecto = apellidoPaterno & apellidoMaterno & nombres & criterioMinimo;

			if (esCorrecto) {
				BusAfiApellidoPaterno = $('#ModBusAfiApellidoPaterno_RP').val();
				BusAfiApellidoMaterno = $('#ModBusAfiApellidoMaterno_RP').val();
				BusAfiNombres = $('#ModBusAfiNombres_RP').val();

				$('#TabAfiliadosIndicePagina_RP').val(1);
				$('#TabAfiliadosColumnaOrdenar_RP').val(1);
				$('#TabAfiliadosDireccionOrdenar_RP').val('A');

				CargarTablaAfiliados_RP(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
			}
			else {
				$('#MCMIcono').attr('class', 'validacion');
				$('#MCMContenedor').html(formatearError(errores));
				$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
				$('#ModalCuadroMensaje').dialog('open');
				return false;
			}
		}
		else {
			return false;
		}
	});

	/* Botón de Paginador de Tabla de Afiliados */
	$('#TabAfiliadosPaginador a').live('click', function () {
		if (!botonesBloqueados) {
			var pagina = $(this).data("pag");
			$('#TabAfiliadosIndicePagina_RP').val(pagina);

			CargarTablaAfiliados2(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
		}
		else {
			return false;
		}
	});

	/* Botón de Ordenar de Tabla de Afiliados */
	$('#TabAfiliados a').live('click', function () {
		if (!botonesBloqueados) {
			var columna = $(this).data("col");
			if (columna != $('#TabAfiliadosColumnaOrdenar_RP').attr('value')) {
				$('#TabAfiliadosColumnaOrdenar_RP').val(columna);
				$('#TabAfiliadosDireccionOrdenar_RP').val('A');
			}
			else {
				if ($('#TabAfiliadosDireccionOrdenar_RP').attr('value') == 'A') {
					$('#TabAfiliadosDireccionOrdenar_RP').val('D');
				}
				else {
					$('#TabAfiliadosDireccionOrdenar_RP').val('A');
				}
			}
			$('#TabAfiliadosIndicePagina_RP').val(1);

			CargarTablaAfiliados2(BusAfiApellidoPaterno, BusAfiApellidoMaterno, BusAfiNombres);
		}
	});

	/* Cambiando la fecha de cotizacion */
	$('#ModSolFechaCotizacion_RP').live('change', function () {
		//en el texto se ve: 14/03/2016 (dd/mm/yyyy)
		if ($("#ModSolFechaCotizacion_RP").val() != "") {
			var fecha = $("#ModSolFechaCotizacion_RP").val();
			var mes = fecha.substring(3, 5);
			var dia = fecha.substring(0, 2);
			var anho = fecha.substring(6, 10);
			var dt = new Date(anho + "/" + mes + "/" + dia);

			//if (mes == "12") {
			//    mes = "01";
			//    anho = "0" + (parseInt(anho) + 1);
			//    anho = anho.substring(anho.length, anho.length - 4);
			//} else {
			//    mes ="0" + (parseInt(mes) + 1);
			//    mes = mes.substring(mes.length, mes.length - 2);
			//}
			$("#ModSolFechaDevengue_RP").val("01/" + mes + "/" + anho);

			//var d = new Date();
			var fecvigencia = sumarDias(dt, 15);

			dia = "0" + fecvigencia.getDate();
			mes = "00" + fecvigencia.getMonth();
			anho = fecvigencia.getFullYear();

			dia = dia.substring(dia.length - 2, dia.length);
			mes = mes.substring(mes.length - 2, mes.length);


			$("#ModSolFechaVigencia_RP").val(dia + "/" + mes + "/" + anho);

			//alert(k);

		}
	});

	//<INIGTI_753_3>
	function sumarDias(fecha, dias) {
		fecha.setDate(fecha.getDate() + dias);
		return fecha;
	};

	//<FINGTI_753_3>
	/* Seleccionar registro de afiliado */
	$('#TabAfiliados input[type=radio]').live('change', function () {
		var radio = $(this);
		var filaPadre = $(this).parent().parent();
		var clasePadre = filaPadre.attr('class');
		setTimeout(function () {
			$("#TabAfiliados tbody tr").removeClass("grilla_active");
			$("#TabAfiliados tbody tr:even").addClass("grilla_alt1");
			$("#TabAfiliados tbody tr:odd").addClass("grilla_alt2");

			filaPadre.toggleClass("grilla_active", radio.is(":checked"));
		}, 0);
	});

	/* Botón Cancelar */
	$('#ModBusAfiCancelar_RP').live('click', function () {
		$('#ModalBusquedaAfiliados_RP').dialog('close');
	});

	/* Cerrar Modal */
	$('#ModalBusquedaAfiliados_RP').live("dialogclose", function () {
		if (ajaxTablaAfiliados != null) {
			ajaxTablaAfiliados.abort();
		}
	});

	/* Botón Aceptar */
	$('#ModBusAfiAceptar').live('click', function () {
		if (!botonesBloqueados && permisoBusAfiExaminarSolicitud) {
			// Validar que haya seleccionado una opción de la tabla
			if ($('#TabAfiliados').find('input[type=radio]:checked').length > 0) {
				$('#BusAfiNroSolicitud_RP').val('');
				$('#BusAfiCUSPP_RP').val($('#TabAfiliados input[type=radio]:checked').attr("value"));
				$('#ModalBusquedaAfiliados_RP').dialog('close');
				$('#BusAfiCUSPP_RP').focus();
				$('#BusAfiBuscar_RP').trigger('click');
				// Simuladores
				$('#SimBusAfiBuscar').trigger('click');
			}
			else {
				var errores = new Array();
				errores.push('Por favor seleccione un afiliado de la lista antes de continuar.');

				$('#MCMIcono').attr('class', 'validacion');
				$('#MCMContenedor').html(formatearError(errores));
				$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
				$('#ModalCuadroMensaje').dialog('open');
				return false;
			}
		}
		else {
			return false;
		}
	});

	/* Botón Buscar Solicitud */
	$('#BusAfiBuscar_RP').live('click', function () {

		if (!botonBusAfiBuscarBloqueado && permisoBusAfiBuscar) {
			var esCorrecto = true;
			var errores = new Array();

			$('#BusAfiNroSolicitud_RP').removeClass('formTextboxError');
			$('#BusAfiCUSPP_RP').removeClass('formTextboxError');

			var solicitud = ($.trim($('#BusAfiNroSolicitud_RP').val()).length > 0) ? true : false;
			var cuspp = ($.trim($('#BusAfiCUSPP_RP').val()).length > 0) ? true : false;

			////if (!(solicitud | cuspp)) {
			////    errores.push('Debe ingresar un criterio de búsqueda.');
			////    $('#BusAfiNroSolicitud_RP').addClass('formTextboxError');
			////    $('#BusAfiCUSPP_RP').addClass('formTextboxError');
			////    esCorrecto = false;
			////}

			if (solicitud & cuspp) {
				errores.push('Sólo debe ingresar un criterio de búsqueda.');
				$('#BusAfiNroSolicitud_RP').addClass('formTextboxError');
				$('#BusAfiCUSPP_RP').addClass('formTextboxError');
				esCorrecto = false;
			}

			if (cuspp && $.trim($('#BusAfiCUSPP_RP').val()).length != 12) {
				errores.push('El <strong>CUSPP</strong> debe contener 12 caracteres.');
				$('#BusAfiCUSPP_RP').addClass('formTextboxError');
				esCorrecto = false;
			}

			if (!esCorrecto) {
				$('#MCMIcono').attr('class', 'validacion');
				$('#MCMContenedor').html(formatearError(errores));
				$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
				$('#ModalCuadroMensaje').dialog('open');
				return false;
			}

			botonBusAfiBuscarBloqueado = true;
			$('#BusAfiBuscar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
		}
		else {
			return false;
		}
	});

	/* Verificar si se ha modificado algún campo */
	$('#CorreoElectronico_RP,#Categoria_RP,#AFP_RP,#SaldoCIC_RP').bind('keyup keydown keypress change', function () {
		if ($.trim($('#CUSPP_RP').val()).length > 0) {
			if ($(this).val() != jQuery.data(this, 'lastvalue')) {
				datosSinGuardar = true;
			}
			jQuery.data(this, 'lastvalue', $(this).val());
		}
	});


	function CargandoSolicitud() {
		$('#BeneficiariosOriginales_RP').show();
		$('#ManSolPestanhas li:eq(0)').trigger('click');

		$('#ManSolTipoSolicitud_RP').val($(this).parent('td').parent('tr').children().eq(3).find('span').html());

		botonModSolAceptarBloqueado = false;
		$('#ModSolAceptarCierre_RP').attr('class', 'boton darkblue sharp');

		var params = {
			solicitud: $("#ModSolNroSolicitud_RP").val()
		}
		$.ajax({
			type: 'POST',
			url: 'MantenerSolicitud.aspx/ObtenerCotizacionesBeneficiarios',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {
				Solicitud = data.d;

				// Beneficiarios cotizados
				CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);

				// Grupo familiar
				CargarTablaBeneficiarios_RP(null);

				// Lista de cotizaciones
				CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());
				
				//Muestra el Tipo de Cambio segun la moneda
				if ($("#ModSolMonedaPrimaUnica_RP").val() == "001") {
					$("#ModSolTipoCambioPanel").hide();
				} else {
					$("#ModSolTipoCambioPanel").show();
				};

				$('#ModSolSaldoCIC_RP').focus();
				$('#ModSolDCOM_RP').focus();
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
				$('#ModalSolicitud').dialog('close');
			},
			complete: function () {

			}
		});
	}

	/* Botón Modificar */
	$('#TabSolicitudes_RP .grilla_editar').live('click', function () {

		var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
		idSolicitud = $(this).data('solicitud');
		var params = {
			idSolicitud: idSolicitud,
			fecCotizacion: fecCotizacion,
			accion: 'M'
		}

		$.ajax({
			type: 'POST',
			url: 'Cotizador.aspx/SessionIdSolicitud',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {

				var solicitud = data.d;
				if (idSolicitud.toString() == solicitud.toString()) {
					window.location.href = "MantenerSolicitud.aspx";
				}
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
				$('#ModalSolicitud').dialog('close');
			}
		});



	});

	//<INIGTI_753_3>

	/*Botón copiar*/
	$('#TabSolicitudes_RP .grilla_copiar').live('click', function () {

		var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
		idSolicitud = $(this).data('solicitud');
		var params = {
			idSolicitud: idSolicitud,
			fecCotizacion: fecCotizacion,
			accion: 'C'
		}

		$.ajax({
			type: 'POST',
			url: 'Cotizador.aspx/SessionIdSolicitud',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {

				var solicitud = data.d;
				if (idSolicitud.toString() == solicitud.toString()) {
					window.location.href = "MantenerSolicitud.aspx";
				}
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
				$('#ModalSolicitud').dialog('close');
			}
		});



	});

	/*Botón Consultar*/
	$('#TabSolicitudes_RP .grilla_consultar').live('click', function () {

		var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
		idSolicitud = $(this).data('solicitud');
		var params = {
			idSolicitud: idSolicitud,
			fecCotizacion: fecCotizacion,
			accion: 'CONS'
		}

		$.ajax({
			type: 'POST',
			url: 'Cotizador.aspx/SessionIdSolicitud',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {

				var solicitud = data.d;
				if (idSolicitud.toString() == solicitud.toString()) {
					window.location.href = "MantenerSolicitud.aspx";
				}
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
				$('#ModalSolicitud').dialog('close');
			}
		});



	});

	//<FINGTI_753_3>
	//<INIGTI_7012>
	$('#TabSolicitudes_RP .grilla_cerrar').live('click', function () {
		$("#MCIcono").attr("class", "cargando");
		$("#MCContenedor").html("Cargando la información de la solicitud, por favor espere un momento...");
		$("#ModalCotizando").dialog({ title: "Cargando" });
		$("#ModalCotizando").dialog("open");

		var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
		idSolicitud = $(this).data('solicitud');
		var tipo_cotizacion = $(this).data('tipo_cotizacion');
		var params = {
			idSolicitud: idSolicitud,
			fecCotizacion: fecCotizacion,
			accion: 'CERRAR'
		}

		$.ajax({
			type: 'POST',
			url: 'Cotizador.aspx/SessionIdSolicitud',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {

				var solicitud = data.d;
				if (tipo_cotizacion == "RPP") {
					if (idSolicitud.toString() == solicitud.toString()) {
						window.location.href = "CerrarSolicitud.aspx";
					}
				} else {
					//alert("Llamar a RentaIFP\CerrarSolicitud.aspx");
					window.location.href = "../RentaIFP/CerrarSolicitud.aspx";
				}
				
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la dirección.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
				$('#ModalSolicitud').dialog('close');
				$("#ModalCotizando").dialog("close");
			}
		});



	});

	//<FINGTI_7012>

	//<INI.GTI_7012_V13>
	$('#TabSolicitudes_RP .grilla_grupo_familiar').live('click', function () {

		//var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
		var idSolicitud = $(this).data('solicitud');
		var fecSoltud = $(this).parent('td').parent('tr').children().eq(1).html();
		var estadoSoltud = $(this).data('estado');

		var cantidad = 0;

		//<INI.GTI_7012_20>
		var params = {
			idGrupoFamiliar: cantidad,
			solitud: idSolicitud,
			fecha: fecSoltud,
			estado: estadoSoltud,
			paginaLlamada: '../RentaPrivadaPlus/CerrarSolicitud.aspx'
		}
		//<FIN.GTI_7012_20>

		$.ajax({
			type: 'POST',
			url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/SessionIdGrupoFamiliar',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			//data: "{idGrupoFamiliar:'" + cantidad + "', solitud: '" + idSolicitud + "', fecha: '" + fecSoltud + "', estado: '" + estadoSoltud + "'}",//<INI.GTI_7012_20>
			data: $.toJSON(params),
			success: function (data) {

				////<INI.GTI_7012_20>
				////if (estadoSoltud == 1) {

				$.ajax({
					type: 'POST',
					url: '../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx/ObtenerListaGruposFamiliares',
					contentType: "application/json; charset=iso-8859-1",
					dataType: 'json',
					data: "{idSolicitud:'" + idSolicitud + "', posicion: '0'}",

					success: function (data) {
						window.location.href = "../RentaPrivadaPlus/GrupoFamiliarAfiliadoCierre.aspx";
					},
					error: function (XMLHttpRequest, textStatus, errorThrown) {
						if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
							document.location.reload(true);
						}
						else {
							$('#MCMIcono').attr('class', 'error');
							$('#MCMContenedor').html('Ha ocurrido un error al cargar la información del beneficiario.');
							$('#ModalCuadroMensaje').dialog({ title: 'Error' });
							$('#ModalCuadroMensaje').dialog('open');
						}
						$('#ModalGrupoFamiliar').dialog('close');
					}
				});
				////}
				////else {
				////    $('#MCMIcono').attr('class', 'validacion');
				////    $('#MCMContenedor').html('La solicitud se encuentra cerrada.');
				////    $('#ModalCuadroMensaje').dialog({ title: 'Validación' });
				////    $('#ModalCuadroMensaje').dialog('open');
				////}
				////<FIN.GTI_7012_20>
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información del beneficiario.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
				$('#ModalGrupoFamiliar').dialog('close');
			}
		});


	});

	//<FIN.GTI_7012_V13>
	$('#TablaRviBenefiReintentar_RP').live('click', function () {
		$('#TablaRviBenefiError_RP').hide();
		CargarTablaBeneficiarios_RP(Solicitud.Beneficiarios);
	});

	/* Botón PDF */
	$("#TabSolicitudes_RP .grilla_pdf,#TabCotizacionCotizaciones .grilla_pdf").live("click", function () {

		var idSolicitud = $(this).data('solicitud');
		//var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
		var fecCotizacion = $(this).data('fechacotizacion');
		var numAgente = $(this).data("numagente");

		var params = {
			idSolicitud: idSolicitud,
			fecCotizacion: fecCotizacion,
			numAgente: numAgente
		}

		$.ajax({
			type: 'POST',
			url: 'Cotizador.aspx/ExportarSolicitudPDF',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {
				if (data.d.Estado == "OK") {
					//if (jQuery.browser.mobile) {
					if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
						window.location.href = "../Reportes/DetallePropuestaPlusMovil.aspx";
					}
					else {
						var w = 800;
						var h = 600;
						var left = (screen.width / 2) - (w / 2);
						var top = (screen.height / 2) - (h / 2);
						//<SOLINI26593>
						var nuevaVentana = window.open("../Reportes/DetallePropuestaPlus.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
						//<SOLFIN26593>
						//var nuevaVentana = window.open("../Reportes/DetalleCotizacion.aspx", "", "width=800,height=600,scrollbars=1,location=no,menubar=no,resizable=1,status=no,toolbar=no");
					}

					/*<SRIINI17003>*/
					selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
					$clock.countdown(selectedDate.toString());
					/*<SRIFIN17003>*/
				}
				else {
					$('#MCMIcono').attr('class', data.d.Icono);
					$('#MCMContenedor').html(data.d.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
					$('#ModalCuadroMensaje').dialog('open');
				}
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al exportar la la solicitud.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
			}
		});
	});

	/*<SRI.INI-20322>*/
	function DeslizarPantallaCorreo() {
		// Deslizar la pantalla hacia los datos de simulación
		//$('html,body').animate({
		//    scrollTop: $('#Simulaciones').offset().top
		//}, 'slow');
	}
	/*<SRI.FIN-20322>*/

	/* Botón Correo Electrónico */
	$('#TabSolicitudes_RP .grilla_email').live('click', function () {
		if ($.trim($('#CorreoElectronicoRegistrado_RP').val()).length > 0) {
			$('#ModEnvCorCargando').show();

			var idSolicitud = $(this).data('solicitud');
			var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
			var tipoCotizacion = $(this).parent('td').parent('tr').children().eq(2).find('span').html();

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				idSolicitud: idSolicitud,
				fecCotizacion: fecCotizacion,
				tipoCotizacion: tipoCotizacion,
				nombre: $.trim($('#Nombres_RP').val()),
				apellidoPaterno: $.trim($('#ApellidoPaterno_RP').val()),
				apellidoMaterno: $.trim($('#ApellidoMaterno_RP').val()),
				sexo: $('#Sexo_RP').val()
			}

			LimpiarFormularioCorreo();

			$.ajax({
				type: 'POST',
				url: 'Cotizador.aspx/CrearDatosCorreo',
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					/* Correo Creado */
					Correo = data.d;
					if (Correo.Respuesta.Estado == 'OK') {
						$('#ModEnvCorDe').val(Correo.De);
						Correo.Para = $('#CorreoElectronicoRegistrado_RP').val();
						$('#ModEnvCorPara').val(Correo.Para);
						$('#ModEnvCorAsunto').val(Correo.Asunto);
						$('#ModEnvCorAdjunto').html(Correo.Adjunto);
						$('#ModEnvCorMensaje').val(Correo.Mensaje);
						localStorage.setItem('pdf_cotizacion', Correo.BinarioAdjunto);
						$('#ModEnvCorCargando').fadeOut();
						$('#ModEnvCorMensaje').focus();
					}
					else if (Correo.Respuesta.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}
					else {
						$('#MCMIcono').attr('class', Correo.Respuesta.Icono);
						$('#MCMContenedor').html(Correo.Respuesta.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: Correo.Respuesta.Titulo });
						$('#ModalCuadroMensaje').dialog('open');

						$('#ModalEnvioCorreo').dialog('close');
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						document.location.reload(true);
					}
					else {
						$('#MCMIcono').attr('class', 'error');
						$('#MCMContenedor').html('Ha ocurrido un error al crear los datos del correo electrónico.');
						$('#ModalCuadroMensaje').dialog({ title: 'Error' });
						$('#ModalCuadroMensaje').dialog('open');
					}
					$('#ModalEnvioCorreo').dialog('close');
				}
			});

			$('#ModalEnvioCorreo').dialog('open');
		}
		else {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html('No se tiene registrado un Correo Electrónico para el afiliado. Por favor ingrese uno en la pestaña <strong>Datos del Afiliado</strong> y luego presione el botón <strong>Guardar</strong>.');
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	});

	//<SRIINI06326>

	/* Botón Enviar Correo Electrónico */
	$('#ModEnvCorEnviar').live('click', function () {
		$('#ModEnvCorCargando').fadeIn();

		Correo.Mensaje = $('#ModEnvCorMensaje').val();
		Correo.Respuesta = null;
		Correo.BinarioAdjunto = localStorage.getItem("pdf_cotizacion");

		var params = {
			//tokenUsuario: $('#TokenUsuario').val(),
			correo: Correo,
			agente: null
		}
		fetch($('#url_api_rentas_rv').val() + '/cotizacion-oficial/enviar-correo-electronico', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json; charset=iso-8859-1',
				'x-username': $('#usuario_actual').val(),
				'x-rol': $('#rol_azman').val()
			},
			body: JSON.stringify(params)
		})
			.then(response => {
				if (!response.ok) {
					if (response.status === 401 || response.status === 12030) {
						document.location.reload(true);
						return;
					}
					throw new Error('Error en la respuesta del servidor');
				}
				return response.json();
			})
			.then(data => {
				if (data.data.Respuesta.Estado == 'OK') {
					$('#ModalEnvioCorreo').dialog('close');

					$('#MCMIcono').attr('class', data.data.Respuesta.Icono);
					$('#MCMContenedor').html(data.data.Respuesta.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
					$('#ModalCuadroMensaje').dialog('open');

					/*<SRIINI17003>*/
					selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
					$clock.countdown(selectedDate.toString());
					/*<SRIFIN17003>*/
				}
				else if (data.data.Respuesta.Estado == 'TOKEN') {
					CerrarSesionExpirada();
				}
				else {
					$('#MCMIcono').attr('class', data.data.Respuesta.Icono);
					$('#MCMContenedor').html(data.data.Respuesta.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.data.Respuesta.Titulo });
					$('#ModalCuadroMensaje').dialog('open');
				}
			})
			.catch(error => {
				if (error.status === 401 || error.status === 12030) {
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');

					$('#ModEnvCorCargando').fadeOut();
				}
			});
		//$.ajax({
		//	type: 'POST',
		//	url: 'Cotizador.aspx/EnviarCorreoElectronico',
		//	contentType: "application/json; charset=iso-8859-1",
		//	dataType: 'json',
		//	data: $.toJSON(params),
		//	success: function (data) {
		//		if (data.d.Estado == 'OK') {
		//			$('#ModalEnvioCorreo').dialog('close');

		//			$('#MCMIcono').attr('class', data.d.Icono);
		//			$('#MCMContenedor').html(data.d.Mensaje);
		//			$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
		//			$('#ModalCuadroMensaje').dialog('open');

		//			/*<SRIINI17003>*/
		//			selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
		//			$clock.countdown(selectedDate.toString());
		//			/*<SRIFIN17003>*/
		//		}
		//		else if (data.d.Estado == 'TOKEN') {
		//			CerrarSesionExpirada();
		//		}
		//		else {
		//			$('#MCMIcono').attr('class', data.d.Icono);
		//			$('#MCMContenedor').html(data.d.Mensaje);
		//			$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
		//			$('#ModalCuadroMensaje').dialog('open');
		//		}
		//	},
		//	error: function (XMLHttpRequest, textStatus, errorThrown) {
		//		if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
		//			document.location.reload(true);
		//		}
		//		else {
		//			$('#MCMIcono').attr('class', 'error');
		//			$('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
		//			$('#ModalCuadroMensaje').dialog({ title: 'Error' });
		//			$('#ModalCuadroMensaje').dialog('open');

		//			$('#ModEnvCorCargando').fadeOut();
		//		}
		//	}
		//});
	});

	/* Botón Cancelar Correo Electrónico */
	$('#ModEnvCorCancelar').live('click', function () {
		$('#ModalEnvioCorreo').dialog('close');
		//<SRI.INI-20322>
		$('#ModalEnvioCorreoSimulador').dialog('close');
		//<SRI.FIN-20322>
	});

  
	/* Seleccionar beneficiario en tabla de beneficiarios */
	$('#TabBeneficiarios_RP input[type=checkbox]').live('change', function () {
		$('#ManSolNumBeneficiarios_RP').html('(' + $('#TabBeneficiarios_RP input[type=checkbox]:checked').length + ')');
	});

	/* Seleccionar cotización */
	//<INIGTI_7012>
	$('#TabCotizaciones_RP input[type=radio]').live('change', function () {
		var radio = $(this);
		var filaPadre = radio.parent().parent();

		var celda = $(this).parent('td');
		var fila = celda.parent('tr');
		var fil = fila.parent('tbody').children().index(fila);

		var clasePadre = filaPadre.attr('class');
		setTimeout(function () {
			$("#TabCotizaciones_RP tbody tr").removeClass("grilla_active");
			$("#TabCotizaciones_RP tbody tr:even").addClass("grilla_alt1");
			$("#TabCotizaciones_RP tbody tr:odd").addClass("grilla_alt2");

			filaPadre.toggleClass("grilla_active", radio.is(":checked"));
		}, 0);

	});
	//<FINGTI_7012>

	/* Botón Cancelar Cierre */
	$('#ModSolCancelar_RP').live('click', function () {
		window.location.href = "ListadoCierrePlus.aspx";
	});

	//<INIGTI_7012>
	/* Botón Aceptar Cierre*/
	$("#ModSolAceptarCierre_RP").live("click", function () {

		if (!botonModSolAceptarBloqueado) {
			$("#lineaCausante").hide();
			// Seleccionar Cotización
			var idSolicitudElegida = 0;

			if ($("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
				idSolicitudElegida = $("#TabCotizaciones_RP input[type=radio]:checked").val();
			}

			if (idSolicitudElegida == 0) {
				$('#MCMIcono').attr('class', 'validacion');
				$('#MCMContenedor').html('Seleccione una <strong>Cotización</strong>. Dato Obligatorio.');
				$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
				$('#ModalCuadroMensaje').dialog('open');
				return;
			}
			
			
			$("#MCAIcono").attr("class", "advertencia");
			$("#MCAContenedor").html("¿Deseas generar la Póliza de la cotización seleccionada?");
			$("#HCondicion").val("GENERAR");
			$("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
			$("#ModalCuadroAdvertencia").dialog("open");
		}
		else {
			return false;
		}
	});
	//<FINGTI_7012>

	//<INIGTI_7012>
	/* Botón Anular Cierre*/
	$("#ModSolAnularCierre_RP").live("click", function () {

		if (!botonModSolAnularBloqueado) {
			$("#lineaCausante").show();
			$("#MCAIcono").attr("class", "advertencia");
			$("#MCAContenedor").html("¿Deseas Anular la Solicitud?");
			$("#ModalCuadroAdvertencia").dialog({ title: "Confirmación" });
			$("#ModalCuadroAdvertencia").dialog("open");
			$('#HCondicion').val('ANULAR');
		}
		else {
			return false;
		}
	});
	//<FINGTI_7012>

	/* CUADRO DE MENSAJE */
	/* Validaciones desde servidor */
	if ($('#MCMEstado').val() == '1') {
		if ($(document).width() >= 1000) {
			$('#MCMIcono').attr('class', $('#MCMEstadoIcono').val());
			$('#ModalCuadroMensaje').dialog({ title: $('#MCMEstadoTitulo').val() });
			$('#ModalCuadroMensaje').dialog({ position: ['center', 'center'] });
			$('#ModalCuadroMensaje').dialog('open');
		}
		else {
			$('#MensajeMovil').attr("class", $('#MCMEstadoIcono').val() + "_movil");
			$("#MensajeMovil").fadeIn();
		}
	}

	/* Botón Aceptar */
	$('#MCMAceptar').live('click', function () {
		$('#ModalCuadroMensaje').dialog('close');
	});

	/* CUADRO DE ADVERTENCIA */

	/* Botón Cancelar */
	$('#MCACancelar_Plus').live('click', function () {
		$('#ModalCuadroAdvertencia').dialog('close');
	});

	//<INIGTI_7012>
	/* Botón Aceptar Cierre*/
	$('#MCAAceptarCierre_Plus').live('click', async function () {
		if (!botonModSolAceptarBloqueado || !botonModSolAnularBloqueado) {
			
			if ($("#HCondicion").val() == "GENERAR") {

				$("#ModalCuadroAdvertencia").dialog("close");

				// Seleccionar Cotización
				var idSolicitudElegida = 0;

				if ($("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
					idSolicitudElegida = $("#TabCotizaciones_RP input[type=radio]:checked").val();
				}

				$("#ModSolCargando").fadeIn();
				$("#MCIcono").attr("class", "cargando");
				$("#MCContenedor").html("Generando Póliza, por favor espere un momento...");
				$("#ModalCotizando").dialog({ title: "Generando" });
				$("#ModalCotizando").dialog("open");


				try {
					const opcionesSistema = JSON.parse(localStorage.getItem("opcionesSistema") || '[]');
					if (opcionesSistema.some(o => o.IdAzman == EnumsPermisos.SolicitudPlusGenerarPoliza && o.Activa)) {
				var params = {
					numCorrelativo: idSolicitudElegida
						}

						try {
							$('#ModSolCargando').fadeIn();
							const data = await ApiCotizadorIFP.GenerarPoliza(params, $("#ModSolNroSolicitud_RP").html());

							if (data.Estado === 'OK') {
								// Cambiar la modal a modo de modificación
								$('#ModSolModo').val('');

								// Cerrar modal de espera
								$('#ModalCotizando').dialog('close');

								// Mostrar mensaje de éxito
								$('#MCMIcono').attr('class', data.Icono);
								$('#MCMContenedor').html(data.Mensaje);
								$('#ModalCuadroMensaje').dialog({ title: data.Titulo });
								$('#ModalCuadroMensaje').dialog('open');

								botonModSolAceptarBloqueado = true;
								$('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
								$("#HCondicion").val("OK");

								//setTimeout("$('#ModalCotizando').dialog('close');", 2000);
								setTimeout(function () {
									window.location.href = "CerrarSolicitud.aspx";
								}, 3000);

							}
							else if (data.Estado === 'TOKEN') {
								CerrarSesionExpirada();
							}
							else {
								$('#ModalCotizando').dialog('close');

								$('#ModalCuadroMensaje').dialog({ title: data.Titulo });
								$('#MCMIcono').attr('class', data.Icono);
								$('#MCMContenedor').html(data.Mensaje);
								$('#ModalCuadroMensaje').dialog('open');
								$('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');

								setTimeout(function () {
									window.location.href = "CerrarSolicitud.aspx";
								}, 3000);
							}
						} catch (error) {
							if (error.status === 401 || error.status === 12030) {
								document.location.reload(true);
							}
							else {
								$('#MCMIcono').attr('class', 'error');
								$('#MCMContenedor').html('Ha ocurrido un error al generar la poliza de la solicitud.');
								$('#ModalCuadroMensaje').dialog({ title: 'Error' });
								$('#ModalCuadroMensaje').dialog('open');

								$('#ModalCotizando').dialog('close');
							}
						} finally {
							$('#ModSolCargando').fadeOut();
						}

					} else {
						$('#MCMIcono').attr('class', 'error');
						$('#MCMContenedor').html("Usted no tiene privilegios sobre esta opción.");
						$('#ModalCuadroMensaje').dialog({ title: 'Error' });
						$('#ModalCuadroMensaje').dialog('open');
						$('#ModalCotizando').dialog('close');
					}
				} catch (error) {
					console.error('Error al validar permisos:', error);
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html("Error al validar los permisos del usuario.");
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
					$('#ModalCotizando').dialog('close');
				}

				

			};

			//Anular la Solicitud
			if ($("#HCondicion").val() == "ANULAR") {

				if ($("#ModSolCausante_RP").val() == "0") {
					$('#MCMIcono').attr('class', 'validacion');
					$('#MCMContenedor').html('Seleccione Causal de Anulación.');
					$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
					$('#ModalCuadroMensaje').dialog('open');
					return;
				}


				$("#ModalCuadroAdvertencia").dialog("close");

				$("#ModSolCargando").fadeIn();
				$("#MCIcono").attr("class", "cargando");
				$("#MCContenedor").html("Anulando la Solicitud, por favor espere un momento...");
				$("#ModalCotizando").dialog({ title: "Anulando" });
				$("#ModalCotizando").dialog("open");

				try {
					const opcionesSistema = JSON.parse(localStorage.getItem("opcionesSistema") || '[]');
					if (opcionesSistema.some(o => o.IdAzman == EnumsPermisos.SolicitudPlusCerrar && o.Activa)) {
						var params = {
							codCausante: $("#ModSolCausante_RP").val()
						}
						try {
							$('#ModSolCargando').fadeIn();
							const data = await ApiCotizadorIFP.AnularPoliza(params, $("#ModSolNroSolicitud_RP").html());
							if (data.Estado == 'OK') {
								$('#ModSolModo').val('');
								$('#ModalCotizando').dialog('close');
								$('#MCMIcono').attr('class', data.Icono);
								$('#MCMContenedor').html(data.Mensaje);
								$('#ModalCuadroMensaje').dialog({ title: data.Titulo });
								$('#ModalCuadroMensaje').dialog('open');
								botonModSolAceptarBloqueado = true;
								$('#ModSolAceptarCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
								botonModSolAnularBloqueado = true;
								$('#ModSolAnularCierre_RP').attr('class', 'botonDeshabilitado gris gris_sharp');
								setTimeout(function () {
									window.location.href = "CerrarSolicitud.aspx";
								}, 1000);
							}
							else if (data.Estado == 'TOKEN') {
								CerrarSesionExpirada();
							}
							else {
								$('#ModalCotizando').dialog('close');
								$('#ModalCuadroMensaje').dialog({ title: data.Titulo });
								$('#MCMIcono').attr('class', data.Icono);
								$('#MCMContenedor').html(data.Mensaje);
								$('#ModalCuadroMensaje').dialog('open');
								$('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
							}
						} catch (error) {
							if (error.status == 401 || error.status == 12030) {
								document.location.reload(true);
							}
							else {
								$('#MCMIcono').attr('class', 'error');
								$('#MCMContenedor').html('Ha ocurrido un error al anular la información de la solicitud.');
								$('#ModalCuadroMensaje').dialog({ title: 'Error' });
								$('#ModalCuadroMensaje').dialog('open');
								$('#ModalCotizando').dialog('close');
							}
						} finally {
							$('#ModSolCargando').fadeOut();
						}
					} else {
						$('#MCMIcono').attr('class', 'error');
						$('#MCMContenedor').html("Usted no tiene privilegios sobre esta opción.");
						$('#ModalCuadroMensaje').dialog({ title: 'Error' });
						$('#ModalCuadroMensaje').dialog('open');
						$('#ModalCotizando').dialog('close');
					}
				} catch (error) {
					console.error('Error al validar permisos:', error);
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html("Error al validar los permisos del usuario.");
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
					$('#ModalCotizando').dialog('close');
				}				

			};


		}
		else {
			return false;
		}
	});

	//<FINGTI_7012>

	/* CERRAR SESIÓN */
	/* Botón Cerrar Sesión */
	$('#MenuSalir').live('click', function () {
		if (datosSinGuardar) {
			$('#ModalCerrarSesion').dialog('open');
			return false;
		}
	});

	/* Botón Aceptar */
	$('#MCSCancelar').live('click', function () {
		$('#ModalCerrarSesion').dialog('close');
	});


	$("#MCAOCancelar").live("click", function () {
		$("#ModalCuadroAdvertencia").dialog("close");
	});


	//<SRI.FIN-20322_E2>

	/* Calcular monto de ACOM - Inicio */
	$("#ModSolACOM,#ModSolIndSeleccionado,#TabCotizaciones_RP input[type=radio]").live("change", function () {
		if (!botonModSolAceptarBloqueado) {
			// Validar sólo si existe el textbox de Monto ACOM (oficiales)
			if ($("#ModSolValMontoAcomAgente").length > 0) {
				if ($("#ModSolACOM").val() > 0 && $("#ModSolIndSeleccionado").val() == "S" && $("#TabCotizaciones_RP input[type=radio]:checked").length > 0) {
					// Bloquear botón Aceptar
					$("#ModSolMontoACOMCargando").show();
					botonModSolAceptarBloqueado = true;
					$("#ModSolAceptarOficial").attr("class", "botonDeshabilitado gris gris_sharp");

					var params = {
						tokenUsuario: $('#TokenUsuario').val(),
						solicitud: Solicitud.Id,
						acom: $("#ModSolACOM").val(),
						cotizacion: $("#TabCotizaciones_RP input[type=radio]:checked").val()
					}
					$.ajax({
						type: "POST",
						url: rutaObtenerMontoACOM,
						contentType: "application/json; charset=iso-8859-1",
						data: $.toJSON(params),
						dataType: "json",
						success: function (data) {
							$("#ModSolValMontoAcomAgente").val(data.d.Contenido);
							$("#ModSolValMontoAcomAgente").autoNumeric("update", { vMax: data.d.Contenido, aDec: ".", aSep: "," });
						},
						error: function (XMLHttpRequest, textStatus, errorThrown) {
							if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
								document.location.reload(true);
							}
							else {
								$("#MCMIcono").attr("class", "error");
								$("#MCMContenedor").html("Ha ocurrido un error al cargar el Monto A.");
								$("#ModalCuadroMensaje").dialog({ title: "Error" });
								$("#ModalCuadroMensaje").dialog("open");
							}
						},
						complete: function () {
							$("#ModSolMontoACOMCargando").hide();
							botonModSolAceptarBloqueado = false;
							$("#ModSolAceptarOficial").attr("class", "boton darkblue sharp");
						}
					});
				}
				else {
					$("#ModSolValMontoAcomAgente").val("0.00");
					$("#ModSolValMontoAcomAgente").autoNumeric("update", { vMax: "0.00" });
				}
			}
		}
	});
	/* Calcular monto de ACOM - Fin */

	if ($('#ModSolModo').val() == "M" || $('#ModSolModo').val() == "C" || $('#ModSolModo').val() == "CONS"
		|| $('#ModSolModo').val() == "CERRAR") {
		CargandoSolicitud();
	};


	//<INIGTI_753>
	$('#ManSolPes1').live('click', function () {
		//var valida = false;
		//for (i = 0; i < $("#TabBeneficiarios_RP tbody tr").length; i++) {
		//    if ($("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").is(":checked")) {
		//        var idParentesco = $("#TabBeneficiarios_RP tbody tr:eq(" + i + ") input").data("parentesco");
		//        if (idParentesco != "80") {
		//            if (idParentesco == "10") {
		//                valida = true;
		//            } else {
		//                valida = false;
		//            }
		//            if (valida == false) {
		//                break;
		//            }
		//        }
		//    }
		//}

		//if (valida == true) {
		//    $("#HConyuge").val("TRUE");
		//} else {
		//    for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
		//        Solicitud.Cotizaciones[i].ValPjeConyuge = "0";
		//    }
		//    $("#HConyuge").val("FALSE");
		//}

		//CargarTablaCotizacionesCierre_RP(Solicitud.Cotizaciones, $("#ModSolTemporalidad_RP").val(), $("#ModSolMonedaPrimaUnica_RP").val(), $("#HConyuge").val());

	});

	//<INIGTI_753>

	//<FINGTI_753>

	


});


//<INI.GTI_7012_3>
function MostrarModal(mensaje) {

	$('#ModalCuadroMensaje').dialog({
		autoOpen: false,
		resizable: false,
		width: 500,
		minHeight: 130,
		show: "fade",
		hide: "explode",
		modal: true
	});

	$('#MCMIcono').attr('class', 'validacion');
	$('#MCMContenedor').html(mensaje);
	$('#ModalCuadroMensaje').dialog({ title: 'Error' });
	$('#ModalCuadroMensaje').dialog('open');
};
//<FIN.GTI_7012_3>

function formatearError(errores) {
	var cadena = '';
	for (var i = 0; i < errores.length; i++) {
		cadena += '<div style="margin:5px 0">' + errores[i] + '</div>';
	}
	return cadena;
}

function LimpiarFormularioBusquedaAfiliados() {
	$('#ModBusAfiApellidoPaterno_RP').removeClass('formTextboxError');
	$('#ModBusAfiApellidoMaterno_RP').removeClass('formTextboxError');
	$('#ModBusAfiNombres_RP').removeClass('formTextboxError');

	$('#ModBusAfiApellidoPaterno_RP').val('');
	$('#ModBusAfiApellidoMaterno_RP').val('');
	$('#ModBusAfiNombres_RP').val('');

	$('#ModBusAfiBuscar_RP').attr('class', 'boton darkblue sharp');

	$('#ModBusAfiCargando_RP').hide();
	$('#ModBusAfiTablaAfiliados_RP').hide();
	$('#TablaAfiliadosError_RP').hide();

	$('#TabAfiliadosIndicePagina_RP').val('1');
	$('#TabAfiliadosTamanhoPagina_RP').val('10');
	$('#TabAfiliadosColumnaOrdenar_RP').val('1');
	$('#TabAfiliadosDireccionOrdenar_RP').val('A');
}



function LimpiarFormularioCorreo() {
	$('#ModEnvCorDe').val('');
	$('#ModEnvCorPara').val('');
	$('#ModEnvCorAsunto').val('');
	$('#ModEnvCorMensaje').val('');
}

//function formatearMonto(val) {
//    return String(val).split("").reverse().join("")
//                      .replace(/(.{3}\B)/g, "$1,")
//                      .split("").reverse().join("");
//}

function formatearMonto(nStr) {
	nStr += '';
	x = nStr.split('.');
	x1 = x[0];
	x2 = x.length > 1 ? '.' + x[1] : '';
	var rgx = /(\d+)(\d{3})/;
	while (rgx.test(x1)) {
		x1 = x1.replace(rgx, '$1' + ',' + '$2');
	}
	return x1 + x2;
}

function formatearMonto2(nStr) {
	nStr += '';
	x = nStr.split('.');
	x1 = x[0];
	x2 = x.length > 1 ? '.' + x[1] : '.00';
	//var rgx = /(\d+)(\d{3})/;
	//while (rgx.test(x1)) {
	//    x1 = x1.replace(rgx, '$1' + '' + '$2');
	//}
	return x1 + x2;
}

//<SRI.INI_20322_E2>
function divexpandcollapse(divname) {
	var img = "img" + divname;
	var htmlTable = "";
	if ($("#" + img).attr("src") == "../Imagenes/plus-icon.png") {
		htmlTable = $("#" + divname).html();
		$("#" + divname).remove();
		$("#" + img)
			.closest("tr")
			.after("<tr><td></td><td colspan = '100%'>" + htmlTable + "</td></tr>")
		$("#" + img).attr("src", "../Imagenes/minus-icon.png");
	} else {
		$("#" + img).closest("tr").next().css("display", "none");
		htmlTable = $("#" + img).closest("tr").next().html();
		htmlTable = htmlTable.replace("<td></td><td colspan=\"100%\">", "");
		htmlTable = htmlTable.substring(1, htmlTable.length - 5);
		htmlTable = htmlTable.replace("<div>", "<div id=\"" + divname + "\"  style=\"display:none\"> <div>");
		htmlTable = htmlTable + "</div>";
		$("#" + img).closest("tr").next().remove();
		$("#hrf" + divname).after(htmlTable);
		$("#" + img).attr("src", "../Imagenes/plus-icon.png");
	}
}
//<SRI.FIN_20322_E2>


//<INIGTI_753>
function ObtenerMonedaAjuste(codMoneda, objeto, fil) {
	$.ajax({
		type: 'POST',
		url: 'MantenerSolicitud.aspx/ObtenerMonedaAjuste',
		contentType: "application/json; charset=iso-8859-1",
		dataType: 'json',
		data: "{codMoneda:'" + codMoneda + "'}",
		success: function (data) {
			var objdata = data.d;
			objeto.empty();
			for (var n = 0; n < objdata.length; n++) {
				objeto[0].options[n] = new Option(objdata[n].Valor_1, objdata[n].Valor_2);
			};


			if ($('#ModSolModo').val() == "CONS") {
				objeto.attr('disabled', 'disabled');
			}
			else if ($('#ModSolModo').val() == "N") {
				objeto.removeAttr('disabled', 'disabled');
			}
			else {
				if ($("#HBloqueo").val() == "TRUE") {
					objeto.attr('disabled', 'disabled');
				} else {
					objeto.removeAttr('disabled', 'disabled');
				}

			}



			if (objdata[0].Valor_1 == "-") {
				objeto.attr('disabled', 'disabled');
				Solicitud.Cotizaciones[fil].ValMonAju = "-1";
				objeto.val("-1");
				//fila.find('.ModSolAjusteMoneda').removeAttr('disabled', 'disabled');
				//if (fila.find('.ModSolAjusteMoneda').val() == "-") {
				//    fila.find('.ModSolAjusteMoneda').attr('disabled', 'disabled');
				//}
			} else {
				if (Solicitud.Cotizaciones[fil].ValMonAju == "-1") {
					Solicitud.Cotizaciones[fil].ValMonAju = objdata[0].Valor_2;
					objeto.val(Solicitud.Cotizaciones[fil].ValMonAju);
				} else {
					objeto.val(Solicitud.Cotizaciones[fil].ValMonAju);
				}
			}

			// Actualizar temporizador
			selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
			$clock.countdown(selectedDate.toString());
		},
		error: function (XMLHttpRequest, textStatus, errorThrown) {
			if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
				document.location.reload(true);
			}
			else {
				$('#MCMIcono').attr('class', 'error');
				$('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de periodo garantizado.');
				$('#ModalCuadroMensaje').dialog({ title: 'Error' });
				$('#ModalCuadroMensaje').dialog('open');
			}
		}
	});
}
//<FINGTI_753>

