document.addEventListener("DOMContentLoaded", function () {
  BandejaFlujoCotizacionModificar.loadData();
});

const BandejaFlujoCotizacionModificar = {
  loadData: async function () {
    const ModSolDCOMInput = document.getElementById("ModSolDCOM");
    const HGrupoEspecialesField = document.getElementById("HGrupoEspeciales");
    const GrupoEspecialesPanel = document.getElementById("GrupoEspeciales");
    const LeyendaDiv = document.getElementById("Leyenda");
    const GrupoInformeAPanel = document.getElementById("GrupoInformeA");

    const sesionUsuario = UtilitariosManager.ObtenerSession();

    UtilitariosManager.ValidarPermiso(EnumsPermisos.BandejaAprobacionOficiales);
    console.log("Se valido el permiso BandejaAprobacionOficiales");

    if (UtilitariosManager.ValidarPermisoActivo(EnumsPermisos.RechazarSolicitud)) {
      console.log("Se habilita el boton de rechazar");
      document.getElementById("PerRechazarSolicitud").value = "1";
      permisoRechazarSolicitud = true;
      document.getElementById("ModSolRechazarBandeja").setAttribute("class", "boton darkblue sharp");
    } else {
      console.log("Se deshabilita el boton de rechazar");
      document.getElementById("PerRechazarSolicitud").value = "0";
      permisoRechazarSolicitud = false;
      document.getElementById("ModSolRechazarBandeja").setAttribute("class", "botonDeshabilitado gris gris_sharp");
    }

    if (UtilitariosManager.ValidarPermisoActivo(EnumsPermisos.PermisoDCOMBandeja)) {
      console.log("Se habilita el input de DCOM");
      ModSolDCOMInput.removeAttribute("readonly");
      ModSolDCOMInput.className = "formTextbox numerico";
    } else {
      console.log("Se deshabilita el input de DCOM");
      ModSolDCOMInput.setAttribute("readonly", "readonly");
      ModSolDCOMInput.className = "formTextbox formTextboxReadOnly";
    }

    if (sesionUsuario.RolAzman === EnumsRoles.JefeOperaciones ||
      sesionUsuario.RolAzman == EnumsRoles.AsistenteOperaciones ||
      sesionUsuario.RolAzman == EnumsRoles.AnalistaOperaciones) {
      console.log("Se habilita el grupo de especiales");
      HGrupoEspecialesField.value = "TRUE";
      GrupoEspecialesPanel.style.display = "block";
      LeyendaDiv.style.display = "block";
    } else {
      console.log("Se deshabilita el grupo de especiales");
      HGrupoEspecialesField.value = "FALSE";
      GrupoEspecialesPanel.style.display = "none";
      LeyendaDiv.style.display = "none";
    }

    if (UtilitariosManager.ValidarPermisoActivo(EnumsPermisos.InformeA)) {
      console.log("Se habilita el grupo de informe A");
      GrupoInformeAPanel.style.display = "block";
    } else {
      console.log("Se deshabilita el grupo de informe A");
      GrupoInformeAPanel.style.display = "none";
    }

    const numCusspp = UtilitariosManager.ObtenerQueryParams("numCusspp");
    const numSolicitud = UtilitariosManager.ObtenerQueryParams("numSolicitud");
    const numOperacion = UtilitariosManager.ObtenerQueryParams("numOperacion");

    BandejaFlujoCotizacionModificar.LlenarDatosSolicitud(numCusspp, numSolicitud, numOperacion);
    const agrupadorCotizacion = document.getElementById("AgrupadorCotizacion");
    agrupadorCotizacion.click();

    document.addEventListener('keypress', (event) => {
      if (event.keyCode === 13) {
        event.preventDefault();
        return false;
      }
    });

    // Ejecutar scroll al cargar el módulo (si lo quieres en otro momento, llama a scrollToAgrupador() manualmente)
    BandejaFlujoCotizacionModificar.scrollToAgrupador();

  },

  scrollToAgrupador: function () {
    const target = document.getElementById('AgrupadorCotizacion');
    if (!target) return;

    // Calcula la posición absoluta del elemento
    const targetPosition = target.getBoundingClientRect().top + window.pageYOffset;

    window.scrollTo({
      top: targetPosition,
      behavior: 'smooth'
    });
  },

  ObtenerDatosSolicitudEscenario: async function (numSolicitud) {
    const solicitudEscenario = await ApiSolicitudesCambio.ObtenerSolicitudEscenario(numSolicitud);
    return solicitudEscenario;
  },

  LlenarDatosSolicitud: async function (numCusspp, numSolicitud, numOperacion) {
    document.getElementById('ModSolModo').value = 'M';
    $('#ModSolCargando').show();


    LimpiarFormularioSolicitudOficial();

    document.getElementById('HCusspp').value = numCusspp;
    document.getElementById('HNumSolicitud').value = numSolicitud;
    document.getElementById('HNumOperacion').value = numOperacion;

    const SolicitudEscenario = await BandejaFlujoCotizacionModificar.renderizarSolicitudEscenario(numSolicitud);

    const acom = document.getElementById('ModSolACOM').value;
    BandejaFlujoCotizacionModificar.renderizarValMontoAcom(numSolicitud, acom, SolicitudEscenario.NumCotizacionElegida);

    BandejaFlujoCotizacionModificar.renderizarSolicitud(numSolicitud, SolicitudEscenario.FechaPresentacion, numCusspp, SolicitudEscenario);

  },

  renderizarSolicitud: async function (numSolicitud, fechaCotizacion, numCusspp, SolicitudEscenario) {
    try {
      Solicitud = await ApiSolicitudesCambio.ObtenerDatosSolicitud(numSolicitud, fechaCotizacion);
      console.log({ Solicitud });
      if (Solicitud.Agente.IdNivel == 0) {
        document.getElementById('ModSolNumAgente').innerHTML = SolicitudEscenario.Agente.Id + ' (Sin Nivel)';
      } else {
        document.getElementById('ModSolNumAgente').innerHTML = SolicitudEscenario.Agente.Id + ' (Nivel:' + Solicitud.Agente.IdNivel + ')';
      }
      Solicitud.Beneficiarios.forEach(beneficiario => {
        beneficiario.Sexo = beneficiario.SexoChar;
      })
      CargarTablaBeneficiariosOficialBandejaMovil(Solicitud.Beneficiarios, numCusspp);

      document.getElementById('HCodTipoMovimiento').value = Solicitud.TipoMovimiento.Id;

      if (Solicitud.TipoMovimiento.Id == 7 || Solicitud.TipoMovimiento.Id == 0) {
        botonModSolAceptarBloqueado = true;
        document.getElementById('ModSolAprobarBandeja').setAttribute('class', 'botonDeshabilitado gris gris_sharp');
        document.getElementById('ModSolRechazarBandeja').setAttribute('class', 'botonDeshabilitado gris gris_sharp');
      }
      else {
        botonModSolAceptarBloqueado = false;
        document.getElementById('ModSolAprobarBandeja').setAttribute('class', 'boton darkblue sharp');
        document.getElementById('ModSolRechazarBandeja').setAttribute('class', 'boton darkblue sharp');
        if (!permisoRechazarSolicitud) {
          document.getElementById('ModSolRechazarBandeja').setAttribute('class', 'botonDeshabilitado gris gris_sharp');
        }
      }

      document.getElementById('ModSolIndSeleccionado').setAttribute('readonly', true);
      document.getElementById('ModSolValMontoAcomAgente').setAttribute('readonly', false);

      CargarTablaCotizacionesOficialesBandejaMovil(Solicitud.Cotizaciones, SolicitudEscenario.NumCotizacionElegida, true, true);

      //sI HAY ACCESO, VISUALIZAR
      if (document.getElementById('HGrupoEspeciales').value == "TRUE") {
        CargarTablaTraDefault(numSolicitud);

        const fecCotizacion = new Date(Solicitud.FechaCotizacion).toString("dd/MM/yyyy");
        CargarTablaTasaMaximaTraMinima(numSolicitud, fecCotizacion);
      }

      selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
      $clock.countdown(selectedDate.toString());
    } catch (error) {
      console.error('Error al cargar la información de la solicitud:', error);
      $('#MCMIcono').attr('class', 'error');
      $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
      $('#ModalCuadroMensaje').dialog({ title: 'Error' });
      $('#ModalCuadroMensaje').dialog('open');

      $('#ModalSolicitud').dialog('close');
    }
  },

  renderizarValMontoAcom: async function (numSolicitud, acom, cotizacion) {
    try {
      const response = await ApiCotizadorRV.ObtenerMontoAcom(numSolicitud, acom, cotizacion);
      const montoAcom = Number(response.monto).toFixed(2);
      document.getElementById('ModSolValMontoAcomAgente').value = formatearMonto(montoAcom);
    } catch (error) {
      console.error('Error al cargar el monto de acom:', error);
      $("#MCMIcono").attr("class", "error");
      $("#MCMContenedor").html("Ha ocurrido un error al cargar el Monto A.");
      $("#ModalCuadroMensaje").dialog({ title: "Error" });
      $("#ModalCuadroMensaje").dialog("open");
    }
  },

  renderizarSolicitudEscenario: async function (numSolicitud) {
    try {
      SolicitudEscenario = await ApiSolicitudesCambio.ObtenerDatosSolicitudEscenario(numSolicitud);
      console.log({ SolicitudEscenario });

      document.getElementById('ModSolNroSolicitud').innerHTML = SolicitudEscenario.NumSolicitud;
      document.getElementById('ModSolNroMeller').innerHTML = SolicitudEscenario.NumOperacion;
      document.getElementById('ModSolACOM').value = formatearMonto2(SolicitudEscenario.PjeAumentoComision || 0);

      document.getElementById('ModSolDCOM').value = formatearMonto2(SolicitudEscenario.CodPjeCesionComision || 0);

      document.getElementById('ModSolIndSeleccionado').innerHTML = SolicitudEscenario.IndEstadoSeleccion;
      document.getElementById('ModSolIndSeleccionado').value = SolicitudEscenario.IndEstadoSeleccion;
      document.getElementById('ModSolValMontoAcomAgente').value = formatearMonto2(SolicitudEscenario.ValMtoAgenteAcom || 0);
      document.getElementById('ModSolMontoCIC').innerHTML = formatearMonto(SolicitudEscenario.ValTotalCic || 0);
      document.getElementById('ModFecCierre').innerHTML = new Date(SolicitudEscenario.FecCierre).toString("dd/MM/yyyy");
      document.getElementById('ModSolCussp').innerHTML = SolicitudEscenario.Afiliado.CUSPP;
      document.getElementById('ModSolNomAfiliado').innerHTML = SolicitudEscenario.Afiliado.NombreEmpresa;
      document.getElementById('ModSolNumAgente').innerHTML = SolicitudEscenario.Agente.Id;
      document.getElementById('ModSolNomAgente').innerHTML = SolicitudEscenario.Agente.Nombre;

      $('#ModSolCargando').fadeOut();
      document.getElementById('ModSolACOM').focus();
      return SolicitudEscenario;
    } catch (error) {
      console.error('Error al cargar la información de la solicitud escenario:', error);
      $('#MCMIcono').attr('class', 'error');
      $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
      $('#ModalCuadroMensaje').dialog({ title: 'Error' });
      $('#ModalCuadroMensaje').dialog('open');
      $('#ModalSolicitud').dialog('close');
    }
  },

  setup: function () {
    window.BandejaFlujoCotizacionModificar = BandejaFlujoCotizacionModificar;
  }
}

$(document).ready(function () {
  BandejaFlujoCotizacionModificar.setup();
});
