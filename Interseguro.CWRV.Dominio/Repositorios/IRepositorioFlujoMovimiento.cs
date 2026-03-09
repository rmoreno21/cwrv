using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;


namespace Interseguro.CWRV.Dominio.Repositorios
{
    //<INIGTI_4081>
    public interface IRepositorioFlujoMovimiento : IRepositorio<FlujoMovimiento>
    {
        List<FlujoMovimiento> ObtenerFlujos(DateTime fecCotizacion, string evento, string rol);

        bool ValidaFlujoSolicitudRol(string num_solicitud, string rol, string evento);

        string ObtenerArchivosExistentes(string num_solicitud);

    }
    //<FINGTI_4081>
}
