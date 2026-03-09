using System;
using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;


namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioSeguimiento : IRepositorio<Seguimiento>
    {
        List<Seguimiento> Listar(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros);
        List<Seguimiento> ListarExcel(string idJefe, string idSupervisor, string idAgente, string cuspp, DateTime fechaInicio, DateTime fechaTermino, int columnaOrdenar, char direccionOrdenar);
    }
}
