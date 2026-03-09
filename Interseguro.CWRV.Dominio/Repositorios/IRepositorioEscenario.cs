//<SRIINI06326>
using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System.Data;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioEscenario
    {
        void Registrar(string xmlData, string usuario);
        void Eliminar(string idSolicitud, string usuario);
        DataSet ObtenerDatosReporte(string idSolicitud, string usuario); 
    }
}
//<SRIFIN06326>