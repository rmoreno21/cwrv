using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioSolicitudEscenario : IRepositorio<SolicitudEscenario>
    {
        List<SolicitudEscenario> Listar(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol);
        SolicitudEscenario ObtenerPorId(string numSolicitud, string codUserName, string codRol);
        void Registrar(ref SolicitudEscenario sol);
        void RegistrarExtraoficial(ref SolicitudEscenario entity);
        void Actualizar(ref SolicitudEscenario entity);
        void Actualizar(SolicitudEscenario sol);
        void Eliminar(SolicitudEscenario sol);
        SolicitudEscenario ObtenerPorId(long Id);
        SolicitudEscenario ObtenerPorId(string numSolicitud);
        List<SolicitudEscenario> Listar();
        //<INIGTI_4081>
        List<SolicitudEscenario> ListarCambios(string numJefe, string numSupervisor, string numAgente, string codUserName, string codRol);
        List<SolicitudEscenario> ListarSolicitudesEmail(string idSolicitudes);
        List<SolicitudEscenario> ListarSolicitudesValidaFlujo(string NumSolicitud, string NumOperacion);
        //<FINGTI_4081>
        //<INIGTI_6556>
        List<SolicitudEscenario> ListarSolicitudesPendientesEmail(string cod_rol);
        //<FINGTI_6556>

        //<GTI.INI-29372>
        void ActualizarEnvioObligatorio(ref SolicitudEscenario entity);
        //<GTI.FIN-29372>
    }
}
