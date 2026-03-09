$(document).ready(function () {

    //$('#date-range').live('change', function () {
    //    alert('cambio');
    //});

    $('.apply-btn').on('click', function () {
        $('.preloader-background').fadeIn();

        CargarTablaConsentimiento();
        CargarTablaFirmaDigital();
        CargarTablaPoliza();
    });

    $('.consentimientoRV').on("click", function () {
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            rangoFecha: $('#date-range').val(),
            idProceso: '1'
        }

        $.ajax({
            type: 'POST',
            url: 'DashboardCntoFD.aspx/TrazabilidadEnvio',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: JSON.stringify(params),
            success: function (data) {

                if (data.d.length > 0) {

                    window.open('../Plantilla/' + data.d, '_blank');

                    setTimeout(function () {
                        var params = {
                            nombreArchivo: data.d
                        }

                        $.ajax({
                            type: 'POST',
                            url: 'DashboardCntoFD.aspx/EliminarArchivoTrazabilidad',
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: 'json',
                            data: JSON.stringify(params),
                            success: function (data) {

                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                    /* Sesión caducada */
                                    document.location.reload(true);
                                }
                            }
                        });
                    }, 5000);

                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
            }
        });

    });


    $('.consentimientoRP').on("click", function () {
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            rangoFecha: $('#date-range').val(),
            idProceso: '2'
        }

        $.ajax({
            type: 'POST',
            url: 'DashboardCntoFD.aspx/TrazabilidadEnvio',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: JSON.stringify(params),
            success: function (data) {

                if (data.d.length > 0) {

                    window.open(' ../Plantilla/' + data.d, '_blank');

                    setTimeout(function () {
                        var params = {
                            nombreArchivo: data.d
                        }

                        $.ajax({
                            type: 'POST',
                            url: 'DashboardCntoFD.aspx/EliminarArchivoTrazabilidad',
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: 'json',
                            data: JSON.stringify(params),
                            success: function (data) {

                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                    /* Sesión caducada */
                                    document.location.reload(true);
                                }
                            }
                        });
                    }, 5000);

                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
            }
        });

    });


    $('.firmadigitalRV').on("click", function () {
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            rangoFecha: $('#date-range').val(),
            idProceso: '3'
        }

        $.ajax({
            type: 'POST',
            url: 'DashboardCntoFD.aspx/TrazabilidadEnvio',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: JSON.stringify(params),
            success: function (data) {
                if (data.d.length > 0) {

                    window.open(' ../Plantilla/' + data.d, '_blank');

                    setTimeout(function () {
                        var params = {
                            nombreArchivo: data.d
                        }

                        $.ajax({
                            type: 'POST',
                            url: 'DashboardCntoFD.aspx/EliminarArchivoTrazabilidad',
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: 'json',
                            data: JSON.stringify(params),
                            success: function (data) {

                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                    /* Sesión caducada */
                                    document.location.reload(true);
                                }
                            }
                        });
                    }, 5000);

                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
            }
        });

    });


    $('.firmadigitalRP').on("click", function () {
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            rangoFecha: $('#date-range').val(),
            idProceso: '4'
        }

        $.ajax({
            type: 'POST',
            url: 'DashboardCntoFD.aspx/TrazabilidadEnvio',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: JSON.stringify(params),
            success: function (data) {
                if (data.d.length > 0) {
                    window.open(' ../Plantilla/' + data.d, '_blank');

                    setTimeout(function () {
                        var params = {
                            nombreArchivo: data.d
                        }

                        $.ajax({
                            type: 'POST',
                            url: 'DashboardCntoFD.aspx/EliminarArchivoTrazabilidad',
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: 'json',
                            data: JSON.stringify(params),
                            success: function (data) {

                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                    /* Sesión caducada */
                                    document.location.reload(true);
                                }
                            }
                        });
                    }, 5000);

                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
            }
        });

    });


    $('.polizaRV').on("click", function () {
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            rangoFecha: $('#date-range').val(),
            idProceso: '9'
        }

        $.ajax({
            type: 'POST',
            url: 'DashboardCntoFD.aspx/TrazabilidadEnvio',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: JSON.stringify(params),
            success: function (data) {
                if (data.d.length > 0) {

                    window.open(' ../Plantilla/' + data.d, '_blank');

                    setTimeout(function () {
                        var params = {
                            nombreArchivo: data.d
                        }

                        $.ajax({
                            type: 'POST',
                            url: 'DashboardCntoFD.aspx/EliminarArchivoTrazabilidad',
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: 'json',
                            data: JSON.stringify(params),
                            success: function (data) {

                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                    /* Sesión caducada */
                                    document.location.reload(true);
                                }
                            }
                        });
                    }, 5000);

                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
            }
        });

    });


    $('.polizaRP').on("click", function () {
        var params = {
            tokenUsuario: $('#TokenUsuario').val(),
            rangoFecha: $('#date-range').val(),
            idProceso: '10,11'
        }

        $.ajax({
            type: 'POST',
            url: 'DashboardCntoFD.aspx/TrazabilidadEnvio',
            contentType: "application/json; charset=iso-8859-1",
            dataType: 'json',
            data: JSON.stringify(params),
            success: function (data) {
                if (data.d.length > 0) {
                    window.open(' ../Plantilla/' + data.d, '_blank');

                    setTimeout(function () {
                        var params = {
                            nombreArchivo: data.d
                        }

                        $.ajax({
                            type: 'POST',
                            url: 'DashboardCntoFD.aspx/EliminarArchivoTrazabilidad',
                            contentType: "application/json; charset=iso-8859-1",
                            dataType: 'json',
                            data: JSON.stringify(params),
                            success: function (data) {

                            },
                            error: function (XMLHttpRequest, textStatus, errorThrown) {
                                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                                    /* Sesión caducada */
                                    document.location.reload(true);
                                }
                            }
                        });
                    }, 5000);

                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                if (XMLHttpRequest.status === 401 || XMLHttpRequest.status === 12030) {
                    /* Sesión caducada */
                    document.location.reload(true);
                }
            }
        });

    });


});
