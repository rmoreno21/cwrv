using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interseguro.CWRV.Dominio.Repositorios
{

    public interface IRepositorioProfesion : IRepositorio<Profesion>
    {

        List<Profesion> Listar(string usuario);

    }

}
