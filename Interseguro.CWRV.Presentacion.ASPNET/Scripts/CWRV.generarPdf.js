/**
 * Archivo: CWRV.generarPdf.js
 * Funciones reutilizables para generación de PDFs
 * 
 */

async function generarPdfAdelanto(numeroSolicitud) {
    var params = {
        solicitud: numeroSolicitud
    };

    //fetch('http://localhost:3207/api/v1/solicitud/generar-pdf-adelanto', {
    fetch($('#url_api_reportes').val() + '/solicitud/generar-pdf-adelanto', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json; charset=iso-8859-1',
            'x-username': $('#usuario_actual').val(),
            'x-rol': $('#rol_azman').val()
        },
        body: JSON.stringify(params)
    })
        .then(async response => {
            if (!response.ok) {
                if (response.status === 401 || response.status === 12030) {
                    document.location.reload(true);
                    return;
                }
                throw new Error("Error en la respuesta del servidor");
            }

            // Convertir respuesta a blob
            const blob = await response.blob();

            // Crear URL temporal para el blob
            const url = window.URL.createObjectURL(blob);

            // Crear enlace temporal y hacer click para descargar
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            a.download = 'solicitud-adelanto.pdf';
            document.body.appendChild(a);
            a.click();

            // Limpiar
            window.URL.revokeObjectURL(url);
            document.body.removeChild(a);
        })
        .catch(error => {
            console.error('Error al generar PDF:', error);
            if (error.status === 401 || error.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            } else {
                mostrarErrorPdf('error', 'Ha ocurrido un error al generar el PDF de la solicitud.', 'Error');
            }
        });
}

function mostrarErrorPdf(icono, mensaje, titulo) {
    if (typeof $ !== 'undefined' && $('#MCMIcono').length > 0) {
        $('#MCMIcono').attr('class', icono);
        $('#MCMContenedor').html(mensaje);
        $('#ModalCuadroMensaje').dialog({ title: titulo });
        $('#ModalCuadroMensaje').dialog('open');
    } else {
        alert(titulo + ': ' + mensaje);
    }
}

// Hacer la función disponible globalmente para uso en otros archivos
window.generarPdfAdelanto = generarPdfAdelanto;

// También agregar al namespace CWRV si existe
if (typeof window.CWRV === 'undefined') {
    window.CWRV = {};
}
window.CWRV.generarPdfAdelanto = generarPdfAdelanto;