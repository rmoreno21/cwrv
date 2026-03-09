using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioEmisionPoliza: IRepositorio<EmisionPoliza>
    {
        EmisionPoliza EmitirPoliza(string num_solicitud, int num_poliza, string dig_poliza);

        EmisionPoliza EmitirPolizaIFP(string num_solicitud, int num_poliza, string dig_poliza);
    }
}
