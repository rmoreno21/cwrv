using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioGrupoFamiliar: IRepositorio<GrupoFamiliar>
    {
        List<GrupoFamiliar> Listar(string cuspp);
        GrupoFamiliar ObtenerDatos(int idGrupoFamiliar, string num_solicitud);//<INI.GTI_7012_V13>

        void RegistrarBeneficiarios(List<GrupoFamiliar> lstEntity, int idGrupoFamiliar, string tipoPlan);

        void RegistrarPersonaVinculada(GrupoFamiliar entity);
        void ActualizarPersonaVinculada(GrupoFamiliar entity);
        void EliminarPersonaVinculada(int idPersonaVinculada, string usuario);
        List<GrupoFamiliar> ObtenerPersonaVinculada(GrupoFamiliar entity);

        void ActualizarBeneficiariosPNoG(List<GrupoFamiliar> lstEntity);

    }
}
