const CerrarSolicitud = {
  ValidarCierre: async function (idSolicitud, fechaSolicitud, estadoSolicitud) {
    $("#MCIcono").attr("class", "cargando");
    $("#MCContenedor").html("Cargando la información de la solicitud, por favor espere un momento...");
    $("#ModalCotizando").dialog({ title: "Cargando" });
    $("#ModalCotizando").dialog("open");

    var validacion = false;

    if ($('#CorreoElectronico_RP').val() == "") {
      $('#MCATablaEliminar').val('cierre');
      $("#MCAIcono").attr("class", "advertencia");
      $("#MCAContenedor").html("Debe ingresar el correo electronico del titular 'En el vtiger' para continuar con el proceso");
      $("#ModalCuadroAdvertencia").dialog({ title: "Validación Email" });
      $("#ModalCuadroAdvertencia").dialog("open");
      $("#ModalCotizando").dialog("close");
      validacion = true;
    }

    if ($('#EstadoCivil_RP').val() == "0") {
      $('#MCATablaEliminar').val('cierre');
      $("#MCAIcono").attr("class", "advertencia");
      $("#MCAContenedor").html("Debe seleccionar el Estado Civil del Afiliado en la pestaña 'Datos del Afiliado'");
      $("#ModalCuadroAdvertencia").dialog({ title: "Validación" });
      $("#ModalCuadroAdvertencia").dialog("open");
      $("#ModalCotizando").dialog("close");
      validacion = true;
    }

    if (estadoSolicitud == 0) {
      if ($('#HConsentimiento').val() != "OK") {
        $('#MCATablaEliminar').val('cierre');
        $("#MCAIcono").attr("class", "advertencia");
        $("#MCAContenedor").html("No se puede cerrar la solicitud " + idSolicitud + " porque el cliente no ha brindado su consentimiento para el tratamiento de datos personales, puede enviarle en enlace de consentimiento en la pestaña de Datos del Afiliado");
        $("#ModalCuadroAdvertencia").dialog({ title: "Validación" });
        $("#ModalCuadroAdvertencia").dialog("open");
        $("#ModalCotizando").dialog("close");
        validacion = true;
      }
    }
    if (validacion == false) {
      try {
        const response = await ApiCotizadorIFP.ValidarVigencia(idSolicitud);
        console.log('respuesta de la validacion de vigencia', response);
        if (response.Estado == "OK") {
          window.location.href = "SeleccionSolicitud.aspx?origen=cotizador&modSolModo=CERRAR&idSolicitud=" + idSolicitud + "&fechaSolicitud=" + fechaSolicitud;
        }
        else if (response.Estado == "ERROR") {
          $("#MCMIcono").attr("class", response.Icono);
          $("#MCMContenedor").html(response.Mensaje);
          $("#ModalCuadroMensaje").dialog({ title: response.Titulo });
          $("#ModalCuadroMensaje").dialog("open");
          $("#ModalCotizando").dialog("close");
        }
        Utilitarios.ReiniciarTimeout();
      } catch (error) {
        $('#MCMIcono').attr('class', 'error');
        $('#MCMContenedor').html('Ha ocurrido un error al procesar la simulación.');
        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
        $('#ModalCuadroMensaje').dialog('open');
        $("#ModalCotizando").dialog("close");
      }
    }
  },
  setup: function () {
    window.CerrarSolicitud = CerrarSolicitud;
  }
}

$(document).ready(function () {
  CerrarSolicitud.setup();
});
