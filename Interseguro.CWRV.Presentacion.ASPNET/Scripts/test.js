fetch('http://localhost:3201/api/v1/cotizacion-extra-oficial/insertar', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json; charset=iso-8859-1'
    },
    body: JSON.stringify(params)
})
.then(response => {
    if (!response.ok) {
        // Manejar errores de respuesta HTTP, como 404, 500, etc.
        if (response.status === 401 || response.status === 12030) {
            // Sesión caducada
            document.location.reload(true);
        } else {
            $('#MCMIcono').attr('class', 'error');
            $('#MCMContenedor').html('Ha ocurrido un error al guardar la información de la solicitud.');
            $('#ModalCuadroMensaje').dialog({ title: 'Error' });
            $('#ModalCuadroMensaje').dialog('open');
            $('#ModalCotizando').dialog('close');
        }
        throw new Error('Network response was not ok.');
    }
    return response.json();
})
.then(data => {
    if (data.d.Respuesta.Estado == 'OK') {
        // Imprimir número de solicitud generada
        $('#ModSolNroSolicitud').val(data.d.Id);

        // Cambiar la modal a modo de modificación
        $('#ModSolModo').val('M');

        // Cerrar modal de espera
        $('#ModalCotizando').dialog('close');

        // Mostrar mensaje de éxito
        $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
        $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
        $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
        $('#ModalCuadroMensaje').dialog('open');

        CargarTablaSolicitudes();

        // Cargar la cotización actualizada
        $('#BeneficiariosOriginales').show();
        $('#ManSolPestanhas li:eq(0)').trigger('click');

        $('#ModSolCargando').show();

        var params3 = {
            idSolicitud: $('#ModSolNroSolicitud').val(),
            fecCotizacion: $('#ModSolFechaCotizacion').val()
        }

        LimpiarFormularioSolicitud();

        fetch('Cotizador.aspx/ObtenerDatosSolicitud', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=iso-8859-1'
            },
            body: JSON.stringify(params3)
        })
        .then(response => {
            if (!response.ok) {
                if (response.status === 401 || response.status === 12030) {
                    document.location.reload(true);
                } else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la solicitud.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                throw new Error('Network response was not ok.');
            }
            return response.json();
        })
        .then(data => {
            Solicitud = data.d;
            $('#ModSolNroSolicitud').val(Solicitud.Id);
            $('#ModSolTipoCambio').val(formatearMonto(Solicitud.TipoCambio));
            $('#ModSolTipoPension').val(Solicitud.TipoPension.Id);
            $('#TexModSolTipoPension').html($('#ModSolTipoPension').find(':selected').text());
            $('#ModSolFechaDevengue').val(new Date(+Solicitud.FechaDevengue.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            $('#ModSolFecUltActualizacion').val(new Date(+Solicitud.FechaSolicitud.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            $('#ModSolFechaRecepcion').val(new Date(+Solicitud.FechaRecepcion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            $('#ModSolFechaPlazoAFP').val(new Date(+Solicitud.FechaPlazoAFP.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            $('#ModSolSaldoCIC').val(formatearMonto(Solicitud.SaldoCIC));
            $('#ModSolListaCIC').val(formatearMonto2(Solicitud.SaldoCIC));
            $('#TexModSolListaCIC').html($('#ModSolListaCIC').find(':selected').text());
            $('#ModSolFactorTasa').val(Solicitud.FactorTasa);
            $('#TexModSolFactorTasa').html($('#ModSolFactorTasa').find(':selected').text());
            $('#ModSolFechaCotizacion').val(new Date(+Solicitud.FechaCotizacion.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            $('#ModSolFechaSolicitudPension').val(new Date(+Solicitud.FechaSolicitudPension.replace(/\/Date\((-?\d+)\)\//gi, "$1")).toString("dd/MM/yyyy"));
            $('#ModSolACOM').val(formatearMonto(Solicitud.PorcentajeAumentoComision));
            $('#ModSolDCOM').val(formatearMonto(Solicitud.PorcentajeDescuentoComision));

            //<INIGTI_4022>
            if (Solicitud.Agente.IdNivel == 0) {
                $("#LabModSolNivel").attr('title', '');
            } else {
                $("#LabModSolNivel").attr('title', 'Nivel ' + Solicitud.Agente.IdNivel);
            }
            //<FINGTI_4022>

            //CargarTablaBeneficiarios(null);
            CargarTablaBeneficiarios(Solicitud.Beneficiarios);

            /* Cargar Tabla de Cotizaciones con las cotizaciones obtenidas */
            var params = {
                idTipoPension: $('#ModSolTipoPension').val()
            }
            fetch('Cotizador.aspx/CargarComboProductos', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json; charset=iso-8859-1'
                },
                body: JSON.stringify(params)
            })
            .then(response => {
                if (!response.ok) {
                    if (response.status === 401 || response.status === 12030) {
                        /* Sesión caducada */
                        document.location.reload(true);
                    }
                    else {
                        $('#MCMIcono').attr('class', 'error');
                        $('#MCMContenedor').html('Ha ocurrido un error al cargar la lista de productos.');
                        $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                        $('#ModalCuadroMensaje').dialog('open');
                    }
                    throw new Error('Network response was not ok.');
                }
                return response.json();
            })
            .then(data => {
                CargarTablaCotizaciones(Solicitud.Cotizaciones, $('#ManSolTipoSolicitud').val());

                /*<SRIINI17003>*/
                selectedDate = new Date().valueOf() + parseFloat($("#Timeout").val());
                $clock.countdown(selectedDate.toString());
                /*<SRIFIN17003>*/
            })
            .catch(error => {
                console.error('Error en la segunda llamada Fetch:', error);
            });

            $('#ModSolCargando').fadeOut();
            $('#ModSolSaldoCIC').focus();
            $('#ModSolACOM').focus();
            $('#ModSolDCOM').focus();
            $('#ModSolTipoCambio').focus();
        })
        .catch(error => {
            console.error('Error en la llamada Fetch anidada:', error);
        });
    }
    else if (data.d.Estado == 'TOKEN') {
        CerrarSesionExpirada();
    }
    else {
        $('#ModalCotizando').dialog('close');

        $('#ModalCuadroMensaje').dialog({ title: data.d.Respuesta.Titulo });
        $('#MCMIcono').attr('class', data.d.Respuesta.Icono);
        $('#MCMContenedor').html(data.d.Respuesta.Mensaje);
        if (data.d.Respuesta.Controles != null) {
            if (data.d.Respuesta.Controles[0].length) $('#ModSolTipoCambio').attr('class', data.d.Respuesta.Controles[0]);
            if (data.d.Respuesta.Controles[1].length) $('#ConModSolTipoPension').attr('class', data.d.Respuesta.Controles[1]);
            if (data.d.Respuesta.Controles[2].length) $('#ConModSolCategoria').attr('class', data.d.Respuesta.Controles[2]);
            if (data.d.Respuesta.Controles[3].length) $('#ModSolFechaDevengue').attr('class', data.d.Respuesta.Controles[3]);
            if (data.d.Respuesta.Controles[4].length) $('#ModSolFecUltActualizacion').attr('class', data.d.Respuesta.Controles[4]);
            if (data.d.Respuesta.Controles[5].length) $('#ModSolFechaRecepcion').attr('class', data.d.Respuesta.Controles[5]);
            if (data.d.Respuesta.Controles[6].length) $('#ModSolFechaPlazoAFP').attr('class', data.d.Respuesta.Controles[6]);
            if (data.d.Respuesta.Controles[7].length) $('#ModSolSaldoCIC').attr('class', data.d.Respuesta.Controles[7]);
            if (data.d.Respuesta.Controles[8].length) $('#ConModSolFactorTasa').attr('class', data.d.Respuesta.Controles[8]);
            if (data.d.Respuesta.Controles[9].length) $('#ModSolFechaCotizacion').attr('class', data.d.Respuesta.Controles[9]);
            if (data.d.Respuesta.Controles[10].length) $('#ModSolFechaSolicitudPension').attr('class', data.d.Respuesta.Controles[10]);
            if (data.d.Respuesta.Controles[11].length) $('#ModSolACOM').attr('class', data.d.Respuesta.Controles[11]);
            if (data.d.Respuesta.Controles[12].length) $('#ModSolDCOM').attr('class', data.d.Respuesta.Controles[12]);

            $('#TabCotizaciones select').removeClass('formTextboxGridError');
            if (data.d.Respuesta.Controles.length > 13) {
                var celdaError;
                for (i = 13; i < data.d.Respuesta.Controles.length; i++) {
                    celdaError = data.d.Respuesta.Controles[i].split(',');
                    $('#TabCotizaciones tbody tr:eq(' + celdaError[0] + ') td:eq(' + celdaError[1] + ') select').addClass('formTextboxGridError');
                }
            }
        }
        $('#ModalCuadroMensaje').dialog('open');
    }

    $('#ModSolCargando').fadeOut();
    //ModSolBotonesInactivos = false;
    $('#ModSolAceptar').attr('class', 'boton darkblue sharp');
})
.catch(error => {
    console.error('Error en la llamada Fetch principal:', error);
});