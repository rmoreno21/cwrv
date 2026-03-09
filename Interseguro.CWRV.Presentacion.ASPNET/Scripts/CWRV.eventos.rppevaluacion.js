$(document).ready(function () {

    $('#TablaSolicitudesContenedor_RPP_Evaluacion .grilla_consultar').live('click', function () {
        
        solicitud = $(this).data('solicitud');
        var fecCotizacion = $(this).parent('td').parent('tr').children().eq(1).html();
        var cuspp = $(this).parent('td').parent('tr').children().eq(2).text();
        var nombre = $(this).parent('td').parent('tr').children().eq(3).text();

        var params = {
            solicitud: solicitud,
            fecCotizacion: fecCotizacion,
            cuspp: cuspp,
            nombre: nombre
        }

        $.ajax({
            type: 'POST',
            url: '../RentaPrivadaPlus/ListadoEvaluacion.aspx/SessionSolicitud',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: $.toJSON(params),
           
            success: function (data) {
                    window.location.href = "../RentaPrivadaPlus/ListadoArchivoDrive.aspx";
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    document.location.reload(true);
                }
                else {
                    $('#MCMIcono').attr('class', 'error');
                    $('#MCMContenedor').html('Ha ocurrido un error al cargar la información de la Solicitud.');
                    $('#ModalCuadroMensaje').dialog({ title: 'Error' });
                    $('#ModalCuadroMensaje').dialog('open');
                }
                
            }
        });

    });
    
});
