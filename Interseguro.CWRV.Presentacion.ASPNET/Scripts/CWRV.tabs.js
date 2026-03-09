$(document).ready(function () {
    /* Pestañas */
    $('#Pestanhas li').attr('class', '');
    $('#Pestanhas div[id^=Pestanha]:gt(0)').hide();
    $('#Pestanha' + $('#PestanhaActiva').val()).show();
    $('#Pes' + $('#PestanhaActiva').val()).attr('class', 'seleccionado');
    
    /* Pestañas Mantenedor de Solicitud */
    $('#ManSolPestanhas li').attr('class', '');
    $('#ManSolPestanhas div[id^=ManSolPestanha]:gt(0)').hide();
    $('#ManSolPestanha' + $('#ManSolPestanhaActiva').val()).show();
    $('#ManSolPes' + $('#ManSolPestanhaActiva').val()).attr('class', 'seleccionado');

    /* Función que cambia de pestaña */
    $('#ManSolPestanhas li').live('click', function () {
        $('#ManSolPestanhas li').attr('class', '');
        $('#ManSolPestanhas div[id^=ManSolPestanha]').hide();
        $('#ManSolPestanha' + $(this).data('pestanha')).show();
        $(this).attr('class', 'seleccionado');
    });

    ////$("#ManSolPestanhas li").click(function () {
    ////    $('#ManSolPestanhas li').attr('class', '');
    ////    $('#ManSolPestanhas div[id^=ManSolPestanha]').hide();
    ////    $('#ManSolPestanha' + $(this).data('pestanha')).show();
    ////    $(this).attr('class', 'seleccionado');
    ////});

});
