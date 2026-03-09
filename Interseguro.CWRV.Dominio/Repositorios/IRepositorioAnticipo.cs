using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioAnticipo : IRepositorio<Anticipo>
    {
        Anticipo ObtenerDatos(string solicitud);
        Anticipo ObtenerDatosAceptacion(string solicitud, string agente);
        Anticipo ObtenerDatosCondiciones(string solicitud);
        void RegistrarAceptacion(Anticipo entity);
        List<Anticipo> ListarAceptacion(string solicitud, string agente, DateTime? fechaInicio, DateTime? fehaFin, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar);
    }
}
