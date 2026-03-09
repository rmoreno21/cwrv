using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioCargaLocalidadVCTP : IRepositorio<CargaLocalidadVCTP>
    {
        bool Validar(DateTime fecPeriodo, string codUsuario);

    }
}
