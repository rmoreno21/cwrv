using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;
using System;
namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioGestionVentas : IRepositorio<GestionVentas>
    {
        List<GestionVentas> Listar(string cuspp);
        List<GestionVentas> ConsultarGestionVentas(DateTime fechaInicial, DateTime fechaFinal, int numJefe, int numSuperv, int numAgente, string indCierre, string tipoCotizacion, String codCiaSeguro);

    }
}
