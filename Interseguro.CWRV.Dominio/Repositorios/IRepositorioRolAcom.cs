using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioRolAcom : IRepositorio<RolAcom>
    {
        //<SRIINI10693>
        //Se creo la interfaz Irepositorio ROLACOM para su implementacion
        //<SRIFIN10693>

        List<RolAcom> ListarRolAcom(RolAcom rolAcom);
        List<RolAcom> ListaAcomEscenario(RolAcom rolAcom);
    }
}
