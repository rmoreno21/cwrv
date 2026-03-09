//<SRIINI06326>
using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
//<INIGTI_4022>
using System;
//<FINGTI_4022>

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioDescuentoComision
    {
        List<PorcentajeComision> ListarDescuentos(DateTime fechaCotizacion);
    }
}
//<SRIFIN06326>