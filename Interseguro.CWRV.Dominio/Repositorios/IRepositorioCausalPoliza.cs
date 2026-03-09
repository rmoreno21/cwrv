using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioCausalPoliza: IRepositorio<CausalPoliza>
    {
        List<CausalPoliza> ListarCausalPolizaPlus();
    }
}
