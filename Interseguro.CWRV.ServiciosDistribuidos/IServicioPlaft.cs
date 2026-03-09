using Interseguro.CWRV.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Interseguro.CWRV.ServiciosDistribuidos
{
    [ServiceContract]
    public interface IServicioPlaft
    {

        [OperationContract]
        //[WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, UriTemplate = "plaft/solicitar-acceso")]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "plaft/solicitar-acceso")]
        Acceso SolicitarAcceso();

        [OperationContract]
        [WebInvoke(Method = "POST",ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest, UriTemplate = "plaft/actualizar-propuesta/")]
        RespuestaPlaft ActualizarSolicitud(string token, string num_solicitud, int cod_estado_plaft, string gls_observacion);

    }
}
