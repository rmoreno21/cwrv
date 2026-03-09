using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioCuotasTra : IRepositorio<CuotasTra>
    {
        CuotasTra ObtenerCuotasTra(CuotasTra rolCuotas);
        
        List<CuotasTra> ListarCuotasTra(int periodo, int mes);

        void RegistrarCuotas(List<CuotasTra> lstCuotas, string usuario);
    }
}
