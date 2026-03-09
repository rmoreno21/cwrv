using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioCargaControlCdA : IRepositorio<CargaControlCdA>
    {
        bool Validar(DateTime fecPeriodo, string codUsuario);

    }
}
