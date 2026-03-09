$(document).ready(function () {

    $('#date-range').dateRangePicker({})

    $("#table_Consentimiento_RV").DataTable({ responsive: !0, lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todo"]], scrollY: 200, scrollX: !0 })

    $("#table_Consentimiento_RP").DataTable({ responsive: !0, lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todo"]], scrollY: 200, scrollX: !0 })

    $("#table_Firma_Digital_RV").DataTable({ responsive: !0, lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todo"]], scrollY: 200, scrollX: !0 })

    $("#table_Firma_Digital_RP").DataTable({ responsive: !0, lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todo"]], scrollY: 200, scrollX: !0 })

    $("#table_Poliza_RV").DataTable({ responsive: !0, lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todo"]], scrollY: 200, scrollX: !0 })

    $("#table_Poliza_RP").DataTable({ responsive: !0, lengthMenu: [[10, 25, 50, -1], [10, 25, 50, "Todo"]], scrollY: 200, scrollX: !0 })

    CargarTablaConsentimiento();

    CargarTablaFirmaDigital();

    CargarTablaPoliza();

});

function CargarTablaConsentimiento() {

    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        rangoFecha: $('#date-range').val()
    }

    $.ajax({
        type: 'POST',
        url: 'DashboardCntoFD.aspx/CargarTablaConsentimiento',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: JSON.stringify(params),
        success: function (data) {

            var table = $('#table_Consentimiento_RV').DataTable();
            var rows = table.rows().remove().draw();

            var table = $('#table_Consentimiento_RP').DataTable();
            var rows = table.rows().remove().draw();

            //CHART
            var sum = function (a, b) { return a + b };

            if (data.d.length > 0) {

                var contador;

                ////CHART
                //var sum = function (a, b) { return a + b };

                if (data.d[0].length > 0) {

                    var tablaConsentimientoRV = $('#table_Consentimiento_RV').DataTable();

                    for (contador = 0; contador < data.d[0].length; contador++) {
                        //fila = "<tr> ";
                        //fila += "<td>" + data.d[i].gls_num_identificacion + "</td>";
                        //fila += "<td>" + data.d[i].gls_nombres + "</td>";
                        //fila += "<td>" + data.d[i].fec_primer_envio + "</td>";
                        //fila += "<td>" + data.d[i].fec_ultimo_envio + "</td>";
                        //fila += "<td>" + data.d[i].ind_consentimiento + "</td>";
                        //fila += "<td>" + data.d[i].ind_consentimiento + "</td>";
                        //fila += "<td>" + data.d[i].gls_nombres_agente + "</td>";
                        //fila += " </tr>";

                        //$('#page-length-option').append(fila);

                        tablaConsentimientoRV.row.add([
                            data.d[0][contador].gls_num_identificacion,
                            data.d[0][contador].gls_nombres,
                            data.d[0][contador].gls_num_cuspp,
                            FormatoFecha(data.d[0][contador].fec_primer_envio),
                            FormatoFecha(data.d[0][contador].fec_ultimo_envio),
                            data.d[0][contador].ind_consentimiento,
                            data.d[0][contador].val_cant_envios_consentimiento,
                            data.d[0][contador].gls_nombres_agente,
                            data.d[0][contador].gls_nombres_supervisor,
                            data.d[0][contador].gls_nombres_jefe
                        ]).draw(false);

                    }

                    //RV CHART
                    var dataCTORV = {
                        series: [data.d[0][0].val_cant_RV_S, data.d[0][0].val_cant_RV_N]
                    };

                    //var labels = ['Firmados: ' + data.d[0][0].val_cant_RV_S, 'No Firmados: ' + data.d[0][0].val_cant_RV_N];
                    var labels = ['Fdo.: ', 'No Fdo.: '];

                    //new Chartist.Pie('#cntoRV-chart', dataCTORV, {
                    //    labelInterpolationFnc: function (value) {
                    //        return Math.round(value / dataCTORV.series.reduce(sum) * 100) + '%';
                    //    }
                    //});

                    new Chartist.Pie('#cntoRV-chart', dataCTORV, {
                        labelInterpolationFnc: function (value, idx) {
                            var percentage = Math.round(value / dataCTORV.series.reduce(sum) * 100) + '%';
                            return labels[idx] + ' ' + percentage;
                        }
                    });

                    $('#caption-cntoRV').text('La etiqueta verde claro representa a los consentimientos firmados, mientras que el rojo claro representa a los consentimientos no firmados.')

                    $('#cntoRV-chart').show()
                }
                else {
                    //RV CHART
                    var dataCTORV = {
                        series: [0, 0]
                    };

                    new Chartist.Pie('#cntoRV-chart', dataCTORV, {
                        labelInterpolationFnc: function (value) {
                            return Math.round(value / dataCTORV.series.reduce(sum) * 100) + '%';
                        }
                    });

                    $('#caption-cntoRV').text('No se encontró información para el rango de fechas')

                    $('#cntoRV-chart').hide()
                }

                if (data.d[1].length > 0) {

                    var tablaConsentimientoRP = $('#table_Consentimiento_RP').DataTable();

                    for (contador = 0; contador < data.d[1].length; contador++) {
                        tablaConsentimientoRP.row.add([
                            data.d[1][contador].gls_num_identificacion,
                            data.d[1][contador].gls_nombres,
                            data.d[1][contador].gls_num_cuspp,
                            FormatoFecha(data.d[1][contador].fec_primer_envio),
                            FormatoFecha(data.d[1][contador].fec_ultimo_envio),
                            data.d[1][contador].ind_consentimiento,
                            data.d[1][contador].val_cant_envios_consentimiento,
                            data.d[1][contador].gls_nombres_agente,
                            data.d[1][contador].gls_nombres_supervisor,
                            data.d[1][contador].gls_nombres_jefe
                        ]).draw(false);

                    }

                    //RP CHART
                    var dataCTORP = {
                        series: [data.d[1][0].val_cant_RP_S, data.d[1][0].val_cant_RP_N]
                    };

                    var labels = ['Fdo.: ', 'No Fdo.: '];

                    new Chartist.Pie('#cntoRP-chart', dataCTORP, {
                        labelInterpolationFnc: function (value, idx) {
                            var percentage = Math.round(value / dataCTORP.series.reduce(sum) * 100) + '%';
                            return labels[idx] + ' ' + percentage;
                        }
                    });

                    $('#caption-cntoRP').text('La etiqueta verde claro representa a los consentimientos firmados, mientras que el rojo claro representa a los consentimientos no firmados.')

                    $('#cntoRP-chart').show()
                }
                else {
                    //RP CHART
                    var dataCTORP = {
                        series: [0, 0]
                    };

                    new Chartist.Pie('#cntoRP-chart', dataCTORP, {
                        labelInterpolationFnc: function (value) {
                            return Math.round(value / dataCTORP.series.reduce(sum) * 100) + '%';
                        }
                    });

                    $('#caption-cntoRP').text('No se encontró información para el rango de fechas')

                    $('#cntoRP-chart').hide()
                }

            }
            else {
                //$("#table_Consentimiento_RV > tbody").remove();

                //$("#table_Consentimiento_RV").empty();
                //$("#table_Consentimiento_RP").empty();

                //var table = $('#table_Consentimiento_RV').DataTable();
                //var rows = table.rows().remove().draw();

                //var table = $('#table_Consentimiento_RP').DataTable();
                //var rows = table.rows().remove().draw();

                ////CHART
                //var sum = function (a, b) { return a + b };

                //RV CHART
                var dataCTORV = {
                    series: [0, 0]
                };

                new Chartist.Pie('#cntoRV-chart', dataCTORV, {
                    labelInterpolationFnc: function (value) {
                        return Math.round(value / dataCTORV.series.reduce(sum) * 100) + '%';
                    }
                });

                $('#caption-cntoRV').text('No se encontró información para el rango de fechas')

                $('#cntoRV-chart').hide()

                //RP CHART
                var dataCTORP = {
                    series: [0, 0]
                };

                new Chartist.Pie('#cntoRP-chart', dataCTORP, {
                    labelInterpolationFnc: function (value) {
                        return Math.round(value / dataCTORP.series.reduce(sum) * 100) + '%';
                    }
                });

                $('#caption-cntoRP').text('No se encontró información para el rango de fechas')

                $('#cntoRP-chart').hide()
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
        }
    });

}

function CargarTablaFirmaDigital() {

    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        rangoFecha: $('#date-range').val()
    }

    $.ajax({
        type: 'POST',
        url: 'DashboardCntoFD.aspx/CargarTablaFirmaDigital',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: JSON.stringify(params),
        success: function (data) {

            var table = $('#table_Firma_Digital_RV').DataTable();
            var rows = table.rows().remove().draw();

            var table = $('#table_Firma_Digital_RP').DataTable();
            var rows = table.rows().remove().draw();

            //CHART
            var sum = function (a, b) { return a + b };

            if (data.d.length > 0) {

                var contador;

                if (data.d[0].length > 0) {

                    var tablaFirmaDigitalRV = $('#table_Firma_Digital_RV').DataTable();

                    for (contador = 0; contador < data.d[0].length; contador++) {
                        tablaFirmaDigitalRV.row.add([
                            data.d[0][contador].gls_identificador,
                            data.d[0][contador].nombres,
                            data.d[0][contador].gls_num_identificacion,
                            data.d[0][contador].gls_num_cuspp,
                            FormatoFecha(data.d[0][contador].fec_primer_envio),
                            FormatoFecha(data.d[0][contador].fec_ultimo_envio),
                            data.d[0][contador].ind_consentimiento,
                            data.d[0][contador].val_cant_envios_consentimiento,
                            data.d[0][contador].nom_agente,
                            data.d[0][contador].gls_nombres_supervisor,
                            data.d[0][contador].gls_nombres_jefe
                        ]).draw(false);
                    }

                    //RV CHART
                    var dataFDRV = {
                        series: [data.d[0][0].val_cant_RV_S, data.d[0][0].val_cant_RV_N]
                    };

                    var labels = ['Fdo.: ', 'No Fdo.: '];

                    new Chartist.Pie('#fdRV-chart', dataFDRV, {
                        labelInterpolationFnc: function (value, idx) {
                            var percentage = Math.round(value / dataFDRV.series.reduce(sum) * 100) + '%';
                            return labels[idx] + ' ' + percentage;
                        }
                    });

                    $('#caption-fdRV').text('La etiqueta verde claro representa a los consentimientos firmados, mientras que el rojo claro representa a los consentimientos no firmados.')

                    $('#fdRV-chart').show()
                }
                else {
                    //RV CHART
                    var dataFDRV = {
                        series: [0, 0]
                    };

                    new Chartist.Pie('#fdRV-chart', dataFDRV, {
                        labelInterpolationFnc: function (value, idx) {
                            return percentage = Math.round(value / dataFDRV.series.reduce(sum) * 100) + '%';
                        }
                    });

                    $('#caption-fdRV').text('No se encontró información para el rango de fechas')

                    $('#fdRV-chart').hide()
                }

                if (data.d[1].length > 0) {

                    var tablaFirmaDigitalRP = $('#table_Firma_Digital_RP').DataTable();

                    for (contador = 0; contador < data.d[1].length; contador++) {
                        tablaFirmaDigitalRP.row.add([
                            data.d[1][contador].gls_identificador,
                            data.d[1][contador].nombres,
                            data.d[1][contador].gls_num_identificacion,
                            data.d[1][contador].gls_num_cuspp,
                            FormatoFecha(data.d[1][contador].fec_primer_envio),
                            FormatoFecha(data.d[1][contador].fec_ultimo_envio),
                            data.d[1][contador].ind_consentimiento,
                            data.d[1][contador].val_cant_envios_consentimiento,
                            data.d[1][contador].nom_agente,
                            data.d[1][contador].gls_nombres_supervisor,
                            data.d[1][contador].gls_nombres_jefe
                        ]).draw(false);
                    }

                    //RP CHART
                    var dataFDRP = {
                        series: [data.d[1][0].val_cant_RP_S, data.d[1][0].val_cant_RP_N]
                    };

                    var labels = ['Fdo.: ', 'No Fdo.: '];

                    new Chartist.Pie('#fdRP-chart', dataFDRP, {
                        labelInterpolationFnc: function (value, idx) {
                            var percentage = Math.round(value / dataFDRP.series.reduce(sum) * 100) + '%';
                            return labels[idx] + ' ' + percentage;
                        }
                    });

                    $('#caption-fdRP').text('La etiqueta verde claro representa a los consentimientos firmados, mientras que el rojo claro representa a los consentimientos no firmados.')

                    $('#fdRP-chart').show()
                }
                else {
                    //RP CHART
                    var dataFDRP = {
                        series: [0, 0]
                    };

                    new Chartist.Pie('#fdRP-chart', dataFDRP, {
                        labelInterpolationFnc: function (value, idx) {
                            return percentage = Math.round(value / dataFDRP.series.reduce(sum) * 100) + '%';
                        }
                    });

                    $('#caption-fdRP').text('No se encontró información para el rango de fechas')

                    $('#fdRP-chart').hide()
                }

            }
            else {

                ////CHART
                //var sum = function (a, b) { return a + b };

                //RV CHART
                var dataFDRV = {
                    series: [0, 0]
                };

                new Chartist.Pie('#fdRV-chart', dataFDRV, {
                    labelInterpolationFnc: function (value, idx) {
                        return percentage = Math.round(value / dataFDRV.series.reduce(sum) * 100) + '%';
                    }
                });

                $('#caption-fdRV').text('No se encontró información para el rango de fechas')

                $('#fdRV-chart').hide()

                //RP CHART
                var dataFDRP = {
                    series: [0, 0]
                };

                new Chartist.Pie('#fdRP-chart', dataFDRP, {
                    labelInterpolationFnc: function (value, idx) {
                        return percentage = Math.round(value / dataFDRP.series.reduce(sum) * 100) + '%';
                    }
                });

                $('#caption-fdRP').text('No se encontró información para el rango de fechas')

                $('#fdRP-chart').hide()
            }

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
        }
    });

}

function CargarTablaPoliza() {

    var params = {
        tokenUsuario: $('#TokenUsuario').val(),
        rangoFecha: $('#date-range').val()
    }

    $.ajax({
        type: 'POST',
        url: 'DashboardCntoFD.aspx/CargarTablaPoliza',
        contentType: "application/json; charset=iso-8859-1",
        dataType: 'json',
        data: JSON.stringify(params),
        success: function (data) {

            var table = $('#table_Poliza_RV').DataTable();
            var rows = table.rows().remove().draw();

            var table = $('#table_Poliza_RP').DataTable();
            var rows = table.rows().remove().draw();

            if (data.d.length > 0) {

                var contador;

                if (data.d[0].length > 0) {

                    var tablaPolizaRV = $('#table_Poliza_RV').DataTable();

                    for (contador = 0; contador < data.d[0].length; contador++) {
                        tablaPolizaRV.row.add([
                            data.d[0][contador].gls_identificador,
                            data.d[0][contador].nombres,
                            data.d[0][contador].gls_num_identificacion,
                            data.d[0][contador].gls_num_cuspp,
                            data.d[0][contador].num_poliza,
                            FormatoFecha(data.d[0][contador].fec_envio),
                            data.d[0][contador].val_cant_envios_consentimiento,
                            data.d[0][contador].nom_agente,
                            data.d[0][contador].gls_nombres_supervisor,
                            data.d[0][contador].gls_nombres_jefe
                        ]).draw(false);
                    }

                }

                if (data.d[1].length > 0) {

                    var tablaPolizaRP = $('#table_Poliza_RP').DataTable();

                    for (contador = 0; contador < data.d[1].length; contador++) {
                        tablaPolizaRP.row.add([
                            data.d[1][contador].gls_identificador,
                            data.d[1][contador].nombres,
                            data.d[1][contador].gls_num_identificacion,
                            data.d[1][contador].gls_num_cuspp,
                            data.d[1][contador].num_poliza,
                            FormatoFecha(data.d[1][contador].fec_envio),
                            data.d[1][contador].val_cant_envios_consentimiento,
                            data.d[1][contador].nom_agente,
                            data.d[1][contador].gls_nombres_supervisor,
                            data.d[1][contador].gls_nombres_jefe
                        ]).draw(false);
                    }

                }

            }

            $('.preloader-background').fadeOut('slow');

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                /* Sesión caducada */
                document.location.reload(true);
            }
        }
    });

}

function FormatoFecha(fecha) {

    if (fecha == null) {
        return fecha;
    }
    else {
        var dateString = fecha.substr(6);
        var currentTime = new Date(parseInt(dateString));
        var month = currentTime.getMonth() + 1;
        var day = currentTime.getDate();
        var year = currentTime.getFullYear();

        if (month < 10) {
            month = "0" + month;
        }
        if (day < 10) {
            day = "0" + day;
        }
        var date = day + "/" + month + "/" + year;
    }

    return "" + date;
};
