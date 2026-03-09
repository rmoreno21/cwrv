using System;
using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioLote : IRepositorio<Lote>
    {
        List<Lote> Listar(int numero);
        List<Lote> Listar(DateTime fechaCierreInicial, DateTime fechaCierreFinal);
        List<Lote> ListarResultado(int numero);
        List<Lote> ListarResultado(DateTime fechaCierreInicial, DateTime fechaCierreFinal);
    }
}
