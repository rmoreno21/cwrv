using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioAporteAdicional : IRepositorio<AporteAdicional>
    {
        AporteAdicional ObtenerDatos(string CUSPP);

        void Actualizar(AporteAdicional aporte, string usuario);

        void Eliminar(AporteAdicional entity, string usuario);
    }
}
