$(function () {
    //<GTI.INI-29372>
    $('#ModalAdvertenciaAporte').dialog({
        autoOpen: false,
        resizable: false,
        width: 500,
        minHeight: 130,
        show: "fade",
        hide: "explode",
        modal: true
    });
    //<GTI.FIN-29372>
    //<SOLINI25621>
    /* Envío de Correo */
    $('#ModalEnvioCorreoCapital').dialog({
        autoOpen: false,
        resizable: false,
        width: 591,
        show: "fade",
        hide: "fade",
        modal: true
    });
    //<SOLFIN25621>

    //<SRI.INI-20322>
    /* Envío correos Simulador*/
    $("#ModalEnvioCorreoSimulador").dialog(
        {
            autoOpen: false,
            resizable: false,
            width: 595,
            show: "fade",
            hide: "fade",
            modal: true
        }

    );
    //<SRI.FIN-20322>

    /* Búsqueda de afiliados */
    $("#ModalBusquedaAfiliados").dialog({
        autoOpen: false,
        resizable: false,
        width: 815,
        show: "fade",
        hide: "fade",
        modal: true
    });

    /* Búsqueda de afiliados Renta Privada */
    $("#ModalBusquedaAfiliados_RP").dialog({
        autoOpen: false,
        resizable: false,
        width: 591,
        show: "fade",
        hide: "fade",
        modal: true
    });

    //<SOLINI26593>
    /* Búsqueda de afiliados*/
    $("#ModalBusquedaAfiliados_RP").dialog({
        autoOpen: false,
        resizable: false,
        width: 815,
        show: "fade",
        hide: "fade",
        modal: true
    });
    //<SOLFIN26593>

    /* Dirección del afiliado */
    $('#ModalDireccion').dialog({
        autoOpen: false,
        resizable: false,
        width: 671,
        show: "fade",
        hide: "fade",
        modal: true
    });

    /* Teléfono del afiliado */
    $('#ModalTelefono').dialog({
        autoOpen: false,
        resizable: false,
        width: 373,
        show: "fade",
        hide: "fade",
        modal: true
    });

    /* Grupo Familiar */
    $('#ModalGrupoFamiliar').dialog({
        autoOpen: false,
        resizable: false,
        width: 815,
        show: "fade",
        hide: "fade",
        modal: true
    });

    /* Grupo Familiar */
    $('#ModalGrupoFamiliar_RP').dialog({
        autoOpen: false,
        resizable: false,
        width: 815,
        show: "fade",
        hide: "fade",
        modal: true
    });

    /* Persona Vinculada */
    $('#ModalGrupoFamiliar_RPP').dialog({
        autoOpen: false,
        resizable: false,
        width: 815,
        show: "fade",
        hide: "fade",
        modal: true
    });

    /* Solicitud */
    $('#ModalSolicitud').dialog({
        autoOpen: false,
        resizable: false,
        width: 915,
        show: "fade",
        hide: "fade",
        modal: true
    });

    //<GTIINI-754>
    /* Reporte Detalle de Cotización */
    $("#ModalElegirFormatoReporte").dialog({
        autoOpen: false,
        resizable: false,
        width: 350,
        show: "fade",
        hide: "fade",
        modal: true
    });
    //<GTIFIN-754>

    /* Flujo Solicitud */
    $('#ModalFlujoSolicitud').dialog({
        autoOpen: false,
        resizable: false,
        width: 915,
        show: "fade",
        hide: "fade",
        modal: true
    });

    /* Envío de Correo */
    $('#ModalEnvioCorreo').dialog({
        autoOpen: false,
        resizable: false,
        width: 591,
        show: "fade",
        hide: "fade",
        modal: true
    });

    /* Ver Actividades */
    $('#ModalActividad').dialog({
        autoOpen: false,
        resizable: false,
        width: 591,
        show: "fade",
        hide: "fade",
        modal: true
    });

    /* Muestra mensajes */
    $('#ModalCuadroMensaje').dialog({
        autoOpen: false,
        resizable: false,
        width: 500,
        minHeight: 130,
        show: "fade",
        hide: "explode",
        modal: true
    });
    $('#ModalCuadroAdvertencia').dialog({
        autoOpen: false,
        resizable: false,
        width: 500,
        minHeight: 130,
        show: "fade",
        hide: "explode",
        modal: true
    });
    $('#ModalCotizando').dialog({
        autoOpen: false,
        resizable: false,
        width: 400,
        minHeight: 90,
        show: "fade",
        hide: "fade",
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            //hide close button.
            $(this).parent().children().children('.ui-dialog-titlebar-close').hide();
        }
    });
    /*<SRIINI20322>*/
    $("#ModalCargandoSolicitudes").dialog({
        autoOpen: false,
        resizable: false,
        width: 380,
        minHeight: 90,
        show: "fade",
        hide: "fade",
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            // hide close button
            $(this).parent().children().children(".ui-dialog-titlebar-close").hide();
            // hide scrollbar
            //$("#ModalCotizandoLote").css("overflow", "hidden");
        }
    });

    $("#ModalGenerandoReporte").dialog({
        autoOpen: false,
        resizable: false,
        width: 380,
        minHeight: 90,
        show: "fade",
        hide: "fade",
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            // hide close button
            $(this).parent().children().children(".ui-dialog-titlebar-close").hide();
        }
    });

    $("#ModalCotizandoLote").dialog({
        autoOpen: false,
        resizable: false,
        width: 500,
        minHeight: 170,
        show: "fade",
        hide: "fade",
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            // hide close button
            $(this).parent().children().children(".ui-dialog-titlebar-close").hide();
            // hide scrollbar
            $("#ModalCotizandoLote").css("overflow", "hidden");
        }
    });

    $("#ModalGuardandoCotizacionesMeler").dialog({
        autoOpen: false,
        resizable: false,
        width: 410,
        minHeight: 90,
        show: "fade",
        hide: "fade",
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            // hide close button
            $(this).parent().children().children(".ui-dialog-titlebar-close").hide();
        }
    });

    $("#ModalGuardandoConfirmaciones").dialog({
        autoOpen: false,
        resizable: false,
        width: 410,
        minHeight: 90,
        show: "fade",
        hide: "fade",
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            // hide close button
            $(this).parent().children().children(".ui-dialog-titlebar-close").hide();
        }
    });
    /*<SRIFIN20322>*/

    $('#ModalGenerandoReporte').dialog({
        autoOpen: false,
        resizable: false,
        width: 480,
        minHeight: 90,
        show: "fade",
        hide: "fade",
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            //hide close button.
            $(this).parent().children().children('.ui-dialog-titlebar-close').hide();
        }
    });

    /*<SRIINI20322>*/
    $("#ModalPensionProyectadaCargando").dialog({
        autoOpen: false,
        resizable: false,
        width: 350,
        minHeight: 90,
        show: "fade",
        hide: "fade",
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            //hide close button.
            $(this).parent().children().children(".ui-dialog-titlebar-close").hide();
        }
    });
    /*<SRIFIN20322>*/

    /* Cerrar Sesión */
    $('#ModalCerrarSesion').dialog({
        autoOpen: false,
        title: 'Cerrar Sesión',
        resizable: false,
        width: 450,
        minHeight: 120,
        show: "fade",
        hide: "explode",
        modal: true
    });

    /*<SRIINI10693>*/
    /* Roles Acom Maximo */
    $('#ModalSeleccionAcom').dialog({
        autoOpen: false,
        resizable: false,
        width: 320,
        minHeight: 90,
        show: "fade",
        hide: "fade",
        modal: true,
        closeOnEscape: false,
        open: function (event, ui) {
            //hide close button.
            $(this).parent().children().children('.ui-dialog-titlebar-close').hide();
        }
    });
    /*<SRIFIN10693>*/
    /*S19*/
    $('#ModalCuadroEnviar').dialog({
        autoOpen: false,
        resizable: false,
        width: 500,
        minHeight: 130,
        show: "fade",
        hide: "explode",
        modal: true
    });

    $('#ModalCuadroEliminarArchivo').dialog({
        autoOpen: false,
        resizable: false,
        width: 500,
        minHeight: 130,
        show: "fade",
        hide: "explode",
        modal: true
    });
    /*S19*/

    $("#ModalConsentimientoAsesoria").dialog({
        autoOpen: false,
        resizable: false,
        width: 700,
        show: "fade",
        hide: "fade",
        modal: true
    });

    $("#ModalConsentimientoAsesoriaSMS").dialog({
        autoOpen: false,
        resizable: false,
        width: 700,
        show: "fade",
        hide: "fade",
        modal: true
    });

    $("#ModalConsentimientoAsesoria_IFP").dialog({
        autoOpen: false,
        resizable: false,
        width: 700,
        show: "fade",
        hide: "fade",
        modal: true
    });

    $("#ModalConsentimientoAsesoria_IFP_SMS").dialog({
        autoOpen: false,
        resizable: false,
        width: 700,
        show: "fade",
        hide: "fade",
        modal: true
    });

    $("#ModalConsentimientoAsesoria_RPP").dialog({
        autoOpen: false,
        resizable: false,
        width: 700,
        show: "fade",
        hide: "fade",
        modal: true
    });

    $("#ModalReenvioConsentimientoAsesoria").dialog({
        autoOpen: false,
        resizable: false,
        width: 700,
        show: "fade",
        hide: "fade",
        modal: true
    });
});
