document.addEventListener("DOMContentLoaded", async function () {
  await SeleccionSolicitud.init();
});

const SeleccionSolicitud = {
  init: async function () {
    try {
      const origen = UtilitariosManager.ObtenerQueryParams('origen');
      if (origen === 'cotizador') {
        UtilitariosManager.ValidarPermiso(EnumsPermisos.CotizacionesIFP)
        const qs_source = UtilitariosManager.ObtenerQueryParams('source');
        const qs_solicitud = UtilitariosManager.ObtenerQueryParams('solicitud');
        let modSolModo = UtilitariosManager.ObtenerQueryParams('modSolModo');
        const idSolicitud = UtilitariosManager.ObtenerQueryParams('idSolicitud');
        const fechaSolicitud = UtilitariosManager.ObtenerQueryParams('fechaSolicitud');
        const seleccione = UtilitariosManager.ObtenerQueryParams('seleccione');
        console.log('qs_source', qs_source);
        console.log('qs_solicitud', qs_solicitud);
        console.log('modSolModo', modSolModo);
        console.log('idSolicitud', idSolicitud);

        const session = UtilitariosManager.ObtenerSession();

        if (qs_source == "correo") {
          console.log("FUNCIONALIDAD NO IMPLEMENTADA");
        }
        const solicitud = await ApiCotizadorIFP.ObtenerSolicitudPorId(idSolicitud)

        Solicitud = solicitud;

        await SeleccionSolicitud.CargarInformacionInicialPantalla(solicitud.Afiliado);

        console.log({ solicitud });

        if (modSolModo) {
          document.getElementById('HCUSPP_RP').value = solicitud.Afiliado.CUSPP;
          if (modSolModo == 'CERRAR') {
            solicitud.Beneficiarios = solicitud.Beneficiarios.filter(ben =>
              ben.IdTipoPeriodoBeneficiario.toString() === EnumTipoPeriodoBeneficiario.Garantizada ||
              ben.Parentesco.Id === EnumParentesco.Afiliado
            );
            document.getElementById('HSolicitudSerializado').value = JSON.stringify(solicitud);

            document.getElementById("HNroSolicitud").value =
              solicitud.Id;

            document.getElementById("ModSolNroSolicitud_IFP").textContent =
              solicitud.Id;

            document.getElementById("ModSolPrimaUnica_IFP").textContent =
              formatearMonto(solicitud.PrimaUnica.toFixed(2));

            // Select de moneda
            const selectMoneda = document.getElementById("ModSolMonedaPrimaUnica_IFP");
            selectMoneda.value = solicitud.MonedaPrimaUnica.Id;
            console.log('selectMoneda', selectMoneda);
            // Mostrar el texto de la opción seleccionada
            document.getElementById("ModSolMonedaPrimaUnicaText_IFP").textContent =
              selectMoneda.options[selectMoneda.selectedIndex].text;

            // Fechas
            document.getElementById("ModSolFechaCotizacion_IFP").textContent =
              new Date(solicitud.FechaCotizacion).toString('dd/MM/yyyy');

            document.getElementById("ModSolFechaDevengue_IFP").textContent =
              new Date(solicitud.FechaDevengue).toString('dd/MM/yyyy');

            document.getElementById("ModSolFechaVigencia_IFP").textContent =
              new Date(solicitud.FechaVigencia).toString('dd/MM/yyyy');

            /* Session["RP_Beneficiarios"] = solicitud.Beneficiarios;
            Session["RP_Cotizaciones"] = solicitud.Cotizaciones;
            Session["RP_CoberturasAdicionales"] = solicitud.CoberturasAdicionales; */

            const pep = solicitud.Beneficiarios
              .find(b => b.Parentesco.Id === EnumParentesco.Afiliado)
              ?.ind_PEP;

            document.getElementById('HPEP').value = pep ? "S" : "N";

            const cotizacionSeleccionada = solicitud.Cotizaciones
              .find(c => (c.EstadoCotizacion == "04" || c.EstadoCotizacion == "05") && c.IndSeleccionada == "S");
            console.log({ cotizacionSeleccionada });
            if (cotizacionSeleccionada) {
              console.log('cotizacionSeleccionada')
              document.getElementById('HSeleccionada').value = "S";
              document.getElementById('LabModSolAviso').style.display = 'block';
            }

            console.log(document.getElementById('HSeleccionada').value)

            document.getElementById('ModSolTipoCambio_IFP').textContent = solicitud.TipoCambio;
            document.getElementById('ModSolTipoCambioPanel').style.display = 'none';

            if (solicitud.MonedaPrimaUnica.Id.toString() === "002") {
              document.getElementById('ModSolTipoCambioPanel').style.display = 'block';
            }

            //Bloqueando controles
            document.getElementById('ModSolMonedaPrimaUnica_IFP').disabled = true;
            document.getElementById('ModSolFechaCotizacion_IFP').disabled = true;
            document.getElementById('ModSolFechaDevengue_IFP').disabled = true;
            document.getElementById('ModSolFechaVigencia_IFP').disabled = true;

            document.getElementById('HEstado').value = solicitud.CodigoEstado;
            document.getElementById('HEstadoPlaft').value = solicitud.CodigoEstadoPlaft;

            document.getElementById('LabMensaje').textContent = solicitud.EstadoSolicitud;
            console.log(document.getElementById('LabMensaje'))
            let firmado = false;
            const usuario = session.Matricula
            const firma = await ApiCotizadorIFP.ObtenerFirmaDigital(idSolicitud, 1, usuario);
            console.log({ firma });
            if (firma) {
              document.getElementById('HFirmado').value = firma.ind_consentimiento;
              firmado = firma.ind_consentimiento === "S";
              if (firma.ind_consentimiento === "S") {
                document.getElementById('ModSolAceptarCierre_RP').style.display = "none";
              }
            }

            const modSolAgregarArchivos = document.getElementById('ModSolAgregarArchivos');
            modSolAgregarArchivos.style.display = "none";

            const corregirDocumentos = document.getElementById('CorregirDocumentos')
            corregirDocumentos.style.display = "none";

            console.log(solicitud.CodigoEstado)
            switch (solicitud.CodigoEstado.toString()) {
              case "0":
                document.getElementById('LabModSolAviso').className = "grilla_info";
                $('#ModImprimirDocumentosIFP').hide();
                break;
              case "1":
                document.getElementById('LabModSolAviso').className = "grilla_info_verde";
                document.getElementById('LabMensaje').style.color = "green";
                document.getElementById('LabModSolAviso').style.display = "block";
                modSolAgregarArchivos.style.display = firmado ? "" : "none";
                if (seleccione) {
                  if (seleccione === "0") {
                    modSolAgregarArchivos.style.display = "none";
                  }
                }
                break;
              case "2":
                document.getElementById('LabModSolAviso').className = "grilla_info";
                document.getElementById('LabModSolAviso').style.display = "block";
                break;
              case "3":
                document.getElementById('LabModSolAviso').className = "grilla_info_rojo";
                document.getElementById('LabMensaje').style.color = "red";
                document.getElementById('LabModSolAviso').style.display = "block";
                break;
              case "4":
                document.getElementById('LabModSolAviso').className = "grilla_info_verde";
                document.getElementById('LabMensaje').style.color = "green";
                document.getElementById('LabModSolAviso').style.display = "block";
                if (solicitud.CodigoEstadoPlaft.toString() === EnumEstadoPlaft.Observado) {
                  document.getElementById('LabMensaje').textContent = "Solicitud Observada";
                }
                break;
              case "5":
                document.getElementById('LabModSolAviso').className = "grilla_info";
                document.getElementById('LabModSolAviso').style.display = "block";
                if (firma && firma.ind_consentimiento === "S") {
                  document.getElementById('ModSolEnviarEvaluacion').style.display = "";
                }
                if (UtilitariosManager.esRolComercial(session.RolAzman) && firma && firma.ind_consentimiento !== "R") {
                  corregirDocumentos.style.display = "";
                }
                modSolAgregarArchivos.style.display = firmado ? "" : "none";
                if (solicitud.CodigoEstadoPlaft.toString() === EnumEstadoPlaft.Evaluacion) {
                  document.getElementById('LabMensaje').textContent = "Solicitud en Evaluación";
                }
                break;
              case "6":
                document.getElementById('LabModSolAviso').className = "grilla_info";
                document.getElementById('LabModSolAviso').style.display = "block";
                if (solicitud.CodigoEstadoPlaft.toString() === EnumEstadoPlaft.Observado) {
                  document.getElementById('LabMensaje').textContent = "Solicitud Observada";
                }
                if (solicitud.CodigoEstadoPlaft.toString() === EnumEstadoPlaft.Evaluacion) {
                  document.getElementById('LabMensaje').textContent = "Solicitud en Evaluación";
                }
                break;
              case "7":
                document.getElementById('LabModSolAviso').className = "grilla_info_rojo";
                document.getElementById('LabMensaje').style.color = "red";
                document.getElementById('LabModSolAviso').style.display = "block";
                break;
              default:
                document.getElementById('LabModSolAviso').className = "grilla_info";
                modSolAgregarArchivos.style.display = firmado ? "" : "none";
                break;
            }

            document.getElementById('ModSolEnviarEvaluacion').style.display = "";
            document.getElementById('ModSolEnviarEvaluacion').disabled = false;
            document.getElementById('ModSolEnviarEvaluacion').className = "botonDeshabilitado gris gris_sharp";

            if (solicitud.CodigoEstado.toString() === "5" && (
              solicitud.CodigoEstadoPlaft.toString() === EnumEstadoPlaft.Observado ||
              solicitud.CodigoEstadoPlaft.toString() === EnumEstadoPlaft.Aprobado
            )) {
              document.getElementById('ModSolEnviarEvaluacion').disabled = true;
              document.getElementById('ModSolEnviarEvaluacion').className = "botonDeshabilitado gris gris_sharp";
            }
            else if (solicitud.CodigoEstadoPlaft.toString() === EnumEstadoPlaft.Observado && (
              solicitud.CodigoEstado.toString() === "5" ||
              solicitud.CodigoEstado.toString() === "6"
            )) {
              document.getElementById('ModSolEnviarEvaluacion').disabled = true;
              document.getElementById('ModSolEnviarEvaluacion').className = "botonDeshabilitado gris gris_sharp";
            }
            else if (solicitud.CodigoEstadoPlaft.toString() === "-1" &&
              solicitud.CodigoEstado.toString() === "6"
            ) {
              document.getElementById('ModSolEnviarEvaluacion').style.display = "none";
              document.getElementById('ModSolEnviarEvaluacion').disabled = true;
              document.getElementById('ModSolEnviarEvaluacion').className = "botonDeshabilitado gris gris_sharp";
            }
          }
          document.getElementById('ModSolModo').value = modSolModo;
          document.getElementById('HCopia').value = "";

          if (modSolModo === "C") {
            document.getElementById('HCopia').value = "C";
          }

          document.getElementById('ManSolTipoSolicitud_RP').value = "EXTRAOFICIAL";
          document.getElementById('HBloqueo').value = "TRUE";

          if (session.RolAzman === "JEF.RVI.OPE") {
            document.getElementById('HBloqueo').value = "FALSE";
          }

          //Validando la fecha de cotizacion
          document.getElementById('ModSolFechaCotizacion_IFP').disabled = true;

          if (modSolModo === 'CERRAR') {
            SeleccionSolicitud.CargandoSolicitud(solicitud);
          }
        }
        else {
          window.location.href = "Cotizador.aspx";
        }
        
      } else {
        if (document.getElementById('ModSolPrimaUnica_IFP').value != '') {
          document.getElementById('ModSolPrimaUnica_IFP').value = formatearMonto(document.getElementById('ModSolPrimaUnica_IFP').value);
        }
      }
      
    } catch (error) {
      console.log("Ocurrio un error al cargar la información de la solicitud", error);
      /* $('#MCMMensaje').text("Ocurrió un error al cargar la información de la solicitud: " + error.message);
      $('#MCMEstadoIcono').val(EnumCuadroMensajeIcono.Error.StringValue());
      $('#MCMEstadoTitulo').val(EnumCuadroMensajeTitulo.Error.StringValue());
      $('#MCMEstado').val("1"); */
    }
  },
  CargarInformacionInicialPantalla: async function (afiliado) {

    const listaCombobox = await ApiParametro.ObtenerParametroSistema();
    console.log({ listaCombobox });

    const selMoneda = document.getElementById('ModSolMonedaPrimaUnica_IFP');
    SeleccionSolicitud.CargarCombobox(selMoneda, listaCombobox.MonedasPrimaUnica);

    //Session["ComboSexo"] = listaCombobox[(int)Enums.CategoriaCombobox.Sexo]; // no se encontró uso

    document.getElementById('ModSolDNI_IFP').textContent = afiliado.NumeroIdentificacion;
    document.getElementById('ModSolNombres_IFP').textContent = afiliado.Nombre.trim();
    document.getElementById('ModSolApellidos_IFP').textContent = afiliado.ApellidoPaterno.trim() + " " + afiliado.ApellidoMaterno.trim();
  },
  CargarCombobox: function (selectElement, items) {
    selectElement.innerHTML = '';

    const placeholder = new Option('«Seleccione»', '0');
    selectElement.add(placeholder);

    items.forEach(item => {
      const opt = new Option(item.Glosa, item.Id);
      selectElement.add(opt);
    });
    console.log('selectElement', selectElement);
  },
  CargandoSolicitud: function (Solicitud) {
    console.log('Solicitud', Solicitud);

    /* $('#ManSolPestanhas li:eq(0)').trigger('click'); */ // se comenta porque en la version oficial no funca

    $('#ManSolTipoSolicitud_RP').val($(this).parent('td').parent('tr').children().eq(3).find('span').html());

    botonModSolAceptarBloqueado = false;
    $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');

    if ($("#HBloqueo").val() == "TRUE") {
      if ($("#ModSolFechaCotizacion_IFP").val() != $("#HFechaActual").val()) {
        botonModSolAceptarBloqueado = true;
        $('#ModSolAceptar_RP').attr('class', 'botonDeshabilitado gris gris_sharp');

        $('#ModSolTipoCambio_IFP').attr('disabled', 'disabled');
        $('#ModSolFechaCotizacion_IFP').attr('disabled', 'disabled');
        $('#ModSolFechaDevengue_IFP').attr('disabled', 'disabled');
        $('#ModSolFechaVigencia_IFP').attr('disabled', 'disabled');
        $('#ModSolPrimaUnica_IFP').attr('disabled', 'disabled');
        $('#ConModSolMonedaPrimaUnica_IFP').attr('disabled', 'disabled');
        $('#ConModSolPlan_IFP').attr('disabled', 'disabled');

        $('#ModSolPrimaUnica_IFP').attr('class', 'formTextbox formTextboxReadOnly');
        $('#ModSolTipoCambio_IFP').attr('class', 'formTextbox formTextboxReadOnly');
        $('#ConModSolMonedaPrimaUnica_IFP').addClass('formComboboxReadOnlyContenedor');
        $('#ConModSolPlan_IFP').addClass('formComboboxReadOnlyContenedor');

        $('#ModSolFechaCotizacion_IFP').attr('class', 'fecha formTextbox formCalendar formTextboxReadOnly');
        $('#ModSolFechaDevengue_IFP').attr('class', 'fecha formTextbox formCalendar formTextboxReadOnly');
        $('#ModSolFechaVigencia_IFP').attr('class', 'fecha formTextbox formCalendar formTextboxReadOnly');
      } else {
        botonModSolAceptarBloqueado = false;
        $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
      };

    } else {
      botonModSolAceptarBloqueado = false;
      $('#ModSolAceptar_RP').attr('class', 'boton darkblue sharp');
    }

    var bPlan1 = false;
    var bPlan2 = false;
    var bPlan3 = false;
    $('#TabCotizacionesLeyenda_RP').hide();

    for (i = 0; i < Solicitud.Cotizaciones.length; i++) {
      if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN1') {
        bPlan1 = true;
      }
      if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN2') {
        bPlan2 = true;
      }
      if (Solicitud.Cotizaciones[i].Plan.Id == 'PLAN3') {
        bPlan3 = true;
      }

      if (Solicitud.Cotizaciones[i].IndCotiza == '**') {
        $('#TabCotizacionesLeyenda_RP').show();
      }
    }

    if (bPlan1) {
      if ($('#ModSolModo').val() == "CERRAR") {
        CargarTablaCotizacionesCierre_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
      } else {
        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN1');
      }
      Plan.push('PLAN1');
    }

    if (bPlan2) {
      console.log('bPlan2', bPlan2);
      if ($('#ModSolModo').val() == "CERRAR") {
        CargarTablaCotizacionesCierre_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
      } else {
        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN2');
      }
      Plan.push('PLAN2');
    }

    if (bPlan3) {
      if ($('#ModSolModo').val() == "CERRAR") {
        CargarTablaCotizacionesCierre_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
      } else {
        CargarTablaCotizaciones_RP(Solicitud, $("#ModSolMonedaPrimaUnica_IFP").val(), 'PLAN3');
        if ($('#ModSolModo').val() == "CONS") {
          $('#txtCAFecNacConyuge').attr('disabled', 'disabled');
          $('#ddlCASexoConyuge').attr('disabled', 'disabled');
          $('#txtCAFecNacPadre').attr('disabled', 'disabled');
          $('#txtCAFecNacMadre').attr('disabled', 'disabled');
        }
      }

      // 017
      $('#ManSolPes2').show();
      Plan.push('PLAN3');
    }

    if ($("#ModSolMonedaPrimaUnica_IFP").val() == "001") {
      $("#ModSolTipoCambioPanel").hide();
    } else {
      $("#ModSolTipoCambioPanel").show();
    };


  }
}
