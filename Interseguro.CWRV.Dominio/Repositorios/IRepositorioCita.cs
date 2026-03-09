using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;


namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioCita : IRepositorio<Cita>
    {
        List<Cita> Listar(Cita citaIn);
        void Registrar(ref Cita sol);
        void RegistrarExtraoficial(ref Cita entity);
        void Actualizar(Cita sol);
        void Eliminar(Cita sol);
        Cita ObtenerPorId(long Id);
        Cita ObtenerPorId(string Id);
        List<Cita> Listar();
    }
}
