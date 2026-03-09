using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioTipoMovimiento : IRepositorio<TipoMovimiento>
    {

        List<RolAzmanTipoMovimiento> ObtenerTipoMovimientoPorRolAzman(string codRol);
        List<CotizacionMovimiento> ObtenerCotizacionTipoMovimientoPorSolicitud(string numSolicitud);

        //void Registrar(TipoMovimiento entity);
        //void Actualizar(TipoMovimiento entity);
        //void Eliminar(TipoMovimiento entity);
        //TipoMovimiento ObtenerPorId(long Id);
        //TipoMovimiento ObtenerPorId(string Id);
        //List<TipoMovimiento> Listar();

    }
}
