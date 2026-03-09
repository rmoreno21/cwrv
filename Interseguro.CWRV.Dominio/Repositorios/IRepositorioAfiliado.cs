using System.Collections.Generic;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.Dominio.Repositorios
{
    public interface IRepositorioAfiliado : IRepositorio<Afiliado>
    {
        List<Afiliado> Listar(string apellidoPaterno, string apellidoMaterno, string nombres, int indicePagina, int tamanhoPagina, int columnaOrdenar, char direccionOrdenar, ref int totalRegistros);
        Afiliado ObtenerDatos(string solicitud, string CUSPP, string tipoIdentificacion, string numIdentificacion, string producto);

        void Registrar(Afiliado entity, string usuario, ref string numCUSPP);

        void ActualizarConsentimiento(Afiliado entity, ConsentimientoAsesoria entityConsentimientoAsesoria);

        List<Beneficiario> ListarBeneficiarios(string numSolicitud, string usuario);
        void ActualizarBeneficiario(Beneficiario entity);
        void ActualizarDireccionBeneficiario(BeneficiarioDireccion entity);
        GrupoFamiliar ObtenerDatosCierre(int idGrupoFamiliar, string num_solicitud);
        void EliminarBeneficiarioIFP(string num_solicitud, string tipoPlan, string usuario);
    }
}
