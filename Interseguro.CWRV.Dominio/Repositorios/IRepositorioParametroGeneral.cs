using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioParametroGeneral
    {
        double obtenerValorComisionAgente(string moneda);
        SDAReporte obtenerPreCubo(int numeroAgente, string cuspp);
        ConfiguracionCorreo ObtenerConfiguracionCorreo(int cod_proceso, DateTime fec_solicitud);
        double ObtenerTipoCambio(string codigo, DateTime fecha, string usuario);
    }
}
