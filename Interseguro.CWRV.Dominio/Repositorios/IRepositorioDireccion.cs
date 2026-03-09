using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioDireccion: IRepositorio<Direccion>
    {
        List<Direccion> Listar(string cuspp);
        Direccion ObtenerDatos(int idDireccion);

        void ActualizarDireccionSolicitud(string numCuspp, string numSolicitud, string usuario);
        List<Direccion> Listar(string numSolicitud, string idDireccion, string usuario);
    }
}
