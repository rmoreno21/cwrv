$(document).ready(function () {

    CargarTablaSolicitudesEvaluacion();

});

function CargarTablaSolicitudesEvaluacion() {

    $('#TablaSolicitudesContenedor_RPP_Evaluacion').hide();
    $('#TablaSolicitudesCargando_RPP_Evaluacion').show();

    $.ajax({
        type: 'POST',
        url: 'ListadoEvaluacion.aspx/CargarTablaSolicitudesEvaluacion',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        //data: $.toJSON(params),
        success: function (data) {
            //if (data.d != 'TOKEN') {
            $('#TablaSolicitudesCargando_RPP_Evaluacion').hide();
            $('#TablaSolicitudesContenedor_RPP_Evaluacion').show();
            $('#TablaSolicitudesContenedor_RPP_Evaluacion').html($(data.d).find('#ContenidoDinamico').html());
            //}
            //else {
            //	CerrarSesionExpirada();
            //}
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
            else {
                $('#TablaSolicitudesCargando_RPP_Evaluacion').hide();
                $('#TablaSolicitudesError_RPP_Evaluacion').show();
            }
        }
    });

}
