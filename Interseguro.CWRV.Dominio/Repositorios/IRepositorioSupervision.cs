using System;
using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioSupervision : IRepositorio<Supervision>
    {
        List<Supervision> Listar(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros);
        List<Supervision> ListarExcel(string idJefe, string idSupervisor, string idAgente, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar);
    }
}
