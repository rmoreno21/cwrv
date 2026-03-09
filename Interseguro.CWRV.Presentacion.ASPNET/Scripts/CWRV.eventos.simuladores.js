/*********************************\
|*          SIMULADORES          *|
\*********************************/

$(document).ready(function () {
	/* Flag para ejecutar automáticamente la Búsqueda de Afiliados */
	var permisoBusAfiBuscar = ($('#PerBusAfiBuscar').val() == '1' ? true : false);

	//<SOLINI25621>
	var permisoNuevoBeneficiario = ($('#PerNuevoBeneficiario').val() == '1' ? true : false);

	var permisoImprimir = ($('#PerbtnImprimir').val() == '1' ? true : false);
	var permisoEnviarCorreo = ($('#PerbtnEnviarEmail').val() == '1' ? true : false);
	//<SOLFIN25621>

	var idSolicitud;
	var Solicitud;
	var correlativo;
	var idCotizacion;
	var Correo;

	//<SOLINI25621>
	var CapitalRequerido;
	//<SOLFIN25621>

	var Afiliado;
	var correlativoAnt1 = null;
	var correlativoAnt2 = null;
	var correlativoAnt3 = null;
	var correlativoAnt4 = null;
	var correlativoAnt5 = null;
	var correlativoAnt6 = null;
	var simulando = false;

	var registros = 0;

	//<SRIINI06326>
	var consentimientoAfiliado = false;
	//<SRIFIN06326>

	//<SRI.INI-20322>
	var ordenNew;
	var uiNew;
	var divActualNew;
	var solicitudNew;
	var uiCorrelativo1New;
	var uiCorrelativo2New;
	var uiCorrelativo3New;
	var uiFilaActual1New;
	var uiFilaActual2New;
	var uiFilaActual3New;
	var uiFilaActual4New;
	var uiFilaActual5New;
	var uiFilaActual6New;
	var uiTipoSimulador;
	//<SRI.FIN-20322>

	/* Cargar datos desde sesión */
	if ($('#BusAfiDatosCargados').val() == '1') {
		if (!botonBusAfiBuscarBloqueado && permisoBusAfiBuscar) {
			var esCorrecto = true;
			var errores = new Array();

			if ($('#FormularioBusqueda').length > 0) {
				$('#BusAfiNroSolicitud').removeClass('formTextboxError');
				$('#BusAfiCUSPP').removeClass('formTextboxError');

				var vsolicitud = ($.trim($('#BusAfiNroSolicitud').val()).length > 0) ? true : false;
				var cuspp = ($.trim($('#BusAfiCUSPP').val()).length > 0) ? true : false;

				if (!(vsolicitud | cuspp)) {
					errores.push('Debe ingresar un criterio de búsqueda.');
					$('#BusAfiNroSolicitud').addClass('formTextboxError');
					$('#BusAfiCUSPP').addClass('formTextboxError');
					esCorrecto = false;
				}

				if (vsolicitud & cuspp) {
					errores.push('Sólo debe ingresar un criterio de búsqueda.');
					$('#BusAfiNroSolicitud').addClass('formTextboxError');
					$('#BusAfiCUSPP').addClass('formTextboxError');
					esCorrecto = false;
				}

				if (cuspp && $.trim($('#BusAfiCUSPP').val()).length != 12) {
					errores.push('El <strong>CUSPP</strong> debe contener 12 caracteres.');
					$('#BusAfiCUSPP').addClass('formTextboxError');
					esCorrecto = false;
				}

				if (!esCorrecto) {
					$('#MCMIcono').attr('class', 'validacion');
					$('#MCMContenedor').html(formatearError(errores));
					$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
					$('#ModalCuadroMensaje').dialog('open');
					return false;
				}
			}

			botonBusAfiBuscarBloqueado = true;
			$('#SimBusAfiBuscar').attr('class', 'botonDeshabilitado gris gris_sharp');

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				nroSolicitud: $.trim($('#HBusAfiNroSolicitud').val()),
				cuspp: $.trim($('#HBusAfiCUSPP').val())
			}
			$.ajax({
				type: 'POST',
				url: rutaObtenerDatosAfiliado,
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					Afiliado = data.d;
					if (Afiliado.Respuesta.Estado == 'OK') {
						$('#CUSPP').val(Afiliado.CUSPP);

						$('#DSCuspp').html(Afiliado.CUSPP);
						$('#DSAfiliado').html(Afiliado.Nombre + ' ' + Afiliado.ApellidoPaterno + ' ' + Afiliado.ApellidoMaterno);
						$('#DSAgente').html($('#CabNombre').html());

						($('#PerGuardar').val() == '1' ? true : false);

						//<SRIINI06326>
						consentimientoAfiliado = Afiliado.Consentimiento;
						//<SRIFIN06326>
						 
						CargarTablaSolicitudesSimulador();

						/*<SRIINI17003>*/
						selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
						$clock.countdown(selectedDate.toString());
						/*<SRIFIN17003>*/
					}
					else if (Afiliado.Respuesta.Estado == 'ERROR') {
						$('#MCMIcono').attr('class', Afiliado.Respuesta.Icono);
						$('#MCMContenedor').html(Afiliado.Respuesta.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: Afiliado.Respuesta.Titulo });
						$('#ModalCuadroMensaje').dialog('open');
					}
					else if (Afiliado.Respuesta.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						/* Sesión caducada */
						document.location.reload(true);
					}

					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información del afiliado.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				},
				complete: function () {
					botonBusAfiBuscarBloqueado = false;
					$('#SimBusAfiBuscar').attr('class', 'boton darkblue sharp');
				}
			});
		}
		else {
			return false;
		}
	}

	/* Botón Buscar Afiliado */
	$('#SimBusAfiBuscar').live('click', function () {
		if (!botonBusAfiBuscarBloqueado && permisoBusAfiBuscar) {
			$('#GrupoCotizaciones').hide();
			$('#DatosSimulacion').hide();

			var esCorrecto = true;
			var errores = new Array();

			$('#BusAfiNroSolicitud').removeClass('formTextboxError');
			$('#BusAfiCUSPP').removeClass('formTextboxError');

			var vsolicitud = ($.trim($('#BusAfiNroSolicitud').val()).length > 0) ? true : false;
			var cuspp = ($.trim($('#BusAfiCUSPP').val()).length > 0) ? true : false;

			if (!(vsolicitud | cuspp)) {
				errores.push('Debe ingresar un criterio de búsqueda.');
				$('#BusAfiNroSolicitud').addClass('formTextboxError');
				$('#BusAfiCUSPP').addClass('formTextboxError');
				esCorrecto = false;
			}

			if (vsolicitud & cuspp) {
				errores.push('Sólo debe ingresar un criterio de búsqueda.');
				$('#BusAfiNroSolicitud').addClass('formTextboxError');
				$('#BusAfiCUSPP').addClass('formTextboxError');
				esCorrecto = false;
			}

			if (cuspp && $.trim($('#BusAfiCUSPP').val()).length != 12) {
				errores.push('El <strong>CUSPP</strong> debe contener 12 caracteres.');
				$('#BusAfiCUSPP').addClass('formTextboxError');
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
			$('#SimBusAfiBuscar').attr('class', 'botonDeshabilitado gris gris_sharp');

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
				cuspp: $.trim($('#BusAfiCUSPP').val())
			}
			$.ajax({
				type: 'POST',
				url: rutaObtenerDatosAfiliado,
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					Afiliado = data.d;
					if (Afiliado.Respuesta.Estado == 'OK') {
						$('#CUSPP').val(Afiliado.CUSPP);
						$('#DSCuspp').html(Afiliado.CUSPP);
						$('#DSAfiliado').html(Afiliado.Nombre + ' ' + Afiliado.ApellidoPaterno + ' ' + Afiliado.ApellidoMaterno);
						$('#DSAgente').html($('#CabNombre').html());
						//<SRIINI06326>
						consentimientoAfiliado = Afiliado.Consentimiento;

						//<INIGTI_2145>
						//document.location.reload(true);
						//<FINGTI_2145>

						//<SRIFIN06326>
						CargarTablaSolicitudesSimulador();

						/*<SRIINI17003>*/
						selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
						$clock.countdown(selectedDate.toString());
						/*<SRIFIN17003>*/

						//<INIGTI_2145>
						

						
						$('#btnRecalcularJubiliarseHoyFuturo').css("display", "none");
						$('#btnNuevoCorreoJubiliarseHoyFuturo').css("display", "none");
						$('#btnPensionProyectadaJubiliarseHoyFuturo').css("display", "none");

						$('#btnRecalcularRentaVitalicia').css("display", "none");
						$('#btnNuevoCorreoRentaVitalicia').css("display", "none");
						$('#btnPensionProyectadaRentaVitalicia').css("display", "none");

						$('#btnRecalcularInmediataDiferida').css("display", "none");
						$('#btnNuevoCorreoInmediataDiferida').css("display", "none");
						$('#btnPensionProyectadaInmediataDiferida').css("display", "none");

						$('#btnRecalcularTipoMoneda').css("display", "none");
						$('#btnNuevoCorreoTipoMoneda').css("display", "none");
						$('#btnPensionProyectadaTipoMoneda').css("display", "none");

						$('#btnRecalcularQueMeConviene').css("display", "none");
						$('#btnNuevoCorreoQueMeConviene').css("display", "none");
						$('#btnPensionProyectadaQueMeConviene').css("display", "none");


						$('#btnRecalcularComparativo').css("display", "none");
						$('#btnNuevoCorreoComparativo').css("display", "none");
						$('#btnPensionProyectadaComparativo').css("display", "none");

						$('#DepositoPlazo').val("");
						$('#InteresDeposito').val("");
						

						//<FINGTI_2145>
					}
					else if (Afiliado.Respuesta.Estado == 'ERROR') {
						$('#MCMIcono').attr('class', Afiliado.Respuesta.Icono);
						$('#MCMContenedor').html(Afiliado.Respuesta.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: Afiliado.Respuesta.Titulo });
						$('#ModalCuadroMensaje').dialog('open');
					}
					else if (Afiliado.Respuesta.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						/* Sesión caducada */
						document.location.reload(true);
					}

					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información del afiliado.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				},
				complete: function () {
					botonBusAfiBuscarBloqueado = false;
					$('#SimBusAfiBuscar').attr('class', 'boton darkblue sharp');
				}
			});
		}
		else {
			return false;
		}
	});

	/* Seleccionar registro de solicitud */
	$('#TabSolicitudes input[type=radio]').live('change', function () {
		//alert('Inicio');
		var radio = $(this);
		var filaPadre = radio.parent().parent();
		var fecCotizacion = filaPadre.children().eq(2).html();
		var clasePadre = filaPadre.attr('class');

		//<SRI.INI-20322>
		$('#HFecCotizacion').val(fecCotizacion);

		var idSolicitud;
		var correlativoSolicitud;
		//var rutaImagenURL;

		uiCorrelativo1New = null;
		uiCorrelativo2New = null;
		uiCorrelativo3New = null;
		//<SRI.FIN-20322>

		correlativoAnt1 = null;
		correlativoAnt2 = null;
		correlativoAnt3 = null;
		//<SRI.INI-20322>
		correlativoAnt4 = null;
		correlativoAnt5 = null;
		correlativoAnt6 = null;
		//<SRI.INI-20322>

		if (!movil)
			$('#Simulacion,#Simulacion1,#Simulacion2').removeClass('areaSimulacionGrafico');
		else
			$('#Simulacion,#Simulacion1,#Simulacion2').addClass('areaSimulacionMovil');

		setTimeout(function () {
			$("#TabSolicitudes tbody tr").removeClass("grilla_active");
			$("#TabSolicitudes tbody tr:even").addClass("grilla_alt1");
			$("#TabSolicitudes tbody tr:odd").addClass("grilla_alt2");

			filaPadre.toggleClass("grilla_active", radio.is(":checked"));
		}, 0);

		// Deslizar la pantalla hacia los datos de simulación
		$('html,body').animate({
			scrollTop: $('#tituloSimulador').offset().top
		}, 'slow');
		
		// Habilitar el formulario
		$('#AnhosJubilarse,#Inflacion,#PromedioRentabilitad,#TipoCambio,#AjusteTipoCambio,#TasaAjuste,#Moneda,#PeriodoGarantizado,#Modalidad').removeAttr('disabled');
		$('#ConAnhosJubilarse,#ConMoneda,#ConPeriodoGarantizado,#ConModalidad').removeClass('formComboboxReadOnlyContenedor');
		$('#Inflacion,#PromedioRentabilitad,#TipoCambio,#AjusteTipoCambio,#TasaAjuste').removeClass('formTextboxReadOnly');
		$('#GrupoCotizaciones').show();
		
		// Cargar información de la solicitud seleccionada
		var params = {
			idSolicitud: $(this).val(),
			fecCotizacion: fecCotizacion
		}
		$.ajax({
			type: 'POST',
			url: rutaObtenerDatosSolicitud,
			contentType: "application/json; charset=iso-8859-1",
			data: $.toJSON(params),
			dataType: 'json',
			success: function (data) {
				Solicitud = data.d;
				//<SRI.INI-20322>
				solicitudNew = Solicitud;
				//<SRI.FIN-20322>
				$('#DSSolicitud').html(Solicitud.Id);
				
				//<INIGTI_2145>
				if ($('#IdSimulador').val() == 90) {
					$("#DepositoPlazo").autoNumeric("update", { vMin: "0.00", aDec: ".", aSep: ",", vMax:  Solicitud.SaldoCIC });
					$("#DepositoPlazo").val(Solicitud.SaldoCIC * 0.955);
					$('#DepositoPlazo').focus();
					CalculoPlazoFijo();
					$("#InteresDeposito").focus();
					$('#TEA').focus();

					
				}
				cantidadCotizacion = 0;
				//<FINGTI_2145>

				//<SRI.INI-20322>
				idSolicitud = Solicitud.Id;
				$('#HIdSolicitud').val(idSolicitud);
				//<SRI.FIN-20322>
				
				//<SRIINI06326>
				if (consentimientoAfiliado) {
					$('#DatosSimulacion').show();
				}
				//<SRIFIN06326>    
				
				// Convertir las Fechas a Date de Javascript para que sean correctamente parseados por el servidor
				Solicitud.FechaDevengue = new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
				//Solicitud.FechaSolicitud = new Date(+Solicitud.FechaSolicitud.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
				//Solicitud.FechaRecepcion = new Date(+Solicitud.FechaRecepcion.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
				//Solicitud.FechaPlazoAFP = new Date(+Solicitud.FechaPlazoAFP.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
				Solicitud.FechaCotizacion = new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1"));

				//Solicitud.Afiliado.FechaNacimiento = null;
				//Solicitud.Afiliado = null;
				//Solicitud.Cotizaciones = null;

				for (var i = 0; i < Solicitud.Beneficiarios.length; i++) {
					if (Solicitud.Beneficiarios[i].FechaNacimiento != null)
						Solicitud.Beneficiarios[i].FechaNacimiento = new Date(+Solicitud.Beneficiarios[i].FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1"));
					if (Solicitud.Beneficiarios[i].FechaInvalidez != null)
						Solicitud.Beneficiarios[i].FechaInvalidez = new Date(+Solicitud.Beneficiarios[i].FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1"))
				}
				//                $('#ModSolNroSolicitud').val(Solicitud.Id);
				//                $('#ModSolTipoCambio').val(Solicitud.TipoCambio);
				//                $('#ModSolTipoPension').val(Solicitud.TipoPension.Id);
				//                $('#TexModSolTipoPension').html($('#ModSolTipoPension').find(':selected').text());
				//                $('#ModSolFechaDevengue').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
				//                $('#ModSolFecUltActualizacion').val(new Date(+Solicitud.FechaSolicitud.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
				//                $('#ModSolFechaRecepcion').val(new Date(+Solicitud.FechaRecepcion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
				//                $('#ModSolFechaPlazoAFP').val(new Date(+Solicitud.FechaPlazoAFP.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
				//                $('#ModSolSaldoCIC').val(Solicitud.SaldoCIC);
				//                $('#ModSolFactorTasa').val(Solicitud.FactorTasa);
				//                $('#TexModSolFactorTasa').html($('#ModSolFactorTasa').find(':selected').text());
				
				// Cargar Tabla de Cotizaciones con las cotizaciones obtenidas desde la solicitud
				if ($('#IdSimulador').val() == 25) {
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 1, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 2, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 3, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
					//<SRI.INI-20322>
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 4, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 5, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 6, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
					//<SRI.FIN-20322>
				}
				else if ($('#IdSimulador').val() == 26) {
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 1, movil, $('#Modalidad').val(), '001', $('#PeriodoGarantizado').val());
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 2, movil, $('#Modalidad').val(), '013', $('#PeriodoGarantizado').val());
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 3, movil, $('#Modalidad').val(), '014', $('#PeriodoGarantizado').val());
				}
					//<SRI.INI-20322>
				else if ($('#IdSimulador').val() == 39) {
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 1, movil, null, "001", null);
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 2, movil, null, "013", null);
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 3, movil, null, "014", null);
				}
				//<INIGTI_2145>
				else if ($('#IdSimulador').val() == 90) {
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 1, movil, null, "001", null);
					//CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 1, movil, null, "013", null);
					//CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 1, movil, null, "014", null);
				}
				//<FINGTI_2145>
					//<SRI.FIN-20322>
				else {
					CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 0, movil, null, null, null);
				}
				
				$('#Simulacion').droppable({
					accept: '.grilla_graficar',
					activeClass: "areaSimulacionActive",
					hoverClass: "areaSimulacionHover",
					drop: function (event, ui) {

						if (!simulando) {

							var divActual = $(this);
							var filaActual = ui.draggable.parent().parent();

							if ($('#IdSimulador').val() == 24) {
								// Renta Vitalicia o Retiro Programado
								//alert('Generar Gráfico: Renta Vitalicia o Retiro Programado');

								//Inicio graficar
								divActualNew = $(this);
								uiNew = ui;
								solicitudNew = Solicitud;
								RecalcularSimulacionRentaVitalicia(simulando, solicitudNew, divActualNew, uiNew);
								//Fin graficar
							}
							else if ($('#IdSimulador').val() == 25) {
								// Inmediata o Diferida
								//alert('Generar Gráfico: Inmediata o Diferida');

								// Definir cuál de las 3 modalidades ha sido elegida
								var correlativo1 = ui.draggable.data('cotizacion1');
								var correlativo2 = ui.draggable.data('cotizacion2');
								var correlativo3 = ui.draggable.data('cotizacion3');
								//<SRI.INI-20322>
								var correlativo4 = ui.draggable.data('cotizacion4');
								var correlativo5 = ui.draggable.data('cotizacion5');
								var correlativo6 = ui.draggable.data('cotizacion6');
								//<SRI.INI-20322>

								var corr;

								if (correlativo1 != null && correlativoAnt1 != correlativo1) {
									correlativoAnt1 = correlativo1;
									filaActual.parent().find('tr').removeClass('grilla_active_blue');
									//<SRI.INI-20322>
									uiFilaActual1New = filaActual;
									//corr = 1;
									//<SRI.FIN-20322>
								}
								else if (correlativo2 != null && correlativoAnt2 != correlativo2) {
									correlativoAnt2 = correlativo2;
									filaActual.parent().find('tr').removeClass('grilla_active_red');
									//<SRI.INI-20322>
									uiFilaActual2New = filaActual;
									//corr = 2;
									//<SRI.FIN-20322>
								}
								else if (correlativo3 != null && correlativoAnt3 != correlativo3) {
									correlativoAnt3 = correlativo3;
									filaActual.parent().find('tr').removeClass('grilla_active_green');
									//<SRI.INI-20322>
									uiFilaActual3New = filaActual;
									//corr = 3;
									//<SRI.FIN-20322>
								}
									//<SRI.INI-20322>
								else if (correlativo4 != null && correlativoAnt4 != correlativo4) {
									correlativoAnt4 = correlativo4;
									filaActual.parent().find('tr').removeClass('grilla_active_green');
									//<SRI.INI-20322>
									uiFilaActual4New = filaActual;
									//corr = 4;
									//<SRI.FIN-20322>
								}
								else if (correlativo5 != null && correlativoAnt5 != correlativo5) {
									correlativoAnt5 = correlativo5;
									filaActual.parent().find('tr').removeClass('grilla_active_green');
									//<SRI.INI-20322>
									uiFilaActual5New = filaActual;
									//corr = 5;
									//<SRI.FIN-20322>
								}
								else if (correlativo6 != null && correlativoAnt6 != correlativo6) {
									correlativoAnt6 = correlativo6;
									filaActual.parent().find('tr').removeClass('grilla_active_green');
									//<SRI.INI-20322>
									uiFilaActual6New = filaActual;
									//corr = 6;
									//<SRI.FIN-20322>
								}
								//<SRI.FIN-20322>

								//<SRI.INI-20322>
								//Inicio graficar
								divActualNew = $(this);
								uiNew = ui;
								solicitudNew = Solicitud;
								uiCorrelativo1New = correlativoAnt1;
								uiCorrelativo2New = correlativoAnt2;
								uiCorrelativo3New = correlativoAnt3;
								uiCorrelativo4New = correlativoAnt4;
								uiCorrelativo5New = correlativoAnt5;
								uiCorrelativo6New = correlativoAnt6;

								RecalcularSimulacionInmediataDiferida(simulando, solicitudNew, divActualNew, uiCorrelativo1New, uiCorrelativo2New, uiCorrelativo3New,
																		 uiCorrelativo4New, uiCorrelativo5New, uiCorrelativo6New,
																		 uiFilaActual1New, uiFilaActual2New, uiFilaActual3New,
																		 uiFilaActual4New, uiFilaActual5New, uiFilaActual6New);
								//simulando = false;
								//Fin graficar
							}
							else if ($('#IdSimulador').val() == 26) {
								// Tipo de Moneda
								//alert('Generar Gráfico: Tipo de Moneda');

								//
								// Definir cuál de las 3 monedas ha sido elegida
								var correlativo1 = ui.draggable.data('cotizacion1');
								var correlativo2 = ui.draggable.data('cotizacion2');
								var correlativo3 = ui.draggable.data('cotizacion3');
								//<SRI.INI-20322>
								//var corr;
								//<SRI.FIN-20322>

								if (correlativo1 != null && correlativoAnt1 != correlativo1) {
									correlativoAnt1 = correlativo1;
									filaActual.parent().find('tr').removeClass('grilla_active_blue');
									//<SRI.INI-20322>
									uiFilaActual1New = filaActual//ui.draggable.parent().parent();
									//corr = 1;
									//<SRI.FIN-20322>
								}
								else if (correlativo2 != null && correlativoAnt2 != correlativo2) {
									correlativoAnt2 = correlativo2;
									filaActual.parent().find('tr').removeClass('grilla_active_red');
									//<SRI.INI-20322>
									uiFilaActual2New = filaActual//ui.draggable.parent().parent();
									//corr = 2;
									//<SRI.FIN-20322>
								}
								else if (correlativo3 != null && correlativoAnt3 != correlativo3) {
									correlativoAnt3 = correlativo3;
									filaActual.parent().find('tr').removeClass('grilla_active_green');
									//<SRI.INI-20322>
									uiFilaActual3New = filaActual//ui.draggable.parent().parent();
									//corr = 3;
									//<SRI.FIN-20322>
								}

								//<SRI.INI-20322>
								//Inicio graficar
								divActualNew = $(this);
								uiNew = ui;
								solicitudNew = Solicitud;
								uiCorrelativo1New = correlativoAnt1;
								uiCorrelativo2New = correlativoAnt2;
								uiCorrelativo3New = correlativoAnt3;
								RecalcularSimulacionTipoMoneda(simulando, solicitudNew, divActualNew, uiCorrelativo1New, uiCorrelativo2New, uiCorrelativo3New,
																  uiFilaActual1New, uiFilaActual2New, uiFilaActual3New);
								//simulando = false;
								//Fin graficar
								//<SRI.FIN-20322>

							}
							//<SRI.INI-20322>	
							else if ($('#IdSimulador').val() == 39) {
								// ¿Qué me conviene?
								//alert('Generar Gráfico: Renta Vitalicia o Retiro Programado');

								//<SRI.INI-20322>
								var correlativo1 = ui.draggable.data('cotizacion1');
								var correlativo2 = ui.draggable.data('cotizacion2');
								var correlativo3 = ui.draggable.data('cotizacion3');

								if (correlativo1 != null && correlativoAnt1 != correlativo1) {
									correlativoAnt1 = correlativo1;
									filaActual.parent().find('tr').removeClass('grilla_active_blue');
									uiFilaActual1New = filaActual;
								}
								else if (correlativo2 != null && correlativoAnt2 != correlativo2) {
									correlativoAnt2 = correlativo2;
									filaActual.parent().find('tr').removeClass('grilla_active_red');
									uiFilaActual2New = filaActual;
								}
								else if (correlativo3 != null && correlativoAnt3 != correlativo3) {
									correlativoAnt3 = correlativo3;
									filaActual.parent().find('tr').removeClass('grilla_active_green');
									uiFilaActual3New = filaActual;
								}


								//Inicio graficar
								divActualNew = $(this);
								uiNew = ui;
								solicitudNew = Solicitud;
								uiCorrelativo1New = correlativoAnt1;
								uiCorrelativo2New = correlativoAnt2;
								uiCorrelativo3New = correlativoAnt3;
								
								RecalcularSimulacionQueMeConviene(simulando, solicitudNew, divActualNew, uiNew,
																  uiCorrelativo1New, uiCorrelativo2New, uiCorrelativo3New,
																  uiFilaActual1New, uiFilaActual2New, uiFilaActual3New);
								//simulando = false;
							}
							//<SRI.INI-20322>
						}
						else {
							$('#MCMIcono').attr('class', 'info');
							$('#MCMContenedor').html('Simulación en proceso, por favor espere a que termine antes de volver a simular.');
							$('#ModalCuadroMensaje').dialog({ title: 'Información' });
							$('#ModalCuadroMensaje').dialog('open');
						}
					}
				});

				//Jubilarse Hoy o Futuro
				$('#Simulacion1,#Simulacion2').droppable({
					accept: '.grilla_graficar',
					activeClass: "areaSimulacionActive",
					hoverClass: "areaSimulacionHover",
					drop: function (event, ui) {
						
						//<SRI.INI-20322>
						if (!simulando) {
							//Inicio graficar
							divActualNew = $(this);
							uiNew = ui;
							solicitudNew = Solicitud;
							RecalcularSimulacionJubilarseHoyFuturo(solicitudNew, divActualNew, uiNew);
							//simulando = false;
							//Fin graficar
						}
						else {
							$('#MCMIcono').attr('class', 'info');
							$('#MCMContenedor').html('Simulación en proceso, por favor espere a que termine antes de volver a simular.');
							$('#ModalCuadroMensaje').dialog({ title: 'Información' });
							$('#ModalCuadroMensaje').dialog('open');
						}
						//<SRI.FIN-20322>
					}
				});
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}

				$('#MCMIcono').attr('class', 'error');
				$('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud seleccionada.');
				$('#ModalCuadroMensaje').dialog({ title: 'Error' });
				$('#ModalCuadroMensaje').dialog('open');
			}
		});
	});
	

	//<INIGTI_2145>

	function CalcularSimulacionPlazoFijo() {
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		var cotizacion = new Array();


		var bCotizacion = true;
		var bDepositoPlazo = true;
		var bTea = true;
		var bTem = true;
		var bAños = true;
		var bInteresDeposito = true;
		var bTasaAjusteIDX = true;
		var bTasaAjusteAJS = true;
		var bTipoCambio = true;
		var bCrecimiento = true;

		//Limpiando .removeClass("formTextboxError");
		$('#DepositoPlazo').removeClass('formTextboxError');
		$('#TEA').removeClass('formTextboxError');
		$('#TEM').removeClass('formTextboxError');
		$('#Anhos').removeClass('formTextboxError');
		$('#InteresDeposito').removeClass('formTextboxError');
		$('#TasaAjusteIDX').removeClass('formTextboxError');
		$('#TasaAjusteAJS').removeClass('formTextboxError');
		$('#TipoCambioParametro').removeClass('formTextboxError');
		$('#Crecimiento').removeClass('formTextboxError');


		if ($.trim($("#DepositoPlazo").val()).length == 0) {
			errores.push("El campo <strong>Depósito a Plazo</strong> debe tener un valor.");
			bDepositoPlazo = false;
		}

		if ($.trim($("#TEA").val()).length == 0) {
			errores.push("El campo <strong>TEA</strong> debe tener un valor.");
			bTea = false;
		}

		if ($.trim($("#TEM").val()).length == 0) {
			errores.push("El campo <strong>TEM</strong> debe tener un valor.");
			bTem = false;
		}

		if ($.trim($("#Anhos").val()).length == 0) {
			errores.push("El campo <strong>Años</strong> debe tener un valor.");
			bAños = false;
		}

		if ($.trim($("#InteresDeposito").val()).length == 0) {
			errores.push("El campo <strong>Interés Depósito</strong> debe tener un valor.");
			bInteresDeposito = false;
		}

		if ($.trim($("#TasaAjusteIDX").val()).length == 0) {
			errores.push("El campo <strong>Tasa Ajuste IDX</strong> debe tener un valor.");
			bTasaAjusteIDX = false;
		}

		if ($.trim($("#TasaAjusteAJS").val()).length == 0) {
			errores.push("El campo <strong>Tasa Ajuste AJS</strong> debe tener un valor.");
			bTasaAjusteAJS = false;
		}

		if ($.trim($("#TipoCambioParametro").val()).length == 0) {
			errores.push("El campo <strong>Tipo de Cambio</strong> debe tener un valor.");
			bTipoCambio = false;
		}

		if ($.trim($("#Crecimiento").val()).length == 0) {
			errores.push("El campo <strong>Crecimiento TC dólar por año</strong> debe tener un valor.");
			bCrecimiento = false;
		}


		for (i = 0; i < $("#TabCotizacionesSimulador tbody tr").length; i++) {
			if ($("#TabCotizacionesSimulador tbody tr:eq(" + i + ") input").is(":checked")) {
				var cot = $("#TabCotizacionesSimulador tbody tr:eq(" + i + ") input").data("cotizacion");
				cotizacion.push(cot);
			}
		}


		if ($('#TabCotizacionesSimulador input[type=checkbox]:checked').length == 0) {
			errores.push("Seleccione al menos una <strong>Cotización</strong>.");
			bCotizacion = false;
		}


		//if (!bCotizacion) $('#ModBusAfiApellidoPaterno_RP').addClass('formTextboxError');
		if (!bDepositoPlazo) $('#DepositoPlazo').addClass('formTextboxError');
		if (!bTea) $('#TEA').addClass('formTextboxError');
		if (!bTem) $('#TEM').addClass('formTextboxError');
		if (!bAños) $('#Anhos').addClass('formTextboxError');
		if (!bInteresDeposito) $('#InteresDeposito').addClass('formTextboxError');
		if (!bTasaAjusteIDX) $('#TasaAjusteIDX').addClass('formTextboxError');
		if (!bTasaAjusteAJS) $('#TasaAjusteAJS').addClass('formTextboxError');
		if (!bTipoCambio) $('#TipoCambioParametro').addClass('formTextboxError');
		if (!bCrecimiento) $('#Crecimiento').addClass('formTextboxError');

		esCorrecto = bCotizacion & bDepositoPlazo & bTea & bTem & bAños & bInteresDeposito & bTasaAjusteIDX
			& bTasaAjusteAJS & bTipoCambio & bCrecimiento;

		if (!esCorrecto) {
			$("#ModalGenerandoReporte").dialog("close");
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');

			$("#btnComparar").attr('class', 'boton darkblue sharp');
			$("#btnRecalcularComparativo").attr('class', 'boton darkblue sharp');

			//$("#btnComparar").prop("disabled", true);
			//$("#btnRecalcularComparativo").prop("disabled", true);
			return;
		} else {
			//Deslizar la pantalla hacia gráfico de simulación
			$("#ModalGenerandoReporte").dialog({ position: ["center", "center"] });
			$("#ModalGenerandoReporte").dialog("open");
			DeslizarPantallaCorreo();
		}

		var params = {
			tokenUsuario: $('#TokenUsuario').val(),
			cotizaciones: Solicitud.Cotizaciones,
			cotizacion: cotizacion,
			depositoPlazo: $("#DepositoPlazo").val(),
			tea : $("#TEA").val(),
			//tem: $("#TEM").val(),
			//interesDeposito: $("#InteresDeposito").val(),
			ajusteIDX: $("#TasaAjusteIDX").val(),
			ajusteAJS: $("#TasaAjusteAJS").val(),
			tipoCambio: $("#TipoCambioParametro").val(),
			crecimiento: $("#Crecimiento").val(),
			anhos: $("#Anhos").val(),
			CUSPP: $("#BusAfiCUSPP").val()
		}

		$.ajax({
			type: 'POST',
			url: 'CompararPlazoFijo.aspx/SimularPlazoFijo',
			contentType: "application/json; charset=utf-8",
			data: $.toJSON(params),
			dataType: 'json',
			success: function (data) {

				if (data.d.Estado == 'OK') {

					var año = 0;
					año = parseInt($("#Anhos").val());

					$('#Simulacion').html($(data.d.Contenido).find('#ContenidoDinamico').html());

					$('#DSFechaHora').html(new Date(+data.d.FechaHora.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy hh:mm:ss tt"));

					var datos = new google.visualization.DataTable();
					datos.addColumn('number', 'x');

					if (data.d.Grafico1 != null) datos.addColumn('number', document.getElementById("MOD2100").innerHTML);
					if (data.d.Grafico2 != null) datos.addColumn('number', document.getElementById("MOD2200").innerHTML);
					if (data.d.Grafico3 != null) datos.addColumn('number', document.getElementById("MOD2300").innerHTML);
					if (data.d.Grafico4 != null) datos.addColumn('number', document.getElementById("MOD2400").innerHTML);


					for (var i = 0; i < año; i++) {
						var valor1 = null;
						var valor2 = null;
						var valor3 = null;
						var valor4 = null;
						var valor5 = null;
						var valor6 = null;
						if (data.d.Grafico1 != null) valor1 = data.d.Grafico1[i].Valor;
						if (data.d.Grafico2 != null) valor2 = data.d.Grafico2[i].Valor;
						if (data.d.Grafico3 != null) valor3 = data.d.Grafico3[i].Valor;
						if (data.d.Grafico4 != null) valor4 = data.d.Grafico4[i].Valor;

						if (data.d.Grafico3 == null) {
							datos.addRow([i + 1, valor1, valor2]);
						} else if (data.d.Grafico4 == null) {
							datos.addRow([i + 1, valor1, valor2, valor3]);
						} else {
							datos.addRow([i + 1, valor1, valor2, valor3, valor4]);
						};
					}

					var colOcultar =  (20 - año) * 75;
				   
					for (var ii = 21; ii > año +1 ; ii--){
						$('#tblCuadro1 td:nth-child(' + ii + '), #tblCuadro1 th:nth-child(' + ii + ')').hide();
						$('#tblCuadro2 td:nth-child(' + ii + '), #tblCuadro2 th:nth-child(' + ii + ')').hide();
					}

					if (colOcultar > 0) {
						if (año <= 8) {
							$("#divCuadro1").removeClass("inner");
							$("#divCuadro2").removeClass("inner");
						}

						if (año == 7) {
							$("#divCuadro1").css({ 'margin-left': "110px" });
							$("#divCuadro2").css({ 'margin-left': "110px" });
						}
						else if (año == 6) {
								$("#divCuadro1").css({ 'margin-left': "30px" });
								$("#divCuadro2").css({ 'margin-left': "30px" });
						}
						else if (año == 5) {
							$("#divCuadro1").css({ 'margin-left': "-60px" });
							$("#divCuadro2").css({ 'margin-left': "-60px" });
						}
						else if (año == 4) {
							$("#divCuadro1").css({ 'margin-left': "-150px" });
							$("#divCuadro2").css({ 'margin-left': "-150px" });
						}
						else if (año == 3) {
							$("#divCuadro1").css({ 'margin-left': "-240px" });
							$("#divCuadro2").css({ 'margin-left': "-240px" });
						}
						else if (año == 2) {
							$("#divCuadro1").css({ 'margin-left': "-330px" });
							$("#divCuadro2").css({ 'margin-left': "-330px" });
						}
						else if (año == 1) {
							$("#divCuadro1").css({ 'margin-left': "-420px" });
							$("#divCuadro2").css({ 'margin-left': "-420px" });
						}
						else {
								$("#divCuadro1").css({ 'margin-left': "180px" });
								$("#divCuadro2").css({ 'margin-left': "180px" });
						}

						colOcultar = parseInt(1660 - (colOcultar * 1.1));
						$("#tblCuadro1").attr('width', colOcultar);
						$("#tblCuadro2").attr('width', colOcultar);
					}               

					if (data.d.Grafico1 == null) $("#PrimeraFila").hide();
					if (data.d.Grafico2 == null) $("#SegundaFila").hide();
					if (data.d.Grafico3 == null) $("#TerceraFila").hide();
					if (data.d.Grafico4 == null) $("#CuartaFila").hide();

					var formatter = new google.visualization.NumberFormat({
						decimalSymbol: '.',
						fractionDigits: 2,
						groupingSymbol: ',',
						prefix: 'S/. '
					});

					if (data.d.Grafico1 != null) formatter.format(datos, 1);
					if (data.d.Grafico2 != null) formatter.format(datos, 2);
					if (data.d.Grafico3 != null) formatter.format(datos, 3);
					if (data.d.Grafico4 != null) formatter.format(datos, 4);

					var options = {
						curveType: "function",
						width: 790,
						height: 400,
						chartArea: { width: 676, height: 300, top: 30, right: 0 },
						colors: ['#4F81BD', '#C0504D', '#9BBB59', '#4F29BD'],
						legend: { position: 'bottom', textStyle: { fontSize: 8 } },
						vAxis: { title: 'Pensión (S/.)', format: '#,###', titleTextStyle: { color: 'black', fontName: '"Arial"', fontSize: '11px' } },
						hAxis: { title: 'Años', titleTextStyle: { color: 'black', fontName: '"Arial"', fontSize: '11px' } },
						pointSize: 8
					};

					var chart_div = document.getElementById('Grafico');
					var chart = new google.visualization.LineChart(chart_div);

					//OBTENER URL DE LA IMAGEN
					google.visualization.events.addListener(chart, 'ready', function () {
						//chart_div.innerHTML = '<img src="' + chart.getImageURI() + '">';
						$('#HRutaImagenSimulada').val(chart.getImageURI());
					});

					chart.draw(datos, options);

					selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
					$clock.countdown(selectedDate.toString());

					/* Mostrar botones de Recálculo, Correo y Pensión Proyectada */
					$(".formLineaBotonSimulacion").show();
				}
				else if (data.d.Estado == 'ERROR') {

					if (movil) {
						$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
						$('#Simulacion,#Simulacion1').html('');
					}
					else {
						divActual.removeClass('areaSimulacionGrafico');
						divActual.html('');
					}

					$('#MCMIcono').attr('class', data.d.Icono);
					$('#MCMContenedor').html(data.d.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
					$('#ModalCuadroMensaje').dialog('open');
				}
				else if (data.d.Estado == 'TOKEN') {
					CerrarSesionExpirada();
				}
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}

				if (movil) {
					$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
					$('#Simulacion,#Simulacion1').html('');
				}
				else {
					divActual.removeClass('areaSimulacionGrafico');
					divActual.html('');
				}

				$('#MCMIcono').attr('class', 'error');
				$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
				$('#ModalCuadroMensaje').dialog({ title: 'Error' });
				$('#ModalCuadroMensaje').dialog('open');

				$("#btnComparar").attr('class', 'boton darkblue sharp');
				$("#btnRecalcularComparativo").attr('class', 'boton darkblue sharp');
				$("#ModalGenerandoReporte").dialog("close");
				//$("#btnComparar").prop("disabled", true);
				//$("#btnRecalcularComparativo").prop("disabled", true);

			},
			complete: function () {
				$("#ModalGenerandoReporte").dialog("close");
				$('#btnRecalcularComparativo').css("display", "");
				$('#btnNuevoCorreoComparativo').css("display", "");
				$('#btnPensionProyectadaComparativo').css("display", "");
				simulando = false;

				if (movil) {
					$('#TabCotizacionesSimulador input').removeAttr('disabled');
				}

				$("#btnComparar").attr('class', 'boton darkblue sharp');
				$("#btnRecalcularComparativo").attr('class', 'boton darkblue sharp');

				//$("#btnComparar").prop("disabled", true);
				//$("#btnRecalcularComparativo").prop("disabled", true);
			}
		});

	}

	
	$("#btnComparar").live('click', function (e) {
		e.preventDefault();

		

		$("#btnComparar").attr('class', 'botonDeshabilitado gris gris_sharp');
		
		
		CalcularSimulacionPlazoFijo();
	});

	$('#btnRecalcularComparativo').live('click', function (e) {
		e.preventDefault();
		$("#ModalGenerandoReporte").dialog({ position: ["center", "center"] });
		$("#ModalGenerandoReporte").dialog("open");

		$("#btnRecalcularComparativo").attr('class', 'botonDeshabilitado gris gris_sharp');
		
		CalcularSimulacionPlazoFijo();
	});


	//<FINGTI_2145>

	//<SRI.INI-20322>
	$('#btnRecalcularQueMeConviene').live('click', function () {
		// Deslizar la pantalla hacia gráfico de simulación
		DeslizarPantallaCorreo();

		RecalcularSimulacionQueMeConviene(simulando, solicitudNew, divActualNew, uiNew,
										  uiCorrelativo1New, uiCorrelativo2New, uiCorrelativo3New,
										  uiFilaActual1New, uiFilaActual2New, uiFilaActual3New);
	});

	function RecalcularSimulacionQueMeConviene(simulandoNewAlt, solicitudNewAlt, divActualNewAlt, uiNewAlt,
											   uiCorrelativo1NewAlt, uiCorrelativo2NewAlt, uiCorrelativo3NewAlt,
											   uiFilaActual1NewAlt, uiFilaActual2NewAlt, uiFilaActual3NewAlt) {
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#PromedioRentabilitad").removeClass("formTextboxError");
		$("#Inflacion").removeClass("formTextboxError");
		$("#TasaAjuste").removeClass("formTextboxError");
		$("#TipoCambio").removeClass("formTextboxError");
		$("#AjusteTipoCambio").removeClass("formTextboxError");

		// Rentabilidad
		var rentabilidad = true;
		if ($.trim($('#PromedioRentabilitad').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Promedio rentabilidad fondo AFP</strong>. Dato Obligatorio.');
			rentabilidad = false;
		}

		// Inflación
		var inflacion = true;
		if ($.trim($('#Inflacion').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Inflación anual</strong>. Dato Obligatorio.');
			inflacion = false;
		}

		// Tasa de Ajuste
		var tasaAjuste = true;
		if ($.trim($('#TasaAjuste').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Tasa de ajuste fija anual</strong>. Dato Obligatorio.');
			tasaAjuste = false;
		}

		// Tipo de cambio
		var tipoCambio = true;
		if ($.trim($('#TipoCambio').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Tipo de cambio</strong>. Dato Obligatorio.');
			tipoCambio = false;
		}

		// Clases de controles
		if (!rentabilidad) $("#PromedioRentabilitad").addClass("formTextboxError");
		if (!inflacion) $("#Inflacion").addClass("formTextboxError");
		if (!tasaAjuste) $("#TasaAjuste").addClass("formTextboxError");
		if (!tipoCambio) $("#TipoCambio").addClass("formTextboxError");

		esCorrecto = rentabilidad & inflacion & tasaAjuste & tipoCambio;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}
		
		
		// validar Correlativo
		var booCorrelativo = true;
		if ((uiCorrelativo1NewAlt == uiCorrelativo2NewAlt && uiCorrelativo1NewAlt != null && uiCorrelativo2NewAlt != null)
			|| (uiCorrelativo1NewAlt == uiCorrelativo3NewAlt && uiCorrelativo1NewAlt != null && uiCorrelativo3NewAlt != null)
			|| (uiCorrelativo2NewAlt == uiCorrelativo3NewAlt && uiCorrelativo2NewAlt != null && uiCorrelativo3NewAlt != null)) {
			errores.push('<strong>Correlativos iguales</strong>. Seleccionar correlativos diferentes.');
			booCorrelativo = false;
		}

		esCorrecto = booCorrelativo;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}


		//var simulando = simulandoNewAlt;
		simulando = simulandoNewAlt;
		var Solicitud = solicitudNewAlt;
		var divActual = divActualNewAlt;
		var ui = uiNewAlt;

		var uiFilaActual1 = uiFilaActual1NewAlt;
		var uiFilaActual2 = uiFilaActual2NewAlt;
		var uiFilaActual3 = uiFilaActual3NewAlt;

		var correlativoAnt1 = uiCorrelativo1NewAlt;
		var correlativoAnt2 = uiCorrelativo2NewAlt;
		var correlativoAnt3 = uiCorrelativo3NewAlt;

		//var filaActual;// = ui.draggable.parent().parent();
		//var correlativo;
		
		if (!simulando) {
			
			if (movil) {
				//filaActual = ui.parent().parent();
				//orden = ui.attr('name').substr(ui.attr('name').length - 1, 1);
				//correlativo = ui.val();

				$('#Simulacion,#Simulacion1').addClass('areaSimulacionGraficoMovil');
				$('#Simulacion,#Simulacion1').html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
				$('#TabCotizacionesSimulador input').attr('disabled', 'disabled');
			}
			else {
				//filaActual = ui.draggable.parent().parent();
				//orden = divActual.attr("id").substr(divActual.attr("id").length - 1, 1);
				//correlativo = ui.draggable.data('cotizacion');
				
				// Limpiar el contenido
				$('#TabCotizacionesSimulador tr').removeClass('grilla_active_blue');
				divActual.addClass('areaSimulacionGrafico');
				divActual.html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
			}
			
			simulando = true;
			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				idSolicitud: Solicitud.Id,
				fechaCotizacion: Solicitud.FechaCotizacion,
				fechaDevengue: Solicitud.FechaDevengue,
				cotizaciones: Solicitud.Cotizaciones,
				beneficiarios: Solicitud.Beneficiarios,
				cic: Solicitud.SaldoCIC,
				tipoCambio: $('#TipoCambio').val(),
				rentabilidadAFP: $('#PromedioRentabilitad').val(),
				ipc: $('#Inflacion').val(),
				ajuste: $('#TasaAjuste').val(),
				correlativo1: correlativoAnt1,
				correlativo2: correlativoAnt2,
				correlativo3: correlativoAnt3
			}
			$.ajax({
				type: 'POST',
				url: 'QueMeConviene.aspx/SimularQueMeConviene',
				contentType: "application/json; charset=utf-8",
				data: $.toJSON(params),
				dataType: 'json',
				success: function (data) {
					
					if (data.d.Estado == 'OK') {
						
						//if (movil) {
						$('#Simulacion').html($(data.d.Contenido).find('#ContenidoDinamico').html());
						//}
						//else {
						//    divActual.html($(data.d.Contenido).find('#ContenidoDinamico').html());
						//}

						$('#DSFechaHora').html(new Date(+data.d.FechaHora.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy hh:mm:ss tt"));

						if (uiFilaActual1 != null) {
							uiFilaActual1.addClass('grilla_active_blue');
						}
						if (uiFilaActual2 != null) {
							uiFilaActual2.addClass('grilla_active_red');
						}
						if (uiFilaActual3 != null) {
							uiFilaActual3.addClass('grilla_active_green');
						}


						var datos = new google.visualization.DataTable();
						datos.addColumn('number', 'x');
						datos.addColumn('number', 'MODALIDAD 1 - PEN 1');
						datos.addColumn('number', 'MODALIDAD 2 - PEN 1');
						datos.addColumn('number', 'MODALIDAD 3 - PEN 1');
						datos.addColumn('number', 'MODALIDAD 1 - PEN 2');
						datos.addColumn('number', 'MODALIDAD 2 - PEN 2');
						datos.addColumn('number', 'MODALIDAD 3 - PEN 2');
						
						//for (var i = 0; i < data.d.Grafico1.length; i++) {
						for (var i = 0; i < 25; i++) {
							var valor1 = null;
							var valor2 = null;
							var valor3 = null;
							var valor4 = null;
							var valor5 = null;
							var valor6 = null;
							if (data.d.Grafico1 != null) valor1 = data.d.Grafico1[i].Valor;
							if (data.d.Grafico2 != null) valor2 = data.d.Grafico2[i].Valor;
							if (data.d.Grafico3 != null) valor3 = data.d.Grafico3[i].Valor;
							if (data.d.Grafico4 != null) valor4 = data.d.Grafico4[i].Valor;
							if (data.d.Grafico5 != null) valor5 = data.d.Grafico5[i].Valor;
							if (data.d.Grafico6 != null) valor6 = data.d.Grafico6[i].Valor;

							//datos.addRow([i +1, data.d.Grafico1[i].Valor, data.d.Grafico2[i].Valor]);
							datos.addRow([i + 1, valor1, valor2, valor3, valor4, valor5, valor6]);
						}
						
						var formatter = new google.visualization.NumberFormat({
							decimalSymbol: '.',
							fractionDigits: 2,
							groupingSymbol: ',',
							prefix: 'S/. '
						});
						formatter.format(datos, 1);
						formatter.format(datos, 2);
						formatter.format(datos, 3);
						formatter.format(datos, 4);
						formatter.format(datos, 5);
						formatter.format(datos, 6);
						
						var options = {
							curveType: "function",
							width: 790,
							height: 400,
							chartArea: { width: 676, height: 300, top: 30, right: 0 },
							colors: ['#4F81BD', '#C0504D', '#9BBB59', '#4F81BD', '#C0504D', '#9BBB59'],
							legend: { position: 'bottom', textStyle: { fontSize: 8 } },
							vAxis: { title: 'Pensión (S/.)', format: '#,###', titleTextStyle: { color: 'black', fontName: '"Arial"', fontSize: '11px' } },
							hAxis: { title: 'Años', titleTextStyle: { color: 'black', fontName: '"Arial"', fontSize: '11px' } },
							pointSize: 8
						};

						var chart_div = document.getElementById('Grafico');
						var chart = new google.visualization.LineChart(chart_div);

						//OBTENER URL DE LA IMAGEN
						google.visualization.events.addListener(chart, 'ready', function () {
							//chart_div.innerHTML = '<img src="' + chart.getImageURI() + '">';
							$('#HRutaImagenSimulada').val(chart.getImageURI());
						});

						chart.draw(datos, options);

						selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
						$clock.countdown(selectedDate.toString());

						/* Mostrar botones de Recálculo, Correo y Pensión Proyectada */
						$(".formLineaBotonSimulacion").show();
					}
					else if (data.d.Estado == 'ERROR') {

						if (movil) {
							$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
							$('#Simulacion,#Simulacion1').html('');
						}
						else {
							divActual.removeClass('areaSimulacionGrafico');
							divActual.html('');
						}

						$('#MCMIcono').attr('class', data.d.Icono);
						$('#MCMContenedor').html(data.d.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
						$('#ModalCuadroMensaje').dialog('open');
					}
					else if (data.d.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						/* Sesión caducada */
						document.location.reload(true);
					}

					if (movil) {
						$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
						$('#Simulacion,#Simulacion1').html('');
					}
					else {
						divActual.removeClass('areaSimulacionGrafico');
						divActual.html('');
					}

					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				},
				complete: function () {
					$('#btnRecalcularQueMeConviene').css("display", "");
					$('#btnNuevoCorreoQueMeConviene').css("display", "");
					$('#btnPensionProyectadaQueMeConviene').css("display", "");
					simulando = false;

					if (movil) {
						$('#TabCotizacionesSimulador input').removeAttr('disabled');
					}
				}
			});
		}
		else {
			$('#MCMIcono').attr('class', 'info');
			$('#MCMContenedor').html('Simulación en proceso, por favor espere a que termine antes de volver a simular.');
			$('#ModalCuadroMensaje').dialog({ title: 'Información' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	}

	//Renta Vitalicia o Retiro Programado
	$('#btnRecalcularRentaVitalicia').live('click', function () {
		// Deslizar la pantalla hacia gráfico de simulación
		DeslizarPantallaCorreo();

		RecalcularSimulacionRentaVitalicia(simulando, solicitudNew, divActualNew, uiNew);
	});
	
	function RecalcularSimulacionRentaVitalicia(simulandoNewAlt, solicitudNewAlt, divActualNewAlt, uiNewAlt) {
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#PromedioRentabilitad").removeClass("formTextboxError");
		$("#Inflacion").removeClass("formTextboxError");
		$("#TasaAjuste").removeClass("formTextboxError");

		// Rentabilidad
		var rentabilidad = true;
		if ($.trim($('#PromedioRentabilitad').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Promedio rentabilidad fondo AFP</strong>. Dato Obligatorio.');
			rentabilidad = false;
		}

		// Inflación
		var inflacion = true;
		if ($.trim($('#Inflacion').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Inflación anual</strong>. Dato Obligatorio.');
			inflacion = false;
		}

		// Tasa de Ajuste
		var tasaAjuste = true;
		if ($.trim($('#TasaAjuste').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Tasa de ajuste fija anual</strong>. Dato Obligatorio.');
			tasaAjuste = false;
		}

		// Clases de controles
		if (!rentabilidad) $("#PromedioRentabilitad").addClass("formTextboxError");
		if (!inflacion) $("#Inflacion").addClass("formTextboxError");
		if (!tasaAjuste) $("#TasaAjuste").addClass("formTextboxError");

		esCorrecto = rentabilidad & inflacion & tasaAjuste;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}
		
		//var simulando = simulandoNewAlt;
		simulando = simulandoNewAlt;
		var Solicitud = solicitudNewAlt;
		var divActual = divActualNewAlt;
		var ui = uiNewAlt;

		var filaActual;
		//var orden;
		//var correlativo;

		if (!simulando) {

			if (movil) {
				filaActual = ui.parent().parent();
				//orden = ui.attr('name').substr(ui.attr('name').length - 1, 1);
				correlativo = ui.val();

				$('#Simulacion,#Simulacion1').addClass('areaSimulacionGraficoMovil');
				$('#Simulacion,#Simulacion1').html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
				$('#TabCotizacionesSimulador input').attr('disabled', 'disabled');
			}
			else {
				filaActual = ui.draggable.parent().parent();
				//orden = divActual.attr("id").substr(divActual.attr("id").length - 1, 1);
				correlativo = ui.draggable.data('cotizacion');

				// Limpiar el contenido
				$('#TabCotizacionesSimulador tr').removeClass('grilla_active_blue');
				divActual.addClass('areaSimulacionGrafico');
				divActual.html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
			}


			simulando = true;
			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				idSolicitud: Solicitud.Id,
				fechaCotizacion: Solicitud.FechaCotizacion,
				fechaDevengue: Solicitud.FechaDevengue,
				cotizaciones: Solicitud.Cotizaciones,
				beneficiarios: Solicitud.Beneficiarios,
				cic: Solicitud.SaldoCIC,
				correlativo: correlativo,
				rentabilidadAFP: $('#PromedioRentabilitad').val(),
				ipc: $('#Inflacion').val(),
				ajuste: $('#TasaAjuste').val()
			}
			$.ajax({
				type: 'POST',
				url: 'RentaVitaliciaRetiroProgramado.aspx/SimularRentaVitaliciaRetiroProgramado',
				contentType: "application/json; charset=utf-8",
				data: $.toJSON(params),
				dataType: 'json',
				success: function (data) {
					if (data.d.Estado == 'OK') {

						//if (movil) {
						$('#Simulacion').html($(data.d.Contenido).find('#ContenidoDinamico').html());
						//}
						//else{
						//    divActual.html($(data.d.Contenido).find('#ContenidoDinamico').html());
						//}

						$('#DSFechaHora').html(new Date(+data.d.FechaHora.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy hh:mm:ss tt"));

						filaActual.addClass('grilla_active_blue');

						var datos = new google.visualization.DataTable();
						datos.addColumn('number', 'x');
						datos.addColumn('number', 'RENTA VITALICIA');
						datos.addColumn('number', 'RETIRO PROGRAMADO');

						for (var i = 0; i < data.d.Grafico1.length; i++) {
							datos.addRow([i + 1, data.d.Grafico1[i].Valor, data.d.Grafico2[i].Valor]);
						}

						var formatter = new google.visualization.NumberFormat({
							decimalSymbol: '.',
							fractionDigits: 2,
							groupingSymbol: ',',
							prefix: 'S/. '
						});
						formatter.format(datos, 1);
						formatter.format(datos, 2);

						var options = {
							curveType: "function",
							width: 790,
							height: 400,
							chartArea: { width: 676, height: 300, top: 30, right: 0 },
							legend: { position: 'bottom', textStyle: { fontSize: 12 } },
							vAxis: { title: 'Pensión (S/.)', format: '#,###', titleTextStyle: { color: 'black', fontName: '"Arial"', fontSize: '12px' } },
							hAxis: { title: 'Años', titleTextStyle: { color: 'black', fontName: '"Arial"', fontSize: '12px' } },
							pointSize: 8
						};

						var chart_div = document.getElementById('Grafico');
						var chart = new google.visualization.LineChart(chart_div);

						//OBTENER URL DE LA IMAGEN
						google.visualization.events.addListener(chart, 'ready', function () {
							//chart_div.innerHTML = '<img src="' + chart.getImageURI() + '">';
							$('#HRutaImagenSimulada').val(chart.getImageURI());
						});

						chart.draw(datos, options);

						selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
						$clock.countdown(selectedDate.toString());

						/* Mostrar botones de Recálculo, Correo y Pensión Proyectada */
						$(".formLineaBotonSimulacion").show();
					}
					else if (data.d.Estado == 'ERROR') {

						if (movil) {
							$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
							$('#Simulacion,#Simulacion1').html('');
						}
						else{
							divActual.removeClass('areaSimulacionGrafico');
							divActual.html('');
						}

						$('#MCMIcono').attr('class', data.d.Icono);
						$('#MCMContenedor').html(data.d.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
						$('#ModalCuadroMensaje').dialog('open');
					}
					else if (data.d.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						/* Sesión caducada */
						document.location.reload(true);
					}

					if (movil) {
						$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
						$('#Simulacion,#Simulacion1').html('');
					}else{
						divActual.removeClass('areaSimulacionGrafico');
						divActual.html('');
					}

					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				},
				complete: function () {
					$('#btnRecalcularRentaVitalicia').css("display", "");
					$('#btnNuevoCorreoRentaVitalicia').css("display", "");
					$('#btnPensionProyectadaRentaVitalicia').css("display", "");
					simulando = false;

					if (movil) {
						$('#TabCotizacionesSimulador input').removeAttr('disabled');
					}
				}
			});
		}
		else {
			$('#MCMIcono').attr('class', 'info');
			$('#MCMContenedor').html('Simulación en proceso, por favor espere a que termine antes de volver a simular.');
			$('#ModalCuadroMensaje').dialog({ title: 'Información' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	}

	// Inmediata o Diferida
	$('#btnRecalcularInmediataDiferida').live('click', function () {
		// Deslizar la pantalla hacia gráfico de simulación
		DeslizarPantallaCorreo();

		RecalcularSimulacionInmediataDiferida(simulando, solicitudNew, divActualNew, uiCorrelativo1New, uiCorrelativo2New, uiCorrelativo3New,
												 uiCorrelativo4New, uiCorrelativo5New, uiCorrelativo6New,
												 uiFilaActual1New, uiFilaActual2New, uiFilaActual3New,
												 uiFilaActual4New, uiFilaActual5New, uiFilaActual6New);
	});
	
	function RecalcularSimulacionInmediataDiferida(simulandoNewAlt, solicitudNewAlt, divActualNewAlt, uiCorrelativo1NewAlt, uiCorrelativo2NewAlt, uiCorrelativo3NewAlt,
													  uiCorrelativo4NewAlt, uiCorrelativo5NewAlt, uiCorrelativo6NewAlt,
													  uiFilaActual1NewAlt, uiFilaActual2NewAlt, uiFilaActual3NewAlt,
													  uiFilaActual4NewAlt, uiFilaActual5NewAlt, uiFilaActual6NewAlt) {
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#Inflacion").removeClass("formTextboxError");
		$("#TasaAjuste").removeClass("formTextboxError");

		// Inflación
		var inflacion = true;
		if ($.trim($('#Inflacion').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Inflación anual</strong>. Dato Obligatorio.');
			inflacion = false;
		}

		// Tasa de Ajuste
		var tasaAjuste = true;
		if ($.trim($('#TasaAjuste').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Tasa de ajuste fija anual</strong>. Dato Obligatorio.');
			tasaAjuste = false;
		}

		// Clases de controles
		if (!inflacion) $("#Inflacion").addClass("formTextboxError");
		if (!tasaAjuste) $("#TasaAjuste").addClass("formTextboxError");

		esCorrecto = inflacion & tasaAjuste;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}

		//var simulando = simulandoNewAlt;
		simulando = simulandoNewAlt;
		var Solicitud = solicitudNewAlt;
		var divActual = divActualNewAlt;
		var uiFilaActual1 = uiFilaActual1NewAlt;
		var uiFilaActual2 = uiFilaActual2NewAlt;
		var uiFilaActual3 = uiFilaActual3NewAlt;
		var uiFilaActual4 = uiFilaActual4NewAlt;
		var uiFilaActual5 = uiFilaActual5NewAlt;
		var uiFilaActual6 = uiFilaActual6NewAlt;

		// Definir cuál de las 3 monedas ha sido elegida
		var correlativoAnt1 = uiCorrelativo1NewAlt;
		var correlativoAnt2 = uiCorrelativo2NewAlt;
		var correlativoAnt3 = uiCorrelativo3NewAlt;
		var correlativoAnt4 = uiCorrelativo4NewAlt;
		var correlativoAnt5 = uiCorrelativo5NewAlt;
		var correlativoAnt6 = uiCorrelativo6NewAlt;
		var corr;

		if (!simulando) {

			if (movil) {
				$('#Simulacion,#Simulacion1').addClass('areaSimulacionGraficoMovil');
				$('#Simulacion,#Simulacion1').html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
				$('#TabCotizacionesSimulador input').attr('disabled', 'disabled');
			}
			else{
				divActual.addClass('areaSimulacionGrafico');
				divActual.html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
			}

			var tasa = ($('#Moneda').val() == '001') ? $('#Inflacion').val() : $('#TasaAjuste').val();
		
			simulando = true;
			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				cotizaciones: Solicitud.Cotizaciones,
				correlativo1: correlativoAnt1,
				correlativo2: correlativoAnt2,
				correlativo3: correlativoAnt3,
				correlativo4: correlativoAnt4,
				correlativo5: correlativoAnt5,
				correlativo6: correlativoAnt6,
				tasa: tasa
			}
			$.ajax({
				type: 'POST',
				url: 'InmediataDiferida.aspx/SimularInmediataDiferida',
				contentType: "application/json; charset=utf-8",
				data: $.toJSON(params),
				dataType: 'json',
				success: function (data) {
					if (data.d.Estado == 'OK') {
						
						//if (movil) {
						$('#Simulacion').html($(data.d.Contenido).find('#ContenidoDinamico').html());
						//}
						//else {
						//    divActual.html($(data.d.Contenido).find('#ContenidoDinamico').html());
						//}

						$('#DSFechaHora').html(new Date(+data.d.FechaHora.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy hh:mm:ss tt"));
				
						if (uiFilaActual1 != null) {
							uiFilaActual1.addClass('grilla_active_blue');
						}
						if (uiFilaActual2 != null) {
							uiFilaActual2.addClass('grilla_active_red');
						}
						if (uiFilaActual3 != null) {
							uiFilaActual3.addClass('grilla_active_green');
						}
						if (uiFilaActual4 != null) {
							uiFilaActual4.addClass('grilla_active_yellow');
						}
						if (uiFilaActual5 != null) {
							uiFilaActual5.addClass('grilla_active_purple');
						}
						if (uiFilaActual6 != null) {
							uiFilaActual6.addClass('grilla_active_pink');
						}

						var datos = new google.visualization.DataTable();
						datos.addColumn('number', 'x');
						datos.addColumn('number', 'RENTA VITALICIA INMEDIATA');
						datos.addColumn('number', 'RENTA TEMPORAL A 1 AÑO');
						datos.addColumn('number', 'RENTA TEMPORAL A 2 AÑOS');
						datos.addColumn('number', 'RENTA TEMPORAL A 3 AÑOS');
						datos.addColumn('number', 'RENTA TEMPORAL A 4 AÑOS');
						datos.addColumn('number', 'RENTA TEMPORAL A 5 AÑOS');

						//for (var i = 0; i < data.d.Grafico1.length; i++) {
						for (var i = 0; i < 25; i++) {
							var valor1 = null;
							var valor2 = null;
							var valor3 = null;
							var valor4 = null;
							var valor5 = null;
							var valor6 = null;
							if (data.d.Grafico1 != null) valor1 = data.d.Grafico1[i].Valor;
							if (data.d.Grafico2 != null) valor2 = data.d.Grafico2[i].Valor;
							if (data.d.Grafico3 != null) valor3 = data.d.Grafico3[i].Valor;
							if (data.d.Grafico4 != null) valor4 = data.d.Grafico4[i].Valor;
							if (data.d.Grafico5 != null) valor5 = data.d.Grafico5[i].Valor;
							if (data.d.Grafico6 != null) valor6 = data.d.Grafico6[i].Valor;

							//if (data.d.Grafico1 != null && !isNaN(parseFloat(data.d.Grafico1)) && parseFloat(data.d.Grafico1) != 0) valor1 = data.d.Grafico1[i].Valor;
							//if (data.d.Grafico2 != null && !isNaN(parseFloat(data.d.Grafico2)) && parseFloat(data.d.Grafico2) != 0) valor2 = data.d.Grafico2[i].Valor;
							//if (data.d.Grafico3 != null && !isNaN(parseFloat(data.d.Grafico3)) && parseFloat(data.d.Grafico3) != 0) valor3 = data.d.Grafico3[i].Valor;
							//if (data.d.Grafico4 != null && !isNaN(parseFloat(data.d.Grafico4)) && parseFloat(data.d.Grafico4) != 0) valor4 = data.d.Grafico4[i].Valor;
							//if (data.d.Grafico5 != null && !isNaN(parseFloat(data.d.Grafico5)) && parseFloat(data.d.Grafico5) != 0) valor5 = data.d.Grafico5[i].Valor;
							//if (data.d.Grafico6 != null && !isNaN(parseFloat(data.d.Grafico6)) && parseFloat(data.d.Grafico6) != 0) valor6 = data.d.Grafico6[i].Valor;

							//datos.addRow([i + 1,
							//             data.d.Grafico1[i].Valor,
							//             data.d.Grafico2[i].Valor,
							//             data.d.Grafico3[i].Valor,
							//             data.d.Grafico4[i].Valor,
							//             data.d.Grafico5[i].Valor,
							//             data.d.Grafico6[i].Valor]);

							datos.addRow([i + 1, valor1, valor2, valor3, valor4, valor5, valor6]);

						}
						
						var formatter = new google.visualization.NumberFormat({
							decimalSymbol: '.',
							fractionDigits: 2,
							groupingSymbol: ',',
							prefix: 'S/. '
						});
						formatter.format(datos, 1);
						formatter.format(datos, 2);
						formatter.format(datos, 3);
						formatter.format(datos, 4);
						formatter.format(datos, 5);
						formatter.format(datos, 6);

						var options = {
							curveType: "function",
							width: 850,
							height: 400,
							chartArea: { width: 600, height: 300, top: 30, right: 0 },
							colors: ['#4F81BD', '#C0504D', '#9BBB59', '#F1CA3A', '#871B47', '#D799AE'],
							legend: { position: 'right', textStyle: { fontSize: 8 } },
							vAxis: { title: 'Pensión (S/.)', format: '#,###', titleTextStyle: { color: 'black', fontName: '"Arial"', fontSize: '12px'} },
							hAxis: { title: 'Años', titleTextStyle: { color: 'black', fontName: '"Arial"', fontSize: '12px' }, minValue: 0, maxValue: 26 }, //gridlines: { count: 6 /* 100/20 = 5 steps */}
							vAxis: { viewWindow: { min: 0 }, gridlines: { count: 10 }},
							pointSize: 8
						};

						var chart_div = document.getElementById('Grafico');
						var chart = new google.visualization.LineChart(chart_div);

						//OBTENER URL DE LA IMAGEN
						google.visualization.events.addListener(chart, 'ready', function () {
							//chart_div.innerHTML = '<img src="' + chart.getImageURI() + '">';
							$('#HRutaImagenSimulada').val(chart.getImageURI());
						});

						chart.draw(datos, options);

						selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
						$clock.countdown(selectedDate.toString());

						/* Mostrar botones de Recálculo, Correo y Pensión Proyectada */
						$(".formLineaBotonSimulacion").show();
					}
					else if (data.d.Estado == 'ERROR') {

						if (movil) {
							$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
							$('#Simulacion,#Simulacion1').html('');
						}
						else{
							divActual.removeClass('areaSimulacionGrafico');
							divActual.html('');
						}

						$('#MCMIcono').attr('class', data.d.Icono);
						$('#MCMContenedor').html(data.d.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
						$('#ModalCuadroMensaje').dialog('open');
					}
					else if (data.d.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						/* Sesión caducada */
						document.location.reload(true);
					}

					if (movil) {
						$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
						$('#Simulacion,#Simulacion1').html('');
					}
					else{
						divActual.removeClass('areaSimulacionGrafico');
						divActual.html('');
					}

					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				},
				complete: function () {
					$('#btnRecalcularInmediataDiferida').css("display", "");
					$('#btnNuevoCorreoInmediataDiferida').css("display", "");
					$('#btnPensionProyectadaInmediataDiferida').css("display", "");
					simulando = false;

					if (movil) {
						$('#TabCotizacionesSimulador input').removeAttr('disabled');
					}
				}
			});
		}
		else {
			$('#MCMIcono').attr('class', 'info');
			$('#MCMContenedor').html('Simulación en proceso, por favor espere a que termine antes de volver a simular.');
			$('#ModalCuadroMensaje').dialog({ title: 'Información' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	}

	//Tipo Moneda
	$('#btnRecalcularTipoMoneda').live('click', function () {
		// Deslizar la pantalla hacia gráfico de simulación
		DeslizarPantallaCorreo();

		RecalcularSimulacionTipoMoneda(simulando, solicitudNew, divActualNew, uiCorrelativo1New, uiCorrelativo2New, uiCorrelativo3New,
										  uiFilaActual1New, uiFilaActual2New, uiFilaActual3New);
	});

	function RecalcularSimulacionTipoMoneda(simulandoNewAlt, solicitudNewAlt, divActualNewAlt, uiCorrelativo1NewAlt, uiCorrelativo2NewAlt, uiCorrelativo3NewAlt,
											   uiFilaActual1NewAlt, uiFilaActual2NewAlt, uiFilaActual3NewAlt) {
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#Inflacion").removeClass("formTextboxError");
		$("#TasaAjuste").removeClass("formTextboxError");
		$("#TipoCambio").removeClass("formTextboxError");
		$("#AjusteTipoCambio").removeClass("formTextboxError");
		
		// Inflación
		var inflacion = true;
		if ($.trim($('#Inflacion').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Inflación anual</strong>. Dato Obligatorio.');
			inflacion = false;
		}

		// Tasa de Ajuste
		var tasaAjuste = true;
		if ($.trim($('#TasaAjuste').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Tasa de ajuste fija anual</strong>. Dato Obligatorio.');
			tasaAjuste = false;
		}

		// Tipo de cambio
		var tipoCambio = true;
		if ($.trim($('#TipoCambio').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Tipo de cambio</strong>. Dato Obligatorio.');
			tipoCambio = false;
		}

		// Ajuste tipo de cambio
		var ajusteTipoCambio = true;
		if ($.trim($('#AjusteTipoCambio').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Ajuste tipo de cambio</strong>. Dato Obligatorio.');
			ajusteTipoCambio = false;
		}

		// Clases de controles
		if (!inflacion) $("#Inflacion").addClass("formTextboxError");
		if (!tasaAjuste) $("#TasaAjuste").addClass("formTextboxError");
		if (!tipoCambio) $("#TipoCambio").addClass("formTextboxError");
		if (!ajusteTipoCambio) $("#AjusteTipoCambio").addClass("formTextboxError");

		esCorrecto = inflacion & tasaAjuste & tipoCambio & ajusteTipoCambio;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}
	
		//var simulando = simulandoNewAlt;
		simulando = simulandoNewAlt;
		var Solicitud = solicitudNewAlt;
		var divActual = divActualNewAlt;
		var uiFilaActual1 = uiFilaActual1NewAlt;
		var uiFilaActual2 = uiFilaActual2NewAlt;
		var uiFilaActual3 = uiFilaActual3NewAlt;

		
		// Definir cuál de las 3 monedas ha sido elegida
		var correlativoAnt1 = uiCorrelativo1NewAlt;
		var correlativoAnt2 = uiCorrelativo2NewAlt;
		var correlativoAnt3 = uiCorrelativo3NewAlt;

		if (!simulando) {
			
			if (movil) {
				$('#Simulacion,#Simulacion1').addClass('areaSimulacionGraficoMovil');
				$('#Simulacion,#Simulacion1').html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
				$('#TabCotizacionesSimulador input').attr('disabled', 'disabled');
			}
			else {
				divActual.addClass('areaSimulacionGrafico');
				divActual.html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
			}


			simulando = true;
			var params = {
				tokenUsuario : $('#TokenUsuario').val(),
				cotizaciones: Solicitud.Cotizaciones,
				correlativo1: correlativoAnt1,
				correlativo2 : correlativoAnt2,
				correlativo3: correlativoAnt3,
				ipc: $('#Inflacion').val(),
				tasaAjuste: $('#TasaAjuste').val(),
				tipoCambio: $('#TipoCambio').val(),
				tasaTipoCambio: $('#AjusteTipoCambio').val()
			}
			$.ajax({
				type: 'POST',
				url: 'TipoMoneda.aspx/SimularTipoMoneda',
				contentType: "application/json; charset=utf-8",
				data: $.toJSON(params),
				dataType: 'json',
				success: function (data) {
					if (data.d.Estado == 'OK') {

						if (movil) {
							$('#Simulacion').html($(data.d.Contenido).find('#ContenidoDinamico').html());
						}
						else{
							divActual.html($(data.d.Contenido).find('#ContenidoDinamico').html());
						}

						$('#DSFechaHora').html(new Date(+data.d.FechaHora.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy hh:mm:ss tt"));

						if (uiFilaActual1 != null) {
							uiFilaActual1.addClass('grilla_active_blue');
						}

						if (uiFilaActual2 != null) {
							uiFilaActual2.addClass('grilla_active_red');
						}

						if (uiFilaActual3 != null) {
							uiFilaActual3.addClass('grilla_active_green');
						}

						var datos = new google.visualization.DataTable();
						datos.addColumn('number', 'x');
						datos.addColumn('number', 'S/. Indexados');
						datos.addColumn('number', 'S/. Ajustados');
						datos.addColumn('number', '$ Ajustados');

						for (var i = 0; i < 25; i++) {
							var valor1 = null;
							var valor2 = null;
							var valor3 = null;
							if (data.d.Grafico1 != null) valor1 = data.d.Grafico1[i].Valor;
							if (data.d.Grafico2 != null) valor2 = data.d.Grafico2[i].Valor;
							if (data.d.Grafico3 != null) valor3 = data.d.Grafico3[i].Valor;

							datos.addRow([i +1, valor1, valor2, valor3]);
						}
						
						var formatter = new google.visualization.NumberFormat({
							decimalSymbol: '.',
							fractionDigits: 2,
							groupingSymbol: ',',
							prefix: 'S/. '
						});
						formatter.format(datos, 1);
						formatter.format(datos, 2);
						formatter.format(datos, 3);

						var options = {
							curveType: "function",
							width: 850,
							height: 400,
							chartArea: { width: 705, height: 300, top: 30, right: 0},
							colors: ['#4F81BD', '#C0504D', '#9BBB59'],
							legend: { position: 'bottom', textStyle: { fontSize: 12 } },
							vAxis: { title : 'Pensión (S/.)', format: '#,###', titleTextStyle : { color: 'black', fontName: '"Arial"', fontSize: '11px'} },
							hAxis: { title: 'Años', titleTextStyle: { color: 'black', fontName: '"Arial"', fontSize: '11px' }, minValue: 0, maxValue: 26 },
							vAxis: { viewWindow: { min: 0}, gridlines: { count: 10 }},
							pointSize: 8
						};

						var chart_div = document.getElementById('Grafico');
						var chart = new google.visualization.LineChart(chart_div);

						//OBTENER URL DE LA IMAGEN
						google.visualization.events.addListener(chart, 'ready', function () {
							//chart_div.innerHTML = '<img src="' +chart.getImageURI() + '">';
							$('#HRutaImagenSimulada').val(chart.getImageURI());
						});

						chart.draw(datos, options);

						selectedDate = new Date().valueOf() +parseFloat($("#Timeout").val());
						$clock.countdown(selectedDate.toString());

						/* Mostrar botones de Recálculo, Correo y Pensión Proyectada */
						$(".formLineaBotonSimulacion").show();
					}
					else if (data.d.Estado == 'ERROR') {

						if (movil) {
							$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
							$('#Simulacion,#Simulacion1').html('');
						}
						else{
							divActual.removeClass('areaSimulacionGrafico');
							divActual.html('');
						}

						$('#MCMIcono').attr('class', data.d.Icono);
						$('#MCMContenedor').html(data.d.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
						$('#ModalCuadroMensaje').dialog('open');
					}
					else if (data.d.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						/* Sesión caducada */
						document.location.reload(true);
					}

					if (movil) {
						$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
						$('#Simulacion,#Simulacion1').html('');
					}
					else{
						divActual.removeClass('areaSimulacionGrafico');
						divActual.html('');
					}

					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				},
				complete: function () {
					$('#btnRecalcularTipoMoneda').css("display", "");
					$('#btnNuevoCorreoTipoMoneda').css("display", "");
					$('#btnPensionProyectadaTipoMoneda').css("display", "");
					simulando = false;

					if (movil) {
						$('#TabCotizacionesSimulador input').removeAttr('disabled');
					}
				}
			});
		}
		else {
			$('#MCMIcono').attr('class', 'info');
			$('#MCMContenedor').html('Simulación en proceso, por favor espere a que termine antes de volver a simular.');
			$('#ModalCuadroMensaje').dialog({ title: 'Información' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	}

	//Gráfico Simulador Jubilarse Hoy Futuro
	$('#btnRecalcularJubiliarseHoyFuturo').live('click', function () {
		// Deslizar la pantalla hacia gráfico de simulación
		DeslizarPantallaCorreo();
		RecalcularSimulacionJubilarseHoyFuturo(solicitudNew, divActualNew, uiNew);
	});

	function RecalcularSimulacionJubilarseHoyFuturo(solicitudNewAlt, divActualNewAlt, uiNewAlt) {
		
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#TipoCambio").removeClass("formTextboxError");
		$("#PromedioRentabilitad").removeClass("formTextboxError");
		$("#Inflacion").removeClass("formTextboxError");
		$("#TasaAjuste").removeClass("formTextboxError");

		// Tipo de cambio
		var tipoCambio = true;
		if ($.trim($('#TipoCambio').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Tipo de cambio</strong>. Dato Obligatorio.');
			tipoCambio = false;
		}
				
		// Rentabilidad
		var rentabilidad = true;
		if ($.trim($('#PromedioRentabilitad').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Promedio rentabilidad fondo AFP</strong>. Dato Obligatorio.');
			rentabilidad = false;
		}
				
		// Inflación
		var inflacion = true;
		if ($.trim($('#Inflacion').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Inflación anual</strong>. Dato Obligatorio.');
			inflacion = false;
		}
				
		// Tasa de Ajuste
		var tasaAjuste = true;
		if ($.trim($('#TasaAjuste').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Tasa de ajuste fija anual</strong>. Dato Obligatorio.');
			tasaAjuste = false;
		}
				
		// Clases de controles
		if (!tipoCambio) $("#TipoCambio").addClass("formTextboxError");
		if (!rentabilidad) $("#PromedioRentabilitad").addClass("formTextboxError");
		if (!inflacion) $("#Inflacion").addClass("formTextboxError");
		if (!tasaAjuste) $("#TasaAjuste").addClass("formTextboxError");
				
		esCorrecto = tipoCambio & rentabilidad & inflacion & tasaAjuste;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}
		
		//var simulando = simulandoNewAlt;
		//simulando = simulandoNewAlt;
		var Solicitud = solicitudNewAlt;
		var divActual = divActualNewAlt;
		var ui = uiNewAlt;

		var filaActual;
		var orden;
		//var correlativo;

		if(!simulando) {

			if (movil){
				filaActual = ui.parent().parent();
				orden = ui.attr('name').substr(ui.attr('name').length - 1, 1);
				correlativo = ui.val();

				$('#Simulacion,#Simulacion1').addClass('areaSimulacionGraficoMovil');
				$('#Simulacion,#Simulacion1').html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
				$('#TabCotizacionesSimulador input').attr('disabled', 'disabled');
			}
			else {
				filaActual = ui.draggable.parent().parent();
				orden = divActual.attr("id").substr(divActual.attr("id").length - 1, 1);
				correlativo = ui.draggable.data('cotizacion');

				// Limpiar el contenido
				if (orden == 1) $('#TabCotizacionesSimulador tr').removeClass('grilla_active_blue');
				else $('#TabCotizacionesSimulador tr').removeClass('grilla_active_red');
				divActual.addClass('areaSimulacionGrafico');
				divActual.html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
			}

			simulando = true;
			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				idSolicitud: Solicitud.Id,
				fechaCotizacion: Solicitud.FechaCotizacion,
				cotizaciones: Solicitud.Cotizaciones,
				beneficiarios: Solicitud.Beneficiarios,
				cic: Solicitud.SaldoCIC,
				correlativo: correlativo,
				edadJubilarse: $('#AnhosJubilarse').val(),
				tipoCambio: $('#TipoCambio').val(),
				rentabilidadAFP: $('#PromedioRentabilitad').val(),
				ipc: $('#Inflacion').val(),
				ajuste: $('#TasaAjuste').val(),
				orden: orden
			}
			$.ajax({
				type: 'POST',
				url: 'JubilarseHoyFuturo.aspx/SimularJubilarseHoyFuturo',
				contentType: "application/json; charset=iso-8859-1",
				data: $.toJSON(params),
				dataType: 'json',
				success: function (data) {
					if (data.d.Estado == 'OK') {

						//if (movil) {
						$('#Simulacion1').html($(data.d.Contenido).find('#ContenidoDinamico').html()); 
						//}
						//else {
						//    divActual.html($(data.d.Contenido).find('#ContenidoDinamico').html());
						//}

						$('#DSFechaHora').html(new Date(+data.d.FechaHora.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy hh:mm:ss tt"));
						
						// Pintar la columna
						if (orden == 1) filaActual.addClass('grilla_active_blue');
						else filaActual.addClass('grilla_active_red');
						
						//function drawVisualization(dataValues) {
						var datos = new google.visualization.DataTable();

						datos.addColumn('string', 'Edad del pensionario');
						datos.addColumn('number', 'Pensión total');

						for (var i = 0; i < data.d.Grafico1.length; i++) {
							datos.addRow([data.d.Grafico1[i].Nombre, data.d.Grafico1[i].Valor]);
						}

						var formatter = new google.visualization.NumberFormat({
							decimalSymbol: '.',
							fractionDigits: 2,
							groupingSymbol: ',',
							prefix: 'S/. '
						});
						formatter.format(datos, 1);

						if (orden == 1)
							var options = {
								chartArea: { height: 200, width: 300, top: 30 },
								colors: ['#4F82BE'],
								legend: { position: 'bottom' },
								vAxis: {
									viewWindow: { min: 0 },
									gridlines: { count: 10 }
								}
							};
						else
							var options = {
								chartArea: { height: 200, width: 300, top: 30 },
								colors: ['#CF3E3E'],
								legend: { position: 'bottom' },
								vAxis: {
									viewWindow: { min: 0 },
									gridlines: { count: 10 }
								}
							};
						
						var chart_div = document.getElementById('Grafico' + orden);
						var chart = new google.visualization.ColumnChart(chart_div);

						//OBTENER URL DE LA IMAGEN
						google.visualization.events.addListener(chart, 'ready', function () {
							//chart_div.innerHTML = '<img src="' + chart.getImageURI() + '">';
							$('#HRutaImagenSimulada').val(chart.getImageURI());
						});
						
						chart.draw(datos, options);

						/* Mostrar botones de Recálculo, Correo y Pensión Proyectada */
						$(".formLineaBotonSimulacion").show();
					}
					else if (data.d.Estado == 'ERROR') {

						if (movil) {
							$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
							$('#Simulacion,#Simulacion1').html('');
						}
						else {
							divActual.removeClass('areaSimulacionGrafico');
							divActual.html('');
						}

						$('#MCMIcono').attr('class', data.d.Icono);
						$('#MCMContenedor').html(data.d.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
						$('#ModalCuadroMensaje').dialog('open');
					}
					else if (data.d.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}

					selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
					$clock.countdown(selectedDate.toString());

				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						/* Sesión caducada */
						document.location.reload(true);
					}

					if (movil) {
						$('#Simulacion,#Simulacion1').removeClass('areaSimulacionGraficoMovil');
						$('#Simulacion,#Simulacion1').html('');
					}
					else {
						divActual.removeClass('areaSimulacionGrafico');
						divActual.html('');
					}

					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');

				},
				complete: function () {
					//$('#btnRecalcularJubiliarseHoyFuturo').css("display", "");
					//$('#btnNuevoCorreoJubiliarseHoyFuturo').css("display", "");
					//$('#btnPensionProyectadaJubiliarseHoyFuturo').css("display", "");
					simulando = false;

					if (movil) {
						$('#TabCotizacionesSimulador input').removeAttr('disabled');
					}
					
				}
			});
		}
		else {
			$('#MCMIcono').attr('class', 'info');
			$('#MCMContenedor').html('Simulación en proceso, por favor espere a que termine antes de volver a simular.');
			$('#ModalCuadroMensaje').dialog({ title: 'Información' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	}

	function DeslizarPantallaSimulacion() {
		// Deslizar la pantalla hacia los datos de simulación
		$('html,body').animate({
			scrollTop: $('#tituloSimulador').offset().top
		}, 'slow');
	}
	//<SRI.INI-20322>


	/* Combobox Modalidad/Moneda/Período Garantizado */
	$('#Modalidad,#Moneda,#PeriodoGarantizado').live('change', function () {
		correlativoAnt1 = null;
		correlativoAnt2 = null;
		correlativoAnt3 = null;
		//<SRI.INI-20322>
		correlativoAnt4 = null;
		correlativoAnt5 = null;
		correlativoAnt6 = null;
		//<SRI.FIN-20322>
		if ($('#IdSimulador').val() == 25) {
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 1, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 2, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 3, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
			//<SRI.INI-20322>
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 4, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 5, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 6, movil, null, $('#Moneda').val(), $('#PeriodoGarantizado').val());
			//<SRI.FIN-20322>
		}
		else if ($('#IdSimulador').val() == 26) {
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 1, movil, $('#Modalidad').val(), '001', $('#PeriodoGarantizado').val());
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 2, movil, $('#Modalidad').val(), '013', $('#PeriodoGarantizado').val());
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 3, movil, $('#Modalidad').val(), '014', $('#PeriodoGarantizado').val());
		}
			//<SRI.INI-20322>
		else if ($('#IdSimulador').val() == 39) {
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 1, movil, null, "001", null);
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 2, movil, null, "013", null);
			CargarTablaCotizacionesSimulador(Solicitud.Cotizaciones, $('#IdSimulador').val(), 3, movil, null, "014", null);
		}
		//<SRI.FIN-20322>
		$('#Simulacion').removeClass('areaSimulacionGrafico');
	});



	/* Seleccionar cotización (Móviles) */
	$('#CotizacionesSimulador #TabCotizacionesSimulador input[type=radio]').live('change', function () {
		
		//if (!simulando) {
			
			var radio = $(this);
			var orden = radio.attr('name').substr(radio.attr('name').length - 1, 1);
			var filaActual = radio.parent().parent();

			//Limpiar el contenido
			//var corr;
			if (orden == 1) {
				$('#TabCotizacionesSimulador tr').removeClass('grilla_active_blue');
				correlativoAnt1 = radio.val();
				//<SRI.INI-20322>
				uiFilaActual1New = filaActual
				//corr = 1;
				//<SRI.FIN-20322>
			}
			else if (orden == 2) {
				$('#TabCotizacionesSimulador tr').removeClass('grilla_active_red');
				correlativoAnt2 = radio.val();
				//<SRI.INI-20322>
				uiFilaActual2New = filaActual
				//corr = 2;
				//<SRI.FIN-20322>
			}
			else if (orden == 3) {
				$('#TabCotizacionesSimulador tr').removeClass('grilla_active_green');
				correlativoAnt3 = radio.val();
				//<SRI.INI-20322>
				uiFilaActual3New = filaActual
				//corr = 3;
				//<SRI.FIN-20322>
			}
			else if (orden == 4) {
				$('#TabCotizacionesSimulador tr').removeClass('grilla_active_yellow');
				correlativoAnt4 = radio.val();
				//<SRI.INI-20322>
				uiFilaActual4New = filaActual
				//corr = 4;
				//<SRI.FIN-20322>
			}
			else if (orden == 5) {
				$('#TabCotizacionesSimulador tr').removeClass('grilla_active_purple');
				correlativoAnt5 = radio.val();
				//<SRI.INI-20322>
				uiFilaActual5New = filaActual
				//corr = 5;
				//<SRI.FIN-20322>
			}
			else if (orden == 6) {
				$('#TabCotizacionesSimulador tr').removeClass('grilla_active_pink');
				correlativoAnt6 = radio.val();
				//<SRI.INI-20322>
				uiFilaActual6New = filaActual
				//corr = 6;
				//<SRI.FIN-20322>
			}
			
			//<SRI.INI-20322>
			//$('#Simulacion,#Simulacion1').addClass('areaSimulacionGraficoMovil');
			//$('#Simulacion,#Simulacion1').html('<div width="100%" align="center"><div class="cargandoIzquierda" style="width:370px;height:32px;font-size:14px;padding:15px 0 0 32px;margin-top:10px">Generando la simulación, por favor espere un momento...</div></div>');
			//<SRI.FIN-20322>

			//simulando = true;

			//<SRI.INI-20322>
			//agregar esto a la función
			//$('#TabCotizaciones input').attr('disabled', 'disabled');
			//<SRI.FIN-20322>

			//Jubilarse hoy o Futuro
			if ($('#IdSimulador').val() == 23) {
				
				//<SRI.INI-20322>
				//Inicio graficar
				divActualNew = $(this);
				uiNew = radio;
				RecalcularSimulacionJubilarseHoyFuturo(solicitudNew, divActualNew, uiNew);
				//simulando = false;
				//Fin graficar
				//<SRI.FIN-20322>
			}
				//Renta Vitalicia o Retiro Programado
			else if ($('#IdSimulador').val() == 24) {

				//<SRI.INI-20322>
				//Inicio graficar
				divActualNew = $(this);
				uiNew = radio;
				RecalcularSimulacionRentaVitalicia(simulando, solicitudNew, divActualNew, uiNew);
				//simulando = false;
				//Fin graficar
				//<SRI.FIN-20322>
			}
				//Inmediata o Diferida
			else if ($('#IdSimulador').val() == 25) {

				//<SRI.INI-20322>
				//Inicio graficar
				divActualNew = $(this);
				uiNew = radio;
				uiCorrelativo1New = correlativoAnt1;
				uiCorrelativo2New = correlativoAnt2;
				uiCorrelativo3New = correlativoAnt3;
				uiCorrelativo4New = correlativoAnt4;
				uiCorrelativo5New = correlativoAnt5;
				uiCorrelativo6New = correlativoAnt6;

				RecalcularSimulacionInmediataDiferida(simulando, solicitudNew, divActualNew, uiCorrelativo1New, uiCorrelativo2New, uiCorrelativo3New,
														 uiCorrelativo4New, uiCorrelativo5New, uiCorrelativo6New,
														 uiFilaActual1New, uiFilaActual2New, uiFilaActual3New,
														 uiFilaActual4New, uiFilaActual5New, uiFilaActual6New);
				//simulando = false;
				//Fin graficar
				//<SRI.FIN-20322>
			}
				//Tipo de Moneda
			else if ($('#IdSimulador').val() == 26) {

				//<SRI.INI-20322>
				//Inicio graficar
				divActualNew = $(this);
				uiNew = radio;
				uiCorrelativo1New = correlativoAnt1;
				uiCorrelativo2New = correlativoAnt2;
				uiCorrelativo3New = correlativoAnt3;
				
				RecalcularSimulacionTipoMoneda(simulando, solicitudNew, divActualNew, uiCorrelativo1New, uiCorrelativo2New, uiCorrelativo3New,
												  uiFilaActual1New, uiFilaActual2New, uiFilaActual3New);
				//simulando = false;
				//Fin graficar
				//<SRI.FIN-20322>
			}
				//<SRI.INI-20322>
				//Que me conviene
			else if ($('#IdSimulador').val() == 39) {
				//Inicio graficar
				divActualNew = $(this);
				uiNew = radio;
				uiCorrelativo1New = correlativoAnt1;
				uiCorrelativo2New = correlativoAnt2;
				uiCorrelativo3New = correlativoAnt3;
				
				RecalcularSimulacionQueMeConviene(!simulando, solicitudNew, divActualNew, uiNew,
												  uiCorrelativo1New, uiCorrelativo2New, uiCorrelativo3New,
												  uiFilaActual1New, uiFilaActual2New, uiFilaActual3New);
				//simulando = false;
				//Fin graficar
			}
			//<SRI.FIN-20322>
		//}
		//else {
		//	return false;
		//}
	});

	var cantidadCotizacion = 0;
	//<INIGTI_2145>
	$('#CotizacionesSimulador #TabCotizacionesSimulador input[type=checkbox]').live('change', function () {
		var celda = $(this).parent('td');
		var fila = celda.parent('tr');
		var fil = fila.parent('tbody').children().index(fila);
		
		if (this.checked) {
			if (cantidadCotizacion == 3) {
				var errores = new Array();
				errores.push("Solo puede seleccionar <strong>3 Cotizaciones. </strong>");
				$('#CotizacionesSimulador #TabCotizacionesSimulador tbody tr:eq(' + fil + ') input').prop("checked", "");
				$('#MCMIcono').attr('class', 'validacion');
				$('#MCMContenedor').html(formatearError(errores));
				$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
				$('#ModalCuadroMensaje').dialog('open');
				return false;
			}
			cantidadCotizacion = cantidadCotizacion + 1;
		} else {
			cantidadCotizacion = cantidadCotizacion - 1;
		}
	});

	
	//<FINGTI_2145>

	/*<SRIINI20322>*/
	/* PENSIÓN PROYECTADA */
	/* Jubilarse hoy o a futuro */
	$("#btnPensionProyectadaJubiliarseHoyFuturo").live("click", function() {
		$("#ModalPensionProyectadaCargando").dialog({ position: ["center", "center"] });
		$("#ModalPensionProyectadaCargando").dialog("open");
		CalcularPensionProyectada(Solicitud, correlativo);
	});

	/* Renta Vitalicia o Retiro Programado */
	$("#btnPensionProyectadaRentaVitalicia").live("click", function () {
		$("#ModalPensionProyectadaCargando").dialog({ position: ["center", "center"] });
		$("#ModalPensionProyectadaCargando").dialog("open");
		CalcularPensionProyectada(Solicitud, correlativo);
	});

	/* Inmediata o diferida */
	$("#btnPensionProyectadaInmediataDiferida").live("click", function () {
		$("#ModalPensionProyectadaCargando").dialog({ position: ["center", "center"] });
		$("#ModalPensionProyectadaCargando").dialog("open");
		CalcularPensionProyectada6(Solicitud, correlativoAnt1, correlativoAnt2, correlativoAnt3, correlativoAnt4, correlativoAnt5, correlativoAnt6);
	});

	/* Inmediata o diferida */
	$("#btnPensionProyectadaTipoMoneda").live("click", function () {
		$("#ModalPensionProyectadaCargando").dialog({ position: ["center", "center"] });
		$("#ModalPensionProyectadaCargando").dialog("open");
		CalcularPensionProyectadaMoneda(Solicitud, correlativoAnt1, correlativoAnt2, correlativoAnt3);
	});
	
	/* Qué me conviene */
	$("#btnPensionProyectadaQueMeConviene").live("click", function () {
		$("#ModalPensionProyectadaCargando").dialog({ position: ["center", "center"] });
		$("#ModalPensionProyectadaCargando").dialog("open");
		CalcularPensionProyectada3(Solicitud, correlativoAnt1, correlativoAnt2, correlativoAnt3);
	});

	//<INIGTI_2145>
	/* Comparativo Plazo Fijo */
	$("#btnPensionProyectadaComparativo").live("click", function () {
		$("#ModalPensionProyectadaCargando").dialog({ position: ["center", "center"] });
		$("#ModalPensionProyectadaCargando").dialog("open");
		CalcularPensionProyectadaPlazoFijo();
	});

	//<FINGTI_2145>

	function CalcularPensionProyectada(oSolicitud, correlativo) {
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#TipoCambio").removeClass("formTextboxError");
		$("#PromedioRentabilitad").removeClass("formTextboxError");
		$("#Inflacion").removeClass("formTextboxError");
		$("#TasaAjuste").removeClass("formTextboxError");

		// Tipo de cambio
		var vtipoCambio = true;
		if ($("#TipoCambio").length) {
			if ($.trim($("#TipoCambio").val()).length == 0) {
				errores.push("Ingrese el campo <strong>Tipo de cambio</strong>. Dato Obligatorio.");
				vtipoCambio = false;
			}
		}

		// Rentabilidad
		var rentabilidad = true;
		if ($.trim($('#PromedioRentabilitad').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Promedio rentabilidad fondo AFP</strong>. Dato Obligatorio.');
			rentabilidad = false;
		}

		// Inflación
		var inflacion = true;
		if ($.trim($('#Inflacion').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Inflación anual</strong>. Dato Obligatorio.');
			inflacion = false;
		}

		// Tasa de Ajuste
		var tasaAjuste = true;
		if ($.trim($('#TasaAjuste').val()).length == 0) {
			errores.push('Ingrese el campo <strong>Tasa de ajuste fija anual</strong>. Dato Obligatorio.');
			tasaAjuste = false;
		}

		// Clases de controles
		if (!vtipoCambio) $("#TipoCambio").addClass("formTextboxError");
		if (!rentabilidad) $("#PromedioRentabilitad").addClass("formTextboxError");
		if (!inflacion) $("#Inflacion").addClass("formTextboxError");
		if (!tasaAjuste) $("#TasaAjuste").addClass("formTextboxError");

		esCorrecto = vtipoCambio & rentabilidad & inflacion & tasaAjuste;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}

		var params = {
			tokenUsuario: $("#TokenUsuario").val(),
			beneficiarios: oSolicitud.Beneficiarios,
			cotizaciones: oSolicitud.Cotizaciones,
			correlativo: correlativo,
			cic: oSolicitud.SaldoCIC,
			tipoCambio: ($("#TipoCambio").length) ? $("#TipoCambio").val() : 1,
			rentabilidadAFP: $("#PromedioRentabilitad").val(),
			ipc: $("#Inflacion").val(),
			ajuste: $("#TasaAjuste").val()
		};
		
		$.ajax({
			type: "POST",
			url: "PensionProyectada.aspx/GenerarPensionProyectada",
			contentType: "application/json; charset=iso-8859-1",
			data: $.toJSON(params),
			dataType: "json",
			success: function (data) {
				
				if (data.d.Estado == "OK") {
					location.replace(rutaPensionProyectada);
				}
				else if (data.d.Estado == "ERROR") {
					$("#MCMIcono").attr("class", data.d.Icono);
					$("#MCMContenedor").html(data.d.Mensaje);
					$("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
					$("#ModalCuadroMensaje").dialog("open");
				}
				else if (data.d.Estado == "TOKEN") {
					CerrarSesionExpirada();
				}
				selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
				$clock.countdown(selectedDate.toString());
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}

				$('#MCMIcono').attr('class', 'error');
				$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
				$('#ModalCuadroMensaje').dialog({ title: 'Error' });
				$('#ModalCuadroMensaje').dialog('open');

			},
			complete: function () {
				$("#ModalPensionProyectadaCargando").dialog("close");
			}
		});
	}

	function CalcularPensionProyectada3(oSolicitud, correlativo1, correlativo2, correlativo3) {
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#TipoCambio").removeClass("formTextboxError");
		$("#PromedioRentabilitad").removeClass("formTextboxError");
		$("#Inflacion").removeClass("formTextboxError");
		$("#TasaAjuste").removeClass("formTextboxError");

		if (correlativo1 == null) correlativo1 = 0;
		if (correlativo2 == null) correlativo2 = 0;
		if (correlativo3 == null) correlativo3 = 0;

		// Tipo de cambio
		var vtipoCambio = true;
		if ($("#TipoCambio").length) {
			if ($.trim($("#TipoCambio").val()).length == 0) {
				errores.push("Ingrese el campo <strong>Tipo de cambio</strong>. Dato Obligatorio.");
				vtipoCambio = false;
			}
		}

		// Rentabilidad
		var rentabilidad = true;
		if ($("#PromedioRentabilitad").length) {
			if ($.trim($('#PromedioRentabilitad').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Promedio rentabilidad fondo AFP</strong>. Dato Obligatorio.');
				rentabilidad = false;
			}
		}

		// Inflación
		var inflacion = true;
		if ($("#Inflacion").length) {
			if ($.trim($('#Inflacion').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Inflación anual</strong>. Dato Obligatorio.');
				inflacion = false;
			}
		}

		// Tasa de Ajuste
		var tasaAjuste = true;
		if ($("#TasaAjuste").length) {
			if ($.trim($('#TasaAjuste').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Tasa de ajuste fija anual</strong>. Dato Obligatorio.');
				tasaAjuste = false;
			}
		}

		// Clases de controles
		if (!vtipoCambio) $("#TipoCambio").addClass("formTextboxError");
		if (!rentabilidad) $("#PromedioRentabilitad").addClass("formTextboxError");
		if (!inflacion) $("#Inflacion").addClass("formTextboxError");
		if (!tasaAjuste) $("#TasaAjuste").addClass("formTextboxError");

		esCorrecto = vtipoCambio & rentabilidad & inflacion & tasaAjuste;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}

		var params = {
			tokenUsuario: $("#TokenUsuario").val(),
			beneficiarios: oSolicitud.Beneficiarios,
			cotizaciones: oSolicitud.Cotizaciones,
			correlativo1: correlativo1,
			correlativo2: correlativo2,
			correlativo3: correlativo3,
			cic: oSolicitud.SaldoCIC,
			tipoCambio: ($("#TipoCambio").length) ? $("#TipoCambio").val() : 1,
			rentabilidadAFP: ($("#PromedioRentabilitad").length) ? $("#PromedioRentabilitad").val() : 1,
			ipc: $("#Inflacion").val(),
			ajusteTasa: $("#TasaAjuste").val()
		};
		
		$.ajax({
			type: "POST",
			url: "PensionProyectada.aspx/GenerarPensionProyectada3",
			contentType: "application/json; charset=iso-8859-1",
			data: $.toJSON(params),
			dataType: "json",
			success: function (data) {
				
				if (data.d.Estado == "OK") {
					location.replace(rutaPensionProyectada);
				}
				else if (data.d.Estado == "ERROR") {
					$("#MCMIcono").attr("class", data.d.Icono);
					$("#MCMContenedor").html(data.d.Mensaje);
					$("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
					$("#ModalCuadroMensaje").dialog("open");
				}
				else if (data.d.Estado == "TOKEN") {
					CerrarSesionExpirada();
				}
				selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
				$clock.countdown(selectedDate.toString());
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}

				$('#MCMIcono').attr('class', 'error');
				$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
				$('#ModalCuadroMensaje').dialog({ title: 'Error' });
				$('#ModalCuadroMensaje').dialog('open');

			},
			complete: function () {
				$("#ModalPensionProyectadaCargando").dialog("close");
			}
		});
	}

	function CalcularPensionProyectada6(oSolicitud, correlativo1, correlativo2, correlativo3, correlativo4, correlativo5, correlativo6) {
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#TipoCambio").removeClass("formTextboxError");
		$("#PromedioRentabilitad").removeClass("formTextboxError");
		$("#Inflacion").removeClass("formTextboxError");
		$("#TasaAjuste").removeClass("formTextboxError");

		if (correlativo1 == null) correlativo1 = 0;
		if (correlativo2 == null) correlativo2 = 0;
		if (correlativo3 == null) correlativo3 = 0;
		if (correlativo4 == null) correlativo4 = 0;
		if (correlativo5 == null) correlativo5 = 0;
		if (correlativo6 == null) correlativo6 = 0;

		// Tipo de cambio
		var vtipoCambio = true;
		if ($("#TipoCambio").length) {
			if ($.trim($("#TipoCambio").val()).length == 0) {
				errores.push("Ingrese el campo <strong>Tipo de cambio</strong>. Dato Obligatorio.");
				vtipoCambio = false;
			}
		}

		// Rentabilidad
		var rentabilidad = true;
		if ($("#PromedioRentabilitad").length) {
			if ($.trim($('#PromedioRentabilitad').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Promedio rentabilidad fondo AFP</strong>. Dato Obligatorio.');
				rentabilidad = false;
			}
		}

		// Inflación
		var inflacion = true;
		if ($("#Inflacion").length) {
			if ($.trim($('#Inflacion').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Inflación anual</strong>. Dato Obligatorio.');
				inflacion = false;
			}
		}

		// Tasa de Ajuste
		var tasaAjuste = true;
		if ($("#TasaAjuste").length) {
			if ($.trim($('#TasaAjuste').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Tasa de ajuste fija anual</strong>. Dato Obligatorio.');
				tasaAjuste = false;
			}
		}

		// Clases de controles
		if (!vtipoCambio) $("#TipoCambio").addClass("formTextboxError");
		if (!rentabilidad) $("#PromedioRentabilitad").addClass("formTextboxError");
		if (!inflacion) $("#Inflacion").addClass("formTextboxError");
		if (!tasaAjuste) $("#TasaAjuste").addClass("formTextboxError");

		esCorrecto = vtipoCambio & rentabilidad & inflacion & tasaAjuste;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}

		var params = {
			tokenUsuario: $("#TokenUsuario").val(),
			beneficiarios: oSolicitud.Beneficiarios,
			cotizaciones: oSolicitud.Cotizaciones,
			correlativo1: correlativo1,
			correlativo2: correlativo2,
			correlativo3: correlativo3,
			correlativo4: correlativo4,
			correlativo5: correlativo5,
			correlativo6: correlativo6,
			cic: oSolicitud.SaldoCIC,
			tipoCambio: ($("#TipoCambio").length) ? $("#TipoCambio").val() : 1,
			rentabilidadAFP: ($("#PromedioRentabilitad").length) ? $("#PromedioRentabilitad").val() : 1,
			ipc: $("#Inflacion").val(),
			ajuste: $("#TasaAjuste").val()
		};
		
		$.ajax({
			type: "POST",
			url: "PensionProyectada.aspx/GenerarPensionProyectada6",
			contentType: "application/json; charset=iso-8859-1",
			data: $.toJSON(params),
			dataType: "json",
			success: function (data) {
				
				if (data.d.Estado == "OK") {
					location.replace(rutaPensionProyectada);
				}
				else if (data.d.Estado == "ERROR") {
					$("#MCMIcono").attr("class", data.d.Icono);
					$("#MCMContenedor").html(data.d.Mensaje);
					$("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
					$("#ModalCuadroMensaje").dialog("open");
				}
				else if (data.d.Estado == "TOKEN") {
					CerrarSesionExpirada();
				}
				selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
				$clock.countdown(selectedDate.toString());
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}

				$('#MCMIcono').attr('class', 'error');
				$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
				$('#ModalCuadroMensaje').dialog({ title: 'Error' });
				$('#ModalCuadroMensaje').dialog('open');

			},
			complete: function () {
				$("#ModalPensionProyectadaCargando").dialog("close");
			}
		});
	}

	function CalcularPensionProyectadaMoneda(oSolicitud, correlativo1, correlativo2, correlativo3) {
		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#TipoCambio").removeClass("formTextboxError");
		$("#PromedioRentabilitad").removeClass("formTextboxError");
		$("#Inflacion").removeClass("formTextboxError");
		$("#TasaAjuste").removeClass("formTextboxError");
		$("#AjusteTipoCambio").removeClass("formTextboxError");
		//$("#Modalidad").removeClass("formComboboxErrorContenedor");
		//$("#PeriodoGarantizado").removeClass("formComboboxErrorContenedor");

		if (correlativo1 == null) correlativo1 = 0;
		if (correlativo2 == null) correlativo2 = 0;
		if (correlativo3 == null) correlativo3 = 0;

		// Tipo de cambio
		var vtipoCambio = true;
		if ($("#TipoCambio").length) {
			if ($.trim($("#TipoCambio").val()).length == 0) {
				errores.push("Ingrese el campo <strong>Tipo de cambio</strong>. Dato Obligatorio.");
				vtipoCambio = false;
			}
		}

		// Rentabilidad
		var rentabilidad = true;
		if ($("#PromedioRentabilitad").length) {
			if ($.trim($('#PromedioRentabilitad').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Promedio rentabilidad fondo AFP</strong>. Dato Obligatorio.');
				rentabilidad = false;
			}
		}

		// Inflación
		var inflacion = true;
		if ($("#Inflacion").length) {
			if ($.trim($('#Inflacion').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Inflación anual</strong>. Dato Obligatorio.');
				inflacion = false;
			}
		}

		// Tasa de Ajuste
		var tasaAjuste = true;
		if ($("#TasaAjuste").length) {
			if ($.trim($('#TasaAjuste').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Tasa de ajuste fija anual</strong>. Dato Obligatorio.');
				tasaAjuste = false;
			}
		}

		// Ajuste Tipo de Cambio
		var tasaAjusteTC = true;
		if ($("#AjusteTipoCambio").length) {
			if ($.trim($('#AjusteTipoCambio').val()).length == 0) {
				errores.push('Ingrese el campo <strong>Ajuste tipo de cambio</strong>. Dato Obligatorio.');
				tasaAjusteTC = false;
			}
		}

		// Clases de controles
		if (!vtipoCambio) $("#TipoCambio").addClass("formTextboxError");
		if (!rentabilidad) $("#PromedioRentabilitad").addClass("formTextboxError");
		if (!inflacion) $("#Inflacion").addClass("formTextboxError");
		if (!tasaAjuste) $("#TasaAjuste").addClass("formTextboxError");
		if (!tasaAjusteTC) $("#AjusteTipoCambio").addClass("formTextboxError");

		esCorrecto = vtipoCambio & rentabilidad & inflacion & tasaAjuste & tasaAjusteTC;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}

		var params = {
			tokenUsuario: $("#TokenUsuario").val(),
			beneficiarios: oSolicitud.Beneficiarios,
			cotizaciones: oSolicitud.Cotizaciones,
			correlativo1: correlativo1,
			correlativo2: correlativo2,
			correlativo3: correlativo3,
			modalidad: $("#Modalidad").val(),
			periodoGarantizado: $("#PeriodoGarantizado").val(),
			cic: oSolicitud.SaldoCIC,
			tipoCambio: ($("#TipoCambio").length) ? $("#TipoCambio").val() : 1,
			rentabilidadAFP: ($("#PromedioRentabilitad").length) ? $("#PromedioRentabilitad").val() : 1,
			ipc: $("#Inflacion").val(),
			ajusteTasa: $("#TasaAjuste").val(),
			ajusteTC: $("#AjusteTipoCambio").val()
		};
		
		$.ajax({
			type: "POST",
			url: "PensionProyectada.aspx/GenerarPensionProyectadaMoneda",
			contentType: "application/json; charset=iso-8859-1",
			data: $.toJSON(params),
			dataType: "json",
			success: function (data) {
				
				if (data.d.Estado == "OK") {
					location.replace(rutaPensionProyectada);
				}
				else if (data.d.Estado == "ERROR") {
					$("#MCMIcono").attr("class", data.d.Icono);
					$("#MCMContenedor").html(data.d.Mensaje);
					$("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
					$("#ModalCuadroMensaje").dialog("open");
				}
				else if (data.d.Estado == "TOKEN") {
					CerrarSesionExpirada();
				}
				selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
				$clock.countdown(selectedDate.toString());
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}

				$('#MCMIcono').attr('class', 'error');
				$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
				$('#ModalCuadroMensaje').dialog({ title: 'Error' });
				$('#ModalCuadroMensaje').dialog('open');

			},
			complete: function () {
				$("#ModalPensionProyectadaCargando").dialog("close");
			}
		});
	}
	/*<SRIINI20322>*/

	//<INIGTI_2145>
	function CalcularPensionProyectadaPlazoFijo() {

		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		var cotizacion = new Array();


		var bCotizacion = true;
		var bDepositoPlazo = true;
		var bTea = true;
		var bTem = true;
		var bAños = true;
		var bInteresDeposito = true;
		var bTasaAjusteIDX = true;
		var bTasaAjusteAJS = true;
		var bTipoCambio = true;
		var bCrecimiento = true;

		//Limpiando .removeClass("formTextboxError");
		$('#DepositoPlazo').removeClass('formTextboxError');
		$('#TEA').removeClass('formTextboxError');
		$('#TEM').removeClass('formTextboxError');
		$('#Anhos').removeClass('formTextboxError');
		$('#InteresDeposito').removeClass('formTextboxError');
		$('#TasaAjusteIDX').removeClass('formTextboxError');
		$('#TasaAjusteAJS').removeClass('formTextboxError');
		$('#TipoCambioParametro').removeClass('formTextboxError');
		$('#Crecimiento').removeClass('formTextboxError');


		if ($.trim($("#DepositoPlazo").val()).length == 0) {
			errores.push("El campo <strong>Depósito a Plazo</strong> debe tener un valor.");
			bDepositoPlazo = false;
		}

		if ($.trim($("#TEA").val()).length == 0) {
			errores.push("El campo <strong>TEA</strong> debe tener un valor.");
			bTea = false;
		}

		if ($.trim($("#TEM").val()).length == 0) {
			errores.push("El campo <strong>TEM</strong> debe tener un valor.");
			bTem = false;
		}

		if ($.trim($("#Anhos").val()).length == 0) {
			errores.push("El campo <strong>Años</strong> debe tener un valor.");
			bAños = false;
		}

		if ($.trim($("#InteresDeposito").val()).length == 0) {
			errores.push("El campo <strong>Interés Depósito</strong> debe tener un valor.");
			bInteresDeposito = false;
		}

		if ($.trim($("#TasaAjusteIDX").val()).length == 0) {
			errores.push("El campo <strong>Tasa Ajuste IDX</strong> debe tener un valor.");
			bTasaAjusteIDX = false;
		}

		if ($.trim($("#TasaAjusteAJS").val()).length == 0) {
			errores.push("El campo <strong>Tasa Ajuste AJS</strong> debe tener un valor.");
			bTasaAjusteAJS = false;
		}

		if ($.trim($("#TipoCambioParametro").val()).length == 0) {
			errores.push("El campo <strong>Tipo de Cambio</strong> debe tener un valor.");
			bTipoCambio = false;
		}

		if ($.trim($("#Crecimiento").val()).length == 0) {
			errores.push("El campo <strong>Crecimiento TC dólar por año</strong> debe tener un valor.");
			bCrecimiento = false;
		}


		for (i = 0; i < $("#TabCotizacionesSimulador tbody tr").length; i++) {
			if ($("#TabCotizacionesSimulador tbody tr:eq(" + i + ") input").is(":checked")) {
				var cot = $("#TabCotizacionesSimulador tbody tr:eq(" + i + ") input").data("cotizacion");
				cotizacion.push(cot);
			}
		}


		if ($('#TabCotizacionesSimulador input[type=checkbox]:checked').length == 0) {
			errores.push("Seleccione al menos una <strong>Cotización</strong>.");
			bCotizacion = false;
		}


		//if (!bCotizacion) $('#ModBusAfiApellidoPaterno_RP').addClass('formTextboxError');
		if (!bDepositoPlazo) $('#DepositoPlazo').addClass('formTextboxError');
		if (!bTea) $('#TEA').addClass('formTextboxError');
		if (!bTem) $('#TEM').addClass('formTextboxError');
		if (!bAños) $('#Anhos').addClass('formTextboxError');
		if (!bInteresDeposito) $('#InteresDeposito').addClass('formTextboxError');
		if (!bTasaAjusteIDX) $('#TasaAjusteIDX').addClass('formTextboxError');
		if (!bTasaAjusteAJS) $('#TasaAjusteAJS').addClass('formTextboxError');
		if (!bTipoCambio) $('#TipoCambioParametro').addClass('formTextboxError');
		if (!bCrecimiento) $('#Crecimiento').addClass('formTextboxError');

		esCorrecto = bCotizacion & bDepositoPlazo & bTea & bTem & bAños & bInteresDeposito & bTasaAjusteIDX
			& bTasaAjusteAJS & bTipoCambio & bCrecimiento;

		if (!esCorrecto) {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');

			$("#btnComparar").attr('class', 'boton darkblue sharp');
			$("#btnRecalcularComparativo").attr('class', 'boton darkblue sharp');

			//$("#btnComparar").prop("disabled", true);
			//$("#btnRecalcularComparativo").prop("disabled", true);

			return;
		} else {
			//Deslizar la pantalla hacia gráfico de simulación
			DeslizarPantallaCorreo();
		}

		var DSAfiliado = $('#DSAfiliado').html();

		var params = {
			tokenUsuario: $('#TokenUsuario').val(),
			cotizaciones: Solicitud.Cotizaciones,
			cotizacion: cotizacion,
			depositoPlazo: $("#DepositoPlazo").val(),
			tea: $("#TEA").val(),
			//tem: $("#TEM").val(),
			//interesDeposito: $("#InteresDeposito").val(),
			ajusteIDX: $("#TasaAjusteIDX").val(),
			ajusteAJS: $("#TasaAjusteAJS").val(),
			tipoCambio: $("#TipoCambioParametro").val(),
			crecimiento: $("#Crecimiento").val(),
			anhos: $("#Anhos").val(),
			afiliado: DSAfiliado
		}

		$.ajax({
			type: "POST",
			url: "PensionProyectada.aspx/GenerarPensionProyectadaPlazoFijo",
			contentType: "application/json; charset=iso-8859-1",
			data: $.toJSON(params),
			dataType: "json",
			success: function (data) {

				 
				if (data.d.Estado == "OK") {
					location.replace(rutaPensionProyectada);
				}
				else if (data.d.Estado == "ERROR") {
					$("#MCMIcono").attr("class", data.d.Icono);
					$("#MCMContenedor").html(data.d.Mensaje);
					$("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
					$("#ModalCuadroMensaje").dialog("open");
				}
				else if (data.d.Estado == "TOKEN") {
					CerrarSesionExpirada();
				}
				selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
				$clock.countdown(selectedDate.toString());
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}

				$('#MCMIcono').attr('class', 'error');
				$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
				$('#ModalCuadroMensaje').dialog({ title: 'Error' });
				$('#ModalCuadroMensaje').dialog('open');

				$("#btnComparar").attr('class', 'boton darkblue sharp');
				$("#btnRecalcularComparativo").attr('class', 'boton darkblue sharp');

				//$("#btnComparar").prop("disabled", true);
				//$("#btnRecalcularComparativo").prop("disabled", true);

			},
			complete: function () {
				$("#ModalPensionProyectadaCargando").dialog("close");

				$("#btnComparar").attr('class', 'boton darkblue sharp');
				$("#btnRecalcularComparativo").attr('class', 'boton darkblue sharp');

				//$("#btnComparar").prop("disabled", true);
				//$("#btnRecalcularComparativo").prop("disabled", true);
			}
		});


	}

	//<FINGTI_2145>

	/*<SRI.INI-20322>*/
	function DeslizarPantallaCorreo() {
		// Deslizar la pantalla hacia los datos de simulación
		$('html,body').animate({
			scrollTop: $('#Simulaciones').offset().top
		}, 'slow');
	}
	/*<SRI.FIN-20322>*/

	/*<SRI.INI-20322>*/
	/* Botón Correo Electrónico Que me Conviene*/
	$('#btnNuevoCorreoQueMeConviene').live('click', function () {

		if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

			$('#ModEnvCorCargandoSimulador').show();

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
				nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
				cuspp: $.trim($('#BusAfiCUSPP').val()),
				rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
			}

			LimpiarFormularioCorreo();

			DeslizarPantallaCorreo();

			$.ajax({
				type: 'POST',
				url: 'QueMeConviene.aspx/CrearDatosCorreo',
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					/* Correo Creado */
					Correo = data.d;
					if (Correo.Respuesta.Estado == 'OK') {
						$('#ModEnvCorDe').val(Correo.De);
						$('#ModEnvCorPara').val(Correo.Para);
						$('#ModEnvCorAsunto').val(Correo.Asunto);
						$('#ModEnvCorAdjunto').html(Correo.Adjunto);
						$('#ModEnvCorMensaje').val(Correo.Mensaje);

						$('#ModEnvCorCargandoSimulador').fadeOut();
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

						$('#ModalEnvioCorreoSimulador').dialog('close');
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
					$('#ModalEnvioCorreoSimulador').dialog('close');
				}
			});

			$('#ModalEnvioCorreoSimulador').dialog('open');
		}
		else {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html('Error al guardar la Cotización para el envío');
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	});
	/*<SRI.FIN-20322>*/

	/*<SRI.INI-20322>*/
	/* Botón Correo Electrónico Simulador Jubilación Hoy o Futuro*/
	$('#btnNuevoCorreoJubiliarseHoyFuturo').live('click', function () {

		if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

			$('#ModEnvCorCargandoSimulador').show();

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
				nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
				cuspp: $.trim($('#BusAfiCUSPP').val()),
				rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
			}

			LimpiarFormularioCorreo();

			DeslizarPantallaCorreo();

			$.ajax({
				type: 'POST',
				url: 'JubilarseHoyFuturo.aspx/CrearDatosCorreo',
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					/* Correo Creado */
					Correo = data.d;
					if (Correo.Respuesta.Estado == 'OK') {
						$('#ModEnvCorDe').val(Correo.De);
						$('#ModEnvCorPara').val(Correo.Para);
						$('#ModEnvCorAsunto').val(Correo.Asunto);
						$('#ModEnvCorAdjunto').html(Correo.Adjunto);
						$('#ModEnvCorMensaje').val(Correo.Mensaje);

						$('#ModEnvCorCargandoSimulador').fadeOut();
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

						$('#ModalEnvioCorreoSimulador').dialog('close');
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
					$('#ModalEnvioCorreoSimulador').dialog('close');
				}
			});

			$('#ModalEnvioCorreoSimulador').dialog('open');

		}
		else {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html('Error al guardar la Cotización para el envío');
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	});
	/*<SRI.FIN-20322>*/

	/*<SRI.INI-20322>*/
	/* Botón Correo Electrónico Simulador Renta Vitalicia Retiro Programado*/
	$('#btnNuevoCorreoRentaVitalicia').live('click', function () {

		if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

			$('#ModEnvCorCargandoSimulador').show();

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
				nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
				cuspp: $.trim($('#BusAfiCUSPP').val()),
				rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
			}

			LimpiarFormularioCorreo();

			DeslizarPantallaCorreo();

			$.ajax({
				type: 'POST',
				url: 'RentaVitaliciaRetiroProgramado.aspx/CrearDatosCorreo',
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					/* Correo Creado */
					Correo = data.d;
					if (Correo.Respuesta.Estado == 'OK') {
						$('#ModEnvCorDe').val(Correo.De);
						$('#ModEnvCorPara').val(Correo.Para);
						$('#ModEnvCorAsunto').val(Correo.Asunto);
						$('#ModEnvCorAdjunto').html(Correo.Adjunto);
						$('#ModEnvCorMensaje').val(Correo.Mensaje);

						$('#ModEnvCorCargandoSimulador').fadeOut();
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

						$('#ModalEnvioCorreoSimulador').dialog('close');
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
					$('#ModalEnvioCorreoSimulador').dialog('close');
				}
			});

			$('#ModalEnvioCorreoSimulador').dialog('open');
		}
		else {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html('Error al guardar la Cotización para el envío');
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	});
	/*<SRI.FIN-20322>*/

	/* Botón Correo Electrónico Simulador Inmediata Diferida*/
	$('#btnNuevoCorreoInmediataDiferida').live('click', function () {

		if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

			$('#ModEnvCorCargandoSimulador').show();

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
				nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
				cuspp: $.trim($('#BusAfiCUSPP').val()),
				rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
			}

			LimpiarFormularioCorreo();

			DeslizarPantallaCorreo();

			$.ajax({
				type: 'POST',
				url: 'InmediataDiferida.aspx/CrearDatosCorreo',
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					/* Correo Creado */
					Correo = data.d;
					if (Correo.Respuesta.Estado == 'OK') {
						$('#ModEnvCorDe').val(Correo.De);
						$('#ModEnvCorPara').val(Correo.Para);
						$('#ModEnvCorAsunto').val(Correo.Asunto);
						$('#ModEnvCorAdjunto').html(Correo.Adjunto);
						$('#ModEnvCorMensaje').val(Correo.Mensaje);

						$('#ModEnvCorCargandoSimulador').fadeOut();
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

						$('#ModalEnvioCorreoSimulador').dialog('close');
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
					$('#ModalEnvioCorreoSimulador').dialog('close');
				}
			});

			$('#ModalEnvioCorreoSimulador').dialog('open');
		}
		else {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html('Error al guardar la Cotización para el envío');
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	});
	/*<SRI.FIN-20322>*/

	/*<SRI.INI-20322>*/
	/* Botón Correo Electrónico Simulador Tipo Moneda*/
	$('#btnNuevoCorreoTipoMoneda').live('click', function () {

		if ($.trim($('#HRutaImagenSimulada').val()).length > 0) {

			$('#ModEnvCorCargandoSimulador').show();

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
				nroCorrelativoSolicitud: $.trim($('#HCorrelativoSolicitud').val()),
				cuspp: $.trim($('#BusAfiCUSPP').val()),
				rutaImagenSimulada: $.trim($('#HRutaImagenSimulada').val())
			}

			LimpiarFormularioCorreo();

			DeslizarPantallaCorreo();

			$.ajax({
				type: 'POST',
				url: 'TipoMoneda.aspx/CrearDatosCorreo',
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					/* Correo Creado */
					Correo = data.d;
					if (Correo.Respuesta.Estado == 'OK') {
						$('#ModEnvCorDe').val(Correo.De);
						$('#ModEnvCorPara').val(Correo.Para);
						$('#ModEnvCorAsunto').val(Correo.Asunto);
						$('#ModEnvCorAdjunto').html(Correo.Adjunto);
						$('#ModEnvCorMensaje').val(Correo.Mensaje);

						$('#ModEnvCorCargandoSimulador').fadeOut();
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

						$('#ModalEnvioCorreoSimulador').dialog('close');
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
					$('#ModalEnvioCorreoSimulador').dialog('close');
				}
			});

			$('#ModalEnvioCorreoSimulador').dialog('open');
		}
		else {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html('Error al guardar la Cotización para el envío');
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
		}
	});
	/*<SRI.FIN-20322>*/

	/*<SRI.INI-20322>*/
	/* Botón Enviar Correo Electrónico Simulador Jubilación Hoy o Futuro*/
	$('#ModEnvCorEnviarSimuladorJubilarseHoyFuturo').live('click', function () {
		$('#ModEnvCorCargandoSimulador').fadeIn();

		Correo.Mensaje = $('#ModEnvCorMensaje').val();
		Correo.Respuesta = null;

		var params = {
			tokenUsuario: $('#TokenUsuario').val(),
			correo: Correo
		}
		$.ajax({
			type: 'POST',
			url: 'JubilarseHoyFuturo.aspx/EnviarCorreoElectronicoSimulador',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {
				if (data.d.Estado == 'OK') {
					$('#ModalEnvioCorreoSimulador').dialog('close');

					$('#MCMIcono').attr('class', data.d.Icono);
					$('#MCMContenedor').html(data.d.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
					$('#ModalCuadroMensaje').dialog('open');

					selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
					$clock.countdown(selectedDate.toString());
				}
				else if (data.d.Estado == 'TOKEN') {
					CerrarSesionExpirada();
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
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');

					$('#ModEnvCorCargandoSimulador').fadeOut();
				}
			}
		});
	});
	/*<SRI.INI-20322>*/

	/*<SRI.INI-20322>*/
	/* Botón Enviar Correo Electrónico Simulador Renta Vitalicia*/
	$('#ModEnvCorEnviarSimuladorRentaVitalicia').live('click', function () {
		$('#ModEnvCorCargandoSimulador').fadeIn();

		Correo.Mensaje = $('#ModEnvCorMensaje').val();
		Correo.Respuesta = null;

		var params = {
			tokenUsuario: $('#TokenUsuario').val(),
			correo: Correo
		}
		$.ajax({
			type: 'POST',
			url: 'RentaVitaliciaRetiroProgramado.aspx/EnviarCorreoElectronicoSimulador',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {
				if (data.d.Estado == 'OK') {
					$('#ModalEnvioCorreoSimulador').dialog('close');

					$('#MCMIcono').attr('class', data.d.Icono);
					$('#MCMContenedor').html(data.d.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
					$('#ModalCuadroMensaje').dialog('open');

					selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
					$clock.countdown(selectedDate.toString());
				}
				else if (data.d.Estado == 'TOKEN') {
					CerrarSesionExpirada();
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
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');

					$('#ModEnvCorCargandoSimulador').fadeOut();
				}
			}
		});
	});
	/*<SRI.INI-20322>*/

	/*<SRI.INI-20322>*/
	/* Botón Enviar Correo Electrónico Simulador Inmediata Diferida*/
	$('#ModEnvCorEnviarSimuladorInmediataDiferida').live('click', function () {
		$('#ModEnvCorCargandoSimulador').fadeIn();

		Correo.Mensaje = $('#ModEnvCorMensaje').val();
		Correo.Respuesta = null;

		var params = {
			tokenUsuario: $('#TokenUsuario').val(),
			correo: Correo
		}
		$.ajax({
			type: 'POST',
			url: 'InmediataDiferida.aspx/EnviarCorreoElectronicoSimulador',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {
				if (data.d.Estado == 'OK') {
					$('#ModalEnvioCorreoSimulador').dialog('close');

					$('#MCMIcono').attr('class', data.d.Icono);
					$('#MCMContenedor').html(data.d.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
					$('#ModalCuadroMensaje').dialog('open');

					selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
					$clock.countdown(selectedDate.toString());
				}
				else if (data.d.Estado == 'TOKEN') {
					CerrarSesionExpirada();
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
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');

					$('#ModEnvCorCargandoSimulador').fadeOut();
				}
			}
		});
	});
	/*<SRI.INI-20322>*/

	/*<SRI.INI-20322>*/
	/* Botón Enviar Correo Electrónico Simulador Tipo Moneda*/
	$('#ModEnvCorEnviarSimuladorTipoMoneda').live('click', function () {
		$('#ModEnvCorCargandoSimulador').fadeIn();

		Correo.Mensaje = $('#ModEnvCorMensaje').val();
		Correo.Respuesta = null;

		var params = {
			tokenUsuario: $('#TokenUsuario').val(),
			correo: Correo
		}
		$.ajax({
			type: 'POST',
			url: 'TipoMoneda.aspx/EnviarCorreoElectronicoSimulador',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {
				if (data.d.Estado == 'OK') {
					$('#ModalEnvioCorreoSimulador').dialog('close');

					$('#MCMIcono').attr('class', data.d.Icono);
					$('#MCMContenedor').html(data.d.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
					$('#ModalCuadroMensaje').dialog('open');

					selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
					$clock.countdown(selectedDate.toString());
				}
				else if (data.d.Estado == 'TOKEN') {
					CerrarSesionExpirada();
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
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');

					$('#ModEnvCorCargandoSimulador').fadeOut();
				}
			}
		});
	});
	/*<SRI.INI-20322>*/


	//<SOLINI25621>

	function ObtenerMoneda(tiporenta) {
		$.ajax({
			type: 'POST',
			url: 'CapitalRequerido.aspx/ObtenerMoneda',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: "{tiporenta:'" + tiporenta + "'}",
			success: function (data) {

				var objdata = data.d;
				$("#Moneda").empty();
				for (var n = 0; n < objdata.length; n++) {
						$("#Moneda")[0].options[n] = new Option(objdata[n].Nombre, objdata[n].Id);
						if (n == 0) {
							$("#Moneda").val(objdata[n].Id);
							$('#TexMoneda').html($('#Moneda').find(':selected').text());
						}
				};

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
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de monedas.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
			}
		});
	}

	function ObtenerPeriodoGarantizado(tiporenta, temporalidad) {
		$.ajax({
			type: 'POST',
			url: 'CapitalRequerido.aspx/ObtenerPeriodoGarantizado',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: "{tiporenta:'" + tiporenta + "', temporalidad:'" + temporalidad + "'}",
			success: function (data) {

				var objdata = data.d;
				$("#PeriodoGarantizado").empty();
				for (var n = 0; n < objdata.length; n++) {
					$("#PeriodoGarantizado")[0].options[n] = new Option(objdata[n].Glosa, objdata[n].Id);
					if (n == 0) {
						$("#PeriodoGarantizado").val(objdata[n].Id);
						$('#TexPeriodoGarantizado').html($('#PeriodoGarantizado').find(':selected').text());
					}
				};

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

	$('#TipoRenta').live('change', function (e) {
		//ObtenerMoneda
		ObtenerMoneda($("#TipoRenta").val());
		ObtenerPeriodoGarantizado($("#TipoRenta").val(), $("#Temporalidad").val());

		if ($("#TipoRenta").val() == "RPV") {
			$("#divTemporalidad").show();
		} else {
			$("#divTemporalidad").hide();
		}

	});

	$('#Temporalidad').live('change', function (e) {
		//Periodo Garantizado
		ObtenerPeriodoGarantizado($("#TipoRenta").val(), $("#Temporalidad").val());
	});


	/* GRUPO FAMILIAR */
	var idGrupoFamiliar;
	/* Botón Nuevo */
	$('#NuevoBeneficiario').live('click', function () {
		
		if (permisoNuevoBeneficiario) {
			idGrupoFamiliar = 0;
			
			$.ajax({
				type: 'POST',
				url: '../RentaPrivada/GrupoFamiliarAfiliado.aspx/SessionIdGrupoFamiliar',
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "', paginaLlamada:'../Simuladores/CapitalRequerido.aspx#grupo_familiar'}",
				success: function (data) {

					var grupofamiliar = data.d;
					if (idGrupoFamiliar.toString() == grupofamiliar.toString()) {
						window.location.href = "../RentaPrivada/GrupoFamiliarAfiliado.aspx";
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
					$('#ModalGrupoFamiliar').dialog('close');
				}
			});
		}
		else {
			return false;
		}
	});



	/* Botón Modificar */
	$('#TabrupoFamiliarCapitalRequerido .grilla_editar').live('click', function () {
		idGrupoFamiliar = $(this).data('grupofamiliar');
		//idGrupoFamiliar = 0;
		$.ajax({
			type: 'POST',
			url: '../RentaPrivada/GrupoFamiliarAfiliado.aspx/SessionIdGrupoFamiliar',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: "{idGrupoFamiliar:'" + idGrupoFamiliar + "', paginaLlamada:'../Simuladores/CapitalRequerido.aspx#grupo_familiar'}",
			success: function (data) {

				var grupofamiliar = data.d;
				if (idGrupoFamiliar.toString() == grupofamiliar.toString()) {
					window.location.href = "../RentaPrivada/GrupoFamiliarAfiliado.aspx";
				}
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información del grupo familiar.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
				$('#ModalGrupoFamiliar').dialog('close');
			}
		});

	});


	/* Cargando grupo familiar*/
	function CargandoGrupoFamiliar() {
		$('#ModGruFamModo_RP').val('M');

		LimpiarFormularioGrupoFamiliar();

		//$('#ModGruFamCargando_RP').show();
		//$('#ModalGrupoFamiliar_RP').dialog('open');
		//idGrupoFamiliar = $(this).data('grupofamiliar');
		idGrupoFamiliar = $("#ModIdGrupoFamiliar").val();
		var params = {
			idGrupoFamiliar: idGrupoFamiliar
		}

		$.ajax({
			type: 'POST',
			url: '../RentaPrivada/GrupoFamiliarAfiliado.aspx/ObtenerDatosGrupoFamiliar',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {
				var grupo = data.d;
				// Apellido Paterno
				$('#ModGruFamApellidoPaterno_RP').val(grupo.ApellidoPaterno);
				// Apellido Materno
				$('#ModGruFamApellidoMaterno_RP').val(grupo.ApellidoMaterno);
				// Nombres
				$('#ModGruFamNombres_RP').val(grupo.Nombre);
				// Tipo de Identificación
				$('#ModGruFamTipoIdentificacion_RP').val(grupo.Identificacion.IdTipo);
				$('#TexModGruFamTipoIdentificacion_RP').html($('#ModGruFamTipoIdentificacion_RP').find(':selected').text());
				// Nro. de Identificación
				$('#ModGruFamNumeroIdentificacion_RP').val(grupo.Identificacion.Numero);
				// Parentesco
				$('#ModGruFamParentesco_RP').val(grupo.Parentesco.Id);
				$('#TexModGruFamParentesco_RP').html($('#ModGruFamParentesco_RP').find(':selected').text());
				if (grupo.Parentesco.Id == '80') {
					$('#ModGruFamParentesco_RP').attr('disabled', 'disabled');
					$('#ConModGruFamParentesco_RP').addClass('formComboboxReadOnlyContenedor');
				}
				else {
					$('#ModGruFamParentesco_RP').removeAttr('disabled');
					$('#ConModGruFamParentesco_RP').removeClass('formComboboxReadOnlyContenedor');
				}
				// Sexo
				$('#ModGruFamSexo_RP').val(grupo.Sexo);
				$('#TexModGruFamSexo_RP').html($('#ModGruFamSexo_RP').find(':selected').text());
				if (grupo.Parentesco.Id == '80') {
					$('#ModGruFamSexo_RP').attr('disabled', 'disabled');
					$('#ConModGruFamSexo_RP').addClass('formComboboxReadOnlyContenedor');
				}
				else {
					$('#ModGruFamSexo_RP').removeAttr('disabled');
					$('#ConModGruFamSexo_RP').removeClass('formComboboxReadOnlyContenedor');
				}
				// Fecha de Nacimiento
				$('#ModGruFamFechaNacimiento_RP').val(new Date(+grupo.FechaNacimiento.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
				// Indicador de Invalidez
				$('#ModGruFamIndInvalidez_RP').val(grupo.Invalido ? 'S' : 'N');
				$('#TexModGruFamIndInvalidez_RP').html($('#ModGruFamIndInvalidez_RP').find(':selected').text());
				if (grupo.Invalido) {
					// Tipo de Invalidez
					$('#ModGruFamTipoInvalidez_RP').val(grupo.TipoInvalidez.Id);
					$('#TexModGruFamTipoInvalidez_RP').html($('#ModGruFamTipoInvalidez_RP').find(':selected').text());
					$('#ModGruFamTipoInvalidez_RP').removeAttr('disabled');
					$('#ConModGruFamTipoInvalidez_RP').removeClass('formComboboxReadOnlyContenedor');
					// Fecha de Invalidez
					if (grupo.FechaInvalidez != null)
						$('#ModGruFamFechaInvalidez_RP').val(new Date(+grupo.FechaInvalidez.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
					$('#ModGruFamFechaInvalidez_RP').removeAttr('disabled');
					$('#ModGruFamFechaInvalidez_RP').removeClass('formCalendarReadOnly');
				}
				else {
					// Tipo de Invalidez
					$('#ModGruFamTipoInvalidez_RP').val('N');
					$('#TexModGruFamTipoInvalidez_RP').html($('#ModGruFamTipoInvalidez_RP').find(':selected').text());
					$('#ModGruFamTipoInvalidez_RP').attr('disabled', 'disabled');
					// Fecha de Invalidez
					$('#ModGruFamFechaInvalidez_RP').val('');
					$('#ModGruFamFechaInvalidez_RP').attr('disabled', 'disabled');
				}

				$('#ModGruFamApellidoPaterno_RP').focus();
				$('#ModGruFamCargando_RP').fadeOut();
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
				$('#ModalGrupoFamiliar_RP').dialog('close');
			}
		});
	}



	/*Calcular Capital*/
	function CalcularCapital() {

		$('#MCIcono').attr('class', 'cargando');
		$('#MCContenedor').html('Calculando el capital requerido...');
		$('#ModalCotizando').dialog({ title: 'Calculando' });
		$('#ModalCotizando').dialog('open');

		var idGrupoFamiliar = "";
		// Obtener la lista de beneficiarios
		for (i = 0; i < $('#TabrupoFamiliarCapitalRequerido tbody tr').length; i++) {
			if ($('#TabrupoFamiliarCapitalRequerido tbody tr:eq(' + i + ') input').is(':checked')) {
				if (i == 0) {
					idGrupoFamiliar = $('#TabrupoFamiliarCapitalRequerido tbody tr:eq(' + i + ') input').val();
				} else {
					idGrupoFamiliar = idGrupoFamiliar + "," + $('#TabrupoFamiliarCapitalRequerido tbody tr:eq(' + i + ') input').val();
				}
			}
		}
		var moneda = {
			Id: $("#Moneda").val(),
			Nombre: $("#Moneda option:selected").text()
		}

		var tipoRenta = {
			Id: $("#TipoRenta").val(),
			Nombre: $("#TipoRenta option:selected").text()
		}

		var temporalidad = {
			Id: $("#Temporalidad").val(),
			Nombre: $("#Temporalidad option:selected").text()
		}

		var cuspp = "";// CUSPP
		if ($("#BusAfiCUSPP").val() != "") {
			cuspp = $("#BusAfiCUSPP").val();
		} else {
			cuspp = $("#CUSPP").html();
		}
		

		var params = {
			tokenUsuario: $('#TokenUsuario').val(),
			num_cuissp: cuspp,
			tipoRenta: tipoRenta,
			moneda: moneda,
			periodo_garantizado: $("#PeriodoGarantizado").val(),
			temporalidad: temporalidad,
			id_grupo_familiar: idGrupoFamiliar,
			pension_requerida: $("#PensionRequerida").val(),
		}


		

		$.ajax({
			type: 'POST',
			url: '../Simuladores/CapitalRequerido.aspx/CalcularCapital',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {

				$('#ModalCotizando').dialog('close');

				
				if (data.d.Estado == 'OK') {
					//Obteniendo la cantidad de registros
					registros = parseInt(data.d.Mensaje);
					CargarTablaCalculoRequerido();
					$('#TabrupoFamiliarCapitalRequerido input[type=checkbox]').attr('disabled', 'true');
				}
				else if (data.d.Estado == 'TOKEN') {
					CerrarSesionExpirada();
				}
				else if (data.d.Estado == 'VALIDACION') {
					$('#MCMIcono').attr('class', data.d.Icono);
					$('#MCMContenedor').html(data.d.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
					$('#ModalCuadroMensaje').dialog('open');
				}
				else if (data.d.Estado == 'ERROR') {
					$('#MCMIcono').attr('class', data.d.Icono);
					$('#MCMContenedor').html(data.d.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
					$('#ModalCuadroMensaje').dialog('open');
				}
				//CapitalRequerido = data.d;	  
				
				
				// Actualizar temporizador
				selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
				$clock.countdown(selectedDate.toString());


			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					document.location.reload(true);
				}
				else {
					$('#ModalCotizando').dialog('close');
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar el cálculo de capital requerido.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				}
			}
		});


	}


	/* Botón Calcular */
	$('#Calcular').live('click', function () {

		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		$("#TipoRenta").removeClass("formComboboxErrorContenedor");
		$("#Moneda").removeClass("formComboboxErrorContenedor");
		$("#PeriodoGarantizado").removeClass("formComboboxErrorContenedor");
		$("#PensionRequerida").removeClass("formTextboxError");
		$("#Temporalidad").removeClass("formComboboxErrorContenedor");

		$("#ConTipoRenta").removeClass("formComboboxErrorContenedor");
		$("#ConMoneda").removeClass("formComboboxErrorContenedor");
		$("#ConPeriodoGarantizado").removeClass("formComboboxErrorContenedor");
		$("#ConPensionRequerida").removeClass("formTextboxError");
		$("#ConTemporalidad").removeClass("formComboboxErrorContenedor");


		//CANTIDAD DE REGISTROS
		var bregistros = true;
		var maxRegistros = parseInt($("#HMaxRegCalculoCapital").val());
		if (registros >=maxRegistros) {
			errores.push("Cantidad máxima de registros <strong>" + maxRegistros + "</strong> permitidos.");
			bregistros = false;
		}

		// TIPORENTA
		var btiporenta = true;
		if ($.trim($("#TipoRenta").val()) == "0") {
			errores.push("Seleccione el campo <strong>Tipo Renta</strong>. Dato Obligatorio.");
			btiporenta = false;
		}

		// MONEDA
		var bmoneda = true;
		if ($.trim($("#Moneda").val()) == "-") {
			errores.push("Seleccione el campo <strong>Moneda</strong>. Dato Obligatorio.");
			bmoneda = false;
		}

		// PERIODOGARANTIZADO
		var bperiodogarantizado = true;
		if ($.trim($("#PeriodoGarantizado").val()) == "-") {
			errores.push("Seleccione el campo <strong>Periodo Garantizado</strong>. Dato Obligatorio.");
			bperiodogarantizado = false;
		}

		// PENSIONREQUERIDA
		var bpensionrequerida = true;
		if ($.trim($("#PensionRequerida").val()).length == 0  ) {
			errores.push("Ingrese el campo <strong>Pensión Requerida</strong>. Dato Obligatorio.");
			bpensionrequerida = false;
		}


		// TEMPORALIDAD
		var btemporalidad = true;
		if ($.trim($("#TipoRenta").val()) == "RPV") {
			if ($.trim($("#Temporalidad").val()) == "0") {
				errores.push("Seleccione el campo <strong>Temporalidad</strong>. Dato Obligatorio.");
				btemporalidad = false;
			}
		}
		


		if (!btiporenta) $("#ConTipoRenta").addClass("formComboboxErrorContenedor");
		if (!bmoneda) $("#ConMoneda").addClass("formComboboxErrorContenedor");
		if (!bperiodogarantizado) $("#ConPeriodoGarantizado").addClass("formComboboxErrorContenedor");
		if (!bpensionrequerida) $("#PensionRequerida").addClass("formTextboxError");
		if (!btemporalidad) $("#ConTemporalidad").addClass("formComboboxErrorContenedor");


		esCorrecto = bregistros & btiporenta & bmoneda & bperiodogarantizado & bpensionrequerida & btemporalidad;

		if (esCorrecto) {
			CalcularCapital();
		} else {
			$('#MCMIcono').attr('class', 'validacion');
			$('#MCMContenedor').html(formatearError(errores));
			$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
			$('#ModalCuadroMensaje').dialog('open');
			return false;
		}
	});


	/* Cargando tabla de Calculo Requerido */
	function CargarTablaCalculoRequerido() {

		$.ajax({
			type: 'POST',
			url: '../Simuladores/CapitalRequerido.aspx/CargarTablaCapitalRequerido',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			//data: $.toJSON(params),
			success: function (data) {
				if (data.d != 'TOKEN') {
					$('#TablaCapitalRequeridoCargando').hide();
					$('#TablaCapitalRequeridoContenedor').show();
					$('#TablaCapitalRequeridoContenedor').html($(data.d).find('#ContenidoDinamico').html());

					if (registros == 0) {
						$("#divImprimir").hide();
						$("#divNota").hide();
					} else {
						TipoCambio();
						$("#divImprimir").show();
						$("#divNota").show();
					}
					
				}
				else {
					CerrarSesionExpirada();
				}
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}
				else {
					$('#ModalCotizando').dialog('close');
					$('#TablaCapitalRequeridoCargando').hide();
					$('#TablaCapitalRequeridoError').show();
				}
			}
		});
	}


	CargarTablaCalculoRequerido();

	/* Botón Imprimir */
	$('#btnImprimir').live('click', function () {
		if (permisoImprimir) {
			$.ajax({
				type: 'POST',
				url: 'CapitalRequerido.aspx/Imprimir',
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				//data: $.toJSON(params),
				success: function (data) {
					if (data.d.Estado == "OK") {
						if (/Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent)) {
							window.location.href = "../Reportes/CalculoCapital.aspx";
						}
						else {
							var w = 800;
							var h = 600;
							var left = (screen.width / 2) - (w / 2);
							var top = (screen.height / 2) - (h / 2);
							var nuevaVentana = window.open("../Reportes/CalculoCapital.aspx", "", 'toolbar=no, location=no, directories=no, status=no, menubar=no, scrollbars=no, resizable=1, copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
						
						}
						selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
						$clock.countdown(selectedDate.toString());
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
		}

	})

	/* Botón Enviar Email */
	$('#btnEnviarEmail').live('click', function () {
		if (permisoEnviarCorreo) {
			if ($.trim($('#CorreoElectronicoRegistrado').val()).length > 0) {
				$('#ModEnvCorCargandoCapital').show();

				var tipoCotizacion = "cálculo de capital requerido";

				var params = {
					tokenUsuario: $('#TokenUsuario').val(),
					tipoCotizacion: tipoCotizacion,
					nombre: $.trim($('#HNombres').val()),
					apellidoPaterno: $.trim($('#HApellidoPaterno').val()),
					apellidoMaterno: $.trim($('#HApellidoMaterno').val()),
					sexo: $('#HSexo').val()
				}

				LimpiarFormularioCorreo();

				$.ajax({
					type: 'POST',
					url: 'CapitalRequerido.aspx/CrearDatosCorreo',
					contentType: "application/json; charset=iso-8859-1",
					dataType: 'json',
					data: $.toJSON(params),
					success: function (data) {
						/* Correo Creado */
						Correo = data.d;
						if (Correo.Respuesta.Estado == 'OK') {
							$('#ModEnvCorDeCapital').val(Correo.De);
							Correo.Para = $('#CorreoElectronicoRegistrado').val();
							$('#ModEnvCorParaCapital').val(Correo.Para);
							$('#ModEnvCorAsuntoCapital').val(Correo.Asunto);
							$('#ModEnvCorAdjuntoCapital').html(Correo.Adjunto);
							$('#ModEnvCorMensajeCapital').val(Correo.Mensaje);

							$('#ModEnvCorCargandoCapital').fadeOut();
							$('#ModEnvCorMensajeCapital').focus();
						}
						else if (Correo.Respuesta.Estado == 'TOKEN') {
							CerrarSesionExpirada();
						}
						else {
							$('#MCMIcono').attr('class', Correo.Respuesta.Icono);
							$('#MCMContenedor').html(Correo.Respuesta.Mensaje);
							$('#ModalCuadroMensaje').dialog({ title: Correo.Respuesta.Titulo });
							$('#ModalCuadroMensaje').dialog('open');

							$('#ModalEnvioCorreoCapital').dialog('close');
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
						$('#ModalEnvioCorreoCapital').dialog('close');
					}
				});

				$('#ModalEnvioCorreoCapital').dialog('open');
			}
			else {
				$('#MCMIcono').attr('class', 'validacion');
				$('#MCMContenedor').html('No se tiene registrado un Correo Electrónico para el afiliado. Por favor ingrese uno en la pestaña <strong>Datos del Afiliado</strong> y luego presione el botón <strong>Guardar</strong>.');
				$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
				$('#ModalCuadroMensaje').dialog('open');
			}

		}
	});

	function LimpiarFormularioCorreo() {
		$('#ModEnvCorDe').val('');
		$('#ModEnvCorPara').val('');
		$('#ModEnvCorAsunto').val('');
		$('#ModEnvCorMensaje').val('');
	}

	/* Botón Enviar Correo Electrónico */
	$('#ModEnvCorEnviarCapital').live('click', function () {
		$('#ModEnvCorCargandoCapital').fadeIn();

		Correo.Mensaje = $('#ModEnvCorMensajeCapital').val();
		Correo.Respuesta = null;

		var params = {
			tokenUsuario: $('#TokenUsuario').val(),
			correo: Correo
		}
		$.ajax({
			type: 'POST',
			url: 'CapitalRequerido.aspx/EnviarCorreoElectronico',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			data: $.toJSON(params),
			success: function (data) {
				if (data.d.Estado == 'OK') {
					$('#ModalEnvioCorreoCapital').dialog('close');

					$('#MCMIcono').attr('class', data.d.Icono);
					$('#MCMContenedor').html(data.d.Mensaje);
					$('#ModalCuadroMensaje').dialog({ title: data.d.Titulo });
					$('#ModalCuadroMensaje').dialog('open');

					/*<SRIINI17003>*/
					selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
					$clock.countdown(selectedDate.toString());
					/*<SRIFIN17003>*/
				}
				else if (data.d.Estado == 'TOKEN') {
					CerrarSesionExpirada();
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
					document.location.reload(true);
				}
				else {
					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al enviar el correo electrónico.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');

					$('#ModEnvCorCargandoCapital').fadeOut();
				}
			}
		});
	});

	/* Botón Cancelar Correo Electrónico */
	$('#ModEnvCorCancelarCapital').live('click', function () {
		$('#ModalEnvioCorreoCapital').dialog('close');
		//<SRI.INI-20322>
		$('#ModalEnvioCorreoSimulador').dialog('close');
		//<SRI.FIN-20322>
	});

	/* Cargando Tipo Cambio */
	function TipoCambio() {

		$.ajax({
			type: 'POST',
			url: '../Simuladores/CapitalRequerido.aspx/TipoCambio',
			contentType: "application/json; charset=iso-8859-1",
			dataType: 'json',
			success: function (data) {
				if (data.d != 'TOKEN') {
					$('#LabNota2').html("* Tipo de cambio referencial: " + data.d);
				}
				else {
					CerrarSesionExpirada();
				}
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}
				else {
					$('#ModalCotizando').dialog('close');
					$('#TablaCapitalRequeridoCargando').hide();
					$('#TablaCapitalRequeridoError').show();
				}
			}
		});
	}
		

	$("#TipoRenta").val("-");
	$('#TexTipoRenta').html($('#TipoRenta').find(':selected').text());

	$("#divTemporalidad").hide();
	//<SOLFIN25621>


	//<INIGTI_2145>

	function CalculoPlazoFijo() {
		//=(1+C18)^(1/12)-1 //Excel
		//=(a)^(b)-1
		//a=1+C18
		//b=1/12

		//Calculando el TEM
		var tea = $("#TEA").val();
		if (tea == "") {
			tea = 0;
		}
		tea = parseFloat(tea);

		var a = 1 + (tea / 100);
		var b = 1 / 12;
		var tem = (Math.pow(a, b) - 1) * 100;
		$("#TEM").val(tem.toFixed(2).toString());

		//Calculado el Interes Deposito a Plazo
		var depositoPlazo = $("#DepositoPlazo").val();
		depositoPlazo = parseFloat(depositoPlazo.replace(",","").replace(",","").replace(",","") );
		var interesDeposito = (depositoPlazo * (tem / 100)).toFixed(2);
		//$("#InteresDeposito").val(interesDeposito.toString());
		$("#InteresDeposito").val(formatNumber.new(interesDeposito.toString()));

		
		//$("#InteresDeposito").focus();
		//$("#TEA").focus();

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

	//$("#TEA").keypress(function () {
		
	//    CalculoPlazoFijo();
	//    //alert(tem);
		
	//});

	//$("#TEA").keydown(function () {

	//    CalculoPlazoFijo();
	//    //alert(tem);

	//});

	$("#TEA").keyup(function () {

		CalculoPlazoFijo();
		//alert(tem);

	});

	$("#DepositoPlazo").keyup(function () {

		CalculoPlazoFijo();
		//alert(tem);

	});


	var formatNumber = {
		separador: ",", // separador para los miles
		sepDecimal: '.', // separador para los decimales
		formatear: function (num) {
			num += '';
			var splitStr = num.split('.');
			var splitLeft = splitStr[0];
			var splitRight = splitStr.length > 1 ? this.sepDecimal + splitStr[1] : '';
			var regx = /(\d+)(\d{3})/;
			while (regx.test(splitLeft)) {
				splitLeft = splitLeft.replace(regx, '$1' + this.separador + '$2');
			}
			return this.simbol + splitLeft + splitRight;
		},
		new: function (num, simbol) {
			this.simbol = simbol || '';
			return this.formatear(num);
		}
	}

	//<FINGTI_2145>

	//<INI.GTI_7012_2_1>
	/* Botón Buscar Afiliado Reporte Cotizacion Plus */
	$('#RepCotPlusBusAfiBuscar').live('click', function () {
		if (!botonBusAfiBuscarBloqueado && permisoBusAfiBuscar) {
			$('#GrupoCotizaciones').hide();
			$('#DatosSimulacion').hide();

			var esCorrecto = true;
			var errores = new Array();

			$('#BusAfiNroSolicitud').removeClass('formTextboxError');
			$('#BusAfiCUSPP').removeClass('formTextboxError');

			var vsolicitud = ($.trim($('#BusAfiNroSolicitud').val()).length > 0) ? true : false;
			var cuspp = ($.trim($('#BusAfiCUSPP').val()).length > 0) ? true : false;

			if (!(vsolicitud | cuspp)) {
				errores.push('Debe ingresar un criterio de búsqueda.');
				$('#BusAfiNroSolicitud').addClass('formTextboxError');
				$('#BusAfiCUSPP').addClass('formTextboxError');
				esCorrecto = false;
			}

			if (vsolicitud & cuspp) {
				errores.push('Sólo debe ingresar un criterio de búsqueda.');
				$('#BusAfiNroSolicitud').addClass('formTextboxError');
				$('#BusAfiCUSPP').addClass('formTextboxError');
				esCorrecto = false;
			}

			if (cuspp && $.trim($('#BusAfiCUSPP').val()).length != 12) {
				errores.push('El <strong>CUSPP</strong> debe contener 12 caracteres.');
				$('#BusAfiCUSPP').addClass('formTextboxError');
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
			$('#SimBusAfiBuscar').attr('class', 'botonDeshabilitado gris gris_sharp');

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				nroSolicitud: $.trim($('#BusAfiNroSolicitud').val()),
				cuspp: $.trim($('#BusAfiCUSPP').val())
			}
			$.ajax({
				type: 'POST',
				url: rutaObtenerDatosAfiliado,
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					Afiliado = data.d;
					if (Afiliado.Respuesta.Estado == 'OK') {
						$('#CUSPP').val(Afiliado.CUSPP);
						$('#DSCuspp').html(Afiliado.CUSPP);
						$('#DSAfiliado').html(Afiliado.Nombre + ' ' + Afiliado.ApellidoPaterno + ' ' + Afiliado.ApellidoMaterno);
						$('#DSAgente').html($('#CabNombre').html());
						
						consentimientoAfiliado = Afiliado.Consentimiento;

						CargarTablaSolicitudesReportePlus();

						selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
						$clock.countdown(selectedDate.toString());
						
					}
					else if (Afiliado.Respuesta.Estado == 'ERROR') {
						$('#MCMIcono').attr('class', Afiliado.Respuesta.Icono);
						$('#MCMContenedor').html(Afiliado.Respuesta.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: Afiliado.Respuesta.Titulo });
						$('#ModalCuadroMensaje').dialog('open');
					}
					else if (Afiliado.Respuesta.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						/* Sesión caducada */
						document.location.reload(true);
					}

					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información del afiliado.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				},
				complete: function () {
					botonBusAfiBuscarBloqueado = false;
					$('#RepCotPlusBusAfiBuscar').attr('class', 'boton darkblue sharp');
				}
			});
		}
		else {
			return false;
		}
	});


	/* Cargar datos desde sesión */
	if ($('#RepCotPlusBusAfiDatosCargados').val() == '1') {
		if (!botonBusAfiBuscarBloqueado && permisoBusAfiBuscar) {
			var esCorrecto = true;
			var errores = new Array();

			if ($('#FormularioBusqueda').length > 0) {
				$('#BusAfiNroSolicitud').removeClass('formTextboxError');
				$('#BusAfiCUSPP').removeClass('formTextboxError');

				var vsolicitud = ($.trim($('#BusAfiNroSolicitud').val()).length > 0) ? true : false;
				var cuspp = ($.trim($('#BusAfiCUSPP').val()).length > 0) ? true : false;

				if (!(vsolicitud | cuspp)) {
					errores.push('Debe ingresar un criterio de búsqueda.');
					$('#BusAfiNroSolicitud').addClass('formTextboxError');
					$('#BusAfiCUSPP').addClass('formTextboxError');
					esCorrecto = false;
				}

				if (vsolicitud & cuspp) {
					errores.push('Sólo debe ingresar un criterio de búsqueda.');
					$('#BusAfiNroSolicitud').addClass('formTextboxError');
					$('#BusAfiCUSPP').addClass('formTextboxError');
					esCorrecto = false;
				}

				if (cuspp && $.trim($('#BusAfiCUSPP').val()).length != 12) {
					errores.push('El <strong>CUSPP</strong> debe contener 12 caracteres.');
					$('#BusAfiCUSPP').addClass('formTextboxError');
					esCorrecto = false;
				}

				if (!esCorrecto) {
					$('#MCMIcono').attr('class', 'validacion');
					$('#MCMContenedor').html(formatearError(errores));
					$('#ModalCuadroMensaje').dialog({ title: 'Validación' });
					$('#ModalCuadroMensaje').dialog('open');
					return false;
				}
			}

			botonBusAfiBuscarBloqueado = true;
			$('#SimBusAfiBuscar').attr('class', 'botonDeshabilitado gris gris_sharp');

			var params = {
				tokenUsuario: $('#TokenUsuario').val(),
				nroSolicitud: $.trim($('#HBusAfiNroSolicitud').val()),
				cuspp: $.trim($('#HBusAfiCUSPP').val())
			}
			$.ajax({
				type: 'POST',
				url: rutaObtenerDatosAfiliado,
				contentType: "application/json; charset=iso-8859-1",
				dataType: 'json',
				data: $.toJSON(params),
				success: function (data) {
					Afiliado = data.d;
					if (Afiliado.Respuesta.Estado == 'OK') {
						$('#CUSPP').val(Afiliado.CUSPP);

						$('#DSCuspp').html(Afiliado.CUSPP);
						$('#DSAfiliado').html(Afiliado.Nombre + ' ' + Afiliado.ApellidoPaterno + ' ' + Afiliado.ApellidoMaterno);
						$('#DSAgente').html($('#CabNombre').html());
						
						consentimientoAfiliado = Afiliado.Consentimiento;
						
						CargarTablaSolicitudesReportePlus();

						selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
						$clock.countdown(selectedDate.toString());
						
					}
					else if (Afiliado.Respuesta.Estado == 'ERROR') {
						$('#MCMIcono').attr('class', Afiliado.Respuesta.Icono);
						$('#MCMContenedor').html(Afiliado.Respuesta.Mensaje);
						$('#ModalCuadroMensaje').dialog({ title: Afiliado.Respuesta.Titulo });
						$('#ModalCuadroMensaje').dialog('open');
					}
					else if (Afiliado.Respuesta.Estado == 'TOKEN') {
						CerrarSesionExpirada();
					}
				},
				error: function (XMLHttpRequest, textStatus, errorThrown) {
					if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
						/* Sesión caducada */
						document.location.reload(true);
					}

					$('#MCMIcono').attr('class', 'error');
					$('#MCMContenedor').html('Ha ocurrido un error al cargar la información del afiliado.');
					$('#ModalCuadroMensaje').dialog({ title: 'Error' });
					$('#ModalCuadroMensaje').dialog('open');
				},
				complete: function () {
					botonBusAfiBuscarBloqueado = false;
					$('#SimBusAfiBuscar').attr('class', 'boton darkblue sharp');
				}
			});
		}
		else {
			return false;
		}
	}


	
	function GenerarReporteCotizacionPlus() {

		// Validaciones
		var esCorrecto = true;
		var errores = new Array();

		var solicitudes = new Array();
		var cotizaciones = new Array();
	   

		$("#TabSolicitudesReportePlus tbody tr input.ChkSolicitudesReportePlus[type=checkbox]:checked").each(function (index, val) {
			solicitudes.push($(this).data("solicitud"));
		});
		$(".TabCotizacionesReportePlus tbody tr input.ChkCotizacionesReportePlus:checkbox:checked").each(function (index, val) {
			cotizaciones.push($(val).val());
		});

		var params = {
			tokenUsuario: $('#TokenUsuario').val(),
			cuspp: $('#CUSPP').val(),
			num_maximo: 5,
			solicitudes: solicitudes,
			cotizaciones: cotizaciones
		}


		$.ajax({
			type: "POST",
			url: "CompararCotizacionPlus.aspx/GenerarReporteCotizacionPlus",
			contentType: "application/json; charset=iso-8859-1",
			data: $.toJSON(params),
			dataType: "json",
			success: function (data) {
				//
				if (data.d.Estado == "OK") {
					location.replace(rutaDescargaExcel);
				}
				else if (data.d.Estado == "ERROR") {
					$("#MCMIcono").attr("class", data.d.Icono);
					$("#MCMContenedor").html(data.d.Mensaje);
					$("#ModalCuadroMensaje").dialog({ title: data.d.Titulo });
					$("#ModalCuadroMensaje").dialog("open");
				}
				else if (data.d.Estado == "TOKEN") {
					CerrarSesionExpirada();
				}
				selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
				$clock.countdown(selectedDate.toString());
			},
			error: function (XMLHttpRequest, textStatus, errorThrown) {
				
				if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
					/* Sesión caducada */
					document.location.reload(true);
				}

				$('#MCMIcono').attr('class', 'error');
				$('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
				$('#ModalCuadroMensaje').dialog({ title: 'Error' });
				$('#ModalCuadroMensaje').dialog('open');

				$("#btnEnviarExcel").attr('class', 'boton darkblue sharp');
			},
			complete: function () {
				$("#ModalCotizando").dialog("close");
				$("#btnEnviarExcel").attr('class', 'boton darkblue sharp');
			}
		});


	}

	$('#btnEnviarExcel').live('click', function () {

		$('#MCIcono').attr('class', 'cargando');
		$('#MCContenedor').html('Generando excel, por favor espere un momento...');
		$('#ModalCotizando').dialog({ title: 'Generando Excel' });
		$('#ModalCotizando').dialog('open');


		var Max = $("input.ChkCotizacionesReportePlus[type=checkbox]:checked").length;

		if (Max > $("#ModSolMaximo_RP").val()) {
			$("#ModalCotizando").dialog("close");
			$("#MCMIcono").attr("class", "validacion");
			$("#MCMContenedor").html("Solo se puede seleccionar [" + $("#ModSolMaximo_RP").val() + "] cotizaciones");
			$("#ModalCuadroMensaje").dialog({ title: "Validación" });
			$("#ModalCuadroMensaje").dialog("open");

			return;
		}

		if (Max == 0) {
			$("#ModalCotizando").dialog("close");
			$("#MCMIcono").attr("class", "validacion");
			$("#MCMContenedor").html("Seleccione al menos una cotización");
			$("#ModalCuadroMensaje").dialog({ title: "Validación" });
			$("#ModalCuadroMensaje").dialog("open");
			return;
		}

		GenerarReporteCotizacionPlus();
	})

	//<FIN.GTI_7012_2_1>

});