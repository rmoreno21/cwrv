using System;
using System.ServiceModel;
using Interseguro.CWRV.Dominio.Entidades;

namespace Interseguro.CWRV.ServiciosDistribuidos
{
    [ServiceContract]
    public interface IServicioCWRV_Publico
    {
        [OperationContract]
        Respuesta CotizarDifTRAPublic(ref Solicitud solicitud, string usuario);

        [OperationContract]
        Respuesta CotizarPublic(string numSolicitud, DateTime fechaCotizacion, string usuario);

        [OperationContract]
        RespuestaCotizacion CotizarPublicV2(string numSolicitud, DateTime fechaCotizacion, string usuario);

        [OperationContract]
        Respuesta RegistrarPolizaIFPADMWRPublic(string num_solicitud, string usuario, GrupoFamiliar grup_fam);

        [OperationContract]
        Respuesta RegistrarPolizaADMWRPublic(string num_solicitud, string usuario, GrupoFamiliar grup_fam);

        [OperationContract]
        Respuesta actualizarPolizaSMEAdmwrPublic(string gls_poliza, int codigo_SME, string usuario);


    }
}
