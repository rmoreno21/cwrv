using Interseguro.CWRV.Dominio.Entidades.MotorCalculo;
using System;
using System.Collections.Generic;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioMotorCalculo
    {
        List<JuegoParametros> ObtenerParametrosRPP(string temporalidad, DateTime fechaCotizacion, string origen, string usuario);
    }
}
